using System.Reflection;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.Devices.Midi;

namespace Fishtrace;


public class CommonElement : Element
{
    public HintBox? HintBox;
    public InputBox? InputBox;
    public FunctionBox? FunctionBox;
    public string? Hint;
    public string? InputInfo;
    public CommonElement(FrameworkElement f, string? name = null)
    {
        if (!(f.Parent is Canvas)) { throw new InvalidOperationException(); }
        UIElement = f;
        Canvas = (Canvas)f.Parent;
        if (name == null)
        {
            Random random = new Random();
            Name = "CE_" + random.Next().ToString();
        }
        else
        {
            Name = name;
        }

    }

    public void AddHint(string text, string? name = null)
    {
        if (!(UIElement.Parent is Canvas)) { return; }
        Hint = text;
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        Canvas c = UIElement.Parent as Canvas;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8604 // Possible null reference argument.
        HintBox = Utils.CreateHintBox(c, text, name, this);
#pragma warning restore CS8604 // Possible null reference argument.
        AddEnteredEvent(ActiveHintBox);
        AddExitedEvent(DeactiveHintBox);
    }

    public void AddInputBox(string info = "", string? name = null)
    {
        if (!(UIElement.Parent is Canvas)) { return; }
        InputInfo = info;
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        Canvas c = UIElement.Parent as Canvas;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8604 // Possible null reference argument.
        InputBox = Utils.CreateInputBox(c, info, name, this);
#pragma warning restore CS8604 // Possible null reference argument.

        AddPressedEvent(CallInputBox);
    }

    public void AddFunctionBox(string? name = null)
    {
        if (!(UIElement.Parent is Canvas)) { return; }
        if (name == null) { name = Name + "_FunctionBox"; }
        var I = Utils.CreateGrayBlock();
        Canvas.Children.Add(I);
        I.Width = 200;
        I.Height = 200;
        FunctionBox = new FunctionBox(I, name, this);

        AddPressedEvent(CallFunctionBox);
    }

    public void ActiveHintBox(object sender, PointerRoutedEventArgs e)
    {
        if (HintBox == null) { return; }
        HintBox._istouching = true;
        HintBox.SetVisible(GetX(), GetY());
        MainPage.Consoles.ConsoleOutput("active");
        CanvasInputStream.Canvas.Focus(FocusState.Programmatic);
    }

    public void DeactiveHintBox(object sender, PointerRoutedEventArgs e)
    {
        if (HintBox == null) { return; }
        HintBox._istouching = false;
        if (HintBox._cls == null) { return; }
        HintBox._cls.Cancel();
    }

    public void CallInputBox(object sender, PointerRoutedEventArgs e)
    {
        if (!e.GetCurrentPoint(Canvas).Properties.IsRightButtonPressed) { return; }
        if (InputBox == null) { return; }

        if (!InputBox._isInputActive) { InputBox.OnInput(GetX(), GetY()); }
        else { InputBox.ExitInput(); }
    }

    public void CallFunctionBox(object sender, PointerRoutedEventArgs e)
    {
        if (!e.GetCurrentPoint(Canvas).Properties.IsRightButtonPressed) { return; }
        if (FunctionBox == null) { return; }

        if (!FunctionBox._isActive) { FunctionBox.Active(GetX(), GetY()); }
        else { FunctionBox.DeActive(); }
    }

    public void UseHintToDisplayInput()
    {
        if (HintBox == null) { throw new NullReferenceException(); }
        if (InputReceive == null) { throw new NullReferenceException(); }
        MainPage.Consoles.ConsoleOutput("ppppp");
        HintBox.SetHint(InputReceive);
    }

    public override void SetX(double x)
    {
        base.SetX(x);
        if (HintBox != null || InputBox != null || FunctionBox != null) return;
        List<double> r = Utils.CalculateBoxOffset(GetX(), GetY());
        if (HintBox != null) { HintBox.SetX(r[0]); }
        if (InputBox != null) { InputBox.SetX(r[0]); }
        if (FunctionBox != null) { FunctionBox.SetX(r[0]); }
    }

    public override void SetY(double y)
    {
        base.SetY(y);
        if (HintBox != null || InputBox != null || FunctionBox != null) return;
        List<double> r = Utils.CalculateBoxOffset(GetX(), GetY());
        if (HintBox != null) { HintBox.SetY(r[1]); }
        if (InputBox != null) { InputBox.SetY(r[1]); }
        if (FunctionBox != null) { FunctionBox.SetY(r[1]); }
    }
}