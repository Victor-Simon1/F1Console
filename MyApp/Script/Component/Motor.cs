using Microsoft.Data.Sqlite;

public class Motor : Component
{
    public static string READDB => throw new NotImplementedException();

    public int Id{get;set;}
    public string? Name{get;set;}
    public int Power{get;set;}
    public int Fiability{get;set;}

    public void LoadData(SqliteDataReader reader)
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        throw new NotImplementedException();
    }
};