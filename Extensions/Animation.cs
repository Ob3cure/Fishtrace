using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Fishtrace;


public class Animation
{
    //FrameCollection stores the information of each frame
    //The format is "x:XXX,y:XXX,Cosmetic:XXX,Visibility:X"
    public string Name;
    public Dictionary<int, string> FrameCollection = new Dictionary<int, string>();
    //FPS
    public int FrameRate;
    //Relative or absolute position
    public double TimeElapse;
    public bool Relative = true;
    public DraggableCharacter? DraggableCharacter;
    public double initX;
    public double initY;
    public List<string> Cosmetics = new List<string>();
    public Animation(int framerate, string name)
    {
        Name = name;
        FrameRate = framerate;
        TimeElapse = 1.0 / FrameRate;
    }

    public void Record(string s)
    {
        if (FrameCollection.Keys.Contains(GlobalRecordTimer.CurrentFrame))
        {
            FrameCollection.Remove(GlobalRecordTimer.CurrentFrame);
        }
        FrameCollection.Add(GlobalRecordTimer.CurrentFrame, s);
    }

    public void Record(double x, double y, string cosmetic, Visibility visibility)
    {
        string v = "";
        if (visibility == Visibility.Visible) { v = "v"; }
        else { v = "c"; }
        Record("x:" + x + ",y:" + y + ",Cosmetic:" + cosmetic + ",Visibility:" + v);
        if (!Cosmetics.Contains(cosmetic))
        {
            Cosmetics.Add(cosmetic);
        }
    }

    public void Record(Element e)
    {
        if (!Relative)
        {
            if (e.CurrentCosmetic == null)
            {
                Record(e.GetX(), e.GetY(), "null", e.UIElement.Visibility);
            }
            else
            {
                Record(e.GetX(), e.GetY(), e.CurrentCosmetic, e.UIElement.Visibility);
            }
        }
        else
        {
            if (FrameCollection.Count() == 0)
            {
                if (e.CurrentCosmetic == null)
                {
                    Record(0, 0, "null", e.UIElement.Visibility);
                }
                else
                {
                    Record(0, 0, e.CurrentCosmetic, e.UIElement.Visibility);
                }
                initX = e.GetX();
                initY = e.GetY();
            }
            else
            {
                if (e.CurrentCosmetic == null)
                {
                    Record(e.GetX() - initX, e.GetY() - initY, "null", e.UIElement.Visibility);
                }
                else
                {
                    Record(e.GetX() - initX, e.GetY() - initY, e.CurrentCosmetic, e.UIElement.Visibility);
                }
            }
        }

    }

    public void AddDraggableCharacter(DraggableCharacter draggableCharacter)
    {
        DraggableCharacter = draggableCharacter;
        DraggableCharacter.Hide();
    }

    // public static (double X, double Y) Translate(string s)
    // {
    //     string input = s;

    //     string[] parts = input.Split(',');

    //     // 2. 分别取第二部分（索引1）
    //     string xStr = parts[0].Split(':')[1]; // 取出 "123.45"
    //     string yStr = parts[1].Split(':')[1]; // 取出 "67.89"

    //     // 3. 转换成数字（根据你的需要选择 int 或 double）
    //     if (double.TryParse(xStr, out double x) && double.TryParse(yStr, out double y))
    //     {
    //         return (x, y);
    //     }
    //     else
    //     {
    //         throw new InvalidOperationException();
    //     }
    // }

    public static (double X, double Y, string? Cosmetic, Visibility Visibility) Translate(string s)
    {
        string[] parts = s.Split(',');
        double x = 0, y = 0;
        string? cosmetic = null;
        Visibility visibility = Visibility.Visible; // 默认值

        foreach (var part in parts)
        {
            var kv = part.Split(':');
            if (kv.Length != 2) continue;
            var key = kv[0].Trim();
            var value = kv[1].Trim();

            switch (key)
            {
                case "x":
                    double.TryParse(value, out x);
                    break;
                case "y":
                    double.TryParse(value, out y);
                    break;
                case "Cosmetic":
                    cosmetic = value == "null" ? null : value;
                    break;
                case "Visibility":
                    visibility = value == "v" ? Visibility.Visible : Visibility.Collapsed;
                    break;
            }
        }

        return (x, y, cosmetic, visibility);
    }

    public string Get(int index)
    {
        FrameCollection.TryGetValue(index, out string? value);
        if (value == null) return "";

        return value;
    }
}