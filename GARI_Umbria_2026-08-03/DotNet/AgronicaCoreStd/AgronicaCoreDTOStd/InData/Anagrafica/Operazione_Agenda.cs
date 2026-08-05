using System;
namespace AgronicaCoreDTOStd.InData.Anagrafica
{
    public class Operazione_Agenda
    {
        public int Sa_Cod { get; set; }

        public int Id_Agenda { get; set; }

        public int Lav_Cod { get; set; }

        public string Des_Lib { get; set; }

        public int Blocco_Flag { get; set; }

        public DateTime Blocco_Data { get; set; }

        public string Blocco_Username { get; set; }

        public DateTime Data { get; set; }

        public int Linea_Cod { get; set; }

        public int Preparazione_Cod { get; set; }

        public int Id_Trasformazione { get; set; }

        public int Tipo_Accettazione { get; set; }

        public int Stato_Export { get; set; }

        public int Stato_Export_2 { get; set; }

        public int Tipo_Visibilita { get; set; }

        public int ChkCoge_Manuale { get; set; }

        public int Id_Attivita { get; set; }

        public int Modulo { get; set; }

        public int Audit_Cod { get; set; }

        public int Raccoglitore_Cod { get; set; }

        public int Pratica_Cod { get; set; }

        public int Split { get; set; }

        public string GisWkt { get; set; }

        public string GisWktSistemaRiferimento { get; set; }

        public string GisWktGps { get; set; }

        public int GisTipoEntita_cod { get; set; }

        public int GisLayerCod { get; set; }

        public DateTime Data_Creazione { get; set; }

        public DateTime Data_Modifica { get; set; }

        public string Username_Creazione { get; set; }

        public string Username_Modifica { get; set; }

        public int TopCode { get; set; }

        public int BaseCode { get; set; }
    }
}
