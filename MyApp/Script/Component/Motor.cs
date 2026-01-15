using Microsoft.Data.Sqlite;

public class Motor : Component
{
    public static string READDB => "SELECT * FROM motors";

    public int Id{get;set;}
    public string? Name{get;set;}
    public int Power{get;set;}
    public int Fiability{get;set;}

    public void LoadData(SqliteDataReader reader)
    {
        Id = reader.GetInt32(0);
        Name = reader.GetString(1);
        Power = reader.GetInt32(2);
        Fiability = reader.GetInt32(3);
    }
    public float GetGeneral() => (Power + Fiability)/2f;
    public override string ToString()
    {
        return $"{Id}: {Name} - Power: {Power}, Fiability: {Fiability}";
    }

};