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


Partial Public Class MenuBS_2017

    '''<summary>
    '''Controllo Meteo_headerPlaceHeader.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents Meteo_headerPlaceHeader As Global.System.Web.UI.WebControls.PlaceHolder

    '''<summary>
    '''Controllo meteo.
    '''</summary>
    '''<remarks>
    '''Campo generato automaticamente.
    '''Per la modifica, spostare la dichiarazione di campo dal file di progettazione al file code-behind.
    '''</remarks>
    Protected WithEvents meteo As Global.AgronicaControlli_2010.AgroMeteo

    '''<summary>
    '''Proprietà Master.
    '''</summary>
    '''<remarks>
    '''Proprietà generata automaticamente.
    '''</remarks>
    Public Shadows ReadOnly Property Master() As AgroAgenda_2010.AgendaBootstrap
        Get
            Return CType(MyBase.Master, AgroAgenda_2010.AgendaBootstrap)
        End Get
    End Property
End Class
