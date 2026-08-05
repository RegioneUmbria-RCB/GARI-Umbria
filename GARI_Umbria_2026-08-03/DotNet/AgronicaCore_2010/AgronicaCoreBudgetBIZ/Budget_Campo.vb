Imports System.Data.Common
Imports System.Xml
Imports System.Xml.Linq
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports System.Transactions
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreModelsSTD.exceptions

Public Class Budget_Campo_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '#############################################################################
    '################### Funzioni per lettura dati Angular #######################
    '#############################################################################

    Public Function Leggi_Campo(Id_Budget As Integer, Piva As String,
                                   Sa_Cod As Integer,
                                   Campo_Cod As Integer,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Campo

        Dim c As New Campo

        Dim objCampi_Dal As New AgronicaCoreBudgetDAL.Budget_Campi_R
        Dim DT = objCampi_Dal.Leggi(Id_Budget, Piva, Sa_Cod, Campo_Cod,
                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                    "", "",
                                    objParametri)

        Dim objCodici_Dal As New AgronicaCoreBudgetDAL.Budget_Campi_codici_R
        Dim codici = objCodici_Dal.Leggi(Id_Budget,
                                         Piva,
                                         Sa_Cod,
                                         Campo_Cod,
                                         0,
                                         "",
                                         AGRODATAINIZIO,
                                         AGRODATAFINE,
                                         enumSelezioneVariabile.Selezione_TabellaCompleta,
                                         "",
                                         "",
                                         objParametri) 'tolto: " id_cod <> " & enum_CodiciAnagrafe.Riferimento_Alfanumerico_Campo & " ",

        Dim objAppezzamentoR As New AgronicaCoreBudgetDAL.Budget_Appezzamento_R
        Dim appezzamenti = objAppezzamentoR.Recupera_Appezzamenti_Colture_del_Campo(Id_Budget,
                                                         Piva,
                                                         Sa_Cod,
                                                         Campo_Cod,
                                                         AGRODATAINIZIO,
                                                         AGRODATAFINE,
                                                         False,
                                                         enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                         "",
                                                         "",
                                                         objParametri)

        Dim objParticelleCatastali As New AgronicaCoreBudgetDAL.Budget_CampixParticelle_R
        Dim particelle = objParticelleCatastali.Leggi(Id_Budget,
                                                         Piva,
                                                         CInt(Sa_Cod),
                                                         CInt(Campo_Cod),
                                                         "",
                                                         "",
                                                         "",
                                                         0,
                                                         0,
                                                         "",
                                                         enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                         "",
                                                         "",
                                                         objParametri)

        If DT.Rows.Count = 0 Then
            Throw New Exception("Campo non trovato")
        End If

        c.primaryKey = New Campo.PK()
        c.primaryKey.centroAziendalePK = New CentroAziendale.PK()
        c.primaryKey.codice = DT.Rows(0)("Campo_Cod")
        c.primaryKey.centroAziendalePK.partitaIva = DT.Rows(0)("Piva")
        c.primaryKey.centroAziendalePK.codice = DT.Rows(0)("Sa_Cod")
        c.descrizione = If(IsDBNull(DT.Rows(0)("Campo_Des")), "", DT.Rows(0)("Campo_Des"))
        c.specie = New AgronicaCoreModelsSTD.metaschema.utilizzi.Specie()
        c.serra = If(DT.Rows(0)("Campo_Tipo") = 1, True, False)
        c.specie.codice = DT.Rows(0)("Veg_Cod")
        c.orientamento_Colturale = DT.Rows(0)("Gru_Cod")
        c.appezzamentoCampo = New List(Of AppezzamentoCampo)
        c.catastoCampo = New List(Of CatastoCampo)


        For Each row In appezzamenti.Rows
            Dim appezzamentoCampo As New AppezzamentoCampo()

            appezzamentoCampo.piva = row("PIVA")
            appezzamentoCampo.sa_cod = row("SA_COD")
            appezzamentoCampo.appezza = row("APPEZZA")
            appezzamentoCampo.campo_cod = row("Campo_Cod")
            appezzamentoCampo.sup_app = row("SUP_APP")
            appezzamentoCampo.app_nome = If(IsDBNull(row("APP_NOME")), "", row("APP_NOME"))
            appezzamentoCampo.validita = New IntervalloTemporale(
                If(IsDBNull(DT.Rows(0)("Validita_Inizio")), AGRODATAINIZIO, DT.Rows(0)("Validita_Inizio")),
                If(IsDBNull(DT.Rows(0)("Validita_Fine")), AGRODATAFINE, DT.Rows(0)("Validita_Fine")))
            appezzamentoCampo.id_reg = row("ID_REG")
            appezzamentoCampo.validita_impianto = New IntervalloTemporale(
                If(IsDBNull(DT.Rows(0)("Validita_Inizio")), AGRODATAINIZIO, DT.Rows(0)("Validita_Inizio")),
                If(IsDBNull(DT.Rows(0)("Validita_Fine")), AGRODATAFINE, DT.Rows(0)("Validita_Fine")))
            appezzamentoCampo.cul_cod = If(IsDBNull(row("CUL_COD")), 0, row("CUL_COD"))
            appezzamentoCampo.cul_des = If(IsDBNull(row("Cul_Des")), "", row("Cul_Des"))
            appezzamentoCampo.veg_des = If(IsDBNull(row("Veg_Des")), "", row("Veg_Des"))


            c.appezzamentoCampo.Add(appezzamentoCampo)
        Next

        For Each row In particelle.Rows
            Dim catastoCampo As New CatastoCampo()
            catastoCampo.particella = New ParticelleCatastali()
            catastoCampo.particella.primaryKey = New ParticelleCatastali.PK()
            catastoCampo.particella.primaryKey.Com = row("COM")
            catastoCampo.particella.primaryKey.Foglio = row("FOGLIO")
            catastoCampo.particella.primaryKey.Numero = row("NUMERO")
            catastoCampo.particella.primaryKey.Prov = row("PROV")
            catastoCampo.particella.primaryKey.Sezione = row("SEZIONE")
            catastoCampo.particella.primaryKey.Subalterno = row("SUBALTERNO")

            catastoCampo.particella.Area = row("AREA")
            catastoCampo.area = row("AREA")

            c.catastoCampo.Add(catastoCampo)
        Next

        c.validita = New IntervalloTemporale(
            If(IsDBNull(DT.Rows(0)("Validita_Inizio")), AGRODATAINIZIO, DT.Rows(0)("Validita_Inizio")),
             If(IsDBNull(DT.Rows(0)("Validita_Fine")), AGRODATAFINE, DT.Rows(0)("Validita_Fine")))

        c.codici = New List(Of CodiciAnagrafeValori)

        For Each row In codici.Rows
            If row("id_cod") = enum_CodiciAnagrafe.Riferimento_Alfanumerico_Campo Then
                c.campo_Codice = row("val_cod")
            ElseIf (row("id_cod") < 2000 Or row("id_cod") > 3000) Then
                Dim codiceAnagrafeValori As New CodiciAnagrafeValori()
                codiceAnagrafeValori.validita = New IntervalloTemporale(row("Validita_Inizio"), row("Validita_Fine"))
                codiceAnagrafeValori.valore = row("val_cod")
                codiceAnagrafeValori.codiceAnagrafe = New CodiceAnagrafe(row("id_cod")) With {
                        .descrizione = row("descrizione")
                    }
                c.codici.Add(codiceAnagrafeValori)
            End If
        Next

        Return c
    End Function

    '#############################################################################
    '############### Fine Funzioni per lettura dati Angular ######################
    '#############################################################################

End Class

Public Class Budget_Campo_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_Campo_Anagrafica(ByRef Id_Budget As Integer,
                                            ByRef objCampo As Campo,
                                            ByVal tipoOperazione As enum_TipoOperazioneDB,
                                            ByRef objParametri_Server As AgronicaCoreParametri,
                                            ByRef objParametri_Utenti As AgronicaCoreParametri,
                                            Optional Delete_Reale_Da_Ribaltamento As Boolean = False,
                                            Optional listErroriGias As List(Of ErroreGias) = Nothing) As String

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser
        Dim dal As New AgronicaCoreBudgetDAL.Budget_EFCampi()

        Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze
        Dim success As Boolean = True

        Dim NomeRoutine As String = "AgronicaCoreBudgetBIZ.Campo_W.Scrivi_Campo_Anagrafica()"
        Dim MessaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)

        Dim Ribaltamento_Campo As New AgronicaCoreEntityFramework_POCO.Ribaltamento_Campi
        Dim listRibaltamento_Appezzamento As New List(Of AgronicaCoreEntityFramework_POCO.Ribaltamento_Appezzamento)
        Dim _campo_cod As Integer = objCampo.primaryKey.codice

        Dim stWa As New Stopwatch
        stWa.Start()

        Dim scopeOption As New TransactionScopeOption
        Dim transactionOptions As New TransactionOptions
        transactionOptions.IsolationLevel = IsolationLevel.ReadUncommitted
        Using scope As New TransactionScope(scopeOption, transactionOptions)
            Using GiasContext As Gias_DeveloperServer_Entities = Gias_EF_Utility.CreateGiasContextConnection(objParametri_Server.StringaConnessione)



                Try
                    'dal.ContextOptions.UseLegacyPreserveChangesBehavior = False
                    'Open the contextObject connection state explicitly
                    GiasContext.Database.Connection.Open()

                    'disabilitaMergeOptions(dal)

                    GiasContext.Database.ExecuteSqlCommand("SET ARITHABORT ON;")

                    If tipoOperazione = enum_TipoOperazioneDB.Modifica Then
                        Verifica_ValiditaInizioFine(Id_Budget, objCampo, objParametri_Server, objParametri_Utenti)
                    End If

                    If (tipoOperazione = enum_TipoOperazioneDB.Scrittura) Then
#Region "Scrittura"
                        'Verifica_ValiditaInizioFine(objCampo, objParametri_Server)
                        Dim Campo_POCO = dal.Campo_Scrivi_EF(Id_Budget,
                                                             objCampo,
                                                             objParametri_Server,
                                                             objParametri_Utenti,
                                                             objParametri_Server.UsernameOperazione,
                                                             GiasContext,
                                                             False)

                        'UtentiXCampi_Scrivi_Campo(Campo_POCO,
                        '                          tipoOperazione,
                        '                          objParametri_Server,
                        '                          objParametri_Utenti)

                        CampiXCodici_ScriviModificaCancella_Campo_EF(Id_Budget,
                                                                     objCampo,
                                                                     tipoOperazione,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     GiasContext,
                                                                     False,
                                                                     Campo_POCO)

                        Appezzamenti_ScriviModificaCancella_Campo(Id_Budget,
                                                                  objCampo,
                                                                  tipoOperazione,
                                                                  objParametri_Server,
                                                                  objParametri_Utenti,
                                                                  Campo_POCO)

                        CampiXParticelle_ScriviModificaCancella_Campo(Id_Budget,
                                                                      objCampo,
                                                                      tipoOperazione,
                                                                      objParametri_Server,
                                                                      objParametri_Utenti,
                                                                      Campo_POCO)

                        '############# STO IGNORANDO LE ENTITA GRAFICHE ##################
#End Region
                    ElseIf (tipoOperazione = enum_TipoOperazioneDB.Modifica) Then
#Region "Modifica"
                        '--------------------------------------------------------------------------------------
                        ' aggiorno le validita solo se è date di inizio o fine del campo sono variate
                        Dim campo_read As New AgronicaCoreBudgetDAL.Budget_Campi_R
                        Dim campo_old = campo_read.Leggi(Id_Budget, objCampo.primaryKey.centroAziendalePK.partitaIva,
                                                                objCampo.primaryKey.centroAziendalePK.codice,
                                                                objCampo.primaryKey.codice,
                                                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                "", "",
                                                                objParametri_Server)

                        Dim Campo_POCO = dal.Campo_Modifica_EF(Id_Budget, objCampo, objParametri_Server, objParametri_Server.UsernameOperazione, objParametri_Utenti, GiasContext, False)

                        If objCampo.validita.inizio <> campo_old.Rows(0)("Validita_Inizio") Or objCampo.validita.fine <> campo_old.Rows(0)("Validita_Fine") Then

                            'Validita_UtentiXCampi_Modifica_Campo(objCampo,
                            '                                     tipoOperazione,
                            '                                     objParametri_Server,
                            '                                     objParametri_Utenti)

                            Validita_CampiXParticelle_Modifica_Campo(Id_Budget,
                                                                     objCampo,
                                                                     tipoOperazione,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti)

                            Validita_Appezzamenti_Modifica_Campo(Id_Budget,
                                                                 objCampo,
                                                                 tipoOperazione,
                                                                 objParametri_Server,
                                                                 objParametri_Utenti)
                        End If
                        '--------------------------------------------------------------------------------------

                        Appezzamenti_ScriviModificaCancella_Campo(Id_Budget,
                                                                  objCampo,
                                                                  tipoOperazione,
                                                                  objParametri_Server,
                                                                  objParametri_Utenti)

                        CampiXParticelle_ScriviModificaCancella_Campo(Id_Budget,
                                                                      objCampo,
                                                                      tipoOperazione,
                                                                      objParametri_Server,
                                                                      objParametri_Utenti)

                        CampiXCodici_ScriviModificaCancella_Campo_EF(Id_Budget,
                                                                     objCampo,
                                                                     tipoOperazione,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     GiasContext,
                                                                     False,
                                                                     Campo_POCO)

                        '############# STO IGNORANDO LE ENTITA GRAFICHE ##################
#End Region
                    ElseIf (tipoOperazione = enum_TipoOperazioneDB.Cancellazione) Then
#Region "Cancellazione"

                        'Appezzamenti_Cancella_Campo(objCampo,
                        '                            tipoOperazione,
                        '                            objParametri_Server,
                        '                            objParametri_Utenti)

                        Dim objAppezza_DAL As New AgronicaCoreBudgetDAL.Budget_Appezzamento_R
                        Dim objAppezza_BIZ_W As New AgronicaCoreBudgetBIZ.Budget_Appezzamento_W
                        Dim objAppezza_BIZ_R As New AgronicaCoreBudgetBIZ.Budget_Appezzamento_R
                        Dim dtAppezza = objAppezza_DAL.LeggiconCampo(Id_Budget, objCampo.primaryKey.centroAziendalePK.partitaIva,
                                                 objCampo.primaryKey.centroAziendalePK.codice,
                                                 objCampo.primaryKey.codice,
                                                 enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                 "", "", objParametri_Server)

                        For Each rowAppezza In dtAppezza.Rows
                            Dim idbudget As Integer = rowAppezza("Id_Budget")
                            Dim Piva As String = rowAppezza("Piva")
                            Dim Sa_Cod As Long = rowAppezza("Sa_Cod")
                            Dim Appezza As Long = rowAppezza("Appezza")

                            Dim appezzamento As New Appezzamento
                            appezzamento.primaryKey = New Appezzamento.PK(Appezza, New CentroAziendale.PK(Sa_Cod, Piva))
                            appezzamento.flag_cancellazione = True

                            objAppezza_BIZ_W.Appezzamento_ScriviModifica(idbudget, appezzamento, objParametri_Server, objParametri_Utenti,
                                                                         listRibaltamento_Appezzamento:=listRibaltamento_Appezzamento)
                        Next

                        CampiXParticelle_ScriviModificaCancella_Campo(Id_Budget,
                                                                      objCampo,
                                                                      tipoOperazione,
                                                                      objParametri_Server,
                                                                      objParametri_Utenti)

                        'UtentiXCampi_Cancella_Campo(objCampo,
                        '                            tipoOperazione,
                        '                            objParametri_Server,
                        '                            objParametri_Utenti)

                        ProgrammazioneEntita_Cancella_Campo(objCampo,
                                                            tipoOperazione,
                                                            objParametri_Server,
                                                            objParametri_Utenti)

                        CampiXCodici_ScriviModificaCancella_Campo_EF(Id_Budget,
                                                                     objCampo,
                                                                     tipoOperazione,
                                                                     objParametri_Server,
                                                                     objParametri_Utenti,
                                                                     GiasContext,
                                                                     False)

                        AgronicaCoreBudgetDAL.Budget_EFCampi.Campo_Cancella_EF(Id_Budget, objCampo, objParametri_Server, objParametri_Utenti, GiasContext, False)

                        Dim _piva As String = objCampo.primaryKey.centroAziendalePK.partitaIva
                        Dim _sa_cod As Integer = objCampo.primaryKey.centroAziendalePK.codice
                        Ribaltamento_Campo = (From rib In GiasContext.Ribaltamento_Campi
                                              Where rib.Budget_Piva = _piva AndAlso
                                                  rib.Budget_Sa_Cod = _sa_cod AndAlso
                                                  rib.Budget_Campo_Cod = _campo_cod).FirstOrDefault()
                        If Ribaltamento_Campo IsNot Nothing Then
                            AgronicaCoreAnagrafeBIZ.RibaltamentoAnagraficheColturali_W.Clean_Tabelle_Ribaltamento_daDeleteElemento(Id_Budget, _piva, _sa_cod, 0, GiasContext, _campo_cod)
                        End If

                        '############# STO IGNORANDO LE ENTITA GRAFICHE ##################
#End Region
                    End If
                    scope.Complete()

                Catch ex As GiasException
                    If scope IsNot Nothing Then
                        scope.Dispose()
                    End If
                    MessaggioErrore = ex.Message
                    Throw ex
                Catch ex As Exception
                    scope.Dispose()
                    MessaggioErrore = ex.Message
                    Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)

                Finally

                    If GiasContext IsNot Nothing AndAlso GiasContext.Database.Connection.State = ConnectionState.Open Then
                        GiasContext.Database.Connection.Close()
                    End If

                End Try

            End Using
        End Using

        stWa.Stop()
        Dim totalTime As TimeSpan = stWa.Elapsed


        If Delete_Reale_Da_Ribaltamento Then
            Dim deleteCampo As Boolean = True
            For Each Ribaltamento_Appezzamento In listRibaltamento_Appezzamento
                Using scope As New TransactionScope(scopeOption, transactionOptions)
                    Using GiasContext As Gias_DeveloperServer_Entities = Gias_EF_Utility.CreateGiasContextConnection(objParametri_Server.StringaConnessione)
                        Dim _app_nome As String = ""
                        Try
                            GiasContext.Database.Connection.Open()

                            GiasContext.Database.ExecuteSqlCommand("SET ARITHABORT ON;")

                            _app_nome = (From a In GiasContext.Appezzamento
                                         Where a.PIVA = Ribaltamento_Appezzamento.Reale_Piva AndAlso
                                             a.SA_COD = Ribaltamento_Appezzamento.Reale_Sa_Cod AndAlso
                                             a.APPEZZA = Ribaltamento_Appezzamento.Reale_Appezza
                                         Select a.APP_NOME).FirstOrDefault()

                            Dim msgCancellazioneReale = AgronicaCoreAnagrafeBIZ.Appezzamento_W.Internal_Delete_Appezzamento_Reale_Da_Ribaltamento(Ribaltamento_Appezzamento, objParametri_Server, objParametri_Utenti,
                                                                                                                                                  GiasContext, False,
                                                                                                                                                  NoteLog:="Cancellato da Eliminazione Campo Budget (" & Ribaltamento_Appezzamento.Budget_Piva & "-" & Ribaltamento_Appezzamento.Budget_Sa_Cod & "-" & _campo_cod & "), Id_Budget: " & Id_Budget)
                            If msgCancellazioneReale <> "" Then
                                deleteCampo = False

                                listErroriGias.Add(New ErroreGias With {
                                         .messaggio = _app_nome & ": " & msgCancellazioneReale,
                                         .severity = ErroreGias_Severity.Info})
                            End If

                            scope.Complete()

                        Catch ex As Exception

                            scope.Dispose()

                            deleteCampo = False
                            listErroriGias.Add(New ErroreGias With {
                                         .messaggio = _app_nome & ": " & ex.Message,
                                         .severity = ErroreGias_Severity.Info})

                        Finally
                            If GiasContext IsNot Nothing AndAlso GiasContext.Database.Connection.State = ConnectionState.Open Then
                                GiasContext.Database.Connection.Close()
                            End If
                        End Try

                    End Using
                End Using
            Next

            'IL CAMPO REALE NON VIENE ELIMINATO PERCHE POTREBBE ESSERE STATO USATO IN ALTRI APPEZZAMENTI
            'If deleteCampo AndAlso Ribaltamento_Campo IsNot Nothing Then

            '        Dim msgCancellazioneReale = Delete_Campo_Reale_Da_Ribaltamento(Ribaltamento_Campo, objParametri_Server, objParametri_Utenti)

            '        If msgCancellazioneReale <> "" Then
            '            listErroriGias.Add(New ErroreGias With {
            '                               .messaggio = msgCancellazioneReale,
            '                               .severity = ErroreGias_Severity.Info})
            '        End If
            '    End If

        End If

        Return MessaggioErrore

    End Function

    '#####Private functions#####

    Private Function Verifica_ValiditaInizioFine(ByRef Id_Budget As Integer, ByRef campo As AgronicaCoreModelsSTD.anagrafiche.Campo,
                                                ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)
        'Passaggio Id_Budget
        Dim keybudget = Id_Budget

        Dim MessaggioErrore As String = ""

        Dim Validita_Fine = campo.validita.fine
        Dim Validita_Inizio = campo.validita.inizio

        Dim sa_cod = campo.primaryKey.centroAziendalePK.codice
        Dim piva = campo.primaryKey.centroAziendalePK.partitaIva
        Dim campo_cod = campo.primaryKey.codice

        Dim objCentriR As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        Dim centro = objCentriR.Leggi(piva, sa_cod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        Dim objCampoR As New AgronicaCoreBudgetDAL.Budget_Campi_R
        Dim camp = objCampoR.Leggi(keybudget, piva, sa_cod, campo_cod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        Dim objAppR As New AgronicaCoreBudgetDAL.Budget_Appezzamento_R
        Dim app = objAppR.LeggiconCampo(keybudget, piva, sa_cod, campo_cod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        Try

            If camp.Rows.Count > 0 Then

                'Se l'intervallo delle validità viene modificato, controllo e aggiorno, se necessario, l'Appezzamento
                If Validita_Inizio <> camp.Rows(0)("Validita_Inizio") Or Validita_Fine <> camp.Rows(0)("Validita_Fine") Then

                    If app.Rows.Count > 0 Then
                        For Each a In app.Rows
                            Dim appezza = a("appezza")

                            'Ripristino le date dopo ogni iterazione
                            Dim Validita_Inizio_AppNew = Validita_Inizio
                            Dim Validita_Fine_AppNew = Validita_Fine

                            'Controllo se le date dell'Appezzamento rientrano nell'intervallo temporale del Centro, in questo caso rimangono invariate
                            If a("Validita_Inizio") > Validita_Inizio_AppNew Then
                                Validita_Inizio_AppNew = a("Validita_Inizio")
                            End If
                            If Validita_Fine_AppNew > a("Validita_Fine") Then
                                Validita_Fine_AppNew = a("Validita_Fine")
                            End If

                            Dim objAppsR As New AgronicaCoreBudgetBIZ.Budget_Appezzamento_R
                            Dim appR = objAppsR.Leggi_Appezzamento_Anagrafica(Id_Budget:=keybudget,
                                                                          Piva:=piva,
                                                                          Sa_Cod:=sa_cod,
                                                                          Appezza:=appezza,
                                                                          0,
                                                                          Leggi_Impianti:=True,
                                                                          Leggi_Indirizzi:=True,
                                                                          Leggi_Catasto:=True,
                                                                          data:=AGRODATAINIZIO,
                                                                          filtroData:=False,
                                                                          Leggi_Distinte:=True,
                                                                          False,
                                                                          objParametri_Server,
                                                                          objParametri_Server,
                                                                          objParametri_Utenti
                                                                          )

                            appR.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(Validita_Inizio_AppNew, Validita_Fine_AppNew)

                            '------------------------
                            'CONTROLLO DATE IMPIANTI
                            '------------------------
                            Dim impiantiDaEliminare As New List(Of AgronicaCoreModelsSTD.anagrafiche.Impianto)
                            Dim eserciziDaEliminare As New List(Of AgronicaCoreModelsSTD.anagrafiche.Esercizio)

                            For Each impianto In appR.impianti

                                'Ripristino le date dopo ogni iterazione
                                Dim Validita_Inizio_Impianto = Validita_Inizio_AppNew
                                Dim Validita_Fine_Impianto = Validita_Fine_AppNew

                                'se la data di inizio dell'impianto è SUCCESSIVA alla FINE dell'Appezzamento, elimino l'Impianto
                                'se la data di fine dell'impianto è PRECEDENTE all'INIZIO dell'Appezzamento, elimino l'Impianto
                                If impianto.validita.inizio > Validita_Fine_AppNew Or impianto.validita.fine < Validita_Inizio_AppNew Then
                                    impiantiDaEliminare.Add(impianto)

                                    'Controllo se le date dell'Impianto rientrano nell'intervallo temporale dell'Appezzamento, in questo caso rimangono invariate
                                ElseIf impianto.validita.inizio >= Validita_Inizio_AppNew AndAlso impianto.validita.fine <= Validita_Fine_AppNew Then
                                    If impianto.validita.inizio > Validita_Inizio_AppNew Then
                                        Validita_Inizio_Impianto = impianto.validita.inizio
                                    End If
                                    If impianto.validita.fine < Validita_Fine_AppNew Then
                                        Validita_Fine_Impianto = impianto.validita.fine
                                    End If
                                End If

                                'aggiorno le validità solo se l'Impianto non è stato cancellato
                                If Not impiantiDaEliminare.Contains(impianto) Then
                                    impianto.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(Validita_Inizio_Impianto, Validita_Fine_Impianto)
                                End If


                                '------------------------
                                'CONTROLLO DATE ESERCIZI
                                '------------------------
                                For Each esercizio In impianto.esercizi

                                    'Ripristino le date dopo ogni iterazione
                                    Dim Validita_Inizio_Esercizio = Validita_Inizio_Impianto
                                    Dim Validita_Fine_Esercizio = Validita_Fine_Impianto

                                    'se la data di inizio dell'Esercizio è SUCCESSIVA alla FINE dell'Impianto, elimino l'Esercizio
                                    'se la data di fine dell'Esercizio è PRECEDENTE all'INIZIO dell'Impianto, elimino l'Esercizio
                                    If esercizio.validita.inizio > Validita_Fine_Impianto Or esercizio.validita.fine < Validita_Inizio_Impianto Then
                                        eserciziDaEliminare.Add(esercizio)

                                        'Controllo se le date dell'Esercizio rientrano nell'intervallo temporale dell'Impianto, in questo caso rimangono invariate
                                    ElseIf esercizio.validita.inizio >= Validita_Inizio_Impianto AndAlso esercizio.validita.fine <= Validita_Fine_Impianto Then
                                        If esercizio.validita.inizio > Validita_Inizio_Impianto Then
                                            Validita_Inizio_Esercizio = esercizio.validita.inizio
                                        End If
                                        If esercizio.validita.fine < Validita_Fine_Impianto Then
                                            Validita_Fine_Esercizio = esercizio.validita.fine
                                        End If
                                    End If

                                    'aggiorno le validità solo se l'Esercizio non è stato cancellato
                                    If Not eserciziDaEliminare.Contains(esercizio) Then
                                        esercizio.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(Validita_Inizio_Esercizio, Validita_Fine_Esercizio)
                                    End If
                                Next

                                For Each esercizio In eserciziDaEliminare
                                    impianto.esercizi.Remove(esercizio)
                                Next
                            Next

                            For Each impianto In impiantiDaEliminare
                                appR.impianti.Remove(impianto)
                            Next

                            Dim objAppsW As New AgronicaCoreBudgetBIZ.Budget_Appezzamento_W
                            Dim appW = objAppsW.Appezzamento_ScriviModifica(keybudget, appR, objParametri_Server, objParametri_Utenti)
                        Next
                    End If

                    'Controlli con  Centro Aziendale
                    If Validita_Inizio < CDate(centro.Rows(0)("Validita_Inizio")) Then
                        MessaggioErrore += ("L'inizio  non può precedere la creazione del Centro Aziendale.") & " (" & CDate(centro.Rows(0)("Validita_Inizio")).ToShortDateString & ")."
                        Throw New GiasException(MessaggioErrore)
                    End If

                    If Validita_Fine > CDate(centro.Rows(0)("Validita_Fine")) Then
                        MessaggioErrore += ("La fine del Campo non può seguire la cessazione del Centro Aziendale") & " (" & CDate(centro.Rows(0)("Validita_Fine")).ToShortDateString & ")."
                        Throw New GiasException(MessaggioErrore)
                    End If
                End If
            End If

            If Validita_Fine < Validita_Inizio Then
                MessaggioErrore += ("La fine del Campo non può precedere la sua data di inizio.")
                Throw New GiasException(MessaggioErrore)
            End If

        Catch ex As GiasException
            MessaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception(MessaggioErrore)
        End Try

        Return MessaggioErrore
    End Function

    Private Function UtentiXCampi_Scrivi_Campo(
            ByRef Campo As AgronicaCoreEntityFramework_POCO.Budget_Campi,
            ByVal tipoOperazione As AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB,
            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser

        Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.EFCampi.Funzione_Accessoria_Scrivi_Campo()"
        Dim MessaggioErrore As String = String.Empty

        Dim objUtentixCampi As New AgronicaCoreAnagrafeDAL.UtentixCampi_Write

        Try
            objUtentixCampi.Scrivi(Campo.Piva,
                                   Campo.Sa_Cod,
                                   Campo.Campo_Cod,
                                   Campo.Validita_Inizio,
                                   Campo.Validita_Fine,
                                   objParametri_Server)
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return MessaggioErrore

    End Function

    Private Function Validita_UtentiXCampi_Modifica_Campo(
            ByRef objCampo As AgronicaCoreModelsSTD.anagrafiche.Campo,
            ByVal tipoOperazione As AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB,
            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser

        Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_EFCampi.Funzione_Accessoria_Scrivi_Campo()"
        Dim MessaggioErrore As String = String.Empty

        Dim objUtentixCampi As New AgronicaCoreAnagrafeDAL.UtentixCampi_Write

        Try
            objUtentixCampi.AggiornaValiditaInizio(objCampo.primaryKey.centroAziendalePK.partitaIva,
                                                   objCampo.primaryKey.centroAziendalePK.codice,
                                                   objCampo.primaryKey.codice,
                                                   objCampo.validita.inizio,
                                                   "",
                                                   objParametri_Server)

            objUtentixCampi.AggiornaValiditaFine(objCampo.primaryKey.centroAziendalePK.partitaIva,
                                                   objCampo.primaryKey.centroAziendalePK.codice,
                                                   objCampo.primaryKey.codice,
                                                   objCampo.validita.fine,
                                                   "",
                                                   objParametri_Server)
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return MessaggioErrore

    End Function

    Private Function CampiXCodici_ScriviModificaCancella_Campo_EF(ByRef Id_Budget As Integer,
            ByRef objCampo As AgronicaCoreModelsSTD.anagrafiche.Campo,
            ByVal tipoOperazioneCampo As AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB,
            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
            Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
            Optional ByVal NewTransaction As Boolean = True,
            Optional ByRef Campo As AgronicaCoreEntityFramework_POCO.Budget_Campi = Nothing) As String

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser

        Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_EFCampi.Funzione_Accessoria_Scrivi_Campo()"
        Dim MessaggioErrore As String = String.Empty

        Dim bCloseContext As Boolean = False
        Dim scope As TransactionScope = Nothing

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri_Server.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Dim objCampiXCodici As New AgronicaCoreBudgetDAL.Budget_Campi_codici_W
        Dim objCodici As New AgronicaCoreBudgetBIZ.Budget_Codici_W

        Try

            'TODO Salvo: devo riguardare cosa fa la funzione qui sotto
            If tipoOperazioneCampo <> enum_TipoOperazioneDB.Cancellazione Then
                If IsNothing(Campo) Then
                    Throw New Exception("Non è stato possibile scrivere/modificare i codici campo perché Campo_POCO è Nothing")
                ElseIf objCampo.codici IsNot Nothing Then
                    'TODO Salvo: CONTROLLARE CHE EFFETTIVAMENTE QUESTA FUNZIONE CANCELLI TUTTI I CODICI
                    objCampiXCodici.Cancella(Id_Budget,
                                     objCampo.primaryKey.centroAziendalePK.partitaIva,
                                     objCampo.primaryKey.centroAziendalePK.codice,
                                     If(tipoOperazioneCampo = enum_TipoOperazioneDB.Scrittura, Campo.Campo_Cod, objCampo.primaryKey.codice),
                                     0,
                                     "(id_cod < 2000 OR id_cod >= 3000)",
                                     objParametri_Server)

                    For Each codice In objCampo.codici
                        objCodici.Scrivi_Codici_Campo(
                            codice,
                            GiasContext,
                            Campo,
                            objParametri_Server,
                            objParametri_Utenti,
                            False)
                    Next
                End If

                If IsNothing(Campo) Then
                    Throw New Exception("Non è stato possibile scrivere/modificare i codici campo perché Campo_POCO è Nothing")
                Else

                    If objCampo.campo_Codice IsNot Nothing Then
                        Dim campo_codice As New CodiciAnagrafeValori()
                        campo_codice.valore = objCampo.campo_Codice
                        campo_codice.codiceAnagrafe = New CodiceAnagrafe()
                        campo_codice.codiceAnagrafe.codice = 1279 'questo è id_cod fisso per campo_Codice

                        objCodici.Scrivi_Codici_Campo(
                            campo_codice,
                            GiasContext,
                            Campo,
                            objParametri_Server,
                            objParametri_Utenti,
                            False)
                    End If

                End If
            Else
                objCampiXCodici.Cancella(Id_Budget,
                                     objCampo.primaryKey.centroAziendalePK.partitaIva,
                                     objCampo.primaryKey.centroAziendalePK.codice,
                                     If(tipoOperazioneCampo = enum_TipoOperazioneDB.Scrittura, Campo.Campo_Cod, objCampo.primaryKey.codice),
                                     0,
                                     "(id_cod < 2000 OR id_cod >= 3000)",
                                     objParametri_Server)
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return MessaggioErrore

    End Function

    Private Function Appezzamenti_ScriviModificaCancella_Campo(ByRef Id_budget As Integer,
            ByRef objCampo As AgronicaCoreModelsSTD.anagrafiche.Campo,
            ByVal tipoOperazioneCampo As AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB,
            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
            Optional ByRef Campo As AgronicaCoreEntityFramework_POCO.Budget_Campi = Nothing) As String

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser

        Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_EFCampi.Funzione_Accessoria_Scrivi_Campo()"
        Dim MessaggioErrore As String = String.Empty

        Try
            'TODO Salvo:
            '########################################################################################################
            '#################### DA RICHIAMARE LA FUNZIONE DI CANCELLAZIONE PER GLI APPEZZAMENTI ###################
            '########################################################################################################

            Dim appezzamenti_new As List(Of AppezzamentoCampo)
            Dim objAppezzamentoR As New AgronicaCoreBudgetDAL.Budget_Appezzamento_R

            Dim objCampiW As New AgronicaCoreBudgetDAL.Budget_Campi_W

            Dim appezzamenti_old = objAppezzamentoR.Recupera_Appezzamenti_Colture_del_Campo(Id_budget,
                    objCampo.primaryKey.centroAziendalePK.partitaIva,
                    objCampo.primaryKey.centroAziendalePK.codice,
                    If(tipoOperazioneCampo = enum_TipoOperazioneDB.Scrittura, Campo.Campo_Cod, objCampo.primaryKey.codice),
                    AGRODATAINIZIO,
                    AGRODATAFINE,
                    False,
                    enumSelezioneVariabile.Selezione_JoinDescrizioni,
                    "",
                    "",
                    objParametri_Server)

            For i = 0 To appezzamenti_old.Rows.Count - 1
                objCampiW.Disaggrega(Id_budget,
                                     objCampo.primaryKey.centroAziendalePK.partitaIva,
                                     objCampo.primaryKey.centroAziendalePK.codice,
                                     If(tipoOperazioneCampo = enum_TipoOperazioneDB.Scrittura, Campo.Campo_Cod, objCampo.primaryKey.codice),
                                     appezzamenti_old.Rows(i).Item("Appezza"),
                                     appezzamenti_old.Rows(i).Item("Validita_Inizio"),
                                     appezzamenti_old.Rows(i).Item("Validita_Fine"),
                                     objParametri_Server)
            Next

            If tipoOperazioneCampo <> enum_TipoOperazioneDB.Cancellazione And objCampo.appezzamentoCampo IsNot Nothing Then
                appezzamenti_new = objCampo.appezzamentoCampo
                For Each app In appezzamenti_new

                    If app.validita.inizio < objCampo.validita.inizio Then

                        Throw New GiasException("La Validità Inizio Non Comprende Alcuni Appezzamenti Selezionati")

                    End If

                    If app.validita.fine > objCampo.validita.fine Then

                        Throw New GiasException("La Validità Finale Non Comprende Alcuni Appezzamenti Selezionati")

                    End If

                    objCampiW.Aggrega(Id_budget,
                                      objCampo.primaryKey.centroAziendalePK.partitaIva,
                                      objCampo.primaryKey.centroAziendalePK.codice,
                                      If(tipoOperazioneCampo = enum_TipoOperazioneDB.Scrittura, Campo.Campo_Cod, objCampo.primaryKey.codice),
                                      app.appezza,
                                      app.validita.inizio,
                                      app.validita.fine,
                                      objParametri_Server)
                Next
            End If

        Catch ex As GiasException
            MessaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return MessaggioErrore

    End Function

    Private Function Validita_Appezzamenti_Modifica_Campo(ByRef Id_Budget As Integer,
            ByRef objCampo As AgronicaCoreModelsSTD.anagrafiche.Campo,
            ByVal tipoOperazioneCampo As AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB,
            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser

        Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Campo_W.Validita_Appezzamenti_Modifica_Campo()"
        Dim MessaggioErrore As String = String.Empty

        Dim objAppezzamenti_W As New AgronicaCoreAnagrafeDAL.Appezzamento_Write
        Dim objAppezzamenti_R As New AgronicaCoreBudgetDAL.Budget_Appezzamento_R
        Dim objAgenda_R As New AgronicaCoreContabDAL.Mov_Destinazioni_R

        Try

            Dim dtAppezza = objAppezzamenti_R.LeggiconCampo(Id_Budget,
                                                            objCampo.primaryKey.centroAziendalePK.partitaIva,
                                                            objCampo.primaryKey.centroAziendalePK.codice,
                                                            objCampo.primaryKey.codice,
                                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                            "", "", objParametri_Server)

            'Dim Validita_Inizio_Orig = objParametri_Server.FinestraTemporaleInizio
            'Dim Validita_Fine_Orig = objParametri_Server.FinestraTemporaleFine

            Dim Validita_Inizio = objCampo.validita.inizio
            Dim Validita_Fine = objCampo.validita.fine

            For Each RowAppezza In dtAppezza.Rows
                Dim piva = RowAppezza("piva")
                Dim sa_cod = RowAppezza("sa_cod")
                Dim appezza = RowAppezza("appezza")

                Dim validita_inizio_appezza As Date = RowAppezza("Validita_Inizio")
                Dim validita_fine_appezza As Date = RowAppezza("Validita_Fine")

                Dim Validita_InizioNew = Validita_Inizio
                Dim Validita_FineNew = Validita_Fine

                If validita_inizio_appezza > Validita_InizioNew Then
                    Validita_InizioNew = validita_inizio_appezza
                End If
                If Validita_FineNew > validita_fine_appezza Then
                    Validita_FineNew = validita_fine_appezza
                End If

                Dim objAppsR As New AgronicaCoreBudgetBIZ.Budget_Appezzamento_R
                Dim appR = objAppsR.Leggi_Appezzamento_Anagrafica(Id_Budget:=Id_Budget,
                                                                  Piva:=piva,
                                                                  Sa_Cod:=sa_cod,
                                                                  Appezza:=appezza,
                                                                  0,
                                                                  Leggi_Impianti:=True,
                                                                  Leggi_Indirizzi:=True,
                                                                  Leggi_Catasto:=True,
                                                                  data:=AGRODATAINIZIO,
                                                                  filtroData:=False,
                                                                  Leggi_Distinte:=True,
                                                                  False,
                                                                  objParametri_Server,
                                                                  objParametri_Server,
                                                                  objParametri_Utenti
                                                                  )

                appR.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(Validita_InizioNew, Validita_FineNew)

                Dim objAppsW As New AgronicaCoreBudgetBIZ.Budget_Appezzamento_W
                Dim appW = objAppsW.Appezzamento_ScriviModifica(Id_Budget, appR, objParametri_Server, objParametri_Utenti)

                'If objCampo.validita.inizio > validita_inizio_appezza Then
                '    objParametri_Server.FinestraTemporaleInizio = validita_inizio_appezza
                '    objParametri_Server.FinestraTemporaleFine = objCampo.validita.inizio
                '    Dim dt = objAgenda_R.Leggi_conImpianti(objCampo.primaryKey.centroAziendalePK.partitaIva,
                '                                            objCampo.primaryKey.centroAziendalePK.codice,
                '                                            0, 0, 0, RowAppezza("Appezza"),
                '                                            0, "", "", objParametri_Server)
                '    If dt.Rows.Count > 0 Then
                '        objParametri_Server.FinestraTemporaleInizio = Validita_Inizio_Orig
                '        objParametri_Server.FinestraTemporaleFine = Validita_Fine_Orig
                '        Throw New Exception("Sono presenti operazioni di agenda precedenti alla chiusura del campo per l'appezzamento " & CStr(RowAppezza("App_Nome")))
                '    End If
                'End If

                'If objCampo.validita.fine < validita_fine_appezza Then
                '    objParametri_Server.FinestraTemporaleInizio = objCampo.validita.fine
                '    objParametri_Server.FinestraTemporaleFine = validita_fine_appezza
                '    Dim dt = objAgenda_R.Leggi_conImpianti(objCampo.primaryKey.centroAziendalePK.partitaIva,
                '                                           objCampo.primaryKey.centroAziendalePK.codice,
                '                                           0, 0, 0, RowAppezza("Appezza"),
                '                                           0, "", "", objParametri_Server)
                '    If dt.Rows.Count > 0 Then
                '        objParametri_Server.FinestraTemporaleInizio = Validita_Inizio_Orig
                '        objParametri_Server.FinestraTemporaleFine = Validita_Fine_Orig
                '        Throw New Exception("Sono presenti operazioni di agenda successive alla chiusura del campo per l'appezzamento " & CStr(RowAppezza("App_Nome")))
                '    End If
                'End If

            Next

            For Each app In objCampo.appezzamentoCampo
                app.validita.inizio = objCampo.validita.inizio
                app.validita.fine = objCampo.validita.fine
            Next

            Aggiorna_ValiditaAppezzamenti_Campo(Id_Budget,
                                                objCampo,
                                                tipoOperazioneCampo,
                                                objParametri_Server,
                                                objParametri_Utenti)
            Aggiorna_ValiditaUtentiXAppezzamenti_Campo(objCampo,
                                                       tipoOperazioneCampo,
                                                       objParametri_Server,
                                                       objParametri_Utenti)
            Aggiorna_ValiditaAppezzamentiXParticelle_Campo(Id_Budget,
                                                           objCampo,
                                                           tipoOperazioneCampo,
                                                           objParametri_Server,
                                                           objParametri_Utenti)
            Aggiorna_ValiditaReg_Impianti_Campo(Id_Budget,
                                                objCampo,
                                                tipoOperazioneCampo,
                                                objParametri_Server,
                                                objParametri_Utenti)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return MessaggioErrore

    End Function

    Private Function Aggiorna_ValiditaAppezzamenti_Campo(ByVal Id_budget As Integer,
            ByRef objCampo As AgronicaCoreModelsSTD.anagrafiche.Campo,
            ByVal tipoOperazioneCampo As AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB,
            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser

        Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.EFCampi.Funzione_Accessoria_Scrivi_Campo()"
        Dim MessaggioErrore As String = String.Empty

        Dim objAppezzamenti_W As New AgronicaCoreBudgetDAL.Budget_Appezzamento_W

        Try

            objAppezzamenti_W.AggiornaValiditaInizio(Id_budget,
                                                     objCampo.primaryKey.centroAziendalePK.partitaIva,
                                                     objCampo.primaryKey.centroAziendalePK.codice,
                                                     objCampo.primaryKey.codice,
                                                     0,
                                                     objCampo.validita.inizio,
                                                     "",
                                                     objParametri_Server)

            objAppezzamenti_W.AggiornaValiditaFine(Id_budget,
                                                     objCampo.primaryKey.centroAziendalePK.partitaIva,
                                                     objCampo.primaryKey.centroAziendalePK.codice,
                                                     objCampo.primaryKey.codice,
                                                     0,
                                                     objCampo.validita.fine,
                                                     "",
                                                     objParametri_Server)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return MessaggioErrore

    End Function

    Private Function Aggiorna_ValiditaUtentiXAppezzamenti_Campo(
            ByVal objCampo As AgronicaCoreModelsSTD.anagrafiche.Campo,
            ByVal tipoOperazioneCampo As AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB,
            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser

        Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.EFCampi.Funzione_Accessoria_Scrivi_Campo()"
        Dim MessaggioErrore As String = String.Empty

        Dim objUtentixAppezzamenti As New AgronicaCoreAnagrafeDAL.UtentixAppezzamenti_W

        Try

            objUtentixAppezzamenti.AggiornaValiditaInizio(objCampo.primaryKey.centroAziendalePK.partitaIva,
                                                          objCampo.primaryKey.centroAziendalePK.codice,
                                                          objCampo.primaryKey.codice,
                                                          0,
                                                          objCampo.validita.inizio,
                                                          "",
                                                          objParametri_Server)

            objUtentixAppezzamenti.AggiornaValiditaFine(objCampo.primaryKey.centroAziendalePK.partitaIva,
                                                        objCampo.primaryKey.centroAziendalePK.codice,
                                                        objCampo.primaryKey.codice,
                                                        0,
                                                        objCampo.validita.fine,
                                                        "",
                                                        objParametri_Server)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return MessaggioErrore

    End Function

    Private Function Aggiorna_ValiditaAppezzamentiXParticelle_Campo(ByVal Id_Budget As Integer,
        ByVal objCampo As AgronicaCoreModelsSTD.anagrafiche.Campo,
        ByVal tipoOperazioneCampo As AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB,
        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser

        Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.EFCampi.Funzione_Accessoria_Scrivi_Campo()"
        Dim MessaggioErrore As String = String.Empty

        Dim objAppezzaxParticelle As New AgronicaCoreBudgetDAL.Budget_AppezzaxParticelle_W

        Try

            objAppezzaxParticelle.AggiornaValiditaInizio(Id_Budget,
                                                         objCampo.primaryKey.centroAziendalePK.partitaIva,
                                                         objCampo.primaryKey.centroAziendalePK.codice,
                                                         objCampo.primaryKey.codice,
                                                         0,
                                                         "",
                                                         "",
                                                         "",
                                                         0, 0, "", objCampo.validita.inizio,
                                                         "",
                                                         objParametri_Server)

            objAppezzaxParticelle.AggiornaValiditaFine(Id_Budget,
                                                        objCampo.primaryKey.centroAziendalePK.partitaIva,
                                                        objCampo.primaryKey.centroAziendalePK.codice,
                                                        objCampo.primaryKey.codice,
                                                        0,
                                                        "", "", "", 0, 0, "", objCampo.validita.fine,
                                                        "",
                                                        objParametri_Server)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return MessaggioErrore

    End Function

    Private Function Aggiorna_ValiditaReg_Impianti_Campo(ByVal Id_Budget As Integer,
            ByVal objCampo As AgronicaCoreModelsSTD.anagrafiche.Campo,
            ByVal tipoOperazioneCampo As AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB,
            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser

        Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.EFCampi.Funzione_Accessoria_Scrivi_Campo()"
        Dim MessaggioErrore As String = String.Empty

        Dim objReg_Impianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Write

        Try

            objReg_Impianti.AggiornaValiditaInizio(objCampo.primaryKey.centroAziendalePK.partitaIva,
                                                   objCampo.primaryKey.centroAziendalePK.codice,
                                                   0,
                                                   objCampo.primaryKey.codice,
                                                   0,
                                                   objCampo.validita.inizio,
                                                   objParametri_Server)

            objReg_Impianti.AggiornaValiditaFine(objCampo.primaryKey.centroAziendalePK.partitaIva,
                                                 objCampo.primaryKey.centroAziendalePK.codice,
                                                 0,
                                                 objCampo.primaryKey.codice,
                                                 0,
                                                 objCampo.validita.fine,
                                                 objParametri_Server)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return MessaggioErrore

    End Function

    Private Function UtentiXCampi_Cancella_Campo(
            ByRef objCampo As AgronicaCoreModelsSTD.anagrafiche.Campo,
            ByVal tipoOperazioneCampo As AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB,
            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser

        Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.EFCampi.Funzione_Accessoria_Scrivi_Campo()"
        Dim MessaggioErrore As String = String.Empty

        Dim objUtentixCampi As New AgronicaCoreAnagrafeDAL.UtentixCampi_Write

        Try

            objUtentixCampi.Cancella(objCampo.primaryKey.centroAziendalePK.partitaIva,
                                     objCampo.primaryKey.centroAziendalePK.codice,
                                     objCampo.primaryKey.codice,
                                     "",
                                     objParametri_Server)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return MessaggioErrore

    End Function

    Private Function ProgrammazioneEntita_Cancella_Campo(
            ByRef objCampo As AgronicaCoreModelsSTD.anagrafiche.Campo,
            ByVal tipoOperazioneCampo As AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB,
            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser

        Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.EFCampi.Funzione_Accessoria_Scrivi_Campo()"
        Dim MessaggioErrore As String = String.Empty

        Dim objPE As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_W

        Try
            objPE.Azzera_Campo_Cod(objCampo.primaryKey.centroAziendalePK.partitaIva,
                                   objCampo.primaryKey.centroAziendalePK.codice,
                                   objCampo.primaryKey.codice,
                                   "",
                                   objParametri_Server)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return MessaggioErrore

    End Function

    Private Function CampiXParticelle_ScriviModificaCancella_Campo(ByRef Id_Budget As Integer,
            ByRef objCampo As AgronicaCoreModelsSTD.anagrafiche.Campo,
            ByVal tipoOperazioneCampo As AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB,
            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
            Optional ByRef Campo As AgronicaCoreEntityFramework_POCO.Budget_Campi = Nothing) As String

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser

        Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_EFCampi.Funzione_Accessoria_Scrivi_Campo()"
        Dim MessaggioErrore As String = String.Empty

        Dim objCampixParticelle As New AgronicaCoreBudgetDAL.Budget_CampixParticelle_W

        Try

            objCampixParticelle.Cancella(Id_Budget,
                                         objCampo.primaryKey.centroAziendalePK.partitaIva,
                                         objCampo.primaryKey.centroAziendalePK.codice,
                                         If(tipoOperazioneCampo = enum_TipoOperazioneDB.Scrittura, Campo.Campo_Cod, objCampo.primaryKey.codice),
                                         "",
                                         "",
                                         "",
                                         0,
                                         0,
                                         "",
                                         "",
                                         objParametri_Server)

            If tipoOperazioneCampo <> enum_TipoOperazioneDB.Cancellazione And objCampo.catastoCampo IsNot Nothing Then
                'TODO Salvo: sistemare scrittura ettari, are, centare
                For Each part In objCampo.catastoCampo
                    Dim ettari = 0
                    Dim are = 0
                    Dim centiare = 0
                    AgronicaCoreDataProvider.Conversioni.EttariAreCentiare_from_Ettari(part.area, ettari, are, centiare)
                    objCampixParticelle.Scrivi(Id_Budget,
                                               objCampo.primaryKey.centroAziendalePK.partitaIva,
                                               objCampo.primaryKey.centroAziendalePK.codice,
                                               If(tipoOperazioneCampo = enum_TipoOperazioneDB.Scrittura, Campo.Campo_Cod, objCampo.primaryKey.codice),
                                               part.particella.primaryKey.Prov,
                                               part.particella.primaryKey.Com,
                                               part.particella.primaryKey.Sezione,
                                               part.particella.primaryKey.Foglio,
                                               part.particella.primaryKey.Numero,
                                               part.particella.primaryKey.Subalterno,
                                               ettari,
                                               are,
                                               centiare,
                                               CDbl(0),
                                               CInt(0),
                                               CInt(0),
                                               CDbl(0),
                                               CInt(0),
                                               CInt(0),
                                               part.area, 'superficie intersezione
                                               objCampo.validita.inizio,
                                               objCampo.validita.fine,
                                               objParametri_Server)
                Next
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return MessaggioErrore

    End Function

    Private Function Validita_CampiXParticelle_Modifica_Campo(ByRef Id_Budget As Integer,
            ByRef objCampo As AgronicaCoreModelsSTD.anagrafiche.Campo,
            ByVal tipoOperazione As AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB,
            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser

        Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim NomeRoutine As String = "AgronicaCoreBudgetDAL.Budget_EFCampi.Funzione_Accessoria_Scrivi_Campo()"
        Dim MessaggioErrore As String = String.Empty

        Dim objCampiXparticelle As New AgronicaCoreBudgetDAL.Budget_CampixParticelle_W

        Try
            objCampiXparticelle.AggiornaValidita(Id_Budget,
                                             objCampo.primaryKey.centroAziendalePK.partitaIva,
                                             objCampo.primaryKey.centroAziendalePK.codice,
                                             objCampo.primaryKey.codice,
                                             objCampo.validita.inizio,
                                             objCampo.validita.fine,
                                             "",
                                             objParametri_Server)
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return MessaggioErrore

    End Function

    Public Shared Function Delete_Campo_Reale_Da_Ribaltamento(Ribaltamento As AgronicaCoreEntityFramework_POCO.Ribaltamento_Campi,
                                                              ByRef objParametri_Server As AgronicaCoreParametri,
                                                              ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                                Optional NoteLog As String = "") As String

        Dim msgErrore As String = ""

        If NoteLog = "" Then
            NoteLog = "Cancellato da Eliminazione Campo Budget (" & Ribaltamento.Budget_Piva & "-" & Ribaltamento.Budget_Sa_Cod & "-" & Ribaltamento.Budget_Campo_Cod & "), Id_Budget: " & Ribaltamento.Budget_Id_Testata
        End If

        Dim campo As New AgronicaCoreModelsSTD.anagrafiche.Campo
        campo.primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Campo.PK(Ribaltamento.Reale_Campo_Cod, New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(Ribaltamento.Reale_Sa_Cod, Ribaltamento.Reale_Piva))
        campo.flag_cancellazione = True


        Try
            Dim objCampoReale_W As New AgronicaCoreAnagrafeBIZ.Campo_W
            objCampoReale_W.Scrivi_Campo_Anagrafica(campo, enum_TipoOperazioneDB.Cancellazione,
                                                    objParametri_Server, objParametri_Utenti,
                                                    NoteLog:=NoteLog)

        Catch ex As Exception
            msgErrore = Gias.ImpossibileCancellareCampoRibaltatoPianoColturaleEffettivo & ": " & ex.Message
        End Try

        Return msgErrore
    End Function

End Class
