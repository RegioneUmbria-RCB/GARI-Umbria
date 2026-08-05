using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Anagrafica
{
    public class ParcoMacchineDto
    {
        public string _Piva;
        public int _Mac_Cod;
        public int _Sa_Cod;
        public int Sa_Cod { get; set; }
        public string _Descrizione_per_Agenda;
        public int _Mac_Cod_Origine;
        public string _Piva_superUser_Origine;
        public bool _GiacenzaIniziale;
        public bool GiacenzaIniziale { get; set; }
        public int _Finalita;
        public Value_Text _Tipo;
        public int _Marca;
        public Value_Text _Dettaglio_1;
        public Value_Text _Dettaglio_2;
        public string _Descrizione;
        public string _Targa;
        public int _Tipo_Targa;
        public string _Telaio;
        public string _Modello;
        public string _Proprietario;
        public int _Alimentazione;
        public decimal _Potenza;
        public int _UDM_Potenza;
        public decimal _Taratura_Ugello;
        public int _Titolo_Possesso;
        public string _CUAA_Proprietario;
        public string _Data_carico;
        public string _Data_scarico;
        public string _Numero_Immatricolazione;
        public string _Data_Immatricolazione;
        public string _Numero_Immatricolazione_Rimorchio;
        public string Numero_Immatricolazione_Rimorchio { get; set; }
        public string _Numero_Autorizzazione_Trasporto;
        public string _Data_Rilascio_Autorizzazione;
        public string Data_Rilascio_Autorizzazione { get; set; }
        public string _Data_Inizio_Utilizzo;
        public decimal _Peso_Tara;
        public bool _Macchina_Attiva;
        public bool Macchina_Attiva { get; set; }
        // public _Macchina_Dismessa As Boolean
        public string _Stato_Utilizzo;
        public string _Data_Dismissione;
        public string _Visibilita;
        public string _Note;
        public string _Data_Ultima_Revisione;
        public decimal _Costo_Acquisto;
        public string _Costo_Manutenzione_Revisione;
        public decimal _Ammortamento_Annuo_Percentuale;
        
        public List<Parco_Macchine_Manutenzione> _Manutenzioni;
        public List<Parco_Macchine_Costo> _Costi;
    }
    public class Value_Text
    {
        public string val;
        public string text;
        public Value_Text()
        {
            val = "";
            text = "";
        }
    }
    public class Parco_Macchine_Manutenzione
    {
        public int _ID_Agenda;
        public string _Data;
        public string _Movimento_Des;
        public decimal _Costo;
        public string _N_Certificato;
    }
    public class Parco_Macchine_Costo
    {
        public int _ID;
        public int _Unita_Misura;
        public string _Unita_Misura_Des;
        public decimal _Prezzo;
        public string _Validita_Inizio;
        public string _Validita_Fine;
    }
}
