Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Gestione_Eccezioni_2015
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreProfilazioneDAL
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreUmaBiz
Imports AgronicaCoreZooDal

Public Class SintesiPartite
    Inherits System.Web.UI.Page

    Private rptSintesiPartite As Rpt_SintesiPartite

    Private Qs_DataInizio As Date
    Private Qs_DataFine As Date

    Private Qs_Piva As String
    Private Qs_Stalla As String
    Private QS_Centro As String
    Private QS_Sesso As String
    Private QS_Razza As String
    Private QS_Stato_Partite As String
    Private QS_Fornitore_Fatturazione As String
    Private QS_Fornitore_Provenienza As String

    Private Log_Errori As String

    Private objParametri_Server As New AgronicaCoreParametri
    Private objParametri_Utenti As New AgronicaCoreParametri

    Private nomeDocIdentifPratica As String = ""

    Private Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
        rptSintesiPartite = New Rpt_SintesiPartite
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Qs_DataInizio = Stringa_Decodifica(Request.QueryString("di"),
                                   AgroKey_EncoderDecoder,
                                   Server)

        Qs_DataFine = Stringa_Decodifica(Request.QueryString("df"),
                                   AgroKey_EncoderDecoder,
                                   Server)

        Qs_Piva = Stringa_Decodifica(Request.QueryString("p"),
                                   AgroKey_EncoderDecoder,
                                   Server)

        Qs_Stalla = Stringa_Decodifica(Request.QueryString("sta"),
                                   AgroKey_EncoderDecoder,
                                   Server)

        QS_Centro = Stringa_Decodifica(Request.QueryString("cen"),
                                   AgroKey_EncoderDecoder,
                                   Server)

        QS_Sesso = Stringa_Decodifica(Request.QueryString("ses"),
                                   AgroKey_EncoderDecoder,
                                   Server)

        QS_Razza = Stringa_Decodifica(Request.QueryString("raz"),
                                   AgroKey_EncoderDecoder,
                                   Server)

        QS_Stato_Partite = Stringa_Decodifica(Request.QueryString("sp"),
                                   AgroKey_EncoderDecoder,
                                   Server)

        If QS_Stato_Partite = "" Then
            QS_Stato_Partite = "1"
        End If

        QS_Fornitore_Fatturazione = Stringa_Decodifica(Request.QueryString("ff"),
                                                       AgroKey_EncoderDecoder,
                                                       Server)

        QS_Fornitore_Provenienza = Stringa_Decodifica(Request.QueryString("fp"),
                                                       AgroKey_EncoderDecoder,
                                                       Server)

        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim Nome_Documento As String = "SintesiPartite"
        Dim IdentificazioneDocumento As String = ""

        If Not Me.IsPostBack Then

            Try

                '--------------------------------------------
                ' LETTURA DEI DATI
                '--------------------------------------------
                Stampa_SintesiPartite()

            Catch ex As Exception
                Log_Errori &= "- Lettura dati: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf
            End Try


            Dim reportTemporaneo As String = CrystalHelper.getFileReportTemporaneo()
            Try
                'Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                'Dim Sottocartella As String = objCatDoc.Sottocartella(enum_CategorieDocumenti.RegistriCampagna, "", "", objParametri_Server)

                'Nome_Documento &= " " & nomeDocIdentifPratica
                'Dim Nome_Documento_Estensione = Nome_Documento & ".pdf"

                ''Salvo il report in formato PDF
                'Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                'Dim pathPdfGenerato = objGestFile.SalvaReportPdf(rptSintesiPartite,
                '                               enum_CategorieDocumenti.RegistriCampagna,
                '                               Sottocartella,
                '                               Nome_Documento_Estensione,
                '                               objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                ''Elimino il file perché non necessario, verrà generato per l'utente nel VisualizzatoreReport
                'IO.File.Delete(pathPdfGenerato)

                rptSintesiPartite.SaveAs(reportTemporaneo, True)
            Catch ex As Exception
                Log_Errori += "- Salvataggio report temporaneo: " + vbCrLf + MessaggioCompletoDataEccezione(ex, True, source:=True) + vbCrLf
            End Try

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------          
            Dim Nome_File_Log As String

            If Log_Errori <> "" Then

                'Log_Errori = Nome_Documento + ", Partita Iva = " & CStr(Qs_Piva) & vbCrLf & vbCrLf & Log_Errori

                Nome_File_Log = "Log_Errori_" & Nome_Documento & ".txt"

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server,
                                                 "CarburantiUMA",
                                                 Nome_File_Log,
                                                 Session("ASG_Utente_Username"),
                                                 Nome_Documento,
                                                 Log_Errori)

            End If


            GC.Collect()


            '-----------------------------------------
            '---- Redirect su VisualizzatoreReport ---
            '-----------------------------------------  
            Response.Redirect(VirtualPathUtility.ToAbsolute("~/GestioneStampe/VisualizzatoreReport.aspx") &
                            "?anteprima=" & Stringa_Codifica("0", AgroKey_EncoderDecoder, Server) &
                            "&tmpReportPath=" + Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server) &
                            "&NomePdf=" & Stringa_Codifica(Nome_Documento, AgroKey_EncoderDecoder, Server))



        End If
    End Sub

    Private Sub Stampa_SintesiPartite()
        'DataSet Report
        Dim dsSintesi As New DS_SintesiPartite()

        'Datatable Query
        Dim dtPartite As New DataTable

        'Oggetti BIZ e DAL
        Dim handleSintesiPartite As New AgronicaCoreZooDAL.Report_Partite_R()

        '-----------------------------------------
        '---- Query di lettura  -------------
        '----------------------------------------- 
        Try

            dtPartite = handleSintesiPartite.LeggiSintesi(Qs_DataInizio,
                                                    Qs_DataFine,
                                                    "",
                                                    Qs_Piva,
                                                    QS_Centro,
                                                    Qs_Stalla,
                                                    QS_Sesso,
                                                    QS_Razza,
                                                    QS_Fornitore_Fatturazione,
                                                    QS_Fornitore_Provenienza,
                                                    "",
                                                    "",
                                                    objParametri_Server,
                                                    statoPartite:=QS_Stato_Partite)

        Catch ex As Exception
            Log_Errori &= "query Stampa_SintesiPartite: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf & vbCrLf
        End Try

        '------------------------------------------------------------
        '---- Impostazione dei dati trovati nei report  -------------
        '------------------------------------------------------------
        Try

            If dtPartite.Rows.Count > 0 Then

                For Each dr As DataRow In dtPartite.Rows

                    Dim drSintesi = dsSintesi.DT_SintesiPartite.NewDT_SintesiPartiteRow

                    drSintesi.Codice_Distinta = dr.Item("Codice_Distinta").ToString
                    drSintesi.Sesso = dr.Item("Sesso").ToString
                    drSintesi.Razza = dr.Item("Razza").ToString
                    drSintesi.Capi_In_Ingresso = dr.Item("Capi_In_Ingresso").ToString
                    drSintesi.Capi_In_Uscita = If(IsDBNull(dr.Item("Capi_In_Uscita")), 0, dr.Item("Capi_In_Uscita").ToString)
                    drSintesi.Primo_Arrivo = CDate(dr.Item("Primo_Arrivo").ToShortDateString).AddDays(DateDiff(DateInterval.Day, CDate(dr.Item("Primo_Arrivo").ToShortDateString), CDate(dr.Item("Ultimo_Arrivo").ToShortDateString)) / 2)
                    drSintesi.Ultimo_Arrivo = CDate(dr.Item("Ultimo_Arrivo").ToShortDateString)
                    drSintesi.Prima_Macellazione = If(IsDBNull(dr.Item("Prima_Macellazione")) OrElse dr.Item("Prima_Macellazione").ToString = "", AGRODATAINIZIO, CDate(dr.Item("Prima_Macellazione").ToShortDateString))
                    drSintesi.Ultima_Macellazione = If(IsDBNull(dr.Item("Ultima_Macellazione")) OrElse dr.Item("Ultima_Macellazione").ToString = "", AGRODATAINIZIO, CDate(dr.Item("Ultima_Macellazione").ToShortDateString))
                    drSintesi.Presenza = If(IsDBNull(dr.Item("Presenza")), 0, dr.Item("Presenza").ToString)
                    drSintesi.Peso_Pagato = If(IsDBNull(dr.Item("Peso_Pagato")), 0, dr.Item("Peso_Pagato").ToString)
                    drSintesi.Peso_Vendita = If(IsDBNull(dr.Item("Peso_Vendita")), 0, dr.Item("Peso_Vendita").ToString)
                    drSintesi.Calo_Acquisto = If(IsDBNull(dr.Item("Calo_Acquisto")), 0, dr.Item("Calo_Acquisto").ToString)
                    drSintesi.Calo_Vendita = If(IsDBNull(dr.Item("Calo_Vendita")), 0, dr.Item("Calo_Vendita").ToString)
                    drSintesi.Incremento_Giornaliero = If(IsDBNull(dr.Item("Incremento_Giornaliero")), 0, dr.Item("Incremento_Giornaliero").ToString)
                    drSintesi.Incremento_Kg = If(IsDBNull(dr.Item("Incremento_Kg")), 0, dr.Item("Incremento_Kg").ToString)
                    drSintesi.CostoAlKg = If(IsDBNull(dr.Item("CostoAlKg")), 0, dr.Item("CostoAlKg").ToString)
                    drSintesi.RicavoAlKg = If(IsDBNull(dr.Item("RicavoAlKg")), 0, dr.Item("RicavoAlKg").ToString)
                    drSintesi.Resa_Presenza = If(IsDBNull(dr.Item("Resa_Presenza")), 0, dr.Item("Resa_Presenza").ToString)
                    drSintesi.ResaAlKg = If(IsDBNull(dr.Item("ResaAlKg")), 0, dr.Item("ResaAlKg").ToString)
                    drSintesi.Costo_Alimentare = If(IsDBNull(dr.Item("Costi_Alimentazione")), 0, dr.Item("Costi_Alimentazione").ToString)
                    drSintesi.Costo_Sanitario = If(IsDBNull(dr.Item("Costi_Farmaci")), 0, dr.Item("Costi_Farmaci").ToString)

                    '-----------------------------------------------------------------------------
                    dsSintesi.DT_SintesiPartite.Rows.Add(drSintesi)
                    '-----------------------------------------------------------------------------

                Next

            End If

        Catch ex As Exception
            Log_Errori &= "- caricamento dataset: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf
        End Try

        '--------------------------------------------
        ' AGGANCIO DATASET AL REPORT
        '--------------------------------------------
        Try
            '-----------------------------------------------------------------------------
            rptSintesiPartite.SetDataSource(dsSintesi)
            '-----------------------------------------------------------------------------
        Catch ex As Exception
            Log_Errori &= "- Aggancio dataset al report: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf
        End Try

        'popolamento caselle di testo fisse del report

        Dim leggiRagSoc As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim leggiRazza As New AgronicaCoreMetaSchemaDAL.Lista_Razze_Animali_R
        Dim cercaCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

        ' Recupera la sezione dove si trova la casella di testo 
        Dim sezione1 As CrystalDecisions.CrystalReports.Engine.Section = rptSintesiPartite.ReportDefinition.Sections("Section1")
        Dim sezione5 As CrystalDecisions.CrystalReports.Engine.Section = rptSintesiPartite.ReportDefinition.Sections("Section5")

        ' Recupera il TextObject
        Dim txtPivaAzienda As CrystalDecisions.CrystalReports.Engine.TextObject = CType(sezione5.ReportObjects("PivaAzienda"), CrystalDecisions.CrystalReports.Engine.TextObject)
        Dim txtDataDa As CrystalDecisions.CrystalReports.Engine.TextObject = CType(sezione1.ReportObjects("DataMAcellazioneDa"), CrystalDecisions.CrystalReports.Engine.TextObject)
        Dim txtDataA As CrystalDecisions.CrystalReports.Engine.TextObject = CType(sezione1.ReportObjects("DataMacellazioneA"), CrystalDecisions.CrystalReports.Engine.TextObject)
        Dim txtStalla As CrystalDecisions.CrystalReports.Engine.TextObject = CType(sezione1.ReportObjects("StallaAa"), CrystalDecisions.CrystalReports.Engine.TextObject)
        Dim txtCentro As CrystalDecisions.CrystalReports.Engine.TextObject = CType(sezione1.ReportObjects("Centro"), CrystalDecisions.CrystalReports.Engine.TextObject)
        Dim txtSesso As CrystalDecisions.CrystalReports.Engine.TextObject = CType(sezione1.ReportObjects("Sesso"), CrystalDecisions.CrystalReports.Engine.TextObject)
        Dim txtRazza As CrystalDecisions.CrystalReports.Engine.TextObject = CType(sezione1.ReportObjects("Razza"), CrystalDecisions.CrystalReports.Engine.TextObject)
        Dim txtStatoPartite As CrystalDecisions.CrystalReports.Engine.TextObject = CType(sezione1.ReportObjects("StatoPartite"), CrystalDecisions.CrystalReports.Engine.TextObject)


        ' Imposta il valore
        txtPivaAzienda.Text = leggiRagSoc.RagSoc_from_Piva(Qs_Piva, objParametri_Server)
        txtDataDa.Text = Qs_DataInizio.ToShortDateString
        txtDataA.Text = Qs_DataFine.ToShortDateString
        txtCentro.Text = If(QS_Centro <> "", cercaCentri.Leggi(Qs_Piva, QS_Centro, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server).Rows(0).Item("Sa_Nome").ToString, "TUTTI")
        txtStalla.Text = If(Qs_Stalla <> "", handleSintesiPartite.StallaDescDaCodice(Qs_Stalla, QS_Centro, Qs_Piva, objParametri_Server), "TUTTE")
        txtSesso.Text = If(QS_Sesso = "", "TUTTI", If(QS_Sesso = "M", "MASCHI", "FEMMINE"))
        txtRazza.Text = If(QS_Razza <> "",
            leggiRazza.Leggi(QS_Razza.Split("-").First,
                             QS_Razza.Split("-")(1),
                             QS_Razza.Split("-").Last,
                             AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                             "", "", objParametri_Server).Rows(0).Item("RAZ_DES").ToString,
                             "TUTTE")
        txtStatoPartite.Text = If(QS_Stato_Partite = "1", "CHIUSE", If(QS_Stato_Partite = "0", "APERTE", "TUTTE"))

    End Sub

End Class