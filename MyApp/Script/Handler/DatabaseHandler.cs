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
        if (parent?.database == null)
        {
            Console.WriteLine("Database is not initialized");
            return;
        }

        int input = SelectActions();
        if (input < 0 || input >= allfiles.Length)
        {
            Console.WriteLine("Invalid database selection");
            return;
        }

        Console.WriteLine("Selected database: " + allfiles[input]);
        parent.database.CurrentDatabaseName = allfiles[input];
        parent.database.LoadDatabase();
        EDBMode mode = SelectModification();
        parent.database.GetTables();
        Console.WriteLine("Mode: " + mode);

        input = SelectTable();
        if (input < 0 || input >= parent.database.tablesNameList.Count)
        {
            Console.WriteLine("Invalid table selection");
            return;
        }

        Console.WriteLine("Selected table: " + parent.database.tablesNameList[input]);
        parent.database.CurrentTableName = parent.database.tablesNameList[input];

        switch(mode)
        {
            case EDBMode.ADD:
                HandleAddMode();
                break;
            case EDBMode.DELETE:
                HandleDeleteMode();
                break;
            default:
                Console.WriteLine("Entry not recognized");
                break;
        }
    }

    private void HandleAddMode()
    {
        if (parent?.database == null)
            return;

        parent.database.GetColumnNames();
        if (parent.database.columnsNameList.Count <= 1)
        {
            Console.WriteLine("No columns available to add");
            return;
        }

        // Construire la liste des noms de colonnes (sauf ID)
        var columnNames = string.Join(", ", parent.database.columnsNameList.Skip(1));
        Console.WriteLine("Columns: " + columnNames);
        Console.WriteLine("Enter values separated by commas (or -1 to quit):");

        bool isAdding = true;
        while(isAdding)
        {
            string? input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
                continue;

            int inputInt = RacingLibrary.ConvertStringToInt(input);
            if(inputInt == -1)
            {
                isAdding = false;
                continue;
            }

            // Parser les valeurs séparées par des virgules
            string[] values = input.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (values.Length != parent.database.columnsNameList.Count - 1)
            {
                Console.WriteLine($"Expected {parent.database.columnsNameList.Count - 1} values, got {values.Length}");
                continue;
            }

            parent.database.InsertRowInTable(parent.database.CurrentTableName, columnNames, values);
        }
    }

    private void HandleDeleteMode()
    {
        if (parent?.database == null)
            return;

        parent.database.ShowTable(parent.database.CurrentTableName);
        Console.WriteLine("Enter an ID to delete, -1 to quit");
        
        int input = RacingLibrary.GetIntInput();
        while(input != -1)
        {
            parent.database.DeleteRow(parent.database.CurrentTableName, input);
            Console.WriteLine("Enter an ID to delete, -1 to quit");
            input = RacingLibrary.GetIntInput();
        }
    }
    private int SelectActions()
    {
        Console.WriteLine("Choose an action!");
        Console.WriteLine("1 : Modify a database");
        GetDatabases();
        
        if (allfiles == null || allfiles.Length == 0)
        {
            Console.WriteLine("No databases found");
            return -1;
        }

        int input = RacingLibrary.GetIntInput();
        bool goodEntry = RacingLibrary.VerifyInput(input, 0, allfiles.Length, "Invalid database selection");
        while(!goodEntry)
        {
            input = RacingLibrary.GetIntInput();
            goodEntry = RacingLibrary.VerifyInput(input, 0, allfiles.Length, "Invalid database selection");
        }
        return input;
    }

    private EDBMode SelectModification()
    {
        Console.WriteLine("Choose a mode");
        Console.WriteLine("1 : ADD ");
        Console.WriteLine("2 : DELETE");

        EDBMode mode = (EDBMode)RacingLibrary.GetIntInput();
        bool goodEntry = RacingLibrary.VerifyInput((int)mode,0,(int)EDBMode.MAX,"TODO");
        while(!goodEntry)
        {
            mode = (EDBMode)RacingLibrary.GetIntInput();
            goodEntry = RacingLibrary.VerifyInput((int)mode,0,(int)EDBMode.MAX,"TODO");
        }
        return mode;
    }

    private int SelectTable()
    {
        if (parent?.database == null)
        {
            Console.WriteLine("Database is not initialized");
            return -1;
        }

        Console.WriteLine("Choose a table");
        for (int i = 0; i < parent.database.tablesNameList.Count; i++)
            Console.WriteLine(i + " : " + parent.database.tablesNameList[i]);    
        
        int input = RacingLibrary.GetIntInput();
        bool goodEntry = RacingLibrary.VerifyInput(input, 0, parent.database.tablesNameList.Count, "Invalid table selection");
        while(!goodEntry)
        {
            input = RacingLibrary.GetIntInput();
            goodEntry = RacingLibrary.VerifyInput(input, 0, parent.database.tablesNameList.Count, "Invalid table selection");
        }
        return input;
    }
    public void SelectMode()
    {
        Console.WriteLine("Select a mode !");
        Console.WriteLine("0 : Modify !");
        Console.WriteLine("1 : Create !");
        int input = RacingLibrary.GetIntInput();
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
