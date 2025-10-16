using System.Windows.Threading;

namespace LMStreaming;

public class StreamingService
{
    public StreamingService(
        HandTrackingService handTrackingService,
        TcpServer tcpServer,
        Dispatcher dispatcher)
    {
        _handTrackingService = handTrackingService;
        _tcpServer = tcpServer;
        _dispatcher = dispatcher;

        handTrackingService.HandData += HandTrackingService_HandData;
    }

    // Internal

    readonly HandTrackingService _handTrackingService;
    readonly TcpServer _tcpServer;
    readonly Dispatcher _dispatcher;

    private void HandTrackingService_HandData(object? sender, HandLocation e)
    {
        if (!_handTrackingService.IsTracking)
            return;

        _dispatcher.Invoke(() =>
        {
            if (e.IsEmpty)
                _tcpServer.Send("0 0 0");
            else
                _tcpServer.Send($"{e.Palm.x:F4} {e.Palm.y:F4} {e.Palm.z:F4}");
        });
    }
}
