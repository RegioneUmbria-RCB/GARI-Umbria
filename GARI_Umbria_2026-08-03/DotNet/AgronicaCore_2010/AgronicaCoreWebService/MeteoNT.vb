
Imports System.Security.Policy
Imports AgronicaCoreDataProvider
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreUtility
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq


Public Class MeteoWSParametri
    Private Shared Function EncryptDecrypt(ByVal StrIn As String, ByVal encrypt As Boolean) As String

        Dim AWS_AgroKey_EncoderDecoder As String = "cJy97ei45vrNIsvbe86Wn42rffCHvc8e3it63koLa12"

        Dim A1 As Integer
        Dim A2 As Integer
        Dim Chiave As String = AWS_AgroKey_EncoderDecoder
        Dim StrOut As String = ""

        Dim lenS As Integer = Len(StrIn)
        Dim lenK As Integer = Len(Chiave)

        For pos = 1 To lenS
            A1 = AscW(Mid(StrIn, pos, 1))
            A2 = AscW(Mid(Chiave, (pos Mod lenK) + 1, 1))

            If encrypt Then

                StrOut = StrOut & ChrW(A2 + A1)

            Else

                StrOut = StrOut & ChrW(A1 - A2)

            End If
        Next

        Return StrOut
    End Function

    Public Shared Function Encrypt(ByVal JsonObj As JObject) As String

        Return EncryptDecrypt(JsonObj.ToString, True)

    End Function

    Public Shared Function Decrypt(ByVal Str) As JObject

        Return JObject.Parse(EncryptDecrypt(Str, False))

    End Function
End Class



Public Class MeteoNT

    Private Function LeggiUrlMeteo(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim DTConfigSiti As DataTable = objConfigSiti.Leggi(0, "GiasOnline_WS_Meteo_Meteo", "", "", objParametri_Server)

        Dim url As String = ""

        If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 AndAlso LCase(DTConfigSiti.Rows(0).Item("Valore")) <> "" Then
            url = DTConfigSiti.Rows(0).Item("Valore")
            url = url.Replace("Meteo.asmx", "ModelliPrevisionaliMeteo.svc")
        End If

        Return url
    End Function


    Public Function ElencoStazioniConDistanza(ByVal Piva_SuperUser As String,
                                              ByVal Doorkey As String,
                                              ByVal Utente_Username_client_GIAS As String,
                                              ByVal Username As String,
                                              ByVal Password As String,
                                              ByVal PivaVisibilita_ClientGIASImpostata As String,
                                              ByVal objParametri_Server As AgronicaCoreParametri,
                                              ByVal Lat As Double,
                                              ByVal Lng As Double,
                                              ByVal TipoSorgente As Integer,
                                              Optional ByVal pGiasOnline_WS_Meteo_Meteo As String = "") As DataTable


        Dim DT_Stazioni As DataTable = Nothing

        Try

            Dim PivaVisibilita_ClientGias As String = ""

            If String.IsNullOrEmpty(PivaVisibilita_ClientGIASImpostata) Then

                Dim xLetturaVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim dtPiva As DataTable = xLetturaVisibilita.Leggi(1, "", "", objParametri_Server)
                PivaVisibilita_ClientGias = String.Join(",", (From pp In dtPiva.AsEnumerable Select CStr(pp("piva"))).ToArray())
            Else

                PivaVisibilita_ClientGias = PivaVisibilita_ClientGIASImpostata
            End If

            Dim objParam As New JObject

            objParam("doorkey") = Doorkey
            objParam("ucg") = Utente_Username_client_GIAS   'Utente_Username_Client_gias_Cript
            objParam("usr") = Username
            objParam("pwd") = Password
            objParam("tkn") = ""
            objParam("piva") = PivaVisibilita_ClientGias
            objParam("piva_superuser") = Piva_SuperUser
            objParam("lat") = Lat
            objParam("lng") = Lng
            objParam("tiposorgente") = TipoSorgente

            Dim strParam As String = MeteoWSParametri.Encrypt(objParam)

            Dim WS_Meteo As New WS_Meteo.Meteo


            If Not String.IsNullOrEmpty(pGiasOnline_WS_Meteo_Meteo) Then
                WS_Meteo.Url = pGiasOnline_WS_Meteo_Meteo
            Else
                Dim objAgroWebConfig As New AgroWebConfig
                WS_Meteo.Url = objAgroWebConfig.GiasOnline_WS_Meteo_Meteo
            End If

            Dim strElenco As String = WS_Meteo.ElencoStazioniConDistanza(strParam)

            Dim jsonObj = JObject.Parse(strElenco)

            Dim ErrCod As Integer = 0
            If jsonObj("errcod") IsNot Nothing Then
                ErrCod = jsonObj("errcod")
            End If

            If ErrCod = 0 Then

                Dim Elenco = jsonObj("risultato")
                If Elenco IsNot Nothing Then

                    DT_Stazioni = New DataTable

                    DT_Stazioni.Columns.Add(New DataColumn("id", GetType(Integer)))
                    DT_Stazioni.Columns.Add(New DataColumn("descrizione", GetType(String)))
                    DT_Stazioni.Columns.Add(New DataColumn("fornitore", GetType(String)))
                    DT_Stazioni.Columns.Add(New DataColumn("rif_fornitore", GetType(String)))
                    DT_Stazioni.Columns.Add(New DataColumn("data_ultimo_agg", GetType(Date)))
                    DT_Stazioni.Columns.Add(New DataColumn("distanza", GetType(Decimal)))

                    Dim row As DataRow

                    For Each elem In Elenco

                        row = DT_Stazioni.NewRow

                        row("id") = CInt(elem("id"))
                        row("descrizione") = elem("descrizione").ToString
                        row("fornitore") = elem("fornitore").ToString
                        row("rif_fornitore") = elem("rif_fornitore").ToString

                        If elem("ultimoAggiornamento") IsNot Nothing Then
                            row("data_ultimo_agg") = CDate(elem("ultimoAggiornamento"))
                        Else
                            row("data_ultimo_agg") = DBNull.Value
                        End If

                        If elem("distanza") IsNot Nothing Then
                            row("distanza") = CDec(elem("distanza"))
                        Else
                            row("distanza") = DBNull.Value
                        End If

                        DT_Stazioni.Rows.Add(row)

                    Next

                End If
            End If

        Catch ex As Exception

        End Try

        Return DT_Stazioni
    End Function

    Public Function DatiMeteoElabora(ByVal objParams As JObject, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim url As String = LeggiUrlMeteo(objParametri_Server)
        url &= "/DatiMeteoElabora_V2"

        Dim hlpHttp As New Http

        Dim meteoParams As New JObject
        meteoParams("meteoParams") = objParams.ToString

        Dim risp As String = hlpHttp.chiamaWS(meteoParams.ToString, Nothing, url, "application/json", "POST", "application/json", "", TimeOutRichiesta:=600)

        Dim objRisp = JObject.Parse(risp)

        Return objRisp("DatiMeteoElabora_V2Result").ToString
    End Function

    Public Function DatiMeteoElaboraRiepilogo(ByVal objParams As JObject, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim url As String = LeggiUrlMeteo(objParametri_Server)
        url &= "/DatiMeteoElaboraRiepilogo"

        Dim hlpHttp As New Http

        Dim meteoParams As New JObject
        meteoParams("meteoParams") = MeteoWSParametri.Encrypt(objParams)

        Dim risp As String = hlpHttp.chiamaWS(meteoParams.ToString, Nothing, url, "application/json", "POST", "application/json", "", TimeOutRichiesta:=600)

        Dim objRisp = JObject.Parse(risp)

        Return objRisp("DatiMeteoElaboraRiepilogoResult").ToString
    End Function

    Public Function DatiMeteoElaboraMonitoraggioSuolo(ByVal objParams As JObject, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim url As String = LeggiUrlMeteo(objParametri_Server)
        url &= "/DatiMeteoElaboraMonitoraggioSuolo"

        Dim hlpHttp As New Http

        Dim meteoParams As New JObject
        meteoParams("meteoParams") = MeteoWSParametri.Encrypt(objParams)

        Dim risp As String = hlpHttp.chiamaWS(meteoParams.ToString, Nothing, url, "application/json", "POST", "application/json", "", TimeOutRichiesta:=600)

        Dim objRisp = JObject.Parse(risp)

        Return objRisp("DatiMeteoElaboraMonitoraggioSuoloResult").ToString
    End Function

    Public Function QuadrantiRER(ByVal objParams As JObject, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String
        Dim url As String = LeggiUrlMeteo(objParametri_Server)
        url &= "/QuadrantiRER"

        Dim hlpHttp As New Http

        Dim meteoParams As New JObject
        meteoParams("params") = objParams.ToString

        Dim risp As String = hlpHttp.chiamaWS(meteoParams.ToString, Nothing, url, "application/json", "POST", "application/json", "", TimeOutRichiesta:=600)

        Dim objRisp = JObject.Parse(risp)

        Return objRisp("QuadrantiRERResult").ToString
    End Function

    Public Function DatiMeteoElaboraPiogge(ByVal objParams As JObject, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim url As String = LeggiUrlMeteo(objParametri_Server)
        url &= "/DatiMeteoElaboraPiogge"

        Dim hlpHttp As New Http

        Dim meteoParams As New JObject
        meteoParams("meteoParams") = objParams.ToString

        Dim risp As String = hlpHttp.chiamaWS(meteoParams.ToString, Nothing, url, "application/json", "POST", "application/json", "", TimeOutRichiesta:=600)

        Dim objRisp = JObject.Parse(risp)

        Return objRisp("DatiMeteoElaboraPioggeResult").ToString
    End Function

    Public Function DatiMeteoElaboraIrrigazione(ByVal objParams As JObject, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim url As String = LeggiUrlMeteo(objParametri_Server)
        url &= "/DatiMeteoElaboraIrrigazione"

        Dim hlpHttp As New Http

        Dim meteoParams As New JObject
        meteoParams("meteoParams") = objParams.ToString

        Dim risp As String = hlpHttp.chiamaWS(meteoParams.ToString, Nothing, url, "application/json", "POST", "application/json", "", TimeOutRichiesta:=600)

        Dim objRisp = JObject.Parse(risp)

        Return objRisp("DatiMeteoElaboraIrrigazioneResult").ToString

    End Function

    Public Function ModelliPrevisionaliElabora(ByVal objParams As JObject, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim url As String = LeggiUrlMeteo(objParametri_Server)
        url &= "/ModelliPrevisionaliElabora"

        Dim hlpHttp As New Http

        Dim inputParams As New JObject
        inputParams("inputParams") = MeteoWSParametri.Encrypt(objParams)

        Dim risp As String = hlpHttp.chiamaWS(inputParams.ToString, Nothing, url, "application/json", "POST", "application/json", "", TimeOutRichiesta:=600)

        Dim objRisp = JObject.Parse(risp)

        Return objRisp("ModelliPrevisionaliElaboraResult").ToString
        'Return Newtonsoft.Json.JsonConvert.SerializeObject(objRisp("ModelliPrevisionaliElaboraResult"), Newtonsoft.Json.Formatting.None)
    End Function


    Public Function ModelliPrevisionaliElabora_Indicatore(objParams As JObject, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim url As String = LeggiUrlMeteo(objParametri_Server)
        url &= "/ModelliPrevisionaliElabora_Indicatore"

        Dim hlpHttp As New Http

        Dim inputParams As New JObject
        inputParams("inputParams") = MeteoWSParametri.Encrypt(objParams)

        Dim risp As String = hlpHttp.chiamaWS(inputParams.ToString, Nothing, url, "application/json", "POST", "application/json", "", TimeOutRichiesta:=600)

        Dim objRisp = JObject.Parse(risp)

        Return objRisp("ModelliPrevisionaliElabora_IndicatoreResult").ToString
    End Function


    Public Function ModelliPrevisionaliElaboraIndicatori(ByVal objParams As JObject, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim url As String = LeggiUrlMeteo(objParametri_Server)
        url &= "/ModelliPrevisionaliIndicatori"

        Dim hlpHttp As New Http

        Dim inputParams As New JObject
        inputParams("inputParams") = MeteoWSParametri.Encrypt(objParams)

        Dim risp As String = hlpHttp.chiamaWS(inputParams.ToString, Nothing, url, "application/json", "POST", "application/json", "", TimeOutRichiesta:=600)

        Dim objRisp = JObject.Parse(risp)

        Return objRisp("ModelliPrevisionaliIndicatoriResult").ToString
    End Function


    Public Function ModelliPrevisionaliElaboraIndicatoriV2(ByVal objParams As JObject, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim url As String = LeggiUrlMeteo(objParametri_Server)
        url &= "/ModelliPrevisionaliIndicatoriV2"

        Dim hlpHttp As New Http

        Dim inputParams As New JObject
        inputParams("inputParams") = MeteoWSParametri.Encrypt(objParams)

        Dim risp As String = hlpHttp.chiamaWS(inputParams.ToString, Nothing, url, "application/json", "POST", "application/json", "", TimeOutRichiesta:=600)

        Dim objRisp = JObject.Parse(risp)

        Return objRisp("ModelliPrevisionaliIndicatoriV2Result").ToString
    End Function


    Public Function ModelliPrevisionaliElaboraIndicatoriV3(ByVal objParams As JObject, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim url As String = LeggiUrlMeteo(objParametri_Server)
        url &= "/ModelliPrevisionaliIndicatoriV3"

        Dim hlpHttp As New Http

        Dim inputParams As New JObject
        inputParams("inputParams") = MeteoWSParametri.Encrypt(objParams)

        Dim risp As String = hlpHttp.chiamaWS(inputParams.ToString, Nothing, url, "application/json", "POST", "application/json", "", TimeOutRichiesta:=600)

        Dim objRisp = JObject.Parse(risp)

        Return objRisp("ModelliPrevisionaliIndicatoriV3Result").ToString
    End Function



    Public Function ElencoSorgentiMeteo(objParams As JObject, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim url As String = LeggiUrlMeteo(objParametri_Server)
        url &= "/LeggiSorgentiMeteo"

        Dim hlpHttp As New Http

        Dim inputParams As New JObject
        inputParams("inputParams") = MeteoWSParametri.Encrypt(objParams)

        Dim risp As String = hlpHttp.chiamaWS(inputParams.ToString, Nothing, url, "application/json", "POST", "application/json", "", TimeOutRichiesta:=600)

        Dim objRisp = JObject.Parse(risp)

        Return objRisp("LeggiSorgentiMeteoResult").ToString
    End Function

    Public Function LeggiAnagraficaStazioni(ByVal objParams As JObject, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim url As String = LeggiUrlMeteo(objParametri_Server)
        url &= "/LeggiAnagraficaStazioni"

        Dim hlpHttp As New Http

        Dim inputParams As New JObject
        inputParams("inputParams") = MeteoWSParametri.Encrypt(objParams)

        Dim risp As String = hlpHttp.chiamaWS(inputParams.ToString, Nothing, url, "application/json", "POST", "application/json", "", TimeOutRichiesta:=600)

        Dim objRisp = JObject.Parse(risp)

        Return objRisp("LeggiAnagraficaStazioniResult").ToString
    End Function

    Public Function AggiornaAnagraficaStazione(ByVal objParams As JObject, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim url As String = LeggiUrlMeteo(objParametri_Server)
        url &= "/AggiornaAnagraficaStazione"

        Dim hlpHttp As New Http

        Dim inputParams As New JObject
        inputParams("inputParams") = MeteoWSParametri.Encrypt(objParams)

        Dim risp As String = hlpHttp.chiamaWS(inputParams.ToString, Nothing, url, "application/json", "POST", "application/json", "", TimeOutRichiesta:=600)

        Dim objRisp = JObject.Parse(risp)

        Return objRisp("AggiornaAnagraficaStazioneResult").ToString
    End Function

    Public Function VerificaEliminaStazione(ByVal objParams As JObject, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim url As String = LeggiUrlMeteo(objParametri_Server)
        url &= "/VerificaEliminaStazione"

        Dim hlpHttp As New Http

        Dim inputParams As New JObject
        inputParams("inputParams") = MeteoWSParametri.Encrypt(objParams)

        Dim risp As String = hlpHttp.chiamaWS(inputParams.ToString, Nothing, url, "application/json", "POST", "application/json", "", TimeOutRichiesta:=600)

        Dim objRisp = JObject.Parse(risp)

        Return objRisp("VerificaEliminaStazioneResult").ToString
    End Function

    Public Function LeggiStazioniXSorgente(ByVal objParams As JObject, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim TipoSorgente As Integer = objParams("TipoSorgente")

        If TipoSorgente = 999 Then

            Dim piva As String = objParams("PIVA")

            'Dim dt_SxI = objSxI.Leggi(piva, xFiltroAggiuntivo, "", objParametri_Server)

            Dim objSxI As New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Irriga_Reader

            Dim filtroAggiuntivo As AgronicaCoreMeteoDAL.DSS_Stazioni_X_Irriga_Reader.Filter = Nothing

            If objParams("CodiceCentro") IsNot Nothing Then
                Try

                    Dim sa_cod As Integer = CInt(objParams("CodiceCentro"))

                    If sa_cod > 0 Then

                        filtroAggiuntivo = New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Irriga_Reader.Filter With {
                            .CodiceCentro = sa_cod,
                            .TipoSorgente = -1,
                            .StazioneCod = -1
                        }
                    End If
                Catch ex As Exception

                End Try
            End If

            Dim dt_SxI = objSxI.Leggi(piva, filtroAggiuntivo, objParametri_Server)

            Dim dictSorgente As New Dictionary(Of Integer, List(Of Integer))

            For Each _si In dt_SxI.Rows

                Dim _tipoSorgente As Integer = _si("Tipo_Sorgente")
                Dim _stazioneCod As Integer = _si("Stazione_Cod")

                If Not dictSorgente.ContainsKey(_tipoSorgente) Then

                    dictSorgente.Add(_tipoSorgente, New List(Of Integer))
                End If

                If Not dictSorgente(_tipoSorgente).Contains(_stazioneCod) Then

                    dictSorgente(_tipoSorgente).Add(_stazioneCod)
                End If
            Next

            Dim j_stazioni As New JArray
            Dim j_staz As JArray
            For Each kvp In dictSorgente

                j_staz = New JArray

                For Each s In kvp.Value

                    j_staz.Add(New JObject(New JProperty("stazione_cod", s)))
                Next

                j_stazioni.Add(New JObject(New JProperty("tipo_sorgente", kvp.Key), New JProperty("elenco_stazioni", j_staz)))
            Next

            objParams.Add(New JProperty("ElencoStazioni", j_stazioni))
        End If

        Dim url As String = LeggiUrlMeteo(objParametri_Server)
        url &= "/LeggiStazioniXSorgente"

        Dim hlpHttp As New Http

        Dim inputParams As New JObject
        inputParams("inputParams") = MeteoWSParametri.Encrypt(objParams)

        Dim risp As String = hlpHttp.chiamaWS(inputParams.ToString, Nothing, url, "application/json", "POST", "application/json", "", TimeOutRichiesta:=600)

        Dim objRisp = JObject.Parse(risp)

        Return objRisp("LeggiStazioniXSorgenteResult").ToString
    End Function

    Public Function LeggiStazione(stazione_cod As Integer, needSensors As Boolean, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim url As String = LeggiUrlMeteo(objParametri_Server)
        url &= "/LeggiStazione"

        Dim hlpHttp As New Http

        Dim objParams As New JObject From {
            {"Id_Stazione", stazione_cod},
            {"NeedSensors", needSensors}
        }

        Dim inputParams As New JObject
        inputParams("inputParams") = MeteoWSParametri.Encrypt(objParams)

        Dim risp As String = hlpHttp.chiamaWS(inputParams.ToString, Nothing, url, "application/json", "POST", "application/json", "", TimeOutRichiesta:=600)

        Dim objRisp = JObject.Parse(risp)

        Return objRisp("LeggiStazioneResult").ToString
    End Function

    Public Function LeggiElencoStazioni(ByVal objParams As JObject, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim url As String = LeggiUrlMeteo(objParametri_Server)
        url &= "/LeggiElencoStazioni"

        Dim hlpHttp As New Http

        Dim inputParams As New JObject
        inputParams("inputParams") = MeteoWSParametri.Encrypt(objParams)

        Dim risp As String = hlpHttp.chiamaWS(inputParams.ToString, Nothing, url, "application/json", "POST", "application/json", "", TimeOutRichiesta:=600)

        Dim objRisp = JObject.Parse(risp)

        Return objRisp("LeggiElencoStazioniResult").ToString
    End Function

    Public Function StazioniDaLatLng(ByVal objParams As JObject, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim url As String = LeggiUrlMeteo(objParametri_Server)
        url &= "/StazioniDaLatLng"

        Dim hlpHttp As New Http

        Dim inputParams As New JObject
        inputParams("inputParams") = MeteoWSParametri.Encrypt(objParams)

        Dim risp As String = hlpHttp.chiamaWS(inputParams.ToString, Nothing, url, "application/json", "POST", "application/json", "", TimeOutRichiesta:=600)

        Dim objRisp = JObject.Parse(risp)

        Return objRisp("StazioniDaLatLngResult").ToString
    End Function

    Public Function LeggiModelliAutorizzati(ByVal objParams As JObject, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim url As String = LeggiUrlMeteo(objParametri_Server)
        url &= "/LeggiModelliAutorizzati"

        Dim hlpHttp As New Http

        Dim inputParams As New JObject
        inputParams("inputParams") = MeteoWSParametri.Encrypt(objParams)

        Dim risp As String = hlpHttp.chiamaWS(inputParams.ToString, Nothing, url, "application/json", "POST", "application/json", "", TimeOutRichiesta:=600)

        Dim objRisp = JObject.Parse(risp)

        Return objRisp("LeggiModelliAutorizzatiResult").ToString
    End Function

    Public Function LeggiTuttiModelliAutorizzati(ByVal objParams As JObject, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim url As String = LeggiUrlMeteo(objParametri_Server)
        url &= "/LeggiTuttiModelliAutorizzati"

        Dim hlpHttp As New Http

        Dim inputParams As New JObject
        inputParams("inputParams") = MeteoWSParametri.Encrypt(objParams)

        Dim risp As String = hlpHttp.chiamaWS(inputParams.ToString, Nothing, url, "application/json", "POST", "application/json", "", TimeOutRichiesta:=600)

        Dim objRisp = JObject.Parse(risp)

        Return objRisp("LeggiTuttiModelliAutorizzatiResult").ToString
    End Function

    Public Function CompletaOutputModelli(ByVal objParams As JObject, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim url As String = LeggiUrlMeteo(objParametri_Server)
        url &= "/CompletaOutputModelli"

        Dim hlpHttp As New Http

        Dim inputParams As New JObject
        inputParams("inputParams") = MeteoWSParametri.Encrypt(objParams)

        Dim risp As String = hlpHttp.chiamaWS(inputParams.ToString, Nothing, url, "application/json", "POST", "application/json", "", TimeOutRichiesta:=600)

        Dim objRisp = JObject.Parse(risp)

        Return objRisp("CompletaOutputModelliResult").ToString
    End Function

    Public Function LeggiTipoSensoriPerStazione(ByVal objParams As JObject, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String
        Dim url As String = LeggiUrlMeteo(objParametri_Server)
        url &= "/LeggiElencoTipiSensorePerStazione"

        Dim hlpHttp As New Http

        Dim inputParams As New JObject
        inputParams("inputParams") = MeteoWSParametri.Encrypt(objParams)

        Dim risp As String = hlpHttp.chiamaWS(inputParams.ToString, Nothing, url, "application/json", "POST", "application/json", "", TimeOutRichiesta:=600)

        Dim objRisp = JObject.Parse(risp)

        Return objRisp("LeggiElencoTipiSensorePerStazioneResult").ToString
    End Function


#Region "Gesione alias"

    Public Class StazioneAlias
        Public Id As Integer
        Public Name As String
        Public Lat As Decimal
        Public Lng As Decimal
    End Class


    Public Class StazioneAliasNew
        Inherits StazioneAlias

        Public Fornitore As String
        Public Dist As Decimal
        Public FlagNew As Boolean
        <JsonProperty(PropertyName:="Alias")>
        Public AliasName As String
    End Class



    Public Function LeggiElencoAlias(piva_superuser As String, piva As String, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As List(Of StazioneAlias)

        Dim url As String = LeggiUrlMeteo(objParametri_Server)
        url &= "/LeggiElencoAlias"

        Dim hlpHttp As New Http

        Dim objParams As New JObject(
            New JProperty("PIVA_Superuser", piva_superuser),
            New JProperty("PIVA", piva))

        Dim inputParams As New JObject
        inputParams("inputParams") = MeteoWSParametri.Encrypt(objParams)

        Dim risp As String = hlpHttp.chiamaWS(inputParams.ToString, Nothing, url, "application/json", "POST", "application/json", "", TimeOutRichiesta:=600)

        Dim objRisp = JObject.Parse(risp)

        Dim json = objRisp("LeggiElencoAliasResult")("RispostaStringa").ToString

        Dim objAlias = JsonConvert.DeserializeObject(Of List(Of StazioneAlias))(json)

        Return objAlias
    End Function



    Public Function LeggiStazioneXAlias(piva_superuser As String, piva As String, lat As Decimal, lng As Decimal, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As StazioneAliasNew

        Dim url As String = LeggiUrlMeteo(objParametri_Server)
        url &= "/LeggiStazioneXAlias"

        Dim hlpHttp As New Http

        Dim objParams As New JObject(
            New JProperty("PIVA_Superuser", piva_superuser),
            New JProperty("PIVA", piva),
            New JProperty("Lat", lat),
            New JProperty("Lng", lng))

        Dim inputParams As New JObject
        inputParams("inputParams") = MeteoWSParametri.Encrypt(objParams)

        Dim risp As String = hlpHttp.chiamaWS(inputParams.ToString, Nothing, url, "application/json", "POST", "application/json", "", TimeOutRichiesta:=600)

        Dim objRisp = JObject.Parse(risp)

        Dim json = objRisp("LeggiStazioneXAliasResult")("RispostaStringa").ToString

        Dim objAlias = JsonConvert.DeserializeObject(Of List(Of StazioneAliasNew))(json)

        If objAlias.Any() Then

            Return objAlias(0)
        End If

        Return Nothing
    End Function



    Public Function UpsertAlias(piva_superuser As String, piva As String, id As Integer, strAlias As String, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As StazioneAlias

        Dim url As String = LeggiUrlMeteo(objParametri_Server)
        url &= "/UpsertAlias"

        Dim hlpHttp As New Http

        Dim objParams As New JObject(
            New JProperty("PIVA_Superuser", piva_superuser),
            New JProperty("PIVA", piva),
            New JProperty("Id", id),
            New JProperty("Alias", strAlias))

        Dim inputParams As New JObject
        inputParams("inputParams") = MeteoWSParametri.Encrypt(objParams)

        Dim risp As String = hlpHttp.chiamaWS(inputParams.ToString, Nothing, url, "application/json", "POST", "application/json", "", TimeOutRichiesta:=600)

        Dim objRisp = JObject.Parse(risp)

        Dim json = objRisp("UpsertAliasResult")("RispostaStringa").ToString

        Dim objAlias = JsonConvert.DeserializeObject(Of List(Of StazioneAlias))(json)

        If objAlias.Any() Then

            Return objAlias(0)
        End If

        Return Nothing
    End Function



    Public Function DeleteAlias(piva_superuser As String, piva As String, id As Integer, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim url As String = LeggiUrlMeteo(objParametri_Server)
        url &= "/DeleteAlias"

        Dim hlpHttp As New Http

        Dim objParams As New JObject(
            New JProperty("PIVA_Superuser", piva_superuser),
            New JProperty("PIVA", piva),
            New JProperty("Id", id))

        Dim inputParams As New JObject
        inputParams("inputParams") = MeteoWSParametri.Encrypt(objParams)

        Dim risp As String = hlpHttp.chiamaWS(inputParams.ToString, Nothing, url, "application/json", "POST", "application/json", "", TimeOutRichiesta:=600)

        Dim objRisp = JObject.Parse(risp)

        Dim json = objRisp("DeleteAliasResult")("RispostaStringa").ToString

        Dim obj = New With {.Deleted = 0}
        Dim list = {obj}.ToList()
        list = JsonConvert.DeserializeAnonymousType(json, list)

        Return list.Any() AndAlso list(0).Deleted > 0
    End Function

#End Region

End Class
