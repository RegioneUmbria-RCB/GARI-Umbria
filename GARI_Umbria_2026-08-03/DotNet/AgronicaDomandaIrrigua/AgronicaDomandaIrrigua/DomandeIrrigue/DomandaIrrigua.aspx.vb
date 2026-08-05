Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreModello.ParametriAgenda_Temp

Public Class DomandaIrrigua
    Inherits System.Web.UI.Page

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public objparametri_server_string, objparametri_utenti_string As String
    Dim objParametriAgenda As ParametriAgenda
    Dim qs_IdDomanda As Integer = -1

    Private Sub inizializzoObjParametri()
        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objparametri_server_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Server)
        '---
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        inizializzoObjParametri()

        'Controllo se l'utente ha i permessi per accedere
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim UtenteAbilitatoLettura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"),
                                            Session("ASG_IdServizio"),
                                            enum_Security_Attivita.DomandaIrrigua_Scheda,
                                            enum_Security_Operazione.Lettura,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)

        Dim UtenteAbilitatoScrittura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                           Session("ASG_Utente_Username"),
                                           Session("ASG_IdServizio"),
                                           enum_Security_Attivita.DomandaIrrigua_Scheda,
                                           enum_Security_Operazione.Modifica,
                                           Date.Now,
                                           "",
                                           objParametri_Utenti)

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

        If Not String.IsNullOrWhiteSpace(Request.QueryString("i")) Then
            qs_IdDomanda = CInt(Stringa_Decodifica(Request.QueryString("i").ToString, AgroKey_EncoderDecoder))
            hdIdDomanda.Value = qs_IdDomanda
        End If

        If Not String.IsNullOrWhiteSpace(Request.QueryString("d")) Then
            hdPaginaRedirect_Codificata.Value = Stringa_Decodifica(Request.QueryString("d").ToString, AgroKey_EncoderDecoder)
        End If


        If Not Page.IsPostBack Then
            If Not IsNothing(Session("ParametriDomandaIrrigua")) Then

                Dim objParametriDomandaIrrigua As New ParametriDomandaIrrigua
                objParametriDomandaIrrigua.Leggi()
                hdPiva.Value = objParametriDomandaIrrigua.Piva
            End If
            If objParametri_Server.FinestraTemporaleInizio <> AGRODATAINIZIO Then
                hdAnno.Value = objParametri_Server.FinestraTemporaleInizio.Year
            End If

            If UtenteAbilitatoScrittura = True Then
                hdEnableMod.Value = True
            End If

            If UtenteAbilitatoScrittura = False Or CheckDateLimit(New Date(Date.Now.Year, 5, 1), New Date(Date.Now.Year, 5, 1)) = False Then
                hdEnableMod.Value = False
            End If
        End If


    End Sub

    Private Shared Function CheckDateLimit(ByVal StartDate As Date, ByVal EndDate As Date) As Boolean
        Return True

        'If Date.Now < StartDate Or Date.Now > EndDate Then
        '    Return False
        'Else
        '    Return True
        'End If
    End Function

    Public Shadows ReadOnly Property Master() As AgronicaDomandaIrrigua.DomandaIrriguaBootstrap
        Get
            Return CType(MyBase.Master, AgronicaDomandaIrrigua.DomandaIrriguaBootstrap)
        End Get
    End Property

End Class