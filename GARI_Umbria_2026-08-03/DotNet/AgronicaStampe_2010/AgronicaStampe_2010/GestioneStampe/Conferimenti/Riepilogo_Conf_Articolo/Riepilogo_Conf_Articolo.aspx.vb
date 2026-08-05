Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.Agro_Math

Public Class Riepilogo_Conf_Articolo
    Inherits System.Web.UI.Page

    Private Rpt_RiepilogoConferimentoXArticolo As Rpt_Riepilogo_Conf_Articolo
    Dim Data_Da, Data_A As String
    Dim Codice_Conferente As String
    Dim Cod_RisUm As Integer
    Dim Cod_Articolo, Codice_Esterno As String
    Dim FiltroAggConferenti As String
    Dim FiltroAggArticoli As String
    Dim Piva As String
    Dim Sa_Cod, Mat_Cod, Veg_Cod, Cul_Cod As Integer
    'Dim RagSoc_Impresa As String
    'Dim Descr_Specie As String
    Dim Descr_Centro, Mat_Des As String
    Dim Flag01_GestioneCodEsterno As Integer

    Dim Param_Date As String = ""
    Dim Param_Rag_Soc As String = ""
    Dim Param_Sa_Nome As String = ""

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri

    Private Sub Riepilogo_Conf_Articolo_Init(sender As Object, e As EventArgs) Handles Me.Init
        '----------------------------
        '   INIZIALIZZAZIONE REPORT
        '----------------------------
        Rpt_RiepilogoConferimentoXArticolo = New Rpt_Riepilogo_Conf_Articolo
    End Sub


    '#####################################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load


        '##############################################################
        '############  Lettura Parametri Query String #################
        '##############################################################

        Data_Da = CDate(Stringa_Decodifica(CStr(Request.QueryString("dd")),
                         AgroKey_EncoderDecoder,
                         Server)).ToShortDateString

        Data_A = CDate(Stringa_Decodifica(CStr(Request.QueryString("da")),
                         AgroKey_EncoderDecoder,
                         Server)).ToShortDateString

        Piva = Stringa_Decodifica(CStr(Request.QueryString("p")),
                                    AgroKey_EncoderDecoder,
                                    Server)

        'la pagina di filtro non manda il dato
        'RagSoc_Impresa = Stringa_Decodifica(CStr(Request.QueryString("rs")),
        '                                   AgroKey_EncoderDecoder,
        '                                   Server)

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

        Cod_Articolo = Stringa_Decodifica(CStr(Request.QueryString("ca")),
                                AgroKey_EncoderDecoder,
                                Server)

        Flag01_GestioneCodEsterno = Stringa_Decodifica(CStr(Request.QueryString("fce")),
                                AgroKey_EncoderDecoder,
                                Server)

        Mat_Des = Stringa_Decodifica(CStr(Request.QueryString("dpr")),
                                AgroKey_EncoderDecoder,
                                Server)

        If Cod_Articolo = "0" Then
            Cod_Articolo = ""
            'il codice non è stato passato, 
            'perchè si vogliono cercare tutti
            'o è stato selezionato un range di codici
        End If

        'FiltroAggArticoli = Session("Str_Codici_Specie")

        'If FiltroAggArticoli <> "" Then
        '    'è stato selezionato un range di codici
        '    FiltroAggArticoli = " AND MP_Raccolta.Cod_Articolo IN " & FiltroAggArticoli
        'Else
        '    FiltroAggArticoli = ""
        'End If

        FiltroAggArticoli = Session("Str_Codici_Prodotto")

        Cod_RisUm = CInt(Stringa_Decodifica(CStr(Request.QueryString("ru")),
                                  AgroKey_EncoderDecoder,
                                  Server))



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

        'old
        'FiltroAggConferenti = Session("Str_Codici_Conferenti")

        'If FiltroAggConferenti <> "" Then
        '    'è stato selezionato un range di codici
        '    FiltroAggConferenti = " AND Risorse_Umane_Conferenti.settore_Des IN " & FiltroAggConferenti
        'Else
        '    FiltroAggConferenti = ""
        'End If

        FiltroAggConferenti = Session("Str_CodRisUm_Conferenti")


        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))


        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim Nome_Documento As String = "RiepilogoConferimentoXArticolo"
        Dim Log_Errori As String = ""
        Dim DS As New DS_Riepilogo_Conf_Articolo

        If Not Me.IsPostBack Then

            '-----------------------------------------
            '-------- funzione del report -------------
            '-----------------------------------------
            Try

                Stampa_RiepilogoConferimentoXArticolo(DS, Log_Errori)

            Catch exc As Exception
                Log_Errori &= "- PageLoad: " & vbCrLf & exc.Message & vbCrLf
            End Try


            '-----------------------------------------
            '----------- SetDataSource ---------------
            '-----------------------------------------
            Try

                Rpt_RiepilogoConferimentoXArticolo.SetDataSource(DS)

            Catch ex As Exception
                Log_Errori &= "- Aggancio dataset: " & vbCrLf & ex.Message & vbCrLf & vbCrLf
            End Try

            '-----------------------------------------
            '----------- Parametri ---------------
            '-----------------------------------------
            Try

                'Dim ObjQDC As New AgronicaCoreStampeDAL.Stampe_QDC
                'ObjQDC.Prepara_Parametri_Intestazione_ReportQDC(Piva,
                '                                            AGRODATAINIZIO,
                '                                            AGRODATAFINE,
                '                                            Param_Rag_Soc,
                '                                            Param_Piva_CUAA,
                '                                            Param_Indirizzo,
                '                                            Nothing,
                '                                            objParametri_Server)

                Rpt_RiepilogoConferimentoXArticolo.SetParameterValue("Rag_Soc", Param_Rag_Soc)
                Rpt_RiepilogoConferimentoXArticolo.SetParameterValue("Stabilimento", Param_Sa_Nome)
                Rpt_RiepilogoConferimentoXArticolo.SetParameterValue("Periodo_Temp", Param_Date)

            Catch ex As Exception
                Log_Errori &= "- impostazione parametri: " & vbCrLf & ex.Message & vbCrLf
            End Try


            '-----------------------------------------
            '-------- Generazione Report -------------
            '-----------------------------------------

            'MS Eliminato passaggio report in session per giro su file: Session("Report") = rptRegCorrispettivi
            Dim reportTemporaneo As String = CrystalHelper.getFileReportTemporaneo()

            Try
                Rpt_RiepilogoConferimentoXArticolo.SaveAs(reportTemporaneo, True)
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
                                "Cod_Articolo = " & CStr(Cod_Articolo) & ", " & vbCrLf &
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
                                                 "RiepilogoConferimentoXArticolo",
                                                 Log_Errori)

            End If
            '-----------------------------------------

            '-----------------------------------------
            '---- redirect -------------
            '-----------------------------------------
            Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" & Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) &
                    "&tmpReportPath=" & Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server) &
                    "&NomePdf=" & Stringa_Codifica(Nome_Documento, AgroKey_EncoderDecoder, Server))

        End If

        '==================================================================


    End Sub



    '#####################################################################################################
    Private Sub Stampa_RiepilogoConferimentoXArticolo(ByRef DS As DS_Riepilogo_Conf_Articolo,
                                                    ByRef Log_Errori As String)

        Dim DT As DataTable = Nothing
        Dim i As Integer

        Param_Date = "Periodo dal "

        Param_Sa_Nome = Descr_Centro

        If CDate(Data_Da) = AGRODATAINIZIO Then
            Param_Date &= ""
        Else
            Param_Date &= Data_Da
        End If

        If CDate(Data_A) = AGRODATAFINE Then
            Param_Date &= ""
        Else
            Param_Date &= " al " & Data_A
        End If

        Try

            Dim objConfFun As New AgronicaCoreStampeDAL.Conferimento_Funzioni
            FiltroAggArticoli = objConfFun.Ricava_FiltroMatCod_da_StrCodiciProdotto(FiltroAggArticoli)
            FiltroAggConferenti = objConfFun.Ricava_FiltroCodRisUm_da_StrCodiciContatto(FiltroAggConferenti)

            Dim objFF As New AgronicaCoreStampeDAL.FreshAndFood

            DT = objFF.RiepilogoConferimentixArticolo(Flag01_GestioneCodEsterno,
                                                    Piva,
                                                    Sa_Cod,
                                                    Veg_Cod,
                                                    Cul_Cod,
                                                     Mat_Cod,
                                                    Cod_Articolo,
                                                    Codice_Conferente,
                                                    Cod_RisUm,
                                                    Data_Da,
                                                    Data_A,
                                                    FiltroAggArticoli,
                                                    FiltroAggConferenti,
                                                    "", "",
                                                    objParametri_Server)

        Catch ex As Exception
            Log_Errori &= "- Lettura dei dati: " & vbCrLf & ex.Message & vbCrLf & vbCrLf
        End Try

        'Dim Tot_Glob_PesoLordo As Double = 0
        'Dim Tot_Glob_PesoNetto As Double = 0
        'Dim Tot_Glob_Degrado As Double = 0
        'Dim Tot_Glob_NettpPag As Double = 0
        'Dim Tot_Glob_NettoXDegradoPerc As Double = 0
        'Dim Tot_Glob_NettoXPunteggio As Double = 0
        'Dim Degrado_Perc_Medio_Globale As Double = 0
        'Dim Indice_Qualit_Medio_Globale As Double = 0


        If Not IsNothing(DT) AndAlso DT.Rows.Count <> 0 Then

            Dim objADDFun As New AgronicaCoreStampeDAL.AccettazioneDaDiversi_Funzioni
            Dim objConfFun As New AgronicaCoreStampeDAL.Conferimento_Funzioni

            Dim Progr_Conferente As String = ""
            Dim RagSoc_Conferente As String = ""

            Dim Cod_Articolo As String
            Dim Desc_Articolo As String = ""
            'Dim Descr_Specie, Descr_Varieta As String

            Dim Peso_Lordo As Double
            Dim Peso_Netto As Double
            Dim Netto_Pagamento As Double
            Dim Degrado As Double
            Dim Degrado_Perc As Double
            Dim Degrado_Perc_Medio As Double
            Dim Indice_Qualit As Double
            Dim Indice_Qualit_Medio As Double
            Dim Tara_Imballi As Double
            Dim Udm_Cod As Integer
            Dim PesoNettoXDegradoPerc As Double
            Dim PesoNettoXPunteggio As Double

            Dim Temp_Specie As String = ""
            Dim Temp_DescrSpecie As String = ""
            Dim Temp_Conferente As String = ""
            Dim Temp_RagSocConferente As String = ""
            Dim Temp_UdmCod As Integer = 0

            Dim Tot_Conf_PesoLordo As Double
            Dim Tot_Conf_PesoNetto As Double
            Dim Tot_Conf_Degrado As Double
            Dim Tot_Conf_NettoPag As Double
            Dim Tot_Conf_NettoXDegradoPerc As Double
            Dim Tot_Conf_NettoXPunteggio As Double

            For i = 0 To DT.Rows.Count - 1

                If i = 0 Then
                    Param_Rag_Soc = DT.Rows(i).Item("rag_soc")
                End If

                Progr_Conferente = DT.Rows(i).Item("Codice_Conferente")
                RagSoc_Conferente = DT.Rows(i).Item("Rag_Soc_Conferente")

                If Flag01_GestioneCodEsterno = 1 Then
                    Cod_Articolo = DT.Rows(i).Item("Codice_Esterno")
                Else
                    Cod_Articolo = DT.Rows(i).Item("Cod_Articolo")
                End If

                If CInt(Session("ASG_ProgressivoGIAS")) = enum_CodiceGIAS_Clienti.Fruttagel Then
                    objConfFun.Ricava_Specie_Varieta_FRG(DT.Rows(i).Item("Mat_Des"), Desc_Articolo, Nothing)
                Else
                    Desc_Articolo = DT.Rows(i).Item("Mat_Des")
                End If

                '=====================================================================
                'PESI - CONF NUOVO:
                'bisogna leggere solo quelli su movimenti_dettagli
                'peso netto = qta_extra_totale
                'tara imballi = tara
                'il resto da calcolare

                Peso_Netto = DT.Rows(i).Item("qta_extra_totale_Raccolta")
                Peso_Netto = ArrotondaVal_0(Peso_Netto)

                'Tara_Imballi = DT.Rows(i).Item("Tara")
                'Tara_Imballi = ArrotondaVal_0(Tara_Imballi)

                Tara_Imballi = objConfFun.Calcola_TaraImballi(DT.Rows(i).Item("tara_totale_imb_entrata"),
                                                              DT.Rows(i).Item("Tara_Dettaglio"),
                                                              DT.Rows(i).Item("Num_Righe"))

                Peso_Lordo = Peso_Netto + Tara_Imballi
                Peso_Lordo = ArrotondaVal_0(Peso_Lordo)

                Degrado_Perc = DT.Rows(i).Item("Variazione_Raccolta")
                Udm_Cod = DT.Rows(i).Item("Udm_Cod_Raccolta")

                Indice_Qualit = DT.Rows(i).Item("Punteggio")

                objADDFun.Calcola_NettoPagamento(Peso_Netto,
                                                 Degrado_Perc,
                                                 Udm_Cod,
                                                 Degrado,
                                                 Netto_Pagamento)

                PesoNettoXDegradoPerc = Peso_Netto * Degrado_Perc

                PesoNettoXPunteggio = Peso_Netto * Indice_Qualit

                '================================

                If Temp_Specie = "" Then
                    'primo giro
                    Temp_Specie = Cod_Articolo
                    Temp_DescrSpecie = Desc_Articolo
                End If

                If Temp_Specie <> Cod_Articolo Then
                    'è cambiata la specie

                    Insert_Ultima_Riga_Specie(DS,
                                            Temp_Specie,
                                            Temp_DescrSpecie,
                                            Progr_Conferente,
                                            RagSoc_Conferente,
                                            Udm_Cod,
                                            Peso_Lordo,
                                            Peso_Netto,
                                            Degrado,
                                            Netto_Pagamento,
                                            PesoNettoXDegradoPerc,
                                            PesoNettoXPunteggio,
                                            Temp_Conferente,
                                            Temp_RagSocConferente,
                                            Temp_UdmCod,
                                            Degrado_Perc_Medio,
                                            Indice_Qualit_Medio,
                                            Tot_Conf_NettoXDegradoPerc,
                                            Tot_Conf_NettoXPunteggio,
                                            Tot_Conf_PesoLordo,
                                            Tot_Conf_PesoNetto,
                                            Tot_Conf_Degrado,
                                            Tot_Conf_NettoPag)

                    Temp_Specie = Cod_Articolo
                    Temp_DescrSpecie = Desc_Articolo

                    'azzero il conferente
                    Temp_Conferente = ""
                    'azzero i totali
                    Tot_Conf_PesoLordo = 0
                    Tot_Conf_PesoNetto = 0
                    Tot_Conf_Degrado = 0
                    Tot_Conf_NettoPag = 0
                    Tot_Conf_NettoXDegradoPerc = 0
                    Tot_Conf_NettoXPunteggio = 0
                    'azzero le medie
                    Degrado_Perc_Medio = 0
                    Indice_Qualit_Medio = 0
                End If

                If Temp_Conferente = "" Then
                    'primo giro
                    Temp_Conferente = Progr_Conferente
                    Temp_RagSocConferente = RagSoc_Conferente
                    Temp_UdmCod = Udm_Cod
                End If

                If Temp_Conferente = Progr_Conferente Then
                    'si tratta dello stesso conferente
                    'devo sommare le quantità

                    Tot_Conf_PesoLordo += Peso_Lordo
                    Tot_Conf_PesoNetto += Peso_Netto
                    Tot_Conf_Degrado += Degrado
                    Tot_Conf_NettoPag += Netto_Pagamento
                    Tot_Conf_NettoXDegradoPerc += PesoNettoXDegradoPerc
                    Tot_Conf_NettoXPunteggio += PesoNettoXPunteggio

                Else
                    'è cambiato il conferente

                    Insert_Ultima_Riga_Conf(DS,
                                            Temp_Specie,
                                            Temp_DescrSpecie,
                                            Progr_Conferente,
                                            RagSoc_Conferente,
                                            Udm_Cod,
                                            Peso_Lordo,
                                            Peso_Netto,
                                            Degrado,
                                            Netto_Pagamento,
                                            PesoNettoXDegradoPerc,
                                            PesoNettoXPunteggio,
                                            Temp_Conferente,
                                            Temp_RagSocConferente,
                                            Temp_UdmCod,
                                            Degrado_Perc_Medio,
                                            Indice_Qualit_Medio,
                                            Tot_Conf_NettoXDegradoPerc,
                                            Tot_Conf_NettoXPunteggio,
                                            Tot_Conf_PesoLordo,
                                            Tot_Conf_PesoNetto,
                                            Tot_Conf_Degrado,
                                            Tot_Conf_NettoPag)

                End If

            Next

            'per l'ultimo conferente occorre chiudere il giro:

            Insert_Ultima_Riga_Conf(DS,
                                    Temp_Specie,
                                    Temp_DescrSpecie,
                                    Progr_Conferente,
                                    RagSoc_Conferente,
                                    Udm_Cod,
                                    Peso_Lordo,
                                    Peso_Netto,
                                    Degrado,
                                    Netto_Pagamento,
                                    PesoNettoXDegradoPerc,
                                    PesoNettoXPunteggio,
                                    Temp_Conferente,
                                    Temp_RagSocConferente,
                                    Temp_UdmCod,
                                    Degrado_Perc_Medio,
                                    Indice_Qualit_Medio,
                                    Tot_Conf_NettoXDegradoPerc,
                                    Tot_Conf_NettoXPunteggio,
                                    Tot_Conf_PesoLordo,
                                    Tot_Conf_PesoNetto,
                                    Tot_Conf_Degrado,
                                    Tot_Conf_NettoPag)

            ''calcolo le medie
            'Degrado_Perc_Medio = Tot_Conf_NettoXDegradoPerc / Tot_Conf_PesoNetto
            'Degrado_Perc_Medio = Arrotonda_2Decimali(Degrado_Perc_Medio)

            'Indice_Qualit_Medio = Tot_Conf_NettoXPunteggio / Tot_Conf_PesoNetto
            'Indice_Qualit_Medio = Arrotonda_2Decimali(Indice_Qualit_Medio)

            '''sommo i totali x conf ai totali del report
            ''Tot_Glob_PesoLordo += Tot_Conf_PesoLordo
            ''Tot_Glob_PesoNetto += Tot_Conf_PesoNetto
            ''Tot_Glob_Degrado += Tot_Conf_Degrado
            ''Tot_Glob_NettpPag += Tot_Conf_NettoPag
            ''Tot_Glob_NettoXDegradoPerc += Tot_Conf_NettoXDegradoPerc
            ''Tot_Glob_NettoXPunteggio += Tot_Conf_NettoXPunteggio

            ''-----------------------------------------
            ''aggiungo la riga nel dataset
            'DR = DS.DS_RiepilogoConferimentoXSpecie.NewDS_RiepilogoConferimentoXSpecieRow

            'DR.Codice_Conferente = Temp_Conferente
            'DR.RagSoc_Conferente = Temp_RagSocConferente

            'Select Case Udm_Cod
            '    Case 2 'kg
            '        Tot_Conf_PesoLordo = Tot_Conf_PesoLordo / 100
            '        Tot_Conf_PesoNetto = Tot_Conf_PesoNetto / 100
            '        Tot_Conf_Degrado = Tot_Conf_Degrado / 100
            '        Tot_Conf_NettoPag = Tot_Conf_NettoPag / 100
            '    Case 4 'q
            '        Tot_Conf_PesoLordo = Tot_Conf_PesoLordo
            '        Tot_Conf_PesoNetto = Tot_Conf_PesoNetto
            '        Tot_Conf_Degrado = Tot_Conf_Degrado
            '        Tot_Conf_NettoPag = Tot_Conf_NettoPag
            '    Case Else
            '        'errore
            '        Tot_Conf_PesoLordo = Tot_Conf_PesoLordo / 100
            '        Tot_Conf_PesoNetto = Tot_Conf_PesoNetto / 100
            '        Tot_Conf_Degrado = Tot_Conf_Degrado / 100
            '        Tot_Conf_NettoPag = Tot_Conf_NettoPag / 100
            'End Select

            'DR.Peso_Lordo = Format(Tot_Conf_PesoLordo, "#,###,##0.00")
            'DR.Peso_Netto = Format(Tot_Conf_PesoNetto, "#,###,##0.00")
            'DR.Degrado = Format(Tot_Conf_Degrado, "#,###,##0.00")
            'DR.Netto_Pag = Format(Tot_Conf_NettoPag, "#,###,##0.00")

            'DR.Degrado_Perc_Medio = Format(Degrado_Perc_Medio, "#,###,##0.00") 'Degrado_Perc_Medio
            'DR.Indice_Qualit_Medio = Format(Indice_Qualit_Medio, "#,###,##0.00") 'Indice_Qualit_Medio

            'DS.DS_RiepilogoConferimentoXSpecie.Rows.Add(DR)
            ''---------------------------

            ''calcolo le medie totali
            'Degrado_Perc_Medio_Globale = Tot_Glob_NettoXDegradoPerc / Tot_Glob_PesoNetto
            'Degrado_Perc_Medio_Globale = Arrotonda_2Decimali(Degrado_Perc_Medio_Globale)

            'Indice_Qualit_Medio_Globale = Tot_Glob_NettoXPunteggio / Tot_Glob_PesoNetto
            'Indice_Qualit_Medio_Globale = Arrotonda_2Decimali(Indice_Qualit_Medio_Globale)

            'Select Case Udm_Cod
            '    Case 2 'kg
            '        Tot_Glob_PesoLordo = Tot_Glob_PesoLordo / 100
            '        Tot_Glob_PesoNetto = Tot_Glob_PesoNetto / 100
            '        Tot_Glob_Degrado = Tot_Glob_Degrado / 100
            '        Tot_Glob_NettpPag = Tot_Glob_NettpPag / 100
            '    Case 4 'q
            '        'rimangono come sono
            '    Case Else
            '        'errore
            '        Tot_Glob_PesoLordo = Tot_Glob_PesoLordo / 100
            '        Tot_Glob_PesoNetto = Tot_Glob_PesoNetto / 100
            '        Tot_Glob_Degrado = Tot_Glob_Degrado / 100
            '        Tot_Glob_NettpPag = Tot_Glob_NettpPag / 100
            'End Select


        End If 'controllo sul dt

        '####################################################


    End Sub

    '####################################################
    Private Sub Insert_Ultima_Riga_Conf(ByRef DS As DS_Riepilogo_Conf_Articolo,
                                        ByVal Temp_Specie As String,
                                        ByVal Temp_DescrSpecie As String,
                                        ByVal Progr_Conferente As String,
                                        ByVal RagSoc_Conferente As String,
                                        ByVal Udm_Cod As Integer,
                                        ByRef Peso_Lordo As Double,
                                        ByRef Peso_Netto As Double,
                                        ByRef Degrado As Double,
                                        ByRef Netto_Pagamento As Double,
                                        ByRef PesoNettoXDegradoPerc As Double,
                                        ByRef PesoNettoXPunteggio As Double,
                                        ByRef Temp_Conferente As String,
                                        ByRef Temp_RagSocConferente As String,
                                        ByRef Temp_UdmCod As Integer,
                                        ByRef Degrado_Perc_Medio As Double,
                                        ByRef Indice_Qualit_Medio As Double,
                                        ByRef Tot_Conf_NettoXDegradoPerc As Double,
                                        ByRef Tot_Conf_NettoXPunteggio As Double,
                                        ByRef Tot_Conf_PesoLordo As Double,
                                        ByRef Tot_Conf_PesoNetto As Double,
                                        ByRef Tot_Conf_Degrado As Double,
                                        ByRef Tot_Conf_NettoPag As Double)

        Dim DR As DS_Riepilogo_Conf_Articolo.DT_Riepilogo_Conf_ArticoloRow

        'prima di gestire il nuovo conferente
        'calcolo le medie del conferente precedente e aggiungo la riga al dataset

        'calcolo delle medie
        Degrado_Perc_Medio = Tot_Conf_NettoXDegradoPerc / Tot_Conf_PesoNetto
        Degrado_Perc_Medio = Arrotonda_2Decimali(Degrado_Perc_Medio)

        Indice_Qualit_Medio = Tot_Conf_NettoXPunteggio / Tot_Conf_PesoNetto
        Indice_Qualit_Medio = Arrotonda_2Decimali(Indice_Qualit_Medio)

        ''sommo i totali x conf ai totali del report
        'Tot_Glob_PesoLordo += Tot_Conf_PesoLordo
        'Tot_Glob_PesoNetto += Tot_Conf_PesoNetto
        'Tot_Glob_Degrado += Tot_Conf_Degrado
        'Tot_Glob_NettpPag += Tot_Conf_NettoPag
        'Tot_Glob_NettoXDegradoPerc += Tot_Conf_NettoXDegradoPerc
        'Tot_Glob_NettoXPunteggio += Tot_Conf_NettoXPunteggio

        '-------------
        'aggiungo la riga nel dataset
        DR = DS.DT_Riepilogo_Conf_Articolo.NewDT_Riepilogo_Conf_ArticoloRow

        DR.Codice_Articolo = Temp_Specie
        DR.Descr_Articolo = Temp_DescrSpecie

        DR.Codice_Conferente = Temp_Conferente
        DR.RagSoc_Conferente = Temp_RagSocConferente

        Select Case Udm_Cod
            Case 2 'kg
                Tot_Conf_PesoLordo = Tot_Conf_PesoLordo / 100
                Tot_Conf_PesoNetto = Tot_Conf_PesoNetto / 100
                Tot_Conf_Degrado = Tot_Conf_Degrado / 100
                Tot_Conf_NettoPag = Tot_Conf_NettoPag / 100
            Case 4 'q
                Tot_Conf_PesoLordo = Tot_Conf_PesoLordo
                Tot_Conf_PesoNetto = Tot_Conf_PesoNetto
                Tot_Conf_Degrado = Tot_Conf_Degrado
                Tot_Conf_NettoPag = Tot_Conf_NettoPag
            Case Else
                'errore
                Tot_Conf_PesoLordo = Tot_Conf_PesoLordo / 100
                Tot_Conf_PesoNetto = Tot_Conf_PesoNetto / 100
                Tot_Conf_Degrado = Tot_Conf_Degrado / 100
                Tot_Conf_NettoPag = Tot_Conf_NettoPag / 100
        End Select

        DR.Peso_Lordo = Format(Tot_Conf_PesoLordo, "#,###,##0.00")
        DR.Peso_Netto = Format(Tot_Conf_PesoNetto, "#,###,##0.00")
        DR.Degrado = Format(Tot_Conf_Degrado, "#,###,##0.00")
        DR.Netto_Pag = Format(Tot_Conf_NettoPag, "#,###,##0.00")

        DR.Degrado_Perc_Medio = Format(Degrado_Perc_Medio, "#,###,##0.00") 'Degrado_Perc_Medio
        DR.Indice_Qualit_Medio = Format(Indice_Qualit_Medio, "#,###,##0.00") 'Indice_Qualit_Medio

        DS.DT_Riepilogo_Conf_Articolo.Rows.Add(DR)
        '---------------------------

        'salvo in temp il nuovo
        Temp_Conferente = Progr_Conferente
        Temp_RagSocConferente = RagSoc_Conferente
        Temp_UdmCod = Udm_Cod

        'azzero i totali per conferente
        Tot_Conf_PesoLordo = 0
        Tot_Conf_PesoNetto = 0
        Tot_Conf_Degrado = 0
        Tot_Conf_NettoPag = 0
        Tot_Conf_NettoXDegradoPerc = 0
        Tot_Conf_NettoXPunteggio = 0

        'azzero le medie
        Degrado_Perc_Medio = 0
        Indice_Qualit_Medio = 0

        ''leggo la ragione sociale a questo punto
        ''(anzichè salvarla in una variabile temp)
        ''perchè altrimenti nel dataset si scriverebbe 
        ''la ragione sociale corrente e non quella del conferente precedente
        'RagSoc_Conferente = DT.Rows(i).Item("Rag_Soc_Conferente")

        'sommo le quantità del nuovo conferente
        Tot_Conf_PesoLordo += Peso_Lordo
        Tot_Conf_PesoNetto += Peso_Netto
        Tot_Conf_Degrado += Degrado
        Tot_Conf_NettoPag += Netto_Pagamento
        Tot_Conf_NettoXDegradoPerc += PesoNettoXDegradoPerc
        Tot_Conf_NettoXPunteggio += PesoNettoXPunteggio

    End Sub



    '####################################################
    Private Sub Insert_Ultima_Riga_Specie(ByRef DS As DS_Riepilogo_Conf_Articolo,
                                        ByVal Temp_Specie As String,
                                        ByVal Temp_DescrSpecie As String,
                                        ByVal Progr_Conferente As String,
                                        ByVal RagSoc_Conferente As String,
                                        ByVal Udm_Cod As Integer,
                                        ByRef Peso_Lordo As Double,
                                        ByRef Peso_Netto As Double,
                                        ByRef Degrado As Double,
                                        ByRef Netto_Pagamento As Double,
                                        ByRef PesoNettoXDegradoPerc As Double,
                                        ByRef PesoNettoXPunteggio As Double,
                                        ByRef Temp_Conferente As String,
                                        ByRef Temp_RagSocConferente As String,
                                        ByRef Temp_UdmCod As Integer,
                                        ByRef Degrado_Perc_Medio As Double,
                                        ByRef Indice_Qualit_Medio As Double,
                                        ByRef Tot_Conf_NettoXDegradoPerc As Double,
                                        ByRef Tot_Conf_NettoXPunteggio As Double,
                                        ByRef Tot_Conf_PesoLordo As Double,
                                        ByRef Tot_Conf_PesoNetto As Double,
                                        ByRef Tot_Conf_Degrado As Double,
                                        ByRef Tot_Conf_NettoPag As Double)

        Dim DR As DS_Riepilogo_Conf_Articolo.DT_Riepilogo_Conf_ArticoloRow

        'prima di gestire il nuovo conferente
        'calcolo le medie del conferente precedente e aggiungo la riga al dataset

        'calcolo delle medie
        Degrado_Perc_Medio = Tot_Conf_NettoXDegradoPerc / Tot_Conf_PesoNetto
        Degrado_Perc_Medio = Arrotonda_2Decimali(Degrado_Perc_Medio)

        Indice_Qualit_Medio = Tot_Conf_NettoXPunteggio / Tot_Conf_PesoNetto
        Indice_Qualit_Medio = Arrotonda_2Decimali(Indice_Qualit_Medio)

        ''sommo i totali x conf ai totali del report
        'Tot_Glob_PesoLordo += Tot_Conf_PesoLordo
        'Tot_Glob_PesoNetto += Tot_Conf_PesoNetto
        'Tot_Glob_Degrado += Tot_Conf_Degrado
        'Tot_Glob_NettpPag += Tot_Conf_NettoPag
        'Tot_Glob_NettoXDegradoPerc += Tot_Conf_NettoXDegradoPerc
        'Tot_Glob_NettoXPunteggio += Tot_Conf_NettoXPunteggio

        '-------------
        'aggiungo la riga nel dataset
        DR = DS.DT_Riepilogo_Conf_Articolo.NewDT_Riepilogo_Conf_ArticoloRow

        DR.Codice_Articolo = Temp_Specie
        DR.Descr_Articolo = Temp_DescrSpecie

        DR.Codice_Conferente = Temp_Conferente
        DR.RagSoc_Conferente = Temp_RagSocConferente

        Select Case Udm_Cod
            Case 2 'kg
                Tot_Conf_PesoLordo = Tot_Conf_PesoLordo / 100
                Tot_Conf_PesoNetto = Tot_Conf_PesoNetto / 100
                Tot_Conf_Degrado = Tot_Conf_Degrado / 100
                Tot_Conf_NettoPag = Tot_Conf_NettoPag / 100
            Case 4 'q
                Tot_Conf_PesoLordo = Tot_Conf_PesoLordo
                Tot_Conf_PesoNetto = Tot_Conf_PesoNetto
                Tot_Conf_Degrado = Tot_Conf_Degrado
                Tot_Conf_NettoPag = Tot_Conf_NettoPag
            Case Else
                'errore
                Tot_Conf_PesoLordo = Tot_Conf_PesoLordo / 100
                Tot_Conf_PesoNetto = Tot_Conf_PesoNetto / 100
                Tot_Conf_Degrado = Tot_Conf_Degrado / 100
                Tot_Conf_NettoPag = Tot_Conf_NettoPag / 100
        End Select

        DR.Peso_Lordo = Format(Tot_Conf_PesoLordo, "#,###,##0.00")
        DR.Peso_Netto = Format(Tot_Conf_PesoNetto, "#,###,##0.00")
        DR.Degrado = Format(Tot_Conf_Degrado, "#,###,##0.00")
        DR.Netto_Pag = Format(Tot_Conf_NettoPag, "#,###,##0.00")

        DR.Degrado_Perc_Medio = Format(Degrado_Perc_Medio, "#,###,##0.00") 'Degrado_Perc_Medio
        DR.Indice_Qualit_Medio = Format(Indice_Qualit_Medio, "#,###,##0.00") 'Indice_Qualit_Medio

        DS.DT_Riepilogo_Conf_Articolo.Rows.Add(DR)
        '---------------------------

    End Sub





End Class