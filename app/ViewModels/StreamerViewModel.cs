using System.ComponentModel;

namespace LMStreamer;

public class StreamerViewModel : INotifyPropertyChanged, IDisposable
{
    public int PacketsSent => _streamer.PacketsSent;

    public event PropertyChangedEventHandler? PropertyChanged;

    public StreamerViewModel(StreamingService streamer)
    {
        _streamer = streamer;

        _timer = new System.Timers.Timer(200);
        _timer.AutoReset = true;
        _timer.Elapsed += (s, e) =>
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PacketsSent)));
        };
        _timer.Start();
    }

    public void Dispose()
    {
        _timer.Stop();
        _timer.Dispose();
        GC.SuppressFinalize(this);
    }

    // Internal

    readonly StreamingService _streamer;
    readonly System.Timers.Timer _timer;
}
