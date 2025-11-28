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
}