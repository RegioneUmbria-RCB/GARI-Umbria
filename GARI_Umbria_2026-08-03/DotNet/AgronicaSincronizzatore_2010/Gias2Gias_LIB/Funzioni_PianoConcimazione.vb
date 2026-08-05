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


#Region "Pratiche"

    Public Sub Elabora_XML_Piano_Concimazione_Salva(
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

        Const NomeFunzione As String = "Elabora_XML_Piano_Concimazione_Salva"

        Try
            Dim leggiPianiConcimazione As New AgronicaCoreG2GLocalDal.G2GPianoConcimazione_R
            Dim oPianiConcimazione As AgronicaCoreModello.G2G_PianoConcimazione =
                leggiPianiConcimazione.LeggiPerGias2Gias(objOpzioniImportImpresa, Piva_Origine, Piva_Destinazione, objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser, objOpzioni.objParametri_Server_GIAS_ORIGINE)

            Dim imprese As List(Of G2G_Recode_Imprese) = (From rr In efG2G.G2G_Recode_Imprese Where rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.FROM_Piva = Piva_Origine).ToList()

            'QUI CAMBIO LE CHIAVI DI Centro, Fabbricato, Appezza, Id_Reg
            For Each pianoConcimazione_Dettagli_insert In oPianiConcimazione.pianoconcimazione_dettagli_insert
                If pianoConcimazione_Dettagli_insert.PC_Dettagli_SaCod <> 0 Then
                    Dim centro = (From rr In imprese Where rr.From_PivaSuperUser = pianoConcimazione_Dettagli_insert.PC_Dettagli_SuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.FROM_Piva = Piva_Origine AndAlso rr.FROM_SaCod = pianoConcimazione_Dettagli_insert.PC_Dettagli_SaCod).FirstOrDefault()
                    pianoConcimazione_Dettagli_insert.PC_Dettagli_SaCod = centro.TO_SaCod
                End If
            Next

            For Each pianoConcimazione_Dettagli_update In oPianiConcimazione.pianoconcimazione_dettagli_update
                If pianoConcimazione_Dettagli_update.PC_Dettagli_SaCod <> 0 Then
                    Dim centro = (From rr In imprese Where rr.From_PivaSuperUser = pianoConcimazione_Dettagli_update.PC_Dettagli_SuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.FROM_Piva = Piva_Origine AndAlso rr.FROM_SaCod = pianoConcimazione_Dettagli_update.PC_Dettagli_SaCod).FirstOrDefault()
                    pianoConcimazione_Dettagli_update.PC_Dettagli_SaCod = centro.TO_SaCod
                End If
            Next

            Dim campi As List(Of G2G_Recode_Campo) = (From rr In efG2G.G2G_Recode_Campo Where rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.From_Piva = Piva_Origine).ToList()
            Dim appezzamenti As List(Of G2G_Recode_Appezzamenti) = (From rr In efG2G.G2G_Recode_Appezzamenti Where rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.From_Piva = Piva_Origine).ToList()
            Dim impianti As List(Of G2G_Recode_Impianti) = (From rr In efG2G.G2G_Recode_Impianti Where rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.From_Piva = Piva_Origine).ToList()
            Dim progetti As List(Of G2G_Recode_Distinta) = (From rr In efG2G.G2G_Recode_Distinta Where rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.From_Piva = Piva_Origine).ToList()
            Dim fabbricati As List(Of G2G_Recode_Fabbricati) = (From rr In efG2G.G2G_Recode_Fabbricati Where rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.FromPiva = Piva_Origine).ToList()

            For Each pianoConcimazione_EntitaxTestata_insert In oPianiConcimazione.pianoconcimazione_entitaxtestata_insert
                Dim sa_cod_origine As Integer = pianoConcimazione_EntitaxTestata_insert.Sa_Cod
                Dim appezza_origine As Integer = pianoConcimazione_EntitaxTestata_insert.Appezza
                Dim idImp_origine As Integer = pianoConcimazione_EntitaxTestata_insert.Id_Imp

                If pianoConcimazione_EntitaxTestata_insert.Sa_Cod <> 0 Then
                    Dim centro = (From rr In imprese Where rr.From_PivaSuperUser = pianoConcimazione_EntitaxTestata_insert.PC_SuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.FROM_Piva = Piva_Origine AndAlso rr.FROM_SaCod = pianoConcimazione_EntitaxTestata_insert.Sa_Cod).FirstOrDefault()
                    pianoConcimazione_EntitaxTestata_insert.Sa_Cod = centro.TO_SaCod
                End If

                If pianoConcimazione_EntitaxTestata_insert.Campo_Cod <> 0 Then
                    Dim campo = (From rr In campi Where rr.From_PivaSuperUser = pianoConcimazione_EntitaxTestata_insert.PC_SuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.From_Piva = Piva_Origine AndAlso rr.From_Sa_Cod = sa_cod_origine AndAlso rr.From_Campo_cod = pianoConcimazione_EntitaxTestata_insert.Campo_Cod).FirstOrDefault()
                    pianoConcimazione_EntitaxTestata_insert.Campo_Cod = campo.To_Campo_cod
                End If

                If pianoConcimazione_EntitaxTestata_insert.Appezza <> 0 Then
                    Dim appezza = (From rr In appezzamenti Where rr.From_PivaSuperUser = pianoConcimazione_EntitaxTestata_insert.PC_SuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.From_Piva = Piva_Origine AndAlso rr.From_Sa_Cod = sa_cod_origine AndAlso rr.From_Appezza = pianoConcimazione_EntitaxTestata_insert.Appezza).FirstOrDefault()
                    pianoConcimazione_EntitaxTestata_insert.Appezza = appezza.To_Appezza
                End If

                If pianoConcimazione_EntitaxTestata_insert.Id_Imp <> 0 Then
                    Dim impianto = (From rr In impianti Where rr.From_PivaSuperUser = pianoConcimazione_EntitaxTestata_insert.PC_SuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.From_Piva = Piva_Origine AndAlso rr.From_Sa_Cod = sa_cod_origine AndAlso rr.From_Appezza = appezza_origine AndAlso rr.From_Id_Reg = pianoConcimazione_EntitaxTestata_insert.Id_Imp).FirstOrDefault()
                    pianoConcimazione_EntitaxTestata_insert.Id_Imp = impianto.To_Id_Reg
                End If

                If pianoConcimazione_EntitaxTestata_insert.Progetto_Cod <> 0 Then
                    Dim progetto = (From rr In progetti Where rr.From_PivaSuperUser = pianoConcimazione_EntitaxTestata_insert.PC_SuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.From_Piva = Piva_Origine AndAlso rr.From_Progetto_cod = pianoConcimazione_EntitaxTestata_insert.Progetto_Cod).FirstOrDefault()
                    pianoConcimazione_EntitaxTestata_insert.Progetto_Cod = progetto.To_Progetto_cod
                End If

                If pianoConcimazione_EntitaxTestata_insert.Fabbricato_Cod <> 0 Then
                    Dim fabbricato = (From rr In fabbricati Where rr.From_PivaSuperUser = pianoConcimazione_EntitaxTestata_insert.PC_SuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.FromPiva = Piva_Origine AndAlso rr.FromSa_cod = sa_cod_origine AndAlso rr.From_FabbricatoCod = pianoConcimazione_EntitaxTestata_insert.Fabbricato_Cod).FirstOrDefault()
                    pianoConcimazione_EntitaxTestata_insert.Fabbricato_Cod = fabbricato.To_FabbricatoCod
                End If

                pianoConcimazione_EntitaxTestata_insert.Chiave_Albero_Imprese = ""

            Next


            Dim recordCoinvolti = oPianiConcimazione.pianoconcimazione_dettagli_delete.Count _
                                + oPianiConcimazione.pianoconcimazione_dettagli_insert.Count _
                                + oPianiConcimazione.pianoconcimazione_dettagli_update.Count _
                                + oPianiConcimazione.pianoconcimazione_elaborazioni_delete.Count _
                                + oPianiConcimazione.pianoconcimazione_elaborazioni_insert.Count _
                                + oPianiConcimazione.pianoconcimazione_elaborazioni_update.Count _
                                + oPianiConcimazione.pianoconcimazione_entitaxtestata_delete.Count _
                                + oPianiConcimazione.pianoconcimazione_entitaxtestata_insert.Count _
                                + oPianiConcimazione.pianoconcimazione_fattoricorrettivi_delete.Count _
                                + oPianiConcimazione.pianoconcimazione_fattoricorrettivi_insert.Count _
                                + oPianiConcimazione.pianoconcimazione_testata_delete.Count _
                                + oPianiConcimazione.pianoconcimazione_testata_insert.Count _
                                + oPianiConcimazione.pianoconcimazione_testata_update.Count _
                                + oPianiConcimazione.G2G_PianoConcimazione_Dettagli_Recode_delete.Count _
                                + oPianiConcimazione.G2G_PianoConcimazione_Dettagli_Recode_insert.Count _
                                + oPianiConcimazione.G2G_PianoConcimazione_Dettagli_Recode_update.Count _
                                + oPianiConcimazione.G2G_PianoConcimazione_Elaborazioni_Recode_delete.Count _
                                + oPianiConcimazione.G2G_PianoConcimazione_Elaborazioni_Recode_insert.Count _
                                + oPianiConcimazione.G2G_PianoConcimazione_Elaborazioni_Recode_update.Count _
                                + oPianiConcimazione.G2G_PianoConcimazione_EntitaxTestata_Recode_delete.Count _
                                + oPianiConcimazione.G2G_PianoConcimazione_EntitaxTestata_Recode_insert.Count _
                                + oPianiConcimazione.G2G_PianoConcimazione_Testata_Recode_delete.Count _
                                + oPianiConcimazione.G2G_PianoConcimazione_Testata_Recode_insert.Count _
                                + oPianiConcimazione.G2G_PianoConcimazione_Testata_Recode_update.Count

            If recordCoinvolti = 0 Then
                Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Piani Concimazione: Nessun dato da trasferire ")
                Exit Sub
            End If

            Dim sXmlPianiConcimazione As String = AgronicaCoreUtility.XMLUtility.SerializzaOggetto(Of G2G_PianoConcimazione)(oPianiConcimazione, "")

            Dim mainDoc As String = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)

            Dim sStringaDaSalvare As String = AgronicaCoreUtility.XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlPianiConcimazione, "//utente/DatiImprese/Impresa", "DatiPianiConcimazioni")

            Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS With {.Url = objOpzioni.wsimportaGiasURl, .Timeout = _timeout_ws_Importa}
            Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

            '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
            sStringaDaSalvare = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvare)

            Dim outputPianiConcimazione As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, sStringaDaSalvare)

            'leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
            Dim xmlRisposta As XDocument = XDocument.Parse(outputPianiConcimazione)
            Dim xmlRispostaAllegati As XElement = xmlRisposta.Element("Risposta").Element("Risposta_DatiPianiConcimazioni")
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
                    Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Piani Concimazione: " & risposta.LogRecode)
                Else
                    'nuove, modificate, eliminate
                    Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Piani Concimazione - Testata: " & oPianiConcimazione.pianoconcimazione_testata_insert.Count & " nuovi " & oPianiConcimazione.pianoconcimazione_testata_update.Count & " modificati " & oPianiConcimazione.G2G_PianoConcimazione_Testata_Recode_delete.Count & " cancellati ")
                    Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Piani Concimazione - Dettagli: " & oPianiConcimazione.pianoconcimazione_dettagli_insert.Count & " nuovi " & oPianiConcimazione.pianoconcimazione_dettagli_update.Count & " modificati " & oPianiConcimazione.G2G_PianoConcimazione_Dettagli_Recode_delete.Count & " cancellati ")
                    Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Piani Concimazione - EntitaXTestata: " & oPianiConcimazione.pianoconcimazione_entitaxtestata_insert.Count & " nuovi " & oPianiConcimazione.G2G_PianoConcimazione_EntitaxTestata_Recode_delete.Count & " cancellati ")
                    Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Piani Concimazione - FattoriCorrettivi: " & oPianiConcimazione.pianoconcimazione_fattoricorrettivi_insert.Count & " nuovi " & oPianiConcimazione.pianoconcimazione_fattoricorrettivi_delete.Count & " cancellati ")
                    Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Piani Concimazione - Elaborazioni: " & oPianiConcimazione.pianoconcimazione_elaborazioni_insert.Count & " nuovi " & oPianiConcimazione.pianoconcimazione_elaborazioni_update.Count & " modificati " & oPianiConcimazione.G2G_PianoConcimazione_Elaborazioni_Recode_delete.Count & " cancellati ")
                End If
            Else
                Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Piani Concimazione ERRORE: " & MessaggioErrore)
            End If

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message
            Log_Import.AppendLine(msg)
            Log_Errori.AppendLine(msg)
            Throw New Exception(msg)
        End Try

    End Sub

    Public Sub Elabora_XML_Piano_Concimazione_SalvaReverse(
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

        Const NomeFunzione As String = "Elabora_XML_Piano_Concimazione_SalvaReverse"

        Try
            Dim leggiPianiConcimazione As New AgronicaCoreG2GLocalDal.G2GPianoConcimazione_R
            Dim oPianiConcimazione As AgronicaCoreModello.G2G_PianoConcimazione_Reverse =
                leggiPianiConcimazione.LeggiPerGias2GiasReverse(objOpzioniImportImpresa, Piva_Origine, Piva_Destinazione, objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser, objOpzioni.objParametri_Server_GIAS_ORIGINE)

            'QUI CAMBIO LE CHIAVI DI Centro, Fabbricato, Appezza, Id_Reg
            For Each pianoConcimazione_Dettagli_insert In oPianiConcimazione.pianoconcimazione_dettagli_insert
                If pianoConcimazione_Dettagli_insert.PC_Dettagli_SaCod <> 0 Then
                    Dim centro = (From rr In efG2G.G2G_Recode_Imprese Where rr.To_PivaSuperUser = pianoConcimazione_Dettagli_insert.PC_Dettagli_SuperUser AndAlso rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.To_Piva = Piva_Origine AndAlso rr.TO_SaCod = pianoConcimazione_Dettagli_insert.PC_Dettagli_SaCod).FirstOrDefault()
                    pianoConcimazione_Dettagli_insert.PC_Dettagli_SaCod = centro.FROM_SaCod
                End If
            Next

            For Each pianoConcimazione_Dettagli_update In oPianiConcimazione.pianoconcimazione_dettagli_update
                If pianoConcimazione_Dettagli_update.PC_Dettagli_SaCod <> 0 Then
                    Dim centro = (From rr In efG2G.G2G_Recode_Imprese Where rr.To_PivaSuperUser = pianoConcimazione_Dettagli_update.PC_Dettagli_SuperUser AndAlso rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.To_Piva = Piva_Origine AndAlso rr.TO_SaCod = pianoConcimazione_Dettagli_update.PC_Dettagli_SaCod).FirstOrDefault()
                    pianoConcimazione_Dettagli_update.PC_Dettagli_SaCod = centro.FROM_SaCod
                End If
            Next

            For Each pianoConcimazione_EntitaxTestata_insert In oPianiConcimazione.pianoconcimazione_entitaxtestata_insert
                Dim sa_cod_origine As Integer = pianoConcimazione_EntitaxTestata_insert.Sa_Cod
                Dim appezza_origine As Integer = pianoConcimazione_EntitaxTestata_insert.Appezza
                Dim idImp_origine As Integer = pianoConcimazione_EntitaxTestata_insert.Id_Imp

                If pianoConcimazione_EntitaxTestata_insert.Sa_Cod <> 0 Then
                    Dim centro = (From rr In efG2G.G2G_Recode_Imprese Where rr.To_PivaSuperUser = pianoConcimazione_EntitaxTestata_insert.PC_SuperUser AndAlso rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.To_Piva = Piva_Origine AndAlso rr.TO_SaCod = pianoConcimazione_EntitaxTestata_insert.Sa_Cod).FirstOrDefault()
                    pianoConcimazione_EntitaxTestata_insert.Sa_Cod = centro.FROM_SaCod
                End If

                If pianoConcimazione_EntitaxTestata_insert.Campo_Cod <> 0 Then
                    Dim campo = (From rr In efG2G.G2G_Recode_Campo Where rr.To_PivaSuperUser = pianoConcimazione_EntitaxTestata_insert.PC_SuperUser AndAlso rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.To_Piva = Piva_Origine AndAlso rr.To_Sa_Cod = sa_cod_origine AndAlso rr.To_Campo_cod = pianoConcimazione_EntitaxTestata_insert.Campo_Cod).FirstOrDefault()
                    pianoConcimazione_EntitaxTestata_insert.Campo_Cod = campo.From_Campo_cod
                End If

                If pianoConcimazione_EntitaxTestata_insert.Appezza <> 0 Then
                    Dim appezza = (From rr In efG2G.G2G_Recode_Appezzamenti Where rr.To_PivaSuperUser = pianoConcimazione_EntitaxTestata_insert.PC_SuperUser AndAlso rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.To_Piva = Piva_Origine AndAlso rr.To_Sa_Cod = sa_cod_origine AndAlso rr.To_Appezza = pianoConcimazione_EntitaxTestata_insert.Appezza).FirstOrDefault()
                    pianoConcimazione_EntitaxTestata_insert.Appezza = appezza.From_Appezza
                End If

                If pianoConcimazione_EntitaxTestata_insert.Id_Imp <> 0 Then
                    Dim impianto = (From rr In efG2G.G2G_Recode_Impianti Where rr.To_PivaSuperUser = pianoConcimazione_EntitaxTestata_insert.PC_SuperUser AndAlso rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.To_Piva = Piva_Origine AndAlso rr.To_Sa_Cod = sa_cod_origine AndAlso rr.To_Appezza = appezza_origine AndAlso rr.To_Id_Reg = pianoConcimazione_EntitaxTestata_insert.Id_Imp).FirstOrDefault()
                    pianoConcimazione_EntitaxTestata_insert.Id_Imp = impianto.From_Id_Reg
                End If

                If pianoConcimazione_EntitaxTestata_insert.Progetto_Cod <> 0 Then
                    Dim progetto = (From rr In efG2G.G2G_Recode_Distinta Where rr.To_PivaSuperUser = pianoConcimazione_EntitaxTestata_insert.PC_SuperUser AndAlso rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.To_Piva = Piva_Origine AndAlso rr.To_Progetto_cod = pianoConcimazione_EntitaxTestata_insert.Progetto_Cod).FirstOrDefault()
                    pianoConcimazione_EntitaxTestata_insert.Progetto_Cod = progetto.From_Progetto_cod
                End If

                If pianoConcimazione_EntitaxTestata_insert.Fabbricato_Cod <> 0 Then
                    Dim fabbricato = (From rr In efG2G.G2G_Recode_Fabbricati Where rr.To_PivaSuperUser = pianoConcimazione_EntitaxTestata_insert.PC_SuperUser AndAlso rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.ToPiva = Piva_Origine AndAlso rr.ToSa_cod = sa_cod_origine AndAlso rr.To_FabbricatoCod = pianoConcimazione_EntitaxTestata_insert.Fabbricato_Cod).FirstOrDefault()
                    pianoConcimazione_EntitaxTestata_insert.Fabbricato_Cod = fabbricato.From_FabbricatoCod
                End If

                pianoConcimazione_EntitaxTestata_insert.Chiave_Albero_Imprese = ""

            Next


            Dim recordCoinvolti = oPianiConcimazione.pianoconcimazione_dettagli_delete.Count _
                                + oPianiConcimazione.pianoconcimazione_dettagli_insert.Count _
                                + oPianiConcimazione.pianoconcimazione_dettagli_update.Count _
                                + oPianiConcimazione.pianoconcimazione_elaborazioni_delete.Count _
                                + oPianiConcimazione.pianoconcimazione_elaborazioni_insert.Count _
                                + oPianiConcimazione.pianoconcimazione_elaborazioni_update.Count _
                                + oPianiConcimazione.pianoconcimazione_entitaxtestata_delete.Count _
                                + oPianiConcimazione.pianoconcimazione_entitaxtestata_insert.Count _
                                + oPianiConcimazione.pianoconcimazione_fattoricorrettivi_delete.Count _
                                + oPianiConcimazione.pianoconcimazione_fattoricorrettivi_insert.Count _
                                + oPianiConcimazione.pianoconcimazione_testata_delete.Count _
                                + oPianiConcimazione.pianoconcimazione_testata_insert.Count _
                                + oPianiConcimazione.pianoconcimazione_testata_update.Count _
                                + oPianiConcimazione.G2G_PianoConcimazione_Dettagli_Recode_delete.Count _
                                + oPianiConcimazione.G2G_PianoConcimazione_Dettagli_Recode_insert.Count _
                                + oPianiConcimazione.G2G_PianoConcimazione_Dettagli_Recode_update.Count _
                                + oPianiConcimazione.G2G_PianoConcimazione_Elaborazioni_Recode_delete.Count _
                                + oPianiConcimazione.G2G_PianoConcimazione_Elaborazioni_Recode_insert.Count _
                                + oPianiConcimazione.G2G_PianoConcimazione_Elaborazioni_Recode_update.Count _
                                + oPianiConcimazione.G2G_PianoConcimazione_EntitaxTestata_Recode_delete.Count _
                                + oPianiConcimazione.G2G_PianoConcimazione_EntitaxTestata_Recode_insert.Count _
                                + oPianiConcimazione.G2G_PianoConcimazione_Testata_Recode_delete.Count _
                                + oPianiConcimazione.G2G_PianoConcimazione_Testata_Recode_insert.Count _
                                + oPianiConcimazione.G2G_PianoConcimazione_Testata_Recode_update.Count

            If recordCoinvolti = 0 Then
                Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Piani Concimazione: Nessun dato da trasferire ")
                Exit Sub
            End If

            Dim sXmlPianiConcimazione As String = AgronicaCoreUtility.XMLUtility.SerializzaOggetto(Of G2G_PianoConcimazione_Reverse)(oPianiConcimazione, "")

            Dim mainDoc As String = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)

            Dim sStringaDaSalvare As String = AgronicaCoreUtility.XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlPianiConcimazione, "//utente/DatiImprese/Impresa", "DatiPianiConcimazioni")

            Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS With {.Url = objOpzioni.wsimportaGiasURl, .Timeout = _timeout_ws_Importa}
            Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

            '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
            sStringaDaSalvare = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvare)

            Dim outputPianiConcimazione As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, sStringaDaSalvare)

            'leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
            Dim xmlRisposta As XDocument = XDocument.Parse(outputPianiConcimazione)
            Dim xmlRispostaAllegati As XElement = xmlRisposta.Element("Risposta").Element("Risposta_DatiPianiConcimazioni")
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
                    Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Piani Concimazione: " & risposta.LogRecode)
                Else
                    'nuove, modificate, eliminate
                    Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Piani Concimazione - Testata: " & oPianiConcimazione.pianoconcimazione_testata_insert.Count & " nuovi " & oPianiConcimazione.pianoconcimazione_testata_update.Count & " modificati " & oPianiConcimazione.G2G_PianoConcimazione_Testata_Recode_delete.Count & " cancellati ")
                    Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Piani Concimazione - Dettagli: " & oPianiConcimazione.pianoconcimazione_dettagli_insert.Count & " nuovi " & oPianiConcimazione.pianoconcimazione_dettagli_update.Count & " modificati " & oPianiConcimazione.G2G_PianoConcimazione_Dettagli_Recode_delete.Count & " cancellati ")
                    Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Piani Concimazione - EntitaXTestata: " & oPianiConcimazione.pianoconcimazione_entitaxtestata_insert.Count & " nuovi " & oPianiConcimazione.G2G_PianoConcimazione_EntitaxTestata_Recode_delete.Count & " cancellati ")
                    Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Piani Concimazione - FattoriCorrettivi: " & oPianiConcimazione.pianoconcimazione_fattoricorrettivi_insert.Count & " nuovi " & oPianiConcimazione.pianoconcimazione_fattoricorrettivi_delete.Count & " cancellati ")
                    Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Piani Concimazione - Elaborazioni: " & oPianiConcimazione.pianoconcimazione_elaborazioni_insert.Count & " nuovi " & oPianiConcimazione.pianoconcimazione_elaborazioni_update.Count & " modificati " & oPianiConcimazione.G2G_PianoConcimazione_Elaborazioni_Recode_delete.Count & " cancellati ")
                End If
            Else
                Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Piani Concimazione ERRORE: " & MessaggioErrore)
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
