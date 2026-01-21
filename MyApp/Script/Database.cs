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
           RacingLogger.Error("LoadDatabase: CurrentDatabaseName is empty");
            return;
        }

        RacingLogger.Debug("Try to open : " + CurrentDatabaseName);
        try
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                RacingLogger.Debug("Database open ! ");
                RacingLogger.Debug(connection.State.ToString());  
                LoadData<Driver>(info.driversList, connection);
                LoadData<Chassis>(info.chassisList, connection);
                LoadData<Motor>(info.motorList, connection);
                LoadData<Team>(info.teamsList, connection);
                LoadData<Race>(info.raceList, connection);
                
                LoadSeasonResult(info.driversList,connection);
                LoadCareerResult(info.driversList,connection);
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
                        RacingLogger.Debug("Team " + team.Name + " a été ajouté dans la division " + team.divison);
                    }
                }
            }
        }
        catch (SqliteException ex) 
        { 
            RacingLogger.Exception(ex, "LoadDatabase");
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
            RacingLogger.Error("GetColumnNames: CurrentTableName is empty");
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
                            RacingLogger.Warning($"No columns found for table '{CurrentTableName}'.");
                        }
                        else
                        {
                            RacingLogger.Debug($"Columns in '{CurrentTableName}':");
                            foreach (var col in columnsNameList)
                            {
                                RacingLogger.Debug($"- {col}");
                            }
                        }
                    }
                }
            }
        }
        catch (SqliteException ex)
        {
            RacingLogger.Exception(ex, "GetColumnNames");
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
                RacingLogger.Error("GetTables: Failed to retrieve tables");
                return;
            }

            foreach (DataRow row in table.Rows)
            {   
                object? value = row.ItemArray[0];
                string? valueStr = value?.ToString();
                if(!string.IsNullOrEmpty(valueStr) && !valueStr.Contains("sqlite"))
                {
                    RacingLogger.Debug(valueStr);
                    tablesNameList.Add(valueStr);
                }
            }
        }
        catch (Exception e)
        {
            RacingLogger.Exception(e, "GetTables");
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
            RacingLogger.Exception(e, "GetDataTable");
            return null;
        }
    }
    #endregion
    #region DELETE_PART
    public void DeleteRow(string table, int id)
    {
        if (string.IsNullOrEmpty(table))
        {
            RacingLogger.Error("DeleteRow: Table name is empty");
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
            RacingLogger.Exception(ex, "DeleteRow");
        }
    }
    #endregion
    #region DRIVER_RESULT
    //call when saving
    public void UpdateSeasonResult(Driver d)
    {
        try
        {
            using (var connection = GetConnection())
            {
                const string table = "drivers_current_season"; 
                int id = d.Id;
                int new_nb_victory = d.seasonStat.nb_victory;
                int new_points = d.seasonStat.seasonPoint;
                connection.Open();
                // Utilisation de paramètres SQL pour éviter les injections SQL
                using (SqliteCommand command = new SqliteCommand($"UPDATE [{table}] SET nb_victory = @new_nb_victory, points = @new_points WHERE ID = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@new_nb_victory", new_nb_victory);
                    command.Parameters.AddWithValue("@new_points", new_points);
                    command.ExecuteNonQuery();
                }
            }
        }
        catch (SystemException ex)
        {
            RacingLogger.Exception(ex, "UpdateAllTimeResult");
        }
    }
    private void LoadSeasonResult(List<Driver> list, SqliteConnection connection)
    {
        const string cmdStr = "SELECT * FROM drivers_current_season"; 
        using (var cmd = new SqliteCommand(cmdStr, connection))
        {
            using (SqliteDataReader reader = cmd.ExecuteReader())
            {
                int driver_index = 0;
                while(reader.Read()) 
                {
                    RacingLogger.Debug(list[driver_index].GetName() +"d'id " + list[driver_index].Id +":"+ reader.GetInt32(0).ToString() + " a eu sa saison remplie");
                    list[driver_index].seasonStat.Load(reader.GetInt32(1),reader.GetInt32(2),reader.GetInt32(3),reader.GetInt32(4));
                    driver_index++;
                }
            }
        }
    }
    //Call only when saving
    public void UpdateAllTimeResult(Driver d)
    {
        try
        {
            using (var connection = GetConnection())
            {
                const string table = "drivers_alltime_stats"; 
                int id = d.Id;
                int new_nb_victory = d.carrerStat.nb_victory + d.seasonStat.nb_victory;
                int new_points = d.carrerStat.points + d.seasonStat.seasonPoint;
                connection.Open();
                // Utilisation de paramètres SQL pour éviter les injections SQL
                using (SqliteCommand command = new SqliteCommand($"UPDATE [{table}] SET nb_victory = @new_nb_victory, points = @new_points WHERE ID = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@new_nb_victory", new_nb_victory);
                    command.Parameters.AddWithValue("@new_points", new_points);
                    command.ExecuteNonQuery();
                }
            }
        }
        catch (SystemException ex)
        {
            RacingLogger.Exception(ex, "UpdateAllTimeResult");
        }
    }
    private void LoadCareerResult(List<Driver> list, SqliteConnection connection)
    {
        const string cmdStr = "SELECT * FROM drivers_current_season"; 
        using (var cmd = new SqliteCommand(cmdStr, connection))
        {
            using (SqliteDataReader reader = cmd.ExecuteReader())
            {
                int driver_index = 0;
                while(reader.Read()) 
                {
                    RacingLogger.Debug(list[driver_index].GetName() +"d'id " + list[driver_index].Id +":"+ reader.GetInt32(0).ToString() + " a eu sa carriere remplie");
                    list[driver_index].carrerStat.Load(reader.GetInt32(1),reader.GetInt32(2),reader.GetInt32(3));
                    driver_index++;
                }
            }
        }
    }
    #endregion
    #region SAVE_DB

    public void UpdateRowCurrentSeasonDriverStat(Driver obj)
    {
        try
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                // Utilisation de paramètres SQL pour éviter les injections SQL
                const string table = "drivers_current_season";
                string setStr = $" nb_victory = @nb_victory , points = @seasonPoint , sumPlace = @sumPlace , nbDnf = @nbDnf ";
                string commandStr = $"UPDATE [{table}] SET {setStr} WHERE ID = @id";
                
                using (SqliteCommand command = new SqliteCommand(commandStr, connection))
                {
                    command.Parameters.AddWithValue("@id", obj.Id);
                    command.Parameters.AddWithValue("@nb_victory", obj.seasonStat.nb_victory);
                    command.Parameters.AddWithValue("@seasonPoint", obj.seasonStat.seasonPoint);
                    command.Parameters.AddWithValue("@sumPlace", obj.seasonStat.sumPlace);
                    command.Parameters.AddWithValue("@nbDnf", obj.seasonStat.nbDnf);
                    command.ExecuteNonQuery();
                }
            }
        }
        catch (SystemException ex)
        {
            RacingLogger.Exception(ex, "UpdateRow");
        }
    }
    public void UpdateRowAllTimeSeasonDriverStat(Driver obj)
    {
        try
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                // Utilisation de paramètres SQL pour éviter les injections SQL
                const string table = "drivers_alltime_stats";
                string setStr = $" nb_victory = @nb_victory , points = @points , nb_dnf = @nbDnf ";
                string commandStr = $"UPDATE [{table}] SET {setStr} WHERE ID = @id";
                
                using (SqliteCommand command = new SqliteCommand(commandStr, connection))
                {
                    command.Parameters.AddWithValue("@id", obj.Id);
                    command.Parameters.AddWithValue("@nb_victory", obj.carrerStat.nb_victory);
                    command.Parameters.AddWithValue("@points", obj.carrerStat.points);
                    command.Parameters.AddWithValue("@nbDnf", obj.carrerStat.nbDnf);
                    command.ExecuteNonQuery();
                }
            }
        }
        catch (SystemException ex)
        {
            RacingLogger.Exception(ex, "UpdateRow");
        }
    }

    public void UpdateRow<T>(T obj, string table) where T : IUpdatable, Component
    {
        if (string.IsNullOrEmpty(table))
        {
            RacingLogger.Error("UpdateRow: Table name is empty");
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
            RacingLogger.Exception(ex, "UpdateRow");
        }
    }
    #endregion
    #region INSERT_PART

    public void AddRow<T>(T obj, string table) where T : Component, IAddable
    {
        if (string.IsNullOrEmpty(table))
        {
            RacingLogger.Error("AddRow: Table name is empty");
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
            RacingLogger.Exception(ex, "AddRow");
        }
    }
    public void InsertRowInTable(string table, string columnNames, string[] values)
    {
        if (string.IsNullOrEmpty(table))
        {
            RacingLogger.Error("InsertRowInTable: Table name is empty");
            return;
        }

        if (string.IsNullOrEmpty(columnNames))
        {
            RacingLogger.Error("InsertRowInTable: Column names are empty");
            return;
        }

        if (values == null || values.Length == 0)
        {
            RacingLogger.Error("InsertRowInTable: No values provided");
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
            RacingLogger.Exception(ex, "InsertRowInTable");
        }
    }    
    #endregion
    #region SHOW_DB
    public void ShowTable(string table)
    {
        if (string.IsNullOrEmpty(table))
        {
            RacingLogger.Error("ShowTable: Table name is empty");
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
                        RacingLogger.Info("--------------------------------");
                        RacingLogger.Info(table);
                        if (!reader.HasRows)
                        {
                            RacingLogger.Warning("No rows found.");
                            return;
                        }
                        int character = 0;
                        string headerStr = "";
                        // Print column headers
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string columnHeader = reader.GetName(i);
                            Type type =  reader.GetFieldType(i);
                            if(type == typeof(System.Int64))
                                columnHeader = columnHeader.PadRight(StringRacing.PadRightDbInt);
                            else
                                columnHeader = columnHeader.PadRight(StringRacing.PadRightDbString);
                            character += columnHeader.Length;
                            headerStr += columnHeader + " "; 
                        }
                        RacingLogger.Info(headerStr);
                        RacingLogger.Info(new string('-', character));
                        // Print all rows
                        while (reader.Read())
                        {
                            string rowStr = "";
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string? value = reader[i]?.ToString();
                                Type type =  reader.GetFieldType(i);
                                if(type == typeof(System.Int64))
                                    value = value.PadRight(StringRacing.PadRightDbInt);
                                else
                                    value = value.PadRight(StringRacing.PadRightDbString);
                                rowStr += value + " ";
                                //RacingLogger.Info((value ?? string.Empty).PadRight(20));
                            }
                            RacingLogger.Info(rowStr);
                        }
                    }
                }
                RacingLogger.Info("--------------------------------");
            }
        }
        catch (SystemException ex)
        {
            RacingLogger.Exception(ex, "ShowTable");
        }
    }
    #endregion
   
}