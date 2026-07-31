namespace Fishtrace;

/// <summary>
/// The parent of element bars
/// </summary>
public class ElementBarManager : Element
{
    public Utils.TwoWayDictionary<ElementBar, int> Index = new Utils.TwoWayDictionary<ElementBar, int>();
    private int count = 0;

    public ElementBarManager()
    {

    }

    public void AddElementbar(ElementBar e)
    {
        if (Index.TryGetRight(e, out var value))
        {
            return;
        }
        else
        {
            AddChild(e);
            Index.Add(e, count);
            count++;
        }
    }
}