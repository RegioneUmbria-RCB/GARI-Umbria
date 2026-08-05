Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Xml
Imports AgronicaCoreDTOStd.InData.Budget
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreDataProvider.My.Resources
Imports enum_TipoOperazioneDB = AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB
Imports AgronicaCoreDTOStd.InData.Metaschema
Imports AgronicaCoreModelsSTD.exceptions


' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class Campi1
    Inherits System.Web.Services.WebService

    <WebMethod()>
    Public Function HelloWorld() As String
        Return "Hello World"
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_AppezzamentiCampo_NG(InData As CoreWS_Generic(Of BudgetAnagrafica(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG))) As RispostaStandard
        Dim r As New RispostaStandard
        Dim DT As DataTable

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objAppezzamentoR As New AgronicaCoreBudgetDAL.Budget_Appezzamento_R

            ' serve per ottenere una corretta lettura degli appezzamenti
            Dim data = New Date(1, 1, 1)

            DT = objAppezzamentoR.Recupera_Appezzamenti_Colture_del_Campo(
                                                         InData.InData.Id_Budget,
                                                         InData.InData.ElementoAnagrafico.Piva,
                                                         InData.InData.ElementoAnagrafico.Sa_Cod,
                                                         InData.InData.ElementoAnagrafico.Campo_Cod,
                                                         If(InData.InData.ElementoAnagrafico.Validita_Inizio = data, CostantiPersonalizzate.AGRODATAINIZIO, InData.InData.ElementoAnagrafico.Validita_Inizio),
                                                         If(InData.InData.ElementoAnagrafico.Validita_Fine = data, CostantiPersonalizzate.AGRODATAFINE, InData.InData.ElementoAnagrafico.Validita_Fine),
                                                         True,
                                                         enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                         "",
                                                         "",
                                                         objParametri_Server)

            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("PIVA", "PIVA", "string") With {._hidden = True})
            l.Add(New ColonneNome("SA_COD", "SA_COD", "string") With {._hidden = True})
            l.Add(New ColonneNome("Campo_Cod", "Campo_Cod", "string") With {._hidden = True})
            l.Add(New ColonneNome("APPEZZA", "APPEZZA", "string"))
            l.Add(New ColonneNome("SUP_APP", "SUP_APP", "string"))
            l.Add(New ColonneNome("APP_NOME", "APP_NOME", "string"))
            l.Add(New ColonneNome("Validita_Inizio", Gias.ValiditaInizio, "date"))
            l.Add(New ColonneNome("Validita_Fine", Gias.ValiditaFine, "date"))
            l.Add(New ColonneNome("ID_REG", "ID_REG", "string"))
            l.Add(New ColonneNome("Impianto_Validita_Inizio", Gias.InizioImpianto, "date"))
            l.Add(New ColonneNome("Impianto_Validita_Fine", Gias.FineImpianto, "date"))
            l.Add(New ColonneNome("CUL_COD", "CUL_COD", "string"))
            l.Add(New ColonneNome("Cul_Des", "Cul_Des", "string"))
            l.Add(New ColonneNome("Veg_des", "Veg_des", "string"))


            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = js.JSON_DataTable_Kendo(DT, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)
            r.RispostaOK = True
            r.RispostaStringa = risp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_AppezzamentiCampo(ByVal objP_super_server As String,
                                            ByVal objP_server As String,
                                            ByVal objP_utenti As String,
                                            ByVal InData As BudgetAnagrafica(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)) As RispostaStandard
        Dim r As New RispostaStandard
        Dim DT As DataTable

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim objAppezzamentoR As New AgronicaCoreBudgetDAL.Budget_Appezzamento_R

            ' serve per ottenere una corretta lettura degli appezzamenti
            Dim data = New Date(1, 1, 1)

            DT = objAppezzamentoR.Recupera_Appezzamenti_Colture_del_Campo(
                                                         InData.Id_Budget,
                                                         InData.ElementoAnagrafico.Piva,
                                                         InData.ElementoAnagrafico.Sa_Cod,
                                                         InData.ElementoAnagrafico.Campo_Cod,
                                                         If(InData.ElementoAnagrafico.Validita_Inizio = data, CostantiPersonalizzate.AGRODATAINIZIO, InData.ElementoAnagrafico.Validita_Inizio),
                                                         If(InData.ElementoAnagrafico.Validita_Fine = data, CostantiPersonalizzate.AGRODATAFINE, InData.ElementoAnagrafico.Validita_Fine),
                                                         True,
                                                         enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                         "",
                                                         "",
                                                         objParametri_Server)

            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("PIVA", "PIVA", "string") With {._hidden = True})
            l.Add(New ColonneNome("SA_COD", "SA_COD", "string") With {._hidden = True})
            l.Add(New ColonneNome("Campo_Cod", "Campo_Cod", "string") With {._hidden = True})
            l.Add(New ColonneNome("APPEZZA", "APPEZZA", "string"))
            l.Add(New ColonneNome("SUP_APP", "SUP_APP", "string"))
            l.Add(New ColonneNome("APP_NOME", "APP_NOME", "string"))
            l.Add(New ColonneNome("Validita_Inizio", Gias.ValiditaInizio, "date"))
            l.Add(New ColonneNome("Validita_Fine", Gias.ValiditaFine, "date"))
            l.Add(New ColonneNome("ID_REG", "ID_REG", "string"))
            l.Add(New ColonneNome("Impianto_Validita_Inizio", Gias.InizioImpianto, "date"))
            l.Add(New ColonneNome("Impianto_Validita_Fine", Gias.FineImpianto, "date"))
            l.Add(New ColonneNome("CUL_COD", "CUL_COD", "string"))
            l.Add(New ColonneNome("Cul_Des", "Cul_Des", "string"))
            l.Add(New ColonneNome("Veg_des", "Veg_des", "string"))


            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = js.JSON_DataTable_Kendo(DT, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)
            r.RispostaOK = True
            r.RispostaStringa = risp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Campi_Anagrafica(InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim settings As New JsonSerializerSettings()
        settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

        Dim iData As CoreWS_Generic(
            Of BudgetAnagrafica(Of Parametri_ObjParametriAgenda_NG)) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of BudgetAnagrafica(Of Parametri_ObjParametriAgenda_NG)))(JsonConvert.SerializeObject(InData), settings)

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objCampi As New AgronicaCoreBudgetDAL.Budget_Campi_R
            Dim objAppezzamento As New AgronicaCoreBudgetDAL.Budget_Appezzamento_R
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)
            'Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

            'If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            '    r.Sessione = False
            '    Return r
            'End If

            Dim objParametriAgenda = iData.InData

            Dim FinestraTemporaleInizio = objParametri_Server.FinestraTemporaleInizio
            Dim FinestraTemporaleFine = objParametri_Server.FinestraTemporaleFine

            If objParametriAgenda.ElementoAnagrafico.Data <> AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then
                objParametri_Server.FinestraTemporaleInizio = objParametriAgenda.ElementoAnagrafico.Data.Date
                objParametri_Server.FinestraTemporaleFine = objParametriAgenda.ElementoAnagrafico.Data.Date
            End If

            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim leggiDatiRibaltamento As Boolean = objPermessi.Controlla_Permessi_Utente(objParametri_Utenti.UtenteUsername,
                                                                                         enum_Id_Servizio.GiasOnline, enum_Security_Attivita.Budget_Ribaltamento_Su_Reale, enum_Security_Operazione.Lettura,
                                                                                         Date.Now, "", objParametri_Utenti)

            Dim dt As DataTable = objCampi.Leggi_x_anagrafica(objParametriAgenda.Id_Budget, objParametriAgenda.ElementoAnagrafico.Piva, objParametriAgenda.ElementoAnagrafico.Sa_Cod, 0, "", "", objParametri_Server, leggiDatiRibaltamento)

            If objParametriAgenda.ElementoAnagrafico.Data <> AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then
                objParametri_Server.FinestraTemporaleInizio = objParametriAgenda.ElementoAnagrafico.Data.Date
                objParametri_Server.FinestraTemporaleFine = objParametriAgenda.ElementoAnagrafico.Data.Date
            End If

            'dt.Columns.AddRange({New DataColumn("Superficie_Totale", Type.GetType("System.Decimal")),
            '                           New DataColumn("Superficie_Convenzionale", Type.GetType("System.Decimal")),
            '                           New DataColumn("Superficie_Biologico", Type.GetType("System.Decimal")),
            '                           New DataColumn("Superficie_Conversione", Type.GetType("System.String")),
            '                           New DataColumn("Superficie_Catastale", Type.GetType("System.String"))
            '                          })

            '' aggiungo codici campo
            'Dim codiciCampo As New Dictionary(Of String, String)
            'LeggiCodiciCampi(objParametriAgenda.Id_Budget, objParametriAgenda.ElementoAnagrafico.Piva, codiciCampo, objParametri_Server)
            'dt.Columns.Add(New DataColumn("rif_alfanumerico", Type.GetType("System.String")))
            'dt.Columns.Add(New DataColumn("sup_contratto", Type.GetType("System.String")))
            'dt.Columns.Add(New DataColumn("filiera", Type.GetType("System.String")))

            'For i As Integer = 0 To dt.Rows.Count - 1

            '    'Recupero le Superfici
            '    Dim Sup_Totale, Sup_Tare, SAU_Totale, SAU_Biologico, SAU_Conversione, SAU_Convenzionale, SAU_Catastale As Double
            '    If Not IsDBNull(dt.Rows(i).Item("Piva")) AndAlso dt.Rows(i).Item("Piva") <> "" AndAlso Not IsDBNull(dt.Rows(i).Item("sa_cod")) AndAlso IsNumeric(dt.Rows(i).Item("sa_cod")) Then
            '        objAppezzamento.Recupera_Superfici_Campo(dt.Rows(i).Item("Id_Budget"), dt.Rows(i).Item("Piva"), dt.Rows(i).Item("sa_cod"), dt.Rows(i).Item("Campo_Cod"),
            '                                        Sup_Totale, SAU_Biologico, SAU_Conversione, SAU_Convenzionale, SAU_Catastale, "", "", objParametri_Server)

            '        dt.Rows(i).Item("Superficie_Totale") = Format(Sup_Totale, "0.0000")
            '        dt.Rows(i).Item("Superficie_Biologico") = Format(SAU_Biologico, "0.0000")
            '        dt.Rows(i).Item("Superficie_Convenzionale") = Format(SAU_Convenzionale, "0.0000")
            '        dt.Rows(i).Item("Superficie_Conversione") = Format(SAU_Conversione, "0.0000")
            '        dt.Rows(i).Item("Superficie_Catastale") = Format(SAU_Catastale, "0.0000")

            '        dt.Rows(i).Item("rif_alfanumerico") = GetCodiceCampo("rif_alfanumerico", dt.Rows(i).Item("sa_cod"), dt.Rows(i).Item("Campo_Cod"), codiciCampo)
            '        dt.Rows(i).Item("sup_contratto") = GetCodiceCampo("sup_contratto", dt.Rows(i).Item("sa_cod"), dt.Rows(i).Item("Campo_Cod"), codiciCampo)
            '        dt.Rows(i).Item("filiera") = GetCodiceCampo("filiera", dt.Rows(i).Item("sa_cod"), dt.Rows(i).Item("Campo_Cod"), codiciCampo)

            '    End If

            'Next

            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("chiave", "Nome", "string") With {._hidden = True})
            'If objParametriAgenda.Sa_Cod = 0 Then
            l.Add(New ColonneNome("sa_nome", Gias.Centro, "string"))
            l.Add(New ColonneNome("sa_cod", "sa_cod", "number") With {._hidden = True})
            'End If
            l.Add(New ColonneNome("Campo", Gias.Campo, "string"))
            l.Add(New ColonneNome("Validita_Inizio", Gias.ValiditaInizio, "date"))
            l.Add(New ColonneNome("Validita_Fine", Gias.ValiditaFine, "date"))
            l.Add(New ColonneNome("Gru_Des", Gias.GruppoVegetale, "string"))
            l.Add(New ColonneNome("Gru_Cod", "Gru_Cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("Veg_Des", Gias.Specie, "string"))
            l.Add(New ColonneNome("Veg_Cod", "Veg_Cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("Superficie_Totale", Gias.SuperficieTotale, "number"))
            l.Add(New ColonneNome("Superficie_Biologico", Gias.SuperficieBiologico, "number"))
            l.Add(New ColonneNome("Superficie_Convenzionale", Gias.SuperficieConvenzionale, "number"))
            l.Add(New ColonneNome("Superficie_Conversione", Gias.SuperficieConversione, "number"))
            l.Add(New ColonneNome("Superficie_Catastale", Gias.SuperficieCatastale, "number"))

            l.Add(New ColonneNome("rif_alfanumerico", Gias.CodiceCampo, "string"))
            l.Add(New ColonneNome("sup_contratto", Gias.SupContratto, "string"))
            l.Add(New ColonneNome("filiera", Gias.Filiera, "string"))

            l.Add(New ColonneNome("Attivo", Gias.Attivo, "number"))

            l.Add(New ColonneNome("Data_Creazione", "Data Creazione", "date") With {._hidden = True})
            l.Add(New ColonneNome("Utente_Creazione", "Utente Creazione", "string") With {._hidden = True})
            l.Add(New ColonneNome("Data_Modifica", "Data Modifica", "date") With {._hidden = True})
            l.Add(New ColonneNome("Utente_Modifica", "Utente Modifica", "string") With {._hidden = True})

            If leggiDatiRibaltamento Then
                l.Add(New ColonneNome("Ribaltato", "Ribaltato", "string") With {._Display = True})
                l.Add(New ColonneNome("Data_Ribaltamento", "Data Ribaltamento", "date") With {._Display = True})
            End If

            Dim serializerSettings As New JsonSerializerSettings()

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None, serializerSettings)

            'Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            'Dim risp As String = js.JSON_DataTable_Kendo(dt, l, False, False, TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa, "")
            'r.RispostaOK = True
            'r.RispostaStringa = risp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_Campo_Anagrafica(
                                          ByVal InData As CoreWS_Generic(
                                            Of BudgetAnagrafica(Of Parametri_ObjParametriAgenda_NG))
                                          ) As rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.Campo)

        Dim r As New rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.Campo)

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objCampi As New AgronicaCoreBudgetBIZ.Budget_Campo_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim c As AgronicaCoreModelsSTD.anagrafiche.Campo

            c = objCampi.Leggi_Campo(InData.InData.Id_Budget, InData.InData.ElementoAnagrafico.Piva,
                                         InData.InData.ElementoAnagrafico.Sa_Cod,
                                         InData.InData.ElementoAnagrafico.Campo_Cod,
                                         objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = c

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function LeggiCampi(ByVal InData As CoreWS_Generic(Of BudgetAnagrafica(Of
                                            AgronicaCoreDTOStd.InData.Metaschema.LeggiCampi))) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.Campo))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.Campo))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objCampi As New AgronicaCoreBudgetDAL.Budget_Campi_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(InData.InData.ElementoAnagrafico.data, InData.InData.ElementoAnagrafico.data)


            Dim dt = objCampi.Leggi_x_anagrafica(InData.InData.Id_Budget,
                                                 InData.InData.ElementoAnagrafico.centro.primaryKey.partitaIva,
                                                 InData.InData.ElementoAnagrafico.centro.primaryKey.codice,
                                                 0,
                                                 "", "", objParametri_Server)

            Dim c = (From row In dt.Rows Select New AgronicaCoreModelsSTD.anagrafiche.Campo() With {
                                             .primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Campo.PK(
                                                        row("campo_cod"),
                                                        New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(row("chiave").ToString().Split("_")(2),
                                                                                                                 row("chiave").ToString().Split("_")(1))
                                             ),
                                             .descrizione = row("Campo")}
                                             ).ToList()

            r.RispostaOK = True
            r.RispostaStringa = c

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_ParticelleCampo_NG(InData As CoreWS_Generic(Of BudgetAnagrafica(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG))) As RispostaStandard

        Dim r As New RispostaStandard
        Dim DT As DataTable

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim Dt_CXP As DataTable

            Dim objParticelleCatastali As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R
            DT = objParticelleCatastali.Recupera_Particelle_per_Centro(
                InData.InData.ElementoAnagrafico.Piva,
                InData.InData.ElementoAnagrafico.Sa_Cod,
                InData.InData.ElementoAnagrafico.Validita_Inizio,
                InData.InData.ElementoAnagrafico.Validita_Fine,
                objParametri_Server,
                True,
                InData.InData.Id_Budget
                )

            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("Prov", "Prov", "string"))
            l.Add(New ColonneNome("Com", "Com", "string"))
            l.Add(New ColonneNome("COMUNI_PROV", "COMUNI_PROV", "string") With {._hidden = True})
            l.Add(New ColonneNome("LOCALITA", Gias.Localita, "string"))
            l.Add(New ColonneNome("Part_Cod", "Part_Cod", "number"))
            l.Add(New ColonneNome("Sezione", Gias.Sezione, "string"))
            l.Add(New ColonneNome("Foglio", Gias.Foglio, "number"))
            l.Add(New ColonneNome("Numero", Gias.Numero, "number"))
            l.Add(New ColonneNome("Subalterno", Gias.Subalterno, "string"))
            l.Add(New ColonneNome("Superficie", Gias.Superficie, "number"))
            l.Add(New ColonneNome("Particella_ettari", "Particella_ettari", "number"))
            l.Add(New ColonneNome("Particella_are", "Particella_are", "number"))
            l.Add(New ColonneNome("Particella_centiare", "Particella_centiare", "number"))
            l.Add(New ColonneNome("Sup_Condotta", "Sup_Condotta" + Gias.SuperficieCondottaAbbr, "number"))
            l.Add(New ColonneNome("SuperficieDisponibile", Gias.SuperficieDisponibile, "number"))
            l.Add(New ColonneNome("SuperficieUtilizzata", Gias.SuperficieUtilizzata, "number"))
            l.Add(New ColonneNome("AreaSuAppLiberi", "AreaSuAppLiberi", "number"))
            l.Add(New ColonneNome("AreaSuCampiSquadri", "AreaSuCampiSquadri", "number"))
            l.Add(New ColonneNome("AreaSuAppSuCampiNonSquadri", "AreaSuAppSuCampiNonSquadri", "number"))

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = js.JSON_DataTable_Kendo(DT, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)
            r.RispostaOK = True
            r.RispostaStringa = risp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_ParticelleCampo(ByVal objP_super_server As String,
                                          ByVal objP_server As String,
                                          ByVal objP_utenti As String,
                                          ByVal InData As BudgetAnagrafica(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)) As RispostaStandard
        Dim r As New RispostaStandard
        Dim DT As DataTable

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)


            Dim Dt_CXP As DataTable

            Dim objParticelleCatastali As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R
            DT = objParticelleCatastali.Recupera_Particelle_per_Centro(
                InData.ElementoAnagrafico.Piva,
                InData.ElementoAnagrafico.Sa_Cod,
                InData.ElementoAnagrafico.Validita_Inizio,
                InData.ElementoAnagrafico.Validita_Fine,
                objParametri_Server,
                True,
                InData.Id_Budget
                )

            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("Prov", "Prov", "string"))
            l.Add(New ColonneNome("Com", "Com", "string"))
            l.Add(New ColonneNome("COMUNI_PROV", "COMUNI_PROV", "string") With {._hidden = True})
            l.Add(New ColonneNome("LOCALITA", Gias.Localita, "string"))
            l.Add(New ColonneNome("Part_Cod", "Part_Cod", "number"))
            l.Add(New ColonneNome("Sezione", Gias.Sezione, "string"))
            l.Add(New ColonneNome("Foglio", Gias.Foglio, "number"))
            l.Add(New ColonneNome("Numero", Gias.Numero, "number"))
            l.Add(New ColonneNome("Subalterno", Gias.Subalterno, "string"))
            l.Add(New ColonneNome("Superficie", Gias.Superficie, "number"))
            l.Add(New ColonneNome("Particella_ettari", "Particella_ettari", "number"))
            l.Add(New ColonneNome("Particella_are", "Particella_are", "number"))
            l.Add(New ColonneNome("Particella_centiare", "Particella_centiare", "number"))
            l.Add(New ColonneNome("Sup_Condotta", "Sup_Condotta" + Gias.SuperficieCondottaAbbr, "number"))
            l.Add(New ColonneNome("SuperficieDisponibile", Gias.SuperficieDisponibile, "number"))
            l.Add(New ColonneNome("SuperficieUtilizzata", Gias.SuperficieUtilizzata, "number"))

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = js.JSON_DataTable_Kendo(DT, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)
            r.RispostaOK = True
            r.RispostaStringa = risp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function ScriviCampiAnagrafica_NG(InData As CoreWS_Generic(Of ScriviCampiAnagrafica)) As RispostaStandard
        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objCampoBIZ As New AgronicaCoreBudgetBIZ.Budget_Campo_W

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim listErroriGias As New List(Of ErroreGias)

            Dim objCampo As BudgetAnagrafica(Of Campo) = InData.InData.InData
            Dim risp = objCampoBIZ.Scrivi_Campo_Anagrafica(InData.InData.InData.Id_Budget,
                                                           objCampo.ElementoAnagrafico,
                                                           InData.InData.tipoOperazione,
                                                           objParametri_Server,
                                                           objParametri_Utenti,
                                                           objCampo.Delete_Reale_From_Ribaltamento,
                                                           listErroriGias:=listErroriGias)

            If objCampo.Delete_Reale_From_Ribaltamento Then
                r.ErroriGias.AddRange(listErroriGias)
            End If

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(InData, Newtonsoft.Json.Formatting.None)


        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = JsonConvert.SerializeObject(InData, Newtonsoft.Json.Formatting.None)
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}

        End Try

        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function ScriviCampiAnagrafica(
                                         ByVal InData As BudgetAnagrafica(Of Campo),
                                         ByVal tipoOperazione As AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB,
                                         ByVal objP_super_server As String,
                                         ByVal objP_server As String,
                                         ByVal objP_utenti As String) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objCampoBIZ As New AgronicaCoreBudgetBIZ.Budget_Campo_W

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim objCampo As BudgetAnagrafica(Of Campo) = InData
            Dim risp = objCampoBIZ.Scrivi_Campo_Anagrafica(InData.Id_Budget,
                                                            objCampo.ElementoAnagrafico,
                                                            tipoOperazione,
                                                            objParametri_Server,
                                                            objParametri_Utenti
                                                            )


            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(InData, Newtonsoft.Json.Formatting.None)


        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = JsonConvert.SerializeObject(InData, Newtonsoft.Json.Formatting.None)
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}

        End Try

        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function ScriviCampiAnagrafica_inLine(
                                         ByVal InData As BudgetAnagrafica(Of Campo),
                                         ByVal tipoOperazione As AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB,
                                         ByVal objP_super_server As String,
                                         ByVal objP_server As String,
                                         ByVal objP_utenti As String) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim objCampo As AgronicaCoreModelsSTD.anagrafiche.Campo
            Dim objCampoBIZ As New AgronicaCoreBudgetBIZ.Budget_Campo_W

            Dim campo = InData.ElementoAnagrafico
            If tipoOperazione <> TipiEnumerativi.enum_TipoOperazioneDB.Scrittura Then
                Dim id_Budget = InData.Id_Budget
                Dim objCampi As New AgronicaCoreBudgetBIZ.Budget_Campo_R
                Dim campo_old = objCampi.Leggi_Campo(
                    id_Budget,
                    campo.primaryKey.centroAziendalePK.partitaIva,
                    campo.primaryKey.centroAziendalePK.codice,
                    campo.primaryKey.codice,
                    objParametri_Server
                    )

                campo_old.descrizione = campo.descrizione
                campo_old.orientamento_Colturale = campo.orientamento_Colturale
                campo_old.specie = campo.specie
                campo_old.validita = campo.validita
                campo_old.campo_Codice = campo.campo_Codice
                objCampo = campo_old
            Else
                objCampo = campo
            End If

            Dim risp = objCampoBIZ.Scrivi_Campo_Anagrafica(
                InData.Id_Budget,
                objCampo,
                tipoOperazione,
                objParametri_Server,
                objParametri_Utenti
                )

            r.RispostaOK = True
            r.RispostaStringa = risp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    '##############################################################################################
    Private Shared Sub LeggiCodiciCampi(ByVal Id_Budget As Integer, ByVal Piva As String, ByRef codici As Dictionary(Of String, String), ByRef objParametri_Server As AgronicaCoreParametri)

        Dim objCampoCodici As New AgronicaCoreBudgetDAL.Budget_Campi_codici_R
        Dim dtCodCampo = objCampoCodici.Leggi2(Id_Budget, Piva, 0, 0, enum_CodiciAnagrafe.Riferimento_Alfanumerico_Campo, "", AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO, AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
        Dim dtSupContratto = objCampoCodici.Leggi2(Id_Budget, Piva, 0, 0, enum_CodiciAnagrafe.Sup_Contratto, "", AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO, AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
        Dim dtFiliera = objCampoCodici.Leggi2(Id_Budget, Piva, 0, 0, enum_CodiciAnagrafe.Filiera, "", AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO, AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        For Each codice In dtCodCampo.Rows
            Dim chiave = "rif_alfanumerico_" & codice("sa_cod") & "_" & codice("campo_cod")
            If Not codici.ContainsKey(chiave) Then
                codici.Add(chiave, codice("val_cod"))
            End If
        Next

        For Each codice In dtSupContratto.Rows
            Dim chiave = "sup_contratto_" & codice("sa_cod") & "_" & codice("campo_cod")
            If Not codici.ContainsKey(chiave) Then
                codici.Add(chiave, codice("val_cod"))
            End If
        Next

        For Each codice In dtFiliera.Rows
            Dim chiave = "filiera_" & codice("sa_cod") & "_" & codice("campo_cod")
            If Not codici.ContainsKey(chiave) Then
                codici.Add(chiave, codice("val_cod"))
            End If
        Next

    End Sub

    Private Shared Function GetCodiceCampo(ByVal codice As String, ByVal sa_cod As Integer, ByVal campo_cod As Integer, ByRef codici As Dictionary(Of String, String)) As String
        Dim chiave As String = codice & "_" & sa_cod & "_" & campo_cod
        Return If(codici.ContainsKey(chiave), codici(chiave), "")
    End Function

End Class