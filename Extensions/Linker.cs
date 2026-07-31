using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;

namespace Fishtrace;

/// <summary>
/// 旧日空虚回响无人问答
/// </summary>
public class Linker
{
    //This class is for coupling elements hence you can treat the coupling elements as a single object
    //Sometimes you have to treat multiple elements as one object
    //When they are treated as a single object, the will move together, hide and show together 
    //Hence there should be a center of mass(Leading element) 
    //Linker will not override the SetX SetY method of coupling elements
    //Every coupling elements can access the linker instance 
    public Element LeadingElement;
    public List<Element> ElementList = new List<Element>();
    public List<double> OffsetX = new List<double>();
    public List<double> OffsetY = new List<double>();
    public ContainerVisual ContainerVisual;
    public ContainerVisual CanvasVisual;
    public Linker(Element leadingelement)
    {
        LeadingElement = leadingelement;
        ElementList.Add(leadingelement);
        LeadingElement.Linker = this;
        AddDummyToOffset();
        CompileOffset();

        CanvasVisual = (ContainerVisual)ElementCompositionPreview.GetElementVisual(LeadingElement.Canvas);
        ContainerVisual = CanvasVisual.Compositor.CreateContainerVisual();
        CanvasVisual.Children.InsertAtTop(ContainerVisual);
    }

    //This is not directionless(i.e. x.LinkElement(y) != y.LinkElement(x)) when e has a linker
    public void LinkElement(Element e)
    {
        if (ElementList.Contains(e)) { return; }
        ElementList.Add(e);

        if (e.Linker == null)
        {
            AddDummyToOffset();
        }
        else
        {
            MergeLinker(e.Linker);
        }


        //shi山代码
        if (e is ElementBar)
        {
            foreach (FunctionElement f in ((ElementBar)e).ChildList)
            {
                LinkElement(e);
            }
        }
        e.Linker = this;

        //神秘O(n^2)
        //shi山代码 

        while (OffsetX.Count() != ElementList.Count())
        {
            AddDummyToOffset();
        }
        CompileOffset();

        //shi山代码
    }

    public void GiveUpElement(Element e, bool PreserveLinking = true) //if PreserveLinking is true then decoupling a single element will not kill the whole linker
    {
        if (!PreserveLinking)
        {
            KillThisLinker();
            return;
        }

        if (ReferenceEquals(LeadingElement, e))
        {
            if (ElementList.Count == 1)
            {
                KillThisLinker();
                return;
            }
            else
            {
                int i = 0;
                ElementList.RemoveAt(i);
                OffsetX.RemoveAt(i);
                OffsetY.RemoveAt(i);
                LeadingElement.Linker = null;
                LeadingElement = ElementList[0];
                CompileOffset();
            }
        }
        else
        {
            int i = ElementList.IndexOf(e);
            ElementList.Remove(e);
            OffsetX.RemoveAt(i);
            OffsetY.RemoveAt(i);
            e.Linker = null;
        }
    }

    public void KillThisLinker()
    {
        foreach (Element u in ElementList)
        {
            u.Linker = null;
        }

        GC.Collect();
    }

    /// <summary>
    /// Consider when you have two elements, a and b. You want to use a single Linker to link them together.
    /// But Linker of a and b is not null. Hence you have to eliminate one Linker.
    /// Pass the Linker that should be eliminated and this function will handle the rest.
    /// </summary>
    /// <param name="l"></param>
    public void MergeLinker(Linker l)
    {
        int n1 = ElementList.Count();
        for (int i = 0; i < l.ElementList.Count(); i++)
        {
            if (ElementList.Contains(l.ElementList[i])) continue;
            Element e = l.ElementList[i];
            ElementList.Add(e);
            e.Linker = this;
        }
    }

    public void CompileOffset(int? index = null)
    {
        double X = LeadingElement.GetX();
        double Y = LeadingElement.GetY();
        if (index == null)
        {
            for (int i = 0; i < ElementList.Count(); i++)
            {
                OffsetX[i] = X - ElementList[i].GetX();
                OffsetY[i] = Y - ElementList[i].GetY();
            }
        }
        else
        {
            OffsetX[(int)index] = X - ElementList[(int)index].GetX();
            OffsetY[(int)index] = Y - ElementList[(int)index].GetY();
        }
    }

    // public void LinkerSetX(double x, Element? ignore = null)
    // {
    //     for (int i = 0; i < ElementList.Count(); i++)
    //     {
    //         if (ignore != null) { if (ignore == ElementList[i]) continue; }
    //         MainPage.Consoles.ConsoleOutput(ElementList[i].Name + "has canvas parent:" + (ElementList[i].UIElement.Parent is Canvas));
    //         ElementList[i].SetX(x - OffsetX[i]);
    //     }
    // }

    // public void LinkerSetY(double y, Element? ignore = null)
    // {
    //     for (int i = 0; i < ElementList.Count(); i++)
    //     {
    //         if (ignore != null) { if (ignore == ElementList[i]) continue; }
    //         ElementList[i].SetY(y - OffsetY[i]);
    //     }
    // }

    // public void LinkerMove(double x, double y)
    // {
    //     LinkerSetX(x);
    //     LinkerSetY(y);
    // }

    public void LinkerSetX(double x, Element? ignore = null)
    {
        for (int i = 0; i < ElementList.Count; i++)
        {
            var el = ElementList[i];
            if (ignore != null && ReferenceEquals(ignore, el)) continue;
            if (el == null || el.Canvas == null) continue;  // 防御：元素已卸载
            if (i >= OffsetX.Count) break;                   // 防御：偏移量缺失
            el.SetX(x - OffsetX[i]);
        }
    }

    public void LinkerSetY(double y, Element? ignore = null)
    {
        for (int i = 0; i < ElementList.Count; i++)
        {
            var el = ElementList[i];
            if (ignore != null && ReferenceEquals(ignore, el)) continue;
            if (el == null || el.Canvas == null) continue;
            if (i >= OffsetY.Count) break;
            el.SetY(y - OffsetY[i]);
        }
    }

    public void LinkerMove(double x, double y)
    {
        try
        {
            LinkerSetX(x);
            LinkerSetY(y);
        }
        catch (Exception ex)
        {
            // 输出到你的控制台，方便定位
            MainPage.Consoles.ConsoleOutput($"{ex.Message}");
            // 可以选择不重新抛出，以免崩溃
        }
    }

    private void AddDummyToOffset()
    {
        OffsetX.Add(0);
        OffsetY.Add(0);
    }

    public void MaintainContainer()
    {
        for (int i = 0; i < ElementList.Count(); i++)
        {
            var e = ElementList[i];
            Visual visual = ElementCompositionPreview.GetElementVisual(e.UIElement);
            (visual.Parent as ContainerVisual)?.Children.Remove(visual);
            ContainerVisual.Children.InsertAtTop(visual);
            //visual.Offset = new Vector3((float)OffsetX[i], (float)OffsetY[i], 0);
        }
    }

    public void ContainerSetX(double x)
    {
        var offset = ContainerVisual.Offset;
        ContainerVisual.Offset = new Vector3((float)x, offset.Y, offset.Z);
    }

    public void ContainerSetY(double y)
    {
        var offset = ContainerVisual.Offset;
        ContainerVisual.Offset = new Vector3(offset.X, (float)y, offset.Z);
    }

    public void ContainerMove(double x, double y)
    {
        ContainerVisual.Offset = new Vector3((float)x, (float)y, 0);
    }
}