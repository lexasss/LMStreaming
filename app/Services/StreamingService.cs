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
            _tcpServer.Send(e.AsJson());
        });
    }
}
