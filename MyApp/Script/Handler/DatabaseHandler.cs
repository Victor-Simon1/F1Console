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
        RETURN,
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
            RacingLogger.Error("Database is not initialized");
            return;
        }

        int input = SelectActions();
        if (input < 0 || input >= allfiles.Length)
        {
            RacingLogger.Error("Invalid database selection");
            return;
        }

        RacingLogger.Info("Selected database: " + allfiles[input]);
        parent.database.CurrentDatabaseName = allfiles[input];
        parent.database.LoadDatabase();
   
        parent.database.GetTables();
       

        input = SelectTable();
        if (input < 0 || input >= parent.database.tablesNameList.Count)
        {
            RacingLogger.Error("Invalid table selection");
            return;
        }

        RacingLogger.Info("Selected table: " + parent.database.tablesNameList[input]);
        parent.database.CurrentTableName = parent.database.tablesNameList[input];
        EDBMode mode;
        do
        {
            mode = SelectModification();
            RacingLogger.Info("Mode: " + mode);
            switch(mode)
            {
            case EDBMode.ADD:
                HandleAddMode();
                break;
            case EDBMode.DELETE:
                HandleDeleteMode();
                break;
            case EDBMode.RETURN:
                RacingLogger.Info("Return to the menu");
                break;
            default:
                RacingLogger.Error("Entry not recognized");
                break;
            }
        }while(mode != EDBMode.RETURN);
   
    }

    private void HandleAddMode()
    {
        if (parent?.database == null)
            return;

        parent.database.GetColumnNames();
        if (parent.database.columnsNameList.Count <= 1)
        {
            RacingLogger.Error("No columns available to add");
            return;
        }

        // Construire la liste des noms de colonnes (sauf ID)
        string[] columnNames =parent.database.columnsNameList.Skip(1).ToArray();
        var columnNamesStr = string.Join(", ", parent.database.columnsNameList.Skip(1));
        //RacingLogger.Info("Columns: " + columnNames);
        //RacingLogger.Info("Enter values separated by commas (or -1 to quit):");

        bool isAdding = true;
        string[] values = new string[columnNames.Length];
        while(isAdding)
        {
            
            RacingLogger.Debug("While +" +isAdding);
            int inputInt; //= RacingLibrary.ConvertStringToInt(input);
            for(int i = 0 ; i<columnNames.Length;i++)
            {
                RacingLogger.Info("Rentrez la valeur pour : " + columnNames[i]);
                string? input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                    continue;
                inputInt = RacingLibrary.ConvertStringToInt(input);
                
                if(inputInt == -1)
                {
                    RacingLogger.Info("Vous sortez de la saisie");
                    isAdding = false;
                    return;
                }

                values[i] = input;
            }
                
           RacingLogger.Debug("While +" +isAdding);

            // Parser les valeurs séparées par des virgules
            //string[] values = input.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (values.Length != parent.database.columnsNameList.Count - 1)
            {
                RacingLogger.Error($"Expected {parent.database.columnsNameList.Count - 1} values, got {values.Length}");
                continue;
            }
            RacingLogger.Debug(values.Length.ToString());
            parent.database.InsertRowInTable(parent.database.CurrentTableName, columnNamesStr, values);
        }
    }

    private void HandleDeleteMode()
    {
        if (parent?.database == null)
            return;

        parent.database.ShowTable(parent.database.CurrentTableName);

        int input;
        do
        {
            RacingLogger.Info("Enter an ID to delete, -1 to quit");
            input = RacingLibrary.GetIntInput();
           
            if(input != -1)
            {
                bool rowHasBeenDeleted = parent.database.DeleteRow(parent.database.CurrentTableName, input);
                if(rowHasBeenDeleted)
                    RacingLogger.Info($"Row of id={input} has been deleted");
                else
                    RacingLogger.Warning($"Row of id={input} does not exist");
            }
           
        }while(input != -1);
        RacingLogger.Info($"You have return to the menu");
    }
    private int SelectActions()
    {
        //RacingLogger.Info("Choose an action!");
        //RacingLogger.Info("1 : Modify a database");
        GetDatabases();
        
        if (allfiles == null || allfiles.Length == 0)
        {
            RacingLogger.Error("No databases found");
            return -1;
        }

        int input = RacingLibrary.GetValidatedIntInput(0, allfiles.Length - 1, "Invalid database selection. Please enter a number between 0 and " + (allfiles.Length - 1));
        return input;
    }

    private EDBMode SelectModification()
    {
        RacingLogger.Info("Choose a mode");
        for(int i = 0; i<(int)EDBMode.MAX;i++)
            RacingLogger.Info(i + " : " + (EDBMode)i);


        int input = RacingLibrary.GetValidatedIntInput(0, (int)EDBMode.MAX - 1, "Invalid mode selection. Please enter 0 for ADD or 1 for DELETE");
        return (EDBMode)input;
    }

    private int SelectTable()
    {
        if (parent?.database == null)
        {
            RacingLogger.Error("Database is not initialized");
            return -1;
        }

        RacingLogger.Info("Choose a table");
        for (int i = 0; i < parent.database.tablesNameList.Count; i++)
            RacingLogger.Info(i + " : " + parent.database.tablesNameList[i]);    
        
        int input = RacingLibrary.GetValidatedIntInput(0, parent.database.tablesNameList.Count - 1, "Invalid table selection. Please enter a number between 0 and " + (parent.database.tablesNameList.Count - 1));
        return input;
    }
    public void SelectMode()
    {
        RacingLogger.Info("Select a mode !");
        RacingLogger.Info("0 : Modify !");
        RacingLogger.Info("1 : Create !");
        int input = RacingLibrary.GetIntInput();
        switch(input)
        {
            case (int)EDBActions.MODIFY:
                RacingLogger.Info("Go in modufy !");
                ModifyDatabase();
            break;
            case (int)EDBActions.CREATE:
                RacingLogger.Info("Go increat !");
                CreateDatabase();
            break;
            default:
                RacingLogger.Error("Default !");
            break;
        }
    }
    private void CreateDatabase()
    {
        RacingLogger.Info("Enter the name of the new database!");
        
        string dbName = Console.ReadLine();
        //dbName +=".sqlite";

        string destinationDB = RacingLibrary.DBFILE + "\\" + dbName+".sqlite";
        //string destinationDB = directory + "/db.sqlite";
        string sourceFile = RacingLibrary.DBFILE + "\\" + "db.sqlite";

        /*allfiles = Directory.GetFiles("Database", "db.sqlite", SearchOption.AllDirectories);*/
        if(!File.Exists(sourceFile))
        {
            RacingLogger.Warning("The default database is not here! Please verify if you have not suppress it !");
            return;
        }
        File.Copy(sourceFile,destinationDB);
        
    }
    private void GetDatabases()
    {
        RacingLogger.Info("Choose a db");
        allfiles = Directory.GetFiles("Database", "*.sqlite", SearchOption.AllDirectories);
        for(int i= 0;i<allfiles.Length;i++)
            RacingLogger.Info(i+ " : " + allfiles[i]);
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
