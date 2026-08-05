Imports System.Globalization
Imports System.Text


'#######################################################
'#############          Request            #############
'#######################################################

Public Class Analisi_API_Risultato_Analisi

    Public Property listaAnalisi As List(Of Analisi_API_Analisi)

    Public Function FormalValidation(ByRef errore As StringBuilder) As Boolean
        Dim erroriInterni As New StringBuilder

        Try

            If listaAnalisi Is Nothing Then
                erroriInterni.Append("[listaAnalisi] is required. ")
            ElseIf listaAnalisi.Count = 0 Then
                erroriInterni.Append("[listaAnalisi] is empty. ")
            Else

                For i As Integer = 0 To listaAnalisi.Count - 1
                    Dim listaAnalisiItemIsValid As Boolean = listaAnalisi(i).FormalValidation(erroriInterni, i)
                Next

            End If
            

        Catch ex As Exception
            errore.AppendLine("Exception validating: " & ex.Message & If(IsNothing(ex.InnerException), ". ", " [ " & ex.InnerException.Message & "]. "))
        End Try

        If erroriInterni.Length <> 0 Then
            errore.AppendLine(String.Format("Data is invalid: ({0}). ", erroriInterni.ToString()))
            Return False
        Else
            Return True
        End If

    End Function

End Class

Public Class Analisi_API_Analisi

    Public Property informazioniGenerali As Analisi_API_InformazioniGenerali
    Public Property esitoAnalisi As List(Of Analisi_API_EsitoAnalisi)
    Public Property allegati As List(Of Analisi_API_Allegati)

    Public Function FormalValidation(ByRef errore As StringBuilder, Optional ByVal numberItemInList As Integer = -1) As Boolean

        Dim erroriInterni As New StringBuilder
        Dim infoGeneraliIsValid As Boolean = False

        Try

            If informazioniGenerali Is Nothing Then
                erroriInterni.Append("[informazioniGenerali] is required. ")
            Else
                infoGeneraliIsValid = informazioniGenerali.FormalValidation(erroriInterni)
            End If

            'esitoAnalisi e allegati potrebbero essere vuoti, se non sono state riscontrate le sostanze o non ci sono pdf

            If Not esitoAnalisi Is Nothing AndAlso esitoAnalisi.Count > 0 Then

                For i As Integer = 0 To esitoAnalisi.Count - 1
                    Dim esitoAnalisiItemIsValid As Boolean = esitoAnalisi(i).FormalValidation(erroriInterni, i)
                Next

            End If

            If Not allegati Is Nothing AndAlso allegati.Count > 0 Then

                For i As Integer = 0 To allegati.Count - 1
                    Dim allegatiItemIsValid As Boolean = allegati(i).FormalValidation(erroriInterni, i)
                Next

            End If


        Catch ex As Exception
            errore.AppendLine("Exception validating [analisi]: " & ex.Message & If(IsNothing(ex.InnerException), ". ", " [ " & ex.InnerException.Message & "]. "))
        End Try

        If erroriInterni.Length <> 0 Then
            If numberItemInList < 0 Then
                errore.AppendLine(String.Format("Data of [analisi] is invalid: ({0}). ", erroriInterni.ToString()))
            Else
                errore.AppendLine(String.Format("Data of item [{0}] in array [listaAnalisi] is invalid: ({1}). ",
                                                numberItemInList, erroriInterni.ToString()))
            End If
            Return False
        Else
            Return True
        End If

    End Function

End Class

Public Class Analisi_API_InformazioniGenerali
    Public Property pivaSuperUser As String
    Public Property chiave As String
    Public Property codiceAnalisiLaboratorio As String
    Public Property dataInizioAnalisi As String
    Public Property dataFineAnalisi As String

    Private Const FORMATO_DATA_JSON As String = "yyyy-MM-dd'T'HH:mm:ss.FFFFFFF"

    Public Shared Function ConvertiData(ByVal dataStrJson As String) As DateTime
        Return DateTime.ParseExact(dataStrJson, FORMATO_DATA_JSON,
                                   CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal)
    End Function

    Public Shared Function TryConvertiData(ByVal dataStrJson As String) As Boolean
        Dim dateValue As DateTime
        Return DateTime.TryParseExact(dataStrJson, FORMATO_DATA_JSON,
                                      CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, dateValue)
    End Function

    Public Function FormalValidation(ByRef errore As StringBuilder) As Boolean

        Dim erroriInterni As New StringBuilder
        
        Try

            If pivaSuperUser Is Nothing OrElse Trim(pivaSuperUser) = "" Then
                erroriInterni.Append("[pivaSuperUser] is required. ")
            End If

            If chiave Is Nothing OrElse Trim(chiave) = "" Then
                erroriInterni.Append("[chiave] is required. ")
            End If

            If codiceAnalisiLaboratorio Is Nothing OrElse Trim(codiceAnalisiLaboratorio) = "" Then
                erroriInterni.Append("[codiceAnalisiLaboratorio] is required. ")
            End If

            If dataInizioAnalisi Is Nothing OrElse Trim(dataInizioAnalisi) = "" Then
                erroriInterni.Append("[dataInizioAnalisi] is required. ")
            Else If Not TryConvertiData(dataInizioAnalisi) Then
                erroriInterni.Append("[dataInizioAnalisi] is not a valid date or is not in the expected format. ")
            End If

            If dataFineAnalisi Is Nothing OrElse Trim(dataFineAnalisi) = "" Then
                erroriInterni.Append("[dataFineAnalisi] is required. ")
            Else If Not TryConvertiData(dataFineAnalisi) Then
                erroriInterni.Append("[dataFineAnalisi] is not a valid date or is not in the expected format. ")
            End If


        Catch ex As Exception
            errore.AppendLine("Exception validating [informazioniGenerali]: " & ex.Message & If(IsNothing(ex.InnerException), ". ", " [ " & ex.InnerException.Message & "]. "))
        End Try

        If erroriInterni.Length <> 0 Then
            errore.AppendLine(String.Format("Data of [informazioniGenerali] is invalid: ({0}). ", erroriInterni.ToString()))
            Return False
        Else
            Return True
        End If

    End Function

End Class

Public Class Analisi_API_EsitoAnalisi
    Public Property tipo As String
    Public Property codiceParametro As String
    Public Property descrizioneParametro As String
    Public Property udmCod As String
    Public Property udmSim As String
    Public Property Qta As String

    Public Function FormalValidation(ByRef errore As StringBuilder, Optional ByVal numberItemInList As Integer = -1) As Boolean

        Dim erroriInterni As New StringBuilder

        Try

            If tipo Is Nothing OrElse Trim(tipo) = "" Then
                erroriInterni.Append("[tipo] is required. ")
            ElseIf Not IsNumeric(tipo) Then
                erroriInterni.Append("[tipo] is not numeric. ")
            End If

            If codiceParametro Is Nothing OrElse Trim(codiceParametro) = "" Then
                erroriInterni.Append("[codiceParametro] is required. ")
            End If

            'descrizioneParametro è facoltativo

            If udmCod Is Nothing OrElse Trim(udmCod) = "" Then
                erroriInterni.Append("[udmCod] is required. ")
            End If

            If udmSim Is Nothing OrElse Trim(udmSim) = "" Then
                erroriInterni.Append("[udmSim] is required. ")
            End If

            If qta Is Nothing OrElse Trim(qta) = "" Then
                erroriInterni.Append("[qta] is required. ")
            ElseIf Not IsNumeric(qta) Then
                erroriInterni.Append("[qta] is not numeric. ")
                'TODO: VALIDAZIONE = verificare esattamente se è nel formato richiesto
            End If

        Catch ex As Exception
            errore.AppendLine("Exception validating [esitoAnalisi]: " & ex.Message & If(IsNothing(ex.InnerException), ". ", " [ " & ex.InnerException.Message & "]. "))
        End Try

        If erroriInterni.Length <> 0 Then
            If numberItemInList < 0 Then
                errore.AppendLine(String.Format("Data of [esitoAnalisi] is invalid: ({0}). ", erroriInterni.ToString()))
            Else
                errore.AppendLine(String.Format("Data of item [{0}] in array [esitoAnalisi] is invalid: ({1}). ",
                                                numberItemInList, erroriInterni.ToString()))
            End If
            Return False
        Else
            Return True
        End If

    End Function

End Class

Public Class Analisi_API_Allegati
    Public Property nomeFileAllegato As String
    Public Property fileBase64String As String

    Public Function FormalValidation(ByRef errore As StringBuilder, Optional ByVal numberItemInList As Integer = -1) As Boolean

        Dim erroriInterni As New StringBuilder

        Try

            If nomeFileAllegato Is Nothing OrElse Trim(nomeFileAllegato) = "" Then
                erroriInterni.Append("[nomeFileAllegato] is required. ")
            End If

            If fileBase64String Is Nothing OrElse Trim(fileBase64String) = "" Then
                erroriInterni.Append("[fileBase64String] is required. ")
                'TODO: VALIDAZIONE = verificare se è effettivamente Base64
            End If

        Catch ex As Exception
            errore.AppendLine("Exception validating [allegati]: " & ex.Message & If(IsNothing(ex.InnerException), ". ", " [ " & ex.InnerException.Message & "]. "))
        End Try

        If erroriInterni.Length <> 0 Then
            If numberItemInList < 0 Then
                errore.AppendLine(String.Format("Data of [allegati] is invalid: ({0}). ", erroriInterni.ToString()))
            Else
                errore.AppendLine(String.Format("Data of item [{0}] in array [allegati] is invalid: ({1}). ",
                                                numberItemInList, erroriInterni.ToString()))
            End If
            Return False
        Else
            Return True
        End If

    End Function

End Class


'#######################################################
'#############          Response           #############
'#######################################################

Public Class Analisi_API_Risultati_Response
    Public Property esitoGlobale As Boolean
    Public Property message As String
    Public Property listaAnalisiElaborata As List(Of Analisi_API_Risultato_Dettaglio_Response)
End Class

Public Class Analisi_API_Risultato_Dettaglio_Response
    Public Property pivaSuperUser As String
    Public Property chiave As String
    Public Property messaggioErrore As String
End Class