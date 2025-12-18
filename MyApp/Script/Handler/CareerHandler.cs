
public class CarrerHandler
{

    Race actualRace;
    int year = 0;
    List<Driver> driverList = new List<Driver>();
    List<Race> raceList = new List<Race>();
    List<Team> teamList = new List<Team>();
    string carreerPath = "";
    private enum EGMCarrerActions
    {
        NEW = 0,
        LOAD,
        RETURN
    };
    private enum EGMCarrerLoopActions
    {
        NEXTRACE = 0,
        STANDING,
        RETURN
    };
    public void SelectMode()
    {
        foreach(int action in Enum.GetValues(typeof(EGMCarrerActions)))
            Console.WriteLine(action + " : " + ((EGMCarrerActions)action).ToString());
        int inputConvert = MyAppLibrary.GetIntInput();
        switch(inputConvert)
        {
            case (int)EGMCarrerActions.NEW:
                //Console.WriteLine("TODO ");
                NewCareer();
                break;
            case (int)EGMCarrerActions.LOAD:
                LoadCareer();
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

    void CarreerLoop()
    {
        bool isLooping = true;
        while(isLooping)
        {
            foreach(int action in Enum.GetValues(typeof(EGMCarrerLoopActions)))
                Console.WriteLine(action + " : " + ((EGMCarrerLoopActions)action).ToString());
            string? input = Console.ReadLine();
            int inputConvert = MyAppLibrary.ConvertStringToInt(input);
            switch(inputConvert)
            {
                case (int)EGMCarrerLoopActions.NEXTRACE:
                    NextRace();
                break;
                case (int)EGMCarrerLoopActions.STANDING:
                    ShowRanking();
                break;
                case (int)EGMCarrerLoopActions.RETURN:
                    isLooping = false;
                break;
            }
        }
    }
    private void LoadCareer()
    {
        Console.WriteLine("Nom de la carriere?");
        //Get all folder in save  
        
        string[] allfiles = Directory.GetDirectories("Saves" , "*.*", SearchOption.TopDirectoryOnly);
        for(int i= 0; i < allfiles.Length ; i++)
            Console.WriteLine(i+ " : " + allfiles[i]);
        int input = MyAppLibrary.GetIntInput();
        bool goodEntry = MyAppLibrary.VerifyInput(input,0,GameManager.instance.raceList.Count,"TODO");
        while(!goodEntry)
        {
            input = MyAppLibrary.GetIntInput();
            goodEntry = MyAppLibrary.VerifyInput(input,0,GameManager.instance.raceList.Count,"TODO");
        }
        carreerPath = allfiles[input];
    }
    private void NewCareer()
    {
        Console.WriteLine("Basefile " + MyAppLibrary.BASEFILE);
        Console.WriteLine("Nom de la carriere?");
        string input = Console.ReadLine();
        string directory = MyAppLibrary.SAVEFILE + "/"+ input;
        Directory.CreateDirectory(directory);
        string sourceFile = MyAppLibrary.BASEFILE + "/Database/db.sqlite";
        carreerPath = directory;
        try
        {
            // Copy the file and overwrite if it already exists
            File.Copy(sourceFile, directory+"/db.sqlite", true);
            Console.WriteLine("File copied successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        string jsonSrc = directory+"/"+input+".json";
        File.Create(jsonSrc);
        SaveSystem.SaveListToJson(jsonSrc);
    }
    private void NextRace()
    {
        
    }
    private void ShowRanking()
    {
        //Drivers
        driverList.Sort((x,y) =>y.seasonPoint.CompareTo(x.seasonPoint));
        
        //Teams
        teamList.Sort((x,y) =>(y.Driver1.seasonPoint + y.Driver2.seasonPoint).CompareTo(x.Driver1.seasonPoint+x.Driver2.seasonPoint));
    }
    public void Start()
    {
        //throw new NotImplementedException();
        SelectMode();
        LoadList();
        CarreerLoop();
    }

    private void LoadList()
    {
        GameManager.instance.database.CurrentDatabaseName =  carreerPath + "/db.sqlite";
        GameManager.instance.database.LoadDatabase();
        driverList = GameManager.instance.driversList;
        raceList = GameManager.instance.raceList;
        teamList = GameManager.instance.teamsList;
    }
}