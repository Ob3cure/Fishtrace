using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage.Pickers;
using WinRT.Interop;

namespace Fishtrace;


public class CharacterSlot : FunctionElement
{
    public string? Cosmetic;
    public DraggableCharacter? DraggableCharacter;
    public CharacterSlot(FrameworkElement f, string name, DraggableCharacter? draggableCharacter) : base(f, name)
    {
        DraggableCharacter = draggableCharacter;
        AddAction(OnCharacterButtonClick);

        EnableHoverGlow();
        EnableSpringFeedback();
    }

    public void AddCosmeticToCharacter(string name, string path)
    {
        if (DraggableCharacter == null)
        {
            MainPage.Consoles.ConsoleOutput("create new character");
            Image I = Utils.CreateImage(path);
            //You may change the size afterward
            if (I.Height > 200 || I.Width > 200)
            {

            }
            I.Height = 200;
            I.Width = 200;
            Canvas.Children.Add(I);
            DraggableCharacter = new DraggableCharacter(I, Name + "_DraggableCharacter");
            DraggableCharacter.SetX(960 / 2);
            DraggableCharacter.SetY(540 / 2);
            MainPage.MainCharacter = DraggableCharacter;
            ((CharacterSlotBar)Parent).DraggableCharacter = DraggableCharacter;
        }
        AddCosmetic(name, path);
        DraggableCharacter.AddCosmetic(name, path);
        DraggableCharacter.ChangeCosmetic(name);
        Cosmetic = name;
        ChangeCosmetic(name);
        UIElement.Width = 80;
        UIElement.Height = 80;

        if (!(Parent is ElementBar)) { throw new InvalidOperationException(); }
        var eb = (ElementBar)Parent;
        eb.AddFunction(CreateCharacterSlot(Canvas, name + "NextSlot", DraggableCharacter));
    }

    public void ChangeCharacterCosmetic()
    {
        if (Cosmetic == null) return;
        if (DraggableCharacter == null) return;
        DraggableCharacter.ChangeCosmetic(Cosmetic);
    }

    public static CharacterSlot CreateCharacterSlot(Canvas canvas, string name, DraggableCharacter? draggableCharacter = null)
    {
        Image I = Utils.CreateImage("Assets/80x80AddButton.png");
        canvas.Children.Add(I);
        I.Width = 80;
        I.Height = 80;
        return new CharacterSlot(I, name, draggableCharacter);
    }

    public async void OnCharacterButtonClick(FunctionElement functionElement)
    {
        if (!(functionElement.Parent is ElementBar)) { return; }
        if (!(functionElement is CharacterSlot)) { return; }

        var cs = (CharacterSlot)functionElement;
        if (cs.Cosmetic == null)
        {
            // 创建一个 FileOpenPicker 实例
            var windowId = UIElement.XamlRoot.ContentIslandEnvironment.AppWindowId;
            var picker = new FileOpenPicker(windowId);

            // 限制用户只能选择 PNG 图片
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            // 如果你还允许其他格式，继续添加：
            // picker.FileTypeFilter.Add(".jpg");
            // picker.FileTypeFilter.Add(".jpeg");

            picker.ViewMode = PickerViewMode.Thumbnail; // 缩略图视图
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary; // 从图片库开始

            // 打开选择器，等待用户选择
            var result = await picker.PickSingleFileAsync();

            if (result != null)
            {
                cs.AddCosmeticToCharacter(cs.Name + "_Cosmetic", result.Path);
            }
            else
            {
                // 用户取消了选择
                MainPage.Consoles.ConsoleOutput("用户取消了选择");
            }
        }
        else
        {
            cs.ChangeCharacterCosmetic();
        }

    }
}