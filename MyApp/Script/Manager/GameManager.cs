
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

    public Database? database;

    public DateTime baseYear = new DateTime(2026,1,1);
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
    }

    public void GameLoop()
    {
        Init();
        bool isRunning = true;
        while(isRunning)
        {
            Console.WriteLine("Choose an actions !");
            //database.ClearDatabase(ref info);
            foreach(int action in Enum.GetValues(typeof(EGMActions)))
                Console.WriteLine(action + " : " + ((EGMActions)action).ToString());
            string? input = Console.ReadLine();
            int inputConvert = RacingLibrary.ConvertStringToInt(input);
            
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