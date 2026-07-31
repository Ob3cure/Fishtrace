using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace Fishtrace;

//Should switch to threading pool timer + dispatcherqueue
public static class GlobalRecordTimer
{
    public static int CurrentFrame = 0;
    public static int MaxFrame = 0;
    public static DispatcherTimer Timer = new DispatcherTimer();
    public static event Action? PauseRequest;
    public static event Action? TickRequest;

    public static void Initialize()
    {
        Timer.Tick += Tick;
        Timer.Interval = TimeSpan.FromSeconds(1 / MainPage.TickRate);
    }

    public static void Tick(object? sedner, object e)
    {
        CurrentFrame++;
        if (CurrentFrame > MaxFrame)
        {
            MaxFrame = CurrentFrame;
        }

        MainPage.AnimationRecording.Record(MainPage.MainCharacter);

        if (TickRequest != null)
        {
            TickRequest.Invoke();
        }
    }

    public static void Start()
    {
        Timer.Start();
    }

    public static void Stop()
    {
        Timer.Stop();
    }

    public static void Reset()
    {
        Timer.Stop();
        CurrentFrame = 0;
    }

    public static void RequestPause()
    {
        Timer.Stop();
        PauseRequest?.Invoke();
    }
}