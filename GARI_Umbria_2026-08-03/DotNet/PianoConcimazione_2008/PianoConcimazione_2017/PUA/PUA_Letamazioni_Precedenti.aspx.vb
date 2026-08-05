Imports System.Web.Services
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json

Public Class PUA_Letamazioni_Precedenti
    Inherits System.Web.UI.Page

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri

    Private _objParametriUtenti As AgronicaCoreParametri
    Private _objParametriServer As AgronicaCoreParametri

    Private _qsOperazione As String
    Private _qsPiva As String
    Private _qsSaCod As Integer
    Private _qsAppezza As Integer
    Private _qsIdReg As Integer
    Private _qsProgettoCod As Integer
    Private _qsPuaCod As Integer
    Private _qsRegCod As Integer
    Private _qsIdAnagrafeVincoli As Integer
    Private _qsDataInizio As String
    Private _qsDataFine As String


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Master.flag_MostraBtnIndietro = False

        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("Messaggi/AccessoNegato.htm")
        End If

        _objParametriUtenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        _objParametriServer = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        If Not IsNothing(Request.QueryString("id")) Then
            _qsIdAnagrafeVincoli = Stringa_Decodifica(Request.QueryString("id").ToString, AgroKey_EncoderDecoder, Server)
        End If
        If Not IsNothing(Request.QueryString("piva")) Then
            _qsPiva = Stringa_Decodifica(Request.QueryString("piva").ToString, AgroKey_EncoderDecoder, Server)
        End If
        If Not IsNothing(Request.QueryString("sa_cod")) Then
            _qsSaCod = Stringa_Decodifica(Request.QueryString("sa_cod").ToString, AgroKey_EncoderDecoder, Server)
        End If
        If Not IsNothing(Request.QueryString("appezza")) Then
            _qsAppezza = Stringa_Decodifica(Request.QueryString("appezza").ToString, AgroKey_EncoderDecoder, Server)
        End If
        If Not IsNothing(Request.QueryString("id_reg")) Then
            _qsIdReg = Stringa_Decodifica(Request.QueryString("id_reg").ToString, AgroKey_EncoderDecoder, Server)
        End If
        If Not IsNothing(Request.QueryString("progetto_cod")) Then
            _qsProgettoCod = Stringa_Decodifica(Request.QueryString("progetto_cod").ToString, AgroKey_EncoderDecoder, Server)
        End If
        If Not IsNothing(Request.QueryString("pua_cod")) Then
            _qsPuaCod = Stringa_Decodifica(Request.QueryString("pua_cod").ToString, AgroKey_EncoderDecoder, Server)
        End If
        If Not IsNothing(Request.QueryString("regolamento_cod")) Then
            _qsRegCod = Stringa_Decodifica(Request.QueryString("regolamento_cod").ToString, AgroKey_EncoderDecoder, Server)
        End If

        If Not IsNothing(Request.QueryString("data_inizio")) Then
            _qsDataInizio = Stringa_Decodifica(Request.QueryString("data_inizio").ToString, AgroKey_EncoderDecoder, Server)
        End If
        If Not IsNothing(Request.QueryString("data_fine")) Then
            _qsDataFine = Stringa_Decodifica(Request.QueryString("data_fine").ToString, AgroKey_EncoderDecoder, Server)
        End If


        hdIdAnagrafeVincoli.Value = _qsIdAnagrafeVincoli
        hdPiva.Value = _qsPiva
        hdSaCod.Value = _qsSaCod
        hdAppezza.Value = _qsAppezza
        hdIdReg.Value = _qsIdReg
        hdProgettoCod.Value = _qsProgettoCod
        hdPuaCod.Value = _qsPuaCod
        hdRegCod.Value = _qsRegCod
        hdIdDataInizio.Value = _qsDataInizio
        hdIdDataFine.Value = _qsDataFine

        Master.Lbl_Titolo.Text = "Letamazioni Precedenti"
        Master.flag_MostraHeader = False
        Master.flag_MostraFooter = False

        If Not Page.IsPostBack Then

        End If



    End Sub


    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiLetamazioniPrecedenti(
        ByVal regolamento_cod As Integer,
        ByVal pua_cod As Integer,
        ByVal id As Integer,
        ByVal piva As String,
        ByVal sa_cod As Integer,
        ByVal appezza As Integer,
        ByVal id_reg As Integer,
        ByVal progetto_cod As Integer
    ) As RispostaStandard


        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim strEffluenti As String = Leggi_Letamazioni(regolamento_cod, pua_cod, id, piva, sa_cod, appezza, id_reg, progetto_cod, objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = strEffluenti

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r

    End Function

    Private Shared Function Leggi_Letamazioni(ByVal regolamento_cod As Integer, ByVal pua_cod As Integer,
                                                 ByVal id As Integer, ByVal piva As String, ByVal sa_cod As Integer, ByVal appezza As Integer, ByVal id_reg As Integer, ByVal progetto_cod As Integer,
                                                        objParametri_Server As AgronicaCoreParametri) As String

        Dim strEffluenti As String = ""

        Try

            Dim strErr As String = ""

            Dim objLetPrec As New AgronicaCorePUA_DAL.PUA_LetamazioniPrecedenti_R
            Dim DtEffluenti As DataTable = objLetPrec.Leggi(regolamento_cod, pua_cod, piva, sa_cod, appezza, id_reg, progetto_cod, "", "", objParametri_Server)

            Dim objEFOutput As AgronicaCorePianoConcimazioneBIZ.PUA_EffluentiXFrequenza_output

            If regolamento_cod <> 0 Then
                Dim objPC As New AgronicaCoreWebService.PianoConcimazione_WS
                Dim objEFInput As New AgronicaCorePianoConcimazioneBIZ.PUA_EffluentiXFrequenza_input
                objEFInput.Regolamento_Cod = regolamento_cod
                objEFOutput = objPC.EffluentiXFrequenza(objEFInput)
            End If

            DtEffluenti.Columns.Add(New DataColumn("eff_des", GetType(String)))
            DtEffluenti.Columns.Add(New DataColumn("frequenza_des", GetType(String)))
            DtEffluenti.Columns.Add(New DataColumn("udm_sim", GetType(String)))
            DtEffluenti.Columns.Add(New DataColumn("n_riduzione", GetType(Decimal)))
            DtEffluenti.Columns.Add(New DataColumn("n_residuo", GetType(Decimal)))


            Dim Eff_Cod As Integer
            Dim Eff_Des As String
            Dim Id_Fre As Integer
            Dim Frequenza_Des As String
            Dim Udm_Sim As String
            Dim N_Riduzione As Decimal


            If Not DtEffluenti Is Nothing Then

                For i = 0 To DtEffluenti.Rows.Count - 1

                    Eff_Cod = DtEffluenti.Rows(i).Item("eff_cod")
                    Eff_Des = ""
                    Id_Fre = DtEffluenti.Rows(i).Item("id_fre")
                    Frequenza_Des = ""
                    Udm_Sim = ""
                    N_Riduzione = 0

                    If Not IsNothing(objEFOutput) Then
                        Dim Effluente As New AgronicaCorePianoConcimazioneBIZ.PUA_EffluentiXFrequenza
                        Effluente = objEFOutput.ListaEffluentiXFrequenza.Where(Function(x) x.Eff_Cod = Eff_Cod And x.Id_Fre = Id_Fre)(0)
                        If Not Effluente Is Nothing Then
                            Eff_Des = Effluente.Eff_Des
                            Frequenza_Des = Effluente.Frequenza_Des
                            Udm_Sim = Effluente.Udm_Sim
                            N_Riduzione = Effluente.N
                        End If
                    End If

                    DtEffluenti.Rows(i).Item("Eff_Des") = Eff_Des
                    DtEffluenti.Rows(i).Item("frequenza_des") = Frequenza_Des
                    DtEffluenti.Rows(i).Item("Udm_Sim") = Udm_Sim
                    DtEffluenti.Rows(i).Item("n_riduzione") = N_Riduzione

                    DtEffluenti.Rows(i).Item("n_residuo") = 0

                    If Not IsDBNull(DtEffluenti.Rows(i).Item("n_titolo")) AndAlso Not IsDBNull(DtEffluenti.Rows(i).Item("qta")) Then
                        DtEffluenti.Rows(i).Item("n_residuo") = Math.Round(CDec(DtEffluenti.Rows(i).Item("qta")) * CDec(DtEffluenti.Rows(i).Item("n_titolo")) * N_Riduzione, 2)
                    End If

                    'arrotondo dopo aver fatto il calcolo
                    If Not IsDBNull(DtEffluenti.Rows(i).Item("qta")) AndAlso IsNumeric(DtEffluenti.Rows(i).Item("qta")) Then
                        DtEffluenti.Rows(i).Item("qta") = Math.Round(DtEffluenti.Rows(i).Item("qta"), 2)
                    End If
                    If Not IsDBNull(DtEffluenti.Rows(i).Item("n_titolo")) AndAlso IsNumeric(DtEffluenti.Rows(i).Item("n_titolo")) Then
                        DtEffluenti.Rows(i).Item("n_titolo") = Math.Round(DtEffluenti.Rows(i).Item("n_titolo"), 2)
                    End If

                Next

            End If

            strEffluenti = JSON_DataTableLetamazioniPrecedenti_Tabella(DtEffluenti)

        Catch ex As Exception

            Return ex.Message

        End Try

        Return strEffluenti

    End Function

    Private Shared Function JSON_DataTableLetamazioniPrecedenti_Tabella(ByRef DT_Effluenti As DataTable,
                                                                    Optional ByVal stringaKendoRow As String = "") As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        l.Add(New ColonneNome("id", "id", "number") With {._hidden = True})
        l.Add(New ColonneNome("eff_cod", "eff_cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("eff_des", "eff_des", "string") With {._hidden = True})
        l.Add(New ColonneNome("udm_cod", "udm_cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("udm_sim", "Unita Misura [Ha]", "string") With {._css = "allineadestra", ._cssHeader = "allineadestra"})
        l.Add(New ColonneNome("qta", "Qta", "number") With {._formatNr = "n2", ._css = "allineadestra", ._cssHeader = "allineadestra"})
        l.Add(New ColonneNome("n_titolo", "Titolo [kg/Unita Misura]", "number") With {._formatNr = "n2", ._css = "allineadestra", ._cssHeader = "allineadestra"})
        l.Add(New ColonneNome("id_fre", "id_fre", "number") With {._hidden = True})
        l.Add(New ColonneNome("frequenza_des", "frequenza_des", "string") With {._hidden = True})
        l.Add(New ColonneNome("n_riduzione", "Riduzione [%]", "number") With {._formatNr = "n2", ._css = "allineadestra", ._cssHeader = "allineadestra"})
        l.Add(New ColonneNome("n_residuo", "N Residuo [kg/Unita Misura]", "number") With {._formatNr = "n2", ._css = "allineadestra", ._cssHeader = "allineadestra", ._sum = True})

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        js.Editabile_Deafault = True

        Dim risp As String = js.JSON_DataTable_Kendo(DT_Effluenti, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.Menu,
                                           stringaKendoRow:=stringaKendoRow) 'True, True, TipoFiltroKendo_colonne.CasellaTesto) '

        Return risp

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiEffluenti(regolamento_cod As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objEffluentiInput As New AgronicaCorePianoConcimazioneBIZ.PUA_EffluentiXFrequenza_input
        objEffluentiInput.Regolamento_Cod = regolamento_cod

        Dim objPC As New AgronicaCoreWebService.PianoConcimazione_WS
        Dim objEffluentiOutput As AgronicaCorePianoConcimazioneBIZ.PUA_EffluentiXFrequenza_output = objPC.EffluentiXFrequenza(objEffluentiInput)

        Dim jArrayListaOp As New JArray()
        Dim HashEff As New Hashtable
        For Each e As AgronicaCorePianoConcimazioneBIZ.PUA_EffluentiXFrequenza In objEffluentiOutput.ListaEffluentiXFrequenza
            If Not HashEff.ContainsKey(e.Eff_Cod) Then
                Dim elem As JObject = New JObject(New JProperty("eff_cod", e.Eff_Cod),
                                              New JProperty("eff_des", e.Eff_Des),
                                              New JProperty("udm_cod", e.Udm_Cod),
                                              New JProperty("udm_sim", e.Udm_Sim),
                                              New JProperty("n", e.N_Titolo))
                If Not jArrayListaOp.Contains(elem) Then
                    jArrayListaOp.Add(elem)
                End If
                HashEff.Add(e.Eff_Cod, "")
            End If
        Next

        r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
        r.RispostaOK = True

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiEffluentiXFrequenza(regolamento_cod As String, eff_cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objEffluentiInput As New AgronicaCorePianoConcimazioneBIZ.PUA_EffluentiXFrequenza_input
        objEffluentiInput.Regolamento_Cod = regolamento_cod
        objEffluentiInput.Eff_Cod = eff_cod

        Dim objPC As New AgronicaCoreWebService.PianoConcimazione_WS
        Dim objEffluentiOutput As AgronicaCorePianoConcimazioneBIZ.PUA_EffluentiXFrequenza_output = objPC.EffluentiXFrequenza(objEffluentiInput)

        Dim jArrayListaOp As New JArray()
        For Each e As AgronicaCorePianoConcimazioneBIZ.PUA_EffluentiXFrequenza In objEffluentiOutput.ListaEffluentiXFrequenza
            Dim elem As JObject = New JObject(New JProperty("id_fre", e.Id_Fre),
                                              New JProperty("frequenza_des", e.Frequenza_Des),
                                              New JProperty("rid", e.N))
            If Not jArrayListaOp.Contains(elem) Then
                jArrayListaOp.Add(elem)
            End If
        Next

        r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
        r.RispostaOK = True

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva(ByVal regolamento_cod As Integer, ByVal pua_cod As Integer,
                                      ByVal id As Integer, ByVal piva As String, ByVal sa_cod As Integer, ByVal appezza As Integer, ByVal id_reg As Integer, ByVal progetto_cod As Integer,
                                                        dati As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim Flag_Connessione, Flag_Transazione As Boolean

        Try

            Utility.VerificaApriTransazione(objParametri_Server, Flag_Connessione, Flag_Transazione)

            Dim HashLetMod As New Hashtable
            Dim DtLetOLD As DataTable
            Dim TipoOperazione As enum_TipoOperazioneDB

            'leggo per il log successivo
            Dim objPUALog As New AgronicaCorePUA_DAL.AgronicaLogPua_W
            Dim objLet As New AgronicaCorePUA_DAL.PUA_LetamazioniPrecedenti_R
            DtLetOLD = objLet.Leggi(regolamento_cod, pua_cod, piva, sa_cod, appezza, id_reg, progetto_cod, "", "", objParametri_Server)

            Dim objPUA_LetPrec_W As New AgronicaCorePUA_DAL.PUA_LetamazioniPrecedenti_W
            Dim bRet As Boolean = objPUA_LetPrec_W.Cancella(CInt(pua_cod), regolamento_cod, piva, sa_cod, appezza, id_reg, progetto_cod, "", objParametri_Server)

            Dim NTot As Decimal = 0

            If bRet = True Then

                Dim rows = JArray.Parse(dati)

                For Each row In rows

                    If Not row("n_residuo") Is Nothing AndAlso IsNumeric(row("n_residuo")) Then
                        NTot += CDec(row("n_residuo"))
                    End If

                    Select Case row("id")
                        Case "0"
                            TipoOperazione = enum_TipoOperazioneDB.Scrittura
                        Case Else
                            TipoOperazione = enum_TipoOperazioneDB.Modifica
                            HashLetMod.Add(CInt(row("id")), "")
                    End Select

                    bRet = objPUA_LetPrec_W.Scrivi(row("id"), regolamento_cod, pua_cod,
                                                   piva, sa_cod, appezza, id_reg, progetto_cod,
                                                   row("eff_cod"), row("id_fre"), row("udm_cod"),
                                                   If(IsNumeric(row("qta")), row("qta"), 0), If(IsNumeric(row("n_titolo")), row("n_titolo"), 0),
                                                    AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)

                    If bRet = True Then
                        objPUALog.Scrivi(TipoOperazione, "PUA_LetamazioniPrecedenti", pua_cod, regolamento_cod, row("id"), piva, sa_cod, appezza, id_reg, progetto_cod, row("eff_cod"), row("id_fre"), "", objParametri_Server)
                    End If

                Next

                'loggo i cancellati
                If Not DtLetOLD Is Nothing Then
                    For Each drC As DataRow In DtLetOLD.Rows
                        If Not HashLetMod.ContainsKey(drC.Item("id")) Then
                            objPUALog.Scrivi(enum_TipoOperazioneDB.Cancellazione, "PUA_LetamazioniPrecedenti", pua_cod, regolamento_cod, drC.Item("id"), piva, sa_cod, appezza, id_reg, progetto_cod, drC.Item("eff_cod"), drC.Item("id_fre"), "", objParametri_Server)
                        End If
                    Next
                End If

            End If


            'modifico Nf nella tabella anagrafe_vincoli
            Dim objAV As New AgronicaCoreAnagrafeDAL.Anagrafe_VincoliAgronomici_W
            objAV.ModificaPuntuale(id, objParametri_Server, N_FertilizzazioniPrecedenti:=NTot)


            Utility.VerificaChiudiTransazione(objParametri_Server, Flag_Transazione)

            r.RispostaOK = True
            r.RispostaStringa = "Salvataggio avvenuto con successo."
            r.ParametroDue_stringa = NTot

        Catch ex As Exception

            Utility.VerificaAnnullaTransazione(objParametri_Server, Flag_Transazione)

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        Finally

            Utility.VerificaChiudiConnessione(objParametri_Server, Flag_Connessione)

        End Try

        Return r

    End Function




    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiLetamazioniPrecedentiQdC(
        ByVal regolamento_cod As Integer,
        ByVal pua_cod As Integer,
        ByVal id As Integer,
        ByVal piva As String,
        ByVal sa_cod As Integer,
        ByVal appezza As Integer,
        ByVal id_reg As Integer,
        ByVal progetto_cod As Integer,
        ByVal data_inizio As String, ByVal data_fine As String
    ) As RispostaStandard


        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim strEffluenti As String = Leggi_LetamazioniQdC(regolamento_cod, pua_cod, id, piva, sa_cod, appezza, id_reg, progetto_cod, data_inizio, data_fine, objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = strEffluenti

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r

    End Function

    Private Shared Function Leggi_LetamazioniQdC(ByVal regolamento_cod As Integer, ByVal pua_cod As Integer,
                                                 ByVal id As Integer, ByVal piva As String, ByVal sa_cod As Integer, ByVal appezza As Integer, ByVal id_reg As Integer, ByVal progetto_cod As Integer,
                                                    ByVal data_inizio As String, ByVal data_fine As String,
                                                 objParametri_Server As AgronicaCoreParametri) As String

        Dim strEffluenti As String = ""

        Try

            Dim strErr As String = ""

            Dim HashLetami As New Hashtable
            Dim str_FerCod_Letami As String = ""
            Dim objEFOutput As AgronicaCorePianoConcimazioneBIZ.PUA_EffluentiXFrequenza_output
            If regolamento_cod <> 0 Then

                Dim objPC As New AgronicaCoreWebService.PianoConcimazione_WS
                Dim objEFInput As New AgronicaCorePianoConcimazioneBIZ.PUA_EffluentiXFrequenza_input
                objEFInput.Regolamento_Cod = regolamento_cod
                objEFOutput = objPC.EffluentiXFrequenza(objEFInput)

                For Each e As AgronicaCorePianoConcimazioneBIZ.PUA_EffluentiXFrequenza In objEFOutput.ListaEffluentiXFrequenza
                    If Not HashLetami.ContainsKey(e.Fer_Cod) Then
                        HashLetami.Add(e.Fer_Cod, "")
                        str_FerCod_Letami &= e.Fer_Cod & ","
                    End If
                Next

                If str_FerCod_Letami <> "" Then
                    str_FerCod_Letami = "(" & Left(str_FerCod_Letami, str_FerCod_Letami.Length - 1) & ")"
                End If

            End If

            Dim DataDa As Date = AGRODATAINIZIO
            Dim DataA As Date = AGRODATAFINE
            If IsDate(data_inizio) Then
                DataDa = CDate(data_inizio)
            End If
            If IsDate(data_fine) Then
                DataA = CDate(data_fine)
            End If

            Dim FiltroMovimenti As String = " m.cau_mov ='2300' and m.data_movimento < '" & DataDa.ToString & "'"
            Dim FiltroMovimentiDettagli As String = " md.elem_cod=3 and md.pro_cod in " & str_FerCod_Letami

            Dim objLeggiAppezzamentoDal As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
            Dim xLetQdC As DataTable =
            objLeggiAppezzamentoDal.LeggiAppezzamentiConCatastoSovrapposto_LetamazioniPrecedenti(piva, sa_cod, appezza, FiltroMovimenti, FiltroMovimentiDettagli, "", objParametri_Server)
            'Dim xLetQdC As DataTable =
            'objLeggiAppezzamentoDal.LeggiAppezzamentiConCatastoSovrapposto_LetamazioniPrecedenti(piva, sa_cod, appezza, " pro_cod in " & str_FerCod_Letami & " And data_movimento <'" & DataDa.ToString & "'", "", objParametri_Server)

            Dim xLetPrec As New DataTable
            Dim DrLetPrec As DataRow
            xLetPrec.Columns.Add(New DataColumn("id", GetType(Integer)))
            xLetPrec.Columns.Add(New DataColumn("id_agenda", GetType(Integer)))
            xLetPrec.Columns.Add(New DataColumn("pro_cod", GetType(Integer)))
            xLetPrec.Columns.Add(New DataColumn("data_movimento", GetType(Date)))
            xLetPrec.Columns.Add(New DataColumn("app_nome", GetType(String)))
            xLetPrec.Columns.Add(New DataColumn("veg_des", GetType(String)))
            xLetPrec.Columns.Add(New DataColumn("eff_cod", GetType(Integer)))
            xLetPrec.Columns.Add(New DataColumn("eff_des", GetType(String)))
            xLetPrec.Columns.Add(New DataColumn("id_fre", GetType(Integer)))
            xLetPrec.Columns.Add(New DataColumn("frequenza_des", GetType(String)))
            xLetPrec.Columns.Add(New DataColumn("udm_cod", GetType(Integer)))
            xLetPrec.Columns.Add(New DataColumn("udm_sim", GetType(String)))
            xLetPrec.Columns.Add(New DataColumn("qta", GetType(Decimal)))
            xLetPrec.Columns.Add(New DataColumn("n", GetType(Decimal)))
            xLetPrec.Columns.Add(New DataColumn("n_riduzione", GetType(Decimal)))
            xLetPrec.Columns.Add(New DataColumn("n_residuo", GetType(Decimal)))


            Dim Anno_Precedente_Inizio As Date = DateAdd(DateInterval.Year, -1, DataDa)
            Dim Anno_Precedente_Fine As Date = DateAdd(DateInterval.Year, -1, DataA)
            Dim Anno_2Precedente_Inizio As Date = DateAdd(DateInterval.Year, -2, DataDa)
            Dim Anno_2Precedente_Fine As Date = DateAdd(DateInterval.Year, -2, DataA)
            Dim Anno_3Precedente_Inizio As Date = DateAdd(DateInterval.Year, -3, DataDa)
            Dim Anno_3Precedente_Fine As Date = DateAdd(DateInterval.Year, -3, DataA)

            Dim Eff_Cod As Integer
            Dim Eff_Des As String
            Dim Id_Fre As Integer
            Dim Frequenza_Des As String
            Dim Udm_Cod As Integer
            Dim Udm_Sim As String
            Dim N_Riduzione As Decimal
            Dim Qta As Decimal
            Dim ExtraInt As Integer

            Dim DatoPresente As Boolean

            If Not xLetQdC Is Nothing Then

                For i = 0 To xLetQdC.Rows.Count - 1

                    DatoPresente = False

                    For j = 0 To xLetPrec.Rows.Count - 1

                        'per la stessa operazione e lo stesso effluente conteggio solo una volta la qta per cui aggiorno solo l'elenco degli appezzamenti
                        If xLetPrec.Rows(j).Item("id_agenda") = xLetQdC.Rows(i).Item("Id_Agenda") And
                                xLetPrec.Rows(j).Item("pro_cod") = xLetQdC.Rows(i).Item("pro_cod") Then
                            xLetPrec.Rows(j).Item("app_nome") &= "," & xLetQdC.Rows(i).Item("app_nome")
                            DatoPresente = True
                            Exit For
                        End If

                    Next

                    If DatoPresente = False Then

                        DrLetPrec = xLetPrec.NewRow

                        DrLetPrec.Item("id_agenda") = xLetQdC.Rows(i).Item("Id_Agenda")
                        DrLetPrec.Item("pro_cod") = xLetQdC.Rows(i).Item("pro_cod")

                        'DrLetPrec.Item("id") = xLetQdC.Rows(i).Item("id")
                        DrLetPrec.Item("data_movimento") = xLetQdC.Rows(i).Item("data_movimento")
                        DrLetPrec.Item("veg_des") = xLetQdC.Rows(i).Item("veg_des")
                        DrLetPrec.Item("app_nome") = xLetQdC.Rows(i).Item("app_nome")

                        Eff_Cod = 0
                        Eff_Des = ""
                        Id_Fre = 0
                        Frequenza_Des = ""
                        Udm_Cod = 0
                        Udm_Sim = ""
                        N_Riduzione = 0
                        Qta = 0

                        Select Case DrLetPrec.Item("Data_Movimento")
                            Case Anno_Precedente_Inizio To Anno_Precedente_Fine
                                Select Case xLetQdC.Rows(i).Item("cul_cod")
                                    Case 0
                                        Id_Fre = 1
                                    Case Else
                                        Id_Fre = 2
                                End Select
                            Case Anno_2Precedente_Inizio To Anno_2Precedente_Fine
                                Select Case xLetQdC.Rows(i).Item("cul_cod")
                                    Case 0
                                        Id_Fre = 2
                                    Case Else
                                        Id_Fre = 3
                                End Select
                            Case Anno_3Precedente_Inizio To Anno_3Precedente_Fine
                                Select Case xLetQdC.Rows(i).Item("cul_cod")
                                    Case 0
                                        Id_Fre = 3
                                    Case Else
                                        Id_Fre = 0
                                End Select
                        End Select

                        'anno corrente --> conteggio il letame distribuito lo scorso anno sui terreni nudi
                        'DR_Anno_Precedente = xDtcolduraPrec.Select("cul_cod=0 and data_movimento<=#" & Anno_Precedente_Fine.ToString("MM/dd/yyyy") & "# and data_movimento>=#" & Anno_Precedente_Inizio.ToString("MM/dd/yyyy") & "#", "pro_cod")

                        If Not IsNothing(objEFOutput) Then
                            Dim Effluente As New AgronicaCorePianoConcimazioneBIZ.PUA_EffluentiXFrequenza
                            Effluente = objEFOutput.ListaEffluentiXFrequenza.Where(Function(x) x.Fer_Cod = xLetQdC.Rows(i).Item("pro_cod") And x.Id_Fre = Id_Fre)(0)
                            If Not Effluente Is Nothing Then
                                Eff_Cod = Effluente.Eff_Cod
                                Eff_Des = Effluente.Eff_Des
                                Frequenza_Des = Effluente.Frequenza_Des
                                Udm_Cod = Effluente.Udm_Cod
                                Udm_Sim = Effluente.Udm_Sim
                                N_Riduzione = Effluente.N
                            End If
                        End If

                        DrLetPrec.Item("Eff_Cod") = Eff_Cod
                        DrLetPrec.Item("Eff_Des") = Eff_Des
                        DrLetPrec.Item("Id_Fre") = Id_Fre
                        DrLetPrec.Item("frequenza_des") = Frequenza_Des
                        DrLetPrec.Item("Udm_Cod") = Udm_Cod
                        DrLetPrec.Item("Udm_Sim") = Udm_Sim

                        DrLetPrec.Item("n_riduzione") = N_Riduzione

                        ExtraInt = xLetQdC.Rows(i).Item("extra_int")

                        If ExtraInt <> Udm_Cod Then
                            DrLetPrec.Item("qta") = AgronicaCoreMetaSchemaDAL.UnitaMisura_R.Converti(ExtraInt, xLetQdC.Rows(i).Item("qta"), Udm_Cod)
                        Else
                            DrLetPrec.Item("qta") = xLetQdC.Rows(i).Item("qta")
                        End If

                        DrLetPrec.Item("n") = xLetQdC.Rows(i).Item("n")

                        DrLetPrec.Item("n_residuo") = DrLetPrec.Item("qta") * DrLetPrec.Item("n") * N_Riduzione

                        xLetPrec.Rows.Add(DrLetPrec)
                    End If


                Next

            End If

            strEffluenti = JSON_DataTableLetamazioniPrecedentiQdC_Tabella(xLetPrec)

        Catch ex As Exception

            Return ex.Message

        End Try

        Return strEffluenti

    End Function

    Private Shared Function JSON_DataTableLetamazioniPrecedentiQdC_Tabella(ByRef DT_Effluenti As DataTable,
                                                                    Optional ByVal stringaKendoRow As String = "") As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        l.Add(New ColonneNome("id", "id", "number") With {._hidden = True})
        l.Add(New ColonneNome("data_movimento", "Data Fertilizzazione", "date"))
        l.Add(New ColonneNome("app_nome", "App.", "string"))
        l.Add(New ColonneNome("veg_des", "Specie Vegetale", "string"))
        l.Add(New ColonneNome("eff_cod", "eff_cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("eff_des", "Effluente", "string"))
        l.Add(New ColonneNome("udm_cod", "udm_cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("udm_sim", "Unita Misura [Ha]", "string") With {._css = "allineadestra", ._cssHeader = "allineadestra"})
        l.Add(New ColonneNome("qta", "Qta", "number") With {._formatNr = "n2", ._css = "allineadestra", ._cssHeader = "allineadestra"})
        l.Add(New ColonneNome("n", "Titolo [kg/Unita Misura]", "number") With {._formatNr = "n2", ._css = "allineadestra", ._cssHeader = "allineadestra"})
        l.Add(New ColonneNome("id_fre", "id_fre", "number") With {._hidden = True})
        l.Add(New ColonneNome("frequenza_des", "Frequenza", "string"))
        l.Add(New ColonneNome("n_riduzione", "Riduzione [%]", "number") With {._formatNr = "n2", ._css = "allineadestra", ._cssHeader = "allineadestra"})
        l.Add(New ColonneNome("n_residuo", "N Residuo [kg/Unita Misura]", "number") With {._formatNr = "n2", ._css = "allineadestra", ._cssHeader = "allineadestra", ._sum = True})

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        js.Editabile_Deafault = True

        Dim risp As String = js.JSON_DataTable_Kendo(DT_Effluenti, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.Menu,
                                           stringaKendoRow:=stringaKendoRow) 'True, True, TipoFiltroKendo_colonne.CasellaTesto) '

        Return risp

    End Function

End Class