using Microsoft.Data.Sqlite;

public interface Component
{
    public static abstract string READDB{get;}
    public void LoadData(SqliteDataReader reader);

   /* protected void InsertData();
    public static void CollectData()
    {
        
    }*/
    //override string ToString();
}