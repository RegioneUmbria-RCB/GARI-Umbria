Imports System.Xml
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Imports AgronicaCoreDataProvider

Imports System.Text
Imports System.Runtime.Remoting

Partial Public Class Funzioni

#Region "Planning"

    Public Sub Elabora_XML_Planning_Salva_WS(
                                            ByVal objOpzioniImportImpresa As AgronicaCoreModello.clsImpresa,
                                            ByVal objOpzioni As clsOpzioni,
                                            ByRef Log_Import As StringBuilder,
                                            ByRef Log_Errori As StringBuilder,
                                            ByRef Log_Riepilogo As StringBuilder,
                                            ByVal Piva_Origine As String,
                                            ByVal Piva_Destinazione As String
                                    )


        Const NomeFunzione As String = "Elabora_XML_Planning_Salva_WS"
        Dim outputPlanning As String = ""
        Try

            Dim leggiPlanning As New AgronicaCoreG2GLocalDal.G2GPlanning_R
            Dim oPlanning As AgronicaCoreModello.G2G_Planning = leggiPlanning.LeggiPerGias2Gias(Piva_Origine, Piva_Destinazione, objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser, objOpzioniImportImpresa, objOpzioni.objParametri_Server_GIAS_ORIGINE)

            Dim sXmlPlanning As String = AgronicaCoreUtility.XMLUtility.SerializzaOggetto(Of AgronicaCoreModello.G2G_Planning)(oPlanning, "")

            Dim mainDoc As String = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)
            mainDoc = AgronicaCoreUtility.XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlPlanning, "//utente/DatiImprese/Impresa", "DatiPlanning")

            Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS With {.Url = objOpzioni.wsimportaGiasURl, .Timeout = _timeout_ws_Importa}
            Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

            '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
            Dim sStringaDaSalvare As String = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, mainDoc)
            outputPlanning = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, sStringaDaSalvare)

            'leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
            Dim xmlRisposta As XDocument = XDocument.Parse(outputPlanning)
            Dim rispostaPlanning As String = xmlRisposta.Element("Risposta").Element("Risposta_DatiPlanning").FirstNode.ToString

            Dim risposta As AgronicaCoreModello.G2G_Recode = AgronicaCoreUtility.XMLUtility.DeserializzaOggetto(Of AgronicaCoreModello.G2G_Recode)(rispostaPlanning, "")

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
                    Log_Import.AppendLine("Trasferimento Planning: " & risposta.LogRecode)
                End If
            Else
                Log_Import.AppendLine("Trasferimento Planning ERRORE: " & MessaggioErrore)
            End If

            'nuove, modificate, eliminate
            Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Planning - Testata: " & oPlanning.programmazione_testata_insert.Count & " nuovi " & oPlanning.programmazione_testata_update.Count & " modificati " & oPlanning.Recode.G2GRecodeProgrammazioneTestataToDelete.Count & " cancellati ")
            Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Planning - Entita: " & oPlanning.programmazione_entita_insert.Count & " nuovi " & oPlanning.programmazione_entita_update.Count & " modificati " & oPlanning.Recode.G2GRecodeProgrammazioneEntitaToDelete.Count & " cancellati ")

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message
            If Not ex.InnerException Is Nothing Then
                msg &= " InnerException:" & ex.InnerException.Message
            End If
            msg &= "Output planning:" & outputPlanning
            Log_Import.AppendLine(msg)
            Log_Errori.AppendLine(msg)
            Throw New Exception(msg)
        End Try

    End Sub

    Public Sub Elabora_XML_Planning_Salva_WSReverse(
                                            ByVal objOpzioniImportImpresa As AgronicaCoreModello.clsImpresa,
                                            ByVal objOpzioni As clsOpzioni,
                                            ByRef Log_Import As StringBuilder,
                                            ByRef Log_Errori As StringBuilder,
                                            ByRef Log_Riepilogo As StringBuilder,
                                            ByVal Piva_Origine As String,
                                            ByVal Piva_Destinazione As String
                                    )


        Const NomeFunzione As String = "Elabora_XML_Planning_Salva_WSReverse"

        Try

            Dim leggiPlanning As New AgronicaCoreG2GLocalDal.G2GPlanning_R
            Dim oPlanning As AgronicaCoreModello.G2G_Planning_Reverse = leggiPlanning.LeggiPerGias2GiasReverse(Piva_Origine, Piva_Destinazione, objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser, objOpzioniImportImpresa, objOpzioni.objParametri_Server_GIAS_ORIGINE)

            Dim sXmlPlanning As String = AgronicaCoreUtility.XMLUtility.SerializzaOggetto(Of AgronicaCoreModello.G2G_Planning_Reverse)(oPlanning, "")

            Dim mainDoc As String = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)
            mainDoc = AgronicaCoreUtility.XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlPlanning, "//utente/DatiImprese/Impresa", "DatiPlanning")

            Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS With {.Url = objOpzioni.wsimportaGiasURl, .Timeout = _timeout_ws_Importa}
            Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

            '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
            Dim sStringaDaSalvare As String = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, mainDoc)
            Dim outputPlanning As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, sStringaDaSalvare)

            'leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
            Dim xmlRisposta As XDocument = XDocument.Parse(outputPlanning)
            Dim rispostaPlanning As String = xmlRisposta.Element("Risposta").Element("Risposta_DatiPlanning").FirstNode.ToString

            Dim risposta As AgronicaCoreModello.G2G_Recode = AgronicaCoreUtility.XMLUtility.DeserializzaOggetto(Of AgronicaCoreModello.G2G_Recode)(rispostaPlanning, "")

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
                    Log_Import.AppendLine("Trasferimento Planning: " & risposta.LogRecode)
                End If
            Else
                Log_Import.AppendLine("Trasferimento Planning ERRORE: " & MessaggioErrore)
            End If

            'nuove, modificate, eliminate
            Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Planning - Testata: " & oPlanning.programmazione_testata_insert.Count & " nuovi " & oPlanning.programmazione_testata_update.Count & " modificati " & oPlanning.Recode.G2GRecodeProgrammazioneTestataToDelete.Count & " cancellati ")
            Log_Import.AppendLine(CStr(Date.Now) & " - " & "Trasferimento Planning - Entita: " & oPlanning.programmazione_entita_insert.Count & " nuovi " & oPlanning.programmazione_entita_update.Count & " modificati " & oPlanning.Recode.G2GRecodeProgrammazioneEntitaToDelete.Count & " cancellati ")

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message
            If Not ex.InnerException Is Nothing Then
                msg &= " InnerException:" & ex.InnerException.Message
            End If
            Log_Import.AppendLine(msg)
            Log_Errori.AppendLine(msg)
            Throw New Exception(msg)
        End Try

    End Sub

    Public Sub Elabora_Planning_Salva(
                                ByVal objOpzioni As clsOpzioni,
                                ByRef Log_Import As StringBuilder,
                                ByRef Log_Errori As StringBuilder,
                                ByRef Log_Riepilogo As StringBuilder,
                                ByVal Piva_Origine As String,
                                ByVal Sa_cod_Origine As Integer,
                                ByVal Piva_Destinazione As String,
                                ByVal sa_cod_destinazione As Integer
                        )

        Const nomeFunzione As String = "Elabora_Planning_Salva"


        Try

            Dim AppezzaLetto As Integer?
            Dim AppezzaDecodificato As Integer?
            Dim campoCodDecodificato As Integer?
            Dim Id_reg_letto As Integer?
            Dim id_reg_Decodificato As Integer?
            Dim progettoCodLetto As Integer?
            Dim progettoCodDecodificato As Integer?



            Dim MessaggioErrore As String = ""
            Dim ErrMSG As String = ""

            Dim Codice As Integer = 0
            Dim RecordInteressati As Integer = 0
            Dim i, j As Integer
            Dim Dr() As DataRow
            Dim BoolDummy As Boolean

            Dim Dt_Testata_R As New DataTable
            Dim Dt_Entita_R As New DataTable
            Dim Dt_Particelle_R As New DataTable

            Dim Dt_ParticelleCondotte_R As New DataTable

            Dim Tipo_Pianificazione As Integer = 0

            Dim Appezza As Integer = 0
            Dim Campo_Cod As Integer = 0
            Dim Area_Cod As Integer = 0

            Dim scriviAreeOmogeneeXEntita As New AgronicaCoreAnagrafeDAL.Area_OmogeneaxEntita_W
            Dim leggiAreeOmogeneeXEntita As New AgronicaCoreAnagrafeDAL.Area_OmogeneaxEntita_R

            '----------------------------------------------
            'leggo i dati della programmazione da copiare
            Dim objProgrammazione_Testata As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_R

            Dt_Testata_R = objProgrammazione_Testata.Leggi(
                                       ErrMSG,
                                       0,
                                       Piva_Origine,
                                       "",
                                       1,
                                       Estremo_Validita_Inizio,
                                       Estremo_Validita_Fine,
                                       enum_TipoRicetta.Non_Filtrare,
                                       AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                       "", "",
                                       objOpzioni.objParametri_Server_GIAS_ORIGINE)


            If Not Dt_Testata_R Is Nothing AndAlso Dt_Testata_R.Rows.Count > 0 Then


                For Each drowTestata In Dt_Testata_R.Rows

                    Dim From_Programmazione_Cod As Integer =
                        CType(drowTestata("Programmazione_Cod"), Integer)

                    Dim lTO_Programmazione_Cod As Integer
                    lTO_Programmazione_Cod = (
                        From p In _funzioniGLOBAL.Planning
                        Where p.From_Programmazione_Cod = From_Programmazione_Cod _
                        And p.From_Piva = Piva_Origine _
                        And p.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser
                        Select p.To_Programmazione_Cod).FirstOrDefault



                    'CREO LA NUOVA TESTATA, se non è già stata rimappata..
                    If lTO_Programmazione_Cod = 0 Then


                        Dim objProgrammazione_Testata_W As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_W

                        BoolDummy = objProgrammazione_Testata_W.Scrivi(
                                                    lTO_Programmazione_Cod,
                                                    CType(drowTestata("Programmazione_Des"), String),
                                                    CType(drowTestata("Programmazione_Des_Long"), String),
                                                    CType(drowTestata("Piva"), String),
                                                    CType(drowTestata("Note"), String),
                                                    CType(drowTestata("Tipo_Pianificazione"), Integer),
                                                    "0",
                                                    CType(drowTestata("Validita_Inizio"), Date),
                                                    CType(drowTestata("Validita_Fine"), Date),
                                                    objOpzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                                    drowTestata("Data_Creazione"),
                                                    drowTestata("Data_Modifica"),
                                                    drowTestata("Username_Creazione"),
                                                    drowTestata("Username_Modifica")
                                            )
                    End If

                    '----------------------------------------------------
                    'Leggo le entità della programmazione da copiare

                    Dim objProgrammazione_Entita_R As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R

                    Dim TO_Programmazione_Entita_Cod As Integer





                    Dt_Entita_R = objProgrammazione_Entita_R.Programmazione_Entita_Leggi_conAreeOmogenee(
                                                From_Programmazione_Cod,
                                                ErrMSG,
                                                0,
                                                "",
                                                "",
                                                Sa_cod_Origine,
                                                0,
                                                0,
                                                0,
                                                0,
                                                Estremo_Validita_Inizio,
                                                Estremo_Validita_Fine,
                                                2,
                                                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                "",
                                                "",
                                                objOpzioni.objParametri_Server_GIAS_ORIGINE,
                                                Sa_cod_Origine = 0)



                    If Not Dt_Entita_R Is Nothing AndAlso Dt_Entita_R.Rows.Count > 0 Then

                        For Each DrowEntita In Dt_Entita_R.Rows

                            Dim From_Programmazione_Entita_Cod As Integer =
                                CType(DrowEntita("Programmazione_entita_Cod"), Integer)

                            Dim objProgrammazione_Entita_W As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_W

                            LeggiDecodifiche(Piva_Origine, Sa_cod_Origine, nomeFunzione, DrowEntita, AppezzaLetto, AppezzaDecodificato, campoCodDecodificato, Id_reg_letto, id_reg_Decodificato, progettoCodLetto, progettoCodDecodificato)
                            'DRUDI 2017/10/04 Inserite codifiche agea
                            TO_Programmazione_Entita_Cod = 0
                            BoolDummy = objProgrammazione_Entita_W.Scrivi(
                                        lTO_Programmazione_Cod,
                                        TO_Programmazione_Entita_Cod,
                                        CType(DrowEntita("Entita_Des"), String),
                                        Piva_Destinazione,
                                        sa_cod_destinazione,
                                        campoCodDecodificato,
                                        AppezzaDecodificato,
                                        id_reg_Decodificato,
                                        progettoCodDecodificato,
                                        CType(DrowEntita("Progetto_Des"), String),
                                        CType(DrowEntita("Id_Cod"), Integer),
                                        CType(DrowEntita("Veg_Cod"), Integer),
                                        CType(DrowEntita("Cul_Cod"), Integer),
                                        CType(DrowEntita("Grva_Cod"), Integer),
                                        CType(DrowEntita("Grfi_Cod"), Integer),
                                        CType(DrowEntita("Cop_Cod"), Integer),
                                        CType(DrowEntita("Superficie"), Double),
                                        CType(DrowEntita("Resa"), Double),
                                        CType(DrowEntita("TipoZona"), String),
                                        CType(DrowEntita("Veg_Cod_Prec"), Integer),
                                        CType(DrowEntita("Id_Mat_O"), Integer),
                                        CType(DrowEntita("Id_Fre"), Integer),
                                        CType(DrowEntita("N_distribuito"), Integer),
                                        CType(DrowEntita("Num_Piante"), Integer),
                                        CType(DrowEntita("Tra_Fila"), Double),
                                        CType(DrowEntita("Su_Fila"), Double),
                                        CType(DrowEntita("Foral_Cod"), Integer),
                                        CType(DrowEntita("Port_Cod"), Integer),
                                        CType(DrowEntita("Imp_Cod"), Integer),
                                        CType(DrowEntita("Regolamento_Cod"), Integer),
                                        CType(DrowEntita("Disciplinare_Cod"), Integer),
                                        CType(DrowEntita("Stato_Cod"), Integer),
                                        CType(DrowEntita("Ciclo"), Integer),
                                        CType(DrowEntita("Data_Semina"), Date),
                                        CType(DrowEntita("Data_Raccolta"), Date),
                                        CType(DrowEntita("Note"), String),
                                        CType(DrowEntita("Veg_Cod_Cliente"), String),
                                        CType(DrowEntita("Cul_Cod_Cliente"), String),
                                        CType(DrowEntita("MetodoProduzione_Cod"), Integer),
                                        CType(DrowEntita("Validita_Inizio"), Date),
                                        CType(DrowEntita("Validita_Fine"), Date),
                                        objOpzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                        DrowEntita("Data_Creazione"),
                                        DrowEntita("Data_Modifica"),
                                        DrowEntita("Username_Creazione"),
                                        DrowEntita("Username_Modifica"),
                                        Veg_Cod_Agea:=DrowEntita("Veg_Cod_Agea"),
                                        Cul_Cod_Agea:=DrowEntita("Cul_Cod_Agea"),
                                        Uso_Cod_Agea:=DrowEntita("Uso_Cod_Agea"),
                                        Occupazione_Cod_Agea:=DrowEntita("Occupazione_Cod_Agea"),
                                        Destinazione_Cod_Agea:=DrowEntita("Destinazione_Cod_Agea"),
                                        Qualita_Cod_Agea:=DrowEntita("Qualita_Cod_Agea")
                                )

                            objProgrammazione_Entita_W.ModificaFabbisogno(
                                lTO_Programmazione_Cod,
                                TO_Programmazione_Entita_Cod,
                                DoubleDefault_per_MODIFICA,
                                CType(DrowEntita("N_fabbisogno"), Double),
                                "",
                                objOpzioni.objParametri_Server_GIAS_DESTINAZIONE
                            )


                            '------------------------------------------
                            'Inserisco le Particelle legate all'entità

                            '----------------------------------
                            'leggo le particelle associate alle varie entità
                            Dim objPP As New AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R
                            Dt_Particelle_R = objPP.Programmazione_Particelle_Leggi_2(objOpzioni.objParametri_Server_GIAS_ORIGINE,
                                                                                     From_Programmazione_Cod,
                                                                                     ErrMSG,
                                                                                     From_Programmazione_Entita_Cod
                                                                                     )




                            Dim objProgrammazione_Particelle_W As New AgronicaCoreAnagrafeDAL.Programmazione_Particelle_W


                            For Each row_Particelle_R In Dt_Particelle_R.Rows

                                BoolDummy = objProgrammazione_Particelle_W.Scrivi(
                                    TO_Programmazione_Entita_Cod,
                                    CType(row_Particelle_R("Prov"), String),
                                    CType(row_Particelle_R("Com"), String),
                                    CType(row_Particelle_R("Sezione"), String),
                                    CType(row_Particelle_R("Foglio"), Integer),
                                    CType(row_Particelle_R("Numero"), Integer),
                                    CType(row_Particelle_R("Subalterno"), String),
                                    CType(row_Particelle_R("Superficie"), Double),
                                    CType(row_Particelle_R("Validita_Inizio"), Date),
                                    CType(row_Particelle_R("Validita_Fine"), Date),
                                    objOpzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                    row_Particelle_R("Data_Creazione"),
                                    row_Particelle_R("Data_Modifica"),
                                    row_Particelle_R("Username_Creazione"),
                                    row_Particelle_R("Username_Modifica")
                                )

                            Next




                            If Not DBNullToNothing(DrowEntita("Area_Cod")) Is Nothing Then

                                'se ho già rimappato l'entità cod ne ho già rimappato anche l'area omogenea.. (caso in cui ci sono più sa_cod..)
                                If (
                                    From p In _funzioniGLOBAL.Planning
                                    Where p.From_Programmazione_Cod = From_Programmazione_Cod _
                                    And p.From_Programmazione_Entita_cod = From_Programmazione_Entita_Cod _
                                    And p.From_Piva = Piva_Origine _
                                    And p.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser _
                                    And p.From_sa_cod = Sa_cod_Origine
                                    ).ToList.Count = 0 Then

                                    Dim dtLeggiAreaXEntita As DataTable =
                                        leggiAreeOmogeneeXEntita.Leggi(
                                            "",
                                            0,
                                            DrowEntita("Area_Cod"),
                                            From_Programmazione_Entita_Cod,
                                            "",
                                            "",
                                            objOpzioni.objParametri_Server_GIAS_ORIGINE
                                        )

                                    If Not dtLeggiAreaXEntita Is Nothing AndAlso dtLeggiAreaXEntita.Rows.Count > 0 Then
                                        Dim areaCodRimappata As Integer?
                                        Dim lDrowArea As Integer = DrowEntita("Area_Cod")

                                        If lDrowArea = 0 Then
                                            areaCodRimappata = 0
                                        Else
                                            areaCodRimappata = (From a In _AreeOmogenee
                                                                Where a.From_Area_Omogenea_cod = lDrowArea _
And a.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser
                                                                Select a.To_Area_Omogenea_cod).FirstOrDefault

                                        End If


                                        If areaCodRimappata Is Nothing Then
                                            Throw New Exception("Area omogenea non decodificata correttamente!")
                                        End If

                                        scriviAreeOmogeneeXEntita.Scrivi(
                                            Piva_Destinazione,
                                            areaCodRimappata,
                                            TO_Programmazione_Entita_Cod,
                                            CType(dtLeggiAreaXEntita.Rows(0)("Validita_inizio"), Date),
                                            CType(dtLeggiAreaXEntita.Rows(0)("Validita_fine"), Date),
                                            objOpzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                            dtLeggiAreaXEntita.Rows(0)("Data_Creazione"),
                                            dtLeggiAreaXEntita.Rows(0)("Data_Modifica"),
                                            dtLeggiAreaXEntita.Rows(0)("Username_Creazione"),
                                            dtLeggiAreaXEntita.Rows(0)("Username_Modifica")
                                        )
                                    End If

                                End If


                            End If


                            _funzioniGLOBAL.Planning.Add(
                                New recode_planning With {
                                    .From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser,
                                    .From_Piva = Piva_Origine,
                                    .From_sa_cod = Sa_cod_Origine,
                                    .From_appezza = AppezzaLetto,
                                    .From_Id_reg = Id_reg_letto,
                                    .From_Programmazione_Cod = From_Programmazione_Cod,
                                    .From_Programmazione_Entita_cod = From_Programmazione_Entita_Cod,
                                    .To_PivaSuperUSer = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser,
                                    .To_Piva = Piva_Destinazione,
                                    .To_sa_cod = sa_cod_destinazione,
                                    .To_appezza = AppezzaDecodificato,
                                    .To_Id_reg = id_reg_Decodificato,
                                    .To_Programmazione_Cod = lTO_Programmazione_Cod,
                                    .To_Programmazione_Entita_cod = TO_Programmazione_Entita_Cod
                                })

                        Next


                    End If

                    _funzioniGLOBAL.Planning.Add(
                        New recode_planning With {
                            .From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser,
                            .From_Piva = Piva_Origine,
                            .From_sa_cod = Sa_cod_Origine,
                            .From_appezza = -1,
                            .From_Id_reg = -1,
                            .From_Programmazione_Cod = From_Programmazione_Cod,
                            .From_Programmazione_Entita_cod = -1,
                            .To_PivaSuperUSer = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser,
                            .To_Piva = Piva_Destinazione,
                            .To_sa_cod = sa_cod_destinazione,
                            .To_appezza = -1,
                            .To_Id_reg = -1,
                            .To_Programmazione_Cod = lTO_Programmazione_Cod,
                            .To_Programmazione_Entita_cod = -1
                        })

                Next
            End If


            Dim Dt_Entita_Eliminate_R As New DataTable

            'leggo tutte le entità eliminate (Programmazione_Entita_cod impostato )
            Dim LeggiProgrammazione_Entita_Eliminate_R As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_Eliminate_R

            Dt_Entita_Eliminate_R = LeggiProgrammazione_Entita_Eliminate_R.Leggi(
                                        0,
                                        ErrMSG,
                                        0,
                                        Estremo_Validita_Inizio,
                                        Estremo_Validita_Fine,
                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                        " PIVA = '" & Piva_Origine & "' AND Sa_cod = " & Sa_cod_Origine,
                                        "",
                                        objOpzioni.objParametri_Server_GIAS_ORIGINE
                                    )

            If Not Dt_Entita_Eliminate_R Is Nothing AndAlso Dt_Entita_Eliminate_R.Rows.Count > 0 Then


                Dim objProgrammazione_Entita_Eliminate_W As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_Eliminate_W
                Dim ObjSequenze As New AgronicaCoreDataProvider.Agro_Sequenze

                For Each rowEntita_Eliminate_R In Dt_Entita_Eliminate_R.Rows

                    LeggiDecodifiche(Piva_Origine, Sa_cod_Origine, nomeFunzione, rowEntita_Eliminate_R, AppezzaLetto, AppezzaDecodificato, campoCodDecodificato, Id_reg_letto, id_reg_Decodificato, progettoCodLetto, progettoCodDecodificato)

                    Dim From_Programmazione_Cod As Integer
                    Dim From_Programmazione_Entita_cod As Integer
                    Dim lTO_programmazione_Cod As Integer?
                    Dim lTO_Programmazione_Entita_Cod As Integer?
                    DecodificaEntitaEliminate(objOpzioni, Piva_Origine, ObjSequenze, rowEntita_Eliminate_R, From_Programmazione_Cod, From_Programmazione_Entita_cod, lTO_programmazione_Cod, lTO_Programmazione_Entita_Cod)

                    If Not (AppezzaDecodificato = 0 And campoCodDecodificato = 0 And id_reg_Decodificato = 0) Then

                        BoolDummy = objProgrammazione_Entita_Eliminate_W.Scrivi(
                                        Piva_Destinazione,
                                        sa_cod_destinazione,
                                        AppezzaDecodificato,
                                        id_reg_Decodificato,
                                        campoCodDecodificato,
                                        lTO_programmazione_Cod,
                                        lTO_Programmazione_Entita_Cod,
                                        progettoCodDecodificato,
                                        0, CType(rowEntita_Eliminate_R("Validita_Inizio"), Date),
                                        CType(rowEntita_Eliminate_R("Validita_Fine"), Date),
                                        objOpzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                        rowEntita_Eliminate_R("Data_Creazione"),
                                        rowEntita_Eliminate_R("Data_Modifica"),
                                        rowEntita_Eliminate_R("Username_Creazione"),
                                        rowEntita_Eliminate_R("Username_Modifica")
                                    )
                    End If

                    _funzioniGLOBAL.Planning.Add(
                                New recode_planning With {
                                    .From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser,
                                    .From_Piva = Piva_Origine,
                                    .From_sa_cod = Sa_cod_Origine,
                                    .From_appezza = AppezzaLetto,
                                    .From_Id_reg = Id_reg_letto,
                                    .From_Programmazione_Cod = From_Programmazione_Cod,
                                    .From_Programmazione_Entita_cod = From_Programmazione_Entita_cod,
                                    .To_PivaSuperUSer = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser,
                                    .To_Piva = Piva_Destinazione,
                                    .To_sa_cod = sa_cod_destinazione,
                                    .To_appezza = AppezzaDecodificato,
                                    .To_Id_reg = id_reg_Decodificato,
                                    .To_Programmazione_Cod = lTO_programmazione_Cod,
                                    .To_Programmazione_Entita_cod = lTO_Programmazione_Entita_Cod
                                })
                Next

            End If


        Catch ex As Exception

            Dim msg As String
            msg = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)

        End Try

    End Sub



    Private Sub DecodificaEntitaEliminate(ByVal objOpzioni As clsOpzioni, ByVal Piva_Origine As String, ByVal ObjSequenze As AgronicaCoreDataProvider.Agro_Sequenze, ByVal rowEntita_Eliminate_R As Object, ByRef From_Programmazione_Cod As Integer, ByRef From_Programmazione_Entita_Cod As Integer, ByRef lTO_programmazione_Cod As Integer?, ByRef lTO_Programmazione_Entita_Cod As Integer?)



        From_Programmazione_Cod = rowEntita_Eliminate_R("Programmazione_COD")
        From_Programmazione_Entita_Cod = rowEntita_Eliminate_R("Programmazione_entita_COD")

        Dim lFrom_Programmazione_Cod As Integer = From_Programmazione_Cod
        Dim lFrom_Programmazione_Entita_Cod As Integer = From_Programmazione_Entita_Cod

        lTO_programmazione_Cod = (
            From p In _funzioniGLOBAL.Planning
            Where p.From_Programmazione_Cod = lFrom_Programmazione_Cod _
            And p.From_Programmazione_Entita_cod = -1 _
            And p.From_Piva = Piva_Origine _
            And p.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser
            Select p.To_Programmazione_Cod).FirstOrDefault


        If lTO_programmazione_Cod = 0 Then
            'lTO_programmazione_Cod = ObjSequenze.Agronica_SequenzaTabelle_NuovoID(
            '                       "Programmazione_Testata",
            '                        objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)
            'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
            lTO_programmazione_Cod = ObjSequenze.NuovoId_Tabella("Programmazione_Testata", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)
        End If


        If lFrom_Programmazione_Entita_Cod <> 0 Then
            lTO_Programmazione_Entita_Cod = (
                From p In _funzioniGLOBAL.Planning
                Where p.From_Programmazione_Cod = lFrom_Programmazione_Cod _
                And p.To_Programmazione_Entita_cod = lFrom_Programmazione_Entita_Cod _
                And p.From_Piva = Piva_Origine _
                And p.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser
                Select p.To_Programmazione_Cod).FirstOrDefault

            If lTO_Programmazione_Entita_Cod = 0 Then
                'lTO_Programmazione_Entita_Cod = ObjSequenze.Agronica_SequenzaTabelle_NuovoID(
                '                       "Programmazione_Entita",
                '                        objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)
                'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                lTO_Programmazione_Entita_Cod = ObjSequenze.NuovoId_Tabella("Programmazione_Entita", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)
            End If
        Else
            lTO_Programmazione_Entita_Cod = 0
        End If


    End Sub



    Private Sub LeggiDecodifiche(ByVal Piva_Origine As String, ByVal Sa_cod_Origine As Integer, ByVal nomeFunzione As String, ByVal Drow As DataRow, ByRef AppezzaLetto As Integer?, ByRef AppezzaDecodificato As Integer?, ByRef campoCodDecodificato As Integer?, ByRef Id_reg_letto As Integer?, ByRef id_reg_Decodificato As Integer?, ByRef progettoCodLetto As Integer?, ByRef progettoCodDecodificato As Integer?)

        AppezzaLetto = Nothing
        AppezzaDecodificato = Nothing
        campoCodDecodificato = Nothing
        Id_reg_letto = Nothing
        id_reg_Decodificato = Nothing
        progettoCodLetto = Nothing
        progettoCodDecodificato = Nothing

        AppezzaLetto = Drow("Appezza")
        If AppezzaLetto Is Nothing Then
            AppezzaLetto = 0
        End If


        If AppezzaLetto > 0 Then
            Dim lAppezzaLetto As Integer = AppezzaLetto
            If NuovaLogicaRecodePC Then
                AppezzaDecodificato = (
                    From a In _efG2G.G2G_Recode_Appezzamenti
                    Where a.From_Piva = Piva_Origine _
                    And a.From_Sa_Cod = Sa_cod_Origine _
                    And a.From_Appezza = lAppezzaLetto
                    Select a.To_Appezza
                ).FirstOrDefault
            Else
                AppezzaDecodificato = (
                    From a In _AppezzamentiMappati
                    Where a.From_Piva = Piva_Origine _
                    And a.From_Sa_Cod = Sa_cod_Origine _
                    And a.From_Appezza = lAppezzaLetto
                    Select a.To_Appezza
                ).FirstOrDefault
            End If

        Else
            AppezzaDecodificato = AppezzaLetto
        End If


        Dim campoCodLetto As Integer? = Drow("Campo_Cod")
        If campoCodLetto Is Nothing Then
            campoCodLetto = 0
        End If


        If campoCodLetto > 0 Then
            campoCodDecodificato = (
                From c In _Campi
                Where c.From_Piva = Piva_Origine _
                And c.From_Sa_Cod = Sa_cod_Origine _
                And c.From_Campo_cod = campoCodLetto
                Select c.To_Campo_cod).FirstOrDefault
        Else
            campoCodDecodificato = campoCodLetto
        End If



        If campoCodDecodificato Is Nothing AndAlso AppezzaDecodificato Is Nothing Then
            Throw New Exception(nomeFunzione & ": deve esistere un campo oppure un appezzaemento.")
        End If


        If Drow.Table.Columns.Contains("ID_Reg") Then
            Id_reg_letto = Drow("Id_Reg")
        End If


        If Id_reg_letto Is Nothing Then
            Id_reg_letto = 0
        End If

        If Id_reg_letto > 0 Then
            Dim lAppezzaLetto As Integer = AppezzaLetto
            If NuovaLogicaRecodePC Then
                id_reg_Decodificato = (
                    From r In _efG2G.G2G_Recode_Impianti
                    Where r.From_Piva = Piva_Origine _
                    And r.From_Sa_Cod = Sa_cod_Origine _
                    And r.From_Appezza = lAppezzaLetto
                    Select r.To_Id_Reg).FirstOrDefault
            Else
                id_reg_Decodificato = (
                    From r In _ImpiantiMappati
                    Where r.From_Piva = Piva_Origine _
                    And r.From_Sa_Cod = Sa_cod_Origine _
                    And r.From_Appezza = lAppezzaLetto
                    Select r.To_Id_Reg).FirstOrDefault
            End If


        Else
            id_reg_Decodificato = Id_reg_letto
        End If

        If id_reg_Decodificato Is Nothing Then
            id_reg_Decodificato = 0
        End If

        If Drow.Table.Columns.Contains("Progetto_Cod") Then
            progettoCodLetto = Drow("Progetto_Cod")
        End If


        If progettoCodLetto Is Nothing Then
            progettoCodLetto = 0
        End If

        If progettoCodLetto > 0 Then
            Dim lProgettoCodLetto As Integer = progettoCodLetto
            If NuovaLogicaRecodePC Then
                progettoCodDecodificato = (
                    From pr In _efG2G.G2G_Recode_Distinta
                    Where pr.From_Piva = Piva_Origine _
                    And pr.From_Progetto_cod = lProgettoCodLetto
                    Select pr.To_Progetto_cod
                ).FirstOrDefault
            Else
                progettoCodDecodificato = (
                    From pr In _Distinta
                    Where pr.From_Piva = Piva_Origine _
                    And pr.From_Progetto_cod = lProgettoCodLetto
                    Select pr.To_Progetto_cod
                ).FirstOrDefault
            End If

        Else
            progettoCodDecodificato = progettoCodLetto
        End If

        If progettoCodDecodificato Is Nothing Then
            progettoCodDecodificato = 0
        End If


    End Sub


#End Region

End Class
