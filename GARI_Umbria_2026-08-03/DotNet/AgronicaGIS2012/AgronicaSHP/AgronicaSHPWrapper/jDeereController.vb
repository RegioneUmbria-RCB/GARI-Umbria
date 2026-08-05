
Imports Newtonsoft.Json
Imports System.Net
Imports System.IO
Imports AgronicaCoreVarieBIZ
Imports AgronicaGIS2012.Commons
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports Agronica.Helpers.OAuth2
Imports AgronicaCoreUtility
Imports AgronicaCoreAnagrafeDAL

Public Class jDeereController


#Region "Riporto Common functions"

    Private Function ElencaIDJDNonAncoraImportateDatoTipoEntita(ByVal TipoEntitaJD As enum_TipoEntitaJohnDeere, objParametri_Server As AgronicaCoreParametri) As String

        Dim leggi As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_EntitaGias_R
        Dim dt As DataTable = leggi.ListaEntitaJDNonImportataInGIAS(TipoEntitaJD, "", "", objParametri_Server)

        Dim rval As String
        Dim l1 As List(Of String) = (
            From d In dt.AsEnumerable
            Select CStr(d("ID"))
        ).ToList()

        If l1.Count = 0 Then
            Return ""
        End If

        rval = String.Join(",", l1)

        Return " ID in( " & rval & ")"

    End Function

    Public Function CaricaEntitaJDViaJsonString(Of T)(jsonData As String) As T

        Dim boundaries As T =
            JsonConvert.DeserializeObject(Of T)(jsonData)

        Return boundaries

    End Function


    Private Function ListaChiaviBoundaryDaListaChiaviField(ListaChiaviField As String, objParametri_Server As AgronicaCoreParametri) As String

        Dim leggi As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_EntitaGias_R

        Dim dt As DataTable =
            leggi.ListaChiaviBoundaryDaListaChiaviField(False, ListaChiaviField.Replace("ID", "ff.id"), "", objParametri_Server)

        Dim l As List(Of String) = (From d In dt.AsEnumerable
                                    Select CStr(d("ID"))).ToList

        Return " bb.ID in (" & String.Join(",", l) & ")"


    End Function

#End Region
#Region "Riporto Boundary su GIAS"


    Public Function RiportoBoundaryJDToGIAS(cfgImport As ConfigurazioneImportazione, ByVal ListaChiaviField As String, objParametri_server As AgronicaCoreParametri) As RispostaStandard

        Dim rval As New RispostaStandard
        rval.RispostaOK = True

        Dim ListaChiaviBoundary As String = ""
        If ListaChiaviField <> "" Then
            ListaChiaviBoundary = ListaChiaviBoundaryDaListaChiaviField(ListaChiaviField, objParametri_server)
        End If


        'Riporto dei boundary come dato "importato"
        Dim objImportJD = New jDeere_ToAgronicaGIS2012
        cfgImport.LayerCod = enum_Gis_LayerElementiGrafici_std.INVISIBILE 'dati importati
        objImportJD.convertConListachiavi(cfgImport, ListaChiaviBoundary, objParametri_server)

        Return rval

    End Function

    Public Function AssociaAppezzamentoInGisEntitaDaJoinBoundaryJD(cfgImport As ConfigurazioneImportazione, ByVal ListaChiavi As String, objParametri_server As AgronicaCoreParametri) As RispostaStandard

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim rval As New AgronicaCoreVarieBIZ.RispostaStandard

        'TODO: Gestire la transazione
        Try

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                            FlagTransazioneLocale,
                                                                            objParametri_server)

            'Assegnazione dei boudary dal layer dei dati importati all'appezzamento associato in GIAS
            Dim xRiporto As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_EntitaGIAS_W
            xRiporto.AssociaAppezzamentoInGisEntitaDaJoinBoundaryJD(ListaChiavi, objParametri_server)

            xRiporto.AssociaLayerAppezzamentoInGisDoveMancante(objParametri_server)

            xRiporto.RiportaSuperficieAnagGIASDaGisDoveMancante(enum_GIS2012_TipoEntita.APPEZZAMENTI, objParametri_server)
            xRiporto.RiportaSuperficieAnagGIASDaTabellaPadreDoveMancante(enum_GIS2012_TipoEntita.APPEZZAMENTI, objParametri_server)

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la transazione e la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_server)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            rval.RispostaOK = True
            rval.RispostaStringa = ""

        Catch ex As Exception

            'Faccio il rollback della transazione
            If objParametri_server.objTransazione IsNot Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_server)

            End If

            Dim messaggioErrore As String = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)


            Dim Messaggio As String = ""
            If messaggioErrore <> "" Then

                Messaggio += "ERR: Sono stati rilevati i seguenti errori : " & vbCrLf
                Messaggio += "" & vbCrLf
                Messaggio += messaggioErrore
                Messaggio += "" & vbCrLf
                Messaggio += "Ritentare il salvataggio dopo la correzione ..."

            End If

            rval.RispostaOK = False
            rval.Errore = Messaggio


        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_server)

        End Try

        Return rval

    End Function

    Public Function RiportoBoundaryGIASLeggiOggetti(cfgImport As ConfigurazioneImportazione, ListaChiaviBoundaries As String, objParametri_Server As AgronicaCoreParametri) As rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_Boundary))
        Dim rval As New rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_Boundary))

        Dim xFiltroB As String
        If ListaChiaviBoundaries <> "" Then
            xFiltroB = ListaChiaviBoundaries
        Else
            xFiltroB = ElencaIDJDNonAncoraImportateDatoTipoEntita(enum_TipoEntitaJohnDeere.Boundary, objParametri_Server)
        End If

        If String.IsNullOrEmpty(xFiltroB) Then
            rval.RispostaOK = True
            rval.RispostaStringa = New jDeereDataModel_GenericList(Of jDeereDataModel_Boundary) With {.total = 0}
            Return rval
        End If

        Dim leggi As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Boundary_R
        Dim jSonContents As String = leggi.LeggiViaJsonSQL(xFiltroB, objParametri_Server)

        Dim boundariesVal As List(Of jDeereDataModel_Boundary) = CaricaEntitaJDViaJsonString(Of List(Of jDeereDataModel_Boundary))(jSonContents)
        Dim boundaries As New jDeereDataModel_GenericList(Of jDeereDataModel_Boundary)

        boundaries.values = boundariesVal
        boundaries.total = boundariesVal.Count

        rval.RispostaStringa = boundaries
        rval.RispostaOK = True

        Return rval

    End Function


#End Region

#Region "Riporto Field su GIAS"

    Public Function RiportoFieldJDToGIAS(cfgImport As ConfigurazioneImportazione, ListaChiavi As String, objParametri_server As AgronicaCoreParametri, objParametri_utenti As AgronicaCoreParametri) As RispostaStandard

        Dim messaggioErrore As String = ""

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim rval As New AgronicaCoreVarieBIZ.RispostaStandard

        'TODO: Gestire la transazione
        Try

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                            FlagTransazioneLocale,
                                                                            objParametri_server)


            'per ogni field creo un'appezzamento (con impianto, con distinta, nuovo)
            Dim listaRiporto As rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_Field)) =
            RiportoFieldGIASLeggiOggetti(cfgImport, ListaChiavi, objParametri_server)

            Dim scriviAppezza As New AgronicaCoreAnagrafeBIZ.Appezzamento_W
            Dim scriviImpiantoDistinta As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_W
            Dim ScriviAssociazioneGISJD As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_EntitaGIAS_W
            Dim seqLeggi As New Agro_Sequenze


            Dim baseCode As Integer = 0
            Dim topCode As Integer = 0

            AgronicaCoreDataProvider.UtilityProvider.Calcola_BaseCode_TopCode(baseCode, topCode, cfgImport.ProgressivoGias)

            For Each ff In listaRiporto.RispostaStringa.values

                'scrivo dato gias
                Dim OUTPUT_Appezza As Integer

                Dim ErroreAppezza As String = ""
                scriviAppezza.Apertura_Appezzamento(
                    cfgImport.Piva,
                    cfgImport.SaCod,
                    cfgImport.CampoCod,
                    OUTPUT_Appezza,
                    ff.name,
                    0,
                    baseCode,
                    topCode,
                    ErroreAppezza,
                    CostantiPersonalizzate.AGRODATAINIZIO,
                    CostantiPersonalizzate.AGRODATAFINE,
                    objParametri_server,
                    objParametri_utenti)

                Dim OUTPUT_Id_Reg As Integer
                Dim ErroreImpianto As String = ""

                Dim esitoScrittura As Boolean = scriviImpiantoDistinta.Apertura_Impianto_Scrivi(
                    OUTPUT_Id_Reg,
                    ErroreImpianto,
                    baseCode,
                    topCode,
                    cfgImport.Piva, cfgImport.SaCod, OUTPUT_Appezza,
                    0,
                    AGRODATAINIZIO,
                    AGRODATAFINE,
                    "",
                    "",
                    objParametri_server
                )

                'scrivo associazione
                AssociaEntitaGiasConEntitaJD(cfgImport.Piva, cfgImport.SaCod, OUTPUT_Appezza, ff.id, objParametri_server, ScriviAssociazioneGISJD, seqLeggi, 0)

            Next


            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la transazione e la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_server)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            rval.RispostaOK = True
            rval.RispostaStringa = ""

        Catch ex As Exception

            'Faccio il rollback della transazione
            If objParametri_server.objTransazione IsNot Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_server)

            End If

            messaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)


            Dim Messaggio As String = ""
            If messaggioErrore <> "" Then


                Messaggio += "ERR: Sono stati rilevati i seguenti errori : " & vbCrLf
                Messaggio += "" & vbCrLf
                Messaggio += messaggioErrore
                Messaggio += "" & vbCrLf
                Messaggio += "Ritentare il salvataggio dopo la correzione ..."

            End If

            rval.RispostaOK = False
            rval.Errore = Messaggio

        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_server)

        End Try

        Return rval


    End Function

    Private Sub AssociaEntitaGiasConEntitaJD(
            piva As String,
            sa_cod As Integer,
            appezza As Integer,
            ElemJD As String,
            objParametri_server As AgronicaCoreParametri,
            ScriviAssociazioneGISJD As jDeereDataModelDAL_EntitaGIAS_W,
            SeqLeggi As Agro_Sequenze,
            GISEntitaCod As Integer
    )

        'Dim jDeereDataModel_EntitaGIAS_COD As Integer =
        '    SeqLeggi.Agronica_SequenzaTabelle_NuovoID("jDeereDataModel_EntitaGIAS", objParametri_server)

        'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
        Dim jDeereDataModel_EntitaGIAS_COD As Integer =
            SeqLeggi.NuovoId_Tabella("jDeereDataModel_EntitaGIAS", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_server)


        ScriviAssociazioneGISJD.ScriviAssociazioneGIASJD(
            enum_TipoEntitaJohnDeere.Field,
            jDeereDataModel_EntitaGIAS_COD,
            piva, sa_cod, appezza,
            GISEntitaCod,
            ElemJD,
            AGRODATAINIZIO,
            AGRODATAFINE,
            objParametri_server
        )

    End Sub

    Public Function RiportoFieldGIASLeggiOggetti(cfgImport As ConfigurazioneImportazione, ListaChiavi As String, objParametri_Server As AgronicaCoreParametri) As rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_Field))
        Dim rval As New rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_Field))

        Dim xFiltroB As String
        If ListaChiavi <> "" Then
            xFiltroB = ListaChiavi
        Else
            xFiltroB = ElencaIDJDNonAncoraImportateDatoTipoEntita(enum_TipoEntitaJohnDeere.Field, objParametri_Server)
        End If



        If String.IsNullOrEmpty(xFiltroB) Then
            rval.RispostaOK = True
            rval.RispostaStringa = New jDeereDataModel_GenericList(Of jDeereDataModel_Field) With {.total = 0, .values = New List(Of jDeereDataModel_Field)}
            Return rval
        End If

        Dim leggi As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Field_R
        Dim jSonContents As String = leggi.LeggiViaJsonSQL(False, False, xFiltroB, objParametri_Server)

        Dim fieldsVal As List(Of jDeereDataModel_Field) = CaricaEntitaJDViaJsonString(Of List(Of jDeereDataModel_Field))(jSonContents)
        Dim fields As New jDeereDataModel_GenericList(Of jDeereDataModel_Field)

        fields.values = fieldsVal
        fields.total = fieldsVal.Count

        rval.RispostaStringa = fields
        rval.RispostaOK = True

        Return rval

    End Function

    Private Function ListaChiaviEstraiCampiJD(listaChiavi As String, TipoChiaveDaEstrarre As enum_TipoChiaveJohnDeere) As String

        If TipoChiaveDaEstrarre = enum_TipoChiaveJohnDeere.ChiaveGias Then
            Dim rvalStmt As String = AgronicaCoreUtility.QueryBuilderUtility.EstraiFiltriSqlClauseDaStringaPipe(
                listaChiavi, "AND", "OR", "", "-", "a.piva,a.sa_Cod,a.appezza", "100")
            Return rvalStmt
        End If

        Dim vc As String() = listaChiavi.TrimStart("[").TrimEnd("]").Trim(" ").Replace("""", "").Split(",")
        Dim rval As New List(Of String)
        For Each s In vc
            If s.StartsWith("-0-") Then
                rval.Add(s.Split("-")(2))
            End If
        Next

        Dim sRval As String = " ID in (-1)"
        If rval.Count > 0 Then
            sRval = " ID in ( " & String.Join(",", rval) & ")"
        End If

        Return sRval
    End Function


#End Region

#Region "Riporto Operations su GIAS"


    Public Function RiportoOperationsJDToGIAS(cfgImport As ConfigurazioneImportazione, ListaChiavi As String, objParametri_server As AgronicaCoreParametri, objParametri_utenti As AgronicaCoreParametri) As RispostaStandard

        Dim messaggioErrore As String = ""

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim rval As New AgronicaCoreVarieBIZ.RispostaStandard

        'TODO: Gestire la transazione
        Try

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                            FlagTransazioneLocale,
                                                                            objParametri_Server)

            Throw New Exception("TEST")

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la transazione e la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            rval.RispostaOK = True
            rval.RispostaStringa = "..."

        Catch ex As Exception

            'Faccio il rollback della transazione
            If objParametri_Server.objTransazione IsNot Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)

            End If

            messaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)


            Dim Messaggio As String = ""
            If messaggioErrore <> "" Then


                Messaggio += "ERR: Sono stati rilevati i seguenti errori : " & vbCrLf
                Messaggio += "" & vbCrLf
                Messaggio += messaggioErrore
                Messaggio += "" & vbCrLf
                Messaggio += "Ritentare il salvataggio dopo la correzione ..."

            End If

            rval.RispostaOK = False
            rval.Errore = Messaggio



        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

        End Try

        Return rval


    End Function

    Private Sub OperationsLettiElaboraTabella(dt As DataTable)

        dt.Columns.Add(New DataColumn("StatoSincronizzazione", GetType(String)))
        dt.Columns.Add(New DataColumn("StatoSincronizzazione_COD", GetType(Integer)))

        For Each row As DataRow In dt.Rows
            If Not String.IsNullOrWhiteSpace(row("DescrizioneGias").ToString()) Then
                row("StatoSincronizzazione") = "Sincronizzato"
                row("StatoSincronizzazione_COD") = enum_StatoSincronizzazioneJohnDeere.Sincronizzato
            Else
                row("StatoSincronizzazione") = "Solo in John Deere"
                row("StatoSincronizzazione_COD") = enum_StatoSincronizzazioneJohnDeere.SoloInJohnDeere
            End If
        Next
    End Sub

    Public Function LeggiOperationsSincronizzatiConJD(orgId As String, elementiSincro As String, piva As String, sa_Cod As Integer, objParametri_Server As AgronicaCoreParametri) As RispostaStandard

        Dim rval As New RispostaStandard
        Try

            Dim letturaOrganizzazioni As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Organization_R
            Dim rEntityGIAS As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_EntitaGias_R
            Dim orgIDGias As Integer = letturaOrganizzazioni.LeggiIDViaGUID(orgId, objParametri_Server)

            Dim letturaOperations As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_FieldOperation_R

            Dim keystr = ListaChiaviEstraiCampiJD(elementiSincro, enum_TipoChiaveJohnDeere.ChiaveGias)

            Dim dtF = rEntityGIAS.LeggiIDJdDaChiaveGIAS(enum_TipoEntitaJohnDeere.Field, keystr, "", "", objParametri_Server)
            Dim FieldID As Integer = dtF(0)(0)
            Dim dt As DataTable =
                letturaOperations.Leggi(orgIDGias, FieldID, "", "", "", "", objParametri_Server)

            OperationsLettiElaboraTabella(dt)

            rval.RispostaOK = True
            rval.RispostaStringa = LeggiOperationsSincronizzatiConJDKendo(dt)

        Catch ex As Exception

            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return rval
    End Function


    Private Sub TestOperationFakeData(dt As DataTable)
        TestOperationFakeDataCreateDT(dt)
        Dim r As DataRow = dt.NewRow
        r("chiave") = "-0-21212"
        r("StatoSincronizzazione") = "aaa"
        r("StatoSincronizzazione_COD") = 1

        'dati JD
        r("fieldOperationType") = "aaa"
        r("startDate") = AGRODATAINIZIO
        r("endDate") = AGRODATAFINE

        'Dati Gias
        r("DescrizioneGias") = "aaa"


        'info "field" ed appezzamento
        r("ConfiniPresenti") = 1
        r("FieldName") = "aaa"
        r("BoundaryName") = "aaa"

        r("ClientName") = "aaa"
        r("ClientID") = 1

        r("FarmName") = "aaa"
        r("FarmID") = 1

        r("App_Nome") = "aaa"

        'altre info...
        r("Data_Modifica") = AGRODATAINIZIO
        dt.Rows.Add(r)
    End Sub

    Private Sub TestOperationFakeDataCreateDT(dt As DataTable)

        dt.Columns.Add(New DataColumn("chiave", GetType(String)))
        dt.Columns.Add(New DataColumn("StatoSincronizzazione", GetType(String)))
        dt.Columns.Add(New DataColumn("StatoSincronizzazione_COD", GetType(Integer)))

        'dati JD
        dt.Columns.Add(New DataColumn("fieldOperationType", GetType(String)))
        dt.Columns.Add(New DataColumn("startDate", GetType(Date)))
        dt.Columns.Add(New DataColumn("endDate", GetType(Date)))

        'Dati Gias
        dt.Columns.Add(New DataColumn("DescrizioneGias", GetType(String)))


        'info "field" ed appezzamento
        dt.Columns.Add(New DataColumn("ConfiniPresenti", GetType(Integer)))
        dt.Columns.Add(New DataColumn("FieldName", GetType(String)))
        dt.Columns.Add(New DataColumn("BoundaryName", GetType(String)))

        dt.Columns.Add(New DataColumn("ClientName", GetType(String)))
        dt.Columns.Add(New DataColumn("ClientID", GetType(Integer)))

        dt.Columns.Add(New DataColumn("FarmName", GetType(String)))
        dt.Columns.Add(New DataColumn("FarmID", GetType(Integer)))

        dt.Columns.Add(New DataColumn("App_Nome", GetType(String)))

        'altre info...
        dt.Columns.Add(New DataColumn("Data_Modifica", GetType(Date)))

    End Sub


    Public Function LeggiListaOperationsDispo(cfgImport As ConfigurazioneImportazione, orgId As String, FieldKey As String, objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, cfgOauthJDeere As jDeereDataModel_ApiCFG) As RispostaStandard
        Dim rsVal As New RispostaStandard
        Try
            Dim rvalDB As New RispostaStandard

            'se richiesta la lettura da api
            If cfgOauthJDeere.JDeereWebHookCFG.RefreshOperation OrElse cfgOauthJDeere.ForcedReFresh Then

                Dim KeyList = JsonConvert.DeserializeObject(Of String())(FieldKey)

                Dim fldList = getIDFieldByListaChiavi(FieldKey, objParametri_Server)
                Dim rFld = New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Field_R

                Dim FieldID = rFld.LeggiGUIDViaID(fldList(0), objParametri_Server)

                Dim ops As rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_FieldOperation)) =
                LeggiListaOperationsAPI(orgId, FieldID, objParametri_Server, cfgOauthJDeere)

                If ops.RispostaOK Then
                    rvalDB = leggiListaOperationsDispoScriviDB(orgId, ops.RispostaStringa, objParametri_Server)

                    'If rvalDB.RispostaOK Then
                    '    Dim rvalRiportoBoundaryToGIS As RispostaStandard = RiportoBoundaryJDToGIAS(cfgImport, "", objParametri_Server)
                    'End If

                Else
                    Throw New Exception(ops.Errore)
                End If

            End If
            rsVal.RispostaOK = True
            rsVal.RispostaStringa = ""

        Catch ex As Exception
            rsVal.RispostaOK = False
            rsVal.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return rsVal
    End Function

    Private Function leggiListaOperationsDispoScriviDB(orgId As String, ListOperations As jDeereDataModel_GenericList(Of jDeereDataModel_FieldOperation), objParametri_Server As AgronicaCoreParametri) As RispostaStandard
        Dim rval As New RispostaStandard

        Try

            Dim jdR As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_FieldOperation_R
            Dim jdM As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_FieldOperationMeasurement_R

            Dim Inseriti As New List(Of jDeereDataModel_FieldOperation)
            Dim modificati As New List(Of jDeereDataModel_FieldOperation)
            Dim mInseriti As New List(Of jDeereDataModel_FieldOperationMeasurement)
            Dim mModificati As New List(Of jDeereDataModel_FieldOperationMeasurement)

            For Each ope In ListOperations.values
                leggiListaOperationDispoScriviDBAccodaArrList(jdR, ope, Inseriti, modificati, objParametri_Server)
            Next

            Dim sInseriti As String = JsonConvert.SerializeObject(Inseriti)
            Dim sModificati As String = JsonConvert.SerializeObject(modificati)
            'sInseriti = sInseriti.Replace("""id""", """ID""")
            'sInseriti = sInseriti.Replace("""name""", """Name""")

            'sModificati = sModificati.Replace("""id""", """ID""")
            'sModificati = sModificati.Replace("""name""", """Name""")

            Dim biz As New AgronicaCoreAnagrafeBIZ.jDeereDataModelBIZ_FieldOperation_W

            Dim rvalScrittura As String =
                biz.Aggiorna_jDeereDataModelBIZ_FieldOperation(sInseriti, sModificati, "[]", objParametri_Server)

            For Each ope In ListOperations.values
                If ope.measurement IsNot Nothing Then
                    For Each m In ope.measurement.values
                        m.FieldOperationID = ope.id
                        leggiListaOperationMeasurementDispoScriviDBAccodaArrList(jdM, m, mInseriti, mModificati, objParametri_Server)
                    Next
                End If
            Next
            Dim smInseriti As String = JsonConvert.SerializeObject(mInseriti)
            Dim smModificati As String = JsonConvert.SerializeObject(mModificati)

            smInseriti = smInseriti.Replace("""id""", """ID""")
            smInseriti = smInseriti.Replace("""name""", """Name""")

            smModificati = smModificati.Replace("""id""", """ID""")
            smModificati = smModificati.Replace("""name""", """Name""")

            Dim bizM As New AgronicaCoreAnagrafeBIZ.jDeereDataModelBIZ_FieldOperationMeasurement_W
            Dim rvalScritturaM As String =
                bizM.Aggiorna_jDeereDataModelBIZ_FieldOperationMeasurement(smInseriti, smModificati, "[]", objParametri_Server)


            rval.RispostaOK = True
            rval.RispostaStringa = ""
        Catch ex As Exception

            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return rval
    End Function

    Private Sub leggiListaOperationDispoScriviDBAccodaArrList(
            ByVal jdR As AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_FieldOperation_R,
            ByVal ope As jDeereDataModel_FieldOperation,
            ByRef Inseriti As List(Of jDeereDataModel_FieldOperation),
            ByRef modificati As List(Of jDeereDataModel_FieldOperation),
            ByVal objParametri_Server As AgronicaCoreParametri
    )

        Dim curjDeereDataModelBIZ_FieldOperation As Boolean = jdR.TestGUID(ope.guid, objParametri_Server)
        If curjDeereDataModelBIZ_FieldOperation Then
            modificati.Add(ope)
        Else
            Inseriti.Add(ope)
        End If

    End Sub

    Private Sub leggiListaOperationMeasurementDispoScriviDBAccodaArrList(
            ByVal jdR As AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_FieldOperationMeasurement_R,
            ByVal mis As jDeereDataModel_FieldOperationMeasurement,
            ByRef Inseriti As List(Of jDeereDataModel_FieldOperationMeasurement),
            ByRef modificati As List(Of jDeereDataModel_FieldOperationMeasurement),
            ByVal objParametri_Server As AgronicaCoreParametri
    )

        Dim curjDeereDataModelBIZ_FieldOperationMeasurement As Boolean = jdR.TestMeasureXFieldOP(mis.FieldOperationID, mis.measurementName, mis.measurementCategory, objParametri_Server)
        If curjDeereDataModelBIZ_FieldOperationMeasurement Then
            modificati.Add(mis)
        Else
            Inseriti.Add(mis)
        End If

    End Sub

    Private Function LeggiListaOperationsAPI(orgId As String, FieldID As String, objParametri_Server As AgronicaCoreParametri, cfgOauthJDeere As jDeereDataModel_ApiCFG) As rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_FieldOperation))
        Dim rval As New rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_FieldOperation))

        Try
            Dim OrganizationID = GetOrganizzationID(orgId, objParametri_Server)
            Dim idField = GetFieldID(FieldID, objParametri_Server)
            Dim measurements As rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_FieldOperationMeasurement))
            'chiamata api
            rval = JDeereGetFieldOpByFieldId(cfgOauthJDeere, orgId, FieldID)
            If rval.RispostaOK Then
                For Each ope In rval.RispostaStringa.values
                    ope.OrganizationID = OrganizationID
                    ope.guid = ope.id
                    ope.id = 0
                    ope.FieldID = idField
                    'elenco misurazioni dei sensori collegate
                    measurements = JDeereGetMeasurementByFieldOp(cfgOauthJDeere, ope.guid)
                    If measurements.RispostaOK Then
                        ope.measurement = measurements.RispostaStringa
                    Else
                        Throw New Exception(measurements.Errore)
                    End If

                Next
            End If
        Catch ex As Exception

            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return rval
    End Function

    Private Function GetFieldID(FieldGUID As String, objParametri_Server As AgronicaCoreParametri) As Integer
        Dim FieldID As Integer = -1
        Try
            Dim fldR = New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Field_R
            FieldID = fldR.LeggiIDViaGUID(FieldGUID, objParametri_Server)
        Catch ex As Exception

        End Try
        Return FieldID
    End Function

#End Region

#Region "FieldOperation File Request"
    Public Function SubmitFileRequest(fieldOpsGUID As String, cfgOauthJDeere As jDeereDataModel_ApiCFG, objParametri_Server As AgronicaCoreParametri) As RispostaStandard
        Dim r As New RispostaStandard
        Try
            ' data la chiave della operation devo richiamare una funzione che:
            '1- verifichi la presenza di una FileRequest
            '   a- Se manca la inserisce e manda messaggio all'utente di aspettare qualche minuto
            '   b- Se già presente bisogna verificare lo stato:
            '       - Se RequestState=0 (inserito) -> non inserire la richiesta e mandare messaggio "Richiesta di download per lo Shape File già presente, attendere l'elaborazione"
            '       - Se RequestState=1 (In Elaborazione) -> non inserire la richiesta e mandare messaggio "Esiste già un'altra richiesta in corso di elaborazione, attendere"
            '       - Se RequestState=2 (Completata) -> aggiornare la richiesta già presente con reset di (RequestState=0,Esito="",Request_Parameters=<nuovi_parametri>) e manda messaggio all'utente di aspettare qualche minuto 
            '       - Se RequestState=3 (Errore) -> Visualizzare Esito della vecchia richiesta a video e chiedere se si vuole risottomettere la richiesta, se l'utente clicca SI.
            '                                       aggiornare la richiesta già presente con reset di (RequestState=0,Esito="",Request_Parameters=<nuovi_parametri>) e manda messaggio all'utente di aspettare qualche minuto

            Dim jdR As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_FieldOperation_R
            Dim jdFR As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_FieldOperationFileRequest_R
            Dim jdF As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Field_R
            Dim fieldOpsID = jdR.LeggiIDViaGUID(fieldOpsGUID, objParametri_Server)

            Dim dt = jdFR.LeggiDaFieldOperation(fieldOpsID, fieldOpsGUID, objParametri_Server)
            Dim fldDT = jdR.LeggiFieldOperation(fieldOpsID, objParametri_Server)

            Dim newReq As New jDeereDataModel_FieldOperationFileRequest

            Dim fieldGuid = jdF.LeggiGUIDViaID(fldDT(0)("FieldID"), objParametri_Server)


            'cfgOauthJDeere
            If dt.Rows.Count <= 0 Then
                'non esiste alcuna richiesta per la FieldOps corrente -> va creata
                newReq.FieldOperationID = fieldOpsID
                newReq.FieldOperationGUID = fieldOpsGUID
                newReq.OAuth_Token = cfgOauthJDeere.JDeere.AuthToken
                newReq.Request_Parameters = JsonConvert.SerializeObject(New jDeereDataModel_FieldOperationFileRequest_Parameter() With {.oauth_cfg = cfgOauthJDeere, .fieldOpGuid = fieldOpsGUID})
                If newReq.OAuth_Token <> "" Then
                    Dim ListReq = New List(Of jDeereDataModel_FieldOperationFileRequest)
                    ListReq.Add(newReq)

                    r = FieldOperationFileRequestScriviDB(ListReq, objParametri_Server)
                    If r.RispostaOK Then
                        dt = jdFR.LeggiDaFieldOperation(fieldOpsID, fieldOpsGUID, objParametri_Server)
                        Dim dr = dt(0)
                        newReq.ID = dr("ID")

                        r.RispostaStringa = newReq.ID.ToString() & "|" & fieldGuid.ToString()
                    Else
                        Throw New Exception(r.Errore)
                    End If
                Else
                    Throw New Exception("Token login JDeere non valido. Eseguire Login")
                End If
            Else
                'esiste già un richiesta pregressa
                Dim dr = dt(0)

                'temporaneo in attesa di soluzione migliore
                newReq.ID = dr("ID")
                newReq.FieldOperationID = dr("FieldOperationID")
                newReq.FieldOperationGUID = dr("FieldOperationGUID")
                newReq.Allegati_Documenti_Cod = dr("Allegati_Documenti_Cod")
                newReq.RequestState = dr("RequestState")
                newReq.Request_Parameters = dr("Request_Parameters")
                newReq.Esito = dr("Esito")
                newReq.OAuth_Token = dr("OAuth_Token")

                Select Case newReq.RequestState
                    Case 1
                        r.RispostaOK = False
                        r.RispostaStringa = "Esiste già un'altra richiesta di download in corso, attendere...."
                    Case Else
                        newReq.OAuth_Token = cfgOauthJDeere.JDeere.AuthToken
                        newReq.Request_Parameters = JsonConvert.SerializeObject(New jDeereDataModel_FieldOperationFileRequest_Parameter() With {.oauth_cfg = cfgOauthJDeere, .fieldOpGuid = fieldOpsGUID})
                        newReq.RequestState = 0

                        If newReq.OAuth_Token <> "" Then
                            Dim ListReq = New List(Of jDeereDataModel_FieldOperationFileRequest)
                            ListReq.Add(newReq)

                            r = FieldOperationFileRequestScriviDB(ListReq, objParametri_Server)
                            If r.RispostaOK Then
                                r.RispostaStringa = newReq.ID.ToString() & "|" & fieldGuid.ToString()
                            Else
                                Throw New Exception(r.Errore)
                            End If
                        Else
                            Throw New Exception("Token login JDeere non valido. Eseguire Login")
                        End If
                End Select
            End If
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return r
    End Function

    Private Function FieldOperationFileRequestScriviDB(ListFileRequest As List(Of jDeereDataModel_FieldOperationFileRequest), objParametri_Server As AgronicaCoreParametri) As RispostaStandard
        Dim rval As New RispostaStandard

        Try

            Dim jdR As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_FieldOperationFileRequest_R

            Dim Inseriti As New List(Of jDeereDataModel_FieldOperationFileRequest)
            Dim modificati As New List(Of jDeereDataModel_FieldOperationFileRequest)

            For Each req In ListFileRequest
                FieldOperationFileRequestScriviDB(jdR, req, Inseriti, modificati, objParametri_Server)
            Next

            Dim sInseriti As String = JsonConvert.SerializeObject(Inseriti)
            Dim sModificati As String = JsonConvert.SerializeObject(modificati)
            'sInseriti = sInseriti.Replace("""id""", """ID""")
            'sInseriti = sInseriti.Replace("""name""", """Name""")

            'sModificati = sModificati.Replace("""id""", """ID""")
            'sModificati = sModificati.Replace("""name""", """Name""")

            Dim biz As New AgronicaCoreAnagrafeBIZ.jDeereDataModelBIZ_FieldOperationFileRequest_W

            Dim rvalScrittura As String =
                biz.Aggiorna_jDeereDataModelBIZ_FieldOperationFileRequest(sInseriti, sModificati, "[]", objParametri_Server)

            rval.RispostaOK = True
            rval.RispostaStringa = ""
        Catch ex As Exception

            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return rval
    End Function

    Private Sub FieldOperationFileRequestScriviDB(
            ByVal jdR As AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_FieldOperationFileRequest_R,
            ByVal req As jDeereDataModel_FieldOperationFileRequest,
            ByRef Inseriti As List(Of jDeereDataModel_FieldOperationFileRequest),
            ByRef modificati As List(Of jDeereDataModel_FieldOperationFileRequest),
            ByVal objParametri_Server As AgronicaCoreParametri
    )

        Dim curjDeereDataModel_FieldOperationFileRequest As Boolean = jdR.Test(req.ID, objParametri_Server)
        If curjDeereDataModel_FieldOperationFileRequest Then
            modificati.Add(req)
        Else
            Inseriti.Add(req)
        End If

    End Sub
#End Region

#Region "Boundaries"
    ''' <summary>
    ''' Legge da api e riporta su DB le Farm dispnibili per un organizzazione
    ''' </summary>
    ''' <returns></returns>
    Public Function leggiListaBoundaryDaCampoDispo(FieldID As String, objParametri_Server As AgronicaCoreParametri) As RispostaStandard

        Dim rval As New RispostaStandard

        Try

            Dim orgs As rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_Boundary)) =
                leggiListaBoundaryDaCampoDispoAPI(FieldID, objParametri_Server)

            If orgs.RispostaOK Then
                leggiListaBoundaryDaCampoDispoAPIScriviDB(orgs.RispostaStringa, objParametri_Server)
            Else
                Throw New Exception(orgs.Errore)
            End If


            rval.RispostaOK = True
            rval.RispostaStringa = "Sincronizzazione avvenuta correttamente"
        Catch ex As Exception

            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return rval

    End Function

    Private Function leggiListaBoundaryDaCampoDispoAPIScriviDB(bnd As jDeereDataModel_GenericList(Of jDeereDataModel_Boundary), objParametri_Server As AgronicaCoreParametri) As RispostaStandard

        Dim rval As New RispostaStandard

        Try

            'Vanni

            Dim jdR As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Boundary_R

            Dim Inseriti As New List(Of jDeereDataModel_Boundary)
            Dim modificati As New List(Of jDeereDataModel_Boundary)

            For Each org In bnd.values
                leggiListaBoundaryDaCampoDispoAPIScriviDBAccodaArrList(jdR, org, Inseriti, modificati, objParametri_Server)
            Next

            Dim sInseriti As String = JsonConvert.SerializeObject(Inseriti)
            Dim sModificati As String = JsonConvert.SerializeObject(modificati)

            Dim biz As New AgronicaCoreAnagrafeBIZ.jDeereDataModelBIZ_Boundary_W
            biz.Aggiorna_jDeereDataModel_Boundary(sInseriti, sModificati, "", objParametri_Server)

            rval.RispostaOK = True
            rval.RispostaStringa = ""
        Catch ex As Exception

            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return rval
    End Function



    Private Sub leggiListaBoundaryDaCampoDispoAPIScriviDBAccodaArrList(
            ByVal jdR As AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Boundary_R,
            ByVal bnd As jDeereDataModel_Boundary,
            ByRef Inseriti As List(Of jDeereDataModel_Boundary),
            ByRef modificati As List(Of jDeereDataModel_Boundary),
            ByVal objParametri_Server As AgronicaCoreParametri
    )

        Dim curjDeereDataModelBIZ_Farm As Boolean = jdR.Test(bnd.id, objParametri_Server)
        If curjDeereDataModelBIZ_Farm Then
            modificati.Add(bnd)
        Else
            Inseriti.Add(bnd)
        End If

    End Sub

    Public Function leggiListaBoundaryDaCampoDispoAPI(FieldID As String, objParametri_Server As AgronicaCoreParametri) As rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_Boundary))


        Dim rval As New rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_Boundary))

        Try

            'Lorenzo
            Dim bnd As jDeereDataModel_GenericList(Of jDeereDataModel_Boundary)

            'chiamata api

            rval.RispostaOK = True
            rval.RispostaStringa = bnd
        Catch ex As Exception

            rval.RispostaOK = False
            rval.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return rval


    End Function
    Public Function CaricaBoundariesDaListaJson(jsonData As String) As List(Of jDeereDataModel_Boundary)

        Dim boundaries As List(Of jDeereDataModel_Boundary) =
            JsonConvert.DeserializeObject(Of List(Of jDeereDataModel_Boundary))(jsonData)

        Return boundaries

    End Function


    Public Shared Function ListaPuntiJDToAgronicaXYZ(listaPunti As List(Of jDeereDataModel_Point)) As List(Of xyz)

        Dim rval As New List(Of xyz)
        For Each p In listaPunti
            rval.Add(New xyz With {.X = p.lat, .Y = p.lon, .Z = 0})
        Next

        Return rval

    End Function

#End Region

#Region "Farm"

    ''' <summary>
    ''' Legge da api e riporta su DB le Farm dispnibili per un organizzazione
    ''' </summary>
    ''' <returns></returns>
    Public Function leggiListaFarmDaOrganizzazioneDispo(orgID As String, objParametri_Server As AgronicaCoreParametri, ByVal cfgOauthJDeere As jDeereDataModel_ApiCFG) As RispostaStandard

        Dim rval As New RispostaStandard

        Try

            Dim orgs As rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_ItemGenerico)) =
                leggiListaFarmDaOrganizzazioneDispoAPI(orgID, objParametri_Server, cfgOauthJDeere)

            If orgs.RispostaOK Then
                leggiListaFarmDaOrganizzazioneDispoScriviDB(orgs.RispostaStringa, objParametri_Server)
            Else
                Throw New Exception(orgs.Errore)
            End If


            rval.RispostaOK = True
            rval.RispostaStringa = "Sincronizzazione avvenuta correttamente"
        Catch ex As Exception

            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return rval

    End Function


    Private Function leggiListaFarmDaOrganizzazioneDispoAPI(orgID As String, objParametri_Server As AgronicaCoreParametri, ByVal cfgOauthJDeere As jDeereDataModel_ApiCFG) As rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_ItemGenerico))

        Dim rval = New rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_ItemGenerico))

        Try

            'Lorenzo

            Dim orgs As jDeereDataModel_GenericList(Of jDeereDataModel_ItemGenerico)


            'chiamata api


            rval.RispostaOK = True
            rval.RispostaStringa = orgs

        Catch ex As Exception
            rval.RispostaOK = False
            rval.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return rval
    End Function

    Private Function leggiListaFarmDaOrganizzazioneDispoScriviDB(orgs As jDeereDataModel_GenericList(Of jDeereDataModel_ItemGenerico), objParametri_Server As AgronicaCoreParametri) As RispostaStandard

        Dim rval As New RispostaStandard

        Try

            'Vanni

            Dim jdR As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Farm_R

            Dim Inseriti As New List(Of jDeereDataModel_ItemGenerico)
            Dim modificati As New List(Of jDeereDataModel_ItemGenerico)

            For Each org In orgs.values
                leggiListaFarmDaOrganizzazioneDispoScriviDBAccodaArrList(jdR, org, Inseriti, modificati, objParametri_Server)
            Next

            Dim sInseriti As String = JsonConvert.SerializeObject(Inseriti)
            Dim sModificati As String = JsonConvert.SerializeObject(modificati)

            'sInseriti = sInseriti.Replace(",""links"":[]", "")
            'sModificati = sModificati.Replace(",""links"":[]", "")

            sInseriti = sInseriti.Replace("""id""", """ID""")
            sInseriti = sInseriti.Replace("""name""", """Name""")
            sModificati = sModificati.Replace("""id""", """ID""")
            sModificati = sModificati.Replace("""name""", """Name""")

            Dim biz As New AgronicaCoreAnagrafeBIZ.jDeereDataModelBIZ_Organization_W
            biz.Aggiorna_jDeereDataModelBIZ_Organization(sInseriti, sModificati, "[]", objParametri_Server)

            rval.RispostaOK = True
            rval.RispostaStringa = ""
        Catch ex As Exception

            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return rval
    End Function

    Private Sub leggiListaFarmDaOrganizzazioneDispoScriviDBAccodaArrList(
            ByVal jdR As AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Farm_R,
            ByVal org As jDeereDataModel_ItemGenerico,
            ByRef Inseriti As List(Of jDeereDataModel_ItemGenerico),
            ByRef modificati As List(Of jDeereDataModel_ItemGenerico),
            ByVal objParametri_Server As AgronicaCoreParametri
    )

        Dim curjDeereDataModelBIZ_Farm As Boolean = jdR.Test(org.id, objParametri_Server)
        org.links.Clear()
        If curjDeereDataModelBIZ_Farm Then
            modificati.Add(org)
        Else
            Inseriti.Add(org)
        End If

    End Sub

#End Region

#Region "Organization"

    Public Function GetOrganizzationID(orgID As String, objParametri_Server As AgronicaCoreParametri) As Integer
        Dim OrganizzationID As Integer = -1
        Try
            Dim orgR = New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Organization_R
            OrganizzationID = orgR.LeggiIDViaGUID(orgID, objParametri_Server)
        Catch ex As Exception

        End Try
        Return OrganizzationID
    End Function

    ''' <summary>
    ''' Legge da api e riporta su DB le organizzazioni dispnibili
    ''' </summary>
    ''' <returns></returns>
    Public Function leggiListaOrganizzazioniDispo(username As String, name As String, objParametri_Server As AgronicaCoreParametri, ByVal cfgOauthJDeere As jDeereDataModel_ApiCFG) As RispostaStandard

        Dim rval As New RispostaStandard

        Try

            'chiama la api se necessario.

            If cfgOauthJDeere.JDeereWebHookCFG.RefreshOrganization OrElse cfgOauthJDeere.ForcedReFresh Then

                Dim orgs As rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_Organization)) =
                    leggiListaOrganizzazioniDispoAPI(username, objParametri_Server, cfgOauthJDeere)

                If orgs.RispostaOK Then
                    cfgOauthJDeere.JDeereWebHookCFG.RefreshOrganization = False
                    leggiListaOrganizzazioniDispoScriviDB(username, name, orgs.RispostaStringa, objParametri_Server)
                Else
                    Throw New Exception(orgs.Errore)
                End If

            End If


            rval.RispostaOK = True
            rval.RispostaStringa = "Sincronizzazione avvenuta correttamente"
        Catch ex As Exception

            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return rval

    End Function

    Private Function leggiListaOrganizzazioniDispoAPI(username As String, objParametri_Server As AgronicaCoreParametri, ByVal cfgOauthJDeere As jDeereDataModel_ApiCFG) As rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_Organization))

        Dim rval As New rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_Organization))

        Try
            'chiamata api
            rval = JDeereGetOrganizationList(cfgOauthJDeere, username)
        Catch ex As Exception

            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return rval
    End Function

    Private Function leggiListaOrganizzazioniDispoScriviDB(username As String, name As String, orgs As jDeereDataModel_GenericList(Of jDeereDataModel_Organization), objParametri_Server As AgronicaCoreParametri) As RispostaStandard

        Dim rval As New RispostaStandard

        Try

            ' 1. Scrivo lo Username se non esiste
            leggiListaUtentiScriviDB(username, name, objParametri_Server)

            ' 2. Scrivo le ornanizzazioni

            Dim jdR As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Organization_R

            Dim Inseriti As New List(Of jDeereDataModel_Organization)
            Dim modificati As New List(Of jDeereDataModel_Organization)

            For Each org In orgs.values
                leggiListaOrganizzazioniDispoScriviDBAccodaArrList(jdR, org, Inseriti, modificati, objParametri_Server)
            Next

            Dim sInseriti As String = JsonConvert.SerializeObject(Inseriti)
            Dim sModificati As String = JsonConvert.SerializeObject(modificati)

            sInseriti = sInseriti.Replace("""id""", """ID""")
            sInseriti = sInseriti.Replace("""name""", """Name""")
            sInseriti = sInseriti.Replace("false", "0")
            sInseriti = sInseriti.Replace("true", "1")

            sModificati = sModificati.Replace("""id""", """ID""")
            sModificati = sModificati.Replace("""name""", """Name""")
            sModificati = sModificati.Replace("false", "0")
            sModificati = sModificati.Replace("true", "1")

            Dim biz As New AgronicaCoreAnagrafeBIZ.jDeereDataModelBIZ_Organization_W
            Dim rvalScrittura As String =
                biz.Aggiorna_jDeereDataModelBIZ_Organization(sInseriti, sModificati, "[]", objParametri_Server)


            ' 3. associazio user con organizzazioni

            For Each org In orgs.values
                leggiListaOrganizzazionXUtentiScriviDB(org.id, username, objParametri_Server)
            Next


            rval.RispostaOK = True
            rval.RispostaStringa = ""
        Catch ex As Exception

            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return rval
    End Function

    Private Function leggiListaUtentiScriviDB(Username As String, Name As String, objParametri_Server As AgronicaCoreParametri) As RispostaStandard

        Dim UsrRead As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Account_R
        Dim UsrWrite As New AgronicaCoreAnagrafeBIZ.jDeereDataModelBIZ_Account_W


        Dim IDAccount As Integer =
            UsrRead.LeggiIDAccountDaUsername(Username, objParametri_Server)

        If IDAccount < 0 Then
            Dim righeInserite As String = "[{ ""UsernaName"": """ & Username & """,  ""IDAccount"": 0, ""Name"": """ & Name & """}]"
            UsrWrite.Aggiorna_jDeereDataModelBIZ_Account(righeInserite, "[]", "[]", objParametri_Server)
        End If

        'queste istruzioni vengono eseguite se non va in eccezione la scrittura
        Dim rval As New RispostaStandard
        rval.RispostaOK = True

        Return rval

    End Function

    Private Function leggiListaOrganizzazionXUtentiScriviDB(OrgID As Integer, Username As String, objParametri_Server As AgronicaCoreParametri) As RispostaStandard


        Dim UsrRead As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Account_R
        Dim OrgRead As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Organization_R
        Dim OrgUsrRead As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_AccountXOrganization_R
        Dim OrgUsrWrite As New AgronicaCoreAnagrafeBIZ.jDeereDataModelBIZ_AccountXOrganization_W

        Dim IDOrganization As Integer =
                OrgRead.LeggiIDViaGUID(OrgID, objParametri_Server)

        Dim EsisteAssociazione As Boolean =
            OrgUsrRead.Test(Username, IDOrganization, objParametri_Server)

        If Not EsisteAssociazione Then

            Dim IDAccount As Integer =
                UsrRead.LeggiIDAccountDaUsername(Username, objParametri_Server)


            If IDAccount >= 0 Then

                Dim righeInserite As String = "[{ ""Username"": """ & Username & """,  ""IDAccount"": """ & IDAccount & """, ""IDOrganization"": " & IDOrganization & "}]"
                OrgUsrWrite.Aggiorna_jDeereDataModelBIZ_AccountXOrganization(righeInserite, "[]", "[]", objParametri_Server)

            End If

        End If


        'queste istruzioni vengono eseguite se non va in eccezione la scrittura
        Dim rval As New RispostaStandard
        rval.RispostaOK = True

        Return rval


    End Function

    Private Sub leggiListaOrganizzazioniDispoScriviDBAccodaArrList(
            ByVal jdR As AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Organization_R,
            ByVal org As jDeereDataModel_Organization,
            ByRef Inseriti As List(Of jDeereDataModel_Organization),
            ByRef modificati As List(Of jDeereDataModel_Organization),
            ByVal objParametri_Server As AgronicaCoreParametri
    )

        Dim curjDeereDataModelBIZ_Organization As Boolean = jdR.TestGUID(org.id, objParametri_Server)
        If curjDeereDataModelBIZ_Organization Then
            modificati.Add(org)
        Else
            Inseriti.Add(org)
        End If

    End Sub

#End Region

#Region "Lettura Appezzamenti, Campi JD e sincro"

    Public Function LeggiAppezzamentiSincronizzatiConJD(orgID As Integer, piva As String, sa_cod As Integer, objParametri_server As AgronicaCoreParametri) As RispostaStandard

        Dim rval As New RispostaStandard
        Try

            Dim letturaOrganizzazioni As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Organization_R
            Dim orgIDGias As Integer = letturaOrganizzazioni.LeggiIDViaGUID(orgID, objParametri_server)

            Dim letturaAppezza As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Field_R
            Dim dt As DataTable =
                letturaAppezza.LeggiConSincronizzazioneAppezzamenti(orgIDGias, piva, sa_cod, 0, "", "", "", "", "", "", objParametri_server)

            AppezzamentiLettiElaboraTabella(dt)

            rval.RispostaOK = True
            rval.RispostaStringa = LeggiAppezzamentiSincronizzatiConJDKendo(dt)

        Catch ex As Exception

            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return rval

    End Function

    Private Sub AppezzamentiLettiElaboraTabella(dt As DataTable)

        dt.Columns.Add(New DataColumn("StatoSincronizzazione", GetType(String)))
        dt.Columns.Add(New DataColumn("StatoSincronizzazione_COD", GetType(Integer)))

        For Each row As DataRow In dt.Rows
            If row("app_nome").ToString() <> "" AndAlso row("val_cod").ToString() <> "" Then
                row("StatoSincronizzazione") = "Sincronizzato"
                row("StatoSincronizzazione_COD") = enum_StatoSincronizzazioneJohnDeere.Sincronizzato
            Else
                If row("val_cod").ToString() <> "" Then
                    If row("chiave").ToString().StartsWith("-0-AGR-") Then
                        row("StatoSincronizzazione") = "Campo John Deere in GIAS non sincronizzato"
                        row("StatoSincronizzazione_COD") = enum_StatoSincronizzazioneJohnDeere.CampoJohnDeereinGIASnonSincronizzato
                    Else
                        row("StatoSincronizzazione") = "Solo in John Deere"
                        row("StatoSincronizzazione_COD") = enum_StatoSincronizzazioneJohnDeere.SoloInJohnDeere
                    End If
                Else
                    row("StatoSincronizzazione") = "Appezzamento GIAS non sincronizzato "
                    row("StatoSincronizzazione_COD") = enum_StatoSincronizzazioneJohnDeere.AppezzamentoGIASNonSincronizzato
                End If

            End If
        Next

    End Sub

    Private Function LeggiAppezzamentiSincronizzatiConJDKendo(dt As DataTable) As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        l.Add(New ColonneNome("chiave", "CodiceJD", "string") With {._hidden = True})
        l.Add(New ColonneNome("StatoSincronizzazione", "Stato Sincronizzazione", "string"))
        l.Add(New ColonneNome("StatoSincronizzazione_COD", "Stato Sincronizzazione COD", "number") With {._hidden = True})
        l.Add(New ColonneNome("ConfiniPresenti", "Confini", "number") With {._FormatoParticolare = "<span class='confini fa fa-globe fa-2x'></span>"})
        l.Add(New ColonneNome("val_cod", "Campo John Deere", "string"))
        l.Add(New ColonneNome("BoundaryName", "Confini John Deere", "string"))

        l.Add(New ColonneNome("ClientName", "Cliente John Deere", "string"))
        l.Add(New ColonneNome("ClientID", "ClientID", "number") With {._hidden = True})

        l.Add(New ColonneNome("FarmName", "Az. agr. John Deere", "string"))
        l.Add(New ColonneNome("FarmID", "FarmID", "number") With {._hidden = True})

        l.Add(New ColonneNome("App_Nome", "Appezzamento GIAS", "string"))

        Dim c As New ColonneNome("sup_app", Gias.Superficie, "number")
        c._formatNr = "n4"
        l.Add(c)

        l.Add(New ColonneNome("Data_Modifica", "Data ultima modifica", "date"))

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable_Kendo(dt, l)
        Return risp

    End Function


    Private Function LeggiOperationsSincronizzatiConJDKendo(dt As DataTable) As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        'chiavi ed info operation
        l.Add(New ColonneNome("chiave", "CodiceJD", "string") With {._hidden = True})
        l.Add(New ColonneNome("StatoSincronizzazione", "Stato Sincronizzazione", "string"))
        l.Add(New ColonneNome("StatoSincronizzazione_COD", "Stato Sincronizzazione COD", "number") With {._hidden = True})

        'dati JD
        l.Add(New ColonneNome("fieldOperationType", "Tipo Operazione", "string"))
        l.Add(New ColonneNome("startDate", "Data/Ora Inizio", "string"))
        l.Add(New ColonneNome("endDate", "Data/Ora Fine", "string"))

        'Dati Gias
        l.Add(New ColonneNome("DescrizioneGias", "Lavorazione Gias", "string"))


        'info "field" ed appezzamento
        l.Add(New ColonneNome("ConfiniPresenti", "Confini", "number") With {._FormatoParticolare = "<span class='confini fa fa-globe fa-2x'></span>"})
        l.Add(New ColonneNome("FieldName", "Campo John Deere", "string"))
        l.Add(New ColonneNome("BoundaryName", "Confini John Deere", "string"))

        l.Add(New ColonneNome("ClientName", "Cliente John Deere", "string"))
        l.Add(New ColonneNome("ClientID", "ClientID", "number") With {._hidden = True})

        l.Add(New ColonneNome("FarmName", "Az. agr. John Deere", "string"))
        l.Add(New ColonneNome("FarmID", "FarmID", "number") With {._hidden = True})

        l.Add(New ColonneNome("App_Nome", "Appezzamento GIAS", "string"))

        'altre info...
        l.Add(New ColonneNome("Data_Modifica", "Data ultima modifica", "date"))
        l.Add(New ColonneNome("allegatoCollegato_Allegati_Documenti_Cod", "allegatoCollegato_Allegati_Documenti_Cod", "number") With {._hidden = True})

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable_Kendo(dt, l)
        Return risp

    End Function
    Private Function LeggiListaFieldsAPI(orgID As String, objParametri_Server As AgronicaCoreParametri, ByVal cfgOauthJDeere As jDeereDataModel_ApiCFG, Optional ByVal StartInterval As Integer = 0, Optional ByVal EndInterval As Integer = 0) As rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_Field))

        Dim rval As New rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_Field))

        Try
            Dim clnts As rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_Client))
            Dim frms As rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_Farm))
            Dim bnds As rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_Boundary))
            Dim rOrg As New jDeereDataModelDAL_Organization_R
            Dim OrganizationID As Integer = rOrg.LeggiIDViaGUID(orgID, objParametri_Server)

            'chiamata api per recupero lista campi
            rval = JDeereGetFieldsList(cfgOauthJDeere, orgID, StartInterval, EndInterval)
            If rval.RispostaOK Then
                For Each fld In rval.RispostaStringa.values
                    fld.OrganizationID = OrganizationID
                    'elenco clienti collegati
                    clnts = JDeereGetClientsByFieldId(cfgOauthJDeere, orgID, fld.id)
                    If clnts.RispostaOK Then
                        fld.clients = clnts.RispostaStringa
                    Else
                        Throw New Exception(clnts.Errore)
                    End If

                    'elenco farm collegate (centri)
                    frms = JDeereGetFarmsByFieldId(cfgOauthJDeere, orgID, fld.id)
                    If frms.RispostaOK Then
                        fld.farms = frms.RispostaStringa
                    Else
                        Throw New Exception(frms.Errore)
                    End If
                    'elenco boundary per campo
                    bnds = JDeereGetBoundariesByFieldId(cfgOauthJDeere.JDeere, orgID, fld.id)
                    If bnds.RispostaOK Then
                        fld.boundaries = bnds.RispostaStringa
                    Else
                        Throw New Exception(bnds.Errore)
                    End If
                Next
            Else
                Throw New Exception(rval.Errore)
            End If
        Catch ex As Exception
            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return rval
    End Function

    Public Function LeggiListaFieldsDispo(cfgImport As ConfigurazioneImportazione, orgID As String, objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, ByVal cfgOauthJDeere As jDeereDataModel_ApiCFG) As RispostaStandard
        Dim rsVal As New RispostaStandard
        Try
            Dim rvalDB As New RispostaStandard

            'se richiesta la lettura da api
            If cfgOauthJDeere.JDeereWebHookCFG.RefreshField OrElse cfgOauthJDeere.ForcedReFresh Then

                Dim flds As rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_Field))
                Dim fldCount As Integer = 0
                Dim fldNumCall As Integer = 1
                Dim cont As Integer
                Dim ind As Integer = 1 'for debug
                Dim startTime As DateTime = DateTime.Now
                Dim endTime As DateTime

                'step 1 - recupero il totale dei campi della organization
                ' di default JD pagina a 10 elementi per volta con un massimo di 100 elementi per pagina.

                Dim rCount = JDeereGetFieldsListCount(cfgOauthJDeere, orgID)
                If Not rCount.RispostaOK Then
                    Throw New Exception(rCount.Errore)
                Else
                    fldCount = rCount.RispostaStringa.total
                    Dim resto As Integer = (fldCount Mod 100)
                    fldNumCall = Math.Floor(fldCount / 100)
                    If resto > 0 Then
                        fldNumCall += 1
                    End If
                End If

                For cont = 0 To fldNumCall - 1

                    flds = LeggiListaFieldsAPI(orgID, objParametri_Server, cfgOauthJDeere, (100 * cont), (100 * (cont + 1)))

                    If flds.RispostaOK Then
                        rvalDB = leggiListaFieldsDispoScriviDB(orgID, flds.RispostaStringa, objParametri_Server)
                    Else
                        Throw New Exception(flds.Errore)
                    End If
                Next
                If rvalDB.RispostaOK Then
                    Dim rvalRiportoBoundaryToGIS As RispostaStandard = RiportoBoundaryJDToGIAS(cfgImport, "", objParametri_Server)
                End If
                endTime = DateTime.Now
                cfgOauthJDeere.JDeereWebHookCFG.RefreshField = False
            End If
            rsVal.RispostaOK = True
            rsVal.RispostaStringa = ""

        Catch ex As Exception
            rsVal.RispostaOK = False
            rsVal.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return rsVal
    End Function

    Private Function leggiListaFieldsDispoScriviDB(orgID As Integer, flds As jDeereDataModel_GenericList(Of jDeereDataModel_Field), objParametri_Server As AgronicaCoreParametri, Optional ByVal RiportaAssociazioniMancanti As Boolean = False) As RispostaStandard

        Dim rval As New RispostaStandard

        Try
            Dim jOrgR As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Organization_R
            Dim jdRC As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Client_R
            Dim jdRF As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Farm_R
            Dim jdR As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Field_R
            Dim jdRB As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Boundary_R
            Dim jdRBF As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_FieldXBoundary_R

            orgID = jOrgR.LeggiIDViaGUID(orgID, objParametri_Server)

            Dim cInseriti As New List(Of jDeereDataModel_Client)
            Dim cModificati As New List(Of jDeereDataModel_Client)
            Dim fInseriti As New List(Of jDeereDataModel_Farm)
            Dim fModificati As New List(Of jDeereDataModel_Farm)
            Dim bInseriti As New List(Of jDeereDataModel_Boundary)
            Dim bModificati As New List(Of jDeereDataModel_Boundary)

            For Each fld In flds.values
                fld.OrganizationID = orgID
                For Each cli In fld.clients.values
                    cli.OrganizationID = orgID
                    leggiListaClientsDispoScriviDBAccodaArrList(jdRC, cli, cInseriti, cModificati, objParametri_Server)
                Next
                For Each frm In fld.farms.values
                    frm.OrganizationID = orgID
                    leggiListaFarmsDispoScriviDBAccodaArrList(jdRF, frm, fInseriti, fModificati, objParametri_Server)
                Next
                For Each bnd In fld.boundaries.values
                    bnd.OrganizationID = orgID
                    leggiListaBoundaryDispoScriviDBAccodaArrList(jdRB, bnd, bInseriti, bModificati, objParametri_Server)
                Next
            Next

            Dim scInseriti As String = JsonConvert.SerializeObject(cInseriti)
            Dim scModificati As String = JsonConvert.SerializeObject(cModificati)
            Dim sfInseriti As String = JsonConvert.SerializeObject(fInseriti)
            Dim sfModificati As String = JsonConvert.SerializeObject(fModificati)
            Dim sbInseriti As String = JsonConvert.SerializeObject(bInseriti)
            Dim sbModificati As String = JsonConvert.SerializeObject(bModificati)

            scInseriti = scInseriti.Replace("""id""", """ID""")
            scInseriti = scInseriti.Replace("""name""", """Name""")
            sfInseriti = sfInseriti.Replace("""id""", """ID""")
            sfInseriti = sfInseriti.Replace("""name""", """Name""")
            sbInseriti = sbInseriti.Replace("""id""", """ID""")
            sbInseriti = sbInseriti.Replace("""name""", """Name""")


            scModificati = scModificati.Replace("""id""", """ID""")
            scModificati = scModificati.Replace("""name""", """Name""")
            sfModificati = sfModificati.Replace("""id""", """ID""")
            sfModificati = sfModificati.Replace("""name""", """Name""")
            sbModificati = sbModificati.Replace("""id""", """ID""")
            sbModificati = sbModificati.Replace("""name""", """Name""")

            Dim bizC As New AgronicaCoreAnagrafeBIZ.jDeereDataModelBIZ_Client_W
            Dim bizF As New AgronicaCoreAnagrafeBIZ.jDeereDataModelBIZ_Farm_W
            Dim bizB As New AgronicaCoreAnagrafeBIZ.jDeereDataModelBIZ_Boundary_W

            Dim rvalScritturaC As String =
                bizC.Aggiorna_jDeereDataModel_Client(scInseriti, scModificati, "[]", objParametri_Server)

            Dim rvalScritturaF As String =
                bizF.Aggiorna_jDeereDataModel_Farm(sfInseriti, sfModificati, "[]", objParametri_Server)

            Dim rvalScritturaB As String =
                bizB.Aggiorna_jDeereDataModel_Boundary(sbInseriti, sbModificati, "[]", objParametri_Server)

            Dim Inseriti As New List(Of jDeereDataModel_Field)
            Dim modificati As New List(Of jDeereDataModel_Field)

            For Each fld In flds.values
                leggiListaFieldsDispoScriviDBAccodaArrList(jdR, fld, Inseriti, modificati, objParametri_Server)
            Next

            Dim sInseriti As String = JsonConvert.SerializeObject(Inseriti)
            Dim sModificati As String = JsonConvert.SerializeObject(modificati)

            sModificati = sModificati.Replace("""id""", """ID""")
            sModificati = sModificati.Replace("""name""", """Name""")

            sInseriti = sInseriti.Replace("""id""", """ID""")
            sInseriti = sInseriti.Replace("""name""", """Name""")

            Dim biz As New AgronicaCoreAnagrafeBIZ.jDeereDataModelBIZ_Field_W

            Dim rvalScrittura As String =
                biz.Aggiorna_jDeereDataModel_Field(sInseriti, sModificati, "[]", objParametri_Server)

            'scrittura relazione fieldXboundary
            For Each fld In flds.values
                For Each bnd In fld.boundaries.values
                    leggiListaFieldXBoundaryScriviDB(fld.id, bnd.id, objParametri_Server)
                Next
            Next


            If RiportaAssociazioniMancanti Then

                Dim seqTab As New Agro_Sequenze

                Dim oRiportaAssociazioniMancanti As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_EntitaGIAS_W
                If Inseriti.Count > 0 Then
                    oRiportaAssociazioniMancanti.RiportaAssociazioniMancanti(enum_TipoEntitaJohnDeere.Field, "", objParametri_Server)
                    seqTab.AllineaUltimoValoreDataTabella("jDeereDataModel_EntitaGIAS", "jDeereDataModel_EntitaGIAS_COD", objParametri_Server)
                End If

                If fInseriti.Count > 0 Then
                    oRiportaAssociazioniMancanti.RiportaAssociazioniMancanti(enum_TipoEntitaJohnDeere.Farm, "", objParametri_Server)
                    seqTab.AllineaUltimoValoreDataTabella("jDeereDataModel_EntitaGIAS", "jDeereDataModel_EntitaGIAS_COD", objParametri_Server)
                End If

                If cInseriti.Count > 0 Then
                    oRiportaAssociazioniMancanti.RiportaAssociazioniMancanti(enum_TipoEntitaJohnDeere.Client, "", objParametri_Server)
                    seqTab.AllineaUltimoValoreDataTabella("jDeereDataModel_EntitaGIAS", "jDeereDataModel_EntitaGIAS_COD", objParametri_Server)
                End If

                If bInseriti.Count > 0 Then
                    oRiportaAssociazioniMancanti.RiportaAssociazioniMancanti(enum_TipoEntitaJohnDeere.Boundary, "", objParametri_Server)
                    seqTab.AllineaUltimoValoreDataTabella("jDeereDataModel_EntitaGIAS", "jDeereDataModel_EntitaGIAS_COD", objParametri_Server)
                End If

            End If

            rval.RispostaOK = True
            rval.RispostaStringa = ""
        Catch ex As Exception

            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return rval
    End Function

    Private Function AggiornaListaBoundaryScriviDB(fld As jDeereDataModel_Field, objParametri_Server As AgronicaCoreParametri, Optional ByVal RiportaAssociazioniMancanti As Boolean = False) As RispostaStandard
        Dim rval As New RispostaStandard
        Try
            Dim jdRB As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Boundary_R

            Dim bInseriti As New List(Of jDeereDataModel_Boundary)
            Dim bModificati As New List(Of jDeereDataModel_Boundary)

            For Each bnd In fld.boundaries.values
                leggiListaBoundaryDispoScriviDBAccodaArrList(jdRB, bnd, bInseriti, bModificati, objParametri_Server)
            Next

            Dim sbInseriti As String = JsonConvert.SerializeObject(bInseriti)
            Dim sbModificati As String = JsonConvert.SerializeObject(bModificati)

            sbInseriti = sbInseriti.Replace("""id""", """ID""")
            sbInseriti = sbInseriti.Replace("""name""", """Name""")

            sbModificati = sbModificati.Replace("""id""", """ID""")
            sbModificati = sbModificati.Replace("""name""", """Name""")

            Dim bizB As New AgronicaCoreAnagrafeBIZ.jDeereDataModelBIZ_Boundary_W

            Dim rvalScritturaB As String =
                bizB.Aggiorna_jDeereDataModel_Boundary(sbInseriti, sbModificati, "[]", objParametri_Server)


            rval.RispostaOK = True
            rval.RispostaStringa = ""
        Catch ex As Exception

            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return rval
    End Function



    Public Function InvioFieldJDeereDispoScriviDB(fld As jDeereDataModel_Field, objParametri_Server As AgronicaCoreParametri, Optional ByVal RiportaAssociazioniMancanti As Boolean = False) As RispostaStandard
        Dim rval As New RispostaStandard
        Try
            Dim jdRC As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Client_R
            Dim jdRF As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Farm_R
            Dim jdR As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Field_R
            Dim jdRB As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Boundary_R
            Dim jdRBF As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_FieldXBoundary_R

            Dim cInseriti As New List(Of jDeereDataModel_Client)
            Dim cModificati As New List(Of jDeereDataModel_Client)
            Dim fInseriti As New List(Of jDeereDataModel_Farm)
            Dim fModificati As New List(Of jDeereDataModel_Farm)
            Dim bInseriti As New List(Of jDeereDataModel_Boundary)
            Dim bModificati As New List(Of jDeereDataModel_Boundary)


            For Each cli In fld.clients.values
                leggiListaClientsDispoScriviDBAccodaArrList(jdRC, cli, cInseriti, cModificati, objParametri_Server)
            Next
            For Each frm In fld.farms.values
                leggiListaFarmsDispoScriviDBAccodaArrList(jdRF, frm, fInseriti, fModificati, objParametri_Server)
            Next
            For Each bnd In fld.boundaries.values
                leggiListaBoundaryDispoScriviDBAccodaArrList(jdRB, bnd, bInseriti, bModificati, objParametri_Server)
            Next

            Dim scInseriti As String = JsonConvert.SerializeObject(cInseriti)
            Dim scModificati As String = JsonConvert.SerializeObject(cModificati)
            Dim sfInseriti As String = JsonConvert.SerializeObject(fInseriti)
            Dim sfModificati As String = JsonConvert.SerializeObject(fModificati)
            Dim sbInseriti As String = JsonConvert.SerializeObject(bInseriti)
            Dim sbModificati As String = JsonConvert.SerializeObject(bModificati)

            scInseriti = scInseriti.Replace("""id""", """ID""")
            scInseriti = scInseriti.Replace("""name""", """Name""")
            sfInseriti = sfInseriti.Replace("""id""", """ID""")
            sfInseriti = sfInseriti.Replace("""name""", """Name""")
            sbInseriti = sbInseriti.Replace("""id""", """ID""")
            sbInseriti = sbInseriti.Replace("""name""", """Name""")


            scModificati = scModificati.Replace("""id""", """ID""")
            scModificati = scModificati.Replace("""name""", """Name""")
            sfModificati = sfModificati.Replace("""id""", """ID""")
            sfModificati = sfModificati.Replace("""name""", """Name""")
            sbModificati = sbModificati.Replace("""id""", """ID""")
            sbModificati = sbModificati.Replace("""name""", """Name""")

            Dim bizC As New AgronicaCoreAnagrafeBIZ.jDeereDataModelBIZ_Client_W
            Dim bizF As New AgronicaCoreAnagrafeBIZ.jDeereDataModelBIZ_Farm_W
            Dim bizB As New AgronicaCoreAnagrafeBIZ.jDeereDataModelBIZ_Boundary_W

            Dim rvalScritturaC As String =
                bizC.Aggiorna_jDeereDataModel_Client(scInseriti, scModificati, "[]", objParametri_Server)

            Dim rvalScritturaF As String =
                bizF.Aggiorna_jDeereDataModel_Farm(sfInseriti, sfModificati, "[]", objParametri_Server)

            Dim rvalScritturaB As String =
                bizB.Aggiorna_jDeereDataModel_Boundary(sbInseriti, sbModificati, "[]", objParametri_Server)

            Dim Inseriti As New List(Of jDeereDataModel_Field)
            Dim modificati As New List(Of jDeereDataModel_Field)

            leggiListaFieldsDispoScriviDBAccodaArrList(jdR, fld, Inseriti, modificati, objParametri_Server)


            Dim sInseriti As String = JsonConvert.SerializeObject(Inseriti)
            Dim sModificati As String = JsonConvert.SerializeObject(modificati)

            sModificati = sModificati.Replace("""id""", """ID""")
            sModificati = sModificati.Replace("""name""", """Name""")

            sInseriti = sInseriti.Replace("""id""", """ID""")
            sInseriti = sInseriti.Replace("""name""", """Name""")

            Dim biz As New AgronicaCoreAnagrafeBIZ.jDeereDataModelBIZ_Field_W

            Dim rvalScrittura As String =
                biz.Aggiorna_jDeereDataModel_Field(sInseriti, sModificati, "[]", objParametri_Server)

            'scrittura relazione fieldXboundary
            For Each bnd In fld.boundaries.values
                leggiListaFieldXBoundaryScriviDB(fld.id, bnd.id, objParametri_Server)
            Next
            rval.RispostaOK = True
            rval.RispostaStringa = ""
        Catch ex As Exception

            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return rval

    End Function

    Public Function UploadFileJDeereScriviDB(file As jDeereDataModel_File, objParametri_Server As AgronicaCoreParametri, Optional ByVal RiportaAssociazioniMancanti As Boolean = False) As RispostaStandard
        Dim rval As New RispostaStandard
        Try

            Dim jdR As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Files_R


            Dim Inseriti As New List(Of jDeereDataModel_File)
            Dim Modificati As New List(Of jDeereDataModel_File)

            leggiListaFilesScriviDBArrList(jdR, file, Inseriti, Modificati, objParametri_Server)


            Dim sInseriti As String = JsonConvert.SerializeObject(Inseriti)
            Dim sModificati As String = JsonConvert.SerializeObject(Modificati)

            sModificati = sModificati.Replace("""id""", """ID""")
            sModificati = sModificati.Replace("""name""", """Name""")

            sInseriti = sInseriti.Replace("""id""", """ID""")
            sInseriti = sInseriti.Replace("""name""", """Name""")

            Dim biz As New AgronicaCoreAnagrafeBIZ.jDeereDataModelBIZ_Files_W

            Dim rvalScrittura As String =
                biz.Aggiorna_jDeereDataModel_Files(sInseriti, sModificati, "[]", objParametri_Server)

            If rvalScrittura <> "" Then
                Throw New Exception(rvalScrittura)
            End If

            rval.RispostaOK = True
            rval.RispostaStringa = ""
        Catch ex As Exception

            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return rval

    End Function

    Private Sub leggiListaFilesScriviDBArrList(
            ByVal jdR As AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Files_R,
            ByVal file As jDeereDataModel_File,
            ByRef Inseriti As List(Of jDeereDataModel_File),
            ByRef modificati As List(Of jDeereDataModel_File),
            ByVal objParametri_Server As AgronicaCoreParametri
    )

        Dim curjDeereDataModel_File As Boolean
        If file.guid Is Nothing OrElse file.guid = "" Then
            'se ho il campo guid a nothing\blank allora provengo dalla funzione di lettura lista campi da JD
            curjDeereDataModel_File = jdR.TestGUID(file.id, objParametri_Server)
        Else
            'se il campo guid è diverso da nothing/blank allora è un oggetto che proviene dal GIS oppure è già esistente nel DB da una precedente sincro,
            'in questo caso il campo ID contiene il campo ID della tabella
            Dim idAsInteger As Integer = 0

            If Integer.TryParse(file.id, idAsInteger) = True Then
                curjDeereDataModel_File = jdR.Test(idAsInteger, objParametri_Server)
            Else
                'impossibile convertire il campo ID da String a INT -> errore (come trappare?)
                'per ora non lo registro
                If file.guid <> "" Then
                    curjDeereDataModel_File = jdR.TestGUID(file.guid, objParametri_Server)
                End If
            End If
        End If

        If curjDeereDataModel_File Then
            modificati.Add(file)
        Else
            Inseriti.Add(file)
        End If

    End Sub

    Private Sub leggiListaFieldsDispoScriviDBAccodaArrList(
            ByVal jdR As AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Field_R,
            ByVal fld As jDeereDataModel_Field,
            ByRef Inseriti As List(Of jDeereDataModel_Field),
            ByRef modificati As List(Of jDeereDataModel_Field),
            ByVal objParametri_Server As AgronicaCoreParametri
    )

        Dim curjDeereDataModel_Field As Boolean
        If fld.guid Is Nothing OrElse fld.guid = "" Then
            'se ho il campo guid a nothing\blank allora provengo dalla funzione di lettura lista campi da JD
            curjDeereDataModel_Field = jdR.TestGUID(fld.id, objParametri_Server)
        Else
            'se il campo guid è diverso da nothing/blank allora è un oggetto che proviene dal GIS oppure è già esistente nel DB da una precedente sincro,
            'in questo caso il campo ID contiene il campo ID della tabella
            Dim idAsInteger As Integer = 0
            If Integer.TryParse(fld.id, idAsInteger) = True Then
                curjDeereDataModel_Field = jdR.Test(idAsInteger, objParametri_Server)
            Else
                'impossibile convertire il campo ID da String a INT -> errore (come trappare?)
                'per ora non lo registro
                Return
            End If
        End If

        If curjDeereDataModel_Field Then
            modificati.Add(fld)
        Else
            Inseriti.Add(fld)
        End If

    End Sub

    Private Sub leggiListaClientsDispoScriviDBAccodaArrList(
            ByVal jdR As AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Client_R,
            ByVal cli As jDeereDataModel_Client,
            ByRef Inseriti As List(Of jDeereDataModel_Client),
            ByRef modificati As List(Of jDeereDataModel_Client),
            ByVal objParametri_Server As AgronicaCoreParametri
    )

        Dim curjDeereDataModel_Client As Boolean = jdR.TestGUID(cli.id, objParametri_Server)
        If curjDeereDataModel_Client Then
            If Not modificati.Exists(Function(x) x.id = cli.id) Then
                modificati.Add(cli)
            End If
        Else
            'se recupero l'elenco dei clients dai fields potrei avere il record ripetuto n volte
            If Not Inseriti.Exists(Function(x) x.id = cli.id) Then
                Inseriti.Add(cli)
            End If
        End If

    End Sub

    Private Sub leggiListaFarmsDispoScriviDBAccodaArrList(
            ByVal jdR As AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Farm_R,
            ByVal frm As jDeereDataModel_Farm,
            ByRef Inseriti As List(Of jDeereDataModel_Farm),
            ByRef modificati As List(Of jDeereDataModel_Farm),
            ByVal objParametri_Server As AgronicaCoreParametri
    )

        Dim curjDeereDataModel_Farm As Boolean = jdR.TestGUID(frm.id, objParametri_Server)
        If curjDeereDataModel_Farm Then
            If Not modificati.Exists(Function(x) x.id = frm.id) Then
                modificati.Add(frm)
            End If
        Else
            'se recupero l'elenco dei clients dai fields potrei avere il record ripetuto n volte
            If Not Inseriti.Exists(Function(x) x.id = frm.id) Then
                Inseriti.Add(frm)
            End If
        End If

    End Sub

    Private Sub leggiListaBoundaryDispoScriviDBAccodaArrList(
            ByVal jdR As AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Boundary_R,
            ByVal bnd As jDeereDataModel_Boundary,
            ByRef Inseriti As List(Of jDeereDataModel_Boundary),
            ByRef modificati As List(Of jDeereDataModel_Boundary),
            ByVal objParametri_Server As AgronicaCoreParametri
    )

        Dim curjDeereDataModel_Boundary As Boolean
        If bnd.guid Is Nothing OrElse bnd.guid = "" Then
            'se ho il campo guid a nothing\blank allora provengo dalla funzione di lettura lista campi da JD
            curjDeereDataModel_Boundary = jdR.TestGUID(bnd.id, objParametri_Server)
        Else
            'se il campo guid è diverso da nothing/blank allora è un oggetto che proviene dal GIS oppure è già esistente nel DB da una precedente sincro,
            'in questo caso il campo ID contiene il campo ID della tabella
            Dim idAsInteger As Integer = 0
            If Integer.TryParse(bnd.id, idAsInteger) = True Then
                curjDeereDataModel_Boundary = jdR.Test(idAsInteger, objParametri_Server)
            Else
                'impossibile convertire il campo ID da String a INT -> errore (come trappare?)
                'per ora non lo registro
                Return
            End If
        End If

        If curjDeereDataModel_Boundary Then
            modificati.Add(bnd)
        Else
            Inseriti.Add(bnd)
        End If

        'Dim curjDeereDataModel_Boundary As Boolean = jdR.TestGUID(bnd.id, objParametri_Server)
        'If curjDeereDataModel_Boundary Then
        '    If Not modificati.Exists(Function(x) x.id = bnd.id) Then
        '        modificati.Add(bnd)
        '    End If
        'Else
        '    'se recupero l'elenco dei clients dai fields potrei avere il record ripetuto n volte
        '    If Not Inseriti.Exists(Function(x) x.id = bnd.id) Then
        '        Inseriti.Add(bnd)
        '    End If
        'End If

    End Sub

    Private Function leggiListaFieldXBoundaryScriviDB(guidField As String, guidBoundary As String, objParametri_Server As AgronicaCoreParametri) As RispostaStandard


        Dim FldRead As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Field_R
        Dim BndRead As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Boundary_R
        Dim FldBndRead As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_FieldXBoundary_R
        Dim FldBndWrite As New AgronicaCoreAnagrafeBIZ.jDeereDataModelBIZ_FieldXBoundary_W

        Dim IDField As Integer =
            FldRead.LeggiIDViaGUID(guidField, objParametri_Server)

        'nel caso di aggiornamento arrivo direttamente con l'IDJD
        If IDField = -1 Then
            IDField = guidField
        End If

        Dim IDBoundary As Integer =
            BndRead.LeggiIDViaGUID(guidBoundary, objParametri_Server)

        'nel caso di aggiornamento arrivo direttamente con l'IDJD
        If IDBoundary = -1 Then
            IDBoundary = guidBoundary
        End If

        Dim EsisteAssociazione As Boolean =
            FldBndRead.Test(IDField, IDBoundary, objParametri_Server)

        If Not EsisteAssociazione Then
            Dim righeInserite As String = "[{ ""IDField"": """ & IDField & """,  ""IDBoundary"": """ & IDBoundary & """ }]"
            FldBndWrite.Aggiorna_jDeereDataModelBIZ_FieldXBoundary(righeInserite, "[]", "[]", objParametri_Server)
        End If

        'queste istruzioni vengono eseguite se non va in eccezione la scrittura
        Dim rval As New RispostaStandard
        rval.RispostaOK = True

        Return rval


    End Function


#End Region
    Public Function AccodaOperationsSelezionePerSincronizzazioneInGIAS(OrgID As String, cfgImport As ConfigurazioneImportazione, ListaChiavi As String, objParametri_server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, cfgOauthJDeere As jDeereDataModel_ApiCFG) As RispostaStandard

        Dim rval As New RispostaStandard

        If ListaChiavi = "[]" OrElse String.IsNullOrEmpty(ListaChiavi) Then
            rval.RispostaOK = True
            rval.RispostaStringa = "Nessun Elemento Selezionato"
            Return rval
        End If

        Try
            If cfgOauthJDeere IsNot Nothing Then

                ' Riporta i dati dalle strutture John Deere su GIAS
                rval = AccodaOperationsSelezionePerSincronizzazioneInGiasLeggiDatiJDScriviGias(cfgImport, ListaChiavi, objParametri_server, objParametri_Utenti)

            End If

        Catch ex As Exception

            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return rval


    End Function

    ''' <summary>
    ''' Procedura di sincronizzazione: Riporta i dati sulle strutture GIAS su John Deere, Carica i dati GIAS su John Deere, Scarica nuovi da API Dati john Deere su GIAS, Riporta i dati sulle strutture John Deere su GIAS
    ''' </summary>
    ''' <returns></returns>
    Public Function AccodaSelezionePerSincronizzazioneInGIAS(OrgID As String, cfgImport As ConfigurazioneImportazione, ListaChiavi As String, objParametri_server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, cfgOauthJDeere As jDeereDataModel_ApiCFG) As RispostaStandard

        Dim rval As New RispostaStandard

        If ListaChiavi = "[]" OrElse String.IsNullOrEmpty(ListaChiavi) Then
            rval.RispostaOK = True
            rval.RispostaStringa = "Nessun Elemento Selezionato"
            Return rval
        End If

        Try
            If cfgOauthJDeere IsNot Nothing Then
                If cfgOauthJDeere.JDeere.AuthToken <> "" Then

                    '1. Riporta i dati dalle strutture GIAS sulle tabelle John Deere + Carica i dati GIAS su John Deere
                    Dim rvalCarica As RispostaStandard =
                        AccodaSelezionePerSincronizzazioneCaricaDati(OrgID, cfgImport, ListaChiavi, objParametri_server)



                    '1.b sincro con JD
                    Dim sincro As New jDeereController
                    Dim rValSinc As New RispostaStandard

                    rValSinc = sincro.EseguiInvioOggettiAJDeere(OrgID, ListaChiavi, cfgOauthJDeere, objParametri_server)

                    If rValSinc.RispostaOK Then

                            '2. Riporta i dati dalle strutture John Deere su GIAS
                            Dim rvalLeggi As RispostaStandard =
                        AccodaSelezionePerSincronizzazioneInGiasLeggiDatiJDScriviGias(cfgImport, ListaChiavi, objParametri_server, objParametri_Utenti)

                            rval.RispostaOK = (rvalCarica.RispostaOK AndAlso rvalLeggi.RispostaOK)
                            rval.RispostaStringa = ""
                        Else
                            Throw New Exception(rValSinc.Errore)
                        End If
                    Else
                        rval.RispostaOK = False
                    rval.Errore = "JDeere Login - Token Autorizzazione non valido. Eseguire Login"
                End If
            End If

        Catch ex As Exception

            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return rval

    End Function

    ''' <summary>
    ''' Legge i dati dalle tabelle di cache e li carica in JD usando le API
    ''' </summary>
    ''' <param name="cfgImport"></param>
    ''' <param name="ListaChiavi"></param>
    ''' <param name="objParametri_server"></param>
    ''' <returns></returns>
    Private Function AccodaSelezionePerSincronizzazioneCaricaDati(orgID As String, cfgImport As ConfigurazioneImportazione, ListaChiavi As String, objParametri_server As AgronicaCoreParametri) As RispostaStandard

        Dim rval As New RispostaStandard
        rval.RispostaOK = True

        'da caricare:

        'campi (comprensivi di farm, client, boundary, se lasciati vuoti vengono creati)
        Dim ListaChiaviGIAS As String = ListaChiaviEstraiCampiJD(ListaChiavi, enum_TipoChiaveJohnDeere.ChiaveGias)

        Dim letturaDatiJD As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_EntitaGias_R
        Dim jSonAppezzaFieldsDaRiportare As String =
            letturaDatiJD.ListaAppezzamentiGiasNonImportataInJDViaJsonPath(ListaChiaviGIAS, "", "", objParametri_server)

        Dim flds As New jDeereDataModel_GenericList(Of jDeereDataModel_Field)
        flds.values =
            JsonConvert.DeserializeObject(Of List(Of jDeereDataModel_Field))(jSonAppezzaFieldsDaRiportare)
        flds.total = flds.values.Count

        Dim rOrg As New jDeereDataModelDAL_Organization_R
        Dim OrganizationID As Integer = rOrg.LeggiIDViaGUID(orgID, objParametri_server)

        'su ciascun field carico i boundary.
        For Each ff In flds.values
            ff.OrganizationID = OrganizationID
            Dim chiave As String = "[""" & ff.id.Replace("AGR-", "") & """]"
            Dim sJsonBoundary As RispostaStandard =
                AccodaSelezionePerSincronizzazioneCaricaDatiBoundaryDaGISAgronicaLeggi(cfgImport, chiave, objParametri_server)

            If sJsonBoundary.RispostaStringa IsNot Nothing Then
                Dim bnd As jDeereDataModel_GenericList(Of jDeereDataModel_Boundary) =
                JsonConvert.DeserializeObject(Of jDeereDataModel_GenericList(Of jDeereDataModel_Boundary))(sJsonBoundary.RispostaStringa)

                If bnd Is Nothing Then
                    ff.boundaries = New jDeereDataModel_GenericList(Of jDeereDataModel_Boundary)
                Else
                    ff.boundaries = bnd
                End If
            End If
            ff.clients = New jDeereDataModel_GenericList(Of jDeereDataModel_Client)
            ff.clients.values = New List(Of jDeereDataModel_Client)
            ff.clients.values.Add(New jDeereDataModel_Client With {.id = ff.farms.values.First.id, .name = ff.farms.values.First.name})
        Next

        leggiListaFieldsDispoScriviDB(orgID, flds, objParametri_server, True)

        Return rval

    End Function




    Private Function AccodaSelezionePerSincronizzazioneCaricaDatiBoundaryDaGISAgronicaLeggi(cfgImport As ConfigurazioneImportazione, ListaChiavi As String, objParametri_server As AgronicaCoreParametri) As RispostaStandard

        Dim vChiavi As String() = JsonConvert.DeserializeObject(Of List(Of String))(ListaChiavi).ToArray()

        'recuperare i dati in stringa XML


        Dim leggiXDoc As New AgronicaCoreGisDAL.GIS_Entita_R
        Dim objXmlToConvert As XDocument =
            leggiXDoc.LeggiXDocumentExportJD(enum_Gis_LayerElementiGrafici_std.APPEZZAMENTI, vChiavi, objParametri_server)

        Dim nsGrafica As XNamespace = "http://www.agronica.it/grafica/"
        Dim nsGml As XNamespace = "http://www.opengis.net/gml"

        Dim xmlToConvert As String
        Dim datiJsonPayload As New RispostaStandard

        If objXmlToConvert IsNot Nothing Then

            Dim lRoot As XElement = objXmlToConvert.Root
            lRoot.Name = nsGrafica + lRoot.Name.LocalName
            lRoot.Add(New XAttribute(XNamespace.Xmlns + "gml", nsGml))

            xmlToConvert = objXmlToConvert.ToString.Replace("xmlns=""""", "")
            Dim xDati As New AgronicaGis2012ToJohnDeere

            datiJsonPayload = xDati.convert(xmlToConvert)
        Else

            xmlToConvert = "<DatiEntita xmlns=""http://www.agronica.it/grafica/""><Entita text="""" info_estese="""" TipoIcona=""""><geodata></geodata></Entita></DatiEntita>"
            datiJsonPayload.RispostaOK = True
        End If


        Return datiJsonPayload

    End Function

    Private Function AccodaSelezionePerSincronizzazioneCaricaDatiBoundaryDaGISAgronica(cfgImport As ConfigurazioneImportazione, ListaChiavi As String, objParametri_server As AgronicaCoreParametri) As RispostaStandard


        Dim rval As New RispostaStandard


        Dim datiJsonPayLoad As RispostaStandard =
            AccodaSelezionePerSincronizzazioneCaricaDatiBoundaryDaGISAgronicaLeggi(cfgImport, ListaChiavi, objParametri_server)

        If datiJsonPayLoad.RispostaOK Then

            'upload dei dati via web service
            Dim rvalSendRequest As RispostaStandard =
                AccodaSelezionePerSincronizzazioneAPICalls(datiJsonPayLoad.RispostaStringa, Nothing)
            If Not rvalSendRequest.RispostaOK Then
                Return rvalSendRequest
            End If

        Else
            rval = datiJsonPayLoad
            Return rval
        End If

        rval.RispostaOK = True
        rval.RispostaStringa = ""

        Return rval

    End Function

    ''' <summary>
    ''' Chiama le funzioni che riportano da tabelle di cache a tabelle GIAS
    ''' </summary>
    ''' <param name="cfgImport"></param>
    ''' <param name="ListaChiavi"></param>
    ''' <param name="objParametri_server"></param>
    ''' <returns></returns>
    Public Function AccodaOperationsSelezionePerSincronizzazioneInGiasLeggiDatiJDScriviGias(cfgImport As ConfigurazioneImportazione, ListaChiavi As String, objParametri_server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri) As RispostaStandard


        Dim ListaChiaviJohnDeere As String = ListaChiaviEstraiCampiJD(ListaChiavi, enum_TipoChiaveJohnDeere.ChiaveJD)

        Dim rvalRiportoField As RispostaStandard =
            RiportoOperationsJDToGIAS(cfgImport, ListaChiaviJohnDeere, objParametri_server, objParametri_Utenti)

        Return rvalRiportoField
    End Function



    ''' <summary>
    ''' Legge i dati dalle API e li riporta sulle tabelle di cache, poi chiama le funzioni che riportano da tabelle di cache a tabelle GIAS
    ''' </summary>
    ''' <param name="cfgImport"></param>
    ''' <param name="ListaChiavi"></param>
    ''' <param name="objParametri_server"></param>
    ''' <returns></returns>
    Public Function AccodaSelezionePerSincronizzazioneInGiasLeggiDatiJDScriviGias(cfgImport As ConfigurazioneImportazione, ListaChiavi As String, objParametri_server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri) As RispostaStandard

        'riportare su database nell'ordine: Farm, Client, Campi, boundary

        Dim rval As New RispostaStandard
        rval.RispostaOK = True

        'Campi ...: 

        Dim ListaChiaviJohnDeere As String = ListaChiaviEstraiCampiJD(ListaChiavi, enum_TipoChiaveJohnDeere.ChiaveJD)

        'riporto su GIAS
        If cfgImport.GestioneRiportoDatiInGias = InterpretaDatiDBF.Tipo_Importazione.Tipo_GestioneRiportoDatiInGias.RiportoAutomaticoDeiDati Then

            Dim rvalRiportoField As RispostaStandard =
                RiportoFieldJDToGIAS(cfgImport, ListaChiaviJohnDeere, objParametri_server, objParametri_Utenti)

            rval.RispostaOK = (rval.RispostaOK AndAlso rvalRiportoField.RispostaOK)

            'Farm

            'Client

            'boundary: associa l'elemento grafico sui dati importati con l'elemento di tipo appezzamento
            AssociaAppezzamentoInGisEntitaDaJoinBoundaryJD(cfgImport, ListaChiaviJohnDeere.Replace("ID", "ff.ID"), objParametri_server)


        End If
        'fine riporto su GIAS

        Return rval

    End Function


    Public Function AccodaSelezionePerSincronizzazioneAPICalls(jSonPayLoad As String, cfgApi As Object) As RispostaStandard

        Dim rval As New RispostaStandard

        Try


            'effettuare la chiamata con la configurazione
            'lorenzo

            rval.RispostaOK = True
            rval.RispostaStringa = ""

        Catch ex As Exception


            rval.RispostaOK = True
            rval.RispostaStringa = ""

        End Try

        Return rval

    End Function

    Public Function EseguiInvioOggettiAJDeere(ByVal orgID As String, ListaChiavi As String, ByVal ConfigOAuth2 As jDeereDataModel_ApiCFG, ByVal objParametriServer As AgronicaCoreParametri) As RispostaStandard
        Dim resp As New RispostaStandard
        Try
            Dim ListaKey As String = ListaChiaviEstraiCampiJD(ListaChiavi, enum_TipoChiaveJohnDeere.ChiaveGias)
            Dim idList = getIDFieldByListaChiavi(ListaKey, objParametriServer)

            If idList.Count > 0 Then

                '1- recupero l'elenco degli oggetti da inviare da GIAS a JDeere (per ora solo Field e Boundary)
                '1.a - Fields
                Dim rField As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Field_R
                Dim rOrg As New jDeereDataModelDAL_Organization_R
                Dim lstFields = rField.LeggiFieldsDaInviareAJDeere(objParametriServer, idList)
                Dim fldReq As New rispostaStandard(Of jDeereDataModel_Field_REQ)
                Dim clnts As rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_Client))
                Dim frms As rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_Farm))
                Dim oFld As New jDeereDataModel_Field
                Dim OrganizationID As Integer = rOrg.LeggiIDViaGUID(orgID, objParametriServer)

                For Each ele In lstFields.Rows
                    fldReq.RispostaStringa = GetJDeereFieldRequest(orgID, ele("ID"), objParametriServer)
                    If fldReq.RispostaStringa.action = jDeereDataModel_RequestType.Insert Then
                        fldReq = JDeereCreateField(ConfigOAuth2, orgID, fldReq.RispostaStringa)
                        If fldReq.RispostaOK Then
                            'Creazione campo su JD ok :
                            'in base ai guid di cliente e farm devo rileggere i corrispettivi dati del campo e recuperare i guid relativi per poi aggiornarli nel db
                            ' NB: AVENDO IN LINEA L'ESTRAZIONE DEI CAMPI DA SINCRONIZZARE, HO GIà A DISPOSIZIONE GLI ID DI CLIENT E FARM
                            'field


                            oFld.id = ele("ID")
                            oFld.guid = fldReq.RispostaStringa.guid
                            oFld.name = fldReq.RispostaStringa.name
                            oFld.archived = fldReq.RispostaStringa.archived
                            oFld.OrganizationID = OrganizationID
                            'elenco clienti collegati
                            clnts = JDeereGetClientsByFieldId(ConfigOAuth2, orgID, oFld.guid)
                            If clnts.RispostaOK Then
                                oFld.clients = clnts.RispostaStringa
                                For Each cli In oFld.clients.values
                                    cli.OrganizationID = oFld.OrganizationID
                                Next
                            Else
                                Throw New Exception(clnts.Errore)
                            End If
                            'elenco farm collegate (centri)
                            frms = JDeereGetFarmsByFieldId(ConfigOAuth2, orgID, oFld.guid)
                            If frms.RispostaOK Then
                                oFld.farms = frms.RispostaStringa
                                For Each farm In oFld.farms.values
                                    farm.OrganizationID = oFld.OrganizationID
                                Next
                            Else
                                Throw New Exception(frms.Errore)
                            End If

                            resp = InvioFieldJDeereDispoScriviDB(oFld, objParametriServer)
                            If Not resp.RispostaOK Then
                                Throw New Exception(resp.Errore)
                            End If

                            resp = ElabBoundaryXField(orgID, ConfigOAuth2, objParametriServer, oFld)
                            If Not resp.RispostaOK Then
                                Throw New Exception(resp.Errore)
                            End If

                            resp = AggiornaListaBoundaryScriviDB(oFld, objParametriServer)
                            If Not resp.RispostaOK Then
                                Throw New Exception(resp.Errore)
                            End If


                        Else
                            'Creazione campo su JD fallita
                            Throw New Exception(fldReq.Errore)
                        End If
                    Else
                        ' field già esistente
                        ' eseguire method update (TO-DO)
                        ' recuperare l'elenco dei boundaries e verificare se i guid esistono
                        oFld.id = ele("ID")
                        oFld.guid = fldReq.RispostaStringa.id
                        oFld.name = fldReq.RispostaStringa.name
                        oFld.archived = fldReq.RispostaStringa.archived
                        oFld.OrganizationID = OrganizationID
                        fldReq = JDeereUpdateField(ConfigOAuth2, orgID, fldReq.RispostaStringa)
                        If fldReq.RispostaOK Then
                            'creo le liste vuote per consistenza della classe padre
                            oFld.clients = New jDeereDataModel_GenericList(Of jDeereDataModel_Client)
                            oFld.farms = New jDeereDataModel_GenericList(Of jDeereDataModel_Farm)
                            oFld.boundaries = New jDeereDataModel_GenericList(Of jDeereDataModel_Boundary)

                            clnts = JDeereGetClientsByFieldId(ConfigOAuth2, orgID, oFld.guid)
                            If clnts.RispostaOK Then
                                oFld.clients = clnts.RispostaStringa
                            Else
                                Throw New Exception(clnts.Errore)
                            End If
                            'elenco farm collegate (centri)
                            frms = JDeereGetFarmsByFieldId(ConfigOAuth2, orgID, oFld.guid)
                            If frms.RispostaOK Then
                                oFld.farms = frms.RispostaStringa
                            Else
                                Throw New Exception(frms.Errore)
                            End If

                            resp = ElabBoundaryXField(orgID, ConfigOAuth2, objParametriServer, oFld)

                            resp = InvioFieldJDeereDispoScriviDB(oFld, objParametriServer)
                            If Not resp.RispostaOK Then
                                Throw New Exception(resp.Errore)
                            End If
                        Else
                            'aggiornamento campo su JD fallita
                            Throw New Exception(fldReq.Errore)
                        End If
                    End If
                Next
            End If
            resp.RispostaOK = True

        Catch ex As Exception
            resp.RispostaOK = False
            resp.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return resp
    End Function


    Private Function getIDFieldByListaChiavi(listachiavi As String, ByVal objParametriServer As AgronicaCoreParametri) As List(Of Integer)
        Dim lista As New List(Of Integer)
        Try
            Dim EGReader As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_EntitaGias_R
            Dim keystr As String = listachiavi
            If listachiavi.Contains("[") Then
                keystr = ListaChiaviEstraiCampiJD(listachiavi, enum_TipoChiaveJohnDeere.ChiaveGias)
            End If

            Dim dt = EGReader.LeggiIDJdDaChiaveGIAS(enum_TipoEntitaJohnDeere.Field, keystr, "", "", objParametriServer)
            For Each row In dt.Rows
                If row("IDJD") <> -1 Then
                    lista.Add(row("IDJD"))
                End If
            Next

        Catch ex As Exception

        End Try
        Return lista
    End Function

    'Private Function ElabBoundaryNOField(ByVal orgID As String, ByVal ConfigOAuth2 As jDeereDataModel_ApiCFG, ByVal objParametriServer As AgronicaCoreParametri) As RispostaStandard
    '    Dim resp As New RispostaStandard
    '    Try
    '        Dim oFld As New jDeereDataModel_Field
    '        Dim bndReq As New rispostaStandard(Of jDeereDataModel_Boundary_REQ)
    '        Dim rBoundary As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Boundary_R

    '        Dim lstBoundaries = rBoundary.LeggiElencoBoundariesDaInviareAJDeere(objParametriServer)

    '        'elenco boundary per campo

    '        For Each bnd In lstBoundaries.Rows
    '            oFld.id = bnd("IDField")
    '            oFld.guid = bnd("GUIDField")

    '            bndReq.RispostaStringa = GetJDeereBoundaryRequest(orgID, Integer.Parse(bnd("ID")), objParametriServer)
    '            If bndReq.RispostaStringa.action = jDeereDataModel_RequestType.Insert Then
    '                bndReq = JDeereCreateBoundary(ConfigOAuth2, orgID, oFld.guid, bndReq.RispostaStringa)
    '                If bndReq.RispostaOK = True Then
    '                Else
    '                    Throw New Exception(bndReq.Errore)
    '                End If
    '            Else
    '                'bndReq = JDeereUpdateBoundary(ConfigOAuth2, orgID, oFld.guid, bndReq.RispostaStringa)
    '                'If bndReq.RispostaOK = True Then
    '                'Else
    '                '    Throw New Exception(bndReq.Errore)
    '                'End If
    '            End If
    '        Next
    '        resp.RispostaOK = True
    '        resp.RispostaStringa = ""
    '        Return resp
    '    Catch ex As Exception
    '        resp.RispostaOK = False
    '        resp.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
    '        Return resp
    '    End Try

    'End Function

    Private Function ElabBoundaryXField(ByVal orgID As String, ByVal ConfigOAuth2 As jDeereDataModel_ApiCFG, ByVal objParametriServer As AgronicaCoreParametri, ByRef oFld As jDeereDataModel_Field) As RispostaStandard
        Dim resp As New RispostaStandard
        Try
            Dim bndReq As New rispostaStandard(Of jDeereDataModel_Boundary_REQ)
            Dim rBoundary As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Boundary_R

            Dim lstBoundaries = rBoundary.LeggiElencoBoundariesDaInviareAJDeere(objParametriServer, oFld.id)

            'elenco boundary per campo

            For Each bnd In lstBoundaries.Rows
                If oFld.guid = "" OrElse oFld.guid Is Nothing Then
                    oFld.guid = bnd("GUIDField")
                End If

                bndReq.RispostaStringa = GetJDeereBoundaryRequest(orgID, bnd("ID"), objParametriServer)
                If bndReq.RispostaStringa.action = jDeereDataModel_RequestType.Insert Then
                    bndReq = JDeereCreateBoundary(ConfigOAuth2, orgID, oFld.guid, bndReq.RispostaStringa)
                    If bndReq.RispostaOK Then
                        Dim newBnd = JDeereGetBoundariesDetail(ConfigOAuth2.JDeere, orgID, oFld.guid, bndReq.RispostaStringa.guid).RispostaStringa
                        newBnd.guid = bndReq.RispostaStringa.guid
                        newBnd.id = bnd("ID")
                        oFld.boundaries.values.Add(newBnd)
                    Else
                        Throw New Exception(bndReq.Errore)
                    End If
                Else
                    'se eseguo l'update significa che ho già la relazione boundary JD <--> boundary GIAS per cui aggiorno i dati di confine sulla tabella di JD
                    'poi eseguo l'aggiornamento tramite api

                    '0- recupero il codice entitià GIAS a partire dall'ID JD
                    Dim rGISEntita = New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_EntitaGias_R
                    Dim dtGISEntitaCod = rGISEntita.LeggiChiaveGIASDaIDJD(enum_TipoEntitaJohnDeere.Boundary, bnd("ID"), "", "", objParametriServer)
                    If dtGISEntitaCod.Rows.Count <= 0 Then
                        Continue For
                    End If

                    '1- recupero i dati geospaziali del campo dal GIS
                    Dim rGIS = New AgronicaCoreGisDAL.GIS_ElementiGrafici_R
                    Dim Data2UpdateBoundary = rGIS.Poligono_GetGeoJsonPointsWithArea(Integer.Parse(dtGISEntitaCod.Rows(0)("Gis_Entita_Cod")), objParametriServer)

                    Dim wktHelp As New AgronicaConversioneCartografiaGias.FormatsConverter.WKT
                    Dim DatiCartograficiBoundary As List(Of xyz) = wktHelp.CreaCoordinateDaWkt(Data2UpdateBoundary.Rows(0)("geo"), True)

                    bndReq.RispostaStringa.multipolygons(0).rings(0).points.Clear()

                    For Each pt In DatiCartograficiBoundary
                        bndReq.RispostaStringa.multipolygons(0).rings(0).points.Add(New jDeereDataModel_Point() With {.Type = "Point", .lat = pt.X, .lon = pt.Y})
                    Next

                    bndReq = JDeereUpdateBoundary(ConfigOAuth2, orgID, oFld.guid, bndReq.RispostaStringa)
                    If bndReq.RispostaOK Then
                        Dim newBnd = JDeereGetBoundariesDetail(ConfigOAuth2.JDeere, orgID, oFld.guid, bnd("GUIDBoundary")).RispostaStringa
                        newBnd.guid = bnd("GUIDBoundary")
                        newBnd.id = bnd("ID")
                        Dim updbnd = oFld.boundaries.values.Where(Function(x) x.id = bnd("ID")).FirstOrDefault()
                        If updbnd Is Nothing Then
                            'aggiunta
                            oFld.boundaries.values.Add(newBnd)
                        Else
                            'aggiornamento
                            updbnd = newBnd
                        End If

                    Else
                        Throw New Exception(bndReq.Errore)
                    End If
                End If
            Next
            resp.RispostaOK = True

        Catch ex As Exception
            resp.RispostaOK = False
            resp.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return resp
    End Function

    Public Function ExecuteUploadFileJD(orgID As String, idFileJD As Integer, ByVal ConfigOAuth2 As jDeereDataModel_ApiCFG, ByVal objParametriServer As AgronicaCoreParametri) As RispostaStandard
        Dim rval As New RispostaStandard
        Try
            Dim reqFile As New jDeereDataModelDAL_Files_R
            Dim rEntityGIAS As New jDeereDataModelDAL_EntitaGias_R
            Dim rAllegato As New Allegati_Documenti_R
            Dim rCFile As New rispostaStandard(Of jDeereDataModel_File)
            Dim rUpload As RispostaStandard
            Dim rW As RispostaStandard

            '0 - recupero il binary data del file dagli allegati
            Dim dtAllegato = rEntityGIAS.LeggiIDAllegato(idFileJD, objParametriServer)
            Dim idAllegato = Integer.Parse(dtAllegato.Rows(0)("idAllegato"))
            Dim guidAllegato = dtAllegato.Rows(0)("guid").ToString()
            Dim fileName As String = dtAllegato.Rows(0)("Name").ToString()

            Dim dt = rAllegato.Leggi(idAllegato, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametriServer)

            Dim bData As Byte() = dt.Rows(0)("File_Allegato_DB")

            '1- stack numeratore JD per files da caricare su operation center
            rCFile = JDeereCreateFileID(orgID, fileName, ConfigOAuth2)
            If rCFile.RispostaOK Then
                Dim rOrg As New jDeereDataModelDAL_Organization_R
                rCFile.RispostaStringa.archived = False
                rCFile.RispostaStringa.delayProcessing = False
                rCFile.RispostaStringa.name = fileName
                rCFile.RispostaStringa.OrganizationID = rOrg.LeggiIDViaGUID(orgID, objParametriServer)
                rCFile.RispostaStringa.id = idFileJD
                '2- registrazione numeratore JD su tabelle di frontiera
                rW = UploadFileJDeereScriviDB(rCFile.RispostaStringa, objParametriServer)
                If rW.RispostaOK Then
                    rCFile.RispostaStringa.id = idFileJD 'reqFile.LeggiIDViaGUID(rCFile.RispostaStringa.guid, objParametriServer)
                    '3- esecuzione upload del file
                    rUpload = JDeereUploadFile(orgID, rCFile.RispostaStringa.guid, bData, ConfigOAuth2)
                    If rUpload.RispostaOK Then
                        Dim messaggioEsito As String = "File caricato con ID " & rCFile.RispostaStringa.guid
                        rCFile.RispostaStringa.StatoInvio = True
                        rCFile.RispostaStringa.EsitoInvio = messaggioEsito
                        '4- aggiornamento record tabella Jd con esito upload
                        rW = UploadFileJDeereScriviDB(rCFile.RispostaStringa, objParametriServer)
                        If rW.RispostaOK Then
                            rval.RispostaOK = True
                            rval.RispostaStringa = messaggioEsito
                        Else
                            Throw New Exception(rW.Errore)
                        End If
                    Else
                        rCFile.RispostaStringa.StatoInvio = False
                        rCFile.RispostaStringa.EsitoInvio = "Errore upload " & rUpload.Errore
                        '4- aggiornamento record tabella Jd con esito upload
                        rW = UploadFileJDeereScriviDB(rCFile.RispostaStringa, objParametriServer)
                        If rW.RispostaOK Then
                            Throw New Exception(rUpload.Errore)
                        Else
                            Throw New Exception(rW.Errore)
                        End If
                    End If
                Else
                    Throw New Exception(rW.Errore)
                End If
            Else
                Throw New Exception(rCFile.Errore)
            End If

        Catch ex As Exception
            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return rval
    End Function

#Region "API John Deere"

#Region "Login & Tokens"

    Public Function TestLoginJDeere(ByRef Config As jDeereDataModel_ApiCFG, ByVal CurrentURI As String, ByRef RedirectUri As String) As RispostaStandard
        Dim rval As New RispostaStandard

        Try
            Dim oauthctrl As OAuth2Controller = New OAuth2Controller(Config.JDeere)

            Dim WebReq As HttpWebRequest = WebRequest.Create(oauthctrl.GetAuthCodeUrl(Config.baseURL, Config.JDeere))

            Dim resp As HttpWebResponse = WebReq.GetResponse()

            If resp.StatusCode = HttpStatusCode.OK Then
                rval.RispostaOK = True
                RedirectUri = resp.ResponseUri.ToString
            Else
                RedirectUri = ""
                Throw New Exception("JDeere Login Failure: " & vbCrLf & resp.StatusCode.ToString() & " " & resp.StatusDescription & vbCrLf)
            End If
        Catch ex As Exception
            RedirectUri = ""
            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return rval
    End Function

    Public Function JDeereGetAccessToken(ByRef ConfigOAuth2 As jDeereDataModel_ApiCFG) As RispostaStandard
        Dim rval As New RispostaStandard
        Try
            Dim oauthctrl As OAuth2Controller = New OAuth2Controller(ConfigOAuth2.JDeere)

            'Dim WebReq As HttpWebRequest = WebRequest.Create()

            Dim HeadCust = New WebHeaderCollection, ParamCust = New Dictionary(Of String, String)
            HeadCust.Add("authorization", "Basic " & oauthctrl.GetBase64EncodedClientCredentials(ConfigOAuth2.JDeere))
            HeadCust.Add("accept", "application/json")
            HeadCust.Add("ContentType", "application/x-www-form-urlencoded")

            Dim client = New Http()

            Dim HttpQueryParams = New Dictionary(Of String, String)

            HttpQueryParams.Add("grant_type", "authorization_code")
            HttpQueryParams.Add("code", ConfigOAuth2.JDeere.AuthToken)
            HttpQueryParams.Add("redirect_uri", ConfigOAuth2.baseURL & "/" & ConfigOAuth2.JDeere.Callback)

            ParamCust.Add("application/x-www-form-urlencoded", QueryHelper.AddQueryStringNoDecode("", HttpQueryParams))

            Dim resp = client.chiamaWS_RestShapr(Nothing, "", oauthctrl.GetAccessTokenUrl(ConfigOAuth2.JDeere), "", RestSharp.Method.POST, "application/json", "", HeadCust, ParamCust)

            If resp.StatusCode = HttpStatusCode.OK Then
                rval.RispostaOK = True
                ConfigOAuth2.JDeere.Token = JsonConvert.DeserializeObject(Of OAuth2Token)(resp.Content)
            Else
                Throw New Exception("JDeere API Call Failure (Get Access Token): " & vbCrLf & resp.StatusCode.ToString() & " " & resp.StatusDescription & vbCrLf)
            End If
        Catch ex As Exception
            rval.RispostaOK = False
            rval.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
    End Function

#End Region

#Region "Letture"
    Public Function JDeereGetOrganizationList(ByVal ConfigOAuth2 As jDeereDataModel_ApiCFG, ByVal username As String) As rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_Organization))
        Dim rval As New rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_Organization))
        Try
            Dim client = New Http()
            Dim HeadCust = New WebHeaderCollection

            HeadCust.Add("Authorization", "Bearer " & ConfigOAuth2.JDeere.Token.access_token)
            HeadCust.Add("Accept", "application/vnd.deere.axiom.v3+json")

            Dim callURI = ConfigOAuth2.JDeere.ApiUrl & If(username <> "", "/users/" & username & "/organizations", "/organizations")

            Dim resp = client.chiamaWS_RestShapr(Nothing, "", callURI, "", RestSharp.Method.GET, "application/json", "", HeadCust)

            If resp.StatusCode = HttpStatusCode.OK Then
                rval.RispostaOK = True
                rval.RispostaStringa = JsonConvert.DeserializeObject(Of jDeereDataModel_GenericList(Of jDeereDataModel_Organization))(resp.Content)
            ElseIf resp.StatusCode = HttpStatusCode.Unauthorized OrElse resp.StatusCode = HttpStatusCode.Forbidden Then
                Throw New Exception("JDeere API Call Failure (Organizations List): Account John Deere non abilitato alla chiamata API" & vbCrLf & vbCrLf)
            Else
                Throw New Exception("JDeere API Call Failure (Organizations List): " & vbCrLf & resp.StatusCode.ToString() & " " & resp.StatusDescription & vbCrLf)
            End If
        Catch ex As Exception
            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return rval
    End Function

    Public Function JDeereGetFieldsListCount(ByVal ConfigOAuth2 As jDeereDataModel_ApiCFG, ByVal orgID As String) As rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_Field))
        Dim rval As New rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_Field))
        Try
            Dim client = New Http()
            Dim HeadCust = New WebHeaderCollection

            HeadCust.Add("Authorization", "Bearer " & ConfigOAuth2.JDeere.Token.access_token)
            HeadCust.Add("Accept", "application/vnd.deere.axiom.v3+json")

            Dim callURI = ConfigOAuth2.JDeere.ApiUrl & "/organizations/" & orgID & "/fields;start=0;count=1"

            Dim resp = client.chiamaWS_RestShapr(Nothing, "", callURI, "", RestSharp.Method.GET, "application/json", "", HeadCust)

            If resp.StatusCode = HttpStatusCode.OK Then
                rval.RispostaOK = True
                rval.RispostaStringa = JsonConvert.DeserializeObject(Of jDeereDataModel_GenericList(Of jDeereDataModel_Field))(resp.Content)
            ElseIf resp.StatusCode = HttpStatusCode.Unauthorized OrElse resp.StatusCode = HttpStatusCode.Forbidden Then
                Throw New Exception("JDeere API Call Failure (Fields List Count per Organization): Account John Deere non abilitato alla chiamata API" & vbCrLf & vbCrLf)
            Else
                Throw New Exception("JDeere API Call Failure (Fields List Count per Organization): " & vbCrLf & resp.StatusCode.ToString() & " " & resp.StatusDescription & vbCrLf)
            End If
        Catch ex As Exception
            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return rval
    End Function

    Public Function JDeereGetFieldsList(ByVal ConfigOAuth2 As jDeereDataModel_ApiCFG, ByVal orgID As String, Optional ByVal StartInterval As Integer = 0, Optional ByVal EndInterval As Integer = 0) As rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_Field))
        Dim rval As New rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_Field))
        Try
            Dim client = New Http()
            Dim HeadCust = New WebHeaderCollection

            HeadCust.Add("Authorization", "Bearer " & ConfigOAuth2.JDeere.Token.access_token)
            HeadCust.Add("Accept", "application/vnd.deere.axiom.v3+json")

            Dim callURI = ConfigOAuth2.JDeere.ApiUrl & "/organizations/" & orgID & "/fields"
            If StartInterval <> 0 OrElse EndInterval <> 0 Then
                callURI &= ";start=" & StartInterval.ToString() & ";count=" & EndInterval.ToString
            End If

            Dim resp = client.chiamaWS_RestShapr(Nothing, "", callURI, "", RestSharp.Method.GET, "application/json", "", HeadCust)

            If resp.StatusCode = HttpStatusCode.OK Then
                rval.RispostaOK = True
                rval.RispostaStringa = JsonConvert.DeserializeObject(Of jDeereDataModel_GenericList(Of jDeereDataModel_Field))(resp.Content)
            ElseIf resp.StatusCode = HttpStatusCode.Unauthorized OrElse resp.StatusCode = HttpStatusCode.Forbidden Then
                Throw New Exception("JDeere API Call Failure (Fields List per Organization): Account John Deere non abilitato alla chiamata API" & vbCrLf & vbCrLf)
            Else
                Throw New Exception("JDeere API Call Failure (Fields List per Organization): " & vbCrLf & resp.StatusCode.ToString() & " " & resp.StatusDescription & vbCrLf)
            End If
        Catch ex As Exception
            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return rval
    End Function

    Public Function JDeereGetBoundariesList(ByVal ConfigOAuth2 As jDeereDataModel_ApiCFG, ByVal orgID As String) As rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_Boundary))
        Dim rval As New rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_Boundary))
        Try
            Dim client = New Http()
            Dim HeadCust = New WebHeaderCollection

            HeadCust.Add("Authorization", "Bearer " & ConfigOAuth2.JDeere.Token.access_token)
            HeadCust.Add("Accept", "application/vnd.deere.axiom.v3+json")

            Dim callURI = ConfigOAuth2.JDeere.ApiUrl & "/organizations/" & orgID & "/boundaries"

            Dim resp = client.chiamaWS_RestShapr(Nothing, "", callURI, "", RestSharp.Method.GET, "application/json", "", HeadCust)

            If resp.StatusCode = HttpStatusCode.OK Then
                rval.RispostaOK = True
                rval.RispostaStringa = JsonConvert.DeserializeObject(Of jDeereDataModel_GenericList(Of jDeereDataModel_Boundary))(resp.Content)
            ElseIf resp.StatusCode = HttpStatusCode.Unauthorized OrElse resp.StatusCode = HttpStatusCode.Forbidden Then
                Throw New Exception("JDeere API Call Failure (Boundaries List): Account John Deere non abilitato alla chiamata API" & vbCrLf & vbCrLf)
            Else
                Throw New Exception("JDeere API Call Failure (Boundaries List): " & vbCrLf & resp.StatusCode.ToString() & " " & resp.StatusDescription & vbCrLf)
            End If
        Catch ex As Exception
            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return rval
    End Function

    Public Function JDeereGetBoundariesByFieldId(ByVal ConfigOAuth2 As OAuth2DataModel_Config, ByVal orgID As String, ByVal fldID As String) As rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_Boundary))
        Dim rval As New rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_Boundary))
        Try
            Dim client = New Http()
            Dim HeadCust = New WebHeaderCollection

            HeadCust.Add("Authorization", "Bearer " & ConfigOAuth2.Token.access_token)
            HeadCust.Add("Accept", "application/vnd.deere.axiom.v3+json")

            Dim callURI = ConfigOAuth2.ApiUrl & "/organizations/" & orgID & "/fields/" & fldID & "/boundaries"

            Dim resp = client.chiamaWS_RestShapr(Nothing, "", callURI, "", RestSharp.Method.GET, "application/json", "", HeadCust)

            If resp.StatusCode = HttpStatusCode.OK Then
                rval.RispostaOK = True
                rval.RispostaStringa = JsonConvert.DeserializeObject(Of jDeereDataModel_GenericList(Of jDeereDataModel_Boundary))(resp.Content)
            ElseIf resp.StatusCode = HttpStatusCode.Unauthorized OrElse resp.StatusCode = HttpStatusCode.Forbidden Then
                Throw New Exception("JDeere API Call Failure (Boundaries List per Field): Account John Deere non abilitato alla chiamata API" & vbCrLf & vbCrLf)
            Else
                Throw New Exception("JDeere API Call Failure (Boundaries List per Field): " & vbCrLf & resp.StatusCode.ToString() & " " & resp.StatusDescription & vbCrLf)
            End If
        Catch ex As Exception
            rval.RispostaOK = False
            rval.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return rval
    End Function

    Public Function JDeereGetBoundariesDetail(ByVal ConfigOAuth2 As OAuth2DataModel_Config, ByVal orgID As String, ByVal fldID As String, ByVal bndID As String) As rispostaStandard(Of jDeereDataModel_Boundary)
        Dim rval As New rispostaStandard(Of jDeereDataModel_Boundary)
        Try
            Dim client = New Http()
            Dim HeadCust = New WebHeaderCollection

            HeadCust.Add("Authorization", "Bearer " & ConfigOAuth2.Token.access_token)
            HeadCust.Add("Accept", "application/vnd.deere.axiom.v3+json")

            Dim callURI = ConfigOAuth2.ApiUrl & "/organizations/" & orgID & "/fields/" & fldID & "/boundaries/" & bndID

            Dim resp = client.chiamaWS_RestShapr(Nothing, "", callURI, "", RestSharp.Method.GET, "application/json", "", HeadCust)

            If resp.StatusCode = HttpStatusCode.OK Then
                rval.RispostaOK = True
                rval.RispostaStringa = JsonConvert.DeserializeObject(Of jDeereDataModel_Boundary)(resp.Content)
            ElseIf resp.StatusCode = HttpStatusCode.Unauthorized OrElse resp.StatusCode = HttpStatusCode.Forbidden Then
                Throw New Exception("JDeere API Call Failure (Boundaries Detail): Account John Deere non abilitato alla chiamata API" & vbCrLf & vbCrLf)
            Else
                Throw New Exception("JDeere API Call Failure (Boundaries Detail): " & vbCrLf & resp.StatusCode.ToString() & " " & resp.StatusDescription & vbCrLf)
            End If
        Catch ex As Exception
            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return rval
    End Function

    Public Function JDeereGetClientsByFieldId(ByVal ConfigOAuth2 As jDeereDataModel_ApiCFG, ByVal orgID As String, ByVal fldID As String) As rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_Client))
        Dim rval As New rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_Client))
        Try
            Dim client = New Http()
            Dim HeadCust = New WebHeaderCollection

            HeadCust.Add("Authorization", "Bearer " & ConfigOAuth2.JDeere.Token.access_token)
            HeadCust.Add("Accept", "application/vnd.deere.axiom.v3+json")

            Dim callURI = ConfigOAuth2.JDeere.ApiUrl & "/organizations/" & orgID & "/fields/" & fldID & "/clients"

            Dim resp = client.chiamaWS_RestShapr(Nothing, "", callURI, "", RestSharp.Method.GET, "application/json", "", HeadCust)
            If resp.StatusCode = HttpStatusCode.OK Then
                rval.RispostaOK = True
                rval.RispostaStringa = JsonConvert.DeserializeObject(Of jDeereDataModel_GenericList(Of jDeereDataModel_Client))(resp.Content)
            ElseIf resp.StatusCode = HttpStatusCode.Unauthorized OrElse resp.StatusCode = HttpStatusCode.Forbidden Then
                Throw New Exception("JDeere API Call Failure (Cleints List per Field): Account John Deere non abilitato alla chiamata API" & vbCrLf & vbCrLf)
            Else
                Throw New Exception("JDeere API Call Failure (Cleints List per Field): " & vbCrLf & resp.StatusCode.ToString() & " " & resp.StatusDescription & vbCrLf)
            End If
        Catch ex As Exception
            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return rval
    End Function

    Public Function JDeereGetFarmsByFieldId(ByVal ConfigOAuth2 As jDeereDataModel_ApiCFG, ByVal orgID As String, ByVal fldID As String) As rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_Farm))
        Dim rval As New rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_Farm))
        Try
            Dim client = New Http()
            Dim HeadCust = New WebHeaderCollection

            HeadCust.Add("Authorization", "Bearer " & ConfigOAuth2.JDeere.Token.access_token)
            HeadCust.Add("Accept", "application/vnd.deere.axiom.v3+json")

            Dim callURI = ConfigOAuth2.JDeere.ApiUrl & "/organizations/" & orgID & "/fields/" & fldID & "/farms"

            Dim resp = client.chiamaWS_RestShapr(Nothing, "", callURI, "", RestSharp.Method.GET, "application/json", "", HeadCust)
            If resp.StatusCode = HttpStatusCode.OK Then
                rval.RispostaOK = True
                rval.RispostaStringa = JsonConvert.DeserializeObject(Of jDeereDataModel_GenericList(Of jDeereDataModel_Farm))(resp.Content)
            ElseIf resp.StatusCode = HttpStatusCode.Unauthorized OrElse resp.StatusCode = HttpStatusCode.Forbidden Then
                Throw New Exception("JDeere API Call Failure (Farms List per Field): Account John Deere non abilitato alla chiamata API" & vbCrLf & vbCrLf)
            Else
                Throw New Exception("JDeere API Call Failure (Farms List per Field): " & vbCrLf & resp.StatusCode.ToString() & " " & resp.StatusDescription & vbCrLf)
            End If
        Catch ex As Exception
            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return rval
    End Function

    Public Function JDeereGetFieldOpByFieldId(ByVal ConfigOAuth2 As jDeereDataModel_ApiCFG, ByVal orgID As String, ByVal fldID As String) As rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_FieldOperation))
        Dim rval As New rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_FieldOperation))
        Try
            Dim client = New Http()
            Dim HeadCust = New WebHeaderCollection

            HeadCust.Add("Authorization", "Bearer " & ConfigOAuth2.JDeere.Token.access_token)
            HeadCust.Add("Accept", "application/vnd.deere.axiom.v3+json")

            Dim callURI = ConfigOAuth2.JDeere.ApiUrl & "/organizations/" & orgID & "/fields/" & fldID & "/fieldOperations"

            Dim resp = client.chiamaWS_RestShapr(Nothing, "", callURI, "", RestSharp.Method.GET, "application/json", "", HeadCust)
            If resp.StatusCode = HttpStatusCode.OK Then
                rval.RispostaOK = True
                rval.RispostaStringa = JsonConvert.DeserializeObject(Of jDeereDataModel_GenericList(Of jDeereDataModel_FieldOperation))(resp.Content)
            ElseIf resp.StatusCode = HttpStatusCode.Unauthorized OrElse resp.StatusCode = HttpStatusCode.Forbidden Then
                Throw New Exception("JDeere API Call Failure (FieldOperations List per Field): Account John Deere non abilitato alla chiamata API" & vbCrLf & vbCrLf)
            Else
                Throw New Exception("JDeere API Call Failure (FieldOperations List per Field): " & vbCrLf & resp.StatusCode.ToString() & " " & resp.StatusDescription & vbCrLf)
            End If
        Catch ex As Exception
            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return rval
    End Function

    Public Function JDeereGetMeasurementByFieldOp(ByVal ConfigOAuth2 As jDeereDataModel_ApiCFG, ByVal fldOpID As String) As rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_FieldOperationMeasurement))
        Dim rval As New rispostaStandard(Of jDeereDataModel_GenericList(Of jDeereDataModel_FieldOperationMeasurement))
        Try
            Dim client = New Http()
            Dim HeadCust = New WebHeaderCollection

            HeadCust.Add("Authorization", "Bearer " & ConfigOAuth2.JDeere.Token.access_token)
            HeadCust.Add("Accept", "application/vnd.deere.axiom.v3+json")

            Dim callURI = ConfigOAuth2.JDeere.ApiUrl & "/fieldOperation/" & fldOpID & "/measurementTypes"

            Dim resp = client.chiamaWS_RestShapr(Nothing, "", callURI, "", RestSharp.Method.GET, "application/json", "", HeadCust)
            If resp.StatusCode = HttpStatusCode.OK Then
                rval.RispostaOK = True
                rval.RispostaStringa = JsonConvert.DeserializeObject(Of jDeereDataModel_GenericList(Of jDeereDataModel_FieldOperationMeasurement))(resp.Content)
            ElseIf resp.StatusCode = HttpStatusCode.NotFound Then
                rval.RispostaOK = True
            ElseIf resp.StatusCode = HttpStatusCode.Unauthorized OrElse resp.StatusCode = HttpStatusCode.Forbidden Then
                Throw New Exception("JDeere API Call Failure (Measurements List per FieldOperation): Account John Deere non abilitato alla chiamata API" & vbCrLf & vbCrLf)
            Else
                Throw New Exception("JDeere API Call Failure (Measurements List per FieldOperation): " & vbCrLf & resp.StatusCode.ToString() & " " & resp.StatusDescription & vbCrLf)
            End If
        Catch ex As Exception
            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return rval
    End Function

    Public Function JDeereGetShapeFileFromOperationID(ByVal ConfigOAuth2 As jDeereDataModel_ApiCFG, ByVal fldOpID As String) As RispostaStandard
        Dim rval As New RispostaStandard
        Try
            Dim client = New Http()
            Dim HeadCust = New WebHeaderCollection

            HeadCust.Add("Authorization", "Bearer " & ConfigOAuth2.JDeere.Token.access_token)
            HeadCust.Add("Accept", "application/vnd.deere.axiom.v3+json")

            Dim callURI = ConfigOAuth2.JDeere.ApiUrl & "/fieldOps/" & fldOpID & "?resolution="

            callURI &= [Enum].GetName(GetType(enum_ResolutionMappaProduzioneJophnDeere), ConfigOAuth2.Resolution)
            callURI &= "&shapeType="
            callURI &= [Enum].GetName(GetType(enum_ShapeTypeMappaProduzioneJophnDeere), ConfigOAuth2.ShapeType)

            'Dim callURI = ConfigOAuth2.JDeere.ApiUrl & "/fieldOps/" & fldOpID & "?resolution=OneHertz&shapeType=Polygon"

            Dim resp = client.chiamaWS_RestShapr(Nothing, "", callURI, "", RestSharp.Method.GET, "application/json", "", HeadCust)
            Select Case resp.StatusCode
                Case HttpStatusCode.Accepted
                    rval.RispostaOK = True
                    rval.RispostaStringa = ""
                Case HttpStatusCode.OK
                    rval.RispostaOK = True
                    Dim HeaderResp = resp.Headers.ToList()
                    Dim location = resp.ResponseUri.AbsoluteUri.ToString() 'IIf(HeaderResp.Find(Function(x) x.Name = "Location") IsNot Nothing, HeaderResp.Find(Function(x) x.Name = "Location").Value, "").ToString()
                    rval.RispostaStringa = location
                Case HttpStatusCode.NotAcceptable
                    rval.RispostaOK = False
                    rval.RispostaStringa = "Shape file cannot be generated"
                Case HttpStatusCode.Unauthorized Or HttpStatusCode.Forbidden
                    Throw New Exception("JDeere API Call Failure (Download ShapeFile per FieldOperation): Account John Deere non abilitato alla chiamata API" & vbCrLf & vbCrLf)
                Case Else
                    Throw New Exception("JDeere API Call Failure (Download ShapeFile per FieldOperation ): " & vbCrLf & resp.StatusCode.ToString() & " " & resp.StatusDescription & vbCrLf)
            End Select
        Catch ex As Exception
            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return rval
    End Function

    Public Function JDeereExecuteDownnloadShapeFile(ByVal ConfigOAuth2 As jDeereDataModel_ApiCFG, ByVal urlDownload As String) As rispostaStandard(Of Byte())
        Dim rval As New rispostaStandard(Of Byte())
        Try
            Dim client = New Http()
            Dim HeadCust = New WebHeaderCollection

            'HeadCust.Add("Authorization", "Bearer " & ConfigOAuth2.JDeere.Token.access_token)
            'HeadCust.Add("Accept", "application/vnd.deere.axiom.v3+json")

            Dim callURI = urlDownload

            Dim resp = client.chiamaWS_RestShapr(Nothing, "", callURI, "", RestSharp.Method.GET, "application/json", "", HeadCust)
            Select Case resp.StatusCode
                Case HttpStatusCode.OK
                    rval.RispostaOK = True
                    Dim HeaderResp = resp.Headers.ToList()
                    Dim contentType = IIf(HeaderResp.Find(Function(x) x.Name = "Content-Type") IsNot Nothing, HeaderResp.Find(Function(x) x.Name = "Content-Type").Value, "").ToString()
                    rval.RispostaStringa = resp.RawBytes
                Case HttpStatusCode.Unauthorized Or HttpStatusCode.Forbidden
                    Throw New Exception("JDeere API Call Failure (Execute Download ShapeFile per FieldOperation): Account John Deere non abilitato alla chiamata API" & vbCrLf & vbCrLf)
                Case Else
                    Throw New Exception("JDeere API Call Failure (Execute Download ShapeFile per FieldOperation ): " & vbCrLf & resp.StatusCode.ToString() & " " & resp.StatusDescription & vbCrLf)
            End Select
        Catch ex As Exception
            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return rval
    End Function
#End Region

#Region "Scritture"
    Public Function JDeereCreateField(ByVal ConfigOAuth2 As jDeereDataModel_ApiCFG, ByVal orgId As String, ByVal objField As jDeereDataModel_Field_REQ) As rispostaStandard(Of jDeereDataModel_Field_REQ)
        Dim rval As New rispostaStandard(Of jDeereDataModel_Field_REQ)
        Try
            Dim client = New Http()

            Dim AcceptFormat As String = "application/vnd.deere.axiom.v3+json"
            Dim BodyReqFormat As String = "application/vnd.deere.axiom.v3+json"

            Dim resourceURI = "/organizations/" & orgId & "/fields"

            Dim HeadCust = New WebHeaderCollection
            HeadCust.Add("Authorization", "Bearer " & ConfigOAuth2.JDeere.Token.access_token)

            Dim ReqBody = JsonConvert.SerializeObject(objField)

            Dim resp = client.CallWS_RestSharp_JSON(ConfigOAuth2.JDeere.ApiUrl, resourceURI, "POST", ReqBody, HeadCust, AcceptFormat, BodyReqFormat)

            If resp.StatusCode = HttpStatusCode.Created OrElse resp.StatusCode = HttpStatusCode.OK Then
                rval.RispostaOK = True
                Dim HeaderResp As List(Of RestSharp.Parameter) = resp.Header
                Dim location = If(HeaderResp.Find(Function(x) x.Name = "Location") IsNot Nothing, HeaderResp.Find(Function(x) x.Name = "Location").Value, "").ToString()
                Dim garbage = location.Split("/")
                objField.guid = garbage(garbage.Length - 1)
                rval.RispostaStringa = objField
            ElseIf resp.StatusCode = HttpStatusCode.Unauthorized OrElse resp.StatusCode = HttpStatusCode.Forbidden Then
                Throw New Exception("JDeere API Call Failure (Create new Field): Account John Deere non abilitato alla chiamata API" & vbCrLf & vbCrLf)
            Else
                Dim ExMessage As String = resp.Content.ToString()
                If resp.Content("errors") IsNot Nothing Then
                    ExMessage = ""
                    For Each ln In resp.Content("errors")(0)
                        ExMessage &= ln.ToString() & vbCrLf
                    Next
                End If
                Throw New Exception("JDeere API Call Failure (Create new Field): " & vbCrLf & resp.StatusCode.ToString() & vbCrLf & ExMessage & vbCrLf)
            End If


        Catch ex As Exception
            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return rval
    End Function

    Public Function JDeereUpdateField(ByVal ConfigOAuth2 As jDeereDataModel_ApiCFG, ByVal orgId As String, ByVal objField As jDeereDataModel_Field_REQ) As rispostaStandard(Of jDeereDataModel_Field_REQ)
        Dim rval As New rispostaStandard(Of jDeereDataModel_Field_REQ)
        Try
            Dim client = New Http()

            Dim AcceptFormat As String = "application/vnd.deere.axiom.v3+json"
            Dim BodyReqFormat As String = "application/vnd.deere.axiom.v3+json"

            Dim resourceURI = "/organizations/" & orgId & "/fields/" & objField.id

            Dim HeadCust = New WebHeaderCollection
            HeadCust.Add("Authorization", "Bearer " & ConfigOAuth2.JDeere.Token.access_token)

            Dim ReqBody = JsonConvert.SerializeObject(objField)

            Dim resp = client.CallWS_RestSharp_JSON(ConfigOAuth2.JDeere.ApiUrl, resourceURI, "PUT", ReqBody, HeadCust, AcceptFormat, BodyReqFormat)

            If resp.StatusCode = HttpStatusCode.NoContent OrElse resp.StatusCode = HttpStatusCode.OK Then
                rval.RispostaOK = True
            ElseIf resp.StatusCode = HttpStatusCode.Unauthorized OrElse resp.StatusCode = HttpStatusCode.Forbidden Then
                Throw New Exception("JDeere API Call Failure (Update existing Field): Account John Deere non abilitato alla chiamata API" & vbCrLf & vbCrLf)
            Else
                Dim ExMessage As String = resp.Content.ToString()
                If resp.Content("errors") IsNot Nothing Then
                    ExMessage = ""
                    For Each ln In resp.Content("errors")(0)
                        ExMessage &= ln.ToString() & vbCrLf
                    Next
                End If
                Throw New Exception("JDeere API Call Failure (Update existing Field): " & vbCrLf & resp.StatusCode.ToString() & vbCrLf & ExMessage & vbCrLf)
            End If
        Catch ex As Exception
            rval.RispostaOK = False
            rval.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return rval
    End Function

    Public Function JDeereCreateBoundary(ByVal ConfigOAuth2 As jDeereDataModel_ApiCFG, ByVal orgId As String, ByVal fldID As String, ByVal objBound As jDeereDataModel_Boundary_REQ) As rispostaStandard(Of jDeereDataModel_Boundary_REQ)
        Dim rval As New rispostaStandard(Of jDeereDataModel_Boundary_REQ)
        Try
            Dim client = New Http()

            Dim AcceptFormat As String = "application/vnd.deere.axiom.v3+json"
            Dim BodyReqFormat As String = "application/vnd.deere.axiom.v3+json"

            Dim resourceURI = "/organizations/" & orgId & "/fields/" & fldID & "/boundaries"

            Dim HeadCust = New WebHeaderCollection
            HeadCust.Add("Authorization", "Bearer " & ConfigOAuth2.JDeere.Token.access_token)

            Dim ReqBody = JsonConvert.SerializeObject(objBound)

            Dim resp = client.CallWS_RestSharp_JSON(ConfigOAuth2.JDeere.ApiUrl, resourceURI, "POST", ReqBody, HeadCust, AcceptFormat, BodyReqFormat)

            If resp.StatusCode = HttpStatusCode.Created OrElse resp.StatusCode = HttpStatusCode.OK Then
                rval.RispostaOK = True
                Dim HeaderResp As List(Of RestSharp.Parameter) = resp.Header
                Dim location = If(HeaderResp.Find(Function(x) x.Name = "Location") IsNot Nothing, HeaderResp.Find(Function(x) x.Name = "Location").Value, "").ToString()
                Dim garbage = location.Split("/")
                objBound.guid = garbage(garbage.Length - 1)
                rval.RispostaStringa = objBound
            ElseIf resp.StatusCode = HttpStatusCode.Unauthorized OrElse resp.StatusCode = HttpStatusCode.Forbidden Then
                Throw New Exception("JDeere API Call Failure (Create new Boundary): Account John Deere non abilitato alla chiamata API" & vbCrLf & vbCrLf)
            Else
                Dim ExMessage As String = resp.Content.ToString()
                If resp.Content("errors") IsNot Nothing Then
                    ExMessage = ""
                    For Each ln In resp.Content("errors")(0)
                        ExMessage &= ln.ToString() & vbCrLf
                    Next
                End If
                Throw New Exception("JDeere API Call Failure (Create new Boundary): " & vbCrLf & resp.StatusCode.ToString() & vbCrLf & ExMessage & vbCrLf)
            End If

        Catch ex As Exception
            rval.RispostaOK = False
            rval.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return rval
    End Function

    Public Function JDeereUpdateBoundary(ByVal ConfigOAuth2 As jDeereDataModel_ApiCFG, ByVal orgId As String, ByVal fldID As String, ByVal objBound As jDeereDataModel_Boundary_REQ) As rispostaStandard(Of jDeereDataModel_Boundary_REQ)
        Dim rval As New rispostaStandard(Of jDeereDataModel_Boundary_REQ)
        Try
            Dim client = New Http()

            Dim AcceptFormat As String = "application/vnd.deere.axiom.v3+json"
            Dim BodyReqFormat As String = "application/vnd.deere.axiom.v3+json"

            Dim resourceURI = "/organizations/" & orgId & "/fields/" & fldID & "/boundaries/" & objBound.guid

            Dim HeadCust = New WebHeaderCollection
            HeadCust.Add("Authorization", "Bearer " & ConfigOAuth2.JDeere.Token.access_token)

            Dim ReqBody = JsonConvert.SerializeObject(objBound)

            Dim resp = client.CallWS_RestSharp_JSON(ConfigOAuth2.JDeere.ApiUrl, resourceURI, "PUT", ReqBody, HeadCust, AcceptFormat, BodyReqFormat)

            If resp.StatusCode = HttpStatusCode.NoContent OrElse resp.StatusCode = HttpStatusCode.OK Then
                rval.RispostaOK = True
                Dim HeaderResp As List(Of RestSharp.Parameter) = resp.Header
                Dim location = ""
                If HeaderResp.Find(Function(x) x.Name = "Location") IsNot Nothing Then
                    location = HeaderResp.Find(Function(x) x.Name = "Location").Value.ToString()
                End If
                Dim garbage = location.Split("/")
                objBound.guid = garbage(garbage.Length - 1)
                rval.RispostaStringa = objBound
            ElseIf resp.StatusCode = HttpStatusCode.Unauthorized OrElse resp.StatusCode = HttpStatusCode.Forbidden Then
                Throw New Exception("JDeere API Call Failure (Update Boundary): Account John Deere non abilitato alla chiamata API" & vbCrLf & vbCrLf)
            Else
                Dim ExMessage As String = resp.Content.ToString()
                If resp.Content("errors") IsNot Nothing Then
                    ExMessage = ""
                    For Each ln In resp.Content("errors")(0)
                        ExMessage &= ln.ToString() & vbCrLf
                    Next
                End If
                Throw New Exception("JDeere API Call Failure (Update Boundary): " & vbCrLf & resp.StatusCode.ToString() & vbCrLf & resp.Content.ToString() & vbCrLf)
            End If

        Catch ex As Exception
            rval.RispostaOK = False
            rval.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return rval
    End Function

    Public Function JDeereCreateFileID(orgID As String, fileName As String, ByVal ConfigOAuth2 As jDeereDataModel_ApiCFG) As rispostaStandard(Of jDeereDataModel_File)
        Dim rval As New rispostaStandard(Of jDeereDataModel_File)
        Try
            Dim client = New Http()

            Dim AcceptFormat As String = "application/vnd.deere.axiom.v3+json"
            Dim BodyReqFormat As String = "application/vnd.deere.axiom.v3+json"

            Dim resourceURI = "/organizations/" & orgID & "/files"

            Dim HeadCust = New WebHeaderCollection
            HeadCust.Add("Authorization", "Bearer " & ConfigOAuth2.JDeere.Token.access_token)

            Dim ReqBody = "{ ""name"":""" & fileName & """ }"

            Dim resp = client.CallWS_RestSharp_JSON(ConfigOAuth2.JDeere.ApiUrl, resourceURI, "POST", ReqBody, HeadCust, AcceptFormat, BodyReqFormat)

            If resp.StatusCode = HttpStatusCode.Created OrElse resp.StatusCode = HttpStatusCode.OK Then
                rval.RispostaOK = True
                rval.RispostaStringa = New jDeereDataModel_File
                Dim HeaderResp As List(Of RestSharp.Parameter) = resp.Header
                Dim location = IIf(HeaderResp.Find(Function(x) x.Name = "Location") IsNot Nothing, HeaderResp.Find(Function(x) x.Name = "Location").Value, "").ToString()
                Dim garbage = location.Split("/")
                rval.RispostaStringa.guid = garbage(garbage.Length - 1)
            ElseIf resp.StatusCode = HttpStatusCode.Unauthorized OrElse resp.StatusCode = HttpStatusCode.Forbidden Then
                Throw New Exception("JDeere API Call Failure (Create file upload repository): Account John Deere non abilitato alla chiamata API" & vbCrLf & vbCrLf)
            Else
                Dim ExMessage As String = resp.Content.ToString()
                If resp.StatusCode <> HttpStatusCode.BadRequest Then
                    If resp.Content("errors") IsNot Nothing Then
                        ExMessage = ""
                        For Each ln In resp.Content("errors")(0)
                            ExMessage &= ln.ToString() & vbCrLf
                        Next
                    End If
                End If
                Throw New Exception("JDeere API Call Failure (Create file upload repository): " & vbCrLf & resp.StatusCode.ToString() & vbCrLf & ExMessage & vbCrLf)
            End If
        Catch ex As Exception
            rval.RispostaOK = False
            rval.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return rval
    End Function

    Public Function JDeereUploadFile(orgID As String, idFile As String, ByVal FilePayload As Byte(), ByVal ConfigOAuth2 As jDeereDataModel_ApiCFG) As RispostaStandard
        Dim rval As New RispostaStandard
        Try
            Dim client = New Http()

            Dim AcceptFormat As String = "application/vnd.deere.axiom.v3+json"
            Dim BodyReqFormat As String = "application/zip"

            Dim resourceURI = "/files/" & idFile

            Dim HeadCust = New WebHeaderCollection
            HeadCust.Add("Authorization", "Bearer " & ConfigOAuth2.JDeere.Token.access_token)

            Dim ReqBody = ""

            Dim resp = client.CallWS_RestSharp_JSON(ConfigOAuth2.JDeere.ApiUrl, resourceURI, "PUT", ReqBody, HeadCust, AcceptFormat, BodyReqFormat, FilePayload)

            If resp.StatusCode = HttpStatusCode.NoContent OrElse resp.StatusCode = HttpStatusCode.OK Then
                rval.RispostaOK = True
            ElseIf resp.StatusCode = HttpStatusCode.Unauthorized OrElse resp.StatusCode = HttpStatusCode.Forbidden Then
                Throw New Exception("JDeere API Call Failure (Upload File content): Account John Deere non abilitato alla chiamata API" & vbCrLf & vbCrLf)
            Else
                Dim ExMessage As String = resp.Content.ToString()
                If resp.Content("errors") IsNot Nothing Then
                    ExMessage = ""
                    For Each ln In resp.Content("errors")(0)
                        ExMessage &= ln.ToString() & vbCrLf
                    Next
                End If
                Throw New Exception("JDeere API Call Failure (Upload File content): " & vbCrLf & resp.StatusCode.ToString() & vbCrLf & resp.Content.ToString() & vbCrLf)
            End If
        Catch ex As Exception
            rval.RispostaOK = False
            rval.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return rval
    End Function

    Public Function JDeereUpdateFile(orgID As String, idFile As String, ByVal ConfigOAuth2 As jDeereDataModel_ApiCFG) As RispostaStandard
        Dim rval As New RispostaStandard
        Try
            Dim client = New Http()

            Dim AcceptFormat As String = "application/vnd.deere.axiom.v3+json"
            Dim BodyReqFormat As String = "application/vnd.deere.axiom.v3+json"

            Dim resourceURI = "/organizations/" & orgID & "/files/" & idFile

            Dim HeadCust = New WebHeaderCollection
            HeadCust.Add("Authorization", "Bearer " & ConfigOAuth2.JDeere.Token.access_token)

            Dim ReqBody = JsonConvert.SerializeObject(New jDeereDataModel_File With {.id = idFile, .archived = True, .delayProcessing = False})

            Dim resp = client.CallWS_RestSharp_JSON(ConfigOAuth2.JDeere.ApiUrl, resourceURI, "PUT", ReqBody, HeadCust, AcceptFormat, BodyReqFormat)

            If resp.StatusCode = HttpStatusCode.NoContent OrElse resp.StatusCode = HttpStatusCode.OK Then
                rval.RispostaOK = True
            ElseIf resp.StatusCode = HttpStatusCode.Unauthorized OrElse resp.StatusCode = HttpStatusCode.Forbidden Then
                Throw New Exception("JDeere API Call Failure (Update File content): Account John Deere non abilitato alla chiamata API" & vbCrLf & vbCrLf)
            Else
                Dim ExMessage As String = resp.Content.ToString()
                If resp.Content("errors") IsNot Nothing Then
                    ExMessage = ""
                    For Each ln In resp.Content("errors")(0)
                        ExMessage &= ln.ToString() & vbCrLf
                    Next
                End If
                Throw New Exception("JDeere API Call Failure (Update File content): " & vbCrLf & resp.StatusCode.ToString() & vbCrLf & resp.Content.ToString() & vbCrLf)
            End If
        Catch ex As Exception
            rval.RispostaOK = False
            rval.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return rval
    End Function
#End Region

#Region "API Builder Request"

    Public Function GetJDeereFieldRequest(ByVal orgID As String, ByVal fldIdGIAS As Integer, ByVal objParametriServer As AgronicaCoreParametri) As jDeereDataModel_Field_REQ
        Dim Req As New jDeereDataModel_Field_REQ
        Try
            Dim rField As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Field_R
            Dim rClient As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Client_R
            Dim rFarm As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Farm_R

            Dim fld = rField.LeggiDettaglioFieldByID(fldIdGIAS, objParametriServer)
            Dim cli As DataTable
            Dim frm As DataTable

            If fld.Rows.Count > 0 Then
                For Each row In fld.Rows
                    If InStr(row("guid"), "AGR-") > 0 Then
                        Req.action = jDeereDataModel_RequestType.Insert
                        Req.id = Nothing
                    Else
                        Req.action = jDeereDataModel_RequestType.Update
                        Req.id = row("guid")
                    End If
                    Req.name = row("Name")
                    Req.archived = row("archived")

                    'client section
                    cli = rClient.LeggiDettaglioClientByID(row("ClientID"), objParametriServer)
                    If cli.Rows.Count > 0 Then
                        For Each rowC In cli.Rows
                            If InStr(rowC("guid"), "AGR-") > 0 Then
                                Dim cliREQ As New jDeereDataModel_Client_REQ
                                cliREQ.name = rowC("Name")
                                cliREQ.id = rClient.LeggiGUIDViaName(cliREQ.name, objParametriServer)
                                If cliREQ.id.Contains("AGR-") Then
                                    cliREQ.id = ""
                                End If
                                'Cliente nuovo
                                Req.clients.clients.Add(cliREQ)
                            Else
                                'Cliente esistente
                                Req.clients.clients.Add(New jDeereDataModel_Client_REQ With {.id = rowC("guid"), .name = rowC("Name")})
                            End If
                        Next
                    End If

                    'farm section
                    frm = rFarm.LeggiDettaglioFarmByID(row("FarmID"), objParametriServer)
                    If frm.Rows.Count > 0 Then
                        For Each rowF In frm.Rows
                            If InStr(rowF("guid"), "AGR-") > 0 Then
                                Dim frmREQ As New jDeereDataModel_Farm_REQ
                                frmREQ.name = rowF("Name")
                                frmREQ.id = rFarm.LeggiGUIDViaName(frmREQ.name, objParametriServer)
                                If frmREQ.id.Contains("AGR-") Then
                                    frmREQ.id = ""
                                End If
                                'Farm nuova
                                Req.farms.farms.Add(frmREQ)
                            Else
                                'Farm esistente
                                Req.farms.farms.Add(New jDeereDataModel_Farm_REQ With {.id = rowF("guid"), .name = rowF("Name")})
                            End If
                        Next
                    End If
                Next
            End If
        Catch ex As Exception
            Req = Nothing
        End Try
        Return Req
    End Function

    Public Function GetJDeereBoundaryRequest(ByVal orgID As String, ByVal bndIdJD As Integer, ByVal objParametriServer As AgronicaCoreParametri) As jDeereDataModel_Boundary_REQ
        Dim Req As New jDeereDataModel_Boundary_REQ
        Try
            Dim rBoundary As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Boundary_R
            Dim bndRec = rBoundary.LeggiCreateReqViaJsonSQL(bndIdJD, objParametriServer)
            Dim res = JsonConvert.DeserializeObject(Of List(Of jDeereDataModel_Boundary_REQ))(bndRec)

            If res(0).guid Is Nothing OrElse res(0).guid = "" Then
                res(0).action = jDeereDataModel_RequestType.Insert
            Else
                res(0).action = jDeereDataModel_RequestType.Update
            End If

            Req = res(0)
        Catch ex As Exception
            Req = Nothing
        End Try
        Return Req
    End Function
#End Region

#End Region


    Public Sub JohnDeereCaricaClienteFarm(ByVal piva As String, ByVal sa_Cod As String, ByRef Cliente As String, ByRef Farm As String, ByVal objParametriServer As AgronicaCoreParametri)

        Dim icL As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_EntitaGias_R

        Dim icDT As DataTable = icL.LeggiAssociazionePivaSaCodConTipoEntitaJD(
            enum_TipoEntitaJohnDeere.Client, piva, 0, "", "", objParametriServer)

        Dim saDT As DataTable = icL.LeggiAssociazionePivaSaCodConTipoEntitaJD(
            enum_TipoEntitaJohnDeere.Farm, piva, sa_Cod, "", "", objParametriServer)

        Cliente = ""
        Farm = ""

        If icDT.Rows.Count > 0 Then
            Cliente = icDT(0)("val_Cod")
        End If

        If saDT.Rows.Count > 0 Then
            Farm = saDT(0)("val_cod")
        End If

    End Sub

    Public Shared Function GetCfgConnParametersFromDB(ByVal ChiaveConfigSiti As String, ByVal objParametri_Server As AgronicaCoreParametri) As jDeereDataModel_ApiCFG
        Dim cfg_db As jDeereDataModel_ApiCFG
        Try
            Dim oReader As New AgronicaCoreVarieDAL.Configurazione_Siti_R()
            cfg_db = JsonConvert.DeserializeObject(Of jDeereDataModel_ApiCFG)(oReader.Leggi_Valore(6, ChiaveConfigSiti, "", "", objParametri_Server))
            Return cfg_db
        Catch ex As Exception
            Dim MessaggioErrore As String
            MessaggioErrore = ex.Message  '' per ora

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[Agronica.Helpers.OAuth2] : " & MessaggioErrore, ex)

            Return cfg_db
        End Try
    End Function

    Public Function JDeereRequestDownload(idReq As String, ByVal objParametri_Server As AgronicaCoreParametri) As RispostaStandard
        Dim ret As New RispostaStandard
        Try
            Dim rReader = New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_FieldOperationFileRequest_R
            Dim rfldOps = New jDeereDataModelDAL_FieldOperation_R

            Dim reqListDT = rReader.LeggiRichieste(idReq, True, objParametri_Server)

            Dim reqParameters As New jDeereDataModel_FieldOperationFileRequest_Parameter
            Dim jdc As New jDeereController

            Dim row = rReader.LeggiDaFieldOperation(Integer.Parse(reqListDT.Rows(0)("FieldOperationID")), reqListDT.Rows(0)("FieldOperationGUID"), objParametri_Server).Rows(0)
            Dim currReq As New jDeereDataModel_FieldOperationFileRequest With {
                    .ID = row("ID"),
                    .FieldOperationID = row("FieldOperationID"),
                    .FieldOperationGUID = row("FieldOperationGUID"),
                    .Allegati_Documenti_Cod = row("Allegati_Documenti_Cod"),
                    .RequestState = row("RequestState"),
                    .Request_Parameters = row("Request_Parameters"),
                    .OAuth_Token = row("OAuth_Token"),
                    .Esito = row("Esito")
                }

            Dim fldOp = rfldOps.LeggiFieldOperation(currReq.FieldOperationID, objParametri_Server)

            If fldOp.Rows.Count() <= 0 Then
                Throw New Exception("Field Operation collegata alla file request non trovata. operazione interrotta")
            End If

            Dim rowFldOp = fldOp.Rows(0)

            Dim currFld As New jDeereDataModel_FieldOperation With {
                    .id = rowFldOp("ID"),
                    .fieldOperationType = rowFldOp("fieldOperationType"),
                    .adaptMachineType = rowFldOp("adaptMachineType"),
                    .cropSeason = rowFldOp("cropSeason"),
                    .startDate = rowFldOp("startDate"),
                    .endDate = rowFldOp("endDate"),
                    .guid = rowFldOp("guid"),
                    .Allegati_Documenti_Cod = rowFldOp("Allegati_Documenti_Cod"),
                    .FieldID = rowFldOp("FieldID"),
                    .OrganizationID = rowFldOp("OrganizationID")
                }

            'per ogni riga eseguire il download e registrare il contenuto nella tabella ellageti_documenti
            reqParameters = JsonConvert.DeserializeObject(Of jDeereDataModel_FieldOperationFileRequest_Parameter)(row("Request_Parameters"))
            Dim chk As New RispostaStandard
            Dim nLoop As Integer = 0
            Dim nLoopTime As Integer = 0
            Dim breakCicleTimeMS As Integer = (30 * 60 * 1000)  ' equivalente di 30 minuti in millisecondi
            'considerando le indicazioni della documentazione John Deere, uno shape file di produzione po
            While nLoop <= 10
                'aggiorno lo stato della richiesta a 1 (elaborazione in corso)
                currReq.RequestState = 1
                chk = AggiornaRichiesta(currReq, objParametri_Server)
                If Not chk.RispostaOK Then
                    Throw New Exception(chk.Errore)
                End If
                chk = jdc.JDeereGetShapeFileFromOperationID(reqParameters.oauth_cfg, reqParameters.fieldOpGuid)
                If Not chk.RispostaOK Then
                    Exit While
                Else
                    If chk.RispostaStringa <> "" Then
                        Exit While
                    End If
                End If
                nLoop += 1
                'sleep di 5 secondi a salire
                Dim waitTime = 5000 * (2 ^ (nLoop - 1))
                Threading.Thread.Sleep(waitTime)
                Debug.WriteLine("Loop: " & nLoop.ToString & " waitTime: " & (waitTime / 1000).ToString())
            End While
            If Not chk.RispostaOK Then
                'chiamata di richiesta generazione shape file andata in errore, aggiornare la richiesta con l'esito
                currReq.Esito = chk.Errore
                currReq.RequestState = 3    'Errore
            Else
                If chk.RispostaStringa = "" Then
                    'uscito da loop con ok ma senza valorizzare l'url di download -> segnare errore con Esito="Tempo di attesa insufficiente per generare il file"
                    currReq.Esito = "Tempo di attesa insufficiente per generare il file"
                    currReq.RequestState = 3    'Errore
                Else
                    ' qui ritorno l'url da cui eseguire il download che verrà fatto da un altro WS
                    ret.RispostaStringa = chk.RispostaStringa
                End If
            End If
            chk = AggiornaRichiesta(currReq, objParametri_Server)
            If Not chk.RispostaOK Then
                Throw New Exception(chk.Errore)
            End If
            ret.RispostaOK = True
        Catch ex As Exception
            ret.RispostaOK = False
            ret.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return ret
    End Function

    Public Function JDeereExecuteDownload(idReq As String, url As String, ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri) As RispostaStandard
        Dim ret As New RispostaStandard
        Try
            Dim rReader = New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_FieldOperationFileRequest_R
            Dim rfldOps = New jDeereDataModelDAL_FieldOperation_R

            Dim reqListDT = rReader.LeggiRichieste(idReq, False, objParametri_Server)

            Dim reqParameters As New jDeereDataModel_FieldOperationFileRequest_Parameter
            Dim jdc As New jDeereController

            Dim row = rReader.LeggiDaFieldOperation(Integer.Parse(reqListDT.Rows(0)("FieldOperationID")), reqListDT.Rows(0)("FieldOperationGUID"), objParametri_Server).Rows(0)
            Dim currReq As New jDeereDataModel_FieldOperationFileRequest With {
                    .ID = row("ID"),
                    .FieldOperationID = row("FieldOperationID"),
                    .FieldOperationGUID = row("FieldOperationGUID"),
                    .Allegati_Documenti_Cod = row("Allegati_Documenti_Cod"),
                    .RequestState = row("RequestState"),
                    .Request_Parameters = row("Request_Parameters"),
                    .OAuth_Token = row("OAuth_Token"),
                    .Esito = row("Esito")
                }

            Dim fldOp = rfldOps.LeggiFieldOperation(currReq.FieldOperationID, objParametri_Server)

            If fldOp.Rows.Count() <= 0 Then
                Throw New Exception("Field Operation collegata alla file request non trovata. operazione interrotta")
            End If

            Dim rowFldOp = fldOp.Rows(0)

            Dim currFld As New jDeereDataModel_FieldOperation With {
                    .id = rowFldOp("ID"),
                    .fieldOperationType = rowFldOp("fieldOperationType"),
                    .adaptMachineType = rowFldOp("adaptMachineType"),
                    .cropSeason = rowFldOp("cropSeason"),
                    .startDate = rowFldOp("startDate"),
                    .endDate = rowFldOp("endDate"),
                    .guid = rowFldOp("guid"),
                    .Allegati_Documenti_Cod = rowFldOp("Allegati_Documenti_Cod"),
                    .FieldID = rowFldOp("FieldID"),
                    .OrganizationID = rowFldOp("OrganizationID")
                }

            'per ogni riga eseguire il download e registrare il contenuto nella tabella ellageti_documenti
            reqParameters = JsonConvert.DeserializeObject(Of jDeereDataModel_FieldOperationFileRequest_Parameter)(row("Request_Parameters"))
            Dim chk As New RispostaStandard


            'considerando le indicazioni della documentazione John Deere, uno shape file di produzione po
            Dim urlFile As String = url
            Dim startIdx As Integer = urlFile.LastIndexOf("/") + 1
            Dim endIdx As Integer = urlFile.LastIndexOf("?")
            Dim fileName As String = urlFile.Substring(startIdx, endIdx - startIdx)

            Dim bData = jdc.JDeereExecuteDownnloadShapeFile(reqParameters.oauth_cfg, url)
            If bData.RispostaStringa Is Nothing OrElse bData.RispostaStringa.Length <= 0 Then
                'dati non scaricati -> segnare errore
                currReq.Esito = "Download eseguito ma nessun dato ricevuto, verificare che non si tratti di uno shape file per preparazione terreno"
                currReq.RequestState = 3    'Errore
            Else
                'memorizzare i dati binari come allegato dell'operazione
                Dim rWriteFile = WriteProductionZipToDB(currReq.FieldOperationID, currReq.Allegati_Documenti_Cod, fileName, bData.RispostaStringa, objParametri_Server, objParametri_Utenti)
                If Not rWriteFile.RispostaOK Then
                    Throw New Exception(rWriteFile.Errore)
                Else
                    currReq.Allegati_Documenti_Cod = rWriteFile.RispostaStringa
                    currFld.Allegati_Documenti_Cod = rWriteFile.RispostaStringa
                End If

                'aggiornare la richiesta come eseguita correttamente
                currReq.Esito = "Download eseguito correttamente e file memorizzato negli allegati GIAS"
                currReq.RequestState = 2
            End If

            chk = AggiornaRichiesta(currReq, objParametri_Server)
            If Not chk.RispostaOK Then
                Throw New Exception(chk.Errore)
            End If

            'devo aggiornare anche la field operation riportando il codice allegato e sottomettere la generazione dello shape file di produzione per il GIS

            Dim jdR As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_FieldOperation_R

            Dim Inseriti As New List(Of jDeereDataModel_FieldOperation)
            Dim modificati As New List(Of jDeereDataModel_FieldOperation)

            leggiListaOperationDispoScriviDBAccodaArrList(jdR, currFld, Inseriti, modificati, objParametri_Server)


            Dim sInseriti As String = JsonConvert.SerializeObject(Inseriti)
            Dim sModificati As String = JsonConvert.SerializeObject(modificati)
            sInseriti = sInseriti.Replace("""id""", """ID""")
            sInseriti = sInseriti.Replace("""name""", """Name""")

            sModificati = sModificati.Replace("""id""", """ID""")
            sModificati = sModificati.Replace("""name""", """Name""")

            Dim biz As New AgronicaCoreAnagrafeBIZ.jDeereDataModelBIZ_FieldOperation_W

            Dim rvalScrittura As String =
                biz.Aggiorna_jDeereDataModelBIZ_FieldOperation(sInseriti, sModificati, "[]", objParametri_Server)

            If rvalScrittura <> "" Then
                Throw New Exception(rvalScrittura)
            End If

            ret.RispostaOK = True
            ret.RispostaStringa = currReq.Allegati_Documenti_Cod
        Catch ex As Exception
            ret.RispostaOK = False
            ret.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return ret
    End Function

    Public Function JDeereConvertShapeFile(idAllegato As Integer, ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri) As RispostaStandard
        Dim ret As New RispostaStandard
        Try
            Dim shpFile2GIS = New ShapeFileToAgronicaGis2012
            Dim resConv = shpFile2GIS.convertAllegatiDocumenti(idAllegato, objParametri_Server, objParametri_Utenti)
            If Not resConv.RispostaOK Then
                Throw New Exception(resConv.Errore)
            End If

            ret.RispostaOK = True
            ret.RispostaStringa = ""
        Catch ex As Exception
            ret.RispostaOK = False
            ret.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return ret
    End Function



    Public Function ElabDownloadShapeFileRequest(ByVal IDReq As Integer, ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri) As RispostaStandard
        Dim ret As New RispostaStandard
        Try

            Dim rReader = New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_FieldOperationFileRequest_R
            Dim rfldOps = New jDeereDataModelDAL_FieldOperation_R

            Dim reqListDT = rReader.LeggiRichieste(IDReq, True, objParametri_Server)

            Dim reqParameters As New jDeereDataModel_FieldOperationFileRequest_Parameter
            Dim jdc As New jDeereController

            Dim row = rReader.LeggiDaFieldOperation(Integer.Parse(reqListDT.Rows(0)("FieldOperationID")), reqListDT.Rows(0)("FieldOperationGUID"), objParametri_Server).Rows(0)
            Dim currReq As New jDeereDataModel_FieldOperationFileRequest With {
                .ID = row("ID"),
                .FieldOperationID = row("FieldOperationID"),
                .FieldOperationGUID = row("FieldOperationGUID"),
                .Allegati_Documenti_Cod = row("Allegati_Documenti_Cod"),
                .RequestState = row("RequestState"),
                .Request_Parameters = row("Request_Parameters"),
                .OAuth_Token = row("OAuth_Token"),
                .Esito = row("Esito")
            }

            Dim fldOp = rfldOps.LeggiFieldOperation(currReq.FieldOperationID, objParametri_Server)

            If fldOp.Rows.Count() <= 0 Then
                Throw New Exception("Field Operation collegata alla file request non trovata. operazione interrotta")
            End If

            Dim rowFldOp = fldOp.Rows(0)

            Dim currFld As New jDeereDataModel_FieldOperation With {
                .id = rowFldOp("ID"),
                .fieldOperationType = rowFldOp("fieldOperationType"),
                .adaptMachineType = rowFldOp("adaptMachineType"),
                .cropSeason = rowFldOp("cropSeason"),
                .startDate = rowFldOp("startDate"),
                .endDate = rowFldOp("endDate"),
                .guid = rowFldOp("guid"),
                .Allegati_Documenti_Cod = rowFldOp("Allegati_Documenti_Cod"),
                .FieldID = rowFldOp("FieldID"),
                .OrganizationID = rowFldOp("OrganizationID")
            }

            'per ogni riga eseguire il download e registrare il contenuto nella tabella ellageti_documenti
            reqParameters = JsonConvert.DeserializeObject(Of jDeereDataModel_FieldOperationFileRequest_Parameter)(row("Request_Parameters"))
            Dim chk As New RispostaStandard
            Dim nLoop As Integer = 0
            Dim nLoopTime As Integer = 0
            Dim breakCicleTimeMS As Integer = (30 * 60 * 1000)  ' equivalente di 30 minuti in millisecondi
            'considerando le indicazioni della documentazione John Deere, uno shape file di produzione po
            While nLoop <= 10
                'aggiorno lo stato della richiesta a 1 (elaborazione in corso)
                currReq.RequestState = 1
                chk = AggiornaRichiesta(currReq, objParametri_Server)
                If Not chk.RispostaOK Then
                    Throw New Exception(chk.Errore)
                End If
                chk = jdc.JDeereGetShapeFileFromOperationID(reqParameters.oauth_cfg, reqParameters.fieldOpGuid)
                If Not chk.RispostaOK Then
                    Exit While
                Else
                    If chk.RispostaStringa <> "" Then
                        Exit While
                    End If
                End If
                nLoop += 1
                'sleep di 5 secondi a salire
                Threading.Thread.Sleep(5000 * (2 ^ (nLoop - 1)))
            End While
            If Not chk.RispostaOK Then
                'chiamata di richiesta generazione shape file andata in errore, aggiornare la richiesta con l'esito
                currReq.Esito = chk.Errore
                currReq.RequestState = 3    'Errore
            Else
                If chk.RispostaStringa = "" Then
                    'uscito da loop con ok ma senza valorizzare l'url di download -> segnare errore con Esito="Tempo di attesa insufficiente per generare il file"
                    currReq.Esito = "Tempo di attesa insufficiente per generare il file"
                    currReq.RequestState = 3    'Errore
                Else
                    Dim urlFile As String = chk.RispostaStringa.Split("?")(0)
                    Dim startIdx As Integer = urlFile.LastIndexOf("/") + 1
                    Dim fileName As String = urlFile.Substring(startIdx, urlFile.Length - startIdx)

                    Dim bData = jdc.JDeereExecuteDownnloadShapeFile(reqParameters.oauth_cfg, chk.RispostaStringa)
                    If bData.RispostaStringa Is Nothing OrElse bData.RispostaStringa.Length <= 0 Then
                        'dati non scaricati -> segnare errore
                        currReq.Esito = "Download eseguito ma nessun dato ricevuto, verificare che non si tratti di uno shape file per preparazione terreno"
                        currReq.RequestState = 3    'Errore
                    Else
                        'memorizzare i dati binari come allegato dell'operazione
                        Dim rWriteFile = WriteProductionZipToDB(currReq.FieldOperationID, currReq.Allegati_Documenti_Cod, fileName, bData.RispostaStringa, objParametri_Server, objParametri_Utenti)
                        If Not rWriteFile.RispostaOK Then
                            Throw New Exception(rWriteFile.Errore)
                        Else
                            currReq.Allegati_Documenti_Cod = rWriteFile.RispostaStringa
                            currFld.Allegati_Documenti_Cod = rWriteFile.RispostaStringa
                        End If

                        'aggiornare la richiesta come eseguita correttamente
                        currReq.Esito = "Download eseguito correttamente e file memorizzato negli allegati GIAS"
                        currReq.RequestState = 2
                    End If
                End If
            End If

            chk = AggiornaRichiesta(currReq, objParametri_Server)
            If Not chk.RispostaOK Then
                Throw New Exception(chk.Errore)
            End If

            'devo aggiornare anche la field operation riportando il codice allegato e sottomettere la generazione dello shape file di produzione per il GIS

            Dim jdR As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_FieldOperation_R

            Dim Inseriti As New List(Of jDeereDataModel_FieldOperation)
            Dim modificati As New List(Of jDeereDataModel_FieldOperation)

            leggiListaOperationDispoScriviDBAccodaArrList(jdR, currFld, Inseriti, modificati, objParametri_Server)


            Dim sInseriti As String = JsonConvert.SerializeObject(Inseriti)
            Dim sModificati As String = JsonConvert.SerializeObject(modificati)
            sInseriti = sInseriti.Replace("""id""", """ID""")
            sInseriti = sInseriti.Replace("""name""", """Name""")

            sModificati = sModificati.Replace("""id""", """ID""")
            sModificati = sModificati.Replace("""name""", """Name""")

            Dim biz As New AgronicaCoreAnagrafeBIZ.jDeereDataModelBIZ_FieldOperation_W

            Dim rvalScrittura As String =
                biz.Aggiorna_jDeereDataModelBIZ_FieldOperation(sInseriti, sModificati, "[]", objParametri_Server)

            If rvalScrittura <> "" Then
                Throw New Exception(rvalScrittura)
            End If

            Dim shpFile2GIS = New ShapeFileToAgronicaGis2012
            Dim resConv = shpFile2GIS.convertAllegatiDocumenti(currReq.Allegati_Documenti_Cod, objParametri_Server, objParametri_Utenti)
            If Not resConv.RispostaOK Then
                Throw New Exception(resConv.Errore)
            End If

            ret.RispostaOK = True
            ret.RispostaStringa = currReq.Esito
        Catch ex As Exception
            ret.RispostaOK = False
            ret.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return ret
    End Function

    Public Function TESTElabDownloadShapeFileRequest(ByVal IDReq As Integer, ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri) As RispostaStandard
        Dim ret As New RispostaStandard
        Try

            Dim rReader = New jDeereDataModelDAL_FieldOperationFileRequest_R
            Dim rfldOps = New jDeereDataModelDAL_FieldOperation_R

            Dim chk As New RispostaStandard

            Dim reqListDT = rReader.LeggiRichieste(IDReq, True, objParametri_Server)

            Dim reqParameters As New jDeereDataModel_FieldOperationFileRequest_Parameter
            Dim jdc As New jDeereController

            Dim dtRequest = rReader.LeggiDaFieldOperation(Integer.Parse(reqListDT.Rows(0)("FieldOperationID")), reqListDT.Rows(0)("FieldOperationGUID"), objParametri_Server)
            If dtRequest.Rows.Count <= 0 Then
                Throw New Exception("Field Operation file request non trovata. operazione interrotta")
            End If

            Dim row = dtRequest.Rows(0)

            Dim currReq As New jDeereDataModel_FieldOperationFileRequest With {
                .ID = row("ID"),
                .FieldOperationID = row("FieldOperationID"),
                .FieldOperationGUID = row("FieldOperationGUID"),
                .Allegati_Documenti_Cod = row("Allegati_Documenti_Cod"),
                .RequestState = row("RequestState"),
                .Request_Parameters = row("Request_Parameters"),
                .OAuth_Token = row("OAuth_Token"),
                .Esito = row("Esito")
            }

            Dim fldOp = rfldOps.LeggiFieldOperation(currReq.FieldOperationID, objParametri_Server)

            If fldOp.Rows.Count() <= 0 Then
                Throw New Exception("Field Operation collegata alla file request non trovata. operazione interrotta")
            End If

            Dim rowFldOp = fldOp.Rows(0)

            Dim currFld As New jDeereDataModel_FieldOperation With {
                .id = rowFldOp("ID"),
                .fieldOperationType = rowFldOp("fieldOperationType"),
                .adaptMachineType = rowFldOp("adaptMachineType"),
                .cropSeason = rowFldOp("cropSeason"),
                .startDate = rowFldOp("startDate"),
                .endDate = rowFldOp("endDate"),
                .guid = rowFldOp("guid"),
                .Allegati_Documenti_Cod = rowFldOp("Allegati_Documenti_Cod")
            }

            'aggiorno lo stato della richiesta a 1 (elaborazione in corso)
            currReq.RequestState = 1
            chk = AggiornaRichiesta(currReq, objParametri_Server)


            'prendo il file da path fisso e lo carico come allegato_documenti
            Dim filePath = "C:\GIASLAN\File_Esportazioni\08498e71-0445-494b-9362-96aa51211511.zip"
            Dim fileName = Path.GetFileName(filePath)
            Dim fileContents = My.Computer.FileSystem.ReadAllBytes(filePath)

            Dim rWriteFile = WriteProductionZipToDB(currReq.FieldOperationID, currReq.Allegati_Documenti_Cod, fileName, fileContents, objParametri_Server, objParametri_Utenti)
            If rWriteFile.RispostaOK Then
                currReq.Allegati_Documenti_Cod = rWriteFile.RispostaStringa
                currFld.Allegati_Documenti_Cod = rWriteFile.RispostaStringa
            End If

            'aggiornare la richiesta come eseguita correttamente
            currReq.Esito = "Download eseguito correttamente e file memorizzato negli allegati GIAS"
            currReq.RequestState = 2

            chk = AggiornaRichiesta(currReq, objParametri_Server)
            If Not chk.RispostaOK Then
                Throw New Exception(chk.Errore)
            End If

            Dim jdR As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_FieldOperation_R

            Dim Inseriti As New List(Of jDeereDataModel_FieldOperation)
            Dim modificati As New List(Of jDeereDataModel_FieldOperation)

            currFld.id = currFld.guid

            leggiListaOperationDispoScriviDBAccodaArrList(jdR, currFld, Inseriti, modificati, objParametri_Server)


            Dim sInseriti As String = JsonConvert.SerializeObject(Inseriti)
            Dim sModificati As String = JsonConvert.SerializeObject(modificati)
            sInseriti = sInseriti.Replace("""id""", """ID""")
            sInseriti = sInseriti.Replace("""name""", """Name""")

            sModificati = sModificati.Replace("""id""", """ID""")
            sModificati = sModificati.Replace("""name""", """Name""")

            Dim biz As New AgronicaCoreAnagrafeBIZ.jDeereDataModelBIZ_FieldOperation_W

            Dim rvalScrittura As String =
                biz.Aggiorna_jDeereDataModelBIZ_FieldOperation(sInseriti, sModificati, "[]", objParametri_Server)

            If rvalScrittura <> "" Then
                Throw New Exception(rvalScrittura)
            End If

            Dim shpFile2GIS = New ShapeFileToAgronicaGis2012
            Dim resConv = shpFile2GIS.convertAllegatiDocumenti(currReq.Allegati_Documenti_Cod, objParametri_Server, objParametri_Utenti)
            If Not resConv.RispostaOK Then
                Throw New Exception(resConv.Errore)
            End If

            ret.RispostaOK = True
            ret.RispostaStringa = resConv.RispostaStringa
        Catch ex As Exception
            ret.RispostaOK = False
            ret.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return ret
    End Function


    Private Function WriteProductionZipToDB(idFieldOps As Integer, IDAllegato As Integer, jdFileName As String, jdFileData As Byte(), ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri) As RispostaStandard
        Dim messaggioErrore As String = ""

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim rval As New RispostaStandard

        Dim rFldOps = New jDeereDataModelDAL_FieldOperation_R
        Dim rFld = New jDeereDataModelDAL_Field_R
        Dim rEntGIAS = New jDeereDataModelDAL_EntitaGias_R

        Try

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(
                FlagConnessioneLocale,
                FlagTransazioneLocale,
                objParametri_Server)

            Dim fldID = rFldOps.LeggiFieldID(idFieldOps, objParametri_Server)

            Dim DT_GiasKey = rEntGIAS.LeggiChiaveGIASDaIDJD(enum_TipoEntitaJohnDeere.Field, fldID, "", "", objParametri_Server)

            If DT_GiasKey.Rows.Count() <= 0 Then
                Throw New Exception("Chiave GIAS da ID JD non trovata per la Field Operation" - idFieldOps.ToString())
            End If

            'scrivo l'allegato in GIAS. -> jdFileNamePath

            Dim Allegati_Documenti_NomeFile As String = ""

            Dim fileContents As Byte() = Nothing

            Dim ScriviFileSuDB As Boolean =
                RiportoFileZipSuDBLeggiImpostazioneSalvataggioFiles(objParametri_Utenti)

            If ScriviFileSuDB Then
                fileContents = jdFileData
                Allegati_Documenti_NomeFile = jdFileName
            End If

            Dim ScriviAllegatoDB As New AgronicaCoreAnagrafeBIZ.Allegati_Documenti_W
            Dim OUTPUT_Allegati_Documenti_Cod As Integer

            Dim Desc As String = "File produzione JD"

            Dim rvalAllegati As RispostaStandard
            If IDAllegato = 0 Then
                rvalAllegati = ScriviAllegatoDB.PrecisionFarmingScriviSuAllegati(
                    enum_CategorieDocumenti.PrecisionFarming_MappaProduzione,
                    Desc,
                    Allegati_Documenti_NomeFile,
                    objParametri_Server,
                    DT_GiasKey.Rows(0)("piva"),
                    Integer.Parse(DT_GiasKey.Rows(0)("sa_cod")),
                    Integer.Parse(DT_GiasKey.Rows(0)("appezza")),
                    0,
                    0,
                    "",
                    fileContents,
                    "",
                    OUTPUT_Allegati_Documenti_Cod
                 )
            Else
                rvalAllegati = ScriviAllegatoDB.PrecisionFarmingAggiornaAllegati(
                    enum_CategorieDocumenti.PrecisionFarming_MappaProduzione,
                    IDAllegato,
                    Desc,
                    Allegati_Documenti_NomeFile,
                    objParametri_Server,
                    DT_GiasKey.Rows(0)("piva"),
                    Integer.Parse(DT_GiasKey.Rows(0)("sa_cod")),
                    Integer.Parse(DT_GiasKey.Rows(0)("appezza")),
                    0,
                    0,
                    "",
                    fileContents,
                    ""
                 )
                OUTPUT_Allegati_Documenti_Cod = IDAllegato
            End If

            If Not rvalAllegati.RispostaOK Then
                Throw New Exception(rvalAllegati.Errore)
            End If

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la transazione e la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            rval.RispostaOK = True
            rval.RispostaStringa = OUTPUT_Allegati_Documenti_Cod

        Catch ex As Exception

            'Faccio il rollback della transazione
            If objParametri_Server.objTransazione IsNot Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            messaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)

            Dim Messaggio As String = ""
            If messaggioErrore <> "" Then
                Messaggio &= "ERR: Sono stati rilevati i seguenti errori : " & vbCrLf
                Messaggio &= "" & vbCrLf
                Messaggio &= messaggioErrore
                Messaggio &= "" & vbCrLf
                Messaggio &= "Ritentare il salvataggio dopo la correzione ..."
            End If

            rval.RispostaOK = False
            rval.Errore = Messaggio
        Finally
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
        End Try
        Return rval
    End Function

    Private Function RiportoFileZipSuDBLeggiImpostazioneSalvataggioFiles(ByVal objParametri_Utenti As AgronicaCoreParametri) As Boolean

        Dim xLetturaImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim dt As DataTable =
            xLetturaImpostazioni.Leggi(enum_Impostazioni_Utenti.SUPERUSER_DOCUMENTALE_SALVA_ALLEGATO_SU_DB, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

        Dim rval As Boolean
        If dt.Rows.Count > 0 Then
            rval = dt.Rows(0)("Impostazione_Valore_1")
        Else
            rval = True
        End If

        Return rval

    End Function


    Private Function AggiornaRichiesta(Richiesta As jDeereDataModel_FieldOperationFileRequest, ByVal objParametri_Server As AgronicaCoreParametri) As RispostaStandard
        Dim ret As New RispostaStandard
        Try
            Dim ListReq = New List(Of jDeereDataModel_FieldOperationFileRequest)
            ListReq.Add(Richiesta)

            ret = FieldOperationFileRequestScriviDB(ListReq, objParametri_Server)
        Catch ex As Exception
            ret.RispostaOK = False
            ret.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return ret
    End Function

End Class





