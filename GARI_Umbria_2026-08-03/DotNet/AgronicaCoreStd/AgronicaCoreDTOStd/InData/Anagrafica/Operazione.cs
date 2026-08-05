using AgronicaCoreDTOStd.InData.Anagrafica;
using System;
using System.Collections.Generic;


namespace AgronicaCoreDTOStd.InData.Anagrafica
{
    public class Operazione
    {
        //private List<Impianto_DTO> _ListaImpianti;

        private List<Nota_Operazione> _ListaNote;

        private List<CostiAccessori> _ListaCostiAccessori;

        private int _ID_Cod;

        private string _Nota;

        private bool _AcquaSiNo;

        private int _AcquaHaTot;

        private decimal _Acqua;

        private int _Lav_Cod;

        private string _Data_Operazione;

        private int _Veg_Cod;

        private string _Fabbricato_Cod;

        private string _Disciplinare_Cod;

        private int _Codice_Filtro_Ricerca;
    }
    public class Impianto_DTO
    {
        public bool selected;
        public string Piva;
        public int Sa_Cod;
        public int Appezza;
        public int Campo_Cod;
        public int ID_Reg;
        public string App_Nome;
        public int Progetto_Cod;
        public decimal Sup_Imp;
        public DateTime Validita_Inizio_Distinta;
        public DateTime Validita_Fine_Distinta;
        public string Cul_Des;
        public string Data_Raccolta;
        public string Data_Raccolta_Prevista;
        public string Data_Fioritura;
        public string Data_Fioritura_Prevista;
        public int Veg_Cod;
        public int Id_Cod;
        public int Cul_Cod;
        public string Rag_Soc;
        public string Veg_Des;
        public DateTime Validita_Inizio;
        public DateTime Validita_Fine;
        private decimal _Qta;
        private decimal _Qta2;
        public string Codici_Anagrafe_Des;
        public string Sa_Nome;
        public string Campo_Des;
        public string RifNumerico;
        public string CodBioApp;
        public string Catasto;
        public string Disciplinare;
        public string Reg_Des;
        public string Capitolato_Privato_Des;
        public string Grfi_Des;
        public string Data_Semina;
        public string Progetto;
        public string Copertura;
        public string Data_Semina_Prevista;
        public string N_Massimo;
        public string P_Massimo;
        public string K_Massimo;
        public string Mg_Massimo;
        public string SpecieAgea;
        public string CultivarAgea;
        public string Tra_Fila;
        public string Su_Fila;
        public string P_Impianto;
        public string Foral_Des;
        public string Port_Des;
        public string Finanziamento;
        public string Regolamento;
        public string Imp_Des;
    }
    public class CostiAccessori
    {
        private string _CentroCosto;
        private string _CategoriaRisorsa;
        private string _CodiceRisorsa;
        private int _Udm_Cod;
        private decimal _Qta;
    }

}
