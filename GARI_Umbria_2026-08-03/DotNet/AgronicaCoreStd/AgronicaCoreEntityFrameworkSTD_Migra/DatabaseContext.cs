using AgronicaCoreEntityFrameworkSTD_POCO.Models;
using Microsoft.EntityFrameworkCore;

namespace AgronicaCoreEntityFrameworkSTD_Migra
{
    public class DatabaseContext : AgronicaCoreEntityFrameworkSTD.GiasDbContext
    { 

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(string.Format("Filename={0}", "Data Source=GiasDB.db"));
        }
    }

}
