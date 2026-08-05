Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza

Public Class Registri_IVA_3
    Inherits System.Web.UI.Page

    Private rptRegistriIVA As Rpt_RegistriIVA_2
    Private rptSottoReportIva_Acquisti As Rpt_SottoReport_IVA
    Private rptSottoReportIva_Vendite As Rpt_SottoReportIVA_Duplicato

    Private Log_Errori As String
    Dim Ordinamento As enum_RegistriIva_Ordinamento
    Dim Report As Integer
    Dim Piva As String
    Dim Str_Trimestri As String
    Dim Anno As String
    Dim Sezionale_Cod As Integer
    Dim EsigibilitaIva As Integer
    Dim Da_Mese As String
    Dim A_Mese As String
    Dim Tipologia As Integer
    Dim Log As String = ""
    Dim Sezionale_Des As String
    'Dim Data_Inizio As String
    'Dim Data_Fine As String
    'Dim Anno As Integer
    'Dim Cod_Rapporto As Integer
    'Dim Cod_RisUm As Integer
    'Dim Tipo_Scadenza As Integer
    'Dim Scadenza As String
    Dim Validita_Inizio, Validita_Fine As String

    Dim Param_Rag_Soc As String = ""
    Dim Param_Piva_CodFiscale As String = ""
    Dim Param_Indirizzo As String = ""
    Dim Param_NumPagina As Integer = 0
    Dim Param_Flag_Stampa_Data As Boolean

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri

#Region " Registri IVA "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_AbortTransaction(sender As Object, e As System.EventArgs) Handles Me.AbortTransaction

    End Sub

  

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()

        rptRegistriIVA = New Rpt_RegistriIVA_2
        rptSottoReportIva_Acquisti = New Rpt_SottoReport_IVA
        rptSottoReportIva_Vendite = New Rpt_SottoReportIVA_Duplicato

    End Sub

#End Region

    '##################################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Piva = Stringa_Decodifica(CStr(Request.QueryString("p")), _
                            AgroKey_EncoderDecoder, _
                            Server)

        Report = CInt(Stringa_Decodifica(CStr(Request.QueryString("r")), _
                                        AgroKey_EncoderDecoder, _
                                        Server))

        '0=mensile 1=trimestrale
        Tipologia = Stringa_Decodifica(CStr(Request.QueryString("tipo")), _
                                        AgroKey_EncoderDecoder, _
                                        Server)

        'stringa, separata da |, contenente la scelta sui trimestri (0=NO, 1=SI)
        'esempio: "1|1|1|0" -> selezionati trimestri I,II,III, non selezionato: IV
        Str_Trimestri = Stringa_Decodifica(CStr(Request.QueryString("tri")), _
                                            AgroKey_EncoderDecoder, _
                                            Server)

        Da_Mese = Stringa_Decodifica(CStr(Request.QueryString("dm")), _
                                    AgroKey_EncoderDecoder, _
                                    Server)

        A_Mese = Stringa_Decodifica(CStr(Request.QueryString("am")), _
                                    AgroKey_EncoderDecoder, _
                                    Server)

        Anno = CInt(Stringa_Decodifica(CStr(Request.QueryString("a")), _
                                    AgroKey_EncoderDecoder, _
                                    Server))

        Sezionale_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("szc")), _
                             AgroKey_EncoderDecoder, _
                             Server))

        Sezionale_Des = Stringa_Decodifica(CStr(Request.QueryString("szd")), _
                    AgroKey_EncoderDecoder, _
                    Server)

        Param_NumPagina = CInt(Stringa_Decodifica(CStr(Request.QueryString("np")), _
                            AgroKey_EncoderDecoder, _
                            Server))

        Param_Flag_Stampa_Data = CBool(Stringa_Decodifica(CStr(Request.QueryString("fsd")), _
                      AgroKey_EncoderDecoder, _
                      Server))

        Ordinamento = Stringa_Decodifica(CStr(Request.QueryString("ord")), _
                               AgroKey_EncoderDecoder, _
                               Server)

        Param_Rag_Soc = Stringa_Decodifica(CStr(Request.QueryString("pmr")), _
                     AgroKey_EncoderDecoder, _
                     Server)

        Param_Piva_CodFiscale = Stringa_Decodifica(CStr(Request.QueryString("pmp")), _
                               AgroKey_EncoderDecoder, _
                               Server)

        Param_Indirizzo = Stringa_Decodifica(CStr(Request.QueryString("pmi")), _
                               AgroKey_EncoderDecoder, _
                               Server)

        'Data_Inizio = Stringa_Decodifica(CStr(Request.QueryString("di")), _
        '                            AgroKey_EncoderDecoder, _
        '                            Server)

        'Data_Fine = Stringa_Decodifica(CStr(Request.QueryString("df")), _
        '                            AgroKey_EncoderDecoder, _
        '                            Server)

        'Cod_RisUm = Stringa_Decodifica(CStr(Request.QueryString("cru")), _
        '                          AgroKey_EncoderDecoder, _
        '                          Server)

        'Cod_Rapporto = Stringa_Decodifica(CStr(Request.QueryString("cr")), _
        '                        AgroKey_EncoderDecoder, _
        '                        Server)

        'Tipo_Scadenza = Stringa_Decodifica(CStr(Request.QueryString("tsc")), _
        '                        AgroKey_EncoderDecoder, _
        '                        Server)

        'Scadenza = Stringa_Decodifica(CStr(Request.QueryString("sc")), _
        '                        AgroKey_EncoderDecoder, _
        '                        Server)

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))


        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        '   Dim CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer

        Dim Nome_Documento As String = ""
        Dim IdentificazioneDocumento As String = ""
        Dim cat_cod As enum_CategorieDocumenti

        Select Case Report
            Case enum_CodificaStampe.Registro_FattureAcquisto
                Nome_Documento = "Registro_Acquisti"
                cat_cod = enum_CategorieDocumenti.RegistriIVA_Acquisti
            Case enum_CodificaStampe.Registro_FattureVendita
                Nome_Documento = "Registro_Vendite"
                cat_cod = enum_CategorieDocumenti.RegistriIVA_Vendite
        End Select

        If Not Me.IsPostBack Then

            'CrystalReportViewer1 = New CrystalDecisions.Web.CrystalReportViewer
            'CrystalReportViewer1.Style.Add("LEFT", "-275px")
            'CrystalReportViewer1.Style.Add("TOP", "0px")
            'CrystalReportViewer1.Style.Add("POSITION", "Absolute")
            'Me.FindControl("Form1").Controls.Add(CrystalReportViewer1)

            Dim DSRegistriIVA As New DS_RegistriIVA
            ' Dim DSSottoReportIVA As New DS_SottoReport_IVA

            'Try
            '    Dim DT_Intestazione As DataTable
            '    Dim str_riga1, str_riga2 As String
            '    Dim objInt As New AgronicaCoreStampeDAL.DocContab
            '    DT_Intestazione = objInt.DatiIntestazioneImpresa(Piva, Log, "", "", objParametri_Server)

            '    If Log = "" Then
            '        str_riga1 = "P.IVA: " & DT_Intestazione.Rows(0).Item("Piva") & _
            '                    " Cod. Fiscale: " & DT_Intestazione.Rows(0).Item("Codice_Fiscale") & _
            '                    "   " & DT_Intestazione.Rows(0).Item("rag_soc")

            '        str_riga2 = DT_Intestazione.Rows(0).Item("ind_impresa") & " " & DT_Intestazione.Rows(0).Item("CAP") + " " + DT_Intestazione.Rows(0).Item("frz_des") + " - " + DT_Intestazione.Rows(0).Item("LOCALITA") + " (" + DT_Intestazione.Rows(0).Item("COMUNI_PROV") + ") "

            '        If Not IsNothing(DT_Intestazione) AndAlso DT_Intestazione.Rows.Count <> 0 Then
            '            CType(rptRegistriIVA.Section1.ReportObjects("TxtRiga1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = str_riga1
            '        End If

            '    End If

            '    If Sezionale_Cod > 0 Then
            '        str_riga2 += " Sezionale: " & CStr(Sezionale_Des)
            '    End If
            '    CType(rptRegistriIVA.Section1.ReportObjects("TxtRiga2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = str_riga2

            'Catch ex As Exception
            '    Log_Errori += "- Intestazione: " + vbCrLf + ex.Message + vbCrLf
            'End Try

            Try
                Dim objSez As New AgronicaCoreContabDAL.Imprese_Sezionali_R
                EsigibilitaIva = objSez.EsigibilitaIva_from_SezionaleCod(Piva, Sezionale_Cod, objParametri_Server)

                Stampa_Registri_IVA(rptRegistriIVA, DSRegistriIVA)

            Catch exc As Exception
                Log_Errori &= "- PageLoad: " & vbCrLf & exc.Message & vbCrLf
            End Try

            Dim nomeFilePdf As String = ""

            Try

                Dim Data_Inizio_Allegati, Data_Fine_Allegati As Date

                Data_Inizio_Allegati = Validita_Inizio
                Data_Fine_Allegati = Validita_Fine
                IdentificazioneDocumento &= "_" & Format(Data_Inizio_Allegati, "yyyy_MM_dd") & "_" & Format(Data_Fine_Allegati, "yyyy_MM_dd")
                nomeFilePdf = Nome_Documento & "_p" & Piva & "_" & IdentificazioneDocumento & ".pdf"

                ' leggo la sottocartella da CategorieDocumenti
                Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                Dim sottoCartella As String = objCatDoc.Sottocartella(cat_cod, "", "", objParametri_Server)

                ' salvo il report in formato PDF
                Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                objGestFile.SalvaReportPdf(rptRegistriIVA,
                                           cat_cod, sottoCartella, nomeFilePdf,
                                           objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
                Dim allegatiDocumentiCod As Integer = objAllegati.SalvaAllegato(Piva,
                                                                                cat_cod,
                                                                                Nome_Documento,
                                                                                nomeFilePdf,
                                                                                sottoCartella,
                                                                                "", "", "", "",
                                                                                Data_Inizio_Allegati,
                                                                                Data_Fine_Allegati,
                                                                                objParametri_Server)


            Catch ex As Exception
                Log_Errori &= "- Gestione allegati: " & vbCrLf & ex.Message & vbCrLf
            End Try


            'MS Eliminato passaggio report in session per giro su file: Session("Report") = rptRegCorrispettivi
            Dim reportTemporaneo As String = CrystalHelper.getFileReportTemporaneo()
            Try
                rptRegistriIVA.SaveAs(reportTemporaneo, True)
            Catch ex As Exception
                Log_Errori &= "- Salvataggio report temporaneo: " & vbCrLf & ex.Message & vbCrLf
            End Try

            'MS Dispose dei dataset e del report per evitare problema deallocazione.
            DSRegistriIVA.Dispose()
            DSRegistriIVA = Nothing

            rptSottoReportIva_Acquisti.Close()
            rptSottoReportIva_Acquisti.Dispose()
            rptSottoReportIva_Acquisti = Nothing

            rptSottoReportIva_Vendite.Close()
            rptSottoReportIva_Vendite.Dispose()
            rptSottoReportIva_Vendite = Nothing

            rptRegistriIVA.Close()
            rptRegistriIVA.Dispose()
            rptRegistriIVA = Nothing

            GC.Collect()

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento & ", Partita Iva = " & CStr(Piva) &
                             vbCrLf & vbCrLf & Log_Errori

                Dim Nome_File As String = "Log_Errori_" & Nome_Documento

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server,
                                                 "Stampe_Contabilita",
                                                 Nome_File & ".txt",
                                                 Session("ASG_Utente_Username"),
                                                 "Registri_IVA_3.aspx",
                                                 Log_Errori)

            End If
            '-----------------------------------------

            Dim PDF As String = Stringa_Decodifica(CStr(Request.QueryString("PDF")), AgroKey_EncoderDecoder)

            If PDF = "1" Then
                Response.Redirect("..\..\VisualizzatoreReport.aspx?tmpReportPath=" & Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server) &
                                  "&NomePdf=" & Stringa_Codifica(nomeFilePdf, AgroKey_EncoderDecoder, Server))
            Else
                Response.Redirect("..\..\VisualizzatoreReport.aspx?ForzaAnteprima=" & Stringa_Codifica("true", AgroKey_EncoderDecoder, Server) &
                                  "&tmpReportPath=" & Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server))
            End If

        End If

        '==================================================================

    End Sub


    '#####################################################################################################
    Private Sub Stampa_Registri_IVA(ByRef rptRegistriIVA As Rpt_RegistriIVA_2, _
                                    ByRef DSRegistriIVA As DS_RegistriIVA _
                                    )

        Dim DSSottoReportIVA_Acq As New DS_SottoReport_IVA
        Dim DSSottoReportIVA_Vend As New DS_SottoReportIVA_Duplicato

        Dim objContabHLP As New AgronicaCoreContabHLP.Contabilita
        Dim DT_Query As DataTable
        Dim i, Lav_Cod As Integer
        Dim Flag_Vendita As Boolean
        Dim Num_Protocollo As Double = 0

        Dim Tot_Imponibile_Intracom As Double = 0
        Dim Tot_IVA_Intracom As Double = 0
        Dim Tot_Imponibile As Double = 0
        Dim Tot_IVA As Double = 0
        Dim Tot_Importo As Double = 0

        '########################################################################

        Dim Param_Titolo As String = ""
        Dim Param_PercIva As String = ""
        Dim Param_PivaContatto As String = ""
        Dim Param_RagSocContatto As String = ""

        Dim Param_AnnoMeseTrim As String = "Anno " & Anno

        Dim Param_TotaleImponibileRDist As String = ""
        Dim Param_TotaleIVARDist As String = ""
        Dim Param_TotaleImponibile As String = ""
        Dim Param_TotaleIVA As String = ""
        Dim Param_TotaleImporto As String = ""

        Select Case Report

            Case enum_CodificaStampe.Registro_FattureVendita

                Flag_Vendita = True

                Param_Titolo = "Registro Vendite"
                Param_PercIva = "% IVA Comp."
                Param_PivaContatto = "Piva/C.F. Cliente"
                Param_RagSocContatto = "Ragione Sociale Cliente"

                'CType(rptRegistriIVA.Section1.ReportObjects("TxtTitolo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Registro Vendite"
                'CType(rptRegistriIVA.Section2.ReportObjects("TxtPercIva"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "% IVA Comp."
                'CType(rptRegistriIVA.Section2.ReportObjects("TxtPivaContatto"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Piva/C.F. Cliente"
                'CType(rptRegistriIVA.Section2.ReportObjects("TxtRagSocContatto"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Ragione Sociale Cliente"

            Case enum_CodificaStampe.Registro_FattureAcquisto

                Flag_Vendita = False

                Param_Titolo = "Registro Acquisti"
                Param_PercIva = "% IVA Indetr."
                Param_PivaContatto = "Piva/C.F. Fornitore"
                Param_RagSocContatto = "Ragione Sociale Fornitore"

                'CType(rptRegistriIVA.Section1.ReportObjects("TxtTitolo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Registro Acquisti"
                'CType(rptRegistriIVA.Section2.ReportObjects("TxtPercIva"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "% IVA Indetr."
                'CType(rptRegistriIVA.Section2.ReportObjects("TxtPivaContatto"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Piva/C.F. Fornitore"
                'CType(rptRegistriIVA.Section2.ReportObjects("TxtRagSocContatto"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Ragione Sociale Fornitore"

        End Select

        '########################################################################
        '########################################################################

        Try

            Dim MeseTrim As String
            'Dim TxtAnno As String = "ANNO " & Anno
            Dim Titolo_Sottoreport As String = "RIEPILOGO IVA - ANNO " & Anno

            '0=mensile 1=trimestrale
            Select Case Tipologia

                Case 0

                    MeseTrim = "Mese " & Da_Mese
                    If Da_Mese <> A_Mese Then
                        MeseTrim += " - " & A_Mese
                    End If

                    Validita_Inizio = "01/" & Da_Mese & "/" & Anno

                    Select Case A_Mese
                        Case "11", "04", "06", "09"
                            Validita_Fine = "30/" & A_Mese & "/" & Anno
                        Case "02"
                            If Date.IsLeapYear(Anno) = False Then
                                Validita_Fine = "28/02/" & Anno
                            Else
                                Validita_Fine = "29/02/" & Anno
                            End If
                        Case Else
                            Validita_Fine = "31/" & A_Mese & "/" & Anno
                    End Select

                Case 1

                    'stringa, separata da |, contenente la scelta sui trimestri (0=NO, 1=SI)
                    'esempio: "1|1|1|0" -> selezionati trimestri I,II,III, non selezionato: IV
                    If Str_Trimestri.Split("|")(0) = 1 Then
                        'trimestre I: gen feb marzo
                        Validita_Inizio = "01/01/" & Anno
                        Validita_Fine = "31/03/" & Anno
                        MeseTrim = "TRIMESTRE I"
                    End If
                    If Str_Trimestri.Split("|")(1) = 1 Then
                        'trimestre II: apr mag giu
                        If Validita_Inizio = "" Then
                            Validita_Inizio = "01/04/" & Anno
                        End If
                        Validita_Fine = "30/06/" & Anno
                        MeseTrim = "TRIMESTRE II"
                    End If
                    If Str_Trimestri.Split("|")(2) = 1 Then
                        'trimestre III: lug ago sett
                        If Validita_Inizio = "" Then
                            Validita_Inizio = "01/07/" & Anno
                        End If
                        Validita_Fine = "30/09/" & Anno
                        MeseTrim = "TRIMESTRE III"
                    End If
                    If Str_Trimestri.Split("|")(3) = 1 Then
                        'trimestre IV: ott nov dic
                        If Validita_Inizio = "" Then
                            Validita_Inizio = "01/10/" & Anno
                        End If
                        Validita_Fine = "31/12/" & Anno
                        MeseTrim = "TRIMESTRE IV"
                    End If

            End Select


            Titolo_Sottoreport &= " - " & MeseTrim
            Param_AnnoMeseTrim &= " - " & MeseTrim

            'CType(rptRegistriIVA.Section1.ReportObjects("TxtAnno"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = TxtAnno
            'CType(rptRegistriIVA.Section1.ReportObjects("TxtMeseTrim"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = MeseTrim


            Dim objStampe As New AgronicaCoreStampeDAL.RegistriContab
            Dim debug As Boolean = True
            Dim objLanRound As New AgronicaCoreContabHLP.GiasLan_Round
            Dim objSottoIva As New cls_SottoReport_IVA
            Dim id_agenda_memo As Integer = 0
            Dim id_agenda As Integer

            Dim DT_Dettagli_Round As DataTable
            Dim DT_IVA_Round As DataTable
            Dim DT_IVA_Generale As DataTable
            Dim DtIvaIndetComp As DataTable 'non è usato, ma lo tengo perchè è passato a delle funzioni


            'DT_Query = objStampe.RegistriIVA_2(Piva, _
            '                                    Sezionale_Cod, _
            '                                    Flag_Vendita, _
            '                                    Validita_Inizio, _
            '                                    Validita_Fine, _
            '                                    "", "", _
            '                                    Ordinamento, _
            '                                    objParametri_Server)

            DT_Query = objStampe.RegistriIVA_3(Piva, _
                                    Sezionale_Cod, _
                                    Flag_Vendita, _
                                    Validita_Inizio, _
                                    Validita_Fine, _
                                    "", "", _
                                    Ordinamento, _
                                    objParametri_Server)

            'non mi salvo gli id_agenda in un hashtable perchè altrimenti viene perso l'ordinamento

            If Not IsNothing(DT_Query) AndAlso DT_Query.Rows.Count > 0 Then

                ' DtIvaIndetComp = objLanRound.CaricaGriglia_DtIvaIndetCompens

                DT_IVA_Generale = objSottoIva.CaricaGriglia_DtIvaGenerale()

                Try
                    DT_Dettagli_Round = objLanRound.CaricaGriglia_DtDettagli

                Catch ex As Exception
                    Throw New Exception("DT_Dettagli_Round, errore nella creazione: " & vbCrLf & ex.Message)
                End Try

                Try
                    DT_IVA_Round = objLanRound.CaricaGriglia_DtIva_LiqIVA

                Catch ex As Exception
                    Throw New Exception("DT_IVA_Round, errore nella creazione: " & vbCrLf & ex.Message)
                End Try

                For i = 0 To DT_Query.Rows.Count - 1

                    id_agenda = DT_Query.Rows(i).Item("id_agenda")

                    Lav_Cod = DT_Query.Rows(i).Item("Lav_Cod")

                    Num_Protocollo = DT_Query.Rows(i).Item("Num_Protocollo")

                    ''x ogni riga x ogni documento
                    'objLanRound.InserisciRiga_DtIvaIndetCompens(DtIvaIndetComp, _
                    '                                            id_agenda, _
                    '                                            DT_Query.Rows(i).Item("imponibile"), _
                    '                                            DT_Query.Rows(i).Item("imponibile_netto"), _
                    '                                            DT_Query.Rows(i).Item("cod_iva"), _
                    '                                            DT_Query.Rows(i).Item("iva"), _
                    '                                            DT_Query.Rows(i).Item("aliquota"), _
                    '                                            DT_Query.Rows(i).Item("Sigla_IVA"), _
                    '                                            DT_Query.Rows(i).Item("Iva_Indetraibile"), _
                    '                                            DT_Query.Rows(i).Item("Iva_Indetraibile_Perc"))


                    '02/02/2018: 
                    'prima faceva -1 * prezzo per le note di accredito perchè dovevano risultare negative per essere sottratte al totale
                    'per le ndc ora bisogna moltiplicare *-1 anche imponibile e iva visto che si leggono da db e non si ricalcolano
                    Select Case Lav_Cod
                        Case LAVCOD_NOTA_ACCREDITO_RICEVUTA, LAVCOD_NOTA_ACCREDITO_EMESSA
                            'le note di accredito vanno sottratte
                            DT_Query.Rows(i).Item("Prezzo_Unitario") = -1 * DT_Query.Rows(i).Item("Prezzo_Unitario")
                            DT_Query.Rows(i).Item("Prezzo_Unitario_Netto") = -1 * DT_Query.Rows(i).Item("Prezzo_Unitario_Netto")

                            DT_Query.Rows(i).Item("Imponibile") = objContabHLP.Leggi_Imponibile_PositivoNegativo2(DT_Query.Rows(i).Item("Lav_Cod"), DT_Query.Rows(i).Item("Imponibile"))
                            DT_Query.Rows(i).Item("Imponibile_Netto") = objContabHLP.Leggi_Imponibile_PositivoNegativo2(DT_Query.Rows(i).Item("Lav_Cod"), DT_Query.Rows(i).Item("Imponibile_Netto"))
                            DT_Query.Rows(i).Item("Iva") = objContabHLP.Leggi_IVA_PositivaNegativa2(DT_Query.Rows(i).Item("Lav_Cod"), DT_Query.Rows(i).Item("Iva"))
                            DT_Query.Rows(i).Item("Iva_Indetraibile") = objContabHLP.Leggi_IVAIndet_PositivaNegativa2(DT_Query.Rows(i).Item("Lav_Cod"), DT_Query.Rows(i).Item("Iva_Indetraibile"))

                        Case Else
                            DT_Query.Rows(i).Item("Imponibile") = objContabHLP.Leggi_Imponibile_PositivoNegativo(DT_Query.Rows(i).Item("Lav_Cod"), DT_Query.Rows(i).Item("Imponibile"))
                            DT_Query.Rows(i).Item("Imponibile_Netto") = objContabHLP.Leggi_Imponibile_PositivoNegativo(DT_Query.Rows(i).Item("Lav_Cod"), DT_Query.Rows(i).Item("Imponibile_Netto"))
                            DT_Query.Rows(i).Item("Iva") = objContabHLP.Leggi_IVA_PositivaNegativa(DT_Query.Rows(i).Item("Lav_Cod"), DT_Query.Rows(i).Item("Iva"))
                            DT_Query.Rows(i).Item("Iva_Indetraibile") = objContabHLP.Leggi_IVAIndet_PositivaNegativa(DT_Query.Rows(i).Item("Lav_Cod"), DT_Query.Rows(i).Item("Iva_Indetraibile"))
                    End Select

                    If id_agenda <> id_agenda_memo Then
                        'nuova operazione

                        'se non sono al primo giro
                        'devo elaborare il gruppo di dettagli dell'operazione precedente
                        'e inserire le righe nel dataset
                        If id_agenda_memo <> 0 Then
                            ElaboraDettagli_x_Riepilogo_IVA(objLanRound, _
                                                            DT_Dettagli_Round, _
                                                            DT_IVA_Round, _
                                                            DT_Query.Rows(i - 1).Item("Num_Protocollo"), _
                                                            DT_Query.Rows(i - 1).Item("lav_cod"), _
                                                            Flag_Vendita, _
                                                            DT_Query.Rows(i - 1).Item("tipo_sconto"))

                            objSottoIva.DtIVAGenerale_ValorizzaDa_DtRiepilogoIVA(DT_IVA_Round, DT_IVA_Generale, Not (Flag_Vendita))

                            Dataset_InserisciRighe(objContabHLP, _
                                                    objLanRound, _
                                                    DSRegistriIVA, _
                                                    DT_Query, _
                                                    DT_IVA_Round, _
                                                    DtIvaIndetComp, _
                                                    i - 1, _
                                                    Flag_Vendita, _
                                                    Tot_Imponibile_Intracom, _
                                                    Tot_IVA_Intracom, _
                                                    Tot_Imponibile, _
                                                    Tot_IVA, _
                                                    Tot_Importo)

                        End If

                        'ad ogni operazione creo un nuovo dt_dettagli
                        DT_Dettagli_Round.Clear()
                        'svuoto anche dt_iva che va popolato al termine
                        DT_IVA_Round.Clear()

                        If DT_Dettagli_Round.Rows.Count <> 0 Then
                            Throw New Exception("DT_Dettagli_Round.Rows.Count <> 0 --> non deve succedere, controllo ")
                        End If
                        If DT_IVA_Round.Rows.Count <> 0 Then
                            Throw New Exception("DT_IVA_Round.Rows.Count <> 0 --> non deve succedere, controllo ")
                        End If

                        'salvo l'id_agenda
                        id_agenda_memo = id_agenda

                        DtDettRound_InserisciDettaglio(objContabHLP, _
                                                            objLanRound, _
                                                            Lav_Cod, _
                                                            DT_Dettagli_Round, _
                                                           DT_Query.Rows(i))

                    Else
                        'stessa operazione
                        DtDettRound_InserisciDettaglio(objContabHLP, _
                                                        objLanRound, _
                                                        Lav_Cod, _
                                                        DT_Dettagli_Round, _
                                                        DT_Query.Rows(i))

                    End If


                Next 'dt_query()

                'per l'ultima operazione
                ElaboraDettagli_x_Riepilogo_IVA(objLanRound, _
                                                DT_Dettagli_Round, _
                                                DT_IVA_Round, _
                                                DT_Query.Rows(DT_Query.Rows.Count - 1).Item("Num_Protocollo"), _
                                                Lav_Cod, _
                                                Flag_Vendita, _
                                                DT_Query.Rows(DT_Query.Rows.Count - 1).Item("tipo_sconto"))

                objSottoIva.DtIVAGenerale_ValorizzaDa_DtRiepilogoIVA(DT_IVA_Round, DT_IVA_Generale, Not (Flag_Vendita))

                Dataset_InserisciRighe(objContabHLP, _
                                         objLanRound, _
                                         DSRegistriIVA, _
                                         DT_Query, _
                                         DT_IVA_Round, _
                                         DtIvaIndetComp, _
                                         DT_Query.Rows.Count - 1, _
                                         Flag_Vendita, _
                                          Tot_Imponibile_Intracom, _
                                        Tot_IVA_Intracom, _
                                        Tot_Imponibile, _
                                        Tot_IVA, _
                                        Tot_Importo)

                If Flag_Vendita = True Then
                    'VENDITE
                    objSottoIva.Carica_DSSottoReportIVA_daDtIVAGenerale_VENDITE_NEW(DSSottoReportIVA_Vend, DT_IVA_Generale, Titolo_Sottoreport, objParametri_Server)
                    objSottoIva.Carica_DSSottoReportIVA_vuoto_VENDITE(DSSottoReportIVA_Acq)
                Else
                    'ACQUISTI
                    objSottoIva.Carica_DSSottoReportIVA_daDtIVAGenerale_ACQUISTI_NEW(DSSottoReportIVA_Acq, DT_IVA_Generale, Titolo_Sottoreport)
                    objSottoIva.Carica_DSSottoReportIVA_duplicato_vuoto(DSSottoReportIVA_Vend)
                End If

            Else
                'non ci sono dati
                debug = True
            End If 'dt_query

            '  Totale_Globale_Importo = Arrotonda_2Decimali(Totale_Globale_Imponibile + Totale_Globale_IVA)

            'CType(rptRegistriIVA.Section4.ReportObjects("TxtTotaleImponibileRDist"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Tot_Imponibile_Intracom, "##,###,##0.00")
            'CType(rptRegistriIVA.Section4.ReportObjects("TxtTotaleIVARDist"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Tot_IVA_Intracom, "##,###,##0.00")

            ' CType(rptRegistriIVA.Section4.ReportObjects("TxtTotaleImponibile"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Tot_Imponibile, "##,###,##0.00")
            'CType(rptRegistriIVA.Section4.ReportObjects("TxtTotaleIVA"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Tot_IVA, "##,###,##0.00")
            'CType(rptRegistriIVA.Section4.ReportObjects("TxtTotaleImporto"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Tot_Importo, "##,###,##0.00")

            Param_TotaleImponibileRDist = Format(Tot_Imponibile_Intracom, "##,###,##0.00")
            Param_TotaleIVARDist = Format(Tot_IVA_Intracom, "##,###,##0.00")
            Param_TotaleImponibile = Format(Tot_Imponibile, "##,###,##0.00")
            Param_TotaleIVA = Format(Tot_IVA, "##,###,##0.00")
            Param_TotaleImporto = Format(Tot_Importo, "##,###,##0.00")

        Catch ex As Exception
            Log_Errori &= "- Lettura delle Fatture: " & vbCrLf & ex.Message & vbCrLf
        End Try


        Try
            'imposto il dataset sul report
            rptRegistriIVA.SetDataSource(DSRegistriIVA)
        Catch ex As Exception
            Log_Errori &= "- SetDataSource: " & vbCrLf & ex.Message & vbCrLf
        End Try

        Try
            'imposto il dataset sul sottoreport
            If Flag_Vendita = True Then
                'VENDITE
                ' rptSottoReportIva_Vendite.SetDataSource(DSSottoReportIVA_Vend)
                rptRegistriIVA.ReportFooterSection1.SectionFormat.EnableSuppress = True
            Else
                'ACQUISTI
                '                rptSottoReportIva_Acquisti.SetDataSource(DSSottoReportIVA_Acq)
                rptRegistriIVA.ReportFooterSection2.SectionFormat.EnableSuppress = True
            End If

            rptSottoReportIva_Vendite.SetDataSource(DSSottoReportIVA_Vend)
            rptSottoReportIva_Acquisti.SetDataSource(DSSottoReportIVA_Acq)

        Catch ex As Exception
            Log_Errori &= "- SetDataSource sottoreport: " & vbCrLf & ex.Message & vbCrLf
        End Try

        Try
            'imposto il sottoreport sul report
            'If Flag_Vendita = True Then
            '    'VENDITE
            '    rptRegistriIVA.OpenSubreport("Rpt_SottoReportIVA_Duplicato.rpt").SetDataSource(DSSottoReportIVA_Vend)
            'Else
            '    'ACQUISTI
            '    rptRegistriIVA.OpenSubreport("Rpt_SottoReport_IVA.rpt").SetDataSource(DSSottoReportIVA_Acq)
            'End If
            rptRegistriIVA.OpenSubreport("Rpt_SottoReportIVA_Duplicato.rpt").SetDataSource(DSSottoReportIVA_Vend)
            rptRegistriIVA.OpenSubreport("Rpt_SottoReport_IVA.rpt").SetDataSource(DSSottoReportIVA_Acq)

        Catch ex As Exception
            Log_Errori &= "- OpenSubreport: " & vbCrLf & ex.Message & vbCrLf
        End Try

        '##################################

        Try

           rptRegistriIVA.SetParameterValue("Sezionale", Sezionale_Des)
            rptRegistriIVA.SetParameterValue("Rag_Soc", Param_Rag_Soc)
            rptRegistriIVA.SetParameterValue("Piva_CodFiscale", Param_Piva_CodFiscale)
            rptRegistriIVA.SetParameterValue("Indirizzo", Param_Indirizzo)

            rptRegistriIVA.SetParameterValue("Titolo", Param_Titolo)
            rptRegistriIVA.SetParameterValue("PercIva", Param_PercIva)
            rptRegistriIVA.SetParameterValue("PivaContatto", Param_PivaContatto)
            rptRegistriIVA.SetParameterValue("RagSocContatto", Param_RagSocContatto)
            rptRegistriIVA.SetParameterValue("AnnoMeseTrim", Param_AnnoMeseTrim)

            rptRegistriIVA.SetParameterValue("Num_Pagina", Param_NumPagina)
            rptRegistriIVA.SetParameterValue("Flag_Stampa_Data", Param_Flag_Stampa_Data)

            rptRegistriIVA.SetParameterValue("TotaleImponibileRDist", Param_TotaleImponibileRDist)
            rptRegistriIVA.SetParameterValue("TotaleIVARDist", Param_TotaleIVARDist)
            rptRegistriIVA.SetParameterValue("TotaleImponibile", Param_TotaleImponibile)
            rptRegistriIVA.SetParameterValue("TotaleIVA", Param_TotaleIVA)
            rptRegistriIVA.SetParameterValue("TotaleImporto", Param_TotaleImporto)

        Catch ex As Exception
            Log_Errori &= "- impostazione parametri: " & vbCrLf & ex.Message & vbCrLf
        End Try



    End Sub


    '########################################################################
    Private Sub DtDettRound_InserisciDettaglio(ByRef objContabHLP As AgronicaCoreContabHLP.Contabilita, _
                                                    ByRef objLanRound As AgronicaCoreContabHLP.GiasLan_Round, _
                                                    ByVal Lav_Cod As Integer, _
                                                    ByRef DT_Dettagli_Round As DataTable, _
                                                    ByRef DrQuery As DataRow)

        Dim x_IVA, x_Imponibile, x_Imponibile_Netto As Double
        Dim modalita_doc As enum_ModalitaFattura

        With DrQuery

            'x_IVA = objContabHLP.Leggi_IVA_PositivaNegativa2(Lav_Cod, CDbl(.Item("iva")))

            'x_Imponibile = objContabHLP.Leggi_Imponibile_PositivoNegativo2(Lav_Cod, CDbl(.Item("imponibile")))
            'x_Imponibile_Netto = objContabHLP.Leggi_Imponibile_PositivoNegativo2(Lav_Cod, CDbl(.Item("imponibile_netto")))

            x_IVA = CDec(.Item("iva"))
            x_Imponibile = CDec(.Item("imponibile"))
            x_Imponibile_Netto = CDec(.Item("imponibile_netto"))

            If .Item("Modalita") = enum_ModalitaFattura.Fattura_AcquistiIntracom Then
                modalita_doc = enum_ModalitaFattura.Fattura_AcquistiIntracom
            Else
                'le altre modalità le devo far viaggiare insieme
                modalita_doc = enum_ModalitaFattura.NonDefinito
            End If

            objLanRound.InserisciRiga_DtDettagli(DT_Dettagli_Round, _
                                                .Item("Id_Mov_Det"), _
                                                .Item("ChkLayOut_Hide"), _
                                                 .Item("Sconto_Modalita"), _
                                                 .Item("Sconto"), _
                                                 .Item("Sconto_listino"), _
                                                .Item("qta"), _
                                                .Item("Prezzo_Unitario"), _
                                                .Item("Prezzo_Unitario_Netto"), _
                                                .Item("Imponibile"), _
                                                .Item("Imponibile_Netto"), _
                                                .Item("Cod_IVA"), _
                                                .Item("IVA"), _
                                                .Item("Aliquota"), _
                                                .Item("Sigla_IVA"), _
                                                0, _
                                                .Item("Iva_Indetraibile"), _
                                                .Item("Iva_Indetraibile_Perc"), _
                                                modalita_doc)

        End With

    End Sub


    '########################################################################
    Private Sub ElaboraDettagli_x_Riepilogo_IVA(ByRef objLanRound As AgronicaCoreContabHLP.GiasLan_Round, _
                                                ByVal DT_Dettagli_Round As DataTable, _
                                                ByRef DT_IVA_Round As DataTable, _
                                                ByRef Num_Protocollo As Double, _
                                                ByVal Lav_Cod As Integer, _
                                                 ByVal Flag_Vendita As Boolean, _
                                                ByVal Edit_Importo As enum_EditImporto)
        '  ByRef Riepilogo_Importo As Double)

        'questi servono per il riepilogo a fine fattura, non servono quindi in questo report
        Dim Riepilogo_ImponibileLordo As Double = 0
        Dim Riepilogo_Variazioni As Double = 0
        Dim Riepilogo_ImponibileNetto As Double = 0
        Dim Riepilogo_Imposta As Double = 0
        Dim Riepilogo_Importo As Double = 0

        DT_IVA_Round = objLanRound.FormAggiornaImportoNEW(objParametri_Server, _
                                                            DT_Dettagli_Round, _
                                                            Riepilogo_ImponibileLordo, _
                                                            Riepilogo_Variazioni, _
                                                            Riepilogo_ImponibileNetto, _
                                                            Riepilogo_Imposta, _
                                                            Riepilogo_Importo, _
                                                            Edit_Importo, _
                                                            True, _
                                                            Flag_Vendita, _
                                                            EsigibilitaIva)


        'modifica del 30/07/2015: nel totale del documento (num_protocollo),
        'non metto riepilogo_importo perchè è il totale da pagare (nel caso degli omaggi è diverso dal totale fattura)
        'lo imposto uguale al totale imponibile + iva
        'Num_Protocollo = Riepilogo_Importo
        Num_Protocollo = Riepilogo_ImponibileNetto + Riepilogo_Imposta

    End Sub



    '########################################################################
    Private Sub Dataset_InserisciRighe(ByRef objContabHLP As AgronicaCoreContabHLP.Contabilita, _
                                        ByRef objLanRound As AgronicaCoreContabHLP.GiasLan_Round, _
                                         ByRef DSRegistriIVA As DS_RegistriIVA, _
                                         ByVal DT_Query As DataTable, _
                                         ByVal DT_IVA_Round As DataTable, _
                                         ByVal DtIvaIndetComp As DataTable, _
                                         ByVal i_operazione As Integer, _
                                         ByVal Flag_Vendita As Boolean, _
                                        ByRef Tot_Imponibile_Intracom As Double, _
                                        ByRef Tot_IVA_Intracom As Double, _
                                        ByRef Tot_Imponibile As Double, _
                                        ByRef Tot_IVA As Double, _
                                        ByRef Tot_Importo As Double)

        Dim j As Integer
        Dim DrR As DS_RegistriIVA.RegistriIVARow
        Dim data_x_periodo As Date
        Dim Imponibile_Netto As Double = 0
        Dim IVA As Double = 0
        Dim Num_Protocollo As Double = 0
        'Dim ImponibPiuIVA As Double = 0

        If Not IsNothing(DT_IVA_Round) AndAlso DT_IVA_Round.Rows.Count > 0 Then

            'per ogni aliquota dell'operazione in corso
            For j = 0 To DT_IVA_Round.Rows.Count - 1

                ' Try

                DrR = DSRegistriIVA.RegistriIVA.NewRow

                'DrR.Num_Pagina = Num_Pagina

                'in Num_Protocollo non c'è il valore salvato nel campo Num_Protocollo della tabella movimenti
                'ma il risultato della'algoritmo di round (campo Totale_Importo)
                'nota del 29/07/2015: ora contiene esattamente il totale imponibile + iva (leggi commento nel punto in cui viene valorizzato)
                Num_Protocollo = DT_Query.Rows(i_operazione).Item("num_protocollo")

                ' non ripeto la visualizzazzione per le righe successive
                If j = 0 Then

                    If Flag_Vendita = True Then
                        data_x_periodo = CDate(DT_Query.Rows(i_operazione).Item("data_movimento"))
                    Else
                        data_x_periodo = CDate(DT_Query.Rows(i_operazione).Item("data_registrazione"))
                    End If
                    Select Case data_x_periodo.Month
                        Case 1, 2, 3
                            'DrR.periodo = "I/" + Right(CStr(CDate(Dt_Mov.Rows(i).Item("data_movimento")).Year), 2)
                            DrR.periodo = "I"
                        Case 4, 5, 6
                            'DrR.periodo = "II/" + Right(CStr(CDate(Dt_Mov.Rows(i).Item("data_movimento")).Year), 2)
                            DrR.periodo = "II"
                        Case 7, 8, 9
                            'DrR.periodo = "III/" + Right(CStr(CDate(Dt_Mov.Rows(i).Item("data_movimento")).Year), 2)
                            DrR.periodo = "III"
                        Case 10, 11, 12
                            'DrR.periodo = "IV/" + Right(CStr(CDate(Dt_Mov.Rows(i).Item("data_movimento")).Year), 2)
                            DrR.periodo = "IV"
                    End Select

                    Select Case DT_Query.Rows(i_operazione).Item("lav_cod")
                        Case LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_RICEVUTA, LAVCOD_FATTURA_PROFESSIONISTI
                            DrR.causale = "Fattura"
                        Case LAVCOD_NOTA_ACCREDITO_EMESSA, LAVCOD_NOTA_ACCREDITO_RICEVUTA
                            'DrR.causale = "Nota di Accredito"
                            DrR.causale = "Nota Acc."
                        Case LAVCOD_RICEVUTA_EMESSA
                            DrR.causale = "Ric. Fisc." 'queste ora sono nel reg. corrispettivi
                        Case LAVCOD_FATTURA_LIQ_CONF_EMESSA
                            DrR.causale = "Fattura"
                        Case LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA
                            DrR.causale = "Fattura"
                        Case LAVCOD_AUTOCONSUMO, LAVCOD_AUTOCONSUMO_VINO_SFUSO
                            DrR.causale = "Autoconsumo"
                    End Select

                    'controllare che in caso di cod_contatto numerico e minore di 0 
                    '(2 istruzioni distinte!!) 
                    'tu non inserisca nella stampa il cod_contatto appunto....
                    'e neanche il campo codice fiscale (cmq stringa vuota in questo caso)
                    If IsNumeric(DT_Query.Rows(i_operazione).Item("Cod_Contatto")) = True Then
                        If CDec(DT_Query.Rows(i_operazione).Item("Cod_Contatto")) < 0 Then
                            DrR.Cod_Contatto = ""
                        Else
                            DrR.Cod_Contatto = DT_Query.Rows(i_operazione).Item("Cod_Contatto")
                        End If
                    Else
                        DrR.Cod_Contatto = DT_Query.Rows(i_operazione).Item("Cod_Contatto")
                    End If

                    DrR.Rag_Soc_Contatto = Trim(DT_Query.Rows(i_operazione).Item("Rag_Soc"))

                    DrR.Progr_protocollo = DT_Query.Rows(i_operazione).Item("Progr_protocollo")

                    DrR.numero_doc = DT_Query.Rows(i_operazione).Item("Doc_Numero_Sin") &
                                     CStr(DT_Query.Rows(i_operazione).Item("Doc_Numero")) &
                                     DT_Query.Rows(i_operazione).Item("Doc_Numero_Des")

                    If DrR.numero_doc = "0" Then
                        DrR.numero_doc = "---"
                    End If

                    DrR.Data_Movimento = CDate(DT_Query.Rows(i_operazione).Item("Data_Movimento")).ToShortDateString
                    DrR.data_registrazione = CDate(DT_Query.Rows(i_operazione).Item("Data_Registrazione")).ToShortDateString

                    'spostato sotto
                    'If j = DT_IVA_Round.Rows.Count - 1 Then
                    '    'all'ultima riga dell'operazione, visualizzo il totale
                    '    DrR.totale = Format(Num_Protocollo, "##,###,##0.00")
                    '    Tot_Importo = Tot_Importo + Num_Protocollo
                    'End If

                Else
                    'righe successive                
                    DrR.periodo = ""
                    DrR.causale = ""
                    DrR.Rag_Soc_Contatto = ""
                    DrR.Cod_Contatto = ""
                    DrR.Progr_protocollo = ""
                    DrR.numero_doc = ""
                    DrR.Data_Movimento = ""
                    DrR.data_registrazione = ""
                    DrR.totale = ""

                    'spostato sotto
                    'If j = DT_IVA_Round.Rows.Count - 1 Then
                    '    'all'ultima riga dell'operazione, visualizzo il totale
                    '    ' DrR.totale = Format(DT_Query.Rows(i_operazione).Item("num_protocollo"), "##,###,##0.00")
                    '    DrR.totale = Format(Num_Protocollo, "##,###,##0.00")
                    '    Tot_Importo = Tot_Importo + Num_Protocollo
                    'End If

                End If 'controllo su prima riga

                '  Cod_IVA = Dt_Mov.Rows(i).Item("Cod_Iva")

                Imponibile_Netto = DT_IVA_Round.Rows(j).Item("imponibile_netto")
                IVA = DT_IVA_Round.Rows(j).Item("iva")

                'ImponibPiuIVA = Imponibile_Netto + IVA

                'If j = DT_IVA_Round.Rows.Count - 1 Then
                '    'all'ultima riga dell'operazione, visualizzo il totale
                '    DrR.totale = Format(ImponibPiuIVA, "##,###,##0.00")
                '    'Tot_Importo è il totale generale stampato alla fine dei movimenti
                '    Tot_Importo = Tot_Importo + ImponibPiuIVA
                'End If

                If j = DT_IVA_Round.Rows.Count - 1 Then
                    'all'ultima riga dell'operazione, visualizzo il totale
                    DrR.totale = Format(Num_Protocollo, "##,###,##0.00")
                    Tot_Importo += Num_Protocollo
                End If


                DrR.aliquota_Iva = DT_IVA_Round.Rows(j).Item("aliquota_des") 'Dt_Mov.Rows(i).Item("Sigla_Iva")

                ''If DT_Query.Rows(i_operazione).Item("Cod_IvaIndetraibile") <> 0 Then
                ''    DrR.note_iva = DT_Query.Rows(i_operazione).Item("Sigla_IVAIndetraibile")
                ''Else
                ''    DrR.note_iva = ""
                ''End If
                'DrR.note_iva = CStr(DT_Query.Rows(i_operazione).Item("Iva_Indetraibile_Perc")) & "%"
                If DT_IVA_Round.Rows(j).Item("iva_indetraibile_perc") <> 0 Then
                    DrR.note_iva = CStr(DT_IVA_Round.Rows(j).Item("iva_indetraibile_perc")) & "%"
                Else
                    DrR.note_iva = ""
                End If

                'DrR.note_iva = objLanRound.Elabora_NoteIva_IndetComp(DtIvaIndetComp, _
                '                                                      DT_Query.Rows(i_operazione).Item("id_agenda"), _
                '                                                      DT_Query.Rows(i_operazione).Item("lav_cod"), _
                '                                                      DT_IVA_Round.Rows(j).Item("cod_iva"))



                If DT_Query.Rows(i_operazione).Item("Modalita") = enum_ModalitaFattura.Fattura_AcquistiIntracom Then
                    'acquisto intracomunitario

                    Tot_Imponibile_Intracom += Imponibile_Netto
                    Tot_IVA_Intracom += IVA

                    DrR.Stringa_1 = Format(Imponibile_Netto, "##,###,##0.00")
                    DrR.Stringa_2 = Format(IVA, "##,###,##0.00")

                    '   Tot_Importo_Intracom = Arrotonda_2Decimali(Tot_Importo_Intracom + Imponibile_Netto + Iva)

                Else

                    DrR.Imponibile_Netto = Format(Imponibile_Netto, "##,###,##0.00")
                    DrR.iva = Format(IVA, "##,###,##0.00")

                    Tot_Imponibile += Imponibile_Netto
                    Tot_IVA += IVA

                End If 'enum_ModalitaFattura.Fattura_AcquistiIntracom

                DSRegistriIVA.RegistriIVA.Rows.Add(DrR)

                'Catch ex As Exception
                '    Log_Errori += "Errore all'iterazione " + CStr(i) + "-esima: " + vbCrLf + ex.Message + vbCrLf
                'End Try

            Next 'righe dt_iva_round

        End If 'dt_iva_round


    End Sub





End Class
