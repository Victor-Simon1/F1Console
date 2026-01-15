using System.Text.Json;
using System.Text.Json.Serialization;
public class SaveSystem
{
    public class Save
    {
        //[JsonInclude]
        //public List<Driver> drivers = new List<Driver>();
        //[JsonInclude]
        //public List<Team> team = new List<Team>();
        [JsonInclude]
        public int actualRace;
        [JsonInclude]
        public int year;
        [JsonInclude]
        public int pointSystemUse;
        public string ToString()
        {
            string str = "";
            str += actualRace+"\n";
            str += year;
            return str;
        }
    }
    //public static Save save;
    private static string json;

    public static void SaveListToJson(CarrerHandler carrerHandler,string path = @"D:\path.json")
    {
        Save save = new Save();

        save.actualRace = carrerHandler.indexRace;
        save.year = carrerHandler.year;
        save.pointSystemUse = (int)carrerHandler.pointSystem.actualPointSystem;
        var options = new JsonSerializerOptions { WriteIndented = true };
        
        json = JsonSerializer.Serialize<Save>(save,options);
        File.WriteAllText(path, json);
    }
    public static Save LoadFromJson(ref Info info , string path = @"D:\path.json")
    {
        Console.WriteLine("Load" + path);

        string json = File.ReadAllText(path);
        Save save = JsonSerializer.Deserialize<Save>(json);
        return save;
    }

}