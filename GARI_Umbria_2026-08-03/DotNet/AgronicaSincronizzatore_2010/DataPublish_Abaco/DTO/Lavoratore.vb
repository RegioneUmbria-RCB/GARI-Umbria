Public Class LavoratoriCUAA
    Public Property nextKey As String
    Public Property records As List(Of AgronicaCoreDTOStd.InData.Anagrafica.AnagraficaWrapper(Of AgronicaCoreDTOStd.InData.Demetra.Contatto))
    Public Property count As Integer

    Public Sub New()
        records = New List(Of AgronicaCoreDTOStd.InData.Anagrafica.AnagraficaWrapper(Of AgronicaCoreDTOStd.InData.Demetra.Contatto))
    End Sub
End Class

Public Class LavoratoriCUAASync
    Inherits AgronicaCoreDTOStd.InData.Anagrafica.AnagraficaWrapper(Of AgronicaCoreDTOStd.InData.Demetra.Contatto)
    Public Property cuaa As String
    Public Property tipo_modifica As String
    Public Property data_eliminazione As String
End Class

Public Class LavoratoriCUAAChangeLog
    Public Property nextKey As String
    Public Property records As List(Of LavoratoriCUAASync)
    Public Property count As Integer

    Public Sub New()
        records = New List(Of LavoratoriCUAASync)
    End Sub
End Class
