using Microsoft.UI.Xaml.Controls;

namespace Fishtrace;


public class ConsoleTextBlock:Element
{
    public TextBlock TextBlock;
    public ConsoleList? ParentList = null;
    public ConsoleTextBlock(TextBlock T,string name)
    {
        Name = name;
        TextBlock = T;
    }

    public void ConsoleOutput(string s)
    {
        TextBlock.Text = s;
    }

    public void ClearConsole()
    {
        TextBlock.Text = "I am a console.";
    }

    public void ChangeFontSize(int f)
    {
        if (!(f > 0))
        {
            return;
        }
        TextBlock.FontSize = f;
    }
}