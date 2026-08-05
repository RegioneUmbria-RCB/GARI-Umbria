Imports System.Data.Entity
Imports System.IO
Imports System.Reflection
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreMetaSchemaDAL
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreModelsSTD.exceptions
Imports AgronicaCoreUtility
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class SincroBDNGestioneCapi
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
    Dim wsChiamawsRegistriDiStalla As ChiamawsRegistriDiStalla
    Dim wsChiamawsRegistroCapiStalla As ChiamaWs_RegistroCapiStalla
    Dim GiasContext As Gias_DeveloperServer_Entities
    Dim wsSincronizzazioneAllevamento As ImportazioneBDN.SincroBDNAllevamento
    Dim wsInterrogazioniModello4 As wsInterrogazioneModello4
    Dim wsGestioneModello4 As wsGestioneModello4
    Dim sincronizzatoreAnimale As SincroBDNAnimale

    Dim AgroWebConfig As AgronicaCoreGestioneRichieste.AgroWebConfig

    Dim objLog As New AgronicaCoreDataProvider.LogProvider

    Dim logDirectory As String
    Dim logFileName As String

    Public Sub New(objParametriServer As AgronicaCoreParametri, objParametriUtenti As AgronicaCoreParametri, token As String, ruolo As String, valore_ruolo_codice As String)
        logDirectory = objParametriServer.LogDirectory & "\BDN\"
        logFileName = "logBDN.txt"
        Me.objParametriServer = objParametriServer
        Me.objParametriUtenti = objParametriUtenti

        Me.wsRegistroStalla = New ChiamawsRegistroStallaQry(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Me.wsAnagraficaCapo = New ChiamawsAnagraficaCapoQry(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Me.wsAziende = New ChiamawsAziendeQry(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Me.wsCodici = New ChiamawsCodiciQry(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Me.wsIdentificativi = New ChiamawsIdentificativiGet(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Me.wsStrutture = New ChiamawsStruttureQry(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Me.wsTerritorio = New ChiamawsTerritorioQry(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Me.wsGestioneAssConsorzi = New ChiamawsGestioneAssConsorzi(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Me.wsChiamawsRegistriDiStalla = New ChiamawsRegistriDiStalla(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Me.wsChiamawsRegistroCapiStalla = New ChiamaWs_RegistroCapiStalla(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Me.wsSincronizzazioneAllevamento = New ImportazioneBDN.SincroBDNAllevamento(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Me.wsInterrogazioniModello4 = New wsInterrogazioneModello4(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Me.wsGestioneModello4 = New wsGestioneModello4(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)

        Me.sincronizzatoreAnimale = New SincroBDNAnimale(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(Me.objParametriServer.StringaConnessione)
        GiasContext = New Gias_DeveloperServer_Entities(EFConnString)

        Me.AgroWebConfig = New AgronicaCoreGestioneRichieste.AgroWebConfig

    End Sub

    ''' <summary>
    ''' Inserimento capi animali in BDN
    ''' </summary>
    ''' <param name="chiave"></param>
    ''' <returns></returns>
    Public Function InviaCapiBDN(ByVal chiave As OggettoInviaIngressi) As List(Of InviaIngressiResponse)

        Dim listaIngressiResponse As New List(Of InviaIngressiResponse)

        'ricavo Piva e Sa_Cod della Stalla dal primo elemento
        Dim Piva As String = chiave.Capi(0).Piva
        Dim Sa_Cod As Integer = chiave.Capi(0).Sa_Cod

        Dim Allev_IdFiscale As String = ""
        Dim CodiceAzienda_BDN As String = ""
        Dim Codice_Specie As String = ""

        Dim listaCodAnimale_CapiInserimento As New List(Of Integer)
        For Each capo In chiave.Capi
            listaCodAnimale_CapiInserimento.Add(capo.Cod_Progetto)
        Next

        Dim objZooAnimale As New AgronicaCoreAnagrafeDAL.Zoo_Animali
        Dim dtAnimaleDB As DataTable = objZooAnimale.Leggi_Giacenze(Piva, 0, 0, 0, 0,
                                                                    Date.Now, objParametriServer, False, False,
                                                                    listaCodAnimale_CapiInserimento)

        Dim listaCarichiNonSincronizzatiDT = objZooAnimale.BDN_Leggi_Movimenti_Carico_Non_Sincronizzati(Piva, Sa_Cod, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, False, New List(Of Integer), objParametriServer)

        Dim listaMatricole_CapiInseriti As New List(Of String)
        If Not IsNothing(dtAnimaleDB) OrElse dtAnimaleDB.Rows.Count > 0 Then
            For Each capoAnimale As DataRow In dtAnimaleDB.Rows
                Dim capoIngresso As New InviaIngressiResponse

                Dim CF_Detentore = capoAnimale("CF_DETENTORE")
                Dim CF_Proprietario = capoAnimale("CF_Proprietario")
                CodiceAzienda_BDN = capoAnimale("BDN_Codice_Azienda")

                Dim Cod_Animale As Integer = capoAnimale("Cod_Animale")
                Dim Matricola As String = capoAnimale("Matricola")
                Dim aggiornaDetentoreProprietario As Boolean = False

                Dim lav_cod = 0
                Try
                    lav_cod = listaCarichiNonSincronizzatiDT.Select(" Cod_Progetto = " & Cod_Animale)(0)("Lav_Cod")
                Catch ex As Exception

                End Try


                If CF_Proprietario <> "" Then
                    Allev_IdFiscale = CF_Proprietario
                Else
                    Allev_IdFiscale = chiave.Destinazione
                    CF_Proprietario = chiave.Destinazione
                    CF_Detentore = trovaCF_DetentoreStalla(Piva, CodiceAzienda_BDN, CF_Proprietario)
                    aggiornaDetentoreProprietario = True
                End If

                capoIngresso.Matricola = Matricola
                capoIngresso.importato = False
                capoIngresso.pathPdf = ""
                'ricava il SPECIE_CODICE dalla tab di mappatura CodificaBDN_SpecieAnimali
                Dim codificaBdnSpecie_R As New AgronicaCoreMetaSchemaDAL.Codifica_BDN_SpecieAnimali
                Dim dtSpecieFromCodificaBDN As DataTable = codificaBdnSpecie_R.leggi(objParametriServer, "", "",
                                                                                     capoAnimale("GEN_COD"),
                                                                                     capoAnimale("SPE_COD"),
                                                                                     3)

                If Not IsNothing(dtSpecieFromCodificaBDN) AndAlso dtSpecieFromCodificaBDN.Rows.Count > 0 Then
                    Codice_Specie = "0" & dtSpecieFromCodificaBDN(0)("CODICE")
                End If

                'controllo che il capo non sia già presente in BDN
                Dim CapoPresenteBDN As Boolean = True
                Dim dtAnimaleBDN As New DataTable

                Dim azienda_codice_capo As String = ""
                Dim allev_id_fiscale_capo As String = ""
                Try
                    dtAnimaleBDN = wsAnagraficaCapo.getCapo(Matricola)

                    If dtAnimaleBDN IsNot Nothing AndAlso dtAnimaleBDN.Rows.Count > 0 AndAlso dtAnimaleBDN.Columns.Contains("ALLEV_ID_FISCALE_ATTUALE") Then
                        If Not IsDBNull(dtAnimaleBDN(0)("ALLEV_ID_FISCALE_ATTUALE")) Then
                            allev_id_fiscale_capo = CStr(dtAnimaleBDN(0)("ALLEV_ID_FISCALE_ATTUALE"))
                        End If
                    End If

                    If dtAnimaleBDN IsNot Nothing AndAlso dtAnimaleBDN.Rows.Count > 0 AndAlso dtAnimaleBDN.Columns.Contains("AZIENDA_CODICE_ATTUALE") Then
                        If Not IsDBNull(dtAnimaleBDN(0)("AZIENDA_CODICE_ATTUALE")) Then
                            azienda_codice_capo = CStr(dtAnimaleBDN(0)("AZIENDA_CODICE_ATTUALE"))
                        End If
                    End If

                    If azienda_codice_capo = "" Or allev_id_fiscale_capo = "" Then
                        Dim stallaAllevamentoOrigine = GetCodiceStallaAttuale(Matricola)
                        If stallaAllevamentoOrigine <> "" Then
                            azienda_codice_capo = stallaAllevamentoOrigine.Split("_")(0)
                            allev_id_fiscale_capo = stallaAllevamentoOrigine.Split("_")(1)
                        End If
                    End If


                Catch ex As Exception

                    If ex.Message.Split(">")(1) = "CODICE CAPO BOVINO NON PRESENTE IN ANAGRAFE" Then
                        CapoPresenteBDN = False
                        Exit Try
                    Else
                        Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
                    End If

                End Try

                Try
                    'ricavo il codice dello stato dalla Matricola
                    Dim Stato_Codice As String = ""
                    If Matricola <> "" Then
                        Stato_Codice = Matricola.Chars(0) & Matricola.Chars(1)
                    End If

                    If Not CapoPresenteBDN AndAlso
                        (IsDBNull(capoAnimale("Modello4_Ingresso_Numero")) OrElse capoAnimale("Modello4_Ingresso_Numero") = "") Then
                        '-------------------------------------------- CAPO FRANCESE/ESTERO = Insert_Capi() ----------------------------------------------------------------------------------------

                        Dim dsCapoInserito As New DataSet

                        'CAPO_ID sempre a 0 in inserimento
                        Dim P_CAPO_ID As String = "0"
                        If CapoPresenteBDN Then
                            P_CAPO_ID = CStr(dtAnimaleBDN.Rows(0)("CAPO_ID"))
                        End If
                        'P_INS_VAR sempre I in inserimento
                        Dim P_INS_VAR As String = "I"
                        'matricola del capo da inserire
                        Dim P_CAPO_CODICE_IDENTIFICATIVO As String = Matricola
                        Dim P_SESSO As String = capoAnimale("Sesso")

                        'ricava il RAZZA_CODICE dalla tab di mappatura CodificaBDN_RazzeAnimali
                        Dim codificaBdnRazza_R As New AgronicaCoreMetaSchemaDAL.Codifica_BDN_RazzeAnimali
                        Dim dtRazzaFromCodificaBDN As DataTable = codificaBdnRazza_R.leggi(objParametriServer,
                                                                                           "", "",
                                                                                           capoAnimale("GEN_COD"),
                                                                                           capoAnimale("SPE_COD"),
                                                                                           capoAnimale("RAZ_COD"),
                                                                                           "3")

                        Dim P_RAZZA_CODICE As String = ""
                        If Not IsNothing(dtRazzaFromCodificaBDN) AndAlso dtRazzaFromCodificaBDN.Rows.Count > 0 Then
                            P_RAZZA_CODICE = dtRazzaFromCodificaBDN(0)("CODICE")
                        Else
                            Throw New BDNException("Razza non sincronizzata con BDN")
                        End If

                        Dim P_DT_NASCITA As String = CDate(capoAnimale("DAT_NASCITA")).ToString("yyyy-MM-dd")
                        Dim P_COD_MARCHIO_MADRE As String = capoAnimale("MAT_MADRE")
                        Dim P_COD_MARCHIO_PADRE As String = capoAnimale("MAT_PADRE")
                        Dim P_TAG As String = ""
                        Dim P_CODICE_PRECEDENTE As String = ""
                        Dim P_DT_INIZIO_LATTAZIONE As String = "1900-01-01"
                        Dim P_DT_FINE_LATTAZIONE As String = "1900-01-01"
                        Dim P_DT_APPLICAZIONE_MARCHIO As String = "1900-01-01"
                        Dim P_DT_ISCRIZIONE_IN_ANAGRAFE As String = CDate(capoAnimale("Validita_Inizio")).ToString("yyyy-MM-dd")
                        Dim P_DT_COMPILAZIONE_CEDOLA As String = "1900-01-01"
                        Dim P_FLAG_INSEMINAZIONE As String = "N"
                        Dim P_TIPO_ORIGINE As String = "E"
                        Dim P_CODICE_PARTITE_ANIMO As String = ""
                        Dim P_CODICE_STATI As String = Stato_Codice
                        'Dim P_ALLEV_ID_FISCALE As String = Allev_IdFiscale
                        Dim P_ALLEV_ID_FISCALE As String = chiave.Destinazione
                        Dim P_AZIENDA_CODICE As String = CodiceAzienda_BDN
                        Dim P_MARCHE_PRODOTTE_CODICE As String = ""
                        Dim P_CODICE_LIBRO As String = ""
                        Dim P_ID_FISCALE_DETEN As String = CF_Detentore
                        Dim P_CODICE_AZIENDA_DETEN As String = capoAnimale("BDN_Codice_Azienda")
                        Dim P_CODICE_SPECIE_DETEN As String = Codice_Specie
                        Dim P_DT_INGRESSO_DETEN As String = CDate(capoAnimale("Validita_Inizio")).ToString("yyyy-MM-dd")
                        Dim P_ASL_CODICE As String = ""
                        Dim P_DISTRETTO_CODICE As String = ""
                        Dim P_DT_INGRESSO As String = CDate(capoAnimale("Validita_Inizio")).ToString("yyyy-MM-dd")

                        Dim P_MOTIVO_INGRESSO As String = "E"
                        If CapoPresenteBDN Then
                            P_MOTIVO_INGRESSO = "M"
                        End If

                        If lav_cod = LAVCOD_NASCITA_ANIMALI Then
                            P_MOTIVO_INGRESSO = "N"
                            P_TIPO_ORIGINE = "N"
                            P_DT_ISCRIZIONE_IN_ANAGRAFE = P_DT_NASCITA 'CDate(Date.Now).ToString("yyyy-MM-dd")
                            P_DT_APPLICAZIONE_MARCHIO = P_DT_NASCITA
                            P_DT_COMPILAZIONE_CEDOLA = P_DT_NASCITA
                            P_MARCHE_PRODOTTE_CODICE = Matricola
                            P_TAG = capoAnimale("Tag")
                            Dim flagPartoGemellare = isPartoGemellare(Piva, Sa_Cod, capoAnimale("Sta_Num"), capoAnimale("DAT_NASCITA"), Cod_Animale, capoAnimale("MAT_MADRE"), objParametriServer)
                            If flagPartoGemellare Then
                                P_MOTIVO_INGRESSO = "W"
                            End If
                        End If

                        Dim P_DT_RILASCIO_PASSAPORTO As String = "1900-01-01"

                        Dim P_TIPO_PASSAPORTO As String = ""
                        If Stato_Codice = "IT" Then
                            P_TIPO_PASSAPORTO = "IT"
                        ElseIf Stato_Codice <> "" Then
                            P_TIPO_PASSAPORTO = "UE"
                        ElseIf Stato_Codice = "" Then
                            P_TIPO_PASSAPORTO = "NP"
                        End If

                        Dim P_NUMERO_RIF_LOCALE As String = ""
                        Dim P_ID_FISCALE_MARCATORE As String = ""
                        Dim P_COD_MADRE_GENETICA As String = ""
                        Dim P_RIF_MODELLO As String = ""
                        Dim P_DT_MODELLO As String = "1900-01-01"
                        Dim P_CODICE_AZIENDA_PROV As String = ""
                        Dim P_ID_FISCALE_PROV As String = ""
                        Dim P_CODICE_SPECIE_PROV As String = ""
                        Dim P_CODICE_FM_PROV As String = ""
                        Dim P_CODICE_STATI_ORIGINE As String = Stato_Codice

                        If (IsDBNull(capoAnimale("Modello4_Ingresso_Numero")) OrElse capoAnimale("Modello4_Ingresso_Numero") = "") Then
                            P_NUMERO_RIF_LOCALE = capoAnimale("Certificato")
                            P_CODICE_PARTITE_ANIMO = capoAnimale("Certificato")
                        Else
                            P_RIF_MODELLO = capoAnimale("Modello4_Ingresso_Numero")
                            P_DT_MODELLO = capoAnimale("Data_Documento_Ingresso")
                        End If


                        'INSERIMENTO CAPO in BDN
                        dsCapoInserito = wsChiamawsRegistroCapiStalla.Insert_Capi(P_CAPO_ID,
                                                                                          P_INS_VAR,
                                                                                          P_CAPO_CODICE_IDENTIFICATIVO,
                                                                                          P_SESSO,
                                                                                          P_RAZZA_CODICE,
                                                                                          P_DT_NASCITA,
                                                                                          P_COD_MARCHIO_MADRE,
                                                                                          P_COD_MARCHIO_PADRE,
                                                                                          P_TAG,
                                                                                          P_CODICE_PRECEDENTE,
                                                                                          P_DT_INIZIO_LATTAZIONE,
                                                                                          P_DT_FINE_LATTAZIONE,
                                                                                          P_DT_APPLICAZIONE_MARCHIO,
                                                                                          P_DT_ISCRIZIONE_IN_ANAGRAFE,
                                                                                          P_DT_COMPILAZIONE_CEDOLA,
                                                                                          P_FLAG_INSEMINAZIONE,
                                                                                          P_TIPO_ORIGINE,
                                                                                          P_CODICE_PARTITE_ANIMO,
                                                                                          P_CODICE_STATI,
                                                                                          P_ALLEV_ID_FISCALE,
                                                                                          P_AZIENDA_CODICE,
                                                                                          P_MARCHE_PRODOTTE_CODICE,
                                                                                          P_CODICE_LIBRO,
                                                                                          Codice_Specie,
                                                                                          P_ID_FISCALE_DETEN,
                                                                                          P_CODICE_AZIENDA_DETEN,
                                                                                          P_CODICE_SPECIE_DETEN,
                                                                                          P_DT_INGRESSO_DETEN,
                                                                                          P_ASL_CODICE,
                                                                                          P_DISTRETTO_CODICE,
                                                                                          P_DT_INGRESSO,
                                                                                          P_MOTIVO_INGRESSO,
                                                                                          P_DT_RILASCIO_PASSAPORTO,
                                                                                          P_TIPO_PASSAPORTO,
                                                                                          P_NUMERO_RIF_LOCALE,
                                                                                          P_ID_FISCALE_MARCATORE,
                                                                                          P_COD_MADRE_GENETICA,
                                                                                          P_RIF_MODELLO,
                                                                                          P_DT_MODELLO,
                                                                                          P_CODICE_AZIENDA_PROV,
                                                                                          P_ID_FISCALE_PROV,
                                                                                          P_CODICE_SPECIE_PROV,
                                                                                          P_CODICE_FM_PROV,
                                                                                          P_CODICE_STATI_ORIGINE)

                        'controlla se è stato effettuato l'inserimento
                        If Not IsNothing(dsCapoInserito) AndAlso dsCapoInserito.Tables.Count > 0 Then
                            Dim Capo_IdBDN As String = ""
                            dtAnimaleBDN = New DataTable

                            For Each table In dsCapoInserito.Tables

                                If Not IsNothing(table) AndAlso
                                        table.Rows.Count > 0 AndAlso
                                        table.TableName = "CAPI_BOVINI" Then
                                    Dim dtCapoInserito As DataTable = table

                                    Matricola = dtCapoInserito(0)("CODICE")
                                    Capo_IdBDN = dtCapoInserito(0)("CAPO_ID")

                                    dtAnimaleBDN = wsAnagraficaCapo.getCapo(Matricola)

                                    'se il capo è stato aggiunto correttamente si salva la sua Matricola (P_CAPO_CODICE)
                                    'e inserisce i dati mancanti su DB (Capo_IdBDN)
                                    If Not IsNothing(dtAnimaleBDN) OrElse dtAnimaleBDN.Rows.Count > 0 Then
                                        listaMatricole_CapiInseriti.Add(Matricola)

                                        Dim objCapoAnimale As New anagrafiche.CapoAnimale(Piva,
                                                                                              capoAnimale("Cod_Animale"),
                                                                                              Matricola)

                                        objCapoAnimale.idCapo_BDN = Capo_IdBDN
                                        objCapoAnimale.codiceFiscaleProprietario = dtAnimaleBDN(0)("ALLEV_ID_FISCALE_ATTUALE")

                                        Dim objScrivi_Zoo As New AgronicaCoreAnagrafeBIZ.Zoo
                                        objScrivi_Zoo.Converti_Animale_DT(objCapoAnimale, objParametriServer,
                                                                              Nothing, True, "InviaCapiBDN")

                                        Dim id_Ingresso = sincronizzatoreAnimale.trovaID_Ingresso(CF_Proprietario,
                                                                                                  capoAnimale("BDN_Codice_Azienda"),
                                                                                                    Matricola)


                                        If id_Ingresso <> 0 Then
                                            Dim drCarico = listaCarichiNonSincronizzatiDT.Select(" Matricola = '" & Matricola & "' ")
                                            If drCarico.Length > 0 Then
                                                Dim PivaCarico As String = drCarico(0)("Piva")
                                                Dim ID_AgendaCarico As Integer = drCarico(0)("id_agenda")
                                                Dim ID_MovCarico As Integer = drCarico(0)("id_mov")
                                                Dim ID_Mov_DetCarico As Integer = drCarico(0)("id_Mov_Det")
                                                Dim lavCod As Integer = drCarico(0)("Lav_Cod")
                                                Dim movDettaglioCarico = (From g In GiasContext.Movimenti_dettagli
                                                                          Where g.PIVA = PivaCarico AndAlso
                                                                                g.Id_Agenda = ID_AgendaCarico AndAlso
                                                                                g.Id_Mov = ID_MovCarico AndAlso
                                                                                g.Id_Mov_Det = ID_Mov_DetCarico).FirstOrDefault()
                                                movDettaglioCarico.Id_Mov_Esterno = id_Ingresso
                                                GiasContext.Entry(movDettaglioCarico).State = EntityState.Modified

                                                GiasContext.SaveChanges()

                                                logMovimentazioniBDN(Piva, Sa_Cod, ID_AgendaCarico, lavCod, enumCausaliBDN.InvAnagraficaBDN,
                                                                     id_Ingresso, ID_MovCarico, ID_Mov_DetCarico, objCapoAnimale.codice,
                                                                     objCapoAnimale.idCapo_BDN, Nothing, dtCapoInserito, True, GiasContext)
                                                capoIngresso.importato = True
                                            End If
                                        End If

                                        Exit For

                                    Else
                                        logMovimentazioniBDN(Piva, Sa_Cod, 0, 0, enumCausaliBDN.InvAnagraficaBDN,
                                                             0, 0, 0, Cod_Animale, Capo_IdBDN, Nothing,
                                                             dtCapoInserito, False, GiasContext)
                                    End If
                                End If

                            Next
                        Else
                            logMovimentazioniBDN(Piva, Sa_Cod, 0, 0, enumCausaliBDN.InvAnagraficaBDN,
                                                 0, 0, 0, Cod_Animale, 0, Nothing,
                                                 Nothing, False, GiasContext)
                        End If

                    ElseIf Not IsDBNull(capoAnimale("Modello4_Ingresso_Numero")) AndAlso
                        capoAnimale("Modello4_Ingresso_Numero") <> "" AndAlso
                        azienda_codice_capo <> CodiceAzienda_BDN Then
                        '-------------------------------------------- CAPO ITALIANO = Insert_Ingresso() ----------------------------------------------------------------------------------------

                        Dim dsCapoInviato As New DataSet

                        'CODICE INGRESSO in inserimento fisso a 0
                        Dim P_INGRESSO_ID As Integer = 0
                        'Operazone da effettuare, in INSERIMENTO fisso a I
                        Dim P_INS_VAR As String = "I"

                        Dim P_NUM_MODELLO As String = capoAnimale("Modello4_Ingresso_Numero")
                        Dim P_DATA_EMISSIONE_MODELLO As String = CDate(capoAnimale("Validita_Inizio")).ToString("yyyy-MM-dd")
                        Dim P_CAPO_CODICE As String = Matricola

                        Dim P_SPECIE_DESTINAZIONE_CODICE As String = Codice_Specie
                        Dim P_AZIENDA_DESTINAZIONE_CODICE As String = CodiceAzienda_BDN
                        Dim P_ALLEV_DESTINAZIONE_ID_FISCALE As String = Allev_IdFiscale

                        Dim P_SPECIE_ORIGINE_CODICE As String = Codice_Specie
                        Dim P_AZIENDA_ORIGINE_CODICE As String = azienda_codice_capo
                        Dim P_ALLEVAMENTO_ORIGINE_IDFISCALE As String = allev_id_fiscale_capo

                        Dim P_ASL_CODICE As String = ""
                        Dim P_DISTRETTO_CODICE As String = ""
                        Dim P_MOTIVO As String = "M"
                        Dim P_DATA_INGRESSO As String = CDate(capoAnimale("Validita_Inizio")).ToString("yyyy-MM-dd")
                        Dim P_DATA_COMUNICAZIONE_INGRESSO As String = CDate(capoAnimale("Validita_Inizio")).ToString("yyyy-MM-dd")

                        Dim P_NOTE_REGISTRO As String = ""
                        Dim P_ID_FISCALE_SOCC As String = ""
                        Dim P_DT_INIZIO_SOCCIDA As String = CDate(AGRODATAINIZIO).ToString("yyyy-MM-dd")

                        objLog.Scrivi_LOG(objParametriServer, "", "Qui ci arrivo1!")

                        'INSERIMENTO INGRESSO CAPO in BDN
                        dsCapoInviato = wsChiamawsRegistriDiStalla.Insert_Ingresso(P_INGRESSO_ID,
                                                                                     P_INS_VAR,
                                                                                     P_NUM_MODELLO,
                                                                                     P_DATA_EMISSIONE_MODELLO,
                                                                                     P_CAPO_CODICE,
                                                                                     P_SPECIE_DESTINAZIONE_CODICE,
                                                                                     P_AZIENDA_DESTINAZIONE_CODICE,
                                                                                     P_ALLEV_DESTINAZIONE_ID_FISCALE,
                                                                                     P_SPECIE_ORIGINE_CODICE,
                                                                                     P_AZIENDA_ORIGINE_CODICE,
                                                                                     P_ALLEVAMENTO_ORIGINE_IDFISCALE,
                                                                                     P_ASL_CODICE,
                                                                                     P_DISTRETTO_CODICE,
                                                                                     P_MOTIVO,
                                                                                     P_DATA_INGRESSO,
                                                                                     P_DATA_COMUNICAZIONE_INGRESSO,
                                                                                     P_NOTE_REGISTRO,
                                                                                     P_ID_FISCALE_SOCC,
                                                                                     P_DT_INIZIO_SOCCIDA)

                        objLog.Scrivi_LOG(objParametriServer, "", "Qui ci arrivo2! dsCapoInviato:" & JsonConvert.SerializeObject(dsCapoInviato))

                        'controlla se è stato effettuato l'inserimento
                        If Not IsNothing(dsCapoInviato) AndAlso dsCapoInviato.Tables.Count > 0 Then
                            Dim Capo_IdBDN As String = ""
                            dtAnimaleBDN = New DataTable

                            For Each table In dsCapoInviato.Tables

                                If Not IsNothing(table) AndAlso
                                    table.Rows.Count > 0 AndAlso
                                    table.TableName = "REGISTRI_DI_STALLA" Then
                                    Dim dtCapoInserito As DataTable = table

                                    Matricola = dtCapoInserito(0)("CAPO_CODICE")
                                    Capo_IdBDN = dtCapoInserito(0)("CAPO_ID")

                                    dtAnimaleBDN = wsAnagraficaCapo.getCapo(Matricola)
                                    objLog.Scrivi_LOG(objParametriServer, "", "Qui ci arrivo3! dtAnimaleBDN:" & JsonConvert.SerializeObject(dtAnimaleBDN))
                                    'se il capo è stato aggiunto correttamente si salva la sua Matricola (P_CAPO_CODICE)
                                    If Not IsNothing(dtAnimaleBDN) OrElse dtAnimaleBDN.Rows.Count > 0 Then
                                        listaMatricole_CapiInseriti.Add(Matricola)
                                        Dim zoo_animali = (From z In GiasContext.Zoo_Animali Where z.Cod_Progetto = Cod_Animale).FirstOrDefault()
                                        If zoo_animali IsNot Nothing Then
                                            zoo_animali.Id_Capo_BDN = Capo_IdBDN
                                            GiasContext.Entry(zoo_animali).State = EntityState.Modified
                                            GiasContext.SaveChanges()
                                        End If

                                        Dim id_Ingresso = sincronizzatoreAnimale.trovaID_Ingresso(CF_Proprietario,
                                                                                                  capoAnimale("BDN_Codice_Azienda"),
                                                                                                  Matricola)

                                        objLog.Scrivi_LOG(objParametriServer, "", "Qui ci arrivo4! id_Ingresso:" & id_Ingresso)

                                        If id_Ingresso <> 0 Then
                                            Dim drCarico = listaCarichiNonSincronizzatiDT.Select(" Matricola = '" & Matricola & "' ")
                                            If drCarico.Length > 0 Then
                                                Dim PivaCarico As String = drCarico(0)("Piva")
                                                Dim ID_AgendaCarico As Integer = drCarico(0)("id_agenda")
                                                Dim ID_MovCarico As Integer = drCarico(0)("id_mov")
                                                Dim ID_Mov_DetCarico As Integer = drCarico(0)("id_Mov_Det")
                                                Dim lavCod As Integer = drCarico(0)("Lav_Cod")

                                                Dim movDettaglioCarico = (From g In GiasContext.Movimenti_dettagli
                                                                          Where g.PIVA = PivaCarico AndAlso
                                                                                g.Id_Agenda = ID_AgendaCarico AndAlso
                                                                                g.Id_Mov = ID_MovCarico AndAlso
                                                                                g.Id_Mov_Det = ID_Mov_DetCarico).FirstOrDefault()
                                                movDettaglioCarico.Id_Mov_Esterno = id_Ingresso
                                                GiasContext.Entry(movDettaglioCarico).State = EntityState.Modified

                                                GiasContext.SaveChanges()

                                                logMovimentazioniBDN(Piva, Sa_Cod, ID_AgendaCarico, lavCod, enumCausaliBDN.InvAnagraficaBDN,
                                                                     id_Ingresso, ID_MovCarico, ID_Mov_DetCarico, Cod_Animale,
                                                                     Capo_IdBDN, Nothing, dtCapoInserito, True, GiasContext)
                                                capoIngresso.importato = True

                                                objLog.Scrivi_LOG(objParametriServer, "", "Qui ci arrivo5! capoIngresso.importato = True")

                                            End If
                                        End If


                                        Exit For
                                    Else
                                        logMovimentazioniBDN(Piva, Sa_Cod, 0, 0, enumCausaliBDN.InvAnagraficaBDN,
                                                             0, 0, 0, Cod_Animale, 0, Nothing,
                                                             Nothing, False, GiasContext)
                                    End If

                                End If

                            Next
                        Else
                            logMovimentazioniBDN(Piva, Sa_Cod, 0, 0, enumCausaliBDN.InvAnagraficaBDN,
                                                 0, 0, 0, Cod_Animale, 0, Nothing,
                                                 Nothing, False, GiasContext)
                        End If

                    ElseIf azienda_codice_capo = CodiceAzienda_BDN Then
                        objLog.Scrivi_LOG(objParametriServer, "", "Else")
                        listaMatricole_CapiInseriti.Add(Matricola)
                        Dim id_Ingresso = sincronizzatoreAnimale.trovaID_Ingresso(CF_Proprietario,
                                                                                  capoAnimale("BDN_Codice_Azienda"),
                                                                                  Matricola)
                        objLog.Scrivi_LOG(objParametriServer, "", "Qui ci arrivo4a! id_Ingresso:" & id_Ingresso)

                        If id_Ingresso <> 0 Then
                            Dim drCarico = listaCarichiNonSincronizzatiDT.Select(" Matricola = '" & Matricola & "' ")
                            If drCarico.Length > 0 Then
                                Dim PivaCarico As String = drCarico(0)("Piva")
                                Dim ID_AgendaCarico As Integer = drCarico(0)("id_agenda")
                                Dim ID_MovCarico As Integer = drCarico(0)("id_mov")
                                Dim ID_Mov_DetCarico As Integer = drCarico(0)("id_Mov_Det")
                                Dim lavCod As Integer = drCarico(0)("Lav_Cod")
                                dtAnimaleBDN = wsAnagraficaCapo.getCapo(Matricola)
                                Dim Capo_IdBDN = CInt(dtAnimaleBDN(0)("CAPO_ID"))
                                Dim movDettaglioCarico = (From g In GiasContext.Movimenti_dettagli
                                                          Where g.PIVA = PivaCarico AndAlso
                                                                g.Id_Agenda = ID_AgendaCarico AndAlso
                                                                g.Id_Mov = ID_MovCarico AndAlso
                                                                g.Id_Mov_Det = ID_Mov_DetCarico).FirstOrDefault()
                                movDettaglioCarico.Id_Mov_Esterno = id_Ingresso
                                GiasContext.Entry(movDettaglioCarico).State = EntityState.Modified

                                GiasContext.SaveChanges()

                                logMovimentazioniBDN(Piva, Sa_Cod, ID_AgendaCarico, lavCod, enumCausaliBDN.InvAnagraficaBDN,
                                                     id_Ingresso, ID_MovCarico, ID_Mov_DetCarico, Cod_Animale,
                                                     Capo_IdBDN, Nothing, Nothing, True, GiasContext)
                                capoIngresso.importato = True

                                objLog.Scrivi_LOG(objParametriServer, "", "Qui ci arrivo5! capoIngresso.importato = True")

                            End If
                        End If
                    Else
                        Throw New BDNException("Errore nei dati riguardanti il capo animale")

                    End If

                Catch ex As BDNException
                    capoIngresso.errore = ex.Message
                    'If CapoPresenteBDN Then
                    '	Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & " Capo già registrato su BDN")
                    'Else
                    '	Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
                    'End If

                Catch ex As Exception
                    capoIngresso.errore = ex.Message
                    'Throw New Exception("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)

                End Try

                If aggiornaDetentoreProprietario Then

                    Dim capo_animale_db = (From z In GiasContext.Zoo_Animali Where z.Matricola = Matricola AndAlso z.Cod_Progetto = Cod_Animale).FirstOrDefault

                    If capo_animale_db IsNot Nothing Then
                        capo_animale_db.CF_DETENTORE = CF_Detentore
                        capo_animale_db.CF_PROPRIETARIO = CF_Proprietario
                        GiasContext.Entry(capo_animale_db).State = EntityState.Modified
                        GiasContext.SaveChanges()
                    End If

                End If

                listaIngressiResponse.Add(capoIngresso)
            Next

        End If

        Dim listaCapiInseriti_Effett As New List(Of String)

        ''recupera lo STA_NUM della Stalla
        'Dim objStalla_R As New AgronicaCoreAnagrafeDAL.Stalla_R
        'Dim dtStalla As DataTable = objStalla_R.Leggi(Piva, Sa_Cod, 0,
        '                                              AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
        '                                              "Stalla.BDN_Allev_IdFiscale = '" & Allev_IdFiscale &
        '                                                           "' AND Stalla.BDN_Codice_Azienda = '" & CodiceAzienda_BDN & "' ",
        '                                              "", objParametriServer)

        'If IsNothing(dtStalla) OrElse dtStalla.Rows.Count = 0 Then
        '    Throw New GiasException("Allevamento " & CodiceAzienda_BDN & " non configurato su GIAS")
        'ElseIf dtStalla.Rows.Count > 1 Then
        '    Throw New GiasException("Allevamento " & CodiceAzienda_BDN & " configurato più volte su GIAS")
        'End If

        'Dim SincroAllevamento As SincroBDN_Allevamento_Response = Me.wsSincronizzazioneAllevamento.SincronizzaAllevamento(Piva,
        '                                                                                                                  CodiceAzienda_BDN,
        '                                                                                                                  Allev_IdFiscale,
        '                                                                                                                  Codice_Specie)

        ''capi che dopo essere stati aggiunti su BDN sono stati anche sincronizzati con quest'ultima
        'listaCapiInseriti_Effett.AddRange(listaMatricole_CapiInseriti.Except(SincroAllevamento.listaCapi_Ingresso))

        Return listaIngressiResponse

    End Function

    Public Function isPartoGemellare(Piva As String, Sa_Cod As Integer, Sta_Num As Integer, Data_Movimento As Date, Cod_Animale As Integer, Matricola_Madre As String, objParametriServer As AgronicaCoreParametri) As Boolean
        Dim objZoo_Animale As New AgronicaCoreAnagrafeDAL.Zoo_Animali
        Dim dtNascita = objZoo_Animale.Leggi_Carico_Capi(Piva, Sa_Cod, Sta_Num, 0, 0, Data_Movimento, Data_Movimento, "", False, Nothing, LAVCOD_NASCITA_ANIMALI, False, False, objParametriServer, False)
        Dim rowNascite = dtNascita.Select(" Mat_Madre = '" & Matricola_Madre & "' AND Cod_Animale <> " & CStr(Cod_Animale))
        If rowNascite.Count > 0 Then
            Return True
        End If
        Return False
    End Function

    Public Function trovaCF_DetentoreStalla(Piva As String, Codice_Azienda_BDN As String, CF_Proprietario As String) As String
        Dim objConfigurazioneBDN As New AgronicaCoreAnagrafeDAL.Stalla_Configurazioni_BDN_R
        Dim dt = objConfigurazioneBDN.LeggiDaCodiceAziendaBDN(0, Piva, 0, 0, "", CF_Proprietario, Codice_Azienda_BDN, DateTime.Now, DateTime.Now, "", "", objParametriServer)
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            Return dt(0)("CF_Detentore")
        End If
        Return ""
    End Function


    Public Function GetCodiceStallaAttuale(Matricola As String) As String

        Dim dtMovimentazioni As DataTable
        Try
            dtMovimentazioni = wsRegistroStalla.getMovimentazioniCapo(Matricola)

            If dtMovimentazioni IsNot Nothing AndAlso dtMovimentazioni.Rows.Count > 0 Then
                If dtMovimentazioni.Rows.Count > 1 Then
                    Dim aa = 0
                End If
                Dim dtUltimaMovimentazione = (From r As DataRow In dtMovimentazioni.Rows
                                              Order By CDate(r("DT_INGRESSO")) Descending
                                              Select r).FirstOrDefault()
                If Not IsNothing(dtUltimaMovimentazione) AndAlso Not IsDBNull(dtUltimaMovimentazione("AZIENDA_CODICE")) Then
                    Dim azienda_codice = dtUltimaMovimentazione("AZIENDA_CODICE")
                    Dim allevamento_id_Fiscale = If(IsDBNull(dtUltimaMovimentazione("ALLEV_ID_FISCALE")), "", dtUltimaMovimentazione("ALLEV_ID_FISCALE"))
                    Return azienda_codice & "_" & allevamento_id_Fiscale
                End If
            End If

        Catch ex As Exception

        End Try

    End Function

    ''' <summary>
    ''' Modifica capi animali in BDN
    ''' </summary>
    ''' <param name="chiave">Piva_SaCod_CodAnimale</param>
    ''' <returns></returns>
    Public Sub ModificaCapiBDN(ByVal chiave As String)
        Dim chiaveArr() As String = JsonConvert.DeserializeObject(Of String())(chiave)

        'ricavo Piva e Sa_Cod della Stalla dal primo elemento
        Dim Piva As String = chiaveArr(0).Split("_")(0)
        Dim Sa_Cod As Integer = chiaveArr(0).Split("_")(1)
        Dim Allev_IdFiscale As String = ""
        Dim CodiceAzienda_BDN As String = ""
        Dim Codice_Specie As String = ""

        Dim listaCodAnimale_CapiModifica As New List(Of Integer)
        For Each capo In chiaveArr
            listaCodAnimale_CapiModifica.Add(capo.Split("_")(2))
        Next

        'ricava le info da DB dei capi in eliminazione
        Dim objZooAnimale As New AgronicaCoreAnagrafeDAL.Zoo_Animali
        Dim dtAnimaleDB As DataTable = objZooAnimale.Leggi_Giacenze(Piva, 0, 0, 0, 0, Date.Now,
                                                                    objParametriServer, False, False,
                                                                    listaCodAnimale_CapiModifica)

        Dim listaMatricole_CapiModificati As New List(Of String)
        If Not IsNothing(dtAnimaleDB) OrElse dtAnimaleDB.Rows.Count > 0 Then

            For Each capoAnimale As DataRow In dtAnimaleDB.Rows
                Dim Cod_Animale As Integer = capoAnimale("Cod_Animale")
                Dim Matricola As String = dtAnimaleDB(0)("Matricola")
                Allev_IdFiscale = dtAnimaleDB(0)("CUAA")
                CodiceAzienda_BDN = dtAnimaleDB(0)("BDN_Codice_Azienda")


                'controllo che il capo non sia già presente in BDN
                Dim CapoPresenteBDN As Boolean = True
                Dim dtAnimaleBDN As New DataTable
                Try
                    dtAnimaleBDN = wsAnagraficaCapo.getCapo(Matricola)

                Catch ex As Exception

                    If ex.Message.Split(">")(1) = "CODICE CAPO BOVINO NON PRESENTE IN ANAGRAFE" Then
                        CapoPresenteBDN = False
                        Exit Try
                    Else
                        Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
                    End If

                End Try

                'entra in modifica se il capo è presente in BDN
                If CapoPresenteBDN Then

                    Try
                        'ricavo il codice dello stato dalla Matricola
                        Dim Stato_Codice As String = ""
                        If Matricola <> "" Then
                            Stato_Codice = Matricola.Chars(0) & Matricola.Chars(1)
                        End If

                        'CAPO_ID sempre a 0 in inserimento
                        Dim P_CAPO_ID As String = "0"
                        'P_INS_VAR sempre I in inserimento
                        Dim P_INS_VAR As String = "I"
                        'matricola del capo da inserire
                        Dim P_CAPO_CODICE_IDENTIFICATIVO As String = Matricola
                        Dim P_SESSO As String = capoAnimale("Sesso")

                        'ricava il SPECIE_CODICE dalla tab di mappatura CodificaBDN_SpecieAnimali
                        Dim codificaBdnSpecie_R As New AgronicaCoreMetaSchemaDAL.Codifica_BDN_SpecieAnimali
                        Dim dtSpecieFromCodificaBDN As DataTable = codificaBdnSpecie_R.leggi(objParametriServer, "", "",
                                                                                             capoAnimale("GEN_COD"),
                                                                                             capoAnimale("SPE_COD"),
                                                                                             3)

                        If Not IsNothing(dtSpecieFromCodificaBDN) AndAlso dtSpecieFromCodificaBDN.Rows.Count > 0 Then
                            Codice_Specie = "0" & dtSpecieFromCodificaBDN(0)("CODICE")
                        End If

                        'ricava il RAZZA_CODICE dalla tab di mappatura CodificaBDN_RazzeAnimali
                        Dim codificaBdnRazza_R As New AgronicaCoreMetaSchemaDAL.Codifica_BDN_RazzeAnimali
                        Dim dtRazzaFromCodificaBDN As DataTable = codificaBdnRazza_R.leggi(objParametriServer,
                                                                                           "", "",
                                                                                           capoAnimale("GEN_COD"),
                                                                                           capoAnimale("SPE_COD"),
                                                                                           capoAnimale("RAZ_COD"),
                                                                                           "3")

                        Dim P_RAZZA_CODICE As String = ""
                        If Not IsNothing(dtRazzaFromCodificaBDN) AndAlso dtRazzaFromCodificaBDN.Rows.Count > 0 Then
                            P_RAZZA_CODICE = dtRazzaFromCodificaBDN(0)("CODICE")
                        Else
                            Throw New BDNException("Razza non sincronizzata con BDN")
                        End If

                        Dim P_DT_NASCITA As String = capoAnimale("DAT_NASCITA")
                        Dim P_COD_MARCHIO_MADRE As String = capoAnimale("MAT_MADRE")
                        Dim P_COD_MARCHIO_PADRE As String = capoAnimale("MAT_PADRE")
                        Dim P_TAG As String = ""
                        Dim P_CODICE_PRECEDENTE As String = ""
                        Dim P_DT_INIZIO_LATTAZIONE As String = "2022-09-16"
                        Dim P_DT_FINE_LATTAZIONE As String = "2022-10-16"
                        Dim P_DT_APPLICAZIONE_MARCHIO As String = "2022-09-16"
                        Dim P_DT_ISCRIZIONE_IN_ANAGRAFE As String = CDate(capoAnimale("Validita_Inizio")).ToString("yyyy-MM-dd")
                        Dim P_DT_COMPILAZIONE_CEDOLA As String = "2022-09-16"
                        Dim P_FLAG_INSEMINAZIONE As String = "N"
                        Dim P_TIPO_ORIGINE As String = "E"
                        Dim P_CODICE_PARTITE_ANIMO As String = ""
                        Dim P_CODICE_STATI As String = Stato_Codice
                        Dim P_ALLEV_ID_FISCALE As String = Allev_IdFiscale
                        Dim P_AZIENDA_CODICE As String = CodiceAzienda_BDN
                        Dim P_MARCHE_PRODOTTE_CODICE As String = ""
                        Dim P_CODICE_LIBRO As String = ""
                        Dim P_ID_FISCALE_DETEN As String = ""
                        Dim P_CODICE_AZIENDA_DETEN As String = ""
                        Dim P_CODICE_SPECIE_DETEN As String = Codice_Specie
                        Dim P_DT_INGRESSO_DETEN As String = CDate(capoAnimale("Validita_Inizio")).ToString("yyyy-MM-dd")
                        Dim P_ASL_CODICE As String = ""
                        Dim P_DISTRETTO_CODICE As String = ""
                        Dim P_DT_INGRESSO As String = CDate(capoAnimale("Validita_Inizio")).ToString("yyyy-MM-dd")
                        Dim P_MOTIVO_INGRESSO As String = "E"
                        Dim P_DT_RILASCIO_PASSAPORTO As String = "2022-09-16"

                        Dim P_TIPO_PASSAPORTO As String = ""
                        If Stato_Codice = "IT" Then
                            P_TIPO_PASSAPORTO = "IT"
                        ElseIf Stato_Codice <> "" Then
                            P_TIPO_PASSAPORTO = "UE"
                        ElseIf Stato_Codice = "" Then
                            P_TIPO_PASSAPORTO = "NP"
                        End If

                        Dim P_NUMERO_RIF_LOCALE As String = capoAnimale("Certificato")
                        Dim P_ID_FISCALE_MARCATORE As String = ""
                        Dim P_COD_MADRE_GENETICA As String = ""
                        Dim P_RIF_MODELLO As String = ""
                        Dim P_DT_MODELLO As String = "2022-09-16"
                        Dim P_CODICE_AZIENDA_PROV As String = ""
                        Dim P_ID_FISCALE_PROV As String = ""
                        Dim P_CODICE_SPECIE_PROV As String = ""
                        Dim P_CODICE_FM_PROV As String = ""
                        Dim P_CODICE_STATI_ORIGINE As String = Stato_Codice

                        If (IsDBNull(capoAnimale("Modello4_Ingresso_Numero")) OrElse capoAnimale("Modello4_Ingresso_Numero") = "") Then
                            P_NUMERO_RIF_LOCALE = capoAnimale("Certificato")
                            P_CODICE_PARTITE_ANIMO = capoAnimale("Certificato")
                        Else
                            P_RIF_MODELLO = capoAnimale("Modello4_Ingresso_Numero")
                            P_DT_MODELLO = capoAnimale("Data_Documento_Ingresso")
                        End If

                        'MODIFICA in BDN
                        Dim dsCapoModificato As DataSet = Me.wsChiamawsRegistroCapiStalla.Update_Capi(P_CAPO_ID,
                                                                                                      P_INS_VAR,
                                                                                                      P_CAPO_CODICE_IDENTIFICATIVO,
                                                                                                      P_SESSO,
                                                                                                      P_RAZZA_CODICE,
                                                                                                      P_DT_NASCITA,
                                                                                                      P_COD_MARCHIO_MADRE,
                                                                                                      P_COD_MARCHIO_PADRE,
                                                                                                      P_TAG,
                                                                                                      P_CODICE_PRECEDENTE,
                                                                                                      P_DT_INIZIO_LATTAZIONE,
                                                                                                      P_DT_FINE_LATTAZIONE,
                                                                                                      P_DT_APPLICAZIONE_MARCHIO,
                                                                                                      P_DT_ISCRIZIONE_IN_ANAGRAFE,
                                                                                                      P_DT_COMPILAZIONE_CEDOLA,
                                                                                                      P_FLAG_INSEMINAZIONE,
                                                                                                      P_TIPO_ORIGINE,
                                                                                                      P_CODICE_PARTITE_ANIMO,
                                                                                                      P_CODICE_STATI,
                                                                                                      P_ALLEV_ID_FISCALE,
                                                                                                      P_AZIENDA_CODICE,
                                                                                                      P_MARCHE_PRODOTTE_CODICE,
                                                                                                      P_CODICE_LIBRO,
                                                                                                      Codice_Specie,
                                                                                                      P_ID_FISCALE_DETEN,
                                                                                                      P_CODICE_AZIENDA_DETEN,
                                                                                                      P_CODICE_SPECIE_DETEN,
                                                                                                      P_DT_INGRESSO_DETEN,
                                                                                                      P_ASL_CODICE,
                                                                                                      P_DISTRETTO_CODICE,
                                                                                                      P_DT_INGRESSO,
                                                                                                      P_MOTIVO_INGRESSO,
                                                                                                      P_DT_RILASCIO_PASSAPORTO,
                                                                                                      P_TIPO_PASSAPORTO,
                                                                                                      P_NUMERO_RIF_LOCALE,
                                                                                                      P_ID_FISCALE_MARCATORE,
                                                                                                      P_COD_MADRE_GENETICA,
                                                                                                      P_RIF_MODELLO,
                                                                                                      P_DT_MODELLO,
                                                                                                      P_CODICE_AZIENDA_PROV,
                                                                                                      P_ID_FISCALE_PROV,
                                                                                                      P_CODICE_SPECIE_PROV,
                                                                                                      P_CODICE_FM_PROV,
                                                                                                      P_CODICE_STATI_ORIGINE)

                        'controlla se è stata effettuata la modifica
                        If Not IsNothing(dsCapoModificato) AndAlso dsCapoModificato.Tables.Count > 0 Then
                            Dim Capo_IdBDN As String = ""
                            dtAnimaleBDN = New DataTable

                            For Each table In dsCapoModificato.Tables

                                If Not IsNothing(table) AndAlso
                                    table.Rows.Count > 0 AndAlso
                                    table.TableName = "CAPI_BOVINI" Then
                                    Dim dtCapoModificato As DataTable = table

                                    Matricola = dtCapoModificato(0)("CODICE")
                                    Capo_IdBDN = dtCapoModificato(0)("CAPO_ID")

                                    dtAnimaleBDN = wsAnagraficaCapo.getCapo(Matricola)

                                    'se il capo è stato aggiunto correttamente si salva la sua Matricola (o P_CAPO_CODICE)
                                    If Not IsNothing(dtAnimaleBDN) OrElse dtAnimaleBDN.Rows.Count > 0 Then
                                        listaMatricole_CapiModificati.Add(Matricola)

                                    End If

                                End If

                            Next

                        End If

                    Catch ex As BDNException
                        Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)

                    Catch ex As Exception
                        Exit Sub

                    End Try

                End If

            Next

        End If

    End Sub

    ''' <summary>
    ''' Eliminazione capi animali in BDN
    ''' </summary>
    ''' <param name="chiave">Piva_SaCod_CodAnimale</param>
    ''' <returns></returns>
    Public Sub EliminaCapiBDN(ByVal chiave As String)
        Dim chiaveArr() As String = JsonConvert.DeserializeObject(Of String())(chiave)

        'ricavo Piva e Sa_Cod della Stalla dal primo elemento
        Dim Piva As String = chiaveArr(0).Split("_")(0)
        Dim Sa_Cod As Integer = chiaveArr(0).Split("_")(1)
        Dim Allev_IdFiscale As String = ""
        Dim CodiceAzienda_BDN As String = ""
        Dim Codice_Specie As String = ""

        Dim listaCodAnimale_CapiEliminazione As New List(Of Integer)
        For Each capo In chiaveArr
            listaCodAnimale_CapiEliminazione.Add(capo.Split("_")(2))
        Next

        'ricava le info da DB dei capi in eliminazione
        Dim objZooAnimale As New AgronicaCoreAnagrafeDAL.Zoo_Animali
        Dim dtAnimaleDB As DataTable = objZooAnimale.BDN_Leggi_Movimenti_Scarico_Non_Sincronizzati(Piva, 0, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, False, listaCodAnimale_CapiEliminazione, objParametriServer)

        Dim listaMatricole_CapiEliminati As New List(Of String)
        If Not IsNothing(dtAnimaleDB) OrElse dtAnimaleDB.Rows.Count > 0 Then
            For Each capoAnimale As DataRow In dtAnimaleDB.Rows

                Dim Cod_Animale As Integer = capoAnimale("Cod_Animale")
                Dim Matricola As String = dtAnimaleDB(0)("Matricola")
                'Allev_IdFiscale = dtAnimaleDB(0)("CUAA")
                'CodiceAzienda_BDN = dtAnimaleDB(0)("BDN_Codice_Azienda")

                ''ricava il SPECIE_CODICE dalla tab di mappatura CodificaBDN_SpecieAnimali
                'Dim codificaBdnSpecie_R As New AgronicaCoreMetaSchemaDAL.Codifica_BDN_SpecieAnimali
                'Dim dtSpecieFromCodificaBDN As DataTable = codificaBdnSpecie_R.leggi(objParametriServer, "", "",
                '                                                                     dtAnimaleDB(0)("GEN_COD"),
                '                                                                     dtAnimaleDB(0)("SPE_COD"),
                '                                                                     3)

                'If Not IsNothing(dtSpecieFromCodificaBDN) AndAlso dtSpecieFromCodificaBDN.Rows.Count > 0 Then
                '    Codice_Specie = "0" & dtSpecieFromCodificaBDN(0)("CODICE")
                'End If

                'controllo che il capo sia presente in BDN
                Dim CapoPresenteBDN As Boolean = True
                Dim dtAnimaleBDN As New DataTable

                Try
                    dtAnimaleBDN = wsAnagraficaCapo.getCapo(dtAnimaleDB(0)("Matricola"))

                Catch ex As Exception

                    If ex.Message.Split(">")(1) = "CODICE CAPO BOVINO NON PRESENTE IN ANAGRAFE" Then
                        CapoPresenteBDN = False
                    Else
                        Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
                    End If

                End Try

                'se è presente in BDN, procede all'eliminazione
                If CapoPresenteBDN AndAlso Not IsNothing(dtAnimaleBDN) AndAlso dtAnimaleBDN.Rows.Count > 0 Then

                    Dim P_CAPO_ID As String = dtAnimaleBDN.Rows(0)("CAPO_ID")
                    Dim P_CAPO_CODICE_IDENTIFICATIVO As String = dtAnimaleBDN.Rows(0)("CODICE")
                    Dim P_ALLEV_ID_FISCALE As String = Allev_IdFiscale
                    Dim P_AZIENDA_CODICE As String = CodiceAzienda_BDN
                    Dim P_CODICE_SPECIE As String = Codice_Specie

                    'ELIMINAZIONE in BDN
                    Dim dsCapoEliminato As DataSet = Me.wsChiamawsRegistroCapiStalla.Delete_Capi(P_CAPO_ID,
                                                                                                   P_CAPO_CODICE_IDENTIFICATIVO,
                                                                                                   P_ALLEV_ID_FISCALE,
                                                                                                   P_AZIENDA_CODICE,
                                                                                                   P_CODICE_SPECIE)

                    'controlla se è stato effettuato l'eliminazione
                    If Not IsNothing(dsCapoEliminato) AndAlso dsCapoEliminato.Tables.Count > 0 Then

                        For Each table As DataTable In dsCapoEliminato.Tables
                            If Not IsNothing(table) AndAlso
                                table.Rows.Count > 0 AndAlso
                                table.TableName = "CAPI_BOVINI" Then

                                'se il capo è stato eliminato correttamente, genera un'eccezione dicendo che non esiste piu in BDN
                                Try
                                    dtAnimaleBDN = wsAnagraficaCapo.getCapo(Matricola)

                                Catch ex As Exception

                                    If ex.Message.Split(">")(1) = "CODICE CAPO BOVINO NON PRESENTE IN ANAGRAFE" Then
                                        CapoPresenteBDN = False
                                    Else
                                        Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
                                    End If

                                End Try

                                'se non è piu presente in BDN, salva la matricola del capo
                                If Not CapoPresenteBDN Then
                                    listaMatricole_CapiEliminati.Add(Matricola)
                                End If

                            End If

                        Next

                    End If

                End If

            Next

        End If

    End Sub

    ''' <summary>
    ''' Sincronizzazione in BDN con scarichi capi animali 
    ''' </summary>
    ''' <param name="chiave">Piva_SaCod_CodAnimale</param>
    ''' <returns></returns>
    Public Function ScaricaCapiBDN(ByVal chiave As List(Of ChiaveCaricoScaricoCapi)) As List(Of InviaUscitaResponse)
        Dim listaUsciteResponse As New List(Of InviaUscitaResponse)

        'ricavo Piva e Sa_Cod della Stalla dal primo elemento
        Dim Piva As String = chiave(0).Piva
        Dim Sa_Cod As Integer = chiave(0).Sa_Cod

        Dim listaMatricole_CapiEliminati As New List(Of String)
        Dim pathsPdf As New List(Of String)
        For Each animale In chiave

            Dim objZooAnimale As New AgronicaCoreAnagrafeDAL.Zoo_Animali

            Dim dtAnimaleDB As DataTable = objZooAnimale.BDN_Leggi_Movimenti_Scarico_Non_Sincronizzati(Piva, 0,
                                                                                                       0, 0,
                                                                                                       animale.Cod_Progetto,
                                                                                                       AGRODATAINIZIO,
                                                                                                       AGRODATAFINE,
                                                                                                       False,
                                                                                                       Nothing,
                                                                                                       objParametriServer)
            Dim capoAnimale As DataRow = dtAnimaleDB.Rows(0)
            Dim Lav_Cod As Integer = capoAnimale("Lav_Cod")

            Dim resp As New InviaUscitaResponse

            Select Case Lav_Cod
                Case LAVCOD_MORTE_ANIMALI
                    resp = Invia_Decessi(Piva, animale, capoAnimale)
                Case LAVCOD_VENDITA_ANIMALI, LAVCOD_TRASFERIMENTO_ANIMALI
                    resp = Invia_Uscita_Allevamento(Piva, animale, capoAnimale, Lav_Cod)
                    'resp = RegistraModello4_Uscita(Piva, Sa_Cod, capoAnimale("Id_Agenda"), capoAnimale("Cod_Progetto"))
                Case LAVCOD_MACELLAZIONE_ANIMALI
                    resp = Invia_Uscita_Macello(Piva, animale, capoAnimale)
                    'resp = RegistraModello4_Uscita(Piva, Sa_Cod, capoAnimale("Id_Agenda"), capoAnimale("Cod_Progetto"))
            End Select

            resp.Matricola = capoAnimale("Matricola")
            listaUsciteResponse.Add(resp)
            GiasContext.SaveChanges()
        Next

        Return listaUsciteResponse

    End Function

    ''' <summary>
    ''' Genera lo scarico per Morte animale su BDN
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="animale"></param>
    ''' <param name="capoAnimale"></param>
    ''' <returns></returns>
    Public Function Invia_Decessi(ByVal Piva As String,
                                  ByVal animale As ChiaveCaricoScaricoCapi,
                                  ByVal capoAnimale As DataRow) As InviaUscitaResponse
        Dim response As New InviaUscitaResponse

        Dim Codice_Specie = ""
        Dim Cod_Animale As Integer = capoAnimale("Cod_Progetto")
        Dim Matricola As String = capoAnimale("Matricola")
        Dim Allev_IdFiscale As String = capoAnimale("CF_PROPRIETARIO")
        Dim CodiceAzienda_BDN As String = capoAnimale("BDN_Codice_Azienda")
        Dim causaleMorte As Integer = capoAnimale("Causale_Morte")

        Dim codificaBdnSpecie_R As New AgronicaCoreMetaSchemaDAL.Codifica_BDN_SpecieAnimali
        Dim dtSpecieFromCodificaBDN As DataTable = codificaBdnSpecie_R.leggi(objParametriServer, "", "",
                                                                             capoAnimale("GEN_COD"),
                                                                             capoAnimale("SPE_COD"),
                                                                             3)

        If Not IsNothing(dtSpecieFromCodificaBDN) AndAlso dtSpecieFromCodificaBDN.Rows.Count > 0 Then
            Codice_Specie = "0" & dtSpecieFromCodificaBDN(0)("CODICE")
        End If

        Try
            Dim idIngresso As Integer = sincronizzatoreAnimale.trovaID_Ingresso(Allev_IdFiscale, CodiceAzienda_BDN, Matricola)
            Dim Data_Operazione As Date = capoAnimale("Data_Movimento")
            Dim Causale_Morte As String = (From c In GiasContext.Lista_Causali_Morte Where c.Cod = causaleMorte Select c.Codice_BDN).FirstOrDefault

            If Causale_Morte Is Nothing OrElse Causale_Morte = "" Then
                Throw New GiasException("Causale Morte non impostata per il capo " & Matricola)
            End If

            Dim risposta = wsChiamawsRegistroCapiStalla.Insert_Decessi(0, "I", idIngresso,
                                                                       Allev_IdFiscale, CodiceAzienda_BDN,
                                                                       Codice_Specie, Matricola,
                                                                       Causale_Morte, Data_Operazione.ToString("yyyy-MM-dd"),
                                                                       AGRODATAINIZIO.ToString("yyyy-MM-dd"),
                                                                       Date.Now.ToString("yyyy-MM-dd"))
            If Not IsNothing(risposta) AndAlso risposta.Tables.Count > 0 Then
                Dim idUscita As Integer = sincronizzatoreAnimale.trovaID_Uscita(Allev_IdFiscale, CodiceAzienda_BDN, Matricola)

                If idUscita <> 0 Then
                    Dim PivaScarico As String = capoAnimale("Piva")
                    Dim ID_AgendaScarico As Integer = capoAnimale("id_agenda")
                    Dim ID_MovScarico As Integer = capoAnimale("id_mov")
                    Dim ID_Mov_DetScarico As Integer = capoAnimale("id_Mov_Det")
                    Dim lavCod As Integer = LAVCOD_MORTE_ANIMALI
                    Dim movDettaglioScarico = (From g In GiasContext.Movimenti_dettagli
                                               Where g.PIVA = PivaScarico AndAlso g.Id_Agenda = ID_AgendaScarico AndAlso
                                               g.Id_Mov = ID_MovScarico AndAlso g.Id_Mov_Det = ID_Mov_DetScarico).FirstOrDefault

                    Dim cod_progetto = movDettaglioScarico.Cod_Progetto
                    Dim idCapoBDN As Integer = GiasContext.Zoo_Animali.Where(Function(z) z.Cod_Progetto = cod_progetto).FirstOrDefault.Id_Capo_BDN

                    movDettaglioScarico.Id_Mov_Esterno = idUscita
                    GiasContext.Entry(movDettaglioScarico).State = EntityState.Modified

                    GiasContext.SaveChanges()

                    logMovimentazioniBDN(Piva, movDettaglioScarico.Sa_Cod, ID_AgendaScarico, lavCod, enumCausaliBDN.InvMorteBDN,
                                         idUscita, ID_MovScarico, ID_Mov_DetScarico, Cod_Animale,
                                         idCapoBDN, Nothing, Nothing, True, GiasContext)

                    response.importato = "SI"
                    response.errore = ""

                Else
                    logMovimentazioniBDN(Piva, 0, 0, 0, enumCausaliBDN.InvMorteBDN,
                                         0, 0, 0, Cod_Animale, 0, Nothing,
                                         Nothing, False, GiasContext)
                End If
            Else
                logMovimentazioniBDN(Piva, 0, 0, 0, enumCausaliBDN.InvMorteBDN,
                                     0, 0, 0, Cod_Animale, 0, Nothing,
                                     Nothing, False, GiasContext)

                response.importato = "NO"
                response.errore = "Errore nella sincronizzazione con la BDN"
            End If

        Catch ex As TokenBDNException
            response.importato = "NO"
            response.errore = ex.Message
            Throw ex
        Catch ex As GiasException
            response.importato = "NO"
            response.errore = ex.Message
        Catch ex As BDNException
            response.importato = "NO"
            response.errore = ex.Message
        Catch ex As Exception
            response.importato = "NO"
            response.errore = ex.Message
        End Try

        Return response

    End Function

    ''' <summary>
    ''' Genera lo scarico per la Vendita/Trasferimento in BDN
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="animale"></param>
    ''' <param name="capoAnimale"></param>
    ''' <returns></returns>
    Public Function Invia_Uscita_Allevamento(ByVal Piva As String,
                                  ByVal animale As ChiaveCaricoScaricoCapi,
                                  ByVal capoAnimale As DataRow, Lav_Cod As Integer) As InviaUscitaResponse
        Dim response As New InviaUscitaResponse

        Dim Codice_Specie = ""

        Dim Sa_Cod As Integer = capoAnimale("Sa_Cod")
        Dim Fabbricato_Cod As Integer = capoAnimale("STA_NUM")
        Dim Cod_Animale As Integer = capoAnimale("Cod_Progetto")
        Dim Matricola As String = capoAnimale("Matricola")
        Dim Allev_IdFiscale As String = capoAnimale("CF_PROPRIETARIO")
        Dim CodiceAzienda_BDN As String = capoAnimale("BDN_Codice_Azienda")
        'Dim causaleMorte As Integer = capoAnimale("Causale_Morte")

        Dim codificaBdnSpecie_R As New AgronicaCoreMetaSchemaDAL.Codifica_BDN_SpecieAnimali
        Dim dtSpecieFromCodificaBDN As DataTable = codificaBdnSpecie_R.leggi(objParametriServer, "", "",
                                                                             capoAnimale("GEN_COD"),
                                                                             capoAnimale("SPE_COD"),
                                                                             3)

        If Not IsNothing(dtSpecieFromCodificaBDN) AndAlso dtSpecieFromCodificaBDN.Rows.Count > 0 Then
            Codice_Specie = "0" & dtSpecieFromCodificaBDN(0)("CODICE")
        End If

        Try
            Dim idIngresso As Integer = sincronizzatoreAnimale.trovaID_Ingresso(Allev_IdFiscale, CodiceAzienda_BDN, Matricola)
            Dim Data_Operazione As Date = capoAnimale("Data_Movimento")
            'Dim Causale_Morte As String = (From c In GiasContext.Lista_Causali_Morte Where c.Cod = causaleMorte Select c.Codice_BDN).FirstOrDefault

            'If Causale_Morte Is Nothing Or Causale_Morte = "" Then
            '	Throw New GiasException("Causale Morte non impostata per il capo " & Matricola)
            'End If
            Dim prenotazioneModello4 As String = capoAnimale("Modello4_Uscita_Prenotazione")
            Dim objModelli4 As CaricaModelli4_Response = wsInterrogazioniModello4.caricaModello4(prenotazioneModello4, Codice_Specie)

            Dim objFabbricati_r As New AgronicaCoreAnagrafeBIZ.Fabbricato_R

            Dim Codice_ASL As String = objFabbricati_r.TrovaCodiceASLFabbricato(Piva, Sa_Cod, Fabbricato_Cod, objParametriServer)
            Dim Codice_Regione As String = objModelli4.regione_codice
            Dim Codice_Macello As String = objModelli4.codAzienda_Dest
            Dim Id_Fiscale_Macello As String = objModelli4.idFiscaleAzienda_Dest
            Dim rif_modello As String = objModelli4.numModello
            Dim data_modello As Date = objModelli4.dataDocumento
            Dim risposta = wsChiamawsRegistriDiStalla.Insert_Uscita_Allevamento(0, "I", idIngresso,
                                                                                  Codice_ASL, "",
                                                                                  Matricola, Codice_Specie,
                                                                                  CodiceAzienda_BDN, Allev_IdFiscale,
                                                                                  "V", Data_Operazione.ToString("yyyy-MM-dd"),
                                                                                  Date.Now.ToString("yyyy-MM-dd"),
                                                                                  Data_Operazione.ToString("yyyy-MM-dd"),
                                                                                  Codice_Specie, Codice_Macello,
                                                                                  Id_Fiscale_Macello, "",
                                                                                  rif_modello, data_modello.ToString("yyyy-MM-dd"), "")
            If Not IsNothing(risposta) AndAlso risposta.Tables.Count > 0 Then
                Dim idUscita As Integer = sincronizzatoreAnimale.trovaID_Uscita(Allev_IdFiscale, CodiceAzienda_BDN, Matricola)

                If idUscita <> 0 Then
                    Dim PivaScarico As String = capoAnimale("Piva")
                    Dim ID_AgendaScarico As Integer = capoAnimale("id_agenda")
                    Dim ID_MovScarico As Integer = capoAnimale("id_mov")
                    Dim ID_Mov_DetScarico As Integer = capoAnimale("id_Mov_Det")
                    Dim movDettaglioScarico = (From g In GiasContext.Movimenti_dettagli
                                               Where g.PIVA = PivaScarico AndAlso g.Id_Agenda = ID_AgendaScarico AndAlso
                                               g.Id_Mov = ID_MovScarico AndAlso g.Id_Mov_Det = ID_Mov_DetScarico).FirstOrDefault

                    Dim cod_progetto = movDettaglioScarico.Cod_Progetto

                    Dim idCapoBDN As Integer = GiasContext.Zoo_Animali.Where(Function(z) z.Cod_Progetto = cod_progetto).FirstOrDefault.Id_Capo_BDN

                    movDettaglioScarico.Id_Mov_Esterno = idUscita
                    GiasContext.Entry(movDettaglioScarico).State = EntityState.Modified

                    GiasContext.SaveChanges()

                    logMovimentazioniBDN(Piva, movDettaglioScarico.Sa_Cod, ID_AgendaScarico, Lav_Cod, enumCausaliBDN.InvMovUscitaModello4,
                                         idUscita, ID_MovScarico, ID_Mov_DetScarico, Cod_Animale,
                                         idCapoBDN, Nothing, Nothing, True, GiasContext)

                    response.importato = "SI"
                    response.errore = ""

                Else
                    logMovimentazioniBDN(Piva, 0, 0, 0, enumCausaliBDN.InvMovUscitaModello4,
                                         0, 0, 0, Cod_Animale, 0, Nothing,
                                         Nothing, False, GiasContext)
                End If
            Else
                logMovimentazioniBDN(Piva, 0, 0, 0, enumCausaliBDN.InvMovUscitaModello4,
                                     0, 0, 0, Cod_Animale, 0, Nothing,
                                     Nothing, False, GiasContext)

                response.importato = "NO"
                response.errore = "Errore nella sincronizzazione con la BDN"
            End If

        Catch ex As TokenBDNException
            response.importato = "NO"
            response.errore = ex.Message
            Throw ex
        Catch ex As GiasException
            response.importato = "NO"
            response.errore = ex.Message
        Catch ex As BDNException
            response.importato = "NO"
            response.errore = ex.Message
        Catch ex As Exception
            response.importato = "NO"
            response.errore = ex.Message
        End Try

        Return response

    End Function

    ''' <summary>
    ''' Genera lo scarico per la Macellazione in BDN
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="animale"></param>
    ''' <param name="capoAnimale"></param>
    ''' <returns></returns>
    Public Function Invia_Uscita_Macello(ByVal Piva As String,
                                  ByVal animale As ChiaveCaricoScaricoCapi,
                                  ByVal capoAnimale As DataRow) As InviaUscitaResponse
        Dim response As New InviaUscitaResponse

        Dim Codice_Specie = ""

        Dim Sa_Cod As Integer = capoAnimale("Sa_Cod")
        Dim Fabbricato_Cod As Integer = capoAnimale("STA_NUM")

        Dim Cod_Animale As Integer = capoAnimale("Cod_Progetto")
        Dim Matricola As String = capoAnimale("Matricola")
        Dim Allev_IdFiscale As String = capoAnimale("CF_PROPRIETARIO")
        Dim CodiceAzienda_BDN As String = capoAnimale("BDN_Codice_Azienda")
        'Dim causaleMorte As Integer = capoAnimale("Causale_Morte")

        Dim codificaBdnSpecie_R As New AgronicaCoreMetaSchemaDAL.Codifica_BDN_SpecieAnimali
        Dim dtSpecieFromCodificaBDN As DataTable = codificaBdnSpecie_R.leggi(objParametriServer, "", "",
                                                                             capoAnimale("GEN_COD"),
                                                                             capoAnimale("SPE_COD"),
                                                                             3)

        If Not IsNothing(dtSpecieFromCodificaBDN) AndAlso dtSpecieFromCodificaBDN.Rows.Count > 0 Then
            Codice_Specie = "0" & dtSpecieFromCodificaBDN(0)("CODICE")
        End If

        Try
            'Dim idIngresso As Integer = sincronizzatoreAnimale.trovaID_Ingresso(Allev_IdFiscale, CodiceAzienda_BDN, Matricola)
            Dim Data_Operazione As Date = capoAnimale("Data_Movimento")
            'Dim Causale_Morte As String = (From c In GiasContext.Lista_Causali_Morte Where c.Cod = causaleMorte Select c.Codice_BDN).FirstOrDefault

            Dim prenotazioneModello4 As String = capoAnimale("Modello4_Uscita_Prenotazione")
            If prenotazioneModello4 = "" Then
                Throw New GiasException("Non è stato trovato il modello 4 di uscita per la matricola " & Matricola)
            End If
            Dim objModelli4 As CaricaModelli4_Response = wsInterrogazioniModello4.caricaModello4(prenotazioneModello4, Codice_Specie)

            Dim objFabbricati_r As New AgronicaCoreAnagrafeBIZ.Fabbricato_R

            Dim Codice_ASL As String = objFabbricati_r.TrovaCodiceASLFabbricato(Piva, Sa_Cod, Fabbricato_Cod, objParametriServer)
            Dim Codice_Regione As String = objModelli4.regione_codice
            Dim Codice_Macello As String = objModelli4.codAzienda_Dest
            Dim Id_Fiscale_Macello As String = objModelli4.idFiscaleAzienda_Dest
            Dim rif_modello As String = objModelli4.numModello
            Dim data_modello As Date = objModelli4.dataDocumento

            Dim numModello As String = rif_modello.Substring(Math.Max(0, rif_modello.Length - 5))

            Dim risposta = wsChiamawsRegistriDiStalla.Insert_Uscita_Macello(0,
                                                                              "I",
                                                                              0,
                                                                              Codice_ASL,
                                                                              "",
                                                                              Matricola,
                                                                              Codice_Specie,
                                                                              CodiceAzienda_BDN,
                                                                              Allev_IdFiscale,
                                                                              "M", Data_Operazione.ToString("yyyy-MM-dd"),
                                                                              Date.Now.ToString("yyyy-MM-dd"),
                                                                              Codice_Specie,
                                                                              Codice_Regione,
                                                                              Codice_Macello,
                                                                              Id_Fiscale_Macello,
                                                                              "", numModello, data_modello.ToString("yyyy-MM-dd"), "")

            If Not IsNothing(risposta) AndAlso risposta.Tables.Count > 0 Then
                Dim idUscita As Integer = sincronizzatoreAnimale.trovaID_Uscita(Allev_IdFiscale, CodiceAzienda_BDN, Matricola)

                If idUscita <> 0 Then
                    Dim PivaScarico As String = capoAnimale("Piva")
                    Dim ID_AgendaScarico As Integer = capoAnimale("id_agenda")
                    Dim ID_MovScarico As Integer = capoAnimale("id_mov")
                    Dim ID_Mov_DetScarico As Integer = capoAnimale("id_Mov_Det")
                    Dim lavCod As Integer = LAVCOD_MACELLAZIONE_ANIMALI
                    Dim movDettaglioScarico = (From g In GiasContext.Movimenti_dettagli
                                               Where g.PIVA = PivaScarico AndAlso g.Id_Agenda = ID_AgendaScarico AndAlso
                                               g.Id_Mov = ID_MovScarico AndAlso g.Id_Mov_Det = ID_Mov_DetScarico).FirstOrDefault
                    Dim cod_progetto = movDettaglioScarico.Cod_Progetto

                    Dim idCapoBDN As Integer = GiasContext.Zoo_Animali.Where(Function(z) z.Cod_Progetto = cod_progetto).FirstOrDefault.Id_Capo_BDN

                    movDettaglioScarico.Id_Mov_Esterno = idUscita
                    GiasContext.Entry(movDettaglioScarico).State = EntityState.Modified

                    GiasContext.SaveChanges()

                    logMovimentazioniBDN(Piva, movDettaglioScarico.Sa_Cod, ID_AgendaScarico, lavCod, enumCausaliBDN.InvMovUscitaModello4,
                                         idUscita, ID_MovScarico, ID_Mov_DetScarico, Cod_Animale,
                                         idCapoBDN, Nothing, Nothing, True, GiasContext)

                    response.importato = "SI"
                    response.errore = ""

                Else
                    logMovimentazioniBDN(Piva, 0, 0, 0, enumCausaliBDN.InvMovUscitaModello4,
                                         0, 0, 0, Cod_Animale, 0, Nothing,
                                         Nothing, False, GiasContext)
                End If
            Else
                logMovimentazioniBDN(Piva, 0, 0, 0, enumCausaliBDN.InvMovUscitaModello4,
                                     0, 0, 0, Cod_Animale, 0, Nothing,
                                     Nothing, False, GiasContext)

                response.importato = "NO"
                response.errore = "Errore nella sincronizzazione con la BDN"
            End If

        Catch ex As TokenBDNException
            response.importato = "NO"
            response.errore = ex.Message
            Throw ex
        Catch ex As GiasException
            response.importato = "NO"
            response.errore = ex.Message
        Catch ex As BDNException
            response.importato = "NO"
            response.errore = ex.Message
        Catch ex As Exception
            response.importato = "NO"
            response.errore = ex.Message
        End Try

        Return response

    End Function

    ''' <summary>
    ''' Inserisce un documento di uscita modello 4 ricavando i capi e i dati 
    ''' dalle operazioni d'agenda passati (e relativi movimenti)
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="AllevIdFiscale_Prov"></param>
    ''' <param name="registraModello"></param>
    ''' <param name="Operazioni_Agenda"></param>
    ''' <returns></returns>
    Public Function InserisciModello4(ByVal Piva As String,
                                      ByVal Sa_Cod As Integer,
                                      ByVal AllevIdFiscale_Prov As String,
                                      ByVal registraModello As Boolean,
                                      ByVal Operazioni_Agenda As List(Of Integer)) As List(Of InserisciModello4Response)
        Dim listaModelliInviati_PrenotazioneId As New List(Of String)
        Dim listaModelli4Response As New List(Of InserisciModello4Response)
        'un modello4 per ogni operazione d'agenda
        For Each idAgenda In Operazioni_Agenda
            Dim modello4Response As New InserisciModello4Response

            Try
                Dim Agenda As AgronicaCoreEntityFramework_POCO.Agenda = (From a In GiasContext.Agenda
                                                                         Where a.PIVA = Piva AndAlso a.Id_Agenda = idAgenda).FirstOrDefault
                Dim Lav_Cod As Integer = Agenda.Lav_Cod

                If IsNothing(Agenda) Then
                    Throw New Exception($"Operazione zootecnica (Id_Agenda={idAgenda}) non valida o inesistente")
                End If

                Dim Movimenti_Scarico As AgronicaCoreEntityFramework_POCO.Movimenti = (From m In GiasContext.Movimenti
                                                                                       Where m.PIVA = Piva AndAlso m.Id_Agenda = idAgenda AndAlso
                                                                                           m.Cau_Mov = CAU_SCARICO_CONSISTENZE).FirstOrDefault
                If IsNothing(Movimenti_Scarico) Then
                    Throw New Exception($"Operazione zootecnica (Id_Agenda={idAgenda}) non valida o inesistente")
                End If
                Dim idMov_Scarico As Integer = Movimenti_Scarico.Id_Mov

                Dim Movimenti_Registrazione As AgronicaCoreEntityFramework_POCO.Movimenti = (From m In GiasContext.Movimenti
                                                                                             Where m.PIVA = Piva AndAlso m.Id_Agenda = idAgenda AndAlso
                                                                                                 m.Cau_Mov = CAU_REGISTRAZIONI).FirstOrDefault
                If IsNothing(Movimenti_Registrazione) Then
                    Throw New Exception($"Operazione zootecnica (Id_Agenda={idAgenda}) non valida o inesistente")
                End If
                Dim idMov_Registrazione As Integer = Movimenti_Registrazione.Id_Mov

                Dim MovDett_Scarico As List(Of AgronicaCoreEntityFramework_POCO.Movimenti_dettagli) = (From md In GiasContext.Movimenti_dettagli
                                                                                                       Where md.PIVA = Piva AndAlso md.Id_Agenda = idAgenda AndAlso
                                                                                                           md.Id_Mov = idMov_Scarico).ToList
                If IsNothing(MovDett_Scarico) OrElse MovDett_Scarico.Count = 0 Then
                    Throw New Exception($"L'operazione (Id_Agenda={idAgenda}) non contiene capi da scaricare")
                End If

                Dim MovDest_Scarico As List(Of AgronicaCoreEntityFramework_POCO.Mov_Destinazioni) = (From md In GiasContext.Mov_Destinazioni
                                                                                                     Where md.Piva = Piva AndAlso md.Id_Agenda = idAgenda AndAlso
                                                                                                           md.Id_Mov = idMov_Scarico).ToList
                If IsNothing(MovDest_Scarico) OrElse MovDest_Scarico.Count = 0 Then
                    Throw New Exception($"L'operazione (Id_Agenda={idAgenda}) non contiene capi da scaricare")
                End If

                Dim MovDettaglio_TecnEx As AgronicaCoreEntityFramework_POCO.Mov_Dettaglio_Tecnico_Extra = (From a In GiasContext.Mov_Dettaglio_Tecnico_Extra
                                                                                                           Where a.Piva = Piva AndAlso a.Id_Agenda = idAgenda AndAlso
                                                                                                                  a.Id_Mov = idMov_Registrazione).FirstOrDefault
                If IsNothing(MovDettaglio_TecnEx) Then
                    Throw New Exception($"Dati di trasporto per l'operazione (Id_Agenda={idAgenda}) non presenti")
                End If

                '------INIZIO RECUPERO DATI MODELLO 4------

                'CODICE_AZIENDA_BDN PROVENIENZA
                Dim objStalla As New AgronicaCoreAnagrafeDAL.Stalla_R
                Dim raggruppamento_cod = MovDest_Scarico(0).Id_Destinazione
                Dim raggruppamento = (From rr In GiasContext.Stalla_Raggruppamenti Where rr.Raggruppamento_Cod = raggruppamento_cod).FirstOrDefault
                Dim Sta_Num As Integer = raggruppamento.STA_NUM
                If Sa_Cod = 0 Then
                    Sa_Cod = raggruppamento.sa_cod
                End If
                Dim dtStalla As DataTable = objStalla.Leggi(Piva, Sa_Cod, Sta_Num,
                                                            enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                            "", "", objParametriServer)
                If IsNothing(dtStalla) OrElse dtStalla.Rows.Count = 0 Then
                    Throw New Exception("Stalla di provenienza non valida o inesistente")
                End If
                Dim CodiceAziendaBDN_Prov As String = dtStalla(0)("BDN_Codice_Azienda")

                'ALLEV_ID_FISCALE PROVENIENZA
                Dim objImprese_codici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
                'Dim AllevIdFiscale_Prov As String = objImprese_codici.Leggi_CUAA(Piva, objParametriServer)

                'CODICE SPECIE
                Dim objCodificaSpecie As New AgronicaCoreMetaSchemaDAL.Codifica_BDN_SpecieAnimali
                Dim dtCodifica As DataTable = objCodificaSpecie.leggi(objParametriServer, "", "",
                                                                      dtStalla(0)("GEN_COD"), dtStalla(0)("SPE_COD"),
                                                                      "3")
                If IsNothing(dtCodifica) OrElse dtCodifica.Rows.Count = 0 Then
                    Throw New GiasException("Codifica specie mancante per BDN")
                End If
                Dim Codice_Specie As String = "0" & dtCodifica.Rows(0)("CODICE")

                'LISTA COD_PROGETTO CAPI
                Dim lista_CodAnimale As New List(Of Integer)
                For Each movDett In MovDett_Scarico
                    lista_CodAnimale.Add(movDett.Cod_Progetto)
                Next

                'DATA PARTENZA
                Dim dataUscita As Date = Movimenti_Registrazione.Extra_Date

                'DATI DESTINAZIONE
                Dim objContatti_R As New AgronicaCoreAnagrafeDAL.Contatti_R
                Dim dtContatto As New DataTable
                Dim objRisorseUmane_R As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R
                Dim dtRisUm As New DataTable

                Dim tipoDestinazione As Integer = 0
                Select Case Lav_Cod
                    Case LAVCOD_DECREMENTO_CONSISTENZE_ZOO, LAVCOD_VENDITA_ANIMALI, LAVCOD_MORTE_ANIMALI, LAVCOD_TRASFERIMENTO_ANIMALI
                        tipoDestinazione = COD_ALLEVATORE
                    Case LAVCOD_MACELLAZIONE_ANIMALI
                        tipoDestinazione = COD_MACELLO
                End Select

                Dim CodiceAziendaBDN_Dest As String = ""    'CODICE_AZIENDA_BDN DESTINAZIONE
                Dim AllevIdFiscale_Dest As String = ""      'ALLEV_ID_FISCALE DESTINAZIONE

                'If Lav_Cod = LAVCOD_TRASFERIMENTO_ANIMALI Then
                '	Dim chiaveTrasfStalla As String = MovDettaglio_TecnEx.Luogo_Consegna

                '	If chiaveTrasfStalla = "" AndAlso Movimenti_Registrazione.Cod_Destinazione <> 0 Then    'Contatto Allevatore
                '		dtContatto = objContatti_R.Leggi(Movimenti_Registrazione.PIVA, "",
                '										 Movimenti_Registrazione.Cod_Destinazione,
                '										 0, True, False,
                '										 Movimenti_Registrazione.Cod_IndirizzoDestinazione,
                '										 0, False, 0, CInt(-99), 0,
                '										 "", False, 0, 0,
                '										 0, 0, 0, AGRODATAINIZIO,
                '										 AGRODATAFINE, False,
                '										 "", "", objParametriServer)

                '		If IsNothing(dtContatto) OrElse dtContatto.Rows.Count = 0 Then
                '			Throw New GiasException("Stalla di destinazione non valida o inesistente")
                '		End If

                '		Dim piva_Dest As String = dtContatto(0)("Cod_Contatto")

                '		dtRisUm = objRisorseUmane_R.Leggi3(Movimenti_Registrazione.PIVA, piva_Dest,
                '											   0, 0, "", "",
                '											   objParametriServer)

                '		If IsNothing(dtRisUm) OrElse dtRisUm.Rows.Count = 0 Then
                '			dtRisUm = objRisorseUmane_R.Leggi3("", piva_Dest,
                '											   0, 0, "", "",
                '											   objParametriServer)

                '			If IsNothing(dtRisUm) OrElse dtRisUm.Rows.Count = 0 Then
                '				Throw New GiasException("Stalla di destinazione non valida o inesistente")
                '			End If
                '		End If

                '		AllevIdFiscale_Dest = dtContatto(0)("Codice_Fiscale")
                '		CodiceAziendaBDN_Dest = dtRisUm(0)("Attivita_Des")

                '	ElseIf chiaveTrasfStalla <> "" Then     'Stalla nello stesso centro
                '		Dim piva_StallaDest As String = chiaveTrasfStalla.Split("_")(0)
                '		Dim saCod_StallaDest As Integer = chiaveTrasfStalla.Split("_")(1)
                '		Dim staNum_StallaDest As Integer = chiaveTrasfStalla.Split("_")(2)

                '		dtStalla = objStalla.Leggi(piva_StallaDest, saCod_StallaDest, staNum_StallaDest,
                '								   enumSelezioneVariabile.Selezione_JoinDescrizioni,
                '								   "", "", objParametriServer)

                '		If IsNothing(dtStalla) OrElse dtStalla.Rows.Count = 0 Then
                '			Throw New GiasException("Stalla di destinazione non valida o inesistente")
                '		End If

                '		CodiceAziendaBDN_Dest = dtStalla(0)("BDN_Codice_Azienda")
                '		AllevIdFiscale_Dest = (From co In GiasContext.Contatti Where co.Cod_Contatto.Equals(piva_StallaDest)).FirstOrDefault.Codice_Fiscale

                '	End If
                'Else
                dtContatto = objContatti_R.Leggi(Movimenti_Registrazione.PIVA, "",
                                                     Movimenti_Registrazione.Cod_Destinazione,
                                                     0, True, False,
                                                     Movimenti_Registrazione.Cod_IndirizzoDestinazione,
                                                     0, False, 0, CInt(-99), 0,
                                                     "", False, 0, 0,
                                                     0, 0, 0, AGRODATAINIZIO,
                                                     AGRODATAFINE, False,
                                                     "", "", objParametriServer)

                If IsNothing(dtContatto) OrElse dtContatto.Rows.Count = 0 Then
                    Throw New Exception("Stalla di destinazione non valida o inesistente")
                End If

                dtRisUm = objRisorseUmane_R.Leggi3(Piva, dtContatto(0)("Codice_Fiscale"),
                                                   0, 0, "", "",
                                                   objParametriServer)

                If IsNothing(dtRisUm) OrElse dtRisUm.Rows.Count = 0 Then
                    dtRisUm = objRisorseUmane_R.Leggi3("", dtContatto(0)("Cod_Contatto"),
                                                       0, 0, "", "",
                                                       objParametriServer)
                End If

                If IsNothing(dtRisUm) OrElse dtRisUm.Rows.Count = 0 Then
                    Throw New Exception("Stalla di destinazione non valida o inesistente")
                End If

                CodiceAziendaBDN_Dest = dtContatto(0)("Attivita_Des")
                AllevIdFiscale_Dest = dtContatto(0)("Codice_Fiscale")
                Dim CodContattoDestinazione As String = dtContatto(0)("Cod_Contatto")
                Dim PivaContattoDestinazione As String = dtContatto(0)("Piva")

                Dim RagSocContatto As String = ""
                If dtContatto(0)("Rag_Soc") <> "" Then
                    RagSocContatto = CStr(dtContatto(0)("Rag_Soc"))
                End If
                If dtContatto(0)("Nome") <> "" AndAlso dtContatto(0)("Cognome") <> "" Then
                    RagSocContatto = CStr(dtContatto(0)("Cognome")) & " " & CStr(dtContatto(0)("Nome"))
                End If
                'End If

                'DATI GESTIONE TRASPORTO
                Dim Targa As String = MovDettaglio_TecnEx.Targa
                Dim Targa_Rimorchio As String = MovDettaglio_TecnEx.Codice_Alternativo
                Dim NumAutorizzazione As String = MovDettaglio_TecnEx.N_Autorizzazione_Trasporto
                Dim DurataViaggio As String = MovDettaglio_TecnEx.Durata_Viaggio
                Dim FlagTipoTrasporto As String = MovDettaglio_TecnEx.Annotazioni

                'PIVA_ASL TRASPORTATORE
                Dim PivaAsl_Trasp As String = MovDettaglio_TecnEx.Trasportatore
                Dim CodFiscale_Trasp As String = ""
                Dim TraspDenom As String = ""
                dtContatto = objContatti_R.LeggiContattoSpecifico(Movimenti_Registrazione.PIVA, PivaAsl_Trasp,
                                                                  99, enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                  "", "", objParametriServer)

                If IsNothing(dtContatto) OrElse dtContatto.Rows.Count = 0 Then
                    dtContatto = objContatti_R.LeggiContattoSpecifico("", PivaAsl_Trasp,
                                                                      99, enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                      "", "", objParametriServer)
                    If Not IsNothing(dtContatto) AndAlso dtContatto.Rows.Count > 0 Then
                        'CODICE FISCALE TRASPORTATORE
                        CodFiscale_Trasp = dtContatto(0)("Codice_Fiscale")
                        'DENOM TRASPORTATORE
                        TraspDenom = dtContatto(0)("Rag_Soc")
                    End If
                End If

                'CONDUCENTE
                Dim conducente = MovDettaglio_TecnEx.Indicazioni_Complementari
                dtRisUm = objRisorseUmane_R.Leggi3(Movimenti_Registrazione.PIVA, PivaAsl_Trasp,
                                                   0, COD_TRASPORTATORE, "", "",
                                                   objParametriServer)

                If IsNothing(dtRisUm) OrElse dtRisUm.Rows.Count = 0 Then
                    dtRisUm = objRisorseUmane_R.Leggi3("", PivaAsl_Trasp,
                                                       0, COD_TRASPORTATORE, "", "",
                                                       objParametriServer)
                End If

                Dim CodiceAsl_Trasp As String = ""

                If Not IsNothing(dtRisUm) AndAlso dtRisUm.Rows.Count > 0 Then
                    'CODICE_ASL TRASPORTATORE
                    CodiceAsl_Trasp = dtRisUm(0)("Settore_Des")
                    If CodiceAsl_Trasp = "" Then
                        Dim objContatti As New AgronicaCoreAnagrafeBIZ.Indirizzi_R
                        CodiceAsl_Trasp = objContatti.CodiceAslDatoContatto(Piva, PivaAsl_Trasp,
                                                                            objParametriServer)
                    End If
                End If

                'controlla se i capi sono presenti i BDN e ne salva le matricole se presenti
                Dim listaMatricoleCapi As New List(Of String)
                modello4Response.listaAnimali = New List(Of String)
                For Each capo In lista_CodAnimale
                    Dim z As AgronicaCoreEntityFramework_POCO.Zoo_Animali = (From c In GiasContext.Zoo_Animali
                                                                             Select c
                                                                             Where c.Cod_Progetto = capo).First
                    If Not IsNothing(z) Then
                        'controllo che il capo sia presente in BDN
                        Dim CapoPresenteBDN As Boolean = True
                        Dim dtAnimaleBDN As New DataTable

                        Try
                            dtAnimaleBDN = wsAnagraficaCapo.getCapo(z.Matricola)
                        Catch ex As Exception
                            If ex.Message.Split(">")(1) = "CODICE CAPO BOVINO NON PRESENTE IN ANAGRAFE" Then
                                CapoPresenteBDN = False
                                Exit Try
                            Else
                                Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
                            End If
                        End Try

                        If CapoPresenteBDN Then
                            listaMatricoleCapi.Add(z.Matricola)
                            modello4Response.listaAnimali.Add(z.Matricola)
                        End If
                    End If
                Next

                'FLAG MACELLO
                Dim objFlagMacello As New JObject
                If MovDettaglio_TecnEx.Precisazioni <> "" Then
                    objFlagMacello = JsonConvert.DeserializeObject(Of JObject)(MovDettaglio_TecnEx.Precisazioni)
                End If

                ' VETERINARIO
                Dim vetNomeCognome As String = ""
                Dim vetCognomeNome As String = ""
                Dim vetIscrizioneAlbo As String = ""
                Dim vetIndirizzo As String = ""
                Dim vetTelefono As String = ""
                Dim vetComIstat As String = ""
                Dim vetProIstat As String = ""

                Dim cruVeterinario As Integer = MovDettaglio_TecnEx.Cod_RisUm_Extra
                If cruVeterinario <> 0 Then
                    Dim vet = GiasContext.Risorse_Umane _
                        .Where(Function(ru) ru.Cod_Rapporto = COD_VETERINARIO) _
                        .Join(GiasContext.Contatti, Function(ru) ru.Piva, Function(co) co.Piva,
                          Function(ru, co) New With {Key ru, Key co}) _
                        .Where(Function(x) x.co.Cod_Contatto = x.ru.Cod_Contatto AndAlso (x.co.Piva = Piva Or x.co.Sa_Cod = -1)) _
                        .Select(Function(v) New With {
                            Key .Cod_Contatto = v.co.Cod_Contatto,
                            Key .Cognome = v.co.Cognome,
                            Key .Nome = v.co.Nome,
                            Key .Cod_RisUm = v.ru.Cod_RisUm,
                            Key .Settore_Des = v.ru.Settore_Des
                        }).FirstOrDefault()
                    Dim vetCodContatto As String = vet.Cod_Contatto
                    vetNomeCognome = $"{vet.Nome} {vet.Cognome}"
                    vetCognomeNome = $"{vet.Cognome} {vet.Nome}"
                    vetIscrizioneAlbo = vet.Settore_Des

                    Dim indirizzi = GiasContext.ContattiXIndirizzi _
                        .Where(Function(i) i.Cod_Contatto = vetCodContatto) _
                        .Join(GiasContext.Indirizzi, Function(coxin) coxin.Cod_Indirizzo, Function(ind) ind.cod_indirizzo,
                              Function(coxin, ind) New With {Key coxin, Key ind}) _
                        .Join(GiasContext.ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166, Function(x) x.ind.stato, Function(paesi) paesi.Codice,
                              Function(x, paesi) New With {Key x.coxin, Key x.ind, Key paesi}) _
                        .Where(Function(x) x.ind.stato = x.paesi.Codice OrElse x.ind.stato = x.paesi.Descrizione) _
                        .Join(GiasContext.Lista_Province, Function(x) x.ind.pro_cod_istat, Function(province) province.PROV,
                              Function(x, province) New With {Key x.coxin, Key x.ind, Key x.paesi, Key province}) _
                        .Join(GiasContext.ISTAT_Comuni,
                              Function(x) New With {.Com_Cod_Istat = x.ind.com_cod_istat, .Pro_Cod_Istat = x.province.PROV},
                              Function(comuni) New With {.Com_Cod_Istat = comuni.Com_Cod_Istat, .Pro_Cod_Istat = comuni.Pro_Cod_Istat},
                              Function(x, comuni) New With {Key x.coxin, Key x.ind, Key x.paesi, Key x.province, Key comuni}) _
                        .Select(Function(i) New With {
                                Key .Indirizzo_Cod = i.ind.cod_indirizzo,
                                Key .Indirizzo_Des = i.ind.ind_des,
                                Key .Stato = If(i.ind.stato, ""),
                                Key .Pro_Cod_Istat = If(i.ind.pro_cod_istat, ""),
                                Key .Provincia_Sigla = If(i.province.SIGLA, ""),
                                Key .Com_Cod_Istat = If(i.ind.com_cod_istat, "")
                        }).ToList()
                    If indirizzi IsNot Nothing AndAlso indirizzi.Count > 0 Then
                        Dim indirizzo = indirizzi.FirstOrDefault(Function(i) i.Stato = "IT")
                        If indirizzo Is Nothing Then indirizzo = indirizzi.FirstOrDefault()

                        vetIndirizzo = indirizzo.Indirizzo_Des
                        vetComIstat = indirizzo.Com_Cod_Istat
                        vetProIstat = indirizzo.Provincia_Sigla
                    End If

                    vetTelefono = GiasContext.ContattiXRubrica _
                        .Where(Function(coxrub) vet.Cod_Contatto = coxrub.Cod_Contatto) _
                        .Join(GiasContext.Rubrica, Function(coxrub) coxrub.Cod_Rubrica, Function(rub) rub.cod_rubrica,
                              Function(coxrub, rub) New With {Key coxrub, Key rub}) _
                        .Where(Function(x) x.rub.descr.Contains("elefono")) _
                        .Select(Function(x) x.rub.numero) _
                        .FirstOrDefault(Function(x) x <> "" AndAlso x <> "#")
                End If

                '------FINE RECUPERO DATI MODELLO 4------

                'TODO Successivamente all'interno del Modello4 da inserire vanno indicati i trattamenti per ogni capo
                '(al momento viene inserito un unico elemento vuoto)
                Dim listaTrattamentiCapo As New List(Of String)

                'se almeno un capo può essere inviato inizia la creazione del modello4
                If listaMatricoleCapi.Count > 0 Then
                    'ricava l'allevamento di destinazione
                    'Dim dtAllevamento_Dest As DataTable = wsAziende.getAllevamento(CodiceAziendaBDN_Dest, AllevIdFiscale_Dest,
                    '                                                               Codice_Specie)

                    'If IsNothing(dtAllevamento_Dest) OrElse dtAllevamento_Dest.Rows.Count = 0 Then
                    '    dtAllevamento_Dest = wsAziende.FindAllevamento(CodiceAziendaBDN_Dest, "", "")
                    'End If
                    'If IsNothing(dtAllevamento_Dest) OrElse dtAllevamento_Dest.Rows.Count = 0 Then
                    '    Throw New BDNException("Allevamento non presente su BDN")
                    'End If

                    Dim P_PRENOTAZIONE_ID As Integer = 0
                    Dim P_DOCUMENTO_ID As Integer = 0
                    Dim P_AZIENDA_CODICE As String = CodiceAziendaBDN_Prov
                    Dim P_ALLEV_ID_FISCALE As String = AllevIdFiscale_Prov
                    Dim P_SPECIE_CODICE As String = Codice_Specie
                    Dim P_DEST_AZIENDA_CODICE As String = ""
                    Dim P_DEST_ALLEV_ID_FISCALE As String = ""
                    Dim P_DEST_SPECIE_CODICE As String = ""
                    Dim P_FIERA_CODICE As String = ""
                    Dim P_STATO_CODICE As String = "IT"
                    Dim P_PASCOLO_CODICE As String = ""
                    Dim P_MACELLO_CODICE As String = ""
                    Dim P_REGIONE_CODICE As String = "" 'dtAllevamento_Dest(0)("PRO_CODICE") 'TODO
                    Dim P_ESTREMI_DOCUMENTO As String = wsInterrogazioniModello4.calcolaEstremiModello(P_AZIENDA_CODICE, P_SPECIE_CODICE,
                                                                                                       "AL")

                    'P_XML_CAPO
                    Dim P_XML_CAPI As New List(Of XmlCapi_InvioModelli)
                    For Each mat In listaMatricoleCapi
                        Dim CapoXml As New XmlCapi_InvioModelli
                        CapoXml.capoCodice = mat
                        CapoXml.codiceElettronico = ""
                        CapoXml.identNome = ""
                        CapoXml.passaporto = ""
                        CapoXml.codiceUeln = ""
                        P_XML_CAPI.Add(CapoXml)
                    Next
                    'P_XML_CAPO

                    Dim P_DT_USCITA As Date = dataUscita

                    Dim P_FLAG_MACELLO_1 As String = ""
                    Dim P_FLAG_MACELLO_2 As String = ""
                    Dim P_FLAG_MACELLO_2A As String = ""
                    Dim P_FLAG_MACELLO_2B As String = ""
                    Dim P_FLAG_MACELLO_2C As String = ""
                    Dim P_FLAG_MACELLO_3 As String = ""
                    Dim P_FLAG_MACELLO_3_ENTERICI As String = ""
                    Dim P_FLAG_MACELLO_3_RESPIRATORI As String = ""
                    Dim P_FLAG_MACELLO_3_CUTANEI As String = ""
                    Dim P_FLAG_MACELLO_3_LOCOMOTORI As String = ""
                    Dim P_FLAG_MACELLO_3_ALTRO As String = ""
                    Dim P_FLAG_MACELLO_3_ALTRO_DESC As String = ""
                    Dim P_FLAG_MACELLO_4 As String = ""
                    Dim P_FLAG_MACELLO_5 As String = ""
                    Dim P_FLAG_MACELLO_6 As String = ""
                    Dim P_FLAG_ELEMENTI As String = ""
                    Dim P_FLAG_RILEVAZIONI As String = ""
                    Dim P_FLAG_ALTRO As String = ""
                    Dim P_FLAG_ALTRO_DESC As String = ""

                    'P_CAUSALE = "U(fiera)", "M(macello)", "V(allevamento)", "S(stalla di sosta)", "G(centro genetico)", "K(centro di raccolta)", "P(pascolo)", "A(macellazione domiciliare per autoconsumo)"
                    Dim P_CAUSALE As String = ""
                    Select Case Lav_Cod
                        Case LAVCOD_DECREMENTO_CONSISTENZE_ZOO, LAVCOD_MORTE_ANIMALI
                            P_CAUSALE = "V"
                            P_DEST_AZIENDA_CODICE = CodiceAziendaBDN_Dest
                            P_DEST_ALLEV_ID_FISCALE = AllevIdFiscale_Dest
                            P_DEST_SPECIE_CODICE = Codice_Specie
                            P_FIERA_CODICE = ""
                            P_STATO_CODICE = "IT"
                            P_PASCOLO_CODICE = ""
                            P_MACELLO_CODICE = ""
                            P_REGIONE_CODICE = "" 'dtAllevamento_Dest(0)("PRO_CODICE") 'TODO

                        Case LAVCOD_VENDITA_ANIMALI, LAVCOD_TRASFERIMENTO_ANIMALI, LAVCOD_MACELLAZIONE_ANIMALI
                            If Lav_Cod = LAVCOD_MACELLAZIONE_ANIMALI Then
                                P_CAUSALE = "M"
                                P_DEST_AZIENDA_CODICE = ""
                                P_DEST_ALLEV_ID_FISCALE = ""
                                P_DEST_SPECIE_CODICE = Codice_Specie
                                P_FIERA_CODICE = ""
                                P_STATO_CODICE = "IT"
                                P_PASCOLO_CODICE = ""
                                P_MACELLO_CODICE = CodiceAziendaBDN_Dest
                                Dim codiceRegione = RegioneDaContatto(RagSocContatto, PivaContattoDestinazione, CodContattoDestinazione, "Macello")
                                P_REGIONE_CODICE = codiceRegione
                            Else
                                P_CAUSALE = "V"
                                P_DEST_AZIENDA_CODICE = CodiceAziendaBDN_Dest
                                P_DEST_ALLEV_ID_FISCALE = AllevIdFiscale_Dest
                                P_DEST_SPECIE_CODICE = Codice_Specie
                                P_FIERA_CODICE = ""
                                P_STATO_CODICE = "IT"
                                P_PASCOLO_CODICE = ""
                                P_MACELLO_CODICE = ""
                                P_REGIONE_CODICE = ""
                            End If

                            P_FLAG_MACELLO_1 = IIf(objFlagMacello.Item("macello1"), "S", "")
                            P_FLAG_MACELLO_2 = IIf(objFlagMacello.Item("macello2"), "O", "N")
                            P_FLAG_MACELLO_2A = IIf(objFlagMacello.Item("macello2A"), "S", "N")
                            P_FLAG_MACELLO_2B = IIf(objFlagMacello.Item("macello2B"), "S", "N")
                            P_FLAG_MACELLO_2C = IIf(objFlagMacello.Item("macello2C"), "S", "N")
                            P_FLAG_MACELLO_3 = IIf(objFlagMacello.Item("macello3"), "S", "N")
                            If P_FLAG_MACELLO_3 = "S" Then
                                P_FLAG_MACELLO_3_ENTERICI = IIf(objFlagMacello.Item("macello3E"), "S", "")
                                P_FLAG_MACELLO_3_RESPIRATORI = IIf(objFlagMacello.Item("macello3R"), "S", "")
                                P_FLAG_MACELLO_3_CUTANEI = IIf(objFlagMacello.Item("macello3C"), "S", "")
                                P_FLAG_MACELLO_3_LOCOMOTORI = IIf(objFlagMacello.Item("macello3L"), "S", "")
                                P_FLAG_MACELLO_3_ALTRO = IIf(objFlagMacello.Item("macello3A"), "S", "")
                                P_FLAG_MACELLO_3_ALTRO_DESC = objFlagMacello.Item("macello3AD")
                            Else
                                P_FLAG_MACELLO_3_ENTERICI = ""
                                P_FLAG_MACELLO_3_RESPIRATORI = ""
                                P_FLAG_MACELLO_3_CUTANEI = ""
                                P_FLAG_MACELLO_3_LOCOMOTORI = ""
                                P_FLAG_MACELLO_3_ALTRO = ""
                                P_FLAG_MACELLO_3_ALTRO_DESC = ""
                            End If

                            P_FLAG_MACELLO_4 = IIf(objFlagMacello.Item("macello4"), "S", "N")
                            P_FLAG_MACELLO_5 = IIf(objFlagMacello.Item("macello5"), "S", "N")
                            If P_FLAG_MACELLO_5 = "S" Then
                                P_FLAG_ELEMENTI = IIf(objFlagMacello.Item("macelloE"), "S", "")
                                P_FLAG_RILEVAZIONI = IIf(objFlagMacello.Item("macelloR"), "S", "")
                                P_FLAG_ALTRO = IIf(objFlagMacello.Item("macelloA"), "S", "")
                                P_FLAG_ALTRO_DESC = objFlagMacello.Item("macelloAD")
                            Else
                                P_FLAG_ELEMENTI = ""
                                P_FLAG_RILEVAZIONI = ""
                                P_FLAG_ALTRO = ""
                                P_FLAG_ALTRO_DESC = ""
                            End If
                            P_FLAG_MACELLO_6 = IIf(objFlagMacello.Item("macello6"), "S", "N")

                    End Select

                    Dim P_TIPO_STAMPA As String = ""
                    'If P_PASCOLO_CODICE <> "" Then
                    '    P_TIPO_STAMPA = "M(Modello4)", "N(Modello 7)", "E(Entrambi)"
                    'End If

                    'P_XML_TRATTAMENTI
                    '!!! Al momento passare con un solo elemento vuoto !!!
                    Dim P_XML_TRATTAMENTI As New List(Of XmlTrattamenti_InvioModelli)
                    'For Each tratt In listaTrattamentiCapo
                    'If P_CAUSALE <> "M" Then

                    '	Dim TrattamentoXml As New XmlTrattamenti_InvioModelli
                    '	TrattamentoXml.trattamentoFlagProntuario = ""
                    '	TrattamentoXml.trattamentoCapoCodice = ""
                    '	TrattamentoXml.trattamentoTipo = ""
                    '	TrattamentoXml.trattamentoAic = ""
                    '	TrattamentoXml.trattamentoDenominazione = ""
                    '	TrattamentoXml.trattamentoConfezione = ""
                    '	TrattamentoXml.trattamentoDataSomm = ""
                    '	TrattamentoXml.trattamentoPeriodoSosp = ""

                    '	'P_XML_TRATTAMENTI.Add(TrattamentoXml)

                    'End If
                    'Next
                    'P_XML_TRATTAMENTI

                    'P_XML_ESAMI
                    '!!! Al momento passare con un solo elemento vuoto !!!
                    Dim P_XML_ESAMI As New List(Of XmlEsamiCapo_InvioModelli)
                    'For Each esame In listaEsamiCapo
                    'If P_CAUSALE <> "M" Then

                    '	Dim EsameXml As New XmlEsamiCapo_InvioModelli
                    '	EsameXml.esameCodice = ""
                    '	EsameXml.esameAltroDesc = ""
                    '	EsameXml.esameData = ""
                    '	EsameXml.esameCodiceRis = ""
                    '	EsameXml.esameCapoCodice = ""
                    '	EsameXml.esameFlagTutti = ""

                    '	'P_XML_ESAMI.Add(EsameXml)

                    'End If
                    'Next
                    'P_XML_ESAMI

                    Dim P_FLAG_UPLOAD As String = ""
                    Dim P_VETERINARIO As String = vetNomeCognome ' Nome e cognome del veterinario ufficiale che ha confermato il modello
                    Dim P_VET_AZIENDALE As String = vetCognomeNome ' Cognome e nome del veterinario aziendale 
                    Dim P_INDIRIZZO As String = vetIndirizzo 'indirizzo del veterinario aziendale
                    Dim P_TELEFONO As String = vetTelefono 'telefono del veterinario aziendale
                    Dim P_ISTAT As String = vetComIstat 'codice istat comune del veterinario aziendale
                    Dim P_SIGLA As String = vetProIstat 'sigla provincia del veterinario aziendale
                    Dim P_NUM_ISCR_ALBO As String = vetIscrizioneAlbo 'num di iscrizione dell'albo del veterinario aziendale

                    'P_XML_TRASPORTATORE
                    Dim P_TRASP_TARGA_MOTRICE As String = ""
                    Dim P_TRASP_TARGA As String = ""
                    Dim P_TRASP_CONDUCENTE As String = ""
                    Dim P_TRASP_DENOM_TRASPORTATORE As String = ""
                    Dim P_TRASP_TARGA_RIMORCHIO As String = ""
                    Dim P_TRASP_NUM_AUTORIZZAZIONE As String = ""
                    'Dim dtPartenza As String = dataUscita.ToString("ddMMyyyy")
                    Dim P_TRASP_DT_PARTENZA As Date = dataUscita
                    Dim P_TRASP_ORA_PARTENZA As String = ""
                    Dim P_TRASP_DURATA_VIAGGIO As String = ""
                    Dim P_TRASP_FLAG_MEZZO_PROPRIO As String = FlagTipoTrasporto
                    Dim P_TRASP_COD_ASL_TRASP As String = ""
                    Dim P_TRASP_SL_COD_FISCALE As String = ""

                    If FlagTipoTrasporto = "S" OrElse FlagTipoTrasporto = "N" Then
                        P_TRASP_TARGA_MOTRICE = Targa
                        P_TRASP_TARGA_RIMORCHIO = Targa_Rimorchio
                        'P_TRASP_TARGA = Targa
                        P_TRASP_CONDUCENTE = conducente
                        P_TRASP_DENOM_TRASPORTATORE = TraspDenom
                        P_TRASP_NUM_AUTORIZZAZIONE = NumAutorizzazione

                        P_TRASP_DT_PARTENZA = dataUscita
                        P_TRASP_ORA_PARTENZA = dataUscita.ToString("HH:mm")
                        'P_TRASP_ORA_PARTENZA = dataUscita.Hour & ":" & dataUscita.Minute
                        P_TRASP_DURATA_VIAGGIO = DurataViaggio
                        P_TRASP_COD_ASL_TRASP = CodiceAsl_Trasp
                        P_TRASP_SL_COD_FISCALE = CodFiscale_Trasp
                    End If
                    'P_XML_TRASPORTATORE

                    Dim P_FLAG_USCITA_AUTOMATICA As String = "S" 'solo valori S e N
                    Dim P_CONFERMA_BDR As String = "S"

                    'Dim P_DT_DOCUMENTO As Date = dataUscita
                    Dim P_DT_DOCUMENTO As Date = Date.Now
                    Dim P_GIORNI_VALIDITA As String = ""
                    Dim P_FLAG_TIPO_NOTA As String = ""
                    Dim P_NOTA As String = ""
                    Dim P_DETEN_PAS_ID_FISCALE As String = ""
                    Dim P_SIGLA_AUTOC As String = ""
                    Dim P_ISTAT_AUTOC As String = ""
                    Dim P_ID_FISCALE_AUTOC As String = ""
                    Dim P_DT_RIENTRO As Date = "1899-01-01"
                    Dim P_DESCR_PERCORSO As String = ""
                    Dim P_SIGLA_DEST As String = ""
                    Dim P_ISTAT_DEST As String = ""

                    modello4Response.importato = "NO"
                    modello4Response.registrato = "NO"

                    'CREAZIONE Modello4
                    Dim dtModelloInserito As DataTable =
                        wsGestioneModello4.insPrenotazioneModello(P_PRENOTAZIONE_ID, P_DOCUMENTO_ID,
                                                                  P_AZIENDA_CODICE, P_ALLEV_ID_FISCALE, P_SPECIE_CODICE,
                                                                  P_DEST_AZIENDA_CODICE, P_DEST_ALLEV_ID_FISCALE, P_DEST_SPECIE_CODICE,
                                                                  P_FIERA_CODICE, P_STATO_CODICE, P_PASCOLO_CODICE,
                                                                  P_MACELLO_CODICE, P_REGIONE_CODICE, P_ESTREMI_DOCUMENTO,
                                                                  P_XML_CAPI,
                                                                  P_DT_USCITA, P_CAUSALE, P_TIPO_STAMPA,
                                                                  P_FLAG_MACELLO_1, P_FLAG_MACELLO_2,
                                                                  P_FLAG_MACELLO_2A, P_FLAG_MACELLO_2B, P_FLAG_MACELLO_2C,
                                                                  P_XML_TRATTAMENTI,
                                                                  P_XML_ESAMI,
                                                                  P_FLAG_MACELLO_3, P_FLAG_MACELLO_3_ENTERICI, P_FLAG_MACELLO_3_RESPIRATORI,
                                                                  P_FLAG_MACELLO_3_CUTANEI, P_FLAG_MACELLO_3_LOCOMOTORI,
                                                                  P_FLAG_MACELLO_3_ALTRO, P_FLAG_MACELLO_3_ALTRO_DESC,
                                                                  P_FLAG_MACELLO_4, P_FLAG_MACELLO_5,
                                                                  P_FLAG_ELEMENTI, P_FLAG_RILEVAZIONI,
                                                                  P_FLAG_ALTRO, P_FLAG_ALTRO_DESC,
                                                                  P_FLAG_UPLOAD, P_FLAG_MACELLO_6,
                                                                  P_VET_AZIENDALE, P_INDIRIZZO,
                                                                  P_TELEFONO, P_ISTAT,
                                                                  P_SIGLA, P_NUM_ISCR_ALBO,
                                                                  P_TRASP_TARGA_MOTRICE, P_TRASP_TARGA, P_TRASP_CONDUCENTE,
                                                                  P_TRASP_DENOM_TRASPORTATORE, P_TRASP_TARGA_RIMORCHIO, P_TRASP_NUM_AUTORIZZAZIONE,
                                                                  P_TRASP_DT_PARTENZA, P_TRASP_ORA_PARTENZA, P_TRASP_DURATA_VIAGGIO,
                                                                  P_TRASP_FLAG_MEZZO_PROPRIO, P_TRASP_COD_ASL_TRASP,
                                                                  P_TRASP_SL_COD_FISCALE, P_FLAG_USCITA_AUTOMATICA,
                                                                  P_CONFERMA_BDR, P_DT_DOCUMENTO, P_GIORNI_VALIDITA,
                                                                  P_FLAG_TIPO_NOTA, P_NOTA,
                                                                  P_VETERINARIO, P_DETEN_PAS_ID_FISCALE,
                                                                  P_SIGLA_AUTOC, P_ISTAT_AUTOC,
                                                                  P_ID_FISCALE_AUTOC, P_DT_RIENTRO, P_DESCR_PERCORSO,
                                                                  P_SIGLA_DEST, P_ISTAT_DEST)

                    objLog.Scrivi_LOG(objParametriServer, "", "Qui ci arrivo1! dtModelloInserito:" & JsonConvert.SerializeObject(dtModelloInserito))

                    'controlla se è stato effettuato l'inserimento
                    If Not IsNothing(dtModelloInserito) AndAlso dtModelloInserito.Rows.Count > 0 Then
                        modello4Response.importato = "SI"
                        modello4Response.registrato = "NO"
                        Dim Prenotazione_Id As String = dtModelloInserito(0)("PRENOTAZIONE_ID")

                        If dtModelloInserito.Columns.Contains("MSG_VALIDAZIONE") Then
                            modello4Response.errore = IIf(dtModelloInserito(0)("MSG_VALIDAZIONE") IsNot DBNull.Value, dtModelloInserito(0)("MSG_VALIDAZIONE").ToString(), "")
                        End If

                        Dim objModelli4 As New CaricaModelli4_Response
                        Dim dsPrenotazioneModello = wsInterrogazioniModello4.getPrenotazioneModello(Prenotazione_Id,
                                                                                                    Codice_Specie)


                        objLog.Scrivi_LOG(objParametriServer, "", "Qui ci arrivo2! dsPrenotazioneModello:" & JsonConvert.SerializeObject(dsPrenotazioneModello))

                        Dim dtModello4 As DataTable = dsPrenotazioneModello.Tables(0)

                        If Not IsNothing(dtModello4) AndAlso dtModello4.Rows.Count > 0 Then
                            Try

                                listaModelliInviati_PrenotazioneId.Add(Prenotazione_Id)

                                'IIf(dtModello4.Columns.Contains("PRENOTAZIONE_ID"), dtModello4.Rows(0).Item("PRENOTAZIONE_ID"), "")
                                objModelli4.prenotazioneId = IIf(dtModello4.Columns.Contains("PRENOTAZIONE_ID"), dtModello4.Rows(0).Item("PRENOTAZIONE_ID"), "")
                                objModelli4.documentoId = IIf(dtModello4.Columns.Contains("DOCUMENTO_ID"), dtModello4.Rows(0).Item("DOCUMENTO_ID"), "")
                                objModelli4.numModello = IIf(dtModello4.Columns.Contains("NUM_MODELLO"), dtModello4.Rows(0).Item("NUM_MODELLO"), "")

                                Try
                                    objModelli4.giorniValidita = IIf(dtModello4.Columns.Contains("GIORNI_VALIDITA"), dtModello4.Rows(0).Item("GIORNI_VALIDITA"), 0)
                                Catch ex As Exception
                                    objModelli4.giorniValidita = 0
                                End Try

                                objModelli4.tipoDestinazione = IIf(dtModello4.Columns.Contains("TIPOLOGIA_DEST"), dtModello4.Rows(0).Item("TIPOLOGIA_DEST"), "")
                                If objModelli4.tipoDestinazione = "MACELLO" Then
                                    objModelli4.codAzienda_Dest = IIf(dtModello4.Columns.Contains("MACELLO_CODICE"), dtModello4.Rows(0).Item("MACELLO_CODICE"), "")
                                Else
                                    objModelli4.codAzienda_Dest = IIf(dtModello4.Columns.Contains("DEST_AZIENDA_CODICE"), dtModello4.Rows(0).Item("DEST_AZIENDA_CODICE"), "")
                                End If
                                objModelli4.dataUscita = IIf(dtModello4.Columns.Contains("DT_USCITA"), dtModello4.Rows(0).Item("DT_USCITA"), AGRODATAINIZIO)

                                modello4Response.NumModello4 = objModelli4.numModello
                                modello4Response.CodiceModello4 = objModelli4.documentoId

                                Dim listaCapi_Modello4 = dsPrenotazioneModello.Tables(1).ToExpandoObject.ToList()
                                Dim listaMatricoleCapi_Modello4 = (From cp In listaCapi_Modello4
                                                                   Select cp.Item("CAPO_CODICE")).ToList()


                                objLog.Scrivi_LOG(objParametriServer, "", "Qui ci arrivo3!")
                                objModelli4.listaMatricoleCapi = New List(Of String)
                                For Each mat In listaMatricoleCapi_Modello4
                                    objModelli4.listaMatricoleCapi.Add(mat)
                                    Dim matricola As String = mat
                                    Dim capo As AgronicaCoreEntityFramework_POCO.Zoo_Animali = (From z In GiasContext.Zoo_Animali
                                                                                                Where z.Matricola = matricola AndAlso
                                                                                                    lista_CodAnimale.Contains(z.Cod_Progetto)
                                                                                                Select z).First
                                    capo.Modello4_Uscita_Numero = objModelli4.numModello
                                    capo.Modello4_Uscita = objModelli4.documentoId
                                    capo.Modello4_Uscita_Prenotazione = objModelli4.prenotazioneId
                                    capo.Codice_Azienda_Uscita = objModelli4.codAzienda_Dest
                                    capo.Data_Documento_Uscita = objModelli4.dataUscita
                                    capo.Codice_Azienda_Fornitore = CodiceAziendaBDN_Prov

                                    GiasContext.Entry(capo).State = EntityState.Modified
                                Next

                                Agenda.Blocco_Flag = 1
                                GiasContext.Entry(Agenda).State = EntityState.Modified

                                Movimenti_Registrazione.Extra_Str = objModelli4.numModello
                                GiasContext.Entry(Movimenti_Registrazione).State = EntityState.Modified

                                MovDettaglio_TecnEx.Num_Riferimento = objModelli4.prenotazioneId
                                GiasContext.Entry(MovDettaglio_TecnEx).State = EntityState.Modified

                                GiasContext.SaveChanges()

                                Dim dtDocAllegato As DataTable = wsInterrogazioniModello4.creaModelloSingoliConDatiCorrenti(Prenotazione_Id,
                                                                                                                            Codice_Specie)

                                objLog.Scrivi_LOG(objParametriServer, "", "Qui ci arrivo4! dtDocAllegato:" & JsonConvert.SerializeObject(dtDocAllegato))

                                Dim file As String = dtDocAllegato(0)("P_FILE_64")
                                Dim pathPdf As String = scaricaPdfModelli(file)
                                modello4Response.pathPdf = pathPdf
                                modello4Response.File = file.Replace("\n", "").Replace(vbCrLf, "")

                                Dim dataConferma As Date = AGRODATAINIZIO
                                Try
                                    dataConferma = If(dtModello4.Columns.Contains("DT_CONFERMA_ASL"), dtModello4.Rows(0).Item("DT_CONFERMA_ASL"), AGRODATAINIZIO)
                                Catch ex As Exception
                                    objLog.Scrivi_LOG(objParametriServer, "", "dataConferma Qui ci arrivo Errore:" & ex.Message)
                                End Try

                                'se selezionata, l'operazione registra il modello generato
                                If registraModello Then
                                    Dim P_DT_CONFERMA_ASL As String = ""
                                    Try
                                        P_DT_CONFERMA_ASL = dtModello4.Rows(0).Item("DT_CONFERMA_ASL")
                                    Catch ex As Exception
                                        objLog.Scrivi_LOG(objParametriServer, "", "P_DT_CONFERMA_ASL Qui ci arrivo Errore:" & ex.Message)
                                    End Try


                                    'REGISTRAZIONE Modello4
                                    Dim dtModelloRegistrato As New DataTable
                                    dtModelloRegistrato = wsGestioneModello4.registraUscitaModello(Prenotazione_Id, Codice_Specie,
                                                                                                   P_DT_USCITA, P_DT_CONFERMA_ASL,
                                                                                                   P_FLAG_MACELLO_1, P_FLAG_MACELLO_2,
                                                                                                   P_FLAG_MACELLO_2A, P_FLAG_MACELLO_2B, P_FLAG_MACELLO_2C,
                                                                                                   P_XML_TRATTAMENTI, P_XML_ESAMI,
                                                                                                   P_FLAG_MACELLO_3, P_FLAG_MACELLO_3_ENTERICI,
                                                                                                   P_FLAG_MACELLO_3_RESPIRATORI, P_FLAG_MACELLO_3_CUTANEI, P_FLAG_MACELLO_3_LOCOMOTORI,
                                                                                                   P_FLAG_MACELLO_3_ALTRO, P_FLAG_MACELLO_3_ALTRO_DESC,
                                                                                                   P_FLAG_MACELLO_4, P_FLAG_MACELLO_5,
                                                                                                   P_FLAG_ELEMENTI, P_FLAG_RILEVAZIONI,
                                                                                                   P_FLAG_ALTRO, P_FLAG_ALTRO_DESC, P_FLAG_MACELLO_6,
                                                                                                   P_VET_AZIENDALE, P_INDIRIZZO, P_TELEFONO, P_ISTAT,
                                                                                                   P_SIGLA, P_NUM_ISCR_ALBO,
                                                                                                   P_TRASP_TARGA_MOTRICE, P_TRASP_TARGA,
                                                                                                   P_TRASP_CONDUCENTE, P_TRASP_DENOM_TRASPORTATORE,
                                                                                                   P_TRASP_TARGA_RIMORCHIO, P_TRASP_NUM_AUTORIZZAZIONE,
                                                                                                   P_TRASP_DT_PARTENZA, P_TRASP_ORA_PARTENZA,
                                                                                                   P_TRASP_DURATA_VIAGGIO, P_TRASP_FLAG_MEZZO_PROPRIO,
                                                                                                   P_TRASP_COD_ASL_TRASP, P_TRASP_SL_COD_FISCALE,
                                                                                                   listaMatricoleCapi, P_FLAG_USCITA_AUTOMATICA)

                                    'se il modello generato è già stato confermato, viene registrato
                                    If Not IsNothing(dtModelloRegistrato) AndAlso dtModelloRegistrato.Rows.Count > 0 Then
                                        Agenda.Tipo_Accettazione = 1
                                        GiasContext.Entry(Agenda).State = EntityState.Modified

                                        For Each mat In listaMatricoleCapi_Modello4
                                            Dim matricola As String = mat
                                            Dim capo = GiasContext.Zoo_Animali.Where(Function(z) z.Matricola = matricola).FirstOrDefault
                                            Dim codAnimale As Integer = capo.Cod_Progetto
                                            Dim idCapoBDN As Integer = capo.Id_Capo_BDN

                                            Dim mds As AgronicaCoreEntityFramework_POCO.Movimenti_dettagli = GiasContext.Movimenti_dettagli.Where(Function(md) md.PIVA = Piva AndAlso md.Id_Agenda = idAgenda AndAlso
                                                                                                                                                      md.Id_Mov = idMov_Scarico AndAlso md.Cod_Progetto = codAnimale).FirstOrDefault
                                            If IsNothing(mds) Then
                                                Throw New Exception($"L'operazione (Id_Agenda={idAgenda}) non contenente capi da scaricare")
                                            End If

                                            Dim idUscita As Integer = sincronizzatoreAnimale.trovaID_Uscita(AllevIdFiscale_Prov,
                                                                                                            CodiceAziendaBDN_Prov,
                                                                                                            mat)
                                            modello4Response.registrato = "SI"

                                            mds.Id_Mov_Esterno = idUscita
                                            GiasContext.Entry(Agenda).State = EntityState.Modified

                                            logMovimentazioniBDN(Piva, Sa_Cod, idAgenda, Lav_Cod, enumCausaliBDN.InvMovUscitaModello4,
                                                                 idUscita, Movimenti_Scarico.Id_Mov, mds.Id_Mov_Det, codAnimale,
                                                                 idCapoBDN, dtModelloRegistrato, dtModelloRegistrato, True, GiasContext)
                                        Next

                                        GiasContext.SaveChanges()
                                    End If

                                End If

                            Catch ex As Exception

                                objLog.Scrivi_LOG(objParametriServer, "", "Errore in InserisciModello4:" & ex.Message)

                                Throw ex

                            End Try
                        End If
                    Else
                        logMovimentazioniBDN(Piva, Sa_Cod, idAgenda, Lav_Cod, enumCausaliBDN.InvMovUscitaModello4,
                                             0, Movimenti_Scarico.Id_Mov, 0, 0, 0, Nothing,
                                             Nothing, False, GiasContext)
                    End If
                End If
            Catch ex As TokenBDNException
                Throw New BDNException("Token scaduto")

            Catch ex As BDNException
                If modello4Response.importato = "NO" Then
                    modello4Response.CodiceModello4 = ""
                    modello4Response.NumModello4 = ""
                    modello4Response.registrato = "NO"
                Else
                    modello4Response.registrato = "NO"
                End If
                If ex.Message.Contains("DL01") Then
                    modello4Response.errore &= "Errore nel tentativo di download del modello"
                Else
                    modello4Response.errore &= ex.Message
                End If
            Catch ex As Exception
                If modello4Response.importato = "NO" Then
                    modello4Response.CodiceModello4 = ""
                    modello4Response.NumModello4 = ""
                    modello4Response.registrato = "NO"
                Else
                    modello4Response.registrato = "NO"
                End If
                If ex.Message.Contains("DL01") Then
                    modello4Response.errore &= "Errore nel tentativo di download del modello"
                Else
                    modello4Response.errore &= ex.Message
                End If
            End Try

            listaModelli4Response.Add(modello4Response)
        Next

        Return listaModelli4Response

    End Function

    Public Function RegioneDaContatto(Rag_Soc_Contatto As String, Piva As String, Contatto_Cod As String, TipoContatto As String) As String
        Dim objContattixIndirizzi As New AgronicaCoreAnagrafeDAL.ContattiXIndirizzi_R
        Dim dtContatto = objContattixIndirizzi.LeggiContattoSpecifico(Piva, Contatto_Cod, 99, 0, 0, enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametriServer)
        If dtContatto.Rows.Count = 0 Then
            dtContatto = objContattixIndirizzi.LeggiContattoSpecifico("", Contatto_Cod, 0, 0, 0, enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametriServer)
        End If
        Dim regione As String = ""
        If dtContatto.Rows.Count > 0 Then
            Dim drContatto = dtContatto.Select(" REG <> '000' ")
            If drContatto.Length > 0 Then
                Dim regioneStr = CStr(drContatto(0)("REG"))
                regioneStr = regioneStr.Substring(1, 2)
                regioneStr = regioneStr + "0"
                regione = regioneStr
            Else
                Throw New GiasException("Contatto:" & Rag_Soc_Contatto & " Tipo: " & TipoContatto & " non è stato possibile risalire al codice della regione, impostare correttamente un indirizzo")
            End If
        Else
            Throw New GiasException("Contatto:" & Rag_Soc_Contatto & " Tipo: " & TipoContatto & " non è stato possibile risalire al codice della regione, impostare correttamente un indirizzo")
        End If

        Return regione
    End Function

    ''' <summary>
    ''' Registra i Modelli 4 già presenti su BDN (DISMESSA?)
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Operazione_Agenda"></param>
    ''' <param name="Cod_Animale"></param>
    ''' <returns></returns>
    'Public Function RegistraModello4_Uscita(ByVal Piva As String,
    '										ByVal Sa_Cod As Integer,
    '										ByVal Operazione_Agenda As Integer,
    '										ByVal Cod_Animale As Integer) As InviaUscitaResponse
    '	Dim listaModelliRegistrati_PrenotazioneId As New List(Of String)
    '	Dim response As New InviaUscitaResponse

    '	Try
    '		Dim idAgenda As Integer = Operazione_Agenda
    '		'For Each idAgenda In Operazioni_Agenda
    '		Dim Agenda As AgronicaCoreEntityFramework_POCO.Agenda = (From a In GiasContext.Agenda
    '																 Where a.PIVA = Piva AndAlso a.Id_Agenda = idAgenda).FirstOrDefault
    '		Dim Lav_Cod As Integer = Agenda.Lav_Cod

    '		If IsNothing(Agenda) Then
    '			Throw New Exception("Operazione zootecnica (Id_Agenda=" & idAgenda & ") non valida o inesistente")
    '		End If

    '		Dim Movimenti_Scarico As AgronicaCoreEntityFramework_POCO.Movimenti = (From m In GiasContext.Movimenti
    '																			   Where m.PIVA = Piva AndAlso m.Id_Agenda = idAgenda AndAlso
    '																					   m.Cau_Mov = CAU_SCARICO_CONSISTENZE).FirstOrDefault
    '		If IsNothing(Movimenti_Scarico) Then
    '			Throw New Exception("Operazione zootecnica (Id_Agenda=" & idAgenda & ") non valida o inesistente")
    '		End If
    '		Dim idMov_Scarico As Integer = Movimenti_Scarico.Id_Mov

    '		Dim Movimenti_Registrazione As AgronicaCoreEntityFramework_POCO.Movimenti = (From m In GiasContext.Movimenti
    '																					 Where m.PIVA = Piva AndAlso m.Id_Agenda = idAgenda AndAlso
    '																							 m.Cau_Mov = CAU_REGISTRAZIONI).FirstOrDefault
    '		If IsNothing(Movimenti_Registrazione) Then
    '			Throw New Exception("Operazione zootecnica (Id_Agenda=" & idAgenda & ") non valida o inesistente")
    '		End If
    '		Dim idMov_Registrazione As Integer = Movimenti_Registrazione.Id_Mov

    '		Dim MovDettaglio_TecnEx As AgronicaCoreEntityFramework_POCO.Mov_Dettaglio_Tecnico_Extra = (From a In GiasContext.Mov_Dettaglio_Tecnico_Extra
    '																								   Where a.Piva = Piva AndAlso a.Id_Agenda = idAgenda AndAlso
    '																											  a.Id_Mov = idMov_Registrazione).FirstOrDefault
    '		If IsNothing(MovDettaglio_TecnEx) Then
    '			Throw New Exception("Dati di trasporto per l'operazione (Id_Agenda=" & idAgenda & ") non presenti")
    '		End If

    '		Dim MovDest_Scarico As List(Of AgronicaCoreEntityFramework_POCO.Mov_Destinazioni) = (From md In GiasContext.Mov_Destinazioni
    '																							 Where md.Piva = Piva AndAlso md.Id_Agenda = idAgenda AndAlso
    '																									   md.Id_Mov = idMov_Scarico).ToList
    '		If IsNothing(MovDest_Scarico) OrElse MovDest_Scarico.Count = 0 Then
    '			Throw New Exception("L'operazione (Id_Agenda=" & idAgenda & ") non contiente capi da scaricare")
    '		End If

    '		'------INIZIO RECUPERO DATI MODELLO 4------

    '		'PRENOTAZIONE_ID
    '		Dim prenotazioneId As String = MovDettaglio_TecnEx.Num_Riferimento

    '		'TARGA MEZZO
    '		Dim targa As String = MovDettaglio_TecnEx.Targa

    '		'CODICE_AZIENDA_BDN PROVENIENZA
    '		Dim objStalla As New AgronicaCoreAnagrafeDAL.Stalla_R
    '		Dim raggruppamento_cod = MovDest_Scarico(0).Id_Destinazione
    '		Dim raggruppamento = (From rr In GiasContext.Stalla_Raggruppamenti Where rr.Raggruppamento_Cod = raggruppamento_cod).FirstOrDefault
    '		Dim Sta_Num As Integer = raggruppamento.STA_NUM
    '		Dim dtStalla As DataTable = objStalla.Leggi(Piva, Sa_Cod, Sta_Num,
    '													enumSelezioneVariabile.Selezione_JoinDescrizioni,
    '													"", "", objParametriServer)
    '		If IsNothing(dtStalla) OrElse dtStalla.Rows.Count = 0 Then
    '			Throw New Exception("Stalla di provenienza non valida o inesistente")
    '		End If
    '		Dim CodiceAziendaBDN_Prov As String = dtStalla(0)("BDN_Codice_Azienda")

    '		'ALLEV_ID_FISCALE PROVENIENZA
    '		'Dim objImprese_codici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
    '		'Dim AllevIdFiscale_Prov As String = objImprese_codici.Leggi_CUAA(Piva, objParametriServer)

    '		'CODICE SPECIE
    '		Dim objCodificaSpecie As New AgronicaCoreMetaSchemaDAL.Codifica_BDN_SpecieAnimali
    '		Dim dtCodifica As DataTable = objCodificaSpecie.leggi(objParametriServer, "", "",
    '															  dtStalla(0)("GEN_COD"), dtStalla(0)("SPE_COD"),
    '															  "3")
    '		If IsNothing(dtCodifica) OrElse dtCodifica.Rows.Count = 0 Then
    '			Throw New GiasException("Codifica specie mancante per BDN")
    '		End If
    '		Dim Codice_Specie As String = "0" & dtCodifica.Rows(0)("CODICE")

    '		'PIVA_ASL TRASPORTATORE
    '		Dim PivaAsl_Trasp As String = MovDettaglio_TecnEx.Trasportatore
    '		Dim CodFiscale_Trasp As String = ""

    '		Dim objContatti_R As New AgronicaCoreAnagrafeDAL.Contatti_R
    '		Dim dtContatto As DataTable = objContatti_R.LeggiContattoSpecifico(Movimenti_Registrazione.PIVA, PivaAsl_Trasp,
    '																		   0, enumSelezioneVariabile.Selezione_TabellaCompleta,
    '																		   "", "", objParametriServer)

    '		If IsNothing(dtContatto) OrElse dtContatto.Rows.Count = 0 Then
    '			dtContatto = objContatti_R.LeggiContattoSpecifico("", PivaAsl_Trasp,
    '															  99, enumSelezioneVariabile.Selezione_TabellaCompleta,
    '															  "", "", objParametriServer)
    '			If IsNothing(dtContatto) OrElse dtContatto.Rows.Count = 0 Then
    '				Throw New Exception("Trasportatore non valido o inesistente")
    '			End If
    '		End If

    '		'CODICE FISCALE TRASPORTATORE
    '		CodFiscale_Trasp = dtContatto(0)("Codice_Fiscale")

    '		'DENOM TRASPORTATORE
    '		Dim TraspDenom As String = dtContatto(0)("Rag_Soc")

    '		Dim objIndirizzi As New AgronicaCoreAnagrafeBIZ.Indirizzi_R
    '		Dim codiceAUSLTrasp = objIndirizzi.CodiceAslDatoContatto(dtContatto(0)("Piva"), dtContatto(0)("Cod_Contatto"), objParametriServer)

    '		'------FINE RECUPERO DATI MODELLO 4------

    '		Dim objModelli4 As New CaricaModelli4_Response
    '		Dim dsPrenotazioneModello = wsInterrogazioniModello4.getPrenotazioneModello(prenotazioneId,
    '																					Codice_Specie)
    '		Dim dtModello4 As DataTable = dsPrenotazioneModello.Tables(0)
    '		Dim listaCapi_Modello4 = dsPrenotazioneModello.Tables(1).ToExpandoObject.ToList()
    '		Dim listaMatricoleCapi As List(Of String) = (From cp In listaCapi_Modello4
    '													 Select CStr(cp.Item("CAPO_CODICE"))).ToList()

    '		Dim matricolaCapo = (From g In GiasContext.Zoo_Animali Where g.Cod_Progetto = Cod_Animale).FirstOrDefault()

    '		listaMatricoleCapi = New List(Of String)
    '		listaMatricoleCapi.Add(matricolaCapo.Matricola)

    '		If Not IsNothing(dtModello4) AndAlso dtModello4.Rows.Count > 0 Then

    '			Dim P_DT_USCITA As String = dtModello4.Rows(0).Item("DT_USCITA")
    '			Dim P_DT_CONFERMA_ASL As String = dtModello4.Rows(0).Item("DT_CONFERMA_ASL")

    '			Dim P_FLAG_MACELLO_1 As String = ""
    '			Dim P_FLAG_MACELLO_2 As String = ""
    '			Dim P_FLAG_MACELLO_2A As String = ""
    '			Dim P_FLAG_MACELLO_2B As String = ""
    '			Dim P_FLAG_MACELLO_2C As String = ""

    '			Dim P_FLAG_MACELLO_3 As String = "N"
    '			Dim P_FLAG_MACELLO_3_ENTERICI As String = ""
    '			Dim P_FLAG_MACELLO_3_RESPIRATORI As String = ""
    '			Dim P_FLAG_MACELLO_3_CUTANEI As String = ""
    '			Dim P_FLAG_MACELLO_3_LOCOMOTORI As String = ""
    '			Dim P_FLAG_MACELLO_3_ALTRO As String = ""
    '			Dim P_FLAG_MACELLO_3_ALTRO_DESC As String = ""
    '			Dim P_FLAG_MACELLO_4 As String = "N"
    '			Dim P_FLAG_MACELLO_5 As String = "N"
    '			Dim P_FLAG_ELEMENTI As String = ""
    '			Dim P_FLAG_RILEVAZIONI As String = ""
    '			Dim P_FLAG_ALTRO As String = ""
    '			Dim P_FLAG_ALTRO_DESC As String = ""
    '			Dim P_FLAG_MACELLO_6 As String = "N"
    '			Dim P_VET_AZIENDALE As String = ""
    '			Dim P_INDIRIZZO As String = ""
    '			Dim P_TELEFONO As String = ""
    '			Dim P_ISTAT As String = ""
    '			Dim P_SIGLA As String = ""
    '			Dim P_NUM_ISCR_ALBO As String = ""

    '			Dim P_TRASP_TARGA_MOTRICE As String = ""
    '			Dim P_TRASP_TARGA As String = ""
    '			Dim P_TRASP_CONDUCENTE As String = ""
    '			Dim P_TRASP_DENOM_TRASPORTATORE As String = ""
    '			Dim P_TRASP_TARGA_RIMORCHIO As String = ""

    '			Dim P_TRASP_NUM_AUTORIZZAZIONE As String = ""
    '			Dim P_TRASP_DT_PARTENZA As String = ""
    '			Dim P_TRASP_ORA_PARTENZA As String = ""
    '			Dim P_TRASP_DURATA_VIAGGIO As String = ""
    '			Dim P_TRASP_FLAG_MEZZO_PROPRIO As String = ""
    '			Dim P_TRASP_COD_ASL_TRASP As String = ""
    '			Dim P_TRASP_SL_COD_FISCALE As String = ""

    '			Dim P_FLAG_USCITA_AUTOMATICA As String = ""


    '			Dim P_XML_TRATTAMENTI = gestioneTrattamenti_RegistrazioneUscita()
    '			Dim P_XML_ESAMI As List(Of XmlEsamiCapo_InvioModelli) = gestioneEsami_RegistrazioneUscita()


    '			'Next
    '			'P_XML_ESAMI
    '			gestioneDichiarazioneICA_RegistrazioneUscita(P_FLAG_MACELLO_1,
    '															P_FLAG_MACELLO_2,
    '															P_FLAG_MACELLO_2A,
    '															P_FLAG_MACELLO_2B,
    '															P_FLAG_MACELLO_2C,
    '															P_FLAG_MACELLO_3,
    '															P_FLAG_MACELLO_3_ENTERICI,
    '															P_FLAG_MACELLO_3_RESPIRATORI,
    '															P_FLAG_MACELLO_3_CUTANEI,
    '															P_FLAG_MACELLO_3_LOCOMOTORI,
    '															P_FLAG_MACELLO_3_ALTRO,
    '															P_FLAG_MACELLO_3_ALTRO_DESC,
    '															P_FLAG_MACELLO_4,
    '															P_FLAG_MACELLO_5,
    '															P_FLAG_ELEMENTI,
    '															P_FLAG_RILEVAZIONI,
    '															P_FLAG_ALTRO,
    '															P_FLAG_ALTRO_DESC,
    '															P_FLAG_MACELLO_6,
    '															P_VET_AZIENDALE,
    '															P_INDIRIZZO,
    '															P_TELEFONO,
    '															P_ISTAT,
    '															P_SIGLA,
    '															P_NUM_ISCR_ALBO,
    '															dtModello4)

    '			gestioneTrasporto(P_TRASP_TARGA_MOTRICE,
    '								P_TRASP_TARGA,
    '								P_TRASP_CONDUCENTE,
    '								P_TRASP_DENOM_TRASPORTATORE,
    '								P_TRASP_TARGA_RIMORCHIO,
    '								P_TRASP_NUM_AUTORIZZAZIONE,
    '								P_TRASP_DT_PARTENZA,
    '								P_TRASP_ORA_PARTENZA,
    '								P_TRASP_DURATA_VIAGGIO,
    '								P_TRASP_FLAG_MEZZO_PROPRIO,
    '								P_TRASP_COD_ASL_TRASP,
    '								P_TRASP_SL_COD_FISCALE,
    '								P_FLAG_USCITA_AUTOMATICA,
    '								TraspDenom,
    '								CodFiscale_Trasp,
    '								codiceAUSLTrasp,
    '								dtModello4)


    '			'REGISTRAZIONE Modello4
    '			Dim dtModelloRegistrato As New DataTable
    '			dtModelloRegistrato = wsGestioneModello4.registraUscitaModello(prenotazioneId, Codice_Specie,
    '																		   P_DT_USCITA, P_DT_CONFERMA_ASL,
    '																		   P_FLAG_MACELLO_1, P_FLAG_MACELLO_2,
    '																		   P_FLAG_MACELLO_2A, P_FLAG_MACELLO_2B, P_FLAG_MACELLO_2C,
    '																		   P_XML_TRATTAMENTI, P_XML_ESAMI,
    '																		   P_FLAG_MACELLO_3, P_FLAG_MACELLO_3_ENTERICI,
    '																		   P_FLAG_MACELLO_3_RESPIRATORI, P_FLAG_MACELLO_3_CUTANEI, P_FLAG_MACELLO_3_LOCOMOTORI,
    '																		   P_FLAG_MACELLO_3_ALTRO, P_FLAG_MACELLO_3_ALTRO_DESC,
    '																		   P_FLAG_MACELLO_4, P_FLAG_MACELLO_5,
    '																		   P_FLAG_ELEMENTI, P_FLAG_RILEVAZIONI,
    '																		   P_FLAG_ALTRO, P_FLAG_ALTRO_DESC, P_FLAG_MACELLO_6,
    '																		   P_VET_AZIENDALE, P_INDIRIZZO, P_TELEFONO, P_ISTAT,
    '																		   P_SIGLA, P_NUM_ISCR_ALBO,
    '																		   P_TRASP_TARGA_MOTRICE, P_TRASP_TARGA,
    '																		   P_TRASP_CONDUCENTE, P_TRASP_DENOM_TRASPORTATORE,
    '																		   P_TRASP_TARGA_RIMORCHIO, P_TRASP_NUM_AUTORIZZAZIONE,
    '																		   P_TRASP_DT_PARTENZA, P_TRASP_ORA_PARTENZA,
    '																		   P_TRASP_DURATA_VIAGGIO, P_TRASP_FLAG_MEZZO_PROPRIO,
    '																		   P_TRASP_COD_ASL_TRASP, P_TRASP_SL_COD_FISCALE,
    '																		   listaMatricoleCapi, P_FLAG_USCITA_AUTOMATICA)


    '			'se il modello generato è già stato confermato, viene registrato
    '			If Not IsNothing(dtModelloRegistrato) AndAlso dtModelloRegistrato.Rows.Count > 0 Then
    '				Agenda.Tipo_Accettazione = 1
    '				GiasContext.Entry(Agenda).State = EntityState.Modified

    '				Dim Allev_IdFiscale As String = dtModelloRegistrato.Rows(0)("ALLEV_ID_FISCALE")

    '				For Each mat In listaMatricoleCapi
    '					Dim matricola As String = mat
    '					Dim capo = GiasContext.Zoo_Animali.Where(Function(z) z.Matricola = matricola).FirstOrDefault
    '					Dim codAnimale As Integer = capo.Cod_Progetto
    '					Dim idCapoBDN As Integer = capo.Id_Capo_BDN

    '					Dim MovDett_Scarico As AgronicaCoreEntityFramework_POCO.Movimenti_dettagli = GiasContext.Movimenti_dettagli.Where(Function(md) md.PIVA = Piva AndAlso md.Id_Agenda = idAgenda AndAlso
    '																																		  md.Id_Mov = idMov_Scarico AndAlso md.Cod_Progetto = codAnimale).FirstOrDefault
    '					If IsNothing(MovDett_Scarico) Then
    '						Throw New Exception("L'operazione (Id_Agenda=" & idAgenda & ") non contiente capi da scaricare")
    '					End If

    '					Dim idUscita As Integer = sincronizzatoreAnimale.trovaID_Uscita(Allev_IdFiscale,
    '																					CodiceAziendaBDN_Prov,
    '																					mat)
    '					MovDett_Scarico.Id_Mov_Esterno = idUscita
    '					GiasContext.Entry(Agenda).State = EntityState.Modified

    '					logMovimentazioniBDN(Piva, Sa_Cod, idAgenda, Lav_Cod, enumCausaliBDN.InvMovUscitaModello4,
    '										 idUscita, Movimenti_Scarico.Id_Mov, MovDett_Scarico.Id_Mov_Det, codAnimale,
    '										 idCapoBDN, dtModelloRegistrato, dtModelloRegistrato, True, GiasContext)
    '				Next

    '				response.importato = "SI"
    '				GiasContext.SaveChanges()
    '			Else
    '				logMovimentazioniBDN(Piva, Sa_Cod, idAgenda, Lav_Cod, enumCausaliBDN.InvMovUscitaModello4,
    '									 0, Movimenti_Scarico.Id_Mov, 0, 0, 0, Nothing,
    '									 Nothing, False, GiasContext)
    '			End If
    '		End If

    '	Catch ex As TokenBDNException
    '		response.importato = "NO"
    '		response.errore = ex.Message
    '		Throw ex
    '	Catch ex As BDNException
    '		response.importato = "NO"
    '		response.errore = ex.Message
    '	Catch ex As Exception
    '		response.importato = "NO"
    '		response.errore = ex.Message
    '	End Try

    '	Return response

    'End Function

    ''' <summary>
    ''' Registra i Modelli 4 già presenti su BDN
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Operazioni_Agenda"></param>
    ''' <returns></returns>
    Public Function RegistraModelli4(ByVal Piva As String,
                                     ByVal Sa_Cod As Integer,
                                     ByVal Operazioni_Agenda As List(Of Integer)) As List(Of RegistraModello4Response)
        Dim listaModelli4Response As New List(Of RegistraModello4Response)

        'un modello4 per ogni operazione d'agenda
        For Each idAgenda In Operazioni_Agenda
            Dim response As New RegistraModello4Response

            Try
                Dim Agenda As AgronicaCoreEntityFramework_POCO.Agenda = (From a In GiasContext.Agenda
                                                                         Where a.PIVA = Piva AndAlso a.Id_Agenda = idAgenda).FirstOrDefault
                Dim Lav_Cod As Integer = Agenda.Lav_Cod

                If IsNothing(Agenda) Then
                    Throw New Exception($"Operazione zootecnica (Id_Agenda={idAgenda}) non valida o inesistente")
                End If

                Dim Movimenti_Scarico = (From m In GiasContext.Movimenti
                                         Where m.PIVA = Piva AndAlso m.Id_Agenda = idAgenda AndAlso
                                             m.Cau_Mov = CAU_SCARICO_CONSISTENZE)
                If IsNothing(Movimenti_Scarico) Then
                    Throw New Exception($"Operazione zootecnica (Id_Agenda={idAgenda}) non valida o inesistente")
                End If
                Dim idMov_Scarico As Integer = Movimenti_Scarico.FirstOrDefault.Id_Mov

                Dim MovDett_Scarico As List(Of AgronicaCoreEntityFramework_POCO.Movimenti_dettagli) = (From md In GiasContext.Movimenti_dettagli
                                                                                                       Where md.PIVA = Piva AndAlso md.Id_Agenda = idAgenda AndAlso
                                                                                                           md.Id_Mov = idMov_Scarico).ToList
                If IsNothing(MovDett_Scarico) OrElse MovDett_Scarico.Count = 0 Then
                    Throw New Exception($"L'operazione (Id_Agenda={idAgenda}) non contiene capi da scaricare")
                End If

                Dim MovDest_Scarico As List(Of AgronicaCoreEntityFramework_POCO.Mov_Destinazioni) = (From md In GiasContext.Mov_Destinazioni
                                                                                                     Where md.Piva = Piva AndAlso md.Id_Agenda = idAgenda AndAlso
                                                                                                           md.Id_Mov = idMov_Scarico).ToList
                If IsNothing(MovDest_Scarico) OrElse MovDest_Scarico.Count = 0 Then
                    Throw New Exception($"L'operazione (Id_Agenda={idAgenda}) non contiene capi da scaricare")
                End If
                Dim Movimenti_Registrazione As AgronicaCoreEntityFramework_POCO.Movimenti = (From m In GiasContext.Movimenti
                                                                                             Where m.PIVA = Piva AndAlso m.Id_Agenda = idAgenda AndAlso
                                                                                                     m.Cau_Mov = CAU_REGISTRAZIONI).FirstOrDefault
                If IsNothing(Movimenti_Registrazione) Then
                    Throw New Exception($"Operazione zootecnica (Id_Agenda={idAgenda}) non valida o inesistente")
                End If
                Dim idMov_Registrazione As Integer = Movimenti_Registrazione.Id_Mov

                Dim MovDettaglio_TecnEx As AgronicaCoreEntityFramework_POCO.Mov_Dettaglio_Tecnico_Extra = (From a In GiasContext.Mov_Dettaglio_Tecnico_Extra
                                                                                                           Where a.Piva = Piva AndAlso a.Id_Agenda = idAgenda AndAlso
                                                                                                                      a.Id_Mov = idMov_Registrazione).FirstOrDefault
                If IsNothing(MovDettaglio_TecnEx) Then
                    Throw New Exception($"Dati di trasporto per l'operazione (Id_Agenda={idAgenda}) non presenti")
                End If

                '------INIZIO RECUPERO DATI MODELLO 4------

                'PRENOTAZIONE_ID
                Dim prenotazioneId As String = MovDettaglio_TecnEx.Num_Riferimento

                'TARGA MEZZO
                Dim targa As String = MovDettaglio_TecnEx.Targa

                'CODICE_AZIENDA_BDN PROVENIENZA
                Dim objStalla As New AgronicaCoreAnagrafeDAL.Stalla_R
                Dim raggruppamento_cod = MovDest_Scarico(0).Id_Destinazione
                Dim raggruppamento = (From rr In GiasContext.Stalla_Raggruppamenti Where rr.Raggruppamento_Cod = raggruppamento_cod).FirstOrDefault
                Dim Sta_Num As Integer = raggruppamento.STA_NUM
                Dim dtStalla As DataTable = objStalla.Leggi(Piva, Sa_Cod, Sta_Num,
                                                            enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                            "", "", objParametriServer)
                If IsNothing(dtStalla) OrElse dtStalla.Rows.Count = 0 Then
                    Throw New Exception("Stalla di provenienza non valida o inesistente")
                End If
                Dim CodiceAziendaBDN_Prov As String = dtStalla(0)("BDN_Codice_Azienda")

                'ALLEV_ID_FISCALE PROVENIENZA
                Dim objImprese_codici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
                'Dim AllevIdFiscale_Prov As String = objImprese_codici.Leggi_CUAA(Piva, objParametriServer)

                'CODICE SPECIE
                Dim objCodificaSpecie As New AgronicaCoreMetaSchemaDAL.Codifica_BDN_SpecieAnimali
                Dim dtCodifica As DataTable = objCodificaSpecie.leggi(objParametriServer, "", "",
                                                                      dtStalla(0)("GEN_COD"), dtStalla(0)("SPE_COD"),
                                                                      "3")
                If IsNothing(dtCodifica) OrElse dtCodifica.Rows.Count = 0 Then
                    Throw New GiasException("Codifica specie mancante per BDN")
                End If
                Dim Codice_Specie As String = "0" & dtCodifica.Rows(0)("CODICE")

                'PIVA_ASL TRASPORTATORE
                Dim PivaAsl_Trasp As String = MovDettaglio_TecnEx.Trasportatore
                Dim CodFiscale_Trasp As String = ""

                Dim TraspDenom As String = ""

                Dim objContatti_R As New AgronicaCoreAnagrafeDAL.Contatti_R
                Dim dtContatto As DataTable = objContatti_R.LeggiContattoSpecifico(Movimenti_Registrazione.PIVA, PivaAsl_Trasp, 0,
                                                                                   enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                   "", "", objParametriServer)
                Dim objIndirizzi As New AgronicaCoreAnagrafeBIZ.Indirizzi_R
                Dim codiceAUSLTrasp = ""
                If IsNothing(dtContatto) OrElse dtContatto.Rows.Count = 0 Then
                    dtContatto = objContatti_R.LeggiContattoSpecifico("", PivaAsl_Trasp, 99,
                                                                      enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                      "", "", objParametriServer)
                    If IsNothing(dtContatto) OrElse dtContatto.Rows.Count = 0 Then
                        'Throw New Exception("Trasportatore non valido o inesistente")
                    Else
                        'CODICE FISCALE TRASPORTATORE
                        CodFiscale_Trasp = dtContatto(0)("Codice_Fiscale")

                        'DENOM TRASPORTATORE
                        TraspDenom = dtContatto(0)("Rag_Soc")

                        codiceAUSLTrasp = objIndirizzi.CodiceAslDatoContatto(dtContatto(0)("Piva"),
                                                                         dtContatto(0)("Cod_Contatto"),
                                                                         objParametriServer)
                    End If
                End If




                'LISTA COD_PROGETTO CAPI
                Dim lista_CodAnimale As New List(Of Integer)
                For Each movDett In MovDett_Scarico
                    lista_CodAnimale.Add(movDett.Cod_Progetto)
                Next

                '------FINE RECUPERO DATI MODELLO 4------


                Dim objModelli4 As New CaricaModelli4_Response
                Dim dsPrenotazioneModello = wsInterrogazioniModello4.getPrenotazioneModello(prenotazioneId,
                                                                                            Codice_Specie)
                Dim dtModello4 As DataTable = dsPrenotazioneModello.Tables(0)
                Dim AllevIdFiscale_Prov As String = dtModello4.Rows(0).Item("ALLEV_ID_FISCALE")
                objModelli4.prenotazioneId = IIf(dtModello4.Columns.Contains("PRENOTAZIONE_ID"), dtModello4.Rows(0).Item("PRENOTAZIONE_ID"), "")
                objModelli4.documentoId = IIf(dtModello4.Columns.Contains("DOCUMENTO_ID"), dtModello4.Rows(0).Item("DOCUMENTO_ID"), "")
                objModelli4.numModello = IIf(dtModello4.Columns.Contains("NUM_MODELLO"), dtModello4.Rows(0).Item("NUM_MODELLO"), "")
                objModelli4.giorniValidita = IIf(dtModello4.Columns.Contains("GIORNI_VALIDITA"), dtModello4.Rows(0).Item("GIORNI_VALIDITA"), 0)
                objModelli4.tipoDestinazione = IIf(dtModello4.Columns.Contains("TIPOLOGIA_DEST"), dtModello4.Rows(0).Item("TIPOLOGIA_DEST"), "")
                If objModelli4.tipoDestinazione = "MACELLO" Then
                    objModelli4.codAzienda_Dest = IIf(dtModello4.Columns.Contains("MACELLO_CODICE"), dtModello4.Rows(0).Item("MACELLO_CODICE"), "")
                Else
                    objModelli4.codAzienda_Dest = IIf(dtModello4.Columns.Contains("DEST_AZIENDA_CODICE"), dtModello4.Rows(0).Item("DEST_AZIENDA_CODICE"), "")
                End If
                objModelli4.dataUscita = IIf(dtModello4.Columns.Contains("DT_USCITA"), dtModello4.Rows(0).Item("DT_USCITA"), AGRODATAINIZIO)
                Dim listaCapi_Modello4 = dsPrenotazioneModello.Tables(1).ToExpandoObject.ToList()
                Dim listaMatricoleCapi As List(Of String) = (From cp In listaCapi_Modello4
                                                             Select CStr(cp.Item("CAPO_CODICE"))).ToList()

                response.NumModello4 = objModelli4.numModello
                response.CodiceModello4 = objModelli4.documentoId
                response.listaAnimali = listaMatricoleCapi

                If Not IsNothing(dtModello4) AndAlso dtModello4.Rows.Count > 0 Then

                    Dim dt_uscita As Date = IIf(dtModello4.Columns.Contains("DT_USCITA"), dtModello4.Rows(0).Item("DT_USCITA"), AGRODATAINIZIO)
                    Dim dt_conferma_asl = IIf(dtModello4.Columns.Contains("DT_CONFERMA_ASL"), dtModello4.Rows(0).Item("DT_CONFERMA_ASL"), AGRODATAINIZIO)
                    Dim P_DT_USCITA As String = dt_uscita
                    Dim P_DT_CONFERMA_ASL As String = dt_conferma_asl

                    If (dt_uscita > dt_conferma_asl) Then
                        dt_conferma_asl = dt_uscita
                        P_DT_CONFERMA_ASL = dt_uscita
                    End If

                    Dim P_FLAG_MACELLO_1 As String = ""
                    Dim P_FLAG_MACELLO_2 As String = ""
                    Dim P_FLAG_MACELLO_2A As String = ""
                    Dim P_FLAG_MACELLO_2B As String = ""
                    Dim P_FLAG_MACELLO_2C As String = ""

                    Dim P_FLAG_MACELLO_3 As String = "N"
                    Dim P_FLAG_MACELLO_3_ENTERICI As String = ""
                    Dim P_FLAG_MACELLO_3_RESPIRATORI As String = ""
                    Dim P_FLAG_MACELLO_3_CUTANEI As String = ""
                    Dim P_FLAG_MACELLO_3_LOCOMOTORI As String = ""
                    Dim P_FLAG_MACELLO_3_ALTRO As String = ""
                    Dim P_FLAG_MACELLO_3_ALTRO_DESC As String = ""
                    Dim P_FLAG_MACELLO_4 As String = "N"
                    Dim P_FLAG_MACELLO_5 As String = "N"
                    Dim P_FLAG_ELEMENTI As String = ""
                    Dim P_FLAG_RILEVAZIONI As String = ""
                    Dim P_FLAG_ALTRO As String = ""
                    Dim P_FLAG_ALTRO_DESC As String = ""
                    Dim P_FLAG_MACELLO_6 As String = "N"
                    Dim P_VET_AZIENDALE As String = ""
                    Dim P_INDIRIZZO As String = ""
                    Dim P_TELEFONO As String = ""
                    Dim P_ISTAT As String = ""
                    Dim P_SIGLA As String = ""
                    Dim P_NUM_ISCR_ALBO As String = ""

                    Dim P_TRASP_TARGA_MOTRICE As String = ""
                    Dim P_TRASP_TARGA As String = ""
                    Dim P_TRASP_CONDUCENTE As String = ""
                    Dim P_TRASP_DENOM_TRASPORTATORE As String = ""
                    Dim P_TRASP_TARGA_RIMORCHIO As String = ""

                    Dim P_TRASP_NUM_AUTORIZZAZIONE As String = ""
                    Dim P_TRASP_DT_PARTENZA As String = ""
                    Dim P_TRASP_ORA_PARTENZA As String = ""
                    Dim P_TRASP_DURATA_VIAGGIO As String = ""
                    Dim P_TRASP_FLAG_MEZZO_PROPRIO As String = ""
                    Dim P_TRASP_COD_ASL_TRASP As String = ""
                    Dim P_TRASP_SL_COD_FISCALE As String = ""

                    Dim P_FLAG_USCITA_AUTOMATICA As String = ""


                    Dim P_XML_TRATTAMENTI = gestioneTrattamenti_RegistrazioneUscita()
                    Dim P_XML_ESAMI As List(Of XmlEsamiCapo_InvioModelli) = gestioneEsami_RegistrazioneUscita()

                    gestioneDichiarazioneICA_RegistrazioneUscita(P_FLAG_MACELLO_1,
                                                                    P_FLAG_MACELLO_2,
                                                                    P_FLAG_MACELLO_2A,
                                                                    P_FLAG_MACELLO_2B,
                                                                    P_FLAG_MACELLO_2C,
                                                                    P_FLAG_MACELLO_3,
                                                                    P_FLAG_MACELLO_3_ENTERICI,
                                                                    P_FLAG_MACELLO_3_RESPIRATORI,
                                                                    P_FLAG_MACELLO_3_CUTANEI,
                                                                    P_FLAG_MACELLO_3_LOCOMOTORI,
                                                                    P_FLAG_MACELLO_3_ALTRO,
                                                                    P_FLAG_MACELLO_3_ALTRO_DESC,
                                                                    P_FLAG_MACELLO_4,
                                                                    P_FLAG_MACELLO_5,
                                                                    P_FLAG_ELEMENTI,
                                                                    P_FLAG_RILEVAZIONI,
                                                                    P_FLAG_ALTRO,
                                                                    P_FLAG_ALTRO_DESC,
                                                                    P_FLAG_MACELLO_6,
                                                                    P_VET_AZIENDALE,
                                                                    P_INDIRIZZO,
                                                                    P_TELEFONO,
                                                                    P_ISTAT,
                                                                    P_SIGLA,
                                                                    P_NUM_ISCR_ALBO,
                                                                    dtModello4)

                    gestioneTrasporto(P_TRASP_TARGA_MOTRICE,
                                        P_TRASP_TARGA,
                                        P_TRASP_CONDUCENTE,
                                        P_TRASP_DENOM_TRASPORTATORE,
                                        P_TRASP_TARGA_RIMORCHIO,
                                        P_TRASP_NUM_AUTORIZZAZIONE,
                                        P_TRASP_DT_PARTENZA,
                                        P_TRASP_ORA_PARTENZA,
                                        P_TRASP_DURATA_VIAGGIO,
                                        P_TRASP_FLAG_MEZZO_PROPRIO,
                                        P_TRASP_COD_ASL_TRASP,
                                        P_TRASP_SL_COD_FISCALE,
                                        P_FLAG_USCITA_AUTOMATICA,
                                        TraspDenom,
                                        CodFiscale_Trasp,
                                        codiceAUSLTrasp,
                                        dtModello4)

                    'REGISTRAZIONE Modello4
                    Dim dtModelloRegistrato As New DataTable
                    dtModelloRegistrato = wsGestioneModello4.registraUscitaModello(prenotazioneId, Codice_Specie,
                                                                                   P_DT_USCITA, P_DT_CONFERMA_ASL,
                                                                                   P_FLAG_MACELLO_1, P_FLAG_MACELLO_2,
                                                                                   P_FLAG_MACELLO_2A, P_FLAG_MACELLO_2B, P_FLAG_MACELLO_2C,
                                                                                   P_XML_TRATTAMENTI, P_XML_ESAMI,
                                                                                   P_FLAG_MACELLO_3, P_FLAG_MACELLO_3_ENTERICI,
                                                                                   P_FLAG_MACELLO_3_RESPIRATORI, P_FLAG_MACELLO_3_CUTANEI, P_FLAG_MACELLO_3_LOCOMOTORI,
                                                                                   P_FLAG_MACELLO_3_ALTRO, P_FLAG_MACELLO_3_ALTRO_DESC,
                                                                                   P_FLAG_MACELLO_4, P_FLAG_MACELLO_5,
                                                                                   P_FLAG_ELEMENTI, P_FLAG_RILEVAZIONI,
                                                                                   P_FLAG_ALTRO, P_FLAG_ALTRO_DESC, P_FLAG_MACELLO_6,
                                                                                   P_VET_AZIENDALE, P_INDIRIZZO, P_TELEFONO, P_ISTAT,
                                                                                   P_SIGLA, P_NUM_ISCR_ALBO,
                                                                                   P_TRASP_TARGA_MOTRICE, P_TRASP_TARGA,
                                                                                   P_TRASP_CONDUCENTE, P_TRASP_DENOM_TRASPORTATORE,
                                                                                   P_TRASP_TARGA_RIMORCHIO, P_TRASP_NUM_AUTORIZZAZIONE,
                                                                                   P_TRASP_DT_PARTENZA, P_TRASP_ORA_PARTENZA,
                                                                                   P_TRASP_DURATA_VIAGGIO, P_TRASP_FLAG_MEZZO_PROPRIO,
                                                                                   P_TRASP_COD_ASL_TRASP, P_TRASP_SL_COD_FISCALE,
                                                                                   listaMatricoleCapi, P_FLAG_USCITA_AUTOMATICA)


                    'se il modello generato è già stato confermato, viene registrato
                    'If Not IsNothing(dtModelloRegistrato) AndAlso dtModelloRegistrato.Rows.Count > 0 Then
                    Agenda.Tipo_Accettazione = 1
                    GiasContext.Entry(Agenda).State = EntityState.Modified
                    Dim numMatricoleCapi = listaMatricoleCapi.Count
                    Dim numMatricoleCapiImportati = 0
                    If Not IsNothing(dtModelloRegistrato) AndAlso dtModelloRegistrato.Rows.Count > 0 Then
                        response.importato = "SI"
                        Try
                            For Each mat In listaMatricoleCapi
                                Dim matricola As String = mat

                                Try




                                    Dim mds = (From md In GiasContext.Movimenti_dettagli
                                               Join z In GiasContext.Zoo_Animali On md.PIVA Equals z.PIVA And md.Cod_Progetto Equals z.Cod_Progetto
                                               Where md.PIVA = Piva AndAlso md.Id_Agenda = idAgenda AndAlso
                                                    md.Id_Mov = idMov_Scarico AndAlso md.Elem_Cod = 300 And z.Matricola = matricola
                                               Select md, z).FirstOrDefault

                                    If IsNothing(mds) Then
                                        Throw New Exception("L'operazione (Id_Agenda=" & idAgenda & ") non contenente capi da scaricare")
                                    End If

                                    Dim codAnimale As Integer = mds.z.Cod_Progetto
                                    Dim idCapoBDN As Integer = mds.z.Id_Capo_BDN

                                    Dim idUscita As Integer = sincronizzatoreAnimale.trovaID_Uscita(AllevIdFiscale_Prov,
                                                                                                        CodiceAziendaBDN_Prov,
                                                                                                        mat)


                                    If idUscita > 0 Then
                                        mds.md.Id_Mov_Esterno = idUscita
                                        GiasContext.Entry(Agenda).State = EntityState.Modified

                                        logMovimentazioniBDN(Piva, Sa_Cod, idAgenda, Lav_Cod, enumCausaliBDN.InvMovUscitaModello4,
                                                              idUscita, idMov_Scarico, mds.md.Id_Mov_Det, codAnimale,
                                                             idCapoBDN, dtModelloRegistrato, dtModelloRegistrato, True, GiasContext)
                                        numMatricoleCapiImportati = numMatricoleCapiImportati + 1
                                    End If
                                Catch ex As Exception
                                    response.errore = response.errore & " Verificare Matricola:" & matricola & " err:" & ex.Message & " " & vbCrLf
                                End Try


                            Next
                        Catch ex As Exception

                        End Try


                    End If

                    listaModelli4Response.Add(response)
                    GiasContext.SaveChanges()

                    'Else
                    '	logMovimentazioniBDN(Piva, Sa_Cod, idAgenda, Lav_Cod, enumCausaliBDN.InvMovUscitaModello4,
                    '						 0, idMov_Scarico, 0, 0, 0, Nothing,
                    '						 Nothing, False, GiasContext)
                    'End If
                End If

            Catch ex As TokenBDNException
                response.importato = "NO"
                response.errore = ex.Message
                listaModelli4Response.Add(response)
                Throw ex
            Catch ex As BDNException
                response.importato = "NO"
                response.errore = ex.Message
                listaModelli4Response.Add(response)
            Catch ex As Exception
                response.importato = "NO"
                response.errore = ex.Message
                listaModelli4Response.Add(response)
            End Try
        Next

        Return listaModelli4Response

    End Function

    ''' <summary>
    ''' Aggiorna i dati dei modelli 4 non confermati
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="AllevIdFiscale_Prov"></param>
    ''' <param name="Registra_Modello"></param>
    ''' <param name="Operazioni_Agenda"></param>
    ''' <returns></returns>
    Public Function AggiornaModello4(ByVal Piva As String,
                                     ByVal Sa_Cod As Integer,
                                     ByVal AllevIdFiscale_Prov As String,
                                     ByVal Registra_Modello As Boolean,
                                     ByVal Operazioni_Agenda As List(Of Integer)) As List(Of InserisciModello4Response)
        Dim listMod4Updated_PrenotId As New List(Of String)
        Dim listMod4_Resp As New List(Of InserisciModello4Response)

        For Each idAgenda In Operazioni_Agenda
            Dim isModified As Boolean = False
            Dim modello4Response As New InserisciModello4Response

            Try
                Dim agenda = GiasContext.Agenda.Where(Function(a) a.PIVA = Piva AndAlso a.Id_Agenda = idAgenda).FirstOrDefault
                If IsNothing(agenda) Then Throw New Exception($"Operazione zootecnica (Id_Agenda={idAgenda}) non valida o inesistente")
                Dim lavCod As Integer = agenda.Lav_Cod

                Dim movScarico = GiasContext.Movimenti.
                    Where(Function(m) m.PIVA = Piva AndAlso m.Id_Agenda = idAgenda AndAlso m.Cau_Mov = CAU_SCARICO_CONSISTENZE).FirstOrDefault
                If IsNothing(movScarico) Then Throw New Exception($"Operazione zootecnica (Id_Agenda={idAgenda}) non valida o inesistente")
                Dim idMov_Scarico As Integer = movScarico.Id_Mov

                Dim movRegistrazione = GiasContext.Movimenti.
                    Where(Function(m) m.PIVA = Piva AndAlso m.Id_Agenda = idAgenda AndAlso m.Cau_Mov = CAU_REGISTRAZIONI).FirstOrDefault
                If IsNothing(movRegistrazione) Then Throw New Exception($"Operazione zootecnica (Id_Agenda={idAgenda}) non valida o inesistente")
                Dim idMov_Registrazione As Integer = movRegistrazione.Id_Mov

                Dim movDettScarico = GiasContext.Movimenti_dettagli.
                    Where(Function(md) md.PIVA = Piva AndAlso md.Id_Agenda = idAgenda AndAlso md.Id_Mov = idMov_Scarico).ToList
                If IsNothing(movDettScarico) OrElse movDettScarico.Count = 0 Then Throw New Exception($"L'operazione (Id_Agenda={idAgenda}) non contiene capi da scaricare")

                Dim movDestScarico = GiasContext.Mov_Destinazioni.
                    Where(Function(md) md.Piva = Piva AndAlso md.Id_Agenda = idAgenda AndAlso md.Id_Mov = idMov_Scarico).ToList
                If IsNothing(movDestScarico) OrElse movDestScarico.Count = 0 Then Throw New Exception($"L'operazione (Id_Agenda={idAgenda}) non contiene capi da scaricare")

                Dim movDettTecnicoEx = GiasContext.Mov_Dettaglio_Tecnico_Extra.
                    Where(Function(md) md.Piva = Piva AndAlso md.Id_Agenda = idAgenda AndAlso md.Id_Mov = idMov_Registrazione).FirstOrDefault
                If IsNothing(movDettTecnicoEx) Then Throw New Exception($"Dati di trasporto per l'operazione (Id_Agenda={idAgenda}) non presenti")

                Dim stalla_R As New Stalla_R
                Dim raggruppamento_cod = movDestScarico(0).Id_Destinazione
                Dim raggruppamento = GiasContext.Stalla_Raggruppamenti.Where(Function(sr) sr.Raggruppamento_Cod = raggruppamento_cod).FirstOrDefault
                Dim staNum As Integer = raggruppamento.STA_NUM
                If Sa_Cod = 0 Then Sa_Cod = raggruppamento.sa_cod

                Dim dtStalla As DataTable = stalla_R.Leggi(Piva, Sa_Cod, staNum,
                                                           enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                           "", "", objParametriServer)
                If IsNothing(dtStalla) OrElse dtStalla.Rows.Count = 0 Then Throw New Exception("Stalla di provenienza non valida o inesistente")

                Dim codifSpecie_R As New Codifica_BDN_SpecieAnimali
                Dim dtCodifica As DataTable = codifSpecie_R.leggi(objParametriServer, "", "",
                                                                  dtStalla(0)("GEN_COD"), dtStalla(0)("SPE_COD"), "3")
                If IsNothing(dtCodifica) OrElse dtCodifica.Rows.Count = 0 Then Throw New GiasException("Codifica specie mancante per BDN")
                Dim specieCod As String = "0" & dtCodifica.Rows(0)("CODICE")

                Dim codAziendaBDN_Prov As String = dtStalla(0)("BDN_Codice_Azienda")

                Dim estremiModello4 As String = wsInterrogazioniModello4.calcolaEstremiModello(codAziendaBDN_Prov, specieCod, "AL")

                ' Se i capi sono presenti in BDN, aggiunge le matricole al modello da modificare
                Dim listCapi_CodAnimale = movDettScarico.Select(Function(md) md.Cod_Progetto).ToList

                Dim listCapi_Matricola As New List(Of String)
                For Each codAnimale In listCapi_CodAnimale
                    Dim existBDN As Boolean = True
                    Dim capo = GiasContext.Zoo_Animali.Where(Function(c) c.Cod_Progetto = codAnimale).FirstOrDefault

                    If Not IsNothing(capo) Then
                        ' Check presenza del capo in BDN
                        Dim dtAnimaleBDN As New DataTable
                        Try
                            dtAnimaleBDN = wsAnagraficaCapo.getCapo(capo.Matricola)
                        Catch ex As Exception
                            If ex.Message.Split(">")(1) = "CODICE CAPO BOVINO NON PRESENTE IN ANAGRAFE" Then
                                existBDN = False
                                Exit Try
                            Else
                                Throw New BDNException("Errore: " & MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
                            End If
                        End Try

                        If existBDN Then listCapi_Matricola.Add(capo.Matricola)
                    End If
                Next
                modello4Response.listaAnimali = listCapi_Matricola

                modello4Response.importato = "NO"
                modello4Response.registrato = "NO"

                ' Check esistenza modello 4
                Dim prenotIdToCheck As String = movDettTecnicoEx.Num_Riferimento
                Dim dsCheckPrenotazioneMod4 = wsInterrogazioniModello4.getPrenotazioneModelloDettaglio(prenotIdToCheck, specieCod)
                If IsNothing(dsCheckPrenotazioneMod4) OrElse dsCheckPrenotazioneMod4.Tables.Count = 0 Then Throw New BDNException($"Documento di accompagnamento non trovato (Id prenotazione {prenotIdToCheck}). ")

                ' Recupero dati modello 4
                Dim dtModello4 As DataTable = dsCheckPrenotazioneMod4.Tables(0)

                Dim objModello4 As New CaricaModelli4_Response
                Dim listaCapiMatxPrenotId As New List(Of Tuple(Of String, String))
                If Not IsNothing(dtModello4) AndAlso dtModello4.Rows.Count > 0 Then
                    listMod4Updated_PrenotId.Add(prenotIdToCheck)

                    objModello4.prenotazioneId = If(dtModello4.Columns.Contains("PRENOTAZIONE_ID"), dtModello4.Rows(0).Item("PRENOTAZIONE_ID"), "")
                    objModello4.documentoId = If(dtModello4.Columns.Contains("DOCUMENTO_ID"), dtModello4.Rows(0).Item("DOCUMENTO_ID"), "")
                    objModello4.numModello = If(dtModello4.Columns.Contains("NUM_MODELLO"), dtModello4.Rows(0).Item("NUM_MODELLO"), "")
                    objModello4.giorniValidita = If(dtModello4.Columns.Contains("GIORNI_VALIDITA"), dtModello4.Rows(0).Item("GIORNI_VALIDITA"), 0)
                    objModello4.tipoDestinazione = If(dtModello4.Columns.Contains("TIPOLOGIA_DEST"), dtModello4.Rows(0).Item("TIPOLOGIA_DEST"), "")
                    If objModello4.tipoDestinazione = "MACELLO" Then
                        objModello4.codAzienda_Dest = If(dtModello4.Columns.Contains("MACELLO_CODICE"), dtModello4.Rows(0).Item("MACELLO_CODICE"), "")
                    Else
                        objModello4.codAzienda_Dest = If(dtModello4.Columns.Contains("DEST_AZIENDA_CODICE"), dtModello4.Rows(0).Item("DEST_AZIENDA_CODICE"), "")
                    End If
                    objModello4.dataUscita = If(dtModello4.Columns.Contains("DT_USCITA"), dtModello4.Rows(0).Item("DT_USCITA"), AGRODATAINIZIO)
                    objModello4.dataDocumento = If(dtModello4.Columns.Contains("DT_DOCUMENTO"), dtModello4.Rows(0).Item("DT_DOCUMENTO"), AGRODATAINIZIO)

                    modello4Response.NumModello4 = objModello4.numModello
                    modello4Response.CodiceModello4 = objModello4.documentoId

                    Dim listCapiMod4 = dsCheckPrenotazioneMod4.Tables(1).ToExpandoObject.ToList()
                    objModello4.listaMatricoleCapi = listCapiMod4.Select(Function(c) CStr(c.Item("CAPO_CODICE"))).ToList
                    listaCapiMatxPrenotId = listCapiMod4.Select(Function(c) New Tuple(Of String, String)(CStr(c.Item("CAPO_CODICE")), CStr(c.Item("PRENOT_ID")))).ToList
                Else
                    Throw New BDNException($"Documento di accompagnamento non trovato (Id prenotazione {prenotIdToCheck}). ")
                End If

                'Dim codAziendaBDN_Dest As String = ""
                'Dim allevIdFiscale_Dest As String = ""

                'Dim contatti_R As New Contatti_R
                'Dim risorseUmane_R As New Risorse_Umane_R
                'Dim dtContatto = contatti_R.Leggi(movRegistrazione.PIVA, "",
                '								  movRegistrazione.Cod_Destinazione,
                '								  0, True, False,
                '								  movRegistrazione.Cod_IndirizzoDestinazione,
                '								  0, False, 0, ID_CF_NOFILTRO,
                '								  0, "", False,
                '								  0, 0, 0, 0, 0,
                '								  AGRODATAINIZIO, AGRODATAFINE, False,
                '								  "", "", objParametriServer)

                'If IsNothing(dtContatto) OrElse dtContatto.Rows.Count = 0 Then Throw New Exception("Stalla di destinazione non valida o inesistente")

                'Dim dtRisUm = risorseUmane_R.Leggi3(Piva, dtContatto(0)("Codice_Fiscale"),
                '								   0, 0, "", "",
                '								   objParametriServer)

                'If IsNothing(dtRisUm) OrElse dtRisUm.Rows.Count = 0 Then
                '	dtRisUm = risorseUmane_R.Leggi3("", dtContatto(0)("Cod_Contatto"),
                '									   0, 0, "", "",
                '									   objParametriServer)
                'End If
                'If IsNothing(dtRisUm) OrElse dtRisUm.Rows.Count = 0 Then Throw New Exception("Stalla di destinazione non valida o inesistente")

                'codAziendaBDN_Dest = dtContatto(0)("Attivita_Des")

                'Dim codAllevamento_Dest As String = If(lavCod <> LAVCOD_MACELLAZIONE_ANIMALI, codAziendaBDN_Dest, "")
                'Dim codMacello_Dest As String = If(lavCod = LAVCOD_MACELLAZIONE_ANIMALI, codAziendaBDN_Dest, "")
                'allevIdFiscale_Dest = dtContatto(0)("Codice_Fiscale")
                'Dim codContatto_Dest As String = dtContatto(0)("Cod_Contatto")
                'Dim pivaContatto_Dest As String = dtContatto(0)("Piva")
                'Dim ragSoc_Dest As String =
                '	dtContatto(0)("Rag_Soc") &
                '	CStr(dtContatto(0)("Cognome")) & " " & CStr(dtContatto(0)("Nome"))

                'Dim causaleCod As String = If(lavCod = LAVCOD_MACELLAZIONE_ANIMALI, "M", "V")
                'Dim codiceStato As String = "IT"
                'Dim codiceRegione As String = If(lavCod = LAVCOD_MACELLAZIONE_ANIMALI,
                '	RegioneDaContatto(ragSoc_Dest, pivaContatto_Dest, codContatto_Dest, "Macello"),
                '	"")

                'Dim objFlagMacello As New objFlagMacello
                'If lavCod = LAVCOD_MACELLAZIONE_ANIMALI AndAlso movDettTecnicoEx.Precisazioni <> "" Then
                '	objFlagMacello = New objFlagMacello(movDettTecnicoEx.Precisazioni)
                'End If

                'Dim xmlListaCapi As List(Of XmlCapi_InvioModelli) = objModello4.listaMatricoleCapi.
                '	Select(Function(mat) New XmlCapi_InvioModelli With {
                '		.capoCodice = mat,
                '		.codiceElettronico = "",
                '		.identNome = "",
                '		.passaporto = "",
                '		.codiceUeln = ""
                '	}).ToList

                'Dim dataUscita As Date = movRegistrazione.Extra_Date

                '' VETERINARIO
                'Dim vetNomeCognome As String = ""
                'Dim vetCognomeNome As String = ""
                'Dim vetIndirizzo As String = ""
                'Dim vetTelefono As String = ""
                'Dim vetComIstat As String = ""
                'Dim vetProIstat As String = ""

                'Dim cruVeterinario As Integer = movDettTecnicoEx.Cod_RisUm_Extra
                'If cruVeterinario <> 0 Then
                '	Dim vet = GiasContext.Risorse_Umane _
                '		.Where(Function(ru) ru.Cod_Rapporto = COD_VETERINARIO) _
                '		.Join(GiasContext.Contatti, Function(ru) ru.Piva, Function(co) co.Piva,
                '		  Function(ru, co) New With {Key ru, Key co}) _
                '		.Where(Function(x) x.co.Cod_Contatto = x.ru.Cod_Contatto AndAlso x.co.Piva = Piva) _
                '		.Select(Function(v) New With {
                '			Key .Cod_Contatto = v.co.Cod_Contatto,
                '			Key .Cognome = v.co.Cognome,
                '			Key .Nome = v.co.Nome,
                '			Key .Cod_RisUm = v.ru.Cod_RisUm
                '		}).FirstOrDefault()
                '	Dim vetCodContatto As String = vet.Cod_Contatto
                '	vetNomeCognome = $"{vet.Nome} {vet.Cognome}"
                '	vetCognomeNome = $"{vet.Cognome} {vet.Nome}"

                '	Dim indirizzi = GiasContext.ContattiXIndirizzi _
                '		.Where(Function(i) i.Cod_Contatto = vetCodContatto) _
                '		.Join(GiasContext.Indirizzi, Function(coxin) coxin.Cod_Indirizzo, Function(ind) ind.cod_indirizzo,
                '			  Function(coxin, ind) New With {Key coxin, Key ind}) _
                '		.Join(GiasContext.ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166, Function(x) x.ind.stato, Function(paesi) paesi.Codice,
                '			  Function(x, paesi) New With {Key x.coxin, Key x.ind, Key paesi}) _
                '		.Where(Function(x) x.ind.stato = x.paesi.Codice OrElse x.ind.stato = x.paesi.Descrizione) _
                '		.Join(GiasContext.Lista_Province, Function(x) x.ind.pro_cod_istat, Function(province) province.PROV,
                '			  Function(x, province) New With {Key x.coxin, Key x.ind, Key x.paesi, Key province}) _
                '		.Join(GiasContext.ISTAT_Comuni,
                '			  Function(x) New With {.Com_Cod_Istat = x.ind.com_cod_istat, .Pro_Cod_Istat = x.province.PROV},
                '			  Function(comuni) New With {.Com_Cod_Istat = comuni.Com_Cod_Istat, .Pro_Cod_Istat = comuni.Pro_Cod_Istat},
                '			  Function(x, comuni) New With {Key x.coxin, Key x.ind, Key x.paesi, Key x.province, Key comuni}) _
                '		.Select(Function(i) New With {
                '				Key .Indirizzo_Cod = i.ind.cod_indirizzo,
                '				Key .Indirizzo_Des = i.ind.ind_des,
                '				Key .Stato = If(i.ind.stato, ""),
                '				Key .Pro_Cod_Istat = If(i.ind.pro_cod_istat, ""),
                '				Key .Provincia_Sigla = If(i.province.SIGLA, ""),
                '				Key .Com_Cod_Istat = If(i.ind.com_cod_istat, "")
                '		}).ToList()
                '	If indirizzi IsNot Nothing AndAlso indirizzi.Count > 0 Then
                '		Dim indirizzo = indirizzi.FirstOrDefault(Function(i) i.Stato = "IT")
                '		If indirizzo Is Nothing Then indirizzo = indirizzi.FirstOrDefault()

                '		vetIndirizzo = indirizzo.Indirizzo_Des
                '		vetComIstat = indirizzo.Com_Cod_Istat
                '		vetProIstat = indirizzo.Pro_Cod_Istat
                '	End If

                '	vetTelefono = GiasContext.ContattiXRubrica _
                '		.Where(Function(coxrub) vet.Cod_Contatto = coxrub.Cod_Contatto) _
                '		.Join(GiasContext.Rubrica, Function(coxrub) coxrub.Cod_Rubrica, Function(rub) rub.cod_rubrica,
                '			  Function(coxrub, rub) New With {Key coxrub, Key rub}) _
                '		.Where(Function(x) x.rub.descr.Contains("elefono")) _
                '		.Select(Function(x) x.rub.numero) _
                '		.FirstOrDefault(Function(x) x <> "" And x <> "#")
                'End If

                '' TRASPORTATORE
                'Dim targaMotrice As String = ""
                'Dim targaRimorchio As String = ""
                'Dim conducente As String = ""
                'Dim numAutorizzazione As String = ""
                'Dim tipoTrasporto As String = movDettTecnicoEx.Annotazioni
                'Dim pivaAsl_Trasp As String = movDettTecnicoEx.Trasportatore
                'Dim dataPartenza_Trasp As Date = dataUscita
                'Dim oraPartenza_Trasp As String = dataUscita.ToString("HH:mm")
                'Dim durataViaggio As String = movDettTecnicoEx.Durata_Viaggio
                'Dim codiceAsl_Trasp As String = ""
                'Dim codFiscale_Trasp As String = ""
                'Dim denom_Trasp As String = ""

                'If tipoTrasporto = "S" Or tipoTrasporto = "N" Then
                '	targaMotrice = movDettTecnicoEx.Targa
                '	targaRimorchio = movDettTecnicoEx.Codice_Alternativo
                '	conducente = movDettTecnicoEx.Indicazioni_Complementari
                '	numAutorizzazione = movDettTecnicoEx.N_Autorizzazione_Trasporto

                '	dtContatto = contatti_R.LeggiContattoSpecifico(movRegistrazione.PIVA, pivaAsl_Trasp, 99,
                '												   enumSelezioneVariabile.Selezione_TabellaCompleta,
                '												   "", "", objParametriServer)
                '	If IsNothing(dtContatto) OrElse dtContatto.Rows.Count = 0 Then
                '		dtContatto = contatti_R.LeggiContattoSpecifico("", pivaAsl_Trasp, 99,
                '													   enumSelezioneVariabile.Selezione_TabellaCompleta,
                '													   "", "", objParametriServer)
                '		If Not IsNothing(dtContatto) AndAlso dtContatto.Rows.Count > 0 Then
                '			codFiscale_Trasp = dtContatto(0)("Codice_Fiscale")
                '			denom_Trasp = dtContatto(0)("Rag_Soc")
                '		End If
                '	End If

                '	dtRisUm = risorseUmane_R.Leggi3(movRegistrazione.PIVA, pivaAsl_Trasp, 0, COD_TRASPORTATORE,
                '									"", "", objParametriServer)
                '	If IsNothing(dtRisUm) OrElse dtRisUm.Rows.Count = 0 Then
                '		dtRisUm = risorseUmane_R.Leggi3("", pivaAsl_Trasp, 0, COD_TRASPORTATORE,
                '										"", "", objParametriServer)
                '	End If

                '	If Not IsNothing(dtRisUm) AndAlso dtRisUm.Rows.Count > 0 Then
                '		codiceAsl_Trasp = dtRisUm(0)("Settore_Des")
                '		If codiceAsl_Trasp = "" Then
                '			Dim indirizzi_R As New Indirizzi_R
                '			codiceAsl_Trasp = indirizzi_R.CodiceAslDatoContatto(Piva, pivaAsl_Trasp, objParametriServer)
                '		End If
                '	End If
                'End If

                '-- Fine recupero dati modello 4 --

                'Dim asdasdsa = 0
                ' TODO Errori presenti:
                ' - Estremi_Modello non accettato (chissa' qual e' il problema)
                ' - Xml Trattamenti e Esami non accettato se vuoto(?)
                ' Update Modello 4 (no dati trasportatore o aggiunta/cancellazione capi)
                'Dim dtMod4Updated As DataTable =
                '		wsGestioneModello4.aggiornaPrenotazioneModello(P_PRENOTAZIONE_ID:=prenotIdToCheck,
                '													   P_DOCUMENTO_ID:=objModello4.documentoId,
                '													   P_AZIENDA_CODICE:=codAziendaBDN_Prov,
                '													   P_ALLEV_ID_FISCALE:=AllevIdFiscale_Prov,
                '													   P_SPECIE_CODICE:=specieCod,
                '													   P_DEST_AZIENDA_CODICE:=codAllevamento_Dest,
                '													   P_DEST_ALLEV_ID_FISCALE:=allevIdFiscale_Dest,
                '													   P_DEST_SPECIE_CODICE:=specieCod,
                '													   P_FIERA_CODICE:="",
                '													   P_STATO_CODICE:=codiceStato,
                '													   P_PASCOLO_CODICE:="",
                '													   P_MACELLO_CODICE:=codMacello_Dest,
                '													   P_REGIONE_CODICE:=codiceRegione,
                '													   P_ESTREMI_DOCUMENTO:=estremiModello4,
                '													   Lista_Capi:=xmlListaCapi,
                '													   P_DT_USCITA:=dataUscita,
                '													   P_CAUSALE:=causaleCod,
                '													   P_TIPO_STAMPA:="",
                '													   P_FLAG_MACELLO_1:=objFlagMacello.macello1,
                '													   P_FLAG_MACELLO_2:=objFlagMacello.macello2,
                '													   P_FLAG_MACELLO_2A:=objFlagMacello.macello2A,
                '													   P_FLAG_MACELLO_2B:=objFlagMacello.macello2B,
                '													   P_FLAG_MACELLO_2C:=objFlagMacello.macello2C,
                '													   Lista_Trattamenti:=New List(Of XmlTrattamenti_InvioModelli),
                '													   Lista_Esami:=New List(Of XmlEsamiCapo_InvioModelli),
                '													   P_FLAG_MACELLO_3:=objFlagMacello.macello3,
                '													   P_FLAG_MACELLO_3_ENTERICI:=objFlagMacello.macello3_Enterici,
                '													   P_FLAG_MACELLO_3_RESPIRATORI:=objFlagMacello.macello3_Respiratori,
                '													   P_FLAG_MACELLO_3_CUTANEI:=objFlagMacello.macello3_Cutanei,
                '													   P_FLAG_MACELLO_3_LOCOMOTORI:=objFlagMacello.macello3_Locomotori,
                '													   P_FLAG_MACELLO_3_ALTRO:=objFlagMacello.macello3_Altro,
                '													   P_FLAG_MACELLO_3_ALTRO_DESC:=objFlagMacello.macello3_AltroDesc,
                '													   P_FLAG_MACELLO_4:=objFlagMacello.macello4,
                '													   P_FLAG_MACELLO_5:=objFlagMacello.macello5,
                '													   P_FLAG_ELEMENTI:=objFlagMacello.macello5_Elementi,
                '													   P_FLAG_RILEVAZIONI:=objFlagMacello.macello5_Rilevazioni,
                '													   P_FLAG_ALTRO:=objFlagMacello.macello5_Altro,
                '													   P_FLAG_ALTRO_DESC:=objFlagMacello.macello5_AltroDesc,
                '													   P_FLAG_UPLOAD:="",
                '													   P_FLAG_MACELLO_6:=objFlagMacello.macello6,
                '													   P_VET_AZIENDALE:=vetCognomeNome,
                '													   P_INDIRIZZO:=vetIndirizzo,
                '													   P_TELEFONO:=vetTelefono,
                '													   P_ISTAT:=vetComIstat,
                '													   P_SIGLA:=vetProIstat,
                '													   P_NUM_ISCR_ALBO:="",
                '													   P_TRASP_TARGA_MOTRICE:=targaMotrice,
                '													   P_TRASP_TARGA:="",
                '													   P_TRASP_CONDUCENTE:=conducente,
                '													   P_TRASP_DENOM_TRASPORTATORE:=denom_Trasp,
                '													   P_TRASP_TARGA_RIMORCHIO:=targaRimorchio,
                '													   P_TRASP_NUM_AUTORIZZAZIONE:=numAutorizzazione,
                '													   P_TRASP_DT_PARTENZA:=dataPartenza_Trasp,
                '													   P_TRASP_ORA_PARTENZA:=oraPartenza_Trasp,
                '													   P_TRASP_DURATA_VIAGGIO:=durataViaggio,
                '													   P_TRASP_FLAG_MEZZO_PROPRIO:=tipoTrasporto,
                '													   P_TRASP_COD_ASL_TRASP:=codiceAsl_Trasp,
                '													   P_TRASP_SL_COD_FISCALE:=pivaAsl_Trasp,
                '													   P_FLAG_USCITA_AUTOMATICA:="S",
                '													   P_CONFERMA_BDR:="S",
                '													   P_DT_DOCUMENTO:=objModello4.dataDocumento,
                '													   P_GIORNI_VALIDITA:="",
                '													   P_FLAG_TIPO_NOTA:="",
                '													   P_NOTA:="",
                '													   P_VETERINARIO:=vetNomeCognome,
                '													   P_DETEN_PAS_ID_FISCALE:="",
                '													   P_SIGLA_AUTOC:="",
                '													   P_ISTAT_AUTOC:="",
                '													   P_ID_FISCALE_AUTOC:="",
                '													   P_DT_RIENTRO:=AGRODATAINIZIO,
                '													   P_DESCR_PERCORSO:="",
                '													   P_SIGLA_DEST:="",
                '													   P_ISTAT_DEST:="")

                '' Check dell'avvenuto inserimento
                'If Not IsNothing(dtMod4Updated) AndAlso dtMod4Updated.Rows.Count > 0 Then
                '	isModified = True
                '	modello4Response.importato = "SI"
                '	modello4Response.registrato = "NO"

                '	Dim prenotazioneId As String = dtMod4Updated(0)("PRENOTAZIONE_ID")
                '	Dim dsPrenotazioneModello = wsInterrogazioniModello4.getPrenotazioneModello(prenotazioneId, specieCod)
                'Else
                '	logMovimentazioniBDN(Piva, Sa_Cod, idAgenda, lavCod, enumCausaliBDN.InvMovUscitaModello4,
                '						 0, movScarico.Id_Mov, 0, 0, 0, Nothing,
                '						 Nothing, False, GiasContext)
                'End If

                ' Update capi del Modello 4 (aggiunta o cancellazione)

                Dim listCapiMod4_ToAdd = listCapi_Matricola.Except(objModello4.listaMatricoleCapi).ToList
                If listCapiMod4_ToAdd.Count > 0 Then
                    For Each matricola In listCapiMod4_ToAdd
                        Dim dtResp = wsGestioneModello4.insertCapoModello(P_LISTA_PRENOT_ID:=0,
                                                                          P_PRENOTAZIONE_ID:=prenotIdToCheck,
                                                                          P_CODICE_CAPO:=matricola,
                                                                          P_DT_USCITA:=objModello4.dataDocumento,
                                                                          P_FLAG_ESAMINATO:="")
                        If Not IsNothing(dtResp) AndAlso dtResp.Rows.Count > 0 Then
                            Dim aasdasd = 0
                        End If

                        isModified = True

                        Dim capo = GiasContext.Zoo_Animali.Where(Function(c) c.Matricola = matricola).FirstOrDefault
                        capo.Modello4_Uscita_Numero = objModello4.numModello
                        capo.Modello4_Uscita = objModello4.documentoId
                        capo.Modello4_Uscita_Prenotazione = objModello4.prenotazioneId
                        capo.Codice_Azienda_Uscita = objModello4.codAzienda_Dest
                        capo.Data_Documento_Uscita = objModello4.dataUscita
                        capo.Codice_Azienda_Fornitore = codAziendaBDN_Prov
                        GiasContext.Entry(capo).State = EntityState.Modified
                    Next
                End If

                Dim listCapiMod4_ToRemove = objModello4.listaMatricoleCapi.Except(listCapi_Matricola).ToList
                If listCapiMod4_ToRemove.Count > 0 Then
                    For Each matricola In listCapiMod4_ToRemove
                        Dim capoPrenotId As String = listaCapiMatxPrenotId.Where(Function(c) c.Item1 = matricola).FirstOrDefault.Item2

                        Dim dtResp = wsGestioneModello4.deleteCapoModello(P_LISTA_PRENOT_ID:=capoPrenotId,
                                                                          P_PRENOTAZIONE_ID:=prenotIdToCheck,
                                                                          P_CODICE_CAPO:=matricola,
                                                                          P_DT_USCITA:=objModello4.dataDocumento,
                                                                          P_FLAG_ESAMINATO:="")
                        If Not IsNothing(dtResp) AndAlso dtResp.Tables.Count > 0 Then
                            Dim aasdasd = 0
                        End If

                        isModified = True

                        Dim capo = GiasContext.Zoo_Animali.Where(Function(c) c.Matricola = matricola).FirstOrDefault
                        capo.Modello4_Uscita_Numero = ""
                        capo.Modello4_Uscita = ""
                        capo.Modello4_Uscita_Prenotazione = ""
                        capo.Codice_Azienda_Uscita = ""
                        capo.Data_Documento_Uscita = AGRODATAFINE
                        capo.Codice_Azienda_Fornitore = ""
                        GiasContext.Entry(capo).State = EntityState.Modified
                    Next
                End If

                If isModified Then
                    modello4Response.importato = "SI"

                    agenda.Blocco_Flag = 1
                    GiasContext.Entry(agenda).State = EntityState.Modified
                    GiasContext.SaveChanges()
                End If

            Catch ex As TokenBDNException
                Throw New BDNException("Token scaduto")

            Catch ex As BDNException
                If modello4Response.importato = "NO" Then
                    modello4Response.CodiceModello4 = ""
                    modello4Response.NumModello4 = ""
                    modello4Response.registrato = "NO"
                Else
                    modello4Response.registrato = "NO"
                End If

                If ex.Message.Contains("DL01") Then
                    modello4Response.errore = "Errore nel tentativo di download del modello"
                Else
                    modello4Response.errore = ex.Message
                End If
            Catch ex As Exception
                If modello4Response.importato = "NO" Then
                    modello4Response.CodiceModello4 = ""
                    modello4Response.NumModello4 = ""
                    modello4Response.registrato = "NO"
                Else
                    modello4Response.registrato = "NO"
                End If

                If ex.Message.Contains("DL01") Then
                    modello4Response.errore = "Errore nel tentativo di download del modello"
                Else
                    modello4Response.errore = ex.Message
                End If

            End Try

            listMod4_Resp.Add(modello4Response)
        Next

        Return listMod4_Resp

    End Function

    Public Sub gestioneDichiarazioneICA_RegistrazioneUscita(ByRef P_FLAG_MACELLO_1 As String,
                                                              ByRef P_FLAG_MACELLO_2 As String,
                                                              ByRef P_FLAG_MACELLO_2A As String,
                                                              ByRef P_FLAG_MACELLO_2B As String,
                                                              ByRef P_FLAG_MACELLO_2C As String,
                                                              ByRef P_FLAG_MACELLO_3 As String,
                                                              ByRef P_FLAG_MACELLO_3_ENTERICI As String,
                                                              ByRef P_FLAG_MACELLO_3_RESPIRATORI As String,
                                                              ByRef P_FLAG_MACELLO_3_CUTANEI As String,
                                                              ByRef P_FLAG_MACELLO_3_LOCOMOTORI As String,
                                                              ByRef P_FLAG_MACELLO_3_ALTRO As String,
                                                              ByRef P_FLAG_MACELLO_3_ALTRO_DESC As String,
                                                              ByRef P_FLAG_MACELLO_4 As String,
                                                              ByRef P_FLAG_MACELLO_5 As String,
                                                              ByRef P_FLAG_ELEMENTI As String,
                                                              ByRef P_FLAG_RILEVAZIONI As String,
                                                              ByRef P_FLAG_ALTRO As String,
                                                              ByRef P_FLAG_ALTRO_DESC As String,
                                                              ByRef P_FLAG_MACELLO_6 As String,
                                                              ByRef P_VET_AZIENDALE As String,
                                                              ByRef P_INDIRIZZO As String,
                                                              ByRef P_TELEFONO As String,
                                                              ByRef P_ISTAT As String,
                                                              ByRef P_SIGLA As String,
                                                              ByRef P_NUM_ISCR_ALBO As String,
                                                              ByRef dtModello4 As DataTable)

        P_FLAG_MACELLO_1 = dtModello4.Rows(0).Item("FLAG_MACELLO_1")
        P_FLAG_MACELLO_2 = dtModello4.Rows(0).Item("FLAG_MACELLO_2")
        P_FLAG_MACELLO_2A = dtModello4.Rows(0).Item("FLAG_MACELLO_2A")
        P_FLAG_MACELLO_2B = dtModello4.Rows(0).Item("FLAG_MACELLO_2B")
        P_FLAG_MACELLO_2C = dtModello4.Rows(0).Item("FLAG_MACELLO_2C")

        If dtModello4.Columns.Contains("FLAG_MACELLO_3") Then
            P_FLAG_MACELLO_3 = dtModello4.Rows(0).Item("FLAG_MACELLO_3")
        End If

        If dtModello4.Columns.Contains("FLAG_MACELLO_3_ENTERICI") Then
            P_FLAG_MACELLO_3_ENTERICI = dtModello4.Rows(0).Item("FLAG_MACELLO_3_ENTERICI")
        End If

        If dtModello4.Columns.Contains("FLAG_MACELLO_3_RESPIRATORI") Then
            P_FLAG_MACELLO_3_RESPIRATORI = dtModello4.Rows(0).Item("FLAG_MACELLO_3_RESPIRATORI")
        End If

        If dtModello4.Columns.Contains("FLAG_MACELLO_3_CUTANEI") Then
            P_FLAG_MACELLO_3_CUTANEI = dtModello4.Rows(0).Item("FLAG_MACELLO_3_CUTANEI")
        End If

        If dtModello4.Columns.Contains("FLAG_MACELLO_3_LOCOMOTORI") Then
            P_FLAG_MACELLO_3_LOCOMOTORI = dtModello4.Rows(0).Item("FLAG_MACELLO_3_LOCOMOTORI")
        End If


        If dtModello4.Columns.Contains("P_FLAG_MACELLO_3_ALTRO") Then
            P_FLAG_MACELLO_3_ALTRO = ""
        End If


        If dtModello4.Columns.Contains("P_FLAG_MACELLO_3_ALTRO_DESC") Then
            P_FLAG_MACELLO_3_ALTRO_DESC = ""
        End If


        If dtModello4.Columns.Contains("P_FLAG_MACELLO_4") Then
            P_FLAG_MACELLO_4 = dtModello4.Rows(0).Item("FLAG_MACELLO_4")
        End If


        If dtModello4.Columns.Contains("P_FLAG_MACELLO_5") Then
            P_FLAG_MACELLO_5 = ""
        End If


        If dtModello4.Columns.Contains("FLAG_ELEMENTI") Then
            P_FLAG_ELEMENTI = dtModello4.Rows(0).Item("FLAG_ELEMENTI")
        End If


        If dtModello4.Columns.Contains("FLAG_RILEVAZIONI") Then
            P_FLAG_RILEVAZIONI = dtModello4.Rows(0).Item("FLAG_RILEVAZIONI")
        End If

        If dtModello4.Columns.Contains("FLAG_ALTRO") Then
            P_FLAG_ALTRO = dtModello4.Rows(0).Item("FLAG_ALTRO")
        End If

        If dtModello4.Columns.Contains("FLAG_ALTRO_DESC") Then
            P_FLAG_ALTRO_DESC = dtModello4.Rows(0).Item("FLAG_ALTRO_DESC")
        End If

        If dtModello4.Columns.Contains("FLAG_MACELLO_6") Then
            P_FLAG_MACELLO_6 = dtModello4.Rows(0).Item("FLAG_MACELLO_6")
        End If

        If dtModello4.Columns.Contains("VETERINARIO_AZIENDALE") Then
            P_VET_AZIENDALE = dtModello4.Rows(0).Item("VETERINARIO_AZIENDALE")
        End If

        If dtModello4.Columns.Contains("INDIRIZZO_VET_AZIENDALE") Then
            P_INDIRIZZO = dtModello4.Rows(0).Item("INDIRIZZO_VET_AZIENDALE")
        End If

        If dtModello4.Columns.Contains("TEL_VET_AZIENDALE") Then
            P_TELEFONO = dtModello4.Rows(0).Item("TEL_VET_AZIENDALE")
        End If

        If dtModello4.Columns.Contains("COM_ISTAT_VET_AZIENDALE") Then
            P_ISTAT = dtModello4.Rows(0).Item("COM_ISTAT_VET_AZIENDALE")
        End If

        If dtModello4.Columns.Contains("SIGLA_PRO_VET_AZIENDALE") Then
            P_SIGLA = dtModello4.Rows(0).Item("SIGLA_PRO_VET_AZIENDALE")
        End If

        If dtModello4.Columns.Contains("P_NUM_ISCR_ALBO") Then
            P_NUM_ISCR_ALBO = ""
        End If

    End Sub

    Public Function gestioneTrattamenti_RegistrazioneUscita() As List(Of XmlTrattamenti_InvioModelli)
        'P_XML_TRATTAMENTI
        '!!! Al momento passare con un solo elemento vuoto !!!
        Dim P_XML_TRATTAMENTI As New List(Of XmlTrattamenti_InvioModelli)
        'For Each tratt In listaTrattamentiCapo
        Dim TrattamentoXml As New XmlTrattamenti_InvioModelli
        TrattamentoXml.trattamentoFlagProntuario = ""
        TrattamentoXml.trattamentoCapoCodice = ""
        TrattamentoXml.trattamentoTipo = ""
        TrattamentoXml.trattamentoAic = ""
        TrattamentoXml.trattamentoDenominazione = ""
        TrattamentoXml.trattamentoConfezione = ""
        TrattamentoXml.trattamentoDataSomm = ""
        TrattamentoXml.trattamentoPeriodoSosp = ""

        P_XML_TRATTAMENTI.Add(TrattamentoXml)

        Return P_XML_TRATTAMENTI
    End Function

    Public Function gestioneEsami_RegistrazioneUscita() As List(Of XmlEsamiCapo_InvioModelli)
        Dim P_XML_ESAMI As New List(Of XmlEsamiCapo_InvioModelli)
        'For Each esame In listaEsamiCapo
        Dim EsameXml As New XmlEsamiCapo_InvioModelli
        EsameXml.esameCodice = ""
        EsameXml.esameAltroDesc = ""
        EsameXml.esameData = ""
        EsameXml.esameCodiceRis = ""
        EsameXml.esameCapoCodice = ""
        EsameXml.esameFlagTutti = ""

        Return P_XML_ESAMI
    End Function

    Public Sub gestioneTrasporto(ByRef P_TRASP_TARGA_MOTRICE As String,
                                        ByRef P_TRASP_TARGA As String,
                                        ByRef P_TRASP_CONDUCENTE As String,
                                        ByRef P_TRASP_DENOM_TRASPORTATORE As String,
                                        ByRef P_TRASP_TARGA_RIMORCHIO As String,
                                        ByRef P_TRASP_NUM_AUTORIZZAZIONE As String,
                                        ByRef P_TRASP_DT_PARTENZA As String,
                                        ByRef P_TRASP_ORA_PARTENZA As String,
                                        ByRef P_TRASP_DURATA_VIAGGIO As String,
                                        ByRef P_TRASP_FLAG_MEZZO_PROPRIO As String,
                                        ByRef P_TRASP_COD_ASL_TRASP As String,
                                        ByRef P_TRASP_SL_COD_FISCALE As String,
                                        ByRef P_FLAG_USCITA_AUTOMATICA As String,
                                        ByRef TraspDenom As String,
                                        ByRef CodFiscale_Trasp As String,
                                        ByRef codiceAUSLTrasp As String,
                                      ByRef dtModello4 As DataTable)
        If dtModello4.Columns.Contains("TARGA_MOTRICE") Then
            P_TRASP_TARGA_MOTRICE = dtModello4.Rows(0).Item("TARGA_MOTRICE")
        End If

        If dtModello4.Columns.Contains("P_TRASP_TARGA") Then
            P_TRASP_TARGA = dtModello4.Rows(0).Item("P_TRASP_TARGA")
        End If

        'If dtModello4.Columns.Contains("TARGA_MOTRICE") Then
        P_TRASP_CONDUCENTE = TraspDenom
        'End If

        'If dtModello4.Columns.Contains("TARGA_MOTRICE") Then
        P_TRASP_DENOM_TRASPORTATORE = TraspDenom
        'End If

        If dtModello4.Columns.Contains("TARGA_RIMORCHIO") Then
            P_TRASP_TARGA_RIMORCHIO = dtModello4.Rows(0).Item("TARGA_RIMORCHIO")
        End If

        If dtModello4.Columns.Contains("NUM_AUTORIZZAZIONE") Then
            P_TRASP_NUM_AUTORIZZAZIONE = dtModello4.Rows(0).Item("NUM_AUTORIZZAZIONE")
        End If

        If dtModello4.Columns.Contains("DT_PARTENZA") Then
            P_TRASP_DT_PARTENZA = dtModello4.Rows(0).Item("DT_PARTENZA")
        End If

        If dtModello4.Columns.Contains("ORA_PARTENZA") Then
            P_TRASP_ORA_PARTENZA = dtModello4.Rows(0).Item("ORA_PARTENZA")
        End If

        If dtModello4.Columns.Contains("DURATA_VIAGGIO") Then
            P_TRASP_DURATA_VIAGGIO = dtModello4.Rows(0).Item("DURATA_VIAGGIO")
        End If

        If dtModello4.Columns.Contains("FLAG_MEZZO_PROPRIO") Then
            P_TRASP_FLAG_MEZZO_PROPRIO = dtModello4.Rows(0).Item("FLAG_MEZZO_PROPRIO")
        End If

        P_TRASP_COD_ASL_TRASP = codiceAUSLTrasp

        'If dtModello4.Columns.Contains("TARGA_MOTRICE") Then
        P_TRASP_SL_COD_FISCALE = CodFiscale_Trasp
        'End If

        If dtModello4.Columns.Contains("FLAG_USCITA_AUTOMATICA") Then
            P_FLAG_USCITA_AUTOMATICA = dtModello4.Rows(0).Item("FLAG_USCITA_AUTOMATICA")
        End If
    End Sub

    ''' <summary>
    ''' Cancella le operazioni di carico già sincronizzate
    ''' </summary>
    ''' <param name="chiave"></param>
    ''' <returns></returns>
    Public Function CancellaOpCarico_Sincro(ByVal chiave As List(Of ChiaveCaricoScaricoCapi)) As List(Of CancellaOperazioneResponse)
        Dim listaIngressiCanc_response As New List(Of CancellaOperazioneResponse)
        Dim listaMatricole_CapiEliminati As New List(Of String)

        'ricavo Piva e Sa_Cod della Stalla dal primo elemento
        Dim Piva As String = chiave(0).Piva
        Dim Sa_Cod As Integer = chiave(0).Sa_Cod
        Dim Allev_IdFiscale As String = ""
        Dim CodiceAzienda_BDN As String = ""
        Dim Codice_Specie As String = ""

        Dim listaCodAnimale_CapiDelete As New List(Of Integer)
        For Each capo In chiave
            listaCodAnimale_CapiDelete.Add(capo.Cod_Progetto)
        Next

        For Each capo In chiave
            Dim resp As New CancellaOperazioneResponse

            Dim objZooAnimale As New AgronicaCoreAnagrafeDAL.Zoo_Animali
            Dim dtAnimaleDB As DataTable =
                objZooAnimale.BDN_Leggi_Movimenti_Carico_Sincronizzati(Piva, Sa_Cod,
                                                                       0, 0, capo.Cod_Progetto,
                                                                       AGRODATAINIZIO, AGRODATAFINE,
                                                                       False, Nothing,
                                                                       objParametriServer)
            Dim capoAnimale As DataRow = dtAnimaleDB.Rows(0)

            Dim Cod_Animale As Integer = capoAnimale("Cod_Progetto")
            Dim Matricola As String = capoAnimale("Matricola")
            Allev_IdFiscale = capoAnimale("CF_DETENTORE")
            CodiceAzienda_BDN = capoAnimale("BDN_Codice_Azienda")
            Dim Id_Agenda As Integer = capoAnimale("id_agenda")
            Dim Id_Mov As Integer = capoAnimale("Id_Mov")
            Dim Id_Mov_Det As Integer = capoAnimale("Id_Mov_Det")
            Dim ingressoId As Integer = capoAnimale("Id_Mov_Esterno")
            Dim dataIngresso As Date = capoAnimale("Data_Movimento")

            'ricava il SPECIE_CODICE dalla tab di mappatura CodificaBDN_SpecieAnimali
            Dim codificaBdnSpecie_R As New AgronicaCoreMetaSchemaDAL.Codifica_BDN_SpecieAnimali
            Dim dtSpecieFromCodificaBDN As DataTable = codificaBdnSpecie_R.leggi(objParametriServer, "", "",
                                                                                 capoAnimale("GEN_COD"),
                                                                                 capoAnimale("SPE_COD"),
                                                                                 3)
            If Not IsNothing(dtSpecieFromCodificaBDN) AndAlso dtSpecieFromCodificaBDN.Rows.Count > 0 Then
                Codice_Specie = "0" & dtSpecieFromCodificaBDN(0)("CODICE")
            End If

            resp.matricola = Matricola

            'controllo che il capo non sia già presente in BDN
            Dim CapoPresenteBDN As Boolean = True
            Dim dtAnimaleBDN As New DataTable
            Try
                dtAnimaleBDN = wsAnagraficaCapo.getCapo(Matricola)

            Catch ex As Exception
                If ex.Message.Split(">")(1) = "CODICE CAPO BOVINO NON PRESENTE IN ANAGRAFE" Then
                    CapoPresenteBDN = False
                    Exit Try
                Else
                    Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
                End If
            End Try

            If CapoPresenteBDN Then

                Try
                    Dim P_INGRESSO_ID As Integer = ingressoId
                    Dim P_AZIENDA_CODICE As String = CodiceAzienda_BDN
                    Dim P_ALLEV_ID_FISCALE As String = Allev_IdFiscale
                    Dim P_CAPO_CODICE As String = Matricola
                    Dim P_CODICE_SPECIE As String = Codice_Specie
                    Dim P_DT_INGRESSO As Date = dataIngresso

                    Dim dsDeleteIngresso = wsChiamawsRegistroCapiStalla.Delete_Ingresso(P_INGRESSO_ID, P_AZIENDA_CODICE,
                                                                                        P_ALLEV_ID_FISCALE, P_CAPO_CODICE,
                                                                                        P_CODICE_SPECIE, P_DT_INGRESSO)

                    'controlla se è stata effettuata la cancellazione
                    If Not IsNothing(dsDeleteIngresso) AndAlso dsDeleteIngresso.Tables.Count > 0 Then
                        Dim mds As AgronicaCoreEntityFramework_POCO.Movimenti_dettagli = (From md In GiasContext.Movimenti_dettagli
                                                                                          Where md.PIVA = Piva AndAlso md.Id_Agenda = Id_Agenda AndAlso
                                                                                              md.Id_Mov = Id_Mov AndAlso md.Id_Mov_Det = Id_Mov_Det
                                                                                          Select md).FirstOrDefault
                        If IsNothing(mds) Then
                            Throw New Exception("L'operazione (Id_Mov_Det=" & Id_Mov_Det & ") non contiene capi da scaricare")
                        End If

                        mds.Id_Mov_Esterno = 0
                        GiasContext.Entry(mds).State = EntityState.Modified

                        resp.cancellato = "SI"
                        resp.errore = ""
                    Else
                        resp.cancellato = "NO"
                        resp.errore = "Errore nella cancellazione dell'ingresso"
                    End If

                    GiasContext.SaveChanges()

                Catch ex As BDNException
                    resp.cancellato = "NO"
                    resp.errore = ex.Message
                    'Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
                Catch ex As Exception
                    Throw New Exception("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
                End Try

            Else
                resp.cancellato = "NO"
                resp.errore = "Capo non presente in BDN"
            End If

            listaIngressiCanc_response.Add(resp)
        Next

        Return listaIngressiCanc_response

    End Function

    ''' <summary>
    ''' Cancella le operazioni di scarico già sincronizzate
    ''' </summary>
    ''' <param name="chiave"></param>
    ''' <returns></returns>
    Public Function CancellaOpScarico_Sincro(ByVal chiave As List(Of ChiaveCaricoScaricoCapi)) As List(Of CancellaOperazioneResponse)
        Dim listaUsciteCanc_response As New List(Of CancellaOperazioneResponse)
        Dim listaMatricole_CapiEliminati As New List(Of String)

        'ricavo Piva e Sa_Cod della Stalla dal primo elemento
        Dim Piva As String = chiave(0).Piva
        Dim Sa_Cod As Integer = chiave(0).Sa_Cod

        For Each capo In chiave
            Dim objZooAnimale As New AgronicaCoreAnagrafeDAL.Zoo_Animali
            Dim dtAnimaleDB As DataTable =
                objZooAnimale.BDN_Leggi_Movimenti_Scarico_Sincronizzati(Piva, Sa_Cod,
                                                                        0, 0, capo.Cod_Progetto,
                                                                        AGRODATAINIZIO, AGRODATAFINE,
                                                                        False, Nothing,
                                                                        objParametriServer)
            Dim capoAnimale As DataRow = dtAnimaleDB.Rows(0)
            Dim Lav_Cod As Integer = capoAnimale("Lav_Cod")

            Dim resp As New CancellaOperazioneResponse
            Select Case Lav_Cod
                Case LAVCOD_MORTE_ANIMALI
                    resp = CancellaDecesso(Piva, Sa_Cod, capoAnimale)
                Case LAVCOD_VENDITA_ANIMALI, LAVCOD_MACELLAZIONE_ANIMALI
                    resp = CancellaUscita(Piva, Sa_Cod, capoAnimale)
            End Select
            resp.matricola = capoAnimale("Matricola")
            listaUsciteCanc_response.Add(resp)
        Next

        Return listaUsciteCanc_response

    End Function

    ''' <summary>
    ''' Cancella il capo dal decesso associato in BDN
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="capoAnimale"></param>
    ''' <returns></returns>
    Public Function CancellaDecesso(ByVal Piva As String,
                                    ByVal Sa_Cod As Integer,
                                    ByVal capoAnimale As DataRow) As CancellaOperazioneResponse
        Dim response As New CancellaOperazioneResponse

        Dim Id_Agenda As Integer = capoAnimale("Id_Agenda")
        Dim Cod_Animale As Integer = capoAnimale("Cod_Progetto")
        Dim Matricola As String = capoAnimale("Matricola")
        Dim Id_Mov As Integer = capoAnimale("Id_Mov")
        Dim Id_Mov_Det As Integer = capoAnimale("Id_Mov_Det")
        Dim uscitaId As Integer = capoAnimale("Id_Mov_Esterno")
        Dim aziendaCodice As String = capoAnimale("BDN_Codice_Azienda")
        Dim allevIdFiscale As String = capoAnimale("CF_DETENTORE")
        Dim dataUscita As Date = capoAnimale("Data_Movimento")

        'ricava il SPECIE_CODICE dalla tab di mappatura CodificaBDN_SpecieAnimali
        Dim Codice_Specie As String = ""
        Dim codificaBdnSpecie_R As New AgronicaCoreMetaSchemaDAL.Codifica_BDN_SpecieAnimali
        Dim dtSpecieFromCodificaBDN As DataTable = codificaBdnSpecie_R.leggi(objParametriServer, "", "",
                                                                             capoAnimale("GEN_COD"),
                                                                             capoAnimale("SPE_COD"),
                                                                             3)
        If Not IsNothing(dtSpecieFromCodificaBDN) AndAlso dtSpecieFromCodificaBDN.Rows.Count > 0 Then
            Codice_Specie = "0" & dtSpecieFromCodificaBDN(0)("CODICE")
        End If

        'controllo che il capo non sia già presente in BDN
        Dim dtAnimaleBDN As New DataTable
        Dim CapoPresenteBDN As Boolean = True
        Try
            dtAnimaleBDN = wsAnagraficaCapo.getCapo(Matricola)

        Catch ex As Exception
            If ex.Message.Split(">")(1) = "CODICE CAPO BOVINO NON PRESENTE IN ANAGRAFE" Then
                CapoPresenteBDN = False
                Exit Try
            Else
                Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
            End If
        End Try

        If CapoPresenteBDN Then

            Try
                Dim respDecesso = wsAnagraficaCapo.getDecesso(aziendaCodice, allevIdFiscale,
                                                              Codice_Specie, Matricola)

                If IsNothing(respDecesso) OrElse respDecesso.Rows.Count = 0 Then
                    Throw New Exception("Decesso non presente in BDN")
                End If

                Dim P_DECESSO_ID As Integer = respDecesso(0)("MORTE_ID")
                Dim P_CAPO_CODICE As String = Matricola
                Dim P_AZIENDA_CODICE As String = aziendaCodice
                Dim P_ALLEV_ID_FISCALE As String = allevIdFiscale
                Dim P_SPECIE_CODICE As String = Codice_Specie

                Dim risposta = wsChiamawsRegistroCapiStalla.Delete_Decessi(P_DECESSO_ID, P_CAPO_CODICE, P_AZIENDA_CODICE,
                                                                           P_ALLEV_ID_FISCALE, P_SPECIE_CODICE)

                If Not IsNothing(risposta) AndAlso risposta.Tables.Count > 0 Then
                    Dim mds As AgronicaCoreEntityFramework_POCO.Movimenti_dettagli = (From md In GiasContext.Movimenti_dettagli
                                                                                      Where md.PIVA = Piva AndAlso md.Id_Agenda = Id_Agenda AndAlso
                                                                                          md.Id_Mov = Id_Mov AndAlso md.Id_Mov_Det = Id_Mov_Det
                                                                                      Select md).FirstOrDefault
                    If IsNothing(mds) Then
                        Throw New Exception("L'operazione (Id_Mov_Det=" & Id_Mov_Det & ") non contiene capi da scaricare")
                    End If

                    mds.Id_Mov_Esterno = 0
                    GiasContext.Entry(mds).State = EntityState.Modified

                    'Dim capo As AgronicaCoreEntityFramework_POCO.Zoo_Animali = (From z In GiasContext.Zoo_Animali
                    '															Where z.Matricola = Matricola
                    '															Select z).First
                    'TODO?
                    'capo.Modello4_Uscita_Numero = ""
                    'capo.Modello4_Uscita = ""
                    'capo.Modello4_Uscita_Prenotazione = ""
                    'capo.Codice_Azienda_Uscita = ""
                    'capo.Data_Documento_Uscita = AGRODATAFINE
                    'GiasContext.Entry(capo).State = EntityState.Modified

                    GiasContext.SaveChanges()

                    response.cancellato = "SI"
                    response.errore = ""
                Else
                    response.cancellato = "NO"
                    response.errore = "Errore nella cancellazione del capo"
                End If

            Catch ex As BDNException
                'Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
                response.cancellato = "NO"
                response.errore = "Errore nella cancellazione del capo"
            Catch ex As Exception
                Throw New Exception("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
            End Try

        Else
            response.cancellato = "NO"
            response.errore = "Capo non presente in BDN"
        End If

        Return response

    End Function

    ''' <summary>
    ''' Cancella il capo dal modello 4 presente in BDN
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="capoAnimale"></param>
    ''' <returns></returns>
    Public Function CancellaUscita(ByVal Piva As String,
                                   ByVal Sa_Cod As Integer,
                                   ByVal capoAnimale As DataRow) As CancellaOperazioneResponse
        Dim resp As New CancellaOperazioneResponse

        Dim Id_Agenda As Integer = capoAnimale("Id_Agenda")
        Dim Cod_Animale As Integer = capoAnimale("Cod_Progetto")
        Dim Matricola As String = capoAnimale("Matricola")
        Dim Allev_IdFiscale As String = capoAnimale("CF_DETENTORE")
        Dim CodiceAzienda_BDN As String = capoAnimale("BDN_Codice_Azienda")
        Dim Id_Mov As Integer = capoAnimale("Id_Mov")
        Dim Id_Mov_Det As Integer = capoAnimale("Id_Mov_Det")
        Dim uscitaId As Integer = capoAnimale("Id_Mov_Esterno")
        Dim dataUscita As Date = capoAnimale("Data_Movimento")
        Dim Lav_Cod As Integer = capoAnimale("Lav_Cod")
        Dim prenotazioneId As String = capoAnimale("Modello4_Uscita_Prenotazione")

        Dim Codice_Specie As String = ""
        'ricava il SPECIE_CODICE dalla tab di mappatura CodificaBDN_SpecieAnimali
        Dim codificaBdnSpecie_R As New AgronicaCoreMetaSchemaDAL.Codifica_BDN_SpecieAnimali
        Dim dtSpecieFromCodificaBDN As DataTable = codificaBdnSpecie_R.leggi(objParametriServer, "", "",
                                                                             capoAnimale("GEN_COD"),
                                                                             capoAnimale("SPE_COD"),
                                                                             3)
        If Not IsNothing(dtSpecieFromCodificaBDN) AndAlso dtSpecieFromCodificaBDN.Rows.Count > 0 Then
            Codice_Specie = "0" & dtSpecieFromCodificaBDN(0)("CODICE")
        End If

        'controllo che il capo non sia già presente in BDN
        Dim dtAnimaleBDN As New DataTable
        Dim CapoPresenteBDN As Boolean = True
        Try
            dtAnimaleBDN = wsAnagraficaCapo.getCapo(Matricola)

        Catch ex As Exception
            If ex.Message.Split(">")(1) = "CODICE CAPO BOVINO NON PRESENTE IN ANAGRAFE" Then
                CapoPresenteBDN = False
                Exit Try
            Else
                Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
            End If
        End Try

        Dim motivoUscita = ""
        Select Case Lav_Cod
            Case LAVCOD_MACELLAZIONE_ANIMALI
                motivoUscita = "M"
            Case LAVCOD_VENDITA_ANIMALI
                motivoUscita = "V"
        End Select

        If CapoPresenteBDN Then

            Try
                Dim objModelli4 As New CaricaModelli4_Response
                If prenotazioneId <> "" Then
                    Dim dsPrenotazioneModello = wsInterrogazioniModello4.getPrenotazioneModello(prenotazioneId,
                                                                                                Codice_Specie)

                End If

                Dim P_USCITA_ID As Integer = uscitaId
                Dim P_AZIENDA_CODICE As String = CodiceAzienda_BDN
                Dim P_ALLEV_ID_FISCALE As String = Allev_IdFiscale
                Dim P_MOTIVO As String = motivoUscita
                Dim P_CAPO_CODICE As String = Matricola
                Dim P_CODICE_SPECIE As String = Codice_Specie
                Dim P_DT_USCITA As Date = dataUscita

                Dim dtDeleteUscita = wsChiamawsRegistroCapiStalla.Delete_Uscita(P_USCITA_ID, P_AZIENDA_CODICE,
                                                                                P_ALLEV_ID_FISCALE, P_CAPO_CODICE,
                                                                                P_CODICE_SPECIE, P_MOTIVO, P_DT_USCITA)

                If Not IsNothing(dtDeleteUscita) AndAlso dtDeleteUscita.Tables.Count > 0 Then
                    Dim mds As AgronicaCoreEntityFramework_POCO.Movimenti_dettagli = (From md In GiasContext.Movimenti_dettagli
                                                                                      Where md.PIVA = Piva AndAlso md.Id_Agenda = Id_Agenda AndAlso
                                                                                          md.Id_Mov = Id_Mov AndAlso md.Id_Mov_Det = Id_Mov_Det
                                                                                      Select md).FirstOrDefault
                    If IsNothing(mds) Then
                        Throw New Exception("L'operazione (Id_Mov_Det=" & Id_Mov_Det & ") non contiene capi da scaricare")
                    End If

                    mds.Id_Mov_Esterno = 0
                    GiasContext.Entry(mds).State = EntityState.Modified

                    'Dim capo As AgronicaCoreEntityFramework_POCO.Zoo_Animali = (From z In GiasContext.Zoo_Animali
                    '															Where z.Matricola = Matricola
                    '															Select z).First
                    'TODO?
                    'capo.Modello4_Uscita_Numero = ""
                    'capo.Modello4_Uscita = ""
                    'capo.Modello4_Uscita_Prenotazione = ""
                    'capo.Codice_Azienda_Uscita = ""
                    'capo.Data_Documento_Uscita = AGRODATAFINE
                    'GiasContext.Entry(capo).State = EntityState.Modified

                    GiasContext.SaveChanges()

                    resp.cancellato = "SI"
                    resp.errore = ""
                Else
                    resp.cancellato = "NO"
                    resp.errore = "Errore nella cancellazione del capo"
                End If

            Catch ex As BDNException
                'Throw New BDNException("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
                resp.cancellato = "NO"
                resp.errore = "Errore nella cancellazione del capo"
            Catch ex As Exception
                Throw New Exception("Errore: " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ->" & ex.Message)
            End Try

        Else
            resp.cancellato = "NO"
            resp.errore = "Capo non presente in BDN"
        End If

        Return resp

    End Function

    ''' <summary>
    ''' Creazione log per movimentazioni BDN
    ''' </summary>
    ''' <param name="piva"></param>
    ''' <param name="saCod"></param>
    ''' <param name="idAgenda"></param>
    ''' <param name="lavCod"></param>
    ''' <param name="causaleBDN"></param>
    ''' <param name="idMovimentoBDN"></param>
    ''' <param name="idMov"></param>
    ''' <param name="idMovDet"></param>
    ''' <param name="codAnimale"></param>
    ''' <param name="idCapoBDN"></param>
    ''' <param name="dtDatiInviati"></param>
    ''' <param name="dtDatiRicevuti"></param>
    ''' <param name="esito"></param>
    ''' <param name="GiasContext"></param>
    ''' <returns></returns>
    Private Function logMovimentazioniBDN(ByVal piva As String,
                                          ByVal saCod As Integer,
                                          ByVal idAgenda As Integer,
                                          ByVal lavCod As Integer,
                                          ByVal causaleBDN As enumCausaliBDN,
                                          ByVal idMovimentoBDN As Integer,
                                          ByVal idMov As Integer,
                                          ByVal idMovDet As Integer,
                                          ByVal codAnimale As Integer,
                                          ByVal idCapoBDN As Integer,
                                          ByVal dtDatiInviati As DataTable,
                                          ByVal dtDatiRicevuti As DataTable,
                                          ByVal esito As Boolean,
                                          ByRef GiasContext As Gias_DeveloperServer_Entities) As Boolean
        Dim objLogInvioChiamate As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_W
        Dim objLogInvioAgenda As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Agenda_W
        Dim objLogInvioAnagrafe As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Anagrafe_W

        Dim chiaveCapo As String = piva & "_" & saCod & "_" & codAnimale
        Dim idMovDest As Integer = 0
        Dim msgEsito As String = IIf(esito, "Eseguita", "Errore")

        Dim stream As New MemoryStream
        Dim streamReader As New StreamReader(stream)
        Dim datiInviati As String = ""
        Dim datiRicevuti As String = ""

        Try
            If Not IsNothing(dtDatiInviati) AndAlso dtDatiInviati.Rows.Count > 0 Then
                datiInviati = JsonConvert.SerializeObject(dtDatiInviati)
            End If

            If Not IsNothing(dtDatiInviati) AndAlso dtDatiInviati.Rows.Count > 0 Then
                datiRicevuti = JsonConvert.SerializeObject(dtDatiInviati)
            End If

            'AGRONICA LOG INVIO CHIAMATE
            Dim LogInvioChiamata = objLogInvioChiamate.Create_Agronica_Log_Invio_Chiamate(enum_Esportazioni_Sistema_Cod.BDN,
                                                                                          datiInviati, DateTime.Now, msgEsito,
                                                                                          datiRicevuti, 0, lavCod,
                                                                                          objParametriServer, GiasContext)

            'AGRONICA LOG INVIO AGENDA
            Dim LogInvioAgenda = objLogInvioAgenda.Create_Agronica_Log_Invio_Agenda(enum_Esportazioni_Sistema_Cod.BDN,
                                                                                    idAgenda, idMovimentoBDN,
                                                                                    LogInvioChiamata.ID, objParametriServer,
                                                                                    GiasContext, idMov, idMovDet,
                                                                                    idMovDest, causaleBDN)

            'AGRONICA LOG INVIO ANAGRAFE
            Dim LogInvioAnagrafe As Boolean = objLogInvioAnagrafe.Scrivi(enum_Esportazioni_Sistema_Cod.BDN, LogInvioChiamata.ID,
                                                                         esito, "BDN", chiaveCapo, piva, saCod, 0, 0,
                                                                         codAnimale, "", DateTime.Now, objParametriServer,
                                                                         idCapoBDN, , , causaleBDN)

            'per ora con eccezione ritorna semplicemente falso
        Catch ex As Exception
            Return False
        End Try

        Return True

    End Function

    ''' <summary>
    ''' crea i pdf dei modelli di accompagnamento selezionati (nella pagina di invia uscite)
    ''' </summary>
    ''' <param name="chiave"></param>
    ''' <returns></returns>
    Public Function downloadModelli4Selezionati(ByVal chiave As List(Of ChiaveCaricoScaricoCapi),
                                                Optional ByRef nomeFile As String = "") As List(Of String)
        Dim pathsPdf As New List(Of String)

        'ricavo Piva e Sa_Cod della Stalla dal primo elemento
        Dim Piva As String = chiave(0).Piva
        Dim Sa_Cod As Integer = chiave(0).Sa_Cod

        Dim objZooAnimale As New AgronicaCoreAnagrafeDAL.Zoo_Animali

        Dim prenotazioneId_Scaricati As New List(Of String)

        Dim unicoFile As Boolean = chiave.Count = 1

        For Each animale In chiave
            Dim dtAnimaleDB As DataTable = objZooAnimale.BDN_Leggi_Movimenti_Scarico_Non_Sincronizzati(Piva, 0, 0,
                                                                                                       0, animale.Cod_Progetto,
                                                                                                       AGRODATAINIZIO, AGRODATAFINE,
                                                                                                       False, Nothing,
                                                                                                       objParametriServer)
            Dim capoAnimale As DataRow = dtAnimaleDB.Rows(0)

            Try
                Dim idAgenda As Integer = animale.ID_Agenda
                'For Each idAgenda In Operazioni_Agenda
                Dim Agenda As AgronicaCoreEntityFramework_POCO.Agenda = (From a In GiasContext.Agenda
                                                                         Where a.PIVA = Piva AndAlso a.Id_Agenda = idAgenda).FirstOrDefault
                Dim Lav_Cod As Integer = Agenda.Lav_Cod

                If IsNothing(Agenda) Then
                    Throw New Exception("Operazione zootecnica (Id_Agenda=" & idAgenda & ") non valida o inesistente")
                End If

                Dim Movimenti_Scarico As AgronicaCoreEntityFramework_POCO.Movimenti = (From m In GiasContext.Movimenti
                                                                                       Where m.PIVA = Piva AndAlso m.Id_Agenda = idAgenda AndAlso
                                                                                               m.Cau_Mov = CAU_SCARICO_CONSISTENZE).FirstOrDefault
                If IsNothing(Movimenti_Scarico) Then
                    Throw New Exception("Operazione zootecnica (Id_Agenda=" & idAgenda & ") non valida o inesistente")
                End If
                Dim idMov_Scarico As Integer = Movimenti_Scarico.Id_Mov

                Dim MovDest_Scarico As List(Of AgronicaCoreEntityFramework_POCO.Mov_Destinazioni) = (From md In GiasContext.Mov_Destinazioni
                                                                                                     Where md.Piva = Piva AndAlso md.Id_Agenda = idAgenda AndAlso
                                                                                                           md.Id_Mov = idMov_Scarico).ToList
                If IsNothing(MovDest_Scarico) OrElse MovDest_Scarico.Count = 0 Then
                    Throw New Exception("L'operazione (Id_Agenda=" & idAgenda & ") non contiente capi da scaricare")
                End If

                'CODICE SPECIE
                Dim objStalla As New AgronicaCoreAnagrafeDAL.Stalla_R
                Dim raggruppamento_cod = MovDest_Scarico(0).Id_Destinazione
                Dim raggruppamento = (From rr In GiasContext.Stalla_Raggruppamenti Where rr.Raggruppamento_Cod = raggruppamento_cod).FirstOrDefault
                Dim Sta_Num As Integer = raggruppamento.STA_NUM
                Dim dtStalla As DataTable = objStalla.Leggi(Piva, Sa_Cod, Sta_Num,
                                                            enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                            "", "", objParametriServer)

                Dim objCodificaSpecie As New AgronicaCoreMetaSchemaDAL.Codifica_BDN_SpecieAnimali
                Dim dtCodifica As DataTable = objCodificaSpecie.leggi(objParametriServer, "", "",
                                                                          dtStalla(0)("GEN_COD"), dtStalla(0)("SPE_COD"),
                                                                          "3")
                If IsNothing(dtCodifica) OrElse dtCodifica.Rows.Count = 0 Then
                    Throw New GiasException("Codifica specie mancante per BDN")
                End If
                Dim codiceSpecie As String = "0" & dtCodifica.Rows(0)("CODICE")

                'Movimenti Registrazione (se presenti)
                Dim prenotazioneId As String = ""
                Dim Movimenti_Registrazione As AgronicaCoreEntityFramework_POCO.Movimenti = (From m In GiasContext.Movimenti
                                                                                             Where m.PIVA = Piva AndAlso m.Id_Agenda = idAgenda AndAlso
                                                                                                     m.Cau_Mov = CAU_REGISTRAZIONI).FirstOrDefault
                If IsNothing(Movimenti_Registrazione) Then
                    'Throw New Exception("Operazione zootecnica (Id_Agenda=" & idAgenda & ") non valida o inesistente")
                    Dim capoAnimaleDB = (From z In GiasContext.Zoo_Animali Where z.Cod_Progetto = animale.Cod_Progetto).FirstOrDefault
                    If capoAnimaleDB IsNot Nothing Then
                        prenotazioneId = capoAnimaleDB.Modello4_Uscita_Prenotazione
                    End If
                Else
                    Dim idMov_Registrazione As Integer = Movimenti_Registrazione.Id_Mov

                    Dim MovDettaglio_TecnEx As AgronicaCoreEntityFramework_POCO.Mov_Dettaglio_Tecnico_Extra = (From a In GiasContext.Mov_Dettaglio_Tecnico_Extra
                                                                                                               Where a.Piva = Piva AndAlso a.Id_Agenda = idAgenda AndAlso
                                                                                                                          a.Id_Mov = idMov_Registrazione).FirstOrDefault
                    If IsNothing(MovDettaglio_TecnEx) Then
                        Throw New Exception("Dati di trasporto per l'operazione (Id_Agenda=" & idAgenda & ") non presenti")
                    End If

                    prenotazioneId = MovDettaglio_TecnEx.Num_Riferimento
                End If


                'esegue il download dei modelli
                Dim pathPdf As String = ""
                If prenotazioneId <> "" AndAlso Not prenotazioneId_Scaricati.Contains(prenotazioneId) Then
                    Dim dtDocAllegato As DataTable = wsInterrogazioniModello4.creaModelloSingoliConDatiCorrenti(prenotazioneId,
                                                                                                                codiceSpecie)
                    pathPdf = scaricaPdfModelli(dtDocAllegato(0)("P_FILE_64"), nomeFile)
                    prenotazioneId_Scaricati.Add(prenotazioneId)
                    pathsPdf.Add(pathPdf)
                End If

            Catch ex As TokenBDNException
                Throw ex
            Catch ex As BDNException
                If ex.Message.Contains("DL01") Then
                    Throw New BDNException("Errore nel tentativo di download del modello selezionato")
                Else
                    Throw ex
                End If
            Catch ex As Exception
                Throw New Exception(ex.Message)
            End Try

        Next

        Return pathsPdf

    End Function

    ''' <summary>
    ''' gestisci i pdf scaricati
    ''' </summary>
    ''' <param name="pathsPdf">lista path dei pdf scaricato</param>
    ''' <returns>path del file che va in download all'utente</returns>
    Public Function gestioneDownloadAllegato(ByVal pathsPdf As List(Of String),
                                             Optional ByRef File As Byte() = Nothing,
                                             Optional ByRef NomeFile As String = "",
                                             Optional ByRef Estensione As String = "") As String
        Dim pathResp As String = ""

        pathsPdf = pathsPdf.Distinct.ToList

        Dim pathTemp As String = FileSystemHelper.AggiungiSlashSeNonEsiste(AgroWebConfig.PathFileTemporanei)

        If pathsPdf.Count = 1 Then
            pathResp = pathsPdf.First
            Estensione = ".pdf"
        ElseIf pathsPdf.Count > 1 Then
            NomeFile = "docModelli4" & "_" & Format(DateTime.Now, "yyyy-MM-dd").Replace(" ", "") & "_" & Format(DateTime.Now, "HHmm ssffff").Replace(" ", "") & ".zip"

            Dim pathFileZip As String = pathTemp & NomeFile
            AgroZip.AggiungiPiuFileAZip(pathFileZip, pathsPdf)
            pathResp = pathFileZip

            Estensione = ".zip"

            FileSystemHelper.EliminaFiles(pathsPdf)
        End If

        If pathResp <> "" Then
            File = My.Computer.FileSystem.ReadAllBytes(pathResp)
        End If

        Return pathResp

    End Function

    ''' <summary>
    ''' scarica il pdf del modello4
    ''' </summary>
    ''' <param name="base64_str">stringa in base64 ottenuta dalla chiamata alla BDN</param>
    ''' <returns>path del pdf scaricato</returns>
    Private Function scaricaPdfModelli(ByVal base64_str As String, Optional ByRef nomeFile As String = "") As String

        Dim pathTemp As String = FileSystemHelper.AggiungiSlashSeNonEsiste(AgroWebConfig.PathFileTemporanei)

        nomeFile = "pdfModello4" & "_" & Format(DateTime.Now, "yyyy-MM-dd").Replace(" ", "") & "_" & Format(DateTime.Now, "HHmm ssffff").Replace(" ", "") & ".pdf"
        Dim pathPdf As String = pathTemp & nomeFile
        If Not Directory.Exists(pathTemp) Then
            Directory.CreateDirectory(pathTemp)
        End If

        base64_str = base64_str.Replace("\n", "").Replace(vbCrLf, "")

        File.WriteAllBytes(pathPdf, Convert.FromBase64String(base64_str))

        Return pathPdf

    End Function


    ''' <summary>
    ''' Utility class
    ''' </summary>
    Private Class objFlagMacello
        Public macello1 As String = ""
        Public macello2 As String = ""
        Public macello2A As String = ""
        Public macello2B As String = ""
        Public macello2C As String = ""
        Public macello3 As String = ""
        Public macello3_Enterici As String = ""
        Public macello3_Respiratori As String = ""
        Public macello3_Cutanei As String = ""
        Public macello3_Locomotori As String = ""
        Public macello3_Altro As String = ""
        Public macello3_AltroDesc As String = ""
        Public macello4 As String = ""
        Public macello5 As String = ""
        Public macello5_Elementi As String = ""
        Public macello5_Rilevazioni As String = ""
        Public macello5_Altro As String = ""
        Public macello5_AltroDesc As String = ""
        Public macello6 As String = ""

        Public Sub New()
            macello1 = ""
            macello2 = ""
            macello2A = ""
            macello2B = ""
            macello2C = ""
            macello3 = ""
            macello3_Enterici = ""
            macello3_Respiratori = ""
            macello3_Cutanei = ""
            macello3_Locomotori = ""
            macello3_Altro = ""
            macello3_AltroDesc = ""
            macello4 = ""
            macello5 = ""
            macello5_Elementi = ""
            macello5_Rilevazioni = ""
            macello5_Altro = ""
            macello5_AltroDesc = ""
            macello6 = ""
        End Sub

        Public Sub New(ByVal jObjSTR As String)
            Dim jObj = JsonConvert.DeserializeObject(Of JObject)(jObjSTR)

            macello1 = If(jObj.Item("macello1"), "S", "")
            macello2 = If(jObj.Item("macello2"), "O", "N")
            macello2A = If(jObj.Item("macello2A"), "S", "N")
            macello2B = If(jObj.Item("macello2B"), "S", "N")
            macello2C = If(jObj.Item("macello2C"), "S", "N")
            macello3 = If(jObj.Item("macello3"), "S", "N")
            If macello3 = "S" Then
                macello3_Enterici = If(jObj.Item("macello3E"), "S", "")
                macello3_Respiratori = If(jObj.Item("macello3R"), "S", "")
                macello3_Cutanei = If(jObj.Item("macello3C"), "S", "")
                macello3_Locomotori = If(jObj.Item("macello3L"), "S", "")
                macello3_Altro = If(jObj.Item("macello3A"), "S", "")
                macello3_AltroDesc = jObj.Item("macello3AD")
            End If
            macello4 = If(jObj.Item("macello4"), "S", "N")
            macello5 = If(jObj.Item("macello5"), "S", "N")
            If macello5 = "S" Then
                macello5_Elementi = If(jObj.Item("macelloE"), "S", "")
                macello5_Rilevazioni = If(jObj.Item("macelloR"), "S", "")
                macello5_Altro = If(jObj.Item("macelloA"), "S", "")
                macello5_AltroDesc = jObj.Item("macelloAD")
            End If
            macello6 = If(jObj.Item("macello6"), "S", "N")
        End Sub

    End Class

    Public Class OggettoInviaIngressi
        Public Destinazione As String
        Public Capi As ChiaveCaricoScaricoCapi()
    End Class

    Public Class OggettoInviaUscite
        Public Piva As String
        Public Destinazione As String
        Public RegistraOp As Boolean
        Public Operazioni As List(Of Integer)
    End Class

    Public Class ChiaveCaricoScaricoCapi
        Public Piva As String
        Public Sa_Cod As Integer
        Public ID_Agenda As Integer
        Public ID_Mov As Integer
        Public ID_Mov_Det As Integer
        Public Cod_Progetto As Integer
    End Class

    Public Class InserisciModello4Response
        Public NumModello4 As String
        Public CodiceModello4 As String
        Public pathPdf As String
        Public listaAnimali As List(Of String)
        Public importato As String
        Public registrato As String
        Public errore As String

        Public File As String
    End Class

    Public Class RegistraModello4Response
        Public NumModello4 As String
        Public CodiceModello4 As String
        Public listaAnimali As List(Of String)
        Public importato As String
        Public errore As String
    End Class

    Public Class InviaUscitaResponse
        Public Matricola As String
        Public pathPdf As String
        Public importato As String
        Public errore As String
    End Class

    Public Class InviaIngressiResponse
        Public Matricola As String
        Public pathPdf As String
        Public importato As String
        Public errore As String
    End Class

    Public Class CancellaOperazioneResponse
        Public matricola As String
        Public cancellato As String
        Public errore As String
    End Class

End Class
