using Microsoft.Data.Sqlite;

public class  RaceType : Component
{
    public static string READDB => throw new NotImplementedException();

    public int Id{get;set;}
    public string? Name{get;set;}

    public void LoadData(SqliteDataReader reader)
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        throw new NotImplementedException();
    }
};