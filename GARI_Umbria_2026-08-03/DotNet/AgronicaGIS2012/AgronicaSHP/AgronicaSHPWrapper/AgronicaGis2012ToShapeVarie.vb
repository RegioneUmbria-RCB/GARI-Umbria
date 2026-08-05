Imports AgronicaCoreDataProvider

Public Class AgronicaGis2012ToShapeVarie



    Public Sub PredisponiPassoLineaGuidaMappaPrescrizione(
            LayerDoveLeggereConfini As Integer,
            ByRef objParametri_Server As AgronicaCoreParametri,
            ByRef objParametri_Utenti As AgronicaCoreParametri,
            curEntita_Cod As Integer,
            leggiLineeGuidaGIS As AgronicaCoreGisDAL.GIS_Entita_R,
            ByRef iFilePrescrizione As Integer,
            Codice_Fiscale_Tecnico As String,
            lRicettaOperazioneCod As Integer,
            lFilePrescription As String,
            mappaturaDatiMappaPrescrizione As List(Of DBFDataModel_MappaturaDati),
            CalcolaAreaPerimetro As Boolean)


        Dim dtAppDati As DataTable =
        leggiLineeGuidaGIS.Leggi(objParametri_Server.PivaSuperUser, curEntita_Cod, 0, "", 0, 0, 0, 0, "", "", "-1", -1, -1, "-1", 0, 0, 0, 0, Codice_Fiscale_Tecnico, "", Nothing, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server, objParametri_Utenti)

        Dim lPiva As String = ""
        Dim lSa_Cod As Integer
        Dim lAppezza As Integer
        Dim lCampo_cod As Integer
        Dim lidImp As Integer
        Dim lProgrammazione_Cod As Integer
        Dim lProgrammazione_Entita_cod As Integer

        If dtAppDati.Rows.Count > 0 Then
            lPiva = dtAppDati(0)("piva")
            lSa_Cod = dtAppDati(0)("sa_cod")

            If LayerDoveLeggereConfini = TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.Pianificazioni_Testata Or LayerDoveLeggereConfini = TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.Impianti_Pianificati_Entita Then
                lProgrammazione_Cod = dtAppDati(0)("Programmazione_Cod")
                lProgrammazione_Entita_cod = dtAppDati(0)("Programmazione_Entita_Cod")

            Else
                lAppezza = dtAppDati(0)("Appezza")
                lCampo_cod = dtAppDati(0)("campo_cod")
                lidImp = dtAppDati(0)("id_imp")

            End If

        End If


        ' VAnni: 2/3/2017: ricerco le mappe di prescrizione legate alla ricetta + impianto (oppure pianificazione)
        Dim xmlPrescription As String
        Dim leggiAllegati As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_R

        ' VAnni: 19/2/2021: leggo la mappa di prescrizione dagli allegati della ricetta
        If Not IsNothing(leggiAllegati) Then
            Dim sFiltroRicettaOperazioni As String = " allegati_Documenti_Cod in (" &
                AgronicaCoreAnagrafeBIZ.Allegati_Documenti.LeggiListaAllegatiDocumentiCodDaListaRicettaOperazioni(objParametri_Server, "Ricetta_Operazione_Cod = " & lRicettaOperazioneCod) &
            ")"
            xmlPrescription = leggiAllegati.LeggiXML(0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, sFiltroRicettaOperazioni, "", objParametri_Server, lPiva)
        Else
            xmlPrescription = leggiLineeGuidaGIS.LeggiXML(objParametri_Server.PivaSuperUser, 0, 51, "", lPiva, lSa_Cod, lAppezza, 0, lidImp, "", "", "-1", -1, -1, "-1", 0, lRicettaOperazioneCod, lProgrammazione_Cod, lProgrammazione_Entita_cod, -1, Codice_Fiscale_Tecnico, "", Nothing, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server, objParametri_Utenti, calcola_AreaPerimetro:=CalcolaAreaPerimetro)
        End If


        If Not String.IsNullOrEmpty(xmlPrescription) Then
            iFilePrescrizione += 1
            EsportaShpDatoXml(lFilePrescription, xmlPrescription, False, True, mappaturaDatiMappaPrescrizione)
        End If

    End Sub



    Public Function EsportaShpDatoXml(
            ByVal filename As String,
            ByVal xmlToConvert As String,
            ByVal comprimiZip As Boolean,
            ByVal importaAttributiTipizzati As Boolean,
            mappaturaDatiMappaPrescrizione As List(Of DBFDataModel_MappaturaDati),
            Optional ByVal ScriviPRJ As Boolean = False
            ) As String
        xmlToConvert = xmlToConvert.Replace("<DatiEntita>", "<DatiEntita xmlns=""http://www.agronica.it/grafica/"" xmlns:gml=""http://www.opengis.net/gml"">")

        Dim target As AgronicaGis2012ToShapeFile = New AgronicaGis2012ToShapeFile()

        target.Convert(xmlToConvert, filename, importaAttributiTipizzati, mappaturaDatiMappaPrescrizione)

        If ScriviPRJ Then
            ScriviFilePRJWSG84(filename.Replace(".shp", ".prj"))
        End If

        'zippa i files.
        If comprimiZip Then
            Dim zipFile As String = filename.Replace(".shp", ".zip")
            AgronicaCoreUtility.AgroZip.AddFileToZip(zipFile, filename)
            AgronicaCoreUtility.AgroZip.AddFileToZip(zipFile, filename.Replace(".shp", ".dbf"))
            AgronicaCoreUtility.AgroZip.AddFileToZip(zipFile, filename.Replace(".shp", ".shx"))
            If ScriviPRJ Then
                AgronicaCoreUtility.AgroZip.AddFileToZip(zipFile, filename.Replace(".shp", ".prj"))
            End If
        End If

        Return "ok"
    End Function

    Private Sub ScriviFilePRJWSG84(ByVal filename As String)
        System.IO.File.WriteAllText(filename, "GEOGCS[""GCS_WGS_1984"",DATUM[""D_WGS_1984"",SPHEROID[""WGS_1984"",6378137.0,298.257223563]],PRIMEM[""Greenwich"",0.0],UNIT[""Degree"",0.0174532925199433]]")
    End Sub

End Class
