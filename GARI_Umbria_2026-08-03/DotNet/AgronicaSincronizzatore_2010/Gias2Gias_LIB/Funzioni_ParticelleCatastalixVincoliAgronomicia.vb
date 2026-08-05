Imports System.Xml
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Imports AgronicaCoreDataProvider

Imports AgronicaCoreModello

Imports System.Text

Partial Public Class Funzioni

    Public Sub Elabora_XML_ParticelleCatastalixVincoliAgronomici_Salva(
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

        Const NomeFunzione As String = "Elabora_XML_ParticelleCatastalixVincoliAgronomici_Salva"

        Try

            Dim leggiParticelleCatastalixVincoliAgronomici As New AgronicaCoreG2GLocalDal.G2GParticelleCatastalixVincoliAgronomici_R
            Dim oParticelleCatastalixVincoliAgronomici As AgronicaCoreModello.G2G_ParticelleCatastalixVincoliAgronomici = leggiParticelleCatastalixVincoliAgronomici.LeggiPerGias2Gias(Piva_Origine, Piva_Destinazione, objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser, objOpzioniImportImpresa, objOpzioni.objParametri_Server_GIAS_ORIGINE)

            Dim sXmlParticelleCatastalixVincoliAgronomici As String = AgronicaCoreUtility.XMLUtility.SerializzaOggetto(Of G2G_ParticelleCatastalixVincoliAgronomici)(oParticelleCatastalixVincoliAgronomici, "")

            Dim mainDoc As String = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)
            mainDoc = AgronicaCoreUtility.XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlParticelleCatastalixVincoliAgronomici, "//utente/DatiImprese/Impresa", "DatiParticelleCatastalixVincoliAgronomici")

            Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS With {.Url = objOpzioni.wsimportaGiasURl, .Timeout = _timeout_ws_Importa}
            Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

            '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
            Dim sStringaDaSalvare As String = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, mainDoc)
            Dim outputParticelleCatastalixVincoliAgronomici As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, sStringaDaSalvare)

            'leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
            Dim xmlRisposta As XDocument = XDocument.Parse(outputParticelleCatastalixVincoliAgronomici)
            Dim rispostaParticelleCatastalixVincoliAgronomici As String = xmlRisposta.Element("Risposta").Element("Risposta_DatiParticelleCatastalixVincoliAgronomici").FirstNode.ToString

            Dim risposta As AgronicaCoreModello.G2G_Recode = AgronicaCoreUtility.XMLUtility.DeserializzaOggetto(Of AgronicaCoreModello.G2G_Recode)(rispostaParticelleCatastalixVincoliAgronomici, "")

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
                    Log_Import.AppendLine("Trasferimento ParticelleCatastaliXVincoliAgronomici: " & risposta.LogRecode)
                End If
            Else
                Log_Import.AppendLine("Trasferimento ParticelleCatastaliXVincoliAgronomici ERRORE: " & MessaggioErrore)
            End If

            'nuove, modificate, eliminate
            Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento ParticelleCatastaliXVincoliAgronomici: " & oParticelleCatastalixVincoliAgronomici.particelle_catastali_x_vincoli_agronomici_insert.Count & " nuovi " & oParticelleCatastalixVincoliAgronomici.particelle_catastali_x_vincoli_agronomici_update.Count & " modificati " & oParticelleCatastalixVincoliAgronomici.Recode.G2GRecodeParticelleCatastalixVincoliAgronomiciToDelete.Count & " cancellati ")

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message
            Log_Import.AppendLine(msg)
            Log_Errori.AppendLine(msg)
            Throw New Exception(msg)
        End Try

    End Sub

    Public Sub Elabora_XML_ParticelleCatastalixVincoliAgronomici_SalvaReverse(
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

        Const NomeFunzione As String = "Elabora_XML_ParticelleCatastalixVincoliAgronomici_SalvaReverse"

        Try

            Dim leggiParticelleCatastalixVincoliAgronomici As New AgronicaCoreG2GLocalDal.G2GParticelleCatastalixVincoliAgronomici_R
            Dim oParticelleCatastalixVincoliAgronomici As AgronicaCoreModello.G2G_ParticelleCatastalixVincoliAgronomici_Reverse = leggiParticelleCatastalixVincoliAgronomici.LeggiPerGias2GiasReverse(Piva_Origine, Piva_Destinazione, objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser, objOpzioniImportImpresa, objOpzioni.objParametri_Server_GIAS_ORIGINE)

            Dim sXmlParticelleCatastalixVincoliAgronomici As String = AgronicaCoreUtility.XMLUtility.SerializzaOggetto(Of G2G_ParticelleCatastalixVincoliAgronomici_Reverse)(oParticelleCatastalixVincoliAgronomici, "")

            Dim mainDoc As String = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)
            mainDoc = AgronicaCoreUtility.XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlParticelleCatastalixVincoliAgronomici, "//utente/DatiImprese/Impresa", "DatiParticelleCatastalixVincoliAgronomici")

            Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS With {.Url = objOpzioni.wsimportaGiasURl, .Timeout = _timeout_ws_Importa}
            Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

            '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
            Dim sStringaDaSalvare As String = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, mainDoc)
            Dim outputParticelleCatastalixVincoliAgronomici As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, sStringaDaSalvare)

            'leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
            Dim xmlRisposta As XDocument = XDocument.Parse(outputParticelleCatastalixVincoliAgronomici)
            Dim rispostaParticelleCatastalixVincoliAgronomici As String = xmlRisposta.Element("Risposta").Element("Risposta_DatiParticelleCatastalixVincoliAgronomici").FirstNode.ToString

            Dim risposta As AgronicaCoreModello.G2G_Recode = AgronicaCoreUtility.XMLUtility.DeserializzaOggetto(Of AgronicaCoreModello.G2G_Recode)(rispostaParticelleCatastalixVincoliAgronomici, "")

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
                    Log_Import.AppendLine("Trasferimento ParticelleCatastaliXVincoliAgronomici: " & risposta.LogRecode)
                End If
            Else
                Log_Import.AppendLine("Trasferimento ParticelleCatastaliXVincoliAgronomici ERRORE: " & MessaggioErrore)
            End If

            'nuove, modificate, eliminate
            Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento ParticelleCatastaliXVincoliAgronomici: " & oParticelleCatastalixVincoliAgronomici.particelle_catastali_x_vincoli_agronomici_insert.Count & " nuovi " & oParticelleCatastalixVincoliAgronomici.particelle_catastali_x_vincoli_agronomici_update.Count & " modificati " & oParticelleCatastalixVincoliAgronomici.Recode.G2GRecodeParticelleCatastalixVincoliAgronomiciToDelete.Count & " cancellati ")

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message
            Log_Import.AppendLine(msg)
            Log_Errori.AppendLine(msg)
            Throw New Exception(msg)
        End Try

    End Sub

End Class
