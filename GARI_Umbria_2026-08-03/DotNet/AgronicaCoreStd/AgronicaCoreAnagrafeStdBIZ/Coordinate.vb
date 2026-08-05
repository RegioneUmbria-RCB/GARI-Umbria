Imports AgronicaCoreAnagrafeStdDAL
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models

Public Class Coordinate
    Inherits BaseBiz

    Public Sub New(dbContext As GiasDbContext)
        MyBase.New(dbContext)
    End Sub

    Public Function LeggiCoordinate() As List(Of APP_GIS)

        Dim xLettura = New Coordinate_R()
        Return xLettura.Leggi(dbContext)

    End Function

    Public Function LeggiUltimaCoordinata() As APP_GIS

        Dim xLettura = New Coordinate_R()
        Return xLettura.LeggiUltimaCoordinata(dbContext)

    End Function

    Public Sub ScriviCoordinate(listCoordinate As List(Of APP_GIS), cancella As Boolean)

        Dim count As Integer = 0
        Dim commit As Boolean = False
        Dim numCoordinate As Integer = listCoordinate.Count
        Dim commitCount As Integer = 100
        Dim xScrittura = New Coordinate_W()

        If cancella Then
            xScrittura.Cancella(dbContext, Nothing)
        End If

        For Each coordinata In listCoordinate
            count += 1
            commit = count Mod commitCount = 0 OrElse count = numCoordinate
            xScrittura.Scrivi(dbContext, coordinata, commit)
        Next

    End Sub

    Public Sub ScriviCoordinata(coordinata As APP_GIS, username As String)

        Dim xScrittura = New Coordinate_W()
        ScritturaDatiComuni(coordinata, username)
        xScrittura.Scrivi(dbContext, coordinata, True)

    End Sub

    Public Sub CancellaCoordinata(coordinata As APP_GIS)

        Dim xScrittura = New Coordinate_W()
        xScrittura.Cancella(dbContext, coordinata)

    End Sub

    Public Sub CancellaCoordinate()

        Dim xScrittura = New Coordinate_W()
        xScrittura.Cancella(dbContext, Nothing)

    End Sub

    Public Sub CancellaCoordinate(listCoordinate As List(Of APP_GIS))

        Dim xScrittura = New Coordinate_W()
        For Each coordinata In listCoordinate
            xScrittura.Cancella(dbContext, coordinata)
        Next

    End Sub
End Class
