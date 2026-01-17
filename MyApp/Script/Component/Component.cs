using Microsoft.Data.Sqlite;

public interface Component
{
    public int Id{get;set;}
    public static abstract string READDB{get;}
    public void LoadData(SqliteDataReader reader);

}