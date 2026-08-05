Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports System.Transactions
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Data.Entity
Imports Newtonsoft.Json
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreModelsSTD.costanti
Imports AgronicaCoreModelsSTD.exceptions

Public Class Budget_EFAppezzamento
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Shared Function CreateIndirizzoAppezzamento(ByVal dati_indirizzo As AgronicaCoreModelsSTD.anagrafiche.Indirizzo,
                                                      ByRef objParametri As AgronicaCoreParametri,
                                                      ByRef appezzamento As Budget_Appezzamento,
                                                      ByRef tipoIndirizzo As Integer,
                                                      ByRef username As String,
                                                      Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                      Optional ByVal NewTransaction As Boolean = True) As Indirizzi

        Dim nomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_EFAppezzamento.CreateIndirizzoAppezzamento()"
        Dim messaggioErrore As String = ""
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        'controllo dati istat comune\provincia\cap
        Dim istatList = From istat In GiasContext.ISTAT
                        Where istat.COM = dati_indirizzo.istatComune.com AndAlso
                            istat.PROV = dati_indirizzo.istatComune.prov

        Dim ista = istatList.FirstOrDefault()
        If (ista Is Nothing) Then
            Throw New Exception("Provincia e/o Comune non trovati in tabella Istat. Operazione annullata")
        End If


        Dim indirizzo As Indirizzi = Nothing

        Try
            indirizzo = EFIndirizzi.CreateIndirizziEF(GiasContext, objParametri, username)
            indirizzo.ind_des = dati_indirizzo.via
            indirizzo.frz_des = dati_indirizzo.frazione
            indirizzo.CAP = dati_indirizzo.cap
            If dati_indirizzo.istatComune.localita IsNot Nothing AndAlso dati_indirizzo.istatComune.localita <> "" Then
                indirizzo.com_des = dati_indirizzo.istatComune.localita
            End If

            If dati_indirizzo.istatComune.comuni_prov IsNot Nothing AndAlso dati_indirizzo.istatComune.comuni_prov <> "" Then
                'indirizzo.pro_cod = dati_indirizzo.istatComune.comuni_prov
            End If
            indirizzo.stato = dati_indirizzo.stato.codice
            indirizzo.note = If(dati_indirizzo.note, "")
            indirizzo.pro_cod_istat = dati_indirizzo.istatComune.prov
            indirizzo.com_cod_istat = dati_indirizzo.istatComune.com
            GiasContext.SaveChanges()

            Dim axi = CreateAppezzamentixIndirizzi(GiasContext, appezzamento.Id_Budget, appezzamento.PIVA, appezzamento.SA_COD, appezzamento.APPEZZA, indirizzo.cod_indirizzo, tipoIndirizzo, username)

            GiasContext.Budget_AppezzamentixIndirizzi.Add(axi)

            GiasContext.SaveChanges()

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As GiasException
            indirizzo = Nothing
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            indirizzo = Nothing
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return indirizzo
    End Function

    Public Shared Function ModificaIndirizzoAppezzamento(ByVal dati_indirizzo As AgronicaCoreModelsSTD.anagrafiche.Indirizzo,
                                                      ByRef objParametri As AgronicaCoreParametri,
                                                      ByVal appezzamento As Budget_Appezzamento,
                                                      ByRef tipoIndirizzo As Integer,
                                                      ByRef username As String,
                                                      Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                      Optional ByVal NewTransaction As Boolean = True) As Indirizzi

        Dim nomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_EFAppezzamento.ModificaIndirizzoAppezzamento()"
        Dim messaggioErrore As String = ""
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        'controllo dati istat comune\provincia\cap
        Dim istatList = From istat In GiasContext.ISTAT
                        Where istat.COM = dati_indirizzo.istatComune.com AndAlso
                            istat.PROV = dati_indirizzo.istatComune.prov

        Dim ista = istatList.FirstOrDefault()
        If (ista Is Nothing) Then
            Throw New Exception("Provincia e/o Comune non trovati in tabella Istat. Operazione annullata")
        End If

        Dim indirizzoMod As Indirizzi = Nothing

        Try
            indirizzoMod = EFIndirizzi.Indirizzo_Modifica_EF(dati_indirizzo, objParametri, username, GiasContext, NewTransaction)
            GiasContext.SaveChanges()

            Dim axiList = From axi In GiasContext.Budget_AppezzamentixIndirizzi
                          Where axi.Id_Budget = appezzamento.Id_Budget AndAlso
                              axi.PIVA = appezzamento.PIVA AndAlso
                              axi.sa_cod = appezzamento.SA_COD AndAlso
                              axi.appezza = appezzamento.APPEZZA AndAlso
                              axi.cod_indirizzo = indirizzoMod.cod_indirizzo
                          Select axi
            Dim appxind = axiList.FirstOrDefault()
            If (appxind Is Nothing) Then

                Dim axi = CreateAppezzamentixIndirizzi(GiasContext, appezzamento.Id_Budget, appezzamento.PIVA, appezzamento.SA_COD, appezzamento.APPEZZA, indirizzoMod.cod_indirizzo, tipoIndirizzo, username)

                GiasContext.Budget_AppezzamentixIndirizzi.Add(axi)
            Else
                Dim axi = ModificaAppezzamentixIndirizzi(appezzamento.Id_Budget, appezzamento.PIVA, appezzamento.SA_COD, appezzamento.APPEZZA, indirizzoMod.cod_indirizzo, tipoIndirizzo, username, GiasContext)
                GiasContext.Budget_AppezzamentixIndirizzi.Attach(axi)
                GiasContext.Entry(axi).State = EntityState.Modified
            End If

            GiasContext.SaveChanges()

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As GiasException
            indirizzoMod = Nothing
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            indirizzoMod = Nothing
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return indirizzoMod
    End Function

    Public Shared Sub ScriviModificaEliminaAppezzamentoCodici(ByVal Id_Budget As Integer,
                                                    ByVal piva As String,
                                                    ByVal saCod As Integer,
                                                    ByVal appezza As Integer,
                                                    ByVal idCod As Integer,
                                                    ByVal valCod As String,
                                                    ByVal delete As Boolean,
                                                    ByVal username As String,
                                                    ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                    Optional ByVal NewTransaction As Boolean = True,
                                                    Optional ByVal validitaInizio As Date = AGRODATAINIZIO,
                                                    Optional ByRef validitaFine As Date = AGRODATAFINE
                                                    )

        Dim nomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_EFAppezzamento.ScriviModificaEliminaAppezzamentoCodici()"
        Dim messaggioErrore As String = ""
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Try

            If idCod <> 0 Then

                Dim app_codl = (From ic In GiasContext.Budget_Appezzamento_Codici
                                Where ic.Id_Budget = Id_Budget AndAlso
                                     ic.PIVA = piva AndAlso
                                     ic.sa_cod = saCod AndAlso
                                     ic.appezza = appezza AndAlso
                                     ic.id_cod = idCod
                                Select ic).ToList()

                Dim operazione As enum_TipoOperazioneDB

                If app_codl.Count > 0 AndAlso (valCod <> "0" AndAlso valCod <> "") Then
                    operazione = enum_TipoOperazioneDB.Modifica
                ElseIf app_codl.Count > 0 AndAlso (valCod = "0" OrElse valCod = "") Then
                    operazione = enum_TipoOperazioneDB.Cancellazione
                ElseIf app_codl.Count = 0 AndAlso (valCod = "" OrElse valCod = "0") Then
                    operazione = enum_TipoOperazioneDB.Lettura
                ElseIf app_codl.Count = 0 AndAlso valCod <> "" Then
                    operazione = enum_TipoOperazioneDB.Scrittura
                End If

                Select Case operazione
                    Case enum_TipoOperazioneDB.Scrittura

                        Dim app_cod As New AgronicaCoreEntityFramework_POCO.Budget_Appezzamento_Codici With {
                            .Id_Budget = Id_Budget,
                            .PIVA = piva,
                            .sa_cod = saCod,
                            .appezza = appezza,
                            .id_cod = idCod,
                            .val_cod = valCod,
                            .inviato = 0,
                            .Data_Creazione = DateTime.Now,
                            .Data_Modifica = DateTime.Now,
                            .Validita_Inizio = validitaInizio,
                            .Validita_Fine = validitaFine,
                            .Username_Creazione = username,
                            .Username_Modifica = username
                        }

                        GiasContext.Budget_Appezzamento_Codici.Add(app_cod)

                    Case enum_TipoOperazioneDB.Modifica

                        Dim app_cod = app_codl.FirstOrDefault

                        If app_cod IsNot Nothing Then
                            app_cod.val_cod = valCod
                            app_cod.Data_Modifica = DateTime.Now
                            app_cod.Username_Modifica = username
                            app_cod.Validita_Inizio = validitaInizio
                            app_cod.Validita_Fine = validitaFine

                            GiasContext.Budget_Appezzamento_Codici.Attach(app_cod)
                            GiasContext.Entry(app_cod).State = EntityState.Modified
                        End If

                    Case enum_TipoOperazioneDB.Cancellazione

                        Dim app_cod = app_codl.FirstOrDefault
                        GiasContext.Budget_Appezzamento_Codici.Attach(app_cod)
                        GiasContext.Budget_Appezzamento_Codici.Remove(app_cod)

                End Select

                GiasContext.SaveChanges()

            End If
            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

    End Sub

    Public Shared Function Appezzamento_Scrivi_EF(ByVal Id_Budget As Integer,
                                                  ByVal DatiAppezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                                  ByRef objParametriServer As AgronicaCoreParametri,
                                                  ByRef objParametriUtenti As AgronicaCoreParametri,
                                                  ByVal username As String,
                                                  Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                  Optional ByVal NewTransaction As Boolean = True
                                                  ) As Budget_Appezzamento

        Dim nomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_EFAppezzamento.Appezzamento_Scrivi_EF"
        Dim messaggioErrore As String = ""
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        If EFImprese.ImpresaExist(GiasContext, DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva) = False Then
            Throw New GiasException("Partita Iva (" & DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva & ") non anagrafica imprese. Operazione annullata")
        End If


        If EFCentri_Aziendali.CentroExist(GiasContext,
                                          DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
                                          DatiAppezzamento.primaryKey.centroAziendalePK.codice) = False Then
            Throw New GiasException("Centro aziendale (" & DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva & "/" &
                                                       DatiAppezzamento.primaryKey.centroAziendalePK.codice.ToString() & ") non anagrafica. Operazione annullata")
        End If

        Dim appezzamento = Create_Appezzamento(Id_Budget,
                                               DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
                                               DatiAppezzamento.primaryKey.centroAziendalePK.codice,
                                               username,
                                               objParametriServer,
                                               objParametriUtenti
                                               )

        'Dim utentixappezzamenti = Create_UtentiXAppezzamenti(DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
        '                                                     DatiAppezzamento.primaryKey.centroAziendalePK.codice,
        '                                                     appezzamento.APPEZZA,
        '                                                     username,
        '                                                     objParametriServer
        '                                                    )

        Try
            appezzamento.SUP_APP = DatiAppezzamento.superficie
            appezzamento.ESPOSIZ = If(DatiAppezzamento.esposizione IsNot Nothing, DatiAppezzamento.esposizione.codice, "")
            appezzamento.PENDE = DatiAppezzamento.pendenza
            appezzamento.UBICAZIONE = If(DatiAppezzamento.ubicazione IsNot Nothing, DatiAppezzamento.ubicazione.codice, "")
            appezzamento.USER = objParametriServer.PivaSuperUser
            If (DatiAppezzamento.descrizione Is Nothing) OrElse DatiAppezzamento.descrizione = "" Then

                Dim seqApp = From seq In GiasContext.SeqAppezzamento
                             Where seq.Piva = DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva AndAlso
                                 seq.Sa_Cod = DatiAppezzamento.primaryKey.centroAziendalePK.codice
                             Select seq

                Dim basecod As Long = 0
                If seqApp.FirstOrDefault() IsNot Nothing Then
                    basecod = seqApp.FirstOrDefault().Base
                End If

                Dim app_nome = ""

                Try

                    Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                    Dim ImpostazioneValore1 As String
                    ImpostazioneValore1 = objImpost.ImpostazioneValore1_from_ImpostazioneCod(
                                                            enum_Impostazioni_Utenti.UTENTE_NumAppezza_Progr_Modalita,
                                                            objParametriUtenti,
                                                            1)

                    Select Case ImpostazioneValore1
                        Case "1"
                            Dim objAppezza As New AgronicaCoreBudgetDAL.Budget_Appezzamento_R
                            Dim Numero_Appezzamenti As Integer
                            Numero_Appezzamenti = objAppezza.Numero_Appezzamenti(Id_Budget, DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva, 0, "", objParametriServer)
                            Numero_Appezzamenti += 1
                            app_nome = "App. " & Format(Numero_Appezzamenti, "000")
                        Case Else
                            Dim veg_cod = 0
                            Dim descrizione_specie = ""
                            If (DatiAppezzamento.impianti.Count > 0) Then
                                If DatiAppezzamento.impianti(0).utilizzoTerreno.classType = ClassType.Varieta Then
                                    veg_cod = CType(DatiAppezzamento.impianti(0).utilizzoTerreno, AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta).specie.codice
                                    descrizione_specie = CType(DatiAppezzamento.impianti(0).utilizzoTerreno, AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta).specie.descrizione
                                End If
                            End If
                            If (ImpostazioneValore1 = "2" OrElse ImpostazioneValore1 = "3") AndAlso veg_cod <> 0 Then


                                Dim objAppezza As New AgronicaCoreBudgetDAL.Budget_Appezzamento_R
                                Dim Numero_Appezzamenti As Integer

                                Dim sacod As Integer = 0
                                If ImpostazioneValore1 = "2" Then
                                    sacod = 0
                                Else
                                    sacod = DatiAppezzamento.primaryKey.centroAziendalePK.codice
                                End If

                                Numero_Appezzamenti = objAppezza.Numero_Appezzamenti_X_Specie(Id_Budget, DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva, sacod, veg_cod, "", objParametriServer)
                                Numero_Appezzamenti += 1
                                If descrizione_specie = "" Then
                                    descrizione_specie = (From s In GiasContext.SpecieVegetali Where s.Veg_Cod = veg_cod Select s.Veg_Des).FirstOrDefault()
                                End If
                                app_nome = descrizione_specie & " " & Format(Numero_Appezzamenti, "000")

                            Else

                                app_nome = "App. " & ((appezzamento.APPEZZA - basecod) Mod 1000).ToString()

                            End If

                    End Select

                Catch ex As Exception
                    app_nome = "App. " & ((appezzamento.APPEZZA - basecod) Mod 1000).ToString()
                End Try

                appezzamento.APP_NOME = app_nome
            Else
                appezzamento.APP_NOME = DatiAppezzamento.descrizione
            End If
            appezzamento.Campo_Cod = If(DatiAppezzamento.campoPK IsNot Nothing, DatiAppezzamento.campoPK.codice, 0)
            appezzamento.DistBZ_CorpiIdrici = DatiAppezzamento.distBZ_CorpiIdrici
            appezzamento.DistBZ_AreeResPub = DatiAppezzamento.distBZ_AreeResPub
            appezzamento.DistBZ_Allevamenti = DatiAppezzamento.distBZ_Allevamenti
            appezzamento.DistBZ_VegNatNonColt = DatiAppezzamento.distBZ_VegNatNonColt
            appezzamento.SupBZ_Riduzione = DatiAppezzamento.supBZ_Riduzione

            If DatiAppezzamento.validita.inizio >= AGRODATAINIZIO AndAlso DatiAppezzamento.validita.inizio <= AGRODATAFINE Then
                appezzamento.Validita_Inizio = DatiAppezzamento.validita.inizio
            End If
            If DatiAppezzamento.validita.fine >= AGRODATAINIZIO AndAlso DatiAppezzamento.validita.fine <= AGRODATAFINE Then
                appezzamento.Validita_Fine = DatiAppezzamento.validita.fine
            End If

            appezzamento.X = DatiAppezzamento.lat
            appezzamento.Y = DatiAppezzamento.lng
            appezzamento.ZSLM = DatiAppezzamento.altitudine

            'lavez - 21/03/2024 - chiavi nuovo tracciato agea
            If DatiAppezzamento.Agea_idSchedaValidazione IsNot Nothing Then
                appezzamento.Agea_idSchedaValidazione = DatiAppezzamento.Agea_idSchedaValidazione
            End If
            If DatiAppezzamento.Agea_codiBarrScheVali IsNot Nothing Then
                appezzamento.Agea_codiBarrScheVali = DatiAppezzamento.Agea_codiBarrScheVali
            End If
            If DatiAppezzamento.Agea_identificativoAppezzamento IsNot Nothing Then
                appezzamento.Agea_identificativoAppezzamento = DatiAppezzamento.Agea_identificativoAppezzamento
            End If
            If DatiAppezzamento.Agea_identificativoIsola IsNot Nothing Then
                appezzamento.Agea_identificativoIsola = DatiAppezzamento.Agea_identificativoIsola
            End If
            If DatiAppezzamento.Agea_identificativoPianoColtivazione IsNot Nothing Then
                appezzamento.Agea_identificativoPianoColtivazione = DatiAppezzamento.Agea_identificativoPianoColtivazione
            End If
            If DatiAppezzamento.Agea_idAppezzamentoOrig IsNot Nothing Then
                appezzamento.Agea_idAppezzamentoOrig = DatiAppezzamento.Agea_idAppezzamentoOrig
            End If

            appezzamento.SABBIA = DatiAppezzamento.sabbia
            appezzamento.LIMO = DatiAppezzamento.limo
            appezzamento.ARGILLA = DatiAppezzamento.argilla
            appezzamento.CLAS = CStr(If(DatiAppezzamento.classeTessitura IsNot Nothing, DatiAppezzamento.classeTessitura.codice, 0))

            GiasContext.Budget_Appezzamento.Add(appezzamento)
            'GiasContext.UtentiXAppezzamenti.Add(utentixappezzamenti)
            GiasContext.SaveChanges()

            Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            Dim DatiAppezzamentoStr = JsonConvert.SerializeObject(DatiAppezzamento, a)

            'Scrittura tabella Agronica_Log_Anagrafe
            Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
            Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Budget_Appezza,
                                                                                    CStr(Id_Budget),
                                                                                    CStr(appezzamento.PIVA), CStr(appezzamento.SA_COD),
                                                                                    CStr(appezzamento.APPEZZA), Nothing,
                                                                                    Nothing,
                                                                                    enum_TipoOperazioneDB.Scrittura,
                                                                                    objParametriServer,
                                                                                    enum_Id_Servizio.GiasOnline,
                                                                                    "",
                                                                                    DatiAppezzamentoStr, Id_Budget:=appezzamento.Id_Budget)

            GiasContext.Agronica_Log_Anagrafe.Add(log)
            GiasContext.SaveChanges()

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As Exception
            appezzamento = Nothing
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return appezzamento
    End Function

    Public Shared Function Appezzamento_Modifica_EF(ByVal Id_Budget As Integer,
                                                    ByVal DatiAppezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                                    ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    ByVal username As String,
                                                    Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                    Optional ByVal NewTransaction As Boolean = True
                                                    ) As Budget_Appezzamento

        Dim nomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_EFAppezzamento.Appezzamento_Modifica_EF"
        Dim messaggioErrore As String = ""
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If

        Dim appezzamenti = From appezzamento In GiasContext.Budget_Appezzamento
                           Where appezzamento.Id_Budget = Id_Budget AndAlso
                                 appezzamento.PIVA = DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva AndAlso
                                 appezzamento.SA_COD = DatiAppezzamento.primaryKey.centroAziendalePK.codice AndAlso
                                 appezzamento.APPEZZA = DatiAppezzamento.primaryKey.codice
                           Select appezzamento

        Dim app = appezzamenti.FirstOrDefault()

        If NewTransaction Then
            scope = New TransactionScope()
        End If

        If EFImprese.ImpresaExist(GiasContext, DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva) = False Then
            Throw New GiasException("Partita Iva (" & DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva & ") non anagrafica imprese. Operazione annullata")
        End If


        If EFCentri_Aziendali.CentroExist(GiasContext,
                                          DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
                                          DatiAppezzamento.primaryKey.centroAziendalePK.codice) = False Then
            Throw New GiasException("Centro aziendale (" & DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva & "/" &
                                                       DatiAppezzamento.primaryKey.centroAziendalePK.codice.ToString() & ") non anagrafica. Operazione annullata")
        End If

        If app Is Nothing Then
            Throw New GiasException("Budget_Appezzamento (" & Id_Budget.ToString() & "/" & DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva & "/" &
                                                   DatiAppezzamento.primaryKey.centroAziendalePK.codice.ToString() & "/" &
                                                   DatiAppezzamento.primaryKey.codice.ToString() &
                                               ") non trovato in anagrafica. Impossibile proseguire")
        End If

        Try
            If (DatiAppezzamento.esposizione IsNot Nothing) Then
                app.ESPOSIZ = DatiAppezzamento.esposizione.codice
            End If
            app.PENDE = DatiAppezzamento.pendenza
            If (DatiAppezzamento.ubicazione IsNot Nothing) Then
                app.UBICAZIONE = If(DatiAppezzamento.ubicazione.codice IsNot Nothing, DatiAppezzamento.ubicazione.codice, "")
            End If
            If DatiAppezzamento.superficie <> 0 Then
                app.SUP_APP = DatiAppezzamento.superficie
            End If
            app.APP_NOME = DatiAppezzamento.descrizione
            app.Campo_Cod = If(DatiAppezzamento.campoPK IsNot Nothing, DatiAppezzamento.campoPK.codice, 0)
            app.Data_Modifica = DateTime.Now
            app.Username_Modifica = username
            app.Validita_Inizio = DatiAppezzamento.validita.inizio
            app.Validita_Fine = DatiAppezzamento.validita.fine
            app.DistBZ_CorpiIdrici = DatiAppezzamento.distBZ_CorpiIdrici
            app.DistBZ_AreeResPub = DatiAppezzamento.distBZ_AreeResPub
            app.DistBZ_Allevamenti = DatiAppezzamento.distBZ_Allevamenti
            app.DistBZ_VegNatNonColt = DatiAppezzamento.distBZ_VegNatNonColt
            app.SupBZ_Riduzione = DatiAppezzamento.supBZ_Riduzione
            app.X = DatiAppezzamento.lat
            app.Y = DatiAppezzamento.lng
            app.ZSLM = DatiAppezzamento.altitudine

            'lavez - 21/03/2024 - chiavi nuovo tracciato agea
            If DatiAppezzamento.Agea_idSchedaValidazione IsNot Nothing Then
                app.Agea_idSchedaValidazione = DatiAppezzamento.Agea_idSchedaValidazione
            End If
            If DatiAppezzamento.Agea_codiBarrScheVali IsNot Nothing Then
                app.Agea_codiBarrScheVali = DatiAppezzamento.Agea_codiBarrScheVali
            End If
            If DatiAppezzamento.Agea_identificativoAppezzamento IsNot Nothing Then
                app.Agea_identificativoAppezzamento = DatiAppezzamento.Agea_identificativoAppezzamento
            End If
            If DatiAppezzamento.Agea_identificativoIsola IsNot Nothing Then
                app.Agea_identificativoIsola = DatiAppezzamento.Agea_identificativoIsola
            End If
            If DatiAppezzamento.Agea_identificativoPianoColtivazione IsNot Nothing Then
                app.Agea_identificativoPianoColtivazione = DatiAppezzamento.Agea_identificativoPianoColtivazione
            End If
            If DatiAppezzamento.Agea_idAppezzamentoOrig IsNot Nothing Then
                app.Agea_idAppezzamentoOrig = DatiAppezzamento.Agea_idAppezzamentoOrig
            End If

            app.SABBIA = DatiAppezzamento.sabbia
            app.LIMO = DatiAppezzamento.limo
            app.ARGILLA = DatiAppezzamento.argilla
            app.CLAS = CStr(If(DatiAppezzamento.classeTessitura IsNot Nothing, DatiAppezzamento.classeTessitura.codice, 0))

            GiasContext.Budget_Appezzamento.Attach(app)
            GiasContext.Entry(app).State = EntityState.Modified
            GiasContext.SaveChanges()

            Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            Dim DatiAppezzamentoStr = JsonConvert.SerializeObject(DatiAppezzamento, a)

            'Scrittura tabella Agronica_Log_Anagrafe
            Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
            Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Budget_Appezza,
                                                                                 Id_Budget,
                                                                                CStr(app.PIVA), CStr(app.SA_COD),
                                                                                CStr(app.APPEZZA), Nothing,
                                                                                Nothing,
                                                                                enum_TipoOperazioneDB.Modifica,
                                                                                objParametriServer, enum_Id_Servizio.GiasOnline,
                                                                                "", DatiAppezzamentoStr, Id_Budget:=app.Id_Budget)

            GiasContext.Agronica_Log_Anagrafe.Add(log)
            GiasContext.SaveChanges()

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If

        Catch ex As Exception
            app = Nothing
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return app
    End Function

    Public Shared Function Create_Appezzamento(ByRef id_budget As Integer,
                                               ByRef piva As String,
                                               ByRef sa_cod As Integer,
                                               ByRef username As String,
                                               ByRef objParametri As AgronicaCoreParametri,
                                               ByRef objParametriUtenti As AgronicaCoreParametri
                                              ) As Budget_Appezzamento


        Dim app As New Budget_Appezzamento

        app.Id_Budget = id_budget
        app.PIVA = piva
        app.SA_COD = sa_cod
        app.APPEZZA = NuovoAppezzamento_Cod(piva, sa_cod, objParametri, objParametriUtenti)
        app.SUP_APP = 0
        app.DATA_APP = Now.Date
        app.EP_CAMP = AGRODATAINIZIO
        app.X = 0
        app.Y = 0
        app.ZSLM = 0
        app.ESPOSIZ = ""
        app.PENDE = 0
        app.UBICAZIONE = ""
        app.NUM_DEL = 0
        app.CLAS = "0"
        app.SABBIA = Nothing
        app.LIMO = 0
        app.ARGILLA = Nothing
        app.PH = 0
        app.CALTOT = 0
        app.CALATT = 0
        app.SOSTORG = 0
        app.K2OASS = 0
        app.P2O5ASS = 0
        app.Mg = 0
        app.NTOT = 0
        app.UM_S = 0
        app.CL_DREN = 0
        app.FALDA = 0
        app.CSC = 0
        app.K2OASS_DATA = AGRODATAINIZIO
        app.MATORG = 0
        app.MATORG_DATA = AGRODATAINIZIO
        app.NOTOT_DATA = AGRODATAINIZIO
        app.NOTOT = 0
        app.P2O5ASS_DATA = AGRODATAINIZIO
        app.SUOLO_CODAttri = ""
        app.CAMPO_SPIA = 0
        app.CAMPO_SPIA_AREA = 0
        app.CS_SIPI = ""
        app.USER = username
        app.APP_NOME = ""
        app.Campo_Cod = 0
        app.Prossimo = 0
        app.DATA_INIZIO = AGRODATAINIZIO
        app.DATA_FINE = AGRODATAFINE
        app.inviato = 0
        app.Data_Creazione = DateTime.Now
        app.Data_Modifica = DateTime.Now
        app.Username_Creazione = username
        app.Username_Modifica = username
        app.Validita_Inizio = AGRODATAINIZIO
        app.Validita_Fine = AGRODATAFINE
        app.Validazione = 0
        app.Data_Validazione = DateTime.Now
        app.UserName_Validazione = ""
        app.Blk_Flag = 0
        app.Blk_Inizio_Data = AGRODATAINIZIO
        app.Blk_Inizio_Username = ""
        app.Blk_Inizio_Note = ""
        app.Blk_Fine_Data = AGRODATAFINE
        app.Blk_Fine_Username = ""
        app.Blk_Fine_Note = ""
        app.Via_Stringa = ""
        app.DistBZ_CorpiIdrici = 0
        app.DistBZ_AreeResPub = 0
        app.DistBZ_Allevamenti = 0
        app.DistBZ_VegNatNonColt = 0
        app.SupBZ_Riduzione = 0

        Return app

    End Function

    Public Shared Function Create_UtentiXAppezzamenti(ByRef piva As String,
                                               ByRef sa_cod As Integer,
                                               ByRef appezza As Integer,
                                               ByRef username As String,
                                               ByRef objParametri As AgronicaCoreParametri
                                              ) As UtentiXAppezzamenti


        Dim UxA As New UtentiXAppezzamenti

        UxA.USER = objParametri.PivaSuperUser
        UxA.PIVA = piva
        UxA.SA_COD = sa_cod
        UxA.Appezza = appezza
        UxA.inviato = 0
        UxA.Data_Creazione = DateTime.Now
        UxA.Data_Modifica = DateTime.Now
        UxA.Username_Creazione = username
        UxA.Username_Modifica = username
        UxA.Validita_Inizio = AGRODATAINIZIO
        UxA.Validita_Fine = AGRODATAFINE
        UxA.Validazione = 0
        UxA.Data_Validazione = DateTime.Now
        UxA.UserName_Validazione = ""

        Return UxA

    End Function

    Public Shared Function Create_CatastoAppezzamento(ByRef Id_Budget As Integer,
                                                      ByVal dati_catasto As AgronicaCoreModelsSTD.anagrafiche.CatastoAppezzamento,
                                                      ByRef objParametri As AgronicaCoreParametri,
                                                      ByRef appezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                                      ByRef username As String,
                                                      Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                      Optional ByVal NewTransaction As Boolean = True) As Budget_AppezzamentiXParticelle

        Dim nomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_EFAppezzamento.CreateCatastoAppezzamento()"
        Dim messaggioErrore As String = ""
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Dim appezzaxParticella As Budget_AppezzamentiXParticelle = Nothing

        Try

            If dati_catasto.particella.Sezione = "" Then
                dati_catasto.particella.Sezione = "0"
            End If

            If dati_catasto.particella.Subalterno = "" Then
                dati_catasto.particella.Subalterno = "0"
            End If

            appezzaxParticella = CreateCatastoAppezzamento(GiasContext,
                                                           Id_Budget,
                                                           appezzamento.primaryKey.centroAziendalePK.partitaIva,
                                                           appezzamento.primaryKey.centroAziendalePK.codice,
                                                           appezzamento.primaryKey.codice,
                                                           dati_catasto.particella.Prov,
                                                           dati_catasto.particella.Com,
                                                           dati_catasto.particella.Sezione,
                                                           dati_catasto.particella.Foglio,
                                                           dati_catasto.particella.Numero,
                                                           dati_catasto.particella.Subalterno,
                                                            appezzamento.validita.inizio,
                                                           appezzamento.validita.fine,
                                                           username)

            appezzaxParticella.AREA = dati_catasto.area

            If dati_catasto.area > 0 Then
                Dim ettari, are, centiare As Integer
                UtilityProvider.EttariAreCentiare_from_Ettari(dati_catasto.area, ettari, are, centiare)

                appezzaxParticella.SAU_Convenz_Ettari = ettari
                appezzaxParticella.SAU_Convenz_Are = are
                appezzaxParticella.SAU_Convenz_Centiare = centiare

            End If

            GiasContext.SaveChanges()

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As Exception
            appezzaxParticella = Nothing
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return appezzaxParticella
    End Function

    Public Shared Function Create_AppezzamentoCodici(ByRef dal As Gias_DeveloperServer_Entities,
                                                    ByRef appezzamento As Budget_Appezzamento,
                                                    ByRef id_cod As Integer,
                                                    ByRef val_cod As String,
                                                    ByRef username As String) As Budget_Appezzamento_Codici

        Return Create_AppezzamentoCodici(dal, appezzamento.Id_Budget, appezzamento.PIVA, appezzamento.SA_COD, appezzamento.APPEZZA, id_cod, val_cod, username)
    End Function

    Private Shared Function Create_AppezzamentoCodici(ByRef dal As Gias_DeveloperServer_Entities,
                                                   ByRef Id_Budget As Integer,
                                                   ByRef piva As String,
                                                   ByRef sa_cod As Integer,
                                                   ByRef appezza As Integer,
                                                   ByRef id_cod As Integer,
                                                   ByRef val_cod As String,
                                                   ByRef username As String) As Budget_Appezzamento_Codici

        Dim appezzamento As New Budget_Appezzamento_Codici

        appezzamento.Id_Budget = Id_Budget
        appezzamento.PIVA = piva
        appezzamento.sa_cod = sa_cod
        appezzamento.appezza = appezza

        appezzamento.id_cod = id_cod
        appezzamento.val_cod = val_cod

        appezzamento.inviato = 0
        appezzamento.datainvio = DateTime.Now

        appezzamento.Data_Creazione = DateTime.Now
        appezzamento.Data_Modifica = DateTime.Now

        appezzamento.Validita_Inizio = AGRODATAINIZIO
        appezzamento.Validita_Fine = AGRODATAFINE

        appezzamento.Username_Creazione = username
        appezzamento.Username_Modifica = username

        appezzamento.Validazione = 0
        appezzamento.Data_Validazione = DateTime.Now
        appezzamento.UserName_Validazione = ""

        dal.Budget_Appezzamento_Codici.Add(appezzamento)
        dal.SaveChanges()

        Return appezzamento
    End Function

    Private Shared Function ModificaAppezzamentixIndirizzi(ByVal idbudget As Integer,
                                                            ByVal piva As String,
                                                            ByVal sa_cod As Integer,
                                                            ByVal appezza As Integer,
                                                            ByVal cod_Indirizzo As Integer,
                                                            ByVal tipo_indirizzo As Integer,
                                                            ByRef username As String,
                                                            Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing
                                                            ) As Budget_AppezzamentixIndirizzi

        Dim appezzaxIndirizzi = From appezzamentixindirizzi In GiasContext.Budget_AppezzamentixIndirizzi
                                Where appezzamentixindirizzi.Id_Budget = idbudget AndAlso
                                        appezzamentixindirizzi.PIVA = piva AndAlso
                                        appezzamentixindirizzi.sa_cod = sa_cod AndAlso
                                        appezzamentixindirizzi.appezza = appezza AndAlso
                                        appezzamentixindirizzi.cod_indirizzo = cod_Indirizzo
                                Select appezzamentixindirizzi

        Dim appxind = appezzaxIndirizzi.FirstOrDefault

        If appxind Is Nothing Then
            Throw New Exception("Budget_appezzamentixindirizzi not found")
        Else
            appxind.Tipo_Indirizzo = tipo_indirizzo

            appxind.Data_Modifica = DateTime.Now
            appxind.Username_Modifica = username

            appxind.Validita_Inizio = AGRODATAINIZIO
            appxind.Validita_Fine = AGRODATAFINE

        End If

        Return appxind
    End Function
    Private Shared Function CreateCatastoAppezzamento(ByRef dal As Gias_DeveloperServer_Entities,
                                                      ByRef idbudget As Integer,
                                                      ByRef piva As String,
                                                      ByRef sa_cod As Integer,
                                                      ByRef appezza As Integer,
                                                      ByRef prov As String,
                                                      ByRef com As String,
                                                      ByRef sezione As String,
                                                      ByRef foglio As Integer,
                                                      ByRef numero As Integer,
                                                      ByRef subalterno As String,
                                                      ByRef Validita_Inizio As Date,
                                                      ByRef Validita_Fine As Date,
                                                   ByRef username As String
                                                   ) As Budget_AppezzamentiXParticelle

        Dim appXPart As New Budget_AppezzamentiXParticelle

        appXPart.Id_Budget = idbudget
        appXPart.PIVA = piva
        appXPart.SA_COD = sa_cod
        appXPart.APPEZZA = appezza

        appXPart.PROV = prov
        appXPart.COM = com
        appXPart.SEZIONE = sezione
        appXPart.FOGLIO = foglio
        appXPart.NUMERO = numero
        appXPart.SUBALTERNO = subalterno

        appXPart.AREA = 0
        appXPart.SAU_Convenz_Ettari = 0
        appXPart.SAU_Convenz_Are = 0
        appXPart.SAU_Convenz_Centiare = 0
        appXPart.SAU_Convers_Ettari = 0
        appXPart.SAU_Convers_Are = 0
        appXPart.SAU_Convers_Centiare = 0
        appXPart.SAU_Bio_Ettari = 0
        appXPart.SAU_Bio_Are = 0
        appXPart.SAU_Bio_Centiare = 0

        appXPart.inviato = 0
        appXPart.datainvio = DateTime.Now

        appXPart.Data_Creazione = DateTime.Now
        appXPart.Data_Modifica = DateTime.Now

        appXPart.Validita_Inizio = Validita_Inizio
        appXPart.Validita_Fine = Validita_Fine

        appXPart.Username_Creazione = username
        appXPart.Username_Modifica = username

        appXPart.Validazione = 0
        appXPart.Data_Validazione = DateTime.Now
        appXPart.UserName_Validazione = ""

        dal.Budget_AppezzamentiXParticelle.Add(appXPart)
        dal.SaveChanges()

        Return appXPart

    End Function
    Private Shared Function NuovoAppezzamento_Cod(piva As String,
                                                  sa_cod As Integer,
                                                  ByRef objParametri As AgronicaCoreParametri,
                                                  ByRef objParametriUtenti As AgronicaCoreParametri) As Integer

        Dim idGen As New Agro_Sequenze
        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R
        Dim dt As DataTable = objUtenti.Leggi("", "", objParametriUtenti)
        Dim progressivogias As Long = dt.Rows(0).Item("ProgressivoGIAS")

        Dim basecode As Long = 0
        Dim topcode As Long = 20000000
        idGen.Calcola_BaseCode(progressivogias, topcode, basecode, objParametri)
        Dim campo_cod = idGen.NuovoId_Appezzamento(piva, sa_cod, basecode, topcode, objParametri)

        Return campo_cod
    End Function
    Private Shared Function CreateAppezzamentixIndirizzi(ByRef dal As Gias_DeveloperServer_Entities,
                                                    ByRef idbudget As Integer,
                                                    ByRef piva As String,
                                                    ByRef sa_cod As Integer,
                                                    ByRef appezza As Integer,
                                                    ByRef cod_Indirizzo As Integer,
                                                    ByRef tipo_indirizzo As Integer,
                                                   ByRef username As String) As Budget_AppezzamentixIndirizzi

        Dim appezzamento As New Budget_AppezzamentixIndirizzi

        appezzamento.Id_Budget = idbudget
        appezzamento.PIVA = piva
        appezzamento.sa_cod = sa_cod
        appezzamento.appezza = appezza

        appezzamento.cod_indirizzo = cod_Indirizzo
        appezzamento.Tipo_Indirizzo = tipo_indirizzo

        appezzamento.inviato = 0
        appezzamento.datainvio = DateTime.Now

        appezzamento.Data_Creazione = DateTime.Now
        appezzamento.Data_Modifica = DateTime.Now

        appezzamento.Username_Creazione = username
        appezzamento.Username_Modifica = username

        appezzamento.Validita_Inizio = AGRODATAINIZIO
        appezzamento.Validita_Fine = AGRODATAFINE

        appezzamento.Validazione = 0
        appezzamento.Data_Validazione = DateTime.Now
        appezzamento.UserName_Validazione = ""

        Return appezzamento
    End Function

    Public Shared Function AppezzamentoExist(ByRef GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                             ByVal idbudget As Integer,
                                             ByVal PartitaIva As String,
                                             ByVal CentroAziendale As Integer,
                                             ByVal CodiceAppezzamento As Integer) As Boolean
        Dim nomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_EFAppezzamento.AppezzamentoExist()"
        Dim messaggioErrore As String = ""
        Dim ret As Boolean = False
        Try
            Dim appezzamenti = From appezzamento In GiasContext.Budget_Appezzamento
                               Where appezzamento.Id_Budget = idbudget AndAlso
                                     appezzamento.PIVA = PartitaIva AndAlso
                                     appezzamento.SA_COD = CentroAziendale AndAlso
                                     appezzamento.APPEZZA = CodiceAppezzamento
                               Select appezzamento

            Dim app = appezzamenti.FirstOrDefault()
            If (app IsNot Nothing) Then
                ret = True
            End If
        Catch ex As Exception
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
        Return ret

    End Function

End Class
