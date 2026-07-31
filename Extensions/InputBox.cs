using System.Runtime.CompilerServices;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.ApplicationModel.Search.Core;
using Windows.ApplicationModel.Store;
using Windows.System;
using Windows.UI.Core;

namespace Fishtrace;


public class InputBox : Element
{
    public bool _isInputActive = false;
    public CommonElement DisplayTextBlock;
    public CommonElement InformationTextBlock;
    public CommonElement Box;
    public Element Owner;
    public Canvas Canvas;
    public string InputCache = "";

    public InputBox(CommonElement box, CommonElement display, CommonElement info, string name, Element owner)
    {
        if (!(box.UIElement is Image)) { throw new NullReferenceException(); }
        if (!(display.UIElement is TextBlock)) { throw new NullReferenceException(); }
        if (!(info.UIElement is TextBlock)) { throw new NullReferenceException(); }

        Box = box;
        DisplayTextBlock = display;
        InformationTextBlock = info;
        Canvas = box.Canvas;
        Hide();
        Name = name;
        UIElement = Box.UIElement;
        Owner = owner;
        Canvas.SetZIndex(Box.UIElement, 40);
        Canvas.SetZIndex(DisplayTextBlock.UIElement, 41);
        Canvas.SetZIndex(InformationTextBlock.UIElement, 41);
    }

    public void Update()
    {
        var d = (TextBlock)DisplayTextBlock.UIElement;
        d.Text = InputCache;
    }

    public void OnInput(double x, double y)
    {
        MainPage.Consoles.ConsoleOutput("OnInput");
        _isInputActive = true;

        List<double> r = CalculateOffset(x,y);
        SetX(r[0]);
        SetY(r[1]);
        Show();

        CanvasInputStream.ConnectStream(OnCanvasKeyDown);
        CanvasInputStream.Canvas.Focus(FocusState.Programmatic);
    }

    public void ExitInput()
    {
        MainPage.Consoles.ConsoleOutput("ExitInput");
        _isInputActive = false;
        Hide();

        Owner.InputReceive = InputCache;
        CanvasInputStream.DisconnectStream(OnCanvasKeyDown);
    }

    public static List<double> CalculateOffset(double x,double y)
    {
        double box_Y = y - 60;
        double box_X = x - 210;
        if (y - 60 < 0)
        {
            box_Y = y;
        }
        else if (y - 60 + 200 > MainPage.CanvasUpperLimit_Y)
        {
            box_Y = y - 200 + 80;
        }

        if (x - 210 < 0)
        {
            box_X = x + 100;
        }
        
        var r = new List<double> {box_X,box_Y};
        return r;
    }

    // public void InputChar(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.CharacterReceivedEventArgs e)
    // {
    //     if (!_isInputActive) { return; }

    //     char c = (char)e.KeyCode;
    //     if (char.IsControl(c)) { return; }

    //     InputCache += c;
    //     Owner.InputReceive = InputCache;
    //     Update();
    // }

    // public void InputControl(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs e)
    // {
    //     if (!_isInputActive) { return; }

    //     if (e.VirtualKey == Windows.System.VirtualKey.Back)
    //     {
    //         if (InputCache.Length > 0)
    //         {
    //             InputCache = InputCache.Remove(InputCache.Length - 1);
    //             Update();
    //         }
    //         e.Handled = true;
    //     }

    //     if (e.VirtualKey == Windows.System.VirtualKey.Enter)
    //     {
    //         ExitInput();
    //         e.Handled = true;
    //     }
    // }

    public void OnCanvasKeyDown(object sender, KeyRoutedEventArgs e)
    {
        MainPage.Consoles.ConsoleOutput("sdsdad");
        if (!_isInputActive) return;

        if (e.Key == VirtualKey.Back)
        {
            if (InputCache.Length > 0)
            {
                InputCache = InputCache.Remove(InputCache.Length - 1);
                Owner.InputReceive = InputCache;
                Update();
            }
            e.Handled = true;
            return;
        }

        // 回车（提交）
        if (e.Key == VirtualKey.Enter)
        {
            ExitInput();
            e.Handled = true;
            return;
        }

        // 空格
        if (e.Key == VirtualKey.Space)
        {
            InputCache += ' ';
            Owner.InputReceive = InputCache;
            Update();
            e.Handled = true;
            return;
        }

        // 字母、数字、符号：需要检查 Shift 状态
        char? c = CanvasInputStream.ConvertVirtualKeyToChar(e.Key);
        if (c.HasValue)
        {
            MainPage.Consoles.ConsoleOutput("Converting");
            InputCache += c.Value;
            Owner.InputReceive = InputCache;
            Update();
            e.Handled = true;
        }
    }

    public override void SetX(double x)
    {
        Box.SetX(x);
        DisplayTextBlock.SetX(x + 3);
        InformationTextBlock.SetX(x + 3);
    }

    public override void SetY(double y)
    {
        Box.SetY(y);
        DisplayTextBlock.SetY(y + 100);
        InformationTextBlock.SetY(y);
    }

    public override void Hide()
    {
        Box.Hide();
        DisplayTextBlock.Hide();
        InformationTextBlock.Hide();
    }

    public override void Show()
    {
        Box.Show();
        DisplayTextBlock.Show();
        InformationTextBlock.Show();
    }
}