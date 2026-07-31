using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.WindowsAppSDK.Runtime.Packages;

namespace Fishtrace;


public class SliderBar : FunctionElement
{
    //Parent:SliderBar Children: Slider
    public int UpperLimit = 0;
    public int CurrentIndex
    {
        get
        {
            return Slider.CurrentIndex;
        }
    }

    public Slider Slider;
    public CommonElement Display;
    public CommonElement Box;
    public FunctionElement VisibilityController;
    public bool _isShowing = true;
    public SliderBar(FrameworkElement f, Slider slider, CommonElement display, CommonElement box, FunctionElement vc, string name) : base(f, name)
    {
        if (!(display.UIElement is TextBlock)) { throw new InvalidOperationException(); }
        Slider = slider;
        Display = display;
        Box = box;
        VisibilityController = vc;
        AddChild(Slider);
        AddChild(Display);
        AddChild(Box);
        AddChild(VisibilityController);

        Slider.SetX(157.5);
        Slider.SetY(495);
        Display.SetX(52.5);
        Display.SetY(505);
        Box.SetX(50);
        Box.SetY(500);
        VisibilityController.SetX(755);
        VisibilityController.SetY(500);

        Canvas.SetZIndex(Slider.UIElement, 21);
        Canvas.SetZIndex(display.UIElement, 20);
        Canvas.SetZIndex(box.UIElement, 19);
        Canvas.SetZIndex(VisibilityController.UIElement, 20);

        AddEnteredEvent(OnPointerEnter);
        VisibilityController.AddAction(OnPressedAction);
    }

    public override void Hide()
    {
        base.Hide();
        Box.Hide();
        Display.Hide();
        Slider.Hide();
        VisibilityController.Hide();
    }

    public override void Show()
    {
        base.Show();
        Box.Show();
        Display.Show();
        Slider.Show();
        VisibilityController.Show();
    }

    public static SliderBar CreateSliderBar(FrameworkElement f, string name)
    {
        if (f.Parent == null) { throw new NullReferenceException(); }
        if (!(f.Parent is Canvas)) { throw new InvalidOperationException(); }
        Image sliderI = Utils.CreateImage("Assets/10x40Slider.png");
        Image BoxI = Utils.CreateImage("Assets/70x30GrayBlcok.png");
        Image vc = Utils.CreateImage("Assets/35x30EyeOff.png");
        TextBlock display = Utils.CreateTextBlock("0");
        Canvas canvas = (Canvas)f.Parent;
        canvas.Children.Add(sliderI);
        canvas.Children.Add(BoxI);
        canvas.Children.Add(display);
        canvas.Children.Add(vc);

        sliderI.Width = 10;
        sliderI.Height = 40;
        BoxI.Width = 70;
        BoxI.Height = 30;
        display.Width = 70;
        display.Height = 30;
        vc.Width = 35;
        vc.Height = 30;


        return new SliderBar(f, new Slider(sliderI, name + "_Slider"), new CommonElement(display, name + "_Display"), new CommonElement(BoxI, name + "_Box"), new FunctionElement(vc, name + "_vc"), name);
    }

    public async void OnPointerEnter(object sender, PointerRoutedEventArgs e)
    {
        if (!_isShowing)
        {
            VisibilityController.Show();
            Slider.Show();
            FadingEffect.FadeIn(UIElement);
            FadingEffect.FadeIn(Slider.UIElement);
            FadingEffect.FadeIn(Box.UIElement);
            FadingEffect.FadeIn(Display.UIElement);
            FadingEffect.FadeIn(VisibilityController.UIElement);
            _delay();
        }
    }

    private async void _delay()
    {
        await Task.Delay(3000);
        _isShowing = !_isShowing;
    }

    public void OnPressedAction()
    {
        if (_isShowing)
        {
            FadingEffect.FadeOut(UIElement);
            FadingEffect.FadeOut(Slider.UIElement);
            FadingEffect.FadeOut(Box.UIElement);
            FadingEffect.FadeOut(Display.UIElement);
            FadingEffect.FadeOut(VisibilityController.UIElement);
            VisibilityController.Hide();
            Slider.Hide();
            _delay();
        }
    }

    public override void AddPressedEvent(PointerEventHandler handler)
    {
        return;
    }


    // public void OnPointerMove(object sender, PointerRoutedEventArgs e)
    // {
    //     try
    //     {
    //         if (e.GetCurrentPoint(Canvas).Position.Y > 500)
    //         {
    //             if (!_isShowing)
    //             {
    //                 FadingEffect.FadeIn(UIElement);
    //                 FadingEffect.FadeIn(Slider.UIElement);
    //                 FadingEffect.FadeIn(Box.UIElement);
    //                 FadingEffect.FadeIn(Display.UIElement);
    //                 _delay();
    //                 //Show();
    //             }
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         MainPage.Consoles.ConsoleOutput(ex.Message);
    //     }
    // }
}