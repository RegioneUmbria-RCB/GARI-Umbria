Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class ImputazioneImpianti
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
        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objparametri_server_string = Utility.convertOBJparametritoString(objParametri_Server)
        '---
    End Sub



    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Master.flag_pag_BeniConfezionamento = True

        inizializzoObjParametri()
        inizializzoParametriPagina()


        Dim PaginaRedirect As String = ""

        'Controllo se l'utente ha i permessi per accedere
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim UtenteAbilitatoLettura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"),
                                            Session("ASG_IdServizio"),
                                            enum_Security_Attivita.Inserimento_CostiRicavi_CdG,
                                            enum_Security_Operazione.Lettura,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)

        Dim UtenteAbilitatoScrittura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                           Session("ASG_Utente_Username"),
                                           Session("ASG_IdServizio"),
                                           enum_Security_Attivita.Inserimento_CostiRicavi_CdG,
                                           enum_Security_Operazione.Modifica,
                                           Date.Now,
                                           "",
                                           objParametri_Utenti)



        'Piva
        hdPiva.Value = Stringa_Decodifica(CStr(Request.QueryString("p")), AgroKey_EncoderDecoder)


        If hdPiva.Value = "" Then
            Response.Redirect("~/Menu/MenuBS_Agenda_Nuovo.aspx")
        End If

        hdPiva_Codificata.Value = Stringa_Codifica(hdPiva.Value, AgroKey_EncoderDecoder)


        'Pagina di origine
        hdPaginaRedirect.Value = ""
        hdPaginaRedirect_Codificata.Value = ""
        If Not Request.QueryString("origine") Is Nothing Then
            hdPaginaRedirect.Value = Stringa_Decodifica(CStr(Request.QueryString("origine")), AgroKey_EncoderDecoder)
        End If


        'If Not Request.QueryString("cod_risum_scarico") Is Nothing Then

        '    hdCod_Risum_Scarico.Value = CStr(Request.QueryString("cod_risum_scarico"))

        'End If



        If Not Request.QueryString("origine_nc") Is Nothing Then

            hdPaginaRedirect.Value = CStr(Request.QueryString("origine_nc"))

        End If

        If Not Request.QueryString("origine") Is Nothing OrElse
          Not Request.QueryString("origine_nc") Is Nothing Then
            hdPaginaRedirect_Codificata.Value = Stringa_Codifica(hdPaginaRedirect.Value, AgroKey_EncoderDecoder)
        End If

        Dim op As Integer = 0
        If Not Request.QueryString("op") Is Nothing Then

            op = Stringa_Decodifica(CStr(Request.QueryString("op")), AgroKey_EncoderDecoder)

            If op = 0 Then
                UtenteAbilitatoScrittura = False
            End If

        End If


        ''Cod_Contatto Scarico
        'hdCod_Contatto_Scarico.Value = ""
        'If Not Request.QueryString("cod_contatto_scarico") Is Nothing Then

        '    hdCod_Contatto_Scarico.Value = Stringa_Decodifica(CStr(Request.QueryString("cod_contatto_scarico")),
        '                                              AgroKey_EncoderDecoder)
        'End If




        'Imposto le variabili di ponte con il client
        hf_UtenteAbilitatoLettura.Value = UtenteAbilitatoLettura
        hf_UtenteAbilitatoScrittura.Value = UtenteAbilitatoScrittura

        If Not UtenteAbilitatoLettura Then
            If String.IsNullOrWhiteSpace(CStr(hdPaginaRedirect_Codificata.Value)) Then
                Response.Redirect("~/Menu/MenuBS_Agenda_Nuovo.aspx")
            Else
                Response.Redirect(CStr(hdPaginaRedirect.Value) & "?p=" & Request.QueryString("p"))
            End If
        End If

        Master().Lbl_Titolo.Text = "Imputazione Impianti Colturali"



        'If Not Page.IsPostBack Then


        '    If Not IsNothing(Session("ParametriAgenda_2010")) Then

        '        Dim objParametriAgenda_2010 As New ParametriAgenda_2010
        '        objParametriAgenda_2010.Leggi()

        '        'Impostazione Chiavi

        '        hdIdAgenda.Value = 73146

        '        If hdIdAgenda.Value <> 0 Then

        '            Dim DT As DataTable

        '            'Lettura Agenda
        '            DT = DocContabile_WS.Leggi_Agenda_BC(hdPiva.Value, hdIdAgenda.Value)

        '            If DT.Rows.Count > 0 Then

        '                For Each dr As DataRow In DT.Rows

        '                    hdLavCod.Value = dr("Lav_Cod")

        '                    Select Case dr("Cau_Mov")

        '                        Case CAU_REGISTRAZIONI

        '                            hdId_Mov_Testata.Value = dr("Id_Mov")

        '                        Case CAU_CARICO

        '                            hdId_Mov.Value = dr("Id_Mov")

        '                    End Select



        '                Next

        '            End If




        '            'Lettura Riferimento
        '            DT = DocContabile_WS.Leggi_Agenda_Riferimento_BC(hdPiva.Value, hdIdAgenda.Value)

        '            If DT.Rows.Count > 0 Then

        '                For Each dr As DataRow In DT.Rows


        '                    hdId_Agenda_Scarico.Value = dr("Id_Agenda")
        '                    hdLav_Cod_Scarico.Value = dr("Lav_Cod")

        '                    Select Case dr("Cau_Mov")

        '                        Case CAU_REGISTRAZIONI

        '                            hdId_Mov_Testata_Scarico.Value = dr("Id_Mov")

        '                        Case CAU_SCARICO

        '                            hdId_Mov_Scarico.Value = dr("Id_Mov")

        '                    End Select



        '                Next

        '            End If

        '        End If

        '    End If


        ' End If



    End Sub


    Private Sub inizializzoParametriPagina()


    End Sub


    Public Shadows ReadOnly Property Master() As AgroAgenda_2010.AgendaBootstrap
        Get
            Return CType(MyBase.Master, AgroAgenda_2010.AgendaBootstrap)
        End Get
    End Property

End Class