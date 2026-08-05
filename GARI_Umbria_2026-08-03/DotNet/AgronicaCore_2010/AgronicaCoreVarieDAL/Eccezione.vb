Public Class Eccezione
  
    ''' <summary>
    ''' Restituisce sottoforma di stringa la catena di eccezioni che ha generato quella specificata.
    ''' </summary>
    ''' <param name="ex">Eccezione da valutare</param>
    ''' <param name="aCapo">Carattere per mandare a capo le eccezioni</param>
    ''' <param name="source">Indica se considerare anche la provenienza (nome dell'oggetto o dell'applicazione che ha generato l'errore)</param>
    ''' <returns>
    ''' Restituisce sottoforma di stringa la catena di eccezioni che ha generato quella specificata.
    ''' La descrizione dell'eccezione avviene utilizzando la proprità Message dell'oggetto eccezione.
    ''' A richiesta viene anche considerata la proprietà Source dell'oggetto eccezione.
    ''' La catena di eccezioni viene creata valutanto ricorsivamente la proprietà InnerException.
    ''' L'ordine è inverso, ovvero viene descritta l'ultima eccezione sollevata e poi quelle precedenti fino alla prima.
    ''' </returns>
    ''' <remarks>
    ''' Autore: Davide Mercuriali<br/>
    ''' Data creazione: 23/08/2005
    ''' </remarks>
    Public Shared Function CatenaEccezioni(ByVal ex As Exception, Optional ByVal aCapo As String = vbCrLf, Optional ByVal source As Boolean = False) As String

        Return CatenaEccezioniRicorsiva(ex, aCapo, source)

    End Function

    Private Shared Function CatenaEccezioniRicorsiva(ByVal ex As Exception, ByVal aCapo As String, ByVal source As Boolean) As String
        ' ***************************************************************************
        ' Scopo:        mostrare la catena di eccezioni che ha generato quella specificata
        ' Risultato: 
        ' ***************************************************************************
        Dim result As String = Nothing
        If Not ex Is Nothing Then
            Dim innerResult As String
            result = ex.Message & aCapo & ex.StackTrace.Replace(" in ", aCapo & "in ")
            If source Then
                result &= " [" & ex.Source & "]"
            End If
            innerResult = CatenaEccezioniRicorsiva(ex.InnerException, aCapo, source)
            If Not innerResult Is Nothing Then
                result &= aCapo & aCapo & innerResult
            End If
        End If
        Return result
    End Function

End Class

 