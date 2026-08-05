

Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreSementieriBIZ.Carica_Controlli
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Agronica.Helpers.GiasBase
Public Class GST_Filtro_Impianti
    Inherits System.Web.UI.Page

    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri

    'Public ReadOnly Property PATH_GIASBASE As String
    '    Get
    '        Return Me.Master.PATH_GIASBASE
    '    End Get
    'End Property

    Private PATH_GIASBASE As String = ""

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        If Session.IsNewSession Then
            Response.Redirect("../GST_Autenticazione/Autenticazione.aspx")
        End If

        Me.Master.ImpostaVisibilitaPulsantiMaster(TipiEnumerativiSementieri.enum_Pannelli.FILTRO)

        '----- Dimensiono le variabili
        objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")
        objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")

        GiasBaseHelper.Setta_Link_GiasBase("ASG_objParametri_Server", PATH_GIASBASE)

        '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim UtenteAbilitato As Boolean = objPermessi.Controlla_Permessi_Utente(
            objParametri_Utenti.UtenteUsername,
            5,
            TipiEnumerativi.enum_Security_Attivita.Interferenze_Visualizzazione_Ridotta,
            TipiEnumerativi.enum_Security_Operazione.Lettura,
            Date.Now,
            "",
            objParametri_Utenti)

        If Not UtenteAbilitato Then

            UtenteAbilitato = objPermessi.Controlla_Permessi_Utente(
                objParametri_Utenti.UtenteUsername,
                5,
                TipiEnumerativi.enum_Security_Attivita.Interferenze_Visualizzazione_Estesa,
                TipiEnumerativi.enum_Security_Operazione.Lettura,
                Date.Now,
                "",
                objParametri_Utenti)
        End If

        '----- Se l'utente non ha il permesso per visualizzare la pagina ... 
        If Not UtenteAbilitato Then

            Response.Redirect("../GST_Menu/Menu.aspx")
        End If

        '##############################################################
        '#####  Verifico se sono in Post-Back  ########################
        '##############################################################

        If Page.IsPostBack Then

            Exit Sub
        End If

        '##############################################################
        '#####  Inizializzo la Pagina  ################################
        '##############################################################

        Txt_DataInizioImpianto.Text = ""
        Txt_DataFineImpianto.Text = ""

        Call Imposta_Pannelli(TipiEnumerativiSementieri.enum_Pannelli.FILTRO)

        Call Carica_RBL_Sementi(RBL_Sementi, objParametri_Server)

        Call Carica_CBL_SpecieVegetali_Light(CBL_SpecieVegetali, objParametri_Server)

        Call Carica_CBL_PadriGerarchia(CBL_ImpreseReferenti, objParametri_Server)

        Call Imposta_Tutti_Check(CBL_ImpreseReferenti, True)

        '----------------------------------------

        Dim UtenteAmministratore As Boolean = objPermessi.Controlla_Permessi_Utente(
            objParametri_Utenti.UtenteUsername,
            5,
            TipiEnumerativi.enum_Security_Attivita.Interferenze_Visualizzazione_Estesa,
            TipiEnumerativi.enum_Security_Operazione.Lettura,
            Date.Now,
            "",
            objParametri_Utenti)

        Dim sementi As String = Session("Sementi").ToString.Split("|")(0)
        RBL_Sementi.SelectedValue = sementi

        Call RBL_Sementi_SelectedIndexChanged(Me, Nothing)

        ImgBtn_Imprese_Seleziona.ImageUrl = PATH_GIASBASE + "agronica/AB_Immagini/icone24/ValidazioneSI_24.ico"
        ImgBtn_Imprese_DeSeleziona.ImageUrl = PATH_GIASBASE + "agronica/AB_Immagini/icone24/ValidazioneNO_24.ico"
        ImgBtn_Specie_DeSeleziona.ImageUrl = PATH_GIASBASE + "agronica/AB_Immagini/icone24/ValidazioneNO_24.ico"

    End Sub


    Private Sub Imposta_Pannelli(ByVal Pannello As TipiEnumerativiSementieri.enum_Pannelli)

        Pannello_Risultato.Visible = (Pannello = TipiEnumerativiSementieri.enum_Pannelli.RISULTATO)

        Me.Master.ImpostaVisibilitaPulsantiMaster(Pannello)
    End Sub


    Private Sub RBL_Sementi_SelectedIndexChanged(sender As Object, e As EventArgs) Handles RBL_Sementi.SelectedIndexChanged

        Call Imposta_Tutti_Check(CBL_SpecieVegetali, False)

        Dim ID_Specie As Integer = RBL_Sementi.SelectedValue

        Dim leggi As New AgronicaCoreSementieriDAL.Varie_R
        Dim DT As DataTable = leggi.Leggi_VegCod_Da_MappaturaSpecie(ID_Specie, objParametri_Server)

        Dim idx As Integer
        Dim cnt As Integer = CBL_SpecieVegetali.Items.Count
        For Each drow As DataRow In DT.Rows
            idx = 0
            While idx < cnt
                If CBL_SpecieVegetali.Items(idx).Value = drow("Veg_Cod") Then
                    CBL_SpecieVegetali.Items(idx).Selected = True
                    idx = cnt
                End If
                idx += 1
            End While
        Next

        'ViewState("bietola") = (ID_Specie = 1 OrElse ID_Specie = 21)

        Call CBL_SpecieVegetali_SelectedIndexChanged(Me, Nothing)


        If ID_Specie = 21 Then

            If Txt_DataInizioImpianto.Text = "" Then

                Dim DataSoglia As Date
                Dim InizioAnnataCorrente As Date

                DataSoglia = CDate("01/09/" & Date.Today.Year)

                If Date.Today < DataSoglia Then
                    '===> 01 gennaio - 31 agosto
                    InizioAnnataCorrente = CDate("01/09/" & (Date.Today.Year - 1).ToString)
                Else
                    '===> 01 settembre - 31 dicembre
                    InizioAnnataCorrente = CDate("01/09/" & Date.Today.Year)
                End If

                Txt_DataInizioImpianto.Text = InizioAnnataCorrente.ToShortDateString
                Opt_Inizio_SuccessivoAl.Checked = True
            End If

        End If

    End Sub


    Protected Sub CBL_SpecieVegetali_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles CBL_SpecieVegetali.SelectedIndexChanged

        Dim i As Integer
        Dim NumeroSelezionati As Integer = 0
        Dim Veg_Cod As Integer

        For i = 0 To CBL_SpecieVegetali.Items.Count - 1
            If CBL_SpecieVegetali.Items(i).Selected = True Then
                NumeroSelezionati += 1
                Veg_Cod = CBL_SpecieVegetali.Items(i).Value
            End If
        Next
    End Sub


    Private Sub Imposta_Tutti_Check(ByRef CBL As CheckBoxList, ByVal Selezionato As Boolean)
        Dim i As Integer
        For i = 0 To CBL.Items.Count - 1
            CBL.Items(i).Selected = Selezionato
        Next
    End Sub


    Protected Sub ImgBtn_Imprese_Seleziona_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Imprese_Seleziona.Click
        Call Imposta_Tutti_Check(CBL_ImpreseReferenti, True)
    End Sub


    Protected Sub ImgBtn_Imprese_DeSeleziona_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Imprese_DeSeleziona.Click
        Call Imposta_Tutti_Check(CBL_ImpreseReferenti, False)
    End Sub


    Protected Sub ImgBtn_Specie_DeSeleziona_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Specie_DeSeleziona.Click

        Dim i As Integer
        For i = 0 To RBL_Sementi.Items.Count - 1
            RBL_Sementi.Items(i).Selected = False
        Next

        Call Imposta_Tutti_Check(CBL_SpecieVegetali, False)
        Call CBL_SpecieVegetali_SelectedIndexChanged(Me, Nothing)

    End Sub

    Protected Sub ImgBtn_Filtro_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)

        '--- Ripulisco il dataset dei risultati

        '--- Attivo il pannello del FILTRO
        Call Imposta_Pannelli(TipiEnumerativiSementieri.enum_Pannelli.FILTRO)
    End Sub


    Private Sub GST_Filtro_Impianti_Init(sender As Object, e As EventArgs) Handles Me.Init
        AddHandler CType(Me.Master, GSTBootstrap).GSTBootstrap_ImgBtn_Filtro.Click, AddressOf ImgBtn_Filtro_Click
        AddHandler CType(Me.Master, GSTBootstrap).GSTBootstrap_ImgBtn_Risultato.Click, AddressOf ImgBtn_Risultato_Click
        AddHandler CType(Me.Master, GSTBootstrap).GSTBootstrap_ImgBtn_Interferenze.Click, AddressOf ImgBtn_Interferenze_Click

        AddHandler CType(Me.Master.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
    End Sub

    Private Function AnnullaTutto()
        Response.Redirect("../GST_Menu/GST_Menu.aspx")
    End Function

    Private Sub AgroMsgBox(Messaggio As String, page As Page)
        'TODO
    End Sub


    Protected Sub ImgBtn_Risultato_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)

        '---------------------------------------------------------------------
        '--- Recupero le informazioni per il filtro
        '---------------------------------------------------------------------

        '--- Referenti

        Dim strList As New List(Of String)

        For i = 0 To CBL_ImpreseReferenti.Items.Count - 1
            If CBL_ImpreseReferenti.Items(i).Selected = True Then
                strList.Add("'" & CBL_ImpreseReferenti.Items(i).Value & "'")
            End If
        Next

        Dim Lista_Referenti = String.Join(", ", strList)

        '--- Specie Vegetali

        strList.Clear()

        For i = 0 To CBL_SpecieVegetali.Items.Count - 1
            If CBL_SpecieVegetali.Items(i).Selected = True Then
                strList.Add(CBL_SpecieVegetali.Items(i).Value)
            End If
        Next

        Dim Lista_SpecieVegetali = String.Join(", ", strList)


        '--- Data Inizio Impianto

        Dim Flag_Inizio_Precedente1_Successivo2 As Integer = 2
        If Opt_Inizio_PrecedenteAl.Checked = True Then
            Flag_Inizio_Precedente1_Successivo2 = 1
        End If

        Dim Testo = Txt_DataInizioImpianto.Text

        Dim Data_Inizio As Date = #1/1/1900#

        If (Testo = "") Or (Not IsDate(Testo)) Then
            Flag_Inizio_Precedente1_Successivo2 = 2
        Else
            Data_Inizio = CDate(Testo)
        End If

        '--- Data Fine Impianto

        Dim Flag_Fine_Precedente1_Successivo2 As Integer = 2
        If Opt_Fine_PrecedenteAl.Checked = True Then
            Flag_Fine_Precedente1_Successivo2 = 1
        End If

        Testo = Txt_DataFineImpianto.Text

        Dim Data_Fine As Date = #12/31/2100#

        If (Testo = "") Or (Not IsDate(Testo)) Then
            Flag_Fine_Precedente1_Successivo2 = 1
        Else
            Data_Fine = CDate(Testo)
        End If

        '---------------------------------------------------------------------
        '--- Effettuo la ricerca ...
        '---------------------------------------------------------------------

        Dim objVarie As New AgronicaCoreSementieriDAL.Varie_R
        Dim DT As DataTable = objVarie.Leggi_Filtrone_New(Lista_Referenti,
                                                          Lista_SpecieVegetali,
                                                          Flag_Inizio_Precedente1_Successivo2,
                                                          Data_Inizio,
                                                          Flag_Fine_Precedente1_Successivo2,
                                                          Data_Fine,
                                                          objParametri_Server,
                                                          objParametri_Utenti)

        '---------------------------------------------------------------------
        '--- Integrazione datatable Impianti
        '---------------------------------------------------------------------

        'Integro le informazioni
        Call AgronicaCoreSementieriBIZ.DataTable_Adapter.SuperDT_Impianti_Integrazione_01(DT, objParametri_Server)

        'Salvo nella Sessione
        Session("SuperDT_Impianti") = DT

        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim UtenteAmministratore As Boolean = objPermessi.Controlla_Permessi_Utente(
            objParametri_Utenti.UtenteUsername,
            5,
            TipiEnumerativi.enum_Security_Attivita.Interferenze_Visualizzazione_Estesa,
            TipiEnumerativi.enum_Security_Operazione.Lettura,
            Date.Now,
            "",
            objParametri_Utenti)

        '---------------------------------------------------------------------
        '--- Attivo il pannello del RISULTATO
        '---------------------------------------------------------------------

        Call Imposta_Pannelli(TipiEnumerativiSementieri.enum_Pannelli.RISULTATO)

        ScriptManager.RegisterStartupScript(update_panel,
                                            update_panel.GetType(),
                                            String.Format("jQuery_{0}", update_panel.ClientID), " RiempiGrigliaRisultato(" & IIf(UtenteAmministratore, "true", "false") & "); ", True)

    End Sub



    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiGrigliaImpianti() As String

        Dim jsondata = New JArray()

        Try

            Dim DT = HttpContext.Current.Session("SuperDT_Impianti")

            For Each row In DT.rows

                jsondata.Add(New JObject(
                    New JProperty("UNID_Impianto", CInt(row("UNID_Impianto"))),
                    New JProperty("Referente", row("Referente_RagSoc").ToString),
                    New JProperty("Indirizzo", row("Indirizzo").ToString),
                    New JProperty("Comune", row("Comune_Des").ToString),
                    New JProperty("Prov", row("Provincia_Sigla").ToString),
                    New JProperty("Regione", row("Regione_Des").ToString),
                    New JProperty("IndApp", row("Via_Stringa").ToString),
                    New JProperty("Specie", row("Veg_Des").ToString),
                    New JProperty("Tipologia", row("Tipologia_Des").ToString),
                    New JProperty("Superficie", Convert.ToDecimal(row("Superficie"))),
                    New JProperty("Lat", Convert.ToDecimal(row("Lat_Baricentro"))),
                    New JProperty("Lng", Convert.ToDecimal(row("Lng_Baricentro")))
                    ))
            Next

        Catch ex As Exception

        End Try

        Return jsondata.ToString
    End Function



    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function ScriviGrigliaImpianti(jsondata As String) As String

        'Recupero il datatable dalla sessione
        Dim DT As DataTable = HttpContext.Current.Session("SuperDT_Impianti")
        DT.TableName = "DT_Impianti"

        'Azzero tutti i flag di selezione
        For i = 0 To DT.Rows.Count - 1
            DT.Rows(i).Item("Selezionato") = 0
        Next

        Dim arrSelected = JArray.Parse(jsondata)
        Dim unid As Integer

        For Each objSel As JObject In arrSelected

            unid = CInt(objSel("UNID_Impianto"))

            Dim irow As Integer = 0
            Dim endWhile As Boolean = False

            While Not endWhile AndAlso irow < DT.Rows.Count

                Dim row = DT.Rows(irow)

                If unid = CInt(row("UNID_Impianto")) Then

                    row("Selezionato") = 1

                    endWhile = True
                End If

                irow += 1
            End While
        Next

        'Salvo il datatable nella Sessione
        HttpContext.Current.Session("SuperDT_Impianti") = DT

        'Garbage collection
        DT.Dispose()

        Return "true"
    End Function


    Protected Sub ImgBtn_Interferenze_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)

        Dim DT As DataTable = Session("SuperDT_Impianti")

        If DT.Select("Selezionato = 1").Length = 0 Then

            AgroMsgBox("Selezionare almeno un impianto !!!", Page)

        Else

            Response.Redirect("../GST_Interferenze/GST_Interferenze_Init.aspx")
        End If
    End Sub

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function VerificaPermessiEstrazione() As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

        Try

            Dim permessiWMS As Boolean = ObjUtenti.Controlla_Permessi_Utente(
            objParametri_Utenti.UtenteUsername,
            enum_Id_Servizio.GiasOnline,
            enum_Security_Attivita.Interferenze_ScaricoDati_Consolida,
            enum_Security_Operazione.Lettura,
            Date.Now,
            "",
            objParametri_Utenti)

            r.RispostaOK = True
            r.RispostaStringa = permessiWMS.ToString

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function SalvaEstrazione(tipoEstrazione As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim ElencoImpianti As DataTable = HttpContext.Current.Session("SuperDT_Impianti")

        Dim Sportello As String = HttpContext.Current.Session("Sementi")
        Dim SportelloInt As Int32

        Dim FlagTransazioneLocale As Boolean
        Dim FlagConnessioneLocale As Boolean

        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim permessiWMS As Boolean

        Try

            permessiWMS = ObjUtenti.Controlla_Permessi_Utente(
                                        objParametri_Utenti.UtenteUsername,
                                        enum_Id_Servizio.GiasOnline,
                                        enum_Security_Attivita.Interferenze_ScaricoDati_Consolida,
                                        enum_Security_Operazione.Lettura,
                                        Date.Now,
                                        "",
                                        objParametri_Utenti)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
            Return r
        End Try


        If Not permessiWMS Then
            r.RispostaOK = False
            r.Errore = "L'utente non ha i permessi per effettuare l'operazione richiesta"
            Return r
        End If

        If Not Int32.TryParse(Sportello.Split("|")(4), SportelloInt) Then
            r.RispostaOK = False
            r.Errore = "Codice sportello non valido"
            Return r
        End If

        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri_Server)

            Dim xWrite As New AgronicaCoreSementieriDAL.Sementieri_EstrazioneColtivazioni_W

            xWrite.Elimina(SportelloInt, 0, tipoEstrazione, objParametri_Server)

            For Each row As DataRow In ElencoImpianti.Rows
                Dim resp = xWrite.Scrivi(SportelloInt,
                                         row("Regione_Des").ToString,
                                         tipoEstrazione,
                                         row("Referente_RagSoc").ToString,
                                         row("UNID_Impianto").ToString,
                                         row("Veg_Des").ToString,
                                         row("NomeScientifico").ToString,
                                         row("Tipologia_Des").ToString,
                                         row("Provincia_Sigla").ToString,
                                         row("Comune_Des").ToString,
                                         row("RagioneSociale").ToString,
                                         row("Indirizzo").ToString,
                                         Convert.ToDecimal(row("Lat_Baricentro")),
                                         Convert.ToDecimal(row("Lng_Baricentro")),
                                         Convert.ToDecimal(row("Superficie")),
                                         0,
                                         CostantiPersonalizzate.AGRODATAINIZIO,
                                         objParametri_Server)

                If Not resp Then
                    Throw New Exception("Errore nel salvataggio dei dati richiesti")
                End If
            Next

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = "Operazione completata con successo."

        Catch ex As Exception

            If Not objParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            r.RispostaOK = False
            r.Errore = ex.Message

        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
        End Try

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function InfoSportello() As RispostaStandard

        Dim r As New RispostaStandard

        Dim Sportello As String = HttpContext.Current.Session("Sementi")
        Dim SportelloInt As Int32

        If Not Int32.TryParse(Sportello.Split("|")(4), SportelloInt) Then
            r.RispostaOK = False
            r.Errore = "Codice sportello non valido"
            Return r
        End If

        Try

            Dim objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")
            Dim obj_R As New AgronicaCoreSementieriDAL.Sportello_R
            Dim DT As DataTable = obj_R.Leggi_Sementieri_Sportello_Configurazione(SportelloInt,
                                                                                  CostantiPersonalizzate.AGRODATAINIZIO,
                                                                                  "",
                                                                                  objParametri_Server)

            If DT.Rows.Count <> 1 Then
                r.RispostaOK = False
                r.Errore = String.Format("Sportello {0} non trovato", SportelloInt)
                Return r
            End If

            Dim DataInizio, DataFine As DateTime

            DataInizio = CDate(DT.Rows(0)("Validita_Inizio"))
            DataFine = CDate(DT.Rows(0)("Validita_Fine"))

            Dim datiSportello As New JObject

            datiSportello.Add("sportello_des", DT.Rows(0)("Sementieri_Sportello_Configurazione_des").ToString)
            datiSportello.Add("data_inizio", DataInizio.ToString("dd/MM/yyyy"))
            datiSportello.Add("data_fine", DataFine.ToString("dd/MM/yyyy"))

            r.RispostaOK = True
            r.RispostaStringa = datiSportello.ToString

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function

End Class