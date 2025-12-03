public class DatabaseHandler
{
    private enum EDBActions
    {
        MODIFY = 0,
        CREATE
    };
    private enum EDBMode
    {
        ADD = 0,
        DELETE,
        MAX
    };

    string[] allfiles;
    GameManager parent;

    public DatabaseHandler(GameManager? _parent) => parent = _parent;  

#region MODIFDB
    private void ModifyDatabase()
    {
        int input = SelectActions();
        Console.WriteLine("INput :" + input);
        Console.WriteLine("INput :" + allfiles[input]);
        parent.database.CurrentDatabaseName = allfiles[input];
        parent.database.LoadDatabase();
        EDBMode mode = SelectModification();
        parent.database.GetTables();
        Console.WriteLine("Mode  :" + mode);

        input = SelectTable();
        Console.WriteLine("INput :" + input);
        Console.WriteLine("INput :" + parent.database.tablesNameList[input]);
        parent.database.CurrentTableName = parent.database.tablesNameList[input];

        switch(mode)
        {
            case EDBMode.ADD:
                parent.database.GetColumnNames();
                var entry = "(";
                for(int i = 1;i< parent.database.columnsNameList.Count; i++)
                {
                    entry += parent.database.columnsNameList[i];
                    if(i != parent.database.columnsNameList.Count-1)
                        entry += ",";
                }
                    
                entry += ")";
                Console.WriteLine("Column :" + entry);
                bool isAdding = true;
                while(isAdding)
                {
                    string? minput = Console.ReadLine();
                    int minputInt = 0;
                    Console.WriteLine("Input : " + minput );
                    try
                    { 
                        minputInt = MyAppLibrary.ConvertStringToInt(minput);   
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    if(minputInt == -1)
                        isAdding = false;
                    else
                        parent.database.InsertRowInTable(parent.database.CurrentTableName, entry,minput);
                }
            break;
            case EDBMode.DELETE:
                parent.database.ShowTable(parent.database.CurrentTableName);
                Console.WriteLine("Enter an ID to delete, -1 to quit");
                try
                {
                    input =  MyAppLibrary.GetIntInput();
                    while(input != -1)
                    {
                        parent.database.DeleteRow(parent.database.CurrentTableName,input.ToString());
                        Console.WriteLine("Enter an ID to delete, -1 to quit");
                        input = MyAppLibrary.GetIntInput();
                    }
                    
                    
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                
                
            break;
            default:
                Console.WriteLine("Entry not recognize");
            break;

        }

    }
    private int SelectActions()
    {
        Console.WriteLine("Choose an actions ! ");
        Console.WriteLine("1 : Modify a database");
        //Console.WriteLine("2 : Create a new database");
        GetDatabases();
        int input = MyAppLibrary.GetIntInput();
        bool goodEntry = MyAppLibrary.VerifyInput(input,0,allfiles.Length,"TODO");
        while(!goodEntry)
        {
            input = MyAppLibrary.GetIntInput();
            goodEntry = MyAppLibrary.VerifyInput(input,0,allfiles.Length,"TODO");
        }
        return input;
    }

    private EDBMode SelectModification()
    {
        Console.WriteLine("Choose a mode");
        Console.WriteLine("1 : ADD ");
        Console.WriteLine("2 : DELETE");

        EDBMode mode = (EDBMode)MyAppLibrary.GetIntInput();
        bool goodEntry = MyAppLibrary.VerifyInput((int)mode,0,(int)EDBMode.MAX,"TODO");
        while(!goodEntry)
        {
            mode = (EDBMode)MyAppLibrary.GetIntInput();
            goodEntry = MyAppLibrary.VerifyInput((int)mode,0,(int)EDBMode.MAX,"TODO");
        }
        return mode;
    }

    private int SelectTable()
    {
        Console.WriteLine("Choose a table");
        for (int i = 0; i < parent.database.tablesNameList.Count; i++)
            Console.WriteLine(i + " : " + parent.database.tablesNameList[i]);    
        
        int input = MyAppLibrary.GetIntInput();
        bool goodEntry = MyAppLibrary.VerifyInput(input,0,parent.database.tablesNameList.Count,"TODO");
        while(!goodEntry)
        {
            input = MyAppLibrary.GetIntInput();
            goodEntry = MyAppLibrary.VerifyInput(input,0,parent.database.tablesNameList.Count,"TODO");
        }
        return input;
    }
    public void SelectMode()
    {
        Console.WriteLine("Select a mode !");
        Console.WriteLine("0 : Modify !");
        Console.WriteLine("1 : Create !");
        int input = MyAppLibrary.GetIntInput();
        switch(input)
        {
            case (int)EDBActions.MODIFY:
                Console.WriteLine("Go in modufy !");
                ModifyDatabase();
            break;
            case (int)EDBActions.CREATE:
                Console.WriteLine("Go increat !");
                CreateDatabase();
            break;
            default:
                Console.WriteLine("Default !");
            break;
        }
    }
    private void CreateDatabase()
    {
        
    }
    private void GetDatabases()
    {
        Console.WriteLine("Choose a db");
        allfiles = Directory.GetFiles("Database", "*.sqlite", SearchOption.AllDirectories);
        for(int i= 0;i<allfiles.Length;i++)
            Console.WriteLine(i+ " : " + allfiles[i]);
    }
    private void AddLine()
    {
        
    }
    private void RemoveLine()
    {
        
    }
#endregion
    public void InsertData()
    {
        
    }
    public void DeleteData()
    {
        
    }
}
