Public Class EquipaggiamentiCUAA
    Public Property nextKey As String
    Public Property records As List(Of AgronicaCoreDTOStd.InData.Anagrafica.AnagraficaWrapper(Of AgronicaCoreDTOStd.InData.Demetra.Equipaggiamento))
    Public Property count As Integer

    Public Sub New()
        records = New List(Of AgronicaCoreDTOStd.InData.Anagrafica.AnagraficaWrapper(Of AgronicaCoreDTOStd.InData.Demetra.Equipaggiamento))
    End Sub
End Class

Public Class EquipaggiamentoCUAASync
    Inherits AgronicaCoreDTOStd.InData.Anagrafica.AnagraficaWrapper(Of AgronicaCoreDTOStd.InData.Demetra.Equipaggiamento)
    Public Property cuaa As String
    Public Property tipo_modifica As String
    Public Property data_eliminazione As String
End Class

Public Class EquipaggiamentoCUAAChangeLog
    Public Property nextKey As String
    Public Property records As List(Of EquipaggiamentoCUAASync)
    Public Property count As Integer

    Public Sub New()
        records = New List(Of EquipaggiamentoCUAASync)
    End Sub
End Class
