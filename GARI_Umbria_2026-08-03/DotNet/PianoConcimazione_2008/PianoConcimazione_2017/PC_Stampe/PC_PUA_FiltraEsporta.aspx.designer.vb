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


Partial Public Class PC_PUA_FiltraEsporta

    '''<summary>
    '''Controllo ddl_Gerarchia_Impresa.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents ddl_Gerarchia_Impresa As Global.System.Web.UI.WebControls.DropDownList

    '''<summary>
    '''Controllo data_inizio.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents data_inizio As Global.System.Web.UI.HtmlControls.HtmlInputText

    '''<summary>
    '''Controllo data_fine.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents data_fine As Global.System.Web.UI.HtmlControls.HtmlInputText

    '''<summary>
    '''Controllo ddl_SpecieVegetali.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents ddl_SpecieVegetali As Global.System.Web.UI.WebControls.DropDownList

    '''<summary>
    '''Controllo chekUltimo.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents chekUltimo As Global.System.Web.UI.HtmlControls.HtmlInputCheckBox

    '''<summary>
    '''Controllo Div_BTNEsporta.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Div_BTNEsporta As Global.System.Web.UI.HtmlControls.HtmlGenericControl

    '''<summary>
    '''Controllo HD_Pua.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents HD_Pua As Global.System.Web.UI.HtmlControls.HtmlInputHidden

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
