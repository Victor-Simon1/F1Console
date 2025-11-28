
using Microsoft.Data.Sqlite;

public class GameManager
{
    private enum EGMActions
    {
        RACE = 0,
        CAREER,
        EXIT
    };
    private enum EGMCarrerActions
    {
        NEW = 0,
        LOAD,
        RETURN
    };
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


    public void Init()
    {
        Database db = new Database();
        db.LoadDatabase(this);
    }


    public void GameLoop()
    {
        Init();
        bool isRunning = true;
        //while(isRunning)
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
}