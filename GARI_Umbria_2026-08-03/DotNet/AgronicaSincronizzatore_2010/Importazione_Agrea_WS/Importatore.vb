Imports System.IO
Imports System.Text
Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ

Public Class Importatore

    Public Function CreaAzienda(ByVal ASG_ProgressivoGIAS As Integer,
                                       ByVal ASG_Utente_Password As String,
                                       ByRef Piva As String,
                                       ByVal Cuaa As String,
                                       ByVal fascicolo As String,
                                       ByRef strErr As String,
                                       ByRef strRis As String,
                                       ByRef RagSoc As String,
                                       ByRef Indirizzo As String,
                                       ByRef Cap As String,
                                       ByRef Istat_Provincia As String,
                                       ByRef Istat_Comune As String,
                                       ByRef OrigineOpr As String,
                                       ByRef objParametri_Server As AgronicaCoreParametri,
                                       ByRef objParametri_Utenti As AgronicaCoreParametri,
                                       ByRef AziendaVisibile As Boolean,
                                       ByRef _Piva_Padre As String
    )

        Dim obj_fascicolo As New Importazione_Agrea_WS.pc.common.webservice.sop.agrea.it.ISWSResponse

        If fascicolo <> "" Then
            XMLUtility.getObjectFromResponse(fascicolo, obj_fascicolo, Nothing, False)
        End If

        Dim TipoOperazione = enum_TipoOperazioneDB.Modifica

        Dim objImp As New AgronicaCoreAnagrafeDAL.Imprese_Read

        Dim Esiste_Impresa = objImp.VerificaEsistenza_PivaGIAS(obj_fascicolo.azienda.partitaIva,
                                                                       objParametri_Server)
        If Not Esiste_Impresa Then
            TipoOperazione = enum_TipoOperazioneDB.Scrittura
        End If

        Dim Piva_Padre = objParametri_Server.PivaSuperUser

        If _Piva_Padre <> "" Then
            Piva_Padre = _Piva_Padre
        End If



        Dim rval As Boolean =
            CreaAziendaGias(ASG_ProgressivoGIAS, ASG_Utente_Password,
                        Piva_Padre, Piva, Cuaa,
                        strErr, strRis,
                        objParametri_Server, objParametri_Utenti, fascicolo)

        Return rval

    End Function

    Private Function Utente_Visibilita_Impresa(Piva As String, objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri) As Boolean

        Dim classFiltrone As New AgronicaCoreUtility.Filtrone
        Dim ClassJoin As New AgronicaCoreUtility.JoinFiltrone

        ClassJoin.bGerarchiaImprese = True

        'End If

        classFiltrone.ImpostaVariabiliJOIN_xFiltroUtente("", ClassJoin)

        Dim Dt_Imprese = classFiltrone.CreaDTFiltrone(objParametri_Server,
                                                    " Imprese.PIVA = '" & Piva & "' ",
                                                    enum_TipoSelect_FiltroneSuperNova.Imprese,
                                                    "",
                                                    ClassJoin)

        If Dt_Imprese IsNot Nothing AndAlso Dt_Imprese.Rows.Count > 0 Then

            Return True

        Else

            Return False

        End If

    End Function

    Public Shared Function CreaAziendaGias(ByVal ASG_ProgressivoGIAS As String,
                                    ByVal ASG_Utente_Password As String,
                                    ByVal Piva_Padre As String,
                                    ByVal Piva As String,
                                    ByVal Cuaa As String,
                                    ByRef strErr As String,
                                    ByRef strRis As String,
                                    ByRef objParametri_Server As AgronicaCoreParametri,
                                    ByRef objParametri_Utenti As AgronicaCoreParametri,
                                    Optional fascicolo As String = Nothing
                                    ) As Boolean


        Dim obj_fascicolo As New Importazione_Agrea_WS.pc.common.webservice.sop.agrea.it.ISWSResponse

        If fascicolo <> "" Then
            XMLUtility.getObjectFromResponse(fascicolo, obj_fascicolo, Nothing, False)
        End If

        Dim cls_agrea As New cls_ImportAgrea

        Dim TipoOperazione = enum_TipoOperazioneDB.Modifica

        Dim objImp As New AgronicaCoreAnagrafeDAL.Imprese_Read

        Dim Esiste_Impresa = objImp.VerificaEsistenza_PivaGIAS(obj_fascicolo.azienda.partitaIva,
                                                                       objParametri_Server)
        If Not Esiste_Impresa Then
            TipoOperazione = enum_TipoOperazioneDB.Scrittura
        End If

        'TipoOperazione = enum_TipoOperazioneDB.Scrittura

        Dim strErrBuilder As New StringBuilder
        Dim strRisBuilder As New StringBuilder

        Dim strXmlAnagrafe = cls_agrea.Xml_Genera_Stringone_Anagrafe_WS_new(obj_fascicolo,
                                                                            TipoOperazione,
                                                                            objParametri_Server,
                                                                            objParametri_Server.UtenteUsername,
                                                                            ASG_Utente_Password,
                                                                            ASG_ProgressivoGIAS,
                                                                            enum_TipoImportazioneAnagrafe.Agrea_Excel,
                                                                            strErrBuilder,
                                                                            strRisBuilder, Piva_Padre, "")


        Dim Str_RisultatoMassivaImport = ""
        Dim Str_RisultatoMassivaErrori = ""
        Dim Str_RisultatoCUAA = ""
        Dim Azienda_InElaborazione As String = ""
        Dim Num_Azi_Sincronizzate = 0

        cls_agrea.Scrivi_Dati(strErrBuilder,
                                        strRisBuilder,
                                        strXmlAnagrafe,
                                         "",
                                         Str_RisultatoMassivaImport,
                                        Str_RisultatoMassivaErrori,
                                        Str_RisultatoCUAA,
                                        Num_Azi_Sincronizzate,
                                        objParametri_Server.UtenteUsername,
                                        ASG_Utente_Password,
                                        objParametri_Server.PivaSuperUser,
                                        Azienda_InElaborazione,
                                        objParametri_Server,
                                        objParametri_Utenti)

        strErr = strErrBuilder.ToString
        strRis = strRisBuilder.ToString

        Dim aziendaCreata As Boolean = False
        If Num_Azi_Sincronizzate > 0 Then
            aziendaCreata = True
        End If

        Return aziendaCreata

    End Function

    Public Shared Sub Importa_Dati(ByVal ASG_Utente_Password As String,
                            ByVal strDatiAnagrafe As String,
                            ByVal strDatiPianificazione As String,
                            ByRef strErr As String,
                            ByRef strRis As String,
                            ByRef AziendaCreata As Boolean,
                            ByRef PianoCreato As Boolean,
                            ByRef objParametri_Server As AgronicaCoreParametri,
                            ByRef objParametri_Utenti As AgronicaCoreParametri,
                            Optional ByVal TipoOperazione As Integer = 1)

        Dim Documento_Finale As New System.Xml.XmlDocument
        Dim XML_Risultato As System.Xml.XmlElement
        Dim XML_Risposta As System.Xml.XmlElement
        Dim XMLs_Risposta As System.Xml.XmlNodeList

        Dim StrFinaleAnagrafe As String
        Dim StrFinalePianificazione As String
        Dim Descrizione As String

        Dim CodiceChiaveCliente As Integer

        Dim x As Integer
        Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim Importa As New ws_Importa_Gias.ImportaWS
        CodiceChiaveCliente = objConfSiti.Leggi_Valore(16, "Sincro_Codice_Chiave_Cliente", "", "", objParametri_Server)


        Try

            Dim LinkWSImportaGIAS As String
            LinkWSImportaGIAS = objConfSiti.Leggi_Valore(16, "Sincro_LinkWSImportaGIAS", "", "", objParametri_Server)

            If LinkWSImportaGIAS <> "" Then

                Importa.Url = LinkWSImportaGIAS

                Importa.Timeout = System.Threading.Timeout.Infinite
                Descrizione = String.Empty

                Dim objcoreXML As New AgronicaCoreXML.XML_WS_Importa_Gias
                Dim strCredenziali As String
                strCredenziali = objcoreXML.Genera_Stringa_Credenziali(True, Nothing, objParametri_Server.UtenteUsername,
                                                                       ASG_Utente_Password,
                                                                       objParametri_Server.PivaSuperUser,
                                                                       True,
                                                                       "", "", "", "", "", "",
                                                                       objParametri_Server.StringaConnessione,
                                                                       objParametri_Utenti.StringaConnessione)

                If strDatiAnagrafe <> String.Empty Then

                    Descrizione &= "<b>Importazione anagrafe:" & "</b></br>"

                    StrFinaleAnagrafe = Importa.Importa_DocumentoPubblico_SuperServer(strCredenziali,
                                                                                      strDatiAnagrafe,
                                                                                      CodiceChiaveCliente)


                    Documento_Finale.LoadXml(StrFinaleAnagrafe)

                    XML_Risultato = Documento_Finale.SelectSingleNode("Risultato")

                    If XML_Risultato.HasAttribute("errore") Then

                        Descrizione &= "- " & XML_Risultato.GetAttribute("errore").ToString & "</br>"
                        strErr = Descrizione
                        AziendaCreata = False

                    Else

                        XMLs_Risposta = XML_Risultato.GetElementsByTagName("Risposta")

                        Dim strRisp As String = String.Empty

                        For x = 0 To XMLs_Risposta.Count - 1

                            XML_Risposta = XMLs_Risposta.Item(x)

                            strRisp = XML_Risposta.GetAttribute("Ris")

                            Descrizione &= "- " & strRisp & IIf(InStr(strRisp, "Errore"), "", " - Terminata correttamente") & "</br>"

                            AziendaCreata = True

                        Next

                    End If


                End If

                If strDatiPianificazione <> String.Empty Then

                    Descrizione &= "</br><b>Importazione pianificazione:" & "</b></br>"

                    StrFinalePianificazione = Importa.Importa_Pianificazione_SuperServer(strCredenziali,
                                                                                         strDatiPianificazione,
                                                                                         CodiceChiaveCliente)

                    Documento_Finale.LoadXml(StrFinalePianificazione)

                    XML_Risultato = Documento_Finale.SelectSingleNode("Risultato")

                    XMLs_Risposta = XML_Risultato.GetElementsByTagName("Risposta")

                    Dim strRisp As String = String.Empty

                    For x = 0 To XMLs_Risposta.Count - 1

                        XML_Risposta = XMLs_Risposta.Item(x)

                        strRisp = XML_Risposta.GetAttribute("Ris")

                        Descrizione &= "- " & strRisp & IIf(InStr(strRisp, "Errore"), "", " - Terminata correttamente") & "</br>"

                        PianoCreato = True

                    Next

                End If

                strRis = Descrizione

            Else

                strErr = "Inserire l'indirizzo del Web Service Gias per continuare."


            End If

        Catch ex As Exception

            strErr = ex.Message

            AziendaCreata = False

        End Try


    End Sub

    Public Shared Sub dtParticelleCreaStruttura(ByRef DtParticelle As DataTable)

        DtParticelle.Columns.Add(New DataColumn("chiave", GetType(Integer)))

        DtParticelle.Columns.Add(New DataColumn("piva", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("sa_cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("appezza", GetType(Integer)))

        DtParticelle.Columns.Add(New DataColumn("programmazione_entita_cod", GetType(Integer)))

        DtParticelle.Columns.Add(New DataColumn("Sa_Nome", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("App_Nome", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("PROV", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("COM", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Prov_Des", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Com_Des", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Sezione", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Foglio", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Numero", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Subalterno", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("possesso", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("TitoloPossesso", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("datepossesso", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("inizio_possesso", GetType(Date)))
        DtParticelle.Columns.Add(New DataColumn("fine_possesso", GetType(Date)))
        DtParticelle.Columns.Add(New DataColumn("sup", GetType(Double)))
        DtParticelle.Columns.Add(New DataColumn("supcondotta", GetType(Double)))

        DtParticelle.Columns.Add(New DataColumn("catasto", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("catasto_key", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("macrouso", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("macrouso_cod", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("macrouso_sup", GetType(Double)))

        DtParticelle.Columns.Add(New DataColumn("utilizzo", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("varieta", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("utilizzo_sup", GetType(Double)))
        'DtParticelle.Columns.Add(New DataColumn("Veg_Cod_Agea", GetType(String)))
        'DtParticelle.Columns.Add(New DataColumn("Cul_Cod_Agea", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Veg_Cod", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Cul_Cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Grfi_Cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Grva_Cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Id_Cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Scarto", GetType(Integer)))

        DtParticelle.Columns.Add(New DataColumn("veg_des", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("cul_des", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("grfi_des", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("grva_des", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("validita_inizio", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("validita_fine", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("ribaltato", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("movimentato", GetType(Integer)))

        'DtParticelle.Columns.Add(New DataColumn("Uso_Cod_Agea", GetType(String)))
        'DtParticelle.Columns.Add(New DataColumn("Occupazione_Cod_Agea", GetType(String)))
        'DtParticelle.Columns.Add(New DataColumn("Destinazione_Cod_Agea", GetType(String)))
        'DtParticelle.Columns.Add(New DataColumn("Qualita_Cod_Agea", GetType(String)))

        'DtParticelle.Columns.Add(New DataColumn("Grva_Cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Cop_Cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Cop_Des", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Lotto", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Resa", GetType(Double)))
        DtParticelle.Columns.Add(New DataColumn("Num_Piante", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("TRA_Fila", GetType(Double)))
        DtParticelle.Columns.Add(New DataColumn("SU_Fila", GetType(Double)))
        DtParticelle.Columns.Add(New DataColumn("Validita_Inizio_Impianto", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("TipoZona", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("TipoZona_Des", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("MetodoProduzione_Cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("MetodoProduzione_Des", GetType(String)))
        'DtParticelle.Columns.Add(New DataColumn("Macrouso_Cod", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Unita_Vitata", GetType(Integer)))

        DtParticelle.Columns.Add(New DataColumn("Veg_Cod_Agea", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Veg_Des_Agea", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Cul_Cod_Agea", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Cul_Des_Agea", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Uso_Cod_Agea", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Uso_Des_Agea", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Occupazione_Cod_Agea", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Occupazione_Des_Agea", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Destinazione_Cod_Agea", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Destinazione_Des_Agea", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Qualita_Cod_Agea", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Qualita_Des_Agea", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Gru_Cod", GetType(Integer)))

        DtParticelle.Columns.Add(New DataColumn("Dpi_Cod", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Reg_Cod", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("StatoImpianto_Cod", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("N", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("P", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("K", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Data_semina", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Data_Raccolta", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Data_Fioritura", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Coltura_Precedente", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Coltura_Precedente2", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Coltura_Precedente3", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Coltura_Precedente4", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Piano_Semina", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Codice_Contratto", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("unito", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("frazionato", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Codice_Fiscale_Tecnico", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("provenienza_fascicolo", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("stato_ribaltamento", GetType(Integer)))

        DtParticelle.Columns.Add(New DataColumn("datoGis", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("IAF", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Disciplinare", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Regolamento_Concimazione_Cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Flag_PubblicoPrivato", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Id_tr", GetType(Integer)))

        DtParticelle.Columns.Add(New DataColumn("DistBZ_CorpiIdrici", GetType(Double)))
        DtParticelle.Columns.Add(New DataColumn("DistBZ_AreeResPub", GetType(Double)))
        DtParticelle.Columns.Add(New DataColumn("DistBZ_Allevamenti", GetType(Double)))
        DtParticelle.Columns.Add(New DataColumn("DistBZ_VegNatNonColt", GetType(Double)))

        DtParticelle.Columns.Add(New DataColumn("ZoneCatasto", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Campo_Cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Campo_Des", GetType(String)))


    End Sub

    Private Sub Popola_DtAppezzamenti_daFascicolo_3(ByVal Piva As String,
                                                           ByRef myISWSResponse As Importazione_Agrea_WS.pc.common.webservice.sop.agrea.it.ISWSResponse,
                                                           ByVal iAggregaSpecie As Integer,
                                                           ByVal bAggregaTare As Boolean,
                                                           ByRef DataInizio As Date,
                                                           ByRef DataFine As Date,
                                                           ByRef DtParticelle As DataTable,
                                                           ByRef objParametri_Server As AgronicaCoreParametri,
                                                           ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                           ByRef Allegati_Documenti_Numero As String,
                                                           ByRef ImportCatasto As Boolean,
                                                           ByRef ASG_Utente_Username As String,
                                                           ByRef ASG_Utente_Password As String,
                                                           ByRef ASG_ProgressivoGIAS As String,
                                                           ByVal CatastoDaImportare As Boolean)


        Dim Cuaa As String = ""

        Dim objCodificaZona As New AgronicaCoreAnagrafeDAL.CAC_Codifica_Zone
        Dim objCultivar As New AgronicaCoreMetaSchemaDAL.Cultivar_R
        Dim objImpresexParticelle As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R
        Dim objXML As New AgronicaCoreXML.AnagrafeXML

        Dim cls_agrea As New cls_ImportAgrea

        Dim SchedaValidazione As String = ""
        Dim DataValidazione As Date = AGRODATAINIZIO
        'Dim DataAperturaFascicolo As Date = AGRODATAINIZIO
        'Dim DataChiusuraFascicolo As Date = AGRODATAFINE
        'Dim DataInizioMandato As Date = AGRODATAINIZIO
        'Dim DataFineMandato As Date = AGRODATAFINE

        Dim CodiceChiaveCliente As Integer = 0

        Dim objconfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        If objconfigSiti.Leggi_Valore(0, "Codice_Chiave_Cliente", "", "", objParametri_Server) <> "" Then
            CodiceChiaveCliente = objconfigSiti.Leggi_Valore(0, "Codice_Chiave_Cliente", "", "", objParametri_Server)
        End If

        Dim ANNO As Integer = myISWSResponse.domanda.annoRiferimento

        Dim strAnno As String = String.Empty
        Dim strValiditaInizio As String = String.Empty
        Dim strValiditaFine As String = String.Empty

        Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim DataInizioUtente As Date = AGRODATAINIZIO
        Dim DataFineUtente As Date = AGRODATAFINE
        objImpost.AnnataAgraria(CDate("01/01/" & ANNO.ToString), DataInizioUtente, DataFineUtente, objParametri_Utenti)

        If IsNumeric(ANNO) AndAlso (CInt(ANNO) > 1900 AndAlso CInt(ANNO) < 2100) Then
            strAnno = CInt(ANNO).ToString
            strValiditaInizio = DataInizioUtente.ToShortDateString
            strValiditaFine = DataFineUtente.ToShortDateString
        Else
            strValiditaInizio = DataInizioUtente.ToShortDateString
            strValiditaFine = DataFineUtente.ToShortDateString
        End If

        Piva = myISWSResponse.azienda.partitaIva
        Cuaa = myISWSResponse.azienda.cuaa

        '06/05/2020: gestione piva estera, campo lungo ora 25
        If Piva = "" AndAlso Cuaa <> "" Then
            'se non ho la partita iva metto il CUAA tagliato a 11 cifre
            'Piva = Cuaa.Substring(0, 11)
            Piva = Cuaa
        End If

        Dim DtParticelleDB As New DataTable
        DtParticelleDB = objImpresexParticelle.Leggi(0,
                                                 Piva,
                                                 0, 0, "", "", "", 0, 0, "",
                                                 enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                 "", "", objParametri_Server)

        If myISWSResponse.domanda.idDomanda <> 0 Then
            SchedaValidazione = myISWSResponse.domanda.idDomanda
        End If
        If myISWSResponse.domanda.dataValidazione IsNot Nothing Then
            DataValidazione = CDate(myISWSResponse.domanda.dataValidazione)
        End If
        'If Not ws_fasciResponse_New.out.fascicolo.fascicolo.DataSottMandato Is Nothing Then
        '    DataInizioMandato = CDate(AgronicaCoreDataProvider.Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse_New.out.fascicolo.fascicolo.DataSottMandato))
        'End If
        'If Not ws_fasciResponse_New.out.fascicolo.fascicolo.DataAperturaFascicolo Is Nothing Then
        '    DataAperturaFascicolo = CDate(AgronicaCoreDataProvider.Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse_New.out.fascicolo.fascicolo.DataAperturaFascicolo))
        'End If
        'If Not ws_fasciResponse_New.out.fascicolo.fascicolo.DataChiusuraFascicolo Is Nothing Then
        '    DataChiusuraFascicolo = CDate(AgronicaCoreDataProvider.Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse_New.out.fascicolo.fascicolo.DataChiusuraFascicolo))
        'End If

        Dim Detentore_Fascicolo As String = ""
        If myISWSResponse.domanda.descCaa IsNot Nothing Then
            Detentore_Fascicolo = myISWSResponse.domanda.descCaa
        End If

        Dim SaCod As Integer = 0
        Dim SaNome As String = ""

        SaCod = New AgronicaCoreAnagrafeDAL.Centri_Codici_Read().RecuperaSaCodImpresaByIdAziendaFascicolo(
                                    CodiceChiaveCliente,
                                    Piva,
                                    myISWSResponse.azienda.idAzienda,
                                    SaNome,
                                    objParametri_Server)


        Dim i, j, k As Integer

        Dim strSezione As String = String.Empty
        Dim strSubalterno As String = String.Empty

        Dim Veg_Cod As Integer
        Dim Veg_Cod_CodificaCultivar As Integer
        Dim Cul_Cod As Integer
        Dim Grfi_Cod As Integer
        Dim Id_Cod As Integer
        Dim Grva_Cod As Integer
        Dim Cul_Cod_Cliente As String
        Dim Veg_Cod_Cliente As String
        Dim Macrouso_Cod As String
        Dim Macrouso_Sup As Double

        Dim Utilizzo_Sup As Double

        Dim Veg_Des As String
        Dim Cul_Des As String
        Dim Grfi_Des As String
        Dim Grva_Des As String
        Dim Sa_Nome As String

        Dim Veg_Cod_Agea As String
        Dim Cul_Cod_Agea As String
        Dim Uso_Cod_Agea As String
        Dim Occupazione_Cod_Agea As String
        Dim Destinazione_Cod_Agea As String
        Dim Qualita_Cod_Agea As String

        Dim N_App As Integer = 1
        Dim appezza As Integer = -1
        Dim chiave As Integer = 1

        Dim DR As DataRow

        Dim Prov As String = ""
        Dim Com As String = ""
        Dim Sezione As String = ""
        Dim Foglio As String = ""
        Dim Numero As String = ""
        Dim Subalterno As String = ""
        Dim Ettari As Double
        Dim Are As Double
        Dim Centiare As Double
        Dim supCatasto As Double = 0
        Dim supConduzione As Double = 0
        Dim strMacrousi As String = ""
        Dim strUtilizzi As String = ""
        Dim strVarieta As String = ""
        Dim TitoloPossesso As String
        Dim TitoloPossessoDes As String
        Dim Inizio_Possesso As Date
        Dim Fine_Possesso As Date
        Dim Prov_Des As String = ""
        Dim Com_Des As String = ""

        Dim Veg_Des_Agea As String = ""
        Dim Cul_Des_Agea As String = ""
        Dim Uso_Des_Agea As String = ""
        Dim Occupazione_Des_Agea As String = ""
        Dim Destinazione_Des_Agea As String = ""
        Dim Qualita_Des_Agea As String = ""
        Dim Gru_Cod As Integer = 0
        Dim Cop_Cod As Integer
        Dim Cop_Des As String
        Dim Validita_Inizio_Impianto As String
        Dim TipoZona As String
        Dim TipoZona_Des As String
        Dim MetodoProduzione_Cod As Integer
        Dim MetodoProduzione_Des As String = ""

        Dim Dpi_Cod As String = ""
        Dim Reg_Cod As String = "1"
        Dim StatoImpianto_Cod As String = ""
        Dim N As Double = 0
        Dim P As Double = 0
        Dim K1 As Double = 0
        Dim Data_Semina As Date = AGRODATAINIZIO
        Dim Data_Raccolta As Date = AGRODATAFINE
        Dim Data_Fioritura As Date = AGRODATAINIZIO
        Dim Coltura_Precedente As Integer = 0
        Dim Coltura_Precedente2 As Integer = 0
        Dim Coltura_Precedente3 As Integer = 0
        Dim Coltura_Precedente4 As Integer = 0
        Dim Piano_Semina As String = ""
        Dim Codice_Contratto As String = ""
        Dim unito As Integer = 0
        Dim frazionato As Integer = 0
        Dim objUtentiDAL As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R
        Dim Codice_Fiscale_Tecnico = objUtentiDAL.Leggi_IdentificativoGruppoUtenti_Singolo(objParametri_Utenti.UtenteUsername, objParametri_Utenti)
        Dim stato_ribaltamento As String = "0"
        Dim provenienza_fascicolo As String = Allegati_Documenti_Numero

        Dim objUtilizzi As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_2015_2020_R
        Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

        Dim objSpec = New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
        Dim objCodAna = New AgronicaCoreMetaSchemaDAL.Codici_Anagrafe_R
        Dim objCulti As New AgronicaCoreMetaSchemaDAL.Cultivar_R
        Dim objGrfi As New AgronicaCoreMetaSchemaDAL.GruppoFinalita_R
        Dim objGrva As New AgronicaCoreMetaSchemaDAL.GruppoVarietale_R
        Dim objIstat As New AgronicaCoreMetaSchemaDAL.Istat_R

        If Not IsNothing(myISWSResponse.possessi) Then

            For i = 0 To myISWSResponse.possessi.Count - 1

                If IsNothing(myISWSResponse.possessi(i).sezione) OrElse myISWSResponse.possessi(i).sezione = "" Then
                    strSezione = "0"
                Else
                    strSezione = myISWSResponse.possessi(i).sezione
                End If

                If IsNothing(myISWSResponse.possessi(i).subalterno) OrElse
                   myISWSResponse.possessi(i).subalterno = "" OrElse
                   myISWSResponse.possessi(i).subalterno = "000" Then
                    strSubalterno = "0"
                Else
                    strSubalterno = myISWSResponse.possessi(i).subalterno
                End If

                If Not IsNothing(myISWSResponse.possessi(i).macrousi) Then

                    For j = 0 To myISWSResponse.possessi(i).macrousi.Count - 1

                        If Not IsNothing(myISWSResponse.possessi(i).macrousi(j).utilizzi) Then

                            For k = 0 To myISWSResponse.possessi(i).macrousi(j).utilizzi.Count - 1

                                Veg_Cod = 0
                                Veg_Cod_CodificaCultivar = 0
                                Cul_Cod = 0
                                Grfi_Cod = 0
                                Id_Cod = 0
                                Grva_Cod = 0

                                Veg_Cod_Agea = ""
                                Cul_Cod_Agea = ""
                                Uso_Cod_Agea = ""
                                Occupazione_Cod_Agea = ""
                                Destinazione_Cod_Agea = ""
                                Qualita_Cod_Agea = ""

                                Veg_Des = ""
                                Cul_Des = ""
                                Grfi_Des = ""
                                Grva_Des = ""
                                Sa_Nome = ""

                                Cul_Cod_Cliente = myISWSResponse.possessi(i).macrousi(j).utilizzi(k).codVarieta
                                Macrouso_Cod = myISWSResponse.possessi(i).macrousi(j).codMacrouso

                                If ANNO <= 2014 Then
                                    Veg_Cod_Cliente = Strings.Right("000" & myISWSResponse.possessi(i).macrousi(j).utilizzi(k).codColtura, 3)
                                    Dim objUtilizzi1 As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_R
                                    objUtilizzi1.Specie_e_Varieta_Gias_Da_Agea("",
                                                                              "",
                                                                              Veg_Cod_Cliente, Cul_Cod_Cliente,
                                                                              Veg_Cod, Cul_Cod, Grfi_Cod, Grva_Cod, Id_Cod,
                                                                              "", "",
                                                                              myISWSResponse.possessi(i).macrousi(j).utilizzi(k).descColtura,
                                                                              myISWSResponse.possessi(i).macrousi(j).utilizzi(k).descVarieta,
                                                                              CDate(strValiditaInizio),
                                                                              Uso_Cod_Agea,
                                                                                Occupazione_Cod_Agea,
                                                                                Destinazione_Cod_Agea,
                                                                                Qualita_Cod_Agea,
                                                                              objParametri_Server)
                                Else
                                    Dim cod_ColturaArr = myISWSResponse.possessi(i).macrousi(j).utilizzi(k).codColtura.Split("-")
                                    Veg_Cod_Agea = ""
                                    Cul_Cod_Agea = cod_ColturaArr(4)
                                    Uso_Cod_Agea = cod_ColturaArr(0)
                                    Occupazione_Cod_Agea = cod_ColturaArr(1)
                                    Destinazione_Cod_Agea = cod_ColturaArr(2)
                                    Qualita_Cod_Agea = cod_ColturaArr(3)

                                    If Cul_Cod_Agea = "000" AndAlso myISWSResponse.possessi(i).macrousi(j).utilizzi(k).codVarieta <> "000" Then
                                        Cul_Cod_Agea = myISWSResponse.possessi(i).macrousi(j).utilizzi(k).codVarieta
                                    End If

                                    objUtilizzi.Specie_e_Varieta_Gias_Da_AGEA("",
                                                          "",
                                                          Veg_Cod_Agea,
                                                          Cul_Cod_Agea,
                                                          Uso_Cod_Agea,
                                                          Occupazione_Cod_Agea,
                                                          Destinazione_Cod_Agea,
                                                          Qualita_Cod_Agea,
                                                          Veg_Cod, Cul_Cod,
                                                          Grfi_Cod,
                                                          Grva_Cod,
                                                          Id_Cod,
                                                          "",
                                                          "",
                                                          objParametri_Server,
                                                          "",
                                                          "")
                                End If

                                If Veg_Cod = 0 Then
                                    Veg_Cod = Veg_Cod_CodificaCultivar
                                End If

                                If CInt(Veg_Cod) <> 0 AndAlso CInt(Cul_Cod) = 0 Then
                                    Cul_Cod = objCultivar.VarietaAltre(Veg_Cod, objParametri_Server)
                                End If

                                Dim z As Integer
                                Dim strZona As String = "n"
                                Dim CodiceZonaGIAS As Integer

                                If Not IsNothing(myISWSResponse.possessi(i).codZone) Then

                                    For z = 0 To myISWSResponse.possessi(i).codZone.Count - 1
                                        CodiceZonaGIAS = objCodificaZona.ConvertiZona(myISWSResponse.possessi(i).codZone(z),
                                                                                      "", objParametri_Server)

                                        If CodiceZonaGIAS = "-17" Then
                                            strZona = "v"
                                            Exit For
                                        End If
                                    Next

                                End If

                                Dim flagIrrigabilita As String = "0"
                                If Not IsNothing(myISWSResponse.possessi(i).flagIrrigabilita) AndAlso LCase(myISWSResponse.possessi(i).flagIrrigabilita) = "s" Then
                                    flagIrrigabilita = "1"
                                End If

                                Dim flagSecondoRaccolto As String = "0"
                                If Not IsNothing(myISWSResponse.possessi(i).flagSecondoRaccolto) AndAlso LCase(myISWSResponse.possessi(i).flagSecondoRaccolto) = "s" Then
                                    flagSecondoRaccolto = "1"
                                End If

                                Prov = myISWSResponse.possessi(i).codProv
                                Com = myISWSResponse.possessi(i).codCom
                                Sezione = strSezione
                                Foglio = myISWSResponse.possessi(i).foglio
                                Numero = myISWSResponse.possessi(i).particella
                                Subalterno = strSubalterno

                                Dim numPiante As Integer = 0
                                Validita_Inizio_Impianto = strValiditaInizio
                                Cop_Cod = 0

                                If myISWSResponse.possessi(i).macrousi(j).utilizzi(k).unitaArboree IsNot Nothing AndAlso myISWSResponse.possessi(i).macrousi(j).utilizzi(k).unitaArboree.Count > 0 Then

                                    For Each unitaArborea In myISWSResponse.possessi(i).macrousi(j).utilizzi(k).unitaArboree
                                        numPiante += unitaArborea.numeroPiante
                                        Validita_Inizio_Impianto = "01/01/" & unitaArborea.annoImpianto
                                        If unitaArborea.scoProtezione IsNot Nothing Then
                                            Cop_Cod = cls_agrea.ConvertiCopertura(unitaArborea.scoProtezione)
                                        End If
                                    Next

                                End If

                                Dim DataInizioDestinazione As String = strValiditaInizio

                                If myISWSResponse.possessi(i).macrousi(j).utilizzi(k).dataInizioDestinazione IsNot Nothing AndAlso IsDate(myISWSResponse.possessi(i).macrousi(j).utilizzi(k).dataInizioDestinazione) Then
                                    DataInizioDestinazione = myISWSResponse.possessi(i).macrousi(j).utilizzi(k).dataInizioDestinazione
                                End If

                                Dim DataFineDestinazione As String = strValiditaFine

                                If myISWSResponse.possessi(i).macrousi(j).utilizzi(k).dataFineDestinazione IsNot Nothing AndAlso IsDate(myISWSResponse.possessi(i).macrousi(j).utilizzi(k).dataFineDestinazione) Then
                                    DataFineDestinazione = myISWSResponse.possessi(i).macrousi(j).utilizzi(k).dataFineDestinazione
                                End If

                                MetodoProduzione_Cod = 1
                                If myISWSResponse.possessi(i).scoBioProdInt IsNot Nothing Then
                                    MetodoProduzione_Cod = cls_agrea.ConvertiMetodo_Produzione(myISWSResponse.possessi(i).scoBioProdInt)
                                End If

                                Select Case MetodoProduzione_Cod
                                    Case 1
                                        MetodoProduzione_Des = "Convenzionale"
                                    Case 2
                                        MetodoProduzione_Des = "In Conversione"
                                    Case 3
                                        MetodoProduzione_Des = "Biologico"
                                End Select

                                Macrouso_Sup = myISWSResponse.possessi(i).macrousi(j).supMacrouso
                                Utilizzo_Sup = myISWSResponse.possessi(i).macrousi(j).utilizzi(k).supUtilizzo

                                '(29/04/2015 fede) verifico in quale centro è la particella
                                If DtParticelleDB IsNot Nothing AndAlso DtParticelleDB.Rows.Count > 0 Then
                                    Dim DrParticella As DataRow() = DtParticelleDB.Select("PROV='" & Prov & "' AND COM='" & Com & "' AND Sezione='" & Sezione & "' AND foglio=" & Foglio.ToString & " AND Numero=" & Numero.ToString & " AND subalterno='" & Subalterno & "'")
                                    If DrParticella IsNot Nothing AndAlso DrParticella.Length > 0 Then
                                        SaCod = DrParticella(0).Item("sa_cod")
                                        Sa_Nome = objCentri.SaNome_from_SaCod(Piva, SaCod, objParametri_Server)
                                    Else
                                        CatastoDaImportare = True
                                        Exit Sub
                                    End If
                                End If

                                Dim sMacrousoSup As String = ""
                                If Not (iAggregaSpecie Or bAggregaTare) Then
                                    sMacrousoSup = " (" & Macrouso_Sup & " Ha) "
                                End If

                                Dim objMacrousi As New AgronicaCoreMetaSchemaDAL.Macrousi_R
                                strMacrousi = objMacrousi.Leggi_MacrousoDes_from_MacrousoCod(Macrouso_Cod,
                                                                                             enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                                             "", "",
                                                                                             objParametri_Server) & sMacrousoSup

                                If Veg_Cod <> 0 Then

                                    Dim dtSpecieVegetali As DataTable = objSpec.Leggi(Veg_Cod, 0, "", "", 0, "", "", objParametri_Server)
                                    If Not IsNothing(dtSpecieVegetali) AndAlso dtSpecieVegetali.Rows.Count = 1 Then
                                        Veg_Des = dtSpecieVegetali.Rows(0).Item("Veg_Des")
                                    End If
                                End If

                                If Id_Cod <> 0 Then

                                    Dim dtCodiciAna As DataTable = objCodAna.Leggi(Id_Cod, "", "", "", objParametri_Server)
                                    If Not IsNothing(dtCodiciAna) AndAlso dtCodiciAna.Rows.Count = 1 Then
                                        Veg_Des = dtCodiciAna.Rows(0).Item("descrizione")
                                    End If
                                End If

                                If Cul_Cod <> 0 Then

                                    Dim dtCultivar As DataTable = objCulti.Leggi(Cul_Cod, Veg_Cod, "", 0, "", "", objParametri_Server)
                                    If Not IsNothing(dtCultivar) AndAlso dtCultivar.Rows.Count = 1 Then
                                        Cul_Des = dtCultivar.Rows(0).Item("Cul_Des")
                                    End If
                                End If

                                If Grfi_Cod <> 0 Then

                                    Dim dtGruppoFinalita As DataTable = objGrfi.Leggi(Grfi_Cod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                                    If Not IsNothing(dtGruppoFinalita) AndAlso dtGruppoFinalita.Rows.Count = 1 Then
                                        Grfi_Des = dtGruppoFinalita.Rows(0).Item("Grfi_Des")
                                    End If
                                End If

                                If Grva_Cod <> 0 Then

                                    Dim dtGruppoVarietale As DataTable = objGrva.Leggi(Veg_Cod, Grva_Cod, "", enumSelezioneVariabile.Selezione_JoinCompleta, "", "", objParametri_Server)
                                    If Not IsNothing(dtGruppoVarietale) AndAlso dtGruppoVarietale.Rows.Count = 1 Then
                                        Grva_Des = dtGruppoVarietale.Rows(0).Item("Grva_Des")
                                    End If
                                End If

                                TitoloPossessoDes = myISWSResponse.possessi(i).descrizioneConduzione
                                TitoloPossesso = 1

                                objIstat.ComuneProvinciaSigla_from_codISTAT(Prov, Com, Prov_Des, "", Com_Des, "", "", objParametri_Server)

                                strUtilizzi = Veg_Des
                                strVarieta = Cul_Des

                                TipoZona = ""
                                TipoZona_Des = ""

                                '(07/11/2016 fede) controllo selezione utente
                                Dim lPart As String = myISWSResponse.possessi(i).codProv & " " & myISWSResponse.possessi(i).codCom & " " &
                                    myISWSResponse.possessi(i).sezione & " " & myISWSResponse.possessi(i).foglio & " " &
                                    myISWSResponse.possessi(i).particella & " " & myISWSResponse.possessi(i).subalterno & " " &
                                    myISWSResponse.possessi(i).macrousi(j).utilizzi(k).codColtura & " " & myISWSResponse.possessi(i).macrousi(j).utilizzi(k).codVarieta & " " &
                                    myISWSResponse.possessi(i).macrousi(j).utilizzi(k).supUtilizzo.ToString


                                DR = DtParticelle.NewRow

                                DR.Item("PROV") = Prov
                                DR.Item("COM") = Com
                                DR.Item("Prov_Des") = Prov_Des
                                DR.Item("Com_Des") = Com_Des
                                DR.Item("Sezione") = Sezione
                                DR.Item("Foglio") = Foglio
                                DR.Item("Numero") = Numero
                                DR.Item("Subalterno") = Subalterno

                                DR.Item("Catasto") = Prov_Des & ":" &
                                                             Com_Des & ":_" &
                                                                    Sezione & ":_" &
                                                                    Foglio.ToString & ":_" &
                                                                    Numero.ToString & ":_" &
                                                                    Subalterno

                                DR.Item("possesso") = TitoloPossessoDes
                                DR.Item("TitoloPossesso") = TitoloPossesso
                                DR.Item("inizio_possesso") = Inizio_Possesso
                                DR.Item("fine_possesso") = Fine_Possesso
                                DR.Item("datepossesso") = If(myISWSResponse.possessi(i).dataInizioPoss <> "01/01/1900", "Dal " & myISWSResponse.possessi(i).dataInizioPoss, "Dal ...") &
                                                                                    If(myISWSResponse.possessi(i).dataFinePoss <> "31/12/2099", " al " & myISWSResponse.possessi(i).dataFinePoss, " al ...")
                                supCatasto = CDbl(myISWSResponse.possessi(i).supCatastale) / 10000.0
                                objXML.EttariAreCentiare_from_Ettari(supCatasto, Ettari, Are, Centiare)
                                DR.Item("sup") = Format(supCatasto, "0.0000")
                                supConduzione = CDbl(myISWSResponse.possessi(i).supCondotta)
                                DR.Item("supcondotta") = Format(supConduzione, "0.0000")

                                DR.Item("Catasto_Key") = Prov & "_" &
                                                                    Com & "_" &
                                                                    strSezione & "_" &
                                                                    Foglio.ToString & "_" &
                                                                    Numero.ToString & "_" &
                                                                    strSubalterno & ":" &
                                                                    Utilizzo_Sup.ToString

                                DR.Item("macrouso_cod") = Macrouso_Cod
                                DR.Item("macrouso_sup") = Macrouso_Sup
                                DR.Item("macrouso") = strMacrousi

                                DR.Item("utilizzo") = strUtilizzi
                                DR.Item("varieta") = strVarieta
                                DR.Item("utilizzo_sup") = Utilizzo_Sup
                                DR.Item("Veg_Cod_Agea") = Veg_Cod_Agea
                                DR.Item("Cul_Cod_Agea") = Cul_Cod_Agea
                                DR.Item("Veg_Cod") = Veg_Cod & "|" & Id_Cod
                                DR.Item("Cul_Cod") = Cul_Cod
                                DR.Item("Grfi_Cod") = Grfi_Cod
                                DR.Item("Grva_Cod") = Grva_Cod
                                DR.Item("Id_Cod") = Id_Cod

                                'DRUDI
                                'Recupero descrizione specie, varietà...

                                DR.Item("veg_des") = Veg_Des
                                DR.Item("cul_des") = Cul_Des
                                DR.Item("grfi_des") = Grfi_Des
                                DR.Item("grva_des") = Grva_Des

                                DR.Item("Scarto") = 0

                                DR.Item("validita_inizio") = DataInizioDestinazione
                                DR.Item("validita_fine") = DataFineDestinazione

                                DR.Item("Sa_Nome") = Sa_Nome
                                DR.Item("App_Nome") = N_App.ToString
                                N_App += 1

                                DR.Item("piva") = Piva
                                DR.Item("sa_cod") = SaCod
                                DR.Item("appezza") = appezza
                                appezza -= 1

                                DR.Item("programmazione_entita_cod") = 0

                                DR.Item("ribaltato") = 0
                                DR.Item("movimentato") = 0

                                DR.Item("stato_ribaltamento") = 0

                                DR.Item("Uso_Cod_Agea") = Uso_Cod_Agea
                                DR.Item("Occupazione_Cod_Agea") = Occupazione_Cod_Agea
                                DR.Item("Destinazione_Cod_Agea") = Destinazione_Cod_Agea
                                DR.Item("Qualita_Cod_Agea") = Qualita_Cod_Agea

                                DR.Item("Veg_Cod_Agea") = Veg_Cod_Agea
                                DR.Item("Veg_Des_Agea") = Veg_Des_Agea
                                DR.Item("Cul_Cod_Agea") = Cul_Cod_Agea
                                DR.Item("Cul_Des_Agea") = Cul_Des_Agea
                                DR.Item("Uso_Cod_Agea") = Uso_Cod_Agea
                                DR.Item("Uso_Des_Agea") = Uso_Des_Agea
                                DR.Item("Occupazione_Cod_Agea") = Occupazione_Cod_Agea
                                DR.Item("Occupazione_Des_Agea") = Occupazione_Des_Agea
                                DR.Item("Destinazione_Cod_Agea") = Destinazione_Cod_Agea
                                DR.Item("Destinazione_Des_Agea") = Destinazione_Des_Agea
                                DR.Item("Qualita_Cod_Agea") = Qualita_Cod_Agea
                                DR.Item("Qualita_Des_Agea") = Qualita_Des_Agea
                                DR.Item("Gru_Cod") = Gru_Cod
                                DR.Item("Cop_Cod") = Cop_Cod
                                DR.Item("Cop_Des") = Cop_Des
                                DR.Item("Lotto") = ""
                                DR.Item("Resa") = 0
                                DR.Item("Num_Piante") = numPiante
                                DR.Item("TRA_Fila") = 0
                                DR.Item("SU_Fila") = 0
                                DR.Item("Validita_Inizio_Impianto") = Validita_Inizio_Impianto
                                DR.Item("TipoZona") = TipoZona
                                DR.Item("TipoZona_Des") = TipoZona_Des
                                DR.Item("MetodoProduzione_Cod") = MetodoProduzione_Cod
                                DR.Item("MetodoProduzione_Des") = MetodoProduzione_Des
                                DR.Item("Unita_Vitata") = 0

                                DR.Item("Dpi_Cod") = Dpi_Cod
                                DR.Item("Reg_Cod") = Reg_Cod
                                DR.Item("StatoImpianto_Cod") = StatoImpianto_Cod
                                DR.Item("N") = N
                                DR.Item("P") = P
                                DR.Item("K") = K1
                                DR.Item("Data_Semina") = Data_Semina
                                DR.Item("Data_Raccolta") = Data_Raccolta
                                DR.Item("Data_Fioritura") = Data_Fioritura
                                DR.Item("Coltura_Precedente") = Coltura_Precedente
                                DR.Item("Coltura_Precedente2") = Coltura_Precedente2
                                DR.Item("Coltura_Precedente3") = Coltura_Precedente3
                                DR.Item("Coltura_Precedente4") = Coltura_Precedente4
                                DR.Item("Piano_Semina") = Piano_Semina
                                DR.Item("Codice_Contratto") = Codice_Contratto
                                DR.Item("unito") = unito
                                DR.Item("frazionato") = frazionato
                                DR.Item("Codice_Fiscale_Tecnico") = Codice_Fiscale_Tecnico

                                DR.Item("stato_ribaltamento") = stato_ribaltamento
                                DR.Item("provenienza_fascicolo") = provenienza_fascicolo
                                DR.Item("Regolamento_Concimazione_Cod") = 0
                                DR.Item("Flag_PubblicoPrivato") = 0
                                DR.Item("id_tr") = 0
                                DR.Item("Disciplinare") = "0"

                                DR.Item("DistBZ_CorpiIdrici") = 0
                                DR.Item("DistBZ_AreeResPub") = 0
                                DR.Item("DistBZ_Allevamenti") = 0
                                DR.Item("DistBZ_VegNatNonColt") = 0
                                DR.Item("SupBZ_Riduzione") = 0
                                'If Not HashMacrousi.ContainsKey(Macrouso_Cod & Qualita & Specie_Cod & Varieta_Cod) Then

                                '    HashMacrousi.Add(Macrouso_Cod & Qualita & Specie_Cod & Varieta_Cod, "")

                                DR.Item("campo_cod") = 0
                                DR.Item("campo_des") = ""

                                DR.Item("chiave") = chiave
                                chiave += 1

                                DtParticelle.Rows.Add(DR)


                            Next

                        End If 'utilizzi

                    Next

                End If 'macrousi

            Next

        End If 'possessi

    End Sub

    Public Function GeneraRandom(objParametri_Server As AgronicaCoreParametri,
                                 objParametri_Utenti As AgronicaCoreParametri) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(AGRODATAINIZIO, AGRODATAFINE)

            Dim ok = False


            Dim objAgroSe As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim objCont As New AgronicaCoreAnagrafeDAL.Contatti_R
            Dim dt As DataTable

            Dim str_r As String = ""
            While ok = False
                str_r = objAgroSe.NuovoId_Tabella("impresa", 0, 0, objParametri_Server).ToString.Replace("-", "F")
                'controllo se è già usato 
                dt = objCont.LeggiContattoSpecifico("", str_r, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
                If dt.Rows.Count = 0 Then
                    ok = True
                End If
            End While
            objParametri_Server.ResettaFinestra()

            r.RispostaOK = True
            r.RispostaStringa = str_r

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    Public Sub dtParticelleAggrega(ByVal iAggregaSpecie As Integer,
                                    ByVal bDividiCentri As Boolean,
                                    ByVal bAggregaTare As Boolean,
                                    ByVal DtParticelle As DataTable,
                                    ByRef dtParticelleAggregate As DataTable)


        If DtParticelle.Rows.Count > 0 Then

            Dim qryRisultato As IEnumerable(Of DataRow)

            Dim qrySpecie As IEnumerable(Of DataRow)
            Dim qryTare As IEnumerable(Of DataRow)


            'Suddividi Unità Produttive in relazione alla localizzazione delle particelle catastali (Provincia, Comune)
            If bDividiCentri Then
                For ipart As Integer = 0 To DtParticelle.Rows.Count - 1

                    Dim curProCom As String = DtParticelle.Rows(ipart)("Prov") & "-" & DtParticelle.Rows(ipart)("Com")
                    Dim curProComDes As String = DtParticelle.Rows(ipart)("Prov_Des") & "-" & DtParticelle.Rows(ipart)("Com_Des")

                    DtParticelle.Rows(ipart)("Sa_Nome") = curProComDes ' curProCom

                Next

            End If

            'tutto rimane com'è.
            If Not (iAggregaSpecie <> 0 OrElse bAggregaTare) Then
                qryRisultato = (From dtt In DtParticelle.AsEnumerable Select dtt)
                CreaStringaCatasto(qryRisultato)
            Else

                'Aggrega le superfici coltivabili per uniformità di Utilizzo (Specie Vegetale), somma gli ettari e crea un unico Appezzamento (oppure le imposta come sono)
                Select Case iAggregaSpecie
                    Case 1
                        'qrySpecie = aggregazioneParticelle(0, DtParticelle, dtParticelleAggregate)
                    Case 2
                        'qrySpecie = aggregazioneParticelleComune(0, DtParticelle, dtParticelleAggregate)
                    Case 3
                        'qrySpecie = aggregazioneParticelleConduzione(0, DtParticelle, dtParticelleAggregate)
                    Case 0
                        qrySpecie = (
                        From dtt In DtParticelle.AsEnumerable
                        Where Not {"810", "840", "880", "920"}.Contains(dtt("Macrouso_Cod"))
                        Select dtt)
                        CreaStringaCatasto(qrySpecie)
                End Select
                'If bAggregaSpecie Then
                '    qrySpecie = aggregazioneParticelle(0, DtParticelle, dtParticelleAggregate)
                'Else
                '    qrySpecie = (
                '    From dtt In DtParticelle.AsEnumerable
                '    Where Not {"810", "840", "880", "920"}.Contains(dtt("Macrouso_Cod"))
                '    Select dtt)
                '    CreaStringaCatasto(qrySpecie)
                'End If


                'Aggrega le Tare, somma gli ettari e crea un unico Appezzamento (oppure le imposta come sono)
                If bAggregaTare Then
                    'qryTare = aggregazioneParticelle(840, DtParticelle, dtParticelleAggregate)
                Else
                    qryTare = (
                    From dtt In DtParticelle.AsEnumerable
                    Where {"810", "840", "880", "920"}.Contains(dtt("Macrouso_Cod"))
                    Select dtt)
                    CreaStringaCatasto(qryTare)
                End If

                qryRisultato = qrySpecie
                qryRisultato = qryRisultato.Concat(qryTare)

            End If

            dtParticelleAggregate = qryRisultato.CopyToDataTable()

            'ricalcolo numerazione.
            For numApp_Nome As Integer = 0 To dtParticelleAggregate.Rows.Count - 1
                dtParticelleAggregate.Rows(numApp_Nome)("App_Nome") = (numApp_Nome + 1).ToString
            Next

        End If

    End Sub

    Private Sub CreaStringaCatasto(DTParticelle As IEnumerable(Of DataRow))
        For Each row As DataRow In DTParticelle
            row.Item("catasto") = "[" & impostaCatasto(row.Item("Prov"),
                                                       row.Item("Prov_Des"),
                                                       row.Item("Com"),
                                                       row.Item("Com_Des"),
                                                       row.Item("Sezione"),
                                                       row.Item("Foglio"),
                                                       row.Item("Numero"),
                                                       row.Item("Subalterno"),
                                                       row.Item("supcondotta"),
                                                       row.Item("possesso"),
                                                       row.Item("datepossesso"),
                                                       row.Item("inizio_possesso"),
                                                       row.Item("fine_possesso"),
                                                       row.Item("sup"),
                                                       row.Item("utilizzo_sup")) & "]"
        Next
    End Sub

    Private Function impostaCatasto(Istat_Prov As String,
                                    Prov As String,
                                    Istat_Com As String,
                                    Com As String,
                                    Sezione As String,
                                    Foglio As Integer,
                                    Numero As Integer,
                                    Subalterno As String,
                                    Sup_Condotta As Double,
                                    possesso As String,
                                    datepossesso As String,
                                    inizio_possesso As Date,
                                    fine_possesso As Date,
                                    sup As Double,
                                    utilizzo_sup As Double) As String
        Dim Catasto As New CatastoAppezzamento
        If Istat_Prov IsNot Nothing Then
            Catasto.Istat_Prov = Istat_Prov
            Catasto.Prov = Prov
            Catasto.Istat_Com = Istat_Com
            Catasto.Com = Com
            Catasto.Sezione = Sezione
            Catasto.Foglio = Foglio
            Catasto.Numero = Numero
            Catasto.Subalterno = Subalterno
            Catasto.Sup_Condotta = Sup_Condotta
            Catasto.possesso = possesso
            Catasto.datepossesso = datepossesso
            Catasto.inizio_possesso = inizio_possesso
            Catasto.fine_possesso = fine_possesso
            Catasto.sup = sup
            Catasto.utilizzo_sup = utilizzo_sup
            Catasto.key = Istat_Prov & "_" & Istat_Com & "_" & Sezione & "_" & CStr(Foglio) & "_" & CStr(Numero) & "_" & Subalterno
            Dim ser = Newtonsoft.Json.JsonSerializer.Create
            Dim sb As New StringBuilder
            Dim sw As New StringWriter(sb)
            ser.Serialize(sw, Catasto)
            Return sb.ToString
        Else
            Return "[]"
        End If

    End Function

    Public Class CatastoAppezzamento
        Public Property Istat_Prov As String
        Public Property Prov As String
        Public Property Com As String
        Public Property Istat_Com As String
        Public Property Sezione As String
        Public Property Foglio As Integer
        Public Property Numero As Integer
        Public Property Subalterno As String
        Public Property Sup_Condotta As Double
        Public Property key As String
        Public Property possesso As String
        Public Property datepossesso As String
        Public Property inizio_possesso As Date
        Public Property fine_possesso As Date
        Public Property sup As Double
        Public Property utilizzo_sup As Double
    End Class

    Public Function Importa_Dati_Catasto(ByVal StringaConnessione As String,
                                         ByVal Utente_Username As String,
                                         ByVal Utente_Password As String,
                                         ByVal ProgressivoGIAS As Integer,
                                         ByVal CodiceChiaveCliente As Integer,
                                         ByVal LinkWSImportaGIAS As String,
                                         ByVal LogDirectory As String,
                                         ByVal LogFileName As String,
                                         ByRef Num_Particelle_Importate As Integer,
                                         ByRef Num_Impianti_Importati As Integer,
                                         ByRef Num_Particelle_NON_Importate As Integer,
                                         ByRef Num_Impianti_NON_Importati As Integer,
                                         ByRef Messaggio As String,
                                         ByRef LogCodificheMancantiSpecie As String,
                                         ByRef LogCodificheMancantiVarieta As String,
                                         ByRef objParametri_Server As AgronicaCoreParametri,
                                         ByRef objParametri_Utenti As AgronicaCoreParametri
                                         ) As Boolean

        Dim Dt_Imprese As New DataTable
        Dim Flag_Risultato As Boolean = False
        Dim NomeRoutine As String = "Importa_Dati"
        Dim customLOGParams As New CustomLOGParams With {
            .LogDescrizioneUtente = objParametri_Server.LogDescrizioneUtente,
            .LogDirectory = LogDirectory,
            .LogFileName = LogFileName
        }

        Dim objLog As New AgronicaCoreDataProvider.LogProvider

        objLog.Scrivi_LOG(objParametri_Server,
                          NomeRoutine,
                          "Inizio importazione",
                          CustomLOGParams:=customLOGParams)

        'Leggo i File
        Crea_Dt_Imprese(StringaConnessione, Messaggio, Dt_Imprese)

        objLog.Scrivi_LOG(objParametri_Server,
                          NomeRoutine,
                          Messaggio,
                          CustomLOGParams:=customLOGParams)

        Dim import_agrea_utils As New AGREA_Utility
        Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Dim aziendaVisibile As Boolean = False
        For Each cuaaRow In Dt_Imprese.Rows

            Dim cuaa = cuaaRow(0)

            Dim piva = objImprese.Piva_from_IdCodValCod(1010, cuaa, objParametri_Server)

            If piva = "" Then
                Try



                    objLog.Scrivi_LOG(objParametri_Server,
                                      NomeRoutine,
                                      "Importo CUAA:" & cuaa,
                                      CustomLOGParams:=customLOGParams)

                    Dim fascicolo As String
                    Dim Num_Scheda As String
                    Dim DataValidazione As Date
                    Dim OrigineOpr As String
                    Dim strErr As String = ""

                    import_agrea_utils.Leggi_Fascicoli_UltimaValidazione_AGREA(cuaa, 4, Num_Scheda, DataValidazione, OrigineOpr, strErr, objParametri_Server)

                    Dim errCOD = 0
                    Dim errMsg = ""

                    aziendaVisibile = False
                    If Num_Scheda IsNot Nothing Then

                        fascicolo = import_agrea_utils.CaricaDati_WS_AgroFascicolo_AGREA(errCOD, errMsg, cuaa, Num_Scheda, objParametri_Server)

                        CreaAzienda(ProgressivoGIAS, Utente_Password, "", cuaa, fascicolo, "", "", "", "", "", "", "", "", objParametri_Server, objParametri_Utenti, aziendaVisibile, "")

                    End If

                Catch ex As Exception

                    objLog.Scrivi_LOG(objParametri_Server,
                                      NomeRoutine,
                                      "Errore su import CUAA " & cuaa & ": " & ex.Message,
                                      CustomLOGParams:=customLOGParams)

                End Try
            End If

        Next


    End Function

    Private Sub Crea_Dt_Imprese(ByVal StringaConnessione As String, ByRef Messaggio As String, ByRef Dt As DataTable)

        Try

            Dim ds As New DataSet
            Dim MyConnection As New System.Data.OleDb.OleDbConnection(StringaConnessione)
            MyConnection.Open()
            Dim dtSheet = MyConnection.GetSchema("Tables")
            Dim firstSheet = dtSheet.Rows(0)("TABLE_NAME").ToString()
            Dim da As New System.Data.OleDb.OleDbDataAdapter("select * from [" & firstSheet & "]", MyConnection)
            da.Fill(ds, "fileXls")
            MyConnection.Close()

            Dt = ds.Tables(0)
        Catch ex As Exception
            Messaggio = "Errore all'apertura del file excel: " & ex.Message
        End Try


    End Sub

    Public Sub PopolaDT_Appezzamenti(ByVal Piva As String,
                                     ByRef fascicolo As String,
                                     ByVal iAggregaSpecie As Integer,
                                     ByVal bAggregaTare As Boolean,
                                     ByRef DataInizio As Date,
                                     ByRef DataFine As Date,
                                     ByRef DtParticelle As DataTable,
                                     ByRef objParametri_Server As AgronicaCoreParametri,
                                     ByRef objParametri_Utenti As AgronicaCoreParametri,
                                     ByRef Allegati_Documenti_Numero As String,
                                     ByRef ASG_Utente_Username As String,
                                     ByRef ASG_Utente_Password As String,
                                     ByRef ASG_ProgressivoGIAS As String)

        Dim obj_fascicolo As New Importazione_Agrea_WS.pc.common.webservice.sop.agrea.it.ISWSResponse

        If fascicolo <> "" Then
            XMLUtility.getObjectFromResponse(fascicolo, obj_fascicolo, Nothing, False)
        End If

        Dim Cuaa = obj_fascicolo.azienda.cuaa

        Try

            Dim importCatasto As Boolean = False
            Popola_DtAppezzamenti_daFascicolo_3(Piva,
                                                obj_fascicolo,
                                                False,
                                                False,
                                                DataInizio,
                                                DataFine,
                                                DtParticelle,
                                                objParametri_Server,
                                                objParametri_Utenti,
                                                Allegati_Documenti_Numero,
                                                importCatasto,
                                                ASG_Utente_Username,
                                                ASG_Utente_Password,
                                                ASG_ProgressivoGIAS, True)

            If importCatasto Then

                Popola_DtAppezzamenti_daFascicolo_3(Piva,
                                                obj_fascicolo,
                                                False,
                                                False,
                                                DataInizio,
                                                DataFine,
                                                DtParticelle,
                                                objParametri_Server,
                                                objParametri_Utenti,
                                                Allegati_Documenti_Numero,
                                                importCatasto,
                                                ASG_Utente_Username,
                                                ASG_Utente_Password,
                                                ASG_ProgressivoGIAS, True)

            End If


        Catch ex As Exception

        End Try


    End Sub

    Private Shared Function ImportaCatastoFascicolo(ByRef fascicolo As String,
                                                    piva As String,
                                                    cuaa As String,
                                                    aggregaCentri As Boolean,
                                                    objParametri_Server As AgronicaCoreParametri,
                                                    objParametri_Utenti As AgronicaCoreParametri,
                                                    ASG_Utente_Username As String,
                                                    ASG_Utente_Password As String,
                                                    ASG_ProgressivoGIAS As String)

        Dim obj_fascicolo As New Importazione_Agrea_WS.pc.common.webservice.sop.agrea.it.ISWSResponse

        If fascicolo <> "" Then
            XMLUtility.getObjectFromResponse(fascicolo, obj_fascicolo, Nothing, False)
        End If

        Dim objImp As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim objImpGer As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R

        Dim Esiste_Impresa = objImp.VerificaEsistenza_PivaGIAS(obj_fascicolo.azienda.partitaIva,
                                                                       objParametri_Server)
        If Not Esiste_Impresa Then
            Return False
        End If

        Dim Piva_Padre = objImpGer.LeggiPadre(piva, objParametri_Server, "")

        Dim strErr As String = ""
        Dim strRis As String = ""

        Dim rval As Boolean =
            CreaAziendaGias(ASG_ProgressivoGIAS, ASG_Utente_Password,
                        Piva_Padre, piva, cuaa,
                        strErr, strRis,
                        objParametri_Server, objParametri_Utenti, fascicolo)

        Return rval


    End Function

    Private Shared Function FascicoloMemorizza(ByVal Fonte_Cod As Integer, ByVal Piva As String, ByVal dataValidazione As DateTime, ByVal numeroValidazione As String, strFascicoli As String, objParametriServer As AgronicaCoreParametri, ByRef allegatoDocumentocod As Integer, Codice_Detentore As String) As Boolean

        Dim fascicoloAllegatiLettura As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_R
        Dim fascicoloAllegatiScrittura As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_W

        Dim esisteFascicolo As Boolean

        esisteFascicolo = fascicoloAllegatiLettura.EsisteDocumento_Da_Numero(numeroValidazione, Piva, enum_CategorieDocumenti.DomandaFascicolo, allegatoDocumentocod, objParametriServer, Fonte_Cod)

        If Not esisteFascicolo Then

            fascicoloAllegatiScrittura.Scrivi(Piva, "", enum_CategorieDocumenti.DomandaFascicolo, "", numeroValidazione, Fonte_Cod, "", AGRODATAINIZIO, AGRODATAFINE, allegatoDocumentocod, objParametriServer, dataValidazione, strXml:=strFascicoli, Codice_Detentore:=Codice_Detentore)

        Else

            fascicoloAllegatiScrittura.Modifica_FascicoloXML(allegatoDocumentocod, Piva, Fonte_Cod, strFascicoli, objParametriServer)

        End If

        Return esisteFascicolo

    End Function

End Class
