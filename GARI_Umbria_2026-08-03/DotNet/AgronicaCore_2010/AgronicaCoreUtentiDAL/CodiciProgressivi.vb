Imports AgronicaCoreDataProvider

Public Class CodiciProgressivi

    Public Shared Sub Trova_Base_e_Top(ByRef progressivo As Integer,
                                       ByRef baseCode As Integer,
                                       ByRef topCode As Integer,
                                       ByRef objParametriUtenti As AgronicaCoreParametri,
                                       ByRef objParametriServer As AgronicaCoreParametri)

        Dim obj As New AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R
        Dim dt As DataTable
        dt = obj.Leggi("", "", objParametriUtenti)
        progressivo = dt.Rows(0).Item("progressivogias")

        'ricavo base e top 
        Dim objAgroSeq As New AgronicaCoreDataProvider.Agro_Sequenze
        objAgroSeq.Calcola_BaseCode(progressivo, topCode, baseCode, objParametriServer)
    End Sub

End Class
