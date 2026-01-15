public class RaceHandler
{
    
    int indexRace = -1;
    private string[] allfiles;
    Info info;
    PointSystem pointSystem;

    public RaceHandler(ref Info _info, ref PointSystem _pointSystem)
    {
        info = _info;
        pointSystem = _pointSystem;
    }
    public RaceHandler()
    {
        info = new Info();
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
        Console.WriteLine("Choose a db");
        allfiles = Directory.GetFiles("Database", "*.sqlite", SearchOption.AllDirectories);
        for(int i= 0;i<allfiles.Length;i++)
            Console.WriteLine(i+ " : " + allfiles[i]);
    }

    private void ChooseDatabase()
    {
        if (GameManager.instance?.database == null)
        {
            Console.WriteLine("GameManager or database is not initialized");
            return;
        }

        GetDatabases();
        if (allfiles == null || allfiles.Length == 0)
        {
            Console.WriteLine("No databases found");
            return;
        }

        int input = RacingLibrary.GetIntInput();
        bool goodEntry = RacingLibrary.VerifyInput(input, 0, allfiles.Length, "Invalid database selection");
        while(!goodEntry)
        {
            input = RacingLibrary.GetIntInput();
            goodEntry = RacingLibrary.VerifyInput(input, 0, allfiles.Length, "Invalid database selection");
        }
        GameManager.instance.database.CurrentDatabaseName = allfiles[input];
        GameManager.instance.database.info = info;
        GameManager.instance.database.LoadDatabase();
    }

    private void ChooseRace()
    {
        Console.WriteLine("Choose a race ! " + info.raceList.Count );
        if(info.raceList.Count <= 0)
            return;
        for(int i= 0;i<info.raceList.Count;i++)
            Console.WriteLine(i+ " : " + info.raceList[i].Name);
        int input = RacingLibrary.GetIntInput();
        bool goodEntry = RacingLibrary.VerifyInput(input,0,info.raceList.Count,"TODO");
        while(!goodEntry)
        {
            input = RacingLibrary.GetIntInput();
            goodEntry = RacingLibrary.VerifyInput(input,0,info.raceList.Count,"TODO");
        }
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
        Console.WriteLine("Division " +division);
        if(!currentRace.divisionRacing.Contains(division) || info.dictionnaryTeam[division].Count <=0)
        {
            Console.WriteLine("Pas de course de cette division "+ division + " cette semaine !");
            return;
        }
            
        Console.WriteLine(currentRace.ToString());

        float maxPoint = currentRace.MaxTour * Driver.CalculMaxPoints(currentRace.NbTurn,currentRace.Length);
        Console.WriteLine("Max point " + maxPoint);
        foreach(Team team in info.dictionnaryTeam[division] )
        {
            if (team.Driver1 == null || team.Driver2 == null)
            {
                Console.WriteLine($"Warning: Team {team.Name} has null drivers, skipping");
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
        Console.WriteLine("Recap " + info.dictionnaryTeam[division].Count);

        info.driversList.Clear();
        Console.WriteLine(info.driversList.Count);
        foreach(Team team in info.dictionnaryTeam[division] )
        {
            if (team.Driver1 != null)
                info.driversList.Add(team.Driver1);
            if (team.Driver2 != null)
                info.driversList.Add(team.Driver2);
        }
        info.driversList.Sort((x,y) =>y.raceStat.racePoints.CompareTo(x.raceStat.racePoints));
        info.dictionnaryTeam[division].Sort((x,y) =>y.GetGeneral().CompareTo(x.GetGeneral()));

        
        Console.WriteLine("-------------------");
        for(int pos = 0; pos < info.driversList.Count ; pos++ )
        {
            Driver d = info.driversList[pos];
            float pen = 0;
            for(int i =0;i<d.raceStat.penaltyPoint.Length;i++)
                pen += d.raceStat.penaltyPoint[i];
            Console.WriteLine("#" + (pos+1) +":" +d.FirstName + d.LastName + " : " + (d.raceStat.racePoints/maxPoint)*100f + " / " + d.GetGeneral()  +
              " / " + d.raceStat.dayForm + " / " + d.raceStat.hasDNF+ "/ " + pen);

            d.seasonStat.sumPlace += (pos+1);
            d.seasonStat.nbRaceMake++;
        }
        List<Team> tempList = info.dictionnaryTeam[division];
        pointSystem.ApplyPointToDriver(ref tempList);
        info.dictionnaryTeam[division] = tempList;

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