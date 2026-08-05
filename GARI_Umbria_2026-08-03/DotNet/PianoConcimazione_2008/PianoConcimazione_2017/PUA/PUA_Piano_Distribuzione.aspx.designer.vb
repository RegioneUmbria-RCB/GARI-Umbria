'------------------------------------------------------------------------------
' <generato automaticamente>
'     Questo codice è stato generato da uno strumento.
'
'     Le modifiche a questo file possono causare un comportamento non corretto e verranno perse se
'     il codice viene rigenerato. 
' </generato automaticamente>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On


Partial Public Class PUA_Piano_Distribuzione

    '''<summary>
    '''Controllo hf_UtenteAbilitatoLettura.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents hf_UtenteAbilitatoLettura As Global.System.Web.UI.WebControls.HiddenField

    '''<summary>
    '''Controllo hf_UtenteAbilitatoScrittura.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents hf_UtenteAbilitatoScrittura As Global.System.Web.UI.WebControls.HiddenField

    '''<summary>
    '''Controllo ddlRegolamento.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents ddlRegolamento As Global.System.Web.UI.WebControls.DropDownList

    '''<summary>
    '''Controllo ddlMetodo.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents ddlMetodo As Global.System.Web.UI.WebControls.DropDownList

    '''<summary>
    '''Controllo Txt_ValiditaInizio.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_ValiditaInizio As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Txt_ValiditaFine.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_ValiditaFine As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo ddlCentriAziendali.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents ddlCentriAziendali As Global.System.Web.UI.WebControls.DropDownList

    '''<summary>
    '''Controllo DivEffluenti.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents DivEffluenti As Global.System.Web.UI.HtmlControls.HtmlGenericControl

    '''<summary>
    '''Controllo HD_Effluenti.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents HD_Effluenti As Global.System.Web.UI.HtmlControls.HtmlInputHidden

    '''<summary>
    '''Controllo HD_Effluenti_Dettagli.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents HD_Effluenti_Dettagli As Global.System.Web.UI.HtmlControls.HtmlInputHidden

    '''<summary>
    '''Controllo DivMedieAziendali.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents DivMedieAziendali As Global.System.Web.UI.HtmlControls.HtmlGenericControl

    '''<summary>
    '''Controllo rigabilancio.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents rigabilancio As Global.System.Web.UI.HtmlControls.HtmlGenericControl

    '''<summary>
    '''Controllo Txt_NUtile.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_NUtile As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Txt_NTotale.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_NTotale As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Txt_IndiceEff.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_IndiceEff As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Txt_Ha_ZVN.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_Ha_ZVN As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Txt_Ha_Ord.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_Ha_Ord As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Txt_Ha_Tot.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_Ha_Tot As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Txt_Ha_ZVN_SecondoRaccolto.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_Ha_ZVN_SecondoRaccolto As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Txt_Ha_Ord_SecondoRaccolto.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_Ha_Ord_SecondoRaccolto As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Txt_Ha_Tot_SecondoRaccolto.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_Ha_Tot_SecondoRaccolto As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Txt_Ha_ZVN_Fertilizzati.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_Ha_ZVN_Fertilizzati As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Txt_Ha_Ord_Fertilizzati.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_Ha_Ord_Fertilizzati As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Txt_Ha_Tot_Fertilizzati.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_Ha_Tot_Fertilizzati As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Txt_Ha_ZVN_Fertilizzati_Zoo.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_Ha_ZVN_Fertilizzati_Zoo As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Txt_Ha_Ord_Fertilizzati_Zoo.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_Ha_Ord_Fertilizzati_Zoo As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Txt_Ha_Tot_Fertilizzati_Zoo.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_Ha_Tot_Fertilizzati_Zoo As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Txt_NZoo_Tot_ZVN.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_NZoo_Tot_ZVN As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Txt_NZoo_Tot_Ord.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_NZoo_Tot_Ord As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Txt_NZoo_Tot_Media.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_NZoo_Tot_Media As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Txt_NZoo_Tot_Let_ZVN.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_NZoo_Tot_Let_ZVN As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Txt_NZoo_Tot_Let_Ord.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_NZoo_Tot_Let_Ord As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Txt_NZoo_Tot_Let_Media.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_NZoo_Tot_Let_Media As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Txt_NZoo_Tot_Liq_ZVN.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_NZoo_Tot_Liq_ZVN As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Txt_NZoo_Tot_Liq_Ord.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_NZoo_Tot_Liq_Ord As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Txt_NZoo_Tot_Liq_Media.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_NZoo_Tot_Liq_Media As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Txt_NZoo_ZVN.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_NZoo_ZVN As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Txt_NZoo_Ord.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_NZoo_Ord As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Txt_NZoo_Media.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_NZoo_Media As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Txt_NLetame_ZVN.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_NLetame_ZVN As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Txt_NLetame_Ord.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_NLetame_Ord As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Txt_NLetame_Tot.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_NLetame_Tot As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Txt_NLiquame_ZVN.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_NLiquame_ZVN As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Txt_NLiquame_Ord.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_NLiquame_Ord As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo Txt_NLiquame_Tot.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Txt_NLiquame_Tot As Global.System.Web.UI.WebControls.TextBox

    '''<summary>
    '''Controllo hdPiva.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents hdPiva As Global.System.Web.UI.HtmlControls.HtmlInputHidden

    '''<summary>
    '''Controllo hdSaCod.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents hdSaCod As Global.System.Web.UI.HtmlControls.HtmlInputHidden

    '''<summary>
    '''Controllo hdPuaCod.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents hdPuaCod As Global.System.Web.UI.HtmlControls.HtmlInputHidden

    '''<summary>
    '''Controllo hdPuaTipo.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents hdPuaTipo As Global.System.Web.UI.HtmlControls.HtmlInputHidden

    '''<summary>
    '''Controllo hdRegCod.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents hdRegCod As Global.System.Web.UI.HtmlControls.HtmlInputHidden

    '''<summary>
    '''Controllo hdRicettaCod.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents hdRicettaCod As Global.System.Web.UI.HtmlControls.HtmlInputHidden

    '''<summary>
    '''Controllo hdModalita.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents hdModalita As Global.System.Web.UI.HtmlControls.HtmlInputHidden

    '''<summary>
    '''Controllo hdDataInizio.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents hdDataInizio As Global.System.Web.UI.HtmlControls.HtmlInputHidden

    '''<summary>
    '''Controllo hdDataFine.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents hdDataFine As Global.System.Web.UI.HtmlControls.HtmlInputHidden

    '''<summary>
    '''Controllo hdBloccoFlag.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents hdBloccoFlag As Global.System.Web.UI.HtmlControls.HtmlInputHidden

    '''<summary>
    '''Controllo hdImportaDaQdC.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents hdImportaDaQdC As Global.System.Web.UI.HtmlControls.HtmlInputHidden

    '''<summary>
    '''Controllo hdVisualizzaAnalisi.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents hdVisualizzaAnalisi As Global.System.Web.UI.HtmlControls.HtmlInputHidden

    '''<summary>
    '''Controllo hdUsaAnalisiNG.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents hdUsaAnalisiNG As Global.System.Web.UI.HtmlControls.HtmlInputHidden

    '''<summary>
    '''Controllo hdAssegnaDefault.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents hdAssegnaDefault As Global.System.Web.UI.HtmlControls.HtmlInputHidden

    '''<summary>
    '''Proprietà Master.
    '''</summary>
    '''<remarks>
    '''Proprietà generata automaticamente.
    '''</remarks>
    Public Shadows ReadOnly Property Master() As PianoConcimazione_2017.MasterConcimazione
        Get
            Return CType(MyBase.Master, PianoConcimazione_2017.MasterConcimazione)
        End Get
    End Property
End Class
