using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.UtilityDB.DAL.DataLayer.UtilityDB
{
    public interface IUtilityDB
    {
        [Obsolete("Usare il metodo ReadSQLVersionMajorAsync")]
        Task<int> Read_SQL_Version_YearAsync(AgronicaCoreParametri objParametri);
        Task<int> ReadSQLVersionMajorAsync(AgronicaCoreParametri objParametri); 
        Task<int> Read_SQL_Compatibility_LevelAsync(AgronicaCoreParametri objParametri);
        string GetDBName(AgronicaCoreParametri objParametri);
        Task<string> GetPivaRealeAsync(string piva, AgronicaCoreParametriServer objParametriServer);
    }
}
