Imports AgronicaCoreAnagrafeStdDAL
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore.Storage


Public Class SpecieVegetaliXStadiCrescita
    Inherits BaseBiz


    Public Sub New(dbContext As GiasDbContext)
        MyBase.New(dbContext)
    End Sub


    ''' <summary>
    ''' legge una lista di Stadi Crescita filtrata per specie e per BBCH
    ''' </summary>
    ''' <param name="dbContext"></param>    
    ''' <returns></returns>
    Public Function EstraiListaSpecieVegetaliXStadiCrescitaFiltrataPerSpecieBBCH(ByVal Veg_Cod As Integer, ByVal BBCH As Integer) As List(Of AgronicaCoreModelloSTD.SpecieVegetaliXStadiCrescita)

        Dim leggi As New SpecieVegetaliXStadiCrescita_R()
        Return leggi.EstraiListaSpecieVegetaliXStadiCrescitaFiltrataPerSpecieBBCH(dbContext, Veg_Cod, BBCH)

    End Function


    Public Sub ScriviSpecieVegetaliXStadiCrescita(listSpecieVegetaliXStadiCrescita As List(Of APP_SpecieVegetaliXStadiCrescita), cancella As Boolean)

        Dim count As Integer = 0
        Dim commit As Boolean = False
        Dim numSpecieVegetaliXStadiCrescita As Integer = listSpecieVegetaliXStadiCrescita.Count
        Dim commitCount As Integer = 100
        Dim xScrittura = New SpecieVegetaliXStadiCrescita_W()

        If cancella Then
            xScrittura.Cancella(dbContext, Nothing)
        End If

        For Each SpecieVegetaliXStadiCrescita In listSpecieVegetaliXStadiCrescita
            count += 1
            commit = count Mod commitCount = 0 OrElse count = numSpecieVegetaliXStadiCrescita
            xScrittura.Scrivi(dbContext, SpecieVegetaliXStadiCrescita, commit)
        Next

    End Sub

    Public Sub CancellaSpecieVegetaliXStadiCrescita(SpecieVegetaliXStadiCrescita As APP_SpecieVegetaliXStadiCrescita)

        Dim xScrittura = New SpecieVegetaliXStadiCrescita_W()
        xScrittura.Cancella(dbContext, SpecieVegetaliXStadiCrescita)

    End Sub

End Class
