Imports System.Web.Services
Imports AgronicaDomandaIrrigua
Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreUtility
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaDomandaIrrigua.Resources
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreModelsSTD.attivita.Attivita

Public Class RilieviBS
    Inherits System.Web.UI.Page
    Implements iOperazioneGUI

    Public Master_Operazione As OperazioneBootstrap
    Dim objParametri_Server, objParametri_Utenti, objParametri_Super_Server As AgronicaCoreParametri
    Dim objParametriAgenda As ParametriAgenda

    Dim Id_Agenda_Old As Integer = 0
    Dim TipoOperazioneAgenda As enum_Tipo_Operazione_Agenda

    Dim Movimento_Dettaglio_Pendente As Integer
    Dim Movimento_Dettaglio_Extra_Date As Date
    Dim Movimento_Dettaglio_Anno As Integer

    Dim Movimento_Dettaglio_Contabilizzato As Integer = 1

    Dim FasiOLD As Boolean = False

    Const NessunDpiNessunaEtichetta As String = "-999"

    Public Shared _aperturadaIFrame As Integer = 0

    Private Enum enum_OggettoVisita
        Impresa = 0
        CentroAz = 1
        Appezzamenti = 2
    End Enum

    Public pivaSuperUser As String

#Region "Salvataggio"


    Public Sub SalvaTutto(sender As Object, e As EventArgs) Implements iOperazioneGUI.SalvaTutto

        Master_Operazione.SalvaCostiAccessori_SuAgendaMovimenti(False, Nothing)

        Dim messaggio_errore As String = ""
        Dim messaggio_alert As String = ""
        Dim Unid_Operazione As String = ""

        Dim TipoSalvataggio As Integer = CType(Ricerca.FindControlIterative(Page.Master, "tipo_salva"), HiddenField).Value

        Dim ListaOpAgenda As New List(Of Operazione_Agenda)

        Dim esito As Boolean = SalvaOperazioneAgenda(TipoSalvataggio, messaggio_errore, Unid_Operazione, ListaOpAgenda, messaggio_alert)

        If esito Then

            'ScriptManager.RegisterStartupScript(UpdatePanel_Tabella, UpdatePanel_Tabella.GetType, "azzera",
            '                                        "$(document).ready(function () {$('#" & HiddenVarie.ClientID & "').val(''); });", True)

            fine_salvataggio(TipoSalvataggio, Unid_Operazione)

        Else

            If messaggio_errore <> "" Then
                Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.AttenzioneLOperazioneNonÈStataRegistrataBr & messaggio_errore, Page, ,
                                    CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            ElseIf messaggio_alert <> "" Then
                ' VAnni: 28/2/2020: Non usare il resx sulla stringa "Salva"
                Messaggi.AgroSiNo(messaggio_alert & vbCr & Resources.AgronicaAgenda_2010.BrBIProcedereUgualmenteIB, "Salva", Page, ,
                                      CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            End If

        End If

    End Sub

    Public Sub fine_salvataggio(TipoSalvataggio As Integer, Unid_Operazione As String) Implements iOperazioneGUI.fine_salvataggio
        Session("UtilizzataRicetta") = False

        If _aperturadaIFrame = 1 AndAlso objParametriAgenda.SitoOrigine = Enum_SiteRedirector.GiasNG Then
            Dim strJS As New StringBuilder
            strJS.AppendLine("$(document).ready(function () { ")
            strJS.AppendLine("      window.parent.postMessage('chiudiWindowGiasNG', '*'); ")
            strJS.AppendLine(" });")

            ScriptManager.RegisterStartupScript(CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel), CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).GetType(),
                              String.Format("jQuery_{0}", CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).ClientID), strJS.ToString, True)
            Exit Sub
        End If

        Select Case TipoSalvataggio

            Case enum_Tipo_Salvataggio.Salva_e_Esci

                Dim strJS As New StringBuilder
                strJS.AppendLine("$(document).ready(function () { ")

                If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.GiasLan OrElse
                    objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Trattamenti OrElse
                    objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Gis OrElse
                    objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Filtrone Then

                    strJS.AppendLine("      window.parent.RilievoRientro(" & objParametriAgenda.Id_Agenda & "); ")
                    strJS.AppendLine(" });")

                Else

                    Dim link As String = ""
                    Try
                        Dim sitoorigine As Enum_SiteRedirector = objParametriAgenda.SitoOrigine
                        Dim paginaOnLineRitorno As Integer = objParametriAgenda.PaginaSitoOrigine

                        If sitoorigine = Enum_SiteRedirector.Sito_GiasOnline_2010 AndAlso paginaOnLineRitorno = enum_PagineGiasOnline_2010.RegistazioneSmart Then
                            link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri(
                                                       Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                                       enum_PagineGiasOnline_2010.RegistazioneSmart,
                                                       enum_PagineAgenda_2010.Menu, objParametriAgenda.Piva, "", "", 0, "")

                        ElseIf sitoorigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010 AndAlso paginaOnLineRitorno = enum_PagineAgenda_2010.Pagina_Visite_Lista Then
                            link = "../Visite/Visite_Lista.aspx"
                        ElseIf sitoorigine = Enum_SiteRedirector.GiasNG Then
                            AgronicaCoreGestioneRichieste.MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva,
                                                                        Enum_SiteRedirector.GiasNG,
                                                                        objParametriAgenda.PaginaSitoOrigine,
                                                                        link,
                                                                        objParametri_Server)
                        Else
                            link = CType(Master.Master, DomandaIrriguaBootstrap).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
                        End If

                    Catch ex As Exception
                        link = CType(Master.Master, DomandaIrriguaBootstrap).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
                    End Try

                    strJS.AppendLine("      ChiamataParent_Id_Ageda(" & objParametriAgenda.Id_Agenda & "); ")
                    strJS.AppendLine("      window.location = '" & link & "'; ")
                    strJS.AppendLine(" });")

                    Messaggi.AgroMsgBuonFine(Resources.AgronicaAgenda_2010.RegistrazioneEffettuataConSuccesso, Page, ,
                                                     CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))

                End If
                ScriptManager.RegisterStartupScript(CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel), CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).GetType(),
                                              String.Format("jQuery_{0}", CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).ClientID), strJS.ToString, True)



                Rilievi_Disposed()
                objParametriAgenda.Svuota_DatiOperazione()
                objParametriAgenda.RecuperaRientro()
                objParametriAgenda.salva()

            Case enum_Tipo_Salvataggio.Salva_e_Nuovo
                Rilievi_Disposed()
                objParametriAgenda.Impianti = New List(Of Impianto)
                objParametriAgenda.Note = New List(Of Nota)
                objParametriAgenda.Movimenti = New List(Of Movimento)

                Dim strJS As New StringBuilder
                strJS.AppendLine("$(document).ready(function () { ")
                strJS.AppendLine("      window.location = '../Operazioni/RilieviBS.aspx'; ")
                strJS.AppendLine(" });")

                ScriptManager.RegisterStartupScript(CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel), CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).GetType(),
                                              String.Format("jQuery_{0}", CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).ClientID), strJS.ToString, True)

                Messaggi.AgroMsgBuonFine(Resources.AgronicaAgenda_2010.RegistrazioneEffettuataConSuccesso, Page, ,
                                  CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))

            Case enum_Tipo_Salvataggio.Salva_e_Duplica
                'Dim strJS As New StringBuilder
                'strJS.AppendLine("$(document).ready(function () { ")
                'strJS.AppendLine("      Abilita_Disabilita_Resto(); ")
                'strJS.AppendLine(" });")
                'ScriptManager.RegisterStartupScript(CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel), CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).GetType(),
                '                              String.Format("jQuery_{0}", CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).ClientID), strJS.ToString, True)

                Messaggi.AgroMsgBuonFine(Resources.AgronicaAgenda_2010.RegistrazioneEffettuataConSuccesso, Page, ,
                                  CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))

            Case enum_Tipo_Salvataggio.Salva_e_Vai_ai_Costi

                Dim strJS As New StringBuilder
                strJS.AppendLine("$(document).ready(function () { ")

                If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.GiasLan OrElse
                    objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Trattamenti OrElse
                    objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Gis Then

                    strJS.AppendLine("      window.parent.RilievoRientro(" & objParametriAgenda.Id_Agenda & "); ")
                    strJS.AppendLine(" });")

                Else

                    ' GestioneCosti.aspx non è presente su DomandaIrrigua: redirect cross-site verso AgroAgenda
                    ' tramite ParametriAgenda_2010 → GestioneRichieste (AgroAgenda) → GestioneCosti.aspx
                    Dim link As String = ""
                    Dim sitoorigine As Enum_SiteRedirector = HttpContext.Current.Session("Sito_Origine")
                    Try

                        Dim paginaOnLineRitorno As Integer = objParametriAgenda.PaginaSitoOrigine

                        If sitoorigine = Enum_SiteRedirector.Sito_GiasOnline_2010 AndAlso paginaOnLineRitorno = enum_PagineGiasOnline_2010.RegistazioneSmart Then
                            link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri(
                                                       Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                                       enum_PagineGiasOnline_2010.RegistazioneSmart,
                                                       enum_PagineAgenda_2010.Menu, objParametriAgenda.Piva, "", "", 0, "")

                        ElseIf sitoorigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010 AndAlso paginaOnLineRitorno = enum_PagineAgenda_2010.Pagina_Visite_Lista Then
                            link = "../Visite/Visite_Lista.aspx"
                        Else
                            link = CType(Master.Master, DomandaIrriguaBootstrap).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
                        End If

                    Catch ex As Exception
                        link = CType(Master.Master, DomandaIrriguaBootstrap).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
                    End Try

                    Dim objPA2010 As New AgronicaCoreGestioneRichieste.ParametriAgenda_2010()
                    objPA2010.PaginaRichiesta = enum_PagineAgenda_2010.Pagina_Gestione_Costi
                    objPA2010.Piva = objParametriAgenda.Piva
                    objPA2010.QueryStringFiltrino = "?p=" & Sicurezza.Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder) &
                                                    "&id_agenda=" & Sicurezza.Stringa_Codifica(objParametriAgenda.Id_Agenda, AgroKey_EncoderDecoder) &
                                                    IIf(sitoorigine = Enum_SiteRedirector.GiasNG, "", "&origine=" & Sicurezza.Stringa_Codifica(link, AgroKey_EncoderDecoder)) &
                                                    "&entrata_diretta=" & Sicurezza.Stringa_Codifica(0, AgroKey_EncoderDecoder, Nothing)
                    objPA2010.Salva()

                    Dim PaginaLink As String = AgronicaCoreGestioneRichieste.RedirectGestione.PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(
                        Enum_SiteRedirector.Sito_AgronicaDomandaIrrigua,
                        Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                        "")

                    strJS.AppendLine("      ChiamataParent_Id_Ageda(" & objParametriAgenda.Id_Agenda & "); ")
                    strJS.AppendLine("      window.location = '" & PaginaLink & "'; ") 'Nelle altre operazioni porto ai costi
                    strJS.AppendLine(" });")

                    Messaggi.AgroMsgBuonFine(Resources.AgronicaAgenda_2010.RegistrazioneEffettuataConSuccesso, Page, ,
                                                     CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))

                End If
                ScriptManager.RegisterStartupScript(CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel), CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).GetType(),
                                              String.Format("jQuery_{0}", CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).ClientID), strJS.ToString, True)



                Rilievi_Disposed()
                objParametriAgenda.Svuota_DatiOperazione()
                objParametriAgenda.RecuperaRientro()
                objParametriAgenda.salva()

        End Select

    End Sub

    Public Function SalvaOperazioneAgenda(ByRef messaggio_errore As String, ByRef Unid_Operazione As String) As Boolean Implements iOperazioneGUI.SalvaOperazioneAgenda
        Throw New NotImplementedException()
    End Function

    Private Function SalvaOperazioneAgenda(
                           ByVal TipoSalvataggio As Integer,
                           ByRef messaggio_errore As String,
                           ByRef Unid_Operazione As String,
                           ByRef ListaOpAgenda As List(Of Operazione_Agenda),
                           ByRef messaggio_alert As String) As Boolean


        Dim res As Boolean = False

        '---------------------------------------
        ' recupero il CENTRO
        If objParametriAgenda.Sa_Cod = "" Then
            messaggio_errore = Resources.AgronicaAgenda_2010.SelezionareUnCentroAziendale
            Return False
        End If

        Try

            '-----------------------------------------------------
            '----------- CONNESSIONE E TRANSAZIONE ---------------
            ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)
            '-----------------------------------------------------

            Dim objAgendaScrivi As New Agenda_Operazione_Helper
            Dim ListaSacod As New List(Of String)

            Dim Raccoglitore_Cod As Integer = 0

            Dim righeModificateArray As JArray = estraiJSONgrigliaRilievi()

            'Cancello le vecchie operazioni collegate e salvo gli id_agenda
            Dim htOldIdAgenda As New Hashtable()

            Dim lista_mdRif2 As New List(Of Movimento_Dettaglio_Riferimento)

            If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then

                'Estraggo la lista del lav_cod
                Dim listaChiavi As List(Of String) = (From x In righeModificateArray Select CStr(x("piva")) & "|" _
                                                                                         & CStr(x("sa_cod")) & "|" _
                                                                                         & If(CStr(x("lav_cod")).Contains("|"), CStr(x("lav_cod")), CStr(x("lav_cod")) & "|0") & "|" _
                                                                                         & If(CInt(x("appezza")) > 0, "1", "0") & "|" _
                                                                                         & CStr(x("ff_cod")) & "|" _
                                                                                         & If(IsNumeric(x("lav_cod")) AndAlso CInt(x("lav_cod")) = LAVCOD_FASI_FENOLOGICHE, CStr(x("Dato")), "")
                                                                                         ).Distinct().ToList()

                'Leggo la vecchia operazione
                Dim objAgenda As New Agenda_Operazione_Helper
                Dim Agenda_Old As Operazione_Agenda = objAgenda.Leggi(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod,
                                                                              objParametriAgenda.Id_Agenda, 0, objParametri_Server)

                'Salvo a parte tutti i vecchi id_agenda e cancello le operazioni d'agenda collegate
                For Each mdr As Movimento_Dettaglio_Riferimento In Agenda_Old.Movimenti(0).Movimenti_Dettagli(0).Movimenti_Dettagli_Riferimenti

                    Dim Agenda_Collegata_Old As Operazione_Agenda = objAgenda.Leggi(mdr.Piva_Rif, mdr.Sa_Cod_Rif, mdr.Id_Agenda_Rif, 0, objParametri_Server)
                    'Dato che le visite possono essere fatte anche solo su centro, mi serve un flag per capire se ci sono degli appezza o no
                    Dim flagDestinazioni As String = If(Agenda_Collegata_Old.Movimenti(0).Movimenti_Dettagli(0).Movimenti_Destinazioni.Count > 0, "1", "0")

                    Dim chiave As String

                    Select Case mdr.Lav_Cod_Rif
                        Case LAVCOD_FASI_FENOLOGICHE
                            Dim FF_Cod As Integer = 0
                            If Agenda_Collegata_Old.Movimenti(0).Movimenti_Dettagli(0).Movimenti_Dettagli_Tecnici(0).ff_classe >= 1000 Then
                                FF_Cod = Agenda_Collegata_Old.Movimenti(0).Movimenti_Dettagli(0).Movimenti_Dettagli_Tecnici(0).ff_classe
                            End If
                            Dim Data As String = Agenda_Collegata_Old.Movimenti(0).Movimenti_Dettagli(0).Movimenti_Destinazioni(0).Data

                            chiave = mdr.Piva_Rif & "|" & mdr.Sa_Cod_Rif & "|" & mdr.Lav_Cod_Rif & "|" & Agenda_Collegata_Old.Id_Attivita & "|" & flagDestinazioni & "|" & FF_Cod & "|" & Data
                        Case Else
                            chiave = mdr.Piva_Rif & "|" & mdr.Sa_Cod_Rif & "|" & mdr.Lav_Cod_Rif & "|" & Agenda_Collegata_Old.Id_Attivita & "|" & flagDestinazioni
                    End Select

                    htOldIdAgenda.Add(chiave, mdr.Id_Agenda_Rif)

                    'Se l'operazione collegata non esisterà più allora lo loggo nella tabella la sua cancellazione
                    Dim logCancellazione As Boolean = Not listaChiavi.Contains(chiave)

                    Dim CancellataOperazione As Boolean = objAgendaScrivi.Cancella(mdr.Piva_Rif,
                                                 mdr.Sa_Cod_Rif, mdr.Id_Agenda_Rif, False,
                                                 objParametri_Server, logCancellazione:=logCancellazione)

                Next

                If objParametriAgenda.Raccoglitore_Cod <> 0 Then

                    Raccoglitore_Cod = objParametriAgenda.Raccoglitore_Cod
                    Dim ListaOperazioni As List(Of Operazione_Agenda)
                    ListaOperazioni = objAgenda.LeggiLista_DaRaccoglitore(objParametriAgenda.Piva, Raccoglitore_Cod, objParametri_Server)

                    For Each Operazione In ListaOperazioni

                        Dim FF_Cod As Integer = 0
                        Dim Data As String
                        Dim chiave As String

                        For Each movimento In Operazione.Movimenti

                            If movimento.Cau_Mov = CAU_RILIEVO_CAMPO Then

                                If movimento.Movimenti_Dettagli(0).Movimenti_Dettagli_Tecnici(0).ff_classe >= 1000 Then
                                    FF_Cod = movimento.Movimenti_Dettagli(0).Movimenti_Dettagli_Tecnici(0).ff_classe
                                End If
                                Data = movimento.Movimenti_Dettagli(0).Movimenti_Destinazioni(0).Data

                                chiave = Operazione.Piva & "|" & Operazione.Sa_Cod & "|" & Operazione.Lav_Cod & "|" & Operazione.Id_Attivita & "|1" & "|" & FF_Cod & "|" & Data

                                Exit For

                            End If

                        Next


                        'If Operazione.Movimenti(0).Movimenti_Dettagli(0).Movimenti_Dettagli_Tecnici(0).ff_classe >= 1000 Then
                        '    FF_Cod = Operazione.Movimenti(0).Movimenti_Dettagli(0).Movimenti_Dettagli_Tecnici(0).ff_classe
                        'End If
                        'Dim Data As String = Operazione.Movimenti(0).Movimenti_Dettagli(0).Movimenti_Destinazioni(0).Data

                        'Dim chiave As String = Operazione.Piva & "|" & Operazione.Sa_Cod & "|" & Operazione.Lav_Cod & "|" & Operazione.Id_Attivita & "|1" & "|" & FF_Cod & "|" & Data

                        htOldIdAgenda.Add(chiave, Operazione.Id_Agenda)

                        'Recupero i vecchi costi CdG prima che vengano cancellati (SOLO SE è UN'OPERAZIONE SINGOLA)
                        If ListaOperazioni.Count = 1 Then
                            Dim mdr_Rif As New Agenda_Movimenti_Dettagli_Riferimenti_Helper()
                            lista_mdRif2 = mdr_Rif.LeggiRiferimentiAgenda(Operazione.Piva, 0, Operazione.Id_Agenda, 0, "", objParametri_Server)
                        End If


                        'Se l'operazione collegata non esisterà più allora lo loggo nella tabella la sua cancellazione
                        Dim logCancellazione As Boolean = Not listaChiavi.Contains(chiave)

                        'Dal 06/23 conserviamo la data e l'username di creazione originali per tutti i record Agenda
                        allinea_DataUsernameCreazione(Operazione, objParametri_Server, Tipo_Attivita.QuadernoDiCampagna, 0)

                        Dim CancellataOperazione As Boolean = objAgendaScrivi.Cancella(Operazione.Piva,
                                                             Operazione.Sa_Cod, Operazione.Id_Agenda, False,
                                                             objParametri_Server, logCancellazione:=logCancellazione)

                    Next

                End If


            End If


            For Each imp As JObject In righeModificateArray

                Select Case objParametriAgenda.Lav_Cod
                    Case LAVCOD_VISITA
                        If Not ListaSacod.Contains(imp("sa_cod").ToString) Then
                            ListaSacod.Add(imp("sa_cod").ToString)
                        End If
                    Case Else
                        If Not ListaSacod.Contains(imp("sa_cod").ToString & "|" & imp("ff_cod").ToString & "|" & If(IsNumeric(imp("lav_cod")) AndAlso CInt(imp("lav_cod")) = LAVCOD_FASI_FENOLOGICHE, CStr(imp("Dato")), "")) Then
                            ListaSacod.Add(imp("sa_cod").ToString & "|" & imp("ff_cod").ToString & "|" & If(IsNumeric(imp("lav_cod")) AndAlso CInt(imp("lav_cod")) = LAVCOD_FASI_FENOLOGICHE, CStr(imp("Dato")), ""))
                        End If
                End Select

            Next


            Dim listaOpCollegate As New List(Of Operazione_Agenda)
            Dim Id_Agenda As Integer

            If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura Then
                Select Case objParametriAgenda.Lav_Cod
                    Case LAVCOD_FASI_FENOLOGICHE
                        Dim objSequenze As New Agro_Sequenze
                        'Raccoglitore_Cod = objSequenze.Agronica_SequenzaTabelle_NuovoID("Raccoglitore",
                        '                               objParametri_Server)

                        'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                        Raccoglitore_Cod = objSequenze.NuovoId_Tabella("Raccoglitore", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server)

                        objSequenze = Nothing
                End Select
            End If

            'Ciclo per ogni centro aziendale
            For Each sacod As String In ListaSacod

                Dim Agenda As Operazione_Agenda = Nothing

                Select Case objParametriAgenda.Lav_Cod

                    Case LAVCOD_RILIEVO_ERBE_INFESTANTI, LAVCOD_RILIEVO_AVVERSITA_CAMPO, LAVCOD_RILIEVO_INDICI_MATURITA, LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA, LAVCOD_DANNI_RACCOLTA

                        Agenda = CreaOggettoAgendaRilievo(Split(sacod, ("|"))(0), objParametriAgenda.Lav_Cod, objParametriAgenda.Id_Agenda, Raccoglitore_Cod)

                    Case LAVCOD_FASI_FENOLOGICHE

                        'ESTRAGGO L'ID AGENDA SE IN MODIFICA
                        Dim idAg As Integer = 0
                        If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then

                            Dim chiave As String = objParametriAgenda.Piva & "|" & Split(sacod, ("|"))(0) & "|" & objParametriAgenda.Lav_Cod & "|0" & "|1" & "|" & Split(sacod, ("|"))(1) & "|" & If(IsNumeric(objParametriAgenda.Lav_Cod) AndAlso objParametriAgenda.Lav_Cod = LAVCOD_FASI_FENOLOGICHE, Split(sacod, ("|"))(2), "") 'Split(sacod, ("|"))(2)

                            'Se presente, ripristino il vecchio id_agenda
                            If htOldIdAgenda.ContainsKey(chiave) Then
                                idAg = htOldIdAgenda(chiave)
                            End If

                        End If

                        Agenda = CreaOggettoAgendaRilievo(Split(sacod, ("|"))(0), objParametriAgenda.Lav_Cod, idAg, Raccoglitore_Cod, Split(sacod, ("|"))(1), Split(sacod, ("|"))(2))

                        Id_Agenda = objAgendaScrivi.Scrivi(Agenda, objParametri_Server, flagUsaOraReale:=True)

                        'Riscrivo i vecchi costi
                        Dim mdRif_helper As New Agenda_Movimenti_Dettagli_Riferimenti_Helper
                        For Each mdRif As Movimento_Dettaglio_Riferimento In lista_mdRif2
                            If mdRif.Lav_Cod_Rif = LAVCOD_COSTI_CDG Then
                                mdRif_helper.Scrivi(mdRif, objParametri_Server)
                                Exit For
                            End If
                        Next

                    Case LAVCOD_VISITA

                        '------------------------------------------------
                        '----- Operazioni collegate
                        '------------------------------------------------
                        'Estraggo la lista del lav_cod
                        Dim listaRilieviXSa_Cod As JArray = LeggiGrigliaRilievi(Split(sacod, ("|"))(0))
                        Dim listaLavCod As List(Of String) = (From x In listaRilieviXSa_Cod Select CStr(x("lav_cod"))).Distinct().ToList()


                        'Creo le operazioni collegate
                        For Each lav As String In listaLavCod

                            Select Case lav

                                Case LAVCOD_RILIEVO_ERBE_INFESTANTI, LAVCOD_RILIEVO_AVVERSITA_CAMPO, LAVCOD_RILIEVO_INDICI_MATURITA, LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA

                                    'ESTRAGGO L'ID AGENDA SE IN MODIFICA
                                    Dim idAg As Integer = 0
                                    If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then

                                        'Nel caso dei rilievi, le destinazioni ci sono sempre, metto quindi 1 come ultima cifra
                                        Dim chiave As String = objParametriAgenda.Piva & "|" & Split(sacod, ("|"))(0) & "|" & If(lav.Contains("|"), lav, lav & "|0") & "|1"

                                        'Se presente, ripristino il vecchio id_agenda
                                        If htOldIdAgenda.ContainsKey(chiave) Then
                                            idAg = htOldIdAgenda(chiave)
                                        End If
                                    End If

                                    Dim AgendaCollegata As Operazione_Agenda = CreaOggettoAgendaRilievo(Split(sacod, ("|"))(0), lav, idAg, 0)

                                    If IsNothing(AgendaCollegata) Then
                                        Throw New Exception(Resources.AgronicaAgenda_2010.NonÈStatoPossibileCreareLOperazione)
                                    End If

                                    Dim Id_Agenda_Collegato As Integer = objAgendaScrivi.Scrivi(AgendaCollegata, objParametri_Server, flagUsaOraReale:=True)
                                    AgendaCollegata.Id_Agenda = Id_Agenda_Collegato

                                    listaOpCollegate.Add(AgendaCollegata)

                                Case LAVCOD_FASI_FENOLOGICHE


                                    Dim listaFasi As List(Of String) = (From x In righeModificateArray Where CStr(x("lav_cod")) = LAVCOD_FASI_FENOLOGICHE AndAlso CStr(x("sa_cod")) = sacod
                                                                        Select CStr(x("piva")) & "|" _
                                                                                         & CStr(x("sa_cod")) & "|" _
                                                                                         & CStr(x("ff_cod")) & "|" _
                                                                                         & CStr(x("Dato"))
                                                            ).Distinct().ToList()

                                    For Each fase As String In listaFasi

                                        'ESTRAGGO L'ID AGENDA SE IN MODIFICA
                                        Dim idAg As Integer = 0
                                        If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then

                                            'Nel caso dei rilievi, le destinazioni ci sono sempre, metto quindi 1 come ultima cifra
                                            'Dim chiave As String = objParametriAgenda.Piva & "|" & Split(sacod, ("|"))(0) & "|" & If(lav.Contains("|"), lav, lav & "|0") & "|1"
                                            Dim chiave As String = objParametriAgenda.Piva & "|" & Split(sacod, ("|"))(0) & "|" & If(lav.Contains("|"), lav, lav & "|0") & "|1" & "|" & Split(fase, ("|"))(2) & "|" & Split(fase, ("|"))(3) 'Split(sacod, ("|"))(2)

                                            'Se presente, ripristino il vecchio id_agenda
                                            If htOldIdAgenda.ContainsKey(chiave) Then
                                                idAg = htOldIdAgenda(chiave)
                                            End If
                                        End If

                                        Dim AgendaCollegata As Operazione_Agenda = CreaOggettoAgendaRilievo(Split(sacod, ("|"))(0), lav, idAg, Raccoglitore_Cod, Split(fase, ("|"))(2), Split(fase, ("|"))(3))

                                        If IsNothing(AgendaCollegata) Then
                                            Throw New Exception(Resources.AgronicaAgenda_2010.NonÈStatoPossibileCreareLOperazione)
                                        End If

                                        Dim Id_Agenda_Collegato As Integer = objAgendaScrivi.Scrivi(AgendaCollegata, objParametri_Server, flagUsaOraReale:=True)
                                        AgendaCollegata.Id_Agenda = Id_Agenda_Collegato

                                        listaOpCollegate.Add(AgendaCollegata)

                                    Next

                                Case Else

                                    If lav.StartsWith(LAVCOD_ALTRE_OPERAZIONI) Then 'Visita Personalizzata

                                        Dim righeModificateArraySaCod As JArray = LeggiGrigliaRilievi(Split(sacod, ("|"))(0), lav)
                                        'Dim righeModificateArraySoloCentro As JArray = (From x In righeModificateArraySaCod.AsEnumerable Where CInt(x("appezza")) = 0 AndAlso CInt(x("id_reg")) = 0 Select x)
                                        'Dim righeModificateArrayAppezzamenti As JArray = (From x In righeModificateArraySaCod.AsEnumerable Where CInt(x("appezza")) <> 0 AndAlso CInt(x("id_reg")) <> 0 Select x)

                                        Dim righeModificateArraySoloCentro As New JArray
                                        Dim righeModificateArrayAppezzamenti As New JArray
                                        For Each x As JObject In righeModificateArraySaCod
                                            If CInt(x("appezza")) = 0 AndAlso CInt(x("id_reg")) = 0 Then
                                                righeModificateArraySoloCentro.Add(x)
                                            Else
                                                righeModificateArrayAppezzamenti.Add(x)
                                            End If
                                        Next

                                        'Se c'è una visita su centro Aziendale
                                        If righeModificateArraySoloCentro.Count > 0 Then

                                            'ESTRAGGO L'ID AGENDA SE IN MODIFICA
                                            Dim idAg As Integer = 0
                                            If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then

                                                'In questo caso mancano le destinazioni, quindi metto l'ultimo valore a 0
                                                Dim chiave As String = objParametriAgenda.Piva & "|" & Split(sacod, ("|"))(0) & "|" & If(lav.Contains("|"), lav, lav & "|0") & "|0"

                                                'Se presente, ripristino il vecchio id_agenda
                                                If htOldIdAgenda.ContainsKey(chiave) Then
                                                    idAg = htOldIdAgenda(chiave)
                                                End If
                                            End If

                                            Dim AgendaCollegata As Operazione_Agenda = CreaOggettoAgendaAltreLavorazioni(Split(sacod, ("|"))(0), lav, idAg, enum_OggettoVisita.CentroAz)

                                            If IsNothing(AgendaCollegata) Then
                                                Throw New Exception(Resources.AgronicaAgenda_2010.NonÈStatoPossibileCreareLOperazione)
                                            End If

                                            Dim Id_Agenda_Collegato As Integer = objAgendaScrivi.Scrivi(AgendaCollegata, objParametri_Server, flagUsaOraReale:=True)
                                            AgendaCollegata.Id_Agenda = Id_Agenda_Collegato

                                            listaOpCollegate.Add(AgendaCollegata)
                                        End If

                                        'Se c'è una visita su appezzamenti
                                        If righeModificateArrayAppezzamenti.Count > 0 Then

                                            'ESTRAGGO L'ID AGENDA SE IN MODIFICA
                                            Dim idAg As Integer = 0
                                            If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then

                                                'In questo caso ci sono le destinazioni, quindi metto l'ultimo valore a 1
                                                Dim chiave As String = objParametriAgenda.Piva & "|" & Split(sacod, ("|"))(0) & "|" & If(lav.Contains("|"), lav, lav & "|0") & "|1"

                                                'Se presente, ripristino il vecchio id_agenda
                                                If htOldIdAgenda.ContainsKey(chiave) Then
                                                    idAg = htOldIdAgenda(chiave)
                                                End If
                                            End If

                                            Dim AgendaCollegata As Operazione_Agenda = CreaOggettoAgendaAltreLavorazioni(Split(sacod, ("|"))(0), lav, idAg, enum_OggettoVisita.Appezzamenti)

                                            If IsNothing(AgendaCollegata) Then
                                                Throw New Exception(Resources.AgronicaAgenda_2010.NonÈStatoPossibileCreareLOperazione)
                                            End If

                                            Dim Id_Agenda_Collegato As Integer = objAgendaScrivi.Scrivi(AgendaCollegata, objParametri_Server, flagUsaOraReale:=True)
                                            AgendaCollegata.Id_Agenda = Id_Agenda_Collegato

                                            listaOpCollegate.Add(AgendaCollegata)
                                        End If

                                    End If
                            End Select

                        Next

                End Select

                'In tutti i casi tranne la visita le operazioni d'agenda vengono salvate per ogni centro aziendale
                Select Case objParametriAgenda.Lav_Cod

                    Case LAVCOD_VISITA, LAVCOD_FASI_FENOLOGICHE 'scritte sopra e sotto

                    Case Else

                        If IsNothing(Agenda) Then
                            Throw New Exception(Resources.AgronicaAgenda_2010.NonÈStatoPossibileCreareLOperazione)
                        End If

                        If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then

                            'Recupero i vecchi costi CdG prima che vengano cancellati
                            Dim mdr_Rif As New Agenda_Movimenti_Dettagli_Riferimenti_Helper()
                            Dim lista_mdRif As List(Of Movimento_Dettaglio_Riferimento) = mdr_Rif.LeggiRiferimentiAgenda(Agenda.Piva, 0, Agenda.Id_Agenda, 0, "", objParametri_Server)

                            'Dal 06/23 conserviamo la data e l'username di creazione originali per tutti i record Agenda
                            allinea_DataUsernameCreazione(Agenda, objParametri_Server, Tipo_Attivita.QuadernoDiCampagna, 0)

                            Dim CancellataOperazione As Boolean = objAgendaScrivi.Cancella(objParametriAgenda.Piva,
                                                                                     objParametriAgenda.Sa_Cod,
                                                                                     objParametriAgenda.Id_Agenda, False,
                                                                                     objParametri_Server, logCancellazione:=False)

                            'Riscrivo i vecchi costi
                            Dim mdRif_helper As New Agenda_Movimenti_Dettagli_Riferimenti_Helper
                            For Each mdRif As Movimento_Dettaglio_Riferimento In lista_mdRif
                                If mdRif.Lav_Cod_Rif = LAVCOD_COSTI_CDG Then
                                    mdRif_helper.Scrivi(mdRif, objParametri_Server)
                                    Exit For
                                End If
                            Next

                        End If


                        Id_Agenda = objAgendaScrivi.Scrivi(Agenda, objParametri_Server, flagUsaOraReale:=True)

                End Select


            Next

            'Nel caso della visita, l'oggetto visita papà è unico indipendentemente dai centri aziendali
            If objParametriAgenda.Lav_Cod = LAVCOD_VISITA Then
                'Creo l'oggetto visita
                Dim Agenda As Operazione_Agenda = CreaOggettoAgendaVisita(listaOpCollegate)

                If IsNothing(Agenda) Then
                    Throw New Exception(Resources.AgronicaAgenda_2010.NonÈStatoPossibileCreareLOperazione)
                End If

                If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then

                    'Dal 06/23 conserviamo la data e l'username di creazione originali per tutti i record Agenda
                    allinea_DataUsernameCreazione(Agenda, objParametri_Server, Tipo_Attivita.QuadernoDiCampagna, 0)

                    Dim CancellataOperazione As Boolean = objAgendaScrivi.Cancella(objParametriAgenda.Piva,
                                                                                 objParametriAgenda.Sa_Cod,
                                                                                 objParametriAgenda.Id_Agenda, False,
                                                                                 objParametri_Server, logCancellazione:=False)

                End If

                Id_Agenda = objAgendaScrivi.Scrivi(Agenda, objParametri_Server, flagUsaOraReale:=True)

            End If

            'If objParametriAgenda.Sa_Cod <> "0" AndAlso TipoSalvataggio = 1 Then
            '    objParametriAgenda.Id_Agenda = Id_Agenda
            'End If

            If ListaSacod.Count = 1 AndAlso (TipoSalvataggio = enum_Tipo_Salvataggio.Salva_e_Esci OrElse TipoSalvataggio = enum_Tipo_Salvataggio.Salva_e_Vai_ai_Costi) Then
                objParametriAgenda.Id_Agenda = Id_Agenda
            End If

            res = True


            ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)

        Catch ex As Exception

            ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)

            If ex.Message <> "" Then
                messaggio_errore = Resources.AgronicaAgenda_2010.AttenzioneLOperazioneNonÈStataRegistrataBr & ex.Message
            Else
                'alert
            End If

            res = False

        Finally

            'chiudi connessione
            ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

        End Try


        Return res


    End Function

    Private Function estraiJSONgrigliaRilievi() As JArray
        Dim righeModificate As String = hdRilievi.Value

        If String.IsNullOrEmpty(righeModificate) OrElse righeModificate = "[]" Then
            Messaggi.AgroMsgBox("", Page, , CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            Return Nothing
        End If

        Dim reader As JsonReader = New JsonTextReader(New System.IO.StringReader(righeModificate))
        reader.DateParseHandling = DateParseHandling.None
        Dim righeModificateArray As JArray = JArray.Load(reader)

        Return righeModificateArray
    End Function

    Private Function CreaOggettoAgendaAltreLavorazioni(Sa_Cod As Integer, lav_cod_selez As String, idAg As Integer, OggettoDellaVisita As enum_OggettoVisita) As Operazione_Agenda

        Dim Agenda As New Operazione_Agenda

        '---------------------------------------
        ' recupero la DATA
        If objParametriAgenda.Data = AGRODATAINIZIO Then
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.IndicareUnaData, Page, ,
                                CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            Return Nothing
        End If

        Dim Data As Date = New Date(objParametriAgenda.Data.Year, objParametriAgenda.Data.Month, objParametriAgenda.Data.Day)
        Dim Ora As Date = objParametriAgenda.Data.AddSeconds(-objParametriAgenda.Data.Second).AddMilliseconds(-objParametriAgenda.Data.Millisecond)

        '---------------------------------------
        ' recupero la SPECIE
        Dim Veg_Cod As String = ""
        Dim Veg_Des As String = ""
        If objParametriAgenda.Veg_Cod.Split("/")(0) <> "-1" Then
            Veg_Cod = objParametriAgenda.Veg_Cod.Split("/")(0)
            Veg_Des = Master.Property_ComboSpecie.Testo_Combo
        End If

        '---------------------------------------
        ' recupero la OPERAZIONE
        Dim Lav_Cod As String = ""
        Dim Lav_Des As String = ""
        Dim Id_Attivita As Integer = 0
        If Not lav_cod_selez.StartsWith(LAVCOD_ALTRE_OPERAZIONI & "|") Then
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.SelezionareUnOperazione, Page, ,
                                CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            Return Nothing
        Else
            Lav_Cod = LAVCOD_ALTRE_OPERAZIONI
            Id_Attivita = lav_cod_selez.Replace(LAVCOD_ALTRE_OPERAZIONI & "|", "")

            Dim a_R As New AgronicaCoreContabDAL.Attivita_R
            Lav_Des = a_R.AttivitaDes_From_AttivitaCod(Id_Attivita, objParametri_Server)
        End If

        Dim righeModificateArray As JArray = LeggiGrigliaRilievi(Sa_Cod, lav_cod_selez)


        Select Case OggettoDellaVisita
            Case enum_OggettoVisita.Impresa
                'Già a posto, filtra con sa_cod=0

            Case enum_OggettoVisita.CentroAz
                'Conservo quelli con appezza = 0 e id_reg = 0
                Dim righeModificateArraySoloCentro As New JArray
                For Each x As JObject In righeModificateArray
                    If CInt(x("appezza")) = 0 AndAlso CInt(x("id_reg")) = 0 Then
                        righeModificateArraySoloCentro.Add(x)
                    End If
                Next

                righeModificateArray = righeModificateArraySoloCentro.DeepClone()
                'righeModificateArray = (From x In righeModificateArray Where CInt(x("appezza")) = 0 AndAlso CInt(x("id_reg")) = 0)

            Case enum_OggettoVisita.Appezzamenti
                'Conservo quelli con appezza <> 0 e id_reg <> 0
                Dim righeModificateArrayAppezzamenti As New JArray
                For Each x As JObject In righeModificateArray
                    If CInt(x("appezza")) <> 0 AndAlso CInt(x("id_reg")) <> 0 Then
                        righeModificateArrayAppezzamenti.Add(x)
                    End If
                Next

                righeModificateArray = righeModificateArrayAppezzamenti.DeepClone()
                'righeModificateArray = (From x In righeModificateArray Where CInt(x("appezza")) <> 0 AndAlso CInt(x("id_reg")) <> 0)

        End Select

        '---------------------------------------

        Dim BaseCode As Integer = 0
        Dim TopCode As Integer = 0

        '------------------------------------------------
        '----- Calcolo i valori di BaseCode e TopCode
        '------------------------------------------------
        UtilityProvider.Calcola_BaseCode_TopCode(BaseCode, TopCode, Session("ASG_ProgressivoGIAS"))

        '------------------------------------------------
        '----- AGENDA
        '------------------------------------------------
        Dim Movimento As Movimento
        Dim Movimento_Dettaglio As Movimento_Dettaglio
        Dim Movimento_Destinazione As Movimento_Destinazione

        Dim ListaVarieta As List(Of String) = (From x In righeModificateArray Select CStr(x("Cul_Des"))).Distinct().ToList()
        Dim StrVarieta As String = String.Join(", ", ListaVarieta)

        Agenda = New Operazione_Agenda

        Agenda.Tipo_Operazione = objParametriAgenda.Tipo_Operazione
        'Agenda.Id_Agenda = 0
        Agenda.Id_Agenda = idAg
        Agenda.Data = Data
        Agenda.Piva = objParametriAgenda.Piva
        Agenda.Sa_Cod = Sa_Cod
        Agenda.Lav_Cod = Lav_Cod
        Agenda.Des_Lib = Lav_Des & " (" & Veg_Des & "  [" & StrVarieta & "])"
        Agenda.Id_Attivita = Id_Attivita

        Agenda.BaseCode = BaseCode
        Agenda.TopCode = TopCode

        '---------------------------------------
        ' recupero i Consigli
        Dim ListaConsigli As List(Of Nota) = CType(Master, OperazioneBootstrap).GetConsigli()
        If ListaConsigli.Count > 0 Then
            Agenda.Note = New List(Of Nota)
            For i = 0 To ListaConsigli.Count - 1
                Agenda.Note.Add(New Nota With {.Id_Agenda = Agenda.Id_Agenda, .Nota_Cod = ListaConsigli(i).Nota_Cod})
            Next
        End If

        '------------------------------------------------
        '----- GPS AGENDA
        '------------------------------------------------
        If OggettoDellaVisita <> enum_OggettoVisita.Appezzamenti Then
            Dim wkt As String = Me.Master.Property_txtPosizione
            If Not String.IsNullOrEmpty(wkt) Then
                Dim vWkt As String() = wkt.Split(",")
                wkt = "POINT (" & vWkt(1).Trim() & ", " & vWkt(0).Trim() & ")"
                Agenda.GisWkt = wkt
                Agenda.GisWktGps = "1"
                Agenda.GisWktSistemaRiferimento = "-1"
                Agenda.GisLayerCod = enum_Gis_LayerElementiGrafici_std.Op_Agenda
                Agenda.GisTipoEntita_cod = enum_GIS2012_TipoEntita.OpAgenda
            End If
        End If

        '------------------------------------------------
        '----- MOVIMENTI
        '------------------------------------------------
        Agenda.Movimenti = New List(Of Movimento)

        'COSTI ACCESSORI 
        For i = 0 To objParametriAgenda.Movimenti.Count - 1
            objParametriAgenda.Movimenti(i).Id_Agenda = Agenda.Id_Agenda
            objParametriAgenda.Movimenti(i).Sa_Cod = Agenda.Sa_Cod
            ' commentato Nico 25/03/2014
            If Not IsNothing(objParametriAgenda.Movimenti(i).Movimenti_Dettagli) Then
                For j As Integer = 0 To objParametriAgenda.Movimenti(i).Movimenti_Dettagli.Count - 1
                    objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Id_Agenda = Agenda.Id_Agenda
                    If objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Sa_Cod = 0 Then
                        objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Sa_Cod = Agenda.Sa_Cod
                    End If
                Next
            End If

            If Not IsNothing(objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici) Then
                For j As Integer = 0 To objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici.Count - 1
                    objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Id_Agenda = Agenda.Id_Agenda
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

        Movimento.Id_Agenda = Agenda.Id_Agenda
        Movimento.Piva = Agenda.Piva
        Movimento.Sa_Cod = Agenda.Sa_Cod
        Movimento.Data = Data
        Movimento.Ora = Ora
        Movimento.Lav_Cod = Lav_Cod
        Movimento.Cau_Mov = CAU_LAVORAZIONE
        Movimento.Mov_Desc = txtNote.Text 'CType(Master, OperazioneBootstrap).GetNota()

        Movimento.BaseCode = BaseCode
        Movimento.TopCode = TopCode

        Agenda.Movimenti.Add(Movimento)



        '------------------------------------------------
        '----- MOVIMENTI DETTAGLI 
        '------------------------------------------------
        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli = New List(Of Movimento_Dettaglio)

        Movimento_Dettaglio = New Movimento_Dettaglio

        Movimento_Dettaglio.Id_Agenda = Agenda.Id_Agenda
        Movimento_Dettaglio.Piva = Agenda.Piva
        Movimento_Dettaglio.Sa_Cod = Agenda.Sa_Cod
        Movimento_Dettaglio.Data = Data
        Movimento_Dettaglio.Lav_Cod = Lav_Cod
        Movimento_Dettaglio.Cau_Mov = CAU_LAVORAZIONE
        Movimento_Dettaglio.Mov_Det_Des = righeModificateArray(0).Item("Dato")

        Movimento_Dettaglio.Contabilizzato = NONCONTABILE

        Movimento_Dettaglio.BaseCode = BaseCode
        Movimento_Dettaglio.TopCode = TopCode

        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Add(Movimento_Dettaglio)

        'AGGIUNGO LE DESTINAZIONI SOLO SE SONO NEL CASO IN CUI L'OGGETTO DELLA VISITA SIANO GLI APPEZZAMENTI
        If OggettoDellaVisita = enum_OggettoVisita.Appezzamenti Then

            '------------------------------------------------
            '----- MOVIMENTI DESTINAZIONI 
            '------------------------------------------------
            Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(0).Movimenti_Destinazioni = New List(Of Movimento_Destinazione)

            Dim xCalcolo_QD_SuperficieTotale As Decimal = (
                From ST In righeModificateArray
                Select CType(ST.Item("Qta2"), Decimal)
            ).Sum


            For Each cRow In righeModificateArray

                Movimento_Destinazione = New Movimento_Destinazione
                Movimento_Destinazione.Id_Agenda = Agenda.Id_Agenda
                Movimento_Destinazione.Data = Data
                Movimento_Destinazione.Piva = cRow.Item("piva")
                Movimento_Destinazione.Sa_Cod = cRow.Item("sa_cod")
                Movimento_Destinazione.Appezza = cRow.Item("appezza")
                Movimento_Destinazione.Id_Destinazione = cRow.Item("id_reg")
                Movimento_Destinazione.Tipo = 0
                Movimento_Destinazione.Qta2 = cRow.Item("Qta2")
                Movimento_Destinazione.BaseCode = BaseCode
                Movimento_Destinazione.TopCode = TopCode
                'Movimento_Destinazione.Programmazione_Entita_Cod = cRow.Item("programmazione_entita_cod")

                If xCalcolo_QD_SuperficieTotale <> 0 Then
                    Movimento_Destinazione.QuotaDistribuzione = Movimento_Destinazione.Qta2 / xCalcolo_QD_SuperficieTotale
                End If

                '------------------------------------------------
                '----- GPS DESTINAZIONI
                '------------------------------------------------
                Dim wkt As String = Me.Master.Property_txtPosizione
                If Not String.IsNullOrEmpty(wkt) Then
                    Dim vWkt As String() = wkt.Split(",")
                    wkt = "POINT (" & vWkt(1).Trim() & ", " & vWkt(0).Trim() & ")"
                    Movimento_Destinazione.GisWkt = wkt
                    Movimento_Destinazione.GisWktGps = "1"
                    Movimento_Destinazione.GisWktSistemaRiferimento = "-1"
                    Movimento_Destinazione.GisLayerCod = enum_Gis_LayerElementiGrafici_std.Op_Agenda
                    Movimento_Destinazione.GisTipoEntita_cod = enum_GIS2012_TipoEntita.OpAgenda
                End If

                Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(0).Movimenti_Destinazioni.Add(Movimento_Destinazione)

            Next

        End If

        Return Agenda

    End Function

    'Private Function CreaOggettoAgendaRilievo(ByVal Sa_Cod As Integer, lav_cod_selez As Integer, veg_cod_selez As String, idAg As Integer) As Operazione_Agenda
    Private Function CreaOggettoAgendaRilievo(ByVal Sa_Cod As Integer, lav_cod_selez As Integer, idAg As Integer, Raccoglitore_Cod As Integer,
                                              Optional ByVal ff_cod As Integer = 0, Optional ByVal Dato As String = "") As Operazione_Agenda

        '---------------------------------------
        ' recupero la DATA
        If objParametriAgenda.Data = AGRODATAINIZIO Then
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.IndicareUnaData, Page, ,
                                CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            Return Nothing
        End If

        Dim Data As Date = New Date(objParametriAgenda.Data.Year, objParametriAgenda.Data.Month, objParametriAgenda.Data.Day)
        Dim Ora As Date = objParametriAgenda.Data.AddSeconds(-objParametriAgenda.Data.Second).AddMilliseconds(-objParametriAgenda.Data.Millisecond)

        '---------------------------------------
        ' recupero la SPECIE
        Dim Veg_Cod As String = ""
        Dim Veg_Des As String = ""
        'If veg_cod_selez = "-1" AndAlso objParametriAgenda.Lav_Cod <> LAVCOD_VISITA Then
        If objParametriAgenda.Veg_Cod.Split("/")(0) = "-1" AndAlso objParametriAgenda.Lav_Cod <> LAVCOD_VISITA Then
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.SelezionareUnaSpecieVegetale, Page, ,
                                CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            Return Nothing
        Else
            Veg_Cod = objParametriAgenda.Veg_Cod.Split("/")(0)
            'Veg_Cod = veg_cod_selez
            Veg_Des = Master.Property_ComboSpecie.Testo_Combo
        End If

        '---------------------------------------
        ' recupero la OPERAZIONE
        Dim Lav_Cod As String = ""
        Dim Lav_Des As String = ""
        If Not IsNumeric(lav_cod_selez) Then
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.SelezionareUnOperazione, Page, ,
                                CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            Return Nothing
        Else
            Lav_Cod = lav_cod_selez
            Lav_Des = Master.Property_ComboOperazione.ddl_Operazioni.Items.FindByValue(lav_cod_selez).Text
        End If


        '---------------------------------------
        'Dim righeModificateArray As JArray = LeggiGrigliaRilievi(Sa_Cod, Lav_Cod)
        Dim righeModificateArray As JArray = LeggiGrigliaRilievi(Sa_Cod, Lav_Cod, ff_cod, Dato)

        Dim BaseCode As Integer = 0
        Dim TopCode As Integer = 0

        '------------------------------------------------
        '----- Calcolo i valori di BaseCode e TopCode
        '------------------------------------------------
        UtilityProvider.Calcola_BaseCode_TopCode(BaseCode, TopCode, Session("ASG_ProgressivoGIAS"))

        '  Vanni, 03/06/2014 17:25:31: recupera la info sull'ora
        Dim xH As String = ""
        If objParametriAgenda.Des_lib.StartsWith("H") AndAlso objParametriAgenda.Des_lib.Contains("-") Then
            Dim app As String() = objParametriAgenda.Des_lib.Split("-")
            xH = app(0).TrimEnd(" ") & " - "
        End If
        'fine Vanni, 03/06/2014 17:25:31: recupera la info sull'ora  

        '------------------------------------------------
        '----- AGENDA
        '------------------------------------------------
        Dim Agenda As Operazione_Agenda
        Dim Movimento As Movimento
        Dim Movimento_Dettaglio_Tecnico As Movimento_Dettaglio_Tecnico
        Dim Movimento_Dettaglio As Movimento_Dettaglio
        Dim Movimento_Destinazione As Movimento_Destinazione

        Dim ListaVarieta As List(Of String) = (From x In righeModificateArray Select CStr(x("Cul_Des"))).Distinct().ToList()
        Dim StrVarieta As String = String.Join(", ", ListaVarieta)

        Agenda = New Operazione_Agenda

        Agenda.Tipo_Operazione = objParametriAgenda.Tipo_Operazione
        'Agenda.Id_Agenda = 0
        Agenda.Id_Agenda = idAg
        Agenda.Data = Data
        Agenda.Piva = objParametriAgenda.Piva
        Agenda.Sa_Cod = Sa_Cod
        Agenda.Lav_Cod = Lav_Cod
        Agenda.Des_Lib = xH & Lav_Des & " (" & Veg_Des & "  [" & StrVarieta & "])"


        '(26/11/2018 fede) introdotto raccoglitore solo per fasi per ora
        Agenda.Raccoglitore_Cod = Raccoglitore_Cod


        Agenda.BaseCode = BaseCode
        Agenda.TopCode = TopCode


        '------------------------------------------------
        '----- NOTE
        '------------------------------------------------
        Dim ListaConsigli As List(Of Nota) = CType(Master, OperazioneBootstrap).GetConsigli()
        If ListaConsigli.Count > 0 Then
            Agenda.Note = New List(Of Nota)
            For i = 0 To ListaConsigli.Count - 1
                Agenda.Note.Add(New Nota With {.Id_Agenda = Agenda.Id_Agenda, .Nota_Cod = ListaConsigli(i).Nota_Cod})
            Next
        End If


        '------------------------------------------------
        '----- MOVIMENTI
        '------------------------------------------------
        Agenda.Movimenti = New List(Of Movimento)


        'COSTI ACCESSORI
        For i = 0 To objParametriAgenda.Movimenti.Count - 1
            objParametriAgenda.Movimenti(i).Id_Agenda = Agenda.Id_Agenda
            objParametriAgenda.Movimenti(i).Sa_Cod = Agenda.Sa_Cod

            If Not IsNothing(objParametriAgenda.Movimenti(i).Movimenti_Dettagli) Then
                For j As Integer = 0 To objParametriAgenda.Movimenti(i).Movimenti_Dettagli.Count - 1
                    objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Id_Agenda = Agenda.Id_Agenda
                    If objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Sa_Cod = 0 Then
                        objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Sa_Cod = Agenda.Sa_Cod
                    End If
                Next
            End If

            If Not IsNothing(objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici) Then
                For j As Integer = 0 To objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici.Count - 1
                    objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Id_Agenda = Agenda.Id_Agenda
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

        Movimento.Id_Agenda = Agenda.Id_Agenda
        Movimento.Piva = Agenda.Piva
        Movimento.Sa_Cod = Agenda.Sa_Cod
        Movimento.Data = Data
        Movimento.Ora = Ora
        Movimento.Lav_Cod = Lav_Cod
        'Movimento.Cau_Mov = objParametriAgenda.Cau_Mov
        Movimento.Mov_Desc = txtNote.Text 'CType(Master, OperazioneBootstrap).GetNota()
        Movimento.BaseCode = BaseCode
        Movimento.TopCode = TopCode

        Select Case Lav_Cod
            Case LAVCOD_RILIEVO_ERBE_INFESTANTI, LAVCOD_FASI_FENOLOGICHE, LAVCOD_RILIEVO_AVVERSITA_CAMPO
                Movimento.Cau_Mov = enum_Agenda_Causali.RILIEVO_CAMPO
            Case LAVCOD_RILIEVO_INDICI_MATURITA, LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA, LAVCOD_DANNI_RACCOLTA
                Movimento.Cau_Mov = enum_Agenda_Causali.RILIEVO_RACCOLTA
            Case LAVCOD_VISITA
                Movimento.Cau_Mov = CAU_VISITE_ISPETTIVE
        End Select

        Movimento.Num_Protocollo = 0

        Select Case objParametriAgenda.Disciplinare
            Case NessunDpiNessunaEtichetta
                Movimento.Num_Protocollo = 0
            Case "0"
                Movimento.Num_Protocollo = -1
            Case "-2"   'BIO
                Movimento.Num_Protocollo = -2
            Case Else   'DPI
                Dim Array() As String = Split(objParametriAgenda.Disciplinare, "/")
                Movimento.Num_Protocollo = Array(0)
                If Array.Length > 1 AndAlso Array(1) IsNot Nothing Then
                    Movimento.Doc_Numero = Array(1)
                End If
                If Array.Length > 4 AndAlso Array(4) IsNot Nothing Then
                    Movimento.Disciplinare_PubblicoPrivato = Array(4)
                End If
        End Select

        Agenda.Movimenti.Add(Movimento)


        '------------------------------------------------
        '----- MOVIMENTI DETTAGLI
        '------------------------------------------------

        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli = New List(Of Movimento_Dettaglio)

        'Estraggo le avversità/Udm
        Dim listaAvvUdm As New List(Of String)

        Select Case Lav_Cod
            Case LAVCOD_RILIEVO_ERBE_INFESTANTI
                Throw New NotImplementedException
            Case LAVCOD_FASI_FENOLOGICHE
                For Each cRow In righeModificateArray
                    Dim elem As String = cRow("ff_cod").ToString()
                    If Not listaAvvUdm.Contains(elem) Then
                        listaAvvUdm.Add(elem)
                    End If
                Next
            Case LAVCOD_RILIEVO_AVVERSITA_CAMPO
                For Each cRow In righeModificateArray
                    Dim elem As String = cRow("av_cod").ToString() & "|" & cRow("udm_cod").ToString()
                    If Not listaAvvUdm.Contains(elem) Then
                        listaAvvUdm.Add(elem)
                    End If
                Next
            Case LAVCOD_RILIEVO_INDICI_MATURITA, LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA
                For Each cRow In righeModificateArray
                    Dim elem As String = cRow("ind_mat_cod").ToString() & "|" & cRow("udm_cod").ToString()
                    If Not listaAvvUdm.Contains(elem) Then
                        listaAvvUdm.Add(elem)
                    End If
                Next
            Case LAVCOD_DANNI_RACCOLTA
                For Each cRow In righeModificateArray
                    Dim elem As String = cRow("dr_cod").ToString() & "|" & cRow("udm_cod").ToString()
                    If Not listaAvvUdm.Contains(elem) Then
                        listaAvvUdm.Add(elem)
                    End If
                Next
            Case Else
                Throw New NotImplementedException
        End Select

        'Per ogni avversità/udm creo un mov/det
        For Each avvUdm As String In listaAvvUdm

            Dim avv As Integer = avvUdm.Split("|")(0)
            Dim udm As Integer = If(avvUdm.Split("|").Length > 1, avvUdm.Split("|")(1), 0)

            '------------------------------
            '----- MOVIMENTO DETTAGLIO-----
            '------------------------------
            Movimento_Dettaglio = New Movimento_Dettaglio
            Movimento_Dettaglio.Id_Agenda = Agenda.Id_Agenda
            Movimento_Dettaglio.Piva = Agenda.Piva
            Movimento_Dettaglio.Sa_Cod = Agenda.Sa_Cod
            Movimento_Dettaglio.Elem_Cod = 0
            Movimento_Dettaglio.Pro_Cod = 0
            'impostazioni in base alla lavorazione
            Select Case Lav_Cod
                Case LAVCOD_RILIEVO_ERBE_INFESTANTI
                    Movimento_Dettaglio.Udm_Cod = 10 'sempre piante per metro quadro
                Case LAVCOD_FASI_FENOLOGICHE
                    'Non serve UDM
                Case LAVCOD_RILIEVO_AVVERSITA_CAMPO, LAVCOD_DANNI_RACCOLTA
                    Movimento_Dettaglio.Udm_Cod = udm 'cRow("udm_cod")
                Case LAVCOD_RILIEVO_INDICI_MATURITA, LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA
                    'Movimento_Dettaglio.Udm_Cod = udm
                Case Else
                    Throw New NotImplementedException
            End Select
            Movimento_Dettaglio.Qta = 0
            Movimento_Dettaglio.Contabilizzato = 1 '1 se data<corrente -1 se data futura
            Movimento_Dettaglio.Pendente = 3 'valore copiato da agenda vecchia
            Movimento_Dettaglio.Extra_Date = AGRODATAINIZIO 'valore copiato da agenda vecchia
            Movimento_Dettaglio.Anno = 1900 'valore copiato da agenda vecchia
            Movimento_Dettaglio.Data = Agenda.Data
            Movimento_Dettaglio.BaseCode = Agenda.BaseCode
            Movimento_Dettaglio.TopCode = Agenda.TopCode

            '------------------------------
            '----- MOVIMENTO DET TECNICO --
            '------------------------------
            Movimento_Dettaglio_Tecnico = New Movimento_Dettaglio_Tecnico
            Movimento_Dettaglio_Tecnico.Id_Agenda = Agenda.Id_Agenda
            Movimento_Dettaglio_Tecnico.Piva = Agenda.Piva
            Movimento_Dettaglio_Tecnico.Sa_Cod = Agenda.Sa_Cod
            Movimento_Dettaglio_Tecnico.Data = Agenda.Data
            Movimento_Dettaglio_Tecnico.Ditta_cod = 0
            Movimento_Dettaglio_Tecnico.Dose = 0
            Movimento_Dettaglio_Tecnico.Freatimetro = 0
            Movimento_Dettaglio_Tecnico.BaseCode = Agenda.BaseCode
            Movimento_Dettaglio_Tecnico.TopCode = Agenda.TopCode
            Movimento_Dettaglio_Tecnico.Inn1_data = New Date
            Movimento_Dettaglio_Tecnico.Sigla_av = "0"

            Select Case Lav_Cod
                Case LAVCOD_RILIEVO_ERBE_INFESTANTI
                    ' sempre piante per metro quadro
                    Movimento_Dettaglio_Tecnico.dett_cod = 10
                    Movimento_Dettaglio_Tecnico.Av_Gru = avv
                Case LAVCOD_FASI_FENOLOGICHE
                    Movimento_Dettaglio_Tecnico.ff_classe = avv
                    'Movimento_Dettaglio_Tecnico.ExtraStr = cRow("Descrizione")
                Case LAVCOD_RILIEVO_AVVERSITA_CAMPO
                    Movimento_Dettaglio_Tecnico.dett_cod = udm
                    Movimento_Dettaglio_Tecnico.Av_Cod = avv
                    Movimento_Dettaglio_Tecnico.ff_classe = 0
                    Movimento_Dettaglio_Tecnico.Piezo2 = 0
                Case LAVCOD_RILIEVO_INDICI_MATURITA, LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA, LAVCOD_DANNI_RACCOLTA
                    Movimento_Dettaglio_Tecnico.dett_cod = udm
                    Movimento_Dettaglio_Tecnico.ff_classe = avv
                Case Else
                    Throw New NotImplementedException
            End Select

            'aggiungo il movimento dettaglio tecnico al movimento dettaglio
            Movimento_Dettaglio.Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)

            Dim righeFiltrate As New JArray()

            Select Case Lav_Cod
                Case LAVCOD_RILIEVO_ERBE_INFESTANTI
                    Throw New NotImplementedException
                Case LAVCOD_FASI_FENOLOGICHE
                    For Each cRow In righeModificateArray
                        If cRow("ff_cod").ToString() = avv Then
                            righeFiltrate.Add(cRow)
                        End If
                    Next
                Case LAVCOD_RILIEVO_AVVERSITA_CAMPO
                    For Each cRow In righeModificateArray
                        If cRow("av_cod").ToString() = avv AndAlso cRow("udm_cod").ToString() = udm Then
                            righeFiltrate.Add(cRow)
                        End If
                    Next
                Case LAVCOD_RILIEVO_INDICI_MATURITA, LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA
                    For Each cRow In righeModificateArray
                        If cRow("ind_mat_cod").ToString() = avv AndAlso cRow("udm_cod").ToString() = udm Then
                            righeFiltrate.Add(cRow)
                        End If
                    Next
                Case LAVCOD_DANNI_RACCOLTA
                    For Each cRow In righeModificateArray
                        If cRow("dr_cod").ToString() = avv AndAlso cRow("udm_cod").ToString() = udm Then
                            righeFiltrate.Add(cRow)
                        End If
                    Next
                Case Else
                    Throw New NotImplementedException
            End Select



            Dim xCalcolo_QD_SuperficieTotale As Decimal = (
                From ST In righeFiltrate
                Select CType(ST.Item("Qta2"), Decimal)
            ).Sum

            For Each cRow In righeFiltrate
                Dim valoreRilevato As String = cRow("Dato").ToString().Trim()

                'Grilli 16/07/2018 Su indicazione di fabrizio converto il mancato inserimento di un dato come zero perché non viene visto nella stampa
                'e permette di utilizzare i preset senza doverli per forza popolare subito
                If valoreRilevato = "" Then
                    If Lav_Cod = LAVCOD_FASI_FENOLOGICHE Then
                        valoreRilevato = "01/01/1900"
                    Else
                        valoreRilevato = "0"
                    End If
                End If

                Dim udmdes As String = ""
                Select Case Lav_Cod
                    Case LAVCOD_RILIEVO_ERBE_INFESTANTI
                        udmdes = ""
                    Case LAVCOD_FASI_FENOLOGICHE, LAVCOD_RILIEVO_AVVERSITA_CAMPO, LAVCOD_RILIEVO_INDICI_MATURITA, LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA, LAVCOD_DANNI_RACCOLTA
                        udmdes = cRow("Descrizione")
                    Case Else
                        Throw New NotImplementedException
                End Select

                Dim causa As String = ""
                If Not DatoCompatibile(valoreRilevato, udmdes, causa, Lav_Cod) Then
                    Dim agroMsg = String.Format(DirectCast(GetLocalResourceObject("CreaOggettoAgendaRilievo_datoNonValido"), String), valoreRilevato, causa)
                    Messaggi.AgroMsgBox(agroMsg, Page, , CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
                    Return Nothing
                End If

                '-----------------------------------
                '----- MOVIMENTI DESTINAZIONE ------
                '-----------------------------------

                'per ciascun movimento dettaglio creo un figlio mov. destinazione
                Movimento_Destinazione = New Movimento_Destinazione

                Movimento_Destinazione.Id_Agenda = Agenda.Id_Agenda
                Movimento_Destinazione.Piva = cRow("piva")
                Movimento_Destinazione.Sa_Cod = cRow("sa_cod")

                If Movimento_Destinazione.Piva <> Agenda.Piva OrElse Movimento_Destinazione.Sa_Cod <> Agenda.Sa_Cod Then
                    'passa ad altra colonna, perché la colonna riguarda un altro centro
                    Dim cftygv = 0
                Else

                    Movimento_Destinazione.Appezza = cRow("appezza")
                    Movimento_Destinazione.Id_Destinazione = cRow("id_reg")
                    Movimento_Destinazione.mov_destinazioni_graphickey = ""

                    For Each riliev As rilievoAvv In objParametriAgenda.Rilievi
                        If Movimento_Destinazione.Piva = riliev.Impianto.Piva AndAlso
                            Movimento_Destinazione.Sa_Cod = riliev.Impianto.Sa_Cod AndAlso
                            Movimento_Destinazione.Appezza = riliev.Impianto.Appezza AndAlso
                            Movimento_Destinazione.Id_Destinazione = riliev.Impianto.ID_Reg Then

                            Movimento_Destinazione.mov_destinazioni_graphickey = riliev.mov_destinazioni_graphickey

                            Exit For
                        End If
                    Next

                    Movimento_Destinazione.Qta2 = cRow("Qta2") 'Sup trattata

                    'Quota Distribuzione
                    If xCalcolo_QD_SuperficieTotale <> 0 Then
                        Movimento_Destinazione.QuotaDistribuzione = Movimento_Destinazione.Qta2 / xCalcolo_QD_SuperficieTotale
                    End If

                    Movimento_Destinazione.BaseCode = Agenda.BaseCode
                    Movimento_Destinazione.TopCode = Agenda.TopCode

                    Movimento_Destinazione.Qta = 0 'aggiorno dopo mentre leggo righe per fare i Movimento_Dettaglio_Tecnico

                    Select Case Lav_Cod
                        Case LAVCOD_FASI_FENOLOGICHE
                            Movimento_Destinazione.Data = CDate(valoreRilevato)
                        Case LAVCOD_RILIEVO_AVVERSITA_CAMPO, LAVCOD_RILIEVO_INDICI_MATURITA, LAVCOD_RILIEVO_ERBE_INFESTANTI, LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA, LAVCOD_DANNI_RACCOLTA
                            Movimento_Destinazione.Qta = CDec(valoreRilevato)
                            Movimento_Destinazione.Data = Agenda.Data
                        Case Else
                            Throw New NotImplementedException
                    End Select

                    '' VAnni: 22/11/2017: prima versione, dato uguale per tutte le destinazioni ...
                    Dim wkt As String = Me.Master.Property_txtPosizione
                    If Not String.IsNullOrEmpty(wkt) Then
                        Dim vWkt As String() = wkt.Split(",")
                        wkt = "POINT (" & vWkt(1).Trim() & ", " & vWkt(0).Trim() & ")"
                        Movimento_Destinazione.GisWkt = wkt
                        Movimento_Destinazione.GisWktGps = "1"
                        Movimento_Destinazione.GisWktSistemaRiferimento = "-1"
                        Movimento_Destinazione.GisLayerCod = enum_Gis_LayerElementiGrafici_std.AGENDA_RILIEVI
                        Movimento_Destinazione.GisTipoEntita_cod = enum_GIS2012_TipoEntita.Destinazione_Agenda
                    End If

                    'aggiungo il movimento destinazione al movimento detaglio
                    Movimento_Dettaglio.Movimenti_Destinazioni.Add(Movimento_Destinazione)

                End If


                'End If

            Next

            'se il movimento dettaglio corrente contiene almeno una destinazione allora
            'vuol dire che ho almeno un valore rilevato sull'appezzamento, quindi aggiungo
            'il movimento dettaglio con iul suo mov dettaglio tecnico e l'insieme delle destinazioni al movimento
            If Movimento_Dettaglio.Movimenti_Destinazioni.Count > 0 Then

                'aggiungo il movimento dettaglio al movimento
                Movimento.Movimenti_Dettagli.Add(Movimento_Dettaglio)
            End If

        Next

        'se ho almeno un movimento dettaglio nell'operazione la aggiungo all'agenda
        If Movimento.Movimenti_Dettagli.Count > 0 Then
            'aggiungo il movimento rilievo in campo-installazione trappola all'operazione agenda
            'Agenda.Movimenti.Add(Movimento)
        Else
            'messaggio_errore = messaggio_errore & Resources.AgronicaAgenda_2010.NessunDatoSalvato
            Return Nothing
        End If

        Return Agenda

    End Function

    Private Function DatoCompatibile(ByVal valoreRilevato As String, ByVal udmdes As String, ByRef causa As String, ByVal Lav_Cod As Integer) As Boolean

        If valoreRilevato.Contains(".") Then
            causa = Resources.AgronicaAgenda_2010.UtilizzareLaVirgolaENonIlPuntoPerIDecimali
            Return False
        End If

        Select Case Lav_Cod
            Case LAVCOD_RILIEVO_ERBE_INFESTANTI
                If Not IsNumeric(valoreRilevato) Then
                    causa = Resources.AgronicaAgenda_2010.NonÈUnNumero
                    Return False
                End If
                If CInt(valoreRilevato) < 0 Then
                    causa = Resources.AgronicaAgenda_2010.NonÈUnNumeroPositivo
                    Return False
                End If
                'If CDbl(valoreRilevato) = 0 Then
                '    causa = "Non può essere zero"
                '    Return False
                'End If
                'If CDbl(valoreRilevato) < 0.01 Then
                '    causa = "Non può essere minore di 0.01"
                '    Return False
                'End If
                'If CDbl(valoreRilevato) > 10000 Then
                '    causa = "Non può essere maggiore di 10000"
                '    Return False
                'End If

            Case LAVCOD_FASI_FENOLOGICHE

                If Not IsDate(valoreRilevato) Then
                    causa = Resources.AgronicaAgenda_2010.NonÈUnaData
                    Return False
                End If


            Case LAVCOD_RILIEVO_AVVERSITA_CAMPO

                If udmdes.Contains("%") Then
                    'percentuale
                    'numero
                    If Not IsNumeric(valoreRilevato) Then
                        causa = Resources.AgronicaAgenda_2010.NonÈUnNumero
                        Return False
                    End If
                    If CInt(valoreRilevato) < 0 Then
                        causa = Resources.AgronicaAgenda_2010.NonÈUnNumeroPositivo
                        Return False
                    End If
                    'If CDbl(valoreRilevato) = 0 Then
                    '    causa = "Non può essere zero"
                    '    Return False
                    'End If
                    'If CDbl(valoreRilevato) < 0.01 Then
                    '    causa = "Non può essere minore di 0.01"
                    '    Return False
                    'End If
                    If CDbl(valoreRilevato) > 100 Then
                        causa = AgronicaAgenda_2010.NonPuòEssereMaggioreDi100
                        Return False
                    End If

                ElseIf udmdes.Contains("(s/n)") OrElse udmdes.Contains("(S/N)") Then
                    'si/no, TODO: disambiguare per il multilingua
                    If Not IsNumeric(valoreRilevato) Then
                        causa = AgronicaAgenda_2010.DeveEssere1O0NelCasoDiPrezenzaONonPresenza
                        Return False
                    End If
                    If CInt(valoreRilevato) <> 1 AndAlso CInt(valoreRilevato) <> 0 Then
                        causa = AgronicaAgenda_2010.DeveEssere1NelCasoDiRilevamentoDellAvversi
                        Return False
                    End If
                Else
                    'numero
                    If Not IsNumeric(valoreRilevato) Then
                        causa = Resources.AgronicaAgenda_2010.NonÈUnNumero
                        Return False
                    End If
                    If valoreRilevato.Contains(",") Then
                        causa = Resources.AgronicaAgenda_2010.NonÈUnNumeroIntero
                        Return False
                    End If
                    If valoreRilevato.Contains(".") Then
                        causa = Resources.AgronicaAgenda_2010.NonÈUnNumeroIntero
                        Return False
                    End If
                    If Not System.Int32.TryParse(valoreRilevato, Nothing) Then
                        causa = Resources.AgronicaAgenda_2010.NonÈUnNumeroInteroValido
                        Return False
                    End If
                    If CInt(valoreRilevato) < 0 Then
                        causa = Resources.AgronicaAgenda_2010.NonÈUnNumeroPositivo
                        Return False
                    End If
                    'If CDbl(valoreRilevato) = 0 Then
                    '    causa = "Non può essere zero"
                    '    Return False
                    'End If
                End If


            Case LAVCOD_RILIEVO_INDICI_MATURITA, LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA, LAVCOD_DANNI_RACCOLTA

                'numero
                If Not IsNumeric(valoreRilevato) Then
                    causa = Resources.AgronicaAgenda_2010.NonÈUnNumero
                    Return False
                End If
                If CInt(valoreRilevato) < 0 Then
                    causa = Resources.AgronicaAgenda_2010.NonÈUnNumeroPositivo
                    Return False
                End If
                'If CInt(valoreRilevato) = 0 Then
                '    causa = "Non può essere zero"
                '    Return False
                'End If
                'If CDbl(valoreRilevato) < 0.01 Then
                '    causa = "Non può essere minore di 0.01"
                '    Return False
                'End If

            Case Else
                Throw New NotImplementedException
        End Select

        Return True

    End Function

    Private Function CreaOggettoAgendaVisita(ByRef listaOpCollegate As List(Of Operazione_Agenda)) As Operazione_Agenda

        '---------------------------------------
        ' recupero la DATA
        If objParametriAgenda.Data = AGRODATAINIZIO Then
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.IndicareUnaData, Page, ,
                                CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            Return Nothing
        End If

        Dim Data As Date = New Date(objParametriAgenda.Data.Year, objParametriAgenda.Data.Month, objParametriAgenda.Data.Day)
        Dim Ora As Date = objParametriAgenda.Data.AddSeconds(-objParametriAgenda.Data.Second).AddMilliseconds(-objParametriAgenda.Data.Millisecond)

        '---------------------------------------
        ' recupero la SPECIE
        Dim Veg_Cod As String = ""
        Dim Veg_Des As String = ""
        If objParametriAgenda.Veg_Cod.Split("/")(0) <> "-1" Then
            Veg_Cod = objParametriAgenda.Veg_Cod.Split("/")(0)
            Veg_Des = Master.Property_ComboSpecie.Testo_Combo
        End If

        '---------------------------------------
        ' recupero la OPERAZIONE
        Dim Lav_Cod As String = ""
        Dim Lav_Des As String = ""
        If objParametriAgenda.Lav_Cod = "" Then
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.SelezionareUnOperazione, Page, ,
                                CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            Return Nothing
        Else
            Lav_Cod = objParametriAgenda.Lav_Cod
            Lav_Des = Master.Property_ComboOperazione.Testo_Combo
        End If


        '---------------------------------------

        Dim BaseCode As Integer = 0
        Dim TopCode As Integer = 0

        '------------------------------------------------
        '----- Calcolo i valori di BaseCode e TopCode
        '------------------------------------------------
        UtilityProvider.Calcola_BaseCode_TopCode(BaseCode, TopCode, Session("ASG_ProgressivoGIAS"))

        '------------------------------------------------
        '----- AGENDA
        '------------------------------------------------
        Dim Agenda As Operazione_Agenda
        Dim Movimento As Movimento
        Dim Movimento_Dettaglio As Movimento_Dettaglio
        Dim Movimento_Dettaglio_Rif As Movimento_Dettaglio_Riferimento
        'Dim Movimento_Destinazione As Movimento_Destinazione

        'Dim righeModificateArray As JArray = LeggiGrigliaRilievi(Sa_Cod)

        'Dim ListaVarieta As List(Of String) = (From x In righeModificateArray Select CStr(x("Cul_Des"))).Distinct().ToList()
        'Dim StrVarieta As String = String.Join(", ", ListaVarieta)
        Dim StrVarieta As String = ""

        Agenda = New Operazione_Agenda

        Agenda.Tipo_Operazione = objParametriAgenda.Tipo_Operazione
        Agenda.Id_Agenda = objParametriAgenda.Id_Agenda
        Agenda.Data = Data
        Agenda.Piva = objParametriAgenda.Piva
        Agenda.Sa_Cod = 0 'Sa_Cod
        Agenda.Lav_Cod = Lav_Cod
        Agenda.Des_Lib = Lav_Des & " (" & Veg_Des & "  [" & StrVarieta & "])"

        Agenda.BaseCode = BaseCode
        Agenda.TopCode = TopCode


        '------------------------------------------------
        '----- NOTE
        '------------------------------------------------
        Dim ListaConsigli As List(Of Nota) = CType(Master, OperazioneBootstrap).GetConsigli()
        If ListaConsigli.Count > 0 Then
            Agenda.Note = New List(Of Nota)
            For i = 0 To ListaConsigli.Count - 1
                Agenda.Note.Add(New Nota With {.Id_Agenda = Agenda.Id_Agenda, .Nota_Cod = ListaConsigli(i).Nota_Cod})
            Next
        End If

        '------------------------------------------------
        '----- GPS
        '------------------------------------------------
        Dim wkt As String = Me.Master.Property_txtPosizione
        If Not String.IsNullOrEmpty(wkt) Then
            Dim vWkt As String() = wkt.Split(",")
            wkt = "POINT (" & vWkt(1).Trim() & ", " & vWkt(0).Trim() & ")"
            Agenda.GisWkt = wkt
            Agenda.GisWktGps = "1"
            Agenda.GisWktSistemaRiferimento = "-1"
            Agenda.GisLayerCod = enum_Gis_LayerElementiGrafici_std.Op_Agenda
            Agenda.GisTipoEntita_cod = enum_GIS2012_TipoEntita.OpAgenda
        End If


        '------------------------------------------------
        '----- MOVIMENTI
        '------------------------------------------------
        Agenda.Movimenti = New List(Of Movimento)


        'COSTI ACCESSORI
        For i = 0 To objParametriAgenda.Movimenti.Count - 1
            objParametriAgenda.Movimenti(i).Id_Agenda = Agenda.Id_Agenda
            objParametriAgenda.Movimenti(i).Sa_Cod = Agenda.Sa_Cod

            If Not IsNothing(objParametriAgenda.Movimenti(i).Movimenti_Dettagli) Then
                For j As Integer = 0 To objParametriAgenda.Movimenti(i).Movimenti_Dettagli.Count - 1
                    objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Id_Agenda = Agenda.Id_Agenda
                    If objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Sa_Cod = 0 Then
                        objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Sa_Cod = Agenda.Sa_Cod
                    End If
                Next
            End If

            If Not IsNothing(objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici) Then
                For j As Integer = 0 To objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici.Count - 1
                    objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Id_Agenda = Agenda.Id_Agenda
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

        Movimento.Id_Agenda = Agenda.Id_Agenda
        Movimento.Piva = Agenda.Piva
        Movimento.Sa_Cod = Agenda.Sa_Cod
        Movimento.Data = Data
        Movimento.Ora = Ora
        Movimento.Lav_Cod = Agenda.Lav_Cod
        Movimento.Cau_Mov = CAU_VISITE_ISPETTIVE 'objParametriAgenda.Cau_Mov
        Movimento.Mov_Desc = txtNote.Text 'CType(Master, OperazioneBootstrap).GetNota()
        Movimento.BaseCode = BaseCode
        Movimento.TopCode = TopCode

        Movimento.Num_Protocollo = 0

        Select Case objParametriAgenda.Disciplinare
            Case NessunDpiNessunaEtichetta
                Movimento.Num_Protocollo = 0
            Case "0"
                Movimento.Num_Protocollo = -1
            Case "-2"   'BIO
                Movimento.Num_Protocollo = -2
            Case Else   'DPI
                Dim Array() As String = Split(objParametriAgenda.Disciplinare, "/")
                Movimento.Num_Protocollo = Array(0)
                If Array.Length > 1 AndAlso Array(1) IsNot Nothing Then
                    Movimento.Doc_Numero = Array(1)
                End If
                If Array.Length > 4 AndAlso Array(4) IsNot Nothing Then
                    Movimento.Disciplinare_PubblicoPrivato = Array(4)
                End If
        End Select

        Agenda.Movimenti.Add(Movimento)


        '------------------------------------------------
        '----- MOVIMENTI DETTAGLI
        '------------------------------------------------

        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli = New List(Of Movimento_Dettaglio)

        Movimento_Dettaglio = New Movimento_Dettaglio
        Movimento_Dettaglio.Id_Agenda = Agenda.Id_Agenda
        Movimento_Dettaglio.Piva = Agenda.Piva
        Movimento_Dettaglio.Sa_Cod = Agenda.Sa_Cod
        Movimento_Dettaglio.Contabilizzato = 1 '1 se data<corrente -1 se data futura
        Movimento_Dettaglio.Pendente = 3 'valore copiato da agenda vecchia
        Movimento_Dettaglio.Extra_Date = AGRODATAINIZIO 'valore copiato da agenda vecchia
        Movimento_Dettaglio.Anno = 1900 'valore copiato da agenda vecchia
        Movimento_Dettaglio.Data = Agenda.Data
        Movimento_Dettaglio.Lav_Cod = Agenda.Lav_Cod
        Movimento_Dettaglio.BaseCode = Agenda.BaseCode
        Movimento_Dettaglio.TopCode = Agenda.TopCode

        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Add(Movimento_Dettaglio)

        '------------------------------------------------
        '----- MOVIMENTI DETTAGLI RIFERIMENTI
        '------------------------------------------------

        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(0).Movimenti_Dettagli_Riferimenti = New List(Of Movimento_Dettaglio_Riferimento)

        For Each op As Operazione_Agenda In listaOpCollegate

            Movimento_Dettaglio_Rif = New Movimento_Dettaglio_Riferimento
            Movimento_Dettaglio_Rif.Id_Agenda = Agenda.Id_Agenda
            Movimento_Dettaglio_Rif.Piva = Agenda.Piva
            Movimento_Dettaglio_Rif.Sa_Cod = Agenda.Sa_Cod
            Movimento_Dettaglio_Rif.Id_Mov = -1
            Movimento_Dettaglio_Rif.Id_Mov_Det = -1
            Movimento_Dettaglio_Rif.Lav_Cod = Agenda.Lav_Cod
            Movimento_Dettaglio_Rif.Cau_Mov = ""
            Movimento_Dettaglio_Rif.Id_Agenda_Rif = op.Id_Agenda
            Movimento_Dettaglio_Rif.Piva_Rif = op.Piva
            Movimento_Dettaglio_Rif.Sa_Cod_Rif = op.Sa_Cod
            Movimento_Dettaglio_Rif.Id_Mov_Rif = -1
            Movimento_Dettaglio_Rif.Id_Mov_Det_Rif = -1
            Movimento_Dettaglio_Rif.Lav_Cod_Rif = op.Lav_Cod
            Movimento_Dettaglio_Rif.Cau_Mov_Rif = ""
            Movimento_Dettaglio_Rif.Qta = 0
            Movimento_Dettaglio_Rif.BaseCode = Agenda.BaseCode
            Movimento_Dettaglio_Rif.TopCode = Agenda.TopCode

            Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(0).Movimenti_Dettagli_Riferimenti.Add(Movimento_Dettaglio_Rif)
        Next


        ''------------------------------------------------
        ''----- MOVIMENTI DESTINAZIONI
        ''------------------------------------------------

        'Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(0).Movimenti_Destinazioni = New List(Of Movimento_Destinazione)

        'Dim xCalcolo_QD_SuperficieTotale As Decimal = (
        '        From ST In ListaImpiantixQuestoSaCod
        '        Select CType(ST.Qta2, Decimal)
        '    ).Sum

        'For j = 0 To ListaImpiantixQuestoSaCod.Count - 1

        '    Movimento_Destinazione = New Movimento_Destinazione

        '    Movimento_Destinazione.Id_Agenda = Agenda.Id_Agenda
        '    Movimento_Destinazione.Data = Data

        '    Movimento_Destinazione.Piva = ListaImpiantixQuestoSaCod(j).Piva
        '    Movimento_Destinazione.Sa_Cod = ListaImpiantixQuestoSaCod(j).Sa_Cod
        '    Movimento_Destinazione.Appezza = ListaImpiantixQuestoSaCod(j).Appezza
        '    Movimento_Destinazione.Id_Destinazione = ListaImpiantixQuestoSaCod(j).ID_Reg
        '    Movimento_Destinazione.Tipo = 0

        '    '' VAnni: 22/11/2017: prima versione, dato uguale per tutte le destinazioni ...
        '    Dim wkt As String = Me.Master.Property_txtPosizione
        '    If Not String.IsNullOrEmpty(wkt) Then
        '        Dim vWkt As String() = wkt.Split(",")
        '        wkt = "POINT (" & vWkt(1).Trim() & ", " & vWkt(0).Trim() & ")"
        '        Movimento_Destinazione.GisWkt = wkt
        '        Movimento_Destinazione.GisWktGps = "1"
        '        Movimento_Destinazione.GisWktSistemaRiferimento = "-1"
        '    End If


        '    'If CType(Me.DataGridImpianti.Items(i).FindControl("ChkRispetto"), CheckBox).Checked = True Then
        '    '    Movimento_Destinazione.Qta = 1
        '    'Else
        '    '    Movimento_Destinazione.Qta = -1
        '    'End If

        '    'Movimento_Destinazione.Qta = DoseHA_Trasformata * ListaImpiantixQuestoSaCod(j).Qta2

        '    Movimento_Destinazione.Qta2 = ListaImpiantixQuestoSaCod(j).Qta2 '''''''''''''''''0
        '    'QTA2_TOT += ListaImpianti(j).Qta2

        '    Movimento_Destinazione.BaseCode = BaseCode
        '    Movimento_Destinazione.TopCode = TopCode

        '    '  Vanni, 20/09/2016 09:15:32: x salvataggio planning
        '    Movimento_Destinazione.Programmazione_Entita_Cod = ListaImpiantixQuestoSaCod(j).Programmazione_Entita_cod
        '    If xCalcolo_QD_SuperficieTotale <> 0 Then
        '        Movimento_Destinazione.QuotaDistribuzione = ListaImpiantixQuestoSaCod(j).Qta2 / xCalcolo_QD_SuperficieTotale
        '    End If

        '    Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(0).Movimenti_Destinazioni.Add(Movimento_Destinazione)

        'Next


        'se ho almeno un movimento dettaglio nell'operazione la aggiungo all'agenda
        If Movimento.Movimenti_Dettagli.Count > 0 Then
            'aggiungo il movimento rilievo in campo-installazione trappola all'operazione agenda
            'Agenda.Movimenti.Add(Movimento)
        Else
            'messaggio_errore = messaggio_errore & Resources.AgronicaAgenda_2010.NessunDatoSalvato
            Return Nothing
        End If

        Return Agenda

    End Function

#End Region


#Region "Caricamento dei dati"

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub

    Private Sub RilieviBS_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        Master_Operazione = CType(Page.Master, OperazioneBootstrap)

        'Master_Operazione.AttivaOrarioInDataMovimento = True
        'Master_Operazione.Property_divUsernameCreazione.Visible = True
        'Master_Operazione.Property_divPosizione.Visible = True
        Master_Operazione.Property_divOperazione.Visible = False

        Master_Operazione.GisAttivo = True

        Dim lat As String = Request.QueryString("lat")
        Dim lng As String = Request.QueryString("lng")

        If lat <> "" Then
            Master_Operazione.Property_txtPosizione = lat & ", " & lng
        End If

        AddHandler CType(Me.Master.Master, DomandaIrriguaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
        AddHandler CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_Salva"), ImageButton).Click, AddressOf Me.SalvaTutto

        AddHandler CType(Ricerca.FindControlIterative(Page.Master, "BTN_ComboCentroAziendale"), Button).Click, AddressOf Me.Aggiorna_Centro_Specie_Disciplinare
        AddHandler CType(Ricerca.FindControlIterative(Page.Master, "BTN_ComboSpecie"), Button).Click, AddressOf Me.Aggiorna_Centro_Specie_Disciplinare
        AddHandler CType(Ricerca.FindControlIterative(Page.Master, "BTN_ComboDisciplinari1"), Button).Click, AddressOf Me.Aggiorna_Centro_Specie_Disciplinare

        Ricerca.FindControlIterative(Page.Master, "Div_BtnFasi").Visible = False

    End Sub

    Private Sub Aggiorna_Centro_Specie_Disciplinare(sender As Object, e As EventArgs)

        Dim strJS1 As New StringBuilder
        strJS1.AppendLine("$(document).ready(function () { ")

        If objParametriAgenda.Lav_Cod = LAVCOD_RILIEVO_AVVERSITA_CAMPO OrElse objParametriAgenda.Lav_Cod = LAVCOD_VISITA Then
            strJS1.AppendLine("      $('#comboRilievoAvversitaInCampo').data('kendoDropDownList').dataSource.read(); ")
        End If

        If objParametriAgenda.Lav_Cod = LAVCOD_RILIEVO_INDICI_MATURITA OrElse objParametriAgenda.Lav_Cod = LAVCOD_VISITA Then
            strJS1.AppendLine("      $('#comboRilievoIndiciMaturita').data('kendoDropDownList').dataSource.read(); ")
        End If

        If objParametriAgenda.Lav_Cod = LAVCOD_FASI_FENOLOGICHE OrElse objParametriAgenda.Lav_Cod = LAVCOD_VISITA Then
            strJS1.AppendLine("      $('#comboRilievoFasiFenologiche').data('kendoDropDownList').dataSource.read(); ")
        End If

        If objParametriAgenda.Lav_Cod = LAVCOD_RILIEVO_ERBE_INFESTANTI OrElse objParametriAgenda.Lav_Cod = LAVCOD_VISITA Then
            strJS1.AppendLine("      $('#comboRilievoErbeInfestanti').data('kendoDropDownList').dataSource.read(); ")
        End If

        If objParametriAgenda.Lav_Cod = LAVCOD_DANNI_RACCOLTA OrElse objParametriAgenda.Lav_Cod = LAVCOD_VISITA Then
            strJS1.AppendLine("      $('#comboRilievoDanniRaccolta').data('kendoDropDownList').dataSource.read(); ")
        End If

        If objParametriAgenda.Lav_Cod = LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA OrElse objParametriAgenda.Lav_Cod = LAVCOD_VISITA Then
            strJS1.AppendLine("      $('#comboRilievoIndiciReseRaccolta').data('kendoDropDownList').dataSource.read(); ")
        End If

        If CType(sender, System.Web.UI.Control).ID = Master.Property_BTN_ComboSpecie.ID OrElse CType(sender, System.Web.UI.Control).ID = Master.Property_BTN_ComboDisciplinari.ID Then
            strJS1.AppendLine("      leggiPreset(); ")
            'strJS1.AppendLine("      AggiornaDopo_SupTrattata(); ")
        End If

        strJS1.AppendLine(" }); ")

        Dim up As UpdatePanel = CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel)
        ScriptManager.RegisterStartupScript(up, up.GetType(), String.Format("jQuery_{0}", up.ClientID), strJS1.ToString, True)

    End Sub

    Private Sub Rilievi_Disposed()
        'Master
        CType(Page.Master, OperazioneBootstrap).Operazioni_Dispose()

        'Page
        Session.Remove("UtenteAbilitato_Lettura")
        Session.Remove("UtenteAbilitato_Modifica")

        Session.Remove("comportamentoComboFF")
        Session.Remove("Tabella")

    End Sub


    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010 AndAlso (
            objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Trattamenti OrElse
            objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Gis OrElse
            objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Filtrone) Then

            Dim strJS As New StringBuilder
            strJS.AppendLine("$(document).ready(Function () { ")
            strJS.AppendLine("      window.parent.RilievoRientro(); ")
            strJS.AppendLine(" });")

            ScriptManager.RegisterStartupScript(CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel), CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).GetType(),
                                              String.Format("jQuery_{0}", CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).ClientID), strJS.ToString, True)

            Rilievi_Disposed()
            objParametriAgenda.Svuota_DatiOperazione()
            objParametriAgenda.RecuperaRientro()

            Exit Sub
        End If

        Rilievi_Disposed()
        objParametriAgenda.Svuota_DatiOperazione()

        Dim link As String = ""
        Try
            Dim sitoorigine As Enum_SiteRedirector = objParametriAgenda.SitoOrigine
            Dim paginaOnLineRitorno As Integer = objParametriAgenda.PaginaSitoOrigine

            If sitoorigine = Enum_SiteRedirector.Sito_GiasOnline_2010 AndAlso paginaOnLineRitorno = enum_PagineGiasOnline_2010.RegistazioneSmart Then
                link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri(
                                       Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                       enum_PagineGiasOnline_2010.RegistazioneSmart,
                                       enum_PagineAgenda_2010.Menu, objParametriAgenda.Piva, "", "", 0, "")

            ElseIf sitoorigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010 AndAlso paginaOnLineRitorno = enum_PagineAgenda_2010.Pagina_Visite_Lista Then
                link = "../Visite/Visite_Lista.aspx"
            ElseIf sitoorigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010 AndAlso paginaOnLineRitorno = enum_PagineAgenda_2010.Pagina_Visite_ListaDettagli Then
                link = "../Visite/Visite_ListaDettagli.aspx"
            ElseIf sitoorigine = Enum_SiteRedirector.GiasNG Then
                AgronicaCoreGestioneRichieste.MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva,
                                                                                      Enum_SiteRedirector.GiasNG,
                                                                                      objParametriAgenda.PaginaSitoOrigine,
                                                                                      link,
                                                                                      objParametri_Server)
            Else
                link = CType(Master.Master, DomandaIrriguaBootstrap).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
            End If

        Catch ex As Exception
            link = CType(Master.Master, DomandaIrriguaBootstrap).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
        End Try

        Response.Redirect(link)


    End Sub


    Private Sub inizializzoParametriAgenda()
        objParametriAgenda = New ParametriAgenda

        'objParametriAgenda.Leggi()
        Id_Agenda_Old = objParametriAgenda.Id_Agenda
        objParametriAgenda.OperazioneMulticentro = True
    End Sub

    Public Sub Ripristina_Dati_nei_Controlli() Implements iOperazioneGUI.Ripristina_Dati_nei_Controlli

        Select Case objParametriAgenda.Tipo_Operazione

            Case enum_TipoOperazioneDB.Scrittura

                If objParametriAgenda.Rilievi.Count > 0 AndAlso objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Trattamenti Then
                    RipristinaControlliDaTrattamento()
                End If

                If objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Gis Then
                    RipristinaControlliDaGIS()
                End If

                'Imposto l'utente
                Dim ud_R As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
                Master.Property_txtUsernameCreazione.Text = ud_R.NomeCognome_From_CodFisc(objParametri_Server.UsernameOperazione, objParametri_Utenti)

                If objParametriAgenda.Lav_Cod = LAVCOD_VISITA Then
                    objParametriAgenda.Data = Now

                    'Imposto l'impresa
                    Master.Property_txtImpresa.Text = objParametriAgenda.RagSoc
                End If

            Case enum_TipoOperazioneDB.Modifica
                RipristinaControlliDaAgenda()
                Master_Operazione.CaricaCostiAccessori()

            Case enum_TipoOperazioneDB.Lettura
                RipristinaControlliDaAgenda()
                Master_Operazione.CaricaCostiAccessori()

        End Select

    End Sub

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function caricaListaPreset() As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        Dim Personalizzati_Indici_Maturita As String = HttpContext.Current.Session("Personalizzati_Indici_Maturita")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            r.RispostaOK = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim objPA As New ParametriAgenda
            If objPA.Lav_Cod = "0" Then
                r.Errore = DirectCast(HttpContext.GetLocalResourceObject("~/Operazioni/App_LocalResources/RilieviBS.aspx.resx", "ErroreOperazioneNonImpostata"), String)
                r.RispostaOK = False
                Return r
            End If

            If objPA.Veg_Cod = "0" OrElse objPA.Veg_Cod = "-1" Then
                r.RispostaStringa = "{}"
                r.RispostaOK = True
                Return r
            End If

            Dim p_R As New AgronicaCoreProfilazioneBIZ.Profilazione_R
            Dim dtProfilazione As DataTable = p_R.LeggiProfilazioneXRilievi_In_Cascata(objPA.Piva, objPA.Lav_Cod, objPA.Lav_Cod, objPA.Veg_Cod.Split("/")(0),
                                                                                       Not String.IsNullOrEmpty(Personalizzati_Indici_Maturita) AndAlso Personalizzati_Indici_Maturita = True,
                                                                                       objParametri_Server, objParametri_Super_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dtProfilazione, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Private Sub RipristinaControlliDaTrattamento()
        GeneraTabella(False, Nothing)

        Dim strJS1 As New StringBuilder
        strJS1.AppendLine("$(document).ready(function () { ")
        strJS1.AppendLine("      $('#" & Master.Property_ComboDisciplinari.ddl_Disciplinari.ClientID & "').val('" & objParametriAgenda.Disciplinare & "') ")
        strJS1.AppendLine("      $('#" & Master.Property_ComboDisciplinari.ddl_Disciplinari.ClientID & "').selectpicker('refresh'); ")
        strJS1.AppendLine("      letturaTabella_Kendo(); ")
        strJS1.AppendLine(" });")

        ScriptManager.RegisterStartupScript(CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel), CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).GetType(),
                                      String.Format("jQuery_{0}", CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).ClientID), strJS1.ToString, True)

    End Sub


    Private Sub RipristinaControlliDaGIS()
        GeneraTabella(False, Nothing)

        Dim strJS1 As New StringBuilder
        strJS1.AppendLine("$(document).ready(function () { ")
        strJS1.AppendLine("      $('.ComboSpecie').selectpicker('val', '" & objParametriAgenda.Veg_Cod & "'); ")
        strJS1.AppendLine("      $('.ComboSpecie').selectpicker('refresh'); ")
        strJS1.AppendLine("      console.log('RipristinaControlliDaGIS'); ")
        strJS1.AppendLine("      letturaTabella_Kendo(); ")

        'strJS1.AppendLine("      leggiPreset(); ")
        'strJS1.AppendLine("      AggiornaDopo_SupTrattata(); ")

        strJS1.AppendLine(" });")

        ScriptManager.RegisterStartupScript(CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel), CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).GetType(),
                                      String.Format("jQuery_{0}", CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).ClientID), strJS1.ToString, True)

    End Sub

    Private Sub RipristinaControlliDaAgenda()


        Dim objAgenda As New Agenda_Operazione_Helper
        Dim Agenda As Operazione_Agenda = objAgenda.Leggi(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, objParametriAgenda.Id_Agenda, 0, objParametri_Server)

        If Not IsNothing(Agenda) Then

            objParametriAgenda.Piva = Agenda.Piva
            objParametriAgenda.Sa_Cod = Agenda.Sa_Cod
            objParametriAgenda.Lav_Cod = Agenda.Lav_Cod
            objParametriAgenda.Des_lib = Agenda.Des_Lib

            objParametriAgenda.Raccoglitore_Cod = Agenda.Raccoglitore_Cod

            'NOTE
            If Not IsNothing(Agenda.Note) Then
                For i = 0 To Agenda.Note.Count - 1
                    objParametriAgenda.Note.Add(Agenda.Note(i))
                Next
            End If
            objParametriAgenda.salva()

            'Imposto la posizione GPS
            Master.Property_txtPosizione = Agenda.GisWkt

            If objParametriAgenda.Lav_Cod = LAVCOD_VISITA Then
                'Utente Username
                Master.Property_txtUsernameCreazione.Text = New AgronicaCoreUtentiDAL.Utenti_Dettagli_R().NomeCognome_From_CodFisc(Agenda.Username_Creazione, objParametri_Utenti)
                'Imposto l'impresa
                Master.Property_txtImpresa.Text = objParametriAgenda.RagSoc
            End If

            'MOVIMENTI
            If Not IsNothing(Agenda.Movimenti) Then

                Dim MovimentiCosti As List(Of Movimento) = New List(Of Movimento)
                objParametriAgenda.Impianti = New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)

                'Grilli 31/01/2018 per fix in seguito ad addrange
                objParametriAgenda.Rilievi = New List(Of rilievoAvv)

                For i = 0 To Agenda.Movimenti.Count - 1

                    'controllo corrispondeza piva con agenda
                    If Agenda.Piva <> Agenda.Movimenti(i).Piva Then
                        Throw New ApplicationException
                    End If

                    Select Case Agenda.Movimenti(i).Cau_Mov

                        Case enum_Agenda_Causali.RILIEVO_CAMPO, enum_Agenda_Causali.RILIEVO_RACCOLTA

                            If Agenda.Lav_Cod <> objParametriAgenda.Lav_Cod Then
                                'controllino per lo sviluppo, da togliere
                                Throw New NotImplementedException
                            End If

                            Select Case CInt(objParametriAgenda.Lav_Cod)
                                Case LAVCOD_FASI_FENOLOGICHE, LAVCOD_RILIEVO_AVVERSITA_CAMPO, LAVCOD_RILIEVO_INDICI_MATURITA, LAVCOD_RILIEVO_ERBE_INFESTANTI, LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA, LAVCOD_DANNI_RACCOLTA
                                    LeggiMovimentoAgenda_Rilievi(Agenda, i)
                                Case Else
                                    Throw New NotImplementedException
                            End Select

                        Case CAU_VISITE_ISPETTIVE
                            Select Case CInt(objParametriAgenda.Lav_Cod)
                                Case LAVCOD_VISITA
                                    LeggiMovimentoAgenda_Visite(Agenda, i)

                                    Dim listaCatSelez As New List(Of String)

                                    For Each rif As Movimento_Dettaglio_Riferimento In Agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Dettagli_Riferimenti
                                        Dim Agenda_Collegata As Operazione_Agenda = objAgenda.Leggi(rif.Piva_Rif, rif.Sa_Cod_Rif, rif.Id_Agenda_Rif, 0, objParametri_Server)

                                        Dim codice = If(Agenda_Collegata.Lav_Cod = LAVCOD_ALTRE_OPERAZIONI, """" & LAVCOD_ALTRE_OPERAZIONI & "|" & Agenda_Collegata.Id_Attivita & """", Agenda_Collegata.Lav_Cod)

                                        If Not listaCatSelez.Contains(codice) Then
                                            listaCatSelez.Add(codice)
                                        End If

                                        For ii As Integer = 0 To Agenda_Collegata.Movimenti.Count - 1
                                            Select Case Agenda_Collegata.Lav_Cod
                                                Case LAVCOD_FASI_FENOLOGICHE, LAVCOD_RILIEVO_AVVERSITA_CAMPO, LAVCOD_RILIEVO_INDICI_MATURITA, LAVCOD_RILIEVO_ERBE_INFESTANTI, LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA, LAVCOD_DANNI_RACCOLTA
                                                    LeggiMovimentoAgenda_Rilievi(Agenda_Collegata, ii)
                                                Case LAVCOD_ALTRE_OPERAZIONI
                                                    LeggiMovimentoAgenda_AltreLavorazioni(Agenda_Collegata, ii)
                                                Case Else
                                                    Throw New NotImplementedException
                                            End Select
                                        Next

                                    Next

                                    hdSelezioneCategorieVisite.Value = "[" & String.Join(",", listaCatSelez) & "]"

                                Case Else
                                    Throw New NotImplementedException
                            End Select

                                '----COSTO ACCESSORIO---------------------
                        Case enum_Agenda_Causali.SCARICO
                            'se c'è è costo accessorio
                            MovimentiCosti.Add(Agenda.Movimenti(i))

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


                    'COMBO DISCIPLINARI
                    impostaComboDisciplinari(Agenda.Movimenti(i).Num_Protocollo, Agenda.Movimenti(i).Disciplinare_PubblicoPrivato, Agenda.Movimenti(i).Doc_Numero)
                Next

                objParametriAgenda.Movimenti = MovimentiCosti

            End If

            'operazione con raccoglitore ( = salvata con altri lav_cod)
            If Agenda.Raccoglitore_Cod <> 0 Then

                Dim ListaOperazioni As List(Of Operazione_Agenda)
                ListaOperazioni = objAgenda.LeggiLista_DaRaccoglitore(objParametriAgenda.Piva, Agenda.Raccoglitore_Cod, objParametri_Server)

                If ListaOperazioni IsNot Nothing Then

                    For Each Operazione In ListaOperazioni

                        'escludo l'operazione corrente (già analizzata sopra)
                        If Operazione.Id_Agenda <> objParametriAgenda.Id_Agenda Then

                            'se trovo un centro diverso imposto 'tutti i centri'
                            If Operazione.Sa_Cod <> objParametriAgenda.Sa_Cod Then
                                objParametriAgenda.Sa_Cod = "0"
                            End If

                            For i = 0 To Operazione.Movimenti.Count - 1

                                Select Case Operazione.Movimenti(i).Cau_Mov

                                    Case enum_Agenda_Causali.RILIEVO_CAMPO

                                        Select Case CInt(Operazione.Lav_Cod)
                                            Case LAVCOD_FASI_FENOLOGICHE ', LAVCOD_RILIEVO_AVVERSITA_CAMPO, LAVCOD_RILIEVO_INDICI_MATURITA, LAVCOD_RILIEVO_ERBE_INFESTANTI, LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA, LAVCOD_DANNI_RACCOLTA
                                                LeggiMovimentoAgenda_Rilievi(Operazione, i)
                                            Case Else
                                                Throw New NotImplementedException
                                        End Select

                                End Select
                            Next



                        End If

                    Next

                End If


            End If

            'CONTROLLO SE CI SONO DEI COSTI COLLEGATI
            For Each mdRif As Movimento_Dettaglio_Riferimento In Agenda.Agenda_Riferimenti
                If mdRif.Lav_Cod_Rif = LAVCOD_COSTI_CDG Then
                    'Messaggi.AgroMsgBuonFine("NB: Esistono costi collegati a questa operazione.", Page, , CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
                    Master.Property_hf_esistonoCostiCollegatiCDG = True
                    Exit For
                End If
            Next

        End If


        '................................
        'se tutto è andato bene ora ho i dati e devo settare le combo, text e ricostruire la tabella... 



        'data, impianti, centro già impostati dalla master

        ''Txt_SupSelezionata
        'Dim ImpUtil As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        'Dim supImp As Decimal = 0
        'For Each imp As AgronicaCoreModello.ParametriAgenda_Temp.Impianto In objParametriAgenda.Impianti
        '    supImp += ImpUtil.LeggiSuperficie(Agenda.Piva, Agenda.Sa_Cod, imp.Appezza, imp.ID_Reg, objParametri_Server)
        'Next
        ''Txt_SupSelezionata.Value = CStr(supImp)

        GeneraTabella(False, Agenda)

        Dim strJS1 As New StringBuilder
        strJS1.AppendLine("$(document).ready(function () { ")

        If objParametriAgenda.Lav_Cod = LAVCOD_VISITA Then
            strJS1.AppendLine("      selezionaCategorieVisite(); ")
        End If

        strJS1.AppendLine("      letturaTabella_Kendo(); ")
        strJS1.AppendLine(" });")

        ScriptManager.RegisterStartupScript(CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel), CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).GetType(),
                                      String.Format("jQuery_{0}", CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).ClientID), strJS1.ToString, True)


    End Sub

    Private Sub LeggiMovimentoAgenda_AltreLavorazioni(ByRef agenda As Operazione_Agenda, ByRef i As Integer)

        '---------------------
        '----MOVIMENTI_DETTAGLI
        '---------------------
        'uno per ciascun impianto influenzato dalle trappole installate
        If Not IsNothing(agenda.Movimenti(i).Movimenti_Dettagli) Then

            For j = 0 To agenda.Movimenti(i).Movimenti_Dettagli.Count - 1

                'controllo corrispondeza piva con agenda
                'sa cod è uguale ma potrebbe cambiare se...
                If agenda.Piva <> agenda.Movimenti(i).Movimenti_Dettagli(j).Piva Then
                    Throw New ApplicationException
                End If

                For r = 0 To agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count - 1

                    '---------------------
                    'MOVIMENTO_DESTINAZIONI
                    '---------------------
                    'ci deve essere almeno un movimento destinazione, uno per ciascun appezzamento 
                    If IsNothing(agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni) Then
                        Throw New ApplicationException
                    End If

                    Dim objAppezzamento As New AgronicaCoreModello.ParametriAgenda_Temp.Impianto
                    objAppezzamento.Piva = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Piva
                    objAppezzamento.Sa_Cod = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Sa_Cod
                    objAppezzamento.Appezza = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Appezza
                    objAppezzamento.ID_Reg = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Id_Destinazione
                    objAppezzamento.Qta2 = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Qta2

                    Dim objImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                    Dim Dt_Imp As DataTable = objImp.Leggi(agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Piva,
                                                 agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Sa_Cod,
                                                 agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Appezza,
                                                 agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Id_Destinazione,
                                                 enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                 "", "", objParametri_Server)

                    If Dt_Imp.Rows.Count > 0 Then
                        objAppezzamento.Veg_Cod = Dt_Imp.Rows(0).Item("veg_cod")
                        If Dt_Imp.Rows(0).Item("veg_cod") = 0 AndAlso Dt_Imp.Rows(0).Item("id_cod") <> 0 Then 'Per Terreni Nudi
                            objParametriAgenda.Veg_Cod = Dt_Imp.Rows(0).Item("veg_cod") & "/" & Dt_Imp.Rows(0).Item("id_cod")
                        Else
                            objParametriAgenda.Veg_Cod = Dt_Imp.Rows(0).Item("veg_cod")
                        End If
                    End If

                    'aggiungo impianto a parameteri agenda, cosi pagina master pouò ricreare i check,
                    'ma devo inserirlo solo se non è gia presente altrimenti mi sdoppia le colonne
                    Dim presente As Boolean = False
                    For Each ap As AgronicaCoreModello.ParametriAgenda_Temp.Impianto In objParametriAgenda.Impianti
                        If ap.Piva = objAppezzamento.Piva AndAlso ap.Sa_Cod = objAppezzamento.Sa_Cod AndAlso
                               ap.Appezza = objAppezzamento.Appezza AndAlso ap.ID_Reg = objAppezzamento.ID_Reg Then

                            presente = True
                        End If
                    Next
                    If Not presente Then
                        objParametriAgenda.Impianti.Add(objAppezzamento)
                        objParametriAgenda.salva()
                    End If

                Next
            Next

        Else
            'ci seve essere almeno un movimento dettaglio per l'installazione di una trappola in un appezzamento
            Throw New ApplicationException
        End If

    End Sub

    Private Sub impostaComboDisciplinari(Dpi_Cod As Integer, Disciplinare_PubblicoPrivato As Integer, Id_Rcdpi As Decimal)

        Select Case Dpi_Cod
            Case 0 'nessuno nessuno caso vecchio
                objParametriAgenda.Disciplinare = NessunDpiNessunaEtichetta
            Case -2 'BIO
                objParametriAgenda.Disciplinare = Dpi_Cod
            Case -1 'etichetta
            Case Else 'dpi
        End Select

        CType(Page.Master, OperazioneBootstrap).CaricaComboDisciplinariExteso()

        '------------------------------
        'Recupero il DPI
        Dim Disciplinare_Valore As String = ""

        Select Case Dpi_Cod

            Case 0  'nessun dpi nessun vincolo
                CType(Ricerca.FindControlIterative(Page.Master, "ComboDisciplinari1"), AgronicaControlli_2010.ComboDisciplinari).ddl_Disciplinari.SelectedIndex =
                        CType(Ricerca.FindControlIterative(Page.Master, "ComboDisciplinari1"), AgronicaControlli_2010.ComboDisciplinari).ddl_Disciplinari.Items.IndexOf(CType(Ricerca.FindControlIterative(Page.Master, "ComboDisciplinari1"), AgronicaControlli_2010.ComboDisciplinari).ddl_Disciplinari.Items.FindByValue(
                            NessunDpiNessunaEtichetta))

                objParametriAgenda.Disciplinare = NessunDpiNessunaEtichetta

            Case -2 'BIO
                CType(Ricerca.FindControlIterative(Page.Master, "ComboDisciplinari1"), AgronicaControlli_2010.ComboDisciplinari).ddl_Disciplinari.SelectedIndex =
                 CType(Ricerca.FindControlIterative(Page.Master, "ComboDisciplinari1"), AgronicaControlli_2010.ComboDisciplinari).ddl_Disciplinari.Items.IndexOf(CType(Ricerca.FindControlIterative(Page.Master, "ComboDisciplinari1"), AgronicaControlli_2010.ComboDisciplinari).ddl_Disciplinari.Items.FindByValue(
                     Dpi_Cod))

                objParametriAgenda.Disciplinare = Dpi_Cod

            Case Is > 0 'DPI
                Dim Disciplinare As Integer = 0
                'Dim Flag_Protetto As Integer = -1
                'Dim Flag_PubblicoPrivato As Integer = 0
                'Dim IdRcdpi As Integer = 0
                'Dim Copertura As Integer = 0

                'XXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                '02/09/2014 escludo la protezione antigrandine
                '''''If Cop_Cod <> 0 AndAlso Cop_Cod <> 1 AndAlso Cop_Cod <> 3 AndAlso Cop_Cod <> 4 AndAlso Cop_Cod <> 5 AndAlso Cop_Cod <> 6 Then
                '''''    Copertura = 1
                '''''End If

                'cerco il DPI giusto x finalita e copertura
                Dim Array() As String
                Dim Disciplinare_Cod As Integer
                Dim Id_RcdpiTmp As Integer = 0

                Dim ddlDiscip As DropDownList = CType(Ricerca.FindControlIterative(Page.Master, "ComboDisciplinari1"), AgronicaControlli_2010.ComboDisciplinari).ddl_Disciplinari

                For c = 1 To ddlDiscip.Items.Count - 1
                    Array = Split(ddlDiscip.Items(c).Value, "/")
                    Disciplinare_Cod = Array(0)
                    If Array.Length > 1 AndAlso Array(1) IsNot Nothing Then
                        Id_RcdpiTmp = Array(1)
                    End If

                    'verifico se il dpi dell'operazione è lo stesso..
                    If Dpi_Cod = Disciplinare_Cod AndAlso Id_Rcdpi = Id_RcdpiTmp Then
                        Disciplinare_Valore = ddlDiscip.Items(c).Value
                        Exit For
                    End If

                Next

                'XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                'For c = 1 To ddlDiscip.Items.Count - 1
                '    Array = Split(ddlDiscip.Items(c).Value, "/")
                '    Disciplinare_Cod = Array(0)
                '    If Array.Length > 1 AndAlso Not Array(1) Is Nothing Then
                '        Id_RcdpiTmp = Array(1)
                '    End If
                '    'verifico se il dpi dell'operazione è lo stesso..
                '    If Dpi_Cod = Disciplinare_Cod And Id_Rcdpi = Id_RcdpiTmp Then
                '        Disciplinare_Valore = ddlDiscip.Items(c).Value
                '        Exit For
                '    Else
                '        If Dpi_Cod = Disciplinare_Cod Then
                '            Dim Grfi_Cod = Array(2)
                '            Flag_Protetto = Array(3)
                '            'Flag_PubblicoPrivato = Array(4)
                '            'verifico se la finalità è = 0 (= tutte x il DPI)
                '            If Grfi_Cod = 0 Then
                '                If Copertura = 1 And Flag_Protetto = 1 Then
                '                    Disciplinare_Valore = ddlDiscip.Items(c).Value
                '                    'Exit For
                '                ElseIf (Copertura = 0 And Flag_Protetto = 0) Or (Copertura = 0 And Flag_Protetto = -1) Or (Copertura = 0 And Flag_Protetto = 2) Then
                '                    Disciplinare_Valore = ddlDiscip.Items(c).Value
                '                    'Exit For
                '                End If
                '            Else
                '                'caso in cui nel dpi è indicata la finalità..
                '                'verifico se è uguale a quella dell'impianto
                '                If Grfi_Cod = xGrfi_Cod Then
                '                    If Copertura = 1 And Flag_Protetto = 1 Then
                '                        Disciplinare_Valore = ddlDiscip.Items(c).Value
                '                        Exit For
                '                    ElseIf (Copertura = 0 And Flag_Protetto = 0) Or (Copertura = 0 And Flag_Protetto = -1) Or (Copertura = 0 And Flag_Protetto = 2) Then
                '                        Disciplinare_Valore = ddlDiscip.Items(c).Value
                '                        'Exit For
                '                    End If
                '                End If
                '            End If
                '            End If
                '    End If
                'Next

                'Imposto la selezione della combobox
                objParametriAgenda.Disciplinare = Disciplinare_Valore

                CType(Ricerca.FindControlIterative(Page.Master, "ComboDisciplinari1"), AgronicaControlli_2010.ComboDisciplinari).ddl_Disciplinari.SelectedIndex =
                    CType(Ricerca.FindControlIterative(Page.Master, "ComboDisciplinari1"), AgronicaControlli_2010.ComboDisciplinari).ddl_Disciplinari.Items.IndexOf(
                        CType(Ricerca.FindControlIterative(Page.Master, "ComboDisciplinari1"), AgronicaControlli_2010.ComboDisciplinari).ddl_Disciplinari.Items.FindByValue(
                            Disciplinare_Valore))
        End Select


    End Sub

    Private Sub LeggiMovimentoAgenda_Visite(ByRef agenda As Operazione_Agenda, ByRef i As Integer)

        ''------------------------------------------
        ''----- Recupero le informazioni
        ''------------------------------------------
        objParametriAgenda.Data = agenda.Movimenti(i).Ora
        Master_Operazione.SetNota(agenda.Movimenti(i).Mov_Desc)
        txtNote.Text = agenda.Movimenti(i).Mov_Desc

        '---------------------
        '----MOVIMENTI_DETTAGLI
        '---------------------
        'uno per ciascun impianto influenzato dalle trappole installate
        If Not IsNothing(agenda.Movimenti(i).Movimenti_Dettagli) Then

            For j = 0 To agenda.Movimenti(i).Movimenti_Dettagli.Count - 1

                'controllo corrispondeza piva con agenda
                'sa cod è uguale ma potrebbe cambiare se...
                If agenda.Piva <> agenda.Movimenti(i).Movimenti_Dettagli(j).Piva Then
                    Throw New ApplicationException
                End If

                'For r = 0 To agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count - 1

                '    '---------------------
                '    'MOVIMENTO_DESTINAZIONI
                '    '---------------------
                '    'ci deve essere almeno un movimento destinazione, uno per ciascun appezzamento 
                '    If IsNothing(agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni) Then
                '        Throw New ApplicationException
                '    End If

                '    Dim objAppezzamento As New AgronicaCoreModello.ParametriAgenda_Temp.Impianto
                '    objAppezzamento.Piva = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Piva
                '    objAppezzamento.Sa_Cod = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Sa_Cod
                '    objAppezzamento.Appezza = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Appezza
                '    objAppezzamento.ID_Reg = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Id_Destinazione
                '    objAppezzamento.Qta2 = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Qta2

                '    Dim objImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                '    Dim Dt_Imp As DataTable = objImp.Leggi(agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Piva,
                '                                             agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Sa_Cod,
                '                                             agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Appezza,
                '                                             agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Id_Destinazione,
                '                                             enumSelezioneVariabile.Selezione_JoinDescrizioni,
                '                                             "", "", objParametri_Server)

                '    If Dt_Imp.Rows.Count > 0 Then
                '        objAppezzamento.Veg_Cod = Dt_Imp.Rows(0).Item("veg_cod")
                '        If Dt_Imp.Rows(0).Item("veg_cod") = 0 AndAlso Dt_Imp.Rows(0).Item("id_cod") <> 0 Then 'Per Terreni Nudi
                '           objParametriAgenda.Veg_Cod = Dt_Imp.Rows(0).Item("veg_cod") & "/" & Dt_Imp.Rows(0).Item("id_cod")
                '        Else
                '           objParametriAgenda.Veg_Cod = Dt_Imp.Rows(0).Item("veg_cod")
                '        End If
                '    End If

                '    'aggiungo impianto a parameteri agenda, cosi pagina master pouò ricreare i check,
                '    'ma devo inserirlo solo se non è gia presente altrimenti mi sdoppia le colonne
                '    Dim presente As Boolean = False
                '    For Each ap As AgronicaCoreModello.ParametriAgenda_Temp.Impianto In objParametriAgenda.Impianti
                '        If ap.Piva = objAppezzamento.Piva AndAlso ap.Sa_Cod = objAppezzamento.Sa_Cod AndAlso
                '               ap.Appezza = objAppezzamento.Appezza AndAlso ap.ID_Reg = objAppezzamento.ID_Reg Then

                '            presente = True
                '        End If
                '    Next
                '    If Not presente Then
                '        objParametriAgenda.Impianti.Add(objAppezzamento)
                '        objParametriAgenda.salva()
                '    End If

                'Next
            Next

        Else
            'ci seve essere almeno un movimento dettaglio per l'installazione di una trappola in un appezzamento
            Throw New ApplicationException
        End If

    End Sub

    Private Sub LeggiMovimentoAgenda_Rilievi(ByRef agenda As Operazione_Agenda, ByRef i As Integer)


        ''------------------------------------------
        ''----- Dichiarazione delle Variabili
        ''------------------------------------------

        ''lista degli appezzamenti coinvolti nell'operazione agenda
        'Dim AppezzamentiCoinvolti_NumTrappole As New Hashtable()
        ''elenco trappele (numero-nome personalizzato)
        'Dim TrappoleLista As New Hashtable()
        ''Coppia trappola - appezzamento che la contiene
        'Dim Trappole_Appezzamenti As New Hashtable()
        ''Coppia innesco - trappola che la contiene che la contiene
        'Dim Inneschi_Trappole As New Hashtable()

        ''variabili di controllo
        'Dim conteggioTrappolePerVerifica As Integer = 0

        Dim rilievoAvvList As New List(Of rilievoAvv)

        ''------------------------------------------
        ''----- Recupero le informazioni
        ''------------------------------------------



        objParametriAgenda.Data = agenda.Movimenti(i).Data
        If txtNote.Text = "" Then
            Master_Operazione.SetNota(agenda.Movimenti(i).Mov_Desc)
            txtNote.Text = agenda.Movimenti(i).Mov_Desc
        End If

        'todo: gestire senza le due combo
        'If Not IsNothing(agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Dettagli_Tecnici) AndAlso
        '    agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Dettagli_Tecnici.Count > 0 Then


        '    Dim lFF_cod1 As Integer = agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Dettagli_Tecnici(0).ff_classe
        '    Dim lFF_cod2 As Integer = agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Dettagli_Tecnici(0).Piezo2

        '    If lFF_cod1 <> 0 Then
        '        ddlFF1.selectedValue = lFF_cod1
        '    End If

        '    If lFF_cod2 <> 0 Then
        '        ddlFF2.selectedValue = lFF_cod2
        '    End If
        'End If



        'MOVIMENTO_DETTAGLIO TECNICO (dovrebbe essere nothing)
        If Not IsNothing(agenda.Movimenti(i).Movimenti_Dettagli_Tecnici) Then

            For j = 0 To agenda.Movimenti(i).Movimenti_Dettagli_Tecnici.Count - 1

                'controllo corrispondeza piva con agenda
                If agenda.Piva <> agenda.Movimenti(i).Piva Then
                    Throw New ApplicationException
                End If

                'non dovrebbe essercene nessuno
                Throw New NotImplementedException
            Next

        End If



        '---------------------
        '----MOVIMENTI_DETTAGLI
        '---------------------
        'uno per ciascun impianto influenzato dalle trappole installate
        If Not IsNothing(agenda.Movimenti(i).Movimenti_Dettagli) Then

            For j = 0 To agenda.Movimenti(i).Movimenti_Dettagli.Count - 1

                'controllo corrispondeza piva con agenda
                'sa cod è uguale ma potrebbe cambiare se...
                If agenda.Piva <> agenda.Movimenti(i).Movimenti_Dettagli(j).Piva Then
                    Throw New ApplicationException
                End If

                For k = 0 To agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici.Count - 1
                    'ci deve essere un solo mov dettaglio tecnico per un mov dettaglio (piu destinazioni invece)
                    If agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici.Count <> 1 Then
                        Throw New ApplicationException
                    End If

                    For r = 0 To agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count - 1

                        Dim rilievoAvv As New rilievoAvv

                        '------------------------------
                        '----- MOVIMENTO DETTAGLIO-----
                        '------------------------------
                        'impostazioni in base alla lavorazione
                        Select Case CInt(agenda.Lav_Cod)
                            Case LAVCOD_RILIEVO_ERBE_INFESTANTI
                                'rilievoAvv.Udm_cod = sempre numero piante per metro quadro
                                rilievoAvv.Udm_cod = agenda.Movimenti(i).Movimenti_Dettagli(j).Udm_Cod
                            Case LAVCOD_FASI_FENOLOGICHE

                            Case LAVCOD_RILIEVO_AVVERSITA_CAMPO
                                'CType(TabellaTrappole.Rows(iRiga).Cells(iColonna).Controls(0), TextBox).Text
                                rilievoAvv.Udm_cod = agenda.Movimenti(i).Movimenti_Dettagli(j).Udm_Cod
                            Case LAVCOD_RILIEVO_INDICI_MATURITA, LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA, LAVCOD_DANNI_RACCOLTA

                            Case Else
                                Throw New NotImplementedException
                        End Select

                        '------------------------------
                        '----- MOVIMENTO DET TECNICO --
                        '------------------------------
                        Select Case CInt(agenda.Lav_Cod)
                            Case LAVCOD_RILIEVO_ERBE_INFESTANTI
                                'rilievoAvv.Udm_cod = sempre numero piante per metro quadro
                                rilievoAvv.Udm_cod = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(k).dett_cod
                                rilievoAvv.Av_cod = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(k).Av_Gru
                            Case LAVCOD_FASI_FENOLOGICHE
                                rilievoAvv.Av_cod = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(k).ff_classe
                            Case LAVCOD_RILIEVO_AVVERSITA_CAMPO
                                rilievoAvv.Udm_cod = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(k).dett_cod
                                rilievoAvv.Av_cod = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(k).Av_Cod
                            Case LAVCOD_RILIEVO_INDICI_MATURITA, LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA, LAVCOD_DANNI_RACCOLTA
                                rilievoAvv.Udm_cod = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(k).dett_cod
                                rilievoAvv.Av_cod = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(k).ff_classe
                            Case Else
                                Throw New NotImplementedException
                        End Select

                        '---------------------
                        'MOVIMENTO_DESTINAZIONI
                        '---------------------
                        'ci deve essere almeno un movimento destinazione, uno per ciascun appezzamento 
                        If IsNothing(agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni) Then
                            Throw New ApplicationException
                        End If

                        Dim objAppezzamento As New AgronicaCoreModello.ParametriAgenda_Temp.Impianto

                        objAppezzamento.Piva = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Piva
                        objAppezzamento.Sa_Cod = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Sa_Cod
                        objAppezzamento.Appezza = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Appezza
                        objAppezzamento.ID_Reg = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Id_Destinazione

                        objAppezzamento.Qta2 = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Qta2


                        Dim objImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                        Dim Dt_Imp As DataTable = objImp.Leggi(agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Piva,
                                                 agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Sa_Cod,
                                                 agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Appezza,
                                                 agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Id_Destinazione,
                                                 enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                 "", "", objParametri_Server)

                        If Dt_Imp.Rows.Count > 0 Then
                            objAppezzamento.Veg_Cod = Dt_Imp.Rows(0).Item("veg_cod")
                            If Dt_Imp.Rows(0).Item("veg_cod") = 0 AndAlso Dt_Imp.Rows(0).Item("id_cod") <> 0 Then 'Per Terreni Nudi
                                objParametriAgenda.Veg_Cod = Dt_Imp.Rows(0).Item("veg_cod") & "/" & Dt_Imp.Rows(0).Item("id_cod")
                            Else
                                objParametriAgenda.Veg_Cod = Dt_Imp.Rows(0).Item("veg_cod")
                            End If
                        End If

                        'aggiungo impianto a parameteri agenda, cosi pagina master può ricreare i check,
                        'ma devo inserirlo solo se non è gia presente altrimenti mi sdoppia le colonne
                        Dim presente As Boolean = False
                        For Each ap As AgronicaCoreModello.ParametriAgenda_Temp.Impianto In objParametriAgenda.Impianti
                            If ap.Piva = objAppezzamento.Piva AndAlso ap.Sa_Cod = objAppezzamento.Sa_Cod AndAlso ap.Appezza = objAppezzamento.Appezza AndAlso ap.ID_Reg = objAppezzamento.ID_Reg Then

                                presente = True
                            End If
                        Next
                        If Not presente Then
                            objParametriAgenda.Impianti.Add(objAppezzamento)
                            objParametriAgenda.salva()
                        End If



                        'parametri operazione
                        rilievoAvv.Impianto = objAppezzamento
                        '  Vanni, 27/05/2014 18:03:00: 
                        rilievoAvv.mov_destinazioni_graphickey = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).mov_destinazioni_graphickey
                        Select Case CInt(agenda.Lav_Cod)
                            Case LAVCOD_RILIEVO_ERBE_INFESTANTI, LAVCOD_RILIEVO_AVVERSITA_CAMPO, LAVCOD_RILIEVO_INDICI_MATURITA, LAVCOD_DANNI_RACCOLTA, LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA
                                rilievoAvv.Valore = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Qta
                            Case LAVCOD_FASI_FENOLOGICHE
                                rilievoAvv.Valore = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Data
                            Case Else
                                Throw New NotImplementedException
                        End Select

                        rilievoAvv.Lav_Cod = agenda.Lav_Cod

                        rilievoAvvList.Add(rilievoAvv)

                    Next
                Next
            Next


        Else
            'ci seve essere almeno un movimento dettaglio per l'installazione di una trappola in un appezzamento
            Throw New ApplicationException
        End If

        ''---------Inserisco i dati in objParametriAgenda ------------
        'objParametriAgenda.Rilievi = rilievoAvvList
        objParametriAgenda.Rilievi.AddRange(rilievoAvvList)


    End Sub


    Private Sub GeneraTabella(ByVal genera_DaMaster As Boolean, ByRef Agenda As Operazione_Agenda)

        '-------------------------Genero le tabelle avversità per la lavorazione-------------------------------------

        Dim Dt As New DataTable

        Dim DT_Infestanti As DataTable


        Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

        Dim Flag_Disciplinare As Boolean = False

        Dim DT_Impianti As DataTable = objImpianti.Leggi_Impianti_xAgenda3(Flag_Disciplinare, objParametriAgenda.Piva,
                                                                           0, 0, 0, 0, objParametriAgenda.Data, 0, 0, False,
                                                                           0, "", " App_Nome, Cul_Des, Progetto ", objParametri_Server, True)

        GeneraTabellaStrutturaColonne(Dt)

        Dim strImpianti As String = ""
        hdImpianti.Value = ""

        Dim N_Righe As Integer = 0
        Dim N_Colonne As Integer = 0

        Dim ListaImpianti As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)
        If genera_DaMaster Then
            'genero tabella da impianti master
            ListaImpianti = CType(Master, OperazioneBootstrap).GetImpianti()
        Else
            'genero tabella da oggetto parametri agenda, quando sono in modifica e devo generare
            'la tabella prima che venga chiamata la load della master
            ListaImpianti = objParametriAgenda.Impianti
        End If


        N_Colonne = ListaImpianti.Count
        If N_Colonne = 0 AndAlso Agenda.Lav_Cod <> LAVCOD_VISITA Then
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.SelezionareAlmenoUnImpiantoColturale, Page, ,
                                CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            Return

        Else
            For i = 0 To ListaImpianti.Count - 1
                Dt.Columns.Add(New DataColumn("App_" & i + 1, GetType(String)))
                'Dt.Columns("App_" & i + 1).ColumnName =ListaImpianti(i).Sa_Nome & " </br> " & ListaImpianti(i).Campo_Des & " </br> " & ListaImpianti(i).App_Nome & " </br>(" & ListaImpianti(i).Cul_Des & ")"
                Dt.Columns("App_" & i + 1).Caption = ListaImpianti(i).Sa_Nome & " </br> " & ListaImpianti(i).Campo_Des & " </br> " & ListaImpianti(i).App_Nome & " </br>(" & ListaImpianti(i).Cul_Des & ")"
                strImpianti &= "{" & """IdColonna"":""App_" & i + 1 & """, ""Chiave"":""" & ListaImpianti(i).Piva & "_" & ListaImpianti(i).Sa_Cod & "_" & ListaImpianti(i).Appezza & "_" & ListaImpianti(i).ID_Reg & """" & "},"
            Next
            If strImpianti <> "" Then
                strImpianti = Left(strImpianti, strImpianti.Length - 1)
                hdImpianti.Value = "[" & strImpianti & "]"
            End If
        End If


        Select Case (objParametriAgenda.Lav_Cod)

            Case LAVCOD_RILIEVO_ERBE_INFESTANTI
                Dt.Columns(1).Caption = DirectCast(GetLocalResourceObject("Infestante"), String)
                Dt.Columns(2).Caption = DirectCast(GetLocalResourceObject("PiantePerMetroQuadroDefault"), String)

                objParametriAgenda.Cau_Mov = CStr(enum_Agenda_Causali.RILIEVO_CAMPO)
                DT_Infestanti = New AgronicaCoreMetaSchemaDAL.GruppoAvversitaAttive_R().Leggi(0, 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            Case LAVCOD_FASI_FENOLOGICHE

                objParametriAgenda.Cau_Mov = CStr(enum_Agenda_Causali.RILIEVO_CAMPO)

                Dim objParametriUscitaFasiFenologiche As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output = LeggiFasiFenologicheDaWS()

                Dim codiceRilievo As Integer = 0
                For Each curRilievo As rilievoAvv In objParametriAgenda.Rilievi
                    codiceRilievo += 1
                    AggiungiRigaTabellaFinale_DaRilievoFasiFenologiche(DT_Impianti, objParametriUscitaFasiFenologiche.ListaFasiFenologiche, curRilievo, Dt, codiceRilievo)
                Next

            Case LAVCOD_RILIEVO_AVVERSITA_CAMPO

                objParametriAgenda.Cau_Mov = CStr(enum_Agenda_Causali.RILIEVO_CAMPO)

                Dim objParametriUscitaAvversita As AgronicaCoreMetaSchemaBIZ.MisuraXAvversita_output = LeggiAvversitaDaWS()

                Dim codiceRilievo As Integer = 0
                For Each curRilievo As rilievoAvv In objParametriAgenda.Rilievi
                    codiceRilievo += 1
                    AggiungiRigaTabellaFinale_DaRilievoAvversita(DT_Impianti, objParametriUscitaAvversita.ListaMisureXAvversita, curRilievo, Dt, codiceRilievo)
                Next

                ''estraggo i rilievi indipendentemente dall'appezzamento
                'Dim singoliRilievi As New List(Of rilievoAvv)
                'Dim strChiave As New List(Of String)
                'For Each curRilievo As rilievoAvv In objParametriAgenda.Rilievi
                '    If Not strChiave.Contains(curRilievo.Av_cod & "|" & curRilievo.Udm_cod) Then
                '        singoliRilievi.Add(curRilievo)
                '        strChiave.Add(curRilievo.Av_cod & "|" & curRilievo.Udm_cod)
                '    End If
                'Next

                'Dim codiceRilievo As Integer = 0
                'For Each curRilievo As rilievoAvv In singoliRilievi
                '    codiceRilievo += 1
                '    AggiungiRigaTabellaFinale_DaRilievoAvversita(objParametriUscitaAvversita.ListaMisureXAvversita, curRilievo, Dt, codiceRilievo)
                'Next

            Case LAVCOD_RILIEVO_INDICI_MATURITA

                objParametriAgenda.Cau_Mov = CStr(enum_Agenda_Causali.RILIEVO_RACCOLTA)

                Dim objParametriUscitaIndiciMaturita As AgronicaCoreMetaSchemaBIZ.IndiciMaturita_output = LeggiIndiciMaturitaDaWS()

                Dim codiceRilievo As Integer = 0
                For Each curRilievo As rilievoAvv In objParametriAgenda.Rilievi
                    codiceRilievo += 1
                    AggiungiRigaTabellaFinale_DaRilievoIndiciMaturita(DT_Impianti, objParametriUscitaIndiciMaturita.ListaIndiciMaturita, curRilievo, Dt, codiceRilievo)
                Next

                ''estraggo i rilievi indipendentemente dall'appezzamento
                'Dim singoliRilievi As New List(Of rilievoAvv)
                'Dim strChiave As New List(Of String)
                'For Each curRilievo As rilievoAvv In objParametriAgenda.Rilievi
                '    If Not strChiave.Contains(curRilievo.Av_cod & "|" & curRilievo.Udm_cod) Then
                '        singoliRilievi.Add(curRilievo)
                '        strChiave.Add(curRilievo.Av_cod & "|" & curRilievo.Udm_cod)
                '    End If
                'Next

                'Dim codiceRilievo As Integer = 0
                'For Each curRilievo As rilievoAvv In singoliRilievi
                '    codiceRilievo += 1
                '    AggiungiRigaTabellaFinale_DaRilievoIndiciMaturita(objParametriUscitaIndiciMaturita.ListaIndiciMaturita, curRilievo, Dt, codiceRilievo)
                'Next

            Case LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA

                objParametriAgenda.Cau_Mov = CStr(enum_Agenda_Causali.RILIEVO_RACCOLTA)

                Dim objParametriUscitaIndiciMaturita As AgronicaCoreMetaSchemaBIZ.IndiciMaturita_output = LeggiIndiciMaturitaDaWS()

                Dim codiceRilievo As Integer = 0
                For Each curRilievo As rilievoAvv In objParametriAgenda.Rilievi
                    codiceRilievo += 1
                    AggiungiRigaTabellaFinale_DaRilievoIndiciReseRaccolta(DT_Impianti, objParametriUscitaIndiciMaturita.ListaIndiciMaturita, curRilievo, Dt, codiceRilievo)
                Next

            Case LAVCOD_DANNI_RACCOLTA

                objParametriAgenda.Cau_Mov = CStr(enum_Agenda_Causali.RILIEVO_RACCOLTA)

                Dim objParametriUscitaDanniRaccolta As AgronicaCoreMetaSchemaBIZ.MisuraXDanniRaccolta_output = LeggiDanniRaccoltaDaWS()

                Dim codiceRilievo As Integer = 0
                For Each curRilievo As rilievoAvv In objParametriAgenda.Rilievi
                    codiceRilievo += 1
                    AggiungiRigaTabellaFinale_DaRilievoDanniRaccolta(DT_Impianti, objParametriUscitaDanniRaccolta.ListaMisureXDanniRaccolta, curRilievo, Dt, codiceRilievo)
                Next

            Case LAVCOD_VISITA

                Dim codiceRiga As Integer = 0

                'RILIEVI AVVERSITA IN CAMPO
                If objParametriAgenda.Rilievi.Count > 0 Then

                    'objParametriAgenda.Cau_Mov = CStr(enum_Agenda_Causali.RILIEVO_CAMPO)


                    Dim objParametriUscitaAvversita As AgronicaCoreMetaSchemaBIZ.MisuraXAvversita_output = Nothing
                    Dim objParametriUscitaIndiciMaturita As AgronicaCoreMetaSchemaBIZ.IndiciMaturita_output = Nothing
                    Dim objParametriUscitaFasiFenologiche As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output = Nothing
                    Dim objParametriUscitaDanniRaccolta As AgronicaCoreMetaSchemaBIZ.MisuraXDanniRaccolta_output = Nothing

                    For Each curRilievo As rilievoAvv In objParametriAgenda.Rilievi
                        codiceRiga += 1

                        Select Case curRilievo.Lav_Cod

                            Case LAVCOD_RILIEVO_AVVERSITA_CAMPO
                                If IsNothing(objParametriUscitaAvversita) Then
                                    objParametriUscitaAvversita = LeggiAvversitaDaWS()
                                End If
                                AggiungiRigaTabellaFinale_DaRilievoAvversita(DT_Impianti, objParametriUscitaAvversita.ListaMisureXAvversita, curRilievo, Dt, codiceRiga)

                            Case LAVCOD_RILIEVO_INDICI_MATURITA
                                If IsNothing(objParametriUscitaIndiciMaturita) Then
                                    objParametriUscitaIndiciMaturita = LeggiIndiciMaturitaDaWS()
                                End If
                                AggiungiRigaTabellaFinale_DaRilievoIndiciMaturita(DT_Impianti, objParametriUscitaIndiciMaturita.ListaIndiciMaturita, curRilievo, Dt, codiceRiga)

                            Case LAVCOD_FASI_FENOLOGICHE
                                If IsNothing(objParametriUscitaFasiFenologiche) Then
                                    objParametriUscitaFasiFenologiche = LeggiFasiFenologicheDaWS()
                                End If
                                AggiungiRigaTabellaFinale_DaRilievoFasiFenologiche(DT_Impianti, objParametriUscitaFasiFenologiche.ListaFasiFenologiche, curRilievo, Dt, codiceRiga)

                            Case LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA
                                If IsNothing(objParametriUscitaIndiciMaturita) Then
                                    objParametriUscitaIndiciMaturita = LeggiIndiciMaturitaDaWS()
                                End If
                                AggiungiRigaTabellaFinale_DaRilievoIndiciReseRaccolta(DT_Impianti, objParametriUscitaIndiciMaturita.ListaIndiciMaturita, curRilievo, Dt, codiceRiga)

                            Case LAVCOD_DANNI_RACCOLTA
                                If IsNothing(objParametriUscitaDanniRaccolta) Then
                                    objParametriUscitaDanniRaccolta = LeggiDanniRaccoltaDaWS()
                                End If
                                AggiungiRigaTabellaFinale_DaRilievoDanniRaccolta(DT_Impianti, objParametriUscitaDanniRaccolta.ListaMisureXDanniRaccolta, curRilievo, Dt, codiceRiga)

                            Case Else
                                Throw New Exception()

                        End Select

                    Next

                End If

                If Not IsNothing(Agenda) Then
                    Dim objAgenda As New Agenda_Operazione_Helper

                    For Each opCollegata As Movimento_Dettaglio_Riferimento In Agenda.Movimenti(0).Movimenti_Dettagli(0).Movimenti_Dettagli_Riferimenti

                        Select Case opCollegata.Lav_Cod_Rif
                            Case LAVCOD_ALTRE_OPERAZIONI

                                Dim AgendaCollegata As Operazione_Agenda = objAgenda.Leggi(opCollegata.Piva_Rif, opCollegata.Sa_Cod_Rif, opCollegata.Id_Agenda_Rif, 0, objParametri_Server)

                                For Each mov As Movimento In AgendaCollegata.Movimenti
                                    If mov.Cau_Mov = CAU_LAVORAZIONE Then

                                        If mov.Movimenti_Dettagli(0).Movimenti_Destinazioni.Count > 0 Then
                                            For Each movDest As Movimento_Destinazione In mov.Movimenti_Dettagli(0).Movimenti_Destinazioni
                                                codiceRiga += 1
                                                AggiungiRigaTabellaFinale_DaAltreLavorazioni(DT_Impianti, AgendaCollegata, mov.Movimenti_Dettagli(0), movDest, Dt, codiceRiga)
                                            Next

                                            Exit For
                                        Else
                                            'Creo un movimento destinazione fittizio con i dati che mi servono
                                            Dim movDest As New Movimento_Destinazione With {.Piva = AgendaCollegata.Piva, .Sa_Cod = AgendaCollegata.Sa_Cod}
                                            codiceRiga += 1
                                            AggiungiRigaTabellaFinale_DaAltreLavorazioni(DT_Impianti, AgendaCollegata, mov.Movimenti_Dettagli(0), movDest, Dt, codiceRiga)

                                        End If


                                    End If
                                Next

                        End Select

                    Next
                End If

        End Select
        '-----------------------------------------------------------------------------------------

        Select Case (objParametriAgenda.Lav_Cod)
            Case LAVCOD_FASI_FENOLOGICHE
                Dim dwT As DataView = Dt.DefaultView
                dwT.Sort = "impianto, ff_cod, dato"
                Dt = dwT.ToTable
        End Select


        ''  Vanni, 27/05/2014 18:00:11: al momento memorizziamo un punto per ogni operazione, che è uguale per tutti.
        'If objParametriAgenda.Rilievi.Count > 0 Then
        '    tcell.Attributes.Add("mov_destinazioni_graphickey", objParametriAgenda.Rilievi.FirstOrDefault.mov_destinazioni_graphickey)
        'End If

        Session("Tabella") = Dt

    End Sub

    Private Sub AggiungiRigaTabellaFinale_DaRilievoAvversita_OLD(
        ByVal Lista_Avversita As List(Of AgronicaCoreMetaSchemaBIZ.MisuraXAvv),
        rilievo As rilievoAvv,
        ByRef dt As DataTable,
        ByRef codiceRilievo As Integer
    )

        Dim objParametriAgenda_TEMP As New ParametriAgenda
        Dim dr As DataRow = dt.NewRow()

        dr("Codice") = "i_" & codiceRilievo.ToString
        dr("Impianto") = ""
        dr("piva") = ""
        dr("sa_cod") = 0
        dr("appezza") = 0
        dr("id_reg") = 0

        'todo: la data sarà gestita su tabella rilievi
        dr("Data") = objParametriAgenda.Data

        dr("lav_cod") = LAVCOD_RILIEVO_AVVERSITA_CAMPO 'objParametriAgenda.Lav_Cod 'XXXXXXXXXXXXXXXXXXXXXXX
        dr("Operazione") = "Rilievo Avversità in Campo" 'objParametriAgenda.Lav_Des 'XXXXXXXXXXXXXXXXXXXXXXX
        dr("av_cod") = rilievo.Av_cod
        dr("udm_cod") = rilievo.Udm_cod
        dr("ff_cod") = 0
        dr("Descrizione") = RilievoAvversitaGeneraDescrizione(Lista_Avversita, rilievo)
        dr("Dato") = RilievoAvversitaGeneraAnagrafica(Lista_Avversita, rilievo)
        dr("Dato_Testuale") = RilievoAvversitaGeneraAnagrafica_DatoTestuale(Lista_Avversita, rilievo)

        dr("Cul_Des") = ""
        dr("Qta2") = 0

        dr("Veg_Cod") = objParametriAgenda.Veg_Cod
        dr("ind_mat_cod") = 0
        dr("dr_cod") = 0

        dr("Descrizione_Unica") = If(objParametriAgenda_TEMP.Lav_Cod = LAVCOD_VISITA,
                                                dr("Operazione") & " - <i>" & dr("Impianto") & "</i> - " & dr("Descrizione"),
                                                "<i>" & dr("Impianto") & "</i> - " & dr("Descrizione"))

        dt.Rows.Add(dr)

    End Sub

    Private Sub AggiungiRigaTabellaFinale_DaRilievoAvversita(
        ByVal DT_Impianti As DataTable,
        ByVal Lista_Avversita As List(Of AgronicaCoreMetaSchemaBIZ.MisuraXAvv),
        rilievo As rilievoAvv,
        ByRef dt As DataTable,
        ByRef codiceRilievo As Integer
    )

        Dim objParametriAgenda_TEMP As New ParametriAgenda
        Dim dr As DataRow = dt.NewRow()

        dr("Codice") = "i_" & codiceRilievo.ToString
        dr("Impianto") = ImpiantoGeneraDescrizione(DT_Impianti, rilievo)
        dr("piva") = rilievo.Impianto.Piva
        dr("sa_cod") = rilievo.Impianto.Sa_Cod
        dr("appezza") = rilievo.Impianto.Appezza
        dr("id_reg") = rilievo.Impianto.ID_Reg

        'todo: la data sarà gestita su tabella rilievi
        dr("Data") = objParametriAgenda.Data

        dr("lav_cod") = LAVCOD_RILIEVO_AVVERSITA_CAMPO 'objParametriAgenda.Lav_Cod 'XXXXXXXXXXXXXXXXXXXXXXX
        dr("Operazione") = DirectCast(GetLocalResourceObject("RilievoAvversitàInCampo"), String) 'objParametriAgenda.Lav_Des 'XXXXXXXXXXXXXXXXXXXXXXX
        dr("av_cod") = rilievo.Av_cod
        dr("udm_cod") = rilievo.Udm_cod
        dr("ff_cod") = 0
        dr("Descrizione") = RilievoAvversitaGeneraDescrizione(Lista_Avversita, rilievo)
        dr("Dato") = RilievoAvversitaGeneraAnagrafica(Lista_Avversita, rilievo)
        dr("Dato_Testuale") = RilievoAvversitaGeneraAnagrafica_DatoTestuale(Lista_Avversita, rilievo)

        dr("Cul_Des") = ImpiantoOttieniCulDes(DT_Impianti, rilievo)
        dr("Qta2") = rilievo.Impianto.Qta2

        dr("Veg_Cod") = rilievo.Impianto.Veg_Cod
        dr("ind_mat_cod") = 0
        dr("dr_cod") = 0

        dr("Descrizione_Unica") = If(objParametriAgenda_TEMP.Lav_Cod = LAVCOD_VISITA,
                                                dr("Operazione") & " - <i>" & dr("Impianto") & "</i> - " & dr("Descrizione"),
                                                dr("Impianto") & " - " & dr("Descrizione"))

        dt.Rows.Add(dr)

    End Sub

    Private Sub AggiungiRigaTabellaFinale_DaRilievoIndiciMaturita_OLD(
        ByVal Lista_IndiciMaturita As List(Of AgronicaCoreMetaSchemaBIZ.IndiciMaturita),
        rilievo As rilievoAvv,
        ByRef dt As DataTable,
        ByRef codiceRilievo As Integer
    )

        Dim objParametriAgenda_TEMP As New ParametriAgenda
        Dim dr As DataRow = dt.NewRow()

        dr("Codice") = "i_" & codiceRilievo.ToString
        dr("Impianto") = ""
        dr("piva") = ""
        dr("sa_cod") = 0
        dr("appezza") = 0
        dr("id_reg") = 0

        'todo: la data sarà gestita su tabella rilievi
        dr("Data") = objParametriAgenda.Data

        dr("lav_cod") = LAVCOD_RILIEVO_INDICI_MATURITA 'objParametriAgenda.Lav_Cod 'XXXXXXXXXXXXXXXXXXXXXXX
        dr("Operazione") = "Rilievo Indici di Maturità" 'objParametriAgenda.Lav_Des 'XXXXXXXXXXXXXXXXXXXXXXX
        dr("av_cod") = 0
        dr("udm_cod") = rilievo.Udm_cod
        dr("ff_cod") = 0
        dr("Descrizione") = RilievoIndiceMaturitaGeneraDescrizione(Lista_IndiciMaturita, rilievo)
        dr("Dato") = RilievoIndiceMaturitaGeneraAnagrafica(Lista_IndiciMaturita, rilievo)
        dr("Dato_Testuale") = RilievoIndiceMaturitaGeneraAnagrafica_DatoTestuale(Lista_IndiciMaturita, rilievo)

        dr("Cul_Des") = ""
        dr("Qta2") = 0

        dr("Veg_Cod") = objParametriAgenda.Veg_Cod
        dr("ind_mat_cod") = rilievo.Av_cod
        dr("dr_cod") = 0

        dr("Descrizione_Unica") = If(objParametriAgenda_TEMP.Lav_Cod = LAVCOD_VISITA,
                                                dr("Operazione") & " - <i>" & dr("Impianto") & "</i> - " & dr("Descrizione"),
                                                dr("Impianto") & " - " & dr("Descrizione"))

        dt.Rows.Add(dr)

    End Sub

    Private Sub AggiungiRigaTabellaFinale_DaRilievoIndiciMaturita(
        ByVal DT_Impianti As DataTable,
        ByVal Lista_IndiciMaturita As List(Of AgronicaCoreMetaSchemaBIZ.IndiciMaturita),
        rilievo As rilievoAvv,
        ByRef dt As DataTable,
        ByRef codiceRilievo As Integer
    )

        Dim objParametriAgenda_TEMP As New ParametriAgenda
        Dim dr As DataRow = dt.NewRow()

        dr("Codice") = "i_" & codiceRilievo.ToString
        dr("Impianto") = ImpiantoGeneraDescrizione(DT_Impianti, rilievo)
        dr("piva") = rilievo.Impianto.Piva
        dr("sa_cod") = rilievo.Impianto.Sa_Cod
        dr("appezza") = rilievo.Impianto.Appezza
        dr("id_reg") = rilievo.Impianto.ID_Reg

        'todo: la data sarà gestita su tabella rilievi
        dr("Data") = objParametriAgenda.Data

        dr("lav_cod") = LAVCOD_RILIEVO_INDICI_MATURITA 'objParametriAgenda.Lav_Cod 'XXXXXXXXXXXXXXXXXXXXXXX
        dr("Operazione") = DirectCast(GetLocalResourceObject("RilievoIndiciDiMaturità"), String) 'objParametriAgenda.Lav_Des 'XXXXXXXXXXXXXXXXXXXXXXX
        dr("av_cod") = 0
        dr("udm_cod") = rilievo.Udm_cod
        dr("ff_cod") = 0
        dr("Descrizione") = RilievoIndiceMaturitaGeneraDescrizione(Lista_IndiciMaturita, rilievo)
        dr("Dato") = RilievoIndiceMaturitaGeneraAnagrafica(Lista_IndiciMaturita, rilievo)
        dr("Dato_Testuale") = RilievoIndiceMaturitaGeneraAnagrafica_DatoTestuale(Lista_IndiciMaturita, rilievo)

        dr("Cul_Des") = ImpiantoOttieniCulDes(DT_Impianti, rilievo)
        dr("Qta2") = rilievo.Impianto.Qta2

        dr("Veg_Cod") = rilievo.Impianto.Veg_Cod
        dr("ind_mat_cod") = rilievo.Av_cod
        dr("dr_cod") = 0

        dr("Descrizione_Unica") = If(objParametriAgenda_TEMP.Lav_Cod = LAVCOD_VISITA,
                                                dr("Operazione") & " - <i>" & dr("Impianto") & "</i> - " & dr("Descrizione"),
                                                dr("Impianto") & " - " & dr("Descrizione"))

        dt.Rows.Add(dr)

    End Sub

    Private Sub AggiungiRigaTabellaFinale_DaRilievoIndiciReseRaccolta(
        ByVal DT_Impianti As DataTable,
        ByVal Lista_IndiciMaturita As List(Of AgronicaCoreMetaSchemaBIZ.IndiciMaturita),
        rilievo As rilievoAvv,
        ByRef dt As DataTable,
        ByRef codiceRilievo As Integer
    )

        Dim objParametriAgenda_TEMP As New ParametriAgenda
        Dim dr As DataRow = dt.NewRow()

        dr("Codice") = "i_" & codiceRilievo.ToString
        dr("Impianto") = ImpiantoGeneraDescrizione(DT_Impianti, rilievo)
        dr("piva") = rilievo.Impianto.Piva
        dr("sa_cod") = rilievo.Impianto.Sa_Cod
        dr("appezza") = rilievo.Impianto.Appezza
        dr("id_reg") = rilievo.Impianto.ID_Reg

        'todo: la data sarà gestita su tabella rilievi
        dr("Data") = objParametriAgenda.Data

        dr("lav_cod") = LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA 'objParametriAgenda.Lav_Cod 'XXXXXXXXXXXXXXXXXXXXXXX
        dr("Operazione") = DirectCast(GetLocalResourceObject("RilievoIndiciReseRaccolta"), String) 'objParametriAgenda.Lav_Des 'XXXXXXXXXXXXXXXXXXXXXXX
        dr("av_cod") = 0
        dr("udm_cod") = rilievo.Udm_cod
        dr("ff_cod") = 0
        dr("Descrizione") = RilievoIndiceMaturitaGeneraDescrizione(Lista_IndiciMaturita, rilievo)
        dr("Dato") = RilievoIndiceMaturitaGeneraAnagrafica(Lista_IndiciMaturita, rilievo)
        dr("Dato_Testuale") = RilievoIndiceMaturitaGeneraAnagrafica_DatoTestuale(Lista_IndiciMaturita, rilievo)

        dr("Cul_Des") = ImpiantoOttieniCulDes(DT_Impianti, rilievo)
        dr("Qta2") = rilievo.Impianto.Qta2

        dr("Veg_Cod") = rilievo.Impianto.Veg_Cod
        dr("ind_mat_cod") = rilievo.Av_cod
        dr("dr_cod") = 0

        dr("Descrizione_Unica") = If(objParametriAgenda_TEMP.Lav_Cod = LAVCOD_VISITA,
                                                dr("Operazione") & " - <i>" & dr("Impianto") & "</i> - " & dr("Descrizione"),
                                                dr("Impianto") & " - " & dr("Descrizione"))

        dt.Rows.Add(dr)

    End Sub

    Private Sub AggiungiRigaTabellaFinale_DaRilievoFasiFenologiche(
        ByVal DT_Impianti As DataTable,
        ByVal Lista_FasiFenologiche As List(Of AgronicaCoreMetaSchemaBIZ.FaseFenologica),
        rilievo As rilievoAvv,
        ByRef dt As DataTable,
        ByRef codiceRilievo As Integer
    )

        Dim objParametriAgenda_TEMP As New ParametriAgenda
        Dim dr As DataRow = dt.NewRow()

        dr("Codice") = "i_" & codiceRilievo.ToString
        dr("Impianto") = ImpiantoGeneraDescrizione(DT_Impianti, rilievo)
        dr("piva") = rilievo.Impianto.Piva
        dr("sa_cod") = rilievo.Impianto.Sa_Cod
        dr("appezza") = rilievo.Impianto.Appezza
        dr("id_reg") = rilievo.Impianto.ID_Reg

        'todo: la data sarà gestita su tabella rilievi
        dr("Data") = objParametriAgenda.Data

        dr("lav_cod") = LAVCOD_FASI_FENOLOGICHE 'objParametriAgenda.Lav_Cod 'XXXXXXXXXXXXXXXXXXXXXXX
        dr("Operazione") = DirectCast(GetLocalResourceObject("RilievoFaseFenologica"), String) 'objParametriAgenda.Lav_Des 'XXXXXXXXXXXXXXXXXXXXXXX
        dr("av_cod") = 0
        dr("udm_cod") = 0
        dr("ff_cod") = rilievo.Av_cod
        dr("Descrizione") = RilievoFasiFenologicheGeneraDescrizione(Lista_FasiFenologiche, rilievo)
        dr("Dato") = RilievoFasiFenologicheGeneraAnagrafica(Lista_FasiFenologiche, rilievo)
        dr("Dato_Testuale") = RilievoFasiFenologicheGeneraAnagrafica_DatoTestuale(Lista_FasiFenologiche, rilievo)

        dr("Cul_Des") = ImpiantoOttieniCulDes(DT_Impianti, rilievo)
        dr("Qta2") = rilievo.Impianto.Qta2

        dr("Veg_Cod") = rilievo.Impianto.Veg_Cod
        dr("ind_mat_cod") = 0
        dr("dr_cod") = 0

        dr("Descrizione_Unica") = If(objParametriAgenda_TEMP.Lav_Cod = LAVCOD_VISITA,
                                                dr("Operazione") & " - <i>" & dr("Impianto") & "</i> - " & dr("Descrizione"),
                                                dr("Impianto") & " - " & dr("Descrizione"))

        dt.Rows.Add(dr)

    End Sub

    Private Sub AggiungiRigaTabellaFinale_DaRilievoDanniRaccolta(
        ByVal DT_Impianti As DataTable,
        ByVal Lista_DanniRaccolta As List(Of AgronicaCoreMetaSchemaBIZ.MisuraXDR),
        rilievo As rilievoAvv,
        ByRef dt As DataTable,
        ByRef codiceRilievo As Integer
    )

        Dim objParametriAgenda_TEMP As New ParametriAgenda
        Dim dr As DataRow = dt.NewRow()

        dr("Codice") = "i_" & codiceRilievo.ToString
        dr("Impianto") = ImpiantoGeneraDescrizione(DT_Impianti, rilievo)
        dr("piva") = rilievo.Impianto.Piva
        dr("sa_cod") = rilievo.Impianto.Sa_Cod
        dr("appezza") = rilievo.Impianto.Appezza
        dr("id_reg") = rilievo.Impianto.ID_Reg

        'todo: la data sarà gestita su tabella rilievi
        dr("Data") = objParametriAgenda.Data

        dr("lav_cod") = LAVCOD_DANNI_RACCOLTA 'objParametriAgenda.Lav_Cod 'XXXXXXXXXXXXXXXXXXXXXXX
        dr("Operazione") = DirectCast(GetLocalResourceObject("RilievoDanniAllaRaccolta"), String) 'objParametriAgenda.Lav_Des 'XXXXXXXXXXXXXXXXXXXXXXX
        dr("av_cod") = 0
        dr("udm_cod") = rilievo.Udm_cod
        dr("ff_cod") = 0
        dr("Descrizione") = RilievoDanniRaccoltaGeneraDescrizione(Lista_DanniRaccolta, rilievo)
        dr("Dato") = RilievoDanniRaccoltaGeneraAnagrafica(Lista_DanniRaccolta, rilievo)
        dr("Dato_Testuale") = RilievoDanniRaccoltaGeneraAnagrafica_DatoTestuale(Lista_DanniRaccolta, rilievo)

        dr("Cul_Des") = ImpiantoOttieniCulDes(DT_Impianti, rilievo)
        dr("Qta2") = rilievo.Impianto.Qta2

        dr("Veg_Cod") = rilievo.Impianto.Veg_Cod
        dr("ind_mat_cod") = 0
        dr("dr_cod") = rilievo.Av_cod

        dr("Descrizione_Unica") = If(objParametriAgenda_TEMP.Lav_Cod = LAVCOD_VISITA,
                                                dr("Operazione") & " - <i>" & dr("Impianto") & "</i> - " & dr("Descrizione"),
                                                dr("Impianto") & " - " & dr("Descrizione"))

        dt.Rows.Add(dr)

    End Sub

    Private Sub AggiungiRigaTabellaFinale_DaAltreLavorazioni(
        ByVal DT_Impianti As DataTable,
        ByRef Agenda As Operazione_Agenda,
        ByRef movDet As Movimento_Dettaglio,
        movDest As Movimento_Destinazione,
        ByRef dt As DataTable,
        ByRef codiceRilievo As Integer
    )

        Dim a_R As New AgronicaCoreContabDAL.Attivita_R
        Dim attivitaDes = a_R.AttivitaDes_From_AttivitaCod(Agenda.Id_Attivita, objParametri_Server)

        Dim objParametriAgenda_TEMP As New ParametriAgenda
        Dim dr As DataRow = dt.NewRow()

        dr("Codice") = "i_" & codiceRilievo.ToString
        dr("Impianto") = ImpiantoGeneraDescrizione(DT_Impianti, movDest)
        dr("piva") = movDest.Piva
        dr("sa_cod") = movDest.Sa_Cod
        dr("appezza") = movDest.Appezza
        dr("id_reg") = movDest.Id_Destinazione

        'todo: la data sarà gestita su tabella rilievi
        dr("Data") = objParametriAgenda.Data

        dr("lav_cod") = LAVCOD_ALTRE_OPERAZIONI & "|" & Agenda.Id_Attivita 'XXXXXXXXXXXXXXXXXXXXXXX
        dr("Operazione") = DirectCast(GetLocalResourceObject("VisitaPersonalizzata"), String) 'XXXXXXXXXXXXXXXXXXXXXXX
        dr("av_cod") = 0
        dr("udm_cod") = 0
        dr("ff_cod") = 0
        dr("Descrizione") = attivitaDes
        dr("Dato") = movDet.Mov_Det_Des
        dr("Dato_Testuale") = movDet.Mov_Det_Des

        dr("Cul_Des") = ImpiantoOttieniCulDes(DT_Impianti, movDest)
        dr("Qta2") = movDest.Qta2

        dr("Veg_Cod") = 0
        dr("ind_mat_cod") = 0
        dr("dr_cod") = 0

        dr("Descrizione_Unica") = If(objParametriAgenda_TEMP.Lav_Cod = LAVCOD_VISITA,
                                                dr("Operazione") & " - <i>" & dr("Impianto") & "</i> - " & dr("Descrizione"),
                                                dr("Impianto") & " - " & dr("Descrizione"))

        dt.Rows.Add(dr)

    End Sub

    Private Function RilievoAvversitaGeneraAnagrafica(ByVal Lista_Avversita As List(Of AgronicaCoreMetaSchemaBIZ.MisuraXAvv), ByVal rilievo As rilievoAvv) As String
        Return rilievo.Valore
    End Function

    Private Function RilievoAvversitaGeneraAnagrafica_DatoTestuale(ByVal Lista_Avversita As List(Of AgronicaCoreMetaSchemaBIZ.MisuraXAvv), ByVal rilievo As rilievoAvv) As String

        Dim mxa As AgronicaCoreMetaSchemaBIZ.MisuraXAvv = (From x As AgronicaCoreMetaSchemaBIZ.MisuraXAvv In Lista_Avversita Where x.Av_Cod = rilievo.Av_cod AndAlso x.Udm_Cod = rilievo.Udm_cod).First()

        Dim oLeggi As New AgronicaCoreMetaSchemaDAL.MisuraxAvversita_Anagrafiche_R
        Dim dt As DataTable = oLeggi.Leggi(mxa.Cod, rilievo.Av_cod, rilievo.Udm_cod, mxa.Veg_Cod, "", "", HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim datoTestuale As String = ""
        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            datoTestuale = (From dr As DataRow In dt.Rows Where dr("anag_valore") = rilievo.Valore Select dr("anag_des")).First()
        Else
            datoTestuale = RilievoAvversitaGeneraAnagrafica(Lista_Avversita, rilievo)
        End If

        Return datoTestuale
    End Function

    Private Function RilievoAvversitaGeneraDescrizione(ByVal Lista_Avversita As List(Of AgronicaCoreMetaSchemaBIZ.MisuraXAvv), ByVal rilievo As rilievoAvv) As String

        Dim rval As String = (From aa In Lista_Avversita
                              Where aa.Udm_Cod = rilievo.Udm_cod _
                                AndAlso aa.Av_Cod = rilievo.Av_cod
                              Select aa.Av_Des_Vol & " (" & aa.Udm_Des & ")"
                                    ).FirstOrDefault

        Return rval
    End Function

    Private Function RilievoIndiceMaturitaGeneraAnagrafica(ByVal Lista_IndiciMaturita As List(Of AgronicaCoreMetaSchemaBIZ.IndiciMaturita), ByVal rilievo As rilievoAvv) As String
        Return If(rilievo.Valore = "0", "", rilievo.Valore)
    End Function

    Private Function RilievoIndiceMaturitaGeneraAnagrafica_DatoTestuale(ByVal Lista_IndiciMaturita As List(Of AgronicaCoreMetaSchemaBIZ.IndiciMaturita), ByVal rilievo As rilievoAvv) As String

        Dim mxa As AgronicaCoreMetaSchemaBIZ.IndiciMaturita = (From x As AgronicaCoreMetaSchemaBIZ.IndiciMaturita In Lista_IndiciMaturita Where x.ind_mat_cod = rilievo.Av_cod AndAlso x.udm_cod = rilievo.Udm_cod).First()

        Dim datoTestuale As String = RilievoIndiceMaturitaGeneraAnagrafica(Lista_IndiciMaturita, rilievo)

        Return datoTestuale
    End Function

    Private Function RilievoIndiceMaturitaGeneraDescrizione(ByVal Lista_IndiciMaturita As List(Of AgronicaCoreMetaSchemaBIZ.IndiciMaturita), ByVal rilievo As rilievoAvv) As String

        Dim rval As String = (From aa In Lista_IndiciMaturita
                              Where aa.udm_cod = rilievo.Udm_cod _
                                AndAlso aa.ind_mat_cod = rilievo.Av_cod
                              Select aa.ind_mat_des & " (" & aa.udm_des & ")"
                                    ).FirstOrDefault

        Return rval
    End Function

    Private Function RilievoDanniRaccoltaGeneraAnagrafica(ByVal Lista_DanniRaccolta As List(Of AgronicaCoreMetaSchemaBIZ.MisuraXDR), ByVal rilievo As rilievoAvv) As String
        Return If(rilievo.Valore = "0", "", rilievo.Valore)
    End Function

    Private Function RilievoDanniRaccoltaGeneraAnagrafica_DatoTestuale(ByVal Lista_DanniRaccolta As List(Of AgronicaCoreMetaSchemaBIZ.MisuraXDR), ByVal rilievo As rilievoAvv) As String

        'Dim mxa As AgronicaCoreMetaSchemaBIZ.MisuraXDanniRaccolta = (From x As AgronicaCoreMetaSchemaBIZ.MisuraXDanniRaccolta In Lista_DanniRaccolta Where x.dr_cod = rilievo.Av_cod AndAlso x.udm_cod = rilievo.Udm_cod).First()

        Dim datoTestuale As String = RilievoDanniRaccoltaGeneraAnagrafica(Lista_DanniRaccolta, rilievo)

        Return datoTestuale
    End Function

    Private Function RilievoDanniRaccoltaGeneraDescrizione(ByVal Lista_DanniRaccolta As List(Of AgronicaCoreMetaSchemaBIZ.MisuraXDR), ByVal rilievo As rilievoAvv) As String

        Dim rval As String = (From aa In Lista_DanniRaccolta
                              Where aa.Udm_Cod = rilievo.Udm_cod _
                                AndAlso aa.Dr_Cod = rilievo.Av_cod
                              Select aa.Dr_Des & " (" & aa.Udm_Des & ")"
                                    ).FirstOrDefault

        Return rval
    End Function

    Private Function RilievoFasiFenologicheGeneraDescrizione(ByVal Lista_FasiFenologiche As List(Of AgronicaCoreMetaSchemaBIZ.FaseFenologica), ByVal rilievo As rilievoAvv) As String

        Dim rval As String

        Select Case rilievo.Av_cod

            Case < 1000 'caso vecchio av_cod = ff_cod
                rval = (From aa In Lista_FasiFenologiche
                        Where aa.FF_Cod = rilievo.Av_cod
                        Select aa.Descrizione
                            ).FirstOrDefault

            Case Else 'caso nuovo av_cod = cod_css
                rval = (From aa In Lista_FasiFenologiche
                        Where aa.Cod_SS = rilievo.Av_cod
                        Select aa.Descrizione & " ( BBCH " & aa.Stadio & " )"
                            ).FirstOrDefault
        End Select

        Return rval

    End Function

    Private Function RilievoFasiFenologicheGeneraAnagrafica(ByVal Lista_FasiFenologiche As List(Of AgronicaCoreMetaSchemaBIZ.FaseFenologica), ByVal rilievo As rilievoAvv) As String
        Return If(rilievo.Valore = "01/01/1900", "", rilievo.Valore)
    End Function

    Private Function RilievoFasiFenologicheGeneraAnagrafica_DatoTestuale(ByVal Lista_FasiFenologiche As List(Of AgronicaCoreMetaSchemaBIZ.FaseFenologica), ByVal rilievo As rilievoAvv) As String

        Dim datoTestuale As String = RilievoFasiFenologicheGeneraAnagrafica(Lista_FasiFenologiche, rilievo)

        Return datoTestuale

    End Function

    Private Function ImpiantoGeneraDescrizione(DT_impianti As DataTable, ByVal rilievo As rilievoAvv) As String

        Dim rval As String = (
            From ii In DT_impianti.AsEnumerable
            Where ii("piva") = rilievo.Impianto.Piva _
                And ii("Sa_Cod") = rilievo.Impianto.Sa_Cod _
                And ii("Appezza") = rilievo.Impianto.Appezza _
                And ii("id_reg") = rilievo.Impianto.ID_Reg
            Select String.Join(" - ", {ii("Sa_Nome"), ii("Campo_Des"), ii("App_Nome"), ii("Cul_Des")}.Where(Function(s) Not String.IsNullOrEmpty(s)))
            ).FirstOrDefault
        'Select ii("Sa_Nome") & " - " & ii("Campo_Des") & " - " & ii("App_Nome") & " - " & ii("Cul_Des")
        Return rval

    End Function

    Private Function ImpiantoGeneraDescrizione(DT_impianti As DataTable, ByVal movDest As Movimento_Destinazione) As String

        Dim rval As String = ""

        If movDest.Appezza <> 0 AndAlso movDest.Id_Destinazione <> 0 Then
            rval = (
                From ii In DT_impianti.AsEnumerable
                Where ii("piva") = movDest.Piva _
                    And ii("Sa_Cod") = movDest.Sa_Cod _
                    And ii("Appezza") = movDest.Appezza _
                    And ii("id_reg") = movDest.Id_Destinazione
                Select String.Join(" - ", {ii("Sa_Nome"), ii("Campo_Des"), ii("App_Nome"), ii("Cul_Des")}.Where(Function(s) Not String.IsNullOrEmpty(s)))
                ).FirstOrDefault
            'Select ii("Sa_Nome") & " - " & ii("Campo_Des") & " - " & ii("App_Nome") & " - " & ii("Cul_Des")
        Else
            If movDest.Sa_Cod = 0 Then
                rval = DirectCast(GetLocalResourceObject("VisitaSuImpresaWrapped"), String)
            Else
                Dim sa_nome As String = (From ii In DT_impianti.AsEnumerable Where ii("piva") = movDest.Piva And ii("Sa_Cod") = movDest.Sa_Cod Select ii("Sa_Nome")).FirstOrDefault
                rval = String.Format(DirectCast(GetLocalResourceObject("VisitaSuCentroPlaceholder"), String), sa_nome)
            End If

        End If


        Return rval

    End Function

    Private Function ImpiantoOttieniCulDes(DT_impianti As DataTable, ByVal rilievo As rilievoAvv) As String

        Dim rval As String = (
            From ii In DT_impianti.AsEnumerable
            Where ii("piva") = rilievo.Impianto.Piva _
                And ii("Sa_Cod") = rilievo.Impianto.Sa_Cod _
                And ii("Appezza") = rilievo.Impianto.Appezza _
                And ii("id_reg") = rilievo.Impianto.ID_Reg
            Select ii("Cul_Des")
            ).FirstOrDefault

        Return rval

    End Function

    Private Function ImpiantoOttieniCulDes(DT_impianti As DataTable, ByVal movDest As Movimento_Destinazione) As String

        Dim rval As String = (
            From ii In DT_impianti.AsEnumerable
            Where ii("piva") = movDest.Piva _
                And ii("Sa_Cod") = movDest.Sa_Cod _
                And ii("Appezza") = movDest.Appezza _
                And ii("id_reg") = movDest.Id_Destinazione
            Select ii("Cul_Des")
            ).FirstOrDefault

        Return rval

    End Function

    Private Shared Sub GeneraTabellaStrutturaColonne(Dt As DataTable)

        Dt.Columns.Add(New DataColumn("Codice", GetType(String)))
        Dt.Columns.Add(New DataColumn("Impianto", GetType(String)))
        Dt.Columns.Add(New DataColumn("piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("sa_cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("appezza", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("id_reg", GetType(Integer)))

        Dt.Columns.Add(New DataColumn("Data", GetType(DateTime)))

        Dt.Columns.Add(New DataColumn("lav_cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Operazione", GetType(String)))

        Dt.Columns.Add(New DataColumn("av_cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("udm_cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("ff_cod", GetType(Integer)))

        Dt.Columns.Add(New DataColumn("Descrizione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dato", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dato_Testuale", GetType(String)))

        Dt.Columns.Add(New DataColumn("Cul_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Qta2", GetType(Decimal)))

        Dt.Columns.Add(New DataColumn("Veg_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("ind_mat_cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("dr_cod", GetType(Integer)))

        Dt.Columns.Add(New DataColumn("Descrizione_Unica", GetType(String)))
    End Sub

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaTabella_Kendo(ByVal param As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            r.RispostaOK = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim Dt_Tabella As DataTable = HttpContext.Current.Session("Tabella")

            If Dt_Tabella Is Nothing Then
                Dt_Tabella = New DataTable
                GeneraTabellaStrutturaColonne(Dt_Tabella)
            End If

            r.RispostaStringa = JSON_Datatable_Tabella(Dt_Tabella)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Private Shared Function JSON_Datatable_Tabella(ByVal dt As DataTable) As String

        Dim objParametriAgenda_TEMP As New ParametriAgenda

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        l.Add(New ColonneNome("Codice", "Codice", "string") With {._Display = False})
        l.Add(New ColonneNome("lav_cod", "lav_cod", "string") With {._hidden = True})
        l.Add(New ColonneNome("Operazione", "Operazione", "string") With {._Display = If(objParametriAgenda_TEMP.Lav_Cod = LAVCOD_VISITA, True, False)})
        l.Add(New ColonneNome("Impianto", AgronicaAgenda_2010.Impianto, "string"))
        l.Add(New ColonneNome("Descrizione_Unica", AgronicaAgenda_2010.DescrizioneUnica, "string") With {._Display = False, ._RemoveHtmlEncode = True})

        'chiave impianto
        l.Add(New ColonneNome("piva", "piva", "string") With {._hidden = True})
        l.Add(New ColonneNome("sa_cod", "sa_cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("appezza", "appezza", "number") With {._hidden = True})
        l.Add(New ColonneNome("id_reg", "id_reg", "number") With {._hidden = True})
        l.Add(New ColonneNome("Data", "Data", "date") With {._Display = False})

        'codici dei diversi rilievi
        l.Add(New ColonneNome("av_cod", "av_cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("udm_cod", "udm_cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("ff_cod", "ff_cod", "number") With {._hidden = True})

        l.Add(New ColonneNome("Descrizione", AgronicaAgenda_2010.Descrizione, "string"))
        l.Add(New ColonneNome("Dato", "Dato Rilevato", "string") With {._hidden = True, ._Editabile = True})
        'l.Add(New ColonneNome("Dato_Testuale", "Dato Rilevato", "string") With {._FormatoParticolare = "<input type='text' class='k-input k-textbox valid' name='Dato_Testuale' data-bind='value:Dato_Testuale' aria-invalid='false' value='#=Dato#' style='width:100%' onchange='aggiornaDatoReale(this);'/>"})
        l.Add(New ColonneNome("Dato_Testuale", "Dato_Testuale", "string") With {._hidden = True, ._Editabile = True})

        l.Add(New ColonneNome("Cul_Des", "Cul_Des", "string") With {._hidden = True})
        l.Add(New ColonneNome("Qta2", "Qta2", "number") With {._hidden = True})
        l.Add(New ColonneNome("Veg_Cod", "Veg_Cod", "number") With {._hidden = True})

        l.Add(New ColonneNome("ind_mat_cod", "ind_mat_cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("dr_cod", "dr_cod", "number") With {._hidden = True})

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable

        Return js.JSON_DataTable_Kendo(dt, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaDiscesa)

    End Function

#End Region


    Public Sub Ripristina_Dati_nei_Controlli_xRicetta(Xml_Operazione As String) Implements iOperazioneGUI.Ripristina_Dati_nei_Controlli_xRicetta
        Throw New NotImplementedException()
    End Sub

    Public Sub Ripristina_Dati_nei_ControlliDaRicetta() Implements iOperazioneGUI.Ripristina_Dati_nei_ControlliDaRicetta
        Throw New NotImplementedException()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        '----- Verifico che l'utente sia autenticato
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If

        Me.Master.Master.jQuery_versione = "1.12.3"


        If Request.QueryString("trattamento") <> "" OrElse Request.QueryString("gis") <> "" Then
            Me.Master.Master.flag_MostraHeader = False
            Me.Master.Master.flag_MostraFooter = False
        End If

        '---
        objParametri_Super_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Super_Server"))
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        '---

        pivaSuperUser = objParametri_Server.PivaSuperUser

        objParametriAgenda = New ParametriAgenda
        TipoOperazioneAgenda = objParametriAgenda.TipoOperazioneAgenda
        Id_Agenda_Old = objParametriAgenda.Id_Agenda

        hdLav_Cod.Value = objParametriAgenda.Lav_Cod
        hdTipoOperazione.Value = objParametriAgenda.Tipo_Operazione

        'da rivedere...
        Dim objOperazioniLeggi As New AgronicaCoreMetaSchemaDAL.Operazioni_R
        Dim Lav_Des As String = objOperazioniLeggi.LavorazioneDes_from_LavorazioneCod(objParametriAgenda.Lav_Cod, objParametri_Server)
        objParametriAgenda.Lav_Des = Lav_Des

        Dim strtipoOperaz As String = ""
        Select Case objParametriAgenda.Tipo_Operazione
            Case enum_TipoOperazioneDB.Lettura
                strtipoOperaz = "Info <br>"
            Case enum_TipoOperazioneDB.Modifica
                strtipoOperaz = "Modifica <br>"
            Case enum_TipoOperazioneDB.Scrittura
                strtipoOperaz = "Nuovo <br>"
        End Select

        Master_Operazione.Master.Lbl_Titolo.Text = strtipoOperaz & Lav_Des
        Master_Operazione.Property_Div_ProvenienzaRisorse.Visible = False
        Master_Operazione.Property_ImgBtn_CheckDPI.Visible = False

        Master.Master.Master_versione = VERSIONE_MASTER_DEFAULT
        'Master.Master.Header_versione = VERSIONE_HEADER_DEFAULT 'Commentato: DomandaIrriguaBootstrap non ha Header_versione

        'valori copiati da agenda vecchia
        Movimento_Dettaglio_Pendente = 3
        Movimento_Dettaglio_Extra_Date = AGRODATAINIZIO
        Movimento_Dettaglio_Anno = 1900

        Select Case CInt(objParametriAgenda.Lav_Cod)
            Case LAVCOD_RILIEVO_ERBE_INFESTANTI, LAVCOD_FASI_FENOLOGICHE, LAVCOD_RILIEVO_AVVERSITA_CAMPO
                objParametriAgenda.Cau_Mov = enum_Agenda_Causali.RILIEVO_CAMPO
            Case LAVCOD_RILIEVO_INDICI_MATURITA, LAVCOD_DANNI_RACCOLTA, LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA
                objParametriAgenda.Cau_Mov = enum_Agenda_Causali.RILIEVO_RACCOLTA
            Case LAVCOD_VISITA
                objParametriAgenda.Cau_Mov = CAU_VISITE_ISPETTIVE
        End Select

        If objParametriAgenda.Rilievi.Count > 0 AndAlso objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Trattamenti Then
            Master_Operazione.caricaDefaultUtente_Disciplinare = False
            Master_Operazione.caricaDefaultAziendale_Disciplinare = False
            objParametriAgenda.Id_Agenda = 0
        End If

        If objParametriAgenda.Lav_Cod = LAVCOD_VISITA Then
            Master_Operazione.AttivaOrarioInDataMovimento = True
            Master_Operazione.flag_MostraBtnSalvaCDG = False
        End If

        If Not IsPostBack Then

            Session("Tabella") = Nothing
            VerificaPermessi()
            LeggiImpostazioni()
            Ripristina_Dati_nei_Controlli()

            Master_Operazione.Property_divPosizione.Visible = True

            Select Case objParametriAgenda.Lav_Cod
                Case LAVCOD_VISITA
                    Master_Operazione.Property_divImpresa.Visible = True
                    Master_Operazione.Property_divUsernameCreazione.Visible = True

                    panelBarDatiGenerali.Attributes.Remove("style")

                Case LAVCOD_RILIEVO_AVVERSITA_CAMPO
                    panelBarAvversitaInCampo.Attributes.Remove("style")
                Case LAVCOD_RILIEVO_INDICI_MATURITA
                    panelBarIndiciMaturita.Attributes.Remove("style")
                Case LAVCOD_DANNI_RACCOLTA
                    panelBarDanniRaccolta.Attributes.Remove("style")
                Case LAVCOD_FASI_FENOLOGICHE
                    panelBarFasiFenologiche.Attributes.Remove("style")


                Case LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA
                    panelBarIndiciReseRaccolta.Attributes.Remove("style")
                Case LAVCOD_RILIEVO_ERBE_INFESTANTI
                    panelBarErbeInfestanti.Attributes.Remove("style")
            End Select


            'Carico la combo delle operazioni d'agenda in testa alla pagina
            'Master.Property_ComboOperazione.Tipo_GruppoOperazioni = "'C','V'"
            Dim filtro As String = String.Join(",", LAVCOD_RILIEVO_AVVERSITA_CAMPO, LAVCOD_FASI_FENOLOGICHE, LAVCOD_RILIEVO_INDICI_MATURITA, LAVCOD_DANNI_RACCOLTA, LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA, LAVCOD_VISITA)
            Master.Property_ComboOperazione.Tipo_GruppoOperazioni = "'C','V') AND Operazioni.Lav_Cod IN (" & filtro
            Master.Property_ComboOperazione.CaricaComboLavorazioni(True, "")

            If objParametriAgenda.Lav_Cod <> LAVCOD_RILIEVO_AVVERSITA_CAMPO AndAlso objParametriAgenda.Lav_Cod <> LAVCOD_VISITA Then
                Master_Operazione.UpdatePanelDisciplinareMaster.Visible = False
            Else
                Master_Operazione.UpdatePanelDisciplinareMaster.Visible = True
                Master_Operazione.Property_divDisciplinare.CssClass = Master_Operazione.Property_divDisciplinare.CssClass.Replace("nopadding", "")
            End If

            If Not IsNothing(Request.QueryString("Ifr")) Then
                _aperturadaIFrame = CInt(Stringa_Decodifica(Request.QueryString("Ifr").ToString, AgroKey_EncoderDecoder))
            End If

            Select Case _aperturadaIFrame
                Case 1
                    Me.Master.Master.flag_MostraHeader = False
                    Me.Master.Master.flag_MostraFooter = False
            End Select

        End If

    End Sub

    Private Sub LeggiImpostazioni()

        Dim Leggi_impostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        Session("Personalizzate_Fasi") = False
        Session("Personalizzate_Infestanti") = False
        Session("Personalizzate_Avversita") = False
        Session("Personalizzati_Indici_Maturita") = False
        Session("Personalizzati_Indici_Rese_Raccolta") = False
        Session("Personalizzati_Danni_Raccolta") = False

        Select Case objParametriAgenda.Lav_Cod

            Case LAVCOD_RILIEVO_ERBE_INFESTANTI

                If Not String.IsNullOrEmpty(Session("Personalizzate_Infestanti")) Then

                    Dim imp As String = Leggi_impostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_ERBE_INFESTANTI, objParametri_Utenti, 2)

                    If imp = "1" Then
                        Session("Personalizzate_Infestanti") = True
                    End If

                End If

            Case LAVCOD_FASI_FENOLOGICHE

                If Not String.IsNullOrEmpty(Session("Personalizzate_Fasi")) Then

                    Dim imp As String = Leggi_impostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_FASI_FENOLOGICHE, objParametri_Utenti, 2)

                    If imp = "1" Then
                        Session("Personalizzate_Fasi") = True
                    End If

                End If

            Case LAVCOD_RILIEVO_AVVERSITA_CAMPO

                If Not String.IsNullOrEmpty(Session("Personalizzate_Avversita")) Then

                    Dim imp As String = Leggi_impostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_AVVERSITA, objParametri_Utenti, 2)

                    If imp = "1" Then
                        Session("Personalizzate_Avversita") = True
                    End If

                End If

            Case LAVCOD_RILIEVO_INDICI_MATURITA

                If Not String.IsNullOrEmpty(Session("Personalizzati_Indici_Maturita")) Then

                    Dim imp As String = Leggi_impostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_INDICI_MATURITA, objParametri_Utenti, 2)

                    If imp = "1" Then
                        Session("Personalizzati_Indici_Maturita") = True
                    End If

                End If

            Case LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA

                If Not String.IsNullOrEmpty(Session("Personalizzati_Indici_Rese_Raccolta")) Then

                    Dim imp As String = Leggi_impostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_INDICI_RESE_RACCOLTA, objParametri_Utenti, 2)

                    If imp = "1" Then
                        Session("Personalizzati_Indici_Rese_Raccolta") = True
                    End If

                End If

            Case LAVCOD_DANNI_RACCOLTA

                If Not String.IsNullOrEmpty(Session("Personalizzati_Danni_Raccolta")) Then

                    Dim imp As String = Leggi_impostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_DANNI_ALLA_RACCOLTA, objParametri_Utenti, 2)

                    If imp = "1" Then
                        Session("Personalizzati_Danni_Raccolta") = True
                    End If

                End If
        End Select


    End Sub



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

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaComboMisuraXAvversitaInputData(ByVal MxAV_Cod As String, ByVal av_cod As String, ByVal udm_cod As String, ByVal veg_cod As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim oLeggi As New AgronicaCoreMetaSchemaDAL.MisuraxAvversita_Anagrafiche_R
        Dim Dt As DataTable = oLeggi.Leggi(MxAV_Cod, av_cod, udm_cod, veg_cod, "", "", HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim listaJSON As New JArray()

        If Not IsNothing(Dt) Then
            For Each dr As DataRow In Dt.Rows
                listaJSON.Add(New JObject(New JProperty("anag_valore", dr("anag_valore")), New JProperty("anag_des", dr("anag_des"))))
            Next
        End If

        r.RispostaStringa = JsonConvert.SerializeObject(listaJSON, Formatting.None)
        r.RispostaOK = True

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaComboMisuraXIndiciMaturitaInputData(ByVal Ind_Mat_Cod As String, ByVal udm_cod As String, ByVal veg_cod As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim oLeggi As New AgronicaCoreMetaSchemaDAL.MisuraxIndiciMaturita_Anagrafiche_R
        Dim Dt As DataTable = oLeggi.Leggi(Ind_Mat_Cod, udm_cod, veg_cod, "", "", HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim listaJSON As New JArray()

        If Not IsNothing(Dt) Then
            For Each dr As DataRow In Dt.Rows
                listaJSON.Add(New JObject(New JProperty("anag_valore", dr("anag_valore")), New JProperty("anag_des", dr("anag_des"))))
            Next
        End If

        r.RispostaStringa = JsonConvert.SerializeObject(listaJSON, Formatting.None)
        r.RispostaOK = True

        Return r

    End Function

    'Private Function LeggiGrigliaRilievi(sa_cod As Integer, Optional lav_cod As String = "", Optional veg_cod As String = "") As JArray
    Private Function LeggiGrigliaRilievi(sa_cod As Integer, Optional lav_cod As String = "", Optional ByVal ff_cod As Integer = 0, Optional ByVal Dato As String = "") As JArray

        Dim righeModificateArray As JArray = estraiJSONgrigliaRilievi()

        'Estraggo solo le righe per il sa_cod specifico
        Dim righeModificateArrayPostFiltri As New JArray

        For Each cRow In righeModificateArray
            If cRow("sa_cod") = sa_cod.ToString AndAlso (lav_cod = "" OrElse cRow("lav_cod") = lav_cod) AndAlso (ff_cod = 0 OrElse cRow("ff_cod") = ff_cod.ToString) AndAlso (Dato = "" OrElse cRow("Dato") = Dato) Then
                righeModificateArrayPostFiltri.Add(cRow)
            End If
        Next

        Return righeModificateArrayPostFiltri

    End Function

    Private Function LeggiAvversitaDaWS() As AgronicaCoreMetaSchemaBIZ.MisuraXAvversita_output

        Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.MisuraXAvversita_input()
        objParametriIngresso.Veg_Cod = CInt(objParametriAgenda.Veg_Cod.Split("/")(0))

        Dim vetDiscplinare As String() = objParametriAgenda.Disciplinare.Split("/")
        If vetDiscplinare.Count >= 5 Then
            objParametriIngresso.Dpi_Cod = CInt(objParametriAgenda.Disciplinare.Split("/")(0))
            objParametriIngresso.Id_Rcdpi = CInt(objParametriAgenda.Disciplinare.Split("/")(1))
            objParametriIngresso.Dpi_Pubblico_Privato = CInt(objParametriAgenda.Disciplinare.Split("/")(4))
        End If

        Dim leggiPath As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim url As String = leggiPath.Leggi_Valore(0, "GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali", "", "", objParametri_Super_Server)

        If Not String.IsNullOrEmpty(Session("Personalizzate_Avversita")) AndAlso Session("Personalizzate_Avversita") = True Then
            objParametriIngresso.Personalizzate = True
            objParametriIngresso.Piva_Superuser = objParametri_Server.PivaSuperUser
        End If

        objParametriIngresso.Url = url & "/MisureXAvversita"

        Dim objWS As New AgronicaCoreWebService.MisureXAvversita_WS
        Dim objParametriUscita As AgronicaCoreMetaSchemaBIZ.MisuraXAvversita_output = objWS.MisureXAvversita(objParametriIngresso)

        If objParametriUscita.MessaggioErrore <> "" Then
            Throw New Exception(objParametriUscita.MessaggioErrore)
        End If

        Return objParametriUscita

    End Function

    Private Function LeggiIndiciMaturitaDaWS() As AgronicaCoreMetaSchemaBIZ.IndiciMaturita_output

        Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.IndiciMaturita_input()
        objParametriIngresso.veg_cod = CInt(objParametriAgenda.Veg_Cod.Split("/")(0))

        Dim leggiPath As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim url As String = leggiPath.Leggi_Valore(0, "GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali", "", "", objParametri_Super_Server)

        If Not String.IsNullOrEmpty(Session("Personalizzati_Indici_Maturita")) AndAlso Session("Personalizzati_Indici_Maturita") = True Then
            objParametriIngresso.Personalizzate = True
            objParametriIngresso.Piva_Superuser = objParametri_Server.PivaSuperUser
        End If

        objParametriIngresso.Url = url & "/IndiciMaturita"
        objParametriIngresso.Lingua_Cod = objParametri_Server.Lingua_Cod

        Dim objWS As New AgronicaCoreWebService.IndiciMaturita_WS
        Dim objParametriUscita As AgronicaCoreMetaSchemaBIZ.IndiciMaturita_output = objWS.IndiciMaturita(objParametriIngresso)

        If objParametriUscita.MessaggioErrore <> "" Then
            Throw New Exception(objParametriUscita.MessaggioErrore)
        End If

        Return objParametriUscita

    End Function

    Private Function LeggiDanniRaccoltaDaWS() As AgronicaCoreMetaSchemaBIZ.MisuraXDanniRaccolta_output

        Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.MisuraXDanniRaccolta_input()
        objParametriIngresso.Veg_Cod = CInt(objParametriAgenda.Veg_Cod.Split("/")(0))

        Dim leggiPath As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim url As String = leggiPath.Leggi_Valore(0, "GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali", "", "", objParametri_Super_Server)

        If Not String.IsNullOrEmpty(Session("Personalizzati_Danni_Raccolta")) AndAlso Session("Personalizzati_Danni_Raccolta") = True Then
            objParametriIngresso.Personalizzate = True
            objParametriIngresso.Piva_Superuser = objParametri_Server.PivaSuperUser
        End If

        objParametriIngresso.Url = url & "/MisureXDanniRaccolta"

        Dim objWS As New AgronicaCoreWebService.MisureXDanniRaccolta_WS
        Dim objParametriUscita As AgronicaCoreMetaSchemaBIZ.MisuraXDanniRaccolta_output = objWS.MisureXDanniRaccolta(objParametriIngresso)

        If objParametriUscita.MessaggioErrore <> "" Then
            Throw New Exception(objParametriUscita.MessaggioErrore)
        End If

        Return objParametriUscita

    End Function

    Private Function LeggiFasiFenologicheDaWS() As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output

        Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.FasiFenologiche_input
        objParametriIngresso.Veg_Cod = CInt(objParametriAgenda.Veg_Cod.Split("/")(0))

        Dim leggiPath As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim url As String = leggiPath.Leggi_Valore(0, "GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali", "", "", objParametri_Super_Server)

        If Not String.IsNullOrEmpty(Session("Personalizzate_Fasi")) AndAlso Session("Personalizzate_Fasi") = True Then
            objParametriIngresso.Personalizzate = True
            objParametriIngresso.Piva_Superuser = objParametri_Server.PivaSuperUser
        End If
        objParametriIngresso.Lingua_Cod = objParametri_Server.Lingua_Cod

        objParametriIngresso.Url = url & "/FasiFenologiche"

        Dim objWS As New AgronicaCoreWebService.FasiFenologiche_WS
        Dim objParametriUscita As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output = objWS.FasiFenologiche(objParametriIngresso)

        If objParametriUscita.MessaggioErrore <> "" Then
            Throw New Exception(objParametriUscita.MessaggioErrore)
        End If

        Return objParametriUscita

    End Function
End Class