using System.Numerics;
using System.Runtime.InteropServices;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.Devices.PointOfService;
using Windows.Graphics.Display;

namespace Fishtrace;


public class DraggableCharacter : CommonElement
{
    private bool _isdragging = false;
    public bool _isHiding = false;
    public bool _isLoop = false;
    private double offsetX = 0;
    private double offsetY = 0;
    public Animation? AnimationOnPlaying = null;
    private TaskCompletionSource<bool>? isHalt;
    private double? initX;
    private double? initY;
    public Visual Visual;
    public DraggableCharacter(FrameworkElement f, string? name = null) : base(f, name)
    {
        if (!(f.Parent is Canvas || f.Parent == null)) { throw new InvalidOperationException(); }
        if (!(f is Image)) { throw new InvalidOperationException(); }
#pragma warning disable CS8601 // Possible null reference assignment.
        Canvas = f.Parent as Canvas;
#pragma warning restore CS8601 // Possible null reference assignment.
        Canvas.SetZIndex(UIElement, 0);
        AddPressedEvent(DragPressed);
        AddReleasedEvent(DragReleased);
        AddHoldingEvent(DragHolding);
        AddCaptureLossEvent(DragCaptureLoss);
        AddFunctionBox(Name + "_FunctionBox");
        FunctionBox.AddFunction(ClickNextCosmetic, "NextCosmetic-ctrl+1");
        FunctionBox.AddFunction(ClickPreviousCosmetic, "PreviousCosmetic-ctrl+2");
        FunctionBox.AddFunction(ClickToHideOrShow, "ClickMeToHide-ctrl+3");

        GlobalAnimationTimer.PauseRequest += ClearAnimationOnPlaying;

        Visual = ElementCompositionPreview.GetElementVisual(UIElement);
        Visual.Offset = new Vector3(0, 0, 0);
    }

    public void ClickNextCosmetic()
    {
        NextCosmetic();
    }

    public void ClickPreviousCosmetic()
    {
        PreviousCosmetic();
    }
    public void ClickToHideOrShow()
    {
        if (_isHiding)
        {
            Show();
            _isHiding = false;
            ((TextBlock)FunctionBox.FunctionList[2].Description.UIElement).Text = "ClickMeToHide";
        }
        else
        {
            Hide(); _isHiding = true;
            ((TextBlock)FunctionBox.FunctionList[2].Description.UIElement).Text = "ClickMeToShow";
        }
    }

    public void DragPressed(object sender, PointerRoutedEventArgs e)
    {
        if (e.GetCurrentPoint(Canvas).Properties.IsLeftButtonPressed == false) { return; }

        _isdragging = true;

        double _x = e.GetCurrentPoint(Canvas).Position.X;
        double _y = e.GetCurrentPoint(Canvas).Position.Y;

        if (Linker == null)
        {
            offsetX = GetX() - _x;
            offsetY = GetY() - _y;
        }
        else
        {
            offsetX = Linker.LeadingElement.GetX() - _x;
            offsetY = Linker.LeadingElement.GetY() - _y;
        }

        UIElement.CapturePointer(e.Pointer);
    }

    public void DragHolding(object sender, PointerRoutedEventArgs e)
    {
        if (!_isdragging) { return; }

        double _x = e.GetCurrentPoint(Canvas).Position.X;
        double _y = e.GetCurrentPoint(Canvas).Position.Y;

        if (Linker == null)
        {
            SetX(_x + offsetX);
            SetY(_y + offsetY);
        }
        else
        {
            Linker.LinkerMove(_x + offsetX, _y + offsetY);
        }

        if (FunctionBox != null)
        {
            List<double> r = Utils.CalculateBoxOffset(_x + offsetX, _y + offsetY);
            FunctionBox.SetX(r[0]);
            FunctionBox.SetY(r[1]);
        }

    }

    public void DragReleased(object sender, PointerRoutedEventArgs e)
    {
        if (_isdragging == true)
        {
            _isdragging = false;
            UIElement.ReleasePointerCapture(e.Pointer);
        }
    }

    public void DragCaptureLoss(object sender, PointerRoutedEventArgs e)
    {
        _isdragging = false;
    }

    public void ResetAnimationPlaying()
    {
        if (isHalt != null)
        {
            isHalt.TrySetResult(true);
        }
        AnimationOnPlaying = null;
    }

    public void PlayAnimation(Animation animation)
    {
        if (animation.FrameCollection.Count() == 0)
        {
            return;
        }
        initX = GetX();
        initY = GetY();

        int i = GlobalAnimationTimer.CurrentFrame;
        AnimationOnPlaying = animation;
        if (i >= animation.FrameCollection.Count())
        {
            Visual.Offset = new Vector3((float)GetX(), (float)GetY(), 0);
            if (_isLoop)
            {
                i = i % animation.FrameCollection.Count();
                if (i == 0)
                {
                    Visual.Offset = new Vector3((float)GetX(), (float)GetY(), 0);
                }
            }
            else
            {
                Visual.Offset = new Vector3((float)GetX(), (float)GetY(), 0);
                return;
            }

        }

        //ElementCompositionPreview.GetElementVisual(UIElement).Offset = new Vector3(0, 0, 0);
        if (!animation.Relative)
        {
            var c = Animation.Translate(animation.Get(i));
            if (i == 0)
            {
                SetX(c.X);
                SetY(c.Y);
            }
            ElementCompositionPreview.GetElementVisual(UIElement).Offset = new Vector3((float)(c.X + initX), (float)(c.Y + initY), 0);
            if (c.Cosmetic != null)
            {
                string cc = CurrentCosmetic;
                try
                {
                    ChangeCosmetic(c.Cosmetic);
                }
                catch (Exception ex)
                {
                    ChangeCosmetic(cc);
                }
            }
            UIElement.Visibility = c.Visibility;

            return;
        }
        else
        {
            var c = Animation.Translate(animation.Get(i));
            // SetX(c.X + (double)initX);
            // SetY(c.Y + (double)initY);

            Visual.Offset = new Vector3((float)(c.X + initX), (float)(c.Y + initY), 0);
            if (c.Cosmetic != null)
            {
                string cc = CurrentCosmetic;
                try
                {
                    ChangeCosmetic(c.Cosmetic);
                }
                catch (Exception ex)
                {
                    ChangeCosmetic(cc);
                }
            }
            UIElement.Visibility = c.Visibility;

            return;
        }

    }

    public void ClearAnimationOnPlaying()
    {
        if (!(initX == null || initY == null))
        {
            Visual.Offset = new Vector3((float)initX, (float)initY, 0);
        }
        AnimationOnPlaying = null;
    }

    public void ResetPosition()
    {
        if (!(initX == null || initY == null))
        {
            Visual.Offset = new Vector3((float)initX, (float)initY, 0);
        }
    }
}