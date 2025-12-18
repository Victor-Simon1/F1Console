
public class RaceHandler
{
    List<Race> raceList = new List<Race>();
    int indexRace = -1;
    private string[] allfiles;

    List<Driver> driversList = new List<Driver>();
    List<Team> teamList = new List<Team>();
    public void Start()
    {
        ChooseDatabase();
        ChooseRace();
        teamList = GameManager.instance.teamsList;
        driversList = GameManager.instance.driversList;
        StartSimulation();
    }
    private void GetDatabases()
    {
        Console.WriteLine("Choose a db");
        allfiles = Directory.GetFiles("Database", "*.sqlite", SearchOption.AllDirectories);
        for(int i= 0;i<allfiles.Length;i++)
            Console.WriteLine(i+ " : " + allfiles[i]);
    }

    private void ChooseDatabase()
    {
        GetDatabases();
        int input = MyAppLibrary.GetIntInput();
        bool goodEntry = MyAppLibrary.VerifyInput(input,0,allfiles.Length,"TODO");
        while(!goodEntry)
        {
            input = MyAppLibrary.GetIntInput();
            goodEntry = MyAppLibrary.VerifyInput(input,0,allfiles.Length,"TODO");
        }
        GameManager.instance.database.CurrentDatabaseName = allfiles[input];
        GameManager.instance.database.LoadDatabase();
    }

    private void ChooseRace()
    {
        Console.WriteLine("Choose a race ! " + GameManager.instance.raceList.Count );
        if(GameManager.instance.raceList.Count <= 0)
            return;
        for(int i= 0;i<GameManager.instance.raceList.Count;i++)
            Console.WriteLine(i+ " : " + GameManager.instance.raceList[i].Name);
        int input = MyAppLibrary.GetIntInput();
        bool goodEntry = MyAppLibrary.VerifyInput(input,0,GameManager.instance.raceList.Count,"TODO");
        while(!goodEntry)
        {
            input = MyAppLibrary.GetIntInput();
            goodEntry = MyAppLibrary.VerifyInput(input,0,GameManager.instance.raceList.Count,"TODO");
        }
        indexRace = input;
    }

    private void StartSimulation()
    {
        if(indexRace == -1)
            return;
        Race currentRace = GameManager.instance.raceList[indexRace];
        Console.WriteLine(currentRace.ToString());

        float maxPoint = currentRace.MaxTour * Driver.CalculMaxPoints(currentRace.NbTurn,currentRace.Length);

        foreach(Driver driver in driversList )
        {
            driver.CalculDayForm();
        }

        for(int tour = 0 ; tour < currentRace.MaxTour;tour++)
        {
            foreach(Team team in teamList )
            {
                team.Driver1.CalculatePointPerTours(currentRace.NbTurn,currentRace.Length,team);
                team.Driver2.CalculatePointPerTours(currentRace.NbTurn,currentRace.Length,team);
            }
                
        }
        Console.WriteLine("Recap " + teamList.Count);
        driversList.Sort((x,y) =>y.racePoints.CompareTo(x.racePoints));
        teamList.Sort((x,y) =>y.GetGeneral().CompareTo(x.GetGeneral()));
      /*  foreach(Team team in teamList )
        {
            Console.WriteLine(team.Name + " : " + team.GetGeneral());
             
        }*/

        Console.WriteLine("-------------------");
        for(int pos = 0;pos<driversList.Count;pos++ )
        {
            Driver d = driversList[pos];
            Console.WriteLine("#" + (pos+1) +":" +d.FirstName + d.LastName + " : " + (d.racePoints/maxPoint) + " / " + d.GetGeneral()  +  " / " + d.dayForm);
        }

    }

}