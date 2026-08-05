
Imports System.Configuration

Public Class Gestione_Eccezioni

    Public Shared Function MessaggioCompletoDataEccezione(ByVal ex As Exception, ByVal includiStackTrace As Boolean, Optional ByVal aCapo As String = vbCrLf, Optional ByVal source As Boolean = False) As String
        Dim stringErrore = CatenaEccezioni(ex, includiStackTrace, aCapo, source)
        If stringErrore.Contains("Page_Load") AndAlso Not restituisciEccezione() Then
            Return "Error"
        Else
            stringErrore = "###Errore### " & stringErrore
        End If
        Return stringErrore
    End Function

    Private Shared Function CatenaEccezioni(ByVal ex As Exception, ByVal includiStackTrace As Boolean, Optional ByVal aCapo As String = vbCrLf, Optional ByVal source As Boolean = False) As String

        Return CatenaEccezioniRicorsiva(ex, includiStackTrace, aCapo, source)

    End Function

    Private Shared Function CatenaEccezioniRicorsiva(ByVal ex As Exception, ByVal includiStackTrace As Boolean, ByVal aCapo As String, ByVal source As Boolean) As String
        ' ***************************************************************************
        ' Scopo:        mostrare la catena di eccezioni che ha generato quella specificata
        ' Risultato: 
        ' ***************************************************************************
        Dim result As String = Nothing
        If ex IsNot Nothing Then
            Dim innerResult As String
            result = ex.Message
            If includiStackTrace Then
                result &= ex.StackTrace.Replace(" in ", aCapo & "in ")
            End If
            If source Then
                result &= " [" & ex.Source & "]"
            End If

            result &= "------------------------------------" & aCapo
            innerResult = CatenaEccezioniRicorsiva(ex.InnerException, includiStackTrace, aCapo, source)
            If innerResult IsNot Nothing Then
                result &= aCapo & aCapo & innerResult
            End If
        End If
        Return result
    End Function

    Private Shared Function restituisciEccezione() As Boolean
        If ConfigurationManager.AppSettings("RestituisciEccezioniGeneriche") Is Nothing Then
            Return False
        End If
        If CStr(ConfigurationManager.AppSettings("RestituisciEccezioniGeneriche")) = "true" Then
            Return False
        End If
        If CStr(ConfigurationManager.AppSettings("RestituisciEccezioniGeneriche")) = "false" Then
            Return True
        End If

        Return False
    End Function

End Class

