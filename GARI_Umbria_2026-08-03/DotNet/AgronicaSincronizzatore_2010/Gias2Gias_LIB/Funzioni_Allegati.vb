Imports System.Xml
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports System.Text
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello.Anagrafe
Imports AgronicaCoreModello
Imports AgronicaCoreUtility
Imports AgronicaCoreG2GLocalDal

Partial Public Class Funzioni


#Region "Allegati"

    Public Sub Elabora_XML_Allegati_Salva(
                            ByVal objOpzioniImportImpresa As clsImpresa,
                            ByVal efG2G As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                            ByVal objOpzioni As clsOpzioni,
                            ByRef Log_Import As StringBuilder,
                            ByRef Log_Errori As StringBuilder,
                            ByRef Log_Riepilogo As StringBuilder,
                            ByVal Piva_Origine As String,
                            ByVal Piva_Destinazione As String,
                            ByVal TipoOperazione_DB As enum_TipoOperazioneDB
                    )


        Const NomeFunzione As String = "Elabora_XML_Allegati_Salva"

        Try
            Dim leggiAllegati As New AgronicaCoreG2GLocalDal.G2GAllegati_R
            Dim oAllegati As AgronicaCoreModello.G2G_Allegati = leggiAllegati.LeggiPerGias2Gias(objOpzioniImportImpresa, Piva_Origine, Piva_Destinazione, objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser, objOpzioni.objParametri_Server_GIAS_ORIGINE)

            'QUI CAMBIO LE CHIAVI DI Centro, Fabbricato, Appezza, Id_Reg
            For Each allegati_entita_insert In oAllegati.allegati_entita_insert
                Dim sa_cod_origine As Integer = allegati_entita_insert.Sa_Cod
                Dim appezza_origine As Integer = allegati_entita_insert.Appezza

                If allegati_entita_insert.Sa_Cod <> 0 Then
                    Dim centro = (From rr In efG2G.G2G_Recode_Imprese Where rr.From_PivaSuperUser = allegati_entita_insert.Allegati_Documenti_SuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.FROM_Piva = Piva_Origine AndAlso rr.FROM_SaCod = allegati_entita_insert.Sa_Cod).FirstOrDefault()
                    allegati_entita_insert.Sa_Cod = centro.TO_SaCod
                End If

                If allegati_entita_insert.Campo_Cod <> 0 Then
                    Dim campo = (From rr In efG2G.G2G_Recode_Campo Where rr.From_PivaSuperUser = allegati_entita_insert.Allegati_Documenti_SuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.From_Piva = Piva_Origine AndAlso rr.From_Sa_Cod = sa_cod_origine AndAlso rr.From_Campo_cod = allegati_entita_insert.Campo_Cod).FirstOrDefault()
                    allegati_entita_insert.Campo_Cod = campo.To_Campo_cod
                End If

                If allegati_entita_insert.Appezza <> 0 Then
                    Dim appezza = (From rr In efG2G.G2G_Recode_Appezzamenti Where rr.From_PivaSuperUser = allegati_entita_insert.Allegati_Documenti_SuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.From_Piva = Piva_Origine AndAlso rr.From_Sa_Cod = sa_cod_origine AndAlso rr.From_Appezza = allegati_entita_insert.Appezza).FirstOrDefault()
                    allegati_entita_insert.Appezza = appezza.To_Appezza
                End If

                If allegati_entita_insert.Id_Imp <> 0 Then
                    Dim impianto = (From rr In efG2G.G2G_Recode_Impianti Where rr.From_PivaSuperUser = allegati_entita_insert.Allegati_Documenti_SuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.From_Piva = Piva_Origine AndAlso rr.From_Sa_Cod = sa_cod_origine AndAlso rr.From_Appezza = appezza_origine AndAlso rr.From_Id_Reg = allegati_entita_insert.Id_Imp).FirstOrDefault()
                    allegati_entita_insert.Id_Imp = impianto.To_Id_Reg
                End If

                If allegati_entita_insert.Fabbricato_Cod <> 0 Then
                    Dim fabbricato = (From rr In efG2G.G2G_Recode_Fabbricati Where rr.From_PivaSuperUser = allegati_entita_insert.Allegati_Documenti_SuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.FromPiva = Piva_Origine AndAlso rr.FromSa_cod = sa_cod_origine AndAlso rr.From_FabbricatoCod = allegati_entita_insert.Fabbricato_Cod).FirstOrDefault()
                    allegati_entita_insert.Fabbricato_Cod = fabbricato.To_FabbricatoCod
                End If
            Next

            For Each allegati_entita_update In oAllegati.allegati_entita_update
                Dim sa_cod_origine As Integer = allegati_entita_update.Sa_Cod
                Dim appezza_origine As Integer = allegati_entita_update.Appezza

                If allegati_entita_update.Sa_Cod <> 0 Then
                    Dim centro = (From rr In efG2G.G2G_Recode_Imprese Where rr.From_PivaSuperUser = allegati_entita_update.Allegati_Documenti_SuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.FROM_Piva = Piva_Origine AndAlso rr.FROM_SaCod = allegati_entita_update.Sa_Cod).FirstOrDefault()
                    allegati_entita_update.Sa_Cod = centro.TO_SaCod
                End If

                If allegati_entita_update.Campo_Cod <> 0 Then
                    Dim campo = (From rr In efG2G.G2G_Recode_Campo Where rr.From_PivaSuperUser = allegati_entita_update.Allegati_Documenti_SuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.From_Piva = Piva_Origine AndAlso rr.From_Sa_Cod = sa_cod_origine AndAlso rr.From_Campo_cod = allegati_entita_update.Campo_Cod).FirstOrDefault()
                    allegati_entita_update.Campo_Cod = campo.To_Campo_cod
                End If

                If allegati_entita_update.Appezza <> 0 Then
                    Dim appezza = (From rr In efG2G.G2G_Recode_Appezzamenti Where rr.From_PivaSuperUser = allegati_entita_update.Allegati_Documenti_SuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.From_Piva = Piva_Origine AndAlso rr.From_Sa_Cod = sa_cod_origine AndAlso rr.From_Appezza = allegati_entita_update.Appezza).FirstOrDefault()
                    allegati_entita_update.Appezza = appezza.To_Appezza
                End If

                If allegati_entita_update.Id_Imp <> 0 Then
                    Dim impianto = (From rr In efG2G.G2G_Recode_Impianti Where rr.From_PivaSuperUser = allegati_entita_update.Allegati_Documenti_SuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.From_Piva = Piva_Origine AndAlso rr.From_Sa_Cod = sa_cod_origine AndAlso rr.From_Appezza = appezza_origine AndAlso rr.From_Id_Reg = allegati_entita_update.Id_Imp).FirstOrDefault()
                    allegati_entita_update.Id_Imp = impianto.To_Id_Reg
                End If

                If allegati_entita_update.Fabbricato_Cod <> 0 Then
                    Dim fabbricato = (From rr In efG2G.G2G_Recode_Fabbricati Where rr.From_PivaSuperUser = allegati_entita_update.Allegati_Documenti_SuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.FromPiva = Piva_Origine AndAlso rr.FromSa_cod = sa_cod_origine AndAlso rr.From_FabbricatoCod = allegati_entita_update.Fabbricato_Cod).FirstOrDefault()
                    allegati_entita_update.Fabbricato_Cod = fabbricato.To_FabbricatoCod
                End If
            Next

            Dim recordCoinvolti = oAllegati.alert_elenco_insert.Count _
                                + oAllegati.alert_elenco_update.Count _
                                + oAllegati.alert_entita_insert.Count _
                                + oAllegati.alert_entita_update.Count _
                                + oAllegati.allegati_delete.Count _
                                + oAllegati.allegati_entita_delete.Count _
                                + oAllegati.allegati_entita_insert.Count _
                                + oAllegati.allegati_entita_update.Count _
                                + oAllegati.allegati_insert.Count _
                                + oAllegati.allegati_update.Count _
                                + oAllegati.G2G_Allegati_Entita_Recode_delete.Count _
                                + oAllegati.G2G_Allegati_Entita_Recode_insert.Count _
                                + oAllegati.G2G_Allegati_Entita_Recode_update.Count _
                                + oAllegati.G2G_Allegati_Recode_delete.Count _
                                + oAllegati.G2G_Allegati_Recode_insert.Count _
                                + oAllegati.G2G_Allegati_Recode_update.Count

            If recordCoinvolti = 0 Then
                Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Allegati: Nessun dato da Inserire/Modificare/Eliminare ")
                Exit Sub
            End If

            Dim sXmlAllegati As String = AgronicaCoreUtility.XMLUtility.SerializzaOggetto(Of G2G_Allegati)(oAllegati, "")

            Dim mainDoc As String = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)

            Dim sStringaDaSalvare As String = AgronicaCoreUtility.XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlAllegati, "//utente/DatiImprese/Impresa", "DatiAllegati")

            Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS With {.Url = objOpzioni.wsimportaGiasURl, .Timeout = _timeout_ws_Importa}
            Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

            '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
            sStringaDaSalvare = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvare)

            Dim outputAllegati As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, sStringaDaSalvare)

            'leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
            Dim xmlRisposta As XDocument = XDocument.Parse(outputAllegati)
            Dim xmlRispostaAllegati As XElement = xmlRisposta.Element("Risposta").Element("Risposta_DatiAllegati")
            Dim rispostaAllegati = xmlRispostaAllegati.FirstNode.ToString
            'Dim rispostaParcoMacchine = AgroZip.DeCompressioneBase64(1, wsimportazione.Scrivi_ParcoMacchineXmlPrivato(Str_Credenziali_WS, AgroZip.CompressioneBase64(1, sXmlParcoMacchine)))
            Dim risposta As G2G_Recode = XMLUtility.DeserializzaOggetto(Of G2G_Recode)(rispostaAllegati, "")

            Dim MessaggioErrore As String = ""
            ' se la scrittura su destinazione è andata a buon fine aggiorno i recode sull'origine
            If String.IsNullOrEmpty(risposta.MessaggioErrore) Then
                MessaggioErrore = G2GUtility.Scrivi_G2G_Recode(risposta, objOpzioni.objParametri_Server_GIAS_ORIGINE)
            Else
                MessaggioErrore = risposta.MessaggioErrore
            End If

            ' log trasferimento dati
            If String.IsNullOrEmpty(MessaggioErrore) Then
                If Not String.IsNullOrEmpty(risposta.LogRecode) Then
                    Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Allegati: " & risposta.LogRecode)
                Else
                    'nuove, modificate, eliminate
                    Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Allegati: " & oAllegati.allegati_insert.Count & " nuovi " & oAllegati.allegati_update.Count & " modificati " & oAllegati.G2G_Allegati_Recode_delete.Count & " cancellati ")
                    Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Allegati - Entita: " & oAllegati.allegati_entita_insert.Count & " nuovi " & oAllegati.allegati_entita_update.Count & " modificati " & oAllegati.G2G_Allegati_Entita_Recode_delete.Count & " cancellati ")
                End If
            Else
                Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Allegati ERRORE: " & MessaggioErrore)
            End If

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message
            Log_Import.AppendLine(msg)
            Log_Errori.AppendLine(msg)
            Throw New Exception(msg)
        End Try





    End Sub

    Public Sub Elabora_XML_Allegati_SalvaReverse(
                            ByVal objOpzioniImportImpresa As clsImpresa,
                            ByVal efG2G As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                            ByVal objOpzioni As clsOpzioni,
                            ByRef Log_Import As StringBuilder,
                            ByRef Log_Errori As StringBuilder,
                            ByRef Log_Riepilogo As StringBuilder,
                            ByVal Piva_Origine As String,
                            ByVal Piva_Destinazione As String,
                            ByVal TipoOperazione_DB As enum_TipoOperazioneDB
                    )


        Const NomeFunzione As String = "Elabora_XML_Allegati_SalvaReverse"

        Try
            Dim leggiAllegati As New AgronicaCoreG2GLocalDal.G2GAllegati_R
            Dim oAllegati As AgronicaCoreModello.G2G_Allegati_Reverse = leggiAllegati.LeggiPerGias2GiasReverse(objOpzioniImportImpresa, Piva_Origine, Piva_Destinazione, objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser, objOpzioni.objParametri_Server_GIAS_ORIGINE)

            'QUI CAMBIO LE CHIAVI DI Centro, Fabbricato, Appezza, Id_Reg
            For Each allegati_entita_insert In oAllegati.allegati_entita_insert
                Dim sa_cod_origine As Integer = allegati_entita_insert.Sa_Cod
                Dim appezza_origine As Integer = allegati_entita_insert.Appezza

                If allegati_entita_insert.Sa_Cod <> 0 Then
                    Dim centro = (From rr In efG2G.G2G_Recode_Imprese Where rr.To_PivaSuperUser = allegati_entita_insert.Allegati_Documenti_SuperUser AndAlso rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.To_Piva = Piva_Origine AndAlso rr.TO_SaCod = allegati_entita_insert.Sa_Cod).FirstOrDefault()
                    allegati_entita_insert.Sa_Cod = centro.FROM_SaCod
                End If

                If allegati_entita_insert.Campo_Cod <> 0 Then
                    Dim campo = (From rr In efG2G.G2G_Recode_Campo Where rr.To_PivaSuperUser = allegati_entita_insert.Allegati_Documenti_SuperUser AndAlso rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.To_Piva = Piva_Origine AndAlso rr.To_Sa_Cod = sa_cod_origine AndAlso rr.To_Campo_cod = allegati_entita_insert.Campo_Cod).FirstOrDefault()
                    allegati_entita_insert.Campo_Cod = campo.From_Campo_cod
                End If

                If allegati_entita_insert.Appezza <> 0 Then
                    Dim appezza = (From rr In efG2G.G2G_Recode_Appezzamenti Where rr.To_PivaSuperUser = allegati_entita_insert.Allegati_Documenti_SuperUser AndAlso rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.To_Piva = Piva_Origine AndAlso rr.To_Sa_Cod = sa_cod_origine AndAlso rr.To_Appezza = allegati_entita_insert.Appezza).FirstOrDefault()
                    allegati_entita_insert.Appezza = appezza.From_Appezza
                End If

                If allegati_entita_insert.Id_Imp <> 0 Then
                    Dim impianto = (From rr In efG2G.G2G_Recode_Impianti Where rr.To_PivaSuperUser = allegati_entita_insert.Allegati_Documenti_SuperUser AndAlso rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.To_Piva = Piva_Origine AndAlso rr.To_Sa_Cod = sa_cod_origine AndAlso rr.To_Appezza = appezza_origine AndAlso rr.To_Id_Reg = allegati_entita_insert.Id_Imp).FirstOrDefault()
                    allegati_entita_insert.Id_Imp = impianto.From_Id_Reg
                End If

                If allegati_entita_insert.Fabbricato_Cod <> 0 Then
                    Dim fabbricato = (From rr In efG2G.G2G_Recode_Fabbricati Where rr.To_PivaSuperUser = allegati_entita_insert.Allegati_Documenti_SuperUser AndAlso rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.ToPiva = Piva_Origine AndAlso rr.ToSa_cod = sa_cod_origine AndAlso rr.To_FabbricatoCod = allegati_entita_insert.Fabbricato_Cod).FirstOrDefault()
                    allegati_entita_insert.Fabbricato_Cod = fabbricato.From_FabbricatoCod
                End If
            Next

            For Each allegati_entita_update In oAllegati.allegati_entita_update
                Dim sa_cod_origine As Integer = allegati_entita_update.Sa_Cod
                Dim appezza_origine As Integer = allegati_entita_update.Appezza

                If allegati_entita_update.Sa_Cod <> 0 Then
                    Dim centro = (From rr In efG2G.G2G_Recode_Imprese Where rr.To_PivaSuperUser = allegati_entita_update.Allegati_Documenti_SuperUser AndAlso rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.To_Piva = Piva_Origine AndAlso rr.TO_SaCod = allegati_entita_update.Sa_Cod).FirstOrDefault()
                    allegati_entita_update.Sa_Cod = centro.FROM_SaCod
                End If

                If allegati_entita_update.Campo_Cod <> 0 Then
                    Dim campo = (From rr In efG2G.G2G_Recode_Campo Where rr.To_PivaSuperUser = allegati_entita_update.Allegati_Documenti_SuperUser AndAlso rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.To_Piva = Piva_Origine AndAlso rr.To_Sa_Cod = sa_cod_origine AndAlso rr.To_Campo_cod = allegati_entita_update.Campo_Cod).FirstOrDefault()
                    allegati_entita_update.Campo_Cod = campo.From_Campo_cod
                End If

                If allegati_entita_update.Appezza <> 0 Then
                    Dim appezza = (From rr In efG2G.G2G_Recode_Appezzamenti Where rr.To_PivaSuperUser = allegati_entita_update.Allegati_Documenti_SuperUser AndAlso rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.To_Piva = Piva_Origine AndAlso rr.To_Sa_Cod = sa_cod_origine AndAlso rr.To_Appezza = allegati_entita_update.Appezza).FirstOrDefault()
                    allegati_entita_update.Appezza = appezza.From_Appezza
                End If

                If allegati_entita_update.Id_Imp <> 0 Then
                    Dim impianto = (From rr In efG2G.G2G_Recode_Impianti Where rr.To_PivaSuperUser = allegati_entita_update.Allegati_Documenti_SuperUser AndAlso rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.To_Piva = Piva_Origine AndAlso rr.To_Sa_Cod = sa_cod_origine AndAlso rr.To_Appezza = appezza_origine AndAlso rr.To_Id_Reg = allegati_entita_update.Id_Imp).FirstOrDefault()
                    allegati_entita_update.Id_Imp = impianto.From_Id_Reg
                End If

                If allegati_entita_update.Fabbricato_Cod <> 0 Then
                    Dim fabbricato = (From rr In efG2G.G2G_Recode_Fabbricati Where rr.To_PivaSuperUser = allegati_entita_update.Allegati_Documenti_SuperUser AndAlso rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.ToPiva = Piva_Origine AndAlso rr.ToSa_cod = sa_cod_origine AndAlso rr.To_FabbricatoCod = allegati_entita_update.Fabbricato_Cod).FirstOrDefault()
                    allegati_entita_update.Fabbricato_Cod = fabbricato.From_FabbricatoCod
                End If
            Next

            Dim recordCoinvolti = oAllegati.alert_elenco_insert.Count _
                                + oAllegati.alert_elenco_update.Count _
                                + oAllegati.alert_entita_insert.Count _
                                + oAllegati.alert_entita_update.Count _
                                + oAllegati.allegati_delete.Count _
                                + oAllegati.allegati_entita_delete.Count _
                                + oAllegati.allegati_entita_insert.Count _
                                + oAllegati.allegati_entita_update.Count _
                                + oAllegati.allegati_insert.Count _
                                + oAllegati.allegati_update.Count _
                                + oAllegati.G2G_Allegati_Entita_Recode_delete.Count _
                                + oAllegati.G2G_Allegati_Entita_Recode_insert.Count _
                                + oAllegati.G2G_Allegati_Entita_Recode_update.Count _
                                + oAllegati.G2G_Allegati_Recode_delete.Count _
                                + oAllegati.G2G_Allegati_Recode_insert.Count _
                                + oAllegati.G2G_Allegati_Recode_update.Count

            If recordCoinvolti = 0 Then
                Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Allegati: Nessun dato da Inserire/Modificare/Eliminare ")
                Exit Sub
            End If

            Dim sXmlAllegati As String = AgronicaCoreUtility.XMLUtility.SerializzaOggetto(Of G2G_Allegati_Reverse)(oAllegati, "")

            Dim mainDoc As String = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)

            Dim sStringaDaSalvare As String = AgronicaCoreUtility.XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlAllegati, "//utente/DatiImprese/Impresa", "DatiAllegati")

            Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS With {.Url = objOpzioni.wsimportaGiasURl, .Timeout = _timeout_ws_Importa}
            Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

            '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
            sStringaDaSalvare = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvare)

            Dim outputAllegati As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, sStringaDaSalvare)

            'leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
            Dim xmlRisposta As XDocument = XDocument.Parse(outputAllegati)
            Dim xmlRispostaAllegati As XElement = xmlRisposta.Element("Risposta").Element("Risposta_DatiAllegati")
            Dim rispostaAllegati = xmlRispostaAllegati.FirstNode.ToString
            'Dim rispostaParcoMacchine = AgroZip.DeCompressioneBase64(1, wsimportazione.Scrivi_ParcoMacchineXmlPrivato(Str_Credenziali_WS, AgroZip.CompressioneBase64(1, sXmlParcoMacchine)))
            Dim risposta As G2G_Recode = XMLUtility.DeserializzaOggetto(Of G2G_Recode)(rispostaAllegati, "")

            Dim MessaggioErrore As String = ""
            ' se la scrittura su destinazione è andata a buon fine aggiorno i recode sull'origine
            If String.IsNullOrEmpty(risposta.MessaggioErrore) Then
                MessaggioErrore = G2GUtility.Scrivi_G2G_Recode(risposta, objOpzioni.objParametri_Server_GIAS_ORIGINE)
            Else
                MessaggioErrore = risposta.MessaggioErrore
            End If

            ' log trasferimento dati
            If String.IsNullOrEmpty(MessaggioErrore) Then
                If Not String.IsNullOrEmpty(risposta.LogRecode) Then
                    Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Allegati: " & risposta.LogRecode)
                Else
                    'nuove, modificate, eliminate
                    Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Allegati: " & oAllegati.allegati_insert.Count & " nuovi " & oAllegati.allegati_update.Count & " modificati " & oAllegati.G2G_Allegati_Recode_delete.Count & " cancellati ")
                    Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Allegati - Entita: " & oAllegati.allegati_entita_insert.Count & " nuovi " & oAllegati.allegati_entita_update.Count & " modificati " & oAllegati.G2G_Allegati_Entita_Recode_delete.Count & " cancellati ")
                End If
            Else
                Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Allegati ERRORE: " & MessaggioErrore)
            End If

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message
            Log_Import.AppendLine(msg)
            Log_Errori.AppendLine(msg)
            Throw New Exception(msg)
        End Try





    End Sub


#End Region
End Class
