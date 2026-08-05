Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreVarieBIZ

Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports System.Web.Services
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreUtility
Imports AgronicaCoreUtentiDAL
Imports AgroAgenda_2010.Resources
Imports AgronicaCoreModelsSTD.exceptions

Public Class GestioneEsercizi
    Inherits System.Web.UI.Page

    Dim objParametriAgenda As ParametriAgenda
    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public objparametri_server_string, objparametri_utenti_string As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Master.Lbl_Titolo.Text = "" 'AgronicaAgenda_2010.CreazioneEserciziPoliennali
        Master.flag_MostraHeader = False
        Master.flag_MostraFooter = False

        objParametriAgenda = New ParametriAgenda
        If objParametriAgenda.Piva <> "" Then
            hdPiva.Value = objParametriAgenda.Piva
        Else
            hdPiva.Value = Stringa_Decodifica(CStr(Request.QueryString("p")), AgroKey_EncoderDecoder, Server)
        End If

        inizializzoObjParametri()
        inizializzoParametriPagina()

    End Sub

    Private Sub inizializzoObjParametri()
        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objparametri_server_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Server)
        '---
    End Sub

    Private Sub inizializzoParametriPagina()

    End Sub

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub

    Public Shadows ReadOnly Property Master() As AgroAgenda_2010.AgendaBootstrap
        Get
            Return CType(MyBase.Master, AgroAgenda_2010.AgendaBootstrap)
        End Get
    End Property

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiAnagraficaEsercizi(ByVal piva As String, ByVal poliennali As Boolean, ByVal arboree As Boolean, ByVal filtroTemporale As Boolean) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim objParametriAgenda As New ParametriAgenda
            Dim FinestraTemporaleInizio = objParametri_Server.FinestraTemporaleInizio
            Dim FinestraTemporaleFine = objParametri_Server.FinestraTemporaleFine

            If filtroTemporale AndAlso objParametriAgenda.Data <> AGRODATAINIZIO Then
                objParametri_Server.FinestraTemporaleInizio = objParametriAgenda.Data.Date
                objParametri_Server.FinestraTemporaleFine = objParametriAgenda.Data.Date
            End If


            Dim gruppo_vegetale As Integer = If(arboree, 1, 0)
            Dim durata_esercizio As Integer = If(poliennali, 365, 0)

            ' filtro esercizi
            Dim filtroEsercizi As String = " a.Blk_Flag = 0 "
            If gruppo_vegetale <> 0 Then
                filtroEsercizi &= " AND s.Gru_Cod = " & gruppo_vegetale
            End If
            If durata_esercizio <> 0 Then
                filtroEsercizi &= " AND DATEDIFF(day,p.Validita_Inizio,p.Validita_Fine) > " & durata_esercizio
            End If

            Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim dt As DataTable = objImpianti.Leggi_x_GestioneEsercizi(piva, 0, 0, 0, 0, filtroEsercizi, "", objParametri_Server)

            Dim colSelected = New DataColumn With {
                .ColumnName = "Selected",
                .DataType = GetType(Boolean),
                .DefaultValue = False
            }

            dt.Columns.Add(colSelected)

            If objParametriAgenda.Impianti IsNot Nothing AndAlso objParametriAgenda.Impianti.Count > 0 Then

                For Each impianto In objParametriAgenda.Impianti
                    Dim drs = dt.Select("Progetto_Cod = " & impianto.Progetto_Cod & " ")
                    If drs.Count > 0 Then
                        drs(0)("Selected") = True
                    End If
                Next

            End If

            If filtroTemporale AndAlso objParametriAgenda.Data <> AGRODATAINIZIO Then
                objParametri_Server.FinestraTemporaleInizio = FinestraTemporaleInizio
                objParametri_Server.FinestraTemporaleFine = FinestraTemporaleFine
            End If

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function EseguiAzioneEsercizi(ByVal azione As String, ByVal dataChiusura As String, ByVal esercizi As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objDistinta As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
            Dim objImpreseProg As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_W
            Dim objEsercizi As JArray = JsonConvert.DeserializeObject(esercizi)
            Dim objAgenda As New AgronicaCoreContabDAL.Mov_Destinazioni_R
            Dim objReplica As New AgronicaCoreAnagrafeBIZ.Replica_GIAS

            Dim errore As String = ""
            Dim annualita = CInt(azione)

            AgronicaCoreUtility.DataOra.JarrayAggiustaDate(objEsercizi)

            For Each obj In objEsercizi

                Dim validita_fine As Date = If(dataChiusura = "", CDate(CStr(obj("validita_fine"))), CDate(dataChiusura))
                Dim dataUltimaChiusura As Date = If(annualita > 0, validita_fine.AddYears(annualita), validita_fine)

                Dim filtroDistinta As String = ""

                Dim strValiditaFineJarray As String = CStr(obj("validita_fine"))
                Dim dateValiditaFine As Date

                If String.IsNullOrEmpty(strValiditaFineJarray) Then
                    dateValiditaFine = AGRODATAFINE
                Else
                    dateValiditaFine = CDate(strValiditaFineJarray)
                End If

                If azione = "-1" Then
                    filtroDistinta = " Imprese_Progetti.Validita_Fine > " & Agro_SQL_SaveDate(dataUltimaChiusura)
                Else
                    filtroDistinta = " Imprese_Progetti.Progetto_Cod <> " & Agro_SQL_SaveNum(CInt(obj("progetto_cod"))) &
                        " AND Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(dataUltimaChiusura) &
                        " AND Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(dateValiditaFine)
                End If

                ' verifico se ci sono sovrapposizioni con esercizi esistenti
                Dim dtDistinta = objDistinta.LeggiDistinta(
                    CStr(obj("piva")), CInt(obj("sa_cod")), CInt(obj("appezza")), CInt(obj("id_reg")), "",
                    enumSelezioneVariabile.Selezione_TabellaCompleta, filtroDistinta, "", objParametri_Server)

                If dtDistinta.Rows.Count > 0 Then
                    errore = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/GestioneEsercizi.aspx",
                        "ImpossibileEseguireOperazioneSuEserciziConDateSovrapposteConAltri"), String)
                Else
                    Dim esistonoMovimenti As Boolean = False
                    Dim esistonoCdG As Boolean = False
                    ' verifico se ci sono movimenti di agenda successivi la data ultima chiusura
                    Dim dtAgenda = objAgenda.LeggiCronologiaMovimenti(
                        CStr(obj("piva")), CInt(obj("sa_cod")), CInt(obj("appezza")), CInt(obj("id_reg")),
                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                        " Movimenti.Data_Movimento > " & Agro_SQL_SaveDate(dataUltimaChiusura),
                        "", objParametri_Server)

                    If dtAgenda.Rows.Count > 0 Then
                        esistonoMovimenti = True
                        errore = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/GestioneEsercizi.aspx",
                            "ImpossibileEseguireInQuantoPresentiOperazioniAgendaSuccessiveADataChiusura"), String)
                        Exit For
                    End If

                    'COSTI DI GESTIONE
                    Dim objControllo As New AgronicaCoreAnagrafeBIZ.Progetto_W
                    Dim controllo = objControllo.controllo_CdG(Nothing, CStr(obj("piva")), CInt(obj("sa_cod")),
                                                               CInt(obj("appezza")), CInt(obj("id_reg")), CInt(obj("progetto_cod")),
                                                               AGRODATAINIZIO, validita_fine, objParametri_Server)
                    If controllo.errore = True Then
                        esistonoCdG = True
                        errore = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/GestioneEsercizi.aspx",
                            "ImpossibileEseguireInQuantoPresentiCdGSuccessiviADataChiusura"), String)
                        Exit For
                    End If

                    If esistonoCdG = False AndAlso esistonoMovimenti = False Then
                        Dim NoteLog As String = "Operazione effettuata da Modifica Multipla Singola Azienda"
                        objReplica.Replica_Esercizio(CStr(obj("piva")), CInt(obj("progetto_cod")), annualita, validita_fine, objParametri_Server, NoteLog)
                    End If
                End If
            Next

            If errore = "" Then
                r.RispostaStringa = ""
                r.RispostaOK = True
            Else
                r.RispostaStringa = "Impossibile proseguire con il salvataggio: <br>" & errore
                r.RispostaOK = False
            End If

        Catch ex As GiasException
            'Errori Gestiti
            r.RispostaStringa = "Impossibile proseguire con il salvataggio: <br>" & ex.Message
            r.RispostaOK = False
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

End Class