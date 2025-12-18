using Microsoft.Data.Sqlite;

public class  Race : Component
{
    public static string READDB => "SELECT * FROM grandprix";

    public int Id{get;set;}
    public string? Name{get;set;}
    public string? Location{get;set;}
    public float Length{get;set;}
    public int NbTurn{get;set;}
    public int MaxTour{get;set;}
    public int IdType{get;set;}


    public void StartRace()
    {
        
    }
    public void LoadData(SqliteDataReader reader)
    {
        Id = reader.GetInt32(0);
        Name = reader.GetString(1);
        Location = reader.GetString(2);

        //Stats
        Length = reader.GetFloat(3);
        NbTurn = reader.GetInt32(4);
        MaxTour = reader.GetInt32(5);

    }

    public override string ToString()
    {
        return Name + " : " + MaxTour + " tours";
    }
};