Public Class Utility

    ' Delegate declaration.
    Public Delegate Sub EventoErroreServizioHandler(ByVal sender As Object, ByVal e As EventoErroreServizio)
    Public Delegate Sub EventoServizioHandler(ByVal sender As Object, ByVal e As EventoServizio)


    Public Shared Function estraiMessaggiEccezioni(ByVal ex As Exception) As String
        Dim s As String = ex.Message
        While (ex.InnerException IsNot Nothing)
            ex = ex.InnerException
            s &= vbCrLf & ex.Message & vbCrLf & vbCrLf & vbCrLf & "Immagine dello stack: " & vbCrLf & ex.StackTrace
        End While
        Return s
    End Function



    ''' <summary>
    ''' Scrive il messaggio specificato nel file di log del servizio.
    ''' Racchiudo la scrittura nel log degli eventi di windows dentro a una funzione che revede sempre un try catch
    ''' per prevedere i casi in cui il log degli eventi è pieno
    ''' </summary>
    ''' <param name="log">oggetto log per scrivere</param>
    ''' <param name="msg">Messaggio da scrivere</param>
    ''' <param name="tipoMessaggio">Tipologia del messaggio</param>
    Public Shared Sub ScriviLogServizio(ByRef log As EventLog, ByVal msg As String, Optional ByVal tipoMessaggio As EventLogEntryType = EventLogEntryType.Information, Optional ByVal id_Evento As Integer = 0)

        Dim id As Integer = id_Evento
        If tipoMessaggio = EventLogEntryType.Error And id_Evento = 0 Then            
            id = ID_eventi.ErroreGenerico
        End If

        Try
            log.WriteEntry(msg, tipoMessaggio, id)
        Catch ex As Exception
            'questo è per impedire che il servizio di importazione si blocchi se il log degli eventi è pieno
        End Try
    End Sub



    Public Class EventoErroreServizio
        Inherits EventArgs
        Private _Descrizione As String

        'Constructor.
        '
        Public Sub New(ByVal TestoDescrizione As String)
            Me._Descrizione = TestoDescrizione
        End Sub

        Public ReadOnly Property Descrizione() As String
            Get
                Return _Descrizione
            End Get
        End Property

    End Class



    Public Class EventoServizio
        Inherits EventArgs
        Private _Descrizione As String

        'Constructor.
        '
        Public Sub New(ByVal TestoDescrizione As String)
            Me._Descrizione = TestoDescrizione
        End Sub

        Public ReadOnly Property Descrizione() As String
            Get
                Return _Descrizione
            End Get
        End Property

    End Class


    Public Enum ID_eventi
        ErroreGenerico = 111

        Stato_Iniziale = 0

        Task_Avviato = 1
        Task_Terminato_Correttamente = 2
        Task_Saltato = 3
        Task_Sospeso = 3
        Task_Terminato_Con_Errori = 10


        ServizioAvviato = 50
        servizioArrestato = 100

        ErroreInAvvioServizio = 51

    End Enum

End Class
