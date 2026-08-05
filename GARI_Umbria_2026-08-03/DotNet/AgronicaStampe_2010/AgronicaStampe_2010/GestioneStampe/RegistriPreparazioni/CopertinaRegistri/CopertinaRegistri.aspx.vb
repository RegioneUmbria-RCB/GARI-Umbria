Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza

Public Class CopertinaRegistri
    Inherits System.Web.UI.Page

    Private rptCopertina As Rpt_CopertinaRegistri

    '----- Gestione Querystring
    Dim Qs_Piva As String
    Dim QS_Report As String
    Dim QS_Progressivo As String
    Dim QS_NumPagine As String
    Dim QS_PrimaPagina As String
    Dim QS_Stampa_SaNome As Boolean
    Dim QS_CodIndirizzo As Integer

    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

#Region " COPERTINA REGISTRI "

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

        ' istanzio l'oggetto
        rptCopertina = New Rpt_CopertinaRegistri

    End Sub

#End Region

    '##############################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'Faccio scadere subito la pagina memorizzata nella cache
        Response.Expires = 0

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))


        '#################################################################################
        '#####  Recupero dati dalla QueryString 
        '#################################################################################

        Qs_Piva = Stringa_Decodifica(CStr(Request.QueryString("p")), _
                                    AgroKey_EncoderDecoder, _
                                    Server)

        QS_Report = Stringa_Decodifica(Request.QueryString("rp").ToString, _
                                       AgroKey_EncoderDecoder, _
                                       Server)

        QS_Progressivo = Stringa_Decodifica(Request.QueryString("pr").ToString, _
                                          AgroKey_EncoderDecoder, _
                                          Server)

        QS_NumPagine = Stringa_Decodifica(Request.QueryString("np").ToString, _
                                   AgroKey_EncoderDecoder, _
                                   Server)

        QS_PrimaPagina = Stringa_Decodifica(Request.QueryString("pp").ToString, _
                                   AgroKey_EncoderDecoder, _
                                   Server)

        QS_Stampa_SaNome = Stringa_Decodifica(Request.QueryString("chkc").ToString, _
                                 AgroKey_EncoderDecoder, _
                                 Server)

        QS_CodIndirizzo = Stringa_Decodifica(Request.QueryString("ci").ToString, _
                                 AgroKey_EncoderDecoder, _
                                 Server)

        '#################################################################################
        '#####  Genero il report
        '#################################################################################



        Dim Nome_Documento As String = "CopertinaRegistro"
        Dim Log_Errori As String


        If Not Me.IsPostBack Then

           
            Try

                Carica_DatiImpresa(Log_Errori)

            Catch exc As Exception
                Log_Errori += "Lettura dei dati dell'impresa: " + vbCrLf + exc.Message + vbCrLf
            End Try


            Try

                If QS_PrimaPagina = "1" Then
                    CType(rptCopertina.Section5.ReportObjects("TextPrimaPagina"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "1"
                End If

                'SEZIONE 9 E 10: VUOTE

                'SEZIONE 2: DATI

                'SEZIONE 5: NUMERO DI PAGINA

                'REGISTRO VINIFICAZIONE DOC
                rptCopertina.Section1.SectionFormat.EnableSuppress = True
                'REGISTRO VINIFICAZIONE TAVOLA
                rptCopertina.Section8.SectionFormat.EnableSuppress = True
                'REGISTRO VINIFICAZIONE DOC E DA TAVOLA
                rptCopertina.Section13.SectionFormat.EnableSuppress = True

                'IMBOTTIGLIAMENTO
                rptCopertina.Section7.SectionFormat.EnableSuppress = True
                'COMMERCIALIZZAZIONE
                rptCopertina.Section6.SectionFormat.EnableSuppress = True
                'REGISTRO FRIZZANTI
                rptCopertina.Section11.SectionFormat.EnableSuppress = True
                'REGISTRO SPUMANTI
                rptCopertina.Section12.SectionFormat.EnableSuppress = True

                Select Case QS_Report

                    Case enum_AgroReportistica.Vinificazione_DOC
                        'rptCopertina.Section1.SectionFormat.EnableSuppress = False
                        rptCopertina.Section13.SectionFormat.EnableSuppress = False

                    Case enum_AgroReportistica.Vinificazione_ViniTavola
                        'rptCopertina.Section8.SectionFormat.EnableSuppress = False
                        rptCopertina.Section13.SectionFormat.EnableSuppress = False

                    Case enum_AgroReportistica.Imbottigliamento
                        rptCopertina.Section7.SectionFormat.EnableSuppress = False

                    Case enum_AgroReportistica.Commercializzazione
                        rptCopertina.Section6.SectionFormat.EnableSuppress = False

                    Case enum_AgroReportistica.Frizzanti
                        rptCopertina.Section11.SectionFormat.EnableSuppress = False

                    Case enum_AgroReportistica.Spumanti
                        rptCopertina.Section12.SectionFormat.EnableSuppress = False

                End Select

            Catch exc As Exception
                Log_Errori += "Visualizzazione del report: " + vbCrLf + exc.Message + vbCrLf
            End Try



            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            Dim Nome_File As String

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento + ", " + vbCrLf + _
                                        "Piva = " + CStr(Qs_Piva) + ", " + vbCrLf + _
                                        vbCrLf + vbCrLf + vbCrLf + vbCrLf + _
                                        Log_Errori

                Nome_File = "Log_Errori_" + Nome_Documento + "_" + CStr(Session("ASG_Utente_Username"))

                'GestioneFile_CreaCartellaNelPathWebConfig("Path_LogErrori_StampeEsportazioni", "Stampe_Cantine", Path_Errore, Str_Errore_Path)
                'If Str_Errore_Path = "" And Path_Errore <> "" Then
                '    GestioneFile_CreaScriviFileConRicercaNome(Log_Errori, Path_Errore, Nome_File, Str_Errore_Path, )
                'End If

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server, _
                                                 "Stampe_Cantine", _
                                                 Nome_File, _
                                                 Session("ASG_Utente_Username"), _
                                                 "CopertinaRegistri.aspx", _
                                                 Log_Errori)



            End If
            '-----------------------------------------
        End If

        '==================================================================

        Try
            Session("Report") = rptCopertina
            Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))
        Catch ex As Exception
            Log_Errori += "- Export: " + vbCrLf + ex.Message + vbCrLf
        End Try


    End Sub


    '########################################################################################################################
    Private Sub Carica_DatiImpresa(ByRef Log As String)

        Dim Vet_Intestazione(19) As String

        Dim xLetturaVET As New AgronicaCoreStampeDAL.Stampe_OP
        xLetturaVET.DatiIntestazione_Impresa_Legale(Qs_Piva, Vet_Intestazione, Log, objParametri_Server)


        If Log <> "" Then
            Log += "Affinchè vengano stampati i dati nella copertina dei registri," & vbCrLf
            Log += "devono essere salvati obbligatoriamente questi dati:" & vbCrLf
            Log += "- indirizzo dell'impresa (si salva nella maschera impresa)" & vbCrLf
            Log += "- legale rappresentante con indirizzo di residenza e luogo di nascita valorizzati" & vbCrLf
            Log += "Se i seguenti dati non ci sono, la query cmq restituisce un risultato:" & vbCrLf
            Log += "- cuaa" & vbCrLf
            Log += "- codice icq" & vbCrLf
            Log += "- codice fiscale dell'impresa (maschera del contatto) " & vbCrLf
        End If

        CType(rptCopertina.Section9.ReportObjects("TextProgressivo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                QS_Progressivo


        CType(rptCopertina.Section2.ReportObjects("TextNumeroPagine"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                "Il presente registro è composto da n. " + QS_NumPagine + " pagine incluso questa."

        CType(rptCopertina.Section2.ReportObjects("TextRagSoc"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                Vet_Intestazione(12)
        CType(rptCopertina.Section2.ReportObjects("TextPiva"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                Qs_Piva
        CType(rptCopertina.Section2.ReportObjects("TextCUAA"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                Vet_Intestazione(18)
        CType(rptCopertina.Section2.ReportObjects("TextCF"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                Vet_Intestazione(17)
        CType(rptCopertina.Section2.ReportObjects("TextICQ"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                Vet_Intestazione(19)
    
        CType(rptCopertina.Section2.ReportObjects("TextLegale"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
           Vet_Intestazione(2)
        CType(rptCopertina.Section2.ReportObjects("TextDataNascita"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
                Vet_Intestazione(3)
        CType(rptCopertina.Section2.ReportObjects("TextLuogoNascita"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
             Vet_Intestazione(5) + " " + Vet_Intestazione(6)
        CType(rptCopertina.Section2.ReportObjects("TextResidenza"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
             Vet_Intestazione(7) + " " + Vet_Intestazione(8) + " " + Vet_Intestazione(9) + " " + Vet_Intestazione(10)


        Dim indirizzo_impresa As String
        Dim objImpresa As New AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_R

        indirizzo_impresa = objImpresa.IndirizzoStrUnica(Qs_Piva, _
                                                                objParametri_Server)

        CType(rptCopertina.Section2.ReportObjects("TextSedeLegale"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = indirizzo_impresa


        Dim indirizzocentro_sede_legale As String
        Dim sa_nome_legale As String = ""
        Dim objCentro As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read
        indirizzocentro_sede_legale = objCentro.Indirizzo_SedeLegale(Qs_Piva, _
                                                                sa_nome_legale, _
                                                                objParametri_Server)

        
        'CType(rptCopertina.Section2.ReportObjects("TextSedeLegale"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _
        '            Vet_Intestazione(13) + " " + Vet_Intestazione(14) + " " + Vet_Intestazione(15) + " " + Vet_Intestazione(16)

        Dim indirizzo_sede_cantina As String
        If QS_CodIndirizzo = 0 Then
            indirizzo_sede_cantina = Vet_Intestazione(13) + " " + Vet_Intestazione(14) + " " + Vet_Intestazione(15) + " " + Vet_Intestazione(16)
        Else
            Dim objInd As New AgronicaCoreAnagrafeDAL.Indirizzi_Read
            indirizzo_sede_cantina = objInd.Indirizzo_from_CodIndirizzo(QS_CodIndirizzo, objParametri_Server)
        End If

        Dim sa_nome As String
        If QS_Stampa_SaNome = False Then
            sa_nome = ""
        Else
            sa_nome = objCentro.SaNome_from_CodIndirizzoCentro(QS_CodIndirizzo, objParametri_Server)
            If sa_nome = "" Then
                sa_nome = sa_nome_legale
            End If
            sa_nome += " - "
        End If

        CType(rptCopertina.Section2.ReportObjects("TextSedeCantina"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = sa_nome & indirizzo_sede_cantina


    End Sub






End Class
