

Imports CrystalDecisions.Shared
Imports CrystalDecisions.CrystalReports.Engine
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class SchedaRilievi
    Inherits System.Web.UI.Page

    Private DsRilieviAvversitaAusiliari As DS_SchedaRilievi

    Private Qs_arrotonda As String
    Private Qs_StampaDefinitiva As String
    Private Qs_DataFine As String
    Private Qs_DataInizio As String
    Private Qs_DataStampa As String

    Dim LinkPaginaStampa As String
    Dim BaseCode As Integer

    Dim Piva As String
    Dim Veg_Cod As String
    Dim strVeg_Cod As String
    Dim Sa_Cod As String
    Dim Gru_Cod As Integer
    Dim Appezza As String
    Dim Id_Reg As String
    'Dim strAppezza_Singoli As String   'senza padri
    'Dim strId_Reg_Singoli As String    'senza padri
    Dim strSa_Cod As String
    Dim strAppezza As String
    Dim strId_Reg As String
    Dim strSa_Cod_Padri As String
    Dim strAppezza_Padri As String
    Dim strId_Reg_Padri As String

    Dim strFiltroImpianti As String
    Dim strFiltroImpianto As String

    Dim strFiltroCentri As String
    Dim strFiltroCentro As String

    'Dim FF_Cod As Integer
    Dim strFF_Cod As String

    Dim strSezioni As String
    Dim strSezioniVuote As String
    Dim strTipoSuperficie As String

    Dim arraySezioni() As String

    Dim Matrice_Padri(,) As String
    ' Dim RsPadri As ADODB.Recordset

    Dim Lingua As Integer = 1

    'Vettori di appoggio per la stampa dei valori di default
    'Dim Appezzamenti() As Integer
    'Dim Date_Semina_Previste() As String
    'Dim Date_Fioritura_Previste() As String
    'Dim Date_Raccolta_Previste() As String
    'Dim Rese_Previste() As String
    'Dim Num_App() As Integer
    'Dim Lotto() As String
    'Dim Cultivar() As String
    'Dim Sup_Ha_App() As String

    Dim defaults() As StampeDefault

    'HashTable per la ricerca del codice del centro aziendale per la scheda di campagna multicentro
    Dim HTCentri As Hashtable

    Dim TerrenoNudoIncluso As Boolean = False
    'var utilizzata per sapere il tipo di report scelto,
    'se la scheda di campagna normale (0), quella ridotta (1), eurep_gap (2), scheda campagna conserve italia (3)
    'eurep_gap semplificata (4)
    Dim TipoReport As Integer


    Dim Patentino As String
    Dim Titolare As String
    Dim Data_Scadenza_Patentino As String

    'oggetto objparametri x server 
    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri

    Public Enum enum_TipoReport

        SchedaCampagna = 0  'Disattivata
        SchedaCampagna_Semplificata = 1
        Eurep_Gap = 2   'Disattivata
        SchedaCampagna_ConserveItalia = 3
        Eurep_Gap_Semplificata = 4
        SchedaCampagna_Pizzoli = 5
        SchedaCampagna_Multi = 6    'Multi Specie Multi Centri
        RegistroTrattamenti_Veneto = 7
        SchedaCampagna_Multicentro = 8
        Eurep_Gap_Multicentro = 9

    End Enum




    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Tolgo la pagina dalla cache
        Response.Expires = 0


        Dim strFullQuery As String
        Dim StrQueryErbacee As String
        Dim StrQueryTratta As String


        Dim CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer

        'Qs_DataInizio = Stringa_Decodifica(Request.QueryString("dI").ToString, _
        '                                   AgroKey_EncoderDecoder, _
        '                                   Server)

        'Qs_DataFine = Stringa_Decodifica(Request.QueryString("dF").ToString, _
        '                                 AgroKey_EncoderDecoder, _
        '                                 Server)

        'Qs_DataStampa = Stringa_Decodifica(Request.QueryString("dG").ToString, _
        '                                AgroKey_EncoderDecoder, _
        '                                Server)


        'If Not IsNothing(Request.QueryString("arr")) AndAlso Stringa_Decodifica(Request.QueryString("arr").ToString, _
        '                            AgroKey_EncoderDecoder, _
        '                            Server) <> "" Then
        '    Qs_arrotonda = Stringa_Decodifica(Request.QueryString("arr").ToString, _
        '                            AgroKey_EncoderDecoder, _
        '                            Server)
        'Else
        '    Qs_arrotonda = "-1"
        'End If

        '' se è una stampa definitiva salvo il pdf e visualizzo l'anteprima, altrimenti visualizzo solo l'anteprima
        'If Not IsNothing(Request.QueryString("stDef")) AndAlso Stringa_Decodifica(Request.QueryString("stDef").ToString, _
        '                            AgroKey_EncoderDecoder, _
        '                            Server) <> "" Then
        '    Qs_StampaDefinitiva = Stringa_Decodifica(Request.QueryString("stDef").ToString, _
        '                            AgroKey_EncoderDecoder, _
        '                            Server)
        'Else
        '    Qs_StampaDefinitiva = "0"       ' stampa di prova
        'End If



        ''Imposto la sezione in sessione con quella passata via query string - introdotta per l'apertura
        ''multipla delle schede del registro trattamenti del Veneto.
        'If Not IsNothing(Request.QueryString("sez")) AndAlso Stringa_Decodifica(Request.QueryString("sez").ToString, _
        '                            AgroKey_EncoderDecoder, _
        '                            Server) <> "" Then
        '    Session("sezioni") = Stringa_Decodifica(Request.QueryString("sez").ToString, _
        '                            AgroKey_EncoderDecoder, _
        '                            Server)
        'End If

        'inizializzazione oggetti objParametri_Utenti e objParametri_Server
        '---
        'objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        '---

        'Dim log As New AgronicaCoreDataProvider.LogProvider
        'log.Scrivi_LOG(objParametri_Server, "Page_Load", " - " & System.DateTime.Now)

        '##############################################################
        '#####  Recupero piva e veg_cod  ##############################
        '##############################################################

        Dim GruppoVeg As New AgronicaCoreMetaSchemaDAL.GruppoVegetale_R

        Dim strXmlVariabilistampe As String
        Dim htVariabiliStampe As System.Collections.Hashtable
        Dim strErr As String
        Dim StrSQL As String
        'Dim objSQL As New Codex_Utility.Sql
        ' Dim Rs As ADODB.Recordset

        Dim Validita_Inizio As String
        Dim Validita_Fine As String
        Dim GiornoPrecedente As Date

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XML_FiltroStampa As System.Xml.XmlElement
        Dim XMLs_VariabiliStampe As System.Xml.XmlNodeList
        Dim XML_VariabiliStampe As System.Xml.XmlElement

        Dim Matrice_Variabili(0, 0) As String
        Dim Matrice_Padri(0, 0) As String

        Dim i As Integer
        Dim RigaPadri As Integer
        Dim NumPadri As Integer

        Dim HCentri As New Hashtable

        strFiltroCentri = String.Empty

        strXmlVariabilistampe = Session("strXmlVariabilistampe")

        'Carico la stringa xml in un nuovo documento
        'tmp perchè non ha padre?? 11/04/2014
        XmlDoc = New System.Xml.XmlDocument
        XmlDoc.LoadXml(strXmlVariabilistampe)

        If XmlDoc.HasChildNodes Then

            'Ricavo i parametri che servono

            XML_FiltroStampa = XmlDoc.SelectSingleNode("ParametriAgronicaStampe_2010")

            Session("ASG_Utente_Username") = XML_FiltroStampa.GetAttribute("username")

            XMLs_VariabiliStampe = XML_FiltroStampa.GetElementsByTagName("VariabiliStampe")

            ReDim Matrice_Variabili(XMLs_VariabiliStampe.Count - 1, 4)
            ReDim Matrice_Padri(XMLs_VariabiliStampe.Count - 1, 100)

            For i = 0 To XMLs_VariabiliStampe.Count - 1

                XML_VariabiliStampe = XMLs_VariabiliStampe.Item(i)

                Matrice_Variabili(i, 0) = XML_VariabiliStampe.GetAttribute("piva")
                Matrice_Variabili(i, 1) = XML_VariabiliStampe.GetAttribute("sa_cod")
                Matrice_Variabili(i, 2) = XML_VariabiliStampe.GetAttribute("appezza")
                Matrice_Variabili(i, 3) = XML_VariabiliStampe.GetAttribute("id_reg")
                Matrice_Variabili(i, 4) = XML_VariabiliStampe.GetAttribute("veg_cod")

                If InStr(strVeg_Cod, XML_VariabiliStampe.GetAttribute("veg_cod") & ",") = 0 AndAlso _
                        XML_VariabiliStampe.GetAttribute("veg_cod") <> "0" Then
                    strVeg_Cod &= XML_VariabiliStampe.GetAttribute("veg_cod") & ","
                End If

                'se sa_cod=0 la stampa riguarda tutti gli impianti di tutti i centri
                If Matrice_Variabili(i, 1) <> "0" AndAlso Not HCentri.ContainsKey(Matrice_Variabili(i, 1)) Then

                    HCentri.Add(Matrice_Variabili(i, 1), Matrice_Variabili(i, 1))

                    strFiltroCentro = String.Format(" ( Centri_Aziendali.PIVA='{0}' AND Centri_Aziendali.SA_COD={1} ) OR", XML_VariabiliStampe.GetAttribute("piva"), XML_VariabiliStampe.GetAttribute("sa_cod"))

                    strFiltroCentri = String.Format("{0}{1}", strFiltroCentri, strFiltroCentro)
                End If

                'se appezza=0 e id_reg=0 la stampa riguarda tutti gli impianti del centro
                If Matrice_Variabili(i, 2) <> "0" And Matrice_Variabili(i, 3) <> "0" Then

                    strFiltroImpianto = " (Reg_Impianti.PIVA='" + XML_VariabiliStampe.GetAttribute("piva") + "' " + _
                                        " AND Reg_Impianti.SA_COD=" + XML_VariabiliStampe.GetAttribute("sa_cod") + _
                                        " AND Reg_Impianti.APPEZZA=" + XML_VariabiliStampe.GetAttribute("appezza") + _
                                        " AND Reg_Impianti.ID_REG=" + XML_VariabiliStampe.GetAttribute("id_reg") + _
                                        " ) OR"

                    strFiltroImpianti = strFiltroImpianti + strFiltroImpianto

                    strSa_Cod = strSa_Cod & Matrice_Variabili(i, 1) & ","
                    strAppezza = strAppezza & Matrice_Variabili(i, 2) & ","
                    strId_Reg = strId_Reg & Matrice_Variabili(i, 3) & ","

                    'figlio
                    Matrice_Padri(i, 0) = Matrice_Variabili(i, 0) & "/" & Matrice_Variabili(i, 1) & "/" & Matrice_Variabili(i, 2) & "/" & Matrice_Variabili(i, 3)

                    NumPadri = 0



                End If

            Next

            ViewState("Matrice_Padri") = Matrice_Padri

            'elimino l'ultima virgola
            If strSa_Cod <> "" Then
                strSa_Cod = Left(strSa_Cod, strSa_Cod.Length - 1)
            End If
            If strAppezza <> "" Then
                strAppezza = Left(strAppezza, strAppezza.Length - 1)
            End If
            If strId_Reg <> "" Then
                strId_Reg = Left(strId_Reg, strId_Reg.Length - 1)
            End If

            If strSa_Cod_Padri <> "" Then
                strSa_Cod_Padri = Left(strSa_Cod_Padri, strSa_Cod_Padri.Length - 1)
            End If
            If strAppezza_Padri <> "" Then
                strAppezza_Padri = Left(strAppezza_Padri, strAppezza_Padri.Length - 1)
            End If
            If strId_Reg_Padri <> "" Then
                strId_Reg_Padri = Left(strId_Reg_Padri, strId_Reg_Padri.Length - 1)
            End If

            If strAppezza <> "" And strAppezza_Padri <> "" Then
                strSa_Cod = strSa_Cod & "," & strSa_Cod_Padri
                strAppezza = strAppezza & "," & strAppezza_Padri
                strId_Reg = strId_Reg & "," & strId_Reg_Padri
            End If

            'tolgo l'ultimo OR
            strFiltroImpianti = Left(strFiltroImpianti, strFiltroImpianti.Length - 2)
            strFiltroCentri = Left(strFiltroCentri, strFiltroCentri.Length - 2)

            'Per ora prendo piva, sa_cod e veg_cod del primo impianto 
            'poichè la scheda è stampabile su 1 Impresa (1 centro) per 1 Specie
            Piva = Matrice_Variabili(0, 0)
            Sa_Cod = Matrice_Variabili(0, 1)
            Veg_Cod = Matrice_Variabili(0, 4)

            If strVeg_Cod <> "" Then
                strVeg_Cod = Left(strVeg_Cod, strVeg_Cod.Length - 1)
            End If

            Gru_Cod = CInt(GruppoVeg.GruCod_from_VegCod(Veg_Cod, objParametri_Server))

            Dim objFioriture As New AgronicaCoreMetaSchemaDAL.FasiFenologichexFioriture_R
            Dim DtFio As DataTable
            DtFio = objFioriture.Fioriture_from_VegCod(strVeg_Cod, _
                                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                                                        "", "", objParametri_Server)
            If Not DtFio Is Nothing AndAlso DtFio.Rows.Count > 0 Then
                For i = 0 To DtFio.Rows.Count - 1
                    strFF_Cod &= DtFio.Rows(i).Item("ff_cod") & ","
                Next
                If strFF_Cod <> "" Then
                    strFF_Cod = Left(strFF_Cod, strFF_Cod.Length - 1)
                End If
            Else
                strFF_Cod = "0"
            End If

        End If





        '##############################################################
        '#####  Verifico se sono in Post-Back  ########################
        '##############################################################

        If Not Me.IsPostBack Then

            'CrystalReportViewer1 = New CrystalDecisions.Web.CrystalReportViewer

            'CrystalReportViewer1.Style.Add("LEFT", "-275px")
            'CrystalReportViewer1.Style.Add("TOP", "0px")
            'CrystalReportViewer1.Style.Add("POSITION", "Absolute")
            'Me.FindControl("Form1").Controls.Add(CrystalReportViewer1)

            'calcolo il basecode x l'utente
            Calcola_BaseCode_TopCode(BaseCode, Nothing, Session("ASG_ProgressivoGIAS"))

            'Inizializzazione dei vettori di appoggio per la stampa dei valori di default
            'NON SPOSTARE! - Per il calcolo del nr. appezzamento ho bisogno del BaseCode.
            Dim DtImpianti As DataTable
            Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim DtProgetti As DataTable
            Dim objProgetti As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
            Dim AppezzamentoNome As String
            Dim objAppezzamenti As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
            Dim objCultivar As New AgronicaCoreMetaSchemaDAL.Cultivar_R
            Dim j As Integer
            Dim Counter As Integer = 0
            Dim strImpiantiDataFiltro As String = String.Empty

            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim DtImpostazioni As DataTable



            Qs_DataStampa = ""

            Qs_DataInizio = "01/01/1900"
            Qs_DataFine = "31/12/2100"

            'se ho scelto una data unica per la stampa..
            If Qs_DataStampa <> "" Then

                strImpiantiDataFiltro = String.Format(" AND Reg_Impianti.Validita_Inizio <= {0} ", Agro_SQL_SaveDate(CDate(Qs_DataStampa)))

                strImpiantiDataFiltro = String.Format("{0}AND Reg_Impianti.Validita_Fine >= {1} ", strImpiantiDataFiltro, Agro_SQL_SaveDate(CDate(Qs_DataStampa)))

            Else

                strImpiantiDataFiltro = String.Format(" AND Reg_Impianti.Validita_Inizio <= {0} ", Agro_SQL_SaveDate(CDate(Qs_DataFine)))

                strImpiantiDataFiltro = String.Format("{0}AND Reg_Impianti.Validita_Fine >= {1} ", strImpiantiDataFiltro, Agro_SQL_SaveDate(CDate(Qs_DataInizio)))

            End If


            strFiltroImpianti = String.Format(" AND {0} {1}", "( " & strFiltroImpianti & " )", strImpiantiDataFiltro)


            '--- rilievi avversità in campo
            DsRilieviAvversitaAusiliari = New DS_SchedaRilievi


            CaricaDSRilAvvAus()

            Dim leggiUtente As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
            Dim dtU As DataTable
            For Each riga As DS_SchedaRilievi.DS_SchedaRilieviRow In DsRilieviAvversitaAusiliari._DS_SchedaRilievi.Rows

                Dim user As String = riga("Utente")
                dtU = leggiUtente.Leggi("", 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, " Utenti_Dettagli.CodFisc = '" & Agro_SQL_SaveText(user) & "'", "", objParametri_Utenti)

                If dtU.Rows.Count > 0 Then
                    riga("Utente") = dtU.Rows(0)("Cognome") & " " & dtU.Rows(0)("Nome")
                End If

            Next

        End If 'page.ispostback()


        Dim rptSchedaRilievo As New ReportSchedaRilieviPivot
        rptSchedaRilievo.SetDataSource(DsRilieviAvversitaAusiliari)

        Session("Report") = rptSchedaRilievo

        Response.Redirect("..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))


    End Sub


    '###########################################################################
    'riempie il ds x il Rilevi Avversità e Ausiliari
    Private Sub CaricaDSRilAvvAus()

        'Dim objSql As New Codex_Utility.Sql
        Dim objSql As New AgronicaCoreDataProvider.DataProvider
        Dim strErr As String
        Dim stb As New System.Text.StringBuilder
        Dim i As Integer
        Dim App_Nome As String
        Dim Num_Appezzamento As Double
        Dim objNum As New AgronicaCoreDataProvider.UtilityProvider

        Dim Rs As DataTable

        'query x reperire i dati
        stb.Append(" " & vbCrLf)
        stb.Append("      SELECT  " & vbCrLf)
        stb.Append("      op.PIVA " & vbCrLf)
        stb.Append("    , i.rag_soc AS RagioneSociale " & vbCrLf)
        stb.Append("    , sa.sa_nome " & vbCrLf)
        stb.Append("    , op.Data_Movimento AS DataOperazione " & vbCrLf)
        stb.Append("    , SpecieVegetali.Veg_Des " & vbCrLf)
        stb.Append("    , Cultivar.Cul_Des " & vbCrLf)
        stb.Append("    , CAST(op.Sup_Imp AS integer) AS Sup_Ha " & vbCrLf)
        stb.Append("    , CAST((op.Sup_Imp - CAST(op.Sup_Imp AS integer)) * 100 AS integer) AS Sup_Are " & vbCrLf)
        stb.Append("    , ff.FF_DES " & vbCrLf)

        stb.Append("    , case when mav1.cod = mav.cod then  " & vbCrLf)
        stb.Append("            case when mav.av_cod = 99999 then '_Ril. Avv. Si/No' else ISNULL(mav.Av_Des_Vol collate Latin1_General_CI_AS, '') + ' ' + ISNULL(mav.UDM_DES, '') end " & vbCrLf)
        stb.Append("        else " & vbCrLf)
        stb.Append("            case when mav1.av_cod = 99999 then '_Ril. Avv. Si/No' else ISNULL(mav1.Av_Des_Vol collate Latin1_General_CI_AS, '') + ' ' + ISNULL(mav1.UDM_DES, '') end " & vbCrLf)
        stb.Append("        end AS UDM_DES " & vbCrLf)

        stb.Append("    , case when mav1.cod = mav.cod then  op.Qta else '' end as Qta " & vbCrLf)
        stb.Append("    , G.lat AS Latitudine " & vbCrLf)
        stb.Append("    , G.lon AS Longitudine " & vbCrLf)
        stb.Append("    , COALESCE (op.Via_Stringa, '') AS Appezzamento_Indirizzo " & vbCrLf)
        stb.Append("    , op.APP_NOME AS Appezzamento_Descrizione " & vbCrLf)
        stb.Append("    , case when mav1.cod = mav.cod then  " & vbCrLf)
        stb.Append("            case when mav.av_cod = 99999 then '_Ril. Avv. Si/No' else ISNULL(mav.Av_Des_Vol collate Latin1_General_CI_AS, '') + ' ' + ISNULL(mav.UDM_DES, '') end " & vbCrLf)
        stb.Append("        else " & vbCrLf)
        stb.Append("            case when mav1.av_cod = 99999 then '_Ril. Avv. Si/No' else ISNULL(mav1.Av_Des_Vol collate Latin1_General_CI_AS, '') + ' ' + ISNULL(mav1.UDM_DES, '') end " & vbCrLf)
        stb.Append("        end AS  Av_Des " & vbCrLf)

        stb.Append("    , op.Username_Modifica AS Utente " & vbCrLf)
        stb.Append("    , G.[TEXT] AS DescrizionePunto " & vbCrLf)
        stb.Append("    , case when op.Piezo1 = 0 or op.Piezo1 = 1 then 'Cv Docg' else case when op.Piezo1 = 2 then  'Pendenze' else 'Testimone Fen.' end end as Progetto  " & vbCrLf)
        stb.Append("    , op.Des_Lib as DescrizioneOperazione " & vbCrLf)
        stb.Append("    , ff2.ff_Des as ff_Des2 " & vbCrLf)
        stb.Append("    , op.NoteGenerali " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("    , case when mav1.cod = mav.cod then  " & vbCrLf)        
        stb.Append("        coalesce(anag.anag_des, cast( op.qta as varchar(1000) ) ) " & vbCrLf)
        stb.Append("    else " & vbCrLf)
        stb.Append("        '' " & vbCrLf)
        stb.Append("    end AS qta_Descrittiva " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("    , op.id_agenda " & vbCrLf)
        stb.Append("    , mav1.ordine " & vbCrLf)

        stb.Append(" FROM  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append(" ( " & vbCrLf)
        stb.Append("    select mav.cod, Avversita.Av_Cod, Avversita.Abbreviazione as Av_Des_Vol, UnitaMisura.UDM_COD, UnitaMisura.UDM_SIM as UDM_DES, mav.ordine   " & vbCrLf)
        stb.Append("    from  " & vbCrLf)
        stb.Append("    MisuraxAvversita mav  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("    INNER JOIN  Avversita ON mav.Av_Cod = Avversita.Av_Cod  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("    INNER JOIN UnitaMisura ON mav.Udm_Cod = UnitaMisura.Udm_Cod  " & vbCrLf)        
        stb.Append(" ) mav " & vbCrLf)
        stb.Append(" inner join  " & vbCrLf)
        stb.Append(" ( " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("    select  " & vbCrLf)
        stb.Append("        Agenda.piva " & vbCrLf)
        stb.Append("        , Agenda.Sa_Cod " & vbCrLf)
        stb.Append("        , Agenda.id_agenda " & vbCrLf)
        stb.Append("        , Agenda.lav_cod " & vbCrLf)
        stb.Append("        , Mov_Destinazioni.mov_destinazioni_graphickey " & vbCrLf)
        stb.Append("        , Mov_Dettaglio_Tecnico.Av_Cod " & vbCrLf)
        stb.Append("        , Movimenti_dettagli.Udm_Cod " & vbCrLf)
        stb.Append("        , Reg_Impianti.CUL_COD " & vbCrLf)
        stb.Append("        , Mov_Dettaglio_Tecnico.FF_Classe " & vbCrLf)
        stb.Append("        , Movimenti.Data_Movimento " & vbCrLf)
        stb.Append("        , Reg_Impianti.Sup_Imp " & vbCrLf)
        stb.Append("        , Mov_Destinazioni.Qta " & vbCrLf)
        stb.Append("        , Appezzamento.Via_Stringa " & vbCrLf)
        stb.Append("        , Appezzamento.APP_NOME " & vbCrLf)
        stb.Append("        , Agenda.Username_Modifica " & vbCrLf)
        stb.Append("        , mov_dettaglio_tecnico.Piezo1 " & vbCrLf)
        stb.Append("        , mov_dettaglio_tecnico.Piezo2 as FF_Classe2 " & vbCrLf)
        stb.Append("        , Agenda.Des_Lib " & vbCrLf)
        stb.Append("        , Reg_Impianti.Appezza " & vbCrLf)
        stb.Append("        , Reg_Impianti.id_reg " & vbCrLf)
        stb.Append("        , Reg_Impianti.Validita_Inizio " & vbCrLf)
        stb.Append("        , Reg_Impianti.Validita_Fine " & vbCrLf)
        stb.Append("        , Agenda.Validita_Inizio as Agenda_Validita_Inizio " & vbCrLf)
        stb.Append("        , Agenda.Validita_Fine as Agenda_Validita_Fine " & vbCrLf)
        stb.Append("        , Movimenti.Mov_Desc as NoteGenerali " & vbCrLf)
        stb.Append("    FROM Agenda  " & vbCrLf)
        stb.Append("    INNER JOIN Movimenti  " & vbCrLf)
        stb.Append("        ON Agenda.Id_Agenda = Movimenti.Id_Agenda  " & vbCrLf)
        stb.Append("        AND Agenda.PIVA = Movimenti.PIVA  " & vbCrLf)
        stb.Append("        AND Agenda.Sa_Cod = Movimenti.Sa_Cod " & vbCrLf)
        stb.Append("     INNER JOIN Movimenti_dettagli  " & vbCrLf)
        stb.Append("        ON Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov  " & vbCrLf)
        stb.Append("        AND Movimenti.PIVA = Movimenti_dettagli.PIVA  " & vbCrLf)
        stb.Append("        AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod  " & vbCrLf)
        stb.Append("        AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda  " & vbCrLf)
        stb.Append("    INNER JOIN    Mov_Dettaglio_Tecnico  " & vbCrLf)
        stb.Append("        ON Movimenti_dettagli.Id_Mov = Mov_Dettaglio_Tecnico.Id_Mov  " & vbCrLf)
        stb.Append("        AND Movimenti_dettagli.PIVA = Mov_Dettaglio_Tecnico.Piva  " & vbCrLf)
        stb.Append("        AND Movimenti_dettagli.Sa_Cod = Mov_Dettaglio_Tecnico.Sa_Cod  " & vbCrLf)
        stb.Append("        AND Movimenti_dettagli.Id_Agenda = Mov_Dettaglio_Tecnico.Id_Agenda  " & vbCrLf)
        stb.Append("        AND Movimenti_dettagli.Id_Mov_Det = Mov_Dettaglio_Tecnico.Id_Mov_Det  " & vbCrLf)
        stb.Append("    INNER JOIN Mov_Destinazioni  " & vbCrLf)
        stb.Append("        ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva  " & vbCrLf)
        stb.Append("        AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod  " & vbCrLf)
        stb.Append("        AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda  " & vbCrLf)
        stb.Append("        AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov  " & vbCrLf)
        stb.Append("        AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det  " & vbCrLf)
        stb.Append("    INNER JOIN Appezzamento  " & vbCrLf)
        stb.Append("        ON Mov_Destinazioni.Piva = Appezzamento.PIVA  " & vbCrLf)
        stb.Append("        AND Mov_Destinazioni.Sa_Cod = Appezzamento.SA_COD  " & vbCrLf)
        stb.Append("        AND Mov_Destinazioni.Appezza = Appezzamento.APPEZZA  " & vbCrLf)
        stb.Append("    INNER JOIN Reg_Impianti  " & vbCrLf)
        stb.Append("        ON Mov_Destinazioni.Piva = Reg_Impianti.PIVA  " & vbCrLf)
        stb.Append("        AND Mov_Destinazioni.Sa_Cod = Reg_Impianti.SA_COD  " & vbCrLf)
        stb.Append("        AND Mov_Destinazioni.Id_Destinazione = Reg_Impianti.ID_REG  " & vbCrLf)
        stb.Append("        AND Mov_Destinazioni.Appezza = Reg_Impianti.APPEZZA  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append(" ) op " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append(" on op.av_cod = mav.av_cod " & vbCrLf)
        stb.Append(" and op.udm_cod = mav.udm_cod " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append(" left join  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append(" ( " & vbCrLf)
        stb.Append("    select mav.cod, Avversita.Av_Cod, Avversita.Abbreviazione as Av_Des_Vol, UnitaMisura.UDM_COD, UnitaMisura.UDM_SIM as UDM_DES, mav.ordine   " & vbCrLf)
        stb.Append("    from  " & vbCrLf)
        stb.Append("    MisuraxAvversita mav  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("    INNER JOIN  Avversita ON mav.Av_Cod = Avversita.Av_Cod  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("    INNER JOIN UnitaMisura ON mav.Udm_Cod = UnitaMisura.Udm_Cod  " & vbCrLf)        
        stb.Append(" ) mav1 " & vbCrLf)
        stb.Append(" on 1=1 " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append(" LEFT JOIN Cultivar  " & vbCrLf)
        stb.Append("        ON Cultivar.Cul_Cod = op.CUL_COD  " & vbCrLf)
        stb.Append(" LEFT JOIN SpecieVegetali  " & vbCrLf)
        stb.Append("    ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod  " & vbCrLf)
        stb.Append(" LEFT JOIN Imprese AS i  " & vbCrLf)
        stb.Append("    ON i.PIVA = op.PIVA  " & vbCrLf)
        stb.Append(" LEFT JOIN Centri_Aziendali AS sa  " & vbCrLf)
        stb.Append("    ON sa.PIVA = op.PIVA  " & vbCrLf)
        stb.Append("    AND sa.sa_cod = op.sa_cod  " & vbCrLf)
        stb.Append(" LEFT OUTER JOIN FasiFenologiche AS ff ON ff.FF_COD = op.FF_Classe " & vbCrLf)
        stb.Append(" LEFT OUTER JOIN FasiFenologiche AS ff2 ON ff2.FF_COD = op.FF_Classe2 " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append(" LEFT OUTER JOIN " & vbCrLf)
        stb.Append("     Grafica AS G ON G.Piva = op.PIVA AND G.Sa_Cod = op.Sa_Cod AND G.Id = op.mov_destinazioni_graphickey " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append(" left join MisuraXAvversita_Anagrafiche anag " & vbCrLf)
        stb.Append("    on anag.MxAV_Cod = mav.COD  " & vbCrLf)
        stb.Append("    and anag.Anag_valore = op.Qta " & vbCrLf)

        stb.Append(" WHERE 1=1 " & vbCrLf)

        'If Veg_Cod = "0" Then
        '    stb.Append(" and Reg_Impianti.cul_cod = 0 ")
        'Else
        '    If TerrenoNudoIncluso = False Then
        '        stb.Append(" AND SpecieVegetali.VEG_COD = " & Veg_Cod)
        '    Else
        '        stb.Append(" AND (SpecieVegetali.VEG_COD = " & Veg_Cod & " OR Reg_Impianti.cul_cod = 0) ")
        '    End If
        'End If

        'stb.Append(" AND op.Piva = '" & Piva & "' ")



        stb.Append(strFiltroImpianti.Replace("Reg_Impianti", "op"))

        'se ho scelto una data unica per la stampa..
        If Qs_DataStampa <> "" Then

            stb.Append(" AND op.Validita_Inizio <= " & Agro_SQL_SaveDate(CDate(Qs_DataStampa)) & " ")
            stb.Append(" AND op.Validita_Fine >= " & Agro_SQL_SaveDate(CDate(Qs_DataStampa)) & " ")

            stb.Append(" AND op.Agenda_Validita_Inizio = " & Agro_SQL_SaveDate(CDate(Qs_DataStampa)) & " ")
        Else

            stb.Append(" AND op.Validita_Inizio <= " & Agro_SQL_SaveDate(CDate(Qs_DataFine)) & " ")
            stb.Append(" AND op.Validita_Fine >= " & Agro_SQL_SaveDate(CDate(Qs_DataInizio)) & " ")

            stb.Append(" AND op.Agenda_Validita_Inizio <= " & Agro_SQL_SaveDate(CDate(Qs_DataFine)) & " ")
            stb.Append(" AND op.Agenda_Validita_Inizio >= " & Agro_SQL_SaveDate(CDate(Qs_DataInizio)) & " ")

        End If

        stb.Append("ORDER BY mav1.ordine ")



        strErr = Nothing
        Try

            objSql.EseguiQuery_Lettura(objParametri_Server, stb.ToString, "CaricaDsRilAvvAus", DsRilieviAvversitaAusiliari, "DS_SchedaRilievi")

        Catch ex As Exception
            strErr = ex.Message
        End Try




    End Sub



End Class