using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AgronicaCoreEntityFrameworkSTD_Migra.Migrations
{
    public partial class GiasApp_20180831 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "APP_Attivita",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Attivita_Extra_Campagna = table.Column<int>(nullable: false),
                    Data_Creazione = table.Column<DateTime>(nullable: false),
                    Data_Modifica = table.Column<DateTime>(nullable: false),
                    Desc = table.Column<string>(nullable: true),
                    Id_Attivita = table.Column<int>(nullable: false),
                    Piva = table.Column<string>(nullable: true),
                    Sa_Cod = table.Column<int>(nullable: false),
                    Username_Creazione = table.Column<string>(nullable: true),
                    Username_Modifica = table.Column<string>(nullable: true),
                    Validita_Fine = table.Column<DateTime>(nullable: false),
                    Validita_Inizio = table.Column<DateTime>(nullable: false),
                    datainvio = table.Column<DateTime>(nullable: true),
                    inviato = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_Attivita", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_AttivitaXOperazioni",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data_Creazione = table.Column<DateTime>(nullable: false),
                    Data_Modifica = table.Column<DateTime>(nullable: false),
                    Descrizione = table.Column<string>(nullable: true),
                    Id_Attivita = table.Column<int>(nullable: false),
                    Lav_Cod = table.Column<int>(nullable: false),
                    Username_Creazione = table.Column<string>(nullable: true),
                    Username_Modifica = table.Column<string>(nullable: true),
                    Validita_Fine = table.Column<DateTime>(nullable: false),
                    Validita_Inizio = table.Column<DateTime>(nullable: false),
                    datainvio = table.Column<DateTime>(nullable: true),
                    inviato = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_AttivitaXOperazioni", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_Avversita",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Av_Cod = table.Column<int>(nullable: false),
                    Av_Des_Vol = table.Column<string>(nullable: true),
                    Av_Gru = table.Column<int>(nullable: false),
                    Av_Gru_Des = table.Column<string>(nullable: true),
                    Data_Creazione = table.Column<DateTime>(nullable: false),
                    Data_Modifica = table.Column<DateTime>(nullable: false),
                    Username_Creazione = table.Column<string>(nullable: true),
                    Username_Modifica = table.Column<string>(nullable: true),
                    Validita_Fine = table.Column<DateTime>(nullable: false),
                    Validita_Inizio = table.Column<DateTime>(nullable: false),
                    datainvio = table.Column<DateTime>(nullable: true),
                    inviato = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_Avversita", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_AvversitaxSpecie",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Av_Cod = table.Column<int>(nullable: false),
                    Data_Creazione = table.Column<DateTime>(nullable: false),
                    Data_Modifica = table.Column<DateTime>(nullable: false),
                    Username_Creazione = table.Column<string>(nullable: true),
                    Username_Modifica = table.Column<string>(nullable: true),
                    Validita_Fine = table.Column<DateTime>(nullable: false),
                    Validita_Inizio = table.Column<DateTime>(nullable: false),
                    Veg_Cod = table.Column<int>(nullable: false),
                    datainvio = table.Column<DateTime>(nullable: true),
                    inviato = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_AvversitaxSpecie", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_CategorieXUnitaMisura",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data_Creazione = table.Column<DateTime>(nullable: false),
                    Data_Modifica = table.Column<DateTime>(nullable: false),
                    Elem_Cod = table.Column<int>(nullable: false),
                    NomeComune = table.Column<string>(nullable: true),
                    Udm_Cod = table.Column<int>(nullable: false),
                    Udm_Sim = table.Column<string>(nullable: true),
                    Udm_des = table.Column<string>(nullable: true),
                    Username_Creazione = table.Column<string>(nullable: true),
                    Username_Modifica = table.Column<string>(nullable: true),
                    Validita_Fine = table.Column<DateTime>(nullable: false),
                    Validita_Inizio = table.Column<DateTime>(nullable: false),
                    datainvio = table.Column<DateTime>(nullable: true),
                    inviato = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_CategorieXUnitaMisura", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_CDG_Generale",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data_Creazione = table.Column<DateTime>(nullable: false),
                    Data_Inserimento = table.Column<DateTime>(nullable: false),
                    Data_Modifica = table.Column<DateTime>(nullable: false),
                    Id_Cdg_Generale = table.Column<int>(nullable: false),
                    Piva = table.Column<string>(nullable: true),
                    Username_Creazione = table.Column<string>(nullable: true),
                    Username_Modifica = table.Column<string>(nullable: true),
                    Validita_Fine = table.Column<DateTime>(nullable: false),
                    Validita_Inizio = table.Column<DateTime>(nullable: false),
                    datainvio = table.Column<DateTime>(nullable: true),
                    inviato = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_CDG_Generale", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_CDG_Movimenti",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Cod_RisUm = table.Column<int>(nullable: false),
                    Data_Creazione = table.Column<DateTime>(nullable: false),
                    Data_Modifica = table.Column<DateTime>(nullable: false),
                    Data_Ora_Fine = table.Column<DateTime>(nullable: false),
                    Data_Ora_Inizio = table.Column<DateTime>(nullable: false),
                    Id_Attivita = table.Column<int>(nullable: false),
                    Id_CDG_Movimenti = table.Column<int>(nullable: false),
                    Id_Cdg_Generale = table.Column<int>(nullable: false),
                    Id_Imputazione = table.Column<int>(nullable: false),
                    Mac_Cod = table.Column<int>(nullable: false),
                    NrBadge = table.Column<string>(nullable: true),
                    Piva = table.Column<string>(nullable: true),
                    Qta = table.Column<decimal>(nullable: false),
                    Username_Creazione = table.Column<string>(nullable: true),
                    Username_Modifica = table.Column<string>(nullable: true),
                    Validita_Fine = table.Column<DateTime>(nullable: false),
                    Validita_Inizio = table.Column<DateTime>(nullable: false),
                    datainvio = table.Column<DateTime>(nullable: true),
                    inviato = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_CDG_Movimenti", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_Configurazione_Siti",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Chiave = table.Column<string>(nullable: true),
                    PivaSuperUser = table.Column<string>(nullable: true),
                    Sito_Cod = table.Column<int>(nullable: false),
                    Valore = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_Configurazione_Siti", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_Contatti",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Cod_Contatto = table.Column<string>(nullable: true),
                    Cod_Rapporto = table.Column<int>(nullable: false),
                    Cod_RisUm = table.Column<int>(nullable: false),
                    Cognome = table.Column<string>(nullable: true),
                    Data_Creazione = table.Column<DateTime>(nullable: false),
                    Data_Modifica = table.Column<DateTime>(nullable: false),
                    Nome = table.Column<string>(nullable: true),
                    NrBadge = table.Column<string>(nullable: true),
                    Piva = table.Column<string>(nullable: true),
                    Rapporto_Des = table.Column<string>(nullable: true),
                    Sa_Cod = table.Column<int>(nullable: false),
                    Username_Creazione = table.Column<string>(nullable: true),
                    Username_Modifica = table.Column<string>(nullable: true),
                    Validita_Fine = table.Column<DateTime>(nullable: false),
                    Validita_Inizio = table.Column<DateTime>(nullable: false),
                    datainvio = table.Column<DateTime>(nullable: true),
                    id_cf = table.Column<int>(nullable: false),
                    inviato = table.Column<int>(nullable: false),
                    rag_soc = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_Contatti", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_Epoche",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data_Creazione = table.Column<DateTime>(nullable: false),
                    Data_Modifica = table.Column<DateTime>(nullable: false),
                    Epoca_Cod = table.Column<int>(nullable: false),
                    Epoca_Des = table.Column<string>(nullable: true),
                    Specie_Cod = table.Column<int>(nullable: false),
                    Username_Creazione = table.Column<string>(nullable: true),
                    Username_Modifica = table.Column<string>(nullable: true),
                    Validita_Fine = table.Column<DateTime>(nullable: false),
                    Validita_Inizio = table.Column<DateTime>(nullable: false),
                    datainvio = table.Column<DateTime>(nullable: true),
                    inviato = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_Epoche", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_GruppoAvversita",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Av_Gru = table.Column<int>(nullable: false),
                    Av_Gru_Des = table.Column<string>(nullable: true),
                    Data_Creazione = table.Column<DateTime>(nullable: false),
                    Data_Modifica = table.Column<DateTime>(nullable: false),
                    Username_Creazione = table.Column<string>(nullable: true),
                    Username_Modifica = table.Column<string>(nullable: true),
                    Validita_Fine = table.Column<DateTime>(nullable: false),
                    Validita_Inizio = table.Column<DateTime>(nullable: false),
                    datainvio = table.Column<DateTime>(nullable: true),
                    inviato = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_GruppoAvversita", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_ImpiantiIrrigazioni",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data_Creazione = table.Column<DateTime>(nullable: false),
                    Data_Modifica = table.Column<DateTime>(nullable: false),
                    Imp_Cod = table.Column<int>(nullable: false),
                    Imp_Des = table.Column<string>(nullable: true),
                    Username_Creazione = table.Column<string>(nullable: true),
                    Username_Modifica = table.Column<string>(nullable: true),
                    Validita_Fine = table.Column<DateTime>(nullable: false),
                    Validita_Inizio = table.Column<DateTime>(nullable: false),
                    datainvio = table.Column<DateTime>(nullable: true),
                    inviato = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_ImpiantiIrrigazioni", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_Imprese",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data_Creazione = table.Column<DateTime>(nullable: false),
                    Data_Modifica = table.Column<DateTime>(nullable: false),
                    Username_Creazione = table.Column<string>(nullable: true),
                    Username_Modifica = table.Column<string>(nullable: true),
                    Validita_Fine = table.Column<DateTime>(nullable: false),
                    Validita_Inizio = table.Column<DateTime>(nullable: false),
                    cuaa = table.Column<string>(nullable: true),
                    datainvio = table.Column<DateTime>(nullable: true),
                    inviato = table.Column<int>(nullable: false),
                    piva = table.Column<string>(nullable: true),
                    rag_soc = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_Imprese", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_Imputazioni_Fasi",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data_Creazione = table.Column<DateTime>(nullable: false),
                    Data_Modifica = table.Column<DateTime>(nullable: false),
                    Id_Attivita = table.Column<int>(nullable: false),
                    Imputazione_Cod = table.Column<int>(nullable: false),
                    Imputazione_Nome = table.Column<string>(nullable: true),
                    Username_Creazione = table.Column<string>(nullable: true),
                    Username_Modifica = table.Column<string>(nullable: true),
                    Validita_Fine = table.Column<DateTime>(nullable: false),
                    Validita_Inizio = table.Column<DateTime>(nullable: false),
                    datainvio = table.Column<DateTime>(nullable: true),
                    inviato = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_Imputazioni_Fasi", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_Note_Intervento",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data_Creazione = table.Column<DateTime>(nullable: false),
                    Data_Modifica = table.Column<DateTime>(nullable: false),
                    NotaGruppo_Cod = table.Column<int>(nullable: false),
                    NotaGruppo_Des = table.Column<string>(nullable: true),
                    Nota_Cod = table.Column<int>(nullable: false),
                    Nota_Des = table.Column<string>(nullable: true),
                    Username_Creazione = table.Column<string>(nullable: true),
                    Username_Modifica = table.Column<string>(nullable: true),
                    Validita_Fine = table.Column<DateTime>(nullable: false),
                    Validita_Inizio = table.Column<DateTime>(nullable: false),
                    datainvio = table.Column<DateTime>(nullable: true),
                    inviato = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_Note_Intervento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_Operazioni",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data_Creazione = table.Column<DateTime>(nullable: false),
                    Data_Modifica = table.Column<DateTime>(nullable: false),
                    Username_Creazione = table.Column<string>(nullable: true),
                    Username_Modifica = table.Column<string>(nullable: true),
                    Validita_Fine = table.Column<DateTime>(nullable: false),
                    Validita_Inizio = table.Column<DateTime>(nullable: false),
                    datainvio = table.Column<DateTime>(nullable: true),
                    gru_cod = table.Column<int>(nullable: false),
                    gru_des = table.Column<string>(nullable: true),
                    inviato = table.Column<int>(nullable: false),
                    lav_cod = table.Column<int>(nullable: false),
                    lav_des = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_Operazioni", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_Parco_Macchine",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Classe_Desc = table.Column<string>(nullable: true),
                    Data_Creazione = table.Column<DateTime>(nullable: false),
                    Data_Modifica = table.Column<DateTime>(nullable: false),
                    Ditta_Des = table.Column<string>(nullable: true),
                    Mac_Cod = table.Column<int>(nullable: false),
                    Mac_Des = table.Column<string>(nullable: true),
                    Modello = table.Column<string>(nullable: true),
                    Piva = table.Column<string>(nullable: true),
                    Sa_Cod = table.Column<int>(nullable: false),
                    Username_Creazione = table.Column<string>(nullable: true),
                    Username_Modifica = table.Column<string>(nullable: true),
                    Validita_Fine = table.Column<DateTime>(nullable: false),
                    Validita_Inizio = table.Column<DateTime>(nullable: false),
                    datainvio = table.Column<DateTime>(nullable: true),
                    inviato = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_Parco_Macchine", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_Prodotti",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Cu = table.Column<decimal>(nullable: true),
                    Data_Creazione = table.Column<DateTime>(nullable: false),
                    Data_Modifica = table.Column<DateTime>(nullable: false),
                    Elem_Cod = table.Column<int>(nullable: false),
                    K2O = table.Column<decimal>(nullable: true),
                    N = table.Column<decimal>(nullable: true),
                    NomeComune = table.Column<string>(nullable: true),
                    P2O5 = table.Column<decimal>(nullable: true),
                    Piva = table.Column<string>(nullable: true),
                    Prodotto_Cod = table.Column<int>(nullable: false),
                    Prodotto_Des = table.Column<string>(nullable: true),
                    Prodotto_Giacenza = table.Column<decimal>(nullable: true),
                    Sa_Cod = table.Column<int>(nullable: false),
                    Udm_Cod = table.Column<int>(nullable: false),
                    Username_Creazione = table.Column<string>(nullable: true),
                    Username_Modifica = table.Column<string>(nullable: true),
                    Uso = table.Column<int>(nullable: true),
                    Validita_Fine = table.Column<DateTime>(nullable: false),
                    Validita_Inizio = table.Column<DateTime>(nullable: false),
                    datainvio = table.Column<DateTime>(nullable: true),
                    inviato = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_Prodotti", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_Prodotti_Giacenze",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Cal_Cod = table.Column<int>(nullable: false),
                    Cod_Progetto = table.Column<int>(nullable: false),
                    Data_Creazione = table.Column<DateTime>(nullable: false),
                    Data_Modifica = table.Column<DateTime>(nullable: false),
                    Elem_Cod = table.Column<int>(nullable: false),
                    Fabbricato_Cod = table.Column<int>(nullable: false),
                    Fabbricato_Des = table.Column<string>(nullable: true),
                    Giacenza = table.Column<decimal>(nullable: false),
                    Lotto = table.Column<string>(nullable: true),
                    Piva = table.Column<string>(nullable: true),
                    Prodotto_Cod = table.Column<int>(nullable: false),
                    Sa_Cod = table.Column<int>(nullable: false),
                    Tipo_Destinazione = table.Column<int>(nullable: false),
                    Udm_Cod = table.Column<int>(nullable: false),
                    Username_Creazione = table.Column<string>(nullable: true),
                    Username_Modifica = table.Column<string>(nullable: true),
                    Validita_Fine = table.Column<DateTime>(nullable: false),
                    Validita_Inizio = table.Column<DateTime>(nullable: false),
                    datainvio = table.Column<DateTime>(nullable: true),
                    inviato = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_Prodotti_Giacenze", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_Reg_Impianti",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Codici_Anagrafe_Impianto = table.Column<string>(nullable: true),
                    Data_Creazione = table.Column<DateTime>(nullable: false),
                    Data_Modifica = table.Column<DateTime>(nullable: false),
                    Username_Creazione = table.Column<string>(nullable: true),
                    Username_Modifica = table.Column<string>(nullable: true),
                    Validita_Fine = table.Column<DateTime>(nullable: false),
                    Validita_Inizio = table.Column<DateTime>(nullable: false),
                    app_nome = table.Column<string>(nullable: true),
                    appezza = table.Column<int>(nullable: false),
                    campo_des = table.Column<string>(nullable: true),
                    codici_anagrafe_des = table.Column<string>(nullable: true),
                    copertura = table.Column<string>(nullable: true),
                    cul_des = table.Column<string>(nullable: true),
                    datainvio = table.Column<DateTime>(nullable: true),
                    id_cod = table.Column<int>(nullable: false),
                    id_reg = table.Column<int>(nullable: false),
                    imp_des = table.Column<string>(nullable: true),
                    inviato = table.Column<int>(nullable: false),
                    piva = table.Column<string>(nullable: true),
                    progetto = table.Column<string>(nullable: true),
                    progetto_cod = table.Column<int>(nullable: false),
                    rag_soc = table.Column<string>(nullable: true),
                    reg_des = table.Column<string>(nullable: true),
                    sa_cod = table.Column<int>(nullable: false),
                    sa_nome = table.Column<string>(nullable: true),
                    sup_imp = table.Column<decimal>(nullable: false),
                    validita_fine_distinta = table.Column<DateTime>(nullable: false),
                    validita_inizio_distinta = table.Column<DateTime>(nullable: false),
                    veg_cod = table.Column<int>(nullable: false),
                    veg_des = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_Reg_Impianti", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_Ricette",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data_Creazione = table.Column<DateTime>(nullable: false),
                    Data_Modifica = table.Column<DateTime>(nullable: false),
                    Tipo_Ricetta = table.Column<int>(nullable: false),
                    Username_Creazione = table.Column<string>(nullable: true),
                    Username_Modifica = table.Column<string>(nullable: true),
                    Validita_Fine = table.Column<DateTime>(nullable: false),
                    Validita_Inizio = table.Column<DateTime>(nullable: false),
                    Veg_Cod = table.Column<int>(nullable: false),
                    datainvio = table.Column<DateTime>(nullable: true),
                    inviato = table.Column<int>(nullable: false),
                    note = table.Column<string>(nullable: true),
                    piva = table.Column<string>(nullable: true),
                    ricetta_cod = table.Column<int>(nullable: false),
                    ricetta_des = table.Column<string>(nullable: true),
                    sa_cod = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_Ricette", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_Ricette_Destinazioni",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Appezza = table.Column<int>(nullable: false),
                    Data_Creazione = table.Column<DateTime>(nullable: false),
                    Data_Modifica = table.Column<DateTime>(nullable: false),
                    Id_Reg = table.Column<int>(nullable: false),
                    Piva = table.Column<string>(nullable: true),
                    Qta = table.Column<decimal>(nullable: false),
                    Qta2 = table.Column<decimal>(nullable: false),
                    QuotaDistribuzione = table.Column<decimal>(nullable: false),
                    Ricetta_Cod = table.Column<int>(nullable: false),
                    Ricetta_Destinazione_Cod = table.Column<int>(nullable: false),
                    Ricetta_Dettaglio_Cod = table.Column<int>(nullable: false),
                    Ricetta_Operazione_Cod = table.Column<int>(nullable: false),
                    Sa_Cod = table.Column<int>(nullable: false),
                    Tipo_Destinazione = table.Column<int>(nullable: false),
                    Username_Creazione = table.Column<string>(nullable: true),
                    Username_Modifica = table.Column<string>(nullable: true),
                    Validita_Fine = table.Column<DateTime>(nullable: false),
                    Validita_Inizio = table.Column<DateTime>(nullable: false),
                    datainvio = table.Column<DateTime>(nullable: true),
                    inviato = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_Ricette_Destinazioni", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_Ricette_Dettagli",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Cau_Mov = table.Column<int>(nullable: false),
                    Data_Creazione = table.Column<DateTime>(nullable: false),
                    Data_Modifica = table.Column<DateTime>(nullable: false),
                    Elem_Cod = table.Column<int>(nullable: false),
                    Extra_Int = table.Column<int>(nullable: false),
                    Lotto = table.Column<string>(nullable: true),
                    Mat_Cod = table.Column<int>(nullable: false),
                    Mezzo_Det = table.Column<int>(nullable: false),
                    Pro_Cod = table.Column<int>(nullable: false),
                    Qta = table.Column<decimal>(nullable: false),
                    Qta_Extra = table.Column<decimal>(nullable: false),
                    Qta_Extra_Totale = table.Column<decimal>(nullable: false),
                    Ricetta_Cod = table.Column<int>(nullable: false),
                    Ricetta_Dettaglio_Cod = table.Column<int>(nullable: false),
                    Ricetta_Operazione_Cod = table.Column<int>(nullable: false),
                    Udm_Cod = table.Column<int>(nullable: false),
                    Udm_Cod_Extra = table.Column<int>(nullable: false),
                    Username_Creazione = table.Column<string>(nullable: true),
                    Username_Modifica = table.Column<string>(nullable: true),
                    Validita_Fine = table.Column<DateTime>(nullable: false),
                    Validita_Inizio = table.Column<DateTime>(nullable: false),
                    datainvio = table.Column<DateTime>(nullable: true),
                    inviato = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_Ricette_Dettagli", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_Ricette_Dettaglio_Tecnico",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Av_Cod = table.Column<int>(nullable: false),
                    Av_Gru = table.Column<int>(nullable: false),
                    CU = table.Column<decimal>(nullable: false),
                    Data_Creazione = table.Column<DateTime>(nullable: false),
                    Data_Modifica = table.Column<DateTime>(nullable: false),
                    Dett_Cod = table.Column<int>(nullable: false),
                    Dose = table.Column<decimal>(nullable: false),
                    Freatimetro = table.Column<decimal>(nullable: false),
                    Inn1_data = table.Column<DateTime>(nullable: false),
                    Inn2_data = table.Column<DateTime>(nullable: false),
                    K = table.Column<decimal>(nullable: false),
                    N = table.Column<decimal>(nullable: false),
                    Nitrati = table.Column<int>(nullable: false),
                    P = table.Column<decimal>(nullable: false),
                    Parziale = table.Column<int>(nullable: false),
                    Piezo1 = table.Column<decimal>(nullable: false),
                    Piezo2 = table.Column<decimal>(nullable: false),
                    Piezo3 = table.Column<decimal>(nullable: false),
                    Piezo4 = table.Column<decimal>(nullable: false),
                    Qta_Ril = table.Column<decimal>(nullable: false),
                    Ricetta_Cod = table.Column<int>(nullable: false),
                    Ricetta_Dettaglio_Cod = table.Column<int>(nullable: false),
                    Ricetta_Operazione_Cod = table.Column<int>(nullable: false),
                    Ricetta_Tecnico_Cod = table.Column<int>(nullable: false),
                    Username_Creazione = table.Column<string>(nullable: true),
                    Username_Modifica = table.Column<string>(nullable: true),
                    Validita_Fine = table.Column<DateTime>(nullable: false),
                    Validita_Inizio = table.Column<DateTime>(nullable: false),
                    datainvio = table.Column<DateTime>(nullable: true),
                    inviato = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_Ricette_Dettaglio_Tecnico", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_Ricette_Operazioni",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data_Creazione = table.Column<DateTime>(nullable: false),
                    Data_Modifica = table.Column<DateTime>(nullable: false),
                    Extra_Int = table.Column<int>(nullable: false),
                    Lav_Cod = table.Column<int>(nullable: false),
                    Mezzo = table.Column<int>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    Ricetta_Cod = table.Column<int>(nullable: false),
                    Ricetta_Operazione_Cod = table.Column<int>(nullable: false),
                    Ricetta_Operazione_Cod_RIF = table.Column<int>(nullable: false),
                    Ricetta_Operazione_Des = table.Column<string>(nullable: true),
                    Username_Creazione = table.Column<string>(nullable: true),
                    Username_Modifica = table.Column<string>(nullable: true),
                    Validita_Fine = table.Column<DateTime>(nullable: false),
                    Validita_Inizio = table.Column<DateTime>(nullable: false),
                    W_Anagrafica_Stati_Cod = table.Column<int>(nullable: false),
                    datainvio = table.Column<DateTime>(nullable: true),
                    inviato = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_Ricette_Operazioni", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_RicettexNote",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data_Creazione = table.Column<DateTime>(nullable: false),
                    Data_Modifica = table.Column<DateTime>(nullable: false),
                    Nota_Cod = table.Column<int>(nullable: false),
                    Ricetta_Cod = table.Column<int>(nullable: false),
                    Ricetta_Operazione_Cod = table.Column<int>(nullable: false),
                    Username_Creazione = table.Column<string>(nullable: true),
                    Username_Modifica = table.Column<string>(nullable: true),
                    Validita_Fine = table.Column<DateTime>(nullable: false),
                    Validita_Inizio = table.Column<DateTime>(nullable: false),
                    datainvio = table.Column<DateTime>(nullable: true),
                    inviato = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_RicettexNote", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_Riferimenti_Interventi_Cdg",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data_Creazione = table.Column<DateTime>(nullable: false),
                    Data_Modifica = table.Column<DateTime>(nullable: false),
                    Id_Cdg_Generale_Rif = table.Column<int>(nullable: false),
                    Piva = table.Column<string>(nullable: true),
                    Ricetta_Cod = table.Column<int>(nullable: false),
                    Username_Creazione = table.Column<string>(nullable: true),
                    Username_Modifica = table.Column<string>(nullable: true),
                    Validita_Fine = table.Column<DateTime>(nullable: false),
                    Validita_Inizio = table.Column<DateTime>(nullable: false),
                    datainvio = table.Column<DateTime>(nullable: true),
                    inviato = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_Riferimenti_Interventi_Cdg", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_Sequenza_Tabelle",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Base = table.Column<int>(nullable: false),
                    Data_Creazione = table.Column<DateTime>(nullable: false),
                    Data_Modifica = table.Column<DateTime>(nullable: false),
                    End = table.Column<int>(nullable: false),
                    Nome_Tabella = table.Column<string>(nullable: true),
                    Ultimo_Valore = table.Column<int>(nullable: false),
                    Username_Creazione = table.Column<string>(nullable: true),
                    Username_Modifica = table.Column<string>(nullable: true),
                    Validita_Fine = table.Column<DateTime>(nullable: false),
                    Validita_Inizio = table.Column<DateTime>(nullable: false),
                    datainvio = table.Column<DateTime>(nullable: true),
                    inviato = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_Sequenza_Tabelle", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_Utenti_Impostazioni",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data_Creazione = table.Column<DateTime>(nullable: false),
                    Data_Modifica = table.Column<DateTime>(nullable: false),
                    Impostazione_Cod = table.Column<int>(nullable: false),
                    Impostazione_Valore_1 = table.Column<string>(nullable: true),
                    Username_Creazione = table.Column<string>(nullable: true),
                    Username_Modifica = table.Column<string>(nullable: true),
                    Validita_Fine = table.Column<DateTime>(nullable: false),
                    Validita_Inizio = table.Column<DateTime>(nullable: false),
                    datainvio = table.Column<DateTime>(nullable: true),
                    inviato = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_Utenti_Impostazioni", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_APP_Attivita_Id_Attivita",
                table: "APP_Attivita",
                column: "Id_Attivita",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APP_AttivitaXOperazioni_Id_Attivita_Lav_Cod",
                table: "APP_AttivitaXOperazioni",
                columns: new[] { "Id_Attivita", "Lav_Cod" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APP_Avversita_Av_Cod",
                table: "APP_Avversita",
                column: "Av_Cod",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APP_AvversitaxSpecie_Av_Cod_Veg_Cod",
                table: "APP_AvversitaxSpecie",
                columns: new[] { "Av_Cod", "Veg_Cod" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APP_CategorieXUnitaMisura_Udm_Cod_Elem_Cod",
                table: "APP_CategorieXUnitaMisura",
                columns: new[] { "Udm_Cod", "Elem_Cod" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APP_CDG_Generale_Piva_Id_Cdg_Generale",
                table: "APP_CDG_Generale",
                columns: new[] { "Piva", "Id_Cdg_Generale" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APP_CDG_Movimenti_Piva_Id_CDG_Movimenti",
                table: "APP_CDG_Movimenti",
                columns: new[] { "Piva", "Id_CDG_Movimenti" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APP_Configurazione_Siti_PivaSuperUser_Sito_Cod_Chiave",
                table: "APP_Configurazione_Siti",
                columns: new[] { "PivaSuperUser", "Sito_Cod", "Chiave" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APP_Contatti_Cod_RisUm",
                table: "APP_Contatti",
                column: "Cod_RisUm",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APP_Epoche_Epoca_Cod_Specie_Cod",
                table: "APP_Epoche",
                columns: new[] { "Epoca_Cod", "Specie_Cod" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APP_GruppoAvversita_Av_Gru",
                table: "APP_GruppoAvversita",
                column: "Av_Gru",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APP_ImpiantiIrrigazioni_Imp_Cod",
                table: "APP_ImpiantiIrrigazioni",
                column: "Imp_Cod",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APP_Imprese_piva",
                table: "APP_Imprese",
                column: "piva",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APP_Imputazioni_Fasi_Imputazione_Cod_Id_Attivita",
                table: "APP_Imputazioni_Fasi",
                columns: new[] { "Imputazione_Cod", "Id_Attivita" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APP_Note_Intervento_Nota_Cod",
                table: "APP_Note_Intervento",
                column: "Nota_Cod",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APP_Operazioni_lav_cod",
                table: "APP_Operazioni",
                column: "lav_cod",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APP_Parco_Macchine_Mac_Cod",
                table: "APP_Parco_Macchine",
                column: "Mac_Cod",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APP_Prodotti_Piva_Elem_Cod_Prodotto_Cod",
                table: "APP_Prodotti",
                columns: new[] { "Piva", "Elem_Cod", "Prodotto_Cod" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APP_Prodotti_Giacenze_Piva_Sa_Cod_Tipo_Destinazione_Fabbricato_Cod_Elem_Cod_Prodotto_Cod_Lotto_Cal_Cod_Cod_Progetto_Udm_Cod",
                table: "APP_Prodotti_Giacenze",
                columns: new[] { "Piva", "Sa_Cod", "Tipo_Destinazione", "Fabbricato_Cod", "Elem_Cod", "Prodotto_Cod", "Lotto", "Cal_Cod", "Cod_Progetto", "Udm_Cod" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APP_Reg_Impianti_piva_sa_cod_appezza_id_reg",
                table: "APP_Reg_Impianti",
                columns: new[] { "piva", "sa_cod", "appezza", "id_reg" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APP_Ricette_ricetta_cod",
                table: "APP_Ricette",
                column: "ricetta_cod",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APP_Ricette_Destinazioni_Ricetta_Destinazione_Cod",
                table: "APP_Ricette_Destinazioni",
                column: "Ricetta_Destinazione_Cod",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APP_Ricette_Dettagli_Ricetta_Dettaglio_Cod",
                table: "APP_Ricette_Dettagli",
                column: "Ricetta_Dettaglio_Cod",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APP_Ricette_Dettaglio_Tecnico_Ricetta_Tecnico_Cod",
                table: "APP_Ricette_Dettaglio_Tecnico",
                column: "Ricetta_Tecnico_Cod",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APP_Ricette_Operazioni_Ricetta_Operazione_Cod",
                table: "APP_Ricette_Operazioni",
                column: "Ricetta_Operazione_Cod",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APP_RicettexNote_Ricetta_Operazione_Cod_Ricetta_Cod_Nota_Cod",
                table: "APP_RicettexNote",
                columns: new[] { "Ricetta_Operazione_Cod", "Ricetta_Cod", "Nota_Cod" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APP_Riferimenti_Interventi_Cdg_Piva_Ricetta_Cod_Id_Cdg_Generale_Rif",
                table: "APP_Riferimenti_Interventi_Cdg",
                columns: new[] { "Piva", "Ricetta_Cod", "Id_Cdg_Generale_Rif" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APP_Sequenza_Tabelle_Nome_Tabella",
                table: "APP_Sequenza_Tabelle",
                column: "Nome_Tabella",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_APP_Utenti_Impostazioni_Impostazione_Cod",
                table: "APP_Utenti_Impostazioni",
                column: "Impostazione_Cod",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "APP_Attivita");

            migrationBuilder.DropTable(
                name: "APP_AttivitaXOperazioni");

            migrationBuilder.DropTable(
                name: "APP_Avversita");

            migrationBuilder.DropTable(
                name: "APP_AvversitaxSpecie");

            migrationBuilder.DropTable(
                name: "APP_CategorieXUnitaMisura");

            migrationBuilder.DropTable(
                name: "APP_CDG_Generale");

            migrationBuilder.DropTable(
                name: "APP_CDG_Movimenti");

            migrationBuilder.DropTable(
                name: "APP_Configurazione_Siti");

            migrationBuilder.DropTable(
                name: "APP_Contatti");

            migrationBuilder.DropTable(
                name: "APP_Epoche");

            migrationBuilder.DropTable(
                name: "APP_GruppoAvversita");

            migrationBuilder.DropTable(
                name: "APP_ImpiantiIrrigazioni");

            migrationBuilder.DropTable(
                name: "APP_Imprese");

            migrationBuilder.DropTable(
                name: "APP_Imputazioni_Fasi");

            migrationBuilder.DropTable(
                name: "APP_Note_Intervento");

            migrationBuilder.DropTable(
                name: "APP_Operazioni");

            migrationBuilder.DropTable(
                name: "APP_Parco_Macchine");

            migrationBuilder.DropTable(
                name: "APP_Prodotti");

            migrationBuilder.DropTable(
                name: "APP_Prodotti_Giacenze");

            migrationBuilder.DropTable(
                name: "APP_Reg_Impianti");

            migrationBuilder.DropTable(
                name: "APP_Ricette");

            migrationBuilder.DropTable(
                name: "APP_Ricette_Destinazioni");

            migrationBuilder.DropTable(
                name: "APP_Ricette_Dettagli");

            migrationBuilder.DropTable(
                name: "APP_Ricette_Dettaglio_Tecnico");

            migrationBuilder.DropTable(
                name: "APP_Ricette_Operazioni");

            migrationBuilder.DropTable(
                name: "APP_RicettexNote");

            migrationBuilder.DropTable(
                name: "APP_Riferimenti_Interventi_Cdg");

            migrationBuilder.DropTable(
                name: "APP_Sequenza_Tabelle");

            migrationBuilder.DropTable(
                name: "APP_Utenti_Impostazioni");
        }
    }
}
