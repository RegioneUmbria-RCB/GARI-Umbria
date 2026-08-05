Imports AgronicaCorePannelloDiControlloBIZ
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class NonConformita_OPTA_Dettaglio
    Inherits System.Web.UI.UserControl

    Private Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        'carico le variabili di sessione
        If Not IsNothing(Session("PnlCtrl_ID_Elem")) Then
            Me.ID_Dettaglio = Session("PnlCtrl_ID_Elem")
            Session.Remove("PnlCtrl_ID_Elem")
        Else
            'Do Errore
        End If

    End Sub

    Public Property TitoloPannello As String
        Get
            Return PnlTitolo.InnerText
        End Get
        Set(ByVal value As String)
            PnlTitolo.InnerText = value
        End Set
    End Property

    Public Property ID_Dettaglio As Integer
        Get
            Return hfID_Elem.Value
        End Get
        Set(ByVal value As Integer)
            hfID_Elem.Value = value
        End Set
    End Property

    Public Property ID_Pannello As String
        Get
            Return hfID_Pannello.Value
        End Get
        Set(ByVal value As String)
            hfID_Pannello.Value = value
        End Set
    End Property

    Public Property Allegati As List(Of PnlCtrl_Allegati)
        Get
            Return Nothing
        End Get
        Set(ByVal value As List(Of PnlCtrl_Allegati))
            'txbNote.Text = value
        End Set
    End Property

    Public Property ID_ListaAllegati As Integer?
        Get
            Return If(hfID_ListaAllegati.Value = "", Nothing, hfID_ListaAllegati.Value)
        End Get
        Set(ByVal value As Integer?)
            hfID_ListaAllegati.Value = value
        End Set
    End Property

End Class