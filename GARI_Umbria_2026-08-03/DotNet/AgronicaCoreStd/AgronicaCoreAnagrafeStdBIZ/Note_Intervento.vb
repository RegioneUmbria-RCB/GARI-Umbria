Imports AgronicaCoreAnagrafeStdDAL
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models

Public Class Note_Intervento
    Inherits BaseBiz

    Public Sub New(dbContext As GiasDbContext)
        MyBase.New(dbContext)
    End Sub

    Public Function LeggiNoteIntervento() As List(Of APP_Note_Intervento)

        Dim xLettura = New Note_Intervento_R()
        Return xLettura.Leggi(dbContext)

    End Function

    Public Sub ScriviNoteIntervento(listNoteIntervento As List(Of APP_Note_Intervento), cancella As Boolean)

        Dim count As Integer = 0
        Dim commit As Boolean = False
        Dim numNoteIntervento As Integer = listNoteIntervento.Count
        Dim commitCount As Integer = 100
        Dim xScrittura = New Note_Intervento_W()

        If cancella Then
            xScrittura.Cancella(dbContext, Nothing)
        End If

        For Each note_intervento In listNoteIntervento
            count += 1
            commit = count Mod commitCount = 0 OrElse count = numNoteIntervento
            xScrittura.Scrivi(dbContext, note_intervento, commit)
        Next

    End Sub

    Public Sub CancellaNoteIntervento(note_intervento As APP_Note_Intervento)

        Dim xScrittura = New Note_Intervento_W()
        xScrittura.Cancella(dbContext, note_intervento)

    End Sub

End Class
