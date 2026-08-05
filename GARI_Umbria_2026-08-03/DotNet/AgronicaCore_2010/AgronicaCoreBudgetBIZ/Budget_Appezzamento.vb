Imports System.Data.Entity
Imports System.Linq
Imports System.Transactions
Imports System.Xml
Imports System.Xml.Linq
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreBudgetDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreGisBIZ
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.baseClass
Imports AgronicaCoreModelsSTD.costanti
Imports AgronicaCoreModelsSTD.exceptions
Imports AgronicaCoreVarieDAL
Imports Newtonsoft.Json

Public Class Budget_Appezzamento_R
    Public Function Leggi_Appezzamento_Anagrafica(
                                                 ByVal Id_Budget As Integer,
                                                 ByVal Piva As String,
                                                 ByVal Sa_Cod As Integer,
                                                 ByVal Appezza As Integer,
                                                 ByVal IdReg As Integer,
                                                 ByVal Leggi_Impianti As Boolean,
                                                 ByVal Leggi_Indirizzi As Boolean,
                                                 ByVal Leggi_Catasto As Boolean,
                                                 ByVal data As Date,
                                                 ByVal filtroData As Boolean,
                                                 ByVal Leggi_Distinte As Boolean,
                                                 ByVal Leggi_Cartografia As Boolean,
                                                 ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                                 ByRef objParametri_Server As AgronicaCoreParametri,
                                                 ByRef objParametri_Utenti As AgronicaCoreParametri) As AgronicaCoreModelsSTD.anagrafiche.Appezzamento

        Dim appezzamento As New AgronicaCoreModelsSTD.anagrafiche.Appezzamento(
            New AgronicaCoreModelsSTD.anagrafiche.Appezzamento.PK(Appezza,
                                                                  New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(Sa_Cod, Piva)))


        Dim appInizio As Date = objParametri_Server.FinestraTemporaleInizio
        Dim appFine As Date = objParametri_Server.FinestraTemporaleFine

        If filtroData Then
            objParametri_Server.FinestraTemporaleInizio = data
            objParametri_Server.FinestraTemporaleFine = data
        Else
            objParametri_Server.FinestraTemporaleInizio = CDate(AGRODATAINIZIO)
            objParametri_Server.FinestraTemporaleFine = CDate(AGRODATAFINE)
        End If


        If Id_Budget <> 0 AndAlso Piva <> "" AndAlso Sa_Cod <> 0 AndAlso Appezza <> 0 Then

            Dim objAppezzamenti As New AgronicaCoreBudgetDAL.Budget_Appezzamento_R
            Dim DTAppezzamenti As DataTable


            Dim tipoSelezione As enumSelezioneVariabile = enumSelezioneVariabile.Selezione_TabellaCompleta
            Dim LetturaCFG As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim jSonStaticMapCFG As String = LetturaCFG.Leggi_Valore(0, "GIS_StaticMapCFG", "", "", objParametri_Server)

            If jSonStaticMapCFG <> "" Then
                Dim StaticMapCFG As GeneraMappaStaticaInData = JsonConvert.DeserializeObject(Of GeneraMappaStaticaInData)(jSonStaticMapCFG)
                If StaticMapCFG.StaticMapAttive Then
                    tipoSelezione = enumSelezioneVariabile.Selezione_JoinCompleta
                End If
            End If

            objParametri_Server.FinestraTemporaleInizio = appInizio
            objParametri_Server.FinestraTemporaleFine = appFine

            DTAppezzamenti = objAppezzamenti.Leggi(
                CInt(Id_Budget),
                CStr(Piva),
                CInt(Sa_Cod),
                CInt(Appezza),
                tipoSelezione,
                "",
                "",
                objParametri_Server,
                Leggi_Cartografia:=Leggi_Cartografia
                )

            objParametri_Server.FinestraTemporaleInizio = AGRODATAINIZIO
            objParametri_Server.FinestraTemporaleFine = AGRODATAFINE


            If DTAppezzamenti.Rows.Count > 0 Then

                appezzamento.campoPK = New AgronicaCoreModelsSTD.anagrafiche.Campo.PK(DTAppezzamenti.Rows(0).Item("Campo_Cod"),
                                                                                      New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(Sa_Cod, Piva))

                appezzamento.descrizione = IIf(IsDBNull(DTAppezzamenti.Rows(0).Item("App_Nome")), "", DTAppezzamenti.Rows(0).Item("App_Nome"))
                appezzamento.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(DTAppezzamenti.Rows(0).Item("Validita_Inizio"),
                                                                                                  DTAppezzamenti.Rows(0).Item("Validita_Fine"))
                appezzamento.superficie = DTAppezzamenti.Rows(0).Item("sup_app")

                appezzamento.pendenza = DTAppezzamenti.Rows(0).Item("pende")
                appezzamento.esposizione = New BaseCodeDescrStr(DTAppezzamenti.Rows(0).Item("esposiz"), DTAppezzamenti.Rows(0).Item("esposiz"))
                appezzamento.ubicazione = New BaseCodeDescrStr(DTAppezzamenti.Rows(0).Item("ubicazione"), DTAppezzamenti.Rows(0).Item("ubicazione"))

                appezzamento.supBZ_Riduzione = DTAppezzamenti.Rows(0).Item("SupBZ_Riduzione")
                appezzamento.distBZ_CorpiIdrici = DTAppezzamenti.Rows(0).Item("DistBZ_CorpiIdrici")
                appezzamento.distBZ_AreeResPub = DTAppezzamenti.Rows(0).Item("DistBZ_AreeResPub")
                appezzamento.distBZ_Allevamenti = DTAppezzamenti.Rows(0).Item("DistBZ_Allevamenti")
                appezzamento.distBZ_VegNatNonColt = DTAppezzamenti.Rows(0).Item("DistBZ_VegNatNonColt")

                appezzamento.lat = DTAppezzamenti.Rows(0).Item("x")
                appezzamento.lng = DTAppezzamenti.Rows(0).Item("y")
                appezzamento.altitudine = DTAppezzamenti.Rows(0).Item("zslm")

                If Leggi_Catasto Then
                    appezzamento.catastoAppezzamento = Leggi_AppezzamentoCatasto(Id_Budget, Piva, Sa_Cod, Appezza, objParametri_Server)
                End If

                If tipoSelezione = enumSelezioneVariabile.Selezione_JoinCompleta Then
                    appezzamento.immagineBase64 = DTAppezzamenti.Rows(0).Item("StaticMapBase64String")
                End If

                appezzamento.n_App_Bio = ""
                appezzamento.confini_A_Rischio = ""
                appezzamento.utilizzo_Terreno = New List(Of BaseCodeDescr)
                appezzamento.fine_Impiego_Prod_Non_Conformi = AGRODATAINIZIO

                Dim objMetaschema As New AgronicaCoreMetaSchemaBIZ.ClassiTessitura

                If IsDBNull(DTAppezzamenti.Rows(0).Item("Sabbia")) Then
                    appezzamento.sabbia = Nothing
                Else
                    appezzamento.sabbia = Convert.ToDecimal(DTAppezzamenti.Rows(0).Item("Sabbia"))
                End If

                If IsDBNull(DTAppezzamenti.Rows(0).Item("Limo")) Then
                    appezzamento.limo = Nothing
                Else
                    appezzamento.limo = Convert.ToDecimal(DTAppezzamenti.Rows(0).Item("Limo"))
                End If

                If IsDBNull(DTAppezzamenti.Rows(0).Item("Argilla")) Then
                    appezzamento.argilla = Nothing
                Else
                    appezzamento.argilla = Convert.ToDecimal(DTAppezzamenti.Rows(0).Item("Argilla"))
                End If

                Dim id_classetessitura As Integer = If(IsDBNull(DTAppezzamenti.Rows(0).Item("CLAS")) OrElse DTAppezzamenti.Rows(0).Item("CLAS") = "", 0, CInt(DTAppezzamenti.Rows(0).Item("CLAS")))
                appezzamento.classeTessitura = New ClasseTessitura(id_classetessitura) With {
                    .descrizione = objMetaschema.ClasseTessituraDes_from_ClasseTessituraCod(id_classetessitura, 0, objParametri_Server)
                }

                Dim DTCodici As DataTable

                Dim objCodici As New AgronicaCoreBudgetDAL.Budget_Appezzamento_Codici_R

                Dim Id_Cod As Integer = 0
                Dim Val_Cod As String = ""

                DTCodici = objCodici.Leggi(
                    Id_Budget,
                    Piva,
                    Sa_Cod,
                    Appezza,
                    Id_Cod,
                    Val_Cod,
                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                    "(id_cod < 2000 OR id_cod >= 3000)",
                    "",
                    objParametri_Server
                    )

                Dim objSpecie_R As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R

                appezzamento.metodo_Produzione = New AgronicaCoreModelsSTD.metaschema.MetodoProduzione(enum_MetodoProduzione.Integrato) With {.descrizione = Gias.Integrato}

                Dim altriCodici As New List(Of AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori)

                If DTCodici.Rows.Count > 0 Then

                    For i = 0 To DTCodici.Rows.Count - 1

                        If (Not IsDBNull(DTCodici.Rows(i).Item("val_cod"))) Then

                            Id_Cod = DTCodici.Rows(i).Item("id_cod")
                            Val_Cod = DTCodici.Rows(i).Item("val_cod")

                            Select Case DTCodici.Rows(i).Item("id_cod")

                                Case enum_CodiciAnagrafe.MetodoDiProduzione
                                    Dim objMetodoProduzione As New AgronicaCoreMetaSchemaBIZ.MetodoProduzione
                                    Select Case DTCodici.Rows(i).Item("val_cod")
                                        Case enum_MetodoProduzione.Integrato, enum_MetodoProduzione.InConversione, enum_MetodoProduzione.Biologico
                                            appezzamento.metodo_Produzione = objMetodoProduzione.Leggi(DTCodici.Rows(i).Item("val_cod"), objParametri_Server)
                                        Case Else
                                            appezzamento.metodo_Produzione = objMetodoProduzione.Leggi(enum_MetodoProduzione.Integrato, objParametri_Server)
                                    End Select

                                Case enum_CodiciAnagrafe.DataFineImpiegoPNC
                                    If IsDate(DTCodici.Rows(i).Item("val_cod")) Then
                                        appezzamento.fine_Impiego_Prod_Non_Conformi = CDate(DTCodici.Rows(i).Item("val_cod"))
                                    End If

                                Case enum_CodiciAnagrafe.Appezzamento_ConfiniRischio
                                    appezzamento.confini_A_Rischio = DTCodici.Rows(i).Item("val_cod")

                                Case enum_CodiciAnagrafe.OrientamentoProduttivo
                                    If Not IsDBNull(DTCodici.Rows(i).Item("val_cod")) Then
                                        appezzamento.utilizzo_Terreno = Leggi_Orientamento_Produttivo(DTCodici.Rows(i).Item("val_cod"), objParametri_Server)
                                    End If

                                Case enum_CodiciAnagrafe.Terreno_Inutilizzato
                                    appezzamento.terrenoInutilizzato = CInt(DTCodici.Rows(i).Item("val_cod"))

                                Case enum_CodiciAnagrafe.Terreno_Degradato
                                    appezzamento.terrenoDegradato = CInt(DTCodici.Rows(i).Item("val_cod"))

                                Case enum_CodiciAnagrafe.Low_ILUC
                                    appezzamento.lowILUC = CInt(DTCodici.Rows(i).Item("val_cod"))

                                Case enum_CodiciAnagrafe.Codice_Appezza_Biologico
                                    appezzamento.n_App_Bio = DTCodici.Rows(i).Item("val_cod")

                                Case enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento
                                    appezzamento.rif_Appezzamento = DTCodici.Rows(i).Item("val_cod")

                                Case enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_1

                                    Dim specie_Cod As Integer = 0
                                    If (IsNumeric(DTCodici.Rows(i).Item("val_cod"))) Then
                                        specie_Cod = DTCodici.Rows(i).Item("val_cod")
                                    Else
                                        specie_Cod = CStr(DTCodici.Rows(i).Item("val_cod")).Split("|")(0)
                                    End If

                                    Dim dtSpecie = objSpecie_R.Leggi(
                                        specie_Cod,
                                        0,
                                        "",
                                        "",
                                        enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                        "",
                                        "",
                                        objParametri_Server
                                        )

                                    If dtSpecie.Rows.Count > 0 Then
                                        appezzamento.coltura_Precedente_1_Anno = New AgronicaCoreModelsSTD.metaschema.utilizzi.Specie(
                                            dtSpecie.Rows(0)("Veg_Cod")) With {
                                                .descrizione = dtSpecie.Rows(0)("Veg_Des")
                                            }
                                    End If

                                Case enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_2

                                    Dim specie_Cod As Integer = 0
                                    If (IsNumeric(DTCodici.Rows(i).Item("val_cod"))) Then
                                        specie_Cod = DTCodici.Rows(i).Item("val_cod")
                                    Else
                                        specie_Cod = CStr(DTCodici.Rows(i).Item("val_cod")).Split("|")(0)
                                    End If

                                    Dim dtSpecie = objSpecie_R.Leggi(specie_Cod,
                                                      0, "", "",
                                                      enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                      "", "", objParametri_Server)
                                    If dtSpecie.Rows.Count > 0 Then
                                        appezzamento.coltura_Precedente_2_Anno = New AgronicaCoreModelsSTD.metaschema.utilizzi.Specie(
                                            dtSpecie.Rows(0)("Veg_Cod")) With {
                                                .descrizione = dtSpecie.Rows(0)("Veg_Des")
                                            }
                                    End If

                                Case enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_3

                                    Dim specie_Cod As Integer = 0
                                    If (IsNumeric(DTCodici.Rows(i).Item("val_cod"))) Then
                                        specie_Cod = DTCodici.Rows(i).Item("val_cod")
                                    Else
                                        specie_Cod = CStr(DTCodici.Rows(i).Item("val_cod")).Split("|")(0)
                                    End If

                                    Dim dtSpecie = objSpecie_R.Leggi(specie_Cod,
                                                      0, "", "",
                                                      enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                      "", "", objParametri_Server)
                                    If dtSpecie.Rows.Count > 0 Then
                                        appezzamento.coltura_Precedente_3_Anno = New AgronicaCoreModelsSTD.metaschema.utilizzi.Specie(
                                            dtSpecie.Rows(0)("Veg_Cod")) With {
                                                .descrizione = dtSpecie.Rows(0)("Veg_Des")
                                            }
                                    End If

                                Case enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_4

                                    Dim specie_Cod As Integer = 0
                                    If (IsNumeric(DTCodici.Rows(i).Item("val_cod"))) Then
                                        specie_Cod = DTCodici.Rows(i).Item("val_cod")
                                    Else
                                        specie_Cod = CStr(DTCodici.Rows(i).Item("val_cod")).Split("|")(0)
                                    End If

                                    Dim dtSpecie = objSpecie_R.Leggi(specie_Cod,
                                                      0, "", "",
                                                      enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                      "", "", objParametri_Server)
                                    If dtSpecie.Rows.Count > 0 Then
                                        appezzamento.coltura_Precedente_4_Anno = New AgronicaCoreModelsSTD.metaschema.utilizzi.Specie(
                                            dtSpecie.Rows(0)("Veg_Cod")) With {
                                                .descrizione = dtSpecie.Rows(0)("Veg_Des")
                                            }
                                    End If

                                Case enum_CodiciAnagrafe.Isola
                                    appezzamento.isola = DTCodici.Rows(i).Item("val_cod")

                                Case enum_CodiciAnagrafe.TitoloPossesso
                                    'DO NOTHING

                                Case Else
                                    Dim codice = New AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori
                                    codice.valore = DTCodici.Rows(i).Item("val_cod")
                                    codice.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(DTCodici.Rows(i).Item("Validita_Inizio"), DTCodici.Rows(i).Item("Validita_Fine"))
                                    Dim cod = New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe
                                    cod.codice = DTCodici.Rows(i).Item("id_Cod")
                                    cod.descrizione = DTCodici.Rows(i).Item("descrizione")
                                    codice.codiceAnagrafe = cod
                                    altriCodici.Add(codice)

                            End Select
                        End If
                    Next
                End If

                appezzamento.codici = altriCodici

                If Leggi_Cartografia AndAlso DTAppezzamenti.Columns.Contains("cartografia") Then
                    appezzamento.cartografia = DTAppezzamenti.Rows(0)("cartografia")
                End If

                If Leggi_Impianti Then
                    Dim objImpianti_R As New AgronicaCoreBudgetBIZ.Budget_Reg_Impianto_R
                    appezzamento.impianti = objImpianti_R.Leggi_Impianti_Anagrafica(
                        Id_Budget,
                        Piva,
                        Sa_Cod,
                        Appezza,
                        IdReg,
                        Leggi_Distinte,
                        data,
                        filtroData,
                        Leggi_Cartografia,
                        objParametri_Super_Server,
                        objParametri_Server,
                        objParametri_Utenti
                        )
                End If

                If Leggi_Indirizzi Then
                    Dim objIndirizzi As New AgronicaCoreAnagrafeBIZ.Indirizzi_R
                    appezzamento.indirizzi = objIndirizzi.Leggi_Indirizzi_Associati_Appezzamento(Piva, Sa_Cod, Appezza, objParametri_Server, Id_Budget)
                End If

            End If
        Else
            appezzamento = Nothing
        End If

        objParametri_Server.FinestraTemporaleInizio = CDate(appInizio)
        objParametri_Server.FinestraTemporaleFine = CDate(appFine)

        Return appezzamento
    End Function

    Public Function Leggi_AppezzamentoCatasto(ByVal Id_Budget As Integer,
                                              ByVal Piva As String,
                                        ByVal Sa_Cod As Integer,
                                        ByVal Appezza As Integer,
                                        ByRef objParametri_Server As AgronicaCoreParametri) As List(Of AgronicaCoreModelsSTD.anagrafiche.CatastoAppezzamento)

        Dim catastoApp As New List(Of AgronicaCoreModelsSTD.anagrafiche.CatastoAppezzamento)
        Dim objAppxPart As New AgronicaCoreBudgetDAL.Budget_AppezzaxParticelle_R
        Dim DtAppxPart As DataTable

        DtAppxPart = objAppxPart.LeggiParticelle_Da_Appezzamento(Id_Budget,
                                                                 Piva,
                                                                 Sa_Cod,
                                                                 Appezza,
                                                                 AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                 "",
                                                                 "",
                                                                 objParametri_Server)

        If DtAppxPart.Rows.Count > 0 Then

            For Each r As DataRow In DtAppxPart.Rows

                Dim prov = r("Prov")
                Dim com = r("Com")
                Dim sezione = r("Sezione")
                Dim foglio = r("Foglio")
                Dim numero = r("Numero")
                Dim subalterno = r("subalterno")

                If sezione = "0" Then
                    sezione = ""
                End If

                If subalterno = "0" Then
                    subalterno = ""
                End If

                Dim Catasto_Appezzamento As New AgronicaCoreModelsSTD.anagrafiche.CatastoAppezzamento
                Dim particella = New AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastali.PK(
                    prov, com, sezione, foglio, numero, subalterno
                )

                Catasto_Appezzamento.area = r("AREA")
                Catasto_Appezzamento.particella = particella

                catastoApp.Add(Catasto_Appezzamento)
            Next


        End If
        Return catastoApp
    End Function

    Public Function Leggi_Orientamento_Produttivo(valoriStr As String, objParametriServer As AgronicaCoreParametri) As List(Of BaseCodeDescr)
        Dim list As New List(Of BaseCodeDescr)
        Dim objOrientamentroProduttivo As New AgronicaCoreMetaSchemaDAL.BIO_Dati_OrientamentoProduttivo_R
        If valoriStr <> "" Then
            Dim ValoriArr = valoriStr.Split(",")
            For Each valore In ValoriArr
                valore = valore.Trim
                If IsNumeric(valore) Then
                    Dim dt = objOrientamentroProduttivo.Leggi(CInt(valore), enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametriServer)
                    If dt.Rows.Count > 0 Then
                        list.Add(New BaseCodeDescr() With {
                            .codice = CInt(valore),
                            .descrizione = dt.Rows(0)("Orientamento_Des")
                        })
                    End If
                End If
            Next
        End If

        Return list
    End Function

    Public Function LeggiMaxData(Id_Budget As Integer, Piva As String, ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri) As DateTime
        Using GiasContext As Gias_DeveloperServer_Entities = Gias_EF_Utility.CreateGiasContextConnection(objParametri_Server.StringaConnessione)

            Dim objAppezzamenti As New AgronicaCoreBudgetDAL.Budget_Appezzamento_R

            Dim DateList As New List(Of DateTime)

            Dim maxDataAppezza As DateTime = objAppezzamenti.Leggi_Max_DataModifica(Id_Budget, Piva,
                                                                             objParametri_Server,
                                                                             objParametri_Utenti)

            Dim objImpianti As New AgronicaCoreBudgetDAL.Budget_Reg_Impianti_Read

            Dim maxDataImpianti As DateTime = objImpianti.Leggi_Max_DataModifica(Id_Budget, Piva,
                                                                             objParametri_Server,
                                                                             objParametri_Utenti)

            Dim objEsercizi As New AgronicaCoreBudgetDAL.Budget_Impresa_Progetti_R

            Dim maxDataEsercizi As DateTime = objEsercizi.Leggi_Max_DataModifica(Id_Budget, Piva,
                                                                             objParametri_Server,
                                                                             objParametri_Utenti)

            Dim listTipo As New List(Of String) From {"Appezzamento", "Reg_Impianti", "Imprese_Progetti"}

            Dim maxLog = AGRODATAINIZIO
            Dim maxLogEls = (From al In GiasContext.Agronica_Log_Anagrafe
                             Where al.Param1 = Piva AndAlso listTipo.Contains(al.Tipo)
                             Select al.Data_Ora_RegistrazioneLog).ToList()
            If maxLogEls.Count > 0 Then
                maxLog = maxLogEls.Max()
            End If

            DateList.Add(maxDataAppezza)
            DateList.Add(maxLog)
            DateList.Add(maxDataImpianti)
            DateList.Add(maxDataEsercizi)

            Return DateList.Max()

        End Using
    End Function

End Class

Public Class Budget_Appezzamento_W

    Public Function Appezzamento_ScriviModifica(ByRef Id_Budget As Integer,
                                                ByRef DatiAppezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                                ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                    Optional ByVal OpenNewTransaction As Boolean = True,
                                                    Optional ByRef listRibaltamento_Appezzamento As List(Of Ribaltamento_Appezzamento) = Nothing
                                                ) As Boolean
        Dim nomeRoutine As String = "AgronicaCoreBudgetBIZ.Appezzamento_W.Appezzamento_ScriviModifica()"
        Dim messaggioErrore As String = ""

        'Dim username As String = If(objParametri_Utenti.UtenteUsername <> "", objParametri_Utenti.UtenteUsername, objParametri_Server.UsernameOperazione)
        Dim ret = False


        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri_Server.StringaConnessione)
            bCloseContext = True
        End If

        If OpenNewTransaction Then
            Dim scopeOption As New TransactionScopeOption
            Dim transactionOptions As New TransactionOptions
            transactionOptions.IsolationLevel = IsolationLevel.ReadUncommitted
            scope = New TransactionScope(scopeOption, transactionOptions)
        End If

        Try

            If Not DatiAppezzamento.flag_cancellazione Then

                Internal_Appezzamento_ScriviModifica(Id_Budget, DatiAppezzamento, objParametri_Server, objParametri_Utenti, GiasContext)

            Else
                Dim idbudget = Id_Budget
                Dim Piva = DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva
                Dim Sa_Cod = DatiAppezzamento.primaryKey.centroAziendalePK.codice
                Dim Appezza = DatiAppezzamento.primaryKey.codice

                If idbudget = 0 Then
                    Throw New GiasException("Id_Budget non impostato correttamente")
                End If

                If Piva = "" Then
                    Throw New GiasException("Partita Iva non impostata correttamente")
                End If

                If Sa_Cod = 0 Then
                    Throw New GiasException("Sa_Cod non impostato correttamente")
                End If

                If Appezza = 0 Then
                    Throw New GiasException("Appezza non impostato correttamente")
                End If

                Dim appezzamentoDB = (From a In GiasContext.Budget_Appezzamento Where a.Id_Budget = idbudget AndAlso a.PIVA = Piva AndAlso a.SA_COD = Sa_Cod AndAlso a.APPEZZA = Appezza).FirstOrDefault()
                If appezzamentoDB IsNot Nothing Then

                    Dim appezzamentixIndirizziDB = (From a In GiasContext.Budget_AppezzamentixIndirizzi Where a.Id_Budget = idbudget AndAlso a.PIVA = Piva AndAlso a.sa_cod = Sa_Cod AndAlso a.appezza = Appezza).ToList
                    Dim appezzamentiXParticelleDB = (From a In GiasContext.Budget_AppezzamentiXParticelle Where a.Id_Budget = idbudget AndAlso a.PIVA = Piva AndAlso a.SA_COD = Sa_Cod AndAlso a.APPEZZA = Appezza).ToList
                    Dim appezzamentiXParticellexMacrousiDB = (From a In GiasContext.AppezzamentiXParticellexMacrousi Where a.Piva = Piva AndAlso a.Sa_cod = Sa_Cod AndAlso a.Appezza = Appezza).ToList
                    Dim appezzamentiXParticellexMacrousixUtilizzoDB = (From a In GiasContext.AppezzamentiXParticellexMacrousixUtilizzo Where a.Piva = Piva AndAlso a.Sa_cod = Sa_Cod AndAlso a.Appezza = Appezza).ToList
                    Dim appezzamentixRubricaDB = (From a In GiasContext.AppezzamentixRubrica Where a.PIVA = Piva AndAlso a.sa_cod = Sa_Cod AndAlso a.appezza = Appezza).ToList
                    Dim appezzamento_CodiciDB = (From a In GiasContext.Budget_Appezzamento_Codici Where a.Id_Budget = idbudget AndAlso a.PIVA = Piva AndAlso a.sa_cod = Sa_Cod AndAlso a.appezza = Appezza).ToList
                    'Dim appezzamento_StoricoDB = (From a In GiasContext.Appezzamento_Storico Where a.Piva = Piva AndAlso a.Sa_cod = Sa_Cod AndAlso a.Appezza = Appezza).ToList
                    Dim utentiXAppezzamentiDB = (From a In GiasContext.UtentiXAppezzamenti Where a.PIVA = Piva AndAlso a.SA_COD = Sa_Cod AndAlso a.Appezza = Appezza).ToList

                    If appezzamentoDB.Blk_Flag = -1 Then
                        Throw New GiasException(String.Format(Gias.ErroreEliminazioneImpiantoBloccato, appezzamentoDB.APP_NOME))
                    End If

                    Dim impiantiDaEliminare = (From i In GiasContext.Budget_Reg_Impianti Where i.Id_Budget = idbudget AndAlso i.PIVA = Piva AndAlso i.SA_COD = Sa_Cod AndAlso i.APPEZZA = Appezza).ToList
                    Dim objImpianti_R As New AgronicaCoreBudgetBIZ.Budget_Reg_Impianto_R
                    For Each impiantoDaEliminare In impiantiDaEliminare
                        Dim id_Reg = impiantoDaEliminare.ID_REG
                        Dim DatiImpianto As AgronicaCoreModelsSTD.anagrafiche.Impianto

                        If DatiAppezzamento.impianti IsNot Nothing Then
                            DatiImpianto = (From ia In DatiAppezzamento.impianti Where ia.primaryKey.codice = id_Reg).FirstOrDefault
                        Else
                            DatiImpianto = objImpianti_R.Leggi_Impianto_Anagrafica(impiantoDaEliminare.Id_Budget,
                                                                                    impiantoDaEliminare.PIVA,
                                                                                    impiantoDaEliminare.SA_COD,
                                                                                    impiantoDaEliminare.APPEZZA,
                                                                                    impiantoDaEliminare.ID_REG,
                                                                                    True,
                                                                                    False,
                                                                                    AGRODATAINIZIO,
                                                                                    False,
                                                                                    False,
                                                                                    objParametri_Server,
                                                                                    objParametri_Server,
                                                                                    objParametri_Utenti)
                        End If

                        Dim descrizione_imp As String = appezzamentoDB.APP_NOME & " [dal " & impiantoDaEliminare.Validita_Inizio & " al " & impiantoDaEliminare.Validita_Fine & "]"

                        Dim objControlloPrenotazioni As New AgronicaCoreBudgetBIZ.Budget_Reg_Impianto_R
                        If objControlloPrenotazioni.controllo_MovimentiProgrammazionexEliminazione(Id_Budget, Piva, Sa_Cod, Appezza, id_Reg, objParametri_Server) Then
                            Dim msgErrore = Gias.ImpossibileEliminareImpiantoEsistonoRichiesteMaterialeVivaisticoAssociate

                            Throw New GiasException(msgErrore)
                        End If

                        eliminaImpianto(impiantoDaEliminare, DatiImpianto, objParametri_Server, objParametri_Utenti, GiasContext)
                    Next

                    GiasContext.Budget_Appezzamento.Remove(appezzamentoDB)
                    GiasContext.Budget_AppezzamentixIndirizzi.RemoveRange(appezzamentixIndirizziDB)
                    GiasContext.Budget_AppezzamentiXParticelle.RemoveRange(appezzamentiXParticelleDB)
                    GiasContext.AppezzamentiXParticellexMacrousi.RemoveRange(appezzamentiXParticellexMacrousiDB)
                    GiasContext.AppezzamentiXParticellexMacrousixUtilizzo.RemoveRange(appezzamentiXParticellexMacrousixUtilizzoDB)
                    GiasContext.AppezzamentixRubrica.RemoveRange(appezzamentixRubricaDB)
                    GiasContext.Budget_Appezzamento_Codici.RemoveRange(appezzamento_CodiciDB)
                    'GiasContext.Appezzamento_Storico.RemoveRange(appezzamento_StoricoDB)
                    GiasContext.UtentiXAppezzamenti.RemoveRange(utentiXAppezzamentiDB)

                    Dim tzh As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
                    Dim DatiAppezzamentoStr = JsonConvert.SerializeObject(DatiAppezzamento, tzh)

                    'Scrittura tabella Agronica_Log_Anagrafe
                    Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
                    Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(
                                                                            enum_TipoEntita_Des.Appezza,
                                                                            CStr(Piva), CStr(Sa_Cod),
                                                                            CStr(Appezza), Nothing,
                                                                            Nothing, Nothing,
                                                                            TipiEnumerativi.enum_TipoOperazioneDB.Cancellazione,
                                                                            objParametri_Server, enum_Id_Servizio.GiasOnline,
                                                                            "", DatiAppezzamentoStr,
                                                                            Id_Budget:=Id_Budget)
                    GiasContext.Agronica_Log_Anagrafe.Add(log)
                    GiasContext.SaveChanges()

                    '------------------------------
                    '   PULIZIA TABELLE RIBALTAMENTO
                    '------------------------------
                    Dim Ribaltamento_Appezzamento = (From rib In GiasContext.Ribaltamento_Appezzamento
                                                     Where rib.Budget_Piva = Piva AndAlso
                                                         rib.Budget_Sa_Cod = Sa_Cod AndAlso
                                                         rib.Budget_Appezza = Appezza).FirstOrDefault()
                    If Ribaltamento_Appezzamento IsNot Nothing Then
                        listRibaltamento_Appezzamento.Add(Ribaltamento_Appezzamento)
                        AgronicaCoreAnagrafeBIZ.RibaltamentoAnagraficheColturali_W.Clean_Tabelle_Ribaltamento_daDeleteElemento(Id_Budget, Piva, Sa_Cod, Appezza, GiasContext)
                    End If
                End If
            End If

            If OpenNewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
            ret = True
        Catch ex As GiasException

            If scope IsNot Nothing Then
                scope.Dispose()
            End If

            messaggioErrore = ex.Message
            Throw ex
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

        Return ret
    End Function

    Private Sub Internal_Appezzamento_ScriviModifica(ByRef Id_Budget As Integer,
                                                     ByRef DatiAppezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                                     ByRef objParametri_Server As AgronicaCoreParametri,
                                                     ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                     ByRef GiasContext As Gias_DeveloperServer_Entities)
        Dim appezzamento As AgronicaCoreEntityFramework_POCO.Budget_Appezzamento = Nothing

        Dim username As String = objParametri_Server.UsernameOperazione

        Dim EntitaCodxImg As New List(Of Integer)
        Dim objImpostazioni_Utenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        Dim DTRiferimentoAppezzamento = objImpostazioni_Utenti.Leggi(enum_Impostazioni_Utenti.Codice_Univoco_Appezzamento, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

        If DTRiferimentoAppezzamento.Rows.Count > 0 AndAlso DTRiferimentoAppezzamento.Rows(0)("Impostazione_Valore_1").trim = "1" Then
            If Not String.IsNullOrEmpty(DatiAppezzamento.rif_Appezzamento) Then
                Dim objAppezzamentoCodici As New AgronicaCoreBudgetDAL.Budget_Appezzamento_Codici_R
                Dim filtro As String = "(Budget_Appezzamento_Codici.Sa_Cod <> " & DatiAppezzamento.primaryKey.centroAziendalePK.codice & " OR Budget_Appezzamento_Codici.Appezza <> " & DatiAppezzamento.primaryKey.codice & ")"
                Dim dtCodAppezzamento = objAppezzamentoCodici.Leggi(Id_Budget, DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva, 0, 0, enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento, DatiAppezzamento.rif_Appezzamento, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, filtro, "", objParametri_Server)
                If dtCodAppezzamento.Rows.Count > 0 Then
                    Throw New GiasException(Gias.RiferimentoAppezzamentoGiaUtilizzato)
                End If
            End If
        End If

        Dim tipoOperazioneAppezzamento As enum_TipoOperazioneDB = enum_TipoOperazioneDB.Lettura

        If DatiAppezzamento.primaryKey.codice = 0 Then
            tipoOperazioneAppezzamento = enum_TipoOperazioneDB.Scrittura
            appezzamento = Internal_Scrivi_Appezzamento(Id_Budget,
                                                        DatiAppezzamento,
                                                        objParametri_Server,
                                                        objParametri_Utenti,
                                                        username,
                                                        GiasContext,
                                                        False
                                                       )

            If appezzamento IsNot Nothing Then
                DatiAppezzamento.primaryKey.codice = appezzamento.APPEZZA
                DatiAppezzamento.primaryKey.centroAziendalePK.codice = appezzamento.SA_COD
            End If

        Else
            tipoOperazioneAppezzamento = enum_TipoOperazioneDB.Modifica
            appezzamento = Internal_Modifica_Appezzamento(Id_Budget,
                                                          DatiAppezzamento,
                                                          objParametri_Server,
                                                          username,
                                                          GiasContext,
                                                          False
                                                         )
        End If

        If appezzamento IsNot Nothing Then
            Internal_ScriviModificaElimina_AppezzamentoCodici(Id_Budget,
                                                              DatiAppezzamento,
                                                              objParametri_Server,
                                                              username,
                                                              GiasContext,
                                                              False
                                                             )

            ''scrivi/modifica indirizzi + apezzamentoxindirizzi
            If (DatiAppezzamento.indirizzi IsNot Nothing) Then
                Internal_Aggiorna_IndirizziAppezzamento(appezzamento, objParametri_Server, DatiAppezzamento.indirizzi, username, GiasContext, False)
            End If

            If (DatiAppezzamento.catastoAppezzamento IsNot Nothing) Then
                Internal_Scrivi_CatastoAppezzamento(Id_Budget, DatiAppezzamento, objParametri_Server, username, GiasContext, False)
            End If

            If DatiAppezzamento.campoPK Is Nothing Then
                DatiAppezzamento.campoPK = New AgronicaCoreModelsSTD.anagrafiche.Campo.PK(0, DatiAppezzamento.primaryKey.centroAziendalePK)
            End If

            If DatiAppezzamento.cartografia IsNot Nothing AndAlso DatiAppezzamento.cartografia <> "" Then
                Internal_ScriviModifica_GIS_Entita_ElementiGrafici_Appezzamento(DatiAppezzamento,
                                                                                objParametri_Server,
                                                                                username,
                                                                                GiasContext,
                                                                                False
                                                                               )
            End If
        End If


        If (DatiAppezzamento.impianti IsNot Nothing) Then
            '-----------------------------------------
            ' CANCELLAZIONE IMPIANTI DA CONTROLLO DATE
            '-----------------------------------------
            DatiAppezzamento = eliminaImpianti(Id_Budget, DatiAppezzamento, GiasContext, objParametri_Server, objParametri_Utenti)

            '' reg_impianti+reg_impianti_codici
            For Each imp As AgronicaCoreModelsSTD.anagrafiche.Impianto In DatiAppezzamento.impianti

                'CONTROLLO SALVATAGGIO CODICE IMPIANTO
                Dim DTRiferimentoImpianto = objImpostazioni_Utenti.Leggi(enum_Impostazioni_Utenti.Codice_Univoco_Impianto, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

                If DTRiferimentoImpianto.Rows.Count > 0 AndAlso DTRiferimentoImpianto.Rows(0)("Impostazione_Valore_1").trim = "1" Then
                    If Not String.IsNullOrEmpty(imp.codiceImpianto) Then
                        Dim objImpiantoCodici As New AgronicaCoreBudgetDAL.Budget_Reg_Impianti_Codici_R
                        Dim filtro As String = "(Budget_Reg_Impianti_Codici.Id_Budget <> " & Id_Budget &
                                    " OR Budget_Reg_Impianti_Codici.Sa_Cod <> " & imp.primaryKey.appezzamentoPK.centroAziendalePK.codice &
                                    " OR Budget_Reg_Impianti_Codici.Appezza <> " & imp.primaryKey.appezzamentoPK.codice &
                                    " OR Budget_Reg_Impianti_Codici.ID_Reg <> " & imp.primaryKey.codice &
                                    ") AND Progetto_Cod = 0 "
                        Dim dtCodImpianto = objImpiantoCodici.Leggi(Id_Budget,
                                                                            imp.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                            0, 0, 0, "",
                                                                            enum_CodiciAnagrafe.Codice_Impianto,
                                                                            imp.codiceImpianto,
                                                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                            filtro, "", objParametri_Server)
                        If dtCodImpianto.Rows.Count > 0 Then
                            Throw New GiasException(Gias.CodiceImpiantoUtilizzato)
                        End If
                    End If
                End If

                Dim tipoOperazioneImpianto As enum_TipoOperazioneDB = enum_TipoOperazioneDB.Lettura
                Dim impianto As AgronicaCoreEntityFramework_POCO.Budget_Reg_Impianti = Nothing
                If imp.primaryKey.codice = 0 Then

                    tipoOperazioneImpianto = enum_TipoOperazioneDB.Scrittura
                    imp.primaryKey.appezzamentoPK = DatiAppezzamento.primaryKey
                    impianto = Internal_Scrivi_Impianti_da_Appezzamento(Id_Budget,
                                                                                imp,
                                                                                objParametri_Server,
                                                                                objParametri_Utenti,
                                                                                username,
                                                                                GiasContext,
                                                                                False
                                                                                )
                    If impianto IsNot Nothing Then
                        imp.primaryKey.codice = impianto.ID_REG
                    End If
                Else
                    tipoOperazioneImpianto = enum_TipoOperazioneDB.Modifica
                    impianto = Internal_Modifica_Impianti_da_Appezzamento(Id_Budget,
                                                                                  imp,
                                                                                  objParametri_Server,
                                                                                  username,
                                                                                  GiasContext,
                                                                                  False
                                                                                  )
                End If

                If impianto IsNot Nothing Then
                    Internal_ScriviModificaElimina_ImpiantiCodici_da_Appezzamento(Id_Budget, imp, objParametri_Server, username, GiasContext, False)
                    If imp.cartografia IsNot Nothing AndAlso imp.cartografia <> "" Then
                        Internal_ScriviModifica_GIS_Entita_ElementiGrafici_ImpiantoAppezzamento(imp,
                                                                                                objParametri_Server,
                                                                                                username,
                                                                                                GiasContext,
                                                                                                False
                                                                                                )
                    End If

                    If imp.macchineIrrigazione IsNot Nothing Then
                        Budget_EFReg_Impianti.ScriviModificaEliminaMacchinaxImpianto(tipoOperazioneImpianto,
                                Id_Budget,
                                imp.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                imp.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                imp.primaryKey.appezzamentoPK.codice,
                                imp.primaryKey.codice,
                                imp.macchineIrrigazione,
                                username,
                                objParametri_Server,
                                GiasContext,
                                False,
                                False)
                    End If
                End If

                'If Not IsNothing(imp.utilizzoTerreno) AndAlso imp.utilizzoTerreno.classType = "Varieta" Then

                '    'CREAZIONE SEMENTI E TRASFORMATI VEGETALI DA VARIETA

                '    Dim cultivar = CType(imp.utilizzoTerreno, AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta)
                '    Dim objGias As New AgronicaCoreAnagrafeBIZ.Importa_GIAS

                '    objGias.Crea_MateriaPrima_Specie_Varieta_Regolamento(
                '        cultivar,
                '        2,
                '        objParametri_Server,
                '        objParametri_Utenti,
                '        True,
                '        True)

                'End If



                If (imp.esercizi IsNot Nothing) Then

                    '-----------------------------------------
                    ' CANCELLAZIONE ESERCIZI DA CONTROLLO DATE
                    '-----------------------------------------
                    imp = eliminaEsercizi(Id_Budget, imp, objParametri_Server, objParametri_Utenti, GiasContext)

                    For Each ese As AgronicaCoreModelsSTD.anagrafiche.Esercizio In imp.esercizi
                        ese.impiantoPK = imp.primaryKey
                        Dim esercizio As AgronicaCoreEntityFramework_POCO.Budget_Imprese_Progetti = Nothing
                        Dim DTRiferimentoProgetto = objImpostazioni_Utenti.Leggi(enum_Impostazioni_Utenti.Codice_Univoco_Progetto, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

                        If DTRiferimentoProgetto.Rows.Count > 0 AndAlso DTRiferimentoProgetto.Rows(0)("Impostazione_Valore_1").trim = "1" Then
                            If Not String.IsNullOrEmpty(ese.lotto) Then
                                Dim objImprese_Progetti As New AgronicaCoreBudgetDAL.Budget_Impresa_Progetti_R
                                Dim filtro As String = "(Budget_Imprese_Progetti.Sa_Cod <> " & imp.primaryKey.appezzamentoPK.centroAziendalePK.codice &
                                            " OR Budget_Imprese_Progetti.Appezza <> " & imp.primaryKey.appezzamentoPK.codice &
                                            " OR Budget_Imprese_Progetti.ID_Reg <> " & imp.primaryKey.codice &
                                            " OR Budget_Imprese_Progetti.Progetto_Cod <> " & ese.codice &
                                            ") AND Progetto_Nome = '" & ese.lotto & "' "
                                Dim dtCodImpianto = objImprese_Progetti.Leggi(Id_Budget,
                                                                                      imp.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                                      0, "", 0, 0, 0, 0, 0, 0,
                                                                                      enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                      filtro, "", objParametri_Server)
                                If dtCodImpianto.Rows.Count > 0 Then
                                    Throw New GiasException("Lotto già utilizzato")
                                End If

                            End If
                        End If

                        If ese.codice = 0 Then
                            'creazione esercizio
                            esercizio = Internal_Scrivi_Esercizi_da_Impianto_Appezzamento(Id_Budget,
                                                                                                  ese,
                                                                                                  objParametri_Server,
                                                                                                  username,
                                                                                                  GiasContext,
                                                                                                  False)
                            If esercizio IsNot Nothing Then
                                ese.codice = esercizio.Progetto_Cod
                            End If
                        Else
                            'modifica esercizio
                            esercizio = Internal_Modifica_Esercizi_da_Impianto_Appezzamento(Id_Budget,
                                                                                                    ese,
                                                                                                    objParametri_Server,
                                                                                                    username,
                                                                                                    GiasContext,
                                                                                                    False
                                                                                                    )
                        End If

                        If esercizio Is Nothing Then

                        Else
                            ese.codice = esercizio.Progetto_Cod
                            'scrittura\aggiornamento\eliminazione codici
                            Internal_ScriviModificaElimina_EserciziCodici_da_Impianto_Appezzamento(Id_Budget,
                                                                                                           ese,
                                                                                                           objParametri_Server,
                                                                                                           username,
                                                                                                           GiasContext,
                                                                                                           False
                                                                                                           )
                        End If
                    Next
                End If
            Next
        End If

        For Each entita In EntitaCodxImg
            GeneraGMapJPG(entita, objParametri_Server)
        Next
    End Sub

    Private Function Internal_Scrivi_Appezzamento(ByRef Id_Budget As Integer,
                                         ByRef app As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                         ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                         ByVal username As String,
                                         Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                         Optional ByVal NewTransaction As Boolean = True
                                         ) As AgronicaCoreEntityFramework_POCO.Budget_Appezzamento

        Dim nomeRoutine As String = "AgronicaCoreBudgetBIZ.Budget_Appezzamento_W.Internal_Scrivi_Appezzamento()"
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

        Dim ret As AgronicaCoreEntityFramework_POCO.Budget_Appezzamento = Nothing

        'Verifica_ValiditaInizioFine(app, objParametri, objParametri_utenti)
        'Verifica_Utilizzo(app, objParametri)
        Try

            ret = AgronicaCoreBudgetDAL.Budget_EFAppezzamento.Appezzamento_Scrivi_EF(Id_Budget,
                                                                                     app,
                                                                                     objParametri,
                                                                                     objParametri_utenti,
                                                                                     username,
                                                                                     GiasContext,
                                                                                     NewTransaction)

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
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

        Return ret
    End Function

    Private Function Internal_Modifica_Appezzamento(ByRef Id_Budget As Integer,
                                                    ByRef app As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    ByVal username As String,
                                                    Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                    Optional ByVal NewTransaction As Boolean = True
                                                    ) As AgronicaCoreEntityFramework_POCO.Budget_Appezzamento

        Dim nomeRoutine As String = "AgronicaCoreBudgetBIZ.Budget_Appezzamento_W.Internal_Modifica_Appezzamento()"
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

        Dim ret As AgronicaCoreEntityFramework_POCO.Budget_Appezzamento = Nothing

        Verifica_ValiditaInizioFine(Id_Budget, True, app, objParametri, objParametri)
        'Verifica_Utilizzo(app, objParametri)
        Verifica_Superficie(app, objParametri)
        Try
            ret = AgronicaCoreBudgetDAL.Budget_EFAppezzamento.Appezzamento_Modifica_EF(Id_Budget,
                                                                                       app,
                                                                                       objParametri,
                                                                                       username,
                                                                                       GiasContext,
                                                                                       NewTransaction)

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If

        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
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
        Return ret
    End Function

    Private Sub Internal_ScriviModificaElimina_AppezzamentoCodici(ByRef Id_Budget As Integer,
                                                                  ByRef DatiAppezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                  ByVal username As String,
                                                                  Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                                  Optional ByVal NewTransaction As Boolean = True)

        Dim nomeRoutine As String = "AgronicaCoreBudgetBIZ.Budget_Appezzamento_W.Internal_ScriviModificaElimina_AppezzamentoCodici()"
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


        Try
            'scrivi appezzamento_codici
            If (DatiAppezzamento.coltura_Precedente_1_Anno IsNot Nothing) Then
                AgronicaCoreBudgetDAL.Budget_EFAppezzamento.ScriviModificaEliminaAppezzamentoCodici(Id_Budget,
                                                              DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
                                                              DatiAppezzamento.primaryKey.centroAziendalePK.codice,
                                                              DatiAppezzamento.primaryKey.codice,
                                                              enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_1,
                                                              DatiAppezzamento.coltura_Precedente_1_Anno.codice,
                                                              False,
                                                              username,
                                                              objParametri,
                                                              GiasContext,
                                                              NewTransaction)
            End If

            If (DatiAppezzamento.coltura_Precedente_2_Anno IsNot Nothing) Then
                AgronicaCoreBudgetDAL.Budget_EFAppezzamento.ScriviModificaEliminaAppezzamentoCodici(Id_Budget,
                                                                DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
                                                                DatiAppezzamento.primaryKey.centroAziendalePK.codice,
                                                                DatiAppezzamento.primaryKey.codice,
                                                                enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_2,
                                                                DatiAppezzamento.coltura_Precedente_2_Anno.codice,
                                                                False,
                                                                username,
                                                                objParametri,
                                                                GiasContext,
                                                                NewTransaction)
            End If

            If (DatiAppezzamento.coltura_Precedente_3_Anno IsNot Nothing) Then
                AgronicaCoreBudgetDAL.Budget_EFAppezzamento.ScriviModificaEliminaAppezzamentoCodici(Id_Budget,
                                                                DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
                                                                DatiAppezzamento.primaryKey.centroAziendalePK.codice,
                                                                DatiAppezzamento.primaryKey.codice,
                                                                enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_3,
                                                                DatiAppezzamento.coltura_Precedente_3_Anno.codice,
                                                                False,
                                                                username,
                                                                objParametri,
                                                                GiasContext,
                                                                NewTransaction)
            End If

            If (DatiAppezzamento.coltura_Precedente_4_Anno IsNot Nothing) Then
                AgronicaCoreBudgetDAL.Budget_EFAppezzamento.ScriviModificaEliminaAppezzamentoCodici(Id_Budget,
                                                                DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
                                                                DatiAppezzamento.primaryKey.centroAziendalePK.codice,
                                                                DatiAppezzamento.primaryKey.codice,
                                                                enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_4,
                                                                DatiAppezzamento.coltura_Precedente_4_Anno.codice,
                                                                False,
                                                                username,
                                                                objParametri,
                                                                GiasContext,
                                                                NewTransaction)
            End If

            If (DatiAppezzamento.confini_A_Rischio IsNot Nothing) Then
                AgronicaCoreBudgetDAL.Budget_EFAppezzamento.ScriviModificaEliminaAppezzamentoCodici(Id_Budget,
                                                                DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
                                                                DatiAppezzamento.primaryKey.centroAziendalePK.codice,
                                                                DatiAppezzamento.primaryKey.codice,
                                                                enum_CodiciAnagrafe.Appezzamento_ConfiniRischio,
                                                                DatiAppezzamento.confini_A_Rischio,
                                                                False,
                                                                username,
                                                                objParametri,
                                                                GiasContext,
                                                                NewTransaction)
            End If

            If (DatiAppezzamento.rif_Appezzamento IsNot Nothing) Then
                AgronicaCoreBudgetDAL.Budget_EFAppezzamento.ScriviModificaEliminaAppezzamentoCodici(Id_Budget,
                                                                DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
                                                                DatiAppezzamento.primaryKey.centroAziendalePK.codice,
                                                                DatiAppezzamento.primaryKey.codice,
                                                                enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento,
                                                                DatiAppezzamento.rif_Appezzamento,
                                                                False,
                                                                username,
                                                                objParametri,
                                                                GiasContext,
                                                                NewTransaction)
            End If

            If (DatiAppezzamento.isola IsNot Nothing) Then
                AgronicaCoreBudgetDAL.Budget_EFAppezzamento.ScriviModificaEliminaAppezzamentoCodici(Id_Budget,
                                                                DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
                                                                DatiAppezzamento.primaryKey.centroAziendalePK.codice,
                                                                DatiAppezzamento.primaryKey.codice,
                                                                enum_CodiciAnagrafe.Isola,
                                                                DatiAppezzamento.isola,
                                                                False,
                                                                username,
                                                                objParametri,
                                                                GiasContext,
                                                                NewTransaction)
            End If

            If (DatiAppezzamento.n_App_Bio IsNot Nothing) Then
                AgronicaCoreBudgetDAL.Budget_EFAppezzamento.ScriviModificaEliminaAppezzamentoCodici(Id_Budget,
                                                                DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
                                                                DatiAppezzamento.primaryKey.centroAziendalePK.codice,
                                                                DatiAppezzamento.primaryKey.codice,
                                                                enum_CodiciAnagrafe.Codice_Appezza_Biologico,
                                                                DatiAppezzamento.n_App_Bio,
                                                                False,
                                                                username,
                                                                objParametri,
                                                                GiasContext,
                                                                NewTransaction)
            End If

            If (DatiAppezzamento.metodo_Produzione Is Nothing) Then
                'AF 04/25: Se proveniamo da giri <> interfaccia Angular il metodo produzione potrebbe non essere valorizzato
                'Ex: APP (da dove è emerso il problema)
                'Per scegliere dinamicamente il valore ci basiamo sul valore 'Disciplinare Aziendale Predefinito' impostato sull'Azienda
                'Se BIO --> Metodo Produzione BIO
                'Else Metodo Produzione Integrato
                Dim objAzienda As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
                Dim codiceDisciplinare = objAzienda.Leggi_Codice_from_Imprese_Codici(CStr(DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva), enum_CodiciAnagrafe.Disciplinare_Aziendale_Default, objParametri)

                If codiceDisciplinare <> "" AndAlso codiceDisciplinare = CInt(enum_Cod_Regolamento.Regolamento_bio).ToString() Then
                    DatiAppezzamento.metodo_Produzione = New AgronicaCoreModelsSTD.metaschema.MetodoProduzione(enum_MetodoProduzione.Biologico)
                Else
                    DatiAppezzamento.metodo_Produzione = New AgronicaCoreModelsSTD.metaschema.MetodoProduzione(enum_MetodoProduzione.Integrato)
                End If
            End If

            If (DatiAppezzamento.metodo_Produzione IsNot Nothing) Then
                AgronicaCoreBudgetDAL.Budget_EFAppezzamento.ScriviModificaEliminaAppezzamentoCodici(Id_Budget,
                                                                DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
                                                                DatiAppezzamento.primaryKey.centroAziendalePK.codice,
                                                                DatiAppezzamento.primaryKey.codice,
                                                                enum_CodiciAnagrafe.MetodoDiProduzione,
                                                                DatiAppezzamento.metodo_Produzione.codice,
                                                                False,
                                                                username,
                                                                objParametri,
                                                                GiasContext,
                                                                NewTransaction)
            End If

            If DatiAppezzamento.fine_Impiego_Prod_Non_Conformi >= AGRODATAINIZIO Then
                Dim strImpiegoProdottiNonConformi = ""
                If DatiAppezzamento.fine_Impiego_Prod_Non_Conformi <> AGRODATAINIZIO AndAlso DatiAppezzamento.fine_Impiego_Prod_Non_Conformi <> AGRODATAFINE Then
                    strImpiegoProdottiNonConformi = DatiAppezzamento.fine_Impiego_Prod_Non_Conformi.ToShortDateString
                End If
                AgronicaCoreBudgetDAL.Budget_EFAppezzamento.ScriviModificaEliminaAppezzamentoCodici(Id_Budget,
                                                                DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
                                                                DatiAppezzamento.primaryKey.centroAziendalePK.codice,
                                                                DatiAppezzamento.primaryKey.codice,
                                                                enum_CodiciAnagrafe.DataFineImpiegoPNC,
                                                                strImpiegoProdottiNonConformi,
                                                                False,
                                                                username,
                                                                objParametri,
                                                                GiasContext,
                                                                NewTransaction)
            End If

            If (DatiAppezzamento.utilizzo_Terreno IsNot Nothing) Then
                Dim valStr = ""
                If DatiAppezzamento.utilizzo_Terreno.Count > 0 Then
                    For Each utlizzoApp In DatiAppezzamento.utilizzo_Terreno
                        valStr &= utlizzoApp.codice & ","
                    Next
                    valStr = valStr.Substring(0, valStr.Length - 1)
                End If
                AgronicaCoreBudgetDAL.Budget_EFAppezzamento.ScriviModificaEliminaAppezzamentoCodici(Id_Budget,
                                                                DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
                                                                DatiAppezzamento.primaryKey.centroAziendalePK.codice,
                                                                DatiAppezzamento.primaryKey.codice,
                                                                enum_CodiciAnagrafe.OrientamentoProduttivo,
                                                                valStr,
                                                                False,
                                                                username,
                                                                objParametri,
                                                                GiasContext,
                                                                NewTransaction)
            End If


            AgronicaCoreBudgetDAL.Budget_EFAppezzamento.ScriviModificaEliminaAppezzamentoCodici(Id_Budget,
                                                                 DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
                                                                DatiAppezzamento.primaryKey.centroAziendalePK.codice,
                                                                DatiAppezzamento.primaryKey.codice,
                                                                enum_CodiciAnagrafe.Terreno_Inutilizzato,
                                                                If(DatiAppezzamento.terrenoInutilizzato, 1, 0),
                                                                False,
                                                                username,
                                                                objParametri,
                                                                GiasContext,
                                                                NewTransaction)

            AgronicaCoreBudgetDAL.Budget_EFAppezzamento.ScriviModificaEliminaAppezzamentoCodici(Id_Budget,
                                                                DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
                                                                DatiAppezzamento.primaryKey.centroAziendalePK.codice,
                                                                DatiAppezzamento.primaryKey.codice,
                                                                enum_CodiciAnagrafe.Terreno_Degradato,
                                                                If(DatiAppezzamento.terrenoDegradato, 1, 0),
                                                                False,
                                                                username,
                                                                objParametri,
                                                                GiasContext,
                                                                NewTransaction)

            AgronicaCoreBudgetDAL.Budget_EFAppezzamento.ScriviModificaEliminaAppezzamentoCodici(Id_Budget,
                                                                DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
                                                                DatiAppezzamento.primaryKey.centroAziendalePK.codice,
                                                                DatiAppezzamento.primaryKey.codice,
                                                                enum_CodiciAnagrafe.Low_ILUC,
                                                                If(DatiAppezzamento.lowILUC, 1, 0),
                                                                False,
                                                                username,
                                                                objParametri,
                                                                GiasContext,
                                                                NewTransaction)




            'TUTTI GLI ALTRI CODICI to do....
            If (DatiAppezzamento.codici IsNot Nothing) Then
                Dim list_id_cod As New List(Of Integer) From {
                    enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_1,
                    enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_2,
                    enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_3,
                    enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_4,
                    enum_CodiciAnagrafe.Appezzamento_ConfiniRischio,
                    enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento,
                    enum_CodiciAnagrafe.Isola,
                    enum_CodiciAnagrafe.Codice_Appezza_Biologico,
                    enum_CodiciAnagrafe.MetodoDiProduzione,
                    enum_CodiciAnagrafe.DataFineImpiegoPNC,
                    enum_CodiciAnagrafe.OrientamentoProduttivo,
                    enum_CodiciAnagrafe.Low_ILUC,
                    enum_CodiciAnagrafe.Terreno_Degradato,
                    enum_CodiciAnagrafe.Terreno_Inutilizzato
                }

                Dim IdBudget = Id_Budget
                Dim Piva = DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva
                Dim sa_cod = DatiAppezzamento.primaryKey.centroAziendalePK.codice
                Dim appezza = DatiAppezzamento.primaryKey.codice

                Dim appezza_codici_del = (From ac In GiasContext.Budget_Appezzamento_Codici Where ac.Id_Budget = IdBudget AndAlso
                                                                                         ac.PIVA = Piva AndAlso
                                                                                         ac.sa_cod = sa_cod AndAlso
                                                                                         ac.appezza = appezza AndAlso
                                                                                         (Not list_id_cod.Contains(ac.id_cod)) AndAlso
                                                                                         (ac.id_cod < 2000 OrElse ac.id_cod > 3000)).ToList

                GiasContext.Budget_Appezzamento_Codici.RemoveRange(appezza_codici_del)
                GiasContext.SaveChanges()

                For Each attr As AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori In DatiAppezzamento.codici
                    Dim ScriviModificaEliminaAppezzamentoCodici As New AgronicaCoreBudgetDAL.Budget_Appezzamento_Codici_W
                    ScriviModificaEliminaAppezzamentoCodici.Scrivi(Id_Budget,
                                                                    DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
                                                                    DatiAppezzamento.primaryKey.centroAziendalePK.codice,
                                                                    DatiAppezzamento.primaryKey.codice,
                                                                    attr.codiceAnagrafe.codice,
                                                                    attr.valore,
                                                                    attr.validita.inizio,
                                                                    attr.validita.fine,
                                                                    objParametri)
                Next
            End If

            'If (DatiAppezzamento.impianti.Count > 0) Then
            '    If (DatiAppezzamento.impianti(0).gruppoVarietale.codice >= 0) Then
            '        AgronicaCoreAnagrafeDAL.EFAppezzamento.ScriviModificaEliminaAppezzamentoCodici(DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
            '                                                        DatiAppezzamento.primaryKey.centroAziendalePK.codice,
            '                                                        DatiAppezzamento.primaryKey.codice,
            '                                                        enum_CodiciAnagrafe.Impianto_Ibrido,
            '                                                        "0",
            '                                                        True,
            '                                                        username,
            '                                                        objParametri,
            '                                                        GiasContext,
            '                                                        NewTransaction)
            '    Else
            '        AgronicaCoreAnagrafeDAL.EFAppezzamento.ScriviModificaEliminaAppezzamentoCodici(DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva,
            '                                                        DatiAppezzamento.primaryKey.centroAziendalePK.codice,
            '                                                        DatiAppezzamento.primaryKey.codice,
            '                                                        enum_CodiciAnagrafe.Impianto_Ibrido,
            '                                                        "1",
            '                                                        True,
            '                                                        username,
            '                                                        objParametri,
            '                                                        GiasContext,
            '                                                        NewTransaction)
            '    End If
            'End If



            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If

        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
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

    Private Sub Internal_ScriviModificaElimina_ImpiantiCodici_da_Appezzamento(ByRef Id_Budget As Integer,
                                                                              ByRef impianto As AgronicaCoreModelsSTD.anagrafiche.Impianto,
                                                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                              ByVal username As String,
                                                                              Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                                              Optional ByVal NewTransaction As Boolean = True)
        Dim nomeRoutine As String = "AgronicaCoreBudgetBIZ.Budget_Appezzamento_W.Internal_ScriviModificaElimina_ImpiantiCodici_da_Appezzamento()"
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


        Try
            'registrazione destinazione uso
            If impianto.utilizzoTerreno IsNot Nothing Then
                If (impianto.utilizzoTerreno.classType = ClassType.DestinazioneUso) Then
                    AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaDestinazioneUsoxImpianto(Id_Budget, impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          impianto.utilizzoTerreno.codice,
                                                                          "",
                                                                          False,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction)
                Else
                    'se ho una varieta\specie i codici da 3000 a 3999 devono essere cancellati
                    AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaDestinazioneUsoxImpianto(Id_Budget,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          impianto.utilizzoTerreno.codice,
                                                                          "",
                                                                          True,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction)
                End If
            End If

            'reg_impianti_codici

            'Interbina
            'If (impianto.interbina <> 0) Then
            '    AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxImpianto(impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
            '                                                              impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
            '                                                              impianto.primaryKey.appezzamentoPK.codice,
            '                                                              impianto.primaryKey.codice,
            '                                                              enum_CodiciAnagrafe.Impianto_Ibrido,
            '                                                              "1",
            '                                                              True,
            '                                                              username,
            '                                                              objParametri,
            '                                                              GiasContext,
            '                                                              NewTransaction)
            'Else
            '    AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaxImpianto(impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
            '                                                              impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
            '                                                              impianto.primaryKey.appezzamentoPK.codice,
            '                                                              impianto.primaryKey.codice,
            '                                                              enum_CodiciAnagrafe.Impianto_Ibrido,
            '                                                              "0",
            '                                                              True,
            '                                                              username,
            '                                                              objParametri,
            '                                                              GiasContext,
            '                                                              NewTransaction)
            'End If

            AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxImpianto(Id_Budget,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          enum_CodiciAnagrafe.Impianto_CodiceB_Maschio,
                                                                          impianto.codBMBDBT_M,
                                                                          True,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction)

            AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxImpianto(Id_Budget,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          enum_CodiciAnagrafe.Impianto_CodiceB_Femmina,
                                                                          impianto.codBMBDBT_F,
                                                                          True,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction)


            AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxImpianto(Id_Budget,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          enum_CodiciAnagrafe.Impianto_Genetica_Maschio,
                                                                          impianto.genetica_M,
                                                                          True,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction)

            AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxImpianto(Id_Budget,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          enum_CodiciAnagrafe.Impianto_Genetica_Femmina,
                                                                          impianto.genetica_F,
                                                                          True,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction)

            AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxImpianto(Id_Budget,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          enum_CodiciAnagrafe.Impianto_OffType_Maschio,
                                                                          impianto.offType_M,
                                                                          True,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction)

            AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxImpianto(Id_Budget,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          enum_CodiciAnagrafe.Impianto_OffType_Femmina,
                                                                          impianto.offType_F,
                                                                          True,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction)

            AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxImpianto(Id_Budget,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          enum_CodiciAnagrafe.Impianto_TraFila_Maschio,
                                                                          impianto.tra_Fila_M,
                                                                          True,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction)

            AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxImpianto(Id_Budget,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          enum_CodiciAnagrafe.Impianto_TraFila_Femmina,
                                                                          impianto.distanzaTraFila_F,
                                                                          True,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction)

            AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxImpianto(Id_Budget,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          enum_CodiciAnagrafe.Impianto_SuFila_Maschio,
                                                                          impianto.su_Fila_M,
                                                                          True,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction)

            AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxImpianto(Id_Budget,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          enum_CodiciAnagrafe.Impianto_SuFila_Femmina,
                                                                          impianto.distanzaSuFila_F,
                                                                          True,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction)

            AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxImpianto(Id_Budget,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          enum_CodiciAnagrafe.Impianto_Interbina,
                                                                          impianto.interbina,
                                                                          True,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction)

            AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxImpianto(Id_Budget,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          enum_CodiciAnagrafe.Impianto_Germinabilita,
                                                                          impianto.germinabilita,
                                                                          True,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction)

            If (impianto.codiceZona IsNot Nothing) Then

                AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxImpianto(Id_Budget,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          enum_CodiciAnagrafe.CodiceZona,
                                                                          impianto.codiceZona.codice,
                                                                          True,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction)

            End If

            If (impianto.tagliatoIntero IsNot Nothing) Then
                AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxImpianto(Id_Budget,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          enum_CodiciAnagrafe.Impianto_Taglio_Tuberi_Patate,
                                                                          impianto.tagliatoIntero.codice,
                                                                          True,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction)
            End If

            AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxImpianto(Id_Budget,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          enum_CodiciAnagrafe.Impianto_Parti_Tuberi_Patate,
                                                                          impianto.partiTuberi,
                                                                          True,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction)

            If (impianto.dettaglio_varieta_personalizzato IsNot Nothing) Then
                AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxImpianto(Id_Budget,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          enum_CodiciAnagrafe.Dettaglio_Specie_Personalizzato,
                                                                          impianto.dettaglio_varieta_personalizzato.codice,
                                                                          True,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction)
            End If

            AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxImpianto(Id_Budget,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                          enum_CodiciAnagrafe.Codice_Impianto,
                                                                          impianto.codiceImpianto,
                                                                          True,
                                                                          username,
                                                                          objParametri,
                                                                          GiasContext,
                                                                          NewTransaction)

            If (impianto.gruppoVarietale.codice >= 0) Then
                AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxImpianto(Id_Budget,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                    enum_CodiciAnagrafe.Impianto_Ibrido,
                                                                    "0",
                                                                    True,
                                                                    username,
                                                                    objParametri,
                                                                    GiasContext,
                                                                    NewTransaction,
                                                                    True)
            Else
                AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxImpianto(Id_Budget,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                          impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                          impianto.primaryKey.appezzamentoPK.codice,
                                                                          impianto.primaryKey.codice,
                                                                    enum_CodiciAnagrafe.Impianto_Ibrido,
                                                                    "1",
                                                                    True,
                                                                    username,
                                                                    objParametri,
                                                                    GiasContext,
                                                                    NewTransaction,
                                                                    True)
            End If


            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
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
    Private Sub Internal_ScriviModificaElimina_EserciziCodici_da_Impianto_Appezzamento(ByRef Id_budget As Integer,
                                                                                        ese As AgronicaCoreModelsSTD.anagrafiche.Esercizio,
                                                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                                        ByVal username As String,
                                                                                        Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                                                       Optional ByVal NewTransaction As Boolean = True)
        Dim nomeRoutine As String = "AgronicaCoreBudgetBIZ.Budget_Appezzamento_W.Internal_Scrivi_EserciziCodici_da_Impianto_Appezzamento()"
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

        Try
            If (ese.apportiMassimiMacroelementi IsNot Nothing) Then
                If (ese.apportiMassimiMacroelementi.tipologia IsNot Nothing) Then
                    AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxProgetto(Id_budget,
                                                                              ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                              ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                              ese.impiantoPK.appezzamentoPK.codice,
                                                                              ese.impiantoPK.codice,
                                                                              ese.codice,
                                                                              enum_CodiciAnagrafe.Finalita_Concimazione_Impianto,
                                                                              ese.apportiMassimiMacroelementi.tipologia.codice,
                                                                              True,
                                                                              username,
                                                                              objParametri,
                                                                              GiasContext,
                                                                              NewTransaction)
                End If

                If (ese.apportiMassimiMacroelementi.n IsNot Nothing) Then
                    AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxProgetto(Id_budget,
                                                                                  ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                                  ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                                  ese.impiantoPK.appezzamentoPK.codice,
                                                                                  ese.impiantoPK.codice,
                                                                                  ese.codice,
                                                                                  enum_CodiciAnagrafe.Impianto_LimiteN,
                                                                                  ese.apportiMassimiMacroelementi.n,
                                                                                  True,
                                                                                  username,
                                                                                  objParametri,
                                                                                  GiasContext,
                                                                                  NewTransaction)
                End If

                If (ese.apportiMassimiMacroelementi.p2o5 IsNot Nothing) Then
                    AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxProgetto(Id_budget,
                                                                                      ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                                      ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                                      ese.impiantoPK.appezzamentoPK.codice,
                                                                                      ese.impiantoPK.codice,
                                                                                      ese.codice,
                                                                                      enum_CodiciAnagrafe.Impianto_LimiteP,
                                                                                      ese.apportiMassimiMacroelementi.p2o5,
                                                                                      True,
                                                                                      username,
                                                                                      objParametri,
                                                                                      GiasContext,
                                                                                      NewTransaction)
                End If

                If (ese.apportiMassimiMacroelementi.k2o IsNot Nothing) Then
                    AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxProgetto(Id_budget,
                                                                                  ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                                  ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                                  ese.impiantoPK.appezzamentoPK.codice,
                                                                                  ese.impiantoPK.codice,
                                                                                  ese.codice,
                                                                                  enum_CodiciAnagrafe.Impianto_LimiteK,
                                                                                  ese.apportiMassimiMacroelementi.k2o,
                                                                                  True,
                                                                                  username,
                                                                                  objParametri,
                                                                                  GiasContext,
                                                                                  NewTransaction)
                End If

                If (ese.apportiMassimiMacroelementi.mgo IsNot Nothing) Then
                    AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxProgetto(Id_budget,
                                                                                  ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                                  ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                                  ese.impiantoPK.appezzamentoPK.codice,
                                                                                  ese.impiantoPK.codice,
                                                                                  ese.codice,
                                                                                  enum_CodiciAnagrafe.Impianto_LimiteMg,
                                                                                  ese.apportiMassimiMacroelementi.mgo,
                                                                                  True,
                                                                                  username,
                                                                                  objParametri,
                                                                                  GiasContext,
                                                                                  NewTransaction)
                End If
            End If

            If (ese.organismo_Referente IsNot Nothing) Then
                If (ese.organismo_Referente.primaryKey IsNot Nothing) Then
                    AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxProgetto(Id_budget,
                                                                              ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                              ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                              ese.impiantoPK.appezzamentoPK.codice,
                                                                              ese.impiantoPK.codice,
                                                                              ese.codice,
                                                                              enum_CodiciAnagrafe.Organismo_Referente,
                                                                              ese.organismo_Referente.primaryKey.codice,
                                                                              True,
                                                                              username,
                                                                              objParametri,
                                                                              GiasContext,
                                                                              NewTransaction)
                End If
            End If

            If (ese.modalita_liquidazione IsNot Nothing) Then
                AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxProgetto(Id_budget,
                                                                                          ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                                          ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                                          ese.impiantoPK.appezzamentoPK.codice,
                                                                                          ese.impiantoPK.codice,
                                                                                          ese.codice,
                                                                                          enum_CodiciAnagrafe.Modalita_Liquidazione,
                                                                                          ese.modalita_liquidazione.codice,
                                                                                          True,
                                                                                          username,
                                                                                          objParametri,
                                                                                          GiasContext,
                                                                                          NewTransaction)
            End If

            If (ese.origine_prodotto IsNot Nothing) Then
                AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxProgetto(Id_budget,
                                                                                           ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                                           ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                                           ese.impiantoPK.appezzamentoPK.codice,
                                                                                           ese.impiantoPK.codice,
                                                                                           ese.codice,
                                                                                           enum_CodiciAnagrafe.Origine_Prodotto,
                                                                                           ese.origine_prodotto.codice,
                                                                                           True,
                                                                                           username,
                                                                                           objParametri,
                                                                                           GiasContext,
                                                                                           NewTransaction)
            End If

            If (ese.riferimento_Trasferimento_Dati IsNot Nothing) Then
                If (ese.riferimento_Trasferimento_Dati.primaryKey IsNot Nothing) Then
                    AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxProgetto(Id_budget,
                                                                              ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                              ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                              ese.impiantoPK.appezzamentoPK.codice,
                                                                              ese.impiantoPK.codice,
                                                                              ese.codice,
                                                                              enum_CodiciAnagrafe.Riferimento_Trasferimento_Dati,
                                                                              ese.riferimento_Trasferimento_Dati.primaryKey.codice,
                                                                              True,
                                                                              username,
                                                                              objParametri,
                                                                              GiasContext,
                                                                              NewTransaction)
                End If
            End If

            If (ese.magazzino_Conferimento IsNot Nothing) Then
                If (ese.magazzino_Conferimento.primaryKey IsNot Nothing) Then
                    AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxProgetto(Id_budget,
                                                                              ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                              ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                              ese.impiantoPK.appezzamentoPK.codice,
                                                                              ese.impiantoPK.codice,
                                                                              ese.codice,
                                                                              enum_CodiciAnagrafe.Magazzino_Conferimento,
                                                                              ese.magazzino_Conferimento.primaryKey.codice & "|" & ese.magazzino_Conferimento.primaryKey.centroAziendalePK.codice & "|" & ese.magazzino_Conferimento.primaryKey.centroAziendalePK.partitaIva,
                                                                              True,
                                                                              username,
                                                                              objParametri,
                                                                              GiasContext,
                                                                              NewTransaction)
                End If
            End If

            If (ese.tecnico IsNot Nothing AndAlso ese.tecnico.Count <> 0) Then
                Dim tecnico_string = ""
                Dim sb As New System.Text.StringBuilder()
                Dim index = ese.tecnico.Count - 1
                If ese.tecnico.Count > 1 Then
                    For Each tecnico_codice In ese.tecnico
                        tecnico_string = sb.Append(tecnico_codice.codice.ToString & If(index = 0, "", "|")).ToString()
                        index = index - 1
                    Next
                Else
                    tecnico_string = ese.tecnico(0).codice
                End If
                AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxProgetto(Id_budget, ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                    ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                    ese.impiantoPK.appezzamentoPK.codice,
                    ese.impiantoPK.codice,
                    ese.codice,
                    enum_CodiciAnagrafe.Tecnico,
                    tecnico_string,
                    True,
                    username,
                    objParametri,
                    GiasContext,
                    NewTransaction)
            Else
                AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxProgetto(Id_budget, ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                    ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                    ese.impiantoPK.appezzamentoPK.codice,
                    ese.impiantoPK.codice,
                    ese.codice,
                    enum_CodiciAnagrafe.Tecnico,
                    "",
                    True,
                    username,
                    objParametri,
                    GiasContext,
                    NewTransaction)
            End If

            If (ese.residuo IsNot Nothing) Then
                If (ese.residuo.codice IsNot Nothing) Then
                    AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxProgetto(Id_budget, ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                    ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                    ese.impiantoPK.appezzamentoPK.codice,
                    ese.impiantoPK.codice,
                    ese.codice,
                    enum_CodiciAnagrafe.Codice_Residuo,
                    ese.residuo.codice,
                    True,
                    username,
                    objParametri,
                    GiasContext,
                    NewTransaction)
                End If
            End If

            If (ese.certificazioneAziendale IsNot Nothing AndAlso ese.certificazioneAziendale.Count <> 0) Then
                Dim certificazione_string = ""
                Dim sb As New System.Text.StringBuilder()
                Dim index = ese.certificazioneAziendale.Count - 1
                If ese.certificazioneAziendale.Count > 1 Then
                    For Each cert_codice In ese.certificazioneAziendale
                        certificazione_string = sb.Append(cert_codice.codice.ToString & If(index = 0, "", "|")).ToString()
                        index = index - 1
                    Next
                Else
                    certificazione_string = ese.certificazioneAziendale(0).codice
                End If
                AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxProgetto(Id_budget, ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                    ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                    ese.impiantoPK.appezzamentoPK.codice,
                    ese.impiantoPK.codice,
                    ese.codice,
                    enum_CodiciAnagrafe.Codice_Certificazione,
                    certificazione_string,
                    True,
                    username,
                    objParametri,
                    GiasContext,
                    NewTransaction)
            Else
                AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxProgetto(Id_budget, ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                    ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                    ese.impiantoPK.appezzamentoPK.codice,
                    ese.impiantoPK.codice,
                    ese.codice,
                    enum_CodiciAnagrafe.Codice_Certificazione,
                    "",
                    True,
                    username,
                    objParametri,
                    GiasContext,
                    NewTransaction)
            End If

            If (ese.contributi IsNot Nothing AndAlso ese.contributi.Count <> 0) Then
                Dim contributi_string = ""
                Dim sb As New System.Text.StringBuilder()
                Dim index = ese.contributi.Count - 1
                If ese.contributi.Count > 1 Then
                    For Each contrib_codice In ese.contributi
                        contributi_string = sb.Append(contrib_codice.codice.ToString & If(index = 0, "", "|")).ToString()
                        index = index - 1
                    Next
                Else
                    contributi_string = ese.contributi(0).codice
                End If
                AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxProgetto(Id_budget, ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                    ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                    ese.impiantoPK.appezzamentoPK.codice,
                    ese.impiantoPK.codice,
                    ese.codice,
                    enum_CodiciAnagrafe.Contributi,
                    contributi_string,
                    True,
                    username,
                    objParametri,
                    GiasContext,
                    NewTransaction)
            Else
                AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxProgetto(Id_budget, ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                    ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                    ese.impiantoPK.appezzamentoPK.codice,
                    ese.impiantoPK.codice,
                    ese.codice,
                    enum_CodiciAnagrafe.Contributi,
                    "",
                    True,
                    username,
                    objParametri,
                    GiasContext,
                    NewTransaction)
            End If

            If (ese.certificazioneProdotto IsNot Nothing) Then
                If (ese.certificazioneProdotto.codice IsNot Nothing) Then

                    Dim piva As String = ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva

                    Dim sa_cod As Integer = ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice

                    Dim appezza As Integer = ese.impiantoPK.appezzamentoPK.codice

                    Dim idReg As Integer = ese.impiantoPK.codice

                    Dim progetto_cod As Integer = ese.codice

                    Dim id_cod As Integer = enum_CodiciAnagrafe.Codice_Certificazione_Prodotto

                    Dim val_Cod As String = ese.certificazioneProdotto.codice

                    Dim imp = AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.Leggi_Reg_Impianti_Codici(GiasContext, Id_budget, piva, sa_cod, appezza, idReg, progetto_cod, id_cod)

                    If Not IsNothing(imp) Then

                        Dim operazione As Integer = -1

                        If imp.Count > 0 AndAlso String.IsNullOrEmpty(val_Cod) Then
                            operazione = TipiEnumerativi.enum_TipoOperazioneDB.Cancellazione
                        ElseIf imp.Count > 0 AndAlso Not String.IsNullOrEmpty(val_Cod) Then
                            operazione = TipiEnumerativi.enum_TipoOperazioneDB.Modifica
                        ElseIf imp.Count = 0 AndAlso String.IsNullOrEmpty(val_Cod) Then
                            operazione = TipiEnumerativi.enum_TipoOperazioneDB.Lettura
                        ElseIf imp.Count = 0 AndAlso Not String.IsNullOrEmpty(val_Cod) Then
                            operazione = TipiEnumerativi.enum_TipoOperazioneDB.Scrittura
                        End If

                        AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxProgetto(Id_budget, piva,
                                                                                                       sa_cod,
                                                                                                        appezza,
                                                                                                        idReg,
                                                                                                        progetto_cod,
                                                                                                        enum_CodiciAnagrafe.Codice_Certificazione_Prodotto,
                                                                                                        val_Cod,
                                                                                                        True,
                                                                                                        username,
                                                                                                        objParametri,
                                                                                                        GiasContext,
                                                                                                        NewTransaction,
                                                                                                            operazione,
                                                                                                            imp)
                    End If
                End If
            End If

            If (ese.capitolato_Privato IsNot Nothing) Then
                If (ese.capitolato_Privato.codice IsNot Nothing) Then
                    AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxProgetto(Id_budget,
                                                                              ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                              ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                              ese.impiantoPK.appezzamentoPK.codice,
                                                                              ese.impiantoPK.codice,
                                                                              ese.codice,
                                                                              enum_CodiciAnagrafe.Capitolato_Privato,
                                                                              ese.capitolato_Privato.codice,
                                                                              True,
                                                                              username,
                                                                              objParametri,
                                                                              GiasContext,
                                                                              NewTransaction)
                End If
            End If

            If (ese.licenza_Coltivazione IsNot Nothing) Then
                AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxProgetto(Id_budget,
                                                                              ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                              ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                              ese.impiantoPK.appezzamentoPK.codice,
                                                                              ese.impiantoPK.codice,
                                                                              ese.codice,
                                                                              enum_CodiciAnagrafe.Zespri_Fasi_Fase,
                                                                              ese.licenza_Coltivazione.codice,
                                                                              True,
                                                                              username,
                                                                              objParametri,
                                                                              GiasContext,
                                                                              NewTransaction)
            End If

            If (ese.piano_Semina IsNot Nothing) Then
                AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxProgetto(Id_budget,
                                                                              ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                              ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                              ese.impiantoPK.appezzamentoPK.codice,
                                                                              ese.impiantoPK.codice,
                                                                              ese.codice,
                                                                              enum_CodiciAnagrafe.Impianto_PianoSemina,
                                                                              ese.piano_Semina.codice,
                                                                              True,
                                                                              username,
                                                                              objParametri,
                                                                              GiasContext,
                                                                              NewTransaction)
            End If

            If (ese.lavorazione IsNot Nothing) Then
                AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxProgetto(Id_budget,
                                                                                           ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                                           ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                                           ese.impiantoPK.appezzamentoPK.codice,
                                                                                           ese.impiantoPK.codice,
                                                                                           ese.codice,
                                                                                           enum_CodiciAnagrafe.Lavorazione,
                                                                                           ese.lavorazione.codice,
                                                                                           True,
                                                                                           username,
                                                                                           objParametri,
                                                                                           GiasContext,
                                                                                           NewTransaction)
            End If

            If (ese.specifica IsNot Nothing) Then
                AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxProgetto(Id_budget,
                                                                                           ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                                           ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                                           ese.impiantoPK.appezzamentoPK.codice,
                                                                                           ese.impiantoPK.codice,
                                                                                           ese.codice,
                                                                                           enum_CodiciAnagrafe.Specifica,
                                                                                           ese.specifica.codice,
                                                                                           True,
                                                                                           username,
                                                                                           objParametri,
                                                                                           GiasContext,
                                                                                           NewTransaction)
            End If

            AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxProgetto(Id_budget,
                                                                              ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                              ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                              ese.impiantoPK.appezzamentoPK.codice,
                                                                              ese.impiantoPK.codice,
                                                                              ese.codice,
                                                                              enum_CodiciAnagrafe.Distinta_Chiusa,
                                                                              If(ese.esercizio_Chiuso, 1, 0),
                                                                              True,
                                                                              username,
                                                                              objParametri,
                                                                              GiasContext,
                                                                              NewTransaction)

            If (ese.iaf IsNot Nothing) Then
                AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScriviModificaEliminaxProgetto(Id_budget,
                                                                              ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                              ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                              ese.impiantoPK.appezzamentoPK.codice,
                                                                              ese.impiantoPK.codice,
                                                                              ese.codice,
                                                                              enum_CodiciAnagrafe.Impianto_IAF_ImpegniAggiuntiviFacoltativi,
                                                                              String.Join("|", ese.iaf),
                                                                              True,
                                                                              username,
                                                                              objParametri,
                                                                              GiasContext,
                                                                              NewTransaction)
            End If

            GiasContext.SaveChanges()

            'TUTTI GLI ALTRI CODICI to do......
            If (ese.codici IsNot Nothing) Then
                Dim listField As New List(Of Integer) From {enum_CodiciAnagrafe.Finalita_Concimazione_Impianto,
                                                            enum_CodiciAnagrafe.Impianto_LimiteN,
                                                            enum_CodiciAnagrafe.Impianto_LimiteP,
                                                            enum_CodiciAnagrafe.Impianto_LimiteK,
                                                            enum_CodiciAnagrafe.Impianto_LimiteMg,
                                                            enum_CodiciAnagrafe.Organismo_Referente,
                                                            enum_CodiciAnagrafe.Modalita_Liquidazione,
                                                            enum_CodiciAnagrafe.Origine_Prodotto,
                                                            enum_CodiciAnagrafe.Riferimento_Trasferimento_Dati,
                                                            enum_CodiciAnagrafe.Magazzino_Conferimento,
                                                            enum_CodiciAnagrafe.Capitolato_Privato,
                                                            enum_CodiciAnagrafe.Codice_Residuo,
                                                            enum_CodiciAnagrafe.Codice_Certificazione,
                                                            enum_CodiciAnagrafe.Codice_Certificazione_Prodotto,
                                                            enum_CodiciAnagrafe.Contributi,
                                                            enum_CodiciAnagrafe.Tecnico,
                                                            enum_CodiciAnagrafe.Lavorazione,
                                                            enum_CodiciAnagrafe.Specifica,
                                                            enum_CodiciAnagrafe.Zespri_Fasi_Fase,
                                                            enum_CodiciAnagrafe.Impianto_PianoSemina,
                                                            enum_CodiciAnagrafe.Distinta_Chiusa,
                                                            enum_CodiciAnagrafe.Impianto_IAF_ImpegniAggiuntiviFacoltativi}

                Dim codiciEse = (From c In ese.codici Select c.codiceAnagrafe.codice).ToList
                Dim idbudget = Id_budget
                Dim Piva = ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva
                Dim Sa_Cod = ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice
                Dim Appezza = ese.impiantoPK.appezzamentoPK.codice
                Dim id_Reg = ese.impiantoPK.codice
                Dim Progetto_Cod = ese.codice

                Dim imprese_progetti_codici_del = (From ipc In GiasContext.Budget_Reg_Impianti_Codici Where ipc.Id_Budget = idbudget AndAlso
                                                                                                   ipc.PIVA = Piva AndAlso
                                                                                                   ipc.sa_cod = Sa_Cod AndAlso
                                                                                                   ipc.appezza = Appezza AndAlso
                                                                                                   ipc.Id_Reg = id_Reg AndAlso
                                                                                                   ipc.Progetto_Cod = Progetto_Cod AndAlso
                                                                                                   (ipc.id_cod < 2000 OrElse ipc.id_cod > 3000) AndAlso
                                                                                                    Not listField.Contains(ipc.id_cod)).ToList

                If imprese_progetti_codici_del.Count > 0 Then
                    GiasContext.Budget_Reg_Impianti_Codici.RemoveRange(imprese_progetti_codici_del)
                    GiasContext.SaveChanges()
                End If

                For Each attr As AgronicaCoreModelsSTD.anagrafiche.CodiciAnagrafeValori In ese.codici

                    If attr.validita Is Nothing Then
                        attr.validita = New IntervalloTemporale()
                    Else
                        If attr.validita.inizio < AGRODATAINIZIO Then
                            attr.validita.inizio = AGRODATAINIZIO
                        End If

                        If attr.validita.fine < AGRODATAINIZIO Then
                            attr.validita.fine = AGRODATAFINE
                        End If

                    End If

                    AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.ScrivixProgetto(Id_budget,
                                                                    ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                    ese.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                                                    ese.impiantoPK.appezzamentoPK.codice,
                                                                    ese.impiantoPK.codice,
                                                                    ese.codice,
                                                                    attr.codiceAnagrafe.codice,
                                                                    attr.valore,
                                                                    True,
                                                                    username,
                                                                    objParametri,
                                                                    GiasContext,
                                                                    NewTransaction,
                                                                    attr.validita.inizio,
                                                                    attr.validita.fine)
                Next
            End If

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If

        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
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

    Private Sub Internal_Aggiorna_IndirizziAppezzamento(Appezzamento As AgronicaCoreEntityFramework_POCO.Budget_Appezzamento,
                                                        ByRef objParametri_Server As AgronicaCoreParametri,
                                                        indirizzi As List(Of IndirizzoAssociato),
                                                        ByVal username As String,
                                                        Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                        Optional ByVal NewTransaction As Boolean = True)

        Dim nomeRoutine As String = "AgronicaCoreBudgetBIZ.Budget_Appezzamento_W.Internal_ScriviModifica_IndirizzoAssociatoAppezzamento()"
        Dim messaggioErrore As String = ""
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri_Server.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Try

            Dim list_indirizzi_cod = (From i In indirizzi Select i.indirizzo.codice).ToList

            Dim Id_Budget = Appezzamento.Id_Budget
            Dim Piva = Appezzamento.PIVA
            Dim Sa_Cod = Appezzamento.SA_COD
            Dim Appezza = Appezzamento.APPEZZA

            Dim appezzaxIndirizzi_Del = (From axi In GiasContext.Budget_AppezzamentixIndirizzi Where axi.Id_Budget = Id_Budget AndAlso
                                                                                          axi.PIVA = Piva AndAlso
                                                                                          axi.sa_cod = Sa_Cod AndAlso
                                                                                          axi.appezza = Appezza AndAlso
                                                                                          Not list_indirizzi_cod.Contains(axi.cod_indirizzo)).ToList

            Dim indirizzi_cod_del = (From a In appezzaxIndirizzi_Del Select a.cod_indirizzo).ToList()

            Dim indirizzi_del = (From i In GiasContext.Indirizzi Where indirizzi_cod_del.Contains(i.cod_indirizzo))

            GiasContext.Indirizzi.RemoveRange(indirizzi_del)
            GiasContext.Budget_AppezzamentixIndirizzi.RemoveRange(appezzaxIndirizzi_Del)

            GiasContext.SaveChanges()

            For Each add As AgronicaCoreModelsSTD.anagrafiche.IndirizzoAssociato In indirizzi
                If (add.indirizzo.istatComune.com IsNot Nothing) AndAlso (add.indirizzo.istatComune.prov IsNot Nothing) Then
                    Internal_ScriviModifica_IndirizzoAssociatoAppezzamento(add,
                                                                           objParametri_Server,
                                                                           Appezzamento,
                                                                           add.tipo_Indirizzo,
                                                                           username,
                                                                           GiasContext,
                                                                           False
                                                                           )
                End If
            Next

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If

        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
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

    Private Sub Internal_ScriviModifica_IndirizzoAssociatoAppezzamento(ByVal indAssociato As AgronicaCoreModelsSTD.anagrafiche.IndirizzoAssociato,
                                                                       ByRef objParametri As AgronicaCoreParametri,
                                                                       ByRef appezzamento As AgronicaCoreEntityFramework_POCO.Budget_Appezzamento,
                                                                       ByRef tipoIndirizzo As Integer,
                                                                       ByRef username As String,
                                                                       Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                                       Optional ByVal NewTransaction As Boolean = True)
        Dim nomeRoutine As String = "AgronicaCoreBudgetBIZ.Budget_Appezzamento_W.Internal_ScriviModifica_IndirizzoAssociatoAppezzamento()"
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

        Try
            If indAssociato.indirizzo.codice = 0 Then
                AgronicaCoreBudgetDAL.Budget_EFAppezzamento.CreateIndirizzoAppezzamento(indAssociato.indirizzo,
                                                                                   objParametri,
                                                                                   appezzamento,
                                                                                   indAssociato.tipo_Indirizzo,
                                                                                   username,
                                                                                   GiasContext,
                                                                                   NewTransaction
)
            Else
                AgronicaCoreBudgetDAL.Budget_EFAppezzamento.ModificaIndirizzoAppezzamento(indAssociato.indirizzo,
                                                                                    objParametri,
                                                                                    appezzamento,
                                                                                    indAssociato.tipo_Indirizzo,
                                                                                    username,
                                                                                    GiasContext,
                                                                                    NewTransaction
                                                                                    )
            End If
            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If

        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
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

    Private Sub Internal_Scrivi_CatastoAppezzamento(ByRef Id_Budget As Integer,
                                                    ByRef appezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                                    ByRef objParametri As AgronicaCoreParametri,
                                                    ByVal username As String,
                                                    Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                    Optional ByVal NewTransaction As Boolean = True)

        Const nomeRoutine = "AgronicaCoreBudgetBIZ.Budget_Appezzamento_W.Internal_ScriviModifica_IndirizzoAssociatoAppezzamento()"
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

        Try

            If appezzamento.primaryKey.codice <> 0 Then

                Dim idbudget = Id_Budget
                Dim Piva = appezzamento.primaryKey.centroAziendalePK.partitaIva
                Dim Sa_Cod = appezzamento.primaryKey.centroAziendalePK.codice
                Dim Appezza = appezzamento.primaryKey.codice
                Dim catastoOld = (From c In GiasContext.Budget_AppezzamentiXParticelle
                                  Where c.Id_Budget = idbudget AndAlso c.PIVA = Piva AndAlso c.SA_COD = Sa_Cod AndAlso c.APPEZZA = Appezza).ToList

                GiasContext.Budget_AppezzamentiXParticelle.RemoveRange(catastoOld)
                GiasContext.SaveChanges()

                For Each catasto In appezzamento.catastoAppezzamento
                    AgronicaCoreBudgetDAL.Budget_EFAppezzamento.Create_CatastoAppezzamento(Id_Budget, catasto, objParametri, appezzamento, username, GiasContext, False)
                Next


            End If

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If

        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
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

    Private Function Verifica_ValiditaInizioFine(ByRef Id_Budget As Integer,
                                                 ByRef internal As Boolean,
                                                 ByRef appezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                                 ByRef objParametri_Server As AgronicaCoreParametri,
                                                 ByRef objParametri_Utenti As AgronicaCoreParametri)

        Dim MessaggioErrore As String = ""
        Dim NomeRoutine = "Verifica_ValiditaInizioFine"
        Dim Validita_Fine = appezzamento.validita.fine
        Dim Validita_Inizio = appezzamento.validita.inizio

        Dim sa_cod = appezzamento.primaryKey.centroAziendalePK.codice
        Dim piva = appezzamento.primaryKey.centroAziendalePK.partitaIva
        Dim appezza = appezzamento.primaryKey.codice
        Dim descrizione = appezzamento.descrizione

        Dim objCentriR As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        Dim centro = objCentriR.Leggi(piva, sa_cod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        Dim objAppR As New AgronicaCoreBudgetDAL.Budget_Appezzamento_R
        Dim app = objAppR.Leggi(Id_Budget, piva, sa_cod, appezza, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        Dim objImpiantoR As New AgronicaCoreBudgetDAL.Budget_Reg_Impianti_Read
        Dim imp = objImpiantoR.Leggi(Id_Budget, piva, sa_cod, appezza, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        Try

            If app.Rows.Count > 0 Then

                'Se l'intervallo delle validità viene modificato, controllo e aggiorno, se necessario, l'Impianto
                If Validita_Inizio <> app.Rows(0)("Validita_Inizio") Or Validita_Fine <> app.Rows(0)("Validita_Fine") Then
                    If False Then
                        If imp.Rows.Count > 0 Then
                            For Each i In imp.Rows
                                Dim id_Reg = i("id_reg")

                                'Ripristino le date dopo ogni iterazione
                                Dim Validita_InizioNew = Validita_Inizio
                                Dim Validita_FineNew = Validita_Fine

                                'Controllo se le date dell'Impianto rientrano nell'intervallo temporale dell'Appezzamneto, in questo caso rimangono invariate
                                If i("Validita_Inizio") > Validita_InizioNew Then
                                    Validita_InizioNew = i("Validita_Inizio")
                                End If
                                If Validita_FineNew > i("Validita_Fine") Then
                                    Validita_FineNew = i("Validita_Fine")
                                End If

                                If i("Validita_Inizio") <> Validita_InizioNew Or Validita_FineNew <> i("Validita_Fine") Then
                                    Dim objImpiantiR As New AgronicaCoreBudgetBIZ.Budget_Appezzamento_R
                                    Dim impR = objImpiantiR.Leggi_Appezzamento_Anagrafica(Id_Budget:=Id_Budget,
                                                                                          Piva:=piva,
                                                                                          Sa_Cod:=sa_cod,
                                                                                          Appezza:=appezza,
                                                                                          IdReg:=id_Reg,
                                                                                          Leggi_Impianti:=True,
                                                                                          Leggi_Indirizzi:=True,
                                                                                          Leggi_Catasto:=True,
                                                                                          data:=AGRODATAINIZIO,
                                                                                          filtroData:=False,
                                                                                          Leggi_Distinte:=True,
                                                                                          Leggi_Cartografia:=False,
                                                                                          objParametri_Server,
                                                                                          objParametri_Server,
                                                                                          objParametri_Utenti
                                                                                          )
                                    'lavez - 15/02/2022 - se non trova nulla, non deve andare in crash...
                                    If impR IsNot Nothing Then

                                        '------------------------
                                        'CONTROLLO DATE IMPIANTI
                                        '------------------------
                                        Dim impiantiDaEliminare As New List(Of AgronicaCoreModelsSTD.anagrafiche.Impianto)
                                        Dim eserciziDaEliminare As New List(Of AgronicaCoreModelsSTD.anagrafiche.Esercizio)

                                        For Each impianto In impR.impianti

                                            'Ripristino le date dopo ogni iterazione
                                            Dim Validita_Inizio_Impianto = Validita_InizioNew
                                            Dim Validita_Fine_Impianto = Validita_FineNew

                                            'se la data di inizio dell'impianto è SUCCESSIVA alla FINE dell'Appezzamento, elimino l'Impianto
                                            'se la data di fine dell'impianto è PRECEDENTE all'INIZIO dell'Appezzamento, elimino l'Impianto
                                            If impianto.validita.inizio > Validita_FineNew OrElse impianto.validita.fine < Validita_InizioNew Then
                                                impiantiDaEliminare.Add(impianto)

                                                'Controllo se le date dell'Impianto rientrano nell'intervallo temporale dell'Appezzamento, in questo caso rimangono invariate
                                            ElseIf impianto.validita.inizio >= Validita_InizioNew AndAlso impianto.validita.fine <= Validita_FineNew Then
                                                If impianto.validita.inizio > Validita_InizioNew Then
                                                    Validita_Inizio_Impianto = impianto.validita.inizio
                                                End If
                                                If impianto.validita.fine < Validita_FineNew Then
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
                                                If esercizio.validita.inizio > Validita_Fine_Impianto OrElse esercizio.validita.fine < Validita_Inizio_Impianto Then
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
                                            impR.impianti.Remove(impianto)
                                        Next

                                        Dim objAppsW As New AgronicaCoreBudgetBIZ.Budget_Appezzamento_W
                                        If Not internal Then
                                            Dim appW = objAppsW.Appezzamento_ScriviModifica(Id_Budget, impR, objParametri_Server, objParametri_Utenti)
                                        End If
                                    End If
                                End If
                            Next
                        End If
                    End If

                    If Validita_Inizio < CDate(centro.Rows(0)("Validita_Inizio")) Then
                        MessaggioErrore += ("L'inizio dell'appezzamento non può precedere la creazione del Centro Aziendale") & " (" & CDate(centro.Rows(0)("Validita_Inizio")).ToShortDateString & ")."
                        Throw New GiasException(MessaggioErrore)
                    End If

                    If Validita_Fine > CDate(centro.Rows(0)("Validita_Fine")) Then
                        MessaggioErrore &= ("La fine dell'appezzamento non può seguire la cessazione del Centro Aziendale") & " (" & CDate(centro.Rows(0)("Validita_Fine")).ToShortDateString & ")."
                        Throw New GiasException(MessaggioErrore)
                    End If

                    'controllo COSTI DI GESTIONE su qualsiasi Esercizio collegato
                    Dim objControlloCdG As New AgronicaCoreAnagrafeBIZ.Progetto_W
                    Dim controlloCdG = objControlloCdG.controllo_CdG(Nothing, piva, sa_cod, appezza, 0, 0, Validita_Inizio, Validita_Fine, objParametri_Server, Id_Budget:=Id_Budget)
                    If controlloCdG.errore Then
                        Dim MessaggioErroreCdG As String = ""

                        If Not controlloCdG.messaggioSpecifico Then
                            MessaggioErroreCdG &= ("Non è possibile modificare la " & controlloCdG.inizio_fine & " dell'appezzamento " & descrizione & ", perché sono stati associati Costi di Gestione ad un esercizio in data successiva a quella selezionata")
                        Else
                            MessaggioErroreCdG &= ("Non è possibile modificare la " & controlloCdG.inizio_fine & " dell'appezzamento " & descrizione & ", perché sono stati associati Costi di Gestione ad un esercizio in data " & controlloCdG.dataCdG)
                        End If

                        Throw New GiasException(MessaggioErroreCdG)
                    End If

                End If
            End If

            If Validita_Fine < Validita_Inizio Then
                MessaggioErrore += ("La fine dell'appezzamento non può precedere la sua data di inizio.")
                Throw New GiasException(MessaggioErrore)
            End If

        Catch ex As GiasException
            MessaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return MessaggioErrore
    End Function

    Private Function eliminaImpianti(ByRef Id_Budget As Integer,
                                     ByRef DatiAppezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                     GiasContext As Gias_DeveloperServer_Entities,
                                     objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri) As AgronicaCoreModelsSTD.anagrafiche.Appezzamento

        Dim MessaggioErrore As String = ""
        Dim nomeRoutine = "eliminaImpianti"
        '-----------------------------------------
        ' CANCELLAZIONE IMPIANTI DA CONTROLLO DATE
        '-----------------------------------------
        Dim id_reg As New List(Of Integer)
        Dim impiantiDaEliminare As New List(Of AgronicaCoreEntityFramework_POCO.Budget_Reg_Impianti)

        Try
            For Each impianto In DatiAppezzamento.impianti
                id_reg.Add(impianto.primaryKey.codice)
            Next

            If id_reg.Count > 0 Then
                Dim idbudget = Id_Budget
                Dim Piva = DatiAppezzamento.primaryKey.centroAziendalePK.partitaIva
                Dim sa_cod = DatiAppezzamento.primaryKey.centroAziendalePK.codice
                Dim appezza = DatiAppezzamento.primaryKey.codice

                impiantiDaEliminare = (From impianti In GiasContext.Budget_Reg_Impianti Where
                                                           impianti.Id_Budget = idbudget _
                                                           AndAlso impianti.PIVA = Piva _
                                                           AndAlso impianti.SA_COD = sa_cod _
                                                           AndAlso impianti.APPEZZA = appezza _
                                                           AndAlso Not id_reg.Contains(impianti.ID_REG)
                                       Select impianti).ToList()

                If impiantiDaEliminare.Count > 0 Then
                    For Each impiantoDaEliminare In impiantiDaEliminare
                        Dim codiceImpianto = impiantoDaEliminare.ID_REG
                        Dim DatiImpianto As AgronicaCoreModelsSTD.anagrafiche.Impianto
                        If DatiAppezzamento.impianti IsNot Nothing Then
                            DatiImpianto = (From ia In DatiAppezzamento.impianti Where ia.primaryKey.codice = codiceImpianto).FirstOrDefault
                        Else
                            Dim objImpianti_R As New AgronicaCoreBudgetBIZ.Budget_Reg_Impianto_R
                            DatiImpianto = objImpianti_R.Leggi_Impianto_Anagrafica(idbudget, impiantoDaEliminare.PIVA,
                                                                                   impiantoDaEliminare.SA_COD,
                                                                                   impiantoDaEliminare.APPEZZA,
                                                                                   impiantoDaEliminare.ID_REG,
                                                                                   True,
                                                                                   False,
                                                                                   AGRODATAINIZIO,
                                                                                   False,
                                                                                   False,
                                                                                   objParametri_Server,
                                                                                   objParametri_Server,
                                                                                   objParametri_Utenti)
                        End If

                        Dim objControlloPrenotazioni As New AgronicaCoreBudgetBIZ.Budget_Reg_Impianto_R
                        If objControlloPrenotazioni.controllo_MovimentiProgrammazionexEliminazione(Id_Budget, impiantoDaEliminare.PIVA, impiantoDaEliminare.SA_COD, impiantoDaEliminare.APPEZZA, impiantoDaEliminare.ID_REG, objParametri_Server) Then
                            Dim msgErrore = Gias.ImpossibileEliminareImpiantoEsistonoRichiesteMaterialeVivaisticoAssociate

                            Throw New GiasException(msgErrore)
                        End If

                        eliminaImpianto(impiantoDaEliminare, DatiImpianto, objParametri_Server, objParametri_Utenti, GiasContext)

                    Next
                End If
            End If

        Catch ex As GiasException
            MessaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DatiAppezzamento
    End Function

    Private Sub eliminaImpianto(impiantoDaEliminare As AgronicaCoreEntityFramework_POCO.Budget_Reg_Impianti,
                                DatiImpianto As AgronicaCoreModelsSTD.anagrafiche.Impianto,
                                objParametri_Server As AgronicaCoreParametri,
                                objparametri_Utenti As AgronicaCoreParametri,
                                GiasContext As Gias_DeveloperServer_Entities)

        Dim Id_Budget = impiantoDaEliminare.Id_Budget
        Dim Piva = impiantoDaEliminare.PIVA
        Dim Sa_Cod = impiantoDaEliminare.SA_COD
        Dim Appezza = impiantoDaEliminare.APPEZZA
        Dim Id_Reg = impiantoDaEliminare.ID_REG


        'SALTO...NEL BUDGET NON CI SONO LE OP COLTURALI
        'Dim controllo As New AgronicaCoreBudgetBIZ.Budget_Reg_Impianto_W
        'controllo.controllo_MovimentiRicettePua(Piva, Sa_Cod, Appezza, Id_Reg, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)
        'If controllo.controllo_MovimentiRicettexEliminazione(DatiImpianto, Piva, Sa_Cod, Appezza, Id_Reg, objParametri_Server) Then
        'Dim descrizioneImpianto = ""
        '    If DatiImpianto.utilizzoTerreno IsNot Nothing Then
        '        If DatiImpianto.utilizzoTerreno.classType = ClassType.DestinazioneUso Then
        '            descrizioneImpianto = DatiImpianto.utilizzoTerreno.descrizione
        '        Else
        '            Dim varieta = CType(DatiImpianto.utilizzoTerreno, AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta)
        '            descrizioneImpianto = varieta.specie.descrizione & " - " & varieta.descrizione
        '        End If

        '        descrizioneImpianto &= " [" & DatiImpianto.superficie & "ha] "
        '    End If
        '    Dim MessaggioErrore = (" Non è possibile modificare/eliminare l'Impianto " & descrizioneImpianto & " perché ci sono registrazioni associate. ")
        '    Throw New GiasException(MessaggioErrore)
        'End If
        'controllo.controllo_CancellazioneVincoliLetamazioniPUAxEliminazione(DatiImpianto, Piva, Sa_Cod, Appezza, Id_Reg, objParametri_Server)


        Dim impiantiCodiciDaEliminare As New List(Of AgronicaCoreEntityFramework_POCO.Budget_Reg_Impianti_Codici)

        Dim impianto As New AgronicaCoreModelsSTD.anagrafiche.Impianto(New AgronicaCoreModelsSTD.anagrafiche.Impianto.PK(
                                Id_Reg, New AgronicaCoreModelsSTD.anagrafiche.Appezzamento.PK(
                                    Appezza, New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(
                                        Sa_Cod, Piva))))

        impiantiCodiciDaEliminare = (From impianti In GiasContext.Budget_Reg_Impianti_Codici Where
                                                          impianti.Id_Budget = Id_Budget _
                                                          AndAlso impianti.PIVA = Piva _
                                                          AndAlso impianti.sa_cod = Sa_Cod _
                                                          AndAlso impianti.appezza = Appezza _
                                                          AndAlso impianti.Id_Reg = Id_Reg _
                                                          AndAlso impianti.Progetto_Cod = 0
                                     Select impianti).ToList()

        If impiantiCodiciDaEliminare.Count > 0 Then
            GiasContext.Budget_Reg_Impianti_Codici.RemoveRange(impiantiCodiciDaEliminare)
            GiasContext.SaveChanges()
        End If

        ' cancella associazione impianto-macchina
        Budget_EFReg_Impianti.ScriviModificaEliminaMacchinaxImpianto(enum_TipoOperazioneDB.Cancellazione,
                                                                      Id_Budget, Piva, Sa_Cod, Appezza, Id_Reg, Nothing,
                                                                      objParametri_Server.UsernameOperazione,
                                                                      objParametri_Server, GiasContext, False, True)

        Dim eserciziDaEliminare = (From ip In GiasContext.Budget_Imprese_Progetti Where ip.Id_Budget = Id_Budget AndAlso
                                                                               ip.Piva = Piva AndAlso
                                                                               ip.Sa_Cod = Sa_Cod AndAlso
                                                                               ip.Appezza = Appezza AndAlso
                                                                               ip.Id_Reg = Id_Reg).ToList

        For Each esercizioDaEliminare In eserciziDaEliminare
            Dim Progetto_Cod = esercizioDaEliminare.Progetto_Cod
            Dim objProg As New AgronicaCoreBudgetBIZ.Budget_Progetto_R
            Dim DatiEsercizio As AgronicaCoreModelsSTD.anagrafiche.Esercizio
            'DatiEsercizio = objProg.Leggi_Esercizio_Anagrafica(Nothing, Id_Budget, Progetto_Cod, Nothing, objParametri_Server, objparametri_Utenti)
            DatiEsercizio = New Esercizio
            'If DatiImpianto IsNot Nothing AndAlso DatiImpianto.esercizi IsNot Nothing Then
            '    DatiEsercizio = (From de In DatiImpianto.esercizi Where de.codice = Progetto_Cod).FirstOrDefault
            'End If

            Dim objControllo As New AgronicaCoreAnagrafeBIZ.Progetto_W
            Dim controlloCdG = objControllo.controllo_CdGxEliminazione(DatiEsercizio, Piva, Sa_Cod, Appezza, Id_Reg, Progetto_Cod, objParametri_Server, Id_Budget:=Id_Budget)
            If controlloCdG.errore Then
                Dim MessaggioErrore As String = ""

                Dim descrizione_imp As String = "[dal " & impiantoDaEliminare.Validita_Inizio & " al " & impiantoDaEliminare.Validita_Fine & "]"

                Dim descrizione_ese As String = esercizioDaEliminare.Progetto_Nome
                If descrizione_ese = "" Then
                    descrizione_ese = "[dal " & esercizioDaEliminare.Validita_Inizio & " al " & esercizioDaEliminare.Validita_Fine & "]"
                End If

                If Not controlloCdG.messaggioSpecifico Then
                    MessaggioErrore = ("Non è possibile eliminare l'impianto " & descrizione_imp & ", perché esistono costi di gestione associati all'esercizio " & descrizione_ese)
                Else
                    MessaggioErrore = ("Non è possibile eliminare l'impianto " & descrizione_imp & ", perché esistono costi di gestione associati all'esercizio " & descrizione_ese & " in data " & controlloCdG.dataCdG)
                End If

                Throw New GiasException(MessaggioErrore)
            Else
                eliminaEsercizio(esercizioDaEliminare, DatiEsercizio, False, objParametri_Server, GiasContext)
            End If
        Next

        Dim DatiImpiantoStr = ""

        If DatiImpianto IsNot Nothing Then
            Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            DatiImpiantoStr = JsonConvert.SerializeObject(DatiImpianto, a)
        End If

        'Scrittura tabella Agronica_Log_Anagrafe
        Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
        Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Impianti,
                                                                                CStr(Piva), CStr(Sa_Cod),
                                                                                CStr(Appezza), CStr(Id_Reg),
                                                                                Nothing, Nothing,
                                                                                TipiEnumerativi.enum_TipoOperazioneDB.Cancellazione,
                                                                                objParametri_Server, enum_Id_Servizio.GiasOnline,
                                                                                "", DatiImpiantoStr, Id_Budget:=Id_Budget)

        ' funzione per cancellare il record in Imprese_Progetti
        GiasContext.Agronica_Log_Anagrafe.Add(log)
        GiasContext.Budget_Reg_Impianti.Remove(impiantoDaEliminare)
        GiasContext.SaveChanges()

    End Sub
    Private Function eliminaEsercizi(Id_budget As Integer,
                                     imp As AgronicaCoreModelsSTD.anagrafiche.Impianto,
                                     objParametri_Server As AgronicaCoreParametri,
                                     objParametri_Utenti As AgronicaCoreParametri,
                                      GiasContext As Gias_DeveloperServer_Entities) As AgronicaCoreModelsSTD.anagrafiche.Impianto

        Dim MessaggioErrore As String = ""
        Dim nomeRoutine = "AngronicaCoreBudgetBiz.Budget_Appezzamento.eliminaEsercizi()
"
        '-----------------------------------------
        ' CANCELLAZIONE ESERCIZI DA CONTROLLO DATE
        '-----------------------------------------
        Dim cod_progetti As New List(Of Integer)
        Dim eserciziDaEliminare As New List(Of AgronicaCoreEntityFramework_POCO.Budget_Imprese_Progetti)
        Dim eserciziCodiciDaEliminare As New List(Of AgronicaCoreEntityFramework_POCO.Budget_Reg_Impianti_Codici)

        Try
            For Each ese In imp.esercizi
                cod_progetti.Add(ese.codice)
            Next

            If cod_progetti.Count > 0 Then
                eserciziDaEliminare = (From ese In GiasContext.Budget_Imprese_Progetti Where
                                                   ese.Id_Budget = Id_budget _
                                                   AndAlso ese.Piva = imp.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva _
                                                   AndAlso ese.Sa_Cod = imp.primaryKey.appezzamentoPK.centroAziendalePK.codice _
                                                   AndAlso ese.Appezza = imp.primaryKey.appezzamentoPK.codice _
                                                   AndAlso ese.Id_Reg = imp.primaryKey.codice _
                                                   AndAlso Not cod_progetti.Contains(ese.Progetto_Cod)
                                       Select ese).ToList()

                If eserciziDaEliminare.Count > 0 Then
                    For Each esercizioDaEliminare In eserciziDaEliminare
                        Dim Progetto_Cod = esercizioDaEliminare.Progetto_Cod
                        Dim DatiEsercizio As AgronicaCoreModelsSTD.anagrafiche.Esercizio
                        If imp IsNot Nothing AndAlso imp.esercizi IsNot Nothing Then
                            'DatiEsercizio = (From de In imp.esercizi Where de.codice = Progetto_Cod).FirstOrDefault
                            Dim objProg As New AgronicaCoreBudgetBIZ.Budget_Progetto_R
                            DatiEsercizio = objProg.Leggi_Esercizio_Anagrafica(Nothing, Id_budget, Progetto_Cod, Nothing, objParametri_Server, objParametri_Utenti)

                        End If
                        eliminaEsercizio(esercizioDaEliminare, DatiEsercizio, True, objParametri_Server, GiasContext)
                    Next
                End If
            End If

        Catch ex As GiasException
            MessaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return imp
    End Function

    Private Sub eliminaEsercizio(esercizioDaEliminare As AgronicaCoreEntityFramework_POCO.Budget_Imprese_Progetti,
                                 DatiEsercizio As AgronicaCoreModelsSTD.anagrafiche.Esercizio,
                                 EseguiControlli As Boolean,
                                 objParametri_Server As AgronicaCoreParametri,
                                 GiasContext As Gias_DeveloperServer_Entities)

        Dim Id_Budget = esercizioDaEliminare.Id_Budget
        Dim piva = esercizioDaEliminare.Piva
        Dim sa_cod = esercizioDaEliminare.Sa_Cod
        Dim appezza = esercizioDaEliminare.Appezza
        Dim id_reg = esercizioDaEliminare.Id_Reg
        Dim progetto_cod = esercizioDaEliminare.Progetto_Cod

        Dim descrizione_distinta As String = ""
        If DatiEsercizio IsNot Nothing Then
            descrizione_distinta = ""
            If DatiEsercizio.lotto <> "" Then
                descrizione_distinta = DatiEsercizio.lotto
            ElseIf DatiEsercizio.validita IsNot Nothing Then
                descrizione_distinta = "con validità dal " & DatiEsercizio.validita.inizio & " al " & DatiEsercizio.validita.fine
            End If
        End If

        If EseguiControlli Then
            'Prima di eliminare controllo Movimenti, Ricette e PUA
            Dim objControllo As New AgronicaCoreAnagrafeBIZ.Progetto_W
            Dim controllo = objControllo.controllo_CdGxEliminazione(DatiEsercizio, piva, sa_cod, appezza, id_reg, progetto_cod, objParametri_Server, Id_Budget:=Id_Budget)
            'controllo.controllo_MovimentiRicettePua(Piva, Sa_Cod, Appezza, Id_Reg, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)
            If controllo.errore Then
                Dim MessaggioErrore As String = ""
                If Not controllo.messaggioSpecifico Then
                    MessaggioErrore = ("Non è possibile eliminare l'esercizio  " & descrizione_distinta & ", perché esistono costi di gestione associati")
                Else
                    MessaggioErrore = ("Non è possibile eliminare l'esercizio " & descrizione_distinta & ", perché esistono costi di gestione associati in data " & controllo.dataCdG)
                End If
                Throw New GiasException(MessaggioErrore)
            End If
        End If



        Dim eserciziCodiciDaEliminare = (From ese In GiasContext.Budget_Reg_Impianti_Codici Where
                                                       ese.Id_Budget = Id_Budget _
                                                       AndAlso ese.PIVA = piva _
                                                       AndAlso ese.sa_cod = sa_cod _
                                                       AndAlso ese.appezza = appezza _
                                                       AndAlso ese.Id_Reg = appezza _
                                                       AndAlso ese.Progetto_Cod = progetto_cod
                                         Select ese).ToList()

        If eserciziCodiciDaEliminare.Count > 0 Then
            GiasContext.Budget_Reg_Impianti_Codici.RemoveRange(eserciziCodiciDaEliminare)
            GiasContext.SaveChanges()
        End If

        Dim DatiEsercizioStr = ""

        If DatiEsercizio IsNot Nothing Then
            Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            DatiEsercizioStr = JsonConvert.SerializeObject(DatiEsercizio, a)
        End If

        'Scrittura tabella Agronica_Log_Anagrafe
        Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
        Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Progetti,
                                                                                CStr(piva), CStr(progetto_cod),
                                                                                CStr(sa_cod), CStr(appezza),
                                                                                CStr(id_reg), Nothing,
                                                                                TipiEnumerativi.enum_TipoOperazioneDB.Cancellazione,
                                                                                objParametri_Server, enum_Id_Servizio.GiasOnline,
                                                                                "", DatiEsercizioStr, Id_Budget:=Id_Budget)

        ' funzione per cancellare il record in Imprese_Progetti
        GiasContext.Agronica_Log_Anagrafe.Add(log)
        GiasContext.Budget_Imprese_Progetti.Remove(esercizioDaEliminare)
        GiasContext.SaveChanges()
    End Sub

    Private Sub Verifica_Superficie(ByRef appezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                    ByRef objParametri_Server As AgronicaCoreParametri)

        Dim MessaggioErrore As String = ""

        If appezzamento.superficie = "0" OrElse appezzamento.superficie = 0 Then
            MessaggioErrore = ("Il valore della SUPERFICIE non può essere nullo.")
            Throw New GiasException(MessaggioErrore)
        ElseIf Not IsNumeric(appezzamento.superficie) Then
            MessaggioErrore = ("Il valore della SUPERFICIE deve essere numerico.")
            Throw New GiasException(MessaggioErrore)
        End If

    End Sub

    Private Sub Internal_ScriviModifica_GIS_Entita_ElementiGrafici_Appezzamento(ByVal dati_appezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                                                                ByRef objParametri As AgronicaCoreParametri,
                                                                                ByVal username As String,
                                                                                Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                                                Optional ByVal NewTransaction As Boolean = True)
        Dim nomeRoutine As String = "AgronicaCoreBudgetBIZ.Budget_Appezzamento_W.Internal_ScriviModifica_GIS_Entita_ElementiGrafici_Appezzamento()"
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
        Try
            Dim modelEntita = New AgronicaCoreGisDAL.Entita
            Dim modelElementoGrafico = New AgronicaCoreGisDAL.ElementiGrafici

            Dim entitaAppezzamentoList = From entApp In GiasContext.GIS_Entita
                                         Where entApp.Piva = dati_appezzamento.primaryKey.centroAziendalePK.partitaIva AndAlso
                                         entApp.Sa_Cod = dati_appezzamento.primaryKey.centroAziendalePK.codice AndAlso
                                         entApp.Appezza = dati_appezzamento.primaryKey.codice AndAlso
                                         entApp.Campo_Cod = dati_appezzamento.campoPK.codice AndAlso
                                         entApp.Id_Imp = 0 AndAlso
                                         entApp.Validita_Inizio <= DateTime.Now AndAlso
                                         entApp.Validita_Fine >= DateTime.Now
                                         Select entApp

            Dim entitaAppezzamento = entitaAppezzamentoList.FirstOrDefault
            If entitaAppezzamento Is Nothing Then
                'creazione
                modelEntita.Piva = dati_appezzamento.primaryKey.centroAziendalePK.partitaIva
                modelEntita.Sa_Cod = dati_appezzamento.primaryKey.centroAziendalePK.codice
                modelEntita.Appezza = dati_appezzamento.primaryKey.codice
                modelEntita.Campo_cod = dati_appezzamento.campoPK.codice
                modelEntita.TipoEntita = enum_GIS2012_TipoEntita.APPEZZAMENTI

                Dim entita = AgronicaCoreGisDAL.EF_GIS_Entita.GIS_Entita_Scrivi_EF(modelEntita, objParametri, username, GiasContext, False)
                modelElementoGrafico.EntitaCod = entita.Entita_Cod
                modelElementoGrafico.Layer = enum_Gis_LayerElementiGrafici_std.APPEZZAMENTI
                modelElementoGrafico.Cartography = If(dati_appezzamento.cartografia Is Nothing, "", dati_appezzamento.cartografia)
                modelElementoGrafico.Flag_GPS = dati_appezzamento.flag_gps

                Dim elementoGrafico = AgronicaCoreGisDAL.EF_GIS_ElementiGrafici.GIS_ElementoGrafico_Scrivi_EF(modelElementoGrafico, objParametri, username, GiasContext, False)
            Else
                modelEntita.PivaSuperUser = entitaAppezzamento.PivaSuperUser
                modelEntita.EntitaCod = entitaAppezzamento.Entita_Cod
                modelEntita.Piva = dati_appezzamento.primaryKey.centroAziendalePK.partitaIva
                modelEntita.Sa_Cod = dati_appezzamento.primaryKey.centroAziendalePK.codice
                modelEntita.Appezza = dati_appezzamento.primaryKey.codice
                modelEntita.Campo_cod = dati_appezzamento.campoPK.codice

                Dim entita = AgronicaCoreGisDAL.EF_GIS_Entita.GIS_Entita_Modifica_EF(modelEntita, objParametri, username, GiasContext, False)


                Dim elementiGraficiList = From elem In GiasContext.GIS_ElementiGrafici
                                          Where elem.PivaSuperUser = modelEntita.PivaSuperUser AndAlso
                                             elem.Entita_Cod = modelEntita.EntitaCod
                                          Select elem

                Dim elementoGrafico = elementiGraficiList.FirstOrDefault
                If elementoGrafico Is Nothing Then
                    modelElementoGrafico.EntitaCod = entita.Entita_Cod
                    modelElementoGrafico.Layer = enum_Gis_LayerElementiGrafici_std.APPEZZAMENTI
                    modelElementoGrafico.Cartography = If(dati_appezzamento.cartografia Is Nothing, "", dati_appezzamento.cartografia)
                    modelElementoGrafico.Flag_GPS = dati_appezzamento.flag_gps
                    Dim ele = AgronicaCoreGisDAL.EF_GIS_ElementiGrafici.GIS_ElementoGrafico_Scrivi_EF(modelElementoGrafico, objParametri, username, GiasContext, False)
                Else
                    modelElementoGrafico.PivaSuperUser = elementoGrafico.PivaSuperUser
                    modelElementoGrafico.ElementoGraficoCod = elementoGrafico.ElementoGrafico_Cod
                    modelElementoGrafico.Cartography = If(dati_appezzamento.cartografia Is Nothing, "", dati_appezzamento.cartografia)
                    modelElementoGrafico.Flag_GPS = dati_appezzamento.flag_gps

                    Dim ele = AgronicaCoreGisDAL.EF_GIS_ElementiGrafici.GIS_ElementoGrafico_Modifica_EF(modelElementoGrafico, objParametri, username, GiasContext, False)
                End If
            End If



            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
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
    Private Sub Internal_ScriviModifica_GIS_Entita_ElementiGrafici_ImpiantoAppezzamento(ByVal dati_impianto As AgronicaCoreModelsSTD.anagrafiche.Impianto,
                                                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                                    ByVal username As String,
                                                                                    Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                                                    Optional ByVal NewTransaction As Boolean = True)
        Dim nomeRoutine As String = "AgronicaCoreBudgetBIZ.Budget_Appezzamento_W.Internal_ScriviModifica_GIS_Entita_ElementiGrafici_ImpiantoAppezzamento()"
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
        Try
            Dim modelEntita = New AgronicaCoreGisDAL.Entita
            Dim modelElementoGrafico = New AgronicaCoreGisDAL.ElementiGrafici
            Dim xMetaschema As New AgronicaCoreMetaSchemaDAL.GruppoVegetale_R

            Dim TipoEntita = enum_GIS2012_TipoEntita.IMPIANTO_GENERICO

            If dati_impianto.utilizzoTerreno.GetType() = GetType(AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta) Then
                Dim Cul_cod As Integer = CType(dati_impianto.utilizzoTerreno, AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta).codice
                Dim gru_cod As Integer = xMetaschema.GruCod_from_Cul_Cod(Cul_cod, objParametri)

                Select Case gru_cod

                    Case 1
                        TipoEntita = enum_GIS2012_TipoEntita.IMPIANTO_ARBOREA
                    Case 2
                        TipoEntita = enum_GIS2012_TipoEntita.IMPIANTI_ERBACEA
                    Case 3
                        TipoEntita = enum_GIS2012_TipoEntita.IMPIANTI_ORTICOLA

                    Case Else
                        TipoEntita = enum_GIS2012_TipoEntita.IMPIANTO_GENERICO

                End Select
            End If


            Dim entitaImpiantoList = From entImp In GiasContext.GIS_Entita
                                     Where entImp.Piva = dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva AndAlso
                                         entImp.Sa_Cod = dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice AndAlso
                                         entImp.Appezza = dati_impianto.primaryKey.appezzamentoPK.codice AndAlso
                                         entImp.Campo_Cod = 0 AndAlso
                                         entImp.Id_Imp = dati_impianto.primaryKey.codice AndAlso
                                         entImp.Validita_Inizio <= DateTime.Now AndAlso
                                         entImp.Validita_Fine >= DateTime.Now
                                     Select entImp

            Dim entitaImpianto = entitaImpiantoList.FirstOrDefault
            If entitaImpianto Is Nothing Then
                'creazione
                modelEntita.Piva = dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva
                modelEntita.Sa_Cod = dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice
                modelEntita.Appezza = dati_impianto.primaryKey.appezzamentoPK.codice
                modelEntita.Id_Imp = dati_impianto.primaryKey.codice
                modelEntita.Campo_cod = 0
                modelEntita.TipoEntita = TipoEntita

                Dim entita = AgronicaCoreGisDAL.EF_GIS_Entita.GIS_Entita_Scrivi_EF(modelEntita, objParametri, username, GiasContext, False)
                If (entita IsNot Nothing) Then
                    modelElementoGrafico.EntitaCod = entita.Entita_Cod
                    modelElementoGrafico.Layer = enum_Gis_LayerElementiGrafici_std.IMPIANTI
                    modelElementoGrafico.Cartography = If(dati_impianto.cartografia Is Nothing, "", dati_impianto.cartografia)
                    modelElementoGrafico.Flag_GPS = dati_impianto.flag_gps

                    Dim elementoGrafico = AgronicaCoreGisDAL.EF_GIS_ElementiGrafici.GIS_ElementoGrafico_Scrivi_EF(modelElementoGrafico, objParametri, username, GiasContext, False)
                End If
            Else
                modelEntita.PivaSuperUser = entitaImpianto.PivaSuperUser
                modelEntita.EntitaCod = entitaImpianto.Entita_Cod
                modelEntita.Piva = dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva
                modelEntita.Sa_Cod = dati_impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice
                modelEntita.Appezza = dati_impianto.primaryKey.appezzamentoPK.codice
                modelEntita.Campo_cod = 0

                Dim entita = AgronicaCoreGisDAL.EF_GIS_Entita.GIS_Entita_Modifica_EF(modelEntita, objParametri, username, GiasContext, False)


                Dim elementiGraficiList = From elem In GiasContext.GIS_ElementiGrafici
                                          Where elem.PivaSuperUser = modelEntita.PivaSuperUser AndAlso
                                             elem.Entita_Cod = modelEntita.EntitaCod
                                          Select elem

                Dim elementoGrafico = elementiGraficiList.FirstOrDefault
                If elementoGrafico Is Nothing Then
                    modelElementoGrafico.EntitaCod = entita.Entita_Cod
                    modelElementoGrafico.Layer = enum_Gis_LayerElementiGrafici_std.IMPIANTI
                    modelElementoGrafico.Cartography = If(dati_impianto.cartografia Is Nothing, "", dati_impianto.cartografia)
                    modelElementoGrafico.Flag_GPS = dati_impianto.flag_gps
                    Dim ele = AgronicaCoreGisDAL.EF_GIS_ElementiGrafici.GIS_ElementoGrafico_Scrivi_EF(modelElementoGrafico, objParametri, username, GiasContext, False)
                Else
                    modelElementoGrafico.PivaSuperUser = elementoGrafico.PivaSuperUser
                    modelElementoGrafico.ElementoGraficoCod = elementoGrafico.ElementoGrafico_Cod
                    modelElementoGrafico.Cartography = If(dati_impianto.cartografia Is Nothing, "", dati_impianto.cartografia)
                    modelElementoGrafico.Flag_GPS = dati_impianto.flag_gps

                    Dim ele = AgronicaCoreGisDAL.EF_GIS_ElementiGrafici.GIS_ElementoGrafico_Modifica_EF(modelElementoGrafico, objParametri, username, GiasContext, False)
                End If
            End If



            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
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
    Private Function Internal_Scrivi_Impianti_da_Appezzamento(ByRef Id_Budget As Integer,
                                                             ByRef imp As AgronicaCoreModelsSTD.anagrafiche.Impianto,
                                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                             ByRef objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                             ByVal username As String,
                                                             Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                             Optional ByVal NewTransaction As Boolean = True
                                                             ) As AgronicaCoreEntityFramework_POCO.Budget_Reg_Impianti
        Dim nomeRoutine As String = "AgronicaCoreBudgetBIZ.Budget_Appezzamento_W.Internal_Scrivi_Impianti_da_Appezzamento()"
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

        Dim ret As AgronicaCoreEntityFramework_POCO.Budget_Reg_Impianti = Nothing
        Dim verificaImpianto = New AgronicaCoreBudgetBIZ.Budget_Reg_Impianto_W

        Try
            'verificaImpianto.Verifica_ValiditaInizioFine(imp, objParametri, objParametriUtenti)

            'LL - 26/01/2022 da fare solo se viene specificata specie\varieta...
            'COMMENTATO PERCHE' IN INSERIMENTO NON DEVO FARE QUESTI CONTROLLI, visto dal corrispetivo non budget
            'If (imp.utilizzoTerreno.classType = ClassType.Varieta) Then
            '    verificaImpianto.Verifica_Utilizzo(Id_Budget, imp, objParametri)
            '    verificaImpianto.Verifica_Finalita(imp, objParametri)
            'End If

            'LL - 28/12/2021 non servono.....
            'verificaImpianto.Verifica_Utilizzo(imp, objParametri)
            'verificaImpianto.Verifica_CodiceImpianto(imp, objParametri, objParametri)
            'verificaImpianto.Verifica_Superficie(imp, objParametri)

            ret = AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.Impianto_Scrivi_EF(Id_Budget,
                                                                            imp,
                                                                            objParametri,
                                                                            objParametriUtenti,
                                                                            username,
                                                                            GiasContext,
                                                                            NewTransaction)

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

        Return ret

    End Function
    Private Function Internal_Scrivi_Esercizi_da_Impianto_Appezzamento(ByRef Id_budget As Integer,
                                                                        ByRef ese As AgronicaCoreModelsSTD.anagrafiche.Esercizio,
                                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                        ByVal username As String,
                                                                        Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                                        Optional ByVal NewTransaction As Boolean = True
                                                                      ) As AgronicaCoreEntityFramework_POCO.Budget_Imprese_Progetti
        Dim nomeRoutine As String = "AgronicaCoreBudgetBIZ.Budget_Appezzamento_W.Internal_Scrivi_EserciziCodici_da_Impianto_Appezzamento()"
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

        Dim objImpreseCodici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read

        Dim algoritmo_codifica = objImpreseCodici.Leggi_Codice_from_Imprese_Codici(ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                                                   enum_CodiciAnagrafe.Algoritmo_Codifica,
                                                                                                   objParametri)

        Dim ret As AgronicaCoreEntityFramework_POCO.Budget_Imprese_Progetti = Nothing
        Dim verificaEsercizio = New AgronicaCoreAnagrafeBIZ.Progetto_W

        Try
            'verificaEsercizio.Verifica_ValiditaInizioFine(ese, objParametri)

            If algoritmo_codifica <> "" AndAlso String.IsNullOrEmpty(ese.lotto) Then
                ese.lotto = AgronicaCoreAnagrafeBIZ.Replica_GIAS.LeggiCodiceProgressivo(ese.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                                               enum_SequenzaProgressiviTipi.CodiciOPAgriZoo,
                                                               ese.validita.inizio.Year,
                                                               objParametri)
            End If

            ret = AgronicaCoreBudgetDAL.Budget_EFEsercizi.Esercizio_Scrivi_EF(Id_budget,
                                                                        ese,
                                                                        objParametri,
                                                                        username,
                                                                        GiasContext,
                                                                        NewTransaction)
            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
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

        Return ret

    End Function

    Public Function Internal_Modifica_Esercizi_da_Impianto_Appezzamento(ByRef Id_Budget As Integer,
                                                                        ByRef ese As AgronicaCoreModelsSTD.anagrafiche.Esercizio,
                                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                        ByVal username As String,
                                                                        Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                                        Optional ByVal NewTransaction As Boolean = True
                                                                      ) As AgronicaCoreEntityFramework_POCO.Budget_Imprese_Progetti

        Dim nomeRoutine As String = "AgronicaCoreBudgetBIZ.Budget_Appezzamento_W.Internal_Modifica_Esercizi_da_Impianto_Appezzamento()"
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

        Dim ret As AgronicaCoreEntityFramework_POCO.Budget_Imprese_Progetti = Nothing
        Dim verificaEsercizio = New AgronicaCoreBudgetBIZ.Budget_Progetto_W

        Try
            verificaEsercizio.Verifica_ValiditaInizioFine(Id_Budget, ese, objParametri)
            ret = AgronicaCoreBudgetDAL.Budget_EFEsercizi.Esercizio_Modifica_EF(Id_Budget,
                                                                                ese,
                                                                        objParametri,
                                                                        username,
                                                                        GiasContext,
                                                                        NewTransaction)
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

        Return ret

    End Function

    Private Function Internal_Modifica_Impianti_da_Appezzamento(ByRef Id_budget As Integer,
                                                                ByRef imp As AgronicaCoreModelsSTD.anagrafiche.Impianto,
                                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                ByVal username As String,
                                                                Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                                Optional ByRef NewTransaction As Boolean = True
                                                              ) As AgronicaCoreEntityFramework_POCO.Budget_Reg_Impianti

        Dim nomeRoutine As String = "AgronicaCoreBudgetBIZ.Budget_Appezzamento_W.Internal_Modifica_Impianti_da_Appezzamento()"
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

        Dim ret As AgronicaCoreEntityFramework_POCO.Budget_Reg_Impianti = Nothing
        Dim verificaImpianto = New AgronicaCoreBudgetBIZ.Budget_Reg_Impianto_W

        Try
            verificaImpianto.Verifica_ValiditaInizioFine(Id_budget, imp, objParametri, objParametri)
            'LL - 28/12/2021 non deve essere eseguito per gli impianti con Destinazione D'uso
            If (imp.utilizzoTerreno IsNot Nothing) Then
                If (imp.utilizzoTerreno.classType = ClassType.Varieta) Then
                    verificaImpianto.Verifica_Utilizzo(Id_budget, imp, objParametri)
                    verificaImpianto.Verifica_Finalita(imp, objParametri)
                End If
            End If
            'verificaImpianto.Verifica_CodiceImpianto(imp, objParametri, objParametri)
            verificaImpianto.Verifica_Superficie(Id_budget, imp, objParametri, GiasContext, NewTransaction)

            ret = AgronicaCoreBudgetDAL.Budget_EFReg_Impianti.Impianto_Modifica_EF(Id_budget,
                                                                            imp,
                                                                            objParametri,
                                                                            username,
                                                                            GiasContext,
                                                                            NewTransaction)
            If NewTransaction Then
                If scope IsNot Nothing Then
                    scope.Complete()
                    scope.Dispose()
                End If
            End If
        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return ret

    End Function
    Private Shared Sub GeneraGMapJPG(ByVal EntitaCod As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Dim leggiCFGStaticMap As New Configurazione_Siti_R
        Dim jSonStaticMapCFG As String = leggiCFGStaticMap.Leggi_Valore(0, "GIS_StaticMapCFG", "", "", objParametri)
        Dim StaticMapCFG As GeneraMappaStaticaInData = JsonConvert.DeserializeObject(Of GeneraMappaStaticaInData)(jSonStaticMapCFG)
        StaticMapCFG.EntitaCod = EntitaCod

        Dim gestioneStaticMaps As New GoogleStaticMaps

        StaticMapCFG.objParametri_Server = objParametri
        gestioneStaticMaps.AggiornaElementoGraficoConMappaStatica(StaticMapCFG)
    End Sub


End Class

