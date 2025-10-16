using System.Windows.Threading;

namespace LMStreaming;

public class MainViewModel
{
    public HandTrackerViewModel HandTracker { get; }
    public TcpServerViewModel TcpServer { get; }

    public MainViewModel(
        HandTrackingService handTrackingService,
        TcpServer tcpServer,
        Dispatcher dispatcher)
    {
        HandTracker = new(handTrackingService, dispatcher);
        TcpServer = new(tcpServer, dispatcher);

        tcpServer.Data += TcpServer_Data;
    }

    // Internal

    private void TcpServer_Data(object? sender, string e)
    {
        if (e == "start")
        {
            HandTracker.IsHandTrackingRunning = true;
        }
        else if (e == "stop")
        {
            HandTracker.IsHandTrackingRunning = false;
        }
    }
}
