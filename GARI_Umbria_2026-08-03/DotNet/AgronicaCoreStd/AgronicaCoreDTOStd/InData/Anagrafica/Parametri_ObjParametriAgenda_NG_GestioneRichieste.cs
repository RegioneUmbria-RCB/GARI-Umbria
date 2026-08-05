using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Anagrafica
{
    public class Parametri_ObjParametriAgenda_NG_GestioneRichieste
    {
        public int? TipoOperazioneDB;
        public string Piva;
        public long? Sa_Cod;
        public long? Fabbricato;
        public long? Campo_Cod;
        public long? Appezza;
        public long? Id_Reg;
        public long? Progetto_Cod;
        public DateTime Validita_Inizio;
        public DateTime Validita_Fine;
        public int? Veg_Cod;
        public string Veg_Des;
        public int? Id_Cod;
        public string Id_Des;
        public int? Cau_Mov;
        public int? Mac_Cod;
        public string Lav_Des;
        public int? Lav_Cod;
        public string SaNome;
        public string RagSoc;
        public int? Pagina_Provenienza;
        public int? Pagina_Richiesta;
        public DateTime Data;
        public string Cod_Contatto;
        public string QueryStringFiltrino;
        public List<ImpiantiAgendaNG> Impianti;
        public int Id_Agenda;
        public AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Attivita TipoOperazioneAgenda;
        public AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Ricetta TipoRicetta;
        public AgronicaCoreModelsSTD.attivita.Attivita.Stati Stato;
        public int Ricetta_Operazione_Cod;
        public int Ricetta_Cod;
        public string GenericObj_string;
        public int TargetOperazione;
        public int Programmazione_Cod;
        public string RedirectUrl;
        public int IdSezione;

    }

    public class ImpiantiAgendaNG
    {
        public string Piva;
        public int Sa_Cod;
        public int Appezza;
        public int Id_Reg;
        public int Progetto_Cod;
        public int Veg_Cod;
        public int Id_Cod;
        public decimal Sup_Imp;
    }
}
