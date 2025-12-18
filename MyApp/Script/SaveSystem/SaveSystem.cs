using System.Text.Json;
using System.Text.Json.Serialization;
public class SaveSystem
{
    public class Save
    {
        [JsonInclude]
        public List<Driver> drivers = new List<Driver>();
        [JsonInclude]
        public List<Team> team = new List<Team>();


        public string ToString()
        {
            string str = "";
            foreach(Driver d in drivers)
                str += d.ToString() + "\n";
            foreach(Team t in team)
                str += t.ToString() + "\n";
            return str;
        }
    }
    public static Save save;
    private static string json;

    public static void SaveListToJson(string path = @"D:\path.json")
    {
        save = new Save();

        foreach(Driver driver in GameManager.instance.driversList)
            save.drivers.Add(driver);
        foreach(Team driver in GameManager.instance.teamsList)
            save.team.Add(driver);

        var options = new JsonSerializerOptions { WriteIndented = true };
        
        json = JsonSerializer.Serialize<Save>(save,options);
        File.WriteAllText(path, json);

        Console.WriteLine(save.drivers.Count + " "  + json);
    }
    public static void LoadFromJson(string path = @"D:\path.json")
    {
        string json = File.ReadAllText(path);
        Save save = JsonSerializer.Deserialize<Save>(json);
        foreach(Team t in save.team)
            t.SetFromId();
        Console.WriteLine("Load" + save.ToString());
    }

}