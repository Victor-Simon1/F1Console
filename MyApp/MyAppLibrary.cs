public static class MyAppLibrary
{
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
        int inputConvert = MyAppLibrary.ConvertStringToInt(input);
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
}