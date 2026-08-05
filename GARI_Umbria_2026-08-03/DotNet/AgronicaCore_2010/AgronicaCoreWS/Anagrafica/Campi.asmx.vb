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
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Xml
Imports AgronicaCoreDTOStd.InData.Anagrafica
Imports AgronicaCoreModelsSTD.costanti
Imports AgronicaCoreModelsSTD.metaschema.utilizzi
Imports AgronicaCoreModelsSTD.exceptions

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class Campi
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetCampi(ByVal piva As String, ByVal sa_cod As Integer, ByVal objP_server As String) As RispostaStandard

        Dim r As New RispostaStandard

        Try
            Dim objParametri_server As AgronicaCoreParametri
            If HttpContext.Current.Session("ASG_objParametri_Server") IsNot Nothing Then
                objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")
            Else
                objParametri_server = Utility.convertStringtoOBJparametri(objP_server)
            End If


            Dim objR As New AgronicaCoreAnagrafeDAL.Campi_R

            'Dim dt As DataTable = objR.Leggi(piva, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_server)
            Dim dt As DataTable = objR.Leggi_x_anagrafica(piva, sa_cod, 0, "", "", objParametri_server)


            Dim lista As New List(Of String)
            Dim listaJ As New JArray

            For Each dr As DataRow In dt.Rows
                Dim objCentro As New JObject

                objCentro.Add(New JProperty("campo_cod", dr.Item("campo_cod")))
                objCentro.Add(New JProperty("campo_des", dr.Item("campo_des")))

                listaJ.Add(objCentro)
                lista.Add("{""campo_cod"":""" & jSon.Escape(dr.Item("campo_cod")) & """, ""campo_des"":""" & dr.Item("campo_des") & """}")
            Next


            'Dim strRisp As String = "[" & String.Join(",", lista.Distinct.ToList) & "]"
            Dim strRisp As String = listaJ.ToString


            r.RispostaOK = True
            r.RispostaStringa = strRisp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


            r.Errore = MessaggioErrore


        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetCampi_NG(InData As CoreWS_Generic(Of GetCampi)) As RispostaStandard

        Dim r As New RispostaStandard

        Try
            Dim objParametri_server As AgronicaCoreParametri
            If HttpContext.Current.Session("ASG_objParametri_Server") IsNot Nothing Then
                objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")
            Else
                objParametri_server = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            End If


            Dim objR As New AgronicaCoreAnagrafeDAL.Campi_R

            'Dim dt As DataTable = objR.Leggi(piva, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_server)
            Dim dt As DataTable = objR.Leggi_x_anagrafica(InData.InData.Piva, InData.InData.Sa_Cod, 0, "", "", objParametri_server)


            Dim lista As New List(Of String)
            Dim listaJ As New JArray

            For Each dr As DataRow In dt.Rows
                Dim objCentro As New JObject

                objCentro.Add(New JProperty("campo_cod", dr.Item("campo_cod")))
                objCentro.Add(New JProperty("campo_des", dr.Item("campo_des")))

                listaJ.Add(objCentro)
                lista.Add("{""campo_cod"":""" & jSon.Escape(dr.Item("campo_cod")) & """, ""campo_des"":""" & dr.Item("campo_des") & """}")
            Next


            'Dim strRisp As String = "[" & String.Join(",", lista.Distinct.ToList) & "]"
            Dim strRisp As String = listaJ.ToString


            r.RispostaOK = True
            r.RispostaStringa = strRisp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


            r.Errore = MessaggioErrore


        End Try

        Return r

    End Function
    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_Campi_Anagrafica(InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim settings As New JsonSerializerSettings()
        settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

        Dim iData As CoreWS_Generic(
            Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG))(JsonConvert.SerializeObject(InData), settings)

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objCampi As New AgronicaCoreAnagrafeDAL.Campi_R
            Dim objAppezzamento As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
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

            If objParametriAgenda.Data <> AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then
                objParametri_Server.FinestraTemporaleInizio = objParametriAgenda.Data.Date
                objParametri_Server.FinestraTemporaleFine = objParametriAgenda.Data.Date
            End If

            Dim dt As DataTable = objCampi.Leggi_x_anagrafica_NG(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, 0, "", "", objParametri_Server)

            If objParametriAgenda.Data <> AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then
                objParametri_Server.FinestraTemporaleInizio = objParametriAgenda.Data.Date
                objParametri_Server.FinestraTemporaleFine = objParametriAgenda.Data.Date
            End If

            'dt.Columns.AddRange({New DataColumn("Superficie_Totale", Type.GetType("System.Decimal")),
            '                           New DataColumn("Superficie_Convenzionale", Type.GetType("System.Decimal")),
            '                           New DataColumn("Superficie_Biologico", Type.GetType("System.Decimal")),
            '                           New DataColumn("Superficie_Conversione", Type.GetType("System.String")),
            '                           New DataColumn("Superficie_Catastale", Type.GetType("System.String"))
            '                          })

            ' aggiungo codici campo
            'Dim codiciCampo As New Dictionary(Of String, String)
            'LeggiCodiciCampi(objParametriAgenda.Piva, codiciCampo, objParametri_Server)
            'dt.Columns.Add(New DataColumn("rif_alfanumerico", Type.GetType("System.String")))
            'dt.Columns.Add(New DataColumn("sup_contratto", Type.GetType("System.String")))
            'dt.Columns.Add(New DataColumn("filiera", Type.GetType("System.String")))

            'For i As Integer = 0 To dt.Rows.Count - 1

            '    'Recupero le Superfici
            '    Dim Sup_Totale, Sup_Tare, SAU_Totale, SAU_Biologico, SAU_Conversione, SAU_Convenzionale, SAU_Catastale As Double
            '    If Not IsDBNull(dt.Rows(i).Item("Piva")) AndAlso dt.Rows(i).Item("Piva") <> "" AndAlso Not IsDBNull(dt.Rows(i).Item("sa_cod")) AndAlso IsNumeric(dt.Rows(i).Item("sa_cod")) Then
            '        objAppezzamento.Recupera_Superfici_Campo(dt.Rows(i).Item("Piva"), dt.Rows(i).Item("sa_cod"), dt.Rows(i).Item("Campo_Cod"),
            '                                        Sup_Totale, SAU_Biologico, SAU_Conversione, SAU_Convenzionale, SAU_Catastale, "", "", objParametri_Server)

            '        dt.Rows(i).Item("Superficie_Totale") = Format(Sup_Totale, "0.0000")
            '        dt.Rows(i).Item("Superficie_Biologico") = Format(SAU_Biologico, "0.0000")
            '        dt.Rows(i).Item("Superficie_Convenzionale") = Format(SAU_Convenzionale, "0.0000")
            '        dt.Rows(i).Item("Superficie_Conversione") = Format(SAU_Conversione, "0.0000")
            '        dt.Rows(i).Item("Superficie_Catastale") = Format(SAU_Catastale, "0.0000")

            '        'dt.Rows(i).Item("rif_alfanumerico") = GetCodiceCampo("rif_alfanumerico", dt.Rows(i).Item("sa_cod"), dt.Rows(i).Item("Campo_Cod"), codiciCampo)
            '        'dt.Rows(i).Item("sup_contratto") = GetCodiceCampo("sup_contratto", dt.Rows(i).Item("sa_cod"), dt.Rows(i).Item("Campo_Cod"), codiciCampo)
            '        'dt.Rows(i).Item("filiera") = GetCodiceCampo("filiera", dt.Rows(i).Item("sa_cod"), dt.Rows(i).Item("Campo_Cod"), codiciCampo)

            '    End If

            'Next

            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("chiave", "Nome", "string") With {._hidden = True})
            'If objParametriAgenda.Sa_Cod = 0 Then
            l.Add(New ColonneNome("sa_nome", "Centro", "string"))
            l.Add(New ColonneNome("sa_cod", "sa_cod", "number"))
            'End If
            l.Add(New ColonneNome("Campo", "chiave", "string"))
            l.Add(New ColonneNome("Validita_Inizio", "Inizio Validità", "date"))
            l.Add(New ColonneNome("Validita_Fine", "Fine Validità", "date"))
            l.Add(New ColonneNome("Gru_Des", "Gruppo Vegetale", "string"))
            l.Add(New ColonneNome("Gru_Cod", "Gru_Cod", "number"))
            l.Add(New ColonneNome("Veg_Des", "Specie", "string"))
            l.Add(New ColonneNome("Veg_Cod", "Veg_Cod", "number"))
            l.Add(New ColonneNome("Superficie_Totale", "Superficie Totale", "number"))
            l.Add(New ColonneNome("Superficie_Biologico", "Superficie Biologico", "number"))
            l.Add(New ColonneNome("Superficie_Convenzionale", "Superficie Convenzionale", "number"))
            l.Add(New ColonneNome("Superficie_Conversione", "Superficie Conversione", "number"))
            l.Add(New ColonneNome("Superficie_Catastale", "Superficie Catastale", "number"))

            l.Add(New ColonneNome("rif_alfanumerico", "Codice Campo", "string"))
            l.Add(New ColonneNome("sup_contratto", "Sup. Contratto", "string"))
            l.Add(New ColonneNome("filiera", "Filiera", "string"))

            l.Add(New ColonneNome("Attivo", "Attivo", "number"))

            l.Add(New ColonneNome("Data_Creazione", "Data Creazione", "date"))
            l.Add(New ColonneNome("Utente_Creazione", "Utente Creazione", "string"))
            l.Add(New ColonneNome("Data_Modifica", "Data Modifica", "date"))
            l.Add(New ColonneNome("Utente_Modifica", "Utente Modifica", "string"))


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
                                            Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)
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
            Dim objCampi As New AgronicaCoreAnagrafeBIZ.Campo_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim c As AgronicaCoreModelsSTD.anagrafiche.Campo

            c = objCampi.Leggi_Campo(InData.InData.Piva,
                                         InData.InData.Sa_Cod,
                                         InData.InData.Campo_Cod,
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

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_SpecieVegetali(ByVal objP_super_server As String,
                                         ByVal objP_server As String,
                                         ByVal objP_utenti As String) As RispostaStandard
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

            Dim objSpecVeg As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R()
            DT = objSpecVeg.Leggi_x_PDC(objParametri_Server)

            DT = DT.DefaultView.ToTable(True, "veg_cod", "veg_des")

            DT.Columns("veg_cod").ColumnName = "veg_cod"
            DT.Columns("veg_des").ColumnName = "veg_des"

            Dim serializerSettings As New JsonSerializerSettings()

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Newtonsoft.Json.Formatting.None, serializerSettings)
        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function
    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_Campi_Codici(ByVal objP_super_server As String,
                                         ByVal objP_server As String,
                                         ByVal objP_utenti As String) As RispostaStandard
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

            Dim obj_Codici As New JArray
            Dim StrCodiciCampo As String = " codice in (" & enum_CodiciAnagrafe.Sup_Contratto & "," & enum_CodiciAnagrafe.Filiera & ") "
            Dim objCodiceAnagrafe As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R
            Dim DTCod = objCodiceAnagrafe.Leggi(0,
                                     "",
                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                     StrCodiciCampo,
                                     "",
                                     objParametri_Server)

            DT = DTCod.DefaultView.ToTable(True, "codice", "descrizione")
            Dim serializerSettings As New JsonSerializerSettings()

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Newtonsoft.Json.Formatting.None, serializerSettings)
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
                                            ByVal InData As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG) As RispostaStandard
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

            Dim objAppezzamentoR As New AgronicaCoreAnagrafeDAL.Appezzamento_Read

            ' serve per ottenere una corretta lettura degli appezzamenti
            Dim data = New Date(1, 1, 1)

            DT = objAppezzamentoR.Recupera_Appezzamenti_Colture_del_Campo(
                                                         InData.Piva,
                                                         InData.Sa_Cod,
                                                         InData.Campo_Cod,
                                                         If(InData.Validita_Inizio = data, CostantiPersonalizzate.AGRODATAINIZIO, InData.Validita_Inizio),
                                                         If(InData.Validita_Fine = data, CostantiPersonalizzate.AGRODATAFINE, InData.Validita_Fine),
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
            l.Add(New ColonneNome("SUP_APP", "SUP_APP", "number"))
            l.Add(New ColonneNome("APP_NOME", "APP_NOME", "string"))
            l.Add(New ColonneNome("Validita_Inizio", "Validita_Inizio", "date"))
            l.Add(New ColonneNome("Validita_Fine", "Validita_Fine", "date"))
            l.Add(New ColonneNome("ID_REG", "ID_REG", "string"))
            l.Add(New ColonneNome("Impianto_Validita_Inizio", "Impianto_Validita_Inizio", "date"))
            l.Add(New ColonneNome("Impianto_Validita_Fine", "Impianto_Validita_Fine", "date"))
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
    Public Function Leggi_AppezzamentiCampo_NG(ByVal InData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)) As RispostaStandard

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

            Dim objAppezzamentoR As New AgronicaCoreAnagrafeDAL.Appezzamento_Read

            ' serve per ottenere una corretta lettura degli appezzamenti
            Dim data = New Date(1, 1, 1)

            DT = objAppezzamentoR.Recupera_Appezzamenti_Colture_del_Campo(
                                                         InData.InData.Piva,
                                                         InData.InData.Sa_Cod,
                                                         InData.InData.Campo_Cod,
                                                         If(InData.InData.Validita_Inizio = data, CostantiPersonalizzate.AGRODATAINIZIO, InData.InData.Validita_Inizio),
                                                         If(InData.InData.Validita_Fine = data, CostantiPersonalizzate.AGRODATAFINE, InData.InData.Validita_Fine),
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
            l.Add(New ColonneNome("SUP_APP", "SUP_APP", "number"))
            l.Add(New ColonneNome("APP_NOME", "APP_NOME", "string"))
            l.Add(New ColonneNome("Validita_Inizio", "Validita_Inizio", "date"))
            l.Add(New ColonneNome("Validita_Fine", "Validita_Fine", "date"))
            l.Add(New ColonneNome("ID_REG", "ID_REG", "string"))
            l.Add(New ColonneNome("Impianto_Validita_Inizio", "Impianto_Validita_Inizio", "date"))
            l.Add(New ColonneNome("Impianto_Validita_Fine", "Impianto_Validita_Fine", "date"))
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
    Public Function Leggi_ParticelleCampo(ByVal objP_super_server As String,
                                          ByVal objP_server As String,
                                          ByVal objP_utenti As String,
                                          ByVal InData As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG) As RispostaStandard
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

            Dim objCOM As New AgronicaCoreAnagrafeDAL.CampixParticelle_R
            Dim Dt_CXP As DataTable

            Dim objParticelleCatastali As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R
            DT = objParticelleCatastali.Recupera_Particelle_per_Centro(
                                                                        InData.Piva,
                                                                        InData.Sa_Cod,
                                                                        InData.Validita_Inizio,
                                                                        InData.Validita_Fine,
                                                                        objParametri_Server)

            'creo la lista delle colonne da visualizzare

            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("Prov", "Prov", "string"))
            l.Add(New ColonneNome("Com", "Com", "string"))
            l.Add(New ColonneNome("COMUNI_PROV", "COMUNI_PROV", "string") With {._hidden = True})
            l.Add(New ColonneNome("LOCALITA", "LOCALITA", "string"))
            l.Add(New ColonneNome("Part_Cod", "Part_Cod", "number"))
            l.Add(New ColonneNome("Sezione", "Sezione", "string"))
            l.Add(New ColonneNome("Foglio", "Foglio", "number"))
            l.Add(New ColonneNome("Numero", "Numero", "number"))
            l.Add(New ColonneNome("Subalterno", "Subalterno", "string"))
            l.Add(New ColonneNome("Superficie", "Superficie", "number"))
            l.Add(New ColonneNome("Particella_ettari", "Particella_ettari", "number"))
            l.Add(New ColonneNome("Particella_are", "Particella_are", "number"))
            l.Add(New ColonneNome("Particella_centiare", "Particella_centiare", "number"))
            l.Add(New ColonneNome("Sup_Condotta", "Sup_Condotta", "number"))
            l.Add(New ColonneNome("SuperficieDisponibile", "SuperficieDisponibile", "number"))
            l.Add(New ColonneNome("SuperficieUtilizzata", "SuperficieUtilizzata", "number"))


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
    Public Function LeggiParticellePerCentro(ByVal InData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)) As RispostaStandard
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

            Dim objCOM As New AgronicaCoreAnagrafeDAL.CampixParticelle_R
            Dim Dt_CXP As DataTable

            Dim objParticelleCatastali As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R
            DT = objParticelleCatastali.Recupera_Particelle_per_Centro(
                InData.InData.Piva,
                InData.InData.Sa_Cod,
                InData.InData.Validita_Inizio,
                InData.InData.Validita_Fine,
                objParametri_Server
                )

            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("Prov", "Prov", "string"))
            l.Add(New ColonneNome("Com", "Com", "string"))
            l.Add(New ColonneNome("COMUNI_PROV", "COMUNI_PROV", "string") With {._hidden = True})
            l.Add(New ColonneNome("LOCALITA", "LOCALITA", "string"))
            l.Add(New ColonneNome("Part_Cod", "Part_Cod", "number"))
            l.Add(New ColonneNome("Sezione", "Sezione", "string"))
            l.Add(New ColonneNome("Foglio", "Foglio", "number"))
            l.Add(New ColonneNome("Numero", "Numero", "number"))
            l.Add(New ColonneNome("Subalterno", "Subalterno", "string"))
            l.Add(New ColonneNome("Superficie", "Superficie", "number"))
            l.Add(New ColonneNome("Particella_ettari", "Particella_ettari", "number"))
            l.Add(New ColonneNome("Particella_are", "Particella_are", "number"))
            l.Add(New ColonneNome("Particella_centiare", "Particella_centiare", "number"))
            l.Add(New ColonneNome("Sup_Condotta", "Sup_Condotta", "number"))
            l.Add(New ColonneNome("SuperficieDisponibile", "SuperficieDisponibile", "number"))
            l.Add(New ColonneNome("SuperficieUtilizzata", "SuperficieUtilizzata", "number"))
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
    Public Function ScriviCampiAnagrafica(ByVal campo As AgronicaCoreModelsSTD.anagrafiche.Campo,
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
            Dim objCampoBIZ As New AgronicaCoreAnagrafeBIZ.Campo_W

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim objCampo As AgronicaCoreModelsSTD.anagrafiche.Campo = campo
            Dim risp = objCampoBIZ.Scrivi_Campo_Anagrafica(objCampo,
                                                            tipoOperazione,
                                                            objParametri_Server,
                                                            objParametri_Utenti
                                                            )


            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(campo, Newtonsoft.Json.Formatting.None)


        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = JsonConvert.SerializeObject(campo, Newtonsoft.Json.Formatting.None)
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}

        End Try

        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function ScriviCampiAnagrafica(ByVal InData As CoreWS_Generic(Of ScriviCampiAnagrafica)) As RispostaStandard
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
            Dim objCampoBIZ As New AgronicaCoreAnagrafeBIZ.Campo_W

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objCampo As AgronicaCoreModelsSTD.anagrafiche.Campo = InData.InData.campo
            Dim risp = objCampoBIZ.Scrivi_Campo_Anagrafica(objCampo,
                                                            InData.InData.tipoOperazione,
                                                            objParametri_Server,
                                                            objParametri_Utenti
                                                            )


            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(InData.InData.campo, Newtonsoft.Json.Formatting.None)


        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = JsonConvert.SerializeObject(InData.InData.campo, Newtonsoft.Json.Formatting.None)
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}

        End Try

        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function ScriviCampiAnagrafica_inLine(ByVal InData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG),
                                                 ByVal campo As AgronicaCoreModelsSTD.anagrafiche.Campo,
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
            Dim objCampoBIZ As New AgronicaCoreAnagrafeBIZ.Campo_W

            If tipoOperazione <> enum_TipoOperazioneDB.Scrittura Then
                Dim objCampi As New AgronicaCoreAnagrafeBIZ.Campo_R
                Dim campo_old = objCampi.Leggi_Campo(campo.primaryKey.centroAziendalePK.partitaIva,
                                             campo.primaryKey.centroAziendalePK.codice,
                                             campo.primaryKey.codice,
                                             objParametri_Server)

                campo_old.descrizione = campo.descrizione
                campo_old.orientamento_Colturale = campo.orientamento_Colturale
                campo_old.specie = campo.specie
                campo_old.validita = campo.validita
                campo_old.campo_Codice = campo.campo_Codice

                objCampo = campo_old
            Else
                objCampo = campo
            End If

            Dim risp = objCampoBIZ.Scrivi_Campo_Anagrafica(objCampo,
                                                            tipoOperazione,
                                                            objParametri_Server,
                                                            objParametri_Utenti
                                                            )

            r.RispostaOK = True
            r.RispostaStringa = "Operation Success"

        Catch ex As GiasException
            r.RispostaOK = False
            r.RispostaStringa = ""
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = ""
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function
    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function ScriviCampiAnagrafica_inLine(ByVal InData As CoreWS_Generic(Of ScriviCampiAnagraficaInLine)) As RispostaStandard

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

            Dim objCampo As AgronicaCoreModelsSTD.anagrafiche.Campo
            Dim objCampoBIZ As New AgronicaCoreAnagrafeBIZ.Campo_W

            If InData.InData.tipoOperazione <> enum_TipoOperazioneDB.Scrittura Then
                Dim objCampi As New AgronicaCoreAnagrafeBIZ.Campo_R
                Dim campo_old = objCampi.Leggi_Campo(InData.InData.campo.primaryKey.centroAziendalePK.partitaIva,
                                             InData.InData.campo.primaryKey.centroAziendalePK.codice,
                                             InData.InData.campo.primaryKey.codice,
                                             objParametri_Server)

                campo_old.descrizione = InData.InData.campo.descrizione
                campo_old.orientamento_Colturale = InData.InData.campo.orientamento_Colturale
                campo_old.specie = InData.InData.campo.specie
                campo_old.validita = InData.InData.campo.validita
                campo_old.campo_Codice = InData.InData.campo.campo_Codice

                objCampo = campo_old
            Else
                objCampo = InData.InData.campo
            End If

            Dim risp = objCampoBIZ.Scrivi_Campo_Anagrafica(objCampo,
                                                            InData.InData.tipoOperazione,
                                                            objParametri_Server,
                                                            objParametri_Utenti
                                                            )

            r.RispostaOK = True
            r.RispostaStringa = "Operation Success"


        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    Private Shared Sub LeggiCodiciCampi(ByVal Piva As String, ByRef codici As Dictionary(Of String, String), ByRef objParametri_Server As AgronicaCoreParametri)

        Dim objCampoCodici As New AgronicaCoreAnagrafeDAL.Campi_Codici_Read
        Dim dtCodCampo = objCampoCodici.Leggi2(Piva, 0, 0, enum_CodiciAnagrafe.Riferimento_Alfanumerico_Campo, "", AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO, AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
        Dim dtSupContratto = objCampoCodici.Leggi2(Piva, 0, 0, enum_CodiciAnagrafe.Sup_Contratto, "", AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO, AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
        Dim dtFiliera = objCampoCodici.Leggi2(Piva, 0, 0, enum_CodiciAnagrafe.Filiera, "", AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO, AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

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

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function LeggiCampi(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.Campo))

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
            Dim objCampi As New AgronicaCoreAnagrafeDAL.Campi_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim strCampi As String = JsonConvert.SerializeObject(InData.InData)
            Dim inDataCampi As AgronicaCoreDTOStd.InData.Metaschema.LeggiCampi = JsonConvert.DeserializeObject(Of AgronicaCoreDTOStd.InData.Metaschema.LeggiCampi)(strCampi,
                                                                                          New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local, .NullValueHandling = NullValueHandling.Ignore, .MissingMemberHandling = MissingMemberHandling.Ignore})


            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(inDataCampi.data, inDataCampi.data)


            Dim dt = objCampi.Leggi_x_anagrafica(inDataCampi.centro.primaryKey.partitaIva,
                                                 inDataCampi.centro.primaryKey.codice,
                                                 0,
                                                 "", "", objParametri_Server)

            Dim c = (From row In dt.Rows Select New AgronicaCoreModelsSTD.anagrafiche.Campo() With {
                                             .primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Campo.PK(
                                                        row("campo_cod"),
                                                        New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(row("chiave").ToString().Split("_")(1),
                                                                                                                 row("chiave").ToString().Split("_")(0))
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

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function LeggiCampi_perSpecieImpianti(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.Campo))

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
            Dim objCampi As New AgronicaCoreAnagrafeDAL.Campi_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)



            Dim strCampi As String = JsonConvert.SerializeObject(InData.InData)
            Dim inDataCampi As AgronicaCoreDTOStd.InData.Metaschema.LeggiCampi = JsonConvert.DeserializeObject(Of AgronicaCoreDTOStd.InData.Metaschema.LeggiCampi)(strCampi,
                                                                                          New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local, .NullValueHandling = NullValueHandling.Ignore, .MissingMemberHandling = MissingMemberHandling.Ignore})

            Dim veg_cod As Integer = 0
            Dim cul_cod As Integer = 0

            If inDataCampi.utilizzoTerreno IsNot Nothing Then
                If inDataCampi.utilizzoTerreno.classType = ClassType.Varieta Then
                    veg_cod = CType(inDataCampi.utilizzoTerreno, Varieta).specie.codice
                    cul_cod = CType(inDataCampi.utilizzoTerreno, Varieta).codice
                End If
            End If

            Dim strFiltroAggiuntivo = ""
            If cul_cod <> 0 Then
                strFiltroAggiuntivo &= " Cultivar.Cul_Cod = " & cul_cod
            End If

            If veg_cod <> 0 Then
                If strFiltroAggiuntivo <> "" Then
                    strFiltroAggiuntivo &= " AND "
                End If
                strFiltroAggiuntivo &= " SpecieVegetali.Veg_Cod = " & veg_cod
            End If

            Dim dt = objCampi.LeggiDatiImpianto_from_CampoCodData(inDataCampi.centro.primaryKey.partitaIva,
                                                                  inDataCampi.centro.primaryKey.codice,
                                                                  0,
                                                                  inDataCampi.data,
                                                                  strFiltroAggiuntivo,
                                                                  "",
                                                                objParametri_Server)

            Dim dtUnique = dt.DefaultView.ToTable(True, "campo_cod", "campo_des")

            Dim c = (From row In dtUnique.Rows Select New AgronicaCoreModelsSTD.anagrafiche.Campo() With {
                                             .primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Campo.PK(
                                                        row("campo_cod"),
                                                        New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(inDataCampi.centro.primaryKey.codice,
                                                                                                                 inDataCampi.centro.primaryKey.partitaIva)
                                             ),
                                             .descrizione = row("campo_des")}
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
    Public Function Test_Campi_Archivio_Lettura(ByVal InData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)) As rispostaStandard(Of Boolean)

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

            Dim objCampiDAL_R As New AgronicaCoreAnagrafeDAL.Campi_R
            Dim objCampiBIZ_R As New AgronicaCoreAnagrafeBIZ.Campo_R

            Dim dt = objCampiDAL_R.Leggi(InData.InData.Piva, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            r.ErroriGias = New List(Of ErroreGias)
            Dim i = 0
            For Each row In dt.Rows
                i += 1

                Try
                    Dim m = objCampiBIZ_R.Leggi_Campo(row("Piva"), row("Sa_Cod"), row("Campo_Cod"), objParametri_Server)

                Catch ex As Exception

                    r.ErroriGias.Add(New ErroreGias() With {
                                     .ex = ex.Message,
                                     .messaggio = "Errore Campo " & row("Piva") & "_" & row("Sa_Cod") & "_" & row("Appezza")}
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
    Public Function Test_Campi_Archivio_Scrittura(ByVal InData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)) As rispostaStandard(Of Boolean)

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

            Dim objCampiDAL_R As New AgronicaCoreAnagrafeDAL.Campi_R
            Dim objCampiBIZ_R As New AgronicaCoreAnagrafeBIZ.Campo_R
            Dim objCampiBIZ_W As New AgronicaCoreAnagrafeBIZ.Campo_W

            Dim dt = objCampiDAL_R.Leggi(InData.InData.Piva, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            r.ErroriGias = New List(Of ErroreGias)
            Dim i = 0
            For Each row In dt.Rows
                i += 1
                Dim m As AgronicaCoreModelsSTD.anagrafiche.Campo

                Try
                    m = objCampiBIZ_R.Leggi_Campo(row("Piva"), row("Sa_Cod"), row("Campo_Cod"), objParametri_Server)
                Catch ex As Exception

                    r.ErroriGias.Add(New ErroreGias() With {
                                     .ex = ex.Message,
                                     .messaggio = "Errore Lettura Campo " & row("Piva") & "_" & row("Sa_Cod") & "_" & row("Campo_Cod")}
                                     )

                End Try


                Try
                    If m IsNot Nothing Then
                        objCampiBIZ_W.Scrivi_Campo_Anagrafica(m, enum_TipoOperazioneDB.Modifica, objParametri_Server, objParametri_Utenti)
                    End If

                Catch ex As Exception

                    r.ErroriGias.Add(New ErroreGias() With {
                                     .ex = ex.Message,
                                     .messaggio = "Errore Scrittura Campo " & row("Piva") & "_" & row("Sa_Cod") & "_" & row("Campo_Cod")}
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

End Class