Imports System.Configuration
Imports AgronicaCoreModelsSTD.exceptions
Imports Newtonsoft.Json

Public Class Gestione_Eccezioni_2015
    Public Shared Function MessaggioCompletoDataEccezione(ByVal ex As Exception, ByVal IncludiStackTrace As Boolean, Optional ByVal aCapo As String = vbCrLf, Optional ByVal source As Boolean = False) As String
        Dim stringErrore = CatenaEccezioni(ex, IncludiStackTrace, aCapo, source)
        If stringErrore.Contains("Page_Load") AndAlso Not restituisciEccezione() Then
            Return "Error"
        Else
            stringErrore = "###Errore### " & stringErrore
        End If
        Return stringErrore
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
        'If Not restituisciEccezione() Then
        '    Return "Error"
        'End If
        If ex IsNot Nothing Then
            Dim innerResult As String
            result = ex.Message
            If IncludiStackTrace AndAlso ex.StackTrace IsNot Nothing Then
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

    Public Shared Function ErrorHandler(ByVal ex As Exception) As ErroreGias
        Dim erroreGias As New ErroreGias With {
            .ex = JsonConvert.SerializeObject(ex),
            .messaggio = ex.Message,
            .severity = ErroreGias_Severity.Bloccante
        }

        If TypeOf ex Is AgroEccezioni_LoginFallito_Exception Then
            erroreGias.tipo = ErroreGias_Tipo.LoginFallito

        ElseIf TypeOf ex Is AgroEccezioni_ParticellaConLegami_Exception Then
            erroreGias.tipo = ErroreGias_Tipo.ParticellaConLegami

        ElseIf TypeOf ex Is GiasException Then
            erroreGias.tipo = ErroreGias_Tipo.Generico

        Else
            erroreGias.tipo = ErroreGias_Tipo.NonGestito
            If Not restituisciEccezione() Then
                erroreGias.ex = "Error"
                erroreGias.messaggio = "Error"
            End If
        End If

        Return erroreGias

    End Function

End Class
