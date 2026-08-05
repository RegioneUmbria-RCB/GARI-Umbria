Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza

Public Class RisultatoEsportazione_XLS
    Inherits System.Web.UI.Page

    Protected WithEvents TableExcel As System.Web.UI.HtmlControls.HtmlTable


#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTE: The following placeholder declaration is required by the Web Form Designer.
    'Do not delete or move it.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region

    '#########################################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'La Pagina deve essere visualizzata come un foglio Excel
        Response.ContentType = "application/vnd.ms-excel"
        Response.AddHeader("Content-Disposition", "inline; filename = RisultatoEsportazione.xls")


        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato

        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If

        '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

        Dim UtenteAbilitato As Boolean
        'Dim strDummy As String      'controllo accesso negato.....

        'UtenteAbilitato = Controlla_Permessi_Utente_2( _
        '                            Server, Session, Page, _
        '                            Session("ASG_Utente_Username"), _
        '                            Session("ASG_IdServizio"), _
        '                            TipiEnumerativi.enum_Security_Attivita.Stampe_Esportatore_Universale, _
        '                            TipiEnumerativi.enum_Security_Operazione.Modifica, _
        '                            strDummy)

        '----- Se l'utente non ha il permesso per visualizzare la pagina ... lo invio al menu.

        '----- !!!!!!!!!!! -------------
        'Attivazione forzata provvisoria

        UtenteAbilitato = True
        '----- !!!!!!!!!!! -------------

        If Not UtenteAbilitato Then
            Response.Redirect("../../Messaggi/AccessoNegato.htm")
        End If

        Dim scheda, nomefileoutput As String

        nomefileoutput = Stringa_Decodifica(Request.QueryString("a").ToString, _
                                       AgroKey_EncoderDecoder, _
                                       Server)

        scheda = Stringa_Decodifica(Request.QueryString("t").ToString, _
                                       AgroKey_EncoderDecoder, _
                                       Server)

        Dim Dt_Finale As DataTable

        Dt_Finale = Session("DT_Finale")

        Session("DT_Finale") = Nothing

        '##############################################################
        '#####  Costruisco la tabella   ###############################
        '##############################################################

        Dim i, j, k As Integer
        Dim Riga As HtmlTableRow

        AgronicaCoreDataProvider.UtilityProvider.ElaboraCellaHTML(Me.TableExcel.Rows(0).Cells(0),
                                                                  2, "", "",
                                                                  "Yellow",
                                                                  "left", "middle")

        Select Case scheda

            Case "impianti"
                Me.TableExcel.Rows(0).Cells(0).InnerHtml = "Esportazione Impianti"
            Case "imprese"
                Me.TableExcel.Rows(0).Cells(0).InnerHtml = "Esportazione Imprese"
            Case "centri"
                Me.TableExcel.Rows(0).Cells(0).InnerHtml = "Esportazione Centri Aziendali"
            Case "agenda"
                Me.TableExcel.Rows(0).Cells(0).InnerHtml = "Esportazione Operazioni d'Agenda"

        End Select

        Me.TableExcel.Rows(0).Cells(0).Style.Item("font-weight") = "bold"
        Me.TableExcel.Rows(0).Cells(0).Style.Item("font-size") = "18px"
        Me.TableExcel.Rows(0).Cells(0).Style.Item("vertical-align") = "middle"
        Me.TableExcel.Rows(0).Cells(0).Style.Item("border-top-width") = "1px"

        If Not IsNothing(Dt_Finale) Then

            If scheda = "impianti" Then
                If Dt_Finale.Columns.Contains("Veg_Cod") Then
                    Dt_Finale.Columns.Remove("Veg_Cod")
                End If

                If Dt_Finale.Columns.Contains("Cul_Cod") Then
                    Dt_Finale.Columns.Remove("Cul_Cod")
                End If

                If Dt_Finale.Columns.Contains("id_Impianto") Then
                    Dt_Finale.Columns.Remove("id_Impianto")
                End If

            End If

            Me.TableExcel.Rows(0).Cells(0).ColSpan = CInt(Dt_Finale.Columns.Count)

            '------------------------------------------
            '------------- INTESTAZIONE ---------------
            '------------------------------------------
            Riga = New HtmlTableRow

            For k = 0 To Dt_Finale.Columns.Count - 1

                Riga.Cells.Add(New HtmlTableCell)
                AgronicaCoreDataProvider.UtilityProvider.ElaboraCellaHTML(Riga.Cells(k),
                                                                          2, "", "",
                                                                          "Gainsboro",
                                                                          "center", "middle")
                Riga.Cells(k).Style.Item("vertical-align") = "middle"
                Riga.Cells(k).Style.Item("font-weight") = "bold"
                Riga.Cells(k).Style.Item("font-size") = "12px"
                'Riga.Cells(k).Height = "26"

                'Riga.Cells(k).InnerHtml = Dt_Finale.Rows(CInt(Dt_Finale.Rows.Count - 1)).Item(k)
                Riga.Cells(k).InnerHtml = Dt_Finale.Columns.Item(k).Caption

                Select Case CStr(Riga.Cells(k).InnerHtml).ToLower

                    Case "PIVA", "piva"
                        Riga.Cells(k).InnerHtml = "Partita IVA<br/>Impresa"
                    Case "SA_COD", "sa_cod"
                        Riga.Cells(k).InnerHtml = "Codice Centro"
                    Case "campo_cod"
                        Riga.Cells(k).InnerHtml = "Codice Campo"
                    Case "APPEZZA", "appezza"
                        Riga.Cells(k).InnerHtml = "Codice<br/>Appezzamento"
                    Case "ID_REG", "id_dest"
                        Riga.Cells(k).InnerHtml = "Codice Impianto"
                    Case "rag_soc"
                        Riga.Cells(k).InnerHtml = "Ragione Sociale Impresa"
                    Case "Codice_Socio", "codice_socio"
                        Riga.Cells(k).InnerHtml = "Codice Socio"
                    Case "coop_padre"
                        Riga.Cells(k).InnerHtml = "Impresa Padre"
                    Case "PIVA_padre", "piva_padre"
                        Riga.Cells(k).InnerHtml = "Partita IVA<br/>Impresa Padre"
                    Case "imp_ind_des"
                        Riga.Cells(k).InnerHtml = "Indirizzo<br/>Impresa"
                    Case "imp_frz_des"
                        Riga.Cells(k).InnerHtml = "Frazione<br/>Impresa"
                    Case "imp_cap"
                        Riga.Cells(k).InnerHtml = "CAP<br/>Impresa"
                    Case "imp_com_des"
                        Riga.Cells(k).InnerHtml = "Comune<br/>Impresa"
                    Case "imp_pro_cod"
                        Riga.Cells(k).InnerHtml = "Prov<br/>Impresa"
                    Case "cen_ind_des"
                        Riga.Cells(k).InnerHtml = "Indirizzo<br/>Centro"
                    Case "cen_frz_des"
                        Riga.Cells(k).InnerHtml = "Frazione<br/>Centro"
                    Case "cen_cap"
                        Riga.Cells(k).InnerHtml = "CAP<br/>Centro"
                    Case "cen_com_des"
                        Riga.Cells(k).InnerHtml = "Comune<br/>Centro"
                    Case "cen_pro_cod"
                        Riga.Cells(k).InnerHtml = "Prov<br/>Centro"
                    Case "pro_cod_istat"
                        Riga.Cells(k).InnerHtml = "Prov ISTAT<br/>Centro"
                    Case "com_cod_istat"
                        Riga.Cells(k).InnerHtml = "Com ISTAT<br/>Centro"
                    Case "sa_nome"
                        Riga.Cells(k).InnerHtml = "Nome Centro"
                    Case "campo_des"
                        Riga.Cells(k).InnerHtml = "Nome Campo"
                    Case "app_nome"
                        Riga.Cells(k).InnerHtml = "Nome<br/>Appezzamento"
                    Case "sup_app"
                        Riga.Cells(k).InnerHtml = "Superficie<br/>Appezzamento"
                    Case "sup_trattata"
                        Riga.Cells(k).InnerHtml = "Superficie<br/>Trattata"
                    Case "qta_totale"
                        Riga.Cells(k).InnerHtml = "Quantita'<br/>Totale [Kg/L]"
                    Case "gru_cod"
                        Riga.Cells(k).InnerHtml = "Codice<br />Gruppo Vegetale"
                    Case "gru_des"
                        Riga.Cells(k).InnerHtml = "Gruppo Vegetale"
                    Case "veg_cod"
                        Riga.Cells(k).InnerHtml = "Codice<br/>Specie Vegetale"
                    Case "veg_des"
                        Riga.Cells(k).InnerHtml = "Specie Vegetale"
                    Case "cul_cod"
                        Riga.Cells(k).InnerHtml = "Codice Varieta'"
                    Case "cul_des"
                        Riga.Cells(k).InnerHtml = "Varieta'"
                    Case "grva_des"
                        Riga.Cells(k).InnerHtml = "Tipologia Varietale"
                    Case "grfi_des"
                        Riga.Cells(k).InnerHtml = "Finalita'"
                    Case "cop_des"
                        Riga.Cells(k).InnerHtml = "Tipo Copertura"
                    Case "reg_des"
                        Riga.Cells(k).InnerHtml = "Regolamento"
                    Case "sup_imp"
                        Riga.Cells(k).InnerHtml = "Superficie<br />Impianto [Ha]"
                    Case "inizio_impianto"
                        Riga.Cells(k).InnerHtml = "Data Inizio<br/>Impianto"
                    Case "fine_impianto"
                        Riga.Cells(k).InnerHtml = "Data Fine<br/>Impianto"
                    Case "inizio_appezza"
                        Riga.Cells(k).InnerHtml = "Data Inizio<br/>Appezzamento"
                    Case "fine_appezza"
                        Riga.Cells(k).InnerHtml = "Data Fine<br/>Appezzamento"
                    Case "lav_cod_imp"
                        Riga.Cells(k).InnerHtml = "Operazione<br/>Semina / Trapianto"
                    Case "lav_cod"
                        Riga.Cells(k).InnerHtml = "Codice Operazione"
                    Case "cau_mov"
                        Riga.Cells(k).InnerHtml = "Causale Movimento"
                    Case "id_agenda"
                        Riga.Cells(k).InnerHtml = "Codice Agenda"
                    Case "des_lib"
                        Riga.Cells(k).InnerHtml = "Descrizione Operazione"
                    Case "gruppo_operazione"
                        Riga.Cells(k).InnerHtml = "Gruppo Operazione"
                    Case "data_operazione"
                        Riga.Cells(k).InnerHtml = "Data Operazione"
                    Case "data_semina"
                        Riga.Cells(k).InnerHtml = "Data<br/>Semina / Trapianto"
                    Case "part_regione"
                        Riga.Cells(k).InnerHtml = "Regione"
                    Case "part_pro_cod"
                        Riga.Cells(k).InnerHtml = "Cod<br/>Prov"
                    Case "part_com_des"
                        Riga.Cells(k).InnerHtml = "Descr<br/>Comune"
                    Case "PROV", "prov"
                        Riga.Cells(k).InnerHtml = "Provincia"
                    Case "COM", "com"
                        Riga.Cells(k).InnerHtml = "Comune"
                    Case "SEZIONE", "sezione"
                        Riga.Cells(k).InnerHtml = "Sezione"
                    Case "FOGLIO", "foglio"
                        Riga.Cells(k).InnerHtml = "Foglio"
                    Case "NUMERO", "numero"
                        Riga.Cells(k).InnerHtml = "Numero"
                    Case "SUBALTERNO", "subalterno"
                        Riga.Cells(k).InnerHtml = "Subalterno"
                    Case "AREA", "area"
                        Riga.Cells(k).InnerHtml = "Superficie d'Intersezione<br/>con Particella [Ha]"
                    Case "rappr_legale"
                        Riga.Cells(k).InnerHtml = "Rappresentante Legale"
                    Case "CF_legale", "cf_legale"
                        Riga.Cells(k).InnerHtml = "Codice Fiscale<br/>Rappresentante Legale"
                    Case "com_legale"
                        Riga.Cells(k).InnerHtml = "Comune Nascita<br/>Rappresentante Legale"
                    Case "pro_legale"
                        Riga.Cells(k).InnerHtml = "Provincia Nascita<br/>Rappresentante Legale"
                    Case "tipo_impresa"
                        Riga.Cells(k).InnerHtml = "Tipo Impresa"
                    Case "tipo_centro"
                        Riga.Cells(k).InnerHtml = "Tipo Centro"
                    Case "inizio_impresa"
                        Riga.Cells(k).InnerHtml = "Data Inizio Impresa"
                    Case "fine_impresa"
                        Riga.Cells(k).InnerHtml = "Data Fine Impresa"
                    Case "inizio_centro"
                        Riga.Cells(k).InnerHtml = "Data Inizio Centro"
                    Case "fine_centro"
                        Riga.Cells(k).InnerHtml = "Data Fine Centro"
                    Case "TitoloPossesso", "titolopossesso"
                        Riga.Cells(k).InnerHtml = "Titolo Possesso<br/>Particella"
                    Case "possesso_centro"
                        Riga.Cells(k).InnerHtml = "Titolo Possesso<br/>Centro"
                    Case "tipo_attivita"
                        Riga.Cells(k).InnerHtml = "Tipo Attivita'<br/>"
                    Case "dal"
                        Riga.Cells(k).InnerHtml = "Data Inizio Possesso<br />Particella"
                    Case "al"
                        Riga.Cells(k).InnerHtml = "Data Fine Possesso<br />Particella"
                    Case "ETTARI", "ettari"
                        Riga.Cells(k).InnerHtml = "Superficie [HA]<br />Particella"
                    Case "ARE", "are"
                        Riga.Cells(k).InnerHtml = "Superficie [AA]<br />Particella"
                    Case "CENTIARE", "centiare"
                        Riga.Cells(k).InnerHtml = "Superficie [CA]<br />Particella"
                    Case "cod_ote"
                        Riga.Cells(k).InnerHtml = "Orientamento<br/>Tenico Economico"
                    Case "cod_operatore"
                        Riga.Cells(k).InnerHtml = "Codice Operatore"
                    Case "cod_zoo"
                        Riga.Cells(k).InnerHtml = "Codice Zooprofilattico"
                    Case "cod_cerpl"
                        Riga.Cells(k).InnerHtml = "Codice CERPL"
                    Case "cod_aua"
                        Riga.Cells(k).InnerHtml = "Codice AUA"
                    Case "cod_ausl"
                        Riga.Cells(k).InnerHtml = "Codice AUSL"
                    Case "cod_cnal"
                        Riga.Cells(k).InnerHtml = "Codice CNAL"
                    Case "fabbricato"
                        Riga.Cells(k).InnerHtml = "Nome Fabbricato"
                    Case "tipo_fabbricato"
                        Riga.Cells(k).InnerHtml = "Tipo Fabbricato"
                    Case "sup_totale"
                        Riga.Cells(k).InnerHtml = "Superficie Totale [Ha]<br/>(somma particelle)"
                    Case "sup_sau"
                        Riga.Cells(k).InnerHtml = "SAU Totale [Ha]<br/>(somma appezzamenti)"
                    Case "sup_tara"
                        Riga.Cells(k).InnerHtml = "Tara [Ha]<br/>(Sup Totale - SAU Totale)"
                    Case "sau_convenz"
                        Riga.Cells(k).InnerHtml = "SAU Convenzionale [Ha]"
                    Case "sau_convers"
                        Riga.Cells(k).InnerHtml = "SAU in Conversione [Ha]"
                    Case "sau_bio"
                        Riga.Cells(k).InnerHtml = "SAU Biologico [Ha]"
                    Case "sup_bosco"
                        Riga.Cells(k).InnerHtml = "Superficie Bosco [Ha]"
                    Case "sup_prato"
                        Riga.Cells(k).InnerHtml = "Superficie Prato [Ha]"
                    Case "mappa"
                        Riga.Cells(k).InnerHtml = "Mappe associate al<br/>Centro Aziendale"
                    Case "organismi"
                        Riga.Cells(k).InnerHtml = "Organismi di Controllo Biologico"
                    Case "capitolato_privato"
                        Riga.Cells(k).InnerHtml = "Capitolato<br/>Privato"
                    Case "dett_specie_pers"
                        Riga.Cells(k).InnerHtml = "Dettaglio<br/>Specie<br/>Personalizzato"
                    Case "foral_des"
                        Riga.Cells(k).InnerHtml = "Forma<br/>Allevamento"
                    Case "resa_prevista"
                        Riga.Cells(k).InnerHtml = "Resa<br/>Prevista [Kg]"
                    Case "P_HA", "p_ha"
                        Riga.Cells(k).InnerHtml = "Num.<br/>Piante/Ha"
                    Case "P_Tot", "p_tot"
                        Riga.Cells(k).InnerHtml = "Num.<br/>Piante Tot"
                    Case "tra_fila_maschio"
                        Riga.Cells(k).InnerHtml = "Distanza<br/>Tra Fila"
                    Case "su_fila_maschio"
                        Riga.Cells(k).InnerHtml = "Distanza<br/>Su Fila"
                    Case "coop_referente"
                        Riga.Cells(k).InnerHtml = "Organismo<br/>Referente"
                    Case "lotto_distinta"
                        Riga.Cells(k).InnerHtml = "Lotto<br/>Distinta"
                    Case "inizio_distinta"
                        Riga.Cells(k).InnerHtml = "Data Inizio<br/>Distinta"
                    Case "fine_distinta"
                        Riga.Cells(k).InnerHtml = "Data Fine<br/>Distinta"
                    Case "Bloccato", "bloccato"
                        Riga.Cells(k).InnerHtml = "Operazione<br/>Bloccata"
                    Case "Data_Bloccato", "data_bloccato"
                        Riga.Cells(k).InnerHtml = "Data Blocco<br/>Operazione"
                    Case "Tecnico_Blocco", "tecnico_blocco"
                        Riga.Cells(k).InnerHtml = "Tecnico Blocco<br/>Operazione"
                    Case "Categoria", "categoria"
                        Riga.Cells(k).InnerHtml = "Categoria Prodotto"
                    Case "Prodotto", "prodotto"
                        Riga.Cells(k).InnerHtml = "Prodotto / Materia Prima"
                    Case "Udm_Des", "udm_des"
                        Riga.Cells(k).InnerHtml = "Unita' di<br/>Misura"
                    Case "Qta", "qta"
                        Riga.Cells(k).InnerHtml = "Quantita'"
                    Case "tecnico"
                        Riga.Cells(k).InnerHtml = "Tecnico di<br/>Riferimento"
                    Case "dest_uso"
                        Riga.Cells(k).InnerHtml = "Destinazione<br/>d'uso"
                    Case "port_des"
                        Riga.Cells(k).InnerHtml = "Portinnesto"
                    Case "imp_des"
                        Riga.Cells(k).InnerHtml = "Impianto Irrigazione"
                    Case "magazzino_conf"
                        Riga.Cells(k).InnerHtml = "Magazzino<br/>Conferimento"
                    Case "principiattivi"
                        Riga.Cells(k).InnerHtml = "Principi<br/>attivi"
                    Case "org_referente"
                        Riga.Cells(k).InnerHtml = "Organismo<br/>Referente"
                    Case "ind_des"
                        Riga.Cells(k).InnerHtml = "Indirizzo"
                    Case "frz_des"
                        Riga.Cells(k).InnerHtml = "Frazione"
                    Case "com_des"
                        Riga.Cells(k).InnerHtml = "Comune"
                    Case "pro_cod"
                        Riga.Cells(k).InnerHtml = "Provincia"
                    Case "metodoproduzione_des"
                        Riga.Cells(k).InnerHtml = "Metodo di<br/>Produzione"
                End Select
            Next

            Me.TableExcel.Rows.Add(Riga)
            '----------------------------------
            '------- FINE INTESTAZIONE --------
            '----------------------------------

            '----------------------------------
            '------ RIEMPIMENTO TABELLA -------
            '----------------------------------

            'scorro le righe
            For i = 0 To Dt_Finale.Rows.Count - 1

                Riga = New HtmlTableRow

                'scorro le colonne
                For j = 0 To Dt_Finale.Columns.Count - 1

                    'aggiungo la cella
                    Riga.Cells.Add(New HtmlTableCell)
                    Riga.Cells(j).Style.Item("text-align") = "center"
                    Riga.Cells(j).Style.Item("vertical-align") = "middle"
                    'Riga.Cells(j).Height = "26"

                    Select Case Dt_Finale.Columns.Item(j).Caption

                        Case "PIVA", "Piva", "piva", "SA_COD", "sa_cod", "APPEZZA", "appezza", "ID_REG", "id_dest", "CUAA", "PIVA_padre", "Codice_Socio", "pro_cod_istat", "com_cod_istat", "PROV", "COM"
                            'aggiungo uno spazio davanti x salvare gli zeri...
                            Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dt_Finale.Rows(i).Item(j)), "&nbsp;" & Dt_Finale.Rows(i).Item(j), "&nbsp;")

                        Case "lav_cod_imp"

                            Select Case Dt_Finale.Rows(i).Item(j)

                                Case 71

                                    Riga.Cells(j).InnerHtml = "Trapianto"

                                Case 2

                                    Riga.Cells(j).InnerHtml = "Semina"

                                Case -1

                                    Riga.Cells(j).InnerHtml = " "

                            End Select


                        Case "FOGLIO", "NUMERO", "ETTARI", "ARE", "CENTIARE", "sup_imp", "veg_cod", "campo_cod", "AREA", _
                        "sup_totale", "sup_sau", "sup_tara", "sau_convenz", "sau_convers", "sau_bio", "sup_bosco", "sup_prato", _
                        "gru_cod", "cul_cod"

                            If Dt_Finale.Rows(i).Item(j) = -1 Then
                                Riga.Cells(j).InnerHtml = " "
                            Else
                                Riga.Cells(j).InnerHtml = Dt_Finale.Rows(i).Item(j)
                            End If

                        Case "tipo_impresa"

                            Select Case Dt_Finale.Rows(i).Item(j)

                                Case -1
                                    Riga.Cells(j).InnerHtml = " "

                                Case 1
                                    Riga.Cells(j).InnerHtml = "Impresa"

                                Case 2
                                    Riga.Cells(j).InnerHtml = "Cooperativa"

                                Case 3
                                    Riga.Cells(j).InnerHtml = "Consorzio"

                                Case 4
                                    Riga.Cells(j).InnerHtml = "OP"

                            End Select

                        Case "tipo_attivita"

                            Select Case Dt_Finale.Rows(i).Item(j)

                                Case "PV"
                                    Riga.Cells(j).InnerHtml = "Produzione vegetale"

                                Case "PZ"
                                    Riga.Cells(j).InnerHtml = "Produzione zootecnica"

                                Case "PVZ"
                                    Riga.Cells(j).InnerHtml = "Produzione vegetale e zootecnica"

                                Case "TPV"
                                    Riga.Cells(j).InnerHtml = "Preparazione vegetale"

                                Case "TPZ"
                                    Riga.Cells(j).InnerHtml = "Preparazione zootecnica"

                                Case "TPVZ"
                                    Riga.Cells(j).InnerHtml = "Preparazione vegetale e zootecnica"

                                Case "I"
                                    Riga.Cells(j).InnerHtml = "Importazione"

                                Case "RS"
                                    Riga.Cells(j).InnerHtml = "Raccolta spontanea"

                                Case "P/TP"
                                    Riga.Cells(j).InnerHtml = "Produzione / Preparazione"

                                Case "TP/I"
                                    Riga.Cells(j).InnerHtml = "Preparazione / Importazione"

                                Case "@"
                                    Riga.Cells(j).InnerHtml = "Altro"

                            End Select


                        Case "TitoloPossesso", "possesso_centro"

                            Select Case Dt_Finale.Rows(i).Item(j)

                                Case -1
                                    Riga.Cells(j).InnerHtml = " "

                                Case 0
                                    Riga.Cells(j).InnerHtml = "Altro"

                                Case 1
                                    Riga.Cells(j).InnerHtml = "Proprieta'"

                                Case 2
                                    Riga.Cells(j).InnerHtml = "Comodato d'uso"

                                Case 3
                                    Riga.Cells(j).InnerHtml = "Affitto con contratto"

                                Case 4
                                    Riga.Cells(j).InnerHtml = "Affitto senza contratto"

                                Case 5
                                    Riga.Cells(j).InnerHtml = "In conto terzi"

                            End Select

                        Case "pro_legale"

                            If (Dt_Finale.Rows(i).Item(j) = "0") Then

                                Riga.Cells(j).InnerHtml = " "

                            Else
                                Riga.Cells(j).InnerHtml = Dt_Finale.Rows(i).Item(j)

                            End If

                        Case "organismi"

                            Riga.Cells(j).Width = "480"
                            Riga.Cells(j).InnerHtml = Dt_Finale.Rows(i).Item(j)

                            'Case "veg_des"
                            '    If Dt_Finale.Rows(i).Item("cul_des") = " " Then
                            '        'se c'è la superficie, ma non la varietà, significa che è terreno nudo
                            '        If CStr(Dt_Finale.Rows(i).Item("sup_imp")) <> " " Then
                            '            Riga.Cells(j).InnerHtml = "Terreno Nudo"
                            '        End If
                            '    Else
                            '        Riga.Cells(j).InnerHtml = Dt_Finale.Rows(i).Item(j)
                            '    End If

                            'Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dt.Rows(i).Item(j)), Dt.Rows(i).Item(j), "Terreno Nudo")
                            'Riga.Cells(j).Align = "center"

                        Case "inizio_distinta", "inizio_impianto", "inizio_centro", "inizio_impresa", "inizio_appezza", "data_semina"
                            If Dt_Finale.Rows(i).Item(j) = "01/01/1900" Then
                                Riga.Cells(j).InnerHtml = "..."
                            Else
                                Riga.Cells(j).InnerHtml = Dt_Finale.Rows(i).Item(j)
                                'Dt_Finale.Rows(i).Item(j) = Format(Dt_Finale.Rows(i).Item(j), "dd/MM/yyyy")
                            End If

                        Case "fine_distinta", "fine_impianto", "fine_centro", "fine_impresa", "fine_appezza"
                            If Dt_Finale.Rows(i).Item(j) = "31/12/2100" Then
                                Riga.Cells(j).InnerHtml = "..."
                            Else
                                Riga.Cells(j).InnerHtml = Dt_Finale.Rows(i).Item(j)
                                'Dt_Finale.Rows(i).Item(j) = Format(Dt_Finale.Rows(i).Item(j), "dd/MM/yyyy")
                            End If

                        Case Else

                            Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dt_Finale.Rows(i).Item(j)), Dt_Finale.Rows(i).Item(j), "&nbsp;")

                            '    Case 3
                            '        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dt.Rows(i).Item(j)), Dt.Rows(i).Item(j), "&nbsp;")
                            '        Riga.Cells(j).Align = "center"                        

                            '    Case 6

                            '        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dt.Rows(i).Item(j)), Dt.Rows(i).Item(j), "Non Definita")


                            '    Case 9
                            '        'stampo la data nel formato short
                            '        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dt.Rows(i).Item(j)), Dt.Rows(i).Item(j), "&nbsp;")
                            '        If Riga.Cells(j).InnerHtml <> "&nbsp;" Then
                            '            Riga.Cells(j).InnerHtml = CDate(Riga.Cells(j).InnerHtml).ToShortDateString
                            '        End If                    

                    End Select

                Next

                'Aggiungo la Riga alla Tabella 
                Me.TableExcel.Rows.Add(Riga)

            Next
            '-------------------------------
            '--------- FINE TABELLA --------
            '-------------------------------

        End If


        Session("DT_Finale") = Nothing


    End Sub

End Class