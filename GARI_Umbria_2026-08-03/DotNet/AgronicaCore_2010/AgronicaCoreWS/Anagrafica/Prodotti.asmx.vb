Imports System.Globalization
Imports System.Web.Services
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreMetaSchemaDAL
Imports AgronicaCoreModelsSTD.attivita.risorse
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreModelsSTD.attivita.dettagli
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreDTOStd.InData.Anagrafica
Imports AgronicaCoreModelsSTD.attivita
Imports AgronicaCoreDTOStd.InData.Metaschema
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreModelsSTD.exceptions

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
Public Class Prodotti
    Inherits System.Web.Services.WebService

    ''' <summary>
    ''' Ottiene elenco prodotti, sia da bacnhe dati che specifici per azienda
    ''' E' possibile ottenere solo i prodotti di una categoria o tutti
    ''' </summary>
    ''' <param name="objP_super_server"></param>
    ''' <param name="objP_server">da tipi Enumerativi, Enum_SiteRedirector (100 per filtrare richieste da Gias APP)</param>
    ''' <param name="objP_utenti"></param>
    ''' <param name="piva"></param>
    ''' <param name="xSa_Cod">Sa_cod utilizzato per ricerca giacenza</param>
    ''' <param name="xFabbricato_Cod">Codice magazzino utilizzato per ricerca giacenza</param>
    ''' <param name="xTipoDestinazione">Tipo magazzino utilizzato per ricerca giacenza</param>
    ''' <param name="Elem_Cod">Se viene passato uno specifico Elem_Cod vengono letti solo i prodotti di quella categoria, altrimenti tutti i prodotti in base al campo metaschema</param>
    ''' <param name="soloInGiacenza">Filtro solo prodotti in giacenza; utilizzato solo per causali di scarico</param>
    ''' <param name="FiltroDescrizioneProdotto">Testo da ricercare all'interno della descrizione prodotto</param>
    ''' <param name="Mode">vuoto o "trasferimento"</param>
    ''' <param name="Cau_Mov">Causale movimento</param>
    ''' <param name="Data_Movimento_Str">Data riferimento per ricerca giacenze</param>
    ''' <param name="xPUARegolamento">Regolamento PUA</param>
    ''' <param name="xLottoAccettazione">Lotto accettazione</param>
    ''' <param name="leggiUMformulati">Lettura U.M. formulati</param>
    ''' <param name="metaschema">Se il parametro metaschema vale "" vengono letti tutti i prodotti di ogni categoria
    '''                         Se il parametro metaschema vale "S" vengono letti solo i prodotti che non appartengono ad una specifica azienda
    '''                         Se il parametro metaschema vale "N" vengono letti solo i prodotti specifici per azienda</param>
    ''' <param name="xTipoPUARegolamento">Tipo Regolamento PUA</param>
    ''' <returns></returns>
    <WebMethod()>
    Public Function LeggiElencoCompletoProdotti_NG(ByVal InData As CoreWS_Generic(Of LeggiElencoCompletoProdotti)) As RispostaStandard

        Dim r As New RispostaStandard

        Dim arespevarList = NormalizzaSpecieVarieta(InData.InData.Elenco_Specie, InData.InData.Elenco_Varieta, InData.objP.objP_server)

        Dim dtProdottiGlobale As New DataTable
        Dim dtProdotti As New DataTable

        Dim leggiAlias = InData.InData.creaGriglia

        'Attualmente la gestione dei gruppi merce non è abilitata quando viene creata la grid, in altre parole è disabilitata nell'anagrafica prodotti
        Dim inibisciVisibilitaGruppiMerce As Boolean = False
        If InData.InData.creaGriglia Then
            inibisciVisibilitaGruppiMerce = True
        End If

        If arespevarList.Count = 0 Then
            dtProdottiGlobale = ElencoProdotti(InData.objP.objP_super_server,
                                               InData.objP.objP_server,
                                               InData.objP.objP_utenti,
                                               InData.InData.piva,
                                               InData.InData.xSa_Cod,
                                               InData.InData.xFabbricato_Cod,
                                               InData.InData.xTipoDestinazione,
                                               InData.InData.Elem_Cod,
                                               InData.InData.soloInGiacenza,
                                               InData.InData.FiltroDescrizioneProdotto,
                                               InData.InData.Mode,
                                               InData.InData.Cau_Mov,
                                               InData.InData.Data_Movimento_Str,
                                               InData.InData.xPUARegolamento,
                                               InData.InData.xLottoAccettazione,
                                               InData.InData.leggiUMformulati,
                                               InData.InData.metaschema,
                                               Nothing,
                                               InData.InData.Flag_QtaNoZero,
                                               False,
                                               InData.InData.xTipoPUARegolamento,
                                               False,
                                               0,
                                               0,
                                               InData.InData.xFiltroAggiuntivoMateriePrime,
                                               leggiAlias,
                                               InData.InData.flagDiversificaDesFertilizzanti,
                                               InData.InData.FiltroCodiceProdotto,
                                               InData.InData.FiltroCodiceTrappola,
                                               inibisciVisibilitaGruppiMerce)
        End If

        If arespevarList.Count > 0 Then

            For Each wSpeVar In arespevarList
                Dim speVar_Array As String() = wSpeVar.Split("|")
                Dim w_veg_cod = CInt(speVar_Array(0))
                Dim w_cul_cod = CInt(speVar_Array(1))
                dtProdotti = ElencoProdotti(InData.objP.objP_super_server,
                                            InData.objP.objP_server,
                                            InData.objP.objP_utenti,
                                            InData.InData.piva,
                                            InData.InData.xSa_Cod,
                                            InData.InData.xFabbricato_Cod,
                                            InData.InData.xTipoDestinazione,
                                            InData.InData.Elem_Cod,
                                            InData.InData.soloInGiacenza,
                                            InData.InData.FiltroDescrizioneProdotto,
                                            InData.InData.Mode,
                                            InData.InData.Cau_Mov,
                                            InData.InData.Data_Movimento_Str,
                                            InData.InData.xPUARegolamento,
                                            InData.InData.xLottoAccettazione,
                                            InData.InData.leggiUMformulati,
                                            InData.InData.metaschema,
                                            Nothing,
                                            InData.InData.Flag_QtaNoZero,
                                            False,
                                            InData.InData.xTipoPUARegolamento,
                                            False,
                                            w_veg_cod,
                                            w_cul_cod,
                                            InData.InData.xFiltroAggiuntivoMateriePrime,
                                            leggiAlias,
                                            InData.InData.flagDiversificaDesFertilizzanti,
                                            InData.InData.FiltroCodiceProdotto,
                                            InData.InData.FiltroCodiceTrappola,
                                            inibisciVisibilitaGruppiMerce)

                If dtProdotti.Rows.Count > 0 Then

                    If dtProdottiGlobale.Rows.Count = 0 Then
                        dtProdottiGlobale = dtProdotti.Clone
                    End If

                    For Each dr In dtProdotti.Rows
                        'Viene fatto il controllo anche qui perchè in caso di scarichi all'interno di ElencoProdotti non viene fatto il filtro per specie/varietà
                        If (w_cul_cod = 0 AndAlso
                            CInt(dr.Item("Veg_Cod")) = w_veg_cod) OrElse
                            (w_cul_cod <> 0 AndAlso
                            dr.Item("Veg_Cod") = w_veg_cod AndAlso
                            dr.Item("Cul_Cod") = w_cul_cod) Then
                            dtProdottiGlobale.ImportRow(dr)
                        End If
                    Next

                End If

            Next

            If dtProdottiGlobale.Rows.Count > 0 Then
                dtProdottiGlobale.DefaultView.Sort = "Prodotto_Des"
                dtProdottiGlobale = dtProdottiGlobale.DefaultView.ToTable()
            End If

        End If

        If Not InData.InData.creaGriglia Then
            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            r.RispostaStringa = JsonConvert.SerializeObject(dtProdottiGlobale, Formatting.None, serializerSettings)

        Else
            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            ImpostaDatatableProdotti(dtProdottiGlobale, l, InData.InData.piva, InData.InData.Elem_Cod, InData.InData.filtroProdottiValorizzati, objParametri_Server)

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = js.JSON_DataTable_Kendo(dtProdottiGlobale, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)
            r.RispostaStringa = risp

        End If

        r.RispostaOK = True
        Return r

    End Function

    <WebMethod()>
    Public Function LeggiElencoCompletoProdotti(ByVal objP_super_server As String,
                                                ByVal objP_server As String,
                                                ByVal objP_utenti As String,
                                                ByVal piva As String,
                                                ByVal xSa_Cod As Integer,
                                                ByVal xFabbricato_Cod As Integer,
                                                ByVal xTipoDestinazione As Integer,
                                                ByVal Elem_Cod As Integer,
                                                ByVal soloInGiacenza As Boolean,
                                                ByVal FiltroDescrizioneProdotto As String,
                                                ByVal Mode As String,
                                                ByVal Cau_Mov As String,
                                                ByVal Data_Movimento_Str As String,
                                                ByVal xPUARegolamento As Integer,
                                                ByVal xLottoAccettazione As String,
                                                ByVal leggiUMformulati As Boolean,
                                                ByVal metaschema As String,
                                                ByVal Flag_QtaNoZero As Boolean,
                                                ByVal xTipoPUARegolamento As Integer,
                                                ByVal Elenco_Specie As String,
                                                ByVal Elenco_Varieta As String,
                                                ByVal xFiltroAggiuntivoMateriePrime As String,
                                                ByVal creaGriglia As Boolean,
                                                ByVal filtroProdottiValorizzati As Integer,
                                                ByVal flagDiversificaDesFertilizzanti As Boolean,
                                                ByVal FiltroCodiceProdotto As Integer,
                                                ByVal FiltroCodiceTrappola As Integer
                                                ) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim arespevarList = NormalizzaSpecieVarieta(Elenco_Specie, Elenco_Varieta, objP_server)

            Dim dtProdottiGlobale As New DataTable
            Dim dtProdotti As New DataTable

            Dim leggiAlias = creaGriglia

            'Attualmente la gestione dei gruppi merce non è abilitata quando viene creata la grid, in altre parole è disabilitata nell'anagrafica prodotti
            Dim inibisciVisibilitaGruppiMerce As Boolean = False
            If creaGriglia Then
                inibisciVisibilitaGruppiMerce = True
            End If

            If arespevarList.Count = 0 Then
                dtProdottiGlobale = ElencoProdotti(objP_super_server,
                                                   objP_server,
                                                   objP_utenti,
                                                   piva,
                                                   xSa_Cod,
                                                   xFabbricato_Cod,
                                                   xTipoDestinazione,
                                                   Elem_Cod,
                                                   soloInGiacenza,
                                                   FiltroDescrizioneProdotto,
                                                   Mode,
                                                   Cau_Mov,
                                                   Data_Movimento_Str,
                                                   xPUARegolamento,
                                                   xLottoAccettazione,
                                                   leggiUMformulati,
                                                   metaschema,
                                                   Nothing,
                                                   Flag_QtaNoZero,
                                                   False,
                                                   xTipoPUARegolamento,
                                                   False,
                                                   0,
                                                   0,
                                                   xFiltroAggiuntivoMateriePrime,
                                                   leggiAlias,
                                                   flagDiversificaDesFertilizzanti,
                                                   FiltroCodiceProdotto,
                                                   FiltroCodiceTrappola,
                                                   inibisciVisibilitaGruppiMerce)
            End If

            If arespevarList.Count > 0 Then

                For Each wSpeVar In arespevarList
                    Dim speVar_Array As String() = wSpeVar.Split("|")
                    Dim w_veg_cod = CInt(speVar_Array(0))
                    Dim w_cul_cod = CInt(speVar_Array(1))
                    dtProdotti = ElencoProdotti(objP_super_server,
                                                objP_server,
                                                objP_utenti,
                                                piva,
                                                xSa_Cod,
                                                xFabbricato_Cod,
                                                xTipoDestinazione,
                                                Elem_Cod,
                                                soloInGiacenza,
                                                FiltroDescrizioneProdotto,
                                                Mode,
                                                Cau_Mov,
                                                Data_Movimento_Str,
                                                xPUARegolamento,
                                                xLottoAccettazione,
                                                leggiUMformulati,
                                                metaschema,
                                                Nothing,
                                                Flag_QtaNoZero,
                                                False,
                                                xTipoPUARegolamento,
                                                False,
                                                w_veg_cod,
                                                w_cul_cod,
                                                xFiltroAggiuntivoMateriePrime,
                                                leggiAlias,
                                                flagDiversificaDesFertilizzanti,
                                                FiltroCodiceProdotto,
                                                FiltroCodiceTrappola,
                                                inibisciVisibilitaGruppiMerce)

                    If dtProdotti.Rows.Count > 0 Then

                        If dtProdottiGlobale.Rows.Count = 0 Then
                            dtProdottiGlobale = dtProdotti.Clone
                        End If

                        For Each dr In dtProdotti.Rows
                            'Viene fatto il controllo anche qui perché in caso di scarichi all'interno di ElencoProdotti non viene fatto il filtro per specie/varietà
                            If (w_cul_cod = 0 AndAlso
                                CInt(dr.Item("Veg_Cod")) = w_veg_cod) OrElse
                                (w_cul_cod <> 0 AndAlso
                                dr.Item("Veg_Cod") = w_veg_cod AndAlso
                                dr.Item("Cul_Cod") = w_cul_cod) Then
                                dtProdottiGlobale.ImportRow(dr)
                            End If
                        Next

                    End If

                Next

                If dtProdottiGlobale.Rows.Count > 0 Then
                    dtProdottiGlobale.DefaultView.Sort = "Prodotto_Des"
                    dtProdottiGlobale = dtProdottiGlobale.DefaultView.ToTable()
                End If

            End If

            If Not creaGriglia Then
                Dim serializerSettings As New JsonSerializerSettings With {
                    .DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                    .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }

                r.RispostaStringa = JsonConvert.SerializeObject(dtProdottiGlobale, Formatting.None, serializerSettings)

            Else
                'creo la lista delle colonne da visualizzare
                Dim l As New List(Of ColonneNome)

                Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

                ImpostaDatatableProdotti(dtProdottiGlobale, l, piva, Elem_Cod, filtroProdottiValorizzati, objParametri_Server)

                Dim js As New AgronicaCoreDataProvider.JSON_DataTable
                Dim risp As String = js.JSON_DataTable_Kendo(dtProdottiGlobale, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)
                r.RispostaStringa = risp

            End If

            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            r.RispostaOK = False
            r.RispostaStringa = ""
            Return r
        End Try

        Return r

    End Function

    ''' <summary>
    ''' Ottiene elenco prodotti, sia da banche dati che specifici per azienda
    ''' E' possibile ottenere solo i prodotti di una categoria o tutti
    ''' </summary>
    ''' <param name="objP_super_server"></param>
    ''' <param name="objP_server">da tipi Enumerativi, Enum_SiteRedirector (100 per filtrare richieste da Gias APP)</param>
    ''' <param name="objP_utenti"></param>
    ''' <param name="piva"></param>
    ''' <param name="xSa_Cod">Sa_cod utilizzato per ricerca giacenza</param>
    ''' <param name="xFabbricato_Cod">Codice magazzino utilizzato per ricerca giacenza</param>
    ''' <param name="xTipoDestinazione">Tipo magazzino utilizzato per ricerca giacenza</param>
    ''' <param name="Elenco_Elem_Cod">Elenco degli Elem_Cod da filtrare</param>
    ''' <param name="Elenco_Specie">Elenco delle specie da filtrare</param>
    ''' <param name="Elenco_Varieta">Elenco delle varietà  da filtrare</param>
    ''' <param name="soloInGiacenza">Filtro solo prodotti in giacenza; utilizzato solo per causali di scarico</param>
    ''' <param name="FiltroDescrizioneProdotto">Testo da ricercare all'interno della descrizione prodotto</param>
    ''' <param name="Mode">vuoto o "trasferimento"</param>
    ''' <param name="Cau_Mov">Causale movimento</param>
    ''' <param name="Data_Movimento_Str">Data riferimento per ricerca giacenze</param>
    ''' <param name="xPUARegolamento">Regolamento PUA</param>
    ''' <param name="xLottoAccettazione">Lotto accettazione</param>
    ''' <param name="leggiUMformulati">Lettura U.M. formulati</param>
    ''' <param name="metaschema">Se il parametro metaschema vale "" vengono letti tutti i prodotti di ogni categoria
    '''                         Se il parametro metaschema vale "S" vengono letti solo i prodotti che non appartengono ad una specifica azienda
    '''                         Se il parametro metaschema vale "N" vengono letti solo i prodotti specifici per azienda</param>
    ''' <param name="xTipoPUARegolamento">Tipo Regolamento PUA</param>
    ''' <returns></returns>
    <WebMethod()>
    Public Function LeggiElencoCompletoProdottiMultiCategoria_NG(ByVal InData As CoreWS_Generic(Of LeggiElencoCompletoProdottiMultiCategoria)) As RispostaStandard

        Dim r As New RispostaStandard

        Dim spevarList = NormalizzaSpecieVarieta(InData.InData.Elenco_Specie, InData.InData.Elenco_Varieta, InData.objP.objP_server)

        Dim elem_cod_Array As String() = {}
        If Not String.IsNullOrEmpty(InData.InData.Elenco_Elem_Cod) Then
            elem_cod_Array = InData.InData.Elenco_Elem_Cod.Split("|")
        End If

        Dim dtProdottiGlobale As New DataTable
        Dim dtProdotti As New DataTable

        'Se non c'è filtro per specie / var forzo comunque una riga per poter fare ciclo sotto
        ' Fatto qui perchè comunque c'è il ciclo su Elem_Cod
        ' Non fatto invece su LeggiElencoCompletoProdotti per evitare di fare il ciclo sul DT se non ci sono filtri per spe/var
        If spevarList.Count = 0 Then
            spevarList.Add("0" & "|" & "0")
        End If

        For Each w_elem In elem_cod_Array

            For Each wSpeVar In spevarList
                Dim speVar_Array As String() = wSpeVar.Split("|")
                Dim w_veg_cod = CInt(speVar_Array(0))
                Dim w_cul_cod = CInt(speVar_Array(1))

                dtProdotti = ElencoProdotti(InData.objP.objP_super_server,
                                            InData.objP.objP_server,
                                             InData.objP.objP_utenti,
                                            InData.InData.piva,
                                            InData.InData.xSa_Cod,
                                            InData.InData.xFabbricato_Cod,
                                            InData.InData.xTipoDestinazione,
                                            CInt(w_elem),
                                            InData.InData.soloInGiacenza,
                                            InData.InData.FiltroDescrizioneProdotto,
                                            InData.InData.Mode,
                                            InData.InData.Cau_Mov,
                                            InData.InData.Data_Movimento_Str,
                                            InData.InData.xPUARegolamento,
                                            InData.InData.xLottoAccettazione,
                                            InData.InData.leggiUMformulati,
                                            InData.InData.metaschema,
                                            Nothing,
                                            InData.InData.Flag_QtaNoZero,
                                            False,
                                            InData.InData.xTipoPUARegolamento,
                                            False,
                                            w_veg_cod,
                                            w_cul_cod,
                                            InData.InData.xFiltroAggiuntivoMateriePrime,
                                            False,
                                            InData.InData.flagDiversificaDesFertilizzanti,
                                            0,
                                            0)

                If dtProdotti.Rows.Count > 0 Then

                    If dtProdottiGlobale.Rows.Count = 0 Then
                        dtProdottiGlobale = dtProdotti.Clone
                    End If

                    For Each dr In dtProdotti.Rows
                        If w_cul_cod = 0 AndAlso w_veg_cod = 0 Then
                            dtProdottiGlobale.ImportRow(dr)
                        Else
                            'Viene fatto il controllo anche qui perchè in caso di scarichi all'interno di ElencoProdotti non viene fatto il filtro per specie/varietà
                            If (w_cul_cod = 0 AndAlso
                            CInt(dr.Item("Veg_Cod")) = w_veg_cod) OrElse
                            (w_cul_cod <> 0 AndAlso
                            dr.Item("Veg_Cod") = w_veg_cod AndAlso
                            dr.Item("Cul_Cod") = w_cul_cod) Then
                                dtProdottiGlobale.ImportRow(dr)
                            End If
                        End If
                    Next

                End If
            Next
        Next

        If dtProdottiGlobale.Rows.Count > 0 Then
            dtProdottiGlobale.DefaultView.Sort = "Prodotto_Des"
            dtProdottiGlobale = dtProdottiGlobale.DefaultView.ToTable()
        End If

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        r.RispostaStringa = JsonConvert.SerializeObject(dtProdottiGlobale, Formatting.None, serializerSettings)

        r.RispostaOK = True

        Return r

    End Function
    <WebMethod()>
    Public Function LeggiElencoCompletoProdottiMultiCategoria(
                                                ByVal objP_super_server As String,
                                                ByVal objP_server As String,
                                                ByVal objP_utenti As String,
                                                ByVal piva As String,
                                                ByVal xSa_Cod As Integer,
                                                ByVal xFabbricato_Cod As Integer,
                                                ByVal xTipoDestinazione As Integer,
                                                ByVal Elenco_Elem_Cod As String,
                                                ByVal soloInGiacenza As Boolean,
                                                ByVal FiltroDescrizioneProdotto As String,
                                                ByVal Mode As String,
                                                ByVal Cau_Mov As String,
                                                ByVal Data_Movimento_Str As String,
                                                ByVal xPUARegolamento As Integer,
                                                ByVal xLottoAccettazione As String,
                                                ByVal leggiUMformulati As Boolean,
                                                ByVal metaschema As String,
                                                ByVal Flag_QtaNoZero As Boolean,
                                                ByVal xTipoPUARegolamento As Integer,
                                                ByVal Elenco_Specie As String,
                                                ByVal Elenco_Varieta As String,
                                                ByVal xFiltroAggiuntivoMateriePrime As String,
                                                ByVal flagDiversificaDesFertilizzanti As Boolean
                                                ) As RispostaStandard

        Dim r As New RispostaStandard

        Dim spevarList = NormalizzaSpecieVarieta(Elenco_Specie, Elenco_Varieta, objP_server)

        Dim elem_cod_Array As String() = {}
        If Not String.IsNullOrEmpty(Elenco_Elem_Cod) Then
            elem_cod_Array = Elenco_Elem_Cod.Split("|")
        End If

        Dim dtProdottiGlobale As New DataTable
        Dim dtProdotti As New DataTable

        'Se non c'è filtro per specie / var forzo comunque una riga per poter fare ciclo sotto
        ' Fatto qui perchè comunque c'è il ciclo su Elem_Cod
        ' Non fatto invece su LeggiElencoCompletoProdotti per evitare di fare il ciclo sul DT se non ci sono filtri per spe/var
        If spevarList.Count = 0 Then
            spevarList.Add("0" & "|" & "0")
        End If

        For Each w_elem In elem_cod_Array

            For Each wSpeVar In spevarList
                Dim speVar_Array As String() = wSpeVar.Split("|")
                Dim w_veg_cod = CInt(speVar_Array(0))
                Dim w_cul_cod = CInt(speVar_Array(1))

                dtProdotti = ElencoProdotti(objP_super_server,
                                            objP_server,
                                            objP_utenti,
                                            piva,
                                            xSa_Cod,
                                            xFabbricato_Cod,
                                            xTipoDestinazione,
                                            CInt(w_elem),
                                            soloInGiacenza,
                                            FiltroDescrizioneProdotto,
                                            Mode,
                                            Cau_Mov,
                                            Data_Movimento_Str,
                                            xPUARegolamento,
                                            xLottoAccettazione,
                                            leggiUMformulati,
                                            metaschema,
                                            Nothing,
                                            Flag_QtaNoZero,
                                            False,
                                            xTipoPUARegolamento,
                                            False,
                                            w_veg_cod,
                                            w_cul_cod,
                                            xFiltroAggiuntivoMateriePrime,
                                            False,
                                            flagDiversificaDesFertilizzanti,
                                            0,
                                            0)

                If dtProdotti.Rows.Count > 0 Then

                    If dtProdottiGlobale.Rows.Count = 0 Then
                        dtProdottiGlobale = dtProdotti.Clone
                    End If

                    For Each dr In dtProdotti.Rows
                        If w_cul_cod = 0 AndAlso w_veg_cod = 0 Then
                            dtProdottiGlobale.ImportRow(dr)
                        Else
                            'Viene fatto il controllo anche qui perchè in caso di scarichi all'interno di ElencoProdotti non viene fatto il filtro per specie/varietà
                            If (w_cul_cod = 0 AndAlso
                            CInt(dr.Item("Veg_Cod")) = w_veg_cod) OrElse
                            (w_cul_cod <> 0 AndAlso
                            dr.Item("Veg_Cod") = w_veg_cod AndAlso
                            dr.Item("Cul_Cod") = w_cul_cod) Then
                                dtProdottiGlobale.ImportRow(dr)
                            End If
                        End If
                    Next

                End If
            Next
        Next

        If dtProdottiGlobale.Rows.Count > 0 Then
            dtProdottiGlobale.DefaultView.Sort = "Prodotto_Des"
            dtProdottiGlobale = dtProdottiGlobale.DefaultView.ToTable()
        End If

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        r.RispostaStringa = JsonConvert.SerializeObject(dtProdottiGlobale, Formatting.None, serializerSettings)

        r.RispostaOK = True

        Return r

    End Function
    ''' <summary>
    ''' Ottiene elenco prodotti, sia da bacnhe dati che specifici per azienda
    ''' E' possibile ottenere solo i prodotti di una categoria o tutti
    ''' </summary>
    ''' <param name="objP_super_server"></param>
    ''' <param name="objP_server">da tipi Enumerativi, Enum_SiteRedirector (100 per filtrare richieste da Gias APP)</param>
    ''' <param name="objP_utenti"></param>
    ''' <param name="piva"></param>
    ''' <param name="Elem_Cod">Se viene passato uno specifico Elem_Cod vengono letti solo i prodotti di quella categoria, altrimenti tutti i prodotti in base al campo metaschema</param>
    ''' <param name="Data_Movimento_Str">Data riferimento per ricerca giacenze</param>
    ''' <param name="metaschema">Se il parametro metaschema vale "" vengono letti tutti i prodotti di ogni categoria
    '''                         Se il parametro metaschema vale "S" vengono letti solo i prodotti che non appartengono ad una specifica azienda
    '''                         Se il parametro metaschema vale "N" vengono letti solo i prodotti specifici per azienda</param>
    ''' <returns></returns>
    <WebMethod()>
    Public Function LeggiElencoCompletoProdotti_APP(ByVal objP_super_server As String,
                                                    ByVal objP_server As String,
                                                    ByVal objP_utenti As String,
                                                    ByVal piva As String,
                                                    ByVal Elem_Cod As Integer,
                                                    ByVal Data_Movimento_Str As String,
                                                    ByVal metaschema As String,
                                                    ByVal soloInGiacenza As Boolean,
                                                    ByVal Flag_QtaNoZero As Boolean
                                                    ) As RispostaStandard

        Dim r As New RispostaStandard

        Dim elemcod_banchedati_sologiacenza As Integer() = {FERTILIZZANTI}

        Dim dtProdotti As DataTable = ElencoProdotti(objP_super_server,
                                                     objP_server,
                                                     objP_utenti,
                                                     piva,
                                                     0,
                                                     0,
                                                     0,
                                                     Elem_Cod,
                                                     soloInGiacenza,
                                                     "",
                                                     "",
                                                     CAU_CARICO,
                                                     Data_Movimento_Str,
                                                     0,
                                                     "",
                                                     False,
                                                     metaschema,
                                                     elemcod_banchedati_sologiacenza,
                                                     Flag_QtaNoZero,
                                                     True,
                                                     0,
                                                     False,
                                                     0,
                                                     0,
                                                     "",
                                                     False,
                                                     False,
                                                     0,
                                                     0)

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        r.RispostaStringa = JsonConvert.SerializeObject(dtProdotti, Formatting.None, serializerSettings)

        r.RispostaOK = True

        Return r

    End Function

    ''' <summary>
    ''' Ottiene elenco prodotti per APP, sia da banche dati che specifici per azienda
    ''' E' possibile ottenere solo i prodotti di una categoria o tutti
    ''' </summary>
    ''' <param name="objP_super_server"></param>
    ''' <param name="objP_server">da tipi Enumerativi, Enum_SiteRedirector (100 per filtrare richieste da Gias APP)</param>
    ''' <param name="objP_utenti"></param>
    ''' <param name="piva">Piva azienda se non indicata leggo prodotti banche dati</param>
    ''' <param name="Elem_Cod">Se viene passato uno specifico Elem_Cod vengono letti solo i prodotti di quella categoria, altrimenti tutti i prodotti in base al campo metaschema</param>
    ''' <param name="Data_Movimento_Str">Data riferimento per ricerca giacenze</param>
    ''' <param name="FiltroDescrizioneProdotto">Testo da ricercare all'interno della descrizione prodotto</param>
    ''' <param name="Veg_Cod">Filtro per specie</param>
    ''' <param name="Magazzino">Prodotti magazzino (1=SI, 0=NO)</param>
    ''' <returns></returns>
    <WebMethod()>
    Public Function LeggiElencoProdotti_APP(ByVal objP_super_server As String,
                                                    ByVal objP_server As String,
                                                    ByVal objP_utenti As String,
                                                    ByVal piva As String,
                                                    ByVal Elem_Cod As Integer,
                                                    ByVal Data_Movimento_Str As String,
                                                    ByVal FiltroDescrizioneProdotto As String,
                                                    ByVal Veg_Cod As Integer,
                                                    ByVal Magazzino As Integer
                                                    ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim metaschema As String = If(Elem_Cod = SEMENTI, "N", "S")
        Dim soloInGiacenza As Boolean = True
        Dim Flag_QtaNoZero As Boolean = True
        Dim elemcod_banchedati_sologiacenza As Integer() = {FERTILIZZANTI}

        ' lettura prodotti senza magazzino
        If Magazzino = 0 Then
            soloInGiacenza = False
            Flag_QtaNoZero = False
            elemcod_banchedati_sologiacenza = Nothing
        End If

        ' converto testo da cercare in stringa json
        If Not String.IsNullOrEmpty(FiltroDescrizioneProdotto) Then
            Dim filtro As New JArray From {
                New JObject From {
                    New JProperty("value", FiltroDescrizioneProdotto)
                }
            }
            FiltroDescrizioneProdotto = JsonConvert.SerializeObject(filtro)
        End If

        Dim dtProdotti As DataTable = ElencoProdotti(objP_super_server,
                                                     objP_server,
                                                     objP_utenti,
                                                     piva,
                                                     0,
                                                     0,
                                                     0,
                                                     Elem_Cod,
                                                     soloInGiacenza,
                                                     FiltroDescrizioneProdotto,
                                                     "",
                                                     CAU_CARICO,
                                                     Data_Movimento_Str,
                                                     0,
                                                     "",
                                                     False,
                                                     metaschema,
                                                     elemcod_banchedati_sologiacenza,
                                                     Flag_QtaNoZero,
                                                     True,
                                                     0,
                                                     False,
                                                     Veg_Cod,
                                                     0,
                                                     "",
                                                     False,
                                                     False,
                                                     0,
                                                     0)

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        r.RispostaStringa = JsonConvert.SerializeObject(dtProdotti, Formatting.None, serializerSettings)
        r.RispostaOK = True

        Return r

    End Function
    <WebMethod()>
    Public Function LeggiElencoProdotti_APP_NG(ByVal InData As CoreWS_Generic(Of LeggiElencoProdotti_APP)) As RispostaStandard

        Dim r As New RispostaStandard
        Dim metaschema As String = If(InData.InData.Elem_Cod = SEMENTI, "N", "S")
        Dim soloInGiacenza As Boolean = True
        Dim Flag_QtaNoZero As Boolean = True
        Dim elemcod_banchedati_sologiacenza As Integer() = {FERTILIZZANTI}

        ' lettura prodotti senza magazzino
        If MAGAZZINO = 0 Then
            soloInGiacenza = False
            Flag_QtaNoZero = False
            elemcod_banchedati_sologiacenza = Nothing
        End If

        ' converto testo da cercare in stringa json
        If Not String.IsNullOrEmpty(InData.InData.FiltroDescrizioneProdotto) Then
            Dim filtro As New JArray From {
                New JObject From {
                    New JProperty("value", InData.InData.FiltroDescrizioneProdotto)
                }
            }
            InData.InData.FiltroDescrizioneProdotto = JsonConvert.SerializeObject(filtro)
        End If

        ' converto testo da cercare in stringa json
        If Not String.IsNullOrEmpty(InData.InData.FiltroDescrizioneProdotto) Then
            Dim filtro As New JArray From {
                New JObject From {
                    New JProperty("value", InData.InData.FiltroDescrizioneProdotto)
                }
            }
            InData.InData.FiltroDescrizioneProdotto = JsonConvert.SerializeObject(filtro)
        End If

        Dim dtProdotti As DataTable = ElencoProdotti(InData.objP.objP_super_server,
                                                     InData.objP.objP_server,
                                                     InData.objP.objP_utenti,
                                                     InData.InData.piva,
                                                     0,
                                                     0,
                                                     0,
                                                     InData.InData.Elem_Cod,
                                                     soloInGiacenza,
                                                     InData.InData.FiltroDescrizioneProdotto,
                                                     "",
                                                     CAU_CARICO,
                                                     InData.InData.Data_Movimento_Str,
                                                     0,
                                                     "",
                                                     False,
                                                     metaschema,
                                                     elemcod_banchedati_sologiacenza,
                                                     Flag_QtaNoZero,
                                                     True,
                                                     0,
                                                     False,
                                                     InData.InData.Veg_Cod,
                                                     0,
                                                     "",
                                                     False,
                                                     False,
                                                     0,
                                                     0)

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        r.RispostaStringa = JsonConvert.SerializeObject(dtProdotti, Formatting.None, serializerSettings)
        r.RispostaOK = True

        Return r

    End Function

    ''' <summary>
    ''' Ottiene elenco prodotti, sia da banche dati che specifici per azienda
    ''' E' possibile ottenere solo i prodotti di una categoria o tutti
    ''' </summary>
    ''' <param name="objP_super_server"></param>
    ''' <param name="objP_server">da tipi Enumerativi, Enum_SiteRedirector (100 per filtrare richieste da Gias APP)</param>
    ''' <param name="objP_utenti"></param>
    ''' <param name="piva"></param>
    ''' <param name="Elem_Cod">Se viene passato uno specifico Elem_Cod vengono letti solo i prodotti di quella categoria, altrimenti tutti i prodotti in base al campo metaschema</param>
    ''' <param name="FiltroDescrizioneProdotto">Se viene passato uno specifico Elem_Cod vengono letti solo i prodotti di quella categoria, altrimenti tutti i prodotti in base al campo metaschema</param>
    ''' <param name="Elenco_Specie">Elenco Specie da filtrare separate da | </param>
    ''' <returns></returns>
    <WebMethod()>
    Public Function RicercaProdottiSenzaGiacenza_APP(ByVal objP_super_server As String,
                                                    ByVal objP_server As String,
                                                    ByVal objP_utenti As String,
                                                    ByVal piva As String,
                                                    ByVal Elem_Cod As Integer,
                                                    ByVal FiltroDescrizioneProdotto As String,
                                                    ByVal Elenco_Specie As String
                                                    ) As RispostaStandard

        Return RicercaProdottiSenzaGiacenza(
            objP_super_server, objP_server, objP_utenti, piva,
            Elem_Cod, FiltroDescrizioneProdotto, Elenco_Specie, "", "IT", 0)

    End Function

    ''' <summary>
    ''' Ottiene elenco prodotti, sia da banche dati che specifici per azienda
    ''' E' possibile ottenere solo i prodotti di una categoria o tutti
    ''' </summary>
    ''' <param name="objP_super_server"></param>
    ''' <param name="objP_server">da tipi Enumerativi, Enum_SiteRedirector (100 per filtrare richieste da Gias APP)</param>
    ''' <param name="objP_utenti"></param>
    ''' <param name="piva"></param>
    ''' <param name="Elem_Cod">Se viene passato uno specifico Elem_Cod vengono letti solo i prodotti di quella categoria, altrimenti tutti i prodotti in base al campo metaschema</param>
    ''' <param name="FiltroDescrizioneProdotto">Se viene passato uno specifico Elem_Cod vengono letti solo i prodotti di quella categoria, altrimenti tutti i prodotti in base al campo metaschema</param>
    ''' <param name="Elenco_Specie">Elenco Specie da filtrare separate da | </param>
    ''' <param name="Stato_Cod">Codice nazione usato per filtrare prodotti banche dati </param>
    ''' <returns></returns>
    <WebMethod()>
    Public Function RicercaProdottiSenzaGiacenza(ByVal objP_super_server As String,
                                                    ByVal objP_server As String,
                                                    ByVal objP_utenti As String,
                                                    ByVal piva As String,
                                                    ByVal Elem_Cod As Integer,
                                                    ByVal FiltroDescrizioneProdotto As String,
                                                    ByVal Elenco_Specie As String,
                                                    ByVal Elenco_Codici As String,
                                                    ByVal Stato_Cod As String,
                                                    ByVal Lav_Cod As Integer
                                                    ) As RispostaStandard

        Dim r As New RispostaStandard

        Dim Data_Movimento_Str As String = Date.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)

        Dim spevarList = NormalizzaSpecieVarieta(Elenco_Specie, "", objP_server)

        Dim dtProdottiGlobale As New DataTable
        Dim dtProdotti As New DataTable

        If String.IsNullOrEmpty(Stato_Cod) Then
            Stato_Cod = "IT"
        End If

        If Not String.IsNullOrEmpty(Elenco_Codici) Then

            Dim codici As String() = Elenco_Codici.Split("|")

            For Each codice In codici

                dtProdottiGlobale.Merge(ElencoProdotti(objP_super_server,
                                                    objP_server,
                                                    objP_utenti,
                                                    piva,
                                                    0,
                                                    0,
                                                    0,
                                                    Elem_Cod,
                                                    False,
                                                    "",
                                                    "",
                                                    CAU_CARICO,
                                                    Data_Movimento_Str,
                                                    0,
                                                    "",
                                                    False,
                                                    "",
                                                    Nothing,
                                                    False,
                                                    True,
                                                    0,
                                                    False,
                                                    0,
                                                    0,
                                                    "",
                                                    False,
                                                    False,
                                                    CInt(codice),
                                                    0,
                                                    Stato_Cod:=Stato_Cod,
                                                    Lav_Cod:=Lav_Cod))
            Next

        ElseIf spevarList.Count = 0 Then

            dtProdottiGlobale = ElencoProdotti(objP_super_server,
                                                    objP_server,
                                                    objP_utenti,
                                                    piva,
                                                    0,
                                                    0,
                                                    0,
                                                    Elem_Cod,
                                                    False,
                                                    FiltroDescrizioneProdotto,
                                                    "",
                                                    CAU_CARICO,
                                                    Data_Movimento_Str,
                                                    0,
                                                    "",
                                                    False,
                                                    "",
                                                    Nothing,
                                                    False,
                                                    True,
                                                    0,
                                                    False,
                                                    0,
                                                    0,
                                                    "",
                                                    False,
                                                    False,
                                                    0,
                                                    0,
                                                    Stato_Cod:=Stato_Cod,
                                                    Lav_Cod:=Lav_Cod)

        ElseIf spevarList.Count > 0 Then

            For Each wSpeVar In spevarList
                Dim speVar_Array As String() = wSpeVar.Split("|")
                Dim w_veg_cod = CInt(speVar_Array(0))
                Dim w_cul_cod = CInt(speVar_Array(1))
                dtProdotti = ElencoProdotti(objP_super_server,
                                                objP_server,
                                                objP_utenti,
                                                piva,
                                                0,
                                                0,
                                                0,
                                                Elem_Cod,
                                                False,
                                                FiltroDescrizioneProdotto,
                                                "",
                                                CAU_CARICO,
                                                Data_Movimento_Str,
                                                0,
                                                "",
                                                False,
                                                "",
                                                Nothing,
                                                False,
                                                True,
                                                0,
                                                False,
                                                w_veg_cod,
                                                w_cul_cod,
                                                "",
                                                False,
                                                False,
                                                0,
                                                0,
                                                Stato_Cod:=Stato_Cod,
                                                Lav_Cod:=Lav_Cod)

                If dtProdotti.Rows.Count > 0 Then

                    If dtProdottiGlobale.Rows.Count = 0 Then
                        dtProdottiGlobale = dtProdotti.Clone
                    End If

                    For Each dr In dtProdotti.Rows
                        dtProdottiGlobale.ImportRow(dr)
                    Next

                End If

            Next

            If dtProdottiGlobale.Rows.Count > 0 Then
                dtProdottiGlobale.DefaultView.Sort = "Prodotto_Des"
                dtProdottiGlobale = dtProdottiGlobale.DefaultView.ToTable()
            End If

        End If

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        r.RispostaStringa = JsonConvert.SerializeObject(dtProdottiGlobale, Formatting.None, serializerSettings)

        r.RispostaOK = True

        Return r

    End Function

    '######################################################################################################################
#Region "Metodi normali"

    '#########################################################################################
    'chiama AgronicaCoreVarieBIZ.CaricaListControl_2010.Prodotti
    'che è la combo usata nella FormProdotto (sia in modalità carico che in modalità scarico)
    'e nel filtro stampe di magazzino
    '
    'se viene passato RicercaTestoJArray, RicercaTesto viene popolato da quello
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Prodotti_x_CAC(ByVal objP_server As String, ByVal objP_utenti As String, ByVal objP_super_server As String,
                             ByVal Cau_Mov As String,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Integer,
                            ByVal Id_Destinazione As Integer,
                            ByVal Elem_Cod As Integer,
                            ByVal Flag_Negativo As Boolean,
                            ByVal RicercaTesto As String,
                            ByVal RicercaTestoJArray As String,
                            ByVal Flag_VisualizzaProCod As Boolean,
                            ByVal Flag_CaricaUdmCod As Boolean,
                            ByVal Pro_Cod As Integer,
                            ByVal Udm_Cod As Integer,
                            ByVal DataFiltroFormulati As Date,
                            ByVal PUA_RegolamentoCod As Integer,
                            ByVal TipoRichiesto As Integer,
                            ByVal Flag_LeggiGiacenze As Boolean,
                            ByVal Flag_FiltraRevocati As Boolean,
                            ByVal RegolamentoCod_Operazioni As Integer,
                            ByVal Tipo_PuaRegolamento As Integer,
                            ByVal Flag_IncludiNPK_Desc As Boolean,
                            ByVal Flag_IncludiClassificazione As Boolean,
                            ByVal Flag_QtaNoZero As Boolean,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal Flag_Filtra_MateriePrime_Per_Piva As Boolean,
                            ByVal Flag_Filtra_MateriePrime_Pubblici As Boolean,
                            ByVal Flag_CodArticolo_In_Descrizione As Boolean
        ) As RispostaStandard

        Return Prodotti_Internal(objP_server, objP_utenti, objP_super_server,
                                 Cau_Mov, Piva, Sa_Cod, Id_Destinazione, Elem_Cod, Flag_Negativo,
                                 RicercaTesto, RicercaTestoJArray, Flag_VisualizzaProCod, Flag_CaricaUdmCod,
                                 Pro_Cod, Udm_Cod, DataFiltroFormulati, PUA_RegolamentoCod,
                                 TipoRichiesto, Flag_LeggiGiacenze, Flag_FiltraRevocati,
                                 RegolamentoCod_Operazioni, Tipo_PuaRegolamento, Flag_IncludiNPK_Desc,
                                 Flag_IncludiClassificazione, Flag_QtaNoZero, xFiltroAggiuntivo,
                                 Flag_Filtra_MateriePrime_Per_Piva,
                                 Flag_Filtra_MateriePrime_Pubblici,
                                 Flag_CodArticolo_In_Descrizione
                            )


    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Prodotti_x_CAC_NG(ByVal InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Anagrafica.Prodotti_x_CAC)) As RispostaStandard

        Return Prodotti_Internal(InData.objP.objP_server, InData.objP.objP_utenti, InData.objP.objP_super_server,
                                 InData.InData.Cau_Mov, InData.InData.Piva, InData.InData.Sa_Cod, InData.InData.Id_Destinazione, InData.InData.Elem_Cod, InData.InData.Flag_Negativo,
                                 InData.InData.RicercaTesto, InData.InData.RicercaTestoJArray, InData.InData.Flag_VisualizzaProCod, InData.InData.Flag_CaricaUdmCod,
                                 InData.InData.Pro_Cod, InData.InData.Udm_Cod, InData.InData.DataFiltroFormulati, InData.InData.PUA_RegolamentoCod,
                                 InData.InData.TipoRichiesto, InData.InData.Flag_LeggiGiacenze, InData.InData.Flag_FiltraRevocati,
                                 InData.InData.RegolamentoCod_Operazioni, InData.InData.Tipo_PuaRegolamento, InData.InData.Flag_IncludiNPK_Desc,
                                 InData.InData.Flag_IncludiClassificazione, InData.InData.Flag_QtaNoZero, InData.InData.xFiltroAggiuntivo,
                                 InData.InData.Flag_Filtra_MateriePrime_Per_Piva, InData.InData.Flag_Filtra_MateriePrime_Pubblici, InData.InData.Flag_CodArticolo_In_Descrizione)

    End Function

    '#########################################################################################
    'chiama AgronicaCoreVarieBIZ.CaricaListControl_2010.Prodotti
    'che è la combo usata nella FormProdotto (sia in modalità carico che in modalità scarico)
    'e nel filtro stampe di magazzino
    '
    'se viene passato RicercaTestoJArray, RicercaTesto viene popolato da quello
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Prodotti(ByVal objP_server As String, ByVal objP_utenti As String, ByVal objP_super_server As String,
                             ByVal Cau_Mov As String,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Integer,
                            ByVal Id_Destinazione As Integer,
                            ByVal Elem_Cod As Integer,
                            ByVal Flag_Negativo As Boolean,
                            ByVal RicercaTesto As String,
                            ByVal RicercaTestoJArray As String,
                            ByVal Flag_VisualizzaProCod As Boolean,
                            ByVal Flag_CaricaUdmCod As Boolean,
                            ByVal Pro_Cod As Integer,
                            ByVal Udm_Cod As Integer,
                            ByVal DataFiltroFormulati As Date,
                            ByVal PUA_RegolamentoCod As Integer,
                            ByVal TipoRichiesto As Integer,
                            ByVal Flag_LeggiGiacenze As Boolean,
                            ByVal Flag_FiltraRevocati As Boolean,
                            ByVal RegolamentoCod_Operazioni As Integer,
                            ByVal Tipo_PuaRegolamento As Integer,
                            ByVal Flag_IncludiNPK_Desc As Boolean,
                            ByVal Flag_IncludiClassificazione As Boolean,
                            ByVal Flag_QtaNoZero As Boolean,
                            ByVal xFiltroAggiuntivo As String) As RispostaStandard

        Return Prodotti_Internal(objP_server, objP_utenti, objP_super_server,
                                 Cau_Mov, Piva, Sa_Cod, Id_Destinazione, Elem_Cod, Flag_Negativo,
                                 RicercaTesto, RicercaTestoJArray, Flag_VisualizzaProCod, Flag_CaricaUdmCod,
                                 Pro_Cod, Udm_Cod, DataFiltroFormulati, PUA_RegolamentoCod,
                                 TipoRichiesto, Flag_LeggiGiacenze, Flag_FiltraRevocati,
                                 RegolamentoCod_Operazioni, Tipo_PuaRegolamento, Flag_IncludiNPK_Desc,
                                 Flag_IncludiClassificazione, Flag_QtaNoZero, xFiltroAggiuntivo,
                                 False,
                                 False,
                                 Flag_CodArticolo_In_Descrizione:=False)

    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Prodotti_NG(ByVal InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Anagrafica.Prodotti)) As RispostaStandard

        Return Prodotti_Internal(InData.objP.objP_server, InData.objP.objP_utenti, InData.objP.objP_super_server,
                                 InData.InData.Cau_Mov, InData.InData.Piva, InData.InData.Sa_Cod, InData.InData.Id_Destinazione, InData.InData.Elem_Cod, InData.InData.Flag_Negativo,
                                 InData.InData.RicercaTesto, InData.InData.RicercaTestoJArray, InData.InData.Flag_VisualizzaProCod, InData.InData.Flag_CaricaUdmCod,
                                 InData.InData.Pro_Cod, InData.InData.Udm_Cod, InData.InData.DataFiltroFormulati, InData.InData.PUA_RegolamentoCod,
                                 InData.InData.TipoRichiesto, InData.InData.Flag_LeggiGiacenze, InData.InData.Flag_FiltraRevocati,
                                 InData.InData.RegolamentoCod_Operazioni, InData.InData.Tipo_PuaRegolamento, InData.InData.Flag_IncludiNPK_Desc,
                                 InData.InData.Flag_IncludiClassificazione, InData.InData.Flag_QtaNoZero, InData.InData.xFiltroAggiuntivo,
                                 False,
                                 False,
                                 Flag_CodArticolo_In_Descrizione:=False)

    End Function

    Public Function Prodotti_Internal(ByVal objP_server As String, ByVal objP_utenti As String, ByVal objP_super_server As String,
                            ByVal Cau_Mov As String,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Integer,
                            ByVal Id_Destinazione As Integer,
                            ByVal Elem_Cod As Integer,
                            ByVal Flag_Negativo As Boolean,
                            ByVal RicercaTesto As String,
                            ByVal RicercaTestoJArray As String,
                            ByVal Flag_VisualizzaProCod As Boolean,
                            ByVal Flag_CaricaUdmCod As Boolean,
                            ByVal Pro_Cod As Integer,
                            ByVal Udm_Cod As Integer,
                            ByVal DataFiltroFormulati As Date,
                            ByVal PUA_RegolamentoCod As Integer,
                            ByVal TipoRichiesto As Integer,
                            ByVal Flag_LeggiGiacenze As Boolean,
                            ByVal Flag_FiltraRevocati As Boolean,
                            ByVal RegolamentoCod_Operazioni As Integer,
                            ByVal Tipo_PuaRegolamento As Integer,
                            ByVal Flag_IncludiNPK_Desc As Boolean,
                            ByVal Flag_IncludiClassificazione As Boolean,
                            ByVal Flag_QtaNoZero As Boolean,
                            ByVal xFiltroAggiuntivo As String,
                            Optional ByVal Flag_Filtra_MateriePrime_Per_Piva As Boolean? = False,
                            Optional ByVal Flag_Filtra_MateriePrime_Pubblici As Boolean? = False,
                            Optional ByVal Flag_CodArticolo_In_Descrizione As Boolean? = False
        ) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If

        Try

            Dim objFilters As JArray = Nothing
            If Not String.IsNullOrEmpty(RicercaTestoJArray) Then
                objFilters = JArray.Parse(RicercaTestoJArray)
                If Not objFilters Is Nothing Then
                    For Each obj As JObject In objFilters
                        RicercaTesto = obj("value").ToString()
                    Next
                End If
            End If

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)

            Dim ddl As New DropDownList
            Dim clc = New AgronicaCoreVarieBIZ.CaricaListControl_2010
            clc.Prodotti(ddl,
                                                                False, "", "",
                                                                Cau_Mov,
                                                                Piva,
                                                                Sa_Cod,
                                                                Id_Destinazione,
                                                                Elem_Cod,
                                                                Flag_Negativo,
                                                                RicercaTesto,
                                                                Flag_VisualizzaProCod,
                                                                Flag_CaricaUdmCod,
                                                                Pro_Cod,
                                                                Udm_Cod,
                                                                DataFiltroFormulati,
                                                                PUA_RegolamentoCod,
                                                                TipoRichiesto,
                                                                Flag_LeggiGiacenze,
                                                                Flag_FiltraRevocati,
                                                                xFiltroAggiuntivo,
                                                                "",
                                                                objParametri_Server,
                                                                objParametri_Utenti,
                                                                Nothing,
                                                                objParametri_Super_Server,
                                                                RegolamentoCod_Operazioni,
                                                                Tipo_PuaRegolamento,
                                                                Flag_IncludiNPK_Desc,
                                                                Flag_IncludiClassificazione,
                                                                Flag_QtaNoZero,
                                                                Flag_Filtra_MateriePrime_Per_Piva:=Flag_Filtra_MateriePrime_Per_Piva,
                                                                Flag_Filtra_MateriePrime_Pubblici:=Flag_Filtra_MateriePrime_Pubblici,
                                                                Flag_CodArticolo_In_Descrizione:=Flag_CodArticolo_In_Descrizione)

            Dim JArrayListaOp As New JArray()
            For Each i As ListItem In ddl.Items
                JArrayListaOp.Add(New JObject(New JProperty("pro_des", i.Text), New JProperty("pro_cod", i.Value)))
            Next

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            r.RispostaOK = False
            r.RispostaStringa = ""
            Return r
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Prodotti(ByVal InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Anagrafica.Prodotti)) As RispostaStandard

        Dim r As New RispostaStandard()

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If

        Try

            Dim objFilters As JArray = Nothing
            If Not String.IsNullOrEmpty(InData.InData.RicercaTestoJArray) Then
                objFilters = JArray.Parse(InData.InData.RicercaTestoJArray)
                If Not objFilters Is Nothing Then
                    For Each obj As JObject In objFilters
                        InData.InData.RicercaTesto = obj("value").ToString()

                        'Per ora gestita solo la descrizione e solo per Like %xxxx%
                        'Dim v = obj("value").ToString()

                        'If obj("field") = "Mat_Des" Then
                        '    If obj("operator") = "startswith" Then
                        '        mat_prime = mat_prime.Where(Function(a) a.Mat_Des.StartsWith(v))
                        '    End If
                        '    If obj("operator") = "contains" Then
                        '        mat_prime = mat_prime.Where(Function(a) a.Mat_Des.Contains(v))
                        '    End If
                        '    If obj("operator") = "equals" Then
                        '        mat_prime = mat_prime.Where(Function(a) a.Mat_Des.Equals(v))
                        '    End If
                        'End If
                    Next
                End If
            End If


            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim ddl As New DropDownList
            Dim clc = New AgronicaCoreVarieBIZ.CaricaListControl_2010
            clc.Prodotti(ddl,
                                                                False, "", "",
                                                                InData.InData.Cau_Mov,
                                                                InData.InData.Piva,
                                                                InData.InData.Sa_Cod,
                                                                InData.InData.Id_Destinazione,
                                                                InData.InData.Elem_Cod,
                                                                InData.InData.Flag_Negativo,
                                                                InData.InData.RicercaTesto,
                                                                InData.InData.Flag_VisualizzaProCod,
                                                                InData.InData.Flag_CaricaUdmCod,
                                                                InData.InData.Pro_Cod,
                                                                InData.InData.Udm_Cod,
                                                                InData.InData.DataFiltroFormulati,
                                                                InData.InData.PUA_RegolamentoCod,
                                                                InData.InData.TipoRichiesto,
                                                                InData.InData.Flag_LeggiGiacenze,
                                                                InData.InData.Flag_FiltraRevocati,
                                                                InData.InData.xFiltroAggiuntivo,
                                                                "",
                                                                 objParametri_Server,
                                                                 objParametri_Utenti,
                                                                Nothing,
                                                                    objParametri_Super_Server,
                                                                   InData.InData.RegolamentoCod_Operazioni,
                                                                   InData.InData.Tipo_PuaRegolamento,
                                                                   InData.InData.Flag_IncludiNPK_Desc,
                                                                   InData.InData.Flag_IncludiClassificazione,
                                                                   InData.InData.Flag_QtaNoZero)

            Dim JArrayListaOp As New JArray()
            For Each i As ListItem In ddl.Items
                JArrayListaOp.Add(New JObject(New JProperty("pro_des", i.Text), New JProperty("pro_cod", i.Value)))
            Next

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            r.RispostaOK = False
            r.RispostaStringa = ""
            Return r
        End Try

        Return r

    End Function

    Private Function ElencoProdotti(ByVal objP_super_server As String,
                                    ByVal objP_server As String,
                                    ByVal objP_utenti As String,
                                    ByVal piva As String,
                                    ByVal xSa_Cod As Integer,
                                    ByVal xFabbricato_Cod As Integer,
                                    ByVal xTipoDestinazione As Integer,
                                    ByVal Elem_Cod As Integer,
                                    ByVal soloInGiacenza As Boolean,
                                    ByVal FiltroDescrizioneProdotto As String,
                                    ByVal Mode As String,
                                    ByVal Cau_Mov As String,
                                    ByVal Data_Movimento_Str As String,
                                    ByVal xPUARegolamento As Integer,
                                    ByVal xLottoAccettazione As String,
                                    ByVal leggiUMformulati As Boolean,
                                    ByVal metaschema As String,
                                    ByVal elemcod_banchedati_sologiacenza As Integer(),
                                    ByVal Flag_QtaNoZero As Boolean,
                                    ByVal xGiasApp As Boolean,
                                    ByVal xTipoPUARegolamento As Integer,
                                    ByVal leggiGiacenzeConAgroDataFine As Boolean,
                                    ByVal w_veg_cod As Integer,
                                    ByVal w_cul_cod As Integer,
                                    ByVal xFiltroAggiuntivoMateriePrime As String,
                                    ByVal leggiAlias As Boolean,
                                    ByVal flagDiversificaDesFertilizzanti As Boolean,
                                    ByVal FiltroCodiceProdotto As Integer,
                                    ByVal FiltroCodiceTrappola As Integer,
                                    Optional ByVal inibisciVisibilitaGruppiMerce As Boolean = False,
                                    Optional ByVal Stato_Cod As String = "IT",
                                    Optional ByVal Lav_Cod As Integer = 0
                                    ) As DataTable

        Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Dim leggiLingua As New Lingue_Read
        Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
        Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
        Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

        Dim Flag_QtaMaggioreZero As Boolean = False

        'TODO: fare fix definitivo per data
        Dim Data_Movimento As Date

        If Date.TryParseExact(Data_Movimento_Str, "dd/MM/yyyy",
                              CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, Nothing) Then
            Data_Movimento = Date.ParseExact(Data_Movimento_Str, "dd/MM/yyyy",
                                             CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal)
        ElseIf IsDate(Data_Movimento_Str) Then
            Data_Movimento = CDate(Data_Movimento_Str)
        Else
            Throw New Exception(String.Format("Impossibile convertire la data movimento: {0}", Data_Movimento_Str))
        End If


        Dim Dt_Materie_Prime As DataTable = Nothing
        Dim x_Mat_Cod = 0

        Dim xFiltroAggiuntivoGiasAPP As String = ""

        If xGiasApp And soloInGiacenza Then

            xFiltroAggiuntivoGiasAPP += " Movimenti_dettagli.Elem_Cod IN ( "
            xFiltroAggiuntivoGiasAPP += CStr(CostantiPersonalizzate.FERTILIZZANTI)
            xFiltroAggiuntivoGiasAPP += ", "
            xFiltroAggiuntivoGiasAPP += CStr(CostantiPersonalizzate.FORMULATI)
            xFiltroAggiuntivoGiasAPP += ", "
            xFiltroAggiuntivoGiasAPP += CStr(CostantiPersonalizzate.INSETTI)
            xFiltroAggiuntivoGiasAPP += ", "
            xFiltroAggiuntivoGiasAPP += CStr(CostantiPersonalizzate.TRAPPOLE)
            xFiltroAggiuntivoGiasAPP += ", "
            xFiltroAggiuntivoGiasAPP += CStr(CostantiPersonalizzate.INNESCHI)
            xFiltroAggiuntivoGiasAPP += ", "
            xFiltroAggiuntivoGiasAPP += CStr(CostantiPersonalizzate.SEMENTI)
            xFiltroAggiuntivoGiasAPP += ", "
            xFiltroAggiuntivoGiasAPP += CStr(CostantiPersonalizzate.TRASFORMATI_VEGETALI)
            xFiltroAggiuntivoGiasAPP += " ) "

            ' Solo giacenze di magazzini visibili da APP
            Dim leggi_fabbricati_codice As New Fabbricati_Codici_R
            Dim magAPP = leggi_fabbricati_codice.Leggi(piva, 0, 0, CInt(enum_CodiciAnagrafe.Visibile_da_App), "1", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
            Dim xF = ""
            For Each r In magAPP.Rows
                If r.Item("val_cod") = "1" Then
                    If Not String.IsNullOrEmpty(xF) Then
                        xF += " OR "
                    End If
                    xF += " ( "
                    xF += " Mov_Destinazioni.Sa_Cod = "
                    xF += CStr(r.Item("Sa_Cod"))
                    xF += " AND "
                    xF += " Mov_Destinazioni.Id_Destinazione = "
                    xF += CStr(r.Item("Fabbricato_Cod"))
                    xF += " ) "
                End If
            Next
            If Not String.IsNullOrEmpty(xF) Then
                xFiltroAggiuntivoGiasAPP += " AND ( "
                xFiltroAggiuntivoGiasAPP += xF
                xFiltroAggiuntivoGiasAPP += " ) "
            Else
                xFiltroAggiuntivoGiasAPP += " AND 1 = 2 "
            End If

        Else

            xFiltroAggiuntivoGiasAPP = xFiltroAggiuntivoMateriePrime

        End If

        Dim xFiltroImpostazioni = ""
        Dim ui_R As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read()

        ' Leggo impostazione che dice se la decodifica dei prodotti è per azienda o per PivaSuperUser
        Dim impostazioneCAC_Codifica_ProdottiAziendali As String = "0"
        xFiltroImpostazioni = " Impostazione_Cod IN ("
        xFiltroImpostazioni += CStr(enum_Impostazioni_Utenti.SUPERUSER_JoinCacPivaSuperUser)
        xFiltroImpostazioni += ") "
        Dim dtImpostazioni_Codifica_ProdottiAziendali = ui_R.Leggi(0, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, xFiltroImpostazioni, "", objParametri_Utenti)
        If dtImpostazioni_Codifica_ProdottiAziendali.Rows.Count = 1 Then
            impostazioneCAC_Codifica_ProdottiAziendali = dtImpostazioni_Codifica_ProdottiAziendali.Rows(0).Item("Impostazione_Valore_1")
        End If

        Dim Dict_CAC_Codifica_ProdottiAziendali As New Dictionary(Of String, String)
        Dim CAC_Codifica_ProdottiAziendali_Leggi As New CAC_Codifica_ProdottiAziendali_R
        Dim cod_articolo_found As Boolean = False
        Dim cod_articolo As String = ""

        ' Se non viene passata la P.Iva e la decodifica dei prodotti è per azienda non verrà mostrato il codice prodotto cliente  
        ' Se viene passata la P.Iva e la decodifica dei prodotti è per SuperUser si legge senza P.Iva
        If Not String.IsNullOrWhiteSpace(piva) OrElse impostazioneCAC_Codifica_ProdottiAziendali = "1" Then
            Dim piva_da_usare = piva
            If impostazioneCAC_Codifica_ProdottiAziendali = "1" Then
                piva_da_usare = ""
            End If
            Dim DT_CAC_Codifica_ProdottiAziendali = CAC_Codifica_ProdottiAziendali_Leggi.Leggi(piva_da_usare, enum_Tipo_CAC_Codifica_ProdottiAziendali.NonDefinito, 0, "",
                            "", "", "", objParametri_Server)
            For Each CAC_Row In DT_CAC_Codifica_ProdottiAziendali.Rows
                If CInt(CAC_Row.Item("Codice_Gias")) <> 0 And
                   Not String.IsNullOrEmpty(CStr(CAC_Row.Item("Cod_prodotto_cliente"))) Then
                    cod_articolo_found = Dict_CAC_Codifica_ProdottiAziendali.TryGetValue(CStr(CAC_Row.Item("Elem_cod")) & "_" & CStr(CAC_Row.Item("Codice_Gias")), cod_articolo)
                    If Not cod_articolo_found Then
                        Dict_CAC_Codifica_ProdottiAziendali.Add(CStr(CAC_Row.Item("Elem_cod")) & "_" & CStr(CAC_Row.Item("Codice_Gias")), CStr(CAC_Row.Item("Cod_prodotto_cliente")))
                    End If
                End If
            Next
        End If

        Dim impostazioneGiacenze() As String = Nothing
        Dim bloccaUtentePerSottogiacenza = False
        If Not xGiasApp Then
            xFiltroImpostazioni = " Impostazione_Cod IN ("
            xFiltroImpostazioni += CStr(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE)
            xFiltroImpostazioni += ") "
            Dim dtImpostazioniGiacenze = ui_R.Leggi(0, 2, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, xFiltroImpostazioni, "", objParametri_Utenti)
            If dtImpostazioniGiacenze.Rows.Count = 1 Then
                impostazioneGiacenze = dtImpostazioniGiacenze.Rows(0).Item("Impostazione_Valore_1").Split("|")
            End If

            xFiltroImpostazioni = " Impostazione_Cod IN ("
            xFiltroImpostazioni += CStr(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE)
            xFiltroImpostazioni += ") "
            dtImpostazioniGiacenze = ui_R.Leggi(0, 1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, xFiltroImpostazioni, "", objParametri_Utenti)
            If dtImpostazioniGiacenze.Rows.Count = 1 Then
                Select Case dtImpostazioniGiacenze.Rows(0).Item("Impostazione_Valore_1")
                    Case "0"
                        bloccaUtentePerSottogiacenza = False
                    Case Else
                        bloccaUtentePerSottogiacenza = True
                End Select
            End If
        End If

        Dim dtProdotti As New DataTable
        Dim drProdotti As DataRow

        Dim objFilters As JArray = Nothing
        If Not String.IsNullOrEmpty(FiltroDescrizioneProdotto) Then
            objFilters = JArray.Parse(FiltroDescrizioneProdotto)
            If Not objFilters Is Nothing Then
                For Each obj As JObject In objFilters
                    FiltroDescrizioneProdotto = obj("value").ToString()

                    'Per ora gestita solo la descrizione e solo per Like %xxxx%
                    'Dim v = obj("value").ToString()

                    'If obj("field") = "Mat_Des" Then
                    '    If obj("operator") = "startswith" Then
                    '        mat_prime = mat_prime.Where(Function(a) a.Mat_Des.StartsWith(v))
                    '    End If
                    '    If obj("operator") = "contains" Then
                    '        mat_prime = mat_prime.Where(Function(a) a.Mat_Des.Contains(v))
                    '    End If
                    '    If obj("operator") = "equals" Then
                    '        mat_prime = mat_prime.Where(Function(a) a.Mat_Des.Equals(v))
                    '    End If
                    'End If
                Next
            End If
        End If

        dtProdotti.Columns.Add(New DataColumn("Elem_Cod", GetType(Integer)))
        If leggiUMformulati Then
            dtProdotti.Columns.Add(New DataColumn("Prodotto_Cod", GetType(String)))
        Else
            dtProdotti.Columns.Add(New DataColumn("Prodotto_Cod", GetType(Integer)))
        End If
        dtProdotti.Columns.Add(New DataColumn("Prodotto_Des", GetType(String)))
        dtProdotti.Columns.Add(New DataColumn("NomeComune", GetType(String)))
        dtProdotti.Columns.Add(New DataColumn("N", GetType(Decimal)))
        dtProdotti.Columns.Add(New DataColumn("P2O5", GetType(Decimal)))
        dtProdotti.Columns.Add(New DataColumn("K2O", GetType(Decimal)))
        dtProdotti.Columns.Add(New DataColumn("Cu", GetType(Decimal)))
        dtProdotti.Columns.Add(New DataColumn("Uso", GetType(Integer)))
        dtProdotti.Columns.Add(New DataColumn("Udm_Cod", GetType(Integer)))
        dtProdotti.Columns.Add(New DataColumn("Extra_Str", GetType(String)))
        dtProdotti.Columns.Add(New DataColumn("IsTrappolaFormulato", GetType(Boolean)) With {.DefaultValue = False})
        If Not xGiasApp OrElse Elem_Cod = TRASFORMATI_VEGETALI Then
            dtProdotti.Columns.Add(New DataColumn("Veg_Cod", GetType(Integer)))
            dtProdotti.Columns.Add(New DataColumn("Cul_Cod", GetType(Integer)))
            dtProdotti.Columns.Add(New DataColumn("Reg_Cod", GetType(Integer)))
            dtProdotti.Columns.Add(New DataColumn("Sem_Cod", GetType(Integer)))
            dtProdotti.Columns.Add(New DataColumn("GRVA_COD_VEG", GetType(Integer)))
            dtProdotti.Columns.Add(New DataColumn("Cat_Cod", GetType(Integer)))

            dtProdotti.Columns.Add(New DataColumn("LegatoALinea", GetType(Integer)))
            dtProdotti.Columns.Add(New DataColumn("Mat_Cod_OMNI", GetType(Integer)))
            dtProdotti.Columns.Add(New DataColumn("Piva", GetType(String)))
            dtProdotti.Columns.Add(New DataColumn("Qta_Extra", GetType(Decimal)))
            dtProdotti.Columns.Add(New DataColumn("Udm_Cod_Extra", GetType(Integer)))
            dtProdotti.Columns.Add(New DataColumn("Veg_Des", GetType(String)))
            dtProdotti.Columns.Add(New DataColumn("Cul_Des", GetType(String)))
            dtProdotti.Columns.Add(New DataColumn("Rag_Soc_Proprietaria", GetType(String)))
            dtProdotti.Columns.Add(New DataColumn("Utente_Creazione", GetType(String)))
            dtProdotti.Columns.Add(New DataColumn("Utente_Modifica", GetType(String)))
            dtProdotti.Columns.Add(New DataColumn("Data_Creazione", GetType(Date)))
            dtProdotti.Columns.Add(New DataColumn("Data_Modifica", GetType(Date)))
            dtProdotti.Columns.Add(New DataColumn("Otabella_Cod_Base", GetType(Integer)))
            dtProdotti.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
            dtProdotti.Columns.Add(New DataColumn("Cod_Articolo", GetType(String)))
            dtProdotti.Columns.Add(New DataColumn("Codice_Esterno", GetType(String)))
            dtProdotti.Columns.Add(New DataColumn("ChkALias", GetType(Integer)))
        End If

        Dim gestioneGruppiMerce As Boolean = False
        Dim objGruppiMerce As New Gruppi_Merce_R
        Dim ListaGruppiMercePerCategoria As New List(Of ImpostazioneDefault_GruppiMerce)()

        If Not xGiasApp Then

            ListaGruppiMercePerCategoria = objGruppiMerce.GetListGruppiMerceDefault(piva, SACOD_NOFILTRO, objParametri_Utenti, objParametri_Server)

            If ListaGruppiMercePerCategoria.Count > 0 Then
                gestioneGruppiMerce = True

                dtProdotti.Columns.Add(New DataColumn("Id_Gruppo_Merce", GetType(Integer)))
                dtProdotti.Columns.Add(New DataColumn("Des_Gruppo_Merce", GetType(String)))
            End If
        End If

        Dim cmb_Prodotti As New DropDownList

        Dim objCat As New Categorie_Magazzino_R
        Dim Dt_Categorie = objCat.Leggi(0,
                                        CAU_MAGAZZINO,
                                        False,
                                        "",
                                        "",
                                        objParametri_Server)

        Dim causaleDaUtilizzare = CAU_CARICO
        If Cau_Mov <> CAU_CARICO Or soloInGiacenza Then
            causaleDaUtilizzare = CAU_SCARICO
        End If

        Select Case Elem_Cod

            'macchine - tipologie sementi - Altre materie prime aziendali - Semilavorati Produzione Vegetale - 
            'Materie Prime Produzione Vegetale - Beni Confezionamento Vegetale - Trasformati Produzione Vegetale
            'Semilavorati Produzione Animale - Materie Prime Produzione Animale - Beni Confezionamento Animale -
            'Mangimi - Farmaci - Trasformati Produzione Animale

            Case CARBURANTI, SEMENTI, ALTRE_MATERIE, MANGIMI, RICAMBI,
                SEMILAVORATI_VEGETALI, MATERIE_VEGETALI, BENI_CONFEZ_VEGETALE, TRASFORMATI_VEGETALI,
                SEMILAVORATI_ANIMALI, MATERIE_ANIMALI, BENI_CONFEZ_ANIMALE, TRASFORMATI_ANIMALI,
                CAT_MAG_SERVIZI_PROFESSIONALI

                If causaleDaUtilizzare = CAU_SCARICO Then
                    If xGiasApp AndAlso Not bloccaUtentePerSottogiacenza Then
                        'APP - Passo comunque quelli movimentati almeno una volta
                        'Flag_QtaNoZero già passata dall'esterno
                        Flag_QtaMaggioreZero = False
                    Else
                        ImpostaCausaleEFlagGiacenza(soloInGiacenza,
                                                    causaleDaUtilizzare,
                                                    Flag_QtaNoZero,
                                                    impostazioneGiacenze,
                                                    bloccaUtentePerSottogiacenza,
                                                    Elem_Cod,
                                                    Flag_QtaMaggioreZero)
                    End If
                End If

                Dim clc = New AgronicaCoreUtility.CaricaListControl
                clc.Materie_Prime(cmb_Prodotti,
                                  False,
                                  "",
                                  "",
                                  causaleDaUtilizzare,
                                  piva,
                                  xSa_Cod,
                                  xFabbricato_Cod,
                                  Elem_Cod,
                                  True,
                                  FiltroDescrizioneProdotto,
                                  xLottoAccettazione,
                                  "", "", FiltroCodiceProdotto, 0, 0,
                                  CODPROGETTO_NONDEFINITO,
                                  0,
                                  LOTTO_NONDEFINITO,
                                  w_veg_cod,
                                  w_cul_cod,
                                  0, 0, 0, 0, 0,
                                  CDate(Data_Movimento).ToShortDateString,
                                  xFiltroAggiuntivoGiasAPP,
                                  "",
                                  objParametri_Server,
                                  objParametri_Utenti,
                                  True,
                                  Dt_Materie_Prime,
                                  Flag_QtaNoZero,
                                  Flag_QtaMaggioreZero:=Flag_QtaMaggioreZero,
                                  LeggiAlias:=leggiAlias,
                                  gruppiMerceDefaultPerCategoria:=ListaGruppiMercePerCategoria, inibisciVisibilitaGruppiMerce)

                If Not xGiasApp AndAlso Elem_Cod = SEMENTI Then
                    dtProdotti.Columns.Add(New DataColumn("Cod_TecnologiaSementi", GetType(Integer)))
                    dtProdotti.Columns.Add(New DataColumn("Germinabilita", GetType(Double)))
                End If

                Dim w_nomecomune As String = (From cat In Dt_Categorie
                                              Where cat("Elem_Cod") = Elem_Cod
                                              Select cat("NomeComune")).FirstOrDefault()

                For Each elem In cmb_Prodotti.Items

                    drProdotti = dtProdotti.NewRow
                    drProdotti.Item("Elem_Cod") = Elem_Cod
                    drProdotti.Item("Prodotto_Cod") = DirectCast(elem, System.Web.UI.WebControls.ListItem).[Value]
                    drProdotti.Item("Prodotto_Des") = DirectCast(elem, System.Web.UI.WebControls.ListItem).[Text]
                    drProdotti.Item("NomeComune") = w_nomecomune
                    drProdotti.Item("N") = 0
                    drProdotti.Item("P2O5") = 0
                    drProdotti.Item("K2O") = 0
                    drProdotti.Item("Cu") = 0
                    drProdotti.Item("Uso") = 0
                    drProdotti.Item("Udm_Cod") = 0

                    'Dati specifici del singolo prodotto
                    Dim riga = (From r In Dt_Materie_Prime
                                Where r("Elem_Cod") = drProdotti.Item("Elem_Cod") And
                                        -r("Mat_Cod") = drProdotti.Item("Prodotto_Cod")
                                Select r).FirstOrDefault()

                    If Not xGiasApp OrElse Elem_Cod = TRASFORMATI_VEGETALI Then

                        If riga IsNot Nothing Then
                            drProdotti.Item("Veg_Cod") = riga.Item("Veg_Cod")
                            drProdotti.Item("Cul_Cod") = riga.Item("Cul_Cod")
                            drProdotti.Item("Reg_Cod") = riga.Item("Regolamento")
                            drProdotti.Item("Sem_Cod") = riga.Item("sem_cod")
                            drProdotti.Item("GRVA_COD_VEG") = riga.Item("GRVA_COD_VEG")
                            drProdotti.Item("Cat_Cod") = riga.Item("cat_cod")
                            drProdotti.Item("LegatoALinea") = riga.Item("LegatoALinea")
                            drProdotti.Item("Mat_Cod_OMNI") = riga.Item("Mat_Cod_OMNI")
                            drProdotti.Item("Piva") = riga.Item("Piva")
                            drProdotti.Item("Qta_Extra") = riga.Item("Qta_Extra")
                            drProdotti.Item("Udm_Cod") = riga.Item("Udm_Cod")
                            drProdotti.Item("Udm_Cod_Extra") = riga.Item("Udm_Cod_Extra")
                            drProdotti.Item("Otabella_Cod_Base") = riga.Item("Otabella_Cod_Base")
                            drProdotti.Item("Cod_Articolo") = riga.Item("Cod_Articolo")
                            drProdotti.Item("Codice_Esterno") = riga.Item("Codice_Esterno")
                            drProdotti.Item("Data_Creazione") = riga.Item("Data_Creazione")
                            drProdotti.Item("Data_Modifica") = riga.Item("Data_Modifica")
                            drProdotti.Item("Extra_Str") = riga.Item("Extra_Str")
                            If causaleDaUtilizzare = CAU_CARICO Then
                                drProdotti.Item("Veg_Des") = riga.Item("Veg_Des")
                                drProdotti.Item("Cul_Des") = riga.Item("Cul_Des")
                                drProdotti.Item("Rag_Soc_Proprietaria") = riga.Item("Rag_Soc_Proprietaria")
                                drProdotti.Item("Utente_Creazione") = riga.Item("Utente_Creazione")
                                drProdotti.Item("Utente_Modifica") = riga.Item("Utente_Modifica")
                                drProdotti.Item("Sa_Cod") = riga.Item("Sa_Cod")
                                drProdotti.Item("ChkAlias") = riga.Item("ChkAlias")
                            Else
                                drProdotti.Item("Veg_Des") = ""
                                drProdotti.Item("Cul_Des") = ""
                                drProdotti.Item("Rag_Soc_Proprietaria") = ""
                                drProdotti.Item("Utente_Creazione") = ""
                                drProdotti.Item("Utente_Modifica") = ""
                                drProdotti.Item("Sa_Cod") = 0
                                drProdotti.Item("ChkAlias") = 0
                            End If
                        End If

                    End If

                    If Not xGiasApp AndAlso Elem_Cod = SEMENTI AndAlso riga IsNot Nothing Then
                        drProdotti.Item("Cod_TecnologiaSementi") = riga.Item("Cod_TecnologiaSementi")
                        drProdotti.Item("Germinabilita") = riga.Item("Germinabilita")
                    End If

                    If gestioneGruppiMerce AndAlso riga IsNot Nothing Then
                        drProdotti.Item("Id_Gruppo_Merce") = riga.Item("Id_Gruppo_Merce")
                        drProdotti.Item("Des_Gruppo_Merce") = riga.Item("Des_Gruppo_Merce")
                    End If

                    dtProdotti.Rows.Add(drProdotti)
                Next

            ' Coadiuvanti
            '  --> Non più gestiti su tabelle specifiche, ORA SONO SUI FITOFARMACI
            Case COADIUVANTI


            'Fertilizzanti - Formulati - Insetti utili - Trappole commerciali - Inneschi trappole - Farmaci
            Case FERTILIZZANTI, FORMULATI, INSETTI, TRAPPOLE, INNESCHI, SERVIZI, CostantiPersonalizzate.FARMACI

                If causaleDaUtilizzare = CAU_SCARICO Then

                    If xGiasApp AndAlso Not bloccaUtentePerSottogiacenza Then
                        'APP - Passo comunque quelli movimentati almeno una volta
                        'Flag_QtaNoZero già passata dall'esterno
                        Flag_QtaMaggioreZero = False
                    Else
                        ImpostaCausaleEFlagGiacenza(soloInGiacenza,
                                                    causaleDaUtilizzare,
                                                    Flag_QtaNoZero,
                                                    impostazioneGiacenze,
                                                    bloccaUtentePerSottogiacenza,
                                                    Elem_Cod,
                                                    Flag_QtaMaggioreZero)
                    End If

                Else

                    If xGiasApp AndAlso soloInGiacenza Then
                        'APP - Anche se si vogliono tutti i prodotti e non solo quelli in giacenza
                        '      per alcune categorie specifiche passo solo quelli movimentati almeno una volta
                        If elemcod_banchedati_sologiacenza IsNot Nothing Then
                            If elemcod_banchedati_sologiacenza.Contains(Elem_Cod) Then
                                causaleDaUtilizzare = CAU_SCARICO
                                'Flag_QtaNoZero già passata dall'esterno
                                Flag_QtaMaggioreZero = False
                            End If
                        End If
                    End If

                End If

                Dim Flag_CaricaUdmCod As Boolean = False

                If Cau_Mov = CAU_CARICO Then
                    If Elem_Cod = FORMULATI Then
                        Flag_CaricaUdmCod = True
                    End If
                End If

                'Chiamata specifica con categoria prodotto richiesta (Elem_Cod)
                CercaProdottiBancheDati(Dt_Categorie,
                                        leggiUMformulati,
                                        piva,
                                        Elem_Cod,
                                        causaleDaUtilizzare,
                                        FiltroDescrizioneProdotto,
                                        Data_Movimento,
                                        Mode,
                                        xPUARegolamento,
                                        xSa_Cod,
                                        xFabbricato_Cod,
                                        dtProdotti,
                                        objParametri_Super_Server,
                                        objParametri_Server,
                                        objParametri_Utenti,
                                        elemcod_banchedati_sologiacenza,
                                        Flag_QtaNoZero,
                                        Dict_CAC_Codifica_ProdottiAziendali,
                                        xFiltroAggiuntivoGiasAPP,
                                        Flag_QtaMaggioreZero,
                                        xTipoPUARegolamento,
                                        xGiasApp,
                                        leggiGiacenzeConAgroDataFine,
                                        flagDiversificaDesFertilizzanti,
                                        FiltroCodiceProdotto,
                                        FiltroCodiceTrappola,
                                        ListaGruppiMercePerCategoria, inibisciVisibilitaGruppiMerce, Stato_Cod, w_veg_cod, Lav_Cod)

            Case 0
                '-------------------------------------------------------------------------------------------------------
                ' Devo selezionare tutti i prodotti di ogni tipo; faccio la ricerca di ognuno accodando a un DataTable
                '-------------------------------------------------------------------------------------------------------

                'Solo se devo leggere solo i prodotti specifici per azienda
                If metaschema = "" OrElse metaschema = "N" Then

                    'Leggo tutte le categorie prodotti e ciclo per ognuna di quelle che si trovano in materie prime
                    Dim w_Elem_Cod As Integer = 0
                    For Each rCat In Dt_Categorie.Rows
                        w_Elem_Cod = rCat.Item("Elem_Cod")
                        Select Case w_Elem_Cod

                            Case CARBURANTI, SEMENTI, ALTRE_MATERIE, MANGIMI, RICAMBI,
                                SEMILAVORATI_VEGETALI, MATERIE_VEGETALI, BENI_CONFEZ_VEGETALE, TRASFORMATI_VEGETALI,
                                SEMILAVORATI_ANIMALI, MATERIE_ANIMALI, BENI_CONFEZ_ANIMALE, TRASFORMATI_ANIMALI,
                                CAT_MAG_SERVIZI_PROFESSIONALI

                                'Solo prodotti in giacenza per alcune categorie specifiche
                                If causaleDaUtilizzare = CAU_SCARICO Then
                                    If xGiasApp AndAlso Not bloccaUtentePerSottogiacenza Then
                                        'APP - Passo comunque quelli movimentati almeno una volta
                                        'Flag_QtaNoZero già passata dall'esterno
                                        Flag_QtaMaggioreZero = False
                                    Else
                                        ImpostaCausaleEFlagGiacenza(soloInGiacenza,
                                                                    causaleDaUtilizzare,
                                                                    Flag_QtaNoZero,
                                                                    impostazioneGiacenze,
                                                                    bloccaUtentePerSottogiacenza,
                                                                    Elem_Cod,
                                                                    Flag_QtaMaggioreZero)
                                    End If
                                End If

                                Select Case Mode.ToLower

                                    Case "trasferimento"

                                        Dim clc = New AgronicaCoreUtility.CaricaListControl
                                        clc.Materie_Prime(cmb_Prodotti,
                                                          False,
                                                          "",
                                                          "",
                                                          causaleDaUtilizzare,
                                                          piva,
                                                          xSa_Cod,
                                                          xFabbricato_Cod,
                                                          w_Elem_Cod,
                                                          True,
                                                          FiltroDescrizioneProdotto,
                                                          xLottoAccettazione,
                                                          "",
                                                          "",
                                                          FiltroCodiceProdotto,
                                                          0,
                                                          0,
                                                          CODPROGETTO_NONDEFINITO,
                                                          0,
                                                          LOTTO_NONDEFINITO,
                                                          w_veg_cod,
                                                          w_cul_cod,
                                                          0,
                                                          0,
                                                          0,
                                                          0,
                                                          0,
                                                          CDate(Data_Movimento).ToShortDateString,
                                                          xFiltroAggiuntivoGiasAPP,
                                                          "",
                                                          objParametri_Server,
                                                          objParametri_Utenti,
                                                          True,
                                                          Dt_Materie_Prime,
                                                          Flag_QtaNoZero,
                                                          Flag_QtaMaggioreZero,
                                                          LeggiAlias:=leggiAlias,
                                                          gruppiMerceDefaultPerCategoria:=ListaGruppiMercePerCategoria, inibisciVisibilitaGruppiMerce)

                                    Case Else

                                        Dim clc = New AgronicaCoreUtility.CaricaListControl
                                        clc.Materie_Prime(cmb_Prodotti,
                                                          False,
                                                          "",
                                                          "",
                                                          causaleDaUtilizzare,
                                                          piva,
                                                          xSa_Cod,
                                                          xFabbricato_Cod,
                                                          w_Elem_Cod,
                                                          True,
                                                          FiltroDescrizioneProdotto,
                                                          xLottoAccettazione,
                                                          "",
                                                          "",
                                                          FiltroCodiceProdotto,
                                                          0,
                                                          0,
                                                          CODPROGETTO_NONDEFINITO,
                                                          0,
                                                          LOTTO_NONDEFINITO,
                                                          w_veg_cod,
                                                          w_cul_cod,
                                                          0,
                                                          0,
                                                          0,
                                                          0,
                                                          0,
                                                          CDate(Data_Movimento).ToShortDateString,
                                                          xFiltroAggiuntivoGiasAPP,
                                                          "",
                                                          objParametri_Server,
                                                          objParametri_Utenti,
                                                          True,
                                                          Dt_Materie_Prime,
                                                          Flag_QtaNoZero,
                                                          Flag_QtaMaggioreZero,
                                                          LeggiAlias:=leggiAlias,
                                                          gruppiMerceDefaultPerCategoria:=ListaGruppiMercePerCategoria, inibisciVisibilitaGruppiMerce)

                                End Select

                                For Each elem In cmb_Prodotti.Items
                                    drProdotti = dtProdotti.NewRow
                                    drProdotti.Item("Prodotto_Cod") = DirectCast(elem, System.Web.UI.WebControls.ListItem).[Value]
                                    drProdotti.Item("Elem_Cod") = w_Elem_Cod
                                    drProdotti.Item("Prodotto_Des") = DirectCast(elem, System.Web.UI.WebControls.ListItem).[Text]
                                    drProdotti.Item("NomeComune") = rCat.Item("NomeComune")
                                    drProdotti.Item("N") = 0
                                    drProdotti.Item("P2O5") = 0
                                    drProdotti.Item("K2O") = 0
                                    drProdotti.Item("Cu") = 0
                                    drProdotti.Item("Uso") = 0
                                    drProdotti.Item("Udm_Cod") = 0

                                    'Dati specifici del singolo prodotto
                                    Dim riga = (From r In Dt_Materie_Prime
                                                Where r("Elem_Cod") = drProdotti.Item("Elem_Cod") And
                                                        -r("Mat_Cod") = drProdotti.Item("Prodotto_Cod")
                                                Select r).FirstOrDefault()

                                    If Not xGiasApp Then

                                        If riga IsNot Nothing Then
                                            drProdotti.Item("Veg_Cod") = riga.Item("Veg_Cod")
                                            drProdotti.Item("Cul_Cod") = riga.Item("Cul_Cod")
                                            drProdotti.Item("Reg_Cod") = riga.Item("Regolamento")
                                            drProdotti.Item("Sem_Cod") = riga.Item("sem_cod")
                                            drProdotti.Item("GRVA_COD_VEG") = riga.Item("GRVA_COD_VEG")
                                            drProdotti.Item("Cat_Cod") = riga.Item("cat_cod")
                                            drProdotti.Item("LegatoALinea") = riga.Item("LegatoALinea")
                                            drProdotti.Item("Mat_Cod_OMNI") = riga.Item("Mat_Cod_OMNI")
                                            drProdotti.Item("Piva") = riga.Item("Piva")
                                            drProdotti.Item("Qta_Extra") = riga.Item("Qta_Extra")
                                            drProdotti.Item("Udm_Cod") = riga.Item("Udm_Cod")
                                            drProdotti.Item("Udm_Cod_Extra") = riga.Item("Udm_Cod_Extra")
                                            drProdotti.Item("Otabella_Cod_Base") = riga.Item("Otabella_Cod_Base")
                                            drProdotti.Item("Cod_Articolo") = riga.Item("Cod_Articolo")
                                            drProdotti.Item("Codice_Esterno") = riga.Item("Codice_Esterno")
                                            drProdotti.Item("Data_Creazione") = riga.Item("Data_Creazione")
                                            drProdotti.Item("Data_Modifica") = riga.Item("Data_Modifica")
                                            drProdotti.Item("Extra_Str") = riga.Item("Extra_Str")
                                            If causaleDaUtilizzare = CAU_CARICO Then
                                                drProdotti.Item("Veg_Des") = riga.Item("Veg_Des")
                                                drProdotti.Item("Cul_Des") = riga.Item("Cul_Des")
                                                drProdotti.Item("Rag_Soc_Proprietaria") = riga.Item("Rag_Soc_Proprietaria")
                                                drProdotti.Item("Utente_Creazione") = riga.Item("Utente_Creazione")
                                                drProdotti.Item("Utente_Modifica") = riga.Item("Utente_Modifica")
                                                drProdotti.Item("Sa_Cod") = riga.Item("Sa_Cod")
                                                drProdotti.Item("ChkAlias") = riga.Item("ChkAlias")
                                            Else
                                                drProdotti.Item("Veg_Des") = ""
                                                drProdotti.Item("Cul_Des") = ""
                                                drProdotti.Item("Rag_Soc_Proprietaria") = ""
                                                drProdotti.Item("Utente_Creazione") = ""
                                                drProdotti.Item("Utente_Modifica") = ""
                                                drProdotti.Item("Sa_Cod") = 0
                                                drProdotti.Item("ChkAlias") = 0
                                            End If
                                        End If
                                    End If

                                    If gestioneGruppiMerce AndAlso riga IsNot Nothing Then
                                        drProdotti.Item("Id_Gruppo_Merce") = riga.Item("Id_Gruppo_Merce")
                                        drProdotti.Item("Des_Gruppo_Merce") = riga.Item("Des_Gruppo_Merce")
                                    End If

                                    dtProdotti.Rows.Add(drProdotti)
                                Next

                        End Select
                    Next

                End If

                '### FERTILIZZANTI

                'Solo prodotti in giacenza per alcune categorie specifiche

                Dim causaleDaUtilizzareFertilizzanti As String = causaleDaUtilizzare

                If causaleDaUtilizzare = CAU_SCARICO Then
                    If xGiasApp AndAlso Not bloccaUtentePerSottogiacenza Then
                        'APP - Solo prodotti in giacenza per alcune categorie specifiche
                        If elemcod_banchedati_sologiacenza IsNot Nothing Then
                            If elemcod_banchedati_sologiacenza.Contains(FERTILIZZANTI) Then
                                causaleDaUtilizzareFertilizzanti = CAU_SCARICO
                                'APP - Passo comunque quelli movimentati almeno una volta
                                'Flag_QtaNoZero già passata dall'esterno
                                Flag_QtaMaggioreZero = False
                            Else
                                causaleDaUtilizzareFertilizzanti = CAU_CARICO
                            End If
                        End If
                    Else
                        ImpostaCausaleEFlagGiacenza(soloInGiacenza,
                                                    causaleDaUtilizzare,
                                                    Flag_QtaNoZero,
                                                    impostazioneGiacenze,
                                                    bloccaUtentePerSottogiacenza,
                                                    Elem_Cod,
                                                    Flag_QtaMaggioreZero)
                    End If
                End If

                If (metaschema = "" OrElse
                    (metaschema = "S" AndAlso causaleDaUtilizzareFertilizzanti = CAU_CARICO) OrElse
                    (metaschema = "N" AndAlso causaleDaUtilizzareFertilizzanti = CAU_SCARICO)) Then
                    CercaProdottiBancheDati(Dt_Categorie,
                                            leggiUMformulati,
                                            piva,
                                            FERTILIZZANTI,
                                            causaleDaUtilizzareFertilizzanti,
                                            FiltroDescrizioneProdotto,
                                            Data_Movimento,
                                            Mode,
                                            xPUARegolamento,
                                            xSa_Cod,
                                            xFabbricato_Cod,
                                            dtProdotti,
                                            objParametri_Super_Server,
                                            objParametri_Server,
                                            objParametri_Utenti,
                                            elemcod_banchedati_sologiacenza,
                                            Flag_QtaNoZero,
                                            Dict_CAC_Codifica_ProdottiAziendali,
                                            xFiltroAggiuntivoGiasAPP,
                                            Flag_QtaMaggioreZero,
                                            xTipoPUARegolamento,
                                            xGiasApp,
                                            leggiGiacenzeConAgroDataFine,
                                            flagDiversificaDesFertilizzanti,
                                            FiltroCodiceProdotto,
                                            0,
                                            ListaGruppiMercePerCategoria, inibisciVisibilitaGruppiMerce, Stato_Cod, w_veg_cod, Lav_Cod)
                End If

                '### FORMULATI

                'Solo prodotti in giacenza per alcune categorie specifiche

                Dim causaleDaUtilizzareFormulati As String = causaleDaUtilizzare

                If causaleDaUtilizzare = CAU_SCARICO Then
                    If xGiasApp AndAlso Not bloccaUtentePerSottogiacenza Then
                        'APP - Solo prodotti in giacenza per alcune categorie specifiche
                        If elemcod_banchedati_sologiacenza IsNot Nothing Then
                            If elemcod_banchedati_sologiacenza.Contains(FORMULATI) Then
                                causaleDaUtilizzareFormulati = CAU_SCARICO
                                'APP - Passo comunque quelli movimentati almeno una volta
                                'Flag_QtaNoZero già passata dall'esterno
                                Flag_QtaMaggioreZero = False
                            Else
                                causaleDaUtilizzareFormulati = CAU_CARICO
                            End If
                        End If
                    Else
                        ImpostaCausaleEFlagGiacenza(soloInGiacenza,
                                                    causaleDaUtilizzare,
                                                    Flag_QtaNoZero,
                                                    impostazioneGiacenze,
                                                    bloccaUtentePerSottogiacenza,
                                                    Elem_Cod,
                                                    Flag_QtaMaggioreZero)
                    End If
                End If

                If (metaschema = "" OrElse
                        (metaschema = "S" AndAlso causaleDaUtilizzareFormulati = CAU_CARICO) OrElse
                        (metaschema = "N" AndAlso causaleDaUtilizzareFormulati = CAU_SCARICO)) Then
                    CercaProdottiBancheDati(Dt_Categorie,
                                            leggiUMformulati,
                                            piva,
                                            FORMULATI,
                                            causaleDaUtilizzareFormulati,
                                            FiltroDescrizioneProdotto,
                                            Data_Movimento,
                                            Mode,
                                            xPUARegolamento,
                                            xSa_Cod,
                                            xFabbricato_Cod,
                                            dtProdotti,
                                            objParametri_Super_Server,
                                            objParametri_Server,
                                            objParametri_Utenti,
                                            elemcod_banchedati_sologiacenza,
                                            Flag_QtaNoZero,
                                            Dict_CAC_Codifica_ProdottiAziendali,
                                            xFiltroAggiuntivoGiasAPP,
                                            Flag_QtaMaggioreZero,
                                            xTipoPUARegolamento,
                                            xGiasApp,
                                            leggiGiacenzeConAgroDataFine,
                                            flagDiversificaDesFertilizzanti,
                                            FiltroCodiceProdotto,
                                            0,
                                            ListaGruppiMercePerCategoria, inibisciVisibilitaGruppiMerce, Stato_Cod, w_veg_cod, Lav_Cod)
                End If

                '### COADIUVANTI

                'ORA SONO SUI FITOFARMACI

                '### INSETTI

                'Solo prodotti in giacenza per alcune categorie specifiche

                Dim causaleDaUtilizzareInsetti As String = causaleDaUtilizzare

                If causaleDaUtilizzare = CAU_SCARICO Then
                    If xGiasApp AndAlso Not bloccaUtentePerSottogiacenza Then
                        'APP - Solo prodotti in giacenza per alcune categorie specifiche
                        If elemcod_banchedati_sologiacenza IsNot Nothing Then
                            If elemcod_banchedati_sologiacenza.Contains(INSETTI) Then
                                causaleDaUtilizzareInsetti = CAU_SCARICO
                                'APP - Passo comunque quelli movimentati almeno una volta
                                'Flag_QtaNoZero già passata dall'esterno
                                Flag_QtaMaggioreZero = False
                            Else
                                causaleDaUtilizzareInsetti = CAU_CARICO
                            End If
                        End If
                    Else
                        ImpostaCausaleEFlagGiacenza(soloInGiacenza,
                                                    causaleDaUtilizzare,
                                                    Flag_QtaNoZero,
                                                    impostazioneGiacenze,
                                                    bloccaUtentePerSottogiacenza,
                                                    Elem_Cod,
                                                    Flag_QtaMaggioreZero)
                    End If
                End If

                If (metaschema = "" OrElse
                    (metaschema = "S" AndAlso causaleDaUtilizzareInsetti = CAU_CARICO) OrElse
                    (metaschema = "N" AndAlso causaleDaUtilizzareInsetti = CAU_SCARICO)) Then
                    CercaProdottiBancheDati(Dt_Categorie,
                                            leggiUMformulati,
                                            piva,
                                            INSETTI,
                                            causaleDaUtilizzareInsetti,
                                            FiltroDescrizioneProdotto,
                                            Data_Movimento,
                                            Mode,
                                            xPUARegolamento,
                                            xSa_Cod,
                                            xFabbricato_Cod,
                                            dtProdotti,
                                            objParametri_Super_Server,
                                            objParametri_Server,
                                            objParametri_Utenti,
                                            elemcod_banchedati_sologiacenza,
                                            Flag_QtaNoZero,
                                            Dict_CAC_Codifica_ProdottiAziendali,
                                            xFiltroAggiuntivoGiasAPP,
                                            Flag_QtaMaggioreZero,
                                            xTipoPUARegolamento,
                                            xGiasApp,
                                            leggiGiacenzeConAgroDataFine,
                                            flagDiversificaDesFertilizzanti,
                                            FiltroCodiceProdotto,
                                            0,
                                            ListaGruppiMercePerCategoria, inibisciVisibilitaGruppiMerce, Stato_Cod, w_veg_cod, Lav_Cod)
                End If

                '### TRAPPOLE

                'Solo prodotti in giacenza per alcune categorie specifiche

                Dim causaleDaUtilizzareTrappole As String = causaleDaUtilizzare

                If causaleDaUtilizzare = CAU_SCARICO Then
                    If xGiasApp AndAlso Not bloccaUtentePerSottogiacenza Then
                        'APP - Solo prodotti in giacenza per alcune categorie specifiche
                        If elemcod_banchedati_sologiacenza IsNot Nothing Then
                            If elemcod_banchedati_sologiacenza.Contains(TRAPPOLE) Then
                                causaleDaUtilizzareTrappole = CAU_SCARICO
                                'APP - Passo comunque quelli movimentati almeno una volta
                                'Flag_QtaNoZero già passata dall'esterno
                                Flag_QtaMaggioreZero = False
                            Else
                                causaleDaUtilizzareTrappole = CAU_CARICO
                            End If
                        End If
                    Else
                        ImpostaCausaleEFlagGiacenza(soloInGiacenza,
                                                    causaleDaUtilizzare,
                                                    Flag_QtaNoZero,
                                                    impostazioneGiacenze,
                                                    bloccaUtentePerSottogiacenza,
                                                    Elem_Cod,
                                                    Flag_QtaMaggioreZero)
                    End If
                End If

                If (metaschema = "" OrElse
                    (metaschema = "S" AndAlso causaleDaUtilizzareTrappole = CAU_CARICO) OrElse
                    (metaschema = "N" AndAlso causaleDaUtilizzareTrappole = CAU_SCARICO)) Then
                    CercaProdottiBancheDati(Dt_Categorie,
                                            leggiUMformulati,
                                            piva,
                                            TRAPPOLE,
                                            causaleDaUtilizzareTrappole,
                                            FiltroDescrizioneProdotto,
                                            Data_Movimento,
                                            Mode,
                                            xPUARegolamento,
                                            xSa_Cod,
                                            xFabbricato_Cod,
                                            dtProdotti,
                                            objParametri_Super_Server,
                                            objParametri_Server,
                                            objParametri_Utenti,
                                            elemcod_banchedati_sologiacenza,
                                            Flag_QtaNoZero,
                                            Dict_CAC_Codifica_ProdottiAziendali,
                                            xFiltroAggiuntivoGiasAPP,
                                            Flag_QtaMaggioreZero,
                                            xTipoPUARegolamento,
                                            xGiasApp,
                                            leggiGiacenzeConAgroDataFine,
                                            flagDiversificaDesFertilizzanti,
                                            FiltroCodiceProdotto,
                                            0,
                                            ListaGruppiMercePerCategoria, inibisciVisibilitaGruppiMerce, Stato_Cod, w_veg_cod, Lav_Cod)
                End If


                '### INNESCHI

                'Solo prodotti in giacenza per alcune categorie specifiche

                Dim causaleDaUtilizzareInneschi As String = causaleDaUtilizzare

                If causaleDaUtilizzare = CAU_SCARICO Then
                    If xGiasApp AndAlso Not bloccaUtentePerSottogiacenza Then
                        'APP - Solo prodotti in giacenza per alcune categorie specifiche
                        If elemcod_banchedati_sologiacenza IsNot Nothing Then
                            If elemcod_banchedati_sologiacenza.Contains(INNESCHI) Then
                                causaleDaUtilizzareInneschi = CAU_SCARICO
                                'APP - Passo comunque quelli movimentati almeno una volta
                                'Flag_QtaNoZero già passata dall'esterno
                                Flag_QtaMaggioreZero = False
                            Else
                                causaleDaUtilizzareInneschi = CAU_CARICO
                            End If
                        End If
                    Else
                        ImpostaCausaleEFlagGiacenza(soloInGiacenza,
                                                    causaleDaUtilizzare,
                                                    Flag_QtaNoZero,
                                                    impostazioneGiacenze,
                                                    bloccaUtentePerSottogiacenza,
                                                    Elem_Cod,
                                                    Flag_QtaMaggioreZero)
                    End If
                End If

                If (metaschema = "" OrElse
                    (metaschema = "S" AndAlso causaleDaUtilizzareInneschi = CAU_CARICO) OrElse
                    (metaschema = "N" AndAlso causaleDaUtilizzareInneschi = CAU_SCARICO)) Then
                    CercaProdottiBancheDati(Dt_Categorie,
                                            leggiUMformulati,
                                            piva,
                                            INNESCHI,
                                            causaleDaUtilizzareInneschi,
                                            FiltroDescrizioneProdotto,
                                            Data_Movimento,
                                            Mode,
                                            xPUARegolamento,
                                            xSa_Cod,
                                            xFabbricato_Cod,
                                            dtProdotti,
                                            objParametri_Super_Server,
                                            objParametri_Server,
                                            objParametri_Utenti,
                                            elemcod_banchedati_sologiacenza,
                                            Flag_QtaNoZero,
                                            Dict_CAC_Codifica_ProdottiAziendali,
                                            xFiltroAggiuntivoGiasAPP,
                                            Flag_QtaMaggioreZero,
                                            xTipoPUARegolamento,
                                            xGiasApp,
                                            leggiGiacenzeConAgroDataFine,
                                            flagDiversificaDesFertilizzanti,
                                            FiltroCodiceProdotto,
                                            FiltroCodiceTrappola,
                                            ListaGruppiMercePerCategoria, inibisciVisibilitaGruppiMerce, Stato_Cod, w_veg_cod, Lav_Cod)
                End If

                '### SERVIZI

                'Non gestiscono la giacenza

                Dim causaleDaUtilizzareServizi As String = CAU_CARICO

                If (metaschema = "" OrElse
                    (metaschema = "S" AndAlso causaleDaUtilizzareInneschi = CAU_CARICO) OrElse
                    (metaschema = "N" AndAlso causaleDaUtilizzareInneschi = CAU_SCARICO)) Then
                    CercaProdottiBancheDati(Dt_Categorie,
                                            leggiUMformulati,
                                            piva,
                                            SERVIZI,
                                            causaleDaUtilizzareServizi,
                                            FiltroDescrizioneProdotto,
                                            Data_Movimento,
                                            Mode,
                                            xPUARegolamento,
                                            xSa_Cod,
                                            xFabbricato_Cod,
                                            dtProdotti,
                                            objParametri_Super_Server,
                                            objParametri_Server,
                                            objParametri_Utenti,
                                            elemcod_banchedati_sologiacenza,
                                            Flag_QtaNoZero,
                                            Dict_CAC_Codifica_ProdottiAziendali,
                                            xFiltroAggiuntivoGiasAPP,
                                            Flag_QtaMaggioreZero,
                                            xTipoPUARegolamento,
                                            xGiasApp,
                                            leggiGiacenzeConAgroDataFine,
                                            flagDiversificaDesFertilizzanti,
                                            FiltroCodiceProdotto,
                                            FiltroCodiceTrappola,
                                            ListaGruppiMercePerCategoria, inibisciVisibilitaGruppiMerce, Stato_Cod, w_veg_cod, Lav_Cod)
                End If

            Case Else
                'Consistenza Zootecnica

        End Select

        Return dtProdotti

    End Function

    Private Sub CercaProdottiBancheDati(ByVal Dt_Categorie As DataTable,
                                        ByVal leggiUMformulati As Boolean,
                                        ByVal piva As String,
                                        ByVal Elem_Cod As Integer,
                                        ByVal Cau_Mov As String,
                                        ByVal FiltroDescrizioneProdotto As String,
                                        ByVal Data_Movimento As Date,
                                        ByVal Mode As String,
                                        ByVal xPuaRegolamento As Integer,
                                        ByVal xSa_Cod As Integer,
                                        ByVal xFabbricato_Cod As Integer,
                                        ByRef dtProdotti As DataTable,
                                        ByVal objParametri_Super_Server As AgronicaCoreParametri,
                                        ByVal objParametri_Server As AgronicaCoreParametri,
                                        ByVal objParametri_Utenti As AgronicaCoreParametri,
                                        ByVal elemcod_banchedati_sologiacenza As Integer(),
                                        ByVal Flag_QtaNoZero As Boolean,
                                        ByVal Dict_CAC_Codifica_ProdottiAziendali As Dictionary(Of String, String),
                                        ByVal xFiltroAggiuntivoGiasAPP As String,
                                        ByVal Flag_QtaMaggioreZero As Boolean,
                                        ByVal xTipoPUARegolamento As Integer,
                                        ByVal xGiasApp As Boolean,
                                        ByVal leggiGiacenzeConAgroDataFine As Boolean,
                                        ByVal flagDiversificaDesFertilizzanti As Boolean,
                                        ByVal FiltroCodiceProdotto As Integer,
                                        ByVal FiltroCodiceTrappola As Integer,
                                        ByVal gruppiMerceDefaultPerCategoria As List(Of ImpostazioneDefault_GruppiMerce),
                                        ByVal inibisciVisibilitaGruppiMerce As Boolean,
                                        ByVal Stato_Cod As String,
                                        ByVal Veg_Cod As Integer,
                                        ByVal Lav_Cod As Integer)

        If xFiltroAggiuntivoGiasAPP <> "" Then
            xFiltroAggiuntivoGiasAPP = " AND " + xFiltroAggiuntivoGiasAPP
        End If

        Dim cmb_Prodotti As New DropDownList
        Dim DT_Risultato As New DataTable

        Dim Flag_VisualizzaProCod As Boolean = True
        If Elem_Cod = CARBURANTI Then
            Flag_VisualizzaProCod = False
        End If

        Dim PuaRegolamento As Integer = 0
        Dim TipoPUARegolamento As Integer = 0
        If Elem_Cod = FERTILIZZANTI Then
            If xPuaRegolamento <> 0 Then
                PuaRegolamento = xPuaRegolamento
            End If
            If xTipoPUARegolamento <> 0 Then
                TipoPUARegolamento = xTipoPUARegolamento
            End If
        End If

        Dim Flag_CaricaUdmCod As Boolean = False
        If Cau_Mov = CAU_CARICO Then
            If Elem_Cod = FORMULATI And leggiUMformulati Then
                Flag_CaricaUdmCod = True
            End If
        End If

        Dim Flag_IncludiNPK_Desc = False
        Dim Flag_IncludiClassificazione = False
        If flagDiversificaDesFertilizzanti AndAlso Elem_Cod = FERTILIZZANTI Then
            Flag_VisualizzaProCod = False
            Flag_IncludiNPK_Desc = True
            Flag_IncludiClassificazione = True
        End If

        Dim tabellaNomeColonnaCod As String = "" 'Nome della colonna Pro_Cod specifica dell'elem_cod, esempio: Ins_Cod, Cer_Cod 

        Dim TipoRichiestoTabellaFormulato As String = ""

        'Se viene passato il Lav_Cod e l'Elem_Cod è FORMULATO ottengo il Tipo di Formulato con cui fare filtro
        If Elem_Cod = FORMULATI AndAlso Lav_Cod > 0 Then

            Dim objSTD_Utility As New AgronicaControlli_2010.STD_Utility

            TipoRichiestoTabellaFormulato = objSTD_Utility.GetTipiFormulatiRichiesti(Lav_Cod.ToString())

        Else

            'Casanova 24/10/2024 Per il giro dell'APP devo caricare solamente le nuove trappole che sono classificate come formulati 
            'ma che hanno le loro classificazioni specifiche CLASS_COD IN (602,613,617,1003).
            'Se invece devo caricare solo i formulati sull'APP escludo quelli con il CLASS_COD delle trappole

            If xGiasApp Then
                If Elem_Cod = TRAPPOLE Then
                    Elem_Cod = FORMULATI
                    TipoRichiestoTabellaFormulato = (+enum_TipoFormulato.InstallazioneTrappoleCattureMassa).ToString()
                ElseIf Elem_Cod = FORMULATI Then
                    TipoRichiestoTabellaFormulato = (-enum_TipoFormulato.InstallazioneTrappoleCattureMassa).ToString()
                End If
            End If

        End If

        Select Case Mode.ToLower

            Case "trasferimento"

                'Carico i prodotti della categoria scelta nella cmb_CatProdotto e con la descrizione scelta

                Dim clc = New CaricaListControl_2010
                clc.Prodotti(cmb_Prodotti,
                             False,
                             "",
                             "",
                             Cau_Mov,
                             piva,
                             xSa_Cod,
                             xFabbricato_Cod,
                             Elem_Cod,
                             False,
                             FiltroDescrizioneProdotto,
                             Flag_VisualizzaProCod,
                             False,
                             FiltroCodiceProdotto,
                             0,
                             Data_Movimento,
                             PuaRegolamento,
                             0,
                             True,
                             False,
                             "",
                             "",
                             objParametri_Server,
                             objParametri_Utenti,
                             DT_Risultato,
                             objParametri_Super_Server,
                             Flag_QtaNoZero:=Flag_QtaNoZero,
                             xFiltroAggiuntivoGiasAPP:=xFiltroAggiuntivoGiasAPP,
                             Tipo_PuaRegolamento:=TipoPUARegolamento,
                             Flag_QtaMaggioreZero:=Flag_QtaMaggioreZero,
                             leggiGiacenzeConAgroDataFine:=leggiGiacenzeConAgroDataFine,
                             Flag_IncludiNPK_Desc:=Flag_IncludiNPK_Desc,
                             Flag_IncludiClassificazione:=Flag_IncludiClassificazione,
                             Trap_Cod:=FiltroCodiceTrappola,
                             NomeCodice:=tabellaNomeColonnaCod,
                             gruppiMerceDefaultPerCategoria:=gruppiMerceDefaultPerCategoria,
                             inibisciVisibilitaGruppiMerce:=inibisciVisibilitaGruppiMerce,
                             Stato_Cod:=Stato_Cod,
                             Veg_Cod:=Veg_Cod,
                             TipiRichiestiFormulati:=TipoRichiestoTabellaFormulato)

            Case Else

                Dim Flag_FiltraRevocati As Boolean = True
                If Cau_Mov = CAU_SCARICO Then
                    Flag_FiltraRevocati = False
                End If

                Dim clc = New CaricaListControl_2010
                clc.Prodotti(cmb_Prodotti,
                             False,
                             "",
                             "",
                             Cau_Mov,
                             piva,
                             xSa_Cod,
                             xFabbricato_Cod,
                             Elem_Cod,
                             False,
                             FiltroDescrizioneProdotto,
                             Flag_VisualizzaProCod,
                             Flag_CaricaUdmCod,
                             FiltroCodiceProdotto,
                             0,
                             Data_Movimento,
                             PuaRegolamento,
                             0,
                             True,
                             Flag_FiltraRevocati,
                             "",
                             "",
                             objParametri_Server,
                             objParametri_Utenti,
                             DT_Risultato,
                             objParametri_Super_Server,
                             Flag_QtaNoZero:=Flag_QtaNoZero,
                             xFiltroAggiuntivoGiasAPP:=xFiltroAggiuntivoGiasAPP,
                             Tipo_PuaRegolamento:=TipoPUARegolamento,
                             Flag_QtaMaggioreZero:=Flag_QtaMaggioreZero,
                             leggiGiacenzeConAgroDataFine:=leggiGiacenzeConAgroDataFine,
                             Flag_IncludiNPK_Desc:=Flag_IncludiNPK_Desc,
                             Flag_IncludiClassificazione:=Flag_IncludiClassificazione,
                             Trap_Cod:=FiltroCodiceTrappola,
                             NomeCodice:=tabellaNomeColonnaCod,
                             gruppiMerceDefaultPerCategoria:=gruppiMerceDefaultPerCategoria,
                             inibisciVisibilitaGruppiMerce:=inibisciVisibilitaGruppiMerce,
                             Stato_Cod:=Stato_Cod,
                             Veg_Cod:=Veg_Cod,
                             TipiRichiestiFormulati:=TipoRichiestoTabellaFormulato)

        End Select

        Dim objCat As New Categorie_Magazzino_R

        Dim drProdotti As DataRow
        Dim w_nomecomune As String = (From cat In Dt_Categorie
                                      Where cat("Elem_Cod") = Elem_Cod
                                      Select cat("NomeComune")).FirstOrDefault()
        Dim x_Pro_Des As String = ""

        Dim cod_articolo_found = False
        Dim cod_articolo As String = ""

        'Calcolo delle variabili necessarie alla gestione gruppi merce in caso di cau_mov di carico
        Dim dtExtraPriv As New DataTable()
        'Dim gestioneGruppiUtentiMerce As Boolean = False
        Dim dtGruppiUtenteMerce As New DataTable()
        Dim defPerElemCodCorrente As New ImpostazioneDefault_GruppiMerce()
        Dim listaGruppiMerceVisibili As New List(Of Integer)()

        If Not IsNothing(gruppiMerceDefaultPerCategoria) AndAlso gruppiMerceDefaultPerCategoria.Count > 0 AndAlso
            Cau_Mov <> CAU_SCARICO Then
            'Se la gestione dei gruppi merce è abilitata, devo aggiungere le rispettive colonne al datatable restituito, queste sono già gestite dalla query in caso di CAU_SCARICO

            Dim listaDefPerElemCodCorrente = gruppiMerceDefaultPerCategoria.Where(Function(defGruppoMerce) defGruppoMerce.Piva = piva AndAlso defGruppoMerce.Elem_Cod = Elem_Cod)

            If listaDefPerElemCodCorrente.Count = 1 Then
                defPerElemCodCorrente = listaDefPerElemCodCorrente.First()
            End If

            Dim handleProdottiExtraPrivata As New Prodotti_Extra_Privata_R
            dtExtraPriv = handleProdottiExtraPrivata.Leggi(piva, 0, Elem_Cod, 0,
                                                                    Nothing, Nothing, Nothing,
                                                                    Nothing, Nothing, Nothing,
                                                                    "", "", "", Nothing, Nothing,
                                                                    Nothing, Nothing, Nothing,
                                                                    enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                                    "", "", objParametri_Server)

            If objParametri_Server.UtenteUsername <> objParametri_Server.SuperUserUsername AndAlso inibisciVisibilitaGruppiMerce = False Then

                'Calcolo gestione visibilità gruppi merce 
                Dim objGruppiUtenteMerce As New Gruppi_UtenteXGruppi_Merce_R(objParametri_Server, objParametri_Utenti)
                dtGruppiUtenteMerce = objGruppiUtenteMerce.Leggi("Gruppi_UtenteXGruppi_Merce.Piva IN('" & piva & "')", "")

                If dtGruppiUtenteMerce.Rows.Count > 0 Then

                    Dim sqlGruppiMerceVIsibili = objGruppiUtenteMerce.ComponiSql_DistinctGruppiMerce_X_GruppiUtente("", piva)
                    Dim dtGruppiMerceVisibiliUtente = objGruppiUtenteMerce.EseguiQuery_Lettura(objParametri_Server, sqlGruppiMerceVIsibili, "")

                    listaGruppiMerceVisibili = dtGruppiMerceVisibiliUtente.AsEnumerable().Select(Of Integer)(Function(rowMerce) rowMerce.Item("Id_Gruppo_Merce")).ToList()
                End If

            End If


        End If


        If Elem_Cod = FERTILIZZANTI Or Elem_Cod = TRAPPOLE Or Elem_Cod = FORMULATI Then

            Dim objCore As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
            Dim Udm_Dose As Integer

            ' Per FERTILIZZANTI, TRAPPOLE e FORMULATI ho bisogno di altri campi che sono solo nel DataTable
            For Each elem As DataRow In DT_Risultato.Rows

                'Se la gestione gruppi merce è attiva, devo "leggere" il gruppo merce associato al prodotto dal dtExtraPriv oppure prendere il suo default
                'Se anche la gestione visibilità è attiva devo verificare se tale gruppo merce è fra quelli dei gruppi degli utenti, in caso contrario devo passare al giro successivo del for
                Dim gruppoMerceProdotto As Integer = 0
                Dim desGruppoMerceProdotto As String = ""

                If Not IsNothing(gruppiMerceDefaultPerCategoria) AndAlso gruppiMerceDefaultPerCategoria.Count > 0 Then

                    If Cau_Mov = CAU_SCARICO Then
                        'In caso di scarico la gestione viene fatta attraverso la query
                        gruppoMerceProdotto = elem.Item("Id_Gruppo_Merce")
                        desGruppoMerceProdotto = elem.Item("Des_Gruppo_Merce")

                    Else
                        'Altrimenti, la ricerca dei prodotti è fatta attraverso webservice, quindi ottengo le colonne via codice
                        If dtExtraPriv.Rows.Count > 0 Then

                            Dim extraPrivProd = dtExtraPriv.AsEnumerable().Where(Function(extraPriv) extraPriv.Item("Piva") = piva AndAlso extraPriv.Item("Pro_Cod") = elem.Item(tabellaNomeColonnaCod))

                            If extraPrivProd.Count = 1 Then

                                gruppoMerceProdotto = If(IsDBNull(extraPrivProd(0)("Id_Gruppo_Merce")), 0, extraPrivProd(0)("Id_Gruppo_Merce"))

                                desGruppoMerceProdotto = If(IsDBNull(extraPrivProd(0)("Id_Gruppo_Merce")), "",
                                    extraPrivProd(0)("Gruppo_Merce_Codice") & " " & extraPrivProd(0)("Gruppo_Merce_Descrizione"))
                            End If

                        End If

                        If gruppoMerceProdotto = 0 Then
                            'In questo caso cerco gruppo merce di default
                            gruppoMerceProdotto = defPerElemCodCorrente.Id_Gruppo_Merce
                            desGruppoMerceProdotto = If(defPerElemCodCorrente.Id_Gruppo_Merce = 0, "", defPerElemCodCorrente.Codice & " " & defPerElemCodCorrente.Descrizione)
                        End If

                        If dtGruppiUtenteMerce.Rows.Count > 0 Then

                            If Not listaGruppiMerceVisibili.Contains(gruppoMerceProdotto) Then
                                Continue For
                            End If

                        End If

                    End If

                End If


                drProdotti = dtProdotti.NewRow


                If Not IsNothing(gruppiMerceDefaultPerCategoria) AndAlso gruppiMerceDefaultPerCategoria.Count > 0 Then
                    drProdotti.Item("Id_Gruppo_Merce") = gruppoMerceProdotto
                    drProdotti.Item("Des_Gruppo_Merce") = desGruppoMerceProdotto
                End If


                If Elem_Cod = FERTILIZZANTI Then

                    cod_articolo_found = Dict_CAC_Codifica_ProdottiAziendali.TryGetValue(CStr(Elem_Cod) & "_" & CStr(elem.Item("Fer_Cod")), cod_articolo)
                    If Not cod_articolo_found Then
                        cod_articolo = ""
                    End If

                    drProdotti.Item("Prodotto_Cod") = elem.Item("Fer_Cod")
                    x_Pro_Des = ""
                    If Flag_VisualizzaProCod = True AndAlso Cau_Mov = CAU_SCARICO Then
                        x_Pro_Des = CStr(elem.Item("Fer_Des")) & " (" & CStr(Math.Abs(elem.Item("Fer_Cod"))) + ")"
                    Else
                        x_Pro_Des = CStr(elem.Item("Fer_Des"))
                    End If
                    If Not String.IsNullOrWhiteSpace(cod_articolo) Then
                        x_Pro_Des = x_Pro_Des & " (" & Gias.CodArticolo & ": " & cod_articolo & ")"
                    End If

                    drProdotti.Item("Prodotto_Des") = x_Pro_Des

                    drProdotti.Item("N") = elem.Item("N")
                    drProdotti.Item("P2O5") = elem.Item("P2O5")
                    drProdotti.Item("K2O") = elem.Item("K2O")
                    drProdotti.Item("Cu") = elem.Item("Cu")
                    drProdotti.Item("Uso") = 0
                    drProdotti.Item("Udm_Cod") = 0
                    If Not xGiasApp Then
                        drProdotti.Item("Veg_Cod") = 0
                        drProdotti.Item("Cul_Cod") = 0
                        drProdotti.Item("Reg_Cod") = 0
                        drProdotti.Item("Sem_Cod") = 0
                        drProdotti.Item("GRVA_COD_VEG") = 0
                        drProdotti.Item("Cat_Cod") = 0
                        drProdotti.Item("LegatoALinea") = 0
                        drProdotti.Item("Mat_Cod_OMNI") = 0
                        drProdotti.Item("Piva") = ""
                        drProdotti.Item("Qta_Extra") = 0
                        drProdotti.Item("Udm_Cod_Extra") = 0
                        drProdotti.Item("Veg_Des") = ""
                        drProdotti.Item("Cul_Des") = ""
                        drProdotti.Item("Rag_Soc_Proprietaria") = ""
                        drProdotti.Item("Utente_Creazione") = ""
                        drProdotti.Item("Utente_Modifica") = ""
                        drProdotti.Item("Data_Creazione") = AGRODATAINIZIO
                        drProdotti.Item("Data_Modifica") = AGRODATAINIZIO
                        drProdotti.Item("Otabella_Cod_Base") = 0
                        drProdotti.Item("Sa_Cod") = 0
                        drProdotti.Item("Cod_Articolo") = ""
                        drProdotti.Item("ChkAlias") = 0
                        drProdotti.Item("Codice_Esterno") = ""
                        cod_articolo_found = Dict_CAC_Codifica_ProdottiAziendali.TryGetValue(CStr(Elem_Cod) & "_" & CStr(elem.Item("Fer_Cod")), cod_articolo)
                        If cod_articolo_found Then
                            drProdotti.Item("Cod_Articolo") = cod_articolo
                        End If
                    End If
                Else
                    If Elem_Cod = TRAPPOLE Then

                        cod_articolo_found = Dict_CAC_Codifica_ProdottiAziendali.TryGetValue(CStr(Elem_Cod) & "_" & CStr(elem.Item("Trap_Cod")), cod_articolo)
                        If Not cod_articolo_found Then
                            cod_articolo = ""
                        End If

                        drProdotti.Item("Prodotto_Cod") = elem.Item("Trap_Cod")
                        drProdotti.Item("Prodotto_Des") = elem.Item("Trap_Des")
                        If Not String.IsNullOrWhiteSpace(cod_articolo) Then
                            drProdotti.Item("Prodotto_Des") = drProdotti.Item("Prodotto_Des") & " (" & Gias.CodArticolo & ": " & cod_articolo & ")"
                        End If

                        drProdotti.Item("N") = 0
                        drProdotti.Item("P2O5") = 0
                        drProdotti.Item("K2O") = 0
                        drProdotti.Item("Cu") = 0
                        drProdotti.Item("Uso") = elem.Item("Uso")
                        drProdotti.Item("Udm_Cod") = 0
                        If Not xGiasApp Then
                            drProdotti.Item("Veg_Cod") = 0
                            drProdotti.Item("Cul_Cod") = 0
                            drProdotti.Item("Reg_Cod") = 0
                            drProdotti.Item("Sem_Cod") = 0
                            drProdotti.Item("GRVA_COD_VEG") = 0
                            drProdotti.Item("Cat_Cod") = 0
                            drProdotti.Item("LegatoALinea") = 0
                            drProdotti.Item("Mat_Cod_OMNI") = 0
                            drProdotti.Item("Piva") = ""
                            drProdotti.Item("Qta_Extra") = 0
                            drProdotti.Item("Udm_Cod_Extra") = 0
                            drProdotti.Item("Veg_Des") = ""
                            drProdotti.Item("Cul_Des") = ""
                            drProdotti.Item("Rag_Soc_Proprietaria") = ""
                            drProdotti.Item("Utente_Creazione") = ""
                            drProdotti.Item("Utente_Modifica") = ""
                            drProdotti.Item("Data_Creazione") = AGRODATAINIZIO
                            drProdotti.Item("Data_Modifica") = AGRODATAINIZIO
                            drProdotti.Item("Otabella_Cod_Base") = 0
                            drProdotti.Item("Sa_Cod") = 0
                            drProdotti.Item("Cod_Articolo") = ""
                            drProdotti.Item("ChkAlias") = 0
                            drProdotti.Item("Codice_Esterno") = ""
                            cod_articolo_found = Dict_CAC_Codifica_ProdottiAziendali.TryGetValue(CStr(Elem_Cod) & "_" & CStr(elem.Item("Trap_Cod")), cod_articolo)
                            If cod_articolo_found Then
                                drProdotti.Item("Cod_Articolo") = cod_articolo
                            End If
                        End If
                    Else
                        If Elem_Cod = FORMULATI Then

                            ' Cerca decodifica prodotto
                            cod_articolo_found = Dict_CAC_Codifica_ProdottiAziendali.TryGetValue(CStr(Elem_Cod) & "_" & CStr(elem.Item("Fr_Cod")), cod_articolo)
                            If Not cod_articolo_found Then
                                cod_articolo = ""
                            End If

                            drProdotti.Item("Prodotto_Cod") = elem.Item("Fr_Cod")
                            drProdotti.Item("Prodotto_Des") = elem.Item("Fr_Des")
                            If Not String.IsNullOrWhiteSpace(cod_articolo) Then
                                drProdotti.Item("Prodotto_Des") = drProdotti.Item("Prodotto_Des") & " (" & Gias.CodArticolo & ": " & cod_articolo & ")"
                            End If

                            drProdotti.Item("N") = 0
                            drProdotti.Item("P2O5") = 0
                            drProdotti.Item("K2O") = 0
                            drProdotti.Item("Cu") = 0
                            drProdotti.Item("Uso") = 0
                            'In scarico (7350) Udm_Dose dal WS non viene mai passato anche se Flag_CaricaUdmCod è passato a true
                            'In carico (7300) Udm_Dose dal WS viene passato sempre anche se Flag_CaricaUdmCod è passato a false
                            'Quindi l'utilizzo per impostare l'U.M. da utilizzare
                            drProdotti.Item("Udm_Cod") = 0
                            If Cau_Mov = CAU_CARICO AndAlso (Flag_CaricaUdmCod OrElse Not xGiasApp) Then
                                Udm_Dose = 0
                                If drProdotti.Table.Columns.Contains("Udm_Cod_A") AndAlso CInt(elem.Item("Udm_Cod_A")) <> 0 Then
                                    Udm_Dose = CInt(elem.Item("Udm_Cod_A"))
                                ElseIf drProdotti.Table.Columns.Contains("Udm_Cod_I") Then
                                    Udm_Dose = CInt(elem.Item("Udm_Cod_I"))
                                End If
                                If Udm_Dose <> 0 Then
                                    drProdotti.Item("Udm_Cod") = objCore.Converti_Kg_L_from_UdmCod(Udm_Dose, "", "")
                                End If
                            End If

                            If DT_Risultato.Columns.Contains("IsTrappolaFormulato") Then
                                drProdotti.Item("IsTrappolaFormulato") = elem.Item("IsTrappolaFormulato")
                            End If

                            If Not xGiasApp Then
                                drProdotti.Item("Veg_Cod") = 0
                                drProdotti.Item("Cul_Cod") = 0
                                drProdotti.Item("Reg_Cod") = 0
                                drProdotti.Item("Sem_Cod") = 0
                                drProdotti.Item("GRVA_COD_VEG") = 0
                                drProdotti.Item("Cat_Cod") = 0
                                drProdotti.Item("LegatoALinea") = 0
                                drProdotti.Item("Mat_Cod_OMNI") = 0
                                drProdotti.Item("Piva") = ""
                                drProdotti.Item("Qta_Extra") = 0
                                drProdotti.Item("Udm_Cod_Extra") = 0
                                drProdotti.Item("Veg_Des") = ""
                                drProdotti.Item("Cul_Des") = ""
                                drProdotti.Item("Rag_Soc_Proprietaria") = ""
                                drProdotti.Item("Utente_Creazione") = ""
                                drProdotti.Item("Utente_Modifica") = ""
                                drProdotti.Item("Data_Creazione") = AGRODATAINIZIO
                                drProdotti.Item("Data_Modifica") = AGRODATAINIZIO
                                drProdotti.Item("Otabella_Cod_Base") = 0
                                drProdotti.Item("Sa_Cod") = 0
                                drProdotti.Item("Cod_Articolo") = ""
                                drProdotti.Item("ChkAlias") = 0
                                drProdotti.Item("Codice_Esterno") = ""
                                cod_articolo_found = Dict_CAC_Codifica_ProdottiAziendali.TryGetValue(CStr(Elem_Cod) & "_" & CStr(elem.Item("Fr_Cod")), cod_articolo)
                                If cod_articolo_found Then
                                    drProdotti.Item("Cod_Articolo") = cod_articolo
                                End If
                            End If
                        End If
                    End If
                End If

                drProdotti.Item("Elem_Cod") = Elem_Cod
                drProdotti.Item("NomeComune") = w_nomecomune

                ' In caso di ricerca di soli prodotti in giacenza potrebbero arrivare righe doppie perchè c'è giacenza per U.M. diverse
                ' In più esistono formulati che vengono passati più volte avendo avuto più periodi di sospensione
                '  Quindi riunisco i risultati
                Dim w_cod = (From prod In dtProdotti
                             Where prod("Elem_Cod") = Elem_Cod And
                                prod("Prodotto_Cod") = drProdotti.Item("Prodotto_Cod")
                             Select prod("Prodotto_Cod")).FirstOrDefault()
                If w_cod Is Nothing Then
                    dtProdotti.Rows.Add(drProdotti)
                End If

            Next

        Else

            'No fertilizzanti e no prodotti
            For Each elem As ListItem In cmb_Prodotti.Items

                'Se la gestione gruppi merce è attiva, devo "leggere" il gruppo merce associato al prodotto dal dtExtraPriv oppure prendere il suo default
                'Se anche la gestione visibilità è attiva devo verificare se tale gruppo merce è fra quelli dei gruppi degli utenti, in caso contrario devo passare al giro successivo del for
                Dim gruppoMerceProdotto As Integer = 0
                Dim desGruppoMerceProdotto As String = ""

                If Not IsNothing(gruppiMerceDefaultPerCategoria) AndAlso gruppiMerceDefaultPerCategoria.Count > 0 Then

                    If Cau_Mov = CAU_SCARICO Then
                        'In caso di scarico la gestione viene fatta attraverso la query
                        Dim drGiacenzaProdRis = DT_Risultato.AsEnumerable().Where(Function(dr) dr.Item(tabellaNomeColonnaCod) = elem.Value).First()

                        gruppoMerceProdotto = drGiacenzaProdRis.Item("Id_Gruppo_Merce")
                        desGruppoMerceProdotto = drGiacenzaProdRis.Item("Des_Gruppo_Merce")
                    Else
                        'Altrimenti, la ricerca dei prodotti è fatta attraverso webservice, quindi ottengo le colonne via codice
                        If dtExtraPriv.Rows.Count > 0 Then

                            Dim extraPrivProd = dtExtraPriv.AsEnumerable().Where(Function(extraPriv) extraPriv.Item("Piva") = piva AndAlso extraPriv.Item("Pro_Cod") = elem.Value)

                            If extraPrivProd.Count = 1 Then

                                gruppoMerceProdotto = If(IsDBNull(extraPrivProd(0)("Id_Gruppo_Merce")), 0, extraPrivProd(0)("Id_Gruppo_Merce"))

                                desGruppoMerceProdotto = If(IsDBNull(extraPrivProd(0)("Id_Gruppo_Merce")), "",
                                    extraPrivProd(0)("Gruppo_Merce_Codice") & " " & extraPrivProd(0)("Gruppo_Merce_Descrizione"))
                            End If

                        End If

                        If gruppoMerceProdotto = 0 Then
                            'In questo caso cerco gruppo merce di default
                            gruppoMerceProdotto = defPerElemCodCorrente.Id_Gruppo_Merce
                            desGruppoMerceProdotto = If(defPerElemCodCorrente.Id_Gruppo_Merce = 0, "", defPerElemCodCorrente.Codice & " " & defPerElemCodCorrente.Descrizione)

                        End If

                        If dtGruppiUtenteMerce.Rows.Count > 0 Then

                            If Not listaGruppiMerceVisibili.Contains(gruppoMerceProdotto) Then
                                Continue For
                            End If

                        End If

                    End If

                End If

                drProdotti = dtProdotti.NewRow


                If Not IsNothing(gruppiMerceDefaultPerCategoria) AndAlso gruppiMerceDefaultPerCategoria.Count > 0 Then
                    drProdotti.Item("Id_Gruppo_Merce") = gruppoMerceProdotto
                    drProdotti.Item("Des_Gruppo_Merce") = desGruppoMerceProdotto
                End If

                drProdotti.Item("Elem_Cod") = Elem_Cod
                drProdotti.Item("Prodotto_Cod") = elem.Value

                cod_articolo_found = Dict_CAC_Codifica_ProdottiAziendali.TryGetValue(CStr(Elem_Cod) & "_" & CStr(drProdotti.Item("Prodotto_Cod")), cod_articolo)
                If Not cod_articolo_found Then
                    cod_articolo = ""
                End If

                drProdotti.Item("Prodotto_Des") = elem.Text
                If Not String.IsNullOrWhiteSpace(cod_articolo) Then
                    drProdotti.Item("Prodotto_Des") = drProdotti.Item("Prodotto_Des") & " (" & Gias.CodArticolo & ": " & cod_articolo & ")"
                End If

                drProdotti.Item("NomeComune") = w_nomecomune
                drProdotti.Item("N") = 0
                drProdotti.Item("P2O5") = 0
                drProdotti.Item("K2O") = 0
                drProdotti.Item("Cu") = 0
                drProdotti.Item("Uso") = 0
                drProdotti.Item("Udm_Cod") = 0
                If Not xGiasApp Then
                    drProdotti.Item("Veg_Cod") = 0
                    drProdotti.Item("Cul_Cod") = 0
                    drProdotti.Item("Reg_Cod") = 0
                    drProdotti.Item("Sem_Cod") = 0
                    drProdotti.Item("GRVA_COD_VEG") = 0
                    drProdotti.Item("Cat_Cod") = 0
                    drProdotti.Item("LegatoALinea") = 0
                    drProdotti.Item("Mat_Cod_OMNI") = 0
                    drProdotti.Item("Piva") = ""
                    drProdotti.Item("Qta_Extra") = 0
                    drProdotti.Item("Udm_Cod_Extra") = 0
                    drProdotti.Item("Veg_Des") = ""
                    drProdotti.Item("Cul_Des") = ""
                    drProdotti.Item("Rag_Soc_Proprietaria") = ""
                    drProdotti.Item("Utente_Creazione") = ""
                    drProdotti.Item("Utente_Modifica") = ""
                    drProdotti.Item("Data_Creazione") = AGRODATAINIZIO
                    drProdotti.Item("Data_Modifica") = AGRODATAINIZIO
                    drProdotti.Item("Otabella_Cod_Base") = 0
                    drProdotti.Item("Sa_Cod") = 0
                    drProdotti.Item("Cod_Articolo") = ""
                    drProdotti.Item("ChkAlias") = 0
                    drProdotti.Item("Codice_Esterno") = ""
                    cod_articolo_found = Dict_CAC_Codifica_ProdottiAziendali.TryGetValue(CStr(Elem_Cod) & "_" & CStr(drProdotti.Item("Prodotto_Cod")), cod_articolo)
                    If cod_articolo_found Then
                        drProdotti.Item("Cod_Articolo") = cod_articolo
                    End If
                End If

                ' In caso di ricerca di soli prodotti in giacenza potrebbero arrivare righe doppie perchè c'è giacenza per U.M. diverse
                '  Quindi riunisco i risultati
                If Cau_Mov = CAU_SCARICO OrElse (Not elemcod_banchedati_sologiacenza Is Nothing AndAlso elemcod_banchedati_sologiacenza.Contains(Elem_Cod)) Then
                    Dim w_cod = (From prod In dtProdotti
                                 Where prod("Elem_Cod") = Elem_Cod And
                                 prod("Prodotto_Cod") = drProdotti.Item("Prodotto_Cod")
                                 Select prod("Prodotto_Cod")).FirstOrDefault()
                    If w_cod Is Nothing Then
                        dtProdotti.Rows.Add(drProdotti)
                    End If
                Else
                    dtProdotti.Rows.Add(drProdotti)
                End If

            Next

        End If

    End Sub

    Private Sub ImpostaCausaleEFlagGiacenza(ByVal soloInGiacenza As Boolean,
                                    ByRef causaleDaUtilizzare As String,
                                      ByRef Flag_QtaNoZero As Boolean,
                                      ByVal impostazioneGiacenze As String(),
                                      ByRef bloccaUtentePerSottogiacenza As Boolean,
                                      ByVal Elem_Cod As Integer,
                                      ByRef Flag_QtaMaggioreZero As Boolean)

        'Se dall'esterno arriva il flag soloInGiacenza = True comanda quello
        ' diversamente vado a cercare impostazione giacenze da Elem_Cod e tramite flag bloccaUtentePerSottogiacenza
        If soloInGiacenza Then
            Flag_QtaNoZero = True
            Flag_QtaMaggioreZero = True
        Else
            ControllaFlagGiacenze(causaleDaUtilizzare, Flag_QtaNoZero, impostazioneGiacenze, bloccaUtentePerSottogiacenza, Elem_Cod, Flag_QtaMaggioreZero)
        End If

    End Sub


    Private Sub ControllaFlagGiacenze(ByRef causaleDaUtilizzare As String,
                                      ByRef Flag_QtaNoZero As Boolean,
                                      ByVal impostazioneGiacenze As String(),
                                      ByVal bloccaUtentePerSottogiacenza As Boolean,
                                      ByVal Elem_Cod As Integer,
                                      ByRef Flag_QtaMaggioreZero As Boolean)

        Dim elem_cod_impostazione() As String = Nothing

        causaleDaUtilizzare = CAU_SCARICO

        If bloccaUtentePerSottogiacenza Then
            Flag_QtaNoZero = True
            Flag_QtaMaggioreZero = True
        Else
            If Not impostazioneGiacenze Is Nothing Then
                For Each s As String In impostazioneGiacenze
                    elem_cod_impostazione = s.Split("_")
                    If CInt(elem_cod_impostazione(0)) = Elem_Cod Then  '0=soloMovimentati (default); 1=soloPresenti; 2=tutti
                        If elem_cod_impostazione(1) = enum_Gestione_Giacenze.TuttiProdotti Then
                            causaleDaUtilizzare = CAU_CARICO
                        Else
                            If elem_cod_impostazione(1) = enum_Gestione_Giacenze.SoloPresenti Then
                                Flag_QtaNoZero = True
                                Flag_QtaMaggioreZero = True
                            Else
                                If elem_cod_impostazione(1) = enum_Gestione_Giacenze.SoloMovimentati Then
                                    Flag_QtaNoZero = False
                                    Flag_QtaMaggioreZero = False
                                End If
                            End If
                        End If
                        Exit For
                    End If
                Next
            End If
        End If
    End Sub

    Private Function NormalizzaSpecieVarieta(ByVal Elenco_Specie As String, ByVal Elenco_Varieta As String, ByVal objP_server As String) As List(Of String)

        'Se c'è più di una specie filtrata e c'è almeno una varietà, potrebbe esserci una specie da filtrare per intero e altre da filtrare solo per varietà
        'Quindi normalizzo la situazione
        'Si dà per scontato che se arriva una varietà arriva anche la sua specie, a meno che non arrivino solo le varietà

        Dim spevarList As New List(Of String)

        If Not String.IsNullOrEmpty(Elenco_Specie) OrElse Not String.IsNullOrEmpty(Elenco_Varieta) Then

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim specie_Array As String() = {}
            If Not String.IsNullOrEmpty(Elenco_Specie) Then
                specie_Array = Elenco_Specie.Split("|")
            End If
            Dim varieta_Array As String() = {}
            If Not String.IsNullOrEmpty(Elenco_Varieta) Then
                varieta_Array = Elenco_Varieta.Split("|")
            End If

            Dim dtVar As DataTable = Nothing
            Dim cultivar_R As New Cultivar_R
            If specie_Array.Length > 0 Then

                If varieta_Array.Length = 0 Then
                    'Nessuna varietà, aggiungo tutte le specie
                    For Each spe In specie_Array
                        spevarList.Add(spe & "|" & "0")
                    Next
                Else
                    If specie_Array.Length = 1 AndAlso varieta_Array.Length > 0 Then
                        'Una sola specie: tutte le varietà sono sicuramente di questa specie
                        For Each var In varieta_Array
                            spevarList.Add(specie_Array(0) & "|" & var)
                        Next
                    Else
                        'Più specie
                        For Each spe In specie_Array
                            Dim foundVarieta = False
                            For Each var In varieta_Array
                                dtVar = cultivar_R.Leggi(CInt(var), CInt(spe), "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
                                If dtVar.Rows.Count > 0 Then
                                    spevarList.Add(spe & "|" & var)
                                    foundVarieta = True
                                End If
                            Next
                            If foundVarieta = False Then
                                spevarList.Add(spe & "|" & "0")
                            End If
                        Next
                    End If
                End If

                If specie_Array.Length = 0 AndAlso varieta_Array.Length > 0 Then
                    'Passate le varietà ma non le specie
                    For Each var In varieta_Array
                        dtVar = cultivar_R.Leggi(CInt(var), 0, "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
                        If dtVar.Rows.Count = 1 Then
                            spevarList.Add(CStr(dtVar.Rows(0).Item("Veg_Cod")) & "|" & var)
                        End If
                    Next
                End If

            End If

        End If

        Return spevarList

    End Function

    Private Sub ImpostaDatatableProdotti(ByRef dtProdotti As DataTable,
                                        ByRef l As List(Of ColonneNome),
                                        ByVal piva As String,
                                        ByVal Elem_Cod As Integer,
                                        ByVal Filtro_Valorizzati As Integer,
                                        ByVal objParametri_Server As AgronicaCoreParametri)

        dtProdotti.Columns.Add(New DataColumn("chiave", GetType(String)))

        dtProdotti.Columns.Add(New DataColumn("visibilita", GetType(String)))
        dtProdotti.Columns.Add(New DataColumn("Reg_Des", GetType(String)))
        dtProdotti.Columns.Add(New DataColumn("GRVA_COD_VEG_DES", GetType(String)))
        dtProdotti.Columns.Add(New DataColumn("Sem_Des", GetType(String)))
        dtProdotti.Columns.Add(New DataColumn("Cat_Des", GetType(String)))
        dtProdotti.Columns.Add(New DataColumn("EAN", GetType(String)))
        dtProdotti.Columns.Add(New DataColumn("Barcode", GetType(String)))
        dtProdotti.Columns.Add(New DataColumn("Produzione_Propria", GetType(Integer)))
        dtProdotti.Columns.Add(New DataColumn("Produzione_Propria_Des", GetType(String)))
        dtProdotti.Columns.Add(New DataColumn("Peso_Netto", GetType(Decimal)))
        dtProdotti.Columns.Add(New DataColumn("Peso_Sgocciolato", GetType(Decimal)))
        dtProdotti.Columns.Add(New DataColumn("Tara", GetType(Decimal)))
        dtProdotti.Columns.Add(New DataColumn("Peso_Egalizzato", GetType(Integer)))
        dtProdotti.Columns.Add(New DataColumn("Peso_Egalizzato_Des", GetType(String)))
        dtProdotti.Columns.Add(New DataColumn("Cod_Iva", GetType(Integer)))
        dtProdotti.Columns.Add(New DataColumn("Cod_Iva_Des", GetType(String)))
        dtProdotti.Columns.Add(New DataColumn("Immagine_Des", GetType(String)))
        If dtProdotti.Columns.Contains("Cod_TecnologiaSementi") Then
            dtProdotti.Columns.Add(New DataColumn("Cod_TecnologiaSementi_Des", GetType(String)))
        End If
        If dtProdotti.Columns.Contains("Germinabilita") Then
            dtProdotti.Columns.Add(New DataColumn("Germinabilita_Des", GetType(String)))
        End If


        'Leggo i Gruppi Varietali
        Dim obj As New AgronicaCoreMetaSchemaDAL.GruppoVarietale_R
        Dim dt_GruppoVarietale = obj.LeggiTabella(0,
                                                  "", "", objParametri_Server)



        'Leggo le Tipologie di Sementi
        Dim objTipoSem As New AgronicaCoreMetaSchemaDAL.TipologieSementi_R
        Dim dt_TipologieSementi = objTipoSem.Leggi(0, "", "", "",
                                                   objParametri_Server)

        Dim objLinea_Classi_Produzioni As New AgronicaCoreContabDAL.Linee_Classi_Produzioni_R
        Dim dt_Linea_Classi_Produzioni = objLinea_Classi_Produzioni.Leggi(piva,
                                                                            0, "", "", objParametri_Server)


        'Leggo la Prodotti_Extra_Privata
        Dim objProdotti_Extra As New Prodotti_Extra_Privata_R
        Dim dt_Prodotti_Extra As DataTable = objProdotti_Extra.Leggi(piva, 0, Elem_Cod, 0,
                                                                    Nothing, Nothing, Nothing,
                                                                    Nothing, Nothing, Nothing,
                                                                    "", "", "", Nothing, Nothing,
                                                                    Nothing, Nothing, Nothing,
                                                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                    "", "", objParametri_Server)

        'Leggo le Descrizioni dell'IVA Aliquota
        Dim objIva_Aliquote As New AgronicaCoreMetaSchemaDAL.IVA_Aliquote_R
        Dim dtIva_Aliquote = objIva_Aliquote.Leggi(0, -1, -1, -1, NATURA_ESCLUSIONE_NOFILTRO,
                                                   "", "", objParametri_Server)

        Dim objTecnologieSementi As New AgronicaCoreMetaSchemaDAL.TecnologieSementi
        Dim dictTecnologieSementi = objTecnologieSementi.Leggi(objParametri_Server).ToExpandoObject.ToDictionary(Of Integer, String)(Function(row) row("Cod_TecnologiaSementi"), Function(row) row("Descrizione"))


        For Each row In dtProdotti.Rows
            row("chiave") = row("Mat_Cod_Omni")

            If CInt(row("sa_cod").ToString()) = -1 Then
                row("visibilita") = Gias.Pubblico
            Else
                row("visibilita") = Gias.Privato
            End If


            'Ottengo il Reg_Des
            Dim Reg_Cod = CInt(row("Reg_Cod").ToString())
            row("Reg_Des") = Gias.RegNessuno


            If Reg_Cod = enum_Cod_Regolamento.Regolamento_bio Then
                row("Reg_Des") = Gias.RegCE834_07
            End If


            'Ottengo il Grva_Des
            row("GRVA_COD_VEG_DES") = ""
            If Not dt_GruppoVarietale Is Nothing Then

                Dim GRVA_COD_VEG = CInt(row("GRVA_COD_VEG").ToString())

                If GRVA_COD_VEG < 0 Then
                    GRVA_COD_VEG = Math.Abs(GRVA_COD_VEG)
                End If

                Dim TrovatoGruppoVarietale = dt_GruppoVarietale.Select("Grva_Cod = " & GRVA_COD_VEG)

                If Not TrovatoGruppoVarietale Is Nothing AndAlso TrovatoGruppoVarietale.Count = 1 Then

                    If CInt(row("GRVA_COD_VEG").ToString()) < 0 Then
                        row("GRVA_COD_VEG_DES") = CStr(TrovatoGruppoVarietale(0)("Grva_Des") & " -- " & Gias.Ibrido)
                    Else
                        row("GRVA_COD_VEG_DES") = CStr(TrovatoGruppoVarietale(0)("Grva_Des"))
                    End If

                End If

            End If

            'Ottengo il Sem_Des
            row("Sem_Des") = ""
            If Not dt_TipologieSementi Is Nothing Then

                Dim TrovatoTipologiaSemente = dt_TipologieSementi.Select("Sem_Cod = " & CInt(row("Sem_Cod").ToString()))

                If Not TrovatoTipologiaSemente Is Nothing AndAlso TrovatoTipologiaSemente.Count = 1 Then

                    row("Sem_Des") = CStr(TrovatoTipologiaSemente(0)("Sem_Des"))

                End If

            End If

            'Ottengo il Cat_Des
            row("Cat_Des") = ""
            If Not dt_Linea_Classi_Produzioni Is Nothing Then

                Dim TrovatoLinea_Classi_Produzioni = dt_Linea_Classi_Produzioni.Select("Linea_Classe_Cod = " & CInt(row("Cat_Cod").ToString()))

                If Not TrovatoLinea_Classi_Produzioni Is Nothing AndAlso TrovatoLinea_Classi_Produzioni.Count = 1 Then

                    row("Cat_Des") = CStr(TrovatoLinea_Classi_Produzioni(0)("Linea_Classe_Des"))

                End If

            End If


            'Ottengo le varie descrizioni dei campi della Prodotto_Extra_Privata
            row("EAN") = ""
            row("Barcode") = ""
            row("Produzione_Propria_Des") = Gias.ProdottoCommercializzato
            row("Peso_Netto") = 0
            row("Peso_Sgocciolato") = 0
            row("Tara") = 0
            row("Peso_Egalizzato_Des") = ""
            row("Cod_Iva") = -1
            row("Cod_Iva_Des") = ""
            row("Immagine_Des") = Gias.No

            Dim Mat_Cod = 0
            Dim Pro_Cod = 0


            If CInt(row("Prodotto_Cod").ToString()) < 0 Then
                Mat_Cod = CInt(row("Prodotto_Cod").ToString()) * -1
            Else
                Pro_Cod = CInt(row("Prodotto_Cod").ToString())
            End If

            Dim TrovatoProdotto_Extra_Privata = dt_Prodotti_Extra.Select("Mat_Cod = " & Mat_Cod & " And Pro_Cod = " & Pro_Cod)


            If Not TrovatoProdotto_Extra_Privata Is Nothing AndAlso TrovatoProdotto_Extra_Privata.Count = 1 Then

                row("EAN") = TrovatoProdotto_Extra_Privata(0)("EAN")
                row("Barcode") = TrovatoProdotto_Extra_Privata(0)("Barcode")
                row("Produzione_Propria") = TrovatoProdotto_Extra_Privata(0)("Produzione_Propria")

                If IsDBNull(TrovatoProdotto_Extra_Privata(0)("Produzione_Propria")) OrElse
                    CInt(TrovatoProdotto_Extra_Privata(0)("Produzione_Propria").ToString()) = enum_Tipo_Produzione.Prodotto_Commercializzato Then
                    row("Produzione_Propria_Des") = Gias.ProdottoCommercializzato
                ElseIf CInt(TrovatoProdotto_Extra_Privata(0)("Produzione_Propria").ToString()) = enum_Tipo_Produzione.Produzione_Propria Then
                    row("Produzione_Propria_Des") = Gias.ProduzionePropria
                End If


                row("Peso_Netto") = TrovatoProdotto_Extra_Privata(0)("Peso_Netto")
                row("Peso_Sgocciolato") = TrovatoProdotto_Extra_Privata(0)("Peso_Sgocciolato")
                row("Tara") = TrovatoProdotto_Extra_Privata(0)("Tara")
                row("Peso_Egalizzato") = TrovatoProdotto_Extra_Privata(0)("Peso_Egalizzato")

                If IsDBNull(TrovatoProdotto_Extra_Privata(0)("Peso_Egalizzato")) OrElse
                    CInt(TrovatoProdotto_Extra_Privata(0)("Peso_Egalizzato").ToString()) = -1 Then
                    row("Peso_Egalizzato_Des") = ""
                ElseIf CInt(TrovatoProdotto_Extra_Privata(0)("Peso_Egalizzato").ToString()) = 0 Then
                    row("Peso_Egalizzato_Des") = Gias.No
                ElseIf CInt(TrovatoProdotto_Extra_Privata(0)("Peso_Egalizzato").ToString()) = 1 Then
                    row("Peso_Egalizzato_Des") = Gias.Si
                End If


                If IsDBNull(TrovatoProdotto_Extra_Privata(0)("Immagine")) OrElse
                   IsDBNull(TrovatoProdotto_Extra_Privata(0)("Immagine_NomeFile")) OrElse
                   IsDBNull(TrovatoProdotto_Extra_Privata(0)("Immagine_Estensione")) Then
                    row("Immagine_Des") = Gias.No
                Else

                    If String.IsNullOrEmpty(TrovatoProdotto_Extra_Privata(0)("Immagine").ToString()) OrElse
                       String.IsNullOrEmpty(TrovatoProdotto_Extra_Privata(0)("Immagine_NomeFile").ToString()) OrElse
                       String.IsNullOrEmpty(TrovatoProdotto_Extra_Privata(0)("Immagine_Estensione").ToString()) Then
                        row("Immagine_Des") = Gias.No
                    Else
                        row("Immagine_Des") = Gias.Si
                    End If

                End If


                If IsDBNull(TrovatoProdotto_Extra_Privata(0)("Cod_Iva")) Then
                    row("Cod_Iva_Des") = ""
                Else
                    If Not dtIva_Aliquote Is Nothing AndAlso dtIva_Aliquote.Rows.Count > 0 Then

                        Dim Descrizine_trovata = dtIva_Aliquote.Select("Codice = " & CInt(TrovatoProdotto_Extra_Privata(0)("Cod_Iva").ToString()))
                        If Not Descrizine_trovata Is Nothing AndAlso Descrizine_trovata.Count = 1 Then
                            row("Cod_Iva_Des") = Descrizine_trovata(0)("Sigla")
                        End If

                    Else
                        row("Cod_Iva_Des") = ""
                    End If
                End If

            End If

            If dtProdotti.Columns.Contains("Cod_TecnologiaSementi") Then
                If dictTecnologieSementi.ContainsKey(row("Cod_TecnologiaSementi")) Then
                    row("Cod_TecnologiaSementi_Des") = dictTecnologieSementi(row("Cod_TecnologiaSementi"))
                Else
                    row("Cod_TecnologiaSementi_Des") = ""
                End If
            End If
            If dtProdotti.Columns.Contains("Germinabilita") Then
                If IsNumeric(row("Germinabilita")) Then
                    row("Germinabilita_Des") = (CDbl(row("Germinabilita")) * 100) & "%"
                Else
                    row("Germinabilita_Des") = ""
                End If
            End If

        Next

        'Se diverso da Tutti(-1) vado a leggere la Prodotti_Costi
        If Filtro_Valorizzati <> -1 Then


            Dim dtProdottiFiltro_Valorizzati As DataTable = dtProdotti.Clone()

            Dim objProdotti_Costi As New AgronicaCoreContabDAL.Prodotti_Costi_R


            For Each row In dtProdotti.Rows

                Dim FiltroperPiva As String = "Prodotti_Costi.piva = '" & piva & "'"


                Dim pro_cod As Integer = 0
                Dim mat_cod As Integer = 0
                Dim prodotto_cod As Integer = CInt(row("Prodotto_Cod").ToString())

                If prodotto_cod < 0 Then
                    mat_cod = prodotto_cod * -1
                Else
                    pro_cod = prodotto_cod
                End If


                Dim Elem_Cod_Row = Elem_Cod

                'Se non è stata selezionata nessuna categoria Prodotto allora prendo quella della colonna Elem_Cod del dataTable Prodotti
                If Elem_Cod_Row = 0 Then
                    Elem_Cod_Row = CInt(row("Elem_Cod").ToString())
                End If

                Dim dt = objProdotti_Costi.Leggi(piva,
                                                   Elem_Cod_Row,
                                                   "",
                                                   pro_cod,
                                                   mat_cod,
                                                   0,
                                                   0,
                                                   0,
                                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                   FiltroperPiva,
                                                   "", objParametri_Server)


                If Filtro_Valorizzati = 0 AndAlso dt.Rows.Count = 0 Then

                    dtProdottiFiltro_Valorizzati.ImportRow(row)

                ElseIf Filtro_Valorizzati = 1 AndAlso dt.Rows.Count > 0 Then

                    dtProdottiFiltro_Valorizzati.ImportRow(row)

                End If

            Next

            dtProdotti.Clear()

            dtProdotti = dtProdottiFiltro_Valorizzati

        End If


        l.Add(New ColonneNome("chiave", "chiave", "string") With {._hidden = True})
        l.Add(New ColonneNome("Piva", "Piva", "string") With {._hidden = True})

        l.Add(New ColonneNome("Elem_Cod", "CatgoriaMagazzino", "string") With {._hidden = True})
        l.Add(New ColonneNome("NomeComune", "Categoria Magazzino", "string") With {._hidden = False, ._FiltrabileConCheck = True})

        l.Add(New ColonneNome("Cod_Articolo", Gias.CodArticolo, "string") With {._hidden = False, ._FiltrabileConCheck = True})

        l.Add(New ColonneNome("Prodotto_Cod", "Codice", "string") With {._hidden = True})
        l.Add(New ColonneNome("Prodotto_Des", Gias.Descrizione, "string") With {._hidden = False, ._FiltrabileConCheck = True})

        l.Add(New ColonneNome("veg_cod", "veg_cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("Veg_Des", Gias.Specie, "string") With {._hidden = False, ._FiltrabileConCheck = True})

        l.Add(New ColonneNome("Cul_cod", "cul_cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("Cul_Des", Gias.Varieta, "string") With {._hidden = False, ._FiltrabileConCheck = True})

        l.Add(New ColonneNome("GRVA_COD_VEG", "GRVA_COD_VEG", "number") With {._hidden = True})
        l.Add(New ColonneNome("GRVA_COD_VEG_DES", Gias.TipologiaVarietale, "string") With {._hidden = False, ._FiltrabileConCheck = True})

        l.Add(New ColonneNome("Sem_Cod", "Sem_Cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("Sem_Des", Gias.TipologiaSemente, "string") With {._hidden = False, ._FiltrabileConCheck = True})

        l.Add(New ColonneNome("Reg_Cod", "Reg_Cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("Reg_Des", Gias.Regolamento, "string") With {._hidden = False, ._FiltrabileConCheck = True})

        l.Add(New ColonneNome("sa_cod", "sa_cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("visibilita", Gias.Visibilità, "string") With {._hidden = False, ._FiltrabileConCheck = True})

        l.Add(New ColonneNome("Cat_Cod", "Cat_Cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("Cat_Des", Gias.CategoriaCommerciale, "string") With {._hidden = False, ._FiltrabileConCheck = True})

        l.Add(New ColonneNome("Rag_Soc_Proprietaria", Gias.AziendaCreazione, "string") With {._hidden = False, ._FiltrabileConCheck = True})

        l.Add(New ColonneNome("Data_Creazione", Gias.DataCreazione, "date") With {._hidden = False, ._FiltrabileConCheck = False, ._formatNr = "{0:dd/MM/yyyy}"})

        l.Add(New ColonneNome("Data_Modifica", Gias.DataModifica, "date") With {._hidden = False, ._FiltrabileConCheck = False, ._formatNr = "{0:dd/MM/yyyy}"})
        l.Add(New ColonneNome("Utente_Creazione", Gias.UtenteCreazione, "string") With {._hidden = False, ._FiltrabileConCheck = True})

        l.Add(New ColonneNome("Utente_Modifica", Gias.UtenteModifica, "string") With {._hidden = False, ._FiltrabileConCheck = True})

        l.Add(New ColonneNome("EAN", "EAN/GTIN", "string") With {._hidden = False, ._Display = False, ._FiltrabileConCheck = True})

        l.Add(New ColonneNome("Barcode", "Barcode Interno", "string") With {._hidden = False, ._Display = False, ._FiltrabileConCheck = True})

        l.Add(New ColonneNome("Produzione_Propria", "Produzione_Propria", "number") With {._hidden = True})
        l.Add(New ColonneNome("Produzione_Propria_Des", Gias.ProduzionePropria, "string") With {._hidden = False, ._Display = False, ._FiltrabileConCheck = True})

        l.Add(New ColonneNome("Peso_Netto", Gias.PesoNetto, "number") With {._hidden = False, ._Display = False, ._FiltrabileConCheck = False, ._formatNr = "n6"})

        l.Add(New ColonneNome("Peso_Sgocciolato", Gias.PesoSgocciolato, "number") With {._hidden = False, ._Display = False, ._FiltrabileConCheck = False, ._formatNr = "n6"})

        l.Add(New ColonneNome("Tara", Gias.Tara, "number") With {._hidden = False, ._Display = False, ._FiltrabileConCheck = False, ._formatNr = "n6"})

        l.Add(New ColonneNome("Peso_Egalizzato", "Peso_Egalizzato", "number") With {._hidden = True})
        l.Add(New ColonneNome("Peso_Egalizzato_Des", Gias.PesoEgalizzato, "string") With {._hidden = False, ._Display = False, ._FiltrabileConCheck = True})

        l.Add(New ColonneNome("Immagine_Des", Gias.Immagine, "string") With {._hidden = False, ._Display = False, ._FiltrabileConCheck = True})

        l.Add(New ColonneNome("Cod_Iva", "Cod_Iva", "number") With {._hidden = True})
        l.Add(New ColonneNome("Cod_Iva_Des", Gias.AliquotaIva, "string") With {._hidden = False, ._FiltrabileConCheck = True})

        l.Add(New ColonneNome("Codice_Esterno", Gias.CodiceEsterno, "string") With {._hidden = False, ._FiltrabileConCheck = True})

        l.Add(New ColonneNome("ChkAlias", "ChkAlias", "number") With {._hidden = True})

        If dtProdotti.Columns.Contains("Des_Gruppo_Merce") Then
            l.Add(New ColonneNome("Des_Gruppo_Merce", "Gruppo Merce", "string") With {._hidden = False, ._FiltrabileConCheck = True})
        End If

        If dtProdotti.Columns.Contains("Cod_TecnologiaSementi") Then
            l.Add(New ColonneNome("Cod_TecnologiaSementi_Des", Gias.TecnologiaSementi, "string") With {._hidden = False, ._Display = False, ._FiltrabileConCheck = True})
        End If
        If dtProdotti.Columns.Contains("Germinabilita") Then
            l.Add(New ColonneNome("Germinabilita_Des", Gias.Germinabilita, "string") With {._hidden = False, ._Display = False, ._FiltrabileConCheck = True})
        End If
    End Sub

#End Region

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Formulati_QdC(InData As Object) As rispostaStandard(Of List(Of DettaglioTrattamento))

        Dim r As New rispostaStandard(Of List(Of DettaglioTrattamento))

        Try
            Dim datiRequest As String = JsonConvert.SerializeObject(InData)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local
            serializerSettings.NullValueHandling = NullValueHandling.Ignore
            serializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of InData.Anagrafica.LeggiProdotti))(datiRequest, serializerSettings)

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objParametri_Formulati As InData.Anagrafica.LeggiProdotti = objRequest.InData

            If objParametri_Server Is Nothing Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            If objParametri_Utenti Is Nothing Then
                r.Errore = "objP_utenti non valorizzato"
                Return r
            End If

            If objParametri_Super_Server Is Nothing Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim UtilityBIZ As New AgronicaControlli_2010.STD_Utility

            Dim formulatiList As New List(Of AgronicaCoreModelsSTD.attivita.dettagli.DettaglioTrattamento)
            Dim FormulatiBiz As New AgronicaControlli_2010.STD_Formulati

            Dim codiceProdotto As Integer = 0

            Dim filtroPerDescrizione As String = objParametri_Formulati.filtroPerDescrizione

            If Not String.IsNullOrEmpty(objParametri_Formulati.filtroPerDescrizione) AndAlso IsNumeric(objParametri_Formulati.filtroPerDescrizione) Then
                codiceProdotto = CInt(objParametri_Formulati.filtroPerDescrizione)
                filtroPerDescrizione = ""
            End If

            formulatiList = FormulatiBiz.LeggiFormulati(objParametri_Formulati.tipoAttivita,
                                                        objParametri_Formulati.statoAttivita,
                                                        objParametri_Formulati.lavorazione,
                                                        objParametri_Formulati.impianti,
                                                        objParametri_Formulati.prodottiDaTrattare,
                                                        objParametri_Formulati.specie,
                                                        objParametri_Formulati.disciplinare,
                                                        objParametri_Formulati.epocaDPI,
                                                        objParametri_Formulati.avversitaGruppo,
                                                        filtroPerDescrizione,
                                                        objParametri_Formulati.data,
                                                        objParametri_Formulati.escludiGiacenzeZero,
                                                        objParametri_Formulati.magazziniAgenzie,
                                                        objParametri_Formulati.magazziniEsterni,
                                                        codiceProdotto:=codiceProdotto,
                                                        soloLetturaAnagrafica:=False,
                                                        objParametri_Super_Server,
                                                        objParametri_Server,
                                                        objParametri_Utenti)

            r.RispostaStringa = formulatiList
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Fertilizzanti_QdC(InData As Object) As rispostaStandard(Of List(Of DettaglioFertilizzazione))

        Dim r As New rispostaStandard(Of List(Of DettaglioFertilizzazione))

        Try
            Dim datiRequest As String = JsonConvert.SerializeObject(InData)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local
            serializerSettings.NullValueHandling = NullValueHandling.Ignore
            serializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of InData.Anagrafica.LeggiProdotti))(datiRequest, serializerSettings)

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objParametri_Fertilizzanti As InData.Anagrafica.LeggiProdotti = objRequest.InData

            If objParametri_Server Is Nothing Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            If objParametri_Utenti Is Nothing Then
                r.Errore = "objP_utenti non valorizzato"
                Return r
            End If

            If objParametri_Super_Server Is Nothing Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim fertilizzantiList As New List(Of AgronicaCoreModelsSTD.attivita.dettagli.DettaglioFertilizzazione)
            Dim FertilizzantiBiz As New AgronicaControlli_2010.STD_Fertilizzanti
            fertilizzantiList = FertilizzantiBiz.LeggiFertilizzanti(objParametri_Fertilizzanti.tipoAttivita,
                                                                    objParametri_Fertilizzanti.statoAttivita,
                                                                    objParametri_Fertilizzanti.lavorazione,
                                                                    objParametri_Fertilizzanti.impianti,
                                                                    objParametri_Fertilizzanti.disciplinare,
                                                                    objParametri_Fertilizzanti.filtroPerDescrizione,
                                                                    objParametri_Fertilizzanti.data,
                                                                    objParametri_Fertilizzanti.escludiGiacenzeZero,
                                                                    objParametri_Fertilizzanti.magazziniAgenzie,
                                                                    objParametri_Fertilizzanti.magazziniEsterni,
                                                                    tipoRicetta:=0, 'enum_TipoRicetta TODO_DT sviluppare con le ricette
                                                                    pua:=objParametri_Fertilizzanti.pua, 'TODO_DT controllare tutto giro da PUA
                                                                    codiceFertilizzante:=0,
                                                                    soloLetturaAnagrafica:=False,
                                                                    objParametri_Super_Server,
                                                                    objParametri_Server,
                                                                    objParametri_Utenti)

            r.RispostaStringa = fertilizzantiList
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Sementi_QdC(InData As Object) As rispostaStandard(Of List(Of DettaglioSemina))

        Dim r As New rispostaStandard(Of List(Of DettaglioSemina))

        Try
            Dim datiRequest As String = JsonConvert.SerializeObject(InData)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local
            serializerSettings.NullValueHandling = NullValueHandling.Ignore
            serializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of InData.Anagrafica.LeggiProdotti))(datiRequest, serializerSettings)

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objParametri_Sementi As InData.Anagrafica.LeggiProdotti = objRequest.InData

            If objParametri_Server Is Nothing Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            If objParametri_Utenti Is Nothing Then
                r.Errore = "objP_utenti non valorizzato"
                Return r
            End If

            If objParametri_Super_Server Is Nothing Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim sementiList As New List(Of AgronicaCoreModelsSTD.attivita.dettagli.DettaglioSemina)
            Dim SementiBiz As New AgronicaControlli_2010.STD_Sementi

            sementiList = SementiBiz.LeggiSementi(objParametri_Sementi.tipoAttivita,
                                                  objParametri_Sementi.statoAttivita,
                                                  objParametri_Sementi.lavorazione,
                                                  objParametri_Sementi.impianti,
                                                  objParametri_Sementi.filtroPerDescrizione,
                                                  objParametri_Sementi.data,
                                                  objParametri_Sementi.escludiGiacenzeZero,
                                                  objParametri_Sementi.magazziniAgenzie,
                                                  objParametri_Sementi.magazziniEsterni,
                                                  objParametri_Super_Server,
                                                  objParametri_Server,
                                                  objParametri_Utenti)

            r.RispostaStringa = sementiList
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_TrasformatiVegetali_QdC(InData As Object) As rispostaStandard(Of List(Of DettaglioRaccolta))

        Dim r As New rispostaStandard(Of List(Of DettaglioRaccolta))

        Try
            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of InData.Anagrafica.LeggiProdotti))(datiRequest)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objParametri_TrasformatiVegetali As InData.Anagrafica.LeggiProdotti = objRequest.InData

            If objParametri_Server Is Nothing Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            If objParametri_Utenti Is Nothing Then
                r.Errore = "objP_utenti non valorizzato"
                Return r
            End If

            If objParametri_Super_Server Is Nothing Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            Dim trasformatiVegetaliList As New List(Of AgronicaCoreModelsSTD.attivita.dettagli.DettaglioRaccolta)
            Dim trasformatiVegetaliBiz As New AgronicaControlli_2010.STD_TrasformatiVegetali
            trasformatiVegetaliList = trasformatiVegetaliBiz.LeggiTrasformatiVegetali_Qdc(objParametri_TrasformatiVegetali.impianti,
                                                                                          objParametri_TrasformatiVegetali.specie,
                                                                                          objParametri_Super_Server,
                                                                                          objParametri_Server,
                                                                                          objParametri_Utenti)

            r.RispostaStringa = trasformatiVegetaliList
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_InsettiUtilili_QdC(InData As Object) As rispostaStandard(Of List(Of DettaglioTrattamento))

        Dim r As New rispostaStandard(Of List(Of DettaglioTrattamento))

        Try
            Dim datiRequest As String = JsonConvert.SerializeObject(InData)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local
            serializerSettings.NullValueHandling = NullValueHandling.Ignore
            serializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of InData.Anagrafica.LeggiProdotti))(datiRequest, serializerSettings)

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objParametri_InsettiUtili As InData.Anagrafica.LeggiProdotti = objRequest.InData

            If objParametri_Server Is Nothing Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            If objParametri_Utenti Is Nothing Then
                r.Errore = "objP_utenti non valorizzato"
                Return r
            End If

            If objParametri_Super_Server Is Nothing Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim insettiUtiliList As New List(Of AgronicaCoreModelsSTD.attivita.dettagli.DettaglioTrattamento)
            Dim InsettiUtiliBiz As New AgronicaControlli_2010.STD_InsettiUtili
            insettiUtiliList = InsettiUtiliBiz.LeggiInsettiUtili(objParametri_InsettiUtili.tipoAttivita,
                                                                 objParametri_InsettiUtili.statoAttivita,
                                                                 objParametri_InsettiUtili.lavorazione,
                                                                 objParametri_InsettiUtili.impianti,
                                                                 objParametri_InsettiUtili.avversitaGruppo,
                                                                 objParametri_InsettiUtili.filtroPerDescrizione,
                                                                 objParametri_InsettiUtili.data,
                                                                 objParametri_InsettiUtili.escludiGiacenzeZero,
                                                                 codiceInsettoUtile:=0,
                                                                 objParametri_InsettiUtili.magazziniAgenzie,
                                                                 objParametri_InsettiUtili.magazziniEsterni,
                                                                 objParametri_Super_Server,
                                                                 objParametri_Server,
                                                                 objParametri_Utenti)

            r.RispostaStringa = insettiUtiliList
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_ProdottiDaTrattare_QdC(InData As CoreWS_Generic(Of InData.Anagrafica.LeggiProdotti)) As rispostaStandard(Of List(Of MovimentoDiMagazzino))

        Dim r As New rispostaStandard(Of List(Of MovimentoDiMagazzino))

        If InData.objP.objP_super_server Is Nothing Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_server Is Nothing Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti Is Nothing Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim LeggiProdotti As InData.Anagrafica.LeggiProdotti = InData.InData

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim lavorazione As Integer = LeggiProdotti.lavorazione.primaryKey.codice
            Dim tipo_attivita As AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Attivita = LeggiProdotti.tipoAttivita

            Dim movimentiMagazzinoList As New List(Of MovimentoDiMagazzino)
            Dim elem_cod As Integer = 0

            Dim objContabBIZ As New AgronicaCoreContabBIZ.Giacenze_R


            Select Case lavorazione
                Case LAVCOD_TRATTAMENTO_POST_RACCOLTA
                    elem_cod = TRASFORMATI_VEGETALI
                Case LAVCOD_CONCIA_SEME
                    elem_cod = SEMENTI
            End Select

            movimentiMagazzinoList = objContabBIZ.LeggiProdottiDaTrattare_QdC(LeggiProdotti.impresa.partitaIva,
                                                                              LeggiProdotti.centroAziendale.primaryKey.codice,
                                                                              LeggiProdotti.specie,
                                                                              LeggiProdotti.data,
                                                                              tipo_attivita,
                                                                              elem_cod,
                                                                              objParametri_Super_Server,
                                                                              objParametri_Server,
                                                                              objParametri_Utenti)

            r.RispostaStringa = movimentiMagazzinoList
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

#Region "OBOSLETE -- CANCELLARE?"
    '<WebMethod()>
    '<Script.Services.ScriptMethod()>
    'Public Function Leggi_Trappole_QdC(InData As Object) As rispostaStandard(Of List(Of DettaglioTrattamento))

    '    Dim r As New rispostaStandard(Of List(Of DettaglioTrattamento))

    '    Try
    '        Dim datiRequest As String = JsonConvert.SerializeObject(InData)

    '        Dim serializerSettings As New JsonSerializerSettings()
    '        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    '        serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local
    '        serializerSettings.NullValueHandling = NullValueHandling.Ignore
    '        serializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore
    '        Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of InData.Anagrafica.LeggiProdotti))(datiRequest, serializerSettings)

    '        Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
    '        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
    '        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
    '        Dim objParametri_InsettiUtili As InData.Anagrafica.LeggiProdotti = objRequest.InData

    '        If objParametri_Server Is Nothing Then
    '            r.Errore = "objP_server non valorizzato"
    '            Return r
    '        End If

    '        If objParametri_Utenti Is Nothing Then
    '            r.Errore = "objP_utenti non valorizzato"
    '            Return r
    '        End If

    '        If objParametri_Super_Server Is Nothing Then
    '            r.Errore = "objP_super_server non valorizzato"
    '            Return r
    '        End If

    '        Dim leggiLingua As New Lingue_Read
    '        Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
    '        Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
    '        Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

    '        Dim trappoleList As New List(Of AgronicaCoreModelsSTD.attivita.dettagli.DettaglioTrattamento)
    '        Dim TrappoleBiz As New AgronicaControlli_2010.STD_InsettiUtili
    '        trappoleList = TrappoleBiz.LeggiTrappole(objParametri_InsettiUtili.tipoAttivita,
    '                                                 objParametri_InsettiUtili.statoAttivita,
    '                                                 objParametri_InsettiUtili.lavorazione,
    '                                                 objParametri_InsettiUtili.impianti,
    '                                                 objParametri_InsettiUtili.specie,
    '                                                 objParametri_InsettiUtili.avversitaGruppo,
    '                                                 objParametri_InsettiUtili.filtroPerDescrizione,
    '                                                 objParametri_InsettiUtili.data,
    '                                                 objParametri_InsettiUtili.escludiGiacenzeZero,
    '                                                 codiceTrappola:=0,
    '                                                 objParametri_Super_Server,
    '                                                 objParametri_Server,
    '                                                 objParametri_Utenti)

    '        r.RispostaStringa = trappoleList
    '        r.RispostaOK = True

    '    Catch ex As Exception
    '        r.RispostaOK = False
    '        r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}
    '        r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
    '    End Try

    '    Return r

    'End Function

    '<WebMethod()>
    '<Script.Services.ScriptMethod()>
    'Public Function Leggi_DitteTrappola_QdC(InData As Object) As rispostaStandard(Of List(Of metaschema.Ditta))

    '    Dim r As New rispostaStandard(Of List(Of metaschema.Ditta))

    '    Try
    '        Dim datiRequest As String = JsonConvert.SerializeObject(InData)

    '        Dim serializerSettings As New JsonSerializerSettings()
    '        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    '        serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local
    '        serializerSettings.NullValueHandling = NullValueHandling.Ignore
    '        serializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore
    '        Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of InData.Agenda.LeggiDitteTrappole))(datiRequest, serializerSettings)

    '        Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
    '        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
    '        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
    '        Dim objParametri_DitteTrappola As InData.Agenda.LeggiDitteTrappole = objRequest.InData

    '        If objParametri_Server Is Nothing Then
    '            r.Errore = "objP_server non valorizzato"
    '            Return r
    '        End If

    '        If objParametri_Utenti Is Nothing Then
    '            r.Errore = "objP_utenti non valorizzato"
    '            Return r
    '        End If

    '        If objParametri_Super_Server Is Nothing Then
    '            r.Errore = "objP_super_server non valorizzato"
    '            Return r
    '        End If

    '        Dim leggiLingua As New Lingue_Read
    '        Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
    '        Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
    '        Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

    '        Dim ditteTrappolaList As New List(Of metaschema.Ditta)
    '        Dim TrappoleBiz As New AgronicaControlli_2010.STD_InsettiUtili
    '        ditteTrappolaList = TrappoleBiz.LeggiDitteTrappola(objParametri_DitteTrappola.trappola,
    '                                                           objParametri_Super_Server,
    '                                                           objParametri_Server,
    '                                                           objParametri_Utenti)

    '        r.RispostaStringa = ditteTrappolaList
    '        r.RispostaOK = True

    '    Catch ex As Exception
    '        r.RispostaOK = False
    '        r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}
    '        r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
    '    End Try

    '    Return r

    'End Function
#End Region

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Crea_Prodotti(InData As Object) As RispostaStandard

        Dim r As New RispostaStandard

        Try
            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Anagrafica.CreaProdotti))(datiRequest)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objParametri_data As AgronicaCoreDTOStd.InData.Anagrafica.CreaProdotti = objRequest.InData

            If objParametri_Server Is Nothing Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            If objParametri_Utenti Is Nothing Then
                r.Errore = "objP_utenti non valorizzato"
                Return r
            End If

            If objParametri_Super_Server Is Nothing Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            Dim entrambiRegolamenti = If(Not IsNothing(objParametri_data.regolamenti) AndAlso objParametri_data.regolamenti.codice = enum_Cod_Regolamento.Regolamento_bio, 1, 0)
            Dim objGias As New AgronicaCoreAnagrafeBIZ.Importa_GIAS
            Dim created = objGias.Crea_MateriaPrima_Specie_Varieta_Regolamento(objParametri_data.varieta, entrambiRegolamenti,
                    objParametri_Server, objParametri_Utenti,
                    objParametri_data.creaSemente, objParametri_data.creaTrasformatoVegetale, -1)

            r.RispostaStringa = "{ ""sementiCreati"":" & created.sementiCreati &
                ", ""sementiEsistenti"":" & created.sementiEsistenti &
                ", ""trasformatiEsistenti"":" & created.trasformatiEsistenti &
                ", ""trasformatiCreati"":" & created.trasformatiCreati & " }"
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_TrasformatiVegetali_Anagrafica(InData As Object) As rispostaStandard(Of List(Of Prodotto))

        Dim r As New rispostaStandard(Of List(Of Prodotto))

        Try
            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of InData.Anagrafica.LeggiProdotti))(datiRequest)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objParametri_TrasformatiVegetali As InData.Anagrafica.LeggiProdotti = objRequest.InData

            If objParametri_Server Is Nothing Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            If objParametri_Utenti Is Nothing Then
                r.Errore = "objP_utenti non valorizzato"
                Return r
            End If

            If objParametri_Super_Server Is Nothing Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            Dim trasformatiVegetaliList As New List(Of Prodotto)
            Dim trasformatiVegetaliBiz As New AgronicaControlli_2010.STD_TrasformatiVegetali
            trasformatiVegetaliList = trasformatiVegetaliBiz.LeggiTrasformatiVegetali_Anagrafica(objParametri_TrasformatiVegetali.impresa,
                                                                                      objParametri_TrasformatiVegetali.specie,
                                                                                      objParametri_TrasformatiVegetali.varieta,
                                                                                      objParametri_TrasformatiVegetali.regolamento,
                                                                                      objParametri_TrasformatiVegetali.finalita,
                                                                                      objParametri_Super_Server,
                                                                                      objParametri_Server,
                                                                                      objParametri_Utenti)

            r.RispostaStringa = trasformatiVegetaliList
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Giacenze_Farmaci(InData As Object) As rispostaStandard(Of List(Of DettaglioRegistroSomministrazioni))

        Dim r As New rispostaStandard(Of List(Of DettaglioRegistroSomministrazioni))

        Dim datiRequest As String = JsonConvert.SerializeObject(InData)

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local
        serializerSettings.NullValueHandling = NullValueHandling.Ignore
        serializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore
        Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of InData.Zoo.LeggiGiacenzaFarmaci))(datiRequest, serializerSettings)

        Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
        Dim paramsFarmaci As InData.Zoo.LeggiGiacenzaFarmaci = objRequest.InData

        If objParametri_Server Is Nothing Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objParametri_Utenti Is Nothing Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If objParametri_Super_Server Is Nothing Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If

        Try

            Dim FarmaciBIZ As New AgronicaControlli_2010.STD_Farmaci

            Dim farmaciList = FarmaciBIZ.LeggiFarmaci(paramsFarmaci.Piva, paramsFarmaci.SaCod, paramsFarmaci.codiceBDN, paramsFarmaci.codFiscaleProprietario, paramsFarmaci.codiceAIC, paramsFarmaci.ValiditaFine, AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Attivita.QuadernoDiCampagna, AgronicaCoreModelsSTD.attivita.Attivita.Stati.Da_Eseguire, True, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

            r.RispostaStringa = farmaciList
            r.RispostaOK = True

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False

        End Try

        Return r

    End Function


#Region "COMBO MODIFICA MULTIPLA"
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboCmb_Prodotti(ByVal objP_server As String, piva As String, veg_cod As Integer, cul_cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            'DT: sa_cod ininfluente, le materie prime non sono usate per centro aziendale
            Dim objTrasformatiVegetali As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
            Dim DT As DataTable = objTrasformatiVegetali.Leggi(piva, Sa_Cod:=0, TRASFORMATI_VEGETALI, 0, "",
                                                               veg_cod, cul_cod, 0, 0, 0, 0, 0, "", 0,
                                                               "", False, Flag_MateriePrimeSoloPrivate:=False,
                                                               "", enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametri_Server)


            Dim jArray As New JArray()
            For Each dr As DataRow In DT.Rows
                Dim testo = dr.Item("mat_des") + " " + "(" & Gias.CodArticolo & ": " + dr.Item("Cod_Articolo") + ")"

                jArray.Add(New JObject(New JProperty("text", testo),
                                                   New JProperty("value", dr.Item("mat_cod"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArray, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
#End Region

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class