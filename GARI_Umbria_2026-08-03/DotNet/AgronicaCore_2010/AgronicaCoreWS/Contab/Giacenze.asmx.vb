Imports System.Web.Services
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Globalization
Imports InData.Agenda
Imports AgronicaCoreUtility
Imports AgronicaCoreModelsSTD.attivita
Imports AgronicaCoreModello
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreContabBIZ
Imports AgronicaCoreModello.AppHelper
Imports AgronicaCoreModelsSTD.Widgets
Imports AgronicaCoreDTOStd.InData.Widgets
Imports AgronicaCoreAnagrafeDAL

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
Public Class Giacenze
    Inherits System.Web.Services.WebService


    ''' <summary>
    ''' Ottiene elenco prodotti, sia da bacnhe dati che specifici per azienda
    ''' E' possibile ottenere solo i prodotti di una categoria o tutti
    ''' </summary>
    ''' <param name="piva"></param>
    ''' <param name="objP_super_server"></param>
    ''' <param name="objP_server">da tipi Enumerativi, Enum_SiteRedirector (100 per filtrare richieste da Gias APP)</param>
    ''' <param name="objP_utenti"></param>
    ''' <param name="tipo_aggregazione">passare 0 se si vogliono i campi distinti</param>
    ''' <param name="soloCampiApp">passare 1 per chiamata da APP</param>
    ''' <param name="sa_Cod">Sa_cod utilizzato per ricerca giacenza</param>
    ''' <param name="tipo_fabbricato_cod">Tipo magazzino utilizzato per ricerca giacenza</param>
    ''' <param name="fabbricato_Cod">Codice magazzino utilizzato per ricerca giacenza</param>
    ''' <param name="Elem_Cod">Se viene passato uno specifico Elem_Cod vengono letti solo i prodotti di quella categoria, altrimenti tutti i prodotti in base al campo metaschema</param>
    ''' <param name="pro_cod">Se viene passato uno specifico pro_cod vengono letti solo i prodotti con quel codice</param>
    ''' <param name="mat_cod">Se viene passato uno specifico mat_cod vengono letti solo i prodotti con quel codice</param>
    ''' <param name="lotto">filtro per lotto specifico</param>
    ''' <param name="Data_Movimento_Str">Data riferimento per ricerca giacenze</param>
    ''' <param name="isFreshAndFood">Flag che identifica se la ricerca è lanciata da F&F</param>
    ''' <param name="flag_QtaNoZero">Flag che identifica se le righe di giacenza a zero vanno mostrate</param>

    ''' <returns></returns>

    <WebMethod()>
    Public Function Leggi_Giacenze(ByVal piva As String,
                                   ByVal objP_super_server As String,
                                   ByVal objP_server As String,
                                   ByVal objP_utenti As String,
                                   ByVal tipo_aggregazione As Integer,
                                   ByVal soloCampiApp As Boolean,
                                   ByVal sa_cod As Integer,
                                   ByVal tipo_fabbricato_cod As Integer,
                                   ByVal fabbricato_cod As Integer,
                                   ByVal elem_cod As Integer,
                                   ByVal pro_cod As Integer,
                                   ByVal mat_cod As Integer,
                                   ByVal lotto As String,
                                   ByVal Data_Movimento_Str As String,
                                   ByVal isFreshAndFood As Boolean,
                                   ByVal flag_QtaNoZero As Boolean
                                   ) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim Data_Movimento As Date = AGRODATAFINE
            If Not String.IsNullOrEmpty(Data_Movimento_Str) Then

                'TODO: fare fix definitivo per data
                If Date.TryParseExact(Data_Movimento_Str, "dd/MM/yyyy",
                                      CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, Nothing) Then
                    Data_Movimento = Date.ParseExact(Data_Movimento_Str, "dd/MM/yyyy",
                                                     CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal)
                ElseIf IsDate(Data_Movimento_Str) Then
                    Data_Movimento = CDate(Data_Movimento_Str)
                End If

            End If

            Dim objParametri_SuperServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim ObjProdotti As New AgronicaCoreContabBIZ.FF_MagazzinoBIZ

            Dim dt = ObjProdotti.Leggi_Giacenze_Globale(tipo_aggregazione, soloCampiApp, piva, sa_cod, tipo_fabbricato_cod, fabbricato_cod, elem_cod, pro_cod, mat_cod, LOTTO_NONDEFINITO, 0, 0, Data_Movimento, isFreshAndFood, flag_QtaNoZero, "Prodotto_Des", objParametri_Server, objParametri_Utenti)


            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function Leggi_Movimenti(ByVal piva As String,
                                    ByVal objP_super_server As String,
                                    ByVal objP_server As String,
                                    ByVal objP_utenti As String,
                                    ByVal sa_cod As Integer,
                                    ByVal fabbricato_cod As Integer,
                                    ByVal elem_cod As Integer,
                                    ByVal cod_articolo As String,
                                    ByVal pro_cod As Integer,
                                    ByVal mat_cod As Integer,
                                    ByVal lotto As String,
                                    ByVal data_inizio As String,
                                    ByVal data_fine As String,
                                    ByVal flag_carichi As Boolean,
                                    ByVal flag_scarichi As Boolean) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim objParametri_SuperServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            If data_inizio = "" Then
                data_inizio = AGRODATAINIZIO.ToShortDateString
            End If

            If data_fine = "" Then
                data_fine = AGRODATAFINE.ToShortDateString
            End If


            'Carico e Scarico
            Dim filterCarichi As String = ""
            Dim filterScarichi As String = ""

            If flag_carichi Then
                filterCarichi = " Cau_Mov IN ('" & CAU_CARICO & "','" & CAU_CONFERIMENTO & "','" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "') "
            End If

            If flag_scarichi Then
                filterScarichi = " Cau_Mov IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "','" & CAU_ACCETTAZIONE_BENI & "') "
            End If

            Dim stringaFilter As String = ""
            If filterCarichi <> "" AndAlso filterScarichi <> "" Then
                stringaFilter &= " AND ( " & filterCarichi & " OR " & filterScarichi & " ) "
            ElseIf filterCarichi <> "" Then
                stringaFilter &= " AND ( " & filterCarichi & " ) "
            ElseIf filterScarichi <> "" Then
                stringaFilter &= " AND ( " & filterScarichi & " ) "
            End If

            Dim leggiAnagrafeLog As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
            Dim isFreshAndFood As Boolean = leggiAnagrafeLog.IsFreshAndFood(piva, elem_cod, objParametri_Server)

            Dim objMovimenti As New AgronicaCoreContabDAL.Movimenti_R
            Dim dtMovimenti As DataTable = objMovimenti.SchedaMovimentiMagazzino(CDate(data_inizio),
                                                                                 CDate(data_fine),
                                                                                 piva,
                                                                                 CInt(sa_cod),
                                                                                 fabbricato_cod,
                                                                                 CInt(elem_cod),
                                                                                 CInt(pro_cod),
                                                                                 CInt(mat_cod),
                                                                                 0,
                                                                                 0,
                                                                                 0,
                                                                                 0,
                                                                                 LOTTO_NONDEFINITO,
                                                                                 stringaFilter, "", "", "", "", "", "", "", "", "", "",
                                                                                 "Movimenti.Data_Movimento DESC",
                                                                                 objParametri_Server, objParametri_Utenti,
                                                                                 flagRecuperaCodArticolo:=True,
                                                                                 codArticolo:=If(cod_articolo, ""),
                                                                                 cercaCodArticoloPerLike:=True,
                                                                                 isFreshAndFood:=isFreshAndFood)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dtMovimenti, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function Scrivi_Movimenti(InData As Object) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim objAppHelper As New AppHelper
            Dim objAgendaHelper As New Agenda_Operazione_Helper
            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of List(Of MovimentoDiMagazzino)))(datiRequest)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objSincroHelper As New AgronicaCoreMapper.SincroAppHelper(objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
            Dim movimenti As List(Of MovimentoDiMagazzino) = objRequest.InData
            Dim response As New List(Of String)

            Dim importazioneDatiApp = objAppHelper.Leggi_ImportDati_APP(enum_Dati_App.Movimenti, objParametri_Utenti)
            Dim importazione = importazioneDatiApp = enum_Import_App.Completo

            For Each movimento In movimenti

                Dim piva As String = movimento.Magazzino.primaryKey.centroAziendalePK.partitaIva
                Dim codice As String = movimento.Codice
                Dim unid As String = movimento.guid
                Dim cancellato As Boolean = movimento.cancellato
                Dim aggiornamento As Boolean = False
                Dim riferimento As String = ""

                If String.IsNullOrEmpty(unid) Then
                    unid = Guid.NewGuid().ToString()
                Else
                    aggiornamento = True
                End If

                ' importazione dati app
                Dim dati As String = JsonConvert.SerializeObject(movimento)
                objSincroHelper.SincroDatiApp(unid, enum_Dati_App.Movimenti, dati, riferimento, aggiornamento, cancellato, importazione, piva, codice, origine:=enum_SistemiEsterni.GiasAPP)

                response.Add(unid)

            Next

            r.RispostaStringa = String.Join("|", response)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function


End Class