using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.System;

namespace Fishtrace;


public class AnimationSlot : FunctionElement
{
    public Animation? Film;
    //The character editing page
    public CharacterSlotBar? CharacterBar;
    public CommonElement NameBlock;
    private TextBox? _nameTextBox;
    public string InputCache;
    public bool _isInputtingName = false;
    public bool _isInPlayList = false;
    public DraggableCharacter? Character
    {
        get
        {
            if (CharacterBar == null) return null;
            if (CharacterBar.DraggableCharacter == null) return null;
            return CharacterBar.DraggableCharacter;
        }
    }

    private TaskCompletionSource<bool>? _inputTcs;
    public AnimationSlot(FrameworkElement f, string name) : base(f, name)
    {
        AddCosmetic("Film", "Assets/80x80Film.png");
        AddAction(OnAddFilmButtonClick);
        AddFunctionBox(Name + "AddToPlayList");
        FunctionBox.AddFunction(AddOrRemoveToPlayList, "AddToPlayList");
        FunctionBox.AddFunction(RelativeRecordingSetting, "Relative Recording:True");
        FunctionBox.AddFunction(ClickToHideOrShow, "Hide/Show Character");
        FunctionBox.AddFunction(ClickToLoop, "ClickToActivateLoop");
        FunctionBox.AddFunction(ClickToResetCharacterPosition,"ResetCharacterPosition");

        MainPage.LatestAnimationSlot = this;
        EnableSpringFeedback();
        EnableHoverGlow();
    }

    public void AddOrRemoveToPlayList()
    {
        if (MainPage.isPlayingAnimation) return;
        if (!_isInPlayList)
        {
            if (Film == null) return;
            if (CharacterBar == null) return;
            if (CharacterBar.DraggableCharacter == null) return;
            MainPage.SelectedAnimation.Add(Film);
            MainPage.SelectedAnimationCharacter.Add(CharacterBar.DraggableCharacter);
            ((TextBlock)FunctionBox.FunctionList[0].Description.UIElement).Text = "RemoveFromPlayList";
            _isInPlayList = true;
        }
        else
        {
            if (CharacterBar == null) return;
            if (CharacterBar.DraggableCharacter == null) return;
            MainPage.SelectedAnimation.Remove(Film);
            MainPage.SelectedAnimationCharacter.Remove(CharacterBar.DraggableCharacter);
            CharacterBar.DraggableCharacter.ResetAnimationPlaying();
            ((TextBlock)FunctionBox.FunctionList[0].Description.UIElement).Text = "AddToPlayList";
            _isInPlayList = false;
        }
    }

    public void ClickToHideOrShow()
    {
        if (Character == null) return;
        if (Character._isHiding)
        {
            Character.Show();
            Character._isHiding = false;
            ((TextBlock)Character.FunctionBox.FunctionList[2].Description.UIElement).Text = "ClickMeToHide";
        }
        else
        {
            Character.Hide();
            Character._isHiding = true;
            ((TextBlock)Character.FunctionBox.FunctionList[2].Description.UIElement).Text = "ClickMeToShow";
        }
    }

    public void RelativeRecordingSetting()
    {
        if (Film == null) return;
        if (Film.Relative)
        {
            Film.Relative = false;
            ((TextBlock)FunctionBox.FunctionList[1].Description.UIElement).Text = "Relative Recording:False";
        }
        else
        {
            Film.Relative = true;
            ((TextBlock)FunctionBox.FunctionList[1].Description.UIElement).Text = "Relative Recording:True";
        }
    }

    public void ClickToLoop()
    {
        if (MainPage.isPlayingAnimation) return;
        if (Character == null) return;
        if (Character._isLoop)
        {
            Character._isLoop = false;
            ((TextBlock)FunctionBox.FunctionList[3].Description.UIElement).Text = "ClickToActivateLoop";
        }
        else
        {
            Character._isLoop = true;
            ((TextBlock)FunctionBox.FunctionList[3].Description.UIElement).Text = "ClickToCancelLoop";
        }
    }

    public void ClickToResetCharacterPosition()
    {
        if (Character == null) return;
        Character.ResetPosition();
    }

    public void AddFilm(Animation film)
    {
        Film = film;
    }

    public static AnimationSlot CreateAnimationSlot(Canvas canvas, string name)
    {
        Image I = Utils.CreateImage("Assets/80x80AddButton.png");
        canvas.Children.Add(I);
        I.Width = 80;
        I.Height = 80;
        return new AnimationSlot(I, name);
    }

    // public async void OnAddFilmButtonClick()
    // {
    //     if (!(Parent is ElementBar)) { return; }

    //     var eb = (ElementBar)Parent;
    //     if (CharacterBar == null)
    //     {
    //         ChangeCosmetic("Film");
    //         Utils.NewFilm(this);

    //         TextBlock nb = Utils.CreateTextBlock("Enter Name");
    //         nb.Height = 20;
    //         nb.Width = 80;
    //         nb.TextWrapping = TextWrapping.Wrap;
    //         Canvas.Children.Add(nb);
    //         Canvas.SetZIndex(nb, 22);
    //         NameBlock = new CommonElement(nb, Name + "_NameBlock");
    //         NameBlock.SetX(GetX());
    //         NameBlock.SetY(GetY() + 80);
    //         eb.AddFunction(NameBlock, false);

    //         CanvasInputStream.Canvas.Focus(FocusState.Programmatic);
    //         _inputTcs = new TaskCompletionSource<bool>();
    //         CanvasInputStream.ConnectStream(InputAnimationName);
    //         CanvasInputStream.CanvasStartFocus();
    //         await _inputTcs.Task;

    //         if (!(Parent is ElementBar)) { throw new InvalidOperationException(); }
    //         eb.AddFunction(CreateAnimationSlot(Canvas, Name + "NextSlot"));
    //         CharacterBar = Utils.CreateCharacterBar(Name + "CharacterBar", Canvas);
    //         CharacterBar.Hide();
    //         MainPage.CurrentCharacterBar = CharacterBar;
    //     }
    //     else
    //     {
    //         Parent.Hide();
    //         CharacterBar.Show();
    //         MainPage.CurrentRightBarStatus = ElementBar.Status_CharacterBar;
    //     }
    // }

    // public void InputAnimationName(object sender, KeyRoutedEventArgs e)
    // {
    //     MainPage.Consoles.ConsoleOutput("sdsdad");

    //     if (e.Key == VirtualKey.Back)
    //     {
    //         if (InputCache.Length > 0)
    //         {
    //             InputCache = InputCache.Remove(InputCache.Length - 1);
    //             ((TextBlock)NameBlock.UIElement).Text = InputCache;
    //         }
    //         e.Handled = true;
    //         return;
    //     }

    //     // 回车（提交）
    //     if (e.Key == VirtualKey.Enter)
    //     {
    //         if (_inputTcs == null) { throw new NullReferenceException(); }
    //         e.Handled = true;
    //         _inputTcs.TrySetResult(true);
    //         CanvasInputStream.DisconnectStream(InputAnimationName);
    //         return;
    //     }

    //     // 空格
    //     if (e.Key == VirtualKey.Space)
    //     {
    //         InputCache += ' ';
    //         ((TextBlock)NameBlock.UIElement).Text = InputCache;
    //         e.Handled = true;
    //         return;
    //     }

    //     // 字母、数字、符号：需要检查 Shift 状态
    //     char? c = CanvasInputStream.ConvertVirtualKeyToChar(e.Key);
    //     if (c.HasValue)
    //     {
    //         MainPage.Consoles.ConsoleOutput("Converting");
    //         InputCache += c.Value;
    //         ((TextBlock)NameBlock.UIElement).Text = InputCache;
    //         e.Handled = true;
    //     }
    // }

    public async void OnAddFilmButtonClick()
    {
        try
        {
            if (!(Parent is ElementBar) || _isInputtingName) { return; }

            var eb = (ElementBar)Parent;
            if (CharacterBar == null)
            {
                _isInputtingName = false;
                ChangeCosmetic("Film");
                Utils.NewFilm(this);

                // 创建 TextBox
                // _nameTextBox = new TextBox
                // {
                //     Text = "Enter Name",
                //     Width = 80,
                //     Height = 20,
                //     Background = new SolidColorBrush(Colors.White),
                //     Foreground = new SolidColorBrush(Colors.Black),
                //     FontSize = 12,
                //     TextWrapping = TextWrapping.NoWrap,
                //     VerticalAlignment = VerticalAlignment.Top
                // };
                // Canvas.Children.Add(_nameTextBox);
                // Canvas.SetZIndex(_nameTextBox, 22);

                // // 包装为 CommonElement（用于统一管理位置）
                // NameBlock = new CommonElement(_nameTextBox, Name + "_NameBlock");
                // NameBlock.SetX(GetX());
                // NameBlock.SetY(GetY() + 80);
                // eb.AddFunction(NameBlock, false, false);

                // // 订阅回车事件
                // _inputTcs = new TaskCompletionSource<bool>();
                // _nameTextBox.KeyDown += OnNameTextBoxKeyDown;

                // // 聚焦到文本框
                // _nameTextBox.Focus(FocusState.Programmatic);

                // // 等待用户按回车提交
                // //await _inputTcs.Task;

                // // 获取输入文本
                // InputCache = _nameTextBox.Text;

                // // 清理
                // _nameTextBox.KeyDown -= OnNameTextBoxKeyDown;
                // Canvas.Children.Remove(_nameTextBox);
                // eb.FunctionList.Remove(NameBlock); // 确保 ElementBar 有 RemoveFunction 方法

                InputCache = "Film" + MainPage.Film.Count().ToString();
                // 继续后续逻辑
                if (!(Parent is ElementBar)) { throw new InvalidOperationException(); }
                eb.AddFunction(CreateAnimationSlot(Canvas, Name + "NextSlot"), true, true);
                CharacterBar = CharacterSlotBar.CreateCharacterBar(Name + "_CharacterBar", Canvas);
                CharacterBar.Hide();
                MainPage.CurrentCharacterBar = CharacterBar;
                MainPage.RightElementBarManager.AddElementbar(CharacterBar);
                //if (Linker == null) Linker = new Linker(this);

                TextBlock nb = Utils.CreateTextBlock(InputCache);
                nb.Height = 20;
                nb.Width = 80;
                nb.TextWrapping = TextWrapping.Wrap;
                Canvas.Children.Add(nb);
                Canvas.SetZIndex(nb, 22);
                NameBlock = new CommonElement(nb, Name + "_NameBlock");
                NameBlock.SetX(GetX());
                NameBlock.SetY(GetY() + 80);
                eb.AddFunction(NameBlock, false, false);
            }
            else
            {
                Parent.Hide();
                CharacterBar.Show();
                MainPage.CurrentRightBarStatus = ElementBar.Status_CharacterBar;
                MainPage.CurrentCharacterBar = CharacterBar;
                MainPage.AnimationRecording = Film;
                MainPage.MainCharacter = CharacterBar.DraggableCharacter;
                MainPage.Consoles.ConsoleOutput(MainPage.AnimationRecording.Name);
            }
        }
        catch (Exception ex)
        {
            MainPage.Consoles.ConsoleOutput(ex.Message);
        }
    }

    public void AddAnimation(string name, Animation? animation, CharacterSlotBar characterbar)
    {
        if (CharacterBar != null || _isInputtingName) return;
        if (!(Parent is ElementBar)) { throw new InvalidOperationException(); }

        ChangeCosmetic("Film");
        Utils.NewFilm(this);
        if (animation != null) { Film = animation; }
        var eb = (ElementBar)Parent;
        eb.AddFunction(CreateAnimationSlot(Canvas, Name + "NextSlot"), true, true);
        CharacterBar = Utils.CloneCharacterBar(characterbar);
        CharacterBar.Hide();

        TextBlock nb = Utils.CreateTextBlock(name);
        nb.Height = 20;
        nb.Width = 80;
        nb.TextWrapping = TextWrapping.Wrap;
        Canvas.Children.Add(nb);
        Canvas.SetZIndex(nb, 22);
        NameBlock = new CommonElement(nb, Name + "_NameBlock");
        NameBlock.SetX(GetX());
        NameBlock.SetY(GetY() + 80);
        eb.AddFunction(NameBlock, false, false);
    }

    private void OnNameTextBoxKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Enter)
        {
            _inputTcs?.TrySetResult(true);
            e.Handled = true;
            _isInputtingName = false;
        }
    }
}