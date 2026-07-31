using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Fishtrace;


public class LeftRightButton : FunctionElement
{
    public bool _isForward = true;
    public LeftRightButton(FrameworkElement f, string name, bool isforward) : base(f, name)
    {
        _isForward = isforward;
        AddAction(OnClick);
    }

    public void OnClick()
    {
        if (!(Parent is ElementBar)) return;

        var eb = (ElementBar)Parent;
        if (_isForward)
        {
            eb.FlipForward();
        }
        else
        {
            eb.FlipBackward();
        }


    }

}