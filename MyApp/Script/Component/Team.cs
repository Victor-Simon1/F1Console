using System.Text.Json.Serialization;
using Microsoft.Data.Sqlite;

public class Team : Component/*,Saveable*/
{
    public static string READDB => "SELECT * FROM teams";

    public int Id{get;set;}
    public string? Name{get;set;}

    [JsonIgnore]
    public Motor? Motor{get;set;}
    public int IdMotor{get;set;}
    [JsonIgnore]
    public Chassis? Chassis {get;set;}
    public int IdChassis{get;set;}
    [JsonIgnore]
    public Driver? Driver1 {get;set;}
    public int IdDriver1{get;set;}
    [JsonIgnore]
    public Driver? Driver2 {get;set;}
    public int IdDriver2{get;set;}

    public void LoadData(SqliteDataReader reader)
    {
        Id = reader.GetInt32(0);
        Name = reader.GetString(1);

        IdDriver1 = reader.GetInt32(2);
        IdDriver2 = reader.GetInt32(3);
        IdChassis = reader.GetInt32(4);
        IdMotor = reader.GetInt32(5);
        
        SetFromId();
    }

    public void SetFromId()
    {
        Driver1 = MyAppLibrary.GetDriverById(IdDriver1);
        Driver2 = MyAppLibrary.GetDriverById(IdDriver2);
        Chassis = MyAppLibrary.GetChassisById(IdChassis);
        Motor = MyAppLibrary.GetMotorById(IdMotor);
    }
    public float GetGeneral()
    {
        return (Motor.GetGeneral() + Chassis.GetGeneral())/2f; 
    }
    public float GetTurnPoint()
    {
        return (Chassis.Stability*1.25f + Chassis.Aero * 0.25f)/1.5f; 
    }
    public float GetLinePoint()
    {
        return (Motor.Power*1.25f + Chassis.Aero * 1.25f)/2.5f; 
    }
    public override string ToString()
    {
        return Id + " : " + Name +"," + Chassis.Id + "," + Motor.Id; 
    }

}