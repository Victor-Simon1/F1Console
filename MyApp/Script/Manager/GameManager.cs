
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
        DEBUG,
        EXIT
    };


    public static GameManager? instance;
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

       //Set Driver Coeff
       Driver.SetCoeff();
    }


    public void GameLoop()
    {
        Init();
        bool isRunning = true;
        while(isRunning)
        {
            Console.WriteLine("Choose an actions !");
            database.ClearDatabase();
            foreach(int action in Enum.GetValues(typeof(EGMActions)))
                Console.WriteLine(action + " : " + ((EGMActions)action).ToString());
            string? input = Console.ReadLine();
            int inputConvert = MyAppLibrary.ConvertStringToInt(input);
            
            switch(inputConvert)
            {
                case (int)EGMActions.RACE:
                   RaceHandler raceHandler = new RaceHandler();
                   raceHandler.Start();
                break;
                case (int)EGMActions.CAREER:
                    CarrerHandler carrerHandler = new CarrerHandler();
                    carrerHandler.Start();
                break; 
                case (int)EGMActions.DATABASE:
                    DatabaseHandler databaseHandler = new DatabaseHandler(this);
                    databaseHandler.SelectMode();
                    break;
                case (int)EGMActions.DEBUG:
    
                    Console.WriteLine("Choose a db");
                    string[] allfiles = Directory.GetFiles("Database", "*.sqlite", SearchOption.AllDirectories);
                    for(int i= 0;i<allfiles.Length;i++)
                        Console.WriteLine(i+ " : " + allfiles[i]);
                    int myinput = MyAppLibrary.GetIntInput();
                    bool goodEntry = MyAppLibrary.VerifyInput(myinput,0,allfiles.Length,"TODO");
                    while(!goodEntry)
                    {
                        myinput = MyAppLibrary.GetIntInput();
                        goodEntry = MyAppLibrary.VerifyInput(myinput,0,allfiles.Length,"TODO");
                    }
                    database.CurrentDatabaseName = allfiles[myinput];
                    database.LoadDatabase();
                    database.GetTables();
                    foreach(string tableName in database.tablesNameList)
                    {
                        database.ShowTable(tableName);
                    }
                    SaveSystem.SaveListToJson();
                    SaveSystem.LoadFromJson();
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



}