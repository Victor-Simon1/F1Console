
public class RaceHandler
{
    List<Race> raceList = new List<Race>();
    int indexRace = -1;
    private string[] allfiles;


    public void Start()
    {
        ChooseDatabase();
        ChooseRace();
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
        if(GameManager.instance.raceList.Count <= 0)
            return;
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

        for(int tour = 0 ; tour < currentRace.MaxTour;tour++)
        {
            
        }
    }
}