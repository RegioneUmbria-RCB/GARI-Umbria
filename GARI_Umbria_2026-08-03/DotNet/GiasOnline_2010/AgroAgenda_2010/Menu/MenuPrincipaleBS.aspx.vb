Imports System.Web
Imports System.Web.Services

Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreUtility
Imports AgronicaCoreUtility.CaricaListControl

Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreVarieBIZ

Public Class MenuPrincipaleBS
    Inherits System.Web.UI.Page

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri

    Public Username As String
    Public Password As String
    Public DPI_1 As String
    Public DPI_2 As String

    Public FITO_1 As String
    Public FITO_2 As String

    Public CAP_1 As String
    Public CAP_2 As String


    Public METEO_1 As String
    Public METEO_2 As String

    Public TimeOut As String


    '#########################################################################################
    <Script.Services.ScriptMethod()> _
    <WebMethod(EnableSession:=True)> _
    Public Shared Function Carica_Preferiti() As String

        Dim dt As DataTable
        Dim objUtentiImpostazioniR As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        dt = objUtentiImpostazioniR.Leggi(enum_Impostazioni_Utenti.UTENTE_LINK_PREFERITE_MENU_ONLINE, 1, _
                                 AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                 "", "", HttpContext.Current.Session("ASG_objParametri_Utenti"))

        Dim preferiti As String
        Dim pref_single As String()
        Dim j As Integer = 1
        Dim rval As String = ""
        Dim link As String

        If dt.Rows.Count > 0 Then
            preferiti = dt.Rows(0).Item("Impostazione_Valore_1")

            pref_single = preferiti.Split(New Char() {"|"c})

            For Each sing In pref_single

                Select Case sing

                    Case enum_Security_Attivita.Gest_AnagraficaAzienda
                        rval &= "<a href=""" & link & """><div class=""preferito_item""><img src=""../AB_Immagini/icone32/impresa.ico"" />Anagrafica e Catasto Aziendale</div></a>"
                    Case enum_Security_Attivita.Agenda_AccessoMenu
                        rval &= "<a href=""" & link & """><div class=""preferito_item""><img src=""../AB_Immagini/icone32/impresa.ico"" />Agenda Operazioni Colutrali, Contabili e Zootecniche</div></a>"
                    Case enum_Security_Attivita.Gest_Magazzino
                        rval &= "<a href=""" & link & """><div class=""preferito_item""><img src=""../AB_Immagini/icone32/impresa.ico"" />Gestione Magazzini</div></a>"
                    Case enum_Security_Attivita.Anagrafica_Contatto
                        rval &= "<a href=""" & link & """><div class=""preferito_item""><img src=""../AB_Immagini/icone32/impresa.ico"" />Gestione Contatti</div></a>"
                    Case enum_Security_Attivita.Gest_Stalle
                        rval &= "<a href=""" & link & """><div class=""preferito_item""><img src=""../AB_Immagini/icone32/impresa.ico"" />Gestione Consistenze Zootecniche</div></a>"
                    Case enum_Security_Attivita.SupportoDecisioni_AccessoMenu
                        rval &= "<a href=""" & link & """><div class=""preferito_item""><img src=""../AB_Immagini/icone32/impresa.ico"" />DSS - Supporti Decisionali</div></a>"
                    Case enum_Security_Attivita.Gest_CartografiaAziendale
                        rval &= "<a href=""" & link & """><div class=""preferito_item""><img src=""../AB_Immagini/icone32/impresa.ico"" />Cartografia Aziendale</div></a>"
                    Case enum_Security_Attivita.Gest_AnalisiCosti
                        rval &= "<a href=""" & link & """><div class=""preferito_item""><img src=""../AB_Immagini/icone32/impresa.ico"" />Analisi Produzioni/Costi/Ricavi</div></a>"
                    Case enum_Security_Attivita.Gest_UtentiImpostazioni
                        rval &= "<a href=""" & link & """><div class=""preferito_item""><img src=""../AB_Immagini/icone32/impresa.ico"" />Gestione Utenti & Permessi</div></a>"
                    Case enum_Security_Attivita.ManutenzioneArchivi_AccessoMenu
                        rval &= "<a href=""" & link & """><div class=""preferito_item""><img src=""../AB_Immagini/icone32/impresa.ico"" />Manutenzione & Utility</div></a>"

                    Case enum_Security_Attivita.Gest_Contabilita
                        rval &= "<a href=""" & link & """><div class=""preferito_item""><img src=""../AB_Immagini/icone32/impresa.ico"" />Contabilità (CO.GE)</div></a>"
                    Case enum_Security_Attivita.Gest_CartellaAziendale_AccessoMenu
                        rval &= "<a href=""" & link & """><div class=""preferito_item""><img src=""../AB_Immagini/icone32/impresa.ico"" />Agricoltura Biologica</div></a>"
                    Case enum_Security_Attivita.Gest_AvvisiMessaggi
                        rval &= "<a href=""" & link & """><div class=""preferito_item""><img src=""../AB_Immagini/icone32/impresa.ico"" />Messaggistica</div></a>"
                    Case enum_Security_Attivita.Gest_Stampe
                        rval &= "<a href=""" & link & """><div class=""preferito_item""><img src=""../AB_Immagini/icone32/impresa.ico"" />Elaborazione/Stampe statistiche</div></a>"


                End Select

            Next

        End If

        Return rval

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()> _
    <WebMethod(EnableSession:=True)> _
    Public Shared Function Salva_Preferiti(ByVal pref1 As String, ByVal pref2 As String, ByVal pref3 As String, ByVal pref4 As String, ByVal pref5 As String)

        'aggiungo i pulsanti al tab preferiti
        Dim strsalva As String = ""
        strsalva = strsalva + pref1
        strsalva = strsalva + "|" + pref2
        strsalva = strsalva + "|" + pref3
        strsalva = strsalva + "|" + pref4
        strsalva = strsalva + "|" + pref5

        Dim objUtentiImpostazioniW As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W
        objUtentiImpostazioniW.Cancella(enum_Impostazioni_Utenti.UTENTE_LINK_PREFERITE_MENU_ONLINE, _
                                 "", HttpContext.Current.Session("ASG_objParametri_Utenti"))
        objUtentiImpostazioniW.Scrivi( _
            enum_Impostazioni_Utenti.UTENTE_LINK_PREFERITE_MENU_ONLINE, _
            strsalva, _
            "", "", "", AGRODATAINIZIO, AGRODATAFINE, HttpContext.Current.Session("ASG_objParametri_Utenti"))


    End Function






    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        ' setto il titolo della pagina
        Master.Lbl_Titolo.Text = "MENU' PRINCIPALE"

        ' Setto la visibilità dei bottoni in Master
        Master.flag_pag_MenuPrincipale = True

        objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")
        objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim vDal As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim Sostituzioni As String
        Sostituzioni = vDal.Leggi_Valore(6, "Nomenclatura_Ricette", "", "", objParametri_Server)

        AgronicaControlli_2010.UI_ControlsHelper.RinominaControlli(Sostituzioni, Me, Nothing)

        ''genero errore 500
        ''objParametri_Utenti.StringaConnessione = ""

        ''controllo se sono loggato
        ''If Session("ASG_Utente_Username") = "" Then
        ''    Response.Redirect("../index.aspx")
        ''End If
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("../Custom500.aspx")
        End If

        'Dim str As String
        'str = "$(document).ready(function () {"
        'str = str & "   $('body').trigger('create'); "

        'str = str & "   $('#FiltraAzienda').click(function () { "
        'str = str & "       document.location='filtra_azienda.aspx?pagina=menu';"
        'str = str & "   });"

        'str = str & "   $('#impostadata').click(function () { "
        'str = str & "       document.location='imposta_data.aspx?pagina=menu';"
        'str = str & "   });"

        'If Request.QueryString("err") = "azienda" Then
        '    str = str & "   alert('" & Resources.GiasOnLine_2010.SelezionaAzienda & " ');"
        'End If

        'str = str & "   $('#cartografia').button('disable'); "

        'str = str & "});"
        'ScriptManager.RegisterClientScriptBlock(Update_Principale, Update_Principale.GetType(),
        '                               String.Format("jQuery_{0}", Update_Principale.ClientID), str, True)


        'CaricaScadenze()
        ''carico l'azienda
        If Not IsPostBack Then

            caricaConfigurazioneSiti()


            CaricaConfigurazione()


            'Dim objAz As New AgronicaCoreAnagrafeDAL.Imprese_Read
            'Dim DTAz As DataTable
            ''controllo se ho la piva selezionata
            'If Not IsNothing(Session("_Piva")) Then
            '    DTAz = objAz.Leggi(Session("_Piva"), AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            '    txt_AziendaSelezionata.Text = DTAz.Rows(0).Item("Rag_Soc")
            'Else
            '    'controllo se ho solo una azienda
            '    Dim objAppDDL As New DropDownList
            '    AgronicaCoreUtility.CaricaListControl.ImpreseConFiltroUtente(objAppDDL, False, "", "", _
            '                                                             "", " ORDER BY Rag_Soc asc", _
            '                                                                 HttpContext.Current.Session("ASG_objParametri_Server"), _
            '                                                                 HttpContext.Current.Session("ASG_objParametri_Utenti"))
            '    If objAppDDL.Items.Count = 1 Then
            '        txt_AziendaSelezionata.Text = objAppDDL.Items(0).Text
            '        Session("_Piva") = objAppDDL.Items(0).Value
            '    Else
            '        txt_AziendaSelezionata.Text = Resources.GiasOnLine_2010.RicercaLAziendaUtilizzandoLaLenteQuiADestr
            '    End If
            'End If

            'If Not IsNothing(Session("_Piva")) Then
            '    If IsDate(Session("_Data")) = False Then
            '        Session("_Data") = Date.Today
            '    End If
            '    If Session("_Piva") > "" AndAlso IsDate(Session("_Data")) = True Then
            '        Dim leggianchebloccatipalmare As Boolean = True
            '        AgronicaCoreUtility.CaricaListControl.TutteSpecieColtivate_3(ddl_SpecieVegetale, _
            '                                                                              True, "", "", _
            '                                                                              Session("_Piva"), 0, CDate(Session("_Data")), False, _
            '                                                                               "", "", HttpContext.Current.Session("ASG_objParametri_Server"), leggianchebloccatipalmare)

            '        'se ho una sola sepecie vegetale 
            '        If ddl_SpecieVegetale.Items.Count = 2 Then
            '            ddl_SpecieVegetale.SelectedIndex = 1
            '            ddl_SpecieVegetale_SelectedIndexChanged(Me, Nothing)
            '        Else
            '            If IsNumeric(Session("_Veg_Cod")) Then
            '                Me.ddl_SpecieVegetale.SelectedIndex = _
            '                    ddl_SpecieVegetale.Items.IndexOf(ddl_SpecieVegetale.Items.FindByValue(Session("_Veg_Cod")))
            '                ddl_SpecieVegetale_SelectedIndexChanged(Me, Nothing)
            '            End If
            '        End If
            '    End If
            'End If
        End If

    End Sub


    Private Sub caricaConfigurazioneSiti()
        Username = CStr(Session("ASG_Utente_Username_Crypt").ToString)
        Password = CStr(Session("ASG_Utente_Password_Crypt").ToString)


        Dim objagrowebconfig As New AgronicaCoreGestioneRichieste.AgroWebConfig

        DPI_1 = objagrowebconfig.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari
        Session("WS_DPI") = DPI_1
        DPI_2 = objagrowebconfig.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari_2

        FITO_1 = objagrowebconfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci
        Session("WS_FITO") = FITO_1
        FITO_2 = objagrowebconfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci_2


        METEO_1 = objagrowebconfig.GiasOnline_WS_Meteo_Meteo
        Session("WS_METEO") = METEO_1
        METEO_2 = objagrowebconfig.GiasOnline_WS_Meteo_Meteo_2


        CAP_1 = objagrowebconfig.GiasOnline_WS_CapitolatoCliente_AgroWS_CapitolatoCliente
        Session("WS_CAPITOLATO") = CAP_1
        CAP_2 = objagrowebconfig.GiasOnline_WS_CapitolatoCliente_AgroWS_CapitolatoCliente_2


        TimeOut = objagrowebconfig.WS_Timeout

    End Sub



    Private Sub CaricaConfigurazione()

        'GestioneRichieste.CaricaConfigurazione()

        ' Carico le voci di menu che sono abilitate per 'utente corrente (per select preferiti)
        Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim ID_Servizio As Integer = System.Web.HttpContext.Current.Session("ASG_IdServizio")

        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim AbilitazioneVociMenuPerUtente As New DataTable
        Dim val As String

        ' Estrapolo tutti i permessi
        AbilitazioneVociMenuPerUtente = ObjUtenti.Controlla_Permessi_Utente_Tutti( _
                               HttpContext.Current.Session("ASG_Utente_Username"), _
                               HttpContext.Current.Session("ASG_IdServizio"), _
                               0, _
                               enum_Security_Operazione.Modifica, _
                               Date.Now, "", objParametri_Utenti)

        ''''''''''''''''''''''''''''''''''''''''''


        Dim conf As Configurazione = HttpContext.Current.Session("Configurazione")

        CaricaDDLPreferiti(ddl_preferito1, AbilitazioneVociMenuPerUtente)
        CaricaDDLPreferiti(ddl_preferito2, AbilitazioneVociMenuPerUtente)
        CaricaDDLPreferiti(ddl_preferito3, AbilitazioneVociMenuPerUtente)
        CaricaDDLPreferiti(ddl_preferito4, AbilitazioneVociMenuPerUtente)
        CaricaDDLPreferiti(ddl_preferito5, AbilitazioneVociMenuPerUtente)

        ' Carico il salvataggio preferiti sulle select
        Dim dt As DataTable
        Dim objUtentiImpostazioniR As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        dt = objUtentiImpostazioniR.Leggi(enum_Impostazioni_Utenti.UTENTE_LINK_PREFERITE_MENU_ONLINE, 1, _
                                 AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                 "", "", objParametri_Utenti)

        Dim preferiti As String
        Dim pref_single As String()
        Dim j As Integer = 1

        If dt.Rows.Count > 0 Then

            preferiti = dt.Rows(0).Item("Impostazione_Valore_1")

            pref_single = preferiti.Split(New Char() {"|"c})

            For Each sing In pref_single

                If sing <> "" Then

                    Select Case j
                        Case 1
                            ddl_preferito1.SelectedValue = sing
                        Case 2
                            ddl_preferito2.SelectedValue = sing
                        Case 3
                            ddl_preferito3.SelectedValue = sing
                        Case 4
                            ddl_preferito4.SelectedValue = sing
                        Case 5
                            ddl_preferito5.SelectedValue = sing

                    End Select

                End If

                j = j + 1

            Next

        End If
        '''''''''''''''''''''''''''''''''''''''''''''''

        'AgronicaCoreUtility.CaricaListControl.OrganismiReferenti(ddl_organismoreferenteDefault, False, "", "", HttpContext.Current.Session("_Piva"), "", objParametri_Server)



        'ddl_organismoreferenteDefault.SelectedValue = conf.SUPERUSER_Smart_NuovoImpianto_DefaultOrganismoReferente
        'chk_Blocca_SbloccaImpianti_Smart.Checked = conf.SUPERUSER_Blocca_Impianti_Smart
        'chk_NuovoImpianto_OrganismoReferente.Checked = conf.SUPERUSER_Smart_NuovoImpianto_OrganismoReferente
        'chk_NuovoImpianto_Finalita.Checked = conf.SUPERUSER_Smart_NuovoImpianto_Finalita
        'chk_NuovaImpresa_AppartieneA.Checked = conf.SUPERUSER_Smart_NuovaImpresa_ImpresaPadre

        'MostraNascondi()
    End Sub


    Private Sub CaricaDDLPreferiti(ByRef ddl As DropDownList, ByRef AbilitazioneVociMenuPerUtente As DataTable)
        Dim conf As Configurazione
        'conf = Session("Configurazione_SMART")
        conf = HttpContext.Current.Session("Configurazione")

        ddl.Items.Clear()

        ddl.Items.Add(New ListItem("selezionare preferito", ""))

        'ddl.Items.Add(New ListItem("AUDIT", enum_Security_Attivita.Gest_CartellaAziendale_AccessoMenu))

        Try
            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

            Dim UtenteAbilitato_checklist_scheda_tti As Boolean = objPermessi.Controlla_Permessi_Utente( _
                                       objParametri_Utenti.UtenteUsername, _
                                       5, _
                                       enum_Security_Attivita.Gest_CartellaAziendale_Check_SchedaTecnicaTTI, _
                                       enum_Security_Operazione.Modifica, _
                                       Date.Now, _
                                       "", _
                                       objParametri_Utenti)

            If UtenteAbilitato_checklist_scheda_tti Then
                ddl.Items.Add(New ListItem("Scheda Tecnica TTI", enum_Security_Attivita.Gest_CartellaAziendale_Check_SchedaTecnicaTTI))
            End If


        Catch ex As Exception

        End Try

        ' Ciclo su tutte le voci di menu abilitate per l'utente
        For i = 0 To AbilitazioneVociMenuPerUtente.Rows.Count - 1

            Select Case AbilitazioneVociMenuPerUtente.Rows(i).Item("Id_Attivita")

                Case enum_Security_Attivita.Gest_AnagraficaAzienda
                    ddl.Items.Add(New ListItem("Anagrafica e Catasto Aziendale", enum_Security_Attivita.Gest_AnagraficaAzienda))
                Case enum_Security_Attivita.Agenda_AccessoMenu
                    ddl.Items.Add(New ListItem("Agenda Operazioni Colutrali, Contabili e Zootecniche", enum_Security_Attivita.Agenda_AccessoMenu))
                Case enum_Security_Attivita.Gest_Magazzino
                    ddl.Items.Add(New ListItem("Gestione Magazzini", enum_Security_Attivita.Gest_Magazzino))
                Case enum_Security_Attivita.Anagrafica_Contatto
                    ddl.Items.Add(New ListItem("Gestione Contatti", enum_Security_Attivita.Anagrafica_Contatto))
                Case enum_Security_Attivita.Gest_Stalle
                    ddl.Items.Add(New ListItem("Gestione Consistenze Zootecniche", enum_Security_Attivita.Gest_Stalle))
                Case enum_Security_Attivita.Gest_Contabilita
                    ddl.Items.Add(New ListItem("Contabilità (CO.GE)", enum_Security_Attivita.Gest_Contabilita))
                Case enum_Security_Attivita.SupportoDecisioni_AccessoMenu
                    ddl.Items.Add(New ListItem("DSS - Supporti Decisionali", enum_Security_Attivita.SupportoDecisioni_AccessoMenu))
                Case enum_Security_Attivita.Gest_CartografiaAziendale
                    ddl.Items.Add(New ListItem("Cartografia Aziendale", enum_Security_Attivita.Gest_CartografiaAziendale))
                Case enum_Security_Attivita.Gest_AnalisiCosti
                    ddl.Items.Add(New ListItem("Analisi Produzioni/Costi/Ricavi", enum_Security_Attivita.Gest_AnalisiCosti))
                Case enum_Security_Attivita.Gest_CartellaAziendale_AccessoMenu
                    ddl.Items.Add(New ListItem("Agricoltura Biologica", enum_Security_Attivita.Gest_CartellaAziendale_AccessoMenu))
                Case enum_Security_Attivita.Gest_UtentiImpostazioni
                    ddl.Items.Add(New ListItem("Gestione Utenti & Permessi", enum_Security_Attivita.Gest_UtentiImpostazioni))
                Case enum_Security_Attivita.ManutenzioneArchivi_AccessoMenu
                    ddl.Items.Add(New ListItem("Manutenzione & Utility", enum_Security_Attivita.ManutenzioneArchivi_AccessoMenu))
                Case enum_Security_Attivita.Gest_AvvisiMessaggi
                    ddl.Items.Add(New ListItem("Messaggistica", enum_Security_Attivita.Gest_AvvisiMessaggi))
                Case enum_Security_Attivita.Gest_Stampe
                    ddl.Items.Add(New ListItem("Elaborazione/Stampe statistiche", enum_Security_Attivita.Gest_Stampe))


            End Select

        Next

       
    End Sub




End Class

Public Class Configurazione
    Public PannelloDiControllo As Boolean
    Public AnalisiDatiCura As Boolean
    Public AnalisiDatiSchedeRilievi As Boolean
    Public CheckCOOP As Boolean
    Public GlobalGap As Boolean
    Public Condizionalita As Boolean
    Public Scheda_Tecnica_TTI As Boolean
    Public Scheda_Controlli_TTI As Boolean
    Public RegistrazioneSmart As Boolean
    Public AgendaOperazioniColturali As Boolean
    Public GiasProfitosan As Boolean
    Public GiS As Boolean
    Public Anagrafica As Boolean
    Public CaricoMagazzino As Boolean
    Public RilievoAttivita As Boolean
    Public Budget As Boolean
    Public Ricette As Boolean
    Public Rilevamento_Smart As Boolean
    Public RilevamentoGis_campi As Boolean
    Public Stampe_Inglese As Boolean
    Public Stampe_Italiano As Boolean
    Public Profilatore As Boolean
    Public Permessi_Utente As Boolean
    Public Scadenziario As Boolean
    Public Rintracciabilità As Boolean
    Public Sementieri As Boolean
    Public Impostazioni_Utente As Boolean
    Public LinkStandard As Boolean

    Public NuovaAzienda As Boolean
    Public NuovoAppezzamento As Boolean
    Public NuovoContatto As Boolean
    Public NuovoCentro As Boolean

    Public SUPERUSER_Blocca_Impianti_Smart As Boolean
    Public SUPERUSER_Smart_NuovoImpianto_OrganismoReferente As Boolean
    Public SUPERUSER_Smart_NuovoImpianto_Finalita As Boolean
    Public SUPERUSER_Smart_NuovaImpresa_ImpresaPadre As Boolean


    Public SUPERUSER_Smart_NuovoImpianto_DefaultOrganismoReferente As String



End Class