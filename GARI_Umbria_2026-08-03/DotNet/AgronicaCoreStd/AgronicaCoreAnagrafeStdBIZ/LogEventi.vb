Imports AgronicaCoreAnagrafeStdDAL
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models

Public Class LogEventi
    Inherits BaseBiz

    Public Sub New(dbContext As GiasDbContext)
        MyBase.New(dbContext)
    End Sub

    Public Function LeggiEventi() As List(Of APP_LogEventi)

        Dim xLettura = New LogEventi_R()
        Return xLettura.Leggi(dbContext, "")

    End Function

    Public Function LeggiEventiDaInviare() As List(Of APP_LogEventi)

        Dim xLettura = New LogEventi_R()
        Return xLettura.LeggiEventiDaInviare(dbContext)

    End Function

    Public Function LeggiUltimoEvento(Evento As String) As APP_LogEventi

        Dim xLettura = New LogEventi_R()
        Return xLettura.LeggiUltimoEvento(dbContext, Evento)

    End Function

    Public Sub ScriviEventi(listEventi As List(Of APP_LogEventi), cancella As Boolean)

        Dim count As Integer = 0
        Dim commit As Boolean = False
        Dim numEventi As Integer = listEventi.Count
        Dim commitCount As Integer = 100
        Dim xScrittura = New LogEventi_W()

        If cancella Then
            xScrittura.Cancella(dbContext, Nothing)
        End If

        For Each evento In listEventi
            count += 1
            commit = count Mod commitCount = 0 OrElse count = numEventi
            xScrittura.Scrivi(dbContext, evento, commit)
        Next

    End Sub

    Public Sub ScriviEvento(evento As APP_LogEventi, username As String)

        Dim xScrittura = New LogEventi_W()
        ScritturaDatiComuni(evento, username)
        xScrittura.Scrivi(dbContext, evento, True)

    End Sub

    Public Sub CancellaEvento(evento As APP_LogEventi)

        Dim xScrittura = New LogEventi_W()
        xScrittura.Cancella(dbContext, evento)

    End Sub

    Public Sub MarcaEventiInviati(listEventi As List(Of APP_LogEventi))

        Dim xScrittura = New LogEventi_W()
        For Each evento In listEventi
            xScrittura.Inviato(dbContext, evento)
        Next

    End Sub

    Public Sub CancellaEventi(listEventi As List(Of APP_LogEventi), ultimoEvento As Boolean)

        Dim xScrittura = New LogEventi_W()
        Dim index As Integer = 0
        For Each evento In listEventi
            index += 1
            If index = listEventi.Count Then
                xScrittura.Inviato(dbContext, evento)
            Else
                xScrittura.Cancella(dbContext, evento)
            End If
        Next

    End Sub

End Class
