Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Trasportatori_XLS
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
    'Dim Codice_Conferente As String
    Dim Codice_Specie As String
    'Dim Str_FiltroConf As String
    Dim Str_FiltroSpecie As String
    Dim Piva As String
    Dim Sa_Cod, Fabbricato_Cod As Integer
    Dim RagSoc_Impresa As String
    Dim Descr_Specie As String
    Dim Descr_Magazzino As String

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri

    '#########################################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim Riga As HtmlTableRow
        Dim Numero_Colonne As Integer = 0
        Dim objImpreseR As New AgronicaCoreAnagrafeDAL.Imprese_Read

        'La Pagina deve essere visualizzata come un foglio Excel
        Response.ContentType = "application/vnd.ms-excel"
        Response.AddHeader("Content-Disposition", "inline; filename = Trasportatori.xls")

        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato

        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        '##############################################################
        '############  Lettura Parametri Query String #################
        '##############################################################

        Try

            Data_Da = Stringa_Decodifica(CStr(Request.QueryString("dd")), _
                             AgroKey_EncoderDecoder, _
                             Server)

            Data_A = Stringa_Decodifica(CStr(Request.QueryString("da")), _
                             AgroKey_EncoderDecoder, _
                             Server)

            Piva = Stringa_Decodifica(CStr(Request.QueryString("p")), _
                                        AgroKey_EncoderDecoder, _
                                        Server)

            Sa_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("s")), _
                                      AgroKey_EncoderDecoder, _
                                      Server))

            Fabbricato_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("f")), _
                                      AgroKey_EncoderDecoder, _
                                      Server))

            Descr_Specie = Stringa_Decodifica(CStr(Request.QueryString("spe")), _
                                                AgroKey_EncoderDecoder, _
                                                Server)

            Descr_Magazzino = Stringa_Decodifica(CStr(Request.QueryString("mag")), _
                                                    AgroKey_EncoderDecoder, _
                                                    Server)

            RagSoc_Impresa = Stringa_Decodifica(CStr(Request.QueryString("rs")), _
                                               AgroKey_EncoderDecoder, _
                                               Server)


            If RagSoc_Impresa = "" Then
                RagSoc_Impresa = objImpreseR.RagSoc_from_Piva(Piva, objParametri_Server)
            End If

            Codice_Specie = Stringa_Decodifica(CStr(Request.QueryString("cs")), _
                                      AgroKey_EncoderDecoder, _
                                      Server)

            If Codice_Specie = "0" Then
                Codice_Specie = ""
                'il codice non è stato passato, 
                'perchè si vogliono cercare tutte le specie
                'o il range di specie selezionato
            End If

            Str_FiltroSpecie = Session("Str_Codici_Specie")

            If Str_FiltroSpecie <> "" Then
                'è stato selezionato un range di codici
                Str_FiltroSpecie = " AND MP_Raccolta.Cod_Articolo IN " & Str_FiltroSpecie
            Else
                Str_FiltroSpecie = ""
            End If


            'Codice_Conferente = Stringa_Decodifica(CStr(Request.QueryString("cc")), _
            '                        AgroKey_EncoderDecoder, _
            '                        Server)

            'If Codice_Conferente = "0" Then
            '    Codice_Conferente = ""
            '    'il codice non è stato passato, 
            '    'perchè si vogliono cercare tutti i conferenti
            '    'o il range di conferenti selezionato
            'Else
            '    Codice_Conferente = CStr(Codice_Conferente)
            'End If


            'Str_FiltroConf = Session("Str_Codici_Conferenti")

            'If Str_FiltroConf <> "" Then
            '    'è stato selezionato un range di codici
            '    Str_FiltroConf = " AND Risorse_Umane_Conferenti.settore_Des IN " & Str_FiltroConf
            'Else
            '    Str_FiltroConf = ""
            'End If




        Catch ex As Exception
            Riga = New HtmlTableRow
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(0).ColSpan = Numero_Colonne
            Riga.Cells(0).InnerHtml = "Dati da querystring: " + ex.Message
            Me.TableExcel.Rows.Add(Riga)
        End Try


        '##############################################################
        '####################  LETTURA DATI ###########################
        '##############################################################

        Dim DT As DataTable
        Dim i As Integer

        Try

            'DT = NewCom_ADD_Trasportatori_Leggi(Server, Session, Page, _
            '                                    Piva, _
            '                                    Sa_Cod, _
            '                                    Fabbricato_Cod, _
            '                                    Codice_Specie, _
            '                                    Data_Da, _
            '                                    Data_A, _
            '                                    Str_FiltroSpecie, _
            '                                    "")

            Dim ADD As New AgronicaCoreStampeDAL.AccettazioneDaDiversi

            DT = ADD.Trasportatori_XLS(Session("ASG_ProgressivoGIAS"), _
                                    Piva, _
                                     Sa_Cod, _
                                     Fabbricato_Cod, _
                                     Codice_Specie, _
                                     Data_Da, _
                                     Data_A, _
                                     Str_FiltroSpecie, _
                                     "", _
                                     "", _
                                     objParametri_Server)

            If Not IsNothing(DT) Then

                For i = 0 To DT.Rows.Count - 1


                    With DT.Rows(i)

                        .Item("Numero_Bolla") = Ricava_NumeroDocumento_Con_Sequenza(.Item("Doc_Numero_Sin"), _
                                                                                    .Item("Doc_Numero"), _
                                                                                    .Item("Doc_Numero_Des"), _
                                                                                    .Item("Lunghezza_Sin"), _
                                                                                    .Item("Lunghezza_Centro"), _
                                                                                    .Item("Lunghezza_Des"), _
                                                                                    .Item("CarattereFormattazione"))

                        .Item("Numero_Conf") = Ricava_NumeroDocumento_Senza_Sequenza(.Item("Doc_Numero_Sin_Conf"), _
                                                                                    .Item("Doc_Numero_Conf"), _
                                                                                    .Item("Doc_Numero_Des_Conf"))

                        ' 'netto trasportato = peso lordo
                        '.Item("Netto_Trasportato") = Calcola_PesoLordo(CInt(.Item("Tipo_Peso")), CDbl(.Item("Peso")), CDbl(.Item("Tara_Imballi")))
                        'netto trasportato = peso netto
                        .Item("Netto_Trasportato") = Calcola_PesoNetto(CInt(.Item("Tipo_Peso")), CDbl(.Item("Peso")), CDbl(.Item("Tara_Imballi")))

                        If InStr(CStr(.Item("Targhe")), "_") > 0 Then
                            .Item("Targa_Automezzo") = CStr(.Item("Targhe")).Split("_")(0)
                            .Item("Targa_Rimorchio") = CStr(.Item("Targhe")).Split("_")(1)
                        Else
                            .Item("Targa_Automezzo") = ""
                            .Item("Targa_Rimorchio") = ""
                        End If

                    End With

                Next

                DT.Columns.Remove("Doc_Numero_Sin")
                DT.Columns.Remove("Doc_Numero")
                DT.Columns.Remove("Doc_Numero_Des")
                DT.Columns.Remove("Doc_Numero_Sin_Conf")
                DT.Columns.Remove("Doc_Numero_Conf")
                DT.Columns.Remove("Doc_Numero_Des_Conf")
                DT.Columns.Remove("Lunghezza_Sin")
                DT.Columns.Remove("Lunghezza_Centro")
                DT.Columns.Remove("Lunghezza_Des")
                DT.Columns.Remove("CarattereFormattazione")
                DT.Columns.Remove("Peso")
                DT.Columns.Remove("Tara_Imballi")
                DT.Columns.Remove("Tipo_Peso")
                DT.Columns.Remove("Targhe")

            End If


        Catch ex As Exception
            Riga = New HtmlTableRow
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(0).ColSpan = Numero_Colonne
            Riga.Cells(0).InnerHtml = "Lettura dei dati: " + ex.Message
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

        Try

            Dim i, j, k As Integer
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

                    Select Case Colonna_Dt

                        Case "numero_bolla"
                            Riga.Cells(k).InnerHtml = "N. Bolla"
                        Case "data_bolla"
                            Riga.Cells(k).InnerHtml = "Data Bolla"
                        Case "numero_conf"
                            Riga.Cells(k).InnerHtml = "N. DDT<br/>Conferimento"
                        Case "data_conf"
                            Riga.Cells(k).InnerHtml = "Data DDT<br/>Conferimento"
                        Case CStr("Piva_Produttore").ToLower
                            Riga.Cells(k).InnerHtml = "P.IVA<br/>Produttore"
                        Case CStr("RagSoc_Produttore").ToLower
                            Riga.Cells(k).InnerHtml = "Produttore"
                        Case CStr("Netto_Trasportato").ToLower
                            Riga.Cells(k).InnerHtml = "Netto<br/>Trasportato"
                        Case "codice_zona"
                            Riga.Cells(k).InnerHtml = "Codice Zona"
                        Case CStr("RagSoc_Trasportatore").ToLower
                            Riga.Cells(k).InnerHtml = "Trasportatore"
                        Case CStr("Codice_Conferente").ToLower
                            Riga.Cells(k).InnerHtml = "Codice<br/>Conferente"
                        Case CStr("RagSoc_Conferente").ToLower
                            Riga.Cells(k).InnerHtml = "Conferente"
                        Case CStr("targa_automezzo").ToLower
                            Riga.Cells(k).InnerHtml = "Targa<br/>Automezzo"
                        Case CStr("targa_rimorchio").ToLower
                            Riga.Cells(k).InnerHtml = "Targa<br/>Rimorchio"

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

                            Case CStr("Piva_Produttore").ToLower
                                'aggiungo uno spazio davanti x salvare gli zeri...
                                Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dt_Finale.Rows(i).Item(j)), "&nbsp;" & Dt_Finale.Rows(i).Item(j), "&nbsp;")

                            Case "numero_conf"
                                If Dt_Finale.Rows(i).Item(j) <> "" Then
                                    'aggiungo uno spazio davanti per vedere visualizzato il numero e non una data
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
            Riga.Cells(0).InnerHtml = "Creazione excel: " + ex.Message
            Me.TableExcel.Rows.Add(Riga)

        End Try


    End Sub




End Class

