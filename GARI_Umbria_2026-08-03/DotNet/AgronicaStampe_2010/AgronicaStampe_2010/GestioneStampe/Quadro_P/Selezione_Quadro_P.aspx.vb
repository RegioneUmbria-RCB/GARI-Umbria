Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza

Public Class Selezione_Quadro_P
    Inherits System.Web.UI.Page

    Dim Qs_Piva As String
    Dim Qs_Sa_Cod As String

    'oggetto objparametri x server e utenti
    Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    '######################################################################################################
    Private Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto

    End Sub



    '############################################################################################################################
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        'Tolgo la pagina dalla cache
        Response.Expires = 0

        '##############################################################
        '############  Lettura Parametri Query String #################
        '##############################################################

        Qs_Piva = Stringa_Decodifica(CStr(Request.QueryString("p")), _
                                    AgroKey_EncoderDecoder, _
                                    Server)

        Qs_Sa_Cod = "0"
        If Not IsNothing(CStr(Request.QueryString("s"))) AndAlso IsNumeric(Stringa_Decodifica(CStr(Request.QueryString("s")), _
                                    AgroKey_EncoderDecoder, _
                                    Server)) Then
            Qs_Sa_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("s")), _
                                        AgroKey_EncoderDecoder, _
                                        Server))
        End If



        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato

        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If

        'inizializzazione oggetti objParametri_Utenti e objParametri_Server
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        '------------



        '##############################################################
        '#####  Recupero Utente, piva e sa_cod  dalla stringa xml
        '##############################################################

        Dim strXmlVariabilistampe As String
        Dim htVariabiliStampe As System.Collections.Hashtable
        Dim strErr As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XML_FiltroStampa As System.Xml.XmlElement


        strXmlVariabilistampe = Session("strXmlVariabilistampe")

        'Carico la stringa xml in un nuovo documento
        XmlDoc = New System.Xml.XmlDocument
        XmlDoc.LoadXml(strXmlVariabilistampe)

        If XmlDoc.HasChildNodes Then

            ' XML_FiltroStampa = XmlDoc.SelectSingleNode("FiltroStampa")


            If Not IsNothing(XmlDoc.SelectSingleNode("ParametriAgronicaStampe_2010")) Then

                XML_FiltroStampa = XmlDoc.SelectSingleNode("ParametriAgronicaStampe_2010")

            Else

                If Not IsNothing(XmlDoc.SelectSingleNode("FiltroStampa")) Then
                    XML_FiltroStampa = XmlDoc.SelectSingleNode("FiltroStampa")
                Else
                    Throw New Exception("FiltroStampa era usato nelle stampe vecchie ora i parametri sono in ParametriAgronicaStampe_2010 ma non c'è nulla :-( ")
                End If
            End If

            'Ricavo i parametri che servono

            Session("ASG_Utente_Username") = XML_FiltroStampa.GetAttribute("username")
            strXmlVariabilistampe = Session("strXmlVariabilistampe")


        End If



        '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

        Dim UtenteAbilitato As Boolean
        Dim strDummy As String      'controllo accesso negato.....

        Dim objUtentiDal As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        UtenteAbilitato = objUtentiDal.Controlla_Permessi_Utente(Session("ASG_Utente_Username"), _
                                                                         Session("ASG_IdServizio"), _
                                                                         enum_Security_Attivita.Gest_Stampe, _
                                                                         enum_Security_Operazione.Lettura, _
                                                                         Now.Date, _
                                                                         "",
                                                                         objParametri_Utenti)

        '----- Se l'utente non ha il permesso per visualizzare la pagina ... lo invio al menu.



        '----- !!!!!!!!!!! -------------

        'Attivazione forzata provvisoria

        UtenteAbilitato = True

        '----- !!!!!!!!!!! -------------

        If UtenteAbilitato = False Then
            Response.Redirect("../../Messaggi/AccessoNegato.htm")
        End If

        '=====================================================


        '##############################################################
        '#####  Verifico se sono in Post-Back  ########################
        '##############################################################

        If Not Page.IsPostBack Then
            'output.Write("Page has just been loaded")

            'la prima volta che carico la pagina la metto in primo piano
            '(in caso contrario rimane in primo piano la pagina del GiasOnline)
            ' Dim strFocus As String = "<script language='javascript'> window.focus() </script>"
            ' Me.FindControl("Form1").Controls.Add(New LiteralControl(strFocus))

        Else
            'output.Write("Postback has occured")
            Exit Sub
        End If

        '##############################################################
        '#####  CARICO I DATI  ########################################
        '##############################################################

        '----- Imposto la data di stampa
        TxtValiditaInizio.Text = Now.Today.ToShortDateString

        Row_Piva.InnerHtml = "<b>Piva: <font color='#0000FF'>" & Qs_Piva & "</font></b>"

        Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Row_RagSoc.InnerHtml = "<b>Ragione sociale: <font color='red'>" & objImprese.RagSoc_from_Piva(Qs_Piva, objParametri_Server) & "</font></b>"
        objImprese = Nothing

        If Not Qs_Sa_Cod Is Nothing AndAlso Qs_Sa_Cod <> "0" Then

            Dim objCentriAz As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
            Row_CentroAziendale.InnerHtml = "<b>In particolare per il Centro Aziendale: </b>" & objCentriAz.SaNome_from_SaCod(Qs_Piva, Qs_Sa_Cod, objParametri_Server)
            objCentriAz = Nothing

            '----- Tabella Centri_Aziendali

            Dim CentriAziendali_Read As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read 'New Agro_Anagrafe_AD.CentriAziendali_Read
            Dim RsCentri As DataTable

            RsCentri = CentriAziendali_Read.Leggi(CStr(Qs_Piva), _
                           CInt(Qs_Sa_Cod), _
                           AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", _
                           objParametri_Server)

            'Se il recordset non e' nullo
            If Not IsNothing(RsCentri) AndAlso RsCentri.Rows.Count <> 0 Then

                If RsCentri.Rows(0).Item("Validita_Inizio") = "01/01/1900" Then
                    Row_InizioAttivita.InnerHtml = "<b>Inizio Attività:<font color='#0000FF'>  ......  </font></b>"
                Else
                    Row_InizioAttivita.InnerHtml = "<b>Inizio Attività:<font color='#0000FF'> " & RsCentri.Rows(0).Item("Validita_Inizio") & "</font></b>"
                End If

                If RsCentri.Rows(0).Item("Validita_Fine") = "31/12/2100" Then
                    Row_FineAttivita.InnerHtml = "<b>Fine Attività:<font color='#0000FF'>  ......  </font></b>"
                Else
                    Row_FineAttivita.InnerHtml = "<b>Fine Attività:<font color='#0000FF'> " & RsCentri.Rows(0).Item("Validita_Fine") & "</font></b>"
                End If


            End If

        Else

            '----- Tabella Imprese

            'Creo gli oggetti COM+
            Dim Imprese_Read As New AgronicaCoreAnagrafeDAL.Imprese_Read 'New Agro_Anagrafe_AD.Imprese_Read
            Dim DTImprese As DataTable

            Row_CentroAziendale.InnerHtml = ""

            Row_NoteCentro.InnerHtml = "Si tenga presente che il periodo di attivita' di tale impresa e' impostato come:"

            DTImprese = Imprese_Read.Leggi(CStr(Qs_Piva), AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

            If Not IsNothing(DTImprese) AndAlso DTImprese.Rows.Count <> 0 Then

                If DTImprese.Rows(0).Item("Validita_Inizio") = "01/01/1900" Then
                    Row_InizioAttivita.InnerHtml = "<b>Inizio Attività:<font color='#0000FF'>  ......  </font></b>"
                Else
                    Row_InizioAttivita.InnerHtml = "<b>Inizio Attività:<font color='#0000FF'> " & DTImprese.Rows(0).Item("Validita_Inizio") & "</font></b>"
                End If

                If DTImprese.Rows(0).Item("Validita_Fine") = "31/12/2100" Then
                    Row_FineAttivita.InnerHtml = "<b>Fine Attività:<font color='#0000FF'>  ......  </font></b>"
                Else
                    Row_FineAttivita.InnerHtml = "<b>Fine Attività:<font color='#0000FF'> " & DTImprese.Rows(0).Item("Validita_Fine") & "</font></b>"
                End If
            End If


        End If

    End Sub


    '############################################################################################################################
    Private Sub ImgBtn_Stampa_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Stampa.Click


        Dim TargetURL As String
        Dim DataStampa As Date
        Dim str_DataStampa As String
        Dim Flag_Centro As Integer = 0
        Dim Flag_SoloOccupate As Integer = 0

        '----- Verifico i dati

        If Me.TxtValiditaInizio.Text = "" Then
            MsgBox("Impostare una data di riferimento per la stampa ...")
            Exit Sub
        Else
            DataStampa = CDate(Me.TxtValiditaInizio.Text)
        End If

        If Chk_Centro.Checked = True Then
            Flag_Centro = 1
        End If

        If Chk_SoloOccupate.Checked = True Then
            Flag_SoloOccupate = 1
        End If

        '----- Data di Stampa

        str_DataStampa = Format(DataStampa, "dd/MM/yyyy")

        '----- Preparo il link

        TargetURL = "Quadro_P.aspx" & _
                    "?ds=" & _
                    Stringa_Codifica(str_DataStampa, AgroKey_EncoderDecoder, Server) & _
                    "&p=" & _
                    Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) & _
                    "&s=" & _
                    Stringa_Codifica(Qs_Sa_Cod, AgroKey_EncoderDecoder, Server) & _
                    "&c=" & _
                    Stringa_Codifica(Flag_Centro, AgroKey_EncoderDecoder, Server) & _
                    "&so=" & _
                    Stringa_Codifica(Flag_SoloOccupate, AgroKey_EncoderDecoder, Server)

        Response.Redirect(TargetURL)


    End Sub

    ' ##################################################################################################
    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim strJS1 As New StringBuilder
        strJS1.AppendLine("$(document).ready(function () { ")
        strJS1.AppendLine("      window.close() ")
        strJS1.AppendLine(" });")

        ScriptManager.RegisterStartupScript(Me.Page, Me.Page.GetType(),
                                  String.Format("jQuery_{0}", Me.Page.ClientID), strJS1.ToString, True)

    End Sub

End Class