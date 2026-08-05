using AgronicaCoreDataProviderSTD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgronicaCoreAPI.models
{
    public class ImpreseEntity
    {
        public string piva;
        public string cuaa;
        public string rag_soc;
        public int tipo;
        public string padre;
        public int livello;
        public string prov;
        public string provincia;
        public string reg;
        public string regione;
        public string stato;
    }

    public class CentriAziendaliEntity
    {
        public string piva;
        public int sa_cod;
        public string sa_nome;
        public double lat;
        public double lng;
    }

    public class MagazziniEntity
    {
        public string Piva;
        public int Sa_Cod;
        public int Id_Destinazione;
        public int Tipo_Destinazione;
        public string Ubic_Des;
        public int Visibile_da_App;
    }
    
    public class RegImpiantiEntity
    {
        public string piva;
        public int sa_cod;
        public int appezza;
        public int id_reg;
        public string rag_soc;
        public string sa_nome;
        public int campo_cod;
        public string campo_des;
        public string app_nome;
        public int veg_cod;
        public string veg_des;
        public int id_cod;
        public string codici_anagrafe_des;
        public int cul_cod;
        public string cul_des;
        public int grfi_cod;
        public string grfi_des;
        public int regolamento;
        public string reg_des;
        public double sup_app;
        public double sup_imp;
        public int cop_cod;
        public string copertura;
        public int imp_cod;
        public string imp_des;
        public int progetto_cod;
        public string progetto;
        public DateTime validita_inizio_distinta;
        public DateTime validita_fine_distinta;
        public DateTime validita_inizio_impianto;
        public DateTime validita_fine_impianto;
        public DateTime validita_inizio_appezza;
        public DateTime validita_fine_appezza;
        public string Codici_Anagrafe_Impianto;
        public string codici_anagrafe_appezzamento;
        public string StaticMapBase64String;
        public int cover;
        public bool Blk_Flag;
        public DateTime Blk_Inizio_Data;
        public DateTime Blk_Fine_Data;
        public string Blk_Inizio_Username;
        public string Blk_Fine_Username;
        public string Blk_Inizio_Note;
        public string Blk_Fine_Note;
    }

    public class MacchineEntity
    {
        public string Piva;
        public int Sa_Cod;
        public int Mac_Cod;
        public string Mac_Des;
        public string Class_Code;
        public string CLASS_DESC;
        public string Modello;
        public int Ditta_Cod;
        public string Ditta_Des;
        public string Codice;
        public int Tipo;
        public int TitoloPossesso;
        public string Denominazione_Proprietario;
        public string Targa;
        public string N_Immatricolazione;
        public DateTime Data_Immatricolazione;
        public DateTime Validita_Inizio;
        public DateTime Validita_Fine;
        public string BTM_Serial;
        public string VIN;
        public string Img_Thumbnail;
        public string Img_Thumbnail_FileName;
        public string Img_Thumbnail_Extension;
        public string Img_Large;
        public string Img_Large_FileName;
        public string Img_Large_Extension;

        public string Distinta_Installazione;
        public string Contratto_Installazione;
        public string Tipologia_Installazione;
        public DateTime? Data_Inizio_Installazione;
        public DateTime? Data_Fine_Installazione;
        public string Stato_Installazione;
        public string Provincia_Istat_Installazione;
        public string Comune_Istat_Installazione;
        public string Indirizzo_Installazione;

        public float? Latitudine_Installazione;
        public float? Longitudine_Installazione;
        public string Cod_Contatto;
        public int Visibile_ctrl_gestione;
    }

    public class TipoMacchineEntity
    {
        public string CLASS_CODE;
        public string CLASS_DESC;
    }

    public class ProdottiEntity
    {
        public int Elem_Cod;
        public string NomeComune;
        public int Prodotto_Cod;
        public string Prodotto_Des;
        public double Prodotto_Giacenza;
        public double N;
        public double P2O5;
        public double K2O;
        public double Cu;
        public int Uso;
        public string Piva;
        public int Sa_Cod;
        public int Udm_Cod;
        public int Veg_Cod;
        public bool IsTrappolaFormulato;
    }

    public class CodificaProdottiEntity
    {
        public string Piva_SuperUser;
        public int Elem_Cod;
        public string Cod_Prodotto_Cliente;
        public string Desc_Prodotto_Cliente;
        public string Categoria_Prodotto_Cliente;
        public int Codice_GIAS;
        public string Desc_GIAS;
        public string Piva;
        public string Cod_Articolo;
        public int Tipo_Codifica;
        public string Note;
    }

    public class ImpostazioniEntity
    {
        public string Piva;
        public int Sa_Cod;
        public int Impostazione_Cod;
        public string Impostazione_Valore;
    }

    public class UnitaMisuraEntity
    {
        public int Elem_Cod;
        public int Udm_Cod;
        public string NomeComune;
        public string Udm_des;
        public string Udm_Sim;
        public int TipoControllo_Cod;
    }

}
