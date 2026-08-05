Imports CrystalDecisions.Shared
Imports System.Resources.ResourceWriter
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreDataProvider
Imports AgronicaCoreStampeDAL
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider.My.Resources

Public Class Ricetta_Stampa

    Inherits System.Web.UI.Page
    Private rptStampa As New Rpt_Ricetta
    Private rptStampa3 As New Rpt_Ricetta_3
    Private rptStampaNew As New Rpt_Ricetta_New
    Private DsLavorazioni As New DS_Ricetta_Lavorazioni
    Private DsTrattamenti As New DS_Ricetta_Trattamenti
    Private DsAppezzamenti As New DS_Ricetta_Appezzamenti
    Private DsFertilizzazioni As New DS_Ricetta_Fertilizzazioni
    Private DsIrrigazioni As New DS_Ricetta_Irrigazioni
    Private DsTrappole As New DS_Ricetta_Trappole

    Private DSLavorazioni3 As New DS_Ricetta3_Lavorazioni
    Private DSTrattamenti3 As New DS_Ricetta3_Trattamenti

    'NPK Max tenuto per poter assegnare i residui nelle fertilizzazioni
    ' tengo i residui per le distribuzioni successive NB= modifciare il residuo solo nel momento utile (Inserisci Fertilizz.)
    Private N_Max As String = ""
    Private P_Max As String = ""
    Private K_Max As String = ""
    Private N_Residuo_tot As String = ""
    Private P_Residuo_tot As String = ""
    Private K_Residuo_tot As String = ""


    Dim Ricetta_Cod As Integer
    Dim Ricetta_Numero As String
    Dim Ricetta_Des As String
    Dim Ricetta_tipo As Integer
    Dim Ricetta_Programmazione_Cod As Integer
    Dim Piva As String
    Dim Rag_Soc As String
    Dim Note As String

    Dim Veg_Cod As Integer
    Dim Veg_Des As String

    Dim Qs_Ricetta_Cod As Integer
    Dim Qs_Stampa_Tipo As Integer
    '1=vecchia ricetta (orizzontale e raggruppata x varietà)
    '2=nuova ricetta (verticale con appezzamenti distinti)

    Dim Param_Rag_Soc As String = ""
    Dim Param_Piva_CUAA As String = ""
    Dim Param_Indirizzo As String = ""
    Dim Param_Intestazione As String = ""
    Dim Param_Coltura As String = ""
    Dim Param_Date As String = ""
    Dim Param_Note As String = ""

    Dim Log_Errori As String = ""

    'oggetto objparametri x server 
    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri


    Public Enum enum_TipoStampaRicetta
        Certificazione = 1
        Aziendale = 2
        PianoLavori = 3
    End Enum

#Region " Codice generato da Progettazione Web Form "

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
        'rptStampa = New Rpt_Ricetta
    End Sub

#End Region

    '###########################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        If Not IsNothing(Request.QueryString("ricetta_cod")) Then

            Qs_Ricetta_Cod = Stringa_Decodifica(Request.QueryString("ricetta_cod").ToString,
                                                AgroKey_EncoderDecoder,
                                                Server)
        ElseIf Not IsNothing(Session("Ricetta_Cod")) Then
            Qs_Ricetta_Cod = Session("Ricetta_Cod")
        End If

        If Not IsNothing(Request.QueryString("ricetta_stampa_tipo")) Then
            Qs_Stampa_Tipo = Stringa_Decodifica(Request.QueryString("ricetta_stampa_tipo").ToString,
                                                AgroKey_EncoderDecoder,
                                                Server)
        Else
            Qs_Stampa_Tipo = enum_TipoStampaRicetta.Certificazione
        End If


        'inizializzazione oggetti objParametri_Utenti e objParametri_Server
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))


        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer

        If Not Me.IsPostBack Then

            CrystalReportViewer1 = New CrystalDecisions.Web.CrystalReportViewer

            CrystalReportViewer1.Style.Add("LEFT", "0px")
            CrystalReportViewer1.Style.Add("TOP", "0px")
            CrystalReportViewer1.Style.Add("POSITION", "Absolute")
            Me.FindControl("Form1").Controls.Add(CrystalReportViewer1)


            '//////////////////////////////////////////

            Stampa_Ricetta(CrystalReportViewer1)

            '////////////////////////////////////////////


            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            Dim Nome_File As String

            If Log_Errori <> "" Then

                Log_Errori = "Ricetta" + ", Partita Iva = " + CStr(Piva) &
                                ", Qs_Ricetta_Cod = " + CStr(Qs_Ricetta_Cod) &
                                ", Qs_Stampa_Tipo = " + CStr(Qs_Stampa_Tipo) &
                               vbCrLf + vbCrLf + Log_Errori

                Nome_File = "Log_Errori_Ricetta" & Qs_Ricetta_Cod.ToString & "_tipo" & Qs_Stampa_Tipo.ToString

                Gestione_LogErrori_Stampe(objParametri_Server,
                                                 "Stampe_QDC",
                                                 Nome_File & ".txt",
                                                 Session("ASG_Utente_Username"),
                                                 "Ricetta_Stampa.aspx",
                                                 Log_Errori)

            End If
            '-----------------------------------------




            ''---------------------------------------------

            ''faccio il databind col visualizzatore dei reports...
            'CrystalReportViewer1.ReportSource = rptStampa
            'CrystalReportViewer1.DataBind()

            'array di dataset e data table
            Dim dsRpt() As DataSet = {DsTrattamenti}

            'salvo il report nella sessione
            Session("DS") = dsRpt

            Dim urlVisualizzatore = "..\..\Stampe\VisualizzatoreReport.aspx?anteprima=" & AgronicaCoreDataProvider.Sicurezza.Stringa_Codifica("1", AgroKey_EncoderDecoder, Nothing)
            Select Case Qs_Stampa_Tipo

                Case enum_TipoStampaRicetta.PianoLavori

                Case enum_TipoStampaRicetta.Certificazione
                    HttpContext.Current.Session("Report") = rptStampa
                    Response.Redirect(urlVisualizzatore)
                Case enum_TipoStampaRicetta.Aziendale
            End Select

        Else 'ad ogni post back devo impostare la fonte dati x il visualizzatore..

            If Not IsNothing(Session("DS")) Then

                'faccio il databind col visualizzatore dei reports...
                Select Case Qs_Stampa_Tipo

                    Case enum_TipoStampaRicetta.Certificazione
                        'imposto la sorgente dati x il report...
                        rptStampa.SetDataSource(CType(Session("DS")(0), DataSet))
                        CrystalReportViewer1.ReportSource = rptStampa
                    Case enum_TipoStampaRicetta.Aziendale
                        'imposto la sorgente dati x il report...
                        rptStampaNew.SetDataSource(CType(Session("DS")(0), DataSet))
                        CrystalReportViewer1.ReportSource = rptStampaNew
                    Case enum_TipoStampaRicetta.PianoLavori
                        'imposto la sorgente dati x il report...
                        rptStampaNew.SetDataSource(CType(Session("DS")(0), DataSet))
                        CrystalReportViewer1.ReportSource = rptStampa3
                End Select

                CrystalReportViewer1.DataBind()

            End If

        End If

        ''==================================================================


    End Sub


    '################################################################################
    Private Sub Gestione_LogErrori_Stampe(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                         ByVal Sottocartella As String,
                                         ByVal NomeFile_ConEstensione As String,
                                         ByVal IdentUtente As String,
                                         ByVal NomeRoutine As String,
                                         ByVal Log_Errori As String)

        Dim Path_Errore As String

        Dim objLog As New AgronicaCoreDataProvider.LogProvider

        Sottocartella = Replace(Sottocartella, "\", "-")
        Sottocartella = Replace(Sottocartella, "/", "-")

        NomeFile_ConEstensione = Replace(NomeFile_ConEstensione, "\", "-")
        NomeFile_ConEstensione = Replace(NomeFile_ConEstensione, "/", "-")
        NomeFile_ConEstensione = Replace(NomeFile_ConEstensione, """", "")

        If objParametri_Server.LogDirectory <> "" Then
            If Not objParametri_Server.LogDirectory.EndsWith("\") Then
                objParametri_Server.LogDirectory &= "\"
            End If
            Path_Errore = objParametri_Server.LogDirectory & Sottocartella
        Else
            If System.IO.Directory.Exists("C:\GIASLAN\Log") = True Then
                Path_Errore = "C:\GIASLAN\Log\" & Sottocartella
            ElseIf System.IO.Directory.Exists("C:\Agronica_LOG") = True Then
                Path_Errore = "C:\Agronica_LOG\" & Sottocartella
            Else
                Path_Errore = "C:\" & Sottocartella
            End If
        End If

        If Not Path_Errore.EndsWith("\") Then
            Path_Errore &= "\"
        End If

        If Path_Errore.Length > 259 Then
            Throw New Exception("Superata la lunghezza tra path e nome del file: " & CStr(Path_Errore.Length) & "caratteri (max 259 caratteri).")
        End If

        Dim CustomLOGParams As New AgronicaCoreDataProvider.CustomLOGParams With {
            .LogDescrizioneUtente = IdentUtente,
            .LogDirectory = Path_Errore,
            .LogFileName = NomeFile_ConEstensione
        }

        'objLog.Scrivi_LOG(Path_Errore, NomeFile_ConEstensione, IdentUtente, NomeRoutine, Log_Errori)
        objLog.Scrivi_LOG(objParametri_Server, NomeRoutine, Log_Errori, CustomLOGParams:=CustomLOGParams)


    End Sub



    '###########################################################################
    Private Sub Stampa_Ricetta(ByRef CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer)

        Dim i As Integer

        Dim StringaXmlRicetta As String

        Dim XmlDoc As New System.Xml.XmlDocument

        Dim XML_DatiRicetta As System.Xml.XmlElement
        Dim XML_Ricetta As System.Xml.XmlElement

        Dim XML_DatiRicettaxCultivar As System.Xml.XmlElement
        Dim XML_Cultivar As System.Xml.XmlElement
        Dim XMLs_Cultivar As System.Xml.XmlNodeList

        Dim XML_DatiOperazioni As System.Xml.XmlElement
        Dim XML_Operazione As System.Xml.XmlElement
        Dim XMLs_Operazione As System.Xml.XmlNodeList

        Dim Cul_Cod As Integer = -1
        Dim Cop_Cod As Integer = 0
        Dim strVarieta As String = ""
        Dim Data_Inizio As String
        Dim Data_Fine As String
        Dim Lav_Cod As Integer
        Dim Ricetta_Operazione_Cod As Integer

        Dim strOperazione As String

        Dim Cultivar As New AgronicaCoreMetaSchemaDAL.Cultivar_R

        Dim BaseCode As Integer = AgronicaCoreDataProvider.UtilityProvider.BaseCode_from_ProgressivoGias(Session("ASG_ProgressivoGIAS"))

        Dim bArrotondaAcqua As Boolean = False
        Dim bNascondiCodRicetta As Boolean = False
        Dim bNascondiFirmaAgricoltore As Boolean = False
        Dim bNascondiMacchine As Boolean = False
        Dim bNascondiOperatori As Boolean = False
        Dim bNascondiAutorizzatore As Boolean = False
        Dim bNascondiDataUltimaManutenzione As Boolean = False
        Dim frm_RicettaNote As String = ""
        Dim objTraduttore As Traduzione_Stampa_Ricette = Nothing

        Try

            '--------------------------------------------------
            ' Leggo le impostazioni dell'utente

            Dim objImpostazioniUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim Dt_Impostazioni As DataTable

            Dt_Impostazioni = objImpostazioniUtenti.Leggi(0, 1,
                                                          AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                          "", "",
                                                          objParametri_Utenti)

            ' Leggo le traduzioni

            Dim lingueDal As New AgronicaCoreUtentiDAL.Lingue_Read
            Dim linguaUtente = lingueDal.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaIso As String = "IT"
            If Not linguaUtente Is Nothing AndAlso linguaUtente.Rows.Count > 0 Then
                linguaIso = linguaUtente.AsEnumerable.FirstOrDefault().Item("CodiceIso").ToString
            End If

            objTraduttore = New Traduzione_Stampa_Ricette(String.Empty,
                                                           enum_CodificaStampe.SchedaCampagna_Ricette,
                                                           linguaIso, 0, objParametri_Server)
            objTraduttore.Traduci("", "")


            '(09/10/2017 fede) aggiunta verifica nome appezzamento + campo da indicare in base ad impostazione utente scheda campagna
            Dim TipoCodiceAppezzamento As Integer = 0
            Dim flag_nascondiCampo As Boolean = False

            Dim Dr() As DataRow

            Dr = Dt_Impostazioni.Select("Impostazione_Cod = " & enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_RICETTA_ARROTONDA_ACQUA)
            If Not Dr Is Nothing AndAlso Dr.Length > 0 Then
                If Dr(0).Item("Impostazione_Valore_1") = "1" Then
                    bArrotondaAcqua = True
                End If
            End If

            Dr = Dt_Impostazioni.Select("Impostazione_Cod = " & enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_RICETTA_NUMERO_RICETTA)
            If Not Dr Is Nothing AndAlso Dr.Length > 0 Then
                If Dr(0).Item("Impostazione_Valore_1") = "1" Then
                    bNascondiCodRicetta = True
                End If
            End If

            Dr = Dt_Impostazioni.Select("Impostazione_Cod = " & enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_RICETTA_FIRMA_AGRICOLTORE)
            If Not Dr Is Nothing AndAlso Dr.Length > 0 Then
                If Dr(0).Item("Impostazione_Valore_1") = "1" Then
                    bNascondiFirmaAgricoltore = True
                End If
            End If

            Dr = Dt_Impostazioni.Select("Impostazione_Cod = " & enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_RICETTA_MACCHINE)
            If Not Dr Is Nothing AndAlso Dr.Length > 0 Then
                If Dr(0).Item("Impostazione_Valore_1") = "1" Then
                    bNascondiMacchine = True
                End If
            End If
            If Qs_Stampa_Tipo = enum_TipoStampaRicetta.Aziendale Then
                bNascondiMacchine = True
            End If

            Dr = Dt_Impostazioni.Select("Impostazione_Cod = " & enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_RICETTA_OPERATORI)
            If Not Dr Is Nothing AndAlso Dr.Length > 0 Then
                If Dr(0).Item("Impostazione_Valore_1") = "1" Then
                    bNascondiOperatori = True
                End If
            End If

            Dr = Dt_Impostazioni.Select("Impostazione_Cod = " & enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_RICETTA_TECNICO_AUTORIZZANTE)
            If Not Dr Is Nothing AndAlso Dr.Length > 0 Then
                If Dr(0).Item("Impostazione_Valore_1") = "1" Then
                    bNascondiAutorizzatore = True
                End If
            End If

            Dr = Dt_Impostazioni.Select("Impostazione_Cod = " & enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_RICETTA_DATA_ULTIMA_MANUTENZIONE)
            If Not Dr Is Nothing AndAlso Dr.Length > 0 Then
                If Dr(0).Item("Impostazione_Valore_1") = "1" Then
                    bNascondiDataUltimaManutenzione = True
                End If
            End If

            Dr = Dt_Impostazioni.Select("Impostazione_Cod = " & enum_Impostazioni_Utenti.UTENTE_COD_SCHEDA_CAMPAGNA_NUM_APPEZZAMENTO)
            If Not Dr Is Nothing AndAlso Dr.Length > 0 Then
                If Not IsDBNull(Dr(0).Item("Impostazione_Valore_1")) AndAlso Dr(0).Item("Impostazione_Valore_1") <> "" Then
                    TipoCodiceAppezzamento = Dr(0).Item("Impostazione_Valore_1")
                End If
            End If

            Dr = Dt_Impostazioni.Select("Impostazione_Cod = " & enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_CAMPAGNA_NASCONDI_CAMPO)
            If Not Dr Is Nothing AndAlso Dr.Length > 0 Then
                If Dr(0).Item("Impostazione_Valore_1") = "1" Then
                    flag_nascondiCampo = True
                End If
            End If




            '--------------------------------------------------
            'carico  l'xml
            Dim objCOM As New AgronicaCoreContabBIZ.Ricette_R

            StringaXmlRicetta = objCOM.Ricetta_Leggi(CInt(Qs_Ricetta_Cod),
                                                        "",
                                                        0,
                                                        0,
                                                        0,
                                                        False,
                                                        objParametri_Server)


            '--------------------------------------------------


            'Carico la stringa nel documento XML
            XmlDoc.LoadXml(StringaXmlRicetta)

            '----- Tag DatiRicetta

            XML_DatiRicetta = XmlDoc.SelectSingleNode("DatiRicetta")

            '----- Tag Ricetta

            XML_Ricetta = XML_DatiRicetta.SelectSingleNode("Ricetta")

            'codice della ricetta da caricare
            Ricetta_Cod = CInt(XML_Ricetta.GetAttribute("ricetta_cod"))
            Ricetta_Des = CStr(XML_Ricetta.GetAttribute("ricetta_des"))
            Ricetta_Numero = CStr(XML_Ricetta.GetAttribute("ricetta_numero"))
            Ricetta_tipo = CInt(XML_Ricetta.GetAttribute("tipo_ricetta"))
            Ricetta_Programmazione_Cod = CInt(XML_Ricetta.GetAttribute("programmazione_cod"))

            Piva = XML_Ricetta.GetAttribute("piva")
            Rag_Soc = ""
            If Piva <> "" Then
                Dim ObjImpresa As New AgronicaCoreAnagrafeDAL.Imprese_Read
                Rag_Soc = ObjImpresa.RagSoc_from_Piva(XML_Ricetta.GetAttribute("piva"), objParametri_Server)
                ObjImpresa = Nothing
            End If

            Note = CStr(XML_Ricetta.GetAttribute("note"))

            '07/08/2018: patch per la stampa della ricetta della tab brogliaccio
            Data_Inizio = CDate(XML_Ricetta.GetAttribute("validita_inizio")).ToShortDateString
            Data_Fine = CDate(XML_Ricetta.GetAttribute("validita_fine")).ToShortDateString
            'If CDate(XML_Ricetta.GetAttribute("validita_inizio")).ToShortDateString <> "01/01/1900" Then
            '    Data_Inizio = CDate(XML_Ricetta.GetAttribute("validita_inizio")).ToShortDateString
            'Else
            '    Data_Inizio = "..."
            'End If

            'If CDate(XML_Ricetta.GetAttribute("validita_fine")).ToShortDateString <> "31/12/2100" Then
            '    Data_Fine = CDate(XML_Ricetta.GetAttribute("validita_fine")).ToShortDateString
            'Else
            '    Data_Fine = "..."
            'End If

            Veg_Cod = CInt(XML_Ricetta.GetAttribute("veg_cod"))

            If Veg_Cod > 0 Then
                Veg_Des = New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R().VegDes_from_VegCod(Veg_Cod, objParametri_Server)
            Else
                'se non è stata salvata la specie nella testata della ricetta la recupero dagli impianti
                Dim objDestinazioni As New AgronicaCoreContabDAL.Ricette_Destinazioni_R
                Dim Dt_Impianti As DataTable
                Dt_Impianti = objDestinazioni.LeggiConDettagliImpiantoReale(Ricetta_Cod,
                                         0, 0, 0, "", 0, 0, 0,
                                         True,
                                         True,
                                         Data_Inizio, Data_Fine,
                                          " RD.Id_Reg <> 0",
                                          "",
                                          objParametri_Server)
                If Not Dt_Impianti Is Nothing AndAlso Dt_Impianti.Rows.Count > 0 Then
                    Veg_Des = Dt_Impianti.Rows(0).Item("veg_des")
                    Veg_Cod = Dt_Impianti.Rows(0).Item("veg_cod")
                End If
            End If


            '--------------------------------------------------
            '----- VERIETA'

            '----- Tag DatiRicettaxCultivar

            XML_DatiRicettaxCultivar = XML_Ricetta.SelectSingleNode("DatiRicettaxCultivar")

            If Not IsNothing(XML_DatiRicettaxCultivar) Then

                '----- Tag Ricetta_Operazione (multiplo)

                XMLs_Cultivar = XML_DatiRicettaxCultivar.GetElementsByTagName("RicettaxCultivar")

                For i = 0 To XMLs_Cultivar.Count - 1

                    XML_Cultivar = XMLs_Cultivar.Item(i)

                    Cul_Cod = CInt(XML_Cultivar.GetAttribute("cul_cod"))

                    '(Cul_Cod = -1 ---> TUTTE LE VARIETA')
                    If Cul_Cod <> -1 Then

                        'carico le varietà la prima volta...
                        strVarieta &= Cultivar.CulDes_from_CulCod(Cul_Cod, objParametri_Server) & ","

                    End If

                Next

            End If

            If strVarieta <> "" Then
                'da eccezione, non c'è nel report ricette sez1 questo campo
                'CType(rptStampa.Section1.ReportObjects("TextVarieta"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Left(strVarieta, strVarieta.Length - 1)
            End If


            '''''''''''''''''''''''''NOTE'''''''''''''''''''
            Dim XMLs_Ricetta_Note As System.Xml.XmlNodeList
            Dim XML_DatiRicetta_Note As System.Xml.XmlElement
            Dim j As Integer

            '----- Tag DatiRicetta_Note
            Dim objNoteIntervento As New AgronicaCoreContabDAL.Note_Intervento_R

            XML_DatiRicetta_Note = XML_Ricetta.SelectSingleNode("DatiRicettaxNote")

            If Not XML_DatiRicetta_Note Is Nothing Then

                '----- Tag RicettaxNote
                XMLs_Ricetta_Note = XML_DatiRicetta_Note.GetElementsByTagName("RicettaxNote")

                If Not XMLs_Ricetta_Note Is Nothing AndAlso XMLs_Ricetta_Note.Count > 0 Then

                    For j = 0 To XMLs_Ricetta_Note.Count - 1

                        XML_DatiRicetta_Note = XMLs_Ricetta_Note.Item(j)

                        frm_RicettaNote &= objNoteIntervento.NotaDesFromNotaCod(
                                                                XML_DatiRicetta_Note.GetAttribute("nota_cod"),
                                                                Session("ASG_objParametri_Server")) & ", "

                    Next

                    If frm_RicettaNote <> "" Then
                        frm_RicettaNote = Left(frm_RicettaNote, frm_RicettaNote.Length - 2)
                    End If

                End If

            End If


            'RIEMPIMENTO DATASET

            '--------------------------------------------------
            '----- APPEZZAMENTI

            DsAppezzamenti = New DS_Ricetta_Appezzamenti

            DSLavorazioni3 = New DS_Ricetta3_Lavorazioni
            DSTrattamenti3 = New DS_Ricetta3_Trattamenti

            Dim Dt_Impianti_Trattamenti As DataTable
            Dim Dt_Impianti_Fertilizzazioni As DataTable
            Dim Dt_Impianti_Trappole As DataTable

            Select Case Qs_Stampa_Tipo
                Case enum_TipoStampaRicetta.Certificazione
                    If (Ricetta_tipo = enum_TipoRicetta.PianoDistribuzioneConcimi) Then
                        CaricaDsAppezzamenti(TipoCodiceAppezzamento, flag_nascondiCampo, BaseCode, Data_Inizio, Data_Fine)
                    Else
                        CaricaDsAppezzamenti_RaggruppatixVarieta(TipoCodiceAppezzamento, flag_nascondiCampo, BaseCode, Data_Inizio, Data_Fine, objTraduttore)
                    End If
                Case enum_TipoStampaRicetta.Aziendale
                    'nascondo forzatamente il campo perchè c'è già l'apposita colonna
                    CaricaDsAppezzamenti(TipoCodiceAppezzamento, True, BaseCode, Data_Inizio, Data_Fine)
                Case enum_TipoStampaRicetta.PianoLavori
                    CaricaDsAppezzamentiRicetta3(TipoCodiceAppezzamento, True, BaseCode, Data_Inizio, Data_Fine,
                                                 Dt_Impianti_Trattamenti, Dt_Impianti_Fertilizzazioni, Dt_Impianti_Trappole)
            End Select


            '--------------------------------------------------
            '----- OPERAZIONI

            '--- trattamenti
            DsTrattamenti = New DS_Ricetta_Trattamenti

            'CaricaDsTrattamenti(Ricetta_Cod)

            Crea_Dt_Trattamenti()

            '--- fertilizzazioni
            DsFertilizzazioni = New DS_Ricetta_Fertilizzazioni

            Crea_Dt_Fertilizzazioni()

            '--- irrigazioni
            DsIrrigazioni = New DS_Ricetta_Irrigazioni

            '--- trappole
            DsTrappole = New DS_Ricetta_Trappole

            DsLavorazioni = New DS_Ricetta_Lavorazioni

            '--------------------------------------------------
            '----- Tag DatiRicetta_Operazioni

            XML_DatiOperazioni = XML_Ricetta.SelectSingleNode("DatiRicetta_Operazioni")

            If Not XML_DatiOperazioni Is Nothing Then

                '----- Tag Ricetta_Operazione (multiplo)

                XMLs_Operazione = XML_DatiOperazioni.GetElementsByTagName("Ricetta_Operazione")


                For i = 0 To XMLs_Operazione.Count - 1

                    XML_Operazione = XMLs_Operazione.Item(i)

                    Lav_Cod = CInt(XML_Operazione.GetAttribute("lav_cod"))

                    Ricetta_Operazione_Cod = CInt(XML_Operazione.GetAttribute("ricetta_operazione_cod"))

                    strOperazione = XML_Operazione.OuterXml

                    Select Case Lav_Cod

                        Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_CONCIA_SEME, LAVCOD_GEODISINFESTAZIONE, LAVCOD_TRATTAMENTO_FITOREGOLATORE,
                             LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO,
                             LAVCOD_DISTRIBUZIONE_INSETTI, LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE

                            Inserisci_Trattamento(strOperazione, bArrotondaAcqua, bNascondiMacchine, bNascondiOperatori, bNascondiAutorizzatore, bNascondiDataUltimaManutenzione, TipoCodiceAppezzamento, flag_nascondiCampo, BaseCode)

                        Case LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_FERTIRRIGAZIONE, LAVCOD_TRATTAMENTO_ANTIBUTTERATURA, LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_DISTRIBUZIONE_AMMENDANTI, LAVCOD_SARCHIATURA_CONCIMAZIONE

                            Inserisci_Fertilizzazione(strOperazione, bNascondiMacchine, bNascondiOperatori, bNascondiAutorizzatore, bNascondiDataUltimaManutenzione, TipoCodiceAppezzamento, flag_nascondiCampo, BaseCode)

                        Case LAVCOD_IRRIGAZIONE

                            Inserisci_Irrigazione(strOperazione, TipoCodiceAppezzamento, flag_nascondiCampo, BaseCode)

                        Case LAVCOD_INSTALLAZIONE_TRAPPOLE,
                             LAVCOD_CATTURE_MASSA,
                             LAVCOD_CONFUSIONE_SESSUALE,
                             LAVCOD_DISORIENTAMENTO_SESSUALE

                            Inserisci_Trappole(strOperazione, TipoCodiceAppezzamento, flag_nascondiCampo, BaseCode)

                        Case Else

                            Inserisci_Lavorazione_Generica(strOperazione, TipoCodiceAppezzamento, flag_nascondiCampo, BaseCode)

                    End Select

                Next

            End If

            If Qs_Stampa_Tipo = enum_TipoStampaRicetta.PianoLavori Then

                Stampa_Ricetta_DatasetAppezzamentiStampa3(Dt_Impianti_Trattamenti, Dt_Impianti_Fertilizzazioni, Dt_Impianti_Trappole)

            End If 'STAMPA TIPO 3

        Catch ex As Exception
            Log_Errori &= "- stampa_ricetta: " & vbCrLf & ex.Message & vbCrLf
        End Try

        Try

            Riempimento_Report(CrystalReportViewer1,
                               bNascondiCodRicetta,
                               bNascondiFirmaAgricoltore,
                               Data_Inizio,
                               Data_Fine,
                               XML_Ricetta,
                               frm_RicettaNote, objTraduttore)



        Catch ex As Exception
            Log_Errori &= "- Riempimento_Report: " & vbCrLf & ex.Message & vbCrLf
        End Try


    End Sub


    '###########################################################################
    Private Sub Riempimento_Report_Tipo1(ByRef CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer,
                                   ByVal bNascondiCodRicetta As Boolean,
                                   ByVal bNascondiFirmaAgricoltore As Boolean,
                                    ByVal Data_Inizio As String,
                                    ByVal Data_Fine As String,
                                    ByVal XML_Ricetta As System.Xml.XmlElement,
                                    ByVal frm_RicettaNote As String, ByVal objTraduttore As Traduzione_Stampa_Ricette)


        Piva = XML_Ricetta.GetAttribute("piva")

        ' Faccio Merge dei dati delle colonne destinazioni e note

        ' Trattamenti
        MergeNoteAppezzamenti(DsTrattamenti.DS_Ricetta_Trattamenti, objTraduttore)
        ' Fertilizzazioni
        MergeNoteAppezzamenti(DsFertilizzazioni.DT_Ricetta_Fertilizzazioni, objTraduttore)
        ' Trappole
        MergeNoteAppezzamenti(DsTrappole.DT_Ricetta_Trappole, objTraduttore)
        ' Lavorazioni
        MergeNoteAppezzamenti(DsLavorazioni._DS_Ricetta_Lavorazioni, objTraduttore)

        Dim IndirizzoImpresa As String = ""
        ' Leggo dati impresa legale

        If Piva <> "" Then
            Dim ObjImpresa As New AgronicaCoreAnagrafeDAL.Imprese_Read
            Dim dtIntestazione = ObjImpresa.Leggi_x_anagrafica(Piva, "ii.tipo_indirizzo IN (1, 101)", "", objParametri_Server)
            ObjImpresa = Nothing
            If Not dtIntestazione Is Nothing AndAlso dtIntestazione.Rows.Count > 0 Then
                ComponiRagioneSociale(dtIntestazione, Piva, Rag_Soc)
                ComponiIndirizzo(dtIntestazione, IndirizzoImpresa)
            Else
                Rag_Soc = Rag_Soc & " ( " & Piva & " )"
            End If
        End If


        rptStampa = New Rpt_Ricetta

        Select Case objParametri_Server.PivaSuperUser
            Case "00055690382" 'ovierre
                CType(rptStampa.Section1.ReportObjects("PictureOvierre"), CrystalDecisions.CrystalReports.Engine.PictureObject).ObjectFormat.EnableSuppress = False
            Case Else
                CType(rptStampa.Section1.ReportObjects("PictureOvierre"), CrystalDecisions.CrystalReports.Engine.PictureObject).ObjectFormat.EnableSuppress = True
        End Select

        Dim logoItalianoVisible = False
        Dim logoFranceseVisible = False

        PersonalizzazioniConserve(logoFranceseVisible, logoItalianoVisible)
        rptStampa.ReportHeaderConservesFrance.SectionFormat.EnableSuppress = Not logoFranceseVisible
        rptStampa.ReportFooterConservesFrance.SectionFormat.EnableSuppress = Not logoFranceseVisible
        rptStampa.Section1.ReportObjects("TxtConservesFanceTitle").ObjectFormat.EnableSuppress = Not logoFranceseVisible

        rptStampa.ReportHeaderConservesItalia.SectionFormat.EnableSuppress = Not logoItalianoVisible


        If bNascondiCodRicetta Then
            If (Ricetta_tipo = enum_TipoRicetta.PianoDistribuzioneConcimi) Then
                CType(rptStampa.Section1.ReportObjects("TextIntestazione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Ricetta_Des
            Else
                '31/08/2018
                'CType(rptStampa.Section1.ReportObjects("TextIntestazione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Ricetta: " & Ricetta_Des
                CType(rptStampa.Section1.ReportObjects("TextIntestazione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = objTraduttore.Traduzioni.RptRicette.Titolo_Report_1
            End If
        Else
            If (Ricetta_tipo = enum_TipoRicetta.PianoDistribuzioneConcimi) Then
                CType(rptStampa.Section1.ReportObjects("TextIntestazione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Ricetta_Des
            Else
                '31/08/2018
                'CType(rptStampa.Section1.ReportObjects("TextIntestazione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Ricetta: " & Ricetta_Des & " Numero: " & Ricetta_Numero
                CType(rptStampa.Section1.ReportObjects("TextIntestazione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = objTraduttore.Traduzioni.RptRicette.Titolo_Report_2 & " " & Ricetta_Numero
            End If
        End If

        CType(rptStampa.Section1.ReportObjects("TextAzienda"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Rag_Soc
        CType(rptStampa.Section1.ReportObjects("TextAziendaInidirizzo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = IndirizzoImpresa

        CType(rptStampa.Section1.ReportObjects("TextNote"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Note

        If Data_Inizio <> Data_Fine Then
            CType(rptStampa.Section1.ReportObjects("TextData"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = objTraduttore.Traduzioni.RptRicette.Da & " " & Data_Inizio & " " & objTraduttore.Traduzioni.RptRicette.A & " " & Data_Fine
        Else
            CType(rptStampa.Section1.ReportObjects("TextData"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Data_Inizio
        End If

        Veg_Cod = CInt(XML_Ricetta.GetAttribute("veg_cod"))

        CType(rptStampa.Section1.ReportObjects("TextColtura"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Veg_Des.ToUpper()

        If CType(rptStampa.Section1.ReportObjects("TextNote"), CrystalDecisions.CrystalReports.Engine.TextObject).Text <> "" Then
            If frm_RicettaNote <> "" Then
                CType(rptStampa.Section1.ReportObjects("TextNote"), CrystalDecisions.CrystalReports.Engine.TextObject).Text &= ", " & frm_RicettaNote
            End If
        Else
            CType(rptStampa.Section1.ReportObjects("TextNote"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = frm_RicettaNote
        End If

        '-------------------------------------------------------
        ' Firma Agricoltore

        If bNascondiFirmaAgricoltore Then
            rptStampa.Section6.ReportObjects("LPFirmaAgricoltore1").ObjectFormat.EnableSuppress = True
            rptStampa.Section6.ReportObjects("LineaFirmaAgricoltore").ObjectFormat.EnableSuppress = True
        End If

        ''---------------------------------------------
        ''Visualizzo i sottoreport
        ''appezzamenti
        ''Mofica Simone Galassi: Se sono della ditrib. concimi uso un sottoreport diverso
        'If DsAppezzamenti.DT_Ricetta_Appezzamenti.Rows.Count > 0 Then
        '    If (Ricetta_tipo = enum_TipoRicetta.PianoDistribuzioneConcimi) Then
        '        rptStampa.OpenSubreport("Rpt_Ricetta_Appezzamenti_piano_distribuzione.rpt").SetDataSource(DsAppezzamenti)
        '        rptStampa.Section10.SectionFormat.EnableSuppress = True
        '    Else
        '        rptStampa.OpenSubreport("Rpt_Ricetta_Appezzamenti.rpt").SetDataSource(DsAppezzamenti)
        '        rptStampa.Section12.SectionFormat.EnableSuppress = True 'TOGLIERE COMMENTO QUI
        '    End If
        'Else
        '    rptStampa.Section10.SectionFormat.EnableSuppress = True
        '    rptStampa.Section12.SectionFormat.EnableSuppress = True 'TOGLIERE COMMENTO QUI
        'End If

        ''If DsTrattamenti.DS_Ricetta_Trattamenti.Rows.Count > 0 Then
        ''    rptStampa.OpenSubreport("Rpt_Ricetta_Trattamenti.rpt").SetDataSource(DsTrattamenti)
        ''Else
        ''    rptStampa.Section4.SectionFormat.EnableSuppress = True
        ''End If
        'rptStampa.Section4.SectionFormat.EnableSuppress = True

        'If DsTrattamenti.DS_Ricetta_Trattamenti.Rows.Count > 0 Then
        '    rptStampa.OpenSubreport("Rpt_Ricetta_Trattamenti_2.rpt").SetDataSource(DsTrattamenti)
        'Else
        '    rptStampa.Section11.SectionFormat.EnableSuppress = True
        'End If

        'If DsFertilizzazioni.DT_Ricetta_Fertilizzazioni.Rows.Count > 0 Then
        '    If (Ricetta_tipo = enum_TipoRicetta.PianoDistribuzioneConcimi) Then
        '        rptStampa.OpenSubreport("Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.rpt").SetDataSource(DsFertilizzazioni)
        '        rptStampa.Section7.SectionFormat.EnableSuppress = True
        '    Else
        '        rptStampa.OpenSubreport("Rpt_Ricetta_Fertilizzazioni.rpt").SetDataSource(DsFertilizzazioni)
        '        rptStampa.Section13.SectionFormat.EnableSuppress = True
        '    End If
        'Else
        '    rptStampa.Section7.SectionFormat.EnableSuppress = True
        '    rptStampa.Section13.SectionFormat.EnableSuppress = True
        'End If

        'If DsIrrigazioni.DS_Ricetta_Irrigazioni.Rows.Count > 0 Then
        '    rptStampa.OpenSubreport("Rpt_Ricetta_Irrigazioni.rpt").SetDataSource(DsIrrigazioni)
        'Else
        '    rptStampa.Section8.SectionFormat.EnableSuppress = True
        'End If

        'If DsTrappole.DT_Ricetta_Trappole.Rows.Count > 0 Then
        '    rptStampa.OpenSubreport("Rpt_Ricetta_Trappole.rpt").SetDataSource(DsTrappole)
        'Else
        '    rptStampa.Section9.SectionFormat.EnableSuppress = True
        'End If

        'If DsLavorazioni._DS_Ricetta_Lavorazioni.Rows.Count > 0 Then
        '    rptStampa.OpenSubreport("Rpt_Ricetta_Lavorazioni.rpt").SetDataSource(DsLavorazioni)
        'Else
        '    rptStampa.ReportFooterSection2.SectionFormat.EnableSuppress = True
        'End If

        Try

            rptStampa.OpenSubreport("Rpt_Ricetta_Appezzamenti_piano_distribuzione.rpt").SetDataSource(DsAppezzamenti)
            rptStampa.OpenSubreport("Rpt_Ricetta_Appezzamenti.rpt").SetDataSource(DsAppezzamenti)

            '   rptStampa.OpenSubreport("Rpt_Ricetta_Trattamenti.rpt").SetDataSource(DsTrattamenti)
            rptStampa.OpenSubreport("Rpt_Ricetta_Trattamenti_2.rpt").SetDataSource(DsTrattamenti)

            rptStampa.OpenSubreport("Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.rpt").SetDataSource(DsFertilizzazioni)
            rptStampa.OpenSubreport("Rpt_Ricetta_Fertilizzazioni.rpt").SetDataSource(DsFertilizzazioni)

            rptStampa.OpenSubreport("Rpt_Ricetta_Irrigazioni.rpt").SetDataSource(DsIrrigazioni)
            rptStampa.OpenSubreport("Rpt_Ricetta_Trappole.rpt").SetDataSource(DsTrappole)
            rptStampa.OpenSubreport("Rpt_Ricetta_Lavorazioni.rpt").SetDataSource(DsLavorazioni)


            ' Impostazione parametri localizzati in lingua
            rptStampa.SetParameterValue("LP_Coltura", objTraduttore.Traduzioni.RptRicette.Coltura)
            rptStampa.SetParameterValue("LP_Azienda", objTraduttore.Traduzioni.RptRicette.Azienda)
            rptStampa.SetParameterValue("LP_Data", objTraduttore.Traduzioni.RptRicette.Data)
            rptStampa.SetParameterValue("LP_Note", objTraduttore.Traduzioni.RptRicette.Note)
            rptStampa.SetParameterValue("LP_Ricetta_Numero", objTraduttore.Traduzioni.RptRicette.Numero)

            Dim dataVisita As String = String.Format("{0} {1}", objTraduttore.Traduzioni.RptRicette.DataVisita, Data_Inizio.ToString)
            rptStampa.SetParameterValue("LP_DataVisita", dataVisita)
            rptStampa.SetParameterValue("LP_Firma_Agricoltore", objTraduttore.Traduzioni.RptRicette.Firma_Agricoltore)
            rptStampa.SetParameterValue("LP_Firma_Responsabile_Tecnico", objTraduttore.Traduzioni.RptRicette.Firma_Responsabile_Tecnico)
            rptStampa.SetParameterValue("LP_Titolo_Principale", objTraduttore.Traduzioni.RptRicette.TitoloPrincipale)


            rptStampa.SetParameterValue("LP_Cultivar", objTraduttore.Traduzioni.Rpt_Ricetta_Appezzamenti.Cultivar, "Rpt_Ricetta_Appezzamenti.rpt")
            rptStampa.SetParameterValue("LP_Numero_Appezzamento", objTraduttore.Traduzioni.Rpt_Ricetta_Appezzamenti.Numero_Appezzamento, "Rpt_Ricetta_Appezzamenti.rpt")
            rptStampa.SetParameterValue("LP_Superficie_Totale", objTraduttore.Traduzioni.Rpt_Ricetta_Appezzamenti.Superficie_Totale, "Rpt_Ricetta_Appezzamenti.rpt")

            rptStampa.SetParameterValue("LP_Centro", objTraduttore.Traduzioni.Rpt_Ricetta_Appezzamenti_piano_distribuzione.Centro, "Rpt_Ricetta_Appezzamenti_piano_distribuzione.rpt")
            rptStampa.SetParameterValue("LP_Campo", objTraduttore.Traduzioni.Rpt_Ricetta_Appezzamenti_piano_distribuzione.Campo, "Rpt_Ricetta_Appezzamenti_piano_distribuzione.rpt")
            rptStampa.SetParameterValue("LP_Numero_Appezzamento", objTraduttore.Traduzioni.Rpt_Ricetta_Appezzamenti_piano_distribuzione.Numero_Appezzamento, "Rpt_Ricetta_Appezzamenti_piano_distribuzione.rpt")
            rptStampa.SetParameterValue("LP_Cultivar", objTraduttore.Traduzioni.Rpt_Ricetta_Appezzamenti_piano_distribuzione.Cultivar, "Rpt_Ricetta_Appezzamenti_piano_distribuzione.rpt")
            rptStampa.SetParameterValue("LP_Superficie_HA", objTraduttore.Traduzioni.Rpt_Ricetta_Appezzamenti_piano_distribuzione.Superficie_HA, "Rpt_Ricetta_Appezzamenti_piano_distribuzione.rpt")
            rptStampa.SetParameterValue("LP_N_Max", objTraduttore.Traduzioni.Rpt_Ricetta_Appezzamenti_piano_distribuzione.N_MAx, "Rpt_Ricetta_Appezzamenti_piano_distribuzione.rpt")
            rptStampa.SetParameterValue("LP_P_Max", objTraduttore.Traduzioni.Rpt_Ricetta_Appezzamenti_piano_distribuzione.P_Max, "Rpt_Ricetta_Appezzamenti_piano_distribuzione.rpt")
            rptStampa.SetParameterValue("LP_K_Max", objTraduttore.Traduzioni.Rpt_Ricetta_Appezzamenti_piano_distribuzione.K_Max, "Rpt_Ricetta_Appezzamenti_piano_distribuzione.rpt")

            rptStampa.SetParameterValue("LP_Intestazione", objTraduttore.Traduzioni.Rpt_Ricetta_Trattamenti_2.Intestazione, "Rpt_Ricetta_Trattamenti_2.rpt")
            rptStampa.SetParameterValue("LP_Data_Cons", objTraduttore.Traduzioni.Rpt_Ricetta_Trattamenti_2.Data_Cons, "Rpt_Ricetta_Trattamenti_2.rpt")
            rptStampa.SetParameterValue("LP_Num_appezzamento", objTraduttore.Traduzioni.Rpt_Ricetta_Trattamenti_2.Numero_appezzamento, "Rpt_Ricetta_Trattamenti_2.rpt")
            rptStampa.SetParameterValue("LP_Avversita", objTraduttore.Traduzioni.Rpt_Ricetta_Trattamenti_2.Avversita, "Rpt_Ricetta_Trattamenti_2.rpt")
            rptStampa.SetParameterValue("LP_Prodottto_Commerciale", objTraduttore.Traduzioni.Rpt_Ricetta_Trattamenti_2.Prodotto_Commerciale, "Rpt_Ricetta_Trattamenti_2.rpt")
            rptStampa.SetParameterValue("LP_Principi_Attivi", objTraduttore.Traduzioni.Rpt_Ricetta_Trattamenti_2.Principi_Attivi, "Rpt_Ricetta_Trattamenti_2.rpt")
            rptStampa.SetParameterValue("LP_Giorni_Carenza", objTraduttore.Traduzioni.Rpt_Ricetta_Trattamenti_2.Carenza, "Rpt_Ricetta_Trattamenti_2.rpt")
            rptStampa.SetParameterValue("LP_Data_Utile_Prima_Raccolta", objTraduttore.Traduzioni.Rpt_Ricetta_Trattamenti_2.Data_Utile_Prima_Raccolta, "Rpt_Ricetta_Trattamenti_2.rpt")
            rptStampa.SetParameterValue("LP_Tempo_Rientro", objTraduttore.Traduzioni.Rpt_Ricetta_Trattamenti_2.Tempo_Rientro, "Rpt_Ricetta_Trattamenti_2.rpt")
            rptStampa.SetParameterValue("LP_Dose", objTraduttore.Traduzioni.Rpt_Ricetta_Trattamenti_2.Dose, "Rpt_Ricetta_Trattamenti_2.rpt")
            rptStampa.SetParameterValue("LP_Qta_Totale", objTraduttore.Traduzioni.Rpt_Ricetta_Trattamenti_2.Qta_Totale, "Rpt_Ricetta_Trattamenti_2.rpt")
            rptStampa.SetParameterValue("LP_Vol_H2O", objTraduttore.Traduzioni.Rpt_Ricetta_Trattamenti_2.Volume_H2O, "Rpt_Ricetta_Trattamenti_2.rpt")
            rptStampa.SetParameterValue("LP_Note", objTraduttore.Traduzioni.Rpt_Ricetta_Trattamenti_2.Note, "Rpt_Ricetta_Trattamenti_2.rpt")


            rptStampa.SetParameterValue("LP_Intestazione", objTraduttore.Traduzioni.Rpt_Ricetta_Fertilizzazioni.Intestazione, "Rpt_Ricetta_Fertilizzazioni.rpt")
            rptStampa.SetParameterValue("LP_Data_Cons", objTraduttore.Traduzioni.Rpt_Ricetta_Fertilizzazioni.Data_Cons, "Rpt_Ricetta_Fertilizzazioni.rpt")
            rptStampa.SetParameterValue("LP_Numero_appezzamento", objTraduttore.Traduzioni.Rpt_Ricetta_Fertilizzazioni.Numero_appezzamento, "Rpt_Ricetta_Fertilizzazioni.rpt")
            rptStampa.SetParameterValue("LP_Prodotto_Commerciale", objTraduttore.Traduzioni.Rpt_Ricetta_Fertilizzazioni.Prodotto_Commerciale, "Rpt_Ricetta_Fertilizzazioni.rpt")
            rptStampa.SetParameterValue("LP_Titolo_NPK", objTraduttore.Traduzioni.Rpt_Ricetta_Fertilizzazioni.Titolo_NPK, "Rpt_Ricetta_Fertilizzazioni.rpt")
            rptStampa.SetParameterValue("LP_Dose", objTraduttore.Traduzioni.Rpt_Ricetta_Fertilizzazioni.Dose, "Rpt_Ricetta_Fertilizzazioni.rpt")
            rptStampa.SetParameterValue("LP_Qta_Totale", objTraduttore.Traduzioni.Rpt_Ricetta_Fertilizzazioni.Qta_Totale, "Rpt_Ricetta_Fertilizzazioni.rpt")
            rptStampa.SetParameterValue("LP_Note", objTraduttore.Traduzioni.Rpt_Ricetta_Fertilizzazioni.Note, "Rpt_Ricetta_Fertilizzazioni.rpt")

            rptStampa.SetParameterValue("LP_Intestazione", objTraduttore.Traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.Intestazione, "Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.rpt")
            rptStampa.SetParameterValue("LP_Data_Cons", objTraduttore.Traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.Data_Cons, "Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.rpt")
            rptStampa.SetParameterValue("LP_Numero_appezzamento", objTraduttore.Traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.Numero_appezzamento, "Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.rpt")
            rptStampa.SetParameterValue("LP_Prodotto_Commerciale", objTraduttore.Traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.Prodotto_Commerciale, "Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.rpt")
            rptStampa.SetParameterValue("LP_Titolo_NPK", objTraduttore.Traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.Titolo_NPK, "Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.rpt")
            rptStampa.SetParameterValue("LP_Dose", objTraduttore.Traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.Dose, "Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.rpt")
            rptStampa.SetParameterValue("LP_Qta_Totale", objTraduttore.Traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.Qta_Totale, "Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.rpt")
            rptStampa.SetParameterValue("LP_N_Distribuzione", objTraduttore.Traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.N_Distribuzione, "Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.rpt")
            rptStampa.SetParameterValue("LP_N_Resid", objTraduttore.Traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.N_Resid, "Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.rpt")
            rptStampa.SetParameterValue("LP_P_Distribuzione", objTraduttore.Traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.P_Distribuzione, "Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.rpt")
            rptStampa.SetParameterValue("LP_P_Resid", objTraduttore.Traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.P_Resid, "Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.rpt")
            rptStampa.SetParameterValue("LP_K_Distribuzione", objTraduttore.Traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.K_Distribuzione, "Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.rpt")
            rptStampa.SetParameterValue("LP_K_Resid", objTraduttore.Traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.K_Resid, "Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.rpt")
            rptStampa.SetParameterValue("LP_Note", objTraduttore.Traduzioni.Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.Note, "Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.rpt")

            rptStampa.SetParameterValue("LP_Intestazione", objTraduttore.Traduzioni.Rpt_Ricetta_Irrigazioni.Intestazione, "Rpt_Ricetta_Irrigazioni.rpt")
            rptStampa.SetParameterValue("LP_Data_Cons", objTraduttore.Traduzioni.Rpt_Ricetta_Irrigazioni.Data_Cons, "Rpt_Ricetta_Irrigazioni.rpt")
            rptStampa.SetParameterValue("LP_Numero_appezzamento", objTraduttore.Traduzioni.Rpt_Ricetta_Irrigazioni.Numero_appezzamento, "Rpt_Ricetta_Irrigazioni.rpt")
            rptStampa.SetParameterValue("LP_Dose", objTraduttore.Traduzioni.Rpt_Ricetta_Irrigazioni.Dose, "Rpt_Ricetta_Irrigazioni.rpt")
            rptStampa.SetParameterValue("LP_Unita_Misura", objTraduttore.Traduzioni.Rpt_Ricetta_Irrigazioni.Unita_Misura, "Rpt_Ricetta_Irrigazioni.rpt")
            rptStampa.SetParameterValue("LP_Ore", objTraduttore.Traduzioni.Rpt_Ricetta_Irrigazioni.Ore, "Rpt_Ricetta_Irrigazioni.rpt")
            rptStampa.SetParameterValue("LP_Portata", objTraduttore.Traduzioni.Rpt_Ricetta_Irrigazioni.Portata, "Rpt_Ricetta_Irrigazioni.rpt")
            rptStampa.SetParameterValue("LP_Periodo_Turno_Medio", objTraduttore.Traduzioni.Rpt_Ricetta_Irrigazioni.Periodo_Turno_Medio, "Rpt_Ricetta_Irrigazioni.rpt")
            rptStampa.SetParameterValue("LP_Giorni", objTraduttore.Traduzioni.Rpt_Ricetta_Irrigazioni.Giorni, "Rpt_Ricetta_Irrigazioni.rpt")

            rptStampa.SetParameterValue("LP_Intestazione", objTraduttore.Traduzioni.Rpt_Ricetta_Trappole.Intestazione, "Rpt_Ricetta_Trappole.rpt")
            rptStampa.SetParameterValue("LP_Data_Cons", objTraduttore.Traduzioni.Rpt_Ricetta_Trappole.Data_Cons, "Rpt_Ricetta_Trappole.rpt")
            rptStampa.SetParameterValue("LP_Numero_appezzamento", objTraduttore.Traduzioni.Rpt_Ricetta_Trappole.Numero_appezzamento, "Rpt_Ricetta_Trappole.rpt")
            rptStampa.SetParameterValue("LP_Lavorazione", objTraduttore.Traduzioni.Rpt_Ricetta_Trappole.Lavorazione, "Rpt_Ricetta_Trappole.rpt")
            rptStampa.SetParameterValue("LP_Trappola", objTraduttore.Traduzioni.Rpt_Ricetta_Trappole.Trappola, "Rpt_Ricetta_Trappole.rpt")
            rptStampa.SetParameterValue("LP_Ditta", objTraduttore.Traduzioni.Rpt_Ricetta_Trappole.Ditta, "Rpt_Ricetta_Trappole.rpt")
            rptStampa.SetParameterValue("LP_Avversita", objTraduttore.Traduzioni.Rpt_Ricetta_Trappole.Avversita, "Rpt_Ricetta_Trappole.rpt")
            rptStampa.SetParameterValue("LP_Qta", objTraduttore.Traduzioni.Rpt_Ricetta_Trappole.Qta, "Rpt_Ricetta_Trappole.rpt")
            rptStampa.SetParameterValue("LP_Note", objTraduttore.Traduzioni.Rpt_Ricetta_Trappole.note, "Rpt_Ricetta_Trappole.rpt")

            rptStampa.SetParameterValue("LP_Intestazione", objTraduttore.Traduzioni.Rpt_Ricetta_Lavorazioni.Intestazione, "Rpt_Ricetta_Lavorazioni.rpt")
            rptStampa.SetParameterValue("LP_Data_Cons", objTraduttore.Traduzioni.Rpt_Ricetta_Lavorazioni.Data_Cons, "Rpt_Ricetta_Lavorazioni.rpt")
            rptStampa.SetParameterValue("LP_Numero_appezzamento", objTraduttore.Traduzioni.Rpt_Ricetta_Lavorazioni.Numero_appezzamento, "Rpt_Ricetta_Lavorazioni.rpt")
            rptStampa.SetParameterValue("LP_Operazione", objTraduttore.Traduzioni.Rpt_Ricetta_Lavorazioni.Operazione, "Rpt_Ricetta_Lavorazioni.rpt")
            rptStampa.SetParameterValue("LP_Note", objTraduttore.Traduzioni.Rpt_Ricetta_Lavorazioni.Note, "Rpt_Ricetta_Lavorazioni.rpt")

        Catch ex As Exception
            Log_Errori &= "- SetDataSource sottoreport: " & vbCrLf & ex.Message & vbCrLf
        End Try

        Try

            '---------------------------------------------
            'Visualizzo i sottoreport
            If DsAppezzamenti.DT_Ricetta_Appezzamenti.Rows.Count > 0 Then
                If (Ricetta_tipo = enum_TipoRicetta.PianoDistribuzioneConcimi) Then
                    rptStampa.Section10.SectionFormat.EnableSuppress = True
                Else
                    rptStampa.Section12.SectionFormat.EnableSuppress = True 'TOGLIERE COMMENTO QUI
                End If
            Else
                rptStampa.Section10.SectionFormat.EnableSuppress = True
                rptStampa.Section12.SectionFormat.EnableSuppress = True 'TOGLIERE COMMENTO QUI
            End If

            'If DsTrattamenti.DS_Ricetta_Trattamenti.Rows.Count > 0 Then
            '    rptStampa.OpenSubreport("Rpt_Ricetta_Trattamenti.rpt").SetDataSource(DsTrattamenti)
            'Else
            '    rptStampa.Section4.SectionFormat.EnableSuppress = True
            'End If
            rptStampa.Section4.SectionFormat.EnableSuppress = True

            If DsTrattamenti.DS_Ricetta_Trattamenti.Rows.Count = 0 Then
                rptStampa.Section11.SectionFormat.EnableSuppress = True
            End If

            If DsFertilizzazioni.DT_Ricetta_Fertilizzazioni.Rows.Count > 0 Then
                If (Ricetta_tipo = enum_TipoRicetta.PianoDistribuzioneConcimi) Then
                    rptStampa.Section7.SectionFormat.EnableSuppress = True
                Else
                    rptStampa.Section13.SectionFormat.EnableSuppress = True
                End If
            Else
                rptStampa.Section7.SectionFormat.EnableSuppress = True
                rptStampa.Section13.SectionFormat.EnableSuppress = True
            End If

            If DsIrrigazioni.DS_Ricetta_Irrigazioni.Rows.Count = 0 Then
                rptStampa.Section8.SectionFormat.EnableSuppress = True
            End If

            If DsTrappole.DT_Ricetta_Trappole.Rows.Count = 0 Then
                rptStampa.Section9.SectionFormat.EnableSuppress = True
            End If

            If DsLavorazioni._DS_Ricetta_Lavorazioni.Rows.Count = 0 Then
                rptStampa.ReportFooterSection2.SectionFormat.EnableSuppress = True
            End If

        Catch ex As Exception
            Log_Errori &= "- visibilità sottoreport: " & vbCrLf & ex.Message & vbCrLf
        End Try


        '---------------------------------------------

        'faccio il databind col visualizzatore dei reports...
        'CrystalReportViewer1.DisplayToolbar = True
        CrystalReportViewer1.ReportSource = rptStampa
        CrystalReportViewer1.DataBind()


        '==================================================================

        ' Dichiara le variabili e restituisce le opzioni di esportazione.
        Dim exportOpts As New ExportOptions
        Dim diskOpts As New DiskFileDestinationOptions
        exportOpts = rptStampa.ExportOptions

        ' Imposta il formato di esportazione.
        exportOpts.ExportFormatType = ExportFormatType.PortableDocFormat
        exportOpts.ExportDestinationType = ExportDestinationType.DiskFile

        ' Imposta le opzioni relative al file del disco.
        Dim strPath As String
        Dim PathFileTemporanei As String

        Dim objAgroWebConfig As New AgroWebConfig

        If objAgroWebConfig.PathFileTemporanei = "" Then
            PathFileTemporanei = Server.MapPath("../Stampa/File_Temporanei")
        Else
            PathFileTemporanei = objAgroWebConfig.PathFileTemporanei
        End If

        strPath = PathFileTemporanei & "\Ricetta_" & Session.SessionID.ToString & ".pdf"
        diskOpts.DiskFileName = strPath
        exportOpts.DestinationOptions = diskOpts

        rptStampa.PrintOptions.PaperOrientation = PaperOrientation.Landscape

        Dim NomeStampante As String
        If objAgroWebConfig.NomeStampante <> "" Then
            NomeStampante = objAgroWebConfig.PathFileTemporanei
            rptStampa.PrintOptions.PrinterName = NomeStampante
        End If
    End Sub

    '###########################################################################
    Private Sub Riempimento_Report_Tipo2(ByRef CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer,
                                   ByVal bNascondiCodRicetta As Boolean,
                                   ByVal bNascondiFirmaAgricoltore As Boolean,
                                      ByVal Data_Inizio As String,
                                    ByVal Data_Fine As String,
                                    ByVal XML_Ricetta As System.Xml.XmlElement,
                                    ByVal frm_RicettaNote As String)


        rptStampaNew = New Rpt_Ricetta_New

        '31/08/2018
        'If bNascondiCodRicetta Then
        '    CType(rptStampaNew.Section1.ReportObjects("TextIntestazione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Ricetta: " & Ricetta_Des
        'Else
        '    CType(rptStampaNew.Section1.ReportObjects("TextIntestazione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Ricetta: " & Ricetta_Des & " Numero: " & Ricetta_Numero
        'End If
        If bNascondiCodRicetta Then
            CType(rptStampaNew.Section1.ReportObjects("TextIntestazione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "RICETTA"
        Else
            CType(rptStampaNew.Section1.ReportObjects("TextIntestazione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "RICETTA Numero: " & Ricetta_Numero
        End If

        CType(rptStampaNew.Section1.ReportObjects("TextAzienda"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Rag_Soc

        CType(rptStampaNew.Section1.ReportObjects("TextNote"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Note

        If Data_Inizio <> Data_Fine Then
            CType(rptStampaNew.Section1.ReportObjects("TextData"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Da " & Data_Inizio & " a " & Data_Fine
        Else
            CType(rptStampaNew.Section1.ReportObjects("TextData"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Data_Inizio
        End If

        CType(rptStampaNew.Section1.ReportObjects("TextColtura"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Veg_Des

        If CType(rptStampaNew.Section1.ReportObjects("TextNote"), CrystalDecisions.CrystalReports.Engine.TextObject).Text <> "" Then
            If frm_RicettaNote <> "" Then
                CType(rptStampaNew.Section1.ReportObjects("TextNote"), CrystalDecisions.CrystalReports.Engine.TextObject).Text &= ", " & frm_RicettaNote
            End If
        Else
            CType(rptStampaNew.Section1.ReportObjects("TextNote"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = frm_RicettaNote
        End If


        ''---------------------------------------------
        ''Visualizzo i sottoreport
        ''appezzamenti
        'If DsAppezzamenti.DT_Ricetta_Appezzamenti.Rows.Count > 0 Then
        '    rptStampaNew.OpenSubreport("Rpt_Ricetta_Appezzamenti_New.rpt").SetDataSource(DsAppezzamenti)
        'Else
        '    rptStampaNew.Section4.SectionFormat.EnableSuppress = True
        'End If

        '''If DsTrattamenti.DS_Ricetta_Trattamenti.Rows.Count > 0 Then
        '''    rptStampa.OpenSubreport("Rpt_Ricetta_Trattamenti.rpt").SetDataSource(DsTrattamenti)
        '''Else
        '''    rptStampa.Section4.SectionFormat.EnableSuppress = True
        '''End If

        ''rptStampaNew.ReportFooterSection1.SectionFormat.EnableSuppress = True
        'If DsTrattamenti.DS_Ricetta_Trattamenti.Rows.Count > 0 Then
        '    rptStampaNew.OpenSubreport("Rpt_Ricetta_Trattamenti_New.rpt").SetDataSource(DsTrattamenti)
        'Else
        '    rptStampaNew.ReportFooterSection1.SectionFormat.EnableSuppress = True
        'End If


        ''If DsFertilizzazioni.DS_Ricetta_Fertilizzazioni.Rows.Count > 0 Then
        ''    rptStampa.OpenSubreport("Rpt_Ricetta_Fertilizzazioni.rpt").SetDataSource(DsFertilizzazioni)
        ''Else
        ''    rptStampa.Section7.SectionFormat.EnableSuppress = True
        ''End If

        ''If DsIrrigazioni.DS_Ricetta_Irrigazioni.Rows.Count > 0 Then
        ''    rptStampa.OpenSubreport("Rpt_Ricetta_Irrigazioni.rpt").SetDataSource(DsIrrigazioni)
        ''Else
        ''    rptStampa.Section8.SectionFormat.EnableSuppress = True
        ''End If

        ''If DsTrappole.DS_Ricetta_Trappole.Rows.Count > 0 Then
        ''    rptStampa.OpenSubreport("Rpt_Ricetta_Trappole.rpt").SetDataSource(DsTrappole)
        ''Else
        ''    rptStampa.Section9.SectionFormat.EnableSuppress = True
        ''End If

        ''If DsLavorazioni._DS_Ricetta_Lavorazioni.Rows.Count > 0 Then
        ''    rptStampa.OpenSubreport("Rpt_Ricetta_Lavorazioni.rpt").SetDataSource(DsLavorazioni)
        ''Else
        ''    rptStampa.ReportFooterSection2.SectionFormat.EnableSuppress = True
        ''End If

        Try

            rptStampaNew.OpenSubreport("Rpt_Ricetta_Appezzamenti_New.rpt").SetDataSource(DsAppezzamenti)

            rptStampaNew.OpenSubreport("Rpt_Ricetta_Trattamenti_New.rpt").SetDataSource(DsTrattamenti)

        Catch ex As Exception
            Log_Errori &= "- SetDataSource sottoreport: " & vbCrLf & ex.Message & vbCrLf
        End Try


        Try

            'Visualizzo i sottoreport

            If DsAppezzamenti.DT_Ricetta_Appezzamenti.Rows.Count = 0 Then
                rptStampaNew.Section4.SectionFormat.EnableSuppress = True
            End If
            'rptStampaNew.ReportFooterSection1.SectionFormat.EnableSuppress = True
            If DsTrattamenti.DS_Ricetta_Trattamenti.Rows.Count = 0 Then
                rptStampaNew.ReportFooterSection1.SectionFormat.EnableSuppress = True
            End If

        Catch ex As Exception
            Log_Errori &= "- visibilità sottoreport: " & vbCrLf & ex.Message & vbCrLf
        End Try


        '---------------------------------------------

        'faccio il databind col visualizzatore dei reports...
        'CrystalReportViewer1.DisplayToolbar = True
        CrystalReportViewer1.ReportSource = rptStampaNew
        CrystalReportViewer1.DataBind()


        '==================================================================

        ' Dichiara le variabili e restituisce le opzioni di esportazione.
        Dim exportOpts As New ExportOptions
        Dim diskOpts As New DiskFileDestinationOptions
        exportOpts = rptStampaNew.ExportOptions

        ' Imposta il formato di esportazione.
        exportOpts.ExportFormatType = ExportFormatType.PortableDocFormat
        exportOpts.ExportDestinationType = ExportDestinationType.DiskFile

        ' Imposta le opzioni relative al file del disco.
        Dim strPath As String
        Dim PathFileTemporanei As String

        Dim objAgroWebConfig As New AgroWebConfig

        If objAgroWebConfig.PathFileTemporanei = "" Then
            PathFileTemporanei = Server.MapPath("../Stampa/File_Temporanei")
        Else
            PathFileTemporanei = objAgroWebConfig.PathFileTemporanei
        End If

        strPath = PathFileTemporanei & "\Ricetta_" & Session.SessionID.ToString & ".pdf"
        diskOpts.DiskFileName = strPath
        exportOpts.DestinationOptions = diskOpts

        'rptStampa.PrintOptions.PaperOrientation = PaperOrientation.Landscape

        Dim NomeStampante As String
        If objAgroWebConfig.NomeStampante <> "" Then
            NomeStampante = objAgroWebConfig.PathFileTemporanei
            rptStampaNew.PrintOptions.PrinterName = NomeStampante
        End If

        ' Esportazione del report.
        rptStampaNew.Export()

        ' Con il seguente codice il file pdf viene scritto 
        '  nel browser del client.
        Response.ClearContent()
        Response.ClearHeaders()
        Response.ContentType = "application/pdf"
        Response.WriteFile(strPath)
        Response.Flush()
        Response.Close()

        ' il file esportato viene eliminato dal disco
        System.IO.File.Delete(strPath)



    End Sub



    '###########################################################################
    Private Sub Stampa_Ricetta_DatasetAppezzamentiStampa3(ByVal Dt_Impianti_Trattamenti As DataTable,
                                                            ByVal Dt_Impianti_Fertilizzazioni As DataTable,
                                                            ByVal Dt_Impianti_Trappole As DataTable)

        'PrimaRigaAppezza è il campo del dataset utilizzato per capire se si tratta di righe dell'operazione
        ' o di righe dell'appezzamento, qual è la prima, ecc (per formule sul report)

        Dim Ric_op_Cod As Integer = 0
        Dim Memo_Ric_op_Cod As Integer = 0

        Try

            '///////////////////////////////////////
            'SEZIONE TRATTAMENTI - aggiungo i record degli appezzamenti
            '(in fondo, devono essere visualizzati per ultimi, dopo i prodotti)

            If Not IsNothing(Dt_Impianti_Trattamenti) Then
                Dim DrTratt As DS_Ricetta3_Trattamenti.DT_Ricetta3_TrattamentiRow
                For j = 0 To Dt_Impianti_Trattamenti.Rows.Count - 1

                    DrTratt = DSTrattamenti3.DT_Ricetta3_Trattamenti.NewDT_Ricetta3_TrattamentiRow

                    Ric_op_Cod = Dt_Impianti_Trattamenti.Rows(j).Item("Ricetta_Operazione_Cod")

                    If Memo_Ric_op_Cod <> Ric_op_Cod Then
                        'primo record o nuovo
                        DrTratt.PrimaRigaAppezza = 1
                        Memo_Ric_op_Cod = Ric_op_Cod
                    Else
                        DrTratt.PrimaRigaAppezza = 2
                    End If

                    'If j = 0 Then 'questo non va bene, perchè lo valorizzava solo al primo record, invece
                    'serve al primo appezza di ogni ricetta_operazione_cod
                    '    DrTratt.PrimaRigaAppezza = 1
                    'Else
                    '    DrTratt.PrimaRigaAppezza = 2
                    'End If

                    DrTratt.Pro_Cod = 0
                    DrTratt.Ricetta_Operazione_Cod = Dt_Impianti_Trattamenti.Rows(j).Item("Ricetta_Operazione_Cod")
                    DrTratt.Sa_Nome = Dt_Impianti_Trattamenti.Rows(j).Item("sa_nome")
                    DrTratt.Campo_Nome = Dt_Impianti_Trattamenti.Rows(j).Item("campo_des")
                    DrTratt.App_nome = Dt_Impianti_Trattamenti.Rows(j).Item("app_nome")
                    DrTratt.Veg_Des = Dt_Impianti_Trattamenti.Rows(j).Item("veg_Des")
                    DrTratt.Cul_Des = Dt_Impianti_Trattamenti.Rows(j).Item("Cul_Des")

                    'DrTratt.Sup_App = CDec(Dt_Impianti_Trattamenti.Rows(j).Item("sup_imp"))
                    'Anna 16/05/22: [rif. chiamata 18108] Nella stampa la colonna Ha viene fuori la Superficie selezionata e non quella trattata
                    DrTratt.Sup_App = CDec(Dt_Impianti_Trattamenti.Rows(j).Item("Qta2"))

                    DrTratt.Regolamento = Dt_Impianti_Trattamenti.Rows(j).Item("reg_dpi")
                    DrTratt.Indirizzo = Dt_Impianti_Trattamenti.Rows(j).Item("Appezza_Indirizzo")
                    DrTratt.Telefono = Dt_Impianti_Trattamenti.Rows(j).Item("Appezza_Rubrica")

                    DrTratt.Data = ""
                    DrTratt.Av_Des = ""
                    DrTratt.Fr_Des = ""
                    DrTratt.Pa_Des = ""
                    DrTratt.Carenza = ""
                    DrTratt.Data_Raccolta = ""
                    DrTratt.Tempo_Rientro = ""
                    DrTratt.Qta = ""
                    DrTratt.Qta_Tot = ""
                    DrTratt.Qta_Acqua = ""
                    DrTratt.Note = ""

                    DSTrattamenti3.DT_Ricetta3_Trattamenti.Rows.Add(DrTratt)

                Next
            End If
            '//////////////////////////////////////

        Catch ex2 As Exception
            Log_Errori &= "TRATTAMENTI: carica appezzamenti: " & vbCrLf & ex2.Message & vbCrLf
        End Try

        Ric_op_Cod = 0
        Memo_Ric_op_Cod = 0

        Try

            '///////////////////////////////////////
            'SEZIONE FERTILIZZAZIONI - aggiungo i record degli appezzamenti
            '(in fondo, devono essere visualizzati per ultimi, dopo i prodotti)

            If Not IsNothing(Dt_Impianti_Fertilizzazioni) Then
                Dim DrFert As DS_Ricetta_Fertilizzazioni.DT_Ricetta_FertilizzazioniRow
                For j = 0 To Dt_Impianti_Fertilizzazioni.Rows.Count - 1

                    DrFert = DsFertilizzazioni.DT_Ricetta_Fertilizzazioni.NewDT_Ricetta_FertilizzazioniRow

                    Ric_op_Cod = Dt_Impianti_Fertilizzazioni.Rows(j).Item("Ricetta_Operazione_Cod")

                    If Memo_Ric_op_Cod <> Ric_op_Cod Then
                        'primo record o nuovo
                        DrFert.PrimaRigaAppezza = 1
                        Memo_Ric_op_Cod = Ric_op_Cod
                    Else
                        DrFert.PrimaRigaAppezza = 2
                    End If

                    'If j = 0 Then 'questo non va bene, perchè lo valorizzava solo al primo record, invece
                    'serve al primo appezza di ogni ricetta_operazione_cod
                    '    DrFert.PrimaRigaAppezza = 1
                    'Else
                    '    DrFert.PrimaRigaAppezza = 2
                    'End If

                    DrFert.Pro_Cod = 0
                    DrFert.Ricetta_Operazione_Cod = Dt_Impianti_Fertilizzazioni.Rows(j).Item("Ricetta_Operazione_Cod")
                    DrFert.Sa_Nome = Dt_Impianti_Fertilizzazioni.Rows(j).Item("sa_nome")
                    DrFert.Campo_Nome = Dt_Impianti_Fertilizzazioni.Rows(j).Item("campo_des")
                    DrFert.App_nome = Dt_Impianti_Fertilizzazioni.Rows(j).Item("app_nome")
                    DrFert.Veg_Des = Dt_Impianti_Fertilizzazioni.Rows(j).Item("veg_Des")
                    DrFert.Cul_Des = Dt_Impianti_Fertilizzazioni.Rows(j).Item("Cul_Des")

                    'DrFert.Sup_App = CDec(Dt_Impianti_Fertilizzazioni.Rows(j).Item("sup_imp"))
                    'Anna 16/05/22: [rif. chiamata 18108] Nella stampa la colonna Ha viene fuori la Superficie selezionata e non quella trattata
                    DrFert.Sup_App = CDec(Dt_Impianti_Trattamenti.Rows(j).Item("Qta2"))

                    DrFert.Regolamento = Dt_Impianti_Fertilizzazioni.Rows(j).Item("reg_dpi")
                    DrFert.Indirizzo = Dt_Impianti_Fertilizzazioni.Rows(j).Item("Appezza_Indirizzo")
                    DrFert.Telefono = Dt_Impianti_Fertilizzazioni.Rows(j).Item("Appezza_Rubrica")

                    DrFert.Data = ""
                    DrFert.Fer_Des = ""
                    DrFert.Titolo = ""
                    DrFert.Qta = ""
                    DrFert.Qta_Tot = ""
                    DrFert.Note = ""

                    DsFertilizzazioni.DT_Ricetta_Fertilizzazioni.Rows.Add(DrFert)

                Next
            End If
            '//////////////////////////////////////


        Catch ex2 As Exception
            Log_Errori &= "FERTILIZZAZIONI: carica appezzamenti: " & vbCrLf & ex2.Message & vbCrLf
        End Try


        Ric_op_Cod = 0
        Memo_Ric_op_Cod = 0

        Try
            '!!!!!! non sono riuscita a debuggare le trappole perchè non si inseriscono trappole nella ricetta aziendale !!!!
            ' anche el irrigazioni, che quidi non ho revisionato

            '///////////////////////////////////////
            'SEZIONE TRAPPOLE - aggiungo i record degli appezzamenti
            '(in fondo, devono essere visualizzati per ultimi, dopo i prodotti)

            If Not IsNothing(Dt_Impianti_Trappole) Then
                Dim DrTrap As DS_Ricetta_Trappole.DT_Ricetta_TrappoleRow
                For j = 0 To Dt_Impianti_Trappole.Rows.Count - 1

                    DrTrap = DsTrappole.DT_Ricetta_Trappole.NewDT_Ricetta_TrappoleRow

                    Ric_op_Cod = Dt_Impianti_Trappole.Rows(j).Item("Ricetta_Operazione_Cod")

                    If Memo_Ric_op_Cod <> Ric_op_Cod Then
                        'primo record o nuovo
                        DrTrap.PrimaRigaAppezza = 1
                        Memo_Ric_op_Cod = Ric_op_Cod
                    Else
                        DrTrap.PrimaRigaAppezza = 2
                    End If

                    'If j = 0 Then 'questo non va bene, perchè lo valorizzava solo al primo record, invece
                    'serve al primo appezza di ogni ricetta_operazione_cod
                    '    DrTrap.PrimaRigaAppezza = 1
                    'Else
                    '    DrTrap.PrimaRigaAppezza = 2
                    'End If

                    DrTrap.Pro_Cod = 0
                    DrTrap.Ricetta_Operazione_Cod = Dt_Impianti_Trappole.Rows(j).Item("Ricetta_Operazione_Cod")
                    DrTrap.Sa_Nome = Dt_Impianti_Trappole.Rows(j).Item("sa_nome")
                    DrTrap.Campo_Nome = Dt_Impianti_Trappole.Rows(j).Item("campo_des")
                    DrTrap.App_nome = Dt_Impianti_Trappole.Rows(j).Item("app_nome")
                    DrTrap.Veg_Des = Dt_Impianti_Trappole.Rows(j).Item("veg_Des")
                    DrTrap.Cul_Des = Dt_Impianti_Trappole.Rows(j).Item("Cul_Des")

                    'DrTrap.Sup_App = CDec(Dt_Impianti_Trappole.Rows(j).Item("sup_imp"))
                    'Anna 16/05/22: [rif. chiamata 18108] Nella stampa la colonna Ha viene fuori la Superficie selezionata e non quella trattata
                    DrTrap.Sup_App = CDec(Dt_Impianti_Trattamenti.Rows(j).Item("Qta2"))

                    DrTrap.Regolamento = Dt_Impianti_Trappole.Rows(j).Item("reg_dpi")
                    DrTrap.Indirizzo = Dt_Impianti_Trappole.Rows(j).Item("Appezza_Indirizzo")
                    DrTrap.Telefono = Dt_Impianti_Trappole.Rows(j).Item("Appezza_Rubrica")
                    DrTrap.Ditta_Des = ""
                    DrTrap.Av_Des = ""
                    DrTrap.Data = ""
                    DrTrap.Qta = ""
                    DrTrap.Qta_Tot = ""
                    DrTrap.Note = ""

                    DsTrappole.DT_Ricetta_Trappole.Rows.Add(DrTrap)

                Next

            End If
            '//////////////////////////////////////

        Catch ex2 As Exception
            Log_Errori &= "TRAPPOLE: carica appezzamenti: " & vbCrLf & ex2.Message & vbCrLf
        End Try


    End Sub

    '###########################################################################
    Private Sub Riempimento_Report_Tipo3(ByRef CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer,
                                   ByVal bNascondiCodRicetta As Boolean,
                                   ByVal bNascondiFirmaAgricoltore As Boolean,
                                      ByVal Data_Inizio As String,
                                    ByVal Data_Fine As String,
                                    ByVal XML_Ricetta As System.Xml.XmlElement,
                                    ByVal frm_RicettaNote As String)

        Dim objTraduttore As New Traduzione_Stampa_Ricette(String.Empty,
                                                           enum_CodificaStampe.SchedaCampagna_Ricette,
                                                           "it", 0, objParametri_Server)
        objTraduttore.Traduci("", "")


        rptStampa3 = New Rpt_Ricetta_3

        Select Case objParametri_Server.PivaSuperUser
            Case "00055690382" 'ovierre
                CType(rptStampa3.Section1.ReportObjects("PictureOvierre"), CrystalDecisions.CrystalReports.Engine.PictureObject).ObjectFormat.EnableSuppress = False
            Case Else
                CType(rptStampa3.Section1.ReportObjects("PictureOvierre"), CrystalDecisions.CrystalReports.Engine.PictureObject).ObjectFormat.EnableSuppress = True
        End Select

        If bNascondiCodRicetta Then
            If (Ricetta_tipo = enum_TipoRicetta.PianoDistribuzioneConcimi) Then
                Param_Intestazione = Ricetta_Des
            Else
                Param_Intestazione = "Ricetta: " & Ricetta_Des
            End If
        Else
            If (Ricetta_tipo = enum_TipoRicetta.PianoDistribuzioneConcimi) Then
                Param_Intestazione = Ricetta_Des
            Else
                Param_Intestazione = "Ricetta: " & Ricetta_Des & " Numero: " & Ricetta_Numero
            End If
        End If

        Param_Rag_Soc = Rag_Soc


        If Data_Inizio <> Data_Fine Then
            Param_Date = "Da " & Data_Inizio & " a " & Data_Fine
        Else
            Param_Date = Data_Inizio
        End If

        Veg_Cod = CInt(XML_Ricetta.GetAttribute("veg_cod"))

        Param_Coltura = Veg_Des


        Param_Note = Note

        If Param_Note <> "" Then
            If frm_RicettaNote <> "" Then
                Param_Note &= ", " & frm_RicettaNote
            End If
        Else
            Param_Note = frm_RicettaNote
        End If

        '-------------------------------------------------------
        ' Firma Agricoltore

        If bNascondiFirmaAgricoltore Then
            rptStampa3.Section6.ReportObjects("TextFirmaAgricoltore").ObjectFormat.EnableSuppress = True
            rptStampa3.Section6.ReportObjects("LineaFirmaAgricoltore").ObjectFormat.EnableSuppress = True
        End If

        '---------------------------------------------
        'Visualizzo i sottoreport

        Try

#Region "COMMENTATO"
            'appezzamenti
            ''Mofica Simone Galassi: Se sono della ditrib. concimi uso un sottoreport diverso
            'If DsAppezzamenti.DT_Ricetta_Appezzamenti.Rows.Count > 0 Then
            '    If (Ricetta_tipo = enum_TipoRicetta.PianoDistribuzioneConcimi) Then
            '        rptStampa3.OpenSubreport("Rpt_Ricetta_Appezzamenti_piano_distribuzione.rpt").SetDataSource(DsAppezzamenti)
            '        rptStampa3.Section10.SectionFormat.EnableSuppress = True
            '    Else
            '        rptStampa3.OpenSubreport("Rpt_Ricetta_Appezzamenti.rpt").SetDataSource(DsAppezzamenti)
            '        rptStampa3.Section12.SectionFormat.EnableSuppress = True 'TOGLIERE COMMENTO QUI
            '    End If
            'Else
            '    rptStampa3.Section10.SectionFormat.EnableSuppress = True
            '    rptStampa3.Section12.SectionFormat.EnableSuppress = True 'TOGLIERE COMMENTO QUI
            'End If

            'QUESTO NON FUNZIONA, RESTITUISCE: Non supportato nei sottoreport.
            'Try
            '    rptStampa3.OpenSubreport("Rpt_Ricetta3_Lavorazioni.rpt").OpenSubreport("Rpt_Ricetta3_Appezzamenti.rpt").SetDataSource(DsAppezzamenti)
            'Catch ex As Exception
            '    Log_Errori &= "- impostazione parametri: " & vbCrLf & ex.Message & vbCrLf
            'End Try

            ''QUESTO NON FUNZIONA, RESTITUISCE: Non supportato nei sottoreport.
            'Try
            '    CType(CType(rptStampa3.OpenSubreport("Rpt_Ricetta3_Lavorazioni.rpt"), CrystalDecisions.CrystalReports.Engine.ReportDocument).OpenSubreport("Rpt_Ricetta3_Appezzamenti.rpt"), CrystalDecisions.CrystalReports.Engine.ReportDocument).SetDataSource(DsAppezzamenti)
            'Catch ex As Exception
            '    Log_Errori &= "- impostazione parametri: " & vbCrLf & ex.Message & vbCrLf
            'End Try

            ''QUESTO NON FUNZIONA, RESTITUISCE: Riferimento a un oggetto non impostato su un'istanza di oggetto.
            'Try
            '    rptStampa3.OpenSubreport("Rpt_Ricetta3_Lavorazioni.rpt").SetDataSource(DsAppezzamenti)
            'Catch ex As Exception
            '    Log_Errori &= "- impostazione parametri: " & vbCrLf & ex.Message & vbCrLf
            'End Try

            'QUESTO NON FUNZIONA, RESTITUISCE: Riferimento a un oggetto non impostato su un'istanza di oggetto.
            'Try
            '    rptStampa3.OpenSubreport("Rpt_Ricetta3_Appezzamenti.rpt").SetDataSource(DsAppezzamenti)
            'Catch ex As Exception
            '    Log_Errori &= "- impostazione parametri: " & vbCrLf & ex.Message & vbCrLf
            'End Try

            ' rptStampa3.OpenSubreport("Rpt_Ricetta3_Appezzamenti.rpt").SetDataSource(DsAppezzamenti)


            'If DSTrattamenti3.DT_Trattamenti_Appezzamenti.Rows.Count > 0 And DSTrattamenti3.DT_Ricetta3_Trattamenti.Rows.Count > 0 Then
            '    rptStampa3.OpenSubreport("Rpt_Ricetta3_Trattamenti_2.rpt").SetDataSource(DSTrattamenti3)
            'Else
            '    rptStampa3.ReportFooterSection2.SectionFormat.EnableSuppress = True
            'End If

            'If DsFertilizzazioni.DT_Ricetta_Fertilizzazioni.Rows.Count > 0 Then
            '    rptStampa3.OpenSubreport("Rpt_Ricetta3_Fertilizzazioni.rpt").SetDataSource(DsFertilizzazioni)
            '    rptStampa3.Section13.SectionFormat.EnableSuppress = True
            '    'If (Ricetta_tipo = enum_TipoRicetta.PianoDistribuzioneConcimi) Then
            '    '    rptStampa3.OpenSubreport("Rpt_Ricetta_Fertilizzazioni_piano_distribuzione.rpt").SetDataSource(DsFertilizzazioni)
            '    '    rptStampa3.Section7.SectionFormat.EnableSuppress = True
            '    'Else
            '    '    rptStampa3.OpenSubreport("Rpt_Ricetta_Fertilizzazioni.rpt").SetDataSource(DsFertilizzazioni)
            '    '    rptStampa3.Section13.SectionFormat.EnableSuppress = True
            '    'End If
            'Else
            '    rptStampa3.Section7.SectionFormat.EnableSuppress = True
            '    rptStampa3.Section13.SectionFormat.EnableSuppress = True
            'End If

            'If DsIrrigazioni.DS_Ricetta_Irrigazioni.Rows.Count > 0 Then
            '    rptStampa3.OpenSubreport("Rpt_Ricetta_Irrigazioni.rpt").SetDataSource(DsIrrigazioni)
            'Else
            '    rptStampa3.Section8.SectionFormat.EnableSuppress = True
            'End If

            'If DsTrappole.DT_Ricetta_Trappole.Rows.Count > 0 Then
            '    rptStampa3.OpenSubreport("Rpt_Ricetta3_Trappole.rpt").SetDataSource(DsTrappole)
            'Else
            '    rptStampa3.Section9.SectionFormat.EnableSuppress = True
            'End If

            'If DSLavorazioni3.DT_Lavorazioni_Appezzamenti.Rows.Count > 0 And DSLavorazioni3.DT_Ricetta3_Lavorazioni.Rows.Count > 0 Then
            '    rptStampa3.OpenSubreport("Rpt_Ricetta3_Lavorazioni.rpt").SetDataSource(DSLavorazioni3)
            'Else
            '    rptStampa3.ReportFooterSection2.SectionFormat.EnableSuppress = True
            'End If
#End Region

            rptStampa3.OpenSubreport("Rpt_Ricetta3_Trattamenti_2.rpt").SetDataSource(DSTrattamenti3)

            rptStampa3.OpenSubreport("Rpt_Ricetta3_Fertilizzazioni.rpt").SetDataSource(DsFertilizzazioni)

            rptStampa3.OpenSubreport("Rpt_Ricetta_Irrigazioni.rpt").SetDataSource(DsIrrigazioni)

            rptStampa3.OpenSubreport("Rpt_Ricetta3_Trappole.rpt").SetDataSource(DsTrappole)

            rptStampa3.OpenSubreport("Rpt_Ricetta3_Lavorazioni.rpt").SetDataSource(DSLavorazioni3)

            rptStampa3.SetParameterValue("LP_Intestazione", objTraduttore.Traduzioni.Rpt_Ricetta_Irrigazioni.Intestazione, "Rpt_Ricetta_Irrigazioni.rpt")
            rptStampa3.SetParameterValue("LP_Data_Cons", objTraduttore.Traduzioni.Rpt_Ricetta_Irrigazioni.Data_Cons, "Rpt_Ricetta_Irrigazioni.rpt")
            rptStampa3.SetParameterValue("LP_Numero_appezzamento", objTraduttore.Traduzioni.Rpt_Ricetta_Irrigazioni.Numero_appezzamento, "Rpt_Ricetta_Irrigazioni.rpt")
            rptStampa3.SetParameterValue("LP_Dose", objTraduttore.Traduzioni.Rpt_Ricetta_Irrigazioni.Dose, "Rpt_Ricetta_Irrigazioni.rpt")
            rptStampa3.SetParameterValue("LP_Unita_Misura", objTraduttore.Traduzioni.Rpt_Ricetta_Irrigazioni.Unita_Misura, "Rpt_Ricetta_Irrigazioni.rpt")
            rptStampa3.SetParameterValue("LP_Ore", objTraduttore.Traduzioni.Rpt_Ricetta_Irrigazioni.Ore, "Rpt_Ricetta_Irrigazioni.rpt")
            rptStampa3.SetParameterValue("LP_Portata", objTraduttore.Traduzioni.Rpt_Ricetta_Irrigazioni.Portata, "Rpt_Ricetta_Irrigazioni.rpt")
            rptStampa3.SetParameterValue("LP_Periodo_Turno_Medio", objTraduttore.Traduzioni.Rpt_Ricetta_Irrigazioni.Periodo_Turno_Medio, "Rpt_Ricetta_Irrigazioni.rpt")
            rptStampa3.SetParameterValue("LP_Giorni", objTraduttore.Traduzioni.Rpt_Ricetta_Irrigazioni.Giorni, "Rpt_Ricetta_Irrigazioni.rpt")


        Catch ex As Exception
            Log_Errori &= "- SetDataSource sottoreport: " & vbCrLf & ex.Message & vbCrLf
        End Try


        Try

            If DSTrattamenti3.DT_Trattamenti_Appezzamenti.Rows.Count <> 0 And DSTrattamenti3.DT_Ricetta3_Trattamenti.Rows.Count <> 0 Then
                Dim debug As Boolean = True
            Else
                rptStampa3.Section11.SectionFormat.EnableSuppress = True
            End If

            If DsFertilizzazioni.DT_Ricetta_Fertilizzazioni.Rows.Count > 0 Then
                rptStampa3.Section13.SectionFormat.EnableSuppress = True
            Else
                rptStampa3.Section7.SectionFormat.EnableSuppress = True
                rptStampa3.Section13.SectionFormat.EnableSuppress = True
            End If

            If DsIrrigazioni.DS_Ricetta_Irrigazioni.Rows.Count = 0 Then
                rptStampa3.Section8.SectionFormat.EnableSuppress = True
            End If

            If DsTrappole.DT_Ricetta_Trappole.Rows.Count = 0 Then
                rptStampa3.Section9.SectionFormat.EnableSuppress = True
            End If

            If DSLavorazioni3.DT_Lavorazioni_Appezzamenti.Rows.Count <> 0 And DSLavorazioni3.DT_Ricetta3_Lavorazioni.Rows.Count <> 0 Then
                Dim debug As Boolean = True
            Else
                rptStampa3.ReportFooterSection2.SectionFormat.EnableSuppress = True
            End If

        Catch ex As Exception
            Log_Errori &= "- visibilità sottoreport: " & vbCrLf & ex.Message & vbCrLf
        End Try

        '///////////////////////////////////////////////////////////////////////////

        '--------------------------------------------------
        ' Leggi intestazione impresa
        Dim objStampe As New AgronicaCoreStampeDAL.Stampe_QDC

        objStampe.Prepara_Parametri_Intestazione_ReportQDC(Piva,
                                                           Data_Inizio,
                                                           Data_Fine,
                                                           Param_Rag_Soc,
                                                            Param_Piva_CUAA,
                                                            Param_Indirizzo,
                                                             Param_Date,
                                                             objParametri_Server)

        Try

            rptStampa3.SetParameterValue("Rag_Soc", Param_Rag_Soc)
            rptStampa3.SetParameterValue("Piva_CUAA", Param_Piva_CUAA)
            rptStampa3.SetParameterValue("Indirizzo", Param_Indirizzo)

            rptStampa3.SetParameterValue("Date", Param_Date)

            rptStampa3.SetParameterValue("Intestazione", Param_Intestazione)
            rptStampa3.SetParameterValue("Coltura", Param_Coltura)
            rptStampa3.SetParameterValue("Note", Param_Note)

            'impostare un parametro sul sottoreport
            ' REPORT.SetParameterValue("NOME PARAMETRO", VARIABILE PARAMETRO, "SOTTOREPORT.rpt")

        Catch ex As Exception
            Log_Errori &= "- impostazione parametri: " & vbCrLf & ex.Message & vbCrLf
        End Try


        '---------------------------------------------

        'faccio il databind col visualizzatore dei reports...
        CrystalReportViewer1.DisplayToolbar = True
        CrystalReportViewer1.ReportSource = rptStampa3
        CrystalReportViewer1.DataBind()


        '==================================================================

        ' Dichiara le variabili e restituisce le opzioni di esportazione.
        Dim exportOpts As New ExportOptions
        Dim diskOpts As New DiskFileDestinationOptions
        exportOpts = rptStampa3.ExportOptions

        ' Imposta il formato di esportazione.
        exportOpts.ExportFormatType = ExportFormatType.PortableDocFormat
        exportOpts.ExportDestinationType = ExportDestinationType.DiskFile

        ' Imposta le opzioni relative al file del disco.
        Dim strPath As String
        Dim PathFileTemporanei As String

        Dim objAgroWebConfig As New AgroWebConfig

        If objAgroWebConfig.PathFileTemporanei = "" Then
            PathFileTemporanei = Server.MapPath("../Stampa/File_Temporanei")
        Else
            PathFileTemporanei = objAgroWebConfig.PathFileTemporanei
        End If

        strPath = PathFileTemporanei & "\Ricetta_" & Session.SessionID.ToString & ".pdf"
        diskOpts.DiskFileName = strPath
        exportOpts.DestinationOptions = diskOpts

        rptStampa3.PrintOptions.PaperOrientation = PaperOrientation.Landscape

        Dim NomeStampante As String
        If objAgroWebConfig.NomeStampante <> "" Then
            NomeStampante = objAgroWebConfig.PathFileTemporanei
            rptStampa3.PrintOptions.PrinterName = NomeStampante
        End If

        ' Esportazione del report.
        rptStampa3.Export()

        ' Con il seguente codice il file pdf viene scritto 
        '  nel browser del client.
        Response.ClearContent()
        Response.ClearHeaders()
        Response.ContentType = "application/pdf"
        Response.AddHeader("Content-Disposition", "inline; filename = Ricette.pdf")
        Response.WriteFile(strPath)
        Response.Flush()
        Response.Close()

        ' il file esportato viene eliminato dal disco
        System.IO.File.Delete(strPath)

    End Sub


    '###########################################################################
    Private Sub Riempimento_Report(ByRef CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer,
                                   ByVal bNascondiCodRicetta As Boolean,
                                   ByVal bNascondiFirmaAgricoltore As Boolean,
                                      ByVal Data_Inizio As String,
                                    ByVal Data_Fine As String,
                                    ByVal XML_Ricetta As System.Xml.XmlElement,
                                    ByVal frm_RicettaNote As String, ByVal objTRaduttore As Traduzione_Stampa_Ricette)


        '-----------------------------------------------------------------
        '-----------------------------------------------------------------
        'RIEMPIMENTO REPORT
        '-----------------------------------------------------------------
        '-----------------------------------------------------------------
        Select Case Qs_Stampa_Tipo

            Case enum_TipoStampaRicetta.PianoLavori

                Riempimento_Report_Tipo3(CrystalReportViewer1,
                                   bNascondiCodRicetta,
                                   bNascondiFirmaAgricoltore,
                                   Data_Inizio,
                                   Data_Fine,
                                   XML_Ricetta,
                                   frm_RicettaNote)


                '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

            Case enum_TipoStampaRicetta.Certificazione
                'Rpt_Ricetta
                Riempimento_Report_Tipo1(CrystalReportViewer1,
                                   bNascondiCodRicetta,
                                   bNascondiFirmaAgricoltore,
                                   Data_Inizio,
                                   Data_Fine,
                                   XML_Ricetta,
                                   frm_RicettaNote, objTRaduttore)


            Case enum_TipoStampaRicetta.Aziendale
                'Rpt_Ricetta_New
                Riempimento_Report_Tipo2(CrystalReportViewer1,
                              bNascondiCodRicetta,
                              bNascondiFirmaAgricoltore,
                              Data_Inizio,
                              Data_Fine,
                              XML_Ricetta,
                              frm_RicettaNote)

        End Select


    End Sub


    '###########################################################################
    Private Sub CaricaDsAppezzamenti_RaggruppatixVarieta(TipoCodiceAppezzamento As Integer,
                                                         flag_nascondiCampo As Boolean,
                                                         ByVal BaseCode As Integer,
                                                         ByVal Data_Inizio As Date,
                                                         ByVal Data_Fine As Date,
                                                         ByVal objTraduttore As Traduzione_Stampa_Ricette)

        Dim objApp As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
        Dim objDestinazioni As New AgronicaCoreContabDAL.Ricette_Destinazioni_R
        Dim Dt_Destinazioni As New DataTable
        Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim Dt_Impianti As New DataTable
        Dim Dr() As DataRow
        Dim RigaDs As DS_Ricetta_Appezzamenti.DT_Ricetta_AppezzamentiRow

        Dim i As Integer = 0
        Dim ArrayPiva() As String = Nothing
        Dim ArraySa_Cod() As Integer
        Dim ArrayAppezza() As Integer
        Dim ArrayId_Reg() As Integer

        Dt_Destinazioni = objDestinazioni.Leggi(Ricetta_Cod,
                                                0, 0, 0, 0, "", 0, 0, 0,
                                                AGRODATAINIZIO,
                                                AGRODATAFINE, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                " Ricette_Destinazioni.Id_Reg <> 0 ",
                                                "",
                                                objParametri_Server)

        objDestinazioni = Nothing

        If Not Dt_Destinazioni Is Nothing AndAlso Dt_Destinazioni.Rows.Count > 0 Then

            For i = 0 To Dt_Destinazioni.Rows.Count - 1

                If ArrayPiva Is Nothing Then
                    ReDim Preserve ArrayPiva(0)
                    ReDim Preserve ArraySa_Cod(0)
                    ReDim Preserve ArrayAppezza(0)
                    ReDim Preserve ArrayId_Reg(0)
                Else
                    ReDim Preserve ArrayPiva(UBound(ArrayPiva) + 1)
                    ReDim Preserve ArraySa_Cod(UBound(ArraySa_Cod) + 1)
                    ReDim Preserve ArrayAppezza(UBound(ArrayAppezza) + 1)
                    ReDim Preserve ArrayId_Reg(UBound(ArrayId_Reg) + 1)
                End If

                ArrayPiva(UBound(ArrayPiva)) = Dt_Destinazioni.Rows(i).Item("piva")
                ArraySa_Cod(UBound(ArraySa_Cod)) = Dt_Destinazioni.Rows(i).Item("sa_cod")
                ArrayAppezza(UBound(ArrayAppezza)) = Dt_Destinazioni.Rows(i).Item("appezza")
                ArrayId_Reg(UBound(ArrayId_Reg)) = Dt_Destinazioni.Rows(i).Item("id_reg")
            Next

            Dt_Impianti = objImpianti.Leggi_Dati_Impianti(ArrayPiva,
                                              ArraySa_Cod,
                                              ArrayAppezza,
                                              ArrayId_Reg,
                                              Data_Inizio, Data_Fine,
                                              "",
                                              "",
                                              objParametri_Server)

        End If



        objImpianti = Nothing

        If Not Dt_Impianti Is Nothing AndAlso Dt_Impianti.Rows.Count > 0 Then

            '(12/11/2018 fede) aggiunta indicazione fase fenologica corrente
            Dim objParametriUscitaFasiNew As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output

            If Veg_Cod > 0 Then

                Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.FasiFenologiche_input
                objParametriIngresso.Veg_Cod = Veg_Cod
                objParametriIngresso.Lingua_Cod = objParametri_Server.Lingua_Cod

                Dim Leggi_impostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                Dim imp As String = Leggi_impostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_FASI_FENOLOGICHE, objParametri_Utenti, 2)
                If imp = "1" Then
                    objParametriIngresso.Personalizzate = True
                End If

                Dim objFasi_WS As New AgronicaCoreWebService.FasiFenologiche_WS
                objParametriUscitaFasiNew = objFasi_WS.FasiFenologiche(objParametriIngresso)

            End If

            Dim objSqlDis As New AgronicaCoreUtility.DatatableUtility
            Dim strCulCod() As String = objSqlDis.SelectDistinct(Dt_Impianti, "Cul_Cod")
            Dim Cul_Cod As String
            Dim strAppezzamenti As String
            Dim Sup_Tot As Double
            Dim j As Integer = 0
            Dim N_app As String = ""
            Dim Num_Appezzamento As Integer
            Dim N_Appezza As Integer

            If Not strCulCod Is Nothing AndAlso strCulCod.Length > 0 Then

                For i = 0 To strCulCod.Length - 1

                    Cul_Cod = strCulCod(i)

                    Dr = Dt_Impianti.Select("cul_cod=" & Cul_Cod)

                    strAppezzamenti = ""
                    Sup_Tot = 0
                    N_app = ""
                    Num_Appezzamento = 0

                    If Not Dr Is Nothing AndAlso Dr.Length > 0 Then

                        For j = 0 To Dr.Length - 1

                            Sup_Tot += CDbl(Dr(j).Item("sup_imp"))

                            'N.ro appezzamento
                            N_app = Dr(j).Item("app_nome_breve")

                            N_Appezza = Dr(j).Item("appezza") - BaseCode

                            N_app = objApp.Numero_Appezzamento(Dr(j).Item("piva"), Dr(j).Item("sa_cod"),
                                                   Dr(j).Item("campo_des"), Dr(j).Item("appezza"),
                                                   TipoCodiceAppezzamento,
                                                   Dr(j).Item("app_nome"), Dr(j).Item("app_nome_breve"),
                                                   N_Appezza, BaseCode,
                                                   Nothing, flag_nascondiCampo)

                            'If N_app = "" Then

                            '    Num_Appezzamento = New AgronicaCoreAnagrafeDAL.Appezzamento_Read().AppezzamentoNumero_from_AppezzamentoNome(Dr(j).Item("app_nome"))

                            '    If Num_Appezzamento <> 0 Then
                            '        N_app = CStr(Num_Appezzamento)
                            '    Else

                            '        If BaseCode = 0 Then
                            '            Dim ObjBase As New AgronicaCoreDataProvider.UtilityProvider
                            '            BaseCode = ObjBase.BaseCode_from_ProgressivoGias(Session("ASG_ProgressivoGIAS"))
                            '            ObjBase = Nothing
                            '        End If

                            '        N_app = Dr(j).Item("appezza") - BaseCode

                            '        'se è un valore alto lo tronco alle ultime 3 cifre..
                            '        If N_app.Length > 3 Then
                            '            N_app = Right(N_app, 3)
                            '        End If

                            '    End If

                            'End If


                            Dim Fase_Cod_Corrente As Integer = 0
                            Dim Fase_Des_Corrente As String = ""
                            Dim Data_Fase_Corrente As String = ""

                            '(08/04/2020 fede)
                            If Not IsDBNull(Dr(j).Item("Fase_Cod_Corrente")) AndAlso
                    IsNumeric(Dr(j).Item("Fase_Cod_Corrente")) AndAlso
                    CInt(Dr(j).Item("Fase_Cod_Corrente")) > 0 Then

                                Fase_Cod_Corrente = CInt(Dr(j).Item("Fase_Cod_Corrente"))

                                Select Case Fase_Cod_Corrente

                                    'Case < 1000 'caso vecchio av_cod = ff_cod
                                    '    Fase_Des_Corrente = (From aa In objParametriUscitaFasiOld.ListaFasiFenologiche
                                    '                         Where aa.FF_Cod = Fase_Cod_Corrente
                                    '                         Select aa.Descrizione
                                    '    ).FirstOrDefault

                                    Case Else ' caso nuovo av_cod= cod_css
                                        Fase_Des_Corrente = (From aa In objParametriUscitaFasiNew.ListaFasiFenologiche
                                                             Where aa.Cod_SS = Fase_Cod_Corrente
                                                             Select aa.Descrizione & " - BBCH " & aa.Stadio
                                        ).FirstOrDefault
                                End Select

                                If Not IsDBNull(Dr(j).Item("Data_Fase_Corrente")) AndAlso
                                    IsDate(Dr(j).Item("Data_Fase_Corrente")) Then
                                    Data_Fase_Corrente = CDate(Dr(j).Item("Data_Fase_Corrente")).ToShortDateString
                                End If

                            End If

                            If Data_Fase_Corrente <> "" Then
                                Fase_Des_Corrente &= " (" & Data_Fase_Corrente & ")"
                            End If


                            If Fase_Des_Corrente <> "" Then
                                strAppezzamenti &= N_app & " (" & Dr(j).Item("sup_imp").ToString & " Ha) - " & objTraduttore.Traduzioni.FaseFenologicaCorrente & " " & Fase_Des_Corrente & ", "
                            Else
                                strAppezzamenti &= N_app & " (" & Dr(j).Item("sup_imp").ToString & " Ha), "
                            End If

                        Next

                        If strAppezzamenti <> "" Then
                            strAppezzamenti = Left(strAppezzamenti, strAppezzamenti.Length - 2)
                        End If

                        'inserisco la riga nel dataset..
                        RigaDs = DsAppezzamenti.DT_Ricetta_Appezzamenti.NewDT_Ricetta_AppezzamentiRow

                        RigaDs.CUL_DES = Dr(0).Item("Cul_Des")
                        RigaDs.str_Appezzamenti = strAppezzamenti
                        RigaDs.Sup_Tot = Sup_Tot.ToString

                        DsAppezzamenti.DT_Ricetta_Appezzamenti.Rows.Add(RigaDs)

                    End If

                Next

            End If

        End If


    End Sub

    Private Sub CaricaDsAppezzamenti(TipoCodiceAppezzamento As Integer, flag_nascondiCampo As Boolean, ByVal BaseCode As Integer, ByVal Data_Inizio As Date, ByVal Data_Fine As Date)

        Dim objApp As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
        Dim objDestinazioni As New AgronicaCoreContabDAL.Ricette_Destinazioni_R
        Dim Dt_Destinazioni As New DataTable
        Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim Dt_Impianti As New DataTable
        Dim RigaDs As DS_Ricetta_Appezzamenti.DT_Ricetta_AppezzamentiRow

        Dim i As Integer = 0
        Dim ArrayPiva() As String
        Dim ArraySa_Cod() As Integer
        Dim ArrayAppezza() As Integer
        Dim ArrayId_Reg() As Integer

        Dt_Destinazioni = objDestinazioni.Leggi(Ricetta_Cod,
                                                0, 0, 0, 0, "", 0, 0, 0,
                                                AGRODATAINIZIO,
                                                AGRODATAFINE, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                " Ricette_Destinazioni.Id_Reg <> 0 ",
                                                "",
                                                objParametri_Server)

        objDestinazioni = Nothing

        If Not Dt_Destinazioni Is Nothing AndAlso Dt_Destinazioni.Rows.Count > 0 Then

            For i = 0 To Dt_Destinazioni.Rows.Count - 1

                If ArrayPiva Is Nothing Then
                    ReDim Preserve ArrayPiva(0)
                    ReDim Preserve ArraySa_Cod(0)
                    ReDim Preserve ArrayAppezza(0)
                    ReDim Preserve ArrayId_Reg(0)
                Else
                    ReDim Preserve ArrayPiva(UBound(ArrayPiva) + 1)
                    ReDim Preserve ArraySa_Cod(UBound(ArraySa_Cod) + 1)
                    ReDim Preserve ArrayAppezza(UBound(ArrayAppezza) + 1)
                    ReDim Preserve ArrayId_Reg(UBound(ArrayId_Reg) + 1)
                End If

                ArrayPiva(UBound(ArrayPiva)) = Dt_Destinazioni.Rows(i).Item("piva")
                ArraySa_Cod(UBound(ArraySa_Cod)) = Dt_Destinazioni.Rows(i).Item("sa_cod")
                ArrayAppezza(UBound(ArrayAppezza)) = Dt_Destinazioni.Rows(i).Item("appezza")
                ArrayId_Reg(UBound(ArrayId_Reg)) = Dt_Destinazioni.Rows(i).Item("id_reg")
            Next

            Dt_Impianti = objImpianti.Leggi_Dati_Impianti(ArrayPiva,
                                              ArraySa_Cod,
                                              ArrayAppezza,
                                              ArrayId_Reg,
                                               Data_Inizio, Data_Fine,
                                               "",
                                              "",
                                              objParametri_Server)

        End If


        Dim dtNPK As DataTable = GeneraStrutturaDTNPK()

        'Se piano distribuzione concimi carico NPK Massimi per inserirli poi nel report
        If (Ricetta_tipo = enum_TipoRicetta.PianoDistribuzioneConcimi AndAlso Not Dt_Destinazioni Is Nothing AndAlso Dt_Destinazioni.Rows.Count > 0) Then

            Dim objPC_EntitaxTestata_R As New AgronicaCoreAnagrafeDAL.PianoConcimazione_EntitaxTestata_R
            Dim Dt_EntitaxTestata As DataTable = objPC_EntitaxTestata_R.Leggi(Ricetta_Programmazione_Cod, 0, "", 0, 0, 0, 0, 0, "", "", "", 0, 0, "", 0, "", AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            'Popolo la lista degli impianti collegati con tanto di NPK
            For Each drElem As DataRow In Dt_EntitaxTestata.Rows

                Dim piva = drElem.Item("Piva")
                Dim Sa_Cod = drElem.Item("Sa_Cod")
                Dim Appezza = drElem.Item("Appezza")
                Dim Id_reg = drElem.Item("Id_Imp")


                Dim dr As DataRow = dtNPK.NewRow()

                dr.Item("piva") = piva
                dr.Item("sa_cod") = Sa_Cod
                dr.Item("campo_cod") = drElem.Item("Campo_Cod")
                dr.Item("appezza") = Appezza
                dr.Item("id_reg") = Id_reg

                'Aggiungo i valori massimi di NPK per lo specifico impianto
                dr.Item("N_Max") = drElem.Item("QtaMaxN")
                dr.Item("P_Max") = drElem.Item("QtaMaxP2O5")
                dr.Item("K_Max") = drElem.Item("QtaMaxK2O")

                dtNPK.Rows.Add(dr)
            Next

            'Salvo un NPK MAX per poter fare il residuo nelle fertilizzazioni
            Me.N_Max = Dt_EntitaxTestata.Rows(0).Item("QtaMaxN")
            Me.P_Max = Dt_EntitaxTestata.Rows(0).Item("QtaMaxP2O5")
            Me.K_Max = Dt_EntitaxTestata.Rows(0).Item("QtaMaxK2O")

        End If

        objImpianti = Nothing

        Dim Sup_Tot As Double
        Dim N_app As String = ""
        'Dim Num_Appezzamento As Integer
        Dim N_Appezza As Integer


        If Not Dt_Impianti Is Nothing AndAlso Dt_Impianti.Rows.Count > 0 Then

            For i = 0 To Dt_Impianti.Rows.Count - 1

                Sup_Tot += CDbl(Dt_Impianti.Rows(i).Item("sup_imp"))

                'N.ro appezzamento
                N_app = Dt_Impianti.Rows(i).Item("app_nome_breve")

                N_Appezza = Dt_Impianti.Rows(i).Item("appezza") - BaseCode

                N_app = objApp.Numero_Appezzamento(Dt_Impianti.Rows(i).Item("piva"), Dt_Impianti.Rows(i).Item("sa_cod"),
                                                   Dt_Impianti.Rows(i).Item("campo_des"), Dt_Impianti.Rows(i).Item("appezza"),
                                                   TipoCodiceAppezzamento,
                                                   Dt_Impianti.Rows(i).Item("app_nome"), Dt_Impianti.Rows(i).Item("app_nome_breve"),
                                                   N_Appezza, BaseCode,
                                                   Nothing, flag_nascondiCampo)

                'If N_app = "" Then
                '    Num_Appezzamento = New AgronicaCoreAnagrafeDAL.Appezzamento_Read().AppezzamentoNumero_from_AppezzamentoNome(Dt_Impianti.Rows(i).Item("app_nome"))
                '    If Num_Appezzamento <> 0 Then
                '        N_app = CStr(Num_Appezzamento)
                '    Else
                '        If BaseCode = 0 Then
                '            Dim ObjBase As New AgronicaCoreDataProvider.UtilityProvider
                '            BaseCode = ObjBase.BaseCode_from_ProgressivoGias(Session("ASG_ProgressivoGIAS"))
                '            ObjBase = Nothing
                '        End If
                '        N_app = Dt_Impianti.Rows(i).Item("appezza") - BaseCode
                '        'se è un valore alto lo tronco alle ultime 3 cifre..
                '        If N_app.Length > 3 Then
                '            N_app = Right(N_app, 3)
                '        End If
                '    End If
                'End If

                'strAppezzamenti &= "N.ro App." & N_app & " (" & Dr(j).Item("sup_imp").ToString & " Ha), "

                'inserisco la riga nel dataset..
                RigaDs = DsAppezzamenti.DT_Ricetta_Appezzamenti.NewDT_Ricetta_AppezzamentiRow

                RigaDs.Campo_Nome = Dt_Impianti.Rows(i).Item("campo_des")
                RigaDs.Sa_Nome = Dt_Impianti.Rows(i).Item("sa_nome")
                RigaDs.CUL_DES = Dt_Impianti.Rows(i).Item("Cul_Des")
                RigaDs.App_nome = N_app
                RigaDs.Sup_App = CDbl(Dt_Impianti.Rows(i).Item("sup_imp"))
                'Simone Galassi: Inserisce qui le righe NPK max dell'impianto in (EntitàXtestata)
                If (Ricetta_tipo = enum_TipoRicetta.PianoDistribuzioneConcimi AndAlso Not dtNPK Is Nothing) Then
                    'FARE SELECT CON RITORNO DATAROW
                    Dim DrNPK() As DataRow

                    DrNPK = dtNPK.Select("piva ='" & Dt_Impianti.Rows(i).Item("piva") & "' AND sa_cod=" & Dt_Impianti.Rows(i).Item("sa_cod") &
                                          "AND appezza=" & Dt_Impianti.Rows(i).Item("appezza") & "AND id_reg=" & Dt_Impianti.Rows(i).Item("id_reg"))

                    If Not DrNPK Is Nothing AndAlso DrNPK.Length > 0 Then
                        RigaDs.N_Max = CStr(DrNPK(0).Item("N_Max")) 'N_Max
                        RigaDs.P_Max = CStr(DrNPK(0).Item("P_Max")) 'P_Max
                        RigaDs.K_Max = CStr(DrNPK(0).Item("K_Max")) 'K_Max
                    End If

                End If

                DsAppezzamenti.DT_Ricetta_Appezzamenti.Rows.Add(RigaDs)

            Next

        End If


    End Sub

    Private Sub CaricaDsAppezzamentiRicetta3(TipoCodiceAppezzamento As Integer,
                                             flag_nascondiCampo As Boolean,
                                             ByVal BaseCode As Integer,
                                             ByVal Data_inizio_xFiltroDistinta As Date,
                                            ByVal Data_fine_xFiltroDistinta As Date,
                                            ByRef Dt_Impianti_Trattamenti As DataTable,
                                            ByRef Dt_Impianti_Fertilizzazioni As DataTable,
                                            ByRef Dt_Impianti_Trappole As DataTable)

        Dim objApp As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
        Dim objDestinazioni As New AgronicaCoreContabDAL.Ricette_Destinazioni_R
        'Dim Dt_Destinazioni As New DataTable
        Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim Dt_Impianti As DataTable

        Dim Dr_Impianti_Trattamenti As DataRow
        Dim Dr_Impianti_Ferti As DataRow
        Dim Dr_Impianti_Trap As DataRow
        'Dim RigaDs As DS_Ricetta_Appezzamenti.DT_Ricetta_AppezzamentiRow

        Dim DrAppezzaLav As DS_Ricetta3_Lavorazioni.DT_Lavorazioni_AppezzamentiRow
        'Dim DrAppezzaTrat As DS_Ricetta3_Trattamenti.DT_Trattamenti_AppezzamentiRow
        Dim DrTrat As DS_Ricetta3_Trattamenti.DT_Ricetta3_TrattamentiRow


        Dim i As Integer = 0

        Dt_Impianti = objDestinazioni.LeggiConDettagliImpiantoReale(Ricetta_Cod,
                                                                    0, 0, 0,
                                                                    "", 0, 0, 0,
                                                                    True, True,
                                                                    Data_inizio_xFiltroDistinta, Data_fine_xFiltroDistinta,
                                                                    " RD.Id_Reg <> 0", "",
                                                                    objParametri_Server)


        Dt_Impianti_Trattamenti = Dt_Impianti.Clone
        Dt_Impianti_Fertilizzazioni = Dt_Impianti.Clone
        Dt_Impianti_Trappole = Dt_Impianti.Clone

        objDestinazioni = Nothing

        'CAPIRE se va gestita anche questa parte:

        'Dim dtNPK As DataTable = GeneraStrutturaDTNPK()

        ''Se piano distribuzione concimi carico NPK Massimi per inserirli poi nel report
        ''If (Ricetta_tipo = enum_TipoRicetta.PianoDistribuzioneConcimi AndAlso Not Dt_Destinazioni Is Nothing AndAlso Dt_Destinazioni.Rows.Count > 0) Then
        'If (Ricetta_tipo = enum_TipoRicetta.PianoDistribuzioneConcimi AndAlso Not Dt_Impianti Is Nothing AndAlso Dt_Impianti.Rows.Count > 0) Then

        '    Dim objPC_EntitaxTestata_R As New AgronicaCoreAnagrafeDAL.PianoConcimazione_EntitaxTestata_R
        '    Dim Dt_EntitaxTestata As DataTable = objPC_EntitaxTestata_R.Leggi(Ricetta_Programmazione_Cod, 0, "", 0, 0, 0, 0, 0, "", "", "", 0, 0, "", 0, "", AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
        '    'Popolo la lista degli impianti collegati con tanto di NPK
        '    For Each drElem As DataRow In Dt_EntitaxTestata.Rows

        '        Dim piva = drElem.Item("Piva")
        '        Dim Sa_Cod = drElem.Item("Sa_Cod")
        '        Dim Appezza = drElem.Item("Appezza")
        '        Dim Id_reg = drElem.Item("Id_Imp")


        '        Dim dr As DataRow = dtNPK.NewRow()

        '        dr.Item("piva") = piva
        '        dr.Item("sa_cod") = Sa_Cod
        '        dr.Item("campo_cod") = drElem.Item("Campo_Cod")
        '        dr.Item("appezza") = Appezza
        '        dr.Item("id_reg") = Id_reg

        '        'Aggiungo i valori massimi di NPK per lo specifico impianto
        '        dr.Item("N_Max") = drElem.Item("QtaMaxN")
        '        dr.Item("P_Max") = drElem.Item("QtaMaxP2O5")
        '        dr.Item("K_Max") = drElem.Item("QtaMaxK2O")

        '        dtNPK.Rows.Add(dr)
        '    Next

        '    'Salvo un NPK MAX per poter fare il residuo nelle fertilizzazioni
        '    Me.N_Max = Dt_EntitaxTestata.Rows(0).Item("QtaMaxN")
        '    Me.P_Max = Dt_EntitaxTestata.Rows(0).Item("QtaMaxP2O5")
        '    Me.K_Max = Dt_EntitaxTestata.Rows(0).Item("QtaMaxK2O")

        'End If

        objImpianti = Nothing

        'Dim Sup_Tot As Double
        Dim N_app As String = ""
        Dim N_Appezza As Integer
        Dim lav_cod As Integer


        Dim hashImpianti As New Hashtable

        If Not Dt_Impianti Is Nothing AndAlso Dt_Impianti.Rows.Count > 0 Then

            For i = 0 To Dt_Impianti.Rows.Count - 1

                'Anna 16/05/22: [rif. chiamata 18108] Nella stampa degli Ordini di Lavoro vengono duplicate le righe degli Impianti se si stampa una ricetta che ha più di un prodotto e più di una destinazione.
                If Not hashImpianti.ContainsKey(Dt_Impianti.Rows(i).Item("piva") & "|" & Dt_Impianti.Rows(i).Item("sa_cod") &
                                                "|" & Dt_Impianti.Rows(i).Item("appezza") & "|" & Dt_Impianti.Rows(i).Item("Id_Reg")) Then

                    hashImpianti.Add(Dt_Impianti.Rows(i).Item("piva") & "|" & Dt_Impianti.Rows(i).Item("sa_cod") &
                                     "|" & Dt_Impianti.Rows(i).Item("appezza") & "|" & Dt_Impianti.Rows(i).Item("Id_Reg"), "")


                    lav_cod = Dt_Impianti.Rows(i).Item("lav_cod")

                    'Sup_Tot += CDbl(Dt_Impianti.Rows(i).Item("sup_imp"))

                    'N.ro appezzamento
                    N_app = Dt_Impianti.Rows(i).Item("app_nome_breve")

                    N_Appezza = Dt_Impianti.Rows(i).Item("appezza") - BaseCode

                    N_app = objApp.Numero_Appezzamento(Dt_Impianti.Rows(i).Item("piva"), Dt_Impianti.Rows(i).Item("sa_cod"),
                                                       Dt_Impianti.Rows(i).Item("campo_des"), Dt_Impianti.Rows(i).Item("appezza"),
                                                       TipoCodiceAppezzamento,
                                                       Dt_Impianti.Rows(i).Item("app_nome"), Dt_Impianti.Rows(i).Item("app_nome_breve"),
                                                       N_Appezza, BaseCode,
                                                       Nothing, flag_nascondiCampo)


                    '-----------------------
                    'tocca salvarsi i dati in un DT, da aggiungere successivamente al dataset del sottoreport in oggetto 
                    '(così gli appezzamenti sono ultimi, dopo i dettagli dell'operazione),
                    'poichè nel dataset non c'è il sort, quindi toccava passare da un dataview per poi ripassare al dataset
                    '(quindi ciclare in ogni caso)
                    Select Case lav_cod

                        Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_CONCIA_SEME,
                             LAVCOD_GEODISINFESTAZIONE, LAVCOD_TRATTAMENTO_FITOREGOLATORE,
                             LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO,
                             LAVCOD_DISTRIBUZIONE_INSETTI, LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE

                            Dr_Impianti_Trattamenti = Dt_Impianti_Trattamenti.NewRow

                            Dr_Impianti_Trattamenti.Item("campo_des") = Dt_Impianti.Rows(i).Item("campo_des")
                            Dr_Impianti_Trattamenti.Item("sa_nome") = Dt_Impianti.Rows(i).Item("sa_nome")
                            Dr_Impianti_Trattamenti.Item("veg_Des") = Dt_Impianti.Rows(i).Item("veg_Des")
                            Dr_Impianti_Trattamenti.Item("Cul_Des") = Dt_Impianti.Rows(i).Item("Cul_Des")
                            Dr_Impianti_Trattamenti.Item("app_nome") = N_app
                            Dr_Impianti_Trattamenti.Item("sup_imp") = CDbl(Dt_Impianti.Rows(i).Item("sup_imp"))
                            Dr_Impianti_Trattamenti.Item("reg_dpi") = Dt_Impianti.Rows(i).Item("reg_dpi")
                            Dr_Impianti_Trattamenti.Item("Appezza_Indirizzo") = Dt_Impianti.Rows(i).Item("Appezza_Indirizzo")
                            Dr_Impianti_Trattamenti.Item("Appezza_Rubrica") = Dt_Impianti.Rows(i).Item("Appezza_Rubrica")
                            Dr_Impianti_Trattamenti.Item("Ricetta_Operazione_Cod") = Dt_Impianti.Rows(i).Item("Ricetta_Operazione_Cod")

                            'Anna 16/05/22: [rif. chiamata 18108] Nella stampa la colonna Ha viene fuori la Superficie selezionata e non quella trattata
                            Dr_Impianti_Trattamenti.Item("Qta") = CDbl(Dt_Impianti.Rows(i).Item("Qta"))
                            Dr_Impianti_Trattamenti.Item("Qta2") = CDbl(Dt_Impianti.Rows(i).Item("Qta2"))

                            Dt_Impianti_Trattamenti.Rows.Add(Dr_Impianti_Trattamenti)

                            If i = 0 Then
                                Dim DrAppezzaTrat As DS_Ricetta3_Trattamenti.DT_Trattamenti_AppezzamentiRow
                                DrAppezzaTrat = DSTrattamenti3.DT_Trattamenti_Appezzamenti.NewDT_Trattamenti_AppezzamentiRow
                                DrAppezzaTrat.App_nome = ""
                                DrAppezzaTrat.App_nome = ""
                                DrAppezzaTrat.App_Nome_Breve = ""
                                DrAppezzaTrat.Sa_Nome = ""
                                DrAppezzaTrat.Campo_Nome = ""
                                DrAppezzaTrat.Veg_Des = ""
                                DrAppezzaTrat.CUL_DES = ""
                                DrAppezzaTrat.Regolamento = ""
                                DrAppezzaTrat.Sup_App = 0
                                DrAppezzaTrat.Indirizzo = ""
                                DrAppezzaTrat.Telefono = ""
                                DSTrattamenti3.DT_Trattamenti_Appezzamenti.Rows.Add(DrAppezzaTrat)
                            End If

                        Case LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_FERTIRRIGAZIONE, LAVCOD_TRATTAMENTO_ANTIBUTTERATURA, LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_DISTRIBUZIONE_AMMENDANTI, LAVCOD_SARCHIATURA_CONCIMAZIONE

                            Dr_Impianti_Ferti = Dt_Impianti_Fertilizzazioni.NewRow

                            Dr_Impianti_Ferti.Item("campo_des") = Dt_Impianti.Rows(i).Item("campo_des")
                            Dr_Impianti_Ferti.Item("sa_nome") = Dt_Impianti.Rows(i).Item("sa_nome")
                            Dr_Impianti_Ferti.Item("veg_Des") = Dt_Impianti.Rows(i).Item("veg_Des")
                            Dr_Impianti_Ferti.Item("Cul_Des") = Dt_Impianti.Rows(i).Item("Cul_Des")
                            Dr_Impianti_Ferti.Item("app_nome") = N_app
                            Dr_Impianti_Ferti.Item("sup_imp") = CDbl(Dt_Impianti.Rows(i).Item("sup_imp"))
                            Dr_Impianti_Ferti.Item("reg_dpi") = Dt_Impianti.Rows(i).Item("reg_dpi")
                            Dr_Impianti_Ferti.Item("Appezza_Indirizzo") = Dt_Impianti.Rows(i).Item("Appezza_Indirizzo")
                            Dr_Impianti_Ferti.Item("Appezza_Rubrica") = Dt_Impianti.Rows(i).Item("Appezza_Rubrica")
                            Dr_Impianti_Ferti.Item("Ricetta_Operazione_Cod") = Dt_Impianti.Rows(i).Item("Ricetta_Operazione_Cod")

                            'Anna 16/05/22: [rif. chiamata 18108] Nella stampa la colonna Ha viene fuori la Superficie selezionata e non quella trattata
                            Dr_Impianti_Ferti.Item("Qta") = CDbl(Dt_Impianti.Rows(i).Item("Qta"))
                            Dr_Impianti_Ferti.Item("Qta2") = CDbl(Dt_Impianti.Rows(i).Item("Qta2"))

                            Dt_Impianti_Fertilizzazioni.Rows.Add(Dr_Impianti_Ferti)

                        Case LAVCOD_IRRIGAZIONE



                        Case LAVCOD_INSTALLAZIONE_TRAPPOLE,
                             LAVCOD_CATTURE_MASSA,
                             LAVCOD_CONFUSIONE_SESSUALE,
                             LAVCOD_DISORIENTAMENTO_SESSUALE

                            Dr_Impianti_Trap = Dt_Impianti_Trappole.NewRow
                            Dr_Impianti_Trap.Item("campo_des") = Dt_Impianti.Rows(i).Item("campo_des")
                            Dr_Impianti_Trap.Item("sa_nome") = Dt_Impianti.Rows(i).Item("sa_nome")
                            Dr_Impianti_Trap.Item("veg_Des") = Dt_Impianti.Rows(i).Item("veg_Des")
                            Dr_Impianti_Trap.Item("Cul_Des") = Dt_Impianti.Rows(i).Item("Cul_Des")
                            Dr_Impianti_Trap.Item("app_nome") = N_app
                            Dr_Impianti_Trap.Item("sup_imp") = CDbl(Dt_Impianti.Rows(i).Item("sup_imp"))
                            Dr_Impianti_Trap.Item("reg_dpi") = Dt_Impianti.Rows(i).Item("reg_dpi")
                            Dr_Impianti_Trap.Item("Appezza_Indirizzo") = Dt_Impianti.Rows(i).Item("Appezza_Indirizzo")
                            Dr_Impianti_Trap.Item("Appezza_Rubrica") = Dt_Impianti.Rows(i).Item("Appezza_Rubrica")
                            Dr_Impianti_Trap.Item("Ricetta_Operazione_Cod") = Dt_Impianti.Rows(i).Item("Ricetta_Operazione_Cod")

                            'Anna 16/05/22: [rif. chiamata 18108] Nella stampa la colonna Ha viene fuori la Superficie selezionata e non quella trattata
                            Dr_Impianti_Trap.Item("Qta") = CDbl(Dt_Impianti.Rows(i).Item("Qta"))
                            Dr_Impianti_Trap.Item("Qta2") = CDbl(Dt_Impianti.Rows(i).Item("Qta2"))

                            Dt_Impianti_Trappole.Rows.Add(Dr_Impianti_Trap)

                        Case Else

                            DrAppezzaLav = DSLavorazioni3.DT_Lavorazioni_Appezzamenti.NewDT_Lavorazioni_AppezzamentiRow
                            DrAppezzaLav.Campo_Nome = Dt_Impianti.Rows(i).Item("campo_des")
                            DrAppezzaLav.Sa_Nome = Dt_Impianti.Rows(i).Item("sa_nome")
                            DrAppezzaLav.Veg_Des = Dt_Impianti.Rows(i).Item("veg_Des")
                            DrAppezzaLav.CUL_DES = Dt_Impianti.Rows(i).Item("Cul_Des")
                            DrAppezzaLav.App_nome = N_app
                            DrAppezzaLav.Sup_App = CDbl(Dt_Impianti.Rows(i).Item("sup_imp"))
                            DrAppezzaLav.Regolamento = Dt_Impianti.Rows(i).Item("reg_dpi")
                            DrAppezzaLav.Indirizzo = Dt_Impianti.Rows(i).Item("Appezza_Indirizzo")
                            DrAppezzaLav.Telefono = Dt_Impianti.Rows(i).Item("Appezza_Rubrica")
                            DrAppezzaLav.Ricetta_Operazione_Cod = Dt_Impianti.Rows(i).Item("Ricetta_Operazione_Cod")

                            DSLavorazioni3.DT_Lavorazioni_Appezzamenti.Rows.Add(DrAppezzaLav)

                    End Select 'lav_cod
                End If
            Next
        End If
    End Sub

    '###########################################################################
    Private Sub Crea_Dt_Trattamenti()

        Dim Dt As New DataTable

        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("Av_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Av_Gru", GetType(String)))
        Dt.Columns.Add(New DataColumn("Av_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Soglia_Value", GetType(String)))
        Dt.Columns.Add(New DataColumn("Soglia_Des", GetType(String)))

        Dt.Columns.Add(New DataColumn("Fr_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Fr_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Carenza", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dose_Etichetta", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dose_Etichetta_Max", GetType(String)))
        Dt.Columns.Add(New DataColumn("Udm_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Udm_Sim", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dose", GetType(Double)))
        Dt.Columns.Add(New DataColumn("Dose_Fittizia", GetType(Double)))
        Dt.Columns.Add(New DataColumn("Mezzo", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Qta_Tot", GetType(Double)))
        Dt.Columns.Add(New DataColumn("Extra_Str", GetType(String)))

        'Dt.Columns.Add(New DataColumn("Fr_Cod2", GetType(Integer)))
        'Dt.Columns.Add(New DataColumn("Fr_Des2", GetType(String)))
        'Dt.Columns.Add(New DataColumn("Carenza2", GetType(String)))
        'Dt.Columns.Add(New DataColumn("Dose_Etichetta2", GetType(String)))
        'Dt.Columns.Add(New DataColumn("Dose_Etichetta_Max2", GetType(String)))
        'Dt.Columns.Add(New DataColumn("Udm_Cod2", GetType(Integer)))
        'Dt.Columns.Add(New DataColumn("Udm_Sim2", GetType(String)))
        'Dt.Columns.Add(New DataColumn("Dose2", GetType(Double)))
        'Dt.Columns.Add(New DataColumn("Dose_Fittizia2", GetType(Double)))
        'Dt.Columns.Add(New DataColumn("Mezzo2", GetType(Integer)))
        'Dt.Columns.Add(New DataColumn("Qta_Tot2", GetType(Double)))

        'Dt.Columns.Add(New DataColumn("Fr_Cod3", GetType(Integer)))
        'Dt.Columns.Add(New DataColumn("Fr_Des3", GetType(String)))
        'Dt.Columns.Add(New DataColumn("Carenza3", GetType(String)))
        'Dt.Columns.Add(New DataColumn("Dose_Etichetta3", GetType(String)))
        'Dt.Columns.Add(New DataColumn("Dose_Etichetta_Max3", GetType(String)))
        'Dt.Columns.Add(New DataColumn("Udm_Cod3", GetType(Integer)))
        'Dt.Columns.Add(New DataColumn("Udm_Sim3", GetType(String)))
        'Dt.Columns.Add(New DataColumn("Dose3", GetType(Double)))
        'Dt.Columns.Add(New DataColumn("Dose_Fittizia3", GetType(Double)))
        'Dt.Columns.Add(New DataColumn("Mezzo3", GetType(Integer)))
        'Dt.Columns.Add(New DataColumn("Qta_Tot3", GetType(Double)))


        'Dt.Columns.Add(New DataColumn("Fr_Cod4", GetType(Integer)))
        'Dt.Columns.Add(New DataColumn("Fr_Des4", GetType(String)))
        'Dt.Columns.Add(New DataColumn("Carenza4", GetType(String)))
        'Dt.Columns.Add(New DataColumn("Dose_Etichetta4", GetType(String)))
        'Dt.Columns.Add(New DataColumn("Dose_Etichetta_Max4", GetType(String)))
        'Dt.Columns.Add(New DataColumn("Udm_Cod4", GetType(Integer)))
        'Dt.Columns.Add(New DataColumn("Udm_Sim4", GetType(String)))
        'Dt.Columns.Add(New DataColumn("Dose4", GetType(Double)))
        'Dt.Columns.Add(New DataColumn("Dose_Fittizia4", GetType(Double)))
        'Dt.Columns.Add(New DataColumn("Mezzo4", GetType(Integer)))
        'Dt.Columns.Add(New DataColumn("Qta_Tot4", GetType(Double)))

        Dt.Columns.Add(New DataColumn("Data", GetType(String)))

        Dt.Columns.Add(New DataColumn("Destinazioni", GetType(String)))
        '----- Definisco l'insieme di colonne che costituiscono la chiave della tabella

        'Vettore di DataColumn
        Dim DtKeys(2) As DataColumn

        'Valorizzo le celle del vettore
        DtKeys(0) = Dt.Columns("Av_Cod")
        DtKeys(1) = Dt.Columns("Av_Gru")
        DtKeys(2) = Dt.Columns("Fr_Cod")
        'DtKeys(3) = Dt.Columns("Fr_Cod2")
        'DtKeys(4) = Dt.Columns("Fr_Cod3")
        'DtKeys(4) = Dt.Columns("Fr_Cod4")

        'Assegno il vettore delle chiavi al DataTable
        Dt.PrimaryKey = DtKeys

        '----- Salvo il DataTable dentro il viewstate

        ViewState("dtDosi_Difesa") = Dt


    End Sub

    '###########################################################################
    'carico il dataset x i dati relativi ai trattamenti
    Private Sub Inserisci_Trattamento(ByVal strOperazione As String,
                                      ByVal bArrotondaAcqua As Boolean, ByVal bNascondiMacchine As Boolean,
                                      ByVal bNascondiOperatori As Boolean, ByVal bNascondiAutorizzatore As Boolean,
                                      ByVal bNascondiDataUltimaManutenzione As Boolean,
                                      ByVal TipoCodiceAppezzamento As Integer,
                                      ByVal flag_nascondiCampo As Boolean,
                                      ByVal BaseCode As Integer)

        Dim Dt As DataTable

        Dim XmlDoc As New System.Xml.XmlDocument

        Dim XML_Ricetta_Operazione As System.Xml.XmlElement

        Dim XML_DatiRicetta_Dettagli_Tecnici As System.Xml.XmlElement
        Dim XML_Ricetta_Dettaglio_Tecnico As System.Xml.XmlElement
        Dim XMLs_Ricetta_Dettaglio_Tecnico As System.Xml.XmlNodeList

        Dim XML_DatiRicetta_Dettagli As System.Xml.XmlElement
        Dim XML_Ricetta_Dettaglio As System.Xml.XmlElement
        Dim XMLs_Ricetta_Dettaglio As System.Xml.XmlNodeList

        Dim XML_Ricetta_Dettaglio_Tecnico_2 As System.Xml.XmlElement
        Dim XMLs_Ricetta_Dettaglio_Tecnico_2 As System.Xml.XmlNodeList

        Dim XML_Ricetta_Dettaglio_Destinazione As System.Xml.XmlElement
        Dim XMLs_Ricetta_Dettaglio_Destinazione As System.Xml.XmlNodeList

        Dim Lav_Cod As String
        Dim Lav_Des As String

        Dim frm_Data As String

        Dim frm_OperazioneCod As Integer
        Dim frm_OperazioneDes As String
        Dim frm_NoteIntervento As String

        Dim frm_CodDisciplinare As Integer
        Dim frm_Modulo As Integer
        Dim frm_IdRcdpi As Integer
        Dim frm_Mezzo As Integer

        Dim frm_AvCod() As String
        Dim frm_AvDes() As String
        Dim frm_AvGru() As String
        Dim frm_AvGruDes() As String
        Dim frm_SogliaValue() As String
        Dim frm_SogliaDes() As String

        Dim frm_Dest As String

        Dim frm_Acqua As Double = 0
        Dim frm_Acqua_Tot As Double = 0

        Dim frm_Cau_Mov As String

        Dim frm_ElemCod As Integer

        Dim frm_MiscelaCod() As Integer
        Dim frm_FrCod() As Integer
        Dim frm_FrDes() As String
        Dim frm_UdmCod() As Integer
        Dim frm_UdmSim() As String
        Dim frm_UdmCodTrasformato() As Integer
        Dim frm_Dose() As Double
        Dim frm_ForVegCod() As String

        Dim i, j, a, m As Integer

        Dim listaMacCod As New List(Of String)

        '''''' note '''''
        Dim frm_RicettaNote As String = ""
        Dim XMLs_Ricetta_Note As System.Xml.XmlNodeList
        Dim XML_DatiRicetta_Note2 As System.Xml.XmlElement

        Dim TrovatoImpianto As Boolean
        Dim ArrayPiva() As String
        Dim ArraySa_Cod() As Integer
        Dim ArrayAppezza() As Integer
        Dim ArrayId_Reg() As Integer

        Dim strCosti As String
        Dim strMacchina As String
        Dim Mac_Des As String
        Dim strCostiMacchine As String
        Dim strCostiOperatori As String
        Dim strCostiResponsabile As String

        Dim nomeContatto As String

        Dim Sup_Tot_Op As Decimal = 0

        Dim PrimoDettaglio As Boolean = True

        Dim DictMagazziniEsterni As New Dictionary(Of (Integer, String, String), String)

        '------------------------------------------
        '----- Analizzo la stringa XML
        '------------------------------------------

        'Carico la stringa nel documento XML
        XmlDoc.LoadXml(strOperazione)

        '----- Tag Ricetta_Operazione

        XML_Ricetta_Operazione = XmlDoc.SelectSingleNode("Ricetta_Operazione")

        frm_OperazioneCod = CStr(XML_Ricetta_Operazione.GetAttribute("ricetta_operazione_cod"))
        frm_OperazioneDes = CStr(XML_Ricetta_Operazione.GetAttribute("ricetta_operazione_des"))

        Dim objRDest As New AgronicaCoreContabDAL.Ricette_Destinazioni_R
        Sup_Tot_Op = objRDest.SupTotTrattata_from_RicettaOperazioneCod(frm_OperazioneCod, "", objParametri_Server)

        Lav_Cod = CInt(XML_Ricetta_Operazione.GetAttribute("lav_cod"))
        Lav_Des = CStr(XML_Ricetta_Operazione.GetAttribute("lav_des"))

        frm_Data = CStr(XML_Ricetta_Operazione.GetAttribute("validita_inizio"))

        frm_NoteIntervento = CStr(XML_Ricetta_Operazione.GetAttribute("note"))

        'Disciplinari
        frm_CodDisciplinare = CInt(XML_Ricetta_Operazione.GetAttribute("num_protocollo"))

        'Rcdpi - Modulo

        If XML_Ricetta_Operazione.GetAttribute("num_protocollo") <> "0" Then
            If XML_Ricetta_Operazione.GetAttribute("id_rcdpi") <> "0" Then
                frm_IdRcdpi = CInt(XML_Ricetta_Operazione.GetAttribute("id_rcdpi"))
            End If
            If XML_Ricetta_Operazione.GetAttribute("extra_int") <> "0" Then
                frm_Modulo = CInt(XML_Ricetta_Operazione.GetAttribute("extra_int"))
            End If
        End If

        frm_Mezzo = CInt(XML_Ricetta_Operazione.GetAttribute("mezzo"))


        If XML_Ricetta_Operazione.HasChildNodes Then

            '----- Tag DatiRicetta_Dettagli_Tecnici

            XML_DatiRicetta_Dettagli_Tecnici = XML_Ricetta_Operazione.SelectSingleNode("DatiRicetta_Dettagli_Tecnici")

            If Not XML_DatiRicetta_Dettagli_Tecnici Is Nothing Then

                '----- Tag Ricetta_Dettaglio_Tecnico  (multiplo)

                'Recupero la collezione dei nodi
                XMLs_Ricetta_Dettaglio_Tecnico = XML_DatiRicetta_Dettagli_Tecnici.GetElementsByTagName("Ricetta_Dettaglio_Tecnico")

                Dim xAvCod As Integer
                Dim xAvGru As Integer
                Dim xAvDes As String
                Dim xAvGruDes As String
                Dim xQtaAcqua As Double

                'Ciclo su tutti i nodi
                For i = 0 To XMLs_Ricetta_Dettaglio_Tecnico.Count - 1

                    'Prendo l'i-esimo nodo della collezione
                    XML_Ricetta_Dettaglio_Tecnico = XMLs_Ricetta_Dettaglio_Tecnico.Item(i)

                    'Recupero i valori
                    xAvCod = CInt(XML_Ricetta_Dettaglio_Tecnico.GetAttribute("av_cod"))
                    xAvGru = CInt(XML_Ricetta_Dettaglio_Tecnico.GetAttribute("av_gru"))
                    xAvDes = CStr(XML_Ricetta_Dettaglio_Tecnico.GetAttribute("av_des_vol"))
                    xAvGruDes = CStr(XML_Ricetta_Dettaglio_Tecnico.GetAttribute("av_gru_des"))
                    xQtaAcqua = CDbl(XML_Ricetta_Dettaglio_Tecnico.GetAttribute("qta_ril"))

                    If (xAvCod = 0) And (xAvGru = 0) And (xQtaAcqua <> 0) Then
                        'Acqua
                        frm_Acqua = xQtaAcqua
                    ElseIf ((xAvCod <> 0) Or (xAvGru <> 0)) And (xQtaAcqua = 0) Then
                        If frm_CodDisciplinare = 0 Then
                            'Avversita'
                            If frm_AvCod Is Nothing Then
                                ReDim Preserve frm_AvCod(0)
                                ReDim Preserve frm_AvGru(0)
                                ReDim Preserve frm_AvDes(0)
                                ReDim Preserve frm_AvGruDes(0)
                            Else
                                ReDim Preserve frm_AvCod(UBound(frm_AvCod) + 1)
                                ReDim Preserve frm_AvGru(UBound(frm_AvGru) + 1)
                                ReDim Preserve frm_AvDes(UBound(frm_AvDes) + 1)
                                ReDim Preserve frm_AvGruDes(UBound(frm_AvGruDes) + 1)
                            End If

                            frm_AvCod(UBound(frm_AvCod)) = xAvCod
                            frm_AvGru(UBound(frm_AvGru)) = xAvGru
                            frm_AvDes(UBound(frm_AvDes)) = xAvDes
                            frm_AvGruDes(UBound(frm_AvGruDes)) = xAvGruDes
                        End If
                    End If
                Next
            End If

            '--------------------------------
            '   INIZIO NOTE
            '--------------------------------
            '----- Tag DatiRicetta_Note2
            Dim objNoteIntervento As New AgronicaCoreContabDAL.Note_Intervento_R
            XML_DatiRicetta_Note2 = XML_Ricetta_Operazione.SelectSingleNode("DatiRicettaxNote_2")
            If Not XML_DatiRicetta_Note2 Is Nothing Then

                '----- Tag RicettaxNote_2
                XMLs_Ricetta_Note = XML_DatiRicetta_Note2.GetElementsByTagName("RicettaxNote_2")

                If Not XMLs_Ricetta_Note Is Nothing AndAlso XMLs_Ricetta_Note.Count > 0 Then

                    For j = 0 To XMLs_Ricetta_Note.Count - 1

                        XML_DatiRicetta_Note2 = XMLs_Ricetta_Note.Item(j)
                        frm_RicettaNote &= objNoteIntervento.NotaDesFromNotaCod(
                        XML_DatiRicetta_Note2.GetAttribute("nota_cod"),
                        Session("ASG_objParametri_Server")) & ", "

                    Next

                    If frm_RicettaNote <> "" Then
                        frm_RicettaNote = Left(frm_RicettaNote, frm_RicettaNote.Length - 2)
                    End If
                End If
            End If
            '--------------------------------
            '   FINE NOTE
            '--------------------------------



            '----- Tag XML_DatiRicetta_Dettagli

            XML_DatiRicetta_Dettagli = XML_Ricetta_Operazione.SelectSingleNode("DatiRicetta_Dettagli")

            If Not XML_DatiRicetta_Dettagli Is Nothing Then

                '----- Tag Ricetta_Dettaglio  (multiplo)

                'Recupero la collezione dei nodi
                XMLs_Ricetta_Dettaglio = XML_DatiRicetta_Dettagli.GetElementsByTagName("Ricetta_Dettaglio")

                For i = 0 To XMLs_Ricetta_Dettaglio.Count - 1

                    'Prendo l'i-esimo nodo della collezione
                    XML_Ricetta_Dettaglio = XMLs_Ricetta_Dettaglio.Item(i)

                    frm_Cau_Mov = XML_Ricetta_Dettaglio.GetAttribute("cau_mov")

                    Select Case frm_Cau_Mov

                        Case "", CAU_TRATTAMENTO

                            XMLs_Ricetta_Dettaglio_Tecnico_2 = XML_Ricetta_Dettaglio.GetElementsByTagName("Ricetta_Dettaglio_Tecnico_2")

                            If Not XMLs_Ricetta_Dettaglio_Tecnico_2 Is Nothing AndAlso XMLs_Ricetta_Dettaglio_Tecnico_2.Count > 0 Then

                                Dim DPI2005 As Integer = 0
                                If frm_CodDisciplinare <> DPI2005 OrElse Lav_Cod = LAVCOD_DISTRIBUZIONE_INSETTI Then
                                    If i = 0 Then
                                        ReDim Preserve frm_AvCod(0)
                                        ReDim Preserve frm_AvGru(0)
                                        ReDim Preserve frm_AvDes(0)
                                        ReDim Preserve frm_AvGruDes(0)
                                        ReDim Preserve frm_SogliaValue(0)
                                        ReDim Preserve frm_SogliaDes(0)
                                    Else
                                        ReDim Preserve frm_AvCod(UBound(frm_AvCod) + 1)
                                        ReDim Preserve frm_AvGru(UBound(frm_AvGru) + 1)
                                        ReDim Preserve frm_AvDes(UBound(frm_AvDes) + 1)
                                        ReDim Preserve frm_AvGruDes(UBound(frm_AvGruDes) + 1)
                                        ReDim Preserve frm_SogliaValue(UBound(frm_SogliaValue) + 1)
                                        ReDim Preserve frm_SogliaDes(UBound(frm_SogliaDes) + 1)
                                    End If
                                End If

                                For j = 0 To XMLs_Ricetta_Dettaglio_Tecnico_2.Count - 1

                                    XML_Ricetta_Dettaglio_Tecnico_2 = XMLs_Ricetta_Dettaglio_Tecnico_2.Item(j)

                                    frm_AvCod(UBound(frm_AvCod)) &= CStr(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("av_cod")) & ","
                                    frm_AvGru(UBound(frm_AvGru)) &= CInt(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("av_gru")) & ","

                                    frm_AvDes(UBound(frm_AvDes)) &= CStr(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("av_des_vol")) & ","
                                    frm_AvGruDes(UBound(frm_AvGruDes)) &= CStr(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("av_gru_des")) & ","

                                    frm_SogliaValue(UBound(frm_SogliaValue)) &= CStr(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("av_cod")) & "_" & CStr(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("soglia_quantita")) & "_" & CStr(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("soglia_cod")) & ","
                                    frm_SogliaDes(UBound(frm_SogliaDes)) &= CStr(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("soglia_des")) & ","

                                Next

                                If frm_AvCod(UBound(frm_AvCod)) <> "" Then
                                    frm_AvCod(UBound(frm_AvCod)) = Left(frm_AvCod(UBound(frm_AvCod)), frm_AvCod(UBound(frm_AvCod)).Length - 1)
                                End If
                                If frm_AvGru(UBound(frm_AvGru)) <> "" Then
                                    frm_AvGru(UBound(frm_AvGru)) = Left(frm_AvGru(UBound(frm_AvGru)), frm_AvGru(UBound(frm_AvGru)).Length - 1)
                                End If
                                If frm_AvDes(UBound(frm_AvDes)) <> "" Then
                                    frm_AvDes(UBound(frm_AvDes)) = Left(frm_AvDes(UBound(frm_AvDes)), frm_AvDes(UBound(frm_AvDes)).Length - 1)
                                End If
                                If frm_AvGruDes(UBound(frm_AvGruDes)) <> "" Then
                                    frm_AvGruDes(UBound(frm_AvGruDes)) = Left(frm_AvGruDes(UBound(frm_AvGruDes)), frm_AvGruDes(UBound(frm_AvGruDes)).Length - 1)
                                End If
                                If frm_SogliaValue(UBound(frm_SogliaValue)) <> "" Then
                                    frm_SogliaValue(UBound(frm_SogliaValue)) = Left(frm_SogliaValue(UBound(frm_SogliaValue)), frm_SogliaValue(UBound(frm_SogliaValue)).Length - 1)
                                End If
                                If frm_SogliaDes(UBound(frm_SogliaDes)) <> "" Then
                                    frm_SogliaDes(UBound(frm_SogliaDes)) = Left(frm_SogliaDes(UBound(frm_SogliaDes)), frm_SogliaDes(UBound(frm_SogliaDes)).Length - 1)
                                End If

                            End If

                            'Inizializzo le variabili
                            If frm_FrCod Is Nothing Then

                                ReDim Preserve frm_FrCod(0)
                                ReDim Preserve frm_FrDes(0)
                                'ReDim Preserve frm_PrincipiA(0)
                                ReDim Preserve frm_MiscelaCod(0)
                                ReDim Preserve frm_UdmCod(0)
                                ReDim Preserve frm_UdmCodTrasformato(0)
                                ReDim Preserve frm_UdmSim(0)
                                ReDim Preserve frm_Dose(0)
                                ReDim Preserve frm_ForVegCod(0)

                            Else

                                ReDim Preserve frm_FrCod(UBound(frm_FrCod) + 1)
                                ReDim Preserve frm_FrDes(UBound(frm_FrDes) + 1)
                                'ReDim Preserve frm_PrincipiA(UBound(frm_PrincipiA) + 1)
                                ReDim Preserve frm_MiscelaCod(UBound(frm_MiscelaCod) + 1)
                                ReDim Preserve frm_UdmCodTrasformato(UBound(frm_UdmCodTrasformato) + 1)
                                ReDim Preserve frm_UdmCod(UBound(frm_UdmCod) + 1)
                                ReDim Preserve frm_UdmSim(UBound(frm_UdmSim) + 1)
                                ReDim Preserve frm_Dose(UBound(frm_Dose) + 1)
                                ReDim Preserve frm_ForVegCod(UBound(frm_ForVegCod) + 1)

                            End If

                            '--------------------------------
                            '   INIZIO DESTINAZIONI
                            '--------------------------------
                            XMLs_Ricetta_Dettaglio_Destinazione = XML_Ricetta_Dettaglio.GetElementsByTagName("Ricetta_Destinazione")

                            If Not XMLs_Ricetta_Dettaglio_Destinazione Is Nothing AndAlso XMLs_Ricetta_Dettaglio_Destinazione.Count > 0 Then

                                For j = 0 To XMLs_Ricetta_Dettaglio_Destinazione.Count - 1

                                    XML_Ricetta_Dettaglio_Destinazione = XMLs_Ricetta_Dettaglio_Destinazione.Item(j)

                                    TrovatoImpianto = False

                                    If Not ArrayPiva Is Nothing Then
                                        For a = 0 To ArrayPiva.Length - 1
                                            If ArrayPiva(a) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("piva") And
                                               ArraySa_Cod(a) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("sa_cod") And
                                               ArrayAppezza(a) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("appezza") And
                                               ArrayId_Reg(a) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("id_reg") Then
                                                TrovatoImpianto = True
                                                Exit For
                                            End If
                                        Next
                                    End If

                                    If TrovatoImpianto = False Then
                                        If ArrayPiva Is Nothing Then
                                            ReDim Preserve ArrayPiva(0)
                                            ReDim Preserve ArraySa_Cod(0)
                                            ReDim Preserve ArrayAppezza(0)
                                            ReDim Preserve ArrayId_Reg(0)
                                        Else
                                            ReDim Preserve ArrayPiva(UBound(ArrayPiva) + 1)
                                            ReDim Preserve ArraySa_Cod(UBound(ArraySa_Cod) + 1)
                                            ReDim Preserve ArrayAppezza(UBound(ArrayAppezza) + 1)
                                            ReDim Preserve ArrayId_Reg(UBound(ArrayId_Reg) + 1)
                                        End If
                                        ArrayPiva(UBound(ArrayPiva)) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("piva")
                                        ArraySa_Cod(UBound(ArraySa_Cod)) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("sa_cod")
                                        ArrayAppezza(UBound(ArrayAppezza)) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("appezza")
                                        ArrayId_Reg(UBound(ArrayId_Reg)) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("id_reg")
                                    End If

                                Next

                            End If
                            '--------------------------------
                            '   FINE DESTINAZIONI
                            '--------------------------------


                            'Recupero i valori
                            frm_FrCod(UBound(frm_FrCod)) = CInt(XML_Ricetta_Dettaglio.GetAttribute("pro_cod"))
                            frm_ElemCod = CInt(XML_Ricetta_Dettaglio.GetAttribute("elem_cod"))

                            frm_ForVegCod(UBound(frm_ForVegCod)) = "0"
                            If XML_Ricetta_Dettaglio.HasAttribute("extra_str") AndAlso IsNumeric(XML_Ricetta_Dettaglio.GetAttribute("extra_str")) Then
                                frm_ForVegCod(UBound(frm_ForVegCod)) = XML_Ricetta_Dettaglio.GetAttribute("extra_str")
                            End If

                            Select Case frm_ElemCod
                                Case FORMULATI
                                    frm_FrDes(UBound(frm_FrDes)) = CStr(XML_Ricetta_Dettaglio.GetAttribute("fr_des"))
                                Case TRAPPOLE
                                    frm_FrDes(UBound(frm_FrDes)) = CStr(XML_Ricetta_Dettaglio.GetAttribute("trap_des"))
                                Case INSETTI
                                    frm_FrDes(UBound(frm_FrDes)) = CStr(XML_Ricetta_Dettaglio.GetAttribute("ins_des"))
                            End Select

                            frm_MiscelaCod(UBound(frm_MiscelaCod)) = CInt(XML_Ricetta_Dettaglio.GetAttribute("miscela_cod"))
                            frm_UdmCod(UBound(frm_UdmCod)) = CInt(XML_Ricetta_Dettaglio.GetAttribute("extra_int"))
                            frm_UdmCodTrasformato(UBound(frm_UdmCodTrasformato)) = CInt(XML_Ricetta_Dettaglio.GetAttribute("udm_cod"))
                            frm_UdmSim(UBound(frm_UdmSim)) = CStr(XML_Ricetta_Dettaglio.GetAttribute("udm_sim"))
                            frm_Dose(UBound(frm_Dose)) = CDbl(XML_Ricetta_Dettaglio.GetAttribute("qta"))

                        Case CAU_IMPUTAZIONE_PARCOMACCHINE

                            Dim objMAc As New AgronicaCoreContabDAL.Parco_Macchine_R
                            Dim dt_Mac As DataTable
                            strMacchina = ""
                            Mac_Des = ""

                            dt_Mac = objMAc.ParcoMacchine_Leggi("",
                                                                CInt(XML_Ricetta_Dettaglio.GetAttribute("mat_cod")),
                                                                False,
                                                                "", "", "", "", "", 0,
                                                                "", False, 0, "", True,
                                                                AGRODATAINIZIO,
                                                                AGRODATAFINE,
                                                                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                "", "",
                                                                objParametri_Server)
                            objMAc = Nothing

                            If dt_Mac.Rows.Count > 0 Then
                                Select Case Qs_Stampa_Tipo

                                    Case enum_TipoStampaRicetta.Certificazione

                                        If dt_Mac.Rows(0).Item("Modello") <> "" And dt_Mac.Rows(0).Item("Ultima_Manutenzione") <> "01/01/1900" Then
                                            strMacchina += dt_Mac.Rows(0).Item("CLASS_DESC") + " (Modello " + dt_Mac.Rows(0).Item("Modello")
                                            If Not bNascondiDataUltimaManutenzione Then
                                                strMacchina += "; Ultima Manutenzione " + dt_Mac.Rows(0).Item("Ultima_Manutenzione") + "),"
                                            Else
                                                strMacchina += "),"
                                            End If
                                        ElseIf dt_Mac.Rows(0).Item("Modello") <> "" And dt_Mac.Rows(0).Item("Ultima_Manutenzione") = "01/01/1900" Then
                                            strMacchina += dt_Mac.Rows(0).Item("CLASS_DESC") + " (Modello " + dt_Mac.Rows(0).Item("Modello") + "),"
                                        ElseIf dt_Mac.Rows(0).Item("Modello") = "" And dt_Mac.Rows(0).Item("Ultima_Manutenzione") <> "01/01/1900" Then
                                            strMacchina += dt_Mac.Rows(0).Item("CLASS_DESC")
                                            If Not bNascondiDataUltimaManutenzione Then
                                                strMacchina += "; Ultima Manutenzione " + dt_Mac.Rows(0).Item("Ultima_Manutenzione") + "),"
                                            Else
                                                strMacchina += "),"
                                            End If
                                        ElseIf dt_Mac.Rows(0).Item("Modello") = "" And dt_Mac.Rows(0).Item("Ultima_Manutenzione") = "01/01/1900" Then
                                            strMacchina += dt_Mac.Rows(0).Item("CLASS_DESC") + ","
                                        End If

                                        If InStr(strCostiMacchine, strMacchina) = 0 Then
                                            strCostiMacchine &= strMacchina
                                        End If

                                    Case enum_TipoStampaRicetta.Aziendale

                                        If dt_Mac.Rows(0).Item("mac_des") <> "" Then
                                            strMacchina = dt_Mac.Rows(0).Item("mac_des")
                                        Else
                                            strMacchina = dt_Mac.Rows(0).Item("CLASS_DESC")
                                        End If

                                        If dt_Mac.Rows(0).Item("ditta_des") <> "" Then
                                            strMacchina += " - " & dt_Mac.Rows(0).Item("ditta_des")
                                        End If

                                        If dt_Mac.Rows(0).Item("Modello") <> "" Then
                                            strMacchina += " - Modello " & dt_Mac.Rows(0).Item("Modello")
                                        End If

                                        If InStr(strCostiMacchine, strMacchina) = 0 Then
                                            strCostiMacchine &= strMacchina
                                        End If

                                        Dim Class_Code As String = dt_Mac.Rows(0).Item("CLASS_code")
                                        Dim strCaratteristiche As String = ""

                                        'CARATTERISTICHE MACCHINE
                                        Dim DtCar As DataTable
                                        Dim DrCar() As DataRow
                                        Dim objCar As New AgronicaCoreMetaSchemaDAL.MacchinexCaratteristiche_R
                                        DtCar = objCar.Leggi(0, Class_Code, "", "", objParametri_Server)


                                        Dim objProfilazioneR As New AgronicaCoreProfilazioneBIZ.Profilazione_R
                                        Dim DS_Macchine As DataSet
                                        DS_Macchine = objProfilazioneR.LeggiProfilazioneMacchine_In_Cascata(Piva,
                                                                                 Lav_Cod, Veg_Cod,
                                                                                 objParametri_Server)

                                        If Not DS_Macchine Is Nothing Then
                                            For Each dtDati As DataTable In DS_Macchine.Tables
                                                For Each drDati In dtDati.Rows
                                                    If (dtDati.TableName.Equals("macXlav")) Then
                                                        If drDati("mac_cod") = XML_Ricetta_Dettaglio.GetAttribute("mat_cod") Then
                                                            'Leggo se il macchinario è profilato
                                                            Dim pmp As New AgronicaCoreProfilazioneDAL.Profilazione_Macchine_R
                                                            Dim dtMP As DataTable
                                                            dtMP = pmp.Leggi(CInt(drDati("Id_Profilo_Dati")),
                                                                             CInt(XML_Ricetta_Dettaglio.GetAttribute("mat_cod")), 0,
                                                                             "",
                                                                             "",
                                                                             objParametri_Server)

                                                            For m = 0 To dtMP.Rows.Count - 1
                                                                DrCar = DtCar.Select("mac_car_cod=" & dtMP.Rows(m).Item("mac_car_cod"))
                                                                If Not DrCar Is Nothing AndAlso DrCar.Length > 0 Then
                                                                    strCaratteristiche &= DrCar(0).Item("mac_car_des") & ":" & dtMP.Rows(m).Item("valore") & "; "
                                                                End If
                                                            Next
                                                        End If
                                                    End If
                                                Next
                                            Next
                                        End If

                                        If strMacchina <> "" Then
                                            strMacchina = Left(strMacchina, strMacchina.Length - 1)
                                        End If

                                        If strCaratteristiche <> "" Then
                                            strCaratteristiche = Left(strCaratteristiche, strCaratteristiche.Length - 2)
                                        End If

                                        listaMacCod.Add(XML_Ricetta_Dettaglio.GetAttribute("mat_cod") & "|" & strMacchina & "|" & strCaratteristiche)

                                End Select
                            End If

                        Case CAU_IMPUTAZIONE_TERZISTI, CAU_IMPUTAZIONE_MANODOPERA

                            If XML_Ricetta_Dettaglio.GetAttribute("rag_soc") <> "" Then
                                nomeContatto = XML_Ricetta_Dettaglio.GetAttribute("rag_soc")
                            Else
                                nomeContatto = String.Format("{0} {1}", XML_Ricetta_Dettaglio.GetAttribute("cognome"), XML_Ricetta_Dettaglio.GetAttribute("nome"))
                            End If

                            If InStr(strCostiOperatori, nomeContatto) = 0 Then
                                strCostiOperatori &= nomeContatto & ","
                            End If

                        Case CAU_IMPUTAZIONE_TECNICO_RESPONSABILE

                            If XML_Ricetta_Dettaglio.GetAttribute("rag_soc") <> "" Then
                                nomeContatto = XML_Ricetta_Dettaglio.GetAttribute("rag_soc")
                            Else
                                nomeContatto = String.Format("{0} {1}", XML_Ricetta_Dettaglio.GetAttribute("cognome"), XML_Ricetta_Dettaglio.GetAttribute("nome"))
                            End If

                            If InStr(strCostiResponsabile, nomeContatto) = 0 Then
                                strCostiResponsabile = nomeContatto & ","
                            End If

                        Case CAU_SCARICO

                            SetDictionaryView(DictMagazziniEsterni, XML_Ricetta_Dettaglio, frm_FrCod)

                    End Select

                Next

                If Not bNascondiAutorizzatore And strCostiResponsabile <> String.Empty Then

                    strCosti = Gias.AutorizzatoDa & " " & Left(strCostiResponsabile, strCostiResponsabile.Length - 1) & vbCrLf & vbCrLf

                End If

                If Not bNascondiOperatori And strCostiOperatori <> String.Empty Then

                    strCosti &= Gias.ACuraDi & " : " & Left(strCostiOperatori, strCostiOperatori.Length - 1)

                Else

                    If Qs_Stampa_Tipo <> enum_TipoStampaRicetta.Aziendale Then
                        strCosti &= Gias.Eseguito
                    End If

                End If

                If Not bNascondiMacchine And strCostiMacchine <> String.Empty Then

                    strCosti &= Gias.Con & " : " & Left(strCostiMacchine, strCostiMacchine.Length - 1)

                Else

                    If InStr(strCosti, Gias.Eseguito) <> 0 Then
                        Left(strCosti, strCosti.Length - 8)
                    End If

                End If

            End If

        End If
        '--------------------------------
        '   FINE SPACCHETTAMENTO
        '--------------------------------


        '------------------------------------------
        '------------------------------------------

        'Recupero il datatable
        Dt = ViewState("dtDosi_Difesa")

        'creo il dt dei formulati dal vettore miscele
        'Scombinatore_Difesa(Miscele, Dt)

        Dim strAvversita As String
        Dim ArrayAvCodTmp() As String
        Dim ArrayAvGruTmp() As String

        Dim drR As DS_Ricetta_Trattamenti.DS_Ricetta_TrattamentiRow

        Dim strCul_Des As String = ""

        Dim Mezzo As Integer
        Dim Extra_Int As Integer
        Dim Udm_Sim As String
        Dim Qta As Double
        Dim Qta_Tot As Double

        Dim N_app As String = ""
        Dim Num_Appezzamento As Integer
        Dim N_Appezza As Integer

        Dim Cop_Cod As Integer = 0

        Dim objApp As New AgronicaCoreAnagrafeDAL.Appezzamento_Read

        For i = 0 To UBound(frm_FrCod)


            If Qs_Stampa_Tipo = enum_TipoStampaRicetta.PianoLavori Then

                '----------------------------------------------------------------------------
                '------------------- TIPO STAMPA 3 --------------------------------
                '----------------------------------------------------------------------------
                Dim DrLav As DS_Ricetta3_Trattamenti.DT_Ricetta3_TrattamentiRow
                DrLav = DSTrattamenti3.DT_Ricetta3_Trattamenti.NewDT_Ricetta3_TrattamentiRow

                If i = UBound(frm_FrCod) Then
                    'valore fittizio per indicare ultima riga
                    'da usare per formula omissione linea su report
                    DrLav.PrimaRigaAppezza = 11
                Else
                    DrLav.PrimaRigaAppezza = 0
                End If

                DrLav.App_nome = ""
                DrLav.App_Nome_Breve = ""
                DrLav.Sa_Nome = ""
                DrLav.Campo_Nome = ""
                DrLav.Veg_Des = ""
                DrLav.Cul_Des = ""
                DrLav.Regolamento = ""
                DrLav.Sup_App = 0
                DrLav.Indirizzo = ""
                DrLav.Telefono = ""

                DrLav.Ricetta_Operazione_Cod = frm_OperazioneCod
                DrLav.Lav_Cod = Lav_Cod
                DrLav.Lav_Des = Lav_Des
                DrLav.Note = ""

                If i = 0 Then

                    'data operazione 
                    DrLav.Data = frm_Data

                    Dim ConImpianti As Boolean = False

                    'modifica rispetto alle altre stampe
                    DrLav.Destinazioni = ""
                    DrLav.Cul_Des = ""

                    'note
                    If frm_NoteIntervento <> "" Then
                        DrLav.Note = Gias.Nota & ": " & frm_NoteIntervento
                    End If

                    If frm_RicettaNote <> "" Then
                        If DrLav.Note <> "" Then
                            DrLav.Note &= vbCrLf & frm_RicettaNote
                        Else
                            DrLav.Note = frm_RicettaNote
                        End If
                    End If

                    If strCosti <> "" Then
                        If DrLav.Note <> "" Then
                            DrLav.Note &= vbCrLf & strCosti
                        Else
                            DrLav.Note = strCosti
                        End If
                    End If

                    Select Case frm_Acqua
                        Case 0
                            DrLav.Qta_Ril = ""
                            frm_Acqua_Tot = 0
                        Case Is < 0
                            DrLav.Qta_Ril = Math.Abs(frm_Acqua) & " [Hl/Ha]"
                            frm_Acqua_Tot = Math.Abs(frm_Acqua) * Sup_Tot_Op

                            DrLav.Qta_Acqua = CInt(CDbl(frm_Acqua_Tot) + 0.5)

                        Case Is > 0
                            DrLav.Qta_Ril = frm_Acqua & " [Hl]"
                            frm_Acqua_Tot = frm_Acqua

                            DrLav.Qta_Acqua = CInt(CDbl(frm_Acqua_Tot) + 0.5)
                    End Select

                Else

                    DrLav.Lav_Des = ""
                    DrLav.Note = ""
                    DrLav.Qta_Ril = ""

                End If

                AddMagazzinoEsternoNote(DrLav.Note, DictMagazziniEsterni, frm_FrCod(i))


                '--------------------------------
                '   AVVERSITA'
                '--------------------------------
                strAvversita = ""
                Select Case Lav_Cod

                    Case LAVCOD_TRATTAMENTO_FITOREGOLATORE, LAVCOD_DISSECCAMENTO
                        strAvversita = ""

                    Case Else

                        ArrayAvCodTmp = Split(frm_AvCod(i), ",")
                        ArrayAvGruTmp = Split(frm_AvGru(i), ",")
                        If Not ArrayAvCodTmp Is Nothing AndAlso ArrayAvCodTmp.Length > 0 Then
                            If ArrayAvCodTmp(0) = "0" Then
                                strAvversita = frm_AvGruDes(i)
                            Else
                                If ArrayAvCodTmp(0) = "-1" AndAlso ArrayAvGruTmp(0) = "-1" AndAlso Lav_Cod = LAVCOD_DISTRIBUZIONE_INSETTI Then
                                    strAvversita = Gias.PerImpollinazione.ToUpper()
                                Else
                                    strAvversita = frm_AvDes(i)
                                End If
                            End If
                        End If

                        If Not frm_SogliaDes Is Nothing AndAlso frm_SogliaDes(i) <> "" Then
                            strAvversita = strAvversita & " (" & Gias.Giustificazione & ":" & frm_SogliaDes(i) & "), "
                            strAvversita = strAvversita.Replace("<i>", "")
                            strAvversita = strAvversita.Replace("</i>", "")
                        End If

                End Select

                DrLav.Av_Des = strAvversita

                '--------------------------------
                '   PRODOTTO
                '--------------------------------
                DrLav.Elem_Cod = FORMULATI
                DrLav.Pro_Cod = frm_FrCod(i)
                DrLav.Fr_Des = frm_FrDes(i)


                '--------------------------------
                '   DOSE
                '--------------------------------
                Qta_Tot = 0

                Extra_Int = frm_UdmCod(i)
                Mezzo = frm_Mezzo
                Qta = frm_Dose(i)

                Select Case frm_ElemCod

                    Case TRAPPOLE

                        Qta = Qta / Sup_Tot_Op
                        Udm_Sim = " n"

                        Qta = Math.Round(Qta, 3)
                        DrLav.Qta = Qta.ToString & Udm_Sim
                        DrLav.Qta_Tot = Math.Round(Qta * Sup_Tot_Op, 3) & Udm_Sim

                    Case Else

                        'Converto in Kg o l
                        Select Case Extra_Int
                            Case enum_UnitaMisura.Numero
                                Udm_Sim = " n"
                            Case enum_UnitaMisura.KG
                                Udm_Sim = " kg"
                            Case enum_UnitaMisura.Grammi
                                Udm_Sim = " kg"
                                Qta = Qta / 1000
                            Case enum_UnitaMisura.Litri
                                Udm_Sim = " lt"
                            Case enum_UnitaMisura.Quintali
                                Udm_Sim = " kg"
                                Qta = Qta * 100
                            Case enum_UnitaMisura.Tonnellate
                                Udm_Sim = " kg"
                                Qta = Qta * 1000
                            Case enum_UnitaMisura.CentimetriCubi
                                Udm_Sim = " lt"
                                Qta = Qta / 100
                            Case enum_UnitaMisura.Millilitri
                                Udm_Sim = " lt"
                                Qta = Qta / 1000
                            Case enum_UnitaMisura.UNITA, enum_UnitaMisura.UNITA__HA,
                                 enum_UnitaMisura.Numero_Diffusori, enum_UnitaMisura.Numero_Diffusori_HA, enum_UnitaMisura.Numero
                                Udm_Sim = " n"
                            Case Else
                                'Rimuovo /ha perchè viene aggiunto successivamente
                                Udm_Sim = " " & UDM_Helper.GetUdmSim(Extra_Int, objParametri_Server).Replace("/ha", "")
                        End Select

                        Qta = Math.Round(Qta, 3)
                        'es xxx kg
                        DrLav.Qta = Qta.ToString & Udm_Sim


                        Dim DoseProdotto As String = ""
                        Dim TotaleProdotto As String = ""
                        Dim QuantitaAcqua As String = ""

                        'controllo se h2o salvata negativa o positiva
                        'frm_Acqua_Tot è già stata impostata come totale acqua in precedenza,
                        'se sono senza impianti è però 0 quindi ci metto quella per ha
                        Select Case frm_Acqua

                            Case 0

                                DrLav.Qta_Ril = ""
                                frm_Acqua_Tot = 0


                            Case Is < 0
                                'h2o salvata ad ettaro [Hl/Ha]

                                'qta totale
                                If Sup_Tot_Op <> 0 Then
                                    'con impianti

                                    'frm_Acqua_Tot non è 0, già calcolata sopra moltiplicando la superficie, uso quel valore e metto udm

                                    'Controllo se devo arrotondare l'acqua all'intero superiore
                                    If bArrotondaAcqua = True Then
                                        Dim AcquaNoRound As Double = 0
                                        If IsNumeric(frm_Acqua_Tot) Then
                                            AcquaNoRound = CDbl(frm_Acqua_Tot)
                                            'Se l'acqua non è intera la arrotondo
                                            If AcquaNoRound - Int(AcquaNoRound) > 0 Then
                                                frm_Acqua_Tot = (Int(AcquaNoRound) + 1).ToString()
                                            End If
                                        End If
                                    End If
                                    QuantitaAcqua = frm_Acqua_Tot & " hl"

                                Else
                                    'senza impianti 

                                    'Controllo se devo arrotondare l'acqua all'intero superiore
                                    If bArrotondaAcqua = True Then
                                        Dim AcquaNoRound As Double = 0
                                        If IsNumeric(frm_Acqua) Then
                                            AcquaNoRound = CDbl(frm_Acqua)
                                            'Se l'acqua non è intera la arrotondo
                                            If AcquaNoRound - Int(AcquaNoRound) > 0 Then
                                                frm_Acqua = (Int(AcquaNoRound) + 1).ToString()
                                            End If
                                        End If

                                        'Non posso calcolare il totale h20 essendo per ettaro e non avendo la superficie, 
                                        'quindi lascio la  [Hl/Ha] e metto la udm
                                        QuantitaAcqua = frm_Acqua & " hl/ha"

                                    Else
                                        'Non posso calcolare il totale h20 essendo per ettaro e non avendo la superficie, 
                                        'quindi lascio la  [Hl/Ha] e metto la udm
                                        QuantitaAcqua = Math.Round(Math.Abs(frm_Acqua), 3) & " hl/ha"
                                    End If

                                End If


                            Case Is > 0

                                'Controllo se devo arrotondare l'acqua all'intero superiore
                                If bArrotondaAcqua = True Then
                                    Dim AcquaNoRound As Double = 0
                                    If IsNumeric(frm_Acqua_Tot) Then
                                        AcquaNoRound = CDbl(frm_Acqua_Tot)
                                        'Se l'acqua non è intera la arrotondo
                                        If AcquaNoRound - Int(AcquaNoRound) > 0 Then
                                            frm_Acqua_Tot = (Int(AcquaNoRound) + 1).ToString()
                                        End If
                                    End If
                                End If

                                'h2o è già salvata in [Hl], quindi uso il valore già impostato in frm_Acqua_Tot e emtto udm hl
                                QuantitaAcqua = frm_Acqua_Tot & " hl"

                        End Select

                        'se mezzo =0 allora la dose DrLav.Qta è per ettolitro, altrimenti per ettaro
                        Select Case Mezzo

                            Case 0  'la qtà è x ettolitro

                                'DOSE, aggiungo hl alla stringa (es kg/hl)
                                DoseProdotto = DrLav.Qta & "/hl"

                                'TOTALEDOSE , devo moltiplicare per tot h2o, ma se non ho impianti potrebbe essere ad ettaro acqua e non totale
                                If Sup_Tot_Op <> 0 Then
                                    TotaleProdotto = Math.Round(Math.Abs(Qta * frm_Acqua), 3) & Udm_Sim
                                Else
                                    'h2o è per hl/ha perchè non ho superfici quindi la dose totale è per ha
                                    TotaleProdotto = Math.Round(Math.Abs(Qta * frm_Acqua), 3) & Udm_Sim & "/ha"
                                End If

                            Case 1  'la qtà è x ettaro

                                'DOSE aggiungo ha (es kg/ha)
                                DoseProdotto = DrLav.Qta & "/ha"

                                'DOSE totale prodotto
                                If Sup_Tot_Op <> 0 Then
                                    'Se sup >0 allora ho gli impianti,
                                    'posso calcolare la qta totale di prodotto
                                    TotaleProdotto = Math.Round(Qta * Sup_Tot_Op, 3) & Udm_Sim
                                Else
                                    'Se Sup è 0 allora sono senza impianti
                                    'non posso calcolare la qta totale, lascio la dose per ettaro e la udm
                                    TotaleProdotto = DoseProdotto
                                End If

                        End Select

                        DrLav.Qta = DoseProdotto
                        DrLav.Qta_Tot = TotaleProdotto
                        DrLav.Qta_Acqua = QuantitaAcqua

                End Select

                If Lav_Cod <> LAVCOD_DISTRIBUZIONE_INSETTI Then
                    Dim CoreAgroWS As New AgronicaCoreWebService.AgroWs
                    Dim tempoCarenza As Integer = CoreAgroWS.TempoCarenza_from_FrCod_VegCod(frm_FrCod(i), CInt(Veg_Cod), frm_ForVegCod(i), 0, Cop_Cod, CDate(frm_Data), objParametri_Server, objParametri_Utenti)

                    If tempoCarenza > 0 Then
                        DrLav.Carenza = tempoCarenza
                        DrLav.Data_Raccolta = CStr(DateAdd(DateInterval.Day, CInt(DrLav.Carenza) + 1, CDate(frm_Data)))
                    Else
                        DrLav.Carenza = "--"
                        DrLav.Data_Raccolta = CStr(CDate(frm_Data))
                    End If

                    DrLav.Tempo_Rientro = "48 " & Gias.Ore

                    Dim DtPrincipi As DataTable = CoreAgroWS.ComposizioneFormulatiRecupera(frm_FrCod(i), objParametri_Server, objParametri_Utenti)
                    Dim principiAttivi As String = Replace(CoreAgroWS.ComposizioneFormulatiDescrizionePrincipiAttivi(objParametri_Server, frm_FrCod(i), DtPrincipi), "     ", "").Trim

                    DrLav.Pa_Des = principiAttivi
                End If

                'aggiungo la nuova riga al dataset
                DSTrattamenti3.DT_Ricetta3_Trattamenti.Rows.Add(DrLav)

            Else
                '----------------------------------------------------------------------------
                '------------------- TIPO STAMPA 1 E 2 --------------------------------
                '----------------------------------------------------------------------------

                'creo una nuova riga del dataset
                drR = DsTrattamenti.DS_Ricetta_Trattamenti.NewDS_Ricetta_TrattamentiRow

                drR.Ricetta_Operazione_Cod = frm_OperazioneCod

                drR.Lav_Cod = Lav_Cod
                drR.Lav_Des = Lav_Des

                drR.Note = ""

                If i = 0 Then

                    'data operazione 
                    drR.Data = frm_Data

                    Dim ConImpianti As Boolean = False

                    If Not ArrayPiva Is Nothing Then

                        Dim ObjImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                        Dim Dt_Imp As DataTable
                        Dt_Imp = ObjImp.Leggi_Dati_Impianti(ArrayPiva,
                                                            ArraySa_Cod,
                                                            ArrayAppezza,
                                                            ArrayId_Reg, drR.Data, drR.Data,
                                                            "", "", objParametri_Server)
                        ObjImp = Nothing

                        For j = 0 To Dt_Imp.Rows.Count - 1

                            Cop_Cod = 0
                            If Not IsDBNull(Dt_Imp.Rows(j).Item("cop_cod")) Then
                                Cop_Cod = Dt_Imp.Rows(j).Item("cop_cod")
                            End If
                            'sup_tot
                            ConImpianti = True

                            'cul_des
                            If InStr(strCul_Des, Dt_Imp.Rows(j).Item("cul_des")) = 0 Then
                                strCul_Des &= Dt_Imp.Rows(j).Item("cul_des") & ", "
                            End If

                            'N.ro appezzamento
                            N_app = Dt_Imp.Rows(j).Item("app_nome_breve")

                            N_Appezza = Dt_Imp.Rows(j).Item("appezza") - BaseCode

                            N_app = objApp.Numero_Appezzamento(Dt_Imp.Rows(j).Item("piva"), Dt_Imp.Rows(j).Item("sa_cod"),
                                                       Dt_Imp.Rows(j).Item("campo_des"), Dt_Imp.Rows(j).Item("appezza"),
                                                       TipoCodiceAppezzamento,
                                                       Dt_Imp.Rows(j).Item("app_nome"), Dt_Imp.Rows(j).Item("app_nome_breve"),
                                                       N_Appezza, BaseCode,
                                                       Nothing, flag_nascondiCampo)

                            frm_Dest &= N_app & ", "

                        Next

                        If strCul_Des <> "" Then
                            strCul_Des = Left(strCul_Des, strCul_Des.Length - 2)
                        End If

                        If frm_Dest <> "" Then
                            frm_Dest = Left(frm_Dest, frm_Dest.Length - 2)
                        End If

                    End If

                    'destinazioni
                    drR.Destinazioni = frm_Dest

                    'cultivar
                    drR.Cul_Des = strCul_Des & " (" & Sup_Tot_Op.ToString & " Ha)"

                    'note
                    If frm_NoteIntervento <> "" Then
                        drR.Note = Gias.Nota & ": " & frm_NoteIntervento
                    End If

                    If frm_RicettaNote <> "" Then
                        If drR.Note <> "" Then
                            drR.Note &= vbCrLf & frm_RicettaNote
                        Else
                            drR.Note = frm_RicettaNote
                        End If
                    End If

                    If strCosti <> "" Then
                        If drR.Note <> "" Then
                            drR.Note &= vbCrLf & strCosti
                        Else
                            drR.Note = strCosti
                        End If
                    End If

                    Select Case frm_Acqua
                        Case 0
                            drR.Qta_Ril = ""
                            frm_Acqua_Tot = 0
                        Case Is < 0
                            drR.Qta_Ril = Math.Abs(frm_Acqua) & " [Hl/Ha]"
                            frm_Acqua_Tot = Math.Abs(frm_Acqua) * Sup_Tot_Op

                            drR.Qta_Acqua = CInt(CDbl(frm_Acqua_Tot) + 0.5)

                        Case Is > 0
                            drR.Qta_Ril = frm_Acqua & " [Hl]"
                            frm_Acqua_Tot = frm_Acqua

                            drR.Qta_Acqua = CInt(CDbl(frm_Acqua_Tot) + 0.5)
                    End Select

                Else

                    drR.Lav_Des = ""
                    drR.Note = ""
                    drR.Qta_Ril = ""

                End If


                AddMagazzinoEsternoNote(drR.Note, DictMagazziniEsterni, frm_FrCod(i))

                '--------------------------------
                '   AVVERSITA'
                '--------------------------------
                strAvversita = ""
                Select Case Lav_Cod

                    Case LAVCOD_TRATTAMENTO_FITOREGOLATORE, LAVCOD_DISSECCAMENTO
                        strAvversita = ""

                    Case Else

                        ArrayAvCodTmp = Split(frm_AvCod(i), ",")
                        ArrayAvGruTmp = Split(frm_AvGru(i), ",")
                        If Not ArrayAvCodTmp Is Nothing AndAlso ArrayAvCodTmp.Length > 0 Then
                            If ArrayAvCodTmp(0) = "0" Then
                                strAvversita = frm_AvGruDes(i)
                            Else
                                If ArrayAvCodTmp(0) = "-1" AndAlso ArrayAvGruTmp(0) = "-1" AndAlso Lav_Cod = LAVCOD_DISTRIBUZIONE_INSETTI Then
                                    strAvversita = Gias.PerImpollinazione.ToUpper()
                                Else
                                    strAvversita = frm_AvDes(i)
                                End If
                            End If
                        End If

                        If Not frm_SogliaDes Is Nothing AndAlso frm_SogliaDes(i) <> "" Then
                            strAvversita = strAvversita & " (" & Gias.Giustificazione & ":" & frm_SogliaDes(i) & "), "
                            strAvversita = strAvversita.Replace("<i>", "")
                            strAvversita = strAvversita.Replace("</i>", "")
                        End If

                End Select

                drR.Av_Des = strAvversita

                '--------------------------------
                '   PRODOTTO
                '--------------------------------
                drR.Elem_Cod = FORMULATI
                drR.Pro_Cod = frm_FrCod(i)
                drR.Fr_Des = frm_FrDes(i)


                '--------------------------------
                '   DOSE
                '--------------------------------
                Qta_Tot = 0

                Extra_Int = frm_UdmCod(i)
                Mezzo = frm_Mezzo
                Qta = frm_Dose(i)

                Select Case frm_ElemCod

                    Case TRAPPOLE

                        Qta = Qta / Sup_Tot_Op
                        Udm_Sim = " n"

                        Qta = Math.Round(Qta, 3)
                        drR.Qta = Qta.ToString & Udm_Sim
                        drR.Qta_Tot = Math.Round(Qta * Sup_Tot_Op, 3) & Udm_Sim

                    Case Else

                        'converto in Kg o l
                        Select Case Extra_Int
                            Case enum_UnitaMisura.KG
                                Udm_Sim = " kg"
                            Case enum_UnitaMisura.Grammi
                                Udm_Sim = " kg"
                                Qta = Qta / 1000
                            Case enum_UnitaMisura.Litri
                                Udm_Sim = " lt"
                            Case enum_UnitaMisura.Quintali
                                Udm_Sim = " kg"
                                Qta = Qta * 100
                            Case enum_UnitaMisura.Tonnellate
                                Udm_Sim = " kg"
                                Qta = Qta * 1000
                            Case enum_UnitaMisura.CentimetriCubi
                                Udm_Sim = " lt"
                                Qta = Qta / 100
                            Case enum_UnitaMisura.Millilitri
                                Udm_Sim = " lt"
                                Qta = Qta / 1000
                            Case enum_UnitaMisura.UNITA, enum_UnitaMisura.UNITA__HA,
                                 enum_UnitaMisura.Numero_Diffusori, enum_UnitaMisura.Numero_Diffusori_HA, enum_UnitaMisura.Numero
                                Udm_Sim = " n"
                            Case Else
                                'AF TODO chiedere se è giusto
                                'Rimuovo /ha perchè viene aggiunto successivamente
                                Udm_Sim = " " & UDM_Helper.GetUdmSim(Extra_Int, objParametri_Server).Replace("/ha", "")
                        End Select

                        Qta = Math.Round(Qta, 3)
                        'es xxx kg
                        drR.Qta = Qta.ToString & Udm_Sim


                        Dim DoseProdotto As String = ""
                        Dim TotaleProdotto As String = ""
                        Dim QuantitaAcqua As String = ""

                        'controllo se h2o salvata negativa o positiva
                        'frm_Acqua_Tot è già stata impostata come totale acqua in precedenza,
                        'se sono senza impianti è però 0 quindi ci metto quella per ha
                        Select Case frm_Acqua

                            Case 0

                                drR.Qta_Ril = ""
                                frm_Acqua_Tot = 0

                            Case Is < 0
                                'h2o salvata ad ettaro [Hl/Ha]

                                'qta totale
                                If Sup_Tot_Op <> 0 Then
                                    'con impianti

                                    'frm_Acqua_Tot non è 0, già calcolata sopra moltiplicando la superficie, uso quel valore e metto udm

                                    'Controllo se devo arrotondare l'acqua all'intero superiore
                                    If bArrotondaAcqua = True Then
                                        Dim AcquaNoRound As Double = 0
                                        If IsNumeric(frm_Acqua_Tot) Then
                                            AcquaNoRound = CDbl(frm_Acqua_Tot)
                                            'Se l'acqua non è intera la arrotondo
                                            If AcquaNoRound - Int(AcquaNoRound) > 0 Then
                                                frm_Acqua_Tot = (Int(AcquaNoRound) + 1).ToString()
                                            End If
                                        End If
                                    End If
                                    QuantitaAcqua = frm_Acqua_Tot & " hl"

                                Else
                                    'senza impianti 

                                    'Controllo se devo arrotondare l'acqua all'intero superiore
                                    If bArrotondaAcqua = True Then
                                        Dim AcquaNoRound As Double = 0
                                        If IsNumeric(frm_Acqua) Then
                                            AcquaNoRound = CDbl(frm_Acqua)
                                            'Se l'acqua non è intera la arrotondo
                                            If AcquaNoRound - Int(AcquaNoRound) > 0 Then
                                                frm_Acqua = (Int(AcquaNoRound) + 1).ToString()
                                            End If
                                        End If

                                        'Non posso calcolare il totale h20 essendo per ettaro e non avendo la superficie, 
                                        'quindi lascio la  [Hl/Ha] e metto la udm
                                        QuantitaAcqua = frm_Acqua & " hl/ha"

                                    Else
                                        'Non posso calcolare il totale h20 essendo per ettaro e non avendo la superficie, 
                                        'quindi lascio la  [Hl/Ha] e metto la udm
                                        QuantitaAcqua = Math.Round(Math.Abs(frm_Acqua), 3) & " hl/ha"
                                    End If

                                End If

                            Case Is > 0

                                'Controllo se devo arrotondare l'acqua all'intero superiore
                                If bArrotondaAcqua = True Then
                                    Dim AcquaNoRound As Double = 0
                                    If IsNumeric(frm_Acqua_Tot) Then
                                        AcquaNoRound = CDbl(frm_Acqua_Tot)
                                        'Se l'acqua non è intera la arrotondo
                                        If AcquaNoRound - Int(AcquaNoRound) > 0 Then
                                            frm_Acqua_Tot = (Int(AcquaNoRound) + 1).ToString()
                                        End If
                                    End If
                                End If

                                'h2o è già salvata in [Hl], quindi uso il valore già impostato in frm_Acqua_Tot e emtto udm hl
                                QuantitaAcqua = frm_Acqua_Tot & " hl"

                        End Select

                        'se mezzo =0 allora la dose drR.Qta è per ettolitro, altrimenti per ettaro
                        Select Case Mezzo

                            Case 0  'la qtà è x ettolitro

                                'DOSE, aggiungo hl alla stringa (es kg/hl)
                                DoseProdotto = drR.Qta & "/hl"

                                'TOTALEDOSE , devo moltiplicare per tot h2o, ma se non ho impianti potrebbe essere ad ettaro acqua e non totale
                                If Sup_Tot_Op <> 0 Then
                                    TotaleProdotto = Math.Round(Math.Abs(Qta * frm_Acqua), 3) & Udm_Sim
                                Else
                                    'h2o è per hl/ha perchè non ho superfici quindi la dose totale è per ha
                                    TotaleProdotto = Math.Round(Math.Abs(Qta * frm_Acqua), 3) & Udm_Sim & "/ha"
                                End If

                            Case 1  'la qtà è x ettaro

                                'DOSE aggiungo ha (es kg/ha)
                                DoseProdotto = drR.Qta & "/ha"

                                'DOSE totale prodotto
                                If Sup_Tot_Op <> 0 Then
                                    'Se sup >0 allora ho gli impianti,
                                    'posso calcolare la qta totale di prodotto
                                    TotaleProdotto = Math.Round(Qta * Sup_Tot_Op, 3) & Udm_Sim
                                Else
                                    'Se Sup è 0 allora sono senza impianti
                                    'non posso calcolare la qta totale, lascio la dose per ettaro e la udm
                                    TotaleProdotto = DoseProdotto
                                End If

                        End Select

                        drR.Qta = DoseProdotto
                        drR.Qta_Tot = TotaleProdotto
                        drR.Qta_Acqua = QuantitaAcqua

                End Select

                If Lav_Cod <> LAVCOD_DISTRIBUZIONE_INSETTI Then
                    Dim CoreAgroWS As New AgronicaCoreWebService.AgroWs
                    Dim tempoCarenza As Integer = CoreAgroWS.TempoCarenza_from_FrCod_VegCod(frm_FrCod(i), CInt(Veg_Cod), frm_ForVegCod(i), 0, Cop_Cod, CDate(frm_Data), objParametri_Server, objParametri_Utenti)

                    If tempoCarenza > 0 Then
                        drR.Carenza = tempoCarenza
                        drR.Data_Raccolta = CStr(DateAdd(DateInterval.Day, CInt(drR.Carenza) + 1, CDate(frm_Data)))

                    Else
                        drR.Carenza = "--"
                        drR.Data_Raccolta = CStr(CDate(frm_Data))
                    End If

                    drR.Tempo_Rientro = "48 ore"

                    Dim DtPrincipi As DataTable = CoreAgroWS.ComposizioneFormulatiRecupera(frm_FrCod(i), objParametri_Server, objParametri_Utenti)
                    Dim principiAttivi As String = Replace(CoreAgroWS.ComposizioneFormulatiDescrizionePrincipiAttivi(objParametri_Server, frm_FrCod(i), DtPrincipi), "     ", "").Trim

                    drR.Pa_Des = principiAttivi
                End If

                'aggiungo la nuova riga al dataset
                DsTrattamenti.DS_Ricetta_Trattamenti.Rows.Add(drR)

            End If 'tipo stampa

        Next 'fr_cod


        'AGGIUNGO LE RIGHE MACCHINE nella stampa aziendale
        For m = 0 To listaMacCod.Count - 1

            'creo una nuova riga del dataset
            drR = DsTrattamenti.DS_Ricetta_Trattamenti.NewDS_Ricetta_TrattamentiRow

            drR.Elem_Cod = MACCHINE
            drR.Ricetta_Operazione_Cod = frm_OperazioneCod
            drR.Mac_Cod = Split(listaMacCod(m), "|")(0)
            drR.Mac_Des = Split(listaMacCod(m), "|")(1)
            drR.Mac_Car_Des = Split(listaMacCod(m), "|")(2)

            DsTrattamenti.DS_Ricetta_Trattamenti.Rows.Add(drR)

        Next

        'ripulisco il Dt di appoggio
        Dt.Rows.Clear()

        ViewState("dtDosi_Difesa") = Dt

        '--------------------------------

    End Sub



    ''#########################################################################################
    'Private Sub Scombinatore_Difesa(ByVal Data As Date, _
    '                                ByVal Vettore() As String, _
    '                                ByRef DT As DataTable)

    '    Dim Dr As DataRow

    '    Dim Pezzi() As String
    '    Dim MaxIndice As Integer

    '    Dim Dati() As String

    '    Dim Mezzo As String
    '    Dim Av_Cod As String
    '    Dim Av_Gru As String
    '    Dim Av_Des As String
    '    Dim Av_Gru_Des As String
    '    Dim Fr_Cod() As String
    '    Dim Fr_Des() As String
    '    Dim Udm_Cod() As String
    '    Dim Udm_Sim() As String
    '    Dim Dose() As String

    '    '-----------------------------------------------------------------------------------
    '    '--- Scopro quante righe deve avere la matrice ... ogni riga avra' un contatore
    '    '-----------------------------------------------------------------------------------

    '    Pezzi = Split(Vettore(0), "$")
    '    MaxIndice = UBound(Pezzi)

    '    Dim Matrice(MaxIndice, 0) As String
    '    Dim Contatori(MaxIndice) As Integer

    '    '-----------------------------------------------------------------------------------
    '    '--- Scansione delle righe del vettore
    '    '-----------------------------------------------------------------------------------

    '    Dim i As Integer
    '    Dim j As Integer
    '    Dim h As Integer
    '    Dim Trovato As Boolean

    '    'Azzero i contatori
    '    For i = 0 To MaxIndice
    '        Contatori(i) = 0
    '    Next


    '    'Per ogni riga del vettore
    '    For i = LBound(Vettore) To UBound(Vettore)

    '        Pezzi = Split(Vettore(i), "$")

    '        'Per ogni codice presente nella cella del vettore
    '        For j = 0 To UBound(Pezzi)

    '            Trovato = False

    '            'Cerco l'elemento nella riga j-esima della matrice
    '            For h = 0 To Contatori(j)
    '                If Matrice(j, h) = Pezzi(j) Then
    '                    Trovato = True
    '                    Exit For
    '                End If
    '            Next

    '            If Trovato = False Then

    '                Matrice(j, Contatori(j)) = Pezzi(j)
    '                Contatori(j) += 1

    '                If UBound(Matrice, 2) < Contatori(j) Then
    '                    ReDim Preserve Matrice(MaxIndice, Contatori(j))
    '                End If

    '            End If

    '        Next

    '    Next

    '    ReDim Preserve Matrice(MaxIndice, UBound(Matrice, 2) - 1)

    '    For i = 0 To UBound(Matrice, 1)
    '        For j = 0 To UBound(Matrice, 2)
    '            If Matrice(i, j) Is Nothing Then
    '                Matrice(i, j) = -1
    '            End If
    '        Next
    '    Next


    '    For i = 0 To UBound(Matrice)

    '        For j = 0 To UBound(Matrice, 2)

    '            Dati = Split(Matrice(i, j), "£")

    '            If Dati(0) <> "-1" Then

    '                'Fr_Cod/Fr_Des/Dose/Udm_Cod/Udm_Cod_Trasformato/Udm_Sim/Av_Cod/Av_Gru/Av_Des/Av_Gru_Des/Mezzo

    '                ReDim Preserve Fr_Cod(j)
    '                Fr_Cod(j) = Dati(0)

    '                ReDim Preserve Fr_Des(j)
    '                Fr_Des(j) = Dati(1)

    '                ReDim Preserve Dose(j)
    '                Dose(j) = Dati(2)

    '                ReDim Preserve Udm_Cod(j)
    '                Udm_Cod(j) = Dati(3)

    '                ReDim Preserve Udm_Sim(j)
    '                Udm_Sim(j) = Dati(5)

    '                Av_Cod = Dati(6)
    '                Av_Gru = Dati(7)
    '                Av_Des = Dati(8)
    '                Av_Gru_Des = Dati(9)

    '                Mezzo = Dati(10)


    '            Else

    '                ReDim Preserve Fr_Cod(j)
    '                Fr_Cod(j) = Dati(0)

    '                ReDim Preserve Fr_Des(j)
    '                Fr_Des(j) = ""

    '                ReDim Preserve Dose(j)
    '                Dose(j) = 0

    '                ReDim Preserve Udm_Cod(j)
    '                Udm_Cod(j) = -1

    '                ReDim Preserve Udm_Sim(j)
    '                Udm_Sim(j) = ""


    '            End If

    '        Next

    '        Dosi_Inserisci_Difesa(Av_Cod, _
    '                              Av_Gru, _
    '                              Av_Des, _
    '                              Av_Gru_Des, _
    '                              Fr_Cod, _
    '                              Fr_Des, _
    '                              Data, _
    '                              Dose, _
    '                              Udm_Cod, _
    '                              Udm_Sim, _
    '                              Mezzo)


    '    Next


    'End Sub

    ''########################################################################################
    'Private Sub Dosi_Inserisci_Difesa(ByVal Av_Cod As String, _
    '                                  ByVal Av_Gru As String, _
    '                                  ByVal Av_Des As String, _
    '                                  ByVal Av_Gru_Des As String, _
    '                                  ByVal Fr_Cod As Array, _
    '                                  ByVal Fr_Des As Array, _
    '                                  ByVal Data As Date, _
    '                                  Optional ByVal Dose As Array = Nothing, _
    '                                  Optional ByVal UdmCod As Array = Nothing, _
    '                                  Optional ByVal UdmSim As Array = Nothing, _
    '                                  Optional ByVal Mezzo As Integer = 1)


    '    Dim CoreAgroWS As New AgronicaCoreWebService.AgroWs
    '    Dim Formulati_r As New AgronicaCoreMetaSchemaDAL.Formulati_R

    '    Dim i, j As Integer
    '    Dim N_For As Integer = 0

    '    Dim DoseMin, DoseMax As Double
    '    Dim DoseMinTemp, DoseMaxTemp As Double
    '    Dim Udm_Cod As Integer
    '    Dim Udm_Sim As String
    '    Dim Udm_CodTemp As Integer
    '    Dim DoseEtichetta As String
    '    Dim DoseEtichettaTemp As String = ""

    '    Dim Num_Avv As Integer

    '    'Dim Array_Av_Cod() As String
    '    'Dim Array_Av_Gru() As String

    '    Dim strAvversita As String
    '    Dim Av_Des_Latino As String
    '    Dim Avversita As String

    '    '----- Dimensiono le variabili
    '    Dim Dt As DataTable
    '    Dim Dr As DataRow

    '    Dim Riga As Integer

    '    Dim Messaggio As String
    '    Dim strErr As String = ""

    '    Dim Carenza As Integer

    '    'Recupero il datatable
    '    Dt = ViewState("dtDosi_Difesa")

    '    'Creo una nuova riga
    '    Dr = Dt.NewRow

    '    'Definisco i valori

    '    '----- Inserisco il nuovo record

    '    N_For = UBound(Fr_Cod) + 1

    '    'Creo una nuova riga
    '    Dr = Dt.NewRow

    '    Dr.Item("Av_Cod") = Av_Cod
    '    Dr.Item("Av_Gru") = Av_Gru

    '    If Av_Cod <> "0" Then
    '        Dr.Item("Av_Des") = Av_Des
    '    Else
    '        If Av_Gru <> "0" Then
    '            Dr.Item("Av_Des") = Av_Gru_Des
    '        Else
    '            Dr.Item("Av_Des") = ""
    '        End If
    '    End If

    '    For i = 1 To N_For

    '        Dr.Item("Fr_Cod" & i) = Fr_Cod(i - 1)

    '        If Fr_Des(i - 1) <> "" Then
    '            Dr.Item("Fr_Des" & i) = Fr_Des(i - 1)
    '        Else
    '            Dr.Item("Fr_Des" & i) = Formulati_r.FrDes_from_FrCod(CInt(Fr_Cod(i - 1)), objParametri_Server)
    '        End If

    '        Carenza = CoreAgroWS.TempoCarenza_from_FrCod_VegCod(Fr_Cod(i - 1), CInt(Veg_Cod), CDate(Data), objParametri_Server, objParametri_Utenti)
    '        If Carenza <> -1 Then
    '            Dr.Item("Carenza" & i) = Carenza
    '        Else
    '            Dr.Item("Carenza" & i) = 0
    '        End If

    '        Dr.Item("Dose_Etichetta" & i) = "Non Disponibile"
    '        Dr.Item("Dose_Etichetta_Max" & i) = 0

    '        If Not Dose Is Nothing Then
    '            Dr.Item("Udm_Cod" & i) = UdmCod(i - 1)
    '            Dr.Item("Udm_sim" & i) = UdmSim(i - 1)
    '            Dr.Item("Dose" & i) = Dose(i - 1)
    '            Dr.Item("Dose_Fittizia" & i) = Dose(i - 1)
    '            Dr.Item("Mezzo" & i) = Mezzo
    '            'Dr.Item("Qta_Tot" & i) = -1
    '        Else
    '            Dr.Item("Udm_Cod" & i) = -1
    '            Dr.Item("Udm_sim" & i) = ""
    '            Dr.Item("Dose" & i) = -1
    '            Dr.Item("Dose_Fittizia" & i) = -1
    '            Dr.Item("Mezzo" & i) = 1
    '            Dr.Item("Qta_Tot" & i) = -1
    '        End If

    '    Next


    '    If Dr.Item("Av_Des") = "" And strAvversita <> "" Then
    '        Dr.Item("Av_Des") = Left(strAvversita, strAvversita.Length - 2)
    '    End If


    '    For i = N_For + 1 To 4

    '        Dr.Item("Fr_Cod" & i) = -1
    '        Dr.Item("Fr_Des" & i) = "---"

    '        Dr.Item("Carenza" & i) = 0
    '        Dr.Item("Dose_Etichetta" & i) = ""
    '        Dr.Item("Dose_Etichetta_Max" & i) = 0

    '        Dr.Item("Udm_Cod" & i) = -1
    '        Dr.Item("Udm_Sim" & i) = ""
    '        Dr.Item("Dose" & i) = -1
    '        Dr.Item("Dose_Fittizia" & i) = -1
    '        Dr.Item("Mezzo" & i) = 1
    '        Dr.Item("Qta_Tot" & i) = -1

    '    Next

    '    'controllo mezzo
    '    '(le dosi devono essere impostate tutte o/ha o /hl)
    '    If Dr.Item("Mezzo1") = 1 Then
    '        Dr.Item("Mezzo2") = 1
    '        Dr.Item("Mezzo3") = 1
    '    Else
    '        Dr.Item("Mezzo2") = 0
    '        Dr.Item("Mezzo3") = 0
    '    End If

    '    'Associo alla tabella la nuova riga creata
    '    Dt.Rows.Add(Dr)

    '    '----- Salvo il DataTable dentro il viewstate

    '    ViewState("dtDosi_Difesa") = Dt


    'End Sub




    '###########################################################################
    Private Sub Crea_Dt_Fertilizzazioni()

        '----- Definizione delle variabili

        Dim Dt As New DataTable

        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("Fer_Cod1", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Fer_Des1", GetType(String)))
        Dt.Columns.Add(New DataColumn("Udm_Cod1", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Udm_Sim1", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dose1", GetType(Double)))
        Dt.Columns.Add(New DataColumn("Dose_Fittizia1", GetType(Double)))
        Dt.Columns.Add(New DataColumn("Mezzo1", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Qta_Tot1", GetType(Double)))

        Dt.Columns.Add(New DataColumn("Fer_Cod2", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Fer_Des2", GetType(String)))
        Dt.Columns.Add(New DataColumn("Udm_Cod2", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Udm_Sim2", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dose2", GetType(Double)))
        Dt.Columns.Add(New DataColumn("Dose_Fittizia2", GetType(Double)))
        Dt.Columns.Add(New DataColumn("Mezzo2", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Qta_Tot2", GetType(Double)))

        Dt.Columns.Add(New DataColumn("Fer_Cod3", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Fer_Des3", GetType(String)))
        Dt.Columns.Add(New DataColumn("Udm_Cod3", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Udm_Sim3", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dose3", GetType(Double)))
        Dt.Columns.Add(New DataColumn("Dose_Fittizia3", GetType(Double)))
        Dt.Columns.Add(New DataColumn("Mezzo3", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Qta_Tot3", GetType(Double)))


        Dt.Columns.Add(New DataColumn("Fer_Cod4", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Fer_Des4", GetType(String)))
        Dt.Columns.Add(New DataColumn("Udm_Cod4", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Udm_Sim4", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dose4", GetType(Double)))
        Dt.Columns.Add(New DataColumn("Dose_Fittizia4", GetType(Double)))
        Dt.Columns.Add(New DataColumn("Mezzo4", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Qta_Tot4", GetType(Double)))

        Dt.Columns.Add(New DataColumn("Data", GetType(String)))

        Dt.Columns.Add(New DataColumn("Destinazioni", GetType(String)))
        '----- Definisco l'insieme di colonne che costituiscono la chiave della tabella

        'Vettore di DataColumn
        Dim DtKeys(2) As DataColumn

        'Valorizzo le celle del vettore
        DtKeys(0) = Dt.Columns("Fer_Cod1")
        DtKeys(1) = Dt.Columns("Fer_Cod2")
        DtKeys(2) = Dt.Columns("Fer_Cod3")

        'Assegno il vettore delle chiavi al DataTable
        Dt.PrimaryKey = DtKeys

        '----- Salvo il DataTable dentro il viewstate

        ViewState("dtDosi_Concimazione") = Dt

    End Sub


    Structure ElementoConcimazione
        Dim prod_cod As Integer
        Dim N As String
        Dim P As String
        Dim K As String
        Dim Mg As String
        Dim Effic As String
    End Structure


    '###########################################################################
    Private Sub Inserisci_Fertilizzazione(ByVal strOperazione As String,
                                          ByVal bNascondiMacchine As Boolean,
                                          ByVal bNascondiOperatori As Boolean, ByVal bNascondiAutorizzatore As Boolean,
                                          ByVal bNascondiDataUltimaManutenzione As Boolean,
                                            TipoCodiceAppezzamento As Integer, flag_nascondiCampo As Boolean, ByVal BaseCode As Integer)

        Dim Fertilizzanti_R As New AgronicaCoreMetaSchemaDAL.Fertilizzanti_R

        Dim Dt As DataTable



        Dim XmlDoc As New System.Xml.XmlDocument

        Dim XML_Ricetta_Operazione As System.Xml.XmlElement

        Dim XML_DatiRicetta_Dettagli_Tecnici As System.Xml.XmlElement
        Dim XML_Ricetta_Dettaglio_Tecnico As System.Xml.XmlElement
        Dim XMLs_Ricetta_Dettaglio_Tecnico As System.Xml.XmlNodeList

        Dim XML_DatiRicetta_Dettagli As System.Xml.XmlElement
        Dim XML_Ricetta_Dettaglio As System.Xml.XmlElement
        Dim XMLs_Ricetta_Dettaglio As System.Xml.XmlNodeList

        Dim Lav_Cod As String
        Dim Lav_Des As String

        Dim frm_OperazioneCod As Integer
        Dim frm_OperazioneDes As String
        Dim frm_NoteIntervento As String
        Dim frm_CodDisciplinare As Integer
        Dim frm_Modulo As Integer
        Dim frm_IdRcdpi As Integer
        Dim frm_Mezzo As Integer

        Dim frm_Data As String

        Dim frm_Acqua As Double
        Dim frm_Acqua_Tot As Double

        Dim frm_MiscelaCod() As Integer
        Dim frm_FrCod() As Integer
        Dim frm_FrDes() As String
        Dim frm_UdmCod() As Integer
        Dim frm_UdmCodTrasformato() As Integer
        Dim frm_UdmSim() As String
        Dim frm_Dose() As Double


        Dim i, j, a As Integer

        Dim UdmDes As String
        Dim UdmSim As String
        Dim N_distribuito, P_distribuito, K_distribuito As Double
        Dim N_residuo, P_residuo, K_residuo As Double

        Dim MiscelaCod As Integer
        Dim Miscela_Cod As Integer
        Dim DatiMiscela() As String
        Dim TrovataMiscela As Boolean = False
        Dim Miscele(0) As String
        Dim Miscela As String
        Dim N_Miscele As Integer = 0

        Dim frm_Cau_Mov As String

        Dim ArrayElementoConcimazione() As ElementoConcimazione
        Dim XML_Ricetta_Dettaglio_Tecnico_2 As System.Xml.XmlElement

        Dim frm_Dest As String
        Dim XML_Ricetta_Dettaglio_Destinazione As System.Xml.XmlElement
        Dim XMLs_Ricetta_Dettaglio_Destinazione As System.Xml.XmlNodeList

        '''''' note '''''
        Dim frm_RicettaNote As String = ""
        Dim XML_Ricetta_Note As System.Xml.XmlElement
        Dim XMLs_Ricetta_Note As System.Xml.XmlNodeList
        Dim XML_DatiRicetta_Note2 As System.Xml.XmlElement

        Dim TrovatoImpianto As Boolean
        Dim ArrayPiva() As String
        Dim ArraySa_Cod() As Integer
        Dim ArrayAppezza() As Integer
        Dim ArrayId_Reg() As Integer

        Dim strCosti As String
        Dim strMacchina As String
        Dim strCostiMacchine As String
        Dim strCostiOperatori As String
        Dim strCostiResponsabile As String

        Dim DictMagazziniEsterni As New Dictionary(Of (Integer, String, String), String)

        Dim nomeContatto As String

        Dim Sup_Tot_Op As Decimal = 0

        '------------------------------------------
        '----- Analizzo la stringa XML
        '------------------------------------------

        'Carico la stringa nel documento XML
        XmlDoc.LoadXml(strOperazione)

        '----- Tag Ricetta_Operazione

        XML_Ricetta_Operazione = XmlDoc.SelectSingleNode("Ricetta_Operazione")

        frm_OperazioneCod = CInt(XML_Ricetta_Operazione.GetAttribute("ricetta_operazione_cod"))
        frm_OperazioneDes = CStr(XML_Ricetta_Operazione.GetAttribute("ricetta_operazione_des"))

        Dim objRDest As New AgronicaCoreContabDAL.Ricette_Destinazioni_R
        Sup_Tot_Op = objRDest.SupTotTrattata_from_RicettaOperazioneCod(frm_OperazioneCod, "", objParametri_Server)

        Lav_Cod = CInt(XML_Ricetta_Operazione.GetAttribute("lav_cod"))
        Lav_Des = CStr(XML_Ricetta_Operazione.GetAttribute("lav_des"))

        frm_NoteIntervento = CStr(XML_Ricetta_Operazione.GetAttribute("note"))

        frm_Mezzo = CInt(XML_Ricetta_Operazione.GetAttribute("mezzo"))

        frm_Data = CStr(XML_Ricetta_Operazione.GetAttribute("validita_inizio"))

        If XML_Ricetta_Operazione.HasChildNodes Then

            '----- Tag DatiRicetta_Dettagli_Tecnici

            XML_DatiRicetta_Dettagli_Tecnici = XML_Ricetta_Operazione.SelectSingleNode("DatiRicetta_Dettagli_Tecnici")

            If Not XML_DatiRicetta_Dettagli_Tecnici Is Nothing Then

                '----- Tag Ricetta_Dettaglio_Tecnico  

                'Recupero la collezione dei nodi
                XMLs_Ricetta_Dettaglio_Tecnico = XML_DatiRicetta_Dettagli_Tecnici.GetElementsByTagName("Ricetta_Dettaglio_Tecnico")

                Dim xQtaAcqua As Double

                'Ciclo su tutti i nodi
                For i = 0 To XMLs_Ricetta_Dettaglio_Tecnico.Count - 1

                    'Prendo l'i-esimo nodo della collezione
                    XML_Ricetta_Dettaglio_Tecnico = XMLs_Ricetta_Dettaglio_Tecnico.Item(i)

                    'Acqua
                    xQtaAcqua = CDbl(XML_Ricetta_Dettaglio_Tecnico.GetAttribute("qta_ril"))

                    frm_Acqua = xQtaAcqua

                Next

            End If


            '''''''''''''''''''''''''NOTE'''''''''''''''''''
            '----- Tag DatiRicetta_Note2
            Dim objNoteIntervento As New AgronicaCoreContabDAL.Note_Intervento_R
            XML_DatiRicetta_Note2 = XML_Ricetta_Operazione.SelectSingleNode("DatiRicettaxNote_2")
            If Not XML_DatiRicetta_Note2 Is Nothing Then

                '----- Tag RicettaxNote_2
                XMLs_Ricetta_Note = XML_DatiRicetta_Note2.GetElementsByTagName("RicettaxNote_2")

                If Not XMLs_Ricetta_Note Is Nothing AndAlso XMLs_Ricetta_Note.Count > 0 Then

                    For j = 0 To XMLs_Ricetta_Note.Count - 1

                        XML_DatiRicetta_Note2 = XMLs_Ricetta_Note.Item(j)
                        frm_RicettaNote &= objNoteIntervento.NotaDesFromNotaCod(
                        XML_DatiRicetta_Note2.GetAttribute("nota_cod"),
                        Session("ASG_objParametri_Server")) & ", "

                    Next

                    If frm_RicettaNote <> "" Then
                        frm_RicettaNote = Left(frm_RicettaNote, frm_RicettaNote.Length - 2)
                    End If
                End If

            End If
            '''''''''''''''''''''''''NOTE'''''''''''''''''''


            '----- Tag XML_DatiRicetta_Dettagli

            XML_DatiRicetta_Dettagli = XML_Ricetta_Operazione.SelectSingleNode("DatiRicetta_Dettagli")

            If Not XML_DatiRicetta_Dettagli Is Nothing Then

                '----- Tag Ricetta_Dettaglio  (multiplo)

                'Recupero la collezione dei nodi
                XMLs_Ricetta_Dettaglio = XML_DatiRicetta_Dettagli.GetElementsByTagName("Ricetta_Dettaglio")

                For i = 0 To XMLs_Ricetta_Dettaglio.Count - 1

                    'Prendo l'i-esimo nodo della collezione
                    XML_Ricetta_Dettaglio = XMLs_Ricetta_Dettaglio.Item(i)

                    frm_Cau_Mov = XML_Ricetta_Dettaglio.GetAttribute("cau_mov")

                    Select Case frm_Cau_Mov

                        Case "", AgronicaCoreDataProvider.CostantiPersonalizzate.CAU_LAVORAZIONE

                            'Inizializzo le variabili
                            If frm_FrCod Is Nothing Then

                                ReDim Preserve frm_FrCod(0)
                                ReDim Preserve frm_FrDes(0)
                                ReDim Preserve frm_MiscelaCod(0)
                                ReDim Preserve frm_UdmCod(0)
                                ReDim Preserve frm_UdmCodTrasformato(0)
                                ReDim Preserve frm_UdmSim(0)
                                ReDim Preserve frm_Dose(0)

                            Else

                                ReDim Preserve frm_FrCod(UBound(frm_FrCod) + 1)
                                ReDim Preserve frm_FrDes(UBound(frm_FrDes) + 1)
                                ReDim Preserve frm_MiscelaCod(UBound(frm_MiscelaCod) + 1)
                                ReDim Preserve frm_UdmCodTrasformato(UBound(frm_UdmCodTrasformato) + 1)
                                ReDim Preserve frm_UdmCod(UBound(frm_UdmCod) + 1)
                                ReDim Preserve frm_UdmSim(UBound(frm_UdmSim) + 1)
                                ReDim Preserve frm_Dose(UBound(frm_Dose) + 1)

                            End If

                            'Recupero i valori
                            frm_FrCod(UBound(frm_FrCod)) = CInt(XML_Ricetta_Dettaglio.GetAttribute("pro_cod"))
                            frm_FrDes(UBound(frm_FrDes)) = CStr(XML_Ricetta_Dettaglio.GetAttribute("fer_des"))
                            frm_MiscelaCod(UBound(frm_MiscelaCod)) = CInt(XML_Ricetta_Dettaglio.GetAttribute("miscela_cod"))
                            frm_UdmCod(UBound(frm_UdmCod)) = CInt(XML_Ricetta_Dettaglio.GetAttribute("extra_int"))
                            frm_UdmCodTrasformato(UBound(frm_UdmCodTrasformato)) = CInt(XML_Ricetta_Dettaglio.GetAttribute("udm_cod"))
                            frm_UdmSim(UBound(frm_UdmSim)) = CStr(XML_Ricetta_Dettaglio.GetAttribute("udm_sim"))
                            frm_Dose(UBound(frm_Dose)) = CDbl(XML_Ricetta_Dettaglio.GetAttribute("qta"))


                            ' Ricetta_Dettaglio_Tecnico_2 ''''''''''''''''''''''''''''

                            XML_Ricetta_Dettaglio_Tecnico_2 = XML_Ricetta_Dettaglio.SelectSingleNode("Ricetta_Dettaglio_Tecnico_2")
                            If Not XML_Ricetta_Dettaglio_Tecnico_2 Is Nothing Then
                                Dim TrovatoFertilizzante = False
                                If Not ArrayElementoConcimazione Is Nothing Then
                                    For a = 0 To ArrayElementoConcimazione.Length - 1
                                        If ArrayElementoConcimazione(a).prod_cod = XML_Ricetta_Dettaglio.GetAttribute("pro_cod") Then
                                            TrovatoFertilizzante = True
                                            Exit For
                                        End If
                                    Next
                                End If

                                If TrovatoFertilizzante = False Then
                                    If ArrayElementoConcimazione Is Nothing Then
                                        ReDim Preserve ArrayElementoConcimazione(0)
                                    Else
                                        ReDim Preserve ArrayElementoConcimazione(UBound(ArrayElementoConcimazione) + 1)
                                    End If
                                    ArrayElementoConcimazione(UBound(ArrayElementoConcimazione)).prod_cod = XML_Ricetta_Dettaglio.GetAttribute("pro_cod")
                                    ArrayElementoConcimazione(UBound(ArrayElementoConcimazione)).N = XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("n")
                                    ArrayElementoConcimazione(UBound(ArrayElementoConcimazione)).P = XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("p")
                                    ArrayElementoConcimazione(UBound(ArrayElementoConcimazione)).K = XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("k")
                                    ArrayElementoConcimazione(UBound(ArrayElementoConcimazione)).Mg = XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("mg")
                                    ArrayElementoConcimazione(UBound(ArrayElementoConcimazione)).Effic = XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("efficienza")
                                End If


                            End If
                            ' Ricetta_Dettaglio_Tecnico_2 ''''''''''''''''''''''''''

                            'Destinazioni ''''''''''''''''''''''''''''
                            Dim strDest As String
                            Dim objRegImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                            frm_Dest = ""
                            XMLs_Ricetta_Dettaglio_Destinazione = XML_Ricetta_Dettaglio.GetElementsByTagName("Ricetta_Destinazione")
                            If Not XMLs_Ricetta_Dettaglio_Destinazione Is Nothing AndAlso XMLs_Ricetta_Dettaglio_Destinazione.Count > 0 Then

                                For j = 0 To XMLs_Ricetta_Dettaglio_Destinazione.Count - 1

                                    XML_Ricetta_Dettaglio_Destinazione = XMLs_Ricetta_Dettaglio_Destinazione.Item(j)

                                    TrovatoImpianto = False

                                    If Not ArrayPiva Is Nothing Then
                                        For a = 0 To ArrayPiva.Length - 1
                                            If ArrayPiva(a) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("piva") And
                                               ArraySa_Cod(a) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("sa_cod") And
                                               ArrayAppezza(a) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("appezza") And
                                               ArrayId_Reg(a) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("id_reg") Then
                                                TrovatoImpianto = True
                                                Exit For
                                            End If
                                        Next
                                    End If

                                    If TrovatoImpianto = False Then
                                        If ArrayPiva Is Nothing Then
                                            ReDim Preserve ArrayPiva(0)
                                            ReDim Preserve ArraySa_Cod(0)
                                            ReDim Preserve ArrayAppezza(0)
                                            ReDim Preserve ArrayId_Reg(0)
                                        Else
                                            ReDim Preserve ArrayPiva(UBound(ArrayPiva) + 1)
                                            ReDim Preserve ArraySa_Cod(UBound(ArraySa_Cod) + 1)
                                            ReDim Preserve ArrayAppezza(UBound(ArrayAppezza) + 1)
                                            ReDim Preserve ArrayId_Reg(UBound(ArrayId_Reg) + 1)
                                        End If
                                        ArrayPiva(UBound(ArrayPiva)) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("piva")
                                        ArraySa_Cod(UBound(ArraySa_Cod)) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("sa_cod")
                                        ArrayAppezza(UBound(ArrayAppezza)) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("appezza")
                                        ArrayId_Reg(UBound(ArrayId_Reg)) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("id_reg")
                                    End If

                                Next

                                If frm_Dest <> "" Then
                                    frm_Dest = Left(frm_Dest, frm_Dest.Length - 2)
                                End If

                            End If
                            'Destinazioni ''''''''''''''''''''''''''''

                        Case AgronicaCoreDataProvider.CostantiPersonalizzate.CAU_IMPUTAZIONE_PARCOMACCHINE

                            Dim objMAc As New AgronicaCoreContabDAL.Parco_Macchine_R
                            Dim dt_Mac As DataTable
                            strMacchina = ""

                            dt_Mac = objMAc.ParcoMacchine_Leggi("",
                                                                CInt(XML_Ricetta_Dettaglio.GetAttribute("mat_cod")),
                                                                False,
                                                                "", "", "", "", "", 0,
                                                                "", False, 0, "", True,
                                                                AGRODATAINIZIO,
                                                                AGRODATAFINE,
                                                                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                "", "",
                                                                objParametri_Server)
                            objMAc = Nothing

                            If dt_Mac.Rows.Count > 0 Then

                                If dt_Mac.Rows(0).Item("Modello") <> "" And dt_Mac.Rows(0).Item("Ultima_Manutenzione") <> "01/01/1900" Then
                                    strMacchina += dt_Mac.Rows(0).Item("CLASS_DESC") + " (Modello " + dt_Mac.Rows(0).Item("Modello")
                                    If Not bNascondiDataUltimaManutenzione Then
                                        strMacchina += "; Ultima Manutenzione " + dt_Mac.Rows(0).Item("Ultima_Manutenzione") + "),"
                                    Else
                                        strMacchina += "),"
                                    End If
                                ElseIf dt_Mac.Rows(0).Item("Modello") <> "" And dt_Mac.Rows(0).Item("Ultima_Manutenzione") = "01/01/1900" Then
                                    strMacchina += dt_Mac.Rows(0).Item("CLASS_DESC") + " (Modello " + dt_Mac.Rows(0).Item("Modello") + "),"
                                ElseIf dt_Mac.Rows(0).Item("Modello") = "" And dt_Mac.Rows(0).Item("Ultima_Manutenzione") <> "01/01/1900" Then
                                    strMacchina += dt_Mac.Rows(0).Item("CLASS_DESC")
                                    If Not bNascondiDataUltimaManutenzione Then
                                        strMacchina += "; Ultima Manutenzione " + dt_Mac.Rows(0).Item("Ultima_Manutenzione") + "),"
                                    Else
                                        strMacchina += "),"
                                    End If
                                ElseIf dt_Mac.Rows(0).Item("Modello") = "" And dt_Mac.Rows(0).Item("Ultima_Manutenzione") = "01/01/1900" Then
                                    strMacchina += dt_Mac.Rows(0).Item("CLASS_DESC") + ","
                                End If

                                If InStr(strCostiMacchine, strMacchina) = 0 Then
                                    strCostiMacchine &= strMacchina
                                End If

                            End If

                            'If InStr(strCostiMacchine, CStr(XML_Ricetta_Dettaglio.GetAttribute("mac_des"))) = 0 Then
                            '    strCostiMacchine &= CStr(XML_Ricetta_Dettaglio.GetAttribute("mac_des")) & ","  '" (" & Dt_Costi.Rows(a).Item("Class_Desc") & "),"
                            'End If

                        Case AgronicaCoreDataProvider.CostantiPersonalizzate.CAU_IMPUTAZIONE_TERZISTI,
                             AgronicaCoreDataProvider.CostantiPersonalizzate.CAU_IMPUTAZIONE_MANODOPERA

                            If XML_Ricetta_Dettaglio.GetAttribute("rag_soc") <> "" Then
                                nomeContatto = XML_Ricetta_Dettaglio.GetAttribute("rag_soc")
                            Else
                                nomeContatto = String.Format("{0} {1}", XML_Ricetta_Dettaglio.GetAttribute("cognome"), XML_Ricetta_Dettaglio.GetAttribute("nome"))
                            End If

                            If InStr(strCostiOperatori, nomeContatto) = 0 Then
                                strCostiOperatori &= nomeContatto & ","
                            End If

                        Case AgronicaCoreDataProvider.CostantiPersonalizzate.CAU_IMPUTAZIONE_TECNICO_RESPONSABILE

                            If XML_Ricetta_Dettaglio.GetAttribute("rag_soc") <> "" Then
                                nomeContatto = XML_Ricetta_Dettaglio.GetAttribute("rag_soc")
                            Else
                                nomeContatto = String.Format("{0} {1}", XML_Ricetta_Dettaglio.GetAttribute("cognome"), XML_Ricetta_Dettaglio.GetAttribute("nome"))
                            End If

                            If InStr(strCostiResponsabile, nomeContatto) = 0 Then
                                strCostiResponsabile = nomeContatto & ","
                            End If

                        Case CAU_SCARICO

                            SetDictionaryView(DictMagazziniEsterni, XML_Ricetta_Dettaglio, frm_FrCod)

                    End Select


                Next


                'If strCostiOperatori <> "" And strCostiMacchine <> "" Then

                '    strCosti = "A cura di : " & Left(strCostiOperatori, strCostiOperatori.Length - 1) & " con : " & Left(strCostiMacchine, strCostiMacchine.Length - 1)

                'ElseIf strCostiOperatori <> "" And strCostiMacchine = "" Then

                '    strCosti = "A cura di : " & Left(strCostiOperatori, strCostiOperatori.Length - 1)

                'ElseIf strCostiOperatori = "" And strCostiMacchine <> "" Then

                '    strCosti = "Eseguito con : " & Left(strCostiMacchine, strCostiMacchine.Length - 1)

                'End If


                If Not bNascondiAutorizzatore And strCostiResponsabile <> String.Empty Then

                    strCosti = "Autorizzato da " & Left(strCostiResponsabile, strCostiResponsabile.Length - 1) & vbCrLf & vbCrLf

                End If

                If Not bNascondiOperatori And strCostiOperatori <> String.Empty Then

                    strCosti &= "A cura di : " & Left(strCostiOperatori, strCostiOperatori.Length - 1)

                Else

                    strCosti &= "Eseguito"

                End If

                If Not bNascondiMacchine And strCostiMacchine <> String.Empty Then

                    strCosti &= " con : " & Left(strCostiMacchine, strCostiMacchine.Length - 1)

                Else

                    If InStr(strCosti, "Eseguito") <> 0 Then
                        Left(strCosti, strCosti.Length - 8)
                    End If

                End If


            End If

        End If


        'FINE SPACCHETTAMENTO

        '------------------------------------------
        '------------------------------------------
        '------------------------------------------



        'recupero le MISCELE
        For i = 0 To UBound(frm_MiscelaCod)

            Miscela_Cod = frm_MiscelaCod(i)

            TrovataMiscela = False

            For j = 0 To UBound(Miscele)

                Miscela = Miscele(j)

                If Not Miscela Is Nothing Then

                    DatiMiscela = Split(Miscela, "$")
                    MiscelaCod = DatiMiscela(0)

                    If MiscelaCod = Miscela_Cod Then

                        'Fr_Cod/Fr_Des/Dose/Udm_Cod/Udm_Cod_Trasformato/Udm_Sim/Mezzo
                        Miscele(j) = Miscele(j) &
                                    frm_FrCod(i) & "£" &
                                    frm_FrDes(i) & "£" &
                                    frm_Dose(i) & "£" &
                                    frm_UdmCod(i) & "£" &
                                    frm_UdmCodTrasformato(i) & "£" &
                                    frm_UdmSim(i) & "£" &
                                    frm_Mezzo &
                                    "$"

                        TrovataMiscela = True
                    End If

                End If

            Next

            If TrovataMiscela = False Then

                ReDim Preserve Miscele(N_Miscele)
                'Fr_Cod/Fr_Des/Dose/Udm_Cod/Udm_Cod_Trasformato/Udm_Sim/Mezzo
                Miscele(N_Miscele) = frm_MiscelaCod(i) & "$" &
                                        frm_FrCod(i) & "£" &
                                        frm_FrDes(i) & "£" &
                                        frm_Dose(i) & "£" &
                                        frm_UdmCod(i) & "£" &
                                        frm_UdmCodTrasformato(i) & "£" &
                                        frm_UdmSim(i) & "£" &
                                        frm_Mezzo &
                                        "$"
                N_Miscele += 1

            End If

        Next

        'elimino miscela_cod dal vettore contenente le MISCELE
        For i = 0 To UBound(Miscele)
            Miscele(i) = Left(Mid(Miscele(i), Miscele(i).IndexOf("$") + 2), Mid(Miscele(i), Miscele(i).IndexOf("$") + 2).Length - 1)
        Next

        'Recupero il datatable
        Dt = ViewState("dtDosi_Concimazione")

        'creo il dt dei formulati dal vettore miscele
        Scombinatore_Concimazione(Miscele, Dt)


        '------------------------------------------
        '----- Inserisco le righe nel dataset
        '------------------------------------------

        Dim drR As DS_Ricetta_Fertilizzazioni.DT_Ricetta_FertilizzazioniRow

        Dim strCul_Des As String = ""

        Dim Mezzo As Integer
        Dim Extra_Int As Integer
        Dim Udm_Sim As String
        Dim Qta As Double
        Dim Qta_Tot As Double
        Dim Dt_Imp As DataTable

        Dim N_app As String = ""
        Dim Num_Appezzamento As Integer
        Dim N_Appezza As Integer

        Dim objApp As New AgronicaCoreAnagrafeDAL.Appezzamento_Read

        For i = 0 To Dt.Rows.Count - 1

            'creo una nuova riga del dataset
            drR = DsFertilizzazioni.DT_Ricetta_Fertilizzazioni.NewDT_Ricetta_FertilizzazioniRow


            If Qs_Stampa_Tipo = enum_TipoStampaRicetta.PianoLavori Then

                If i = Dt.Rows.Count - 1 Then
                    'valore fittizio per indicare ultima riga
                    'da usare per formula omissione linea su report
                    drR.PrimaRigaAppezza = 11
                Else
                    drR.PrimaRigaAppezza = 0
                End If
                drR.App_nome = ""
                drR.App_Nome_Breve = ""
                drR.Sa_Nome = ""
                drR.Campo_Nome = ""
                drR.Veg_Des = ""
                drR.Cul_Des = ""
                drR.Regolamento = ""
                drR.Sup_App = 0
                drR.Indirizzo = ""
                drR.Telefono = ""
            End If

            drR.Ricetta_Operazione_Cod = frm_OperazioneCod

            drR.Lav_Cod = Lav_Cod
            drR.Lav_Des = Lav_Des

            If i = 0 Then

                'data operazione 
                drR.Data = frm_Data

                If Qs_Stampa_Tipo <> enum_TipoStampaRicetta.PianoLavori Then

                    If Not ArrayPiva Is Nothing Then

                        Dim ObjImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                        Dt_Imp = ObjImp.Leggi_Dati_Impianti(ArrayPiva,
                                                            ArraySa_Cod,
                                                            ArrayAppezza,
                                                            ArrayId_Reg, drR.Data, drR.Data,
                                                            "", "", objParametri_Server)
                        ObjImp = Nothing

                        For j = 0 To Dt_Imp.Rows.Count - 1

                            'sup_tot
                            'Sup_Tot_Op += Dt_Imp.Rows(j).Item("sup_imp")

                            'cul_des
                            If InStr(strCul_Des, Dt_Imp.Rows(j).Item("cul_des")) = 0 Then
                                strCul_Des &= Dt_Imp.Rows(j).Item("cul_des") & ", "
                            End If

                            'N.ro appezzamento
                            N_app = Dt_Imp.Rows(j).Item("app_nome_breve")

                            N_Appezza = Dt_Imp.Rows(j).Item("appezza") - BaseCode

                            N_app = objApp.Numero_Appezzamento(Dt_Imp.Rows(j).Item("piva"), Dt_Imp.Rows(j).Item("sa_cod"),
                                                       Dt_Imp.Rows(j).Item("campo_des"), Dt_Imp.Rows(j).Item("appezza"),
                                                       TipoCodiceAppezzamento,
                                                       Dt_Imp.Rows(j).Item("app_nome"), Dt_Imp.Rows(j).Item("app_nome_breve"),
                                                       N_Appezza, BaseCode,
                                                       Nothing, flag_nascondiCampo)

                            frm_Dest &= N_app & ", "

                        Next

                        If strCul_Des <> "" Then
                            strCul_Des = Left(strCul_Des, strCul_Des.Length - 2)
                        End If

                        If frm_Dest <> "" Then
                            frm_Dest = Left(frm_Dest, frm_Dest.Length - 2)
                        End If

                    End If

                End If 'STAMPA 3

                'destinazioni
                drR.Destinazioni = frm_Dest

                'cultivar
                drR.Cul_Des = strCul_Des & " (" & Sup_Tot_Op.ToString & " Ha)"


                '--------------
                'note
                drR.Note = ""

                If frm_NoteIntervento <> "" Then
                    drR.Note = "NOTA: " & frm_NoteIntervento
                End If

                If frm_RicettaNote <> "" Then
                    If drR.Note <> "" Then
                        drR.Note &= vbCrLf & frm_RicettaNote
                    Else
                        drR.Note = frm_RicettaNote
                    End If
                End If

                If strCosti <> "" Then
                    If drR.Note <> "" Then
                        drR.Note &= vbCrLf & strCosti
                    Else
                        drR.Note = strCosti
                    End If
                End If


                Select Case frm_Acqua
                    Case 0
                        drR.Qta_Ril = ""
                        frm_Acqua_Tot = 0
                    Case Is < 0
                        drR.Qta_Ril = Math.Abs(frm_Acqua) & " [Hl/Ha]"
                        frm_Acqua_Tot = Math.Abs(frm_Acqua) * Sup_Tot_Op
                    Case Is > 0
                        drR.Qta_Ril = frm_Acqua & " [Hl]"
                        frm_Acqua_Tot = frm_Acqua
                End Select

            Else

                drR.Lav_Des = ""
                drR.Note = ""
                drR.Qta_Ril = ""

            End If


            For j = 1 To 3

                If Dt.Rows(i).Item("Fer_Cod" & j) <> -1 Then

                    If j = 1 Then 'Se siamo nella prima riga della ricetta

                        drR.Pro_Cod = Dt.Rows(i).Item("Fer_Cod" & j)
                        drR.Fer_Des = Dt.Rows(i).Item("Fer_Des" & j)

                        'drR.Udm_Cod = Dt.Rows(i).Item("Udm_Cod" & j)
                        'drR.Udm_Des = Dt.Rows(i).Item("Udm_Sim" & j)

                        '---------------------------------
                        'Dose
                        Qta_Tot = 0

                        Extra_Int = Dt.Rows(i).Item("Udm_Cod" & j)
                        Mezzo = Dt.Rows(i).Item("Mezzo" & j)
                        Qta = Dt.Rows(i).Item("Dose" & j)

                        'converto in Kg o l
                        Select Case Extra_Int
                            Case 2  'kg
                                Udm_Sim = " kg"
                            Case 3  'g
                                Udm_Sim = " kg"
                                Qta = Qta / 1000
                            Case 29 'l
                                Udm_Sim = " lt"
                            Case 4  'q
                                Udm_Sim = " kg"
                                Qta = Qta * 100
                            Case 304  't
                                Udm_Sim = " kg"
                                Qta = Qta * 1000
                            Case 104  'cc
                                Udm_Sim = " lt"
                                Qta = Qta / 100
                            Case 101  'ml
                                Udm_Sim = " lt"
                                Qta = Qta / 1000
                        End Select

                        'se ho gli impianti calcolo i dosaggi
                        If Sup_Tot_Op <> 0 Then

                            Select Case drR.Lav_Cod

                                Case AgronicaCoreDataProvider.CostantiPersonalizzate.LAVCOD_DISTRIBUZIONE_CONCIME,
                                     AgronicaCoreDataProvider.CostantiPersonalizzate.LAVCOD_DISTRIBUZIONE_AMMENDANTI,
                                     AgronicaCoreDataProvider.CostantiPersonalizzate.LAVCOD_SARCHIATURA_CONCIMAZIONE

                                    Select Case Mezzo
                                        Case 0     'qtà totale
                                            Qta = Qta / Sup_Tot_Op
                                        Case 1  'qtà x ettaro
                                    End Select

                                Case Else

                                    Select Case Mezzo
                                        Case 0   'qtà x ettolitro
                                            Qta = Qta * frm_Acqua_Tot / Sup_Tot_Op
                                        Case 1  'qtà x ettaro
                                    End Select

                            End Select
                        End If

                        Qta = Math.Round(Qta, 3)
                        drR.Qta = Qta.ToString & Udm_Sim
                        drR.Qta_Tot = Math.Round(Qta * Sup_Tot_Op, 3) & Udm_Sim

                        'drR.Qta = Format(Qta, "0.0000")
                        'drR.Qta_Tot = Format(Qta * Sup_Tot, "0.0000")

                        '----------------------------
                        'drR.Mezzo = Dt.Rows(i).Item("Mezzo" & j)

                        'If drR.Mezzo = 1 Then
                        '    Mezzo = "/Ha"
                        'Else
                        '    Mezzo = "/Hl"
                        'End If

                        'drR.Qta = Dt.Rows(i).Item("Dose" & j) & " " & UdmSim & Mezzo

                        'Effettuo una ricerca nel vettore di struct Elemento Concime dove erano stati inseriti i valori di 
                        Dim Trov_fert_in_locale As Boolean = False
                        Dim l As Integer
                        Dim temp_pro_cod As Integer = CInt(Dt.Rows(i).Item("Fer_Cod" & j))
                        For l = 0 To ArrayElementoConcimazione.Length - 1
                            If temp_pro_cod = ArrayElementoConcimazione(l).prod_cod Then
                                Trov_fert_in_locale = True
                                Exit For
                            End If
                        Next
                        Dim elemConc As ElementoConcimazione
                        If Trov_fert_in_locale = True Then
                            elemConc = ArrayElementoConcimazione(l)
                        Else
                            'Ecco dove cavolo prende gli NPK del fertilizzante
                            Fertilizzanti_R.Titoli_from_FerCod(CInt(Dt.Rows(i).Item("Fer_Cod" & j)), Nothing, elemConc.N, elemConc.P, elemConc.K, elemConc.Mg, objParametri_Server)
                            elemConc.Effic = "1"
                        End If

                        drR.Titolo = elemConc.N & " - " & elemConc.P & " - " & elemConc.K


                        'Se piano distribuzione concimi carico NPK Massimi per inserirli poi nel report
                        If (Ricetta_tipo = enum_TipoRicetta.PianoDistribuzioneConcimi) Then


                            'NPK ditribuito e residuo della prima riga
                            N_distribuito = Math.Round(((CDbl(elemConc.N)) * (CDbl(elemConc.Effic)) * Qta / 100), 3)
                            drR.N_Distribuito = CStr(N_distribuito)
                            P_distribuito = Math.Round(((CDbl(elemConc.P)) * (CDbl(elemConc.Effic)) * Qta / 100), 3)
                            drR.P_Distribuito = CStr(P_distribuito)
                            K_distribuito = Math.Round(((CDbl(elemConc.K)) * (CDbl(elemConc.Effic)) * Qta / 100), 3)
                            drR.K_Distribuito = CStr(K_distribuito)
                            If N_Residuo_tot <> "" AndAlso P_Residuo_tot <> "" AndAlso K_Residuo_tot <> "" Then
                                N_residuo = Math.Round(((CDbl(N_Residuo_tot) - N_distribuito)), 3)
                                drR.N_Residuo = CStr(N_residuo)
                                P_residuo = Math.Round(((CDbl(P_Residuo_tot) - P_distribuito)), 3)
                                drR.P_Residuo = CStr(P_residuo)
                                K_residuo = Math.Round(((CDbl(K_Residuo_tot) - K_distribuito)), 3)
                                drR.K_Residuo = CStr(K_residuo)
                            Else
                                N_residuo = Math.Round(((CInt(N_Max) - N_distribuito)), 3)
                                drR.N_Residuo = CStr(N_residuo)
                                P_residuo = Math.Round(((CInt(P_Max) - P_distribuito)), 3)
                                drR.P_Residuo = CStr(P_residuo)
                                K_residuo = Math.Round(((CInt(K_Max) - K_distribuito)), 3)
                                drR.K_Residuo = CStr(K_residuo)
                            End If
                        End If


                    Else

                        drR.Pro_Cod &= vbCrLf & Dt.Rows(i).Item("Fer_Cod" & j)
                        drR.Fer_Des &= vbCrLf & Dt.Rows(i).Item("Fer_Des" & j)

                        'drR.Udm_Cod &= vbCrLf & Dt.Rows(i).Item("Udm_Cod" & j)
                        'drR.Udm_Des &= vbCrLf & Dt.Rows(i).Item("Udm_Cod" & j)

                        'drR.Qta &= vbCrLf & Dt.Rows(i).Item("Dose" & j) & " " & UdmSim & Mezzo

                        '---------------------------------
                        'Dose
                        Qta_Tot = 0

                        Extra_Int = Dt.Rows(i).Item("Udm_Cod" & j)
                        Mezzo = Dt.Rows(i).Item("Mezzo" & j)
                        Qta = Dt.Rows(i).Item("Dose" & j)

                        'converto in Kg o l
                        Select Case Extra_Int
                            Case 2  'kg
                                Udm_Sim = " kg"
                            Case 3  'g
                                Udm_Sim = " kg"
                                Qta = Qta / 1000
                            Case 29 'l
                                Udm_Sim = " lt"
                            Case 4  'q
                                Udm_Sim = " kg"
                                Qta = Qta * 100
                            Case 304  't
                                Udm_Sim = " kg"
                                Qta = Qta * 1000
                            Case 104  'cc
                                Udm_Sim = " lt"
                                Qta = Qta / 100
                            Case 101  'ml
                                Udm_Sim = " lt"
                                Qta = Qta / 1000
                        End Select

                        Select Case drR.Lav_Cod

                            Case AgronicaCoreDataProvider.CostantiPersonalizzate.LAVCOD_DISTRIBUZIONE_CONCIME,
                                 AgronicaCoreDataProvider.CostantiPersonalizzate.LAVCOD_DISTRIBUZIONE_AMMENDANTI,
                                 AgronicaCoreDataProvider.CostantiPersonalizzate.LAVCOD_SARCHIATURA_CONCIMAZIONE

                                Select Case Mezzo
                                    Case 0     'qtà totale
                                        Qta = Qta / Sup_Tot_Op
                                    Case 1  'qtà x ettaro
                                End Select

                            Case Else

                                Select Case Mezzo
                                    Case 0   'qtà x ettolitro
                                        Qta = Qta * frm_Acqua_Tot / Sup_Tot_Op
                                    Case 1  'qtà x ettaro
                                End Select

                        End Select

                        Qta = Math.Round(Qta, 3) 'Dose /Ha
                        drR.Qta &= vbCrLf & Qta.ToString & Udm_Sim
                        drR.Qta_Tot &= vbCrLf & Math.Round(Qta * Sup_Tot_Op, 3) & Udm_Sim

                        '----------------------------

                        'Effettuo una ricerca nel vettore di struct Elemento Concime dove erano stati inseriti i valori di 
                        Dim Trov_fert_in_locale As Boolean = False
                        Dim l As Integer
                        Dim temp_pro_cod As Integer = CInt(Dt.Rows(i).Item("Fer_Cod" & j))
                        For l = 0 To ArrayElementoConcimazione.Length - 1
                            If temp_pro_cod = ArrayElementoConcimazione(l).prod_cod Then
                                Trov_fert_in_locale = True
                                Exit For
                            End If
                        Next
                        Dim elemConc As ElementoConcimazione
                        If Trov_fert_in_locale = True Then
                            elemConc = ArrayElementoConcimazione(l)
                        Else
                            'Ecco dove cavolo prende gli NPK del fertilizzante
                            Fertilizzanti_R.Titoli_from_FerCod(CInt(Dt.Rows(i).Item("Fer_Cod" & j)), Nothing, elemConc.N, elemConc.P, elemConc.K, elemConc.Mg, objParametri_Server)
                            elemConc.Effic = "1"
                        End If
                        drR.Titolo &= vbCrLf & elemConc.N & " - " & elemConc.P & " - " & elemConc.K
                        'Inserire qui il distribuito e residuo dalla seconda riga in poi..
                        N_distribuito = Math.Round(((CDbl(elemConc.N)) * (CDbl(elemConc.Effic)) * Qta / 100), 3)
                        drR.N_Distribuito &= vbCrLf & CStr(N_distribuito)
                        P_distribuito = Math.Round(((CDbl(elemConc.P)) * (CDbl(elemConc.Effic)) * Qta / 100), 3)
                        drR.P_Distribuito &= vbCrLf & CStr(P_distribuito)
                        K_distribuito = Math.Round(((CDbl(elemConc.K)) * (CDbl(elemConc.Effic)) * Qta / 100), 3)
                        drR.K_Distribuito &= vbCrLf & CStr(K_distribuito)
                        N_residuo = Math.Round((N_residuo - N_distribuito), 3)
                        drR.N_Residuo &= vbCrLf & CStr(N_residuo)
                        P_residuo = Math.Round((P_residuo - P_distribuito), 3)
                        drR.P_Residuo &= vbCrLf & CStr(P_residuo)
                        K_residuo = Math.Round((K_residuo - K_distribuito), 3)
                        drR.K_Residuo &= vbCrLf & CStr(K_residuo)
                        N_Residuo_tot = N_residuo
                        P_Residuo_tot = P_residuo
                        K_Residuo_tot = K_residuo

                    End If

                    AddMagazzinoEsternoNote(drR.Note, DictMagazziniEsterni, Dt.Rows(i).Item("Fer_Cod" & j))

                End If

            Next

            'aggiungo la nuova riga al dataset
            DsFertilizzazioni.DT_Ricetta_Fertilizzazioni.Rows.Add(drR)

        Next

        'ripulisco il Dt di appoggio
        Dt.Rows.Clear()

        ViewState("dtDosi_Concimazione") = Dt


    End Sub



    '#########################################################################################
    Private Sub Scombinatore_Concimazione(ByVal Vettore() As String,
                                          ByRef DT As DataTable)

        Dim Dr As DataRow

        Dim Pezzi() As String
        Dim MaxIndice As Integer

        Dim Dati() As String

        Dim Fr_Cod() As String
        Dim Fr_Des() As String
        Dim Udm_Cod() As String
        Dim Udm_Sim() As String
        Dim Dose() As String
        Dim Mezzo As String

        '-----------------------------------------------------------------------------------
        '--- Scopro quante righe deve avere la matrice ... ogni riga avra' un contatore
        '-----------------------------------------------------------------------------------

        Pezzi = Split(Vettore(0), "$")
        MaxIndice = UBound(Pezzi)

        Dim Matrice(MaxIndice, 0) As String
        Dim Contatori(MaxIndice) As Integer

        '-----------------------------------------------------------------------------------
        '--- Scansione delle righe del vettore
        '-----------------------------------------------------------------------------------

        Dim i As Integer
        Dim j As Integer
        Dim h As Integer
        Dim Trovato As Boolean

        'Azzero i contatori
        For i = 0 To MaxIndice
            Contatori(i) = 0
        Next


        'Per ogni riga del vettore
        For i = LBound(Vettore) To UBound(Vettore)

            Pezzi = Split(Vettore(i), "$")

            'Per ogni codice presente nella cella del vettore
            For j = 0 To UBound(Pezzi)

                Trovato = False

                'Cerco l'elemento nella riga j-esima della matrice
                For h = 0 To Contatori(j)
                    If Matrice(j, h) = Pezzi(j) Then
                        Trovato = True
                        Exit For
                    End If
                Next

                If Trovato = False Then

                    Matrice(j, Contatori(j)) = Pezzi(j)
                    Contatori(j) += 1

                    If UBound(Matrice, 2) < Contatori(j) Then
                        ReDim Preserve Matrice(MaxIndice, Contatori(j))
                    End If

                End If

            Next

        Next

        ReDim Preserve Matrice(MaxIndice, UBound(Matrice, 2) - 1)

        For i = 0 To UBound(Matrice, 1)
            For j = 0 To UBound(Matrice, 2)
                If Matrice(i, j) Is Nothing Then
                    Matrice(i, j) = -1
                End If
            Next
        Next


        For i = 0 To UBound(Matrice)

            For j = 0 To UBound(Matrice, 2)

                Dati = Split(Matrice(i, j), "£")

                If Dati(0) <> "-1" Then

                    'Fr_Cod/Fr_Des/Dose/Udm_Cod/Udm_Cod_Trasformato/Udm_Sim/Mezzo

                    ReDim Preserve Fr_Cod(j)
                    Fr_Cod(j) = Dati(0)

                    ReDim Preserve Fr_Des(j)
                    Fr_Des(j) = Dati(1)

                    ReDim Preserve Dose(j)
                    Dose(j) = Dati(2)

                    ReDim Preserve Udm_Cod(j)
                    Udm_Cod(j) = Dati(3)

                    ReDim Preserve Udm_Sim(j)
                    Udm_Sim(j) = Dati(5)

                    Mezzo = Dati(6)

                Else

                    ReDim Preserve Fr_Cod(j)
                    Fr_Cod(j) = Dati(0)

                    ReDim Preserve Fr_Des(j)
                    Fr_Des(j) = ""

                    ReDim Preserve Dose(j)
                    Dose(j) = 0

                    ReDim Preserve Udm_Cod(j)
                    Udm_Cod(j) = -1

                    ReDim Preserve Udm_Cod(j)
                    Udm_Sim(j) = ""

                End If

            Next

            Dosi_Inserisci_Concimazione(Fr_Cod,
                                        Fr_Des,
                                        Dose,
                                        Udm_Cod,
                                        Udm_Sim,
                                        Mezzo)


        Next


    End Sub

    '########################################################################################
    Private Sub Dosi_Inserisci_Concimazione(ByVal Fer_Cod As Array,
                                            ByVal Fer_Des As Array,
                                            Optional ByVal Dose As Array = Nothing,
                                            Optional ByVal UdmCod As Array = Nothing,
                                            Optional ByVal UdmSim As Array = Nothing,
                                            Optional ByVal Mezzo As Integer = 1)

        '----- Dimensiono le variabili

        Dim Fertilizzanti_R As New AgronicaCoreMetaSchemaDAL.Fertilizzanti_R

        Dim Dt As DataTable
        Dim Dr As DataRow
        Dim i As Integer
        Dim N_Fer As Integer = 0

        'Recupero il datatable
        Dt = ViewState("dtDosi_Concimazione")

        '----- Inserisco il nuovo record

        N_Fer = UBound(Fer_Cod) + 1

        'Creo una nuova riga
        Dr = Dt.NewRow

        For i = 1 To N_Fer

            Dr.Item("Fer_Cod" & i) = Fer_Cod(i - 1)

            If Fer_Des(i - 1) <> "" Then
                Dr.Item("Fer_Des" & i) = Fer_Des(i - 1)
            Else
                Dr.Item("Fer_Des" & i) = Fertilizzanti_R.FerDes_from_FerCod(CInt(Fer_Cod(i - 1)), objParametri_Server)
            End If

            If Not Dose Is Nothing Then

                Dr.Item("Udm_Cod" & i) = UdmCod(i - 1)
                Dr.Item("Udm_sim" & i) = UdmSim(i - 1)
                Dr.Item("Dose" & i) = Dose(i - 1)
                Dr.Item("Dose_Fittizia" & i) = Dose(i - 1)
                Dr.Item("Mezzo" & i) = Mezzo
                'Dr.Item("Qta_Tot" & i) = -1
            Else

                Dr.Item("Udm_sim" & i) = ""
                Dr.Item("Dose" & i) = Dose(i - 1)
                Dr.Item("Dose_Fittizia" & i) = Math.Round(Dose(i - 1), 3)
                Dr.Item("Mezzo" & i) = 1
                Dr.Item("Qta_Tot" & i) = Dose(i - 1)

            End If

        Next

        For i = N_Fer + 1 To 3

            Dr.Item("Fer_Cod" & i) = -1
            Dr.Item("Fer_Des" & i) = "---"
            Dr.Item("Udm_Cod" & i) = -1
            Dr.Item("Udm_sim" & i) = ""

            Dr.Item("Dose" & i) = -1
            Dr.Item("Dose_Fittizia" & i) = -1
            Dr.Item("Mezzo" & i) = 1
            Dr.Item("Qta_Tot" & i) = -1

        Next


        'controllo mezzo
        '(le dosi devono essere impostate tutte o/ha o /hl)
        If Dr.Item("Mezzo1") = 1 Then
            Dr.Item("Mezzo2") = 1
            Dr.Item("Mezzo3") = 1
        Else
            Dr.Item("Mezzo2") = 0
            Dr.Item("Mezzo3") = 0
        End If


        'Associo alla tabella la nuova riga creata
        Dt.Rows.Add(Dr)

        '----- Salvo il DataTable dentro il viewstate

        ViewState("dtDosi_Concimazione") = Dt


    End Sub

    '###########################################################################
    Private Sub Inserisci_Irrigazione(ByVal strOperazione As String,
                                      TipoCodiceAppezzamento As Integer, flag_nascondiCampo As Boolean, ByVal BaseCode As Integer)

        Dim Dt As DataTable

        Dim XmlDoc As New System.Xml.XmlDocument

        Dim XML_Ricetta_Operazione As System.Xml.XmlElement

        Dim XML_DatiRicetta_Dettagli_Tecnici As System.Xml.XmlElement
        Dim XML_Ricetta_Dettaglio_Tecnico As System.Xml.XmlElement
        Dim XMLs_Ricetta_Dettaglio_Tecnico As System.Xml.XmlNodeList

        Dim XML_DatiRicetta_Dettagli As System.Xml.XmlElement
        Dim XML_Ricetta_Dettaglio As System.Xml.XmlElement
        Dim XMLs_Ricetta_Dettaglio As System.Xml.XmlNodeList

        Dim XML_Ricetta_Dettaglio_Tecnico_2 As System.Xml.XmlElement
        Dim XMLs_Ricetta_Dettaglio_Tecnico_2 As System.Xml.XmlNodeList

        Dim Lav_Cod As String
        Dim Lav_Des As String

        Dim frm_Data As String

        Dim frm_OperazioneCod As Integer
        Dim frm_OperazioneDes As String
        Dim frm_NoteIntervento As String

        Dim i, j, a As Integer
        Dim frm_Cau_Mov As String
        Dim frm_UdmCod As Integer
        Dim frm_Dose As Double
        Dim frm_Ore As Double
        Dim frm_Portata As Double
        Dim frm_DataInizio As Date
        Dim frm_DataFine As Date
        Dim frm_Frequenza As Integer
        Dim frm_N_Distribuito As Double
        Dim frm_P_Distribuito As Double
        Dim frm_K_Distribuito As Double

        Dim UdmDes As String
        Dim UdmSim As String


        Dim frm_Dest As String
        Dim XML_Ricetta_Dettaglio_Destinazione As System.Xml.XmlElement
        Dim XMLs_Ricetta_Dettaglio_Destinazione As System.Xml.XmlNodeList


        '''''' note '''''
        Dim frm_RicettaNote As String
        Dim XML_Ricetta_Note As System.Xml.XmlElement
        Dim XMLs_Ricetta_Note As System.Xml.XmlNodeList
        Dim XML_DatiRicetta_Note2 As System.Xml.XmlElement

        Dim TrovatoImpianto As Boolean
        Dim ArrayPiva() As String
        Dim ArraySa_Cod() As Integer
        Dim ArrayAppezza() As Integer
        Dim ArrayId_Reg() As Integer

        '------------------------------------------
        '----- Analizzo la stringa XML
        '------------------------------------------

        'Carico la stringa nel documento XML
        XmlDoc.LoadXml(strOperazione)

        '----- Tag Ricetta_Operazione

        XML_Ricetta_Operazione = XmlDoc.SelectSingleNode("Ricetta_Operazione")

        frm_OperazioneDes = CStr(XML_Ricetta_Operazione.GetAttribute("ricetta_operazione_des"))
        frm_OperazioneCod = CStr(XML_Ricetta_Operazione.GetAttribute("ricetta_operazione_cod"))

        Lav_Cod = CInt(XML_Ricetta_Operazione.GetAttribute("lav_cod"))
        Lav_Des = CStr(XML_Ricetta_Operazione.GetAttribute("lav_des"))

        frm_Data = CStr(XML_Ricetta_Operazione.GetAttribute("validita_inizio"))

        frm_NoteIntervento = CStr(XML_Ricetta_Operazione.GetAttribute("note"))

        If XML_Ricetta_Operazione.HasChildNodes Then

            '----- Tag XML_DatiRicetta_Dettagli

            XML_DatiRicetta_Dettagli = XML_Ricetta_Operazione.SelectSingleNode("DatiRicetta_Dettagli")

            If Not XML_DatiRicetta_Dettagli Is Nothing Then

                '----- Tag Ricetta_Dettaglio 

                'Recupero la collezione dei nodi
                XMLs_Ricetta_Dettaglio = XML_DatiRicetta_Dettagli.GetElementsByTagName("Ricetta_Dettaglio")
                'XML_Ricetta_Dettaglio = XML_DatiRicetta_Dettagli.SelectSingleNode("Ricetta_Dettaglio")

                For i = 0 To XMLs_Ricetta_Dettaglio.Count - 1

                    'Prendo l'i-esimo nodo della collezione
                    XML_Ricetta_Dettaglio = XMLs_Ricetta_Dettaglio.Item(i)

                    frm_Cau_Mov = XML_Ricetta_Dettaglio.GetAttribute("cau_mov")

                    Select Case frm_Cau_Mov

                        Case "", AgronicaCoreDataProvider.CostantiPersonalizzate.CAU_LAVORAZIONE

                            XML_Ricetta_Dettaglio_Tecnico_2 = XML_Ricetta_Dettaglio.SelectSingleNode("Ricetta_Dettaglio_Tecnico_2")

                            'Recupero i valori
                            frm_Dose = CDbl(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("qta_ril"))
                            frm_Ore = CDbl(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("dose"))
                            frm_Portata = CDbl(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("parziale"))
                            frm_DataInizio = CDate(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("inn1_data"))
                            frm_DataFine = CDate(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("inn2_data"))
                            frm_Frequenza = CInt(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("nitrati"))
                            frm_UdmCod = CInt(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("dett_cod"))


                            'Destinazioni ''''''''''''''''''''''''''''
                            XMLs_Ricetta_Dettaglio_Destinazione = XML_Ricetta_Dettaglio.GetElementsByTagName("Ricetta_Destinazione")

                            If Not XMLs_Ricetta_Dettaglio_Destinazione Is Nothing AndAlso XMLs_Ricetta_Dettaglio_Destinazione.Count > 0 Then

                                For j = 0 To XMLs_Ricetta_Dettaglio_Destinazione.Count - 1

                                    XML_Ricetta_Dettaglio_Destinazione = XMLs_Ricetta_Dettaglio_Destinazione.Item(j)

                                    TrovatoImpianto = False

                                    If Not ArrayPiva Is Nothing Then
                                        For a = 0 To ArrayPiva.Length - 1
                                            If ArrayPiva(a) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("piva") And
                                               ArraySa_Cod(a) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("sa_cod") And
                                               ArrayAppezza(a) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("appezza") And
                                               ArrayId_Reg(a) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("id_reg") Then
                                                TrovatoImpianto = True
                                                Exit For
                                            End If
                                        Next
                                    End If

                                    If TrovatoImpianto = False Then
                                        If ArrayPiva Is Nothing Then
                                            ReDim Preserve ArrayPiva(0)
                                            ReDim Preserve ArraySa_Cod(0)
                                            ReDim Preserve ArrayAppezza(0)
                                            ReDim Preserve ArrayId_Reg(0)
                                        Else
                                            ReDim Preserve ArrayPiva(UBound(ArrayPiva) + 1)
                                            ReDim Preserve ArraySa_Cod(UBound(ArraySa_Cod) + 1)
                                            ReDim Preserve ArrayAppezza(UBound(ArrayAppezza) + 1)
                                            ReDim Preserve ArrayId_Reg(UBound(ArrayId_Reg) + 1)
                                        End If
                                        ArrayPiva(UBound(ArrayPiva)) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("piva")
                                        ArraySa_Cod(UBound(ArraySa_Cod)) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("sa_cod")
                                        ArrayAppezza(UBound(ArrayAppezza)) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("appezza")
                                        ArrayId_Reg(UBound(ArrayId_Reg)) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("id_reg")
                                    End If

                                Next

                            End If
                            'Destinazioni ''''''''''''''''''''''''''''

                    End Select

                Next


            End If




            '''''''''''''''''''''''''NOTE'''''''''''''''''''
            '----- Tag DatiRicetta_Note2
            Dim objNoteIntervento As New AgronicaCoreContabDAL.Note_Intervento_R
            XML_DatiRicetta_Note2 = XML_Ricetta_Operazione.SelectSingleNode("DatiRicettaxNote_2")
            If Not XML_DatiRicetta_Note2 Is Nothing Then

                '----- Tag RicettaxNote_2 
                XMLs_Ricetta_Note = XML_DatiRicetta_Note2.GetElementsByTagName("RicettaxNote_2")

                If Not XMLs_Ricetta_Note Is Nothing AndAlso XMLs_Ricetta_Note.Count > 0 Then

                    For j = 0 To XMLs_Ricetta_Note.Count - 1

                        XML_DatiRicetta_Note2 = XMLs_Ricetta_Note.Item(j)
                        frm_RicettaNote &= objNoteIntervento.NotaDesFromNotaCod(
                                                XML_DatiRicetta_Note2.GetAttribute("nota_cod"),
                                                Session("ASG_objParametri_Server")) & ", "

                    Next

                    If frm_RicettaNote <> "" Then
                        frm_RicettaNote = Left(frm_RicettaNote, frm_RicettaNote.Length - 2)
                    End If
                End If

            End If
            '''''''''''''''''''''''''NOTE'''''''''''''''''''


        End If



        'FINE SPACCHETTAMENTO


        '------------------------------------------
        '----- Inserisco le righe nel dataset
        '------------------------------------------

        Dim Sup_Tot_Op As Double = 0

        Dim strCul_Des As String = ""

        Dim Dt_Imp As DataTable

        Dim N_app As String = ""
        Dim Num_Appezzamento As Integer
        Dim N_Appezza As Integer
        Dim objApp As New AgronicaCoreAnagrafeDAL.Appezzamento_Read

        Dim drR As DS_Ricetta_Irrigazioni.DS_Ricetta_IrrigazioniRow

        'creo una nuova riga del dataset
        drR = DsIrrigazioni.DS_Ricetta_Irrigazioni.NewDS_Ricetta_IrrigazioniRow

        drR.Ricetta_Operazione_Cod = frm_OperazioneCod

        drR.Data = frm_Data

        drR.Lav_Cod = Lav_Cod
        drR.Lav_Des = Lav_Des

        drR.Note = frm_NoteIntervento
        If drR.Note <> "" Then
            drR.Note &= vbCrLf & frm_RicettaNote
        Else
            drR.Note = frm_RicettaNote
        End If

        drR.Qta_Ril = frm_Dose

        drR.Dett_Cod = frm_UdmCod
        UdmDes = New AgronicaCoreMetaSchemaDAL.UnitaMisura_R().UdmDes_from_UdmCod(frm_UdmCod, UdmSim, objParametri_Server)
        drR.Udm_Des = UdmSim

        If frm_Ore <> 0 Then
            drR.Dose = frm_Ore
        Else
            drR.Dose = ""
        End If

        If frm_Portata <> 0 Then
            drR.Parziale = frm_Portata
        Else
            drR.Parziale = ""
        End If

        If frm_Frequenza <> 0 Then
            drR.Nitrati = frm_Frequenza
        Else
            drR.Nitrati = ""
        End If

        If frm_DataInizio <> #1/1/1900# Then
            drR.Inn1_Data = CDate(frm_DataInizio).ToShortDateString
        Else
            drR.Inn1_Data = "..."
        End If

        If frm_DataFine <> #12/31/2100# And frm_DataFine <> #1/1/1900# Then
            drR.Inn2_Data = CDate(frm_DataFine).ToShortDateString
        Else
            drR.Inn2_Data = "..."
        End If

        If Not ArrayPiva Is Nothing Then

            Dim ObjImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dt_Imp = ObjImp.Leggi_Dati_Impianti(ArrayPiva,
                                                ArraySa_Cod,
                                                ArrayAppezza,
                                                ArrayId_Reg, frm_Data, frm_Data,
                                                "", "", objParametri_Server)
            ObjImp = Nothing

            For j = 0 To Dt_Imp.Rows.Count - 1

                'sup_tot
                Sup_Tot_Op += Dt_Imp.Rows(j).Item("sup_imp")

                'cul_des
                If InStr(strCul_Des, Dt_Imp.Rows(j).Item("cul_des")) = 0 Then
                    strCul_Des &= Dt_Imp.Rows(j).Item("cul_des") & ", "
                End If

                'N.ro appezzamento
                N_app = Dt_Imp.Rows(j).Item("app_nome_breve")

                N_Appezza = Dt_Imp.Rows(j).Item("appezza") - BaseCode

                N_app = objApp.Numero_Appezzamento(Dt_Imp.Rows(j).Item("piva"), Dt_Imp.Rows(j).Item("sa_cod"),
                                                   Dt_Imp.Rows(j).Item("campo_des"), Dt_Imp.Rows(j).Item("appezza"),
                                                   TipoCodiceAppezzamento,
                                                   Dt_Imp.Rows(j).Item("app_nome"), Dt_Imp.Rows(j).Item("app_nome_breve"),
                                                   N_Appezza, BaseCode,
                                                   Nothing, flag_nascondiCampo)

                'If N_app = "" Then

                '    Num_Appezzamento = New AgronicaCoreAnagrafeDAL.Appezzamento_Read().AppezzamentoNumero_from_AppezzamentoNome(Dt_Imp.Rows(j).Item("app_nome"))

                '    If Num_Appezzamento <> 0 Then
                '        N_app = CStr(Num_Appezzamento)
                '    Else

                '        If BaseCode = 0 Then
                '            Dim ObjBase As New AgronicaCoreDataProvider.UtilityProvider
                '            BaseCode = ObjBase.BaseCode_from_ProgressivoGias(Session("ASG_ProgressivoGIAS"))
                '            ObjBase = Nothing
                '        End If

                '        N_app = Dt_Imp.Rows(i).Item("appezza") - BaseCode

                '        'se è un valore alto lo tronco alle ultime 3 cifre..
                '        If N_app.Length > 3 Then
                '            N_app = Right(N_app, 3)
                '        End If

                '    End If

                'End If

                frm_Dest &= N_app & ", "

            Next

            If strCul_Des <> "" Then
                strCul_Des = Left(strCul_Des, strCul_Des.Length - 2)
            End If

            If frm_Dest <> "" Then
                frm_Dest = Left(frm_Dest, frm_Dest.Length - 2)
            End If

        End If

        'destinazioni
        drR.Destinazioni = frm_Dest

        'cultivar
        drR.Cul_Des = strCul_Des & " (" & Sup_Tot_Op.ToString & " Ha)"


        'aggiungo la nuova riga al dataset
        DsIrrigazioni.DS_Ricetta_Irrigazioni.Rows.Add(drR)



    End Sub



    '###########################################################################
    Private Sub Inserisci_InsettiUtili(ByVal strOperazione As String,
                                       ByVal bNascondiMacchine As Boolean,
                                       ByVal bNascondiOperatori As Boolean,
                                       ByVal bNascondiAutorizzatore As Boolean,
                                       ByVal bNascondiDataUltimaManutenzione As Boolean,
                                       ByVal TipoCodiceAppezzamento As Integer,
                                       ByVal flag_nascondiCampo As Boolean,
                                       ByVal BaseCode As Integer)

        Dim Dt As DataTable

        Dim XmlDoc As New System.Xml.XmlDocument

        Dim XML_Ricetta_Operazione As System.Xml.XmlElement

        Dim XML_DatiRicetta_Dettagli_Tecnici As System.Xml.XmlElement
        Dim XML_Ricetta_Dettaglio_Tecnico As System.Xml.XmlElement
        Dim XMLs_Ricetta_Dettaglio_Tecnico As System.Xml.XmlNodeList

        Dim XML_DatiRicetta_Dettagli As System.Xml.XmlElement
        Dim XML_Ricetta_Dettaglio As System.Xml.XmlElement
        Dim XMLs_Ricetta_Dettaglio As System.Xml.XmlNodeList

        Dim XML_Ricetta_Dettaglio_Tecnico_2 As System.Xml.XmlElement
        Dim XMLs_Ricetta_Dettaglio_Tecnico_2 As System.Xml.XmlNodeList

        Dim XML_Ricetta_Dettaglio_Destinazione As System.Xml.XmlElement
        Dim XMLs_Ricetta_Dettaglio_Destinazione As System.Xml.XmlNodeList

        Dim Lav_Cod As String
        Dim Lav_Des As String

        Dim frm_Data As String

        Dim frm_OperazioneCod As Integer
        Dim frm_OperazioneDes As String
        Dim frm_NoteIntervento As String

        Dim frm_Mezzo As Integer

        Dim frm_AvCod() As String
        Dim frm_AvDes() As String
        Dim frm_AvGru() As String
        Dim frm_AvGruDes() As String

        Dim frm_Dest As String


        Dim frm_Cau_Mov As String
        Dim frm_ElemCod As Integer

        Dim frm_InsCod() As Integer
        Dim frm_InsDes() As String
        Dim frm_UdmCod() As Integer
        Dim frm_UdmSim() As String
        Dim frm_UdmCodTrasformato() As Integer

        Dim frm_Dose() As Double
        Dim frm_ForVegCod() As String

        Dim i, j, a, m As Integer

        Dim listaMacCod As New List(Of String)

        '''''' note '''''
        Dim frm_RicettaNote As String = ""
        Dim XMLs_Ricetta_Note As System.Xml.XmlNodeList
        Dim XML_DatiRicetta_Note2 As System.Xml.XmlElement

        Dim TrovatoImpianto As Boolean
        Dim ArrayPiva() As String
        Dim ArraySa_Cod() As Integer
        Dim ArrayAppezza() As Integer
        Dim ArrayId_Reg() As Integer

        Dim strCosti As String
        Dim strMacchina As String
        Dim Mac_Des As String
        Dim strCostiMacchine As String
        Dim strCostiOperatori As String
        Dim strCostiResponsabile As String

        Dim nomeContatto As String

        Dim Sup_Tot_Op As Decimal = 0

        Dim PrimoDettaglio As Boolean = True


        Dim DictMagazziniEsterni As New Dictionary(Of (Integer, String, String), String)

        '------------------------------------------
        '----- Analizzo la stringa XML
        '------------------------------------------

        'Carico la stringa nel documento XML
        XmlDoc.LoadXml(strOperazione)

        '----- Tag Ricetta_Operazione

        XML_Ricetta_Operazione = XmlDoc.SelectSingleNode("Ricetta_Operazione")

        frm_OperazioneCod = CStr(XML_Ricetta_Operazione.GetAttribute("ricetta_operazione_cod"))
        frm_OperazioneDes = CStr(XML_Ricetta_Operazione.GetAttribute("ricetta_operazione_des"))

        Dim objRDest As New AgronicaCoreContabDAL.Ricette_Destinazioni_R
        Sup_Tot_Op = objRDest.SupTotTrattata_from_RicettaOperazioneCod(frm_OperazioneCod, "", objParametri_Server)

        Lav_Cod = CInt(XML_Ricetta_Operazione.GetAttribute("lav_cod"))
        Lav_Des = CStr(XML_Ricetta_Operazione.GetAttribute("lav_des"))

        frm_Data = CStr(XML_Ricetta_Operazione.GetAttribute("validita_inizio"))

        frm_NoteIntervento = CStr(XML_Ricetta_Operazione.GetAttribute("note"))

        frm_Mezzo = CInt(XML_Ricetta_Operazione.GetAttribute("mezzo"))

        If XML_Ricetta_Operazione.HasChildNodes Then

            '----- Tag DatiRicetta_Dettagli_Tecnici

            XML_DatiRicetta_Dettagli_Tecnici = XML_Ricetta_Operazione.SelectSingleNode("DatiRicetta_Dettagli_Tecnici")

            If Not XML_DatiRicetta_Dettagli_Tecnici Is Nothing Then

                '----- Tag Ricetta_Dettaglio_Tecnico  (multiplo)

                'Recupero la collezione dei nodi
                XMLs_Ricetta_Dettaglio_Tecnico = XML_DatiRicetta_Dettagli_Tecnici.GetElementsByTagName("Ricetta_Dettaglio_Tecnico")

                Dim xAvCod As Integer
                Dim xAvGru As Integer
                Dim xAvDes As String
                Dim xAvGruDes As String

                'Ciclo su tutti i nodi
                For i = 0 To XMLs_Ricetta_Dettaglio_Tecnico.Count - 1

                    'Prendo l'i-esimo nodo della collezione
                    XML_Ricetta_Dettaglio_Tecnico = XMLs_Ricetta_Dettaglio_Tecnico.Item(i)

                    'Recupero i valori
                    xAvCod = CInt(XML_Ricetta_Dettaglio_Tecnico.GetAttribute("av_cod"))
                    xAvGru = CInt(XML_Ricetta_Dettaglio_Tecnico.GetAttribute("av_gru"))
                    xAvDes = CStr(XML_Ricetta_Dettaglio_Tecnico.GetAttribute("av_des_vol"))
                    xAvGruDes = CStr(XML_Ricetta_Dettaglio_Tecnico.GetAttribute("av_gru_des"))

                    If (xAvCod = 0) And (xAvGru = 0) Then
                        'Acqua
                    ElseIf ((xAvCod <> 0) Or (xAvGru <> 0)) Then
                        'Avversita'
                        If frm_AvCod Is Nothing Then
                            ReDim Preserve frm_AvCod(0)
                            ReDim Preserve frm_AvGru(0)
                            ReDim Preserve frm_AvDes(0)
                            ReDim Preserve frm_AvGruDes(0)
                        Else
                            ReDim Preserve frm_AvCod(UBound(frm_AvCod) + 1)
                            ReDim Preserve frm_AvGru(UBound(frm_AvGru) + 1)
                            ReDim Preserve frm_AvDes(UBound(frm_AvDes) + 1)
                            ReDim Preserve frm_AvGruDes(UBound(frm_AvGruDes) + 1)
                        End If

                        frm_AvCod(UBound(frm_AvCod)) = xAvCod
                        frm_AvGru(UBound(frm_AvGru)) = xAvGru
                        frm_AvDes(UBound(frm_AvDes)) = xAvDes
                        frm_AvGruDes(UBound(frm_AvGruDes)) = xAvGruDes
                    End If
                Next

            End If


            '''''''''''''''''''''''''NOTE'''''''''''''''''''
            '----- Tag DatiRicetta_Note2
            Dim objNoteIntervento As New AgronicaCoreContabDAL.Note_Intervento_R
            XML_DatiRicetta_Note2 = XML_Ricetta_Operazione.SelectSingleNode("DatiRicettaxNote_2")
            If Not XML_DatiRicetta_Note2 Is Nothing Then

                '----- Tag RicettaxNote_2
                XMLs_Ricetta_Note = XML_DatiRicetta_Note2.GetElementsByTagName("RicettaxNote_2")

                If Not XMLs_Ricetta_Note Is Nothing AndAlso XMLs_Ricetta_Note.Count > 0 Then

                    For j = 0 To XMLs_Ricetta_Note.Count - 1

                        XML_DatiRicetta_Note2 = XMLs_Ricetta_Note.Item(j)
                        frm_RicettaNote &= objNoteIntervento.NotaDesFromNotaCod(
                        XML_DatiRicetta_Note2.GetAttribute("nota_cod"),
                        Session("ASG_objParametri_Server")) & ", "

                    Next

                    If frm_RicettaNote <> "" Then
                        frm_RicettaNote = Left(frm_RicettaNote, frm_RicettaNote.Length - 2)
                    End If
                End If

            End If
            '''''''''''''''''''''''''NOTE'''''''''''''''''''



            '----- Tag XML_DatiRicetta_Dettagli

            XML_DatiRicetta_Dettagli = XML_Ricetta_Operazione.SelectSingleNode("DatiRicetta_Dettagli")

            If Not XML_DatiRicetta_Dettagli Is Nothing Then

                '----- Tag Ricetta_Dettaglio  (multiplo)

                'Recupero la collezione dei nodi
                XMLs_Ricetta_Dettaglio = XML_DatiRicetta_Dettagli.GetElementsByTagName("Ricetta_Dettaglio")

                For i = 0 To XMLs_Ricetta_Dettaglio.Count - 1


                    'Prendo l'i-esimo nodo della collezione
                    XML_Ricetta_Dettaglio = XMLs_Ricetta_Dettaglio.Item(i)

                    frm_Cau_Mov = XML_Ricetta_Dettaglio.GetAttribute("cau_mov")

                    Select Case frm_Cau_Mov

                        Case "", AgronicaCoreDataProvider.CostantiPersonalizzate.CAU_TRATTAMENTO

                            XMLs_Ricetta_Dettaglio_Tecnico_2 = XML_Ricetta_Dettaglio.GetElementsByTagName("Ricetta_Dettaglio_Tecnico_2")

                            If Not XMLs_Ricetta_Dettaglio_Tecnico_2 Is Nothing AndAlso XMLs_Ricetta_Dettaglio_Tecnico_2.Count > 0 Then


                                If i = 0 Then
                                    'If frm_AvCod Is Nothing Then
                                    ReDim Preserve frm_AvCod(0)
                                    ReDim Preserve frm_AvGru(0)
                                    ReDim Preserve frm_AvDes(0)
                                    ReDim Preserve frm_AvGruDes(0)
                                Else
                                    ReDim Preserve frm_AvCod(UBound(frm_AvCod) + 1)
                                    ReDim Preserve frm_AvGru(UBound(frm_AvGru) + 1)
                                    ReDim Preserve frm_AvDes(UBound(frm_AvDes) + 1)
                                    ReDim Preserve frm_AvGruDes(UBound(frm_AvGruDes) + 1)
                                End If


                                For j = 0 To XMLs_Ricetta_Dettaglio_Tecnico_2.Count - 1

                                    XML_Ricetta_Dettaglio_Tecnico_2 = XMLs_Ricetta_Dettaglio_Tecnico_2.Item(j)

                                    frm_AvCod(UBound(frm_AvCod)) &= CStr(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("av_cod")) & ","
                                    frm_AvGru(UBound(frm_AvGru)) &= CInt(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("av_gru")) & ","

                                    frm_AvDes(UBound(frm_AvDes)) &= CStr(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("av_des_vol")) & ","
                                    frm_AvGruDes(UBound(frm_AvGruDes)) &= CStr(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("av_gru_des")) & ","

                                Next

                                If frm_AvCod(UBound(frm_AvCod)) <> "" Then
                                    frm_AvCod(UBound(frm_AvCod)) = Left(frm_AvCod(UBound(frm_AvCod)), frm_AvCod(UBound(frm_AvCod)).Length - 1)
                                End If
                                If frm_AvGru(UBound(frm_AvGru)) <> "" Then
                                    frm_AvGru(UBound(frm_AvGru)) = Left(frm_AvGru(UBound(frm_AvGru)), frm_AvGru(UBound(frm_AvGru)).Length - 1)
                                End If
                                If frm_AvDes(UBound(frm_AvDes)) <> "" Then
                                    frm_AvDes(UBound(frm_AvDes)) = Left(frm_AvDes(UBound(frm_AvDes)), frm_AvDes(UBound(frm_AvDes)).Length - 1)
                                End If
                                If frm_AvGruDes(UBound(frm_AvGruDes)) <> "" Then
                                    frm_AvGruDes(UBound(frm_AvGruDes)) = Left(frm_AvGruDes(UBound(frm_AvGruDes)), frm_AvGruDes(UBound(frm_AvGruDes)).Length - 1)
                                End If
                            End If

                            'Inizializzo le variabili
                            If frm_InsCod Is Nothing Then

                                ReDim Preserve frm_InsCod(0)
                                ReDim Preserve frm_InsDes(0)
                                'ReDim Preserve frm_PrincipiA(0)
                                ReDim Preserve frm_UdmCod(0)
                                ReDim Preserve frm_UdmCodTrasformato(0)
                                ReDim Preserve frm_UdmSim(0)
                                ReDim Preserve frm_Dose(0)
                                ReDim Preserve frm_ForVegCod(0)

                            Else

                                ReDim Preserve frm_InsCod(UBound(frm_InsCod) + 1)
                                ReDim Preserve frm_InsDes(UBound(frm_InsDes) + 1)
                                'ReDim Preserve frm_PrincipiA(UBound(frm_PrincipiA) + 1)
                                ReDim Preserve frm_UdmCodTrasformato(UBound(frm_UdmCodTrasformato) + 1)
                                ReDim Preserve frm_UdmCod(UBound(frm_UdmCod) + 1)
                                ReDim Preserve frm_UdmSim(UBound(frm_UdmSim) + 1)
                                ReDim Preserve frm_Dose(UBound(frm_Dose) + 1)
                                ReDim Preserve frm_ForVegCod(UBound(frm_ForVegCod) + 1)

                            End If

                            'Destinazioni ''''''''''''''''''''''''''''

                            XMLs_Ricetta_Dettaglio_Destinazione = XML_Ricetta_Dettaglio.GetElementsByTagName("Ricetta_Destinazione")

                            If Not XMLs_Ricetta_Dettaglio_Destinazione Is Nothing AndAlso XMLs_Ricetta_Dettaglio_Destinazione.Count > 0 Then

                                For j = 0 To XMLs_Ricetta_Dettaglio_Destinazione.Count - 1

                                    XML_Ricetta_Dettaglio_Destinazione = XMLs_Ricetta_Dettaglio_Destinazione.Item(j)

                                    TrovatoImpianto = False

                                    If Not ArrayPiva Is Nothing Then
                                        For a = 0 To ArrayPiva.Length - 1
                                            If ArrayPiva(a) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("piva") And
                                               ArraySa_Cod(a) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("sa_cod") And
                                               ArrayAppezza(a) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("appezza") And
                                               ArrayId_Reg(a) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("id_reg") Then
                                                TrovatoImpianto = True
                                                Exit For
                                            End If
                                        Next
                                    End If

                                    If TrovatoImpianto = False Then
                                        If ArrayPiva Is Nothing Then
                                            ReDim Preserve ArrayPiva(0)
                                            ReDim Preserve ArraySa_Cod(0)
                                            ReDim Preserve ArrayAppezza(0)
                                            ReDim Preserve ArrayId_Reg(0)
                                        Else
                                            ReDim Preserve ArrayPiva(UBound(ArrayPiva) + 1)
                                            ReDim Preserve ArraySa_Cod(UBound(ArraySa_Cod) + 1)
                                            ReDim Preserve ArrayAppezza(UBound(ArrayAppezza) + 1)
                                            ReDim Preserve ArrayId_Reg(UBound(ArrayId_Reg) + 1)
                                        End If
                                        ArrayPiva(UBound(ArrayPiva)) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("piva")
                                        ArraySa_Cod(UBound(ArraySa_Cod)) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("sa_cod")
                                        ArrayAppezza(UBound(ArrayAppezza)) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("appezza")
                                        ArrayId_Reg(UBound(ArrayId_Reg)) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("id_reg")
                                    End If

                                Next

                            End If
                            'Destinazioni ''''''''''''''''''''''''''''


                            'Recupero i valori
                            frm_InsCod(UBound(frm_InsCod)) = CInt(XML_Ricetta_Dettaglio.GetAttribute("pro_cod"))
                            frm_ElemCod = CInt(XML_Ricetta_Dettaglio.GetAttribute("elem_cod"))

                            frm_ForVegCod(UBound(frm_ForVegCod)) = "0"
                            If XML_Ricetta_Dettaglio.HasAttribute("extra_str") AndAlso IsNumeric(XML_Ricetta_Dettaglio.GetAttribute("extra_str")) Then
                                frm_ForVegCod(UBound(frm_ForVegCod)) = XML_Ricetta_Dettaglio.GetAttribute("extra_str")
                            End If

                            Select Case frm_ElemCod
                                Case FORMULATI
                                    frm_InsDes(UBound(frm_InsDes)) = CStr(XML_Ricetta_Dettaglio.GetAttribute("fr_des"))

                                    'Dim o As New AgronicaCoreMetaSchemaDAL.FormulatixPrincipiAttivi_R
                                    'Dim principiAttivi As String = o.PrincipiAttivi_from_FrCod(CInt(XML_Ricetta_Dettaglio.GetAttribute("pro_cod")), True, "", "", objParametri_Server)
                                    'frm_PrincipiA(UBound(frm_PrincipiA)) = principiAttivi

                                Case TRAPPOLE
                                    frm_InsDes(UBound(frm_InsDes)) = CStr(XML_Ricetta_Dettaglio.GetAttribute("trap_des"))
                                    'frm_PrincipiA(UBound(frm_PrincipiA)) = ""

                                Case INSETTI
                                    frm_InsDes(UBound(frm_InsDes)) = CStr(XML_Ricetta_Dettaglio.GetAttribute("Ins_Des"))
                            End Select

                            frm_UdmCod(UBound(frm_UdmCod)) = CInt(XML_Ricetta_Dettaglio.GetAttribute("extra_int"))
                            frm_UdmCodTrasformato(UBound(frm_UdmCodTrasformato)) = CInt(XML_Ricetta_Dettaglio.GetAttribute("udm_cod"))
                            frm_UdmSim(UBound(frm_UdmSim)) = CStr(XML_Ricetta_Dettaglio.GetAttribute("udm_sim"))
                            frm_Dose(UBound(frm_Dose)) = CDbl(XML_Ricetta_Dettaglio.GetAttribute("qta"))

                        Case AgronicaCoreDataProvider.CostantiPersonalizzate.CAU_IMPUTAZIONE_PARCOMACCHINE

                            Dim objMAc As New AgronicaCoreContabDAL.Parco_Macchine_R
                            Dim dt_Mac As DataTable
                            strMacchina = ""
                            Mac_Des = ""

                            dt_Mac = objMAc.ParcoMacchine_Leggi("",
                                                                CInt(XML_Ricetta_Dettaglio.GetAttribute("mat_cod")),
                                                                False,
                                                                "", "", "", "", "", 0,
                                                                "", False, 0, "", True,
                                                                AGRODATAINIZIO,
                                                                AGRODATAFINE,
                                                                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                "", "",
                                                                objParametri_Server)
                            objMAc = Nothing

                            If dt_Mac.Rows.Count > 0 Then

                                Select Case Qs_Stampa_Tipo

                                    Case enum_TipoStampaRicetta.Certificazione

                                        If dt_Mac.Rows(0).Item("Modello") <> "" And dt_Mac.Rows(0).Item("Ultima_Manutenzione") <> "01/01/1900" Then
                                            strMacchina += dt_Mac.Rows(0).Item("CLASS_DESC") + " (Modello " + dt_Mac.Rows(0).Item("Modello")
                                            If Not bNascondiDataUltimaManutenzione Then
                                                strMacchina += "; Ultima Manutenzione " + dt_Mac.Rows(0).Item("Ultima_Manutenzione") + "),"
                                            Else
                                                strMacchina += "),"
                                            End If
                                        ElseIf dt_Mac.Rows(0).Item("Modello") <> "" And dt_Mac.Rows(0).Item("Ultima_Manutenzione") = "01/01/1900" Then
                                            strMacchina += dt_Mac.Rows(0).Item("CLASS_DESC") + " (Modello " + dt_Mac.Rows(0).Item("Modello") + "),"
                                        ElseIf dt_Mac.Rows(0).Item("Modello") = "" And dt_Mac.Rows(0).Item("Ultima_Manutenzione") <> "01/01/1900" Then
                                            strMacchina += dt_Mac.Rows(0).Item("CLASS_DESC")
                                            If Not bNascondiDataUltimaManutenzione Then
                                                strMacchina += "; Ultima Manutenzione " + dt_Mac.Rows(0).Item("Ultima_Manutenzione") + "),"
                                            Else
                                                strMacchina += "),"
                                            End If
                                        ElseIf dt_Mac.Rows(0).Item("Modello") = "" And dt_Mac.Rows(0).Item("Ultima_Manutenzione") = "01/01/1900" Then
                                            strMacchina += dt_Mac.Rows(0).Item("CLASS_DESC") + ","
                                        End If

                                        If InStr(strCostiMacchine, strMacchina) = 0 Then
                                            strCostiMacchine &= strMacchina
                                        End If

                                    Case enum_TipoStampaRicetta.Aziendale

                                        If dt_Mac.Rows(0).Item("mac_des") <> "" Then
                                            strMacchina = dt_Mac.Rows(0).Item("mac_des")
                                        Else
                                            strMacchina = dt_Mac.Rows(0).Item("CLASS_DESC")
                                        End If

                                        If dt_Mac.Rows(0).Item("ditta_des") <> "" Then
                                            strMacchina += " - " & dt_Mac.Rows(0).Item("ditta_des")
                                        End If

                                        If dt_Mac.Rows(0).Item("Modello") <> "" Then
                                            strMacchina += " - Modello " & dt_Mac.Rows(0).Item("Modello")
                                        End If

                                        If InStr(strCostiMacchine, strMacchina) = 0 Then
                                            strCostiMacchine &= strMacchina
                                        End If


                                        Dim Class_Code As String = dt_Mac.Rows(0).Item("CLASS_code")
                                        Dim strCaratteristiche As String = ""

                                        'CARATTERISTICHE MACCHINE
                                        Dim DtCar As DataTable
                                        Dim DrCar() As DataRow
                                        Dim objCar As New AgronicaCoreMetaSchemaDAL.MacchinexCaratteristiche_R
                                        DtCar = objCar.Leggi(0, Class_Code, "", "", objParametri_Server)


                                        Dim objProfilazioneR As New AgronicaCoreProfilazioneBIZ.Profilazione_R
                                        Dim DS_Macchine As DataSet
                                        DS_Macchine = objProfilazioneR.LeggiProfilazioneMacchine_In_Cascata(Piva,
                                                                                 Lav_Cod, Veg_Cod,
                                                                                 objParametri_Server)

                                        If Not DS_Macchine Is Nothing Then


                                            For Each dtDati As DataTable In DS_Macchine.Tables
                                                For Each drDati In dtDati.Rows
                                                    If (dtDati.TableName.Equals("macXlav")) Then
                                                        If drDati("mac_cod") = XML_Ricetta_Dettaglio.GetAttribute("mat_cod") Then
                                                            'leggo se il macchinario è profilato
                                                            Dim pmp As New AgronicaCoreProfilazioneDAL.Profilazione_Macchine_R
                                                            Dim dtMP As DataTable
                                                            dtMP = pmp.Leggi(CInt(drDati("Id_Profilo_Dati")),
                                                                             CInt(XML_Ricetta_Dettaglio.GetAttribute("mat_cod")), 0,
                                                                             "",
                                                                             "",
                                                                             objParametri_Server)

                                                            For m = 0 To dtMP.Rows.Count - 1
                                                                DrCar = DtCar.Select("mac_car_cod=" & dtMP.Rows(m).Item("mac_car_cod"))
                                                                If Not DrCar Is Nothing AndAlso DrCar.Length > 0 Then
                                                                    strCaratteristiche &= DrCar(0).Item("mac_car_des") & ":" & dtMP.Rows(m).Item("valore") & "; "
                                                                End If
                                                                'strCarCod &= dtMP.Rows(m).Item("mac_car_cod") & ","
                                                            Next
                                                        End If
                                                    End If
                                                Next
                                            Next
                                        End If

                                        If strMacchina <> "" Then
                                            strMacchina = Left(strMacchina, strMacchina.Length - 1)
                                        End If

                                        If strCaratteristiche <> "" Then
                                            strCaratteristiche = Left(strCaratteristiche, strCaratteristiche.Length - 2)
                                        End If

                                        listaMacCod.Add(XML_Ricetta_Dettaglio.GetAttribute("mat_cod") & "|" & strMacchina & "|" & strCaratteristiche)

                                End Select




                            End If


                        Case AgronicaCoreDataProvider.CostantiPersonalizzate.CAU_IMPUTAZIONE_TERZISTI,
                             AgronicaCoreDataProvider.CostantiPersonalizzate.CAU_IMPUTAZIONE_MANODOPERA

                            If XML_Ricetta_Dettaglio.GetAttribute("rag_soc") <> "" Then
                                nomeContatto = XML_Ricetta_Dettaglio.GetAttribute("rag_soc")
                            Else
                                nomeContatto = String.Format("{0} {1}", XML_Ricetta_Dettaglio.GetAttribute("cognome"), XML_Ricetta_Dettaglio.GetAttribute("nome"))
                            End If

                            If InStr(strCostiOperatori, nomeContatto) = 0 Then
                                strCostiOperatori &= nomeContatto & ","
                            End If

                        Case AgronicaCoreDataProvider.CostantiPersonalizzate.CAU_IMPUTAZIONE_TECNICO_RESPONSABILE

                            If XML_Ricetta_Dettaglio.GetAttribute("rag_soc") <> "" Then
                                nomeContatto = XML_Ricetta_Dettaglio.GetAttribute("rag_soc")
                            Else
                                nomeContatto = String.Format("{0} {1}", XML_Ricetta_Dettaglio.GetAttribute("cognome"), XML_Ricetta_Dettaglio.GetAttribute("nome"))
                            End If

                            If InStr(strCostiResponsabile, nomeContatto) = 0 Then
                                strCostiResponsabile = nomeContatto & ","
                            End If

                        Case CAU_SCARICO

                            SetDictionaryView(DictMagazziniEsterni, XML_Ricetta_Dettaglio, frm_InsCod)

                    End Select

                Next

                If Not bNascondiAutorizzatore And strCostiResponsabile <> String.Empty Then

                    strCosti = "Autorizzato da " & Left(strCostiResponsabile, strCostiResponsabile.Length - 1) & vbCrLf & vbCrLf

                End If

                If Not bNascondiOperatori And strCostiOperatori <> String.Empty Then

                    strCosti &= "A cura di : " & Left(strCostiOperatori, strCostiOperatori.Length - 1)

                Else

                    If Qs_Stampa_Tipo <> enum_TipoStampaRicetta.Aziendale Then
                        strCosti &= "Eseguito"
                    End If

                End If

                If Not bNascondiMacchine And strCostiMacchine <> String.Empty Then

                    strCosti &= " con : " & Left(strCostiMacchine, strCostiMacchine.Length - 1)

                Else

                    If InStr(strCosti, "Eseguito") <> 0 Then
                        Left(strCosti, strCosti.Length - 8)
                    End If

                End If

            End If

        End If


        'FINE SPACCHETTAMENTO

        '------------------------------------------
        '------------------------------------------

        'Recupero il datatable
        Dt = ViewState("dtDosi_Difesa")

        'creo il dt dei formulati dal vettore miscele
        'Scombinatore_Difesa(Miscele, Dt)

        Dim strAvversita As String
        Dim ArrayAvCodTmp() As String

        Dim drR As DS_Ricetta_Trattamenti.DS_Ricetta_TrattamentiRow

        Dim strCul_Des As String = ""

        Dim Mezzo As Integer
        Dim Extra_Int As Integer
        Dim Udm_Sim As String
        Dim Qta As Double
        Dim Qta_Tot As Double

        Dim N_app As String = ""
        Dim Num_Appezzamento As Integer
        Dim N_Appezza As Integer

        Dim Cop_Cod As Integer = 0

        Dim objApp As New AgronicaCoreAnagrafeDAL.Appezzamento_Read

        For i = 0 To UBound(frm_InsCod)


            If Qs_Stampa_Tipo = enum_TipoStampaRicetta.PianoLavori Then

                '----------------------------------------------------------------------------
                '------------------- TIPO STAMPA 3 --------------------------------
                '----------------------------------------------------------------------------
                Dim DrLav As DS_Ricetta3_Trattamenti.DT_Ricetta3_TrattamentiRow
                DrLav = DSTrattamenti3.DT_Ricetta3_Trattamenti.NewDT_Ricetta3_TrattamentiRow

                If i = UBound(frm_InsCod) Then
                    'valore fittizio per indicare ultima riga
                    'da usare per formula omissione linea su report
                    DrLav.PrimaRigaAppezza = 11
                Else
                    DrLav.PrimaRigaAppezza = 0
                End If
                DrLav.App_nome = ""
                DrLav.App_Nome_Breve = ""
                DrLav.Sa_Nome = ""
                DrLav.Campo_Nome = ""
                DrLav.Veg_Des = ""
                DrLav.Cul_Des = ""
                DrLav.Regolamento = ""
                DrLav.Sup_App = 0
                DrLav.Indirizzo = ""
                DrLav.Telefono = ""

                DrLav.Ricetta_Operazione_Cod = frm_OperazioneCod
                DrLav.Lav_Cod = Lav_Cod
                DrLav.Lav_Des = Lav_Des
                DrLav.Note = ""

                If i = 0 Then

                    'data operazione 
                    DrLav.Data = frm_Data

                    Dim ConImpianti As Boolean = False

                    'modifica rispetto alle altre stampe
                    DrLav.Destinazioni = ""
                    DrLav.Cul_Des = ""

                    'note
                    If frm_NoteIntervento <> "" Then
                        DrLav.Note = "NOTA: " & frm_NoteIntervento
                    End If

                    If frm_RicettaNote <> "" Then
                        If DrLav.Note <> "" Then
                            DrLav.Note &= vbCrLf & frm_RicettaNote
                        Else
                            DrLav.Note = frm_RicettaNote
                        End If
                    End If

                    If strCosti <> "" Then
                        If DrLav.Note <> "" Then
                            DrLav.Note &= vbCrLf & strCosti
                        Else
                            DrLav.Note = strCosti
                        End If
                    End If
                Else

                    DrLav.Lav_Des = ""
                    DrLav.Note = ""
                    DrLav.Qta_Ril = ""

                End If

                '-------------------------------------------------------
                'avversità

                strAvversita = ""
                Select Case Lav_Cod

                    Case LAVCOD_TRATTAMENTO_FITOREGOLATORE, LAVCOD_DISSECCAMENTO
                        strAvversita = ""

                    Case Else

                        ArrayAvCodTmp = Split(frm_AvCod(i), ",")
                        If Not ArrayAvCodTmp Is Nothing AndAlso ArrayAvCodTmp.Length > 0 Then
                            If ArrayAvCodTmp(0) = "0" Then
                                strAvversita = frm_AvGruDes(i)
                            Else
                                strAvversita = frm_AvDes(i)
                            End If
                        End If

                End Select

                DrLav.Av_Des = strAvversita

                '---------------------
                'PRODOTTo
                DrLav.Elem_Cod = FORMULATI
                DrLav.Pro_Cod = frm_InsCod(i)
                DrLav.Fr_Des = frm_InsDes(i)


                '----------------------
                'princpi attivi
                'DrLav.Pa_Des = frm_PrincipiA(i)

                '---------------------------------
                'Dose
                Qta_Tot = 0

                Extra_Int = frm_UdmCod(i)
                Mezzo = frm_Mezzo
                Qta = frm_Dose(i)

                Select Case frm_ElemCod

                    Case TRAPPOLE

                        Qta = Qta / Sup_Tot_Op
                        Udm_Sim = " n"

                        Qta = Math.Round(Qta, 3)
                        DrLav.Qta = Qta.ToString & Udm_Sim
                        DrLav.Qta_Tot = Math.Round(Qta * Sup_Tot_Op, 3) & Udm_Sim

                    Case Else

                        'converto in Kg o l
                        Select Case Extra_Int
                            Case 2  'kg
                                Udm_Sim = " kg"
                            Case 3  'g
                                Udm_Sim = " kg"
                                Qta = Qta / 1000
                            Case 29 'l
                                Udm_Sim = " lt"
                            Case 4  'q
                                Udm_Sim = " kg"
                                Qta = Qta * 100
                            Case 304  't
                                Udm_Sim = " kg"
                                Qta = Qta * 1000
                            Case 104  'cc
                                Udm_Sim = " lt"
                                Qta = Qta / 100
                            Case 101  'ml
                                Udm_Sim = " lt"
                                Qta = Qta / 1000
                        End Select

                        Qta = Math.Round(Qta, 3)
                        'es xxx kg
                        DrLav.Qta = Qta.ToString & Udm_Sim


                        Dim DoseProdotto As String = ""
                        Dim TotaleProdotto As String = ""

                        'se mezzo =0 allora la dose DrLav.Qta è per ettolitro, altrimenti per ettaro
                        Select Case Mezzo

                            Case 0  'la qtà è x ettolitro

                            Case 1  'la qtà è x ettaro

                                'DOSE aggiungo ha (es kg/ha)
                                DoseProdotto = DrLav.Qta & "/ha"

                                'DOSE totale prodotto
                                If Sup_Tot_Op <> 0 Then
                                    'Se sup >0 allora ho gli impianti,
                                    'posso calcolare la qta totale di prodotto
                                    TotaleProdotto = Math.Round(Qta * Sup_Tot_Op, 3) & Udm_Sim
                                Else
                                    'Se Sup è 0 allora sono senza impianti
                                    'non posso calcolare la qta totale, lascio la dose per ettaro e la udm
                                    TotaleProdotto = DoseProdotto
                                End If
                        End Select

                        DrLav.Qta = DoseProdotto
                        DrLav.Qta_Tot = TotaleProdotto
                        DrLav.Qta_Acqua = 0

                End Select


                AddMagazzinoEsternoNote(DrLav.Note, DictMagazziniEsterni, frm_InsCod(i))

                'aggiungo la nuova riga al dataset
                DSTrattamenti3.DT_Ricetta3_Trattamenti.Rows.Add(DrLav)

            Else
                '----------------------------------------------------------------------------
                '------------------- TIPO STAMPA 1 E 2 --------------------------------
                '----------------------------------------------------------------------------

                'creo una nuova riga del dataset
                drR = DsTrattamenti.DS_Ricetta_Trattamenti.NewDS_Ricetta_TrattamentiRow

                drR.Ricetta_Operazione_Cod = frm_OperazioneCod


                drR.Lav_Cod = Lav_Cod
                drR.Lav_Des = Lav_Des

                drR.Note = ""

                If i = 0 Then

                    'data operazione 
                    drR.Data = frm_Data


                    Dim ConImpianti As Boolean = False

                    If Not ArrayPiva Is Nothing Then

                        Dim ObjImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                        Dim Dt_Imp As DataTable
                        Dt_Imp = ObjImp.Leggi_Dati_Impianti(ArrayPiva,
                                                            ArraySa_Cod,
                                                            ArrayAppezza,
                                                            ArrayId_Reg, drR.Data, drR.Data,
                                                            "", "", objParametri_Server)
                        ObjImp = Nothing

                        For j = 0 To Dt_Imp.Rows.Count - 1

                            Cop_Cod = 0
                            If Not IsDBNull(Dt_Imp.Rows(j).Item("cop_cod")) Then
                                Cop_Cod = Dt_Imp.Rows(j).Item("cop_cod")
                            End If
                            'sup_tot
                            'Sup_Tot_Op += Dt_Imp.Rows(j).Item("sup_imp")
                            ConImpianti = True

                            'cul_des
                            If InStr(strCul_Des, Dt_Imp.Rows(j).Item("cul_des")) = 0 Then
                                strCul_Des &= Dt_Imp.Rows(j).Item("cul_des") & ", "
                            End If

                            'N.ro appezzamento
                            N_app = Dt_Imp.Rows(j).Item("app_nome_breve")

                            N_Appezza = Dt_Imp.Rows(j).Item("appezza") - BaseCode

                            N_app = objApp.Numero_Appezzamento(Dt_Imp.Rows(j).Item("piva"), Dt_Imp.Rows(j).Item("sa_cod"),
                                                       Dt_Imp.Rows(j).Item("campo_des"), Dt_Imp.Rows(j).Item("appezza"),
                                                       TipoCodiceAppezzamento,
                                                       Dt_Imp.Rows(j).Item("app_nome"), Dt_Imp.Rows(j).Item("app_nome_breve"),
                                                       N_Appezza, BaseCode,
                                                       Nothing, flag_nascondiCampo)

                            'If N_app = "" Then

                            '    Num_Appezzamento = New AgronicaCoreAnagrafeDAL.Appezzamento_Read().AppezzamentoNumero_from_AppezzamentoNome(Dt_Imp.Rows(j).Item("app_nome"))

                            '    If Num_Appezzamento <> 0 Then
                            '        N_app = CStr(Num_Appezzamento)
                            '    Else

                            '        If BaseCode = 0 Then
                            '            Dim ObjBase As New AgronicaCoreDataProvider.UtilityProvider
                            '            BaseCode = ObjBase.BaseCode_from_ProgressivoGias(Session("ASG_ProgressivoGIAS"))
                            '            ObjBase = Nothing
                            '        End If

                            '        N_app = Dt_Imp.Rows(i).Item("appezza") - BaseCode

                            '        'se è un valore alto lo tronco alle ultime 3 cifre..
                            '        If N_app.Length > 3 Then
                            '            N_app = Right(N_app, 3)
                            '        End If

                            '    End If

                            'End If

                            frm_Dest &= N_app & ", "

                        Next

                        If strCul_Des <> "" Then
                            strCul_Des = Left(strCul_Des, strCul_Des.Length - 2)
                        End If

                        If frm_Dest <> "" Then
                            frm_Dest = Left(frm_Dest, frm_Dest.Length - 2)
                        End If

                    End If

                    'destinazioni
                    drR.Destinazioni = frm_Dest

                    'cultivar
                    drR.Cul_Des = strCul_Des & " (" & Sup_Tot_Op.ToString & " Ha)"

                    'note
                    If frm_NoteIntervento <> "" Then
                        drR.Note = "NOTA: " & frm_NoteIntervento
                    End If

                    If frm_RicettaNote <> "" Then
                        If drR.Note <> "" Then
                            drR.Note &= vbCrLf & frm_RicettaNote
                        Else
                            drR.Note = frm_RicettaNote
                        End If
                    End If

                    If strCosti <> "" Then
                        If drR.Note <> "" Then
                            drR.Note &= vbCrLf & strCosti
                        Else
                            drR.Note = strCosti
                        End If
                    End If

                Else

                    drR.Lav_Des = ""
                    drR.Note = ""
                    drR.Qta_Ril = ""

                End If

                '-------------------------------------------------------
                'avversità

                strAvversita = ""
                Select Case Lav_Cod

                    Case LAVCOD_TRATTAMENTO_FITOREGOLATORE, LAVCOD_DISSECCAMENTO
                        strAvversita = ""

                    Case Else

                        ArrayAvCodTmp = Split(frm_AvCod(i), ",")
                        If Not ArrayAvCodTmp Is Nothing AndAlso ArrayAvCodTmp.Length > 0 Then
                            If ArrayAvCodTmp(0) = "0" Then
                                strAvversita = frm_AvGruDes(i)
                            Else
                                strAvversita = frm_AvDes(i)
                            End If
                        End If
                End Select

                drR.Av_Des = strAvversita

                '---------------------
                'PRODOTTo
                drR.Elem_Cod = FORMULATI
                drR.Pro_Cod = frm_InsCod(i)
                drR.Fr_Des = frm_InsDes(i)


                '----------------------
                'princpi attivi
                'drR.Pa_Des = frm_PrincipiA(i)

                '---------------------------------
                'Dose
                Qta_Tot = 0

                Extra_Int = frm_UdmCod(i)
                Mezzo = frm_Mezzo
                Qta = frm_Dose(i)

                Select Case frm_ElemCod

                    Case TRAPPOLE

                        Qta = Qta / Sup_Tot_Op
                        Udm_Sim = " n"

                        Qta = Math.Round(Qta, 3)
                        drR.Qta = Qta.ToString & Udm_Sim
                        drR.Qta_Tot = Math.Round(Qta * Sup_Tot_Op, 3) & Udm_Sim

                    Case Else

                        'converto in Kg o l
                        Select Case Extra_Int
                            Case enum_UnitaMisura.Numero  'kg
                                Udm_Sim = " n"
                        End Select

                        Qta = Math.Round(Qta, 3)
                        'es xxx kg
                        drR.Qta = Qta.ToString & Udm_Sim


                        Dim DoseProdotto As String = ""
                        Dim TotaleProdotto As String = ""

                        'se mezzo =0 allora la dose drR.Qta è per ettolitro, altrimenti per ettaro
                        Select Case Mezzo

                            Case 0  'la qtà è x ettolitro

                            Case 1  'la qtà è x ettaro

                                'DOSE aggiungo ha (es kg/ha)
                                DoseProdotto = drR.Qta & "/ha"

                                'DOSE totale prodotto
                                If Sup_Tot_Op <> 0 Then
                                    'Se sup >0 allora ho gli impianti,
                                    'posso calcolare la qta totale di prodotto
                                    TotaleProdotto = Math.Round(Qta * Sup_Tot_Op, 3) & Udm_Sim
                                Else
                                    'Se Sup è 0 allora sono senza impianti
                                    'non posso calcolare la qta totale, lascio la dose per ettaro e la udm
                                    TotaleProdotto = DoseProdotto
                                End If


                        End Select

                        drR.Qta = DoseProdotto
                        drR.Qta_Tot = TotaleProdotto
                        drR.Qta_Acqua = 0

                End Select



                Dim CoreAgroWS As New AgronicaCoreWebService.AgroWs
                Dim tempoCarenza As Integer = CoreAgroWS.TempoCarenza_from_FrCod_VegCod(frm_InsCod(i), CInt(Veg_Cod), frm_ForVegCod(i), 0, Cop_Cod, CDate(frm_Data), objParametri_Server, objParametri_Utenti)

                If tempoCarenza > 0 Then
                    drR.Carenza = tempoCarenza
                    drR.Data_Raccolta = CStr(DateAdd(DateInterval.Day, CInt(drR.Carenza) + 1, CDate(frm_Data)))

                Else
                    drR.Carenza = "--"
                    drR.Data_Raccolta = CStr(CDate(frm_Data))
                End If

                drR.Tempo_Rientro = "48 ore"

                Dim DtPrincipi As DataTable = CoreAgroWS.ComposizioneFormulatiRecupera(frm_InsCod(i), objParametri_Server, objParametri_Utenti)
                Dim principiAttivi As String = Replace(CoreAgroWS.ComposizioneFormulatiDescrizionePrincipiAttivi(objParametri_Server, frm_InsCod(i), DtPrincipi), "     ", "").Trim

                'Dim o As New AgronicaCoreMetaSchemaDAL.FormulatixPrincipiAttivi_R
                'Dim principiAttivi As String = o.PrincipiAttivi_from_FrCod(frm_FrCod(i), True, "", "", objParametri_Server)

                drR.Pa_Des = principiAttivi

                AddMagazzinoEsternoNote(drR.Note, DictMagazziniEsterni, frm_InsCod(i))

                'aggiungo la nuova riga al dataset
                DsTrattamenti.DS_Ricetta_Trattamenti.Rows.Add(drR)

            End If 'tipo stampa

        Next 'fr_cod

        'AGGIUNGO LE RIGHE MACCHINE nella stampa aziendale

        For m = 0 To listaMacCod.Count - 1

            'creo una nuova riga del dataset
            drR = DsTrattamenti.DS_Ricetta_Trattamenti.NewDS_Ricetta_TrattamentiRow

            drR.Elem_Cod = MACCHINE
            drR.Ricetta_Operazione_Cod = frm_OperazioneCod
            drR.Mac_Cod = Split(listaMacCod(m), "|")(0)
            drR.Mac_Des = Split(listaMacCod(m), "|")(1)
            drR.Mac_Car_Des = Split(listaMacCod(m), "|")(2)

            DsTrattamenti.DS_Ricetta_Trattamenti.Rows.Add(drR)

        Next

        'ripulisco il Dt di appoggio
        Dt.Rows.Clear()

        ViewState("dtDosi_Difesa") = Dt

        '--------------------------------

    End Sub


    '###########################################################################
    Private Sub Inserisci_Trappole(ByVal strOperazione As String,
                                   TipoCodiceAppezzamento As Integer, flag_nascondiCampo As Boolean, ByVal BaseCode As Integer)

        Dim Trappole_R As New AgronicaCoreMetaSchemaDAL.Trappole_R
        Dim Avversita_R As New AgronicaCoreMetaSchemaDAL.Avversita_R

        Dim Dt As DataTable

        Dim XmlDoc As New System.Xml.XmlDocument

        Dim XML_Ricetta_Operazione As System.Xml.XmlElement

        Dim XML_DatiRicetta_Dettagli_Tecnici As System.Xml.XmlElement
        Dim XML_Ricetta_Dettaglio_Tecnico As System.Xml.XmlElement
        Dim XMLs_Ricetta_Dettaglio_Tecnico As System.Xml.XmlNodeList

        Dim XML_DatiRicetta_Dettagli As System.Xml.XmlElement
        Dim XML_Ricetta_Dettaglio As System.Xml.XmlElement
        Dim XMLs_Ricetta_Dettaglio As System.Xml.XmlNodeList

        Dim XML_Ricetta_Dettaglio_Tecnico_2 As System.Xml.XmlElement
        Dim XMLs_Ricetta_Dettaglio_Tecnico_2 As System.Xml.XmlNodeList

        Dim Lav_Cod As String
        Dim Lav_Des As String

        Dim frm_OperazioneDes As String
        Dim frm_NoteIntervento As String

        Dim i, j As Integer

        Dim ArrayPiva() As String
        Dim ArraySa_Cod() As Integer
        Dim ArrayAppezza() As Integer
        Dim ArrayId_Reg() As Integer

        '''''' note '''''
        Dim frm_RicettaNote As String
        Dim XML_Ricetta_Note As System.Xml.XmlElement
        Dim XMLs_Ricetta_Note As System.Xml.XmlNodeList
        Dim XML_DatiRicetta_Note2 As System.Xml.XmlElement

        Dim frm_Data As String
        Dim frm_TrapCod As Integer
        Dim frm_NumTrappole As Integer
        Dim frm_AvCod As Integer
        Dim frm_DittaTrappola As Integer
        Dim frm_UsoTrappola As Integer
        Dim frm_OperazioneCod As Integer

        '------------------------------------------
        '----- Analizzo la stringa XML
        '------------------------------------------

        'Carico la stringa nel documento XML
        XmlDoc.LoadXml(strOperazione)

        '----- Tag Ricetta_Operazione

        XML_Ricetta_Operazione = XmlDoc.SelectSingleNode("Ricetta_Operazione")


        frm_OperazioneCod = CStr(XML_Ricetta_Operazione.GetAttribute("ricetta_operazione_cod"))

        frm_OperazioneDes = CStr(XML_Ricetta_Operazione.GetAttribute("ricetta_operazione_des"))

        Lav_Cod = CInt(XML_Ricetta_Operazione.GetAttribute("lav_cod"))
        Lav_Des = CStr(XML_Ricetta_Operazione.GetAttribute("lav_des"))

        frm_Data = CStr(XML_Ricetta_Operazione.GetAttribute("validita_inizio"))

        frm_NoteIntervento = CStr(XML_Ricetta_Operazione.GetAttribute("note"))

        If XML_Ricetta_Operazione.HasChildNodes Then

            '----- Tag XML_DatiRicetta_Dettagli

            XML_DatiRicetta_Dettagli = XML_Ricetta_Operazione.SelectSingleNode("DatiRicetta_Dettagli")

            If Not XML_DatiRicetta_Dettagli Is Nothing Then

                '----- Tag Ricetta_Dettaglio 

                'Recupero la collezione dei nodi
                XML_Ricetta_Dettaglio = XML_DatiRicetta_Dettagli.SelectSingleNode("Ricetta_Dettaglio")

                'codice del tipo di trappola
                frm_TrapCod = CInt(XML_Ricetta_Dettaglio.GetAttribute("pro_cod"))

                'numero delle trappole installate
                frm_NumTrappole = CInt(XML_Ricetta_Dettaglio.GetAttribute("qta"))

                XML_Ricetta_Dettaglio_Tecnico_2 = XML_Ricetta_Dettaglio.SelectSingleNode("Ricetta_Dettaglio_Tecnico_2")

                frm_AvCod = CInt(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("av_cod"))
                frm_DittaTrappola = CInt(XML_Ricetta_Dettaglio_Tecnico_2.GetAttribute("ditta_cod"))

                XMLs_Ricetta_Dettaglio = XML_DatiRicetta_Dettagli.GetElementsByTagName("Ricetta_Dettaglio")

                'cerco le destinazioni
                For i = 0 To XMLs_Ricetta_Dettaglio.Count - 1

                    'Prendo l'i-esimo nodo della collezione
                    XML_Ricetta_Dettaglio = XMLs_Ricetta_Dettaglio.Item(i)

                    Dim Cau_Mov As String = XML_Ricetta_Dettaglio.GetAttribute("cau_mov")

                    Select Case Cau_Mov

                        Case "", AgronicaCoreDataProvider.CostantiPersonalizzate.CAU_LAVORAZIONE, AgronicaCoreDataProvider.CostantiPersonalizzate.CAU_TRATTAMENTO

                            XML_Ricetta_Dettaglio_Tecnico_2 = XML_Ricetta_Dettaglio.SelectSingleNode("Ricetta_Dettaglio_Tecnico_2")

                            'Recupero i valori

                            'Destinazioni ''''''''''''''''''''''''''''
                            Dim XMLs_Ricetta_Dettaglio_Destinazione As System.Xml.XmlNodeList
                            XMLs_Ricetta_Dettaglio_Destinazione = XML_Ricetta_Dettaglio.GetElementsByTagName("Ricetta_Destinazione")

                            If Not XMLs_Ricetta_Dettaglio_Destinazione Is Nothing AndAlso XMLs_Ricetta_Dettaglio_Destinazione.Count > 0 Then
                                Dim TrovatoImpianto As Boolean = False
                                For j = 0 To XMLs_Ricetta_Dettaglio_Destinazione.Count - 1

                                    Dim XML_Ricetta_Dettaglio_Destinazione As System.Xml.XmlElement
                                    XML_Ricetta_Dettaglio_Destinazione = XMLs_Ricetta_Dettaglio_Destinazione.Item(j)

                                    TrovatoImpianto = False

                                    If Not ArrayPiva Is Nothing Then
                                        For a = 0 To ArrayPiva.Length - 1
                                            If ArrayPiva(a) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("piva") And
                                               ArraySa_Cod(a) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("sa_cod") And
                                               ArrayAppezza(a) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("appezza") And
                                               ArrayId_Reg(a) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("id_reg") Then
                                                TrovatoImpianto = True
                                                Exit For
                                            End If
                                        Next
                                    End If

                                    If TrovatoImpianto = False Then
                                        If ArrayPiva Is Nothing Then
                                            ReDim Preserve ArrayPiva(0)
                                            ReDim Preserve ArraySa_Cod(0)
                                            ReDim Preserve ArrayAppezza(0)
                                            ReDim Preserve ArrayId_Reg(0)
                                        Else
                                            ReDim Preserve ArrayPiva(UBound(ArrayPiva) + 1)
                                            ReDim Preserve ArraySa_Cod(UBound(ArraySa_Cod) + 1)
                                            ReDim Preserve ArrayAppezza(UBound(ArrayAppezza) + 1)
                                            ReDim Preserve ArrayId_Reg(UBound(ArrayId_Reg) + 1)
                                        End If
                                        ArrayPiva(UBound(ArrayPiva)) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("piva")
                                        ArraySa_Cod(UBound(ArraySa_Cod)) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("sa_cod")
                                        ArrayAppezza(UBound(ArrayAppezza)) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("appezza")
                                        ArrayId_Reg(UBound(ArrayId_Reg)) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("id_reg")
                                    End If

                                Next

                            End If
                            'Destinazioni ''''''''''''''''''''''''''''

                    End Select

                Next





            End If






            '''''''''''''''''''''''''NOTE'''''''''''''''''''
            '----- Tag DatiRicetta_Note2
            Dim objNoteIntervento As New AgronicaCoreContabDAL.Note_Intervento_R
            XML_DatiRicetta_Note2 = XML_Ricetta_Operazione.SelectSingleNode("DatiRicettaxNote_2")
            If Not XML_DatiRicetta_Note2 Is Nothing Then

                '----- Tag RicettaxNote_2 
                XMLs_Ricetta_Note = XML_DatiRicetta_Note2.GetElementsByTagName("RicettaxNote_2")

                If Not XMLs_Ricetta_Note Is Nothing AndAlso XMLs_Ricetta_Note.Count > 0 Then

                    For j = 0 To XMLs_Ricetta_Note.Count - 1

                        XML_DatiRicetta_Note2 = XMLs_Ricetta_Note.Item(j)
                        frm_RicettaNote &= objNoteIntervento.NotaDesFromNotaCod(
                                                XML_DatiRicetta_Note2.GetAttribute("nota_cod"),
                                                Session("ASG_objParametri_Server")) & ", "

                    Next

                    If frm_RicettaNote <> "" Then
                        frm_RicettaNote = Left(frm_RicettaNote, frm_RicettaNote.Length - 2)
                    End If
                End If

            End If
            '''''''''''''''''''''''''NOTE'''''''''''''''''''


        End If




        'FINE SPACCHETTAMENTO


        '------------------------------------------
        '----- Inserisco le righe nel dataset
        '------------------------------------------

        Dim drR As DS_Ricetta_Trappole.DT_Ricetta_TrappoleRow

        Dim Sup_Tot_Op As Double = 0
        Dim strCul_Des As String = ""
        Dim N_app As String = ""
        Dim Num_Appezzamento As Integer = 0
        Dim N_Appezza As Integer
        Dim frm_Dest As String = ""

        'creo una nuova riga del dataset
        drR = DsTrappole.DT_Ricetta_Trappole.NewDT_Ricetta_TrappoleRow

        drR.Note = frm_NoteIntervento
        If drR.Note <> "" Then
            drR.Note &= vbCrLf & frm_RicettaNote
        Else
            drR.Note = frm_RicettaNote
        End If

        If Qs_Stampa_Tipo = enum_TipoStampaRicetta.PianoLavori Then
            '!!!!!! non sono riuscita a debuggare le trappole perchè non si inseriscono trappole nella ricetta aziendale !!!!

            If i = Dt.Rows.Count - 1 Then
                'valore fittizio per indicare ultima riga
                'da usare per formula omissione linea su report
                drR.PrimaRigaAppezza = 11
            Else
                drR.PrimaRigaAppezza = 0
            End If
            drR.App_nome = ""
            drR.App_Nome_Breve = ""
            drR.Sa_Nome = ""
            drR.Campo_Nome = ""
            drR.Veg_Des = ""
            drR.Cul_Des = ""
            drR.Regolamento = ""
            drR.Sup_App = 0
            drR.Indirizzo = ""
            drR.Telefono = ""

        Else
            ' STAMPA 1 E 2
            Dim objApp As New AgronicaCoreAnagrafeDAL.Appezzamento_Read

            If Not ArrayPiva Is Nothing Then

                Dim ObjImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                Dim Dt_Imp As DataTable = ObjImp.Leggi_Dati_Impianti(ArrayPiva,
                                                    ArraySa_Cod,
                                                    ArrayAppezza,
                                                    ArrayId_Reg, frm_Data, frm_Data,
                                                    "", "", objParametri_Server)
                ObjImp = Nothing

                For j = 0 To Dt_Imp.Rows.Count - 1

                    'sup_tot
                    Sup_Tot_Op += Dt_Imp.Rows(j).Item("sup_imp")

                    'cul_des
                    If InStr(strCul_Des, Dt_Imp.Rows(j).Item("cul_des")) = 0 Then
                        strCul_Des &= Dt_Imp.Rows(j).Item("cul_des") & ", "
                    End If

                    'N.ro appezzamento
                    N_app = Dt_Imp.Rows(j).Item("app_nome_breve")

                    N_Appezza = Dt_Imp.Rows(j).Item("appezza") - BaseCode

                    N_app = objApp.Numero_Appezzamento(Dt_Imp.Rows(j).Item("piva"), Dt_Imp.Rows(j).Item("sa_cod"),
                                                       Dt_Imp.Rows(j).Item("campo_des"), Dt_Imp.Rows(j).Item("appezza"),
                                                       TipoCodiceAppezzamento,
                                                       Dt_Imp.Rows(j).Item("app_nome"), Dt_Imp.Rows(j).Item("app_nome_breve"),
                                                       N_Appezza, BaseCode,
                                                       Nothing, flag_nascondiCampo)

                    frm_Dest &= N_app & ", "

                Next

                If strCul_Des <> "" Then
                    strCul_Des = Left(strCul_Des, strCul_Des.Length - 2)
                End If

                If frm_Dest <> "" Then
                    frm_Dest = Left(frm_Dest, frm_Dest.Length - 2)
                End If

            End If

        End If 'stampa 3

        'destinazioni
        drR.Destinazioni = frm_Dest

        drR.Ricetta_Operazione_Cod = frm_OperazioneCod

        'cultivar
        drR.Cul_Des = strCul_Des & " (" & Sup_Tot_Op.ToString & " Ha)"

        drR.Lav_Cod = Lav_Cod
        drR.Lav_Des = Lav_Des 'LavDes_from_LavCod(Server, Session, Page, frm_LavCod)

        drR.Data = frm_Data

        drR.Pro_Cod = frm_TrapCod
        drR.Trap_Des = Trappole_R.TrapDes_from_TrapCod(frm_TrapCod, objParametri_Server)

        drR.Av_Cod = frm_AvCod
        drR.Av_Des = Avversita_R.AvDes_from_AvCod(CInt(frm_AvCod), Nothing, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta, "", "", objParametri_Server)

        drR.Ditta_Cod = frm_DittaTrappola
        Dim ditte As New AgronicaCoreMetaSchemaDAL.Ditte_R
        drR.Ditta_Des = ditte.DittaDes_from_DittaCod(frm_DittaTrappola, objParametri_Server)
        'drR.
        drR.Qta = frm_NumTrappole

        'aggiungo la nuova riga al dataset
        DsTrappole.DT_Ricetta_Trappole.Rows.Add(drR)

    End Sub


    ''###########################################################################
    ''carico il dataset x i dati relativi ai trattamenti
    'Private Sub CaricaDsTrattamenti(ByVal Ricetta_Cod As Integer, ByVal bArrotondaAcqua As Boolean, ByVal bNascondiMacchine As Boolean, ByVal bNascondiOperatori As Boolean, ByVal bNascondiAutorizzatore As Boolean)

    '    Dim CoreAgroWS As New AgronicaCoreWebService.AgroWs
    '    Dim drR As DS_Ricetta_Trattamenti.DS_Ricetta_TrattamentiRow

    '    Dim ObjTrattamenti As New AgronicaCoreStampeDAL.Ricette

    '    Dim Dt_Trattamenti As New DataTable
    '    Dim Dt_TrattamentixCulCod As DataTable
    '    Dim Dt_Trattamento As DataTable
    '    Dim Dt_Prodotto As DataTable
    '    Dim DrTrattamentoTmp() As DataRow
    '    Dim DrTrattamento() As DataRow
    '    Dim DrTrattamentixCulCod() As DataRow

    '    Dim i, j, p, c, t, a, b As Integer

    '    Dim Ricetta_Operazione_Cod As Integer
    '    Dim Cul_Cod As Integer

    '    Dim DrAcqua() As DataRow
    '    Dim Acqua As Double
    '    Dim Acqua_Tot As Double

    '    Dim DrPrincipiAttivi() As DataRow
    '    Dim strPrincipiAttivi As String

    '    Dim DrAvversita() As DataRow
    '    Dim strAvversita As String

    '    Dim DrProdotto() As DataRow
    '    Dim Pro_Cod As Integer
    '    Dim Disciplinare_Cod As Integer
    '    Dim Dettaglio_Cod As Integer

    '    Dim Dt_App As DataTable
    '    Dim InseritoApp As Boolean
    '    Dim App_Nome As String
    '    Dim Num_App As String = ""
    '    Dim N_App As Integer
    '    Dim strApp As String

    '    Dim Sup_Tot As Double
    '    Dim Sup_Tot_Op As Double

    '    Dim Mezzo As Integer
    '    Dim Extra_Int As Integer
    '    Dim Qta As Double
    '    Dim Qta_Tot As Double

    '    Dim strCosti As String
    '    Dim Dt_Costi As DataTable
    '    Dim strMacchina As String
    '    Dim strCostiMacchine As String
    '    Dim strCostiOperatori As String
    '    Dim strCostiResponsabile As String

    '    Dim strProCod As String = ""
    '    Dim strVegCod As String = ""

    '    Dt_Trattamenti = ObjTrattamenti.Leggi_Trattamenti(Ricetta_Cod, _
    '                                                      " Cul_Cod, Ricette_Operazioni.Validita_Inizio, Ricette_Operazioni.Ricetta_Operazione_Cod ", _
    '                                                      objParametri_Server)

    '    ObjTrattamenti = Nothing

    '    If Not Dt_Trattamenti Is Nothing AndAlso Dt_Trattamenti.Rows.Count > 0 Then

    '        Dim objSqlDis As New AgronicaCoreUtility.DatatableUtility

    '        'ottengo le varieta distinte
    '        Dim strCul_Cod() As String = objSqlDis.SelectDistinct(Dt_Trattamenti, "Cul_Cod")

    '        If Not strCul_Cod Is Nothing Then

    '            For c = 0 To strCul_Cod.Length - 1

    '                Cul_Cod = strCul_Cod(c)

    '                Dt_TrattamentixCulCod = New DataTable
    '                Dt_TrattamentixCulCod = Dt_Trattamenti.Clone

    '                'estraggo i dati della singola operazione
    '                DrTrattamentixCulCod = Dt_Trattamenti.Select("Cul_Cod=" & Cul_Cod.ToString)

    '                For t = 0 To DrTrattamentixCulCod.Length - 1
    '                    Dt_TrattamentixCulCod.ImportRow(DrTrattamentixCulCod(t))
    '                Next

    '                'ottengo le operazioni distinte
    '                Dim strRicetta_Operazione_Cod() As String = objSqlDis.SelectDistinct(Dt_TrattamentixCulCod, "Ricetta_Operazione_Cod")

    '                If Not strRicetta_Operazione_Cod Is Nothing Then

    '                    Dt_Trattamento = New DataTable
    '                    Dt_Trattamento = Dt_Trattamenti.Clone
    '                    Dt_Costi = New DataTable

    '                    Dt_App = New DataTable
    '                    Dt_App = Dt_Trattamenti.Clone
    '                    InseritoApp = False

    '                    For i = 0 To strRicetta_Operazione_Cod.Length - 1

    '                        Ricetta_Operazione_Cod = CInt(strRicetta_Operazione_Cod(i))

    '                        Dt_Trattamento.Clear()
    '                        Dt_Costi.Clear()

    '                        strApp = ""

    '                        'estraggo i dati della superficie TOTALE dell'operazione
    '                        Sup_Tot_Op = 0
    '                        DrTrattamentoTmp = Dt_Trattamenti.Select("Ricetta_Operazione_Cod=" & Ricetta_Operazione_Cod.ToString, "appezza")

    '                        Dim strAppezza() As String = {"appezza", "sup_imp"}
    '                        Dim lstAppezza() As ListItem = objSqlDis.SelectDistinct(DrTrattamentoTmp, strAppezza)

    '                        For a = 0 To lstAppezza.Length - 1
    '                            Sup_Tot_Op += CDbl(lstAppezza(a).Text)
    '                        Next

    '                        Sup_Tot_Op = Format(Sup_Tot_Op, "0.0000")

    '                        'estraggo i dati della singola operazione
    '                        DrTrattamento = Dt_TrattamentixCulCod.Select("Ricetta_Operazione_Cod=" & Ricetta_Operazione_Cod.ToString, "Pro_Cod")

    '                        If Not DrTrattamento Is Nothing AndAlso DrTrattamento.Length > 0 Then

    '                            For j = 0 To DrTrattamento.Length - 1
    '                                Dt_Trattamento.ImportRow(DrTrattamento(j))
    '                            Next

    '                            'ottengo i Pro_Cod distinti
    '                            Dim strPro_Cod() As String = objSqlDis.SelectDistinct(Dt_Trattamento, "Pro_Cod")

    '                            Dim ProCodCnt As Integer

    '                            For ProCodCnt = 0 To strPro_Cod.Length - 1
    '                                Dim str_app_P As String
    '                                str_app_P = strPro_Cod(i) & " "
    '                                If strProCod.Length = 0 Then
    '                                    strProCod = str_app_P
    '                                    strVegCod = Veg_Cod
    '                                Else
    '                                    If InStr(strProCod, str_app_P) = 0 Then
    '                                        'controllo che non ci sia già il procod 
    '                                        strProCod = strProCod & "," & str_app_P
    '                                        strVegCod = strVegCod & "," & Veg_Cod
    '                                    End If
    '                                End If
    '                            Next

    '                            For j = 0 To strPro_Cod.Length - 1

    '                                Pro_Cod = CInt(strPro_Cod(j))

    '                                Dt_Prodotto = New DataTable
    '                                Dt_Prodotto = Dt_Trattamenti.Clone

    '                                DrProdotto = Dt_Trattamento.Select("Pro_Cod=" & Pro_Cod.ToString)

    '                                If Not DrProdotto Is Nothing AndAlso DrProdotto.Length > 0 Then

    '                                    For p = 0 To DrProdotto.Length - 1
    '                                        Dt_Prodotto.ImportRow(DrProdotto(p))
    '                                    Next

    '                                    If Not Dt_Prodotto Is Nothing AndAlso Dt_Prodotto.Rows.Count > 0 Then

    '                                        'creo una nuova riga del dataset
    '                                        drR = DsTrattamenti.DS_Ricetta_Trattamenti.NewDS_Ricetta_TrattamentiRow

    '                                        drR.Cul_Cod = Cul_Cod
    '                                        drR.Cul_Des = Dt_Prodotto.Rows(0).Item("Cul_Des")

    '                                        drR.Lav_Cod = Dt_Prodotto.Rows(0).Item("Lav_Cod")

    '                                        'dati visibili sono nella prima riga
    '                                        If j = 0 Then

    '                                            drR.Lav_Des = DrProdotto(0).Item("Lav_Des")

    '                                            'data operazione 
    '                                            drR.Data = DrProdotto(0).Item("Validita_Inizio")

    '                                            drR.Note = DrProdotto(0).Item("Note")

    '                                            '-------------------------------------
    '                                            'Costi Accessori
    '                                            strCosti = ""
    '                                            strCostiMacchine = ""
    '                                            strCostiOperatori = ""
    '                                            strMacchina = ""

    '                                            Dim objCosti As New AgronicaCoreContabDAL.Ricette_Operazioni_R
    '                                            Dt_Costi = objCosti.CostiAccessori_from_Ricetta_Operazione_Cod(Ricetta_Cod, Ricetta_Operazione_Cod, "", objParametri_Server)
    '                                            objCosti = Nothing

    '                                            For a = 0 To Dt_Costi.Rows.Count - 1

    '                                                strMacchina = ""

    '                                                Select Case Dt_Costi.Rows(a).Item("elem_cod")

    '                                                    Case 1

    '                                                        'è un record macchinario
    '                                                        If Dt_Costi.Rows(a).Item("Modello") <> "" And Dt_Costi.Rows(a).Item("Ultima_Manutenzione") <> "01/01/1900" Then
    '                                                            strMacchina += Dt_Costi.Rows(a).Item("CLASS_DESC") + " (Modello " + Dt_Costi.Rows(a).Item("Modello") + "; Ultima Manutenzione " + Dt_Costi.Rows(a).Item("Ultima_Manutenzione") + "),"
    '                                                        ElseIf Dt_Costi.Rows(a).Item("Modello") <> "" And Dt_Costi.Rows(a).Item("Ultima_Manutenzione") = "01/01/1900" Then
    '                                                            strMacchina += Dt_Costi.Rows(a).Item("CLASS_DESC") + " (Modello " + Dt_Costi.Rows(a).Item("Modello") + "),"
    '                                                        ElseIf Dt_Costi.Rows(a).Item("Modello") = "" And Dt_Costi.Rows(a).Item("Ultima_Manutenzione") <> "01/01/1900" Then
    '                                                            strMacchina += Dt_Costi.Rows(a).Item("CLASS_DESC") + " (Ultima Manutenzione " + Dt_Costi.Rows(a).Item("Ultima_Manutenzione") + "),"
    '                                                        ElseIf Dt_Costi.Rows(a).Item("Modello") = "" And Dt_Costi.Rows(a).Item("Ultima_Manutenzione") = "01/01/1900" Then
    '                                                            strMacchina += Dt_Costi.Rows(a).Item("CLASS_DESC") + ","
    '                                                        End If

    '                                                        If InStr(strCostiMacchine, strMacchina) = 0 Then
    '                                                            strCostiMacchine &= strMacchina
    '                                                        End If

    '                                                    Case Else
    '                                                        If InStr(strCostiOperatori, Dt_Costi.Rows(a).Item("Rag_Soc")) = 0 Then
    '                                                            strCostiOperatori &= Dt_Costi.Rows(a).Item("Rag_Soc") & ","
    '                                                        End If
    '                                                End Select
    '                                            Next

    '                                            'If strCostiOperatori <> "" And strCostiMacchine <> "" Then

    '                                            '    strCosti = "A cura di : " & Left(strCostiOperatori, strCostiOperatori.Length - 1) & " con : " & Left(strCostiMacchine, strCostiMacchine.Length - 1)

    '                                            'ElseIf strCostiOperatori <> "" And strCostiMacchine = "" Then

    '                                            '    strCosti = "A cura di : " & Left(strCostiOperatori, strCostiOperatori.Length - 1)

    '                                            'ElseIf strCostiOperatori = "" And strCostiMacchine <> "" Then

    '                                            '    strCosti = "Eseguito con : " & Left(strCostiMacchine, strCostiMacchine.Length - 1)

    '                                            'End If

    '                                            If Not bNascondiAutorizzatore And strCostiResponsabile <> String.Empty Then

    '                                                strCosti = "Autorizzato da " & Left(strCostiResponsabile, strCostiResponsabile.Length - 1) & vbCrLf & vbCrLf

    '                                            End If

    '                                            If Not bNascondiOperatori And strCostiOperatori <> String.Empty Then

    '                                                strCosti &= "A cura di : " & Left(strCostiOperatori, strCostiOperatori.Length - 1)

    '                                            Else

    '                                                strCosti &= "Eseguito"

    '                                            End If

    '                                            If Not bNascondiMacchine And strCostiMacchine <> String.Empty Then

    '                                                strCosti &= " con : " & Left(strCostiMacchine, strCostiMacchine.Length - 1)

    '                                            Else

    '                                                If InStr(strCosti, "Eseguito") <> 0 Then
    '                                                    Left(strCosti, strCosti.Length - 8)
    '                                                End If

    '                                            End If


    '                                            If strCosti <> "" Then
    '                                                If drR.Note <> "" Then
    '                                                    drR.Note &= vbCrLf & strCosti
    '                                                Else
    '                                                    drR.Note = strCosti
    '                                                End If
    '                                            End If


    '                                            '------------------------------------
    '                                            'appezzamenti
    '                                            Dt_App.Clear()
    '                                            Sup_Tot = 0
    '                                            Qta_Tot = 0
    '                                            For b = 0 To Dt_Prodotto.Rows.Count - 1
    '                                                InseritoApp = False
    '                                                If b <> 0 Then
    '                                                    For a = 0 To Dt_App.Rows.Count - 1
    '                                                        If Dt_App.Rows(a).Item("Piva") = DrProdotto(b).Item("Piva") And _
    '                                                            Dt_App.Rows(a).Item("sa_cod") = DrProdotto(b).Item("sa_cod") And _
    '                                                            Dt_App.Rows(a).Item("appezza") = DrProdotto(b).Item("appezza") And _
    '                                                            Dt_App.Rows(a).Item("id_reg") = DrProdotto(b).Item("id_reg") Then
    '                                                            InseritoApp = True
    '                                                            Exit For
    '                                                        End If
    '                                                    Next
    '                                                End If
    '                                                If InseritoApp = False Then
    '                                                    Dt_App.ImportRow(Dt_Prodotto.Rows(b))
    '                                                    Sup_Tot += CDbl(Dt_Prodotto.Rows(b).Item("Sup_Imp"))
    '                                                End If
    '                                            Next

    '                                            Sup_Tot = Format(Sup_Tot, "0.0000")

    '                                            For a = 0 To Dt_App.Rows.Count - 1

    '                                                If Dt_App.Rows(a).Item("app_nome_breve") <> "" Then
    '                                                    Num_App = Dt_App.Rows(a).Item("app_nome_breve")
    '                                                Else
    '                                                    Dim objNum As New AgronicaCoreDataProvider.UtilityProvider
    '                                                    N_App = objNum.Numero_from_Stringa(Dt_App.Rows(a).Item("app_nome"))
    '                                                    objNum = Nothing
    '                                                    If N_App <> 0 Then
    '                                                        Num_App = CStr(N_App)
    '                                                    Else
    '                                                        Dim BaseCode As Integer = 0
    '                                                        Dim ObjBase As New AgronicaCoreDataProvider.UtilityProvider
    '                                                        BaseCode = ObjBase.BaseCode_from_ProgressivoGias(Session("ASG_ProgressivoGIAS"))
    '                                                        ObjBase = Nothing
    '                                                        Num_App = Dt_App.Rows(a).Item("appezza") - BaseCode
    '                                                        'se è un valore alto lo tronco alle ultime 3 cifre..
    '                                                        If CStr(Num_App).Length > 3 Then
    '                                                            Num_App = Right(CStr(Num_App), 3)
    '                                                        End If
    '                                                    End If
    '                                                End If

    '                                                strApp &= Num_App & ","

    '                                            Next

    '                                            If strApp <> "" Then
    '                                                strApp = Left(strApp, strApp.Length - 1)
    '                                            End If

    '                                            drR.Destinazioni = strApp


    '                                            '-------------------------------------
    '                                            'Acqua
    '                                            Acqua = 0
    '                                            Acqua_Tot = 0

    '                                            DrAcqua = Dt_Prodotto.Select("Qta_Ril<>0")

    '                                            If Not DrAcqua Is Nothing AndAlso DrAcqua.Length > 0 Then
    '                                                Acqua = DrAcqua(0).Item("Qta_Ril")
    '                                                If Acqua > 0 Then
    '                                                    Acqua_Tot = Acqua
    '                                                Else
    '                                                    Acqua_Tot = Format(Math.Abs(Acqua) * Sup_Tot_Op, "0.0000")
    '                                                End If

    '                                                'Select Case Acqua
    '                                                '    Case 0
    '                                                '        drR.Qta_Ril = ""
    '                                                '    Case Is < 0
    '                                                '        drR.Qta_Ril = Math.Abs(Acqua) & " [Hl/Ha]"
    '                                                '    Case Is > 0
    '                                                '        drR.Qta_Ril = Acqua & " [Hl]"
    '                                                'End Select
    '                                            End If




    '                                        End If

    '                                        '---------------------------------
    '                                        'Avversita
    '                                        strAvversita = ""

    '                                        Disciplinare_Cod = Dt_Prodotto.Rows(0).Item("Num_Protocollo")
    '                                        Dettaglio_Cod = Dt_Prodotto.Rows(0).Item("Ricetta_Dettaglio_Cod")

    '                                        If Disciplinare_Cod <> 0 Then

    '                                            DrAvversita = Dt_Prodotto.Select("Qta_Ril=0 AND Ricetta_Dettaglio_Cod_Tecnico=" & Dettaglio_Cod.ToString)

    '                                            If Not DrAvversita Is Nothing AndAlso DrAvversita.Length > 0 Then
    '                                                drR.Av_Cod = DrAvversita(0).Item("Av_Cod")
    '                                                drR.Av_Gru = DrAvversita(0).Item("Av_Gru")
    '                                                If drR.Av_Cod <> 0 Then
    '                                                    drR.Av_Des = DrAvversita(0).Item("Av_Des_Vol")
    '                                                Else
    '                                                    If drR.Av_Gru <> 0 Then
    '                                                        drR.Av_Des = DrAvversita(0).Item("Av_Gru_Des")
    '                                                    End If
    '                                                End If
    '                                            End If

    '                                        Else

    '                                            strAvversita = ""

    '                                            If j = 0 Then
    '                                                DrAvversita = Dt_Prodotto.Select("Qta_Ril=0")

    '                                                If Not DrAvversita Is Nothing AndAlso DrAvversita.Length > 0 Then
    '                                                    For a = 0 To DrAvversita.Length - 1
    '                                                        If DrAvversita(a).Item("Av_Cod") <> 0 Then
    '                                                            If InStr(strAvversita, DrAvversita(a).Item("Av_Des_Vol")) = 0 Then
    '                                                                strAvversita &= DrAvversita(a).Item("Av_Des_Vol") & ","
    '                                                            End If
    '                                                        Else
    '                                                            If DrAvversita(a).Item("Av_Gru") <> 0 Then
    '                                                                If InStr(strAvversita, DrAvversita(a).Item("Av_Gru_Des")) = 0 Then
    '                                                                    strAvversita &= DrAvversita(a).Item("Av_Gru_Des") & ","
    '                                                                End If
    '                                                            End If
    '                                                        End If
    '                                                    Next

    '                                                    If strAvversita <> "" Then
    '                                                        strAvversita = Left(strAvversita, strAvversita.Length - 1)
    '                                                    End If

    '                                                    drR.Av_Des = strAvversita

    '                                                End If

    '                                            End If

    '                                        End If

    '                                        '------------------------------------
    '                                        'prodotti
    '                                        drR.Elem_Cod = Dt_Prodotto.Rows(0).Item("Elem_Cod")
    '                                        drR.Pro_Cod = Dt_Prodotto.Rows(0).Item("Pro_Cod")

    '                                        Select Case drR.Elem_Cod

    '                                            Case FORMULATI
    '                                                drR.Fr_Des = Dt_Prodotto.Rows(0).Item("Fr_Des")
    '                                                'drR.Carenza = CoreAgroWS.TempoCarenza_from_FrCod_VegCod(drR.Pro_Cod, CInt(Veg_Cod), objParametri_Server, objParametri_Utenti)
    '                                                drR.Data_Raccolta = CStr(DateAdd(DateInterval.Day, CInt(drR.Carenza) + 1, CDate(DrProdotto(0).Item("Validita_Inizio"))))
    '                                                drR.Tempo_Rientro = "48 ore"
    '                                            Case TRAPPOLE
    '                                                drR.Fr_Des = Dt_Prodotto.Rows(0).Item("Trap_Des")

    '                                        End Select



    '                                        '---------------------------------
    '                                        'Principi Attivi
    '                                        strPrincipiAttivi = ""

    '                                        DrPrincipiAttivi = Dt_Prodotto.Select("", "pa_cod")

    '                                        'prendo l'elenco dei principi attivi distinti
    '                                        Dim strPA() As String = {"pa_cod", "pa_des"}
    '                                        Dim lstPrinAttivi() As ListItem = objSqlDis.SelectDistinct(DrPrincipiAttivi, strPA)

    '                                        'ciclo sui principi attivi....
    '                                        For a = 0 To lstPrinAttivi.Length - 2
    '                                            strPrincipiAttivi &= lstPrinAttivi(a).Text & ","
    '                                        Next

    '                                        'accodo alla stringa l'ultimo oggetto senza la virgola..
    '                                        strPrincipiAttivi &= lstPrinAttivi(lstPrinAttivi.Length - 1).Text

    '                                        drR.Pa_Des = strPrincipiAttivi


    '                                        '---------------------------------
    '                                        'Dose
    '                                        Qta_Tot = 0

    '                                        Extra_Int = Dt_Prodotto.Rows(0).Item("Extra_Int_Dett")
    '                                        Mezzo = Dt_Prodotto.Rows(0).Item("Mezzo")
    '                                        Qta = Dt_Prodotto.Rows(0).Item("qta")

    '                                        Select Case drR.Lav_Cod

    '                                            Case AgronicaCoreDataProvider.CostantiPersonalizzate.LAVCOD_DISORIENTAMENTO_SESSUALE, _
    '                                                 AgronicaCoreDataProvider.CostantiPersonalizzate.LAVCOD_CONFUSIONE_SESSUALE, _
    '                                                 AgronicaCoreDataProvider.CostantiPersonalizzate.LAVCOD_DISTRIBUZIONE_INSETTI

    '                                                Qta = Qta / Sup_Tot_Op

    '                                            Case Else

    '                                                Select Case Extra_Int
    '                                                    Case 2  'kg
    '                                                    Case 3  'g
    '                                                        Qta = Qta / 1000
    '                                                    Case 29 'l
    '                                                    Case 4  'q
    '                                                        Qta = Qta * 100
    '                                                    Case 304  't
    '                                                        Qta = Qta * 1000
    '                                                    Case 104  'cc
    '                                                        Qta = Qta / 100
    '                                                    Case 101  'ml
    '                                                        Qta = Qta / 1000
    '                                                End Select

    '                                                Select Case Mezzo
    '                                                    Case 0   'qtà x ettolitro
    '                                                        Qta = Qta * Acqua_Tot / Sup_Tot_Op
    '                                                    Case 1  'qtà x ettaro
    '                                                End Select

    '                                        End Select

    '                                        drR.Qta = Format(Qta, "0.0000")
    '                                        drR.Qta_Tot = Format(Qta * Sup_Tot, "0.0000")

    '                                        'aggiungo la nuova riga al dataset
    '                                        DsTrattamenti.DS_Ricetta_Trattamenti.Rows.Add(drR)


    '                                    End If

    '                                End If


    '                            Next

    '                        End If

    '                    Next

    '                End If

    '            Next

    '        End If

    '        If DsTrattamenti.DS_Ricetta_Trattamenti.Rows.Count > 0 Then

    '            'una volta che ho tutti i principi attivi faccio una chiamata al webservice unica in modo da avere tutte le
    '            'carenze
    '            Dim DT_Carenze As DataTable
    '            If strProCod.Length <> 0 Then
    '                DT_Carenze = CoreAgroWS.TempoCarenza_from_Multiple_FrCod_VegCod(strProCod, _
    '                                                                            strVegCod, _
    '                                                                            objParametri_Server, _
    '                                                                            objParametri_Utenti)

    '                If Not DT_Carenze Is Nothing AndAlso DT_Carenze.Rows.Count > 0 Then

    '                    Dim dr() As DataRow
    '                    'manca il filtro sul veg cod 
    '                    dr = DT_Carenze.Select("FR_Cod = '" & DsTrattamenti.DS_Ricetta_Trattamenti.Rows(i).Item("Pro_Cod").ToString() & "'")
    '                    If dr.Length > 0 Then

    '                        If IsNumeric(dr(0).Item("TempoCarenza")) AndAlso dr(0).Item("TempoCarenza") > 0 Then
    '                            DsTrattamenti.DS_Ricetta_Trattamenti.Rows(i).Item("Carenza") = dr(0).Item("TempoCarenza")
    '                        Else
    '                            DsTrattamenti.DS_Ricetta_Trattamenti.Rows(i).Item("Carenza") = 0
    '                        End If
    '                    Else
    '                        DsTrattamenti.DS_Ricetta_Trattamenti.Rows(i).Item("Carenza") = "0"

    '                    End If

    '                Else

    '                    DsTrattamenti.DS_Ricetta_Trattamenti.Rows(i).Item("Carenza") = "0"
    '                End If


    '            End If


    '        End If


    '        objSqlDis = Nothing
    '        Dt_Trattamenti = Nothing
    '        Dt_TrattamentixCulCod = Nothing
    '        Dt_Trattamento = Nothing
    '        Dt_Prodotto = Nothing
    '        Dt_App = Nothing
    '        Dt_Costi = Nothing

    '    End If




    'End Sub


    '###########################################################################
    Private Sub Inserisci_Lavorazione_Generica(ByVal strOperazione As String,
                                               ByVal TipoCodiceAppezzamento As Integer,
                                               ByVal flag_nascondiCampo As Boolean,
                                               ByVal BaseCode As Integer,
                                               Optional ByVal Data_inizio_ricetta As Date = AGRODATAINIZIO,
                                               Optional ByVal Data_fine_ricetta As Date = AGRODATAFINE)

        'Dim Dt As DataTable
        Dim XmlDoc As New System.Xml.XmlDocument

        Dim XML_Ricetta_Operazione As System.Xml.XmlElement

        'Dim XML_DatiRicetta_Dettagli_Tecnici As System.Xml.XmlElement
        'Dim XML_Ricetta_Dettaglio_Tecnico As System.Xml.XmlElement
        'Dim XMLs_Ricetta_Dettaglio_Tecnico As System.Xml.XmlNodeList

        Dim XML_DatiRicetta_Dettagli As System.Xml.XmlElement
        Dim XML_Ricetta_Dettaglio As System.Xml.XmlElement
        Dim XMLs_Ricetta_Dettaglio As System.Xml.XmlNodeList

        Dim XML_Ricetta_Dettaglio_Tecnico_2 As System.Xml.XmlElement
        'Dim XMLs_Ricetta_Dettaglio_Tecnico_2 As System.Xml.XmlNodeList

        Dim Lav_Cod As String
        Dim Lav_Des As String

        Dim frm_Data As String

        Dim frm_OperazioneCod As Integer
        Dim frm_OperazioneDes As String
        Dim frm_NoteIntervento As String

        Dim i, j, a As Integer
        Dim frm_Cau_Mov As String


        Dim frm_Dest As String = ""
        Dim XML_Ricetta_Dettaglio_Destinazione As System.Xml.XmlElement
        Dim XMLs_Ricetta_Dettaglio_Destinazione As System.Xml.XmlNodeList


        '''''' note '''''
        Dim frm_RicettaNote As String = ""
        'Dim XML_Ricetta_Note As System.Xml.XmlElement
        Dim XMLs_Ricetta_Note As System.Xml.XmlNodeList
        Dim XML_DatiRicetta_Note2 As System.Xml.XmlElement

        Dim TrovatoImpianto As Boolean
        Dim ArrayPiva() As String
        Dim ArraySa_Cod() As Integer
        Dim ArrayAppezza() As Integer
        Dim ArrayId_Reg() As Integer

        '------------------------------------------
        '----- Analizzo la stringa XML
        '------------------------------------------

        'Carico la stringa nel documento XML
        XmlDoc.LoadXml(strOperazione)

        '----- Tag Ricetta_Operazione

        XML_Ricetta_Operazione = XmlDoc.SelectSingleNode("Ricetta_Operazione")

        frm_OperazioneDes = CStr(XML_Ricetta_Operazione.GetAttribute("ricetta_operazione_des"))
        frm_OperazioneCod = CStr(XML_Ricetta_Operazione.GetAttribute("ricetta_operazione_cod"))

        Lav_Cod = CInt(XML_Ricetta_Operazione.GetAttribute("lav_cod"))
        Lav_Des = CStr(XML_Ricetta_Operazione.GetAttribute("lav_des"))

        frm_Data = CStr(XML_Ricetta_Operazione.GetAttribute("validita_inizio"))

        frm_NoteIntervento = CStr(XML_Ricetta_Operazione.GetAttribute("note"))

        If XML_Ricetta_Operazione.HasChildNodes Then

            '----- Tag XML_DatiRicetta_Dettagli

            XML_DatiRicetta_Dettagli = XML_Ricetta_Operazione.SelectSingleNode("DatiRicetta_Dettagli")

            If Qs_Stampa_Tipo <> enum_TipoStampaRicetta.PianoLavori Then

                If Not XML_DatiRicetta_Dettagli Is Nothing Then

                    '----- Tag Ricetta_Dettaglio 

                    'Recupero la collezione dei nodi
                    XMLs_Ricetta_Dettaglio = XML_DatiRicetta_Dettagli.GetElementsByTagName("Ricetta_Dettaglio")
                    'XML_Ricetta_Dettaglio = XML_DatiRicetta_Dettagli.SelectSingleNode("Ricetta_Dettaglio")

                    For i = 0 To XMLs_Ricetta_Dettaglio.Count - 1

                        'Prendo l'i-esimo nodo della collezione
                        XML_Ricetta_Dettaglio = XMLs_Ricetta_Dettaglio.Item(i)

                        frm_Cau_Mov = XML_Ricetta_Dettaglio.GetAttribute("cau_mov")

                        Select Case frm_Cau_Mov

                            Case "", AgronicaCoreDataProvider.CostantiPersonalizzate.CAU_LAVORAZIONE

                                XML_Ricetta_Dettaglio_Tecnico_2 = XML_Ricetta_Dettaglio.SelectSingleNode("Ricetta_Dettaglio_Tecnico_2")

                                'Recupero i valori

                                'Destinazioni ''''''''''''''''''''''''''''
                                XMLs_Ricetta_Dettaglio_Destinazione = XML_Ricetta_Dettaglio.GetElementsByTagName("Ricetta_Destinazione")

                                If Not XMLs_Ricetta_Dettaglio_Destinazione Is Nothing AndAlso XMLs_Ricetta_Dettaglio_Destinazione.Count > 0 Then

                                    For j = 0 To XMLs_Ricetta_Dettaglio_Destinazione.Count - 1

                                        XML_Ricetta_Dettaglio_Destinazione = XMLs_Ricetta_Dettaglio_Destinazione.Item(j)

                                        TrovatoImpianto = False

                                        If Not ArrayPiva Is Nothing Then
                                            For a = 0 To ArrayPiva.Length - 1
                                                If ArrayPiva(a) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("piva") And
                                                   ArraySa_Cod(a) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("sa_cod") And
                                                   ArrayAppezza(a) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("appezza") And
                                                   ArrayId_Reg(a) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("id_reg") Then
                                                    TrovatoImpianto = True
                                                    Exit For
                                                End If
                                            Next
                                        End If

                                        If TrovatoImpianto = False Then
                                            If ArrayPiva Is Nothing Then
                                                ReDim Preserve ArrayPiva(0)
                                                ReDim Preserve ArraySa_Cod(0)
                                                ReDim Preserve ArrayAppezza(0)
                                                ReDim Preserve ArrayId_Reg(0)
                                            Else
                                                ReDim Preserve ArrayPiva(UBound(ArrayPiva) + 1)
                                                ReDim Preserve ArraySa_Cod(UBound(ArraySa_Cod) + 1)
                                                ReDim Preserve ArrayAppezza(UBound(ArrayAppezza) + 1)
                                                ReDim Preserve ArrayId_Reg(UBound(ArrayId_Reg) + 1)
                                            End If
                                            ArrayPiva(UBound(ArrayPiva)) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("piva")
                                            ArraySa_Cod(UBound(ArraySa_Cod)) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("sa_cod")
                                            ArrayAppezza(UBound(ArrayAppezza)) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("appezza")
                                            ArrayId_Reg(UBound(ArrayId_Reg)) = XML_Ricetta_Dettaglio_Destinazione.GetAttribute("id_reg")
                                        End If

                                    Next

                                End If                            'Destinazioni 

                        End Select

                    Next

                End If 'dettagli

            End If 'tipo stampa


            '''''''''''''''''''''''''NOTE'''''''''''''''''''
            '----- Tag DatiRicetta_Note2
            Dim objNoteIntervento As New AgronicaCoreContabDAL.Note_Intervento_R
            XML_DatiRicetta_Note2 = XML_Ricetta_Operazione.SelectSingleNode("DatiRicettaxNote_2")
            If Not XML_DatiRicetta_Note2 Is Nothing Then

                '----- Tag RicettaxNote_2 
                XMLs_Ricetta_Note = XML_DatiRicetta_Note2.GetElementsByTagName("RicettaxNote_2")

                If Not XMLs_Ricetta_Note Is Nothing AndAlso XMLs_Ricetta_Note.Count > 0 Then

                    For j = 0 To XMLs_Ricetta_Note.Count - 1

                        XML_DatiRicetta_Note2 = XMLs_Ricetta_Note.Item(j)
                        frm_RicettaNote &= objNoteIntervento.NotaDesFromNotaCod(
                                                XML_DatiRicetta_Note2.GetAttribute("nota_cod"),
                                                Session("ASG_objParametri_Server")) & ", "

                    Next

                    If frm_RicettaNote <> "" Then
                        frm_RicettaNote = Left(frm_RicettaNote, frm_RicettaNote.Length - 2)
                    End If
                End If

            End If
            '''''''''''''''''''''''''NOTE'''''''''''''''''''


        End If 'ricetta operazione

        'FINE SPACCHETTAMENTO


        '------------------------------------------
        '----- Inserisco le righe nel dataset
        '------------------------------------------

        Dim Sup_Tot_Op As Double = 0
        Dim strCul_Des As String = ""
        Dim Dt_Imp As DataTable
        Dim N_app As String = ""
        'Dim Num_Appezzamento As Integer
        Dim N_Appezza As Integer
        Dim objApp As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
        Dim drR As DS_Ricetta_Lavorazioni.DS_Ricetta_LavorazioniRow
        Dim Indirizzo As String
        Dim rubrica As String
        Dim str_indirizzoRubrica As String = ""


        If Qs_Stampa_Tipo = enum_TipoStampaRicetta.PianoLavori Then
            Dim DrLav As DS_Ricetta3_Lavorazioni.DT_Ricetta3_LavorazioniRow
            DrLav = DSLavorazioni3.DT_Ricetta3_Lavorazioni.NewDT_Ricetta3_LavorazioniRow

            DrLav.Ricetta_Operazione_Cod = frm_OperazioneCod
            DrLav.Data = frm_Data
            DrLav.Lav_Cod = Lav_Cod
            DrLav.Lav_Des = Lav_Des
            DrLav.Note = frm_NoteIntervento
            If DrLav.Note <> "" Then
                DrLav.Note &= vbCrLf & frm_RicettaNote
            Else
                DrLav.Note = frm_RicettaNote
            End If

            DrLav.Destinazioni = ""
            DrLav.Cul_Des = ""

            DSLavorazioni3.DT_Ricetta3_Lavorazioni.Rows.Add(DrLav)

        Else
            'REPORT TIPO 1 E 2

            'creo una nuova riga del dataset
            drR = DsLavorazioni._DS_Ricetta_Lavorazioni.NewDS_Ricetta_LavorazioniRow

            drR.Ricetta_Operazione_Cod = frm_OperazioneCod

            drR.Data = frm_Data

            drR.Lav_Cod = Lav_Cod
            drR.Lav_Des = Lav_Des

            drR.Note = frm_NoteIntervento
            If drR.Note <> "" Then
                drR.Note &= vbCrLf & frm_RicettaNote
            Else
                drR.Note = frm_RicettaNote
            End If


            If Not ArrayPiva Is Nothing Then

                Dim ObjImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                Dt_Imp = ObjImp.Leggi_Dati_Impianti(ArrayPiva,
                                                    ArraySa_Cod,
                                                    ArrayAppezza,
                                                    ArrayId_Reg, drR.Data, drR.Data,
                                                    "", "", objParametri_Server)

                ObjImp = Nothing

                For j = 0 To Dt_Imp.Rows.Count - 1

                    'sup_tot
                    Sup_Tot_Op += Dt_Imp.Rows(j).Item("sup_imp")

                    'cul_des
                    If InStr(strCul_Des, Dt_Imp.Rows(j).Item("cul_des")) = 0 Then
                        'nuova varietà
                        strCul_Des &= Dt_Imp.Rows(j).Item("cul_des") & ", "
                    Else
                        'varietà esistente
                    End If

                    'Indirizzo = Dt_Imp.Rows(j).Item("Appezza_Indirizzo")
                    'rubrica = Dt_Imp.Rows(j).Item("Appezza_Rubrica")
                    'If InStr(str_indirizzoRubrica, Indirizzo) = 0 Then
                    '    str_indirizzoRubrica &= Indirizzo
                    '    If rubrica <> "" Then
                    '        str_indirizzoRubrica &= " - " & rubrica
                    '    End If
                    '    str_indirizzoRubrica = LTrim(str_indirizzoRubrica)
                    '    str_indirizzoRubrica = RTrim(str_indirizzoRubrica)
                    'End If

                    'N.ro appezzamento
                    N_app = Dt_Imp.Rows(j).Item("app_nome_breve")

                    N_Appezza = Dt_Imp.Rows(j).Item("appezza") - BaseCode

                    N_app = objApp.Numero_Appezzamento(Dt_Imp.Rows(j).Item("piva"), Dt_Imp.Rows(j).Item("sa_cod"),
                                                       Dt_Imp.Rows(j).Item("campo_des"), Dt_Imp.Rows(j).Item("appezza"),
                                                       TipoCodiceAppezzamento,
                                                       Dt_Imp.Rows(j).Item("app_nome"), Dt_Imp.Rows(j).Item("app_nome_breve"),
                                                       N_Appezza, BaseCode,
                                                       Nothing, flag_nascondiCampo)
                    'If N_app = "" Then

                    '    Num_Appezzamento = New AgronicaCoreAnagrafeDAL.Appezzamento_Read().AppezzamentoNumero_from_AppezzamentoNome(Dt_Imp.Rows(j).Item("app_nome"))

                    '    If Num_Appezzamento <> 0 Then
                    '        N_app = CStr(Num_Appezzamento)
                    '    Else

                    '        If BaseCode = 0 Then
                    '            Dim ObjBase As New AgronicaCoreDataProvider.UtilityProvider
                    '            BaseCode = ObjBase.BaseCode_from_ProgressivoGias(Session("ASG_ProgressivoGIAS"))
                    '            ObjBase = Nothing
                    '        End If

                    '        N_app = Dt_Imp.Rows(i).Item("appezza") - BaseCode

                    '        'se è un valore alto lo tronco alle ultime 3 cifre..
                    '        If N_app.Length > 3 Then
                    '            N_app = Right(N_app, 3)
                    '        End If

                    '    End If

                    'End If

                    frm_Dest &= N_app & ", "

                Next

                If strCul_Des <> "" Then
                    strCul_Des = Left(strCul_Des, strCul_Des.Length - 2)
                End If

                If frm_Dest <> "" Then
                    frm_Dest = Left(frm_Dest, frm_Dest.Length - 2)
                End If

            End If

            'destinazioni
            drR.Destinazioni = frm_Dest

            'cultivar
            drR.Cul_Des = strCul_Des & " (" & Sup_Tot_Op.ToString & " Ha)"

            'aggiungo la nuova riga al dataset
            DsLavorazioni._DS_Ricetta_Lavorazioni.Rows.Add(drR)

        End If

    End Sub


    Private Function GeneraStrutturaDTNPK() As DataTable
        Dim Dt As New DataTable

        '----- Definisco la struttura del DataTable
        Dt.Columns.Add(New DataColumn("piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("sa_cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("campo_cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("appezza", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("id_reg", GetType(Integer)))

        '  Marco Grilli, 12/04/2016 09:40:28: Aggiunti per Piano Distribuzione
        Dt.Columns.Add(New DataColumn("N_Max", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("P_Max", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("K_Max", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("N_Distribuito", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("P_Distribuito", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("K_Distribuito", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("N_Residuo", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("P_Residuo", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("K_Residuo", GetType(Decimal)))

        Return Dt

    End Function

    Private Sub MergeNoteAppezzamenti(ByRef tabella As DataTable, ByVal traduttore As Traduzione_Stampa_Ricette)

        'Faccio Merge dei dati delle colonne destinazioni e note
        If Not tabella Is Nothing AndAlso tabella.Rows.Count > 0 Then

            For Each t As DataRow In tabella.Rows
                Dim destinazioni As String = If(t.Item("destinazioni") Is DBNull.Value, "", t.Item("destinazioni").ToString())
                Dim note As String = If(t.Item("note") Is DBNull.Value, "", t.Item("note").ToString())

                Dim sb As New StringBuilder
                sb.AppendFormat("{0} {1}", traduttore.Traduzioni.Appezzamenti, destinazioni.Replace("App.", ""))
                sb.Append(Environment.NewLine)
                sb.AppendFormat(note)

                t.Item("note") = sb.ToString()

            Next
        End If

    End Sub

    Private Sub PersonalizzazioniConserve(ByRef logoFranceseVisible As Boolean,
                                          ByRef logoItalianoVisible As Boolean)

        Dim nascondiPersonalizzazioni As Boolean = True
        logoItalianoVisible = False
        logoFranceseVisible = False
        nascondiPersonalizzazioni = ApplicaPersonalizzazioniCOnserve("F1999999987")
        If Not nascondiPersonalizzazioni Then
            logoFranceseVisible = True
            Return
        End If

        nascondiPersonalizzazioni = ApplicaPersonalizzazioniCOnserve("00708311204")
        If Not nascondiPersonalizzazioni Then
            logoItalianoVisible = True
        End If
    End Sub

    Private Function ApplicaPersonalizzazioniCOnserve(ByVal pivaRadice As String) As Boolean

        ' Sezione LOgo e Footer per Conserve France
        Dim nascondiPersonalizzazioni As Boolean = True
        Dim giDal As New GerarchiaImprese_R
        Dim elencoImprese As String = String.Empty
        giDal.LeggiFigliNodoGerarchiaImprese(pivaRadice, elencoImprese, objParametri_Server)
        If Not String.IsNullOrEmpty(elencoImprese) Then
            Dim impreseFiglie As New List(Of String)
            impreseFiglie = elencoImprese.Split(",").ToList()
            For i As Integer = 0 To impreseFiglie.Count - 1
                impreseFiglie(i) = impreseFiglie(i).Replace("'", "").Trim
            Next

            If impreseFiglie.Contains(Piva) Then
                nascondiPersonalizzazioni = False
            End If
        End If

        Return nascondiPersonalizzazioni

    End Function

    Private Sub ComponiRagioneSociale(ByVal dtDati As DataTable, ByVal piva As String, ByRef ragioneSociale As String)

        ragioneSociale = dtDati.AsEnumerable.FirstOrDefault().Item("Rag_Soc").ToString()
        ragioneSociale = ragioneSociale & " ( " + piva + " )"

    End Sub

    Private Sub ComponiIndirizzo(ByVal dtDati As DataTable, ByRef indirizzzo As String)

        Dim indOperativo = dtDati.AsEnumerable.Where(Function(i) i.Item("tipo_Indirizzo") = 1).FirstOrDefault()
        Dim indLegale = dtDati.AsEnumerable.Where(Function(i) i.Item("tipo_Indirizzo") = 101).FirstOrDefault()

        Dim rigaIndirizzo = If(Not indLegale Is Nothing, indLegale, indOperativo)
        If Not rigaIndirizzo Is Nothing Then
            indirizzzo = If(rigaIndirizzo.Item("Ind_Des") Is DBNull.Value, "", rigaIndirizzo.Item("Ind_Des").ToString())
            indirizzzo = indirizzzo & If(rigaIndirizzo.Item("Cap") Is DBNull.Value, " ", " " & rigaIndirizzo.Item("Cap") & " ")
            indirizzzo = indirizzzo & If(rigaIndirizzo.Item("Stato") Is DBNull.Value, " ", " " & rigaIndirizzo.Item("Stato") & " ")
        End If

    End Sub

    Private Sub SetDictionaryView(ByRef DictMagazziniEsterni As Dictionary(Of (Integer, String, String), String), ByVal XML_Ricetta_Dettaglio As System.Xml.XmlElement, ByVal frm_ProdottoCod() As Integer)

        Dim XMLs_Ricetta_Destinazione_Magazzino As System.Xml.XmlNodeList = XML_Ricetta_Dettaglio.GetElementsByTagName("Ricetta_Destinazione")

        If Not XMLs_Ricetta_Destinazione_Magazzino Is Nothing AndAlso XMLs_Ricetta_Destinazione_Magazzino.Count > 0 Then

            For j = 0 To XMLs_Ricetta_Destinazione_Magazzino.Count - 1
                Dim XML_Ricetta_Destinazione_Magazzino As System.Xml.XmlElement = XMLs_Ricetta_Destinazione_Magazzino.Item(j)

                Dim magazzinoesterno_cod As String = XML_Ricetta_Destinazione_Magazzino.GetAttribute("magazzinoesterno_cod")

                Dim magazzinoesterno_des As String = XML_Ricetta_Destinazione_Magazzino.GetAttribute("magazzinoesterno_des")

                If Not String.IsNullOrEmpty(magazzinoesterno_cod) AndAlso Not String.IsNullOrEmpty(magazzinoesterno_des) Then

                    Dim magazzino_esterno_key = magazzinoesterno_cod.Split("-")

                    If Not IsNothing(magazzino_esterno_key) AndAlso magazzino_esterno_key.Count > 3 AndAlso magazzino_esterno_key(3) = enum_MagazzinoEsterno_Tipo.Uso_da_Terzi Then

                        Dim Elem_Cod As Integer = XML_Ricetta_Dettaglio.GetAttribute("elem_cod")

                        Dim Lotto As String = XML_Ricetta_Dettaglio.GetAttribute("lotto")

                        Select Case Elem_Cod
                            Case FORMULATI

                                Dim Fr_Cod As Integer = XML_Ricetta_Dettaglio.GetAttribute("pro_cod")

                                If Not IsNothing(frm_ProdottoCod) AndAlso frm_ProdottoCod.ToList().Contains(Fr_Cod) AndAlso Not DictMagazziniEsterni.ContainsKey((Fr_Cod, magazzinoesterno_cod, Lotto)) Then
                                    DictMagazziniEsterni.Add((Fr_Cod, magazzinoesterno_cod, Lotto), magazzinoesterno_des)
                                End If

                            Case FERTILIZZANTI
                                Dim Fer_Cod As Integer = XML_Ricetta_Dettaglio.GetAttribute("pro_cod")

                                If Not IsNothing(frm_ProdottoCod) AndAlso frm_ProdottoCod.ToList().Contains(Fer_Cod) AndAlso Not DictMagazziniEsterni.ContainsKey((Fer_Cod, magazzinoesterno_cod, Lotto)) Then
                                    DictMagazziniEsterni.Add((Fer_Cod, magazzinoesterno_cod, Lotto), magazzinoesterno_des)
                                End If
                            Case TRASFORMATI_VEGETALI
                            Case SEMENTI
                                Dim Mat_Cod As Integer = XML_Ricetta_Dettaglio.GetAttribute("mat_cod")

                                If Not IsNothing(frm_ProdottoCod) AndAlso frm_ProdottoCod.ToList().Contains(Mat_Cod) AndAlso Not DictMagazziniEsterni.ContainsKey((Mat_Cod, magazzinoesterno_cod, Lotto)) Then
                                    DictMagazziniEsterni.Add((Mat_Cod, magazzinoesterno_cod, Lotto), magazzinoesterno_des)
                                End If
                            Case INSETTI
                                Dim Ins_Cod As Integer = XML_Ricetta_Dettaglio.GetAttribute("pro_cod")

                                If Not IsNothing(frm_ProdottoCod) AndAlso frm_ProdottoCod.ToList().Contains(Ins_Cod) AndAlso Not DictMagazziniEsterni.ContainsKey((Ins_Cod, magazzinoesterno_cod, Lotto)) Then
                                    DictMagazziniEsterni.Add((Ins_Cod, magazzinoesterno_cod, Lotto), magazzinoesterno_des)
                                End If
                        End Select
                    End If

                End If
            Next

        End If
    End Sub

    Private Sub AddMagazzinoEsternoNote(ByRef Note As String, ByVal DictMagazziniEsterni As Dictionary(Of (Integer, String, String), String), ByVal Prodotto_Cod As Integer)

        'Aggiungo l'indicazione del Magazzino Esterno nelle Note 
        If Not IsNothing(DictMagazziniEsterni) AndAlso DictMagazziniEsterni.Count > 0 Then

            Dim ListMagazziniEsterniConFr_Cod As List(Of (Integer, String, String)) = DictMagazziniEsterni.Keys.ToList().FindAll(Function(MagazzinoEsterno) MagazzinoEsterno.Item1 = Prodotto_Cod).ToList()

            Dim ListDescrizioneMagazziniEsterni As New List(Of String)

            If Not IsNothing(ListMagazziniEsterniConFr_Cod) AndAlso ListMagazziniEsterniConFr_Cod.Count > 0 Then
                For Each MagazzinoEsterno_Key In ListMagazziniEsterniConFr_Cod
                    If Not ListDescrizioneMagazziniEsterni.Contains(DictMagazziniEsterni.Item(MagazzinoEsterno_Key)) Then
                        ListDescrizioneMagazziniEsterni.Add(DictMagazziniEsterni.Item(MagazzinoEsterno_Key))
                    End If
                Next
            End If

            If ListDescrizioneMagazziniEsterni.Count > 0 Then
                Note &= vbCrLf & "Magazzino Esterno: " & String.Join(", ", ListDescrizioneMagazziniEsterni)
            End If

        End If
    End Sub

End Class
