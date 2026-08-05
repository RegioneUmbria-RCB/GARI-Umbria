Imports AgronicaCoreAnagrafeStdDAL
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models

Public Class Imprese
    Inherits BaseBiz

    Public Sub New(dbContext As GiasDbContext)
        MyBase.New(dbContext)
    End Sub

    Public Function LeggiImprese(Piva As String) As List(Of APP_Imprese)

        Dim xLettura = New Imprese_R()
        Return xLettura.Leggi(dbContext, Piva)

    End Function

    Public Function CercaImprese(Search As String) As List(Of APP_Imprese)

        Dim xLettura = New Imprese_R()
        Return xLettura.Cerca(dbContext, Search)

    End Function

    Public Sub ScriviImpresa(impresa As APP_Imprese)

        Dim xScrittura = New Imprese_W()
        xScrittura.Scrivi(dbContext, impresa)

    End Sub

    Public Sub ScriviImprese(listImprese As List(Of APP_Imprese), cancella As Boolean)

        Dim count As Integer = 0
        Dim commit As Boolean = False
        Dim numImprese As Integer = listImprese.Count
        Dim commitCount As Integer = 100
        Dim xScrittura = New Imprese_W()

        Using transaction = dbContext.Database.BeginTransaction()

            Try

                If cancella Then
                    xScrittura.Cancella(dbContext, Nothing)
                End If

                For Each impresa In listImprese
                    count += 1
                    commit = count Mod commitCount = 0 OrElse count = numImprese
                    xScrittura.Scrivi(dbContext, impresa, commit)
                Next

                transaction.Commit()

            Catch ex As Exception
                transaction.Rollback()
            End Try
        End Using

    End Sub

    Public Sub CancellaImprese(impresa As APP_Imprese)

        Dim xScrittura = New Imprese_W()
        xScrittura.Cancella(dbContext, impresa)

    End Sub
End Class
