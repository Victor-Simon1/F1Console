public class RaceHandler
{
    
    int indexRace = -1;
    private string[]? allfiles;
    Info info;
    PointSystem? pointSystem;

    public RaceHandler(ref Info _info, ref PointSystem _pointSystem)
    {
        info = _info;
        pointSystem = _pointSystem;
    }
    public RaceHandler()
    {
        info = new Info();
        pointSystem = null;
    }
    public void Start()
    {
        
        ChooseDatabase();
        ChooseRace();
        for(int division = 0; division<(int)EDivisionType.MAX;division++)
            StartSimulation((EDivisionType)division);
    }
    private void GetDatabases()
    {
        RacingLogger.Info("Choose a db");
        allfiles = Directory.GetFiles("Database", "*.sqlite", SearchOption.AllDirectories);
        for(int i= 0;i<allfiles.Length;i++)
            RacingLogger.Info(i+ " : " + allfiles[i]);
    }

    private void ChooseDatabase()
    {
        if (GameManager.instance?.database == null)
        {
            RacingLogger.Error("GameManager or database is not initialized");
            return;
        }

        GetDatabases();
        if (allfiles == null || allfiles.Length == 0)
        {
            RacingLogger.Error("No databases found");
            return;
        }

        int input = RacingLibrary.GetValidatedIntInput(0, allfiles.Length - 1, "Invalid database selection. Please enter a number between 0 and " + (allfiles.Length - 1));
        GameManager.instance.database.CurrentDatabaseName = allfiles[input];
        GameManager.instance.database.info = info;
        GameManager.instance.database.LoadDatabase();
        if (info != null)
        {
            foreach(Team t in info.teamsList)
                t.SetFromId(ref info);
        }
    }

    private void ChooseRace()
    {
        RacingLogger.Info("Choose a race ! " + info.raceList.Count );
        if(info.raceList.Count <= 0)
            return;
        for(int i= 0;i<info.raceList.Count;i++)
            RacingLogger.Info(i+ " : " + info.raceList[i].Name);
        int input = RacingLibrary.GetValidatedIntInput(0, info.raceList.Count - 1, "Invalid race selection. Please enter a number between 0 and " + (info.raceList.Count - 1));
        indexRace = input;
    }
    public void SetRace(int _indexRace)
    {
        indexRace = _indexRace;
    }
    public void SetInfo(List<Driver> _driversList,List<Team> _teamsList)
    {
        info.teamsList = _teamsList;
        info.driversList = _driversList;
    }
    public void StartSimulation(EDivisionType division)
    {
        if(indexRace == -1)
            return;
        EventExtensions.InitEvent();
        Race currentRace = info.raceList[indexRace];
        //foreach(EDivisionType eDivisionType in currentRace.divisionRacing)
        RacingLogger.Info("Division " +division);
        if(!currentRace.divisionRacing.Contains(division) || info.dictionnaryTeam[division].Count <=0)
        {
            RacingLogger.Info("Pas de course de cette division "+ division + " cette semaine !");
            return;
        }
            
        RacingLogger.Info(currentRace.ToString());

        float maxPoint = currentRace.MaxTour * Driver.CalculMaxPoints(currentRace.NbTurn,currentRace.Length);
        RacingLogger.Info("Max point " + maxPoint);
        foreach(Team team in info.dictionnaryTeam[division] )
        {
            if (team.Driver1 == null || team.Driver2 == null)
            {
                RacingLogger.Warning($"Warning: Team {team.Name} has null drivers, skipping");
                continue;
            }
            team.Driver1.CalculDayForm();
            team.Driver2.CalculDayForm();
        }

        for(int tour = 0 ; tour < currentRace.MaxTour;tour++)
        {
            foreach(Team team in info.dictionnaryTeam[division] )
            {
                if (team.Driver1 == null || team.Driver2 == null)
                    continue;

                EventExtensions.TriggerEvent(team.Driver1);
                if(!team.Driver1.raceStat.hasDNF)
                    team.Driver1.CalculatePointPerTours(currentRace.NbTurn,currentRace.Length,team);
                EventExtensions.TriggerEvent(team.Driver2);
                if(!team.Driver2.raceStat.hasDNF)
                    team.Driver2.CalculatePointPerTours(currentRace.NbTurn,currentRace.Length,team);
            }
                
        }
        RacingLogger.Info("Recap " + info.dictionnaryTeam[division].Count);

        info.driversList.Clear();
        RacingLogger.Info(info.driversList.Count.ToString());
        foreach(Team team in info.dictionnaryTeam[division] )
        {
            if (team.Driver1 != null)
                info.driversList.Add(team.Driver1);
            if (team.Driver2 != null)
                info.driversList.Add(team.Driver2);
        }
        info.driversList.Sort((x,y) =>y.raceStat.racePoints.CompareTo(x.raceStat.racePoints));
        info.dictionnaryTeam[division].Sort((x,y) =>y.GetGeneral().CompareTo(x.GetGeneral()));

        
        RacingLogger.Info("-------------------");
        for(int pos = 0; pos < info.driversList.Count ; pos++ )
        {
            Driver d = info.driversList[pos];
            float pen = 0;
            for(int i =0;i<d.raceStat.penaltyPoint.Length;i++)
                pen += d.raceStat.penaltyPoint[i];
            RacingLogger.Info("#" + (pos+1) +":" +d.FirstName + d.LastName + " : " + (d.raceStat.racePoints/maxPoint)*100f + " / " + d.GetGeneral()  +
              " / " + d.raceStat.dayForm + " / " + d.raceStat.hasDNF+ "/ " + pen);

            d.seasonStat.sumPlace += (pos+1);
            d.seasonStat.nbRaceMake++;
        }
        List<Team> tempList = info.dictionnaryTeam[division];
        if (pointSystem != null)
        {
            pointSystem.ApplyPointToDriver(ref tempList);
            info.dictionnaryTeam[division] = tempList;
        }

        ResetDriverRaceVariable();
    }

    public void ResetDriverRaceVariable()
    {
        foreach(Team team in info.teamsList)
        {
            if (team.Driver1 != null)
                team.Driver1.ResetRaceVariable();
            if (team.Driver2 != null)
                team.Driver2.ResetRaceVariable();
        }
    }

}