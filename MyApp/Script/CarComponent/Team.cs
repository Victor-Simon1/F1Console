using System.Text.Json.Serialization;
using Microsoft.Data.Sqlite;

public class Team : Component,IUpdatable,IRaceAble/*,Saveable*/
{
    public static string READDB => "SELECT * FROM teams";

    public int Id{get;set;}
    public string? Name{get;set;}

    public Motor? Motor{get;set;}
    public int IdMotor{get;set;}
    public Chassis? Chassis {get;set;}
    public int IdChassis{get;set;}

    public Driver[] driversList = new Driver[3];
    //public Driver? Driver1 {get;set;}
    //public int IdDriver1{get;set;}
    //public Driver? Driver2 {get;set;}
    //public int IdDriver2{get;set;}
    public EDivisionType divison;

    public void LoadData(SqliteDataReader reader)
    {
        Id = reader.GetInt32(0);
        Name = reader.GetString(1);

        //IdDriver1 = reader.GetInt32(2);
        //IdDriver2 = reader.GetInt32(3);
        IdChassis = reader.GetInt32(2);
        IdMotor = reader.GetInt32(3);
        
        divison = (EDivisionType)reader.GetInt32(4);
    }
    public void AddDriver(Driver driver)
    {
        for(int i=0;i<driversList.Length;i++)
        {
            if(driversList[i] == null)
            {
                RacingLogger.Debug($"{driver.LastName} a été link avec {Name}");
                driversList[i] = driver;
                break;
            }
        }
    }

    public void ForEachDriver()
    {
        
    }
    public void SetFromId(ref Info info)
    {
        if(Id == 0)
            return;
       /* try
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
        */
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
        int seasonPts = 0;
        for(int i=0;i<driversList.Length;i++)
        {
            if(driversList[i] != null)
            {
                seasonPts += driversList[i].seasonStat.seasonPoint ;
            }
        }

        return seasonPts;
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
        
        return Id + " : " + Name +"," + IdChassis + "," + IdMotor + ",Driver " ; 
    }

    public string UpdateRowString()
    {
        return  "idChassis = " + IdChassis +
                ", idMotor = " + IdMotor;
    }

}