Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports Newtonsoft.Json.Converters
Imports AgronicaCoreAnagrafeBIZ
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreModelsSTD.baseClass
Imports InData.Anagrafica
Imports AgronicaCoreModelsSTD.anagrafiche
Imports System.IO
Imports AgronicaCoreDTOStd.InData.Anagrafica
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreModelsSTD.exceptions

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class Imprese
    Inherits System.Web.Services.WebService

    '<WebMethod()>
    'Public Function LeggiImpresa() As AgronicaCoreEntityFramework_POCO.Imprese

    '    Dim ii As New AgronicaCoreEntityFramework_POCO.Imprese
    '    Return ii

    'End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiImpreseConFiltroUtente(ByVal objP_server As String, ByVal objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard()

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

            Dim ddl_Aziende As New DropDownList
            AgronicaCoreUtility.CaricaListControl.ImpreseConFiltroUtente(ddl_Aziende, False,
                                                "", "", "", " ORDER BY Rag_Soc asc",
                                                objParametri_Server, objParametri_Utenti)

            Dim JArrayListaOp As New JArray()
            For Each i As ListItem In ddl_Aziende.Items
                JArrayListaOp.Add(New JObject(New JProperty("rag_soc", i.Text), New JProperty("piva", i.Value)))
            Next

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function



    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiImpreseConFiltroUtenteCodiceSocio(ByVal objP_server As String, ByVal objP_utenti As String, ByVal solo_aziende_attive As Integer) As RispostaStandard

        Dim r As New RispostaStandard()
        Dim xFiltro_Aggiuntivo As String = ""
        Dim DataFiltro As String = ""

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If


        If solo_aziende_attive = 1 Then

            DataFiltro = Year(Now) & "-" & Month(Now) & "-" & Day(Now)

            xFiltro_Aggiuntivo = " Exists (Select * From Reg_Impianti as RI Where RI.Piva = Imprese.Piva  " &
                                 " And RI.Validita_Inizio <= Convert(Datetime, '" & DataFiltro & "',120) " &
                                 " And RI.Validita_Fine >= Convert(Datetime, '" & DataFiltro & "',120))"

        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim ddl_Aziende As New DropDownList
            AgronicaCoreUtility.CaricaListControl.ImpreseConFiltroUtente(ddl_Aziende, False,
                                                "", "", xFiltro_Aggiuntivo, " ORDER BY Rag_Soc asc",
                                                objParametri_Server, objParametri_Utenti,,, True)

            Dim JArrayListaOp As New JArray()
            For Each i As ListItem In ddl_Aziende.Items
                JArrayListaOp.Add(New JObject(New JProperty("rag_soc", i.Text), New JProperty("piva", i.Value)))
            Next

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiImpreseConFiltroUtenteCodiceSocio_NG(InData As Object) As RispostaStandard



        Dim r As New RispostaStandard()
        Dim xFiltro_Aggiuntivo As String = ""
        Dim DataFiltro As String = ""

        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData))

        Dim solo_aziende_attive As Integer = InData.InData

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If
        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        If solo_aziende_attive = 1 Then

            DataFiltro = Year(Now) & "-" & Month(Now) & "-" & Day(Now)

            xFiltro_Aggiuntivo = " Exists (Select * From Reg_Impianti as RI Where RI.Piva = Imprese.Piva  " &
                                 " And RI.Validita_Inizio <= Convert(Datetime, '" & DataFiltro & "',120) " &
                                 " And RI.Validita_Fine >= Convert(Datetime, '" & DataFiltro & "',120))"

        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim ddl_Aziende As New DropDownList
            AgronicaCoreUtility.CaricaListControl.ImpreseConFiltroUtente(ddl_Aziende, False,
                                                "", "", xFiltro_Aggiuntivo, " ORDER BY Rag_Soc asc",
                                                objParametri_Server, objParametri_Utenti,,, True)

            Dim JArrayListaOp As New JArray()
            For Each i As ListItem In ddl_Aziende.Items
                JArrayListaOp.Add(New JObject(New JProperty("rag_soc", i.Text), New JProperty("piva", i.Value)))
            Next

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function



    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiImpreseConFiltroUtente_APP(ByVal objP_server As String, ByVal objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If


        'test, ottengo versione della app dall'header
        Dim VAPP As String = ""
        Try

            Dim httphlp As New AgronicaCoreUtility.Http
            VAPP = httphlp.GetFromCurrentHeader2("versioneGias_APP")

        Catch ex As Exception

        End Try



        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim ddl_Aziende As New DropDownList
            Dim DT_Imprese As DataTable = Nothing
            AgronicaCoreUtility.CaricaListControl.ImpreseConFiltroUtente(ddl_Aziende, False,
                                                "", "", "", " ORDER BY Rag_Soc asc",
                                                objParametri_Server, objParametri_Utenti, True, DT_Imprese)

            Dim JArrayListaAz As New JArray()
            For Each row In DT_Imprese.Rows
                JArrayListaAz.Add(New JObject(New JProperty("cuaa", row.Item("CodiceCuaa")), New JProperty("rag_soc", row.Item("Rag_Soc")), New JProperty("piva", row.Item("PIVA"))))
            Next

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaAz, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiImprese_APP(InData As Object) As RispostaStandard

        Dim r As New RispostaStandard()

        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of FiltroAziendeAPP))(JsonConvert.SerializeObject(InData))

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

            ' ricavo impresa padre gerarchia relativa al gruppo utente
            Dim objImprese As New Impresa_R
            Dim padreGerarchia As String = objImprese.Leggi_Padre_Gerarchia(objParametri_Server, objParametri_Utenti)

            Dim Filtro As String = ""
            If Not String.IsNullOrEmpty(InData.InData.gerarchia) Then
                Filtro &= " AND GerarchiaImprese.Foglia = " & If(InData.InData.gerarchia = "1", "0", "1") & " "
            End If
            If Not String.IsNullOrEmpty(InData.InData.tipologie) Then
                Filtro &= " AND Imprese.TipoImpresaGerarchia IN (" & InData.InData.tipologie & ") "
            End If
            If Not String.IsNullOrEmpty(InData.InData.ricerca) Then
                Filtro &= " AND Imprese.Rag_Soc LIKE '%" & InData.InData.ricerca.Replace("'", "''") & "%' "
            End If

            Dim classFiltrone As New Filtrone
            Dim ClassJoin As New JoinFiltrone
            Dim Ordinamento As String = "ORDER BY Livello, Rag_Soc"
            Dim DT_Imprese = classFiltrone.CreaDTFiltrone(objParametri_Server, Filtro, enum_TipoSelect_FiltroneSuperNova.Imprese_APP, Ordinamento, ClassJoin)

            ' Utilizzo oggetti anonimi per ottimizzare la creazione del JSON
            Dim listaImprese As New List(Of Object)

            For Each row As DataRow In DT_Imprese.Rows
                If padreGerarchia <> "" AndAlso padreGerarchia = row.Item("PIVA") Then
                    padreGerarchia = ""
                End If
                listaImprese.Add(New With {
                    .partitaIva = row.Item("PIVA"),
                    .ragioneSociale = row.Item("Rag_Soc"),
                    .CUAA = row.Item("CodiceCuaa"),
                    .tipoImpresa = row.Item("TipoImpresaGerarchia"),
                    .impresaPadre = row.Item("Padre"),
                    .livello = row.Item("Livello"),
                    .foglia = row.Item("Foglia"),
                    .provinciaCod = row.Item("PROV"),
                    .provincia = row.Item("PROVINCIA"),
                    .regioneCod = row.Item("REG"),
                    .regione = row.Item("REGIONE"),
                    .statoCod = row.Item("Stato")
                })
            Next

            ' aggiungo padre gerarchia se non è visibile
            If Not String.IsNullOrEmpty(padreGerarchia) Then
                Filtro = "Imprese.Piva='" & padreGerarchia & "' "
                Dim DT_Padre = classFiltrone.CreaDTFiltrone(objParametri_Server, Filtro, enum_TipoSelect_FiltroneSuperNova.ImpreseAlbero, "", ClassJoin)
                If DT_Padre.Rows.Count > 0 Then
                    Dim row = DT_Padre.Rows(0)
                    listaImprese.Add(New With {
                        .partitaIva = row.Item("PIVA"),
                        .ragioneSociale = row.Item("Rag_Soc"),
                        .CUAA = Nothing,
                        .tipoImpresa = row.Item("TipoImpresaGerarchia"),
                        .impresaPadre = row.Item("Padre"),
                        .livello = row.Item("Livello"),
                        .foglia = row.Item("Foglia"),
                        .provinciaCod = Nothing,
                        .provincia = Nothing,
                        .regioneCod = Nothing,
                        .regione = Nothing,
                        .statoCod = Nothing
                    })
                End If
            End If

            r.RispostaStringa = JsonConvert.SerializeObject(listaImprese, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    ''' <summary>
    ''' Lightweight variant of LeggiImprese_APP used exclusively by the
    ''' GET api/v1/user/companies/visible endpoint (FS002, DS01-BL).
    ''' Wraps the standard Filtrone query in SELECT TOP 2 so that at most 2 rows
    ''' are returned from the database — enough to distinguish the three cases
    ''' (0 companies → manual, 1 company → auto-sync, ≥2 companies → manual)
    ''' without fetching and serialising the full company list.
    ''' </summary>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ContaImpreseVisibili_APP(InData As Object) As RispostaStandard

        Dim r As New RispostaStandard()

        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of FiltroAziendeAPP))(JsonConvert.SerializeObject(InData))

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

            Dim classFiltrone As New Filtrone
            Dim ClassJoin As New JoinFiltrone

            ' Build the full visibility-filtered query through the standard Filtrone pipeline.
            ' Pass a non-empty dummy Ordinamento (" ") so Filtrone skips the Case Else default
            ' ORDER BY — a bare ORDER BY inside a subquery is illegal in SQL Server.
            ' The outer SELECT TOP 2 does not need an order; we only need a count of 0, 1, or ≥2.
            Dim innerSQL As String = classFiltrone.CreaStringaQueryPerDTFiltrone(
                objParametri_Server, "", enum_TipoSelect_FiltroneSuperNova.Imprese_APP,
                " ", ClassJoin)

            Dim topSQL As String = "SELECT TOP 2 * FROM (" & innerSQL & ") AS _visibilita"

            Dim DT_Imprese As DataTable = classFiltrone.EseguiQuery_Lettura(objParametri_Server, topSQL, "ContaImpreseVisibili_APP")

            Dim listaImprese As New List(Of Object)
            For Each row As DataRow In DT_Imprese.Rows
                listaImprese.Add(New With {
                    .partitaIva = row.Item("PIVA"),
                    .ragioneSociale = row.Item("Rag_Soc"),
                    .CUAA = row.Item("CodiceCuaa"),
                    .tipoImpresa = row.Item("TipoImpresaGerarchia"),
                    .impresaPadre = row.Item("Padre"),
                    .livello = row.Item("Livello"),
                    .foglia = row.Item("Foglia"),
                    .provinciaCod = row.Item("PROV"),
                    .provincia = row.Item("PROVINCIA"),
                    .regioneCod = row.Item("REG"),
                    .regione = row.Item("REGIONE"),
                    .statoCod = row.Item("Stato")
                })
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(listaImprese, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiImprese_APP_GIS(InData As Object) As RispostaStandard

        Dim r As New RispostaStandard()

        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of FiltroAziendeMappaAPP))(JsonConvert.SerializeObject(InData))

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

            Dim objImprese As New Impresa_R
            Dim padreGerarchia As String = objImprese.Leggi_Padre_Gerarchia(objParametri_Server, objParametri_Utenti)

            Dim objProfilo As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
            dim Filtro_Visibilita_Utente = not objProfilo.HasFullVisibility(objParametri_Utenti.UtenteUsername, objParametri_Utenti)

            Dim classFiltrone As New Filtrone
            Dim ClassJoin As New JoinFiltrone
            Dim objGISEntita As New AgronicaCoreGisDAL.GIS_Entita_R
            Dim DT_Imprese = objGISEntita.LeggiAziendeMappa_Visibilita(InData.InData.wkt, Filtro_Visibilita_Utente, enum_TipoSelect_FiltroneSuperNova.Imprese_APP, ClassJoin, objParametri_Server)

            Dim JArrayListaAz As New JArray()
            For Each row In DT_Imprese.Rows
                If padreGerarchia <> "" AndAlso padreGerarchia = row.Item("PIVA") Then
                    padreGerarchia = ""
                End If
                JArrayListaAz.Add(
                    New JObject(
                        New JProperty("partitaIva", row.Item("PIVA")),
                        New JProperty("ragioneSociale", row.Item("Rag_Soc")),
                        New JProperty("CUAA", row.Item("CodiceCuaa")),
                        New JProperty("tipoImpresa", row.Item("TipoImpresaGerarchia")),
                        New JProperty("impresaPadre", row.Item("Padre")),
                        New JProperty("livello", row.Item("Livello")),
                        New JProperty("foglia", row.Item("Foglia")),
                        New JProperty("provinciaCod", row.Item("PROV")),
                        New JProperty("provincia", row.Item("PROVINCIA")),
                        New JProperty("regioneCod", row.Item("REG")),
                        New JProperty("regione", row.Item("REGIONE")),
                        New JProperty("statoCod", row.Item("Stato"))
                ))
            Next

            ' aggiungo padre gerarchia se non è visibile
            If Not String.IsNullOrEmpty(padreGerarchia) Then
                Dim Filtro = "Imprese.Piva='" & padreGerarchia & "' "
                Dim DT_Padre = classFiltrone.CreaDTFiltrone(objParametri_Server, Filtro, enum_TipoSelect_FiltroneSuperNova.ImpreseAlbero, "", ClassJoin)
                If DT_Padre.Rows.Count > 0 Then
                    Dim row = DT_Padre.Rows(0)
                    JArrayListaAz.Add(
                        New JObject(
                            New JProperty("partitaIva", row.Item("PIVA")),
                            New JProperty("ragioneSociale", row.Item("Rag_Soc")),
                            New JProperty("tipoImpresa", row.Item("TipoImpresaGerarchia")),
                            New JProperty("impresaPadre", row.Item("Padre")),
                            New JProperty("livello", row.Item("Livello")),
                            New JProperty("foglia", row.Item("Foglia"))
                     ))
                End If
            End If

            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaAz, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiTipologieGerarchiaImprese_APP(ByVal objP_server As String) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim leggiGerarchiaImprese As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
            Dim dtGerarchiaImprese = leggiGerarchiaImprese.LeggiTipologieGerarchiaImprese(enum_Id_Servizio.GiasAPP, objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(dtGerarchiaImprese)

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ScriviImpresa(ByVal s As String, ByVal objP_server As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Try

            'Inserire il codice QUI..
            Dim xScrivi As New AgronicaCoreAnagrafeBIZ.Impresa_W

            Dim rval As String = ""
            'xScrivi.Impresa_EF_Scrivi(s, rval, objParametri_Server, Nothing)



            r.RispostaOK = True
            r.RispostaStringa = ""

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function CaricaAzienda_GIS(ByVal objP_server As String, ByVal objP_utenti As String, ByVal testoRicerca As String) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Try

            Dim LeggiCFGDatiIniziali As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim dtLeggiCfgDatiIniziali As DataTable = LeggiCFGDatiIniziali.Leggi(
                                                        0,
                                                        "GIS_EscludiFiltroCodiceFiscaleTecnico",
                                                        "",
                                                        "",
                                                        objParametri_Server
                                                        )

            Dim condizionePIVA As String = ""
            Dim strIn As String = ""

            Dim xUtenteCorrente As String = CType(objParametri_Server, AgronicaCoreDataProvider.AgronicaCoreParametri).UtenteUsername
            Dim objUtentiDAL As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R
            Dim Codice_Fiscale_Tecnico = objUtentiDAL.Leggi_IdentificativoGruppoUtenti_Singolo(xUtenteCorrente, objParametri_Utenti)

            'leggo tutte le piva che hanno codice_fiscale_utente
            Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
            Dim dt_i As DataTable = objImprese.distinct_Piva_From_Codice_Fiscale_Tecnico(Codice_Fiscale_Tecnico, -1, objParametri_Server)


            If dtLeggiCfgDatiIniziali.Rows.Count = 0 _
                    OrElse dtLeggiCfgDatiIniziali(0)("valore").ToString.ToLower = "false" Then

                strIn = "'" & Codice_Fiscale_Tecnico & "'"
                For i = 0 To dt_i.Rows.Count - 1
                    strIn = strIn & " , '" & dt_i.Rows(i).Item("Piva") & "'"
                Next

                condizionePIVA = " Imprese.PIVA in (" & strIn & ")"
            End If


            Dim condizioneFiltroTesto As String = ""
            If Not String.IsNullOrEmpty(testoRicerca) Then
                condizioneFiltroTesto = " Imprese.Rag_Soc like '%" & testoRicerca & "%' "

                If condizionePIVA <> "" Then
                    condizioneFiltroTesto &= " AND "
                End If
            End If

            Dim ddl_Aziende As New DropDownList
            AgronicaCoreUtility.CaricaListControl.ImpreseConFiltroUtente(ddl_Aziende, False, "", "",
                    condizioneFiltroTesto & condizionePIVA, " order by Imprese.rag_soc asc ", objParametri_Server, objParametri_Utenti)

            r.RispostaStringa = AgronicaCoreUtility.CaricaListControl.itemsCollection_AsJsonArray(ddl_Aziende.Items, "Piva", "Rag_Soc")
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function
    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function CaricaAzienda_GIS_NG(InData As Object) As RispostaStandard

        Dim r As New RispostaStandard

        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData))

        Dim testoRicerca As String = InData.InData

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If
        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

        Try

            Dim LeggiCFGDatiIniziali As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim dtLeggiCfgDatiIniziali As DataTable = LeggiCFGDatiIniziali.Leggi(
                                                        0,
                                                        "GIS_EscludiFiltroCodiceFiscaleTecnico",
                                                        "",
                                                        "",
                                                        objParametri_Server
                                                        )

            Dim condizionePIVA As String = ""
            Dim strIn As String = ""

            Dim xUtenteCorrente As String = CType(objParametri_Server, AgronicaCoreDataProvider.AgronicaCoreParametri).UtenteUsername
            Dim objUtentiDAL As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R
            Dim Codice_Fiscale_Tecnico = objUtentiDAL.Leggi_IdentificativoGruppoUtenti_Singolo(xUtenteCorrente, objParametri_Utenti)

            'leggo tutte le piva che hanno codice_fiscale_utente
            Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
            Dim dt_i As DataTable = objImprese.distinct_Piva_From_Codice_Fiscale_Tecnico(Codice_Fiscale_Tecnico, -1, objParametri_Server)


            If dtLeggiCfgDatiIniziali.Rows.Count = 0 _
                    OrElse dtLeggiCfgDatiIniziali(0)("valore").ToString.ToLower = "false" Then

                strIn = "'" & Codice_Fiscale_Tecnico & "'"
                For i = 0 To dt_i.Rows.Count - 1
                    strIn = strIn & " , '" & dt_i.Rows(i).Item("Piva") & "'"
                Next

                condizionePIVA = " Imprese.PIVA in (" & strIn & ")"
            End If


            Dim condizioneFiltroTesto As String = ""
            If Not String.IsNullOrEmpty(testoRicerca) Then
                condizioneFiltroTesto = " Imprese.Rag_Soc like '%" & testoRicerca & "%' "

                If condizionePIVA <> "" Then
                    condizioneFiltroTesto &= " AND "
                End If
            End If

            Dim ddl_Aziende As New DropDownList
            AgronicaCoreUtility.CaricaListControl.ImpreseConFiltroUtente(ddl_Aziende, False, "", "",
                    condizioneFiltroTesto & condizionePIVA, " order by Imprese.rag_soc asc ", objParametri_Server, objParametri_Utenti)

            r.RispostaStringa = AgronicaCoreUtility.CaricaListControl.itemsCollection_AsJsonArray(ddl_Aziende.Items, "Piva", "Rag_Soc")
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function


    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Carica_Cmb_Imprese_NG(InData As CoreWS_Generic(Of String)) As RispostaStandard
        Dim r As New RispostaStandard()

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim dt_imprese = Carica_Imprese(objParametri_Server, objParametri_Utenti)

            Dim JArrayLista As New JArray()
            For Each dr In dt_imprese.Rows
                JArrayLista.Add(New JObject(New JProperty("piva", dr.Item("piva")), New JProperty("rag_soc", dr.Item("rag_soc"))))
            Next

            r.RispostaOK = True
            r.RispostaStringa = JArrayLista.ToString
        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Carica_Cmb_Imprese(ByVal objP_server As String, ByVal objP_utenti As String) As RispostaStandard
        Dim r As New RispostaStandard()

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

            Dim dt_imprese = Carica_Imprese(objParametri_Server, objParametri_Utenti)

            Dim JArrayLista As New JArray()
            For Each dr In dt_imprese.Rows
                JArrayLista.Add(New JObject(New JProperty("piva", dr.Item("piva")), New JProperty("rag_soc", dr.Item("rag_soc"))))
            Next

            r.RispostaOK = True
            r.RispostaStringa = JArrayLista.ToString
        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function


    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Carica_Cmb_Imprese_Area(ByVal objP_server As String, ByVal objP_utenti As String, ByVal area As String) As RispostaStandard
        Dim r As New RispostaStandard()

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

            Dim areaCod As Integer

            Select Case area.ToLower
                Case "uma"
                    areaCod = TipiEnumerativi.enum_Area_Visibilita.UMA
            End Select

            Dim dt_imprese = Carica_Imprese_Area(objParametri_Server, objParametri_Utenti, areaCod)

            Dim JArrayLista As New JArray()
            For Each dr In dt_imprese.Rows
                JArrayLista.Add(New JObject(New JProperty("piva", dr.Item("piva")), New JProperty("rag_soc", dr.Item("rag_soc"))))
            Next

            r.RispostaOK = True
            r.RispostaStringa = JArrayLista.ToString
        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Carica_Cmb_Imprese_Area_NG(InData As Object) As RispostaStandard
        Dim r As New RispostaStandard()

        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData))

        Dim area As String = InData.InData

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If
        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If
        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim areaCod As Integer

            Select Case area.ToLower
                Case "uma"
                    areaCod = TipiEnumerativi.enum_Area_Visibilita.UMA
            End Select

            Dim dt_imprese = Carica_Imprese_Area(objParametri_Server, objParametri_Utenti, areaCod)

            Dim JArrayLista As New JArray()
            For Each dr In dt_imprese.Rows
                JArrayLista.Add(New JObject(New JProperty("piva", dr.Item("piva")), New JProperty("rag_soc", dr.Item("rag_soc"))))
            Next

            r.RispostaOK = True
            r.RispostaStringa = JArrayLista.ToString
        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function
    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Carica_Cmb_Imprese_UMA(objP_server As String,
                                           objP_utenti As String,
                                           Tipo_Azienda As Integer) As RispostaStandard

        Dim r As New RispostaStandard()

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

            Dim dt_imprese = Carica_Imprese_Area(objParametri_Server, objParametri_Utenti,
                                                 enum_Area_Visibilita.UMA,
                                                 Tipo_Azienda_UMA:=Tipo_Azienda)

            Dim JArrayLista As New JArray()
            For Each dr In dt_imprese.Rows
                JArrayLista.Add(New JObject(New JProperty("piva", dr.Item("piva")),
                                            New JProperty("rag_soc", dr.Item("rag_soc")),
                                            New JProperty("forma_giuridica", dr.Item("forma_giuridica")),
                                            New JProperty("flagPubblica", dr.Item("flagPubblica"))))
            Next

            r.RispostaOK = True
            r.RispostaStringa = JArrayLista.ToString
        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Carica_Cmb_Imprese_UMA_NG(InData As Object) As RispostaStandard

        Dim r As New RispostaStandard()

        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData))

        Dim Tipo_Azienda As Integer = InData.InData

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If
        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim dt_imprese = Carica_Imprese_Area(objParametri_Server, objParametri_Utenti,
                                                 enum_Area_Visibilita.UMA,
                                                 Tipo_Azienda_UMA:=Tipo_Azienda)

            Dim JArrayLista As New JArray()
            For Each dr In dt_imprese.Rows
                JArrayLista.Add(New JObject(New JProperty("piva", dr.Item("piva")),
                                            New JProperty("rag_soc", dr.Item("rag_soc")),
                                            New JProperty("forma_giuridica", dr.Item("forma_giuridica")),
                                            New JProperty("flagPubblica", dr.Item("flagPubblica"))))
            Next

            r.RispostaOK = True
            r.RispostaStringa = JArrayLista.ToString
        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function
    Public Function Carica_Imprese(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri) As DataTable

        Dim objAnagrafeBIZ As New AgronicaCoreAnagrafeBIZ.Impresa_R

        Dim dt = objAnagrafeBIZ.Imprese_Leggi_VisibilitaUtente_CUAA(objParametri_Server, objParametri_Utenti)

        Return dt

    End Function

    Private Function Carica_Imprese_Area(ByVal objP_server As AgronicaCoreParametri,
                                         ByVal objP_utenti As AgronicaCoreParametri,
                                         ByVal area As Integer,
                                         Optional Tipo_Azienda_UMA As Integer = 0) As DataTable

        Dim objImprese_Dal As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim objUtenti_Visibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
        Dim objGruppi_Utente As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R

        Dim userName As String = objP_server.UtenteUsername

        Dim gruppo As Integer = 0

        Dim checkDt As DataTable = objUtenti_Visibilita.CheckVisibilitaUtenteGruppoArea(userName, gruppo, area, "", "", objP_utenti)

        Dim visibilitaCompleta As Boolean = False

        Dim dt As New DataTable

        If checkDt.Rows.Count <= 0 Then

            Dim dtGruppi = objGruppi_Utente.Leggi_IdentificativoGruppoUtenti(userName, objP_utenti)

            If dtGruppi.Rows.Count > 0 Then

                gruppo = dtGruppi.Rows.Item(0).Item("Gruppi_Utente_cod")
                userName = String.Empty

                checkDt = objUtenti_Visibilita.CheckVisibilitaUtenteGruppoArea(userName, gruppo, area, "", "", objP_utenti)

            End If

        End If

        If checkDt.Rows.Count > 0 Then

            If (checkDt.Rows.Item(0).Item("Visibilita_Completa")) Then
                visibilitaCompleta = True
            End If

            dt = objImprese_Dal.Leggi_ImpreseCUAA_Visibilita_Area(objP_server, objP_utenti,
                                                                  area, userName, gruppo, visibilitaCompleta,
                                                                  Tipo_Azienda_UMA:=Tipo_Azienda_UMA)

        End If

        Return dt

    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_Imprese_Anagrafica(InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim settings As New JsonSerializerSettings()
        settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

        Dim iData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG))(
            JsonConvert.SerializeObject(InData, settings),
            settings
            )

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            Dim objProfilo As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
            dim Filtro_Visibilita_Utente = not objProfilo.HasFullVisibility(objParametri_Utenti.UtenteUsername, objParametri_Utenti)

            Dim Piva = iData.InData.Piva

            If iData.InData.Data <> AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then
                objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(iData.InData.Data, iData.InData.Data)
            End If
            Dim leggiSuperfici = True
            Dim distinctPiva = False
            If Not String.IsNullOrWhiteSpace(iData.InData.GenericObj_string) Then
                Dim obj = JsonConvert.DeserializeObject(iData.InData.GenericObj_string)
                If obj IsNot Nothing AndAlso obj("leggiSuperfici") IsNot Nothing Then
                    leggiSuperfici = obj("leggiSuperfici")
                End If
                If obj IsNot Nothing AndAlso obj("distinctPiva") IsNot Nothing Then
                    distinctPiva = obj("distinctPiva")
                End If
            End If

            Dim dtAnagrafica As DataTable = objImprese.Leggi_x_anagraficaVisibilita_Utente_NG(
                Piva,
                "",
                "",
                objParametri_Server,
                objParametri_Utenti,
                Filtro_Visibilita_Utente,
                leggiSuperfici
                )

            If iData.InData.Data <> AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then
                objParametri_Server.ResettaFinestra()
            End If

            If distinctPiva Then
                dtAnagrafica = dtAnagrafica.Select.AsParallel.
                    GroupBy(Function(row) CStr(row("piva"))).
                    Select(Function(g) g.First).
                    CopyToDataTable
            End If

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(dtAnagrafica)
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function
    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_Certificazioni(InData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)
                                            ) As rispostaStandard(Of List(Of BaseCodeDescr))
        Dim r As New rispostaStandard(Of List(Of BaseCodeDescr))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objImprese As New AgronicaCoreAnagrafeBIZ.Impresa_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim certificazioni = objImprese.Leggi_Certificazioni_Disponibili(objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = certificazioni
        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r

    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_Impresa_Anagrafica(
                                            InData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)
                                            ) As rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.Impresa)
        Dim r As New rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.Impresa)

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objImprese As New AgronicaCoreAnagrafeBIZ.Impresa_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)

            Dim Piva = InData.InData.Piva

            Dim impresa = objImprese.Impresa_Leggi_Anagrafica(Piva:=Piva,
                                                Leggi_Indirizzo:=True,
                                                Leggi_Padri:=True,
                                                Leggi_Contatti:=True,
                                                Leggi_Contatto_Superuser:=True,
                                                Leggi_Codici:=True,
                                                objParametri_Server,
                                                objParametri_Utenti,
                                                objParametri_Super_Server)

            r.RispostaOK = True
            r.RispostaStringa = impresa
        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiImpreseConFiltroUtente_Modello(ByVal objP_server As String, ByVal objP_utenti As String) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.Impresa))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.Impresa))

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

            Dim ddl_Aziende As New DropDownList
            Dim dtImprese As DataTable
            AgronicaCoreUtility.CaricaListControl.ImpreseConFiltroUtente(ddl_Aziende, False,
                                                "", "", "", " ORDER BY Rag_Soc asc",
                                                objParametri_Server, objParametri_Utenti, True, dtImprese)

            Dim JArrayListaOp As New List(Of AgronicaCoreModelsSTD.anagrafiche.Impresa)
            For Each i As DataRow In dtImprese.Rows
                JArrayListaOp.Add(New AgronicaCoreModelsSTD.anagrafiche.Impresa() With {
                                      .partitaIva = i("Piva"),
                                      .ragioneSociale = i("rag_soc"),
                                      .CUAA = IIf(IsDBNull(i("CodiceCuaa")), "", i("CodiceCuaa"))
                })
            Next

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = JArrayListaOp
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    Private Function LeggiImpreseConFiltroUtenteHubAgea(InData As CoreWS_Generic(Of InData.Agenda.FarmFilters), objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, objParametri_Super_Server As AgronicaCoreParametri) As List(Of Impresa)

        Dim ddl_Aziende As New DropDownList
        Dim dtImprese As DataTable

        Dim objBundle As New AgronicaCoreAgeaBIZ.BundleState
        Dim hubAgeaResponse = objBundle.GetAvailableFarms(InData.InData, objParametri_Super_Server)
        Dim filteredFarms = JsonConvert.DeserializeObject(Of List(Of String))(hubAgeaResponse).Select(Function(x) "'" & x & "'")

        Dim filtroAggiuntivo = ""

        If Not IsNothing(filteredFarms) AndAlso filteredFarms.Count > 0 Then
            If InData.InData.WithBundles Then
                filtroAggiuntivo = " IC_Cuaa.val_cod IN (" & String.Join(",", filteredFarms) & ") "
            Else
                filtroAggiuntivo = " IC_Cuaa.val_cod NOT IN (" & String.Join(",", filteredFarms) & ") "
            End If
        End If

        AgronicaCoreUtility.CaricaListControl.ImpreseConFiltroUtente(ddl_Aziende, False,
                                            "", "", filtroAggiuntivo, " ORDER BY Rag_Soc asc",
                                            objParametri_Server, objParametri_Utenti, True, dtImprese)

        Dim JArrayListaOp As New List(Of AgronicaCoreModelsSTD.anagrafiche.Impresa)
        For Each i As DataRow In dtImprese.Rows
            JArrayListaOp.Add(New AgronicaCoreModelsSTD.anagrafiche.Impresa() With {
                                  .partitaIva = i("Piva"),
                                  .ragioneSociale = i("rag_soc"),
                                  .CUAA = IIf(IsDBNull(i("CodiceCuaa")), "", i("CodiceCuaa"))
            })
        Next

        Return JArrayListaOp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiImpreseConFiltroUtenteHubAgea_Modello(ByVal InData As CoreWS_Generic(Of InData.Agenda.FarmFilters)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.Impresa))
        'CoreWSRequest<CoreWS_Generic<FarmFilters>>
        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.Impresa))

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
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)

            Dim JArrayListaOp As List(Of Impresa) = LeggiImpreseConFiltroUtenteHubAgea(InData, objParametri_Server, objParametri_Utenti, objParametri_Super_Server)

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = JArrayListaOp
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ExportImpreseAgeaExcel(ByVal InData As CoreWS_Generic(Of InData.Agenda.FarmFilters)) As rispostaStandard(Of ImpresaAgeaExcel)

        Dim r As New rispostaStandard(Of ImpresaAgeaExcel)

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
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)

            Dim objspv = New AgronicaCoreAnagrafeBIZ.Impresa_W

            Dim listaImprese As List(Of Impresa) = LeggiImpreseConFiltroUtenteHubAgea(InData, objParametri_Server, objParametri_Utenti, objParametri_Super_Server)

            Dim excelFile = objspv.creaExcel(listaImprese, objParametri_Server)

            Dim objExcel As New ImpresaAgeaExcel
            Dim fileInfo = New FileInfo(excelFile)
            objExcel.FileName = fileInfo.Name
            objExcel.Extension = fileInfo.Extension
            objExcel.Data = My.Computer.FileSystem.ReadAllBytes(excelFile)
            File.Delete(excelFile)

            r.RispostaOK = True
            r.RispostaStringa = objExcel

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_Imprese_Codici(ByVal objP_super_server As String,
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

            Dim objCodiceAnagrafe As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R

            Dim StrCodiciAzienda = objCodiceAnagrafe.Filtro_Codici_Anagrafe(1, 3, 2, objParametri_Server)

            'Elimino codice CUAA, Titolo Possesso, Codice Libro Soci, Data Iscrizione Libro Soci e tecnico perchè già presenti nella form
            StrCodiciAzienda = Replace(StrCodiciAzienda, " Or Codice = 1010", "")
            StrCodiciAzienda = Replace(StrCodiciAzienda, " Codice = 1010", "")
            StrCodiciAzienda = Replace(StrCodiciAzienda, " Or Codice = 1016", "")
            StrCodiciAzienda = Replace(StrCodiciAzienda, "Codice = 1016 Or ", "")
            StrCodiciAzienda = Replace(StrCodiciAzienda, " Or Codice = 1086", "")
            StrCodiciAzienda = Replace(StrCodiciAzienda, " Codice = 1086", "")
            StrCodiciAzienda = Replace(StrCodiciAzienda, " Or Codice = 1087", "")
            StrCodiciAzienda = Replace(StrCodiciAzienda, " Codice = 1087", "")
            StrCodiciAzienda = Replace(StrCodiciAzienda, " Or Codice = 1088", "")
            StrCodiciAzienda = Replace(StrCodiciAzienda, " Codice = 1088", "")

            Dim DT = objCodiceAnagrafe.Leggi(
                0,
                "",
                enumSelezioneVariabile.Selezione_TabellaCompleta,
                StrCodiciAzienda,
                "",
                objParametri_Server
                )

            DT = DT.DefaultView.ToTable(True, "codice", "descrizione")
            Dim serializerSettings As New JsonSerializerSettings()

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Formatting.None, serializerSettings)
        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_Imprese_Codici_NG(InData As CoreWS_Generic(Of Object)) As RispostaStandard
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
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objCodiceAnagrafe As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R

            Dim StrCodiciAzienda = objCodiceAnagrafe.Filtro_Codici_Anagrafe(1, 3, 2, objParametri_Server)

            'Elimino codice CUAA, Titolo Possesso, Codice Libro Soci, Data Iscrizione Libro Soci e tecnico perchè già presenti nella form
            StrCodiciAzienda = Replace(StrCodiciAzienda, " Or Codice = 1010", "")
            StrCodiciAzienda = Replace(StrCodiciAzienda, " Codice = 1010", "")
            StrCodiciAzienda = Replace(StrCodiciAzienda, " Or Codice = 1016", "")
            StrCodiciAzienda = Replace(StrCodiciAzienda, "Codice = 1016 Or ", "")
            StrCodiciAzienda = Replace(StrCodiciAzienda, " Or Codice = 1086", "")
            StrCodiciAzienda = Replace(StrCodiciAzienda, " Codice = 1086", "")
            StrCodiciAzienda = Replace(StrCodiciAzienda, " Or Codice = 1087", "")
            StrCodiciAzienda = Replace(StrCodiciAzienda, " Codice = 1087", "")
            StrCodiciAzienda = Replace(StrCodiciAzienda, " Or Codice = 1088", "")
            StrCodiciAzienda = Replace(StrCodiciAzienda, " Codice = 1088", "")
            StrCodiciAzienda = Replace(StrCodiciAzienda, " Or Codice = 1105", "")
            StrCodiciAzienda = Replace(StrCodiciAzienda, " Codice = 1105", "")
            StrCodiciAzienda = Replace(StrCodiciAzienda, " Or Codice = 1106", "")
            StrCodiciAzienda = Replace(StrCodiciAzienda, " Codice = 1106", "")

            Dim DT = objCodiceAnagrafe.Leggi(
                0,
                "",
                enumSelezioneVariabile.Selezione_TabellaCompleta,
                StrCodiciAzienda,
                "",
                objParametri_Server
                )

            DT = DT.DefaultView.ToTable(True, "codice", "descrizione")
            Dim serializerSettings As New JsonSerializerSettings()

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Formatting.None, serializerSettings)
        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Scrivi_Impresa_Anagrafica(InData As Object) As rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.Impresa)
        Dim r As New rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.Impresa)

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim settings As New JsonSerializerSettings()
        settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

        Dim iData As CoreWS_Generic(Of AgronicaCoreModelsSTD.anagrafiche.Impresa) =
            JsonConvert.DeserializeObject(
            Of CoreWS_Generic(Of AgronicaCoreModelsSTD.anagrafiche.Impresa))(JsonConvert.SerializeObject(InData, settings),
                                                                                      settings)

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

        Try
            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim objImpresaBIZ As New AgronicaCoreAnagrafeBIZ.Impresa_W

            Dim impresa = objImpresaBIZ.Scrivi_Impresa_Anagrafica(iData.InData, objParametri_Server, objParametri_Utenti)

            r.RispostaOK = True
            r.RispostaStringa = impresa

        Catch ex As GiasException

            r.RispostaOK = False
            r.RispostaStringa = iData.InData
            r.Errore = ex.Message
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}

        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = Nothing
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
        End Try

        Return r

    End Function


    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_Imprese_Padri(InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Anagrafica.Leggi_Padri)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.Impresa))
        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.Impresa))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objImpresaBIZ As New AgronicaCoreAnagrafeBIZ.Impresa_W

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim padri = objImpresaBIZ.Leggi_Imprese_Padri(objParametri_Server, objParametri_Utenti)

            r.RispostaOK = True
            r.RispostaStringa = padri

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_Imprese_Padri_BaseCodeDescrStr(InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Anagrafica.Leggi_Padri)) As rispostaStandard(Of List(Of BaseCodeDescrStr))
        Dim r As New rispostaStandard(Of List(Of BaseCodeDescrStr))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objImpresaBIZ As New AgronicaCoreAnagrafeBIZ.Impresa_W

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim padri As List(Of BaseCodeDescrStr) = objImpresaBIZ.Leggi_Imprese_Padri(objParametri_Server, objParametri_Utenti).
                Select(Of BaseCodeDescrStr)(Function(impresa)
                                                Return New BaseCodeDescrStr(impresa.partitaIva, impresa.ragioneSociale)
                                            End Function).ToList

            r.RispostaOK = True
            r.RispostaStringa = padri

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    Public Function Leggi_Impresa_Padre_Sementieri(InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Anagrafica.Leggi_Padri)) As rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.Impresa)
        Dim r As New rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.Impresa)

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objImpresaBIZ As New AgronicaCoreAnagrafeBIZ.Impresa_W

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim padri = objImpresaBIZ.Leggi_Impresa_Padre_Sementieri(objParametri_Server, objParametri_Utenti)

            r.RispostaOK = True
            r.RispostaStringa = padri

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Controllo_Presenza_Piva(InData As Object) As RispostaStandard

        Dim r As New RispostaStandard()

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim iData As CoreWS_Generic(Of String) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData), a)

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            Dim dtAnagrafica As DataTable = objImprese.Leggi_x_anagraficaVisibilita_Utente_NG(iData.InData,
                                                                                           "",
                                                                                           "",
                                                                                           objParametri_Server,
                                                                                           objParametri_Utenti,
                                                                                           False)
            If dtAnagrafica.Rows.Count <> 0 Then
                r.RispostaStringa = JsonConvert.SerializeObject(dtAnagrafica.Rows(0), Formatting.None)
            Else
                r.RispostaStringa = JsonConvert.SerializeObject("", Formatting.None)
            End If

            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Max_DataModifica(InData As Object) As rispostaStandard(Of AgronicaCoreDTOStd.InData.Data)

        Dim r As New rispostaStandard(Of AgronicaCoreDTOStd.InData.Data)

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim objStr = JsonConvert.SerializeObject(InData, a)

        Dim iData As CoreWS_Generic(Of String) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData), a)

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objImprese As New AgronicaCoreAnagrafeBIZ.Impresa_R
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            Dim resp = New AgronicaCoreDTOStd.InData.Data
            resp.data = objImprese.Leggi_Max_Data_Modifica(objParametri_Server, objParametri_Utenti)

            r.RispostaStringa = resp

            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            r.RispostaOK = False
            Return r
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Imprese_Filtro(InData As Object) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.Impresa))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.Impresa))

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim objStr = JsonConvert.SerializeObject(InData, a)

        Dim iData As CoreWS_Generic(Of String) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData), a)

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objImprese As New AgronicaCoreAnagrafeBIZ.Impresa_R
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            r.RispostaStringa = MenuBS_2017_RedirectGestione.LeggiImpreseModello(iData.InData, 0, objParametri_Server)

            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            r.RispostaOK = False
            Return r
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Imprese_Filtro_NG(InData As CoreWS_Generic(Of String)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.Impresa))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.Impresa))

        'Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        'Dim objStr = JsonConvert.SerializeObject(InData, a)

        'Dim iData As CoreWS_Generic(Of String) =
        '    JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData), a)

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objImprese As New AgronicaCoreAnagrafeBIZ.Impresa_R
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            r.RispostaStringa = MenuBS_2017_RedirectGestione.LeggiImpreseModello(InData.InData, 0, objParametri_Server)

            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            r.RispostaOK = False
            Return r
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Imprese_Filtro_StringaRicerca(InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.LeggiFiltro)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.Impresa))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.Impresa))

        'Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        'Dim objStr = JsonConvert.SerializeObject(InData, a)

        'Dim iData As CoreWS_Generic(Of String) =
        '    JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData), a)

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objImprese As New AgronicaCoreAnagrafeBIZ.Impresa_R
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            r.RispostaStringa = MenuBS_2017_RedirectGestione.LeggiImpreseModello(InData.InData.Ricerca, 0, objParametri_Server)

            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            r.RispostaOK = False
            Return r
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ImpresaBiologica(InData As Object) As rispostaStandard(Of Boolean)

        Dim r As New rispostaStandard(Of Boolean)

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim objStr = JsonConvert.SerializeObject(InData, a)

        Dim iData As CoreWS_Generic(Of AgronicaCoreModelsSTD.anagrafiche.Impresa) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreModelsSTD.anagrafiche.Impresa))(JsonConvert.SerializeObject(InData), a)

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objImprese As New AgronicaCoreAnagrafeBIZ.Impresa_R
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            Dim objImprese_Codici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
            Dim dt = objImprese_Codici.Leggi(iData.InData.partitaIva, enum_CodiciAnagrafe.Disciplinare_Aziendale_Default, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
            Dim bio = False
            If dt.Rows.Count > 0 Then
                Dim Val = CStr(dt.Rows(0)("val_cod"))
                If Val = "-2" Then
                    bio = True
                End If
            End If

            r.RispostaStringa = bio

            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            r.RispostaOK = False
            Return r
        End Try

        Return r

    End Function



    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Test_Imprese_Archivio_Lettura(ByVal InData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)) As rispostaStandard(Of Boolean)

        Dim r As New rispostaStandard(Of Boolean)

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objImpreseDAL_R As New AgronicaCoreAnagrafeDAL.Imprese_Read
            Dim objImpreseBIZ_R As New AgronicaCoreAnagrafeBIZ.Impresa_R

            Dim dt = objImpreseDAL_R.Leggi(InData.InData.Piva, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            r.ErroriGias = New List(Of ErroreGias)
            Dim i = 0
            For Each row In dt.Rows
                i += 1

                Try
                    Dim m = objImpreseBIZ_R.Impresa_Leggi_Anagrafica(row("Piva"), True, True, True, True, True, objParametri_Server)

                Catch ex As Exception

                    r.ErroriGias.Add(New ErroreGias() With {
                                     .ex = ex.Message,
                                     .messaggio = "Errore Impresa " & row("Piva")}
                                     )

                End Try
            Next

            r.RispostaOK = True
            r.RispostaStringa = True

            If r.ErroriGias.Count > 0 Then
                r.RispostaOK = False
                r.RispostaStringa = False
            End If

        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = False
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Test_Imprese_Archivio_Scrittura(ByVal InData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)) As rispostaStandard(Of Boolean)

        Dim r As New rispostaStandard(Of Boolean)

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objImpreseDAL_R As New AgronicaCoreAnagrafeDAL.Imprese_Read
            Dim objImpreseBIZ_R As New AgronicaCoreAnagrafeBIZ.Impresa_R
            Dim objImpreseBIZ_W As New AgronicaCoreAnagrafeBIZ.Impresa_W

            Dim dt = objImpreseDAL_R.Leggi(InData.InData.Piva, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            r.ErroriGias = New List(Of ErroreGias)
            Dim i = 0
            For Each row In dt.Rows
                i += 1
                Dim m As AgronicaCoreModelsSTD.anagrafiche.Impresa

                Try
                    m = objImpreseBIZ_R.Impresa_Leggi_Anagrafica(row("Piva"), True, True, True, True, True, objParametri_Server)
                Catch ex As Exception

                    r.ErroriGias.Add(New ErroreGias() With {
                                     .ex = ex.Message,
                                     .messaggio = "Errore Lettura Impresa " & row("Piva")}
                                     )

                End Try

                If row("Blk_Flag") = 0 Then
                    Try
                        If m IsNot Nothing Then
                            objImpreseBIZ_W.Scrivi_Impresa_Anagrafica(m, objParametri_Server, objParametri_Utenti)
                        End If

                    Catch ex As Exception

                        r.ErroriGias.Add(New ErroreGias() With {
                                         .ex = ex.Message,
                                         .messaggio = "Errore Scrittura Impresa " & row("Piva")}
                                         )

                    End Try
                End If


            Next

            r.RispostaOK = True
            r.RispostaStringa = True

            If r.ErroriGias.Count > 0 Then
                r.RispostaOK = False
                r.RispostaStringa = False
            End If



        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = False
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

#Region "COMBO MODIFICA MULTIPLA"
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboCmb_CertificazioniAziendali(ByVal objP_server As String, ByVal objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard()

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

            Dim objCAC As New AgronicaCoreAnagrafeDAL.CertificazioniAziendali_R
            Dim DT As DataTable = objCAC.Leggi(New List(Of Integer), "", AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)

            Dim jArray As New JArray()
            For Each dr As DataRow In DT.Rows
                jArray.Add(New JObject(New JProperty("text", dr.Item("CA_Des")),
                                                   New JProperty("value", dr.Item("CA_Cod"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArray, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function
#End Region


End Class