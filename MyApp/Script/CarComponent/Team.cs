using System.Text.Json.Serialization;
using Microsoft.Data.Sqlite;

public class Team : Component,IUpdatable,IRaceAble/*,Saveable*/
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
    public EDivisionType divison;

    public void LoadData(SqliteDataReader reader)
    {
        Id = reader.GetInt32(0);
        Name = reader.GetString(1);

        IdDriver1 = reader.GetInt32(2);
        IdDriver2 = reader.GetInt32(3);
        IdChassis = reader.GetInt32(4);
        IdMotor = reader.GetInt32(5);
        
        divison = (EDivisionType)reader.GetInt32(6);
        //SetFromId(ref info);
    }

    public void SetFromId(ref Info info)
    {
        try
        {
            Driver1 = RacingLibrary.GetDriverById(IdDriver1, ref info);
        }
        catch (Exception ex)
        {
            RacingLogger.Exception(ex, $"Warning: Driver1 (ID: {IdDriver1}) not found for team {Name}");
            Driver1 = null;
        }

        try
        {
            Driver2 = RacingLibrary.GetDriverById(IdDriver2, ref info);
        }
        catch (Exception ex)
        {
            RacingLogger.Exception(ex, $"Warning: Driver2 (ID: {IdDriver2}) not found for team {Name}");
            Driver2 = null;
        }

        try
        {
            Chassis = RacingLibrary.GetChassisById(IdChassis, ref info);
        }
        catch (Exception ex)
        {
            RacingLogger.Exception(ex, $"Warning: Chassis (ID: {IdChassis}) not found for team {Name}");
            Chassis = null;
        }

        try
        {
            Motor = RacingLibrary.GetMotorById(IdMotor, ref info);
        }
        catch (Exception ex)
        {
            RacingLogger.Exception(ex, $"Warning: Motor (ID: {IdMotor}) not found for team {Name}");
            Motor = null;
        }
    }
    public float GetGeneral()
    {
        if(Motor != null && Chassis != null)
            return (Motor.GetGeneral() + Chassis.GetGeneral())/2f; 
        return 0f;
        
    }
    public int GetSeasonPoint()
    {
        if(Driver1 == null || Driver2 == null)
        {
             RacingLogger.Error("Drivers are null");
             return 0;
        }
        return Driver1.seasonStat.seasonPoint + Driver2.seasonStat.seasonPoint;
    }


    public float CalculateTurnPoint()
    {
        if(Chassis == null)
        {
            RacingLogger.Error($"Chassis of team {Name} is null");
            return 0f;
        }
        if(Motor == null)
        {
            RacingLogger.Error($"Motor of team {Name} is null");
            return 0f;
        }
        return (Chassis.CalculateTurnPoint() + Motor.CalculateTurnPoint()) / 2f;
    }
    public float CalculateLinePoint()
    {
        if(Chassis == null)
        {
            RacingLogger.Error($"Chassis of team {Name} is null");
            return 0f;
        }
        if(Motor == null)
        {
            RacingLogger.Error($"Motor of team {Name} is null");
            return 0f;
        }
        return (Chassis.CalculateLinePoint() + Motor.CalculateLinePoint()) / 2f;
    }

    public override string ToString()
    {
        return Id + " : " + Name +"," + IdChassis + "," + IdMotor + ",Driver " + IdDriver1 + "/"+IdDriver2; 
    }

    public string UpdateRowString()
    {
        return  "idPilot1 = " + IdDriver1 +
                ", idPilot2 = " + IdDriver2 +
                ", idChassis = " + IdChassis +
                ", idMotor = " + IdMotor;
    }

}