Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class AnagraficaContatti_XLS
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
        Response.AddHeader("Content-Disposition", "inline; filename = AnagraficaContatti.xls")
        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato

        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If


        ''----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

        'Dim UtenteAbilitato As Boolean
        'Dim strDummy As String      'controllo accesso negato.....

        'UtenteAbilitato = Controlla_Permessi_Utente_2( _
        '                            Server, Session, Page, _
        '                            Session("ASG_Utente_Username"), _
        '                            Session("ASG_IdServizio"), _
        '                            TipiEnumerativi.enum_Security_Attivita.Gest_Prodotti, _
        '                            TipiEnumerativi.enum_Security_Operazione.Lettura, _
        '                            strDummy)

        ''----- Se l'utente non ha il permesso per visualizzare la pagina ... lo invio al menu.
        'If UtenteAbilitato = False Then
        '    Response.Redirect("../../Messaggi/AccessoNegato.htm")
        'End If

        'Dim nomefileoutput As String
        'nomefileoutput = Stringa_Decodifica(Request.QueryString("nfo").ToString, _
        '                               AgroKey_EncoderDecoder, _
        '                               Server)

        Dim Dt_Finale As DataTable
        Dim Riga As HtmlTableRow
        Dim Numero_Colonne As Integer = 0

        Dt_Finale = Session("DT_Finale")

        If IsNothing(Dt_Finale) Then
            Riga = New HtmlTableRow
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(0).ColSpan = Numero_Colonne
            Riga.Cells(0).InnerHtml = "Non è arrivato alcun dato dal filtro dei prodotti."
            Me.TableExcel.Rows.Add(Riga)
            Exit Sub
        End If

        Numero_Colonne = CInt(Dt_Finale.Columns.Count)

        Session("DT_Finale") = Nothing


        '##############################################################
        '#####  Costruisco la tabella   ###############################
        '##############################################################

        Try

            Dim i, j, k As Integer
            Dim Colonna_Dt As String

            AgronicaCoreDataProvider.UtilityProvider.ElaboraCellaHTML(Me.TableExcel.Rows(0).Cells(0), 2, "", "", "Yellow", "left", "middle")
            Me.TableExcel.Rows(0).Cells(0).InnerHtml = "Anagrafica Contatti"
            Me.TableExcel.Rows(0).Cells(0).Style.Item("font-weight") = "bold"
            Me.TableExcel.Rows(0).Cells(0).Style.Item("font-size") = "18px"
            Me.TableExcel.Rows(0).Cells(0).Style.Item("vertical-align") = "middle"
            Me.TableExcel.Rows(0).Cells(0).Style.Item("border-top-width") = "1px"

            If Not IsNothing(Dt_Finale) Then

                Me.TableExcel.Rows(0).Cells(0).ColSpan = CInt(Dt_Finale.Columns.Count)

                '------------------------------------------
                '------------- INTESTAZIONE ---------------
                '------------------------------------------
                Riga = New HtmlTableRow

                For k = 0 To Dt_Finale.Columns.Count - 1

                    Riga.Cells.Add(New HtmlTableCell)
                    AgronicaCoreDataProvider.UtilityProvider.ElaboraCellaHTML(Riga.Cells(k), 2, "", "", "Gainsboro", "center", "middle")
                    Riga.Cells(k).Style.Item("vertical-align") = "middle"
                    Riga.Cells(k).Style.Item("font-weight") = "bold"
                    Riga.Cells(k).Style.Item("font-size") = "12px"
                    'Riga.Cells(k).Height = "26"

                    'Riga.Cells(k).InnerHtml = Dt_Finale.Rows(CInt(Dt_Finale.Rows.Count - 1)).Item(k)
                    Riga.Cells(k).InnerHtml = Dt_Finale.Columns.Item(k).Caption

                    Colonna_Dt = Riga.Cells(k).InnerHtml.ToLower

                    Select Case Colonna_Dt

                        Case "piva"
                            Riga.Cells(k).InnerHtml = "P.IVA Impresa<br/>creatore del Contatto"
                        Case "impresa"
                            Riga.Cells(k).InnerHtml = "Ragione Sociale Impresa<br/>creatore del Contatto"
                        Case "cod_contatto"
                            Riga.Cells(k).InnerHtml = "P.IVA P.G./<br/>Codice Fiscale P.F."
                        Case "codice_fiscale"
                            Riga.Cells(k).InnerHtml = "Codice Fiscale<br/>P.G."
                        Case "rag_soc"
                            Riga.Cells(k).InnerHtml = "Nome Cognome /<br/>Ragione Sociale"
                        Case "sa_cod"
                            Riga.Cells(k).InnerHtml = "Visibilità"
                        Case "id_cf"
                            Riga.Cells(k).InnerHtml = "Tipologia"
                        Case "tipo_indirizzo_default"
                            Riga.Cells(k).InnerHtml = "Tipo Indirizzo di Default<br/>per Doc Contabili"
                        Case "sconto"
                            Riga.Cells(k).InnerHtml = "Sconto di Default per<br/>Doc Contabili Emessi"
                        Case "cod_risum"
                            Riga.Cells(k).InnerHtml = "Codice<br/>GIAS"
                        Case "chkspesometro"
                            Riga.Cells(k).InnerHtml = "Flag<br/>Spesometro"
                        Case "settore_des"
                            Riga.Cells(k).InnerHtml = "Progressivo"
                        Case "attivita_des"
                            Riga.Cells(k).InnerHtml = "Attività"
                        Case "validita_inizio"
                            Riga.Cells(k).InnerHtml = "Validità<br/>Inizio"
                        Case "validita_fine"
                            Riga.Cells(k).InnerHtml = "Validità<br/>Fine"
                        Case "rapporto_des"
                            Riga.Cells(k).InnerHtml = "Rapporto<br/>Contabile"
                        Case "patentino"
                            Riga.Cells(k).InnerHtml = "Dati<br/>Patentino"
                            'Case "validita_inizio_patentino"
                            '    Riga.Cells(k).InnerHtml = "Validità Inizio<br/>Patentino"
                            'Case "validita_fine_patentino"
                            '    Riga.Cells(k).InnerHtml = "Validità Fine<br/>Patentino"
                        Case "corrispettivo"
                            Riga.Cells(k).InnerHtml = "Corrispettivo"
                            'Case "udmsim_prodcosti"
                            '    Riga.Cells(k).InnerHtml = "Unità di Misura<br/>Listino Acquisto"
                        Case "validita_inizio_prodcosti"
                            Riga.Cells(k).InnerHtml = "Validità Inizio<br/>Corrispettivo"
                        Case "validita_fine_prodcosti"
                            Riga.Cells(k).InnerHtml = "Validità Fine<br/>Corrispettivo"
                        Case "ore_settimanali"
                            Riga.Cells(k).InnerHtml = "Ore Settimanali"
                        Case "giorni_malattia"
                            Riga.Cells(k).InnerHtml = "Giorni Malattia"
                        Case "giorni_ferie"
                            Riga.Cells(k).InnerHtml = "Giorni Ferie"
                        Case "giorni_goduti"
                            Riga.Cells(k).InnerHtml = "Giorni Goduti"

                        Case "ind_des_1"
                            Riga.Cells(k).InnerHtml = "Indirizzo<br>Residenza/<br>Sede Operativa"
                        Case "frz_des_1"
                            Riga.Cells(k).InnerHtml = "Frazione<br>Residenza/<br>Sede Operativa"
                        Case "cap_1"
                            Riga.Cells(k).InnerHtml = "CAP<br>Residenza/<br>Sede Operativa"
                        Case "comune_1"
                            Riga.Cells(k).InnerHtml = "Comune<br>Residenza/<br>Sede Operativa"
                        Case "sigla_prov_1"
                            Riga.Cells(k).InnerHtml = "Prov.<br>Residenza/<br>Sede Operativa"
                        Case "stato_1"
                            Riga.Cells(k).InnerHtml = "Stato<br>Residenza/<br>Sede Operativa"

                        Case "ind_des_2"
                            Riga.Cells(k).InnerHtml = "Indirizzo<br>Luogo di Nascita/<br/>Sede Legale"
                        Case "frz_des_2"
                            Riga.Cells(k).InnerHtml = "Frazione<br>Luogo di Nascita/<br/>Sede Legale"
                        Case "cap_2"
                            Riga.Cells(k).InnerHtml = "CAP<br>Luogo di Nascita/<br/>Sede Legale"
                        Case "comune_2"
                            Riga.Cells(k).InnerHtml = "Comune<br>Luogo di Nascita/<br/>Sede Legale"
                        Case "sigla_prov_2"
                            Riga.Cells(k).InnerHtml = "Prov.<br>Luogo di Nascita/<br/>Sede Legale"
                        Case "stato_2"
                            Riga.Cells(k).InnerHtml = "Stato<br>Luogo di Nascita/<br/>Sede Legale"

                        Case "ind_des_3"
                            Riga.Cells(k).InnerHtml = "Indirizzo<br>Domicilio/<br/>Sede Aziendale"
                        Case "frz_des_3"
                            Riga.Cells(k).InnerHtml = "Frazione<br>Domicilio/<br/>Sede Aziendale"
                        Case "cap_3"
                            Riga.Cells(k).InnerHtml = "CAP<br>Domicilio/<br/>Sede Aziendale"
                        Case "comune_3"
                            Riga.Cells(k).InnerHtml = "Comune<br>Domicilio/<br/>Sede Aziendale"
                        Case "sigla_prov_3"
                            Riga.Cells(k).InnerHtml = "Prov.<br>Domicilio/<br/>Sede Aziendale"
                        Case "stato_3"
                            Riga.Cells(k).InnerHtml = "Stato<br>Domicilio/<br/>Sede Aziendale"

                        Case "ind_des_4"
                            Riga.Cells(k).InnerHtml = "Indirizzo<br>Residenza Estiva<br/>Stabilimento"
                        Case "frz_des_4"
                            Riga.Cells(k).InnerHtml = "Frazione<br>Residenza Estiva<br/>Stabilimento"
                        Case "cap_4"
                            Riga.Cells(k).InnerHtml = "CAP<br>Residenza Estiva<br/>Stabilimento"
                        Case "comune_4"
                            Riga.Cells(k).InnerHtml = "Comune<br>Residenza Estiva<br/>Stabilimento"
                        Case "sigla_prov_4"
                            Riga.Cells(k).InnerHtml = "Prov.<br>Residenza Estiva<br/>Stabilimento"
                        Case "stato_4"
                            Riga.Cells(k).InnerHtml = "Stato<br>Residenza Estiva<br/>Stabilimento"

                        Case "telefono"
                            Riga.Cells(k).InnerHtml = "Telefono"
                        Case "fax"
                            Riga.Cells(k).InnerHtml = "Fax"
                        Case "email"
                            Riga.Cells(k).InnerHtml = "Email"
                        Case "cellulare"
                            Riga.Cells(k).InnerHtml = "Cellulare"
                        Case "referente"
                            Riga.Cells(k).InnerHtml = "Referente"
                        Case "ist_credito"
                            Riga.Cells(k).InnerHtml = "Istituto di Credito"
                        Case "coordinate_iban"
                            Riga.Cells(k).InnerHtml = "Coordinate IBAN"
                        Case "inizio_conto"
                            Riga.Cells(k).InnerHtml = "Data<br/>Apertura"
                        Case "fine_conto"
                            Riga.Cells(k).InnerHtml = "Estinzione<br/>Conto"
                        Case "conto"
                            Riga.Cells(k).InnerHtml = "Conto Economico<br>Direttamente Imputabili"
                        Case "cod_risum_origine"
                            Riga.Cells(k).InnerHtml = "Codice GIAS<br/>Pre Importazione"
                        Case "sdi"
                            Riga.Cells(k).InnerHtml = "Codice SDI"
                        Case "listinovenditadefault"
                            Riga.Cells(k).InnerHtml = "Listino Vendita Default"
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

                        Select Case Dt_Finale.Columns.Item(j).Caption.ToLower

                            Case "cod_contatto"
                                If IsNumeric(Dt_Finale.Rows(i).Item(j)) Then
                                    'no cast a int se no va in overflow!
                                    If CDbl(Dt_Finale.Rows(i).Item(j)) < 0 Then
                                        Riga.Cells(j).InnerHtml = "n.d."
                                    Else
                                        'aggiungo uno spazio davanti x salvare gli zeri...
                                        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dt_Finale.Rows(i).Item(j)), "&nbsp;" & Dt_Finale.Rows(i).Item(j), "&nbsp;")
                                    End If
                                Else
                                    'aggiungo uno spazio davanti x salvare gli zeri...
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dt_Finale.Rows(i).Item(j)), "&nbsp;" & Dt_Finale.Rows(i).Item(j), "&nbsp;")
                                End If

                            Case "piva", "codice_fiscale", "telefono", "fax", "cellulare"
                                'aggiungo uno spazio davanti x salvare gli zeri...
                                Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dt_Finale.Rows(i).Item(j)), "&nbsp;" & Dt_Finale.Rows(i).Item(j), "&nbsp;")

                            Case "sa_cod"
                                Select Case Dt_Finale.Rows(i).Item(j)
                                    Case PUBBLICO
                                        Riga.Cells(j).InnerHtml = "Pubblico"
                                    Case PRIVATO
                                        Riga.Cells(j).InnerHtml = "Privato"
                                    Case Else
                                        'chi ha la visibilità del centro stampa cmq privato (e non il nome del centro aziendale)
                                        'aspettiamo che ce lo chiedano
                                        Riga.Cells(j).InnerHtml = "Privato"
                                End Select
                                'If Dt_Finale.Rows(i).Item(j) = -1 Then
                                '    Riga.Cells(j).InnerHtml = "Pubblico"
                                'Else
                                '    Riga.Cells(j).InnerHtml = "Privato"
                                'End If
                            Case "id_cf"
                                If Dt_Finale.Rows(i).Item(j) = 1 Then
                                    Riga.Cells(j).InnerHtml = "Persona Giuridica"
                                Else
                                    Riga.Cells(j).InnerHtml = "Persona Fisica"
                                End If

                            Case "validita_inizio", "validita_inizio_prodcosti", "data_rilascio_patentino"
                                If Dt_Finale.Rows(i).Item(j) = "01/01/1900" Then
                                    Riga.Cells(j).InnerHtml = "..."
                                Else
                                    Riga.Cells(j).InnerHtml = Dt_Finale.Rows(i).Item(j)
                                    'Dt_Finale.Rows(i).Item(j) = Format(Dt_Finale.Rows(i).Item(j), "dd/MM/yyyy")
                                End If

                            Case "validita_fine", "validita_fine_prodcosti", "data_scadenza_patentino"
                                If Dt_Finale.Rows(i).Item(j) = "31/12/2100" Then
                                    Riga.Cells(j).InnerHtml = "..."
                                Else
                                    Riga.Cells(j).InnerHtml = Dt_Finale.Rows(i).Item(j)
                                    'Dt_Finale.Rows(i).Item(j) = Format(Dt_Finale.Rows(i).Item(j), "dd/MM/yyyy")
                                End If

                            Case Else

                                Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dt_Finale.Rows(i).Item(j)), Dt_Finale.Rows(i).Item(j), "&nbsp;")

                        End Select

                    Next

                    'Aggiungo la Riga alla Tabella 
                    Me.TableExcel.Rows.Add(Riga)

                Next
                '-------------------------------
                '--------- FINE TABELLA --------
                '-------------------------------

            End If

        Catch ex As Exception

            Riga = New HtmlTableRow
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(0).ColSpan = Numero_Colonne
            Riga.Cells(0).InnerHtml = ex.Message
            Me.TableExcel.Rows.Add(Riga)

        End Try


    End Sub


End Class

