using System.Data;
using Microsoft.Data.Sqlite;

public class Database
{
    public Info? info;
    public List<string> tablesNameList = new List<string>();
    public List<string> columnsNameList = new List<string>();

    public string CurrentDatabaseName = System.String.Empty;
    public string CurrentTableName = System.String.Empty;

    /// <summary>
    /// Crée et retourne une nouvelle connexion SQLite pour la base de données actuelle
    /// </summary>
    private SqliteConnection GetConnection()
    {
        string connString = @"Data Source=file:" + CurrentDatabaseName + ";Mode=ReadWrite;";
        return new SqliteConnection(connString);
    }

    public void ClearDatabase(ref Info info)
    {
        info.driversList.Clear();
        info.teamsList.Clear();
        info.raceList.Clear();
        info.motorList.Clear();
        info.chassisList.Clear();
    }
    #region LOAD_DB
    public void LoadDatabase()
    {
        if(info == null)
            return ;
        
        if (string.IsNullOrEmpty(CurrentDatabaseName))
        {
            Console.WriteLine("LoadDatabase: CurrentDatabaseName is empty");
            return;
        }

        Console.WriteLine("Try to open : " + CurrentDatabaseName);
        try
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                Console.WriteLine("Database open ! ");
                Console.WriteLine(connection.State);  
                LoadData<Driver>(info.driversList, connection);
                LoadData<Chassis>(info.chassisList, connection);
                LoadData<Motor>(info.motorList, connection);
                LoadData<Team>(info.teamsList, connection);
                LoadData<Race>(info.raceList, connection);
                
                // Initialiser le dictionnaire des équipes par division
                if (!info.dictionnaryTeam.ContainsKey(EDivisionType.Formula1))
                    info.dictionnaryTeam.Add(EDivisionType.Formula1, new List<Team>());
                if (!info.dictionnaryTeam.ContainsKey(EDivisionType.Formula2))
                    info.dictionnaryTeam.Add(EDivisionType.Formula2, new List<Team>());
                if (!info.dictionnaryTeam.ContainsKey(EDivisionType.Formula3))
                    info.dictionnaryTeam.Add(EDivisionType.Formula3, new List<Team>());
                if (!info.dictionnaryTeam.ContainsKey(EDivisionType.FormulaE))
                    info.dictionnaryTeam.Add(EDivisionType.FormulaE, new List<Team>());
                if (!info.dictionnaryTeam.ContainsKey(EDivisionType.F1Academy))
                    info.dictionnaryTeam.Add(EDivisionType.F1Academy, new List<Team>());
                
                foreach(Team team in info.teamsList)
                {
                    if (info.dictionnaryTeam.ContainsKey(team.divison))
                    {
                        info.dictionnaryTeam[team.divison].Add(team);
                        Console.WriteLine("Team " + team.Name + " a été ajouté dans la division " + team.divison);
                    }
                }
            }
        }
        catch (SqliteException ex) 
        { 
            Console.WriteLine("LoadDatabase error: " + ex.Message);
        }
    }

   
    private void LoadData<T>(List<T> list, SqliteConnection connection) where T : Component
    {
        using (var cmd = new SqliteCommand(T.READDB, connection))
        {
            using (SqliteDataReader reader = cmd.ExecuteReader())
            {
                while(reader.Read()) 
                {
                    T? component = Activator.CreateInstance<T>();
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
        if (string.IsNullOrEmpty(CurrentTableName))
        {
            Console.WriteLine("GetColumnNames: CurrentTableName is empty");
            return;
        }

        columnsNameList.Clear();
        try
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                // Utilisation de brackets pour sécuriser le nom de table
                using (var cmd = new SqliteCommand($"PRAGMA table_info([{CurrentTableName}]);", connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Column name is in the "name" field
                            string? columnName = reader["name"]?.ToString();
                            if (!string.IsNullOrEmpty(columnName))
                            {
                                columnsNameList.Add(columnName);
                            }
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
            }
        }
        catch (SqliteException ex)
        {
            Console.WriteLine("GetColumnNames error: " + ex.Message);
        }
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
            DataTable? table = GetDataTable(query);
            if (table == null)
            {
                Console.WriteLine("GetTables: Failed to retrieve tables");
                return;
            }

            foreach (DataRow row in table.Rows)
            {   
                object? value = row.ItemArray[0];
                string? valueStr = value?.ToString();
                if(!string.IsNullOrEmpty(valueStr) && !valueStr.Contains("sqlite"))
                {
                    Console.WriteLine(valueStr);
                    tablesNameList.Add(valueStr);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("GetTables error: " + e.Message);
        }
    }
    public DataTable? GetDataTable(string sql)
    {
        try
        {
            DataTable dt = new();
            using (var connection = GetConnection())
            {
                connection.Open();
                using (SqliteCommand cmd = new SqliteCommand(sql, connection))
                {
                    using (SqliteDataReader rdr = cmd.ExecuteReader())
                    {
                        dt.Load(rdr);
                        return dt;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("GetDataTable error: " + e.Message);
            return null;
        }
    }
    #endregion
    #region DELETE_PART
    public void DeleteRow(string table, int id)
    {
        if (string.IsNullOrEmpty(table))
        {
            Console.WriteLine("DeleteRow: Table name is empty");
            return;
        }

        try
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                // Utilisation de paramètres SQL pour éviter les injections SQL
                using (SqliteCommand command = new SqliteCommand($"DELETE FROM [{table}] WHERE ID = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
        catch (SystemException ex)
        {
            Console.WriteLine("DeleteRow error: " + ex.Message);
        }
    }
    #endregion

    #region SAVE_DB

    public void UpdateRow<T>(T obj, string table) where T : IUpdatable, Component
    {
        if (string.IsNullOrEmpty(table))
        {
            Console.WriteLine("UpdateRow: Table name is empty");
            return;
        }

        try
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                // Utilisation de paramètres SQL pour éviter les injections SQL
                string commandStr = $"UPDATE [{table}] SET {obj.UpdateRowString()} WHERE ID = @id";
                
                using (SqliteCommand command = new SqliteCommand(commandStr, connection))
                {
                    command.Parameters.AddWithValue("@id", obj.Id);
                    command.ExecuteNonQuery();
                }
            }
        }
        catch (SystemException ex)
        {
            Console.WriteLine("UpdateRow error: " + ex.Message);
        }
    }
    #endregion
    #region INSERT_PART

    public void AddRow<T>(T obj, string table) where T : Component, IAddable
    {
        if (string.IsNullOrEmpty(table))
        {
            Console.WriteLine("AddRow: Table name is empty");
            return;
        }

        try
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                // Construire la requête avec des paramètres SQL pour éviter les injections SQL
                // Note: AddRowString() devrait retourner les colonnes et valeurs formatées
                string sql = $"INSERT INTO [{table}] {obj.AddRowString()}";
                
                using (SqliteCommand command = new SqliteCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
        catch (SystemException ex)
        {
            Console.WriteLine("AddRow error: " + ex.Message);
        }
    }
    public void InsertRowInTable(string table, string columnNames, string[] values)
    {
        if (string.IsNullOrEmpty(table))
        {
            Console.WriteLine("InsertRowInTable: Table name is empty");
            return;
        }

        if (string.IsNullOrEmpty(columnNames))
        {
            Console.WriteLine("InsertRowInTable: Column names are empty");
            return;
        }

        if (values == null || values.Length == 0)
        {
            Console.WriteLine("InsertRowInTable: No values provided");
            return;
        }

        try
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                // Construire la requête avec des paramètres SQL
                string placeholders = string.Join(", ", values.Select((_, i) => $"@value{i}"));
                string sql = $"INSERT INTO [{table}] ({columnNames}) VALUES ({placeholders})";
                
                using (SqliteCommand command = new SqliteCommand(sql, connection))
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        object paramValue = string.IsNullOrEmpty(values[i]) ? DBNull.Value : (object)values[i];
                        command.Parameters.AddWithValue($"@value{i}", paramValue);
                    }
                    command.ExecuteNonQuery();
                }
            }
        }
        catch (SqliteException ex)
        {
            Console.WriteLine("InsertRowInTable error: " + ex.Message);
        }
    }    
    #endregion
    #region SHOW_DB
    public void ShowTable(string table)
    {
        if (string.IsNullOrEmpty(table))
        {
            Console.WriteLine("ShowTable: Table name is empty");
            return;
        }

        try
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                // Utilisation de brackets pour sécuriser le nom de table
                using (SqliteCommand command = new SqliteCommand($"SELECT * FROM [{table}]", connection))
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
                        Console.WriteLine("\n" + new string('-', character));
                        // Print all rows
                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string? value = reader[i]?.ToString();
                                Console.Write((value ?? string.Empty).PadRight(20));
                            }
                            Console.WriteLine();
                        }
                    }
                }
                Console.WriteLine("--------------------------------");
            }
        }
        catch (SystemException ex)
        {
            Console.WriteLine("ShowTable error: " + ex.Message);
        }
    }
    #endregion
   
}