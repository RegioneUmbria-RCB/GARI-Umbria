Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreXML.XML_Stampe
Imports System.Web
Imports System.Xml
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreModelsSTD.exceptions

Public Class RedirectGestione_Stampe

    Public Shared Sub GestioneRedirectStampe(ByVal Report As enum_CodificaStampe, ByVal Piva As String,
                                             ByRef RedirectURL As String,
                                             Optional ByVal ParametriAggiuntivi As List(Of ParametriAggiuntivi_QueryString) = Nothing,
                                             Optional ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = Nothing,
                                             Optional ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = Nothing)

        Select Case Report

            Case enum_CodificaStampe.PianoColturaleCatasto
                RedirectURL = "../Statistiche/Anagrafica/InvestimentoCatasto.aspx"

            Case enum_CodificaStampe.ReportRisultatoFilrone
                Dim objp As New AgronicaCoreGestioneRichieste.ParametriFILTRONE_2010
                objp.Sito_Origine = Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, AgroKey_EncoderDecoder, Nothing)
                objp.Pagina_Origine = Stringa_Codifica("../Stampe/Menu_Stampe.aspx", AgroKey_EncoderDecoder, Nothing)
                objp.Sito_Destinazione = Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaStampe_2010, AgroKey_EncoderDecoder, Nothing)
                objp.Pagina_Destinazione = Stringa_Codifica("../GestioneStampe/ChiamaStampe.aspx", AgroKey_EncoderDecoder, Nothing)
                objp.TipoFiltrone = Stringa_Codifica(CStr(enum_TipoFiltrone.Stampa), AgroKey_EncoderDecoder, Nothing)
                objp.CodificaStampe = Stringa_Codifica(Report, AgroKey_EncoderDecoder, Nothing)

                RedirectURL = "../Filtrone/Filtrone_Nuovo.aspx?p_o=" & objp.Pagina_Origine &
                           "&s_o=" & objp.Sito_Origine &
                           "&p_d=" & objp.Pagina_Destinazione &
                           "&s_d=" & objp.Sito_Destinazione &
                           "&t_f=" & objp.TipoFiltrone &
                           "&c_s=" & objp.CodificaStampe &
                           "&v_c=" & objp.Veg_Cod &
                           "&c_c=" & objp.Cul_Cod &
                           "&d_i=" & objp.Data_Inizio &
                           "&d_f=" & objp.Data_Fine &
                           "&nopiva=1"

            Case enum_CodificaStampe.PianoColturale,
                 enum_CodificaStampe.ReportRisultatoFiltroneG2G,
                 enum_CodificaStampe.ReportRisultatoFiltroneIncludiVisita

                Dim objp As New AgronicaCoreGestioneRichieste.ParametriFILTRONE_2010
                objp.Sito_Origine = Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, AgroKey_EncoderDecoder, Nothing)
                objp.Pagina_Origine = Stringa_Codifica("../Stampe/Menu_Stampe.aspx", AgroKey_EncoderDecoder, Nothing)
                objp.Sito_Destinazione = Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaStampe_2010, AgroKey_EncoderDecoder, Nothing)
                objp.Pagina_Destinazione = Stringa_Codifica("../GestioneStampe/ChiamaStampe.aspx", AgroKey_EncoderDecoder, Nothing)
                objp.TipoFiltrone = Stringa_Codifica(CStr(enum_TipoFiltrone.Stampa), AgroKey_EncoderDecoder, Nothing)
                objp.CodificaStampe = Stringa_Codifica(Report, AgroKey_EncoderDecoder, Nothing)

                RedirectURL = "../Filtrone/Filtrone_Nuovo.aspx?p_o=" & objp.Pagina_Origine &
                           "&s_o=" & objp.Sito_Origine &
                           "&p_d=" & objp.Pagina_Destinazione &
                           "&s_d=" & objp.Sito_Destinazione &
                           "&t_f=" & objp.TipoFiltrone &
                           "&c_s=" & objp.CodificaStampe &
                           "&v_c=" & objp.Veg_Cod &
                           "&c_c=" & objp.Cul_Cod &
                           "&d_i=" & objp.Data_Inizio &
                           "&d_f=" & objp.Data_Fine

                ''questo sarà da aggiungere (anche in if che contiene l'istruzione quando lo si rilascia 
                ''come comportamento per il pulsante  "statistiche piano colturale catasto"
                'If Report = enum_CodificaStampe.PianoColturaleCatasto Then
                '    RedirectURL &= "&PCC=1"
                'End If

                If Report = enum_CodificaStampe.ReportRisultatoFiltroneG2G Then
                    RedirectURL &= "&g2g=1"
                End If

                If Report = enum_CodificaStampe.ReportRisultatoFiltroneIncludiVisita Then
                    RedirectURL &= "&IncludiVisite=true"
                End If

            Case enum_CodificaStampe.Conf_FiltroStampe,
                 enum_CodificaStampe.Filtro_StampeBiologico,
                 enum_CodificaStampe.Lista_InsolutiClienti,
                 enum_CodificaStampe.Lista_InsolutiFornitori,
                 enum_CodificaStampe.EstrazioneCatastoAffitti,
                 enum_CodificaStampe.Statistometro

                Dim vVarStampe(0) As ElementoStampe
                vVarStampe(0).Nome = "piva"
                vVarStampe(0).Valore = Piva

                Dim objVS As New AgronicaCoreXML.XML_Stampe
                Dim StrNodo As String = objVS.XML_VariabiliStampe(vVarStampe)
                Dim StrNodiVariabili As String = StrNodo

                Dim objAgronicaStampe As New AgronicaCoreGestioneRichieste.ParametriAgronicaStampe
                objAgronicaStampe.report = Report
                objAgronicaStampe.username = CStr(HttpContext.Current.Session("ASG_Utente_Username"))
                objAgronicaStampe.user_profilo = CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS"))
                objAgronicaStampe.Xml_Generico.Length = 0
                objAgronicaStampe.Xml_Generico.Append(StrNodiVariabili)
                RedirectURL = RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(Enum_SiteRedirector.Sito_AgronicaStampe_2010, objAgronicaStampe)
                RedirectURL &= "?p=" & Stringa_Codifica(Piva, AgroKey_EncoderDecoder, HttpContext.Current.Session)

            case enum_CodificaStampe.RisultatoAnalisiConformita
                Dim this As New RedirectGestione_Stampe
                this.HandlePrint(RedirectURL, Piva, ParametriAggiuntivi)

            Case Else
                Dim this As New RedirectGestione_Stampe
                this.gestisciAltraStampa(Report, Piva, RedirectURL, ParametriAggiuntivi, objParametri_Server, objParametri_Utenti)


        End Select

    End Sub

    Private sub HandlePrint(byref RedirectURL As String, piva As string, ParametriAggiuntivi As List(Of ParametriAggiuntivi_QueryString))
        If IsNothing(ParametriAggiuntivi) Then
            ParametriAggiuntivi = New List(Of ParametriAggiuntivi_QueryString)
        End If
        Dim vVarStampe(6) As ElementoStampe
        vVarStampe(0).Nome = "piva"
        vVarStampe(0).Valore = piva
        vVarStampe(1).Nome = "veg_cod"
        vVarStampe(1).Valore = ParametriAggiuntivi.FirstOrDefault(Function(x) x.key = "veg_cod")?.value
        vVarStampe(2).Nome = "data_da"
        vVarStampe(2).Valore = ParametriAggiuntivi.FirstOrDefault(Function(x) x.key = "data_da")?.value
        vVarStampe(3).Nome = "data_a"
        vVarStampe(3).Valore = ParametriAggiuntivi.FirstOrDefault(Function(x) x.key = "data_a")?.value
        vVarStampe(4).Nome = "sa_cod"
        vVarStampe(4).Valore = ParametriAggiuntivi.FirstOrDefault(Function(x) x.key = "sa_cod")?.value
        vVarStampe(5).Nome = "sa_nome"
        vVarStampe(5).Valore = ParametriAggiuntivi.FirstOrDefault(Function(x) x.key = "sa_nome")?.value
        vVarStampe(6).Nome = "veg_des"
        vVarStampe(6).Valore = ParametriAggiuntivi.FirstOrDefault(Function(x) x.key = "veg_des")?.value
        
        Dim strKendoOperazioni = ParametriAggiuntivi.FirstOrDefault(Function(x) x.key = "strKendoOperazioni")?.value
        Dim strKendoDettagli = ParametriAggiuntivi.FirstOrDefault(Function(x) x.key = "strKendoDettagli")?.value
        Dim strKendoOperazioniMagazzino = ParametriAggiuntivi.FirstOrDefault(Function(x) x.key = "strKendoOperazioniMagazzino")?.value
        Dim strKendoDettagliMagazzino = ParametriAggiuntivi.FirstOrDefault(Function(x) x.key = "strKendoDettagliMagazzino")?.value
        Dim strKendoAppIAF = "[]"
        Dim strKendoAppIAFDettagli = "[]"
        Dim strKendoAppControlli = "[]"
        Dim strKendoAppControlliDettagli = "[]"

        Dim objVS As New AgronicaCoreXML.XML_Stampe
        Dim StrNodo As String = objVS.XML_VariabiliStampe(vVarStampe)
        Dim StrNodiVariabili As String = StrNodo

        Dim objAgronicaStampe As New AgronicaCoreGestioneRichieste.ParametriAgronicaStampe
        objAgronicaStampe.report = enum_CodificaStampe.RisultatoAnalisiConformita
        objAgronicaStampe.username = CStr(HttpContext.Current.Session("ASG_Utente_Username"))
        objAgronicaStampe.user_profilo = CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS"))
        objAgronicaStampe.Xml_Generico.Length = 0
        objAgronicaStampe.Xml_Generico.Append(StrNodiVariabili)
        objAgronicaStampe.JSon_Generico.Length = 0
        objAgronicaStampe.JSon_Generico.Append(
            strKendoOperazioni & "||||||||||" & strKendoDettagli & "||||||||||" &
            strKendoOperazioniMagazzino & "||||||||||" & strKendoDettagliMagazzino & "||||||||||" &
            strKendoAppIAF & "||||||||||" & strKendoAppIAFDettagli & "||||||||||" &
            strKendoAppControlli & "||||||||||" & strKendoAppControlliDettagli)

        Dim strJS As String = RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                        Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objAgronicaStampe)

        Dim strSplit As String() = strJS.Split(New Char() {"'"c}, StringSplitOptions.RemoveEmptyEntries)
        RedirectURL = strSplit(3)
    End sub

    Private Sub gestisciAltraStampa(ByVal Report As enum_CodificaStampe, ByVal Piva As String, ByRef RedirectURL As String,
                                    ByVal ParametriAggiuntivi As List(Of ParametriAggiuntivi_QueryString),
                                    ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

        If IsNothing(objParametri_Server) OrElse IsNothing(objParametri_Utenti) Then
            Exit Sub
        End If

        Dim Origine As String = Stringa_Codifica("../Stampe/MenuStampe.aspx", AgroKey_EncoderDecoder, objParametri_Server)
        Dim Destinazione As String = Stringa_Codifica("../GestioneStampe/ChiamaStampe.aspx", AgroKey_EncoderDecoder, objParametri_Server)
        Dim Funzione As String = Stringa_Codifica(CStr(enum_TipoFiltrone.Stampa), AgroKey_EncoderDecoder, objParametri_Server)

        Select Case Report
            Case enum_CodificaStampe.Bilancio_Fertilizzazioni, enum_CodificaStampe.Bilancio_Fertilizzazioni_Dettagliato
                redirectBilancioFertilizzazioni(Report, RedirectURL)

            Case enum_CodificaStampe.SchedaCampagna_Multicentro,
                 enum_CodificaStampe.SchedaCampagna_Multicentro_ACA,
                 enum_CodificaStampe.SchedaCampagna_2078_Semplificata,
                 enum_CodificaStampe.Eurep_Gap_Multicentro,
                 enum_CodificaStampe.Eurep_Gap_Semplificata,
                 enum_CodificaStampe.Registro_Fertilizzazioni,
                 enum_CodificaStampe.RegistroTrattamenti_Semplificata,
                 enum_CodificaStampe.RegistroTrattamenti_Veneto,
                 enum_CodificaStampe.SchedaCampagna_ProvAut_Trento,
                 enum_CodificaStampe.SchedaCampagna_Multi_Lombardia,
                 enum_CodificaStampe.SchedaInterventiAgronomici,
                 enum_CodificaStampe.SchedaRegistrazione_Semplificata,
                 enum_CodificaStampe.SchedaCampagna_ConserveItalia,
                 enum_CodificaStampe.RegistroAziendaleUnico,
                 enum_CodificaStampe.RegistroTrattamentiVeneto_StdCondizionalita,
                 enum_CodificaStampe.SchedaColturale_Biologico

                If IsNothing(ParametriAggiuntivi) Then
                    ParametriAggiuntivi = New List(Of ParametriAggiuntivi_QueryString)
                End If

                redirectDaMenuAgendaNoFiltrone(Report, RedirectURL, ParametriAggiuntivi, objParametri_Server)

            Case enum_CodificaStampe.SchedaMagazzinoGiacenze,
                 enum_CodificaStampe.SchedaMagazzinoMovimenti,
                 enum_CodificaStampe.SchedaMagazzinoFertilizzanti,
                 enum_CodificaStampe.SchedaMagazzinoProdottiFitosanitari,
                 enum_CodificaStampe.RiepilogoProdottiUtilizzati

                Dim ParametriAgronicaStampe As ParametriAgronicaStampe =
                            RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                            Report, Piva, HttpContext.Current.Session, objParametri_Server, "", 0, 0, 0, 0, 0)

                RedirectURL = RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                                 Enum_SiteRedirector.Sito_AgronicaAgenda_2010, ParametriAgronicaStampe)

            Case enum_CodificaStampe.SchedaMateriePrime_Biologico,
                 enum_CodificaStampe.SchedaVendite_Biologico,
                 enum_CodificaStampe.SchedaPreparati_Biologico

                Dim ParametriAgronicaStampe As ParametriAgronicaStampe =
                            RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                            Report, Piva, HttpContext.Current.Session, objParametri_Server)

                RedirectURL = RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                                 Enum_SiteRedirector.Sito_AgronicaAgenda_2010, ParametriAgronicaStampe)
            Case enum_CodificaStampe.Stampa_Abilitazioni

                Dim ParametriAgronicaStampe As ParametriAgronicaStampe =
                            RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                            Report, Piva, HttpContext.Current.Session, objParametri_Server)

                Dim vVarStampe(8) As ElementoStampe
                vVarStampe(0).Nome = "piva"
                vVarStampe(0).Valore = Piva
                vVarStampe(1).Nome = "stringa_lista_pive"
                vVarStampe(1).Valore = ParametriAggiuntivi(0).value
                vVarStampe(2).Nome = "veg_cod"
                vVarStampe(2).Valore = ParametriAggiuntivi(1).value
                vVarStampe(3).Nome = "grva_cod"
                vVarStampe(3).Valore = ParametriAggiuntivi(2).value
                vVarStampe(4).Nome = "cul_cod"
                vVarStampe(4).Valore = ParametriAggiuntivi(3).value
                vVarStampe(5).Nome = "anno"
                vVarStampe(5).Valore = ParametriAggiuntivi(4).value
                vVarStampe(6).Nome = "validita_inizio"
                vVarStampe(6).Valore = ParametriAggiuntivi(5).value
                vVarStampe(7).Nome = "validita_fine"
                vVarStampe(7).Valore = ParametriAggiuntivi(6).value
                vVarStampe(8).Nome = "tipologia_report"
                vVarStampe(8).Valore = ParametriAggiuntivi(7).value

                Dim objVS As New AgronicaCoreXML.XML_Stampe
                Dim StrNodo As String = objVS.XML_VariabiliStampe(vVarStampe)
                Dim StrNodiVariabili As String = StrNodo

                ParametriAgronicaStampe.Xml_Generico.Length = 0
                ParametriAgronicaStampe.Xml_Generico.Append(StrNodiVariabili)

                RedirectURL = RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                                 Enum_SiteRedirector.Sito_AgronicaAgenda_2010, ParametriAgronicaStampe)

        End Select

    End Sub

    Private Sub redirectBilancioFertilizzazioni(ByVal Report As enum_CodificaStampe, ByRef RedirectURL As String)
        Dim objp As New ParametriFILTRONE_2010

        objp.Sito_Origine = Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, AgroKey_EncoderDecoder, Nothing)
        objp.Pagina_Origine = Stringa_Codifica("../Menu/MenuBs_Agenda_Nuovo.aspx", AgroKey_EncoderDecoder, Nothing)
        objp.Sito_Destinazione = Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaStampe_2010, AgroKey_EncoderDecoder, Nothing)
        objp.Pagina_Destinazione = Stringa_Codifica("../GestioneStampe/ChiamaStampe.aspx", AgroKey_EncoderDecoder, Nothing)
        objp.TipoFiltrone = Stringa_Codifica(enum_TipoFiltrone.Stampa, AgroKey_EncoderDecoder, Nothing)
        objp.CodificaStampe = Stringa_Codifica(Report, AgroKey_EncoderDecoder, Nothing)

        RedirectURL = "../Filtrone/Filtrone.aspx" &
                       "?p_o=" & objp.Pagina_Origine &
                       "&s_o=" & objp.Sito_Origine &
                       "&p_d=" & objp.Pagina_Destinazione &
                       "&s_d=" & objp.Sito_Destinazione &
                       "&t_f=" & objp.TipoFiltrone &
                       "&c_s=" & objp.CodificaStampe &
                       "&v_c=" & objp.Veg_Cod &
                       "&c_c=" & objp.Cul_Cod &
                       "&d_i=" & objp.Data_Inizio &
                       "&d_f=" & objp.Data_Fine
    End Sub

    ''' <summary>
    ''' Stampe gestite a parte, selezionando la specie e/o gli impianti e 
    ''' chiamando direttamente la pagina senza passare dal filtrone
    ''' </summary>
    ''' <param name="Report">Pagina stampe di destinazione</param>
    ''' <param name="RedirectURL">URL della pagina (valorizzato all'interno della funzione)</param>
    ''' <param name="ParametriAggiuntivi">Parametri di riferimento: impianti, specie,
    '''     tipoOperazione, lav_cod, Piva, Sa_Cod</param>
    ''' <param name="objParametri_Server"></param>
    Private Sub redirectDaMenuAgendaNoFiltrone(ByVal Report As enum_CodificaStampe, ByRef RedirectURL As String,
                                               ByVal ParametriAggiuntivi As List(Of ParametriAggiuntivi_QueryString),
                                               ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim objAgronicaStampe As New AgronicaCoreGestioneRichieste.ParametriAgronicaStampe
        objAgronicaStampe.report = Report
        Dim StrNodiVariabili As String = ""
        Dim xmlDoc As New XmlDocument
        Dim objVS As New AgronicaCoreXML.XML_Stampe
        Dim xmlFiltroStampa As XmlElement = xmlDoc.CreateElement("FiltroStampa")
        xmlDoc.AppendChild(xmlFiltroStampa)

        'Estrai parametri aggiuntivi
        Dim Piva = ParametriAggiuntivi.FirstOrDefault(Function(k) k.[key] = "Piva").value
        Dim Sa_Cod = ParametriAggiuntivi.FirstOrDefault(Function(k) k.[key] = "Sa_Cod").value
        Dim impianti = parseListaImpianti(ParametriAggiuntivi)

        Dim tmp = ParametriAggiuntivi.FirstOrDefault(Function(k) k.[key] = "specie")
        Dim specie As String = If(IsNothing(tmp), "", tmp.value)

        tmp = ParametriAggiuntivi.FirstOrDefault(Function(k) k.[key] = "tipo_operazione")
        Dim tipo_operazione As String = If(IsNothing(tmp), "", tmp.value)

        If impianti.Count = 0 Then

            Dim SoloImpiantiBIO As Boolean = False

            If Report = enum_CodificaStampe.SchedaColturale_Biologico Then
                SoloImpiantiBIO = True
            End If

            parseImpiantiDaTabella(specie, tipo_operazione,
                                        Piva, Sa_Cod, SoloImpiantiBIO, objVS, objParametri_Server,
                                        xmlDoc)
        Else
            For Each impianto As Reg_Impianti In impianti
                Dim vVarStampe(4) As ElementoStampe
                vVarStampe(0).Nome = "piva"
                vVarStampe(0).Valore = impianto.PIVA
                vVarStampe(1).Nome = "sa_cod"
                vVarStampe(1).Valore = impianto.SA_COD
                vVarStampe(2).Nome = "appezza"
                vVarStampe(2).Valore = impianto.APPEZZA
                vVarStampe(3).Nome = "id_reg"
                vVarStampe(3).Valore = impianto.ID_REG
                vVarStampe(4).Nome = "veg_cod"
                vVarStampe(4).Valore = specie
                StrNodiVariabili &= objVS.XML_VariabiliStampe(vVarStampe)
            Next

            xmlFiltroStampa.InnerXml = StrNodiVariabili

            Dim xmlTxt As XmlElement = xmlDoc.CreateElement("FiltraImpianti")
            xmlTxt.SetAttribute("UtilizzaImpiantiFiltrati", 1)
            xmlFiltroStampa.AppendChild(xmlTxt)
        End If

        objAgronicaStampe.username = CStr(HttpContext.Current.Session("ASG_Utente_Username"))
        objAgronicaStampe.user_profilo = CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS"))
        objAgronicaStampe.Xml_Generico.Length = 0
        objAgronicaStampe.Xml_Generico.Append(xmlDoc.InnerXml)

        RedirectURL = RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
            Enum_SiteRedirector.Sito_AgronicaStampe_2010, objAgronicaStampe)
        RedirectURL &= "?p=" & Stringa_Codifica(Piva, AgroKey_EncoderDecoder, HttpContext.Current.Session)

    End Sub

    Private Function parseListaImpianti(ByVal ParametriAggiuntivi As List(Of ParametriAggiuntivi_QueryString)) As List(Of Reg_Impianti)
        Dim impianti = ParametriAggiuntivi.FirstOrDefault(Function(k) k.[key] = "impianti")
        Dim impiantiStr As String = If(IsNothing(impianti), "", impianti.value)

        Dim listaImpianti As New List(Of Reg_Impianti)
        If Not String.IsNullOrWhiteSpace(impiantiStr) Then
            Dim arrayChiaviImpianti As String() = impiantiStr.Split("|")

            For Each strChiaviImpianto As String In arrayChiaviImpianti

                Dim chiaviImpianto As String() = strChiaviImpianto.Split("_")

                listaImpianti.Add(New Reg_Impianti() With {
                    .PIVA = chiaviImpianto(0),
                    .SA_COD = chiaviImpianto(1),
                    .APPEZZA = chiaviImpianto(2),
                    .ID_REG = chiaviImpianto(3)
                })
            Next

        End If

        Return listaImpianti
    End Function

    Private Sub parseImpiantiDaTabella(ByVal specie As String, ByVal tipo_operazione As String,
                                       ByVal Piva As String, ByVal Sa_Cod As String, ByVal SoloImpiantiBIO As Boolean,
                                       ByRef objVS As AgronicaCoreXML.XML_Stampe,
                                       ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       ByRef xmlDoc As XmlDocument)
        Dim strVegCod As String = ""
        Dim filtroImp As String = ""
        Dim StrNodiVariabili As String = ""
        Dim StrNodo As String

        'TODO: check filtro sessione(?) non nullo e valorizza filtroImp, altrimenti:
        If specie <> "" AndAlso specie <> "-1" Then
            strVegCod = If(specie.Contains("/"), Split(specie, "/")(0), specie)
        End If

        If SoloImpiantiBIO Then
            filtroImp &= "Imprese_Progetti.Regolamento_Cod = 4 AND Imprese_Progetti.Disciplinare_Cod = 0"
        End If

        ' Crea tabella impianti
        Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim HashImp As New Hashtable
        Dim leggiAncheBloccati As Boolean = True
        Dim Dt As DataTable = objImpianti.Leggi_Impianti_xAgenda2(False,
                                Piva,
                                Sa_Cod,
                                strVegCod,
                                0, "", 0, -1,
                                filtroImp,
                                " Cul_Des, App_Nome, Progetto ",
                                objParametri_Server, leggiAncheBloccati)

        Dim vegcodstrtemp As String = ""
        If Dt.Rows.Count > 0 Then
            For i = 0 To Dt.Rows.Count - 1
                If i = 0 Then
                    'Inizializza specie vegetale
                    vegcodstrtemp = Dt.Rows(i).Item("veg_cod")
                End If

                If vegcodstrtemp <> Dt.Rows(i).Item("veg_cod") AndAlso
                   (tipo_operazione = "7a" OrElse tipo_operazione = "7b") Then
                    'Specie vegetale non unica per gli impianti selezionati
                End If
                vegcodstrtemp = Dt.Rows(i).Item("veg_cod")

                'Se l'impianto non è già stato considerato
                If Not HashImp.ContainsKey(Dt.Rows(i).Item("sa_cod") & "|" & Dt.Rows(i).Item("appezza") & "|" & Dt.Rows(i).Item("id_reg")) Then

                    HashImp.Add(Dt.Rows(i).Item("sa_cod") & "|" & Dt.Rows(i).Item("appezza") & "|" & Dt.Rows(i).Item("id_reg"), "")

                    'Aggiungi parametri stampa
                    Dim vVarStampe(4) As ElementoStampe
                    vVarStampe(0).Nome = "piva"
                    vVarStampe(0).Valore = Piva
                    vVarStampe(1).Nome = "sa_cod"
                    vVarStampe(1).Valore = Dt.Rows(i).Item("sa_cod")
                    vVarStampe(2).Nome = "appezza"
                    vVarStampe(2).Valore = Dt.Rows(i).Item("appezza")
                    vVarStampe(3).Nome = "id_reg"
                    vVarStampe(3).Valore = Dt.Rows(i).Item("id_reg")
                    vVarStampe(4).Nome = "veg_cod"
                    vVarStampe(4).Valore = Dt.Rows(i).Item("veg_cod")
                    StrNodo = objVS.XML_VariabiliStampe(vVarStampe)
                    StrNodiVariabili &= StrNodo
                End If
            Next

            xmlDoc.Item("FiltroStampa").InnerXml = StrNodiVariabili

            'Se il flag SoloImpiantiBIO è true metto nell'XML UtilizzaImpiantiFiltrati a 1 e filtro solo ed esclusivamente gli
            'impianti BIO nella stampa
            If SoloImpiantiBIO Then
                Dim xmlTxt As XmlElement = xmlDoc.CreateElement("FiltraImpianti")

                xmlTxt.SetAttribute("UtilizzaImpiantiFiltrati", 1)

                xmlDoc.Item("FiltroStampa").AppendChild(xmlTxt)
            End If

        Else
            If SoloImpiantiBIO Then
                Throw New GiasException(My.Resources.AgronicaCoreGestioneRichieste.NonPresentiImpiantiBIO)
            Else
                Throw New GiasException(My.Resources.AgronicaCoreGestioneRichieste.NonPresentiImpianti)
            End If

        End If
    End Sub

    'Public Shared Function gestisciStampa(ByVal Report As enum_CodificaStampe, ByVal specie As String, ByVal tipo_operazione As String) As RispostaStandard

    '    Dim r As New RispostaStandard
    '    Dim objParametriAgenda As New ParametriAgenda
    '    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
    '    Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

    '    '07/01/2019: delibera di dismettere le stampe semplificate facendole puntare sempre alle multicentro
    '    Report = AgronicaCoreStampeDAL.Stampe_QDC.ReplaceTipoReport(Report)

    '    r.RispostaOK = True
    '    r.Tipo = ""
    '    r.ParametroDue_stringa = ""
    '    r.RispostaStringa = ""

    '    Dim OrigineM As String = "../Menu/MenuBS_Agenda_Nuovo.aspx"

    '    Select Case Report

    '        'stampe gestite a parte, selezionando la specie e/o gli impianti e chiamando direttamente la pagina
    '        'senza passare dal filtrone
    '        Case enum_CodificaStampe.SchedaCampagna_Multicentro,
    '            enum_CodificaStampe.SchedaCampagna_2078_Semplificata,
    '            enum_CodificaStampe.Eurep_Gap_Multicentro,
    '            enum_CodificaStampe.Eurep_Gap_Semplificata,
    '            enum_CodificaStampe.Registro_Fertilizzazioni,
    '            enum_CodificaStampe.RegistroTrattamenti_Semplificata,
    '            enum_CodificaStampe.RegistroTrattamenti_Veneto,
    '            enum_CodificaStampe.SchedaCampagna_ProvAut_Trento,
    '            enum_CodificaStampe.SchedaCampagna_Multi_Lombardia,
    '            enum_CodificaStampe.SchedaInterventiAgronomici,
    '             enum_CodificaStampe.SchedaRegistrazione_Semplificata,
    '             enum_CodificaStampe.SchedaCampagna_ConserveItalia,
    '             enum_CodificaStampe.RegistroAziendaleUnico,
    '             enum_CodificaStampe.RegistroTrattamentiVeneto_StdCondizionalita

    '            Dim objAgronicaStampe As New AgronicaCoreGestioneRichieste.ParametriAgronicaStampe

    '            objAgronicaStampe.report = Report

    '            '07/01/2019: delibera di dismettere le stampe semplificate facendole puntare sempre alle multicentro
    '            ''se non c'è il centro selezionato e ho selezionato la semplificata forzo la multicentro
    '            'If objAgronicaStampe.report = enum_CodificaStampe.SchedaCampagna_2078_Semplificata Then
    '            '    If objParametriAgenda.Sa_Cod = "0" Then
    '            '        objAgronicaStampe.report = enum_CodificaStampe.SchedaCampagna_Multicentro
    '            '    End If
    '            'End If

    '            'If objAgronicaStampe.report = enum_CodificaStampe.Eurep_Gap_Semplificata Then
    '            '    If objParametriAgenda.Sa_Cod = "0" Then
    '            '        objAgronicaStampe.report = enum_CodificaStampe.Eurep_Gap_Multicentro
    '            '    End If
    '            'End If


    '            'Verifico le stampe che devono avere la specie selezionata
    '            'spostata la scelta della specie nel filtro pre-stampa
    '            Dim strVegCod As String = ""
    '            Dim filtroImp As String = ""
    '            'Select Case Report
    '            '    Case enum_CodificaStampe.SchedaCampagna_Multicentro,
    '            '        enum_CodificaStampe.SchedaCampagna_2078_Semplificata,
    '            '        enum_CodificaStampe.Eurep_Gap_Multicentro,
    '            '        enum_CodificaStampe.Eurep_Gap_Semplificata

    '            '        If Not IsNothing(HttpContext.Current.Session("Filtro")) Then
    '            '            'se ho il filtro dell'impianto allora non occorre selezionare la scpecie perchè l'impianto è 1
    '            '        Else
    '            '            If specie = "-1" Then
    '            '                'Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.PerStampaRicettaSelezSpecie, Page, , CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanel_Menu"), UpdatePanel))
    '            '                'Exit Function
    '            '                r.RispostaOK = False
    '            '                r.Errore = "Per stampare la Scheda scelta occorre selezionare la Specie Vegetale"
    '            '            End If
    '            '        End If
    '            '    Case Else
    '            'End Select
    '            If Not IsNothing(HttpContext.Current.Session("Filtro")) Then
    '                'se ho il filtro dell'impianto allora non occorre selezionare la scpecie perchè l'impianto è 1
    '                Dim sessionfiltro As String = HttpContext.Current.Session("Filtro")
    '                If sessionfiltro.Split("|").Count > 1 AndAlso sessionfiltro.Split("|")(1).Trim <> "" Then
    '                    filtroImp = " (" & sessionfiltro.Split("|")(1) & " ) "
    '                End If
    '            Else
    '                If specie <> "-1" Then
    '                    'tare etc
    '                    If InStr(specie, "/") <> 0 Then
    '                        strVegCod = Split(specie, "/")(0)
    '                    Else
    '                        'specie
    '                        strVegCod = specie
    '                    End If
    '                End If
    '            End If



    '            Dim XmlDoc As New System.Xml.XmlDocument
    '            Dim StrVariabiliStampe As String = ""
    '            Dim StrNodiVariabili As String = ""
    '            Dim StrNodo As String = ""
    '            Dim objVS As New AgronicaCoreXML.XML_Stampe

    '            Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
    '            Dim HashImp As New Hashtable
    '            Dim leggiAncheBloccati As Boolean = True
    '            Dim Dt As DataTable = objImpianti.Leggi_Impianti_xAgenda2(False,
    '                                            objParametriAgenda.Piva,
    '                                            objParametriAgenda.Sa_Cod,
    '                                            strVegCod,
    '                                            objParametriAgenda.Cul_Cod,
    '                                            "",
    '                                            0,
    '                                            -1,
    '                                            filtroImp,
    '                                            " Cul_Des, App_Nome, Progetto ",
    '                                            objParametri_Server, leggiAncheBloccati)

    '            Dim vegcodstrtemp As String = ""
    '            If Dt.Rows.Count = 0 Then
    '                'Messaggi.AgroMsgBox("Nessun Impianto Selezionato Per La Stampa", Page, , CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanel_Menu"), UpdatePanel))
    '                'Exit Function
    '                r.RispostaOK = False
    '                r.Errore = "Nessun Impianto Selezionato Per La Stampa"
    '            End If
    '            For i = 0 To Dt.Rows.Count - 1
    '                'controllo che ci sia una sola specie
    '                If i = 0 Then
    '                    vegcodstrtemp = Dt.Rows(i).Item("veg_cod")
    '                End If
    '                If vegcodstrtemp <> Dt.Rows(i).Item("veg_cod") AndAlso (tipo_operazione = "7a" OrElse tipo_operazione = "7b") Then
    '                    'Messaggi.AgroMsgBox("Non c'è un'unica specie negli impianti selezionati Per La Stampa", Page, , CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanel_Menu"), UpdatePanel))
    '                    'Exit Function
    '                    r.RispostaOK = False
    '                    r.Errore = "Non c'è un'unica specie negli impianti selezionati Per La Stampa"

    '                End If
    '                vegcodstrtemp = Dt.Rows(i).Item("veg_cod")
    '                If Not HashImp.ContainsKey(Dt.Rows(i).Item("sa_cod") & "|" & Dt.Rows(i).Item("appezza") & "|" & Dt.Rows(i).Item("id_reg")) Then
    '                    HashImp.Add(Dt.Rows(i).Item("sa_cod") & "|" & Dt.Rows(i).Item("appezza") & "|" & Dt.Rows(i).Item("id_reg"), "")
    '                    Dim vVarStampe(4) As ElementoStampe
    '                    vVarStampe(0).Nome = "piva"
    '                    vVarStampe(0).Valore = objParametriAgenda.Piva
    '                    vVarStampe(1).Nome = "sa_cod"
    '                    vVarStampe(1).Valore = Dt.Rows(i).Item("sa_cod")
    '                    vVarStampe(2).Nome = "appezza"
    '                    vVarStampe(2).Valore = Dt.Rows(i).Item("appezza")
    '                    vVarStampe(3).Nome = "id_reg"
    '                    vVarStampe(3).Valore = Dt.Rows(i).Item("id_reg")
    '                    vVarStampe(4).Nome = "veg_cod"
    '                    vVarStampe(4).Valore = Dt.Rows(i).Item("veg_cod")
    '                    StrNodo = objVS.XML_VariabiliStampe(vVarStampe)
    '                    StrNodiVariabili &= StrNodo
    '                End If
    '            Next


    '            objAgronicaStampe.username = CStr(HttpContext.Current.Session("ASG_Utente_Username"))
    '            objAgronicaStampe.user_profilo = CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS"))
    '            objAgronicaStampe.Xml_Generico.Length = 0
    '            objAgronicaStampe.Xml_Generico.Append(StrNodiVariabili)

    '            Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
    '                                             Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objAgronicaStampe)

    '            r.Tipo = "1"
    '            r.ParametroDue_stringa = strJS

    '        Case enum_CodificaStampe.Bolle, enum_CodificaStampe.Fatture, enum_CodificaStampe.Nota_Accredito

    '            Dim Id_Agenda As String = ""
    '            Dim Data As String = ""
    '            Dim Lav_Cod As String = ""
    '            Dim Blocco_Flag As String = ""
    '            Dim Piva As String = ""
    '            Dim Sa_Cod As Integer
    '            Dim Data2 As Date
    '            Dim Veg_Cod As Integer = 0

    '            Dim Dt_Operazioni As DataTable

    '            If HttpContext.Current.Session("DataGrid_Lavorazioni") IsNot Nothing Then
    '                Dt_Operazioni = New DataTable
    '                Dt_Operazioni = CType(HttpContext.Current.Session("DataGrid_Lavorazioni"), DataTable)
    '            End If

    '            'If OperazioniSelezionate(Dt_Operazioni, Piva, Sa_Cod, Id_Agenda, Data, Data2, Lav_Cod, Blocco_Flag, Veg_Cod) <> 1 Then
    '            '    'alert 1 sola
    '            '    'Messaggi.AgroMsgBox("Selezionare un documento contabile alla volta!", Page, , CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanel_Menu"), UpdatePanel))
    '            '    Exit Function
    '            'Else
    '            'se non ho selezionato una FATTURA,bolla,nota emessa
    '            'Select Case Lav_Cod
    '            '    Case LAVCOD_FATTURA_EMESSA, LAVCOD_BOLLA_EMESSA, LAVCOD_NOTA_ACCREDITO_EMESSA
    '            '    Case Else
    '            '        'Messaggi.AgroMsgBox("E' possibile stampare solo le Documenti Emessi!", Page, , CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanel_Menu"), UpdatePanel))
    '            '        Exit Function
    '            'End Select
    '            ''Stampa_Documento(Server, objParametri_Server, Session, Page, Lav_Cod, Piva, Id_Agenda, "", CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanel_Menu"), UpdatePanel))
    '            'End If

    '        Case Else

    '            Dim piva As String = objParametriAgenda.Piva
    '            Dim Origine As String = Stringa_Codifica("../Stampe/MenuStampe.aspx", AgroKey_EncoderDecoder, objParametri_Server)
    '            Dim Destinazione As String = Stringa_Codifica("../GestioneStampe/ChiamaStampe.aspx", AgroKey_EncoderDecoder, objParametri_Server)
    '            Dim Funzione As String = Stringa_Codifica(CStr(enum_TipoFiltrone.Stampa), AgroKey_EncoderDecoder, objParametri_Server)

    '            Select Case Report

    '                '---------------------- 
    '                'Scheda Campagna
    '                '---------------------- 

    '                Case enum_CodificaStampe.SchedaCampagna_Biologico
    '                    'AgroMsgBox("Report in fase di costruzione!", Page)
    '                    'Exit Function
    '                    r.RispostaOK = False
    '                    r.Errore = "Report in fase di costruzione"

    '                Case enum_CodificaStampe.SchedaCampagna_Biologico_Semplificata
    '                    ' AgroMsgBox("Report in fase di costruzione!", Page)
    '                    'Exit Function
    '                    r.RispostaOK = False
    '                    r.Errore = "Report in fase di costruzione"


    '                    '========================================================================

    '                Case enum_CodificaStampe.SchedaMateriePrime_Biologico,
    '                    enum_CodificaStampe.SchedaVendite_Biologico,
    '                    enum_CodificaStampe.SchedaPreparati_Biologico


    '                    Dim ParametriAgronicaStampe As AgronicaCoreGestioneRichieste.ParametriAgronicaStampe =
    '                      AgronicaCoreGestioneRichieste.RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
    '                                  Report, piva, HttpContext.Current.Session, objParametri_Server)

    '                    Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
    '                                                     Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
    '                                                     ParametriAgronicaStampe)

    '                    r.Tipo = "1"
    '                    r.ParametroDue_stringa = strJS
    '                    Return r

    '                    '========================================================================

    '                Case enum_CodificaStampe.PAP_Vegetale

    '                    If piva IsNot Nothing Then
    '                        piva = piva.ToString
    '                    End If

    '                    Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_Sito_AgronicaBio_PassandoDirettamente_Parametri(
    '                                                Enum_SiteRedirector.Sito_GiasOnline, enum_CodificaPagBio.PAP_Vegetale,
    '                                                HttpContext.Current.Session("ASG_Utente_CodFiscale").ToString, piva)

    '                    r.Tipo = "1"
    '                    r.ParametroDue_stringa = strJS
    '                    Return r

    '                    '========================================================================

    '                Case enum_CodificaStampe.Notifica_Biologico

    '                    Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_Sito_AgronicaBio_PassandoDirettamente_Parametri(
    '                                                AgronicaCoreDataProvider.TipiEnumerativi.Enum_SiteRedirector.Sito_GiasOnline, enum_CodificaPagBio.Notifica,
    '                                                HttpContext.Current.Session("ASG_Utente_CodFiscale").ToString, piva)


    '                    r.Tipo = "1"
    '                    r.ParametroDue_stringa = strJS

    '                    '========================================================================

    '                Case enum_CodificaStampe.SchedaMagazzinoGiacenze,
    '                        enum_CodificaStampe.SchedaMagazzinoMovimenti,
    '                             enum_CodificaStampe.SchedaMagazzinoFertilizzanti,
    '                                enum_CodificaStampe.SchedaMagazzinoProdottiFitosanitari,
    '                                    enum_CodificaStampe.RiepilogoProdottiUtilizzati

    '                    Dim ParametriAgronicaStampe As AgronicaCoreGestioneRichieste.ParametriAgronicaStampe =
    '                        AgronicaCoreGestioneRichieste.RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
    '                                    Report, piva, HttpContext.Current.Session, objParametri_Server, "", 0, 0, 0, 0, 0)

    '                    Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
    '                                                     Enum_SiteRedirector.Sito_AgronicaAgenda_2010, ParametriAgronicaStampe)

    '                    r.Tipo = "1"
    '                    r.ParametroDue_stringa = strJS

    '                    Return r

    '                    '========================================================================


    '                Case enum_CodificaStampe.RiepilogoImpiegoSuperfici

    '                    Dim objGiasOnline As New AgronicaCoreGestioneRichieste.ParametriGiasOnline
    '                    objGiasOnline.PaginaRichiesta = enum_PagineGiasOnline.Filtro_RiepilogoUtilizzoSuperfici
    '                    objGiasOnline.Piva = objParametriAgenda.Piva
    '                    objGiasOnline.funzioneoriginedestinazionefiltrone = Funzione & "|" & Origine & "|" & Destinazione & "|" & Report

    '                    If Not IsNothing(ConfigurationManager.AppSettings("LinkAgronicaAgenda2010")) AndAlso ConfigurationManager.AppSettings("LinkAgronicaAgenda2010") <> "" Then
    '                        objGiasOnline.LinkAgronicaAgenda2010 = ConfigurationManager.AppSettings("LinkAgronicaAgenda2010")
    '                    Else
    '                        objGiasOnline.LinkAgronicaAgenda2010 = ""
    '                    End If

    '                    Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoOnline_PassandoDirettamente_ParametriGiasOnline(
    '                                              Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objGiasOnline)

    '                    r.Tipo = "1"
    '                    r.ParametroDue_stringa = strJS


    '                    '========================================================================

    '                Case enum_CodificaStampe.Esporta_GiasToSap

    '                    'Dim XmlDoc As New System.Xml.XmlDocument

    '                    'Dim LinkPaginaStampa As String = "../GestioneStampe/ChiamaStampe.aspx"
    '                    'Dim LinkSitoStampe As String = ConfigurationSettings.AppSettings("LinkAgronicaStampe")

    '                    Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamenteIParametri(Enum_SiteRedirector.Sito_GiasOnline,
    '                                                                             enum_CodificaStampe.Esporta_GiasToSap,
    '                                                                             CStr(HttpContext.Current.Session("ASG_Utente_Username")),
    '                                                                             CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS")),
    '                                                                             "", "", "", "", "", "", "", "")

    '                    r.Tipo = "1"
    '                    r.ParametroDue_stringa = strJS

    '                    '========================================================================


    '                Case enum_CodificaStampe.Costo_Manodopera_XLS


    '                    Dim objGiasOnline As New AgronicaCoreGestioneRichieste.ParametriGiasOnline
    '                    objGiasOnline.PaginaRichiesta = enum_PagineGiasOnline.Filtro_Manodopera
    '                    objGiasOnline.Piva = objParametriAgenda.Piva
    '                    objGiasOnline.funzioneoriginedestinazionefiltrone = Funzione & "|" & Origine & "|" & Destinazione & "|" & Report

    '                    If Not IsNothing(ConfigurationManager.AppSettings("LinkAgronicaAgenda2010")) AndAlso ConfigurationManager.AppSettings("LinkAgronicaAgenda2010") <> "" Then
    '                        objGiasOnline.LinkAgronicaAgenda2010 = ConfigurationManager.AppSettings("LinkAgronicaAgenda2010")
    '                    Else
    '                        objGiasOnline.LinkAgronicaAgenda2010 = ""
    '                    End If

    '                    Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoOnline_PassandoDirettamente_ParametriGiasOnline(
    '                                              Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objGiasOnline)

    '                    r.Tipo = "1"
    '                    r.ParametroDue_stringa = strJS

    '                    '========================================================================

    '                Case enum_CodificaStampe.Costo_ParcoMacchine_XLS


    '                    Dim objGiasOnline As New AgronicaCoreGestioneRichieste.ParametriGiasOnline
    '                    objGiasOnline.PaginaRichiesta = enum_PagineGiasOnline.Filtro_ParcoMacchine
    '                    objGiasOnline.Piva = objParametriAgenda.Piva
    '                    objGiasOnline.funzioneoriginedestinazionefiltrone = Funzione & "|" & Origine & "|" & Destinazione & "|" & Report


    '                    If Not IsNothing(ConfigurationManager.AppSettings("LinkAgronicaAgenda2010")) AndAlso ConfigurationManager.AppSettings("LinkAgronicaAgenda2010") <> "" Then
    '                        objGiasOnline.LinkAgronicaAgenda2010 = ConfigurationManager.AppSettings("LinkAgronicaAgenda2010")
    '                    Else
    '                        objGiasOnline.LinkAgronicaAgenda2010 = ""
    '                    End If

    '                    Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoOnline_PassandoDirettamente_ParametriGiasOnline(
    '                                              Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objGiasOnline)


    '                    r.Tipo = "1"
    '                    r.ParametroDue_stringa = strJS

    '                    '========================================================================



    '                Case enum_CodificaStampe.Esportazione_OP_Inv

    '                    Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportazione_OP_Inv), AgroKey_EncoderDecoder, objParametri_Server)

    '                Case enum_CodificaStampe.Esportazione_OP_Gest

    '                    Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportazione_OP_Gest), AgroKey_EncoderDecoder, objParametri_Server)

    '                Case enum_CodificaStampe.Esportazione_OP_Gest_Coop

    '                    Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportazione_OP_Gest_Coop), AgroKey_EncoderDecoder, objParametri_Server)

    '                Case enum_CodificaStampe.Report_RiconversioneVarietale

    '                    'Dim LinkSitoStampe, LinkpaginaStampa As String
    '                    'Dim XmlDoc As New System.Xml.XmlDocument


    '                    Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamenteIParametri(Enum_SiteRedirector.Sito_GiasOnline,
    '                                                                             enum_CodificaStampe.Report_RiconversioneVarietale,
    '                                                                             CStr(HttpContext.Current.Session("ASG_Utente_Username")),
    '                                                                             CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS")),
    '                                                                             "", "", "", "", "", "", "", "")

    '                    r.Tipo = "1"
    '                    r.ParametroDue_stringa = strJS

    '                    '========================================================================


    '                Case enum_CodificaStampe.Report_ImpegnoProduzioneSoci

    '                    'Dim LinkSitoStampe, LinkPaginaStampa As String
    '                    'Dim XmlDoc As New System.Xml.XmlDocument

    '                    'LinkSitoStampe = ConfigurationSettings.AppSettings("LinkAgronicaStampe")


    '                    Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamenteIParametri(Enum_SiteRedirector.Sito_GiasOnline,
    '                                                                             enum_CodificaStampe.Report_ImpegnoProduzioneSoci,
    '                                                                             CStr(HttpContext.Current.Session("ASG_Utente_Username")),
    '                                                                             CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS")),
    '                                                                             "", "", "", "", "", "", "", "")

    '                    r.Tipo = "1"
    '                    r.ParametroDue_stringa = strJS

    '                    '========================================================================

    '                Case enum_CodificaStampe.Esportazione_CellulariContatti

    '                    Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportazione_CellulariTecnici), AgroKey_EncoderDecoder, objParametri_Server)

    '                Case enum_CodificaStampe.Esportatore_Universale_Imprese


    '                    Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportatore_Universale_Imprese), AgroKey_EncoderDecoder, objParametri_Server)

    '                Case enum_CodificaStampe.Esportatore_Universale_Centri

    '                    Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportatore_Universale_Centri), AgroKey_EncoderDecoder, objParametri_Server)

    '                Case enum_CodificaStampe.Esportatore_Universale_Appezza

    '                    'AgroMsgBox("Funzione non ancora attivata!", Page)
    '                    'Exit Function
    '                    r.RispostaOK = False
    '                    r.Errore = "Funzione non ancora attivata"
    '                    '========================================================================


    '                Case enum_CodificaStampe.Esportatore_Universale_Impianti

    '                    Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportatore_Universale_Impianti), AgroKey_EncoderDecoder, objParametri_Server)

    '                    '========================================================================

    '                Case enum_CodificaStampe.Esportatore_Universale_Agenda

    '                    Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportatore_Universale_Agenda), AgroKey_EncoderDecoder, objParametri_Server)

    '                    '========================================================================

    '                Case enum_CodificaStampe.Esportatore_Universale_Rintraccio

    '                    Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
    '                    Dim UtenteAbilitato As Boolean = objPermessi.Controlla_Permessi_Utente(
    '                                                                HttpContext.Current.Session("ASG_Utente_Username"), HttpContext.Current.Session("ASG_IdServizio"),
    '                                                                enum_Security_Attivita.Stampe_Esportazione_Rintraccio, enum_Security_Operazione.Modifica,
    '                                                                Date.Now, "", objParametri_Utenti)

    '                    If UtenteAbilitato = True Then

    '                        Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportatore_Universale_Rintraccio), AgroKey_EncoderDecoder, objParametri_Server)

    '                    Else
    '                        'AgroMsgBox("Permesso negato!", Page)
    '                        'Exit Function
    '                        r.RispostaOK = False
    '                        r.Errore = "Permesso negato"

    '                    End If

    '                    '========================================================================

    '                    '--------------------------------
    '                    'Schede Varie x le OP
    '                    '--------------------------------
    '                Case enum_CodificaStampe.Atto_Notorio

    '                    'Dim LinkSitoStampe, LinkPaginaStampa As String

    '                    'LinkSitoStampe = ConfigurationSettings.AppSettings("LinkAgronicaStampe")

    '                    Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamenteIParametri(Enum_SiteRedirector.Sito_GiasOnline,
    '                                                                             enum_CodificaStampe.Atto_Notorio,
    '                                                                             CStr(HttpContext.Current.Session("ASG_Utente_Username")),
    '                                                                             CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS")),
    '                                                                             "", "", "", "", "", "", "", "")

    '                    r.Tipo = "1"
    '                    r.ParametroDue_stringa = strJS

    '                    '========================================================================

    '                Case enum_CodificaStampe.Programmazione_Vegetale

    '                    Dim ParametriPlanning As New AgronicaCoreGestioneRichieste.ParametriPlanning
    '                    ParametriPlanning.PaginaProvenienza = enum_PagineGiasOnline.MenuStampe
    '                    ParametriPlanning.PaginaRichiesta = enum_CodificaPagPlanning.PianificazioneVegetale
    '                    ParametriPlanning.Cuaa = CUAA_from_PIVA(objParametri_Server, piva)
    '                    ParametriPlanning.Piva = piva

    '                    Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_Sito_AgronicaPlanning_PassandoDirettamente_ParametriPlanning(
    '                                         Enum_SiteRedirector.Sito_GiasOnline, ParametriPlanning)

    '                    r.Tipo = "1"
    '                    r.ParametroDue_stringa = strJS

    '                    '========================================================================

    '                Case enum_CodificaStampe.Esportazione_AnagraficaProdotti



    '                    piva = AgronicaCoreGestioneRichieste.RedirectGestione.ControllaNumeroImprese_per_SiteRedirector(objParametri_Server, objParametri_Utenti)
    '                    If piva = "" Then
    '                        'mando al filtrino
    '                        Dim Indirizzofiltrino As String = AgronicaCoreGestioneRichieste.RedirectGestione.GetLinkFiltrinoAgenda(
    '                                                            "../Stampe/MenuStampe.aspx", "../GestioneStampe/ChiamaStampe.aspx", Enum_SiteRedirector.Sito_AgronicaStampe, Report)

    '                        Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.PreparaScripPerPopup(Indirizzofiltrino, "Stampa")

    '                        r.Tipo = "1"
    '                        r.ParametroDue_stringa = strJS

    '                    Else
    '                        Dim objanag As New AgronicaCoreAnagrafeDAL.Imprese_Read
    '                        Dim rag_soc As String = objanag.RagSoc_from_Piva(piva, objParametri_Server)
    '                        Dim ParametriAgronicaStampe As AgronicaCoreGestioneRichieste.ParametriAgronicaStampe =
    '                                    AgronicaCoreGestioneRichieste.RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
    '                                    Report, piva, HttpContext.Current.Session, objParametri_Server, rag_soc)

    '                        Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
    '                                                         Enum_SiteRedirector.Sito_AgronicaAgenda_2010, ParametriAgronicaStampe)


    '                        r.Tipo = "1"
    '                        r.ParametroDue_stringa = strJS


    '                    End If

    '                    'Exit Function


    '                    '=======================================================================

    '                Case enum_CodificaStampe.Esportazione_AnagraficaContatti

    '                    Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportatore_Contatti), AgroKey_EncoderDecoder, objParametri_Server)

    '                    '=======================================================================

    '                Case enum_CodificaStampe.SchedaTracciabilita_Animale

    '                    'imposto la versione ZOO dell'alberoimprese
    '                    HttpContext.Current.Session("VersioneAlbero") = enum_VersioneAlberoImprese.Albero_Stalle

    '                    'come pagina di ritorno non metto il menùstampe, ma l'alberoimprese
    '                    'perché così se l'utente vuole cambiare centro, può farlo facendo exit
    '                    'dalla apgian delle consistenze

    '                    If piva <> "" Then
    '                        'c'è una sola azienda o l'utente vede una sola azienda

    '                        Dim objGiasOnline As New AgronicaCoreGestioneRichieste.ParametriGiasOnline
    '                        objGiasOnline.PaginaRichiesta = enum_PagineGiasOnline.StalleConsistenze_Info
    '                        objGiasOnline.Piva = objParametriAgenda.Piva

    '                        If Not IsNothing(ConfigurationManager.AppSettings("LinkAgronicaAgenda2010")) AndAlso ConfigurationManager.AppSettings("LinkAgronicaAgenda2010") <> "" Then
    '                            objGiasOnline.LinkAgronicaAgenda2010 = ConfigurationManager.AppSettings("LinkAgronicaAgenda2010")
    '                        Else
    '                            objGiasOnline.LinkAgronicaAgenda2010 = ""
    '                        End If

    '                        Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoOnline_PassandoDirettamente_ParametriGiasOnline(
    '                                                  Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objGiasOnline)


    '                        r.Tipo = "1"
    '                        r.ParametroDue_stringa = strJS

    '                    Else

    '                        Dim Indirizzofiltrino As String = AgronicaCoreGestioneRichieste.RedirectGestione.GetLinkFiltrinoAgenda(
    '                                        "../Stampe/MenuStampe.aspx", ".../GestioneStalle/StalleConsistenze_Info.aspx", Enum_SiteRedirector.Sito_AgronicaStampe, Report)

    '                        Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.PreparaScripPerPopup(Indirizzofiltrino, "Stampa")

    '                        r.Tipo = "1"
    '                        r.ParametroDue_stringa = strJS

    '                    End If

    '                    '========================================================================

    '                Case enum_CodificaStampe.PacchettoIgiene_RegistroFornitori

    '                    piva = AgronicaCoreGestioneRichieste.RedirectGestione.ControllaNumeroImprese_per_SiteRedirector(objParametri_Server, objParametri_Utenti)
    '                    If piva = "" Then
    '                        'mando al filtrino
    '                        Dim Indirizzofiltrino As String = AgronicaCoreGestioneRichieste.RedirectGestione.GetLinkFiltrinoAgenda(
    '                                                            "../Stampe/MenuStampe.aspx", "../GestioneStampe/ChiamaStampe.aspx", Enum_SiteRedirector.Sito_AgronicaStampe, enum_CodificaStampe.PacchettoIgiene_RegistroFornitori)

    '                        Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.PreparaScripPerPopup(Indirizzofiltrino, "Stampa")

    '                        r.Tipo = "1"
    '                        r.ParametroDue_stringa = strJS
    '                    Else

    '                        Dim vVarStampe(0) As ElementoStampe
    '                        vVarStampe(0).Nome = "piva"
    '                        vVarStampe(0).Valore = piva
    '                        Dim StrNodo As String = XML_VariabiliStampe(vVarStampe)
    '                        Dim StrNodiVariabili As String = StrNodo

    '                        Dim username As String = HttpContext.Current.Session("ASG_Utente_Username")
    '                        Dim user_profilo As String = HttpContext.Current.Session("ASG_ProgressivoGIAS")

    '                        Dim user_profilo_codfiscale As String = HttpContext.Current.Session("ASG_Utente_CodFiscale")
    '                        Dim Sql_Filtro As String = ""
    '                        Dim XML_Filtro As String = ""
    '                        Dim username_codfisc As String = ""

    '                        Dim utente_codfiscale As String = HttpContext.Current.Session("ASG_Utente_CodFiscale")
    '                        Dim superuser_username As String = HttpContext.Current.Session("ASG_SuperUser_Username")
    '                        Dim superuser_codfiscale As String = HttpContext.Current.Session("ASG_SuperUser_CodFiscale")

    '                        Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamenteIParametri(Enum_SiteRedirector.Sito_GiasOnline,
    '                                                                        Report,
    '                                                                        username,
    '                                                                        user_profilo,
    '                                                                        StrNodiVariabili,
    '                                                                        user_profilo_codfiscale,
    '                                                                        Sql_Filtro,
    '                                                                        XML_Filtro,
    '                                                                        username_codfisc,
    '                                                                        utente_codfiscale,
    '                                                                        superuser_username,
    '                                                                        superuser_codfiscale)

    '                        r.Tipo = "1"
    '                        r.ParametroDue_stringa = strJS

    '                    End If


    '                    'Exit Function

    '                    '========================================================================

    '                Case enum_CodificaStampe.PacchettoIgiene_RegistroClienti

    '                    piva = AgronicaCoreGestioneRichieste.RedirectGestione.ControllaNumeroImprese_per_SiteRedirector(objParametri_Server, objParametri_Utenti)
    '                    If piva = "" Then
    '                        'mando al filtrino
    '                        Dim Indirizzofiltrino As String = AgronicaCoreGestioneRichieste.RedirectGestione.GetLinkFiltrinoAgenda(
    '                                                            "../Stampe/MenuStampe.aspx", "../GestioneStampe/ChiamaStampe.aspx", Enum_SiteRedirector.Sito_AgronicaStampe, Report)

    '                        Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.PreparaScripPerPopup(Indirizzofiltrino, "Stampa")

    '                        r.Tipo = "1"
    '                        r.ParametroDue_stringa = strJS

    '                    Else
    '                        Dim objanag As New AgronicaCoreAnagrafeDAL.Imprese_Read
    '                        Dim rag_soc As String = objanag.RagSoc_from_Piva(piva, objParametri_Server)
    '                        Dim ParametriAgronicaStampe As AgronicaCoreGestioneRichieste.ParametriAgronicaStampe =
    '                                    AgronicaCoreGestioneRichieste.RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
    '                                    Report, piva, HttpContext.Current.Session, objParametri_Server, rag_soc)

    '                        Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
    '                                                         Enum_SiteRedirector.Sito_AgronicaAgenda_2010, ParametriAgronicaStampe)

    '                        r.Tipo = "1"
    '                        r.ParametroDue_stringa = strJS

    '                    End If

    '                    ' Exit Function

    '                    '========================================================================

    '                Case enum_CodificaStampe.PacchettoIgiene_SchedaUsoAlimentiOGM

    '                    'AgroMsgBox("Stampa in fase di manutenzione.", Page)
    '                    ' Exit Function
    '                    r.RispostaOK = False
    '                    r.Errore = "Stampa in fase di manutenzione"

    '                    '========================================================================


    '                Case enum_CodificaStampe.PacchettoIgiene_RegistroAlimentazioneStalla

    '                    piva = AgronicaCoreGestioneRichieste.RedirectGestione.ControllaNumeroImprese_per_SiteRedirector(objParametri_Server, objParametri_Utenti)
    '                    If piva = "" Then
    '                        'mando al filtrino
    '                        Dim Indirizzofiltrino As String = AgronicaCoreGestioneRichieste.RedirectGestione.GetLinkFiltrinoAgenda(
    '                                                            "../Stampe/MenuStampe.aspx", "../GestioneStampe/ChiamaStampe.aspx", Enum_SiteRedirector.Sito_AgronicaStampe, Report)

    '                        Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.PreparaScripPerPopup(Indirizzofiltrino, "Stampa")

    '                        r.Tipo = "1"
    '                        r.ParametroDue_stringa = strJS

    '                    Else
    '                        Dim objanag As New AgronicaCoreAnagrafeDAL.Imprese_Read
    '                        Dim rag_soc As String = objanag.RagSoc_from_Piva(piva, objParametri_Server)
    '                        Dim ParametriAgronicaStampe As AgronicaCoreGestioneRichieste.ParametriAgronicaStampe =
    '                                    AgronicaCoreGestioneRichieste.RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
    '                                    Report, piva, HttpContext.Current.Session, objParametri_Server, rag_soc)

    '                        Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
    '                                                         Enum_SiteRedirector.Sito_AgronicaAgenda_2010, ParametriAgronicaStampe)

    '                        r.Tipo = "1"
    '                        r.ParametroDue_stringa = strJS

    '                    End If

    '                    ' Exit Function


    '                    '========================================================================

    '                Case enum_CodificaStampe.PacchettoIgiene_RegistroRazionamento

    '                    piva = AgronicaCoreGestioneRichieste.RedirectGestione.ControllaNumeroImprese_per_SiteRedirector(objParametri_Server, objParametri_Utenti)
    '                    If piva = "" Then
    '                        'mando al filtrino
    '                        Dim Indirizzofiltrino As String = AgronicaCoreGestioneRichieste.RedirectGestione.GetLinkFiltrinoAgenda(
    '                                                            "../Stampe/MenuStampe.aspx", "../GestioneStampe/ChiamaStampe.aspx", Enum_SiteRedirector.Sito_AgronicaStampe, Report)

    '                        Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.PreparaScripPerPopup(Indirizzofiltrino, "Stampa")

    '                        r.Tipo = "1"
    '                        r.ParametroDue_stringa = strJS

    '                    Else
    '                        Dim objanag As New AgronicaCoreAnagrafeDAL.Imprese_Read
    '                        Dim rag_soc As String = objanag.RagSoc_from_Piva(piva, objParametri_Server)
    '                        Dim ParametriAgronicaStampe As AgronicaCoreGestioneRichieste.ParametriAgronicaStampe =
    '                                  AgronicaCoreGestioneRichieste.RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
    '                                  Report, piva, HttpContext.Current.Session, objParametri_Server, rag_soc)

    '                        Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
    '                                                         Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
    '                                                         ParametriAgronicaStampe)

    '                        r.Tipo = "1"
    '                        r.ParametroDue_stringa = strJS

    '                    End If

    '                    'Exit Function


    '                    '========================================================================

    '                Case enum_CodificaStampe.PacchettoIgiene_RegistroAnalisiNonConformi

    '                    'AgroMsgBox("Stampa in fase di manutenzione.", Page)
    '                    ' Exit Function
    '                    r.RispostaOK = False
    '                    r.Errore = "Stampa in fase di manutenzione"

    '                    '========================================================================


    '                Case enum_CodificaStampe.Registro_Fertilizzazioni

    '                    '========================================================================


    '                Case enum_CodificaStampe.ADD_Filtro_Report_Accettazione_DaDiversi

    '                    Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
    '                    Dim UtenteAbilitato As Boolean = objPermessi.Controlla_Permessi_Utente(
    '                                                             HttpContext.Current.Session("ASG_Utente_Username"), HttpContext.Current.Session("ASG_IdServizio"),
    '                                                             enum_Security_Attivita.Report_Accettazione_DaDiversi, enum_Security_Operazione.Lettura,
    '                                                             Date.Now, "", objParametri_Utenti)

    '                    If UtenteAbilitato = True Then

    '                        piva = AgronicaCoreGestioneRichieste.RedirectGestione.ControllaNumeroImprese_per_SiteRedirector(objParametri_Server, objParametri_Utenti)
    '                        If piva = "" Then
    '                            'mando al filtrino
    '                            Dim Indirizzofiltrino As String = AgronicaCoreGestioneRichieste.RedirectGestione.GetLinkFiltrinoAgenda(
    '                                                                "../Stampe/MenuStampe.aspx", "../GestioneStampe/ChiamaStampe.aspx", Enum_SiteRedirector.Sito_AgronicaStampe, Report)


    '                            Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.PreparaScripPerPopup(Indirizzofiltrino, "Stampa")

    '                            r.Tipo = "1"
    '                            r.ParametroDue_stringa = strJS

    '                        Else
    '                            Dim objanag As New AgronicaCoreAnagrafeDAL.Imprese_Read
    '                            Dim rag_soc As String = objanag.RagSoc_from_Piva(piva, objParametri_Server)
    '                            Dim ParametriAgronicaStampe As AgronicaCoreGestioneRichieste.ParametriAgronicaStampe =
    '                                        AgronicaCoreGestioneRichieste.RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
    '                                        Report, piva, HttpContext.Current.Session, objParametri_Server, rag_soc)

    '                            Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
    '                                                                   Enum_SiteRedirector.Sito_AgronicaAgenda_2010, ParametriAgronicaStampe)

    '                            r.Tipo = "1"
    '                            r.ParametroDue_stringa = strJS

    '                        End If



    '                    Else
    '                        'AgroMsgBox("Non si dispone dei permessi di stampa di questo report.", Page)
    '                        'Exit Function
    '                        r.RispostaOK = False
    '                        r.Errore = "Non si dispone dei permessi di stampa di questo report"

    '                    End If

    '                    'Exit Function

    '                    '========================================================================

    '                Case enum_CodificaStampe.Registri_Preparazioni


    '                    Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
    '                    Dim UtenteAbilitato As Boolean = objPermessi.Controlla_Permessi_Utente(
    '                                                             HttpContext.Current.Session("ASG_Utente_Username"), HttpContext.Current.Session("ASG_IdServizio"),
    '                                                             enum_Security_Attivita.Registri_Cantina, enum_Security_Operazione.Lettura,
    '                                                             Date.Now, "", objParametri_Utenti)

    '                    If UtenteAbilitato = True Then
    '                        piva = AgronicaCoreGestioneRichieste.RedirectGestione.ControllaNumeroImprese_per_SiteRedirector(objParametri_Server, objParametri_Utenti)
    '                        If piva = "" Then
    '                            'mando al filtrino
    '                            Dim Indirizzofiltrino As String = AgronicaCoreGestioneRichieste.RedirectGestione.GetLinkFiltrinoAgenda(
    '                                                                "../Stampe/MenuStampe.aspx", "../GestioneStampe/ChiamaStampe.aspx", Enum_SiteRedirector.Sito_AgronicaStampe, Report)
    '                            Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.PreparaScripPerPopup(Indirizzofiltrino, "Stampa")

    '                            r.Tipo = "1"
    '                            r.ParametroDue_stringa = strJS

    '                        Else
    '                            Dim objanag As New AgronicaCoreAnagrafeDAL.Imprese_Read
    '                            Dim rag_soc As String = objanag.RagSoc_from_Piva(piva, objParametri_Server)
    '                            Dim ParametriAgronicaStampe As AgronicaCoreGestioneRichieste.ParametriAgronicaStampe =
    '                                        AgronicaCoreGestioneRichieste.RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
    '                                        Report, piva, HttpContext.Current.Session, objParametri_Server, rag_soc)

    '                            Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
    '                                                             Enum_SiteRedirector.Sito_AgronicaAgenda_2010, ParametriAgronicaStampe)

    '                            r.Tipo = "1"
    '                            r.ParametroDue_stringa = strJS

    '                        End If

    '                    End If

    '                    'Exit Function

    '                    '========================================================================

    '                Case enum_CodificaStampe.Report_Incongruenze_CatastoVSAgrea

    '                    Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
    '                    Dim UtenteAbilitato As Boolean = objPermessi.Controlla_Permessi_Utente(
    '                                                             HttpContext.Current.Session("ASG_Utente_Username"), HttpContext.Current.Session("ASG_IdServizio"),
    '                                                             enum_Security_Attivita.Report_Incongruenze_CatastoVSAgrea, enum_Security_Operazione.Lettura,
    '                                                             Date.Now, "", objParametri_Utenti)

    '                    If UtenteAbilitato = True Then

    '                        piva = AgronicaCoreGestioneRichieste.RedirectGestione.ControllaNumeroImprese_per_SiteRedirector(objParametri_Server, objParametri_Utenti)
    '                        If piva = "" Then
    '                            'mando al filtrino
    '                            Dim Indirizzofiltrino As String = AgronicaCoreGestioneRichieste.RedirectGestione.GetLinkFiltrinoAgenda(
    '                                                                                        "../Stampe/MenuStampe.aspx", "../GestioneStampe/ChiamaStampe.aspx",
    '                                                                                        Enum_SiteRedirector.Sito_AgronicaStampe, Report)

    '                            Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.PreparaScripPerPopup(Indirizzofiltrino, "Stampa")

    '                            r.Tipo = "1"
    '                            r.ParametroDue_stringa = strJS

    '                        Else
    '                            Dim objanag As New AgronicaCoreAnagrafeDAL.Imprese_Read
    '                            Dim rag_soc As String = objanag.RagSoc_from_Piva(piva, objParametri_Server)
    '                            Dim ParametriAgronicaStampe As AgronicaCoreGestioneRichieste.ParametriAgronicaStampe =
    '                                                AgronicaCoreGestioneRichieste.RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
    '                                                Report, piva, HttpContext.Current.Session, objParametri_Server, rag_soc)

    '                            Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
    '                                                                      Enum_SiteRedirector.Sito_AgronicaAgenda_2010, ParametriAgronicaStampe)

    '                            r.Tipo = "1"
    '                            r.ParametroDue_stringa = strJS

    '                        End If


    '                    Else
    '                        'AgroMsgBox("Non si dispone dei permessi di stampa di questo report.", Page)
    '                        'Exit Function
    '                        r.RispostaOK = False
    '                        r.Errore = "Non si dispone dei permessi di stampa di questo report"
    '                    End If

    '                    'Exit Function

    '                    '========================================================================


    '                Case enum_CodificaStampe.Esportazione_OP_Catasto

    '                    Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
    '                    Dim UtenteAbilitato As Boolean = objPermessi.Controlla_Permessi_Utente(
    '                                                            HttpContext.Current.Session("ASG_Utente_Username"), HttpContext.Current.Session("ASG_IdServizio"),
    '                                                            enum_Security_Attivita.Stampe_Esportazione_OP_Catasto, enum_Security_Operazione.Modifica,
    '                                                            Date.Now, "", objParametri_Utenti)

    '                    If UtenteAbilitato = False Then
    '                        ' Messaggi.AgroMsgBox("Permesso negato!", Page, , CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanel_Menu"), UpdatePanel))
    '                        'Exit Function
    '                        r.RispostaOK = False
    '                    End If

    '                    Dim objp As New AgronicaCoreGestioneRichieste.ParametriFILTRONE_2010
    '                    objp.Sito_Origine = Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, AgroKey_EncoderDecoder, objParametri_Server)
    '                    objp.Pagina_Origine = Stringa_Codifica(OrigineM, AgroKey_EncoderDecoder, objParametri_Server)
    '                    objp.Sito_Destinazione = Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaStampe_2010, AgroKey_EncoderDecoder, objParametri_Server)
    '                    objp.Pagina_Destinazione = Stringa_Codifica("", AgroKey_EncoderDecoder, objParametri_Server)
    '                    objp.TipoFiltrone = Stringa_Codifica(CStr(enum_TipoFiltrone.Stampa), AgroKey_EncoderDecoder, objParametri_Server)
    '                    objp.CodificaStampe = Stringa_Codifica(enum_CodificaStampe.Esportazione_OP_Catasto, AgroKey_EncoderDecoder, objParametri_Server)

    '                    Dim link As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriFILTRONE_2010(
    '                                                           Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objp)

    '                    'Response.Redirect(link)
    '                    r.RispostaStringa = link


    '                Case enum_CodificaStampe.Esportazione_OP_Produttori

    '                    Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
    '                    Dim UtenteAbilitato As Boolean = objPermessi.Controlla_Permessi_Utente(
    '                                                         HttpContext.Current.Session("ASG_Utente_Username"), HttpContext.Current.Session("ASG_IdServizio"),
    '                                                         enum_Security_Attivita.Stampe_Esportazione_OP_Produttori, enum_Security_Operazione.Modifica,
    '                                                         Date.Now, "", objParametri_Utenti)

    '                    If UtenteAbilitato = False Then
    '                        ' Messaggi.AgroMsgBox("Permesso negato!", Page, , CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanel_Menu"), UpdatePanel))
    '                        'Exit Function
    '                        r.RispostaOK = False
    '                        r.Errore = "Permesso negato"
    '                    End If

    '                    Dim objp As New AgronicaCoreGestioneRichieste.ParametriFILTRONE_2010
    '                    objp.Sito_Origine = Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, AgroKey_EncoderDecoder, objParametri_Server)
    '                    objp.Pagina_Origine = Stringa_Codifica(OrigineM, AgroKey_EncoderDecoder, objParametri_Server)
    '                    objp.Sito_Destinazione = Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaStampe_2010, AgroKey_EncoderDecoder, objParametri_Server)
    '                    objp.Pagina_Destinazione = Stringa_Codifica("", AgroKey_EncoderDecoder, objParametri_Server)
    '                    objp.TipoFiltrone = Stringa_Codifica(CStr(enum_TipoFiltrone.Stampa), AgroKey_EncoderDecoder, objParametri_Server)
    '                    objp.CodificaStampe = Stringa_Codifica(enum_CodificaStampe.Esportazione_OP_Produttori, AgroKey_EncoderDecoder, objParametri_Server)

    '                    Dim link As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriFILTRONE_2010(
    '                                                           Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objp)

    '                    'Response.Redirect(link)
    '                    r.RispostaStringa = link

    '                Case enum_CodificaStampe.Quadro_P

    '                    Dim ParametriAgronicaStampe As AgronicaCoreGestioneRichieste.ParametriAgronicaStampe =
    '                      AgronicaCoreGestioneRichieste.RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
    '                                  Report, piva, HttpContext.Current.Session, objParametri_Server)

    '                    Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
    '                                                     Enum_SiteRedirector.Sito_AgronicaAgenda_2010, ParametriAgronicaStampe)
    '                    r.Tipo = "1"
    '                    r.ParametroDue_stringa = strJS
    '                    Return r

    '                Case enum_CodificaStampe.SchedaTracciabilita, enum_CodificaStampe.SchedaColturale_Biologico

    '                    Dim objp As New AgronicaCoreGestioneRichieste.ParametriFILTRONE_2010

    '                    objp.Sito_Origine = Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, AgroKey_EncoderDecoder, Nothing)
    '                    objp.Pagina_Origine = Stringa_Codifica("../Menu/MenuBs_Agenda_Nuovo.aspx", AgroKey_EncoderDecoder, Nothing)
    '                    objp.Sito_Destinazione = Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaStampe_2010, AgroKey_EncoderDecoder, Nothing)
    '                    objp.Pagina_Destinazione = Stringa_Codifica("../GestioneStampe/ChiamaStampe.aspx", AgroKey_EncoderDecoder, Nothing)
    '                    objp.TipoFiltrone = Stringa_Codifica(enum_TipoFiltrone.Stampa, AgroKey_EncoderDecoder, Nothing)
    '                    objp.CodificaStampe = Stringa_Codifica(Report, AgroKey_EncoderDecoder, Nothing)

    '                    Dim TargetRedirect = "../Filtrone/Filtrone_nuovo.aspx" &
    '                       "?p_o=" & objp.Pagina_Origine &
    '                       "&s_o=" & objp.Sito_Origine &
    '                       "&p_d=" & objp.Pagina_Destinazione &
    '                       "&s_d=" & objp.Sito_Destinazione &
    '                       "&t_f=" & objp.TipoFiltrone &
    '                       "&c_s=" & objp.CodificaStampe &
    '                       "&v_c=" & objp.Veg_Cod &
    '                       "&c_c=" & objp.Cul_Cod &
    '                       "&d_i=" & objp.Data_Inizio &
    '                       "&d_f=" & objp.Data_Fine

    '                    r.RispostaStringa = TargetRedirect
    '                    Return r

    '                Case enum_CodificaStampe.Bilancio_Fertilizzazioni, enum_CodificaStampe.Bilancio_Fertilizzazioni_Dettagliato

    '                    Dim objp As New AgronicaCoreGestioneRichieste.ParametriFILTRONE_2010

    '                    objp.Sito_Origine = Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, AgroKey_EncoderDecoder, Nothing)
    '                    objp.Pagina_Origine = Stringa_Codifica("../Menu/MenuBs_Agenda_Nuovo.aspx", AgroKey_EncoderDecoder, Nothing)
    '                    objp.Sito_Destinazione = Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaStampe_2010, AgroKey_EncoderDecoder, Nothing)
    '                    objp.Pagina_Destinazione = Stringa_Codifica("../GestioneStampe/ChiamaStampe.aspx", AgroKey_EncoderDecoder, Nothing)
    '                    objp.TipoFiltrone = Stringa_Codifica(enum_TipoFiltrone.Stampa, AgroKey_EncoderDecoder, Nothing)
    '                    objp.CodificaStampe = Stringa_Codifica(Report, AgroKey_EncoderDecoder, Nothing)

    '                    Dim TargetRedirect = "../Filtrone/Filtrone.aspx" &
    '                       "?p_o=" & objp.Pagina_Origine &
    '                       "&s_o=" & objp.Sito_Origine &
    '                       "&p_d=" & objp.Pagina_Destinazione &
    '                       "&s_d=" & objp.Sito_Destinazione &
    '                       "&t_f=" & objp.TipoFiltrone &
    '                       "&c_s=" & objp.CodificaStampe &
    '                       "&v_c=" & objp.Veg_Cod &
    '                       "&c_c=" & objp.Cul_Cod &
    '                       "&d_i=" & objp.Data_Inizio &
    '                       "&d_f=" & objp.Data_Fine

    '                    r.RispostaStringa = TargetRedirect
    '                    Return r

    '                Case Else

    '                    Dim debug As Boolean = True

    '                    ' ''Case enum_CodificaStampe.SchedaCampagna_2078
    '                    ' ''Case enum_CodificaStampe.RegistroTrattamenti
    '                    ' ''Case enum_CodificaStampe.SchedaRegistrazione
    '                    ' ''Case enum_CodificaStampe.RegistroTrattamenti_Veneto
    '                    ' ''Case enum_CodificaStampe.SchedaCampagna_2078_Semplificata
    '                    ' ''Case enum_CodificaStampe.SchedaCampagna_Multicentro
    '                    ' ''Case enum_CodificaStampe.RegistroTrattamenti_Semplificata
    '                    ' ''Case enum_CodificaStampe.SchedaRegistrazione_Semplificata
    '                    ' ''Case enum_CodificaStampe.Eurep_Gap
    '                    ' ''Case enum_CodificaStampe.Eurep_Gap_Semplificata
    '                    ' ''Case enum_CodificaStampe.Eurep_Gap_Multicentro


    '                    ' ''Case enum_CodificaStampe.Quadro_P
    '                    ' ''Case enum_CodificaStampe.PianoRaccolta
    '                    ' ''Case enum_CodificaStampe.SchedaColturale_Biologico
    '                    ' ''Case enum_CodificaStampe.RiepilogoImpiegoSuperfici_Multiazienda
    '                    ' ''Case enum_CodificaStampe.SchedaTracciabilita
    '                    ' ''Case enum_CodificaStampe.ReportConserveItalia
    '                    ' ''Case enum_CodificaStampe.SchedaCampagna_ConserveItalia
    '                    ' ''Case enum_CodificaStampe.DatiAnelloFilieraIngresso
    '                    ' ''Case enum_CodificaStampe.DatiAnelloFilieraLegameLotti
    '                    ' ''Case enum_CodificaStampe.EstrattoreDatiGrafici
    '                    ' ''Case enum_CodificaStampe.PianoColturale
    '                    ' ''Case enum_CodificaStampe.ReportRisultatoFilrone
    '                    ' ''Case enum_CodificaStampe.SchedaCampagna_Pizzoli



    '            End Select

    '            '--------------------------------------------------



    '            Dim objGiasOnline2 As New AgronicaCoreGestioneRichieste.ParametriGiasOnline
    '            objGiasOnline2.PaginaRichiesta = enum_PagineGiasOnline.FiltroImpresa_new4_menustampeagenda
    '            objGiasOnline2.Piva = objParametriAgenda.Piva

    '            objGiasOnline2.funzioneoriginedestinazionefiltrone = Funzione & "|" & Origine & "|" & Destinazione & "|" & Report

    '            If Not IsNothing(ConfigurationManager.AppSettings("LinkAgronicaAgenda2010")) AndAlso ConfigurationManager.AppSettings("LinkAgronicaAgenda2010") <> "" Then
    '                objGiasOnline2.LinkAgronicaAgenda2010 = ConfigurationManager.AppSettings("LinkAgronicaAgenda2010")
    '            Else
    '                objGiasOnline2.LinkAgronicaAgenda2010 = ""
    '            End If

    '            Dim strJS2 As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoOnline_PassandoDirettamente_ParametriGiasOnline(
    '                                      Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objGiasOnline2)

    '            r.Tipo = "1"
    '            r.ParametroDue_stringa = strJS2

    '            '----------------------------------------------------------------------------------


    '    End Select

    '    Return r

    'End Function

End Class
