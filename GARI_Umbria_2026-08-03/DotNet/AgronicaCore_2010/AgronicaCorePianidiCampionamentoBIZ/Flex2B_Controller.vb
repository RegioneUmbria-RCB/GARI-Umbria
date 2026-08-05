Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class Flex2B_Controller

    Private _cfg As Flex2B_Model_CFG

    Public Sub New(ByRef objParametriServer As AgronicaCoreParametri)

        'legge la configurazione ed i dati da inviare
        If _cfg Is Nothing Then
            LetturaF2BCFG(objParametriServer)
        End If

    End Sub

    Public ReadOnly Property ServizioInvioConfigurato() As Boolean
        Get
            Return Not _cfg Is Nothing And Not String.IsNullOrEmpty(_cfg.url)
        End Get
    End Property

    Private Function InvioFlex2B_SessionInit(ByRef objParametriServer As AgronicaCoreParametri) As RispostaStandard

        Dim rval As rispostaStandard(Of Flex2B_Model_SessionInit_out)

        Dim endPoint As String = "sessioninit"
        
        'Hash SHA256 of Password
        If String.IsNullOrEmpty(_cfg.passwordHash) Then
            _cfg.passwordHash = Sicurezza.CalculateSHA256_ToString(_cfg.password)
        End If


        Dim dati As New Flex2B_Model_SessionInit_in With {
            .userid = _cfg.username,
            .password = _cfg.passwordHash
        }

        rval = CallAPI(Of Flex2B_Model_SessionInit_in, Flex2B_Model_SessionInit_out)(dati, endPoint)

        Dim r1 As New RispostaStandard
        r1.RispostaOK = rval.RispostaOK
        r1.RispostaStringa = rval.RispostaStringa.sessionuuid

        If Not rval.RispostaOK Then
            r1.Errore = JsonConvert.SerializeObject(rval.RispostaStringa)
        End If

        Return r1

    End Function

    Private Function InvioFlex2B_Logout(ByRef objParametriServer As AgronicaCoreParametri) As RispostaStandard
        Throw New NotImplementedException("InvioFlex2B_Logout")
    End Function

    Private Function InvioFlex2B_test(ByVal paramFiltro As InvioFlex2B_IN) As Boolean
        Dim rval As Boolean = True
        If paramFiltro.ID_PDC_Testata = 0 Then
            rval = False
        End If
        If paramFiltro.ID_PDC_Dettagli = 0 Then
            rval = False
        End If
        Return rval

    End Function

    Public Function InvioFlex2B(ByVal paramFiltro As InvioFlex2B_IN,
                                ByRef objParametriServer As AgronicaCoreParametri
                                ) As RispostaStandard
        
        Const nomeRoutine = "InvioFlex2B()"
        Dim rvalInvioFlex2B As New RispostaStandard
        Dim rvalUUID As RispostaStandard

        'Inizializzazione
        Try 
            If Not ServizioInvioConfigurato() Then
                Throw New Exception("Please configure parameters for Flex2B.")
            End If

            Dim testParamIn As Boolean = InvioFlex2B_test(paramFiltro)

            If Not testParamIn Then
                Throw New Exception("Parameter input error in InvioFlex2B")
            End If

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "]: " & ex.Message)
        End Try
        

        Try
            
            'Lettura dati
            Dim dati As Flex2B_Model_Agronica_in = LetturaF2BDati(paramFiltro, objParametriServer)

            Dim msgValidazione As String = ""
            Dim risValidazione As Boolean = ValidazioneFlex2B(dati, msgValidazione)

            If risValidazione = False OrElse Not String.IsNullOrEmpty(msgValidazione) Then
                rvalInvioFlex2B.RispostaOK = False
                rvalInvioFlex2B.Errore = "Error Validation data: " & msgValidazione
                Return rvalInvioFlex2B
            End If

            'inizializza la sessione
            rvalUUID = InvioFlex2B_SessionInit(objParametriServer)

            If Not rvalUUID.RispostaOK Then
                Throw New Exception("Error initializing session: " & rvalUUID.Errore)
            End If

            Dim rvalInvioApi As RispostaStandard 
            DIm stato As Integer = -1
            Dim err As String = ""
            Try
                'effettua l'invio
                rvalInvioApi = InvioFlex2B_Agronica(rvalUUID.RispostaStringa, dati)

                If Not rvalInvioApi.RispostaOK Then
                    stato = 0
                    err = rvalInvioApi.Errore
                    Throw New Exception("Error sending data: " & rvalInvioApi.Errore)
                Else
                    stato = 1

                    'Modifica la riga (cioè tutti i capitolati) scrivendo il blocco!!!
                    Dim objPDCMarketW As New AgronicaCorePianidiCampionamentoDAL.PDC_MarketAccess_W
                    Dim esito = objPDCMarketW.ModificaPuntuale(paramFiltro.ID_PDC_Testata, paramFiltro.ID_PDC_Dettagli, -1,
                                                               objParametriServer,
                                                               Blocco_Flag:=1,
                                                               Blocco_Data:=Now,
                                                               Blocco_Username:=objParametriServer.UsernameOperazione)

                End If
            Catch exc As Exception
                stato = 0
                If err = "" Then
                    err = "Error sending data"
                End If
            Finally
                'Scrivo il log (se c'era già lo aggiorno)
                Dim objFlexLogW As new AgronicaCorePianidiCampionamentoDAL.PDC_Flex2B_W
                objFlexLogW.ScriviAggiornaRecord(paramFiltro.ID_PDC_Testata, paramFiltro.ID_PDC_Dettagli,
                                                 stato, err, Now, objParametriServer)
            End Try

            rvalInvioFlex2B.RispostaOK = True

        Catch ex As Exception
            rvalInvioFlex2B.RispostaOK = False
            rvalInvioFlex2B.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        Finally

            'effettua il logout
            If rvalUUID IsNot Nothing AndAlso rvalUUID.RispostaOK Then
                'solo se sono stato loggato.
            End If

        End Try

        Return rvalInvioFlex2B

    End Function

    Private Sub LetturaF2BCFG(ByRef objParametriServer As AgronicaCoreParametri)

        Dim letturaCfgSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim sCfg As String = letturaCfgSiti.Leggi_Valore(6, "G2F2B_Cfg", "", "", objParametriServer)

        If Not String.IsNullOrEmpty(sCfg) Then
            _cfg = JsonConvert.DeserializeObject(Of Flex2B_Model_CFG)(sCfg)
        Else
            _cfg = New Flex2B_Model_CFG
        End If

    End Sub


    Private Function LetturaF2BDati(ByVal paramFiltro As InvioFlex2B_IN, ByRef objParametriServer As AgronicaCoreParametri) As Flex2B_Model_Agronica_in

        Dim objR As New AgronicaCorePianidiCampionamentoDAL.PDC_MarketAccess_R
        Dim dtMarket As DataTable = objR.LeggiF2B(ID_PDC_Testata:=paramFiltro.ID_PDC_Testata,
                                                  ID_PDC_Dettagli:=paramFiltro.ID_PDC_Dettagli,
                                                  xFiltroAggiuntivo:="",
                                                  xOrderBy:="",
                                                  objParametri:=objParametriServer)

        If dtMarket.Rows.Count = 0 Then
            Throw New Exception("Data Access Error")
        End If

        Dim pivotatore As New Pivot(dtMarket)
        Dim dtPivotata As DataTable = pivotatore.Pivot(dtMarket, dtMarket.Columns("Market"), dtMarket.Columns("Esito"))

        Dim ds As New DataSet("DataSet")
        ds.Tables.Add(dtPivotata)

        Dim json1 As String = JsonConvert.SerializeObject(ds, Formatting.Indented)
        Dim rss As JObject = JObject.Parse(json1)
        Dim q = (From r In rss("Table1")).ToList().First.ToString()
        Dim rval As Flex2B_Model_Agronica_in = JsonConvert.DeserializeObject(Of Flex2B_Model_Agronica_in)(q)

        'Modifico il separatore dei decimali per ton
        rval.ton = Cstr(rval.ton).Replace(".", ",")

        Return rval

    End Function

    Private Function ValidazioneFlex2B(ByRef rval As Flex2B_Model_Agronica_in, ByRef msgValidazione As String) As Boolean
        Dim risValidazione As Boolean = False

        If String.IsNullOrEmpty(rval.facility) Then
            msgValidazione &= "facility field is missing. Please contact the administrator. "
        End If

        If String.IsNullOrEmpty(rval.ggn) Then
            msgValidazione &= "ggn field is missing. "
        ElseIf rval.ggn.Length <> 13 OrElse Not IsNumeric(rval.ggn) Then
            msgValidazione &= "ggn field must be numeric and 13 characters long. "
        ElseIf rval.ggn.StartsWith("0") Then
            msgValidazione &= "ggn field can't start with zero. "
        End If

        If String.IsNullOrEmpty(rval.ton) OrElse rval.ton = "0" OrElse CDec(rval.ton) = 0D  Then
            msgValidazione &= "ton field is missing. "
        End If

        If msgValidazione = "" Then
            risValidazione = True
        End If

        Return risValidazione

    End Function

    ''' <summary>
    ''' invio all'API
    ''' </summary>
    ''' <param name="uuid"></param>
    ''' <param name="cfg"></param>
    ''' <param name="Dati"></param>
    ''' <returns></returns>
    Private Function InvioFlex2B_Agronica(uuid As String, Dati As Flex2B_Model_Agronica_in) As RispostaStandard

        Dim rval As rispostaStandard(Of Flex2B_Model_Agronica_Out)

        Dim endPoint As String = "agronica?sessionUUID=" & uuid

        rval = CallAPI(Of Flex2B_Model_Agronica_in, Flex2B_Model_Agronica_Out)(Dati, endPoint)

        Dim r1 As New RispostaStandard
        r1.RispostaOK = rval.RispostaOK

        If Not rval.RispostaOK Then
            r1.Errore = JsonConvert.SerializeObject(rval.RispostaStringa)
        End If

        Return r1

    End Function


    Private Function CallAPI(Of TIn As Flex2B_Model_api_in, TOut As Flex2B_Model_api_out)(Dati As TIn, endPoint As String) As rispostaStandard(Of TOut)

        Dim hlp As New AgronicaCoreUtility.RestSharpHelper(addSlashIfNotExists(_cfg.url) & endPoint)
        Dim sDati As String = APIFlex2BSerialize(Dati)

        hlp.AddBody(sDati)

        Dim restval As AgronicaCoreUtility.RestSharpHelper.Response = hlp.Execute()

        Dim r As TOut = JsonConvert.DeserializeObject(Of TOut)(restval.Content("content").ToString())

        Dim rval As New rispostaStandard(Of TOut)

        Select Case restval.StatusCode
            Case Net.HttpStatusCode.OK

            Case Else
                rval.RispostaOK = False

        End Select

        'a prescindere dallo stato HTTP l'esito si trova nella proprietà passed
        rval.RispostaOK = r.passed
        rval.RispostaStringa = r


        Return rval

    End Function

    Private Function addSlashIfNotExists(s As String) As String
        If Not s.EndsWith("/") Then
            s &= "/"
        End If
        Return s
    End Function


    Private Function APIFlex2BSerialize(Of T)(Dati As T) As String

        Dim settings As JsonSerializerSettings = New JsonSerializerSettings With {
            .Culture = New Globalization.CultureInfo(_cfg.serializationCultureInfo)
        }

        Dim rval As String = JsonConvert.SerializeObject(Dati, settings)
        Return rval

    End Function

End Class
