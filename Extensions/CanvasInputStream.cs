using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.System;
using Windows.UI.Core;

namespace Fishtrace;


public class CanvasInputStream
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public static Canvas Canvas;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public CanvasInputStream(Canvas canvas)
    {
        Canvas = canvas;
        Canvas.AllowFocusOnInteraction = true;
        Canvas.IsTabStop = true;
    }

    public static void ConnectStream(KeyEventHandler handler)
    {
        Canvas.KeyDown += handler;
    }

    public static void DisconnectStream(KeyEventHandler handler)
    {
        Canvas.KeyDown -= handler;
    }

    public static void CanvasStartFocus()
    {
        Canvas.Focus(FocusState.Programmatic);
    }

    public static char? ConvertVirtualKeyToChar(VirtualKey key)
    {
        // 检查 Shift 是否按下（使用 CoreWindow 只读状态，不订阅事件）
        var shiftState = InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift);
        bool isShiftDown = (shiftState & CoreVirtualKeyStates.Down) != 0;

        // 字母 A-Z
        if (key >= VirtualKey.A && key <= VirtualKey.Z)
        {
            char baseChar = (char)('A' + (key -VirtualKey.A));
            // 如果 Shift 或 CapsLock 生效，大写，否则小写
            bool capsLock = (InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.CapitalLock)
                             & CoreVirtualKeyStates.Locked) != 0;
            bool upper = isShiftDown ^ capsLock; // 异或：Shift 切换大小写
            return upper ? char.ToUpper(baseChar) : char.ToLower(baseChar);
        }

        // 数字 0-9 映射到符号
        if (key >= VirtualKey.Number0 && key <= VirtualKey.Number9)
        {
            int num = key - VirtualKey.Number0;
            string symbols = ")!@#$%^&*("; // Shift+数字对应的符号
            if (isShiftDown)
                return symbols[num];
            else
                return (char)('0' + num);
        }
        return null;
    }
}