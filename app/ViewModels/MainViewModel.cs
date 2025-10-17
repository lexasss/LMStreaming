using System.Windows;
using System.Windows.Threading;

namespace LMStreamer;

public class MainViewModel
{
    public HandTrackerViewModel HandTracker { get; }
    public TcpServerViewModel TcpServer { get; }
    public StreamerViewModel Streamer { get; }

    public MainViewModel(Dispatcher dispatcher)
    {
        var app = (App)Application.Current;

        HandTracker = new(app.HandTrackingService, dispatcher);
        TcpServer = new(app.TcpServer, dispatcher);
        Streamer = new(app.StreamingService);

        app.TcpServer.Data += TcpServer_Data;
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
