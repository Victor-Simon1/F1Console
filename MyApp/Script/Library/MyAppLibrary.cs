using System.Reflection;

public static class RacingLibrary
{
    static Random rand = new Random();
    public static string BASEFILE = System.IO.Directory.GetCurrentDirectory();
    public static string SAVEFILE = BASEFILE + "\\Saves";
    public static float GetRandomFloat(float minValue, float maxValue)
    {
        if (minValue > maxValue)
            throw new ArgumentException("minValue must be less than or equal to maxValue.");

        double range = maxValue - minValue;
        return (float)(minValue + rand.NextDouble() * range);
    }
    public static int GetRandomInt(int minValue, int maxValue)
    {
        if (minValue > maxValue)
            throw new ArgumentException("minValue must be less than or equal to maxValue.");

        return  rand.Next(minValue,maxValue);
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

    public static bool VerifyInput(int value,int min, int max, string errorText)
    {
        if(value < 0 || value > max )
        {
            Console.WriteLine(errorText);
            return false;
        }
        return true;
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