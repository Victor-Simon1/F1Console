
using Microsoft.Data.Sqlite;

public class GameManager
{
    private enum EGMMenu
    {
        RACE = 0,
        CAREER,
        PRINCIPAL
    };
    private enum EGMActions
    {
        RACE = 0,
        CAREER,
        DATABASE,
        EXIT
    };
    private enum EGMCarrerActions
    {
        NEW = 0,
        LOAD,
        RETURN
    };

    public GameManager? instance;
    public List<Driver> driversList = new List<Driver>();
    public List<Team> teamsList = new List<Team>();
    public List<Motor> motorList = new List<Motor>();
    public List<Chassis> chassisList = new List<Chassis>();
    //Tyre
    public List<TireBrand> tireList = new List<TireBrand>();
    public List<TireType> tireTypeList = new List<TireType>();
    //RaceList
    public List<Race> raceList = new List<Race>();
    public List<RaceType> raceTypeList = new List<RaceType>();
    public Database? database;

    public GameManager()
    {
        if(instance == null)
            instance = this;
        else
            Console.WriteLine("A gameManager already exist !");
    }
    public void Init()
    {
        Console.WriteLine("Init database");
        database = new Database();
       // db.LoadDatabase(this);
    }


    public void GameLoop()
    {
        Init();
        bool isRunning = true;
        while(isRunning)
        {
 
            Console.WriteLine("Choose an actions !");
            foreach(int action in Enum.GetValues(typeof(EGMActions)))
                Console.WriteLine(action + " : " + ((EGMActions)action).ToString());
            string? input = Console.ReadLine();
            int inputConvert = MyAppLibrary.ConvertStringToInt(input);
            
            switch(inputConvert)
            {
                case (int)EGMActions.RACE:
                    RaceHandler();
                break;
                case (int)EGMActions.CAREER:
                    CarrerHandler();
                break;
                case (int)EGMActions.DATABASE:
                    DatabaseHandler();
                break;
                case (int)EGMActions.EXIT:
                    isRunning = false;
                    Console.WriteLine("Goodbye !");
                break;
                default:
                    Console.WriteLine("Entry not recognize ! Please enter a number in a relation with the different options ! ");
                    break;
            }
        }
    }

    private void RaceHandler()
    {
        int returnValue = raceList.Count + 1;
        for(int i = 0; i <raceList.Count;i++)
            Console.WriteLine(i + " : " + raceList[i].Name);
        Console.WriteLine((returnValue) + " : " + "Return");
        string? input = Console.ReadLine();
        int inputConvert = MyAppLibrary.ConvertStringToInt(input);
        
        if(inputConvert <raceList.Count)//On lance une course
        {
            
        }
        else if(inputConvert == returnValue)//on revient en arriere
        {
            
        }
        else//input non reconnu
        {
            
        }
    }
    private void CarrerHandler()
    {
        string? input = Console.ReadLine();
        int inputConvert = MyAppLibrary.ConvertStringToInt(input);
        switch(inputConvert)
        {
            case (int)EGMCarrerActions.NEW:
                Console.WriteLine("TODO ");
                break;
            case (int)EGMCarrerActions.LOAD:
                Console.WriteLine("TODO ");
                break;
            case (int)EGMCarrerActions.RETURN:
                Console.WriteLine("TODO ");
                break;
            default:
                Console.WriteLine("Entry not recognize ! Please enter a number in a relation with the different options ! ");
                break;
        }
        Console.WriteLine("TODO ");
    }

    private class DatabaseControl
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

        public DatabaseControl(GameManager? _parent) => parent = _parent;  
    
#region MODIFDB
        private void ModifyDatabase()
        {
            int input = SelectActions();
            Console.WriteLine("INput :" + input);
            Console.WriteLine("INput :" + allfiles[input]);
            parent.database.CurrentDatabaseName = allfiles[input];
            parent.database.LoadDatabase(parent);
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
                    //while(isAdding)
                    {
                        string minput = Console.ReadLine();
                        string addData;
                        
                        //try
                        //{
                          //  int inputConvert = MyAppLibrary.ConvertStringToInt(minput);
                        //}
                        //catch(Exception e)
                        {
                            addData = minput;
                            Console.WriteLine("Input : " + addData ); 
                        }
                        
                        
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
    private void DatabaseHandler()
    {
        DatabaseControl databaseControl = new DatabaseControl(this);
        databaseControl.SelectMode();
    }
}