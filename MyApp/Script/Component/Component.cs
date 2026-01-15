using Microsoft.Data.Sqlite;

public interface Component
{
    public int Id{get;set;}
    public static abstract string READDB{get;}
    //public string UpdateRowString();
    public void LoadData(SqliteDataReader reader);

}

public class Info
{
    public List<Driver> driversList = new List<Driver>();
    public Dictionary<EDivisionType,List<Team>> dictionnaryTeam = new Dictionary<EDivisionType, List<Team>>();
    public List<Team> teamsList = new List<Team>();
    public List<Motor> motorList = new List<Motor>();
    public List<Chassis> chassisList = new List<Chassis>();
    //Tyre
    public List<TireBrand> tireList = new List<TireBrand>();
    public List<TireType> tireTypeList = new List<TireType>();
    //RaceList
    public List<Race> raceList = new List<Race>();
    public List<RaceType> raceTypeList = new List<RaceType>();
}