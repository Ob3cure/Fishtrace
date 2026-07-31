using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.WindowsAppSDK.Runtime.Packages;

namespace Fishtrace;


public class FunctionBox : Element
{
    //FunctionBox contains the functions in an extension window
    //It is different to Inputbox and HintBox, it uses Linker to link functions while HintBox and InputBox do not
    public bool _isActive = false;
    public Element Owner;
    public List<FunctionElement> FunctionList = new List<FunctionElement>();
    public FunctionBox(FrameworkElement box, string name, Element owner)
    {
        if (!(box.Parent is Canvas)) { throw new InvalidOperationException(); }
        Canvas = (Canvas)box.Parent;
        Name = name;
        Owner = owner;
        if (!(box is Image)) { throw new NullReferenceException(); }
        UIElement = box;
        Canvas.SetZIndex(UIElement, 40);

        Linker = new Linker(this);

        Hide();
    }

    public void AddFunction(FunctionElement f)
    {
        if (FunctionList.Count < 5)
        {
            if (Linker == null) { throw new NullReferenceException(); }
            AddChild(f);
            f.SetX(GetX() + 10);
            f.SetY(GetY() + 35 * FunctionList.Count() + 5);
            Canvas.SetZIndex(f.UIElement, 41);
            FunctionList.Add(f);
            Linker.LinkElement(f);
            f.Parent = this;

        }
        else
        {
            throw new ArgumentOutOfRangeException();
        }
    }

    public void AddFunction(Action action, string description = "")
    {
        Image I = Utils.CreateImage("Assets/180x30GrayBlock.png");
        I.Width = 180;
        I.Height = 30;
        Canvas.Children.Add(I);
        FunctionElement f = new FunctionElement(I, Name + "_Function");
        f.Hide();
        f.AddAction(action);
        AddFunction(f);
        f.AddDescription(description);

        f.EnableHoverGlow();
        f.EnableSpringFeedback();
    }

    public void Active(double x, double y)
    {
        _isActive = true;

        List<double> r = CalculateOffset(x, y);
        SetX(r[0]);
        SetY(r[1]);

        Show();
    }

    public static List<double> CalculateOffset(double x, double y)
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

        var r = new List<double> { box_X, box_Y };
        return r;
    }

    public void DeActive()
    {
        _isActive = false;
        Hide();
    }

    public override void SetX(double x)
    {
        base.SetX(x);
        if (Linker != null) { Linker.LinkerSetX(x, this); }
    }

    public override void SetY(double y)
    {
        base.SetY(y);
        if (Linker != null) { Linker.LinkerSetY(y, this); }
    }

    public override void Hide()
    {
        base.Hide();
        foreach (FunctionElement f in FunctionList)
        {
            f.Hide();
        }
    }

    public override void Show()
    {
        base.Show();
        foreach (FunctionElement f in FunctionList)
        {
            f.Show();
        }
    }
}