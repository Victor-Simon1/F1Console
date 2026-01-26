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
    public List<EDivisionType> divisionRacing = new List<EDivisionType>();

    public  Race()
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

        for(int i = 0;i<(int)EDivisionType.MAX;i++)
        {
            bool isRacing = reader.GetBoolean(6+i);
            if(isRacing)
                divisionRacing.Add((EDivisionType)i);
        }
     
    }

    public override string ToString()
    {
        return Name + " : " + MaxTour + " tours";
    }
};