using Leap;

namespace LMStreamer;

public class HandTrackingService : IDisposable
{
    public bool IsReady => _lm != null;
    public DeviceList Devices => _lm?.Devices ?? [];
    public bool IsConnected => _lm?.IsConnected ?? false;

    public bool IsTracking { get; set; } = false;

    /// <summary>
    /// Maximum distance for the hand to be tracked, in cm
    /// </summary>
    public double MaxTrackingDistance
    {
        get => Properties.Settings.Default.MaxTrackingDistance;
        set
        {
            Properties.Settings.Default.MaxTrackingDistance = value;
            Properties.Settings.Default.Save();
        }
    }

    public event EventHandler<Device>? DeviceAdded;
    public event EventHandler<Device>? DeviceRemoved;
    public event EventHandler<bool>? ConnectionStatusChanged;

    /// <summary>
    /// Reports hand location as 3 vectors: palm, infdex finger tip and middle finger tip.
    /// The coordinate system is as it used to be in the original Leap Motion:
    /// X = left, Y = forward, Z = down
    /// </summary>
    public event EventHandler<HandLocation>? HandData;

    public HandTrackingService()
    {
        try
        {
            _lm = new();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }

        if (_lm == null)
            return;

        _lm.Connect += Lm_Connect;
        _lm.Disconnect += Lm_Disconnect;

        _lm.Device += Lm_Device;
        _lm.DeviceLost += Lm_DeviceLost;
        _lm.DeviceFailure += Lm_DeviceFailure;

        _lm.FrameReady += Lm_FrameReady;

        // Ask for frames even in the background - this is important!
        //_lm.SetPolicy(LeapMotion.PolicyFlag.POLICY_BACKGROUND_FRAMES);
        //_lm.SetPolicy(LeapMotion.PolicyFlag.POLICY_ALLOW_PAUSE_RESUME);

        _lm.ClearPolicy(LeapMotion.PolicyFlag.POLICY_IMAGES);
    }

    public void Connect()
    {
        _lm?.StartConnection();
    }

    public void Dispose()
    {
        _lm?.StopConnection();
        _lm?.Dispose();
        GC.SuppressFinalize(this);
    }


    // Internal

    readonly LeapMotion? _lm = null;

    private void Lm_DeviceFailure(object? sender, DeviceFailureEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"Hand tracking device {e.DeviceSerialNumber} failure: {e.ErrorMessage} ({e.ErrorCode})");
    }

    private void Lm_DeviceLost(object? sender, DeviceEventArgs e) =>
        DeviceRemoved?.Invoke(sender, e.Device);

    private void Lm_Device(object? sender, DeviceEventArgs e) =>
        DeviceAdded?.Invoke(sender, e.Device);

    private void Lm_Disconnect(object? sender, ConnectionLostEventArgs e) =>
        ConnectionStatusChanged?.Invoke(this, false);

    private void Lm_Connect(object? sender, ConnectionEventArgs e) =>
        ConnectionStatusChanged?.Invoke(this, true);

    private void Lm_FrameReady(object? sender, FrameEventArgs e)
    {
        bool handDetected = false;

        int handIndex = -1;

        /*while (handIndex < e.frame.Hands.Count && e.frame.Hands[handIndex].IsLeft) // accept only right hand !! works badly for the top-view setup, thus rejected
        {
            handIndex++;
        }*/

        // find the closest hand
        double minDistance = double.MaxValue;
        for (int i = 0; i < e.frame.Hands.Count; i++)
        {
            var palm = e.frame.Hands[i].PalmPosition / 10;
            var distance = Math.Sqrt(palm.x * palm.x + palm.y * palm.y + palm.z * palm.z);
            if (distance < minDistance && distance < MaxTrackingDistance)
            {
                minDistance = distance;
                handIndex = i;
            }
        }

        if (-1 < handIndex && handIndex < e.frame.Hands.Count)
        {
            handDetected = true;

            var fingers = e.frame.Hands[handIndex].Fingers;

            // convert mm to cm
            var palm = e.frame.Hands[handIndex].PalmPosition / 10;
            var thumb = fingers[0].TipPosition / 10;
            var index = fingers[1].TipPosition / 10;
            var middle = fingers[2].TipPosition / 10;

            HandData?.Invoke(this, new HandLocation(Vector.From(in palm), Vector.From(in thumb), Vector.From(in index), Vector.From(in middle)));
        }

        if (!handDetected)
        {
            HandData?.Invoke(this, new HandLocation());
        }

        // e.frame.Hands[0].Fingers[0..4].TipPosition.x;
        // e.frame.Hands[0].PalmVelocity.Magnitude);
        // e.frame.Hands[0].Fingers[x].TipPosition.DistanceTo(e.frame.Hands[0].Fingers[x + 1].TipPosition);
        // e.frame.Hands[0].PalmNormal.x
    }
}
