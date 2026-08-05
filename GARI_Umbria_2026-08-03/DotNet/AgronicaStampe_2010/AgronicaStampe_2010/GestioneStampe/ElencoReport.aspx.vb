Imports System.IO
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUtility

Public Class ElencoReport
    Inherits System.Web.UI.Page

    '----- objParametri
    Dim objParametri_Utenti As AgronicaCoreParametri
    Dim objParametri_Server As AgronicaCoreParametri

    Dim QS_Cat_Cod As enum_CategorieDocumenti

    Private Const COL_PIVA As Integer = 0
    Private Const COL_DOC_COD As Integer = 1
    Private Const COL_SOTTOCARTELLA As Integer = 2
    Private Const COL_NOME_FILE As Integer = 3
    Private Const COL_CMD_DOWNLOAD As Integer = 4
    Private Const COL_INIZIO As Integer = 5
    Private Const COL_FINE As Integer = 6

    '######################################################################################################
    Private Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto

    End Sub

    ' ##################################################################################################
    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)


        Dim strJS1 As New StringBuilder
        strJS1.AppendLine("$(document).ready(function () { ")
        strJS1.AppendLine("      window.close() ")
        strJS1.AppendLine(" });")

        ScriptManager.RegisterStartupScript(Me.Page, Me.Page.GetType(),
                                            String.Format("jQuery_{0}", Me.Page.ClientID),
                                            strJS1.ToString, True)

    End Sub


    ' ##################################################################################################
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato
        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If

        '##############################################################
        '###################### QUERYSTRING ###########################
        '##############################################################
        If Not IsNothing(Request.QueryString("cc")) Then
            QS_Cat_Cod = Stringa_Decodifica(Request.QueryString("cc").ToString, AgroKey_EncoderDecoder)
        Else
            QS_Cat_Cod = enum_CategorieDocumenti.RegistriCampagna 'default
        End If


        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        If Not Me.IsPostBack Then
            CaricaElenco(QS_Cat_Cod)
        End If

    End Sub

    ' ##################################################################################################
    Private Sub CaricaElenco(ByVal Cat_Cod As enum_CategorieDocumenti)

        Dim strFiltroImpianti As String = ""

        Select Case Cat_Cod

            Case enum_CategorieDocumenti.RegistriCampagna

                Dim strXmlVariabilistampe As String = Session("strXmlVariabilistampe")

                Dim XmlDoc As New System.Xml.XmlDocument
                Dim XML_FiltroStampa As System.Xml.XmlElement
                Dim XMLs_VariabiliStampe As System.Xml.XmlNodeList
                Dim XML_VariabiliStampe As System.Xml.XmlElement

                'Carico la stringa xml in un nuovo documento
                XmlDoc = New System.Xml.XmlDocument
                XmlDoc.LoadXml(strXmlVariabilistampe)

                If XmlDoc.HasChildNodes Then

                    'Ricavo i parametri che servono

                    XML_FiltroStampa = XmlDoc.SelectSingleNode("ParametriAgronicaStampe_2010")

                    Session("ASG_Utente_Username") = XML_FiltroStampa.GetAttribute("username")

                    XMLs_VariabiliStampe = XML_FiltroStampa.GetElementsByTagName("VariabiliStampe")

                    Dim strFiltroImpianto As String

                    For i = 0 To XMLs_VariabiliStampe.Count - 1

                        XML_VariabiliStampe = XMLs_VariabiliStampe.Item(i)

                        strFiltroImpianto = " (Alert_Entita.PIVA='" & XML_VariabiliStampe.GetAttribute("piva") & "' " &
                                            " AND Alert_Entita.SA_COD=" & XML_VariabiliStampe.GetAttribute("sa_cod") &
                                            " AND Alert_Entita.APPEZZA=" & XML_VariabiliStampe.GetAttribute("appezza") &
                                            " AND Alert_Entita.Id_Imp=" & XML_VariabiliStampe.GetAttribute("id_reg") &
                                            " ) OR"

                        strFiltroImpianti &= strFiltroImpianto

                    Next

                    'tolgo l'ultimo OR
                    strFiltroImpianti = "(" & Left(strFiltroImpianti, strFiltroImpianti.Length - 2) & ")"

                End If

        End Select

        Try
            
            Dim objAlert As New AgronicaCoreScadenziario.Alert_Elenco_R
            Dim dt As DataTable = objAlert.Leggi_ElencoAllegati(Cat_Cod,
                                                                strFiltroImpianti,
                                                                "",
                                                                "",
                                                                objParametri_Server)

            If IsNothing(dt) OrElse dt.Rows.Count = 0 Then
                VisualizzaElencoVuoto(Cat_Cod)
                Exit Sub
            End If

            Dim dtElenco As New DataTable
            dtElenco.Columns.Add(New DataColumn("Piva", GetType(String)))
            dtElenco.Columns.Add(New DataColumn("Documento_Cod", GetType(Integer)))
            dtElenco.Columns.Add(New DataColumn("SottoCartella", GetType(String)))
            dtElenco.Columns.Add(New DataColumn("NomeFile", GetType(String)))
            dtElenco.Columns.Add(New DataColumn("Inizio", GetType(String)))
            dtElenco.Columns.Add(New DataColumn("Fine", GetType(String)))

            For i = 0 To dt.Rows.Count - 1

                Dim nomeFile As String = If(IsDBNull(dt.Rows(i).Item("Allegati_Documenti_NomeFile")), "", dt.Rows(i).Item("Allegati_Documenti_NomeFile"))

                If Not String.IsNullOrEmpty(nomeFile) Then

                    Dim dr As DataRow = dtElenco.NewRow

                    dr.Item("Piva") = dt.Rows(i).Item("Allegati_Documenti_Piva")
                    dr.Item("Documento_Cod") = CInt(dt.Rows(i).Item("Allegati_Documenti_Cod"))
                    dr.Item("SottoCartella") = If(IsDBNull(dt.Rows(i).Item("Sottocartella")), "", dt.Rows(i).Item("Sottocartella"))
                    dr.Item("NomeFile") = nomeFile

                    If Not IsDBNull(dt.Rows(i).Item("Validita_Inizio")) AndAlso (IsDate(dt.Rows(i).Item("Validita_Inizio"))) Then
                        dr.Item("Inizio") = CDate(dt.Rows(i).Item("Validita_Inizio")).ToShortDateString
                    Else
                        dr.Item("Inizio") = ""
                    End If

                    If Not IsDBNull(dt.Rows(i).Item("Validita_Fine")) AndAlso (IsDate(dt.Rows(i).Item("Validita_Fine"))) Then
                        dr.Item("Fine") = CDate(dt.Rows(i).Item("Validita_Fine")).ToShortDateString
                    Else
                        dr.Item("Fine") = ""
                    End If

                    dtElenco.Rows.Add(dr)
                End If

            Next

            dgrReport.DataSource = dtElenco
            dgrReport.DataBind()

        Catch ex As Exception
            'MsgBox("Errore: " & ex.Message)
            Dim messaggio As String = "Errore: " & ex.Message
            Dim js As String = "<script language=javascript> alert('" & messaggio & "') </script>"
            If Not ClientScript.IsStartupScriptRegistered("clientscript_messagebox") Then
                ClientScript.RegisterStartupScript(Me.GetType(),"clientscript_messagebox", js)
            End If
        End Try

    End Sub

    Private Sub VisualizzaElencoVuoto(ByVal Cat_Cod As Integer)

        ElencoVuoto.Visible = True

        Select Case Cat_Cod

            Case enum_CategorieDocumenti.RegistriCampagna
                Me.ElencoVuoto.InnerText = "Non sono stati archiviati report per gli impianti selezionati."

            Case Else
                'Case enum_CategorieDocumenti.LibroGiornale
                Me.ElencoVuoto.InnerText = "Non sono stati archiviati report per la stampa in oggetto."

        End Select

    End Sub
    
    Protected Sub lnkDownload_Click(ByVal sender As ImageButton, ByVal e As EventArgs)

        Dim inError As Boolean = False
        Dim percorsoFile As String = ""

        Try

            Dim clickedRow As GridViewRow = TryCast(sender.NamingContainer, GridViewRow)
            
            Dim nomeFile As String = clickedRow.Cells(COL_NOME_FILE).Text
            Dim sottoCartella As String = clickedRow.Cells(COL_SOTTOCARTELLA).Text.Replace("&nbsp;", "")

            percorsoFile = OttieniFile(nomeFile, sottoCartella, objParametri_Server:=Session("ASG_objParametri_Server"))

            If String.IsNullOrEmpty(percorsoFile) OrElse Not File.Exists(percorsoFile) Then
                Throw New Exception("File non trovato")
            End If

        Catch ex As Exception
            inError = True
            'MsgBox("Errore: " & ex.Message)

            Dim messaggio As String = "Errore: " & ex.Message
            Dim js As String = "<script language=javascript> alert('" & messaggio & "') </script>"
            If Not ClientScript.IsStartupScriptRegistered("clientscript_messagebox") Then
                ClientScript.RegisterStartupScript(Me.GetType(),"clientscript_messagebox", js)
            End If

        End Try

        If Not inError Then
            DownloadFile(percorsoFile, Response)
        End If
    End Sub

    Private Shared Function LeggiPercorsoAllegati(ByRef objParametriServer As AgronicaCoreParametri) As String
        Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim percorso As String = objConfSiti.Leggi_Valore(6,
                                                          "GestioneAllegati_Repository",
                                                          "", "", objParametriServer)

        If String.IsNullOrEmpty(percorso) Then
            percorso = "C:\GIASLAN\AgronicaStampe_Allegati\" 'default
        End If

        Return FileSystemHelper.AggiungiSlashSeNonEsiste(percorso)

    End Function

    Private Function OttieniFile(ByVal nomeFile As String,
                                 ByVal sottoCartella As String,
                                 ByRef objParametri_Server As AgronicaCoreParametri
                                 ) As String

        Dim xRisp As String = ""
        Try

            Dim SitoAllegati As String = LeggiPercorsoAllegati(objParametri_Server)
            
            Dim link As String = ""

            'TODO: Nel nomeFile potrei avere già il percorso completo

            If nomeFile.Contains("/") Then
                link = SitoAllegati & nomeFile
            Else
                'controllo se sono in modifica o in inserimento
                If IsDBNull(nomeFile) Then
                    link = SitoAllegati & "TEMP\" & nomeFile
                Else
                    Dim sottoCartellaLoc As String = ""
                    If Not String.IsNullOrEmpty(sottoCartella) Then
                        sottoCartellaLoc = FileSystemHelper.AggiungiSlashSeNonEsiste(sottoCartella)
                    End If

                    link = SitoAllegati & sottoCartellaLoc & nomeFile
                End If
            End If

            xRisp = link


        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

        Return xRisp

    End Function

    Private Shared Sub DownloadFile(ByVal filePath As String, ByVal response As System.Web.HttpResponse)

        Dim file As New FileInfo(filePath)

        If file.Exists Then
            response.Clear()
            response.AddHeader("Content-Disposition", "attachment; filename=" & file.Name)
            response.AddHeader("Content-Length", file.Length.ToString())
            'response.ContentType = "application/octet-stream"
            response.WriteFile(file.FullName)
            response.[End]()
            response.Close()
            file = Nothing
        Else
            Throw New Exception("File non trovato")
        End If
    End Sub

End Class