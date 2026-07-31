using System.ComponentModel.DataAnnotations;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace Fishtrace;


public class Slider : FunctionElement
{
    //The Slider on the SliderBar
    //Parent:SliderBar Children: Slider
    public bool isHolding = false;
    public int UpperLimit
    {
        get
        {
            if (Parent == null) return 1;
            if (!(Parent is SliderBar)) return 1;
            return ((SliderBar)Parent).UpperLimit;
        }
    }

    public double StepSize
    {
        get
        {
            if (UpperLimit <= 1) return 0;
            return 550.0 / (UpperLimit - 1);
        }
    }

    public int CurrentIndex
    {
        get
        {
            if (UpperLimit == 1) return 0;
            return (int)Math.Round((GetX() - initX) / StepSize);
        }
    }

    public double initX = 157.5;

    public Slider(FrameworkElement f, string name) : base(f, name)
    {
        AddPressedEvent(OnLeftClick);
        AddReleasedEvent(OnLeftClickRelease);
        AddHoldingEvent(OnHolding);
        GlobalAnimationTimer.TickRequest += AnimateTickSetX;
        GlobalRecordTimer.TickRequest += RecordTickSetX;
    }

    public void SetXAccordingToIndex(int index)
    {
        if (Parent == null) return;
        if (!(Parent is SliderBar)) return;
        if (index > UpperLimit || index < 0) { MainPage.Consoles.ConsoleOutput((index - UpperLimit).ToString()); return; }
        SetX(initX + StepSize * index);
        if (MainPage.isRecordingOn)
        {
            ((TextBlock)((SliderBar)Parent).Display.UIElement).Text = CurrentIndex.ToString() + "/" + GlobalRecordTimer.MaxFrame;
        }
        else
        {
            ((TextBlock)((SliderBar)Parent).Display.UIElement).Text = CurrentIndex.ToString() + "/" + MainPage.FilmCountMax;
        }
    }

    public void SetXAccordingToValue(double x)
    {
        if (UpperLimit == 1) return;
        int index = (int)Math.Floor((x - initX) / StepSize);
        if (index >= UpperLimit || index < 0) return;
        SetXAccordingToIndex(index);
    }

    public void AnimateTickSetX()
    {
        if (isHolding) return;
        if (MainPage.isRecordingOn) return;
        SetXAccordingToIndex(GlobalAnimationTimer.CurrentFrame);
    }

    public void RecordTickSetX()
    {
        ((SliderBar)Parent).UpperLimit = GlobalRecordTimer.MaxFrame;
        if (isHolding) return;
        if (GlobalRecordTimer.CurrentFrame >= GlobalRecordTimer.MaxFrame)
        {
            SetXAccordingToIndex(GlobalRecordTimer.MaxFrame);
        }
        else
        {
            SetXAccordingToIndex(GlobalRecordTimer.CurrentFrame);
        }
    }

    public void OnLeftClick(object sender, object e)
    {
        if (Parent == null) return;
        if (!(Parent is SliderBar)) return;
        if (((SliderBar)Parent).UpperLimit == 0) return;
        isHolding = true;
    }

    public void OnHolding(object sender, PointerRoutedEventArgs e)
    {
        if (!isHolding) return;
        if (!e.GetCurrentPoint(Canvas).Properties.IsLeftButtonPressed) return;
        SetXAccordingToValue(e.GetCurrentPoint(Canvas).Position.X);
        if (MainPage.isRecordingOn)
        {
            GlobalRecordTimer.CurrentFrame = CurrentIndex;
        }
        else
        {
            GlobalAnimationTimer.CurrentFrame = CurrentIndex;
        }
    }

    public void OnLeftClickRelease(object sender, PointerRoutedEventArgs e)
    {
        if (!isHolding) return;
        if (e.GetCurrentPoint(Canvas).Properties.IsLeftButtonPressed) return;

        isHolding = false;
    }
}