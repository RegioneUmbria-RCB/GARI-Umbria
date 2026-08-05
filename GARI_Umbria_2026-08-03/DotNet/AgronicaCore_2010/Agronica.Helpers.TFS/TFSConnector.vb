Imports System.Text
Imports AgronicaCoreVarieBIZ
Imports Microsoft.TeamFoundation.WorkItemTracking.Client
Imports Microsoft.TeamFoundation.WorkItemTracking.WebApi
Imports Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models
Imports Microsoft.VisualStudio.Services.Client
Imports Microsoft.VisualStudio.Services.WebApi
Imports Microsoft.VisualStudio.Services.WebApi.Patch
Imports Microsoft.VisualStudio.Services.WebApi.Patch.Json

Public Class TFSConnector

    Private _vstsCollectioUrl As String = ""
    Private _connection As VssConnection = Nothing
    Private _witClient As WorkItemTrackingHttpClient = Nothing
    Private _wistore As WorkItemStore

    Public Sub New(vstsCollectioUrl)
        _vstsCollectioUrl = vstsCollectioUrl
        If _connection Is Nothing Then
            _connection = New VssConnection(New Uri(_vstsCollectioUrl), New VssClientCredentials())
        End If
        If _witClient Is Nothing Then
            _witClient = _connection.GetClient(Of WorkItemTrackingHttpClient)()
        End If
        'If _wistore Is Nothing Then
        '    _wistore = New WorkItemStore(_vstsCollectioUrl)
        'End If
    End Sub

    Public Function AggiornaCampiWorkItem(inData As AggiornaCampiWorkItemIn) As RispostaStandard

        Dim r As New RispostaStandard
        Try

            Dim patchDocument As New JsonPatchDocument

            For Each campoDaAggiornare In inData.listaCampiDaAggiornare

                Dim op As Operation
                If campoDaAggiornare.valoreTrovatoSuTFS Then
                    op = Operation.Replace
                Else
                    op = Operation.Add
                End If

                Dim val1 As Object
                Dim aggiorna As Boolean = False
                Select Case campoDaAggiornare.campo
                    Case "Microsoft.VSTS.Common.StackRank"
                        val1 = CDbl(campoDaAggiornare.valore)
                    Case "Microsoft.VSTS.Scheduling.OriginalEstimate"
                        val1 = CDbl(campoDaAggiornare.valore)
                    Case "Microsoft.VSTS.Scheduling.RemainingWork"
                        val1 = CDbl(campoDaAggiornare.valore)
                    Case Else
                        Throw New Exception("Il campo """ & campoDaAggiornare.campo & """ non è aggiornabile da questa procedura.")
                End Select

                Dim patchOp As New JsonPatchOperation With {
                    .Operation = op,
                    .Path = "/fields/" & campoDaAggiornare.campo,
                    .Value = val1
                }

                patchDocument.Add(patchOp)

            Next
            'campo

            Dim result As Models.WorkItem = _witClient.UpdateWorkItemAsync(patchDocument, inData.id).Result

            r.RispostaOK = True
            r.RispostaStringa = ""

        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = Nothing
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function


    Public Function ListaWorkItems(idDaRicercare As Integer, inData As ConfigurazioneWIQL, tipoEstrazione As TipoEstrazione) As rispostaStandard(Of List(Of SmallWorkItem))

        Dim r As New rispostaStandard(Of List(Of SmallWorkItem))
        r.RispostaStringa = New List(Of SmallWorkItem)
        Try

            Dim ListaCampiDaLeggere As String = "id,Microsoft.VSTS.Scheduling.RemainingWork,Microsoft.VSTS.Common.StackRank,Microsoft.VSTS.Scheduling.OriginalEstimate,System.AssignedTo"
            Dim sQuery As String
            Dim queryType As QueryResultType = QueryResultType.WorkItem
            Select Case tipoEstrazione
                Case TipoEstrazione.Attivita
                    sQuery = SommatoriaTempiConFiltriWIQY(inData)
                Case TipoEstrazione.Funzionalita
                    sQuery = ListaFunzionalitaConFiltriWIQY(inData)
                Case TipoEstrazione.Gerarchia
                    sQuery = ListaFigliWIQY(idDaRicercare, inData)
                    queryType = QueryResultType.WorkItemLink
            End Select


            Dim WITList As List(Of Models.WorkItem) = OttiemiWorkItemsDaQueryWIQL(sQuery, ListaCampiDaLeggere, queryType)

            'per ciascun workitem in tfs leggo i dati
            If Not WITList Is Nothing Then

                For Each ww In WITList

                    Dim smallItem As New SmallWorkItem With {
                        .id = ww.Id,
                        .Trovato_OreRimanenti = ww.Fields.ContainsKey("Microsoft.VSTS.Scheduling.RemainingWork"),
                        .Trovato_StimaOriginale = ww.Fields.ContainsKey("Microsoft.VSTS.Scheduling.OriginalEstimate"),
                        .Trovato_OrdineDiPriorita = ww.Fields.ContainsKey("Microsoft.VSTS.Common.StackRank"),
                        .Trovato_AssegnatoA = ww.Fields.ContainsKey("System.AssignedTo")
                    }

                    Dim v1 As Double = 0
                    If smallItem.Trovato_OreRimanenti Then
                        v1 = SommatoriaTempiConFiltriLeggiDouble("Microsoft.VSTS.Scheduling.RemainingWork", ww)
                    End If

                    Dim v2 As Double = 0
                    If smallItem.Trovato_OrdineDiPriorita Then
                        v2 = SommatoriaTempiConFiltriLeggiDouble("Microsoft.VSTS.Common.StackRank", ww)
                    End If


                    Dim v3 As Double = 0
                    If smallItem.Trovato_StimaOriginale Then
                        v3 = SommatoriaTempiConFiltriLeggiDouble("Microsoft.VSTS.Scheduling.OriginalEstimate", ww)
                    End If

                    smallItem.OreRimanenti = v1
                    smallItem.OrdineDiPriorita = v2
                    smallItem.StimaOriginale = v3

                    If smallItem.Trovato_AssegnatoA Then
                        LeggiAssegnatoA(ww, smallItem)
                    End If

                    r.RispostaStringa.Add(smallItem)

                Next

            End If


        Catch ex As Exception
            r.RispostaOK = False
            r.RispostaStringa = Nothing
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function
    Public Function SommatoriaTempiConFiltri(inData As ConfigurazioneWIQL) As rispostaStandard(Of Double)
        Dim r As New rispostaStandard(Of Double)

        Try

            Dim sQuery As String = SommatoriaTempiConFiltriWIQY(inData)
            Dim WITList As List(Of Models.WorkItem) = OttiemiWorkItemsDaQueryWIQL(sQuery, inData.campoPerAggregazioni, QueryResultType.WorkItem)

            r.RispostaStringa = 0

            'per ciascun workitem sommo i tempi
            If Not WITList Is Nothing Then

                For Each ww In WITList
                    Dim v1 As Double = SommatoriaTempiConFiltriLeggiDouble(inData.campoPerAggregazioni, ww)
                    r.RispostaStringa += v1

                Next
            End If



        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function

    Private Shared Sub LeggiAssegnatoA(ww As Models.WorkItem, smallItem As SmallWorkItem)

        Try
            Dim assegnatoA As IdentityRef = ww.Fields("System.AssignedTo")
            smallItem.AssegnatoA = assegnatoA.UniqueName
            If smallItem.AssegnatoA <> "" Then
                smallItem.AssegnatoA_email = smallItem.AssegnatoA.Replace("AGRONICAGROUP\", "") & "@agronica.it"
            End If
        Catch ex As Exception
            smallItem.AssegnatoA = ""
        End Try

    End Sub

    Private Shared Function SommatoriaTempiConFiltriLeggiDouble(campoDaLeggere As String, ww As Models.WorkItem) As Double
        Dim v1 As Double
        Dim s1 As String = "0"
        Try
            s1 = ww.Fields(campoDaLeggere)
        Catch ex As Exception
        End Try

        If String.IsNullOrEmpty(s1) Then
            v1 = 0
        Else
            Dim pRval As Boolean = Double.TryParse(s1.Replace(".", AgronicaCoreUtility.CulturaHelper.SeparatoreDecimaleVB), v1)
            If Not pRval Then
                Throw New Exception()
            End If
        End If

        Return v1
    End Function

    Private Function SommatoriaTempiConFiltriWIQY(inData As ConfigurazioneWIQL) As String

        Dim stb As New StringBuilder
        'SELECT [Id] FROM workitems WHERE [Tipo di elemento di lavoro] = 'Attività' And [Tag] Contains 'Commessa 2019/00001' AND [Tag] Contains 'Task 10' 
        stb.AppendLine("SELECT [Id] FROM workitems WHERE [Tipo di elemento di lavoro] = '" & inData.tipoWorkItem & "' AND ")

        Dim l1 As New List(Of String)
        For Each ff As ConfigurazioneWIQLCampo In inData.listaFiltri
            l1.Add("[Tag] Contains '" & ff.campo & " " & ff.valore & "'")
        Next

        stb.Append(String.Join(" AND ", l1))


        Return stb.ToString

    End Function


    Private Function ListaFunzionalitaConFiltriWIQY(inData As ConfigurazioneWIQL) As String

        Dim stb As New StringBuilder
        'SELECT [Id] FROM workitems WHERE [Tipo di elemento di lavoro] = 'Attività' And [Tag] Contains 'Commessa 2019/00001' AND [Tag] Contains 'Task 10' 
        stb.AppendLine("SELECT [Id] FROM workitems WHERE [Tipo di elemento di lavoro] = '" & inData.tipoWorkItemFunzionalita & "' AND ")

        Dim l1 As New List(Of String)
        For Each ff As ConfigurazioneWIQLCampo In inData.listaFiltri
            l1.Add("[Tag] Contains '" & ff.campo & " " & ff.valore & "'")
        Next

        stb.Append(String.Join(" AND ", l1))


        Return stb.ToString

    End Function

    Private Function ListaFigliWIQY(idDaRicercare As Integer, inData As ConfigurazioneWIQL) As String

        'AND  [Target].[System.WorkItemType] = '{2}'
        Dim _wiql As String = String.Format("SELECT [System.Id] FROM WorkItemLinks WHERE ([Source].[System.WorkItemType] = '{1}') And ([System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward') And ([Source].[System.Id] = {0}) ORDER BY [System.Id] mode(MustContain)", idDaRicercare, inData.tipoWorkItemFunzionalita, inData.tipoWorkItem)


        Return _wiql

    End Function

    Private Function GetWorkItem(id As Integer) As Task(Of Models.WorkItem)
        Return _witClient.GetWorkItemAsync(id)
    End Function


    Private Function WITGetDetails(wit As WorkItemReference, inData As ParseJSonWorkItemIn) As String

        Dim sJsonDetail As String = TFSApiRestCall(wit.Url)

    End Function

    Private Function TFSApiRestCall(url As String) As String

    End Function

    ''' <summary>
    ''' Esegue una query team
    ''' </summary>
    ''' <param name="sQuery">es: SELECT [Id], [Titolo], [Lavoro Rimanente] FROM workitems WHERE [Tipo di elemento di lavoro] = 'Bug' AND [Assegnato a] = @Me</param>
    ''' <returns></returns>
    Private Function EseguiQueryWIQL(sQuery As String) As WorkItemQueryResult


        Dim query As Wiql = New Wiql() With {
            .Query = sQuery
        }
        Dim queryResults As WorkItemQueryResult = _witClient.QueryByWiqlAsync(query).Result

        Return queryResults

    End Function

    Private Function ListaIdViaQueryWIQLViaStore(sQuery As String) As List(Of Integer)



        Dim _query As Query = New Query(_wistore, sQuery)
        Dim _links As List(Of WorkItemLinkInfo) = _query.RunLinkQuery().ToList

        Dim rval As New List(Of Integer)

        For Each l As WorkItemLinkInfo In _links
            rval.Add(l.TargetId)
        Next

        Return rval

    End Function

    Private Function OttiemiWorkItemsDaQueryWIQL(ByVal sQuery As String, ByVal ListaCampiDaLeggere As String, queryType As QueryResultType) As List(Of Models.WorkItem)

        Dim ids As List(Of Integer)

        Select Case queryType
            Case QueryResultType.WorkItem
                ids = ListaIdViaQueryWIQL(sQuery)
            Case QueryResultType.WorkItemLink
                'ids = ListaIdViaQueryWIQLViaStore(sQuery)
                ids = ListaIdViaQueryWIQL(sQuery)
        End Select

        If ids.Count = 0 Then
            Return New List(Of Models.WorkItem)
        End If

        Dim fields = ListaCampiDaLeggere.Split(",")
        Return _witClient.GetWorkItemsAsync(ids.ToArray(), fields).Result

    End Function


    Public Function ListaIdViaQueryWIQL(sQuery As String) As List(Of Integer)
        Dim result As WorkItemQueryResult = EseguiQueryWIQL(sQuery)

        'Leggo le relazioni
        If result.WorkItems Is Nothing Then

            Dim rval As New List(Of Integer)
            If Not result.WorkItemRelations Is Nothing Then
                For Each rel In result.WorkItemRelations
                    If Not rel.Source Is Nothing Then
                        rval.Add(rel.Target.Id)
                    End If

                Next
            End If
            Return rval
        End If

        'oppure leggo gli item diretti
        Dim ids = result.WorkItems.[Select](Function(item) item.Id).ToList()
        Return ids

    End Function

    Public Class ConfigurazioneWIQL
        Public listaFiltri As List(Of ConfigurazioneWIQLCampo)
        Public tipoWorkItem As String
        Public tipoWorkItemFunzionalita As String
        Public campoPerAggregazioni As String
    End Class

    Public Class ConfigurazioneWIQLCampo
        Public campo As String
        Public valore As String
        Public valoreTrovatoSuTFS As Boolean
    End Class


    Public Class ParseJSonWorkItemIn
        Public jSonWIT As String
    End Class

    Public Class SmallWorkItem
        Public Property id As Integer
        Public Property AssegnatoA As String
        Public Property Trovato_AssegnatoA As Boolean
        Public Property AssegnatoA_email As String
        Public Property StimaOriginale As Double
        Public Property OreRimanenti As Double
        Public Property Trovato_OreRimanenti As Boolean
        Public Property Trovato_StimaOriginale As Boolean
        Public Property OrdineDiPriorita As Integer
        Public Property Trovato_OrdineDiPriorita As Boolean
    End Class

    Public Class AggiornaCampiWorkItemIn
        Public Property id As Integer
        Public Property listaCampiDaAggiornare As List(Of ConfigurazioneWIQLCampo)
    End Class

    Public Enum TipoEstrazione
        Attivita = 1
        Funzionalita = 2
        Gerarchia = 3
    End Enum

End Class
