public class PointSystem
{

    public enum EPointSystem
    {
        F1,
        MOTOGP,
        INDYCAR,
        SUPERCAR
    }

    public EPointSystem actualPointSystem;

    // Utilisation d'un dictionnaire pour un accès O(1) au lieu d'un switch O(n)
    private static readonly Dictionary<EPointSystem, int[]> PointSystemArrays = new Dictionary<EPointSystem, int[]>
    {
        { EPointSystem.F1, new int[] {25,18,15,12,10,8,6,4,2,1} },
        { EPointSystem.MOTOGP, new int[] {25,20,15,13,11,10,9,8,7,6,5,4,3,2,1} },
        { EPointSystem.INDYCAR, new int[] {50,40,35,32,28,26,24,22,20,18,16,14,12,10,8,6,4,2} },
        { EPointSystem.SUPERCAR, new int[] {75,69,64,60,55,51,48,45,42,39,36,34,33,31,30,28,27,25,24,22,21,19,18,16,15,13,12,10,9,7} }
    };

    public void ApplyPointToDriver(ref List<Team> team)
    {
        if (PointSystemArrays.TryGetValue(actualPointSystem, out int[]? pointArray))
        {
            ApplyPoint(ref team, pointArray);
        }
        else
        {
            Console.WriteLine($"Warning: Point system '{actualPointSystem}' not found, no points applied.");
        }
    }

    public void ApplyPoint(ref List<Team> team,int[] pointArray)
    {
        List<Driver> drivers = new List<Driver>();
        foreach(Team t in team)
        {
            if(t.Driver1 != null)
                drivers.Add(t.Driver1);
            if(t.Driver2 != null)
                drivers.Add(t.Driver2);
        }
        drivers.Sort((x,y) => y.raceStat.racePoints.CompareTo(x.raceStat.racePoints));
        for(int indexPoint = 0;indexPoint < pointArray.Length;indexPoint++)
            if(indexPoint < drivers.Count )
                drivers[indexPoint].seasonStat.seasonPoint += pointArray[indexPoint];
    }
}

