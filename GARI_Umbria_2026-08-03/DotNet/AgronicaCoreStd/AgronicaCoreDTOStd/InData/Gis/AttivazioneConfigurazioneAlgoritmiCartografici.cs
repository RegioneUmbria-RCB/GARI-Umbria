using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Gis
{
    public class AttivazioneConfigurazioneAlgoritmiCartografici
    {
        public int configurazioneProiezione_Cod { get; set; }
        public int layer_cod { get; set; }
        public int tipologia_layer_cod { get; set; }
        public List<EntitaAlgoritmoCartografico> listaEntita { get; set; }
        public bool isAttivo { get; set; }
    }

    public class EntitaAlgoritmoCartografico
    {
        public int entita_cod_1 { get; set; }
        public int entita_cod_2 { get; set; }
        public int entita_cod_risultato { get; set; }
    }

    public class SottomettiElaborazioneMassivaGISCheckList_In
    {
        public int checkListType { get; set; }
        public DateTime dataRiferimento { get; set; } 
        public List<string> elencoPiva { get; set; }
        public bool elaboraAziendeInVisibilita { get; set; }
    }

    public class SottomettiElaborazioneMassivaGISCheckList_Async
    {
        public int id { get; set; }
        public int checkListType { get; set; }
        public DateTime dataRiferimento { get; set; }
        public List<string> elencoPiva { get; set; }
        public bool elaboraAziendeInVisibilita { get; set; }
        public string UserRequest { get; set; }
        public DateTime DateTimeRequest { get; set; }
    }

    public class ParametriAggiuntiviAlgoritmiCartografici_ISCC
    {
        public int CheckList_Type { get; set; }
        public int CheckList_ID { get; set; }
        public int Elaborazione_ID { get; set; }
    }

    public class IncludiEsludiImpresaISCC_In
    {
        public int checkListType { get; set; }
        public List<IncludiEsludiImpresaISCCDetail_In> elencoAziende { get; set; }
    }

    public class IncludiEsludiImpresaISCCDetail_In
    {
        public string piva { get; set; }
        public int flag_includi { get; set; }
    }

    public class LeggiElencoElaborazioniMassive_In
    {
        public int checkListType { get; set; }
        public DateTime dataLetturaInizio { get; set; }
        public DateTime dataLetturaFine { get; set; }
        public string piva { get; set; }
    }

    public class LeggiElencoElaborazioniMassive_Out
    {
        public List<ElencoElaborazioniMassivePerAzienda> elenco { get; set; }
    }

    public class ElencoElaborazioniMassivePerAzienda
    {
        public string piva { get; set; }
        public List<ElencoElaborazioniMassivePerTipoCheckList> elencoElaborazioniPerTipo { get; set; }
    }

    public class ElencoElaborazioniMassivePerTipoCheckList
    {
        public int checkList_id { get; set; }
        public string checkList_des { get; set; }
        public List<ElencoElaborazioniMassiveDettaglio> elencoElaborazioni { get; set; }
    }

    public class ElencoElaborazioniMassiveDettaglio
    {
        public int id_elaborazione { get; set; }
        public DateTime dataElaborazione { get; set; }
        public string esito { get; set; }
        public List<ElencoElaborazioniMassiveDettaglioXAlgoritmo> dettaglioAlgoritmi { get; set; }
    }

    public class ElencoElaborazioniMassiveDettaglioXAlgoritmo
    {
        public int id_algoritmo { get; set; }
        public string descrizione { get; set; }
        public string esito { get; set; }
        public List<ElencoElaborazioniMassiveDettaglioXAlgoritmoXRichiesta> dettaglio { get; set; }
    }

    public class ElencoElaborazioniMassiveDettaglioXAlgoritmoXRichiesta
    { 
        public string descrizione { get; set; }
        public string esito { get; set; }
    }

    public class VerificaAziendaAbilitataISCC_In
    {
        public int checkListType { get; set; }
        public string piva { get; set; }
    }

    public class LeggiDateImpiantiPerFiltroTemporale_In
    {
        public DateTime date { get; set; }
        public string piva { get; set; }
        public string sa_cod { get; set; }
    }
    
    public class LeggiDateImpiantiPerFiltroTemporale_Out
    {
        public DateTime dataElaborazione { get; set; }
    }
}
