namespace Fishtrace;


public class ConsoleList
{
    public List<ConsoleTextBlock> Consoles;
    public List<string> User;
    public string Name;
    public ConsoleTextBlock? ConsoleOwned = null;

    public ConsoleList(List<ConsoleTextBlock> l,string name)
    {
        Consoles = l;
        Name = name ;
        User = new List<string>();
        for (int i = 0; i < Consoles.Count(); i++)
        {
            User.Add("");
            Consoles[i].ParentList = this;
        }
    }

    public void ConsoleOutput(string s)
    {
        if(ConsoleOwned == null)
        {
            ConsoleTextBlock? _ = MainPage.Consoles.GetAvaliableConsole(this);
            if (_ != null)
            {
                ConsoleOwned = _;
                ConsoleOutput(s);
            }
            return;
        }

        ConsoleOwned.ConsoleOutput($"{Name} is using this console:" + s);
    }

    public void FreeConsoleOwnership()
    {
        if(ConsoleOwned != null)
        {
            #pragma warning disable CS8602 // Dereference of a possibly null reference.
            ConsoleOwned.ParentList.RemoveUserByConsoleName(ConsoleOwned.Name);
#           pragma warning restore CS8602 // Dereference of a possibly null reference.
        }
    }

    public ConsoleTextBlock? GetAvaliableConsole(Element e)
    {
        for (int i = 0; i < User.Count(); i++)
        {
            if (User[i] == "")
            {
                return Consoles[i];
            }
        }

        Console.WriteLine("No avaliable Consoles");
        return null;
    }

    public ConsoleTextBlock? GetAvaliableConsole(ConsoleList e)
    {
        for (int i = 0; i < User.Count(); i++)
        {
            if (User[i] == "")
            {
                return Consoles[i];
            }
        }

        Console.WriteLine("No avaliable Consoles");
        return null;
    }

    public void RemoveUserByUserName(string s)
    {
        for (int i = 0; i < User.Count(); i++)
        {
            if (User[i] == s)
            {
                User[i] = "";
            }
        }
    }

    public void RemoveUserByConsoleName(string s)
    {
        for (int i = 0; i < Consoles.Count(); i++)
        {
            if (Consoles[i].Name == s)
            {
                User[i] = "";
            }
        }
    }

    public void FreeAllConsoles()
    {
        for (int i = 0; i < Consoles.Count(); i++)
        {
            User[i] = "";
        }
    }

    public ConsoleTextBlock this[int index]
    {
        get
        {
            if (index < 0 || index >= Consoles.Count)
                throw new IndexOutOfRangeException($"Index {index} out of bound (0-{Consoles.Count - 1})");
            return Consoles[index];
        }
        set
        {
            if (index < 0 || index >= Consoles.Count)
                throw new IndexOutOfRangeException($"Index {index} out of bound (0-{Consoles.Count - 1})");
            Consoles[index] = value;
        }
    }
}