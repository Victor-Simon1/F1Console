using Microsoft.Data.Sqlite;

public class Database
{

    private SqliteConnection connection;
    public void LoadDatabase(GameManager gm)
    {
        var connStringMFile = "Data Source=file:db.sqlite;Mode=ReadWrite;";
        try
        {
            using (connection = new SqliteConnection(connStringMFile))
            {
                connection.Open();
                Console.WriteLine("Database open ! ");
                Console.WriteLine(connection.State);
                LoadData<Driver>(gm.driversList);
                //LoadData<Chassis>(gm.chassisList);
                //LoadData<Motor>(gm.motorList);
                //LoadData<Team>(gm.teamsList);
                //LoadData<Race>(gm.raceList);
                //LoadData<RaceType>(gm.raceTypeList);
                //LoadData<TireBrand>(gm.tireList);
                //LoadData<TireType>(gm.tireTypeList);
                
                connection.Close();
            }
           
        }
        catch (SqliteException ex) { 
            Console.WriteLine(ex.Message);
        }
    }

   
    private void LoadData<T>(List<T> list ) where T : Component
    {
        using (var cmd = new SqliteCommand(T.READDB,connection))
        {
            using (SqliteDataReader reader = cmd.ExecuteReader())
            {
                while(reader.Read()) 
                {

                    T component = Activator.CreateInstance<T>();
                    if (component != null)
                    {
                        component.LoadData(reader); 
                        list.Add(component);
                    }
                }
            }
        }
    }
  
}