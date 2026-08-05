Imports AgronicaCoreAnagrafeStdDAL
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models

Public Class CentriAziendali
    Inherits BaseBiz

    Public Sub New(dbContext As GiasDbContext)
        MyBase.New(dbContext)
    End Sub

    Public Function EstraiListaCentriAziendali(Piva As String) As List(Of AgronicaCoreModelloSTD.Centri_Aziendali)

        Dim xLettura As New CentriAziendali_R()
        Return xLettura.EstraiListaCentriAziendali(dbContext, Piva)

    End Function

    Public Sub ScriviCentriAziendali(listCentriAziendali As List(Of APP_Centri_Aziendali), piva As String, cancella As Boolean)

        Dim count As Integer = 0
        Dim commit As Boolean = False
        Dim numCentri As Integer = listCentriAziendali.Count
        Dim commitCount As Integer = 100
        Dim xScrittura = New CentriAziendali_W()

        If cancella Then
            xScrittura.Cancella(dbContext, Nothing, piva)
        End If

        For Each centro In listCentriAziendali
            count += 1
            commit = count Mod commitCount = 0 OrElse count = numCentri
            xScrittura.Scrivi(dbContext, centro, commit)
        Next

    End Sub

End Class
