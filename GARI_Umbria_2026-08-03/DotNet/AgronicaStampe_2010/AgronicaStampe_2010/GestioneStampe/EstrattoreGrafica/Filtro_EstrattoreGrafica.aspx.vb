Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider


Partial Class Filtro_EstrattoreGrafica
    Inherits System.Web.UI.Page

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

    '==================================================
    '=====  Definizione Variabili Globali  ============
    '==================================================


    '----- Gestione Querystring
    Dim Qs_Piva As String
    Dim Qs_Sa_Cod As String
    Dim Qs_Sezione As String

    Dim Report As Integer

    Dim Matrice_Variabili(0, 0) As String

    'oggetto objparametri x server e utenti
    Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    '###########################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'Tolgo la pagina dalla cache
        Response.Expires = 0

        Dim objCentriAz As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

        Dim strXmlVariabilistampe As String
        Dim strErr As String
        Dim i As Integer

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XML_FiltroStampa As System.Xml.XmlElement
        Dim XMLs_VariabiliStampe As System.Xml.XmlNodeList
        Dim XML_VariabiliStampe As System.Xml.XmlElement


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

        '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

        Dim UtenteAbilitato As Boolean
        Dim strDummy As String      'controllo accesso negato.....

        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        UtenteAbilitato = objPermessi.Controlla_Permessi_Utente( _
                            Session("ASG_Utente_Username"), _
                            Session("ASG_IdServizio"), _
                            enum_Security_Attivita.Gest_Stampe, _
                            enum_Security_Operazione.Modifica , _
                            Date.Now, _
                            "", _
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
        '#####  Recupero Piva e Sa_Cod  
        '##############################################################

        strXmlVariabilistampe = Session("strXmlVariabilistampe")

        'Carico la stringa xml in un nuovo documento
        XmlDoc = New System.Xml.XmlDocument
        XmlDoc.LoadXml(strXmlVariabilistampe)

        If XmlDoc.HasChildNodes Then

            XML_FiltroStampa = XmlDoc.SelectSingleNode("ParametriAgronicaStampe_2010")

            'Ricavo i parametri che servono
            Session("ASG_Utente_Username") = XML_FiltroStampa.GetAttribute("username")

            Report = XML_FiltroStampa.GetAttribute("report")

            XMLs_VariabiliStampe = XML_FiltroStampa.GetElementsByTagName("VariabiliStampe")

            ReDim Matrice_Variabili(XMLs_VariabiliStampe.Count - 1, 2)

            For i = 0 To XMLs_VariabiliStampe.Count - 1

                XML_VariabiliStampe = XMLs_VariabiliStampe.Item(i)

                Matrice_Variabili(i, 0) = XML_VariabiliStampe.GetAttribute("piva")
                Matrice_Variabili(i, 1) = XML_VariabiliStampe.GetAttribute("sa_cod")

                'If Not CStr(XML_VariabiliStampe.GetAttribute("sezione")) Is Nothing Then
                '    Qs_Sezione = CStr(XML_VariabiliStampe.GetAttribute("sezione"))
                '    Matrice_Variabili(i, 5) = XML_VariabiliStampe.GetAttribute("sezione")
                'Else
                '    Matrice_Variabili(i, 5) = ""
                'End If

            Next

            Qs_Piva = Matrice_Variabili(0, 0)
            Qs_Sa_Cod = Matrice_Variabili(0, 1)


        End If

        '##############################################################
        '#####  Verifico se sono in Post-Back  ########################
        '##############################################################

        If Not Page.IsPostBack Then

            'output.Write("Page has just been loaded")

            'la prima volta che carico la pagina la metto in primo piano
            '(in caso contrario rimane in primo piano la pagina del GiasOnline)
            Dim strFocus As String = "<script language='javascript'> window.focus() </script>"
            Me.FindControl("Form1").Controls.Add(New LiteralControl(strFocus))

            AgronicaControlliGIS.CaricaListControl.Gis_LayerElementiGrafici(Me.CBL_Layers, False, "", "", " TipologiaLayer_cod = 1 ", "", objParametri_Server)

            If Qs_Sezione = "" Then
                Me.ImgBtnSelezionaTutte_Click(Me, Nothing)
            Else
                For i = 0 To Me.CBL_Layers.Items.Count - 1

                    If Me.CBL_Layers.Items(i).Value = Qs_Sezione Then
                        Me.CBL_Layers.Items(i).Selected = True
                    Else
                        Me.CBL_Layers.Items(i).Selected = False
                    End If

                Next

            End If

        Else

            'output.Write("Postback has occured")
            Exit Sub

        End If


        '##############################################################
        '#####  CARICO I DATI  ########################################
        '##############################################################


        '----- Imposto la data di stampa

        Dim Mese As Integer

        Me.rblStampa.SelectedItem.Value = 1

        Me.Pannello_Giorno.Visible = False
        Me.Pannello_Intervallo.Visible = True

        'di default imposto le date dell'intervallo coincidenti con l'annata agraria 
        'dal 1/11 al 31/10

        Mese = Now.Month

        Select Case Mese
            Case 11, 12
                'se sono nei mesi di novembre o dicembre..........
                'l'annata agraria va dal 1/11 di quest'anno al 31/10 del prossimo
                Me.TxtValiditaInizio.Text = "01/11/" & Now.Year
                Me.TxtValiditaFine.Text = "31/10/" & Now.Year + 1

            Case Else
                'l'annata agraria va dal 1/11 dell'anno scorso al 31/10 di quest'anno
                Me.TxtValiditaInizio.Text = "01/11/" & Now.Year - 1
                Me.TxtValiditaFine.Text = "31/10/" & Now.Year

        End Select


        Me.Lbl_Piva.Text = Qs_Piva
        Me.Lbl_Centro.Text = objCentriAz.SaNome_from_SaCod(Qs_Piva, Qs_Sa_Cod, objParametri_Server)
        'Me.Lbl_Impresa.Text = RagSoc_from_Piva(Qs_Piva, Server)


        '----- Tabella Imprese

        'Creo gli oggetti COM+
        Dim Imprese_Read As New AgronicaCoreAnagrafeDAL.Imprese_Read 'New Agro_Anagrafe_AD.Imprese_Read
        Dim DTImprese As DataTable

        'Leggo le informazioni sull'impresa selezionata	
        'RsImprese = objImprese.Leggi(CStr(Qs_Piva), _
        '                    , , , , _
        '                    CStr(Session("ASG_Connessione_Server")
        DTImprese = Imprese_Read.Leggi(CStr(Qs_Piva), AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

        'Se il recordset non e' nullo
        If Not IsNothing(DTImprese) AndAlso DTImprese.Rows.Count <> 0 Then

            Me.Lbl_Impresa.Text = DTImprese.Rows(0).Item("rag_soc")

            If DTImprese.Rows(0).Item("Validita_Inizio") = "01/01/1900" Then
                Lbl_ValiditaInizio.Text = "  ......  "
            Else
                Lbl_ValiditaInizio.Text = DTImprese.Rows(0).Item("Validita_Inizio")
            End If

            If DTImprese.Rows(0).Item("Validita_Fine") = "31/12/2100" Then
                Lbl_ValiditaFine.Text = "  ......  "
            Else
                Lbl_ValiditaFine.Text = DTImprese.Rows(0).Item("Validita_Fine")

            End If
        End If
    End Sub

    '###########################################################################
    Private Sub rblStampa_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rblStampa.SelectedIndexChanged

        Dim Mese As Integer

        If Me.rblStampa.SelectedItem.Value = 0 Then

            Me.Pannello_Giorno.Visible = True
            Me.Pannello_Intervallo.Visible = False

            Me.TxtStampa.Text = Now.Today

        Else

            Me.Pannello_Giorno.Visible = False
            Me.Pannello_Intervallo.Visible = True

            'di default imposto le date dell'intervallo coincidenti con l'annata agraria 
            'dal 1/11 al 31/10

            Mese = Now.Month

            Select Case Mese
                Case 11, 12
                    'se sono nei mesi di novembre o dicembre..........
                    'l'annata agraria va dal 1/11 di quest'anno al 31/10 del prossimo
                    Me.TxtValiditaInizio.Text = "01/11/" & Now.Year
                    Me.TxtValiditaFine.Text = "31/10/" & Now.Year + 1

                Case Else
                    'l'annata agraria va dal 1/11 dell'anno scorso al 31/10 di quest'anno
                    Me.TxtValiditaInizio.Text = "01/11/" & Now.Year - 1
                    Me.TxtValiditaFine.Text = "31/10/" & Now.Year

            End Select

        End If

    End Sub

    '###########################################################################
    Private Sub ImgBtn_Stampa_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Stampa.Click

        Dim TargetURL As String
        Dim DataInizio As Date
        Dim DataFine As Date
        Dim DataStampa As Date
        Dim str_DataInizio As String
        Dim str_DataFine As String
        Dim str_DataStampa As String
        Dim Anno As Integer
        Dim i As Integer
        Dim str_Cod_Layers As String
        Dim str_Des_Layers As String
        Dim Num_Layers As Integer

        '----- Verifico i dati

        'Se seleziono un intervallo passo alla scheda di campagna le due date scelte
        If Me.rblStampa.SelectedItem.Value = 1 Then

            If Me.TxtValiditaInizio.Text = "" Then
                AgroMsgBox("Impostare una data iniziale di riferimento per la stampa ...", Page)
                Exit Sub
            Else
                DataInizio = CDate(Me.TxtValiditaInizio.Text)
            End If

            If Me.TxtValiditaFine.Text = "" Then
                AgroMsgBox("Impostare una data finale di riferimento per la stampa ...", Page)
                Exit Sub
            Else
                DataFine = CDate(Me.TxtValiditaFine.Text)
            End If

            str_DataInizio = Format(DataInizio, "dd/MM/yyyy")
            str_DataFine = Format(DataFine, "dd/MM/yyyy")
            str_DataStampa = ""

            'altrimenti passo alla scheda di campagna solo la data scelta
        Else

            If Me.TxtStampa.Text = "" Then
                AgroMsgBox("Impostare una data iniziale di riferimento per la stampa ...", Page)
                Exit Sub
            Else
                DataStampa = CDate(Me.TxtStampa.Text)
            End If

            str_DataInizio = ""
            str_DataFine = ""
            str_DataStampa = Format(DataStampa, "dd/MM/yyyy")

        End If

        TargetURL = "EstrattoreGrafica_XLS.aspx" & _
                            "?dI=" & _
                            Stringa_Codifica(str_DataInizio, AgroKey_EncoderDecoder, Server) & _
                            "&dF=" & _
                            Stringa_Codifica(str_DataFine, AgroKey_EncoderDecoder, Server) & _
                            "&dG=" & _
                            Stringa_Codifica(str_DataStampa, AgroKey_EncoderDecoder, Server)


        'Salvo in una var di sessione i dati selezionati
        'se ho selezionato il sottoreport metto il num a cui equivale
        'altrimenti metto 0
        For i = 0 To Me.CBL_Layers.Items.Count - 1
            If Me.CBL_Layers.Items(i).Selected Then
                str_Cod_Layers = str_Cod_Layers & CBL_Layers.Items(i).Value & ","
                str_Des_Layers = str_Des_Layers & CBL_Layers.Items(i).Text & ", "
                Num_Layers += 1
            End If
        Next

        'controllo che sia selezionata almeno una sezione
        If Num_Layers = 0 Then
            AgroMsgBox("Selezionare almeno un Layer!", Page)
            Exit Sub
        End If

        'tolgo l'ultima virgola
        If str_Cod_Layers <> "" Then
            str_Cod_Layers = Left(str_Cod_Layers, str_Cod_Layers.Length - 1)
            str_Des_Layers = Left(str_Des_Layers, str_Des_Layers.Length - 2)
        End If

        Session("Cod_Layers") = str_Cod_Layers

        Session("Des_Layers") = str_Des_Layers

        Response.Redirect(TargetURL)


    End Sub


    '########################################################################################
    Private Sub ImgBtnSelezionaTutte_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtnSelezionaTutte.Click

        Dim i As Integer

        For i = 0 To Me.CBL_Layers.Items.Count - 1

            'seleziono tutte le righe
            Me.CBL_Layers.Items(i).Selected = True

        Next


    End Sub

    '########################################################################################
    Private Sub ImgBtnEliminaSelezione_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtnEliminaSelezione.Click

        Dim i As Integer

        For i = 0 To Me.CBL_Layers.Items.Count - 1

            'Deseleziono tutte le righe
            Me.CBL_Layers.Items(i).Selected = False

        Next


    End Sub

    '###########################################################################
    Private Sub ImgBtnAnnulla_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtnAnnulla.Click

        Dim strClose As String = "<script language='javascript'> window.close() </script>"
        Me.FindControl("Form1").Controls.Add(New LiteralControl(strClose))

    End Sub

End Class
