using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;

namespace Fishtrace;


public class Puller : FunctionElement
{
    public ElementBarManager ElementBarManager;
    public bool _isdragging = false;
    public double offsetX;
    public double offsetY;
    public double xleftlimit;
    public double ydownlimit;
    public bool HorizontalPull;

    // -------- 回弹相关 --------
    private bool _isRebounding = false;
    private const double EdgeThreshold = 5;    // 像素
    public Puller(FrameworkElement f, ElementBarManager elementBarManager, string name, bool horizontalpull = false) : base(f, name)
    {
        ElementBarManager = elementBarManager;
        //Linker = new Linker(this);
        AddChild(elementBarManager);
        HorizontalPull = horizontalpull;

        xleftlimit = GetX();
        ydownlimit = GetY();

        AddPressedEvent(DragPressed);
        AddHoldingEvent(DragHolding);
        AddReleasedEvent(DragReleased);
        AddEnteredEvent(OnPointerEnter);
        AddReleasedEvent(OnPointerRelease);
        AddCaptureLossEvent(DragCaptureLoss);
    }

    public void DragPressed(object sender, PointerRoutedEventArgs e)
    {
        if (_isRebounding)
        {
            // 停止所有正在进行的动画
            var visual = ElementCompositionPreview.GetElementVisual(this.UIElement);
            visual.StopAnimation("Offset");
            foreach (ElementBar eb in ElementBarManager.ChildList)
            {
                var barVisual = ElementCompositionPreview.GetElementVisual(eb.UIElement);
                barVisual.StopAnimation("Offset");
            }
            _isRebounding = false;
        }

        if (e.GetCurrentPoint(Canvas).Properties.IsLeftButtonPressed == false) { return; }

        _isdragging = true;

        double _x = e.GetCurrentPoint(Canvas).Position.X;
        double _y = e.GetCurrentPoint(Canvas).Position.Y;


        if (HorizontalPull)
        {
            if (_x >= xleftlimit && _x <= 960 - UIElement.Width)
            {
                offsetX = GetX() - _x;
            }
        }
        else
        {
            if (_y <= ydownlimit + UIElement.Height && _y >= 0)
            {
                offsetY = GetY() - _y;
            }
        }
        // if (Linker == null)
        // {


        // }
        // else
        // {
        //     offsetX = Linker.LeadingElement.GetX() - _x;
        //     offsetY = Linker.LeadingElement.GetY() - _y;
        // }

        UIElement.CapturePointer(e.Pointer);

    }

    public void DragHolding(object sender, PointerRoutedEventArgs e)
    {
        if (!_isdragging) { return; }

        double _x = e.GetCurrentPoint(Canvas).Position.X;
        double _y = e.GetCurrentPoint(Canvas).Position.Y;


        if (HorizontalPull)
        {
            if (_x >= xleftlimit && _x <= 960 - UIElement.Width)
            {
                SetX(_x + offsetX);
            }
        }
        else
        {
            if (_y <= ydownlimit + UIElement.Height && _y >= 0)
            {
                SetY(_y + offsetY);
            }
        }
        // if (Linker == null)
        // {

        // }
        // else
        // {
        //     if (HorizontalPull)
        //     {
        //         if (_x >= xleftlimit && _x <= 960 - UIElement.Width)
        //         {
        //             Linker.LinkerSetX(_x + offsetX);
        //         }
        //     }
        //     else
        //     {
        //         if (_y <= ydownlimit + UIElement.Height && _y >= 0)
        //         {
        //             Linker.LinkerSetY(_y + offsetY);
        //         }
        //     }
        // }

        if (FunctionBox != null)
        {
            List<double> r = Utils.CalculateBoxOffset(_x + offsetX, _y + offsetY);
            FunctionBox.SetX(r[0]);
            FunctionBox.SetY(r[1]);
        }


    }

    public void DragReleased(object sender, PointerRoutedEventArgs e)
    {
        if (e.GetCurrentPoint(Canvas).Properties.IsLeftButtonPressed) return;
        if (_isdragging == true)
        {
            _isdragging = false;
            UIElement.ReleasePointerCapture(e.Pointer);
            Rebound();
        }
    }

    public void DragCaptureLoss(object sender, PointerRoutedEventArgs e)
    {
        _isdragging = false;
        UIElement.ReleasePointerCapture(e.Pointer);
        Rebound();
    }


    // ---------- 回弹逻辑 ----------
    // public void Rebound()
    // {
    //     if (_isRebounding || Linker == null) return;
    //     Linker.MaintainContainer();

    //     // 现在 currentX/Y 直接从容器Offset获取（已经同步）
    //     double currentX = GetX();
    //     double currentY = GetY();
    //     double targetX = currentX, targetY = currentY;

    //     // 计算目标位置（原有逻辑保持不变）
    //     if (!HorizontalPull)
    //     {
    //         double leftBound = xleftlimit;
    //         double rightBound = 960 - UIElement.Width;
    //         if (currentX - leftBound > -1 * EdgeThreshold)
    //             targetX = leftBound;
    //         else
    //             targetX = xleftlimit;
    //         targetY = ydownlimit; // 固定 Y
    //     }
    //     else
    //     {
    //         double topBound = 0;
    //         double bottomBound = MainPage.CanvasUpperLimit_Y - UIElement.Height;
    //         if (currentY - topBound < EdgeThreshold)
    //             targetY = topBound;
    //         else
    //             targetY = ydownlimit;
    //         targetX = xleftlimit;
    //     }


    //     var container = Linker.ContainerVisual;
    //     var compositor = container.Compositor;

    //     container.Offset = new Vector3((float)0, (float)0, 0);

    //     var springAnim = compositor.CreateSpringVector3Animation();
    //     springAnim.DampingRatio = 0.7f;
    //     springAnim.Period = TimeSpan.FromSeconds(0.01);
    //     springAnim.FinalValue = new Vector3((float)(targetX - currentX), (float)(targetY - currentY), 0);


    //     _isRebounding = true;

    //     var batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
    //     batch.Completed += (s, e) =>
    //     {
    //         container.Offset = new Vector3(0, 0, 0);
    //         Linker.LinkerMove(targetX, targetY);  // 同步Canvas位置
    //         _isRebounding = false;
    //     };

    //     container.StartAnimation("Offset", springAnim);
    //     batch.End();
    // }

    public void Rebound()
    {
        double currentX = GetX();
        double currentY = GetY();
        double targetX = currentX, targetY = currentY;
        if (HorizontalPull)
        {
            if (currentX - 960 + 50 > -1 * EdgeThreshold)
            {
                targetX = 960 - UIElement.Width;
            }
            else
            {
                targetX = xleftlimit;
            }
            targetY = ydownlimit; // 固定 Y
        }
        else
        {
            if (currentY - 0 < EdgeThreshold)
            {
                targetY = 0;
            }
            else
            {
                targetY = ydownlimit;
            }
            targetX = xleftlimit;
        }

        SetX(targetX);
        SetY(targetY);
    }

    public void OnPointerEnter(object sender, PointerRoutedEventArgs e)
    {
        if (!(UIElement.Opacity != 0)) return;
        FadingEffect.FadeIn(UIElement, 300);
    }

    public void OnPointerRelease(object sender, PointerRoutedEventArgs e)
    {
        if (e.GetCurrentPoint(Canvas).Properties.IsLeftButtonPressed) return;

        FadingEffect.FadeOut(UIElement, 300);
    }
}