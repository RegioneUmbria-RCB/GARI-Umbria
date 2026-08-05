Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Agro_Math
Imports AgronicaCoreDataProvider.Sicurezza

Public Class EC_Bolle_Conf
    Inherits System.Web.UI.Page

    '----------------------------
    '   DICHIARAZIONE REPORT
    '----------------------------
    Private Rpt_EC_Bolle As Rpt_EC_Bolle_Conf

    Dim Data_Da, Data_A As String
    Dim Codice_Conferente As String
    Dim Piva As String
    Dim Mat_Cod, Veg_Cod, Cul_Cod As Integer
    Dim Sa_Cod, Cod_Risum As Integer
    Dim Descr_Centro As String
    Dim FiltroAggArticoli, FiltroAggConferenti As String
    Dim Flag01_GestioneCodEsterno As Integer

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri


    Private Sub EC_Bolle_Conf_Init(sender As Object, e As EventArgs) Handles Me.Init
        '----------------------------
        '   INIZIALIZZAZIONE REPORT
        '----------------------------
        Rpt_EC_Bolle = New Rpt_EC_Bolle_Conf
    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        '##############################################################
        '############  Lettura Parametri Query String #################
        '##############################################################

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

        Descr_Centro = Stringa_Decodifica(CStr(Request.QueryString("sn")),
                                                AgroKey_EncoderDecoder,
                                                Server)

        Veg_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("spe")),
                                  AgroKey_EncoderDecoder,
                                  Server))

        Cul_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("var")),
                                  AgroKey_EncoderDecoder,
                                  Server))

        Mat_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("m")),
                                  AgroKey_EncoderDecoder,
                                  Server))


        FiltroAggArticoli = Session("Str_Codici_Prodotto")

        Cod_RisUm = CInt(Stringa_Decodifica(CStr(Request.QueryString("ru")),
                                  AgroKey_EncoderDecoder,
                                  Server))

        FiltroAggConferenti = Session("Str_CodRisUm_Conferenti")

        Flag01_GestioneCodEsterno = Stringa_Decodifica(CStr(Request.QueryString("fce")),
                                AgroKey_EncoderDecoder,
                                Server)

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))


        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim Nome_Documento As String = "EstrattoConto_BolleConferimento"
        Dim Log_Errori As String = ""

        If Not Me.IsPostBack Then

            Dim DS As New DS_EC_Bolle_Conf

            '-----------------------------------------
            '-------- funzione del report -------------
            '-----------------------------------------
            Try

                Stampa_EstrattoConto_Bolle(DS, Log_Errori)

            Catch exc As Exception
                Log_Errori &= "- Stampa_EstrattoConto_Bolle: " & vbCrLf & exc.Message & vbCrLf
            End Try

            '-----------------------------------------
            '----------- SetDataSource ---------------
            '-----------------------------------------
            Try

                Rpt_EC_Bolle.SetDataSource(DS)

            Catch ex As Exception
                Log_Errori &= "- Aggancio dataset: " & vbCrLf & ex.Message & vbCrLf & vbCrLf
            End Try

            '-----------------------------------------
            '----------- Parametri ---------------
            '-----------------------------------------
            Try
                Dim handleImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
                Dim ragSocimpresa As String = handleImprese.RagSoc_from_Piva(Piva, objParametri_Server)

                Rpt_EC_Bolle.SetParameterValue(Rpt_EC_Bolle.Parameter_ImpresaRagSoc.ParameterFieldName, ragSocimpresa.ToUpper)
                Rpt_EC_Bolle.SetParameterValue(Rpt_EC_Bolle.Parameter_ImpresaCentroAz.ParameterFieldName, Descr_Centro)
                Rpt_EC_Bolle.SetParameterValue(Rpt_EC_Bolle.Parameter_FiltroDataInizio.ParameterFieldName, Data_Da)
                Rpt_EC_Bolle.SetParameterValue(Rpt_EC_Bolle.Parameter_FiltroDataFine.ParameterFieldName, Data_A)

            Catch ex As Exception
                Log_Errori &= "- impostazione parametri: " & vbCrLf & ex.Message & vbCrLf
            End Try


            '-----------------------------------------
            '-------- Generazione Report -------------
            '-----------------------------------------

            'MS Eliminato passaggio report in session per giro su file: Session("Report") = rptRegCorrispettivi
            Dim reportTemporaneo As String = CrystalHelper.getFileReportTemporaneo()

            Try
                Rpt_EC_Bolle.SaveAs(reportTemporaneo, True)
            Catch ex As Exception
                Log_Errori &= "- Salvataggio report temporaneo: " & vbCrLf & ex.Message & vbCrLf
            End Try

            GC.Collect()


            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            Dim Nome_File As String

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento & ", " & vbCrLf &
                                "Data DA = " & CStr(Data_Da) & ", " & vbCrLf &
                                "Data A = " & CStr(Data_A) & ", " & vbCrLf &
                                "Codice Conferente = " & CStr(Codice_Conferente) & ", " & vbCrLf &
                                "Mat_cod = " & CStr(Mat_Cod) & ", " & vbCrLf &
                                "veg_cod = " & CStr(Veg_Cod) & ", " & vbCrLf &
                                "cul_cod = " & CStr(Cul_Cod) & ", " & vbCrLf &
                                "Stringa Filtro cod_Articolo = " & CStr(FiltroAggArticoli) & ", " & vbCrLf &
                                "Stringa Filtro Conferenti = " & CStr(FiltroAggConferenti) & ", " & vbCrLf &
                                "Piva = " & CStr(Piva) & ", " & vbCrLf &
                                "Sa_Cod = " & CStr(Sa_Cod) & ", " & vbCrLf &
                                                 vbCrLf & vbCrLf & vbCrLf & vbCrLf &
                                Log_Errori

                Nome_File = "Log_Errori_" & Nome_Documento

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server,
                                                 "Stampe_Conferimento",
                                                 Nome_File & ".txt",
                                                 Session("ASG_Utente_Username"),
                                                 "EC_Bolle_Conferimento",
                                                 Log_Errori)

            End If

            '------------------------------------------------
            '---- Redirect su VisualizzatoreReport -------------
            '------------------------------------------------
            Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" & AgronicaCoreDataProvider.Sicurezza.Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) &
                    "&tmpReportPath=" & AgronicaCoreDataProvider.Sicurezza.Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server))

        End If

        '==================================================================


    End Sub

    '#####################################################################################################
    Private Sub Stampa_EstrattoConto_Bolle(ByRef DS As DS_EC_Bolle_Conf,
                                           ByRef Log_Errori As String)

        Dim objConfFun As New AgronicaCoreStampeDAL.Conferimento_Funzioni
        Dim objADDFun As New AgronicaCoreStampeDAL.AccettazioneDaDiversi_Funzioni

        Dim DT As DataTable = Nothing
        Dim i As Integer

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        Try

            Dim objConf As New AgronicaCoreStampeDAL.FreshAndFood

            FiltroAggArticoli = objConfFun.Ricava_FiltroMatCod_da_StrCodiciProdotto(FiltroAggArticoli)
            FiltroAggConferenti = objConfFun.Ricava_FiltroCodRisUm_da_StrCodiciContatto(FiltroAggConferenti)

            DT = objConf.EstrattoConto_BolleConferimento(Flag01_GestioneCodEsterno,
                                                         Piva,
                                                         Sa_Cod,
                                                         Data_Da,
                                                         Data_A,
                                                         Cod_Risum,
                                                         Veg_Cod,
                                                         Cul_Cod,
                                                         Mat_Cod,
                                                         FiltroAggArticoli,
                                                         FiltroAggConferenti,
                                                         "",
                                                         objParametri_Server)

        Catch ex As Exception
            Log_Errori &= "- Lettura dei dati: " & vbCrLf & ex.Message & vbCrLf & vbCrLf
        End Try


        If Not IsNothing(DT) AndAlso DT.Rows.Count <> 0 Then

            Dim Descr_Specie As String = ""
            Dim Descr_Articolo As String = ""
            Dim Numero_DDT As String
            Dim Tara_Imballi As Double
            Dim Peso_Lordo, Peso_Netto As Double
            Dim Degrado_Perc, Degrado As Double
            Dim Netto_Pag As Double
            Dim Udm_Cod As Integer

            Dim DR As DS_EC_Bolle_Conf.DT_EC_Bolle_ConfRow

            For i = 0 To DT.Rows.Count - 1

                DR = DS.DT_EC_Bolle_Conf.NewDT_EC_Bolle_ConfRow

                If Flag01_GestioneCodEsterno = 1 AndAlso CInt(Session("ASG_ProgressivoGIAS")) = enum_CodiceGIAS_Clienti.Fruttagel Then

                    Codice_Conferente = DT.Rows(i).Item("Codice_Conferente")
                    DR.RagSoc_Conferente = Codice_Conferente & " - " & DT.Rows(i).Item("RagSoc_Conferente")

                    DR.Codice_Specie = DT.Rows(i).Item("Codice_Esterno")

                    objConfFun.Ricava_Specie_Varieta_FRG(DT.Rows(i).Item("Mat_Des"), Descr_Specie, Descr_Articolo)

                    DR.Descr_Specie = DT.Rows(i).Item("Codice_Esterno") & " - " & Descr_Specie
                    DR.Descr_Articolo = DT.Rows(i).Item("Cod_Articolo") & " - " & If(Descr_Articolo = "", Descr_Specie, Descr_Articolo)
                Else

                    Codice_Conferente = DT.Rows(i).Item("Cod_RisUm_Conferente")
                    DR.RagSoc_Conferente = DT.Rows(i).Item("RagSoc_Conferente")

                    DR.Codice_Specie = DT.Rows(i).Item("Veg_Cod")

                    DR.Descr_Specie = DT.Rows(i).Item("Veg_Des")
                    DR.Descr_Articolo = DT.Rows(i).Item("Mat_Des")
                End If

                DR.Codice_Conferente = Codice_Conferente

                DR.Codice_Articolo = DT.Rows(i).Item("Cod_Articolo")

                Numero_DDT = Ricava_NumeroDocumento_Senza_Sequenza(CStr(DT.Rows(i).Item("Doc_Numero_Sin_Conf")), CDbl(DT.Rows(i).Item("Doc_Numero_Conf")), CStr(DT.Rows(i).Item("Doc_Numero_Des_Conf")))

                '=====================================================================
                'PESI - CONF NUOVO:
                'bisogna leggere solo quelli su movimenti_dettagli
                'peso netto = qta_extra_totale
                'tara imballi = tara
                'il resto da calcolare

                Peso_Netto = DT.Rows(i).Item("qta_extra_totale_Raccolta")
                Peso_Netto = ArrotondaVal_0(Peso_Netto)

                Tara_Imballi = objConfFun.Calcola_TaraImballi(DT.Rows(i).Item("tara_totale_imb_entrata"),
                                                              DT.Rows(i).Item("Tara_Dettaglio"),
                                                              DT.Rows(i).Item("Num_Righe"))

                Peso_Lordo = Peso_Netto + Tara_Imballi
                Peso_Lordo = ArrotondaVal_0(Peso_Lordo)

                Degrado_Perc = DT.Rows(i).Item("Variazione_Raccolta")
                Udm_Cod = DT.Rows(i).Item("Udm_Cod_Raccolta")

                objADDFun.Calcola_NettoPagamento(Peso_Netto,
                                                 Degrado_Perc,
                                                 Udm_Cod,
                                                 Degrado,
                                                 Netto_Pag)

                '================================

                DR.Data_Bolla = DT.Rows(i).Item("Data_Bolla")
                DR.Numero_Bolla = DT.Rows(i).Item("Numero_Bolla")
                DR.Data_DDT = DT.Rows(i).Item("Data_Conf")
                DR.Numero_DDT = Numero_DDT
                DR.Peso_Lordo = Peso_Lordo
                DR.Tara_Imballi = Tara_Imballi
                DR.Peso_Netto = Peso_Netto
                DR.Degrado_Perc = Degrado_Perc
                DR.Degrado = Degrado
                DR.Netto_Pag = Netto_Pag
                DR.Indice_Qualitativo = DT.Rows(i).Item("Calibro_Des")
                DR.Punteggio = DT.Rows(i).Item("Punteggio")

                DS.DT_EC_Bolle_Conf.Rows.Add(DR)
                '---------------------------

            Next

        End If

    End Sub

End Class