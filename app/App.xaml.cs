using System.Windows;

namespace LMStreamer;

public partial class App : Application
{
    public HandTrackingService HandTrackingService { get; } = new();
    public TcpServer TcpServer { get; } = new();
    public StreamingService StreamingService { get; }

    public App() : base()
    {
        StreamingService = new StreamingService(HandTrackingService);

        Exit += (s, e) =>
        {
            HandTrackingService.Dispose();
            TcpServer.Dispose();
            StreamingService.Dispose();
        };
    }
}
