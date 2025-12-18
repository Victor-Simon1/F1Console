using System.Text.Json.Serialization;
using Microsoft.Data.Sqlite;

public class  Driver : Component
{
    private enum E_DriverStats : int
    {
        TURN = 0,
        BREAK = 1,
        OVERTAKE = 2,
        DEFENSE = 3,
        TYRECONTROL = 4,
        MAX = 5
    };
    public static string READDB => "SELECT * FROM drivers";
    [JsonInclude]
    public int Id{get;set;}
    public string? FirstName{get;set;}
    public string? LastName {get;set;}
    public string? Birthdate{get;set;}

    //Statistic

    public int[] driverStats = new int[(int)E_DriverStats.MAX];
    public int[] driverForm = new int[(int)E_DriverStats.MAX];
    public static float[] driverCoeff = new float[(int)E_DriverStats.MAX];
    //Race Variable
    [JsonIgnore]
    public int dayForm;
    [JsonIgnore]
    public float racePoints;
    //Carreer Variable
    [JsonInclude]
    public int seasonPoint;

    public void LoadData(SqliteDataReader reader)
    {
        Id = reader.GetInt32(0);
        FirstName = reader.GetString(1);
        LastName = reader.GetString(2);
        Birthdate = reader.GetString(3);

        //Stats

        driverStats[(int)E_DriverStats.TURN] = reader.GetInt32(4);
        driverStats[(int)E_DriverStats.BREAK] = reader.GetInt32(5);
        driverStats[(int)E_DriverStats.OVERTAKE] = reader.GetInt32(6);
        driverStats[(int)E_DriverStats.DEFENSE] = reader.GetInt32(7);
        driverStats[(int)E_DriverStats.TYRECONTROL] = reader.GetInt32(8);
    }

    public static void SetCoeff()
    {
        driverCoeff[(int)E_DriverStats.TURN] = 1.2f;
        driverCoeff[(int)E_DriverStats.BREAK] = 1.8f;
        driverCoeff[(int)E_DriverStats.OVERTAKE] = 0.25f;
        driverCoeff[(int)E_DriverStats.DEFENSE] = 0.45f;
        driverCoeff[(int)E_DriverStats.TYRECONTROL] = 0.75f;
    }

    public void CalculDayForm()
    {
        dayForm = MyAppLibrary.GetRandomInt(-3,3);
        int minValue,maxValue;
        MyAppLibrary.SetMinMaxFormAccordingDayForm(dayForm,out minValue,out maxValue);
        driverForm[(int)E_DriverStats.TURN] = MyAppLibrary.GetRandomInt(minValue,maxValue);
        driverForm[(int)E_DriverStats.BREAK] = MyAppLibrary.GetRandomInt(minValue,maxValue);
        driverForm[(int)E_DriverStats.OVERTAKE] = MyAppLibrary.GetRandomInt(minValue,maxValue);
        driverForm[(int)E_DriverStats.DEFENSE] = MyAppLibrary.GetRandomInt(minValue,maxValue);
        driverForm[(int)E_DriverStats.TYRECONTROL] = MyAppLibrary.GetRandomInt(minValue,maxValue);
    }
    public override string ToString()
    {
        return  Id + " : " + FirstName + " " + LastName + " birth at "  +Birthdate ;
    }
    public float GetGeneral()
    {
        float general = 0f;
        for (int i = 0; i < (int)E_DriverStats.MAX; i++)
        {
            general += driverStats[i];
        }
        return general / (float)E_DriverStats.MAX; 
    }

    public static float CalculMaxPoints(int nbTurn, float length)
    {
        float random = 1.4f;
        float turnPoint = nbTurn * /*team.GetTurnPoint()* */ 
            (99f * driverCoeff[(int)E_DriverStats.TURN]+ 
            99f * driverCoeff[(int)E_DriverStats.BREAK]);
        float linePoint = (99f *  driverCoeff[(int)E_DriverStats.OVERTAKE]+ 
                99f * driverCoeff[(int)E_DriverStats.TYRECONTROL]+ 
                99f  * driverCoeff[(int)E_DriverStats.DEFENSE])  
                /*team.GetLinePoint()*/;
        return   (float)( turnPoint+linePoint )* length * random;
    }

    private float GetStat(E_DriverStats stat)
    {
        return (driverStats[(int)stat]+driverForm[(int)stat])* driverCoeff[(int)stat];
    }
    public float CalculatePointPerTours(int nbTurn, float length,Team team)
    {
        //float random = MyAppLibrary.GetRandomFloat(0.8f,1.4f);
        float turnPoint = /*nbTurn */ ((GetStat(E_DriverStats.TURN) + GetStat(E_DriverStats.BREAK)
                                /(driverCoeff[(int)E_DriverStats.TURN] + driverCoeff[(int)E_DriverStats.BREAK]))*0.75f
                                + team.GetTurnPoint() *1.5f)/2.25f  ;  
        float linePoint = /*length */ ((team.GetLinePoint() *1.5f) + 
                        ((GetStat(E_DriverStats.OVERTAKE) + GetStat(E_DriverStats.DEFENSE) + GetStat(E_DriverStats.TYRECONTROL))/
                         (driverCoeff[(int)E_DriverStats.OVERTAKE] + driverCoeff[(int)E_DriverStats.DEFENSE] + driverCoeff[(int)E_DriverStats.TYRECONTROL]))
                         *0.75f)/2.25f  ; 

        float actualTurnPoint =  (float)( turnPoint+linePoint )/2f;
        racePoints +=actualTurnPoint;
        return actualTurnPoint/* random*/;
    }
   /* public sealed class DriverData : Data
    {
       
        [JsonInclude]
        int seasonPoint{get;set;}
        public DriverData(int _id,int _seasonPoint)
        { 
            Id = _id;
            seasonPoint = _seasonPoint;
        }
        public override string ToString()
        {
            return Id + " : " + seasonPoint;
        }
    }*/
    //public Data ConvertToData()
    //{
    //    return new DriverData(Id,seasonPoint);
   // }

 
};