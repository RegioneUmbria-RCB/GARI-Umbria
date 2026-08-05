Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza

Public Class EtichetteVascheEnologiche
    Inherits System.Web.UI.Page

    ' Private rptEtichetta_OLD As Rpt_EtichettaVascaEnologica
    Private rptEtichetta As Rpt_EtichetteVasche


#Region " ETICHETTE VASCHE "

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

        ' rptEtichetta_OLD = New Rpt_EtichettaVascaEnologica
        rptEtichetta = New Rpt_EtichetteVasche

    End Sub

#End Region

    Dim Log_Errori As String = ""
    Dim strVasche As String = ""
    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    '#################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'Faccio scadere subito la pagina memorizzata nella cache
        Response.Expires = 0

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))


        '#################################################################################
        '#####  Genero il report
        '#################################################################################


        Dim Nome_Documento As String = "EtichetteVasche"
        Dim IdentificazioneDocumento As String = ""
        Dim Piva As String = ""
        Piva = Stringa_Decodifica(CStr(Request.QueryString("p")), AgroKey_EncoderDecoder, Server)

        'Dim cat_cod As enum_CategorieDocumenti
        'cat_cod = enum_CategorieDocumenti.Cantina_ConsistenzeEnologiche


        If Not Me.IsPostBack Then

            Try
                Carica_DatiVasca_NEW()
            Catch exc As Exception
                Log_Errori += "Carica_DatiVasca_NEW: " + vbCrLf + exc.Message + vbCrLf
            End Try


            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            'Dim Path_Errore, Str_Errore_Path As String
            Dim Nome_File As String

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento + ", " + vbCrLf + _
                                "Piva = " + CStr(Piva) + ", " + vbCrLf + _
                                vbCrLf + vbCrLf + vbCrLf + vbCrLf + _
                                Log_Errori



                Nome_File = "Log_Errori_" + Nome_Documento + "_" + CStr(Session("ASG_Utente_Username"))

  
                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server, _
                                                 "Stampe_Cantine", _
                                                 Nome_File, _
                                                 Session("ASG_Utente_Username"), _
                                                 "EtichetteVascheEnologiche.aspx", _
                                                 Log_Errori)


            End If
            '-----------------------------------------

        End If

        '==================================================================

        Try
            Session("Report") = rptEtichetta

        Catch ex As Exception
            Log_Errori += "- Export: " + vbCrLf + ex.Message + vbCrLf
        End Try

        Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))

    End Sub


    ''########################################################################################################################
    'Private Sub Carica_DatiVasca_OLD()

    '    strVasche = Session("strXmlVariabilistampe")

    '    Dim XmlDoc As New System.Xml.XmlDocument
    '    Dim XML_FiltroStampa As System.Xml.XmlElement
    '    Dim XMLs_Vasca As System.Xml.XmlNodeList
    '    Dim XML_Vasca As System.Xml.XmlElement
    '    Dim XMLs_Cantina As System.Xml.XmlNodeList
    '    Dim XML_Cantina As System.Xml.XmlElement

    '    'Carico la stringa xml in un nuovo documento
    '    XmlDoc = New System.Xml.XmlDocument

    '    strVasche = strVasche.Replace("{", "<")
    '    strVasche = strVasche.Replace("}", ">")

    '    XmlDoc.LoadXml(strVasche)


    '    '<CANTINA des="vendita" dimx="700" dimy="360" ce="#1010FF" ci="#1010FF" s="0" fill="0" zoom="1" modalita="3">
    '    '  <VASCA prog="1" tipo="C" des1="1" des2="hl 30" des3="14 PigRubIgp" des4="Pignoletto Rubicone Igp" dimx="100" dimy="100" posx="5" posy="5" ce="#00000" flag_selezione="S" flag_checked="0" s="1" ci="#00FF80" flag_enabled="1" fill="1"/>
    '    '  <VASCA prog="2" tipo="C" des1="2" des2="hl 30" des3="14 BiaSill" des4="Bianco del Sillaro Igp" dimx="100" dimy="100" posx="110" posy="5" ce="#00000" flag_selezione="S" flag_checked="0" s="1" ci="#FFFF00" flag_enabled="1" fill="1"/>
    '    '  <VASCA prog="3" tipo="C" des1="3" des2="hl 30" des3="14 Char" des4="Chardonnay Rubicone igp" dimx="100" dimy="100" posx="215" posy="5" ce="#00000" flag_selezione="S" flag_checked="1" s="6" ci="#8080FF" flag_enabled="1" fill="1"/>
    '    '  <VASCA prog="4" tipo="C" des1="4" des2="hl 30" des3="14 Trebb" des4="Trebbiano del Rubicone Igp" dimx="100" dimy="100" posx="320" posy="5" ce="#00000" flag_selezione="S" flag_checked="0" s="1" ci="#80FFFF" flag_enabled="1" fill="1"/>
    '    '  <VASCA prog="5" tipo="C" des1="5" des2="hl 30" des3="14 Sang" des4="Sangiovese Rubicone Igp" dimx="100" dimy="100" posx="425" posy="5" ce="#00000" flag_selezione="S" flag_checked="0" s="1" ci="#FF0000" flag_enabled="1" fill="1"/>
    '    '  <VASCA prog="6" tipo="C" des1="6" des2="hl 30" des3="14 CabSauv" des4="Cabernet Sauvignon Rubicone Igp" dimx="100" dimy="100" posx="530" posy="5" ce="#00000" flag_selezione="S" flag_checked="0" s="1" ci="#FF0000" flag_enabled="1" fill="1"/>
    '    '  <VASCA prog="7" tipo="C" des1="7" des2="hl 30" des3="" des4="" dimx="100" dimy="100" posx="425" posy="250" ce="#00000" flag_selezione="S" flag_checked="0" s="1" ci="#FFFFFF" flag_enabled="1" fill="1"/>
    '    '  <VASCA prog="8" tipo="C" des1="8" des2="hl 10" des3="" des4="" dimx="50" dimy="50" posx="530" posy="290" ce="#00000" flag_selezione="S" flag_checked="0" s="1" ci="#FFFFFF" flag_enabled="1" fill="1"/>
    '    '  <VASCA prog="9" tipo="R" des1="trasporto1" des2="hl 10" des3="" des4="" dimx="20" dimy="35" posx="600" posy="200" ce="#00000" flag_selezione="S" flag_checked="0" s="1" ci="#FFFFFF" flag_enabled="1" fill="1"/>
    '    '  <VASCA prog="10" tipo="R" des1="trasporto2" des2="hl 10" des3="14 Stram" des4="Vino da Uve Stramature" dimx="20" dimy="35" posx="600" posy="160" ce="#00000" flag_selezione="S" flag_checked="0" s="1" ci="#FFFF80" flag_enabled="1" fill="1"/>
    '    '</CANTINA>

    '    Dim i, j As Integer

    '    If XmlDoc.HasChildNodes Then

    '        XML_FiltroStampa = XmlDoc.SelectSingleNode("FiltroStampa")

    '        XMLs_Cantina = XML_FiltroStampa.GetElementsByTagName("CANTINA")

    '        For i = 0 To XMLs_Cantina.Count - 1

    '            XML_Cantina = XMLs_Cantina(i)

    '            XMLs_Vasca = XML_Cantina.GetElementsByTagName("VASCA")

    '            For j = 0 To XMLs_Vasca.Count - 1

    '                XML_Vasca = XMLs_Vasca(j)

    '                If XML_Vasca.GetAttribute("flag_checked") = 1 Then

    '                    CType(rptEtichetta_OLD.Section2.ReportObjects("TextCodiceVasca"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = XML_Vasca.GetAttribute("des1")
    '                    CType(rptEtichetta_OLD.Section8.ReportObjects("TextCapacita"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Capacità: " & XML_Vasca.GetAttribute("des2")

    '                    'Dim strLinea As String
    '                    'Dim strLineaCod As String
    '                    'Dim vetSplit() As String
    '                    'vetSplit = XML_Vasca.GetAttribute("des3").Split("(")
    '                    'strLineaCod = vetSplit(0).Trim

    '                    'strLinea = LineaDes_from_LineaCodDes(Server, Session, Page, strLineaCod)
    '                    'strLinea & " " &
    '                    CType(rptEtichetta_OLD.Section7.ReportObjects("TextReferenza"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = XML_Vasca.GetAttribute("des4")
    '                    CType(rptEtichetta_OLD.Section6.ReportObjects("TextLotto"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Lotto: " & XML_Vasca.GetAttribute("des3")


    '                    Exit For

    '                End If
    '            Next

    '        Next


    '    End If

    'End Sub

    '########################################################################################################################
    Private Sub Carica_DatiVasca_NEW()

        '#################################################################################
        '#####  Recupero dati dalla QueryString 
        '#################################################################################

        Dim Opt_Identificativo As Boolean
        Dim Opt_Capacita As Boolean
        Dim Opt_Linea As Boolean
        Dim Opt_MatDes As Boolean
        Dim Opt_GradoBabo As Boolean
        Dim Opt_ContoLav As Boolean
        Dim Opt_Colore As Boolean
        Dim Opt_AnnoProd As Boolean
        Dim Opt_Lotto As Boolean
        Dim Opt_Regolamento As Boolean
        Dim Opt_AttoApprovazione As Boolean
        Dim Opt_Qta As Boolean
        Dim Piva As String

        'Dim Str_Identificativo As String = ""
        'Dim Str_Capacita As String = ""
        'Dim Str_Linea As String = ""
        'Dim Str_GradoBabo As String = ""
        'Dim Str_ContoLav As String = ""
        'Dim Str_Colore As String = ""
        'Dim Str_AnnoProd As String = ""
        'Dim Str_Regolamento As String = ""
        'Dim Str_AttoApprovazione As String = ""
        'Dim Str_Qta As String = ""

        Opt_Identificativo = Stringa_Decodifica(CStr(Request.QueryString("oid")), AgroKey_EncoderDecoder, Server)
        Opt_Capacita = Stringa_Decodifica(CStr(Request.QueryString("ocap")), AgroKey_EncoderDecoder, Server)
        Opt_Linea = Stringa_Decodifica(CStr(Request.QueryString("olin")), AgroKey_EncoderDecoder, Server)
        Opt_GradoBabo = Stringa_Decodifica(CStr(Request.QueryString("obabo")), AgroKey_EncoderDecoder, Server)
        Opt_ContoLav = Stringa_Decodifica(CStr(Request.QueryString("oclav")), AgroKey_EncoderDecoder, Server)
        Opt_Colore = Stringa_Decodifica(CStr(Request.QueryString("ocol")), AgroKey_EncoderDecoder, Server)
        Opt_AnnoProd = Stringa_Decodifica(CStr(Request.QueryString("oap")), AgroKey_EncoderDecoder, Server)
        Opt_Regolamento = Stringa_Decodifica(CStr(Request.QueryString("oreg")), AgroKey_EncoderDecoder, Server)
        Opt_AttoApprovazione = Stringa_Decodifica(CStr(Request.QueryString("oaa")), AgroKey_EncoderDecoder, Server)
        Opt_Qta = Stringa_Decodifica(CStr(Request.QueryString("oqta")), AgroKey_EncoderDecoder, Server)
        Opt_MatDes = Stringa_Decodifica(CStr(Request.QueryString("osem")), AgroKey_EncoderDecoder, Server)
        Opt_Lotto = Stringa_Decodifica(CStr(Request.QueryString("olot")), AgroKey_EncoderDecoder, Server)

        Piva = Stringa_Decodifica(CStr(Request.QueryString("p")), AgroKey_EncoderDecoder, Server)

        'Str_Identificativo = Stringa_Decodifica(CStr(Request.QueryString("id")), AgroKey_EncoderDecoder, Server)
        'Str_Capacita = Stringa_Decodifica(CStr(Request.QueryString("cap")), AgroKey_EncoderDecoder, Server)
        'Str_Linea = Stringa_Decodifica(CStr(Request.QueryString("lin")), AgroKey_EncoderDecoder, Server)
        'Str_GradoBabo = Stringa_Decodifica(CStr(Request.QueryString("babo")), AgroKey_EncoderDecoder, Server)
        'Str_ContoLav = Stringa_Decodifica(CStr(Request.QueryString("clav")), AgroKey_EncoderDecoder, Server)
        'Str_Colore = Stringa_Decodifica(CStr(Request.QueryString("col")), AgroKey_EncoderDecoder, Server)
        'Str_AnnoProd = Stringa_Decodifica(CStr(Request.QueryString("ap")), AgroKey_EncoderDecoder, Server)
        'Str_Regolamento = Stringa_Decodifica(CStr(Request.QueryString("reg")), AgroKey_EncoderDecoder, Server)
        'Str_AttoApprovazione = Stringa_Decodifica(CStr(Request.QueryString("aa")), AgroKey_EncoderDecoder, Server)
        'Str_Qta = Stringa_Decodifica(CStr(Request.QueryString("qta")), AgroKey_EncoderDecoder, Server)

        ' CType(rptEtichetta.Section2.ReportObjects("TextCodiceVasca"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = XML_Vasca.GetAttribute("des1")
        ' CType(rptEtichetta.Section8.ReportObjects("TextCapacita"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Capacità: " & XML_Vasca.GetAttribute("des2")
        ' CType(rptEtichetta.Section7.ReportObjects("TextReferenza"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = XML_Vasca.GetAttribute("des4")
        '  CType(rptEtichetta.Section6.ReportObjects("TextLotto"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Lotto: " & XML_Vasca.GetAttribute("des3")

        '1) dichiarare e creare il dataset
        Dim DSEtichette As New DS_EtichetteVasche
        '2) dichiarare la riga
        Dim DSEticRow As DS_EtichetteVasche.DT_EtichetteVascheRow


        Dim DT_Etichette As DataTable
        Dim Lotto As String

        DT_Etichette = Session("DT_Etichette")
        Session("DT_Etichette") = Nothing

        Dim objMovXReport As New AgronicaCoreContabDAL.MovimentixReport_R

        If Not IsNothing(DT_Etichette) AndAlso DT_Etichette.Rows.Count > 0 Then

            Dim i As Integer

            For i = 0 To DT_Etichette.Rows.Count - 1

                'riga= dataset.datatable.newrow
                DSEticRow = DSEtichette.DT_EtichetteVasche.NewDT_EtichetteVascheRow

                Lotto = DT_Etichette.Rows(i).Item("Lotto")

                '20/07/2020: richiesta di ruggeri di poter stampare più etichette della stessa vasca
                'il campo Identificativo serve per lo split del gruppo sul report
                '-> lo  valorizzo con un contatore mentre il vero Identificativo vasca lo salvo sul nuovo campo del dataset IDVasca 
                'If Opt_Identificativo = True And DT_Etichette.Rows(i).Item("Identificativo") <> "" Then
                '    DSEticRow.Identificativo = DT_Etichette.Rows(i).Item("Identificativo")
                'Else
                '    DSEticRow.Identificativo = ""
                'End If
                DSEticRow.Identificativo = i + 1
                If Opt_Identificativo = True And DT_Etichette.Rows(i).Item("Identificativo") <> "" Then
                    DSEticRow.IDVasca = DT_Etichette.Rows(i).Item("Identificativo")
                Else
                    DSEticRow.IDVasca = ""
                End If

                If Opt_Capacita = True And DT_Etichette.Rows(i).Item("Capacita") <> "" Then
                    DSEticRow.Capacita = "Capacità: " & DT_Etichette.Rows(i).Item("Capacita")
                Else
                    DSEticRow.Capacita = ""
                End If

                DSEticRow.Linea = ""
                If Opt_Linea = True And DT_Etichette.Rows(i).Item("Linea") <> "" Then
                    DSEticRow.Linea = DT_Etichette.Rows(i).Item("Linea")
                    'Else
                    '    DSEticRow.Linea = ""
                End If
                'il semilavorato è alternativo al nome della linea
                If Opt_MatDes = True And DT_Etichette.Rows(i).Item("Semilavorato") <> "" Then
                    DSEticRow.Linea = DT_Etichette.Rows(i).Item("Semilavorato")
                    'Else
                    '    DSEticRow.Linea = ""
                End If

                If Opt_Colore = True And DT_Etichette.Rows(i).Item("Colore") <> "" Then
                    DSEticRow.Colore = DT_Etichette.Rows(i).Item("Colore")
                Else
                    DSEticRow.Colore = ""
                End If

                If Opt_AnnoProd = True And DT_Etichette.Rows(i).Item("AnnoProduzione") <> "" Then
                    DSEticRow.AnnoProduzione = "Anno Produzione: " & DT_Etichette.Rows(i).Item("AnnoProduzione")
                Else
                    DSEticRow.AnnoProduzione = ""
                End If

                If Opt_Lotto = True And DT_Etichette.Rows(i).Item("Lotto") <> "" Then
                    DSEticRow.Lotto = "Lotto: " & DT_Etichette.Rows(i).Item("lotto")
                Else
                    DSEticRow.Lotto = ""
                End If

                If Opt_GradoBabo = True And DT_Etichette.Rows(i).Item("Grado_Babo") <> "" Then
                    DSEticRow.Grado_Babo = "Grado Babo: " & DT_Etichette.Rows(i).Item("Grado_Babo")
                Else
                    DSEticRow.Grado_Babo = ""
                End If

                If Opt_Regolamento = True And DT_Etichette.Rows(i).Item("Regolamento") <> "" Then
                    DSEticRow.Regolamento = DT_Etichette.Rows(i).Item("Regolamento")
                Else
                    DSEticRow.Regolamento = ""
                End If

                If Opt_AttoApprovazione = True And DT_Etichette.Rows(i).Item("AttoApprovazione") <> "" Then
                    DSEticRow.AttoApprovazione = "Num. Idoneità: " & DT_Etichette.Rows(i).Item("AttoApprovazione")
                Else
                    If Opt_AttoApprovazione = True And Lotto <> "" Then
                        'il numero di approvazione il giaslan non la passo, devo leggerlo
                        Dim Num_idoneita As String
                        Num_idoneita = objMovXReport.NumeroIdoneita_Classificazioni_from_Lotto(Piva, Lotto, Date.Today, "", objParametri_Server)
                        If Num_idoneita <> "" Then
                            DSEticRow.AttoApprovazione = "Num. Idoneità: " & Num_idoneita
                        End If
                    Else
                        DSEticRow.AttoApprovazione = ""
                    End If
                End If

                If Opt_ContoLav = True And DT_Etichette.Rows(i).Item("ContoLav") <> "" Then
                    DSEticRow.ContoLav = "C/Lav: " & DT_Etichette.Rows(i).Item("ContoLav")
                Else
                    DSEticRow.ContoLav = ""
                End If

                If Opt_Qta = True And DT_Etichette.Rows(i).Item("Qta") <> "" Then
                    DSEticRow.Qta = "Quantità: " & DT_Etichette.Rows(i).Item("Qta")
                Else
                    DSEticRow.Qta = ""
                End If

                'dataset.datatble.addrow(row)
                DSEtichette.DT_EtichetteVasche.AddDT_EtichetteVascheRow(DSEticRow)

            Next

            DSEtichette.DT_EtichetteVasche.AcceptChanges()

            Try

                rptEtichetta.SetDataSource(DSEtichette)

            Catch ex As Exception
                Log_Errori += "- SetDataSource: " + vbCrLf + ex.Message + vbCrLf
            End Try

        Else
            Dim debug As Boolean = True
        End If

    End Sub



End Class
