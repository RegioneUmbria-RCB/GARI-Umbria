Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class Parco_Macchine_Costo

#Region ""
    Private _ID As Integer
    Public Property ID() As Integer
        Get
            Return _ID
        End Get
        Set(ByVal value As Integer)
            _ID = value
        End Set
    End Property


    Private _Unita_Misura As Integer
    Public Property Unita_Misura() As Integer
        Get
            Return _Unita_Misura
        End Get
        Set(ByVal value As Integer)
            _Unita_Misura = value
        End Set
    End Property
    Private _Unita_Misura_Des As String
    Public Property Unita_Misura_Des() As String
        Get
            Return _Unita_Misura_Des
        End Get
        Set(ByVal value As String)
            _Unita_Misura_Des = value
        End Set
    End Property

    Private _Prezzo As Decimal
    Public Property Prezzo() As Decimal
        Get
            Return _Prezzo
        End Get
        Set(ByVal value As Decimal)
            _Prezzo = value
        End Set
    End Property
    Private _Validita_Inizio As String
    Public Property Validita_Inizio() As String
        Get
            Return _Validita_Inizio
        End Get
        Set(ByVal value As String)
            _Validita_Inizio = value
        End Set
    End Property

    Private _Validita_Fine As String
    Public Property Validita_Fine() As String
        Get
            Return _Validita_Fine
        End Get
        Set(ByVal value As String)
            _Validita_Fine = value
        End Set
    End Property


#End Region

    Public Shared Function getListaProdottiCostiMacchina(ByVal piva As String, ByVal mac_cod As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As List(Of Parco_Macchine_Costo)
        Dim corePC As New AgronicaCoreContabDAL.Prodotti_Costi_R
        Dim dtPC As DataTable = corePC.Leggi(piva, _
                                            MACCHINE, _
                                            "", 0, _
                                            mac_cod, _
                                            0, 0, 0, _
                                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                            "", "", _
                                            objParametri)
        Dim ris As New List(Of Parco_Macchine_Costo)
        Dim i As Integer
        For i = 0 To dtPC.Rows.Count - 1
            Dim n As New Parco_Macchine_Costo
            n.ID = dtPC.Rows(i).Item("ID")
            n.Unita_Misura = dtPC.Rows(i).Item("Mezzo")
            n.Unita_Misura_Des = IIf(n.Unita_Misura = 1, "Ettaro", "Ora")
            n.Prezzo = dtPC.Rows(i).Item("Prezzo_Unitario")
            If (dtPC.Rows(i).Item("Validita_Inizio") <> AGRODATAINIZIO) Then
                n.Validita_Inizio = DateToITA(dtPC.Rows(i).Item("Validita_Inizio"))
            Else
                n.Validita_Inizio = ""
            End If

            If (dtPC.Rows(i).Item("Validita_fine") <> AGRODATAFINE) Then
                n.Validita_Fine = DateToITA(dtPC.Rows(i).Item("Validita_fine"))
            Else
                n.Validita_Fine = ""
            End If
            ris.Add(n)
        Next
        Return ris
    End Function


    Public Shared Function DateToITA(ByVal d As Date)
        Return d.Day & "/" & d.Month & "/" & d.Year
    End Function
End Class
