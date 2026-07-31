using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.UI.Core;
using Windows.Foundation.Metadata;
using Windows.ApplicationModel.UserDataTasks;
using System.Security.Cryptography.X509Certificates;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Fishtrace;

/// <summary>
/// The main content page displayed inside the application window.
/// Add your UI logic, event handlers, and data binding here.
/// </summary>
/// 
/// 
/// Zindex allocation:
/// 0:The bottom, should be given to the Animate Characters.
/// 10:For the bars.
/// 20:For the buttons.
/// 30,31:For the hintboxes.
/// 40,41:For the inputboxes and functionboxes
public sealed partial class MainPage : Page
{
    public static ConsoleList Consoles = new ConsoleList(new List<ConsoleTextBlock>(), "dummy");
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public static CanvasInputStream InputStream;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public static double CanvasUpperLimit_Y = 540;
    public static double CanvasUpperXLimi_X = 960;
    public static Linker L;
    public DraggableCharacter e;
    public int LoadedCount1 = 0;
    public int LoadedCount2 = 0;
    public int LoadCount = 0;
    public int NumElement;
    public ElementBar var_UpperBar1;
    public ElementBar var_UpperBar2;
    public ElementBar var_UpperBar3;
    public ElementBar var_RightBar1;
    public ElementBar var_RightBar2;
    public ElementBar var_RightBar3;
    public FunctionElement var_RecordButton;
    public FunctionElement var_StopButton;
    public FunctionElement var_PlayButton;
    public AnimationSlot var_AddButton;
    public FunctionElement var_HaltButton;
    public FunctionElement var_HomeButton;
    public FunctionElement var_FFTButton;
    public FunctionElement var_PolyFitButton;
    public SliderBar var_FilmSliderBar;
    public Puller var_UpperBarPuller;
    public Puller var_RightBarPuller;
    public int TickCount = 0;
    public static Animation? AnimationRecording = null;
    public static DraggableCharacter? MainCharacter = null;
    public static int TickRate = 60;
    public static Dictionary<string, Animation> Film = new Dictionary<string, Animation>();
    public static CharacterSlotBar CurrentCharacterBar;
    public static ElementBar CurrentAnimationBar;
    public static string CurrentRightBarStatus = ElementBar.Status_AnimationBar;
    public static List<Animation> SelectedAnimation = new List<Animation>();
    public static List<DraggableCharacter> SelectedAnimationCharacter = new List<DraggableCharacter>();
    public static bool isPlayingAnimation = false;
    public static bool isRecordingOn = false;
    public static int CurrentSliderIndex;
    public static AnimationSlot? LatestAnimationSlot;
    public static ElementBarManager RightElementBarManager = new ElementBarManager();
    public static ElementBarManager UpElementBarManager = new ElementBarManager();
    public static int FilmCountMax
    {
        get
        {
            int m = 0;
            foreach (Animation animation in Film.Values)
            {
                if (animation.FrameCollection.Count() > m)
                {
                    m = animation.FrameCollection.Count();
                }
            }
            return m;
        }
    }

    public MainPage()
    {
        InitializeComponent();

        NumElement = MyDrawingCanvas.Children.Count();

        InitializeConsoles();

        AddLoadedEvent(MyDrawingCanvas);

        // UtilsBar.Loaded += (s, e) => { TestingFunctionBox(); };
        // f2.Loaded += (s, e) => { TestingFunctionBox(); };
        // g.Loaded += (s, e) => { TestingFunctionBox(); };
        // h.Loaded += (s, e) => { TestingFunctionBox(); };
        // i.Loaded += (s, e) => { TestingFunctionBox(); };
        // j.Loaded += (s, e) => { TestingFunctionBox(); };

        MyDrawingCanvas.Loaded += (s, e) =>
        {
            InputStream = new CanvasInputStream(MyDrawingCanvas);
        };

        // Testing1.Loaded += (s, e) => { TestingLinker(); };
        // Testing2.Loaded += (s, e) => { TestingLinker(); };
        // Testing3.Loaded += (s, e) => { TestingLinker(); };
    }

    public void InitializeConsoles()
    {
        List<ConsoleTextBlock> _Consoles = new List<ConsoleTextBlock>();
        _Consoles.Add(new ConsoleTextBlock(Console1, Console1.Name));
        Consoles = new ConsoleList(_Consoles, "Console1");
        Canvas.SetZIndex(Console1, 1000);
        Console1.TextWrapping = TextWrapping.Wrap;
    }

    // public void ThisIsATest()
    // {
    //     fe = new FunctionElement(Testing3, "Testing3");
    //     fe.AddPressedEvent(ThisIsAlsoATest);
    //     fe.AddCosmetic("1","Assets/80x80LeftArrow.png");
    // }

    // public void ThisIsAlsoATest(object sender, PointerRoutedEventArgs e)
    // {
    //     if (!e.GetCurrentPoint(Testing3.Parent as Canvas).Properties.IsLeftButtonPressed) return;
    //     fe.ChangeCosmetic("1");
    // }

    // public void TestingDragElement()
    // {
    //     DraggableCharacter c = new DraggableCharacter(Testing4,"drag");
    // }

    // public void TestingLinker()
    // {
    //     LoadedCount1++;
    //     if (LoadedCount1 != 3) return;
    //     DraggableCharacter a = new DraggableCharacter(Testing1, "drag1");
    //     DraggableCharacter b = new DraggableCharacter(Testing2, "drag2");
    //     DraggableCharacter c = new DraggableCharacter(Testing3, "drag3");
    //     Linker l = new Linker(a);
    //     l.LinkElement(b);
    //     l.LinkElement(c);
    //     L = l;

    //     TestingLinker2();
    // }

    public async void TestingLinker2()
    {
        await Task.Delay(10000);

        L.GiveUpElement(L.ElementList[2]);
        Consoles.ConsoleOutput(L.LeadingElement.Name);
    }

    public void TestingFunctionBox()
    {
        // LoadedCount2++;
        // if (LoadedCount2 != 6) return;
        // e = new DraggableCharacter(UtilsBar, "dragc");
        // FunctionElement f = new FunctionElement(f2, "f1");
        // FunctionElement f22 = new FunctionElement(g, "f11");
        // FunctionElement f3 = new FunctionElement(h, "f111");
        // FunctionElement f4 = new FunctionElement(i, "f1111");
        // FunctionElement f5 = new FunctionElement(j, "f11111");
        // f.Hide();
        // f22.Hide();
        // f3.Hide();
        // f4.Hide();
        // f5.Hide();
        // e.AddFunctionBox();
        // e.FunctionBox.AddFunction(f);
        // f.AddDescription("This is a button");
        // e.FunctionBox.AddFunction(f22);
        // e.FunctionBox.AddFunction(f3);
        // e.FunctionBox.AddFunction(f4);
        // e.FunctionBox.AddFunction(f5);
    }

    public void AddLoadedEvent(Canvas canvas)
    {
        foreach (FrameworkElement f in canvas.Children)
        {
            f.Loaded += (s, e) => { OnElementLoaded(); };
        }
    }

    private void OnElementLoaded()
    {
        InitializeElements();
    }

    public void InitializeElements()
    {
        LoadCount++;
        if (LoadCount != NumElement) return;
        var_UpperBar1 = new ElementBar(UpperBar1, UpperBar1.Name);
        var_RightBar1 = new ElementBar(RightBar1, RightBar1.Name);
        var_RightBar2 = new ElementBar(RightBar2, RightBar2.Name);
        var_RightBar3 = new ElementBar(RightBar3, RightBar3.Name);

        var_RecordButton = new FunctionElement(RecordButton, RecordButton.Name);
        var_StopButton = new FunctionElement(StopButton, StopButton.Name);
        var_PlayButton = new FunctionElement(PlayButton, PlayButton.Name);
        var_AddButton = new AnimationSlot(AddButton, AddButton.Name);
        var_HomeButton = new FunctionElement(HomeButton, HomeButton.Name);
        var_HaltButton = new FunctionElement(HaltButton, HaltButton.Name);
        var_FFTButton = new FunctionElement(FFTButton, FFTButton.Name);
        var_PolyFitButton = new FunctionElement(PolyFitButton, PolyFitButton.Name);

        RightElementBarManager.AddElementbar(var_RightBar1);
        UpElementBarManager.AddElementbar(var_UpperBar1);

        var_UpperBarPuller = new Puller(UpperBarPuller, UpElementBarManager, UpperBarPuller.Name);
        var_RightBarPuller = new Puller(RightBarPuller, RightElementBarManager, RightBarPuller.Name, true);

        var_FilmSliderBar = SliderBar.CreateSliderBar(FilmSliderBar, FilmSliderBar.Name);

        var_RecordButton.AddAction(OnRecordButtonClick);
        var_StopButton.AddAction(OnStopButtonClick);
        var_PlayButton.AddAction(OnPlayButtonClick);
        var_HomeButton.AddAction(OnHomeButtonClick);
        var_HaltButton.AddAction(OnHaltButtonClick);
        var_FFTButton.AddAction(OnFFTButtonClick);
        var_PolyFitButton.AddAction(OnPolyFitButtonClick);

        var_RecordButton.EnableSpringFeedback();
        var_RecordButton.EnableHoverGlow();
        var_PlayButton.EnableSpringFeedback();
        var_PlayButton.EnableHoverGlow();
        var_StopButton.EnableSpringFeedback();
        var_StopButton.EnableHoverGlow();
        var_HaltButton.EnableSpringFeedback();
        var_HaltButton.EnableHoverGlow();
        var_HomeButton.EnableSpringFeedback();

        var_FFTButton.EnableHoverGlow();
        var_FFTButton.EnableSpringFeedback();
        var_PolyFitButton.EnableHoverGlow();
        var_PolyFitButton.EnableSpringFeedback();

        var_FFTButton.AddFunctionBox(FFTButton.Name + "_FunctionBox");
        var_FFTButton.FunctionBox.AddFunction(FFT.ChangeRatio03, "LowPassFilterRatio:0.3");
        var_FFTButton.FunctionBox.AddFunction(FFT.ChangeRatio04, "LowPassFilterRatio:0.4");
        var_FFTButton.FunctionBox.AddFunction(FFT.ChangeRatio05, "LowPassFilterRatio:0.5");
        var_FFTButton.FunctionBox.AddFunction(FFT.ChangeRatio06, "LowPassFilterRatio:0.6");
        var_FFTButton.FunctionBox.AddFunction(FFT.ChangeRatio09, "LowPassFilterRatio:0.9");

        var_PolyFitButton.AddFunctionBox(PolyFitButton.Name + "_FunctionBox");
        var_PolyFitButton.FunctionBox.AddFunction(PolynomialFit.ChangeDegree02, "Linear");
        var_PolyFitButton.FunctionBox.AddFunction(PolynomialFit.ChangeDegree03, "Degree:3");
        var_PolyFitButton.FunctionBox.AddFunction(PolynomialFit.ChangeDegree05, "Degree:5");
        var_PolyFitButton.FunctionBox.AddFunction(PolynomialFit.ChangeDegree010, "Degree:10");
        var_PolyFitButton.FunctionBox.AddFunction(PolynomialFit.ChangeDegree020, "Degree:20");

        var_UpperBar1.AddFunction(var_RecordButton, false, false);
        var_UpperBar1.AddFunction(var_StopButton, false, false);
        var_UpperBar1.AddFunction(var_PlayButton, false, false);
        var_UpperBar1.AddFunction(var_HaltButton, false, false);
        var_UpperBar1.AddFunction(var_FFTButton, false, false);
        var_UpperBar1.AddFunction(var_PolyFitButton, false, false);
        var_RightBar1.AddFunction(var_AddButton);
        var_RightBarPuller.AddChild(var_HomeButton);

        var_UpperBar1.SetHorizontal();
        var_RightBar1.SetVertical();

        var_UpperBar1.Show();
        var_RightBar1.Show();
        var_RightBar2.Hide();
        var_RightBar3.Hide();
        var_HomeButton.Show();
        var_FFTButton.Show();
        var_FFTButton.Show();

        var_StopButton.Hide();
        var_HaltButton.Hide();

        var_FilmSliderBar.Show();

        CurrentAnimationBar = var_RightBar1;

        GlobalRecordTimer.Initialize();
        GlobalAnimationTimer.Initialize();
        GlobalAnimationTimer.PauseRequest += HaltAnimation;

        //var_RightBarPuller.Linker.LinkElement(var_HomeButton);
    }

    public void OnRecordButtonClick()
    {
        if (AnimationRecording == null || MainCharacter == null) return;
        TickCount = 0;
        if (!Film.ContainsValue(AnimationRecording))
        {
            Consoles.ConsoleOutput("No Such value");
            GlobalRecordTimer.Reset();
        }
        GlobalRecordTimer.MaxFrame = AnimationRecording.FrameCollection.Count();
        GlobalRecordTimer.Start();
        var_RecordButton.Hide();
        var_StopButton.Show();
        isRecordingOn = true;

    }


    public void OnStopButtonClick()
    {
        var_RecordButton.Show();
        var_StopButton.Hide();
        isRecordingOn = false;

        GlobalRecordTimer.Stop();
    }

    public void OnPlayButtonClick()
    {
        if (SelectedAnimation.Count == 0) return;
        if (!isRecordingOn)
        {
            var_FilmSliderBar.UpperLimit = FilmCountMax;
        }

        GlobalAnimationTimer.Start();

        var_PlayButton.Hide();
        var_HaltButton.Show();
        isPlayingAnimation = true;
    }

    public void OnHaltButtonClick()
    {
        GlobalAnimationTimer.Stop();
        HaltAnimation();
    }

    public void HaltAnimation()
    {
        var_HaltButton.Hide();
        var_PlayButton.Show();
        isPlayingAnimation = false;
    }

    public void OnHomeButtonClick()
    {
        RightElementBarManager.Hide();
        CurrentAnimationBar.Show();
        CurrentRightBarStatus = ElementBar.Status_AnimationBar;
    }

    public void OnFFTButtonClick()
    {
        try
        {
            if (LatestAnimationSlot == null) return;
            if (AnimationRecording == null) return;

            Animation a = AnimationRecording;
            if (a.FrameCollection.Count() == 0) return;
            for (int i = 0; i < Math.Ceiling(a.FrameCollection.Count() / (double)FFT.FFTwindow); i++)
            {
                Consoles.ConsoleOutput("fft" + i.ToString());
                a = FFT.ApplyFFT(a, i * FFT.FFTwindow, i * FFT.FFTwindow + FFT.FFTwindow - 1);
                if (a == null)
                {
                    return;
                }
            }

            LatestAnimationSlot.AddAnimation("Film" + (Film.Count() + 1) + "_fft", a, CurrentCharacterBar);
        }
        catch(Exception ex)
        {
            Consoles.ConsoleOutput(ex.Message);
            OnHomeButtonClick();
        }
    }

    public void OnPolyFitButtonClick()
    {
        if (LatestAnimationSlot == null) return;
        if (AnimationRecording == null) return;

        Animation a = AnimationRecording;
        if (a.FrameCollection.Count() == 0) return;
        for (int i = 0; i < Math.Ceiling(a.FrameCollection.Count() / (double)PolynomialFit.PolyFitWindow); i++)
        {
            Consoles.ConsoleOutput("PolyFit" + i.ToString());
            a = PolynomialFit.ApplyPolynomialFit(a, i * PolynomialFit.PolyFitWindow, i * PolynomialFit.PolyFitWindow + PolynomialFit.PolyFitWindow - 1);
        }

        LatestAnimationSlot.AddAnimation("Film" + (Film.Count() + 1) + "_PolyFit", a, CurrentCharacterBar);
    }

    public void OnCtrl1(object handler, object e)
    {
        if (MainCharacter == null) return;
        MainCharacter.ClickNextCosmetic();
    }

    public void OnCtrl2(object handler, object e)
    {
        if (MainCharacter == null) return;
        MainCharacter.ClickPreviousCosmetic();
    }

    public void OnCtrl3(object handler, object e)
    {
        if (MainCharacter == null) return;
        MainCharacter.ClickToHideOrShow();
    }

    public void OnCtrl4(object handler, object e)
    {
    }

    public void OnCtrl5(object handler, object e)
    {
    }

    public void OnCtrlQ(object handler, object e)
    {
        OnPlayButtonClick();
    }

    public void OnCtrlW(object handler, object e)
    {
        OnHaltButtonClick();
    }
}
