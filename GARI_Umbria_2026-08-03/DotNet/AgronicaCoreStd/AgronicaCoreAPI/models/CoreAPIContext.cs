using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.attivita.centri_di_costo;
using AgronicaCoreModelsSTD.attivita.risorse;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AgronicaCoreAPI.models
{
    public class CoreAPIContext : DbContext
    {
        //public DbSet<Attivita> Attivita { get; set; }
        //public DbSet<EsercizioCDC> EsercizioCDC { get; set; }
        //public DbSet<AttivitaCDG> AttivitaCDG { get; set; }
        //public DbSet<Lavorazione> Lavorazione { get; set; }
        //public DbSet<RisorsaPersona> RisorsaPersona { get; set; }
        //public DbSet<RisorsaProdotto> RisorsaProdotto { get; set; }
        //public DbSet<RisorsaMacchina> RisorsaMacchina { get; set; }

        public DbSet<Token> Token { get; set; }

        public string DbPath { get; private set; }

        public CoreAPIContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = AppContext.BaseDirectory;
            //path = "C:\\AgroConnessioni";
            DbPath = $"{path}{System.IO.Path.DirectorySeparatorChar}AgronicaCoreAPI.db";
            // ChangeTracker.AutoDetectChangesEnabled = false;
            // Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");



    }
}
