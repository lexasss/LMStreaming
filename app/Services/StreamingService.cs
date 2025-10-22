using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Threading;

namespace LMStreamer;

public class StreamingService : IDisposable
{
    public int PacketsSent { get; private set; } = 0;

    public StreamingService(HandTrackingService handTrackingService)
    {
        _handTrackingService = handTrackingService;

        // create UDP client and enable broadcast
        _udpClient = new UdpClient();
        _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
        _targetEndpoint = new IPEndPoint(IPAddress.Broadcast, 8982);

        handTrackingService.HandData += HandTrackingService_HandData;
    }

    // Internal

    readonly HandTrackingService _handTrackingService;
    
    readonly UdpClient _udpClient;
    readonly IPEndPoint _targetEndpoint;

    private void HandTrackingService_HandData(object? sender, HandLocation e)
    {
        if (!_handTrackingService.IsTracking)
            return;

        var payload = e.AsJson();

        try
        {
            var bytes = Encoding.UTF8.GetBytes(payload);
            _udpClient.Send(bytes, bytes.Length, _targetEndpoint);
            PacketsSent++;
        }
        catch (SocketException)
        {
            // ignore send errors for now
        }
    }

    public void Dispose()
    {
        _handTrackingService.HandData -= HandTrackingService_HandData;

        _udpClient?.Close();
        _udpClient?.Dispose();

        GC.SuppressFinalize(this);
    }
}
