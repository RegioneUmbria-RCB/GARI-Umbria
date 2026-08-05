Imports AgronicaCoreModelsSTD.metaschema.utilizzi
Imports AgronicaCoreModelsSTD.attivita.dettagli
Imports AgronicaCoreModelsSTD.metaschema
Imports System.Net
Imports AgronicaCoreModelsSTD.metaschema.avversita
Imports Newtonsoft.Json.Linq
Imports System.Text.RegularExpressions
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD.AgronicaChatGPT
Imports Newtonsoft.Json
Imports AgronicaCoreModelsSTD.Widgets
Imports System.Reflection
Imports System.Text

Public Class AgroChatGPT

    Private OPENAI_API_KEY As String = ""

    Private MODEL As String = ""

    Private ASSISTANT_ID As String = ""

    Private Const BASE_URL_OPEN_AI = "https://api.openai.com/v1/"

    Private http_Request As New AgronicaCoreUtility.Http

    Public Function CheckTrattamento(ByVal utilizzoTerreno As UtilizzoTerreno, ByVal dettaglioTrattamento As DettaglioTrattamento, ByVal disciplinare As Disciplinare, ByVal avversita As AvversitaGruppo, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim risp As String = ""

        ReadChatGPTConfig("", objParametri_Server)

        Dim sQuestion As String = PrepareQuestion(utilizzoTerreno, dettaglioTrattamento, disciplinare, avversita)

        Dim request As String = ""
        request = "{"
        request += " ""model"":""" & MODEL & ""","
        request += " ""messages"": [{""role"":""user"", ""content"": """ & sQuestion & """}]"
        request += "}"

        Dim completeUrl As String = BASE_URL_OPEN_AI + "chat/completions"

        Dim StrResp = http_Request.chiamaWS(request, Nothing, completeUrl, "application/json", "POST", "application/json", "", GetHeader())

        If Not String.IsNullOrEmpty(StrResp) Then
            Dim RespObj = JObject.Parse(StrResp)

            If Not IsNothing(RespObj) AndAlso Not IsNothing(RespObj("choices")) Then
                Dim content As JArray = RespObj("choices")

                If content.Count = 1 Then
                    risp = content(0)("message")("content")
                End If
            End If
        End If



        Return risp
    End Function

    Public Function getCostsAndRevenuesEstimates(ByVal dettCrops As AgronicaCoreModelsSTD.Widgets.PrevisioniChatGPT_IN, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim risp As String = ""

        ReadChatGPTConfig("", objParametri_Server)

        Dim jsonArrayCrops As String = Newtonsoft.Json.JsonConvert.SerializeObject(dettCrops.arrayCrops)
        jsonArrayCrops = jsonArrayCrops.Replace("""", "'")
        Dim typeResponseCosts As String = "public class CostsCrop { public string id; public string description; public double costs; public string explanation }"  'GetClassDefinition(Of CostsCrop)()
        Dim typeResponseRevenues As String = "public class RevenuesCrop { public string id; public string description; public double revenues; public string explanation; }" 'GetClassDefinition(Of RevenuesCrop)()

        Dim systemContest As String = String.Format(My.Resources.Agronica_ChatGPT.contestoSystemPrevisioniCostiRicavi)
        Dim questionCosts As String = String.Format(My.Resources.Agronica_ChatGPT.RichiestaSulCalcoloDeiCosti, dettCrops.year, jsonArrayCrops, typeResponseCosts)
        Dim questionRevenues As String = String.Format(My.Resources.Agronica_ChatGPT.RichiestaSulCalcoloDeiRicavi, dettCrops.year, jsonArrayCrops, typeResponseRevenues)

        Dim temperature As Double = 0.2
        Dim CostsEstimates As String = getResponseFromChatGPT(systemContest, questionCosts, temperature).ToString().Replace("```json", "").Replace("```", "").Trim()
        Dim RevenuesEstimates As String = getResponseFromChatGPT(systemContest, questionRevenues, temperature).ToString().Replace("```json", "").Replace("```", "").Trim()

        'una volta ottenuti costi e ricavi dell'anno in corso, calcolarsi l'ammontare di tutto
        Dim Costs As CostsCrop() = JsonConvert.DeserializeObject(Of CostsCrop())(CostsEstimates)
        Dim Revenues As RevenuesCrop() = JsonConvert.DeserializeObject(Of RevenuesCrop())(RevenuesEstimates)

        Dim TotalCost As Double = 0
        Dim TotalRevenue As Double = 0

        For Each cost In Costs
            TotalCost += cost.costs
        Next

        For Each revenue In Revenues
            TotalRevenue += revenue.revenues
        Next

        Dim typeResponseBalances As String = "public class Balance { public int year; public double costs; public double revenues; public string explanation; }" 'GetClassDefinition(Of Balance)()
        Dim questionBalanceFiveYears As String = String.Format(My.Resources.Agronica_ChatGPT.RichiestaSulCalcoloBilancioPluriennale, dettCrops.year, TotalCost, TotalRevenue, jsonArrayCrops, typeResponseBalances)

        Dim balanceFiveYearsEstimate As String = getResponseFromChatGPT(systemContest, questionBalanceFiveYears, temperature).ToString().Replace("```json", "").Replace("```", "").Trim()
        Dim BalanceFiveYears As Balance() = JsonConvert.DeserializeObject(Of Balance())(balanceFiveYearsEstimate)

        Dim resultObject As New CostsRevenuesTot

        resultObject.Balances = BalanceFiveYears
        resultObject.CostsCrops = Costs
        resultObject.RevenuesCrops = Revenues

        risp = JsonConvert.SerializeObject(resultObject)

        Return risp

    End Function

    'Private Function PrepareQuestionForCostsAndRevenues(dettCrops() As Widget_PrevisioniAI) As String

    '    If dettCrops Is Nothing OrElse dettCrops.Length = 0 Then
    '        Return ""
    '    End If

    '    ' Serializza l'array in formato JSON
    '    Dim jsonArrayCrops As String = Newtonsoft.Json.JsonConvert.SerializeObject(dettCrops)

    '    Dim typeResponseBalance As String = " export interface Balance { year: number, costs: number, revenues: number }[] "
    '    Dim typeResponseCosts As String = " export interface CostsCrop { id: string, description: string, costs: number, explanation: string }[]"
    '    Dim typeResponseRevenues As String = " export interface RevenuesCrop { id: string, description: string, revenues: number, explanation: string }[]"
    '    Dim typeResponseTot As String = " export interface CostsRevenuesTot { Balances: Balance[], CostsCrops: CostsCrop[], RevenuesCrops: RevenueCrop[] }"

    '    ' Costruisci la stringa del messaggio
    '    Dim question As String = "Ecco un array di campi. Per ogni campo, calcola i costi e i ricavi, includendo costi per lavoratori, macchine, trattamenti, e considera eventuali fluttuazioni di mercato, la possibile resa di ogni coltura e eventuali incentivi statali e/o continentali (esempio: UE). Considera quindi la localita di provenienza e puoi aiutarti con il WEB per fare una ricerca più mirata riguardo costi e ricavi. La superficie è espressa in ettari. Questi sono i dettagli dei campi: " & jsonArrayCrops
    '    question += " Restituisci il risultato dei costi come un array definito così, in formato JSON (ogni elemento dell'array corrisponde ai costi di un appezzamento di una certa coltura, con 'id' la chiave di questo, nella descrizione il NomeApp, nel campo 'costs' l'ammontare per quell'appezzamento dei costi e nel campo 'explanations' come è stato calcolato in dettaglio quel costo): " & typeResponseCosts
    '    question += " e restituisci il risultato dei costi come un array definito così, in formato JSON (ogni elemento dell'array corrisponde ai costi di un appezzamento di una certa coltura, con 'id' la chiave di questo, nella descrizione il NomeApp, nel campo 'revenues' l'ammontare per quell'appezzamento dei ricavi e nel campo 'explanations' come è stato calcolato in dettaglio quel ricavo): " & typeResponseRevenues
    '    question += " Infine, stima costi e ricavi da qui a 5 anni e, per ogni anno, sintetizzalo in un array, in formato JSON, così espresso " & typeResponseBalance
    '    question += " Poni quindi tutto in un formato JSON così, strutturato " & typeResponseTot

    '    Return question

    'End Function

    'Private Function getCostEstimates(dettCrops() As Widget_PrevisioniAI)

    '    Dim risp As String = ""

    '    ' Serializza l'array in formato JSON
    '    Dim jsonArrayCrops As String = Newtonsoft.Json.JsonConvert.SerializeObject(dettCrops)
    '    Dim typeResponseCosts As String = GetClassDefinition(Of CostsCrop)()

    '    ' Costruisci la stringa del messaggio
    '    Dim question As String = "Fai parte di un'app per il calcolo delle stime per aziende agricole. Calcola e restituisci, per un'azienda agricola con le seguenti colture, la ripartizione stimata dei costi per l'anno fiscale in corso. "
    '    question += " Considera che ti viene passata la specie vegetale, la località in cui l'appezzamento risiede, la superficie espressa in ettari e se l'agricoltura è BIO. Calcola quindi il costo dei trattamenti delle colture, del lavoro necessario "
    '    question += " affinchè i campi rendano il più possibile, considerando lavoratori e macchinari necessari a tutto ciò. Inoltre, considera che le attrezzature e i lavoratori non operano o lavorano per tutto l'anno, ma solo per le ore necessarie in base alle colture e alla superficie indicata. "
    '    question += " Descrivi i passaggi e i calcoli utilizzati (puoi cercare su internet un esempio più preciso di stima dei costi e riportarne la fonte) nel campo 'explanation'. Questa spiegazione dovrà essere nella lingua di questo messaggio e servirà come guida per l'utente, in modo che possa comprendere meglio il funzionamento del calcolo. "
    '    question += " Inserisci poi i costi nel campo 'costs', nella 'description' l'appezzamento a cui è attribuito quel costo e per 'id' un identificativo univoco. "
    '    question += " Ecco le colture dell'azienda: " & jsonArrayCrops
    '    question += " Restituirai la risposta in formato JSON secondo la seguente definizione TypeScript: " & typeResponseCosts

    '    Dim request As String = ""
    '    request = "{"
    '    request += " ""model"":""" & MODEL & ""","
    '    request += " ""messages"": [ "
    '    request += " ""{""role"":""system"", ""content"": ""Sei un esperto in analisi economiche per aziende agricole. Riceverai un array di campi e dovrai stimare i costi, per l'anno finanziario corrente""},"
    '    request += " ""{""role"":""user"", ""content"": """ & question & """}]"
    '    request += "}"

    '    Dim completeUrl As String = BASE_URL_OPEN_AI + "chat/completions"

    '    Dim StrResp = http_Request.chiamaWS(request, Nothing, completeUrl, "application/json", "POST", "application/json", "", GetHeader())

    '    If Not String.IsNullOrEmpty(StrResp) Then
    '        Dim RespObj = JObject.Parse(StrResp)

    '        If Not IsNothing(RespObj) AndAlso Not IsNothing(RespObj("choices")) Then
    '            Dim content As JArray = RespObj("choices")

    '            If content.Count = 1 Then
    '                risp = content(0)("message")("content")
    '            End If
    '        End If
    '    End If

    '    Return risp

    'End Function

    Private Function getResponseFromChatGPT(systemContest As String,
                                            question As String,
                                            Optional temperature As Double? = Nothing) As String

        Dim risp As String = ""

        '' Serializza l'array in formato JSON
        'Dim jsonArrayCrops As String = Newtonsoft.Json.JsonConvert.SerializeObject(dettCrops)
        'Dim typeResponseRevenues As String = GetClassDefinition(Of RevenuesCrop)()

        '' Costruisci la stringa del messaggio
        'Dim question As String = " Fai parte di un'app di calcolo delle stime per le aziende agricole, localizzata in italiano (quindi anche le proprietà dell'azienda agricola fornite saranno in italiano). Calcola e restituisci, per un'azienda agricola con le seguenti proprietà, la ripartizione stimata dei ricavi per l'anno fiscale in corso. "
        'question += " Considera che ti viene passata la specie vegetale, la località in cui l'appezzamento risiede, la superficie espressa in ettari e se l'agricoltura è BIO. Calcola quindi la possibile resa dei campi, la quantità del raccolto ottenuto "
        'question += " in base alla collocazione geografica e al fatto che la agricoltura sia BIO o meno. Inoltre, includi eventuali incentivi agricoli previsti continentali e/o statali. "
        'question += " Descrivi i passaggi e i calcoli utilizzati (puoi cercare su internet un esempio più preciso di stima dei costi e riportarne la fonte) nel campo 'explanation'. Questa spiegazione dovrà essere nella lingua di questo messaggio e servirà come guida per l'utente, in modo che possa comprendere meglio il funzionamento del calcolo. "
        'question += " Inserisci poi i ricavi nel campo 'revenues', nella 'description' l'appezzamento a cui è attribuito quel ricavo e per 'id' un identificativo univoco. "
        'question += " Ecco le colture dell'azienda: " & jsonArrayCrops
        'question += " Restituirai la risposta in formato JSON secondo la seguente definizione TypeScript: " & typeResponseRevenues

        Dim request As String = ""
        request = "{"
        request += " ""model"":""" & MODEL & ""","
        request += " ""messages"": [ "
        request += " {""role"":""system"", ""content"": """ & systemContest & """},"
        request += " {""role"":""user"", ""content"": """ & question & """}]"

        If temperature.HasValue Then
            request += " ,""temperature"": " & temperature.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) & " "
        End If

        'If responseJSON = True Then
        '    request += " ,""response_format"": ""json"" "
        'End If

        request += "}"

        Dim completeUrl As String = BASE_URL_OPEN_AI + "chat/completions"

        Dim StrResp = http_Request.chiamaWS(request, Nothing, completeUrl, "application/json", "POST", "application/json", "", GetHeader())

        If Not String.IsNullOrEmpty(StrResp) Then
            Dim RespObj = JObject.Parse(StrResp)

            If Not IsNothing(RespObj) AndAlso Not IsNothing(RespObj("choices")) Then
                Dim content As JArray = RespObj("choices")

                If content.Count = 1 Then
                    risp = content(0)("message")("content")
                End If
            End If
        End If

        Return risp

    End Function

    Private Function GetClassDefinition(Of T)() As String
        Dim type As Type = GetType(T)
        Dim sb As String = ""
        'Dim sb As New StringBuilder()

        'sb.AppendLine("public class " & type.Name & " {")
        'For Each prop As PropertyInfo In type.GetProperties()
        '    sb.AppendLine("    public string " & prop.Name & ";")
        'Next
        'sb.AppendLine("}")

        sb += "public class " & type.Name & " {"
        For Each prop As PropertyInfo In type.GetProperties()
            sb += " public string " & prop.Name & ";"
        Next
        sb += "}"

        'Return sb.ToString()

        Return sb
    End Function

    Public Function CheckTrattamentoByProfitosanDPIRegionale(ByVal utilizzoTerreno As UtilizzoTerreno, ByVal dettaglioTrattamento As DettaglioTrattamento, ByVal disciplinare As Disciplinare, ByVal avversita As AvversitaGruppo, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim response As String = ""

        Dim thread_id As String = ""

        Try

            ReadChatGPTConfig(Assistant_Name.ProfitosanDPIRegionale, objParametri_Server)

            thread_id = CreateThreadWithMessage(utilizzoTerreno, dettaglioTrattamento, disciplinare, avversita)

            response = CreateRunWithStream(thread_id)

        Catch ex As Exception
            Throw ex
        Finally
            If Not String.IsNullOrEmpty(thread_id) Then
                DeleteThread(thread_id)
            End If
        End Try

        Return response
    End Function

    Private Function PrepareQuestion(ByVal utilizzoTerreno As UtilizzoTerreno, ByVal dettaglioTrattamento As DettaglioTrattamento, ByVal disciplinare As Disciplinare, ByVal avversita As AvversitaGruppo) As String

        Dim Question As String = ""

        Dim Specie_Descrizione As String = ""

        Dim Avversita_Descrizione As String = ""

        Dim Prodotto_Descrizione As String = ""

        Dim DestinazioneUso_Descrizione As String = ""

        Dim DPI_Regionale_Descrizione As String = ""

        If Not IsNothing(utilizzoTerreno) Then
            Select Case utilizzoTerreno.classType
                Case AgronicaCoreModelsSTD.costanti.ClassType.Varieta
                    Dim varieta = CType(utilizzoTerreno, Varieta)
                    Specie_Descrizione = varieta.specie.descrizione
                Case AgronicaCoreModelsSTD.costanti.ClassType.DestinazioneUso
                    Dim destinazioneUSo = CType(utilizzoTerreno, DestinazioneUso)
                    DestinazioneUso_Descrizione = destinazioneUSo.descrizione
            End Select
        End If

        If Not IsNothing(avversita) Then
            Avversita_Descrizione = avversita.descrizione
        End If

        If Not IsNothing(dettaglioTrattamento) AndAlso Not IsNothing(dettaglioTrattamento.prodotto) Then
            Prodotto_Descrizione = dettaglioTrattamento.prodotto.descrizione
        End If

        If Not IsNothing(disciplinare) Then
            Dim disciplinare_codice As Integer = CInt(disciplinare.codice)
            If disciplinare_codice <> 0 AndAlso disciplinare_codice <> enum_Disciplinare_Operazione.NessunDpiNessunaEtichetta AndAlso disciplinare_codice <> enum_Disciplinare_Operazione.Nessuno AndAlso disciplinare_codice <> enum_Disciplinare_Operazione.Biologico Then
                DPI_Regionale_Descrizione = disciplinare.descrizione
            End If
        End If

        If Not String.IsNullOrEmpty(Specie_Descrizione) Then

            If Not String.IsNullOrEmpty(Prodotto_Descrizione) AndAlso Not String.IsNullOrEmpty(Avversita_Descrizione) Then

                If Not String.IsNullOrEmpty(DPI_Regionale_Descrizione) Then
                    Question = String.Format(My.Resources.Agronica_ChatGPT.SecondoDPICorrettoUtilizzareProdottoPerAvversitaNellaSpecie, Prodotto_Descrizione, Avversita_Descrizione, Specie_Descrizione, DPI_Regionale_Descrizione)
                Else
                    Question = String.Format(My.Resources.Agronica_ChatGPT.CorrettoUtilizzareProdottoPerAvversitaNellaSpecie, Prodotto_Descrizione, Avversita_Descrizione, Specie_Descrizione)
                End If

            Else

                If Not String.IsNullOrEmpty(Prodotto_Descrizione) AndAlso String.IsNullOrEmpty(Avversita_Descrizione) Then

                    If Not String.IsNullOrEmpty(DPI_Regionale_Descrizione) Then
                        Question = String.Format(My.Resources.Agronica_ChatGPT.SecondoDPIQualiAvversitaCombattoConProdottoNellaSpecie, Prodotto_Descrizione, Specie_Descrizione, DPI_Regionale_Descrizione)
                    Else
                        Question = String.Format(My.Resources.Agronica_ChatGPT.QualiAvversitaCombattoConProdottoNellaSpecie, Prodotto_Descrizione, Specie_Descrizione)
                    End If

                End If

                If Not String.IsNullOrEmpty(Avversita_Descrizione) AndAlso String.IsNullOrEmpty(Prodotto_Descrizione) Then

                    If Not String.IsNullOrEmpty(DPI_Regionale_Descrizione) Then
                        Question = String.Format(My.Resources.Agronica_ChatGPT.SecondoDPIQualiProdottiUtilizzoPerCombattereAvversitaNellaSpecie, Avversita_Descrizione, Specie_Descrizione, DPI_Regionale_Descrizione)
                    Else
                        Question = String.Format(My.Resources.Agronica_ChatGPT.QualiProdottiUtilizzoPerCombattereAvversitaNellaSpecie, Avversita_Descrizione, Specie_Descrizione)
                    End If

                End If

            End If

        ElseIf Not String.IsNullOrEmpty(DestinazioneUso_Descrizione) Then

            If Not String.IsNullOrEmpty(Prodotto_Descrizione) AndAlso Not String.IsNullOrEmpty(Avversita_Descrizione) Then

                If Not String.IsNullOrEmpty(DPI_Regionale_Descrizione) Then
                    Question = String.Format(My.Resources.Agronica_ChatGPT.SecondoDPICorrettoUtilizzareProdottoPerAvversitaNellaDestinazione, Prodotto_Descrizione, Avversita_Descrizione, DestinazioneUso_Descrizione, DPI_Regionale_Descrizione)
                Else
                    Question = String.Format(My.Resources.Agronica_ChatGPT.CorrettoUtilizzareProdottoPerAvversitaNellaDestinazione, Prodotto_Descrizione, Avversita_Descrizione, DestinazioneUso_Descrizione)
                End If

            Else

                If Not String.IsNullOrEmpty(Prodotto_Descrizione) AndAlso String.IsNullOrEmpty(Avversita_Descrizione) Then

                    If Not String.IsNullOrEmpty(DPI_Regionale_Descrizione) Then
                        Question = String.Format(My.Resources.Agronica_ChatGPT.SecondoDPIQualiAvversitaCombattoConProdottoNellaDestinazione, Prodotto_Descrizione, DestinazioneUso_Descrizione, DPI_Regionale_Descrizione)
                    Else
                        Question = String.Format(My.Resources.Agronica_ChatGPT.QualiAvversitaCombattoConProdottoNellaDestinazione, Prodotto_Descrizione, DestinazioneUso_Descrizione)
                    End If


                End If

                If Not String.IsNullOrEmpty(Avversita_Descrizione) AndAlso String.IsNullOrEmpty(Prodotto_Descrizione) Then

                    If Not String.IsNullOrEmpty(DPI_Regionale_Descrizione) Then
                        Question = String.Format(My.Resources.Agronica_ChatGPT.SecondoDPIQualiProdottiUtilizzoPerCombattereAvversitaNellaSpecie, Avversita_Descrizione, DestinazioneUso_Descrizione, DPI_Regionale_Descrizione)
                    Else
                        Question = String.Format(My.Resources.Agronica_ChatGPT.QualiProdottiUtilizzoPerCombattereAvversitaNellaSpecie, Avversita_Descrizione, DestinazioneUso_Descrizione)
                    End If

                End If

            End If

        End If

        Return Question

    End Function

    Private Function CreateThreadWithMessage(ByVal utilizzoTerreno As UtilizzoTerreno, ByVal dettaglioTrattamento As DettaglioTrattamento, ByVal disciplinare As Disciplinare, ByVal avversita As AvversitaGruppo) As String

        Dim thread_id As String = ""

        Dim completeUrl As String = BASE_URL_OPEN_AI + "threads"

        Dim question As String = PrepareQuestion(utilizzoTerreno, dettaglioTrattamento, disciplinare, avversita)

        If Not String.IsNullOrEmpty(question) Then

            Dim request As String = ""
            request = "{"
            request += " ""messages"": [{""role"":""user"", ""content"": """ & question & """}]"
            request += "}"

            Dim StrResp = http_Request.chiamaWS(request, Nothing, completeUrl, "application/json", "POST", "application/json", "", GetHeader())

            If Not String.IsNullOrEmpty(StrResp) Then
                Dim RespObj = JObject.Parse(StrResp)

                If Not IsNothing(RespObj) AndAlso Not IsNothing(RespObj("id")) AndAlso Not String.IsNullOrEmpty(RespObj("id")) Then
                    thread_id = RespObj("id")
                End If
            End If

        End If

        Return thread_id
    End Function

    Private Function CreateRun(ByVal thread_id As String, ByVal assistant_id As String, ByVal model As String) As String

        Dim run_id As String = ""

        Dim completeUrl As String = BASE_URL_OPEN_AI + "threads/" & thread_id & "/runs"

        Dim request As String = ""
        request = "{"
        request += " ""assistant_id"": """ & assistant_id & ""","
        request += " ""model"": """ & model & """"
        request += "}"

        Dim StrResp = http_Request.chiamaWS(request, Nothing, completeUrl, "application/json", "POST", "application/json", "", GetHeader())

        If Not String.IsNullOrEmpty(StrResp) Then

            Dim RespObj = JObject.Parse(StrResp)

            If Not IsNothing(RespObj) AndAlso Not IsNothing(RespObj("id")) AndAlso Not String.IsNullOrEmpty(RespObj("id")) Then
                run_id = RespObj("id")
            End If
        End If

        Return run_id
    End Function

    Private Function CreateRunWithStream(ByVal thread_id As String)
        Dim response As String = ""

        Dim completeUrl As String = BASE_URL_OPEN_AI + "threads/" & thread_id & "/runs"

        Dim request As String = ""
        request = "{"
        request += " ""assistant_id"": """ & ASSISTANT_ID & ""","
        request += " ""model"": """ & MODEL & ""","
        request += " ""stream"": true"
        request += "}"

        Dim StrResp = http_Request.chiamaWS(request, Nothing, completeUrl, "application/json", "POST", "application/json", "", GetHeader())

        If Not String.IsNullOrEmpty(StrResp) Then

            If StrResp.Contains("event: done") AndAlso StrResp.Contains("event: thread.message.completed") Then

                Dim m = Regex.Match(StrResp, "thread.message.completed\ndata: (.*)", RegexOptions.None, TimeSpan.FromSeconds(3))

                If m.Success AndAlso Not IsNothing(m.Groups) AndAlso m.Groups.Count = 2 Then
                    response = GetResponseByAssistant(m.Groups(1).Value)
                End If
            End If
        End If

        Return response
    End Function

    Private Function CheckResponse(ByVal thread_id As String, ByVal run_id As String) As String

        Dim response As String = ""

        Dim status As String = CheckRun(thread_id, run_id)

        If status = "completed" Then
            response = CheckMessage(thread_id)
        End If

        Return response

    End Function

    Private Function CheckRun(ByVal thread_id As String, ByVal run_id As String) As String

        Dim status As String = ""

        Dim completeUrl As String = BASE_URL_OPEN_AI + "threads/" & thread_id & "/runs/" & run_id

        Dim request As String = ""

        Dim StrResp = http_Request.chiamaWS(request, Nothing, completeUrl, "application/json", "GET", "application/json", "", GetHeader())

        If Not String.IsNullOrEmpty(StrResp) Then
            Dim RespObj = JObject.Parse(StrResp)

            If Not IsNothing(RespObj) Then

                status = RespObj("status")

                'If status <> "completed" Then
                '    CheckRun(thread_id, run_id)
                'End If

            End If
        End If

        Return status
    End Function

    Private Function CheckMessage(ByVal thread_id As String)

        Dim response As String = ""

        Dim completeUrl As String = BASE_URL_OPEN_AI + "threads/" & thread_id & "/messages"

        Dim request As String = ""

        Dim StrResp = http_Request.chiamaWS(request, Nothing, completeUrl, "application/json", "GET", "application/json", "", GetHeader())

        response = GetResponse(StrResp)

        Return response
    End Function

    Private Function GetResponse(ByVal StrResp As String) As String

        Dim response As String = ""

        If Not String.IsNullOrEmpty(StrResp) Then
            Dim RespObj = JObject.Parse(StrResp)

            If Not IsNothing(RespObj) AndAlso Not IsNothing(RespObj("data")) Then

                Dim JData As JArray = JArray.Parse(RespObj("data"))

                If Not IsNothing(JData) AndAlso JData.Count > 0 Then
                    For Each JOb In JData
                        If JOb("role") = "assistant" Then
                            Dim JContents = JArray.Parse(RespObj("content"))

                            If Not IsNothing(JContents) AndAlso JContents.Count > 0 Then
                                For Each JContent In JContents
                                    If JContent("type") = "text" Then
                                        response = JContent("text")("value")
                                        Return response
                                    End If
                                Next
                            End If
                        End If
                    Next
                End If

                response = RespObj("id")
            End If
        End If

        Return response
    End Function


    Private Function GetResponseByAssistant(ByVal StrResp As String) As String
        Dim response As String = ""

        If Not String.IsNullOrEmpty(StrResp) Then
            Dim RespObj = JObject.Parse(StrResp)

            If Not IsNothing(RespObj) AndAlso Not IsNothing(RespObj("role")) Then

                If RespObj("role") = "assistant" Then
                    Dim JContents = JArray.Parse(RespObj("content").ToString())

                    If Not IsNothing(JContents) AndAlso JContents.Count > 0 Then
                        For Each JContent In JContents
                            If Not IsNothing(JContent("type")) AndAlso JContent("type") = "text" AndAlso Not IsNothing(JContent("text")("value")) Then
                                response = JContent("text")("value")
                                Return response
                            End If
                        Next
                    End If
                End If
            End If
        End If

        Return response
    End Function

    Private Sub DeleteThread(ByVal thread_id As String)
        Dim completeUrl As String = BASE_URL_OPEN_AI + "threads/" & thread_id

        Dim request As String = ""

        http_Request.chiamaWS(request, Nothing, completeUrl, "application/json", "GET", "application/json", "", GetHeader())
    End Sub

    Private Function GetHeader() As WebHeaderCollection

        Dim hdr As New WebHeaderCollection()

        hdr.Add("Authorization", "Bearer " + OPENAI_API_KEY)
        hdr.Add("OpenAI-Beta", "assistants=v2")

        Return hdr
    End Function

    Private Sub ReadChatGPTConfig(ByVal assistant_name As String, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim Obj_Config_Siti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim config = Obj_Config_Siti.Leggi_Valore(0, "ConfigChatGPT", "", "", objParametri_Server)

        If String.IsNullOrEmpty(config) Then
            Throw New Exception("Chiave ConfigChatGPT in Configurazione_Siti non impostata")
        End If

        Dim GPTConfig = JsonConvert.DeserializeObject(Of Config)(config.ToString())

        If IsNothing(GPTConfig) Then
            Throw New Exception("Chiave ConfigChatGPT in Configurazione_Siti non impostata correttamente")
        End If

        If Not String.IsNullOrEmpty(assistant_name) Then

            If IsNothing(GPTConfig.assistants) Then
                Throw New Exception("Assistants in ConfigChatGPT in Configurazione_Siti non presenti")
            End If

            Dim index = GPTConfig.assistants.FindIndex(Function(a) a.assistant_name = assistant_name)

            If index = -1 Then
                Throw New Exception("Assistant " & assistant_name & " in ConfigChatGPT in Configurazione_Siti non presente")
            End If

            OPENAI_API_KEY = AgronicaCoreUtility.AgroZip.DeCompressioneBase64(1, GPTConfig.assistants(index).assistant_api_key)

            MODEL = GPTConfig.assistants(index).model

            ASSISTANT_ID = AgronicaCoreUtility.AgroZip.DeCompressioneBase64(1, GPTConfig.assistants(index).assistant_id)

            If String.IsNullOrEmpty(ASSISTANT_ID) Then
                Throw New Exception("Assistant ID in ConfigChatGPT in Configurazione_Siti non valorizzato")
            End If

        Else

            OPENAI_API_KEY = AgronicaCoreUtility.AgroZip.DeCompressioneBase64(1, GPTConfig.open_api_key)

            MODEL = GPTConfig.model

        End If

        If String.IsNullOrEmpty(OPENAI_API_KEY) Then
            Throw New Exception("API Key in ConfigChatGPT in Configurazione_Siti non valorizzato")
        End If

        If String.IsNullOrEmpty(MODEL) Then
            Throw New Exception("Model in ConfigChatGPT in Configurazione_Siti non valorizzato")
        End If

    End Sub

End Class