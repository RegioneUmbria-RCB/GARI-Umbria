Public Class LibroConferimenti_XLS
    Inherits System.Web.UI.Page

    Protected WithEvents TableExcel As System.Web.UI.HtmlControls.HtmlTable

#Region " EXCEL CONFERIMENTI "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region


    '#########################################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim Riga As HtmlTableRow
        Dim Numero_Colonne As Integer = 0

        'La Pagina deve essere visualizzata come un foglio Excel
        Response.ContentType = "application/vnd.ms-excel"
        Response.AddHeader("Content-Disposition", "inline; filename = LibroConferiementi.xls")

        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato

        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If


        ''##############################################################
        ''############  Lettura Parametri Query String #################
        ''##############################################################

        'Try

        '    Data_Da = Stringa_Decodifica(CStr(Request.QueryString("dal")), _
        '                     AgroKey_EncoderDecoder, _
        '                     Server)

        'Catch ex As Exception
        '    Riga = New HtmlTableRow
        '    Riga.Cells.Add(New HtmlTableCell)
        '    Riga.Cells(0).ColSpan = Numero_Colonne
        '    Riga.Cells(0).InnerHtml = "Dati da querystring: " + ex.Message
        '    Me.TableExcel.Rows.Add(Riga)
        'End Try


        '##############################################################
        '####################  LETTURA DATI ###########################
        '##############################################################

        Dim DT As DataTable = Nothing
        Dim Log_Errori As String = ""
        Dim Flag_ConferimentoLatte As Boolean = False

        Try

            DT = Carica_DT()

            Lettura_Dati(DT, Flag_ConferimentoLatte, Log_Errori)

            If Log_Errori <> "" Then
                Riga = New HtmlTableRow
                Riga.Cells.Add(New HtmlTableCell)
                Riga.Cells(0).ColSpan = Numero_Colonne
                Riga.Cells(0).InnerHtml = "Lettura dei dati: " & Log_Errori
                Me.TableExcel.Rows.Add(Riga)
            End If

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

        Crea_EXCEL(DT, Flag_ConferimentoLatte)

    End Sub

    '###########################################################################
    Private Function Carica_DT() As DataTable

        Dim DT As New DataTable

        DT.Columns.Add(New DataColumn("data_bolla", GetType(String)))
        DT.Columns.Add(New DataColumn("numero_bolla", GetType(String)))
        DT.Columns.Add(New DataColumn("conferente_rag_soc", GetType(String)))
        DT.Columns.Add(New DataColumn("dettagli_coltura", GetType(String)))
        DT.Columns.Add(New DataColumn("peso_netto", GetType(String)))
        DT.Columns.Add(New DataColumn("peso_effettivo", GetType(String)))
        'DT.Columns.Add(New DataColumn("udm_des", GetType(String)))

        Return DT

    End Function

    '###########################################################################
    Public Sub Lettura_Dati(ByRef DT As DataTable, _
                            ByRef Flag_ConferimentoLatte As Boolean, _
                            ByRef Log_Errori As String)

        'Dati generali
        'Dim Piva As String
        'Dim rag_soc As String
        'Dim Validita_Inizio As String
        'Dim Validita_Fine As String
        'Dim peso_netto_complessivo As String
        'Dim peso_netto_complessivo_dbl As Double
        'Dim contratto_des As String

        Dim data_bolla As String
        Dim numero_bolla As String
        'Dim conferente_piva As String
        Dim conferente_rag_soc As String
        Dim dettagli_coltura As String
        Dim udm_des As String
        Dim peso_netto As Double
        Dim peso_specifico As Double
        Dim peso_effettivo As Double
        'Dim peso_netto_dbl As Double
        'Dim lotto_interno As String

        Dim i As Integer

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XML_FiltroStampa As System.Xml.XmlElement
        Dim XMLs_DettaglioVariabiliStampe As System.Xml.XmlNodeList
        Dim XML_DettaglioVariabiliStampe As System.Xml.XmlElement

        '##############################################################
        '#####  Recupero l'xml            ##############################
        '##############################################################

        Dim strXmlVariabilistampe As String
        strXmlVariabilistampe = Session("strXmlVariabilistampe")

        If strXmlVariabilistampe <> "" Then

            'Carico la stringa xml in un nuovo documento
            XmlDoc = New System.Xml.XmlDocument

            XmlDoc.LoadXml(strXmlVariabilistampe)

            If XmlDoc.HasChildNodes Then

                ''XML_FiltroStampa = XmlDoc.SelectSingleNode("FiltroStampa")

                'Piva = XML_FiltroStampa.GetAttribute("piva")
                'rag_soc = XML_FiltroStampa.GetAttribute("rag_soc")
                'contratto_des = XML_FiltroStampa.GetAttribute("contratto_des")
                'Validita_Inizio = XML_FiltroStampa.GetAttribute("validita_inizio")
                'Validita_Fine = XML_FiltroStampa.GetAttribute("validita_fine")
                'peso_netto_complessivo = XML_FiltroStampa.GetAttribute("peso_netto_complessivo")
                'If XML_FiltroStampa.GetAttribute("peso_netto_complessivo") <> "" Then
                '    peso_netto_complessivo_dbl = CDbl(XML_FiltroStampa.GetAttribute("peso_netto_complessivo"))
                'Else
                '    peso_netto_complessivo_dbl = 0
                'End If

                'XMLs_DettaglioVariabiliStampe = XML_FiltroStampa.GetElementsByTagName("DettaglioVariabiliStampe")

                XML_FiltroStampa = XmlDoc.SelectSingleNode("//DatiDettagliVariabiliStampe")

                'Piva = XML_FiltroStampa.GetAttribute("piva")
                'rag_soc = XML_FiltroStampa.GetAttribute("rag_soc")
                'contratto_des = XML_FiltroStampa.GetAttribute("contratto_des")
                'Validita_Inizio = XML_FiltroStampa.GetAttribute("validita_inizio")
                'Validita_Fine = XML_FiltroStampa.GetAttribute("validita_fine")


                XMLs_DettaglioVariabiliStampe = XmlDoc.SelectNodes("//DettaglioVariabiliStampe")

                Dim Num_DettagliXML As Integer = XMLs_DettaglioVariabiliStampe.Count
                Dim Dr As DataRow

                For i = 0 To Num_DettagliXML - 1

                    XML_DettaglioVariabiliStampe = XMLs_DettaglioVariabiliStampe.Item(i)

                    If i = 0 Then
                        'se c'è l'attributo peso_specifico
                        'è il caso del grano
                        'va aggiunta la colonna al dt
                        '(controllo solo al primo giro)
                        If XML_DettaglioVariabiliStampe.HasAttribute("peso_specifico") Then
                            DT.Columns.Add(New DataColumn("peso_specifico", GetType(String)))
                        End If

                    End If

                    data_bolla = XML_DettaglioVariabiliStampe.GetAttribute("data_bolla")
                    numero_bolla = XML_DettaglioVariabiliStampe.GetAttribute("numero_bolla")
                    'conferente_piva = XML_DettaglioVariabiliStampe.GetAttribute("conferente_piva")
                    conferente_rag_soc = XML_DettaglioVariabiliStampe.GetAttribute("conferente_rag_soc")
                    dettagli_coltura = XML_DettaglioVariabiliStampe.GetAttribute("dettagli_coltura")
                    udm_des = XML_DettaglioVariabiliStampe.GetAttribute("udm_des")
                    peso_netto = CDbl(XML_DettaglioVariabiliStampe.GetAttribute("peso_netto"))
                    peso_effettivo = CDec(XML_DettaglioVariabiliStampe.GetAttribute("peso_effettivo"))

                    'If udm_des.ToLower = "l" Then
                    '    Flag_ConferimentoLatte = True
                    'End If

                    'If XML_DettaglioVariabiliStampe.GetAttribute("peso_netto") <> "" Then
                    '    peso_netto_dbl = CDbl(XML_DettaglioVariabiliStampe.GetAttribute("peso_netto"))
                    'Else
                    '    peso_netto_dbl = 0
                    'End If
                    'lotto_interno = XML_DettaglioVariabiliStampe.GetAttribute("lotto_interno")

                    'Totale += peso_netto_dbl

                    Dr = DT.NewRow

                    Dr.Item("data_bolla") = data_bolla
                    Dr.Item("numero_bolla") = numero_bolla
                    Dr.Item("conferente_rag_soc") = conferente_rag_soc
                    Dr.Item("dettagli_coltura") = dettagli_coltura
                    Dr.Item("peso_netto") = Format(peso_netto, "###,###,##0.00")
                    Dr.Item("peso_effettivo") = Format(peso_effettivo, "###,###,##0.00")
                    Dr.Item("data_bolla") = data_bolla
                    'Dr.Item("udm_des") = udm_des

                    If XML_DettaglioVariabiliStampe.HasAttribute("peso_specifico") Then
                        peso_specifico = XML_DettaglioVariabiliStampe.GetAttribute("peso_specifico")
                        Dr.Item("peso_specifico") = peso_specifico
                    End If

                    'Inserisco la riga
                    DT.Rows.Add(Dr)

                Next

            End If
        Else
            Log_Errori &= "- Lettura dei dati: " & vbCrLf & "Non sono arrivati dati dal GiasLan." & vbCrLf & vbCrLf
        End If

    End Sub


    '##############################################################
    Private Sub Crea_EXCEL(ByVal Dt_Finale As DataTable, _
                           ByVal Flag_ConferimentoLatte As Boolean)

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
            'Dim IndiceColPesoEffettivo As Integer = 0

            'AgronicaCoreDataProvider.UtilityProvider.ElaboraCellaHTML(Me.TableExcel.Rows(0).Cells(0), 2, "", "", "Yellow", "left", "middle")
            'Me.TableExcel.Rows(0).Cells(0).InnerHtml = "Anagrafica Contatti"
            'Me.TableExcel.Rows(0).Cells(0).Style.Item("font-weight") = "bold"
            'Me.TableExcel.Rows(0).Cells(0).Style.Item("font-size") = "18px"
            'Me.TableExcel.Rows(0).Cells(0).Style.Item("vertical-align") = "middle"
            'Me.TableExcel.Rows(0).Cells(0).Style.Item("border-top-width") = "1px"

            If Not IsNothing(Dt_Finale) Then

                'Me.TableExcel.Rows(0).Cells(0).ColSpan = CInt(Dt_Finale.Columns.Count)

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
                        Case "data_bolla"
                            Riga.Cells(k).InnerHtml = "Data Bolla"
                        Case "numero_bolla"
                            Riga.Cells(k).InnerHtml = "N. Bolla"
                        Case "conferente_rag_soc"
                            Riga.Cells(k).InnerHtml = "Ente"
                        Case "dettagli_coltura"
                            Riga.Cells(k).InnerHtml = "Specie Varietà"
                        Case "peso_netto"
                            If Not Flag_ConferimentoLatte Then
                                Riga.Cells(k).InnerHtml = "Peso Netto (kg)"
                            Else
                                Riga.Cells(k).InnerHtml = "Litri"
                            End If
                        Case "peso_effettivo"
                            'IndiceColPesoEffettivo = k
                            'If Flag_ConferimentoLatte = False Then
                            '    Riga.Cells(k).InnerHtml = "Peso Effettivo (kg)"
                            'Else
                            '    Riga.Cells(k).InnerHtml = ""
                            'End If
                            Riga.Cells(k).InnerHtml = "Peso Effettivo (kg)"
                        Case "peso_specifico"
                            Riga.Cells(k).InnerHtml = "Peso Specifico"
                    End Select

                Next

                Me.TableExcel.Rows.Add(Riga)
                '----------------------------------
                '------- FINE INTESTAZIONE --------
                '----------------------------------

                ''nel caso conferimenti del latte non serve la colonan del peso effettivo
                'If Flag_ConferimentoLatte = True Then
                '    Dt_Finale.Columns.RemoveAt(IndiceColPesoEffettivo)
                'End If

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

                        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dt_Finale.Rows(i).Item(j)), Dt_Finale.Rows(i).Item(j), "&nbsp;")

                        Select Case Dt_Finale.Columns.Item(j).Caption.ToLower

                            Case CStr("data_bolla").ToLower
                                If Dt_Finale.Rows(i).Item(j) <> "" Then
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
            Riga.Cells(0).InnerHtml = "Creazione Excel: " & ex.Message
            Me.TableExcel.Rows.Add(Riga)

        End Try

        '########################################################


    End Sub


End Class
