// See https://aka.ms/new-console-template for more information
using Microsoft.Data.Sqlite;

//Console.WriteLine("Hello, World!");

class Program
{
    static void Main()
    {
        GameManager gameManager = new();
        gameManager.GameLoop();
       /* var connStringMem = "Data Source=:memory";
        var connStringMFile = "Data Source=file:db.sqlite;Mode=ReadWrite;";

        try
        {
            using (var connection = new SqliteConnection(connStringMFile))
            {
                connection.Open();
                Console.WriteLine("Database open ! ");
                Console.WriteLine(connection.State);

                using (var cmd = new SqliteCommand("SELECT * FROM drivers",connection))
                {
                    using (SqliteDataReader  reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string FirstName = reader.GetString(1);
                            string LastName = reader.GetString(2);
                            Console.WriteLine(id+ " " + FirstName + " " +LastName);
                        }
                    }
                }
                
                
                connection.Close();
            }
           
        }
        catch (SqliteException ex) { 
            Console.WriteLine(ex.Message);
        }*/
    }
}