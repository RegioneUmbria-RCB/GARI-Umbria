Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreModello
Imports AgronicaCoreUtility
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreDTOStd.InData.Agenda
Imports AgronicaCoreDTOStd.InData.Visite

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class Visite
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_ListaVisite_ToKendoGrid(ByVal objP_server As String, ByVal objP_utenti As String) As RispostaStandard

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

            Dim v_R As New AgronicaCoreVisiteBIZ.Visite_R
            Dim dt As DataTable = v_R.Leggi("", 0, 0, "", "", objParametri_Server, objParametri_Utenti)

            'Aggiungo la colonna Azioni
            dt.Columns.Add(New DataColumn("Azioni"))

            'aggiungo i pulsanti per modifica ed eliminazione
            Dim listaBtn = New List(Of btnAzioni)

            'Controllo se l'utente ha i permessi per Modificare/cancellare le scadenze
            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim PermessiScrittura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                           objParametri_Utenti.UtenteUsername, TipiEnumerativi.enum_Id_Servizio.GiasOnline,
                                           enum_Security_Attivita.Visite_Lista,
                                           enum_Security_Operazione.Modifica,
                                           Date.Now, "", objParametri_Utenti)

            Dim utenteAbilitatoGestioneNuovoAllegato = objPermessi.Controlla_Permessi_Utente(objParametri_Utenti.UtenteUsername,
                                                                                    TipiEnumerativi.enum_Id_Servizio.GiasOnline,
                                                                                   enum_Security_Attivita.Documentale_Inser,
                                                                                   enum_Security_Operazione.Modifica,
                                                                                   Now, "",
                                                                                   objParametri_Utenti)

            Dim utenteAbilitatoGestioneVisualizaAllegato = objPermessi.Controlla_Permessi_Utente(objParametri_Utenti.UtenteUsername,
                                                                                        TipiEnumerativi.enum_Id_Servizio.GiasOnline,
                                                                                       enum_Security_Attivita.Documentale_Lista,
                                                                                       enum_Security_Operazione.Lettura,
                                                                                       Now, "",
                                                                                       objParametri_Utenti)

            If PermessiScrittura Then
                listaBtn.Add(New btnAzioni("fa-pencil-square-o edit_elem", ""))
                listaBtn.Add(New btnAzioni("fa-trash-o del_elem", ""))
            Else
                listaBtn.Add(New btnAzioni("fa-info info_elem", ""))
            End If

            If utenteAbilitatoGestioneNuovoAllegato Then
                listaBtn.Add(New btnAzioni("fa fa-file-text-o fa-2x visible_doc", ""))
            End If

            If utenteAbilitatoGestioneNuovoAllegato Then
                listaBtn.Add(New btnAzioni("fa fa-paperclip fa-2x add_doc", ""))
            End If

            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("Azioni", "Azioni", "string") With {._FormatoParticolare = New ToolStandard(listaBtn).toString(), ._Filtrabile = False, ._FiltrabileConCheck = False})
            l.Add(New ColonneNome("Data_Movimento", "Data Visita", "date"))
            l.Add(New ColonneNome("Ora_Movimento", "Orario", "string"))
            l.Add(New ColonneNome("Piva", "Partita Iva", "string") With {._Display = False})
            l.Add(New ColonneNome("rag_soc", "Ragione sociale", "string"))
            l.Add(New ColonneNome("Sa_Cod", "Sa_Cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("Centro_Aziendale", "Centro Aziendale", "string") With {._hidden = True})
            l.Add(New ColonneNome("Lav_Cod", "Lav_Cod", "number") With {._hidden = True})
            'l.Add(New ColonneNome("Lav_Des", "Lav_Des", "string") With {._hidden = True})
            'l.Add(New ColonneNome("Des_Lib", "Des_Lib", "string") With {._hidden = True})
            l.Add(New ColonneNome("Note", "Note", "string"))
            l.Add(New ColonneNome("Username_Creazione", "Username_Creazione", "string") With {._hidden = True})
            l.Add(New ColonneNome("Operatore", "Operatore", "string"))
            l.Add(New ColonneNome("AttivitaSvolte", "Attività Svolte", "string"))
            l.Add(New ColonneNome("Id_Agenda", "ID", "number") With {._Display = False})

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.Menu, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_ListaVisiteDettagli_ToKendoGrid(ByVal objP_server As String, ByVal objP_utenti As String) As RispostaStandard

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

            Dim v_R As New AgronicaCoreVisiteBIZ.Visite_R
            Dim dt As DataTable = v_R.LeggiDettagli("", 0, 0, "", "", objParametri_Server, objParametri_Utenti)

            'Aggiungo la colonna Azioni
            dt.Columns.Add(New DataColumn("Azioni"))

            'aggiungo i pulsanti per modifica ed eliminazione
            Dim listaBtn = New List(Of btnAzioni)

            'Controllo se l'utente ha i permessi per Modificare/cancellare le scadenze
            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim PermessiScrittura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                           objParametri_Utenti.UtenteUsername, TipiEnumerativi.enum_Id_Servizio.GiasOnline,
                                           enum_Security_Attivita.Visite_Lista,
                                           enum_Security_Operazione.Modifica,
                                           Date.Now, "", objParametri_Utenti)

            If PermessiScrittura Then
                listaBtn.Add(New btnAzioni("fa-pencil-square-o edit_elem", ""))
                'listaBtn.Add(New btnAzioni("fa-trash-o del_elem", ""))
            Else
                listaBtn.Add(New btnAzioni("fa-info info_elem", ""))
            End If

            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("Azioni", "Azioni", "string") With {._FormatoParticolare = New ToolStandard(listaBtn).toString(), ._Filtrabile = False, ._FiltrabileConCheck = False})
            l.Add(New ColonneNome("Data_Movimento", "Data Visita", "date"))
            l.Add(New ColonneNome("Ora_Movimento", "Orario", "string"))
            l.Add(New ColonneNome("Piva", "Partita Iva", "string") With {._Display = False})
            l.Add(New ColonneNome("rag_soc", "Ragione sociale", "string"))
            l.Add(New ColonneNome("Sa_Cod", "Sa_Cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("Sa_Nome", "Centro Aziendale", "string"))
            l.Add(New ColonneNome("Lav_Cod", "Lav_Cod", "number") With {._hidden = True})
            'l.Add(New ColonneNome("Lav_Des", "Lav_Des", "string") With {._hidden = True})
            'l.Add(New ColonneNome("Des_Lib", "Des_Lib", "string") With {._hidden = True})
            l.Add(New ColonneNome("Note", "Note", "string"))
            l.Add(New ColonneNome("Username_Creazione", "Username_Creazione", "string") With {._hidden = True})
            l.Add(New ColonneNome("Operatore", "Operatore", "string"))
            l.Add(New ColonneNome("Operazione", "Attività Svolta", "string"))
            l.Add(New ColonneNome("Descrizione", "Descrizione", "string"))
            l.Add(New ColonneNome("Id_Agenda", "ID", "number") With {._Display = False})

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.Menu, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_ListaVisiteDettagli_ToKendoGrid_new(InData As Object) As rispostaStandard(Of String)

        Dim r As New rispostaStandard(Of String)

        Try
            Dim datiRequest As String = JsonConvert.SerializeObject(InData)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local
            serializerSettings.NullValueHandling = NullValueHandling.Ignore
            serializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Visite.LeggiVisite))(datiRequest, serializerSettings)

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objParametri_Visite As AgronicaCoreDTOStd.InData.Visite.LeggiVisite = objRequest.InData

            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server
            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti

            Dim veg_cod As Integer = -1
            Dim id_cod As Integer = -1
            Dim xFiltroAggiuntivo As String = ""
            Dim usernameFilter As String = ""
            Dim pivaAzienda As String = ""
            Dim dataDa As Date
            Dim dataA As Date
            Dim tipoVisita As Integer = 0
            Dim pivaCentro As String = ""
            Dim saCodCentro As String = ""
            Dim operazioniFilter As Integer()
            Dim impArr As String()
            Dim Gen_Cod As Integer = -1
            Dim Spe_Cod As Integer = -1
            Dim IPro_Cod As Integer = -1

            If (objParametri_Visite.specie IsNot Nothing) Then
                If objParametri_Visite.specie.classType = "Varieta" Then
                    veg_cod = CType(objParametri_Visite.specie, AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta).specie.codice
                Else
                    id_cod = objParametri_Visite.specie.codice
                End If
            End If

            'costruisco il filtro aggiuntivo
            If objParametri_Visite.operatore.Username IsNot Nothing Then
                usernameFilter = objParametri_Visite.operatore.Username
            End If

            If objParametri_Visite.azienda IsNot Nothing Then
                pivaAzienda = objParametri_Visite.azienda.partitaIva
            End If

            If objParametri_Visite.tipoVisita <> 0 Then
                tipoVisita = objParametri_Visite.tipoVisita
            End If

            dataDa = objParametri_Visite.data_da
            dataA = objParametri_Visite.data_a

            If objParametri_Visite.centro_aziendale IsNot Nothing Then
                pivaCentro = objParametri_Visite.centro_aziendale.primaryKey.partitaIva
                saCodCentro = objParametri_Visite.centro_aziendale.primaryKey.codice
            End If

            impArr = objParametri_Visite.impianti.ToArray()

            If objParametri_Visite.operazioni IsNot Nothing Then
                operazioniFilter = objParametri_Visite.operazioni.ConvertAll(Of Integer)(Function(o As AgronicaCoreModelsSTD.attivita.AttivitaPersonalizzata) o.codice).ToArray()
            End If

            If objParametri_Visite.risorsaZootecnica IsNot Nothing Then
                Gen_Cod = objParametri_Visite.risorsaZootecnica.genere.codice
                Spe_Cod = objParametri_Visite.risorsaZootecnica.specie.codice
                IPro_Cod = objParametri_Visite.risorsaZootecnica.indirizzoProd.codice
            End If

            Dim v_R As New AgronicaCoreVisiteBIZ.Visite_R
            Dim dt As DataTable = v_R.LeggiDettagliVisiteNew(veg_cod, id_cod, usernameFilter, pivaAzienda, tipoVisita, dataDa, dataA, pivaCentro, saCodCentro, impArr, operazioniFilter, Gen_Cod, Spe_Cod, IPro_Cod, "", "", objParametri_Server, objParametri_Utenti, Nothing, objParametri_Visite.withDettaglioRilievo)

            For i = 0 To dt.Rows.Count - 1
                If dt.Rows(i).Item("Veg_Des") = "" AndAlso dt.Rows(i).Item("Id_Des") = "" Then
                    dt.Rows(i).Item("Veg_Des") = AgronicaCoreDataProvider.My.Resources.Gias.NessunaSpecie
                End If
            Next

            'Aggiungo la colonna Azioni
            dt.Columns.Add(New DataColumn("Azioni"))

            'aggiungo i pulsanti per modifica ed eliminazione
            Dim listaBtn = New List(Of btnAzioni)

            'Controllo se l'utente ha i permessi per Modificare/cancellare le scadenze
            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim PermessiScrittura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                           objParametri_Utenti.UtenteUsername, TipiEnumerativi.enum_Id_Servizio.GiasOnline,
                                           enum_Security_Attivita.Visite_Lista,
                                           enum_Security_Operazione.Modifica,
                                           Date.Now, "", objParametri_Utenti)

            If PermessiScrittura Then
                listaBtn.Add(New btnAzioni("fa-pencil-square-o edit_elem", ""))
                'listaBtn.Add(New btnAzioni("fa-trash-o del_elem", ""))
            Else
                listaBtn.Add(New btnAzioni("fa-info info_elem", ""))
            End If

            'Carico la lingua per i file resx delle traduzioni
            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                  AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                  "", "", objParametri_Server)

            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("Azioni", AgronicaCoreDataProvider.My.Resources.Gias.Azioni, "string") With {._FormatoParticolare = New ToolStandard(listaBtn).toString(), ._Filtrabile = False, ._FiltrabileConCheck = False})
            l.Add(New ColonneNome("Data_Movimento", AgronicaCoreDataProvider.My.Resources.Gias.DataVisita, "date"))
            l.Add(New ColonneNome("Ora_Movimento", AgronicaCoreDataProvider.My.Resources.Gias.Ora, "string"))
            l.Add(New ColonneNome("Piva", AgronicaCoreDataProvider.My.Resources.Gias.PartitaIva, "string") With {._Display = False})
            l.Add(New ColonneNome("rag_soc", AgronicaCoreDataProvider.My.Resources.Gias.RagioneSociale, "string"))
            l.Add(New ColonneNome("Sa_Cod", AgronicaCoreDataProvider.My.Resources.Gias.CodiceCentro, "number") With {._hidden = True})
            l.Add(New ColonneNome("Sa_Nome", AgronicaCoreDataProvider.My.Resources.Gias.CentroAziendale, "string"))
            l.Add(New ColonneNome("Lav_Cod", AgronicaCoreDataProvider.My.Resources.Gias.Operazione, "number") With {._hidden = True})
            l.Add(New ColonneNome("Id_Agenda_Rif", "Id_Agenda_Rif", "number") With {._hidden = True})
            l.Add(New ColonneNome("id_Attivita_Rif", "ID Attivita", "number") With {._hidden = True})
            l.Add(New ColonneNome("Validita_Inizio_Rif", "Validita_Inizio_Rif", "date") With {._hidden = True})
            l.Add(New ColonneNome("Blocco_Flag_Rif", "Blocco_Flag_Rif", "number") With {._hidden = True})
            l.Add(New ColonneNome("Lav_Cod_Rif", "Lav_Cod_Rif", "number") With {._hidden = True})
            l.Add(New ColonneNome("Lav_Des_Rif", "Lav_Des_Rif", "string") With {._hidden = True})
            l.Add(New ColonneNome("Sa_Cod_Rif", "Sa_Cod_Rif", "number") With {._hidden = True})
            'l.Add(New ColonneNome("Lav_Des", "Lav_Des", "string") With {._hidden = True})
            'l.Add(New ColonneNome("Des_Lib", "Des_Lib", "string") With {._hidden = True})
            'l.Add(New ColonneNome("Note", "Note", "string"))
            'l.Add(New ColonneNome("Username_Creazione", "Username_Creazione", "string") With {._hidden = True})
            l.Add(New ColonneNome("Operatore", AgronicaCoreDataProvider.My.Resources.Gias.Operatori, "string"))
            l.Add(New ColonneNome("Operazione", AgronicaCoreDataProvider.My.Resources.Gias.Attivita, "string"))
            l.Add(New ColonneNome("Lav_Des", AgronicaCoreDataProvider.My.Resources.Gias.Descrizione, "string"))
            l.Add(New ColonneNome("Stato_Visita", AgronicaCoreDataProvider.My.Resources.Gias.StatoVisita, "string"))
            l.Add(New ColonneNome("DaRemoto", AgronicaCoreDataProvider.My.Resources.Gias.InPresenza, "string"))
            l.Add(New ColonneNome("Appezza_Visite", AgronicaCoreDataProvider.My.Resources.Gias.AppezzamentoVisita, "string"))
            'l.Add(New ColonneNome("Nome_impianto", "Nome Impianto", "string"))
            'l.Add(New ColonneNome("Rac_Cod", "Codice Multi Attività", "number") With {._Display = False})
            l.Add(New ColonneNome("Latitude", AgronicaCoreDataProvider.My.Resources.Gias.Latitudine, "string"))
            l.Add(New ColonneNome("Longitude", AgronicaCoreDataProvider.My.Resources.Gias.Longitudine, "string"))

            l.Add(New ColonneNome("Id_Agenda", "ID", "number") With {._Display = False})

            l.Add(New ColonneNome("Veg_Des", AgronicaCoreDataProvider.My.Resources.Gias.SpecieVegetale, "string"))
            l.Add(New ColonneNome("Id_Des", AgronicaCoreDataProvider.My.Resources.Gias.DestinazioneUso, "string"))
            l.Add(New ColonneNome("IPro_Des", AgronicaCoreDataProvider.My.Resources.Gias.SpecieAnimale, "string"))
            l.Add(New ColonneNome("Lav_Des_Rif", AgronicaCoreDataProvider.My.Resources.Gias.RilievoAssociato, "string"))

            If objParametri_Visite.withDettaglioRilievo Then
                l.Add(New ColonneNome("Appezza_Rilievo", AgronicaCoreDataProvider.My.Resources.Gias.AppezzamentoRilievo, "string"))
                l.Add(New ColonneNome("Rilievo", AgronicaCoreDataProvider.My.Resources.Gias.IndiceRilievo, "string"))
                l.Add(New ColonneNome("LatitudeRilievo", AgronicaCoreDataProvider.My.Resources.Gias.LatitudeRilievo, "string"))
                l.Add(New ColonneNome("LongitudeRilievo", AgronicaCoreDataProvider.My.Resources.Gias.LongitudeRilievo, "string"))
                l.Add(New ColonneNome("Valore_Rilievo", AgronicaCoreDataProvider.My.Resources.Gias.ValoreRilievo, "string"))
                l.Add(New ColonneNome("Causali_Rilievo", AgronicaCoreDataProvider.My.Resources.Gias.CausaliRilievo, "string"))
            End If

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.Menu, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiCategorieVisiteLiv1(ByVal objP_server As String, ByVal objP_utenti As String) As RispostaStandard

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

            Dim dtProtocolli As New DataTable

            'Creo una lista di oggetti con l'elenco delle categorie
            Dim JArrayLista As New JArray()
            For Each dr In dtProtocolli.Rows
                JArrayLista.Add(New JObject(New JProperty("nome", dr.Item("nome_protocollo")), New JProperty("val", dr.Item("id_protocollo")), New JProperty("gruppo", "Protocolli"), New JProperty("preset", "")))
            Next

            JArrayLista.Add(New JObject(New JProperty("nome", "Visite"), New JProperty("val", -1), New JProperty("gruppo", "Altri"), New JProperty("preset", "")))
            JArrayLista.Add(New JObject(New JProperty("nome", "Rilievi"), New JProperty("val", -2), New JProperty("gruppo", "Altri"), New JProperty("preset", "")))
            JArrayLista.Add(New JObject(New JProperty("nome", "Audit"), New JProperty("val", -3), New JProperty("gruppo", "Altri"), New JProperty("preset", "")))

            'ritorno l'array trasformato in json 
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = JsonConvert.SerializeObject(JArrayLista, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiCategorieVisiteLiv2(ByVal objP_server As String, ByVal objP_utenti As String) As RispostaStandard

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

            'Dim filtroGruppi As String = " AND GRU_COD in (1,2,20) AND Operazioni.LAV_COD <> " & CostantiPersonalizzate.LAVCOD_VISITA
            Dim filtroOpImplementate As String = " AND Operazioni.LAV_COD IN (" & String.Join(",", LAVCOD_RILIEVO_AVVERSITA_CAMPO, LAVCOD_RILIEVO_INDICI_MATURITA, LAVCOD_FASI_FENOLOGICHE) & ")"
            Dim op_R As New Operazioni
            Dim rispOp As RispostaStandard = op_R.CaricaComboLavorazioni_ConFiltro(objP_server, objP_utenti, True, "", filtroOpImplementate)
            Dim dtProtocolli As New DataTable

            'Creo una lista di oggetti con l'elenco delle categorie
            Dim JArrayListaOld As JArray = JsonConvert.DeserializeObject(rispOp.RispostaStringa)

            Dim JArrayListaNew As New JArray
            For Each elem In JArrayListaOld
                'Dim gruppo As String = ""
                'Select Case elem("gru_cod")
                '    Case "1"
                '        gruppo = "Rilievi in Campo"
                '    Case "2"
                '        gruppo = "Rilievi alla Raccolta"
                '    Case "20"
                '        gruppo = "Audit/Monitoraggi"
                'End Select
                JArrayListaNew.Add(New JObject(New JProperty("nome", elem("lav_des")), New JProperty("val", elem("lav_cod")), New JProperty("gruppo", elem("gru_des"))))
            Next

            'Carico le attività associate alle visite generiche
            Dim axo_R As New AgronicaCoreContabDAL.AttivitaXOperazioni_R
            Dim dtAxo As DataTable = axo_R.Leggi(0, CostantiPersonalizzate.LAVCOD_VISITA, "", "", objParametri_Server)
            For Each dr As DataRow In dtAxo.Rows
                'JArrayListaNew.Add(New JObject(New JProperty("nome", dr("Descrizione")), New JProperty("val", dr("lav_cod") & "|" & dr("ID_Attivita")), New JProperty("gruppo", "Visite Personalizzate")))
                JArrayListaNew.Add(New JObject(New JProperty("nome", dr("Descrizione")), New JProperty("val", CostantiPersonalizzate.LAVCOD_ALTRE_OPERAZIONI & "|" & dr("ID_Attivita")), New JProperty("gruppo", "Visite Personalizzate")))
            Next

            'ritorno l'array trasformato in json 
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaNew, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiCategorieVisiteOperazioni(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.attivita.Lavorazione))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.attivita.Lavorazione))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If
        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If
        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            'Dim filtroGruppi As String = " AND GRU_COD in (1,2,20) AND Operazioni.LAV_COD <> " & CostantiPersonalizzate.LAVCOD_VISITA
            Dim filtroOpImplementate As String = "Operazioni.LAV_COD IN (" & String.Join(",", LAVCOD_RILIEVO_AVVERSITA_CAMPO, LAVCOD_RILIEVO_INDICI_MATURITA, LAVCOD_FASI_FENOLOGICHE, LAVCOD_ALTRE_OPERAZIONI) & ")"
            Dim op_R As New Operazioni
            Dim objOperazioniLeggi As New AgronicaCoreMetaSchemaDAL.Operazioni_R
            Dim DTOperazioni As DataTable = objOperazioniLeggi.Leggi(0, 0, 0, "", 0, "", "", False, False, False, False,
                                                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                                filtroOpImplementate, "", objParametri_Server)

            Dim ArrayListaNew As New List(Of AgronicaCoreModelsSTD.attivita.Lavorazione)
            For Each elem In DTOperazioni.Rows
                'Dim gruppo As String = ""
                'Select Case elem("gru_cod")
                '    Case "1"
                '        gruppo = "Rilievi in Campo"
                '    Case "2"
                '        gruppo = "Rilievi alla Raccolta"
                '    Case "20"
                '        gruppo = "Audit/Monitoraggi"
                'End Select
                ArrayListaNew.Add(New AgronicaCoreModelsSTD.attivita.Lavorazione(elem("lav_cod"), elem("lav_des")))
            Next

            'ritorno l'array trasformato in json 
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = ArrayListaNew
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiCategorieVisiteAttivita(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.attivita.AttivitaPersonalizzata))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.attivita.AttivitaPersonalizzata))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If
        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If
        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim ArrayListaNew As New List(Of AgronicaCoreModelsSTD.attivita.AttivitaPersonalizzata)

            Dim objAttivita As New AgronicaCoreContabBIZ.AttivitaPersonalizzata
            ArrayListaNew = objAttivita.LeggiAttivitaPersonalizzata(0, CostantiPersonalizzate.LAVCOD_VISITA, objParametri_Server, verbose:=True)


            'ritorno l'array trasformato in json 
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = ArrayListaNew
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiUserTecnico(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of String)

        Dim r As New rispostaStandard(Of String)

        Try

            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim contattiBIZ As New AgronicaCoreAnagrafeBIZ.Contatti_MultiHost_R

            Dim result As String = contattiBIZ.LeggiUserTecnicoOCapo(objParametri_Utenti.UtenteUsername, objParametri_Server)

            r.RispostaStringa = result
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiListaTecnici(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of String)

        Dim r As New rispostaStandard(Of String)

        Try

            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim contattiBIZ As New AgronicaCoreAnagrafeBIZ.Contatti_MultiHost_R

            Dim result As DataTable = contattiBIZ.LeggiListaTecnici(objParametri_Utenti, objParametri_Server)

            Dim jArrayListaOp As New JArray()

            For Each dr As DataRow In result.Rows
                Dim utente As String = Trim(dr.Item("Nome") & " " & dr.Item("Cognome"))

                If utente = "" Then
                    utente = Trim(dr.Item("username"))
                End If

                jArrayListaOp.Add(New JObject(New JProperty("username", dr.Item("username")),
                                              New JProperty("Codice_Fiscale", dr.Item("Codice_Fiscale")),
                                              New JProperty("nome_cognome", utente),
                                              New JProperty("Cod_RisUm", dr.Item("Cod_RisUm"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiListaAziende(InData As Object) As rispostaStandard(Of String)

        Dim r As New rispostaStandard(Of String)

        Try

            Dim datiRequest As String = JsonConvert.SerializeObject(InData)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local
            serializerSettings.NullValueHandling = NullValueHandling.Ignore
            serializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Visite.LeggiAziende))(datiRequest, serializerSettings)

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objParametri_Aziende As AgronicaCoreDTOStd.InData.Visite.LeggiAziende = objRequest.InData

            Dim objBIZ As New AgronicaCoreAnagrafeBIZ.Contatti_MultiHost_R

            Dim DT As DataTable

            DT = objBIZ.LeggiListaAziende(objParametri_Utenti,
                                          objParametri_Server,
                                          objParametri_Aziende.usernameOperatore
                                          )

            Dim jArrayListaOp As New JArray()

            For Each dr As DataRow In DT.Rows

                jArrayListaOp.Add(New JObject(New JProperty("partitaIva", dr.Item("PIVA")),
                                              New JProperty("ragioneSociale", dr.Item("rag_soc"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function VisiteScrivi_APP(
        ByVal objP_super_server As String,
        ByVal objP_server As String,
        ByVal objP_utenti As String,
        ByVal VisiteDaMemorizzare As VisitePerScarico
        ) As RispostaStandard

        Dim result As New RispostaStandard

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim unid As String = VisiteDaMemorizzare.guid
            Dim cancellazione As Boolean = False
            Dim importazione As Boolean = False

            If String.IsNullOrEmpty(unid) Then
                unid = Guid.NewGuid().ToString()
            Else
                cancellazione = True
            End If

            Dim scriviVisiteAPP As New AgronicaCoreVisiteBIZ.Visite_APP
            scriviVisiteAPP.ScriviVisiteAPP(
                unid,
                VisiteDaMemorizzare.Visite,
                VisiteDaMemorizzare.VisiteDettagli,
                VisiteDaMemorizzare.VisiteDestinazioni,
                objParametri_Server,
                cancellazione
            )

            ' scrivo documenti allegati alle visite dell'app
            If VisiteDaMemorizzare.VisiteDocumenti IsNot Nothing AndAlso VisiteDaMemorizzare.VisiteDocumenti.Count > 0 Then
                scriviVisiteAPP.ScriviDocumentiVisiteAPP(unid, VisiteDaMemorizzare.VisiteDocumenti, objParametri_Server)
            End If

            ' importazione dati app
            If importazione Then
                Dim msgFinale As New StringBuilder
                Dim xScrittura As New AgronicaCoreVisiteBIZ.Visite_APP
                Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R
                Dim dt As DataTable = objUtenti.Leggi("", "", objParametri_Utenti)
                Dim progressivoGias As Integer = dt.Rows(0).Item("ProgressivoGIAS")
                Dim esito As Boolean = xScrittura.ImportaVisiteAPP("", progressivoGias, msgFinale, objParametri_Server, unid)
            End If

            result.RispostaStringa = unid
            result.RispostaOK = True

        Catch ex As Exception

            result.RispostaOK = False
            result.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return result
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ImportaVisiteAPP(ByVal Piva As String, ByVal objP_server As String, ByVal objP_utenti As String) As RispostaStandard
        Dim r As New RispostaStandard
        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R
            Dim dt As DataTable = objUtenti.Leggi("", "", objParametri_Utenti)
            Dim progressivoGias As Integer = dt.Rows(0).Item("ProgressivoGIAS")

            Dim xScrittura As New AgronicaCoreVisiteBIZ.Visite_APP
            Dim msgFinale As New StringBuilder
            Dim esitoFinale As Boolean = xScrittura.ImportaVisiteAPP(Piva, progressivoGias, msgFinale, objParametri_Server)
            r.RispostaOK = esitoFinale
            If esitoFinale = True Then
                r.RispostaStringa = msgFinale.ToString
            Else
                r.Errore = msgFinale.ToString
            End If
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiListaAgenzie(InData As Object) As rispostaStandard(Of String)

        Dim r As New rispostaStandard(Of String)

        Try

            Dim datiRequest As String = JsonConvert.SerializeObject(InData)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local
            serializerSettings.NullValueHandling = NullValueHandling.Ignore
            serializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Visite.LeggiAziende))(datiRequest, serializerSettings)

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objParametri_Aziende As AgronicaCoreDTOStd.InData.Visite.LeggiAziende = objRequest.InData

            Dim objBIZ As New AgronicaCoreVisiteBIZ.Visite_R

            Dim DT As DataTable

            DT = objBIZ.Leggi_Agenzie_Visibilita_Utente_Visite(objParametri_Utenti,
                                                              objParametri_Server,
                                                              objParametri_Aziende.usernameOperatore
                                                              )

            Dim jArrayListaOp As New JArray()

            For Each dr As DataRow In DT.Rows

                jArrayListaOp.Add(New JObject(New JProperty("partitaIva", dr.Item("PIVA")),
                                              New JProperty("ragioneSociale", dr.Item("rag_soc"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiRisorseZootecniche(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.attivita.risorse.RisorsaZootecnica))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.attivita.risorse.RisorsaZootecnica))

        Try

            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objBIZ As New AgronicaCoreVisiteBIZ.Visite_R

            Dim result As List(Of AgronicaCoreModelsSTD.attivita.risorse.RisorsaZootecnica) = objBIZ.LeggiRisorseZootecniche(objParametri_Utenti, objParametri_Server)

            r.RispostaStringa = result
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function EliminaRilieviVisite(InData As CoreWS_Generic(Of Elimina_Rilievi_Visita)) As RispostaStandard

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim objP_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
        Dim objP_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

        Try

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objP_Server)

            Dim eliminaRilieviVisita As String = JsonConvert.SerializeObject(InData.InData)
            Dim params As Elimina_Rilievi_Visita = JsonConvert.DeserializeObject(Of Elimina_Rilievi_Visita)(eliminaRilieviVisita)

            Dim objBIZ As New AgronicaCoreVisiteBIZ.Visite_W
            Dim esito As Boolean = objBIZ.EliminaRilieviVisite(params.Piva, params.idAgendaVisita, objP_Server, objP_Utenti)

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objP_Server)

            Return ProvideRispostaStandardFrom(esito)

        Catch ex As Exception

            If Not objP_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objP_Server)
            End If

            Return MessaggioErroreFrom(ex)

        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objP_Server)

        End Try

    End Function

End Class
