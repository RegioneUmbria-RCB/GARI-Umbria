Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza

Public Class RiepilogoConferimentoXSpecie
    Inherits System.Web.UI.Page

    '----------------------------
    '   DICHIARAZIONE REPORT
    '----------------------------
    Private Rpt_RiepilogoConferimentoXSpecie As Rpt_RiepilogoConferimentoXSpecie

#Region " RiepilogoConferimentoXSpecie "

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

        '----------------------------
        '   INIZIALIZZAZIONE REPORT
        '----------------------------
        Rpt_RiepilogoConferimentoXSpecie = New Rpt_RiepilogoConferimentoXSpecie

    End Sub

#End Region

    Dim Data_Da, Data_A As String
    Dim Codice_Conferente As String
    Dim Codice_Specie As String
    Dim Str_FiltroConf As String
    Dim Str_FiltroSpecie As String
    Dim Piva As String
    Dim Sa_Cod, Fabbricato_Cod As Integer
    Dim RagSoc_Impresa As String
    'Dim Descr_Specie As String
    Dim Descr_Magazzino As String

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri



    '#####################################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim objImpreseR As New AgronicaCoreAnagrafeDAL.Imprese_Read
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        '##############################################################
        '############  Lettura Parametri Query String #################
        '##############################################################

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

        'Descr_Specie = Stringa_Decodifica(CStr(Request.QueryString("spe")), _
        '                                    AgroKey_EncoderDecoder, _
        '                                    Server)

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


        Codice_Conferente = Stringa_Decodifica(CStr(Request.QueryString("cc")), _
                                AgroKey_EncoderDecoder, _
                                Server)

        If Codice_Conferente = "0" Then
            Codice_Conferente = ""
            'il codice non è stato passato, 
            'perchè si vogliono cercare tutti i conferenti
            'o il range di conferenti selezionato
        Else
            Codice_Conferente = CStr(Codice_Conferente)
        End If

        Str_FiltroConf = Session("Str_Codici_Conferenti")

        If Str_FiltroConf <> "" Then
            'è stato selezionato un range di codici
            Str_FiltroConf = " AND Risorse_Umane_Conferenti.settore_Des IN " & Str_FiltroConf
        Else
            Str_FiltroConf = ""
        End If




        
        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        'Dim CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer

        Dim Nome_Documento As String = "RiepilogoConferimentoXSpecie"
        Dim Log_Errori As String

        If Not Me.IsPostBack Then

            '--------------------------------------------
            ' DICHIARAZIONE  E INIZIALIZZAZIONE DATASET
            '--------------------------------------------
            Dim DS As New DS_RiepilogoConferimentoXSpecie

            Try


                Log_Errori = ""


                'FUNZIONE CHE FA TUTTO
                Stampa_RiepilogoConferimentoXSpecie(DS, Log_Errori)


            Catch exc As Exception
                Log_Errori += "- PageLoad: " + vbCrLf + exc.Message + vbCrLf
            End Try

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            Dim Nome_File As String

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento + ", " + vbCrLf + _
                                "Data DA = " + CStr(Data_Da) + ", " + vbCrLf + _
                                "Data A = " + CStr(Data_A) + ", " + vbCrLf + _
                                "Codice Conferente = " + CStr(Codice_Conferente) + ", " + vbCrLf + _
                                "Codice Specie = " + CStr(Codice_Specie) + ", " + vbCrLf + _
                                "Stringa Filtro Specie = " + CStr(Str_FiltroSpecie) + ", " + vbCrLf + _
                                "Stringa Filtro Conferenti = " + CStr(Str_FiltroConf) + ", " + vbCrLf + _
                                "Piva = " + CStr(Piva) + ", " + vbCrLf + _
                                "Sa_Cod = " + CStr(Sa_Cod) + ", " + vbCrLf + _
                                "Fabbricato_Cod = " + CStr(Fabbricato_Cod) + ", " + vbCrLf + _
                                vbCrLf + vbCrLf + vbCrLf + vbCrLf + _
                                Log_Errori

                Nome_File = "Log_Errori_" + Nome_Documento + "_" + CStr(Session("ASG_Utente_Username")) + "_" + CStr(Piva)

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server, _
                                                 "Stampe_AccettazioneDaDiversi", _
                                                 Nome_File, _
                                                 Session("ASG_Utente_Username"), _
                                                 "RiepilogoConferimentoXSpecie.aspx", _
                                                 Log_Errori)


            End If
            '-----------------------------------------



            '==================================================================

            Try
                Session("Report") = Rpt_RiepilogoConferimentoXSpecie
                Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))
            Catch ex As Exception
                Log_Errori += "- Export: " + vbCrLf + ex.Message + vbCrLf
            End Try

        End If


    End Sub



    '#####################################################################################################
    Private Sub Stampa_RiepilogoConferimentoXSpecie(ByRef DS As DS_RiepilogoConferimentoXSpecie, _
                                                    ByRef Log_Errori As String)

        Dim DT As DataTable
        Dim i As Integer
        Dim objADDFun As New AgronicaCoreStampeDAL.AccettazioneDaDiversi_Funzioni


        CType(Rpt_RiepilogoConferimentoXSpecie.Section2.ReportObjects("TxtImpresa"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = RagSoc_Impresa
        CType(Rpt_RiepilogoConferimentoXSpecie.Section2.ReportObjects("TxtStabilimento"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Descr_Magazzino
        'CType(Rpt_RiepilogoConferimentoXSpecie.Section2.ReportObjects("TxtSpecie"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Codice_Specie + " " + Descr_Specie

        If CDate(Data_Da) = AGRODATAINIZIO Then
            CType(Rpt_RiepilogoConferimentoXSpecie.Section2.ReportObjects("TxtDataDA"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""
        Else
            CType(Rpt_RiepilogoConferimentoXSpecie.Section2.ReportObjects("TxtDataDA"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Data_Da
        End If

        If CDate(Data_A) = AGRODATAFINE Then
            CType(Rpt_RiepilogoConferimentoXSpecie.Section2.ReportObjects("TxtDataA"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""
        Else
            CType(Rpt_RiepilogoConferimentoXSpecie.Section2.ReportObjects("TxtDataA"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Data_A
        End If

        Try

            'DT = NewCom_ADD_RiepilogoConferimentixSpecie_Leggi(Server, Session, Page, _
            '                                                    Piva, _
            '                                                    Sa_Cod, _
            '                                                    Fabbricato_Cod, _
            '                                                    Codice_Specie, _
            '                                                    Codice_Conferente, _
            '                                                    Data_Da, _
            '                                                    Data_A, _
            '                                                    Str_FiltroSpecie, _
            '                                                    Str_FiltroConf, _
            '                                                    "")

            Dim ADD As New AgronicaCoreStampeDAL.AccettazioneDaDiversi

            DT = ADD.RiepilogoConferimentixSpecie(Piva, _
                                                Sa_Cod, _
                                                Fabbricato_Cod, _
                                                Codice_Specie, _
                                                Codice_Conferente, _
                                                Data_Da, _
                                                Data_A, _
                                                Str_FiltroSpecie, _
                                                Str_FiltroConf, _
                                                "", "", _
                                                objParametri_Server)


        Catch ex As Exception
            Log_Errori += "- Lettura dei dati: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
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

            Dim Progr_Conferente As String
            Dim RagSoc_Conferente As String

            Dim Cod_Articolo As String
            Dim Descr_Specie, Descr_Varieta As String

            Dim Peso_Lordo As Double
            Dim Peso_Netto As Double
            Dim Netto_Pagamento As Double
            Dim Degrado As Double
            Dim Degrado_Perc As Double
            Dim Degrado_Perc_Medio As Double
            Dim Indice_Qualit As Double
            Dim Indice_Qualit_Medio As Double
            Dim Tipo_Peso As Integer
            Dim Tara_Imballi As Double
            Dim Peso As Double
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

            Dim DR As DS_RiepilogoConferimentoXSpecie.DS_RiepilogoConferimentoXSpecieRow


            For i = 0 To DT.Rows.Count - 1

                Progr_Conferente = DT.Rows(i).Item("Codice_Conferente")
                RagSoc_Conferente = DT.Rows(i).Item("Rag_Soc_Conferente")

                Cod_Articolo = DT.Rows(i).Item("Cod_Articolo_Raccolta")

                objADDFun.Ricava_Specie_Varieta_FRG(DT.Rows(i).Item("Mat_Des_Raccolta"), Descr_Specie, Descr_Varieta)

                Peso_Netto = DT.Rows(i).Item("Qta_Raccolta")

                Tipo_Peso = DT.Rows(i).Item("Tipo_Peso")
                Peso = DT.Rows(i).Item("Peso")
                Tara_Imballi = DT.Rows(i).Item("Tara_Imballi")

                Peso_Lordo = Calcola_PesoLordo(Tipo_Peso, Peso, Tara_Imballi)

                Degrado_Perc = DT.Rows(i).Item("Variazione_Raccolta")
                Udm_Cod = DT.Rows(i).Item("Udm_Cod_Raccolta")

                Indice_Qualit = DT.Rows(i).Item("Punteggio")

                objADDFun.Calcola_NettoPagamento(Peso_Netto, Degrado_Perc, Udm_Cod, Degrado, Netto_Pagamento)

                PesoNettoXDegradoPerc = Peso_Netto * Degrado_Perc

                PesoNettoXPunteggio = Peso_Netto * Indice_Qualit

                '================================

                If Temp_Specie = "" Then
                    'primo giro
                    Temp_Specie = Cod_Articolo
                    Temp_DescrSpecie = Descr_Specie
                End If

                If Temp_Specie <> Cod_Articolo Then
                    'è cambiata la specie

                    Insert_Ultima_Riga_Specie(DS, _
                                            Temp_Specie, _
                                            Temp_DescrSpecie, _
                                            Progr_Conferente, _
                                            RagSoc_Conferente, _
                                            Udm_Cod, _
                                            Peso_Lordo, _
                                            Peso_Netto, _
                                            Degrado, _
                                            Netto_Pagamento, _
                                            PesoNettoXDegradoPerc, _
                                            PesoNettoXPunteggio, _
                                            Temp_Conferente, _
                                            Temp_RagSocConferente, _
                                            Temp_UdmCod, _
                                            Degrado_Perc_Medio, _
                                            Indice_Qualit_Medio, _
                                            Tot_Conf_NettoXDegradoPerc, _
                                            Tot_Conf_NettoXPunteggio, _
                                            Tot_Conf_PesoLordo, _
                                            Tot_Conf_PesoNetto, _
                                            Tot_Conf_Degrado, _
                                            Tot_Conf_NettoPag)

                    Temp_Specie = Cod_Articolo
                    Temp_DescrSpecie = Descr_Specie

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

                    Insert_Ultima_Riga_Conf(DS, _
                                            Temp_Specie, _
                                            Temp_DescrSpecie, _
                                            Progr_Conferente, _
                                            RagSoc_Conferente, _
                                            Udm_Cod, _
                                            Peso_Lordo, _
                                            Peso_Netto, _
                                            Degrado, _
                                            Netto_Pagamento, _
                                            PesoNettoXDegradoPerc, _
                                            PesoNettoXPunteggio, _
                                            Temp_Conferente, _
                                            Temp_RagSocConferente, _
                                            Temp_UdmCod, _
                                            Degrado_Perc_Medio, _
                                            Indice_Qualit_Medio, _
                                            Tot_Conf_NettoXDegradoPerc, _
                                            Tot_Conf_NettoXPunteggio, _
                                            Tot_Conf_PesoLordo, _
                                            Tot_Conf_PesoNetto, _
                                            Tot_Conf_Degrado, _
                                            Tot_Conf_NettoPag)

                End If

            Next

            'per l'ultimo conferente occorre chiudere il giro:

            Insert_Ultima_Riga_Conf(DS, _
                                    Temp_Specie, _
                                    Temp_DescrSpecie, _
                                    Progr_Conferente, _
                                    RagSoc_Conferente, _
                                    Udm_Cod, _
                                    Peso_Lordo, _
                                    Peso_Netto, _
                                    Degrado, _
                                    Netto_Pagamento, _
                                    PesoNettoXDegradoPerc, _
                                    PesoNettoXPunteggio, _
                                    Temp_Conferente, _
                                    Temp_RagSocConferente, _
                                    Temp_UdmCod, _
                                    Degrado_Perc_Medio, _
                                    Indice_Qualit_Medio, _
                                    Tot_Conf_NettoXDegradoPerc, _
                                    Tot_Conf_NettoXPunteggio, _
                                    Tot_Conf_PesoLordo, _
                                    Tot_Conf_PesoNetto, _
                                    Tot_Conf_Degrado, _
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

        'CType(Rpt_RiepilogoConferimentoXSpecie.Section4.ReportObjects("TxtTotLordo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Tot_Glob_PesoLordo, "#,###,##0.00")  'CStr(Tot_Glob_PesoLordo)
        'CType(Rpt_RiepilogoConferimentoXSpecie.Section4.ReportObjects("TxtTotNetto"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Tot_Glob_PesoNetto, "#,###,##0.00")  'CStr(Tot_Glob_PesoNetto)
        'CType(Rpt_RiepilogoConferimentoXSpecie.Section4.ReportObjects("TxtTotDegrado"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Tot_Glob_Degrado, "#,###,##0.00")  'CStr(Tot_Glob_Degrado)
        'CType(Rpt_RiepilogoConferimentoXSpecie.Section4.ReportObjects("TxtTotNettoPag"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Tot_Glob_NettpPag, "#,###,##0.00") ' CStr(Tot_Glob_NettpPag)
        'CType(Rpt_RiepilogoConferimentoXSpecie.Section4.ReportObjects("TxtTotDegradoMedio"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Degrado_Perc_Medio_Globale, "#,###,##0.00") ' CStr(Degrado_Perc_Medio_Globale)
        'CType(Rpt_RiepilogoConferimentoXSpecie.Section4.ReportObjects("TxtTotIndiceMedio"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Indice_Qualit_Medio_Globale, "#,###,##0.00") ' CStr(Indice_Qualit_Medio_Globale)


        '####################################################


        Try

            'imposto il datset sul report
            Rpt_RiepilogoConferimentoXSpecie.SetDataSource(DS)


        Catch ex As Exception
            Log_Errori += "- Aggancio dataset: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try

    End Sub

    '####################################################
    Private Sub Insert_Ultima_Riga_Conf(ByRef DS As DS_RiepilogoConferimentoXSpecie, _
                                        ByVal Temp_Specie As String, _
                                        ByVal Temp_DescrSpecie As String, _
                                        ByVal Progr_Conferente As String, _
                                        ByVal RagSoc_Conferente As String, _
                                        ByVal Udm_Cod As Integer, _
                                        ByRef Peso_Lordo As Double, _
                                        ByRef Peso_Netto As Double, _
                                        ByRef Degrado As Double, _
                                        ByRef Netto_Pagamento As Double, _
                                        ByRef PesoNettoXDegradoPerc As Double, _
                                        ByRef PesoNettoXPunteggio As Double, _
                                        ByRef Temp_Conferente As String, _
                                        ByRef Temp_RagSocConferente As String, _
                                        ByRef Temp_UdmCod As Integer, _
                                        ByRef Degrado_Perc_Medio As Double, _
                                        ByRef Indice_Qualit_Medio As Double, _
                                        ByRef Tot_Conf_NettoXDegradoPerc As Double, _
                                        ByRef Tot_Conf_NettoXPunteggio As Double, _
                                        ByRef Tot_Conf_PesoLordo As Double, _
                                        ByRef Tot_Conf_PesoNetto As Double, _
                                        ByRef Tot_Conf_Degrado As Double, _
                                        ByRef Tot_Conf_NettoPag As Double)

        Dim DR As DS_RiepilogoConferimentoXSpecie.DS_RiepilogoConferimentoXSpecieRow

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
        DR = DS.DS_RiepilogoConferimentoXSpecie.NewDS_RiepilogoConferimentoXSpecieRow

        DR.Codice_Specie = Temp_Specie
        DR.Descr_Specie = Temp_DescrSpecie

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

        DS.DS_RiepilogoConferimentoXSpecie.Rows.Add(DR)
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
    Private Sub Insert_Ultima_Riga_Specie(ByRef DS As DS_RiepilogoConferimentoXSpecie, _
                                        ByVal Temp_Specie As String, _
                                        ByVal Temp_DescrSpecie As String, _
                                        ByVal Progr_Conferente As String, _
                                        ByVal RagSoc_Conferente As String, _
                                        ByVal Udm_Cod As Integer, _
                                        ByRef Peso_Lordo As Double, _
                                        ByRef Peso_Netto As Double, _
                                        ByRef Degrado As Double, _
                                        ByRef Netto_Pagamento As Double, _
                                        ByRef PesoNettoXDegradoPerc As Double, _
                                        ByRef PesoNettoXPunteggio As Double, _
                                        ByRef Temp_Conferente As String, _
                                        ByRef Temp_RagSocConferente As String, _
                                        ByRef Temp_UdmCod As Integer, _
                                        ByRef Degrado_Perc_Medio As Double, _
                                        ByRef Indice_Qualit_Medio As Double, _
                                        ByRef Tot_Conf_NettoXDegradoPerc As Double, _
                                        ByRef Tot_Conf_NettoXPunteggio As Double, _
                                        ByRef Tot_Conf_PesoLordo As Double, _
                                        ByRef Tot_Conf_PesoNetto As Double, _
                                        ByRef Tot_Conf_Degrado As Double, _
                                        ByRef Tot_Conf_NettoPag As Double)

        Dim DR As DS_RiepilogoConferimentoXSpecie.DS_RiepilogoConferimentoXSpecieRow

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
        DR = DS.DS_RiepilogoConferimentoXSpecie.NewDS_RiepilogoConferimentoXSpecieRow

        DR.Codice_Specie = Temp_Specie
        DR.Descr_Specie = Temp_DescrSpecie

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

        DS.DS_RiepilogoConferimentoXSpecie.Rows.Add(DR)
        '---------------------------

    End Sub




End Class
