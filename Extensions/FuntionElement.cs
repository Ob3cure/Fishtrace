using System.Diagnostics;
using System.Numerics;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.Storage.FileProperties;

namespace Fishtrace;


public class FunctionElement : CommonElement
{
    //This is the main part in ui.
    //Which is the class for all buttons.
    //There are two types of FunctionElement, Action and Resource.
    //Action functions are related to some operation by the application such as fft and start record.
    //Resources are refer to characters import from the user.
    public static string Region_Action = "Action";
    public static string Region_Resource = "Resource";
    public string Region = Region_Action;
    public List<Action> ActionList = new List<Action>();
    public List<Action<FunctionElement>> ActionListWithArg = new List<Action<FunctionElement>>();
    public CommonElement? Description;
    private bool _isleftbuttonpressed = false;
    public FunctionElement(FrameworkElement f, string? name = null) : base(f, name)
    {
        Microsoft.UI.Xaml.Controls.Canvas.SetZIndex(UIElement, 20);
        AddPressedEvent(OnClickResponse);
        AddReleasedEvent(OnReleaseResponse);
    }

    public void AddAction(Action action)
    {
        ActionList.Add(action);
    }

    public void AddAction(Action<FunctionElement> action)
    {
        ActionListWithArg.Add(action);
    }

    public void RemoveAction(Action action)
    {
        try
        {
            ActionList.Remove(action);
        }
        catch (Exception)
        {
            throw new InvalidOperationException();
        }
    }

    public void RemoveAction(Action<FunctionElement> action)
    {
        try
        {
            ActionListWithArg.Remove(action);
        }
        catch (Exception)
        {
            throw new InvalidOperationException();
        }
    }

    /// <summary>
    /// Use this function to add a decription text block of the function element
    /// </summary>
    public void AddDescription(CommonElement c)
    {
        c.Hide();
        Description = c;
        c.SetX(GetX() + 10);
        c.SetY(GetY() + UIElement.Height / 5);
        if (Linker == null)
        {
            Linker = new Linker(this);
        }
        Linker.LinkElement(Description);
        Microsoft.UI.Xaml.Controls.Canvas.SetZIndex(c.UIElement, 42);
    }

    public void AddDescription(string s)
    {
        TextBlock t = Utils.CreateTextBlock(s);
        this.Canvas.Children.Add(t);
        t.Width = UIElement.Width ;
        t.Height = UIElement.Height;
        t.IsHitTestVisible = false;
        AddDescription(new CommonElement(t, Name + "_DescriptionText"));
    }

    public void SetRegion(string s)
    {
        if (s != Region_Action || s != Region_Resource) { throw new InvalidOperationException(); }
        Region = s;
    }

    public virtual void OnClickResponse(object sender, PointerRoutedEventArgs e)
    {
        var properties = e.GetCurrentPoint(Canvas).Properties;
        if (!properties.IsLeftButtonPressed) return;

        UIElement.Opacity = 0.5;
        UIElement.CapturePointer(e.Pointer);
        _isleftbuttonpressed = true;
    }


    public virtual void OnReleaseResponse(object sender, PointerRoutedEventArgs e)
    {
        if (e.GetCurrentPoint(Canvas).Properties.IsLeftButtonPressed) { return; }
        if (!_isleftbuttonpressed) return;

        _isleftbuttonpressed = false;
        UIElement.Opacity = 1;
        UIElement.ReleasePointerCapture(e.Pointer);
        foreach (Action a in ActionList)
        {
            a();
        }

        foreach (Action<FunctionElement> a in ActionListWithArg)
        {
            a(this);
        }

        if (Parent is FunctionBox)
        {
            ((FunctionBox)Parent).DeActive();
        }
    }

    public override void Hide()
    {
        base.Hide();
        if (Description != null) { Description.Hide(); }
    }

    public override void Show()
    {
        base.Show();
        if (Description != null) { Description.Show(); }
    }

    public void EnableSpringFeedback()
    {
        if (UIElement == null) return;
        var visual = ElementCompositionPreview.GetElementVisual(UIElement);
        var compositor = visual.Compositor;


        double w = UIElement.Width;
        double h = UIElement.Height;
        if (w > 0 && h > 0)
            visual.CenterPoint = new Vector3((float)w / 2, (float)h / 2, 0);

        // 按下缩小
        UIElement.PointerPressed += (s, e) =>
        {
            var anim = compositor.CreateSpringVector3Animation();
            anim.Target = "Scale";
            anim.FinalValue = new Vector3(0.9f, 0.9f, 1f);
            anim.DampingRatio = 0.9f;
            anim.Period = TimeSpan.FromSeconds(0.01);
            visual.StartAnimation("Scale", anim);
        };

        // 释放/取消/离开 回弹
        void ResetScale()
        {
            var anim = compositor.CreateSpringVector3Animation();
            anim.Target = "Scale";
            anim.FinalValue = new Vector3(1f, 1f, 1f);
            anim.DampingRatio = 0.95f;
            anim.Period = TimeSpan.FromSeconds(0.01);
            visual.StartAnimation("Scale", anim);
        }

        UIElement.PointerReleased += (s, e) => ResetScale();
        UIElement.PointerCanceled += (s, e) => ResetScale();
        UIElement.PointerExited += (s, e) => ResetScale();
    }


    public void EnableHoverGlow()
    {
        if (!(UIElement is Image img)) return;

        var visual = ElementCompositionPreview.GetElementVisual(img);
        var compositor = visual.Compositor;

        // 保存初始透明度（如果已经设置了）
        float originalOpacity = (float)img.Opacity;

        UIElement.PointerEntered += (s, e) =>
        {
            var anim = compositor.CreateScalarKeyFrameAnimation();
            anim.InsertKeyFrame(0.0f, originalOpacity);
            anim.InsertKeyFrame(1.0f, originalOpacity * 0.8f); // 变暗或变亮，这里变暗
            anim.Duration = TimeSpan.FromSeconds(0.15);
            visual.StartAnimation("Opacity", anim);
        };

        UIElement.PointerExited += (s, e) =>
        {
            var anim = compositor.CreateScalarKeyFrameAnimation();
            anim.InsertKeyFrame(0.0f, originalOpacity * 0.6f);
            anim.InsertKeyFrame(1.0f, originalOpacity);
            anim.Duration = TimeSpan.FromSeconds(0.15);
            visual.StartAnimation("Opacity", anim);
        };
    }
}