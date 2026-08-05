Imports System.Web
Imports System.Web.Services

Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreUtility
Imports AgronicaCoreGestioneRichieste

Imports AgronicaControlli_2010

Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreModelsSTD.attivita.Attivita

Public Class Trattamenti_PostRaccolta
    Inherits System.Web.UI.Page

    Public Master_Operazione As Operazione
    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Dim objParametriAgenda As ParametriAgenda

    Dim Id_Agenda_Old As Integer = 0
    Dim CopiaQuantita As Boolean = True

    Private Sub Trattamenti_Disposed()
        'Master
        CType(Page.Master, Operazione).Operazioni_Dispose()

        'Page
        session.Remove("UtenteAbilitato_Lettura")
        session.Remove("UtenteAbilitato_Modifica")
        session.Remove("DtAvv")
        session.Remove("DtAvvGru")
        session.Remove("vs_dtDosi")
        session.Remove("DoseMax")
        session.Remove("Udm_Cod_Max")
        session.Remove("DoseEtichetta")
        session.Remove("Dpi_Cod")
        session.Remove("modulo")
        session.Remove("Disciplinare_Des")
        session.Remove("Id_Rcdpi")

        session.Remove("D_HA_Min")
        session.Remove("Acqua_Min")
        session.Remove("Acqua_Max")
        session.Remove("D_HA_Max")
        session.Remove("D_HL_Max")
        session.Remove("D_HL_Min")
        session.Remove("D_HA_Min")

        session.Remove("Udm_Radice_HA")
        session.Remove("Udm_Radice_HL")

        Session.Remove("Udm_Cod_HA")
        session.Remove("Udm_Cod_HL")

    End Sub



#Region "Inizializzazione"



    Private Sub Trattamenti_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        Master_Operazione = CType(Page.Master, Operazione)

        AddHandler CType(Page.Master.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto
        AddHandler CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_Salva"), ImageButton).Click, AddressOf Me.SalvaTutto

        AddHandler CType(Ricerca.FindControlIterative(Page.Master, "BTN_ChangeData"), Button).Click, AddressOf Me.Aggiorna_Centro_Specie

        AddHandler CType(Ricerca.FindControlIterative(Page.Master, "BTN_ComboCentroAziendale"), Button).Click, AddressOf Me.Aggiorna_Centro_Specie

        AddHandler CType(Ricerca.FindControlIterative(Page.Master, "BTN_ComboSpecie"), Button).Click, AddressOf Me.Aggiorna_Centro_Specie

        AddHandler CType(Ricerca.FindControlIterative(Page.Master, "BTN_Magazzini"), Button).Click, AddressOf Me.CaricaComboUnitadiMisura

        CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_CheckDPI"), ImageButton).Visible = False
        AddHandler CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_Disciplinare"), ImageButton).Click, AddressOf Me.BTN_Disciplinare

        AddHandler CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_Carico"), ImageButton).Click, AddressOf Me.BTN_CaricoMagazzino

        'AddHandler Master_Operazione.Property_Btn_Conferma_Ricetta.Click, AddressOf Me.Btn_Conferma_Ricetta

    End Sub



    ''' <summary>
    ''' Aggiunge Gli Script lato client
    ''' da ottimizzare ....
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InizializzaScriptClient()

        'script DPI
        ScriptManager.RegisterStartupScript(UpdatePanelDisciplinare, UpdatePanelDisciplinare.GetType(),
                                  String.Format("jQuery_{0}", ComboDisciplinari1.ClientID), ComboDisciplinari1.GetJS(), True)

        'script FILTRI AGGIUNTIVI
        ScriptManager.RegisterStartupScript(UpdatePanelFiltriAggiuntivi, UpdatePanelFiltriAggiuntivi.GetType(),
                                  String.Format("jQuery_{0}", ComboFiltriAggiuntiviAgenda1.ClientID), ComboFiltriAggiuntiviAgenda1.GetJS(), True)


        Dim script As New StringBuilder


        script.AppendLine("$(document).ready(function () { ")

        script.AppendLine("     AbilitaDisabilita_QtaTrattata();")

        script.AppendLine("     $('#chkSelezionaTuttiLavorati').click(function (){ ")
        script.AppendLine("         SelezionaDeselezionaLavorati();")
        script.AppendLine("     });")

        script.AppendLine("     $('.ChkSelezionaLavorato').click(function (){ ")
        script.AppendLine("         ChkSelezionaLavorato_Click($(this).find('input'),false);")
        script.AppendLine("     });")


        script.AppendLine("     RicalcolaQtaCoinvolta(); ")

        script.AppendLine("     $('.Qta_Giacenza').keyup(function (){")
        script.AppendLine("         Qta_Coinvolta_Keyup($(this));")
        script.AppendLine("     });")

        'script.AppendLine("     $('.SommaSuperficieTrattata').keyup(function () {")
        'script.AppendLine("         SommaSuperficieTrattata_Keyup(); ")
        'script.AppendLine("     });")

        script.AppendLine("     $('.AcquaHa').keyup(function () {")
        script.AppendLine("         AcquaHA_Keyup();")
        script.AppendLine("     });")

        script.AppendLine("     $('.AcquaTot').keyup( function () {")
        script.AppendLine("         AcquaTot_Keyup();")
        script.AppendLine("     });")

        'If SupTrattata = True Then
        'script.AppendLine("     RicalcolaSuperficieCoinvolta(); ")


        'script.AppendLine("     $('.Sup_Coinvolta').keyup(function (){")
        'script.AppendLine("         Sup_Coinvolta_Keyup($(this));")
        'script.AppendLine("     });")

        'script.AppendLine("     $('.SommaSuperficieTrattata').keyup(function () {")
        'script.AppendLine("         SommaSuperficieTrattata_Keyup(); ")
        'script.AppendLine("     });")

        'script.AppendLine("     $('.AcquaHa').keyup(function () {")
        'script.AppendLine("         AcquaHA_Keyup();")
        'script.AppendLine("     });")

        'script.AppendLine("     $('.AcquaTot').keyup( function () {")
        'script.AppendLine("         AcquaTot_Keyup();")
        'script.AppendLine("     });")
        'End If


        script.AppendLine("}); ")


        ScriptManager.RegisterStartupScript(UpdatePanelLavorati, UpdatePanelLavorati.GetType(),
                                         String.Format("jQuery_{0}", UpdatePanelLavorati.ClientID), script.ToString, True)








        Dim STR_UpdatePanelDose As New StringBuilder



        STR_UpdatePanelDose.AppendLine("$(document).ready(function () { ")

        STR_UpdatePanelDose.AppendLine("    Init(); ")


        STR_UpdatePanelDose.AppendLine("    $('#" & Cmb_FormulatoClassificazioni.ClientID & "').combobox();")

        STR_UpdatePanelDose.AppendLine("    $('.ChkSelezionaAvversita').click( function () {")
        STR_UpdatePanelDose.AppendLine("        SelezionaAutoSoglia($(this));")
        STR_UpdatePanelDose.AppendLine("    });")

        STR_UpdatePanelDose.AppendLine("    });")

        ScriptManager.RegisterStartupScript(Master_Operazione.Property_UpdatePanelPerScript, Master_Operazione.Property_UpdatePanelPerScript.GetType(),
                                      String.Format("jQuery_{0}", Master_Operazione.Property_UpdatePanelPerScript.ClientID),
                                                    STR_UpdatePanelDose.ToString, True)



        STR_UpdatePanelDose = New StringBuilder


        STR_UpdatePanelDose.AppendLine("$(document).ready(function () { ")


        STR_UpdatePanelDose.AppendLine("    $('#" & Cmb_FormulatoClassificazioni.ClientID & "').combobox();")

        STR_UpdatePanelDose.AppendLine("    $('#" & Txt_Dose_HA.ClientID & "').keyup( function () {")
        STR_UpdatePanelDose.AppendLine("        AggiornaDOSI();")
        STR_UpdatePanelDose.AppendLine("    });")

        STR_UpdatePanelDose.AppendLine("    $('#" & Txt_Dose_HL.ClientID & "').keyup(function () {")
        STR_UpdatePanelDose.AppendLine("        AggiornaDOSI();")
        STR_UpdatePanelDose.AppendLine("    });")

        STR_UpdatePanelDose.AppendLine("    $('#" & Txt_DoseTot_HA.ClientID & "').keyup(function () {")
        STR_UpdatePanelDose.AppendLine("        AggiornaDOSI();")
        STR_UpdatePanelDose.AppendLine("    });")

        STR_UpdatePanelDose.AppendLine("    $('#" & Txt_Acqua_Ha.ClientID & "').keyup(function () { ")
        STR_UpdatePanelDose.AppendLine("        AcquaHA_Keyup(); ")
        STR_UpdatePanelDose.AppendLine("    });")

        STR_UpdatePanelDose.AppendLine("    $('#" & Txt_Acqua_Tot.ClientID & "').keyup(function () { ")
        STR_UpdatePanelDose.AppendLine("        AcquaTot_Keyup(); ")
        STR_UpdatePanelDose.AppendLine("    });")





        STR_UpdatePanelDose.AppendLine("    $('#" & rbl_DoseHA.ClientID & "').click( function () {")
        STR_UpdatePanelDose.AppendLine("        Abilita_Disabilita_DOSI();")
        STR_UpdatePanelDose.AppendLine("    });")

        STR_UpdatePanelDose.AppendLine("    $('#" & rbl_DoseHL.ClientID & "').click(function () {")
        STR_UpdatePanelDose.AppendLine("        Abilita_Disabilita_DOSI();")
        STR_UpdatePanelDose.AppendLine("    });")

        STR_UpdatePanelDose.AppendLine("    $('#" & rbl_QtaDose.ClientID & "').click(function () {")
        STR_UpdatePanelDose.AppendLine("        Abilita_Disabilita_DOSI();")
        STR_UpdatePanelDose.AppendLine("    });")

        STR_UpdatePanelDose.AppendLine("    $('#" & rbl_QtaTot.ClientID & "').click( function () {")
        STR_UpdatePanelDose.AppendLine("        Abilita_Disabilita_DOSI();")
        STR_UpdatePanelDose.AppendLine("    }); ")


        STR_UpdatePanelDose.AppendLine("    $('#" & rblAcqua_HA.ClientID & "').click(function () { ")
        STR_UpdatePanelDose.AppendLine("        Abilita_Disabilita_ACQUA(); ")
        STR_UpdatePanelDose.AppendLine("    });")

        STR_UpdatePanelDose.AppendLine("    $('#" & rblAcqua_Tot.ClientID & "').click(function () { ")
        STR_UpdatePanelDose.AppendLine("        Abilita_Disabilita_ACQUA(); ")
        STR_UpdatePanelDose.AppendLine("    });")

        STR_UpdatePanelDose.AppendLine("        Abilita_Disabilita_ACQUA(); ")
        STR_UpdatePanelDose.AppendLine("        Abilita_Disabilita_ACQUA(); ")
        STR_UpdatePanelDose.AppendLine("        Abilita_Disabilita_DOSI();")
        STR_UpdatePanelDose.AppendLine("        Abilita_Disabilita_DOSI();")
        STR_UpdatePanelDose.AppendLine("        Abilita_Disabilita_DOSI();")
        STR_UpdatePanelDose.AppendLine("        Abilita_Disabilita_DOSI();")

        STR_UpdatePanelDose.AppendLine("        $('#" & Txt_Formulati.ClientID & "').keydown( function () {")
        STR_UpdatePanelDose.AppendLine("        Verifica_Tasto_Premuto();")
        STR_UpdatePanelDose.AppendLine("    });")

        STR_UpdatePanelDose.AppendLine(" });")


        ScriptManager.RegisterStartupScript(UpdatePanelFormulati, UpdatePanelFormulati.GetType(),
                                      String.Format("jQuery_{0}", UpdatePanelFormulati.ClientID), STR_UpdatePanelDose.ToString, True)


        Dim STR_Colore As New StringBuilder
        STR_Colore.AppendLine("$(document).ready(function () { ")
        STR_Colore.AppendLine("    MettiColori(); ")
        STR_Colore.AppendLine("    }); ")
        ScriptManager.RegisterStartupScript(UpdatePanelDisciplinare, UpdatePanelDisciplinare.GetType(),
                                      String.Format("jQuery_{0}", UpdatePanelDisciplinare.ClientID), STR_Colore.ToString, True)



        Dim STR_UpdatePanelDosea As New StringBuilder
        STR_UpdatePanelDosea.AppendLine("$(document).ready(function () { ")

        STR_UpdatePanelDosea.AppendLine("    $('.ChkSelezionaAvversita').click( function () {")
        STR_UpdatePanelDosea.AppendLine("        $('#" & EventoAggiornamentoAvversita.ClientID & "').click();")
        STR_UpdatePanelDosea.AppendLine("    });")

        STR_UpdatePanelDosea.AppendLine("    $('.ChkSelezionaGruppoAvversita').click( function () {")
        STR_UpdatePanelDosea.AppendLine("        $('#" & EventoAggiornamentoAvversita.ClientID & "').click();")
        STR_UpdatePanelDosea.AppendLine("    });")

        STR_UpdatePanelDosea.AppendLine("    });")


        ScriptManager.RegisterStartupScript(UpdatePanelAvversita, UpdatePanelAvversita.GetType(),
                                      String.Format("jQuery_{0}", UpdatePanelAvversita.ClientID), STR_UpdatePanelDosea.ToString, True)


    End Sub

#End Region

#Region "Bottoni Delegati"

    Private Sub Sblocca_x_Modifica(ByVal sender As Object, ByVal e As EventArgs) Handles LockModifica.Click
        CaricaGriglia_Avversita()
    End Sub




    Private Sub BTN_Disciplinare(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim objGiasOnline As New AgronicaCoreGestioneRichieste.ParametriGiasOnline
        objGiasOnline.Cul_Cod = objParametriAgenda.Cul_Cod
        objGiasOnline.DataSelezionata = objParametriAgenda.Data
        objGiasOnline.Id_Agenda = objParametriAgenda.Id_Agenda
        objGiasOnline.Lavorazione = objParametriAgenda.Lav_Cod
        objGiasOnline.PaginaRichiesta = enum_PagineGiasOnline.GestioneDisciplinari_Consultazione_Disciplinari

        objGiasOnline.Piva = objParametriAgenda.Piva
        objGiasOnline.Sa_Cod = objParametriAgenda.Sa_Cod
        Dim specie As Integer = 0
        If IsNumeric(objParametriAgenda.Veg_Cod.Split("/")(0)) AndAlso CInt(objParametriAgenda.Veg_Cod.Split("/")(0)) > 0 Then
            specie = CInt(objParametriAgenda.Veg_Cod.Split("/")(0))
        End If
        objGiasOnline.Veg_Cod = specie

        objGiasOnline.Xml_Generico.Length = 0

        Dim Array = Split(objParametriAgenda.Disciplinare, "/")
        objGiasOnline.DPI_Cod = Array(0)
        objGiasOnline.IdRcdpi = Array(1)

        'Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        ''viene caricato in automatico dal costruttore
        'Dim objVarSess As New AgronicaCoreGestioneRichieste.VariabiliSessione
        ''Scrittura XML
        'Dim objScrivi As New AgronicaCoreGestioneRichieste.ScriviXml
        'objScrivi.AgroWebConfig = objWebConfig
        'objScrivi.VariabiliSessione = objVarSess
        'objScrivi.ParametriGiasOnline = objGiasOnline

        'Dim strJS As String
        'strJS = objScrivi.ApriPopUpConSito(objWebConfig.LinkGiasOnline, _
        '                                    Enum_SiteRedirector.Sito_AgronicaAgenda_2010, _
        '                                    Enum_SiteRedirector.Sito_GiasOnline)

        'ScriptManager.RegisterStartupScript(CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel), _
        '                                    CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel).GetType(), _
        '                                    "jQuery_{0}", strJS, False)

        Dim strJS As String
        strJS = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoOnline_PassandoDirettamente_ParametriGiasOnline(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objGiasOnline)

        ScriptManager.RegisterStartupScript(CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel), _
                                            CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel).GetType(), _
                                            "jQuery_{0}", strJS, False)




    End Sub

    Private Sub BTN_CaricoMagazzino(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim xChiave As String = ""
        Call Albero.ChiaveAlbero_Codifica(xChiave, _
                                               enum_TipoNodo.p_PortafoglioProdotti, _
                                               objParametriAgenda.Fabbricato.Split("|")(2), _
                                               objParametriAgenda.Fabbricato.Split("|")(1), , , , , , , , , , , , _
                                               objParametriAgenda.Fabbricato.Split("|")(0))

        ' FormProdotto non è presente su DomandaIrrigua: redirect cross-site verso AgroAgenda
        ' tramite il meccanismo standard ParametriAgenda_2010 → GestioneRichieste → FormProdotto.aspx
        Dim objPA2010 As New ParametriAgenda_2010()
        objPA2010.PaginaRichiesta     = enum_PagineAgenda_2010.Pagina_FormProdotto
        objPA2010.Chiave              = xChiave
        objPA2010.OperazioneMagazzino = "C"
        objPA2010.Mode                = "magazzino"
        objPA2010.Lavorazione         = objParametriAgenda.Lav_Cod
        objPA2010.DataSelezionata     = objParametriAgenda.Data
        objPA2010.Sa_Cod              = objParametriAgenda.Sa_Cod
        objPA2010.Id_Agenda           = objParametriAgenda.Id_Agenda
        objPA2010.Piva                = objParametriAgenda.Piva
        objPA2010.Salva()

        Dim url As String = RedirectGestione.PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(
            Enum_SiteRedirector.Sito_AgronicaDomandaIrrigua,
            Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
            "")

        Dim strJS As String = "<script language='javascript'>" &
            "window.open('" & url & "'," &
            "'stampe'," &
            "'height=700,width=1000,menubar=yes,resizable=yes,scrollbars=yes,top=0,left=0');" &
            "</script>"

        ScriptManager.RegisterStartupScript(CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel),
                                            CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel).GetType(),
                                            "jQuery_{0}", strJS, False)

    End Sub

    ''' <summary>
    ''' Bottone di annullamento
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)
        Trattamenti_Disposed()
        objParametriAgenda.Svuota_DatiOperazione()


        Dim link As String = ""
        Try
            Dim sitoorigine As Enum_SiteRedirector = objParametriAgenda.SitoOrigine
            Dim paginaOnLineRitorno As Integer = objParametriAgenda.PaginaSitoOrigine

            If sitoorigine = Enum_SiteRedirector.Sito_GiasOnline_2010 AndAlso paginaOnLineRitorno = enum_PagineGiasOnline_2010.RegistazioneSmart Then
                link = RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri(
                                       Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                       enum_PagineGiasOnline_2010.Menu,
                                       enum_PagineAgenda_2010.Menu, objParametriAgenda.Piva, "", "", 0, "")

            ElseIf sitoorigine = Enum_SiteRedirector.GiasNG Then
                MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva,
                                                                Enum_SiteRedirector.GiasNG,
                                                                objParametriAgenda.PaginaSitoOrigine,
                                                                link,
                                                                objParametri_Server,
                                                                SitoOrigine:=Enum_SiteRedirector.Sito_AgronicaDomandaIrrigua)

            Else
                link = CType(Master.Master, Agenda).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
            End If

        Catch ex As Exception
            link = CType(Master.Master, Agenda).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
        End Try

        Response.Redirect(link)

    End Sub

#End Region

#Region "Carica Combo"

    ''' <summary>
    ''' Funzione invocata dal controllo dei Disciplinari
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CaricaComboDisciplinariExteso()

        'Dim _Includi_Biologico As Boolean = True
        Dim _Includi_Biologico As Boolean = False

        ComboDisciplinari1.WS_Disciplinari_AgroWS_Disciplinari = objParametriAgenda.WS_Disciplinari_AgroWS_Disciplinari
        ComboDisciplinari1.Includi_Biologico = _Includi_Biologico
        ComboDisciplinari1.Veg_Cod = objParametriAgenda.Veg_Cod.Split("/")(0)

        'Select Case objParametriAgenda.Lav_Cod
        '    Case LAVCOD_DISTRIBUZIONE_INSETTI, LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_TRATTAMENTO_FITOREGOLATORE, LAVCOD_CONCIA_SEME, LAVCOD_GEODISINFESTAZIONE
        '        ComboDisciplinari1.Tipo_Testata = 0
        '    Case LAVCOD_DISERBO
        '        ComboDisciplinari1.Tipo_Testata = 1
        '    Case LAVCOD_DISSECCAMENTO
        '        ComboDisciplinari1.Tipo_Testata = 0
        'End Select

        If Session("permessoDPIPrivati") = False Then
            ComboDisciplinari1.Flag_Privato_Pubblico = 1
        Else
            ComboDisciplinari1.Flag_Privato_Pubblico = 0
        End If

        ComboDisciplinari1.FinestraTemporaleInizio = objParametriAgenda.Data
        ComboDisciplinari1.FinestraTemporaleFine = objParametriAgenda.Data
        ComboDisciplinari1.CaricaComboDisciplinari()

        'se ho come impostazioni utente il dpi lo imposto
        'metto un try catch in modo che non sia bloccante se ci sono errori
        Try

            If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura AndAlso
                Not IsNothing(ViewState("ImpostoDPINellaCombo")) AndAlso
                ViewState("ImpostoDPINellaCombo") = True AndAlso
                ComboDisciplinari1.ddl_Disciplinari.Items.Count > 1 Then

                ComboDisciplinari1.ddl_Disciplinari.SelectedIndex = 1

                If Not IsNothing(Session("DpiPredefinitoUtente")) AndAlso Session("DpiPredefinitoUtente") <> "0" Then

                    ' ComboDisciplinari1.ddl_Disciplinari.SelectedValue = Session("DpiPredefinitoUtente")
                    'se cambio la specie il dpi predefinito cambia, cambia il codice della specie,
                    'quini non posso fare un assegnamento diretto ma devo ciclare
                    Dim disciplinare As String = Session("DpiPredefinitoUtente")
                    ComboDisciplinari1.ddl_Disciplinari.SelectedIndex = 0
                    For kk = 0 To ComboDisciplinari1.ddl_Disciplinari.Items.Count - 1
                        Dim val As String() = ComboDisciplinari1.ddl_Disciplinari.Items(kk).Value.Split("/")

                        If val(0) = disciplinare.Split("/")(0) Then
                            If val.Length = 1 Then
                                ComboDisciplinari1.ddl_Disciplinari.SelectedIndex = kk
                            End If
                            If val.Length = 5 AndAlso disciplinare.Split("/").Length = 2 Then
                                If val(4) = disciplinare.Split("/")(1) Then
                                    ComboDisciplinari1.ddl_Disciplinari.SelectedIndex = kk
                                End If
                            End If
                            If val.Length = 5 AndAlso disciplinare.Split("/").Length = 5 Then
                                If val(4) = disciplinare.Split("/")(4) Then
                                    ComboDisciplinari1.ddl_Disciplinari.SelectedIndex = kk
                                End If
                            End If

                        End If

                    Next

                End If

                objParametriAgenda.Disciplinare = ComboDisciplinari1.Valore_Combo
                Session("DpiPredefinitoUtente") = objParametriAgenda.Disciplinare

                'rimuovo dalla combo il filtro nessuno--> nessuno
                Dim jj As Integer = 0
                For jj = 0 To ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.Items.Count - 1
                    If ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.Items(jj).Value = 0 Then
                        ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.Items.RemoveAt(jj)
                        Exit For
                    End If
                Next
            End If

        Catch ex As Exception
            'ignoro
        End Try

    End Sub


    ''' <summary>
    ''' Imposta i Filtri Aggiuntivi
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub CaricaComboFiltriAggiuntivi()
        ComboFiltriAggiuntiviAgenda1.Lav_Cod = objParametriAgenda.Lav_Cod
        ComboFiltriAggiuntiviAgenda1.CaricaComboFiltriAggiuntivi()
    End Sub



    Private Sub AggiornaFiltroRicerca()
        CaricaComboFiltriAggiuntivi()
    End Sub

#End Region

#Region "COMBO SelectedIndexChanged"

    ''' <summary>
    ''' UNITA' DI MISURA
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CaricaComboUnitadiMisura()

        AgronicaCoreUtility.CaricaListControl.UnitaMisuraAgenda(Cmb_UdM, _
                                                      False, _
                                                      "", "", _
                                                      FORMULATI, _
                                                      "", _
                                                      "", _
                                                      objParametri_Server)

        aggiornaGiacenzaPerUdm()

    End Sub




#End Region


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If

        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        '---
        objParametriAgenda = New ParametriAgenda
        'objParametriAgenda.Leggi()

        Id_Agenda_Old = objParametriAgenda.Id_Agenda

        'script iniziali
        InizializzaScriptClient()


        ComboFiltriAggiuntiviAgenda1.Lav_Cod = objParametriAgenda.Lav_Cod



        '##############################################################
        '#####  Recupero la chiave che identifica l'oggetto  ##########
        '##############################################################
        If Not IsPostBack Then

            '==================================
            '======= VERIFICA PERMESSI ========
            '==================================
            VerificaPermessi()

            Dim objAgroWebConfig As New AgroWebConfig
            IndirizzoProfitosan.Value = objAgroWebConfig.LinkProfitosan



            Txt_Acqua_Ha.Text = "0"
            Txt_Acqua_Tot.Text = "0"
            Txt_DoseTot_HA.Text = "0"
            Txt_Dose_HA.Text = "0"
            Txt_Dose_HL.Text = "0"


            CaricaGriglia_Dosi()

            Cella_Avversita.Visible = True

            AggiornaFiltroRicerca()


            '--------------------------------------
            'leggo le eventuali IMPOSTAZIONI UTENTE

            CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_CheckDPI"), ImageButton).Visible = True

            SettaImpostazioneUtente_UDM()

            Select Case objParametriAgenda.Tipo_Operazione

                Case TipiEnumerativi.enum_TipoOperazioneDB.Scrittura
                    CaricaComboDisciplinariExteso()

                    SettaImpostazioniUtente()
                    '--------------------------------
                    Aggiorna_Dpi()

                    'CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_CheckDPI"), ImageButton).Visible = True
                    If objParametriAgenda.Disciplinare <> "0" Then
                        'CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_CheckDPI"), ImageButton).Visible = True
                        CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_Disciplinare"), ImageButton).Visible = True
                        Select Case objParametriAgenda.Lav_Cod
                            Case LAVCOD_DISERBO
                                CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_DoseConsigliata"), ImageButton).Visible = True
                            Case Else
                                CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_DoseConsigliata"), ImageButton).Visible = False
                        End Select
                    Else
                        'CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_CheckDPI"), ImageButton).Visible = True
                        CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_Disciplinare"), ImageButton).Visible = False
                        CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_DoseConsigliata"), ImageButton).Visible = False
                    End If

                Case TipiEnumerativi.enum_TipoOperazioneDB.Lettura, TipiEnumerativi.enum_TipoOperazioneDB.Modifica
                    LockModifica.Visible = True

                    SettaImpostazioniUtente()

                    Ripristina_Dati_nei_Controlli()

                    'controlo il permesso sulla specie, se non ce l'ho metto operazione in lettura
                    If objParametriAgenda.Tipo_Operazione = CStr(TipiEnumerativi.enum_TipoOperazioneDB.Modifica) Then
                        Try
                            Dim objSpecVeg As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
                            Dim Dt As DataTable = objSpecVeg.SpecieVegetali_GestioneFiltroUtente_Leggi(objParametriAgenda.Veg_Cod.Split("/")(0), _
                                                                             0, _
                                                                             "", _
                                                                             "", _
                                                                             "", _
                                                                             "", _
                                                                             objParametri_Utenti)
                            If Dt.Rows.Count = 0 Then
                                objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Lettura
                            End If
                        Catch ex As Exception
                            objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Lettura
                        End Try
                    End If

                    If objParametriAgenda.Tipo_Operazione = TipiEnumerativi.enum_TipoOperazioneDB.Lettura Then

                        CType(Ricerca.FindControlIterative(Page.Master, "Box_Salva"), Panel).Visible = False
                        'CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_Duplica"), ImageButton).Visible = False

                        ' Ripristina_Dati_nei_Controlli()
                        Riga_Formulati.Visible = False

                        If objParametriAgenda.Disciplinare <> "0" Then
                            'CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_CheckDPI"), ImageButton).Visible = True
                            CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_Disciplinare"), ImageButton).Visible = True
                            Select Case objParametriAgenda.Lav_Cod
                                Case LAVCOD_DISERBO
                                    CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_DoseConsigliata"), ImageButton).Visible = True
                                Case Else
                                    CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_DoseConsigliata"), ImageButton).Visible = False
                            End Select
                        Else
                            'CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_CheckDPI"), ImageButton).Visible = True
                            CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_Disciplinare"), ImageButton).Visible = False
                            CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_DoseConsigliata"), ImageButton).Visible = False
                        End If

                    End If

                    If objParametriAgenda.Tipo_Operazione = TipiEnumerativi.enum_TipoOperazioneDB.Modifica Then

                        CType(Ricerca.FindControlIterative(Page.Master, "RBL_Salva"), RadioButtonList).Visible = False
                        'CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_Duplica"), ImageButton).Visible = False

                        ' Ripristina_Dati_nei_Controlli()
                        CType(Page.Master, Operazione).CaricaCostiAccessori()

                        'SettaImpostazioniUtente()

                        If objParametriAgenda.Disciplinare <> "0" Then
                            'CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_CheckDPI"), ImageButton).Visible = True
                            CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_Disciplinare"), ImageButton).Visible = True
                            Select Case objParametriAgenda.Lav_Cod
                                Case LAVCOD_DISERBO
                                    CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_DoseConsigliata"), ImageButton).Visible = True
                                Case Else
                                    CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_DoseConsigliata"), ImageButton).Visible = False
                            End Select
                        Else
                            'CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_CheckDPI"), ImageButton).Visible = True
                            CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_Disciplinare"), ImageButton).Visible = False
                            CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_DoseConsigliata"), ImageButton).Visible = False
                        End If

                    End If



            End Select

            If ComboDisciplinari1.Valore_Combo <> "0" Then
                Session("Disciplinare_Attivo") = True
            Else
                Session("Disciplinare_Attivo") = False
            End If

        Else
            Dim app As Integer = CType(Ricerca.FindControlIterative(Page.Master, "ComboSpecie"), ComboSpecie).Valore_Combo.Split("/")(0)
            If objParametriAgenda.Veg_Cod.Split("/")(0) <> app Then
                objParametriAgenda.Veg_Cod.Split("/")(0) = app
            End If

            Exit Sub
        End If




    End Sub

    '###############################################################################
    'Eseguito quando cambio il centro az o la specie, in questo caso devo azzerarmi il dpi e ricaricare la combo
    Public Sub Aggiorna_Centro_Specie()

        'Imposto dpi a zero
        objParametriAgenda.Disciplinare = "0"

        'ricarico combo dpi
        CaricaComboDisciplinariExteso()

        'gestisco cambio dpi (per oggetto agenda e per ricaricare impianti)
        Cambiato_Disciplinare()

        caricaDtGiacenze()

    End Sub

    Protected Sub BTN_ComboDisciplinari1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTN_ComboDisciplinari1.Click
        Cambiato_Disciplinare()
    End Sub

    Private Sub Cambiato_Disciplinare()
        If ComboDisciplinari1.Valore_Combo <> "0" Then
            Session("Disciplinare_Attivo") = True
        Else
            Session("Disciplinare_Attivo") = False
        End If
        If objParametriAgenda.Disciplinare <> ComboDisciplinari1.Valore_Combo Then
            objParametriAgenda.Disciplinare = ComboDisciplinari1.Valore_Combo
            'objParametriAgenda.Leggi()

            'CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_CheckDPI"), ImageButton).Visible = True
            If objParametriAgenda.Disciplinare <> "0" AndAlso objParametriAgenda.Disciplinare <> "-2" Then
                CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_Disciplinare"), ImageButton).Visible = True
                'CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_CheckDPI"), ImageButton).Visible = True
                'ricarico i filtri di ricerca
                ComboFiltriAggiuntiviAgenda1.Disciplinare = Session("Disciplinare_Attivo")
                ComboFiltriAggiuntiviAgenda1.CaricaComboFiltriAggiuntivi()
                Select Case objParametriAgenda.Lav_Cod
                    Case LAVCOD_DISERBO
                        CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_DoseConsigliata"), ImageButton).Visible = True
                    Case Else
                        CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_DoseConsigliata"), ImageButton).Visible = False
                End Select
            Else
                CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_Disciplinare"), ImageButton).Visible = False
                'CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_CheckDPI"), ImageButton).Visible = True
                'ricarico i filtri di ricerca
                If ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.Items.Count < 3 Then
                    ComboFiltriAggiuntiviAgenda1.Disciplinare = Session("Disciplinare_Attivo")
                    ComboFiltriAggiuntiviAgenda1.CaricaComboFiltriAggiuntivi()
                End If
                CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_DoseConsigliata"), ImageButton).Visible = False
            End If

            'CType(Page.Master, Operazione).GridView_Impianti_EvidenziaRigheDPI()
            Aggiorna_Dpi()


            'invop il click su aggiornagriglia impianti
            Dim STR_UpdatePanelDose As New StringBuilder
            STR_UpdatePanelDose.AppendLine("$(document).ready(function () { ")
            STR_UpdatePanelDose.AppendLine("    $('#" & CType(Page.Master, Operazione).Property_AggiornaGrigliaImpianti.ClientID & "').click();")
            STR_UpdatePanelDose.AppendLine(" });")


        End If

    End Sub

    Public Sub Aggiorna_Dpi()

        If Not IsNothing(objParametriAgenda.Disciplinare) AndAlso _
            objParametriAgenda.Disciplinare <> "0" AndAlso _
            objParametriAgenda.Disciplinare <> "-2" AndAlso _
            objParametriAgenda.Disciplinare <> "" Then

            Dim Array() As String
            Dim Dpi_Cod As Integer = 0
            Dim IdRcdpi As Integer = 0
            Dim Grfi_Cod As Integer = 0
            Dim Flag_Protetto As Integer = 0
            Dim Flag_PubblicoPrivato As Integer = 0
            Dim TipoTestata As Integer = 0

            Array = Split(objParametriAgenda.Disciplinare, "/")
            Dpi_Cod = Array(0)
            IdRcdpi = Array(1)
            Grfi_Cod = Array(2)
            Flag_Protetto = Array(3)
            Flag_PubblicoPrivato = Array(4)

            Select Case objParametriAgenda.Lav_Cod
                Case LAVCOD_DISTRIBUZIONE_INSETTI, LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_TRATTAMENTO_FITOREGOLATORE, LAVCOD_CONCIA_SEME, LAVCOD_GEODISINFESTAZIONE
                    TipoTestata = 0
                Case LAVCOD_DISERBO
                    TipoTestata = 1
                Case LAVCOD_DISSECCAMENTO
                    TipoTestata = 0
            End Select

        End If

        If Not LockModifica.Visible Then
            CaricaGriglia_Avversita()
        End If

    End Sub

    '########################################################################################
    ' CaricaGriglia_Avversita utilizzata con e senza DPI
    '########################################################################################
    Private Sub CaricaGriglia_Avversita(Optional ByVal FrCod As Integer = 0)

        '----- Definizione delle variabili

        Dim Dt_Avv_Tot As DataTable = Nothing
        Dim Dt_Inf_Tot As DataTable
        Dim DrInf() As DataRow
        Dim DrInfGru() As DataRow

        Dim DtAvv As New DataTable
        Dim Dr As DataRow
     
        Dim i As Integer

        '----- Definisco la struttura dei DataTable

        DtAvv.Columns.Add(New DataColumn("Av_Des", GetType(String)))
        DtAvv.Columns.Add(New DataColumn("Av_Cod", GetType(Integer)))
        DtAvv.Columns.Add(New DataColumn("Av_Gru", GetType(Integer)))
        DtAvv.Columns.Add(New DataColumn("Avversita_Infestanti", GetType(Integer)))

        Dim DtKeys(1) As DataColumn

        DtKeys(0) = DtAvv.Columns("Av_Cod")
        DtKeys(1) = DtAvv.Columns("Av_Gru")

        DtAvv.PrimaryKey = DtKeys


        If objParametriAgenda.Veg_Cod.Split("/")(0) = "" Then
            objParametriAgenda.Veg_Cod = 0
        End If
        '---------------------------------------------------------------
        If objParametriAgenda.Veg_Cod.Split("/")(0) = "" OrElse objParametriAgenda.Veg_Cod.Split("/")(0) = "-1" Then
            'If objParametriAgenda.Veg_Cod = "" OrElse objParametriAgenda.Veg_Cod = "0" OrElse objParametriAgenda.Veg_Cod = "-1" Then
            'AgroMsgBox("Selezionare la Specie Vegetale!", Page)
            Exit Sub
        End If


        Select Case Session("Collegamento_Fito")

            Case True

                '----------------------------------
                ' DISCIPLINARE
                '----------------------------------
                If Session("Collegamento_DPI") = True Then

                    Select Case objParametriAgenda.Disciplinare

                        Case "0", "-2"

                            '----------------------------------
                            ' NESSUN DISCIPLINARE
                            '----------------------------------

                            Dim objDPILeggi As New AgronicaCoreDpiBIZ.Fitofarmaci_Leggi

                            Dt_Avv_Tot = objDPILeggi.Leggi_Avversita_Da_Formulati_SpecieVegetali_Avversita_4(FrCod, _
                                                                                CInt(objParametriAgenda.Veg_Cod.Split("/")(0)), _
                                                                                0, _
                                                                                0, _
                                                                                1, _
                                                                                Txt_Avv.Text, _
                                                                                objParametriAgenda.Data, _
                                                                                Session)

                            Dt_Inf_Tot = objDPILeggi.Leggi_Infestanti_Da_Formulati_SpecieVegetali_Infestanti_4(FrCod, _
                                                                            CInt(objParametriAgenda.Veg_Cod.Split("/")(0)), _
                                                                            0, _
                                                                            0, _
                                                                            1, _
                                                                            Txt_Avv.Text, _
                                                                            objParametriAgenda.Data, _
                                                                            Session)


                            If IsNothing(Dt_Avv_Tot) AndAlso IsNothing(Dt_Inf_Tot) Then
                                Messaggi.AgroMsgBox("Nessuna Avversità/Infestante Trovata", Page, , _
                                 CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
                                Exit Sub
                            End If

                            For i = 0 To Dt_Avv_Tot.Rows.Count - 1
                                If Dt_Avv_Tot.Rows(i).Item("Av_Cod") <> 0 Then
                                    If InStr(Dt_Avv_Tot.Rows(i).Item("Av_Des_Vol"), "non usare") = 0 Then 'todo, non usare come lo traduco?!!!
                                        Dr = DtAvv.NewRow
                                        Dr.Item("Av_Des") = Dt_Avv_Tot.Rows(i).Item("Av_Des_Vol") & " (<i>" & Dt_Avv_Tot.Rows(i).Item("Av_Des_Lat") & "</i>)"
                                        Dr.Item("Av_Cod") = Dt_Avv_Tot.Rows(i).Item("Av_Cod")
                                        Dr.Item("Av_Gru") = 0
                                        Dr.Item("Avversita_Infestanti") = 0
                                        Try
                                            DtAvv.Rows.Add(Dr)
                                        Catch ex As Exception
                                        End Try
                                    End If
                                ElseIf Dt_Avv_Tot.Rows(i).Item("Av_Gru") <> 0 Then
                                    If InStr(Dt_Avv_Tot.Rows(i).Item("Av_Gru_Des"), "non usare") = 0 Then 'todo, non usare come lo traduco?!!
                                        Dr = DtAvv.NewRow
                                        Dr.Item("Av_Des") = Dt_Avv_Tot.Rows(i).Item("Av_Gru_Des") & " (<i>" & Dt_Avv_Tot.Rows(i).Item("Av_Gru_Des_Lat") & "</i>)"
                                        Dr.Item("Av_Gru") = Dt_Avv_Tot.Rows(i).Item("Av_Gru")
                                        Dr.Item("Av_Cod") = 0
                                        Dr.Item("Avversita_Infestanti") = 0
                                        Try
                                            DtAvv.Rows.Add(Dr)
                                        Catch ex As Exception
                                        End Try
                                    End If
                                End If
                            Next

                            For i = 0 To Dt_Inf_Tot.Rows.Count - 1
                                If Dt_Inf_Tot.Rows(i).Item("Av_Cod") <> 0 Then
                                    If InStr(Dt_Inf_Tot.Rows(i).Item("Av_Des_Vol"), "non usare") = 0 Then 'todo, non usare come lo traduco?!!!
                                        Dr = DtAvv.NewRow
                                        Dr.Item("Av_Des") = Dt_Inf_Tot.Rows(i).Item("Av_Des_Vol") & " (<i>" & Dt_Inf_Tot.Rows(i).Item("Av_Des_Lat") & "</i>)"
                                        Dr.Item("Av_Cod") = Dt_Inf_Tot.Rows(i).Item("Av_Cod")
                                        Dr.Item("Av_Gru") = 0
                                        Dr.Item("Avversita_Infestanti") = 1
                                        Try
                                            DtAvv.Rows.Add(Dr)
                                        Catch ex As Exception
                                        End Try
                                    End If
                                ElseIf Dt_Inf_Tot.Rows(i).Item("Av_Gru") <> 0 Then
                                    If InStr(Dt_Inf_Tot.Rows(i).Item("Av_Gru_Des"), "non usare") = 0 Then 'todo, non usare come lo traduco?!!
                                        Dr = DtAvv.NewRow
                                        Dr.Item("Av_Des") = Dt_Inf_Tot.Rows(i).Item("Av_Gru_Des") & " (<i>" & Dt_Inf_Tot.Rows(i).Item("Av_Gru_Des_Lat") & "</i>)"
                                        Dr.Item("Av_Gru") = Dt_Inf_Tot.Rows(i).Item("Av_Gru")
                                        Dr.Item("Av_Cod") = 0
                                        Dr.Item("Avversita_Infestanti") = 1
                                        Try
                                            DtAvv.Rows.Add(Dr)
                                        Catch ex As Exception
                                        End Try
                                    End If
                                End If
                            Next

                        Case Else 'DPI

                    End Select

                End If

            Case Else

                '-----------------------
                'SINGOLE

                Dim filtroAvv As String
                Dim DtAvvTmp As DataTable
                Dim DtAvvGruTmp As DataTable
                Dim DtInfTmp As DataTable
                Dim DtInfGruTmp As DataTable

                '-------------------------------------
                'leggo infestanti e gruppi 
                Dim filtroInf As String
                filtroInf &= " Avversita.Av_Des_Vol NOT LIKE '%non usare%' " & _
                             " AND Avversita.Av_Des_Vol NOT LIKE '%(#)%' "

                Dim objInf As New AgronicaCoreMetaSchemaDAL.InfestantiAttive_R
                DtInfTmp = objinf.Leggi(0, 0, _
                                    enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                                    filtroInf, _
                                    "", _
                                    objParametri_Server)


                Dim filtroInfGru As String
                'filtro infestanti/gruppi infestanti
                'elimino avversita/gruppi con (#) e non usare IN SCRITTURA
                filtroinfGru = " GruppoAvversita.Av_Gru_Des NOT LIKE '%non usare%' " & _
                                " AND GruppoAvversita.Av_Gru_Des NOT LIKE '%(#)%' "

                Dim objInfGru As New AgronicaCoreMetaSchemaDAL.GruppoAvversitaAttive_R
                DtInfGruTmp = objinfGru.Leggi(0, 0, _
                                    enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                                    filtroInfGru, _
                                    "", _
                                    objParametri_Server)

                '----------------------------------------------
                'leggo avversita e gruppi

                'elimino avversita/gruppi con (#) e non usare IN SCRITTURA
                filtroAvv &= " AND Avversita.Av_Des_Vol NOT LIKE '%non usare%' " & _
                             " AND Avversita.Av_Des_Vol NOT LIKE '%(#)%' "

                Dim objAvv As New AgronicaCoreMetaSchemaDAL.Avversita_R
                DtAvvTmp = objAvv.Leggi(0, "", _
                                    enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                                    filtroAvv, _
                                    "", _
                                    objParametri_Server)
                objAvv = Nothing
                If Not IsNothing(DtAvvTmp) Then
                    For i = 0 To DtAvvTmp.Rows.Count - 1
                        Dr = DtAvv.NewRow
                        Dr.Item("Av_Des") = DtAvvTmp.Rows(i).Item("Av_Des_Vol") & " (<i>" & DtAvvTmp.Rows(i).Item("Av_Des_Lat") & "</i>)"
                        Dr.Item("Av_Cod") = DtAvvTmp.Rows(i).Item("Av_Cod")
                        Dr.Item("Av_Gru") = 0
                        DrInf = DtInfTmp.Select("av_cod=" & DtAvvTmp.Rows(i).Item("Av_Cod"))
                        If DrInf IsNot Nothing AndAlso DrInf.Length > 0 Then
                            Dr.Item("Avversita_Infestanti") = 1
                        Else
                            Dr.Item("Avversita_Infestanti") = 0
                        End If
                        Try
                            DtAvv.Rows.Add(Dr)
                        Catch ex As Exception
                        End Try
                    Next
                End If

                '-----------------------
                'GRUPPI
                Dim filtroAvvGru As String
                ''filtro infestanti/gruppi infestanti
                'filtroAvvGru = "  NOT EXISTS (SELECT *	FROM GruppoAvversitaAttive " & _
                '             " WHERE GruppoAvversitaAttive.Av_Gru = GruppoAvversita.Av_Gru ) "

                'elimino avversita/gruppi con (#) e non usare IN SCRITTURA
                filtroAvvGru &= " AND GruppoAvversita.Av_Gru_Des NOT LIKE '%non usare%' " & _
                                " AND GruppoAvversita.Av_Gru_Des NOT LIKE '%(#)%' "

                'End If
                Dim objAvvGru As New AgronicaCoreMetaSchemaDAL.GruppoAvversita_R
                DtAvvGruTmp = objAvvGru.Leggi(0, 0, _
                                    enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                                    filtroAvvGru, _
                                    "", _
                                    objParametri_Server)
                objAvvGru = Nothing
                If Not IsNothing(DtAvvGruTmp) Then
                    For i = 0 To DtAvvGruTmp.Rows.Count - 1
                        Dr = DtAvv.NewRow
                        Dr.Item("Av_Cod") = 0
                        Dr.Item("Av_Des") = DtAvvGruTmp.Rows(i).Item("Av_Gru_Des") & " (<i>" & DtAvvGruTmp.Rows(i).Item("Av_Gru_Des_Lat") & "</i>)"
                        Dr.Item("Av_Gru") = DtAvvGruTmp.Rows(i).Item("Av_Gru")
                        DrInfGru = DtInfGruTmp.Select("av_gru=" & DtAvvGruTmp.Rows(i).Item("Av_Gru"))
                        If DrInfGru IsNot Nothing AndAlso DrInfGru.Length > 0 Then
                            Dr.Item("Avversita_Infestanti") = 1
                        Else
                            Dr.Item("Avversita_Infestanti") = 0
                        End If
                        Try
                            DtAvv.Rows.Add(Dr)
                        Catch ex As Exception
                        End Try
                    Next
                End If


        End Select


        '----------------------------------------------------------------------
        '----- Associo il DataTable con la DataGrid

        'uso il dataview per ordinare
        Dim DvAvv As New DataView
       
        DtAvv.TableName = "Av_Des"
        DvAvv.Table = DtAvv
        DvAvv.Sort = "Av_Des ASC"

        GridViewAvversita.DataSource = DvAvv.ToTable
        GridViewAvversita.DataBind()

        Session("DtAvv") = DvAvv.ToTable

        '----------------------------------------------------------------------


   
    End Sub



    Private Sub aggiornaGiacenzaPerUdm()
        'giacenze alla data
        Try
            If Not IsNothing(Session("HashGiacenze")) Then

                Dim Giacenze As Hashtable = Session("HashGiacenze")

                If Not IsNothing(Cmb_UdM) AndAlso Cmb_UdM.Items.Count > 0 Then

                    If Cmb_UdM.SelectedItem.Value <> "" Then

                        If Giacenze.Count > 0 Then

                            Dim udm As String = CStr(get_UDM_Padre(Cmb_UdM.SelectedItem.Value))
                            Lbl_Giacenza.Text = Giacenze.Item(udm)

                        Else
                            Lbl_Giacenza.Text = "0"
                        End If

                    Else
                        Lbl_Giacenza.Text = ""
                    End If

                Else
                    Lbl_Giacenza.Text = ""
                End If

            Else
                Lbl_Giacenza.Text = ""
            End If

        Catch ex As Exception
            Lbl_Giacenza.Text = "0.000"
        End Try


        'giacenze totali
        Try
            If Not IsNothing(Session("HashGiacenzeTotali")) Then

                Dim Giacenze As Hashtable = Session("HashGiacenzeTotali")

                If Not IsNothing(Cmb_UdM) AndAlso Cmb_UdM.Items.Count > 0 Then

                    If Cmb_UdM.SelectedItem.Value <> "" Then

                        If Giacenze.Count > 0 Then

                            Dim udm As String = CStr(get_UDM_Padre(Cmb_UdM.SelectedItem.Value))
                            Lbl_GiacenzaTotale.Text = Giacenze.Item(udm)

                        Else
                            Lbl_GiacenzaTotale.Text = "0"
                        End If

                    Else
                        Lbl_GiacenzaTotale.Text = ""
                    End If

                Else
                    Lbl_GiacenzaTotale.Text = ""
                End If

            Else
                Lbl_GiacenzaTotale.Text = ""
            End If

        Catch ex As Exception
            Lbl_GiacenzaTotale.Text = "0.000"
        End Try

    End Sub

    Private Function get_UDM_Padre(ByVal udm As Integer) As Integer
        Select Case udm
            'peso
            Case 3, 2032, 2, 304, 4
                Return 2

                'acqua
            Case 104, 101, 29
                Return 29
                'devo trasformare le dosi
        End Select
    End Function


    'utilizzato da Nessun DIP o quando si seleziona il DPI (no filtro nessuno nessuno)
    Private Function CreaOggettoAgenda_Normale(ByVal Sa_Cod As Integer, _
                                               ByVal Qta_da_scaricare As List(Of Decimal)) As Operazione_Agenda


        Dim indice As Integer
        '---------------------------------------
        ' recupero i Consigli
        Dim ListaConsigli As List(Of Nota)
        ListaConsigli = CType(Master, Operazione).GetConsigli()


        '---------------------------------------
        ' recupero le NOTE
        Dim strNota As String = CType(Master, Operazione).GetNota()


        '---------------------------------------
        ' recupero la DATA
        Dim Data As Date
        If objParametriAgenda.Data = AGRODATAINIZIO Then
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.IndicareUnaData, Page, , _
                                CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            Return Nothing
        Else
            Data = objParametriAgenda.Data
        End If

        '---------------------------------------
        ' recupero la SPECIE
        Dim Veg_Cod As String = ""
        Dim Veg_Des As String = ""
        If objParametriAgenda.Veg_Cod.Split("/")(0) = "-1" Then
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.SelezionareUnaSpecieVegetale, Page, , _
                                CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            Return Nothing
        Else
            Veg_Cod = objParametriAgenda.Veg_Cod.Split("/")(0)
            Veg_Des = CType(Ricerca.FindControlIterative(Page.Master, "ComboSpecie"), AgronicaControlli_2010.ComboSpecie).Testo_Combo
        End If

        '---------------------------------------
        ' recupero la OPERAZIONE
        Dim Lav_Cod As String = ""
        Dim Lav_Des As String = ""
        If objParametriAgenda.Lav_Cod = "" Then
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.SelezionareUnOperazione, Page, , UpdatePanelMiscela)
            Return Nothing
        Else
            Lav_Cod = objParametriAgenda.Lav_Cod
            Lav_Des = CType(Ricerca.FindControlIterative(Page.Master, "ComboOperazione"), AgronicaControlli_2010.ComboOperazioni).Testo_Combo
        End If


        '---------------------------------------
        ' recupero le DOSI
        Dim Dt_Dosi As New DataTable
        If Session("vs_dtDosi") IsNot Nothing Then
            Dt_Dosi = Session("vs_dtDosi")
            If Dt_Dosi.Rows.Count < 1 Then
                Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.SelezionareAlmenoUnFormulato, Page, , _
                                    CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
                Return Nothing
            End If
        Else
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.SelezionareAlmenoUnFormulato, Page, , _
                                CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            Return Nothing
        End If




        Dim BaseCode As Integer = 0
        Dim TopCode As Integer = 0

        '------------------------------------------------
        '----- Calcolo i valori di BaseCode e TopCode
        '------------------------------------------------
        Calcola_BaseCode_TopCode(BaseCode, TopCode, Session("ASG_ProgressivoGIAS"))




        '------------------------------------------------
        '----- AGENDA
        '------------------------------------------------
        Dim Agenda As Operazione_Agenda
        Dim Nota As Nota
        Dim Movimento As Movimento
        Dim Movimento_Dettaglio_Tecnico As Movimento_Dettaglio_Tecnico
        Dim Movimento_Dettaglio As Movimento_Dettaglio
        Dim Movimento_Destinazione As Movimento_Destinazione

        Dim Array_AvCod() As String
        Dim Array_AvGru() As String

        Agenda = New Operazione_Agenda

        Agenda.Tipo_Operazione = objParametriAgenda.Tipo_Operazione
        Agenda.Id_Agenda = objParametriAgenda.Id_Agenda
        'Agenda.Id_Agenda = 0

        Agenda.Data = Data
        Agenda.Piva = objParametriAgenda.Piva
        Agenda.Sa_Cod = Sa_Cod
        Agenda.Lav_Cod = Lav_Cod
        Agenda.Des_Lib = Lav_Des & " (" & Veg_Des & ")"

        Agenda.BaseCode = BaseCode
        Agenda.TopCode = TopCode


        '------------------------------------------------
        '----- NOTE
        '------------------------------------------------

        If ListaConsigli.Count > 0 Then
            Agenda.Note = New List(Of Nota)
            For i = 0 To ListaConsigli.Count - 1
                Nota = New Nota
                Nota.Id_Agenda = objParametriAgenda.Id_Agenda
                Nota.Nota_Cod = ListaConsigli(i).Nota_Cod
                Agenda.Note.Add(Nota)
            Next
        End If


        '------------------------------------------------
        '----- MOVIMENTI
        '------------------------------------------------
        Agenda.Movimenti = New List(Of Movimento)

        'COSTI ACCESSORI
        For i = 0 To objParametriAgenda.Movimenti.Count - 1
            objParametriAgenda.Movimenti(i).Id_Agenda = objParametriAgenda.Id_Agenda
            objParametriAgenda.Movimenti(i).Sa_Cod = Agenda.Sa_Cod
            If Not IsNothing(objParametriAgenda.Movimenti(i).Movimenti_Dettagli) Then
                Dim j As Integer = 0
                For j = 0 To objParametriAgenda.Movimenti(i).Movimenti_Dettagli.Count - 1
                    objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Id_Agenda = objParametriAgenda.Id_Agenda
                    If objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Sa_Cod = 0 Then
                        objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Sa_Cod = Agenda.Sa_Cod
                    End If
                Next
            End If

            If Not IsNothing(objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici) Then
                Dim j As Integer = 0
                For j = 0 To objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici.Count - 1
                    objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Id_Agenda = objParametriAgenda.Id_Agenda
                    If objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Sa_Cod = 0 Then
                        objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Sa_Cod = Agenda.Sa_Cod
                    End If
                Next
            End If

            objParametriAgenda.Movimenti(i).Data = Data
            Agenda.Movimenti.Add(objParametriAgenda.Movimenti(i))
        Next


        '------------------------------------------------
        '------------------------------------------------
        '----- MOVIMENTO LAVORAZIONE
        '------------------------------------------------
        '------------------------------------------------
        Movimento = New Movimento

        Movimento.Id_Agenda = objParametriAgenda.Id_Agenda
        Movimento.Piva = Agenda.Piva
        Movimento.Sa_Cod = Agenda.Sa_Cod
        Movimento.Data = Data
        Movimento.Lav_Cod = Lav_Cod
        Movimento.Cau_Mov = CAU_TRATTAMENTO
        Movimento.Mov_Desc = strNota
        If rbl_DoseHL.Checked Then
            'HL
            Movimento.Mezzo = 0
        Else
            'q
            Movimento.Mezzo = 1
        End If

        If rbl_QtaDose.Checked Then
            'dose
            Movimento.Modalita = 11
        Else
            'totale
            Movimento.Modalita = 10
        End If


        Movimento.Num_Protocollo = 0
        Movimento.Extra_Int = 0
        Select Case objParametriAgenda.Disciplinare
            Case "0"
                Select Case CInt(ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.SelectedValue)
                    Case "0"
                        Movimento.Num_Protocollo = 0
                    Case Else
                        Movimento.Num_Protocollo = -1
                End Select
            Case "-2"   'BIO
                Select Case CInt(ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.SelectedValue)
                    Case "0"
                        Movimento.Num_Protocollo = 0
                    Case Else
                        Movimento.Num_Protocollo = -2
                End Select
            Case Else   'DPI
                Dim Array() As String
                Array = Split(objParametriAgenda.Disciplinare, "/")
                Movimento.Num_Protocollo = Array(0)
                If Array(4) IsNot Nothing Then
                    Movimento.Disciplinare_PubblicoPrivato = Array(4)
                End If
        End Select

        Movimento.BaseCode = BaseCode
        Movimento.TopCode = TopCode

        Agenda.Movimenti.Add(Movimento)

        '------------------------------------------------
        '----- MOVIMENTO DETTAGLIO TECNICO X ACQUA
        '------------------------------------------------

        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli_Tecnici = New List(Of Movimento_Dettaglio_Tecnico)

        Movimento_Dettaglio_Tecnico = New Movimento_Dettaglio_Tecnico

        Movimento_Dettaglio_Tecnico.Id_Agenda = objParametriAgenda.Id_Agenda
        Movimento_Dettaglio_Tecnico.Piva = Agenda.Piva
        Movimento_Dettaglio_Tecnico.Sa_Cod = Agenda.Sa_Cod
        Movimento_Dettaglio_Tecnico.Data = Data

        If rblAcqua_HA.Checked Then
            Movimento_Dettaglio_Tecnico.Qta_Ril = 0 - CDbl(Txt_Acqua_Ha.Text)
        Else
            Movimento_Dettaglio_Tecnico.Qta_Ril = CDbl(Txt_Acqua_Tot.Text)
        End If

        Movimento_Dettaglio_Tecnico.BaseCode = BaseCode
        Movimento_Dettaglio_Tecnico.TopCode = TopCode

        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)

        '------------------------------------------------
        '----- MOVIMENTI DETTAGLI (1 DETTAGLIO PER OGNI PRODOTTO + 1 DETTAGLIO PER OGNI SEMILAVORATO)
        '------------------------------------------------

        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli = New List(Of Movimento_Dettaglio)

        '-------------------------------------------------
        'dettagli prodotti
        Dim DoseTotale_Trasformata As Decimal
        Dim DoseTotale As Decimal

        Dim SuperficieTotale As Decimal = CDbl(Txt_SupTrattata.Value.Replace(".", ","))
        Dim AcquaTotale As Decimal = CDbl(Txt_Acqua_Tot.Text)
        Dim DoseHA_Trasformata As Decimal

        For i = 0 To Dt_Dosi.Rows.Count - 1

            Movimento_Dettaglio = New Movimento_Dettaglio

            Movimento_Dettaglio.Id_Agenda = objParametriAgenda.Id_Agenda
            Movimento_Dettaglio.Piva = Agenda.Piva
            Movimento_Dettaglio.Sa_Cod = Agenda.Sa_Cod
            Movimento_Dettaglio.Data = Data
            Movimento_Dettaglio.Lav_Cod = Lav_Cod
            Movimento_Dettaglio.Cau_Mov = CAU_TRATTAMENTO

            Movimento_Dettaglio.Elem_Cod = FORMULATI
            Movimento_Dettaglio.Pro_Cod = Dt_Dosi.Rows(i).Item("Fr_Cod")
            Movimento_Dettaglio.Mat_Cod = 0

            If IsNumeric(Dt_Dosi.Rows(i).Item("Carenza")) Then
                Movimento_Dettaglio.TempoCarenza = Dt_Dosi.Rows(i).Item("Carenza")
            Else
                Movimento_Dettaglio.TempoCarenza = 0
            End If
            Movimento_Dettaglio.DoseEtichetta = Dt_Dosi.Rows(i).Item("Dose_Etichetta")

            Movimento_Dettaglio.PrincipiAttivi = Dt_Dosi.Rows(i).Item("strPA_COD")
            Movimento_Dettaglio.CLassiTossicologiche = Dt_Dosi.Rows(i).Item("strCLTOSS_COD")

            Dim DoseHa As Decimal
            DoseHa = Dt_Dosi.Rows(i).Item("Dose")
            Select Case Dt_Dosi.Rows(i).Item("Udm_Cod")
                Case 3  'g
                    DoseHA_Trasformata = DoseHa / 1000  'caso in cui ho i grammi
                Case 2032 'mg
                    DoseHA_Trasformata = DoseHa / 1000000  'caso in cui ho i grammi
                Case 4  'q
                    DoseHA_Trasformata = DoseHa * 100  'caso in cui ho i quintali
                Case 304 't
                    DoseHA_Trasformata = DoseHa * 1000   'caso in cui ho le tonnelate
                Case 101 'ml
                    DoseHA_Trasformata = DoseHa / 1000  'caso in cui ho i ml
                Case 104 'cc
                    DoseHA_Trasformata = DoseHa / 1000  'caso in cui ho i cc
                Case 2, 29 'kg,l
                    DoseHA_Trasformata = DoseHa
            End Select

            Movimento_Dettaglio.Extra_Int = Dt_Dosi.Rows(i).Item("Udm_Cod")
            Movimento_Dettaglio.Udm_Cod = Dt_Dosi.Rows(i).Item("Udm_Cod_Trasformato")

            Movimento_Dettaglio.Qta = Dt_Dosi.Rows(i).Item("Dose")

            Movimento_Dettaglio.Contabilizzato = NONCONTABILE

            Movimento_Dettaglio.BaseCode = BaseCode
            Movimento_Dettaglio.TopCode = TopCode

            Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Add(Movimento_Dettaglio)

            '------------------------------------------------
            '----- MOVIMENTO DETTAGLIO TECNICO
            '------------------------------------------------

            Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(i).Movimenti_Dettagli_Tecnici = New List(Of Movimento_Dettaglio_Tecnico)

            Array_AvCod = Split(Dt_Dosi.Rows(i).Item("Av_Cod"), ",")
            Array_AvGru = Split(Dt_Dosi.Rows(i).Item("Av_Gru"), ",")

            If Not IsNothing(Array_AvCod) Then

                For j = 0 To UBound(Array_AvCod)

                    Movimento_Dettaglio_Tecnico = New Movimento_Dettaglio_Tecnico

                    Movimento_Dettaglio_Tecnico.Id_Agenda = objParametriAgenda.Id_Agenda
                    Movimento_Dettaglio_Tecnico.Piva = Agenda.Piva
                    Movimento_Dettaglio_Tecnico.Sa_Cod = Agenda.Sa_Cod
                    Movimento_Dettaglio_Tecnico.Data = Data

                    If IsNumeric(Array_AvCod(j)) Then
                        Movimento_Dettaglio_Tecnico.Av_Cod = Array_AvCod(j)
                    End If
                    If IsNumeric(Array_AvGru(j)) Then
                        Movimento_Dettaglio_Tecnico.Av_Gru = Array_AvGru(j)
                    End If

                    Movimento_Dettaglio_Tecnico.BaseCode = BaseCode
                    Movimento_Dettaglio_Tecnico.TopCode = TopCode

                    Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(i).Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)

                Next

            End If

        Next


        For i = 0 To GridViewMagazzino.Rows.Count - 1

            If (CType(GridViewMagazzino.Rows(i).FindControl("ChkSeleziona"), CheckBox).Checked = True) Then

                Movimento_Dettaglio = New Movimento_Dettaglio

                Movimento_Dettaglio.Id_Agenda = objParametriAgenda.Id_Agenda
                Movimento_Dettaglio.Piva = Agenda.Piva
                Movimento_Dettaglio.Sa_Cod = Agenda.Sa_Cod
                Movimento_Dettaglio.Data = Data
                Movimento_Dettaglio.Lav_Cod = Lav_Cod
                Movimento_Dettaglio.Cau_Mov = CAU_TRATTAMENTO

                Movimento_Dettaglio.Elem_Cod = CInt(GridViewMagazzino.Rows(i).Cells(1).Text)
                Movimento_Dettaglio.Pro_Cod = 0
                Movimento_Dettaglio.Mat_Cod = CInt(GridViewMagazzino.Rows(i).Cells(2).Text)

                Movimento_Dettaglio.Cod_Progetto = CInt(GridViewMagazzino.Rows(i).Cells(3).Text)
                Movimento_Dettaglio.Cal_Cod = CInt(GridViewMagazzino.Rows(i).Cells(4).Text)

                Movimento_Dettaglio.Lotto = GridViewMagazzino.Rows(i).Cells(12).Text
                Movimento_Dettaglio.Udm_Cod = enum_UnitaMisura.KG 'CInt(GridViewMagazzino.Rows(i).Cells(5).Text)
                Movimento_Dettaglio.Qta = CDbl(CType(GridViewMagazzino.Rows(i).FindControl("TxtQtaGiacenza"), TextBox).Text) * 100

                Movimento_Dettaglio.Contabilizzato = NONCONTABILE

                Movimento_Dettaglio.BaseCode = BaseCode
                Movimento_Dettaglio.TopCode = TopCode

                Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Add(Movimento_Dettaglio)

                '------------------------------------------------
                '----- MOVIMENTI DESTINAZIONI
                '------------------------------------------------

                INDIce = Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Count - 1

                Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(indice).Movimenti_Destinazioni = New List(Of Movimento_Destinazione)

                Movimento_Destinazione = New Movimento_Destinazione

                Movimento_Destinazione.Id_Agenda = objParametriAgenda.Id_Agenda
                Movimento_Destinazione.Data = Data

                Movimento_Destinazione.Piva = CStr(GridViewMagazzino.Rows(i).Cells(6).Text)
                Movimento_Destinazione.Sa_Cod = CInt(GridViewMagazzino.Rows(i).Cells(7).Text)
                Movimento_Destinazione.Appezza = 0
                Movimento_Destinazione.Id_Destinazione = CInt(GridViewMagazzino.Rows(i).Cells(8).Text)
                Movimento_Destinazione.Tipo = MAGAZZINO

                Movimento_Destinazione.BaseCode = BaseCode
                Movimento_Destinazione.TopCode = TopCode

                Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(indice).Movimenti_Destinazioni.Add(Movimento_Destinazione)


            End If

        Next




        '------------------------------------------------
        '------------------------------------------------
        '----- MOVIMENTO SCARICO
        '------------------------------------------------
        '------------------------------------------------

        If objParametriAgenda.Fabbricato <> "0" Then

            Dim sa_cod_magazzino As String = ""
            Dim fabbricatox_Cod As String = ""

            Dim magazzinoEsterno As Boolean = True
            If objParametriAgenda.Fabbricato <> "0" AndAlso Split(objParametriAgenda.Fabbricato, "|").Count = 3 AndAlso Split(objParametriAgenda.Fabbricato, "|")(2) <> objParametriAgenda.Piva Then
                'se magazzino è della azienda padre
                magazzinoEsterno = True
                Dim sa_cod_magazzino_predefinito_azienda As Integer = 0
                Dim fabbricatox_Cod_magazzino_predefinito_azienda As Integer = 0
                Dim fabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R()
                fabbricati.Ricava_PrimoMagazzino_Impresa(Agenda.Piva, sa_cod_magazzino_predefinito_azienda, fabbricatox_Cod_magazzino_predefinito_azienda, objParametri_Server)
                sa_cod_magazzino = sa_cod_magazzino_predefinito_azienda
                fabbricatox_Cod = fabbricatox_Cod_magazzino_predefinito_azienda

            Else
                'se magazzino è quello dell'azienda
                magazzinoEsterno = False
                sa_cod_magazzino = Split(objParametriAgenda.Fabbricato, "|")(1)
                fabbricatox_Cod = Split(objParametriAgenda.Fabbricato, "|")(0)

            End If


            Movimento = New Movimento

            Movimento.Id_Agenda = objParametriAgenda.Id_Agenda
            Movimento.Piva = Agenda.Piva
            Movimento.Sa_Cod = sa_cod_magazzino
            Movimento.Data = Data
            Movimento.Lav_Cod = Lav_Cod

            Movimento.Cau_Mov = CAU_SCARICO
            Movimento.Mov_Desc = "Scarico Magazzino"

            If rbl_DoseHL.Checked Then
                Movimento.Mezzo = 0
            Else
                Movimento.Mezzo = 1
            End If

            Movimento.BaseCode = BaseCode
            Movimento.TopCode = TopCode

            Agenda.Movimenti.Add(Movimento)

            '------------------------------------------------
            '----- MOVIMENTI DETTAGLI
            '------------------------------------------------

            Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli = New List(Of Movimento_Dettaglio)

            For i = 0 To Dt_Dosi.Rows.Count - 1

                Movimento_Dettaglio = New Movimento_Dettaglio

                Movimento_Dettaglio.Id_Agenda = objParametriAgenda.Id_Agenda
                Movimento_Dettaglio.Piva = Agenda.Piva
                Movimento_Dettaglio.Sa_Cod = sa_cod_magazzino
                Movimento_Dettaglio.Data = Data
                Movimento_Dettaglio.Lav_Cod = Lav_Cod
                Movimento_Dettaglio.Cau_Mov = CAU_SCARICO

                Movimento_Dettaglio.Elem_Cod = FORMULATI
                Movimento_Dettaglio.Pro_Cod = Dt_Dosi.Rows(i).Item("Fr_Cod")
                Movimento_Dettaglio.Mat_Cod = 0

                Movimento_Dettaglio.Extra_Int = 0
                Movimento_Dettaglio.Udm_Cod = Dt_Dosi.Rows(i).Item("Udm_Cod_Trasformato")

                'DoseTotale = Dt_Dosi.Rows(i).Item("Qta_Tot")
                DoseTotale = Qta_da_scaricare(i)

                Select Case Dt_Dosi.Rows(i).Item("Udm_Cod") 'verificare queste conversioni
                    Case 3  'g
                        DoseTotale_Trasformata = DoseTotale / 1000  'caso in cui ho i grammi
                    Case 2032  'mg
                        DoseTotale_Trasformata = DoseTotale / 1000000  'caso in cui ho i grammi
                    Case 4  'q
                        DoseTotale_Trasformata = DoseTotale * 100  'caso in cui ho i quintali
                    Case 304 't
                        DoseTotale_Trasformata = DoseTotale * 1000   'caso in cui ho le tonnelate
                    Case 101 'ml
                        DoseTotale_Trasformata = DoseTotale / 1000  'caso in cui ho i ml
                    Case 104 'cc
                        DoseTotale_Trasformata = DoseTotale / 1000  'caso in cui ho i cc
                    Case 2, 29 'kg,l
                        DoseTotale_Trasformata = DoseTotale
                End Select
                Movimento_Dettaglio.Qta = DoseTotale_Trasformata

                Movimento_Dettaglio.Contabilizzato = NONCONTABILE

                Movimento_Dettaglio.BaseCode = BaseCode
                Movimento_Dettaglio.TopCode = TopCode

                Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Add(Movimento_Dettaglio)

                '------------------------------------------------
                '----- MOVIMENTI DESTINAZIONI
                '------------------------------------------------
                indice = Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Count - 1

                Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(indice).Movimenti_Destinazioni = New List(Of Movimento_Destinazione)

                Movimento_Destinazione = New Movimento_Destinazione

                Movimento_Destinazione.Id_Agenda = objParametriAgenda.Id_Agenda
                Movimento_Destinazione.Data = Data

                Movimento_Destinazione.Piva = objParametriAgenda.Piva
                Movimento_Destinazione.Sa_Cod = sa_cod_magazzino
                Movimento_Destinazione.Appezza = 0
                Movimento_Destinazione.Id_Destinazione = fabbricatox_Cod
                Movimento_Destinazione.Tipo = MAGAZZINO

                Movimento_Destinazione.Qta = DoseTotale_Trasformata '* QTA2_TOT

                Movimento_Destinazione.BaseCode = BaseCode
                Movimento_Destinazione.TopCode = TopCode

                Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(indice).Movimenti_Destinazioni.Add(Movimento_Destinazione)

            Next

        End If

        Return Agenda
    End Function

    'utilizzato da Nessun DIP o quando si seleziona il DPI (no filtro nessuno nessuno)
    Private Function CreaOggettoAgenda_Nessuno(ByVal Sa_Cod As Integer, ByVal Qta_da_scaricare As List(Of Decimal)) As Operazione_Agenda


        '---------------------------------------
        ' recupero i Consigli
        Dim ListaConsigli As List(Of Nota)
        ListaConsigli = CType(Master, Operazione).GetConsigli()


        '---------------------------------------
        ' recupero le NOTE
        Dim strNota As String = CType(Master, Operazione).GetNota()


        '---------------------------------------
        ' recupero la DATA
        Dim Data As Date
        If objParametriAgenda.Data = AGRODATAINIZIO Then
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.IndicareUnaData, Page, , _
                                CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            Return Nothing
        Else
            Data = objParametriAgenda.Data
        End If

        '---------------------------------------
        ' recupero la SPECIE
        Dim Veg_Cod As String = ""
        Dim Veg_Des As String = ""
        If objParametriAgenda.Veg_Cod.Split("/")(0) = "-1" Then
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.SelezionareUnaSpecieVegetale, Page, , _
                                CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            Return Nothing
        Else
            Veg_Cod = objParametriAgenda.Veg_Cod.Split("/")(0)
            Veg_Des = CType(Ricerca.FindControlIterative(Page.Master, "ComboSpecie"), AgronicaControlli_2010.ComboSpecie).Testo_Combo
        End If

        '---------------------------------------
        ' recupero la OPERAZIONE
        Dim Lav_Cod As String = ""
        Dim Lav_Des As String = ""
        If objParametriAgenda.Lav_Cod = "" Then
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.SelezionareUnOperazione, Page, , UpdatePanelMiscela)
            Return Nothing
        Else
            Lav_Cod = objParametriAgenda.Lav_Cod
            Lav_Des = CType(Ricerca.FindControlIterative(Page.Master, "ComboOperazione"), AgronicaControlli_2010.ComboOperazioni).Testo_Combo
        End If


        '---------------------------------------
        ' recupero le DOSI
        Dim Dt_Dosi As New DataTable
        If Session("vs_dtDosi") IsNot Nothing Then
            Dt_Dosi = Session("vs_dtDosi")
            If Dt_Dosi.Rows.Count < 1 Then
                Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.SelezionareAlmenoUnFormulato, Page, , _
                                    CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
                Return Nothing
            End If
        Else
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.SelezionareAlmenoUnFormulato, Page, , _
                                CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            Return Nothing
        End If




        Dim BaseCode As Integer = 0
        Dim TopCode As Integer = 0

        '------------------------------------------------
        '----- Calcolo i valori di BaseCode e TopCode
        '------------------------------------------------
        Calcola_BaseCode_TopCode(BaseCode, TopCode, Session("ASG_ProgressivoGIAS"))




        '------------------------------------------------
        '----- AGENDA
        '------------------------------------------------
        Dim Agenda As Operazione_Agenda
        Dim Nota As Nota
        Dim Movimento As Movimento
        Dim Movimento_Dettaglio_Tecnico As Movimento_Dettaglio_Tecnico
        Dim Movimento_Dettaglio As Movimento_Dettaglio
        Dim Movimento_Destinazione As Movimento_Destinazione


        Agenda = New Operazione_Agenda

        Agenda.Tipo_Operazione = objParametriAgenda.Tipo_Operazione
        'Agenda.Id_Agenda = 0
        Agenda.Id_Agenda = objParametriAgenda.Id_Agenda
        Agenda.Data = Data
        Agenda.Piva = objParametriAgenda.Piva
        Agenda.Sa_Cod = Sa_Cod
        Agenda.Lav_Cod = Lav_Cod
        Agenda.Des_Lib = Lav_Des & " (" & Veg_Des & ")"

        Agenda.BaseCode = BaseCode
        Agenda.TopCode = TopCode


        '------------------------------------------------
        '----- NOTE
        '------------------------------------------------

        If ListaConsigli.Count > 0 Then
            Agenda.Note = New List(Of Nota)
            For i = 0 To ListaConsigli.Count - 1
                Nota = New Nota
                Nota.Id_Agenda = objParametriAgenda.Id_Agenda
                Nota.Nota_Cod = ListaConsigli(i).Nota_Cod
                Agenda.Note.Add(Nota)
            Next
        End If


        '------------------------------------------------
        '----- MOVIMENTI
        '------------------------------------------------
        Agenda.Movimenti = New List(Of Movimento)


        'COSTI ACCESSORI
        For i = 0 To objParametriAgenda.Movimenti.Count - 1
            objParametriAgenda.Movimenti(i).Id_Agenda = objParametriAgenda.Id_Agenda
            objParametriAgenda.Movimenti(i).Sa_Cod = Agenda.Sa_Cod
            If Not IsNothing(objParametriAgenda.Movimenti(i).Movimenti_Dettagli) Then
                Dim j As Integer = 0
                For j = 0 To objParametriAgenda.Movimenti(i).Movimenti_Dettagli.Count - 1
                    objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Id_Agenda = objParametriAgenda.Id_Agenda
                    If objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Sa_Cod = 0 Then
                        objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Sa_Cod = Agenda.Sa_Cod
                    End If
                Next
            End If

            If Not IsNothing(objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici) Then
                Dim j As Integer = 0
                For j = 0 To objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici.Count - 1
                    objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Id_Agenda = objParametriAgenda.Id_Agenda
                    If objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Sa_Cod Then
                        objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Sa_Cod = Agenda.Sa_Cod
                    End If
                Next
            End If

            objParametriAgenda.Movimenti(i).Data = Data
            Agenda.Movimenti.Add(objParametriAgenda.Movimenti(i))
        Next




        '------------------------------------------------
        '------------------------------------------------
        '----- MOVIMENTO LAVORAZIONE
        '------------------------------------------------
        '------------------------------------------------
        Movimento = New Movimento

        Movimento.Id_Agenda = objParametriAgenda.Id_Agenda
        Movimento.Piva = Agenda.Piva
        Movimento.Sa_Cod = Agenda.Sa_Cod
        Movimento.Data = Data
        Movimento.Lav_Cod = Lav_Cod
        Movimento.Cau_Mov = CAU_TRATTAMENTO
        Movimento.Mov_Desc = strNota
        If rbl_DoseHL.Checked Then
            'HL
            Movimento.Mezzo = 0
        Else
            'HA
            Movimento.Mezzo = 1
        End If

        If rbl_QtaDose.Checked Then
            'dose
            Movimento.Modalita = 11
        Else
            'totale
            Movimento.Modalita = 10
        End If

        'Caso Nessuno Nessuno
        Movimento.Num_Protocollo = 0
        Movimento.Disciplinare_PubblicoPrivato = 0
        Movimento.Extra_Int = 0

        Movimento.BaseCode = BaseCode
        Movimento.TopCode = TopCode

        Agenda.Movimenti.Add(Movimento)

        '------------------------------------------------
        '----- MOVIMENTO DETTAGLIO TECNICO X ACQUA
        '------------------------------------------------
        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli_Tecnici = New List(Of Movimento_Dettaglio_Tecnico)

        Movimento_Dettaglio_Tecnico = New Movimento_Dettaglio_Tecnico
        Movimento_Dettaglio_Tecnico.Id_Agenda = objParametriAgenda.Id_Agenda
        Movimento_Dettaglio_Tecnico.Piva = Agenda.Piva
        Movimento_Dettaglio_Tecnico.Sa_Cod = Agenda.Sa_Cod
        Movimento_Dettaglio_Tecnico.Data = Data

        If rblAcqua_HA.Checked Then
            Movimento_Dettaglio_Tecnico.Qta_Ril = 0 - CDbl(Txt_Acqua_Ha.Text)
        Else
            Movimento_Dettaglio_Tecnico.Qta_Ril = CDbl(Txt_Acqua_Tot.Text)
        End If

        Movimento_Dettaglio_Tecnico.BaseCode = BaseCode
        Movimento_Dettaglio_Tecnico.TopCode = TopCode


        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)

        'End Select

        '------------------------------------------------
        '----- MOVIMENTO DETTAGLIO TECNICO X AVVERSITA
        '------------------------------------------------
        'guardo tutte le avversità
        Dim listAvCod As New List(Of Integer)
        Dim listAvGruCod As New List(Of Integer)
        For i = 0 To GridViewAvversita.Rows.Count - 1
            If CType(GridViewAvversita.Rows(i).FindControl("ChkSelezionaAvversita"), CheckBox).Checked = True Then
                If IsNumeric(GridViewAvversita.Rows(i).Cells(1).Text) AndAlso CInt(GridViewAvversita.Rows(i).Cells(1).Text) <> 0 Then
                    listAvCod.Add(CInt(GridViewAvversita.Rows(i).Cells(1).Text))
                ElseIf IsNumeric(GridViewAvversita.Rows(i).Cells(2).Text) AndAlso CInt(GridViewAvversita.Rows(i).Cells(2).Text) <> 0 Then
                    listAvGruCod.Add(CInt(GridViewAvversita.Rows(i).Cells(2).Text))
                End If
            End If
        Next

        'avversità
        If Not IsNothing(listAvCod) AndAlso listAvCod.Count > 0 Then
            For j = 0 To listAvCod.Count - 1
                Movimento_Dettaglio_Tecnico = New Movimento_Dettaglio_Tecnico
                Movimento_Dettaglio_Tecnico.Id_Agenda = objParametriAgenda.Id_Agenda
                Movimento_Dettaglio_Tecnico.Piva = Agenda.Piva
                Movimento_Dettaglio_Tecnico.Sa_Cod = Agenda.Sa_Cod
                Movimento_Dettaglio_Tecnico.Data = Data
                Movimento_Dettaglio_Tecnico.Av_Cod = listAvCod(j)
                Movimento_Dettaglio_Tecnico.Av_Gru = 0
                Movimento_Dettaglio_Tecnico.BaseCode = BaseCode
                Movimento_Dettaglio_Tecnico.TopCode = TopCode
                Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)
            Next
        End If

        'gruppi avversità
        If Not IsNothing(listAvGruCod) AndAlso listAvGruCod.Count > 0 Then
            For j = 0 To listAvGruCod.Count - 1
                Movimento_Dettaglio_Tecnico = New Movimento_Dettaglio_Tecnico
                Movimento_Dettaglio_Tecnico.Id_Agenda = objParametriAgenda.Id_Agenda
                Movimento_Dettaglio_Tecnico.Piva = Agenda.Piva
                Movimento_Dettaglio_Tecnico.Sa_Cod = Agenda.Sa_Cod
                Movimento_Dettaglio_Tecnico.Data = Data
                Movimento_Dettaglio_Tecnico.Av_Cod = 0
                Movimento_Dettaglio_Tecnico.Av_Gru = listAvGruCod(j)
                Movimento_Dettaglio_Tecnico.BaseCode = BaseCode
                Movimento_Dettaglio_Tecnico.TopCode = TopCode
                Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)
            Next
        End If


        '------------------------------------------------
        '----- MOVIMENTI DETTAGLI
        '------------------------------------------------

        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli = New List(Of Movimento_Dettaglio)

        Dim DoseTotale_Trasformata As Decimal
        Dim DoseTotale As Decimal

        Dim SuperficieTotale As Decimal = CDbl(Txt_SupTrattata.Value.Replace(".", ","))
        Dim AcquaTotale As Decimal = CDbl(Txt_Acqua_Tot.Text)
        'Dim QTA2_TOT As Decimal = 0
        Dim Indice As Integer

        Dim DoseHA_Trasformata As Decimal

        For i = 0 To Dt_Dosi.Rows.Count - 1

            Movimento_Dettaglio = New Movimento_Dettaglio

            Movimento_Dettaglio.Id_Agenda = objParametriAgenda.Id_Agenda
            Movimento_Dettaglio.Piva = Agenda.Piva
            Movimento_Dettaglio.Sa_Cod = Agenda.Sa_Cod
            Movimento_Dettaglio.Data = Data
            Movimento_Dettaglio.Lav_Cod = Lav_Cod
            Movimento_Dettaglio.Cau_Mov = CAU_TRATTAMENTO

            Movimento_Dettaglio.Elem_Cod = FORMULATI
            Movimento_Dettaglio.Pro_Cod = Dt_Dosi.Rows(i).Item("Fr_Cod")
            Movimento_Dettaglio.Mat_Cod = 0

            Movimento_Dettaglio.TempoCarenza = 0
            Movimento_Dettaglio.DoseEtichetta = ""

            Movimento_Dettaglio.PrincipiAttivi = ""
            Movimento_Dettaglio.CLassiTossicologiche = ""

            Dim DoseHa As Decimal 'todo, verificare queste conversioni
            DoseHa = Dt_Dosi.Rows(i).Item("Dose")
            Select Case Dt_Dosi.Rows(i).Item("Udm_Cod")
                Case 3  'g
                    DoseHA_Trasformata = DoseHa / 1000  'caso in cui ho i grammi
                Case 2032  'mg
                    DoseHA_Trasformata = DoseHa / 1000000  'caso in cui ho i mg

                Case 4  'q
                    DoseHA_Trasformata = DoseHa * 100  'caso in cui ho i quintali
                Case 304 't
                    DoseHA_Trasformata = DoseHa * 1000   'caso in cui ho le tonnelate
                Case 101 'ml
                    DoseHA_Trasformata = DoseHa / 1000  'caso in cui ho i ml
                Case 104 'cc
                    DoseHA_Trasformata = DoseHa / 1000  'caso in cui ho i cc
                Case 2, 29 'kg,l
                    DoseHA_Trasformata = DoseHa
            End Select

            Movimento_Dettaglio.Extra_Int = Dt_Dosi.Rows(i).Item("Udm_Cod")
            Movimento_Dettaglio.Udm_Cod = Dt_Dosi.Rows(i).Item("Udm_Cod_Trasformato")

            Movimento_Dettaglio.Qta = Dt_Dosi.Rows(i).Item("Dose")

            Movimento_Dettaglio.Contabilizzato = NONCONTABILE

            Movimento_Dettaglio.BaseCode = BaseCode
            Movimento_Dettaglio.TopCode = TopCode

            Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Add(Movimento_Dettaglio)

        Next

        For i = 0 To GridViewMagazzino.Rows.Count - 1

            If (CType(GridViewMagazzino.Rows(i).FindControl("ChkSeleziona"), CheckBox).Checked = True) Then

                Movimento_Dettaglio = New Movimento_Dettaglio

                Movimento_Dettaglio.Id_Agenda = objParametriAgenda.Id_Agenda
                Movimento_Dettaglio.Piva = Agenda.Piva
                Movimento_Dettaglio.Sa_Cod = Agenda.Sa_Cod
                Movimento_Dettaglio.Data = Data
                Movimento_Dettaglio.Lav_Cod = Lav_Cod
                Movimento_Dettaglio.Cau_Mov = CAU_TRATTAMENTO

                Movimento_Dettaglio.Elem_Cod = CInt(GridViewMagazzino.Rows(i).Cells(1).Text)
                Movimento_Dettaglio.Pro_Cod = 0
                Movimento_Dettaglio.Mat_Cod = CInt(GridViewMagazzino.Rows(i).Cells(2).Text)

                Movimento_Dettaglio.Cod_Progetto = CInt(GridViewMagazzino.Rows(i).Cells(3).Text)
                Movimento_Dettaglio.Cal_Cod = CInt(GridViewMagazzino.Rows(i).Cells(4).Text)

                Movimento_Dettaglio.Lotto = GridViewMagazzino.Rows(i).Cells(12).Text
                Movimento_Dettaglio.Udm_Cod = enum_UnitaMisura.KG 'CInt(GridViewMagazzino.Rows(i).Cells(5).Text)
                Movimento_Dettaglio.Qta = CDbl(CType(GridViewMagazzino.Rows(i).FindControl("TxtQtaGiacenza"), TextBox).Text) * 100

                Movimento_Dettaglio.BaseCode = BaseCode
                Movimento_Dettaglio.TopCode = TopCode

                Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Add(Movimento_Dettaglio)

                '------------------------------------------------
                '----- MOVIMENTI DESTINAZIONI
                '------------------------------------------------

                INDIce = Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Count - 1

                Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(indice).Movimenti_Destinazioni = New List(Of Movimento_Destinazione)

                Movimento_Destinazione = New Movimento_Destinazione

                Movimento_Destinazione.Id_Agenda = objParametriAgenda.Id_Agenda
                Movimento_Destinazione.Data = Data

                Movimento_Destinazione.Piva = CStr(GridViewMagazzino.Rows(i).Cells(6).Text)
                Movimento_Destinazione.Sa_Cod = CInt(GridViewMagazzino.Rows(i).Cells(7).Text)
                Movimento_Destinazione.Appezza = 0
                Movimento_Destinazione.Id_Destinazione = CInt(GridViewMagazzino.Rows(i).Cells(8).Text)
                Movimento_Destinazione.Tipo = MAGAZZINO

                Movimento_Destinazione.BaseCode = BaseCode
                Movimento_Destinazione.TopCode = TopCode

                Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(indice).Movimenti_Destinazioni.Add(Movimento_Destinazione)

            End If

        Next




        '------------------------------------------------
        '------------------------------------------------
        '----- MOVIMENTO SCARICO
        '------------------------------------------------
        '------------------------------------------------

        If objParametriAgenda.Fabbricato <> "0" Then

            Dim sa_cod_magazzino As String = ""
            Dim fabbricatox_Cod As String = ""

            Dim magazzinoEsterno As Boolean = True
            If objParametriAgenda.Fabbricato <> "0" AndAlso Split(objParametriAgenda.Fabbricato, "|").Count = 3 AndAlso Split(objParametriAgenda.Fabbricato, "|")(2) <> objParametriAgenda.Piva Then
                'se magazzino è della azienda padre
                magazzinoEsterno = True
                Dim sa_cod_magazzino_predefinito_azienda As Integer = 0
                Dim fabbricatox_Cod_magazzino_predefinito_azienda As Integer = 0
                Dim fabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R()
                fabbricati.Ricava_PrimoMagazzino_Impresa(Agenda.Piva, sa_cod_magazzino_predefinito_azienda, fabbricatox_Cod_magazzino_predefinito_azienda, objParametri_Server)
                sa_cod_magazzino = sa_cod_magazzino_predefinito_azienda
                fabbricatox_Cod = fabbricatox_Cod_magazzino_predefinito_azienda

            Else
                'se magazzino è quello dell'azienda
                magazzinoEsterno = False
                sa_cod_magazzino = Split(objParametriAgenda.Fabbricato, "|")(1)
                fabbricatox_Cod = Split(objParametriAgenda.Fabbricato, "|")(0)

            End If

            Movimento = New Movimento

            Movimento.Id_Agenda = objParametriAgenda.Id_Agenda
            Movimento.Piva = Agenda.Piva
            Movimento.Sa_Cod = sa_cod_magazzino
            ' Movimento.Sa_Cod = Sa_Cod sbagliato se magazzino in un altro centro
            Movimento.Data = Data
            Movimento.Lav_Cod = Lav_Cod

            Movimento.Cau_Mov = CAU_SCARICO
            Movimento.Mov_Desc = "Scarico Magazzino"

            If rbl_DoseHL.Checked Then
                Movimento.Mezzo = 0
            Else
                Movimento.Mezzo = 1
            End If

            Movimento.BaseCode = BaseCode
            Movimento.TopCode = TopCode

            Agenda.Movimenti.Add(Movimento)

            '------------------------------------------------
            '----- MOVIMENTI DETTAGLI
            '------------------------------------------------

            Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli = New List(Of Movimento_Dettaglio)

            For i = 0 To Dt_Dosi.Rows.Count - 1

                Movimento_Dettaglio = New Movimento_Dettaglio

                Movimento_Dettaglio.Id_Agenda = objParametriAgenda.Id_Agenda
                Movimento_Dettaglio.Piva = Agenda.Piva
                Movimento_Dettaglio.Sa_Cod = sa_cod_magazzino
                Movimento_Dettaglio.Data = Data
                Movimento_Dettaglio.Lav_Cod = Lav_Cod
                Movimento_Dettaglio.Cau_Mov = CAU_SCARICO

                Movimento_Dettaglio.Elem_Cod = FORMULATI
                Movimento_Dettaglio.Pro_Cod = Dt_Dosi.Rows(i).Item("Fr_Cod")
                Movimento_Dettaglio.Mat_Cod = 0

                Movimento_Dettaglio.Extra_Int = 0
                Movimento_Dettaglio.Udm_Cod = Dt_Dosi.Rows(i).Item("Udm_Cod_Trasformato")

                'DoseTotale = Dt_Dosi.Rows(i).Item("Qta_Tot")
                DoseTotale = Qta_da_scaricare(i)



                Select Case Dt_Dosi.Rows(i).Item("Udm_Cod") 'todo, verificare queste conversioni
                    Case 3  'g
                        DoseTotale_Trasformata = DoseTotale / 1000  'caso in cui ho i grammi
                    Case 2032  'mg
                        DoseTotale_Trasformata = DoseTotale / 1000000  'caso in cui ho i mg
                    Case 4  'q
                        DoseTotale_Trasformata = DoseTotale * 100  'caso in cui ho i quintali
                    Case 304 't
                        DoseTotale_Trasformata = DoseTotale * 1000   'caso in cui ho le tonnelate
                    Case 101 'ml
                        DoseTotale_Trasformata = DoseTotale / 1000  'caso in cui ho i ml
                    Case 104 'cc
                        DoseTotale_Trasformata = DoseTotale / 1000  'caso in cui ho i cc
                    Case 2, 29 'kg,l
                        DoseTotale_Trasformata = DoseTotale
                End Select
                Movimento_Dettaglio.Qta = DoseTotale_Trasformata

                Movimento_Dettaglio.Contabilizzato = NONCONTABILE

                Movimento_Dettaglio.BaseCode = BaseCode
                Movimento_Dettaglio.TopCode = TopCode

                Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Add(Movimento_Dettaglio)

                '------------------------------------------------
                '----- MOVIMENTI DESTINAZIONI
                '------------------------------------------------
                Indice = Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Count - 1

                Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(indice).Movimenti_Destinazioni = New List(Of Movimento_Destinazione)

                Movimento_Destinazione = New Movimento_Destinazione

                Movimento_Destinazione.Id_Agenda = objParametriAgenda.Id_Agenda
                Movimento_Destinazione.Data = Data

                Movimento_Destinazione.Piva = objParametriAgenda.Piva
                Movimento_Destinazione.Sa_Cod = sa_cod_magazzino
                Movimento_Destinazione.Appezza = 0
                Movimento_Destinazione.Id_Destinazione = fabbricatox_Cod
                Movimento_Destinazione.Tipo = MAGAZZINO

                'Movimento_Destinazione.Qta = DoseTotale_Trasformata * QTA2_TOT
                Movimento_Destinazione.Qta = DoseTotale_Trasformata '* QTA2_TOT

                Movimento_Destinazione.BaseCode = BaseCode
                Movimento_Destinazione.TopCode = TopCode

                Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(indice).Movimenti_Destinazioni.Add(Movimento_Destinazione)

            Next

        End If

        Return Agenda
    End Function

    Private Sub SalvaTutto(ByVal sender As Object, ByVal e As System.EventArgs)


        '--------------BLOCCO SALVATAGGIO SENZA MAGAZZINO-------------------
        If Not IsNothing(Session("UTENTE_COD_BLOCCA_SE_SENZA_MAGAZZINO")) AndAlso Session("UTENTE_COD_BLOCCA_SE_SENZA_MAGAZZINO") = True Then
            If objParametriAgenda.Fabbricato = "0" OrElse objParametriAgenda.Fabbricato = "" Then
                Dim MErrore As String
                MErrore = "In base alle impostazioni utente NON è possibile salvare l'operazione senza utilizzare il magazzino!"
                Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.AttenzioneLOperazioneNonÈStataRegistrataBr & MErrore, Page, , _
                    CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
                Exit Sub
            End If

        End If
        '---------------------------------------------------------------------


        '-----------------------------------------------------------------------------------------------------
        '-----------------------TEMPORANERO PER EVITARE SALVATAGGIO MAGAZZINI ESTERNI------------------------
        If objParametriAgenda.Fabbricato <> "0" AndAlso Split(objParametriAgenda.Fabbricato, "|").Count = 3 AndAlso Split(objParametriAgenda.Fabbricato, "|")(2) <> objParametriAgenda.Piva Then
            Dim MErrore As String
            MErrore = Resources.AgronicaAgenda_2010.ATTENZIONEBrAlMomentoNonÈPermessoIlSalvata
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.AttenzioneLOperazioneNonÈStataRegistrataBr & MErrore, Page, , _
                CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            Exit Sub
        End If
        '-----------------------------------------------------------------------------------------------------
        '-----------------------------------------------------------------------------------------------------

        Dim messaggio_errore As String = ""
        Dim TipoSalvataggio As Integer = CType(Ricerca.FindControlIterative(Page.Master, "RBL_Salva"), RadioButtonList).SelectedValue + 1
        If Salva(TipoSalvataggio, messaggio_errore) Then
            fine_salvataggio(TipoSalvataggio)
        Else
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.AttenzioneLOperazioneNonÈStataRegistrataBr & messaggio_errore, Page, , _
                CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
        End If
    End Sub

    Private Function Salva(ByVal TipoSalvataggio As Integer, ByRef messaggio_errore As String) As Boolean
        Dim res As Boolean = False


        '---------------------------------------
        ' recupero il CENTRO
        Dim Sa_Cod As Integer = 0
        If objParametriAgenda.Sa_Cod = "" Then
            'Messaggi.AgroMsgBox("Selezionare un centro aziendale!", Page, , _
            '                   CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            messaggio_errore = Resources.AgronicaAgenda_2010.SelezionareUnCentroAziendale
            Return False
        Else
            Sa_Cod = CInt(objParametriAgenda.Sa_Cod)
        End If


        '---------------------------------------
        ' recupero il MAGAZZINO
        Dim Fabbricato_Cod As String = ""
        Fabbricato_Cod = objParametriAgenda.Fabbricato


        '////////////////////////////////////////////////////////////
        '//////////////////DATI PER L OPERAZIONE///////////////////////
        '////////////////////////////////////////////////////////////

        '---------------------------------------
        ' recupero i semilavorati
        '---------------------------------------

        Dim AlmenoUno As Boolean = False
        For i = 0 To GridViewMagazzino.Rows.Count - 1
            If (CType(GridViewMagazzino.Rows(i).FindControl("ChkSeleziona"), CheckBox).Checked = True) Then
                AlmenoUno = True
                Exit For
            End If
        Next

        If Not AlmenoUno Then
            messaggio_errore = "Selezionare almeno un prodotto in magazzino!"
            'messaggio_errore = Resources.AgronicaAgenda_2010.SelezionareAlmenoUnImpiantoColturale
            Return False
        End If

        'modifico la selezione degli rbl per salvare correttamente i dati dato che salvo sempre e solo la dose/ha
        rbl_DoseHA.Checked = True
        rbl_DoseHL.Checked = False
        'rbl_QtaDose.Checked = True
        'rbl_QtaTot.Checked = False
        'se ho l'agenda metto a totale
        If objParametriAgenda.Fabbricato.Length > 1 Then
            rbl_QtaDose.Checked = False
            rbl_QtaTot.Checked = True
        End If

        'modifico le dosi a ettaro in modo da avare una precisione maggiore
        Dim dt_dosi As DataTable = Session("vs_dtDosi")
        Dim sup_trattata As Decimal = Txt_SupTrattata.Value
        For i = 0 To dt_dosi.Rows.Count - 1
            dt_dosi.Rows(i).Item("Dose") = CDbl(CDbl(dt_dosi.Rows(i).Item("Qta_tot")) / sup_trattata)
        Next

        Try


            '-----------------------------------------------------
            '----------- CONNESSIONE E TRANSAZIONE ---------------
            ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)
            '-----------------------------------------------------

            Select Case objParametriAgenda.Sa_Cod

                '--------------------------------------------------
                '--------------------------------------------------
                '--------- OPERAZIONE MULTI-CENTRO    -------------
                '--------------------------------------------------
                '--------------------------------------------------
                Case "0"

                    'se è multicentro salvo sempre la qta di acqua a ettaro
                    rblAcqua_HA.Checked = True
                    rblAcqua_Tot.Checked = False

                    Dim ListaSacod As New List(Of Integer)
                    Dim Trovato As Boolean
                    For i = 0 To GridViewMagazzino.Rows.Count - 1
                        If (CType(GridViewMagazzino.Rows(i).FindControl("ChkSeleziona"), CheckBox).Checked = True) Then
                            Trovato = False
                            For j = 0 To ListaSacod.Count - 1
                                If CInt(GridViewMagazzino.Rows(i).Cells(7).Text) = ListaSacod(j) Then
                                    Trovato = True
                                    Exit For
                                End If
                            Next
                            If Not Trovato Then
                                ListaSacod.Add(CInt(GridViewMagazzino.Rows(i).Cells(7).Text))
                            End If

                        End If
                    Next


                    'calcolo le qta per singolo Centro
                    Dim listalista As New List(Of List(Of Decimal))
                    'Dim Dt_Dosi As New DataTable

                    If Session("vs_dtDosi") IsNot Nothing Then
                        'Dt_Dosi = Session("vs_dtDosi")
                        Dim tot As Decimal = Txt_SupTrattata.Value
                        For j = 0 To ListaSacod.Count - 1
                            Dim l As New List(Of Decimal)
                            listalista.Add(l)
                        Next

                        For j = 0 To ListaSacod.Count - 1

                            Dim sommatoriaQtaquestoSa_Cod As Decimal = 0

                            For i = 0 To GridViewMagazzino.Rows.Count - 1
                                If (CType(GridViewMagazzino.Rows(i).FindControl("ChkSeleziona"), CheckBox).Checked = True) Then
                                    If CInt(GridViewMagazzino.Rows(i).Cells(7).Text) = ListaSacod(j) Then
                                        sommatoriaQtaquestoSa_Cod += CDbl(CType(GridViewMagazzino.Rows(i).FindControl("TxtQtaGiacenza"), TextBox).Text) * 100
                                    End If
                                End If
                            Next

                            Dim rapporto As Decimal = sommatoriaQtaquestoSa_Cod / tot

                            'controllo se sono all'ultimo giro o no
                            If j = ListaSacod.Count - 1 Then

                                For i = 0 To dt_dosi.Rows.Count - 1
                                    Dim qta As Decimal = dt_dosi.Rows(i).Item("Qta_tot")
                                    Dim jj As Integer = 0
                                    For jj = 0 To listalista.Count - 2
                                        qta = qta - listalista(jj)(i)
                                    Next
                                    listalista(j).Add(qta)
                                Next

                            Else
                                For i = 0 To dt_dosi.Rows.Count - 1
                                    Dim qta As Decimal
                                    qta = dt_dosi.Rows(i).Item("Qta_tot") * rapporto
                                    listalista(j).Add(qta)
                                Next
                            End If
                        Next
                    End If

                    'effettuo verifica arrotondamento
                    For i = 0 To dt_dosi.Rows.Count - 1
                        'per ogni prodotto mi calcolo la somma totale
                        Dim sum_qta As Decimal = 0
                        For j = 0 To listalista.Count - 1
                            sum_qta = sum_qta + listalista(j)(i)
                        Next
                        If sum_qta <> dt_dosi.Rows(i).Item("Qta_tot") Then
                            listalista(listalista.Count - 1)(i) = listalista(listalista.Count - 1)(i) + (dt_dosi.Rows(i).Item("Qta_tot") - sum_qta)
                        End If
                    Next



                    'Inizio il ciclo sui centri Aziendali
                    'Ciclo per ogni centro aziendale
                    For i = 0 To ListaSacod.Count - 1

                        'Dim ListaImpiantixQuestoSaCod As New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)

                        ''Seleziono solamente gli impianti relativi a questo centro aziendale
                        'For j = 0 To ListaImpianti.Count - 1
                        '    If ListaImpianti(j).Sa_Cod = ListaSacod(i) Then
                        '        ListaImpiantixQuestoSaCod.Add(ListaImpianti(j))
                        '    End If
                        'Next

                        '--------------------------------------------------
                        '--------------------------------------------------
                        '--------- CREAZIONE OGGETTO DA SALVARE   ---------
                        '--------------------------------------------------
                        '--------------------------------------------------

                        Dim Agenda As Operazione_Agenda
                        If ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.SelectedValue = "0" Then
                            Agenda = CreaOggettoAgenda_Nessuno(ListaSacod(i), listalista(i))
                        Else
                            Agenda = CreaOggettoAgenda_Normale(ListaSacod(i), listalista(i))
                        End If


                        If IsNothing(Agenda) Then
                            Throw New Exception(Resources.AgronicaAgenda_2010.NonÈStatoPossibileCreareLOperazione)
                        End If

                        ''il controllo lo devo fare solamente se è attivata l'opzione del blocco
                        ''If session("ControllaBlocco") = True And ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.SelectedValue <> "0" Then
                        'If Session("ControllaBlocco") = True And _
                        '    pannelloDisciplinari.Visible = True And _
                        '    ComboDisciplinari1.ddl_Disciplinari.SelectedValue <> "0" Then

                        '    Dim Corretto As Boolean
                        '    Dim Messaggio As String = ""
                        '    Corretto = VerificaDPI(Agenda, Messaggio)

                        '    If Corretto = False Then
                        '        Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.BLOperazioneNonÈConformeBBr & Messaggio, Page, , _
                        '                       CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))

                        '        Throw New Exception(Resources.AgronicaAgenda_2010.BLOperazioneNonÈConformeBBr & Messaggio)
                        '    End If
                        'End If


                        Dim objAgendaScrivi As New Agenda_Operazione_Helper
                        Dim Id_Agenda As Integer = 0

                        If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then

                            'Dal 06/23 conserviamo la data e l'username di creazione originali per tutti i record Agenda
                            allinea_DataUsernameCreazione(Agenda, objParametri_Server, Tipo_Attivita.QuadernoDiCampagna, 0)

                            Dim CancellataOperazione As Boolean = False
                            CancellataOperazione = objAgendaScrivi.Cancella(objParametriAgenda.Piva,
                                                                             objParametriAgenda.Sa_Cod,
                                                                             objParametriAgenda.Id_Agenda, False,
                                                                             objParametri_Server, logCancellazione:=False)
                        End If

                        Id_Agenda = objAgendaScrivi.Scrivi(Agenda, objParametri_Server)

                        'Id_Agenda = objAgendaScrivi.Scrivi(Agenda, objParametri_Server)

                        ''non spostare, l'operazione agenda vecchia in modifica devo eliminarla dopo
                        'Dim util As New Utility_NS.Utility_Operazioni()
                        'util.Gestisci_Magazzino_Aziendale(Agenda, Id_Agenda, objParametriAgenda, objParametri_Server)

                        ''se la scrittura è andata a buon fine e sono in modifica
                        ''CANCELLO la vecchia operazione

                        'If Id_Agenda <> 0 And objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then
                        '    If Id_Agenda_Old > 0 Then
                        '        Dim CancellataOperazione As Boolean = False

                        '        CancellataOperazione = objAgendaScrivi.Cancella(objParametriAgenda.Piva, _
                        '                                                         objParametriAgenda.Sa_Cod, _
                        '                                                         Id_Agenda_Old, False, _
                        '                                                         objParametri_Server)


                        '        'modifico l'aggancio alla ricetta
                        '        Dim objRicetta As New AgronicaCoreContabDAL.RicettexAgenda_W
                        '        Dim ModRicetta As Boolean
                        '        ModRicetta = objRicetta.Modifica_Agenda(0, 0, _
                        '                                                Id_Agenda_Old, _
                        '                                                Id_Agenda, _
                        '                                                AGRODATAINIZIO, _
                        '                                                AGRODATAFINE, _
                        '                                                "", _
                        '                                                objParametri_Server)
                        '    End If
                        'End If

                        'se sono n scrittura devo agganciare la ricetta se presente
                        If Id_Agenda <> 0 AndAlso objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura Then
                            ' se Session("UtilizzataRicetta") esiste e se è true allora ho utilizzato la ricetta per creare l'operazione
                            If Not IsNothing(Session("UtilizzataRicetta")) AndAlso Session("UtilizzataRicetta") Then

                                Dim ricetta_cod As String = Session("ricetta_cod")
                                Dim Ricetta_Operazione_Cod As String = Session("Ricetta_Operazione_Cod")

                                Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                                Dim Impostazione_RicetteXagenda As String = objUtenti.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_RiferimentoRicetteOperazioni, _
                                                                        HttpContext.Current.Session("ASG_objParametri_Utenti"), _
                                                                        1)
                                If Impostazione_RicetteXagenda <> "0" Then
                                    Dim objRicetta As New AgronicaCoreContabDAL.RicettexAgenda_W
                                    If Not objRicetta.Scrivi(ricetta_cod, Ricetta_Operazione_Cod, Id_Agenda, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server) Then
                                        Throw New Exception(Resources.AgronicaAgenda_2010.BNonÈRiuscitoLAggancioDellaRicettaBBr)
                                    End If
                                End If



                            End If
                        End If
                        'objParametriAgenda.Id_Agenda = Id_Agenda
                    Next


                    '--------------------------------------------------
                    '--------------------------------------------------
                    '--------- OPERAZIONE SINGOLO CENTRO    -----------
                    '--------------------------------------------------
                    '--------------------------------------------------
                Case Else


                    '--------------------------------------------------
                    '--------------------------------------------------
                    '--------- CREAZIONE OGGETTO DA SALVARE   ---------
                    '--------------------------------------------------
                    '--------------------------------------------------

                    Dim Qta_da_scaricare As New List(Of Decimal)
                    'la qta da scaricare è esattamente quella indicata nella qta_totale
                    For i = 0 To dt_dosi.Rows.Count - 1
                        Qta_da_scaricare.Add(dt_dosi.Rows(i).Item("Qta_Tot"))
                    Next


                    Dim Agenda As Operazione_Agenda
                    If ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.SelectedValue = "0" Then
                        Agenda = CreaOggettoAgenda_Nessuno(objParametriAgenda.Sa_Cod, Qta_da_scaricare)
                    Else
                        Agenda = CreaOggettoAgenda_Normale(objParametriAgenda.Sa_Cod, Qta_da_scaricare)
                    End If


                    If IsNothing(Agenda) Then
                        Throw New Exception(Resources.AgronicaAgenda_2010.NonÈStatoPossibileCreareLOperazione)
                    End If

                    ''il controllo lo devo fare solamente se è attivata l'opzione del blocco
                    'If Session("ControllaBlocco") = True And ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.SelectedValue <> "0" And _
                    '        ComboDisciplinari1.ddl_Disciplinari.SelectedValue <> "0" Then
                    '    Dim Corretto As Boolean
                    '    Dim Messaggio As String = ""
                    '    Corretto = VerificaDPI(Agenda, Messaggio)
                    '    If Corretto = False Then
                    '        ''Messaggi.AgroMsgBox("<b>L'operazione non è conforme!</b><br>" & Messaggio, Page, , _
                    '        ''               CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
                    '        ''Exit Sub

                    '        Throw New Exception(Resources.AgronicaAgenda_2010.BLOperazioneNonÈConformeBBr & Messaggio)


                    '    End If
                    'End If


                    Dim objAgendaScrivi As New Agenda_Operazione_Helper
                    Dim Id_Agenda As Integer = 0


                    If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then

                        'Dal 06/23 conserviamo la data e l'username di creazione originali per tutti i record Agenda
                        allinea_DataUsernameCreazione(Agenda, objParametri_Server, Tipo_Attivita.QuadernoDiCampagna, 0)

                        Dim CancellataOperazione As Boolean = False
                        CancellataOperazione = objAgendaScrivi.Cancella(objParametriAgenda.Piva,
                                                                         objParametriAgenda.Sa_Cod,
                                                                         objParametriAgenda.Id_Agenda, False,
                                                                         objParametri_Server, logCancellazione:=False)
                    End If

                    Id_Agenda = objAgendaScrivi.Scrivi(Agenda, objParametri_Server)

                    'Id_Agenda = objAgendaScrivi.Scrivi(Agenda, objParametri_Server)

                    ''non spostare, l'operazione agenda vecchia in modifica devo eliminarla dopo
                    'Dim util As New Utility_NS.Utility_Operazioni()
                    'util.Gestisci_Magazzino_Aziendale(Agenda, Id_Agenda, objParametriAgenda, objParametri_Server)

                    ''se la scrittura è andata a buon fine e sono in modifica
                    ''CANCELLO la vecchia operazione

                    'If Id_Agenda <> 0 And objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then
                    '    If Id_Agenda_Old > 0 Then
                    '        Dim CancellataOperazione As Boolean = False

                    '        CancellataOperazione = objAgendaScrivi.Cancella(objParametriAgenda.Piva, _
                    '                                                         objParametriAgenda.Sa_Cod, _
                    '                                                         Id_Agenda_Old, False, _
                    '                                                         objParametri_Server)


                    '        'modifico l'aggancio alla ricetta
                    '        Dim objRicetta As New AgronicaCoreContabDAL.RicettexAgenda_W
                    '        Dim ModRicetta As Boolean
                    '        ModRicetta = objRicetta.Modifica_Agenda(0, 0, _
                    '                                                Id_Agenda_Old, _
                    '                                                Id_Agenda, _
                    '                                                AGRODATAINIZIO, _
                    '                                                AGRODATAFINE, _
                    '                                                "", _
                    '                                                objParametri_Server)
                    '    End If
                    'End If

                    'se sono n scrittura devo agganciare la ricetta se presente
                    If Id_Agenda <> 0 AndAlso objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura Then
                        ' se Session("UtilizzataRicetta") esiste e se è true allora ho utilizzato la ricetta per creare l'operazione
                        If Not IsNothing(Session("UtilizzataRicetta")) AndAlso Session("UtilizzataRicetta") Then

                            Dim ricetta_cod As String = Session("ricetta_cod")
                            Dim Ricetta_Operazione_Cod As String = Session("Ricetta_Operazione_Cod")

                            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                            Dim Impostazione_RicetteXagenda As String = objUtenti.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_RiferimentoRicetteOperazioni, _
                                                                    HttpContext.Current.Session("ASG_objParametri_Utenti"), _
                                                                    1)
                            If Impostazione_RicetteXagenda <> "0" Then
                                Dim objRicetta As New AgronicaCoreContabDAL.RicettexAgenda_W
                                If Not objRicetta.Scrivi(ricetta_cod, Ricetta_Operazione_Cod, Id_Agenda, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server) Then
                                    Throw New Exception(Resources.AgronicaAgenda_2010.BNonÈRiuscitoLAggancioDellaRicettaBBr)
                                End If
                            End If



                        End If
                    End If

                    If CType(Ricerca.FindControlIterative(Page.Master, "RBL_Salva"), RadioButtonList).SelectedValue = 0 Then
                        objParametriAgenda.Id_Agenda = Id_Agenda
                    End If



            End Select

            res = True

            'commit transazione
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)

        Catch ex As Exception

            'commit transazione rollback
            ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)

            messaggio_errore = Resources.AgronicaAgenda_2010.AttenzioneLOperazioneNonÈStataRegistrataBr & ex.Message
            res = False

        Finally

            'chiudi connessione
            ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

        End Try


        Return res


    End Function


    Private Sub fine_salvataggio(ByVal TipoSalvataggio As Integer)

        Session("UtilizzataRicetta") = False
        Select Case TipoSalvataggio
            Case 1

                Dim strJS As New StringBuilder
                strJS.AppendLine("$(document).ready(function () { ")


                Dim link As String = ""
                Dim paginaOnLineRitorno As Integer = objParametriAgenda.PaginaSitoOrigine
                Dim sitoorigine As Enum_SiteRedirector = HttpContext.Current.Session("Sito_Origine")

                If sitoorigine = Enum_SiteRedirector.GiasNG Then
                    AgronicaCoreGestioneRichieste.MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva,
                                                                        Enum_SiteRedirector.GiasNG,
                                                                        objParametriAgenda.PaginaSitoOrigine,
                                                                        link,
                                                                        objParametri_Server,
                                                                        SitoOrigine:=Enum_SiteRedirector.Sito_AgronicaDomandaIrrigua)

                Else

                    If paginaOnLineRitorno = 0 Then
                        link = CType(Master.Master, Agenda).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)

                    Else
                        link = (CType(Master.Master, Agenda).TrovaRedirectCorretto(True, paginaOnLineRitorno, objParametriAgenda))
                    End If

                End If



                strJS.AppendLine("      ChiamataParent_Id_Ageda(" & objParametriAgenda.Id_Agenda & "); ")
                strJS.AppendLine("      window.location = '" & link & "'; ")



                strJS.AppendLine(" });")
                ScriptManager.RegisterStartupScript(CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel), CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).GetType(),
                                              String.Format("jQuery_{0}", CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).ClientID), strJS.ToString, True)

                Messaggi.AgroMsgBuonFine(Resources.AgronicaAgenda_2010.RegistrazioneEffettuataConSuccesso, Page, , _
                                  CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))

                Trattamenti_Disposed()
                objParametriAgenda.Svuota_DatiOperazione()

            Case 2
                Trattamenti_Disposed()
                objParametriAgenda.Impianti = New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)
                objParametriAgenda.Note = New List(Of Nota)
                objParametriAgenda.Movimenti = New List(Of Movimento)

                Dim strJS As New StringBuilder
                strJS.AppendLine("$(document).ready(function () { ")
                strJS.AppendLine("      BloccaSbloccaTotale(); ")
                strJS.AppendLine("      window.location = '../Operazioni/Trattamenti.aspx'; ")
                strJS.AppendLine(" });")
                ScriptManager.RegisterStartupScript(CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel), CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).GetType(),
                                              String.Format("jQuery_{0}", CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).ClientID), strJS.ToString, True)

                Messaggi.AgroMsgBuonFine(Resources.AgronicaAgenda_2010.RegistrazioneEffettuataConSuccesso, Page, , _
                                  CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))

            Case 3
                Dim strJS As New StringBuilder
                strJS.AppendLine("$(document).ready(function () { ")
                strJS.AppendLine("      BloccaSbloccaTotale(); ")
                strJS.AppendLine(" });")
                ScriptManager.RegisterStartupScript(CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel), CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).GetType(),
                                              String.Format("jQuery_{0}", CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).ClientID), strJS.ToString, True)

                Messaggi.AgroMsgBuonFine(Resources.AgronicaAgenda_2010.RegistrazioneEffettuataConSuccesso, Page, , _
                                  CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))


        End Select
    End Sub

    'Private Function VerificaDPI(ByVal Agenda As Operazione_Agenda, ByRef Errore As String) As Boolean

    '    ''una volta creato l'oggetto agenda posso infocare il suo metodo che mi genera l'xml
    '    'Dim Helper As New Agenda_Operazione_Helper
    '    'Dim strXML As String = Helper.GeneraXML_CAU_TRATTAMENTO(Agenda)
    '    'strXML = Replace(strXML, "TipoOperazioneDB=""1""", "TipoOperazioneDB=""" & Agenda.Tipo_Operazione & """")


    '    'Dim stringaXmlDpiVerifica As String
    '    'Dim objDpiVerifica As New AgronicaCoreDpiBIZ.DPI_Verifica
    '    'stringaXmlDpiVerifica = objDpiVerifica.DPI_Verifica_Conformita_Intervento(objParametri_Server, _
    '    '                                                            objParametri_Utenti, _
    '    '                                                            CStr(strXML), _
    '    '                                                            CInt(0), _
    '    '                                                            CStr(""), _
    '    '                                                            CInt(0), _
    '    '                                                            CInt(0), _
    '    '                                                            CInt(0), _
    '    '                                                            CInt(Agenda.Movimenti(0).Num_Protocollo))

    '    'Dim Dt_Difesa As DataTable
    '    Dim Conforme As Boolean
    '    'Dt_Difesa = Crea_Dt_Interventi()

    '    'Inserisci_Riga_Difesa(stringaXmlDpiVerifica, _
    '    '                      Dt_Difesa, _
    '    '                      Conforme)

    '    'Errore = VerificaStringaErrori(stringaXmlDpiVerifica)

    '    Return Conforme


    'End Function

    Private Sub Ripristina_Dati_nei_Controlli()

        'Questa subroutine legge le informazioni dal database
        'e ripristina lo stato dei controlli sulla form.

        '------------------------------------------
        '----- Dichiarazione delle Variabili
        '------------------------------------------

        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim x As Integer = 0

        Dim Mat_Cod As Integer = 0
        Dim Cod_Progetto As Integer = 0
        Dim Cal_Cod As Integer = 0
        Dim Lotto As String = ""
        Dim Fr_Cod As Integer = 0
        Dim Fr_Des As String
        Dim Udm_Cod As Integer = 0
        Dim Udm_Cod_Trasformato As Integer = 0
        Dim Udm_Des As String = ""
        Dim Udm_Sim As String = ""
        Dim Dose As String = ""
        Dim Dose_Totale As Decimal
        Dim Dose_HA, Dose_HL As Decimal
        Dim Carenza As Integer = 0

        Dim PrincipiAttivi As String = ""
        Dim ClassiTossicologiche As String = ""

        Dim Dpi_Cod As Integer = 0
        Dim Disciplinare_PubblicoPrivato As Integer
        Dim Modulo As Integer = 0

        Dim AvCod() As String
        Dim AvGru() As String
        Dim AvCod_2() As String
        Dim AvGru_2() As String
        Dim N_Avv As Integer = 0

        Dim strAvCod As String = ""
        Dim strAvGru As String = ""
        Dim strAvDes As String = ""

        Dim DoseEtichetta As String = ""
        Dim DoseEtichetta_Max As String = ""

        Dim Destinazione As Integer
        Dim SaCodDestinazione As Integer = 0

        Dim Sup_Tot As Decimal = 0
        Dim Sup_Tot_Sel As Decimal = 0
        Dim xQtaAcqua As Decimal = 0
        Dim xGrfi_Cod As Integer = 0
        Dim Grfi_Cod As Integer = 0
        Dim Cop_Cod As Integer = 0
        Dim Copertura As Integer = 0

        Dim Veg_Cod As Integer = 0

        '------------------------------------------
        '----- Recupero le informazioni
        '------------------------------------------

        Dim objAgenda As New Agenda_Operazione_Helper

        Dim Agenda As New Operazione_Agenda

        Agenda = objAgenda.Leggi(objParametriAgenda.Piva, _
                                     CInt(objParametriAgenda.Sa_Cod), _
                                     CInt(objParametriAgenda.Id_Agenda), _
                                     0, _
                                     objParametri_Server)

        objAgenda = Nothing

        If Not IsNothing(Agenda) Then

            objParametriAgenda.Piva = Agenda.Piva
            objParametriAgenda.Sa_Cod = Agenda.Sa_Cod
            objParametriAgenda.Lav_Cod = Agenda.Lav_Cod

            'NOTE
            If Not IsNothing(Agenda.Note) Then
                For i = 0 To Agenda.Note.Count - 1
                    objParametriAgenda.Note.Add(Agenda.Note(i))
                Next
            End If
            objParametriAgenda.salva()

            'MOVIMENTI
            If Not IsNothing(Agenda.Movimenti) Then

                Dim MovimentiCosti As List(Of Movimento) = New List(Of Movimento)
                objParametriAgenda.Impianti = New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)

                For i = 0 To Agenda.Movimenti.Count - 1

                    Select Case Agenda.Movimenti(i).Cau_Mov

                        Case CAU_TRATTAMENTO

                            objParametriAgenda.Data = Agenda.Movimenti(i).Data

                            CType(Page.Master, Operazione).SetNota(Agenda.Movimenti(i).Mov_Desc)

                            Select Case Agenda.Movimenti(i).Mezzo
                                Case 1
                                    'HA
                                    Me.rbl_DoseHA.Checked = True
                                    Me.rbl_DoseHL.Checked = False
                                Case 0
                                    'HL
                                    Me.rbl_DoseHL.Checked = True
                                    Me.rbl_DoseHA.Checked = False
                            End Select

                            Select Case Agenda.Movimenti(i).Modalita
                                Case 10
                                    'Totale
                                    Me.rbl_QtaTot.Checked = True
                                    Me.rbl_QtaDose.Checked = False
                                Case 11
                                    'dose
                                    Me.rbl_QtaDose.Checked = True
                                    Me.rbl_QtaTot.Checked = False
                            End Select



                            '-------------------------------------------
                            '-------------------------------------------
                            'Disciplinari
                            '= 0  --> Nessun Filtro + Casi Vecchi senza DPI
                            '> 0  --> DPI
                            '= -1 --> Nuovo caso con etichetta ma senza DPI
                            '-------------------------------------------
                            '-------------------------------------------
                            Dpi_Cod = Agenda.Movimenti(i).Num_Protocollo
                            Disciplinare_PubblicoPrivato = Agenda.Movimenti(i).Disciplinare_PubblicoPrivato

                            'modulo
                            If Dpi_Cod <> 0 Then
                                If Agenda.Movimenti(i).Extra_Int <> 0 Then
                                    Modulo = Agenda.Movimenti(i).Extra_Int
                                    Session("modulo") = Modulo
                                End If
                            End If




                            'MOVIMENTO_DETTAGLIO TECNICO x ACQUA
                            If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici) Then

                                For j = 0 To Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici.Count - 1

                                    If Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Av_Cod = 0 AndAlso
                                       Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Av_Gru = 0 Then

                                        'Acqua
                                        xQtaAcqua = Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Qta_Ril

                                        'se l'acqua è stata salvata negativa significa che è stata salvata/ha, altrimenti totale
                                        If xQtaAcqua < 0 Then
                                            Me.Txt_Acqua_Ha.Text = Math.Abs(xQtaAcqua)
                                            rblAcqua_HA.Checked = True
                                            rblAcqua_Tot.Checked = False
                                        Else
                                            Me.Txt_Acqua_Tot.Text = Math.Abs(xQtaAcqua)
                                            rblAcqua_Tot.Checked = True
                                            rblAcqua_HA.Checked = False
                                        End If
                                    Else

                                        Select Case Dpi_Cod
                                            Case 0
                                                ReDim Preserve AvCod(N_Avv)
                                                ReDim Preserve AvGru(N_Avv)
                                                AvCod(N_Avv) = Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Av_Cod.ToString
                                                AvGru(N_Avv) = Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Av_Gru.ToString
                                                N_Avv += 1
                                        End Select
                                    End If

                                Next

                            End If


                            Dim grfi_cod_x_etichette As Integer = -99

                            'MOVIMENTI_DETTAGLI
                            If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli) Then

                                For j = 0 To Agenda.Movimenti(i).Movimenti_Dettagli.Count - 1
                                    Select Case Agenda.Movimenti(i).Movimenti_Dettagli(j).Elem_Cod
                                        Case Is <> FORMULATI
                                            Sup_Tot_Sel += Agenda.Movimenti(i).Movimenti_Dettagli(j).Qta / 100
                                    End Select
                                Next

                                Txt_SupSelezionata.Value = Sup_Tot_Sel.ToString
                                Txt_SupTrattata.Value = Sup_Tot_Sel.ToString

                                For j = 0 To Agenda.Movimenti(i).Movimenti_Dettagli.Count - 1

                                    Select Case Agenda.Movimenti(i).Movimenti_Dettagli(j).Elem_Cod

                                        Case FORMULATI

                                            Fr_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Pro_Cod
                                            Fr_Des = ""

                                            Dim objFrDes As New AgronicaCoreMetaSchemaDAL.Formulati_R
                                            Dim DT_Fr As DataTable
                                            DT_Fr = objFrDes.Leggi(Agenda.Movimenti(i).Movimenti_Dettagli(j).Pro_Cod, _
                                                                      AGRODATAINIZIO, _
                                                                      AGRODATAFINE, _
                                                                       enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                                                       "", _
                                                                       "", _
                                                                       objParametri_Server)

                                            If DT_Fr IsNot Nothing AndAlso DT_Fr.Rows.Count > 0 Then
                                                Fr_Des = DT_Fr.Rows(0).Item("Fr_Des")
                                            End If

                                            Udm_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Extra_Int
                                            Udm_Cod_Trasformato = Agenda.Movimenti(i).Movimenti_Dettagli(j).Udm_Cod

                                            Dim objUdm As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
                                            Udm_Des = objUdm.UdmDes_from_UdmCod(Udm_Cod, _
                                                                                Udm_Sim, _
                                                                                objParametri_Server)

                                            If rbl_DoseHL.Checked Then 'todo, hl ha ??
                                                Udm_Des = "[" & Udm_Sim & "/hl]"
                                            Else
                                                Udm_Des = "[" & Udm_Sim & "/q]"
                                            End If

                                            Dose = Agenda.Movimenti(i).Movimenti_Dettagli(j).Qta


                                            If rbl_DoseHA.Checked Then
                                                '/ha
                                                Dose_Totale = Math.Round(Dose * Sup_Tot_Sel, 4)
                                                Dose_HA = Dose
                                                Dim app As Decimal
                                                If xQtaAcqua < 0 Then
                                                    app = (Math.Abs(xQtaAcqua) * Sup_Tot_Sel)
                                                Else
                                                    app = xQtaAcqua
                                                End If
                                                If xQtaAcqua <> 0 Then
                                                    Dose_HL = CDbl(Dose_Totale / app)
                                                Else
                                                    Dose_HL = 0
                                                End If
                                            Else
                                                '/hl o totale
                                                If xQtaAcqua < 0 Then
                                                    Dose_Totale = Math.Round(Dose * (Math.Abs(xQtaAcqua) * Sup_Tot_Sel), 4)
                                                Else
                                                    Dose_Totale = Math.Round(Dose * xQtaAcqua)
                                                End If
                                                If xQtaAcqua <> 0 Then
                                                    Dose_HL = Dose
                                                    Dose_HA = CDbl(Dose_Totale / Sup_Tot_Sel)
                                                Else
                                                    Dose_HL = 0
                                                    Dose_HA = CDbl(Dose_Totale / Sup_Tot_Sel)
                                                End If
                                            End If

                                            DoseEtichetta = Agenda.Movimenti(i).Movimenti_Dettagli(j).DoseEtichetta

                                            'MOVIMENTI_DETTAGLI_TECNICI

                                            strAvCod = ""
                                            strAvGru = ""
                                            strAvDes = ""
                                            Dim Soglia_Value, Soglia_Des As String

                                            If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici) Then

                                                Select Case Dpi_Cod
                                                    Case Is <> 0
                                                        ReDim AvCod_2(Agenda.Movimenti(i).Movimenti_Dettagli.Count - 1)
                                                        ReDim AvGru_2(Agenda.Movimenti(i).Movimenti_Dettagli.Count - 1)
                                                End Select

                                                For x = 0 To Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici.Count - 1

                                                    AvCod_2(j) &= Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(x).Av_Cod.ToString & ","
                                                    AvGru_2(j) &= Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(x).Av_Gru.ToString & ","

                                                    If Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(x).Av_Cod <> 0 Then


                                                        strAvCod &= Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(x).Av_Cod.ToString & ","
                                                        strAvGru &= "0,"

                                                        Dim objAvv As New AgronicaCoreMetaSchemaDAL.Avversita_R
                                                        Dim descrizioneAvv = objAvv.AvDes_from_AvCod(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(x).Av_Cod, _
                                                                                                     Nothing, _
                                                                                                     enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                                                                                                     "", "", _
                                                                                                     objParametri_Server)
                                                        If descrizioneAvv = "" Then
                                                            descrizioneAvv = Resources.AgronicaAgenda_2010.AVVERSITANONRICONOSCIUTASostituirla
                                                        End If
                                                        strAvDes &= descrizioneAvv & ","
                                                        objAvv = Nothing

                                                    Else

                                                        strAvCod &= "0,"
                                                        strAvGru &= Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(x).Av_Gru.ToString & ","

                                                        Dim objAvv As New AgronicaCoreMetaSchemaDAL.GruppoAvversita_R
                                                        Dim descrizioneAvv = objAvv.AvGruDes_from_AvGruCod(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(x).Av_Gru, _
                                                                                                           Nothing, _
                                                                                                        enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                                                                                                     "", "", _
                                                                                                     objParametri_Server)
                                                        If descrizioneAvv = "" Then
                                                            descrizioneAvv = Resources.AgronicaAgenda_2010.GRUPPONONRICONOSCIUTOSostituirlo
                                                        End If
                                                        strAvDes &= descrizioneAvv & ","

                                                        objAvv = Nothing

                                                    End If

                                                    Dim AvCodTmp, AvGruTmp As Integer

                                                    If Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(x).Soglia_Cod = 0 Then
                                                        AvCodTmp = 0
                                                        AvGruTmp = 0

                                                        Soglia_Value &= "0,"
                                                        Soglia_Des &= ","
                                                    Else
                                                        AvCodTmp = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(x).Av_Cod
                                                        AvGruTmp = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(x).Av_Gru

                                                        Soglia_Value &= Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(x).Av_Cod & "_" & _
                                                                        IIf(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(x).Soglia_Quantita <> 0, _
                                                                            Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(x).Soglia_Quantita, _
                                                                            "") & "_" & _
                                                                        Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(x).Soglia_Cod & ","

                                                        Soglia_Des &= Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(x).Soglia_Des & ","
                                                    End If


                                                Next

                                                If Not IsNothing(AvCod_2) Then
                                                    If AvCod_2(j) <> "" Then
                                                        AvCod_2(j) = Left(AvCod_2(j), AvCod_2(j).Length - 1)
                                                    End If
                                                End If
                                                If Not IsNothing(AvGru_2) Then
                                                    If AvGru_2(j) <> "" Then
                                                        AvGru_2(j) = Left(AvGru_2(j), AvGru_2(j).Length - 1)
                                                    End If
                                                End If
                                                If strAvCod <> "" Then
                                                    strAvCod = Left(strAvCod, strAvCod.Length - 1)
                                                End If
                                                If strAvGru <> "" Then
                                                    strAvGru = Left(strAvGru, strAvGru.Length - 1)
                                                End If
                                                If strAvDes <> "" Then
                                                    strAvDes = Left(strAvDes, strAvDes.Length - 1)
                                                End If
                                            End If

                                            Carenza = Agenda.Movimenti(i).Movimenti_Dettagli(j).TempoCarenza
                                            If Carenza = -1 OrElse Carenza = 0 Then
                                                Dim CoreAgroWS As New AgronicaCoreWebService.AgroWs
                                                Carenza = CoreAgroWS.TempoCarenza_from_FrCod_VegCod(Fr_Cod, objParametriAgenda.Veg_Cod.Split("/")(0), 0, 0, 0, objParametriAgenda.Data, objParametri_Server, objParametri_Utenti)
                                                If Carenza = -1 Then
                                                    Carenza = 0
                                                End If
                                            End If

                                            PrincipiAttivi = Agenda.Movimenti(i).Movimenti_Dettagli(j).PrincipiAttivi
                                            If PrincipiAttivi = "" Then
                                                'Carenza = TempoCarenza_from_FrCod_VegCod(Server, Session, Page, Fr_Cod, objParametriAgenda.Veg_Cod.Split("/")(0))
                                                'If Carenza = -1 Then
                                                '    Carenza = 0
                                                'End If
                                            End If
                                            ClassiTossicologiche = Agenda.Movimenti(i).Movimenti_Dettagli(j).CLassiTossicologiche
                                            If ClassiTossicologiche = "" Then
                                                'Carenza = TempoCarenza_from_FrCod_VegCod(Server, Session, Page, Fr_Cod, objParametriAgenda.Veg_Cod.Split("/")(0))
                                                'If Carenza = -1 Then
                                                '    Carenza = 0
                                                'End If
                                            End If

                                            Dim Prima_Data As String
                                            If IsNumeric(Carenza.ToString) Then
                                                Dim data As Date
                                                data = objParametriAgenda.Data.AddDays(1)
                                                Prima_Data = data.AddDays(CInt(Carenza.ToString)).ToShortDateString
                                            Else
                                                Prima_Data = ""
                                            End If

                                            Dim N_Trattamenti_Max, N_Trattamenti_Umd_Cod As Integer
                                            Dim N_Trattamenti_UDM_Sim As String
                                            N_Trattamenti_Max = Session("N_Trattamenti_Max")
                                            N_Trattamenti_Umd_Cod = Session("N_Trattamenti_Umd_Cod")
                                            N_Trattamenti_UDM_Sim = Session("N_Trattamenti_UDM_Sim")

                                            Dim IntervalloTrattamenti_Min, IntervalloTrattamenti_Max As Integer
                                            IntervalloTrattamenti_Min = Session("IntervalloTrattamenti_Min")
                                            IntervalloTrattamenti_Max = Session("IntervalloTrattamenti_Max")

                                            'tolgo la virgola 
                                            If Not IsNothing(Soglia_Value) Then
                                                If Soglia_Value.Length > 0 Then
                                                    Soglia_Value = Left(Soglia_Value, Soglia_Value.Length - 1)
                                                End If
                                                If Soglia_Des.Length > 0 Then
                                                    Soglia_Des = Left(Soglia_Des, Soglia_Des.Length - 1)
                                                End If
                                            Else

                                            End If

                                            Dosi_Inserisci(Fr_Cod, _
                                                            Fr_Des, _
                                                            Dose, _
                                                            Dose_HA, _
                                                            Dose_HL, _
                                                            Dose_Totale, _
                                                            Udm_Cod, _
                                                            Udm_Des, _
                                                            Udm_Cod_Trasformato, _
                                                            Carenza.ToString, _
                                                            Prima_Data, _
                                                            strAvCod, _
                                                            strAvGru, _
                                                            strAvDes, _
                                                            DoseEtichetta, _
                                                            DoseEtichetta_Max, _
                                                            N_Trattamenti_Max, N_Trattamenti_UDM_Sim, N_Trattamenti_Umd_Cod, _
                                                            Soglia_Value, Soglia_Des, _
                                                            IntervalloTrattamenti_Min, IntervalloTrattamenti_Max, PrincipiAttivi, ClassiTossicologiche)

                                            Soglia_Value = ""
                                            Soglia_Des = ""

                                        Case Else

                                            Mat_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Mat_Cod
                                            Cod_Progetto = Agenda.Movimenti(i).Movimenti_Dettagli(j).Cod_Progetto
                                            Cal_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Cal_Cod
                                            Lotto = Agenda.Movimenti(i).Movimenti_Dettagli(j).Lotto

                                            If Veg_Cod = 0 Then
                                                Dim objMateria As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
                                                objMateria.VegCod_CulCod_from_MatCod(objParametriAgenda.Piva, Agenda.Movimenti(i).Movimenti_Dettagli(j).Elem_Cod, Mat_Cod, Veg_Cod, 0, objParametri_Server)
                                                objParametriAgenda.Veg_Cod = Veg_Cod
                                                Aggiorna_Centro_Specie()
                                            End If

                                            For l = 0 To GridViewMagazzino.Rows.Count - 1
                                                If CInt(GridViewMagazzino.Rows(l).Cells(2).Text) = Mat_Cod AndAlso
                                                   CInt(GridViewMagazzino.Rows(l).Cells(3).Text) = Cod_Progetto AndAlso
                                                   CInt(GridViewMagazzino.Rows(l).Cells(4).Text) = Cal_Cod AndAlso
                                                   GridViewMagazzino.Rows(l).Cells(12).Text = Lotto Then
                                                    CType(GridViewMagazzino.Rows(l).FindControl("ChkSeleziona"), CheckBox).Checked = True
                                                    CType(GridViewMagazzino.Rows(l).FindControl("TxtQtaGiacenza"), TextBox).Text = Agenda.Movimenti(i).Movimenti_Dettagli(j).Qta / 100
                                                End If
                                            Next

                                    End Select

                                Next

                                Txt_SupSelezionata.Value = Sup_Tot.ToString

                            End If


                            CaricaComboDisciplinariExteso()

                            '------------------------------
                            'Recupero il DPI
                            If Dpi_Cod > 0 Then

                                Dim Flag_Protetto As Integer = -1
                                Dim Flag_PubblicoPrivato As Integer = 0
                                Dim Disciplinare_Valore As String = ""

                                If Cop_Cod <> 0 AndAlso
                                   Cop_Cod <> 3 AndAlso
                                   Cop_Cod <> 4 AndAlso
                                   Cop_Cod <> 5 AndAlso
                                   Cop_Cod <> 6 Then
                                    Copertura = 1
                                End If

                                'cerco il DPI giusto x finalita e copertura
                                Dim Array() As String
                                Dim Disciplinare_Cod As Integer


                                For c = 1 To ComboDisciplinari1.ddl_Disciplinari.Items.Count - 1
                                    Array = Split(ComboDisciplinari1.ddl_Disciplinari.Items(c).Value, "/")
                                    Disciplinare_Cod = Array(0)
                                    'verifico se il dpi dell'operazione è lo stesso..
                                    If Dpi_Cod = Disciplinare_Cod Then
                                        Grfi_Cod = Array(2)
                                        Flag_Protetto = Array(3)
                                        Flag_PubblicoPrivato = Array(4)
                                        'verifico se la finalità è = 0 (= tutte x il DPI)
                                        If Grfi_Cod = 0 Then
                                            If Copertura = 1 AndAlso Flag_Protetto = 1 Then
                                                Disciplinare_Valore = ComboDisciplinari1.ddl_Disciplinari.Items(c).Value
                                                Exit For
                                            ElseIf (Copertura = 0 AndAlso Flag_Protetto = 0) OrElse _
                                            (Copertura = 0 AndAlso Flag_Protetto = -1) OrElse _
                                            (Copertura = 0 AndAlso Flag_Protetto = 2) Then
                                                Disciplinare_Valore = ComboDisciplinari1.ddl_Disciplinari.Items(c).Value
                                                Exit For
                                            End If
                                        Else
                                            'caso in cui nel dpi è indicata la finalità..
                                            'verifico se è uguale a quella dell'impianto
                                            If Grfi_Cod = xGrfi_Cod Then
                                                If Copertura = 1 AndAlso Flag_Protetto = 1 Then
                                                    Disciplinare_Valore = ComboDisciplinari1.ddl_Disciplinari.Items(c).Value
                                                    Exit For
                                                ElseIf (Copertura = 0 AndAlso Flag_Protetto = 0) OrElse _
                                                (Copertura = 0 AndAlso Flag_Protetto = -1) OrElse _
                                                (Copertura = 0 AndAlso Flag_Protetto = 2) Then
                                                    Disciplinare_Valore = ComboDisciplinari1.ddl_Disciplinari.Items(c).Value
                                                    Exit For
                                                End If
                                            End If
                                        End If
                                    End If
                                Next

                                'Imposto la selezione della combobox
                                objParametriAgenda.Disciplinare = Disciplinare_Valore

                                ComboDisciplinari1.ddl_Disciplinari.SelectedIndex = _
                                    ComboDisciplinari1.ddl_Disciplinari.Items.IndexOf(ComboDisciplinari1.ddl_Disciplinari.Items.FindByValue( _
                                        Disciplinare_Valore))


                            Else
                                'BIO
                                If Dpi_Cod = -2 Then
                                    ComboDisciplinari1.ddl_Disciplinari.SelectedIndex = _
                                        ComboDisciplinari1.ddl_Disciplinari.Items.IndexOf(ComboDisciplinari1.ddl_Disciplinari.Items.FindByValue( _
                                            Dpi_Cod))
                                    objParametriAgenda.Disciplinare = Dpi_Cod
                                End If
                            End If

                            Aggiorna_Dpi()


                            '------------------------------


                            'se sono nel caso del nessuno nessuno
                            If Dpi_Cod = 0 Then
                                'seleziono il filtor nessuno
                                ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.SelectedIndex = 2
                                'carico tutte le avversità e gruppi avversità
                                CaricaGriglia_Avversita()

                                'seleziono le avversità
                                If N_Avv > 0 Then
                                    Dim jj As Integer = 0
                                    Dim kk As Integer = 0

                                    If AvCod(0) > 0 Then
                                        'ho avversità singole
                                        For jj = 0 To N_Avv - 1

                                            For kk = 0 To GridViewAvversita.Rows.Count - 1
                                                If GridViewAvversita.Rows(kk).Cells(1).Text = AvCod(jj) Then
                                                    CType(GridViewAvversita.Rows(kk).FindControl("ChkSelezionaAvversita"), CheckBox).Checked = True
                                                    Exit For
                                                End If
                                            Next
                                        Next
                                    Else
                                        For jj = 0 To N_Avv - 1
                                            For kk = 0 To GridViewAvversita.Rows.Count - 1
                                                If GridViewAvversita.Rows(kk).Cells(2).Text = AvGru(jj) Then
                                                    CType(GridViewAvversita.Rows(kk).FindControl("ChkSelezionaAvversita"), CheckBox).Checked = True
                                                    Exit For
                                                End If
                                            Next
                                        Next
                                    End If

                                End If
                                MostraNascondiColonne_NessunoNessuno()
                            End If



                        Case CAU_SCARICO

                            'MOVIMENTI_DETTAGLI
                            If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli) Then

                                For j = 0 To Agenda.Movimenti(i).Movimenti_Dettagli.Count - 1

                                    Select Case Agenda.Movimenti(i).Movimenti_Dettagli(j).Elem_Cod
                                        Case FERTILIZZANTI
                                            If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni) Then
                                                For x = 0 To Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count - 1
                                                    Destinazione = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione
                                                    SaCodDestinazione = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod
                                                    objParametriAgenda.Fabbricato = Destinazione.ToString & "|" & SaCodDestinazione.ToString & "|" & Agenda.Piva
                                                Next
                                            End If


                                        Case FORMULATI
                                            If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni) Then
                                                For x = 0 To Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count - 1
                                                    Destinazione = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione
                                                    SaCodDestinazione = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod
                                                    objParametriAgenda.Fabbricato = Destinazione.ToString & "|" & SaCodDestinazione.ToString & "|" & Agenda.Piva
                                                Next
                                            End If


                                        Case Else
                                            '----COSTO ACCESSORIO---------------------
                                            'è un movimento dovuto ad un costo accessorio
                                            'Throw New NotImplementedException
                                            MovimentiCosti.Add(Agenda.Movimenti(i))
                                            Exit For


                                    End Select

                                Next

                            End If



                        Case CAU_IMPUTAZIONE_PARCOMACCHINE
                            MovimentiCosti.Add(Agenda.Movimenti(i))

                        Case CAU_IMPUTAZIONE_MANODOPERA
                            MovimentiCosti.Add(Agenda.Movimenti(i))

                        Case CAU_IMPUTAZIONE_TERZISTI
                            MovimentiCosti.Add(Agenda.Movimenti(i))

                        Case CAU_IMPUTAZIONE_UTILIZZO_PRODOTTI
                            MovimentiCosti.Add(Agenda.Movimenti(i))

                        Case CAU_IMPUTAZIONE_TECNICO_RESPONSABILE
                            MovimentiCosti.Add(Agenda.Movimenti(i))

                        Case Else
                            Throw New NotImplementedException

                    End Select
                Next

                objParametriAgenda.Movimenti = MovimentiCosti

                'gestisco la selezione del fabbricato dell'azienda esterna se l'operazione era stata registrata con quella
                Dim util As New Utility_NS.Utility_Operazioni()
                util.ImpostaFabbricatoDelMagazzinoEsternoSePresente(Agenda, objParametriAgenda, objParametri_Server)

            End If


        End If

        'aggiorno il Datagrid delle dosi
        Griglia_Dosi_Data_Bind()


        Dim script As New StringBuilder


        script.AppendLine("$(document).ready(function () { ")
        script.AppendLine("     BloccaSbloccaTotale();")
        If ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.SelectedValue <> "0" Then
            script.AppendLine("     PulisciGrigliaAv_GrAv();")
        End If
        script.AppendLine("}); ")


        ScriptManager.RegisterStartupScript(UpdatePanelMiscela, UpdatePanelMiscela.GetType(),
                                         String.Format("jQuery_{0}", UpdatePanelMiscela.ClientID), script.ToString, True)

    End Sub


#Region "GESTIONE DOSI"
    ''' <summary>
    ''' DOSI
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CaricaGriglia_Dosi()
        '----- Definizione delle variabili

        Dim Dt As New DataTable
        '----- Definisco la struttura del DataTable
        Dt.Columns.Add(New DataColumn("Av_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Av_Gru", GetType(String)))
        Dt.Columns.Add(New DataColumn("Av_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Fr_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Fr_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Udm_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Udm_Cod_Trasformato", GetType(Integer))) 'l o Kg x scarico magazzino
        Dt.Columns.Add(New DataColumn("Udm_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dose", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Dose_HA", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Dose_HL", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Qta_Tot", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Carenza", GetType(String)))
        Dt.Columns.Add(New DataColumn("Prima_Raccolta", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dose_Max", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Dose_Etichetta", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dose_Etichetta_Max", GetType(String)))
        Dt.Columns.Add(New DataColumn("Limite_Numero_Trattamenti", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Limite_Numero_Trattamenti_UDM_SIM", GetType(String)))
        Dt.Columns.Add(New DataColumn("Limite_Numero_Trattamenti_UDM_COD", GetType(Integer)))

        'aggiungo le tre colonne per la gestione delle soglie
        Dt.Columns.Add(New DataColumn("Soglia_Value", GetType(String)))
        Dt.Columns.Add(New DataColumn("Soglia_Des", GetType(String)))

        Dt.Columns.Add(New DataColumn("IntervalloTrattamenti_Min", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("IntervalloTrattamenti_Max", GetType(Integer)))

        Dt.Columns.Add(New DataColumn("strPA_COD", GetType(String)))
        Dt.Columns.Add(New DataColumn("strCLTOSS_COD", GetType(String)))


        '----- Definisco l'insieme di colonne che costituiscono la chiave della tabella
        'Vettore di DataColumn
        Dim DtKeys(0) As DataColumn
        'Valorizzo le celle del vettore
        DtKeys(0) = Dt.Columns("Fr_Cod")
        'Assegno il vettore delle chiavi al DataTable
        Dt.PrimaryKey = DtKeys
        '----- Salvo il DataTable dentro il session
        session("vs_dtDosi") = Dt
        Griglia_Dosi_Data_Bind()

    End Sub


    ''' <summary>
    ''' Bind Delle Dosi
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub Griglia_Dosi_Data_Bind()
        GridView_Dosi.DataSource = session("vs_dtDosi")
        GridView_Dosi.DataBind()
    End Sub

    ''' <summary>
    ''' inserisce una nuova riga all'interno del dt delle dosi mantenuto in sessione
    ''' </summary>
    ''' <param name="Fr_Cod"></param>
    ''' <param name="Fr_Des"></param>
    ''' <param name="Dose"></param>
    ''' <param name="Dose_HA"></param>
    ''' <param name="Dose_HL"></param>
    ''' <param name="Qta_Tot"></param>
    ''' <param name="Udm_Cod"></param>
    ''' <param name="Udm_Des"></param>
    ''' <param name="Udm_Cod_Trasformato"></param>
    ''' <param name="Carenza"></param>
    ''' <param name="Prima_Raccolta"></param>
    ''' <param name="Av_Cod"></param>
    ''' <param name="Av_Gru"></param>
    ''' <param name="Av_Des"></param>
    ''' <param name="Dose_Etichetta"></param>
    ''' <param name="Dose_Etichetta_Max"></param>
    ''' <param name="Limite_Numero_Trattamenti"></param>
    ''' <param name="Limite_Numero_Trattamenti_UDM_SIM"></param>
    ''' <param name="Limite_Numero_Trattamenti_UDM_COD"></param>
    ''' <remarks></remarks>
    Private Sub Dosi_Inserisci(ByVal Fr_Cod As String, _
                           ByVal Fr_Des As String, _
                           ByVal Dose As Decimal, _
                           ByVal Dose_HA As Decimal, _
                           ByVal Dose_HL As Decimal, _
                           ByVal Qta_Tot As Decimal, _
                           ByVal Udm_Cod As Integer, _
                           ByVal Udm_Des As String, _
                           ByVal Udm_Cod_Trasformato As Integer, _
                           ByVal Carenza As String, _
                           ByVal Prima_Raccolta As String, _
                           ByVal Av_Cod As String, _
                           ByVal Av_Gru As String, _
                           ByVal Av_Des As String, _
                           ByVal Dose_Etichetta As String, _
                           ByVal Dose_Etichetta_Max As String, _
                           ByVal Limite_Numero_Trattamenti As Integer, _
                           ByVal Limite_Numero_Trattamenti_UDM_SIM As String, _
                           ByVal Limite_Numero_Trattamenti_UDM_COD As Integer, _
                           ByVal Soglia_Value As String, _
                           ByVal Soglia_Des As String, _
                           ByVal IntervalloTrattamenti_Min As Integer, _
                           ByVal IntervalloTrattamenti_Max As Integer, _
                           ByVal strPA_COD As String, _
                           ByVal strCLTOSS_COD As String)

        '----- Dimensiono le variabili

        Dim Dt As DataTable
        Dim Dr As DataRow
        Dim IndiceRiga As Integer = 0
        Dim ElementoPresente As Boolean
        Dim Messaggio As String

        '----- Recupero i dati 

        '   Li ho gia' tutti ...
        '   Non ho la necessita' di recuperare nulla
        '   Lascio questa nota per usi futuri di Copia e Incolla

        '----- Verifico che il formulato non sia gia' presente nel datatable

        'Inizializzo
        ElementoPresente = False

        'Recupero il datatable
        Dt = Session("vs_dtDosi")

        'Ciclo nelle righe del datatable
        For IndiceRiga = 0 To Dt.Rows.Count - 1
            If Dt.Rows(IndiceRiga).Item("Fr_Cod") = Fr_Cod Then
                ElementoPresente = True
                Messaggio = Resources.AgronicaAgenda_2010.NonEConsentitoInserireUnFormulatoGiaPresen
                Messaggi.AgroMsgBox(Messaggio, Page, , updateDoseInserisci)
                Exit Sub
            End If
        Next


        '----- Inserisco il nuovo record

        'Creo una nuova riga
        Dr = Dt.NewRow

        'Definisco i valori
        Dr.Item("Av_Cod") = Av_Cod
        Dr.Item("Av_Gru") = Av_Gru
        Dr.Item("Av_Des") = Av_Des
        Dr.Item("Fr_Cod") = Fr_Cod
        Dr.Item("Fr_Des") = Fr_Des

        Dr.Item("Carenza") = Carenza
        Dr.Item("Prima_Raccolta") = Prima_Raccolta

        Dr.Item("Dose_Etichetta") = Dose_Etichetta
        Dr.Item("Dose_Etichetta_Max") = Dose_Etichetta_Max

        Dr.Item("Udm_Cod") = Udm_Cod
        Dr.Item("Udm_Des") = Udm_Des
        Dr.Item("Udm_Cod_Trasformato") = Udm_Cod_Trasformato

        Dr.Item("Dose") = Dose

        Dr.Item("Dose_HA") = Math.Round(Dose_HA, 4)
        Dr.Item("Dose_HL") = Math.Round(Dose_HL, 4)
        Dr.Item("Qta_Tot") = Qta_Tot

        Dr.Item("Limite_Numero_Trattamenti") = Limite_Numero_Trattamenti
        Dr.Item("Limite_Numero_Trattamenti_UDM_SIM") = Limite_Numero_Trattamenti_UDM_SIM
        Dr.Item("Limite_Numero_Trattamenti_UDM_COD") = Limite_Numero_Trattamenti_UDM_COD


        Dr.Item("Soglia_Value") = Soglia_Value
        Dr.Item("Soglia_Des") = Soglia_Des

        Dr.Item("IntervalloTrattamenti_Min") = IntervalloTrattamenti_Min
        Dr.Item("IntervalloTrattamenti_Max") = IntervalloTrattamenti_Max

        Dr.Item("strPA_COD") = strPA_COD
        Dr.Item("strCLTOSS_COD") = strCLTOSS_COD

        Dt.Rows.Add(Dr)

        '----- Salvo il DataTable dentro il session

        Session("vs_dtDosi") = Dt

        '----- Azzero i controlli di provenienza dei dati


        Me.Cmb_UdM.SelectedIndex = -1

        Me.Txt_Formulati.Text = ""
        Me.Lbl_Dose_Etichetta.Text = ""
        Me.Lbl_Dose_Etichetta.Visible = False
        Me.Lbl_Dose_Consigliata.Text = ""
        Lbl_Dose_Consigliata.Visible = False
        lbl_qta_residua.Visible = False
        Me.Txt_Dose_HA.Text = ""
        Me.Txt_Dose_HL.Text = ""
        Me.Txt_DoseTot_HA.Text = ""
        Me.Lbl_Giacenza.Text = ""
        Me.Lbl_Num_Formulati.InnerText = ""

        'Me.ImgBtn_Info.Visible = False

    End Sub





    ''' <summary>
    ''' Cancellazione / Modifica di una dose
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub GridView_Dosi_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles GridView_Dosi.RowCommand

        Dim IndiceRigaGriglia As Integer = 0
        Dim FrCod As Integer = 0
        Dim AvCod As String
        Dim AvGru As String
        Dim AvDes As String

        Dim Dt As DataTable
        Dim Dr As DataRow

        Dim Dose As Decimal = 0
        Dim DoseTotale As Decimal = 0
        Dim UdmCod As Integer = 0
        Dim DoseEtichetta As String = ""

        Dim TipoRichiesto As Integer = 0

        'Recupero l'indice di riga del datagrid
        IndiceRigaGriglia = Convert.ToInt32(e.CommandArgument)

        'Recupero il datatable
        Dim Dt_Dosi As DataTable

        Dt_Dosi = session("vs_dtDosi")

        If Dt_Dosi IsNot Nothing Then

            FrCod = Dt_Dosi.Rows(IndiceRigaGriglia).Item("Fr_Cod")
            UdmCod = Dt_Dosi.Rows(IndiceRigaGriglia).Item("Udm_Cod")
            Dose = Dt_Dosi.Rows(IndiceRigaGriglia).Item("Dose")
            DoseTotale = Dt_Dosi.Rows(IndiceRigaGriglia).Item("Qta_Tot")

            AvCod = Dt_Dosi.Rows(IndiceRigaGriglia).Item("av_Cod")
            AvGru = Dt_Dosi.Rows(IndiceRigaGriglia).Item("av_gru")
            AvDes = Dt_Dosi.Rows(IndiceRigaGriglia).Item("av_des")

            DoseEtichetta = Dt_Dosi.Rows(IndiceRigaGriglia).Item("Dose_Etichetta")

            If ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.SelectedValue <> "0" Then
                For j = 0 To Me.GridViewAvversita.Rows.Count - 1
                    CType(GridViewAvversita.Rows(j).FindControl("ChkSelezionaAvversita"), CheckBox).Checked = False
                Next
            End If

            Select Case e.CommandName

                Case "Modifica"

                    Resettasession_Etichetta()

                    ComboFormulati.TipoRichiesto = TipoRichiesto

                    CopiaQuantita = False
                    ComboFormulati.Veg_Cod = objParametriAgenda.Veg_Cod.Split("/")(0)
                    ComboFormulati.Disciplinare_Cod = objParametriAgenda.Disciplinare
                    If ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.SelectedValue = "0" Then
                        ComboFormulati.FiltroRicerca = 0
                        ComboFormulati.FiltroAggiuntivo = " Formulati.Fr_Cod=" & FrCod.ToString
                    Else

                        ComboFormulati.FiltroRicerca = 1
                        ComboFormulati.FiltroAggiuntivo = " AND Formulati.Fr_Cod=" & FrCod.ToString
                    End If

                    ComboFormulati.PrimaRiga_Flag = False
                    ComboFormulati.FrCod = FrCod

                    ComboFormulati.Flag_PrincipiAttivi = True

                    ComboFormulati.Piva = objParametriAgenda.Piva
                    ComboFormulati.Fabbricato_Cod = objParametriAgenda.Fabbricato
                    ComboFormulati.Cau_Mov = CAU_SCARICO
                    ComboFormulati.Validita_Fine = objParametriAgenda.Data
                    ComboFormulati.Validita_Inizio = objParametriAgenda.Data

                    ComboFormulati.WS_Disciplinari_AgroWS_Disciplinari = objParametriAgenda.WS_Disciplinari_AgroWS_Disciplinari
                    ComboFormulati.WS_Fitofarmaci_AgroWS_Fitofarmaci = objParametriAgenda.WS_Fitofarmaci_AgroWS_Fitofarmaci

                    Select Case objParametriAgenda.Lav_Cod
                        Case LAVCOD_DISTRIBUZIONE_INSETTI, LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_TRATTAMENTO_FITOREGOLATORE, LAVCOD_CONCIA_SEME, LAVCOD_GEODISINFESTAZIONE
                            ComboFormulati.Opt_Avversita_Infestanti = 0
                            ComboFormulati.Tipo_Testata = 0
                        Case LAVCOD_DISERBO
                            ComboFormulati.Opt_Avversita_Infestanti = 1
                            ComboFormulati.Tipo_Testata = 1
                        Case LAVCOD_DISSECCAMENTO
                            ComboFormulati.Opt_Avversita_Infestanti = 0
                            ComboFormulati.Tipo_Testata = 0
                    End Select
                    ComboFormulati.Flag_ClasseTossicologica = True


                    ComboFormulati.CaricaComboFormulati()

                    Lbl_FormulatoInRevisione.InnerText = ""

                    Dim ArrayTmp() As String
                    Dim InRevisione As String = ""
                    Dim DataAttoNormativo As String = ""

                    'ATTENZIONE!!! Il prodotto selezionato è in fase di revisione da Decreto del ... ! Si consiglia di verificare i dosaggi della nuova etichetta su Profitosan!</label>

                    If ComboFormulati.Valore_Combo <> "" Then

                        ArrayTmp = Split(ComboFormulati.Valore_Combo, "£")

                        DataAttoNormativo = ""
                        InRevisione = ""

                        If ArrayTmp.Length > 1 Then
                            If ArrayTmp(1) <> "" Then
                                InRevisione = ArrayTmp(1)
                            End If
                        End If

                        If ArrayTmp.Length > 2 Then
                            If ArrayTmp(2) <> "" Then
                                DataAttoNormativo = ArrayTmp(2)
                            End If
                        End If
                        If InRevisione <> "" Then
                            If DataAttoNormativo <> "" Then
                                Lbl_FormulatoInRevisione.InnerText = "ATTENZIONE!!! Il prodotto selezionato è in fase di revisione da Decreto del " & DataAttoNormativo & "!Si consiglia di riferirsi ai dosaggi della nuova etichetta consultando Profitosan!"
                            Else
                                Lbl_FormulatoInRevisione.InnerText = "ATTENZIONE!!! Il prodotto selezionato è in fase di revisione!Si consiglia di riferirsi ai dosaggi della nuova etichetta consultando Profitosan!"
                            End If
                        End If

                    End If

                    'ricarico solamente se ho il filtro diverso da nessuno nessuno
                    If ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.SelectedValue <> "0" Then

                        If TipoRichiesto <> enum_TipoFormulato.Coadiuvanti AndAlso TipoRichiesto <> enum_TipoFormulato.Corroboranti_Fisiofarmaci Then

                            Dim DtAvv As New DataTable
                            Dim DrAvv As DataRow
                            '----- Definisco la struttura dei DataTable

                            DtAvv.Columns.Add(New DataColumn("Av_Des", GetType(String)))
                            DtAvv.Columns.Add(New DataColumn("Av_Cod", GetType(Integer)))
                            DtAvv.Columns.Add(New DataColumn("Av_Gru", GetType(Integer)))
                            DtAvv.Columns.Add(New DataColumn("Avversita_Infestanti", GetType(Integer)))

                            'Vettore di DataColumn
                            Dim DtKeys(1) As DataColumn

                            DtKeys(0) = DtAvv.Columns("Av_Cod")
                            DtKeys(1) = DtAvv.Columns("Av_Gru")

                            DtAvv.PrimaryKey = DtKeys

                            'ricrica griglia avversità
                            'CaricaGriglia_Avversita(FrCod)

                            '--------------------------------------------------------
                            'controllo se ho selezionato almeno una Avversità o un Gruppo
                            Dim Array_AvCod_Temp() As String
                            Dim Array_AvGru_Temp() As String
                            Dim Array_AvDes_Temp() As String

                            Array_AvCod_Temp = Split(AvCod, ",")
                            Array_AvGru_Temp = Split(AvGru, ",")
                            Array_AvDes_Temp = Split(AvDes, ",")

                            Dim DtInf As DataTable
                            Dim DtInfGru As DataTable
                            Dim DrInf() As DataRow
                            Dim DrInfGru() As DataRow

                            Dim objInf As New AgronicaCoreMetaSchemaDAL.InfestantiAttive_R
                            DtInf = objInf.Leggi(0, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
                            Dim objInfGru As New AgronicaCoreMetaSchemaDAL.GruppoAvversitaAttive_R
                            DtInfGru = objInfGru.Leggi(0, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

                            If Array_AvCod_Temp IsNot Nothing Then
                                For i = 0 To Array_AvCod_Temp.Length - 1
                                    If Array_AvCod_Temp(i) <> "0" Then
                                        DrAvv = DtAvv.NewRow
                                        DrAvv.Item("Av_Des") = Array_AvDes_Temp(i)
                                        DrAvv.Item("Av_Cod") = Array_AvCod_Temp(i)
                                        DrAvv.Item("Av_Gru") = 0
                                        DrInf = DtInf.Select("Av_Cod=" & Array_AvCod_Temp(i))
                                        If DrInf IsNot Nothing AndAlso DrInf.Length > 0 Then
                                            DrAvv.Item("Avversita_Infestanti") = 1
                                        Else
                                            DrAvv.Item("Avversita_Infestanti") = 0
                                        End If
                                        DtAvv.Rows.Add(DrAvv)
                                    End If
                                Next
                            End If
                            If Array_AvGru_Temp IsNot Nothing Then
                                For i = 0 To Array_AvGru_Temp.Length - 1
                                    If Array_AvGru_Temp(i) <> "0" Then
                                        DrAvv = DtAvv.NewRow
                                        DrAvv.Item("Av_Des") = Array_AvDes_Temp(i)
                                        DrAvv.Item("Av_Cod") = 0
                                        DrAvv.Item("Av_Gru") = Array_AvGru_Temp(i)
                                        DrInfGru = DtInfGru.Select("Av_Gru=" & Array_AvGru_Temp(i))
                                        If DrInfGru IsNot Nothing AndAlso DrInfGru.Length > 0 Then
                                            DrAvv.Item("Avversita_Infestanti") = 1
                                        Else
                                            DrAvv.Item("Avversita_Infestanti") = 0
                                        End If
                                        DtAvv.Rows.Add(DrAvv)
                                    End If
                                Next
                            End If

                            GridViewAvversita.DataSource = DtAvv
                            GridViewAvversita.DataBind()

                            For i = 0 To GridViewAvversita.Rows.Count - 1
                                CType(GridViewAvversita.Rows(i).FindControl("ChkSelezionaAvversita"), CheckBox).Checked = True
                            Next

                        End If

                        If FrCod = 0 Then
                            ' Me.ImgBtn_Info.Visible = False
                        Else
                            Me.ImgBtn_Info.Visible = True
                        End If

                    End If


                    'seleziono l'unita di misura corretta
                    Cmb_UdM.Items.Clear()
                    Select Case UdmCod
                        'peso
                        Case 3, 2032, 2, 304, 4
                            CaricaKG(UdmCod)

                            'acqua
                        Case 104, 101, 29
                            CaricaLitri(UdmCod)

                    End Select


                    '----RICALCOLA---- e arrotonda
                    Me.Txt_Dose_HA.Text = Math.Round(Dose, 4)
                    Me.Txt_DoseTot_HA.Text = Math.Round(DoseTotale, 4)
                    If Txt_Acqua_Tot.Text = 0 Then
                        Txt_Dose_HL.Text = 0
                    Else
                        Txt_Dose_HL.Text = Math.Round((Txt_DoseTot_HA.Text / Txt_Acqua_Tot.Text), 4)
                    End If
                    '----RICALCOLA---- 

                    Imposta_DosiEtichetta(DoseEtichetta)
                    Lbl_Dose_Etichetta.Text = DoseEtichetta
                    Session("DoseEtichetta") = Lbl_Dose_Etichetta.Text
                    Lbl_Dose_Etichetta.Visible = True

                    '---------------------------------------------
                    ' ELIMINO LA RIGA DAL DT
                    Dt = Session("vs_dtDosi")
                    'Trovo la riga da cancellare    (chiave = FrCod)
                    Dr = Dt.Rows.Find(FrCod)
                    Dr.Delete()

                    Session("vs_dtDosi") = Dt

                    Griglia_Dosi_Data_Bind()

                    '---------------------------------------------

                    'Verifico se ho cancellato anche l'ultimo formulato 
                    If Dt.Rows.Count < 1 Then

                        'riattivo la modifica del magazzino
                        'Me.OptMagazzinoAziendale.Enabled = True
                        'Me.OptMagazzinoGIAS.Enabled = True
                        'Me.Cmb_Magazzino.Enabled = True
                        'aggiungo il filtro nessuno nessuno se posso
                        If ComboDisciplinari1.ddl_Disciplinari.SelectedValue = "0" AndAlso ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.SelectedValue <> "0" Then
                            ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Nessuno, "0"))
                        End If

                    End If



                Case "Cancella"

                    '---------------------------------------------
                    ' ELIMINO LA RIGA DAL DT
                    Dt = Session("vs_dtDosi")

                    'Trovo la riga da cancellare    (chiave = FrCod)
                    Dr = Dt.Rows.Find(FrCod)

                    Dr.Delete()

                    'GridView_Dosi.DataSource = Dt
                    'GridView_Dosi.DataBind()

                    Session("vs_dtDosi") = Dt

                    Griglia_Dosi_Data_Bind()

                    '---------------------------------------------
                    If Dt.Rows.Count < 1 Then
                        If ComboDisciplinari1.ddl_Disciplinari.SelectedValue = "0" Then
                            ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Nessuno, "0"))
                        End If
                    End If
            End Select

        End If



        Dim STR_UpdatePanelDose As New StringBuilder
        STR_UpdatePanelDose.AppendLine("BloccaSbloccaTotale();")
        ScriptManager.RegisterStartupScript(UpdatePanelMiscela, UpdatePanelMiscela.GetType(),
                                      String.Format("jQuery_{0}", UpdatePanelMiscela.ClientID), STR_UpdatePanelDose.ToString, True)


    End Sub


#End Region


    Private Sub CaricaKG(ByVal unitaMisuraSelezionata As Integer)

        'Cmb_UdM.Items.Clear()
        'Cmb_UdM.Items.Add(New ListItem("milligrammi", 2032))

        Dim i As Integer
        Dim UdmPresente = False

        If Cmb_UdM IsNot Nothing AndAlso Cmb_UdM.Items IsNot Nothing Then
            For i = 0 To Cmb_UdM.Items.Count - 1
                If Cmb_UdM.Items(i).Value = "2" Then
                    UdmPresente = True
                    Exit For
                End If
            Next
        End If

        If Not UdmPresente Then

            Cmb_UdM.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Grammi, 3)) 'todo, indicare udm da codice.
            Cmb_UdM.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Chilogrammi, 2))
            Cmb_UdM.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Quintali, 4))
            Cmb_UdM.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Tonnellate, 304))

            Cmb_UdM.SelectedIndex = Cmb_UdM.Items.IndexOf(Cmb_UdM.Items.FindByValue(unitaMisuraSelezionata))

            aggiornaGiacenzaPerUdm()

        End If

    End Sub

    Private Sub CaricaLitri(ByVal unitaMisuraSelezionata As Integer)
        'Cmb_UdM.Items.Clear()

        Dim i As Integer
        Dim UdmPresente = False

        If Cmb_UdM IsNot Nothing AndAlso Cmb_UdM.Items IsNot Nothing Then
            For i = 0 To Cmb_UdM.Items.Count - 1
                If Cmb_UdM.Items(i).Value = "29" Then
                    UdmPresente = True
                    Exit For
                End If
            Next
        End If

        If Not UdmPresente Then
            Cmb_UdM.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Millilitri, 101))
            Cmb_UdM.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.CentimetriCubi, 104))
            Cmb_UdM.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Litri, 29))

            Cmb_UdM.SelectedIndex = Cmb_UdM.Items.IndexOf(Cmb_UdM.Items.FindByValue(unitaMisuraSelezionata))

            aggiornaGiacenzaPerUdm()
        End If
    End Sub

    Private Sub Imposta_DosiEtichetta(ByVal DoseEtichetta As String)


        If Session("D_HA_Max") <> 0 Or Session("D_HL_Max") <> 0 Then

            Me.Lbl_Dose_Etichetta.Text = "<br>" & DoseEtichetta
            Cmb_UdM.Items.Clear()
            Dim udm_sorgente As Integer = 0
            'imposto la qta a HA
            If Session("D_HA_Max") <> 0 Then

                Carica_Cmb_UdM_fromUdm(Session("Udm_Radice_HA"), 2123)

                udm_sorgente = Session("Udm_Radice_HA")
                Dim Moltiplicatore As Decimal

                If Session("Chili_Litri") = True Then
                    ConvertiToKG_L(udm_sorgente, Moltiplicatore)
                Else
                    Moltiplicatore = 1
                End If

                If CopiaQuantita Then

                    If Session("Udm_Cod_HA") = 2026 Then
                        Me.Txt_Dose_HA.Text = Session("D_HA_Max") / 10 * Moltiplicatore
                    Else
                        Me.Txt_Dose_HA.Text = Session("D_HA_Max") * Moltiplicatore
                    End If

                    'calcolo la dose totale 
                    Txt_DoseTot_HA.Text = Txt_Dose_HA.Text * Txt_SupTrattata.Value
                    If (CDbl(Txt_Acqua_Tot.Text) > 0) Then
                        Txt_Dose_HL.Text = Txt_DoseTot_HA.Text / Txt_Acqua_Tot.Text
                    Else
                        Txt_Dose_HL.Text = 0
                    End If
                    Cmb_UdM.SelectedValue = udm_sorgente
                End If

            Else

                Carica_Cmb_UdM_fromUdm(Session("Udm_Radice_HL"), 2121)

                udm_sorgente = Session("Udm_Radice_HL")
                Dim Moltiplicatore As Decimal

                If Session("Chili_Litri") = True Then
                    ConvertiToKG_L(udm_sorgente, Moltiplicatore)
                Else
                    Moltiplicatore = 1
                End If

                'imposto la Qta a HL 
                If CopiaQuantita Then
                    Me.Txt_Dose_HL.Text = Session("D_HL_Max") * Moltiplicatore
                    'calcolo la dose totale 
                    Txt_DoseTot_HA.Text = Txt_Dose_HL.Text * Txt_Acqua_Tot.Text
                    If (CDbl(Txt_SupTrattata.Value) > 0) Then
                        Txt_Dose_HA.Text = Txt_DoseTot_HA.Text / Txt_SupTrattata.Value
                    End If
                    Cmb_UdM.SelectedValue = udm_sorgente
                End If

            End If

            'modifico le dosi in base alla selezione kg/litri
            If Session("Chili_Litri") = True Then
                'gestisco kg / litri
                Select Case udm_sorgente
                    'peso
                    Case 3, 2032, 2, 304, 4
                        Cmb_UdM.SelectedValue = "2"

                        'acqua
                    Case 104, 101, 29
                        Cmb_UdM.SelectedValue = "29"
                        'devo trasformare le dosi
                End Select
            End If

        Else

            '--------------------
            'DOSE NON DISPONIBILE
            Me.Lbl_Dose_Etichetta.Text = Resources.AgronicaAgenda_2010.BrNonDisponibile

            Resettasession_Etichetta()

            If CopiaQuantita Then

                Me.Txt_Dose_HA.Text = "0"
                Me.Txt_Dose_HL.Text = "0"
                Txt_DoseTot_HA.Text = "0"

                'imposto il radiobutton solo la prima volta
                If Me.rbl_DoseHA.Enabled Then

                    Select Case 0

                        Case 21, 23, 37, 126, 164, 165, 173, 175
                            Me.rbl_DoseHL.Checked = True   '/hl

                        Case 20, 22, 88, 89, 90, 163, 176, 318
                            Me.rbl_DoseHA.Checked = True  '/ha

                        Case Else
                            Me.rbl_DoseHA.Checked = True
                    End Select

                End If
            End If
        End If



        'preseleziono kg / Litri
        If Session("Chili_Litri") = True Then
            If Not IsNothing(Cmb_UdM.Items.FindByValue("2")) Then
                Cmb_UdM.SelectedValue = "2"
            End If

            If Not IsNothing(Cmb_UdM.Items.FindByValue("29")) Then
                Cmb_UdM.SelectedValue = "29"
            End If
        End If

        Session("DoseEtichetta") = Lbl_Dose_Etichetta.Text

        aggiornaGiacenzaPerUdm()

    End Sub

    Private Sub Imposta_VolumiAcqua(ByVal AcquaMin As Decimal, _
                                ByVal AcquaMax As Decimal, _
                                ByVal AcquaUdm_Cod As Integer)
        'se ho già inserito un prodotto non modifico la dose acqua
        If GridView_Dosi.Rows.Count > 0 Then
            Exit Sub
        End If

        If AcquaMax <> 0 OrElse AcquaMin <> 0 Then
            Dim AcquaUdm_Radice, Acquaper_ha_hl As Integer
            Dim AcquaMaxHL As Decimal
            Dim AcquaMinHL As Decimal
            AcquaUdm_Radice = -1
            ScomponiUdm(AcquaUdm_Radice, Acquaper_ha_hl, AcquaUdm_Cod)
            If AcquaUdm_Radice <> -1 Then
                If Acquaper_ha_hl = 2123 Then
                    Me.rblAcqua_HA.Checked = True
                    Me.rblAcqua_Tot.Checked = False
                    If AcquaMax <> 0 Then
                        AcquaMaxHL = AgronicaCoreMetaSchemaDAL.UnitaMisura_R.Converti(AcquaUdm_Cod, _
                                                    AcquaMax, TipiEnumerativi.enum_UnitaMisura.Ettolitro)
                    End If

                    If AcquaMin <> 0 Then
                        AcquaMinHL = AgronicaCoreMetaSchemaDAL.UnitaMisura_R.Converti(AcquaUdm_Cod, _
                                                    AcquaMin, TipiEnumerativi.enum_UnitaMisura.Ettolitro)
                    End If

                    'controllo se l'acqua non è già stata inserita
                    If IsNumeric(Txt_Acqua_Ha.Text) AndAlso Txt_Acqua_Ha.Text > 0 Then
                        'se è all'interno del range non faccio nulla altrimenti lo modifico
                        'If Txt_Acqua_Ha.Text < AcquaMin Then
                        '    'imposto la qta di acqua minima
                        '    Txt_Acqua_Ha.Text = AcquaMinHL.ToString
                        '    lbl_acqua_provenienza.Text = "da vincoli etichetta"
                        'Else

                        'If Txt_Acqua_Ha.Text > AcquaMax Then
                        'imposto la qta di acqua max
                        Txt_Acqua_Ha.Text = AcquaMaxHL.ToString
                        lbl_acqua_provenienza.Text = Resources.AgronicaAgenda_2010.DaDosiDiEtichetta
                        'Else
                        'mantengo la dose di acqua impostata dato che è all'interno del range
                        'di etichetta
                        'End If
                        'End If
                    Else
                        Txt_Acqua_Ha.Text = AcquaMaxHL.ToString
                        lbl_acqua_provenienza.Text = Resources.AgronicaAgenda_2010.DaVincoliEtichetta
                    End If



                    '                    //Aggiorno l'acqua
                    'AggiornaACQUA();
                    '//Aggiorno le Dosi solo se ho la qta/ha selezionata
                    'AggiornaDOSI();
                    Dim STR_UpdatePanelDose As New StringBuilder
                    STR_UpdatePanelDose.AppendLine("$(document).ready(function () { ")
                    STR_UpdatePanelDose.AppendLine("    AggiornaACQUA();")
                    STR_UpdatePanelDose.AppendLine("    AggiornaDOSI();")
                    STR_UpdatePanelDose.AppendLine(" });")


                    'ScriptManager.RegisterStartupScript(UpdatePanel_Epoca, UpdatePanel_Epoca.GetType(),
                    '                              String.Format("jQuery_{0}", UpdatePanel_Epoca.ClientID), STR_UpdatePanelDose.ToString, True)


                End If
            End If
        End If
    End Sub

    Private Function CreaStringa_DoseEtichetta(ByVal DoseMin As String, _
                                          ByVal DoseMax As String, _
                                          ByVal Udm_Sim As String, _
                                          ByVal Udm_Cod As Integer, _
                                          ByVal AcquaMin As String, _
                                          ByVal AcquaMax As String, _
                                          ByVal AcquaUdm_Sim As String, _
                                          ByVal AcquaUdm_Cod As Integer, _
                                          ByVal Limite As String, _
                                          ByVal Limite_Sim As String, _
                                          ByVal Limite_Udm_Cod As Integer, _
                                          ByVal Da_Epoca As String, _
                                          ByVal A_Epoca As String, _
                                          ByVal Flag_Fioritura As Integer, _
                                          ByVal IntervalloTrattamenti_Min As Integer, _
                                          ByVal IntervalloTrattamenti_Max As Integer) As String

        Dim DoseEtichetta As String = "<FONT color=blue>"
        DoseEtichetta &= DoseMin.ToString & "-" & DoseMax.ToString & " " & Udm_Sim.ToString
        DoseEtichetta &= "<font style='font-size:9px; color:#555;'>"

        If AcquaMin <> 0 OrElse AcquaMax <> 0 Then
            DoseEtichetta &= String.Format(Resources.AgronicaAgenda_2010.VolAcquaX0X1X2, AcquaMin, AcquaMax, AcquaUdm_Sim)
        End If

        If Limite <> 0 Then
            DoseEtichetta &= String.Format(Resources.AgronicaAgenda_2010.MaxX0InterventiX1, Limite, Limite_Sim)
        End If

        If Flag_Fioritura <> 0 Then
            DoseEtichetta &= Resources.AgronicaAgenda_2010.SospendereITrattamentiAFineFioritura
        End If

        If IntervalloTrattamenti_Min <> 0 OrElse IntervalloTrattamenti_Max <> 0 Then
            DoseEtichetta &= " da effettuare da " & IntervalloTrattamenti_Min & " - " & IntervalloTrattamenti_Max & " gg. dal precedente trattamento"
        End If

        'verifica se è ok o no



        Dim strEpoca As String = ""
        If Da_Epoca <> 0 AndAlso A_Epoca <> 0 Then
            If Da_Epoca <> A_Epoca Then
                If Da_Epoca <> 0 Then
                    Dim objEpoca As New AgronicaCoreMetaSchemaDAL.Epoche_R
                    strEpoca = Resources.AgronicaAgenda_2010.DA & objEpoca.EpocaDes_from_EpocaCod(Da_Epoca, objParametri_Server)
                End If
                If A_Epoca <> 0 Then
                    Dim objEpoca As New AgronicaCoreMetaSchemaDAL.Epoche_R
                    strEpoca &= Resources.AgronicaAgenda_2010.A & objEpoca.EpocaDes_from_EpocaCod(A_Epoca, objParametri_Server)
                End If
            Else
                Dim objEpoca As New AgronicaCoreMetaSchemaDAL.Epoche_R
                strEpoca &= " in " & objEpoca.EpocaDes_from_EpocaCod(A_Epoca, objParametri_Server) 'todo, in come si indica nelle risorse??
            End If
        Else
            If Da_Epoca <> 0 Then
                Dim objEpoca As New AgronicaCoreMetaSchemaDAL.Epoche_R
                strEpoca = Resources.AgronicaAgenda_2010.DA & objEpoca.EpocaDes_from_EpocaCod(Da_Epoca, objParametri_Server)
            End If
            If A_Epoca <> 0 Then
                Dim objEpoca As New AgronicaCoreMetaSchemaDAL.Epoche_R
                strEpoca &= Resources.AgronicaAgenda_2010.A & objEpoca.EpocaDes_from_EpocaCod(A_Epoca, objParametri_Server)
            End If
        End If

        If strEpoca <> "" Then
            DoseEtichetta &= strEpoca
        End If
        DoseEtichetta &= "</font></font>"
        DoseEtichetta &= "<br>"

        'modifico le variabili di sessione

        'verifico se è dose Ettato o dose HL
        Dim Udm_Radice, per_ha_hl As Integer
        ScomponiUdm(Udm_Radice, per_ha_hl, Udm_Cod)
        If Udm_Radice <> -1 Then
            'converto l'acqua in Hl
            Dim AcquaMaxHL As Decimal = 0
            Dim AcquaMinHL As Decimal = 0

            If AcquaMax <> 0 Then
                Dim AcquaUdm_Radice, Acquaper_ha_hl As Integer

                AcquaUdm_Radice = -1
                ScomponiUdm(AcquaUdm_Radice, Acquaper_ha_hl, AcquaUdm_Cod)
                If AcquaUdm_Radice <> -1 Then
                    If Acquaper_ha_hl = 2123 Then
                        Select Case AcquaUdm_Cod
                            Case 2 'kg
                                AcquaMaxHL = AcquaMax / 100
                            Case 4 'q
                                AcquaMaxHL = AcquaMax
                            Case 304 't
                                AcquaMaxHL = AcquaMax * 10
                            Case 3 'g
                                AcquaMaxHL = AcquaMax / 10000
                            Case 2032 'mg
                                AcquaMaxHL = AcquaMax / 100000
                            Case 29 'l
                                AcquaMaxHL = AcquaMax / 100
                            Case 101 'ml
                                AcquaMaxHL = AcquaMax / 100000
                            Case 104 'cc
                                AcquaMaxHL = AcquaMax / 100000
                        End Select
                    End If
                End If

            End If

            If AcquaMin <> 0 Then
                Dim AcquaUdm_Radice, Acquaper_ha_hl As Integer

                AcquaUdm_Radice = -1
                ScomponiUdm(AcquaUdm_Radice, Acquaper_ha_hl, AcquaUdm_Cod)
                If AcquaUdm_Radice <> -1 Then
                    If Acquaper_ha_hl = 2123 Then
                        Select Case AcquaUdm_Cod
                            Case 2 'kg
                                AcquaMinHL = AcquaMin / 100
                            Case 4 'q
                                AcquaMinHL = AcquaMin
                            Case 304 't
                                AcquaMinHL = AcquaMin * 10
                            Case 3 'g
                                AcquaMinHL = AcquaMin / 10000
                            Case 2032 'mg
                                AcquaMinHL = AcquaMin / 100000
                            Case 29 'l
                                AcquaMinHL = AcquaMin / 100
                            Case 101 'ml
                                AcquaMinHL = AcquaMin / 100000
                            Case 104 'cc
                                AcquaMinHL = AcquaMin / 100000
                        End Select
                    End If
                End If
            End If


            If per_ha_hl = 2121 Then
                'HL
                Impostasession_Etichetta(AcquaMinHL, AcquaMaxHL, 0, DoseMax, 0, DoseMin, 0, 0, Udm_Radice, Udm_Cod, Limite, Limite_Sim, Limite_Udm_Cod, Flag_Fioritura, IntervalloTrattamenti_Min, IntervalloTrattamenti_Max)
            Else
                'HA
                Impostasession_Etichetta(AcquaMinHL, AcquaMaxHL, DoseMax, 0, DoseMin, 0, Udm_Radice, Udm_Cod, 0, 0, Limite, Limite_Sim, Limite_Udm_Cod, Flag_Fioritura, IntervalloTrattamenti_Min, IntervalloTrattamenti_Max)
            End If
        End If


        Return DoseEtichetta
    End Function

    Private Sub Carica_Cmb_UdM_fromUdm(ByVal Udm_Radice As String, ByVal Udm_per As Integer)


        Select Case Udm_Radice
            'peso
            Case 3      'g
                CaricaKG(Udm_Radice)
            Case 2032   'mg
                CaricaKG(Udm_Radice)
            Case 2      'kg
                CaricaKG(Udm_Radice)
            Case 304    't
                CaricaKG(Udm_Radice)
            Case 4      'q 
                CaricaKG(Udm_Radice)

                'acqua
            Case 104
                'cc
                CaricaLitri(Udm_Radice)
            Case 101
                'ml
                CaricaLitri(Udm_Radice)
            Case 29
                'l
                CaricaLitri(Udm_Radice)

        End Select

        If CopiaQuantita Then
            Select Case Udm_per
                Case 2121
                    Me.rbl_DoseHL.Checked = True
                    Me.rbl_DoseHA.Checked = False
                Case Else
                    Me.rbl_DoseHA.Checked = True
                    Me.rbl_DoseHL.Checked = False
            End Select
        End If

        aggiornaGiacenzaPerUdm()

    End Sub

    Private Sub ConvertiToKG_L(ByVal UDM As Integer, ByRef Moltiplicatore As Decimal)
        Select Case UDM
            Case 3  'g
                Moltiplicatore = 0.001 'caso in cui ho i grammi
            Case 2032  'mg
                Moltiplicatore = 0.000001   'caso in cui ho i mg
            Case 4  'q
                Moltiplicatore = 100  'caso in cui ho i quintali
            Case 304 't
                Moltiplicatore = 1000   'caso in cui ho le tonnelate
            Case 101 'ml
                Moltiplicatore = 0.001  'caso in cui ho i ml
            Case 104 'cc
                Moltiplicatore = 0.001  'caso in cui ho i cc
            Case 2, 29 'kg,l
                Moltiplicatore = 1
        End Select
    End Sub

    Private Sub ScomponiUdm(ByRef UDM_radice As Integer, _
                              ByRef perHa_hl As Integer, _
                              ByRef UnitaMisura As Integer)
        Select Case UnitaMisura
            '---------------------
            ' a HL
            Case 21  'cc/hl
                UDM_radice = 104
                perHa_hl = 2121
            Case 23 'g/hl
                UDM_radice = 3
                perHa_hl = 2121
            Case 126 'mg/hl
                UDM_radice = 2032
                perHa_hl = 2121
            Case 164  'ml/hl
                UDM_radice = 101
                perHa_hl = 2121
            Case 173 'l/hl
                UDM_radice = 29
                perHa_hl = 2121
            Case 175  'kg/hl
                UDM_radice = 2
                perHa_hl = 2121

                '---------------------
                'a HA
            Case 20  'g/ha
                UDM_radice = 3
                perHa_hl = 2123
            Case 22  'l/ha
                UDM_radice = 29
                perHa_hl = 2123
            Case 88  'kg/ha
                UDM_radice = 2
                perHa_hl = 2123

            Case 89 'unita/ha
                UDM_radice = -1
                perHa_hl = 2123

            Case 90  'm3/ha
                UDM_radice = -1
                perHa_hl = 2123

            Case 163  'ml/ha
                UDM_radice = 101
                perHa_hl = 2123
            Case 176  'n° u/ha
                perHa_hl = 2123

            Case 2098  't/ha
                UDM_radice = 304
                perHa_hl = 2123

            Case 2112 't/ha spighe
                UDM_radice = 304
                perHa_hl = 2123

            Case 2120  'q/ha
                UDM_radice = 4
                perHa_hl = 2123

            Case 2 'kg
                UDM_radice = 2
                perHa_hl = 2123

            Case 4 'q
                UDM_radice = 4
                perHa_hl = 2123

            Case 29 'l
                UDM_radice = 29
                perHa_hl = 2123

                '--------------------
                'concianti
            Case 2004   'l/100 kg di seme
                UDM_radice = 29
                perHa_hl = 0
            Case 2005   'ml/100 kg di seme	
                UDM_radice = 101
                perHa_hl = 0
            Case 2017   'ml/100 kg di semi	
                UDM_radice = 101
                perHa_hl = 0
            Case 2021   'ml/kg di semente	
                UDM_radice = 101
                perHa_hl = 0
            Case 5001003    'ml/unità di seme	
                UDM_radice = 101
                perHa_hl = 0
            Case 2006   'g/unita' di seme	
                UDM_radice = 3
                perHa_hl = 0
            Case 2007   'g/100 kg di semente	
                UDM_radice = 3
                perHa_hl = 0
            Case 2010   'kg/100 kg di seme
                UDM_radice = 2
                perHa_hl = 0
            Case 2026  'kg/1 tonnellata di semente
                UDM_radice = 2
                perHa_hl = 0


        End Select

    End Sub

    Private Sub Resettasession_Etichetta()
        ViewState("DoseConsentita") = 0
        Session.Remove("D_HA_Max_Diserbo")


        Session.Remove("N_Trattamenti_Max")
        Session.Remove("N_Trattamenti_Umd_Cod")
        Session.Remove("N_Trattamenti_UDM_Sim")

        Session.Remove("Flag_Fioritura")

        Session.Remove("Acqua_Min")
        Session.Remove("Acqua_Max")

        Session.Remove("D_HA_Max")
        Session.Remove("D_HL_Max")

        Session.Remove("D_HL_Min")
        Session.Remove("D_HA_Min")


        Session.Remove("Udm_Radice_HA")
        Session.Remove("Udm_Radice_HL")

        Session.Remove("Udm_Cod_HA")
        Session.Remove("Udm_Cod_HL")


    End Sub

    Private Sub Impostasession_Etichetta(ByVal Acqua_Min As Decimal, _
                                     ByVal Acqua_Max As Decimal, _
                                     ByVal D_HA_Max As Decimal, _
                                     ByVal D_HL_Max As Decimal, _
                                     ByVal D_HA_Min As Decimal, _
                                     ByVal D_HL_Min As Decimal, _
                                     ByVal Udm_Radice_HA As Integer, _
                                     ByVal Udm_Cod_HA As Integer, _
                                     ByVal Udm_Radice_HL As Integer, _
                                     ByVal Udm_Cod_HL As String, _
                                     ByVal N_Trattamenti_Max As Integer, _
                                     ByVal N_Trattamenti_UDM_Sim As String, _
                                     ByVal N_Trattamenti_Umd_Cod As Integer, _
                                     ByVal Flag_Fioritura As Integer, _
                                     ByVal IntervalloTrattamenti_Min As Integer, _
                                     ByVal IntervalloTrattamenti_Max As Integer)


        If N_Trattamenti_Max <> 0 Then
            If IsNothing(Session("N_Trattamenti_Max")) Then
                Session("N_Trattamenti_Max") = N_Trattamenti_Max
                Session("N_Trattamenti_Umd_Cod") = N_Trattamenti_Umd_Cod
                Session("N_Trattamenti_UDM_Sim") = N_Trattamenti_UDM_Sim
            Else
                If N_Trattamenti_Max > Session("N_Trattamenti_Max") Then
                    Session("N_Trattamenti_Max") = N_Trattamenti_Max
                    Session("N_Trattamenti_Umd_Cod") = N_Trattamenti_Umd_Cod
                    Session("N_Trattamenti_UDM_Sim") = N_Trattamenti_UDM_Sim
                End If
            End If
        End If

        Session("IntervalloTrattamenti_Min") = IntervalloTrattamenti_Min
        Session("IntervalloTrattamenti_Max") = IntervalloTrattamenti_Max



        If Flag_Fioritura <> 0 Then
            Session("Flag_Fioritura") = Flag_Fioritura
        End If


        If Acqua_Min <> 0 Then
            If Acqua_Min < Session("Acqua_Min") Then
                Session("Acqua_Min") = Acqua_Min
            Else
                If Session("Acqua_Min") = 0 Then
                    Session("Acqua_Min") = Acqua_Min
                End If
            End If
        End If

        If Acqua_Max <> 0 AndAlso Acqua_Max > Session("Acqua_Max") Then
            Session("Acqua_Max") = Acqua_Max
        End If

        If D_HA_Max <> 0 AndAlso D_HA_Max > Session("D_HA_Max") Then
            Session("D_HA_Max") = D_HA_Max
        End If

        If D_HL_Max <> 0 AndAlso D_HL_Max > Session("D_HL_Max") Then
            Session("D_HL_Max") = D_HL_Max
        End If


        If D_HL_Min <> 0 AndAlso D_HL_Min < IIf(IsNothing(Session("D_HL_Min")), _
                                                10000000, Session("D_HL_Min")) Then
            Session("D_HL_Min") = D_HL_Min
        End If
        If D_HA_Min <> 0 AndAlso D_HA_Min < IIf(IsNothing(Session("D_HA_Min")), _
                                                10000000, Session("D_HA_Min")) Then
            Session("D_HA_Min") = D_HA_Min
        End If



        If Udm_Radice_HA <> 0 Then
            Session("Udm_Radice_HA") = Udm_Radice_HA
        End If
        If Udm_Radice_HL <> 0 Then
            Session("Udm_Radice_HL") = Udm_Radice_HL
        End If

        If Udm_Cod_HA <> 0 Then
            Session("Udm_Cod_HA") = Udm_Cod_HA
        End If

        If Udm_Cod_HL <> 0 Then
            Session("Udm_Cod_HL") = Udm_Cod_HL
        End If

    End Sub

    Private Sub impostazione_EtichetteUDM(ByVal UDM_DaUtente As String)



        lblSupHaSelezionata.Text = String.Format(lblSupHaSelezionata.Text, UDM_DaUtente)
        lblSupHaTrattata.Text = String.Format(lblSupHaTrattata.Text, UDM_DaUtente)
        rbl_DoseHA.Text = String.Format(rbl_DoseHA.Text, UDM_DaUtente)
        rblAcqua_HA.Text = String.Format(rblAcqua_HA.Text, UDM_DaUtente)

        UDM_Helper.CambiaIntestazioneGridview(0, "Dose_HA", UDM_DaUtente, GridView_Dosi, 8)
        GridView_Dosi.DataBind()

    End Sub

    Public Function Formulati_SpecieVegetali_Avversita_Dosi(ByVal Fr_Cod As Integer, _
                                                        ByVal Veg_Cod As Integer, _
                                                        ByVal Av_Cod As Integer, _
                                                        ByVal Av_Gru As Integer, _
                                                        ByVal grfi_cod As Integer) As DataTable

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XML_Credenziali As System.Xml.XmlElement
        Dim StrCredenziali As String = ""
        Dim StrParametri As String = ""
        Dim strErr As String = ""
        Dim Parametri As String = ""
        Dim DtRisultati As DataTable


        'Dim Dt_Dose As New DataTable

        Dim objParametri_Utenti As New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        Dim ObjDownloadWs As New WS_Fitofarmaci.AgroWS_Fitofarmaci
        Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

        objWs.NewWS(ObjDownloadWs, _
                        objParametriAgenda.WS_Fitofarmaci_AgroWS_Fitofarmaci, _
                        objParametri_Utenti)

        Dim objCoreAgroWs As New AgronicaCoreWebService.AgroWs


        objCoreAgroWs.AgroWS_XML__Credenziali(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA, _
                            StrCredenziali, _
                            AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali_Avversita_Dosi, _
                            objCoreAgroWs.AgroWS_DoorKey(AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali_Avversita_Dosi), _
                            Session("ASG_ProgressivoGIAS"), _
                            Session("ASG_SuperUser_Username").ToString, _
                            Session("ASG_SuperUser_Password").ToString)

        XmlDoc.LoadXml(StrCredenziali)


        XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")


        objCoreAgroWs.AgroWS_XML_Parametri_Formulati_SpecieVegetali_Avversita_Dosi(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                                                         StrParametri,
                                                                         Fr_Cod.ToString,
                                                                         Veg_Cod.ToString,
                                                                         Av_Cod.ToString,
                                                                         Av_Gru.ToString,
                                                                         grfi_cod,
                                                                         "0", "0", "",
                                                                         objParametriAgenda.Data,
                                                                         "0", strErr
                                                                         )

        XML_Credenziali.InnerXml = StrParametri

        Parametri = XmlDoc.OuterXml

        Parametri = objCoreAgroWs.AWS_Codifica_P(Parametri)

        DtRisultati = ObjDownloadWs.Formulati_SpecieVegetali_Avversita_Dosi_DT(Parametri, strErr)

        'If Not DtRisultati Is Nothing Then

        'End If

        'Risultati = objCoreAgroWs.AWS_Decodifica_R(Risultati)
        'Risultati = Risultati.Replace(">", ">" & vbCrLf)

        'objCoreAgroWs.AgroWS_XML_Risultati_Formulati_SpecieVegetali_Avversita_Dosi(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_DECODIFICA, _
        '                                                                Risultati, _
        '                                                                Dt_Dose, _
        '                                                                strErr)
        Return DtRisultati

    End Function

    Public Function Formulati_SpecieVegetali_Infestanti_Dosi(ByVal Fr_Cod As Integer, _
                                                        ByVal Veg_Cod As Integer, _
                                                        ByVal Av_Cod As Integer, _
                                                        ByVal Av_Gru As Integer, _
                                                        ByVal grfi_cod As Integer) As DataTable

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XML_Credenziali As System.Xml.XmlElement
        Dim StrCredenziali As String = ""
        Dim StrParametri As String = ""
        Dim strErr As String = ""
        Dim Parametri As String = ""
        Dim DtRisultati As DataTable

        'Dim Dt_Dose As New DataTable

        Dim objParametri_Utenti As New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        Dim ObjDownloadWs As New WS_Fitofarmaci.AgroWS_Fitofarmaci
        Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

        objWs.NewWS(ObjDownloadWs, _
                        objParametriAgenda.WS_Fitofarmaci_AgroWS_Fitofarmaci, _
                        objParametri_Utenti)

        Dim objCoreAgroWs As New AgronicaCoreWebService.AgroWs


        objCoreAgroWs.AgroWS_XML__Credenziali(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA, _
                            StrCredenziali, _
                            AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali_Infestanti_Dosi, _
                            objCoreAgroWs.AgroWS_DoorKey(AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali_Infestanti_Dosi), _
                            Session("ASG_ProgressivoGIAS"), _
                            Session("ASG_SuperUser_Username").ToString, _
                            Session("ASG_SuperUser_Password").ToString)

        XmlDoc.LoadXml(StrCredenziali)

        XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")


        objCoreAgroWs.AgroWS_XML_Parametri_Formulati_SpecieVegetali_Infestanti_Dosi(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                                                         StrParametri,
                                                                         Fr_Cod.ToString,
                                                                         Veg_Cod.ToString,
                                                                         Av_Cod.ToString,
                                                                         Av_Gru.ToString,
                                                                         grfi_cod,
                                                                         "0", "0", "",
                                                                         objParametriAgenda.Data,
                                                                         "0",
                                                                                    strErr
                                                                         )

        XML_Credenziali.InnerXml = StrParametri

        Parametri = XmlDoc.OuterXml

        Parametri = objCoreAgroWs.AWS_Codifica_P(Parametri)

        DtRisultati = ObjDownloadWs.Formulati_SpecieVegetali_Infestanti_Dosi_DT(Parametri, strErr)

        'Risultati = objCoreAgroWs.AWS_Decodifica_R(Risultati)
        'Risultati = Risultati.Replace(">", ">" & vbCrLf)

        'objCoreAgroWs.AgroWS_XML_Risultati_Formulati_SpecieVegetali_Infestanti_Dosi(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_DECODIFICA, _
        '                                                                Risultati, _
        '                                                                Dt_Dose, _
        '                                                                strErr)
        Return DtRisultati


    End Function

    'Public Function TempoCarenza_from_FrCod_VegCod(ByRef objServer As System.Web.HttpServerUtility, _
    '                                            ByRef objsession As System.Web.SessionState.HttpSessionState, _
    '                                            ByRef objPage As System.Web.UI.Page, _
    '                                            ByVal FrCod As Integer, _
    '                                            ByVal VegCod As Integer) As Integer

    '    Dim Parametri As String = ""
    '    Dim Risultati As String = ""

    '    Dim strErr As String = ""

    '    Dim XmlDoc As System.Xml.XmlDocument
    '    Dim XML_Credenziali As System.Xml.XmlElement
    '    Dim StrCredenziali As String = ""
    '    Dim StrParametri As String = ""

    '    Dim DtRisultati As DataTable

    '    If VegCod = 0 Or VegCod < 0 Then
    '        Return -1
    '    End If

    '    Try

    '        Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    '        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

    '        Dim ObjDownloadWs As New WS_Fitofarmaci.AgroWS_Fitofarmaci
    '        Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

    '        objWs.NewWS(ObjDownloadWs, _
    '                        objParametriAgenda.WS_Fitofarmaci_AgroWS_Fitofarmaci, _
    '                        objParametri_Utenti)

    '        Dim objCoreAgroWs As New AgronicaCoreWebService.AgroWs


    '        XmlDoc = New System.Xml.XmlDocument

    '        objCoreAgroWs.AgroWS_XML__Credenziali(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA, _
    '                                StrCredenziali, _
    '                                AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali, _
    '                                objCoreAgroWs.AgroWS_DoorKey(AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali), _
    '                                objsession("ASG_ProgressivoGIAS"), _
    '                                objsession("ASG_SuperUser_Username").ToString, _
    '                                objsession("ASG_SuperUser_Password").ToString)

    '        XmlDoc.LoadXml(StrCredenziali)

    '        XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

    '        objCoreAgroWs.AgroWS_XML_Parametri_Formulati_SpecieVegetali(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA, _
    '                                                      StrParametri, _
    '                                                      FrCod, _
    '                                                      VegCod, _
    '                                                      0, _
    '                                                      strErr)

    '        XML_Credenziali.InnerXml = StrParametri

    '        Parametri = XmlDoc.OuterXml

    '        Parametri = objCoreAgroWs.AWS_Codifica_P(Parametri)

    '        Risultati = ObjDownloadWs.Formulati_SpecieVegetali(Parametri)

    '        Risultati = objCoreAgroWs.AWS_Decodifica_R(Risultati)
    '        Risultati = Risultati.Replace(">", ">" & vbCrLf)

    '        DtRisultati = New DataTable

    '        objCoreAgroWs.AgroWS_XML_Risultati_Formulati_SpecieVegetali(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_DECODIFICA, _
    '                                                      Risultati, _
    '                                                      DtRisultati, _
    '                                                      strErr)


    '        If strErr = "" Then

    '            If DtRisultati.Rows.Count > 0 Then

    '                If IsDBNull(DtRisultati.Rows(0).Item("tempocarenza")) Then
    '                    Return -1
    '                End If

    '                If DtRisultati.Rows(0).Item("tempocarenza") = "" Then
    '                    Return -1
    '                End If
    '                Return DtRisultati.Rows(0).Item("tempocarenza")
    '            Else
    '                Return -1
    '            End If

    '        Else
    '            Return -1
    '        End If

    '    Catch ex As Exception

    '        Return 0

    '    End Try
    'End Function

#Region "Impostazioni Varie"

    Private Sub SettaUdmDatoCodice(ByVal lUdm As Integer)
        Dim udmSim As String

        udmSim = UDM_Helper.GetUdmSim(lUdm, objParametri_Server)
        impostazione_EtichetteUDM(udmSim)
    End Sub

    Private Sub SettaImpostazioneUtente_UDM()
        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim Dt_Impostazioni As DataTable

        Dt_Impostazioni = ObjUtenti.Leggi(0, _
                                          1, _
                                          enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                          "", _
                                          "", _
                                          objParametri_Utenti)

        Dim fatto As Boolean = False

        If Dt_Impostazioni IsNot Nothing AndAlso Dt_Impostazioni.Rows.Count > 0 Then

            For i = 0 To Dt_Impostazioni.Rows.Count - 1

                Select Case CInt(Dt_Impostazioni.Rows(i).Item("Impostazione_Cod"))

                    Case enum_Impostazioni_Utenti.UTENTE_UDM_Area_COD
                        Dim lUdm As Integer = CInt(Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1"))

                        SettaUdmDatoCodice(lUdm)
                        fatto = True
                End Select

            Next
        End If

        If Not fatto Then
            SettaUdmDatoCodice(2123)
        End If

    End Sub

    Private Sub SettaImpostazioniUtente()

        ViewState("ImpostoDPINellaCombo") = Nothing
        Session("DpiPredefinitoUtente") = Nothing
        Session("ControllaBlocco") = Nothing
        Session("Blocca_Dose_Massima") = Nothing
        Session("Blocca_Dose_Minima") = Nothing
        Session("Blocca_Acqua_Massima") = Nothing
        Session("Blocca_Acqua_Minima") = Nothing
        Session("Blocca_Numero_Massimo") = Nothing
        Session("Blocca_Carenza") = Nothing
        Session("Blocca_Fioritura") = Nothing
        Session("Blocca_Intervallo_Minimo") = Nothing

        If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura Then

            If Cella_Avversita.Visible Then

                Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                Dim Dt_Impostazioni As DataTable

                Dt_Impostazioni = ObjUtenti.Leggi(0, _
                                                  1, _
                                                  enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                                  "", _
                                                  "", _
                                                  objParametri_Utenti)


                Session("ControllaBlocco") = False
                Dim impostatodisciplinarepredefinito As Boolean = False
                If Dt_Impostazioni IsNot Nothing AndAlso Dt_Impostazioni.Rows.Count > 0 Then


                    For i = 0 To Dt_Impostazioni.Rows.Count - 1

                        Select Case CInt(Dt_Impostazioni.Rows(i).Item("Impostazione_Cod"))

                            ''AVVERSITA  --> valore: 88=gruppi 77=singole
                            Case enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_SINGOLE_GRUPPI_AVVERSITA

                                '    Select Case Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                                '        Case "77"
                                '            Me.RBL_Avversita.SelectedValue = "0"
                                '        Case Else
                                '            Me.RBL_Avversita.SelectedValue = "1"
                                '    End Select

                                '    RBL_Avversita_SelectedIndexChanged(Me, Nothing)

                                'DPI  --> valore: 0=nessuno 1=dpi
                            Case enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_DPI

                                If Not impostatodisciplinarepredefinito Then



                                    If ComboDisciplinari1.ddl_Disciplinari.Visible Then

                                        Select Case Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                                            Case "0"
                                                ComboDisciplinari1.ddl_Disciplinari.SelectedIndex = 0
                                                objParametriAgenda.Disciplinare = ComboDisciplinari1.Valore_Combo
                                                'ComboDisciplinari_new.SelectedIndex = 0
                                                'objParametriAgenda.Disciplinare = ComboDisciplinari_new.SelectedValue
                                                ViewState("ImpostoDPINellaCombo") = False
                                                Session("DpiPredefinitoUtente") = "0"
                                            Case Else
                                                'salvo in viewstate un valore per indicare che devo impostare il disciplinare,
                                                'usato nella caricacombo quando viene ricaricata a seguito di modifiche
                                                ViewState("ImpostoDPINellaCombo") = True
                                                If ComboDisciplinari1.ddl_Disciplinari.Items.Count > 1 Then
                                                    ComboDisciplinari1.ddl_Disciplinari.SelectedIndex = 1

                                                    objParametriAgenda.Disciplinare = ComboDisciplinari1.Valore_Combo
                                                    Session("DpiPredefinitoUtente") = objParametriAgenda.Disciplinare
                                                    'rimuovo dalla combo il filtro nessuno--> nessuno
                                                    Dim jj As Integer = 0
                                                    For jj = 0 To ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.Items.Count - 1
                                                        If ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.Items(jj).Value = 0 Then
                                                            ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.Items.RemoveAt(jj)
                                                            Exit For
                                                        End If
                                                    Next
                                                End If
                                                'If ComboDisciplinari_new.Items.Count > 1 Then
                                                '    ComboDisciplinari_new.SelectedIndex = 1
                                                '    objParametriAgenda.Disciplinare = ComboDisciplinari_new.SelectedValue
                                                'End If
                                        End Select

                                    Else
                                        ComboDisciplinari1.ddl_Disciplinari.SelectedIndex = 0
                                        objParametriAgenda.Disciplinare = "0"
                                        'ComboDisciplinari_new.SelectedIndex = 0
                                        'objParametriAgenda.Disciplinare = ComboDisciplinari_new.SelectedValue
                                        ViewState("ImpostoDPINellaCombo") = False
                                        Session("DpiPredefinitoUtente") = "0"
                                    End If


                                End If





                                'DPI  --> valore: disciplinare predefinito es 34/1
                            Case enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_DPI_PREDEFINITO
                                impostatodisciplinarepredefinito = True
                                If ComboDisciplinari1.ddl_Disciplinari.Visible Then

                                    Select Case Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                                        Case "0"
                                            ComboDisciplinari1.ddl_Disciplinari.SelectedIndex = 0
                                            objParametriAgenda.Disciplinare = ComboDisciplinari1.Valore_Combo
                                            ViewState("ImpostoDPINellaCombo") = False
                                            Session("DpiPredefinitoUtente") = "0"
                                        Case Else
                                            'salvo in viewstate un valore per indicare che devo impostare il disciplinare,
                                            'usato nella caricacombo quando viene ricaricata a seguito di modifiche
                                            Dim disciplinare As String = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                                            ViewState("ImpostoDPINellaCombo") = True
                                            Session("DpiPredefinitoUtente") = disciplinare
                                            If ComboDisciplinari1.ddl_Disciplinari.Items.Count > 1 Then
                                                'ComboDisciplinari1.ddl_Disciplinari.SelectedIndex = 1
                                                ComboDisciplinari1.ddl_Disciplinari.SelectedIndex = 0
                                                For kk = 0 To ComboDisciplinari1.ddl_Disciplinari.Items.Count - 1
                                                    Dim val As String() = ComboDisciplinari1.ddl_Disciplinari.Items(kk).Value.Split("/")

                                                    If val(0) = disciplinare.Split("/")(0) Then
                                                        If val.Length = 1 Then
                                                            ComboDisciplinari1.ddl_Disciplinari.SelectedIndex = kk
                                                        End If
                                                        If val.Length = 5 AndAlso disciplinare.Split("/").Length = 2 Then
                                                            If val(4) = disciplinare.Split("/")(1) Then
                                                                ComboDisciplinari1.ddl_Disciplinari.SelectedIndex = kk
                                                            End If
                                                        End If

                                                    End If

                                                Next


                                                objParametriAgenda.Disciplinare = ComboDisciplinari1.Valore_Combo
                                                Session("DpiPredefinitoUtente") = objParametriAgenda.Disciplinare

                                                'rimuovo dalla combo il filtro nessuno--> nessuno
                                                Dim jj As Integer = 0
                                                For jj = 0 To ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.Items.Count - 1
                                                    If ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.Items(jj).Value = 0 Then
                                                        ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.Items.RemoveAt(jj)
                                                        Exit For
                                                    End If
                                                Next
                                            End If
                                            'If ComboDisciplinari_new.Items.Count > 1 Then
                                            '    ComboDisciplinari_new.SelectedIndex = 1
                                            '    objParametriAgenda.Disciplinare = ComboDisciplinari_new.SelectedValue
                                            'End If
                                    End Select

                                Else
                                    ComboDisciplinari1.ddl_Disciplinari.SelectedIndex = 0
                                    objParametriAgenda.Disciplinare = "0"
                                    'ComboDisciplinari_new.SelectedIndex = 0
                                    'objParametriAgenda.Disciplinare = ComboDisciplinari_new.SelectedValue
                                    ViewState("ImpostoDPINellaCombo") = False
                                    Session("DpiPredefinitoUtente") = "0"
                                End If


                                'Filtro Prodotti  --> valore: 0=nessuno 1=coltura 2=coltura/avversita
                            Case enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_FILTRO_PRODOTTI

                                ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.SelectedIndex = _
                                        ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.Items.IndexOf(ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.Items.FindByValue( _
                                                Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")))


                                'ACQUA  --> valore: 88=totale 77=ettaro
                            Case enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_QTA_ACQUA

                                Select Case Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                                    Case "77"
                                        rblAcqua_HA.Checked = True
                                        rblAcqua_Tot.Checked = False
                                    Case Else
                                        rblAcqua_HA.Checked = False
                                        rblAcqua_Tot.Checked = True
                                End Select


                                'Blocco DPI
                            Case enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME
                                If Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1") = 1 Then
                                    Session("ControllaBlocco") = True
                                End If


                                'CONTROLLI DI ETICHETTA
                                'dose Massima Etichetta
                            Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSEMASSIMA_ETICHETTA
                                If Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1") = 1 Then
                                    Session("Blocca_Dose_Massima") = True
                                End If

                                'dose Minima Etichetta
                            Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSEMINIMA_ETICHETTA
                                If Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1") = 1 Then
                                    Session("Blocca_Dose_Minima") = True
                                End If

                                'Acqua Massima Etichetta
                            Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_ACQUAMASSIMA_ETICHETTA
                                If Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1") = 1 Then
                                    Session("Blocca_Acqua_Massima") = True
                                End If

                                'Acqua Minima Etichetta
                            Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_ACQUAMINIMA_ETICHETTA
                                If Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1") = 1 Then
                                    Session("Blocca_Acqua_Minima") = True
                                End If

                                'Numero Trattamento Massimo
                            Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_NUMERO_MASSIMO_TRATTAMENTI_ETICHETTA
                                If Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1") = 1 Then
                                    Session("Blocca_Numero_Massimo") = True
                                End If

                                'Carenza
                            Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_CARENZA_NON_RISPETTATA
                                If Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1") = 1 Then
                                    Session("Blocca_Carenza") = True
                                End If

                                'Fino a Fioritura
                            Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_FINO_FIORITURA_ETICHETTA
                                If Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1") = 1 Then
                                    Session("Blocca_Fioritura") = True
                                End If

                            Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_INTERVALLOTRATTAMENTI_ETICHETTA
                                If Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1") = 1 Then
                                    Session("Blocca_Intervallo_Minimo") = True
                                End If
                        End Select

                    Next

                End If

            End If

        ElseIf objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then
            'se sono in modifica imposto comunque il filtro prodotto-avversità se impostato

            'per non rendere bloccante in caso di errori, trattandosi di una impostazione non importante metto un trycatch

            Try

                If Cella_Avversita.Visible Then

                    Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                    Dim Dt_Impostazioni As DataTable

                    Dt_Impostazioni = ObjUtenti.Leggi(0, _
                                                      1, _
                                                      enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                                      "", _
                                                      "", _
                                                      objParametri_Utenti)


                    Session("ControllaBlocco") = False

                    If Dt_Impostazioni IsNot Nothing AndAlso Dt_Impostazioni.Rows.Count > 0 Then

                        For i = 0 To Dt_Impostazioni.Rows.Count - 1

                            Select Case CInt(Dt_Impostazioni.Rows(i).Item("Impostazione_Cod"))



                                'Filtro Prodotti  --> valore: 0=nessuno 1=coltura 2=coltura/avversita
                                Case enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_FILTRO_PRODOTTI

                                    ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.SelectedIndex = _
                                            ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.Items.IndexOf(ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.Items.FindByValue( _
                                                    Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")))



                                    'Blocco DPI
                                Case enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME
                                    If Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1") = 1 Then
                                        Session("ControllaBlocco") = True
                                    End If


                                    'CONTROLLI DI ETICHETTA
                                    'dose Massima Etichetta
                                Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSEMASSIMA_ETICHETTA
                                    If Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1") = 1 Then
                                        Session("Blocca_Dose_Massima") = True
                                    End If

                                    'dose Minima Etichetta
                                Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSEMINIMA_ETICHETTA
                                    If Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1") = 1 Then
                                        Session("Blocca_Dose_Minima") = True
                                    End If

                                    'Acqua Massima Etichetta
                                Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_ACQUAMASSIMA_ETICHETTA
                                    If Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1") = 1 Then
                                        Session("Blocca_Acqua_Massima") = True
                                    End If

                                    'Acqua Minima Etichetta
                                Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_ACQUAMINIMA_ETICHETTA
                                    If Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1") = 1 Then
                                        Session("Blocca_Acqua_Minima") = True
                                    End If

                                    'Numero Trattamento Massimo
                                Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_NUMERO_MASSIMO_TRATTAMENTI_ETICHETTA
                                    If Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1") = 1 Then
                                        Session("Blocca_Numero_Massimo") = True
                                    End If

                                    'Carenza
                                Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_CARENZA_NON_RISPETTATA
                                    If Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1") = 1 Then
                                        Session("Blocca_Carenza") = True
                                    End If

                                    'Fino a Fioritura
                                Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_FINO_FIORITURA_ETICHETTA
                                    If Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1") = 1 Then
                                        Session("Blocca_Fioritura") = True
                                    End If






                            End Select

                        Next



                    End If

                End If

            Catch ex As Exception

            End Try


        End If
    End Sub

    ''' <summary>
    ''' Verifica dei Permessi
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub VerificaPermessi()
        Dim UtenteAbilitato_Lettura As Boolean = False
        Dim UtenteAbilitato_Modifica As Boolean = False

        Dim objUtility As New AgronicaCoreModello.Utility_Operazioni

        objUtility.Verifica_Permessi_OperazioniAgenda_X_PagineAgronicaAgenda(objParametri_Server, objParametri_Utenti, UtenteAbilitato_Lettura, UtenteAbilitato_Modifica)

        Session("UtenteAbilitato_Lettura") = UtenteAbilitato_Lettura

        If Not UtenteAbilitato_Lettura Then
            Response.Redirect("~/classi/Agro_Pages/AccessoNonConsentito.aspx")
            Exit Sub
        End If

        Session("UtenteAbilitato_Modifica") = UtenteAbilitato_Modifica

        If objParametriAgenda.Tipo_Operazione <> enum_TipoOperazioneDB.Lettura AndAlso Not UtenteAbilitato_Modifica Then
            Response.Redirect("~/classi/Agro_Pages/AccessoNonConsentito.aspx")
            Exit Sub
        End If

    End Sub

    Private Sub MostraNascondiColonne_NessunoNessuno()
        Dim i As Integer = 0

        If ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.SelectedValue = "0" Then 'todo, non fare un select sulle descrizioni
            For i = 0 To GridView_Dosi.Columns.Count - 1
                Select Case GridView_Dosi.Columns(i).HeaderText
                    Case "Avversità / Gruppi Avversità"
                        GridView_Dosi.Columns(i).Visible = False
                    Case "Dose Etichetta"
                        GridView_Dosi.Columns(i).Visible = False
                    Case "Tempo di Carenza"
                        GridView_Dosi.Columns(i).Visible = False
                    Case "Data prima raccolta utile"
                        GridView_Dosi.Columns(i).Visible = False
                End Select
            Next
        Else

            'se non sono in modalità nessuno nessuno rimuovo questa possibilità
            If ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.Items.Count = 3 Then
                ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.Items.RemoveAt(2)
            End If

            For i = 0 To GridView_Dosi.Columns.Count - 1
                Select Case GridView_Dosi.Columns(i).HeaderText 'todo, non fare il case sulle descrizioni!
                    Case "Avversità / Gruppi Avversità"
                        GridView_Dosi.Columns(i).Visible = True
                    Case "Dose Etichetta"
                        GridView_Dosi.Columns(i).Visible = True
                    Case "Tempo di Carenza"
                        GridView_Dosi.Columns(i).Visible = True
                    Case "Data prima raccolta utile"
                        GridView_Dosi.Columns(i).Visible = True
                End Select
            Next
        End If

    End Sub

#End Region

    Private Sub caricaDtGiacenze()

        Dim Dr As DataRow

        Dim i As Integer

        Dim Filtro As String = ""

        Dim Dt_Magazzino As New DataTable
        aggiungiColonneLotti(Dt_Magazzino)

        'If objParametriAgenda.Fabbricato <> "0" Then

        'Applico il filtro sulla tipologia di semente e sulla specie
        Filtro = " AND Movimenti_dettagli.Elem_Cod IN (201,210) "
        Filtro = Filtro & " and Agenda.Id_Agenda <>  " & objParametriAgenda.Id_Agenda

        Dim FiltroMateriePrime As String = " AND Materie_Prime.Veg_Cod = " & Agro_SQL_SaveNum(objParametriAgenda.Veg_Cod)

        Dim objGIACENZE As New AgronicaCoreContabDAL.Giacenze_R

        Dim Giacenze As DataTable

        'caso solito
        Giacenze = objGIACENZE.SchedaGiacenzeMagazzino(objParametriAgenda.Data,
                        objParametriAgenda.Piva,
                        CInt(objParametriAgenda.Sa_Cod),
                        0,
                        0,
                        0,
                        0,
                        0, 0, 0, 0, LOTTO_NONDEFINITO,
                        True,
                        Filtro, "", "", "", "", "", "", "", "", FiltroMateriePrime, FiltroMateriePrime,
                        "",
                        objParametri_Server, objParametri_Utenti, FiltroMateriePrime)

        If Giacenze IsNot Nothing AndAlso Giacenze.Rows.Count > 0 Then

            For i = 0 To Giacenze.Rows.Count - 1

                'Creo una nuova riga
                Dr = Dt_Magazzino.NewRow

                'Definisco i valori
                Dr.Item("Piva") = Giacenze.Rows(i).Item("Piva")
                Dr.Item("Sa_Cod") = Giacenze.Rows(i).Item("Sa_Cod")
                Dr.Item("Id_Destinazione") = Giacenze.Rows(i).Item("Id_Destinazione")
                Dr.Item("Fabbricato_Des") = Giacenze.Rows(i).Item("Fabbricato_Des")

                Dr.Item("Descrizione_Prodotto") = Giacenze.Rows(i).Item("Descrizione_Prodotto")

                Dr.Item("Elem_Cod") = Giacenze.Rows(i).Item("Elem_Cod")
                Dr.Item("Mat_Cod") = Giacenze.Rows(i).Item("Mat_Cod")
                Dr.Item("Cod_Progetto") = Giacenze.Rows(i).Item("Cod_Progetto")
                Dr.Item("Cal_Cod") = Giacenze.Rows(i).Item("Cal_Cod")
                Dr.Item("Cod_Articolo") = Giacenze.Rows(i).Item("Cod_Articolo")
                Dr.Item("Lotto") = Giacenze.Rows(i).Item("Lotto")

                Dr.Item("Udm_Cod") = enum_UnitaMisura.Quintali ' Giacenze.Rows(i).Item("Udm_Cod")
                Dr.Item("Giacenza") = Giacenze.Rows(i).Item("Giacenza") / 100 '& " " & Giacenze.Rows(i).Item("Udm_Sim")
                'Dr.Item("Udm_Sim") = Giacenze.Rows(i).Item("Udm_Sim")
                Dr.Item("Qta") = Giacenze.Rows(i).Item("Giacenza") / 100

                Dt_Magazzino.Rows.Add(Dr)

            Next

        End If

        'Else

        '    Me.CheckBoxGiacenzePositive.Visible = False

        'End If

        GridViewMagazzino.DataSource = Dt_Magazzino
        GridViewMagazzino.DataBind()

    End Sub

    Private Sub aggiungiColonneLotti(ByRef Dt As DataTable)

        Dt.Columns.Add(New DataColumn("Piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Id_Destinazione", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Fabbricato_Des", GetType(String)))

        Dt.Columns.Add(New DataColumn("Descrizione_Prodotto", GetType(String)))
        Dt.Columns.Add(New DataColumn("Elem_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Mat_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Cod_Progetto", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Cal_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Udm_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Cod_Articolo", GetType(String)))
        Dt.Columns.Add(New DataColumn("Lotto", GetType(String)))

        Dt.Columns.Add(New DataColumn("Giacenza", GetType(String)))
        Dt.Columns.Add(New DataColumn("Udm_Sim", GetType(String)))
        Dt.Columns.Add(New DataColumn("Qta", GetType(String)))

    End Sub


    Protected Sub btn_cerca_Click(sender As Object, e As EventArgs) Handles btn_cerca.Click

        'End Sub


        ''' <summary>
        ''' Cerca Formulato
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        'Protected Sub ImgBtn_Cerca_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Cerca.Click

        'cancello la giacenza precedente
        Lbl_Giacenza.Text = ""
        Lbl_Dose_Etichetta.Text = ""
        Lbl_Dose_Consigliata.Text = ""
        Lbl_FormulatoInRevisione.InnerText = ""

        Cmb_UdM.Items.Clear()

        ComboFormulati.Veg_Cod = objParametriAgenda.Veg_Cod.Split("/")(0)

        ComboFormulati.Disciplinare_Cod = objParametriAgenda.Disciplinare
        'ComboFormulati.PrimaRiga_Flag = true

        ComboFormulati.TipoRichiesto = enum_TipoFormulato.Tutti

        ComboFormulati.TestoRicerca = Txt_Formulati.Text
        ComboFormulati.Flag_PrincipiAttivi = True

        ComboFormulati.Piva = objParametriAgenda.Piva
        ComboFormulati.Fabbricato_Cod = objParametriAgenda.Fabbricato
        ComboFormulati.Cau_Mov = CAU_SCARICO
        ComboFormulati.Validita_Fine = objParametriAgenda.Data
        ComboFormulati.Validita_Inizio = objParametriAgenda.Data

        ComboFormulati.WS_Disciplinari_AgroWS_Disciplinari = objParametriAgenda.WS_Disciplinari_AgroWS_Disciplinari
        ComboFormulati.WS_Fitofarmaci_AgroWS_Fitofarmaci = objParametriAgenda.WS_Fitofarmaci_AgroWS_Fitofarmaci

        Dim Valore_Ricerca As String
        Valore_Ricerca = ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.SelectedValue

        ComboFormulati.Opt_Avversita_Infestanti = 0

        Select Case Valore_Ricerca

            Case "0" 'nessuno
                ComboFormulati.FiltroRicerca = 0

            Case "1" 'Prodotti --> Avversità
                ComboFormulati.Opt_Singola_Gruppo = -1
                ComboFormulati.FiltroRicerca = 1

            Case "2" 'Avversità --> Prodotti
                ComboFormulati.FiltroRicerca = 2

                ComboFormulati.Opt_Avversita_Infestanti = 0

                Dim Array_AvCod(0) As Integer
                Dim Array_AvGru(0) As Integer

                Dim strAvversita As String = ""
                Dim NumAvv As Integer = 0

                For i = 0 To GridViewAvversita.Rows.Count - 1
                    If CType(GridViewAvversita.Rows(i).FindControl("ChkSelezionaAvversita"), CheckBox).Checked = True Then
                        If IsNumeric(GridViewAvversita.Rows(i).Cells(1).Text) AndAlso CInt(GridViewAvversita.Rows(i).Cells(1).Text) <> 0 Then
                            ComboFormulati.Opt_Singola_Gruppo = 0
                        Else
                            ComboFormulati.Opt_Singola_Gruppo = 1
                        End If
                        ReDim Preserve Array_AvCod(NumAvv)
                        Array_AvCod(NumAvv) = GridViewAvversita.Rows(i).Cells(1).Text
                        ReDim Preserve Array_AvGru(NumAvv)
                        Array_AvGru(NumAvv) = GridViewAvversita.Rows(i).Cells(2).Text
                        NumAvv += 1
                        ComboFormulati.Opt_Avversita_Infestanti = CInt(GridViewAvversita.Rows(i).Cells(4).Text)
                    End If
                Next

                If NumAvv <> 0 Then
                    If strAvversita <> "" Then
                        strAvversita = Left(strAvversita, strAvversita.Length - 4)
                        ComboFormulati.StrAvversita = strAvversita
                    End If
                Else
                    Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.SeSiSelezionaIlFiltroAvversitaProdottiOcco, _
                                       Page, , updateDoseInserisci)
                    Exit Sub
                End If

                ComboFormulati.Av_Cod = Array_AvCod
                ComboFormulati.Av_Gru = Array_AvGru


        End Select



        ComboFormulati.Flag_ClasseTossicologica = True
        ComboFormulati.CaricaComboFormulati()

        Lbl_Num_Formulati.InnerText = String.Format(Resources.AgronicaAgenda_2010.TrovatiX0Formulati, ComboFormulati.N_Formulati)

        'se ho caricato un solo formulato preseleziono quello
        'gesticco eccezione per non bloccare
        If ComboFormulati.N_Formulati = 1 Then
            Try
                ComboFormulati.ddl_Formulati.SelectedIndex = 1
                Cambiato_Formulato()
            Catch ex As Exception

            End Try

        End If

    End Sub

    Private Sub BTN_ComboFormulati_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BTN_ComboFormulati.Click


        Cambiato_Formulato()

    End Sub


    Private Sub Cambiato_Formulato()

        Dim FrCod As Integer = 0
        Dim i As Integer = 0
        Dim UdmCod As Integer = 0
        Dim UdmDes, Udm_Sim As String
        Dim QtaPresente As String = "0"

        Dim DoseMin, DoseMax As Decimal
        Dim Udm_Cod As Integer = 0
        Dim AcquaMin, AcquaMax As Decimal
        Dim AcquaUdm_Cod As Integer = 0
        Dim AcquaUdm_Sim As String
        Dim A_Epoca As Integer = 0
        Dim Da_Epoca As Integer = 0
        Dim Limite As Integer = 0
        Dim LimiteUdm_Cod As Integer = 0

        Dim Intervallo_Min, Intervallo_Max As Integer
        Dim LimiteUdm_Sim As String

        Dim DoseEtichetta As String = ""

        Me.Cmb_UdM.SelectedIndex = -1

        Me.Txt_Formulati.Text = ""
        Me.Lbl_Dose_Etichetta.Text = ""
        Me.Lbl_Dose_Etichetta.Visible = False
        Me.Lbl_Dose_Consigliata.Text = ""
        Me.Lbl_Dose_Consigliata.Visible = False
        lbl_qta_residua.Visible = False

        Me.Txt_Dose_HA.Text = ""
        Me.Txt_Dose_HL.Text = ""
        Me.Txt_DoseTot_HA.Text = ""
        Me.Lbl_Giacenza.Text = ""
        Me.Lbl_Num_Formulati.InnerText = ""
        Lbl_FormulatoInRevisione.InnerText = ""

        Dim InRevisione As String = ""
        Dim DataAttoNormativo As String = ""

        Resettasession_Etichetta()

        CaricaComboUnitadiMisura()

        Dim ArrayTmp() As String

        If ComboFormulati.Valore_Combo <> "" Then

            ArrayTmp = Split(ComboFormulati.Valore_Combo, "£")

            If ArrayTmp IsNot Nothing Then
                FrCod = ArrayTmp(0)
            End If

            DataAttoNormativo = ""
            InRevisione = ""

            If ArrayTmp.Length > 1 Then
                If ArrayTmp(1) <> "" Then
                    InRevisione = ArrayTmp(1)
                End If
            End If

            If ArrayTmp.Length > 2 Then
                If ArrayTmp(2) <> "" Then
                    DataAttoNormativo = ArrayTmp(2)
                End If
            End If
            If InRevisione <> "" Then
                If DataAttoNormativo <> "" Then
                    Lbl_FormulatoInRevisione.InnerText = "ATTENZIONE!!! Il prodotto selezionato è in fase di revisione da Decreto del " & DataAttoNormativo & "!Si consiglia di riferirsi ai dosaggi della nuova etichetta consultando Profitosan!"
                Else
                    Lbl_FormulatoInRevisione.InnerText = "ATTENZIONE!!! Il prodotto selezionato è in fase di revisione!Si consiglia di riferirsi ai dosaggi della nuova etichetta consultando Profitosan!"
                End If
            End If

            Select Case objParametriAgenda.Fabbricato

                Case Is <> "0"

                    Dim objGia As New AgronicaCoreContabDAL.Giacenze_R

                    If True Then
                        'Leggo giacenze alla data dell'operazione

                        Dim Dt_Giacenze As DataTable

                        Dt_Giacenze = objGia.SchedaGiacenzeMagazzino(objParametriAgenda.Data, _
                                                                     Split(objParametriAgenda.Fabbricato, "|")(2), _
                                                                     Split(objParametriAgenda.Fabbricato, "|")(1), _
                                                                     Split(objParametriAgenda.Fabbricato, "|")(0), _
                                                                     FORMULATI, _
                                                                     FrCod, _
                                                                     0, _
                                                                     0, 0, 0, 0, LOTTO_NONDEFINITO, _
                                                                     False, _
                                                                     "", "", "", "", "", "", "", "", "", "", "", _
                                                                     "", _
                                                                     objParametri_Server, objParametri_Utenti)

                        Dim Giacenze As New Hashtable
                        If Not IsNothing(Dt_Giacenze) AndAlso Dt_Giacenze.Rows.Count > 0 Then

                            For i = 0 To Dt_Giacenze.Rows.Count - 1

                                'ignoro le giacenze infinitesime
                                If Dt_Giacenze.Rows(i).Item("Giacenza") <> 0 And Not (Dt_Giacenze.Rows(i).Item("Giacenza") < QTA_GiancenzeVisualizzate And Dt_Giacenze.Rows(i).Item("Giacenza") > -QTA_GiancenzeVisualizzate) Then

                                    UdmCod = Dt_Giacenze.Rows(i).Item("Udm_Cod")
                                    UdmDes = CStr(Dt_Giacenze.Rows(i).Item("Udm_Des"))

                                    If Not IsDBNull(UdmCod) Then
                                        'non aggiungo se sono in tipologia formulato
                                        'If Cmb_FormulatoClassificazioni.SelectedValue <> 1 Then
                                        If IsNothing(Cmb_UdM.Items.FindByValue(UdmCod)) Then
                                            If UdmCod = 29 Then
                                                CaricaLitri(UdmCod)
                                                'Cmb_UdM.Items.Add(New ListItem(UdmDes, UdmCod))
                                            End If
                                            If UdmCod = 2 Then
                                                CaricaKG(UdmCod)
                                            End If

                                        End If


                                    End If

                                    QtaPresente = Math.Round(Dt_Giacenze.Rows(i).Item("Giacenza"), 4).ToString
                                    Giacenze.Add(CStr(Dt_Giacenze.Rows(i).Item("Udm_Cod")), QtaPresente)

                                End If

                            Next
                        End If



                        Session("HashGiacenze") = Giacenze
                    End If


                    If True Then
                        'Leggo giacenze totali

                        Dim Dt_GiacenzeTotali As DataTable

                        Dt_GiacenzeTotali = objGia.SchedaGiacenzeMagazzino(AGRODATAFINE, _
                                                                     Split(objParametriAgenda.Fabbricato, "|")(2), _
                                                                     Split(objParametriAgenda.Fabbricato, "|")(1), _
                                                                     Split(objParametriAgenda.Fabbricato, "|")(0), _
                                                                     FORMULATI, _
                                                                     FrCod, _
                                                                     0, _
                                                                     0, 0, 0, 0, LOTTO_NONDEFINITO, _
                                                                     False, _
                                                                     "", "", "", "", "", "", "", "", "", "", "", _
                                                                     "", _
                                                                     objParametri_Server, objParametri_Utenti)

                        Dim GiacenzeTotali As New Hashtable
                        If Not IsNothing(Dt_GiacenzeTotali) AndAlso Dt_GiacenzeTotali.Rows.Count > 0 Then

                            For i = 0 To Dt_GiacenzeTotali.Rows.Count - 1

                                'ignoro le giacenze infinitesime
                                If Dt_GiacenzeTotali.Rows(i).Item("Giacenza") <> 0 And Not (Dt_GiacenzeTotali.Rows(i).Item("Giacenza") < QTA_GiancenzeVisualizzate And Dt_GiacenzeTotali.Rows(i).Item("Giacenza") > -QTA_GiancenzeVisualizzate) Then

                                    UdmCod = Dt_GiacenzeTotali.Rows(i).Item("Udm_Cod")
                                    UdmDes = CStr(Dt_GiacenzeTotali.Rows(i).Item("Udm_Des"))

                                    If Not IsDBNull(UdmCod) Then
                                        'non aggiungo se sono in tipologia formulato
                                        'If Cmb_FormulatoClassificazioni.SelectedValue <> 1 Then
                                        If IsNothing(Cmb_UdM.Items.FindByValue(UdmCod)) Then
                                            If UdmCod = 29 Then
                                                CaricaLitri(UdmCod)
                                                'Cmb_UdM.Items.Add(New ListItem(UdmDes, UdmCod))
                                            End If
                                            If UdmCod = 2 Then
                                                CaricaKG(UdmCod)
                                            End If

                                        End If
                                        'End If

                                    End If

                                    QtaPresente = Math.Round(Dt_GiacenzeTotali.Rows(i).Item("Giacenza"), 4).ToString
                                    GiacenzeTotali.Add(CStr(Dt_GiacenzeTotali.Rows(i).Item("Udm_Cod")), QtaPresente)

                                End If

                            Next
                        End If



                        Session("HashGiacenzeTotali") = GiacenzeTotali
                    End If

                    aggiornaGiacenzaPerUdm()

            End Select



            '----------------------------------------------------------

            Select Case ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.SelectedValue

                Case "0" 'NESSUN FILTRO

                    Me.Lbl_Dose_Etichetta.Text = Resources.AgronicaAgenda_2010.BrNonDisponibile
                    Me.Lbl_Dose_Consigliata.Text = Resources.AgronicaAgenda_2010.BrNonDisponibile

                    Session("DoseEtichetta") = Resources.AgronicaAgenda_2010.BrNonDisponibile
                    Session("DoseMax") = 0

                    Txt_Dose_HA.Text = "0"
                    Txt_Dose_HL.Text = "0"


                Case "1" 'COLTURA

                    CaricaGriglia_Avversita(FrCod)

                Case Else 'COLTURA / AVVERSITA


                    If ArrayTmp IsNot Nothing Then

                        DoseEtichetta = ""
                        Dim Flag_Fioritura As Integer = 0

                        If ArrayTmp.Length > 1 Then

                            For i = 6 To ArrayTmp.Length - 1

                                Dim ArrayDose() As String
                                ArrayDose = Split(ArrayTmp(i), "$")

                                If ArrayDose IsNot Nothing AndAlso ArrayDose.Length > 0 Then

                                    DoseMin = ArrayDose(1)
                                    DoseMax = ArrayDose(2)
                                    Udm_Cod = ArrayDose(3)
                                    Udm_Sim = ArrayDose(4)

                                    AcquaMin = ArrayDose(5)
                                    AcquaMax = ArrayDose(6)
                                    AcquaUdm_Cod = ArrayDose(7)
                                    AcquaUdm_Sim = ArrayDose(8)

                                    Da_Epoca = ArrayDose(9)
                                    A_Epoca = ArrayDose(10)

                                    Limite = ArrayDose(11)
                                    LimiteUdm_Cod = ArrayDose(12)
                                    LimiteUdm_Sim = ArrayDose(13)

                                    Flag_Fioritura = ArrayDose(15)
                                    Intervallo_Min = ArrayDose(16)
                                    Intervallo_Max = ArrayDose(17)

                                    DoseEtichetta &= CreaStringa_DoseEtichetta(DoseMin, DoseMax, Udm_Sim, Udm_Cod, _
                                                                  AcquaMin, AcquaMax, AcquaUdm_Sim, AcquaUdm_Cod, _
                                                                  Limite, LimiteUdm_Sim, LimiteUdm_Cod, _
                                                                  Da_Epoca, A_Epoca, Flag_Fioritura, _
                                                                  Intervallo_Min, Intervallo_Max)

                                    '----
                                    'Acqua
                                    Imposta_VolumiAcqua(AcquaMin, AcquaMax, AcquaUdm_Cod)
                                    '----------------
                                    'DOSE

                                    Imposta_DosiEtichetta(DoseEtichetta)

                                End If

                            Next

                            If DoseEtichetta <> "" Then
                                DoseEtichetta = Left(DoseEtichetta, DoseEtichetta.Length - 4)
                            End If

                        End If


                    End If

                    Imposta_DosiEtichetta(DoseEtichetta)
                    Lbl_Dose_Etichetta.Text = DoseEtichetta 'vanni, 23 apr 13 - impostata dose su label
                    Lbl_Dose_Etichetta.Visible = True


            End Select

        End If

        'aggiungo blocco e sblocco

        Dim script As New StringBuilder
        script.AppendLine("$(document).ready(function () { ")
        script.AppendLine("     BloccaSbloccaTotale();")
        script.AppendLine("}); ")

        ScriptManager.RegisterStartupScript(UpdatePanelMiscela, UpdatePanelMiscela.GetType(),
                                         String.Format("jQuery_{0}", UpdatePanelMiscela.ClientID), script.ToString, True)


        Post_CaricamentoEtichetta_AggiornaCaselleTesto()

        aggiornaGiacenzaPerUdm()

    End Sub

    Private Sub Post_CaricamentoEtichetta_AggiornaCaselleTesto()

        Dim strJS As New StringBuilder
        strJS.AppendLine("$(document).ready(function () { ")
        If Txt_Acqua_Ha.Text <> "0" AndAlso Txt_Acqua_Ha.Text <> "" Then
            strJS.AppendLine("      AcquaHA_Keyup();  ")
        End If
        strJS.AppendLine("      AggiornaDOSI();  ")
        strJS.AppendLine(" });")
        ScriptManager.RegisterStartupScript(CType(Page.Master, Operazione).Property_UpdatePanelPerScript, _
                                            CType(Page.Master, Operazione).Property_UpdatePanelPerScript.GetType(),
                                      String.Format("jQuery_{0}", CType(Page.Master, Operazione).Property_UpdatePanelPerScript.ClientID), _
                                      strJS.ToString, True)

    End Sub

    Protected Sub EventoAggiornamentoAvversita_Click(sender As Object, e As EventArgs) Handles EventoAggiornamentoAvversita.Click

        'Resettasession_Etichetta()

        If ComboFiltriAggiuntiviAgenda1.Valore_Combo = 1 AndAlso ComboFormulati.Valore_Combo.Length > 0 Then

            Dim DoseEtichetta As String
            Dim Udm_Cod As Integer = 0
            Dim DoseMin, DoseMax As Decimal
            Dim Intervallo_min, Intervallo_Max As Integer
            Dim Udm_Sim As String
            Dim DtDosi As DataTable = Nothing
            Dim AcquaMin, AcquaMax As Decimal
            Dim AcquaUdm_Cod As Integer = 0
            Dim AcquaUdm_Sim As String
            Dim A_Epoca As Integer = 0
            Dim Da_Epoca As Integer = 0
            Dim Limite As Integer = 0
            Dim LimiteUdm_Sim As String

            Dim Flag_Fioritura As Integer = 0

            Dim Fr_Cod As Integer = ComboFormulati.Valore_Combo.Split("£")(0)
            Dim Veg_Cod As Integer = objParametriAgenda.Veg_Cod.Split("/")(0)
            Dim Av_Cod As Integer = 0
            Dim Av_Gru As Integer = 0
            Dim i As Integer = 0

            Resettasession_Etichetta()
            CaricaComboUnitadiMisura()

            Lbl_FormulatoInRevisione.InnerText = ""

            Dim Opt_Avversita_Infestanti As Integer = 0

            For i = 0 To GridViewAvversita.Rows.Count - 1
                If CType(GridViewAvversita.Rows(i).FindControl("ChkSelezionaAvversita"), CheckBox).Checked = True Then
                    Av_Cod = CInt(GridViewAvversita.Rows(i).Cells(1).Text)
                    Av_Gru = CInt(GridViewAvversita.Rows(i).Cells(2).Text)
                    Opt_Avversita_Infestanti = CInt(GridViewAvversita.Rows(i).Cells(4).Text)
                    Exit For
                End If
            Next
            If Av_Cod = 0 AndAlso Av_Gru = 0 Then
                Exit Sub
            End If


            Lbl_Dose_Etichetta.Text = Resources.AgronicaAgenda_2010.NonDisponibile

            Dim grfi_cod As Integer = 0

            Dim Udm_Da_Magazzino As Integer
            'controllo se ho una unità di misura da magazzino
            Dim Giacenze As Hashtable = Session("HashGiacenze")
            Dim GiacenzeTotali As Hashtable = Session("HashGiacenzeTotali")
            'If Not IsNothing(Cmb_UdM) AndAlso Cmb_UdM.Items.Count > 0 Then
            If Not IsNothing(Giacenze) AndAlso Giacenze.Keys.Count = 1 Then
                'identifico kg o litri
                Udm_Da_Magazzino = Giacenze.Keys(0)
            Else
                Udm_Da_Magazzino = -99
            End If



            If Veg_Cod <> 0 Then

                Select Case Opt_Avversita_Infestanti
                    Case 0
                        DtDosi = Formulati_SpecieVegetali_Avversita_Dosi(Fr_Cod, Veg_Cod, Av_Cod, Av_Gru, grfi_cod)
                    Case 1
                        DtDosi = Formulati_SpecieVegetali_Infestanti_Dosi(Fr_Cod, Veg_Cod, Av_Cod, Av_Gru, grfi_cod)
                End Select

                'imposto i valori trovati
                If DtDosi IsNot Nothing Then

                    If DtDosi.Rows.Count > 0 Then

                        For i = 0 To DtDosi.Rows.Count - 1

                            DoseMin = Math.Round(CDbl(DtDosi.Rows(i).Item("Dose_Min")), 4)
                            DoseMax = Math.Round(CDbl(DtDosi.Rows(i).Item("Dose_Max")), 4)
                            Udm_Cod = CInt(DtDosi.Rows(i).Item("Udm_Cod"))
                            Udm_Sim = DtDosi.Rows(i).Item("Udm_Sim")

                            AcquaMin = CDbl(DtDosi.Rows(i).Item("Acqua_Min"))
                            AcquaMax = CDbl(DtDosi.Rows(i).Item("Acqua_Max"))
                            AcquaUdm_Cod = CDbl(DtDosi.Rows(i).Item("Acqua_Udm_Cod"))
                            If AcquaUdm_Cod <> 0 Then
                                Dim objConverti As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
                                objConverti.UdmDes_from_UdmCod(AcquaUdm_Cod, AcquaUdm_Sim, objParametri_Server)
                                'AcquaUdm_Sim = ArrayDose(7)
                            End If

                            Intervallo_min = CInt(DtDosi.Rows(i).Item("IntervalloTrattamenti_Min"))
                            Intervallo_Max = CInt(DtDosi.Rows(i).Item("IntervalloTrattamenti_Max"))


                            Da_Epoca = DtDosi.Rows(i).Item("Da_Epoca_1")
                            A_Epoca = DtDosi.Rows(i).Item("a_Epoca_1")

                            Limite = DtDosi.Rows(i).Item("Limiteinterventi")
                            Dim objUDMMetaschema As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
                            LimiteUdm_Sim = objUDMMetaschema.UdmDes_from_UdmCod(DtDosi.Rows(i).Item("udm_cod_limite"), "", _
                                           objParametri_Server)

                            Flag_Fioritura = DtDosi.Rows(i).Item("Epoca_Cod")


                            DoseEtichetta &= CreaStringa_DoseEtichetta(DoseMin, DoseMax, Udm_Sim, Udm_Cod, _
                                                      AcquaMin, AcquaMax, AcquaUdm_Sim, AcquaUdm_Cod, _
                                                      Limite, LimiteUdm_Sim, DtDosi.Rows(i).Item("udm_cod_limite"), _
                                                      Da_Epoca, A_Epoca, Flag_Fioritura, _
                                                      Intervallo_min, Intervallo_Max)


                            'If DoseEtichetta <> "" Then
                            '    DoseEtichetta = Left(DoseEtichetta, DoseEtichetta.Length - 4)
                            'End If

                            '----
                            'Acqua
                            Imposta_VolumiAcqua(AcquaMin, AcquaMax, AcquaUdm_Cod)


                            Dim Udm_Radice, per_ha_hl As Integer
                            ScomponiUdm(Udm_Radice, per_ha_hl, CInt(DtDosi.Rows(i).Item("Udm_Cod")))
                            If Udm_Da_Magazzino <> -99 Then
                                Dim v As Integer
                                Select Case Udm_Radice
                                    Case 3, 2032, 2, 304, 4
                                        v = 2
                                        'acqua
                                    Case 104, 101, 29
                                        v = 29
                                End Select

                                If Udm_Da_Magazzino = v Then
                                    Imposta_DosiEtichetta(DoseEtichetta)
                                End If
                            Else
                                Imposta_DosiEtichetta(DoseEtichetta)
                            End If
                        Next

                        If DoseEtichetta <> "" Then
                            DoseEtichetta = Left(DoseEtichetta, DoseEtichetta.Length - 4)
                        End If

                        ''----
                        ''Acqua
                        'Imposta_VolumiAcqua(AcquaMin, AcquaMax, AcquaUdm_Cod)

                        'Imposta_DosiEtichetta(DoseEtichetta)

                    Else

                        DoseMin = 0
                        DoseMax = 0
                        Udm_Cod = 0
                        Session("DoseEtichetta") = Lbl_Dose_Etichetta.Text

                        Dim objDPILeggi As New AgronicaCoreDpiBIZ.Fitofarmaci_Leggi
                        objDPILeggi.Recupera_UdM_da_FrCod(Nothing, _
                                                            Fr_Cod, _
                                                            Udm_Cod, _
                                                            Nothing, _
                                                            Nothing, _
                                                            Session)

                        Select Case Udm_Cod
                            Case 2
                                CaricaKG(Udm_Cod)
                            Case 29
                                CaricaLitri(Udm_Cod)
                        End Select

                    End If

                    Lbl_Dose_Etichetta.Visible = True

                End If

            End If



            'dosi multi 
            Dim kg As Boolean = False
            Dim l As Boolean = False
            If DtDosi.Rows.Count > 0 Then

                If Not IsNothing(Giacenze) AndAlso Giacenze.Keys.Count = 1 Then
                    'carico solo le udm indicate da magazzino
                Else
                    For i = 0 To DtDosi.Rows.Count - 1
                        'controllo se è sempre lo stesso
                        Dim Udm_Radice, per_ha_hl As Integer
                        ScomponiUdm(Udm_Radice, per_ha_hl, CInt(DtDosi.Rows(i).Item("Udm_Cod")))

                        Select Case Udm_Radice
                            Case 3, 2032, 2, 304, 4
                                kg = True
                                'acqua
                            Case 104, 101, 29
                                l = True
                        End Select
                    Next

                    If kg AndAlso l Then
                        'preseleziono il valore precedente
                        Dim valSel As Integer = Cmb_UdM.SelectedValue
                        Cmb_UdM.Items.Clear()
                        'ricarico con tutte le unita di misura
                        CaricaKGeLitri(valSel)
                    End If
                End If



            End If


        End If


        Dim strJS As New StringBuilder
        strJS.AppendLine("$(document).ready(function () { ")
        strJS.AppendLine("      BloccaSbloccaTotale(); ")
        strJS.AppendLine("      $('#div_pannello_avversita_scroll').scrollTop(Y); ")

        'strJS.AppendLine("      SelezioneChkSelezionaAvversita(); ")
        strJS.AppendLine(" });")
        ScriptManager.RegisterClientScriptBlock(UpdatePanelAvversita, UpdatePanelAvversita.GetType(),
                                      String.Format("jQuery_{0}", UpdatePanelAvversita.ClientID), strJS.ToString, True)

        Post_CaricamentoEtichetta_AggiornaCaselleTesto()

    End Sub

    Private Sub CaricaKGeLitri(ByVal unitaMisuraSelezionata As Integer)

        Cmb_UdM.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Grammi, 3)) 'todo, indicare udm da codice.
        Cmb_UdM.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Chilogrammi, 2))
        Cmb_UdM.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Quintali, 4))
        Cmb_UdM.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Tonnellate, 304))

        Cmb_UdM.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Millilitri, 101))
        Cmb_UdM.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.CentimetriCubi, 104))
        Cmb_UdM.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Litri, 29))

        Cmb_UdM.SelectedIndex = Cmb_UdM.Items.IndexOf(Cmb_UdM.Items.FindByValue(unitaMisuraSelezionata))

        aggiornaGiacenzaPerUdm()

    End Sub

    Protected Sub ImgBtn_DoseInserisci_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_DoseInserisci.Click

        Dim FrCod As String = ""
        Dim FrDes As String = ""
        Dim Carenza As Decimal = -1

        Dim Dose As Decimal
        Dim Dose_Tot As Decimal
        Dim UdmCod As Integer = 0
        Dim UdmCodTrasformato As Integer = 0
        Dim UdmDes As String = ""
        Dim UdmDesSimbolo As String = ""

        Dim Av_Cod As String = ""
        Dim Av_Gru As String = ""
        Dim Av_Des As String = ""

        Dim Dose_Etichetta As String = ""
        Dim Dose_Etichetta_Max As String = ""

        Dim Soglia_Value As String
        Dim Soglia_Des As String

        Dim strPA_COD As String = ""
        Dim strCLTOSS_COD As String = ""

        'controllo sulle giacenze di magazzino
        If ControllaGiacenzaMagazzino() = False Then
            Exit Sub
        End If



        'Controlli Vari
        '-----
        If ControllaDose(Dose, Dose_Tot) = False Then
            Exit Sub
        End If

        'controllo la qta di Acqua
        If ControllaAcqua() = False Then
            Exit Sub
        End If




        '--------
        If ControllaSelezioneFormulato(FrCod, FrDes, Carenza, strPA_COD, strCLTOSS_COD) = False Then
            Exit Sub
        End If
        '-----
        If ControllaSelezioneAvversita(Av_Cod, Av_Gru, Av_Des) = False Then
            Exit Sub
        End If
        '-----
        If ControllaSelezioneUnitadiMisura(UdmCod, UdmDesSimbolo, UdmDes, UdmCodTrasformato) = False Then
            Exit Sub
        End If
        '-----

        ''controllo Numero Trattamenti
        'If ControllaNumeroMaxTrattamenti(FrCod) = False Then
        '    Exit Sub
        'End If

        ''controllo Intervallo Trattamenti
        'If ControllaIntervalloTrattamenti(FrCod) = False Then
        '    Exit Sub
        'End If

        'controllo sulla fine fioritura
        'If ControllaFinoaFioritura() = False Then
        '    Exit Sub
        'End If
        ScriptManager.RegisterStartupScript(UpdateProgress2, UpdateProgress2.GetType, "azzera", _
                                                "$(document).ready(function () {$('#" & HiddenVarie.ClientID & "').val(''); });", True)



        'Disabilito il RadioButton
        'lascio libera la selezione mi salvo solamente l'ultima impostazione
        If Session("DoseEtichetta") IsNot Nothing Then
            Dose_Etichetta = Session("DoseEtichetta")
        End If
        If Session("DoseMax") IsNot Nothing Then
            Dose_Etichetta_Max = Session("DoseMax")
        End If

        '------
        Dim Prima_Data As String
        If IsNumeric(Carenza.ToString) AndAlso CInt(Carenza) > 0 Then
            Dim data As Date
            data = objParametriAgenda.Data.AddDays(1)
            Prima_Data = data.AddDays(CInt(Carenza.ToString)).ToShortDateString
        Else
            Prima_Data = ""
        End If

        Dim N_Trattamenti_Max, N_Trattamenti_Umd_Cod As Integer
        Dim N_Trattamenti_UDM_Sim As String
        N_Trattamenti_Max = Session("N_Trattamenti_Max")
        N_Trattamenti_Umd_Cod = Session("N_Trattamenti_Umd_Cod")
        N_Trattamenti_UDM_Sim = Session("N_Trattamenti_UDM_Sim")

        Dim IntervalloTrattamenti_Min As Integer = Session("IntervalloTrattamenti_Min")
        Dim IntervalloTrattamenti_Max As Integer = Session("IntervalloTrattamenti_Max")



        MostraNascondiColonne_NessunoNessuno()
        'Inserisco il formulato nella griglia delle dosi
        Call Dosi_Inserisci(FrCod, FrDes, Dose, _
                            Txt_Dose_HA.Text, _
                            Txt_Dose_HL.Text, _
                            Dose_Tot, UdmCod, UdmDes, UdmCodTrasformato, _
                            Carenza.ToString, Prima_Data, _
                            Av_Cod, Av_Gru, Av_Des, _
                            Dose_Etichetta, Dose_Etichetta_Max, _
                            N_Trattamenti_Max, N_Trattamenti_UDM_Sim, N_Trattamenti_Umd_Cod, _
                            Soglia_Value, Soglia_Des, IntervalloTrattamenti_Min, IntervalloTrattamenti_Max, _
                            strPA_COD, strCLTOSS_COD)

        'dopo che ho inserito il primo formulato non posso più modificare il magazzino
        'CType(Page.Master.FindControl("ComboMagazzini"), AgronicaControlli_2010.ComboMagazzini).Enabled = True
        Griglia_Dosi_Data_Bind()

        Dim script As New StringBuilder


        script.AppendLine("$(document).ready(function () { ")
        script.AppendLine("     BloccaSbloccaTotale();")
        If ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.SelectedValue <> "0" Then
            script.AppendLine("     PulisciGrigliaAv_GrAv();")
        End If
        script.AppendLine("}); ")


        ScriptManager.RegisterStartupScript(UpdatePanelMiscela, UpdatePanelMiscela.GetType(),
                                         String.Format("jQuery_{0}", UpdatePanelMiscela.ClientID), script.ToString, True)



        'attivo la verifica del disciplinare solo se ho un disciplinare selezionato 
        If objParametriAgenda.Disciplinare <> "0" AndAlso objParametriAgenda.Disciplinare <> "-2" Then
            'CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_CheckDPI"), ImageButton).Visible = True
            CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_Disciplinare"), ImageButton).Visible = True
            Select Case objParametriAgenda.Lav_Cod
                Case LAVCOD_DISERBO
                    CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_DoseConsigliata"), ImageButton).Visible = True
                Case Else
                    CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_DoseConsigliata"), ImageButton).Visible = False
            End Select
        End If

        'pulisco la selezione
        Cmb_UdM.Items.Clear()
        ComboFormulati.ddl_Formulati.Items.Clear()

        Lbl_Dose_Etichetta.Text = ""
        Lbl_Dose_Consigliata.Text = ""
        Lbl_Giacenza.Text = ""
        Lbl_FormulatoInRevisione.InnerText = ""

    End Sub

    Private Function ControllaGiacenzaMagazzino()
        'controllo se ho già visualizzato il messaggio di alert 
        If HiddenVarie.Value <> "" Then
            HiddenVarie.Value = ""
            Return True
        End If

        'verifico se il magazzino è stato selezionato
        If objParametriAgenda.Fabbricato.Length > 3 Then
            Dim frm_FerCod As Integer = 0
            Dim frm_UdmCod As Integer = 0
            Dim unita_m As String = ""
            Dim frm_MatCod As Integer = 0
            Dim qta_in_data As Decimal

            Dim Messaggio As String = ""

            Dim QtaTot As Decimal
            QtaTot = Txt_DoseTot_HA.Text

            'converto in k / l, todo, tutto da rivedere per internazionalizzazione
            Select Case Cmb_UdM.SelectedValue
                Case 2
                    frm_UdmCod = 2
                    unita_m = Resources.AgronicaAgenda_2010.Kg
                Case 3
                    frm_UdmCod = 2
                    unita_m = Resources.AgronicaAgenda_2010.Kg
                    QtaTot = QtaTot / 1000

                Case 2032  'mg
                    frm_UdmCod = 2
                    unita_m = Resources.AgronicaAgenda_2010.Kg
                    QtaTot = QtaTot / 1000000


                Case 104
                    frm_UdmCod = 29
                    unita_m = Resources.AgronicaAgenda_2010.Litri
                    QtaTot = QtaTot / 1000
                Case 29
                    frm_UdmCod = 29
                    unita_m = Resources.AgronicaAgenda_2010.Litri
                Case 101
                    frm_UdmCod = 29
                    unita_m = Resources.AgronicaAgenda_2010.Litri
                    QtaTot = QtaTot / 1000
            End Select

            frm_FerCod = ComboFormulati.ddl_Formulati.SelectedValue.Split("£")(0)
            frm_MatCod = 0


            'guardo se la quantità è conforme per la data di intervento
            Dim objMovDet As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
            qta_in_data = objMovDet.Verifica_Giacenze_Con_Magazzino_Esterno(Split(objParametriAgenda.Fabbricato, "|")(2), _
                                CInt(Split(objParametriAgenda.Fabbricato, "|")(1)), _
                                CInt(Split(objParametriAgenda.Fabbricato, "|")(0)), _
                                Int(191), _
                                frm_FerCod, _
                                frm_MatCod, _
                                0, 0, LOTTO_NONDEFINITO, _
                                0, CInt(frm_UdmCod), _
                                AGRODATAINIZIO, _
                                CDate(objParametriAgenda.Data), _
                                objParametriAgenda.Piva, _
                                objParametriAgenda.Sa_Cod, _
                                objParametriAgenda.Id_Agenda, _
                                objParametri_Server)
            If QtaTot > Math.Round(qta_in_data, 4) Then
                Messaggio = Messaggio & String.Format(Resources.AgronicaAgenda_2010.ATTENZIONEBrLaQuantitàDiBX0BAlBX1BÈPariABX1, CStr(ComboFormulati.ddl_Formulati.SelectedItem.Text), objParametriAgenda.Data, qta_in_data, unita_m)
            End If

            'guardo se la quantità di giacenza è conforme a prescindere dalla data
            qta_in_data = objMovDet.Verifica_Giacenze_Con_Magazzino_Esterno(Split(objParametriAgenda.Fabbricato, "|")(2), _
                                CInt(Split(objParametriAgenda.Fabbricato, "|")(1)), _
                                CInt(Split(objParametriAgenda.Fabbricato, "|")(0)), _
                                Int(191), _
                                frm_FerCod, _
                                frm_MatCod, _
                                0, 0, LOTTO_NONDEFINITO, _
                                0, CInt(frm_UdmCod), _
                                AGRODATAINIZIO, _
                                AGRODATAFINE, _
                                objParametriAgenda.Piva, _
                                objParametriAgenda.Sa_Cod, _
                                objParametriAgenda.Id_Agenda, _
                                objParametri_Server)
            If QtaTot > Math.Round(qta_in_data, 4) Then
                Messaggio = Messaggio & String.Format(Resources.AgronicaAgenda_2010.ATTENZIONEBrLaGiacenzaAttualeDiBX0BX1X2Non1, CStr(ComboFormulati.ddl_Formulati.SelectedItem.Text), qta_in_data, unita_m)
            End If




            If Messaggio <> "" Then

                '--------------BLOCCO SALVATAGGIO SE_SUPERA_GIACENZE-------------------
                If Not IsNothing(Session("UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE")) AndAlso Session("UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE") = True Then

                    Dim MErrore As String
                    MErrore = "In base alle impostazioni utente NON è possibile usare un prodotto con giacenza non sufficiente! " & vbCr & Messaggio
                    Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.AttenzioneLOperazioneNonÈStataRegistrataBr & MErrore, Page, , _
                        CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
                    Return False

                End If
                '---------------------------------------------------------------------

                Messaggio = Messaggio & vbCr & Resources.AgronicaAgenda_2010.BrBIProcedereUgualmenteIB
                'AgroSiNo
                ' VAnni: 28/2/2020: Non usare il resx sulla stringa "Salva"
                Messaggi.AgroSiNo(Messaggio, "Salva", Page, , Script_Giacenza_Magazzino)

                Return False

            End If
        End If



        Return True

    End Function

    Private Function ControllaDose(ByRef Dose As Decimal, ByRef Dose_Tot As Decimal)

        Txt_Dose_HL.Text = Replace(Txt_Dose_HL.Text, ".", ",")
        Txt_Dose_HA.Text = Replace(Txt_Dose_HA.Text, ".", ",")
        Txt_DoseTot_HA.Text = Replace(Txt_DoseTot_HA.Text, ".", ",")

        'controllo che i valori siano Nuemrici
        Dim Dose_HA As Decimal
        'controlli generici sulla dose
        If Not IsNumeric(Me.Txt_Dose_HA.Text) Then
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.InserireUnValoreNumPerLaBDoseFormulato, Page, , updateDoseInserisci)
            Return False
        Else
            If CDbl(Me.Txt_Dose_HA.Text) <= 0 Then
                Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.NonEPossibileInserireUnaDoseNullaONegativa, Page, , updateDoseInserisci)
                Return False
            End If
            Dose_HA = Txt_Dose_HA.Text
            Dose = Txt_Dose_HA.Text
        End If
        Dim Dose_HL As Decimal
        'controlli generici sulla dose
        If Not IsNumeric(Me.Txt_Dose_HL.Text) Then
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.InserireUnValoreNumPerLaBDoseFormulato, Page, , updateDoseInserisci)
            Return False
        Else
            If CDbl(Me.Txt_Dose_HL.Text) < 0 Then
                Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.NonEPossibileInserireUnaDoseNegativa, Page, , updateDoseInserisci)
                Return False
            End If
            Dose_HL = Txt_Dose_HL.Text
        End If
        'controlli generici sulla dose totale
        If Not IsNumeric(Txt_DoseTot_HA.Text) Then
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.InserireUnValoreNumericoPerIndicareLaBDose, Page, , updateDoseInserisci)
            Return False
        Else
            If CDbl(Me.Txt_DoseTot_HA.Text) <= 0 Then
                Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.NonEPossibileInserireUnaBDoseTotaleBNullaO, Page, , updateDoseInserisci)
                Return False
            End If
            Dose_Tot = Txt_DoseTot_HA.Text
        End If




        'controllo le dosi MASSIME
        'HA
        Dim DoseMassimaHaPresente As Boolean = False
        If True Then
            Dim SuperataDose As Boolean = False
            Dim SuperataDoseDiserbo As Boolean = False
            Dim BloccoDoseMassimaHa As Boolean = ControllaDoseMassimaHa(DoseMassimaHaPresente, SuperataDose, SuperataDoseDiserbo)
            If DoseMassimaHaPresente AndAlso Not BloccoDoseMassimaHa Then
                Return False
            End If
        End If


        'HL
        If Not DoseMassimaHaPresente Then
            Dim DoseMassimaHLPresente As Boolean = False
            Dim SuperataDose As Boolean = False
            Dim SuperataDoseDiserbo As Boolean = False
            Dim BloccoDoseMassimaHL As Boolean = ControllaDoseMassimaHL(DoseMassimaHLPresente, SuperataDose, SuperataDoseDiserbo)
            If DoseMassimaHLPresente AndAlso Not BloccoDoseMassimaHL Then
                Return False
            End If
        End If


        'controllo le dosi MINIME
        'le dosi minime NON sono BLOCCANTI
        'HA
        Dim DoseMinimaHAPresente As Boolean = False
        If True Then
            Dim SuperataDose As Boolean = False
            Dim BloccoDoseMinimaHA As Boolean = ControllaDoseMinimaHA(DoseMinimaHAPresente, SuperataDose)
            If DoseMinimaHAPresente AndAlso Not BloccoDoseMinimaHA Then
                Return False
            End If
        End If


        'HL
        If Not DoseMinimaHAPresente Then
            Dim DoseMinimaHLPresente As Boolean = False
            Dim SuperataDose As Boolean = False
            Dim BloccoDoseMinimaHL As Boolean = ControllaDoseMinimaHL(DoseMinimaHLPresente, SuperataDose)
            If DoseMinimaHLPresente AndAlso Not BloccoDoseMinimaHL Then
                Return False
            End If
        End If


        Return True

    End Function


    Private Function ControllaDoseMassimaHa(ByRef DosePresente As Boolean, ByRef SuperataDose As Boolean, ByRef SuperataDoseDiserbo As Boolean) As Boolean
        DosePresente = False
        SuperataDose = False
        SuperataDoseDiserbo = False
        If Session("D_HA_Max") IsNot Nothing AndAlso Session("D_HA_Max") <> 0 Then
            DosePresente = True
            'se ho una dose massima a ettaro
            Dim doseMaxHa_k_L As Decimal
            Dim perHa As Integer = 0
            Dim Udm_Cod_Max_scomposta As Integer = 0
            ScomponiUdm(Udm_Cod_Max_scomposta, perHa, Session("Udm_Cod_HA"))
            'converto to kg o litri
            Dim Moltiplicatore As Decimal
            Dim doseIndicata As Decimal
            'converto in kg o l
            ConvertiToKG_L(Udm_Cod_Max_scomposta, Moltiplicatore)
            doseMaxHa_k_L = Session("D_HA_Max") * Moltiplicatore
            ConvertiToKG_L(Cmb_UdM.SelectedValue, Moltiplicatore)
            doseIndicata = Txt_Dose_HA.Text * Moltiplicatore
            If doseIndicata > doseMaxHa_k_L Then
                SuperataDose = True
                Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.ATTENZIONEBrLaBDoseHaBSelezionataNonPuòSup, Page, , updateDoseInserisci)
                If Session("Blocca_Dose_Massima") = True Then
                    Return False
                End If
            End If
            Dim DoseConsentitaDiserbo As Decimal = 0
            If Session("D_HA_Max_Diserbo") IsNot Nothing AndAlso Session("D_HA_Max_Diserbo") <> 0 Then
                DoseConsentitaDiserbo = CDbl(Session("D_HA_Max_Diserbo"))
                If doseIndicata > DoseConsentitaDiserbo Then
                    SuperataDoseDiserbo = True
                    Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.ATTENZIONEBrLaBDoseHaBSelezionataNonPuòSup1 & DoseConsentitaDiserbo & Resources.AgronicaAgenda_2010.KgOLHa, Page, , updateDoseInserisci)
                    If Session("Blocca_Dose_Massima") = True Then
                        Return False
                    End If
                End If
            End If

        End If
        Return True
    End Function

    Private Function ControllaDoseMassimaHL(ByRef DosePresente As Boolean, ByRef SuperataDose As Boolean, ByRef SuperataDoseDiserbo As Boolean) As Boolean
        DosePresente = False
        SuperataDose = False
        SuperataDoseDiserbo = False
        'HL
        If Session("D_HL_Max") IsNot Nothing AndAlso Session("D_HL_Max") <> 0 Then
            DosePresente = True
            'se ho una dose massima a ettaro
            Dim doseMaxHL_k_L As Decimal
            Dim perHl As Integer = 0
            Dim Udm_Cod_Max_scomposta As Integer = 0
            ScomponiUdm(Udm_Cod_Max_scomposta, perHl, Session("Udm_Cod_HL"))
            'converto to kg o litri
            Dim Moltiplicatore As Decimal
            Dim doseIndicata As Decimal
            'converto in kg o l
            ConvertiToKG_L(Udm_Cod_Max_scomposta, Moltiplicatore)
            doseMaxHL_k_L = Session("D_HL_Max") * Moltiplicatore
            ConvertiToKG_L(Cmb_UdM.SelectedValue, Moltiplicatore)
            doseIndicata = Txt_Dose_HL.Text * Moltiplicatore
            If doseIndicata > doseMaxHL_k_L Then
                SuperataDose = True
                Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.ATTENZIONEBrLaBDoseHlBSelezionataNonPuòSup, Page, , updateDoseInserisci)
                If Session("Blocca_Dose_Massima") = True Then
                    Return False
                End If
            End If
            Dim DoseConsentitaDiserbo As Decimal = 0
            If Session("D_HA_Max_Diserbo") IsNot Nothing AndAlso Session("D_HA_Max_Diserbo") <> 0 Then
                DoseConsentitaDiserbo = CDbl(Session("D_HA_Max_Diserbo"))
                If doseIndicata > DoseConsentitaDiserbo Then
                    SuperataDoseDiserbo = True
                    Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.ATTENZIONEBrLaBDoseHaBSelezionataNonPuòSup1 & DoseConsentitaDiserbo & Resources.AgronicaAgenda_2010.KgOLHa, Page, , updateDoseInserisci) 'todo, kg o litri da verificare per multicultura
                    If Session("Blocca_Dose_Massima") = True Then
                        Return False
                    End If
                End If
            End If
        End If
        Return True
    End Function

    Private Function ControllaDoseMinimaHA(ByRef DosePresente As Boolean, ByRef SuperataDose As Boolean) As Boolean
        DosePresente = False
        SuperataDose = False
        If Session("D_HA_Min") IsNot Nothing AndAlso Session("D_HA_Min") <> 0 Then
            DosePresente = True
            'se ho una dose massima a ettaro
            Dim doseMinHa_k_L As Decimal
            Dim perHa As Integer = 0
            Dim Udm_Cod_Max_scomposta As Integer = 0
            ScomponiUdm(Udm_Cod_Max_scomposta, perHa, Session("Udm_Cod_HA"))
            'converto to kg o litri
            Dim Moltiplicatore As Decimal
            Dim doseIndicata As Decimal
            'converto in kg o l
            ConvertiToKG_L(Udm_Cod_Max_scomposta, Moltiplicatore)
            doseMinHa_k_L = Session("D_HA_Min") * Moltiplicatore
            ConvertiToKG_L(Cmb_UdM.SelectedValue, Moltiplicatore)
            doseIndicata = Txt_Dose_HA.Text * Moltiplicatore
            If doseIndicata < doseMinHa_k_L Then
                SuperataDose = True
                Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.ATTENZIONEBrLaBDoseHaBSelezionataÈInferior, Page, , updateDoseInserisci) 'todo, dose ha !!
                If Session("Blocca_Dose_Minima") = True Then
                    Return False
                End If
            End If
        End If
        Return True
    End Function

    Private Function ControllaDoseMinimaHL(ByRef DosePresente As Boolean, ByRef SuperataDose As Boolean) As Boolean
        DosePresente = False
        SuperataDose = False
        If Session("D_HL_Min") IsNot Nothing AndAlso Session("D_HL_Min") <> 0 Then
            DosePresente = True
            'se ho una dose massima a ettaro
            Dim doseMinHL_k_L As Decimal
            Dim perHl As Integer = 0
            Dim Udm_Cod_Max_scomposta As Integer = 0
            ScomponiUdm(Udm_Cod_Max_scomposta, perHl, Session("Udm_Cod_HL"))
            'converto to kg o litri
            Dim Moltiplicatore As Decimal
            Dim doseIndicata As Decimal
            'converto in kg o l
            ConvertiToKG_L(Udm_Cod_Max_scomposta, Moltiplicatore)
            doseMinHL_k_L = Session("D_HL_Min") * Moltiplicatore
            ConvertiToKG_L(Cmb_UdM.SelectedValue, Moltiplicatore)
            doseIndicata = Txt_Dose_HL.Text * Moltiplicatore
            If doseIndicata < doseMinHL_k_L Then
                SuperataDose = True
                Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.ATTENZIONEBrLaBDoseHlBSelezionataÈInferior, Page, , updateDoseInserisci) 'todo, dose hl
                If Session("Blocca_Dose_Minima") = True Then
                    Return False
                End If
            End If
        End If
        Return True
    End Function



    ''' <summary>
    ''' Controlla se è selezionato un FORMULATO
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ControllaSelezioneFormulato(ByRef FrCod As String, _
                                                 ByRef FrDes As String, _
                                                 ByRef Carenza As Decimal, _
                                                 ByRef strPA_Cod As String, _
                                                 ByRef strCLTOSS_COD As String) As Boolean
        If Me.ComboFormulati.Testo_Combo = "" Then
            Messaggi.AgroMsgBox("Selezionare un formulato.", Page, , updateDoseInserisci)
            Return False
        Else
            Dim ArrayTmp() As String
            Dim ArrayTmpFrDes() As String
            ArrayTmp = Split(ComboFormulati.Valore_Combo, "£")
            ArrayTmpFrDes = Split(ComboFormulati.Testo_Combo, "---")
            If ArrayTmp IsNot Nothing Then
                FrCod = ArrayTmp(0)
                FrDes = ComboFormulati.Testo_Combo
                If ArrayTmp.Length > 1 Then
                    If ArrayTmp(1) <> "" Then
                        Carenza = ArrayTmp(1)
                    Else
                        Carenza = 0
                    End If
                End If
                If ArrayTmp.Length > 4 Then
                    If ArrayTmp(4) <> "" Then
                        strPA_Cod = ArrayTmp(4)
                    Else
                        strPA_Cod = ""
                    End If
                End If
                'If ArrayTmp.Length > 1 Then
                '    If ArrayTmp(3) <> "" Then
                '        strTITOLI = ArrayTmp(3)
                '    Else
                '        strTITOLI = 0
                '    End If
                'End If
                If ArrayTmp.Length > 5 Then
                    If ArrayTmp(5) <> "" Then
                        strCLTOSS_COD = ArrayTmp(5)
                    Else
                        strCLTOSS_COD = 0
                    End If
                End If
            End If
            If ArrayTmpFrDes IsNot Nothing Then
                FrDes = ArrayTmpFrDes(0)
            End If
        End If

        'Verifico la Carenza
        If Carenza > 0 Then
            If Session("ControllaBlocco") = True And ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.SelectedValue <> "0" Then
                Dim ListaImpianti As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)
                ListaImpianti = CType(Master, Operazione).GetImpianti()
                Dim DataMinima As Date
                DataMinima = AGRODATAFINE
                For i = 0 To ListaImpianti.Count - 1
                    If ListaImpianti(i).Data_Raccolta <> New Date Then
                        If DataMinima > ListaImpianti(i).Data_Raccolta AndAlso _
                                ListaImpianti(i).Data_Raccolta_Prevista >= objParametriAgenda.Data Then
                            DataMinima = ListaImpianti(i).Data_Raccolta
                        End If
                    Else
                        If ListaImpianti(i).Data_Raccolta_Prevista <> New Date Then
                            If DataMinima > ListaImpianti(i).Data_Raccolta_Prevista AndAlso _
                                ListaImpianti(i).Data_Raccolta_Prevista >= objParametriAgenda.Data Then
                                DataMinima = ListaImpianti(i).Data_Raccolta_Prevista
                            End If
                        End If
                    End If
                Next

                Dim DataMinimaRaccolta As Date
                DataMinimaRaccolta = objParametriAgenda.Data
                DataMinimaRaccolta = DataMinimaRaccolta.AddDays(CInt(Carenza + 1))
                'controllo che la data della registrazione non sia posteriore di quella della prima raccolta
                If DataMinima >= objParametriAgenda.Data Then
                    If DataMinimaRaccolta > DataMinima Then
                        Messaggi.AgroMsgBox(String.Format(Resources.AgronicaAgenda_2010.LaDataDiRaccoltaDiUnoDegliAppezzamentiNonÈ, DataMinima, Carenza), Page, , updateDoseInserisci)
                        If Session("Blocca_Carenza") = True Then
                            Return False
                        End If
                    End If
                End If
            End If

        End If



        Return True
    End Function

    Private Function ControllaAcqua() As Boolean
        Txt_Acqua_Ha.Text = Replace(Txt_Acqua_Ha.Text, ".", ",")

        'controllo se esiste una qta di acqua max
        Dim Dose_Acqua As Decimal
        'l'acqua non è obbligatoria quindi posso avere anche 0
        If Not IsNumeric(Me.Txt_Acqua_Ha.Text) Then
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.InserireUnValoreNumericoPerIndicareLaBQuan, Page, , updateDoseInserisci)
            Return False
        Else
            Dose_Acqua = Txt_Acqua_Ha.Text
        End If

        'controllo che qta Max
        If session("Acqua_Max") IsNot Nothing AndAlso session("Acqua_Max") <> 0 Then
            If Dose_Acqua > session("Acqua_Max") Then
                'entrambe le quantità sono già espresse in HL
                Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.ATTENZIONEBrLaBQuantitàDiAcquaBSuperaLaQua, Page, , updateDoseInserisci)
                If session("Blocca_Acqua_Massima") = True Then
                    Return False
                End If
            End If
        End If
        'controllo che qta Min
        If session("Acqua_Min") IsNot Nothing AndAlso session("Acqua_Min") <> 0 Then
            If Dose_Acqua < session("Acqua_Min") Then
                'entrambe le quantità sono già espresse in HL
                Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.ATTENZIONEBrLaBQuantitàDiAcquaBÈInferioreA, Page, , updateDoseInserisci)
                If session("Blocca_Acqua_Minima") = True Then
                    Return False
                End If
            End If
        End If
        Return True
    End Function

    Private Function ControllaSelezioneAvversita(ByRef Av_Cod As String, ByRef Av_Gru As String, ByRef Av_Des As String) As Boolean


        Select Case ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.SelectedValue

            Case "0"
                Av_Cod = "0"
                Av_Gru = "0"
                Av_Des = ""

            Case Else

                For i = 0 To GridViewAvversita.Rows.Count - 1
                    If CType(GridViewAvversita.Rows(i).FindControl("ChkSelezionaAvversita"), CheckBox).Checked = True Then
                        Av_Cod &= GridViewAvversita.Rows(i).Cells(1).Text() & ","
                        Av_Gru &= GridViewAvversita.Rows(i).Cells(2).Text() & ","
                        Av_Des &= GridViewAvversita.Rows(i).Cells(3).Text() & ", "
                        Av_Gru &= "0,"
                    End If
                Next


                If Av_Des = "" Then
                    Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.ENecessarioSelezionareUnAvversità, Page, , updateDoseInserisci)
                    Return False
                End If

                If Av_Cod <> "0" Then
                    Av_Cod = Left(Av_Cod, Av_Cod.Length - 1)
                End If
                If Av_Gru <> "0" Then
                    Av_Gru = Left(Av_Gru, Av_Gru.Length - 1)
                End If
                If Av_Des <> "" Then
                    Av_Des = Left(Av_Des, Av_Des.Length - 2)
                End If

        End Select

        Return True

    End Function


    ''' <summary>
    ''' UNITA' di MISURA
    ''' </summary>
    ''' <param name="UdmCod"></param>
    ''' <param name="UdmDesSimbolo"></param>
    ''' <param name="UdmDes"></param>
    ''' <param name="UdmCodTrasformato"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ControllaSelezioneUnitadiMisura(ByRef UdmCod As Integer, ByRef UdmDesSimbolo As String, ByRef UdmDes As String, ByRef UdmCodTrasformato As Integer) As Boolean
        If Me.Cmb_UdM.SelectedItem.Text = "" Then
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.SelezionareUnUnitaDiMisura, Page, , updateDoseInserisci)
            Return False
        Else

            UdmCod = Cmb_UdM.SelectedItem.Value
            UdmDesSimbolo = Cmb_UdM.SelectedItem.Text
            If Me.rbl_DoseHL.Checked Then
                UdmDes = "[" & UdmDesSimbolo & "/hl]" 'todo, litri ettolitri, ecc..
            Else
                UdmDes = "[" & UdmDesSimbolo & "/q]"
            End If

            Select Case UdmCod
                Case 3  'g
                    UdmCodTrasformato = 2
                Case 4  'q
                    UdmCodTrasformato = 2
                Case 304 't
                    UdmCodTrasformato = 2
                Case 2032
                    UdmCodTrasformato = 2

                Case 101 'ml
                    UdmCodTrasformato = 29
                Case 104 'cc
                    UdmCodTrasformato = 29

                Case 2, 29 'kg,l
                    UdmCodTrasformato = UdmCod
            End Select

        End If


        Return True
    End Function

    Protected Sub BTN_ComboFiltriAggiuntiviAgenda_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTN_ComboFiltriAggiuntiviAgenda.Click
        objParametriAgenda.FiltroRicerca = ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.SelectedValue

        If objParametriAgenda.FiltroRicerca = "2" Then
            CaricaGriglia_Avversita()
        End If

        'se seleziono il filtro nessuno allora carico tutte le avversità
        If objParametriAgenda.FiltroRicerca = "0" Then
            CaricaGriglia_Avversita()
        End If


    End Sub


    Private Sub btn_cerca_avv_Click(sender As Object, e As System.EventArgs) Handles btn_cerca_avv.Click

        Dim DtAvv As DataTable = Session("DtAvv")

        Dim DrAvv() As DataRow
        Dim DtAvvF As New DataTable

        If DtAvv IsNot Nothing AndAlso DtAvv.Rows.Count > 0 Then
            DrAvv = DtAvv.Select("av_des like '%" & Txt_Avv.Text & "%'")
            If DrAvv IsNot Nothing AndAlso DrAvv.Length > 0 Then
                DtAvvF = DtAvv.Clone
                For i = 0 To DrAvv.Length - 1
                    DtAvvF.ImportRow(DrAvv(i))
                Next
            End If
        End If

        GridViewAvversita.DataSource = DtAvvF
        GridViewAvversita.DataBind()

        GridViewAvversita.Columns(4).HeaderStyle.CssClass = "displaynone"
        GridViewAvversita.Columns(4).ItemStyle.CssClass = "displaynone"

        Select Case objParametriAgenda.Lav_Cod
            Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_CONCIA_SEME, LAVCOD_GEODISINFESTAZIONE
                Select Case objParametriAgenda.Disciplinare
                    Case "0", "-2"
                    Case Else
                        GridViewAvversita.Columns(4).HeaderStyle.CssClass = ""
                        GridViewAvversita.Columns(4).ItemStyle.CssClass = ""
                End Select
            Case Else
        End Select


    End Sub


End Class