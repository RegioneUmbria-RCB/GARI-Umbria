Imports System.Text
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Interscambio_Util.Util

Public Class InterscambioMail

    Public Shared Sub MailLogCondizionale(ByRef mailMsg As StringBuilder, ByVal msg As String, ByVal scrivi As Boolean)
        If scrivi = True Then
            mailMsg.AppendLine(msg)
        End If
    End Sub

    Public Shared Sub CreaRiepilogoMailImport(ByRef summaryMail As StringBuilder, ByVal tipoXml As enum_TipoXml, ByVal fileTrovati As Integer, ByVal fileOk As Integer, ByRef fileOkxType As Dictionary(Of String, DetFile), ByVal fileError As Integer, ByVal fileBypassData As Integer, ByVal dataFrom As Date, ByVal dataTo As Date, Optional ByVal msgMappingBancaDati As String = "")

        summaryMail.AppendLine("Importazione  -  Data: " & Now.ToLongDateString & " " & Now.ToLongTimeString & vbCrLf)
        summaryMail.AppendLine("Filtro date DA: " & dataFrom.ToShortDateString & " - A: " & dataTo.ToShortDateString & vbCrLf)

        'If msgMappingBancaDati <> "" Then
        '    summaryMail.AppendLine(msgMappingBancaDati & vbCrLf)
        'End If

        summaryMail.AppendLine("*********************************")
        summaryMail.AppendLine("*   File Trovati " & vbTab & " = " & vbTab & vbTab & fileTrovati)
        summaryMail.AppendLine("*   File Ok-Imp " & vbTab & " = " & vbTab & vbTab & fileOk)
        summaryMail.AppendLine("*   File Errori " & vbTab & " = " & vbTab & vbTab & fileError)
        summaryMail.AppendLine("*   File Saltati " & vbTab & " = " & vbTab & vbTab & fileBypassData)
        summaryMail.AppendLine("*********************************")
        summaryMail.AppendLine("*   " & tipoXml.ToString & " Importati per File: ")
        summaryMail.AppendLine("*")

        If Not fileOkxType Is Nothing Then
            For Each pair In fileOkxType
                Dim val As DetFile = pair.value
                summaryMail.AppendLine("*   ***** File [" & CStr(pair.Key) & "] ***** ")
                summaryMail.AppendLine("*")
                summaryMail.AppendLine("*   " & tipoXml.ToString & " Presenti " & vbTab & " = " & vbTab & vbTab & CStr(val.NumTotali))
                summaryMail.AppendLine("*   " & tipoXml.ToString & " Aggiunti " & vbTab & " = " & vbTab & vbTab & CStr(val.NumInseriti))
                summaryMail.AppendLine("*   " & tipoXml.ToString & " Updated " & vbTab & " = " & vbTab & vbTab & CStr(val.NumModificati))
                summaryMail.AppendLine("*   " & tipoXml.ToString & " Errore " & vbTab & " = " & vbTab & vbTab & CStr(val.NumErrori))
                summaryMail.AppendLine("*")
            Next
        End If
        summaryMail.AppendLine("*********************************")

        If msgMappingBancaDati <> "" Then
            summaryMail.Append(vbCrLf)
            summaryMail.AppendLine(msgMappingBancaDati)
        End If

        summaryMail.Append(vbCrLf & vbCrLf)

    End Sub

    Public Shared Sub CreaRiepilogoMailImportMovimenti(ByRef summaryMail As StringBuilder, ByVal fileTrovati As Integer, ByVal fileOk As Integer, ByRef fileOkxType As Hashtable, ByVal fileError As Integer, ByVal fileBypassData As Integer, ByVal fileSospesi As Integer, ByVal dataFrom As Date, ByVal dataTo As Date, Optional ByVal msgMappingBancaDati As String = "")

        summaryMail.AppendLine("Importazione  -  Data: " & Now.ToLongDateString & " " & Now.ToLongTimeString & vbCrLf)
        summaryMail.AppendLine("Filtro date DA: " & dataFrom.ToShortDateString & " - A: " & dataTo.ToShortDateString & vbCrLf)

        'If msgMappingBancaDati <> "" Then
        '    summaryMail.AppendLine(msgMappingBancaDati & vbCrLf)
        'End If

        summaryMail.AppendLine("*********************************")
        summaryMail.AppendLine("*   File Trovati " & vbTab & " = " & vbTab & vbTab & fileTrovati)
        summaryMail.AppendLine("*   File Ok-Imp " & vbTab & " = " & vbTab & vbTab & fileOk)
        summaryMail.AppendLine("*   File Errori " & vbTab & " = " & vbTab & vbTab & fileError)
        summaryMail.AppendLine("*   File Saltati " & vbTab & " = " & vbTab & vbTab & fileBypassData)
        summaryMail.AppendLine("*   File Sospesi " & vbTab & " = " & vbTab & vbTab & fileSospesi)
        summaryMail.AppendLine("*********************************")
        summaryMail.AppendLine("*   File Importati per Tipo: ")
        summaryMail.AppendLine("*")
        For Each pair As DictionaryEntry In fileOkxType
            summaryMail.AppendLine("*   Tipo " & CStr(pair.Key) & " = " & CStr(pair.Value))
        Next
        summaryMail.AppendLine("*********************************")

        If msgMappingBancaDati <> "" Then
            summaryMail.Append(vbCrLf)
            summaryMail.AppendLine(msgMappingBancaDati)
        End If

        summaryMail.Append(vbCrLf & vbCrLf)

    End Sub

    Public Shared Sub CreaRiepilogoMailExport(ByRef summaryMail As StringBuilder, ByVal recordTrovati As Integer, ByVal recordOk As Integer, ByVal recordError As Integer, ByVal recordSaltati As Integer, ByRef fileTipoXResult As Dictionary(Of String, DetFile), ByVal dataUltimaEsportazione As Date, Optional ByVal msgMappingBancaDati As String = "")

        summaryMail.AppendLine("Esportazione  -  Data: " & Now.ToLongDateString & " " & Now.ToLongTimeString & vbCrLf)
        summaryMail.AppendLine("Filtro data Esportazione DA: " & dataUltimaEsportazione.ToShortDateString & vbCrLf)

        If msgMappingBancaDati <> "" Then
            summaryMail.AppendLine(msgMappingBancaDati & vbCrLf)
        End If

        summaryMail.AppendLine("*********************************")
        summaryMail.AppendLine("*   Record Trovati " & vbTab & " = " & vbTab & vbTab & recordTrovati)
        summaryMail.AppendLine("*   Record Ok-Exp " & vbTab & " = " & vbTab & vbTab & recordOk)
        summaryMail.AppendLine("*   Record Errori " & vbTab & " = " & vbTab & vbTab & recordError)
        summaryMail.AppendLine("*   Record Saltati " & vbTab & " = " & vbTab & vbTab & recordSaltati)
        summaryMail.AppendLine("*********************************")

        If Not fileTipoXResult Is Nothing Then
            summaryMail.AppendLine("*   RECORD Esportati per Tipologia: ")
            summaryMail.AppendLine("*")

            For Each pair In fileTipoXResult
                Dim val As DetFile = pair.value
                summaryMail.AppendLine("*   ***** TIPO RECORD [" & CStr(pair.Key) & "] ***** ")
                summaryMail.AppendLine("*")
                summaryMail.AppendLine("*   RECORD Trovati " & vbTab & " = " & vbTab & vbTab & CStr(val.NumTotali))
                summaryMail.AppendLine("*   RECORD Esportati " & vbTab & " = " & vbTab & vbTab & CStr(val.NumOk))
                summaryMail.AppendLine("*   RECORD Errore " & vbTab & " = " & vbTab & vbTab & CStr(val.NumErrori))
                summaryMail.AppendLine("*   RECORD Cambiati " & vbTab & " = " & vbTab & vbTab & CStr(val.NumCambioStato))
                summaryMail.AppendLine("*   RECORD Saltati " & vbTab & " = " & vbTab & vbTab & CStr(val.NumSaltati))
                summaryMail.AppendLine("*")

            Next

            summaryMail.AppendLine("*********************************")
        End If

        summaryMail.Append(vbCrLf & vbCrLf)
    End Sub

End Class
