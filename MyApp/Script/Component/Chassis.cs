using Microsoft.Data.Sqlite;

public class  Chassis : Component
{
    public static string READDB => "SELECT * FROM chassis";

    public int Id{get;set;}
    public string? Name{get;set;}
    public int Aero{get;set;} 
    public int Stability{get;set;}
    public void LoadData(SqliteDataReader reader)
    {
        Id = reader.GetInt32(0);
        Name = reader.GetString(1);
        Aero = reader.GetInt32(2);
        Stability = reader.GetInt32(3);
    }
    public float GetGeneral() => (Aero + Stability)/2f;
    public override string ToString()
    {
        return $"{Id}: {Name} - Aero: {Aero}, Stability: {Stability}";
    }
};