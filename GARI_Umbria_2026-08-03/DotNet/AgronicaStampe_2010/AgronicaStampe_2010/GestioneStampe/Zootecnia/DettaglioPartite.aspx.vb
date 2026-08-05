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
Imports AgronicaCoreZooDAL

Public Class DettaglioPartite
    Inherits System.Web.UI.Page

    Private rptDettaglioPartite As Rpt_DettaglioPartite
    Private rptDettaglioCarichi As Rpt_Carichi
    Private rptDettaglioScarichi As Rpt_Scarichi

    Private Qs_Piva As String
    Private Qs_DataInizio As Date
    Private Qs_DataFine As Date
    Private QS_Centro As String
    Private Qs_Stalla As String
    Private Qs_Lotto As String
    Private Qs_Lotto2 As String
    Private QS_Sesso As String
    Private Qs_Razza As String

    Private QS_Fatturazione As String
    Private QS_Provenienza As String

    Private Log_Errori As String

    Private objParametri_Server As New AgronicaCoreParametri
    Private objParametri_Utenti As New AgronicaCoreParametri

    Private nomeDocIdentifPratica As String = ""

    Private Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
        rptDettaglioPartite = New Rpt_DettaglioPartite
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

        QS_Centro = Stringa_Decodifica(Request.QueryString("cen"),
                                   AgroKey_EncoderDecoder,
                                   Server)

        Qs_Stalla = Stringa_Decodifica(Request.QueryString("sta"),
                                   AgroKey_EncoderDecoder,
                                   Server)

        Qs_Lotto = Stringa_Decodifica(Request.QueryString("lot"),
                                   AgroKey_EncoderDecoder,
                                   Server)

        Qs_Lotto2 = Stringa_Decodifica(Request.QueryString("lot2"),
                                   AgroKey_EncoderDecoder,
                                   Server)

        QS_Sesso = Stringa_Decodifica(Request.QueryString("ses"),
                                      AgroKey_EncoderDecoder,
                                      Server)

        Qs_Razza = Stringa_Decodifica(Request.QueryString("raz"),
                                      AgroKey_EncoderDecoder,
                                      Server)

        QS_Fatturazione = Stringa_Decodifica(Request.QueryString("ff"),
                                                       AgroKey_EncoderDecoder,
                                                       Server)

        QS_Provenienza = Stringa_Decodifica(Request.QueryString("fp"),
                                                       AgroKey_EncoderDecoder,
                                                       Server)

        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim Nome_Documento As String = "DettaglioPartite"
        Dim IdentificazioneDocumento As String = ""

        If Not Me.IsPostBack Then

            Try

                '--------------------------------------------
                ' LETTURA DEI DATI
                '--------------------------------------------
                Stampa_DettaglioPartite()

            Catch ex As Exception
                Log_Errori &= "- Lettura dati: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf
            End Try


            Dim reportTemporaneo As String = CrystalHelper.getFileReportTemporaneo()
            Try
                Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                Dim Sottocartella As String = objCatDoc.Sottocartella(enum_CategorieDocumenti.RegistriCampagna, "", "", objParametri_Server)

                Nome_Documento &= " " & nomeDocIdentifPratica
                Dim Nome_Documento_Estensione = Nome_Documento & ".pdf"

                'Salvo il report in formato PDF
                Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                Dim pathPdfGenerato = objGestFile.SalvaReportPdf(rptDettaglioPartite,
                                               enum_CategorieDocumenti.RegistriCampagna,
                                               Sottocartella,
                                               Nome_Documento_Estensione,
                                               objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                'Elimino il file perché non necessario, verrà generato per l'utente nel VisualizzatoreReport
                IO.File.Delete(pathPdfGenerato)

                rptDettaglioPartite.SaveAs(reportTemporaneo, True)
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

    Private Sub Stampa_DettaglioPartite()
        'DataSet Report
        Dim dsDettaglio As New DS_DettaglioPartite()
        Dim dsCarichi As New DS_Carichi()
        Dim dsScarichi As New DS_Scarichi()

        'Datatable Query
        Dim dtPartite As New DataTable
        Dim dtCarichi As New DataTable
        Dim dtScarichi As New DataTable

        Dim dtCostiAlimentazione As New DataTable
        Dim dtCostiFarmaci As New DataTable

        'Oggetti BIZ e DAL
        Dim handleDettaglioPartite As New AgronicaCoreZooDAL.Report_Partite_R()

        Dim dictionaryPesipagati As New Dictionary(Of String, Decimal)
        Dim dictionaryCostoTotale As New Dictionary(Of String, Decimal)
        Dim dictionaryCostiSanitari As New Dictionary(Of String, Decimal)
        Dim dictionaryCostiAlimentari As New Dictionary(Of String, Decimal)
        '-----------------------------------------
        '---- Query di lettura  -------------
        '----------------------------------------- 
        Try

            dtPartite = handleDettaglioPartite.LeggiDettagliPartite(Qs_Piva,
                                                    Qs_DataInizio,
                                                    Qs_DataFine,
                                                    Qs_Lotto,
                                                    Qs_Lotto2,
                                                    QS_Centro,
                                                    Qs_Stalla,
                                                    QS_Fatturazione,
                                                    QS_Provenienza,
                                                    "",
                                                    "",
                                                    objParametri_Server)

            dtCarichi = handleDettaglioPartite.LeggiDettagliCarichi(
                                                    Qs_Lotto,
                                                    Qs_Lotto2,
                                                    QS_Centro,
                                                    Qs_Stalla,
                                                    QS_Fatturazione,
                                                    QS_Provenienza,
                                                    Qs_Razza,
                                                    QS_Sesso,
                                                    "",
                                                    "",
                                                    objParametri_Server)

            dtScarichi = handleDettaglioPartite.LeggiDettagliScarichi(
                                                    Qs_Lotto,
                                                    Qs_Lotto2,
                                                    QS_Centro,
                                                    Qs_Stalla,
                                                    QS_Fatturazione,
                                                    QS_Provenienza,
                                                    Qs_Razza,
                                                    QS_Sesso,
                                                    "",
                                                    "",
                                                    objParametri_Server)

        Catch ex As Exception
            Log_Errori &= "query Stampa_DettagliPartite: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf & vbCrLf
        End Try

        '------------------------------------------------------------
        '---- Impostazione dei dati trovati nei report  -------------
        '------------------------------------------------------------
        Try

            Dim partiteElaborate As New List(Of String)

            If dtPartite.Rows.Count > 0 Then

                partiteElaborate.AddRange(dtPartite.AsEnumerable().Select(Function(r) r.Field(Of String)("Codice_Distinta")).Distinct())

                For Each dr As DataRow In dtPartite.Rows

                    Dim drDettaglio As DS_DettaglioPartite.DT_Dati_PartitaRow = dsDettaglio.DT_Dati_Partita.NewRow

                    drDettaglio.Codice_Distinta = dr.Item("Codice_Distinta").ToString
                    drDettaglio.Sesso = dr.Item("Moda_Sesso").ToString
                    drDettaglio.Chiusa = If(CInt(dr.Item("Capi_Totali").ToString) = CInt(If(IsDBNull(dr.Item("scarichi")), 0, dr.Item("scarichi").ToString)), "SI", "NO")
                    drDettaglio.Razza = dr.Item("Moda_RAZ_DES").ToString
                    drDettaglio.Descrizione = ""

                    '-----------------------------------------------------------------------------
                    dsDettaglio.DT_Dati_Partita.Rows.Add(drDettaglio)
                    '-----------------------------------------------------------------------------

                Next

            End If

            If dtCarichi.Rows.Count > 0 Then
                For Each dr As DataRow In dtCarichi.Rows
                    Dim drCarichi As DS_Carichi.DT_CarichiRow = dsCarichi.DT_Carichi.NewRow
                    drCarichi.Codice_Distinta = dr.Item("Codice_Distinta").ToString
                    drCarichi.Data_Operazione = dr.Item("Data_Operazione").ToString
                    drCarichi.Capi = dr.Item("Capi").ToString
                    drCarichi.Sesso = dr.Item("Sesso").ToString
                    drCarichi.Costo_Totale = If(IsDBNull(dr.Item("Costo_Totale")), 0, dr.Item("Costo_Totale").ToString)
                    drCarichi.CostoAlKg = If(IsDBNull(dr.Item("CostoAlKg")), 0, dr.Item("CostoAlKg").ToString)
                    drCarichi.Peso_Pagato = If(IsDBNull(dr.Item("Peso_Pagato")), 0, dr.Item("Peso_Pagato").ToString)
                    drCarichi.Peso_Arrivo = If(IsDBNull(dr.Item("Peso_Arrivo")), 0, dr.Item("Peso_Arrivo").ToString)
                    drCarichi.Calo_Perc_Calcolato = If(IsDBNull(dr.Item("Calo_Perc_Calcolato")), 0, dr.Item("Calo_Perc_Calcolato").ToString)
                    drCarichi.Fornitore_Fatturazione = If(IsDBNull(dr.Item("Fornitore_Fatturazione")), "", dr.Item("Fornitore_Fatturazione").ToString)
                    drCarichi.Fornitore_Provenienza = If(IsDBNull(dr.Item("Fornitore_Provenienza")), "", dr.Item("Fornitore_Provenienza").ToString)
                    drCarichi.Allevamento_Entrata = If(IsDBNull(dr.Item("Allevamento_Entrata")), "", dr.Item("Allevamento_Entrata").ToString)
                    drCarichi.Costo_Totale_Medio = If(IsDBNull(dr.Item("Costo_Totale_Medio")), 0, dr.Item("Costo_Totale_Medio").ToString)
                    drCarichi.Peso_Pagato_Medio = If(IsDBNull(dr.Item("Peso_Pagato_Medio")), 0, dr.Item("Peso_Pagato_Medio").ToString)
                    drCarichi.Peso_Arrivo_Medio = If(IsDBNull(dr.Item("Peso_Arrivo_Medio")), 0, dr.Item("Peso_Arrivo_Medio").ToString)

                    '-----------------------------------------------------------------------------
                    dsCarichi.DT_Carichi.Rows.Add(drCarichi)
                    '-----------------------------------------------------------------------------
                Next
            End If

            If partiteElaborate.Count > 0 Then
                dtCostiAlimentazione = handleDettaglioPartite.LeggiCostiAlimentazionePartite(
                                                   partiteElaborate,
                                                   Qs_Piva,
                                                   QS_Centro,
                                                   Qs_Stalla,
                                                   objParametri_Server)

                dtCostiFarmaci = handleDettaglioPartite.LeggiCostiFarmaciPartite(
                                                    partiteElaborate,
                                                    Qs_Piva,
                                                    QS_Centro,
                                                    Qs_Stalla,
                                                    objParametri_Server)
            End If

            If dtScarichi.Rows.Count > 0 Then
                For Each dr As DataRow In dtScarichi.Rows

                    Dim pesoTot As Decimal = 0D
                    Dim CostoTot As Decimal = 0D
                    Dim CostoAlimentare As Decimal = 0D
                    Dim CostoSanitario As Decimal = 0D

                    If dictionaryPesipagati.ContainsKey(CStr(dr.Item("Codice_Distinta"))) Then
                        pesoTot = dictionaryPesipagati(CStr(dr.Item("Codice_Distinta")))
                    Else
                        pesoTot = dtCarichi.AsEnumerable().
                                            Where(Function(r) Not r.IsNull("Codice_Distinta") AndAlso r.Field(Of String)("Codice_Distinta") = CStr(dr.Item("Codice_Distinta"))).
                                            Sum(Function(r)
                                                    If r.IsNull("Peso_Pagato") Then
                                                        Return 0D
                                                    End If
                                                    Dim v As Object = r.Field(Of Object)("Peso_Pagato")
                                                    ' tenta conversione sicura a Decimal
                                                    Dim d As Decimal
                                                    If Decimal.TryParse(Convert.ToString(v), d) Then
                                                        Return d
                                                    Else
                                                        Return 0D
                                                    End If
                                                End Function)
                        dictionaryPesipagati.Add(CStr(dr.Item("Codice_Distinta")), pesoTot)
                    End If

                    If dictionaryCostoTotale.ContainsKey(CStr(dr.Item("Codice_Distinta"))) Then
                        CostoTot = dictionaryCostoTotale(CStr(dr.Item("Codice_Distinta")))
                    Else
                        CostoTot = dtCarichi.AsEnumerable().
                                            Where(Function(r) Not r.IsNull("Codice_Distinta") AndAlso r.Field(Of String)("Codice_Distinta") = CStr(dr.Item("Codice_Distinta"))).
                                            Sum(Function(r)
                                                    If r.IsNull("Costo_Totale") Then
                                                        Return 0D
                                                    End If
                                                    Dim v As Object = r.Field(Of Object)("Costo_Totale")
                                                    ' tenta conversione sicura a Decimal
                                                    Dim d As Decimal
                                                    If Decimal.TryParse(Convert.ToString(v), d) Then
                                                        Return d
                                                    Else
                                                        Return 0D
                                                    End If
                                                End Function)
                        dictionaryCostoTotale.Add(CStr(dr.Item("Codice_Distinta")), CostoTot)
                    End If

                    If dictionaryCostiAlimentari.ContainsKey(CStr(dr.Item("Codice_Distinta"))) Then
                        CostoAlimentare = dictionaryCostiAlimentari(CStr(dr.Item("Codice_Distinta")))
                    Else
                        CostoAlimentare = dtCostiAlimentazione.AsEnumerable().
                                            Where(Function(r) Not r.IsNull("Codice_Distinta") AndAlso r.Field(Of String)("Codice_Distinta") = CStr(dr.Item("Codice_Distinta"))).
                                            Sum(Function(r)
                                                    If r.IsNull("Costo_Alimentare") Then
                                                        Return 0D
                                                    End If
                                                    Dim v As Object = r.Field(Of Object)("Costo_Alimentare")
                                                    ' tenta conversione sicura a Decimal
                                                    Dim d As Decimal
                                                    If Decimal.TryParse(Convert.ToString(v), d) Then
                                                        Return d
                                                    Else
                                                        Return 0D
                                                    End If
                                                End Function)
                        dictionaryCostiAlimentari.Add(CStr(dr.Item("Codice_Distinta")), CostoAlimentare)
                    End If

                    If dictionaryCostiSanitari.ContainsKey(CStr(dr.Item("Codice_Distinta"))) Then
                        CostoSanitario = dictionaryCostiSanitari(CStr(dr.Item("Codice_Distinta")))
                    Else
                        CostoSanitario = dtCostiFarmaci.AsEnumerable().
                                            Where(Function(r) Not r.IsNull("Codice_Distinta") AndAlso r.Field(Of String)("Codice_Distinta") = CStr(dr.Item("Codice_Distinta"))).
                                            Sum(Function(r)
                                                    If r.IsNull("Costo_Farmaci") Then
                                                        Return 0D
                                                    End If
                                                    Dim v As Object = r.Field(Of Object)("Costo_Farmaci")
                                                    ' tenta conversione sicura a Decimal
                                                    Dim d As Decimal
                                                    If Decimal.TryParse(Convert.ToString(v), d) Then
                                                        Return d
                                                    Else
                                                        Return 0D
                                                    End If
                                                End Function)
                        dictionaryCostiSanitari.Add(CStr(dr.Item("Codice_Distinta")), CostoSanitario)
                    End If

                    Dim drScarichi As DS_Scarichi.DT_ScarichiRow = dsScarichi.DT_Scarichi.NewRow
                    drScarichi.Codice_Distinta = If(IsDBNull(dr.Item("Codice_Distinta")), "", dr.Item("Codice_Distinta").ToString)
                    drScarichi.Data_Operazione = If(IsDBNull(dr.Item("Data_Operazione")), "", dr.Item("Data_Operazione").ToString)
                    drScarichi.Capi = If(IsDBNull(dr.Item("Capi")), 0, dr.Item("Capi").ToString)
                    drScarichi.Sesso = If(IsDBNull(dr.Item("Sesso")), "", dr.Item("Sesso").ToString)
                    drScarichi.Ricavo_Totale = If(IsDBNull(dr.Item("Ricavo_Totale")), 0, dr.Item("Ricavo_Totale").ToString)
                    drScarichi.Presenze = If(IsDBNull(dr.Item("Presenze")), 0, dr.Item("Presenze").ToString)
                    drScarichi.Peso_Vendita = If(IsDBNull(dr.Item("Peso_Vendita_Totale")), 0, dr.Item("Peso_Vendita_Totale").ToString)
                    drScarichi.Peso_Netto_Vendita = If(IsDBNull(dr.Item("Peso_Netto_Totale")), 0, dr.Item("Peso_Netto_Totale").ToString)
                    drScarichi.RicavoAlKg = If(IsDBNull(dr.Item("RicavoAlKg")), 0, dr.Item("RicavoAlKg").ToString)
                    drScarichi.Tipologia = If(IsDBNull(dr.Item("Tipologia")), "", dr.Item("Tipologia").ToString)
                    drScarichi.Macello = If(IsDBNull(dr.Item("Macello")), "", dr.Item("Macello").ToString)
                    drScarichi.Ricavo_Totale_Medio = If(IsDBNull(dr.Item("Ricavo_Medio")), 0, dr.Item("Ricavo_Medio").ToString)
                    drScarichi.Peso_Vendita_Medio = If(IsDBNull(dr.Item("Peso_Vendita_Medio")), 0, dr.Item("Peso_Vendita_Medio").ToString)
                    drScarichi.Peso_Netto_Medio = If(IsDBNull(dr.Item("Peso_Netto_Medio")), 0, dr.Item("Peso_Netto_Medio").ToString)
                    drScarichi.Allevamento_Uscita = If(IsDBNull(dr.Item("Allevamento_Uscita")), "", dr.Item("Allevamento_Uscita").ToString)
                    drScarichi.Costo_Totale = CostoTot 'dr.Item("Costo_Totale").ToString
                    drScarichi.Peso_Pagato = pesoTot 'dr.Item("Peso_Pagato").ToString
                    drScarichi.Fornitore_Provenienza = If(IsDBNull(dr.Item("Fornitore_Provenienza")), "", dr.Item("Fornitore_Provenienza").ToString)
                    drScarichi.Fornitore_Fatturazione = If(IsDBNull(dr.Item("Fornitore_Fatturazione")), "", dr.Item("Fornitore_Fatturazione").ToString)
                    drScarichi.Costo_Alimentare = CostoAlimentare
                    drScarichi.Costo_Sanitario = CostoSanitario

                    '-----------------------------------------------------------------------------
                    dsScarichi.DT_Scarichi.Rows.Add(drScarichi)
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
            rptDettaglioPartite.SetDataSource(dsDettaglio)
            '-----------------------------------------------------------------------------

            rptDettaglioPartite.OpenSubreport("Rpt_Carichi.rpt").SetDataSource(dsCarichi)

            rptDettaglioPartite.OpenSubreport("Rpt_Scarichi.rpt").SetDataSource(dsScarichi)

        Catch ex As Exception
            Log_Errori &= "- Aggancio dataset al report: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf
        End Try


        'popolamento caselle di testo fisse del report

        Dim leggiRagSoc As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim cercaCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        Dim leggiRazza As New AgronicaCoreMetaSchemaDAL.Lista_Razze_Animali_R

        ' Recupera la sezione dove si trova la casella di testo 
        Dim sezione1 As CrystalDecisions.CrystalReports.Engine.Section = rptDettaglioPartite.ReportDefinition.Sections("Section1")
        Dim sezione5 As CrystalDecisions.CrystalReports.Engine.Section = rptDettaglioPartite.ReportDefinition.Sections("Section5")

        ' Recupera il TextObject
        Dim txtPivaAzienda As CrystalDecisions.CrystalReports.Engine.TextObject = CType(sezione5.ReportObjects("PivaAzienda"), CrystalDecisions.CrystalReports.Engine.TextObject)
        Dim txtDataDa As CrystalDecisions.CrystalReports.Engine.TextObject = CType(sezione1.ReportObjects("DataDa"), CrystalDecisions.CrystalReports.Engine.TextObject)
        Dim txtDataA As CrystalDecisions.CrystalReports.Engine.TextObject = CType(sezione1.ReportObjects("DataA"), CrystalDecisions.CrystalReports.Engine.TextObject)
        Dim txtPartitaDa As CrystalDecisions.CrystalReports.Engine.TextObject = CType(sezione1.ReportObjects("PartitaDa"), CrystalDecisions.CrystalReports.Engine.TextObject)
        Dim txtPartitaA As CrystalDecisions.CrystalReports.Engine.TextObject = CType(sezione1.ReportObjects("PartitaA"), CrystalDecisions.CrystalReports.Engine.TextObject)
        Dim txtStalla As CrystalDecisions.CrystalReports.Engine.TextObject = CType(sezione1.ReportObjects("Stalla"), CrystalDecisions.CrystalReports.Engine.TextObject)
        Dim txtCentro As CrystalDecisions.CrystalReports.Engine.TextObject = CType(sezione1.ReportObjects("Centro"), CrystalDecisions.CrystalReports.Engine.TextObject)
        Dim txtRazza As CrystalDecisions.CrystalReports.Engine.TextObject = CType(sezione1.ReportObjects("Razza"), CrystalDecisions.CrystalReports.Engine.TextObject)
        Dim txtSesso As CrystalDecisions.CrystalReports.Engine.TextObject = CType(sezione1.ReportObjects("Sesso"), CrystalDecisions.CrystalReports.Engine.TextObject)

        ' Imposta il valore
        txtPivaAzienda.Text = leggiRagSoc.RagSoc_from_Piva(Qs_Piva, objParametri_Server)
        txtDataDa.Text = Qs_DataInizio.ToShortDateString
        txtDataA.Text = Qs_DataFine.ToShortDateString
        txtPartitaDa.Text = If(Qs_Lotto <> "", Qs_Lotto, "TUTTE")
        txtPartitaA.Text = If(Qs_Lotto2 <> "", Qs_Lotto2, "TUTTE")
        txtCentro.Text = If(QS_Centro <> "", cercaCentri.Leggi(Qs_Piva, QS_Centro, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server).Rows(0).Item("Sa_Nome").ToString, "TUTTI")
        txtStalla.Text = If(Qs_Stalla <> "", handleDettaglioPartite.StallaDescDaCodice(Qs_Stalla, QS_Centro, Qs_Piva, objParametri_Server), "TUTTE")
        txtSesso.Text = If(QS_Sesso = "", "TUTTI", If(QS_Sesso = "M", "MASCHI", "FEMMINE"))
        txtRazza.Text = If(Qs_Razza <> "",
            leggiRazza.Leggi(Qs_Razza.Split("-").First,
                             Qs_Razza.Split("-")(1),
                             Qs_Razza.Split("-").Last,
                             AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                             "", "", objParametri_Server).Rows(0).Item("RAZ_DES").ToString,
                             "TUTTE")

    End Sub

End Class