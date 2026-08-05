Public Class Gestione_Eccezioni_2015
    Public Shared Function MessaggioCompletoDataEccezione(ByVal ex As Exception, ByVal IncludiStackTrace As Boolean, Optional ByVal aCapo As String = vbCrLf, Optional ByVal source As Boolean = False) As String
        Return CatenaEccezioni(ex, IncludiStackTrace, aCapo, source)
    End Function

    Private Shared Function CatenaEccezioni(ByVal ex As Exception, ByVal IncludiStackTrace As Boolean, Optional ByVal aCapo As String = vbCrLf, Optional ByVal source As Boolean = False) As String

        Return CatenaEccezioniRicorsiva(ex, IncludiStackTrace, aCapo, source)

    End Function

    Private Shared Function CatenaEccezioniRicorsiva(ByVal ex As Exception, ByVal IncludiStackTrace As Boolean, ByVal aCapo As String, ByVal source As Boolean) As String
        ' ***************************************************************************
        ' Scopo:        mostrare la catena di eccezioni che ha generato quella specificata
        ' Risultato: 
        ' ***************************************************************************
        Dim result As String = Nothing
        If ex IsNot Nothing Then
            Dim innerResult As String
            result = ex.Message
            If IncludiStackTrace Then
                result &= ex.StackTrace.Replace(" in ", aCapo & "in ")
            End If
            If source Then
                result &= " [" & ex.Source & "]"
            End If

            result &= "------------------------------------" & aCapo
            innerResult = CatenaEccezioniRicorsiva(ex.InnerException, IncludiStackTrace, aCapo, source)
            If innerResult IsNot Nothing Then
                result &= aCapo & aCapo & innerResult
            End If
        End If
        Return result
    End Function
End Class
