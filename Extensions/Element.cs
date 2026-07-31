using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.Generic;
using System.Reflection;
using Windows.ApplicationModel;
using Windows.Media.PlayTo;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Notifications;

namespace Fishtrace;

public abstract class Element
{
    //This is the base class of nearly all ui elemens
    public Canvas Canvas;
    public List<Element> ChildList = new List<Element>();
    public Dictionary<Element, (double offsetX, double offsetY)> ChildOffset = new Dictionary<Element, (double offsetX, double offsetY)>();
    public Element? Parent;
    public Canvas? CanvasParent
    {
        get
        {
            if (!(UIElement.Parent is Canvas))
            {
                return null;
            }
            else
            {
                return (Canvas)UIElement.Parent;
            }
        }
    }
    public string Name = "";
    public FrameworkElement UIElement = new TextBlock();
    //Cosmetics store the paths of cosmetic images
    public Dictionary<string, string> Cosmetics = new Dictionary<string, string>();
    public Dictionary<string, WriteableBitmap> CosmeticCache = new Dictionary<string, WriteableBitmap>();
    private List<string> _cosmetics = new List<string>();
    public ConsoleTextBlock? ConsoleOwned = null;
    public Linker? Linker;
    public string? HintReceive;
    public string? InputReceive;
    public string? CurrentCosmetic;
    public Visibility Visibility
    {
        get
        {
            return UIElement.Visibility;
        }
        set
        {
            UIElement.Visibility = value;
        }
    }
    public bool isHiding
    {
        get
        {
            return Visibility == Visibility.Collapsed;
        }
    }

    public bool isShowing
    {
        get
        {
            return Visibility == Visibility.Visible;
        }
    }

    public double X
    {
        get
        {
            return GetX();
        }
        set
        {
            SetX(value);
        }
    }

    public double Y
    {
        get
        {
            return GetY();
        }
        set
        {
            SetY(value);
        }
    }

    public void AddChild(Element o)
    {
        if (ChildList.Contains(o)) return;
        ChildList.Add(o);
        o.Parent = this;
        UpdateOffset(o);
    }

    public void UpdateOffset(Element o)
    {
        double offsetX = o.X - X;
        double offsetY = o.Y - Y;

        var offset = (offsetX, offsetY);
        ChildOffset.Add(o, offset);
    }

    public void RemoveChild(Element o)
    {
        ChildList.Remove(o);
        ChildOffset.Remove(o);
    }

    public virtual void Hide()
    {
        UIElement.Visibility = Visibility.Collapsed;
        foreach (Element e in ChildList)
        {
            e.Hide();
        }
    }

    public virtual void Show()
    {
        UIElement.Visibility = Visibility.Visible;
        foreach (Element e in ChildList)
        {
            e.Show();
        }
    }

    public double GetX()
    {
        double _x = 0;
        try
        {
            _x = Canvas.GetLeft(UIElement);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");
        }
        return _x;
    }

    public double GetY()
    {
        double _y = 0;
        try
        {
            _y = Canvas.GetTop(UIElement);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");
        }
        return _y;
    }

    public virtual void SetX(double x)
    {
        try
        {
            Canvas.SetLeft(UIElement, x);
            foreach (Element e in ChildList)
            {
                if (ChildOffset.TryGetValue(e, out var value))
                {
                    e.SetX(x + value.offsetX);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");
        }
    }

    public virtual void SetY(double y)
    {
        try
        {
            Canvas.SetTop(UIElement, y);
            foreach (Element e in ChildList)
            {
                if (ChildOffset.TryGetValue(e, out var value))
                {
                    e.SetY(y + value.offsetY);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");
        }
    }

    public virtual void AddPressedEvent(PointerEventHandler handler)
    {
        UIElement.PointerPressed += handler;
    }

    public virtual void RemovePressedEvent(PointerEventHandler handler)
    {
        UIElement.PointerPressed -= handler;
    }

    public virtual void AddHoldingEvent(PointerEventHandler handler)
    {
        UIElement.PointerMoved += handler;
    }

    public virtual void RemoveHoldingEvent(PointerEventHandler handler)
    {
        UIElement.PointerMoved -= handler;
    }

    public virtual void AddReleasedEvent(PointerEventHandler handler)
    {
        UIElement.PointerReleased += handler;
    }

    public virtual void RemoveReleasedEvent(PointerEventHandler handler)
    {
        UIElement.PointerReleased -= handler;
    }

    public virtual void AddCancelledEvent(PointerEventHandler handler)
    {
        UIElement.PointerCanceled += handler;
    }

    public virtual void RemoveCancelledEvent(PointerEventHandler handler)
    {
        UIElement.PointerCanceled -= handler;
    }

    public virtual void AddCaptureLossEvent(PointerEventHandler handler)
    {
        UIElement.PointerCaptureLost += handler;
    }

    public virtual void RemoveCaptureLossEvent(PointerEventHandler handler)
    {
        UIElement.PointerCaptureLost -= handler;
    }

    public virtual void AddEnteredEvent(PointerEventHandler handler)
    {
        UIElement.PointerEntered += handler;
    }

    public virtual void RemoveEnteredEvent(PointerEventHandler handler)
    {
        UIElement.PointerEntered -= handler;
    }

    public virtual void AddExitedEvent(PointerEventHandler handler)
    {
        UIElement.PointerExited += handler;
    }

    public virtual void RemoveExitedEvent(PointerEventHandler handler)
    {
        UIElement.PointerExited -= handler;
    }

    public virtual void pressed()
    {

    }

    public virtual void touched()
    {

    }

    public virtual void released()
    {

    }

    public virtual void holding()
    {

    }

    //You can use this to achieve some special effect 
    //Such as Click response.
    public void AddCosmetic(string name, string path)
    {
        Cosmetics.Add(name, path);
        _cosmetics.Add(name);
    }

    public async void ChangeCosmetic(string s)
    {
        if (!(UIElement is Image)) return;

        var I = (Image)UIElement;
        try
        {
            CosmeticCache.TryGetValue(s, out WriteableBitmap? bm);
            if (bm == null)
            {
                Cosmetics.TryGetValue(s, out string? value);
                if (value == null) return;
                WriteableBitmap source = await Utils.LoadWriteableBitmapFromPathAsync(value);
                I.Source = source;
                CurrentCosmetic = s;
                CosmeticCache.Add(s, source);
            }
            else
            {
                I.Source = bm;
                CurrentCosmetic = s;
            }
        }
        catch
        {
            throw new InvalidOperationException();
        }
    }



    public void NextCosmetic()
    {
        if (CurrentCosmetic == null) return;
        if (_cosmetics.Count() == 0) return;

        int index = _cosmetics.IndexOf(CurrentCosmetic);
        if (index == -1)
        {
            return;
        }
        else
        {
            if (index + 1 == _cosmetics.Count())
            {
                ChangeCosmetic(_cosmetics[0]);
            }
            else
            {
                ChangeCosmetic(_cosmetics[index + 1]);
            }
        }
    }

    public void PreviousCosmetic()
    {
        if (CurrentCosmetic == null) return;
        if (_cosmetics.Count() == 0) return;

        int index = _cosmetics.IndexOf(CurrentCosmetic);
        if (index == -1)
        {
            return;
        }
        else
        {
            if (index - 1 == -1)
            {
                ChangeCosmetic(_cosmetics[_cosmetics.Count() - 1]);
            }
            else
            {
                ChangeCosmetic(_cosmetics[index - 1]);
            }
        }
    }

    public void ConsoleWrite(string s)
    {
        if (ConsoleOwned == null)
        {
            ConsoleTextBlock? _ = MainPage.Consoles.GetAvaliableConsole(this);
            if (_ != null)
            {
                ConsoleOwned = _;
                ConsoleWrite(s);
            }
            return;
        }

        ConsoleOwned.ConsoleOutput($"{Name} is using this console:" + s);
    }
}