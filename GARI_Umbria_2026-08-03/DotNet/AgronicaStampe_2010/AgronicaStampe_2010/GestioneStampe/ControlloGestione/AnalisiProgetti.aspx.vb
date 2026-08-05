Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreContabDAL
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.IO
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreMetaSchemaDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility

Public Class AnalisiProgetti
    Inherits System.Web.UI.Page

    Private rptStampa As Analisi_Progetti

    Dim Piva As String
    Dim arr_Id_Mov_Det() As Integer
    Dim Log_Errori As String

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri

    '####################################################################
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim Report As Integer

        Piva = Stringa_Decodifica(CStr(Request.QueryString("p")), AgroKey_EncoderDecoder, Server)

        Dim budget_consuntivo As String = Stringa_Decodifica(CStr(Request.QueryString("b_c")), AgroKey_EncoderDecoder, Server)
        Dim costi_ricavi As String = Stringa_Decodifica(CStr(Request.QueryString("c_r")), AgroKey_EncoderDecoder, Server)
        Dim dataDal As String = Stringa_Decodifica(CStr(Request.QueryString("dtDal")), AgroKey_EncoderDecoder, Server)
        Dim dataAl As String = Stringa_Decodifica(CStr(Request.QueryString("dtAl")), AgroKey_EncoderDecoder, Server)
        Dim livelloRottura As String = Stringa_Decodifica(CStr(Request.QueryString("l_r")), AgroKey_EncoderDecoder, Server)
        Dim gestDettaglioVarieta As Boolean = Boolean.Parse(Stringa_Decodifica(CStr(Request.QueryString("dettVarieta")), AgroKey_EncoderDecoder, Server))
        Dim gestDettaglioMacchine As Boolean = Boolean.Parse(Stringa_Decodifica(CStr(Request.QueryString("dettMacchine")), AgroKey_EncoderDecoder, Server))
        Dim gestDettaglioPersonale As Boolean = Boolean.Parse(Stringa_Decodifica(CStr(Request.QueryString("dettPersone")), AgroKey_EncoderDecoder, Server))
        Dim gestDettaglioProdotti As Boolean = Boolean.Parse(Stringa_Decodifica(CStr(Request.QueryString("dettProdotti")), AgroKey_EncoderDecoder, Server))
        Dim gestDettaglioNote As Boolean = Boolean.Parse(Stringa_Decodifica(CStr(Request.QueryString("dettNote")), AgroKey_EncoderDecoder, Server))
        Dim sopprimiDettaglio As Boolean = Boolean.Parse(Stringa_Decodifica(CStr(Request.QueryString("sopprimiDettaglio")), AgroKey_EncoderDecoder, Server))
        Dim dataRifDistinta As String = Stringa_Decodifica(CStr(Request.QueryString("dtRifProgetto")), AgroKey_EncoderDecoder, Server)
        Dim filtro_codice_appezzamento As String = Stringa_Decodifica(CStr(Request.QueryString("filtro_codice_appezzamento")), AgroKey_EncoderDecoder, Server)
        Dim filtro_codice_impianto As String = Stringa_Decodifica(CStr(Request.QueryString("filtro_codice_impianto")), AgroKey_EncoderDecoder, Server)

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        rptStampa = New Analisi_Progetti

        Dim Nome_Documento As String = "AnalisiProgetti"

        If Not Me.IsPostBack Then

            Dim DSAnalisiProgetti As New DS_AnalisiProgetti

            Try
                Dim DT_Intestazione As DataTable
                Dim str_tipostampa1, str_tipostampa2, str_tipostampa3 As String
                Dim str_riga1, str_riga2, str_riga3 As String

                str_tipostampa1 = "Analisi Costi / Ricavi"

                If budget_consuntivo = 0 Then
                    str_tipostampa2 = "Dati a Consuntivo"
                Else
                    str_tipostampa2 = "Dati a Budget"
                End If
                If Not String.IsNullOrWhiteSpace(dataRifDistinta) Then
                    str_tipostampa2 = str_tipostampa2 & " - Progetti riferiti alla data " & dataRifDistinta
                End If

                If Not String.IsNullOrWhiteSpace(dataDal) OrElse Not String.IsNullOrWhiteSpace(dataAl) Then
                    str_tipostampa3 = "Periodo: "
                    If Not String.IsNullOrWhiteSpace(dataDal) Then
                        str_tipostampa3 = str_tipostampa3 & dataDal
                    Else
                        str_tipostampa3 = str_tipostampa3 & AGRODATAINIZIO.ToShortDateString
                    End If
                    str_tipostampa3 = str_tipostampa3 & " - "
                    If Not String.IsNullOrWhiteSpace(dataAl) Then
                        str_tipostampa3 = str_tipostampa3 & dataAl
                    Else
                        str_tipostampa3 = str_tipostampa3 & AGRODATAFINE.ToShortDateString
                    End If
                Else
                    str_tipostampa3 = ""
                End If

                CType(rptStampa.Section1.ReportObjects("TxtTipo1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = str_tipostampa1
                CType(rptStampa.Section1.ReportObjects("TxtTipo2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = str_tipostampa2
                CType(rptStampa.Section1.ReportObjects("TxtTipo3"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = str_tipostampa3

                Dim objInt As New AgronicaCoreStampeDAL.DocContab
                Dim Log As String = ""
                DT_Intestazione = objInt.DatiIntestazioneImpresa(Piva, Log, "", "", objParametri_Server)

                If Log = "" Then
                    str_riga1 = DT_Intestazione.Rows(0).Item("rag_soc")

                    str_riga2 = "P.IVA: " & DT_Intestazione.Rows(0).Item("PivaReale") &
                                " Cod. Fiscale: " & DT_Intestazione.Rows(0).Item("Codice_Fiscale")

                    str_riga3 = DT_Intestazione.Rows(0).Item("ind_impresa") & " " & DT_Intestazione.Rows(0).Item("CAP") + " " + DT_Intestazione.Rows(0).Item("frz_des") + " - " + DT_Intestazione.Rows(0).Item("LOCALITA") + " (" + DT_Intestazione.Rows(0).Item("COMUNI_PROV") + ") "

                    If Not IsNothing(DT_Intestazione) AndAlso DT_Intestazione.Rows.Count <> 0 Then
                        CType(rptStampa.Section1.ReportObjects("TxtIntestazione1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = str_riga1
                        CType(rptStampa.Section1.ReportObjects("TxtIntestazione2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = str_riga2
                        CType(rptStampa.Section1.ReportObjects("TxtIntestazione3"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = str_riga3
                    End If
                End If

            Catch ex As Exception
                Log_Errori += "- Intestazione: " + vbCrLf + ex.Message + vbCrLf
            End Try

            Try

                Log_Errori = ""

                Stampa_AnalisiProgetti(DSAnalisiProgetti, budget_consuntivo, costi_ricavi, dataDal, dataAl, dataRifDistinta, gestDettaglioMacchine, gestDettaglioPersonale, gestDettaglioProdotti, gestDettaglioNote,
                      livelloRottura,
                      gestDettaglioVarieta,
                      sopprimiDettaglio, filtro_codice_appezzamento, filtro_codice_impianto)

            Catch exc As Exception
                Log_Errori += "- Stampa: " + vbCrLf + exc.Message + vbCrLf
            End Try

            Try
                'Dim IdentificazioneDocumento As String

                'IdentificazioneDocumento = "_" + DateTime.Now().ToString("yyyy_MM_dd_hhmmssff")

                '' leggo la sottocartella da CategorieDocumenti
                'Dim Sottocartella As String
                'Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                'Sottocartella = objCatDoc.Sottocartella(enum_CategorieDocumenti.Conferimento, "", "", objParametri_Server)

                '' salvo il report in formato PDF
                'Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                'objGestFile.SalvaReportPdf(rptStampa,
                '                           enum_CategorieDocumenti.Conferimento,
                '                           Sottocartella,
                '                           Nome_Documento + IdentificazioneDocumento + ".pdf",
                '                           objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)
            Catch ex As Exception
                Log_Errori += "- Gestione allegati: " + vbCrLf + ex.Message + vbCrLf
            End Try

            'Dim agroWebCfg As AgronicaCoreGestioneRichieste.AgroWebConfig = New AgronicaCoreGestioneRichieste.AgroWebConfig
            'Dim pdfPath As String = agroWebCfg.PathFileTemporanei

            Dim objAgroWeb = New AgronicaCoreGestioneRichieste.AgroWebConfig
            Dim PathAllegati As String
            If objAgroWeb.GestioneAllegati_Repository = String.Empty Then
                PathAllegati = "C:\GIASLAN\AgronicaStampe_Allegati\" 'default
            Else
                PathAllegati = objAgroWeb.GestioneAllegati_Repository
            End If

            Dim pdfPath As String = FileSystemHelper.AggiungiSlashSeNonEsiste(PathAllegati) + "\" + Nome_Documento
            If Not Directory.Exists(pdfPath) Then Directory.CreateDirectory(pdfPath)
            Dim pdfTmp As String = pdfPath & Path.DirectorySeparatorChar & Nome_Documento & "_" & DateTime.Now().ToString("yyyy_MM_dd_hhmmssff") & ".pdf"

            Try

                'rptStampa.SaveAs(reportTemporano, True)
                rptStampa.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, pdfTmp)

            Catch ex As Exception
                Log_Errori += "- Generazione pdf temporaneo: " + vbCrLf + ex.Message + vbCrLf
            End Try

            'MS Dispose del report per evitare problema deallocazione.
            DSAnalisiProgetti.Dispose()
            DSAnalisiProgetti = Nothing

            rptStampa.Close()
            rptStampa.Dispose()
            rptStampa = Nothing

            GC.Collect()

            'Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) +
            '                  "&tmpReportPath=" + Stringa_Codifica(reportTemporano, AgroKey_EncoderDecoder, Server))

            Response.Redirect("..\VisualizzatoreReport.aspx?pdf=" + Stringa_Codifica(pdfTmp, AgroKey_EncoderDecoder, Server))

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            Dim Nome_File As String

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento +
                            vbCrLf(+vbCrLf + Log_Errori)

                Nome_File = "Log_Errori_" + Nome_Documento + "_" + CStr(Session("ASG_Utente_Username"))

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server,
                                                 "ControlloGestione",
                                                 Nome_File & ".txt",
                                                 Session("ASG_Utente_Username"),
                                                 "AnalisiProgetti.aspx",
                                                 Log_Errori)

            End If
            '-----------------------------------------

        End If

    End Sub

    '#####################################################################
    Private Sub Stampa_AnalisiProgetti(ByRef DSAnalisiProgetti As DS_AnalisiProgetti,
                                       ByVal budget_consuntivo As Integer,
                                       ByVal costi_ricavi As Integer,
                                       ByVal dataDal As String,
                                       ByVal dataAl As String,
                                       ByVal dataRifDistinta As String,
                                       ByVal gestDettaglioMacchine As Boolean,
                                       ByVal gestDettaglioPersonale As Boolean,
                                       ByVal gestDettaglioProdotti As Boolean,
                                       ByVal gestDettaglioNote As Boolean,
                                       ByVal livelloRottura As String,
                                       ByVal gestDettaglioVarieta As Boolean,
                                       ByVal sopprimiDettaglio As Boolean,
                                       ByVal filtro_codice_appezzamento As String,
                                       ByVal filtro_codice_impianto As String)

        Try

        Catch ex As Exception
            Log_Errori += "- Intestazione report: " + vbCrLf + ex.Message + vbCrLf
        End Try


        '==============================================
        '====== QUERY E CARICAMENTO DATASET ===========
        '==============================================

        Try

            LeggiRighe(DSAnalisiProgetti, budget_consuntivo, costi_ricavi, dataDal, dataAl, dataRifDistinta, gestDettaglioMacchine, gestDettaglioPersonale, gestDettaglioProdotti, gestDettaglioNote, livelloRottura, gestDettaglioVarieta, filtro_codice_appezzamento, filtro_codice_impianto)

            'imposto il dataset sul report
            rptStampa.SetDataSource(DSAnalisiProgetti)

            If sopprimiDettaglio Then
                rptStampa.SetParameterValue("SopprimiDettaglio", "1")
            Else
                rptStampa.SetParameterValue("SopprimiDettaglio", "0")
            End If

        Catch exc As Exception
            Log_Errori += "- Aggancio dataset: " + vbCrLf + exc.Message + vbCrLf
        End Try

    End Sub

    '###################################################

    Private Sub LeggiRighe(ByVal DSAnalisiProgetti As DS_AnalisiProgetti,
                          ByVal budget_consuntivo As Integer,
                          ByVal costi_ricavi As Integer,
                          ByVal dataDal As String,
                          ByVal dataAl As String,
                          ByVal dataRifDistinta As String,
                          ByVal gestDettaglioMacchine As Boolean,
                           ByVal gestDettaglioPersonale As Boolean,
                  ByVal gestDettaglioProdotti As Boolean,
                  ByVal gestDettaglioNote As Boolean,
                  ByVal livelloRottura As String,
                  ByVal gestDettaglioVarieta As Boolean,
                           ByVal filtro_codice_appezzamento As String,
                           ByVal filtro_codice_impianto As String)

        Dim objReader As New DW_CDG_Costi_Ricavi_DAL_R

        Dim xOrderBy As String = ""
        ' SORTO IL DataTable xOrderBy = "Costi_Ricavi, Specie_impianto, Descrizione_Centro, Linea_Produzione, Progetto, Descrizione_Macchina_Input_Costi, Categoria, Attivita, Descrizione, Descrizione_Prodotto, Nome_Cognome, Descrizione_Macchina, Data_Inserimento"

        If Trim(filtro_codice_appezzamento) = "" Then
            filtro_codice_appezzamento = ""
        Else
            If Right(" " & filtro_codice_appezzamento, 1) = "," Then
                filtro_codice_appezzamento = Left(filtro_codice_appezzamento, Len(filtro_codice_appezzamento) - 1)
            End If
            filtro_codice_appezzamento = filtro_codice_appezzamento.Replace("'", "''")
        End If

        If Trim(filtro_codice_impianto) = "" Then
            filtro_codice_impianto = ""
        Else
            If Right(" " & filtro_codice_impianto, 1) = "," Then
                filtro_codice_impianto = Left(filtro_codice_impianto, Len(filtro_codice_impianto) - 1)
            End If
        End If

        Dim dtRighe As DataTable = objReader.Leggi_Tabellone_DW(Piva, 0, budget_consuntivo, costi_ricavi, dataDal, dataAl, dataRifDistinta,
                                           "", filtro_codice_appezzamento, filtro_codice_impianto, "", xOrderBy, objParametri_Server)

        Dim DR As DS_AnalisiProgetti.DT_AnalisiProgettiRow
        Dim keySpecieCampoHT As New Hashtable
        Dim keyChiaviImpiantiHT As New Hashtable

        For Each riga In dtRighe.Rows
            Dim keySpecieCampo = ""

            DR = DSAnalisiProgetti.DT_AnalisiProgetti.NewRow()
            DR.ID = riga("Id_CDG_Dettagli")
            DR.Ragione_Sociale = riga("Ragione_Sociale_Azienda")
            DR.CdC = ""

            If riga("Id_Reg") <> 0 And livelloRottura <> "Progetti" Then
                Dim CdC_Parte1 As String = ""
                If livelloRottura = "Azienda" Then
                    CdC_Parte1 = ""
                    keySpecieCampo = riga("piva").ToString & "|0|0|0"
                Else
                    If livelloRottura = "Centro" Then
                        CdC_Parte1 = riga("Descrizione_Centro")
                        keySpecieCampo = riga("piva").ToString & "|" & riga("sa_cod").ToString & "|0|0"
                    Else
                        If livelloRottura = "Campo" Then
                            If String.IsNullOrWhiteSpace(riga("Descrizione_Campo")) Then
                                CdC_Parte1 = riga("Descrizione_Centro")
                                keySpecieCampo = riga("piva").ToString & "|" & riga("sa_cod").ToString & "|0|0"
                            Else
                                CdC_Parte1 = riga("Descrizione_Centro") & ", " & riga("Descrizione_Campo")
                                keySpecieCampo = riga("piva").ToString & "|" & riga("sa_cod").ToString & "|" & riga("campo_cod").ToString & "|0"
                            End If
                        Else
                            If livelloRottura = "Appezzamento" Then
                                If String.IsNullOrWhiteSpace(riga("Descrizione_Campo")) Then
                                    CdC_Parte1 = riga("Descrizione_Centro") & ", " & riga("Nome_Appezzamento")
                                    keySpecieCampo = riga("piva").ToString & "|" & riga("sa_cod").ToString & "|0|" & riga("appezza").ToString
                                Else
                                    CdC_Parte1 = riga("Descrizione_Centro") & ", " & riga("Descrizione_Campo") & ", " & riga("Nome_Appezzamento")
                                    keySpecieCampo = riga("piva").ToString & "|" & riga("sa_cod").ToString & "|" & riga("campo_cod").ToString & "|" & riga("appezza").ToString
                                End If
                            End If
                        End If
                    End If
                End If

                Dim SpeVar As String
                If gestDettaglioVarieta Then
                    SpeVar = riga("Specie_Impianto") & " " & riga("Varieta_impianto")
                    keySpecieCampo = keySpecieCampo & "|" & riga("veg_cod").ToString & "|" & riga("cul_cod").ToString
                Else
                    SpeVar = riga("Specie_Impianto")
                    keySpecieCampo = keySpecieCampo & "|" & riga("veg_cod").ToString
                End If
                If String.IsNullOrWhiteSpace(SpeVar) Then
                    keySpecieCampo = ""
                    'If riga("Destinazione_Uso") <> "" Then
                    SpeVar = riga("Destinazione_Uso")
                    'Else
                    '    SpeVar = "TERRENO NUDO"   'TODO Terreno Nudo
                    'End If
                End If

                'Utilizzato per specchietto ettari
                '  Colleziono sia le chiavi che gli impianti interessati
                If Not keySpecieCampo = "" Then
                    Dim k = riga("piva").ToString & "|" & riga("sa_cod").ToString & "|" & riga("appezza") & "|" & riga("Id_Reg").ToString
                    If Not keySpecieCampoHT.ContainsKey(keySpecieCampo) Then
                        keySpecieCampoHT.Add(keySpecieCampo, 0)

                        Dim listChiaviImpianti As New List(Of String)
                        listChiaviImpianti.Add(k)
                        keyChiaviImpiantiHT.Add(keySpecieCampo, listChiaviImpianti)
                    Else
                        Dim listChiaviImpianti As List(Of String) = keyChiaviImpiantiHT(keySpecieCampo)
                        If Not listChiaviImpianti.Contains(k) Then
                            listChiaviImpianti.Add(k)
                            keyChiaviImpiantiHT(keySpecieCampo) = listChiaviImpianti
                        End If
                    End If
                End If

                If String.IsNullOrWhiteSpace(CdC_Parte1) Then
                    DR.CdC = SpeVar
                Else
                    DR.CdC = CdC_Parte1 & ", " & SpeVar
                End If
            Else

                If riga("Id_Imputazione") <> 0 Then
                    DR.CdC = "Progetto " & riga("Progetto")
                    keySpecieCampo = ""
                Else
                    If riga("Linea_Cod") <> 0 Then
                        DR.CdC = riga("Linea_Produzione")
                        keySpecieCampo = ""
                    Else
                        If riga("Macchine_Cod") <> 0 Then
                            DR.CdC = riga("Descrizione_Macchina_Input_Costi")
                            keySpecieCampo = ""
                        End If
                    End If
                End If
            End If

            'Metto in testa comunque il tipo progetto perchè costituisca elemento di rottura
            If Not String.IsNullOrWhiteSpace(riga("Tipo_Progetto")) Then
                DR.CdC = riga("Tipo_Progetto") & ": " & DR.CdC
            End If
            DR.Costi_Ricavi_Des = riga("Costi_Ricavi_Des")

            DR.Categoria = riga("Categoria")

            If riga("Attivita") <> "" Then
                DR.Attivita = riga("Attivita")
            Else
                DR.Attivita = riga("descrizione_operazione")
            End If

            DR.Data_Inserimento = riga("Data_Inserimento")

            If Not String.IsNullOrWhiteSpace(riga("Descrizione_Prodotto")) Then
                If Not gestDettaglioProdotti Then
                    DR.Dettaglio = DR.Attivita
                Else
                    DR.Dettaglio = riga("Descrizione_Prodotto")
                    If CInt(riga("Costi_Ricavi")) = 1 Then
                        If String.IsNullOrWhiteSpace(riga("Descrizione")) Then
                            If gestDettaglioNote Then
                                DR.Dettaglio = DR.Dettaglio & " " & riga("Note")
                            End If
                        Else
                            DR.Dettaglio = DR.Dettaglio & " " & riga("Descrizione")
                        End If
                    Else
                        If gestDettaglioNote Then
                            DR.Dettaglio = DR.Dettaglio & " " & riga("Note")
                        End If
                    End If
                End If
            Else
                If Not String.IsNullOrWhiteSpace(riga("Nome_Cognome")) Then
                    If Not gestDettaglioPersonale Then
                        DR.Dettaglio = DR.Attivita
                    Else
                        DR.Dettaglio = riga("Nome_Cognome")
                        If gestDettaglioNote Then
                            DR.Dettaglio = DR.Dettaglio & " " & riga("Note")
                        End If
                    End If
                Else
                    If Not String.IsNullOrWhiteSpace(riga("Descrizione_Macchina")) Then
                        If Not gestDettaglioMacchine Then
                            DR.Dettaglio = DR.Attivita
                        Else
                            DR.Dettaglio = riga("Descrizione_Macchina")
                            If gestDettaglioNote Then
                                DR.Dettaglio = DR.Dettaglio & " " & riga("Note")
                            End If
                        End If
                    Else
                        If Not String.IsNullOrWhiteSpace(riga("Descrizione")) Then
                            DR.Dettaglio = riga("Descrizione")
                        Else
                            DR.Dettaglio = DR.Attivita
                        End If
                        If gestDettaglioNote Then
                            DR.Dettaglio = DR.Dettaglio & " " & riga("Note")
                        End If
                    End If
                End If
            End If
            ' Distinte poliennali
            If CDec(riga("Percentuale_Ripart")) <> 100 Then
                Dim strPerc As String = ""
                If CDec(riga("Percentuale_Ripart")) = CInt(riga("Percentuale_Ripart")) Then
                    strPerc = Format(riga("Percentuale_Ripart"), "0")
                Else
                    strPerc = Format(riga("Percentuale_Ripart"), "0.00")
                End If

                DR.Dettaglio = DR.Dettaglio & " - " & riga("Progetto_Nome") & " ripart. " & strPerc & " %"
                'riga("Progetto_Validita_Inizio")
                'riga("Progetto_Validita_Fine")
            End If

            DR.Mezzo_Des = riga("Mezzo_Des")
            DR.Prezzo = riga("Prezzo_Unitario")
            DR.Quantita = riga("Qta")
            DR.Importo = riga("Valore")

            DR.keySpecieCampo = keySpecieCampo

            DSAnalisiProgetti.DT_AnalisiProgetti.Rows.Add(DR)

        Next


        'Leggo varietà ed Ha per ogni Progetto - Campo
        Dim Reg_Impianti_Leggi As New Reg_Impianti_Read
        Dim var_R As New Cultivar_R
        Dim DtVarietaEttari As DataTable
        Dim DtVar As DataTable
        Dim dr1 As DS_AnalisiProgetti.DT_Analisi_Progetti_ImpiantiPerCdCRow
        Dim haTot As Decimal = 0

        Dim keySpecieCampoArray(keySpecieCampoHT.Keys.Count - 1) As String
        keySpecieCampoHT.Keys.CopyTo(keySpecieCampoArray, 0)

        For i = 0 To keySpecieCampoArray.Length - 1

            Dim keys As String() = keySpecieCampoArray(i).Split("|")
            Dim w_piva = keys(0)
            Dim w_sa_cod = 0
            If keys.Length > 1 Then
                w_sa_cod = keys(1)
            End If
            Dim w_campo_cod = 0
            If keys.Length > 2 Then
                w_campo_cod = keys(2)
            End If
            Dim w_appezza = 0
            If keys.Length > 3 Then
                w_appezza = keys(3)
            End If
            Dim w_specie = 0
            If keys.Length > 4 Then
                w_specie = keys(4)
            End If
            Dim w_varietà = 0
            If keys.Length > 5 Then
                w_varietà = keys(5)
            End If

            Dim xFiltroAggiuntivoVar As String = " 1 = 1 "
            'If w_campo_cod <> 0 Then
            '    xFiltroAggiuntivoVar += " AND Campo_Cod = " & w_campo_cod
            'End If

            Dim keysImpianto As String() = Nothing

            xFiltroAggiuntivoVar += " AND ( "
            Dim listChiaviImpianti As List(Of String) = keyChiaviImpiantiHT(keySpecieCampoArray(i))
            Dim primoGiro As Boolean = True

            For Each chiave_impianto In listChiaviImpianti
                keysImpianto = chiave_impianto.Split("|")

                If Not primoGiro Then
                    xFiltroAggiuntivoVar += " OR "
                End If
                xFiltroAggiuntivoVar += " ( Reg_Impianti.piva = '" & keysImpianto(0) & "'"
                xFiltroAggiuntivoVar += " AND Reg_Impianti.sa_cod = " & keysImpianto(1)
                xFiltroAggiuntivoVar += " AND Reg_Impianti.appezza = " & keysImpianto(2)
                xFiltroAggiuntivoVar += " AND Reg_Impianti.Id_Reg = " & keysImpianto(3) & ")"

                primoGiro = False
            Next
            xFiltroAggiuntivoVar += " ) "

            Dim xOrderByVar As String = "Cultivar.Cul_Des"

            'Dim str keySpecieCampo = riga("veg_cod").ToString & "|" & riga("piva").ToString & "|" & riga("sa_cod").ToString & "|" & riga("campo_cod").ToString
            DtVarietaEttari = Reg_Impianti_Leggi.Leggi_SpecieVarieta(w_piva, w_sa_cod, w_appezza, 0, w_specie, w_varietà,
                                                                     AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                     xFiltroAggiuntivoVar,
                                                                    xOrderByVar, objParametri_Server)

            ' Fatto così per errore di Cast da approfondire
            Dim DtAppVarietaEttari As New DataTable
            DtAppVarietaEttari.Columns.Add("Cul_Des", GetType(String))
            DtAppVarietaEttari.Columns.Add("Sup_Imp", GetType(Decimal))


            For Each r In DtVarietaEttari.Rows
                Dim dr0 = DtAppVarietaEttari.NewRow()
                dr0.Item("Cul_Des") = r("Cul_Des")
                dr0.Item("Sup_Imp") = r("Sup_Imp")

                DtAppVarietaEttari.Rows.Add(dr0)
            Next

            'Sommo per varietà
            Dim datiAggregatiVarietaEttari = From row In DtAppVarietaEttari
                                             Group row By datiRaggruppatiVarietaEttari = New With {
                                                          Key .Cul_Des = row.Field(Of String)("Cul_Des")
                                                                     } Into Group
                                             Order By datiRaggruppatiVarietaEttari.Cul_Des
                                             Select New With {
                                                              datiRaggruppatiVarietaEttari.Cul_Des,
                                                            .Sup_Imp = Group.Sum(Function(x) x.Field(Of Decimal)("Sup_Imp"))
                                                        }

            Dim myListVarietaEttari = datiAggregatiVarietaEttari.ToList()
            haTot = 0
            For Each RowDtVarietaEttari In myListVarietaEttari
                dr1 = DSAnalisiProgetti.DT_Analisi_Progetti_ImpiantiPerCdC.NewRow()
                dr1.Key = keySpecieCampoArray(i)
                dr1.Varieta = RowDtVarietaEttari.Cul_Des
                dr1.Ha = RowDtVarietaEttari.Sup_Imp
                haTot += dr1.Ha
                If Not gestDettaglioVarieta Then
                    DSAnalisiProgetti.DT_Analisi_Progetti_ImpiantiPerCdC.Rows.Add(dr1)
                End If
            Next

            keySpecieCampoHT(keySpecieCampoArray(i)) = haTot

        Next i

        Dim datiAggregati = From row In DSAnalisiProgetti.DT_AnalisiProgetti
                            Group row By datiRaggruppati = New With {
                                         Key .keySpecieCampo = row.Field(Of String)("keySpecieCampo"),
                                         Key .CdC = row.Field(Of String)("CdC"),
                                         Key .Costi_Ricavi_Des = row.Field(Of String)("Costi_Ricavi_Des"),
                                         Key .Categoria = row.Field(Of String)("Categoria"),
                                         Key .Attivita = row.Field(Of String)("Attivita"),
                                         Key .Dettaglio = row.Field(Of String)("Dettaglio"),
                                         Key .Data_Inserimento = row.Field(Of String)("Data_Inserimento"),
                                         Key .Mezzo_Des = row.Field(Of String)("Mezzo_Des"),
                                         Key .Prezzo = row.Field(Of Decimal)("Prezzo")
                                                    } Into Group
                            Order By datiRaggruppati.CdC, datiRaggruppati.Costi_Ricavi_Des, datiRaggruppati.Categoria, datiRaggruppati.Attivita, Date.Parse(datiRaggruppati.Data_Inserimento), datiRaggruppati.Mezzo_Des, datiRaggruppati.Prezzo
                            Select New With {
                                 datiRaggruppati.keySpecieCampo,
                                 datiRaggruppati.CdC,
                                 datiRaggruppati.Costi_Ricavi_Des,
                                 datiRaggruppati.Categoria,
                                 datiRaggruppati.Attivita,
                                 datiRaggruppati.Dettaglio,
                                 datiRaggruppati.Data_Inserimento,
                                 datiRaggruppati.Mezzo_Des,
                                 datiRaggruppati.Prezzo,
                                 .Quantita = Group.Sum(Function(x) x.Field(Of Decimal)("Quantita")),
                                 .Importo = Group.Sum(Function(x) x.Field(Of Decimal)("Importo"))
                                       }

        Dim myList = datiAggregati.ToList()

        DSAnalisiProgetti.DT_AnalisiProgetti.Rows.Clear()
        For Each r In myList
            DR = DSAnalisiProgetti.DT_AnalisiProgetti.NewRow()
            DR.ID = 0
            DR.Ragione_Sociale = ""
            DR.keySpecieCampo = r.keySpecieCampo
            DR.CdC = r.CdC
            If r.keySpecieCampo <> "" Then
                DR.Ha = keySpecieCampoHT(r.keySpecieCampo)
            Else
                DR.Ha = 0
            End If
            DR.Costi_Ricavi_Des = r.Costi_Ricavi_Des
            DR.Categoria = r.Categoria
            DR.Attivita = r.Attivita
            DR.Dettaglio = r.Dettaglio
            DR.Data_Inserimento = r.Data_Inserimento
            DR.Mezzo_Des = r.Mezzo_Des
            DR.Prezzo = r.Prezzo
            'DR.Quantita = r.Quantita
            DR.Quantita = Agro_Math.ArrotondaVal(r.Quantita, 2)
            DR.Importo = r.Importo
            DSAnalisiProgetti.DT_AnalisiProgetti.Rows.Add(DR)
        Next


        'Dim dv As DataView = DSAnalisiProgetti.DT_AnalisiProgetti.DefaultView
        'dv.Sort = "CdC, Costi_Ricavi_Des, Categoria, Attivita, Data_Inserimento, Dettaglio"
        'Dim sortedDT As DataTable = dv.ToTable()
        'DSAnalisiProgetti.DT_AnalisiProgetti.Rows.Clear()
        'For Each drSorted In sortedDT.Rows
        '    DSAnalisiProgetti.DT_AnalisiProgetti.ImportRow(drSorted)
        'Next

    End Sub

End Class