Imports System.Web
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Text
Imports AgronicaCoreUtility
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreAnagrafeDAL
Imports Agronica.Helpers.GiasBase
Imports Newtonsoft.Json

Public Class MenuBS_2017_Buildingblocks

    Public CollocazioneJQuerySelector As String

    Public Contenutohtml As String

End Class

Public Class MenuBS_2017_RedirectGestione

    Public Shared Function MenuAttivo(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim lettureDB As New AgronicaCoreVarieDAL.MenuBS_2017_Operazioni_DB_R
        Return lettureDB.MenuAttivo(objParametri)
    End Function

    Public Shared Function LeggiImprese(ByVal ricerca As String, ByVal idSezione As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, Optional ByVal lenFiltro As Integer = 0) As List(Of MenuBS_2017_Buildingblocks)

        Dim dtLettura As DataTable
        Dim RispostaStringa = New List(Of MenuBS_2017_Buildingblocks)

        Dim Filtrone As New AgronicaCoreUtility.Filtrone
        Dim ClassJoin As New JoinFiltrone
        ClassJoin.bGerarchiaImprese = True

        ' Controllo su numero caratteri inseriti
        If lenFiltro > 0 AndAlso ricerca.Length < lenFiltro Then
            RispostaStringa.Add(New MenuBS_2017_Buildingblocks With {
                      .Contenutohtml = "<div class='alert alert-info buttonpreferitielementi' style='font-size: 14px !important;'>Inserire almeno " & lenFiltro & " caratteri</div>",
                      .CollocazioneJQuerySelector = ""
                    })
            Return RispostaStringa
        End If

        'Definisco il filtro sql
        Dim FiltroSQL = " (Imprese.Piva = '" & Agro_SQL_SaveText(ricerca.Trim) & "' OR IC_Cuaa.val_cod='" & Agro_SQL_SaveText(ricerca.Trim) & "' OR Imprese.Rag_Soc like '%" & Agro_SQL_SaveText(ricerca.Trim) & "%') "
        Filtrone.ImpostaVariabiliJOIN_xFiltroUtente(FiltroSQL, ClassJoin)

        dtLettura = Filtrone.CreaDTFiltrone(objParametri,
                                            FiltroSQL,
                                            enum_TipoSelect_FiltroneSuperNova.Imprese,
                                            "ORDER BY Imprese.rag_soc",
                                            ClassJoin)

        'leggo il risultato della query per ottenere la lista imprese
        If Not IsNothing(dtLettura) Then

            Dim colore As String = "#428bca"
            Dim testo As String
            Dim idhtml As String
            Dim PivaSelezionata As String
            Dim AziendaSelezionata As String

            For Each riga As DataRow In dtLettura.Rows

                testo = riga.Item("Rag_Soc")
                idhtml = "Piva_" & riga.Item("piva")
                PivaSelezionata = riga.Item("piva")
                AziendaSelezionata = riga.Item("Rag_Soc")

                Dim stbContenuto As New System.Text.StringBuilder
                stbContenuto.AppendLine("<div class='btn btn-default buttonpreferitielementi' style='background-color:" & colore & "' id='" & idhtml & "' aria-PivaSelezionata='" & PivaSelezionata & "' aria-AziendaSelezionata='" & HttpUtility.HtmlAttributeEncode(AziendaSelezionata) & "' aria-idSezione='" & idSezione & "' onclick='gestioneCambioImpresa(this);'>" & testo & "</div>")

                RispostaStringa.Add(New MenuBS_2017_Buildingblocks With {
                      .Contenutohtml = stbContenuto.ToString,
                      .CollocazioneJQuerySelector = ""
                    })

            Next

        Else

            RispostaStringa.Add(New MenuBS_2017_Buildingblocks With {
                      .Contenutohtml = "<h1> Errore nella lettura della imprese </h1>",
                      .CollocazioneJQuerySelector = ""
                    })

        End If

        Return RispostaStringa

    End Function

    Public Shared Function LeggiImpreseModello(ByVal ricerca As String, ByVal idSezione As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, Optional ByVal lenFiltro As Integer = 0) As List(Of AgronicaCoreModelsSTD.anagrafiche.Impresa)

        Dim dtLettura As DataTable
        Dim RispostaStringa = New List(Of AgronicaCoreModelsSTD.anagrafiche.Impresa)

        Dim Filtrone As New AgronicaCoreUtility.Filtrone
        Dim ClassJoin As New JoinFiltrone
        ClassJoin.bGerarchiaImprese = True

        'Definisco il filtro sql
        Dim FiltroSQL = " (Imprese.Piva = '" & Agro_SQL_SaveText(ricerca.Trim) & "' OR Imprese.partitaIvaReale = '" & Agro_SQL_SaveText(ricerca.Trim) & "' OR IC_Cuaa.val_cod='" & Agro_SQL_SaveText(ricerca.Trim) & "' OR Imprese.Rag_Soc like '%" & Agro_SQL_SaveText(ricerca.Trim) & "%') "
        Filtrone.ImpostaVariabiliJOIN_xFiltroUtente(FiltroSQL, ClassJoin)

        dtLettura = Filtrone.CreaDTFiltrone(objParametri,
                                            FiltroSQL,
                                            enum_TipoSelect_FiltroneSuperNova.Imprese,
                                            "ORDER BY Imprese.rag_soc",
                                            ClassJoin)

        'leggo il risultato della query per ottenere la lista imprese
        If Not IsNothing(dtLettura) Then

            Dim colore As String = "#428bca"
            Dim testo As String
            Dim idhtml As String
            Dim PivaSelezionata As String
            Dim AziendaSelezionata As String

            For Each riga As DataRow In dtLettura.Rows

                testo = riga.Item("Rag_Soc")
                idhtml = "Piva_" & riga.Item("piva")
                PivaSelezionata = riga.Item("piva")
                AziendaSelezionata = riga.Item("Rag_Soc")

                Dim cuaa As String = ""
                If Not IsDBNull(riga.Item("CodiceCuaa")) Then
                    cuaa = riga.Item("CodiceCuaa")
                End If

                Dim impresa As New AgronicaCoreModelsSTD.anagrafiche.Impresa
                impresa.partitaIva = PivaSelezionata
                impresa.ragioneSociale = AziendaSelezionata
                impresa.CUAA = cuaa
                RispostaStringa.Add(impresa)
            Next

        End If

        Return RispostaStringa

    End Function


    Public Shared Function LeggiSezioniPreferiti(ByVal IDSezionePadre As Integer, ByVal Ricerca As String, ByRef objParametri_Server As AgronicaCoreParametri, ByRef objParametri_Utenti As AgronicaCoreParametri) As List(Of MenuBS_2017_Buildingblocks)

        Dim lettureDB As New AgronicaCoreVarieDAL.MenuBS_2017_Operazioni_DB_R
        Dim dtLettura As DataTable

        Dim RispostaStringa = New List(Of MenuBS_2017_Buildingblocks)
        Dim stbContenuto As New StringBuilder

        'lettura delle sezioni per gestione preferiti
        dtLettura = lettureDB.LeggiSezioniMenu(IDSezionePadre, Ricerca, objParametri_Server, objParametri_Utenti)

        If Not IsNothing(dtLettura) Then
            Dim indicePreferito As String = ""
            Dim letturaPreferiti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim dtPreferiti = letturaPreferiti.Leggi(enum_Impostazioni_Utenti.UTENTE_LINK_PREFERITE_MENUBS2017, 1, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            If Not IsNothing(dtPreferiti) AndAlso dtPreferiti.Rows.Count > 0 Then
                indicePreferito = CStr(dtPreferiti.Rows(0).Item("Impostazione_Valore_1"))
            End If
            Dim indici = indicePreferito.Split("|")
            For Each riga As DataRow In dtLettura.Rows
                riga.Item("Testo") = "<span class='hidden-xs'>" & riga.Item("Padre") & " <i class='fa fa-arrow-right'></i></span> " & riga.Item("Testo")
                Dim isPreferito As Boolean = False
                For Each index In indici
                    If index = CStr(riga.Item("IDSezione")) Then
                        isPreferito = True
                        Exit For
                    End If
                Next
                If isPreferito Then
                    riga.Item("Testo") = riga.Item("Testo") & " <i class='fa fa-star-o' aria-hidden='true'></i>"
                End If

            Next
        End If


        GeneraContenutoDatatable(RispostaStringa, dtLettura, stbContenuto, "")

        Return RispostaStringa

    End Function

    Public Shared Function LeggiSezioni(ByVal IDTipoSezione As Integer,
                                        ByVal IDSezionePadre As Integer,
                                        ByVal Ricerca As String,
                                        ByVal Preferiti As String,
                                        ByVal NascondiMenu As Boolean,
                                        ByRef objParametri_Server As AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreParametri
                                        ) As List(Of MenuBS_2017_Buildingblocks)

        Dim lettureDB As New AgronicaCoreVarieDAL.MenuBS_2017_Operazioni_DB_R
        Dim dtLettura As DataTable

        Dim RispostaStringa = New List(Of MenuBS_2017_Buildingblocks)
        Dim stbContenuto As New StringBuilder

        'pulsanti del menu a sinistra
        'leggo le voci del menu
        If IDTipoSezione = 1 AndAlso IDSezionePadre = 0 Then

            Dim LinkGiasApp As String = ""
            Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim DTConfigSiti = objConfigSiti.Leggi(0, "LinkGiasApp", "", "", objParametri_Server)
            If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 Then
                LinkGiasApp = DTConfigSiti.Rows(0)("valore").ToString
            End If

            If Not String.IsNullOrEmpty(LinkGiasApp) Then
                If Not LinkGiasApp.StartsWith("http") Then
                    LinkGiasApp = "https://www.agronica.it/deposito/giasapp/" & LinkGiasApp
                End If
                GeneraBottone(0, "#689f38", "GIASapp", "buttonmenu faw-android", "menu_giasapp", 0, "", "", LinkGiasApp, stbContenuto)
                RispostaStringa.Add(New MenuBS_2017_Buildingblocks With {
                      .Contenutohtml = stbContenuto.ToString,
                      .CollocazioneJQuerySelector = "dashboard_menu"
                })
            End If

            dtLettura = lettureDB.letturaMenu(objParametri_Server)
            GeneraContenutoDatatable(RispostaStringa, dtLettura, stbContenuto, "dashboard_menu")

        End If

        'lettura delle macrosezioni da database
        dtLettura = lettureDB.LeggiSezioni(IDTipoSezione, IDSezionePadre, Ricerca, objParametri_Server, objParametri_Utenti)

        If IDTipoSezione = 2 AndAlso Not IsNothing(dtLettura) Then
            Dim indicePreferito As String = ""
            Dim letturaPreferiti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim dtPreferiti = letturaPreferiti.Leggi(enum_Impostazioni_Utenti.UTENTE_LINK_PREFERITE_MENUBS2017, 1, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            If Not IsNothing(dtPreferiti) AndAlso dtPreferiti.Rows.Count > 0 Then
                indicePreferito = CStr(dtPreferiti.Rows(0).Item("Impostazione_Valore_1"))
            End If
            Dim indici = indicePreferito.Split("|")
            For Each riga As DataRow In dtLettura.Rows
                Dim isPreferito As Boolean = False
                For Each index In indici
                    If index = CStr(riga.Item("IDSezione")) Then
                        isPreferito = True
                        Exit For
                    End If
                Next
                If isPreferito Then
                    riga.Item("Testo") = riga.Item("Testo") & " <i class='fa fa-star-o' aria-hidden='true'></i>"
                ElseIf Preferiti = "true" Then
                    riga.Item("Testo") = ""
                End If
            Next
        End If

        GeneraContenutoDatatable(RispostaStringa, dtLettura, stbContenuto, "", NascondiMenu)

        Return RispostaStringa

    End Function


    Public Shared Function LeggiSezioniModello(ByVal IDTipoSezione As Integer,
                                        ByVal IDSezionePadre As Integer,
                                        ByVal Ricerca As String,
                                        ByVal Preferiti As String,
                                        ByVal NascondiMenu As Boolean,
                                        ByRef objParametri_Server As AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreParametri
                                        ) As List(Of AgronicaCoreModelsSTD.Menu.LinkMenu)

        Dim lettureDB As New AgronicaCoreVarieDAL.MenuBS_2017_Operazioni_DB_R
        Dim dtLettura As DataTable

        Dim RispostaStringa = New List(Of AgronicaCoreModelsSTD.Menu.LinkMenu)
        Dim stbContenuto As New StringBuilder

        'pulsanti del menu a sinistra            
        'leggo le voci del menu
        If IDTipoSezione = 1 AndAlso IDSezionePadre = 0 Then

            Dim LinkGiasApp As String = ""
            Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim DTConfigSiti = objConfigSiti.Leggi(0, "LinkGiasApp", "", "", objParametri_Server)
            If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 Then
                LinkGiasApp = DTConfigSiti.Rows(0)("valore").ToString
            End If

            If Not String.IsNullOrEmpty(LinkGiasApp) Then
                If Not LinkGiasApp.StartsWith("http") Then
                    LinkGiasApp = "https://www.agronica.it/deposito/giasapp/" & LinkGiasApp
                End If
                GeneraBottone(0, "#689f38", "GIASapp", "buttonmenu faw-android", "menu_giasapp", 0, "", "", LinkGiasApp, stbContenuto)
                Dim linkMenu As New AgronicaCoreModelsSTD.Menu.LinkMenu
                linkMenu.idSezione = 0
                linkMenu.colore = "#689f38"
                linkMenu.testo = "GIASapp"
                linkMenu.idHtml = "menu_giasapp"
                linkMenu.richiedeAziendaSelezionata = 0
                linkMenu.redirectUrl = LinkGiasApp
                RispostaStringa.Add(linkMenu)
            End If

            dtLettura = lettureDB.letturaMenu(objParametri_Server)
            RispostaStringa.AddRange(GeneraContenutoDatatableModello(dtLettura, stbContenuto, "dashboard_menu"))

        End If

        'lettura delle macrosezioni da database
        dtLettura = lettureDB.LeggiSezioni(IDTipoSezione, IDSezionePadre, Ricerca, objParametri_Server, objParametri_Utenti)

        dtLettura.Columns.Add("preferito", GetType(Integer))

        If IDTipoSezione = 2 AndAlso Not IsNothing(dtLettura) Then
            Dim indicePreferito As String = ""
            Dim letturaPreferiti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim dtPreferiti = letturaPreferiti.Leggi(enum_Impostazioni_Utenti.UTENTE_LINK_PREFERITE_MENUBS2017, 1, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            If Not IsNothing(dtPreferiti) AndAlso dtPreferiti.Rows.Count > 0 Then
                indicePreferito = CStr(dtPreferiti.Rows(0).Item("Impostazione_Valore_1"))
            End If
            Dim indici = indicePreferito.Split("|")
            For Each riga As DataRow In dtLettura.Rows
                Dim isPreferito As Boolean = False
                For Each index In indici
                    If index = CStr(riga.Item("IDSezione")) Then
                        isPreferito = True
                        Exit For
                    End If
                Next
                If isPreferito Then
                    riga.Item("preferito") = 1
                ElseIf Preferiti = "true" Then
                    riga.Item("Testo") = ""
                    riga.Item("preferito") = 1
                Else
                    riga.Item("preferito") = 0
                End If
            Next
        End If

        RispostaStringa.AddRange(GeneraContenutoDatatableModello(dtLettura, stbContenuto, "", NascondiMenu))

        Return RispostaStringa

    End Function

    Public Shared Function LeggiWidgetAllarmi(ByRef objParametri_Server As AgronicaCoreParametri) As String

        Dim lettureDB As New AgronicaCoreVarieDAL.MenuBS_2017_Operazioni_DB_R
        Dim dtLettura As DataTable

        Dim IDWidgetAllarmi As Integer
        Dim colore As String
        Dim idhtml As String
        Dim testo As String
        Dim classecss As String

        Dim stringaHTML As New System.Text.StringBuilder
        stringaHTML.Length = 0

        'leggo i widget degli allarmi
        dtLettura = lettureDB.leggiWidgetAllarmi(objParametri_Server)

        If Not IsNothing(dtLettura) Then
            For Each riga As DataRow In dtLettura.Rows
                IDWidgetAllarmi = riga.Item("IDWidget_Allarme")
                colore = riga.Item("Colore")
                testo = riga.Item("Testo")
                classecss = riga.Item("ClasseCSS")
                idhtml = riga.Item("id_html")
                GeneraBottone(0, colore, testo, classecss, idhtml, 0, 0, 0, "", stringaHTML)
            Next
        Else
            stringaHTML.Append("<h1> Errore nella lettura delle voci del menu </h1>")
        End If

        Return stringaHTML.ToString

    End Function

    Public Shared Sub GeneraContenutoDatatable(ByRef RispostaStringa As List(Of MenuBS_2017_Buildingblocks), ByVal dtLettura As DataTable, ByRef stbContenuto As StringBuilder, ByVal colonnaBootstrap As String, Optional ByVal nascondiMenu As Boolean = True)

        Dim colore As String
        Dim testo As String
        Dim classeCSS As String
        Dim idhtml As String
        Dim colonnaBootstrap2 As String
        Dim idSezione As Integer = 0
        Dim RedirectURL As String = ""
        Dim sitoRichiesto As Integer = 0
        Dim paginaRichiesta As Integer = 0
        Dim richiedeAziendaSelezionata As Integer = 0
        Dim menuDisattivato As Boolean = True

        'leggo il risultato della query per ottenere le macrosezioni
        If Not IsNothing(dtLettura) Then
            For Each riga As DataRow In dtLettura.Rows
                colore = riga.Item("Colore")
                testo = riga.Item("Testo")
                classeCSS = riga.Item("ClasseCSS")
                idhtml = riga.Item("id_html")

                menuDisattivato = nascondiMenu AndAlso classeCSS.Contains("disattivato")
                If String.IsNullOrEmpty(testo) OrElse menuDisattivato Then
                    Continue For
                End If

                If dtLettura.Columns.Contains("Enum_SiteRedirector") Then
                    sitoRichiesto = riga.Item("Enum_SiteRedirector")
                End If

                If dtLettura.Columns.Contains("PaginaRichiesta") Then
                    paginaRichiesta = riga.Item("PaginaRichiesta")
                End If

                If dtLettura.Columns.Contains("IDSezione") Then
                    idSezione = riga.Item("IDSezione")
                End If

                If dtLettura.Columns.Contains("richiedeAziendaSelezionata") Then
                    richiedeAziendaSelezionata = riga.Item("richiedeAziendaSelezionata")
                End If

                If dtLettura.Columns.Contains("RedirectURL") Then
                    RedirectURL = riga.Item("RedirectURL").ToString
                End If

                If colonnaBootstrap = "" Then
                    colonnaBootstrap2 = riga.Item("colonna_bootstrap")
                Else
                    colonnaBootstrap2 = colonnaBootstrap
                End If

                stbContenuto.Length = 0
                GeneraBottone(idSezione, colore, testo, classeCSS, idhtml, richiedeAziendaSelezionata, sitoRichiesto, paginaRichiesta, RedirectURL, stbContenuto)

                RispostaStringa.Add(New MenuBS_2017_Buildingblocks With {
                      .Contenutohtml = stbContenuto.ToString,
                      .CollocazioneJQuerySelector = colonnaBootstrap2
                })

            Next

        Else
            stbContenuto.Append("<h1> Errore nella lettura della macrosezione </h1>")
        End If

    End Sub

    Public Shared Function GeneraContenutoDatatableModello(ByVal dtLettura As DataTable, ByRef stbContenuto As StringBuilder, ByVal colonnaBootstrap As String, Optional ByVal nascondiMenu As Boolean = True) As List(Of AgronicaCoreModelsSTD.Menu.LinkMenu)

        Dim colore As String
        Dim testo As String
        Dim classeCSS As String
        Dim idhtml As String
        Dim menuDisattivato As Boolean = True
        Dim list As New List(Of AgronicaCoreModelsSTD.Menu.LinkMenu)
        'leggo il risultato della query per ottenere le macrosezioni
        If Not IsNothing(dtLettura) Then
            For Each riga As DataRow In dtLettura.Rows
                colore = riga.Item("Colore")
                testo = riga.Item("Testo")
                idhtml = riga.Item("id_html")
                classeCSS = riga.Item("ClasseCSS")

                menuDisattivato = nascondiMenu AndAlso classeCSS.Contains("disattivato")
                If String.IsNullOrEmpty(testo) OrElse menuDisattivato Then
                    Continue For
                End If

                Dim linkMenu As New AgronicaCoreModelsSTD.Menu.LinkMenu
                linkMenu.colore = colore
                linkMenu.testo = testo
                linkMenu.idHtml = idhtml

                If dtLettura.Columns.Contains("Enum_SiteRedirector") Then
                    linkMenu.sitoRichiesto = riga.Item("Enum_SiteRedirector")
                End If

                If dtLettura.Columns.Contains("PaginaRichiesta") Then
                    linkMenu.paginaRichiesta = riga.Item("PaginaRichiesta")
                End If

                If dtLettura.Columns.Contains("IDSezione") Then
                    linkMenu.idSezione = riga.Item("IDSezione")
                End If

                If dtLettura.Columns.Contains("richiedeAziendaSelezionata") Then
                    linkMenu.richiedeAziendaSelezionata = riga.Item("richiedeAziendaSelezionata")
                End If

                If dtLettura.Columns.Contains("RedirectURL") Then
                    linkMenu.redirectUrl = riga.Item("RedirectURL").ToString
                End If

                If dtLettura.Columns.Contains("preferito") Then
                    linkMenu.preferito = riga.Item("preferito")
                End If

                list.Add(linkMenu)

            Next

        End If

        Return list

    End Function

    Public Shared Sub GeneraBottone(ByVal idSezione As Integer, colore As String, testo As String, classeCSS As String, idhtml As String, richiedeAziendaSelezionata As Integer, sitoRichiesto As String, paginaRichiesta As String, RedirectURL As String, stringaHTML As StringBuilder)

        Dim strRichiedeAziendaSelezionata As String = "false"
        If richiedeAziendaSelezionata = 1 Then
            strRichiedeAziendaSelezionata = "true"
        End If

        Dim linkGiasBase As String = ""
        Dim objParametriServer As AgronicaCoreParametri = Nothing

        GiasBaseHelper.Setta_Link_GiasBase("ASG_objParametri_Server", linkGiasBase)


        ' aggiunta gestione pulsanti disattivati
        Dim azione As String = "onclick='gestioneMacrosezioneSezione(this);'"
        Dim stile As String = "background-color: " & colore & ";"
        If classeCSS.Contains("disattivato") Then
            azione = "disabled='disabled'"
            stile &= "background-color:#cccccc;"
            'stile &= "border-color: red !important;"
        End If

        ' aggiunge icona / immagine
        If classeCSS.Contains(" faw-") Then
            Dim icona = classeCSS.Substring(classeCSS.IndexOf(" faw-") + 5)
            classeCSS = classeCSS.Replace(" faw-" & icona, "")
            testo = "<i class='fa fa-" & icona & "'></i> " & testo
        ElseIf classeCSS.Contains(" ico-") Then
            Dim icona = classeCSS.Substring(classeCSS.IndexOf(" ico-") + 5)
            classeCSS = classeCSS.Replace(" ico-" & icona, "")
            testo = "<img src='" & linkGiasBase & "agronica/AB_Immagini/icone24/" & icona & ".ico' /> " & testo
        ElseIf classeCSS.Contains(" img-") Then
            Dim icona = classeCSS.Substring(classeCSS.IndexOf(" img-") + 5)
            classeCSS = classeCSS.Replace(" img-" & icona, "")
            testo = "<img class='" & icona & "' /> " & testo
        End If

        stringaHTML.AppendLine("<div class='btn btn-default " & classeCSS & "' id='" & idhtml & "' style='" & stile & "' aria-RichiedeAziendaSelezionata='" & strRichiedeAziendaSelezionata & "' aria-sitoRichiesto='" & sitoRichiesto & "' aria-paginaRichiesta='" & paginaRichiesta & "' aria-idSezione='" & idSezione & "' aria-RedirectURL='" & RedirectURL & "' " & azione & ">" & testo & "</div>")

    End Sub

    Public Shared Sub RedirectGenerico(ByVal Piva As String, sitoRichiesto As Integer, ByVal paginaRichiesta As Integer, ByRef RedirectURL As String, ByVal objParametri_Server As AgronicaCoreParametri,
                                       Optional ByVal Parametri_Aggiuntivi As JObject = Nothing, Optional ByVal SitoOrigine As Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaAgenda_2010)

        Dim Utente_CodFiscale As String = ""
        If HttpContext.Current.Session("ASG_Utente_CodFiscale") IsNot Nothing Then
            Utente_CodFiscale = HttpContext.Current.Session("ASG_Utente_CodFiscale").ToString
        End If

        If HttpContext.Current.Session("ASG_objParametri_Server") Is Nothing Then
            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
        End If

        Select Case sitoRichiesto

            Case Enum_SiteRedirector.Sito_AgronicaPlanning

                Dim ParametriPlanning As New ParametriPlanning
                ParametriPlanning.PaginaProvenienza = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
                ParametriPlanning.PaginaRichiesta = paginaRichiesta
                ParametriPlanning.Cuaa = CUAA_from_PIVA(objParametri_Server, Piva)
                ParametriPlanning.Piva = Piva
                RedirectURL = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_Sito_AgronicaPlanning_PassandoDirettamente_ParametriPlanning(SitoOrigine, ParametriPlanning)

            Case Enum_SiteRedirector.Sito_AgronicaSincronizzatore

                Dim Parametrisincronizzatore_2010 As New AgronicaCoreGestioneRichieste.ParametriSincronizzatore_2010
                Parametrisincronizzatore_2010.Piva = Piva
                Parametrisincronizzatore_2010.Pagina_Richiesta = paginaRichiesta
                If Not String.IsNullOrEmpty(RedirectURL) Then
                    Parametrisincronizzatore_2010.ParametriQueryString = RedirectURL
                End If
                Parametrisincronizzatore_2010.Id_Cod_Cliente = 0
                Parametrisincronizzatore_2010.Salva()

                RedirectURL = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoSincronizzatore_PassandoDirettamente_ParametriSincro_2010(
                                        SitoOrigine,
                                        Parametrisincronizzatore_2010)

                'RedirectURL = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_Sitosincronizzatore_PassandoDirettamenteIParametri(
                '                        Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                '                        paginaRichiesta,
                '                        enum_PagineAgenda_2010.Menu_BS,
                '                        Piva,
                '                        0)

            Case Enum_SiteRedirector.Sito_PianoConcimazione_2017

                Dim objConcimazione As New AgronicaCoreGestioneRichieste.ParametriConcimazione_2017
                objConcimazione.Pagina_Richiesta = paginaRichiesta 'enum_PaginePianoConcimazione_2017.MenuBS
                objConcimazione.Piva = Piva
                objConcimazione.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
                objConcimazione.Pagina_SitoOrigine = enum_PagineAgenda_2010.Menu_BS

                RedirectURL = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoPianoConcimazione_PassandoDirettamente_ParametriConcimazione_2017(
                SitoOrigine,
                objConcimazione)

            Case Enum_SiteRedirector.Sito_AgronicaAudit

                Dim Audit_Tipo = IIf(paginaRichiesta = 0, enum_AuditPuaTipo.Audit_Condizionalita, paginaRichiesta)
                RedirectURL = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_Sito_AgronicaAudit_PassandoDirettamente_Parametri(
                    SitoOrigine,
                    Audit_Tipo,
                    Utente_CodFiscale,
                    Piva, 0)

            Case Enum_SiteRedirector.Sito_AgronicaGlobalGAP

                RedirectURL = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_Sito_AgronicaAuditSicurezzaGlobalCoop_PassandoDirettamente_Parametri(
                    SitoOrigine,
                    enum_AuditPuaTipo.Audit_GlobalGap,
                    Utente_CodFiscale,
                    Piva, 0)

            Case Enum_SiteRedirector.Sito_AgronicaCheckCOOP

                RedirectURL = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_Sito_AgronicaAuditSicurezzaGlobalCoop_PassandoDirettamente_Parametri(
                    SitoOrigine,
                    paginaRichiesta,
                    Utente_CodFiscale,
                    Piva, 0)

            Case Enum_SiteRedirector.Sito_AgronicaSicurezzaLavoro

                RedirectURL = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_Sito_AgronicaAuditSicurezzaGlobalCoop_PassandoDirettamente_Parametri(
                    SitoOrigine,
                    enum_AuditPuaTipo.Audit_SicurezzaLavoro,
                    Utente_CodFiscale,
                    Piva, 0)

            Case Enum_SiteRedirector.Sito_AgronicaProfilazione

                Dim ParametriProfilazione As New ParametriProfilazione_2010

                ParametriProfilazione.Globale = False 'da testare se true o false
                ParametriProfilazione.Pagina_Richiesta = paginaRichiesta 'in questo modo prendo sia "profilazione imprese=1" che "utenti e permessi=4"
                ParametriProfilazione.Piva = Piva
                ParametriProfilazione.SitoRichiesto = Enum_SiteRedirector.Sito_AgronicaProfilazione
                ParametriProfilazione.Username = "" 'objParametri_Server.UtenteUsername

                RedirectURL = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoProfilazione_PassandoDirettamente_ParametriProfilazione_2010(SitoOrigine, ParametriProfilazione)

            Case Enum_SiteRedirector.Sito_AgronicaLabQualita

                Dim ParametriLabCQ As New AgronicaCoreGestioneRichieste.ParametriLabCQ

                RedirectURL = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_Sito_AgronicaLabCQ_PassandoDirettamente_ParametriLabCQ(
                            SitoOrigine,
                            ParametriLabCQ)

            Case Enum_SiteRedirector.Sito_AgronicaPianiCampionamento

                RedirectURL = RedirectGestione.IndirizzoCompleto_SitoPianiCampionamento_PassandoDirettamenteIParametri(
                    SitoOrigine, paginaRichiesta, 0,
                    Nothing, 0, Piva)

            Case Enum_SiteRedirector.Sito_AgronicaAnalisi_2010

                Dim ParametriAnalisi_2010 As New AgronicaCoreGestioneRichieste.ParametriAnalisi_2010
                ParametriAnalisi_2010.Piva = Piva
                ParametriAnalisi_2010.Tipo_Operazione = enum_TipoOperazioneDB.Lettura 'va settato?
                ParametriAnalisi_2010.Pagina_Richiesta = If(paginaRichiesta = 0, enum_PagineAnalisi_2010.Pagina_Analisi, paginaRichiesta)
                ParametriAnalisi_2010.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
                If paginaRichiesta <> enum_PagineAnalisi_2010.Pagina_Gestione_Laboratori Then
                    ParametriAnalisi_2010.Tipo_Analisi = If(paginaRichiesta = 0, enum_AnalisiTipo.Analisi_Fitofarmaci, paginaRichiesta)
                End If

                RedirectURL = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAnalisi_2010_PassandoDirettamente_ParametriAnalisi_2010(SitoOrigine, ParametriAnalisi_2010)

            Case Enum_SiteRedirector.Sito_GiasOnline

                Dim ParametriGiasOnline As New ParametriGiasOnline

                ParametriGiasOnline.PaginaRichiesta = paginaRichiesta
                ParametriGiasOnline.Piva = Piva

                RedirectURL = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline_PassandoDirettamente_ParametriGiasOnline(SitoOrigine, ParametriGiasOnline)

            Case Enum_SiteRedirector.Sito_AgronicaPianiSemina

                RedirectURL = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoPianiSemina_PassandoDirettamenteIParametri(
                            SitoOrigine,
                            0,
                            enum_PagineAgenda_2010.Pagina_FiltrinoImprese)

            Case Enum_SiteRedirector.Sito_GiasOnline_2010

                RedirectURL = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri(
                                            SitoOrigine,
                                            enum_PagineGiasOnline_2010.giassmart_punti,
                                            enum_PagineAgenda_2010.Menu, Piva, "", "", 0, "")

            Case Enum_SiteRedirector.Sito_AgronicaStampe_2010

                'Dim ParametriAgronicaStampe As AgronicaCoreGestioneRichieste.ParametriAgronicaStampe =
                '            AgronicaCoreGestioneRichieste.RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                '                        paginaRichiesta, Piva, HttpContext.Current.Session, objParametri_Server)

                'RedirectURL = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                '                                         Enum_SiteRedirector.Sito_AgronicaAgenda_2010, ParametriAgronicaStampe)

            Case Enum_SiteRedirector.Sito_AgronicaBio

                'enum_CodificaPagBio.PAP_Vegetale
                RedirectURL = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_Sito_AgronicaBio_PassandoDirettamente_Parametri(
                        SitoOrigine,
                        paginaRichiesta,
                        Utente_CodFiscale,
                        Piva)

            Case Enum_SiteRedirector.Sito_AgronicaPUA

                'enum_AuditPuaTipo.PUA
                RedirectURL = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_Sito_AgronicaPUA_PassandoDirettamente_Parametri(
                    SitoOrigine,
                    paginaRichiesta,
                    Utente_CodFiscale,
                    Piva, 0)

            Case Enum_SiteRedirector.Sito_AgronicaAgenda_2010

                Dim objAgenda As New AgronicaCoreGestioneRichieste.ParametriAgenda_2010
                objAgenda.PaginaRichiesta = paginaRichiesta
                objAgenda.Piva = Piva
                RedirectURL = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriAgenda_2010(SitoOrigine, objAgenda)

            Case Enum_SiteRedirector.GiasNG

                ' devo recuperare la ragione sociale della piva selezionata in modo che arrivi sempre al GiasNG
                Dim ragSoc As String = ""
                If Piva <> "" Then
                    Dim objImprese As New Imprese_Read
                    Dim dtImprese As DataTable = objImprese.Leggi(Piva, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                    ragSoc = String.Empty
                    If Not IsNothing(dtImprese) AndAlso dtImprese.Rows.Count > 0 Then
                        Dim r As DataRow = dtImprese.Rows(0)
                        ragSoc = If(r.Item("Rag_Soc") Is DBNull.Value, "", r.Item("Rag_Soc"))
                    End If
                End If
                Dim objAgenda As New AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG
                objAgenda.Pagina_Richiesta = paginaRichiesta
                objAgenda.RagSoc = ragSoc
                objAgenda.Piva = Piva
                objAgenda.IdSezione = DammiIdSezione(paginaRichiesta, sitoRichiesto)
                Dim Lav_Cod = getValueParametriAggiuntivi(Parametri_Aggiuntivi, "Lav_Cod")

                If Not IsNothing(Lav_Cod) AndAlso Not String.IsNullOrEmpty(Lav_Cod) Then
                    objAgenda.Lav_Cod = CInt(Lav_Cod)
                End If

                Dim Lav_Des = getValueParametriAggiuntivi(Parametri_Aggiuntivi, "Lav_Des")

                If Not IsNothing(Lav_Des) AndAlso Not String.IsNullOrEmpty(Lav_Des) Then
                    objAgenda.Lav_Des = Lav_Des
                End If

                Dim TipoOperazioneDB = getValueParametriAggiuntivi(Parametri_Aggiuntivi, "TipoOperazioneDB")

                If Not IsNothing(TipoOperazioneDB) AndAlso Not String.IsNullOrEmpty(TipoOperazioneDB) Then
                    objAgenda.TipoOperazioneDB = CInt(TipoOperazioneDB)
                End If

                Dim queryStringFiltrino = getValueParametriAggiuntivi(Parametri_Aggiuntivi, "QSF")
                If Not IsNothing(queryStringFiltrino) AndAlso Not String.IsNullOrEmpty(queryStringFiltrino) Then
                    objAgenda.QueryStringFiltrino = queryStringFiltrino
                End If

                Dim Id_Agenda = getValueParametriAggiuntivi(Parametri_Aggiuntivi, "Id_Agenda")
                If Not IsNothing(Id_Agenda) AndAlso Not String.IsNullOrEmpty(Id_Agenda) Then
                    objAgenda.Id_Agenda = CInt(Id_Agenda)
                End If

                Dim Ricetta_Cod = getValueParametriAggiuntivi(Parametri_Aggiuntivi, "Ricetta_Cod")
                If Not IsNothing(Ricetta_Cod) AndAlso Not String.IsNullOrEmpty(Ricetta_Cod) Then
                    objAgenda.Ricetta_Cod = CInt(Ricetta_Cod)
                End If

                Dim Ricetta_Operazione_Cod = getValueParametriAggiuntivi(Parametri_Aggiuntivi, "Ricetta_Operazione_Cod")
                If Not IsNothing(Ricetta_Operazione_Cod) AndAlso Not String.IsNullOrEmpty(Ricetta_Operazione_Cod) Then
                    objAgenda.Ricetta_Operazione_Cod = CInt(Ricetta_Operazione_Cod)
                End If

                Dim Pagina_Provenienza = getValueParametriAggiuntivi(Parametri_Aggiuntivi, "Pagina_Provenienza")
                If Not IsNothing(Pagina_Provenienza) AndAlso Not String.IsNullOrEmpty(Pagina_Provenienza) Then
                    objAgenda.Pagina_Provenienza = CInt(Pagina_Provenienza)
                End If

                Dim Pagina_Provenienza_AltroSito = getValueParametriAggiuntivi(Parametri_Aggiuntivi, "Pagina_Provenienza_AltroSito")
                If Not IsNothing(Pagina_Provenienza_AltroSito) AndAlso Not String.IsNullOrEmpty(Pagina_Provenienza_AltroSito) Then
                    objAgenda.Pagina_Provenienza_AltroSito = CInt(Pagina_Provenienza_AltroSito)
                End If

                If SitoOrigine <> Enum_SiteRedirector.GiasNG Then

                    If IsNothing(objAgenda.Pagina_Provenienza_AltroSito) OrElse objAgenda.Pagina_Provenienza_AltroSito = 0 Then
                        If objAgenda.Pagina_Provenienza > 0 Then
                            objAgenda.Pagina_Provenienza_AltroSito = objAgenda.Pagina_Provenienza
                        Else
                            objAgenda.Pagina_Provenienza_AltroSito = enum_PagineAgenda_2010.Menu
                        End If
                    End If

                    objAgenda.Pagina_Provenienza = 0
                Else

                    If IsNothing(objAgenda.Pagina_Provenienza) OrElse objAgenda.Pagina_Provenienza = 0 Then
                        If objAgenda.Pagina_Provenienza_AltroSito > 0 Then
                            objAgenda.Pagina_Provenienza = objAgenda.Pagina_Provenienza_AltroSito
                        End If
                    End If

                    objAgenda.Pagina_Provenienza_AltroSito = 0
                End If

                Dim TipoOperazioneAgenda = getValueParametriAggiuntivi(Parametri_Aggiuntivi, "TipoOperazioneAgenda")
                If Not IsNothing(TipoOperazioneAgenda) AndAlso Not String.IsNullOrEmpty(TipoOperazioneAgenda) Then
                    objAgenda.TipoOperazioneAgenda = CInt(TipoOperazioneAgenda)
                End If

                Dim TipoRicetta = getValueParametriAggiuntivi(Parametri_Aggiuntivi, "TipoRicetta")
                If Not IsNothing(TipoRicetta) AndAlso Not String.IsNullOrEmpty(TipoRicetta) Then
                    objAgenda.TipoRicetta = CInt(TipoRicetta)
                End If

                Dim Stato = getValueParametriAggiuntivi(Parametri_Aggiuntivi, "Stato")
                If Not IsNothing(Stato) AndAlso Not String.IsNullOrEmpty(Stato) Then
                    objAgenda.Stato = CInt(Stato)
                End If

                Dim Regolamento_Cod = getValueParametriAggiuntivi(Parametri_Aggiuntivi, "Regolamento_Cod")
                If Not IsNothing(Regolamento_Cod) AndAlso Not String.IsNullOrEmpty(Regolamento_Cod) Then
                    objAgenda.Regolamento_Cod = CInt(Regolamento_Cod)
                End If

                Dim Tipo_Regolamento = getValueParametriAggiuntivi(Parametri_Aggiuntivi, "Tipo_Regolamento")
                If Not IsNothing(Tipo_Regolamento) AndAlso Not String.IsNullOrEmpty(Tipo_Regolamento) Then
                    objAgenda.Tipo_Regolamento = CInt(Tipo_Regolamento)
                End If

                Dim GenericObj_string = getValueParametriAggiuntivi(Parametri_Aggiuntivi, "GenericObj_string")
                If Not IsNothing(GenericObj_string) AndAlso Not String.IsNullOrEmpty(GenericObj_string) Then
                    objAgenda.GenericObj_string = GenericObj_string
                End If

                Dim Impianti = getValueParametriAggiuntivi(Parametri_Aggiuntivi, "Impianti")
                If Not IsNothing(Impianti) AndAlso Not String.IsNullOrEmpty(Impianti) Then
                    objAgenda.Impianti = JsonConvert.DeserializeObject(Of List(Of AgronicaCoreGestioneRichieste.ImpiantiAgendaNG))(Impianti)
                End If

                objAgenda.Sito_Provenienza = SitoOrigine

                objAgenda.Salva()
                RedirectURL = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoGiasNG_PassandoDirettamente_ParametriAgenda_NG(SitoOrigine, objAgenda)

            Case Enum_SiteRedirector.Sito_AgronicaUma

                Dim objAgenda As New AgronicaCoreGestioneRichieste.ParametriAgenda_2010
                objAgenda.PaginaRichiesta = paginaRichiesta
                objAgenda.Piva = Piva
                objAgenda.Salva()
                RedirectURL = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgronicaUMA_PassandoDirettamente_ParametriAgenda_2010(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objAgenda)

            Case Enum_SiteRedirector.Sito_AgronicaDomandaIrrigua
                Dim objAgenda As New AgronicaCoreGestioneRichieste.ParametriDomandaIrrigua
                objAgenda.PaginaRichiesta = paginaRichiesta
                objAgenda.Piva = Piva
                objAgenda.Salva()
                RedirectURL = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_Sito_AgronicaDomandaIrrigua_PassandoDirettamente_ParametriDomandaIrrigua(Enum_SiteRedirector.Sito_AgronicaDomandaIrrigua, objAgenda)

            Case Else

        End Select

    End Sub

    Public Shared Function CUAA_from_PIVA(ByRef objParametri_Server As AgronicaCoreParametri,
                                    ByVal Piva As String) As String


        Dim DT_Impresa As DataTable

        Dim leggiI As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        DT_Impresa = leggiI.Leggi(
            Piva,
            1010,
            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
            "",
            "",
            objParametri_Server
        )

        If Not IsNothing(DT_Impresa) Then
            If DT_Impresa.Rows.Count <> 0 Then
                Return DT_Impresa.Rows(0).Item("val_cod")
            Else
                Return ""
            End If
        Else
            Return ""
        End If

    End Function

    Public Shared Function getValueParametriAggiuntivi(ByVal ParametriAggiuntivi As JObject, ByVal Chiave As String) As String

        Dim Value As String = ""

        If Not IsNothing(ParametriAggiuntivi) AndAlso Not String.IsNullOrEmpty(Chiave) Then
            If Not IsNothing(ParametriAggiuntivi(Chiave)) Then
                Value = ParametriAggiuntivi(Chiave).ToString()
            End If
        End If

        Return Value

    End Function

    Private Shared Function DammiIdSezione(ByVal paginaRichiesta As Integer, ByVal sitoRichiesto As Integer) As Integer

        Dim context As HttpContext = HttpContext.Current
        Dim IDSezione = 0
        If paginaRichiesta = enum_PagineGiasNG.Pagina_Dashboard Then
            IDSezione = 0
        Else
            If context.Session.Item("ASG_MenuBS_2017") IsNot Nothing Then
                Dim s As String = CType(context.Session("ASG_MenuBS_2017"), String)
                Dim oggettoSessioneMenu As JObject = JObject.Parse(s)

                Dim paginaRichiestaSessione = CInt(oggettoSessioneMenu("paginaRichiesta"))
                Dim sitoRichiestoSessione = CInt(oggettoSessioneMenu("sitoRichiesto"))

                If paginaRichiesta <> paginaRichiestaSessione OrElse sitoRichiesto <> sitoRichiestoSessione Then
                    Dim lettureDB As New AgronicaCoreVarieDAL.MenuBS_2017_Operazioni_DB_R
                    IDSezione = lettureDB.LeggiIDSezione(sitoRichiesto, paginaRichiesta, HttpContext.Current.Session("ASG_objParametri_Server"), HttpContext.Current.Session("ASG_objParametri_Utenti"))
                Else
                    IDSezione = CInt(oggettoSessioneMenu("IDSezione"))
                End If

            End If
        End If

        Return IDSezione

    End Function

End Class
