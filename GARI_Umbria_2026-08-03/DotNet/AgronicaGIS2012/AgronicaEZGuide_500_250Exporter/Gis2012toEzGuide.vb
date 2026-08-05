Imports <xmlns="http://EzOffice_ezg_profile">

Imports AgronicaCoreDataProvider
Imports AgronicaSHPWrapper
Imports System.Collections.ObjectModel
Imports System.IO

Imports AgronicaConversioneCartografiaGias.FormatsConverter
Imports AgronicaGIS2012.Commons
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Gis2012toEzGuide

    Private Const _lenName As Integer = 100




    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="PivaSuperUser"></param>
    ''' <param name="Entita_Cod"></param>
    ''' <param name="TipoEntita_Cod"></param>
    ''' <param name="PivaPadre"></param>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Appezza"></param>
    ''' <param name="Campo_Cod"></param>
    ''' <param name="ID_Imp"></param>
    ''' <param name="Prov"></param>
    ''' <param name="Com"></param>
    ''' <param name="Sezione"></param>
    ''' <param name="Foglio"></param>
    ''' <param name="Numero"></param>
    ''' <param name="Subalterno"></param>
    ''' <param name="ID_Agenda"></param>
    ''' <param name="Ricetta_Operazione_Cod"></param>
    ''' <param name="Programmazione_Entita_cod"></param>
    ''' <param name="exportfileBasePath"></param>
    ''' <param name="NomeFileFinale"></param>
    ''' <param name="LeggiContattiMacchineOppureDatoFinale">valore negativo per dato finale, 1 - contatti, 2 - macchine, 3 - operazioni</param>
    ''' <param name="objParametri_Server"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreaDatiEzGuide_RecuperaElencoMacchineOperatori(
                            ByVal PivaSuperUser As String,
                            ByVal Entita_Cod As Int32,
                            ByVal TipoEntita_Cod As Int32,
                            ByVal PivaPadre As String,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Appezza As Int32,
                            ByVal Campo_Cod As Int32,
                            ByVal ID_Imp As Int32,
                            ByVal Prov As String,
                            ByVal Com As String,
                            ByVal Sezione As String,
                            ByVal Foglio As Int32,
                            ByVal Numero As Int32,
                            ByVal Subalterno As String,
                            ByVal ID_Agenda As Int32,
                            ByVal Ricetta_Operazione_Cod As Integer,
                            ByVal Programmazione_Entita_cod As Integer,
                            ByVal exportfileBasePath As String,
                            ByVal NomeFileFinale As String,
                            ByVal LeggiContattiMacchineOppureDatoFinale As Integer,
                            ByVal datada As DateTime,
                            ByVal dataa As DateTime,
                            ByVal layerElementiPF As Integer,
                            ByRef objParametri_Server As AgronicaCoreParametri,
                            ByVal Codice_Fiscale_Tecnico As String
                        ) As DataTable

        Return GetDtOperazioniRicette(Entita_Cod, TipoEntita_Cod, PivaPadre, Piva, Sa_Cod, Appezza, Campo_Cod,
                                      ID_Imp, Prov, Com, Sezione, Foglio, Numero, Subalterno, ID_Agenda, Ricetta_Operazione_Cod,
                                      Programmazione_Entita_cod, "", objParametri_Server, LeggiContattiMacchineOppureDatoFinale,
                                      datada, dataa, layerElementiPF, Codice_Fiscale_Tecnico)

    End Function



    Private Shared Function GetDtOperazioniRicette(ByVal Entita_Cod As Int32, ByVal TipoEntita_Cod As Int32, ByVal PivaPadre As String, ByVal Piva As String, ByVal Sa_Cod As Int32, ByVal Appezza As Int32, ByVal Campo_Cod As Int32, ByVal ID_Imp As Int32, ByVal Prov As String, ByVal Com As String, ByVal Sezione As String, ByVal Foglio As Int32, ByVal Numero As Int32, ByVal Subalterno As String, ByVal ID_Agenda As Int32, ByVal Ricetta_Operazione_Cod As Integer, ByVal Programmazione_Entita_cod As Integer, ByVal ListaContatti As String, ByRef objParametri_Server As AgronicaCoreParametri, ByVal LeggiContattiMacchineOppureDatoFinale As Integer, ByVal datada As DateTime, ByVal dataa As DateTime, ByVal layerElementiPF As Integer, ByVal Codice_Fiscale_Tecnico As String) As DataTable
        Dim curEntita_Cod As Int32 = 0
        Dim leggiOperazioniRicette As New AgronicaCoreGisDAL.PrecisionFarming

        ' VAnni: 3/3/2017: seleziono i layer per le linee guida ed il layer per i confini (appezzamenti, impianti oppure planning)
        Dim xLeggiTipoEntita As New AgronicaCoreGisDAL.GIS_TipoEntita_R
        Dim dtLEggiTipoEntita As DataTable =
        xLeggiTipoEntita.Leggi(0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, " LayerElementiGrafici_cod = " & layerElementiPF, "", objParametri_Server)
        Dim FiltroTipi As String = String.Join(",",
            (
            From d In dtLEggiTipoEntita.AsEnumerable
            Select d("TipoEntita_Cod")).ToArray
       )

        Return leggiOperazioniRicette.LeggiLineeGuidaAB(objParametri_Server.PivaSuperUser, Entita_Cod, TipoEntita_Cod, PivaPadre, Piva, Sa_Cod, Appezza, Campo_Cod, ID_Imp, Prov, Com, Sezione, Foglio, Numero, Subalterno, ID_Agenda, 0, Programmazione_Entita_cod, 0, ListaContatti, LeggiContattiMacchineOppureDatoFinale, datada, dataa, layerElementiPF, Codice_Fiscale_Tecnico, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, " Entita.TipoEntita_Cod in (" & FiltroTipi & ", 55) ", "", objParametri_Server)

    End Function


    ''' <summary>
    ''' Scrive i diversi file nella cartella dell'impianto: 1. File con LATLON linee guida, 2. Swats, cioè linee guida (SHP), 3. Boundaray (SHP), 4. mappa di prescrizione (SHP)
    ''' </summary>
    ''' <param name="CurrentPathA"></param>
    ''' <param name="CurrentPath"></param>
    ''' <param name="LayerDoveLeggereConfini"></param>
    ''' <param name="objParametri_Server"></param>
    ''' <param name="curEntita_Cod"></param>
    ''' <param name="lPrescriptionBasePath"></param>
    ''' <param name="leggiLineeGuidaGIS"></param>
    ''' <param name="iFilePrescrizione"></param>
    ''' <param name="lineaGuida"></param>
    ''' <param name="xmlGIS"></param>
    ''' <param name="ltotalFieldPath"></param>
    ''' <param name="Codice_Fiscale_Tecnico"></param>
    Private Sub PredisponiPassoLineaGuida(ByVal CurrentPathA As String, ByVal CurrentPath As String, ByVal LayerDoveLeggereConfini As Integer,
                                          ByRef objParametri_Server As AgronicaCoreParametri, ByRef objParametri_Utenti As AgronicaCoreParametri,
                                          ByVal curEntita_Cod As Int32, ByVal lPrescriptionBasePath As String,
                                          ByVal leggiLineeGuidaGIS As AgronicaCoreGisDAL.GIS_Entita_R,
                                          ByRef iFilePrescrizione As Integer, ByVal lineaGuida As DataRow, ByVal xmlGIS As String,
                                          ByVal ltotalFieldPath As String, ByVal Codice_Fiscale_Tecnico As String)

        Dim lFileDascrivere As String = ltotalFieldPath & "\Swaths.shp"

        'Se siamo sul layer 55 allora si tratta di linee guida.
        If lineaGuida("LayerElementiGrafici_Cod") = TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.Precision Then

            '1. File con LATLON linee guida
            If lineaGuida("ElementoGrafico_DES") = "A" Then
                ScriviFileCoordinate(xmlGIS, CurrentPathA)
            End If

            'If lineaGuida("ElementoGrafico_DES") = "B" Then non deve fare nulla sul B

            If lineaGuida("ElementoGrafico_DES").ToString.StartsWith("Date") Then

                '2. Swats, cioè linee guida (SHP)                


                If Not My.Computer.FileSystem.FileExists(lFileDascrivere) Then
                    Dim FullFileNameForSwaths As String = lFileDascrivere
                    Dim SHPWrap As New AgronicaSHPWrapper.AgronicaGis2012ToShapeVarie
                    SHPWrap.EsportaShpDatoXml(FullFileNameForSwaths, xmlGIS, False, False, New List(Of DBFDataModel_MappaturaDati))
                End If

            End If

        Else


            '3. Boundaray (SHP)
            Dim pivaB As String = ""
            Dim sa_codB As Integer = 0
            Dim appezzaB As Integer = 0
            Dim id_impB As Integer = 0

            Dim programmazione_cod As Integer = 0
            Dim Programmazione_entita_Cod As Integer = 0

            pivaB = lineaGuida("Piva")
            sa_codB = lineaGuida("sa_cod")

            If LayerDoveLeggereConfini = TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.Impianti_Pianificati_Entita Or LayerDoveLeggereConfini = TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.Pianificazioni_Testata Then
                programmazione_cod = lineaGuida("Programmazione_Cod")
            Else
                appezzaB = lineaGuida("Appezza")
                id_impB = lineaGuida("id_reg")
            End If


            Dim id_impLayerB As Integer = id_impB
            If LayerDoveLeggereConfini <> 19 Then
                id_impLayerB = 0
            End If


            Dim finalLayerDoveLeggereConfini As TipiEnumerativi.enum_Gis_LayerElementiGrafici_std

            If LayerDoveLeggereConfini = TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.Pianificazioni_Testata Then
                finalLayerDoveLeggereConfini = TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.Impianti_Pianificati_Entita
            Else
                finalLayerDoveLeggereConfini = LayerDoveLeggereConfini
            End If

            'la funzione viene chiamata su tutte le entità.
            Dim finalCurEntita_cod As Integer = 0
            If LayerDoveLeggereConfini = TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.Impianti_Pianificati_Entita Then
                finalCurEntita_cod = curEntita_Cod
            End If


            Dim xmlGISBoundary As String =
            leggiLineeGuidaGIS.LeggiXML(objParametri_Server.PivaSuperUser, finalCurEntita_cod, 0, "", pivaB, sa_codB, appezzaB, 0, id_impLayerB, "", "", "-1", -1, -1, "-1", 0, 0,
                                        programmazione_cod, Programmazione_entita_Cod, -1, Codice_Fiscale_Tecnico, "", Nothing, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                        " geodata.LayerElementiGrafici_Cod = " & finalLayerDoveLeggereConfini, "", objParametri_Server, objParametri_Utenti)

            If xmlGISBoundary <> "" Then
                Dim xEntita As XDocument = XDocument.Parse(xmlGISBoundary)
                If xEntita.Elements("DatiEntita").Elements("Entita").Count > 0 Then

                    For Each sNode In xEntita.Elements("DatiEntita").Elements("Entita")
                        sNode.Attribute("text").Value = "A§ 1"
                    Next
                    lFileDascrivere = ltotalFieldPath & "\Boundary.shp"

                    If Not My.Computer.FileSystem.FileExists(lFileDascrivere) Then
                        Dim FullFileNameForSwaths As String = lFileDascrivere
                        Dim SHPWrap As New AgronicaSHPWrapper.AgronicaGis2012ToShapeVarie
                        SHPWrap.EsportaShpDatoXml(FullFileNameForSwaths, xEntita.ToString, False, False, New List(Of DBFDataModel_MappaturaDati))
                    End If
                End If
            End If
            'FINE 3. Boundaray (SHP)


            '4. mappa di prescrizione (SHP)
            'Dim lFilePrescription As String  & ltotalFieldPath.Replace(exportfileBasePath, "").Replace("\", "_").Substring(0, 7) & ".shp"
            Dim lRicettaOperazioneCod As Integer
            lRicettaOperazioneCod = lineaGuida("Ricetta_Operazione_cod")

            Dim lSoloNomeFilePrescrizione As String = lRicettaOperazioneCod & "_" & iFilePrescrizione
            lSoloNomeFilePrescrizione = Right("PPPPPPPP" & lSoloNomeFilePrescrizione, 8)

            Dim lFilePrescription As String = lPrescriptionBasePath & "\" & lSoloNomeFilePrescrizione & ".shp"

            If Not My.Computer.FileSystem.FileExists(lFilePrescription) Then

                Dim SHPWrap As New AgronicaSHPWrapper.AgronicaGis2012ToShapeVarie
                SHPWrap.PredisponiPassoLineaGuidaMappaPrescrizione(LayerDoveLeggereConfini, objParametri_Server, objParametri_Utenti, curEntita_Cod, leggiLineeGuidaGIS, iFilePrescrizione, Codice_Fiscale_Tecnico, lRicettaOperazioneCod, lFilePrescription, New List(Of DBFDataModel_MappaturaDati), False)

                'FINE 4. mappa di prescrizione (SHP)

            End If
        End If

    End Sub

    Public Sub CreaDatiEzGuide_EsportaSoloLineeGuida(
                                ByVal PivaSuperUser As String,
                                ByVal Entita_Cod As Int32,
                                ByVal TipoEntita_Cod As Int32,
                                ByVal PivaPadre As String,
                                ByVal Piva As String,
                                ByVal Sa_Cod As Int32,
                                ByVal Appezza As Int32,
                                ByVal Campo_Cod As Int32,
                                ByVal ID_Imp As Int32,
                                ByVal Prov As String,
                                ByVal Com As String,
                                ByVal Sezione As String,
                                ByVal Foglio As Int32,
                                ByVal Numero As Int32,
                                ByVal Subalterno As String,
                                ByVal ID_Agenda As Int32,
                                ByVal Ricetta_Operazione_Cod As Integer,
                                ByVal Programmazione_Entita_cod As Integer,
                                ByVal exportfileBasePath As String,
                                ByVal NomeFileFinale As String,
                                ByVal ListaContatti As String,
                                ByVal LeggiContattiMacchineOppureDatoFinale As Integer,
                                ByVal datada As DateTime,
                                ByVal dataa As DateTime,
                                ByVal LayerDoveLeggereConfini As Integer,
                                ByVal Codice_Fiscale_Tecnico As String,
                                ByRef objParametri_Server As AgronicaCoreParametri,
                                ByRef objParametri_Utenti As AgronicaCoreParametri
                                )

        AgronicaCoreUtility.FileSystemHelper.PuliziaCartella(exportfileBasePath)

        Dim lNomeCartellaFinale As String = NomeFileFinale.Replace(".zip", "")
        CreaStrutturaCartelleRicorsiva(lNomeCartellaFinale, exportfileBasePath)



        Dim curEntita_Cod As Int32
        Dim dtOperazioniRicette As DataTable = GetDtOperazioniRicette(Entita_Cod, TipoEntita_Cod, PivaPadre, Piva, Sa_Cod, Appezza, Campo_Cod, ID_Imp,
                                                                      Prov, Com, Sezione, Foglio, Numero, Subalterno, Ricetta_Operazione_Cod, ID_Agenda,
                                                                      Programmazione_Entita_cod, ListaContatti, objParametri_Server,
                                                                      LeggiContattiMacchineOppureDatoFinale, datada, dataa, LayerDoveLeggereConfini, Codice_Fiscale_Tecnico)

        GeneraXmlConfig(dtOperazioniRicette, exportfileBasePath & "\EZOffice_ezg_profile.cfg")
        CreaStrutturaCartelleRicorsiva("AgGPS", exportfileBasePath)

        Dim lPrescriptionBasePath As String = exportfileBasePath & "\Prescriptions"
        My.Computer.FileSystem.CreateDirectory(lPrescriptionBasePath)

        CreaStrutturaCartelleRicorsiva("Data", exportfileBasePath)


        Dim CurrentPath As String = ""
        CurrentPath = exportfileBasePath

        Dim leggiLineeGuidaGIS As New AgronicaCoreGisDAL.GIS_Entita_R

        Dim iFilePrescrizione As Integer = 0
        Dim icontatore As Integer = 0

        Dim currentPathA As String = ""


        ' VAnni: 2/3/2017: Gestito anche il caso in cui ci siano solo confini e non linee guida.

        'scorrendo i record otterò un raggruppamento di: "Linea guida - Punto A, Linea guida - Punto B - Linea guida (retta che congiunge), poligono di confini 
        'se non sono presenti le linee guida allora otterrò il solo  poligono di confine.

        For Each lineaGuida As DataRow In dtOperazioniRicette.Rows

            curEntita_Cod = lineaGuida("Entita_cod")

            Dim xmlGIS As String =
            leggiLineeGuidaGIS.LeggiXML(objParametri_Server.PivaSuperUser, curEntita_Cod, 0, "", "", 0, 0, 0, 0, "", "", "-1", -1, -1, "-1", 0, 0, 0, 0, -1, Codice_Fiscale_Tecnico, "", Nothing, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server, objParametri_Utenti)

            Dim ltotalFieldPath As String
            ltotalFieldPath = GetLtotalFieldPath(CurrentPath, lineaGuida)

            Dim ldescrizioneCartellaOperazione As String =
                RimuoviCaratteriIllegali(lineaGuida("descrizioneCartellaOperazione"), 100)

            Dim lPathDaCreare As String =
                lineaGuida("rag_soc") & "\" &
                lineaGuida("sa_nome") & "\" &
                lineaGuida("campo_des") & "\" &
                lineaGuida("App_Nome")

            ' Inizializzazione, genero le cartelle che servono.. (es.: Agricola_San_Giorgio_SpA\Centro_GOMBITO\PASTORE_4)
            If String.IsNullOrEmpty(currentPathA) OrElse Not My.Computer.FileSystem.DirectoryExists(CurrentPath & "\" & lPathDaCreare) Then
                CreaStrutturaCartelleRicorsiva(lineaGuida("rag_soc"), CurrentPath)
                CreaStrutturaCartelleRicorsiva(lineaGuida("sa_nome"), CurrentPath)
                CreaStrutturaCartelleRicorsiva(lineaGuida("Campo_Des") & lineaGuida("App_nome"), CurrentPath)

                currentPathA = CurrentPath

            End If

            Dim lCartellaOperazione As String
            lCartellaOperazione = ltotalFieldPath & "\" & ldescrizioneCartellaOperazione
            If Not My.Computer.FileSystem.DirectoryExists(lCartellaOperazione) Then
                My.Computer.FileSystem.CreateDirectory(lCartellaOperazione)
                My.Computer.FileSystem.WriteAllBytes(lCartellaOperazione & "\idle.txt", New Byte() {}, False)
            End If

            ' VAnni: 3/3/2017: chiamo la funzione che genera quanto serve (linee guida, confini, mappa di prescrizione..)
            PredisponiPassoLineaGuida(
                currentPathA,
                CurrentPath,
                LayerDoveLeggereConfini,
                objParametri_Server,
                objParametri_Utenti,
                curEntita_Cod,
                lPrescriptionBasePath,
                leggiLineeGuidaGIS,
                iFilePrescrizione,
                lineaGuida,
                xmlGIS,
                ltotalFieldPath,
                Codice_Fiscale_Tecnico
            )

            CurrentPath = exportfileBasePath

        Next

        ZippaCartella(exportfileBasePath.Replace("\AgGPS\Data", "").Replace("\\", "\"))


    End Sub

    Private Shared Function GetLtotalFieldPath(ByVal CurrentPath As String, ByVal lineaGuida As Object) As String

        Dim ltotalFieldPath As String

        ltotalFieldPath =
                        CurrentPath & "\" &
                        RimuoviCaratteriIllegali(lineaGuida("rag_soc"), _lenName) & "\" &
                        RimuoviCaratteriIllegali(lineaGuida("sa_nome"), _lenName) & "\" &
                        RimuoviCaratteriIllegali(lineaGuida("Campo_Des") & lineaGuida("App_nome"), _lenName)
        Return ltotalFieldPath
    End Function
    Private Sub CreaStrutturaCartelleRicorsiva(ByVal folderToAdd As String, ByRef basePath As String)
        folderToAdd = RimuoviCaratteriIllegali(folderToAdd, _lenName)
        Dim lNuovaCartella As String = basePath & "\" & folderToAdd

        Dim esisteCartella As Boolean
        esisteCartella = My.Computer.FileSystem.DirectoryExists(lNuovaCartella)

        If Not esisteCartella Then
            My.Computer.FileSystem.CreateDirectory(lNuovaCartella)

        End If

        basePath = lNuovaCartella

    End Sub

    Private Shared Function RimuoviCaratteriIllegali(ByVal daDove As String, ByVal maxLen As Int16) As String
        Dim caratteri As String = "!.£.$.%.&.#.§."
        For Each c In caratteri
            daDove = daDove.Replace(c, "")
        Next

        Dim rval As String = daDove.Replace(" ", "_")
        If rval.Length >= maxLen Then
            rval = rval.Substring(0, maxLen - 1)
        End If
        Return rval
    End Function

    Private Sub ScriviFileCoordinate(ByVal xDataGML As String, ByVal basePath As String)

        Dim lFileDaScrivere As String = basePath & "\" & GetFileCoordinate(xDataGML)

        If Not My.Computer.FileSystem.FileExists(lFileDaScrivere) Then
            My.Computer.FileSystem.WriteAllBytes(lFileDaScrivere, New Byte() {}, False)
        End If


    End Sub

    Private Function GetFileCoordinate(ByVal xDataGML As String) As String


        Dim coord As String() = GML.DammiArrayCoordinateDatoXml(xDataGML)

        '12.21694E44.56547N39H.POS
        Return coord(1).Substring(0, 8).Replace(",", ".") & "E" & coord(0).Substring(0, 8).Replace(",", ".") & "N41H.pos"

    End Function



    Private Shared Sub ZippaCartella(ByVal CartellaDaZippare As String)

        Dim files As ReadOnlyCollection(Of String)
        files = My.Computer.FileSystem.GetFiles(CartellaDaZippare, FileIO.SearchOption.SearchAllSubDirectories, "*")

        For Each myFile In files

            AgronicaCoreUtility.AgroZip.AddFileToZip(CartellaDaZippare & ".zip", myFile, CartellaDaZippare)
        Next


    End Sub




    Private Sub GeneraXmlConfig(ByVal dt As DataTable, ByVal fullfilenamewithpath As String)

        Dim maxUserValue As Integer = 19
        Dim maxFieldValue As Integer = 59

        Dim UserClientName As String = "UserClientName"


        Dim myDoc As XDocument = XDocument.Parse("<Configuration version=""0.0"" productname=""EZ-Guide 500"" productversion=""1.0"" xmlns=""http://EzOffice_ezg_profile""></Configuration>")

        Dim GroupRootNode As XElement =
            <Group name="Root" description="">

            </Group>

        Dim UserFieldFileNamesNode As XElement =
            <Group name="UserFieldFileNames" description="">
                <Group name="UserClientNames" description="">
                </Group>
                <Group name="UserFarmNames" description="">
                </Group>
                <Group name="UserFieldNames" description="">
                </Group>
            </Group>

        Dim UserEventNamesNode As XNode =
            <Group name="UserEventNames" description="">
                <String name="UserEventName0" description="" length="40" default="">Test suolo</String>
                <String name="UserEventName1" description="" length="40" default="">Lavorazione Terreno</String>
                <String name="UserEventName2" description="" length="40" default="">Piantagione_semina</String>
                <String name="UserEventName3" description="" length="40" default="">Applicazione</String>
                <String name="UserEventName4" description="" length="40" default="">Raccolto</String>
                <String name="UserEventName5" description="" length="40" default="">Esplorazione</String>
                <String name="UserEventName6" description="" length="40" default="">Cura bestiame</String>
                <String name="UserEventName7" description="" length="40" default="">Traino_trasporto</String>
                <String name="UserEventName8" description="" length="40" default="">Manutenzione azienda</String>
                <String name="UserEventName9" description="" length="40" default=""></String>
                <String name="UserEventName10" description="" length="40" default=""></String>
                <String name="UserEventName11" description="" length="40" default=""></String>
                <String name="UserEventName12" description="" length="40" default=""></String>
                <String name="UserEventName13" description="" length="40" default=""></String>
                <String name="UserEventName14" description="" length="40" default=""></String>
                <String name="UserEventName15" description="" length="40" default=""></String>
                <String name="UserEventName16" description="" length="40" default=""></String>
                <String name="UserEventName17" description="" length="40" default=""></String>
                <String name="UserEventName18" description="" length="40" default=""></String>
                <String name="UserEventName19" description="" length="40" default=""></String>
            </Group>

        Dim EventAttributesNode As XNode =
                <Group name="EventAttributes" description="">
                    <Group name="EventOperatorNames" description="">
                        <String name="EventOperatorName0" description="" length="40" default=""></String>
                        <String name="EventOperatorName1" description="" length="40" default=""></String>
                        <String name="EventOperatorName2" description="" length="40" default=""></String>
                        <String name="EventOperatorName3" description="" length="40" default=""></String>
                        <String name="EventOperatorName4" description="" length="40" default=""></String>
                        <String name="EventOperatorName5" description="" length="40" default=""></String>
                        <String name="EventOperatorName6" description="" length="40" default=""></String>
                        <String name="EventOperatorName7" description="" length="40" default=""></String>
                        <String name="EventOperatorName8" description="" length="40" default=""></String>
                        <String name="EventOperatorName9" description="" length="40" default=""></String>
                        <String name="EventOperatorName10" description="" length="40" default=""></String>
                        <String name="EventOperatorName11" description="" length="40" default=""></String>
                        <String name="EventOperatorName12" description="" length="40" default=""></String>
                        <String name="EventOperatorName13" description="" length="40" default=""></String>
                        <String name="EventOperatorName14" description="" length="40" default=""></String>
                        <String name="EventOperatorName15" description="" length="40" default=""></String>
                        <String name="EventOperatorName16" description="" length="40" default=""></String>
                        <String name="EventOperatorName17" description="" length="40" default=""></String>
                        <String name="EventOperatorName18" description="" length="40" default=""></String>
                        <String name="EventOperatorName19" description="" length="40" default=""></String>
                    </Group>
                    <Group name="EventEPANumbers" description="">
                        <String name="EventEPANumber0" description="" length="40" default=""></String>
                        <String name="EventEPANumber1" description="" length="40" default=""></String>
                        <String name="EventEPANumber2" description="" length="40" default=""></String>
                        <String name="EventEPANumber3" description="" length="40" default=""></String>
                        <String name="EventEPANumber4" description="" length="40" default=""></String>
                        <String name="EventEPANumber5" description="" length="40" default=""></String>
                        <String name="EventEPANumber6" description="" length="40" default=""></String>
                        <String name="EventEPANumber7" description="" length="40" default=""></String>
                        <String name="EventEPANumber8" description="" length="40" default=""></String>
                        <String name="EventEPANumber9" description="" length="40" default=""></String>
                        <String name="EventEPANumber10" description="" length="40" default=""></String>
                        <String name="EventEPANumber11" description="" length="40" default=""></String>
                        <String name="EventEPANumber12" description="" length="40" default=""></String>
                        <String name="EventEPANumber13" description="" length="40" default=""></String>
                        <String name="EventEPANumber14" description="" length="40" default=""></String>
                        <String name="EventEPANumber15" description="" length="40" default=""></String>
                        <String name="EventEPANumber16" description="" length="40" default=""></String>
                        <String name="EventEPANumber17" description="" length="40" default=""></String>
                        <String name="EventEPANumber18" description="" length="40" default=""></String>
                        <String name="EventEPANumber19" description="" length="40" default=""></String>
                    </Group>
                    <Group name="EventFarmLocationNames" description="">
                        <String name="EventFarmLocationName0" description="" length="40" default=""></String>
                        <String name="EventFarmLocationName1" description="" length="40" default=""></String>
                        <String name="EventFarmLocationName2" description="" length="40" default=""></String>
                        <String name="EventFarmLocationName3" description="" length="40" default=""></String>
                        <String name="EventFarmLocationName4" description="" length="40" default=""></String>
                        <String name="EventFarmLocationName5" description="" length="40" default=""></String>
                        <String name="EventFarmLocationName6" description="" length="40" default=""></String>
                        <String name="EventFarmLocationName7" description="" length="40" default=""></String>
                        <String name="EventFarmLocationName8" description="" length="40" default=""></String>
                        <String name="EventFarmLocationName9" description="" length="40" default=""></String>
                        <String name="EventFarmLocationName10" description="" length="40" default=""></String>
                        <String name="EventFarmLocationName11" description="" length="40" default=""></String>
                        <String name="EventFarmLocationName12" description="" length="40" default=""></String>
                        <String name="EventFarmLocationName13" description="" length="40" default=""></String>
                        <String name="EventFarmLocationName14" description="" length="40" default=""></String>
                        <String name="EventFarmLocationName15" description="" length="40" default=""></String>
                        <String name="EventFarmLocationName16" description="" length="40" default=""></String>
                        <String name="EventFarmLocationName17" description="" length="40" default=""></String>
                        <String name="EventFarmLocationName18" description="" length="40" default=""></String>
                        <String name="EventFarmLocationName19" description="" length="40" default=""></String>
                    </Group>
                    <Group name="EventVehicleNames" description="">
                        <String name="EventVehicleName0" description="" length="40" default=""></String>
                        <String name="EventVehicleName1" description="" length="40" default=""></String>
                        <String name="EventVehicleName2" description="" length="40" default=""></String>
                        <String name="EventVehicleName3" description="" length="40" default=""></String>
                        <String name="EventVehicleName4" description="" length="40" default=""></String>
                        <String name="EventVehicleName5" description="" length="40" default=""></String>
                        <String name="EventVehicleName6" description="" length="40" default=""></String>
                        <String name="EventVehicleName7" description="" length="40" default=""></String>
                        <String name="EventVehicleName8" description="" length="40" default=""></String>
                        <String name="EventVehicleName9" description="" length="40" default=""></String>
                        <String name="EventVehicleName10" description="" length="40" default=""></String>
                        <String name="EventVehicleName11" description="" length="40" default=""></String>
                        <String name="EventVehicleName12" description="" length="40" default=""></String>
                        <String name="EventVehicleName13" description="" length="40" default=""></String>
                        <String name="EventVehicleName14" description="" length="40" default=""></String>
                        <String name="EventVehicleName15" description="" length="40" default=""></String>
                        <String name="EventVehicleName16" description="" length="40" default=""></String>
                        <String name="EventVehicleName17" description="" length="40" default=""></String>
                        <String name="EventVehicleName18" description="" length="40" default=""></String>
                        <String name="EventVehicleName19" description="" length="40" default=""></String>
                    </Group>
                    <Group name="EventImplementNames" description="">
                        <String name="EventImplementName0" description="" length="40" default=""></String>
                        <String name="EventImplementName1" description="" length="40" default=""></String>
                        <String name="EventImplementName2" description="" length="40" default=""></String>
                        <String name="EventImplementName3" description="" length="40" default=""></String>
                        <String name="EventImplementName4" description="" length="40" default=""></String>
                        <String name="EventImplementName5" description="" length="40" default=""></String>
                        <String name="EventImplementName6" description="" length="40" default=""></String>
                        <String name="EventImplementName7" description="" length="40" default=""></String>
                        <String name="EventImplementName8" description="" length="40" default=""></String>
                        <String name="EventImplementName9" description="" length="40" default=""></String>
                        <String name="EventImplementName10" description="" length="40" default=""></String>
                        <String name="EventImplementName11" description="" length="40" default=""></String>
                        <String name="EventImplementName12" description="" length="40" default=""></String>
                        <String name="EventImplementName13" description="" length="40" default=""></String>
                        <String name="EventImplementName14" description="" length="40" default=""></String>
                        <String name="EventImplementName15" description="" length="40" default=""></String>
                        <String name="EventImplementName16" description="" length="40" default=""></String>
                        <String name="EventImplementName17" description="" length="40" default=""></String>
                        <String name="EventImplementName18" description="" length="40" default=""></String>
                        <String name="EventImplementName19" description="" length="40" default=""></String>
                    </Group>
                    <Group name="EventAppMethodNames" description="">
                        <String name="EventAppMethodName0" description="" length="40" default="">Pre-emergenza</String>
                        <String name="EventAppMethodName1" description="" length="40" default="">Pre-semina incorport</String>
                        <String name="EventAppMethodName2" description="" length="40" default="">Post-emergenza</String>
                        <String name="EventAppMethodName3" description="" length="40" default=""></String>
                        <String name="EventAppMethodName4" description="" length="40" default=""></String>
                        <String name="EventAppMethodName5" description="" length="40" default=""></String>
                        <String name="EventAppMethodName6" description="" length="40" default=""></String>
                        <String name="EventAppMethodName7" description="" length="40" default=""></String>
                        <String name="EventAppMethodName8" description="" length="40" default=""></String>
                        <String name="EventAppMethodName9" description="" length="40" default=""></String>
                        <String name="EventAppMethodName10" description="" length="40" default=""></String>
                        <String name="EventAppMethodName11" description="" length="40" default=""></String>
                        <String name="EventAppMethodName12" description="" length="40" default=""></String>
                        <String name="EventAppMethodName13" description="" length="40" default=""></String>
                        <String name="EventAppMethodName14" description="" length="40" default=""></String>
                        <String name="EventAppMethodName15" description="" length="40" default=""></String>
                        <String name="EventAppMethodName16" description="" length="40" default=""></String>
                        <String name="EventAppMethodName17" description="" length="40" default=""></String>
                        <String name="EventAppMethodName18" description="" length="40" default=""></String>
                        <String name="EventAppMethodName19" description="" length="40" default=""></String>
                    </Group>
                    <Group name="EventWindDirNames" description="">
                        <String name="EventWindDirName0" description="" length="40" default="">Nord</String>
                        <String name="EventWindDirName1" description="" length="40" default="">Nord-nordest</String>
                        <String name="EventWindDirName2" description="" length="40" default="">Nordest</String>
                        <String name="EventWindDirName3" description="" length="40" default="">Est-nordest</String>
                        <String name="EventWindDirName4" description="" length="40" default="">Est</String>
                        <String name="EventWindDirName5" description="" length="40" default="">Est-sudest</String>
                        <String name="EventWindDirName6" description="" length="40" default="">Sudest</String>
                        <String name="EventWindDirName7" description="" length="40" default="">Sud-sudest</String>
                        <String name="EventWindDirName8" description="" length="40" default="">Sud</String>
                        <String name="EventWindDirName9" description="" length="40" default="">Sud-sudovest</String>
                        <String name="EventWindDirName10" description="" length="40" default="">Sudovest</String>
                        <String name="EventWindDirName11" description="" length="40" default="">Ovest-sudovest</String>
                        <String name="EventWindDirName12" description="" length="40" default="">Ovest</String>
                        <String name="EventWindDirName13" description="" length="40" default="">Ovest-nordovest</String>
                        <String name="EventWindDirName14" description="" length="40" default="">Nordovest</String>
                        <String name="EventWindDirName15" description="" length="40" default="">Nord-nordovest</String>
                        <String name="EventWindDirName16" description="" length="40" default=""></String>
                        <String name="EventWindDirName17" description="" length="40" default=""></String>
                        <String name="EventWindDirName18" description="" length="40" default=""></String>
                        <String name="EventWindDirName19" description="" length="40" default=""></String>
                    </Group>
                    <Group name="EventSkyConditionsNames" description="">
                        <String name="EventSkyConditionsName0" description="" length="40" default="">Nuvoloso</String>
                        <String name="EventSkyConditionsName1" description="" length="40" default="">Parzialmente nuvolos</String>
                        <String name="EventSkyConditionsName2" description="" length="40" default="">Molto nuvoloso</String>
                        <String name="EventSkyConditionsName3" description="" length="40" default="">Soleggiato</String>
                        <String name="EventSkyConditionsName4" description="" length="40" default="">Parzialmente soleggt</String>
                        <String name="EventSkyConditionsName5" description="" length="40" default="">Molto soleggiato</String>
                        <String name="EventSkyConditionsName6" description="" length="40" default="">Nebbioso</String>
                        <String name="EventSkyConditionsName7" description="" length="40" default="">Coperto</String>
                        <String name="EventSkyConditionsName8" description="" length="40" default="">Piovoso</String>
                        <String name="EventSkyConditionsName9" description="" length="40" default="">Limpido</String>
                        <String name="EventSkyConditionsName10" description="" length="40" default=""></String>
                        <String name="EventSkyConditionsName11" description="" length="40" default=""></String>
                        <String name="EventSkyConditionsName12" description="" length="40" default=""></String>
                        <String name="EventSkyConditionsName13" description="" length="40" default=""></String>
                        <String name="EventSkyConditionsName14" description="" length="40" default=""></String>
                        <String name="EventSkyConditionsName15" description="" length="40" default=""></String>
                        <String name="EventSkyConditionsName16" description="" length="40" default=""></String>
                        <String name="EventSkyConditionsName17" description="" length="40" default=""></String>
                        <String name="EventSkyConditionsName18" description="" length="40" default=""></String>
                        <String name="EventSkyConditionsName19" description="" length="40" default=""></String>
                    </Group>
                    <Group name="EventSoilConditionsNames" description="">
                        <String name="EventSoilConditionsName0" description="" length="40" default="">Umido</String>
                        <String name="EventSoilConditionsName1" description="" length="40" default="">Secco</String>
                        <String name="EventSoilConditionsName2" description="" length="40" default=""></String>
                        <String name="EventSoilConditionsName3" description="" length="40" default=""></String>
                        <String name="EventSoilConditionsName4" description="" length="40" default=""></String>
                        <String name="EventSoilConditionsName5" description="" length="40" default=""></String>
                        <String name="EventSoilConditionsName6" description="" length="40" default=""></String>
                        <String name="EventSoilConditionsName7" description="" length="40" default=""></String>
                        <String name="EventSoilConditionsName8" description="" length="40" default=""></String>
                        <String name="EventSoilConditionsName9" description="" length="40" default=""></String>
                        <String name="EventSoilConditionsName10" description="" length="40" default=""></String>
                        <String name="EventSoilConditionsName11" description="" length="40" default=""></String>
                        <String name="EventSoilConditionsName12" description="" length="40" default=""></String>
                        <String name="EventSoilConditionsName13" description="" length="40" default=""></String>
                        <String name="EventSoilConditionsName14" description="" length="40" default=""></String>
                        <String name="EventSoilConditionsName15" description="" length="40" default=""></String>
                        <String name="EventSoilConditionsName16" description="" length="40" default=""></String>
                        <String name="EventSoilConditionsName17" description="" length="40" default=""></String>
                        <String name="EventSoilConditionsName18" description="" length="40" default=""></String>
                        <String name="EventSoilConditionsName19" description="" length="40" default=""></String>
                    </Group>
                    <Group name="EventSoilTypeNames" description="">
                        <String name="EventSoilTypeName0" description="" length="40" default="">Sabbioso</String>
                        <String name="EventSoilTypeName1" description="" length="40" default="">Argillo Sabbioso</String>
                        <String name="EventSoilTypeName2" description="" length="40" default="">Argilla</String>
                        <String name="EventSoilTypeName3" description="" length="40" default="">Terra argil.</String>
                        <String name="EventSoilTypeName4" description="" length="40" default="">Terra</String>
                        <String name="EventSoilTypeName5" description="" length="40" default="">Limo</String>
                        <String name="EventSoilTypeName6" description="" length="40" default=""></String>
                        <String name="EventSoilTypeName7" description="" length="40" default=""></String>
                        <String name="EventSoilTypeName8" description="" length="40" default=""></String>
                        <String name="EventSoilTypeName9" description="" length="40" default=""></String>
                        <String name="EventSoilTypeName10" description="" length="40" default=""></String>
                        <String name="EventSoilTypeName11" description="" length="40" default=""></String>
                        <String name="EventSoilTypeName12" description="" length="40" default=""></String>
                        <String name="EventSoilTypeName13" description="" length="40" default=""></String>
                        <String name="EventSoilTypeName14" description="" length="40" default=""></String>
                        <String name="EventSoilTypeName15" description="" length="40" default=""></String>
                        <String name="EventSoilTypeName16" description="" length="40" default=""></String>
                        <String name="EventSoilTypeName17" description="" length="40" default=""></String>
                        <String name="EventSoilTypeName18" description="" length="40" default=""></String>
                        <String name="EventSoilTypeName19" description="" length="40" default=""></String>
                    </Group>
                    <Group name="EventCropNames" description="">
                        <String name="EventCropName0" description="" length="40" default=""></String>
                        <String name="EventCropName1" description="" length="40" default=""></String>
                        <String name="EventCropName2" description="" length="40" default=""></String>
                        <String name="EventCropName3" description="" length="40" default=""></String>
                        <String name="EventCropName4" description="" length="40" default=""></String>
                        <String name="EventCropName5" description="" length="40" default=""></String>
                        <String name="EventCropName6" description="" length="40" default=""></String>
                        <String name="EventCropName7" description="" length="40" default=""></String>
                        <String name="EventCropName8" description="" length="40" default=""></String>
                        <String name="EventCropName9" description="" length="40" default=""></String>
                        <String name="EventCropName10" description="" length="40" default=""></String>
                        <String name="EventCropName11" description="" length="40" default=""></String>
                        <String name="EventCropName12" description="" length="40" default=""></String>
                        <String name="EventCropName13" description="" length="40" default=""></String>
                        <String name="EventCropName14" description="" length="40" default=""></String>
                        <String name="EventCropName15" description="" length="40" default=""></String>
                        <String name="EventCropName16" description="" length="40" default=""></String>
                        <String name="EventCropName17" description="" length="40" default=""></String>
                        <String name="EventCropName18" description="" length="40" default=""></String>
                        <String name="EventCropName19" description="" length="40" default=""></String>
                    </Group>
                    <Group name="EventTargetPestsNames" description="">
                        <String name="EventTargetPestsName0" description="" length="40" default=""></String>
                        <String name="EventTargetPestsName1" description="" length="40" default=""></String>
                        <String name="EventTargetPestsName2" description="" length="40" default=""></String>
                        <String name="EventTargetPestsName3" description="" length="40" default=""></String>
                        <String name="EventTargetPestsName4" description="" length="40" default=""></String>
                        <String name="EventTargetPestsName5" description="" length="40" default=""></String>
                        <String name="EventTargetPestsName6" description="" length="40" default=""></String>
                        <String name="EventTargetPestsName7" description="" length="40" default=""></String>
                        <String name="EventTargetPestsName8" description="" length="40" default=""></String>
                        <String name="EventTargetPestsName9" description="" length="40" default=""></String>
                        <String name="EventTargetPestsName10" description="" length="40" default=""></String>
                        <String name="EventTargetPestsName11" description="" length="40" default=""></String>
                        <String name="EventTargetPestsName12" description="" length="40" default=""></String>
                        <String name="EventTargetPestsName13" description="" length="40" default=""></String>
                        <String name="EventTargetPestsName14" description="" length="40" default=""></String>
                        <String name="EventTargetPestsName15" description="" length="40" default=""></String>
                        <String name="EventTargetPestsName16" description="" length="40" default=""></String>
                        <String name="EventTargetPestsName17" description="" length="40" default=""></String>
                        <String name="EventTargetPestsName18" description="" length="40" default=""></String>
                        <String name="EventTargetPestsName19" description="" length="40" default=""></String>
                    </Group>
                    <Group name="EventAppMaterialNames" description="">
                        <String name="EventAppMaterialName0" description="" length="40" default=""></String>
                        <String name="EventAppMaterialName1" description="" length="40" default=""></String>
                        <String name="EventAppMaterialName2" description="" length="40" default=""></String>
                        <String name="EventAppMaterialName3" description="" length="40" default=""></String>
                        <String name="EventAppMaterialName4" description="" length="40" default=""></String>
                        <String name="EventAppMaterialName5" description="" length="40" default=""></String>
                        <String name="EventAppMaterialName6" description="" length="40" default=""></String>
                        <String name="EventAppMaterialName7" description="" length="40" default=""></String>
                        <String name="EventAppMaterialName8" description="" length="40" default=""></String>
                        <String name="EventAppMaterialName9" description="" length="40" default=""></String>
                        <String name="EventAppMaterialName10" description="" length="40" default=""></String>
                        <String name="EventAppMaterialName11" description="" length="40" default=""></String>
                        <String name="EventAppMaterialName12" description="" length="40" default=""></String>
                        <String name="EventAppMaterialName13" description="" length="40" default=""></String>
                        <String name="EventAppMaterialName14" description="" length="40" default=""></String>
                        <String name="EventAppMaterialName15" description="" length="40" default=""></String>
                        <String name="EventAppMaterialName16" description="" length="40" default=""></String>
                        <String name="EventAppMaterialName17" description="" length="40" default=""></String>
                        <String name="EventAppMaterialName18" description="" length="40" default=""></String>
                        <String name="EventAppMaterialName19" description="" length="40" default=""></String>
                    </Group>
                </Group>

        Dim listaClienti As List(Of String) =
            (From drows In dt.AsEnumerable
             Group By g = drows("Rag_soc").ToString
             Into dRag = Group
             Select g
             ).ToList

        Dim listaCentri As List(Of String) =
            (From drows In dt.AsEnumerable
             Group By g = drows("sa_nome").ToString
             Into dRag = Group
             Select g
             ).ToList

        Dim listaCampi As List(Of String) =
            (From drows In dt.AsEnumerable
             Group By g = drows("campo_des").ToString & drows("app_nome").ToString
             Into dRag = Group
             Select g
             ).ToList

        Generalista(maxUserValue, "UserClientName", "UserClientNames", UserFieldFileNamesNode, listaClienti)
        Generalista(maxUserValue, "UserFarmName", "UserFarmNames", UserFieldFileNamesNode, listaCentri)
        Generalista(maxFieldValue, "UserFieldName", "UserFieldNames", UserFieldFileNamesNode, listaCampi)

        UserFieldFileNamesNode.Add(UserEventNamesNode)
        GroupRootNode.Add(UserFieldFileNamesNode)
        GroupRootNode.Add(EventAttributesNode)

        Dim listaNS As New List(Of String)

        listaNS.Add("http://EzOffice_ezg_profile")
        myDoc.<Configuration>.FirstOrDefault.Add(GroupRootNode)
        myDoc = xmlHelper.RemoveNamespace(myDoc, listaNS)
        myDoc.Save(fullfilenamewithpath)

    End Sub


    Private Shared Sub Generalista(ByVal maxUserValue As Integer, ByVal NameToSet As String, ByVal NameToSearch As String, ByVal Node As XElement, ByVal listaClienti As List(Of String))
        Dim i As Integer = 0

        Dim NodeToAdd As XElement =
                        (From n In Node.<Group>
                         Where n.@name = NameToSearch).FirstOrDefault


        For Each cli In listaClienti
            cli = RimuoviCaratteriIllegali(cli, _lenName)
            Dim myMode = <String name=<%= NameToSet & i.ToString %> description="" length="40" default=""><%= cli %></String>
            i += 1



            NodeToAdd.Add(myMode)


        Next

        For k As Integer = i To maxUserValue
            Dim myMode = <String name=<%= NameToSet & k.ToString %> description="" length="40" default=""></String>

            NodeToAdd.Add(myMode)
        Next
    End Sub

    Public Shared Function EsportaShp(
                    ByVal Entita_Cod As Int32,
                    ByVal TipoEntita_Cod As Int32,
                    ByVal PivaPadre As String,
                    ByVal Piva As String,
                    ByVal Sa_Cod As Int32,
                    ByVal Appezza As Int32,
                    ByVal Campo_Cod As Int32,
                    ByVal ID_Imp As Int32,
                    ByVal Prov As String,
                    ByVal Com As String,
                    ByVal Sezione As String,
                    ByVal Foglio As Int32,
                    ByVal Numero As Int32,
                    ByVal Subalterno As String,
                    ByVal ID_Agenda As Int32,
                    ByVal Ricetta_Operazione_Cod As Integer,
                    ByVal Programmazione_cod As Integer,
                    ByVal Programmazione_Entita_cod As Integer,
                    ByVal filename As String,
                    ByVal importaAttributiTipizzati As Boolean,
                    ByVal Codice_Fiscale_Tecnico As String,
                    ByRef objParametri_Server As AgronicaCoreParametri,
                    ByRef objParametri_Utenti As AgronicaCoreParametri
                    ) As String


        Dim leggi As New AgronicaCoreGisDAL.GIS_Entita_R
        Dim xmlToConvert As String = leggi.LeggiXML(objParametri_Server.PivaSuperUser, Entita_Cod, TipoEntita_Cod, PivaPadre, Piva, Sa_Cod, Appezza, Campo_Cod, ID_Imp,
                                                    Prov, Com, Sezione, Foglio, Numero, Subalterno, ID_Agenda, Ricetta_Operazione_Cod,
                                                    Programmazione_cod, Programmazione_Entita_cod, False, Codice_Fiscale_Tecnico, "", Nothing,
                                                    AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server, objParametri_Utenti)

        Dim SHPWrap As New AgronicaSHPWrapper.AgronicaGis2012ToShapeVarie
        Return SHPWrap.EsportaShpDatoXml(filename, xmlToConvert, True, importaAttributiTipizzati, New List(Of DBFDataModel_MappaturaDati))
    End Function

End Class
