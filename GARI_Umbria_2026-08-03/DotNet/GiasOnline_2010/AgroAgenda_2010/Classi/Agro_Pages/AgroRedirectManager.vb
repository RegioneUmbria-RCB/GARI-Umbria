Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider
Imports AgronicaCoreGestioneRichieste

Public Class AgroRedirectManager

    Private LinguaCorrente As Lingua
    Private objParametri_Server As AgronicaCoreParametri
    Private objSession As System.Web.SessionState.HttpSessionState

    ''' <summary>
    ''' Session("LinguaCorrente")
    ''' </summary>
    ''' <param name="LinguaCorrente"></param>
    Public Sub New(LinguaCorrente As Lingua, objParametri_Server As AgronicaCoreParametri, ByRef objSession As System.Web.SessionState.HttpSessionState)
        Me.LinguaCorrente = LinguaCorrente
        Me.objParametri_Server = objParametri_Server
        Me.objSession = objSession
    End Sub


    ''' <summary>
    '''  Genera il link per il sito e la pagina richieste
    ''' </summary>
    ''' <param name="piva"></param>
    ''' <param name="Qs_SitoRichiesto"></param>
    ''' <param name="Qs_PaginaSitoRichiesta"></param>
    ''' <param name="Qs_Destinazione"></param>
    ''' <param name="objSession"></param>
    ''' <param name="strOpenRedirect"></param>
    ''' <param name="strOpenScript"></param>
    ''' <param name="dt_selected">se diverso da null chiamare Button4_Click(Nothing, Nothing)</param>
    Public Sub aprisitoversione2013(
            ByVal piva As String,
            Qs_SitoRichiesto As Enum_SiteRedirector,
            Qs_PaginaSitoRichiesta As Integer,
            Qs_Destinazione As String,
            ByRef strOpenRedirect As String,
            ByRef strOpenScript As String,
            ByRef dt_selected As DataTable
    )


        '-----------------------------------------------------------------------------------------
        '--------------------------NUOVA GESTIONE---------------------------------------------------
        '-----------------------------------------------------------------------------------------

        Select Case Qs_SitoRichiesto

            Case Enum_SiteRedirector.Sito_GiasOnline
                Dim ParametriGiasOnline As New AgronicaCoreGestioneRichieste.ParametriGiasOnline
                ParametriGiasOnline.Piva = piva
                ParametriGiasOnline.PaginaRichiesta = Qs_PaginaSitoRichiesta
                Dim strOpen As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoOnline_PassandoDirettamente_ParametriGiasOnline(
                                                Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                                ParametriGiasOnline)
                'ScriptManager.RegisterStartupScript(UpdatePanel_Menu, UpdatePanel_Menu.GetType, _
                '                                  "jQuery_{0}", strOpen, False)
                strOpenRedirect = strOpen
                Exit Sub

            Case Enum_SiteRedirector.Sito_AgronicaAgenda_2010

                Select Case Qs_Destinazione

                    'GRILLI - Duplica Operazione
                    Case "../Operazioni/DuplicaOperazione.aspx"

                        'Emulo la selezione (check) della Piva                        
                        dt_selected = creaDTimpreseSelezionate()
                        dt_selected.Rows.Add(dt_selected.NewRow)
                        dt_selected.Rows(0).Item("Piva") = piva


                        'Emulo la pressione della freccia per aggiungere le imprese
                        'ImgBtn_Inserisci_Aziende_Selezionate_Click(Nothing, Nothing)

                        'TODO out of function
                        ''Emulo la pressione del tasto prosegui
                        'Button4_Click(Nothing, Nothing)


                    Case Else

                        'per l'agenda rimango nel sito
                        Dim UrlTarget As String

                        'Recupero il link alla destinazione
                        Dim Destinazione As String = Qs_Destinazione

                        Try
                            Dim paginaDest As enum_PagineAgenda_2010 = CInt(Destinazione)
                            Dim objparamentriAg As New AgronicaCoreGestioneRichieste.ParametriAgenda_2010
                            objparamentriAg.Leggi()
                            objparamentriAg.Piva = piva
                            objparamentriAg.PaginaRichiesta = paginaDest

                            UrlTarget = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriAgenda_2010(
                                                     Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objparamentriAg)
                        Catch ex As Exception
                            'Preparo la querystring
                            If InStr(Destinazione, "?") > 0 Then

                                UrlTarget = Destinazione & "&p=" & Stringa_Codifica(piva, AgroKey_EncoderDecoder, Nothing)
                            Else
                                UrlTarget = Destinazione & "?p=" & Stringa_Codifica(piva, AgroKey_EncoderDecoder, Nothing)
                            End If

                        End Try

                        UrlTarget &= "&ln=" & LinguaCorrente.CodiceISO & "|" & objParametri_Server.Lingua_Cod
                        'Salto alla pagina
                        'Response.Redirect(UrlTarget)
                        strOpenRedirect = UrlTarget
                End Select

                Exit Sub
                '------------------------------------------------------------

            Case Enum_SiteRedirector.Sito_AgronicaAnalisi_2010
                Dim strOpen As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoAnalisi_2010_PassandoDirettamenteIParametri(
                            Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                            Qs_PaginaSitoRichiesta,
                            enum_PagineAgenda_2010.Pagina_FiltrinoImprese,
                            piva,
                            enum_PagineAnalisi_2010.Pagina_Analisi)

                'ScriptManager.RegisterStartupScript(UpdatePanel_Menu, UpdatePanel_Menu.GetType,
                '                                  "jQuery_{0}", strOpen, False)
                strOpenRedirect = strOpen
                Exit Sub
                '------------------------------------------------------------

            Case Enum_SiteRedirector.Sito_AgronicaPianiCampionamento
                Dim strOpen As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoPianiCampionamento_PassandoDirettamenteIParametri(
                              Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                              Qs_PaginaSitoRichiesta,
                              enum_PagineAgenda_2010.Pagina_FiltrinoImprese,
                              Nothing, 0)

                'ScriptManager.RegisterStartupScript(UpdatePanel_Menu, UpdatePanel_Menu.GetType,
                '                                "jQuery_{0}", strOpen, False)
                strOpenRedirect = strOpen
                Exit Sub
                '------------------------------------------------------------

            Case Enum_SiteRedirector.Sito_AgronicaPianiSemina
                'NON HA PARAMETRI PECIFICI PER ORA
                Dim strOpen As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoPianiSemina_PassandoDirettamenteIParametri(
                                Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                Qs_PaginaSitoRichiesta,
                                enum_PagineAgenda_2010.Pagina_FiltrinoImprese)

                'ScriptManager.RegisterStartupScript(UpdatePanel_Menu, UpdatePanel_Menu.GetType,
                '                                "jQuery_{0}", strOpen, False)

                strOpenRedirect = strOpen
                Exit Sub
                '------------------------------------------------------------

            Case Enum_SiteRedirector.Sito_AgronicaProfilazione
                Dim strOpen As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoProfilazione_PassandoDirettamenteIParametri(
                                Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                Qs_Destinazione,
                                enum_PagineAgenda_2010.Pagina_FiltrinoImprese,
                                piva)

                strOpenRedirect = strOpen
                Exit Sub
                '------------------------------------------------------------

            Case Enum_SiteRedirector.Sito_AgronicaSincronizzatore
                Dim strOpen As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoSincronizzatore_PassandoDirettamenteIParametri(
                            Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                            Qs_PaginaSitoRichiesta,
                            enum_PagineAgenda_2010.Pagina_FiltrinoImprese,
                            piva,
                            AgronicaCoreDataProvider.Conversioni.IdCodCliente_fromCodiceGIAS(CStr(objSession("ASG_ProgressivoGIAS"))))

                'ScriptManager.RegisterStartupScript(UpdatePanel_Menu, UpdatePanel_Menu.GetType,
                '                                 "jQuery_{0}", strOpen, False)

                strOpenRedirect = strOpen
                Exit Sub
                '------------------------------------------------------------

            Case Enum_SiteRedirector.Sito_AgronicaStampe, Enum_SiteRedirector.Sito_AgronicaStampe_2010, Enum_SiteRedirector.Sito_AgronicaStampe_NewMode
                Dim ParametriAgronicaStampe As AgronicaCoreGestioneRichieste.ParametriAgronicaStampe =
                AgronicaCoreGestioneRichieste.RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                        Qs_PaginaSitoRichiesta,
                        piva,
                        objSession,
                        objParametri_Server,
                        "",
                        0,
                        0,
                        0, 0, 0)

                Dim strOpen As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                    Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                    ParametriAgronicaStampe)

                'ScriptManager.RegisterStartupScript(UpdatePanel_Menu, UpdatePanel_Menu.GetType,
                '                                "jQuery_{0}", strOpen, False)
                strOpenScript = strOpen
                Exit Sub
                '------------------------------------------------------------

            Case Enum_SiteRedirector.Sito_GiasOnline_2010
                Dim strOpen As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoOnline2010_PassandoDirettamenteIParametri(
                                 Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                Qs_PaginaSitoRichiesta,
                                enum_PagineAgenda_2010.Pagina_FiltrinoImprese,
                                piva, "", "", 0, "")

                'ScriptManager.RegisterStartupScript(UpdatePanel_Menu, UpdatePanel_Menu.GetType,
                '                                 "jQuery_{0}", strOpen, False)
                strOpenRedirect = strOpen
                Exit Sub
                '------------------------------------------------------------

            Case Enum_SiteRedirector.Sito_PianoConcimazione
                'uniformare alle altre chiamate
                Dim objAnalisiCosti2010 As New AgronicaCoreGestioneRichieste.ParametriAnalisiCosti_2010
                objAnalisiCosti2010.PaginaRichiesta = enum_CodificaPagPianoConcimazione.Menu
                objAnalisiCosti2010.Piva = piva
                'viene inizializzato tramite i valori presenti nel webconfig


                Dim link As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoPianoConcimazione_PassandoDirettamente_ParametriAnalisiCosti_2010(
                    Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objAnalisiCosti2010)

                strOpenRedirect = link

            Case Enum_SiteRedirector.Sito_PianoConcimazione_2017

                Dim objConcimazione As New AgronicaCoreGestioneRichieste.ParametriConcimazione_2017
                'LEGGO IN MODO DA SFRUTTARE TIPO E OPERAZIONE CHE ERANO GIà SALVATI NEI PARAMETRI
                objConcimazione.Leggi()
                objConcimazione.Pagina_Richiesta = Qs_PaginaSitoRichiesta
                objConcimazione.Piva = piva
                'objConcimazione.Tipo_Concimazione QUELLI GIà SALVATI
                'objConcimazione.Tipo_Operazione QUELLI GIà SALVATI
                objConcimazione.SitoOrigine = Enum_SiteRedirector.Sito_PianoConcimazione_2017

                Dim sitoOrigine As Enum_SiteRedirector = If(IsNumeric(objSession("Sito_Origine")), objSession("Sito_Origine"), Enum_SiteRedirector.Sito_AgronicaAgenda_2010)

                Dim link As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoPianoConcimazione_PassandoDirettamente_ParametriConcimazione_2017(
                    sitoOrigine, objConcimazione)

                strOpenRedirect = link

                '------------------------------------------------------------
                Exit Sub
            Case Enum_SiteRedirector.GiasNG
                Dim objGiasNG As New AgronicaCoreGestioneRichieste.GiasNG_Redirect
                Dim objParametriAgendaNG As New Parametri_ObjParametriAgenda_NG
                objParametriAgendaNG.Piva = piva
                objParametriAgendaNG.Pagina_Richiesta = Qs_Destinazione
                objParametriAgendaNG.Salva()
                Dim link As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoGiasNG_PassandoDirettamente_ParametriAgenda_NG(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objParametriAgendaNG)
                strOpenRedirect = link
                'Case Enum_SiteRedirector.Sito_AgronicaMeteo


                'Case Enum_SiteRedirector.Sito_AgronicaPlanning
                'Case Enum_SiteRedirector.Sito_AgronicaPUA
                'Case Enum_SiteRedirector.Sito_AgronicaSicurezzaLavoro
                ' Case Enum_SiteRedirector.Sito_AgronicaView
                'Case Enum_SiteRedirector.Sito_AgronicaAnalisi
                'Case Enum_SiteRedirector.Sito_AgronicaAudit
                'Case Enum_SiteRedirector.Sito_AgronicaBio
                'Case Enum_SiteRedirector.Sito_AgronicaCheckCOOP
                'Case Enum_SiteRedirector.Sito_AgronicaGlobalGAP
                'Case Enum_SiteRedirector.Sito_AgronicaManutenzione

        End Select

        '------------------------------------------------------------
        '------------------------------------------------------------
        '------------------------------------------------------------
        '------------------------------------------------------------

    End Sub


    Public Sub RedirectStessoSito(ByVal Piva As String, Qs_Destinazione As String, ByRef strOpenRedirect As String)
        Dim UrlTarget As String

        'Recupero il link alla destinazione
        Dim Destinazione As String = Qs_Destinazione

        'Preparo la querystring
        If InStr(Destinazione, "?") > 0 Then
            UrlTarget = Destinazione & "&p=" & Stringa_Codifica(Piva, AgroKey_EncoderDecoder, Nothing)
        Else
            UrlTarget = Destinazione & "?p=" & Stringa_Codifica(Piva, AgroKey_EncoderDecoder, Nothing)
        End If

        'Salto alla pagina
        UrlTarget &= "&ln=" & LinguaCorrente.CodiceISO & "|" & objParametri_Server.Lingua_Cod

        strOpenRedirect = UrlTarget

    End Sub


    Public Shared Function creaDTimpreseSelezionate() As DataTable
        Dim dt_selected As New DataTable
        dt_selected.Columns.Add(New DataColumn("piva", GetType(String)))
        dt_selected.Columns.Add(New DataColumn("Rag_Soc", GetType(String)))
        dt_selected.Columns.Add(New DataColumn("provincia", GetType(String)))
        dt_selected.Columns.Add(New DataColumn("TipoImpresaGerarchia", GetType(Integer)))
        Return dt_selected
    End Function

End Class
