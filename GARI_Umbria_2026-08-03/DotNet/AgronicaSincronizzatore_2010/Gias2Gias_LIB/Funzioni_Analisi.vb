Imports System.Text
Imports System.Xml
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreG2GLocalDal
Imports AgronicaCoreModello
Imports AgronicaCoreUtility

Partial Public Class Funzioni



    Public Sub Elabora_XML_Analisi_Salva(
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

        Const NomeFunzione As String = "Elabora_XML_Analisi_Salva"

        Try
            Dim leggiAnalisi As New AgronicaCoreG2GLocalDal.G2GAnalisi_R
            Dim oAnalisi As AgronicaCoreModello.G2G_Analisi = leggiAnalisi.LeggiPerGias2Gias(objOpzioniImportImpresa, Piva_Origine, Piva_Destinazione, objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser, objOpzioni.objParametri_Server_GIAS_ORIGINE)

            'QUI CAMBIO LE CHIAVI DI Centro, Fabbricato, Appezza, Id_Reg
            For Each analisi_entita In oAnalisi.analisi_entitaxtestata_insert
                Dim sa_cod_origine As Integer = analisi_entita.Sa_Cod
                Dim appezza_origine As Integer = analisi_entita.Appezza

                If analisi_entita.Sa_Cod <> 0 Then
                    Dim centro = (From rr In efG2G.G2G_Recode_Imprese Where rr.From_PivaSuperUser = analisi_entita.Analisi_SuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.FROM_Piva = Piva_Origine AndAlso rr.FROM_SaCod = analisi_entita.Sa_Cod).FirstOrDefault()
                    analisi_entita.Sa_Cod = centro.TO_SaCod
                End If

                If analisi_entita.Campo_Cod <> 0 Then
                    Dim campo = (From rr In efG2G.G2G_Recode_Campo Where rr.From_PivaSuperUser = analisi_entita.Analisi_SuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.From_Piva = Piva_Origine AndAlso rr.From_Sa_Cod = sa_cod_origine AndAlso rr.From_Campo_cod = analisi_entita.Campo_Cod).FirstOrDefault()
                    analisi_entita.Campo_Cod = campo.To_Campo_cod
                End If

                If analisi_entita.Appezza <> 0 Then
                    Dim appezza = (From rr In efG2G.G2G_Recode_Appezzamenti Where rr.From_PivaSuperUser = analisi_entita.Analisi_SuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.From_Piva = Piva_Origine AndAlso rr.From_Sa_Cod = sa_cod_origine AndAlso rr.From_Appezza = analisi_entita.Appezza).FirstOrDefault()
                    analisi_entita.Appezza = appezza.To_Appezza
                End If

                If analisi_entita.Id_Imp <> 0 Then
                    Dim impianto = (From rr In efG2G.G2G_Recode_Impianti Where rr.From_PivaSuperUser = analisi_entita.Analisi_SuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.From_Piva = Piva_Origine AndAlso rr.From_Sa_Cod = sa_cod_origine AndAlso rr.From_Appezza = appezza_origine AndAlso rr.From_Id_Reg = analisi_entita.Id_Imp).FirstOrDefault()
                    analisi_entita.Id_Imp = impianto.To_Id_Reg
                End If

                If analisi_entita.Fabbricato_Cod <> 0 Then
                    Dim fabbricato = (From rr In efG2G.G2G_Recode_Fabbricati Where rr.From_PivaSuperUser = analisi_entita.Analisi_SuperUser AndAlso rr.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.FromPiva = Piva_Origine AndAlso rr.FromSa_cod = sa_cod_origine AndAlso rr.From_FabbricatoCod = analisi_entita.Fabbricato_Cod).FirstOrDefault()
                    analisi_entita.Fabbricato_Cod = fabbricato.To_FabbricatoCod
                End If

            Next

            Dim sXmlAnalisi As String = AgronicaCoreUtility.XMLUtility.SerializzaOggetto(Of G2G_Analisi)(oAnalisi, "")

            Dim mainDoc As String = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)

            Dim sStringaDaSalvare As String = AgronicaCoreUtility.XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlAnalisi, "//utente/DatiImprese/Impresa", "DatiAnalisi")

            Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS With {.Url = objOpzioni.wsimportaGiasURl, .Timeout = _timeout_ws_Importa}
            Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

            '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
            sStringaDaSalvare = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvare)

            Dim outputAllegati As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, sStringaDaSalvare)

            'leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
            Dim xmlRisposta As XDocument = XDocument.Parse(outputAllegati)
            Dim xmlRispostaAllegati As XElement = xmlRisposta.Element("Risposta").Element("Risposta_DatiAnalisi")
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
                    Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Analisi: " & risposta.LogRecode)
                Else
                    'nuove, modificate, eliminate
                    Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Analisi - Certificati: " & oAnalisi.analisi_certificato_insert.Count & " nuovi " & oAnalisi.analisi_certificato_update.Count & " modificati " & oAnalisi.G2G_Analisi_Certificato_Recode_delete.Count & " cancellati ")
                    Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Analisi - Testate: " & oAnalisi.analisi_testata_insert.Count & " nuovi " & oAnalisi.analisi_testata_update.Count & " modificati " & oAnalisi.G2G_Analisi_Testata_Recode_delete.Count & " cancellati ")
                    Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Analisi - EntitaXTestata: " & oAnalisi.analisi_entitaxtestata_insert.Count & " nuovi " & oAnalisi.G2G_Analisi_EntitaxTestata_Recode_delete.Count & " cancellati ")
                    Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Analisi - Dettagli: " & oAnalisi.analisi_dettagli_insert.Count & " nuovi " & oAnalisi.analisi_dettagli_update.Count & " modificati " & oAnalisi.G2G_Analisi_Dettagli_Recode_delete.Count & " cancellati ")
                End If
            Else
                Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Analisi ERRORE: " & MessaggioErrore)
            End If

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message
            Log_Import.AppendLine(msg)
            Log_Errori.AppendLine(msg)
            Throw New Exception(msg)
        End Try

    End Sub

    Public Sub Elabora_XML_Analisi_SalvaReverse(
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

        Const NomeFunzione As String = "Elabora_XML_Analisi_SalvaReverse"

        Try
            Dim leggiAnalisi As New AgronicaCoreG2GLocalDal.G2GAnalisi_R
            Dim oAnalisi As AgronicaCoreModello.G2G_Analisi_Reverse = leggiAnalisi.LeggiPerGias2GiasReverse(objOpzioniImportImpresa, Piva_Origine, Piva_Destinazione, objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser, objOpzioni.objParametri_Server_GIAS_ORIGINE)

            'QUI CAMBIO LE CHIAVI DI Centro, Fabbricato, Appezza, Id_Reg
            For Each analisi_entita In oAnalisi.analisi_entitaxtestata_insert
                Dim sa_cod_origine As Integer = analisi_entita.Sa_Cod
                Dim appezza_origine As Integer = analisi_entita.Appezza

                If analisi_entita.Sa_Cod <> 0 Then
                    Dim centro = (From rr In efG2G.G2G_Recode_Imprese Where rr.To_PivaSuperUser = analisi_entita.Analisi_SuperUser AndAlso rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.To_Piva = Piva_Origine AndAlso rr.TO_SaCod = analisi_entita.Sa_Cod).FirstOrDefault()
                    If centro Is Nothing Then
                        Throw New Exception("Recode centro non trovato To_Piva:" & Piva_Origine & " To_Sa_Cod:" & analisi_entita.Sa_Cod)
                    End If
                    analisi_entita.Sa_Cod = centro.FROM_SaCod
                End If

                If analisi_entita.Campo_Cod <> 0 Then
                    Dim campo = (From rr In efG2G.G2G_Recode_Campo Where rr.To_PivaSuperUser = analisi_entita.Analisi_SuperUser AndAlso rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.To_Piva = Piva_Origine AndAlso rr.To_Sa_Cod = sa_cod_origine AndAlso rr.To_Campo_cod = analisi_entita.Campo_Cod).FirstOrDefault()
                    If campo Is Nothing Then
                        Throw New Exception("Recode campo non trovato To_Piva:" & Piva_Origine & " To_Sa_Cod:" & sa_cod_origine & " To_Campo_Cod:" & analisi_entita.Campo_Cod)
                    End If
                    analisi_entita.Campo_Cod = campo.From_Campo_cod
                End If

                If analisi_entita.Appezza <> 0 Then
                    Dim appezza = (From rr In efG2G.G2G_Recode_Appezzamenti Where rr.To_PivaSuperUser = analisi_entita.Analisi_SuperUser AndAlso rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.To_Piva = Piva_Origine AndAlso rr.To_Sa_Cod = sa_cod_origine AndAlso rr.To_Appezza = analisi_entita.Appezza).FirstOrDefault()
                    If appezza Is Nothing Then
                        Throw New Exception("Recode appezzamento non trovato To_Piva:" & Piva_Origine & " To_Sa_Cod:" & sa_cod_origine & " To_Appezza_Cod:" & analisi_entita.Appezza)
                    End If
                    analisi_entita.Appezza = appezza.From_Appezza
                End If

                If analisi_entita.Id_Imp <> 0 Then
                    Dim impianto = (From rr In efG2G.G2G_Recode_Impianti Where rr.To_PivaSuperUser = analisi_entita.Analisi_SuperUser AndAlso rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.To_Piva = Piva_Origine AndAlso rr.To_Sa_Cod = sa_cod_origine AndAlso rr.To_Appezza = appezza_origine AndAlso rr.To_Id_Reg = analisi_entita.Id_Imp).FirstOrDefault()
                    If impianto Is Nothing Then
                        Throw New Exception("Recode impianto non trovato To_Piva:" & Piva_Origine & " To_Sa_Cod:" & sa_cod_origine & " To_Appezza_Cod:" & appezza_origine & " To_Id_Reg:" & analisi_entita.Id_Imp)
                    End If
                    analisi_entita.Id_Imp = impianto.From_Id_Reg
                End If

                If analisi_entita.Fabbricato_Cod <> 0 Then
                    Dim fabbricato = (From rr In efG2G.G2G_Recode_Fabbricati Where rr.To_PivaSuperUser = analisi_entita.Analisi_SuperUser AndAlso rr.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser AndAlso rr.ToPiva = Piva_Origine AndAlso rr.ToSa_cod = sa_cod_origine AndAlso rr.To_FabbricatoCod = analisi_entita.Fabbricato_Cod).FirstOrDefault()
                    If fabbricato Is Nothing Then
                        Throw New Exception("Recode fabbricato non trovato To_Piva:" & Piva_Origine & " To_Sa_Cod:" & sa_cod_origine & " To_Fabbricato:" & analisi_entita.Fabbricato_Cod)
                    End If
                    analisi_entita.Fabbricato_Cod = fabbricato.From_FabbricatoCod
                End If

            Next

            Dim sXmlAnalisi As String = AgronicaCoreUtility.XMLUtility.SerializzaOggetto(Of G2G_Analisi_Reverse)(oAnalisi, "")

            Dim mainDoc As String = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)

            Dim sStringaDaSalvare As String = AgronicaCoreUtility.XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlAnalisi, "//utente/DatiImprese/Impresa", "DatiAnalisi")

            Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS With {.Url = objOpzioni.wsimportaGiasURl, .Timeout = _timeout_ws_Importa}
            Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

            '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
            sStringaDaSalvare = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvare)

            Dim outputAllegati As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, sStringaDaSalvare)

            'leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
            Dim xmlRisposta As XDocument = XDocument.Parse(outputAllegati)
            Dim xmlRispostaAllegati As XElement = xmlRisposta.Element("Risposta").Element("Risposta_DatiAnalisi")
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
                    Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Analisi: " & risposta.LogRecode)
                Else
                    'nuove, modificate, eliminate
                    Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Analisi - Certificati: " & oAnalisi.analisi_certificato_insert.Count & " nuovi " & oAnalisi.analisi_certificato_update.Count & " modificati " & oAnalisi.G2G_Analisi_Certificato_Recode_delete.Count & " cancellati ")
                    Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Analisi - Testate: " & oAnalisi.analisi_testata_insert.Count & " nuovi " & oAnalisi.analisi_testata_update.Count & " modificati " & oAnalisi.G2G_Analisi_Testata_Recode_delete.Count & " cancellati ")
                    Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Analisi - EntitaXTestata: " & oAnalisi.analisi_entitaxtestata_insert.Count & " nuovi " & oAnalisi.G2G_Analisi_EntitaxTestata_Recode_delete.Count & " cancellati ")
                    Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Analisi - Dettagli: " & oAnalisi.analisi_dettagli_insert.Count & " nuovi " & oAnalisi.analisi_dettagli_update.Count & " modificati " & oAnalisi.G2G_Analisi_Dettagli_Recode_delete.Count & " cancellati ")
                End If
            Else
                Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Analisi ERRORE: " & MessaggioErrore)
            End If

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message
            Log_Import.AppendLine(msg)
            Log_Errori.AppendLine(msg)
            Throw New Exception(msg)
        End Try

    End Sub



    '##############################################################
    'Per ogni impresa, legge le analisi utilizzate
    'e salva i testa_cod nell'hashtable
    Public Sub Import_PrelevaTestataCod(
                                       ByVal Piva_Origine As String,
                                        ByVal Piva_Destinazione As String,
                                       ByVal objOpzioni As clsOpzioni,
                                     ByRef Log_Import As StringBuilder,
                                     ByRef Log_Errori As StringBuilder,
                                     ByRef Log_Riepilogo As StringBuilder,
                                    ByRef HT_Testata_Cod As Hashtable,
                                    ByRef TipoAnalisi As enum_AnalisiTipo)

        Dim DT_TestataCOD As DataTable
        Dim Filtro_Testata As String
        Dim NewTestataCod As Integer
        Dim objA_Testata_R As New AgronicaCoreAnagrafeDAL.Analisi_Testata_R
        Dim objSequenze As New AgronicaCoreDataProvider.Agro_Sequenze

        '=======================================================
        '========= 1) LETTURA ANALISI DA... A... =============
        '=======================================================
        'Filtro_Testata = " ( Analisi_Testata_Data_Inizio <= " & Agro_SQL_SaveDate(objConfig.Data_Rif_Analisi) & _
        '                 " AND Analisi_Testata_Data_Fine >= " & Agro_SQL_SaveDate(objConfig.Data_Rif_Analisi) & ")"

        Filtro_Testata = " ( Analisi_Testata.Analisi_Testata_Tipo =" & Agro_SQL_SaveNum(TipoAnalisi) & " )"

        'objConfig.Piva_Filtro

        If Piva_Origine = "" Then
            'lettura dei testata_cod 
            DT_TestataCOD = objA_Testata_R.DistinctTestataCod(Filtro_Testata, "", objOpzioni.objParametri_Server_GIAS_ORIGINE)
        Else
            'lettura dei testata_cod 
            DT_TestataCOD = objA_Testata_R.DistinctTestataCod_conFiltroPiva(Piva_Origine,
                                                                            Filtro_Testata,
                                                                            "", objOpzioni.objParametri_Server_GIAS_ORIGINE)
        End If

        If Not IsNothing(DT_TestataCOD) AndAlso DT_TestataCOD.Rows.Count > 0 Then

            Dim j As Integer
            For j = 0 To DT_TestataCOD.Rows.Count - 1

                NewTestataCod = objSequenze.NuovoId_Tabella(CStr("ANALISI_TESTATA"),
                                          objOpzioni.BaseCode_DESTINAZIONE,
                                          objOpzioni.TopCode_DESTINAZIONE,
                                           objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)

                'intanto che ci sono aggiungo già le chiavi all'ht
                If Not HT_Testata_Cod.Contains(DT_TestataCOD.Rows(j).Item("Analisi_Testata_Cod")) Then
                    HT_Testata_Cod.Add(DT_TestataCOD.Rows(j).Item("Analisi_Testata_Cod"), NewTestataCod)

                    FunzioniGLOBAL.RecodeAnalisi.Add(New Recode_Analisi With {.FromAnalisi_Testata_Cod = DT_TestataCOD.Rows(j).Item("Analisi_Testata_Cod"), .ToAnalisi_Testata_cod = NewTestataCod})
                End If
            Next


        Else
            'objConfig.Data_Rif_Analisi()
            Log_Errori.Append(CStr(Date.Now) & " - " & "non è stato trovato alcuna analisi valida alla data " & CStr(Now) & vbCrLf & vbCrLf)
            Exit Sub
        End If


    End Sub

    '##############################################################
    Public Function FiltroTestataCOD_from_TestataCOD(ByVal HT_Testata_Cod As Hashtable) As String

        Dim Filtro_TestataCOD As String = ""

        For Each key In HT_Testata_Cod.Keys

            'Filtro_TestataCOD &= CStr(DT_TestataCOD.Rows(j).Item("Analisi_Testata_Cod")) 6 ","
            Filtro_TestataCOD &= CStr(key) & ","

        Next

        If HT_Testata_Cod.Count = 0 Then
            Filtro_TestataCOD = " Analisi_Testata_Cod IN (-1)"
        Else
            Filtro_TestataCOD = " Analisi_Testata_Cod IN (" & Left(Filtro_TestataCOD, Filtro_TestataCOD.Length - 1) & ")"
        End If



        Return Filtro_TestataCOD

    End Function


    '##################################
    Public Sub Import_Analisi_Gias2Gias(
                                        ByVal Piva_Origine As String,
                                        ByVal Piva_Destinazione As String,
                                         ByVal objOpzioni As clsOpzioni,
                                         ByRef Log_Import As StringBuilder,
                                         ByRef Log_Errori As StringBuilder,
                                         ByRef Log_Riepilogo As StringBuilder,
                                         ByRef HT_Testata_Cod As Hashtable,
                                         ByVal Filtro_TestataCOD As String,
                                         ByRef ErrorLog As Integer
                                        )

        'ByRef HT_Dettaglio_Cod As Hashtable, _
        'ByRef HT_Certificato_Cod As Hashtable, _
        'ByRef HT_Campione_Cod As Hashtable)

        Dim HT_Dettaglio_Cod As New Hashtable
        Dim HT_Certificato_Cod As New Hashtable
        Dim HT_Campione_Cod As New Hashtable


        Const NomeFunzione As String = "Import_Analisi_Gias2Gias"

        '1) lettura tabelle analisi, un dt per ognuna
        '2) hashtable delle chiavi vecchie mappate con le chiavi nuove
        '3) chiamata core DAL inviando datatable con chiave nuova
        '4) allegati delle analisi
        '5) tipologia analisi

        Dim objA_EntitaTestata_R As New Analisi_EntitaxTestata_R
        Dim objA_Testata_R As New Analisi_Testata_R
        Dim objA_Dettagli_R As New Analisi_Dettagli_R
        Dim objA_Certificato_R As New Analisi_Certificato_Read
        Dim objA_Campioni_R As New Analisi_Campione_Read
        Dim objA_CampioniDettagli_R As New Analisi_CampionexDet_R

        Dim objA_EntitaTestata_W As New AgronicaCoreAnagrafeDAL.Analisi_EntitaxTestata_W
        Dim objA_Testata_W As New AgronicaCoreAnagrafeDAL.Analisi_Testata_W
        Dim objA_Dettagli_W As New AgronicaCoreAnagrafeDAL.Analisi_Dettagli_W
        Dim objA_Certificato_W As New AgronicaCoreAnagrafeDAL.Analisi_Certificato_W
        Dim objA_Campioni_W As New AgronicaCoreAnagrafeDAL.Analisi_Campione_W
        Dim objA_CampioniDettagli_W As New AgronicaCoreAnagrafeDAL.Analisi_CampionexDet_W

        Dim DT_Testata As DataTable
        Dim DT_EntitaTestata As DataTable
        Dim DT_Dettagli As DataTable
        Dim DT_Certificato As DataTable
        Dim DT_Campioni As DataTable
        Dim DT_CampioniDettagli As DataTable
        Dim dtAllegati_documenti As DataTable
        Dim dtAllegati_EntitaxDocumenti As DataTable

        Dim leggiAllegati_documenti As New Allegati_Documenti_R
        Dim leggiAllegati_EntitaxDocumenti As New Allegati_EntitaxDocumenti_R

        Dim scriviAllegati_documenti As New Allegati_Documenti_W
        Dim scriviAllegati_EntitaxDocumenti As New Allegati_EntitaxDocumenti_W


        Dim n_Testata As Integer
        Dim n_EntitaTestata As Integer
        Dim n_Dettagli As Integer
        Dim n_Certificato As Integer
        Dim n_Campioni As Integer
        Dim n_CampioniDettagli As Integer

        Dim ni_Testata As Integer = 0
        Dim ni_EntitaTestata As Integer = 0
        Dim ni_Dettagli As Integer = 0
        Dim ni_Certificato As Integer = 0
        Dim ni_Campioni As Integer = 0
        Dim ni_CampioniDettagli As Integer = 0


        'Dim Filtro_Dettagli As String
        'Dim Filtro_Certificato As String
        Dim Filtro_Campioni As String = ""
        'Dim Filtro_CampioniDettagli As String

        Dim NewTestataCod As Integer
        Dim NewDettaglioCod As Integer
        Dim NewCertificatoCod As Integer
        Dim NewCampioneCod As Integer
        Dim objSequenze As New AgronicaCoreDataProvider.Agro_Sequenze

        Try

            ''=======================================================
            ''========= 1) LETTURA ANALISI DA... A... =============
            ''=======================================================
            'Filtro_Testata = " ( Analisi_Testata_Data_Inizio <= " & Agro_SQL_SaveDate(objConfig.Data_Rif_Analisi) & _
            '                 " AND Analisi_Testata_Data_Fine >= " & Agro_SQL_SaveDate(objConfig.Data_Rif_Analisi) & ")"

            'If objConfig.Piva_Filtro = "" Then
            '    'lettura dei testata_cod 
            '    DT_TestataCOD = objA_Testata_R.DistinctTestataCod(Filtro_Testata, "", objParametri_Server_GIAS_Origine)
            'Else
            '    'lettura dei testata_cod 
            '    DT_TestataCOD = objA_Testata_R.DistinctTestataCod_conFiltroPiva(objConfig.Piva_Filtro, _
            '                                                                    Filtro_Testata, _
            '                                                                    "", objParametri_Server_GIAS_Origine)
            'End If

            'If Not IsNothing(DT_TestataCOD) AndAlso DT_TestataCOD.Rows.Count > 0 Then
            '    Dim j As Integer
            '    For j = 0 To DT_TestataCOD.Rows.Count - 1
            '        Filtro_TestataCOD &= CStr(DT_TestataCOD.Rows(j).Item("Analisi_Testata_Cod")) & ","

            '        NewTestataCod = objSequenze.NuovoId_Tabella(CStr("ANALISI_TESTATA"), _
            '                                  objConfig.BaseCode, _
            '                                  objConfig.TopCode, _
            '                                   objParametri_Server_GIAS_Destinazione)

            '        'intanto che ci sono aggiungo già le chiavi all'ht
            '        HT_Testata_Cod.Add(DT_TestataCOD.Rows(j).Item("Analisi_Testata_Cod"), NewTestataCod)

            '    Next

            '    Filtro_TestataCOD = " Analisi_Testata_Cod IN (" & Left(Filtro_TestataCOD, Filtro_TestataCOD.Length - 1) & ")"
            'Else
            '    Log_Errori.Append(CStr(Date.Now) & " - " & "non è stato trovato alcuna analisi valida alla data " & CStr(objConfig.Data_Rif_Analisi) & vbCrLf & vbCrLf)
            '    Exit Sub
            'End If



            DT_Testata = objA_Testata_R.Leggi(0, 0,
                                              enumSelezioneVariabile.Selezione_TabellaCompleta,
                                              Filtro_TestataCOD,
                                              "", objOpzioni.objParametri_Server_GIAS_ORIGINE)
            n_Testata = DT_Testata.Rows.Count


            DT_EntitaTestata = objA_EntitaTestata_R.Distinct_TestataCod_Piva(Filtro_TestataCOD,
                                                                            "", objOpzioni.objParametri_Server_GIAS_ORIGINE)
            n_EntitaTestata = DT_EntitaTestata.Rows.Count


            DT_Dettagli = objA_Dettagli_R.Leggi(0, 0, 0,
                                              enumSelezioneVariabile.Selezione_TabellaCompleta,
                                              Filtro_TestataCOD,
                                              "", objOpzioni.objParametri_Server_GIAS_ORIGINE)
            n_Dettagli = DT_Dettagli.Rows.Count

            DT_Certificato = objA_Certificato_R.LeggiConFiltroTestata(0,
                                                                     Filtro_TestataCOD,
                                                                     "", objOpzioni.objParametri_Server_GIAS_ORIGINE)
            n_Certificato = DT_Certificato.Rows.Count

            DT_CampioniDettagli = objA_CampioniDettagli_R.Leggi(0, 0, 0,
                                                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                Filtro_TestataCOD,
                                                                " Analisi_Campione_Cod ASC", objOpzioni.objParametri_Server_GIAS_ORIGINE)
            n_CampioniDettagli = DT_CampioniDettagli.Rows.Count

            If Not IsNothing(DT_CampioniDettagli) AndAlso DT_CampioniDettagli.Rows.Count > 0 Then
                Dim j As Integer
                Dim Temp_Campione As Integer
                For j = 0 To DT_CampioniDettagli.Rows.Count - 1
                    If Temp_Campione <> DT_CampioniDettagli.Rows(j).Item("Analisi_Campione_Cod") Then
                        Temp_Campione = DT_CampioniDettagli.Rows(j).Item("Analisi_Campione_Cod")
                        Filtro_Campioni &= CStr(DT_CampioniDettagli.Rows(j).Item("Analisi_Campione_Cod")) & ","
                    End If
                Next
                Filtro_Campioni = " Analisi_Campione_Cod IN (" & Left(Filtro_Campioni, Filtro_Campioni.Length - 1) & ")"
            End If

            DT_Campioni = objA_Campioni_R.Leggi(0,
                                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                Filtro_Campioni,
                                                "", objOpzioni.objParametri_Server_GIAS_ORIGINE)
            n_Campioni = DT_Campioni.Rows.Count

            Log_Riepilogo.Append(CStr(Date.Now) & " - " & " Record Testata: " & CStr(n_Testata) & vbCrLf & vbCrLf)
            Log_Riepilogo.Append(CStr(Date.Now) & " - " & " Record EntitaTestata: " & CStr(n_EntitaTestata) & vbCrLf & vbCrLf)
            Log_Riepilogo.Append(CStr(Date.Now) & " - " & " Record Dettagli: " & CStr(n_Dettagli) & vbCrLf & vbCrLf)
            Log_Riepilogo.Append(CStr(Date.Now) & " - " & " Record Certificato: " & CStr(n_Certificato) & vbCrLf & vbCrLf)
            Log_Riepilogo.Append(CStr(Date.Now) & " - " & " Record Campioni: " & CStr(n_Campioni) & vbCrLf & vbCrLf)
            Log_Riepilogo.Append(CStr(Date.Now) & " - " & " Record CampioniDettagli: " & CStr(n_CampioniDettagli) & vbCrLf & vbCrLf)


            '=======================================================
            '================ 2) MAPPATURA CHIAVI ==================
            '=======================================================

            'riempimento hash dettaglio_cod
            If Not IsNothing(DT_Dettagli) AndAlso DT_Dettagli.Rows.Count > 0 Then
                Dim j As Integer
                Dim Analisi_Dettaglio_Cod As Integer
                For j = 0 To DT_Dettagli.Rows.Count - 1
                    Analisi_Dettaglio_Cod = DT_Dettagli.Rows(j).Item("Analisi_Dettaglio_Cod")
                    If Not HT_Dettaglio_Cod.Contains(Analisi_Dettaglio_Cod) Then

                        NewDettaglioCod = objSequenze.NuovoId_Tabella(CStr("ANALISI_DETTAGLI"),
                                                                    objOpzioni.BaseCode_DESTINAZIONE,
                                                                    objOpzioni.TopCode_DESTINAZIONE,
                                                                       objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)

                        HT_Dettaglio_Cod.Add(Analisi_Dettaglio_Cod, NewDettaglioCod)

                    End If
                Next
            End If

            'riempimento hash certificato_cod
            If Not IsNothing(DT_Certificato) AndAlso DT_Certificato.Rows.Count > 0 Then
                Dim j As Integer
                Dim Analisi_Certificato_Cod As Integer
                For j = 0 To DT_Certificato.Rows.Count - 1
                    Analisi_Certificato_Cod = DT_Certificato.Rows(j).Item("Analisi_Certificato_Cod")
                    If Not HT_Certificato_Cod.Contains(Analisi_Certificato_Cod) Then

                        NewCertificatoCod = objSequenze.NuovoId_Tabella(CStr("ANALISI_CERTIFICATO"),
                                                            objOpzioni.BaseCode_DESTINAZIONE,
                                                            objOpzioni.TopCode_DESTINAZIONE,
                                               objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)

                        HT_Certificato_Cod.Add(Analisi_Certificato_Cod, NewCertificatoCod)

                    End If
                Next
            End If

            'riempimento hash campione_cod
            If Not IsNothing(DT_Campioni) AndAlso DT_Campioni.Rows.Count > 0 Then
                Dim j As Integer
                Dim Analisi_Campione_Cod As Integer
                For j = 0 To DT_Campioni.Rows.Count - 1
                    Analisi_Campione_Cod = DT_Campioni.Rows(j).Item("Analisi_Campione_Cod")
                    If Not HT_Campione_Cod.Contains(Analisi_Campione_Cod) Then

                        NewCampioneCod = objSequenze.NuovoId_Tabella(CStr("ANALISI_CAMPIONI"),
                                                objOpzioni.BaseCode_DESTINAZIONE,
                                                objOpzioni.TopCode_DESTINAZIONE,
                                                 objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)

                        HT_Campione_Cod.Add(Analisi_Campione_Cod, NewCampioneCod)

                    End If
                Next
            End If

            '=======================================================
            '========= 3) SCRITTURA ANALISI DB DESTINAZIONE ========
            '=======================================================
            Dim i As Integer
            Dim Risp As Boolean


            'ANALISI ENTITA X TESTATA
            Dim NEW_HT_EntitaTestata As New Hashtable
            Dim key_EntitaTestata As String
            'non devo scrivere tutto quello che è presente nella tabella Analisi_EntitaxTestata di origine,
            'perchè le analisi possono essere state associate ad entità dal centro in giù 
            '(e le chiavi non sono le stesse del db di destinazione)
            'importo il record associandola all'impresa
            '(di cui si è sicuri che la piva è nel db di destinazione)
            For i = 0 To DT_EntitaTestata.Rows.Count - 1

                With DT_EntitaTestata.Rows(i)

                    NewTestataCod = HT_Testata_Cod(.Item("Analisi_Testata_Cod"))

                    If NewTestataCod = 0 Then
                        Throw New Exception("Importazione EntitaTestata:  NewTestataCod = 0")
                    End If

                    key_EntitaTestata = CStr(.Item("Piva")) & "|" & CStr(NewTestataCod)

                    If Not NEW_HT_EntitaTestata.ContainsKey(key_EntitaTestata) Then

                        NEW_HT_EntitaTestata.Add(key_EntitaTestata, "")

                        Risp = objA_EntitaTestata_W.Scrivi(NewTestataCod,
                                                   enum_Entita_Analisi.Impresa,
                                                    .Item("Piva"),
                                                   0, 0, 0, 0, 0,
                                                   "", "", "", 0, 0, "",
                                                   "",
                                                   0,
                                                    objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.UsernameOperazione,
                                                     Date.Now,
                                                     Date.Now,
                                                     AGRODATAINIZIO,
                                                     AGRODATAFINE,
                                                     objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)

                        ni_EntitaTestata += 1

                    End If

                End With



            Next

            'TESTATA
            For i = 0 To DT_Testata.Rows.Count - 1

                With DT_Testata.Rows(i)

                    NewTestataCod = HT_Testata_Cod(.Item("Analisi_Testata_Cod"))
                    If NewTestataCod = 0 Then
                        Throw New Exception("Importazione Testata:  NewTestataCod = 0")
                    End If

                    If .Item("Analisi_Certificato_Cod") = 0 Then
                        NewCertificatoCod = 0
                    Else
                        NewCertificatoCod = HT_Certificato_Cod(.Item("Analisi_Certificato_Cod"))
                        If NewCertificatoCod = 0 Then
                            Throw New Exception("Importazione Certificato:  NewCertificatoCod = 0")
                        End If
                    End If
                    Risp = objA_Testata_W.Scrivi(NewTestataCod,
                                                NewCertificatoCod,
                                                .Item("Analisi_Testata_Des"),
                                                .Item("Analisi_Testata_Data_Inizio"),
                                                .Item("Analisi_Testata_Data_Fine"),
                                                .Item("Analisi_Testata_Coord_X"),
                                                .Item("Analisi_Testata_Coord_Y"),
                                                .Item("Analisi_Testata_Riferimento_1"),
                                                .Item("Analisi_Testata_Riferimento_2"),
                                                .Item("Analisi_Testata_Riferimento_3"),
                                                .Item("Analisi_Testata_Riferimento_4"),
                                                .Item("Analisi_Testata_Riferimento_5"),
                                                .Item("Analisi_Testata_Note1"),
                                                .Item("Analisi_Testata_Note2"),
                                                .Item("Analisi_Testata_Note3"),
                                                .Item("Analisi_Testata_Note4"),
                                                .Item("Analisi_Testata_Tipo"),
                                                0,
                                                .Item("DataLock"),
                                                .Item("Data_Agg"),
                                                .Item("Validita_Inizio"),
                                                .Item("Validita_Fine"),
                                                objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)

                    ni_Testata += 1

                End With

                'Dim SeqTab As New AgronicaCoreDataProvider.Agro_Sequenze

                Dim VecchioAnalisi_testata_cod As Integer
                VecchioAnalisi_testata_cod = DT_Testata.Rows(i)("Analisi_Testata_Cod")
                Dim nuovoAnalisi_Testata_Cod As Integer
                nuovoAnalisi_Testata_Cod =
                    (From at In FunzioniGLOBAL.RecodeAnalisi
                     Where at.FromAnalisi_Testata_Cod = VecchioAnalisi_testata_cod
                     Select at.ToAnalisi_Testata_cod
                      ).FirstOrDefault


                dtAllegati_EntitaxDocumenti = leggiAllegati_EntitaxDocumenti.Leggi(
                    0,
                    "",
                    0,
                    0,
                    0,
                    0,
                    0,
                    "",
                    "",
                    "",
                    0,
                    0,
                    "",
                    "",
                    "",
                    VecchioAnalisi_testata_cod,
                    0,
                    0,
                    "",
                    "",
                    objOpzioni.objParametri_Server_GIAS_ORIGINE
                    )

                Dim AllegatiFatti As New List(Of recode_AllegatiDocumenti)

                For Each idtAllegati_EntitaxDocumenti As DataRow In dtAllegati_EntitaxDocumenti.Rows

                    Dim iCurAllegatoCod As Integer = idtAllegati_EntitaxDocumenti("Allegati_Documenti_Cod")

                    Dim fatto As Integer? = 0
                    fatto = (
                        From a In AllegatiFatti
                        Where a.FROM_AllegatiDocumentiCod = iCurAllegatoCod
                        Select a.TO_AllegatiDocumentiCod
                        ).FirstOrDefault

                    Dim NuovoAllegatiDocumentiCod As Integer
                    If fatto = 0 Then
                        dtAllegati_documenti = leggiAllegati_documenti.Leggi(
                            iCurAllegatoCod _
                            , enumSelezioneVariabile.Selezione_TabellaCompleta _
                            , "" _
                            , "" _
                            , objOpzioni.objParametri_Server_GIAS_ORIGINE
                        )



                        With dtAllegati_documenti.Rows(0)
                            Dim ldtAllegatiDocumentiNumero As String = ""
                            Dim ldtAllegatiDocumentiEnteCod As Integer = 0

                            If Not IsDBNull(.Item("Allegati_Documenti_Numero")) Then
                                ldtAllegatiDocumentiNumero = .Item("Allegati_Documenti_Numero")
                            End If

                            If Not IsDBNull(.Item("Allegati_Documenti_Ente_Cod")) Then
                                ldtAllegatiDocumentiEnteCod = .Item("Allegati_Documenti_Ente_Cod")
                            End If

                            Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                            Dim dtCatDoc As DataTable = objCatDoc.Leggi(.Item("Allegati_Documenti_CatCod"), "", "", objOpzioni.objParametri_Server_GIAS_ORIGINE)



                            scriviAllegati_documenti.Scrivi(
                                 Piva_Destinazione _
                                , .Item("Allegati_Documenti_DES") _
                                 , .Item("Allegati_Documenti_CatCod") _
                                 , .Item("Allegati_Documenti_NomeFile") _
                                 , ldtAllegatiDocumentiNumero _
                                 , ldtAllegatiDocumentiEnteCod _
                                 , dtCatDoc.Rows(0).Item("Sottocartella") _
                                 , .Item("Validita_Inizio") _
                                 , .Item("Validita_Fine") _
                                 , NuovoAllegatiDocumentiCod _
                                 , objOpzioni.objParametri_Server_GIAS_DESTINAZIONE _
                                 , Data_creazione:= .Item("Data_creazione") _
                                 , Data_modifica:= .Item("Data_modifica") _
                                 , username_creazione:= .Item("username_creazione") _
                                 , username_modifica:= .Item("username_modifica")
                            )

                            AllegatiFatti.Add(New recode_AllegatiDocumenti With {.FROM_AllegatiDocumentiCod = iCurAllegatoCod, .TO_AllegatiDocumentiCod = NuovoAllegatiDocumentiCod})


                        End With

                    End If

                    Dim nuovoAllegatiEntitaCod As Integer
                    'nuovoAllegatiEntitaCod = SeqTab.Agronica_SequenzaTabelle_NuovoID(
                    '        "allegati_entitaxdocumenti",
                    '        objOpzioni.objParametri_Server_GIAS_DESTINAZIONE
                    '        )
                    'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                    nuovoAllegatiEntitaCod = objSequenze.NuovoId_Tabella("allegati_entitaxdocumenti", 0, UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)


                    Dim lDtProgrammazione_Cod As Integer = 0

                    If Not IsDBNull(idtAllegati_EntitaxDocumenti("Programmazione_Cod")) Then
                        lDtProgrammazione_Cod = idtAllegati_EntitaxDocumenti("Programmazione_Cod")
                    End If
                    Dim lDtProgrammazione_Entita_Cod As Integer = 0

                    If Not IsDBNull(idtAllegati_EntitaxDocumenti("Programmazione_Entita_Cod")) Then
                        lDtProgrammazione_Entita_Cod = idtAllegati_EntitaxDocumenti("Programmazione_Entita_Cod")
                    End If



                    scriviAllegati_EntitaxDocumenti.Scrivi(
                        NuovoAllegatiDocumentiCod _
                        , nuovoAllegatiEntitaCod _
                        , idtAllegati_EntitaxDocumenti("piva") _
                        , idtAllegati_EntitaxDocumenti("sa_cod") _
                        , idtAllegati_EntitaxDocumenti("campo_cod") _
                        , idtAllegati_EntitaxDocumenti("appezza") _
                        , idtAllegati_EntitaxDocumenti("id_imp") _
                        , idtAllegati_EntitaxDocumenti("fabbricato_cod") _
                        , idtAllegati_EntitaxDocumenti("prov") _
                        , idtAllegati_EntitaxDocumenti("com") _
                        , idtAllegati_EntitaxDocumenti("sezione") _
                        , idtAllegati_EntitaxDocumenti("foglio") _
                        , idtAllegati_EntitaxDocumenti("numero") _
                        , idtAllegati_EntitaxDocumenti("subalterno") _
                        , idtAllegati_EntitaxDocumenti("id_oggetto_Grafico") _
                        , idtAllegati_EntitaxDocumenti("chiave_albero_imprese") _
                        , nuovoAnalisi_Testata_Cod _
                        , lDtProgrammazione_Cod _
                        , lDtProgrammazione_Entita_Cod _
                        , idtAllegati_EntitaxDocumenti("validita_inizio") _
                        , idtAllegati_EntitaxDocumenti("validita_fine") _
                        , objOpzioni.objParametri_Server_GIAS_DESTINAZIONE _
                        , idtAllegati_EntitaxDocumenti("data_creazione") _
                        , idtAllegati_EntitaxDocumenti("data_modifica") _
                        , idtAllegati_EntitaxDocumenti("username_creazione") _
                        , idtAllegati_EntitaxDocumenti("username_modifica")
                    )

                Next



            Next

            'DETTAGLI
            For i = 0 To DT_Dettagli.Rows.Count - 1

                With DT_Dettagli.Rows(i)

                    NewTestataCod = HT_Testata_Cod(.Item("Analisi_Testata_Cod"))
                    NewDettaglioCod = HT_Dettaglio_Cod(.Item("Analisi_Dettaglio_Cod"))

                    If NewTestataCod = 0 Then
                        Throw New Exception("Importazione Dettagli:  NewTestataCod = 0")
                    End If
                    If NewDettaglioCod = 0 Then
                        Throw New Exception("Importazione Dettagli:  NewDettaglioCod = 0")
                    End If

                    Risp = objA_Dettagli_W.Scrivi(NewTestataCod,
                                                    NewDettaglioCod,
                                                     .Item("Analisi_Parametro_Cod"),
                                                     .Item("Analisi_Dettaglio_Valore_1"),
                                                     .Item("Analisi_Dettaglio_MargineErrore_1"),
                                                     .Item("Analisi_Dettaglio_Valore_2"),
                                                     .Item("Analisi_Dettaglio_MargineErrore_2"),
                                                     .Item("DataLock"),
                                                     .Item("Validita_Inizio"),
                                                     .Item("Validita_Fine"),
                                                    objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)

                    ni_Dettagli += 1

                End With

            Next

            'CERTIFICATI
            For i = 0 To DT_Certificato.Rows.Count - 1

                With DT_Certificato.Rows(i)

                    NewCertificatoCod = HT_Certificato_Cod(.Item("Analisi_Certificato_Cod"))

                    If NewCertificatoCod = 0 Then
                        Throw New Exception("Importazione Certificato:  NewCertificatoCod = 0")
                    End If

                    '.Item("Analisi_Certificato_Laboratorio") sarebbe da rimappare con nuovo contatto laboratorio

                    Risp = objA_Certificato_W.Scrivi(NewCertificatoCod,
                                                    .Item("Analisi_Certificato_Des"),
                                                    .Item("Analisi_Certificato_Data_Inizio"),
                                                    .Item("Analisi_Certificato_Data_Fine"),
                                                    0,
                      .Item("TipologiaCod"),
                                                    .Item("Analisi_Certificato_TipoCampione"),
                                                    .Item("Analisi_Certificato_Provenienza"),
                                                    .Item("Analisi_Certificato_Verbale"),
                                                    .Item("Analisi_Certificato_Richiedente"),
                                                    .Item("Analisi_Certificato_PrelevatoDa"),
                                                    .Item("Analisi_Certificato_Comune"),
                                                    .Item("Analisi_Certificato_Protocollo"),
                                                    .Item("Analisi_Certificato_NumRegistro"),
                                                    .Item("Analisi_Certificato_Sezione"),
                                                    .Item("Analisi_Certificato_DataFirma"),
                                                    .Item("Analisi_Certificato_Responsabile"),
                                                    .Item("Analisi_Certificato_Analista"),
                                                    .Item("Data_Agg"),
                                                    .Item("DataLock"),
                                                    .Item("Validita_Inizio"),
                                                    .Item("Validita_Fine"),
                                                    objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)

                    ni_Certificato += 1

                End With

            Next


            'CAMPIONI DETTAGLI
            For i = 0 To DT_CampioniDettagli.Rows.Count - 1

                With DT_CampioniDettagli.Rows(i)

                    If HT_Testata_Cod.ContainsKey(.Item("Analisi_Testata_Cod")) Then
                        NewTestataCod = HT_Testata_Cod(.Item("Analisi_Testata_Cod"))
                    Else
                        Throw New Exception("Importazione CampioniDettagli:  NewTestataCod = 0")
                    End If

                    If HT_Dettaglio_Cod.ContainsKey(.Item("Analisi_Dettaglio_Cod")) Then
                        NewDettaglioCod = HT_Dettaglio_Cod(.Item("Analisi_Dettaglio_Cod"))
                    Else
                        Throw New Exception("Importazione CampioniDettagli:  NewDettaglioCod = 0")
                    End If

                    If HT_Campione_Cod.ContainsKey(.Item("Analisi_Campione_Cod")) Then
                        NewCampioneCod = HT_Campione_Cod(.Item("Analisi_Campione_Cod"))
                    Else
                        Throw New Exception("Importazione CampioniDettagli:  NewCampioneCod = 0")
                    End If

                    Risp = objA_CampioniDettagli_W.Scrivi(NewTestataCod,
                                                        NewDettaglioCod,
                                                        NewCampioneCod,
                                                        .Item("DataLock"),
                                                         .Item("Validita_Inizio"),
                                                         .Item("Validita_Fine"),
                                                        objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)

                    ni_CampioniDettagli += 1

                End With

            Next

            'CAMPIONI 
            For i = 0 To DT_Campioni.Rows.Count - 1

                With DT_Campioni.Rows(i)

                    NewCampioneCod = HT_Campione_Cod(.Item("Analisi_Campione_Cod"))

                    If NewCampioneCod = 0 Then
                        Throw New Exception("Importazione Campioni:  NewCampioneCod = 0")
                    End If

                    Risp = objA_Campioni_W.Scrivi(NewCampioneCod,
                                                    .Item("Analisi_Campione_Des"),
                                                    .Item("Analisi_Campione_Coord_X"),
                                                    .Item("Analisi_Campione_Coord_Y"),
                                                    .Item("Analisi_Campione_Quantita"),
                                                    .Item("Analisi_Campione_UdM"),
                                                    .Item("Analisi_Campione_Profondita"),
                                                    .Item("Analisi_Campione_Profondita_Min"),
                                                    .Item("Analisi_Campione_Profondita_Max"),
                                                    .Item("Analisi_Campione_Riferimento_1"),
                                                    .Item("Analisi_Campione_Riferimento_2"),
                                                    .Item("Analisi_Campione_Riferimento_3"),
                                                    .Item("Analisi_Campione_Riferimento_4"),
                                                     .Item("Analisi_Campione_Riferimento_5"),
                                                    .Item("Analisi_Campione_Note"),
                                                   "",
                                                    0,
                                                    0,
                                                    .Item("DataLock"),
                                                    .Item("Analisi_Campione_Prov"),
                                                    .Item("Analisi_Campione_Com"),
                                                    .Item("Analisi_Campione_Sezione"),
                                                    .Item("Analisi_Campione_Foglio"),
                                                    .Item("Analisi_Campione_Numero"),
                                                    .Item("Analisi_Campione_Subalterno"),
                                                    .Item("Validita_Inizio"),
                                                    .Item("Validita_Fine"),
                                                     objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)

                    ni_Campioni += 1

                End With

            Next



            Log_Riepilogo.Append(vbCrLf & vbCrLf)
            Log_Riepilogo.Append(CStr(Date.Now) & " - " & " Record importati Testata: " & CStr(ni_Testata) & vbCrLf & vbCrLf)
            Log_Riepilogo.Append(CStr(Date.Now) & " - " & " Record importati EntitaTestata: " & CStr(ni_EntitaTestata) & vbCrLf & vbCrLf)
            Log_Riepilogo.Append(CStr(Date.Now) & " - " & " Record importati Dettagli: " & CStr(ni_Dettagli) & vbCrLf & vbCrLf)
            Log_Riepilogo.Append(CStr(Date.Now) & " - " & " Record importati Certificato: " & CStr(ni_Certificato) & vbCrLf & vbCrLf)
            Log_Riepilogo.Append(CStr(Date.Now) & " - " & " Record importati Campioni: " & CStr(ni_Campioni) & vbCrLf & vbCrLf)
            Log_Riepilogo.Append(CStr(Date.Now) & " - " & " Record importati CampioniDettagli: " & CStr(ni_CampioniDettagli) & vbCrLf & vbCrLf)


        Catch ex As Exception
            Log_Import.Append(CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message & vbCrLf)
            Log_Errori.Append(CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message & vbCrLf)

            ErrorLog = 1
        End Try

    End Sub

End Class


