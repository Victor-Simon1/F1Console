
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
            RacingLogger.Info(action + " : " + ((EGMCarrerActions)action).ToString());
        int inputConvert = RacingLibrary.GetIntInput();
        switch(inputConvert)
        {
            case (int)EGMCarrerActions.NEW:
                NewCareer();
                break;
            case (int)EGMCarrerActions.LOAD:
                LoadCareer();
                break;
            case (int)EGMCarrerActions.RETURN:
                RacingLogger.Warning("TODO ");
                break;
            default:
                RacingLogger.Error("Entry not recognize ! Please enter a number in a relation with the different options ! ");
                break;
        }
    }

    private void NewCareer()
    {
        RacingLogger.Info("Basefile " + RacingLibrary.BASEFILE);
        RacingLogger.Info("Nom de la carriere?");
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
            RacingLogger.Info("File copied successfully at " + destinationDB);
        }
        catch (Exception ex)
        {
            RacingLogger.Error($"Error: {ex.Message}");
        }
        //System.Threading.Thread.Sleep(1000);
        if (GameManager.instance?.database == null)
        {
            RacingLogger.Error("Error: GameManager or database is not initialized");
            return;
        }
        GameManager.instance.database.CurrentDatabaseName = dbFile;
        GameManager.instance.database.info = info;
        GameManager.instance.database.LoadDatabase();

        string jsonSrc = directory+"/"+input+".json";
        File.Create(jsonSrc).Dispose();
        SaveSystem.SaveListToJson(this,jsonSrc);
        if (info != null)
        {
            foreach(Team t in info.teamsList)
                t.SetFromId(ref info);
        }

        RacingLogger.Info("Quelle systeme de points voulez vous utilisez?");
        foreach(int pointSystemName in Enum.GetValues(typeof(PointSystem.EPointSystem)))
                RacingLogger.Info(pointSystemName + " : " + ((PointSystem.EPointSystem)pointSystemName).ToString());
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
            RacingLogger.Info(carreerPath + " : GP " + indexRace + "/" + info?.raceList.Count + " - Year : " + year );
            foreach(int action in Enum.GetValues(typeof(EGMCarrerLoopActions)))
                RacingLogger.Info(action + " : " + ((EGMCarrerLoopActions)action).ToString());
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
            RacingLogger.Error("ERROR ! RACE NOT RECOGNIZE !! ");
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
        {
            RacingLogger.Error("Error: Info is not initialized");
            return;
        }
        
        for(int indexDiv = 0; indexDiv < (int)EDivisionType.MAX; indexDiv++)
        {
            EDivisionType division = (EDivisionType)indexDiv;
            
            // Vérifier si la division existe dans le dictionnaire et a des équipes
            if(!info.dictionnaryTeam.TryGetValue(division, out List<Team>? teams) || teams == null || teams.Count <= 0)
                continue;
            
            RacingLogger.Info("--- " + division + " ---- ");
            info.driversList.Clear();
            
            // Collecter tous les pilotes de cette division
            for(int indexTeam = 0; indexTeam < teams.Count; indexTeam++)
            {
                var team = teams[indexTeam];
                if (team.Driver1 != null)
                    info.driversList.Add(team.Driver1);
                if (team.Driver2 != null)
                    info.driversList.Add(team.Driver2);
            }
            
            // Afficher le classement des pilotes
            info.driversList.Sort((x,y) => y.seasonStat.seasonPoint.CompareTo(x.seasonStat.seasonPoint));
            RacingLogger.Info("Drivers Standings : ");
            string headerDriverStr = "#".PadRight(StringRacing.PadRightIndex) 
                + " Name".PadRight(StringRacing.PadRightNameDriver) + " | " 
                + "Points".PadRight(StringRacing.PadRightSeason) + " | " 
                + "Dnf".PadRight(StringRacing.PadRightSeason) + " | " 
                + "AvgPlace".PadRight(StringRacing.PadRightSeason);
            RacingLogger.Info(headerDriverStr);
            
            for(int index = 0; index < info.driversList.Count; index++)
                RacingLogger.Info("#" + (index+1).ToString().PadRight(StringRacing.PadRightIndex) 
                + info.driversList[index].GetName().PadRight(StringRacing.PadRightNameDriver) 
                + " " + info.driversList[index].ToStringSeason());

            // Afficher le classement des équipes
            teams.Sort((x,y) => y.GetSeasonPoint().CompareTo(x.GetSeasonPoint()));
            RacingLogger.Info("Teams Standings : ");
            string headerTeamStr = "#".PadRight(StringRacing.PadRightIndex) 
                            + " Name".PadRight(StringRacing.PadRightNameTeam) + " | " 
                            + "Points".PadRight(StringRacing.PadRightPointTeam);
            RacingLogger.Info(headerTeamStr);
            
            for(int index = 0; index < teams.Count; index++)
            {
                string teamName = teams[index].Name ?? "Unknown";
                RacingLogger.Info("#" + (index+1).ToString().PadRight(StringRacing.PadRightIndex) + " " 
                + teamName.PadRight(StringRacing.PadRightNameTeam) + "| " 
                + teams[index].GetSeasonPoint().ToString().PadRight(StringRacing.PadRightPointTeam));
            }
        }
    }

    private void ShowNote()
    {
        if(info == null)
        {
            RacingLogger.Error("Error: Info is not initialized");
            return;
        }
        
        for(int indexDiv = 0; indexDiv < (int)EDivisionType.MAX; indexDiv++)
        {
            EDivisionType division = (EDivisionType)indexDiv;
            
            // Vérifier si la division existe dans le dictionnaire et a des équipes
            if(!info.dictionnaryTeam.TryGetValue(division, out List<Team>? teams) || teams == null || teams.Count <= 0)
                continue;
            
            RacingLogger.Info("--- " + division + " ---- ");
            info.driversList.Clear();
            
            // Collecter tous les pilotes de cette division
            for(int indexTeam = 0; indexTeam < teams.Count; indexTeam++)
            {
                var team = teams[indexTeam];
                if (team.Driver1 != null)
                    info.driversList.Add(team.Driver1);
                if (team.Driver2 != null)
                    info.driversList.Add(team.Driver2);
            }
            
            // Afficher les notes des pilotes
            info.driversList.Sort((x,y) => y.GetGeneral().CompareTo(x.GetGeneral()));
            RacingLogger.Info("Drivers Notes : ");
            for(int index = 0; index < info.driversList.Count; index++)
                RacingLogger.Info("#" + (index+1) + " " + info.driversList[index].GetName() + " " + info.driversList[index].GetGeneral());

            // Afficher les notes des équipes
            teams.Sort((x,y) => y.GetGeneral().CompareTo(x.GetGeneral()));
            RacingLogger.Info("Teams Notes : ");
            for(int index = 0; index < teams.Count; index++)
                RacingLogger.Info("#" + (index+1) + " " + teams[index].Name + " - " + teams[index].GetGeneral());
        }
    }

    private void PassYear()
    {
        if(info == null)
        {
            RacingLogger.Error("Error: Info is not initialized");
            return;
        }
        RacingLogger.Info("New Season !! ");
        //Show Standing before next year
        ShowRanking();

        //Pass year stuff
        year++;
        indexRace = 0;
        for(int indexDiv = 0; indexDiv < (int)EDivisionType.MAX; indexDiv++)
        {
            EDivisionType division = (EDivisionType)indexDiv;
            
            // Vérifier si la division existe dans le dictionnaire
            if(!info.dictionnaryTeam.TryGetValue(division, out List<Team>? teams) || teams == null)
                continue;
            
            for(int indexTeam = 0; indexTeam < teams.Count; indexTeam++)
            {
                var team = teams[indexTeam];
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
            RacingLogger.Error("Cannot save: info or database is null");
            return;
        }

        if (string.IsNullOrEmpty(carreerPath))
        {
            RacingLogger.Error("Cannot save: career path is empty");
            return;
        }

        string[] allfiles = Directory.GetFiles(carreerPath, "*.json", SearchOption.TopDirectoryOnly);
        if (allfiles.Length == 0)
        {
            RacingLogger.Error("No JSON file found in career path");
            return;
        }

        string jsonPath = allfiles[0];
        RacingLogger.Info("Save career to path: " + jsonPath);
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
        RacingLogger.Info("Career saved successfully");
    }

    private void LoadCareer()
    {
        if(info == null)
            return;
        RacingLogger.Info("Nom de la carriere?");
        //Get all folder in save  
        
        string[] allfiles = Directory.GetDirectories("Saves" , "*.*", SearchOption.TopDirectoryOnly);
        for(int i= 0; i < allfiles.Length ; i++)
            RacingLogger.Info(i+ " : " + allfiles[i]);
        
        //int nbRace = info.raceList.Count;
        
        int input = RacingLibrary.GetValidatedIntInput(0, allfiles.Length - 1, "Invalid career selection. Please enter a number between 0 and " + (allfiles.Length - 1));
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