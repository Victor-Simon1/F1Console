
using Microsoft.Data.Sqlite;

public class GameManager
{
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
        //while(true)
        {
            Console.WriteLine("Drivers size :" + driversList.Count);
            for(int i = 0;i<driversList.Count;i++)
                Console.WriteLine(driversList[i].ToString());
        }
    }
}