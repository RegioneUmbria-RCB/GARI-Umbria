Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class Parco_Macchine_Manutenzione

#Region ""
    Private _ID_Agenda As Integer
    Public Property ID_Agenda() As Integer
        Get
            Return _ID_Agenda
        End Get
        Set(ByVal value As Integer)
            _ID_Agenda = value
        End Set
    End Property


    Private _Data As String
    Public Property Data() As String
        Get
            Return _Data
        End Get
        Set(ByVal value As String)
            _Data = value
        End Set
    End Property
    Private _Movimento_Des As String
    Public Property Movimento_Des() As String
        Get
            Return _Movimento_Des
        End Get
        Set(ByVal value As String)
            _Movimento_Des = value
        End Set
    End Property

    Private _Costo As Decimal
    Public Property Costo() As Decimal
        Get
            Return _Costo
        End Get
        Set(ByVal value As Decimal)
            _Costo = value
        End Set
    End Property

    Private _N_Certificato As String
    Public Property N_Certificato() As String
        Get
            Return _N_Certificato
        End Get
        Set(ByVal value As String)
            _N_Certificato = value
        End Set
    End Property


#End Region

    Public Shared Function getListaProdottiManutenzioniMacchinari(ByVal mac_cod As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As List(Of Parco_Macchine_Manutenzione)

        Dim DT_Manutenzioni As DataTable
        Dim objMovDet As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
        DT_Manutenzioni = objMovDet.Leggi("", 0, 0, 0, 0, _
                                               MACCHINE, 0, _
                                               mac_cod, _
                                               CAU_MANUTENZIONE_PARCOMACCHINE, _
                                               0, 0, 0, 0, 0, 0, _
                                               AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta, _
                                               "", "Data_Movimento DESC", objParametri)


        Dim ris As New List(Of Parco_Macchine_Manutenzione)
        Dim i As Integer
        For i = 0 To DT_Manutenzioni.Rows.Count - 1
            Dim n As New Parco_Macchine_Manutenzione
            n.ID_Agenda = DT_Manutenzioni.Rows(i).Item("ID_Agenda")
            n.Movimento_Des = DT_Manutenzioni.Rows(i).Item("des_lib")
            n.Costo = 0 'anche nel gias vecchio il costo è sempre a 0
            n.Data = DT_Manutenzioni.Rows(i).Item("Data_Movimento")
            n.N_Certificato = DT_Manutenzioni.Rows(i).Item("Doc_Numero")
            ris.Add(n)
        Next
        Return ris
    End Function

End Class
