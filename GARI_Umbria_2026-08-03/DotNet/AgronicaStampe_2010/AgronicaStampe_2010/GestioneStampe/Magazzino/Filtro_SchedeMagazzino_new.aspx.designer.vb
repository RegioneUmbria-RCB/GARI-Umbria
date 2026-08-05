'------------------------------------------------------------------------------
' <generato automaticamente>
'     Codice generato da uno strumento.
'
'     Le modifiche a questo file possono causare un comportamento non corretto e verranno perse se
'     il codice viene rigenerato. 
' </generato automaticamente>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On


Partial Public Class Filtro_SchedeMagazzino_new
    
    '''<summary>
    '''Controllo CB_BloccaOperazioni.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents CB_BloccaOperazioni As Global.System.Web.UI.WebControls.CheckBox
    
    '''<summary>
    '''Controllo ImgBtn_Stampa.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents ImgBtn_Stampa As Global.System.Web.UI.WebControls.ImageButton
    
    '''<summary>
    '''Controllo ImgBtn_StampaExcel.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents ImgBtn_StampaExcel As Global.System.Web.UI.WebControls.ImageButton
    
    '''<summary>
    '''Controllo caricoscarico.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents caricoscarico As Global.System.Web.UI.WebControls.RadioButtonList
    
    '''<summary>
    '''Controllo rbl_caricoscarico2.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents rbl_caricoscarico2 As Global.System.Web.UI.WebControls.RadioButtonList
    
    '''<summary>
    '''Controllo Rbl_Arrotondamento.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Rbl_Arrotondamento As Global.System.Web.UI.WebControls.RadioButtonList
    
    '''<summary>
    '''Controllo TxtDataDa.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents TxtDataDa As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo TxtDataA.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents TxtDataA As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo btn_AnnataPrecedente.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents btn_AnnataPrecedente As Global.System.Web.UI.HtmlControls.HtmlGenericControl
    
    '''<summary>
    '''Controllo ImgBtn_AnnataPrecedente.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents ImgBtn_AnnataPrecedente As Global.System.Web.UI.WebControls.ImageButton
    
    '''<summary>
    '''Controllo btn_AnnataSuccessiva.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents btn_AnnataSuccessiva As Global.System.Web.UI.HtmlControls.HtmlGenericControl
    
    '''<summary>
    '''Controllo ImgBtn_AnnataSuccessiva.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents ImgBtn_AnnataSuccessiva As Global.System.Web.UI.WebControls.ImageButton
    
    '''<summary>
    '''Controllo TxtStampa.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents TxtStampa As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo Txt_Impresa.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_Impresa As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo ImgBtn_CercaImpresa.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents ImgBtn_CercaImpresa As Global.System.Web.UI.WebControls.ImageButton
    
    '''<summary>
    '''Controllo Chk_LogoRegione.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Chk_LogoRegione As Global.System.Web.UI.WebControls.CheckBox
    
    '''<summary>
    '''Controllo Cmb_Regioni.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Cmb_Regioni As Global.System.Web.UI.WebControls.DropDownList
    
    '''<summary>
    '''Controllo Cmb_Impresa.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Cmb_Impresa As Global.System.Web.UI.WebControls.DropDownList
    
    '''<summary>
    '''Controllo Lbl_NumImprese.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Lbl_NumImprese As Global.System.Web.UI.WebControls.Label
    
    '''<summary>
    '''Controllo Cmb_CentroAziendale.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Cmb_CentroAziendale As Global.System.Web.UI.WebControls.DropDownList
    
    '''<summary>
    '''Controllo Lbl_NumCentri.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Lbl_NumCentri As Global.System.Web.UI.WebControls.Label
    
    '''<summary>
    '''Controllo Cmb_Magazzino.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Cmb_Magazzino As Global.System.Web.UI.WebControls.DropDownList
    
    '''<summary>
    '''Controllo Lbl_NumMagazzini.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Lbl_NumMagazzini As Global.System.Web.UI.WebControls.Label
    
    '''<summary>
    '''Controllo Chk_Composizione.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Chk_Composizione As Global.System.Web.UI.WebControls.CheckBox
    
    '''<summary>
    '''Controllo cmb_CatProdotto.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents cmb_CatProdotto As Global.System.Web.UI.WebControls.DropDownList
    
    '''<summary>
    '''Controllo Txt_ProdottoCerca.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_ProdottoCerca As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo ImgBtn_ProdottiCerca.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents ImgBtn_ProdottiCerca As Global.System.Web.UI.WebControls.ImageButton
    
    '''<summary>
    '''Controllo cmb_Prodotti.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents cmb_Prodotti As Global.System.Web.UI.WebControls.DropDownList
    
    '''<summary>
    '''Controllo Lbl_NumProdotti.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Lbl_NumProdotti As Global.System.Web.UI.WebControls.Label
    
    '''<summary>
    '''Controllo Txt_Lotto.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_Lotto As Global.System.Web.UI.WebControls.TextBox
    
    '''<summary>
    '''Controllo ChkList_Categorie.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents ChkList_Categorie As Global.System.Web.UI.WebControls.CheckBoxList
    
    '''<summary>
    '''Controllo Rbl_Ordinamento.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Rbl_Ordinamento As Global.System.Web.UI.WebControls.RadioButtonList
    
    '''<summary>
    '''Controllo Rbl_StampaLotto.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Rbl_StampaLotto As Global.System.Web.UI.WebControls.RadioButtonList
    
    '''<summary>
    '''Controllo Rbl_Raggruppamento.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Rbl_Raggruppamento As Global.System.Web.UI.WebControls.RadioButtonList
    
    '''<summary>
    '''Controllo hdQs_Sa_Cod.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents hdQs_Sa_Cod As Global.System.Web.UI.HtmlControls.HtmlInputHidden
    
    '''<summary>
    '''Controllo hdQs_Fabbricato_Cod.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents hdQs_Fabbricato_Cod As Global.System.Web.UI.HtmlControls.HtmlInputHidden
    
    '''<summary>
    '''Controllo hdQS_DataStampa.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents hdQS_DataStampa As Global.System.Web.UI.HtmlControls.HtmlInputHidden
    
    '''<summary>
    '''Controllo hdQS_DataInizio.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents hdQS_DataInizio As Global.System.Web.UI.HtmlControls.HtmlInputHidden
    
    '''<summary>
    '''Controllo hdQS_DataFine.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents hdQS_DataFine As Global.System.Web.UI.HtmlControls.HtmlInputHidden
    
    '''<summary>
    '''Controllo hds_piva.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents hds_piva As Global.System.Web.UI.HtmlControls.HtmlInputHidden
    
    '''<summary>
    '''Controllo hds_sa_cod.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents hds_sa_cod As Global.System.Web.UI.HtmlControls.HtmlInputHidden
    
    '''<summary>
    '''Controllo hds_fabbricato_cod.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents hds_fabbricato_cod As Global.System.Web.UI.HtmlControls.HtmlInputHidden
    
    '''<summary>
    '''Controllo hds_cat_prodotto.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents hds_cat_prodotto As Global.System.Web.UI.HtmlControls.HtmlInputHidden
    
    '''<summary>
    '''Controllo hds_prodotto.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents hds_prodotto As Global.System.Web.UI.HtmlControls.HtmlInputHidden
    
    '''<summary>
    '''Controllo hdTipo_Scheda.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents hdTipo_Scheda As Global.System.Web.UI.HtmlControls.HtmlInputHidden
    
    '''<summary>
    '''Controllo hdReportSelezionato.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents hdReportSelezionato As Global.System.Web.UI.HtmlControls.HtmlInputHidden
End Class
