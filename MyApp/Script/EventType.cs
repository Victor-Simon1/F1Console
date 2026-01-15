public enum EEventType
{
    DNF = 0,
    TYRE_FLAT,
    DAMAGE_WINGS,
    DAMAGE_PLANKS,
    MAX_EVENT
};

public static class EventExtensions
{
    // Probabilités en pourcentage (0-100) pour chaque événement par tour
    static float[] probaEvent = new float[(int)EEventType.MAX_EVENT];
    
    public static void InitEvent()
    {
        // Probabilités ajustées en pourcentage (0-100)
        probaEvent[(int)EEventType.DNF] = 0.2f;           // 0.2% de chance de DNF par tour
        probaEvent[(int)EEventType.TYRE_FLAT] = 0.15f;    // 0.15% de chance de crevaison par tour
        probaEvent[(int)EEventType.DAMAGE_WINGS] = 0.3f;  // 0.3% de chance de dommage aileron par tour
        probaEvent[(int)EEventType.DAMAGE_PLANKS] = 0.4f; // 0.4% de chance de dommage planche par tour
    }
    
    /// <summary>
    /// Déclenche un événement aléatoire pour un pilote selon les probabilités définies.
    /// Un seul événement peut se déclencher par appel (le plus grave en priorité).
    /// </summary>
    public static void TriggerEvent(Driver driver)
    {
        if(driver.raceStat.hasDNF)
            return;
        for(int i= 0;i<(int)EEventType.MAX_EVENT;i++)
        {
            float randomProba = RacingLibrary.GetRandomFloat(0f,100f);
            if(randomProba < probaEvent[i])
                driver.TriggerEvent((EEventType)i);
        }
       
    }
}