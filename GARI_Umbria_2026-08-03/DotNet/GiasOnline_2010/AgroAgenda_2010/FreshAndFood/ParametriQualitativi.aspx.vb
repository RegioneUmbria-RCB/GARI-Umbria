
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports System.Web.Services
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreAnagrafeDAL
Imports Newtonsoft.Json
Imports AgronicaCoreAnagrafeBIZ

Public Class ParametriQualitativi
    Inherits System.Web.UI.Page

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

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' TODO Autorizzazioni
        'Per ora lascio questo redirect (perchè non so ancora quali permessi utilizzare);
        'dopo in sviluppo togliere questo redirect e togliere il commento nel codice successivo
        Response.Redirect("~/classi/Agro_Pages/AccessoNonConsentito.aspx")

        inizializzoObjParametri()
        inizializzoParametriPagina()

        ' TODO Autorizzazioni
        'Controllo se l'utente ha i permessi per accedere
        'Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        'Dim UtenteAbilitatoLettura As Boolean = objPermessi.Controlla_Permessi_Utente(
        '                                    Session("ASG_Utente_Username"),
        '                                    Session("ASG_IdServizio"),
        '                                    enum_Security_Attivita.Consegne_Conferimento,
        '                                    enum_Security_Operazione.Lettura,
        '                                    Date.Now,
        '                                    "",
        '                                    objParametri_Utenti)

        'Dim UtenteAbilitatoScrittura As Boolean = objPermessi.Controlla_Permessi_Utente(
        '                                   Session("ASG_Utente_Username"),
        '                                   Session("ASG_IdServizio"),
        '                                   enum_Security_Attivita.Consegne_Conferimento,
        '                                   enum_Security_Operazione.Modifica,
        '                                   Date.Now,
        '                                   "",
        '                                   objParametri_Utenti)

        ''Imposto le variabili di ponte con il client
        'hf_UtenteAbilitatoLettura.Value = UtenteAbilitatoLettura
        'hf_UtenteAbilitatoScrittura.Value = UtenteAbilitatoScrittura

        'If UtenteAbilitatoLettura = False Then
        '    Response.Redirect("~/classi/Agro_Pages/AccessoNonConsentito.aspx")
        'End If

        'hdPiva.Value = Stringa_Decodifica(CStr(Request.QueryString("p")),
        '                                    AgroKey_EncoderDecoder,
        '                                    Server)


    End Sub

    Private Sub inizializzoParametriPagina()


    End Sub

    Public Shadows ReadOnly Property Master() As AgroAgenda_2010.AgendaBootstrap
        Get
            Return CType(MyBase.Master, AgroAgenda_2010.AgendaBootstrap)
        End Get
    End Property

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_ModuliAnagrafeAttivi(ByVal piva As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim objAnagrafeLog = New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R

            Dim ListModuliAnagrafe As New List(Of Object)

            Dim DT = objAnagrafeLog.Leggi_Join_Con_OGenerazioni_Anagrafe_Moduli(objParametri_Server.PivaSuperUser, piva, 0, "",
                                                                                "", "", objParametri_Server)

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then

                For Each rows In DT.Rows

                    ListModuliAnagrafe.Add(New With
                                            {
                                             .Modulo_Generazione = rows("Modulo_Generazione"),
                                             .Modulo_Descrizione = rows("Modulo_Descrizione"),
                                             .Modulo_Padre = False
                                            })

                Next

            Else
                ListModuliAnagrafe.Add(New With
                        {
                         .Modulo_Generazione = -Math.Abs(enum_Omni_Modulo_Generazione.Nessuno),
                         .Modulo_Descrizione = "Nessuno",
                         .Modulo_Padre = False
                        })
            End If

            Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa = JsonConvert.SerializeObject(ListModuliAnagrafe, Formatting.None, serializerSettings)

            r.RispostaOK = True


        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                    AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaGrigliaTestata_ParametriQualitativi(ByVal piva As String, ByVal modulo_generazione As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim objOModuli_Referenze_Config_Testata = New AgronicaCoreContabDAL.OModuli_Referenze_Config_Testata_R

            Dim XFiltroAggiuntivo = "Validita_Inizio <= " & AGRODATAFINE & " AND Validita_Fine >= " & AGRODATAINIZIO

            Dim XOrderBy = "Piva, Modulo_Generazione, Descrizione ASC"

            Dim DT = objOModuli_Referenze_Config_Testata.Leggi(piva, 0, modulo_generazione,
                                                               XFiltroAggiuntivo, XOrderBy, objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Formatting.None, serializerSettings)

            r.RispostaOK = True


        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                    AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function



    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaGrigliaDettagli_ParametriQualitativi(ByVal piva As String, ByVal modulo_generazione As Integer, ByVal id_testata As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim objOModuli_Referenze_Config_Testata = New AgronicaCoreContabDAL.OModuli_Referenze_Config_Testata_R

            Dim DT = objOModuli_Referenze_Config_Testata.LeggiDettagliTestata(piva, id_testata, "",
                                                                              modulo_generazione, 0,
                                                                              "", "", objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Formatting.None, serializerSettings)

            r.RispostaOK = True


        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                    AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaddlTabella(ByVal modulo_generazione As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim objOTabelle = New AgronicaCoreAnagrafeDAL.OTabelle_R

            'Non tabelle in consultazione
            Dim xFiltroAggiuntivo = "OTabelle.Tipo <> 2"

            Dim DT = objOTabelle.LeggiParametri("AAAAAAAAAAA", 0, modulo_generazione, xFiltroAggiuntivo, objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Formatting.None, serializerSettings)

            r.RispostaOK = True


        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                    AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

End Class

