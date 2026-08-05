Imports System.Configuration
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.Linq.Expressions
Imports System.Reflection
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading.Tasks
Imports System.Xml
Imports Microsoft.SqlServer.Management.SqlParser.Parser


Public Class ParametrizzatoreNew
    Inherits AgronicaCoreDataProvider.LogProviderEsteso
    Implements IParametrizzatore

    Private _nomenclatoreParametri As NomenclatoreParametri = Nothing
    Private _parametri As Dictionary(Of Int32, AgroDBParametro) = Nothing
    Private _copiaParametri As Dictionary(Of Int32, AgroDBParametro) = Nothing
    Private _filtroAggiuntivoOriginale As String = String.Empty
    Private _orderByOriginale As String = String.Empty
    Private _filtroAggiuntivoManipolato As String = String.Empty
    Private _orderByManipolato As String = String.Empty
    Private _byBassaLog As Boolean = False

    Private Const QUERY_MAX_LENGTH As Integer = 330000
    Private Const MAX_PARAMETRI As Integer = 1900

    Public ReadOnly Property Parametri() As Dictionary(Of Int32, AgroDBParametro) Implements IParametrizzatore.Parametri
        Get
            Return _parametri
        End Get
    End Property

    Public ReadOnly Property CopiaParametri() As Dictionary(Of Int32, AgroDBParametro) Implements IParametrizzatore.CopiaParametri
        Get
            Return _copiaParametri
        End Get
    End Property

    Public Property FiltroAggiuntivoOriginale() As String Implements IParametrizzatore.FiltroAggiuntivoOriginale
        Get
            Return _filtroAggiuntivoOriginale
        End Get
        Set(ByVal value As String)
            _filtroAggiuntivoOriginale = value
        End Set
    End Property

    Public Property FiltroAggiuntivoManipolato() As String Implements IParametrizzatore.FiltroAggiuntivoManipolato
        Get
            Return _filtroAggiuntivoManipolato
        End Get
        Set(ByVal value As String)
            _filtroAggiuntivoManipolato = value
        End Set
    End Property

    Public Property OrderByOriginale() As String Implements IParametrizzatore.OrderByOriginale
        Get
            Return _orderByOriginale
        End Get
        Set(ByVal value As String)
            _orderByOriginale = value
        End Set
    End Property

    Public Property OrderByManipolato() As String Implements IParametrizzatore.OrderByManipolato
        Get
            Return _orderByManipolato
        End Get
        Set(ByVal value As String)
            _orderByManipolato = value
        End Set
    End Property

    Public Property ByPassaLog() As Boolean Implements IParametrizzatore.ByPassaLog
        Get
            Return _byBassaLog
        End Get
        Set(ByVal value As Boolean)
            _byBassaLog = value
        End Set
    End Property

    Sub New(ByVal configurazioneLogProviderEsteso As ConfigurazioneLogProviderEsteso)
        _parametri = New Dictionary(Of Int32, AgroDBParametro)
        _nomenclatoreParametri = New NomenclatoreParametri

    End Sub

    Public Function Parametrizza(ByRef stringaSql As String,
                                 ByVal parametriInput As Dictionary(Of Int32, AgroDBParametro),
                                 ByVal orderByEFiltroAggiuntivo As OrderByFiltroAggiuntivo,
                                 ByRef parametriOutput As List(Of SqlParameter),
                                 Optional ByVal objParametri_Server As AgronicaCoreParametri = Nothing) As Boolean Implements IParametrizzatore.Parametrizza

        Dim nomeRoutine As String = "AgronicaCoreDataProvider.Parametrizzatore.Parametrizza"
        Dim stringSqlOriginale = stringaSql.ToOrigin(objParametri_Server)
        Dim cache = DataProviderFactory.Instance.CacheQueryInErrore

        parametriOutput = New List(Of SqlParameter)

        Try

            If Not IsNothing(cache) Then
                If cache.VerificaChiaveCacheata(stringSqlOriginale.ToHash, objParametri_Server) Then
                    stringaSql = stringSqlOriginale
                    Return False
                End If
            End If

            ' Check 1 -> non ci sono parametri collezionati e la stringa contiene valori parametrizzati
            If parametriInput Is Nothing OrElse Not parametriInput.Any Then
                If _nomenclatoreParametri.ContieneNomenclatura(stringaSql) Then

                    stringaSql = stringSqlOriginale
                    Dim messaggioErrore As String = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "PARAMETRI NON COLLEZIONATI" & vbCr & "METODO CHIAMANTE: {1}",
                                                       stringaSql, DammiStackTrace())

                    Logga(nomeRoutine, messaggioErrore, objParametri_Server)
                    SollevaEccezioneSeDebug(messaggioErrore, stringSqlOriginale)

                End If

                Return False

            End If

            'Check 2:  Controllo su lunghezza stringaSql >= 325.000 chars
            If stringSqlOriginale.Length >= QUERY_MAX_LENGTH Then

                If stringSqlOriginale.ToUpper.Contains(" ORDER BY ") Then

                    stringaSql = stringSqlOriginale
                    Dim messaggioErrore As String = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "RILEVATA STRINGA SQL ECCESSIVAMENTE LUNGA PER IL PARSER" & vbCr & "METODO CHIAMANTE: {1}",
                                                       stringSqlOriginale, DammiStackTrace())

                    Logga(nomeRoutine, messaggioErrore, objParametri_Server)
                    SollevaEccezioneSeDebug(messaggioErrore, stringSqlOriginale)

                    Return False

                End If

            End If

            ' CHeck 3 -> più di 2000 parametri collezionati

            If parametriInput.Count >= 2000 Then

                stringaSql = stringSqlOriginale
                Dim messaggioErrore As String = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "RILEVATA STRINGA SQL CON PIU' DI 2000 PARAMETRI" & vbCr & "METODO CHIAMANTE: {1}",
                                                      stringSqlOriginale, DammiStackTrace())

                Logga(nomeRoutine, messaggioErrore, objParametri_Server)
                SollevaEccezioneSeDebug(messaggioErrore, stringSqlOriginale)

                Return False

            End If

            Dim sb = New StringBuilder(stringaSql)
            Dim parametriDesc = (From kvp In parametriInput Order By kvp.Key Descending Select kvp).ToList

            For Each kvp In parametriDesc

                Dim valoreEffettivo = ""
                Dim parametroOriginale As AgroDBParametro = kvp.Value
                Dim parametroDaUtilizzare As AgroDBParametro = kvp.Value

                If parametroOriginale.ChiaveRiferimento IsNot Nothing AndAlso parametroOriginale.ChiaveRiferimento.HasValue Then
                    parametroDaUtilizzare = parametriInput(parametroDaUtilizzare.ChiaveRiferimento)
                End If

                If parametroDaUtilizzare.Risolto Then
                    Continue For
                End If

                Dim nomenclatura As Tuple(Of String, String) = _nomenclatoreParametri.OttieniNomenclatura(parametroDaUtilizzare.Tipo)
                If String.IsNullOrEmpty(nomenclatura.Item1) OrElse String.IsNullOrEmpty(nomenclatura.Item2) Then
                    valoreEffettivo = parametroDaUtilizzare.Valore.ToString()
                Else
                    valoreEffettivo = parametroDaUtilizzare.Valore.ToString().Replace(nomenclatura.Item1, "").Replace(nomenclatura.Item2, "")
                End If

                Dim parteFissaNomeParametro As String = _nomenclatoreParametri.OttieniParteFissaNomeParametro(parametroDaUtilizzare.Tipo)
                'Dim p As New SqlParameter(String.Format("{0}{1}", parteFissaNomeParametro, kvp.Key.ToString().PadLeft(4, "0")), valoreEffettivo)
                Dim p As New SqlParameter()
                p.ParameterName = String.Format("{0}{1}", parteFissaNomeParametro, kvp.Key.ToString().PadLeft(4, CChar("0")))

                Select Case parametroDaUtilizzare.Tipo
                    Case GetType(String)
                        p.Value = valoreEffettivo
                    Case GetType(Decimal)
                        ParamettroEffettivo(valoreEffettivo, p)
                    Case GetType(Date)
                        p.Value = valoreEffettivo
                End Select

                Dim stringaDaCercare As String = ""
                Select Case kvp.Value.Tipo
                    Case GetType(String)
                        stringaDaCercare = "'" & UtilityProvider.Agro_SQL_SaveText(parametroDaUtilizzare.Valore) & "'"
                    Case GetType(Date)
                        stringaDaCercare = "'" & parametroDaUtilizzare.Valore & "'"
                    Case GetType(Decimal)
                        stringaDaCercare = parametroDaUtilizzare.Valore
                End Select

                Dim patternRisoloto As Boolean = False
                Dim indiceNellaStringa As Integer = 0

                While indiceNellaStringa <> -1

                    indiceNellaStringa = sb.ToString().LastIndexOf(stringaDaCercare, StringComparison.InvariantCultureIgnoreCase)

                    If indiceNellaStringa <> -1 Then
                        sb.Remove(indiceNellaStringa, stringaDaCercare.Length)
                        sb.Insert(indiceNellaStringa, p.ParameterName)
                        If Not patternRisoloto Then
                            parametriOutput.Add(p)
                        End If
                        patternRisoloto = True
                        parametroDaUtilizzare.Risolto = True
                        parametroOriginale.Risolto = True
                    End If

                End While

                If Not patternRisoloto Then

                    ' Prova a cercare per pattern particolari
                    ' Like 
                    Dim patternRicercaLike = DammiPatternsRicercaLike(UtilityProvider.Agro_SQL_SaveText(parametroDaUtilizzare.Valore))
                    patternRisoloto = ApplicaPatternRicerca(sb, patternRicercaLike, p, parametriOutput)

                    ' NULL
                    If Not patternRisoloto Then
                        Dim patternRicercaNULL = DammiPatternsRicercaNull(parametroDaUtilizzare.Valore)
                        patternRisoloto = ApplicaPatternRicerca(sb, patternRicercaNULL, p, parametriOutput)
                    End If

                End If

                If Not _nomenclatoreParametri.ContieneNomenclatura(sb.ToString) Then
                    Exit For
                End If

            Next

            stringaSql = sb.ToString()

            Dim checkOK As Boolean = DoppioCheckSicurezza(stringSqlOriginale, stringaSql, parametriOutput)
            If Not checkOK Then
                ' Errore, sostituzione parametri non completa. le due stringhe non sono identiche

                Dim sbLog = New StringBuilder
                sbLog.AppendLine(stringaSql)
                sbLog.AppendLine("")

                If Not IsNothing(parametriOutput) AndAlso parametriOutput.Any Then
                    Dim parametriOrdinati = parametriOutput.OrderBy(Of String)(Function(p) p.ParameterName).ToList
                    sbLog.AppendLine("PARAMETRI: ")
                    For Each p As SqlParameter In parametriOrdinati
                        sbLog.AppendLine(String.Format("{0} -> {1}", p.ParameterName, p.Value.ToString))
                    Next
                End If
                Dim messaggioErrore = ""

                messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "FALLITO DOPPIO CHECK DI SICUREZZA" & vbCrLf & "METODO CHIAMANTE: {1}",
                                                   sbLog.ToString(), DammiStackTrace())

                If Not _byBassaLog Then
                    Logga(nomeRoutine, messaggioErrore, objParametri_Server)
                End If

                If Debugger.IsAttached Then
                    messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "FALLITO DOPPIO CHECK DI SICUREZZA" & vbCrLf & "METODO CHIAMANTE: {1}",
                                                   sbLog.ToString(), DammiStackTrace())
                Else
                    messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "FALLITO DOPPIO CHECK DI SICUREZZA",
                                                   "")
                End If

                stringaSql = stringSqlOriginale
                SollevaEccezioneSeDebug(messaggioErrore, stringSqlOriginale)


            End If

            If checkOK Then

                Dim orderByOk As Boolean = ParametrizzaOrderBY(stringaSql, stringSqlOriginale, parametriDesc, parametriOutput, objParametri_Server)
                If Not orderByOk Then

                    Dim sbLog = New StringBuilder
                    sbLog.AppendLine(stringaSql)
                    sbLog.AppendLine("")
                    sbLog.AppendLine("PARAMETRI: ")
                    For Each p As SqlParameter In parametriOutput
                        sbLog.AppendLine(String.Format("{0} -> {1}", p.ParameterName, p.Value.ToString))
                    Next

                    Dim messaggioErrore = ""

                    messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "FALLITA PARAMETRIZZAZIONE ORDER BY" & vbCrLf & "METODO CHIAMANTE: {1}",
                                                   sbLog.ToString(), DammiStackTrace())

                    If Not _byBassaLog Then
                        Logga(nomeRoutine, messaggioErrore, objParametri_Server)
                    End If

                    If Debugger.IsAttached Then
                        messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "FALLITA PARAMETRIZZAZIONE ORDER BY" & vbCrLf & "METODO CHIAMANTE: {1}",
                                                   sbLog.ToString(), DammiStackTrace())
                    Else
                        messaggioErrore = String.Format("---> MANCATA PARAMETRIZZAZIONE DELLA QUERY : {0}" & vbCrLf & "FALLITA PARAMETRIZZAZIONE ORDER BY",
                                                   "")
                    End If

                    stringaSql = stringSqlOriginale
                    checkOK = False

                    SollevaEccezioneSeDebug(messaggioErrore, stringSqlOriginale)

                End If

            End If

            Return checkOK

        Catch ex As ParametrizzatoreException
            Throw ex
        Catch ex As OutOfMemoryException
            stringaSql = stringSqlOriginale
            Dim stackTrace As String = DammiStackTrace()

            Dim messaggioErrore = ""

            messaggioErrore = String.Format("---> RILEVATA STRINGA SQL ECCESSIVAMENTE LUNGA PER IL PARSER : {0}" & vbCrLf & "METODO CHIAMANTE: {1}",
                                                   stringaSql, stackTrace)

            Cachea_Query_In_Errore(stringaSql, messaggioErrore, stackTrace, objParametri_Server)

            If Debugger.IsAttached Then
                messaggioErrore = String.Format("---> RILEVATA STRINGA SQL ECCESSIVAMENTE LUNGA PER IL PARSER : {0}" & vbCrLf & "METODO CHIAMANTE: {1}",
                                                   stringaSql, stackTrace)
            Else
                messaggioErrore = String.Format("---> RILEVATA STRINGA SQL ECCESSIVAMENTE LUNGA PER IL PARSER : {0}",
                                   "")
            End If

            Return False
        Catch ex As TempoParserException
            stringaSql = stringSqlOriginale
            Dim stackTrace As String = DammiStackTrace()
            Dim cfgEstesa = ConfigurazioneEstesaSqlProviderFactory.Instance(objParametri_Server)
            Dim maxTempoParsing As Integer = If(cfgEstesa IsNot Nothing, CInt(cfgEstesa.TempoWarningParser_Millisecondi), 3000)
            Dim messaggioErrore = ""

            messaggioErrore = String.Format("---> IL PARSER SQL HA IMPIEGATO PIU' DI {0} MILLISECONDI DI TEMPO PER VALUTARE LA STRINGA. {1} " & vbCrLf & "METODO CHIAMANTE: {2}",
                 maxTempoParsing, stringaSql, stackTrace)

            Cachea_Query_In_Errore(stringaSql, messaggioErrore, stackTrace, objParametri_Server)

            If Debugger.IsAttached Then
                messaggioErrore = String.Format("---> IL PARSER SQL HA IMPIEGATO PIU' DI {0} MILLISECONDI DI TEMPO PER VALUTARE LA STRINGA. {1} " & vbCrLf & "METODO CHIAMANTE: {2}",
                 maxTempoParsing, stringaSql, stackTrace)
            Else
                messaggioErrore = String.Format("---> IL PARSER SQL HA IMPIEGATO PIU' DI {0} MILLISECONDI DI TEMPO PER VALUTARE LA STRINGA. {1} ",
                 maxTempoParsing, "")
            End If

            Return False
        Catch ex As Exception
            stringaSql = stringSqlOriginale
            Return False

        End Try

    End Function

    Private Sub Cachea_Query_In_Errore(ByVal stringaSql As String,
                                       ByVal messaggioErrore As String,
                                       ByVal stackTrace As String,
                                       ByVal objParametri_Server As AgronicaCoreParametri)

        Dim nomeRoutine As String = "AgronicaCoreDataProvider.Parametrizzatore.Parametrizza"
        Dim cache = DataProviderFactory.Instance.CacheQueryInErrore

        ' Metto in cache la query Errata
        ' provo a leggere la configurazione della cache
        Dim cfgEstesa = ConfigurazioneEstesaSqlProviderFactory.Instance(objParametri_Server)
        ' se la lettura è andata a buon fine e la cache non è ancora stata inizializzata la inizializzo
        If cfgEstesa IsNot Nothing AndAlso IsNothing(DataProviderFactory.Instance.CacheQueryInErrore) Then
            DataProviderFactory.Instance.CacheQueryInErrore = New CachedQueryInErrore(cfgEstesa)
            If IsNothing(cache) Then
                cache = DataProviderFactory.Instance.CacheQueryInErrore
                MemoryCacheFactory.Instance.Add_Cache_To_Collection(cache)
            End If
        End If
        If cfgEstesa IsNot Nothing AndAlso IsNothing(DataProviderFactory.Instance.ConfigurazioneEstesa) Then
            DataProviderFactory.Instance.ConfigurazioneEstesa = cfgEstesa
        End If

        If Not IsNothing(cache) Then
            cache.Scrivi(stringaSql.ToHash, stackTrace, objParametri_Server)
        End If

        Logga(nomeRoutine, messaggioErrore, objParametri_Server)

    End Sub

    Private Sub SollevaEccezioneSeDebug(ByVal messaggioErrore As String, ByVal stringSqlOriginale As String)
        If Debugger.IsAttached Then
            Dim sbMsg As New StringBuilder
            sbMsg.AppendLine(messaggioErrore)
            sbMsg.Append(Environment.NewLine)
            sbMsg.AppendLine(stringSqlOriginale)
            Throw New ParametrizzatoreException(sbMsg.ToString)
        End If
    End Sub

    Public Class OrderByWithParent
        Public Property OrderByClause As Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlOrderByClause
        Public Property Parent As Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlCodeObject

        Public Sub New(orderBy As Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlOrderByClause, parent As Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlCodeObject)
            Me.OrderByClause = orderBy
            Me.Parent = parent
        End Sub
    End Class

    Private Sub Trova_OrdersBy(ByRef elem As Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlCodeObject, ByRef parent As Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlCodeObject, ByRef ordersBy As List(Of OrderByWithParent))
        If elem.GetType().Name = "SqlOrderByClause" Then
            ordersBy.Add(New OrderByWithParent(elem, parent))
            Return
        End If
        For Each child In elem.Children
            Trova_OrdersBy(child, elem, ordersBy)
        Next
    End Sub

    Private Sub Trova_ordersByItem(ByRef elem As Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlCodeObject, ByRef ordersByItem As List(Of Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlOrderByItem), ByRef ordersByOffeset As Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlOffsetFetchClause)
        For Each cn As Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlCodeObject In elem.Children
            If TypeOf cn Is Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlOrderByItem Then
                ordersByItem.Add(cn)
            End If
            If TypeOf cn Is Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlOffsetFetchClause Then
                ordersByOffeset = cn
            End If
        Next
    End Sub
    Private Function ParametrizzaOrderBY(ByRef stringaSql As String, stringSqlOrigin As String,
                                         parametriInput As List(Of KeyValuePair(Of Integer, AgroDBParametro)),
                                         ByRef parametriOutput As List(Of SqlParameter),
                                         Optional ByVal objParametri_Server As AgronicaCoreParametri = Nothing) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreDataProvider.Parametrizzatore.ParametrizzaOrderBY()"
        Dim stringaSqlOriginale As String = stringSqlOrigin
        Dim returnValue As Boolean = False

        Try

            Dim parseOptions As New ParseOptions(";")

            'Dim sw As New Stopwatch
            'sw.Start()

            Dim parser = Microsoft.SqlServer.Management.SqlParser.Parser.Parser.Parse(stringSqlOrigin, parseOptions)

            Dim tempoParser As Long = 0

            Dim orderByNodes = New List(Of OrderByWithParent)()
            For Each child In parser.Script.Batches
                ' TODO: for now is recursive search, but it's possible to setup a personalized search, understanding the structure above different queries
                Trova_OrdersBy(child, parser.Script, orderByNodes)
            Next

            If orderByNodes.Count > 0 Then

                Dim progressivoParametriOrderBy = parametriInput.First.Key + 1

                For cont As Integer = (orderByNodes.Count - 1) To -1 Step -1

                    Dim orderByNode As OrderByWithParent = If(cont = -1, Nothing, orderByNodes(cont))
                    If Not IsNothing(orderByNode) Then

                        Dim orderBySB As New StringBuilder
                        Dim orderByBody As String = orderByNode.OrderByClause.Sql
                        Dim orderByLength = orderByBody.Length
                        Dim stringaFinaleOrderBy As String = String.Empty

                        If Query_Con_Union(orderByNode) Then
                            stringaFinaleOrderBy = orderByBody
                        ElseIf Query_Con_Distinct(orderByNode) Then
                            stringaFinaleOrderBy = orderByBody
                        Else

                            Dim parteFissaNomeParametroOrderBy As String = _nomenclatoreParametri.OttieniParteFissaNomeParametro(GetType(String))

                            Dim elencoAlias = Ottieni_Alias_Campi(orderByNode).Distinct().ToList()
                            Dim elencoCampiRaw = Ottieni_Lista_Campi_Select(orderByNode).Distinct().ToList()
                            Dim elencoCampi = elencoCampiRaw.Except(elencoAlias).ToList()

                            ' Ricavo la lista dei campi in order by
                            Dim orderByFields = New List(Of Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlOrderByItem)
                            Dim orderByOffset As Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlOffsetFetchClause = Nothing
                            Trova_ordersByItem(orderByNode.OrderByClause, orderByFields, orderByOffset)
                            For Each orderByField In orderByFields
                                Dim orderByParameterName = String.Format("{0}{1}", parteFissaNomeParametroOrderBy, progressivoParametriOrderBy.ToString().PadLeft(4, "0"c))
                                Dim originalCompleteFieldName As String = orderByField.Expression.Sql
                                Dim completeFieldName As String = originalCompleteFieldName.ToLower

                                'Dim expressionType As Type = orderByField.Expression.GetType()

                                ' TODO here's is possible to cast and not treat is at a generic object
                                ' Check if the ColumnName property exists in the expression type

                                Dim scalarRefExp As Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlScalarRefExpression = Nothing
                                Dim columnNameProperty As String = ""
                                Dim partialFieldName As String = ""
                                Dim sqlSortOrder As String = ""

                                Dim parserSortOrder = orderByField.SortOrder

                                If parserSortOrder <> Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlSortOrder.None Then
                                    sqlSortOrder = If(parserSortOrder = Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlSortOrder.Ascending, "ASC", "DESC")
                                End If

                                If TypeOf orderByField.Children(0) Is Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlScalarRefExpression Then

                                    scalarRefExp = DirectCast(orderByField.Children(0), Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlScalarRefExpression)
                                    columnNameProperty = scalarRefExp.MultipartIdentifier.Last.Value

                                    partialFieldName = If(columnNameProperty, completeFieldName).ToLower()

                                    Dim fieldNode = orderByField.Expression

                                    Dim fieldType = fieldNode.GetType()

                                    ' TODO here's is possible to cast and not treat is at a generic object
                                    ' Check if the ColumnName property exists in the expression type
                                    'Dim fieldTypeProperty As PropertyInfo = expressionType.GetProperty("Type")

                                End If

                                Dim orderByType As String = If(scalarRefExp Is Nothing, "", Convert.ToString(scalarRefExp).ToLower())

                                If Not elencoAlias.Contains(completeFieldName) AndAlso orderByType <> "integer" AndAlso orderByType <> "" Then
                                    If elencoCampi.Count > 0 AndAlso (elencoCampi.Contains(completeFieldName.ToLower) OrElse elencoCampi.Contains(partialFieldName.ToLower)) Then
                                        Dim pOB As New SqlParameter(orderByParameterName, originalCompleteFieldName)
                                        parametriOutput.Add(pOB)
                                        orderBySB.AppendLine(String.Format("case when {0} = '{1}' then {1} end {2} ,", orderByParameterName, originalCompleteFieldName, sqlSortOrder))
                                        progressivoParametriOrderBy = progressivoParametriOrderBy + 1
                                    Else
                                        orderBySB.AppendLine(String.Format("{0} {1} ,", originalCompleteFieldName, sqlSortOrder))
                                    End If
                                Else
                                    orderBySB.AppendLine(String.Format("{0} {1} ,", originalCompleteFieldName, sqlSortOrder))
                                End If

                            Next
                            stringaFinaleOrderBy = orderBySB.ToString

                            If (Not String.IsNullOrEmpty(stringaFinaleOrderBy)) Then

                                stringaFinaleOrderBy = stringaFinaleOrderBy.Remove(stringaFinaleOrderBy.LastIndexOf(",")).Insert(0, "order by ")

                            End If

                            If Not String.IsNullOrEmpty(stringaFinaleOrderBy) AndAlso Not IsNothing(orderByOffset) Then
                                stringaFinaleOrderBy = String.Concat(stringaFinaleOrderBy, " " & orderByOffset.Sql)
                            End If

                            ' TODO: use the orderByNode.OrderByClause.startLocation and endLocation to identify where replace for better performance and stability
                            Dim orderByFinaleSB As New StringBuilder
                            Dim lastIndexOrderBy As Integer = stringaSql.LastIndexOf(orderByBody, StringComparison.InvariantCultureIgnoreCase)
                            orderByFinaleSB.Append(stringaSql)
                            orderByFinaleSB.Remove(lastIndexOrderBy, orderByLength)
                            orderByFinaleSB.Insert(lastIndexOrderBy, stringaFinaleOrderBy)

                            stringaSql = orderByFinaleSB.ToString


                        End If
                    End If
                Next

                returnValue = True

            Else
                returnValue = True
            End If

            'sw.Stop()

            'Scrivi_LOG("", "", "", "ParametrizzatoreNew.ParametrizzaOrderBy", String.Format("Time for parsing: {0} milliseconds", sw.ElapsedMilliseconds))

        Catch ex As OutOfMemoryException
            Throw ex
        Catch ex As TempoParserException
            Throw ex
        Catch ex As Exception
            stringaSql = stringaSqlOriginale
            returnValue = False

        End Try

        Return returnValue

    End Function

    Private Function Query_Con_Union(ByVal nodoOrderBy As OrderByWithParent) As Boolean

        If IsNothing(nodoOrderBy) Then
            Return False
        End If

        Dim nodoPadre = nodoOrderBy.Parent
        If IsNothing(nodoPadre) Then
            Return False
        End If

        If IsNothing(nodoPadre.Children) Then
            Return False
        End If

        For Each cn As Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlCodeObject In nodoPadre.Children
            If TypeOf cn Is Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlBinaryQueryExpression Then
                Dim binaryQueryExpression = DirectCast(cn, Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlBinaryQueryExpression)
                Dim op = binaryQueryExpression.Operator

                If op = Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlBinaryQueryOperatorType.Union OrElse op = Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlBinaryQueryOperatorType.UnionAll Then
                    Return True
                End If
            End If
        Next

        Return False

    End Function

    Private Function Query_Con_Distinct(ByVal nodoOrderBy As OrderByWithParent) As Boolean

        If IsNothing(nodoOrderBy) Then
            Return False
        End If

        Dim nodoPadre = nodoOrderBy.Parent
        If IsNothing(nodoPadre) Then
            Return False
        End If

        If IsNothing(nodoPadre.Children) Then
            Return False
        End If

        For Each cn As Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlCodeObject In nodoPadre.Children
            If TypeOf cn Is Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlQuerySpecification Then
                Dim binaryQueryExpression = DirectCast(cn, Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlQuerySpecification)
                For Each child In binaryQueryExpression.Children
                    If TypeOf child Is Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlSelectClause Then
                        If DirectCast(child, Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlSelectClause).IsDistinct Then
                            Return True
                        End If
                    End If
                Next
            End If
        Next

        Return False

    End Function

    Private Function Ottieni_Lista_Campi_Select(ByVal nodoOrderBy As OrderByWithParent) As List(Of String)

        Dim nomiCampi As New List(Of String)

        If IsNothing(nodoOrderBy) Then
            Return nomiCampi
        End If

        Dim nodoPadre = nodoOrderBy.Parent
        If IsNothing(nodoPadre) Then
            Return nomiCampi
        End If

        EsploraSqlRicorsivoDaPadre(nodoPadre, nomiCampi, False)

        Return nomiCampi

    End Function

    Private Shared Sub Riempi_Lista_Campi_Select(ByRef nomiCampi As List(Of String), bibi As Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlCodeObject)
        If TypeOf bibi Is Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlSelectClause Then
            Dim selectClause = DirectCast(bibi, Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlSelectClause)
            For Each child In selectClause.Children
                If TypeOf child Is Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlSelectScalarExpression Then
                    Trova_Lista_Campi_Ricorsivo(nomiCampi, child)
                End If
            Next
        End If
    End Sub

    Private Shared Sub Trova_Lista_Campi_Ricorsivo(ByRef nomiCampi As List(Of String), child As Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlCodeObject)
        Dim identifier = ""
        For Each child2 In child.Children
            If identifier = "" Then
                If TypeOf child2 Is Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlScalarRefExpression Then
                    Dim selectScalar = DirectCast(child2, Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlScalarRefExpression)
                    If selectScalar.MultipartIdentifier.Count > 1 Then
                        identifier = selectScalar.MultipartIdentifier.Item(0).Value.ToLower & "." & selectScalar.MultipartIdentifier.Item(1).Value.ToLower
                    Else
                        identifier = selectScalar.MultipartIdentifier.Item(0).Value.ToLower
                    End If
                    Continue For
                ElseIf TypeOf child2 Is Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlColumnRefExpression Then
                    Dim selectScalar = DirectCast(child2, Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlColumnRefExpression)
                    identifier = selectScalar.MultipartIdentifier.Item(0).Value.ToLower
                    Continue For
                ElseIf TypeOf child2 Is Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlIdentifier Then
                    Dim selectIdentifier = DirectCast(child2, Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlIdentifier)
                    identifier = selectIdentifier.Value.ToLower
                ElseIf TypeOf child2 Is Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlObjectIdentifier Then
                    Dim selectObjIdentifier = DirectCast(child2, Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlObjectIdentifier)
                    identifier = selectObjIdentifier.Sql.ToLower
                ElseIf TypeOf child2 Is Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlCastExpression Then
                    Dim selectObjIdentifier = DirectCast(child2, Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlCastExpression)
                    Trova_Lista_Campi_Ricorsivo(nomiCampi, child2.Children(0))
                ElseIf child2.Children.Count > 0 Then
                    For Each child3 In child2.Children
                        Trova_Lista_Campi_Ricorsivo(nomiCampi, child3)
                    Next
                End If
            End If
        Next
        If identifier <> "" Then
            nomiCampi.Add(identifier)
        End If
    End Sub

    Private Shared Function OttieniNomiDaBinaryExpression(ByRef nomiCampi As List(Of String), ByRef identifier As String, child2 As Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlCodeObject) As String
        Dim selectBinary = DirectCast(child2, Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlBinaryScalarExpression)
        If TypeOf selectBinary.Left Is Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlBinaryScalarExpression Then
            OttieniNomiDaBinaryExpression(nomiCampi, identifier, selectBinary.Left) 'ricorsivo perchè "u.piva " + " " + " u.piva2 " viene spezzato in SqlBinaryScalarExpression ("u.piva " + " ") e SqlScalarRefExpression
        End If
        If TypeOf selectBinary.Right Is Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlBinaryScalarExpression Then
            OttieniNomiDaBinaryExpression(nomiCampi, identifier, selectBinary.Right)
        End If
        'Riassumibile in una sub modale
        If TypeOf selectBinary.Left Is Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlScalarRefExpression Then
            Dim argScalar = DirectCast(selectBinary.Left, Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlScalarRefExpression)
            If identifier <> "" Then
                nomiCampi.Add(identifier)
                identifier = ""
            End If
            If argScalar.MultipartIdentifier.Count > 1 Then
                identifier = argScalar.MultipartIdentifier.Item(0).Value.ToLower & "." & argScalar.MultipartIdentifier.Item(1).Value.ToLower
            Else
                identifier = argScalar.MultipartIdentifier.Item(0).Value.ToLower
            End If
        End If
        If TypeOf selectBinary.Right Is Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlScalarRefExpression Then
            Dim argScalar = DirectCast(selectBinary.Right, Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlScalarRefExpression)
            If identifier <> "" Then
                nomiCampi.Add(identifier)
                identifier = ""
            End If
            If argScalar.MultipartIdentifier.Count > 1 Then
                identifier = argScalar.MultipartIdentifier.Item(0).Value.ToLower & "." & argScalar.MultipartIdentifier.Item(1).Value.ToLower
            Else
                identifier = argScalar.MultipartIdentifier.Item(0).Value.ToLower
            End If
        End If

        Return identifier
    End Function

    Private Shared Function OttieniNomiDaScalarFunction(ByRef nomiCampi As List(Of String), ByRef identifier As String, child2 As Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlCodeObject) As String
        Dim selectBuiltinScalar = DirectCast(child2, Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlBuiltinScalarFunctionCallExpression)
        For Each arg In selectBuiltinScalar.Arguments
            If TypeOf arg Is Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlScalarRefExpression Then
                Dim argScalar = DirectCast(arg, Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlScalarRefExpression)
                If identifier <> "" Then
                    nomiCampi.Add(identifier)
                    identifier = ""
                End If
                If argScalar.MultipartIdentifier.Count > 1 Then
                    identifier = argScalar.MultipartIdentifier.Item(0).Value.ToLower & "." & argScalar.MultipartIdentifier.Item(1).Value.ToLower
                Else
                    identifier = argScalar.MultipartIdentifier.Item(0).Value.ToLower
                End If
            End If
        Next
        Return identifier
    End Function

    Private Function Ottieni_Alias_Campi(ByVal nodoOrderBy As OrderByWithParent) As List(Of String)

        Dim nomiAlias As New List(Of String)

        If IsNothing(nodoOrderBy) Then
            Return nomiAlias
        End If

        Dim nodoPadre = nodoOrderBy.Parent
        If IsNothing(nodoPadre) Then
            Return nomiAlias
        End If

        EsploraSqlRicorsivoDaPadre(nodoPadre, nomiAlias, True)

        Return nomiAlias

    End Function

    Private Shared Sub EsploraSqlRicorsivoDaPadre(ByVal nodoPadre As Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlCodeObject,
                                         ByRef nomiTrovati As List(Of String),
                                         ByVal cerca_Alias As Boolean)
        Dim nodoQuerySpec

        If TypeOf nodoPadre Is Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlQuerySpecification Then
            nodoQuerySpec = nodoPadre

            If IsNothing(nodoQuerySpec) Then
                Exit Sub
            End If
            ' SQLSelectSpecification -> SQLQuerySpecification -> SQLSelectClause
            Dim cont = 0
            For Each child In nodoQuerySpec.Children
                If TypeOf child Is Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlSelectClause Then
                    If cerca_Alias Then
                        TrovaAlias(nomiTrovati, child)
                    Else
                        Riempi_Lista_Campi_Select(nomiTrovati, child)
                    End If
                ElseIf TypeOf child Is Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlFromClause Then
                    If TypeOf nodoPadre.Children(cont).Children(0) Is Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlQualifiedJoinTableExpression OrElse
                        TypeOf nodoPadre.Children(cont).Children(0) Is Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlDerivedTableExpression Then
                        EsploraSqlRicorsivoDaPadre(nodoPadre.Children(cont).Children(0).Children(0), nomiTrovati, cerca_Alias)
                    End If
                ElseIf TypeOf nodoPadre.Children(cont).Children(0) Is Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlDerivedTableExpression Then
                    EsploraSqlRicorsivoDaPadre(nodoPadre.Children(cont).Children(0).Children(0), nomiTrovati, cerca_Alias)
                End If
                cont += 1
            Next
        Else
            nodoQuerySpec = nodoPadre.Children

            If IsNothing(nodoQuerySpec) OrElse nodoPadre.Children.Count < 2 Then
                Exit Sub
            End If

            For Each child In nodoPadre.Children(0).Children
                If TypeOf child Is Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlSelectClause Then
                    If cerca_Alias Then
                        TrovaAlias(nomiTrovati, child)
                    Else
                        Riempi_Lista_Campi_Select(nomiTrovati, child)
                    End If
                ElseIf TypeOf child Is Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlFromClause Then
                    If TypeOf child.Children(0) Is Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlQualifiedJoinTableExpression OrElse
                       TypeOf child.Children(0) Is Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlDerivedTableExpression Then
                        EsploraSqlRicorsivoDaPadre(child.Children(0).Children(0), nomiTrovati, cerca_Alias)
                    End If
                ElseIf TypeOf child.Children(0) Is Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlDerivedTableExpression Then
                    EsploraSqlRicorsivoDaPadre(child.Children(0).Children(0), nomiTrovati, cerca_Alias)
                End If
            Next
        End If
    End Sub


    Private Shared Sub TrovaAlias(ByRef nomiAlias As List(Of String), child As Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlCodeObject)
        Dim selectClause = DirectCast(child, Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlSelectClause)
        For Each bibi In selectClause.Children
            If TypeOf bibi Is Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlSelectScalarExpression Then
                Dim scalarChild = DirectCast(bibi, Microsoft.SqlServer.Management.SqlParser.SqlCodeDom.SqlSelectScalarExpression)
                If Not IsNothing(scalarChild.Alias) Then
                    nomiAlias.Add(scalarChild.Alias.Value.ToLower)
                    'Else
                    '    nomiAlias.Add(scalarChild.Sql.ToLower)
                End If
            End If
        Next
    End Sub

    Private Function ValutaStringaParsata(parser As ParseResult,
                                          ByRef stringaXml As String,
                                          ByRef millisecondi As Long
                                          ) As Boolean

        Dim sw As New Stopwatch
        sw.Start()
        stringaXml = parser.Script.Xml
        sw.Stop()

        millisecondi = sw.ElapsedMilliseconds
        Dim sogliaWarning = If(Not IsNothing(DataProviderFactory.Instance.ConfigurazioneEstesa),
                                CInt(DataProviderFactory.Instance.ConfigurazioneEstesa.TempoWarningParser_Millisecondi), 3000)

        Return sw.ElapsedMilliseconds > sogliaWarning

    End Function
    Public Function ParametroEffettivo(Of T)(ByVal valore As Object) As T

        Return Convert.ChangeType(valore, GetType(T))

    End Function

    Public Sub ParamettroEffettivo(ByVal valoreStringa As String, ByRef p As SqlParameter)

        If Not IsNumeric(valoreStringa) Then
            p.Value = valoreStringa
        End If

        Dim resOutInt As Integer
        Dim resOutDecimal As Decimal
        Dim resOutDouble As Double

        Try

            If Not valoreStringa.Contains(".") AndAlso Not valoreStringa.Contains(",") Then
                If Integer.TryParse(valoreStringa, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, resOutInt) Then
                    p.Value = resOutInt
                    p.SqlDbType = SqlDbType.Int
                    Return
                End If
            End If

            If Decimal.TryParse(valoreStringa, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, resOutDecimal) Then
                p.Value = resOutDecimal
                p.SqlDbType = SqlDbType.Decimal
                Return
            End If

            If Double.TryParse(valoreStringa, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, resOutDouble) Then
                p.Value = resOutDouble
                p.SqlDbType = SqlDbType.Float
                Return
            End If

            p.Value = valoreStringa

        Catch ex As Exception
            p.Value = valoreStringa
        End Try

    End Sub

    Public Sub SettaCopiaParametri(ByVal parametri As Dictionary(Of Integer, AgroDBParametro)) Implements IParametrizzatore.SettaCopiaParametri
        If parametri IsNot Nothing AndAlso parametri.Any Then
            If IsNothing(_copiaParametri) Then
                _copiaParametri = New Dictionary(Of Integer, AgroDBParametro)
            Else
                _copiaParametri.Clear()
            End If

            For Each kvp In parametri
                Dim cp As New AgroDBParametro With
                {
                    .ChiaveRiferimento = kvp.Value.ChiaveRiferimento,
                    .Valore = kvp.Value.Valore,
                    .IndiceDizionario = kvp.Value.IndiceDizionario,
                    .Risolto = kvp.Value.Risolto,
                    .Tipo = kvp.Value.Tipo
                }
                _copiaParametri.Add(kvp.Key, cp)
            Next
        End If
    End Sub

    Public Function DammiStackTrace() As String Implements IParametrizzatore.DammiStackTrace

        Dim st = New StackTrace()
        Dim metodoChiamante As String = String.Empty
        Dim sb = New StringBuilder

        Dim frames As List(Of StackFrame) = st.GetFrames().Take(10).ToList()
        For Each f As StackFrame In frames
            sb.AppendLine(String.Format("{0} - {1}", f.GetMethod().DeclaringType.FullName, f.GetMethod.Name))
        Next
        metodoChiamante = sb.ToString
        Return metodoChiamante

    End Function

    Private Function DammiPatternsRicercaLike(ByVal valore As String) As List(Of Tuple(Of String, String, String))

        Return New List(Of Tuple(Of String, String, String)) From
            {
                New Tuple(Of String, String, String)("%", "", String.Format("'{0}{1}'", "%", valore)),
                New Tuple(Of String, String, String)("%", "%", String.Format("'{0}{1}{2}'", "%", valore, "%")),
                New Tuple(Of String, String, String)("", "%", String.Format("'{0}{1}'", valore, "%"))
            }

    End Function

    Private Function DammiPatternsRicerca_Json() As List(Of String)

        Return New List(Of String) From
            {
                """:""",
                """ :""",
                """ : """,
                """: """
            }

    End Function

    Private Function Valore_Parser_Tipo_Data(ByVal valore As String, ByRef valoreData As DateTime) As Boolean

        Dim valoreOut As DateTime
        Dim result As Boolean = False

        Dim patterns = New List(Of Tuple(Of Integer, String)) From
            {
                New Tuple(Of Integer, String)(23, "yyyy/MM/dd HH:mm:ss:fff")
            }

        For Each f As Tuple(Of Integer, String) In patterns
            If valore.Trim.Length = f.Item1 AndAlso DateTime.TryParseExact(valore.Trim, f.Item2, CultureInfo.InvariantCulture, DateTimeStyles.None, valoreOut) Then
                valoreData = valoreOut
                result = True
                Exit For
            End If
        Next

        Return result

    End Function

    Private Function DammiPatternsRicercaNull(ByVal valore As String) As List(Of Tuple(Of String, String, String))

        Return New List(Of Tuple(Of String, String, String)) From
            {
                New Tuple(Of String, String, String)("", "", valore),
                New Tuple(Of String, String, String)("", "", String.Format("'{0}'", valore))
            }

    End Function

    Private Function ApplicaPatternRicerca(ByVal sb As StringBuilder,
                                           ByVal patterns As List(Of Tuple(Of String, String, String)),
                                           ByRef p As SqlParameter,
                                           ByRef parametriOutput As List(Of SqlParameter)
                                           ) As Boolean

        Dim indiceNellaStringa As Integer
        Dim patternRisoloto As Boolean = False

        For Each pattern As Tuple(Of String, String, String) In patterns
            indiceNellaStringa = sb.ToString().LastIndexOf(pattern.Item3, StringComparison.InvariantCultureIgnoreCase)
            If indiceNellaStringa <> -1 Then
                If p.Value.ToString.ToUpper.Equals("NULL") Then
                    p.Value = DBNull.Value
                Else
                    p.Value = String.Format("{0}{1}{2}", pattern.Item1, p.Value, pattern.Item2)
                End If

                parametriOutput.Add(p)
                sb.Remove(indiceNellaStringa, pattern.Item3.Length)
                sb.Insert(indiceNellaStringa, p.ParameterName)
                patternRisoloto = True
                Exit For
            End If
        Next

        Return patternRisoloto

    End Function

    Private Function DoppioCheckSicurezza(ByVal stringSqlOriginale As String,
                                          ByVal stringSqlParametrizzata As String,
                                          ByVal parametriOutput As List(Of SqlParameter)) As Boolean

        If _nomenclatoreParametri.ContieneNomenclatura(stringSqlParametrizzata) Then
            Return False
        End If

        Dim stringaControllo As String = stringSqlParametrizzata

        For Each p As SqlParameter In parametriOutput
            Dim tipoParametro = _nomenclatoreParametri.OttieniTipoParametro(p.ParameterName)
            Dim valoreDaSostituire As String = ""

            If p.Value.Equals(DBNull.Value) Then
                valoreDaSostituire = "NULL"
            Else
                Select Case tipoParametro
                    Case GetType(String)
                        valoreDaSostituire = String.Format("'{0}'", UtilityProvider.Agro_SQL_SaveText(p.Value))
                    Case GetType(Date)
                        valoreDaSostituire = String.Format("'{0}'", p.Value)
                    Case GetType(Decimal)
                        valoreDaSostituire = UtilityProvider.Agro_SQL_SaveNum(p.Value)

                End Select
            End If

            If valoreDaSostituire.ToLower().Contains(p.ParameterName.ToLower) Then
                Return False
            End If

            stringaControllo = stringaControllo.ReplaceCaseInsensitive(p.ParameterName, valoreDaSostituire)
        Next

        Return stringSqlOriginale.ToLowerInvariant.Equals(stringaControllo.ToLowerInvariant)

    End Function

    Private Function DammiOccorrenzeSottostringa(ByVal stringaInput As String, ByVal stringaDaCercare As String) As Integer

        Dim myMatches As MatchCollection
        Dim myPattern As New Regex(stringaDaCercare, RegexOptions.None, TimeSpan.FromSeconds(3))
        myMatches = myPattern.Matches(stringaInput)



        Return System.Text.RegularExpressions.Regex.Split(stringaInput, stringaDaCercare, RegexOptions.None, TimeSpan.FromSeconds(3)).Length - 1

    End Function

#Region "Agro_SQL_SaveText"

    Public Function Agro_SQL_SaveText(ByVal Testo As String, Optional ByVal creaParametroSql As Boolean = True,
                                      Optional ByVal injectionGuid As Guid = Nothing) As String Implements IParametrizzatore.Agro_SQL_SaveText

        If creaParametroSql Then
            Return Agro_SQL_SaveText_Con_Parametro(Testo.ToOrigin, injectionGuid)
        End If

        Return Agro_SQL_SaveText_Senza_Parametro(Testo)

    End Function

    Private Function Agro_SQL_SaveText_Da_Filtro_Aggiuntivo(ByVal Testo As String, Optional ByVal creaParametroSql As Boolean = True,
                                      Optional ByVal injectionGuid As Guid = Nothing) As String

        If creaParametroSql Then
            Return Agro_SQL_SaveText_Con_Parametro(Testo.ToOrigin, injectionGuid, True)
        End If

        Return Agro_SQL_SaveText_Senza_Parametro(Testo)

    End Function

    Private Function Agro_SQL_SaveText_Senza_Parametro(ByVal Testo As String) As String

        'Se la stringa e' nulla, restituisco la stringa nulla
        If IsNothing(Testo) Then
            Testo = ""
            Return Testo
        End If

        'Sostituisco i singoli apici con due singoli apici
        Testo = Testo.Replace("'", "''")

        'Restituisco il risultato
        Return Testo

    End Function

    Private Function Agro_SQL_SaveText_Con_Parametro(
                                ByVal Testo As String,
                                Optional ByVal injectionGuid As Guid = Nothing,
                                Optional ByVal daFiltroAggiuntivo As Boolean = False) As String

        'Sostituisco i singoli apici con due singoli apici
        Dim nomenclatura As Tuple(Of String, String) = _nomenclatoreParametri.OttieniNomenclatura(Testo.GetType())
        Dim valoreParametro As String = ""

        'Se la stringa e' nulla, restituisco la stringa nulla
        If IsNothing(Testo) Then
            Testo = ""
            Return Testo
        End If

        valoreParametro = String.Format("{0}{1}{2}", nomenclatura.Item1, Testo, nomenclatura.Item2)
        Testo = String.Format("{0}{1}{2}", nomenclatura.Item1, Testo.Replace("'", "''"), nomenclatura.Item2)
        AggiungiParametro(valoreParametro, Testo.GetType(), injectionGuid)

        'Restituisco il risultato
        Return Testo

    End Function

#End Region

#Region "Agro_SQL_SaveText_UNICODE"

    Public Function Agro_SQL_SaveText_UNICODE(Testo As String, par As String, Optional creaParametroSql As Boolean = True, Optional injectionGuid As Guid = Nothing) As String Implements IParametrizzatore.Agro_SQL_SaveText_UNICODE

        If creaParametroSql Then
            Return Agro_SQL_SaveText_Con_Parametro(Testo.ToOrigin, injectionGuid)
        End If

        Return par & "'" & Testo & "'"

    End Function

#End Region

#Region "Agro_SQL_SaveText_NULL"
    Public Function Agro_SQL_SaveText_NULL(ByVal item As Object, Optional ByVal creaParametroSql As Boolean = True, Optional ByVal injectionGuid As Guid = Nothing) As String Implements IParametrizzatore.Agro_SQL_SaveText_NULL

        If creaParametroSql Then
            Return Agro_SQL_SaveText_NULL_Con_Parametro(item, injectionGuid)
        End If

        Return Agro_SQL_SaveText_NULL_Senza_Parametro(item)

    End Function

    Private Function Agro_SQL_SaveText_NULL_Con_Parametro(ByVal item As Object, Optional ByVal injectionGuid As Guid = Nothing) As String

        Dim testo As String
        Dim tipo As Type = GetType(String)
        Dim nomenclatura As Tuple(Of String, String) = _nomenclatoreParametri.OttieniNomenclatura(tipo)
        If IsDBNull(item) Then
            testo = String.Format("{0}{1}{2}", nomenclatura.Item1, "NULL", nomenclatura.Item2)
            AggiungiParametro(testo, tipo, injectionGuid)
            Return testo
        End If

        testo = CStr(item).ToOrigin
        If IsNothing(testo) Then
            testo = String.Format("{0}{1}{2}", nomenclatura.Item1, "NULL", nomenclatura.Item2)
            AggiungiParametro(testo, tipo, injectionGuid)
            Return testo
        End If

        Dim valoreParametro As String = String.Format("{0}{1}{2}", nomenclatura.Item1, testo, nomenclatura.Item2)
        testo = String.Format("{0}{1}{2}", nomenclatura.Item1, testo.Replace("'", "''"), nomenclatura.Item2)
        AggiungiParametro(valoreParametro, testo.GetType(), injectionGuid)

        Return "'" & testo & "'"


    End Function

    Private Function Agro_SQL_SaveText_NULL_Senza_Parametro(ByVal item As Object) As String

        If IsDBNull(item) Then
            Return "NULL"
        End If

        Dim Testo As String = CStr(item)

        'Se la stringa e' nulla, restituisco la stringa nulla
        If IsNothing(Testo) Then
            Testo = "NULL"
            Return Testo
        End If

        'Sostituisco i singoli apici con due singoli apici
        Testo = Testo.Replace("'", "''")

        'Restituisco il risultato
        Return "'" & Testo & "'"

    End Function

#End Region

#Region "Agro_SQL_Save_Clausola_IN"
    Public Function Agro_SQL_Save_Clausola_IN(ByVal clausolaIN As String, ByVal valoriStringa As Boolean, Optional ByVal creaParametriSql As Boolean = True, Optional ByVal injectionGuid As Guid = Nothing) As String Implements IParametrizzatore.Agro_SQL_Save_Clausola_IN

        If creaParametriSql Then
            Return Agro_SQL_Save_Clausola_IN_Con_Parametri(clausolaIN.ToOrigin, valoriStringa, injectionGuid)
        End If

        Return Agro_SQL_Save_Clausola_IN_Senza_Parametri(clausolaIN)

    End Function
    Private Function Agro_SQL_Save_Clausola_IN_Con_Parametri(ByVal clausolaIN As String, ByVal valoriStringa As Boolean, Optional ByVal injectionGuid As Guid = Nothing) As String

        Dim sb = New StringBuilder
        Dim parentesiIncluse As Boolean = False
        Dim clausolaInOriginale = clausolaIN

        'Se la stringa e' nulla, restituisco la stringa nulla
        If String.IsNullOrEmpty(clausolaIN) Then
            Return String.Empty
        End If

        clausolaIN = clausolaIN.Trim
        If clausolaIN.StartsWith("(") AndAlso clausolaIN.EndsWith(")") Then
            parentesiIncluse = True
            clausolaIN = clausolaIN.Substring(1, clausolaIN.Length - 2)
        End If


        'Sostituisco i singoli apici con due singoli apici
        Dim tipoParametri As Type = If(valoriStringa, GetType(String), GetType(Decimal))
        Dim nomenclatura As Tuple(Of String, String) = _nomenclatoreParametri.OttieniNomenclatura(tipoParametri)

        Dim ins = clausolaIN.Split({","c}).Where(Function(s) s <> String.Empty).ToList
        If Not valoriStringa Then
            Dim maxElementiClausolaIn = If(Not IsNothing(DataProviderFactory.Instance.ConfigurazioneEstesa), DataProviderFactory.Instance.ConfigurazioneEstesa.LimiteElementiClausoleIn, 50)
            If ins.Count > maxElementiClausolaIn Then
                Try
                    ControllaInjection("select * from x WHERE " & clausolaInOriginale, True, True)
                    Return clausolaInOriginale
                Catch ex As UnauthorizedAccessException
                End Try
            End If
        End If

        For Each s As String In ins
            Dim valoreParametro As String = ""
            If valoriStringa Then
                Dim sTras = String.Empty
                s = s.Trim
                If s.StartsWith("'") AndAlso s.EndsWith("'") Then
                    s = s.Substring(1).Substring(0, s.Length - 2)
                End If
                sTras = s
                valoreParametro = String.Format("{0}{1}{2}", nomenclatura.Item1, sTras, nomenclatura.Item2)
                sb.Append(String.Format("'{0}',", valoreParametro.Replace("'", "''")))
            Else
                valoreParametro = String.Format("{0}{1}{2}", nomenclatura.Item1, s.Trim.Replace("'", ""), nomenclatura.Item2)
                sb.Append(String.Format("{0},", valoreParametro))
            End If
            AggiungiParametro(valoreParametro, tipoParametri, injectionGuid)
        Next

        Dim valore As String = sb.ToString
        If Not parentesiIncluse Then
            Return valore.Remove(valore.LastIndexOf(","), 1)
        Else
            Return "(" & valore.Remove(valore.LastIndexOf(","), 1) & ")"
        End If

    End Function

    Private Function Agro_SQL_Save_Clausola_IN_Senza_Parametri(ByVal clausolaIN As String) As String
        Return clausolaIN
    End Function

#End Region

#Region "Agro_SQL_SaveDate"
    Public Function Agro_SQL_SaveDate(ByVal DataItaliana As Date, Optional ByVal creaParametroSql As Boolean = True,
                                      Optional ByVal injectionGuid As Guid = Nothing) As String Implements IParametrizzatore.Agro_SQL_SaveDate

        If creaParametroSql Then
            Return Agro_SQL_SaveDate_Con_Parametro(DataItaliana, injectionGuid)
        End If

        Return Agro_SQL_SaveDate_Senza_Parametro(DataItaliana)

    End Function

    Private Function Agro_SQL_SaveDate_Senza_Parametro(ByVal DataItaliana As Date) As String

        Dim testo As String

        If DataItaliana = New Date Then
            testo = "Null"
        Else
            testo = " CONVERT(DateTime,'" & Format(DataItaliana, "yyyy/MM/dd") & "',120) "
        End If

        Return testo

    End Function

    Private Function Agro_SQL_SaveDate_Con_Parametro(ByVal DataItaliana As Date, Optional ByVal injectionGuid As Guid = Nothing) As String

        Dim nomenclatura As Tuple(Of String, String) = _nomenclatoreParametri.OttieniNomenclatura(DataItaliana.GetType())
        Dim testo As String = ""
        Dim valoreParametro As String = String.Empty

        If DataItaliana = New Date Then
            testo = String.Format("{0}{1}{2}", nomenclatura.Item1, "Null", nomenclatura.Item2)
            valoreParametro = testo
        Else

            testo = " CONVERT(DateTime,'" & String.Format("{0}{1}{2}", nomenclatura.Item1, Format(DataItaliana, "yyyy/MM/dd"), nomenclatura.Item2) & "',120) "
            valoreParametro = String.Format("{0}{1}{2}", nomenclatura.Item1, Format(DataItaliana, "yyyy/MM/dd"), nomenclatura.Item2)
        End If
        AggiungiParametro(valoreParametro, DataItaliana.GetType(), injectionGuid)

        Return testo

    End Function
#End Region

#Region "Agro_SQL_SaveDateTime"
    Public Function Agro_SQL_SaveDateTime(ByVal DataOraItaliana As DateTime, Optional ByVal creaParametroSql As Boolean = True, Optional ByVal injectionGuid As Guid = Nothing) As String Implements IParametrizzatore.Agro_SQL_SaveDateTime

        If creaParametroSql Then
            Return Agro_SQL_SaveDateTime_Con_Parametro(DataOraItaliana, injectionGuid)
        End If

        Return Agro_SQL_SaveDateTime_Senza_Parametro(DataOraItaliana)


    End Function

    Private Function Agro_SQL_SaveDateTime_Senza_Parametro(ByVal DataOraItaliana As Date) As String

        Dim Testo = " CONVERT(DateTime,'" & Format(DataOraItaliana, "yyyy/MM/dd") & " " & Replace(Format(DataOraItaliana, "HH:mm:ss.fff"), ".", ":") & "',120) "
        Return Testo

    End Function

    Private Function Agro_SQL_SaveDateTime_Con_Parametro(ByVal DataOraItaliana As Date, Optional ByVal injectionGuid As Guid = Nothing) As String

        Dim nomenclatura As Tuple(Of String, String) = _nomenclatoreParametri.OttieniNomenclatura(DataOraItaliana.GetType())
        Dim testo As String = ""
        Dim valoreParametro As String = String.Empty

        testo = " CONVERT(DateTime,'" & String.Format("{0}{1}{2}", nomenclatura.Item1, Format(DataOraItaliana, "yyyy/MM/dd") & " " & Replace(Format(DataOraItaliana, "HH:mm:ss.fff"), ".", ":"), nomenclatura.Item2) & "',120) "
        valoreParametro = String.Format("{0}{1}{2}", nomenclatura.Item1, Format(DataOraItaliana, "yyyy/MM/dd") & " " & Replace(Format(DataOraItaliana, "HH:mm:ss.fff"), ".", ":"), nomenclatura.Item2)

        AggiungiParametro(valoreParametro, DataOraItaliana.GetType, injectionGuid)

        Return testo

    End Function

    Private Function Agro_SQL_SaveDateTime_Da_Filtro_Aggiuntivo(ByVal dataOraStringa As String,
                                                                Optional ByVal creaParametroSql As Boolean = True,
                                                                Optional ByVal injectionGuid As Guid = Nothing) As String

        If creaParametroSql Then
            Dim nomenclatura As Tuple(Of String, String) = _nomenclatoreParametri.OttieniNomenclatura(GetType(Date))
            Dim valoreParametro As String = String.Format("{0}{1}{2}", nomenclatura.Item1, dataOraStringa, nomenclatura.Item2)
            Dim testo As String = valoreParametro
            AggiungiParametro(valoreParametro, GetType(Date), injectionGuid)
            Return testo
        End If

        Return dataOraStringa

    End Function
#End Region

#Region "Agro_SQL_SaveDateTime_NULL"
    Public Function Agro_SQL_SaveDateTime_NULL(ByVal item As Object, Optional ByVal creaParametroSql As Boolean = True, Optional ByVal injectionGuid As Guid = Nothing) As String Implements IParametrizzatore.Agro_SQL_SaveDateTime_NULL

        If creaParametroSql Then
            Return Agro_SQL_SaveDateTime_NULL_Con_Parametro(item, injectionGuid)
        End If

        Return Agro_SQL_SaveDateTime_NULL_Senza_Parametro(item)

    End Function

    Private Function Agro_SQL_SaveDateTime_NULL_Con_Parametro(ByVal item As Object, Optional ByVal injectionGuid As Guid = Nothing) As String

        Dim testo As String
        Dim tipo As Type = GetType(Date)
        Dim nomenclatura As Tuple(Of String, String) = _nomenclatoreParametri.OttieniNomenclatura(tipo)

        If IsDBNull(item) Then
            testo = String.Format("{0}{1}{2}", nomenclatura.Item1, "NULL", nomenclatura.Item2)
            AggiungiParametro(testo, tipo, injectionGuid)
            Return testo
        End If

        Dim DataOraItaliana As DateTime = CDate(item)
        If DataOraItaliana = New Date Then
            testo = String.Format("{0}{1}{2}", nomenclatura.Item1, "NULL", nomenclatura.Item2)
            AggiungiParametro(testo, tipo, injectionGuid)
            Return testo
        End If

        testo = " CONVERT(DateTime,'" & String.Format("{0}{1}{2}", nomenclatura.Item1, Format(DataOraItaliana, "yyyy/MM/dd") & " " & Replace(Format(DataOraItaliana, "HH:mm:ss.fff"), ".", ":"), nomenclatura.Item2) & "',120) "
        Dim valoreParametro = String.Format("{0}{1}{2}", nomenclatura.Item1, Format(DataOraItaliana, "yyyy/MM/dd") & " " & Replace(Format(DataOraItaliana, "HH:mm:ss.fff"), ".", ":"), nomenclatura.Item2)
        AggiungiParametro(valoreParametro, tipo, injectionGuid)

        Return testo

    End Function

    Private Function Agro_SQL_SaveDateTime_NULL_Senza_Parametro(ByVal item As Object) As String

        If IsDBNull(item) Then
            Return "NULL"
        End If

        Dim DataOraItaliana As DateTime = CDate(item)
        If DataOraItaliana = New Date Then
            Return "NULL"
        End If
        Dim Testo = " CONVERT(DateTime,'" & Format(DataOraItaliana, "yyyy/MM/dd") & " " & Replace(Format(DataOraItaliana, "HH:mm:ss.fff"), ".", ":") & "',120) "
        Return Testo

    End Function

#End Region

#Region "Agro_SQL_SaveNum"
    Public Function Agro_SQL_SaveNum(ByVal StringaNumero As String, Optional ByVal creaParametroSql As Boolean = True,
                                     Optional ByVal injectionGuid As Guid = Nothing) As String Implements IParametrizzatore.Agro_SQL_SaveNum

        If creaParametroSql Then
            Return Agro_SQL_SaveNum_Con_Parametro(StringaNumero.ToOrigin, injectionGuid)
        End If

        Return Agro_SQL_SaveNum_Senza_Parametro(StringaNumero)

    End Function

    Private Function Agro_SQL_SaveNum_Da_Filtro_Aggiuntivo(ByVal StringaNumero As String, Optional ByVal creaParametroSql As Boolean = True,
                                     Optional ByVal injectionGuid As Guid = Nothing) As String

        If creaParametroSql Then
            Return Agro_SQL_SaveNum_Con_Parametro(StringaNumero.ToOrigin, injectionGuid, True)
        End If

        Return Agro_SQL_SaveNum_Senza_Parametro(StringaNumero)

    End Function

    Private Function Agro_SQL_SaveNum_Con_Parametro(
                                                   ByVal StringaNumero As String,
                                                   Optional ByVal injectionGuid As Guid = Nothing,
                                                   Optional ByVal daFiltroAggiuntivo As Boolean = False) As String

        Dim StrSeparatoreDecimale As String = String.Empty
        Dim ValoreFinale As String = String.Empty

        Dim nomenclatura As Tuple(Of String, String) = _nomenclatoreParametri.OttieniNomenclatura(GetType(Decimal))

        'Recupero il Separatore Decimale di Sistema
        StrSeparatoreDecimale = SeparatoreDecimale()

        'Verifico il separatore decimale desiderato
        If (StrSeparatoreDecimale <> ",") AndAlso (StrSeparatoreDecimale <> ".") Then
            StringaNumero = "0"
            ValoreFinale = String.Format("{0}{1}{2}", nomenclatura.Item1, StringaNumero, nomenclatura.Item2)
            AggiungiParametro(ValoreFinale, GetType(Decimal), injectionGuid)
            Return ValoreFinale
        End If

        'Se StringaNumero e' nullo, restituisco uno zero (0)
        If IsNothing(StringaNumero) Then
            StringaNumero = "0"
            ValoreFinale = String.Format("{0}{1}{2}", nomenclatura.Item1, StringaNumero, nomenclatura.Item2)
            AggiungiParametro(ValoreFinale, GetType(Decimal), injectionGuid)
            Return ValoreFinale
        End If

        'Se StringaNumero e' una stringa vuota, restituisco lo zero (0)
        If StringaNumero = "" Then
            StringaNumero = "0"
            ValoreFinale = String.Format("{0}{1}{2}", nomenclatura.Item1, StringaNumero, nomenclatura.Item2)
            AggiungiParametro(ValoreFinale, GetType(Decimal), injectionGuid)
            Return ValoreFinale
        End If

        'Se StringaNumero non e' numerico, restituisco lo zero (0)
        If Not IsNumeric(StringaNumero.ToString) Then
            StringaNumero = "0"
            ValoreFinale = String.Format("{0}{1}{2}", nomenclatura.Item1, StringaNumero, nomenclatura.Item2)
            AggiungiParametro(ValoreFinale, GetType(Decimal), injectionGuid)
            Return ValoreFinale
        End If

        'Verifico in quale situazione mi trovo
        If StrSeparatoreDecimale = "," Then

            '===== Caso ITALIANO =====

            'Elimino i punti
            StringaNumero = Replace(StringaNumero, ".", "")

            'Sostituisco la virgola col punto
            StringaNumero = Replace(StringaNumero, ",", ".")

        Else

            '===== Caso ANGLOSASSONE =====

            'Elimino le virgole
            StringaNumero = Replace(StringaNumero, ",", "")

        End If

        ValoreFinale = String.Format("{0}{1}{2}", nomenclatura.Item1, StringaNumero, nomenclatura.Item2)
        AggiungiParametro(ValoreFinale, GetType(Decimal), injectionGuid)
        Return ValoreFinale

    End Function



    Private Function Agro_SQL_SaveNum_Senza_Parametro(ByVal StringaNumero As String) As String

        'Recupero il Separatore Decimale di Sistema
        Dim StrSeparatoreDecimale As String = SeparatoreDecimale()

        'Verifico il separatore decimale desiderato
        If (StrSeparatoreDecimale <> ",") AndAlso (StrSeparatoreDecimale <> ".") Then
            Return "0"
        End If

        'Se StringaNumero e' nullo, restituisco uno zero (0)
        If IsNothing(StringaNumero) Then
            Return "0"
        End If

        'Se StringaNumero e' una stringa vuota, restituisco lo zero (0)
        If StringaNumero = "" Then
            Return "0"
        End If

        'Se StringaNumero non e' numerico, restituisco lo zero (0)
        If Not IsNumeric(StringaNumero.ToString) Then
            Return "0"
        End If

        'Verifico in quale situazione mi trovo
        If StrSeparatoreDecimale = "," Then

            '===== Caso ITALIANO =====

            'Elimino i punti
            StringaNumero = Replace(StringaNumero, ".", "")

            'Sostituisco la virgola col punto
            StringaNumero = Replace(StringaNumero, ",", ".")

        Else

            '===== Caso ANGLOSASSONE =====

            'Elimino le virgole
            StringaNumero = Replace(StringaNumero, ",", "")

        End If

        Return StringaNumero

    End Function
#End Region

#Region "Agro_SQL_Save_xFiltroAggiuntivo"
    Public Function Agro_SQL_Save_xFiltroAggiuntivo(ByVal filtro As String,
                                                    Optional ByRef creaParametriSql As Boolean = True,
                                                    Optional ByVal injectionGuid As Guid = Nothing,
                                                    Optional ByVal objParametri As AgronicaCoreParametri = Nothing) As String Implements IParametrizzatore.Agro_SQL_Save_xFiltroAggiuntivo

        Dim sb As New StringBuilder
        Dim messaggioErrore As String = String.Empty

        'E' necessario aggiungere una "select from where" fittizia per poter utilizzare il parser xml
        Dim filtroEsteso = " select * from x where " & filtro
        Dim xmlDoc As XmlDocument = Nothing
        Dim nomeRoutine = "AgronicaCoreDataProvider.Parametrizzatore.Agro_SQL_Save_xFiltroAggiuntivo"

        Try
            If DataProviderFactory.Instance.TipoProvider = TipiEnumerativi.enum_DataProvidersType.OleDbProvider Then
                Return Agro_SQL_Save_xFiltroAggiuntivo_Senza_Parametri(filtro)
            End If

            If creaParametriSql Then
                xmlDoc = ControllaInjection(filtroEsteso, True, creaParametriSql, objParametri)
            Else
                Return Agro_SQL_Save_xFiltroAggiuntivo_Senza_Parametri(filtro)
            End If

        Catch ex As UnauthorizedAccessException
            creaParametriSql = False
            messaggioErrore = String.Format("---> RILEVATA INJECTION NEL FILTRO AGGIUNTIVO" & vbCrLf & "Filtro Aggiuntivo: {0}" & vbCrLf & "Info parser: {1}", filtro, ex.Message)
            sb.Append(messaggioErrore)
            sb.AppendLine()
            sb.Append(String.Format("StackTrace: " & vbCancel & "{0}", DammiStackTrace()))
            Logga(nomeRoutine, sb.ToString, objParametri)
            Dim cfgEstesa = ConfigurazioneEstesaSqlProviderFactory.Instance(objParametri)
            If If(cfgEstesa IsNot Nothing, cfgEstesa.LanciaEccezioneSuInjection, CBool(ConfigurationManager.AppSettings("LanciaEccezioneSuInjection"))) Then
                Throw ex
            Else
                If Debugger.IsAttached Then
                    Throw New ParametrizzatoreException(sb.ToString)
                Else
                    Return filtro
                End If
            End If

        Catch ex As TempoParserException
            creaParametriSql = False
            messaggioErrore = String.Format("---> RILEVATA STRINGA SQL CON TEMPO PARSER ELEVATO" & vbCrLf & "Filtro Aggiuntivo: {0}" & vbCrLf & "Info parser: {1}", filtro, ex.Message)
            sb.Append(messaggioErrore)
            sb.AppendLine()
            sb.Append(String.Format("StackTrace: " & vbCancel & "{0}", DammiStackTrace()))
            Logga(nomeRoutine, sb.ToString, objParametri)
            Return filtro

        Catch ex As OutOfMemoryException
            creaParametriSql = False
            messaggioErrore = String.Format("---> RILEVATA STRINGA SQL ECCESSIVAMENTE LUNGA PER IL PARSER" & vbCrLf & "Filtro Aggiuntivo: {0}" & vbCrLf & "Info parser: {1}", filtro, ex.Message)
            sb.Append(messaggioErrore)
            sb.AppendLine()
            sb.Append(String.Format("StackTrace: " & vbCancel & "{0}", DammiStackTrace()))
            Logga(nomeRoutine, sb.ToString, objParametri)
            Return filtro

        Catch ex As Exception
            creaParametriSql = False
            messaggioErrore = String.Format("---> ECCEZIONE NON GESTITA NEL PARSER" & vbCrLf & "Filtro Aggiuntivo: {0}" & vbCrLf & "Info parser: {1}", filtro, ex.Message)
            sb.Append(messaggioErrore)
            sb.AppendLine()
            sb.Append(String.Format("StackTrace: " & vbCancel & "{0}", DammiStackTrace()))
            Logga(nomeRoutine, sb.ToString, objParametri)
            Return filtro

        End Try

        If creaParametriSql AndAlso Not IsNothing(xmlDoc) Then
            Try
                Return Agro_SQL_Save_xFiltroAggiuntivo_Con_Parametri(filtro, xmlDoc, injectionGuid)
            Catch ex As ArgumentOutOfRangeException
                creaParametriSql = False
                messaggioErrore = String.Format("---> RILEVATA FILTRO AGGIUNTIVO CON TROPPI PARAMETRI" & vbCrLf & "Filtro Aggiuntivo: {0}" & vbCrLf & "Info parser: {1}", filtro, ex.Message)
                sb.Append(messaggioErrore)
                sb.AppendLine()
                sb.Append(String.Format("StackTrace: " & vbCancel & "{0}", DammiStackTrace()))
                Logga(nomeRoutine, sb.ToString, objParametri)
                Return filtro
            Catch ex As Exception
                creaParametriSql = False
                messaggioErrore = String.Format("---> RILEVATA INJECTION NEL FILTRO AGGIUNTIVO" & vbCrLf & "Filtro Aggiuntivo: {0}" & vbCrLf & "Info parser: {1}", filtro, ex.Message)
                sb.Append(messaggioErrore)
                sb.AppendLine()
                sb.Append(String.Format("StackTrace: " & vbCancel & "{0}", DammiStackTrace()))
                Logga(nomeRoutine, sb.ToString, objParametri)
                Return filtro
            End Try

        End If

        Return Agro_SQL_Save_xFiltroAggiuntivo_Senza_Parametri(filtro)

    End Function

    Public Function Agro_SQL_Save_xFiltroAggiuntivo_Semplificato(ByVal filtro As String,
                                                    Optional ByVal creaParametriSql As Boolean = True,
                                                    Optional ByVal injectionGuid As Guid = Nothing,
                                                    Optional ByVal objParametri As AgronicaCoreParametri = Nothing) As String Implements IParametrizzatore.Agro_SQL_Save_xFiltroAggiuntivo_Semplificato

        Dim messaggioErrore As String = String.Empty

        'E' necessario aggiungere una "select from where" fittizia per poter utilizzare il parser xml
        Dim filtroEsteso = " select * from x where " & filtro
        Dim nomeRoutine = "AgronicaCoreDataProvider.Parametrizzatore.Agro_SQL_Save_xFiltroAggiuntivo"

        'traduco la stringa in xml
        Dim parseOptions As New ParseOptions(";")
        Dim parser = Microsoft.SqlServer.Management.SqlParser.Parser.Parser.Parse(filtroEsteso, parseOptions)
        Dim stringaXml As String = String.Empty
        Dim xmlDoc As New XmlDocument()

        If DataProviderFactory.Instance.TipoProvider = TipiEnumerativi.enum_DataProvidersType.OleDbProvider Then
            Return Agro_SQL_Save_xFiltroAggiuntivo_Senza_Parametri(filtro)
        End If

        If creaParametriSql Then
            Try
                stringaXml = parser.Script.Xml
            Catch ex As Exception
                Throw New OutOfMemoryException(ex.Message)
            End Try
            xmlDoc.LoadXml(stringaXml)
            If Not IsNothing(xmlDoc) Then
                Return Agro_SQL_Save_xFiltroAggiuntivo_Con_Parametri(filtro, xmlDoc, injectionGuid)
            Else
                Return Agro_SQL_Save_xFiltroAggiuntivo_Senza_Parametri(filtro)
            End If
        Else
            Return Agro_SQL_Save_xFiltroAggiuntivo_Senza_Parametri(filtro)
        End If

    End Function

    Private Function Agro_SQL_Save_xFiltroAggiuntivo_Con_Parametri(ByVal filtro As String,
                                                                   ByVal xmlDoc As XmlDocument,
                                                                   Optional ByVal injectionGuid As Guid = Nothing) As String

        'Utilizzando xml, è ancora necessaria l'aggiunta di una parte fittizia che verrà poi rimossa alla fine
        Dim addFiltro = "select * from x where "
        Dim filtroEsteso = addFiltro & filtro


        Dim nodeLiteral As XmlNode
        Dim location As String()
        Dim startPosition As Integer
        Dim endPosition As Integer
        Dim minus As Boolean = False
        Dim attr As XmlAttribute

        'Seleziono ogni singola espressione terminale nella query
        Dim queryLiteral As XmlNodeList = xmlDoc.SelectNodes(".//SqlLiteralExpression | .//SqlUnaryScalarExpression")

        If queryLiteral.Count > MAX_PARAMETRI Then
            Throw New ArgumentOutOfRangeException(String.Format("N° Eccessivo di parametri. ( {0} )", queryLiteral.Count.ToString))
        End If

        'Itero al contrario sulle espressioni ricavate così da mantenere i riferimenti sulla posizione presenti nell'xml validi anche durante la parametrizzazione
        For cont As Integer = queryLiteral.Count - 1 To 0 Step -1

            nodeLiteral = queryLiteral.Item(cont)

            'Controllo se la literal in questione è in realtà un numero negativo (e dunque è necessario il parent)
            If (cont >= 1 AndAlso
                queryLiteral.Item(cont - 1).Name.Contains("SqlUnaryScalarExpression") AndAlso
                String.Compare(queryLiteral.Item(cont - 1).ChildNodes.Item(1).Attributes("Value").Value, nodeLiteral.Attributes("Value").Value) = 0 AndAlso
                String.Compare(nodeLiteral.Attributes("Type").Value, "Integer") = 0) Then

                nodeLiteral = queryLiteral.Item(cont - 1)
                cont -= 1
                attr = xmlDoc.CreateAttribute("Value")
                attr.Value = nodeLiteral.FirstChild.InnerText
                nodeLiteral.Attributes.SetNamedItem(attr)
                attr = xmlDoc.CreateAttribute("Type")
                attr.Value = "Integer"
                nodeLiteral.Attributes.SetNamedItem(attr)

            End If

            'Controllo che la literal expression non sia il codice numerico dentro una convert
            'in tal caso, lo ignoro
            If Not (String.Compare(nodeLiteral.Attributes("Type").Value, "Integer") = 0 AndAlso
                (nodeLiteral.ParentNode.Name.Contains("SqlConvertExpression") AndAlso
                nodeLiteral.ParentNode.ChildNodes.Item(1).FirstChild.InnerText.Contains("DateTime") OrElse
                (nodeLiteral.ParentNode.Attributes("FunctionName") IsNot Nothing AndAlso
                nodeLiteral.ParentNode.Attributes("FunctionName").Value.Contains("substring")))) Then

                'Estraggo l'attributo "Location" che identifica la posizione di inizio e fine dell' espressione 
                '(il -1 è per portare la numerazione da uno spazio [1,end] a uno [0,end-1] compatibile con gli array)
                If (String.Compare(nodeLiteral.ParentNode.Name, "SqlTopSpecification") <> 0) Then

                    location = nodeLiteral.Attributes("Location").Value.Split({")"c})
                    startPosition = Integer.Parse(location(0).Split({","c})(1)) - If(nodeLiteral.Attributes("Type").Value = "String", 1, 2)
                    endPosition = Integer.Parse(location(1).Split({","c})(2)) - If(nodeLiteral.Attributes("Type").Value = "String", 3, 2)

                    'rimuovo il vecchio valore dell'espressione dalla stringa SQL
                    filtroEsteso = filtroEsteso.Remove(startPosition, endPosition - startPosition - If(nodeLiteral.Attributes("Type").Value = "UnicodeString", 1, 0))

                    Dim valoreTipo As String = nodeLiteral.Attributes("Type").Value
                    Dim valoreStringa = nodeLiteral.Attributes("Value").Value

                    'a seconda del tipo chiamo uno dei metodi per parametrizzare 
                    Select Case (valoreTipo)
                        Case "String"
                            Dim patternMatchato As Boolean = False
                            Dim patterns_Json = DammiPatternsRicerca_Json()

                            For Each json In patterns_Json
                                If valoreStringa.Contains(json) Then
                                    patternMatchato = True
                                    Exit For
                                End If
                            Next

                            If patternMatchato Then
                                filtroEsteso = filtroEsteso.Insert(startPosition, Agro_SQL_SaveText_Da_Filtro_Aggiuntivo(valoreStringa, True, injectionGuid))
                            Else
                                filtroEsteso = filtroEsteso.Insert(startPosition, Agro_SQL_SaveText_Da_Filtro_Aggiuntivo(valoreStringa.Replace(Chr(34), ""), True, injectionGuid))
                            End If

                        Case "UnicodeString"
                            filtroEsteso = filtroEsteso.Insert(startPosition, "'" & Agro_SQL_SaveText_Da_Filtro_Aggiuntivo(valoreStringa.Replace(Chr(34), ""), True, injectionGuid))
                        Case "Integer", "Numeric"
                            filtroEsteso = filtroEsteso.Insert(startPosition, Agro_SQL_SaveNum_Da_Filtro_Aggiuntivo(valoreStringa, True, injectionGuid))
                        Case Else
                            Throw New ArgumentOutOfRangeException("Tipo di dato non riconosciuto come stringa, intero o data")
                    End Select

                End If

            End If

        Next

        'rimuovo la parte fittizia aggiunta all'inizio
        filtroEsteso = filtroEsteso.Remove(0, addFiltro.Length)
        Return filtroEsteso

    End Function

    Private Function Agro_SQL_Save_xFiltroAggiuntivo_Senza_Parametri(ByVal filtro As String) As String

        Return filtro

    End Function
#End Region


#Region "Agro_SQL_Save_xOrderBy"
    Public Function Agro_SQL_Save_xOrderBy(filtro As String, Optional ByVal objParametri As AgronicaCoreParametri = Nothing) As String Implements IParametrizzatore.Agro_SQL_Save_xOrderBy

        Dim xmlDoc As XmlDocument
        Dim nomeRoutine = "AgronicaCoreDataProvider.Parametrizzatore.Agro_SQL_Save_xOrderBy"
        Dim filtroOriginale As String = filtro
        Dim sb As New StringBuilder
        Dim messaggioErrore As String = String.Empty

        filtro = "select * from x order by " & filtro

        Try

            If DataProviderFactory.Instance.TipoProvider = TipiEnumerativi.enum_DataProvidersType.OleDbProvider Then
                Return filtroOriginale
            End If

            xmlDoc = ControllaInjection(filtro, False, False, objParametri)
        Catch ex As UnauthorizedAccessException
            messaggioErrore = String.Format("---> RILEVATA INJECTION NELLA STRINGA ORDER BY" & vbCrLf & "Order By: {0}" & vbCrLf & "Info parser: {1}", filtro, ex.Message)
            sb.Append(messaggioErrore)
            sb.AppendLine()
            sb.Append(String.Format("StackTrace: " & vbCancel & "{0}", DammiStackTrace()))
            Logga(nomeRoutine, sb.ToString, objParametri)
            Dim cfgEstesa = ConfigurazioneEstesaSqlProviderFactory.Instance(objParametri)
            If If(cfgEstesa IsNot Nothing, cfgEstesa.LanciaEccezioneSuInjection, CBool(ConfigurationManager.AppSettings("LanciaEccezioneSuInjection"))) Then
                Throw ex
            Else
                Return filtroOriginale
            End If
        Catch ex As OutOfMemoryException
            messaggioErrore = String.Format("---> RILEVATA STRINGA SQL ECCESSIVAMENTE LUNGA PER IL PARSER" & vbCrLf & "Order By: {0}" & vbCrLf & "Info parser: {1}", filtro, ex.Message)
            sb.Append(messaggioErrore)
            sb.AppendLine()
            sb.Append(String.Format("StackTrace: " & vbCancel & "{0}", DammiStackTrace()))
            Logga(nomeRoutine, sb.ToString, objParametri)
            Return filtroOriginale
        Catch ex As Exception
            messaggioErrore = String.Format("---> ECCEZIONE NON GESTITA NEL PARSER " & vbCrLf & "Order By: {0}" & vbCrLf & "Info parser: {1}", filtro, ex.Message)
            sb.Append(messaggioErrore)
            sb.AppendLine()
            sb.Append(String.Format("StackTrace: " & vbCancel & "{0}", DammiStackTrace()))
            Logga(nomeRoutine, sb.ToString, objParametri)
            Return filtroOriginale
        End Try

        Return filtroOriginale

    End Function

#End Region

#Region "ControllaInjection"

    Private Function ControllaInjection(ByVal query As String,
                                        ByVal isFiltroAggiuntivo As Boolean,
                                        ByRef creaParametri As Boolean,
                                        Optional ByVal objParametri As AgronicaCoreParametri = Nothing) As XmlDocument

        'scrittura messaggio di errore
        Dim errore As String = " Rilevato possibile tentativo di SQL Injection :: "

        'Prima di procedere alla parametrizzazione controllo la presenza di eventuali parti pericolose nella query

        'Controllo subito se sono presenti dei ";", un possibile tentativo di SQL Injection
        'Controllo inoltre la presenza di "--" e "/*", che indicano l'intenzione di commentare il resto della query prevedendone l'esecuzione
        'If (query.Contains(";") OrElse query.Contains("--") OrElse query.Contains("/*") OrElse Regex.Match(query, "\s*.*\s*from\s*.*\s*sys.*\s*where\s*.*").Success) Then

        '    Throw New UnauthorizedAccessException(errore & "Utilizzo di ; / del prefisso sys / di commenti relativamente sospetti")

        'End If

        If (query.Contains("@@")) Then

            Throw New UnauthorizedAccessException(errore & "Utilizzo di System Function")

        End If

        'traduco la stringa in xml
        Dim parseOptions As New ParseOptions(";")
        Dim parser = Microsoft.SqlServer.Management.SqlParser.Parser.Parser.Parse(query, parseOptions)
        Dim stringaXml As String = String.Empty
        Dim tempoParser As Long = 0

        Try
            Dim overtime As Boolean = ValutaStringaParsata(parser, stringaXml, tempoParser)
            If overtime Then
                Throw New TempoParserException(String.Format("Il Parser Sql ha impiegato {0} millisecondi di tempo per valutare la stringa", tempoParser))
            End If
        Catch ex As TempoParserException
            Throw ex
        Catch ex As Exception
            Throw New OutOfMemoryException(ex.Message)
        End Try

        Dim xmlDoc As New XmlDocument()
        xmlDoc.LoadXml(stringaXml)

        Dim tokens As XmlNodeList = xmlDoc.SelectNodes(".//Tokens")
        If Not IsNothing(tokens) AndAlso tokens.Count > 0 Then
            For Each token As XmlNode In tokens
                Dim lc = token.LastChild
                If Not IsNothing(lc) AndAlso Not IsNothing(lc.Attributes("type")) Then
                    Dim oi As String = lc.Attributes("type").Value.ToLower.Trim
                    If oi = ";" OrElse oi.Contains("comment") Then
                        Throw New UnauthorizedAccessException(errore & "Utilizzo di ; o di commenti sospetti")
                    End If
                End If
            Next
        End If

        Dim fromClauses As XmlNodeList = xmlDoc.SelectNodes(".//SqlFromClause")
        If Not IsNothing(fromClauses) AndAlso fromClauses.Count > 0 Then
            For Each fromClause As XmlNode In fromClauses
                Dim lc = fromClause.LastChild
                If Not IsNothing(lc) AndAlso Not IsNothing(lc.Attributes("ObjectIdentifier")) Then
                    Dim oi As String = lc.Attributes("ObjectIdentifier").Value.ToLower.Trim
                    If oi.StartsWith("sys.") OrElse oi.Contains("sys") Then
                        Throw New UnauthorizedAccessException(errore & "Utilizzo del prefisso sys sospetto")
                    End If
                End If
            Next
        End If

        Dim scalarCount As Integer = xmlDoc.SelectNodes(".//SqlBuiltinScalarFunctionCallExpression").Count
        Dim sibling As XmlNode

        For Each scalar As XmlNode In xmlDoc.SelectNodes(".//SqlBuiltinScalarFunctionCallExpression")

            If ((scalar.Attributes("FunctionName").Value.ToUpper = "UPPER" OrElse
                scalar.Attributes("FunctionName").Value.ToUpper = "LOWER") AndAlso
                scalar.NextSibling Is Nothing) Then
                sibling = scalar.PreviousSibling
            Else
                sibling = scalar.NextSibling
            End If

            Select Case scalar.Attributes("FunctionName").Value.ToUpper

                Case "SUBSTRING" : If (Not String.Compare(scalar.ChildNodes(1).Name, "SqlLiteralExpression") = 0) Then
                        scalarCount -= 1
                        Continue For
                    End If

                Case "UPPER"

                    If Not IsNothing(scalar.ChildNodes(1).Attributes("Value")) AndAlso (sibling.Attributes("Value") Is Nothing OrElse
                        Not (String.Compare(scalar.ChildNodes(1).Attributes("Value").Value.ToUpper, sibling.Attributes("Value").Value))) Then
                        scalarCount -= 1
                        Continue For
                    End If

                    If Not IsNothing(scalar.ChildNodes(1).Attributes("MultipartIdentifier")) AndAlso (sibling.Attributes("Value") Is Nothing OrElse
                    Not (String.Compare(scalar.ChildNodes(1).Attributes("MultipartIdentifier").Value.ToUpper, sibling.Attributes("Value").Value) = 0)) Then
                        scalarCount -= 1
                        Continue For
                    End If

                Case "LOWER"

                    If Not IsNothing(scalar.ChildNodes(1).Attributes("Value")) AndAlso (sibling.Attributes("Value") Is Nothing OrElse
                    Not (String.Compare(scalar.ChildNodes(1).Attributes("Value").Value.ToLower, sibling.Attributes("Value").Value))) Then
                        scalarCount -= 1
                        Continue For
                    End If

                    If Not IsNothing(scalar.ChildNodes(1).Attributes("MultipartIdentifier")) AndAlso (sibling.Attributes("Value") Is Nothing OrElse
                    Not (String.Compare(scalar.ChildNodes(1).Attributes("MultipartIdentifier").Value.ToLower, sibling.Attributes("Value").Value) = 0)) Then
                        scalarCount -= 1
                        Continue For
                    End If

                Case "LEFT" : If (Not String.Compare(scalar.ChildNodes(1).Name, "SqlLiteralExpression") = 0) Then
                        scalarCount -= 1
                        Continue For
                    End If

                Case "RIGHT" : If (Not String.Compare(scalar.ChildNodes(1).Name, "SqlLiteralExpression") = 0) Then
                        scalarCount -= 1
                        Continue For
                    End If


            End Select

            If ([Enum].GetNames(GetType(ScalarFunctionsNoCheck)).Contains(scalar.Attributes("FunctionName").Value.ToUpper)) Then
                scalarCount -= 1
            End If

        Next

        'Rilevo la presenza di "Select" sullo stesso livello di indentazione, causati da errori o tentativi SQL Injection
        'Rilevo la presenza di attacchi del tipo " char(32) = '', NOW() = NOW(), ecc.. "
        If (xmlDoc.SelectNodes(".//SqlSelectStatement").Count > 1 OrElse scalarCount >= 1 OrElse
            xmlDoc.SelectNodes(".//SqlNullStatement").Count >= 1) Then

            Throw New UnauthorizedAccessException(errore & "Individuato utilizzo di Select non innestate o funzioni scalari sospette")

        End If



        Try

            'Se siamo in un order by non è necessario proseguire oltre siccome non è possibile inserire operatori OR 
            'se non all'interno di una subquery (la cui presenza è già stata individuata precedentemente)
            If (isFiltroAggiuntivo) Then

                'Considero tutti gli "Or", "IN", "LIKE" e "BETWEEN" presenti nella query
                Dim orExpressions As XmlNodeList = xmlDoc.SelectNodes(".//SqlBinaryBooleanExpression[@Operator='Or']")
                Dim inExpressions As XmlNodeList = xmlDoc.SelectNodes(".//SqlInBooleanExpression")

                Dim likeNodes As XmlNodeList = xmlDoc.SelectNodes(".//SqlLikeBooleanExpression")

                Dim betweenExpressions As XmlNodeList = xmlDoc.SelectNodes(".//SqlBetweenBooleanExpression")

                ControlloOr(orExpressions, errore)

                'Controllo la presenza di espressioni "LIKE" del tipo 'x' LIKE 'x'
                ControlloLike(likeNodes, errore)

                'Controllo la presenza di "IN" un po' strane tipo 3 IN (1, 2, 3)
                ControlloIn(inExpressions, errore, creaParametri, objParametri)

                'Controllo la presenza di "BETWEEN" veramente ambigue tipo 3 BETWEEN 1 AND 5 o a.id BETWEEN a.id AND a.id
                ControlloBetween(betweenExpressions, errore)


            End If

        Catch ex As UnauthorizedAccessException
            Throw ex
        Catch ex As OutOfMemoryException
            Throw ex
        Catch ex As Exception
            Throw ex.InnerException
        End Try

        Return xmlDoc

    End Function

    Private Sub ControlloOr(ByVal orExpressions As XmlNodeList, ByVal errore As String)
        Dim orLiteralExpression As XmlNodeList
        Dim orComparison As XmlNodeList
        Dim subQs As XmlNodeList
        Dim convertNodes As XmlNodeList
        Dim convertLiteralNodes As XmlNodeList
        Dim literalNodesToBeExcluded As New List(Of XmlNode)
        Dim subQArray As List(Of String)

        Dim refExpressionL As String
        Dim refExpressionR As String

        Dim isLiteralInjection As Boolean = False
        Dim scalarRefCounter As Integer
        Dim columnRefCounter As Integer
        Dim literalCount As Integer
        Dim topCount As Integer
        Dim subQCount As Integer
        Dim literalToBeExcludedCount As Integer
        Dim orComparisonFiltered As New List(Of XmlNode)

        For Each orNodes As XmlNode In orExpressions

            'Considero il termine a sinistra e a destra
            orComparison = orNodes.SelectNodes(".//SqlComparisonBooleanExpression")

            ' Escludiamo dalla lista dei nodi Condizione all'interno di una clausola OR, quelli il cui padre diretto è un nodo AND
            For Each orComparisonNodes As XmlNode In orComparison
                Dim sqbNode As XmlNode = orComparisonNodes.ParentNode

                If Not IsNothing(sqbNode) Then
                    While Not sqbNode.Name.Contains("SqlBinaryBooleanExpression")
                        sqbNode = sqbNode.ParentNode
                    End While
                End If

                If Not IsNothing(sqbNode) Then
                    Dim op = sqbNode.Attributes("Operator").Value.ToLower
                    If op <> "and" Then
                        orComparisonFiltered.Add(orComparisonNodes)
                    End If
                End If

            Next

            For Each orComparisonNodes As XmlNode In orComparisonFiltered

                scalarRefCounter = 0
                columnRefCounter = 0
                'Seleziono tutte le espressioni letterali presenti nei termini considerati
                orLiteralExpression = orComparisonNodes.SelectNodes(".//SqlLiteralExpression")
                convertNodes = orComparisonNodes.SelectNodes(".//SqlConvertExpression")
                literalCount = orLiteralExpression.Count
                topCount = orComparisonNodes.SelectNodes(".//SqlTopSpecification").Count
                subQs = orComparisonNodes.SelectNodes(".//SqlScalarSubQueryExpression")
                subQCount = subQs.Count
                subQArray = New List(Of String)

                For Each subQ As XmlNode In subQs
                    subQArray.Add(subQ.InnerXml)
                Next

                For Each convertNode In convertNodes
                    convertLiteralNodes = convertNode.SelectNodes(".//SqlLiteralExpression")
                    If convertLiteralNodes.Count > 1 Then
                        literalNodesToBeExcluded.Add(convertLiteralNodes.Item(1))
                    End If
                Next

                literalToBeExcludedCount = literalNodesToBeExcluded.Count

                'Individuo la presenza di espressioni referenziali di tipo scalare o relativi ad una colonna
                'Se nessuno dei due è presente, viene impostata una stringa vuota
                Try
                    If (orComparisonNodes.SelectNodes(".//SqlScalarRefExpression").Count = 0) Then

                        If (orComparisonNodes.SelectNodes(".//SqlColumnRefExpression").Item(columnRefCounter) IsNot Nothing) Then
                            refExpressionL = orComparisonNodes.SelectNodes(".//SqlColumnRefExpression").Item(columnRefCounter).Attributes("MultipartIdentifier").Value.ToLower
                            columnRefCounter += 1
                        Else
                            refExpressionL = String.Empty
                        End If

                    Else

                        If (orComparisonNodes.SelectNodes(".//SqlScalarRefExpression").Item(scalarRefCounter) IsNot Nothing) Then
                            refExpressionL = orComparisonNodes.SelectNodes(".//SqlScalarRefExpression").Item(scalarRefCounter).Attributes("MultipartIdentifier").Value.ToLower
                            scalarRefCounter += 1
                        Else
                            refExpressionL = String.Empty
                        End If

                    End If

                    'Uguale per l'altra espressione
                    If (orComparisonNodes.SelectNodes(".//SqlScalarRefExpression").Count <= 1) Then

                        If (orComparisonNodes.SelectNodes(".//SqlColumnRefExpression").Item(columnRefCounter) IsNot Nothing) Then

                            refExpressionR = orComparisonNodes.SelectNodes(".//SqlColumnRefExpression").Item(columnRefCounter).Attributes("MultipartIdentifier").Value.ToLower

                        Else

                            refExpressionR = String.Empty

                        End If

                    Else
                        If (orComparisonNodes.SelectNodes(".//SqlScalarRefExpression").Item(scalarRefCounter) IsNot Nothing) Then

                            refExpressionR = orComparisonNodes.SelectNodes(".//SqlScalarRefExpression").Item(scalarRefCounter).Attributes("MultipartIdentifier").Value.ToLower

                        Else

                            refExpressionR = String.Empty

                        End If

                    End If
                Catch ex As NullReferenceException
                    refExpressionR = String.Empty
                    refExpressionL = String.Empty
                End Try

                'Vengono ignorati tutti i numeri all'interno di una singola espressione algebrica
                If (literalCount > 1) Then
                    Dim parent As XmlNode
                    Dim siblingParent As XmlNode
                    Dim subQ1 As XmlNode
                    Dim nodesFamily As New List(Of Boolean)

                    For fill = 1 To orLiteralExpression.Count
                        nodesFamily.Add(True)
                    Next

                    For counter = 0 To orLiteralExpression.Count - 2

                        If Not literalNodesToBeExcluded.Contains(orLiteralExpression.Item(counter)) Then

                            If (nodesFamily.Item(counter)) Then

                                parent = orLiteralExpression.Item(counter)
                                While (Not (parent.Name.Contains("SqlBinaryScalarExpression") AndAlso Not parent.ParentNode.Name.Contains("SqlBinaryScalarExpression")) AndAlso
                                                Not parent.Name.Contains("SqlComparisonBooleanExpression"))

                                    parent = parent.ParentNode()

                                End While

                                subQ1 = orLiteralExpression.Item(counter)
                                While (Not subQ1.Name.Contains("SqlScalarSubQueryExpression") AndAlso Not subQ1.Name.Contains("SqlBatch"))

                                    subQ1 = subQ1.ParentNode()

                                End While

                                If (Not subQArray.Contains(subQ1.InnerXml())) Then

                                    For innerCounter = counter + 1 To orLiteralExpression.Count - 1

                                        If Not literalNodesToBeExcluded.Contains(orLiteralExpression.Item(innerCounter)) Then

                                            If (nodesFamily.Item(innerCounter)) Then

                                                siblingParent = orLiteralExpression.Item(innerCounter)
                                                While (Not siblingParent.Name.Contains("SqlTopSpecification") AndAlso Not (siblingParent.Name.Contains("SqlBinaryScalarExpression") AndAlso
                                                                Not siblingParent.ParentNode.Name.Contains("SqlBinaryScalarExpression")) AndAlso
                                                                Not siblingParent.Name.Contains("SqlComparisonBooleanExpression") AndAlso Not siblingParent.Name.Contains("SqlInBooleanExpression"))
                                                    siblingParent = siblingParent.ParentNode()
                                                End While

                                                If (parent.Equals(siblingParent) AndAlso siblingParent.Name.Contains("SqlBinaryScalarExpression")) Then
                                                    nodesFamily.Item(innerCounter) = False
                                                    literalCount -= 1

                                                ElseIf (parent.Equals(siblingParent) AndAlso siblingParent.Name.Contains("SqlComparisonBooleanExpression")) Then

                                                    isLiteralInjection = True

                                                    Exit For

                                                End If

                                            End If

                                        End If

                                    Next

                                End If

                            End If

                        End If

                    Next
                End If

                'Se le parti a sinistra e a destra dell' "OR" sono entrambe espressioni terminali (ossia entrambi numeri o stringhe dentro apici)
                'Oppure sono due riferimenti a colonne uguali (a.id_agenda = a.id_agenda)
                'Vengono ignorate le literal dei numeri dentro la clausola TOP e l'eventuale presenza di subquery
                If (((literalCount - topCount - subQCount - literalToBeExcludedCount) = 2 OrElse isLiteralInjection) OrElse (literalCount = 0 AndAlso String.Compare(refExpressionL, refExpressionR) = 0)) Then

                    'E' un probabile tentativo di SQL Injection
                    Throw New UnauthorizedAccessException(errore & "Individuata la presenza di due espressioni terminali di tipo uguale " &
                                                                      "sospette (tipo int=int o 'x'='c' o a.id=a.id)")

                End If

            Next

            If (orExpressions.Count > 30 AndAlso subQCount = 0) Then
                Exit For
            End If

            orComparisonFiltered.Clear()

        Next


    End Sub

    Private Sub ControlloLike(ByVal likeNodes As XmlNodeList, ByVal errore As String)


        Dim likelitCont As String
        Dim scalar As String
        Dim likeLiteralInjection As Boolean

        For Each likeNode As XmlNode In likeNodes
            likeLiteralInjection = False
            For Each likeLiteralNode As XmlNode In likeNode.SelectNodes(".//SqlLiteralExpression")
                If (Not IsNothing(likeLiteralNode.NextSibling) AndAlso likeLiteralNode.NextSibling.Name.Contains("BinaryScalarExpression")) Then
                    Select Case likeLiteralNode.Attributes("Type").Value
                        Case "String"
                            scalar = likeLiteralNode.NextSibling.InnerXml
                            likelitCont = likeLiteralNode.InnerXml
                            scalar = scalar.Remove(scalar.IndexOf(">")).Remove(0, 4)
                            scalar = scalar.Remove(scalar.Length - 2)
                            scalar = scalar.Replace(" + ", "").Replace("' ", " ").Replace(" '", " ").Replace("'", "")
                            likelitCont = likelitCont.Remove(likelitCont.IndexOf(">")).Remove(0, 4)
                            likelitCont = likelitCont.Remove(likelitCont.Length - 2)
                            likelitCont = likelitCont.Replace(" + ", "").Replace("' ", " ").Replace(" '", " ").Replace("'", "")
                            If (String.Compare(likelitCont, scalar) = 0) Then
                                likeLiteralInjection = True
                            End If
                    End Select
                End If

                If (Not IsNothing(likeLiteralNode.NextSibling) AndAlso likeLiteralNode.NextSibling.Name.Contains("SqlLiteralExpression") AndAlso String.Compare(likeLiteralNode.Value, likeLiteralNode.NextSibling.Value) = 0) Then
                    likeLiteralInjection = True
                End If
            Next

            If (likeLiteralInjection OrElse
                    (likeNode.SelectNodes(".//SqlColumnRefExpression").Count + likeNode.SelectNodes(".//SqlScalarRefExpression").Count) > 1) Then

                Throw New UnauthorizedAccessException(errore & "Individuata la presenza di due espressioni terminali di tipo uguale " &
                                                          "accanto alla stessa 'like' (tipo 3 like 4 , 'x' like 'x' o a.id like a.id)")

            End If
        Next


    End Sub

    Private Sub ControlloIn(ByVal inExpressions As XmlNodeList,
                            ByVal errore As String,
                            ByRef creaParametri As Boolean,
                            Optional ByVal objParametri As AgronicaCoreParametri = Nothing)

        Dim inList As XmlNodeList
        Dim inToCompare As XmlNode
        Dim maxValoriIN = If(Not IsNothing(DataProviderFactory.Instance.ConfigurazioneEstesa), DataProviderFactory.Instance.ConfigurazioneEstesa.LimiteElementiClausoleIn, 50)

        For Each inNode As XmlNode In inExpressions

            inToCompare = inNode.ChildNodes.Item(1)     'Il valore da valutare (prima dell' IN)

            If (inNode.SelectNodes(".//SqlInBooleanExpressionCollectionValue").Count > 0) Then
                'Seleziono tutti i nodi che hanno lo stesso nome (literalExpression, ScalarRefExpression ecc.) del valore da valutare
                inList = inNode.SelectNodes(".//SqlInBooleanExpressionCollectionValue").Item(0).SelectNodes(".//" & inToCompare.Name)

                Dim literalNodes As XmlNodeList = inNode.SelectNodes(".//SqlInBooleanExpressionCollectionValue").Item(0).SelectNodes(".//SqlLiteralExpression")

                If (inNode.SelectNodes(".//SqlInBooleanExpressionCollectionValue").Item(0).ChildNodes.Count > maxValoriIN OrElse literalNodes.Count = inNode.SelectNodes(".//SqlInBooleanExpressionCollectionValue").Item(0).ChildNodes.Count - 1) AndAlso
                    (inToCompare.Name <> "SqlLiteralExpression") Then
                    creaParametri = False
                    Continue For
                End If

                'Dim max = DataProviderFactory.Instance.ConfigurazioneEstesa(NonSerializedAttribute Nothing)
                'Controllo per ogni possibile tipo se il valore da valutare è uguale ad uno già presente nella lista dentro IN
                Select Case (inToCompare.Name)

                    Case "SqlLiteralExpression"

                        For Each inLiteralExpression As XmlNode In inList

                            If (String.Compare(inLiteralExpression.Attributes("Value").Value, inToCompare.Attributes("Value").Value) = 0) Then

                                Throw New UnauthorizedAccessException(errore & "Individuata la presenza di due espressioni terminali uguali " &
                                                              "in una IN (tipo 3 IN (1, 2, 3) o a.id IN (a.id, b.id, c.id))")

                            End If

                        Next

                    Case "SqlBinaryScalarExpression"

                        If (inToCompare.SelectNodes(".//SqlColumnRefExpression").Count = 0 AndAlso
                                    inToCompare.SelectNodes(".//SqlScalarRefExpression").Count = 0) Then

                            Throw New UnauthorizedAccessException(errore & "Individuata la presenza di un'espressione algebrica nell'espressione da valutare " &
                                                              "in una IN (tipo 3 IN (1, 2, 3) o a.id IN (a.id, b.id, c.id))")

                        End If

                    Case "SqlColumnRefExpression", "SqlScalarRefExpression"

                        For Each inColumnRefExpression As XmlNode In inList

                            If (String.Compare(inColumnRefExpression.Attributes("MultipartIdentifier").Value.ToLower,
                                                   inToCompare.Attributes("MultipartIdentifier").Value.ToLower) = 0) Then

                                Throw New UnauthorizedAccessException(errore & "Individuata la presenza di due espressioni terminali uguali " &
                                                              "in una IN (tipo 3 IN (1, 2, 3) o a.id IN (a.id, b.id, c.id))")

                            End If

                        Next

                End Select

            Else
                Dim inWhere As XmlNodeList
                inList = inNode.SelectNodes(".//SqlInBooleanExpressionQueryValue")

                For Each inSubQuery As XmlNode In inList
                    inWhere = inSubQuery.SelectNodes(".//SqlWhereClause")

                    For Each where As XmlNode In inWhere

                        ControllaInjection(where.ChildNodes.Item(1).ChildNodes.Item(0).InnerText.ToString, True, creaParametri, objParametri)

                    Next

                Next

            End If
        Next


    End Sub

    Private Sub ControlloBetween(ByVal betweenExpressions As XmlNodeList, ByVal errore As String)
        Dim betweenCompare As XmlNode
        Dim betweenStart As XmlNode
        Dim betweenEnd As XmlNode

        For Each betweenNode As XmlNode In betweenExpressions

            betweenCompare = betweenNode.ChildNodes.Item(1)     'il valore da valutare (prima del between)
            betweenStart = betweenNode.ChildNodes.Item(2)       'il valore di inizio intervallo
            betweenEnd = betweenNode.ChildNodes.Item(3)         'il valore di fine intervallo

            'Se il valore da valutare è un numero o una stringa, la cosa è alquanto sospetta
            If (betweenCompare.Name.Contains("SqlLiteralExpression") OrElse betweenCompare.Name.Contains("ScalarExpression")) Then

                Throw New UnauthorizedAccessException(errore & "Individuata la presenza di incongruenze sospette nella clausola " &
                                                          "BETWEEN (tipo 3 BETWEEN 1 AND 5 oppure a.id BETWEEN a.id AND a.id)")

            End If

            'altrimenti, se tutte e tre le espressioni hanno come corpo lo stesso valore, la cosa è ugualmente sospetta
            If (betweenCompare.Name.Contains("RefExpression") AndAlso betweenStart.Name.Contains("RefExpression") AndAlso
                    betweenEnd.Name.Contains("RefExpression") AndAlso (String.Compare(betweenCompare.Attributes("MultipartIdentifier").Value.ToLower,
                    betweenStart.Attributes("MultipartIdentifier").Value.ToLower) = 0 AndAlso
                    String.Compare(betweenEnd.Attributes("MultipartIdentifier").Value.ToLower, betweenCompare.Attributes("MultipartIdentifier").Value.ToLower) = 0)) Then

                Throw New UnauthorizedAccessException(errore & "Individuata la presenza di incongruenze sospette nella clausola " &
                                                          "BETWEEN (tipo 3 BETWEEN 1 AND 5 oppure a.id BETWEEN a.id AND a.id)")

            End If

        Next

    End Sub


#End Region

    Protected Sub AggiungiParametro(ByVal valore As Object, ByVal tipo As Type, Optional ByVal injectionGuid As Guid = Nothing)

        If IsNothing(injectionGuid) OrElse injectionGuid = Guid.Empty Then
            If _parametri IsNot Nothing Then

                Dim parametriEsistenti = _parametri.Values.ToList()
                Dim chiave As Integer = _parametri.Count() + 1
                Dim p = New AgroDBParametro With
                               {
                                    .Valore = valore,
                                    .Tipo = tipo,
                                    .ChiaveRiferimento = Nothing,
                                    .IndiceDizionario = chiave
                               }
                Dim esistente = parametriEsistenti.FirstOrDefault(Function(pe) pe.Tipo = p.Tipo AndAlso pe.Valore = p.Valore)
                If esistente IsNot Nothing Then
                    p.ChiaveRiferimento = esistente.IndiceDizionario
                End If

                _parametri.Add(chiave, p)
            End If
        Else
            Dim paramInjected = DataProviderFactory.Instance.ParametriInjected
            If paramInjected IsNot Nothing Then
                Dim params As Dictionary(Of Integer, AgroDBParametro) = Nothing

                If paramInjected.ContainsKey(injectionGuid.ToString) Then

                    ' sono già stati accodati parametri per il metodo col guid indicato
                    If paramInjected.TryGetValue(injectionGuid.ToString(), params) Then

                        Dim parametriEsistenti = params.Values.ToList()
                        Dim chiave As Integer = params.Count() + 1
                        Dim p = New AgroDBParametro With
                               {
                                    .Valore = valore,
                                    .Tipo = tipo,
                                    .ChiaveRiferimento = Nothing,
                                    .IndiceDizionario = chiave
                               }
                        Dim esistente = parametriEsistenti.FirstOrDefault(Function(pe) pe.Tipo = p.Tipo AndAlso pe.Valore = p.Valore)
                        If esistente IsNot Nothing Then
                            p.ChiaveRiferimento = esistente.IndiceDizionario
                        End If

                        params.Add(chiave, p)

                    End If
                Else

                    params = New Dictionary(Of Int32, AgroDBParametro)
                    Dim chiave As Integer = 1
                    Dim p = New AgroDBParametro With
                               {
                                    .Valore = valore,
                                    .Tipo = tipo,
                                    .ChiaveRiferimento = Nothing,
                                    .IndiceDizionario = chiave
                               }
                    params.Add(chiave, p)
                    ' primo parametro del metodo con guid indicato
                    paramInjected.Add(injectionGuid.ToString(), params)

                End If

            End If
        End If

    End Sub

    Private Function SeparatoreDecimale() As String

        Dim StrSeparatoreDecimale As String = ""
        Dim xNumberFormatInfo As NumberFormatInfo

        xNumberFormatInfo = CultureInfo.CurrentCulture.NumberFormat
        StrSeparatoreDecimale = xNumberFormatInfo.NumberDecimalSeparator()

        Return StrSeparatoreDecimale

    End Function

#Region "Logging"

    Overloads Sub Logga(ByVal NomeRoutine As String,
                        ByVal MessaggioErrore As String,
                            Optional ByVal objParametri As AgronicaCoreParametri = Nothing,
                            Optional ByVal verificaInviaElasticSearch As Boolean = False) Implements IParametrizzatore.Logga

        MyBase.Logga(NomeRoutine, MessaggioErrore, objParametri, verificaInviaElasticSearch:=verificaInviaElasticSearch)

    End Sub

#End Region

End Class
