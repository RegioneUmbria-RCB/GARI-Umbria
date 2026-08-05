Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModello
Imports System.Text

Partial Public Class Funzioni

    Public Sub Elabora_XML_PUA_LetamazioniPrecedenti_Salva(
                            ByVal objOpzioniImportImpresa As AgronicaCoreModello.clsImpresa,
                            ByVal objOpzioni As clsOpzioni,
                            ByRef Log_Import As StringBuilder,
                            ByRef Log_Errori As StringBuilder,
                            ByRef Log_Riepilogo As StringBuilder,
                            ByVal Piva_Origine As String,
                            ByVal Piva_Destinazione As String,
                            ByVal Piva_Padre_Destinazione As String,
                            ByVal Piva_SuperUser_Destinazione As String
                    )

        Const NomeFunzione As String = "Elabora_XML_PUA_LetamazioniPrecedenti_Salva"

        Try

            Dim leggiPUA_LetamazioniPrecedenti As New AgronicaCoreG2GLocalDal.G2GPUA_LetamazioniPrecedenti_R
            Dim oPUA_LetamazioniPrecedenti As AgronicaCoreModello.G2G_PUA_LetamazioniPrecedenti = leggiPUA_LetamazioniPrecedenti.LeggiPerGias2Gias(Piva_Origine, Piva_Destinazione, objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser, objOpzioniImportImpresa, objOpzioni.objParametri_Server_GIAS_ORIGINE)

            Dim sXmlPUA_LetamazioniPrecedenti As String = AgronicaCoreUtility.XMLUtility.SerializzaOggetto(Of G2G_PUA_LetamazioniPrecedenti)(oPUA_LetamazioniPrecedenti, "")

            Dim mainDoc As String = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)
            mainDoc = AgronicaCoreUtility.XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlPUA_LetamazioniPrecedenti, "//utente/DatiImprese/Impresa", "DatiPUA_LetamazioniPrecedenti")

            Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS With {.Url = objOpzioni.wsimportaGiasURl, .Timeout = _timeout_ws_Importa}
            Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

            '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
            Dim sStringaDaSalvare As String = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, mainDoc)
            Dim outputPUA_LetamazioniPrecedenti As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, sStringaDaSalvare)

            'leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
            Dim xmlRisposta As XDocument = XDocument.Parse(outputPUA_LetamazioniPrecedenti)
            Dim rispostaPUA_LetamazioniPrecedenti As String = xmlRisposta.Element("Risposta").Element("Risposta_DatiPUA_LetamazioniPrecedenti").FirstNode.ToString

            Dim risposta As AgronicaCoreModello.G2G_Recode = AgronicaCoreUtility.XMLUtility.DeserializzaOggetto(Of AgronicaCoreModello.G2G_Recode)(rispostaPUA_LetamazioniPrecedenti, "")

            Dim MessaggioErrore As String = ""
            ' se la scrittura su destinazione è andata a buon fine aggiorno i recode sull'origine
            If String.IsNullOrEmpty(risposta.MessaggioErrore) Then
                MessaggioErrore = AgronicaCoreG2GLocalDal.G2GUtility.Scrivi_G2G_Recode(risposta, objOpzioni.objParametri_Server_GIAS_ORIGINE)
            Else
                MessaggioErrore = risposta.MessaggioErrore
            End If

            ' log trasferimento dati
            If String.IsNullOrEmpty(MessaggioErrore) Then
                If Not String.IsNullOrEmpty(risposta.LogRecode) Then
                    Log_Import.AppendLine("Trasferimento PUA_LetamazioniPrecedenti: " & risposta.LogRecode)
                End If
            Else
                Log_Import.AppendLine("Trasferimento PUA_LetamazioniPrecedenti ERRORE: " & MessaggioErrore)
            End If

            'nuove, modificate, eliminate
            Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento PUA_LetamazioniPrecedenti: " & oPUA_LetamazioniPrecedenti.pua_letamazioni_precedenti_insert.Count & " nuovi " & oPUA_LetamazioniPrecedenti.pua_letamazioni_precedenti_update.Count & " modificati " & oPUA_LetamazioniPrecedenti.Recode.G2GRecodePUA_LetamazioniPrecedentiToDelete.Count & " cancellati ")

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message
            Log_Import.AppendLine(msg)
            Log_Errori.AppendLine(msg)
            Throw New Exception(msg)
        End Try

    End Sub

    Public Sub Elabora_XML_PUA_LetamazioniPrecedenti_SalvaReverse(
                            ByVal objOpzioniImportImpresa As AgronicaCoreModello.clsImpresa,
                            ByVal objOpzioni As clsOpzioni,
                            ByRef Log_Import As StringBuilder,
                            ByRef Log_Errori As StringBuilder,
                            ByRef Log_Riepilogo As StringBuilder,
                            ByVal Piva_Origine As String,
                            ByVal Piva_Destinazione As String,
                            ByVal Piva_Padre_Destinazione As String,
                            ByVal Piva_SuperUser_Destinazione As String
                    )

        Const NomeFunzione As String = "Elabora_XML_PUA_LetamazioniPrecedenti_SalvaReverse"

        Try

            Dim leggiPUA_LetamazioniPrecedenti As New AgronicaCoreG2GLocalDal.G2GPUA_LetamazioniPrecedenti_R
            Dim oPUA_LetamazioniPrecedenti As AgronicaCoreModello.G2G_PUA_LetamazioniPrecedenti_Reverse = leggiPUA_LetamazioniPrecedenti.LeggiPerGias2GiasReverse(Piva_Origine, Piva_Destinazione, objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser, objOpzioniImportImpresa, objOpzioni.objParametri_Server_GIAS_ORIGINE)

            Dim sXmlPUA_LetamazioniPrecedenti As String = AgronicaCoreUtility.XMLUtility.SerializzaOggetto(Of G2G_PUA_LetamazioniPrecedenti_Reverse)(oPUA_LetamazioniPrecedenti, "")

            Dim mainDoc As String = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)
            mainDoc = AgronicaCoreUtility.XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlPUA_LetamazioniPrecedenti, "//utente/DatiImprese/Impresa", "DatiPUA_LetamazioniPrecedenti")

            Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS With {.Url = objOpzioni.wsimportaGiasURl, .Timeout = _timeout_ws_Importa}
            Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

            '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
            Dim sStringaDaSalvare As String = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, mainDoc)
            Dim outputPUA_LetamazioniPrecedenti As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, sStringaDaSalvare)

            'leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
            Dim xmlRisposta As XDocument = XDocument.Parse(outputPUA_LetamazioniPrecedenti)
            Dim rispostaPUA_LetamazioniPrecedenti As String = xmlRisposta.Element("Risposta").Element("Risposta_DatiPUA_LetamazioniPrecedenti").FirstNode.ToString

            Dim risposta As AgronicaCoreModello.G2G_Recode = AgronicaCoreUtility.XMLUtility.DeserializzaOggetto(Of AgronicaCoreModello.G2G_Recode)(rispostaPUA_LetamazioniPrecedenti, "")

            Dim MessaggioErrore As String = ""
            ' se la scrittura su destinazione è andata a buon fine aggiorno i recode sull'origine
            If String.IsNullOrEmpty(risposta.MessaggioErrore) Then
                MessaggioErrore = AgronicaCoreG2GLocalDal.G2GUtility.Scrivi_G2G_Recode(risposta, objOpzioni.objParametri_Server_GIAS_ORIGINE)
            Else
                MessaggioErrore = risposta.MessaggioErrore
            End If

            ' log trasferimento dati
            If String.IsNullOrEmpty(MessaggioErrore) Then
                If Not String.IsNullOrEmpty(risposta.LogRecode) Then
                    Log_Import.AppendLine("Trasferimento PUA_LetamazioniPrecedenti: " & risposta.LogRecode)
                End If
            Else
                Log_Import.AppendLine("Trasferimento PUA_LetamazioniPrecedenti ERRORE: " & MessaggioErrore)
            End If

            'nuove, modificate, eliminate
            Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento PUA_LetamazioniPrecedenti: " & oPUA_LetamazioniPrecedenti.pua_letamazioni_precedenti_insert.Count & " nuovi " & oPUA_LetamazioniPrecedenti.pua_letamazioni_precedenti_update.Count & " modificati " & oPUA_LetamazioniPrecedenti.Recode.G2GRecodePUA_LetamazioniPrecedentiToDelete.Count & " cancellati ")

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message
            Log_Import.AppendLine(msg)
            Log_Errori.AppendLine(msg)
            Throw New Exception(msg)
        End Try

    End Sub

End Class
