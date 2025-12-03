
public class CarrerHandler
{
    private enum EGMCarrerActions
    {
        NEW = 0,
        LOAD,
        RETURN
    };
    public void SelectMode()
    {
        string? input = Console.ReadLine();
        int inputConvert = MyAppLibrary.ConvertStringToInt(input);
        switch(inputConvert)
        {
            case (int)EGMCarrerActions.NEW:
                Console.WriteLine("TODO ");
                break;
            case (int)EGMCarrerActions.LOAD:
                Console.WriteLine("TODO ");
                break;
            case (int)EGMCarrerActions.RETURN:
                Console.WriteLine("TODO ");
                break;
            default:
                Console.WriteLine("Entry not recognize ! Please enter a number in a relation with the different options ! ");
                break;
        }
        Console.WriteLine("TODO ");
    }

    public void Start()
    {
        throw new NotImplementedException();
    }
}