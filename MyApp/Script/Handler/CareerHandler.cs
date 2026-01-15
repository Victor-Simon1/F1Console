
public class CarrerHandler
{
    public int indexRace;
    Race? actualRace;
    public int year = 0;
    public Info? info = new Info();
    public PointSystem pointSystem = new PointSystem();
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
        NOTE,
        SAVE,
        RETURN
    };

        
    public void Start()
    {
        SelectMode();
        CarreerLoop();
    }

    #region CAREER_ACTIONS
    public void SelectMode()
    {
        foreach(int action in Enum.GetValues(typeof(EGMCarrerActions)))
            Console.WriteLine(action + " : " + ((EGMCarrerActions)action).ToString());
        int inputConvert = RacingLibrary.GetIntInput();
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

    private void NewCareer()
    {
        Console.WriteLine("Basefile " + RacingLibrary.BASEFILE);
        Console.WriteLine("Nom de la carriere?");
        string? input = Console.ReadLine();

        string directory = RacingLibrary.SAVEFILE + "\\"+ input;
        string destinationDB = directory + "/db.sqlite";
        string sourceFile = RacingLibrary.BASEFILE + "/Database/db.sqlite";
        string dbFile = "Saves/"+input+"/db.sqlite" ;

        Directory.CreateDirectory(directory);
        carreerPath = directory;
        try
        {
            // Copy the file and overwrite if it already exists
            File.Copy(sourceFile, destinationDB);
            Console.WriteLine("File copied successfully at " + destinationDB);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        //System.Threading.Thread.Sleep(1000);
        GameManager.instance.database.CurrentDatabaseName = dbFile;
        GameManager.instance.database.info = info;
        GameManager.instance.database.LoadDatabase();

        string jsonSrc = directory+"/"+input+".json";
        File.Create(jsonSrc).Dispose();
        SaveSystem.SaveListToJson(this,jsonSrc);
        foreach(Team t in info.teamsList)
            t.SetFromId(ref info);

        Console.WriteLine("Quelle systeme de points voulez vous utilisez?");
        foreach(int pointSystemName in Enum.GetValues(typeof(PointSystem.EPointSystem)))
                Console.WriteLine(pointSystemName + " : " + ((PointSystem.EPointSystem)pointSystemName).ToString());
        int pSystemInput = RacingLibrary.GetIntInput();
        pointSystem.actualPointSystem = (PointSystem.EPointSystem)pSystemInput;
    }

  
#endregion

#region CAREER_GESTION
    void CarreerLoop()
    {
        bool isLooping = true;
        while(isLooping)
        {
            Console.WriteLine(carreerPath + " : GP " + indexRace + "/" + info?.raceList.Count + " - Year : " + year );
            foreach(int action in Enum.GetValues(typeof(EGMCarrerLoopActions)))
                Console.WriteLine(action + " : " + ((EGMCarrerLoopActions)action).ToString());
            string? input = Console.ReadLine();
            int inputConvert = RacingLibrary.ConvertStringToInt(input);
            switch(inputConvert)
            {
                case (int)EGMCarrerLoopActions.NEXTRACE:
                    NextRace();
                break;
                case (int)EGMCarrerLoopActions.STANDING:
                    ShowRanking();
                break;
                case (int)EGMCarrerLoopActions.NOTE:
                    ShowNote();
                break;
                case (int)EGMCarrerLoopActions.SAVE:
                    SaveCareer();
                break;
                case (int)EGMCarrerLoopActions.RETURN:
                    isLooping = false;
                break;
            }
        }
    }

    private void NextRace()
    {
        if(info == null)
            return;
        if(indexRace >= info.raceList.Count)
        {
            Console.WriteLine("ERROR ! RACE NOT RECOGNIZE !! ");
            return;
        }
        actualRace = info.raceList[indexRace];
        RaceHandler raceHandler = new RaceHandler(ref info,ref pointSystem);

        raceHandler.SetRace(indexRace);
        //raceHandler.SetInfo(info.driversList,info.teamsList);
        for(int division = 0; division<(int)EDivisionType.MAX;division++)
            raceHandler.StartSimulation((EDivisionType)division);
        
        //raceHandler.SetChampionshipPoint();
        //raceHandler.ResetDriverRaceVariable();

       
        indexRace++;
        if(indexRace>= info.raceList.Count)
        {
            PassYear();    
        }
    }
    private void ShowRanking()
    {
        if(info == null)
            return;
        
        
        for(int indexDiv =0;indexDiv<(int)EDivisionType.MAX;indexDiv++)
        {
            if(info.dictionnaryTeam[(EDivisionType)indexDiv].Count <=0)
                continue;
            Console.WriteLine("--- " + (EDivisionType)indexDiv + " ---- ");
            info.driversList.Clear();
            for(int indexTeam =0; indexTeam<info.dictionnaryTeam[(EDivisionType)indexDiv].Count;indexTeam++)
            {
                var team = info.dictionnaryTeam[(EDivisionType)indexDiv][indexTeam];
                if (team.Driver1 != null)
                    info.driversList.Add(team.Driver1);
                if (team.Driver2 != null)
                    info.driversList.Add(team.Driver2);
            }
            //Drivers
            info.driversList.Sort((x,y) =>y.seasonStat.seasonPoint.CompareTo(x.seasonStat.seasonPoint));
            Console.WriteLine("Drivers Standings : ");
            string headerDriverStr = "#".PadRight(StringRacing.PadRightIndex) 
                + " Name".PadRight(StringRacing.PadRightNameDriver) + " | " 
                + "Points".PadRight(StringRacing.PadRightSeason) + " | " 
                + "Dnf".PadRight(StringRacing.PadRightSeason) + " | " 
                + "AvgPlace".PadRight(StringRacing.PadRightSeason);
                Console.WriteLine(headerDriverStr);
            for(int index = 0;index<info.driversList.Count;index++)
                Console.WriteLine("#"+(index+1).ToString().PadRight(StringRacing.PadRightIndex) 
                + info.driversList[index].GetName().PadRight(StringRacing.PadRightNameDriver) 
                + " " + info.driversList[index].ToStringSeason());

             //Teams
            //for(int indexTeam =0;indexTeam<(int)EDivisionType.MAX;indexTeam++)
            {
                info.dictionnaryTeam[(EDivisionType)indexDiv].Sort((x,y) =>y.GetSeasonPoint().CompareTo(x.GetSeasonPoint()));
                
                Console.WriteLine("Teams Standings : ");
                string headerTeamStr = "#".PadRight(StringRacing.PadRightIndex) 
                                + " Name".PadRight(StringRacing.PadRightNameTeam) + " | " 
                                + "Points".PadRight(StringRacing.PadRightPointTeam);
                Console.WriteLine(headerTeamStr);
                for(int index = 0;index<info.dictionnaryTeam[(EDivisionType)indexDiv].Count;index++)
                    Console.WriteLine("#"+(index+1).ToString().PadRight(StringRacing.PadRightIndex) + " " 
                    +info.dictionnaryTeam[(EDivisionType)indexDiv][index].Name.PadRight(StringRacing.PadRightNameTeam)  + "| " 
                    + info.dictionnaryTeam[(EDivisionType)indexDiv][index].GetSeasonPoint().ToString().PadRight(StringRacing.PadRightPointTeam));
            }
        }
    }

    private void ShowNote()
    {
        if(info == null)
            return;
        for(int indexDiv =0;indexDiv<(int)EDivisionType.MAX;indexDiv++)
        {
            if(info.dictionnaryTeam[(EDivisionType)indexDiv].Count <=0)
                continue;
            Console.WriteLine("--- " + (EDivisionType)indexDiv + " ---- ");
            info.driversList.Clear();
            for(int indexTeam =0; indexTeam<info.dictionnaryTeam[(EDivisionType)indexDiv].Count;indexTeam++)
            {
                var team = info.dictionnaryTeam[(EDivisionType)indexDiv][indexTeam];
                if (team.Driver1 != null)
                    info.driversList.Add(team.Driver1);
                if (team.Driver2 != null)
                    info.driversList.Add(team.Driver2);
            }
            //Drivers
            info.driversList.Sort((x,y) =>y.GetGeneral().CompareTo(x.GetGeneral()));
            Console.WriteLine("Drivers Notes : ");
            for(int index = 0;index<info.driversList.Count;index++)
                Console.WriteLine("#"+(index+1) + " " + info.driversList[index].GetName()+ " " + info.driversList[index].GetGeneral());

             //Teams
            //for(int indexTeam =0;indexTeam<(int)EDivisionType.MAX;indexTeam++)
            {
                info.dictionnaryTeam[(EDivisionType)indexDiv].Sort((x,y) =>y.GetGeneral().CompareTo(x.GetGeneral()));
                Console.WriteLine("Teams Standings : ");
                for(int index = 0;index<info.dictionnaryTeam[(EDivisionType)indexDiv].Count;index++)
                    Console.WriteLine("#"+(index+1) + " " +info.dictionnaryTeam[(EDivisionType)indexDiv][index].Name + " - " + info.dictionnaryTeam[(EDivisionType)indexDiv][index].GetGeneral());
            }
        }
    }

    private void PassYear()
    {
        Console.WriteLine("New Season !! ");
        //Show Standing before next year
        ShowRanking();

        //Pass year stuff
        year++;
        indexRace = 0;
        for(int indexDiv = 0;indexDiv< (int)EDivisionType.MAX;indexDiv++)
        {
            for(int indexTeam = 0;indexTeam<info.dictionnaryTeam[(EDivisionType)indexDiv].Count;indexTeam++)
            {
                var team = info.dictionnaryTeam[(EDivisionType)indexDiv][indexTeam];
                if (team.Driver1 != null)
                {
                    team.Driver1.seasonStat.seasonPoint = 0;
                    team.Driver1.UpdateStats();
                }
                if (team.Driver2 != null)
                {
                    team.Driver2.seasonStat.seasonPoint = 0;
                    team.Driver2.UpdateStats();
                }
            }
        }
            
    }
#endregion

#region CAREER_LOAD_SAVE
    private void SaveCareer()
    {
        if (info == null || GameManager.instance?.database == null)
        {
            Console.WriteLine("Cannot save: info or database is null");
            return;
        }

        if (string.IsNullOrEmpty(carreerPath))
        {
            Console.WriteLine("Cannot save: career path is empty");
            return;
        }

        string[] allfiles = Directory.GetFiles(carreerPath, "*.json", SearchOption.TopDirectoryOnly);
        if (allfiles.Length == 0)
        {
            Console.WriteLine("No JSON file found in career path");
            return;
        }

        string jsonPath = allfiles[0];
        Console.WriteLine("Save career to path: " + jsonPath);
        SaveSystem.SaveListToJson(this, jsonPath);
        
        foreach(Team team in info.teamsList)
        {
            if (team == null)
                continue;

            GameManager.instance.database.UpdateRow<Team>(team, "teams");
            if (team.Driver1 != null)
                GameManager.instance.database.UpdateRow<Driver>(team.Driver1, "drivers");
            if (team.Driver2 != null)
                GameManager.instance.database.UpdateRow<Driver>(team.Driver2, "drivers");
        }
        Console.WriteLine("Career saved successfully");
    }

    private void LoadCareer()
    {
        if(info == null)
            return;
        Console.WriteLine("Nom de la carriere?");
        //Get all folder in save  
        
        string[] allfiles = Directory.GetDirectories("Saves" , "*.*", SearchOption.TopDirectoryOnly);
        for(int i= 0; i < allfiles.Length ; i++)
            Console.WriteLine(i+ " : " + allfiles[i]);
        
        //int nbRace = info.raceList.Count;
        
        int input = RacingLibrary.GetIntInput();
        bool goodEntry = RacingLibrary.VerifyInput(input, 0, allfiles.Length, "TODO");
        while(!goodEntry)
        {
            input = RacingLibrary.GetIntInput();
            goodEntry = RacingLibrary.VerifyInput(input,0,allfiles.Length,"TODO");
        }
        carreerPath = allfiles[input];

        LoadList();
    }

    private void LoadList()
    {
        if(GameManager.instance == null|| GameManager.instance.database == null || info == null)
            return;
        GameManager.instance.database.CurrentDatabaseName =  carreerPath + "/db.sqlite";
        GameManager.instance.database.info = info;
        GameManager.instance.database.LoadDatabase();
        string[] allfiles = Directory.GetFiles(carreerPath, "*.json", SearchOption.TopDirectoryOnly);
        string jsonPath = allfiles[0];

        SaveSystem.Save loadingSave = SaveSystem.LoadFromJson(ref info,jsonPath);
        indexRace = loadingSave.actualRace;
        year = loadingSave.year;
        pointSystem.actualPointSystem = (PointSystem.EPointSystem)loadingSave.pointSystemUse;
        foreach(Team t in info.teamsList)
            t.SetFromId(ref info);
    }
#endregion


}