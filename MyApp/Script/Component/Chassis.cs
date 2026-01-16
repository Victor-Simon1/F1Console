using Microsoft.Data.Sqlite;

public class  Chassis : Component,IRaceAble
{
    private enum EChassisStats
    {
        Aero = 0,
        Stability,
        MAX
    };
    public static string READDB => "SELECT * FROM chassis";

    public int Id{get;set;}
    public string? Name{get;set;}
    private int[] statistics = new int[(int)EChassisStats.MAX];
    private float[] turnCoeff = {0.25f,1.25f};
    private float[] lineCoeff = {2.25f,.75f};
    public void LoadData(SqliteDataReader reader)
    {
        Id = reader.GetInt32(0);
        Name = reader.GetString(1);
        for(int i = 0; i < statistics.Length;i++)
            statistics[i] = reader.GetInt32(i+2);

    }
    public float GetGeneral()
    {
        int sum = 0;
        for (int i = 0;i< (int)EChassisStats.MAX;i++)
        {
            sum += statistics[i];
        }
        return (float) sum / (float)EChassisStats.MAX;
    }
    
    public override string ToString()
    {
        string statStr = $"";
        for (int i = 0;i< (int)EChassisStats.MAX;i++)
        {
            statStr += $"{(EChassisStats)i}: {statistics[i]},";
        }
        return $"{Id}: {Name} - " + statStr;
    }

    public float CalculateTurnPoint()
    {
        IRaceAble raceAble = this;
        return raceAble.CalculatePoint(turnCoeff,statistics);
    }

    public float CalculateLinePoint()
    {
        IRaceAble raceAble = this;
        return raceAble.CalculatePoint(lineCoeff,statistics);
    }
};