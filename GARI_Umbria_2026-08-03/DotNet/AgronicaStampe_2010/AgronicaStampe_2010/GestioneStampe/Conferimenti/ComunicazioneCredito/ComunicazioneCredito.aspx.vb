Imports AgronicaCoreContabBIZ
Imports AgronicaCoreContabDAL
Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.Agro_Math
Imports Newtonsoft.Json
Imports AgronicaCoreUtility
Imports System.Globalization


Public Class ComunicazioneCredito
    Inherits System.Web.UI.Page

    Private _rptComunicazioneCredito As Rpt_ComunicazioneCredito
    Dim Data_Da, Data_A As String
    Dim Codice_Conferente As String
    Dim Cod_RisUm As Integer
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

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        
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

        Flag01_GestioneCodEsterno = Stringa_Decodifica(CStr(Request.QueryString("fce")),
                                AgroKey_EncoderDecoder,
                                Server)

        Mat_Des = Stringa_Decodifica(CStr(Request.QueryString("dpr")),
                                AgroKey_EncoderDecoder,
                                Server)


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

        FiltroAggConferenti = Session("Str_CodRisUm_Conferenti")


        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim Nome_Documento As String = "LettereElencoConferimenti"
        Dim Log_Errori As String = ""
        Dim DS As New DS_ComunicazioneCredito

        If Not Me.IsPostBack Then

            '-----------------------------------------
            '-------- funzione del report ------------
            '-----------------------------------------
            Try
                
                Stampa_ComunicazioneCredito(DS, Log_Errori)

            Catch ex As Exception
                Log_Errori &= "- PageLoad: " & vbCrLf & ex.Message & vbCrLf
            End Try


            '-----------------------------------------
            '----------- SetDataSource ---------------
            '-----------------------------------------
            Try
                _rptComunicazioneCredito = New Rpt_ComunicazioneCredito()
                _rptComunicazioneCredito.SetDataSource(DS)

            Catch ex As Exception
                Log_Errori &= "- Aggancio dataset: " & vbCrLf & ex.Message & vbCrLf
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

                'Rpt_RiepilogoConferimentoXArticolo.SetParameterValue("Rag_Soc", Param_Rag_Soc)

            Catch ex As Exception
                Log_Errori &= "- impostazione parametri: " & vbCrLf & ex.Message & vbCrLf
            End Try


            '-----------------------------------------
            '-------- Generazione Report -------------
            '-----------------------------------------

            'MS Eliminato passaggio report in session per giro su file: Session("Report") = rptRegCorrispettivi
            Dim reportTemporaneo As String = CrystalHelper.getFileReportTemporaneo()

            Try
                _rptComunicazioneCredito.SaveAs(reportTemporaneo, True)
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
                                "Stringa Filtro cod_Articolo = " & CStr(FiltroAggArticoli) & ", " & vbCrLf &
                                "Stringa Filtro Conferenti = " & CStr(FiltroAggConferenti) & ", " & vbCrLf &
                                "Piva = " & CStr(Piva) & ", " & vbCrLf &
                                "Sa_Cod = " & CStr(Sa_Cod) & ", " & vbCrLf &
                                                 vbCrLf & vbCrLf & vbCrLf & vbCrLf &
                                Log_Errori

                Nome_File = "Log_Errori_" + Nome_Documento

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server,
                                                 "Stampe_Conferimento",
                                                 Nome_File & ".txt",
                                                 Session("ASG_Utente_Username"),
                                                 "ComunicazioneCreditoConferenti",
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

    Private Sub Stampa_ComunicazioneCredito(ByRef DS As DS_ComunicazioneCredito, ByRef Log_Errori As String)

        Dim DT As DataTable
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
            Dim drDatiGenerali = DS.DT_ComunicazioneCredito_DatiGenerali.NewDT_ComunicazioneCredito_DatiGeneraliRow()
            DS.DT_ComunicazioneCredito_DatiGenerali.AddDT_ComunicazioneCredito_DatiGeneraliRow(drDatiGenerali)

            drDatiGenerali.LogoIntestazione = OttieniLogo()

            Dim handleImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
            Dim ragSocimpresa As String = handleImprese.RagSoc_from_Piva(Piva, objParametri_Server)

            'TODO Migliorare la gestione della firma magari creando un nuovo codice_anagrafe
            'di modo che l'utente possa indicare convenevole + nominativo della persona
            If CInt(Session("ASG_ProgressivoGIAS")) = enum_CodiceGIAS_Clienti.Fruttagel Then
                drDatiGenerali.Oggetto = "Acquisto/conferimento frutta"
                drDatiGenerali.Firma = ragSocimpresa & vbCrLf & "Il Direttore Generale" & vbCrLf & "Paolo Cristofori"
            Else
                drDatiGenerali.Oggetto = "Riepilogo Conferimenti"
                drDatiGenerali.Firma = ragSocimpresa
            End If
            
            Dim objConfFun As New AgronicaCoreStampeDAL.Conferimento_Funzioni
            FiltroAggArticoli = objConfFun.Ricava_FiltroMatCod_da_StrCodiciProdotto(FiltroAggArticoli)
            FiltroAggConferenti = objConfFun.Ricava_FiltroCodRisUm_da_StrCodiciContatto(FiltroAggConferenti)

            Dim objFF As New AgronicaCoreStampeDAL.FreshAndFood
            DT = objFF.ComunicazioneCredito(Flag01_GestioneCodEsterno,
                                                    Piva,
                                                    Sa_Cod,
                                                    Veg_Cod,
                                                    Cul_Cod,
                                                     Mat_Cod,
                                                    Codice_Conferente,
                                                    Cod_RisUm,
                                                    Data_Da,
                                                    Data_A,
                                                    FiltroAggArticoli,
                                                    FiltroAggConferenti,
                                                    "", "",
                                                    objParametri_Server)                

            If DT.Rows.Count > 0 Then

                drDatiGenerali.CentroAz_Municipio = Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(DT.Rows(0)("CentroAz_Municipio").ToString.ToLower)
                drDatiGenerali.CentroAz_Provincia = DT.Rows(0)("CentroAz_Provincia")

                For i = 0 To DT.Rows.Count - 1

                    Dim DR = DS.DT_ComunicazioneCredito.NewDT_ComunicazioneCreditoRow()

                    Dim rowDB = DT.Rows(i)

                    DR.Conferente_Cod_Risum = rowDB.Field(Of Integer)("Cod_RisUm_Conferente")
                    DR.Conferente_SettoreDes = rowDB.Field(Of String)("Codice_Conferente")
                    DR.Conferente_Rag_Soc = rowDB.Field(Of String)("Rag_Soc_Conferente")
                    DR.Conferente_Mail = rowDB.Field(Of String)("Conferente_Referente_Mail")
                    DR.Conferente_Referente = rowDB.Field(Of String)("Conferente_Referente")
                    DR.Conferente_ModalitaPagamento = rowDB.Field(Of String)("Cau_Pagamento_Des")

                    Dim vegDes = rowDB.Field(Of String)("Veg_Des")
                    Dim matDes = rowDB.Field(Of String)("Mat_Des")

                    If Flag01_GestioneCodEsterno = 1 Then
                    
                        DR.Prodotto_Ide_Gruppo = rowDB.Field(Of String)("Codice_Esterno")

                        If CInt(Session("ASG_ProgressivoGIAS")) = enum_CodiceGIAS_Clienti.Fruttagel Then

                            DR.Prodotto_Des_Gruppo = "Cod. Sp. " & DR.Prodotto_Ide_Gruppo
                            DR.Prodotto_Descrizione = ""
                            objConfFun.Ricava_Specie_Varieta_FRG(matDes, DR.Prodotto_Descrizione, Nothing)

                        Else
                        
                            DR.Prodotto_Des_Gruppo = "Cod. " & DR.Prodotto_Ide_Gruppo
                            DR.Prodotto_Descrizione = vegDes & " - " & matDes

                        End If

                    Else

                        DR.Prodotto_Ide_Gruppo = rowDB.Field(Of String)("Cod_Articolo")
                        DR.Prodotto_Des_Gruppo = "Cod. Articolo " & DR.Prodotto_Ide_Gruppo
                        DR.Prodotto_Descrizione = vegDes & " - " & matDes

                    End If

                    DR.Veg_Des = vegDes
                    DR.Cul_Des = rowDB.Field(Of String)("Cul_Des")
                    DR.Accettazione_Numero = rowDB.Field(Of String)("Accettazione_NumeroVis")
                    DR.Accettazione_Data = rowDB.Field(Of Date)("Accettazione_Data")
                    DR.DDTConferente_Numero = rowDB.Field(Of String)("DDTConferente_NumeroVis")
                    DR.DDTConferente_Data = rowDB.Field(Of Date)("DDTConferente_Data")
                    DR.Qta = rowDB.Field(Of Integer)("Netto_Pagamento")
                    DR.Prezzo = rowDB.Field(Of Double)("Prezzo_Effettivo")
                    DR.Produttore_Rag_Soc = If(Not IsDBNull(rowDB("Rag_Soc_Produttore")), "Produttore: " & rowDB("Rag_Soc_Produttore"), "")
                    DS.DT_ComunicazioneCredito.AddDT_ComunicazioneCreditoRow(DR)
                Next

            End If

        Catch ex As Exception
            Log_Errori += "- Lettura dei dati: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try


    End Sub

    Private Function OttieniLogo() As Byte()

        'di default inserisco un'immagine bianca come logo
        Dim bmpImg As New Drawing.Bitmap(1, 1)
        bmpImg.SetPixel(0, 0, Drawing.Color.White)

        Dim cx As New Drawing.ImageConverter
        Dim logoBianco() As Byte
        logoBianco = cx.ConvertTo(bmpImg, GetType(Byte()))

        Dim byteLogo As Byte() = logoBianco

        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig

        If objWebConfig.Path_Directory_Loghi_Cliente <> "" Then

            objWebConfig.Path_Directory_Loghi_Cliente = FileSystemHelper.AggiungiSlashSeNonEsiste(objWebConfig.Path_Directory_Loghi_Cliente)

            Dim path = objWebConfig.Path_Directory_Loghi_Cliente & objParametri_Server.PivaSuperUser & "_comunicazionecredito.jpg"

            If IO.File.Exists(path) = True Then
                byteLogo = My.Computer.FileSystem.ReadAllBytes(path)
            End If

        End If

        Return byteLogo

    End Function

End Class