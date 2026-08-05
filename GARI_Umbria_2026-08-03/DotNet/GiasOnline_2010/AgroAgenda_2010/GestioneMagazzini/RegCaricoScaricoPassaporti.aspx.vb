Imports System.Web.Services
Imports AgronicaCoreContabBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreVarieBIZ

Public Class RegCaricoScaricoPassaporti
    Inherits System.Web.UI.Page

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub

    Public piva As String

#Region "Web service"

    <WebMethod(EnableSession:=True)>
    Public Shared Function ListaSpecieCultivar() As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If


        Try

            'Inserire il codice QUI..

            r.RispostaOK = True
            Dim Veg_Cod As Integer = 1
            r.RispostaStringa = CaricaSecieVarieta(Veg_Cod)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                       AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function

    ''' <summary>
    ''' Selezionato un Veg_Cod ne carica le varietà
    ''' </summary>
    ''' <param name="Veg_Cod"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function CaricaSecieVarieta(ByVal Veg_Cod As Integer) As String
        Dim objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim objVar As New AgronicaCoreMetaSchemaDAL.Cultivar_R
        Dim dt As DataTable
        dt = objVar.GestioneFiltroUtente_Leggi(0, Veg_Cod, "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                          "", "", objParametri_Utenti)
        Dim str_R As New List(Of String)
        Dim i As Integer
        For i = 0 To dt.Rows.Count - 1

            str_R.Add("{ ""Cul_COD"": " & dt.Rows(i).Item("Cul_Cod") & ", ""Veg_Des_Lat"": """ &
                      dt.Rows(i).Item("Veg_Des").ToLower & " " & dt.Rows(i).Item("Cul_Des").ToLower & """, ""Cultivar"": """ & dt.Rows(i).Item("Cul_Des").ToLower & """ }")


        Next

        Return "[" & String.Join(",", str_R) & "]"
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function PassaportoVivaiAggiornaDatiSrv(ByVal righeInserite As String, ByVal righeModificate As String, ByVal righeCancellate As String, byval RecuperaRigheRegistro As Boolean) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If


        Try

            'Inserire il codice QUI..


            Dim xAggiorna As New AgronicaCoreContabBIZ.ws_VivaiPassaporti_Operazioni_W
            Dim rval As String = xAggiorna.Aggiorna_ws_VivaiPassaporti_Operazioni(righeInserite, righeModificate, righeCancellate, RecuperaRigheRegistro, objParametri_Server)



            r.RispostaOK = True
            r.RispostaStringa = rval

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                       AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ConfermaVoceRegistro(piva As String, VivaiPassaporti_Operazione_cod As String, byval RecuperaRigheRegistro As Boolean) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim permessi As New PermessiUtente
            If Not permessi.getPermesso(TipiEnumerativi.enum_Security_Attivita.VivaiAttivitaAdempimenti).Scrittura Then
                Throw New Exception(DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/GestioneMagazzini/RegCaricoScaricoPassaporti.aspx", "RegCaricoScaricoPassaporti_Permessi"), String))
            End If

            Dim bConfermaVoce As New AgronicaCoreContabBIZ.ws_VivaiPassaportiOperazioniBiz
            r = bConfermaVoce.ConfermaVoceRegistro(piva, VivaiPassaporti_Operazione_cod, RecuperaRigheRegistro, objParametri_Server)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                       AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r

    End Function
    <WebMethod(EnableSession:=True)>
    Public Shared Function StampaPassaporto(piva As String, VivaiPassaporti_Operazione_cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim vVarStampe(4) As AgronicaCoreXML.XML_Stampe.ElementoStampe
            vVarStampe(0).Nome = "piva"
            vVarStampe(0).Valore = piva
            vVarStampe(1).Nome = "id_agenda"
            vVarStampe(1).Valore = VivaiPassaporti_Operazione_cod
            vVarStampe(2).Nome = "printcode"
            vVarStampe(2).Valore = 0
            vVarStampe(3).Nome = "printtoprinter"
            vVarStampe(3).Valore = 0
            vVarStampe(4).Nome = "printname"
            vVarStampe(4).Valore = ""

            Dim objVS As New AgronicaCoreXML.XML_Stampe
            Dim StrNodo As String = objVS.XML_VariabiliStampe(vVarStampe)
            Dim StrNodiVariabili As String = StrNodo
            Dim Report As Integer = TipiEnumerativi.enum_CodificaStampe.PassaportoMaterialeVivaistico

            Dim objAgronicaStampe As New AgronicaCoreGestioneRichieste.ParametriAgronicaStampe With {
                    .report = Report,
                    .username = CStr(HttpContext.Current.Session("ASG_Utente_Username")),
                    .user_profilo = CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS"))
                    }
            objAgronicaStampe.Xml_Generico.Length = 0
            objAgronicaStampe.Xml_Generico.Append(StrNodiVariabili)

            r.RispostaStringa = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(TipiEnumerativi.Enum_SiteRedirector.Sito_AgronicaStampe_2010, objAgronicaStampe)
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
    Public Shared Function LeggiAnnataAgraria() As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            'Inserire il codice QUI..

            r.RispostaOK = True

            Dim xAnnata As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim d1 As Date
            Dim d2 As Date

            xAnnata.AnnataAgraria(Now, d1, d2, objParametri_Utenti)

            r.RispostaStringa = "{ ""Txt_Data_DA"": """ & d1.ToShortDateString & """, ""Txt_Data_A"": """ & d2.ToShortDateString & """ }"

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                       AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function RegistroPassaportiLettura(ByVal piva As String, ByVal DataDa As String, ByVal DataA As String, ByVal RiportoDocumenti As Boolean, byval RecuperaRigheRegistro As Boolean) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim MessaggiRiportoDocumenti As String = ""
            Dim permessi As New PermessiUtente
            If Not permessi.getPermesso(TipiEnumerativi.enum_Security_Attivita.VivaiAttivitaAdempimenti).Scrittura Then
                Throw New Exception(DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/GestioneMagazzini/RegCaricoScaricoPassaporti.aspx", "RegCaricoScaricoPassaporti_Permessi"), String))
            End If

            If RiportoDocumenti Or RecuperaRigheRegistro Then
                Dim riportatore As New ws_VivaiPassaportiOperazioniBiz
                Dim rStdRiporto As RispostaStandard =
                        riportatore.RiportaDocumentiGias(RecuperaRigheRegistro, piva, objParametri_Server)

                If Not rStdRiporto.RispostaOK Then
                    MessaggiRiportoDocumenti = rStdRiporto.Errore
                End If
            End If

            Dim letturaRegistro As New AgronicaCoreContabDAL.VivaiPassaporti_Operazioni_R
                
            ' VAnni: 7/5/2020: ipotizzo di utilizzare il flag "inviato" per mostrare le azioni in fase di recupero
            '   quando mi trovo in modalità recupero, tutto il resto in modalità aggiorna.

            'If RecuperaRigheRegistro Then
            '    objParametri_Server.FlagVisibilita = AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
            'Else
            '    objParametri_Server.FlagVisibilita = AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
            'End If

            Dim dt As DataTable =
                    letturaRegistro.Leggi(piva, DataDa, DataA, "", " cast(op.DataMovimento as date) Desc, l.codiceRiga desc ", objParametri_Server)



            r.RispostaOK = True
            r.RispostaStringa = ws_VivaiPassaportiOperazioniBiz.CaricaGriglia_RegistroPassaporti_xJSON(dt, TipiEnumerativi.enum_PassaportiVivaiTipoGriglia.InserimentoDati)

            r.ParametroDue_stringa = MessaggiRiportoDocumenti

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = Resources.AgronicaAgenda_2010.SiEVerificatoUnErrore & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function


#End Region



    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim objParametriAgenda = New ParametriAgenda
        piva = objParametriAgenda.Piva

    End Sub

End Class