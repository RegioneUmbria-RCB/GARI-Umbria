Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class Filtro_SceltaStampa
    Inherits System.Web.UI.Page

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim Report As Integer
    Dim Qs_Piva As String

    '##################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'Faccio scadere subito la pagina memorizzata nella cache
        Response.Expires = 0

        Try

            '##############################################################
            '#####  Verifico Credenziali di Accesso  ######################
            '##############################################################

            '----- Verifico che l'utente sia autenticato
            If Session("ASG_objParametri_Server") Is Nothing Then
                Response.Redirect("~/Custom500.aspx")
            End If

            '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina
            Dim UtenteAbilitato As Boolean
            '  Dim strDummy As String      'controllo accesso negato.....

            'UtenteAbilitato = Controlla_Permessi_Utente_2( _
            '                            Server, Session, Page, _
            '                            Session("ASG_Utente_Username"), _
            '                            Session("ASG_IdServizio"), _
            '                            enum_Security_Attivita.Stampe_Contabilita, _
            '                            enum_Security_Operazione.Lettura, _
            '                            strDummy)


            '----- !!!!!!!!!!! -------------
            'Attivazione forzata provvisoria
            UtenteAbilitato = True
            '----- !!!!!!!!!!! -------------

            '----- Se l'utente non ha il permesso per visualizzare la pagina ... lo invio ad AlberoImprese.

            If UtenteAbilitato = False Then
                Dim strClose As String = "<script language='javascript'> window.close() </script>"
                Me.FindControl("Form1").Controls.Add(New LiteralControl(strClose))
                Exit Sub
            End If


            '##############################################################
            '###################### QUERYSTRING ###########################
            '##############################################################

            Report = Stringa_Decodifica(Request.QueryString("rep").ToString, _
                                   AgroKey_EncoderDecoder, _
                                   Server)

            Qs_Piva = Stringa_Decodifica(Request.QueryString("p").ToString, _
                           AgroKey_EncoderDecoder, _
                           Server)

            'Select Case Report

            '    Case enum_CodificaStampe.Fatture


            'End Select


            If IsNothing(Session("ASG_objParametri_Server")) Then
                'AgroMsgBox("Sessione scaduta", Page)
                Dim strClose As String = "<script language='javascript'> window.close() </script>"
                Me.FindControl("Form1").Controls.Add(New LiteralControl(strClose))
                Exit Sub
            End If

            objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

            If Me.IsPostBack Then
                Exit Sub
            End If


            'caricamento rbl con valori della tabella configurazione_stampe epr quel codice report




        Catch ex As Exception
            AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("PageLoad: " & ex.Message, Page)
        End Try


    End Sub


    Private Sub Stampa()

        Dim TargetURL As String
        Dim Qs_Aggiunta As String

        'in base alla scelta su radiobuttonlist
        'leggere su configurazione_stampe gli altri campi x capire cosa bisogna fare

        ' TargetURL = valore letto dalla tabella


        'Qs_Aggiunta da gestire sui campi letti e sulla stampa

        Dim Querystring As String = Request.QueryString().ToString

        If TargetURL <> String.Empty Then
            Response.Redirect(TargetURL & Querystring & Qs_Aggiunta)

        End If





    End Sub



End Class