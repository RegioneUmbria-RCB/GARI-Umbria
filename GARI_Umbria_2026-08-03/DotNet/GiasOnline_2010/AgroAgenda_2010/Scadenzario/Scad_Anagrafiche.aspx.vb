Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreVarieBIZ
Imports System.Web.Services
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports Newtonsoft.Json.Linq
Imports System.Web.Script.Serialization
Imports AgroAgenda_2010.Resources

Public Class Scad_Anagrafiche
    Inherits System.Web.UI.Page

    Public objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public objparametri_server_string, objparametri_utenti_string As String

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub

#Region "script services Gestione Indici di Ricerca"

    <WebMethod(EnableSession:=True)>
    Public Shared Function Ricerca_Alert_Indici() As RispostaStandard

        Dim r As New RispostaStandard
        Dim DT As New DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim leggi As New AgronicaCoreScadenziario.Alert_Indice_R
            DT = leggi.Leggi("", 0, objParametri_Server)

            DT.Columns.Add(New DataColumn("TipoCampo_Des", GetType(String)))
            DT.Columns.Add(New DataColumn("Obbligatorio", GetType(String)))
            DT.Columns.Add(New DataColumn("Validita_Inizio_Short", GetType(String)))
            DT.Columns.Add(New DataColumn("Validita_Fine_Short", GetType(String)))

            If DT.Rows.Count > 0 Then

                For Each dr As DataRow In DT.Rows

                    dr.Item("Validita_Inizio_Short") = Format(dr.Item("Validita_Inizio"), "dd/MM/yyyy").ToString
                    dr.Item("Validita_Fine_Short") = Format(dr.Item("Validita_Fine"), "dd/MM/yyyy").ToString

                    Select Case dr.Item("ChkObbligatorio")
                        Case 0 : dr("Obbligatorio") = AgronicaAgenda_2010.No
                        Case 1 : dr("Obbligatorio") = AgronicaAgenda_2010.Si
                    End Select

                    Select Case dr.Item("TipoCampo")
                        Case 0

                            dr("TipoCampo_Des") = AgronicaAgenda_2010.LiberaImputazione
                            Select Case dr("TipoDato")
                                Case "string" : dr("TipoCampo_Des") = dr("TipoCampo_Des") & " " & AgronicaAgenda_2010.Carattere
                                Case "numeric" : dr("TipoCampo_Des") = dr("TipoCampo_Des") & " " & AgronicaAgenda_2010.Numerica
                                Case "date" : dr("TipoCampo_Des") = dr("TipoCampo_Des") & " " & AgronicaAgenda_2010.Data
                                Case "boolean" : dr("TipoCampo_Des") = dr("TipoCampo_Des") & " " & AgronicaAgenda_2010.Booleano

                            End Select

                        Case 1
                            dr("TipoCampo_Des") = AgronicaAgenda_2010.SceltaValori
                        Case 2

                            dr("TipoCampo_Des") = AgronicaAgenda_2010.SceltaElenco
                            Select Case dr("Elenco_Tipo")

                                Case 1 : dr("TipoCampo_Des") = dr("TipoCampo_Des") & " " & AgronicaAgenda_2010.Imprese
                                Case 2 : dr("TipoCampo_Des") = dr("TipoCampo_Des") & " " & AgronicaAgenda_2010.Contatti
                                Case 3 : dr("TipoCampo_Des") = dr("TipoCampo_Des") & " " & AgronicaAgenda_2010.RapportiContabili
                                Case 4 : dr("TipoCampo_Des") = dr("TipoCampo_Des") & " " & AgronicaAgenda_2010.SpecieVegetali
                                Case 5 : dr("TipoCampo_Des") = dr("TipoCampo_Des") & " " & AgronicaAgenda_2010.Macchine
                                Case 6 : dr("TipoCampo_Des") = dr("TipoCampo_Des") & " " & AgronicaAgenda_2010.CentriAziendali
                                Case 7 : dr("TipoCampo_Des") = dr("TipoCampo_Des") & " " & AgronicaAgenda_2010.Campi
                                Case 8 : dr("TipoCampo_Des") = dr("TipoCampo_Des") & " " & AgronicaAgenda_2010.Appezzamenti

                                Case Else

                                    '.....

                            End Select


                    End Select

                Next

            End If

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
#End Region

#Region "Carica Indici per ListBox"
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Alert_Indici_ListBox() As RispostaStandard
        Return IndiciXTipologie_UC.Ricerca_Alert_Indici_UC()
    End Function
#End Region


#Region "Scrittura su Alert_IndicexTipologia"
    <WebMethod(EnableSession:=True)>
    Public Shared Function Insert_Alert_IndicexTipologia(ByVal Id_Indice As Integer, ByVal ChkObbligatorio As Integer, ByVal ID_Tipologia As Integer, ByVal ID_Area As Integer) As RispostaStandard
        Return IndiciXTipologie_UC.Insert_Alert_IndicexTipologia_UC(Id_Indice, ChkObbligatorio, ID_Tipologia, ID_Area)
    End Function
#End Region


#Region "Caricamento ListBox IndiciXTipologie"
    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiIndicexTipologia(ByVal ID_Tipologia As Integer, ByVal ID_Area As Integer, ByVal ID_IndiceXTipo As Integer) As RispostaStandard
        Return IndiciXTipologie_UC.LeggiIndicexTipologia_UC(ID_Tipologia, ID_Area, ID_IndiceXTipo)
    End Function
#End Region



#Region "Modifica Ordine su Alert_IndicexTipologia"
    <WebMethod(EnableSession:=True)>
    Public Shared Function Modifica_Ordine_Alert_IndicexTipologia(ByVal lista As String) As RispostaStandard
        Return IndiciXTipologie_UC.Modifica_Ordine_Alert_IndicexTipologia_UC(lista)
    End Function
#End Region


#Region "Modifica Obbligo su Alert_IndicexTipologia"
    <WebMethod(EnableSession:=True)>
    Public Shared Function Modifica_Obbligo_Alert_IndicexTipologia(ByVal Id_Indice As Integer, ByVal ID_Tipologia As Integer, ByVal ID_Area As Integer, ByVal Obbligatorio As Double) As RispostaStandard
        Return IndiciXTipologie_UC.Modifica_Obbligo_Alert_IndicexTipologia(Id_Indice, ID_Tipologia, ID_Area, Obbligatorio)
    End Function
#End Region

#Region "Cancella su Alert_IndicexTipologia"
    <WebMethod(EnableSession:=True)>
    Public Shared Function Cancella_Alert_IndicexTipologia(ByVal Id_Indice As Integer, ByVal ID_Tipologia As Integer, ByVal ID_Area As Integer) As RispostaStandard
        Return IndiciXTipologie_UC.Cancella_Alert_IndicexTipologia_UC(Id_Indice, ID_Tipologia, ID_Area)
    End Function

#End Region


#Region "Carica Griglia IndiciXTipologie"
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Griglia_IndiciXTipologie(ByVal ID_Area As Integer, ByVal ID_Tipologia As Integer) As RispostaStandard
        Return IndiciXTipologie_UC.Carica_griglia_IndiciXTipologie_UC(ID_Area, ID_Tipologia)
    End Function
#End Region

#Region "Carica Griglia Utenti"
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Griglia_Utenti() As RispostaStandard
        Return CategTipolDocumentoXUtenti_UC.Carica_Griglia_Utenti()
    End Function
#End Region

#Region "Carica Griglia Categoria Tipololgia"
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Griglia_Categoria_Tipologia() As RispostaStandard
        Return CategTipolDocumentoXUtenti_UC.Carica_Griglia_Categoria_Tipologia()
    End Function
#End Region

#Region "Carica Griglia Autorizzazioni Utenti per Categoria Tipololgia"
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Griglia_Categoria_TipologiaXUtenti(ByVal piva As String) As RispostaStandard
        Lingua.Gias_InizializzaCultura_DaSession()
        Return CategTipolDocumentoXUtenti_UC.Carica_Griglia_Categoria_TipologiaXUtenti(piva)
    End Function
#End Region

#Region "Scrittura Griglia Autorizzazioni Utenti per Categoria Tipololgia"
    <WebMethod(EnableSession:=True)>
    Public Shared Function Scrivi_Griglia_Categoria_TipologiaXUtenti(ByVal piva As String, ByVal modelutenti As String, ByVal modelcategoriatipologia As String, ByVal Autorizzato As Integer) As RispostaStandard
        Return CategTipolDocumentoXUtenti_UC.Scrivi_Griglia_Categoria_TipologiaXUtenti(piva, modelutenti, modelcategoriatipologia, Autorizzato)
    End Function
#End Region

#Region "Modifica Griglia Autorizzazioni Utenti per Categoria Tipololgia"
    <WebMethod(EnableSession:=True)>
    Public Shared Function Modifica_Griglia_Categoria_TipologiaXUtenti(ByVal piva As String, ByVal modelutenti As String, ByVal modelcategoriatipologia As String, ByVal modelcategoriatipologiaxutenti As String) As RispostaStandard
        Return CategTipolDocumentoXUtenti_UC.Modifica_Griglia_Categoria_TipologiaXUtenti(piva, modelutenti, modelcategoriatipologia, modelcategoriatipologiaxutenti)
    End Function
#End Region

#Region "Cancella Griglia Autorizzazioni Utenti per Categoria Tipololgia"
    <WebMethod(EnableSession:=True)>
    Public Shared Function Cancella_Griglia_Categoria_TipologiaXUtenti(ByVal piva As String, ByVal modelcategoriatipologiaxutenti As String) As RispostaStandard
        Return CategTipolDocumentoXUtenti_UC.Cancella_Griglia_Categoria_TipologiaXUtenti(piva, modelcategoriatipologiaxutenti)
    End Function
#End Region

    <WebMethod(EnableSession:=True)>
    Public Shared Function Carico_Autorizzazione_UtentixCategoriaTipologia(ByVal piva As String) As RispostaStandard
        Return CategTipolDocumentoXUtenti_UC.Carico_Autorizzazione_UtentixCategoriaTipologia(piva)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Delete_Alert_Indice(ByVal piva As String,
                                               ByVal id_indice As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim DT As New DataTable

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If
        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim FiltroAggiuntivo As String = "Alert_EntitaxIndici.ID_indice = " & id_indice

            Dim leggi As New AgronicaCoreScadenziario.Alert_Indice_R
            DT = leggi.LeggiEntitaxIndici("", 0, FiltroAggiuntivo, objParametriServer)

            Select Case DT.Rows.Count

                Case 0 'Cancellazione Consentita

                    Dim objIndice As New AgronicaCoreScadenziario.Alert_Indice_W
                    Dim objIndiceDettaglio As New AgronicaCoreScadenziario.Alert_Indice_Dettagli_W

                    'Cancellazione
                    objIndiceDettaglio.Cancella("", id_indice, 0, objParametriServer)
                    objIndice.Cancella("", id_indice, objParametriServer)

                    'Cancellazione Ok
                    r.Errore = ""



                Case Else

                    'Cancellazione Negata
                    r.Errore = "-1"

            End Select

            r.RispostaStringa = CStr(id_indice)
            r.RispostaOK = True



        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = 0

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=False)
        End Try

        Return r

    End Function

#Region "Controlla Prima di eliminare una Categoria o Tipologia se ci sono degli Indici o degli Utenti associati"
    <WebMethod(EnableSession:=True)>
    Public Shared Function ComponiMessaggioDiAvvisoCancellazioneAreaTipologia(ByVal ID_Area As Integer, ByVal ID_Tipologia As Integer) As RispostaStandard
        Return Scad_CategorieTipologie_UC.ComponiMessaggioDiAvvisoCancellazioneAreaTipologia(ID_Area, ID_Tipologia)
    End Function
#End Region


    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaConfigurazioniSchemaDocumenti() As RispostaStandard
        Return SchemaDocumenti_UC.CaricaConfigurazioniSchemaDocumenti()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Servizi_LeggiElencoDropdown() As RispostaStandard
        Return SchemaDocumenti_UC.Servizi_LeggiElencoDropdown()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Stato_Da_LeggiElencoDropdown(Servizio_Cod) As RispostaStandard
        Return SchemaDocumenti_UC.Stato_Da_LeggiElencoDropdown(Servizio_Cod)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Stato_A_LeggiElencoDropdown(Servizio_Cod) As RispostaStandard
        Return SchemaDocumenti_UC.Stato_A_LeggiElencoDropdown(Servizio_Cod)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Tipologia_LeggiElencoDropdown() As RispostaStandard
        Return SchemaDocumenti_UC.Tipologia_LeggiElencoDropdown()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Ambito_LeggiElencoDropdown() As RispostaStandard
        Return SchemaDocumenti_UC.Ambito_LeggiElencoDropdown()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Fase_LeggiElencoDropdown() As RispostaStandard
        Return SchemaDocumenti_UC.Fase_LeggiElencoDropdown()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Firmato_LeggiElencoDropdown() As RispostaStandard
        Return SchemaDocumenti_UC.Firmato_LeggiElencoDropdown()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Obbligatorio_LeggiElencoDropdown() As RispostaStandard
        Return SchemaDocumenti_UC.Obbligatorio_LeggiElencoDropdown()
    End Function



    <WebMethod(EnableSession:=True)>
    Public Shared Function SchemaDocumenti_SalvaGriglia(ByVal righeCancellate As String,
                                                        ByVal righeInserite As String,
                                                        ByVal righeModificate As String) As RispostaStandard
        Return SchemaDocumenti_UC.SchemaDocumenti_SalvaGriglia(righeCancellate, righeInserite, righeModificate)
    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Master().Lbl_Titolo.Text = DirectCast(GetLocalResourceObject("GestioneParametriIndici"), String)


        Response.Expires = 0

        hdPiva.Value = Stringa_Decodifica(CStr(Request.QueryString("p")),
                                          AgroKey_EncoderDecoder,
                                          Server)

        hdPiva_Codificata.Value = Stringa_Codifica(hdPiva.Value, AgroKey_EncoderDecoder, HttpContext.Current.Session)


        hdPaginaRedirect.Value = ""
        If Request.QueryString("origine") IsNot Nothing Then

            hdPaginaRedirect.Value = Stringa_Decodifica(CStr(Request.QueryString("origine")),
                                            AgroKey_EncoderDecoder,
                                            Server)
        Else
            hdPaginaRedirect.Value = "../Menu/MenuBS_Agenda_Nuovo.aspx"
        End If


        If Request.QueryString("tab_default") IsNot Nothing Then
            hdTabDefault.Value = CStr(Request.QueryString("tab_default"))
        Else
            hdTabDefault.Value = ""
        End If


        'Controllo se la sessione è ancora su
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If

        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objparametri_server_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Server)

        'Controllo se l'utente ha i permessi per accedere
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

        'Scadenzario/Lettura controllato nella master

        Dim UtenteAbilitatoLettura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                           Session("ASG_Utente_Username"),
                                           Session("ASG_IdServizio"),
                                           enum_Security_Attivita.Scadenzario_IndiciRicerca,
                                           enum_Security_Operazione.Lettura,
                                           Date.Now,
                                           "",
                                           objParametri_Utenti)

        Dim UtenteAbilitatoScrittura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                           Session("ASG_Utente_Username"),
                                           Session("ASG_IdServizio"),
                                           enum_Security_Attivita.Scadenzario_IndiciRicerca,
                                           enum_Security_Operazione.Modifica,
                                           Date.Now,
                                           "",
                                           objParametri_Utenti)

        'Imposto le variabili di ponte con il client
        hf_UtenteAbilitatoScrittura.Value = UtenteAbilitatoLettura
        hf_UtenteAbilitatoScrittura.Value = UtenteAbilitatoScrittura


        ' Anna 23/07/21: Aggiunta button per spostare documenti su DB
        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim StringImpostazioni = objUtenti.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SUPERUSER_DOCUMENTALE_SALVA_ALLEGATO_SU_DB,
                                                                                             objParametri_Utenti, 2)
        If Not String.IsNullOrEmpty(StringImpostazioni) AndAlso CInt(StringImpostazioni) = 1 AndAlso objParametri_Utenti.UtenteUsername.ToLower = objParametri_Utenti.SuperUserUsername.ToLower Then
            hf_UtenteAbilitatoExportSuDB.Value = "True"
        Else
            hf_UtenteAbilitatoExportSuDB.Value = "False"
        End If

        If Not UtenteAbilitatoLettura Then
            Response.Redirect("~/Classi/Agro_Pages/AccessoNonConsentito.aspx")
        End If

        'Anna 17/05/22: aggiunto limite alla cancellazione di legame tra Tipologie x Indici riservati (solo SuperUser)
        If objParametri_Utenti.UtenteUsername.ToLower = objParametri_Utenti.SuperUserUsername.ToLower Then
            hf_UtenteAbilitatoModificaTipologiexIndiciProtetti.Value = "True"
        Else
            hf_UtenteAbilitatoModificaTipologiexIndiciProtetti.Value = "False"
        End If

    End Sub

    'Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

    '    Dim objParametriAgenda As New AgronicaCoreModello.ParametriAgenda_Temp.ParametriAgenda()

    '    Dim link As String = ""
    '    Try
    '        Dim sitoorigine As Enum_SiteRedirector = HttpContext.Current.Session("Sito_Origine")
    '        Dim paginaOnLineRitorno As Integer = objParametriAgenda.PaginaSitoOrigine

    '        If sitoorigine = Enum_SiteRedirector.Sito_GiasOnline_2010 And paginaOnLineRitorno = enum_PagineGiasOnline_2010.Menu Then

    '            link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri(
    '                                   Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
    '                                   enum_PagineGiasOnline_2010.Menu,
    '                                   enum_PagineAgenda_2010.Menu, objParametriAgenda.Piva, "", "", 0, "")

    '        ElseIf sitoorigine = Enum_SiteRedirector.Sito_GiasOnline And paginaOnLineRitorno = enum_PagineGiasOnline.MenuCartellaAziendale Then

    '            objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
    '            objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Scadenzario_Lista
    '            link = AgronicaCoreModello.Utility_Operazioni.Link_GiasOnline_STR(enum_PagineGiasOnline.MenuCartellaAziendale, objParametriAgenda)

    '        Else
    '            link = CType(Master.Master, AgendaBootstrap).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
    '        End If

    '    Catch ex As Exception
    '        link = CType(Master.Master, AgendaBootstrap).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
    '    End Try

    '    Response.Redirect(link)
    'End Sub


    Public Shadows ReadOnly Property Master() As AgroAgenda_2010.AgendaBootstrap
        Get
            Return CType(MyBase.Master, AgroAgenda_2010.AgendaBootstrap)
        End Get
    End Property

End Class

