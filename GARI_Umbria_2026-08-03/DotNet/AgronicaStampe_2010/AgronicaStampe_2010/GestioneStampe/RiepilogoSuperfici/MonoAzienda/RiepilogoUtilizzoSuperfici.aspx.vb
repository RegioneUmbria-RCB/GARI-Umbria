Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreAnagrafeDAL
Imports System.Web.Services
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider.My.Resources
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreStampeDAL

Public Class RiepilogoUtilizzoSuperfici
    Inherits System.Web.UI.Page

    Private rptStampa As Rpt_RiepilogoUtilizzoSuperfici
    Private DSRiepilogoUtilizzoSuperfici As DS_RiepilogoUtilizzoSuperfici

    '----- Gestione Querystring
    Dim Qs_Piva As String
    Dim Qs_Sa_Cod As String
    Dim QS_Data As Date

    Dim LinkPaginaStampa As String

    Dim Matrice_SAUInUsoxSpecie_Azienda(1, 0) As Object
    Dim Numero_Specie_Azienda As Integer

    'oggetto objparametri x server e utenti
    Dim objParametri_Utenti As AgronicaCoreParametri
    Dim objParametri_Server As AgronicaCoreParametri

    Private Sub RiepilogoUtilizzoSuperfici_Init(sender As Object, e As EventArgs) Handles Me.Init
        'istanzio l'oggetto report
        rptStampa = New Rpt_RiepilogoUtilizzoSuperfici
    End Sub

    '###########################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato

        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If

        'inizializzazione oggetti objParametri_Utenti e objParametri_Server
        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        '------------

        '''#################################################################################
        '''#####  Recupero piva, centro aziendale e data di stampa dalla QueryString 
        '''#################################################################################

        Qs_Piva = Stringa_Decodifica(Request.QueryString("p").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)

        Qs_Sa_Cod = Stringa_Decodifica(Request.QueryString("s_c").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)

        QS_Data = Stringa_Decodifica(Request.QueryString("d_r").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)

        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        If Not Me.IsPostBack Then

            Dim Nome_Documento As String = "RiepilogoUtilizzoSuperfici"
            Dim Log_Errori As String = ""

            Try
                Dim objImprese As New Imprese_Read
                Dim objCentriAz As New CentriAziendali_Read

                CType(rptStampa.Section1.ReportObjects("TextAzienda"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = objImprese.RagSoc_from_Piva(Qs_Piva, objParametri_Server)
                If Qs_Sa_Cod <> "0" Then
                    CType(rptStampa.Section1.ReportObjects("TextCentro"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = objCentriAz.SaNome_from_SaCod(Qs_Piva, Qs_Sa_Cod, objParametri_Server)
                Else
                    CType(rptStampa.Section1.ReportObjects("TextCentro"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Tutti"
                End If

                Dim objImp As New AgronicaCoreAnagrafeDAL.Imprese_Read
                Dim pivaReale As String = objImp.Leggi_PivaReale(Qs_Piva, objParametri_Server)

                CType(rptStampa.Section1.ReportObjects("TextPiva"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = pivaReale

                CType(rptStampa.Section1.ReportObjects("TextData"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = QS_Data.ToString("d")

            Catch ex As Exception
                Log_Errori &= "- Impostazione TextObject:" & vbCrLf & ex.Message & vbCrLf
            End Try

            'creo una nuova istanza dei 3 dataset
            Dim DSRiepilogoUtilizzoSuperfici As New DS_RiepilogoUtilizzoSuperfici
            Dim DS_SAU_Azienda As New DS_Dettaglio_SAU_Utilizzata_Azienda
            Dim DS_SAU_Controllate As New DS_Dettaglio_SAU_Utilizzata_Controllate

            Try
                'carico i dati nei 3 dataset
                Carica_DSRiepilogoUtilizzoSuperfici(DSRiepilogoUtilizzoSuperfici, DS_SAU_Azienda, DS_SAU_Controllate, Log_Errori)

            Catch ex As Exception
                Log_Errori &= "- PageLoad:" & vbCrLf & ex.Message & vbCrLf
            End Try


            Try
                'sorgente dati.....
                rptStampa.SetDataSource(DSRiepilogoUtilizzoSuperfici)
                rptStampa.OpenSubreport("Dettaglio_SAU_Utilizzata_Azienda.rpt").SetDataSource(DS_SAU_Azienda)
                rptStampa.OpenSubreport("Dettaglio_SAU_Utilizzata_Controllate.rpt").SetDataSource(DS_SAU_Controllate)

            Catch ex As Exception
                Log_Errori &= "- Aggancio dataset:" & vbCrLf & ex.Message & vbCrLf
            End Try


            Dim reportTemporaneo As String = CrystalHelper.getFileReportTemporaneo()

            Try
                Dim prc As New CrystalDecisions.Shared.ReportPageRequestContext
                Dim dummy = rptStampa.FormatEngine.GetLastPageNumber(prc)
                rptStampa.SaveAs(reportTemporaneo, True)
            Catch ex As Exception
                Log_Errori += "- Salvataggio report temporaneo: " + vbCrLf + ex.Message + vbCrLf
            End Try



            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            Dim Nome_File As String

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento + ", " + vbCrLf +
                                "Piva = " + CStr(Qs_Piva) + ", " + vbCrLf +
                                "Sa_Cod = " + CStr(Qs_Sa_Cod) + ", " + vbCrLf +
                                                 vbCrLf + vbCrLf + vbCrLf + vbCrLf +
                                Log_Errori

                Nome_File = "Log_Errori_" + Nome_Documento

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server,
                                                 "RiepilogoUtilizzoSuperfici",
                                                 Nome_File & ".txt",
                                                 Session("ASG_Utente_Username"),
                                                 Nome_Documento,
                                                 Log_Errori)

            End If
            '-----------------------------------------

            '-----------------------------------------
            '---- redirect -------------
            '-----------------------------------------
            Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) +
                    "&tmpReportPath=" + Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server) &
                    "&NomePdf=" & Stringa_Codifica(Nome_Documento, AgroKey_EncoderDecoder, Server))

        End If


    End Sub


    '############################################################################################
    'riempie i ds DSRiepilogoUtilizzoSuperfici, DS_SAU_Azienda, DS_SAU_Controllate
    Private Sub Carica_DSRiepilogoUtilizzoSuperfici(ByRef DSRiepilogoUtilizzoSuperfici As DS_RiepilogoUtilizzoSuperfici,
                                                    ByRef DS_SAU_Azienda As DS_Dettaglio_SAU_Utilizzata_Azienda,
                                                    ByRef DS_SAU_Controllate As DS_Dettaglio_SAU_Utilizzata_Controllate,
                                                    ByRef Log_Errori As String)

        Dim GerarchiaImprese_R As New GerarchiaImprese_R
        Dim objCentriAz As New CentriAziendali_Read
        Dim objImpresa As New Imprese_Read

        Dim Aziende_Controllate As String
        Dim Aziende_Controllate2 As String
        Dim Aziende() As String

        Dim Sup_Azienda As Double
        Dim Sup_Controllata As Double
        Dim Sup_Controllate As Double

        Dim SAU_Totale_Azienda As Double
        Dim SAU_Totale_Controllata As Double
        Dim SAU_Totale_Controllate As Double

        Dim Sup_AziendaInUso As Double
        Dim Sup_ControllataInUso As Double
        Dim Sup_ControllateInUso As Double

        Dim Sup_AziendaInutilizzata As Double
        Dim Sup_ControllataInutilizzata As Double
        Dim Sup_ControllateInutilizzata As Double

        Dim i As Integer

        'creo una nuova riga del dataset
        Dim drR As DS_RiepilogoUtilizzoSuperfici.DT_RiepilogoUtilizzoSuperficiRow
        drR = DSRiepilogoUtilizzoSuperfici.DT_RiepilogoUtilizzoSuperfici.NewDT_RiepilogoUtilizzoSuperficiRow

        'inserisco piva, rag_soc e data
        drR.piva = Qs_Piva
        'drR.rag_soc = RagSoc_from_Piva(Qs_Piva, Server)
        drR.data = QS_Data.ToString("d")

        If Qs_Sa_Cod = "0" Then

            '-----------------------------------------------------------------------
            'Data la piva dell'azienda selezionata ricavo tutte le pive 
            'delle aziende da lei controllate e le metto in un vettore
            '-----------------------------------------------------------------------

            Call GerarchiaImprese_R.LeggiFigliNodoGerarchiaImprese(Qs_Piva, Aziende_Controllate, objParametri_Server)

            If Aziende_Controllate <> "" Then
                Aziende_Controllate2 = Replace(Aziende_Controllate, "'", "")
                Aziende = Split(Aziende_Controllate2, " , ")
            End If


            '-----------------------------------------------------------------------
            ' SUPERFICIE CATASTALE + SUPERFICIE SAU ( Azienda - Controllate - Somma )
            '-----------------------------------------------------------------------

            objImpresa.Recupera_Superfici_Impresa(Qs_Piva, Sup_Azienda, Nothing, Nothing, Nothing, SAU_Totale_Azienda, Nothing, Nothing, Nothing, QS_Data, objParametri_Server)

            drR.sup_azienda = Format(Sup_Azienda, "0.0000")

            drR.sau_azienda = Format(SAU_Totale_Azienda, "0.0000")

            If Aziende_Controllate <> "" Then

                For i = 0 To UBound(Aziende)
                    objImpresa.Recupera_Superfici_Impresa(Aziende(i), Sup_Controllata, Nothing, Nothing, Nothing, SAU_Totale_Controllata, Nothing, Nothing, Nothing, QS_Data, objParametri_Server)
                    Sup_Controllate += Sup_Controllata
                    SAU_Totale_Controllate += SAU_Totale_Controllata
                Next

            Else

                Sup_Controllate = 0
                SAU_Totale_Controllate = 0

            End If

            drR.sup_controllate = Format(Sup_Controllate, "0.0000")

            drR.sau_controllate = Format(SAU_Totale_Controllate, "0.0000")

            drR.sup_tot = Format(Sup_Azienda + Sup_Controllate, "0.0000")
            drR.sau_tot = Format(SAU_Totale_Azienda + SAU_Totale_Controllate, "0.0000")


            '-----------------------------------------------------------------------
            ' SUPERFICIE SAU IN USO ( Azienda - Controllate - Somma )
            '-----------------------------------------------------------------------

            Recupera_SuperficieInUso_Impresa(Qs_Piva, 0, Sup_AziendaInUso, Server, QS_Data)

            drR.sau_inuso_azienda = Format(Sup_AziendaInUso, "0.0000")

            '--------------------------------------
            ' Riempio il Dataset DS_SAU_Azienda
            '--------------------------------------


            If Sup_AziendaInUso <> 0 Then

                If Not Matrice_SAUInUsoxSpecie_Azienda(0, 0) Is Nothing Then

                    For i = 0 To UBound(Matrice_SAUInUsoxSpecie_Azienda, 2)

                        'creo una nuova riga del dataset
                        Dim drAzienda As DS_Dettaglio_SAU_Utilizzata_Azienda.DT_Dettaglio_SAU_Utilizzata_AziendaRow
                        drAzienda = DS_SAU_Azienda.DT_Dettaglio_SAU_Utilizzata_Azienda.NewDT_Dettaglio_SAU_Utilizzata_AziendaRow

                        drAzienda.SpecieVegetale = Matrice_SAUInUsoxSpecie_Azienda(0, i)
                        drAzienda.Sup = Format(Matrice_SAUInUsoxSpecie_Azienda(1, i), "0.0000")

                        'aggiungo la nuova riga al dataset
                        DS_SAU_Azienda.DT_Dettaglio_SAU_Utilizzata_Azienda.Rows.Add(drAzienda)

                    Next

                Else

                    rptStampa.Section4.SectionFormat.EnableSuppress = True

                End If

            Else

                rptStampa.Section4.SectionFormat.EnableSuppress = True

            End If


            If Aziende_Controllate <> "" Then

                For i = 0 To UBound(Aziende)
                    Recupera_SuperficieInUso_Impresa(Aziende(i), 0, Sup_ControllataInUso, Server, QS_Data)
                    Sup_ControllataInUso += Sup_ControllataInUso
                    Sup_ControllateInUso += Sup_ControllataInUso
                Next

                'Recupera_SuperficieInUso_Impresa(Aziende_Controllate, 0, Sup_ControllataInUso, Server, QS_Data)

            End If

            drR.sau_inuso_controllate = Format(Sup_ControllateInUso, "0.0000")

            drR.sau_inuso_tot = Format(Sup_AziendaInUso + Sup_ControllateInUso, "0.0000")


            '--------------------------------------
            ' Riempio il Dataset DS_SAU_Controllate
            '--------------------------------------

            If Aziende_Controllate <> "" Then

                If Not Matrice_SAUInUsoxSpecie_Azienda(0, 0) Is Nothing Then

                    For i = 0 To UBound(Matrice_SAUInUsoxSpecie_Azienda, 2)

                        'creo una nuova riga del dataset
                        Dim drControllate As DS_Dettaglio_SAU_Utilizzata_Controllate.DT_Dettaglio_SAU_Utilizzata_ControllateRow
                        drControllate = DS_SAU_Controllate.DT_Dettaglio_SAU_Utilizzata_Controllate.NewDT_Dettaglio_SAU_Utilizzata_ControllateRow

                        drControllate.SpecieVegetale = Matrice_SAUInUsoxSpecie_Azienda(0, i)
                        drControllate.Sup = Format(Matrice_SAUInUsoxSpecie_Azienda(1, i), "0.0000")

                        'aggiungo la nuova riga al dataset
                        DS_SAU_Controllate.DT_Dettaglio_SAU_Utilizzata_Controllate.Rows.Add(drControllate)

                    Next

                Else

                    rptStampa.Section6.SectionFormat.EnableSuppress = True

                End If

            Else

                rptStampa.Section6.SectionFormat.EnableSuppress = True

            End If


            '-----------------------------------------------------------------------
            ' SUPERFICIE SAU INUTILIZZATA ( Azienda - Controllate - Somma )
            '-----------------------------------------------------------------------

            drR.sau_inutilizzata_azienda = Format(SAU_Totale_Azienda - Sup_AziendaInUso, "0.0000")

            drR.sau_inutilizzata_controllate = Format(SAU_Totale_Controllate - Sup_ControllataInUso, "0.0000")

            drR.sau_inutilizzata_tot = Format((SAU_Totale_Azienda - Sup_AziendaInUso) + (SAU_Totale_Controllate - Sup_ControllataInUso), "0.0000")


            'aggiungo la nuova riga al dataset
            DSRiepilogoUtilizzoSuperfici.DT_RiepilogoUtilizzoSuperfici.Rows.Add(drR)

        Else

            '-----------------------------------------------------------------------
            ' SUPERFICIE CATASTALE + SUPERFICIE SAU ( Azienda - Controllate - Somma )
            '-----------------------------------------------------------------------

            objCentriAz.Recupera_Superfici_CentroAziendale(Qs_Piva, Qs_Sa_Cod, Sup_Azienda, Nothing, Nothing, Nothing, SAU_Totale_Azienda, Nothing, Nothing, Nothing, QS_Data, objParametri_Server)

            drR.sup_azienda = Format(Sup_Azienda, "0.0000")

            drR.sau_azienda = Format(SAU_Totale_Azienda, "0.0000")


            Sup_Controllate = 0
            SAU_Totale_Controllate = 0

            drR.sup_controllate = Format(Sup_Controllate, "0.0000")

            drR.sau_controllate = Format(SAU_Totale_Controllate, "0.0000")

            drR.sup_tot = Format(Sup_Azienda + Sup_Controllate, "0.0000")
            drR.sau_tot = Format(SAU_Totale_Azienda + SAU_Totale_Controllate, "0.0000")


            '-----------------------------------------------------------------------
            ' SUPERFICIE SAU IN USO ( Azienda - Controllate - Somma )
            '-----------------------------------------------------------------------

            Recupera_SuperficieInUso_Impresa(Qs_Piva, CInt(Qs_Sa_Cod), Sup_AziendaInUso, Server, QS_Data)

            drR.sau_inuso_azienda = Format(Sup_AziendaInUso, "0.0000")

            '--------------------------------------
            ' Riempio il Dataset DS_SAU_Azienda
            '--------------------------------------

            If Sup_AziendaInUso <> 0 Then

                If Not Matrice_SAUInUsoxSpecie_Azienda(0, 0) Is Nothing Then

                    For i = 0 To UBound(Matrice_SAUInUsoxSpecie_Azienda, 2)

                        'creo una nuova riga del dataset
                        Dim drAzienda As DS_Dettaglio_SAU_Utilizzata_Azienda.DT_Dettaglio_SAU_Utilizzata_AziendaRow
                        drAzienda = DS_SAU_Azienda.DT_Dettaglio_SAU_Utilizzata_Azienda.NewDT_Dettaglio_SAU_Utilizzata_AziendaRow

                        drAzienda.SpecieVegetale = Matrice_SAUInUsoxSpecie_Azienda(0, i)
                        drAzienda.Sup = Format(Matrice_SAUInUsoxSpecie_Azienda(1, i), "0.0000")

                        'aggiungo la nuova riga al dataset
                        DS_SAU_Azienda.DT_Dettaglio_SAU_Utilizzata_Azienda.Rows.Add(drAzienda)

                    Next

                Else

                    rptStampa.Section4.SectionFormat.EnableSuppress = True

                End If

            Else

                rptStampa.Section4.SectionFormat.EnableSuppress = True

            End If


            drR.sau_inuso_controllate = Format(Sup_ControllataInUso, "0.0000")

            drR.sau_inuso_tot = Format(Sup_AziendaInUso + Sup_ControllataInUso, "0.0000")


            '--------------------------------------
            ' Dataset DS_SAU_Controllate
            '--------------------------------------

            rptStampa.Section6.SectionFormat.EnableSuppress = True


            '-----------------------------------------------------------------------
            ' SUPERFICIE SAU INUTILIZZATA ( Azienda - Controllate - Somma )
            '-----------------------------------------------------------------------

            drR.sau_inutilizzata_azienda = Format(SAU_Totale_Azienda - Sup_AziendaInUso, "0.0000")

            drR.sau_inutilizzata_controllate = Format(SAU_Totale_Controllate - Sup_ControllataInUso, "0.0000")

            drR.sau_inutilizzata_tot = Format((SAU_Totale_Azienda - Sup_AziendaInUso) + (SAU_Totale_Controllate - Sup_ControllataInUso), "0.0000")


            'aggiungo la nuova riga al dataset
            DSRiepilogoUtilizzoSuperfici.DT_RiepilogoUtilizzoSuperfici.Rows.Add(drR)

        End If

    End Sub


    '################################################################################
    Public Sub Recupera_SuperficieInUso_Impresa(
                                        ByVal Piva As String,
                                        ByVal Sa_Cod As Integer,
                                        ByRef Sup_Totale As Double,
                                        ByRef objServer As Object,
                                        Optional ByVal DataRecupero As Date = #1/1/1900#)

        '----- Variabili

        Dim Codice_Anagrafe_R As New Codice_Anagrafe_R
        Dim objSQL As New Codex_Utility.Sql
        'Dim Rs As ADODB.Recordset
        Dim StrSQL As String
        Dim Messaggio As String
        Dim i, j As Integer
        Dim PrimaVolta As Boolean = True
        Dim Dt_Colture As DataTable
        Dim Dt_Destinazioni As DataTable

        Dim handleRiepilogoSuperfici As New RiepilogoSuperfici

        '----- Se il giorno di recupero non e' impostato, considero il giorno corrente

        If DataRecupero = #1/1/1900# Then
            DataRecupero = Date.Today
        End If

        Dim Sup_Destinazione As Double

        '§TO DO
        'spostare le query sul core AgronicaCoreStampeDAL.RiepilogoSuperfici
        'SOSTITUIRE RECORDSET CON DT

        '-------------------------------------------
        'COLTURE
        '-------------------------------------------

        'Genero la query SQL

        'StrSQL = ""
        'StrSQL = StrSQL & " SELECT  Appezzamento.PIVA, Appezzamento.SA_COD,  "
        'StrSQL = StrSQL & " Reg_Impianti.cul_cod, SpecieVegetali.Veg_Des, SpecieVegetali.Veg_Cod, "
        'StrSQL = StrSQL & " Appezzamento.APPEZZA, Appezzamento.APP_NOME, Reg_Impianti.sup_imp AS SUP_APP, "
        'StrSQL = StrSQL & " Appezzamento.Validita_Inizio, Appezzamento.Validita_Fine, "
        'StrSQL = StrSQL & " Reg_Impianti.Validita_Inizio, Reg_Impianti.Validita_Fine,  "

        ''Questa parte di query mi restituisce il numero di specie distinte utilizzate nell'impresa

        'StrSQL = StrSQL & " (SELECT  count (distinct SpecieVegetali.Veg_Cod) "
        'StrSQL = StrSQL & "  FROM Appezzamento, Reg_Impianti, SpecieVegetali, Cultivar "
        'StrSQL = StrSQL & " WHERE   Appezzamento.PIVA = '" & SQL_SaveText(Piva) & "' "

        'If Sa_Cod <> 0 Then
        '    StrSQL = StrSQL & " AND   Appezzamento.sa_cod = " & SQL_SaveNum(Sa_Cod) & " "
        'End If

        'StrSQL = StrSQL & " AND     Appezzamento.Validita_Inizio <= " & SQL_SaveDate(DataRecupero) & " "
        'StrSQL = StrSQL & " AND     Appezzamento.Validita_Fine >= " & SQL_SaveDate(DataRecupero) & " "
        'StrSQL = StrSQL & " AND     Reg_Impianti.Validita_Inizio <= " & SQL_SaveDate(DataRecupero) & " "
        'StrSQL = StrSQL & " AND     Reg_Impianti.Validita_Fine >= " & SQL_SaveDate(DataRecupero) & " "
        'StrSQL = StrSQL & " AND Reg_Impianti.PIVA=Appezzamento.PIVA"
        'StrSQL = StrSQL & " AND Reg_Impianti.SA_COD=Appezzamento.SA_COD"
        'StrSQL = StrSQL & " AND Reg_Impianti.appezza=Appezzamento.appezza"
        'StrSQL = StrSQL & " AND Reg_Impianti.cul_cod=Cultivar.cul_cod"
        'StrSQL = StrSQL & " AND SpecieVegetali.veg_cod=Cultivar.veg_cod"
        'StrSQL = StrSQL & " AND Reg_Impianti.cul_cod <> 0"
        'StrSQL = StrSQL & " )  as Numero_Specie "

        'StrSQL = StrSQL & " FROM Appezzamento, Reg_Impianti, SpecieVegetali, Cultivar "

        'StrSQL = StrSQL & " WHERE   Appezzamento.PIVA = '" & SQL_SaveText(Piva) & "' "

        'If Sa_Cod <> 0 Then
        '    StrSQL = StrSQL & " AND   Appezzamento.sa_cod = " & SQL_SaveNum(Sa_Cod) & " "
        'End If

        'StrSQL = StrSQL & " AND   Reg_Impianti.cul_cod <> 0 "
        'StrSQL = StrSQL & " AND   Appezzamento.Validita_Inizio <= " & SQL_SaveDate(DataRecupero) & " "
        'StrSQL = StrSQL & " AND   Appezzamento.Validita_Fine >= " & SQL_SaveDate(DataRecupero) & " "
        'StrSQL = StrSQL & " AND   Reg_Impianti.Validita_Inizio <= " & SQL_SaveDate(DataRecupero) & " "
        'StrSQL = StrSQL & " AND   Reg_Impianti.Validita_Fine >= " & SQL_SaveDate(DataRecupero) & " "

        'StrSQL = StrSQL & " AND Reg_Impianti.PIVA=Appezzamento.PIVA "
        'StrSQL = StrSQL & " AND Reg_Impianti.SA_COD=Appezzamento.SA_COD "
        'StrSQL = StrSQL & " AND Reg_Impianti.appezza=Appezzamento.appezza "
        'StrSQL = StrSQL & " AND Reg_Impianti.cul_cod=Cultivar.cul_cod "
        'StrSQL = StrSQL & " AND SpecieVegetali.veg_cod=Cultivar.veg_cod "

        'StrSQL = StrSQL & " order by SpecieVegetali.Veg_Des "

        ''Recupero il recordset
        'Rs = objSQL.SqlSelect(objParametri_Server.StringaConnessione,
        '                      Session("ASG_Connessione_Server"),
        '                      StrSQL,
        '                      0,
        '                      Messaggio)

        ''Elimino l'oggetto
        'objSQL = Nothing

        Dt_Colture = handleRiepilogoSuperfici.Recupera_Superfici_Utilizzo_Colture(Piva, Sa_Cod, DataRecupero, objParametri_Server)

        If Dt_Colture IsNot Nothing AndAlso Dt_Colture.Rows.Count > 0 Then

            Numero_Specie_Azienda = Dt_Colture.Rows.Item(0).Field(Of Integer)("Numero_Specie")
            ReDim Matrice_SAUInUsoxSpecie_Azienda(1, Numero_Specie_Azienda - 1)

            Dim colMatrice As Integer = 0

            For index = 0 To Dt_Colture.Rows.Count - 1

                'Aggiorno il totale
                Sup_Totale += Dt_Colture.Rows.Item(index).Field(Of Double)("sup_app")

                If PrimaVolta = True Then

                    Matrice_SAUInUsoxSpecie_Azienda(0, colMatrice) = Dt_Colture.Rows.Item(index).Field(Of String)("Veg_Des")
                    Matrice_SAUInUsoxSpecie_Azienda(1, colMatrice) = Dt_Colture.Rows.Item(index).Field(Of Double)("sup_app")

                    PrimaVolta = False

                Else
                    If Matrice_SAUInUsoxSpecie_Azienda(0, colMatrice - 1) = Dt_Colture.Rows.Item(index).Field(Of String)("Veg_Des") Then

                        'Matrice_SAUInUsoxSpecie_Azienda(0, colMatrice - 1) = Dt_Colture.Rows.Item(index).Field(Of String)("Veg_Des")
                        Matrice_SAUInUsoxSpecie_Azienda(1, colMatrice - 1) = Matrice_SAUInUsoxSpecie_Azienda(1, colMatrice - 1) + Dt_Colture.Rows.Item(index).Field(Of Double)("sup_app")
                        colMatrice -= 1
                    Else

                        Matrice_SAUInUsoxSpecie_Azienda(0, colMatrice) = Dt_Colture.Rows.Item(index).Field(Of String)("Veg_Des")
                        Matrice_SAUInUsoxSpecie_Azienda(1, colMatrice) = Dt_Colture.Rows.Item(index).Field(Of Double)("sup_app")

                    End If

                End If

                colMatrice += 1
            Next

        Else

            Array.Clear(Matrice_SAUInUsoxSpecie_Azienda, 0, Matrice_SAUInUsoxSpecie_Azienda.Length)
            Numero_Specie_Azienda = 0
            Sup_Totale = 0

        End If

        'Verifico la presenza di errori
        'If Not IsNothing(Messaggio) Then

        '    'ERRORE
        'Else

        '    If Rs.State <> 0 Then

        '        If Not Rs.EOF Then

        '            Numero_Specie_Azienda = CInt(Rs("Numero_Specie").Value)

        '            ReDim Preserve Matrice_SAUInUsoxSpecie_Azienda(1, Numero_Specie_Azienda - 1)

        '            i = 0

        '            'Ciclo sugli elementi selezionati
        '            Do While Not Rs.EOF

        '                'Aggiorno il totale
        '                Sup_Totale += CDbl(Rs("sup_app").Value)

        '                If PrimaVolta = True Then

        '                    Matrice_SAUInUsoxSpecie_Azienda(0, i) = Rs.Fields("Veg_Des").Value
        '                    Matrice_SAUInUsoxSpecie_Azienda(1, i) = Rs("sup_app").Value

        '                    PrimaVolta = False

        '                Else
        '                    If Matrice_SAUInUsoxSpecie_Azienda(0, i - 1) = Rs.Fields("Veg_Des").Value Then

        '                        Matrice_SAUInUsoxSpecie_Azienda(0, i - 1) = Rs.Fields("Veg_Des").Value
        '                        Matrice_SAUInUsoxSpecie_Azienda(1, i - 1) = Matrice_SAUInUsoxSpecie_Azienda(1, i - 1) + Rs("sup_app").Value
        '                        i -= 1
        '                    Else

        '                        Matrice_SAUInUsoxSpecie_Azienda(0, i) = Rs.Fields("Veg_Des").Value
        '                        Matrice_SAUInUsoxSpecie_Azienda(1, i) = Rs("sup_app").Value

        '                    End If

        '                End If

        '                i += 1

        '                'Prossimo record
        '                Rs.MoveNext()

        '            Loop

        '        Else

        '            Matrice_SAUInUsoxSpecie_Azienda.Clear(Matrice_SAUInUsoxSpecie_Azienda, 0, Matrice_SAUInUsoxSpecie_Azienda.Length)
        '            Numero_Specie_Azienda = 0

        '        End If

        '        Rs.Close()

        '    Else
        '        Sup_Totale = 0
        '    End If

        '    'Elimino il recordset
        '    Rs = Nothing

        'End If


        '-------------------------------------------
        'VIVAI, LAGHETTI, BOSCHETTI...
        '-------------------------------------------

        'Sup_Destinazione = 0

        'StrSQL = ""
        'StrSQL = StrSQL & " SELECT  DISTINCT Appezzamento.PIVA, Appezzamento.SA_COD,  "
        'StrSQL = StrSQL & " Reg_Impianti.cul_cod,  "
        'StrSQL = StrSQL & " Appezzamento.APPEZZA, Appezzamento.APP_NOME, Reg_Impianti.sup_imp AS SUP_APP, "
        'StrSQL = StrSQL & " Appezzamento.Validita_Inizio, Appezzamento.Validita_Fine, "
        'StrSQL = StrSQL & " Reg_Impianti.Validita_Inizio, Reg_Impianti.Validita_Fine,  "
        'StrSQL = StrSQL & " Reg_Impianti_Codici.id_cod "

        'StrSQL = StrSQL & " FROM Reg_Impianti_Codici INNER JOIN"
        'StrSQL = StrSQL & " Appezzamento INNER JOIN"
        'StrSQL = StrSQL & " Reg_Impianti ON Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.SA_COD = Reg_Impianti.SA_COD AND "
        'StrSQL = StrSQL & " Appezzamento.APPEZZA = Reg_Impianti.APPEZZA ON Reg_Impianti_Codici.PIVA = Reg_Impianti.PIVA AND "
        'StrSQL = StrSQL & " Reg_Impianti_Codici.sa_cod = Reg_Impianti.SA_COD AND Reg_Impianti_Codici.appezza = Reg_Impianti.APPEZZA AND "
        'StrSQL = StrSQL & " Reg_Impianti_Codici.Id_Reg = Reg_Impianti.ID_REG"

        'StrSQL = StrSQL & " WHERE   Appezzamento.PIVA = '" & SQL_SaveText(Piva) & "' "

        'If Sa_Cod <> 0 Then
        '    StrSQL = StrSQL & " AND   Appezzamento.sa_cod = " & SQL_SaveNum(Sa_Cod) & " "
        'End If

        'StrSQL = StrSQL & " AND   Reg_Impianti.cul_cod = 0 "
        'StrSQL = StrSQL & " AND   Reg_Impianti_Codici.id_cod >= 3000 "
        'StrSQL = StrSQL & " AND   Reg_Impianti_Codici.id_cod < 4000 "
        ''Escludo le tare improduttive '3015'
        'StrSQL = StrSQL & " AND   Reg_Impianti_Codici.id_cod <> 3015 "

        'StrSQL = StrSQL & " AND   Appezzamento.Validita_Inizio <= " & SQL_SaveDate(DataRecupero) & " "
        'StrSQL = StrSQL & " AND   Appezzamento.Validita_Fine >= " & SQL_SaveDate(DataRecupero) & " "
        'StrSQL = StrSQL & " AND   Reg_Impianti.Validita_Inizio <= " & SQL_SaveDate(DataRecupero) & " "
        'StrSQL = StrSQL & " AND   Reg_Impianti.Validita_Fine >= " & SQL_SaveDate(DataRecupero) & " "


        'Recupero il recordset
        'Dt_Destinazioni = objSQL.SqlSelect(objParametri_Server.StringaConnessione,
        '                                    Session("ASG_Connessione_Server"),
        '                                    StrSQL,
        '                                    1,
        '                                    Messaggio)

        Dt_Destinazioni = handleRiepilogoSuperfici.Recupera_Superfici_Utilizzo_TerreniNudi(Piva, Sa_Cod, DataRecupero, objParametri_Server)

        If Not Dt_Destinazioni Is Nothing AndAlso Dt_Destinazioni.Rows.Count > 0 Then

            Dim objSqlDis As New Codex_Utility_Sql_DistinctOnDT

            'seleziono i tipi distinti di destinazione uso
            Dim strId_Cod() As String = objSqlDis.SelectDistinct(Dt_Destinazioni, "id_cod")

            If Not IsNothing(strId_Cod) Then

                ReDim Preserve Matrice_SAUInUsoxSpecie_Azienda(1, Numero_Specie_Azienda + strId_Cod.Length - 1)

                For i = 0 To strId_Cod.Length - 1

                    Sup_Destinazione = 0

                    Dim Righe() As DataRow = Dt_Destinazioni.Select("id_cod=" & strId_Cod(i))

                    For j = 0 To Righe.Length - 1
                        Sup_Destinazione = Sup_Destinazione + Righe(j).Item("sup_app")
                    Next

                    Matrice_SAUInUsoxSpecie_Azienda(0, i + Numero_Specie_Azienda) = Codice_Anagrafe_R.CodiceAnagrafeDes_from_CodiceAnagrafeCod(CInt(strId_Cod(i)), objParametri_Server)
                    Matrice_SAUInUsoxSpecie_Azienda(1, i + Numero_Specie_Azienda) = Sup_Destinazione

                    Sup_Totale += Sup_Destinazione

                Next

            End If

        End If

        '-------------------------------------------
        'TERRENI NUDI...
        '-------------------------------------------


        'Elimino l'oggetto
        'objSQL = Nothing


        '-------------------------------------------

    End Sub



End Class