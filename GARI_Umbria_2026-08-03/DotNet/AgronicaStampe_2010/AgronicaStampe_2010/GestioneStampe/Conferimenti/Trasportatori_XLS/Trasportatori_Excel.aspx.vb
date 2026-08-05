Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Trasportatori_Excel
    Inherits System.Web.UI.Page

    Protected WithEvents TableExcel As System.Web.UI.HtmlControls.HtmlTable

    Dim Data_Da, Data_A As String
    Dim Cod_Articolo As String
    Dim Codice_Esterno As String
    Dim FiltroAggArticoli As String
    Dim FiltroAggConferenti As String
    Dim Piva As String
    Dim Sa_Cod, Mat_Cod, Veg_Cod, Cul_Cod As Integer
    Dim RagSoc_Impresa As String
    Dim Codice_Conferente As String
    'Dim Descr_Articolo As String

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

            Data_Da = Stringa_Decodifica(CStr(Request.QueryString("dd")),
                             AgroKey_EncoderDecoder,
                             Server)

            Data_A = Stringa_Decodifica(CStr(Request.QueryString("da")),
                             AgroKey_EncoderDecoder,
                             Server)

            Piva = Stringa_Decodifica(CStr(Request.QueryString("p")),
                                        AgroKey_EncoderDecoder,
                                        Server)

            Sa_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("s")),
                                      AgroKey_EncoderDecoder,
                                      Server))

            'Descr_Articolo = Stringa_Decodifica(CStr(Request.QueryString("spe")),
            '                                    AgroKey_EncoderDecoder,
            '                                    Server)

            RagSoc_Impresa = Stringa_Decodifica(CStr(Request.QueryString("rs")),
                                               AgroKey_EncoderDecoder,
                                               Server)


            If RagSoc_Impresa = "" Then
                RagSoc_Impresa = objImpreseR.RagSoc_from_Piva(Piva, objParametri_Server)
            End If

            Veg_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("spe")),
                                  AgroKey_EncoderDecoder,
                                  Server))

            Cul_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("var")),
                                  AgroKey_EncoderDecoder,
                                  Server))

            Mat_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("m")),
                                  AgroKey_EncoderDecoder,
                                  Server))

            Cod_Articolo = Stringa_Decodifica(CStr(Request.QueryString("ca")),
                                      AgroKey_EncoderDecoder,
                                      Server)

            If Cod_Articolo = "0" Then
                Cod_Articolo = ""
                'il codice non è stato passato, 
                'perchè si vogliono cercare tutte le specie
                'o il range di specie selezionato
            End If

            FiltroAggArticoli = Session("Str_Codici_Prodotto")


            Codice_Conferente = Stringa_Decodifica(CStr(Request.QueryString("cc")),
                                    AgroKey_EncoderDecoder,
                                    Server)

            If Codice_Conferente = "0" Then
                Codice_Conferente = ""
                'il codice non è stato passato, 
                'perchè si vogliono cercare tutti i conferenti
                'o il range di conferenti selezionato
            Else
                Codice_Conferente = CStr(Codice_Conferente)
            End If

            FiltroAggConferenti = Session("Str_CodRisUm_Conferenti")

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
            Riga.Cells(0).InnerHtml = "Dati da querystring: " & ex.Message
            Me.TableExcel.Rows.Add(Riga)
        End Try


        '##############################################################
        '####################  LETTURA DATI ###########################
        '##############################################################

        Dim DT As DataTable = Nothing
        Dim i As Integer

        Try

            Dim ADD As New AgronicaCoreStampeDAL.FreshAndFood
            Dim objConfFun As New AgronicaCoreStampeDAL.Conferimento_Funzioni
            Dim objADDFun As New AgronicaCoreStampeDAL.AccettazioneDaDiversi_Funzioni

            FiltroAggArticoli = objConfFun.Ricava_FiltroMatCod_da_StrCodiciProdotto(FiltroAggArticoli)
            FiltroAggConferenti = objConfFun.Ricava_FiltroCodRisUm_da_StrCodiciContatto(FiltroAggConferenti)

            DT = ADD.Trasportatori_XLS(Piva,
                                     Sa_Cod,
                                     Veg_Cod,
                                     Cul_Cod,
                                     Mat_Cod,
                                     Cod_Articolo,
                                     Codice_Esterno,
                                     Codice_Conferente,
                                     Data_Da,
                                     Data_A,
                                     FiltroAggArticoli,
                                     FiltroAggConferenti,
                                     "",
                                     "",
                                     objParametri_Server)

            If Not IsNothing(DT) Then

                For i = 0 To DT.Rows.Count - 1

                    With DT.Rows(i)

                        .Item("Numero_Conf") = Ricava_NumeroDocumento_Senza_Sequenza(.Item("Doc_Numero_Sin_Conf"),
                                                                                    .Item("Doc_Numero_Conf"),
                                                                                    .Item("Doc_Numero_Des_Conf"))

                        '' 'netto trasportato = peso lordo
                        ''.Item("Netto_Trasportato") = Calcola_PesoLordo(CInt(.Item("Tipo_Peso")), CDbl(.Item("Peso")), CDbl(.Item("Tara_Imballi")))
                        ''netto trasportato = peso netto
                        '.Item("Netto_Trasportato") = Calcola_PesoNetto(CInt(.Item("Tipo_Peso")), CDbl(.Item("Peso")), CDbl(.Item("Tara_Imballi")))

                        .Item("Netto_Trasportato") = Agro_Math.ArrotondaVal_0(.Item("Netto_Trasportato"))

                        If InStr(CStr(.Item("Targhe")), "_") > 0 Then
                            .Item("Targa_Automezzo") = CStr(.Item("Targhe")).Split("_")(0)
                            .Item("Targa_Rimorchio") = CStr(.Item("Targhe")).Split("_")(1)
                        Else
                            .Item("Targa_Automezzo") = ""

                            .Item("Targa_Rimorchio") = ""
                        End If

                        objADDFun.Calcola_NettoPagamento(.Item("Netto_Trasportato"),
                                                         .Item("Degrado_Perc"),
                                                         .Item("Udm_Cod"),
                                                         .Item("Degrado"),
                                                         .Item("Netto_Pagamento"))


                        'If IsDBNull(.Item("Listino")) Then
                        '    .Item("Costo_Totale") = .Item("Netto_Pagamento") * 0

                        'Else
                        '    .Item("Costo_Totale") = .Item("Netto_Pagamento") * .Item("Listino")
                        'End If


                    End With

                Next


                '#########################################################################################
                '####################  Start Calcolo Costo_Totale Conferimento ###########################
                '#########################################################################################
                'Aggiungo nuova colonna per pulire il datable alla fine
                Dim newColumn As New DataColumn("DaEliminare", GetType(Boolean))
                newColumn.DefaultValue = True
                DT.Columns.Add(newColumn)

                'Raggruppo le raccolte
                Dim DTConferimenti = DT.AsEnumerable() _
                    .Select(Function(row) row.Field(Of Integer)("riga_Conferimento")) _
                    .Distinct()

                Dim DTImpianti
                Dim costo_totale As Decimal
                Dim CodiceZona As String
                Dim listino As Decimal
                Dim listinoUdm As Integer
                Dim Qta As Decimal
                Dim ListCodiceZona As New List(Of String)()

                For Each item As Integer In DTConferimenti

                    costo_totale = 0
                    CodiceZona = ""
                    DTImpianti = Nothing
                    ListCodiceZona = New List(Of String)()

                    'recupero tutti gli impianti della raccolta
                    DTImpianti = DT.AsEnumerable() _
                    .Where(Function(row) row.Field(Of Integer)("riga_Conferimento") = item _
                    AndAlso Not IsDBNull(row("Piva")) _
                    AndAlso Not IsDBNull(row("Sa_Cod")) _
                    AndAlso Not IsDBNull(row("Appezza")) _
                    AndAlso Not IsDBNull(row("Id_Destinazione"))) _
                    .Select(Function(row) row)

                    Dim listinoPrec As Decimal? = Nothing
                    Dim listinoUdmPrec As Integer? = Nothing

                    Dim invalidaCostoTotale As Boolean = False

                    'calcolo costo_totale di tutti gli impianti della stessa raccolta e concateno il codiceZona
                    For Each impianto In DTImpianti

                        listino = 0
                        Qta = 0
                        listinoUdm = 0

                        If Not IsDBNull(impianto("Listino")) AndAlso Not IsDBNull(impianto("Qta")) AndAlso Not IsDBNull(impianto("Listino_Udm")) Then

                            listino = impianto("Listino")
                            listinoUdm = impianto("Listino_Udm")

                            Select Case listinoUdm
                                Case enum_UnitaMisura.Numero
                                    Qta = 1
                                Case enum_UnitaMisura.KG
                                    Qta = impianto("Qta")
                                Case Else
                                    Qta = 0
                            End Select

                        End If

                        If listinoPrec.HasValue Then

                            If listinoUdm = listinoUdmPrec OrElse listinoUdm = 0 OrElse listinoPrec = 0 Then

                                If listinoUdm = enum_UnitaMisura.KG Then
                                    costo_totale += listino * Qta
                                End If
                                
                            Else
                                invalidaCostoTotale = true
                            End If

                        Else
                            'Primo giro
                            costo_totale += listino * Qta
                        End If

                        listinoPrec = listino
                        listinoUdmPrec = listinoUdm

                        If Not String.IsNullOrEmpty(impianto("Codice_Zona").ToString()) Then
                            ' Controllo se codice zona gia esiste
                            Dim itemExists As Boolean = ListCodiceZona.Contains(impianto("Codice_Zona").ToString())
                            If Not itemExists Then
                                ' Aggiungo nuovo codice zona nella lista
                                ListCodiceZona.Add(impianto("Codice_Zona").ToString())

                                If CodiceZona = "" Then
                                    CodiceZona = impianto("Codice_Zona").ToString()
                                Else
                                    CodiceZona = CodiceZona & " | " & impianto("Codice_Zona").ToString()
                                End If
                            End If
                        End If

                    Next

                    'Aggiorno il primo impianto della raccolta
                    Dim rowsToUpdate = (DT.AsEnumerable() _
                            .Where(Function(row) row.Field(Of Integer)("riga_Conferimento") = item)).FirstOrDefault()

                    rowsToUpdate("Costo_Totale") = if(invalidaCostoTotale, "N.D.", Agro_Math.ArrotondaVal_2(costo_totale))
                    rowsToUpdate("Codice_Zona") = CodiceZona
                    'questa riga non sara eliminata dal DT alla fine della procedura
                    rowsToUpdate("DaEliminare") = False

                Next

                ' elimino tutti le righe che non servono da  visualizzare nel excel
                Dim rowsToDelete = DT.AsEnumerable() _
                                .Where(Function(row) row.Field(Of Boolean)("DaEliminare") <> False) _
                                .ToList()

                For Each row As DataRow In rowsToDelete
                    DT.Rows.Remove(row)
                Next
                '#########################################################################################
                '####################  End Calcolo Costo_Totale Conferimento ###########################
                '#########################################################################################




                'DT.AsEnumerable().ForEach(Sub(obj) obj.Field < Decimal > ("id") = 0.5)

                'DT.Columns.Remove("Doc_Numero_Sin")
                'DT.Columns.Remove("Doc_Numero")
                'DT.Columns.Remove("Doc_Numero_Des")
                DT.Columns.Remove("Doc_Numero_Sin_Conf")
                DT.Columns.Remove("Doc_Numero_Conf")
                DT.Columns.Remove("Doc_Numero_Des_Conf")
                'DT.Columns.Remove("Peso")
                'DT.Columns.Remove("Tara_Imballi")
                'DT.Columns.Remove("Tipo_Peso")
                DT.Columns.Remove("Targhe")
                DT.Columns.Remove("ordine_det")
                DT.Columns.Remove("Degrado_Perc")
                DT.Columns.Remove("Udm_Cod")
                DT.Columns.Remove("Degrado")
                DT.Columns.Remove("Listino")
                DT.Columns.Remove("Listino_Udm")
                DT.Columns.Remove("Qta")
                DT.Columns.Remove("riga_Conferimento")
                DT.Columns.Remove("Appezza")
                DT.Columns.Remove("Piva")
                DT.Columns.Remove("Sa_Cod")
                DT.Columns.Remove("Id_Destinazione")
                DT.Columns.Remove("DaEliminare")


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

        Crea_EXCEL(DT)


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
                        Case CStr("Piva_Prima_Cooperativa").ToLower
                            Riga.Cells(k).InnerHtml = "P.IVA <br/>Cooperativa"
                        Case CStr("RagSoc_Prima_Cooperativa").ToLower
                            Riga.Cells(k).InnerHtml = "Cooperativa"
                        Case CStr("Piva_Sec_Cooperativa").ToLower
                            Riga.Cells(k).InnerHtml = "P.IVA Seconda<br/>Cooperativa"
                        Case CStr("RagSoc_Sec_Cooperativa").ToLower
                            Riga.Cells(k).InnerHtml = "Seconda Cooperativa"
                        Case CStr("Piva_Produttore").ToLower
                            Riga.Cells(k).InnerHtml = "P.IVA<br/>Produttore"
                        Case CStr("RagSoc_Produttore").ToLower
                            Riga.Cells(k).InnerHtml = "Produttore"
                        Case CStr("Netto_Trasportato").ToLower
                            Riga.Cells(k).InnerHtml = "Netto<br/>Trasportato"
                        Case CStr("Netto_Pagamento").ToLower
                            Riga.Cells(k).InnerHtml = "Netto a<br/>Pagamento"
                        Case CStr("Costo_Totale").ToLower
                            Riga.Cells(k).InnerHtml = "Costo<br/>Totale"
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

                            Case CStr("Piva_Produttore").ToLower, "Piva_Prima_Cooperativa".ToLower, "Piva_Sec_Cooperativa".ToLower
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
            Riga.Cells(0).InnerHtml = "Creazione excel: " & ex.Message
            Me.TableExcel.Rows.Add(Riga)
        End Try

    End Sub


End Class


