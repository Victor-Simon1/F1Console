using Microsoft.Data.Sqlite;

public class Motor : Component,IRaceAble
{
    private enum EMotorStats
    {
        Power = 0,
        Fiability,
        Acceleration,
        MAX
    };
    public static string READDB => "SELECT * FROM motors";

    public int Id{get;set;}
    public string? Name{get;set;}
    private int[] statistics = new int[(int)EMotorStats.MAX];
    private float[] turnCoeff = {1f,0f};
    private float[] lineCoeff = {1f,0f};
    public Motor(){}
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
        for (int i = 0;i< (int)EMotorStats.MAX;i++)
        {
            sum += statistics[i];
        }
        return (float) sum / (float)EMotorStats.MAX;
    }
    public override string ToString()
    {
        string statStr = $"";
        for (int i = 0;i< (int)EMotorStats.MAX;i++)
        {
            statStr += $"{(EMotorStats)i}: {statistics[i]},";
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
}