using System.Data;
using Microsoft.Data.Sqlite;

public class Database
{

    private SqliteConnection connection;
    public List<string> tablesNameList = new List<string>();
    public List<string> columnsNameList = new List<string>();

    public string CurrentDatabaseName = System.String.Empty;
    public string CurrentTableName = System.String.Empty;
    public void LoadDatabase()
    {
        string connStringMFile = "Data Source=file:"+CurrentDatabaseName+";Mode=ReadWrite;";
        Console.WriteLine("Try to open : " + connStringMFile);
        try
        {
            using (connection = new SqliteConnection(connStringMFile))
            {
                connection.Open();
                Console.WriteLine("Database open ! ");
                Console.WriteLine(connection.State);  
                LoadData<Driver>(GameManager.instance.driversList);
                //LoadData<Chassis>(GameManager.instance.chassisList);
                //LoadData<Motor>(GameManager.instance.motorList);
                //LoadData<Team>(GameManager.instance.teamsList);
                //LoadData<Race>(GameManager.instance.raceList);
                //LoadData<RaceType>(GameManager.instance.raceTypeList);
                //LoadData<TireBrand>(GameManager.instance.tireList);
                //LoadData<TireType>(GameManager.instance.tireTypeList);
                
                connection.Close();
            }
           
        }
        catch (SqliteException ex) { 
            Console.WriteLine("Try to open : " + connStringMFile);
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
    public void GetColumnNames()
    {
        connection.Open();
        using (var cmd = new SqliteCommand($"PRAGMA table_info({CurrentTableName});", connection))
        {
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    // Column name is in the "name" field
                    columnsNameList.Add(reader["name"].ToString());
                }

                if (columnsNameList.Count == 0)
                {
                    Console.WriteLine($"No columns found for table '{CurrentTableName}'.");
                }
                else
                {
                    Console.WriteLine($"Columns in '{CurrentTableName}':");
                    foreach (var col in columnsNameList)
                    {
                        Console.WriteLine($"- {col}");
                    }
                }
            }
        }
        connection.Close();
    }
    public void GetTables()
    {
        // executes query that select names of all tables in master table of the database
        String query = "SELECT name FROM sqlite_master " +
                        "WHERE type = 'table'" +
                        "ORDER BY 1";
        tablesNameList.Clear();
        try
        {
            DataTable table = GetDataTable(query);
            foreach (DataRow row in table.Rows)
            {   
                Console.WriteLine("Row");
                object? value = row.ItemArray[0];
                if(value != null && !value.ToString().Contains("sqlite"))
                {
                    Console.WriteLine(value.ToString());
                    tablesNameList.Add(value.ToString());
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("GetTables " + e.Message);
        }
    }
     public DataTable GetDataTable(string sql)
    {
        Console.WriteLine("DATA TABLE");
        try
        {
            DataTable dt = new DataTable();
           Console.WriteLine("DATA TABLE2");
                connection.Open();
                Console.WriteLine("OPEN");
                using (SqliteCommand cmd = new SqliteCommand(sql, connection))
                {
                    Console.WriteLine("COMMANDE");
                    using (SqliteDataReader rdr = cmd.ExecuteReader())
                    {
                        Console.WriteLine("RADE");
                        dt.Load(rdr);
                        connection.Close();
                        return dt;
                    }
                }
                
            
        }
        catch (Exception e)
        {
            Console.WriteLine("GetDataTable  " +  e.Message);
            return null;
        }
    }
    public void DeleteRow(string table/*, string columnName*/, string IDNumber)
    {
        try
        {
            connection.Open();
            string columnName = "ID";
            using (SqliteCommand command = new SqliteCommand("DELETE FROM " + table + " WHERE " + columnName + " = '" + IDNumber+"'", connection))
            {
                command.ExecuteNonQuery();
            }
            connection.Close();
    
        }
        catch (SystemException ex)
        {
            //MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
        }
    }

    public void ShowTable(string table)
    {
        try
        {
            connection.Open();
            using (SqliteCommand command = new SqliteCommand("SELECT * FROM " + table , connection))
            {
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    Console.WriteLine("--------------------------------");
                    Console.WriteLine(table);
                    if (!reader.HasRows)
                    {
                        Console.WriteLine("No rows found.");
                        return;
                    }
                    int character = 0;
                    // Print column headers
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string columnHeader = reader.GetName(i).PadRight(20);
                        character += columnHeader.Length;
                        Console.Write(columnHeader);
                    }
                   // Console.WriteLine(character);
                    Console.WriteLine("\n" + new string('-', character));
                    // Print all rows
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.Write(reader[i]?.ToString().PadRight(20) );
                        }
                        Console.WriteLine();
                    }
                }
            }
            Console.WriteLine("--------------------------------");
            connection.Close();
        }
        catch (SystemException ex)
        {
            //MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
        }
    }
    public void InsertRowInTable(string table,string columName, string row)
    {
        connection.Open();
        row = row.Replace(",", "\",\"");
        row = row.Replace("(", "(\"");
        row = row.Replace(")", "\")");
        string sql = "INSERT INTO "+ table +  " " + columName + " VALUES " + row;
        Console.WriteLine("Line : " +sql);
        try
        {
            SqliteCommand command = new SqliteCommand(sql, connection);
            command.ExecuteNonQuery();

        }
        catch (SqliteException ex) { 
            Console.WriteLine(ex.Message);
        }
      

        connection.Close();
    }
           
}