using System.Text.Json.Serialization;
using Microsoft.Data.Sqlite;


public class  Driver : Component,IUpdatable
{
    private enum EDriverStats : int
    {
        TURN = 0,
        BREAK ,
        OVERTAKE ,
        DEFENSE ,
        TYRECONTROL ,
        REGULARITY ,
        REACTIVITY,
        MAX
    };
    public enum ESexe
    {
        Men,
        Women
    };
    public struct SeasonStat
    {
        public int seasonPoint =0;
        public int nbDnf =0;

        public float sumPlace = 0f;
        public int nbRaceMake = 0;
        public SeasonStat(){}
        public new string ToString()
        {
            float avgPlace = 0;
            if(nbRaceMake != 0)
                avgPlace = sumPlace / nbRaceMake;
            string seasonPointStr = ("| "+seasonPoint + " ").PadRight(StringRacing.PadRightSeason);
            string nbDnfStr = ("| "+ nbDnf + " ").PadRight(StringRacing.PadRightSeason);
            string avgPlaceStr = ("| "+ avgPlace+ " ").PadRight(StringRacing.PadRightSeason);
            return seasonPointStr + nbDnfStr + avgPlaceStr; 
        }
    }
    public struct RaceStat
    {
        public int dayForm = 0;
        public float racePoints = 0;
        public bool hasDNF = false;
        public float[] penaltyPoint = new float[(int)EEventType.MAX_EVENT];
        public int[] driverForm = new int[(int)EDriverStats.MAX];
        public RaceStat(){}

        public void Reset()
        {
            dayForm = 0;
            racePoints = 0;
            hasDNF = false;
            for(int i = 0; i<penaltyPoint.Length;i++)
            {
                penaltyPoint[i] = 0;
            }
        }
    }
    
    public static string READDB => "SELECT * FROM drivers";
    public int Id { get ; set; }
    public string? FirstName{get;set;}
    public string? LastName {get;set;}
    public string? Birthdate{get;set;}
    public ESexe driverSexe{get;set;}

    //Statistic
    public int potential;
    public int[] driverStats = new int[(int)EDriverStats.MAX];

    public static float[] driverTurnCoeff = {1.2f,1.8f,0.25f,1f,0.1f,0.1f,0.7f};
    public static float[] driverLineCoeff = {0.05f,0.2f,1.7f,1.6f,1.5f,.1f,0.5f};
    //Race Variable
    public RaceStat raceStat = new RaceStat();
    //Carreer Variable
    public SeasonStat seasonStat = new SeasonStat();
    //const variable
    const float turnDriverCoeff = 0.7f;
    const float turnTeamCoeff = 0.5f;
    const float lineDriverCoeff = 0.4f;
    const float lineTeamCoeff = 1.7f;

    #region INIT_FUNCTION
    public void LoadData(SqliteDataReader reader)
    {
        if (reader == null)
        {
            RacingLogger.Error("LoadData: reader is null");
            return;
        }

        Id = reader.GetInt32(0);
        FirstName = reader.GetString(1);
        LastName = reader.GetString(2);
        Birthdate = reader.GetString(3);

        //Stats

        driverStats[(int)EDriverStats.TURN] = reader.GetInt32(4);
        driverStats[(int)EDriverStats.BREAK] = reader.GetInt32(5);
        driverStats[(int)EDriverStats.OVERTAKE] = reader.GetInt32(6);
        driverStats[(int)EDriverStats.DEFENSE] = reader.GetInt32(7);
        driverStats[(int)EDriverStats.TYRECONTROL] = reader.GetInt32(8);

        potential = reader.GetInt32(9);

        driverStats[(int)EDriverStats.REGULARITY] = reader.GetInt32(10);
        driverStats[(int)EDriverStats.REACTIVITY] = reader.GetInt32(11);
    }

    #endregion
    #region RACE_FUNCTION
    public void CalculDayForm()
    {
        raceStat.dayForm = MathRacing.GetNumber(driverStats[(int)EDriverStats.REGULARITY]);
        int minValue,maxValue;
        RacingLibrary.SetMinMaxFormAccordingDayForm(raceStat.dayForm,out minValue,out maxValue);
        for(int indexStat = 0;indexStat < (int)EDriverStats.MAX;indexStat++)
            raceStat.driverForm[(int)indexStat] = RacingLibrary.GetRandomInt(minValue,maxValue);
    }
    public void ResetRaceVariable( )
    {
        if(raceStat.hasDNF)
            seasonStat.nbDnf++;
        
        raceStat.Reset();
    }
    private float CalculatePoint(float[] array,bool isMax = true)
    {
        float maxPoint = 0f;
        float divisor = 0f;
        float stat = 99f;
        for(int i = 0; i < array.Length; i++)
        {
            if(!isMax)
                stat = driverStats[i] + raceStat.driverForm[i];
            maxPoint += stat * array[i];
            divisor += array[i];
        }
           
        return maxPoint / divisor;
    } 
    private float CalculateMaxTurnPoint()
    {
        return CalculatePoint(driverTurnCoeff);
    }
    private float CalculateMaxLinePoint()
    {
        return  CalculatePoint(driverLineCoeff);
    }
    public static float CalculMaxPoints(int nbTurn, float length)
    {
        Driver driver = new Driver();
        float turnPoint = nbTurn * (driver.CalculateMaxTurnPoint()*turnDriverCoeff+ Team.GetMaxTurnPoint() * turnTeamCoeff) / (turnDriverCoeff+turnTeamCoeff);
        float linePoint = length* (driver.CalculateMaxLinePoint()*lineDriverCoeff + Team.GetMaxLinePoint() * lineTeamCoeff) / (lineDriverCoeff+lineTeamCoeff); 
        return (float)( turnPoint + linePoint ) / 2f;
    }

    private float CalculateTurnPoint()
    {
        return CalculatePoint(driverTurnCoeff,false);
    }
    private float CalculateLinePoint()
    {
        return CalculatePoint(driverLineCoeff,false);
    }
    public float CalculatePointPerTours(int nbTurn, float length,Team team)
    {
        float turnPoint = nbTurn * ( CalculateTurnPoint() * turnDriverCoeff + team.GetTurnPoint() * turnTeamCoeff) / (turnDriverCoeff + turnTeamCoeff);  
        float linePoint = length * (CalculateLinePoint() * lineDriverCoeff + team.GetLinePoint() * lineTeamCoeff ) / (lineDriverCoeff +lineTeamCoeff); 

        float actualTurnPoint =  (float)(turnPoint + linePoint) / 2f;
        foreach(float penality in raceStat.penaltyPoint)
            actualTurnPoint -= actualTurnPoint * penality;
        raceStat.racePoints +=actualTurnPoint;
        return actualTurnPoint;
    }


    public void TriggerEvent(EEventType EEventType)
    {
        switch(EEventType)
        {
            case EEventType.DNF:
                raceStat.hasDNF = true;
                break;
            case EEventType.TYRE_FLAT:
                raceStat.penaltyPoint[(int)EEventType.TYRE_FLAT] = 0.3f;
                break;
            case EEventType.DAMAGE_PLANKS:
                raceStat.penaltyPoint[(int)EEventType.DAMAGE_PLANKS] = 0.1f;
                break;
            case EEventType.DAMAGE_WINGS:
                raceStat.penaltyPoint[(int)EEventType.DAMAGE_WINGS] = 0.1f;
                break;
            default :
                RacingLogger.Error("Error : EEventType not recognize");
            break;
        }
    }
    #endregion

    #region CAREER_FUNCTION
    public void UpdateStats()
    {
        int update = 0;
        int age = BirthdateToAge();
        if(age>34)
            update = -1;
        else if(age <25 && GetGeneral() < potential)
            update = 1;
        for(int i = 0;i<(int)EDriverStats.MAX;i++)
            driverStats[i] += update;
        if(update != 0)
            RacingLogger.Info("Le pilote " + LastName + " a changé de notes");
    }
    public int BirthdateToAge()
    {
        if (string.IsNullOrEmpty(Birthdate))
        {
            RacingLogger.Warning($"Warning: Birthdate is null or empty for driver {GetName()}");
            return 0;
        }

        if (GameManager.instance == null)
        {
            RacingLogger.Warning($" GameManager.instance is null");
            return 0;
        }

        var dateTime = DateTime.Parse(Birthdate);
        int age = GameManager.instance.baseYear.Year - dateTime.Year;
        if(dateTime.Date > GameManager.instance.baseYear.AddYears(-age))
            age--;
        return age;
    }
    #endregion
    #region TOSTRING
    public override string ToString()
    {
        return  Id + " : " + FirstName + " " + LastName + " birth at "  +Birthdate  +" : " + seasonStat.seasonPoint;
    }
    public string ToStringSeason()
    {
        return seasonStat.ToString();
    }

    public string GetName()
    {
        return FirstName + " " + LastName;
    }
    #endregion

    public float GetGeneral()
    {
        float general = 0f;
        for (int i = 0; i < (int)EDriverStats.MAX; i++)
        {
            general += driverStats[i];
        }
        return general / (float)EDriverStats.MAX; 
    }

    public string UpdateRowString()
    {
        return  "s_turn = '" + driverStats[(int)EDriverStats.TURN] + 
                "', s_break = '" + driverStats[(int)EDriverStats.BREAK] +
                "', s_overtake = '" + driverStats[(int)EDriverStats.OVERTAKE] +
                "', s_defense = '" + driverStats[(int)EDriverStats.DEFENSE] +
                "', s_tyrecontrol = '" + driverStats[(int)EDriverStats.TYRECONTROL] +"'" ;
    }
};