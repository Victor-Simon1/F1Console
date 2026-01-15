using System.Reflection;

public static class RacingLibrary
{
    static Random rand = new Random();
    public static string BASEFILE = System.IO.Directory.GetCurrentDirectory();
    public static string SAVEFILE = BASEFILE + "\\Saves";
    /// <summary>
    /// Génère un nombre aléatoire flottant dans la plage [minValue, maxValue] (inclusif)
    /// </summary>
    public static float GetRandomFloat(float minValue, float maxValue)
    {
        if (minValue > maxValue)
            throw new ArgumentException("minValue must be less than or equal to maxValue.");

        double range = maxValue - minValue;
        return (float)(minValue + rand.NextDouble() * range);
    }
    
    /// <summary>
    /// Génère un nombre entier aléatoire dans la plage [minValue, maxValue[ (minValue inclusif, maxValue exclusif).
    /// Note: Pour obtenir maxValue inclusif, utilisez GetRandomInt(minValue, maxValue + 1)
    /// </summary>
    /// <param name="minValue">Valeur minimale (inclusive)</param>
    /// <param name="maxValue">Valeur maximale (exclusive)</param>
    /// <returns>Un entier aléatoire dans la plage [minValue, maxValue[</returns>
    public static int GetRandomInt(int minValue, int maxValue)
    {
        if (minValue > maxValue)
            throw new ArgumentException("minValue must be less than or equal to maxValue.");

        return rand.Next(minValue, maxValue);
    }
    
    /// <summary>
    /// Génère un nombre entier aléatoire dans la plage [minValue, maxValue] (inclusif des deux côtés).
    /// Version inclusive qui inclut maxValue dans les résultats possibles.
    /// </summary>
    /// <param name="minValue">Valeur minimale (inclusive)</param>
    /// <param name="maxValue">Valeur maximale (inclusive)</param>
    /// <returns>Un entier aléatoire dans la plage [minValue, maxValue]</returns>
    public static int GetRandomIntInclusive(int minValue, int maxValue)
    {
        if (minValue > maxValue)
            throw new ArgumentException("minValue must be less than or equal to maxValue.");

        return rand.Next(minValue, maxValue + 1);
    }

    public static void SetMinMaxFormAccordingDayForm(int dayForm,out int minValue,out int maxValue)
    {
        minValue = 0;
        maxValue= 0;
        switch(dayForm)
        {
            case -3:
                minValue = RacingLibrary.GetRandomInt(-5,-4);
                maxValue= minValue + 1;
            break;
            case -2:
                minValue = RacingLibrary.GetRandomInt(-4,-2);
                maxValue= minValue + 1;
            break;
            case -1:
                minValue = RacingLibrary.GetRandomInt(-2,0);
                maxValue= minValue + 1;
            break;
            case 0:
                minValue = RacingLibrary.GetRandomInt(-1,1);
                maxValue= minValue + 1;
            break;
            case 1:
                minValue = RacingLibrary.GetRandomInt(0,2);
                maxValue= minValue + 1;
            break;
            case 2:
                minValue = RacingLibrary.GetRandomInt(1,3);
                maxValue= minValue + 1;
            break;
            case 3:
                minValue = RacingLibrary.GetRandomInt(3,4);
                maxValue= minValue + 1;
            break;
            default:
                Console.WriteLine("Form not determined ");
            break;
        }
    }
    public static int  ConvertStringToInt(string? value)
    {
        try
        {
            int number = Convert.ToInt32(value);
            return number; 
        }
        catch(Exception e)
        {
            Console.WriteLine(e.ToString());
            return -1;
        }
    }

    public static int GetIntInput()
    {
        string? input = Console.ReadLine();
        int inputConvert = RacingLibrary.ConvertStringToInt(input);
        return inputConvert;
    }

    /// <summary>
    /// Vérifie si une valeur est dans la plage [min, max] (inclusif)
    /// </summary>
    public static bool VerifyInput(int value, int min, int max, string errorText)
    {
        if(value < min || value > max)
        {
            Console.WriteLine(errorText);
            return false;
        }
        return true;
    }

    /// <summary>
    /// Demande une entrée utilisateur et valide qu'elle est dans la plage [min, max] (inclusif).
    /// Continue à demander jusqu'à ce qu'une valeur valide soit entrée.
    /// </summary>
    /// <param name="min">Valeur minimale incluse</param>
    /// <param name="max">Valeur maximale incluse</param>
    /// <param name="errorMessage">Message d'erreur à afficher si la valeur est invalide</param>
    /// <returns>La valeur entière validée</returns>
    public static int GetValidatedIntInput(int min, int max, string errorMessage)
    {
        int input = GetIntInput();
        bool isValid = VerifyInput(input, min, max, errorMessage);
        
        while(!isValid)
        {
            input = GetIntInput();
            isValid = VerifyInput(input, min, max, errorMessage);
        }
        
        return input;
    }

    public static Driver GetDriverById(int id,ref Info info)
    {
        foreach(Driver driver in info.driversList)
        {
            if(driver.Id == id)
            {
                return driver;
            }
        }
        throw new Exception("Not driver found");
    }
    public static Motor GetMotorById(int id,ref Info info)
    {
        foreach(Motor motor in info.motorList)
        {
            if(motor.Id == id)
            {
                return motor;
            }
        }
        throw new Exception("Not motor found");
    }

    public static Chassis GetChassisById(int id,ref Info info)
    {
        foreach(Chassis chassis in info.chassisList)
        {
            if(chassis.Id == id)
            {
                return chassis;
            }
        }
        throw new Exception("Not chassis found");
    }
}