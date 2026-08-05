Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza

Public Class ConsistenzeEnologiche
    Inherits System.Web.UI.Page

    Private rptConsistenze As Rpt_ConsistenzeEnologiche
    Private SottoRpt_Riepilogo As Sottorpt_ConsEno_Riepilogo
    Private DsConsistenze As DS_ConsistenzeEnologiche

    '----- Gestione Querystring
    Dim Qs_Piva As String
    Dim Qs_Sa_Cod As String
    Dim QS_Data As String
    Dim QS_Piano_Cod As String
    Dim QS_Vas_Cod As String
    Dim QS_Linea_Cod As String
    Dim QS_Categoria_Cod As String
    Dim QS_Semilavorato_Cod As String
    Dim QS_Ordinamento As enum_OrdinamentoStampaConsEnologiche
    Dim QS_Flag_QtaNoZero As Boolean
    Dim QS_Flag_VascheNoMov As Boolean
    Dim QS_Flag_StampaRiepilogo As Boolean
    Dim QS_1Contiene_2Noncontiene As Integer
    Dim QS_FiltroLotto As String

    Dim QS_Linea_Des, QS_Categoria_Des, QS_Semilavorato_Des As String

    'oggetto objparametri x server e utenti
    Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    Dim Log_Errori As String = ""

#Region " CONSISTENZE ENOLOGICHE "

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
        ' istanzio gli oggetti report
        rptConsistenze = New Rpt_ConsistenzeEnologiche
        SottoRpt_Riepilogo = New Sottorpt_ConsEno_Riepilogo
    End Sub

#End Region

    '#################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'Faccio scadere subito la pagina memorizzata nella cache
        Response.Expires = 0

        'inizializzazione oggetti objParametri_Utenti e objParametri_Server
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))


        '#################################################################################
        '#####  Recupero dati dalla QueryString 
        '#################################################################################

        Qs_Piva = Stringa_Decodifica(CStr(Request.QueryString("p")), _
                                    AgroKey_EncoderDecoder, _
                                    Server)

        QS_Data = Stringa_Decodifica(Request.QueryString("d").ToString, _
                                     AgroKey_EncoderDecoder, _
                                     Server)

        Qs_Sa_Cod = Stringa_Decodifica(Request.QueryString("s").ToString, _
                                       AgroKey_EncoderDecoder, _
                                       Server)

        QS_Piano_Cod = Stringa_Decodifica(Request.QueryString("pc").ToString, _
                                            AgroKey_EncoderDecoder, _
                                            Server)

        QS_Vas_Cod = Stringa_Decodifica(Request.QueryString("vc").ToString, _
                                      AgroKey_EncoderDecoder, _
                                      Server)

        QS_Linea_Cod = Stringa_Decodifica(Request.QueryString("lc").ToString, _
                              AgroKey_EncoderDecoder, _
                              Server)

        QS_Linea_Des = Stringa_Decodifica(Request.QueryString("ld").ToString, _
                      AgroKey_EncoderDecoder, _
                      Server)

        QS_Categoria_Cod = Stringa_Decodifica(Request.QueryString("cc").ToString, _
                           AgroKey_EncoderDecoder, _
                           Server)

        QS_Categoria_Des = Stringa_Decodifica(Request.QueryString("cd").ToString, _
                   AgroKey_EncoderDecoder, _
                   Server)

        QS_Semilavorato_Cod = Stringa_Decodifica(Request.QueryString("sc").ToString, _
                           AgroKey_EncoderDecoder, _
                           Server)


        QS_Semilavorato_Des = Stringa_Decodifica(Request.QueryString("sd").ToString, _
                      AgroKey_EncoderDecoder, _
                      Server)

        QS_Ordinamento = Stringa_Decodifica(Request.QueryString("ord").ToString, _
                                            AgroKey_EncoderDecoder, _
                                            Server)


        QS_Flag_QtaNoZero = Not CBool((Stringa_Decodifica(Request.QueryString("fqz").ToString, _
                                            AgroKey_EncoderDecoder, _
                                            Server)))

        QS_Flag_VascheNoMov = CBool((Stringa_Decodifica(Request.QueryString("fvm").ToString, _
                                           AgroKey_EncoderDecoder, _
                                           Server)))

        QS_Flag_StampaRiepilogo = CBool((Stringa_Decodifica(Request.QueryString("fsr").ToString, _
                                   AgroKey_EncoderDecoder, _
                                   Server)))

        QS_1Contiene_2Noncontiene = CInt((Stringa_Decodifica(Request.QueryString("cnc").ToString, _
                             AgroKey_EncoderDecoder, _
                             Server)))

        QS_FiltroLotto = CStr((Stringa_Decodifica(Request.QueryString("lot").ToString, _
                      AgroKey_EncoderDecoder, _
                      Server)))


        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim Nome_Documento As String = "ConsistenzeEnologiche"
        Dim IdentificazioneDocumento As String = ""
        Dim cat_cod As enum_CategorieDocumenti

        cat_cod = enum_CategorieDocumenti.Cantina_ConsistenzeEnologiche


        If Not Me.IsPostBack Then

            Try

                Dim DsConsistenze As New DS_ConsistenzeEnologiche

                'carico i dati nei datatable 
                Carica_DsConsistenze(DsConsistenze)

                '####################################################


            Catch exc As Exception
                Log_Errori += " " + vbCrLf + exc.Message + vbCrLf
            End Try



            '-----------------------------------------
            '---- Gestione Salvataggio PDF -----------  
            '-----------------------------------------
            Try

                Dim Data_Inizio_Allegati, Data_Fine_Allegati As Date

                Data_Inizio_Allegati = AGRODATAINIZIO
                Data_Fine_Allegati = QS_Data
                IdentificazioneDocumento += "_" + Format(Data_Inizio_Allegati, "yyyy_MM_dd") + "_" + Format(Data_Fine_Allegati, "yyyy_MM_dd")


                ' leggo la sottocartella da CategorieDocumenti
                Dim Sottocartella As String
                Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                Sottocartella = objCatDoc.Sottocartella(cat_cod, "", "", objParametri_Server)

                ' salvo il report in formato PDF
                Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                objGestFile.SalvaReportPdf(rptConsistenze, _
                                           cat_cod, _
                                           Sottocartella, _
                                           Nome_Documento + "_p" & Qs_Piva + "_" + IdentificazioneDocumento + ".pdf", _
                                           objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
                Dim AllegatiDocumentiCod As Integer

                AllegatiDocumentiCod = objAllegati.SalvaAllegato(Qs_Piva, _
                                                                 cat_cod, _
                                                                 Nome_Documento, _
                                                                 Nome_Documento + "_p" & Qs_Piva + "_" + IdentificazioneDocumento + ".pdf", _
                                                                 Sottocartella, _
                                                                 "", "", "", "", _
                                                                 Data_Inizio_Allegati, _
                                                                 Data_Fine_Allegati, _
                                                                 objParametri_Server)


            Catch ex As Exception
                Log_Errori += "- Gestione allegati: " + vbCrLf + ex.Message + vbCrLf
            End Try


            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            'Dim Path_Errore, Str_Errore_Path As String
            Dim Nome_File As String

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento + ", " + vbCrLf + _
                                "Piva = " + CStr(Qs_Piva) + ", " + vbCrLf + _
                                vbCrLf + vbCrLf + vbCrLf + vbCrLf + _
                                Log_Errori



                Nome_File = "Log_Errori_" + Nome_Documento + "_" + CStr(Session("ASG_Utente_Username")) & ".txt"

                'GestioneFile_CreaCartellaNelPathWebConfig("Path_LogErrori_StampeEsportazioni", "Stampe_Magazzino", Path_Errore, Str_Errore_Path)
                'If Str_Errore_Path = "" And Path_Errore <> "" Then
                '    GestioneFile_CreaScriviFileConRicercaNome(Log_Errori, Path_Errore, Nome_File, Str_Errore_Path, )
                'End If

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server, _
                                                 "Stampe_Cantine", _
                                                 Nome_File, _
                                                 Session("ASG_Utente_Username"), _
                                                 "ConsistenzeEnologiche.aspx", _
                                                 Log_Errori)


            End If
            '-----------------------------------------

        End If

        '==================================================================

        Try
            Session("Report") = rptConsistenze
            Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))

        Catch ex As Exception
            Log_Errori += "- Export: " + vbCrLf + ex.Message + vbCrLf
        End Try




    End Sub

    '###########################################################################
    Private Sub Carica_DsConsistenze(ByRef DsConsistenzeEnologiche As DS_ConsistenzeEnologiche)
        'ByRef rag_Soc As String, _
        'ByRef Sa_Nome As String)

        Dim i As Integer
        Dim objConsistenze As New AgronicaCoreStampeDAL.DocCantina
        Dim objMP As New AgronicaCoreAnagrafeDAL.Materie_PrimexLC_R
        Dim dt As DataTable
        Dim Dr As DS_ConsistenzeEnologiche.DS_ConsistenzeEnologicheRow
        Dim HT_Linee As Hashtable
        Dim HT_QtaRiepilogo As Hashtable
        Dim Linea_cod As Integer
        Dim Qta_Riepilogo As Double = 0
        Dim DsConsEcoRiep As New DS_ConsEno_Riepilogo
        Dim Anno_Prod As String
        Dim Str_AnnoProd As String
        Dim key_riepilogo As String

        Dim Parametro_Filtro As String = ""
        Dim Parametro_Rag_Soc As String = ""
        Dim Parametro_Sa_Nome As String = ""
        Dim Parametro_Data As String = CDate(QS_Data).ToShortDateString

        Try

            Dim FiltroAgg1 As String = ""
            Dim FiltroAgg2 As String = ""

            'creazione del filtro sul lotto
            'lo metto solo su FiltroAgg1 perchè nella parte di UNION che legge le vasche vuote non serve
            If Trim(QS_FiltroLotto) <> "" Then
                Select Case QS_1Contiene_2Noncontiene
                    Case 0
                    Case 1
                        FiltroAgg1 &= " ( Movimenti_Dettagli.lotto LIKE '%" & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveText(QS_FiltroLotto) & "%' ) "
                    Case 2
                        FiltroAgg1 &= " ( Movimenti_Dettagli.lotto NOT LIKE '%" & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveText(QS_FiltroLotto) & "%' ) "
                End Select
            End If


            '06/10/2017: nel caso di data futura, eseguita la stessa query
            'prima usava la vecchia query record statico (era stata lasciata come "trucchetto" per verificare che il record statico fosse salvato bene, con la giacenza = alla giacenza dinamica
            'su Ruggeri winematch ci sono operazioni future da considerare

            ''data futura
            'If CDate(QS_Data) > Date.Today Then
            '    '------------------------------------------------
            '    '----- query giacenza statica (x verifica)  ----
            '    '-----------------------------------------------
            '    dt = objConsistenze.ConsistenzeEnologiche_RecordStatico(Qs_Piva, _
            '                                                            Qs_Sa_Cod, _
            '                                                            QS_Piano_Cod, _
            '                                                            QS_Vas_Cod, _
            '                                                            QS_Linea_Cod, _
            '                                                            QS_Flag_QtaNoZero, _
            '                                                            QS_Flag_VascheNoMov, _
            '                                                            QS_Ordinamento, _
            '                                                            "", "", _
            '                                                            objParametri_Server)
            'Else
            'compreso oggi
            '----------------------------------
            '----- query giacenza dinamica  ----
            '----------------------------------
            dt = objConsistenze.ConsistenzeEnologiche(Qs_Piva, _
                                                    Qs_Sa_Cod, _
                                                    QS_Piano_Cod, _
                                                    QS_Vas_Cod, _
                                                    QS_Linea_Cod, _
                                                    QS_Categoria_Cod, _
                                                    QS_Semilavorato_Cod, _
                                                    QS_Data, _
                                                    QS_Flag_QtaNoZero, _
                                                    QS_Flag_VascheNoMov, _
                                                    QS_Ordinamento, _
                                                    FiltroAgg1, FiltroAgg2, _
                                                    objParametri_Server)

            'End If

        Catch ex As Exception
            Log_Errori += "- Consistenze_Enologiche_Leggi: " + vbCrLf + ex.Message + vbCrLf
        End Try

        Try

            If QS_Semilavorato_Des <> "" Then
                Parametro_Filtro += "Tipo Semilavorato: " & QS_Semilavorato_Des
            End If

            If QS_Categoria_Des <> "" Then
                Parametro_Filtro += "  Categoria: " & QS_Categoria_Des
            End If

            If QS_Linea_Des <> "" Then
                Parametro_Filtro += "  Linea: " & QS_Linea_Des
            End If

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then

                If QS_Flag_StampaRiepilogo = True Then
                    HT_Linee = New Hashtable
                    HT_QtaRiepilogo = New Hashtable
                End If

                Dim Giacenza As Decimal
                For i = 0 To dt.Rows.Count - 1

                    Giacenza = dt.Rows(i).Item("Qta")

                    'Se QS_Flag_QtaNoZero = true
                    'oltre al filtro nella query, devo fare anche il filtro su codice
                    'perchè molti decimal vengono salvati come valori infinitamente piccoli
                    'ad esempio 0.00003680000000017003
                    'Se QS_Flag_QtaNoZero = False deve stampare sempre
                    If QS_Flag_QtaNoZero = False Or (Giacenza <> 0 And Not (Giacenza < QTA_GiancenzeVisualizzate And Giacenza > -QTA_GiancenzeVisualizzate)) Then

                        '---------------------------------------------------------------------
                        Dr = DsConsistenzeEnologiche.DS_ConsistenzeEnologiche.NewDS_ConsistenzeEnologicheRow
                        '---------------------------------------------------------------------

                        With dt.Rows(i)

                            If i = 0 Then
                                Parametro_Rag_Soc = .Item("rag_soc")
                                Parametro_Sa_Nome = .Item("sa_nome")
                            End If

                            Dr.Vas_Cod = .Item("Vas_Cod")
                            Dr.Piano_Cod = .Item("Piano_Cod")
                            Dr.Piano_Des = .Item("Piano_Des")
                            Dr.Identificativo = .Item("Identificativo")
                            Dr.Numero_Serie = .Item("Numero_Serie")
                            Dr.Materiale = .Item("Materiale")
                            Dr.Udm_Capacita = .Item("Udm_Capacita")
                            Dr.Capacita_Nominale = .Item("Capacita_Nominale")
                            Dr.Capacita_Effettiva = .Item("Capacita_Effettiva")
                            Dr.Modello = .Item("Modello")
                            Dr.Udm_Altezza = .Item("Udm_Altezza")
                            Dr.Altezza_Cilindro = .Item("Altezza_Cilindro")
                            Dr.Udm_Sim = .Item("Udm_Sim")
                            Dr.Qta = .Item("Qta")
                            '25/05/2015:
                            'il crystal faceva quel che gli pareva, arrotondava il valore
                            '12.016,25 stampava 12.016,30 (anche se nelle proprietà era impostato bene)
                            Dr.Qta_Str = Format(.Item("Qta"), "#,###,##0.00")
                            Dr.Mat_Des = .Item("Mat_Des")
                            Dr.Lotto = .Item("Lotto")
                            'If InStr(.Item("LineaProduttiva"), "§") > 0 Then
                            '    Dr.LineaProduttiva = CStr(.Item("LineaProduttiva")).Split("§")(0)
                            'Else
                            '    Dr.LineaProduttiva = .Item("LineaProduttiva")
                            'End If
                            Dr.LineaProduttiva = Trim(.Item("LineaProduttiva"))

                            Try

                                If QS_Flag_StampaRiepilogo = True Then

                                    'If InStr(.Item("LineaProduttiva"), "§") > 0 Then
                                    '    Linea_Cod = CStr(.Item("LineaProduttiva")).Split("§")(2)
                                    'End If
                                    Linea_cod = .Item("Linea_Cod")

                                    '16/08/2017: ricavo anno di produzione da lotto
                                    'default '', se non c'è annata
                                    Anno_Prod = objMP.AnnoProd_from_Lotto(.Item("Elem_Cod"), .Item("Mat_cod"), Dr.Lotto, objParametri_Server)
                                    If Anno_Prod <> "" Then
                                        Str_AnnoProd = " - " & CStr(Anno_Prod)
                                    Else
                                        Anno_Prod = 0
                                        Str_AnnoProd = ""
                                    End If

                                    key_riepilogo = CStr(Linea_cod) & "|" & CStr(Anno_Prod)

                                    If Linea_cod <> 0 Then

                                        If Not HT_Linee.ContainsKey(key_riepilogo) Then
                                            HT_Linee.Add(key_riepilogo, Dr.LineaProduttiva & Str_AnnoProd)
                                        End If

                                        If Not HT_QtaRiepilogo.ContainsKey(key_riepilogo) Then
                                            HT_QtaRiepilogo.Add(key_riepilogo, Dr.Qta)
                                        Else
                                            Qta_Riepilogo = HT_QtaRiepilogo(key_riepilogo)
                                            Qta_Riepilogo += Format(Dr.Qta, "#,###,##0.00")
                                            HT_QtaRiepilogo(key_riepilogo) = Qta_Riepilogo
                                        End If

                                        'If Not HT_Linee.ContainsKey(Linea_Cod) Then
                                        '    HT_Linee.Add(Linea_Cod, Dr.LineaProduttiva)
                                        'End If

                                        'If Not HT_QtaRiepilogo.ContainsKey(Linea_Cod) Then
                                        '    HT_QtaRiepilogo.Add(Linea_Cod, Dr.Qta)
                                        'Else
                                        '    Qta_Riepilogo = HT_QtaRiepilogo(Linea_Cod)
                                        '    Qta_Riepilogo += Format(Dr.Qta, "#,###,##0.00")
                                        '    HT_QtaRiepilogo(Linea_Cod) = Qta_Riepilogo
                                        'End If

                                    End If

                                End If 'QS_Flag_StampaRiepilogo

                            Catch ex As Exception
                                Log_Errori += "- Analisi linee e annata: " + vbCrLf + ex.Message + vbCrLf
                            End Try


                        End With

                        '---------------------------------------------------------------------
                        DsConsistenzeEnologiche.DS_ConsistenzeEnologiche.Rows.Add(Dr)
                        '---------------------------------------------------------------------
                    Else
                        Dim debug As Boolean = True
                    End If 'verifica giacenza <>0

                Next

            DsConsistenzeEnologiche.DS_ConsistenzeEnologiche.AcceptChanges()

            Try

                If QS_Flag_StampaRiepilogo = True Then

                    Dim DTLinee As New DataTable
                    DTLinee.Columns.Add(New DataColumn("linea_cod", GetType(String)))
                    DTLinee.Columns.Add(New DataColumn("linea_des", GetType(String)))
                    DTLinee.Columns.Add(New DataColumn("Qta_float", GetType(Double)))
                    DTLinee.Columns.Add(New DataColumn("Qta_str", GetType(String)))
                    'DTLinee.Columns.Add(New DataColumn("Qta_Tot_float", GetType(Double)))
                    'DTLinee.Columns.Add(New DataColumn("Qta_Tot_str", GetType(String)))

                    Dim Dr_Riep As DS_ConsEno_Riepilogo.DT_RiepilogoRow
                    Dim key As Object
                    Dim Qta_Totale As Double = 0
                    Dim DtRow As DataRow

                    If Not IsNothing(HT_Linee) Then

                        For Each key In HT_Linee.Keys

                            DtRow = DTLinee.NewRow()

                            DtRow("linea_cod") = key
                            DtRow("linea_des") = Trim(HT_Linee(key))

                            Qta_Riepilogo = HT_QtaRiepilogo(key)

                            DtRow("Qta_float") = Format(Qta_Riepilogo, "#,###,##0.00")
                            DtRow("Qta_str") = Format(Qta_Riepilogo, "#,###,##0.00")

                            Qta_Totale += Format(Qta_Riepilogo, "#,###,##0.00")

                            DTLinee.Rows.Add(DtRow)

                        Next

                    End If

                    'passo dal dv per ordinare per nome linea
                    Dim DvLinee As New DataView
                    DTLinee.TableName = "Linee"
                    DvLinee.Table = DTLinee
                    DvLinee.Sort = "Linea_Des"

                    If Not IsNothing(DvLinee) Then
                        For i = 0 To DvLinee.Count - 1

                            '---------------------------------------------------------------------
                            Dr_Riep = DsConsEcoRiep.DT_Riepilogo.NewDT_RiepilogoRow
                            '---------------------------------------------------------------------

                            Dr_Riep.Linea_Cod = DvLinee(i).Item("Linea_Cod")
                            Dr_Riep.LineaProduttiva = DvLinee(i).Item("linea_des")

                            Dr_Riep.Qta_float = DvLinee(i).Item("Qta_float")
                            Dr_Riep.Qta_Str = DvLinee(i).Item("Qta_str")

                            Dr_Riep.Qta_Totale_float = Format(Qta_Totale, "#,###,##0.00") 'DvLinee(i).Item("Qta_Tot_float")
                            Dr_Riep.Qta_Totale_str = Format(Qta_Totale, "#,###,##0.00") 'DvLinee(i).Item("Qta_Tot_str")

                            '---------------------------------------------------------------------
                            DsConsEcoRiep.DT_Riepilogo.Rows.Add(Dr_Riep)
                            '---------------------------------------------------------------------
                        Next

                    End If

                End If 'QS_Flag_StampaRiepilogo

            Catch ex As Exception
                Log_Errori += "- Riepilogo linee produttive: " + vbCrLf + ex.Message + vbCrLf
            End Try

            End If ' elenco consistenze

            Try

                rptConsistenze.SetDataSource(DsConsistenzeEnologiche)

            Catch ex As Exception
                Log_Errori += "- Aggancio dataset report: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
            End Try

            Try

                SottoRpt_Riepilogo.SetDataSource(DsConsEcoRiep)

            Catch ex As Exception
                Log_Errori += "- Aggancio dataset sottoreport: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
            End Try

            Try
                rptConsistenze.OpenSubreport("Sottorpt_ConsEno_Riepilogo.rpt").SetDataSource(DsConsEcoRiep)
            Catch ex As Exception
                Log_Errori += "- OpenSubreport credito: " + vbCrLf + ex.Message + vbCrLf
            End Try


            If QS_Flag_StampaRiepilogo = False Then

                rptConsistenze.Section4.SectionFormat.EnableSuppress = True

            End If

        Catch ex As Exception
            Log_Errori += "- Elaborazione dati: " + vbCrLf + ex.Message + vbCrLf
        End Try

        Try
            rptConsistenze.SetParameterValue("Filtro", Parametro_Filtro)
            rptConsistenze.SetParameterValue("Rag_Soc", Parametro_Rag_Soc)
            rptConsistenze.SetParameterValue("Sa_Nome", Parametro_Sa_Nome)
            rptConsistenze.SetParameterValue("Data", Parametro_Data)

        Catch ex As Exception
            Log_Errori += "- impostazione parametri: " + vbCrLf + ex.Message + vbCrLf
        End Try

    End Sub



End Class
