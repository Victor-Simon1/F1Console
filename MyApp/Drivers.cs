using Microsoft.Data.Sqlite;

public class  Driver : Component
{
    public static string READDB => "SELECT * FROM drivers";

    public int Id{get;set;}
    public string? FirstName{get;set;}
    public string? LastName {get;set;}
    public string? Birthdate{get;set;}
    public int IdTeam{get;set;}


    public void LoadData(SqliteDataReader reader)
    {
        Id = reader.GetInt32(0);
        FirstName = reader.GetString(1);
        LastName = reader.GetString(2);
        Birthdate = reader.GetString(3);
    }

    public override string ToString()
    {
        return  Id + " : " + FirstName + " " + LastName + " birth at "  +Birthdate ;
    }
};