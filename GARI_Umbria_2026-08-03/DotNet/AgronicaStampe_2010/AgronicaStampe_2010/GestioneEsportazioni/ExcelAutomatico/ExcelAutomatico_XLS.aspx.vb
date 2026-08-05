Imports System.Xml
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class ExcelAutomatico_XLS
    Inherits System.Web.UI.Page

    Protected WithEvents TableExcel As System.Web.UI.HtmlControls.HtmlTable
    Protected WithEvents TableDati As System.Web.UI.HtmlControls.HtmlTable


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

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim Riga As HtmlTableRow
        Dim Numero_Colonne As Integer = 0

        'La Pagina deve essere visualizzata come un foglio Excel
        Response.ContentType = "application/vnd.ms-excel"
        Response.Charset = "utf-8"
        Response.AddHeader("Content-Disposition", "attachment; filename = ExcelAutomatico.xls")


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
        '                            TipiEnumerativi.enum_Security_Attivita.Gest_Stampe, _
        '                            TipiEnumerativi.enum_Security_Operazione.Lettura, _
        '                            strDummy)
        '----- !!!!!!!!!!! -------------
        'Attivazione forzata provvisoria
        UtenteAbilitato = True
        '----- !!!!!!!!!!! -------------

        '----- Se l'utente non ha il permesso per visualizzare la pagina ... lo invio al menu.
        If Not UtenteAbilitato Then
            Response.Redirect("../../Messaggi/AccessoNegato.htm")
        End If

        Try

            '##############################################################
            '###################  Creazione EXCEL #########################
            '##############################################################

            EXCEL_Crea()


            ''--- Spacchetto la stringa e genero i dettagli
            'Call CreaDtRisultati()


        Catch ex As Exception
            Riga = New HtmlTableRow
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(0).ColSpan = Numero_Colonne
            Riga.Cells(0).InnerHtml = "Caricamento pagina: " & ex.Message
            Me.TableDati.Rows.Add(Riga)
        End Try


    End Sub



    '##############################################################################################################
    'funzione vecchia by simone
    Private Sub CreaDtRisultati()


        ''--- Genero l'intestazione ---
        'Dim Piano_cod As Integer
        'Dim Riga As HtmlTableRow

        'TableDati.Rows(0).Cells(0).InnerHtml = "<font color='white'><strong>" + RagSoc_from_Piva(Server, Session, Page, Session("PartitaIVA")) + "</strong></font> "
        'TableDati.Rows(0).Cells(0).ColSpan = 10
        'ElaboraCellaHTML(TableDati.Rows(0).Cells(0), 0, "", "", "#74920E", "center", "top")
        'Riga = New HtmlTableRow
        'Riga.Cells.Add(New HtmlTableCell)
        'TableDati.Rows.Add(Riga)

        ''--- Riga vuota
        'Riga = New HtmlTableRow
        'Riga.Cells.Add(New HtmlTableCell)
        'Riga.Cells(0).InnerHtml = ""
        'Riga.Cells(0).ColSpan = 5
        'Riga.Cells(0).BgColor = "#74920E"
        'Riga.Cells(0).Height = "4"

        'TableDati.Rows.Add(Riga)


        'Dim strErr As String

        'Dim XmlDoc As New XmlDocument
        'Dim XML_FiltroStampa As XmlElement
        'Dim XML_Report As XmlElement

        'Dim XMLs_Sottoreport As XmlNodeList
        'Dim XML_Sottoreport As XmlElement

        'Dim XML_Sezione As XmlElement
        'Dim XMLs_Sezione As XmlNodeList

        'Dim XML_SezioneTitolo As XmlElement
        'Dim XMLs_SezioneTitolo As XmlNodeList

        'Dim XML_Riga As XmlElement
        'Dim XMLs_Riga As XmlNodeList

        'Dim xmlAttributo As XmlAttribute

        ''--- Contatori
        'Dim i, j As Integer
        'Dim i_sezione As Integer
        ''Dim Riga As HtmlTableRow


        'Try

        '    'Carico la stringa xml in un nuovo documento
        '    XmlDoc = New XmlDocument
        '    XmlDoc.LoadXml(Session("strXmlVariabilistampe").ToString)

        '    If XmlDoc.HasChildNodes Then

        '        XML_FiltroStampa = XmlDoc.SelectSingleNode("FiltroStampa") ' --E--

        '        XML_Report = XML_FiltroStampa.SelectSingleNode("report") ' --E--

        '        Session("PartitaIVA") = XML_Report.GetAttribute("piva")


        '        '----- Tag Sottoreport (multiplo)
        '        XMLs_Sottoreport = XML_Report.GetElementsByTagName("sottoreport")  ' --E--


        '        '--------------------------------------------------------
        '        For i = 0 To XMLs_Sottoreport.Count - 1

        '            XML_Sottoreport = XMLs_Sottoreport(i)

        '            'Creo una nuova riga ---
        '            Riga = New HtmlTableRow
        '            'Aggiungo la Riga alla Tabella 
        '            TableDati.Rows.Add(Riga)

        '            'Creo una nuova riga ---
        '            Riga = New HtmlTableRow
        '            Riga.Cells.Add(New HtmlTableCell)
        '            'Riga.Cells(0).InnerHtml = XML_Sottoreport("titolo_sottoreport").InnerXml + XML_Sottoreport.GetAttribute("sa_nome") ' --E--
        '            Riga.Cells(0).InnerHtml = XML_Sottoreport.GetElementsByTagName("titolo_sottoreport").Item(0).Attributes("sa_nome").Value()
        '            'Aggiungo la Riga alla Tabella 
        '            TableDati.Rows.Add(Riga)
        '            '                   ---


        '            XMLs_Sezione = XML_Sottoreport.GetElementsByTagName("sezione")

        '            '-----------------------------------------------------------------------------
        '            For i_sezione = 0 To XML_Sottoreport.GetElementsByTagName("sezione").Count - 1


        '                XML_Sezione = XMLs_Sezione(i_sezione)


        '                If XML_Sezione.HasChildNodes = True Then

        '                    XMLs_Riga = XML_Sezione.GetElementsByTagName("riga")
        '                    XMLs_SezioneTitolo = XML_Sezione.GetElementsByTagName("sezione_titolo")

        '                    '----------------------------------------
        '                    For j = 0 To XMLs_Riga.Count - 1 '--- Riga

        '                        XML_Riga = XMLs_Riga.Item(j)

        '                        Dim titolo As String

        '                        'Creo una nuova riga
        '                        Riga = New HtmlTableRow

        '                        If j = 0 Then

        '                            ''Creo una nuova riga
        '                            'Riga = New HtmlTableRow

        '                            XML_SezioneTitolo = XMLs_SezioneTitolo.Item(0)
        '                            titolo = XML_SezioneTitolo.InnerText

        '                            Riga.Cells.Add(New HtmlTableCell)
        '                            Riga.Cells(0).InnerHtml = CStr(titolo)
        '                            Riga.Cells(0).ColSpan = 1
        '                            Riga.Cells(0).BgColor = "#f7a00a"
        '                            'Riga.Cells(0).Height = "3"
        '                            Riga.Cells(0).RowSpan = 1

        '                            ''Aggiungo la Riga alla Tabella 
        '                            'TableDati.Rows.Add(Riga)

        '                        Else

        '                            XML_SezioneTitolo = XMLs_SezioneTitolo.Item(0)
        '                            titolo = XML_SezioneTitolo.InnerText

        '                            Riga.Cells.Add(New HtmlTableCell)
        '                            Riga.Cells(0).InnerHtml = ""
        '                            Riga.Cells(0).ColSpan = 1
        '                            Riga.Cells(0).BgColor = "#f7a00a"
        '                            'Riga.Cells(0).Height = "3"
        '                            Riga.Cells(0).RowSpan = 1

        '                        End If


        '                        Riga.Cells.Add(New HtmlTableCell)
        '                        Riga.Cells(1).InnerHtml = CStr(XML_Riga.GetAttribute("c0"))
        '                        Riga.Cells.Add(New HtmlTableCell)
        '                        Riga.Cells(2).InnerHtml = CStr(XML_Riga.GetAttribute("c1"))
        '                        Riga.Cells.Add(New HtmlTableCell)
        '                        Riga.Cells(3).InnerHtml = CStr(XML_Riga.GetAttribute("c2"))
        '                        Riga.Cells.Add(New HtmlTableCell)
        '                        Riga.Cells(4).InnerHtml = CStr(XML_Riga.GetAttribute("c3"))
        '                        Riga.Cells.Add(New HtmlTableCell)
        '                        Riga.Cells(5).InnerHtml = CStr(XML_Riga.GetAttribute("c4"))
        '                        Riga.Cells.Add(New HtmlTableCell)
        '                        Riga.Cells(6).InnerHtml = CStr(XML_Riga.GetAttribute("c5"))
        '                        Riga.Cells.Add(New HtmlTableCell)
        '                        Riga.Cells(7).InnerHtml = CStr(XML_Riga.GetAttribute("c6"))
        '                        Riga.Cells.Add(New HtmlTableCell)
        '                        Riga.Cells(8).InnerHtml = CStr(XML_Riga.GetAttribute("c7"))
        '                        Riga.Cells.Add(New HtmlTableCell)
        '                        Riga.Cells(9).InnerHtml = CStr(XML_Riga.GetAttribute("c8"))

        '                        'Aggiungo la Riga alla Tabella 
        '                        TableDati.Rows.Add(Riga)

        '                    Next '--- riga

        '                End If

        '                '---Riga vuota -- dopo ogni sezione
        '                Riga = New HtmlTableRow
        '                Riga.Cells.Add(New HtmlTableCell)
        '                Riga.Cells(0).InnerHtml = ""
        '                Riga.Cells(0).ColSpan = 10
        '                Riga.Cells(0).BgColor = "#74920E"
        '                Riga.Cells(0).Height = "3"

        '                TableDati.Rows.Add(Riga)

        '            Next '--- sezione 

        '            '---Riga vuota -- dopo ogni sottosezione
        '            Riga = New HtmlTableRow
        '            Riga.Cells.Add(New HtmlTableCell)
        '            Riga.Cells(0).InnerHtml = ""
        '            Riga.Cells(0).ColSpan = 10
        '            'Riga.Cells(0).BgColor = "#74920E"
        '            'Riga.Cells(0).Height = "10"

        '            TableDati.Rows.Add(Riga)

        '        Next '--- sottoreport

        '    End If


        'Catch ex As Exception

        '    strErr = ex.Message

        'End Try


    End Sub


    '##############################################################################################################
    Private Sub EXCEL_Crea()
        'ByRef Numero_Colonne As Integer
        Dim Numero_Colonne As Integer

        Dim Riga As HtmlTableRow

        Dim XmlDoc As New XmlDocument
        Dim XML_Report As XmlElement
        Dim XMLs_Sottoreport As XmlNodeList
        Dim XML_Sottoreport As XmlElement
        Dim XML_Sezione As XmlElement
        Dim XMLs_Sezione As XmlNodeList
        Dim XML_Intestazione As XmlElement
        Dim XML_PiePagina As XmlElement
        Dim XML_Riga As XmlElement
        Dim XMLs_Riga As XmlNodeList
        Dim XMLs_Cella As XmlNodeList

        Dim i_sr, i_se, i_ri As Integer

        Dim r_piva As String
        Dim r_rag_soc As String
        Dim r_titolo As String
        Dim r_produzione_codice As Integer
        Dim sr_titolo As String
        Dim sr_numero_colonne As Integer


        Try
            If IsNothing(Session("strXmlVariabilistampe")) Or Session("strXmlVariabilistampe") = "" Then
                ' AgroMsgBox("ERRORE! Non sono arrivati dati da stampare!", Page)
            Else

                'Carico la stringa xml in un nuovo documento
                XmlDoc = New XmlDocument
                XmlDoc.LoadXml(Session("strXmlVariabilistampe").ToString)


                '###########################################################
                '############             REPORT      ######################
                '###########################################################
                XML_Report = XmlDoc.SelectSingleNode("report")

                r_piva = XML_Report.GetAttribute("piva")
                r_rag_soc = XML_Report.GetAttribute("rag_soc")
                r_titolo = XML_Report.GetAttribute("titolo")
                If XML_Report.HasAttribute("produzione_codice") AndAlso XML_Report.GetAttribute("produzione_codice") <> "" Then
                    r_produzione_codice = XML_Report.GetAttribute("produzione_codice")
                Else
                    r_produzione_codice = 0
                End If

                'RAGIONE SOCIALE
                '=======================================
                If r_rag_soc <> "" Then
                    Riga = New HtmlTableRow
                    Riga.Cells.Add(New HtmlTableCell)
                    EXCEL_Formatta_Report_Titolo(Riga.Cells(0), 1, r_rag_soc)
                    Me.TableDati.Rows.Add(Riga)
                End If
                '=======================================

                'TITOLO REPORT
                '=======================================
                If r_titolo <> "" Then
                    Riga = New HtmlTableRow
                    Riga.Cells.Add(New HtmlTableCell)
                    EXCEL_Formatta_Report_Titolo(Riga.Cells(0), 1, r_titolo)
                    Me.TableDati.Rows.Add(Riga)
                End If
                '=======================================


                '###########################################################
                '############           SOTTOREPORT      ###################
                '###########################################################
                '----- Tag Sottoreport (multiplo)
                XMLs_Sottoreport = XML_Report.GetElementsByTagName("sottoreport")

                '--------------------------------------------------------
                For i_sr = 0 To XMLs_Sottoreport.Count - 1

                    XML_Sottoreport = XMLs_Sottoreport(i_sr)

                    sr_titolo = XML_Sottoreport.GetAttribute("titolo")
                    sr_numero_colonne = XML_Sottoreport.GetAttribute("numero_colonne")
                    Numero_Colonne = sr_numero_colonne

                    If i_sr <> 0 Then
                        'SE il report contiene più sottoreport
                        'aggiungo 1 riga vuota per separarlo dal sottoreport sopra

                        '=======================================
                        Riga = New HtmlTableRow
                        Me.TableDati.Rows.Add(Riga)
                        '=======================================

                    Else
                        If Me.TableDati.Rows.Count >= 2 Then
                            Me.TableDati.Rows(0).Cells(0).ColSpan = Numero_Colonne
                            Me.TableDati.Rows(1).Cells(0).ColSpan = Numero_Colonne
                        End If
                    End If

                    'TITOLO SOTTOREPORT
                    '=======================================
                    If sr_titolo <> "" Then
                        Riga = New HtmlTableRow
                        Riga.Cells.Add(New HtmlTableCell)
                        EXCEL_Formatta_SottoReport_Titolo(Riga.Cells(0), Numero_Colonne, sr_titolo)
                        Me.TableDati.Rows.Add(Riga)
                    End If
                    '=======================================

                    '###########################################################
                    '###########             SEZIONE      ######################
                    '###########################################################
                    XMLs_Sezione = XML_Sottoreport.GetElementsByTagName("sezione")

                    '-----------------------------------------------------------------------------
                    For i_se = 0 To XMLs_Sezione.Count - 1

                        XML_Sezione = XMLs_Sezione(i_se)


                        '###########################################################
                        '###########           INTESTAZIONE      ###################
                        '###########################################################
                        XML_Intestazione = XML_Sezione.SelectSingleNode("intestazione")

                        If Not IsNothing(XML_Intestazione) AndAlso XML_Intestazione.HasChildNodes Then

                            'c'è l'intestazione
                            'creo una nuova riga, formattata come intestazione

                            '###########################################################
                            '###############           CELLA      ######################
                            '###########################################################
                            XMLs_Cella = XML_Intestazione.GetElementsByTagName("cella")

                            EXCEL_Inserisci_Cella(XMLs_Cella, enum_TipoFormat.Intestazione)

                        End If 'intestazione


                        '###########################################################
                        '################           RIGA      ######################
                        '###########################################################
                        XMLs_Riga = XML_Sezione.GetElementsByTagName("riga")

                        If Not IsNothing(XMLs_Riga) AndAlso XMLs_Riga.Count > 0 Then

                            For i_ri = 0 To XMLs_Riga.Count - 1

                                XML_Riga = XMLs_Riga.Item(i_ri)

                                If Not IsNothing(XML_Riga) AndAlso XML_Riga.HasChildNodes Then

                                    'creo una nuova riga, formattata come riga normale

                                    '###########################################################
                                    '###############           CELLA      ######################
                                    '###########################################################
                                    XMLs_Cella = XML_Riga.GetElementsByTagName("cella")

                                    EXCEL_Inserisci_Cella(XMLs_Cella, enum_TipoFormat.Riga)

                                End If

                            Next

                        End If 'riga


                        '###########################################################
                        '###########          PIE DI PAGINA      ###################
                        '###########################################################
                        XML_PiePagina = XML_Sezione.SelectSingleNode("pie_pagina")

                        If Not IsNothing(XML_PiePagina) AndAlso XML_PiePagina.HasChildNodes Then

                            'c'è il piè di pagina
                            'creo una nuova riga, formattata come piè di pagina

                            '###########################################################
                            '###############           CELLA      ######################
                            '###########################################################
                            XMLs_Cella = XML_PiePagina.GetElementsByTagName("cella")

                            EXCEL_Inserisci_Cella(XMLs_Cella, enum_TipoFormat.Pie_Pagina)

                        End If 'pie pagina

                    Next '--- sezione 


                Next '--- sottoreport


            End If

        Catch ex As Exception
            Riga = New HtmlTableRow
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(0).ColSpan = Numero_Colonne
            Riga.Cells(0).InnerHtml = "Caricamento pagina: " & ex.Message
            Me.TableDati.Rows.Add(Riga)
        End Try


    End Sub

    Private Enum enum_TipoFormat

        Intestazione = 0
        Riga = 1
        Pie_Pagina = 2

    End Enum

    '##############################################################################################################
    Private Sub EXCEL_Inserisci_Cella(ByRef XMLs_Cella As XmlNodeList,
                                    ByVal Tipo_Formattazione As enum_TipoFormat)


        Dim i_ce As Integer
        Dim XML_Cella As XmlElement
        Dim Riga As HtmlTableRow
        Dim c_testo As String
        Dim c_colspan As Integer
        Dim c_rowspan As Integer
        Dim c_background As String
        Dim c_width As String
        Dim c_formatta As enum_FormattazioneExcel

        '###########################################################
        '###############           CELLA      ######################
        '###########################################################

        '=======================================
        Riga = New HtmlTableRow

        If Not IsNothing(XMLs_Cella) AndAlso XMLs_Cella.Count > 0 Then

            For i_ce = 0 To XMLs_Cella.Count - 1

                XML_Cella = XMLs_Cella.Item(i_ce)

                c_testo = XML_Cella.GetAttribute("testo")

                'se non c'è, il default è 1
                If XML_Cella.HasAttribute("rowspan") AndAlso XML_Cella.GetAttribute("rowspan") <> "" Then
                    c_rowspan = XML_Cella.GetAttribute("rowspan")
                Else
                    c_rowspan = 1
                End If

                'se non c'è, il default è 1
                If XML_Cella.HasAttribute("colspan") AndAlso XML_Cella.GetAttribute("colspan") <> "" Then
                    c_colspan = XML_Cella.GetAttribute("colspan")
                Else
                    c_colspan = 1
                End If

                'se non c'è, il default è sfondo bianco
                If XML_Cella.HasAttribute("background_color") AndAlso XML_Cella.GetAttribute("background_color") <> "" Then

                    If Len(XML_Cella.GetAttribute("background_color")) = 7 AndAlso XML_Cella.GetAttribute("background_color").StartsWith("#") Then
                        c_background = XML_Cella.GetAttribute("background_color")
                    Else
                        If Len(XML_Cella.GetAttribute("background_color")) = 6 Then
                            c_background = "#" & XML_Cella.GetAttribute("background_color")
                        Else
                            c_background = "#FFFFFF"
                        End If
                    End If
                Else
                    c_background = "#FFFFFF"
                End If

                'se non c'è, non la specifico
                If XML_Cella.HasAttribute("colwidth") AndAlso XML_Cella.GetAttribute("colwidth") <> "" Then
                    c_width = XML_Cella.GetAttribute("colwidth")
                Else
                    c_width = 0
                End If

                'se non c'è, default è testo
                If XML_Cella.HasAttribute("formatta") AndAlso XML_Cella.GetAttribute("formatta") <> "" Then
                    c_formatta = CInt(XML_Cella.GetAttribute("formatta"))
                    'Else
                    '    c_formatta = enum_FormattazioneExcel.FE_Text
                End If

                Riga.Cells.Add(New HtmlTableCell)
                ' "#f7a00a"

                'la formattazione viene passata dal lan
                Select Case Tipo_Formattazione

                    Case enum_TipoFormat.Intestazione
                        'EXCEL_Formatta_Intestazione_Cella(Riga.Cells(i_ce), c_colspan, c_rowspan, c_width, c_background, c_testo)
                        EXCEL_Formatta_Cella(Riga.Cells(i_ce), c_colspan, c_rowspan, c_width, c_background, c_testo, c_formatta)
                        '=======================================

                    Case enum_TipoFormat.Riga
                        'EXCEL_Formatta_Riga_Cella(Riga.Cells(i_ce), c_colspan, c_rowspan, c_width, c_background, c_testo)
                        EXCEL_Formatta_Cella(Riga.Cells(i_ce), c_colspan, c_rowspan, 0, c_background, c_testo, c_formatta)
                        '=======================================

                    Case enum_TipoFormat.Pie_Pagina
                        'EXCEL_Formatta_PiePagina_Cella(Riga.Cells(i_ce), c_colspan, c_rowspan, c_width, c_background, c_testo)
                        EXCEL_Formatta_Cella(Riga.Cells(i_ce), c_colspan, c_rowspan, 0, c_background, c_testo, c_formatta)
                        '=======================================

                End Select

            Next

        Else
            'cella vuota
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(0).InnerHtml = ""
        End If

        Me.TableDati.Rows.Add(Riga)
        '=======================================


    End Sub

    '###########################################################
    Private Sub EXCEL_Formatta_Report_Titolo(ByRef Cella As HtmlTableCell,
                                            ByVal Colspan As Integer,
                                            ByVal Testo As String)

        Dim Bordo_Spessore As Integer = 2
        Dim Bordo_Tipo As String = "solid"
        Dim Bordo_Colore As String = "#000000"
        Dim Sfondo_Colore As String = "#FFFFFF"
        Dim Font_Weight As String = "bold"
        Dim Font_Size As String = "20"
        Dim Font_Color As String = "#0000CC"
        Dim Text_Allinea_Orizzontale As String = "center"
        Dim Text_Allinea_Verticale As String = "middle"

        'Cella.BgColor = "#FFFFFF"
        'Cella.BorderColor = "#FFFFFF"
        'Cella.Height = "20"

        Cella.InnerHtml = Testo

        ElaboraCellaHTML(Cella,
                Bordo_Spessore,
                Bordo_Tipo,
                Bordo_Colore,
                Sfondo_Colore,
                Text_Allinea_Orizzontale,
                Text_Allinea_Verticale,
                Colspan,
                Font_Weight,
                Font_Size,
                Font_Color)

        ''---Riga vuota -- dopo ogni sezione
        'Riga.Cells(0).BgColor = "#74920E"

    End Sub

    '###########################################################
    Private Sub EXCEL_Formatta_SottoReport_Titolo(ByRef Cella As HtmlTableCell,
                                            ByVal Colspan As Integer,
                                            ByVal Testo As String)

        Dim Bordo_Spessore As Integer = 2
        Dim Bordo_Tipo As String = "solid"
        Dim Bordo_Colore As String = "#000000"
        Dim Sfondo_Colore As String = "#FFFFFF"
        Dim Font_Weight As String = "bold"
        Dim Font_Size As String = "18"
        Dim Font_Color As String = "#0000CC"
        Dim Text_Allinea_Orizzontale As String = "center"
        Dim Text_Allinea_Verticale As String = "middle"

        'Cella.BgColor = "#FFFFFF"
        'Cella.BorderColor = "#FFFFFF"
        'Cella.Height = "18"

        Cella.InnerHtml = Testo


        ElaboraCellaHTML(Cella,
                        Bordo_Spessore,
                        Bordo_Tipo,
                        Bordo_Colore,
                        Sfondo_Colore,
                        Text_Allinea_Orizzontale,
                        Text_Allinea_Verticale,
                        Colspan,
                        Font_Weight,
                        Font_Size,
                        Font_Color)


    End Sub

    '###########################################################
    Private Sub EXCEL_Formatta_Intestazione_Cella(ByRef Cella As HtmlTableCell,
                                                ByVal Colspan As Integer,
                                                ByVal Rowspan As Integer,
                                                ByVal Width As Integer,
                                                ByVal Background As String,
                                                ByVal Testo As String)

        Dim Bordo_Spessore As Integer = 2
        Dim Bordo_Tipo As String = "solid"
        Dim Bordo_Colore As String = "#0000CC"
        Dim Sfondo_Colore As String = "#0000CC"
        Dim Font_Weight As String = ""
        Dim Font_Size As String = "14"
        Dim Font_Color As String = "#FFFFFF"
        Dim Text_Allinea_Orizzontale As String = "center"
        Dim Text_Allinea_Verticale As String = "middle"

        'Cella.BgColor = "#FFFFFF"
        'Cella.BorderColor = "#FFFFFF"
        'Cella.Height = "14"

        Cella.InnerHtml = Testo

        Cella.RowSpan = Rowspan

        If Width <> 0 Then
            Cella.Width = Width
        End If


        ElaboraCellaHTML(Cella,
                        Bordo_Spessore,
                        Bordo_Tipo,
                        Bordo_Colore,
                        Sfondo_Colore,
                        Text_Allinea_Orizzontale,
                        Text_Allinea_Verticale,
                        Colspan,
                        Font_Weight,
                        Font_Size,
                        Font_Color)


    End Sub


    '###########################################################
    Private Sub EXCEL_Formatta_Riga_Cella(ByRef Cella As HtmlTableCell,
                                       ByVal Colspan As Integer,
                                        ByVal Rowspan As Integer,
                                        ByVal Width As Integer,
                                        ByVal Background As String,
                                        ByVal Testo As String)

        Dim Bordo_Spessore As Integer = 2
        Dim Bordo_Tipo As String = "solid"
        Dim Bordo_Colore As String = "#000000"
        Dim Sfondo_Colore As String = "#FFFFFF"
        Dim Font_Weight As String = ""
        Dim Font_Size As String = "12"
        Dim Font_Color As String = "#000000"
        Dim Text_Allinea_Orizzontale As String = "center"
        Dim Text_Allinea_Verticale As String = "middle"

        'Cella.BgColor = "#FFFFFF"
        'Cella.BorderColor = "#FFFFFF"
        'Cella.Height = "12"

        Cella.InnerHtml = Testo

        Cella.RowSpan = Rowspan

        If Width <> 0 Then
            Cella.Width = Width
        End If


        ElaboraCellaHTML(Cella,
                        Bordo_Spessore,
                        Bordo_Tipo,
                        Bordo_Colore,
                        Sfondo_Colore,
                        Text_Allinea_Orizzontale,
                        Text_Allinea_Verticale,
                        Colspan,
                        Font_Weight,
                        Font_Size,
                        Font_Color)


    End Sub

    '###########################################################
    Private Sub EXCEL_Formatta_PiePagina_Cella(ByRef Cella As HtmlTableCell,
                                           ByVal Colspan As Integer,
                                            ByVal Rowspan As Integer,
                                            ByVal Width As Integer,
                                            ByVal Background As String,
                                            ByVal Testo As String)

        Dim Bordo_Spessore As Integer = 2
        Dim Bordo_Tipo As String = "solid"
        Dim Bordo_Colore As String = "#00CCCC"
        Dim Sfondo_Colore As String = "#00CCCC"
        Dim Font_Weight As String = ""
        Dim Font_Size As String = "10"
        Dim Font_Color As String = "#000000"
        Dim Text_Allinea_Orizzontale As String = "center"
        Dim Text_Allinea_Verticale As String = "middle"

        'Cella.BgColor = "#FFFFFF"
        'Cella.BorderColor = "#FFFFFF"
        Cella.Height = "12"

        Cella.InnerHtml = Testo

        Cella.RowSpan = Rowspan

        If Width <> 0 Then
            Cella.Width = Width
        End If


        ElaboraCellaHTML(Cella,
                        Bordo_Spessore,
                        Bordo_Tipo,
                        Bordo_Colore,
                        Sfondo_Colore,
                        Text_Allinea_Orizzontale,
                        Text_Allinea_Verticale,
                        Colspan,
                        Font_Weight,
                        Font_Size,
                        Font_Color)


    End Sub


    '###########################################################
    Private Sub EXCEL_Formatta_Cella(ByRef Cella As HtmlTableCell,
                                    ByVal Colspan As Integer,
                                    ByVal Rowspan As Integer,
                                    ByVal Width As Integer,
                                    ByVal Background As String,
                                    ByVal Testo As String,
                                    Optional ByVal Tipo As enum_FormattazioneExcel = enum_FormattazioneExcel.FE_Non_Specificato)

        Dim Bordo_Spessore As Integer = 2
        Dim Bordo_Tipo As String = "solid"
        Dim Bordo_Colore As String = "#000000"
        'Dim Sfondo_Colore As String = "#FFFFFF"
        Dim Font_Weight As String = ""
        Dim Font_Size As String = "12"
        Dim Font_Color As String = "#000000"
        Dim Text_Allinea_Orizzontale As String = "center"
        Dim Text_Allinea_Verticale As String = "middle"

        'Cella.BgColor = "#FFFFFF"
        'Cella.BorderColor = "#FFFFFF"
        'Cella.Height = "12"

        Cella.InnerHtml = Testo

        Cella.RowSpan = Rowspan

        If Width <> 0 Then
            'Cella.Width = CStr(CInt(Width / 10))
            Cella.Width = CStr(Width)
            'Cella.Width = CStr(20)
        End If

        ElaboraCellaHTML(Cella,
                        Bordo_Spessore,
                        Bordo_Tipo,
                        Bordo_Colore,
                        Background,
                        Text_Allinea_Orizzontale,
                        Text_Allinea_Verticale,
                        Colspan,
                        Font_Weight,
                        Font_Size,
                        Font_Color,
                        Tipo)


    End Sub

End Class
