
Imports <xmlns="http://www.agronica.it/track/">

Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports Newtonsoft.Json
Imports System.Drawing
Imports System.IO

Public Class FF_Track

    Private xDresult As XDocument

    Private _stackToTrack As New List(Of stackToTrack)
    Private _NextGroupToTrack As New List(Of blockToTrack)

#Region "Algoritmi di rintracciata"

    Public Function OttieniDescrizioneProdottoDatoLotto(
                ByVal LotToTrack As String, ByVal IdMovDetSelected As Integer,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
            ) As String


        Dim dt As DataTable
        Dim xLeggi As New FF_Trace_R

        dt =
        xLeggi.LeggiDescrizioneProdottoDatoCodiceLotto(LotToTrack, IdMovDetSelected, objParametri)

        Dim rVal As String = ""
        If dt.Rows.Count > 0 Then

            rVal &= "Prodotto Ricercato: "
            rVal &= dt.Rows(0)("Mat_Des")
            rVal &= " - Lotto: "
            rVal &= LotToTrack

        End If

        Return rVal
    End Function

    Public Function OttieniImmagineProdottoDatoLotto(
                ByVal LotToTrack As String,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
            ) As String


        Dim dt As DataTable
        Dim xLeggi As New FF_Trace_R

        dt =
        xLeggi.LeggiImmagineProdottoDatoCodiceLotto_daInterfacciamenti(LotToTrack, objParametri)

        Dim rVal As String = ""
        If dt.Rows.Count > 0 Then

            rVal &= dt.Rows(0)("DirPicture")

        End If

        Return rVal

    End Function

    Public Function TrackMe(
            ByVal xmlTrack As String,
            ByVal LeggiXmlCompleto As Boolean,
            ByVal DiagramFormat As Boolean,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
            Optional ByVal AggiungiDettaglioAziendaAppezza As Boolean = False,
            Optional ByVal AggiungiDettaglioVeicolo As Boolean = False,
            Optional ByVal SwitchMatCodXPOC As Boolean = False
        ) As String

        Dim lns As String = "xmlns=""http://www.agronica.it/track/"""
        If Not xmlTrack.Contains(lns) Then
            xmlTrack = xmlTrack.Replace("<track", "<track " & lns)
        End If

        xDresult = XDocument.Parse(xmlTrack)

        Dim listOfLotti As New List(Of String)

        Dim trk As New FF_Trace_R
        Dim trackCFG As New FF_TrackCFG_R
        Dim FFCache As New FF_TrackedData_R

        Dim dtCfg As DataTable
        Dim progressivo As Integer = 1
        Dim ricarica As Boolean
        Dim FromOutToIn As Boolean


        For Each ll In xDresult.<track>.<lotsToTrack>.<lot>

            Dim cLotto As String
            Dim cTipo As Integer
            Dim cCalCod As Integer = 0
            Dim cIdMovDet As Integer = 0
            Dim cCertificazione As Integer = 0
            Dim cPiva As String = ""

            Dim UltimaDataMov As Date = Date.ParseExact("21001231", "yyyyMMdd", Globalization.CultureInfo.InvariantCulture)

            cLotto = ll.<inputData>.<lotto>.Value
            cTipo = ll.<inputData>.<tipolotto>.Value
            If Not String.IsNullOrEmpty(ll.<inputData>.<xCalCodSelected>.Value) Then
                cCalCod = ll.<inputData>.<xCalCodSelected>.Value
            End If
            If Not String.IsNullOrEmpty(ll.<inputData>.<xIdMovDetSelected>.Value) Then
                cIdMovDet = ll.<inputData>.<xIdMovDetSelected>.Value
            End If
            If Not String.IsNullOrEmpty(ll.<inputData>.<xCertificazione>.Value) Then
                cCertificazione = ll.<inputData>.<xCertificazione>.Value
            End If
            If Not String.IsNullOrEmpty(ll.<inputData>.<xPiva>.Value) Then
                cPiva = ll.<inputData>.<xPiva>.Value
            End If
            If ll.<inputData>.<ricarica>.Value = "1" Then
                ricarica = True
            End If
            If ll.<inputData>.<xFromOutToIn>.Value = "1" Then
                FromOutToIn = True
            End If

            If FromOutToIn Then
                'Cerca data limite movimenti

                Dim mdR As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
                Dim dt As New DataTable

                If cCalCod <> 0 AndAlso cCertificazione <> 0 AndAlso cIdMovDet <> 0 Then
                    'Singolo DDT
                    dt = mdR.MovimentiDettagli_Leggi_FF(
                                 cPiva, 0, "", 0, 0, cIdMovDet, 0,
                                 0, 0, 0, 0, cLotto, 0, 0, Nothing,
                                     Nothing, 0, 0, "", "", "", "", objParametri)
                    If dt.Rows.Count > 0 Then
                        UltimaDataMov = Convert.ToDateTime(dt.Rows(0)("Data_Movimento"))
                    End If
                Else
                    'Lavorazione più recente
                    'Dim Lav_Cod As Integer() = {LAVCOD_TRASFORMAZIONI}
                    'dt = mdR.MovimentiDettagli_Leggi_FF(
                    '             cPiva, 0, "", 0, 0, cIdMovDet, 0,
                    '             0, 0, 0, 0, cLotto, 0, 0, Lav_Cod,
                    '                 Nothing, 0, 0, "", "", "", "", objParametri)
                    'For Each dr In dt.Rows
                    '    If Convert.ToDateTime(dr("Data_Movimento")) > UltimaDataMov Then
                    '        UltimaDataMov = Convert.ToDateTime(dr("Data_Movimento"))
                    '    End If
                    'Next
                End If

            End If

            'leggo da cache oppure esegue la rintracciata ..
            Dim dtCache As DataTable =
                FFCache.VerificaEsistenza(cLotto, cCalCod, cIdMovDet, cTipo, FromOutToIn, "", "", objParametri)

            If dtCache.Rows.Count = 0 Or ricarica Then
                ClearCache(ll, dtCache, objParametri)
                DoTrack(objParametri, trk, trackCFG, dtCfg, progressivo, ll, cLotto, cTipo, cCalCod, cCertificazione, FromOutToIn, UltimaDataMov, AggiungiDettaglioAziendaAppezza, DiagramFormat, AggiungiDettaglioVeicolo, SwitchMatCodXPOC)
                SaveToCache(ll, cLotto, cCalCod, cIdMovDet, cTipo, FromOutToIn, objParametri)
            Else
                dtCache = FFCache.VerificaEsistenza(cLotto, cCalCod, cIdMovDet, cTipo, FromOutToIn, "", "", objParametri)
                LeggiDaCache(ll, dtCache, LeggiXmlCompleto, cTipo, objParametri, AggiungiDettaglioVeicolo, SwitchMatCodXPOC)
            End If





        Next


        Dim rs As String = ResultToString(xDresult)
        writeOutputToXmlFile(rs)
        Return rs

    End Function


    Private Function ClearCache(ByVal ll As System.Xml.Linq.XElement, ByVal dtCache As DataTable, ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        If dtCache.Rows.Count = 0 Then
            Return "Ok"
        End If

        Dim oCod As Integer


        Dim oDettagli As New FF_TrackedData_Agenda_W
        Dim oTestata As New FF_TrackedData_W

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            oCod = dtCache(0)("FF_TrackedData_Cod")

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(
                    FlagConnessioneLocale,
                    FlagTransazioneLocale,
                    objParametri
            )

            oDettagli.Cancella(
                              oCod _
                             , "" _
                             , objParametri
                            )

            oTestata.Cancella(
                  oCod _
                 , "" _
                 , objParametri
                )

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la transazione al DB
            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)
            '''''''''

        Catch ex As Exception

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri)

            End If

            Return "Errore " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True, "<br />")

        Finally
            'Chiudo la connessione al DB
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
        End Try

        Return "Ok"

    End Function

    Private Function LeggiDaCache(ByVal ll As System.Xml.Linq.XElement,
                                  ByVal dtCache As DataTable, LeggiXmlCompleto As Boolean,
                                  ByVal cTipo As Integer,
                                  ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                  ByVal AggiungiDettaglioVeicolo As Boolean,
                                  Optional ByVal SwitchMatCodXPOC As Boolean = False) As String

        Dim FF_TrackedData_Cod As Integer = dtCache(0)("FF_TrackedData_Cod")
        Dim elem = <FF_TrackedData_Cod><%= FF_TrackedData_Cod %></FF_TrackedData_Cod>
        ll.<inputData>.FirstOrDefault.Add(elem)

        If LeggiXmlCompleto Then

            Dim xLetturaCache As New AgronicaCoreContabDAL.FF_TrackedData_Agenda_R
            Dim rTipoLotto As New FF_Tipo_Lotto_R

            Dim rMetaSchema = New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R
            Dim dtCacheCompleta As DataTable =
                xLetturaCache.LeggiXCache(FF_TrackedData_Cod, "", "", objParametri, SwitchMatCodXPOC)

            'lavezzo - 30/03/2021 - aggiungo descrizione prodotto
            For Each row In dtCacheCompleta.Rows
                Dim dtDescItem = rMetaSchema.LeggiTabella_da_CategorieMagazzino(row("Tabella"), row("Tabella_Cod"), row("Tabella_Des"), "", IIf(row("Pro_Cod") = 0, row("Mat_Cod"), row("Pro_Cod")), "", "", objParametri)
                If dtDescItem.Rows.Count > 0 Then
                    row("prodotto") = dtDescItem(0)(row("Tabella_Des"))
                End If
            Next


            dtCacheCompleta.TableName = "datiGias"
            Dim ds1 As New DataSet
            ds1.DataSetName = "trasformazioni"
            ds1.Tables.Add(dtCacheCompleta)

            ds1.Tables(0).Columns.Remove("Tabella")
            ds1.Tables(0).Columns.Remove("Tabella_Cod")
            ds1.Tables(0).Columns.Remove("Tabella_Des")

            Dim result As String

            Using sw As StringWriter = New StringWriter()
                dtCacheCompleta.WriteXml(sw)
                result = sw.ToString()
            End Using


            Dim xD1 As XDocument = XDocument.Parse(result)
            Dim n As XNamespace = "http: //www.agronica.it/track/"
            Dim nodoOut = <outputData><trasformazioni></trasformazioni></outputData>

            Dim flgWeb = rTipoLotto.CheckTipoLottoXFlusso(dtCache(0)("Tipo"), TipiEnumerativi.FF_Track_Flusso.WebApi, "", "", objParametri)
            'lavez - 09/06/2021 - flag esponi dati GIS
            Dim EsponiDatiGIS = rTipoLotto.CheckEsponiDatiGISxTipoLotto(dtCache(0)("Tipo"), "", "", objParametri)

            For Each nodoDati In xD1.Element("trasformazioni").Elements("datiGias")
                Dim gg = AddDetailCentroAziendale(nodoDati.Element("piva"), nodoDati.Element("sa_cod"), nodoDati.Element("id_agenda"), nodoDati.Element("id_mov"), nodoDati.Element("id_mov_det"), nodoDati.Element("lav_cod"), nodoDati.Element("Data_Movimento"), EsponiDatiGIS, objParametri, AggiungiDettaglioVeicolo)
                If gg IsNot Nothing Then
                    nodoDati.Add(gg)
                End If
                Dim n1 = <trasformazione_dati></trasformazione_dati>
                n1.Add(nodoDati)
                nodoOut.<trasformazioni>.FirstOrDefault.Add(n1)
            Next

            ll.Add(nodoOut)

        End If

    End Function


    Private Function SaveToCache(ByVal ll As System.Xml.Linq.XElement, ByVal cLotto As String, ByVal cCalCod As Integer, ByVal cIdMovDet As Integer, ByVal cTipo As Integer, ByVal FromOutToIn As Boolean, ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim agroSequeze As New AgronicaCoreDataProvider.Agro_Sequenze
        'Dim oCod As Integer =
        '    agroSequeze.Agronica_SequenzaTabelle_NuovoID("FF_TrackedData", objParametri)
        'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
        Dim oCod As Integer =
            agroSequeze.NuovoId_Tabella("FF_TrackedData", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri)

        Dim oDettagli As New FF_TrackedData_Agenda_W
        Dim oTestata As New FF_TrackedData_W

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(
                    FlagConnessioneLocale,
                    FlagTransazioneLocale,
                    objParametri
            )

            oTestata.Scrivi(
                  oCod _
                , "Lotto: '" & cLotto & ", Tipo: " & cTipo _
                , cLotto _
                , cCalCod _
                , cIdMovDet _
                , cTipo _
                , FromOutToIn _
                 , AGRODATAINIZIO _
                 , AGRODATAFINE _
                 , objParametri
                )

            ' Non scrivo i movimenti di tipo -208 (unificazione lotti); questi vengono comunque letti 
            ' per poter risalire a tutta la catena dei movimenti
            ' Nel caso in cui in futuri si riesca a utilizzare il TreeView o Grafi occorrerà avere anche questi movimenti
            Dim lAgenda As List(Of trackToSave)
            lAgenda = (
                From aG In ll.<outputData>.<trasformazioni>.<trasformazione_dati>.<datiGias>
                Where (aG.<codice_generazione>.Value <> -208)
                Select New trackToSave With {
                   .Piva = aG.<piva>.Value _
                   , .Sa_cod = aG.<sa_cod>.Value _
                   , .Id_agenda = aG.<id_agenda>.Value _
                   , .Id_mov = aG.<id_mov>.Value _
                   , .Id_mov_det = aG.<id_mov_det>.Value _
                   , .Lotto = aG.<Lotto>.Value _
                   , .Lotto_Padre = aG.<Lotto_Padre>.Value _
                    , .Cal_Cod = aG.<cal_Cod>.Value _
                    , .Cal_Cod_Padre = aG.<cal_Cod_Padre>.Value _
                   , .Qta_Extra_Totale = aG.<Qta_Extra_Totale>.Value.Replace(".", System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator) _
                    , .Piva_Padre = aG.<piva_Padre>.Value _
                    , .Sa_cod_Padre = aG.<sa_Cod_Padre>.Value _
                    , .id_agenda_padre = aG.<id_agenda_padre>.Value _
                    , .id_mov_padre = aG.<id_mov_padre>.Value _
                    , .id_mov_det_padre = aG.<id_mov_det_padre>.Value _
                    , .DataMovimento = aG.<Data_Movimento>.Value _
                    , .NumeroDocumento = aG.<numero_documento>.Value _
                    , .DataDocumento = aG.<data_documento>.Value _
                    , .IntestatarioDocumentoRagioneSociale = aG.<intestatario_documento_ragione_sociale>.Value _
                    , .NumeroDocumentoFornitore = aG.<numero_documento_fornitore>.Value _
                    , .DataDocumentoFornitore = aG.<data_documento_fornitore>.Value _
                    , .Visibile = aG.<visibile>.Value _
                    , .Ordine = aG.<ordine>.Value
               }).Distinct.ToList



            For Each cAg In lAgenda

                oDettagli.Scrivi(
                    oCod,
                    cAg.Piva,
                    cAg.Sa_cod,
                    cAg.Id_agenda,
                    cAg.Id_mov,
                    cAg.Id_mov_det,
                    cAg.Lotto,
                    cAg.Lotto_Padre,
                    cAg.Cal_Cod,
                    cAg.Cal_Cod_Padre,
                    cAg.Qta_Extra_Totale,
                    cAg.Piva_Padre,
                    cAg.Sa_cod_Padre,
                    cAg.id_agenda_padre,
                    cAg.id_mov_padre,
                    cAg.id_mov_det_padre,
                    cAg.DataMovimento,
                    cAg.NumeroDocumento,
                    cAg.DataDocumento,
                    cAg.IntestatarioDocumentoRagioneSociale,
                    cAg.NumeroDocumentoFornitore,
                    cAg.DataDocumentoFornitore,
                    Convert.ToInt32(cAg.Visibile),
                    cAg.Ordine,
                    AGRODATAINIZIO,
                    AGRODATAFINE,
                    objParametri
                )

            Next

            Dim elem = <FF_TrackedData_Cod><%= oCod %></FF_TrackedData_Cod>
            ll.<inputData>.FirstOrDefault.Add(elem)

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la transazione al DB
            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)
            '''''''''

        Catch ex As Exception

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri)

            End If

            Return "Errore " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True, "<br/>")

        Finally
            'Chiudo la connessione al DB
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
        End Try

        Return "Ok"

    End Function


    Private Sub DoTrack(
         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
         ByVal trk As FF_Trace_R,
         ByVal trackCFG As FF_TrackCFG_R,
         ByRef dtCfg As DataTable,
         ByRef progressivo As Integer,
         ByVal ll As System.Xml.Linq.XElement,
         ByVal cLotto As String,
         ByVal cTipo As Integer,
         ByVal cCalCod As Integer,
         ByVal cCertificazione As Integer,
         ByVal FromOutToIn As Boolean,
         ByVal UltimaDataMov As Date,
         ByVal AggiungiDettaglioAziendaAppezza As Boolean,
         ByVal DiagramFormat As Boolean,
         ByVal AggiungiDettaglioVeicolo As Boolean,
         Optional ByVal SwitchMatCodXPOC As Boolean = False)

        'Punto di ingresso per la rintracciata


        Dim elem = <outputData>
                       <trasformazioni></trasformazioni>
                   </outputData>

        ll.Add(elem)

        'TODO Stefano 
        ' Questo è da perfezionare: è stato messo perchè se si legge per singolo calCod che rappresenta un documento di vendita o di entrata 
        ' non dobbiamo selezionare altri movimenti di vendita o idi entrata paralleli
        Dim xFiltroAgg As String = ""
        If cCalCod <> 0 Then
            If FromOutToIn Then
                xFiltroAgg = " ordine > 2 "
            Else
                xFiltroAgg = " ordine <5 "
            End If
        End If

        If FromOutToIn Then
            dtCfg = trackCFG.Leggi(cTipo, False, xFiltroAgg, " ordine asc ", objParametri)
        Else
            dtCfg = trackCFG.Leggi(cTipo, False, xFiltroAgg, " ordine desc ", objParametri)
        End If

        progressivo = 1

        Dim BarcodeOPTA As String = ""

        If cTipo = 2 Or cTipo = 3 Then
            Dim lBCode As New AgronicaCoreContabDAL.FF_Opta_Track
            'se sto rintracciando un barcode oppure un lotto OPTA
            If cTipo = 2 Then

                'mi devo procurare il barcode

                Dim lCodeDT As DataTable = lBCode.Barcode_Da_Lotto(cLotto, "", "", objParametri)
                If lCodeDT.Rows.Count > 0 Then
                    BarcodeOPTA = lCodeDT(0)("Barcode")
                End If

            Else
                'mi devo procurare il lotto                
                BarcodeOPTA = cLotto
                Dim lCodeDT As DataTable = lBCode.Lotto_Da_Barcode("01", BarcodeOPTA, "", "", objParametri)
                If lCodeDT.Rows.Count > 0 Then
                    cLotto = lCodeDT(0)("lotto")
                Else
                    lCodeDT = lBCode.Lotto_Da_Barcode("02", BarcodeOPTA, "", "", objParametri)
                    If lCodeDT.Rows.Count > 0 Then
                        cLotto = lCodeDT(0)("lotto")
                    Else
                        cLotto = "-1"
                    End If

                End If
            End If


            'caso: opta, rintraccio le operazioni di agenda dalle tabelle pre-compilate gli impianti..
            Dim lTRkOPTA As New FF_OPTA_Track
            lTRkOPTA.TrackImpiantiFromBarcode(
                BarcodeOPTA,
                1,
                1,
                progressivo,
                trk,
                elem,
                "",
                objParametri,
                AggiungiDettaglioAziendaAppezza
            )

        End If

        'accodo il lotto da rintracciare
        EnqueueLot(cLotto, 0, cCalCod, 1)

        'lavez - 09/06/2021 - introdotto flag per pilotare l'esposizione dei dati GIS si/no
        Dim FFTipoLotto As New FF_Tipo_Lotto_R
        Dim EsponiDatiGIS As Boolean = FFTipoLotto.CheckEsponiDatiGISxTipoLotto(cTipo, "", "", objParametri)

        'Rintraccia il primo lotto
        While _NextGroupToTrack.Count > 0

            Dim oToTrack As blockToTrack = _NextGroupToTrack.First

            'Escludo il tracking dei lotti di tipo Indefinito
            If oToTrack.Lotto.ToLower <> "indefinito" AndAlso oToTrack.Lotto <> "" Then
                'Rintraccia le configurazioni
                For Each rowDtCfg As DataRow In dtCfg.Rows

                    If FromOutToIn Then

                        TrackALot(oToTrack.Lotto, rowDtCfg("lav_cod"), rowDtCfg("cau_mov_in"), rowDtCfg("cau_mov_out"), oToTrack.Mat_cod, oToTrack.Cal_Cod, cTipo, rowDtCfg("FF_TrackCFG_Cod"), oToTrack.PercentualeContributo, rowDtCfg("ordine"), progressivo, rowDtCfg("Ricorsivo"), rowDtCfg("Visibilita"), rowDtCfg("PREPARAZIONE_COD"), 0, trk, trackCFG, elem, cLotto, cCertificazione, UltimaDataMov, objParametri, AggiungiDettaglioAziendaAppezza, DiagramFormat, FromOutToIn, EsponiDatiGIS, AggiungiDettaglioVeicolo, SwitchMatCodXPOC)
                    Else
                        If CStr(rowDtCfg("cau_mov_out")) <> "" And CStr(rowDtCfg("cau_mov_in")) <> "" Then
                            TrackALot(oToTrack.Lotto, rowDtCfg("lav_cod"), rowDtCfg("cau_mov_out"), rowDtCfg("cau_mov_in"), oToTrack.Mat_cod, oToTrack.Cal_Cod, cTipo, rowDtCfg("FF_TrackCFG_Cod"), oToTrack.PercentualeContributo, rowDtCfg("ordine"), progressivo, rowDtCfg("Ricorsivo"), rowDtCfg("Visibilita"), 0, rowDtCfg("PREPARAZIONE_COD"), trk, trackCFG, elem, cLotto, cCertificazione, UltimaDataMov, objParametri, AggiungiDettaglioAziendaAppezza, DiagramFormat, FromOutToIn, EsponiDatiGIS, AggiungiDettaglioVeicolo, SwitchMatCodXPOC)
                        Else
                            TrackALot(oToTrack.Lotto, rowDtCfg("lav_cod"), rowDtCfg("cau_mov_in"), rowDtCfg("cau_mov_out"), oToTrack.Mat_cod, oToTrack.Cal_Cod, cTipo, rowDtCfg("FF_TrackCFG_Cod"), oToTrack.PercentualeContributo, rowDtCfg("ordine"), progressivo, rowDtCfg("Ricorsivo"), rowDtCfg("Visibilita"), rowDtCfg("PREPARAZIONE_COD"), 0, trk, trackCFG, elem, cLotto, cCertificazione, UltimaDataMov, objParametri, AggiungiDettaglioAziendaAppezza, DiagramFormat, FromOutToIn, EsponiDatiGIS, AggiungiDettaglioVeicolo, SwitchMatCodXPOC)
                        End If
                    End If

                Next
            End If

            progressivo += 1
            _NextGroupToTrack.Remove(oToTrack)

        End While
    End Sub
    Private Sub writeOutputToXmlFile(ByVal sXml As String)

        Dim fileTosave As String = "C:\TFS_AreaLavoro\Gias\RamoPrincipale\Src\GiasDotNet\AgronicaCore_2010\AgronicaCoreContabBIZ\Track\Xml\TrackEsempio1.xml"
        If My.Computer.FileSystem.FileExists(fileTosave) And Not _
            My.Computer.FileSystem.GetFileInfo(fileTosave).IsReadOnly Then

            My.Computer.FileSystem.WriteAllText(fileTosave, sXml, False)
        End If

    End Sub

    Private Sub EnqueueLot(ByVal Lotto As String, ByVal Mat_cod As Integer, ByVal cal_cod As Integer, ByVal PercentualeContributo As Decimal)

        If (From i In _NextGroupToTrack Where i.Mat_cod = Mat_cod And i.Lotto = Lotto And i.Cal_Cod = cal_cod).Count = 0 Then

            _NextGroupToTrack.Add(
                New blockToTrack With {
                    .Lotto = Lotto,
                    .Mat_cod = Mat_cod,
                    .Cal_Cod = cal_cod,
                    .PercentualeContributo = PercentualeContributo
            })

        End If

    End Sub



    Private Sub TrackALot(
        ByVal lot As String,
        ByVal lav_cod As Integer,
        ByVal cau_mov_in As String,
        ByVal cau_mov_out As String,
        ByVal mat_cod As Integer,
        ByVal cal_Cod As Integer,
        ByVal cTipo As Integer,
        ByVal cfg As Integer,
        ByVal percentuale_contributo_precedente As Decimal,
        ByVal ordine As Integer,
        ByVal progressivo As Integer,
        ByVal Ricorsivo As Integer,
        ByVal Visibile As Boolean,
        ByVal Preparazione_Cod_in As Integer,
        ByVal Preparazione_Cod_out As Integer,
        ByVal trk As FF_Trace_R,
        ByVal trackCFG As FF_TrackCFG_R,
        ByRef elem As XElement,
        ByVal cLotto As String,
        ByVal cCertificazione As Integer,
        ByVal UltimaDataMov As Date,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        ByVal AggiungiDettaglioAziendaAppezza As Boolean,
        ByVal DiagramFormat As Boolean,
        ByVal FromOutToIn As Boolean,
        ByVal EsponiDatiGIS As Boolean,
        ByVal AggiungiDettaglioVeicolo As Boolean,
        Optional ByVal SwitchMatCodXPOC As Boolean = False      'lavez - 21/01/2022 - switch per mat_cod da 3 a 2 in caso di confezionamento x POC Antares
    )


        Dim dtTrKOperazioniConf As DataTable
        Dim rTrackCFG As New FF_TrackCFG_R
        Dim rTipoLotto As New FF_Tipo_Lotto_R

        Dim outpuNode = <output></output>

        Dim idAg As String = ""

        ' Stefano - 21/6/2017:  facendo così si perde dei movimenti nel caso in cui ci siano lotti diversi 
        '                       all'interno della stessa agenda (ad es. accettazione)
        'idAg = String.Join(",", (From ids In _stackToTrack Select ids.Id_agenda).ToList.ToArray)
        'idAg = String.Join(",", (From ids In _stackToTrack Where ids.Lotto = lot Select ids.Id_agenda).ToList.ToArray)
        ' Fine Stefano - 21/6/2017

        If Not String.IsNullOrEmpty(idAg) Then
            idAg = " A.id_agenda not in (" & idAg & ")"
        End If


        dtTrKOperazioniConf = trk.Leggi(
                        lot,
                        lav_cod,
                        cau_mov_in,
                        cau_mov_out,
                        mat_cod,
                        cal_Cod,
                        Preparazione_Cod_in,
                        Preparazione_Cod_out,
                       Ricorsivo,
                         idAg,
                        "",
                       cCertificazione,
                        UltimaDataMov,
                        objParametri
                    )




        For Each dd As DataRow In dtTrKOperazioniConf.Rows

            Dim lId_agenda As Integer = dd("id_Agenda")
            Dim lLotto As String = dd("Lotto")
            Dim lpiva As String = dd("piva")
            Dim lId_mov_det As Integer = dd("id_mov_det")
            Dim wCalCod As Integer = CInt(dd("cal_cod"))
            'Dim wCalCod As Integer = GetCalCodCurrentRecord(dd, lav_cod, objParametri) 'CInt(dd("cal_cod"))

            'N.B.  questa If è necessaria per creare correttamente il TreeView
            If wCalCod = cal_Cod Then
                wCalCod = -2000000000 + cal_Cod
            End If

            ''nel caso sia visibile salvo l'id movimento dettaglio nella cache di estrazione dati e lo visualizzo nell'xml
            'If (From s In _stackToTrack Where s.Id_agenda = lId_agenda And s.Id_Mov_Det = lId_mov_det And s.Lotto = lLotto).Count = 0 AndAlso lLotto.ToLower <> "indefinito" Then
            '    _stackToTrack.Add(New stackToTrack With {.Id_agenda = lId_agenda, .Id_Mov_Det = lId_mov_det, .Lotto = lLotto, .Piva = lpiva, .Gestito = 1})
            'End If

            Dim lPercentuale_contributo As Decimal
            lPercentuale_contributo = dd("percentuale_contributo") * percentuale_contributo_precedente

            Dim newCalCodPadre As Integer = cal_Cod

            Dim checkChiavePadreFiglio As Boolean = False
            If dd("piva_padre") = dd("piva") And dd("sa_cod_padre") = dd("sa_cod") And dd("id_agenda") = dd("id_agenda_padre") And
                        dd("id_mov") = dd("id_mov_padre") And dd("id_mov_det") = dd("id_mov_det_padre") Then

                checkChiavePadreFiglio = True

            End If

            'vanni, 26/06/2017: questo impedisce che per un qualsiasi motivo si vadano a scrivere chiavi duplicate.
            '    Dim xTrovato As Integer = (
            '        From t In elem.<trasformazioni>.<trasformazione_dati>.<datiGias>
            '        Where t.<piva>.Value = dd("piva").ToString _
            'And t.<sa_cod>.Value = dd("sa_cod").ToString _
            'And t.<id_agenda>.Value = dd("id_agenda").ToString _
            'And t.<id_mov>.Value = dd("id_mov").ToString _
            'And t.<id_mov_det>.Value = dd("id_mov_det").ToString _
            'And t.<cal_Cod>.Value = wCalCod.ToString _
            'And t.<cal_Cod_Padre>.Value = cal_Cod.ToString
            '        Select CStr(t.<id_mov_det>.Value)
            '    ).ToList.Count

            cal_Cod = GetCalCodPadreCurrentRecord(dd, lav_cod, checkChiavePadreFiglio, FromOutToIn, objParametri)
            wCalCod = GetCalCodCurrentRecord(dd, lav_cod, FromOutToIn, objParametri)

            Dim xTrovato As Integer = (
                From t In elem.<trasformazioni>.<trasformazione_dati>.<datiGias>
                Where t.<piva>.Value = dd("piva").ToString _
                    And t.<sa_cod>.Value = dd("sa_cod").ToString _
                    And t.<id_agenda>.Value = dd("id_agenda").ToString _
                    And t.<id_mov>.Value = dd("id_mov").ToString _
                    And t.<id_mov_det>.Value = dd("id_mov_det").ToString _
                    And t.<cal_Cod>.Value = wCalCod.ToString _
                    And t.<cal_Cod_Padre>.Value = cal_Cod.ToString
                Select CStr(t.<id_mov_det>.Value)
            ).ToList.Count

            'lavezzo - 13/04/2021 - è una pezza a come vengono scritti i movimenti di raccolta a fronte di un conferimento
            If cal_Cod = dd("id_mov_det") Then
                xTrovato += 1
            End If

            'Stefano - 18/9/2017:   esistono diversi casi in cui più lotti padre, ad esempio di calibrato
            '                       nascono da uno stesso figlio; fino ad oggi esistevano casi in cui venivano scartate
            '                       per errore delle righe, ora con le due righe aggiunte sopra
            '   And t.<cal_Cod>.Value = wCalCod.ToString _
            '   And t.<cal_Cod_Padre>.Value = cal_Cod.ToString
            '                       questo non avviene più.
            '                       Ma a questo punto il problema è che il treeList di Kendo non riesce
            '                       a renderizzare (in effetti non è un treelist ma un grafo)
            '                       Questo sotto era un tentativo di ovviare al problema introducendo più volte la 
            '                       riga figlia nell'elenco xml, rinumerando il cal_cod figlio e quello del primo
            '                       padre trovato.
            '                       Ma rimanevano casi non coperti e in più a questo punto le quantità venivano
            '                       duplicate quindi sarebbe stata necessaria una proporzione.
            '                       Quindi in data di oggi il treeList viene momentaneamente abbandonato
            'If xTrovato > 0 Then
            '    
            '    Dim xOccorrenzePadre = (
            '    From t In elem.<trasformazioni>.<trasformazione_dati>.<datiGias>
            '    Where t.<piva>.Value = dd("piva").ToString _
            '         And t.<cal_Cod>.Value = cal_Cod.ToString
            '    Select CStr(t.<cal_Cod>.Value)
            ').ToList.Count


            '    If xOccorrenzePadre > 1 Then
            '        Dim xCalCodPadre = (
            '    From t In elem.<trasformazioni>.<trasformazione_dati>.<datiGias>
            '    Where t.<piva>.Value = dd("piva").ToString _
            '            And t.<cal_Cod>.Value = cal_Cod.ToString
            '    Select t
            '        ).FirstOrDefault

            '        Dim xCalCodPadre2 =
            '            (From t1 In xCalCodPadre.<cal_Cod> Select t1).FirstOrDefault

            '        newCalCodPadre = CInt(xCalCodPadre2.Value) + -1000000000

            '        For i1 As Integer = newCalCodPadre To -2000000000 Step -1000000
            '            newCalCodPadre = i1
            '            xCalCodPadre2.Value = newCalCodPadre
            '            Dim xTrovato2 As Integer = (
            '             From t In elem.<trasformazioni>.<trasformazione_dati>.<datiGias>
            '             Where t.<piva>.Value = dd("piva").ToString _
            '                And t.<sa_cod>.Value = dd("sa_cod").ToString _
            '                And t.<id_agenda>.Value = dd("id_agenda").ToString _
            '                And t.<id_mov>.Value = dd("id_mov").ToString _
            '                And t.<id_mov_det>.Value = dd("id_mov_det").ToString _
            '                And t.<cal_Cod>.Value = wCalCod.ToString _
            '                And t.<cal_Cod_Padre>.Value = newCalCodPadre.ToString
            '             Select CStr(t.<id_mov_det>.Value)
            '             ).ToList.Count
            '            If xTrovato2 = 0 Then
            '                xTrovato = 0
            '                Exit For
            '            End If
            '        Next

            '    End If
            'End If

            If xTrovato = 0 Then

                ' Nel caso in cui il movimento sia di lavorazione e lotto = lotto padre significa che è quello che sto
                ' tracciando, quindi il padre è lui stesso
                Dim WLottoPadre As String = lot
                'Questo sotto serviva quando si usava il TreeView
                'If cLotto = dd("Lotto") Then
                '    WLottoPadre = ""
                'End If

                Dim WLotto As String = dd("Lotto")


                Dim flgVis = True
                If FromOutToIn Then
                    flgVis = rTrackCFG.CheckVisibilitaXTipoLotto_LavCod_CauMovOut_CauMovIn_PreparazioneCod(cTipo, dd("lav_Cod"), dd("Cau_Mov_Out"), dd("Cau_Mov_In"), dd("codice_generazione"), "", "", objParametri)
                Else
                    flgVis = rTrackCFG.CheckVisibilitaXTipoLotto_LavCod_CauMovOut_CauMovIn_PreparazioneCod(cTipo, dd("lav_Cod"), dd("Cau_Mov_in"), dd("Cau_Mov_out"), dd("codice_generazione"), "", "", objParametri)
                End If
                Dim flgWeb = rTipoLotto.CheckTipoLottoXFlusso(cTipo, TipiEnumerativi.FF_Track_Flusso.WebApi, "", "", objParametri)

                If flgWeb = True Or DiagramFormat = False Then
                    If CStr(dd("lav_Cod")) <> CStr(LAVCOD_TRASFORMAZIONI) Then
                        WLotto = dd("Des_lib")
                    End If
                End If

                'se sono via webapi devo aggiungere il nodo xml solo se tra quelli da esporre
                'nel caso in cui sia nelle altre condizioni (Interfaccia), devo riportare sempre tutti i movimenti ma con il flag visibile si\no per poi rimappare i collegamenti tra i nodi del grafo
                If (flgWeb = True And flgVis = True) Or flgWeb = False Then
                    'nel caso sia visibile salvo l'id movimento dettaglio nella cache di estrazione dati e lo visualizzo nell'xml
                    If (From s In _stackToTrack Where s.Id_agenda = lId_agenda And s.Id_Mov_Det = lId_mov_det And s.Lotto = lLotto).Count = 0 AndAlso lLotto.ToLower <> "indefinito" AndAlso lLotto <> "" Then
                        _stackToTrack.Add(New stackToTrack With {.Id_agenda = lId_agenda, .Id_Mov_Det = lId_mov_det, .Lotto = lLotto, .Piva = lpiva, .Gestito = 1})
                    End If



                    Dim dtDocCollegati = trk.LeggiDatiDocContabiliCollegati(dd("piva"), dd("id_agenda"), 0, "", "", objParametri)

                    Dim numDoc As String = ""
                    Dim datDoc As Date = New DateTime(1900, 1, 1)
                    Dim IntDocRagSoc As String = ""
                    Dim numDocForn As String = ""
                    Dim datDocForn As Date = New DateTime(1900, 1, 1)
                    Dim wElem_Cod As Integer = 0
                    Dim wPro_cod As Integer = 0
                    Dim wMat_Cod As Integer = 0

                    If dtDocCollegati.Rows.Count > 0 Then
                        numDoc = dtDocCollegati.Rows(0)("Numero_Documento")
                        datDoc = dtDocCollegati.Rows(0)("Data_Documento")
                        IntDocRagSoc = dtDocCollegati.Rows(0)("Rag_Soc")
                        numDocForn = dtDocCollegati.Rows(0)("Numero_Documento_Fornitore")
                        datDocForn = dtDocCollegati.Rows(0)("Data_Documento_Fornitore")
                    End If


                    If checkChiavePadreFiglio = False Then
                        Dim dtMovDettagliPadre = trk.LeggiMovimentiDettagliXMovimentoPadre(dd("piva_padre"), dd("sa_cod_padre"), dd("id_agenda_padre"), dd("id_mov_padre"), dd("id_mov_det_padre"), "", "", objParametri)
                        If (dtMovDettagliPadre IsNot Nothing) AndAlso dtMovDettagliPadre.Rows.Count > 0 Then
                            wElem_Cod = dtMovDettagliPadre.Rows(0)("Elem_Cod")
                            wPro_cod = dtMovDettagliPadre.Rows(0)("Pro_cod")
                            wMat_Cod = dtMovDettagliPadre.Rows(0)("mat_Cod")
                        End If
                    End If

                    '<Elem_Cod_padre><%= wElem_Cod %></Elem_Cod_padre>
                    '<Pro_Cod_padre><%= wPro_cod %></Pro_Cod_padre>
                    '<mat_cod_padre><%= wMat_Cod %></mat_cod_padre>


                    'lavez - 21/01/2022 - PORCATA DA ELIMINARE!!! - Inserito switch per mat_cod da 3 a 2 in caso di confezionamento x POC Antares

                    Dim nn = <trasformazione_dati>
                                 <datiGias>
                                     <piva><%= dd("piva") %></piva>
                                     <sa_cod><%= dd("sa_cod") %></sa_cod>
                                     <id_agenda><%= dd("id_agenda") %></id_agenda>
                                     <lav_cod><%= dd("lav_Cod") %></lav_cod>
                                     <Des_lib><%= dd("Des_lib") %></Des_lib>
                                     <Data_Movimento><%= dd("Data_Movimento") %></Data_Movimento>
                                     <id_mov><%= dd("id_mov") %></id_mov>
                                     <id_mov_det><%= dd("id_mov_det") %></id_mov_det>
                                     <Elem_Cod><%= dd("Elem_Cod") %></Elem_Cod>
                                     <Pro_Cod><%= dd("Pro_cod") %></Pro_Cod>
                                     <mat_cod><%= IIf((dd("lav_Cod") = "5000" And dd("codice_generazione") = -137 And SwitchMatCodXPOC = True), "2", dd("mat_Cod")) %></mat_cod>
                                     <udm_cod><%= dd("udm_cod") %></udm_cod>
                                     <udm_sim><%= dd("UDM_SIM") %></udm_sim>
                                     <cal_Cod><%= GetCalCodCurrentRecord(dd, lav_cod, FromOutToIn, objParametri) %></cal_Cod>
                                     <Lotto><%= WLotto %></Lotto>
                                     <percentuale_contributo><%= lPercentuale_contributo %></percentuale_contributo>
                                     <ordine><%= ordine %></ordine>
                                     <progressivo><%= progressivo %></progressivo>
                                     <Lotto_Padre><%= WLottoPadre %></Lotto_Padre>
                                     <cal_Cod_Padre><%= GetCalCodPadreCurrentRecord(dd, lav_cod, checkChiavePadreFiglio, FromOutToIn, objParametri) %></cal_Cod_Padre>
                                     <piva_Padre><%= IIf(checkChiavePadreFiglio = False, dd("piva_padre"), "") %></piva_Padre>
                                     <sa_Cod_Padre><%= IIf(checkChiavePadreFiglio = False, dd("sa_cod_padre"), 0) %></sa_Cod_Padre>
                                     <id_agenda_padre><%= IIf(checkChiavePadreFiglio = False, dd("id_agenda_padre"), 0) %></id_agenda_padre>
                                     <id_mov_padre><%= IIf(checkChiavePadreFiglio = False, dd("id_mov_padre"), 0) %></id_mov_padre>
                                     <id_mov_det_padre><%= IIf(checkChiavePadreFiglio = False, dd("id_mov_det_padre"), 0) %></id_mov_det_padre>
                                     <Qta_Extra_Totale><%= dd("Qta_Extra_Totale") %></Qta_Extra_Totale>
                                     <codice_generazione><%= dd("codice_generazione") %></codice_generazione>
                                     <prodotto><%= GetProdottoDes(dd("Tabella"), dd("Tabella_Des"), dd("Tabella_Cod"), IIf(dd("Pro_Cod") = 0, dd("Mat_Cod"), dd("Pro_Cod")), objParametri) %></prodotto>
                                     <numero_documento><%= numDoc %></numero_documento>
                                     <data_documento><%= datDoc %></data_documento>
                                     <intestatario_documento_ragione_sociale><%= IntDocRagSoc %></intestatario_documento_ragione_sociale>
                                     <numero_documento_fornitore><%= numDocForn %></numero_documento_fornitore>
                                     <data_documento_fornitore><%= datDocForn %></data_documento_fornitore>
                                     <visibile><%= flgVis %></visibile>
                                 </datiGias>
                             </trasformazione_dati>

                    'Lavezzo - 24/03/2021 - nel caso di AggiungiDettaglioAziendaAppezza a true, aggiungo la valorizzazione di un ulteriore elemento
                    '                       che contiene i dati relativi al centro aziendale e agli appezzamenti interessati con rapporto 1 a N
                    If AggiungiDettaglioAziendaAppezza = True Then
                        ' il tutto deve partire solo se ha riferimento nella tabella movimenti_destinazioni con tipo_Destinazione=0 per le operazioni culturali
                        Dim gg = AddDetailCentroAziendale(dd("piva"), dd("sa_cod"), dd("id_agenda"), dd("id_mov"), dd("id_mov_det"), dd("lav_Cod"), dd("Data_Movimento"), EsponiDatiGIS, objParametri, AggiungiDettaglioVeicolo)
                        If gg IsNot Nothing Then
                            nn.<datiGias>.FirstOrDefault.Add(gg)
                        End If
                    End If

                    elem.<trasformazioni>.FirstOrDefault.Add(nn)
                End If
                If dd("percentuale_contributo") > 1 Then
                    lPercentuale_contributo = percentuale_contributo_precedente
                End If

                If {201, 210, 204, 205, 200}.Contains(dd("Elem_Cod")) AndAlso dd("Lotto") <> "" AndAlso dd("Lotto").ToString.ToLower <> "indefinito" Then
                    EnqueueLot(dd("Lotto"), dd("mat_cod"), dd("cal_Cod"), lPercentuale_contributo)
                End If

            End If
        Next
    End Sub

    Private Function GetProdottoDes(ByVal Tabella As String, ByVal TabellaDes As String, ByVal TabellaCod As String, ByVal ProCod As String, ByVal ObjParametri As AgronicaCoreParametri) As String
        Dim desc As String = ""

        Dim rMetaSchema = New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R
        Dim dtDescItem = rMetaSchema.LeggiTabella_da_CategorieMagazzino(Tabella, TabellaCod, TabellaDes, "", ProCod, "", "", ObjParametri)
        If dtDescItem.Rows.Count > 0 Then
            desc = dtDescItem(0)(TabellaDes)
        End If
        Return desc
    End Function

    Private Function GetCalCodCurrentRecord(ByVal Row As DataRow, lav_cod As Integer, ByVal FromOutToIn As Boolean, objParametri As AgronicaCoreParametri) As Integer
        Dim ret As Integer = -1
        If FromOutToIn Then
            ret = Row("Cal_Cod")
            Select Case lav_cod
                Case LAVCOD_ACCETTAZIONE_DIVERSI
                    'se il record è di accettazione ed è collegato ad una raccolta devo rimappare i cal_cod in modo che
                    'il Cal_Cod_Padre sia il Cal_Cod del movimento_dettaglio corrente e su Cal_Cod inserisco il riferimento al id_mov_det dell record di raccolta collegato

                    ret = GetIdMovDetRaccolta(Row("piva"), Row("sa_cod"), Row("id_agenda"), Row("id_mov"), Row("id_mov_det"), lav_cod, Row("Cau_Mov_In"), objParametri)
                Case LAVCOD_FATTURA_EMESSA
                    ret = 0
                Case Else
                    'non fare nulla
            End Select
        Else
            ret = Row("Cal_Cod_Padre")
            Select Case lav_cod
                Case LAVCOD_ACCETTAZIONE_DIVERSI
                    'se il record è di accettazione ed è collegato ad una raccolta devo rimappare i cal_cod in modo che
                    'il Cal_Cod_Padre sia il Cal_Cod del movimento_dettaglio corrente e su Cal_Cod inserisco il riferimento al id_mov_det dell record di raccolta collegato

                    ret = GetIdMovDetRaccolta(Row("piva"), Row("sa_cod"), Row("id_agenda"), Row("id_mov"), Row("id_mov_det"), lav_cod, Row("Cau_Mov_In"), objParametri)
                Case LAVCOD_RACCOLTA
                    'Specularmente al movimento di accettazione devo sostituire il Cal_Cod originale con l'id_mov_det di quel movimento_dettaglio
                    'in questo modo la catena di collegamento dei Cal_Cod basati sul magazzino non viene cambiata
                    ret = Row("Cal_Cod")
                Case LAVCOD_FATTURA_EMESSA
                    ret = 0
                Case Else
                    'non fare nulla
            End Select
        End If

        Return ret
    End Function

    Private Function GetCalCodPadreCurrentRecord(ByVal Row As DataRow, lav_cod As Integer, checkChiavePadreFiglio As Boolean, ByVal FromOutToIn As Boolean, objParametri As AgronicaCoreParametri) As Integer
        Dim ret As Integer = -1
        If FromOutToIn Then
            ret = Row("Cal_Cod_Padre")
            Select Case lav_cod
                Case LAVCOD_ACCETTAZIONE_DIVERSI
                    'se il record è di accettazione ed è collegato ad una raccolta devo rimappare i cal_cod in modo che
                    'il Cal_Cod_Padre sia il Cal_Cod del movimento_dettaglio corrente e su Cal_Cod inserisco il riferimento al id_mov_det dell record di raccolta collegato
                    ret = Row("Cal_Cod")
                Case LAVCOD_RACCOLTA
                    'Specularmente al movimento di accettazione devo sostituire il Cal_Cod originale con l'id_mov_det di quel movimento_dettaglio
                    'in questo modo la catena di collegamento dei Cal_Cod basati sul magazzino non viene cambiata
                    ret = Row("id_mov_det_padre")
                Case LAVCOD_FATTURA_EMESSA
                    'test


                Case Else
                    'nel caso in cui abbia un cal_cod=cal_cod_padre e non sono 
                    If checkChiavePadreFiglio = True Then
                        ret = 0
                    End If
                    'non fare nulla
            End Select
        Else
            ret = Row("Cal_Cod")
            Select Case lav_cod
                Case LAVCOD_ACCETTAZIONE_DIVERSI
                    'non fa nulla

                Case LAVCOD_RACCOLTA
                    'Specularmente al movimento di accettazione devo sostituire il Cal_Cod originale con l'id_mov_det di quel movimento_dettaglio
                    'in questo modo la catena di collegamento dei Cal_Cod basati sul magazzino non viene cambiata
                    ret = Row("id_mov_det_padre")

                Case LAVCOD_FATTURA_EMESSA
                    'test non fa nulla

                Case Else
                    'nel caso in cui abbia un cal_cod=cal_cod_padre e non sono 
                    If checkChiavePadreFiglio = True Then
                        ret = 0
                    End If
                    'non fare nulla
            End Select
        End If
        Return ret
    End Function

    Private Function GetIdMovDetRaccolta(ByVal piva As String, ByVal sa_cod As Integer, ByVal id_agenda As Integer, ByVal id_mov As Integer, ByVal id_mov_det As Integer, ByVal lav_cod As Integer, ByVal cau_mov As String, objParametri As AgronicaCoreParametri) As Integer
        Dim ret As Integer = -1
        Dim rMovDetRif = New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
        Dim rMovDet = New AgronicaCoreContabDAL.Movimenti_Dettagli_R

        'verifico se ho il collegamento tra le righe di agenda
        Dim dtRec = rMovDetRif.Leggi(piva, sa_cod, id_agenda, id_mov, id_mov_det, lav_cod, cau_mov, "", "", objParametri)
        If dtRec.Rows.Count > 0 Then
            If dtRec(0)("Id_Mov_Det_Rif") <> -1 Then
                ret = dtRec(0)("Id_Mov_Det_Rif")
            Else
                Dim dtDet = rMovDet.Leggi(dtRec(0)("Piva_Rif"), dtRec(0)("Sa_Cod_Rif"), dtRec(0)("Id_Agenda_Rif"), IIf(dtRec(0)("Id_Mov_Rif") <> -1, dtRec(0)("Id_Mov_Rif"), 0), 0, 0, 0, 0, "", 0, 0, 0, 0, 0, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)
                If dtDet.Rows.Count > 0 Then
                    ret = dtDet(0)("Id_Mov_Det")
                End If
            End If
        End If
        Return ret
    End Function

    Public Function AddDetailCentroAziendale(piva As String, sa_cod As Integer, id_agenda As Integer, id_mov As Integer, id_mov_det As Integer, lav_cod As String, data_movimento As Date, EsponiDatiGIS As Boolean, objParametri As AgronicaCoreParametri, ByVal AggiungiDettaglioVeicolo As Boolean) As XElement
        Dim ret As XElement
        ' il tutto deve partire solo se ha riferimento nella tabella movimenti_destinazioni con tipo_Destinazione=0 per le operazioni culturali
        Dim rMovDest = New AgronicaCoreContabDAL.Mov_Destinazioni_R
        Dim rApp = New AgronicaCoreAnagrafeDAL.Appezzamento_Read
        Dim rInd = New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        Dim rGIS = New AgronicaCoreGisDAL.GIS_Entita_R
        Dim rImpProg = New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
        Dim rRegImp = New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim rMovimenti = New AgronicaCoreContabDAL.Movimenti_R
        Dim rOperazioni = New AgronicaCoreAnagrafeDAL.Operazioni_R
        Dim rMovMacchine = New AgronicaCoreContabDAL.Movimenti_R
        Dim rFF_ImportDatiMacchineOperazioniAgenda = New AgronicaCoreContabDAL.FF_ImportDatiMacchinaOperazioniAgenda_R


        'letture iniziali da fare una sola volta su tutto il gruppo di dati del movimento
        Dim dtMovDEst = rMovDest.Leggi_Raccolte(piva, sa_cod, id_agenda, id_mov, id_mov_det, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta, "", "", objParametri)
        Dim dtInd = rInd.Leggi(piva, sa_cod, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, "Tipo_indirizzo=1", "", objParametri)

        If dtMovDEst.Rows.Count() > 0 Then
            Dim centroaziendale = New XElement("centroaziendale")

            If dtInd.Rows.Count() > 0 Then
                centroaziendale.Add(New XElement("indirizzo", dtInd(0)("ind_des")))
                centroaziendale.Add(New XElement("comune", dtInd(0)("com_des")))
                centroaziendale.Add(New XElement("codice_istat_comune", dtInd(0)("com_cod_istat")))
                centroaziendale.Add(New XElement("frazione", dtInd(0)("frz_des")))
                centroaziendale.Add(New XElement("provincia", dtInd(0)("pro_cod")))
                centroaziendale.Add(New XElement("codice_istat_provincia", dtInd(0)("pro_cod_istat")))
                centroaziendale.Add(New XElement("cap", dtInd(0)("CAP")))
                centroaziendale.Add(New XElement("stato", dtInd(0)("stato")))
            End If

            Dim appezzamenti = New XElement("appezzamenti")
            For Each row In dtMovDEst.Rows
                Dim dtApp = rApp.Leggi_x_anagrafica(row("piva"), row("sa_cod"), 0, row("appezza"), "", "", objParametri)



                Dim appezzamento = New XElement("appezzamento")
                appezzamento.Add(New XElement("appezzamento_denominazione", dtApp(0)("APP_NOME")))

                'lavez - 096/06/2021 - espongo i dati GIS solo se nil flag specifico è true (preso dalla parametrizzazione del tipo lotto
                If EsponiDatiGIS = True Then
                    Dim dtGIS = rGIS.LeggiSmallAppezza(row("piva"), row("sa_cod"), row("appezza"), row("id_Destinazione"), "", "", objParametri)
                    If dtGIS.Rows.Count <= 0 Then
                        appezzamento.Add(New XElement("gis", ""))
                    Else
                        appezzamento.Add(New XElement("gis", dtGIS(0)("Oggetto")))
                    End If
                End If

                Dim dtImpProg = rImpProg.LeggiMinimal(row("piva"), row("sa_cod"), row("appezza"), row("id_Destinazione"), 0, data_movimento, data_movimento, "", "", objParametri)
                If dtImpProg.Rows.Count > 0 Then
                    Dim esercizi = New XElement("esercizi")

                    For Each rowd In dtImpProg.Rows
                        Dim dtSpecieVeg = rRegImp.Leggi_SpecieVarieta(row("piva"), row("sa_cod"), row("appezza"), row("id_Destinazione"), 0, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)

                        Dim esercizio = New XElement("esercizio")
                        esercizio.Add(New XElement("lotto", rowd("Progetto_Nome")))
                        For Each specie In dtSpecieVeg.Rows
                            esercizio.Add(New XElement("specie_vegetale", specie("Veg_Des")))
                        Next
                        Dim dtOpeImp = rRegImp.Leggi_Operazioni_Impianti_Dettaglio(row("piva"), row("sa_cod"), row("appezza"), row("id_Destinazione"), rowd("Validita_Inizio"), IIf(rowd("Validita_Fine") < data_movimento, rowd("Validita_Fine"), data_movimento), "(Agenda.id_agenda<>" + id_agenda.ToString() + " and Agenda.Lav_Cod<>'" + lav_cod + "')", "Movimenti.Data_Movimento Desc", objParametri)
                        If dtOpeImp.Rows.Count() > 0 Then
                            Dim operazioni = New XElement("operazioni")
                            For Each ope In dtOpeImp.Rows
                                Dim operazione = New XElement("operazione")
                                operazione.Add(New XElement("piva", ope("Piva")))
                                operazione.Add(New XElement("sa_cod", ope("Sa_Cod")))
                                operazione.Add(New XElement("id_agenda", ope("Id_Agenda")))
                                operazione.Add(New XElement("lav_cod", ope("lav_cod")))
                                operazione.Add(New XElement("lav_des", rOperazioni.Lav_Des_From_Lav_Cod(ope("lav_cod"), objParametri)))
                                Dim dsc = GetProdottoDes(ope("Tabella"), ope("Tabella_Des"), ope("Tabella_Cod"), ope("Pro_Cod"), objParametri)
                                If dsc <> "" Then
                                    operazione.Add(New XElement("descrizione_operazione", dsc))
                                Else
                                    operazione.Add(New XElement("descrizione_operazione", ope("des_lib")))
                                End If
                                operazione.Add(New XElement("Data_Movimento", ope("Data_Movimento")))
                                operazione.Add(New XElement("id_mov", ope("id_mov")))
                                operazione.Add(New XElement("id_mov_det", ope("id_mov_det")))
                                operazione.Add(New XElement("Elem_Cod", ope("Elem_Cod")))
                                operazione.Add(New XElement("Pro_Cod", ope("Pro_Cod")))
                                operazione.Add(New XElement("mat_cod", ope("mat_cod")))
                                operazione.Add(New XElement("udm_cod", ope("udm_cod")))
                                operazione.Add(New XElement("udm_sim", ope("UDM_SIM")))
                                operazione.Add(New XElement("cal_Cod", ope("Cal_Cod")))
                                operazione.Add(New XElement("lotto", ope("Lotto")))
                                operazione.Add(New XElement("percentuale_contributo", 0))
                                operazione.Add(New XElement("ordine", 0))
                                operazione.Add(New XElement("progressivo", 0))
                                operazione.Add(New XElement("Lotto_Padre", ""))
                                operazione.Add(New XElement("cal_Cod_Padre", 0))
                                operazione.Add(New XElement("Qta_Extra_Totale", ope("Qta")))
                                operazione.Add(New XElement("codice_generazione", 0))
                                operazione.Add(New XElement("prodotto", GetProdottoDes(ope("Tabella"), ope("Tabella_Des"), ope("Tabella_Cod"), IIf(ope("Pro_Cod") = 0, ope("Mat_Cod"), ope("Pro_Cod")), objParametri)))

                                'lavezzo - 13/12/2021 - soluzione temporanea per estrazione dati macchine x POC Antares
                                If AggiungiDettaglioVeicolo Then
                                    '1- verifica se ho un record di TaskData associato per piva\esercizio\cod_operazione\id_agenda
                                    'se non trovato -> prova associazione
                                    'se trovato -> recupera TaskData

                                    Dim dtCheckTaskData = rFF_ImportDatiMacchineOperazioniAgenda.GetMovimentoAgendaAssociatoConTaskDataSDF(ope("Piva"),
                                                                                                                                        rowd("Progetto_Cod"),
                                                                                                                                        CInt(ope("lav_cod")),
                                                                                                                                        ope("Id_Agenda"),
                                                                                                                                        objParametri)
                                    Dim bLink As Boolean = False
                                    If dtCheckTaskData.Rows.Count <= 0 Then
                                        Dim dtMov = rFF_ImportDatiMacchineOperazioniAgenda.GetTaskDataAssociabiliConMovimentiAgenda(ope("Piva"),
                                                                                                                                        rowd("Progetto_Cod"),
                                                                                                                                        CInt(ope("lav_cod")),
                                                                                                                                        objParametri)

                                        If dtMov.Rows.Count > 0 Then
                                            dtMov.DefaultView.Sort = "taskDataEndDateTime Desc"
                                        End If



                                        For Each drow In dtMov.Rows
                                            If drow("taskDataEndDateTime") >= ope("Data_Movimento") Then
                                                AgronicaCoreContabBIZ.FF_ImportDatiMacchineOperazioniAgenda.LinkTaskDataWithAgendaMov(drow("id"),
                                                                                                                                      ope("Id_Agenda"),
                                                                                                                                      objParametri)
                                                bLink = True
                                                Exit For
                                            End If
                                        Next
                                        dtCheckTaskData = rFF_ImportDatiMacchineOperazioniAgenda.GetMovimentoAgendaAssociatoConTaskDataSDF(ope("Piva"),
                                                                                                                                        rowd("Progetto_Cod"),
                                                                                                                                        CInt(ope("lav_cod")),
                                                                                                                                        ope("Id_Agenda"),
                                                                                                                                        objParametri)
                                    End If

                                    If dtCheckTaskData.Rows.Count > 0 Then
                                        Dim dettaglioVeicolo = New XElement("vehicle")
                                        dettaglioVeicolo.Add(New XElement("vehicleBrand", dtCheckTaskData.Rows(0)("vehicleBrand")))
                                        dettaglioVeicolo.Add(New XElement("vehicleModel", dtCheckTaskData.Rows(0)("vehicleModel")))
                                        dettaglioVeicolo.Add(New XElement("co2", dtCheckTaskData.Rows(0)("cO2")))
                                        dettaglioVeicolo.Add(New XElement("co2_udm_cod", "2"))
                                        dettaglioVeicolo.Add(New XElement("co2_udm_sim", "KG"))
                                        dettaglioVeicolo.Add(New XElement("fuelConsumption", dtCheckTaskData.Rows(0)("fuelConsumption")))
                                        dettaglioVeicolo.Add(New XElement("fuelConsumption_udm_cod", "29"))
                                        dettaglioVeicolo.Add(New XElement("fuelConsumption_udm_sim", "L"))
                                        operazione.Add(dettaglioVeicolo)
                                    End If
                                End If

                                Dim dtCarichi = rMovimenti.LeggiMovimentixTracciabilità(row("piva"), row("sa_cod"), {LAVCOD_ACQUISTO, LAVCOD_CARICO, LAVCOD_BOLLA_RICEVUTA}, ope("Elem_Cod"), ope("Pro_Cod"), ope("Mat_Cod"), CAU_CARICO, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri)
                                If dtCarichi.Rows.Count > 0 Then
                                    Dim carichi = New XElement("ingressi_di_magazzino")
                                    For Each car In dtCarichi.Rows
                                        Dim carico = New XElement("ingresso")
                                        carico.Add(New XElement("data_Carico", car("Data_Movimento")))
                                        carico.Add(New XElement("descrizione_carico", car("des_lib")))
                                        carico.Add(New XElement("qta", Math.Round(car("qta"), 4)))
                                        carico.Add(New XElement("um", car("UDM_SIM")))
                                        carichi.Add(carico)
                                    Next
                                    operazione.Add(carichi)
                                End If
                                operazioni.Add(operazione)
                            Next
                            esercizio.Add(operazioni)
                        End If
                        esercizi.Add(esercizio)
                    Next
                    appezzamento.Add(esercizi)
                End If

                appezzamenti.Add(appezzamento)
            Next
            centroaziendale.Add(appezzamenti)

            ret = centroaziendale
        Else
            ret = Nothing
        End If
        Return ret
    End Function

    Public Function GetDocFilterDataTable(ByVal lotto As String,
            ByVal docNumeroSin As String, ByVal docNumero As Integer,
            ByVal docNumeroDes As String, ByVal nrRiga As String, ByVal fromOutToIn As Boolean, ByVal certificazione As Integer, ByVal piva As String, ByVal objParametri_Server As AgronicaCoreParametri) As DataTable

        Dim dt = New DataTable
        Try
            Dim xSelectAggiuntiva As String = ""
            Dim xFiltroAggiuntivo As String = ""
            Dim xOrderBy As String = ""
            Dim FlagJollyInt As Short = 0
            Dim LavCodArray As Integer() = Nothing
            Dim Cau_MovArray As String() = Nothing
            Dim Cau_Agg As Integer = 0
            If fromOutToIn Then
                ReDim LavCodArray(2)
                LavCodArray(0) = (LAVCOD_BOLLA_EMESSA)
                LavCodArray(1) = (LAVCOD_FATTURA_EMESSA)
                LavCodArray(2) = (LAVCOD_DDT_CONTABILIZZATO_EMESSO)
                xSelectAggiuntiva += ", MovimentiCausaleAgg.Doc_Numero_Sin As MCauAgg_Doc_Numero_Sin , MovimentiCausaleAgg.Doc_Numero As MCauAgg_Doc_Numero, MovimentiCausaleAgg.Doc_Numero_Des As  MCauAgg_Doc_Numero_Des "
                Cau_Agg = CAU_REGISTRAZIONI
                If docNumeroSin <> "" Then
                    xFiltroAggiuntivo += " AND MovimentiCausaleAgg.Doc_Numero_Sin = '" & docNumeroSin & "'   "
                End If
                If docNumero <> 0 Then
                    xFiltroAggiuntivo += " AND MovimentiCausaleAgg.Doc_Numero = " & docNumero & "   "
                End If
                If docNumeroDes <> "" Then
                    xFiltroAggiuntivo += " AND MovimentiCausaleAgg.Doc_Numero_Des = '" & docNumeroDes & "'   "
                End If
                If nrRiga <> "" Then
                    xFiltroAggiuntivo += " And Movimenti_Dettagli.Extra_Str = '" & nrRiga & "' "
                End If
                If certificazione <> 0 Then
                    xFiltroAggiuntivo += " And OTabelle_Parametri_certificazioni.Tabella_Par_Cod = " & certificazione.ToString & " "
                End If
                xOrderBy = " ORDER BY Agenda.PIVA, Movimenti.Data_Movimento DESC, MovimentiCausaleAgg.Doc_Numero_Sin, MovimentiCausaleAgg.Doc_Numero, MovimentiCausaleAgg.Doc_Numero_Des, Movimenti_Dettagli.Extra_Str"
            Else
                ReDim Cau_MovArray(0)
                Cau_MovArray(0) = (CAU_ACCETTAZIONE_BENI_DA_DIVERSI)
                Cau_Agg = CAU_REGISTRAZIONI_TERZIARIA
                xSelectAggiuntiva += ", MovimentiCausaleAgg.Doc_Numero_Sin As MCauAgg_Doc_Numero_Sin , MovimentiCausaleAgg.Doc_Numero As MCauAgg_Doc_Numero, MovimentiCausaleAgg.Doc_Numero_Des As  MCauAgg_Doc_Numero_Des "
                If docNumeroSin <> "" Then
                    xFiltroAggiuntivo += " AND MovimentiCausaleAgg.Doc_Numero_Sin = '" & docNumeroSin & "'   "
                End If
                If docNumero <> 0 Then
                    xFiltroAggiuntivo += " AND MovimentiCausaleAgg.Doc_Numero = " & docNumero & "   "
                End If
                If docNumeroDes <> "" Then
                    xFiltroAggiuntivo += " AND MovimentiCausaleAgg.Doc_Numero_Des = '" & docNumeroDes & "'   "
                End If
                If nrRiga <> "" Then
                    xFiltroAggiuntivo += " And Movimenti_Dettagli.Extra_Str = '" & nrRiga & "' "
                End If
                If certificazione <> 0 Then
                    xFiltroAggiuntivo += " And OTabelle_Parametri_certificazioni.Tabella_Par_Cod = " & certificazione.ToString & " "
                End If
                xOrderBy = " ORDER BY Agenda.PIVA, Movimenti.Data_Movimento DESC, MovimentiCausaleAgg.Doc_Numero_Sin, MovimentiCausaleAgg.Doc_Numero, MovimentiCausaleAgg.Doc_Numero_Des, Movimenti_Dettagli.Extra_Str"
            End If

            Dim leggi As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
            dt = leggi.MovimentiDettagli_Leggi_FF(piva,
                                            0,
                                            "",
                                            0,
                                            0,
                                            0,
                                            210,
                                            0,
                                            0,
                                            0,
                                            0,
                                            lotto,
                                            0,
                                            0,
                                            LavCodArray,
                                            Cau_MovArray,
                                            Cau_Agg,
                                            FlagJollyInt,
                                            xSelectAggiuntiva,
                                            "",
                                            xFiltroAggiuntivo,
                                            xOrderBy,
                                            objParametri_Server
                                            )
        Catch ex As Exception
            dt = Nothing
        End Try
        Return dt

    End Function

#End Region


#Region "Gestione della cache"


    Private Sub LeggiCache(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim LeggiTraformazioni As New AgronicaCoreContabDAL.Trasformazioni_R




    End Sub

    Private Sub ScriviCache(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim leggiProgressivo As New AgronicaCoreDataProvider.Agro_Sequenze


        Dim scriviTrasformazioni As New AgronicaCoreContabDAL.Trasformazioni_W
        Dim scriviTrasformazioni_rif As New AgronicaCoreContabDAL.Trasformazioni_Riferimenti_W


        For Each toCache In _stackToTrack

            Dim id_tr As Integer
            'id_tr = leggiProgressivo.Agronica_SequenzaTabelle_NuovoID("trasformazioni", objParametri)

            'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
            id_tr = leggiProgressivo.NuovoId_Tabella("trasformazioni", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri)

            scriviTrasformazioni.Scrivi(
                toCache.Piva,
                0,
                id_tr,
                toCache.Lotto,
                0,
                0,
                0,
                0,
                toCache.Cau_Mov,
                0,
                AGRODATAINIZIO,
                AGRODATAINIZIO,
                AGRODATAINIZIO,
                AGRODATAINIZIO,
                0,
                0,
                "",
                0,
                0,
                0,
                0,
                0,
                AGRODATAINIZIO,
                AGRODATAFINE,
                objParametri
            )



        Next

    End Sub


#End Region



#Region "Utility xml"

    Public Shared Function ResultToString(ByVal sResult As XDocument) As String

        Return RemoveNamespace(sResult).ToString.Replace("<track>", "<track xmlns=""http://www.agronica.it/track/"">")

    End Function


    Public Shared Function RemoveNamespace(xdoc As XDocument) As XDocument


        For Each e As XElement In xdoc.Root.DescendantsAndSelf()
            If e.Name.[Namespace] <> XNamespace.None Then
                e.Name = XNamespace.None.GetName(e.Name.LocalName)
            End If
            If e.Attributes().Where(Function(a) a.IsNamespaceDeclaration OrElse a.Name.[Namespace] <> XNamespace.None).Any() Then
                e.ReplaceAttributes(e.Attributes().[Select](Function(a) If(a.IsNamespaceDeclaration, Nothing, If(a.Name.[Namespace] <> XNamespace.None, New XAttribute(XNamespace.None.GetName(a.Name.LocalName), a.Value), a))))
            End If
        Next
        Return xdoc
    End Function
#End Region

End Class

Public Class FF_Track_Model_TrackInput
    Public Property TrackCode As String
    Public Property TipologiaCatena As String
    Public Property ListaDocumentiPerFiltro As List(Of String)
    Public Property LinguaDatiISO As String
    Public Property FromOutToIn As Boolean
    Public Property PivaRiferimento As String
End Class

Public Class FF_Track_Model_TrackInput_Estesa
    Inherits FF_Track_Model_TrackInput
    Public Property cCertificazione As Integer
    Public Property CalColSelected As Integer
    Public Property idMovDetSelected As Integer
    Public Property ForzaNuovaEsecuzioneAlgoritmoRintraccia As Boolean

End Class

Public Class FF_TaskDataMachine_Model_Input
    Public Property ID_Task As Integer
    Public Property PivaSuperUser As String
    Public Property Piva As String
    Public Property Cod_Operazione As Integer
    Public Property Cod_Impianto As Integer
    Public Property Id_Agenda As Integer
End Class


Public Class blockToTrack

    Private _Lotto As String
    Private _Mat_cod As Integer
    Private _Cal_Cod As Integer

    Private _percentualeContributo As Decimal
    Public Property PercentualeContributo As Decimal
        Get
            Return _percentualeContributo
        End Get
        Set(ByVal value As Decimal)
            _percentualeContributo = value
        End Set
    End Property


    Public Property Cal_Cod As Integer
        Get
            Return _Cal_Cod
        End Get
        Set(ByVal value As Integer)
            _Cal_Cod = value
        End Set
    End Property
    Public Property Mat_cod As Integer
        Get
            Return _Mat_cod
        End Get
        Set(ByVal value As Integer)
            _Mat_cod = value
        End Set
    End Property
    Public Property Lotto As String
        Get
            Return _Lotto
        End Get
        Set(ByVal value As String)
            _Lotto = value
        End Set
    End Property

End Class


Public Class trackToSave

    Private _id_mov As Integer
    Private _id_mov_det As Integer

    Private _id_agenda As Integer
    Private _sa_cod As Integer
    Private _Lotto As String
    Private _Lotto_Padre As String
    Private _cal_cod As Integer
    Private _cal_cod_padre As Integer
    Private _qta_Extra_Totale As Decimal
    Private _Cau_Mov As String
    Private _Piva As String

    Private _Gestito As Boolean

    Private _Piva_Padre As String
    Private _sa_cod_Padre As Integer
    Private _id_agenda_Padre As Integer
    Private _id_mov_Padre As Integer
    Private _id_mov_det_Padre As Integer

    Private _ordineTipologiaMovimento As Integer
    Private _data_movimento As DateTime

    Private _Num_Doc As String
    Private _Num_Doc_Forn As String
    Private _Int_Doc_Rag_Soc As String
    Private _Dat_Doc As DateTime
    Private _Dat_Doc_Forn As DateTime

    Private _visibile As Boolean

    Public Property NumeroDocumento As String
        Get
            Return _Num_Doc
        End Get
        Set(ByVal value As String)
            _Num_Doc = value
        End Set
    End Property

    Public Property NumeroDocumentoFornitore As String
        Get
            Return _Num_Doc_Forn
        End Get
        Set(ByVal value As String)
            _Num_Doc_Forn = value
        End Set
    End Property

    Public Property IntestatarioDocumentoRagioneSociale As String
        Get
            Return _Int_Doc_Rag_Soc
        End Get
        Set(ByVal value As String)
            _Int_Doc_Rag_Soc = value
        End Set
    End Property

    Public Property DataDocumentoFornitore As DateTime
        Get
            Return _Dat_Doc_Forn
        End Get
        Set(ByVal value As DateTime)
            _Dat_Doc_Forn = value
        End Set
    End Property

    Public Property DataDocumento As DateTime
        Get
            Return _Dat_Doc
        End Get
        Set(ByVal value As DateTime)
            _Dat_Doc = value
        End Set
    End Property

    Public Property DataMovimento As DateTime
        Get
            Return _data_movimento
        End Get
        Set(ByVal value As DateTime)
            _data_movimento = value
        End Set
    End Property

    Public Property Visibile As Boolean
        Get
            Return _visibile
        End Get
        Set(ByVal value As Boolean)
            _visibile = value
        End Set
    End Property

    Public Property Ordine As Integer
        Get
            Return _ordineTipologiaMovimento
        End Get
        Set(ByVal value As Integer)
            _ordineTipologiaMovimento = value
        End Set
    End Property
    Public Property Id_mov_det As Integer
        Get
            Return _id_mov_det
        End Get
        Set(ByVal value As Integer)
            _id_mov_det = value
        End Set
    End Property
    Public Property Id_mov As Integer
        Get
            Return _id_mov
        End Get
        Set(ByVal value As Integer)
            _id_mov = value
        End Set
    End Property
    Public Property Sa_cod As Integer
        Get
            Return _sa_cod
        End Get
        Set(ByVal value As Integer)
            _sa_cod = value
        End Set
    End Property
    Public Property Piva As String
        Get
            Return _Piva
        End Get
        Set(ByVal value As String)
            _Piva = value
        End Set
    End Property

    Public Property Piva_Padre As String
        Get
            Return _Piva_Padre
        End Get
        Set(ByVal value As String)
            _Piva_Padre = value
        End Set
    End Property

    Public Property Sa_cod_Padre As Integer
        Get
            Return _sa_cod_Padre
        End Get
        Set(ByVal value As Integer)
            _sa_cod_Padre = value
        End Set
    End Property

    Public Property id_agenda_padre As Integer
        Get
            Return _id_agenda_Padre
        End Get
        Set(ByVal value As Integer)
            _id_agenda_Padre = value
        End Set
    End Property

    Public Property id_mov_padre As Integer
        Get
            Return _id_mov_Padre
        End Get
        Set(ByVal value As Integer)
            _id_mov_Padre = value
        End Set
    End Property

    Public Property id_mov_det_padre As Integer
        Get
            Return _id_mov_det_Padre
        End Get
        Set(ByVal value As Integer)
            _id_mov_det_Padre = value
        End Set
    End Property
    Public Property Cau_Mov As String
        Get
            Return _Cau_Mov
        End Get
        Set(ByVal value As String)
            _Cau_Mov = value
        End Set
    End Property
    Public Property Lotto As String
        Get
            Return _Lotto
        End Get
        Set(ByVal value As String)
            _Lotto = value
        End Set
    End Property

    Public Property Lotto_Padre As String
        Get
            Return _Lotto_Padre
        End Get
        Set(ByVal value As String)
            _Lotto_Padre = value
        End Set
    End Property

    Public Property Cal_Cod As Integer
        Get
            Return _cal_cod
        End Get
        Set(ByVal value As Integer)
            _cal_cod = value
        End Set
    End Property
    Public Property Cal_Cod_Padre As Integer
        Get
            Return _cal_cod_padre
        End Get
        Set(ByVal value As Integer)
            _cal_cod_padre = value
        End Set
    End Property


    Public Property Qta_Extra_Totale As Decimal
        Get
            Return _qta_Extra_Totale
        End Get
        Set(ByVal value As Decimal)
            _qta_Extra_Totale = value
        End Set
    End Property

    Public Property Gestito As Boolean
        Get
            Return _Gestito
        End Get
        Set(ByVal value As Boolean)
            _Gestito = value
        End Set
    End Property
    Public Property Id_agenda As Integer
        Get
            Return _id_agenda
        End Get
        Set(ByVal value As Integer)
            _id_agenda = value
        End Set
    End Property



End Class
Public Class trackDiagram
    Public EntityList As List(Of TrackDiagramEntity)
    Public EntityLinkList As List(Of TrackDiagramLinkEntity)

    Public Sub New()
        EntityList = New List(Of TrackDiagramEntity)
        EntityLinkList = New List(Of TrackDiagramLinkEntity)
    End Sub
End Class

Public Class TrackDiagramEntity
    Public Property ID As Integer
    Public Property id_mov_det As Integer
    Public Property Name As String

    Public Property Lotto As String

    Public Property TipoDocumento As String
    Public Property IntestatarioDocumento As String
    Public Property NumeroDocumento As String
    Public Property DataDocumento As String

    Public Property StartChainEntity As Boolean
    Public Property QtyDoc As Decimal
    Public Property OrdineLayer As Integer

    Public Property CalCod As Integer
    Public Property CalCodPadre As Integer
    Public Property X As Integer
    Public Property Y As Integer
    'Public Property Link As List(Of TrackDiagramLinkEntity)
    Public Sub New()
        ID = -1
        Name = ""
        CalCod = -1
        CalCodPadre = -1
        Lotto = ""
        StartChainEntity = False
        QtyDoc = 0
        OrdineLayer = 0
        X = 0
        Y = 0
        'Link = New List(Of TrackDiagramLinkEntity)
    End Sub
End Class

Public Class TrackDiagramLinkEntity
    Public Property StartLink As Integer
    Public Property EndLink As Integer

    Public Property Qty As Decimal
    Public Property UM As String
    Public Sub New()
        StartLink = -1
        EndLink = -1
        Qty = 0
        UM = ""
    End Sub
End Class

Public Class stackToTrack

    Private _id_agenda As Integer
    Private _sa_cod As Integer
    Private _Lotto As String
    Private _Cau_Mov As String
    Private _Piva As String
    Private _id_mov_det As Integer

    Private _Gestito As Boolean
    Public Property Sa_cod As Integer
        Get
            Return _sa_cod
        End Get
        Set(ByVal value As Integer)
            _sa_cod = value
        End Set
    End Property
    Public Property Piva As String
        Get
            Return _Piva
        End Get
        Set(ByVal value As String)
            _Piva = value
        End Set
    End Property
    Public Property Cau_Mov As String
        Get
            Return _Cau_Mov
        End Get
        Set(ByVal value As String)
            _Cau_Mov = value
        End Set
    End Property
    Public Property Lotto As String
        Get
            Return _Lotto
        End Get
        Set(ByVal value As String)
            _Lotto = value
        End Set
    End Property

    Public Property Gestito As Boolean
        Get
            Return _Gestito
        End Get
        Set(ByVal value As Boolean)
            _Gestito = value
        End Set
    End Property
    Public Property Id_agenda As Integer
        Get
            Return _id_agenda
        End Get
        Set(ByVal value As Integer)
            _id_agenda = value
        End Set
    End Property

    Public Property Id_Mov_Det As Integer
        Get
            Return _id_mov_det
        End Get
        Set(ByVal value As Integer)
            _id_mov_det = value
        End Set
    End Property

End Class
