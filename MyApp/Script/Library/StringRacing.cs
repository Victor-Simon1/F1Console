public static class StringRacing
{
    public const int PadRightIndex = 3;
    public const int PadRightSeason = 10;
    public const int PadRightNameTeam = 50;
    public const int PadRightPointTeam = 10;
    public const int PadRightNameDriver = 30;
//Notes
    public const int PadRightGeneralAndPotential = 5;

//Database
    public const int PadRightDbString = 40;
    public const int PadRightDbInt = 10;

    public const string Separator = " | ";


    public static string CenterString(string strBase)
    {
        int baseLenght = PadRightIndex + PadRightNameDriver + 3 * PadRightSeason; 
        string newString = strBase ;
        int StrLength = baseLenght + newString.Length;
        newString = newString.PadLeft(StrLength/2).PadRight(StrLength/2);
        return newString;
    }
}