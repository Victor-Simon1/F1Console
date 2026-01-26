
using Microsoft.Data.Sqlite;
using System.Diagnostics;

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
            RacingLogger.Error("A gameManager already exist !");
    }
    public void Init()
    {
        if (!Debugger.IsAttached)
            RacingLogger.MinimumLevel = RacingLogger.LogLevel.Info;
        database = new Database();
    }

    public void GameLoop()
    {

        Init();
        database.Verify();
        bool isRunning = true;
        while(isRunning)
        {
            RacingLogger.Info("Choose an actions !");
            //database.ClearDatabase(ref info);
            foreach(int action in Enum.GetValues(typeof(EGMActions)))
                RacingLogger.Info(action + " : " + ((EGMActions)action).ToString());
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
                    RacingLogger.Info("Goodbye !");
                break;
                default:
                    RacingLogger.Info("Entry not recognize ! Please enter a number in a relation with the different options ! ");
                    break;
            }
        }
    }
}