

Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ

Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports System.Web.Services
Imports AgronicaUMA.Resources
Imports Newtonsoft.Json.Linq

Imports AgronicaCoreContabDAL
Imports Newtonsoft.Json
Imports AgronicaCoreContabBIZ
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreXML.XML_Stampe
Imports System.Web
Imports AgronicaCoreUmaBiz
Imports AgronicaCoreUmaDal

Public Class ElencoRichieste
    Inherits System.Web.UI.Page

    Public QS_Avanzamento As Integer = 0
    Public QS_Piva As String = ""

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public objparametri_server_string, objparametri_utenti_string As String
    Dim objParametriAgenda As ParametriAgenda

    Private Sub inizializzoObjParametri()
        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objparametri_server_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Server)
        '---
    End Sub

#Region "script services Carica Griglia"

    <WebMethod(EnableSession:=True)>
    Public Shared Function Cerca_Dettagli_Azienda(ByVal piva As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim ind_des As String = ""
        Dim com_des As String = ""
        Dim CUAA As String = ""
        Dim pro_cod As String = ""
        Dim comDesIstat As String = ""
        Dim PartitaIvaReale As String = ""
        Dim Dt As New DataTable
        Dim leggiPivaReale As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim leggiIndirizzo As New AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_R
        Dim leggiCUAA As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            If (Not piva.Equals(String.Empty)) Then
                leggiIndirizzo.Indirizzo_from_Piva(piva, "", ind_des, "", "", com_des, "",
                                      pro_cod, comDesIstat, objParametri_Server)

                CUAA = leggiCUAA.Leggi_CUAA(piva, objParametri_Server)

                PartitaIvaReale = leggiPivaReale.Leggi_PivaReale(piva, objParametri_Server)
            End If
            'Dt.Columns.Add(New DataColumn("indDes", GetType(String)))
            Dt.Columns.Add(New DataColumn("comDes", GetType(String)))
            Dt.Columns.Add(New DataColumn("CUAA", GetType(String)))
            Dt.Columns.Add(New DataColumn("pro_cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("comDesIstat", GetType(String)))
            Dt.Columns.Add(New DataColumn("PartitaIvaReale", GetType(String)))

            Dim d0 As DataRow
            d0 = Dt.NewRow
            'd0("indDes") = ind_des
            d0("comDes") = com_des
            d0("CUAA") = CUAA
            d0("pro_cod") = pro_cod
            d0("comDesIstat") = comDesIstat
            d0("PartitaIvaReale") = PartitaIvaReale
            Dt.Rows.Add(d0)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "ErroreDuranteOperazione_" & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function Trova_Richieste(ByVal piva As String,
                                           ByVal statoCod As Integer,
                                           ByVal citta As String,
                                           ByVal prov As String,
                                           ByVal rendicontazioni As Boolean,
                                           ByVal anno As Integer,
                                           ByVal filtroConto As Integer,
                                           ByVal nuovaVisibilita As Boolean) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As New DataTable
        Dim ind_des As String = ""
        Dim com_des As String = ""
        Dim rag_soc As String = ""
        Dim frz_des As String = ""
        Dim CAP As String = ""
        Dim CUAA As String = ""
        Dim leggiIndirizzo As New AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_R
        Dim leggiCUAA As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dt.Columns.Add(New DataColumn("ID", GetType(String)))
        Dt.Columns.Add(New DataColumn("CUAA", GetType(String)))
        Dt.Columns.Add(New DataColumn("piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("azienda", GetType(String)))
        Dt.Columns.Add(New DataColumn("nRichiesta", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("richiestaCod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("citta", GetType(String)))
        Dt.Columns.Add(New DataColumn("CAP", GetType(String)))
        Dt.Columns.Add(New DataColumn("Prov", GetType(String)))
        Dt.Columns.Add(New DataColumn("via", GetType(String)))
        Dt.Columns.Add(New DataColumn("val_Inizio", GetType(DateTime)))
        Dt.Columns.Add(New DataColumn("val_Fine", GetType(DateTime)))
        Dt.Columns.Add(New DataColumn("ultimo_Avanzamento", GetType(DateTime)))
        Dt.Columns.Add(New DataColumn("annoRichiesta", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("CodstatoAv", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("statoAv", GetType(String)))
        Dt.Columns.Add(New DataColumn("calcolato_Gasolio", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("calcolato_Benzina", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("calcolato_Gasolio_Serra", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("richiesto_Gasolio", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("richiesto_Benzina", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("richiesto_Gasolio_Serra", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("assegnato_gasolio", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("assegnato_benzina", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("assegnato_gasolio_serra", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("tipo_richiesta", GetType(String)))
        'Anna 12/08/21: Aggiunte due nuove colonne per UMA
        Dt.Columns.Add(New DataColumn("Richiedente", GetType(String)))
        Dt.Columns.Add(New DataColumn("Approvatore", GetType(String)))
        Dt.Columns.Add(New DataColumn("Tipo_Azienda", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Tipo_Azienda_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("PartitaIvaReale", GetType(String)))

        Dim objTestate As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R
        Dim objProfilo As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
        Dim Filtro_Visibilita_Utente = not nuovaVisibilita andalso Not objProfilo.HasFullVisibility(
            HttpContext.Current.Session("ASG_Utente_Username"), objParametri_Utenti)

        Dim piveVisibili As String = ""
        If piva = "-1" Then
            piva = ""
        End If

        Dim FiltroUtente As Boolean = False
        Dim FiltroGruppo As Boolean = False
        Dim Gruppo As Integer = 0
        Dim VisibilitaTotale As Boolean = False

        If (nuovaVisibilita) Then
            'Dim objUtenti_Visibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            'Dim objGruppi_Utente As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R

            'Dim userName As String = objParametri_Server.UtenteUsername

            'Dim dtt As DataTable = objUtenti_Visibilita.OttieniPIVEVisibilita(userName, objParametri_Server, objParametri_Utenti)

            'If (dtt.Rows.Count > 0) Then
            '    piveVisibili = dtt.AsEnumerable().
            '    [Select](Function(x) x("Piva_Azienda").ToString()).Aggregate(Function(a, b) String.Concat(a & "'" & "," & "'" & b))
            'Else
            '    piveVisibili = "-1"
            'End If

            Dim objUtenti_Visibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            objUtenti_Visibilita.OttieniTipoFiltroVisibilita(objParametri_Utenti, 0, FiltroUtente, FiltroGruppo, Gruppo, VisibilitaTotale)

        End If

        'Dim dtRichiesteUmaTestat = objTestate.Leggi_Elenco_Con_Indirizzi(piva, anno, "", "", objParametri_Server, rendicontazioni:=rendicontazioni, statoCod:=statoCod, citta:=citta, prov:=prov, conto:=conto)
        Dim dtRichiesteUmaTestat = objTestate.Leggi_Elenco_Con_Indirizzi(piva,
                                                                         anno,
                                                                         "",
                                                                         "",
                                                                         objParametri_Server,
                                                                         objParametri_Utenti,
                                                                         Filtro_Visibilita_Utente,
                                                                         piveVisibili,
                                                                         rendicontazioni:=rendicontazioni,
                                                                         statoCod:=statoCod,
                                                                         citta:=citta,
                                                                         prov:=prov,
                                                                         conto:=filtroConto,
                                                                         nuovaVisibilita:=nuovaVisibilita,
                                                                         FiltroUtente:=FiltroUtente,
                                                                         FiltroGruppoUtente:=FiltroGruppo,
                                                                         GruppoUtente:=Gruppo,
                                                                         VisibilitaTotale:=VisibilitaTotale)

        If (rendicontazioni) Then
            For Each row In dtRichiesteUmaTestat.Rows
                'If (row.item("Acquistato_e_Rimanenza_Gasolio") > row.item("Richiesto_Netto_Gasolio") OrElse
                '    row.item("Acquistato_e_Rimanenza_Benzina") > row.item("Richiesto_Netto_Benzina") OrElse
                '    row.item("Acquistato_e_Rimanenza_Gasolio_Serra") > row.item("Richiesto_Netto_Gasolio_Serra")) OrElse
                '   ((row.item("Acquistato_e_Rimanenza_Gasolio") + row.item("Acquistato_e_Rimanenza_Benzina") + row.item("Acquistato_e_Rimanenza_Gasolio_Serra")) >
                '   (row.item("assegnato_gasolio") + row.item("assegnato_benzina") + row.item("assegnato_gasolio_serra"))) Then
                '    row.item("Litri_In_Esubero") = "NO"
                'Else
                '    row.item("Litri_In_Esubero") = "SI"
                'End If
                row.item("Litri_In_Esubero") = OttieniLitriInEsubero(row)
            Next
        End If

        r.RispostaStringa = JsonConvert.SerializeObject(dtRichiesteUmaTestat, Formatting.None, serializerSettings)
        r.RispostaOK = True
        Return r

    End Function

    Private Shared Function OttieniLitriInEsubero(ByRef row As Object) As String

        Dim LitriInEsubero = False

        'Controllo se Acquistato + Rimanenza > Richiesto Netto
        If (row.item("Acquistato_e_Rimanenza_Gasolio") > row.item("Richiesto_Netto_Gasolio") OrElse
            row.item("Acquistato_e_Rimanenza_Benzina") > row.item("Richiesto_Netto_Benzina") OrElse
            row.item("Acquistato_e_Rimanenza_Gasolio_Serra") > row.item("Richiesto_Netto_Gasolio_Serra")) Then
            LitriInEsubero = True
        End If

        'Controllo se Acquistato + Rimanenza > Assegnato
        If Not LitriInEsubero Then
            If (row.item("Acquistato_e_Rimanenza_Gasolio") > Math.Round(CDec(row.item("assegnato_gasolio")), 0) OrElse
                row.item("Acquistato_e_Rimanenza_Benzina") > Math.Round(CDec(row.item("assegnato_benzina")), 0) OrElse
                row.item("Acquistato_e_Rimanenza_Gasolio_Serra") > Math.Round(CDec(row.item("assegnato_gasolio_serra")), 0)) Then
                LitriInEsubero = True
            End If
        End If

        'Controllo se Recupero Accise > 0
        If Not LitriInEsubero Then
            If (row.item("Rec_Acc_Conf_Gasolio") > 0 OrElse
                row.item("Rec_Acc_Conf_Benzina") > 0 OrElse
                row.item("Rec_Acc_Conf_Gasolio_Serra") > 0) Then
                LitriInEsubero = True
            End If
        End If

        Return IIf(LitriInEsubero, "SI", "NO")

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function RiempiStatiPratiche() As RispostaStandard

        Dim r As New RispostaStandard
        Dim pratiche As New AgronicaCoreProfilazioneDAL.Pratiche_R
        Dim Dt As DataTable
        Dim DtRes As New DataTable

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        DtRes.Columns.Add(New DataColumn("cod", GetType(String)))
        DtRes.Columns.Add(New DataColumn("stato", GetType(String)))

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dt = pratiche.Leggi_WAnagraficaStati_Da_Servizio(0, enum_Servizi.Gestione_UMA, objParametri_Server)

            For Each row As DataRow In Dt.Rows

                If CInt(row.Item("Stato_Cod")) <> 2003 AndAlso CInt(row.Item("Stato_Cod")) <> 2008 Then 'STATI NON PIù IN USO

                    Dim d0 As DataRow = DtRes.NewRow()
                    d0.Item("cod") = row.Item("Stato_Cod")
                    d0.Item("stato") = row.Item("WAnagraficaStati_Des")
                    DtRes.Rows.Add(d0)

                End If

            Next

            r.RispostaStringa = JsonConvert.SerializeObject(DtRes, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "ErroreDuranteOperazione_" & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function RiempiCitta(ByVal prov As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim istat As New AgronicaCoreMetaSchemaDAL.Istat_R
        Dim Dt As DataTable
        Dim DtRes As New DataTable

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        DtRes.Columns.Add(New DataColumn("proCom", GetType(String)))
        DtRes.Columns.Add(New DataColumn("citta", GetType(String)))

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dt = istat.Leggi(prov, "", "", "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, " PROV NOT LIKE '%E%' ", " PROV ", objParametri_Server)

            For Each row As DataRow In Dt.Rows

                Dim d0 As DataRow = DtRes.NewRow()
                If (prov IsNot "") Then
                    d0.Item("proCom") = row.Item("COM")
                Else
                    d0.Item("proCom") = row.Item("LOCALITA")
                End If
                d0.Item("citta") = row.Item("LOCALITA")
                DtRes.Rows.Add(d0)

            Next

            r.RispostaStringa = JsonConvert.SerializeObject(DtRes, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "ErroreDuranteOperazione_" & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function RiempiProv() As RispostaStandard

        Dim r As New RispostaStandard
        Dim istat As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R
        Dim Dt As DataTable

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dt = istat.GetProvince(objParametri_Server, "IT")

            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "ErroreDuranteOperazione_" & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Conta_Richieste(ByVal piva As String,
                                           ByVal cuaa As String,
                                           ByVal isTerzista As Boolean,
                                           ByVal anno As Integer,
                                           ByVal Avanzamento_Richiesta As Integer,
                                           ByVal Tipo_Azienda As Integer,
                                           ByVal bocciate As Boolean) As RispostaStandard
        Dim r As New RispostaStandard
        Dim pratica_num As Integer = 1
        Dim Dt As New DataTable

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Try

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim testate As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R

            Dim dt_testate = testate.Leggi(piva, 0, 0,
                                           AGRODATAINIZIO,
                                           AGRODATAFINE,
                                           anno, "",
                                           Avanzamento_Richiesta,
                                           objParametri_Server,
                                           isTerzista:=isTerzista,
                                           " Pratiche_Stati_Attuali.Stato_Cod NOT IN (" & IIf(bocciate, "", enum_WAnagraficaStati.Verifica_Intermedia_Non_Superata & ", ") & enum_WAnagraficaStati.Rinuncia & ") ",
                                           Tipo_Azienda:=Tipo_Azienda)

            r.RispostaStringa = JsonConvert.SerializeObject(dt_testate, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

#End Region

#Region "Edit"

    <WebMethod(EnableSession:=True)>
    Public Shared Function EditRichiesta(ByVal chiave As String,
                                         ByVal richiestaCod As Integer,
                                         ByVal tipo_azienda As String) As RispostaStandard
        Dim objParametriAgenda = New ParametriAgenda

        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaUma
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_UMA_Elenco
        'objParametriAgenda.Piva = chiave
        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica
        Dim risp As New RispostaStandard
        Dim queryString As New With {Key .piva = Stringa_Codifica(chiave, AgroKey_EncoderDecoder, Nothing).ToString,
                                         .richiestaCod = Stringa_Codifica(richiestaCod, AgroKey_EncoderDecoder, Nothing).ToString,
                                         .fromPage = enum_PagineAgenda_2010.Pagina_UMA_Elenco,
                                         .tipo_azienda = tipo_azienda,
                                         .tipoOp = enum_TipoOperazioneDB.Modifica}

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        risp.RispostaOK = True
        risp.RispostaStringa = JsonConvert.SerializeObject(queryString, Formatting.None, serializerSettings)

        Return risp
    End Function

#End Region

#Region "Delete"

    <WebMethod(EnableSession:=True)>
    Public Shared Function DeleteRichiesta(ByVal Piva As String,
                                           ByVal richiestaCod As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Try
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim richieste As New AgronicaCoreUmaBiz.UMA_Richieste

            r.RispostaStringa = richieste.EliminaRichiesta(Piva, richiestaCod, objParametri_Server)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

#End Region

#Region "Rinuncia"

    <WebMethod(EnableSession:=True)>
    Public Shared Function RinunciaRichiesta(ByVal Piva As String,
                                             ByVal Cuaa As String,
                                             ByVal richiestaCod As String,
                                             ByVal pratica_cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Try
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim pratiche As New AgronicaCoreProfilazioneBIZ.Pratiche_W

            r = pratiche.impostaPratica(0,
                                        Piva,
                                        Cuaa,
                                        objParametri_Server.UtenteUsername,
                                        enum_Servizi.Gestione_UMA,
                                        2009,
                                        objParametri_Server,
                                        objParametri_Utenti, 0,
                                        "", pratica_cod,
                                        False, "",
                                        AGRODATAINIZIO,
                                        AGRODATAFINE, 0, 0)

            If r.Errore = "" Then
                r.RispostaOK = True
            End If

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

#End Region

#Region "Stampa"

    <WebMethod(EnableSession:=True)>
    Public Shared Function StampaRichiesta(ByVal piva As String,
                                           ByVal richiestaAvanz As Integer,
                                           ByVal richiestaCod As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim objAgronicaStampe As New AgronicaCoreGestioneRichieste.ParametriAgronicaStampe
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If richiestaAvanz = 1 Then
            objAgronicaStampe.report = enum_CodificaStampe.UMA_RendicontazioneCarb
        Else
            objAgronicaStampe.report = enum_CodificaStampe.UMA_RichiestaCarbPrevisioneLav
        End If

        Try
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim vVarStampe(1) As ElementoStampe

            vVarStampe(0).Nome = "piva"
            vVarStampe(0).Valore = piva

            vVarStampe(1).Nome = "richiesta_cod"
            vVarStampe(1).Valore = richiestaCod


            Dim objVS As New AgronicaCoreXML.XML_Stampe
            Dim StrNodo As String = objVS.XML_VariabiliStampe(vVarStampe)
            Dim StrNodiVariabili As String = StrNodo

            objAgronicaStampe.username = CStr(HttpContext.Current.Session("ASG_Utente_Username"))
            objAgronicaStampe.user_profilo = CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS"))
            objAgronicaStampe.Xml_Generico.Length = 0
            objAgronicaStampe.Xml_Generico.Append(StrNodiVariabili)

            Dim link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                                    Enum_SiteRedirector.Sito_AgronicaUma, objAgronicaStampe)



            r.RispostaOK = True
            r.RispostaStringa = link

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

#End Region

#Region "Caricamento"

    Private Sub caricaControlli()

        'Select Case objParametriAgenda.Tipo_Operazione

        '    Case TipiEnumerativi.enum_TipoOperazioneDB.Scrittura

        '    Case TipiEnumerativi.enum_TipoOperazioneDB.Modifica, TipiEnumerativi.enum_TipoOperazioneDB.Lettura
        '        RipristinaControlliDaAgenda()

        'End Select
    End Sub

    <WebMethod(EnableSession:=True)>
    Public Shared Function CheckPerc(ByVal anno As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim setup As New AgronicaCoreUmaDal.UMASetup_R
        Dim Dt As DataTable
        Dim perc As Integer

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dt = setup.LeggiSetup(anno, objParametri_Server)

            If Dt.Rows.Count > 0 Then
                perc = Dt.Rows.Item(0).Item("Per_Riduzione")
            Else
                perc = 23
            End If

            r.RispostaStringa = JsonConvert.SerializeObject(perc, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "ErroreDuranteOperazione_" & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

#End Region

#Region "Ricerca Macrousi Laorazioni"

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Causali_UMA() As RispostaStandard

        Dim r As New RispostaStandard

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim leggiCausali As New UMA_Causali_BIZ

        Try

            Dim Dt = leggiCausali.Leggi(objParametri_Server)

            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Approvatori() As RispostaStandard

        Dim r As New RispostaStandard

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim leggiApprovatori As New UMA_Richieste_Testata_R

        Try

            Dim Dt = leggiApprovatori.Leggi_Utenti_Approvatori(objParametri_Utenti, objParametri_Server)

            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Allevamenti() As RispostaStandard

        Dim r As New RispostaStandard

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim leggiAllevamenti As New UMA_Allevamenti_R

        Try

            Dim Dt = leggiAllevamenti.Leggi("", "", objParametri_Server)

            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Lavorazioni() As RispostaStandard

        Dim r As New RispostaStandard

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim leggiLavorazioni As New UMA_Richieste_Lavorazioni_R

        Try

            Dim Dt = leggiLavorazioni.Leggi_Elenco_Lavorazioni(objParametri_Server)

            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Colture() As RispostaStandard

        Dim r As New RispostaStandard

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim leggiColture As New UMA_Richieste_Lavorazioni_R

        Try

            Dim Dt = leggiColture.Leggi_Elenco_Colture(objParametri_Server)

            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Ricerca_Macrousi_Lavorazioni(tipoPratica As Integer,
                                                        statoCod As Integer,
                                                        citta As String,
                                                        prov As String,
                                                        approvatore As String,
                                                        causali As String,
                                                        dettaglio As Integer,
                                                        colture As String,
                                                        lavorazione As String,
                                                        allevamento As String,
                                                        anno As Integer,
                                                        filtroConto As Integer,
                                                        nuovaVisibilita As Boolean) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As New DataTable
        Dim ind_des As String = ""
        Dim com_des As String = ""
        Dim rag_soc As String = ""
        Dim frz_des As String = ""
        Dim CAP As String = ""
        Dim CUAA As String = ""
        Dim leggiIndirizzo As New AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_R
        Dim leggiCUAA As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim objTestate As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R
        Dim objProfilo As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
        Dim Filtro_Visibilita_Utente = not nuovaVisibilita andalso Not objProfilo.HasFullVisibility(
            HttpContext.Current.Session("ASG_Utente_Username"), objParametri_Utenti)

        Dim piveVisibili As String = ""
        Dim FiltroUtente As Boolean = False
        Dim FiltroGruppo As Boolean = False
        Dim Gruppo As Integer = 0
        Dim VisibilitaTotale As Boolean = False

        If (nuovaVisibilita) Then
            Dim objUtenti_Visibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            objUtenti_Visibilita.OttieniTipoFiltroVisibilita(objParametri_Utenti, 0, FiltroUtente, FiltroGruppo, Gruppo, VisibilitaTotale)

        End If

        Dim dtRichiesteUmaTestat = objTestate.Ricerca_Macrousi_Lavorazioni(anno,
                                                                         "",
                                                                         "",
                                                                         objParametri_Server,
                                                                         objParametri_Utenti,
                                                                         Filtro_Visibilita_Utente,
                                                                         piveVisibili,
                                                                         rendicontazioni:=tipoPratica,
                                                                         statoCod:=statoCod,
                                                                         citta:=citta,
                                                                         prov:=prov,
                                                                         conto:=filtroConto,
                                                                         approvatore:=approvatore,
                                                                         causali:=causali,
                                                                         dettaglio:=dettaglio,
                                                                         colture:=colture,
                                                                         lavorazione:=lavorazione,
                                                                         allevamento:=allevamento,
                                                                         nuovaVisibilita:=nuovaVisibilita,
                                                                         FiltroUtente:=FiltroUtente,
                                                                         FiltroGruppoUtente:=FiltroGruppo,
                                                                         GruppoUtente:=Gruppo,
                                                                         VisibilitaTotale:=VisibilitaTotale)

        r.RispostaStringa = JsonConvert.SerializeObject(dtRichiesteUmaTestat, Formatting.None, serializerSettings)
        r.RispostaOK = True
        Return r

    End Function


#End Region

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        inizializzoObjParametri()
        inizializzoParametriPagina()


        Me.Master.flag_MostraBtnIndietro = False

        Dim Entrata_Diretta As Integer = 0
        Dim Split As Integer = 0

        QS_Avanzamento = 0
        If Not IsNothing(Request.QueryString("avanzamento")) Then
            QS_Avanzamento = CInt(Request.QueryString("avanzamento"))
        End If

        If Not IsNothing(Request.QueryString("p")) Then
            QS_Piva = Stringa_Decodifica(Request.QueryString("p").ToString, AgroKey_EncoderDecoder, Server)
        End If

        Dim enumAppro = IIf(QS_Avanzamento = 0, enum_Security_Attivita.Approvazione_Richiesta_UMA, enum_Security_Attivita.Approvazione_Rendicontazione_UMA)
        Dim enumBase = IIf(QS_Avanzamento = 0, enum_Security_Attivita.Richiesta_UMA, enum_Security_Attivita.Rendicontazione_UMA)

        'Controllo se l'utente ha i permessi per accedere
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim UtenteAbilitatoLettura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"),
                                            Session("ASG_IdServizio"),
                                            enumAppro,
                                            enum_Security_Operazione.Lettura,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)
        If Not UtenteAbilitatoLettura Then
            UtenteAbilitatoLettura = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"),
                                            Session("ASG_IdServizio"),
                                            enumBase,
                                            enum_Security_Operazione.Lettura,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)
        End If

        Dim UtenteAbilitatoScrittura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                           Session("ASG_Utente_Username"),
                                           Session("ASG_IdServizio"),
                                           enumAppro,
                                           enum_Security_Operazione.Modifica,
                                           Date.Now,
                                           "",
                                           objParametri_Utenti)

        If Not UtenteAbilitatoScrittura Then
            UtenteAbilitatoScrittura = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"),
                                            Session("ASG_IdServizio"),
                                            enumBase,
                                            enum_Security_Operazione.Modifica,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)
        End If

        'Pagina di origine
        hdPaginaRedirect.Value = ""
        hdPaginaRedirect_Codificata.Value = ""

        'Imposto le variabili di ponte con il client
        hf_UtenteAbilitatoLettura.Value = UtenteAbilitatoLettura
        hf_UtenteAbilitatoScrittura.Value = UtenteAbilitatoScrittura

        If Not UtenteAbilitatoLettura Then
            If String.IsNullOrWhiteSpace(CStr(hdPaginaRedirect_Codificata.Value)) Then
                Response.Redirect("~/Menu/MenuBS_2017.aspx")
            Else
                Response.Redirect(CStr(hdPaginaRedirect.Value) & "?p=" & Request.QueryString("p"))
            End If
        End If

        Dim objGruppi_Utenti As New AgronicaCoreUtentiDAL.Gruppi_Utente_R
        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R
        Dim coordinatore As Boolean
        Dim DTGruppoUtente = objUtenti.Leggi(objParametri_Server.UtenteUsername, "0", "", "", objParametri_Utenti)
        If DTGruppoUtente.Rows.Count > 0 Then
            Dim DTConfigurazioneGruppo = objGruppi_Utenti.Leggi(DTGruppoUtente.Rows(0)("Gruppi_Utente_cod"), "", "", objParametri_Utenti)
            If DTConfigurazioneGruppo.Rows.Count > 0 AndAlso Not IsDBNull(DTConfigurazioneGruppo.Rows(0)("ConfigurazioniAggiuntive")) Then
                Dim ConfigurazioneGruppoStr = CStr(DTConfigurazioneGruppo.Rows(0)("ConfigurazioniAggiuntive"))
                If ConfigurazioneGruppoStr <> "" Then
                    Try
                        Dim ConfigurazioneGruppo = JObject.Parse(ConfigurazioneGruppoStr)
                        If ConfigurazioneGruppo("CoordinatoreAfor") IsNot Nothing Then
                            If ConfigurazioneGruppo("CoordinatoreAfor") Then
                                coordinatore = True
                            End If
                        End If
                    Catch ex As Exception

                    End Try
                End If
            End If
        End If

        hdId_Agenda.Value = 0
        hdId_Mov.Value = 0
        hdId_Mov_Det.Value = 0
        hdId_Agenda_CDG.Value = 0
        hdModalita.Value = Entrata_Diretta
        hdId_CDG.Value = 0
        hdLav_Cod.Value = 0
        hdVeg_Cod.Value = 0
        hdSuperUser.Value = objParametri_Server.SuperUserUsername
        hdutenteUsername.Value = objParametri_Server.UtenteUsername
        hcoordinatoreAfor.Value = coordinatore
        hdDes_Lib.Value = ""
        hdMov_Desc.Value = ""
        hdSplit.Value = Split
        hdData.Value = JToken.Parse(JsonConvert.SerializeObject(Now))

        'Automatico (Impostazione da QDC)
        hdAutomatico.Value = 1 'Default

        If Not Page.IsPostBack Then


            If Not IsNothing(Session("ParametriAgenda_2010")) Then

                Dim objParametriAgenda_2010 As New ParametriAgenda_2010
                objParametriAgenda_2010.Leggi()
                hdPiva.Value = objParametriAgenda_2010.Piva
                hdId_Agenda.Value = Val(objParametriAgenda_2010.Id_Agenda)

            End If

            'If Entrata_Diretta = 0 Then

            'Introdotta per non rieseguirlo se appena creata con provenienza da APP
            Dim bombardinoDaEseguire = True

            If Val(hdId_Agenda.Value) = 0 Then

            End If

        End If

    End Sub


    Private Sub inizializzoParametriPagina()


    End Sub


    Public Shadows ReadOnly Property Master() As AgronicaUMA.UmaBootstrap
        Get
            Return CType(MyBase.Master, AgronicaUMA.UmaBootstrap)
        End Get
    End Property

End Class