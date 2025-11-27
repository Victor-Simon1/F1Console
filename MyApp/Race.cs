using Microsoft.Data.Sqlite;

public class  Race : Component
{
    public static string READDB => throw new NotImplementedException();

    public int Id{get;set;}
    public string? Name{get;set;}
    public string? Location{get;set;}
    public float Length{get;set;}
    public int IdType{get;set;}

    public void LoadData(SqliteDataReader reader)
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        throw new NotImplementedException();
    }
};