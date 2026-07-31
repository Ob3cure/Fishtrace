using System.Reflection.Emit;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Foundation.Collections;
using Windows.Gaming.XboxLive;
using Windows.Storage;
using Windows.Storage.Streams;


namespace Fishtrace;


public class Utils
{
    public Utils()
    {

    }

    public class DraggableElement : Element
    {
        private bool _isdragging = false;
        private double offsetX = 0;
        private double offsetY = 0;
        private Canvas? canvas;

        private Element element;

        public DraggableElement(Element e)
        {
            element = e;
            canvas = e.UIElement.Parent as Canvas;
            if (canvas == null) { throw new NullReferenceException(); }
            e.AddPressedEvent(DragPressed);
            e.AddReleasedEvent(DragReleased);
            e.AddHoldingEvent(DragHolding);
            e.AddCaptureLossEvent(DragCaptureLoss);
        }

        public DraggableElement(FrameworkElement u)
        {
            UIElement = u;
            Name = u.Name;
            element = this;
            canvas = u.Parent as Canvas;
            if (canvas == null) { throw new NullReferenceException(); }
            AddPressedEvent(DragPressed);
            AddReleasedEvent(DragReleased);
            AddHoldingEvent(DragHolding);
            AddCaptureLossEvent(DragCaptureLoss);
        }

        public void DragPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.GetCurrentPoint(canvas).Properties.IsLeftButtonPressed == false) { return; }

            _isdragging = true;

            double _x = e.GetCurrentPoint(canvas).Position.X;
            double _y = e.GetCurrentPoint(canvas).Position.Y;

            offsetX = element.GetX() - _x;
            offsetY = element.GetY() - _y;

            element.UIElement.CapturePointer(e.Pointer);
        }

        public void DragHolding(object sender, PointerRoutedEventArgs e)
        {
            if (!_isdragging) { return; }

            double _x = e.GetCurrentPoint(canvas).Position.X;
            double _y = e.GetCurrentPoint(canvas).Position.Y;

            element.SetX(_x + offsetX);
            element.SetY(_y + offsetY);

            MainPage.Consoles.ConsoleOutput("Position:" + $"({Math.Round(element.GetX())}," + $"{Math.Round(element.GetY())})");
        }

        public void DragReleased(object sender, PointerRoutedEventArgs e)
        {
            if (_isdragging == true)
            {
                _isdragging = false;
                element.UIElement.ReleasePointerCapture(e.Pointer);
            }
        }

        public void DragCaptureLoss(object sender, PointerRoutedEventArgs e)
        {
            _isdragging = false;
        }
    }

    public static BitmapImage ReadImageSourceUsingPath(string path)
    {
        BitmapImage bitmap = new BitmapImage();
        bitmap.UriSource = new Uri("ms-appx:///" + path); // 使用绝对 URI

        return bitmap;
    }

    public static async Task<WriteableBitmap> LoadWriteableBitmapFromPathAsync(string path)
    {
        try
        {
            StorageFile file;

            // 1. 判断是 "ms-appx:///" 资源还是本地绝对路径
            if (path.StartsWith("Assets/"))
            {

                file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///" + path));
            }
            else
            {
                file = await StorageFile.GetFileFromPathAsync(path); // 视为绝对路径
            }

            // 2. 打开只读流
            using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
            {
                // 3. 先随便填个(1,1)，SetSourceAsync 会自动覆盖尺寸
                WriteableBitmap wb = new WriteableBitmap(1, 1);
                await wb.SetSourceAsync(stream);
                return wb;
            }
        }
        catch (Exception ex)
        {
            // 处理文件不存在等异常
            return null;
        }
    }

    public static Image CreateGrayBlock()
    {
        return CreateImage("Assets/200x200GrayBlock.png");
    }

    public static Image CreateImage(string path)
    {
        Image _I = new Image();
        _I.Source = ReadImageSourceUsingPath(path);
        return _I;
    }

    public static TextBlock CreateTextBlock(string text)
    {
        TextBlock _T = new TextBlock();
        _T.Text = text;

        return _T;
    }

    public static HintBox CreateHintBox(Canvas canvas, string text, string? name, Element creator)
    {
        Image _I = CreateGrayBlock();
        TextBlock _T = CreateTextBlock(text);
        _T.Width = 200;
        _T.Height = 200;

        canvas.Children.Add(_I);
        canvas.Children.Add(_T);

        _T.TextWrapping = TextWrapping.Wrap;
        HintBox? _H;
        if (name != null)
        {
            _H = new HintBox(new CommonElement(_I, "Backgroud_" + name), new CommonElement(_T, "Hint_" + name), name, creator);
        }
        else
        {
            _H = new HintBox(new CommonElement(_I), new CommonElement(_T), "HintBox_" + text, creator);
        }

        return _H;
    }

    public static InputBox CreateInputBox(Canvas canvas, string info, string? name, Element creator)
    {
        Image _I = CreateGrayBlock();
        TextBlock _T1 = CreateTextBlock(info);
        TextBlock _T2 = CreateTextBlock("Input:");
        _T1.Width = 200;
        _T1.Height = 200;
        _T2.Width = 200;
        _T2.Height = 200;

        canvas.Children.Add(_I);
        canvas.Children.Add(_T1);
        canvas.Children.Add(_T2);

        _T1.TextWrapping = TextWrapping.Wrap;
        _T1.TextWrapping = TextWrapping.Wrap;
        InputBox? _IN;
        if (name != null)
        {
            _IN = new InputBox(new CommonElement(_I, "Backgroud_" + name), new CommonElement(_T1, "InfoText_" + name), new CommonElement(_T2, "Display_" + name), name, creator);
        }
        else
        {
            _IN = new InputBox(new CommonElement(_I), new CommonElement(_T1), new CommonElement(_T2), "InputBox_" + info, creator);
        }

        return _IN;
    }

    public static ElementBar CreateElmenetBar(string name, Canvas canvas, bool horizontal, int? index = null)
    {
        if (horizontal)
        {
            Image I = CreateImage("Assets/740x100WhiteBar.png");
            canvas.Children.Add(I);
            ElementBar eb = new ElementBar(I, name, index);
            eb.SetHorizontal();
            return eb;
        }
        else
        {
            Image I = CreateImage("Assets/100x520WhiteBar.png");
            canvas.Children.Add(I);
            ElementBar eb = new ElementBar(I, name, index);
            eb.SetVertical();
            return eb;
        }
    }

    public static CharacterSlotBar CreateCharacterBar(string name, Canvas canvas, bool horizontal = false)
    {
        CharacterSlotBar eb = CharacterSlotBar.CreateCharacterBar(name, canvas, horizontal);
        eb.AddFunction(CharacterSlot.CreateCharacterSlot(canvas, name));

        return eb;
    }

    public int GetCanvasMaxZ(Canvas canvas)
    {
        int maxZ = -1;
        foreach (UIElement child in canvas.Children)
        {
            int z = Canvas.GetZIndex(child);
            if (z > maxZ) maxZ = z;
        }

        return maxZ;
    }

    public static List<double> CalculateBoxOffset(double x, double y)
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

    public static void NewFilm(AnimationSlot ab)
    {
        Animation film = new Animation(MainPage.TickRate, "Film" + (MainPage.Film.Count() + 1));
        MainPage.Film.Add("Film" + (MainPage.Film.Count() + 1).ToString(),film);
        ab.AddFilm(film);
    }

    public static CharacterSlotBar CloneCharacterBar(CharacterSlotBar characterbar)
    {
        CharacterSlotBar r = CreateCharacterBar(characterbar.Name + "_Clone", characterbar.Canvas);
        if (characterbar.DraggableCharacter == null) return r;
        foreach (var kvp in characterbar.DraggableCharacter.Cosmetics)
        {
            r.LatestAnimationSlot.AddCosmeticToCharacter(kvp.Key, kvp.Value);
        }

        return r;
    }

    public class TwoWayDictionary<TLeft, TRight>
    {
        private Dictionary<TLeft, TRight> _leftToRight = new Dictionary<TLeft, TRight>();
        private Dictionary<TRight, TLeft> _rightToLeft = new Dictionary<TRight, TLeft>();

        public void Add(TLeft left, TRight right)
        {
            // 可选的唯一性检查
            if (_leftToRight.ContainsKey(left) || _rightToLeft.ContainsKey(right))
                throw new InvalidOperationException("Duplicate key or value.");

            _leftToRight.Add(left, right);
            _rightToLeft.Add(right, left);
        }

        // 正向查找 (Left -> Right)
        public bool TryGetRight(TLeft left, out TRight right)
            => _leftToRight.TryGetValue(left, out right);

        // 反向查找 (Right -> Left)
        public bool TryGetLeft(TRight right, out TLeft left)
            => _rightToLeft.TryGetValue(right, out left);
    }
}