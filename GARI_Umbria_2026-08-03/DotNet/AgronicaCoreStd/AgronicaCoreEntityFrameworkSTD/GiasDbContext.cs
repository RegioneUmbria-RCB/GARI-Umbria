using System;
using AgronicaCoreEntityFrameworkSTD_POCO;
using AgronicaCoreEntityFrameworkSTD_POCO.Models;
using Microsoft.EntityFrameworkCore;

namespace AgronicaCoreEntityFrameworkSTD
{
    public class GiasDbContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
            modelBuilder.Entity<APP_Avversita>()
                .HasIndex(b => b.Av_Cod).IsUnique();

            modelBuilder.Entity<APP_AvversitaxSpecie>()
                .HasIndex(b => new { b.Av_Cod, b.Veg_Cod }).IsUnique();

            modelBuilder.Entity<APP_Configurazione_Siti>()
                .HasIndex(b => new { b.PivaSuperUser, b.Sito_Cod, b.Chiave}).IsUnique();

            modelBuilder.Entity<APP_Utenti_Impostazioni>()
                .HasIndex(b => new { b.Impostazione_Cod }).IsUnique();

            modelBuilder.Entity<APP_Contatti>()
                .HasIndex(b => new { b.Cod_RisUm }).IsUnique();

            modelBuilder.Entity<APP_GruppoAvversita>()
                .HasIndex(b => new { b.Av_Gru }).IsUnique();

            modelBuilder.Entity<APP_Epoche>()
                .HasIndex(b => new { b.Epoca_Cod, b.Specie_Cod }).IsUnique();

            modelBuilder.Entity<APP_SpecieVegetaliXStadiCrescita>()
                .HasIndex(b => new { b.Cod_SS }).IsUnique();

            modelBuilder.Entity<APP_IndiciMaturitaxSpecieVegetali>()
                .HasIndex(b => new { b.IND_MAT_COD, b.VEG_COD }).IsUnique();

            modelBuilder.Entity<APP_MisuraXIndiciMaturita>()
                .HasIndex(b => new { b.IND_MAT_COD, b.UDM_COD }).IsUnique();

            modelBuilder.Entity<APP_IndiciMaturita>()
                .HasIndex(b => new { b.IND_MAT_COD }).IsUnique();

            modelBuilder.Entity<APP_MisuraxAvversita>()
                .HasIndex(b => new { b.COD }).IsUnique();

            modelBuilder.Entity<APP_MisuraXAvversita_Anagrafiche>()
                .HasIndex(b => new { b.MxAV_Cod, b.Anag_valore }).IsUnique();

            modelBuilder.Entity<APP_ImpiantiIrrigazioni>()
                .HasIndex(b => new { b.Imp_Cod }).IsUnique();

            modelBuilder.Entity<APP_Imprese>()
                .HasIndex(b => b.piva).IsUnique();

            modelBuilder.Entity<APP_Centri_Aziendali>()
                .HasIndex(b => new { b.piva, b.sa_cod }).IsUnique();

            modelBuilder.Entity<APP_Magazzini>()
                .HasIndex(b => new { b.Piva, b.Sa_Cod, b.Id_Destinazione }).IsUnique();

            modelBuilder.Entity<APP_Note_Intervento>()
                .HasIndex(b => new { b.Nota_Cod }).IsUnique();

            modelBuilder.Entity<APP_Operazioni>()
                .HasIndex(b => new { b.lav_cod }).IsUnique();

            modelBuilder.Entity<APP_Attivita>()
                .HasIndex(b => new { b.Id_Attivita }).IsUnique();

            modelBuilder.Entity<APP_AttivitaXOperazioni>()
                .HasIndex(b => new { b.Id_Attivita, b.Lav_Cod }).IsUnique();

            modelBuilder.Entity<APP_AttivitaXCentri_Aziendali>()
                .HasIndex(b => new { b.Piva, b.Sa_Cod, b.ID_Attivita }).IsUnique();

            modelBuilder.Entity<APP_Imputazioni_Fasi>()
                .HasIndex(b => new { b.Imputazione_Cod, b.Id_Attivita }).IsUnique();

            modelBuilder.Entity<APP_Parco_Macchine>()
                .HasIndex(b => new { b.Mac_Cod }).IsUnique();

            modelBuilder.Entity<APP_Prodotti>()
                .HasIndex(b => new { b.Piva, b.Elem_Cod, b.Prodotto_Cod }).IsUnique();

            modelBuilder.Entity<APP_Prodotti_Giacenze>()
                .HasIndex(b => new { b.Piva, b.Sa_Cod, b.Tipo_Destinazione, b.Fabbricato_Cod, b.Elem_Cod, b.Prodotto_Cod, b.Lotto, b.Cal_Cod, b.Cod_Progetto, b.Udm_Cod}).IsUnique();

            modelBuilder.Entity<APP_Reg_Impianti>()
                .HasIndex(b => new { b.piva, b.sa_cod, b.appezza, b.id_reg}).IsUnique();

            modelBuilder.Entity<APP_Ricette>()
                .HasIndex(b => new { b.ricetta_cod }).IsUnique();

            modelBuilder.Entity<APP_Ricette_Destinazioni>()
                .HasIndex(b => new { b.Ricetta_Destinazione_Cod }).IsUnique();

            modelBuilder.Entity<APP_Ricette_Dettagli>()
                .HasIndex(b => new { b.Ricetta_Dettaglio_Cod }).IsUnique();

            modelBuilder.Entity<APP_Ricette_Dettaglio_Tecnico>()
                .HasIndex(b => new { b.Ricetta_Tecnico_Cod }).IsUnique();

            modelBuilder.Entity<APP_Ricette_Operazioni>()
                .HasIndex(b => new { b.Ricetta_Operazione_Cod }).IsUnique();

            modelBuilder.Entity<APP_RicettexNote>()
                .HasIndex(b => new { b.Ricetta_Operazione_Cod, b.Ricetta_Cod, b.Nota_Cod }).IsUnique();

            modelBuilder.Entity<APP_Sequenza_Tabelle>()
                .HasIndex(b => new { b.Nome_Tabella }).IsUnique();

            modelBuilder.Entity<APP_CategorieXUnitaMisura>()
                .HasIndex(b => new { b.Udm_Cod, b.Elem_Cod }).IsUnique();

            modelBuilder.Entity<APP_CDG_Generale>()
                .HasIndex(b => new { b.Piva, b.Id_Cdg_Generale }).IsUnique();

            modelBuilder.Entity<APP_CDG_Movimenti>()
                .HasIndex(b => new { b.Piva, b.Id_CDG_Movimenti }).IsUnique();

            modelBuilder.Entity<APP_Riferimenti_Interventi_Cdg>()
                .HasIndex(b => new { b.Piva, b.Ricetta_Cod, b.Ricetta_Operazione_Cod, b.Id_Cdg_Generale_Rif  }).IsUnique();

            modelBuilder.Entity<APP_Visite>()
                .HasIndex(b => new { b.Visita_Cod }).IsUnique();

            modelBuilder.Entity<APP_Visite_Dettagli>()
                .HasIndex(b => new { b.Visita_Dettaglio_Cod }).IsUnique();

            modelBuilder.Entity<APP_Visite_Destinazioni>()
                .HasIndex(b => new { b.Visita_Destinazione_Cod }).IsUnique();

            modelBuilder.Entity<APP_Documenti>()
                .HasIndex(b => new { b.Documento_Cod }).IsUnique();

            modelBuilder.Entity<APP_Tipologie>()
                .HasIndex(b => new { b.ID_Tipologia, b.ID_Area }).IsUnique();
        }

        // COMUNI
        public DbSet<APP_Sequenza_Tabelle> APP_Sequenza_Tabelle { get; set; }
        public DbSet<APP_CategorieXUnitaMisura> APP_CategorieXUnitaMisura { get; set; }

        // CONFIGURAZIONE
        public DbSet<APP_Configurazione_Siti> APP_Configurazione_Siti { get; set; }
        public DbSet<APP_Utenti_Impostazioni> APP_Utenti_Impostazioni { get; set; }

        // AZIENDE
        public DbSet<APP_Imprese> APP_Imprese { get; set; }
        public DbSet<APP_Centri_Aziendali> APP_Centri_Aziendali { get; set; }
        public DbSet<APP_Magazzini> APP_Magazzini { get; set; }

        // BANCA DATI
        public DbSet<APP_Operazioni> APP_Operazioni { get; set; }
        public DbSet<APP_Attivita> APP_Attivita { get; set; }
        public DbSet<APP_AttivitaXOperazioni> APP_AttivitaXOperazioni { get; set; }
        public DbSet<APP_AttivitaXCentri_Aziendali> APP_AttivitaXCentri_Aziendali { get; set; }
        public DbSet<APP_Imputazioni_Fasi> APP_Imputazioni_Fasi { get; set; }
        public DbSet<APP_Avversita> APP_Avversita { get; set; }
        public DbSet<APP_GruppoAvversita> APP_GruppoAvversita { get; set; }
        public DbSet<APP_AvversitaxSpecie> APP_AvversitaxSpecie { get; set; }
        public DbSet<APP_Epoche> APP_Epoche { get; set; }
        public DbSet<APP_ImpiantiIrrigazioni> APP_ImpiantiIrrigazioni { get; set; }
        public DbSet<APP_Note_Intervento> APP_Note_Intervento { get; set; }
        public DbSet<APP_SpecieVegetaliXStadiCrescita> APP_SpecieVegetaliXStadiCrescita { get; set; }
        public DbSet<APP_MisuraxAvversita> APP_MisuraxAvversita { get; set; }
        public DbSet<APP_MisuraXAvversita_Anagrafiche> APP_MisuraXAvversita_Anagrafiche { get; set; }
        public DbSet<APP_IndiciMaturita> APP_IndiciMaturita { get; set; }
        public DbSet<APP_MisuraXIndiciMaturita> APP_MisuraXindiciMaturita { get; set; }
        public DbSet<APP_IndiciMaturitaxSpecieVegetali> APP_IndiciMaturitaxSpecieVegetali { get; set; }

        // ANAGRAFICA
        public DbSet<APP_Reg_Impianti> APP_Reg_Impianti { get; set; }
        public DbSet<APP_Contatti> APP_Contatti { get; set; }
        public DbSet<APP_Parco_Macchine> APP_Parco_Macchine { get; set; }

        // MAGAZZINO
        public DbSet<APP_Prodotti> APP_Prodotti { get; set; }
        public DbSet<APP_Prodotti_Giacenze> APP_Prodotti_Giacenze { get; set; }

        // RICETTE
        public DbSet<APP_Ricette> APP_Ricette { get; set; }
        public DbSet<APP_Ricette_Operazioni> APP_Ricette_Operazioni { get; set; }
        public DbSet<APP_RicettexNote> APP_RicettexNote { get; set; }
        public DbSet<APP_Ricette_Dettagli> APP_Ricette_Dettagli { get; set; }
        public DbSet<APP_Ricette_Dettaglio_Tecnico> APP_Ricette_Dettaglio_Tecnico { get; set; }
        public DbSet<APP_Ricette_Destinazioni> APP_Ricette_Destinazioni { get; set; }

        // CDG
        public DbSet<APP_CDG_Generale> APP_CDG_Generale { get; set; }
        public DbSet<APP_CDG_Movimenti> APP_CDG_Movimenti { get; set; }
        public DbSet<APP_Riferimenti_Interventi_Cdg> APP_Riferimenti_Interventi_Cdg { get; set; }

        // ENTRATA/USCITA
        public DbSet<APP_LogEventi> APP_LogEventi { get; set; }
        public DbSet<APP_GIS> APP_GIS{ get; set; }

        // VISITE
        public DbSet<APP_Visite> APP_Visite { get; set; }
        public DbSet<APP_Visite_Dettagli> APP_Visite_Dettagli { get; set; }
        public DbSet<APP_Visite_Destinazioni> APP_Visite_Destinazioni { get; set; }

        // DOCUMENTI
        public DbSet<APP_Documenti> APP_Documenti { get; set; }
        public DbSet<APP_Tipologie> APP_Tipologie { get; set; }
    }
}
