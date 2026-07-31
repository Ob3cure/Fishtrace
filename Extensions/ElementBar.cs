using System.Security.Cryptography.X509Certificates;
using Microsoft.UI.Composition;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.UserDataTasks;
using Windows.Foundation.Metadata;

namespace Fishtrace;
//神秘shi山代码

public class ElementBar : Element
{
    //The child reference of ElementBar will be pointing to the next ElementBar instead of Functions on the bar.
    //While FunctionList is the list holding the referencs from functions on the bar.
    public const string Status_AnimationBar = "Animation";
    public const string Status_CharacterBar = "Character";
    public ElementBar? NextElementBar;
    public ElementBar? PreviousElementBar;
    public int Index;
    /// <summary>
    /// Count of "useful" elements.
    /// Which can be used to calculate offset when calling add function easily.
    /// </summary>
    public int VisualElementCount = 0;
    public ElementBar(Image I, string name, int? index = null)
    {
        if (!(I.Parent is Canvas && I.Parent != null)) { throw new InvalidOperationException(); }
        Canvas = (Canvas)I.Parent;
        UIElement = I;
        Name = name;

        if (index == null)
        {
            Index = 0;
        }
    }

    public virtual void AddNextElementBar(ElementBar elementBar)
    {
        NextElementBar = elementBar;
        elementBar.PreviousElementBar = this;
    }

    public virtual void AddPreviousElementBar(ElementBar elementBar)
    {
        PreviousElementBar = elementBar;
        elementBar.AddNextElementBar(this);
    }

    public virtual void AddFunction(Element e, bool setposition = true, bool iscountincrement = true)
    {
        if (ChildList.Contains(e)) return;
        if (VisualElementCount == 3 && iscountincrement)
        {
            //Adding next page button
            if (NextElementBar == null)
            {
                Image right = Utils.CreateImage("Assets/80x40RightArrow.png");
                Canvas.Children.Add(right);
                Element el = new LeftRightButton(right, Name + "_RightButton", true);
                if (UIElement.Visibility == Visibility.Collapsed)
                {
                    el.Hide();
                }
                el.SetX(836.677);
                el.SetY(401.55);
                AddChild(el);

                ElementBar eb = CreateElmenetBar(Name + "_NextEB", Canvas, false, Index + 1);
                AddNextElementBar(eb);
                eb.AddFunction(e, setposition, iscountincrement);
                //Adding previous page button
                Image left = Utils.CreateImage("Assets/80x40LeftArrow.png");
                Canvas.Children.Add(left);
                LeftRightButton l = new LeftRightButton(left, Name + "_LeftButton", false);
                l.SetX(836.677);
                l.SetY(441.55);
                eb.AddFunction(l, false, false);
                eb.Hide();

                if (!(Parent is ElementBarManager))
                {
                    throw new InvalidOperationException();
                }
                else
                {
                    Parent.AddChild(eb);
                }
            }
            else
            {
                NextElementBar.AddFunction(e, setposition, iscountincrement);
            }
        }
        else
        {
            if (setposition)
            {
                if (GetX() == 50) //Horizontal Bar
                {
                    e.SetX(123.3 * VisualElementCount + 70.656);
                    e.SetY(20);
                }
                else
                {
                    e.SetX(835);
                    e.SetY(123.3 * VisualElementCount + 31.65);
                }
            }
            if (iscountincrement)
            {
                VisualElementCount++;
            }
            AddChild(e);
        }

        if (e.Parent.UIElement.Visibility == Microsoft.UI.Xaml.Visibility.Collapsed)
        {
            e.Hide();
        }
    }

    public void FlipForward()
    {
        Hide();
        if (NextElementBar == null) return;
        NextElementBar.Show();
        if (MainPage.CurrentRightBarStatus == Status_AnimationBar)
        {
            MainPage.CurrentAnimationBar = NextElementBar;
        }
        else
        {
            MainPage.CurrentCharacterBar = (CharacterSlotBar)NextElementBar;
        }
    }

    public void FlipBackward()
    {
        Hide();
        if (PreviousElementBar == null) return;
        PreviousElementBar.Show();
        if (MainPage.CurrentRightBarStatus == Status_AnimationBar)
        {
            MainPage.CurrentAnimationBar = PreviousElementBar;
        }
        else
        {
            MainPage.CurrentCharacterBar = (CharacterSlotBar)PreviousElementBar;
        }
    }

    public void SetVertical()
    {
        SetX(825);
        SetY(10);
    }

    public void SetHorizontal()
    {
        SetX(50);
        SetY(10);
    }

    public static ElementBar CreateElmenetBar(string name, Canvas canvas, bool horizontal, int? index = null)
    {
        if (horizontal)
        {
            Image I = Utils.CreateImage("Assets/740x100WhiteBar.png");
            canvas.Children.Add(I);
            ElementBar eb = new ElementBar(I, name, index);
            eb.SetHorizontal();
            return eb;
        }
        else
        {
            Image I = Utils.CreateImage("Assets/100x520WhiteBar.png");
            canvas.Children.Add(I);
            ElementBar eb = new ElementBar(I, name, index);
            eb.SetVertical();
            return eb;
        }
    }
}

public class CharacterSlotBar : ElementBar
{
    public DraggableCharacter? DraggableCharacter;
    public CharacterSlot? LatestAnimationSlot;
    public CharacterSlotBar(Image I, string name, int? index = null) : base(I, name, index)
    {

    }

    public CharacterSlotBar(ElementBar e) : this((Image)e.UIElement, e.Name + "_Cast" + e.Index)
    {

    }

    public override void AddFunction(Element e, bool setposition = true, bool iscountincrement = true)
    {
        base.AddFunction(e, setposition, iscountincrement);
        if (e is CharacterSlot)
        {
            LatestAnimationSlot = (CharacterSlot)e;
            if (DraggableCharacter == null)
            {
                DraggableCharacter = ((CharacterSlot)e).DraggableCharacter;
            }
        }
    }

    public static CharacterSlotBar CreateCharacterBar(string name, Canvas canvas, bool horizontal = false)
    {
        CharacterSlotBar eb = new CharacterSlotBar(CreateElmenetBar(name, canvas, horizontal));
        eb.AddFunction(CharacterSlot.CreateCharacterSlot(canvas, name));

        return eb;
    }
}