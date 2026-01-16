public interface IRaceAble
{
    float CalculateMaxTurnPoint() => 99f;
    float CalculateMaxLinePoint() => 99f;
    float CalculateTurnPoint();
    float CalculateLinePoint();
    
    public float CalculatePoint(float[] arrayCoeef,int[] arrayStat)
    {
        float maxPoint = 0f;
        float divisor = 0f;
        for(int i = 0; i < arrayCoeef.Length; i++)
        {
            maxPoint += arrayStat[i] * arrayCoeef[i];
            divisor += arrayCoeef[i];
        }
           
        return maxPoint / divisor;
    } 
  
 
   



}