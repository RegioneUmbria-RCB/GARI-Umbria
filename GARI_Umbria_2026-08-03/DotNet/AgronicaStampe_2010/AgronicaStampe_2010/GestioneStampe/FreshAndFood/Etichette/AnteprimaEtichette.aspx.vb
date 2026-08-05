

Imports AgronicaCoreStampeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider_2010
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreStampeDAL.FF_GestoreConfigStampa

Imports System.Web.Services
Imports CrystalDecisions.CrystalReports.Engine
Imports System.Drawing
Imports CrystalDecisions.CrystalReports
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Management
Imports System.IO
Imports AgronicaCoreVarieBIZ


Public Class AnteprimaEtichette
    Inherits System.Web.UI.Page

    Private objParametri_server As AgronicaCoreParametri

    Private Const LabelSeparator = "|"c
    Private Const LabelPrintSeparator = ","c
    Private Const NomeTabella_AgendaxUpdate As String = "Movimenti_Dettagli"
    Private Const NomeColonna_AgendaxUpdate As String = "ID_Attivita"
    Private Const SeparatoreCodiceQRY As String = "-"
    Public isDebugAtt As String = "false"

#Region "Web Service part"


    <WebMethod(EnableSession:=True)> _
    Public Shared Function LeggiConfigurazioneStampa_DatoCod(ByVal FF_Stampa_Dettagli_Cod As String) As rispostaStandard(Of List(Of FF_ConfigurazioneDiStampa_obj))
        Dim r As New rispostaStandard(Of List(Of FF_ConfigurazioneDiStampa_obj))

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If


        Try

            'Inserire il codice QUI..
            Dim rval As New List(Of FF_ConfigurazioneDiStampa_obj)
            Dim ll As New FF_ConfigurazioneDiStampa_biz

            Dim cfg As FF_ConfigurazioneDiStampa_obj

            Dim parteFF_Stampa_Dettagli_Cod, parteFF_Stampanti_Cod As Integer
            For Each iFF_Stampa_Dettagli_Cod As String In FF_Stampa_Dettagli_Cod.Split(",")

                parteFF_Stampanti_Cod = iFF_Stampa_Dettagli_Cod.Split(SeparatoreCodiceQRY)(0)
                parteFF_Stampa_Dettagli_Cod = iFF_Stampa_Dettagli_Cod.Split(SeparatoreCodiceQRY)(1)

                cfg = ll.Leggi(parteFF_Stampa_Dettagli_Cod, parteFF_Stampanti_Cod, objParametri_Server)
                If Not cfg Is Nothing Then
                    rval.Add(cfg)
                Else
                    rval.Add(New FF_ConfigurazioneDiStampa_obj With {.TipoReport = 1})
                End If

            Next

            r.RispostaOK = True
            r.RispostaStringa = rval

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & _
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function



    <WebMethod(EnableSession:=True)> _
    Public Shared Function Filtra(ByVal id_agenda As String, ByVal OModuli_Referenze_Config_Testata As Integer, ByVal lav_cod As String) As RispostaStandard
        Dim r As New rispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If


        Try

            'Inserire il codice QUI..

            r.RispostaOK = True
            r.RispostaStringa = GeneraAnteprima(id_agenda, OModuli_Referenze_Config_Testata, lav_cod)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & _
                 AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)



        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)> _
    Public Shared Function Anteprima_o_Stampa(ByVal ConfigurazioneDaStampare As String, ByVal StampaDiretta As Boolean) As Etichette_RispostaStandard
        Dim r As New Etichette_RispostaStandard

        ConfigurazioneDaStampare = ConfigurazioneDaStampare.Replace("&#92;", "\")

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If


        Try

            'Inserire il codice QUI..
            r = ReportEPassa(ConfigurazioneDaStampare, StampaDiretta, objParametri_Server)

            r.RispostaOK = True
            r.IsLink = True
            r.RispostaStringa = "Ok."

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & _
                 AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)



        End Try

        Return r
    End Function




#End Region

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        'AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto

        'CType(Page.Master.FindControl("Lbl_Titolo"), Label).Text = "Parametri di Stampa Etichette"


    End Sub


    ' ##################################################################################################
    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)


        Dim strJS1 As New StringBuilder
        strJS1.AppendLine("$(document).ready(function () { ")
        strJS1.AppendLine("      window.close() ")
        strJS1.AppendLine(" });")

        ScriptManager.RegisterStartupScript(Me.Page, Me.Page.GetType(),
                                  String.Format("jQuery_{0}", Me.Page.ClientID), strJS1.ToString, True)

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        isDebugAtt = Debugger.IsAttached.ToString.ToLower

        If txtID_Agenda.Value = "" Then

            Dim id_Agenda As String = Request.QueryString("i")
            If Not String.IsNullOrEmpty(id_Agenda) Then
                txtID_Agenda.Value = Stringa_Decodifica(id_Agenda, AgroKey_EncoderDecoder, Server)
            End If

        End If

        Dim piva = Stringa_Decodifica(HttpContext.Current.Request.QueryString("piva"), AgroKey_EncoderDecoder)
        HttpContext.Current.Session("piva") = piva

        objParametri_server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        Dim lavCod As Integer
        getReport(txtID_Agenda.Value, lavCod, 1, objParametri_server)

        hLav_Cod.Value = lavCod

        btnAnteprimaEtichetteImballo.Visible = False
        btnAnteprimaEtichetteDettaglio.Visible = False
        Select Case lavCod
            Case 5000, 5001
                btnAnteprimaEtichetteImballo.Visible = False
                btnAnteprimaEtichette.Visible = False
                btnAnteprimaEtichetteConfezione.Visible = False


                btnPickinglist.Visible = False

            Case LAVCOD_ORDINE_VENDITA
                btnAnteprimaEtichette.Visible = True
                btnAnteprimaEtichetteImballo.Visible = True
                btnPickinglist.Visible = True

            Case LAVCOD_BOLLA_EMESSA
                btnAnteprimaEtichette.Visible = True
                btnPickinglist.Visible = False
                btnAnteprimaEtichetteImballo.Visible = True

            Case _
                LAVCOD_TESTATE_ORDINE_LAVORAZIONE,
                LAVCOD_BOLLA_RICEVUTA,
                LAVCOD_ACCETTAZIONE_DIVERSI,
                LAVCOD_DISTINTA_CARICO,
                LAVCOD_DISTINTA_CARICO_ACCETTAZIONE,
                LAVCOD_AUTO_DDT_EMESSO,
                LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE

                btnAnteprimaEtichetteImballo.Visible = False
                btnAnteprimaEtichette.Visible = True
                btnPickinglist.Visible = False
        End Select

        If Not Page.IsPostBack Then
            bindLayout()
            bindLingua()
            BindStampante()
            bindCFGTabelle()
        End If

    End Sub


#Region "Passaggio report ad Anteprima"



    Private Shared Function GeneraAnteprima( _
            ByVal id_agenda As Integer, _
            ByVal OModuli_Referenze_Config_Testata As Integer, _
            ByVal lavCod As Integer _
        ) As String

        Dim coreLettura As New AgronicaCoreStampeDAL.FF_Etichette_R

        Dim objParametri_server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))


        Dim ff_te_c As String
        ElencoFF_Etichette_tipo_DaLav_Cod(lavCod, ff_te_c, objParametri_server)

        Dim dt As DataTable
        Select Case lavCod
            Case LAVCOD_BOLLA_EMESSA, LAVCOD_ORDINE_VENDITA
                dt = coreLettura.LeggiEtichetta_PalletSintetica(id_agenda, OModuli_Referenze_Config_Testata, FF_Etichette_tipo.DescrizionePerCombo, "", "", objParametri_server)
                Return DT_toJson_Anteprima(ff_te_c, id_agenda, dt, objParametri_server)

                'dt = coreLettura.LeggiEtichetta_xWaTable_v2(id_agenda, "", "", objParametri_server)
                'Return DT_toJson_Anteprima_prn(id_agenda, dt, objParametri_server)

            Case 5000
                dt = coreLettura.LeggiEtichetta_PalletSintetica(id_agenda, OModuli_Referenze_Config_Testata, FF_Etichette_tipo.DescrizionePerCombo, "", "", objParametri_server)
                Return DT_toJson_Anteprima(ff_te_c, id_agenda, dt, objParametri_server)

            Case 5001
                dt = coreLettura.LeggiEtichetta_xWaTable_v2(id_agenda, "", "", objParametri_server)
                Return DT_toJson_Anteprima_prn(ff_te_c, id_agenda, OModuli_Referenze_Config_Testata, dt, objParametri_server)

            Case _
                LAVCOD_TESTATE_ORDINE_LAVORAZIONE,
                LAVCOD_BOLLA_RICEVUTA,
                LAVCOD_ACCETTAZIONE_DIVERSI,
                LAVCOD_DISTINTA_CARICO,
                LAVCOD_DISTINTA_CARICO_ACCETTAZIONE,
                LAVCOD_AUTO_DDT_EMESSO,
                LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE

                Dim xFiltroAggiuntivo As String = FiltraRighe()

                dt = coreLettura.LeggiEtichetta_ConferimentoSintetica(id_agenda, OModuli_Referenze_Config_Testata, FF_Etichette_tipo.DescrizionePerCombo, xFiltroAggiuntivo, "", objParametri_server)
                Return DT_toJson_Anteprima(ff_te_c, id_agenda, dt, objParametri_server)

                'dt = coreLettura.LeggiEtichetta_xWaTable_v2(id_agenda, "", "", objParametri_server)
                'Return DT_toJson_Anteprima_prn(id_agenda, dt, objParametri_server)

        End Select



    End Function


    Public Shared Function DT_toJson_Anteprima_prn( _
                ByVal ff_te_c As String, _
                ByVal id_agenda As Integer, _
                ByVal OModuli_Referenze_Config_Testata As Integer, _
                ByVal dt As DataTable, _
                ByVal ObjParametri_Server As AgronicaCoreParametri _
    ) As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        c = New ColonneNome("Codice", "Sel", "string")
        c._Filtrabile = False
        'c._FormatoParticolare = " <input type='checkbox' value='{0}' class='seleziona_lotto'  /> "
        c._ColonnaDiSelezione = True
        l.Add(c)

        c = New ColonneNome("Descrizione", "Stampante", "string")
        c._placeHolder = "..."
        l.Add(c)

        c = New ColonneNome("Codice", "PDF", "string")
        c._Filtrabile = False
        c._FormatoParticolare = "<input type='Button' id='btn_anteprima_{0}' value='PDF' onclick='anteprima_dettaglio2(" & id_agenda & ", this)' /> "
        l.Add(c)

        c = New ColonneNome("Codice", "Parametri per etichette", "string")
        c._Filtrabile = False
        c._FormatoParticolare = bindMovimentiDettagli2(id_agenda, OModuli_Referenze_Config_Testata, "ddl_movDet_{0}", ObjParametri_Server)
        l.Add(c)

        c = New ColonneNome("Codice", "Layout", "string")
        c._Filtrabile = False
        c._FormatoParticolare = bindLayout2(ff_te_c, "ddl_layout_{0}", ObjParametri_Server)
        l.Add(c)

        c = New ColonneNome("Codice", "Lingua", "string")
        c._Filtrabile = False
        c._FormatoParticolare = bindLingua2("ddl_lingua_{0}", ObjParametri_Server)
        l.Add(c)

        c = New ColonneNome("Codice", "Numero", "string")
        c._Filtrabile = False
        c._FormatoParticolare = " <input id='N_Etichette_{0}' type='text' value='' class='numero_etichette'  /> "
        l.Add(c)

        c = New ColonneNome("NumeroEtichettePedana", "Numero_di_Etichette_Pedana", "string")
        c._Filtrabile = False
        c._FormatoParticolare = " <input type='text' value='{0}' class='numero_etichette'  /> "
        l.Add(c)

        c = New ColonneNome("NumeroEtichetteImballi", "Numero_di_Etichette_Imballi", "string")
        c._Filtrabile = False
        c._FormatoParticolare = " <input type='text' value='{0}' class='numero_etichette'  /> "
        l.Add(c)

        c = New ColonneNome("FF_Stampanti_cod_FF_Stampa_Dettagli_COD", "FF_Stampanti_cod_FF_Stampa_Dettagli_COD", "string")
        c._Filtrabile = False
        l.Add(c)


        c = New ColonneNome("Nome_Per_Stampa", "Nome_Per_Stampa", "string")
        c._Filtrabile = False
        l.Add(c)

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp
    End Function

    Public Shared Function DT_toJson_Anteprima(ByVal ff_te_c As String, ByVal id_agenda As Integer, ByVal dt As DataTable, ByVal ObjParametri_Server As AgronicaCoreParametri) As String



        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        c = New ColonneNome("Codice", "Sel", "string")
        c._Filtrabile = False
        'c._FormatoParticolare = " <input type='checkbox' value='{0}' class='seleziona_lotto'  /> "
        c._ColonnaDiSelezione = True
        l.Add(c)

        c = New ColonneNome("descrizioneConfezione", "Descrizione", "string")
        c._placeHolder = "..."
        l.Add(c)

        c = New ColonneNome("Codice", "Anteprima", "string")
        c._Filtrabile = False
        c._FormatoParticolare = "<input type='Button' id='btn_anteprima_{0}' value='Anteprima' onclick='anteprima_dettaglio(" & id_agenda & ", {0})' /> "
        l.Add(c)

        c = New ColonneNome("Codice", "Layout", "string")
        c._Filtrabile = False
        c._FormatoParticolare = bindLayout2(ff_te_c, "ddl_layout_{0}", ObjParametri_Server)
        l.Add(c)

        c = New ColonneNome("Codice", "Lingua", "string")
        c._Filtrabile = False
        c._FormatoParticolare = bindLingua2("ddl_lingua_{0}", ObjParametri_Server)
        l.Add(c)

        c = New ColonneNome("Codice", "Stampante", "string")
        c._Filtrabile = False
        c._FormatoParticolare = Stampanti_Load2("ddl_stampante_{0}", ObjParametri_Server).Replace("\", "\\")
        l.Add(c)

        c = New ColonneNome("Codice", "Numero", "string")
        c._Filtrabile = False
        c._FormatoParticolare = " <input id='N_Etichette_{0}' type='text' value='' class='numero_etichette'  /> "
        l.Add(c)

        c = New ColonneNome("NumeroEtichettePedana", "Numero_di_Etichette_Pedana", "string")
        c._Filtrabile = False
        c._FormatoParticolare = " <input type='text' value='{0}' class='numero_etichette'  /> "
        l.Add(c)

        c = New ColonneNome("NumeroEtichetteImballi", "Numero_di_Etichette_Imballi", "string")
        c._Filtrabile = False
        c._FormatoParticolare = " <input type='text' value='{0}' class='numero_etichette'  /> "
        l.Add(c)

        c = New ColonneNome("FF_Stampa_Dettagli_COD", "FF_Stampa_Dettagli_COD", "string")
        c._Filtrabile = False
        l.Add(c)

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp

    End Function

    Private Shared Function GetLavCod(ByVal id_agenda As Integer, ByVal objParametri_Server As AgronicaCoreParametri) As Integer
        Dim lavCodReader As New AgronicaCoreContabDAL.Agenda_R
        Dim dtLavCod As DataTable
        Dim lavCod As Integer


        If Not String.IsNullOrEmpty(id_agenda) Then
            dtLavCod = lavCodReader.Leggi( _
                "", _
                0, _
                id_agenda, _
                0, _
                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                "", _
                "", _
                objParametri_Server _
            )


        End If

        Return dtLavCod(0)("lav_cod")
    End Function


    Private Shared Function getReport(ByVal id_agenda As Integer, ByRef lavCod As Integer, ByVal TipoReport As FF_Etichette_tipo, ByVal objParametri_server As AgronicaCoreParametri, Optional ByVal NomeFileReport As String = "") As CrystalDecisions.CrystalReports.Engine.ReportDocument

        lavCod = GetLavCod(id_agenda, objParametri_server)


        Dim crHlp As New CrystalHelper

        'lettura da file system
        If NomeFileReport <> "" Then

            Dim pFile As String = HttpContext.Current.Server.MapPath("~/GestioneStampe/FreshAndFood/Etichette/Personalizzazioni")
            pFile = AgronicaCoreUtility.FileSystemHelper.AggiungiSlashSeNonEsiste(pFile)

            If My.Computer.FileSystem.FileExists(pFile & NomeFileReport) Then
                Return crHlp.getReportDaFile(pFile, NomeFileReport)
            End If

        End If


        'lettura da Reflection
        If NomeFileReport <> "" Then
            Return crHlp.getReportDaRisorseByTempFile(NomeFileReport)
        End If



        If TipoReport <> FF_Etichette_tipo.Interne Then
            Return New ImballoGenerica
        End If



        Select Case lavCod
            Case _
                LAVCOD_BOLLA_RICEVUTA, _
                LAVCOD_ACCETTAZIONE_DIVERSI, _
                LAVCOD_DISTINTA_CARICO, _
                LAVCOD_DISTINTA_CARICO_ACCETTAZIONE, _
                LAVCOD_AUTO_DDT_EMESSO, _
                LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE

                Return New rptEtichetteConferimento_A5
            Case Else
                Return New PalletGenerica

        End Select


    End Function


    Private Shared Function ReportEPassa(ByVal ConfigurazioneDaStampare As String, ByVal StampaDiretta As Boolean, ByVal objParametri_server As AgronicaCoreParametri) As Etichette_RispostaStandard



        If Not StampaDiretta Then            
            Return ReportEPassa_SingolaAnteprima(ConfigurazioneDaStampare, StampaDiretta, objParametri_server)
            Exit Function
        End If

        Dim RStandard As New Etichette_RispostaStandard
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(HttpContext.Current.Session("ASG_objParametri_Server")) Then
            RStandard.Sessione = False
            Return RStandard
        End If

        Dim ListaReportDaPassare As New List(Of ReportDaPassare)
        For Each copia In ConfigurazioneDaStampare.TrimEnd("|").Split("|")

            Dim id_agenda As Integer
            Dim id_mov_det As Integer
            Dim old_id_mov_det As Integer
            Dim numero_copie As Integer
            Dim lingua_cod As Integer
            Dim layout_cod As Integer
            Dim stampante_cod As Integer
            Dim Stampante_NomePerStampa As String
            Dim FF_Stampa_Dettagli_Cod As String
            Dim OModuli_Referenze_Config_Testata As Integer

            Dim tipoReport As Integer

            Dim lav_cod As Integer
            lav_cod = GetLavCod(id_agenda, objParametri_server)

            getLayoutLinguaStampante(copia, OModuli_Referenze_Config_Testata, id_agenda, id_mov_det, layout_cod, stampante_cod, Stampante_NomePerStampa, lingua_cod, numero_copie, FF_Stampa_Dettagli_Cod, old_id_mov_det)

            Dim altezza, larghezza As Integer
            Dim rpt As ReportDocument
            rpt = ReportEPassa_StampaMassiva(id_mov_det & ",-1", id_agenda, lav_cod, tipoReport, id_mov_det, "", layout_cod, lingua_cod, StampaDiretta, altezza, larghezza, numero_copie, OModuli_Referenze_Config_Testata, objParametri_server)

            If altezza = -1 Then
                numero_copie = altezza * numero_copie
            End If

            ListaReportDaPassare.Add( _
                New ReportDaPassare With { _
                    .Codice = AgronicaCoreUtility.FileSystemHelper.NomeFileUnivoco(".rpt"), _
                    .ilReportDaPassare = rpt, _
                    .Numero_Copie = numero_copie, _
                    .PrintName = Stampante_NomePerStampa, _
                    .Altezza = altezza, _
                    .Larghezza = larghezza _
                }
            )

        Next

        'TODO: Gestire opportunamente la configurazione        
        RStandard.Lista_FF_Stampa_Dettagli_Cod = SalvaConfigurazioneDiStampa(ConfigurazioneDaStampare, objParametri_server)

        HttpContext.Current.Session("ListaReportDaPassare") = ListaReportDaPassare

        RStandard.Lista_FF_Stampa_Dettagli_Cod = RStandard.Lista_FF_Stampa_Dettagli_Cod.TrimEnd(",")

        Dim lPageToOpen As String = GetLPageToOpen()

        RStandard.UrlLink = lPageToOpen

        Return RStandard


    End Function


    Private Shared Function SalvaConfigurazioneDiStampa(ByVal configurazioneDaSalvare As String, ByVal objParametri_server As AgronicaCoreParametri) As String

        Dim xSalva As New FF_GestoreConfigStampa
        Return xSalva.SalvaConfigurazioneDiStampa(configurazioneDaSalvare, objParametri_server)

    End Function



    Private Shared Function ReportEPassa_StampaMassiva( _
        ByVal InfoNumeroCopie As String, _
        ByVal id_agenda As Integer, _
        ByVal Lavcod As Integer, _
        ByRef tipoReport As Int16, _
        ByVal id_mov_det As Integer, _
        ByVal Nome_Per_Stampa As String, _
        ByVal Layouyt_cod As Integer, _
        ByVal Lingua_Cod As Integer, _
        ByVal stampaDiretta As Boolean, _
        ByRef Altezza As Integer, _
        ByRef Larghezza As Integer, _
        ByVal numero_copie As Integer, _
        ByVal OModuli_Referenze_Config_Testata As Integer, _
        ByVal objParametri_server As AgronicaCoreParametri _
    ) As Engine.ReportDocument

        Dim rpt As Engine.ReportDocument
        Dim dtRpt As DataTable

        Dim xFiltroDettaglio As String = ""
        If stampaDiretta Then
            xFiltroDettaglio = " detProd.Id_Mov_Det = " & id_mov_det
        End If

        Dim CoreLettura As New AgronicaCoreStampeDAL.FF_Etichette_R

        Dim StartCodeChar As String
        Dim EndCodeChar As String
        Dim StartAIChar As String
        Dim lSetCode128 As String

        Dim BarcodeFont As String
        Dim barcodeType As String
        Dim fileRpt As String


        ReportEPassa_GetDatiLayout( _
            CoreLettura, _
            Layouyt_cod, _
            StartCodeChar, _
            EndCodeChar, _
            StartAIChar, _
            lSetCode128, _
            BarcodeFont, _
            barcodeType, _
            fileRpt, _
            Altezza, _
            Larghezza, _
            tipoReport, _
            objParametri_server _
        )




        rpt = getReport(id_agenda, Lavcod, tipoReport, objParametri_server, fileRpt)
        Lavcod = GetLavCod(id_agenda, objParametri_server)

        Dim BindEffettuato As Boolean
        ReportEPassa_DecidiDatatable(id_agenda, Lingua_Cod, OModuli_Referenze_Config_Testata, tipoReport, rpt, dtRpt, CoreLettura, StartCodeChar, EndCodeChar, StartAIChar, lSetCode128, barcodeType, InfoNumeroCopie, xFiltroDettaglio, BindEffettuato, numero_copie, objParametri_server)

        If tipoReport = FF_Etichette_tipo.Interne Then
            ImpostaCodiceABarreSuELI(dtRpt, objParametri_server)
        End If

        If Not BindEffettuato Then
            rpt.SetDataSource(dtRpt)
        End If


        Return rpt

    End Function

    Private Shared Function ImpostaCodiceABarreSuELI(ByVal dtRpt As DataTable, ByVal objParametri_server As AgronicaCoreParametri)
        Dim CoreScrittura As New AgronicaCoreStampeDAL.FF_Etichette_W

        For Each drr As DataRow In dtRpt.Rows
            CoreScrittura.ImpostaCodiceABarreSuELI_UPDT( _
            drr("Cal_Cod"), _
            "oEli", _
            0, _
            0, _
            drr("CodBinHR"), _
            objParametri_server _
        )
        Next




    End Function

    Private Shared Function ReportEPassa_GetDatiLayout( _
        ByVal CoreLettura As AgronicaCoreStampeDAL.FF_Etichette_R, _
        ByVal layout_Cod As String, _
        ByRef StartCodeChar As String, _
        ByRef EndCodeChar As String, _
        ByRef StartAIChar As String, _
        ByRef lSetCode128 As String, _
        ByRef BarcodeFont As String, _
        ByRef barcodeType As String, _
        ByRef FileRpt As String, _
        ByRef Altezza As Integer, _
        ByRef Larghezza As Integer, _
        ByRef TipoReport As Integer, _        
        ByVal objParametri_Server As AgronicaCoreParametri _
    )


        Dim dtL As DataTable = CoreLettura.LeggiLayoutEtichette(" ff_LayoutEtichette_cod = " & layout_Cod, "", objParametri_Server)

        StartCodeChar = dtL.Rows(0)("StartCodeChar")
        EndCodeChar = dtL.Rows(0)("EndCodeChar")
        StartAIChar = dtL.Rows(0)("StartAIChar")
        lSetCode128 = dtL.Rows(0)("CodeSet128")

        BarcodeFont = dtL.Rows(0)("BarcodeFont")
        barcodeType = dtL.Rows(0)("barcodeType")
        FileRpt = dtL.Rows(0)("FileRpt")

        Larghezza = dtL.Rows(0)("Larghezza_Etichetta")
        Altezza = dtL.Rows(0)("Altezza_Etichetta")

        TipoReport = dtL.Rows(0)("FF_TipologiaEtichette_Cod")

    End Function

    Private Shared Sub ReportEPassa_DecidiDatatable( _
        ByVal id_agenda As Integer, _
        ByVal Lingua_cod As Integer, _
        ByVal OModuli_Referenze_Config_Testata As Integer, _
        ByVal tipoReport As FF_Etichette_tipo, _
        ByRef rpt As Engine.ReportDocument, _
        ByRef dtRpt As DataTable, _
        ByVal coreLettura As AgronicaCoreStampeDAL.FF_Etichette_R, _
        ByVal StartCodeChar As String, _
        ByVal EndCodeChar As String, _
        ByVal StartAIChar As String, _
        ByVal lSetCode128 As String, _
        ByVal barcodeType As String, _
        ByVal InfoNumeroCopie As String, _
        ByVal xFiltroDettaglio As String, _
        ByRef DataSourceImpostato As Boolean, _
        ByVal numero_copie As Integer, _
        ByVal objparametri_server As AgronicaCoreParametri _
    )


        Select Case tipoReport
            Case FF_Etichette_tipo.Interne


                Dim strFiltroEtichetta As String = FiltraRighe()

                If strFiltroEtichetta <> "" Then
                    If String.IsNullOrEmpty(xFiltroDettaglio) Then
                        xFiltroDettaglio = strFiltroEtichetta
                    Else
                        xFiltroDettaglio = "(" + xFiltroDettaglio + " AND " + strFiltroEtichetta + ")"
                    End If
                End If

                dtRpt = _
                coreLettura.LeggiEtichetta_Conferimento_Full( _
                    id_agenda, _
                    StartCodeChar, _
                    EndCodeChar, _
                    StartAIChar, _
                    lSetCode128, _
                    3, _
                    InfoNumeroCopie, _
                    barcodeType, _
                    Lingua_cod, _
                    OModuli_Referenze_Config_Testata, _
                    tipoReport, _
                    DataSourceImpostato, _
                    numero_copie, _
                    xFiltroDettaglio, _
                    "", _
                    objparametri_server, _
                    rpt _
                )
            Case FF_Etichette_tipo.SSCC
                dtRpt = _
                coreLettura.LeggiEtichetta_PalletFull( _
                    id_agenda, _
                    StartCodeChar, _
                    EndCodeChar, _
                    StartAIChar, _
                    lSetCode128, _
                    3, _
                    InfoNumeroCopie, _
                    xFiltroDettaglio, _
                    "", _
                    objparametri_server _
                )
            Case FF_Etichette_tipo.Imballo
                dtRpt = _
                coreLettura.LeggiEtichetta_Imballo_Full( _
                    id_agenda, _
                    StartCodeChar, _
                    EndCodeChar, _
                    StartAIChar, _
                    lSetCode128, _
                    3, _
                    InfoNumeroCopie, _
                    barcodeType, _
                    Lingua_cod, _
                    OModuli_Referenze_Config_Testata, _
                    tipoReport, _
                    xFiltroDettaglio, _
                    "", _
                    objparametri_server, _
                    rpt, _
                    DataSourceImpostato _
               )

            Case FF_Etichette_tipo.Confezione
                dtRpt = _
                coreLettura.LeggiEtichetta_Confezione_Full( _
                    id_agenda, _
                    StartCodeChar, _
                    EndCodeChar, _
                    StartAIChar, _
                    lSetCode128, _
                    3, _
                    InfoNumeroCopie, _
                    barcodeType, _
                    Lingua_cod, _
                    OModuli_Referenze_Config_Testata, _
                    tipoReport, _
                    xFiltroDettaglio, _
                    "", _
                    objparametri_server, _
                    rpt, _
                    DataSourceImpostato _
               )

        End Select
    End Sub


    Private Shared Function FiltraRighe() As String

        Dim strFiltroEtichetta As String = ""

        Dim strXmlVariabilistampe As String = HttpContext.Current.Session("strXmlVariabilistampe")
        Dim XmlDoc As New System.Xml.XmlDocument
        XmlDoc.LoadXml(strXmlVariabilistampe)
        Dim XML_FiltroStampa As System.Xml.XmlElement
        Dim XMLs_VariabiliStampe As System.Xml.XmlNodeList
        Dim XML_VariabiliStampe As System.Xml.XmlElement
        XML_FiltroStampa = XmlDoc.SelectSingleNode("ParametriAgronicaStampe_2010")
        XMLs_VariabiliStampe = XML_FiltroStampa.GetElementsByTagName("VariabiliStampe")
        Dim attributo_jolly As String = Nothing
        Dim attributo_jolly_SPLIT As String() = Nothing
        Dim cal_cod As Integer = 0


        For i = 0 To XMLs_VariabiliStampe.Count - 1

            XML_VariabiliStampe = XMLs_VariabiliStampe.Item(i)

            attributo_jolly = XML_VariabiliStampe.GetAttribute("attributo_jolly")

            If Not String.IsNullOrEmpty(attributo_jolly) Then
                attributo_jolly_SPLIT = attributo_jolly.Split(",")
            End If
            If Not attributo_jolly_SPLIT Is Nothing AndAlso attributo_jolly_SPLIT.Length = 3 Then
                cal_cod = Integer.Parse(attributo_jolly_SPLIT(2))

                If strFiltroEtichetta = "" Then
                    strFiltroEtichetta = "("
                Else
                    strFiltroEtichetta = strFiltroEtichetta + " OR "
                End If

                strFiltroEtichetta = strFiltroEtichetta + "detProd.cal_cod=" + cal_cod.ToString()
            End If
        Next

        If strFiltroEtichetta <> "" Then
            strFiltroEtichetta = strFiltroEtichetta + ")"
        End If

        Return strFiltroEtichetta

    End Function

    Private Shared Function GetLPageToOpen() As String
        Dim vAppUrl As String() = HttpContext.Current.Request.Url.ToString.Split("/")
        'Dim vBase As String = vAppUrl(0) & "//" & vAppUrl(2) & "/" & vAppUrl(3)
        'Dim lPageToOpen As String = vBase & "/GestioneStampe/VisualizzatoreReport.aspx?anteprima=" & Stringa_Codifica("1", AgroKey_EncoderDecoder, HttpContext.Current.Server)
        Dim vBase As String = vAppUrl(0) & "//" & HttpContext.Current.Request.Url.Authority & HttpContext.Current.Request.ApplicationPath
        Dim lPageToOpen As String = vBase & "/GestioneStampe/VisualizzatoreReport.aspx?anteprima=" & Stringa_Codifica("1", AgroKey_EncoderDecoder, HttpContext.Current.Server)
        Return lPageToOpen
    End Function
    Private Shared Function ReportEPassa_SingolaAnteprima(ByVal ConfigurazioneDaStampare As String, ByVal StampaDiretta As Boolean, ByVal ObjParametri_server As AgronicaCoreParametri) As Etichette_RispostaStandard

        Dim lavCod As Integer
        Dim rpt As New Engine.ReportDocument
        Dim dtRpt As DataTable
        Dim coreLettura As New AgronicaCoreStampeDAL.FF_Etichette_R

        Dim dtL As DataTable

        Dim id_agenda As Integer
        Dim id_mov_det As Integer
        Dim old_id_mov_det As Integer
        Dim numero_copie As Integer
        Dim lingua_cod As Integer
        Dim layout_cod As Integer
        Dim stampante_cod As Integer
        Dim Stampante_NomePerStampa As String
        Dim FF_Stampa_Dettagli_Cod As Integer
        Dim OModuli_Referenze_Config_Testata As Integer

        Dim tipoReport As Integer

        getLayoutLinguaStampante(ConfigurazioneDaStampare, OModuli_Referenze_Config_Testata, id_agenda, id_mov_det, layout_cod, stampante_cod, Stampante_NomePerStampa, lingua_cod, numero_copie, FF_Stampa_Dettagli_Cod, old_id_mov_det)

        dtL = coreLettura.LeggiLayoutEtichette(" ff_LayoutEtichette_cod = " & layout_cod, "", ObjParametri_server)

        tipoReport = dtL.Rows(0)("FF_TipologiaEtichette_Cod")

        rpt = getReport(id_agenda, lavCod, tipoReport, ObjParametri_server, dtL(0)("FileRPT"))


        Dim StartCodeChar As String
        Dim EndCodeChar As String
        Dim StartAIChar As String
        Dim lSetCode128 As String

        Dim BarcodeFont As String
        Dim barcodeType As String


        StartCodeChar = dtL.Rows(0)("StartCodeChar")
        EndCodeChar = dtL.Rows(0)("EndCodeChar")
        StartAIChar = dtL.Rows(0)("StartAIChar")
        lSetCode128 = dtL.Rows(0)("CodeSet128")

        BarcodeFont = dtL.Rows(0)("BarcodeFont")
        barcodeType = dtL.Rows(0)("barcodeType")

        Dim InfoNumeroCopie As String
        InfoNumeroCopie = id_mov_det & "," & numero_copie

        If StampaDiretta Then
            InfoNumeroCopie = InfoNumeroCopie.Replace(",", ",-")
        End If

        Dim xFiltroDettaglio As String
        If StampaDiretta Then
            xFiltroDettaglio = " detProd.Id_Mov_Det = "
        End If


        Dim DataSourceImpostato As Boolean
        ReportEPassa_DecidiDatatable(id_agenda, lingua_cod, OModuli_Referenze_Config_Testata, tipoReport, rpt, dtRpt, coreLettura, StartCodeChar, EndCodeChar, StartAIChar, lSetCode128, barcodeType, InfoNumeroCopie, xFiltroDettaglio, DataSourceImpostato, numero_copie, ObjParametri_server)

        If Not DataSourceImpostato Then
            rpt.SetDataSource(dtRpt)
        End If

        ImpostaNomeAzienda(rpt, ObjParametri_server)

        HttpContext.Current.Session.Remove("ListaReportDaPassare")
        HttpContext.Current.Session("Report") = rpt


        Dim QS_PrintName As String
        Dim QS_Numero_Copie As Integer
        Dim QS_Str_Flag_Fascicola As String
        Dim QS_Start_Page As Integer
        Dim QS_End_Page As Integer

        Dim qS_cript As String


        If StampaDiretta Then

            QS_PrintName = Stampante_NomePerStampa
            QS_Numero_Copie = 1
            QS_Start_Page = 0
            QS_End_Page = 0
            QS_Str_Flag_Fascicola = "False"

            qS_cript = _
                "&PrintServizioWindows=" & Stringa_Codifica("1", AgroKey_EncoderDecoder, HttpContext.Current.Server) & _
                "&QS_PrintName=" & Stringa_Codifica(QS_PrintName, AgroKey_EncoderDecoder, HttpContext.Current.Server) & _
                "&QS_Numero_Copie=" & Stringa_Codifica(QS_Numero_Copie, AgroKey_EncoderDecoder, HttpContext.Current.Server) & _
                "&QS_Start_Page=" & Stringa_Codifica(QS_Start_Page, AgroKey_EncoderDecoder, HttpContext.Current.Server) & _
                "&QS_End_Page=" & Stringa_Codifica(QS_End_Page, AgroKey_EncoderDecoder, HttpContext.Current.Server) & _
                "&QS_Str_Flag_Fascicola=" & Stringa_Codifica(QS_Str_Flag_Fascicola, AgroKey_EncoderDecoder, HttpContext.Current.Server)

        End If


        Dim lPageToOpen As String = GetLPageToOpen()

        Dim rval As New Etichette_RispostaStandard
        rval.UrlLink = lPageToOpen & qS_cript

        Return rval

    End Function

    Private Shared Sub ImpostaNomeAzienda(ByRef rpt As Engine.ReportDocument, ByVal ObjParametri_server As AgronicaCoreParametri)

        If ObjParametri_server.PivaSuperUser = "04273150716" Then

            Dim piva = HttpContext.Current.Session("piva")
            Dim aziendaDes = "PdP Coop"

            If Not piva Is Nothing Then
                Select Case piva
                    Case "04011420710"
                        aziendaDes = "PdP Coop"
                    Case "04149620710"
                        aziendaDes = "Piana del V."
                End Select
            End If

            rpt.SetParameterValue("AziendaDes", aziendaDes)
        End If

        HttpContext.Current.Session.Remove("piva")

    End Sub



#End Region



#Region "Bind delle combo"


    Private Shared Function bindMovimentiDettagli2( _
        ByVal id_agenda As Integer, _
        ByVal OModuli_Referenze_Config_Testata As Integer, _
        ByVal ctrlId As String, _
        ByVal objParametri_server As AgronicaCoreParametri _
    ) As String


        Dim dtRpt As DataTable
        Dim coreLettura As New AgronicaCoreStampeDAL.FF_Etichette_R

        Dim ddlLayout1 As New DropDownList

        dtRpt = _
        coreLettura.LeggiEtichetta_ConferimentoSintetica(id_agenda, OModuli_Referenze_Config_Testata, FF_Etichette_tipo.DescrizionePerCombo, "", "", objParametri_server)

        ddlLayout1.ID = ctrlId
        ddlLayout1.DataSource = dtRpt
        ddlLayout1.CssClass = "selectpicker"
        ddlLayout1.Attributes.Add("data-live-search", "true")
        ddlLayout1.Attributes.Add("data-container", "body")
        ddlLayout1.Attributes.Add("data-width", "400")
        ddlLayout1.Attributes.Add("data-style", "selectpicker_layout")


        ddlLayout1.DataTextField = "DescrizioneXwaTable"
        ddlLayout1.DataValueField = "codice"

        ddlLayout1.DataBind()


        Return GetHtmlFromControl(ddlLayout1)

    End Function


    Private Shared Function bindLayout2(ByVal ff_te_c As String, ByVal ctrlId As String, ByVal objParametri_server As AgronicaCoreParametri) As String


        Dim dtRpt As DataTable
        Dim coreLettura As New AgronicaCoreStampeDAL.FF_Etichette_R

        Dim ddlLayout1 As New DropDownList

        dtRpt = _
        coreLettura.LeggiLayoutEtichette(" ff_TipologiaEtichette_cod in ( " & ff_te_c & " )", "", objParametri_server)


        ddlLayout1.ID = ctrlId
        ddlLayout1.DataSource = dtRpt
        ddlLayout1.CssClass = "selectpicker"
        ddlLayout1.Attributes.Add("data-live-search", "true")
        ddlLayout1.Attributes.Add("data-container", "body")
        ddlLayout1.Attributes.Add("data-width", "450")
        ddlLayout1.Attributes.Add("data-style", "selectpicker_layout")


        ddlLayout1.DataTextField = "DescrizioneEstesa"
        ddlLayout1.DataValueField = "FF_LayoutEtichette_COD"

        ddlLayout1.DataBind()


        Return GetHtmlFromControl(ddlLayout1)

    End Function


    Private Shared Sub ElencoFF_Etichette_tipo_DaLav_Cod(ByVal lavCod As Integer, ByRef ff_te_c As String, ByVal ObjParametri_server As AgronicaCoreParametri)


        Dim xLetturaLav_Cod As New FF_OperazioniXFF_TipologiaEtichette_R
        Dim dtxLavCod As DataTable = _
         xLetturaLav_Cod.Leggi( _
             lavCod, _
             "", _
             "", _
             ObjParametri_server _
         )

        ff_te_c = String.Join( _
            ",", _
            ( _
                From dd In dtxLavCod.AsEnumerable _
                Select dd("FF_Etichette_Tipologia_Cod") _
            ).ToArray _
        )
    End Sub
    Private Sub bindLayout()


        Dim dtRpt As DataTable
        Dim coreLettura As New AgronicaCoreStampeDAL.FF_Etichette_R

        Dim lavCod As Integer
        lavCod = GetLavCod(txtID_Agenda.Value, objParametri_server)

        Dim ff_te_c As String
        ElencoFF_Etichette_tipo_DaLav_Cod(lavCod, ff_te_c, objParametri_server)


        dtRpt = _
        coreLettura.LeggiLayoutEtichette(" ff_TipologiaEtichette_cod in ( " & ff_te_c & " )", "", objParametri_server)

        ddlayout.DataSource = dtRpt

        ddlayout.DataTextField = "DescrizioneEstesa"
        ddlayout.DataValueField = "FF_LayoutEtichette_COD"

        ddlayout.DataBind()



    End Sub

    Private Sub bindCFGTabelle()

        Dim oTestata As New OModuli_Referenze_Config_Testata_R
        Dim dt As DataTable = _
            oTestata.Leggi( _
                0, _
                2, _
                "", _
                "", _
                objParametri_server _
            )

        cmbCFGTabelle.DataSource = dt

        cmbCFGTabelle.DataTextField = "Descrizione"
        cmbCFGTabelle.DataValueField = "id_testata"
        cmbCFGTabelle.DataBind()
    End Sub

    Private Sub BindStampante()
        Stampanti_Load(ddlPrinter)
    End Sub

    Private Shared Function bindLingua2(ByVal id As String, ByVal objParametri_server As AgronicaCoreParametri) As String

        Dim dtRpt As DataTable
        Dim coreLettura As New AgronicaCoreStampeDAL.FF_Etichette_R


        dtRpt = _
        coreLettura.LeggiLinguEtichette("", " Lingua_Default Desc, LE1.Nome Asc  ", objParametri_server)

        Dim ddLingua1 As New DropDownList

        ddLingua1.DataSource = dtRpt
        ddLingua1.ID = id
        ddLingua1.CssClass = "selectpicker"
        ddLingua1.Attributes.Add("data-live-search", "true")
        ddLingua1.Attributes.Add("data-container", "body")
        ddLingua1.Attributes.Add("data-width", "170")
        ddLingua1.Attributes.Add("data-style", "selectpicker_lingua")

        ddLingua1.DataTextField = "Lingua_Des"
        ddLingua1.DataValueField = "Lingua_Cod"

        ddLingua1.DataBind()

        Return GetHtmlFromControl(ddLingua1)

    End Function


    Private Sub bindLingua()

        Dim dtRpt As DataTable
        Dim coreLettura As New AgronicaCoreStampeDAL.FF_Etichette_R


        dtRpt = _
        coreLettura.LeggiLinguEtichette("", " Lingua_Default Desc, LE1.Nome Asc ", objParametri_server)

        ddlingua.DataSource = dtRpt

        ddlingua.DataTextField = "Lingua_Des"
        ddlingua.DataValueField = "Lingua_Cod"

        ddlingua.DataBind()



    End Sub

#End Region

#Region "Gestione Stampanti"


    Private Shared Function Stampanti_Load2(ByVal id As String, ByVal objParametri_Server As AgronicaCoreParametri) As String

        Dim i As Integer
        Dim NomeStampante As String
        Dim Cmb_Stampante1 As New DropDownList
        Cmb_Stampante1.ID = id
        Cmb_Stampante1.CssClass = "selectpicker"
        Cmb_Stampante1.Attributes.Add("data-live-search", "true")
        Cmb_Stampante1.Attributes.Add("data-container", "body")
        Cmb_Stampante1.Attributes.Add("data-width", "225")
        Cmb_Stampante1.Attributes.Add("data-style", "selectpicker_stampa")
        Cmb_Stampante1.Items.Clear()


        '==============================================
        '--- Aggiungi stampanti a mano dal database

        Dim xLeggiP As New AgronicaCoreStampeDAL.FF_Stampanti_R
        Dim dtP As DataTable = xLeggiP.Leggi( _
            "", _
            "", _
            objParametri_Server _
        )

        Dim iContaP As Integer = 0
        For Each Stampante As DataRow In dtP.Rows

            Cmb_Stampante1.Items.Add(New ListItem(Stampante("Nome_Per_Stampa"), Stampante("FF_Stampanti_Cod")))
            iContaP += 1
        Next


        Return GetHtmlFromControl(Cmb_Stampante1)


    End Function

    Private Sub Stampanti_Load(ByVal Cmb_Stampante As DropDownList)

        Dim i As Integer
        Dim NomeStampante As String

        Cmb_Stampante.Items.Clear()


        '==============================================
        '--- Aggiungi stampanti a mano dal database

        Dim xLeggiP As New AgronicaCoreStampeDAL.FF_Stampanti_R
        Dim dtP As DataTable = xLeggiP.Leggi( _
            "", _
            "", _
            objParametri_server _
        )

        Dim iContaP As Integer = 0
        For Each Stampante As DataRow In dtP.Rows

            Cmb_Stampante.Items.Add(New ListItem(Stampante("Nome_Per_Stampa"), CStr(iContaP)))
            iContaP += 1
        Next

        '--- Fine Aggiungi stmpanti a mano dal database




    End Sub

#End Region


#Region "Eventi di click"

    Protected Sub btnPickinglist_Click(sender As Object, e As EventArgs) Handles btnPickinglist.Click
        Response.Redirect("../PickingList/AnteprimaPickingList.aspx?i=" & Request.QueryString("i"))
    End Sub


    'TODO: segare appena pronta nuova versione
    'Protected Sub btnAnteprimaEtichette_Click(sender As Object, e As EventArgs) Handles btnAnteprimaEtichette.Click
    '    ReportEPassa(1, False)
    'End Sub

    'Protected Sub btnAnteprimaEtichetteImballo_Click(sender As Object, e As EventArgs) Handles btnAnteprimaEtichetteImballo.Click
    '    ReportEPassa(2, False)
    'End Sub

    'Protected Sub btnStampaDirettaEtichette_Click(sender As Object, e As EventArgs) Handles btnStampaDirettaEtichette.Click
    '    ReportEPassa(1, True)
    'End Sub

    'Protected Sub btnAnteprimaEtichetteConfezione_Click(sender As Object, e As EventArgs) Handles btnAnteprimaEtichetteConfezione.Click
    '    ReportEPassa(3, False)
    'End Sub



    'Protected Sub btnAnteprimaEtichetteDettaglio_Click(sender As Object, e As EventArgs) Handles btnAnteprimaEtichetteDettaglio.Click
    '    ReportEPassa(-1, False)
    'End Sub

#End Region


#Region "funzioni di utilità"

    Private Shared Function GetHtmlFromControl(ByVal ctrl As Control, Optional ByVal newId As String = "") As String

        Dim sb As New StringBuilder()
        Dim tw As New StringWriter(sb)
        Dim hw As New HtmlTextWriter(tw)

        If newId <> "" Then
            ctrl.ID = newId
        End If

        ctrl.RenderControl(hw)
        Dim html = sb.ToString()
        Return html.Replace(vbCr, "").Replace(vbLf, "").Replace("""", "'").Replace(vbTab, "")

    End Function

#End Region

#Region "Classe di appoggio"



    Public Class Etichette_RispostaStandard
        Inherits RispostaStandard

        Public IsLink As Boolean

        Public UrlLink As String
        Public Lista_FF_Stampa_Dettagli_Cod As String

    End Class




#End Region

End Class


