
Imports System.Web.Services
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreGestioneRichieste
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Runtime.Serialization
Imports Newtonsoft.Json.Converters
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD.meteo.Consiglio_Irrigazione
Imports AgronicaCoreVarieDAL
Imports AgroAgenda_2010.Irriframe
Imports DocumentFormat.OpenXml.Drawing

Public Class DSS_Irrigazione
    Inherits System.Web.UI.Page

    Private objParametriAgenda As ParametriAgenda
    Public srv_gm As String

#Region "Filtro Impianto da visualizzare"
    Private Shared Piva_Indicatore As String
    Private Shared Sa_Cod_Indicatore As Integer
    Private Shared Appezza_Indicatore As Integer
    Private Shared Id_Reg_Indicatore As Integer
#End Region


    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub


    Private Sub DSSIrrigazione_Init(sender As Object, e As EventArgs) Handles Me.Init
        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Piva_Indicatore = ""
        Sa_Cod_Indicatore = 0
        Appezza_Indicatore = 0
        Id_Reg_Indicatore = 0

        objParametriAgenda = New ParametriAgenda
        Session("objParametriAgenda") = objParametriAgenda

        Dim DSSIrrigazione_Autorizzato = False

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If Not (HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server)) Then

            'Recupero da webconfig la connessione alternativa da usare
            Dim AgroWebC As New AgroWebConfig()

            srv_gm = "https://maps.googleapis.com/maps/api/js?key="

            If AgroWebC.GoogleMaps <> "" Then
                srv_gm = AgroWebC.GoogleMaps
                srv_gm = srv_gm.Replace("&sensor=false&libraries=drawing,geometry", "")
            End If

            If Debugger.IsAttached Then
                srv_gm = "https://maps.googleapis.com/maps/api/js?v=3.exp&amp;client=gme-addictive&amp;channel=geocoder-tool&amp;libraries=places&callback=Function.prototype"
            End If

            Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

            DSSIrrigazione_Autorizzato = ObjUtenti.Controlla_Permessi_Utente(
                HttpContext.Current.Session("ASG_Utente_Username"),
                HttpContext.Current.Session("ASG_IdServizio"),
                enum_Security_Attivita.DSS_Irrigazione,
                enum_Security_Operazione.Modifica,
                Date.Now, "", objParametri_Utenti)

        End If

        If Not DSSIrrigazione_Autorizzato Then

            AnnullaTutto()
        End If

        If Not IsNothing(Request.QueryString.Item("ifr")) AndAlso IsNumeric(Request.QueryString.Item("ifr")) AndAlso CInt(Request.QueryString.Item("ifr")) = 1 Then
            Master.flag_MostraHeader = False
            Master.flag_MostraFooter = False
        End If

        If Not IsNothing(Request.QueryString.Item("imp")) AndAlso Not String.IsNullOrEmpty(Request.QueryString.Item("imp")) AndAlso Not String.IsNullOrWhiteSpace(Request.QueryString.Item("imp")) Then
            Dim impInfo As List(Of String) = Request.QueryString.Item("imp").Split("_").ToList()

            If Not IsNothing(impInfo) AndAlso impInfo.Count = 4 Then
                Piva_Indicatore = impInfo(0).ToString()
                Sa_Cod_Indicatore = CInt(impInfo(1))
                Appezza_Indicatore = CInt(impInfo(2))
                Id_Reg_Indicatore = CInt(impInfo(3))
            End If
        End If

    End Sub


    Private Sub AnnullaTutto()

        Dim objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim DTConfigSiti As DataTable = objConfigSiti.Leggi(0, "MenuBS_2017", "", "", objParametri_Server)

        Dim TargetUrl = "~/menu/menubs_2017.aspx"

        If DTConfigSiti Is Nothing OrElse DTConfigSiti.Rows.Count = 0 OrElse DTConfigSiti.Rows(0).Item("Valore").ToString <> "true" Then

            TargetUrl = CType(Master, AgendaBootstrap).TrovaRedirectCorretto(False, enum_PagineAgenda_2010.Menu, objParametriAgenda)
        End If

        Response.Redirect(TargetUrl)
    End Sub



    <WebMethod(EnableSession:=True)>
    Public Shared Function MessaggioANBI() As RispostaStandard

        Dim r As New RispostaStandard
        r.RispostaOK = True

        Dim MsgFull, MsgShort As String

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")

        Dim objConfigurazioneSitiR As New Configurazione_Siti_R
        Dim dtConfigurazioneSiti = objConfigurazioneSitiR.Leggi_ServerESuperServer2(0, "", "", "", objParametri_Server, objParametri_Super_Server, True)
        Dim dsConfigurazioneSiti = dtConfigurazioneSiti.Select("Chiave = 'personalizzazioniRegioneUmbria'")
        Dim personalizzazioneRU As Boolean = dsConfigurazioneSiti.Count() > 0 AndAlso dsConfigurazioneSiti(0).Item("Valore") = "1"
        Dim strNomeSistema As String = If(personalizzazioneRU, "GARI", "GIAS")

        MsgFull = "Il sistema " & strNomeSistema & " integra il <strong>Servizio IRRIFRAME</strong>. Irriframe è un progetto <a href='http://www.anbi.it' target='_blank'>ANBI</a> "
        MsgFull &= "(Associzione Nazionale Consorzi di gestione e tutela del territorio e acque irrigue), "
        MsgFull &= "il coordinamento tecnico-agronomico è del <a href='http://www.consorziocer.it' target='_blank'>CER</a> (Consorzio per il Canale Emiliano Romagnolo).</br>"
        MsgFull &= "Il servizio ha l'obiettivo di indicare il preciso momento di intervento irriguo ed il volume di adacquata e si alimenta dei dati del modello di calcolo del bilancio idrico.</br>"
        MsgFull &= "</br>"
        MsgFull &= "Si ricorda che il <strong>Servizio IRRIFRAME è fruibile gratuitamente e indipendentemente da " & strNomeSistema & " sul sito <a href='http://www.irriframe.it' target='_blank'>irriframe</a>.</strong>"

        MsgShort = "Servizio IRRIFRAME - Fornito da ANBI e Consorzi di Bonifica"

        Dim BannerHidden As Boolean = (objParametri_Server.PivaSuperUser = "05644051004")

        r.RispostaStringa = JsonConvert.SerializeObject(New With {
                                                        MsgFull,
                                                        MsgShort,
                                                        BannerHidden},
                                                        Newtonsoft.Json.Formatting.None,
                                                        New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore})

        Return r
    End Function





#Region "Vecchio codice sostituito con V2"



    <DataContract>
    Public Class Indicatore
        <DataMember>
        Public CUAA As String
        <DataMember>
        Public Centro As String
        <DataMember>
        Public Campo As String
        <DataMember>
        Public Impianto As String
        <DataMember>
        Public Coltura As String
        <DataMember>
        Public Varieta As String
        <DataMember>
        Public Veg_Cod As Integer
        <DataMember>
        Public Cul_Cod As Integer
        <DataMember>
        Public GruppVegetale As String
        <DataMember>
        Public TraFila As Decimal
        <DataMember>
        Public SuFila As Decimal
        <DataMember>
        Public CondTraFila As String
        <DataMember>
        Public DataFaseStart As Date?
        <DataMember>
        Public DataInizioImpianto As Date?
        <DataMember>
        Public Sup_Imp As Decimal

        <DataContract>
        Public Class _latlng
            <DataMember>
            Public Lat As Decimal
            <DataMember>
            Public Lng As Decimal
            Public Sub New(_lat As Decimal, _lng As Decimal)
                Lat = _lat
                Lng = _lng
            End Sub
        End Class

        <DataMember>
        Public LatLng As _latlng
        <DataMember>
        Public Indirizzo As String
        <DataMember>
        Public Irri_Veg_Cod As Integer
        <DataMember>
        Public Irri_Veg_Des As String
        <DataMember>
        Public Irri_Imp_Cod As Integer
        <DataMember>
        Public Irri_Imp_Des As String
        <DataMember>
        Public Pendenza As Decimal
        <DataMember>
        Public Vigoria As Integer
        <DataMember>
        Public Sabbia As Decimal?
        <DataMember>
        Public Limo As Decimal?
        <DataMember>
        Public Argilla As Decimal?

        <DataContract>
        Public Class _stazioneMeteo
            <DataMember>
            Public Sorgente As Integer
            <DataMember>
            Public Stazione As Integer
            Public Sub New(s As Integer, z As Integer)
                Sorgente = s
                Stazione = z
            End Sub
        End Class

        <DataMember>
        Public Stazioni_Meteo As List(Of _stazioneMeteo)

        <DataContract>
        Public Class _irrigazione
            <DataMember>
            Public DataIrri As String
            <DataMember>
            Public VolumeMM As Decimal
            Public Sub New(d As Date, v As Decimal)
                DataIrri = d.ToString("yyyy-MM-dd")
                VolumeMM = v
            End Sub
        End Class

        <DataMember>
        Public Irrigazioni As List(Of _irrigazione)

        <DataContract>
        Public Class _persistData
            <DataMember>
            Public GIAS_Piva As String
            <DataMember>
            Public GIAS_SaCod As Integer
            <DataMember>
            Public GIAS_Appezza As Integer
            <DataMember>
            Public GIAS_IdReg As Integer
            <DataMember>
            Public Irri_PlotId As Integer
            <DataMember>
            Public Irri_CropId As Integer
        End Class

        <DataMember>
        Public PersistData As _persistData

        'Aggiungo anche un oggetto per semplificare il feedback nel client
        <DataContract>
        Public Class _infoElem
            <DataMember>
            Public text As String
            <DataMember>
            Public value As String
            Public Sub New(t As String, v As String)
                text = t
                value = v
            End Sub
        End Class

        <DataMember>
        Public Info As List(Of _infoElem)

        <DataMember>
        Public ColturaProtetta As Boolean

        <DataMember>
        Public Id_GreenHouseType As Integer

        <DataMember>
        Public Errore As String

        Public Sub New(row As DataRow)
            CUAA = row("CUAA").ToString
            Centro = row("Centro").ToString
            Campo = row("Campo").ToString
            Impianto = row("Impianto").ToString
            Coltura = row("Coltura").ToString
            Varieta = row("Varieta").ToString
            Veg_Cod = CInt(row("Veg_Cod"))
            Cul_Cod = CInt(row("Cul_Cod"))
            GruppVegetale = row("GruppoVegetale").ToString
            TraFila = CDec(row("TraFila"))
            SuFila = CDec(row("SuFila"))
            CondTraFila = row("CondTraFila").ToString

            'controllare che GruppVegetale.ToLower.Trim = "arboree" ???
            If Not String.IsNullOrEmpty(row("DataRilievo").ToString) Then

                DataFaseStart = CDate(row("DataRilievo"))
            Else

                If Not String.IsNullOrEmpty(row("DataSemina").ToString) Then

                    DataFaseStart = CDate(row("DataSemina"))
                Else

                    If Not String.IsNullOrEmpty(row("DataSeminaPrevista").ToString) Then

                        Dim dfs = CDate(row("DataSeminaPrevista"))
                        If dfs.Date > (New Date(1900, 1, 1)).Date Then

                            DataFaseStart = dfs
                        End If
                    End If
                End If
            End If

            If Not String.IsNullOrEmpty(row("DataInizioImpianto").ToString) Then
                DataInizioImpianto = CDate(row("DataInizioImpianto"))
            End If

            Sup_Imp = CDec(row("sup_imp")) '.ToString(CultureInfo.InvariantCulture)

            If Not String.IsNullOrEmpty(row("Lat").ToString) AndAlso Not String.IsNullOrEmpty(row("Lng").ToString) Then
                Dim lat As Decimal = CDec(row("Lat")) '.ToString(CultureInfo.InvariantCulture)
                Dim lng As Decimal = CDec(row("Lng")) '.ToString(CultureInfo.InvariantCulture)
                LatLng = New _latlng(lat, lng)
            End If

            If Not String.IsNullOrEmpty(row("Indirizzo").ToString) Then

                Dim objIndir = JObject.Parse(row("Indirizzo").ToString)

                Indirizzo = String.Join(" ", {
                                        objIndir("Indirizzo").ToString,
                                        objIndir("Fraz").ToString,
                                        objIndir("CAP").ToString,
                                        objIndir("Comune").ToString,
                                        objIndir("Prov").ToString,
                                        "IT"
                                        }.Where(Function(s) Not String.IsNullOrEmpty(s)))
            End If

            Irri_Veg_Cod = CInt(row("Irri_Veg_Cod"))
            Irri_Veg_Des = row("Irri_Veg_Des").ToString
            Irri_Imp_Cod = CInt(row("Irri_Imp_Cod"))
            Irri_Imp_Des = row("Irri_Imp_Des").ToString
            Pendenza = CDec(row("Pendenza")) 'Convert.ToDecimal(row("Pendenza"), CultureInfo.InvariantCulture)
            Vigoria = CInt(row("Vigoria"))

            If Not IsDBNull(row("Sabbia")) Then
                Sabbia = CDec(row("Sabbia")) 'Convert.ToDecimal(row("Sabbia"), CultureInfo.InvariantCulture)
            End If
            If Not IsDBNull(row("Limo")) Then
                Limo = CDec(row("Limo")) 'Convert.ToDecimal(row("Limo"), CultureInfo.InvariantCulture)
            End If
            If Not IsDBNull(row("Argilla")) Then
                Argilla = CDec(row("Argilla")) 'Convert.ToDecimal(row("Argilla"), CultureInfo.InvariantCulture)
            End If

            Stazioni_Meteo = New List(Of _stazioneMeteo)
            If Not String.IsNullOrEmpty(row("Stazioni_Meteo").ToString) Then
                Dim stazArr = JArray.Parse(row("Stazioni_Meteo").ToString)
                For Each stazObj As JObject In stazArr
                    Stazioni_Meteo.Add(New _stazioneMeteo(CInt(stazObj("Sorgente")), CInt(stazObj("Stazione"))))
                Next
            End If

            Irrigazioni = New List(Of _irrigazione)
            If Not String.IsNullOrEmpty(row("Irrigazioni")) Then
                Dim irriArr = JArray.Parse(row("Irrigazioni").ToString)
                For Each irriObj As JObject In irriArr
                    Irrigazioni.Add(New _irrigazione(CDate(irriObj("DataIrri")), CDec(irriObj("VolumeMM"))))
                Next
            End If

            PersistData = New _persistData With {
                .GIAS_Piva = row("GIAS_PIva").ToString,
                .GIAS_SaCod = CInt(row("GIAS_SaCod")),
                .GIAS_Appezza = CInt(row("GIAS_Appezza")),
                .GIAS_IdReg = CInt(row("GIAS_IdReg")),
                .Irri_PlotId = CInt(row("Irri_Plot_Id")),
                .Irri_CropId = CInt(row("Irri_Crop_Id"))
            }

            Info = New List(Of _infoElem)

            Dim tmpValue As String

            tmpValue = ""
            If DataInizioImpianto.HasValue Then
                tmpValue = DataInizioImpianto.Value.ToString("d")
            End If
            Info.Add(New _infoElem("Inizio impianto", tmpValue))

            If GruppVegetale.ToLower.Trim = "arboree" Then

                tmpValue = ""
                If TraFila > 0 Then
                    tmpValue = Format(TraFila, "0.0")
                End If
                Info.Add(New _infoElem("Distanza tra fila", tmpValue))

                tmpValue = ""
                If SuFila > 0 Then
                    tmpValue = Format(SuFila, "0.0")
                End If
                Info.Add(New _infoElem("Distanza su fila", tmpValue))

                tmpValue = CondTraFila
                If Not String.IsNullOrEmpty(CondTraFila) Then
                    If tmpValue.ToLower.Trim <> "inerbito" Then
                        tmpValue = "Lavorato"
                    End If
                End If
                Info.Add(New _infoElem("Conduzione interfilare", tmpValue))

                tmpValue = ""
                Select Case Vigoria
                    Case 1
                        tmpValue = "Debole"
                    Case 2
                        tmpValue = "Medio"
                    Case 3
                        tmpValue = "Vigoroso"
                    Case 4
                        tmpValue = "Molto vigoroso"
                End Select
                Info.Add(New _infoElem("Classe vigore", tmpValue))

            End If

            tmpValue = ""
            If Irri_Imp_Cod > 0 Then
                tmpValue = Irri_Imp_Des
            End If
            Info.Add(New _infoElem("Impianto irriguo", tmpValue))

            Info.Add(New _infoElem("Pendenza", Format(Pendenza, "0") + "%"))

            tmpValue = ""
            If Sabbia.HasValue Then
                tmpValue = Format(Sabbia.Value, "0.0") + "%"
            End If
            Info.Add(New _infoElem("Sabbia", tmpValue))

            If Argilla.HasValue Then
                tmpValue = Format(Argilla.Value, "0.0") + "%"
            End If
            Info.Add(New _infoElem("Argilla", tmpValue))

            ColturaProtetta = False
            Id_GreenHouseType = 0
            Dim cop = row("Cop_Cod")
            Dim cop_mappato = row("Cop_Cod_Irriframe")
            If Not IsDBNull(cop) Then
                Dim copNum = CInt(cop)
                If Not IsDBNull(cop_mappato) Then
                    ColturaProtetta = True
                    Id_GreenHouseType = CInt(cop_mappato)
                Else
                    If (Not {0, 3, 4, 5, 6}.Contains(copNum)) Then
                        ColturaProtetta = True
                        Errore = "Mappatura copertura non trovata"
                    End If
                End If
            End If
        End Sub
    End Class


    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaIndicatori() As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")

        Try

            Dim objWS As New AgronicaCoreWebService.Codifica_SpecieVegetali_WS

            Dim objParIn As New AgronicaCoreMetaSchemaBIZ.Codifica_SpecieVegetaliCliente_Input With {
                .Cli_Cod = 2116 'Irriframe
            }

            Dim objParOut As AgronicaCoreMetaSchemaBIZ.Codifica_SpecieVegetaliCliente_Output = objWS.SpecieVegetaliClienteTranscod(objParIn)

            Dim json_txcod As String = JsonConvert.SerializeObject(objParOut.Lista)

            '--------------------------------------------------------------------------------------

            Dim objParIn_Irri As New AgronicaCoreMetaSchemaBIZ.Codifica_ImpiantiIrrigui_Cliente_Input With {
                .Cli_Cod = 2116 'Irriframe
            }

            Dim objParOut_Irri As AgronicaCoreMetaSchemaBIZ.Codifica_ImpiantiIrrigui_Cliente_Output = objWS.ImpiantiIrriguiClienteTranscod(objParIn_Irri)

            Dim json_tx_irricod As String = JsonConvert.SerializeObject(objParOut_Irri.Lista)

            '--------------------------------------------------------------------------------------

            Dim objFF_input As New AgronicaCoreMetaSchemaBIZ.FasiFenologiche_input With {
                .Lingua_Cod = objParametri_Server.Lingua_Cod,
                .SoloRipresaVegetativa = True
            }

            Dim objFF_output As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output = (New AgronicaCoreWebService.FasiFenologiche_WS).FasiFenologiche(objFF_input)

            Dim json_ff_start As String = ""

            If objFF_output.MessaggioErrore <> "" Then

                'Throw New Exception(objParametriUscita.MessaggioErrore)
            Else

                json_ff_start = JsonConvert.SerializeObject(objFF_output.ListaFasiFenologiche)
            End If

            '--------------------------------------------------------------------------------------

            Dim objParametriAgenda As New ParametriAgenda

            Dim objRead As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim DT As DataTable = objRead.Leggi_x_DSS_Irrigazione(objParametriAgenda.Piva, json_txcod, json_tx_irricod, json_ff_start, objParametri_Server)

            Dim Indics As New List(Of Indicatore)

            For Each row In DT.Rows

                Indics.Add(New Indicatore(row))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(Indics, New IsoDateTimeConverter() With {.DateTimeFormat = "yyyy-MM-dd"})
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
    Public Shared Function Transcodifica(ByVal Veg_Cod As Integer, ByVal Cul_Cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParIn As New AgronicaCoreMetaSchemaBIZ.Codifica_SpecieVegetali_Input
        Dim objWS As New AgronicaCoreWebService.Codifica_SpecieVegetali_WS
        Dim objParOut As AgronicaCoreMetaSchemaBIZ.Codifica_SpecieVegetali_Output

        Dim Irri_Veg_Cod As Integer = -1
        Dim Irri_Veg_Des As String = ""

        objParIn.Cli_Cod = 2116 'Irriframe
        objParIn.Veg_Cod = Veg_Cod
        objParIn.Cul_Cod = 0 'Cul_Cod

        Try

            objParOut = objWS.SpecieVegetaliTranscod(objParIn)

            If objParOut.Lista.Count > 0 Then
                Irri_Veg_Cod = objParOut.Lista(0).Veg_Cod
                Irri_Veg_Des = objParOut.Lista(0).Veg_Des
            End If

            Dim obj As New JObject(
                New JProperty("Irri_Veg_Cod", Irri_Veg_Cod),
                New JProperty("Irri_Veg_Des", Irri_Veg_Des)
                )

            r.RispostaStringa = obj.ToString
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
    Public Shared Function CalcolaWBResult(ByVal inPar As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim if_api As New IrriframeApi(objParametri_Server)

            Dim objIn = JObject.Parse(inPar)
            Dim objOut As New JObject

            If Not if_api.Persistent() Then

                objOut = if_api.GetAdvice(objIn)

            Else

                Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

                Dim auth As UserAuth = GetAuthToken(objParametri_Utenti, objParametri_Server)

                objOut = if_api.GetAdvicePersist(objIn, auth.Token)

                If CInt(objOut("Status")) = 0 Then

                    EnsurePersistence(objIn("PersistData"), objOut("IrriframeData"), objParametri_Server)
                End If

            End If

            r.RispostaOK = True
            r.RispostaStringa = objOut.ToString

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function



#End Region





    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaIndicatori_V2() As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim irri As New DSSIrrigazione.IrrigazioneShared()

            Dim centriList = irri.ImpiantiIrrigabili(objParametri_Server, False, Sa_Cod_Indicatore, Appezza_Indicatore, Id_Reg_Indicatore)

            r.RispostaStringa = JsonConvert.SerializeObject(centriList, New IsoDateTimeConverter() With {.DateTimeFormat = "yyyy-MM-dd"})
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
    Public Shared Function CalcolaWBResult_V2(input As Irriframe.WBInput) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then

            r.Sessione = False
            Return r
        End If

        Dim resp As IrriframeAPI_V2.Response

        Try

            Dim if_api As New IrriframeAPI_V2(objParametri_Server)

            If Not if_api.Persistent Then

                resp = if_api.GetAdvice(input)

            Else

                Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

                Dim auth As UserAuth = GetAuthToken(objParametri_Utenti, objParametri_Server)

                resp = if_api.GetAdvicePersist(input, auth.Token)

                If resp.Status = 0 Then

                    'EnsurePersistence(objIn("PersistData"), objOut("IrriframeData"), objParametri_Server)
                End If
            End If

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(resp)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function IrriframeAllowPersitence() As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim if_api As New IrriframeAPI_V2(objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(New With {.AllowPersistence = if_api.Persistent})

        Catch ex As Exception

            r.RispostaOK = False
        End Try

        Return r
    End Function

    Private Class UserAuth
        Public Token As String
        Public Id As Integer
    End Class


    Private Shared Function GetAuthToken(objParametri_Utenti As AgronicaCoreParametri, objParametri_Server As AgronicaCoreParametri) As UserAuth

        Dim if_api As New IrriframeApi(objParametri_Server)

        Dim auth As UserAuth = Nothing

        Dim leggiCredenziali As New AgronicaCoreUtentiDAL.Credenziali_Irriframe_R

        Dim dtCred = leggiCredenziali.Leggi(objParametri_Utenti.SuperUserUsername, objParametri_Utenti)

        If dtCred Is Nothing OrElse dtCred.Rows.Count = 0 Then

            Dim IF_username As String = objParametri_Utenti.SuperUserUsername

            Dim objCred = if_api.CreaCredenziali(IF_username)

            If objCred IsNot Nothing Then

                Dim scriviCredenziali As New AgronicaCoreUtentiDAL.Credenziali_Irriframe_W
                scriviCredenziali.Scrivi(objParametri_Utenti.SuperUserUsername, IF_username, CInt(objCred("id")), objParametri_Utenti)

                auth = New UserAuth With {
                .Token = objCred("token").ToString,
                .Id = CInt(objCred("id"))
            }
            End If

        Else

            Dim rCred = dtCred.Rows(0)

            Dim objCred = if_api.LeggiCredenziali(CInt(rCred("UserId_IF")), rCred("UserName_IF").ToString)

            If objCred IsNot Nothing Then

                auth = New UserAuth With {
                .Token = objCred("token").ToString,
                .Id = CInt(rCred("UserId_IF"))
            }
            End If
        End If

        If auth Is Nothing Then
            auth = New UserAuth With {
                .Token = "",
                .Id = 0
            }
        End If

        Return auth
    End Function


    Private Shared Sub EnsurePersistence(inPersist As JObject, outPersist As JObject, objParametri_Server As AgronicaCoreParametri)

        Try

            If inPersist Is Nothing OrElse outPersist Is Nothing Then
                Return
            End If

            Dim inPlotId As Integer = CInt(inPersist("Irri_PlotId"))
            Dim outPlotId As Integer = CInt(outPersist("PlotId"))

            Dim PIVA As String = inPersist("GIAS_Piva").ToString
            Dim SaCod As Integer = CInt(inPersist("GIAS_SaCod"))
            Dim Appezza As Integer = CInt(inPersist("GIAS_Appezza"))

            If inPlotId <> outPlotId Then

                Dim app_codR = New AgronicaCoreAnagrafeDAL.Appezzamento_Codici_R
                Dim app_codW = New AgronicaCoreAnagrafeDAL.Appezzamento_Codici_W

                Dim dt = app_codR.Leggi(PIVA, SaCod, Appezza, 2225, "", AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                    app_codW.Modifica(PIVA, SaCod, Appezza, 2225, outPlotId,
                                  objParametri_Server.FinestraTemporaleInizio,
                                  objParametri_Server.FinestraTemporaleFine,
                                  "",
                                  objParametri_Server)
                Else
                    app_codW.Scrivi(PIVA, SaCod, Appezza, 2225, outPlotId,
                                objParametri_Server.FinestraTemporaleInizio,
                                objParametri_Server.FinestraTemporaleFine,
                                objParametri_Server)
                End If
            End If

            Dim inCropId As Integer = CInt(inPersist("Irri_CropId"))
            Dim outCropId As Integer = CInt(outPersist("CropId"))

            Dim IdReg As Integer = CInt(inPersist("GIAS_IdReg"))

            If inCropId <> outCropId Then

                Dim imp_codR = New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
                Dim imp_codW = New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W

                Dim dt = imp_codR.Leggi(PIVA, SaCod, Appezza, IdReg, "", 2225, "", AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                    imp_codW.Modifica(PIVA, SaCod, Appezza, IdReg, 2225, outCropId,
                                  objParametri_Server.FinestraTemporaleInizio,
                                  objParametri_Server.FinestraTemporaleFine,
                                  "",
                                  objParametri_Server)
                Else
                    imp_codW.Scrivi(PIVA, SaCod, Appezza, IdReg, 2225, outCropId,
                                objParametri_Server.FinestraTemporaleInizio,
                                objParametri_Server.FinestraTemporaleFine,
                                objParametri_Server)
                End If
            End If

        Catch ex As Exception

        End Try

    End Sub












    <WebMethod(EnableSession:=True)>
    Public Shared Function PreparaPaginaIrrigazione() As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objParametriAgenda As New ParametriAgenda
            objParametriAgenda.Lav_Cod = CostantiPersonalizzate.LAVCOD_IRRIGAZIONE
            objParametriAgenda.salva()

            r.RispostaOK = True
            r.RispostaStringa = "../../Operazioni/Irrigazione.aspx"

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function


    Public Class Irri2Save
        Public Class _giaskey
            Public PIVA As String
            Public SaCod As Integer
            Public Appezza As Integer
            Public IdReg As Integer
            Public ProgettoCod As Integer
            Public VegCod As Integer
            Public CulCod As Integer
            Public SupImp As Decimal
        End Class
        Public Class _ifkey
            Public IdPlot As Integer
            Public IdCrop As Integer
        End Class
        Public GIAS_key As _giaskey
        Public IF_key As _ifkey
        Public Data As Date
        Public VolumeMM As Decimal
        Public RunData As Date
    End Class

    Public Class IrriTurno2Save
        Public Data As DateTime
        Public DurataM As Integer
        Public VolumeMM As Decimal
    End Class


    <WebMethod(EnableSession:=True)>
    Public Shared Function SalvaIrrigazione(irri As Irri2Save) As RispostaStandard

        Dim risp As New RispostaStandard

        Try

            Dim GIAS_Saved As Boolean = False
            Dim IF_Saved As Boolean = False

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                risp.Sessione = False
                Return risp
            End If

            If irri.GIAS_key IsNot Nothing Then

                Dim irri_GIAS As New JObject(
                New JProperty("Piva", irri.GIAS_key.PIVA),
                New JProperty("Sa_Cod", irri.GIAS_key.SaCod),
                New JProperty("Appezza", irri.GIAS_key.Appezza),
                New JProperty("Id_Reg", irri.GIAS_key.IdReg),
                New JProperty("Veg_Cod", irri.GIAS_key.VegCod),
                New JProperty("Cul_Cod", irri.GIAS_key.CulCod),
                New JProperty("Sup_Imp", irri.GIAS_key.SupImp),
                New JProperty("Data", irri.Data),
                New JProperty("Dose", irri.VolumeMM)
                )

                Dim arr As New JArray From {irri_GIAS}

                Dim prog As Integer = HttpContext.Current.Session("ASG_ProgressivoGIAS")

                Dim irriSaver As New AgronicaCoreModello.Irrigazione_BIZ
                Dim rispSaver As RispostaStandard = irriSaver.RegistraIrrigazione(arr.ToString, prog, objParametri_Server)

                GIAS_Saved = rispSaver.RispostaOK

            End If

            If irri.IF_key IsNot Nothing Then

                Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

                Dim auth As UserAuth = GetAuthToken(objParametri_Utenti, objParametri_Server)

                Dim if_api As New IrriframeApi(objParametri_Server)

                Dim irri_if As New IrriframeApi.Irri2Save With {
                .Id_Plot = irri.IF_key.IdPlot,
                .DataIrri = irri.Data,
                .VolumeMM = irri.VolumeMM,
                .User = auth.Id
            }

                Dim objResp = if_api.SaveIrrigation(irri_if, auth.Token)

                IF_Saved = CInt(objResp("Status")) = 0

            End If

            Dim irrigazione As Indicatore._irrigazione = Nothing

            If GIAS_Saved OrElse IF_Saved Then
                irrigazione = New Indicatore._irrigazione(irri.Data.ToString("yyy-MM-dd"), irri.VolumeMM)
            End If

            risp.RispostaOK = True
            risp.RispostaStringa = New JObject(
            New JProperty("GIAS_Saved", GIAS_Saved),
            New JProperty("IF_Saved", IF_Saved),
            New JProperty("Irrigazione", JObject.FromObject(irrigazione))
            ).ToString

        Catch ex As Exception

            risp.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return risp
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ControllaDSSIrrigazione(irri As Irri2Save) As RispostaStandard

        Dim risp As New RispostaStandard

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                risp.Sessione = False
                Return risp
            End If

            Dim Obj As New AgronicaCoreMeteoDAL.DSS_Irrigazione_R

            Dim DT = Obj.Leggi(irri.GIAS_key.PIVA, irri.GIAS_key.SaCod, irri.GIAS_key.Appezza, irri.GIAS_key.IdReg,
                               irri.GIAS_key.ProgettoCod, irri.RunData, "", "", objParametri_Server)

            risp.RispostaOK = True
            risp.RispostaStringa = If(Not IsNothing(DT) AndAlso DT.Rows.Count > 0, DT.Rows(0)("ID").ToString(), "0")

        Catch ex As Exception

            risp.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return risp
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function SalvaDSSIrrigazione(irri As Irri2Save) As RispostaStandard

        Dim risp As New RispostaStandard

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                risp.Sessione = False
                Return risp
            End If

            Dim Obj As New AgronicaCoreMeteoDAL.DSS_Irrigazione_W

            Dim ScritturaOk = Obj.Scrivi(irri.GIAS_key.PIVA, irri.GIAS_key.SaCod, irri.GIAS_key.Appezza, irri.GIAS_key.IdReg,
                                            irri.GIAS_key.ProgettoCod, irri.RunData, irri.Data,
                                            irri.VolumeMM, enum_UnitaMisura.Millimetri,
                                            Provider_DSS_Irrigazione.GIAS_Irriframe, Modello_DSS_Irrigazione.Irriframe, objParametri_Server)

            risp.RispostaOK = ScritturaOk
            risp.RispostaStringa = ""

        Catch ex As Exception

            risp.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return risp
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function SalvaDSSIrrigazioneConTurni(irri As Irri2Save, irriTurni As IrriTurno2Save()) As RispostaStandard

        Dim risp As New RispostaStandard

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                risp.Sessione = False
                Return risp
            End If

            Dim ScritturaOk As Boolean = True

            Dim Flag_Connessione, Flag_Transazione As Boolean

            Dim ObjR As New AgronicaCoreMeteoDAL.DSS_Irrigazione_R
            Dim ObjW As New AgronicaCoreMeteoDAL.DSS_Irrigazione_W

            Try
                Utility.VerificaApriTransazione(objParametri_Server, Flag_Connessione, Flag_Transazione)

                ScritturaOk = ObjW.Scrivi(irri.GIAS_key.PIVA, irri.GIAS_key.SaCod, irri.GIAS_key.Appezza, irri.GIAS_key.IdReg,
                                          irri.GIAS_key.ProgettoCod, irri.RunData, irri.Data,
                                          irri.VolumeMM, enum_UnitaMisura.Millimetri,
                                          Provider_DSS_Irrigazione.GIAS_Irriframe, Modello_DSS_Irrigazione.Irriframe, objParametri_Server)

                If ScritturaOk Then

                    Dim dtDSS = ObjR.Leggi(irri.GIAS_key.PIVA, irri.GIAS_key.SaCod, irri.GIAS_key.Appezza, irri.GIAS_key.IdReg,
                                           irri.GIAS_key.ProgettoCod, irri.RunData, "", "ID Desc", objParametri_Server)

                    Dim IDIrrig As Integer = dtDSS.Rows(0)("ID")

                    ' TODO: solo se la scrittura del consiglio irriguo dovesse andare in aggiornamento, in caso di consiglio già esistente 
                    ' per impianto/esercizio colturale e data esecuzione, anche i relativi turni dovrebbero essere cancellati
                    'ObjW.CancellaTurni(IDIrrig, 0, objParametri_Server)

                    'Dim IDIrrigTurno As Integer = 0
                    Dim objSequenze As New AgronicaCoreDataProvider.Agro_Sequenze

                    For Each irriTurno In irriTurni

                        'IDIrrigTurno = IDIrrigTurno + 1
                        Dim IDIrrigTurno = objSequenze.NuovoId_Tabella("DSS_Irrigazione_Turni", 0, 0, objParametri_Server)

                        ScritturaOk = ObjW.ScriviTurno(IDIrrig, IDIrrigTurno, irriTurno.Data, irriTurno.DurataM, enum_UnitaMisura.Minuti, objParametri_Server)

                        If Not ScritturaOk Then
                            Exit For
                        End If
                    Next

                End If

                If ScritturaOk Then
                    Utility.VerificaChiudiTransazione(objParametri_Server, Flag_Transazione)
                Else
                    Utility.VerificaAnnullaTransazione(objParametri_Server, Flag_Transazione)
                End If

            Catch ex As Exception
                Utility.VerificaAnnullaTransazione(objParametri_Server, Flag_Transazione)
                Throw

            Finally
                Utility.VerificaChiudiConnessione(objParametri_Server, Flag_Connessione)

            End Try

            risp.RispostaOK = ScritturaOk
            risp.RispostaStringa = ""

        Catch ex As Exception

            risp.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return risp
    End Function

End Class