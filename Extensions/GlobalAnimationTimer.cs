using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using Windows.System;
using Windows.System.Threading;

namespace Fishtrace;


public static class GlobalAnimationTimer
{
    // public static int CurrentFrame = 0;
    // public static DispatcherTimer Timer = new DispatcherTimer();
    // public static event Action? PauseRequest;
    // public static event Action? TickRequest;

    // public static void Initialize()
    // {
    //     Timer.Tick += Tick;
    //     Timer.Interval = TimeSpan.FromSeconds(1 / MainPage.TickRate);
    // }
    public static int CurrentFrame = 0;
    private static ThreadPoolTimer? Timer;
    private static DispatcherQueue? _dispatcherQueue;

    public static event Action? PauseRequest;
    public static event Action? TickRequest;

    public static void Initialize()
    {
        // 获取 UI 线程的 DispatcherQueue
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    }

    public static void Start()
    {
        if (Timer != null) return;
        // 创建周期为 16 毫秒（约 60 FPS）的定时器
        Timer = ThreadPoolTimer.CreatePeriodicTimer(
            new TimerElapsedHandler(OnTimerElapsed), // 使用命名方法
            TimeSpan.FromMilliseconds(16)
        );
    }

    private static void OnTimerElapsed(ThreadPoolTimer timer)
    {
        // 在 UI 线程上执行 Tick
        _dispatcherQueue?.TryEnqueue(new DispatcherQueueHandler(Tick));
    }

    public static void Tick()
    {
        CurrentFrame++;
        for (int i = 0; i < MainPage.SelectedAnimationCharacter.Count(); i++)
        {
            MainPage.SelectedAnimationCharacter[i].PlayAnimation(MainPage.SelectedAnimation[i]);
        }
        if (TickRequest != null)
        {
            TickRequest.Invoke();
        }
        if (CurrentFrame >= MainPage.FilmCountMax)
        {
            RequestPause();
            Reset();
        }
    }

    public static void Stop()
    {
        Timer?.Cancel();
        Timer = null;
    }

    public static void Reset()
    {
        Stop();
        CurrentFrame = 0;
    }

    public static void RequestPause()
    {
        Stop();
        PauseRequest?.Invoke();
    }
}