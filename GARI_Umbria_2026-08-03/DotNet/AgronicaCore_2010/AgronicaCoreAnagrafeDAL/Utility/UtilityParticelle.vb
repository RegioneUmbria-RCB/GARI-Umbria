Module UtilityParticelle
    ''' <summary>
    ''' Returns an array of DataRows from a given <paramref name="dt"/>
    ''' </summary>
    ''' <param name="dt"></param>
    ''' <param name="prov"></param>
    ''' <param name="com"></param>
    ''' <param name="sezione"></param>
    ''' <param name="foglio"></param>
    ''' <param name="numero"></param>
    ''' <param name="subalterno"></param>
    ''' <param name="filter"></param>
    ''' <returns></returns>
    Public Function FilterParcelRows(
                                    ByVal dt As DataTable,
                                    ByVal prov As String,
                                    ByVal com As String,
                                    ByVal sezione As String,
                                    ByVal foglio As Integer,
                                    ByVal numero As Integer,
                                    ByVal subalterno As String,
                                    ByVal filter As String
                                    ) As DataRow()
        Return dt.Select(
            "Prov='" & prov.ToString & "' " &
            "AND Com='" & com.ToString & "' " &
            "AND sezione='" & sezione.ToString & "' " &
            "AND foglio=" & foglio & " " &
            "AND numero=" & numero & " " &
            "AND subalterno='" & subalterno.ToString & "' " &
            filter
            )
    End Function

    ''' <summary>
    ''' Return an array containings all distinct dates of <paramref name="endDateFieldName"/> and <paramref name="startDateFieldName"/>
    ''' </summary>
    ''' <param name="arrayDate"></param>
    ''' <param name="drs"></param>
    ''' <param name="startDate"></param>
    ''' <param name="endDate"></param>
    ''' <param name="startDateFieldName"></param>
    ''' <param name="endDateFieldName"></param>
    Public Function ExtractDateArray(
                                    ByVal drs As DataRow(),
                                    ByVal startDate As Date,
                                    ByVal endDate As Date,
                                    ByVal startDateFieldName As String,
                                    ByVal endDateFieldName As String
                                    ) As Date()

        Dim arrayDate As Date() = {}

        Dim Data1Presente As Boolean
        Dim Data2Presente As Boolean

        Dim N_Date = 0

        If Not drs Is Nothing Then

            For j = 0 To drs.Length - 1

                Data1Presente = arrayDate.Contains(drs(j).Item(startDateFieldName))
                If Not Data1Presente Then
                    ReDim Preserve arrayDate(N_Date)
                    arrayDate(N_Date) = drs(j).Item(startDateFieldName)
                    N_Date += 1
                End If

                Data2Presente = arrayDate.Contains(drs(j).Item(endDateFieldName))
                If Not Data2Presente Then
                    ReDim Preserve arrayDate(N_Date)
                    arrayDate(N_Date) = drs(j).Item(endDateFieldName)
                    N_Date += 1
                End If

            Next

            '----------------------------------------------------------------
            'aggiungo al vettore la data inizio e fine 
            'dell'appezzamento che sto creando

            Data1Presente = arrayDate.Contains(startDate)
            If Not Data1Presente Then
                ReDim Preserve arrayDate(N_Date)
                arrayDate(N_Date) = startDate
                N_Date += 1
            End If

            Data2Presente = arrayDate.Contains(endDate)
            If Not Data2Presente Then
                ReDim Preserve arrayDate(N_Date)
                arrayDate(N_Date) = endDate
                N_Date += 1
            End If

            'ordino le date..
            If arrayDate IsNot Nothing Then
                Array.Sort(arrayDate)
            End If

        End If

        Return arrayDate

    End Function

    Public Function CalculateMaxUsedSurface(
                                           ByVal arrayDate As Date(),
                                           ByVal dataInizio As Date,
                                           ByVal dataFine As Date,
                                           ByVal drs As DataRow(),
                                           ByVal surfaceFieldName As String,
                                           ByVal validityStartName As String,
                                           ByVal validityEndName As String
                                           ) As Decimal
        Dim data As Date
        Dim usedSurface As Decimal = 0
        Dim maxUsedSurface As Decimal = 0

        For j = 0 To UBound(arrayDate)

            data = arrayDate(j)

            If data >= dataInizio And data <= dataFine Then
                usedSurface = 0
                usedSurface = AgronicaCoreDataProvider.Conversioni.CustomDecimalFieldAtDate(data, drs, surfaceFieldName, validityStartName, validityEndName)

                If usedSurface > maxUsedSurface Then
                    maxUsedSurface = usedSurface
                End If
            End If

        Next

        Return maxUsedSurface
    End Function

    ''' <summary>
    ''' This sub sets the correct tables depending on if it is Budget case or not
    ''' </summary>
    ''' <param name="stbQuery"></param>
    ''' <param name="isBudget"></param>
    ''' <param name="campiTableName"></param>
    ''' <param name="campiXParticelleTableName"></param>
    ''' <param name="appezzamentoTableName"></param>
    ''' <param name="appezzamentiXParticelleTableName"></param>
    Public Sub GetQueryTablesAlias(
                                  ByRef stbQuery As System.Text.StringBuilder,
                                  ByVal isBudget As Boolean,
                                  ByVal campiTableName As String,
                                  ByVal campiXParticelleTableName As String,
                                  ByVal appezzamentoTableName As String,
                                  ByVal appezzamentiXParticelleTableName As String
                                  )

        Dim TableName As String = String.Empty

        stbQuery.AppendLine(String.Format("WITH {0} AS (", campiTableName))
        stbQuery.AppendLine("    SELECT *")

        If Not isBudget Then
            TableName = "Campi"
        Else
            TableName = "Budget_Campi"
        End If

        stbQuery.AppendLine(String.Format("    FROM {0}", TableName))
        stbQuery.AppendLine(")")
        stbQuery.AppendLine("")

        stbQuery.AppendLine(String.Format(", {0} AS (", campiXParticelleTableName))
        stbQuery.AppendLine("    SELECT *")

        If Not isBudget Then
            TableName = "CampiXParticelle"
        Else
            TableName = "Budget_CampiXParticelle"
        End If

        stbQuery.AppendLine(String.Format("    FROM {0}", TableName))
        stbQuery.AppendLine(")")
        stbQuery.AppendLine("")

        '-------------------------------------------------------------------------
        stbQuery.AppendLine(String.Format(", {0} AS (", appezzamentoTableName))
        stbQuery.AppendLine("    SELECT *")

        If Not isBudget Then
            TableName = "Appezzamento"
        Else
            TableName = "Budget_Appezzamento"
        End If

        stbQuery.AppendLine(String.Format("    FROM {0}", TableName))
        stbQuery.AppendLine(")")
        stbQuery.AppendLine("")

        stbQuery.AppendLine(String.Format(", {0} AS (", appezzamentiXParticelleTableName))
        stbQuery.AppendLine("    SELECT *")

        If Not isBudget Then
            TableName = "AppezzamentiXParticelle"
        Else
            TableName = "Budget_AppezzamentiXParticelle"
        End If

        stbQuery.AppendLine(String.Format("    FROM {0}", TableName))
        stbQuery.AppendLine(")")
        stbQuery.AppendLine("")

    End Sub
End Module
