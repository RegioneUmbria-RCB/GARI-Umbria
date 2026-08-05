Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class MonitoraggioCorpiEstranei_XLS
    Inherits System.Web.UI.Page

    Protected WithEvents TableExcel As System.Web.UI.HtmlControls.HtmlTable


#Region " Trasportatori "

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

    Dim Data_Da, Data_A As String
    Dim Veg_Cod As Integer
    Dim Flag_Appezza As Boolean
    Dim Appezza As Integer
    Dim Id_Reg As Integer
    Dim Piva_Produttore As String
    Dim Mat_Cod As Integer
    Dim Sa_Cod, Tipologia, Pericolosita, Regolamento As Integer
    Dim Sa_Cod_Fabbricato As Integer
    Dim Fabbricato_Cod As Integer

    'oggetto objparametri x server e utenti
    Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    '#########################################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim Riga As HtmlTableRow
        Dim Numero_Colonne As Integer = 0



        'La Pagina deve essere visualizzata come un foglio Excel
        Response.ContentType = "application/vnd.ms-excel"
        Response.AddHeader("Content-Disposition", "inline; filename = monitoraggiocorpiestranei.xls")


        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato

        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If

        'inizializzazione oggetti objParametri_Utenti e objParametri_Server
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        '##############################################################
        '############  Lettura Parametri Query String #################
        '##############################################################

        Try

            Data_Da = Stringa_Decodifica(CStr(Request.QueryString("dal")), AgroKey_EncoderDecoder, Server)

            Data_A = Stringa_Decodifica(CStr(Request.QueryString("al")), AgroKey_EncoderDecoder, Server)

            Piva_Produttore = Stringa_Decodifica(CStr(Request.QueryString("piva_produttore")), AgroKey_EncoderDecoder, Server)

            Sa_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("sa_cod")), AgroKey_EncoderDecoder, Server))

            Veg_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("veg_cod")), AgroKey_EncoderDecoder, Server))

            Mat_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("mat_cod")), AgroKey_EncoderDecoder, Server))

            Flag_Appezza = Stringa_Decodifica(CStr(Request.QueryString("flag_appezza")), AgroKey_EncoderDecoder, Server)

            Appezza = Stringa_Decodifica(CStr(Request.QueryString("appezza")), AgroKey_EncoderDecoder, Server)

            Id_Reg = Stringa_Decodifica(CStr(Request.QueryString("id_reg")), AgroKey_EncoderDecoder, Server)

            Tipologia = Stringa_Decodifica(CStr(Request.QueryString("tipologia")), AgroKey_EncoderDecoder, Server)

            Pericolosita = Stringa_Decodifica(CStr(Request.QueryString("pericolosita")), AgroKey_EncoderDecoder, Server)

            Regolamento = Stringa_Decodifica(CStr(Request.QueryString("regolamento")),
                                             AgroKey_EncoderDecoder, Server)

            Sa_Cod_Fabbricato = CInt(Stringa_Decodifica(CStr(Request.QueryString("sa_cod_fabbr")),
                                                        AgroKey_EncoderDecoder, Server))

            Fabbricato_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("fabbr_cod")),
                                                     AgroKey_EncoderDecoder, Server))


        Catch ex As Exception
            Riga = New HtmlTableRow
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(0).ColSpan = Numero_Colonne
            Riga.Cells(0).InnerHtml = "Dati da querystring: " & ex.Message
            Me.TableExcel.Rows.Add(Riga)
        End Try


        '##############################################################
        '####################  LETTURA DATI ###########################
        '##############################################################

        Dim DT As DataTable = Nothing
        Dim i As Integer
        Dim DT_Stampa As DataTable
        Dim xFiltroAggiuntivo As String

        Try

            Select Case Regolamento
                Case -1
                    xFiltroAggiuntivo = ""
                Case enum_Cod_Regolamento.Regolamento_bio
                    xFiltroAggiuntivo = " Materie_Prime.Regolamento = 4 "
                Case 0
                    xFiltroAggiuntivo = " Materie_Prime.Regolamento <> 4 "
                Case Else
                    xFiltroAggiuntivo = ""
            End Select

            'Sa_Cod_Fabbricato = Me.Cmb_Magazzino.SelectedValue.Split("|")(1)
            'Fabbricato_Cod = Me.Cmb_Magazzino.SelectedValue.Split("|")(0)

            Dim objCE_R As New AgronicaCoreAnagrafeDAL.CorpiEstranei_R
            DT = objCE_R.LeggiCorpiEstraneiBolle(False,
                                                 Piva_Produttore,
                                                 Data_Da, Data_A,
                                                 Veg_Cod,
                                                 Mat_Cod,
                                                 True,
                                                 Sa_Cod, Appezza, Id_Reg,
                                                 Tipologia,
                                                 Pericolosita,
                                                 Sa_Cod_Fabbricato,
                                                 Fabbricato_Cod,
                                                 xFiltroAggiuntivo, "",
                                                 objParametri_Server)

            'copio la struttura del datatable
            DT_Stampa = DT.Clone()

            'DT_Stampa.Columns.Add("Linea 1 - Aereoseparatore", GetType(Integer))
            'DT_Stampa.Columns.Add("Linea 1 - Cernitrice Ottica", GetType(Integer))
            'DT_Stampa.Columns.Add("Linea 1 - Cernita Manuale", GetType(Integer))
            'DT_Stampa.Columns.Add("Linea 2 - Aereoseparatore", GetType(Integer))
            'DT_Stampa.Columns.Add("Linea 2 - Cernitrice Ottica", GetType(Integer))
            'DT_Stampa.Columns.Add("Linea 2 - Cernita Manuale", GetType(Integer))


            Dim N_Bolla As Integer
            Dim Cod_CE As String
            Dim DR As DataRow
            Dim Peso_Netto As Double = 0
            Dim Degrado_Perc As Double = 0
            Dim Degrado As Double = 0
            Dim Netto_Pagamento As Double = 0
            Dim Udm_Cod As Integer


            If Not IsNothing(DT) Then

                N_Bolla = -1
                Cod_CE = -1
                Dim Pesticidi As String
                Dim objADDFun As New AgronicaCoreStampeDAL.AccettazioneDaDiversi_Funzioni

                For i = 0 To DT.Rows.Count - 1

                    If N_Bolla <> DT.Rows(i).Item("Numero Bolla") OrElse
                       (N_Bolla = DT.Rows(i).Item("Numero Bolla") AndAlso
                        Cod_CE <> If(IsDBNull(DT.Rows(i).Item("Corpo Estraneo")), "-999", DT.Rows(i).Item("Corpo Estraneo"))) Then

                        DR = DT_Stampa.NewRow()

                        N_Bolla = DT.Rows(i).Item("Numero Bolla")
                        DR.Item("Anno") = DT.Rows(i).Item("Anno")
                        DR.Item("Numero Bolla") = DT.Rows(i).Item("Numero Bolla")
                        DR.Item("Data di Arrivo") = DT.Rows(i).Item("Data di Arrivo")
                        DR.Item("Carico Numero") = DT.Rows(i).Item("Carico Numero")

                        Peso_Netto = DT.Rows(i).Item("Peso Netto")
                        Degrado_Perc = DT.Rows(i).Item("Degrado")
                        Udm_Cod = DT.Rows(i).Item("Udm_Cod")
                        objADDFun.Calcola_NettoPagamento(Peso_Netto, Degrado_Perc, Udm_Cod, Degrado, Netto_Pagamento)

                        DR.Item("Peso Netto") = Peso_Netto
                        DR.Item("Netto Pagamento") = Netto_Pagamento

                        DR.Item("Specie - Varieta") = CStr(DT.Rows(i).Item("Specie - Varieta"))
                        DR.Item("Punteggio") = DT.Rows(i).Item("Punteggio")
                        DR.Item("Calibro") = DT.Rows(i).Item("Calibro")
                        DR.Item("Ragione Sociale Conferente") = DT.Rows(i).Item("Ragione Sociale Conferente")
                        DR.Item("Ragione Sociale Produttore") = DT.Rows(i).Item("Ragione Sociale Produttore")
                        DR.Item("Data-Ora Inizio Cottura") = CStr(DT.Rows(i).Item("Data-Ora Inizio Cottura"))
                        DR.Item("Confezione / Marchio 1") = CStr(DT.Rows(i).Item("Confezione / Marchio 1"))
                        DR.Item("Confezione / Marchio 2") = CStr(DT.Rows(i).Item("Confezione / Marchio 2"))

                        DR.Item("Note") = CStr(DT.Rows(i).Item("Note"))

                        DR.Item("Stabilimento") = CStr(DT.Rows(i).Item("Stabilimento"))

                        Pesticidi = CStr(DT.Rows(i).Item("Pesticidi"))
                        Select Case Pesticidi
                            Case "1"
                                Pesticidi = "SI COOP"
                            Case "2"
                                Pesticidi = "NO COOP"
                            Case "3"
                                Pesticidi = "In Attesa di Analisi"
                            Case Else
                                Pesticidi = ""
                        End Select
                        DR.Item("Pesticidi") = Pesticidi

                        DR.Item("Livello Qualitativo") = CStr(DT.Rows(i).Item("Livello Qualitativo"))
                        DR.Item("Nome Appezzamento") = CStr(DT.Rows(i).Item("Nome Appezzamento"))
                        DR.Item("Data Inizio Impianto") = DT.Rows(i).Item("Data Inizio Impianto")
                        DR.Item("Superficie Impianto") = DT.Rows(i).Item("Superficie Impianto")


                        'Tipologia CE
                        DR.Item("Corpo Estraneo") = DT.Rows(i).Item("Corpo Estraneo")
                        Cod_CE = If(IsDBNull(DT.Rows(i).Item("Corpo Estraneo")), "-1", DT.Rows(i).Item("Corpo Estraneo"))

                        'posiziono il valore nella casella corretta 
                        If DT.Rows(i).Item("Fase_Cod") = 1 Then
                            Select Case CStr(DT.Rows(i).Item("Tipologia Ritrovamento"))
                                Case "Aereoseparatore"
                                    DR.Item("Linea 1 - Aereoseparatore") = DT.Rows(i).Item("Quantita Rilevata")
                                Case "Cernitrice Ottica"
                                    DR.Item("Linea 1 - Cernitrice Ottica") = DT.Rows(i).Item("Quantita Rilevata")
                                Case "Cernita Manuale"
                                    DR.Item("Linea 1 - Cernita Manuale") = DT.Rows(i).Item("Quantita Rilevata")
                            End Select
                        End If

                        If DT.Rows(i).Item("Fase_Cod") = 2 Then
                            Select Case CStr(DT.Rows(i).Item("Tipologia Ritrovamento"))
                                Case "Aereoseparatore"
                                    DR.Item("Linea 2 - Aereoseparatore") = DT.Rows(i).Item("Quantita Rilevata")
                                Case "Cernitrice Ottica"
                                    DR.Item("Linea 2 - Cernitrice Ottica") = DT.Rows(i).Item("Quantita Rilevata")
                                Case "Cernita Manuale"
                                    DR.Item("Linea 2 - Cernita Manuale") = DT.Rows(i).Item("Quantita Rilevata")
                            End Select
                        End If


                        DT_Stampa.Rows.Add(DR)

                    Else

                        If DT.Rows(i).Item("Fase_Cod") = 1 Then
                            Select Case CStr(DT.Rows(i).Item("Tipologia Ritrovamento"))
                                Case "Aereoseparatore"
                                    DT_Stampa.Rows(DT_Stampa.Rows.Count - 1).Item("Linea 1 - Aereoseparatore") = DT.Rows(i).Item("Quantita Rilevata")
                                Case "Cernitrice Ottica"
                                    DT_Stampa.Rows(DT_Stampa.Rows.Count - 1).Item("Linea 1 - Cernitrice Ottica") = DT.Rows(i).Item("Quantita Rilevata")
                                Case "Cernita Manuale"
                                    DT_Stampa.Rows(DT_Stampa.Rows.Count - 1).Item("Linea 1 - Cernita Manuale") = DT.Rows(i).Item("Quantita Rilevata")
                            End Select
                        End If

                        If DT.Rows(i).Item("Fase_Cod") = 2 Then
                            Select Case CStr(DT.Rows(i).Item("Tipologia Ritrovamento"))
                                Case "Aereoseparatore"
                                    DT_Stampa.Rows(DT_Stampa.Rows.Count - 1).Item("Linea 2 - Aereoseparatore") = DT.Rows(i).Item("Quantita Rilevata")
                                Case "Cernitrice Ottica"
                                    DT_Stampa.Rows(DT_Stampa.Rows.Count - 1).Item("Linea 2 - Cernitrice Ottica") = DT.Rows(i).Item("Quantita Rilevata")
                                Case "Cernita Manuale"
                                    DT_Stampa.Rows(DT_Stampa.Rows.Count - 1).Item("Linea 2 - Cernita Manuale") = DT.Rows(i).Item("Quantita Rilevata")
                            End Select
                        End If

                    End If


                    ''verifico che non sia una riga fittizia
                    If DR.Item("Nome Appezzamento") = "" Then
                        DR.Item("Data Inizio Impianto") = System.DBNull.Value
                        DR.Item("Superficie Impianto") = System.DBNull.Value
                    End If

                Next
                'elimina info inutili

                'DT.Columns.Remove("Appezzamento")

            End If

            DT_Stampa.Columns.Remove("Fase_Cod")
            DT_Stampa.Columns.Remove("Tipologia Ritrovamento")
            DT_Stampa.Columns.Remove("Quantita Rilevata")
            DT_Stampa.Columns.Remove("Pericolosita")
            DT_Stampa.Columns.Remove("degrado")
            DT_Stampa.Columns.Remove("udm_cod")
            DT_Stampa.Columns.Remove("Stabilimento_Sa_Cod")
            DT_Stampa.Columns.Remove("Stabilimento_Fabbr_Cod")

            DT = DT_Stampa.Copy()


        Catch ex As Exception
            Riga = New HtmlTableRow
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(0).ColSpan = Numero_Colonne
            Riga.Cells(0).InnerHtml = "Lettura dei dati: " & ex.Message
            Me.TableExcel.Rows.Add(Riga)
        End Try



        '##############################################################
        '###################  Creazione EXCEL #########################
        '##############################################################

        'If Not IsNothing(DT) AndAlso DT.Rows.Count <> 0 Then

        Crea_EXCEL(DT)

        'End If



    End Sub


    '##############################################################
    Private Sub Crea_EXCEL(ByVal Dt_Finale As DataTable)


        Dim Riga As HtmlTableRow
        Dim Numero_Colonne As Integer = 0

        If IsNothing(Dt_Finale) Then
            Riga = New HtmlTableRow
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(0).ColSpan = Numero_Colonne
            Riga.Cells(0).InnerHtml = "Il criterio di filtro non ha prodotto alcun risultato."
            Me.TableExcel.Rows.Add(Riga)
            Exit Sub
        End If

        Numero_Colonne = CInt(Dt_Finale.Columns.Count)


        '##############################################################
        '#####  Costruisco la tabella   ###############################
        '##############################################################

        Dim i, j, k As Integer

        Try

            Dim Colonna_Dt As String

            'ElaboraCellaHTML(Me.TableExcel.Rows(0).Cells(0), 2, "", "", "Yellow", "left", "middle")
            'Me.TableExcel.Rows(0).Cells(0).InnerHtml = "Anagrafica Contatti"
            'Me.TableExcel.Rows(0).Cells(0).Style.Item("font-weight") = "bold"
            'Me.TableExcel.Rows(0).Cells(0).Style.Item("font-size") = "18px"
            'Me.TableExcel.Rows(0).Cells(0).Style.Item("vertical-align") = "middle"
            'Me.TableExcel.Rows(0).Cells(0).Style.Item("border-top-width") = "1px"

            If Not IsNothing(Dt_Finale) Then

                Me.TableExcel.Rows(0).Cells(0).ColSpan = CInt(Dt_Finale.Columns.Count)

                '------------------------------------------
                '------------- INTESTAZIONE ---------------
                '------------------------------------------
                Riga = New HtmlTableRow

                For k = 0 To Dt_Finale.Columns.Count - 1

                    Riga.Cells.Add(New HtmlTableCell)
                    ElaboraCellaHTML(Riga.Cells(k), 2, "", "", "Gainsboro", "center", "middle")
                    Riga.Cells(k).Style.Item("vertical-align") = "middle"
                    Riga.Cells(k).Style.Item("font-weight") = "bold"
                    Riga.Cells(k).Style.Item("font-size") = "12px"
                    'Riga.Cells(k).Height = "26"

                    'Riga.Cells(k).InnerHtml = Dt_Finale.Rows(CInt(Dt_Finale.Rows.Count - 1)).Item(k)
                    Riga.Cells(k).InnerHtml = Dt_Finale.Columns.Item(k).Caption

                    Colonna_Dt = Riga.Cells(k).InnerHtml.ToLower

                    'Select Case Colonna_Dt

                    '    Case "numero_bolla"
                    '        Riga.Cells(k).InnerHtml = "N. Bolla"

                    'End Select

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

                        Riga.Cells(j).InnerHtml = If(Not IsDBNull(Dt_Finale.Rows(i).Item(j)), Dt_Finale.Rows(i).Item(j), "&nbsp;")

                        Select Case Dt_Finale.Columns.Item(j).Caption.ToLower


                            Case CStr("Note").ToLower
                                If Dt_Finale.Rows(i).Item(j) <> "" Then
                                    'aggiungo un apice davanti per vedere visualizzato il numero e non una data
                                    'Riga.Cells(j).InnerHtml = "'" & Dt_Finale.Rows(i).Item(j)
                                    Riga.Cells(j).InnerHtml = "&nbsp;" & Dt_Finale.Rows(i).Item(j)
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
            Riga.Cells(0).InnerHtml = "Creazione Excel Esportazione: " & ex.Message
            Me.TableExcel.Rows.Add(Riga)

        End Try

        '########################################################

    End Sub

End Class
