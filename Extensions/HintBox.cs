using System.Runtime.InteropServices;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.WindowsAppSDK.Runtime.Packages;

namespace Fishtrace;



public class HintBox : Element
{
    //Hint Box in size 200x200
    public CommonElement Box;
    public CommonElement Text;
    public Element Owner;
    public bool _istouching = false;
    public CancellationTokenSource? _cls;
    public double xoffset = 3;

    public HintBox(CommonElement box, CommonElement text, string name,Element owner)
    {
        Box = box;
        Text = text;
        Hide();
        Box.UIElement.IsHitTestVisible = false;
        Name = name;
        UIElement = box.UIElement;
        Owner = owner;
        Canvas.SetZIndex(Box.UIElement,30);
        Canvas.SetZIndex(Text.UIElement,31);
    }

    public void SetHint(string s)
    {
        var t = (TextBlock)Text.UIElement;
        t.Text = s;
    }

    public async void SetVisible(double x, double y)
    {
        _cls = new CancellationTokenSource();

        try
        {
            await Task.Delay(1500, _cls.Token);
        }
        catch (TaskCanceledException)
        {
            return;
        }

        List<double> r = CalculateOffset(x,y);
        SetX(r[0]);
        SetY(r[1]);

        Show();

        while (_istouching)
        {
            await Task.Delay(50);
        }

        Hide();
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

    public override void SetX(double x)
    {
    
        Box.SetX(x);
        Text.SetX(x + 3);
    }

    public override void SetY(double y)
    {
        Box.SetY(y);
        Text.SetY(y);
    }

    public override void Hide()
    {
        Box.Hide();
        Text.Hide();
    }

    public override void Show()
    {
        Box.Show();
        Text.Show();
    }
}