Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json.Linq

Public Class UtentiFiltriUtility

    ''' <summary>
    ''' Aggiunge il nome della tabella su cui controllare il campo durante la lettura degli utenti
    ''' </summary>
    ''' <param name="f">JToken del filtro utente</param>
    ''' <returns>Il campo da leggere preceduto dal nome detta tabella da cui è preso</returns>
    Private Function GetFilterFieldExtender(f As JToken) As String
        Dim field As String = CStr(f("field"))
        Select Case CStr(f("field"))
            Case "Username", "Nome", "Cognome", "UsernameCommerciale", "Data_Modifica"
                Return "Utenti_Dettagli." & field
            Case "Tipologia_Des"
                Return "Utenti_Tipologie." & field
            Case "GruppiDes"
                Return "Utenti_Gruppi." & field
            Case "UltimoAccesso"
                Return "AWS_log_cte." & field
            Case Else
                Return field
        End Select
    End Function

    ''' <summary>
    ''' Costruisce la clausola di paragone per applicare il filtro nella query.
    ''' </summary>
    Private Function GetStringFilterValue(f As JToken) As String
        Dim contains = 0, startsWith = 1, endsWith = 2
        Dim value As String = CStr(f("value"))
        Select Case CInt(f("filterType"))
            Case contains
                Return " like '%" & value & "%'"
            Case startsWith
                Return " like '" & value & "%'"
            Case endsWith
                Return " like '%" & value & "'"
            Case Else
                Return " like '" & value & "'"
        End Select
    End Function

    ''' <summary>
    ''' Costruisce la clausola di paragone per applicare il filtro nella query.
    ''' </summary>
    Private Function GetDateFilterValue(f As JToken, extendedField As String) As String
        Dim dateFilter = ""
        Dim parseDateOr = Function(v As String, defaultValue As Date) As String
                              Dim dateValue As Date
                              Try
                                  dateValue = CDate(f(v)).ToLocalTime
                              Catch ex As Exception
                                  dateValue = defaultValue
                              End Try
                              Return DateValue.ToString("s", Globalization.CultureInfo.InvariantCulture)
                          End Function
        If f("value") IsNot Nothing Then
            dateFilter += extendedField & " >= '" & parseDateOr("value", CostantiPersonalizzate.AGRODATAINIZIO) & "'"
        End If
        If f("valueAlt") IsNot Nothing Then
            dateFilter += If(String.IsNullOrEmpty(dateFilter), "", " AND ")
            dateFilter += extendedField & " <= '" & parseDateOr("valueAlt", CostantiPersonalizzate.AGRODATAFINE) & "'"
        End If
        Return dateFilter
    End Function

    ''' <summary>
    ''' Estrapola la strina da usare come filtro durante la lettura degli utrenti.
    ''' </summary>
    Public Function GetFilterStrFromJArray(Jfilters As JArray) As String
        Dim excludedFields As String() = {"firstTime", "Data_Creazione"}

        Dim toParse = Jfilters.Where(Function(f) Not String.IsNullOrWhiteSpace(f("value"))).
                Where(Function(f) Not excludedFields.Contains(CStr(f("field"))))

        Dim usersFilter = toParse.Where(Function(f) CInt(f("controlType")) = enum_TipoControllo.CASELLA_TESTO).
                Select(Function(f As JToken) " " & GetFilterFieldExtender(f) & GetStringFilterValue(f) & " ").
                DefaultIfEmpty("").
                Aggregate(Function(acc, str) acc & " AND " & str)

        Dim dateFilters = toParse.Where(Function(f) CInt(f("controlType")) = enum_TipoControllo.CALENDARIO).
                Select(Function(f As JToken) " " & GetDateFilterValue(f, GetFilterFieldExtender(f)) & " ").
                DefaultIfEmpty("").
                Aggregate(Function(acc, str) acc & " AND " & str)

        Dim jStartDate = Jfilters.FirstOrDefault(Function(f) f("field") = "Data_Creazione" AndAlso Not String.IsNullOrWhiteSpace(f("value")))
        If jStartDate IsNot Nothing Then
            Dim createdAfter = CDate(jStartDate("value")).ToLocalTime
            createdAfter = createdAfter.AddHours(-1)
            usersFilter &= If(String.IsNullOrEmpty(usersFilter), "", " OR ")
            usersFilter &= " Utenti_Dettagli.Data_Creazione > '" & createdAfter.ToString("s", Globalization.CultureInfo.InvariantCulture) & "' "
        End If

        If String.IsNullOrWhiteSpace(usersFilter) Then
            Return dateFilters
        Else
            Return usersFilter & If(String.IsNullOrEmpty(dateFilters), "", " AND " & dateFilters)
        End If
    End Function

End Class
