Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Gestione_Eccezioni_2015
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCorePianidiCampionamentoBiz

Public Class Stampa_Rapida_Fitofarmaci_vb
    Inherits System.Web.UI.Page

    Dim objParametriStampe As AgronicaCoreGestioneRichieste.ParametriAgronicaStampe_2010

#Region "Variabili"
    Private objParametri_Server As AgronicaCoreParametri
    Private objParametri_Utenti As AgronicaCoreParametri
    Private rptStampa_Rapida_Fitofarmaci As Stampa_Rapida_Fitofarmaci
    Private Log_Errori As String
    Private nomeDoc As String

    '----- Gestione Querystring
    Dim rag_soc, anno As String
    Dim TipoIntervalloTemp As Integer
    Dim validita_inizio, validita_fine As Date
    Dim Tipo_Selezione As String
#End Region

    Private Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init

        rptStampa_Rapida_Fitofarmaci = New Stampa_Rapida_Fitofarmaci
        Log_Errori = ""
        nomeDoc = ""

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        objParametri_Server = CType(Session("ASG_objParametri_Server"), AgronicaCoreParametri)
        objParametri_Utenti = CType(Session("ASG_objParametri_Utenti"), AgronicaCoreParametri)
        Dim pathFileTemplate As String

        Dim ds As New System.Data.DataSet

        Dim dt As DataTable = Nothing
        Dim dt_dati As DataTable = Nothing
        Dim dt2 As New DataTable





        objParametriStampe = New AgronicaCoreGestioneRichieste.ParametriAgronicaStampe_2010
        objParametriStampe.Leggi()

        Dim ParametriStampaPDC As New ParametriStampaPDC

        AgronicaCoreUtility.XMLUtility.getObjectFromXml(objParametriStampe.Xml_Filtro, ParametriStampaPDC)


        Dim Stb As New StringBuilder

        Stb.Append("SELECT     isnull ((select val_cod from Imprese_Codici where Imprese_Codici.id_cod = 1033 and PDC_Dettagli.Piva = Imprese_Codici.PIVA   )  ,'') AS 'Codice Fornitore', ")
        Stb.Append("        PDC_Dettagli.rag_soc AS 'Ragione Sociale', SpecieVegetali.Veg_Des AS 'Specie', Cultivar.Cul_Des AS 'Varietà', " &
                      " Analisi_Testata.Analisi_Testata_Note1 AS 'Altre Varietà del Lotto', Analisi_Testata.Analisi_Testata_Des AS 'Codice Analisi', CONVERT(VARCHAR(10),  " &
                      " Analisi_Testata.Analisi_Testata_Data_Inizio, 103) AS 'Data Inizio Analisi', CONVERT(VARCHAR(10), Analisi_Testata.Analisi_Testata_Data_Fine, 103)  " &
                      " AS 'Data Fine Analisi', Contatti.Rag_Soc AS 'Ragione Sociale Laboratorio', Analisi_Tipologia.Analisi_Tipologia_Des AS 'Tipologia di Analisi',  " &
                      " PDC_Analisi.Altre_Molecole AS 'Molecole Aggiuntive', PDC_Campioni.Codice_Campione AS 'Codice Campione', CONVERT(VARCHAR(10),  " &
                      " PDC_Campioni.Data_Campionamento, 103) AS 'Data Campione', ISNULL(PDC_Campioni.Note_Campione, '') AS 'Note Campione', ISNULL(PDC_Dettagli.note_impianto,  '') AS 'Note Impianto' , '' as sumQtaRilevata " &
                " FROM         PDC_Analisi INNER JOIN " &
                      " Analisi_Testata ON PDC_Analisi.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod AND  " &
                      " PDC_Analisi.PivaSuperUser = Analisi_Testata.Analisi_SuperUser INNER JOIN " &
                      " PDC_Campioni ON PDC_Analisi.PivaSuperUser = PDC_Campioni.PivaSuperUser AND PDC_Analisi.ID_PDC_Testata = PDC_Campioni.ID_PDC_Testata AND  " &
                      " PDC_Analisi.ID_PDC_Dettagli = PDC_Campioni.ID_PDC_Dettagli AND PDC_Analisi.ID_PDC_Campione = PDC_Campioni.ID_PDC_Campione INNER JOIN " &
                      " Risorse_Umane ON PDC_Analisi.Cod_Risum = Risorse_Umane.Cod_RisUm INNER JOIN " &
                      " Contatti ON Risorse_Umane.Piva = Contatti.Piva AND Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto INNER JOIN " &
                      " Analisi_Tipologia ON PDC_Analisi.PivaSuperUser = Analisi_Tipologia.PivaSuperUser AND  " &
                      " PDC_Analisi.Analisi_Tipologia_Cod = Analisi_Tipologia.Analisi_Tipologia_Cod INNER JOIN " &
                      " PDC_Dettagli ON PDC_Campioni.PivaSuperUser = PDC_Dettagli.PivaSuperUser AND PDC_Campioni.ID_PDC_Testata = PDC_Dettagli.ID_PDC_Testata AND  " &
                      " PDC_Campioni.ID_PDC_Dettagli = PDC_Dettagli.ID_PDC_Dettagli " &
                      " left JOIN " &
                      " SpecieVegetali ON PDC_Dettagli.Veg_Cod = SpecieVegetali.Veg_Cod left JOIN " &
                      " Cultivar ON PDC_Dettagli.Cul_Cod = Cultivar.Cul_Cod ")


        Stb.Append("  WHERE     (PDC_Analisi.ID_PDC_Testata = " & ParametriStampaPDC.ID_PDC_Testata & ")")
        Stb.Append("  and      (PDC_Analisi.ID_PDC_Dettagli = " & ParametriStampaPDC.ID_PDC_Dettagli & ")")
        Stb.Append("  and      (PDC_Analisi.Analisi_Testata_Cod = " & ParametriStampaPDC.Analisi_Testata_Cod & ")")
        'Stb.Append("  AND (Imprese_Codici.id_cod = 1033) ")


        Dim objSQL As New AgronicaCoreDataProvider.DataProvider
        dt_dati = objSQL.EseguiQuery_Lettura(objParametri_Server, Stb.ToString, "")
        If dt_dati.Rows.Count > 0 Then
            If dt_dati.Rows(0).Item("Altre Varietà del Lotto").ToString.Split(":").Length = 4 Then
                dt_dati.Rows(0).Item("Altre Varietà del Lotto") = dt_dati.Rows(0).Item("Altre Varietà del Lotto").ToString.Split(":")(3)
            Else
                dt_dati.Rows(0).Item("Altre Varietà del Lotto") = ""
            End If

        End If

        dt_dati.TableName = "Dati"




        Dim objPdc As New AgronicaCorePianidiCampionamentoDAL.PDC_R
        dt = objPdc.Leggi(ParametriStampaPDC.ID_PDC_Testata, "", 0, -99, "", AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)


        Dim PDC_Data_Istantanea As Date
        PDC_Data_Istantanea = dt.Rows(0).Item("PDC_Data_Istantanea")

        Dim strXml As String
        strXml = AgronicaCorePianidiCampionamentoBiz.Validazione.getXmlPerValidazione_SingoloCodice_Limite_Legge(ParametriStampaPDC.ID_PDC_Testata,
                                                                                                                 PDC_Data_Istantanea,
                                                                                                                 ParametriStampaPDC.Analisi_Testata_Cod)

        Dim strXmlRisposta As String
        strXmlRisposta = AgronicaCorePianidiCampionamentoBiz.Validazione.inviaXmlpervalidazione_al_WebService(strXml)

        Dim dpiAn As List(Of AgronicaCorePianidiCampionamentoBiz.ObjAnalisi)


        dpiAn = AgronicaCorePianidiCampionamentoBiz.Validazione.SpacchettaRisposta(strXmlRisposta)

        dt_dati.Rows(0).Item("sumQtaRilevata") = dpiAn(0).verifiche.listCapitolatiCliente(0).capitolatoResponse.sumQtaRilevata
        If dt_dati.Rows(0).Item("sumQtaRilevata") = -1 Then
            dt_dati.Rows(0).Item("sumQtaRilevata") = ""
        Else
            If IsNumeric(dt_dati.Rows(0).Item("sumQtaRilevata")) Then
                dt_dati.Rows(0).Item("sumQtaRilevata") = Format(CDbl(dt_dati.Rows(0).Item("sumQtaRilevata")), "########.00")
            End If
        End If
        ds.Tables.Add(dt_dati)


        Dim dr() As DataRow
        Dim objAnalisiW As New AgronicaCorePianidiCampionamentoDAL.PDC_Analisi_W
        Dim i As Integer

        Dim hashPA As New Hashtable
        Dim hashFam As New Hashtable


        For i = 0 To dpiAn.Count - 1
            'seleziono la riga
            'dr = dt.Select(" Analisi_Testata_Cod =" & dpiAn(i).id & "")
            'lista capitolati
            For j = 0 To dpiAn(i).verifiche.listCapitolatiCliente.Count - 1
                Dim val As Integer = dpiAn(i).verifiche.listCapitolatiCliente(j).ID

                Dim des As String
                If val = 0 Then
                    des = "limiti di legge"
                End If
                Dim k As Integer
                For k = 0 To dpiAn(i).verifiche.listCapitolatiCliente(j).capitolatoResponse.listFAMnonConformi.Count - 1
                    If Not hashFam.ContainsKey(dpiAn(i).verifiche.listCapitolatiCliente(j).capitolatoResponse.listFAMnonConformi(k).paCod) Then
                        hashFam.Add(dpiAn(i).verifiche.listCapitolatiCliente(j).capitolatoResponse.listFAMnonConformi(k).paCod,
                                   dpiAn(i).verifiche.listCapitolatiCliente(j).capitolatoResponse.listFAMnonConformi(k))
                    End If
                Next
                For k = 0 To dpiAn(i).verifiche.listCapitolatiCliente(j).capitolatoResponse.listSingoliPAnonConformi.Count - 1
                    If dpiAn(i).verifiche.listCapitolatiCliente(j).capitolatoResponse.listSingoliPAnonConformi(k).isFamiglia = False Then
                        If Not hashPA.ContainsKey(dpiAn(i).verifiche.listCapitolatiCliente(j).capitolatoResponse.listSingoliPAnonConformi(k).paCod) Then
                            hashPA.Add(dpiAn(i).verifiche.listCapitolatiCliente(j).capitolatoResponse.listSingoliPAnonConformi(k).paCod,
                                       dpiAn(i).verifiche.listCapitolatiCliente(j).capitolatoResponse.listSingoliPAnonConformi(k))
                        End If
                    End If
                Next
            Next
        Next


        Stb.Length = 0
        Dim DT_componenti_Rilevati As DataTable
        Stb.Append(" SELECT     Analisi_Dettagli.Analisi_Parametro_Cod, Analisi_Dettagli.Analisi_Dettaglio_Valore_1 AS 'Quantità Rilevata', UnitaMisura.UDM_SIM AS 'Unità Misura' ")
        Stb.Append(" FROM         Analisi_Dettagli INNER JOIN ")
        Stb.Append(" UnitaMisura ON Analisi_Dettagli.Analisi_Dettaglio_Valore_2 = UnitaMisura.UDM_COD ")
        Stb.Append("  WHERE      (Analisi_Dettagli.Analisi_Testata_Cod = " & ParametriStampaPDC.Analisi_Testata_Cod & ")")

        DT_componenti_Rilevati = objSQL.EseguiQuery_Lettura(objParametri_Server, Stb.ToString, "")

        Stb.Length = 0
        Dim DT_Famiglie As DataTable
        Stb.Append(" SELECT    fam_cod , descrizione from FamigliePrincipiAttivi ")
        DT_Famiglie = objSQL.EseguiQuery_Lettura(objParametri_Server, Stb.ToString, "")

        Stb.Length = 0
        Dim DT_PA As DataTable
        Stb.Append(" SELECT   Pa_Cod , Pa_Des from PrincipiAttivi ")
        DT_PA = objSQL.EseguiQuery_Lettura(objParametri_Server, Stb.ToString, "")




        'per ogni riga creo una colonna 
        If DT_componenti_Rilevati.Rows.Count = 0 Then
            Dim desc As DataColumn = New DataColumn("nessuna positività rilevata")
            desc.DataType = System.Type.GetType("System.String")
            dt.Columns.Add(desc)
        End If



        Dim pa_cod As DataColumn = New DataColumn("pa_cod")
        pa_cod.DataType = System.Type.GetType("System.String")
        dt2.Columns.Add(pa_cod)
        Dim pa_des As DataColumn = New DataColumn("pa_des")
        pa_des.DataType = System.Type.GetType("System.String")
        dt2.Columns.Add(pa_des)
        Dim testo As DataColumn = New DataColumn("testo")
        testo.DataType = System.Type.GetType("System.String")
        dt2.Columns.Add(testo)




        For i = 0 To DT_componenti_Rilevati.Rows.Count - 1
            Dim dr2 As DataRow = dt2.NewRow


            Dim str As String = ""
            Dim Val As Integer
            Dim header As String = ""


            Dim obj As AgronicaCorePianidiCampionamentoBiz.ObjPrincipiAttivi

            dr2.Item("pa_Cod") = DT_componenti_Rilevati.Rows(i).Item("Analisi_Parametro_Cod")

            Val = DT_componenti_Rilevati.Rows(i).Item("Analisi_Parametro_Cod")
            If Val > 0 Then
                dr = DT_PA.Select("PA_Cod = " & Val)
                header = header & dr(0).Item("Pa_Des")

                dr2.Item("Pa_Des") = dr(0).Item("Pa_Des")

                obj = hashPA(Val)
            Else
                obj = hashFam(Math.Abs(Val))
                dr = DT_Famiglie.Select("fam_cod = '" & Math.Abs(Val) & "'")
                header = header & dr(0).Item("descrizione")

                dr2.Item("Pa_Des") = dr(0).Item("descrizione")

            End If

            str = str & "Quantità rilevata: " & DT_componenti_Rilevati.Rows(i).Item("Quantità Rilevata")
            str = str & " " & DT_componenti_Rilevati.Rows(i).Item("Unità Misura") & vbNewLine
            str = str & "Pari al " & obj.percentuale & " % del LMR" & vbNewLine
            str = str & "LMR= " & obj.rma & " mg/kg"



            dr2.Item("testo") = str
            dt2.Rows.Add(dr2)



            Dim desc As DataColumn = New DataColumn(header)
            desc.DataType = System.Type.GetType("System.String")
            dt.Columns.Add(desc)

            dt.Rows(0).Item(header) = str


        Next

        dt2.TableName = "Pesticidi"
        ds.Tables.Add(dt2)

        rptStampa_Rapida_Fitofarmaci.Database.Tables("Dati").SetDataSource(dt_dati)
        rptStampa_Rapida_Fitofarmaci.Database.Tables("Pesticidi").SetDataSource(dt2)

        Dim reportTemporaneo As String = CrystalHelper.getFileReportTemporaneo()

        Try
            'NOTA Come miglioramento, creare una enum_CategorieDocumenti specifica. Non è obbligatorio in quanto il file generato lo elimino
            Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
            Dim Sottocartella As String = objCatDoc.Sottocartella(enum_CategorieDocumenti.RegistriCampagna, "", "", objParametri_Server)

            Dim rnd As New Random
            Dim nomeDoc_Estensione = nomeDoc & "_" & rnd.Next() & ".pdf"

            'Salvo il report in formato PDF
            Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
            Dim pathPdfGenerato = objGestFile.SalvaReportPdf(rptStampa_Rapida_Fitofarmaci,
                                        enum_CategorieDocumenti.RegistriCampagna,
                                        Sottocartella,
                                        nomeDoc_Estensione,
                                        objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

            'Elimino il file perché non necessario, verrà generato per l'utente nel VisualizzatoreReport
            IO.File.Delete(pathPdfGenerato)
            'Dim prc As New CrystalDecisions.Shared.ReportPageRequestContext
            'Dim dummy = rptRichiestaCarb.FormatEngine.GetLastPageNumber(prc)
            rptStampa_Rapida_Fitofarmaci.SaveAs(reportTemporaneo, True)
        Catch ex As Exception
            Log_Errori &= "- Salvataggio report temporaneo: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf
        End Try

        '-----------------------------------------
        '---- Salvataggio Log Errori -------------
        '-----------------------------------------          
        Dim Nome_File_Log As String

        If Log_Errori <> "" Then

            Log_Errori = nomeDoc & vbCrLf & vbCrLf & Log_Errori

            Nome_File_Log = "Log_Errori_" & nomeDoc & ".txt"

            Dim objLog As New GestioneLogStampe
            objLog.Gestione_LogErrori_Stampe(objParametri_Server,
                                            "Stampe_PdC_StampaRapidaFitofarmaci",
                                            Nome_File_Log,
                                            Session("ASG_Utente_Username"),
                                            nomeDoc,
                                            Log_Errori)

        End If


        GC.Collect()


        '-----------------------------------------
        '---- Redirect su VisualizzatoreReport ---
        '-----------------------------------------  
        Response.Redirect(VirtualPathUtility.ToAbsolute("~/GestioneStampe/VisualizzatoreReport.aspx") &
                    "?anteprima=" & Stringa_Codifica("0", AgroKey_EncoderDecoder, Server) &
                    "&tmpReportPath=" & Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server) &
                    "&NomePdf=" & Stringa_Codifica(nomeDoc, AgroKey_EncoderDecoder, Server))



    End Sub

End Class