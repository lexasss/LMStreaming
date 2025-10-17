using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace LMStreamer;

public class TcpServerViewModel : INotifyPropertyChanged
{
    public ImageSource ServerStatusIcon
    {
        get => field;
        private set
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ServerStatusIcon)));
        }
    } = _iconDisabled;

    public event PropertyChangedEventHandler? PropertyChanged;

    public TcpServerViewModel(TcpServer server, Dispatcher dispatcher)
    {
        _server = server;
        _dispatcher = dispatcher;

        _server.Started += (s, e) =>
        {
            _dispatcher.Invoke(() =>
            {
                ServerStatusIcon = _iconOff;
            });
        };

        _server.ClientConnected += (s, e) =>
        {
            _dispatcher.Invoke(() =>
            {
                ServerStatusIcon = _iconOn;
            });
        };

        _server.ClientDisconnected += (s, e) =>
        {
            _dispatcher.Invoke(() =>
            {
                ServerStatusIcon = _iconOff;
            });
        };
    }

    // Internal
    readonly static ImageSource _iconDisabled = new BitmapImage(new Uri("pack://application:,,,/assets/images/tcp-no.png"));
    readonly static ImageSource _iconOn = new BitmapImage(new Uri("pack://application:,,,/assets/images/tcp-on.png"));
    readonly static ImageSource _iconOff = new BitmapImage(new Uri("pack://application:,,,/assets/images/tcp-off.png"));

    readonly TcpServer _server;
    readonly Dispatcher _dispatcher;
}
