Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.attivita
Imports AgronicaCoreModelsSTD.attivita.centri_di_costo
Imports AgronicaCoreMetaSchemaDAL
Imports AgronicaCoreEntityFramework
Imports Newtonsoft.Json
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModelsSTD.metaschema
Imports AgronicaCoreModelsSTD.exceptions

Public Class SincroBDNAnimale
    Dim objParametriServer As AgronicaCoreParametri
    Dim objParametriUtenti As AgronicaCoreParametri

    Dim wsRegistroStalla As ChiamawsRegistroStallaQry
    Dim wsAnagraficaCapo As ChiamawsAnagraficaCapoQry
    Dim wsAziende As ChiamawsAziendeQry
    Dim wsCodici As ChiamawsCodiciQry
    Dim wsIdentificativi As ChiamawsIdentificativiGet
    Dim wsStrutture As ChiamawsStruttureQry
    Dim wsTerritorio As ChiamawsTerritorioQry
    Dim wsGestioneAssConsorzi As ChiamawsGestioneAssConsorzi
    Dim wsInterrogazioniModello4 As wsInterrogazioneModello4

    Dim Codifica_RazzeAnimali_DT As DataTable
    Dim GiasContext As Gias_DeveloperServer_Entities

    Dim Lista_Specie_Animali As List(Of AgronicaCoreEntityFramework_POCO.Lista_Specie_Animali)
    Dim Lista_Generi_Animali As List(Of AgronicaCoreEntityFramework_POCO.Lista_Generi_Animali)
    Dim Lista_Razze_Animali As List(Of AgronicaCoreEntityFramework_POCO.Lista_Razze_Animali)
    Dim Zoo_Animali_Lista_Stati_Accrescimento As List(Of AgronicaCoreEntityFramework_POCO.Zoo_Animali_Lista_Stati_Accrescimento)

    'Dim logDirectory As String
    'Dim logFileName As String
    Dim objLog As New AgronicaCoreDataProvider.LogProvider
    Dim listMotiviIngressiModello4 As New List(Of String) From {"I", "K", "Z", "N", "E", "T", "R", "M", "F", "S", "G", "L", "V", "W", "C", "P", "Q"}
    Dim listMotiviUscitaModello4 As New List(Of String) From {"J", "X", "H", "W", "T", "R", "I", "L", "K", "D", "F", "U", "E", "A", "V", "M", "N", "G", "S", "P"}

    Dim obj_ZooAnimali_R As AgronicaCoreAnagrafeDAL.Zoo_Animali
    Dim customLOGParams As CustomLOGParams

    Public Sub New(objParametriServer As AgronicaCoreParametri, objParametriUtenti As AgronicaCoreParametri, token As String, ruolo As String, valore_ruolo_codice As String)
        Me.objParametriServer = objParametriServer
        Me.objParametriUtenti = objParametriUtenti

        Dim logDirectory = objParametriServer.LogDirectory & "\BDN\"

        customLOGParams = New CustomLOGParams With {
            .LogDescrizioneUtente = objParametriServer.LogDescrizioneUtente,
            .LogDirectory = logDirectory,
            .LogFileName = "logBDN.txt"
        }

        Try
            Me.wsRegistroStalla = New ChiamawsRegistroStallaQry(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Catch ex As Exception
            objLog.Scrivi_LOG(objParametriServer, System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore creazione wsRegistroStalla: " & ex.Message,
                          CustomLOGParams:=customLOGParams)
        End Try


        Try
            Me.wsAnagraficaCapo = New ChiamawsAnagraficaCapoQry(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Catch ex As Exception
            objLog.Scrivi_LOG(objParametriServer, System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore creazione wsAnagraficaCapo: " & ex.Message,
                          CustomLOGParams:=customLOGParams)
        End Try

        Try
            Me.wsAziende = New ChiamawsAziendeQry(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Catch ex As Exception
            objLog.Scrivi_LOG(objParametriServer, System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore creazione wsAziende: " & ex.Message,
                          CustomLOGParams:=customLOGParams)
        End Try


        Try
            Me.wsCodici = New ChiamawsCodiciQry(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Catch ex As Exception
            objLog.Scrivi_LOG(objParametriServer, System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore creazione wsCodici: " & ex.Message,
                          CustomLOGParams:=customLOGParams)
        End Try

        Try
            Me.wsIdentificativi = New ChiamawsIdentificativiGet(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Catch ex As Exception
            objLog.Scrivi_LOG(objParametriServer, System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore creazione wsIdentificativi: " & ex.Message,
                          CustomLOGParams:=customLOGParams)
        End Try


        Try
            Me.wsStrutture = New ChiamawsStruttureQry(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Catch ex As Exception
            objLog.Scrivi_LOG(objParametriServer, System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore creazione wsStrutture: " & ex.Message,
                          CustomLOGParams:=customLOGParams)
        End Try

        Try
            Me.wsTerritorio = New ChiamawsTerritorioQry(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Catch ex As Exception
            objLog.Scrivi_LOG(objParametriServer, System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore creazione wsTerritorio: " & ex.Message,
                          CustomLOGParams:=customLOGParams)
        End Try

        Try
            Me.wsGestioneAssConsorzi = New ChiamawsGestioneAssConsorzi(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Catch ex As Exception
            objLog.Scrivi_LOG(objParametriServer, System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore creazione wsGestioneAssConsorzi: " & ex.Message,
                          CustomLOGParams:=customLOGParams)
        End Try

        Try
            Me.wsInterrogazioniModello4 = New wsInterrogazioneModello4(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Catch ex As Exception
            objLog.Scrivi_LOG(objParametriServer, System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore creazione wsInterrogazioniModello4: " & ex.Message,
                          CustomLOGParams:=customLOGParams)
        End Try


        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(Me.objParametriServer.StringaConnessione)

        GiasContext = New Gias_DeveloperServer_Entities(EFConnString)



        Dim obj_RazzeAnimali_R As New AgronicaCoreMetaSchemaDAL.Codifica_BDN_RazzeAnimali
        Me.Codifica_RazzeAnimali_DT = obj_RazzeAnimali_R.leggi(Me.objParametriServer,
                                                                            "", "",
                                                                            "", "", "",
                                                                            enum_Esportazioni_Sistema_Cod.BDN)


        Lista_Specie_Animali = (From a In GiasContext.Lista_Specie_Animali Select a).ToList()

        Lista_Generi_Animali = (From a In GiasContext.Lista_Generi_Animali Select a).ToList()

        Lista_Razze_Animali = (From a In GiasContext.Lista_Razze_Animali Select a).ToList()

        Zoo_Animali_Lista_Stati_Accrescimento = (From a In GiasContext.Zoo_Animali_Lista_Stati_Accrescimento Select a).ToList()

        obj_ZooAnimali_R = New AgronicaCoreAnagrafeDAL.Zoo_Animali()

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="p_azienda_codice"></param>
    ''' <param name="IdFiscale_Allevamento"></param>
    ''' <param name="SpeCodice"></param>
    ''' <param name="Allev_Id"></param>
    ''' <param name="Codice_Capo"></param>
    ''' <returns></returns>
    Public Function SincronizzaAnimale(ByVal Piva As String,
                                       ByVal Sa_Cod As Integer,
                                       ByVal Sta_Num As Integer,
                                       ByVal Codice_Azienda As String,
                                       ByVal IdFiscale_Allevamento As String,
                                       ByVal SpeCodice As String,
                                       ByVal Allev_Id As String,
                                       ByVal Codice_Capo As String,
                                       ByVal IdFiscale_Detentore As String,
                                       ByRef Motivo_Ingresso As String,
                                       ByRef Motivo_Uscita As String,
                                       ByRef Id_Ingresso As Integer,
                                       ByRef Id_Uscita As Integer,
                                       ByRef Codice_Asl As String,
                                       objLog As AgronicaCoreDataProvider.LogProvider,
                                       customLOGParams As CustomLOGParams,
                                       Optional Ingresso_Modello4_Prenotazione As String = "",
                                       Optional Ingresso_Modello4_Numero As String = "") As CapoAnimaleCDC

        Dim obj_CapoAnimaleCDC As New CapoAnimaleCDC()
        obj_CapoAnimaleCDC.codice = New CentroDiCosto.CodeType(Sa_Cod)
        Dim NomeRoutine As String = "SincronizzaAnimale"

        Try
            Dim objImpreseCodici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read

            Id_Ingresso = 0
            Id_Uscita = 0

            Dim dtAnimale = wsAnagraficaCapo.getCapo(Codice_Capo)
            If IsNothing(dtAnimale) OrElse dtAnimale.Rows.Count = 0 Then
                objLog.Scrivi_LOG(objParametriServer,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "Chiamata getCapo " & Codice_Capo & " in errore",
                              CustomLOGParams:=customLOGParams)
                Throw New Exception("Chiamata getCapo " & Codice_Capo & " in errore")
            End If

            Dim dtMovimentazioni As DataTable
            Try
                dtMovimentazioni = wsRegistroStalla.getMovimentazioniCapo(Codice_Capo)
            Catch ex As Exception
                objLog.Scrivi_LOG(objParametriServer,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "Chiamata getMovimentazioniCapo " & Codice_Capo & " in errore",
                              CustomLOGParams:=customLOGParams)
                Throw New Exception("Chiamata getMovimentazioniCapo " & Codice_Capo & " in errore")
            End Try
            Dim Azienda_Nascita = ""
            Dim codiceModello4_Uscita = ""
            'Dim Ingresso_Modello4_Numero = ""
            'Dim Ingresso_Modello4_Prenotazione = ""
            Dim Uscita_Modello4_Numero = ""
            Dim Uscita_Modello4_Prenotazione = ""
            Dim Uscita_Modello4_Data = AGRODATAINIZIO
            Dim codiceModello4_Ingresso = ""

            Dim obj_CapoAnimale As New CapoAnimale(Piva, 0, dtAnimale.Rows(0)("CODICE"))
            obj_CapoAnimale.validita = New IntervalloTemporale(Nothing, AGRODATAFINE)
            Dim validitaInizioImpostata As Boolean = False
            Dim validitaFineImpostata As Boolean = False
            If dtMovimentazioni IsNot Nothing AndAlso dtMovimentazioni.Rows.Count > 0 Then
                Dim drMovimentazioni = dtMovimentazioni.Select(" ALLEV_ID_FISCALE = '" & IdFiscale_Allevamento & "' AND AZIENDA_CODICE = '" & Codice_Azienda & "' ", "DT_INGRESSO DESC")

                If drMovimentazioni.Length > 0 Then
                    If Not IsDBNull(drMovimentazioni(0)("MOTIVO_INGRESSO")) AndAlso CStr(drMovimentazioni(0)("MOTIVO_INGRESSO")) <> "" Then
                        Motivo_Ingresso = CStr(drMovimentazioni(0)("MOTIVO_INGRESSO"))
                        Id_Ingresso = drMovimentazioni(0)("REGSTA_ID")
                    End If
                    If Not IsDBNull(drMovimentazioni(0)("MOTIVO_USCITA")) AndAlso CStr(drMovimentazioni(0)("MOTIVO_USCITA")) <> "" Then
                        Motivo_Uscita = CStr(drMovimentazioni(0)("MOTIVO_USCITA"))
                        Id_Uscita = drMovimentazioni(0)("REGSTA_ID")
                    End If
                    If Not IsDBNull(drMovimentazioni(0)("USCITA_MM_ID")) Then
                        codiceModello4_Uscita = CStr(drMovimentazioni(0)("USCITA_MM_ID"))
                        Try
                            Dim dtModello = wsInterrogazioniModello4.getPrenotazioneModello(codiceModello4_Uscita, SpeCodice, True)
                            If dtModello IsNot Nothing AndAlso dtModello.Tables.Count > 0 AndAlso dtModello.Tables(0).Rows.Count > 0 Then
                                Uscita_Modello4_Prenotazione = dtModello.Tables(0).Rows(0).Item("PRENOTAZIONE_ID")
                                Uscita_Modello4_Numero = dtModello.Tables(0).Rows(0).Item("NUM_MODELLO")
                            End If
                        Catch ex As Exception

                        End Try
                    End If

                    If dtMovimentazioni.Columns.Contains("DT_INGRESSO") Then
                        If Not IsDBNull(drMovimentazioni(0)("DT_INGRESSO")) Then
                            obj_CapoAnimale.validita.inizio = CDate(drMovimentazioni(0)("DT_INGRESSO"))
                            validitaInizioImpostata = True
                        End If
                    End If

                    If dtMovimentazioni.Columns.Contains("DT_USCITA") Then
                        If Not IsDBNull(drMovimentazioni(0)("DT_USCITA")) Then
                            obj_CapoAnimale.validita.fine = CDate(drMovimentazioni(0)("DT_USCITA"))
                            validitaFineImpostata = True
                        End If
                    End If

                    'If Not IsDBNull(drMovimentazioni(0)("INGRESSO_MM_ID")) Then
                    '    codiceModello4_Ingresso = CStr(drMovimentazioni(0)("INGRESSO_MM_ID"))
                    '    'Dim dtRegistro = wsRegistroStalla.getRegistriStalla(Codice_Capo, Codice_Azienda, IdFiscale_Allevamento, SpeCodice, CDate(drMovimentazioni(0)("DT_INGRESSO")))
                    '    Dim dtModello = wsInterrogazioniModello4.getPrenotazioneModello(codiceModello4_Ingresso, SpeCodice)
                    '    If dtModello IsNot Nothing AndAlso dtModello.Tables.Count > 0 AndAlso dtModello.Tables(0).Rows.Count > 0 Then
                    '        Ingresso_Modello4_Prenotazione = dtModello.Tables(0).Rows(0).Item("PRENOTAZIONE_ID")
                    '        Ingresso_Modello4_Numero = dtModello.Tables(0).Rows(0).Item("NUM_MODELLO")
                    '    End If
                    'End If
                    Dim dr_nascita
                    dr_nascita = dtMovimentazioni.Select(" MOTIVO_INGRESSO = 'N' ")
                    If dr_nascita.Length > 0 Then
                        Azienda_Nascita = dr_nascita(0)("AZIENDA_CODICE")
                        'creaModificaContattoAllevatore(Azienda_Nascita, SpeCodice)
                    End If

                    dr_nascita = dtMovimentazioni.Select(" MOTIVO_INGRESSO = 'W' ")
                    If dr_nascita.Length > 0 Then
                        Azienda_Nascita = dr_nascita(0)("AZIENDA_CODICE")
                        'creaModificaContattoAllevatore(Azienda_Nascita, SpeCodice)
                    End If

                End If
            End If

            Dim Num_Certificato = ""
            If dtAnimale.Columns.Contains("NUM_CERTIFICATO") AndAlso Not IsDBNull(dtAnimale.Rows(0)("NUM_CERTIFICATO")) AndAlso dtAnimale.Rows(0)("NUM_CERTIFICATO") <> "" Then
                Num_Certificato = dtAnimale.Rows(0)("NUM_CERTIFICATO")
            End If

            If Num_Certificato = "" AndAlso dtAnimale.Columns.Contains("NUMERO_RIF_LOCALE") AndAlso Not IsDBNull(dtAnimale.Rows(0)("NUMERO_RIF_LOCALE")) AndAlso dtAnimale.Rows(0)("NUMERO_RIF_LOCALE") <> "" Then
                Num_Certificato = dtAnimale.Rows(0)("NUMERO_RIF_LOCALE")
            End If


            obj_CapoAnimale.nome = ""
            obj_CapoAnimale.collare = ""
            obj_CapoAnimale.sesso = dtAnimale.Rows(0)("SESSO")

            obj_CapoAnimale.idCapo_BDN = dtAnimale.Rows(0)("CAPO_ID")
            obj_CapoAnimale.numCertificato = Num_Certificato
            obj_CapoAnimale.lottoFornitore = ""
            obj_CapoAnimale.dataNascita = dtAnimale.Rows(0)("DT_NASCITA")
            obj_CapoAnimale.validitaConversione = New IntervalloTemporale(AGRODATAINIZIO, AGRODATAFINE)
            obj_CapoAnimale.codiceFiscaleProprietario = IdFiscale_Allevamento
            obj_CapoAnimale.codiceFiscaleDetentore = IdFiscale_Detentore
            obj_CapoAnimale.codiceAziendaNascita = Azienda_Nascita

            'VALIDITA CAPO

            If Not validitaInizioImpostata Then
                If dtAnimale.Columns.Contains("DT_INGRESSO") Then
                    obj_CapoAnimale.validita.inizio = dtAnimale.Rows(0)("DT_INGRESSO")

                ElseIf dtAnimale.Columns.Contains("DATA_ESTRAZIONE") Then
                    obj_CapoAnimale.validita.inizio = dtAnimale.Rows(0)("DT_INIVAL")

                Else
                    obj_CapoAnimale.validita.inizio = AGRODATAINIZIO
                End If
            End If

            If Not validitaFineImpostata Then
                If CDate(dtAnimale.Rows(0)("DT_FINVAL")) < AGRODATAFINE Then
                    obj_CapoAnimale.validita.fine = dtAnimale.Rows(0)("DT_FINVAL")
                End If
            End If

            If Num_Certificato <> "" Then
                If dtAnimale.Columns.Contains("DT_INGRESSO") Then
                    obj_CapoAnimale.ingresso_modello4_data_prenotazione = dtAnimale.Rows(0)("DT_INGRESSO")

                ElseIf dtAnimale.Columns.Contains("DATA_ESTRAZIONE") Then
                    obj_CapoAnimale.ingresso_modello4_data_prenotazione = dtAnimale.Rows(0)("DT_INIVAL")

                Else
                    obj_CapoAnimale.ingresso_modello4_data_prenotazione = AGRODATAINIZIO
                End If
            Else
                Dim aaaa = 0
            End If



            'MADRE CAPO
            Dim MadreCapo_Matricola As String = dtAnimale.Rows(0)("COD_MADRE")

            Dim MadreCapo_Razza As New AgronicaCoreModelsSTD.metaschema.Razza
            If Not IsNothing(MadreCapo_Matricola) AndAlso MadreCapo_Matricola <> "" Then
                MadreCapo_Razza = Ricava_RazzaCapo(MadreCapo_Matricola)
            End If

            Dim modello4IngressoCapoDB As String = ""
            Dim modello4UscitaCapoDB As String = ""
            Dim rigaGiacenza = obj_ZooAnimali_R.Leggi_Giacenze(Piva, Sa_Cod, Sta_Num, 0, 0, Date.Now, objParametriServer, False, False, Nothing, True, False, False, False, " Zoo_Animali.Matricola ='" & Codice_Capo & "' ")
            If rigaGiacenza.Rows.Count > 0 Then
                obj_CapoAnimale.codice = CInt(rigaGiacenza(0)("Cod_Animale"))
                modello4IngressoCapoDB = CStr(rigaGiacenza(0)("Modello4_Ingresso_Numero"))
                modello4UscitaCapoDB = CStr(rigaGiacenza(0)("Modello4_Uscita_Numero"))
            End If

            If dtMovimentazioni IsNot Nothing AndAlso dtMovimentazioni.Rows.Count > 0 Then
                Dim drMovimentazioni = dtMovimentazioni.Select(" ALLEV_ID_FISCALE = '" & IdFiscale_Allevamento & "' AND AZIENDA_CODICE = '" & Codice_Azienda & "' ")

                If modello4IngressoCapoDB = "" AndAlso listMotiviIngressiModello4.Contains(Motivo_Ingresso) AndAlso
                            Not IsDBNull(drMovimentazioni(0)("DT_INGRESSO")) Then
                    Try
                        Dim dataFineRicerca As Date = CDate(drMovimentazioni(0)("DT_INGRESSO")).AddDays(2)
                        Dim dataInizioRicerca As Date = CDate(drMovimentazioni(0)("DT_INGRESSO")).AddDays(-10)
                        If Ingresso_Modello4_Numero = "" AndAlso Ingresso_Modello4_Prenotazione = "" Then
                            Dim dataMod As Date
                            RecuperaDatiModello4Ingresso(Ingresso_Modello4_Prenotazione, Ingresso_Modello4_Numero, dataMod, Codice_Azienda, IdFiscale_Allevamento, SpeCodice, Allev_Id, Codice_Asl, Codice_Capo, dataInizioRicerca, dataFineRicerca)

                            obj_CapoAnimale.ingresso_mm_id = codiceModello4_Ingresso
                            obj_CapoAnimale.ingresso_modello4_numero = Ingresso_Modello4_Numero
                            obj_CapoAnimale.ingresso_modello4_prenotazione = Ingresso_Modello4_Prenotazione
                            obj_CapoAnimale.ingresso_modello4_data_prenotazione = dataMod
                        End If
                    Catch ex As Exception

                    End Try
                End If

                If modello4UscitaCapoDB = "" AndAlso listMotiviUscitaModello4.Contains(Motivo_Uscita) AndAlso
                            Not IsDBNull(drMovimentazioni(0)("DT_USCITA")) Then
                    Dim dataFineRicerca As Date = CDate(drMovimentazioni(0)("DT_USCITA")).AddDays(2)
                    Dim dataInizioRicerca As Date = CDate(drMovimentazioni(0)("DT_USCITA")).AddDays(-10)
                    Try
                        RecuperaDatiModello4Uscita(Uscita_Modello4_Prenotazione, Uscita_Modello4_Numero, Uscita_Modello4_Data, Codice_Azienda, IdFiscale_Allevamento, SpeCodice, Allev_Id, Codice_Asl, Codice_Capo, dataInizioRicerca, dataFineRicerca)
                        obj_CapoAnimale.uscita_mm_id = codiceModello4_Uscita
                        obj_CapoAnimale.uscita_modello4_numero = Uscita_Modello4_Numero
                        obj_CapoAnimale.uscita_modello4_prenotazione = Uscita_Modello4_Prenotazione
                        obj_CapoAnimale.uscita_modello4_data_prenotazione = Uscita_Modello4_Data
                    Catch ex As Exception

                    End Try
                End If
            End If

            obj_CapoAnimale.madre = New CapoAnimale
            obj_CapoAnimale.madre.matricola = MadreCapo_Matricola
            obj_CapoAnimale.madre.codice = 0
            obj_CapoAnimale.madre.razza = MadreCapo_Razza

            'PADRE CAPO
            'Dim MatricolaCapo_Padre As String
            'Dim PadreCapo_Razza As New AgronicaCoreModelsSTD.metaschema.Razza
            'If Not IsNothing(MadreCapo_Matricola) AndAlso MadreCapo_Matricola <> "" Then
            '    PadreCapo_Razza = Ricava_RazzaCapo(MatricolaCapo_Padre)
            'End If
            obj_CapoAnimale.padre = New CapoAnimale
            obj_CapoAnimale.padre.matricola = ""
            obj_CapoAnimale.padre.codice = 0
            obj_CapoAnimale.padre.razza = New AgronicaCoreModelsSTD.metaschema.Razza(0, "")

            obj_CapoAnimale.fornitore = New Contatto
            obj_CapoAnimale.fornitore.primaryKey = New Contatto.PK("", "")

            obj_CapoAnimale.esercizi = New List(Of EsercizioCapoAnimale)
            obj_CapoAnimale.statiAccrescimento = New List(Of StatoAccrescimento)




            Dim Spe_Id As Integer = dtAnimale.Rows(0)("SPE_ID")
            Dim Razza_Id As Integer = dtAnimale.Rows(0)("RAZZA_ID")

            RicavaCodici_CapoAnimale(obj_CapoAnimale,
                                     Spe_Id.ToString, Razza_Id.ToString)

            'If Not dtAnimale.Columns.Contains("DATA_ESTRAZIONE") Then

            Crea_Distinte(obj_CapoAnimale)

            Crea_StatiAccrescimento(obj_CapoAnimale)

            'End If

            'wsRegistroStalla.getRegistriStalla(p_capo_codice, p_azienda_codice, p_allev_idfiscale, p_spe_codice, "")

            obj_CapoAnimaleCDC.capoAnimale = obj_CapoAnimale

            'Dim dtGetCapiAllevamento = wsAnagraficaCapo.Get_Capi_Allevamento(allev_id, "01012022", "03102022")
            'wsRegistroStalla.getRegistriStalla(p_capo_codice, p_azienda_codice, p_allev_idfiscale, p_spe_codice, "")

            'Dim dtGetCapiAllevamento = wsAnagraficaCapo.Get_Capi_Allevamento(allev_id, "01012022", "03102022")
            'wsRegistroStalla.getRegistriStalla(p_capo_codice, p_azienda_codice, p_allev_idfiscale, p_spe_codice, "")

            Dim A = 0

        Catch ex As GiasException
            objLog.Scrivi_LOG(objParametriServer,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "[" & NomeRoutine & "] GiasException: " & ex.Message,
                              CustomLOGParams:=customLOGParams)
            Throw New Exception("[" & NomeRoutine & "] GiasException: " & ex.Message)
        Catch ex As Exception
            objLog.Scrivi_LOG(objParametriServer,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "[" & NomeRoutine & "] : " & ex.Message,
                              CustomLOGParams:=customLOGParams)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return obj_CapoAnimaleCDC

    End Function


    Public Function RecuperaAziendaNascita(ByVal Cod_Animale As Integer,
                                           ByVal SpeCodice As String,
                                           ByVal Codice_Capo As String) As String

        Dim NomeRoutine As String = "RecuperaAziendaNascita"
        Dim azienda_trovata As Boolean = False
        Try

            Dim dtAnimale = wsAnagraficaCapo.getCapo(Codice_Capo)
            If IsNothing(dtAnimale) OrElse dtAnimale.Rows.Count = 0 Then
                objLog.Scrivi_LOG(objParametriServer,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "Chiamata getCapo " & Codice_Capo & " in errore",
                              CustomLOGParams:=customLOGParams)
                Throw New Exception("Chiamata getCapo " & Codice_Capo & " in errore")
            End If

            Dim dtMovimentazioni As DataTable
            Try
                dtMovimentazioni = wsRegistroStalla.getMovimentazioniCapo(Codice_Capo)
            Catch ex As Exception
                objLog.Scrivi_LOG(objParametriServer,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "Chiamata getMovimentazioniCapo " & Codice_Capo & " in errore",
                              CustomLOGParams:=customLOGParams)
                Throw New Exception("Chiamata getMovimentazioniCapo " & Codice_Capo & " in errore")
            End Try
            Dim Azienda_Nascita = ""
            If dtMovimentazioni IsNot Nothing AndAlso dtMovimentazioni.Rows.Count > 0 Then

                Dim dr_nascita
                dr_nascita = dtMovimentazioni.Select(" MOTIVO_INGRESSO = 'N' ")
                If dr_nascita.Length > 0 Then
                    Azienda_Nascita = dr_nascita(0)("AZIENDA_CODICE")
                    'creaModificaContattoAllevatore(Azienda_Nascita, SpeCodice)
                    azienda_trovata = True
                End If

                dr_nascita = dtMovimentazioni.Select(" MOTIVO_INGRESSO = 'W' ")
                If dr_nascita.Length > 0 Then
                    Azienda_Nascita = dr_nascita(0)("AZIENDA_CODICE")
                    'creaModificaContattoAllevatore(Azienda_Nascita, SpeCodice)
                    azienda_trovata = True
                End If

                If azienda_trovata Then
                    Return Azienda_Nascita
                Else
                    Return ""
                End If

            End If

        Catch ex As GiasException
            azienda_trovata = False
            objLog.Scrivi_LOG(objParametriServer,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "[" & NomeRoutine & "] GiasException: " & ex.Message)
            Throw New Exception("[" & NomeRoutine & "] GiasException: " & ex.Message)
        Catch ex As Exception
            azienda_trovata = False
            objLog.Scrivi_LOG(objParametriServer,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "[" & NomeRoutine & "] : " & ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return azienda_trovata
    End Function


    Private Sub creaModificaContattoAllevatore(Codice_Azienda As String, SpeCod As String)
        Try
            If Codice_Azienda = "" Then
                Exit Sub
            End If

            Dim risorse_Umane As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R
            Dim dtRisorsaAllevatore = risorse_Umane.Leggi4(objParametriServer.PivaSuperUser, "", 0, enum_Rapporti_Contabili_Standard.Allevatore, "", False, False, False, False, False, False, False, "", "", objParametriServer, Codice_Azienda)
            If dtRisorsaAllevatore.Rows.Count > 0 Then
                Exit Sub
            End If
            If dtRisorsaAllevatore.Rows.Count = 0 Then
                Dim dtAllevamenti As New DataTable
                dtAllevamenti = wsAziende.FindAllevamento(Codice_Azienda, "", "")
                Dim dtAllevamentiFiltered As DataTable
                If Not dtAllevamenti.Columns.Contains("DT_FINE_ATTIVITA") Then
                    Dim _drAllevamento = dtAllevamenti.Select(" SPE_CODICE = '" + SpeCod + "' ")
                    If _drAllevamento.Count > 0 Then
                        dtAllevamentiFiltered = _drAllevamento.CopyToDataTable
                    End If
                Else
                    Dim _drAllevamento = dtAllevamenti.Select(" SPE_CODICE = '" + SpeCod + "' AND ( DT_FINE_ATTIVITA = '' OR DT_FINE_ATTIVITA IS NULL ) ")
                    If _drAllevamento.Count > 0 Then
                        dtAllevamentiFiltered = _drAllevamento.CopyToDataTable
                    End If
                End If

                If dtAllevamentiFiltered Is Nothing Then
                    Exit Sub
                End If

                If dtAllevamentiFiltered.Rows.Count = 0 Then
                    Exit Sub
                End If
                dtAllevamenti = dtAllevamentiFiltered

                Dim ragSoc As String = dtAllevamenti(0)("DENOMINAZIONE")
                Dim CodFiscale As String = dtAllevamenti(0)("ID_FISCALE")
                Dim indirizzo = ""

                If dtAllevamenti.Columns.Contains("INDIRIZZO") Then
                    If Not IsDBNull(dtAllevamenti(0)("INDIRIZZO")) Then
                        indirizzo = dtAllevamenti(0)("INDIRIZZO")
                    End If
                End If

                Dim cap = ""
                If dtAllevamenti.Columns.Contains("CAP") Then
                    If Not IsDBNull(dtAllevamenti(0)("CAP")) Then
                        cap = dtAllevamenti(0)("CAP")
                    End If
                End If

                Dim telefono = ""
                If dtAllevamenti.Columns.Contains("TELEFONO") Then
                    If Not IsDBNull(dtAllevamenti(0)("TELEFONO")) Then
                        telefono = CStr(dtAllevamenti(0)("TELEFONO"))
                    End If
                End If

                Dim mail = ""
                If dtAllevamenti.Columns.Contains("EMAIL") Then
                    If Not IsDBNull(dtAllevamenti(0)("EMAIL")) Then
                        mail = CStr(dtAllevamenti(0)("EMAIL"))
                    End If

                End If

                Dim com_cod_istat = "000"
                If dtAllevamenti.Columns.Contains("COM_CODICE") Then
                    If Not IsDBNull(dtAllevamenti(0)("COM_CODICE")) Then
                        com_cod_istat = dtAllevamenti(0)("COM_CODICE")
                    End If
                End If


                Dim pro_cod_istat = "000"
                If dtAllevamenti.Columns.Contains("PRO_CODICE") Then
                    If Not IsDBNull(dtAllevamenti(0)("PRO_CODICE")) Then
                        Dim prov_sigla = dtAllevamenti(0)("PRO_CODICE")
                        Dim lista_provincie As New AgronicaCoreMetaSchemaDAL.Lista_Province_R
                        Dim dtProv = lista_provincie.Leggi(prov_sigla, "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametriServer)
                        If dtProv.Rows.Count = 0 Then
                            Exit Sub
                        End If
                        pro_cod_istat = dtProv(0)("PROV")
                    End If
                End If

                Dim istat As New AgronicaCoreMetaSchemaDAL.Istat_R
                Dim localita = ""
                Dim Provincia = ""
                If (pro_cod_istat <> "000" AndAlso com_cod_istat <> "000") Then
                    Dim dtIstat = istat.Leggi(pro_cod_istat, com_cod_istat, "", "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametriServer)
                    If dtIstat.Rows.Count = 0 Then
                        Exit Sub
                    End If
                    localita = dtIstat(0)("LOCALITA")
                    Provincia = dtIstat(0)("Comuni_Prov")
                End If



                Dim pivasuperuser = objParametriServer.PivaSuperUser
                Dim contatto = (From c In GiasContext.Contatti
                                Where c.Piva = pivasuperuser AndAlso
                                    c.Codice_Fiscale = CodFiscale).FirstOrDefault
                If contatto IsNot Nothing Then
                    Dim risorsa_umana = (From r In GiasContext.Risorse_Umane
                                         Where contatto.Piva = r.Piva AndAlso
                                             contatto.Cod_Contatto = r.Cod_Contatto AndAlso
                                             r.Cod_Rapporto = enum_Rapporti_Contabili_Standard.Allevatore).FirstOrDefault
                    If risorsa_umana IsNot Nothing Then
                        risorsa_umana.Attivita_Des = Codice_Azienda
                        GiasContext.Entry(risorsa_umana).State = Entity.EntityState.Modified
                        GiasContext.SaveChanges()
                    Else
                        Dim new_risorsa_umana As New Risorse_Umane
                        new_risorsa_umana.Piva = objParametriServer.PivaSuperUser
                        new_risorsa_umana.Cod_Contatto = contatto.Cod_Contatto
                        new_risorsa_umana.Cod_Rapporto = enum_Rapporti_Contabili_Standard.Allevatore
                        new_risorsa_umana.Attivita_Des = Codice_Azienda
                        GiasContext.Risorse_Umane.Add(new_risorsa_umana)
                        GiasContext.SaveChanges()
                    End If
                    Exit Sub
                End If

                Dim RisorseUmaneEF As AgronicaCoreEntityFramework_POCO.Risorse_Umane
                Dim ContattiEF As AgronicaCoreEntityFramework_POCO.Contatti
                Dim id_CF As Integer

                id_CF = 1
                Dim Cod_Contatto As String

                If CodFiscale.Length = 11 Then
                    Cod_Contatto = CodFiscale
                Else
                    Dim sequenza_tabelle As New AgronicaCoreDataProvider.Agro_Sequenze
                    Cod_Contatto = sequenza_tabelle.NuovoId_Tabella("contatti", 0, Integer.MaxValue, objParametriServer)
                    Cod_Contatto = Cod_Contatto.Replace("-", "F")
                    id_CF = 0
                End If


                Dim impresa = New AgronicaCoreEntityFramework_POCO.Imprese
                impresa.PIVA = objParametriServer.PivaSuperUser
                ContattiEF = AgronicaCoreAnagrafeDAL.EFImprese.CreaContattoImpresa(GiasContext,
                                                                      objParametriServer,
                                                                      impresa,
                                                                      Cod_Contatto,
                                                                      -1,
                                                                      id_CF,
                                                                      objParametriUtenti.UsernameOperazione
                                                                      )
                ContattiEF.Rag_Soc = ragSoc
                ContattiEF.Codice_Fiscale = CodFiscale

                RisorseUmaneEF = AgronicaCoreAnagrafeDAL.EFImprese.CreaRisorseUmaneImpresa(GiasContext,
                                                                       objParametriServer,
                                                                       impresa,
                                                                       ContattiEF,
                                                                       objParametriServer.UsernameOperazione
                                                                       )
                RisorseUmaneEF.Cod_Rapporto = enum_Rapporti_Contabili_Standard.Allevatore
                RisorseUmaneEF.Attivita_Des = Codice_Azienda

                If telefono <> "" Then
                    Dim rubrica1 = AgronicaCoreAnagrafeDAL.EFContatti.CreateRubricaContatto(GiasContext,
                                                                         objParametriServer,
                                                                         ContattiEF.Piva, ContattiEF.Sa_Cod, ContattiEF, telefono, "Telefono",
                                                                         objParametriUtenti.UsernameOperazione)
                End If

                If mail <> "" Then
                    Dim rubrica1 = AgronicaCoreAnagrafeDAL.EFContatti.CreateRubricaContatto(GiasContext,
                                                                         objParametriServer,
                                                                         ContattiEF.Piva, ContattiEF.Sa_Cod, ContattiEF, mail, "Email",
                                                                         objParametriUtenti.UsernameOperazione)
                End If

                Dim dati_indirizzo As New AgronicaCoreModelsSTD.anagrafiche.Indirizzo
                dati_indirizzo.cap = cap
                dati_indirizzo.codice = 0
                dati_indirizzo.via = indirizzo
                dati_indirizzo.stato = New CodiciNazioniISO3166 With {.codice = "IT", .descrizione = "ITALIA"}
                dati_indirizzo.istatComune = New AgronicaCoreModelsSTD.metaschema.Istat With {
                    .com = com_cod_istat,
                    .prov = pro_cod_istat,
                    .localita = localita,
                    .comuni_prov = Provincia
                }


                Dim indirizzoEF = AgronicaCoreAnagrafeDAL.EFContatti.CreateIndirizzoContatto(dati_indirizzo,
                                                                                             GiasContext,
                                                                                             objParametriServer,
                                                                                             ContattiEF.Piva,
                                                                                             ContattiEF.Sa_Cod,
                                                                                             ContattiEF,
                                                                                             1,
                                                                                             objParametriUtenti.UsernameOperazione)

                GiasContext.SaveChanges()
            End If

        Catch ex As Exception
            objLog.Scrivi_LOG(objParametriServer, System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore in creaModificaContattoAllevatore: " & Codice_Azienda & " " & ex.Message,
                          CustomLOGParams:=customLOGParams)
        End Try

    End Sub

    Private Sub RecuperaDatiModello4Ingresso(ByRef Ingresso_Modello4_Prenotazione As String,
                                             ByRef Ingresso_Modello4_Numero As String,
                                             ByRef Ingresso_Modello4_Data As Date,
                                             ByVal Codice_Azienda As String,
                                             ByVal IdFiscale_Allevamento As String,
                                             ByVal SpeCodice As String,
                                             ByVal Allev_Id As String,
                                             ByVal Codice_Asl As String,
                                             ByVal Codice_Capo As String,
                                             ByVal DataDa As Date,
                                             ByVal DataA As Date
                                             )
        If DataA = AGRODATAFINE Then
            DataA = Date.Now
        End If

        If DataDa = AGRODATAINIZIO Then
            DataDa = DataA.AddDays(-10)
        End If

        If DataA > Date.Now Then
            DataA = Date.Now
        End If

        While True

            'Con codice asl
            Dim dtModelli4_Ingresso = wsInterrogazioniModello4.getListaPrenotazioniModelli(p_data_da:=DataDa,
                                                                                           p_data_a:=DataA,
                                                                                           p_asl_codice_prov:="",
                                                                                           p_azienda_codice_prov:="",
                                                                                           p_tipo_dest:="AL",
                                                                                           p_stato_modello:="C",
                                                                                           p_asl_codice_dest:=Codice_Asl,
                                                                                           p_codice_struttura_dest:=Codice_Azienda,
                                                                                           p_regione_codice_dest:="",
                                                                                           True)

            If dtModelli4_Ingresso IsNot Nothing Then
                Try
                    Dim modelli4 = Carica_Modelli4(dtModelli4_Ingresso, IdFiscale_Allevamento, SpeCodice)

                    For Each modello4 In modelli4
                        If modello4.listaMatricoleCapi.Contains(Codice_Capo) Then
                            Ingresso_Modello4_Prenotazione = modello4.prenotazioneId
                            Ingresso_Modello4_Numero = modello4.numModello
                            Ingresso_Modello4_Data = modello4.dataDocumento
                            Exit While
                        End If
                    Next

                Catch ex As Exception
                    Dim modelloStr = JsonConvert.SerializeObject(dtModelli4_Ingresso)
                    objLog.Scrivi_LOG(objParametriServer,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore caricamento modello 4 ingresso " & ex.Message & ": " & modelloStr,
                          CustomLOGParams:=customLOGParams)
                End Try

            End If

            DataA = DataDa
            DataDa = DataA.AddDays(-10)

            If DataDa < Date.Now.AddDays(-30) Then
                Exit While
            End If

        End While

    End Sub

    Private Sub RecuperaDatiModello4Uscita(ByRef Uscita_Modello4_Prenotazione As String,
                                           ByRef Uscita_Modello4_Numero As String,
                                           ByRef Uscita_Modello4_Data As Date,
                                           ByVal Codice_Azienda As String,
                                           ByVal IdFiscale_Allevamento As String,
                                           ByVal SpeCodice As String,
                                           ByVal Allev_Id As String,
                                           ByVal Codice_Asl As String,
                                           ByVal Codice_Capo As String,
                                           ByVal DataDa As Date,
                                           ByVal DataA As Date)
        If DataA = AGRODATAFINE Then
            DataA = Date.Now
        End If

        If DataDa = AGRODATAINIZIO Then
            DataDa = DataA.AddDays(-10)
        End If

        If DataA > Date.Now Then
            DataA = Date.Now
        End If

        'controlla i capi in uscita con Modello4 nell'arco di un mese
        While True

            'Con codice asl
            Dim dtModelli4_Uscita = wsInterrogazioniModello4.getListaPrenotazioniModelli(p_data_da:=DataDa,
                                                                                         p_data_a:=DataA,
                                                                                         p_asl_codice_prov:=Codice_Asl,
                                                                                         p_azienda_codice_prov:=Codice_Azienda,
                                                                                         p_tipo_dest:="",
                                                                                         p_stato_modello:="C",
                                                                                         p_asl_codice_dest:="",
                                                                                         p_codice_struttura_dest:="",
                                                                                         p_regione_codice_dest:="",
                                                                                         True)

            If dtModelli4_Uscita IsNot Nothing Then
                Try
                    Dim modelli4 = Carica_Modelli4(dtModelli4_Uscita, Allev_Id, SpeCodice)
                    For Each modello4 In modelli4
                        If modello4.listaMatricoleCapi.Contains(Codice_Capo) Then
                            Uscita_Modello4_Prenotazione = modello4.prenotazioneId
                            Uscita_Modello4_Numero = modello4.numModello
                            Uscita_Modello4_Data = modello4.dataDocumento
                            Exit While
                        End If
                    Next

                Catch ex As ExpiredTokenBDNException
                    Throw ex
                Catch ex As Exception
                    Dim modelloStr = JsonConvert.SerializeObject(dtModelli4_Uscita)
                    objLog.Scrivi_LOG(objParametriServer,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore caricamento modello 4 uscita " & ex.Message & ": " & modelloStr,
                          CustomLOGParams:=customLOGParams)
                End Try

            End If

            DataA = DataDa
            DataDa = DataA.AddDays(-10)

            If DataDa < Date.Now.AddDays(-30) Then
                Exit While
            End If

        End While
    End Sub




    Private Function Carica_Modelli4(ByVal dtModelli As DataTable,
                                    ByVal Allev_IdFiscale As String,
                                    ByVal GrSpe_Cod As String) As List(Of CaricaModelli4_Response)
        Dim objResp_CaricaModelli As New List(Of CaricaModelli4_Response)

        Try

            If dtModelli IsNot Nothing AndAlso dtModelli.Rows.Count > 0 Then
                'filtra i modelli per Allevamento non potendo farlo direttamente sulla chiamata 
                Dim listaModelliP = dtModelli.Select()

                For Each rowModello In listaModelliP
                    Dim objModelli4 As New CaricaModelli4_Response
                    objModelli4.listaMatricoleCapi = New List(Of String)

                    Dim dsPrenotazioneModello = wsInterrogazioniModello4.getPrenotazioneModello(rowModello("PRENOTAZIONE_ID"),
                                                                                                GrSpe_Cod, True)

                    Dim dtModello4 As DataTable = dsPrenotazioneModello.Tables(0)
                    objModelli4.prenotazioneId = If(dtModello4.Columns.Contains("PRENOTAZIONE_ID"), dtModello4.Rows(0).Item("PRENOTAZIONE_ID"), "")
                    objModelli4.documentoId = If(dtModello4.Columns.Contains("DOCUMENTO_ID"), dtModello4.Rows(0).Item("DOCUMENTO_ID"), "")
                    objModelli4.numModello = If(dtModello4.Columns.Contains("NUM_MODELLO"), dtModello4.Rows(0).Item("NUM_MODELLO"), "")

                    If dtModello4.Columns.Contains("GIORNI_VALIDITA") Then
                        objModelli4.giorniValidita = dtModello4.Rows(0).Item("GIORNI_VALIDITA")
                    Else
                        objModelli4.giorniValidita = ""
                    End If

                    objModelli4.tipoDestinazione = If(dtModello4.Columns.Contains("TIPOLOGIA_DEST"), dtModello4.Rows(0).Item("TIPOLOGIA_DEST"), "")
                    If dtModello4.Columns.Contains("PASCOLO_CODICE") Then
                        objModelli4.codAzienda_Prov = dtModello4.Rows(0).Item("PASCOLO_CODICE")
                    Else
                        objModelli4.codAzienda_Prov = ""
                    End If

                    objModelli4.oraPartenza = If(dtModello4.Columns.Contains("ORA_PARTENZA"), dtModello4.Rows(0).Item("ORA_PARTENZA"), "")
                    If objModelli4.tipoDestinazione = "MACELLO" Then
                        objModelli4.codAzienda_Dest = If(dtModello4.Columns.Contains("MACELLO_CODICE"), dtModello4.Rows(0).Item("MACELLO_CODICE"), "")
                    Else
                        objModelli4.codAzienda_Dest = If(dtModello4.Columns.Contains("DEST_AZIENDA_CODICE"), dtModello4.Rows(0).Item("DEST_AZIENDA_CODICE"), "")
                        objModelli4.idFiscaleAzienda_Dest = If(dtModello4.Columns.Contains("DEST_ALLEV_ID_FISCALE"), dtModello4.Rows(0).Item("DEST_ALLEV_ID_FISCALE"), "")
                    End If

                    If dtModello4.Columns.Contains("COD_ASL_TRASP") AndAlso Not IsNothing(dtModello4.Rows(0).Item("COD_ASL_TRASP")) AndAlso dtModello4.Rows(0).Item("COD_ASL_TRASP") = "" Then
                        objModelli4.codAsl_Trasp = dtModello4.Rows(0).Item("COD_ASL_TRASP")
                    End If

                    objModelli4.durataViaggio = If(dtModello4.Columns.Contains("DURATA_VIAGGIO"), dtModello4.Rows(0).Item("DURATA_VIAGGIO"), "")

                    If dtModello4.Columns.Contains("NUM_AUTORIZZAZIONE") AndAlso Not IsNothing(dtModello4.Rows(0).Item("NUM_AUTORIZZAZIONE")) AndAlso dtModello4.Rows(0).Item("NUM_AUTORIZZAZIONE") = "" Then
                        objModelli4.numAutorizzazione = dtModello4.Rows(0).Item("NUM_AUTORIZZAZIONE")
                    End If

                    If dtModello4.Columns.Contains("TARGA_MOTRICE") AndAlso Not IsNothing(dtModello4.Rows(0).Item("TARGA_MOTRICE")) AndAlso dtModello4.Rows(0).Item("TARGA_MOTRICE") = "" Then
                        objModelli4.targa = dtModello4.Rows(0).Item("TARGA_MOTRICE")
                    End If
                    If dtModello4.Columns.Contains("TARGA_RIMORCHIO") AndAlso Not IsNothing(dtModello4.Rows(0).Item("TARGA_RIMORCHIO")) AndAlso dtModello4.Rows(0).Item("TARGA_RIMORCHIO") = "" Then
                        objModelli4.targa_rimorchio = dtModello4.Rows(0).Item("TARGA_RIMORCHIO")
                    End If

                    objModelli4.dataUscita = dtModello4.Rows(0).Item("DT_USCITA")
                    If dtModello4.Columns.Contains("DT_DOCUMENTO") AndAlso Not IsNothing(dtModello4.Rows(0).Item("DT_DOCUMENTO")) Then
                        objModelli4.dataDocumento = dtModello4.Rows(0).Item("DT_DOCUMENTO")
                    End If

                    objModelli4.dataPrenotazione = Nothing

                    Dim listaCapi_Modello4 = dsPrenotazioneModello.Tables(1).ToExpandoObject.ToList()
                    Dim listaMatricoleCapi_Modello4 = (From cp In listaCapi_Modello4
                                                       Select cp.Item("CAPO_CODICE")).ToList()

                    For Each mat In listaMatricoleCapi_Modello4
                        objModelli4.listaMatricoleCapi.Add(mat.ToString)
                    Next

                    objResp_CaricaModelli.Add(objModelli4)
                Next

            End If

        Catch ex As Exception

        End Try

        Return objResp_CaricaModelli

    End Function

    ''' <summary>
    ''' Crea i codici per il capo animale passato partendo dai codici di BDN
    ''' </summary>
    ''' <param name="obj_CapoAnimale"></param>
    ''' <param name="Spe_Id"></param>
    ''' <param name="Razza_Id"></param>
    Private Sub RicavaCodici_CapoAnimale(ByRef obj_CapoAnimale As CapoAnimale,
                                         ByVal Spe_Id As String,
                                         ByVal Razza_Id As String)
        Dim NomeRoutine = "RicavaCodici_CapoAnimale"

        Try
            'ZOO_SPECIE
            Dim rowCodifica_RazzeAnimali = getDTCodificaRazzaId(Spe_Id, Razza_Id)
            'ZOO_CATEGORIA
            'Dim obj_CategorieAnimali_R As New Codifica_BDN_CategorieAnimali
            'Dim dtCodifica_CategorieAnimali As DataTable = obj_CategorieAnimali_R.leggi(objParametriServer, )

            If IsNothing(rowCodifica_RazzeAnimali) OrElse rowCodifica_RazzeAnimali.Rows.Count = 0 Then
                Throw New GiasException("Errore! Non esiste una mappatura dell'animale con questi codici.")
            End If

            '------------------------------------------------------------------------------------------------------------------------------------------------------
            'GEN_COD - GEN_DES
            Dim Gen_Cod As Integer = rowCodifica_RazzeAnimali.Rows(0)("GEN_COD")
            Dim Genere = From zan In Lista_Generi_Animali
                         Where (zan.GEN_COD = Gen_Cod)
                         Select zan

            obj_CapoAnimale.genere = New AgronicaCoreModelsSTD.metaschema.Genere(Genere.First.GEN_COD, Genere.First.GEN_DES)
            Gen_Cod = Genere.First.GEN_COD

            '------------------------------------------------------------------------------------------------------------------------------------------------------
            'SPE_COD - SPE_DES
            Dim Spe_Cod As Integer = rowCodifica_RazzeAnimali.Rows(0)("SPE_COD")
            Dim Specie = From zan In Lista_Specie_Animali
                         Where (zan.GEN_COD = Gen_Cod And zan.SPE_COD = Spe_Cod)
                         Select zan

            obj_CapoAnimale.specie = New AgronicaCoreModelsSTD.metaschema.utilizzi.Specie(Specie.First.SPE_COD, Specie.First.SPE_DES)
            Spe_Cod = Specie.First.SPE_COD

            '------------------------------------------------------------------------------------------------------------------------------------------------------
            'RAZ_COD - RAZ_DES
            Dim Raz_Cod As Integer = rowCodifica_RazzeAnimali.Rows(0)("RAZ_COD")
            Dim Razza = From zan In Lista_Razze_Animali
                        Where (zan.GEN_COD = Gen_Cod And zan.SPE_COD = Spe_Cod And zan.RAZ_COD = Raz_Cod)
                        Select zan

            obj_CapoAnimale.razza = New AgronicaCoreModelsSTD.metaschema.Razza(Razza.First.RAZ_COD, Razza.First.RAZ_DES)
            Raz_Cod = Razza.First.RAZ_COD

            '------------------------------------------------------------------------------------------------------------------------------------------------------

            'per il momento indirizzoProd, categoria, tipologia e metodoProduzione sono settati come generico,
            'successivamente verranno implementati quando mappati
            obj_CapoAnimale.indirizzoProd = New AgronicaCoreModelsSTD.metaschema.IndirizzoProduttivo(0)
            obj_CapoAnimale.categoria = New AgronicaCoreModelsSTD.metaschema.Categoria(0)
            obj_CapoAnimale.tipologia = New AgronicaCoreModelsSTD.metaschema.TipologiaCapoAnimale(1)
            obj_CapoAnimale.metodoProduzione = New AgronicaCoreModelsSTD.metaschema.MetodoProduzione(1)
        Catch ex As Exception
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Sub

    Public Function getDTCodificaRazzaId(ByVal Spe_Id As String, ByVal Razza_Id As String) As DataTable
        Dim dr = Codifica_RazzeAnimali_DT.Select(" SPE_ID = '" & Spe_Id & "' AND RAZZA_ID = '" & Razza_Id & "' ")
        If dr.Length > 0 Then
            Return dr.CopyToDataTable()
        End If
        Return Nothing
    End Function

    Public Function getDTCodificaRazzaCodice(ByVal Spe_Id As String, ByVal RazzaCodice As String) As DataTable
        Dim dr = Codifica_RazzeAnimali_DT.Select(" CODICE = '" & RazzaCodice & "' ")
        If dr.Length > 0 Then
            Return dr.CopyToDataTable()
        End If
        Return Nothing
    End Function

    ''' <summary>
    ''' Crea le distinte per il singolo capo
    ''' </summary>
    ''' <param name="obj_CapoAnimale"></param>
    Private Sub Crea_Distinte(ByRef obj_CapoAnimale As CapoAnimale)
        Dim NomeRoutine = "Crea_Distinte"
        Try
            Dim obj_Esercizio As New AgronicaCoreModelsSTD.anagrafiche.EsercizioCapoAnimale
            obj_Esercizio.codice_capo_animale = obj_CapoAnimale.codice
            obj_Esercizio.validita = New IntervalloTemporale(obj_CapoAnimale.validita.inizio, obj_CapoAnimale.validita.fine)

            obj_Esercizio.progettoNome = ""

            obj_CapoAnimale.esercizi.Add(obj_Esercizio)

            'TODO
            'Dim Validita_Inizio As Date = obj_CapoAnimale.validita.inizio
            'Dim Validita_Fine As Date = obj_CapoAnimale.validita.fine

            'If (Validita_Fine = AGRODATAFINE) Then
            '    Validita_Fine = New Date(Validita_Inizio.Year, 12, 31)

            'End If

            'While Validita_Fine.Year <> Date.Now.Year + 1
            '    Dim obj_Esercizio As New AgronicaCoreModelsSTD.anagrafiche.EsercizioCapoAnimale
            '    obj_Esercizio.codice_capo_animale = obj_CapoAnimale.codice

            '    obj_Esercizio.validita.inizio = Validita_Inizio
            '    obj_Esercizio.validita.fine = Validita_Fine

            '    obj_CapoAnimale.esercizi.Add(obj_Esercizio)

            '    Validita_Inizio = New Date(Validita_Inizio.Year + 1, 1, 1)
            '    Validita_Fine = New Date(Validita_Fine.Year + 1, 12, 31)

            'End While

        Catch ex As Exception
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' Crea gli stati di accrescimento del capo animale passato (se ne esistono)
    ''' </summary>
    ''' <param name="obj_CapoAnimale"></param>
    Private Sub Crea_StatiAccrescimento(ByRef obj_CapoAnimale As CapoAnimale)
        Dim Piva As String = obj_CapoAnimale.partitaIva
        Dim Gen_Cod As Integer = obj_CapoAnimale.genere.codice
        Dim Spe_Cod As Integer = obj_CapoAnimale.specie.codice
        Dim Tipo_Cod As Integer = obj_CapoAnimale.tipologia.codice
        Dim NomeRoutine = "Crea_StatiAccrescimento"
        Try
            'Leggo gli stati di accrescimento da db
            Dim Stati_Accrescimento = From zan In Zoo_Animali_Lista_Stati_Accrescimento
                                      Where (zan.PIVA = Piva Or zan.Sa_Cod = -1) And zan.GEN_COD = Gen_Cod And zan.SPE_COD = Spe_Cod And zan.TIPO_COD = Tipo_Cod
                                      Select zan
                                      Order By zan.Giorno_Da

            If IsNothing(Stati_Accrescimento) OrElse Stati_Accrescimento.Count = 0 Then
                Throw New GiasException("Errore! Non esistono stati di accrescimento per questo capo.")

            End If

            For Each sa In Stati_Accrescimento
                Dim obj_StatoAccr As New StatoAccrescimento

                '-------------------------------------------------------------------------------------------------------
                'Aggiunge alla data di nascita i giorni di durata del periodo dello stato di accrescimento
                '-------------------------------------------------------------------------------------------------------

                obj_StatoAccr.validita = New IntervalloTemporale

                'VALIDITA_INIZIO
                If IsNothing(sa.Giorno_Da) Then
                    obj_StatoAccr.validita.inizio = obj_CapoAnimale.dataNascita
                Else
                    obj_StatoAccr.validita.inizio = obj_CapoAnimale.dataNascita.AddDays(sa.Giorno_Da)
                End If

                'VALIDITA_FINE
                If IsNothing(sa.Giorno_A) Then
                    obj_StatoAccr.validita.fine = AGRODATAFINE
                Else
                    obj_StatoAccr.validita.fine = obj_CapoAnimale.dataNascita.AddDays(sa.Giorno_A)
                End If

                '-------------------------------------------------------------------------------------------------------

                obj_StatoAccr.codice = sa.STATO_COD
                obj_StatoAccr.descrizione = sa.Stato_Des

                obj_CapoAnimale.statiAccrescimento.Add(obj_StatoAccr)
            Next
        Catch ex As Exception
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Ricava la razza (RAZ_COD - RAZ_DES) del capo tramite la matricola
    ''' </summary>
    ''' <param name="CapoMadre_Matricola"></param>
    ''' <returns></returns>
    Private Function Ricava_RazzaCapo(ByVal CapoMadre_Matricola As String) As AgronicaCoreModelsSTD.metaschema.Razza
        Dim dtCapo_Madre As DataTable
        Dim NomeRoutine = "Ricava_RazzaCapo"
        Try
            Try
                dtCapo_Madre = wsAnagraficaCapo.getCapo(CapoMadre_Matricola)
            Catch ex As Exception

            End Try
            Dim MadreCapo_Razza As New AgronicaCoreModelsSTD.metaschema.Razza
            MadreCapo_Razza.codice = 0
            If dtCapo_Madre IsNot Nothing Then
                Dim obj_Madre_CapoAnimale As New CapoAnimale

                Dim MadreCapo_Spe_Id As Integer = dtCapo_Madre.Rows(0)("SPE_ID")
                Dim MadreCapo_Razza_Id As Integer = dtCapo_Madre.Rows(0)("RAZZA_ID")

                Dim rowCodifica_RazzeAnimali As DataTable = Codifica_RazzeAnimali_DT.Select(" SPE_ID = '" & MadreCapo_Spe_Id &
                                                                                            "' AND RAZZA_ID = '" & MadreCapo_Razza_Id & "' ").CopyToDataTable

                If IsNothing(rowCodifica_RazzeAnimali) OrElse rowCodifica_RazzeAnimali.Rows.Count = 0 Then
                    Throw New GiasException("Errore! Non esiste una mappatura dell'animale con questi codici.")
                End If

                MadreCapo_Razza.codice = rowCodifica_RazzeAnimali.Rows(0)("RAZ_COD")
            End If
            Return MadreCapo_Razza
        Catch ex As Exception
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try


    End Function


    Public Function trovaID_Uscita(IdFiscale_Allevamento As String,
                                    Codice_Azienda As String,
                                    Codice_Capo As String) As Integer
        Dim Id_Uscita As Integer = 0
        Dim dtMovimentazioni
        Try
            dtMovimentazioni = wsRegistroStalla.getMovimentazioniCapo(Codice_Capo)
        Catch ex As Exception
            objLog.Scrivi_LOG(objParametriServer,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Chiamata getMovimentazioniCapo " & Codice_Capo & " in errore",
                          CustomLOGParams:=customLOGParams)
            'Throw New Exception("Chiamata getMovimentazioniCapo " & Codice_Capo & " in errore")
        End Try

        If dtMovimentazioni IsNot Nothing AndAlso dtMovimentazioni.Rows.Count > 0 Then
            Dim drMovimentazioni = dtMovimentazioni.Select(" ALLEV_ID_FISCALE = '" & IdFiscale_Allevamento & "' AND AZIENDA_CODICE = '" & Codice_Azienda & "' ")

            If drMovimentazioni.Length > 0 Then
                If Not IsDBNull(drMovimentazioni(0)("MOTIVO_USCITA")) AndAlso CStr(drMovimentazioni(0)("MOTIVO_USCITA")) <> "" Then
                    Id_Uscita = drMovimentazioni(0)("REGSTA_ID")
                End If
            End If
        End If
        Return Id_Uscita
    End Function


    Public Function trovaID_Ingresso(IdFiscale_Allevamento As String,
                                    Codice_Azienda As String,
                                    Codice_Capo As String) As Integer
        Dim Id_Ingresso As Integer = 0
        Dim dtMovimentazioni
        Try
            dtMovimentazioni = wsRegistroStalla.getMovimentazioniCapo(Codice_Capo)
        Catch ex As TokenBDNException
            Throw ex
        Catch ex As Exception
            objLog.Scrivi_LOG(objParametriServer,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Chiamata getMovimentazioniCapo " & Codice_Capo & " in errore",
                          CustomLOGParams:=customLOGParams)
            'Throw New Exception("Chiamata getMovimentazioniCapo " & Codice_Capo & " in errore")
        End Try

        If dtMovimentazioni IsNot Nothing AndAlso dtMovimentazioni.Rows.Count > 0 Then
            Dim drMovimentazioni = dtMovimentazioni.Select(" ALLEV_ID_FISCALE = '" & IdFiscale_Allevamento & "' AND AZIENDA_CODICE = '" & Codice_Azienda & "' ")

            If drMovimentazioni.Length > 0 Then
                If Not IsDBNull(drMovimentazioni(0)("MOTIVO_INGRESSO")) AndAlso CStr(drMovimentazioni(0)("MOTIVO_INGRESSO")) <> "" Then
                    Id_Ingresso = drMovimentazioni(0)("REGSTA_ID")
                End If
            End If
        End If
        Return Id_Ingresso
    End Function

End Class

