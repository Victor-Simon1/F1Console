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
         RacingLogger.Info(StringRacing.CenterString($"----------------"));
        if(!currentRace.divisionRacing.Contains(division) || info.dictionnaryTeam[division].Count <=0)
        {
            RacingLogger.Info("No race in "+ division + " this week!");
            return;
        }
        else
        {
            RacingLogger.Info(StringRacing.CenterString($"Division {division}"));
        }
            
            
        RacingLogger.Info(currentRace.ToString());

        float maxPoint = currentRace.MaxTour * Driver.CalculMaxPoints(currentRace.NbTurn,currentRace.Length);
        RacingLogger.Debug("Max point " + maxPoint);
        foreach(Team team in info.dictionnaryTeam[division] )
        {
            for(int i=0;i<team.driversList.Length;i++)
            {
                if(team.driversList[i] != null)
                {
                    team.driversList[i].CalculDayForm();
                }
            }
        }

        for(int tour = 0 ; tour < currentRace.MaxTour;tour++)
        {
            foreach(Team team in info.dictionnaryTeam[division] )
            {
                for(int i=0;i<team.driversList.Length;i++)
                {
                    if(team.driversList[i] != null)
                    {
                        EventExtensions.TriggerEvent(team.driversList[i]);
                        if(!team.driversList[i].raceStat.hasDNF)
                            team.driversList[i].CalculatePointPerTours(currentRace.NbTurn,currentRace.Length,team);
                    }
                }
            }

                
        }
        RacingLogger.Debug("Recap " + info.dictionnaryTeam[division].Count);

        info.driversList.Clear();
        RacingLogger.Debug(info.driversList.Count.ToString());
        foreach(Team team in info.dictionnaryTeam[division] )
        {
             for(int i=0;i<team.driversList.Length;i++)
                if(team.driversList[i] != null)  
                    info.driversList.Add(team.driversList[i]);
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

            string driverInfoStr = ("#" + (pos + 1)).PadRight(StringRacing.PadRightIndex) + StringRacing.Separator +
                                   d.GetName().PadRight(StringRacing.PadRightNameDriver) + StringRacing.Separator +
                                  ((d.raceStat.racePoints / maxPoint) * 100f).ToString().PadRight(StringRacing.PadRightNameDriver) + StringRacing.Separator +
                                   d.GetGeneral().ToString().PadRight(StringRacing.PadRightGeneralAndPotential) + StringRacing.Separator +
                                   d.raceStat.dayForm.ToString().PadRight(StringRacing.PadRightGeneralAndPotential) + StringRacing.Separator +
                                   d.raceStat.hasDNF.ToString().PadRight(StringRacing.PadRightGeneralAndPotential) ;
            RacingLogger.Info(driverInfoStr);

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
            for(int i=0;i<team.driversList.Length;i++)
                if(team.driversList[i] != null) 
                    team.driversList[i].ResetRaceVariable();
        }
    }

}