
Imports System
Imports System.Globalization
Imports System.Data
Imports System.Xml
Imports System.Web.UI
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.Web.UI.HtmlControls
Imports System.Text
Imports System.Reflection
Imports System.Text.RegularExpressions


Public Class UtilityProvider


    Public Shared Function ValoreToString(ByVal val As Integer?) As String

        If val Is Nothing Then
            Return ""
        Else
            Return val.ToString
        End If

    End Function

    Public Shared Function ValoreToString(ByVal val As Double?) As String

        If val Is Nothing Then
            Return ""
        Else
            Return val.ToString
        End If

    End Function

    Public Shared Function ValoreToString(ByVal val As String) As String

        If String.IsNullOrEmpty(val) Then
            Return ""
        Else
            Return val
        End If

    End Function

    Public Shared Function ValoreToString(ByVal val As DateTime?, Optional ByVal dateFormat As dateFormat = 0) As String
        If val Is Nothing Then
            Return ""
        Else
            Select Case dateFormat
                Case UtilityProvider.dateFormat.onlyDate
                    Return val.Value.Year.ToString("0000") & "-" & val.Value.Month.ToString("00") & "-" & val.Value.Day.ToString("00")
                Case UtilityProvider.dateFormat.totalDatetime
                    Return val.Value.Year.ToString("0000") & "-" & val.Value.Month.ToString("00") & "-" & val.Value.Day.ToString("00") & " " & val.Value.Hour.ToString("00") & ":" & val.Value.Minute.ToString("00") & ":" & val.Value.Second.ToString("00")
            End Select
            Return ""
        End If
    End Function

    Enum dateFormat
        onlyDate = 0
        totalDatetime = 1
    End Enum

    '####################################################################################################################################
    Public Shared Function StringaUnivoca() As String

        Dim myGuid As String = System.Guid.NewGuid.ToString
        Return myGuid

    End Function


    Public Shared Function meseCorrente_DataprimoGiorno(Optional ByVal mData As DateTime = AGRODATAINIZIO) As DateTime
        If mData = AGRODATAINIZIO Then
            mData = Now
        End If
        Return mData.AddMonths(-1).AddDays(-(mData.Day - 1))
    End Function

    Public Shared Function meseCorrente_DataultimoGiorno(Optional ByVal mData As DateTime = AGRODATAINIZIO) As DateTime
        If mData = AGRODATAINIZIO Then
            mData = Now
        End If
        Return New DateTime(mData.Year, mData.AddMonths(-1).Month, 1).AddMonths(1).AddDays(-1)
    End Function

    '####################################################################################################################################
    Public Shared Function LevenshteinDistance(ByVal s As String,
                              ByVal t As String) As Integer
        Dim n As Integer = s.Length
        Dim m As Integer = t.Length
        Dim d(n + 1, m + 1) As Integer

        If n = 0 Then
            Return m
        End If

        If m = 0 Then
            Return n
        End If

        Dim i As Integer
        Dim j As Integer

        For i = 0 To n
            d(i, 0) = i
        Next

        For j = 0 To m
            d(0, j) = j
        Next

        For i = 1 To n
            For j = 1 To m

                Dim cost As Integer
                If t.Chars(j - 1) = s.Chars(i - 1) Then
                    cost = 0
                Else
                    cost = 1
                End If

                d(i, j) = Math.Min(Math.Min(d(i - 1, j) + 1, d(i, j - 1) + 1),
                               d(i - 1, j - 1) + cost)
            Next
        Next

        Return d(n, m)


    End Function



    '##############################################################################################

    Public Shared Function Agro_SQL_Load(ByVal Campo As Object) As Object

        If IsDBNull(Campo) Then
            Campo = ""
        End If

        If IsNothing(Campo) Then
            Campo = ""
        End If

        Return Campo

    End Function

    ''' <summary>
    ''' Se l'oggetto è null ritorna esattamente la stringa NULL
    ''' </summary>
    Public Shared Function Agro_SQL_Load_NULL(ByVal Campo As Object) As Object

        If IsDBNull(Campo) Then
            Campo = "NULL"
        End If

        If IsNothing(Campo) Then
            Campo = "NULL"
        End If

        Return Campo

    End Function

    ''' <summary>
    ''' Se l'oggetto è null ritorna il default in base al tipo di dato
    ''' </summary>
    Public Shared Function Agro_SQL_Load_ConDefault(ByVal Campo As Object, ByVal tipoDato As Type) As Object
        If IsDBNull(Campo) OrElse IsNothing(Campo) Then
            Select Case tipoDato.Name
                Case "String"
                    Return ""
                Case "Integer", "Int32", "Short", "Int16", "Long", "Int64", "Decimal"
                    Return 0
                Case "DateTime", "Date"
                    Return AGRODATAINIZIO
                Case Else
                    Dim a = 0
            End Select
        End If

        Return Campo

    End Function

    '##############################################################################################
    Public Shared Function Agro_SQL_SaveDate2(ByVal DataItaliana As Date) As String
        If DataItaliana = New Date Then
            Return "Null"
        End If
        Dim Testo As String = ""
        Testo = " CONVERT(Date,'" & Format(DataItaliana, "yyyy/MM/dd") & "',120) "
        Return Testo

    End Function

    ''' <summary>
    ''' Genera uno Statement SQL con la chiamata alla funzione di conversione per generare un testo in formato ISO 8601 per il tipo data/ora
    ''' </summary>
    ''' <param name="ColonnaData">Nome della Colonna DateTime o Date per la tabella</param>
    ''' <returns></returns>
    Public Shared Function Agro_SQL_QRY_ConvertiColonnaDataToStringaISO8601(ByVal ColonnaData As String) As String
        Return "convert(varchar(100), " & ColonnaData & " , 126) + 'Z'"
    End Function

    '''' <summary>
    '''' Genera uno Statement SQL precedenti a versione 2016 con la chiamata alla funzione di conversione di una data in UTC
    '''' </summary>
    '''' <param name="ColonnaData">Nome della Colonna DateTime o Date per la tabella</param>
    '''' <returns></returns>
    '''' <remarks>NON usare, non tiene conto dell'ora legale e non si possono passare info sulla time zone</remarks>    
    'Public Shared Function Agro_SQL_QRY_ConvertiColonnaDataToUTC(ByVal ColonnaData As String) As String
    '    Return " DATEADD(hh, DATEDIFF(hh, GETDATE(), GETUTCDATE()), " & ColonnaData & "  ) "
    'End Function

    ''' <summary>
    ''' Genera uno Statement SQL 2016 o successivi con la chiamata alla funzione di conversione di una data in UTC
    ''' </summary>
    ''' <param name="ColonnaData">Nome della Colonna DateTime o Date per la tabella</param>
    ''' <param name="sql_time_zone_info">Sql timeZone info come da vista in sql 2016 o superiori: "select * from sys.time_zone_info" (es.:"Central Europe Standard Time") </param>
    ''' <returns></returns>
    Public Shared Function Agro_SQL_QRY_ConvertiColonnaDataToUTC_SQL2016(ByVal ColonnaData As String, ByVal Colonna_sql_time_zone_info As String, Optional ByVal default_sql_time_zone_info_ifNULL As String = "Central Europe Standard Time") As String
        Return " DATEADD(hh,  DATEDIFF(hh,  " & ColonnaData & " AT TIME ZONE 'UTC', dataOraRilievo AT TIME ZONE ISNULL(z.sql_system_time_zone, '" & default_sql_time_zone_info_ifNULL & "') ), DataOraRilievo ) "
    End Function

    ''' <summary>
    ''' Genera uno Statement SQL con la chiamata alla funzione di conversione di una data in UTC
    ''' </summary>
    ''' <param name="ColonnaTimeStamp">Nome della Colonna con integer o string che rappresenta il dato Unix Time Stamp per la tabella</param>
    ''' <returns></returns>
    Public Shared Function Agro_SQL_QRY_ConvertiColonnaUnixTimeStampToStringaISO8601(
        ByVal ColonnaTimeStamp As String,
        Optional ByVal TroncamentoData As enum_Sql_TipoTroncamentoData = enum_Sql_TipoTroncamentoData.Nessuno
    ) As String


        Select Case TroncamentoData
            Case enum_Sql_TipoTroncamentoData.Nessuno
                Return " CONVERT(VARCHAR(100),  dateadd(S, CAST( " & ColonnaTimeStamp & " as int) , '1970-01-01'), 127) + 'Z' "
            Case enum_Sql_TipoTroncamentoData.Ora
                Return " substring( CONVERT(VARCHAR(33),  dateadd(S, CAST( " & ColonnaTimeStamp & " as int) , '1970-01-01'), 126) , 1, 13) + ':00:00Z' "
            Case enum_Sql_TipoTroncamentoData.SoloParteData
                Return " substring( CONVERT(VARCHAR(33),  dateadd(S, CAST( " & ColonnaTimeStamp & " as int) , '1970-01-01'), 126) , 1, 11) + '00:00:00Z' "
        End Select

    End Function
    '##############################################################################################
    'in realtà fa cast a datetime
    Public Shared Function Agro_SQL_SaveDate(ByVal DataItaliana As Date) As String

        Dim identificatoreMetodo As Guid = Guid.Empty
        If ControllaPerInjection(identificatoreMetodo) Then
            Return DataProviderFactory.Instance.Agro_SQL_SaveDate(DataItaliana, False, identificatoreMetodo)
        End If

        If DataItaliana = New Date Then
            Return "Null"
        End If
        Dim Testo As String = ""
        Testo = " CONVERT(DateTime,'" & Format(DataItaliana, "yyyy/MM/dd") & "',120) "
        Return Testo

    End Function



    '##############################################################################################
    Public Shared Function Agro_SQL_SaveDateTime(ByVal DataOraItaliana As DateTime) As String

        Dim identificatoreMetodo As Guid = Guid.Empty
        If ControllaPerInjection(identificatoreMetodo) Then
            Return DataProviderFactory.Instance.Agro_SQL_SaveDateTime(DataOraItaliana, False, identificatoreMetodo)
        End If

        Dim Testo As String = ""
        'Testo = " CONVERT(DateTime,'" & Format(DataOraItaliana, "yyyy/MM/dd") & " " & Replace(Format(DataOraItaliana, "HH:mm:ss"), ".", ":") & "',120) "
        Testo = " CONVERT(DateTime,'" & Format(DataOraItaliana, "yyyy/MM/dd") & " " & Replace(Format(DataOraItaliana, "HH:mm:ss.fff"), ".", ":") & "',120) "
        Return Testo

    End Function

    '##############################################################################################
    Public Shared Function Agro_SQL_SaveDateTime(ByVal DataItaliana As DateTime, ByVal Ora As String) As String

        Dim Testo As String = ""
        Testo = " CONVERT(DateTime,'" & Format(DataItaliana, "yyyy/MM/dd") & " " & Replace(Ora, ".", ":") & "',120) "
        Return Testo

    End Function

    '##############################################################################################
    Public Shared Function Agro_SQL_SaveDateTime_NULL(ByVal item As Object) As String

        Dim identificatoreMetodo As Guid = Guid.Empty
        If ControllaPerInjection(identificatoreMetodo) Then
            Return DataProviderFactory.Instance.Agro_SQL_SaveDateTime_NULL(item, False, identificatoreMetodo)
        End If

        If IsDBNull(item) Then
            Return "NULL"
        End If

        Dim DataOraItaliana As DateTime = CDate(item)
        If DataOraItaliana = New Date Then
            Return "NULL"
        End If
        Dim Testo As String = ""
        Testo = " CONVERT(DateTime,'" & Format(DataOraItaliana, "yyyy/MM/dd") & " " & Replace(Format(DataOraItaliana, "HH:mm:ss.fff"), ".", ":") & "',120) "
        Return Testo

    End Function

    '##############################################################################################
    Public Shared Function Agro_SQL_SaveText_NULL(ByVal item As Object) As String

        Dim identificatoreMetodo As Guid = Guid.Empty
        If ControllaPerInjection(identificatoreMetodo) Then
            Return DataProviderFactory.Instance.Agro_SQL_SaveText_NULL(item, False, identificatoreMetodo)
        End If

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

    Public Shared Function Agro_SQL_SaveNum_NULL(ByVal item As Object) As String

        If IsDBNull(item) Then
            Return "NULL"
        End If

        Dim StringaNumero As String = CStr(CDbl(item))

        Dim StrSeparatoreDecimale As String = ""

        'Recupero il Separatore Decimale di Sistema

        StrSeparatoreDecimale = SeparatoreDecimale()


        'Verifico il separatore decimale desiderato
        If (StrSeparatoreDecimale <> ",") AndAlso (StrSeparatoreDecimale <> ".") Then
            Return "0"
            Throw New Exception("Il separatore decimale indicato non e' supportato.")
            Exit Function
        End If

        'Se StringaNumero e' nullo, restituisco uno zero (0)
        If IsNothing(StringaNumero) Then
            Return "0"
            Throw New Exception("Il valore numerico non e' corretto.")
            Exit Function
        End If

        'Se StringaNumero e' una stringa vuota, restituisco lo zero (0)
        If StringaNumero = "" Then
            Return "0"
            Throw New Exception("Il valore numerico non e' corretto.")
            Exit Function
        End If

        'Se StringaNumero non e' numerico, restituisco lo zero (0)
        If Not IsNumeric(StringaNumero.ToString) Then
            Return "0"
            Throw New Exception("Il valore numerico non e' corretto.")
            Exit Function
        End If

        'Verifico in quale situazione mi trovo
        If StrSeparatoreDecimale = "," Then

            '===== Caso ITALIANO =====

            'Elimino i punti
            StringaNumero = Replace(StringaNumero, ".", "")

            'Sostituisco la virgola col punto
            StringaNumero = Replace(StringaNumero, ",", ".")

            'Restituisco in uscita il risultato
            Return StringaNumero

        Else

            '===== Caso ANGLOSASSONE =====

            'Elimino le virgole
            StringaNumero = Replace(StringaNumero, ",", "")

            'Restituisco in uscita il risultato
            Return StringaNumero

        End If

    End Function

    Public Shared Function Agro_SQL_Save_Clausola_IN(ByVal clausolaIN As String, ByVal valoriStringa As Boolean) As String

        Dim identificatoreMetodo As Guid = Guid.Empty
        If ControllaPerInjection(identificatoreMetodo) Then
            Return DataProviderFactory.Instance.Agro_SQL_Save_Clausola_IN(clausolaIN, valoriStringa, False, identificatoreMetodo)
        End If

        Return clausolaIN

    End Function

    Public Shared Function Agro_SQL_Save_xFiltroAggiuntivo(ByVal filtro As String, Optional ByVal objParametri As AgronicaCoreParametri = Nothing) As String

        Dim identificatoreMetodo As Guid = Guid.Empty
        If ControllaPerInjection(identificatoreMetodo) Then
            Return DataProviderFactory.Instance.Agro_SQL_Save_xFiltroAggiuntivo(filtro, False, identificatoreMetodo, objParametri)
        End If

        Return filtro

    End Function

    Public Shared Function Agro_SQL_Save_xOrderBy(ByVal filtro As String) As String

        Dim identificatoreMetodo As Guid = Guid.Empty
        If ControllaPerInjection(identificatoreMetodo) Then
            Return DataProviderFactory.Instance.Agro_SQL_Save_xOrderBy(filtro)
        End If

        Return filtro

    End Function

    '##############################################################################################
    Public Shared Function Agro_SQL_SaveText(ByVal Testo As String) As String

        Dim identificatoreMetodo As Guid = Guid.Empty
        If ControllaPerInjection(identificatoreMetodo) Then
            Return DataProviderFactory.Instance.Agro_SQL_SaveText(Testo, False, identificatoreMetodo)
        End If

        'Se la stringa e' nulla, restituisco la stringa nulla
        If IsNothing(Testo) Then
            Testo = ""
            Return Testo
            Exit Function
        End If

        'Sostituisco i singoli apici con due singoli apici
        Testo = Testo.Replace("'", "''")

        'Restituisco il risultato
        Return Testo

    End Function

    Public Shared Function Agro_vb_SaveNum(ByVal StringaNumero As String) As String

        Dim StrSeparatoreDecimale As String = ""

        'Recupero il Separatore Decimale di Sistema
        StrSeparatoreDecimale = SeparatoreDecimale()

        'Verifico il separatore decimale desiderato
        If (StrSeparatoreDecimale <> ",") AndAlso (StrSeparatoreDecimale <> ".") Then
            Return "0"
            Throw New Exception("Il separatore decimale indicato non e' supportato.")
            Exit Function
        End If

        'Se StringaNumero e' nullo, restituisco uno zero (0)
        If IsNothing(StringaNumero) Then
            Return "0"
            Throw New Exception("Il valore numerico non e' corretto.")
            Exit Function
        End If

        'Se StringaNumero e' una stringa vuota, restituisco lo zero (0)
        If StringaNumero = "" Then
            Return "0"
            Throw New Exception("Il valore numerico non e' corretto.")
            Exit Function
        End If

        'Se StringaNumero non e' numerico, restituisco lo zero (0)
        If Not IsNumeric(StringaNumero.ToString) Then
            Return "0"
            Throw New Exception("Il valore numerico non e' corretto.")
            Exit Function
        End If

        StringaNumero = Replace(StringaNumero, ".", SeparatoreDecimale)

        Return StringaNumero

    End Function



    Public Shared Sub LatitudineLongitudineDaKWT_QRY(
            GisTXT As String,
            AliasLatitudine As String,
            AliasLongitudine As String,
            ConvertToNumber As Boolean,
            stb As StringBuilder)

        LatitudineLongitudineDaKWT_QRY_Latitudine(GisTXT, ConvertToNumber, AliasLatitudine, stb)
        stb.Append(",")
        stb.AppendLine("")
        LatitudineLongitudineDaKWT_QRY_Longitudine(GisTXT, ConvertToNumber, AliasLongitudine, stb)

    End Sub

    Private Shared Sub LatitudineLongitudineDaKWT_QRY_Longitudine(GisTXT As String, ConvertToNumber As Boolean, AliasLongitudine As String, stb As StringBuilder)

        If ConvertToNumber Then
            stb.AppendLine("    cast (")
            stb.AppendLine("        substring (")
        End If
        stb.AppendLine("            substring(")
        stb.AppendLine("                RTrim(LTrim(Replace(Replace(" & GisTXT & ", 'POINT (', ''), ')', ''))),")
        stb.AppendLine("                0,")
        stb.AppendLine("                Len(")
        stb.AppendLine("                    RTrim(LTrim(Replace(Replace(" & GisTXT & ", 'POINT (', ''), ')', '')))")
        stb.AppendLine("                    ) - charindex(")
        stb.AppendLine("                        ' ',")
        stb.AppendLine("                        RTrim(LTrim(Replace(Replace(" & GisTXT & ", 'POINT (', ''), ')', ''))),")
        stb.AppendLine("                        0")
        stb.AppendLine("                    )")
        stb.AppendLine("                )")
        If ConvertToNumber Then
            stb.AppendLine("			, 0, 12 --oltre i 12 non viene riconvertito in float")
            stb.AppendLine("		)")
            stb.AppendLine("		as float")
            stb.AppendLine("	)")
        End If
        stb.Append("as " & AliasLongitudine)

    End Sub

    Private Shared Sub LatitudineLongitudineDaKWT_QRY_Latitudine(GisTXT As String, ConvertToNumber As Boolean, AliasLatitudine As String, stb As StringBuilder)

        If ConvertToNumber Then
            stb.AppendLine("    CAST (")
            stb.AppendLine("        SUBSTRING (")
        End If

        stb.AppendLine("			RTrim(")
        stb.AppendLine("                LTrim(")
        stb.AppendLine("                    SUBSTRING(")
        stb.AppendLine("                        RTrim(LTrim(Replace(Replace(" & GisTXT & ", 'POINT (', ''), ')', ''))),")
        stb.AppendLine("                        charindex(' ', RTrim(LTrim(Replace(Replace(" & GisTXT & ", 'POINT (', ''), ')', ''))), 0),")
        stb.AppendLine("                        Len(" & GisTXT & ")")
        stb.AppendLine("                    )")
        stb.AppendLine("                ) ")
        stb.AppendLine("			) ")

        If ConvertToNumber Then
            stb.AppendLine("			, 0, 12) --oltre i 12 non viene riconvertito in float")
            stb.AppendLine("		as float")
            stb.AppendLine("	)")
        End If
        stb.Append(" as " & AliasLatitudine)

    End Sub

    Public Shared Function Agro_SQL_SaveGeograpyFromWKTString(ByVal WKT As String) As String
        Return " geography::STGeomFromText('" & Agro_SQL_SaveText(WKT) & "' , 4326) "
    End Function

    Public Shared Function Agro_SQL_SaveGeograpyFromGMLString(ByVal gml As String) As String
        Return " geography::GeomFromGml('" & Agro_SQL_SaveText(gml) & "' , 4326)"
    End Function

    Public Shared Function Agro_SQL_SaveGeograpyFromGMLString_usa_MakeValid(ByVal gml As String) As String
        Return "geography::STGeomFromWKB(GEOMETRY::GeomFromGml('" & Agro_SQL_SaveText(gml) & "', 4326).MakeValid().STAsBinary(), 4326)"
    End Function


    Public Shared Function Agro_SQL_SaveBoolStrToInt(ByVal StringaBool As String) As String
        Dim rval As String = "0"
        If StringaBool.ToLower = "true" Then
            rval = "1"
        End If

        Return rval
    End Function

    Public Shared Function Agro_SQL_SaveBoolStrToInt_NULL(ByVal StringaBool As String) As String

        If IsDBNull(StringaBool) Then
            Return "NULL"
        End If

        Dim rval As String = "0"
        If StringaBool.ToLower = "true" Then
            rval = "1"
        End If

        Return rval
    End Function

    Public Shared Function Agro_SQL_SaveStringToXML(ByVal StringaXml As String) As String
        Return "CONVERT(XML, N'" & Agro_SQL_SaveText(StringaXml) & "')"
    End Function

    '##############################################################################################
    Public Shared Function Agro_SQL_SaveNum(ByVal StringaNumero As String) As String

        Dim identificatoreMetodo As Guid = Guid.Empty
        If ControllaPerInjection(identificatoreMetodo) Then
            Return DataProviderFactory.Instance.Agro_SQL_SaveNum(StringaNumero, False, identificatoreMetodo)
        End If

        Dim StrSeparatoreDecimale As String = ""

        'Recupero il Separatore Decimale di Sistema
        StrSeparatoreDecimale = SeparatoreDecimale()

        'Verifico il separatore decimale desiderato
        If (StrSeparatoreDecimale <> ",") AndAlso (StrSeparatoreDecimale <> ".") Then
            Return "0"
            Throw New Exception("Il separatore decimale indicato non e' supportato.")
            Exit Function
        End If

        'Se StringaNumero e' nullo, restituisco uno zero (0)
        If IsNothing(StringaNumero) Then
            Return "0"
            Throw New Exception("Il valore numerico non e' corretto.")
            Exit Function
        End If

        'Se StringaNumero e' una stringa vuota, restituisco lo zero (0)
        If StringaNumero = "" Then
            Return "0"
            Throw New Exception("Il valore numerico non e' corretto.")
            Exit Function
        End If

        'Se StringaNumero non e' numerico, restituisco lo zero (0)
        If Not IsNumeric(StringaNumero.ToString) Then
            Return "0"
            Throw New Exception("Il valore numerico non e' corretto.")
            Exit Function
        End If

        'Verifico in quale situazione mi trovo
        If StrSeparatoreDecimale = "," Then

            '===== Caso ITALIANO =====

            'Elimino i punti
            StringaNumero = Replace(StringaNumero, ".", "")

            'Sostituisco la virgola col punto
            StringaNumero = Replace(StringaNumero, ",", ".")

            'Restituisco in uscita il risultato
            Return StringaNumero

        Else

            '===== Caso ANGLOSASSONE =====

            'Elimino le virgole
            StringaNumero = Replace(StringaNumero, ",", "")

            'Restituisco in uscita il risultato
            Return StringaNumero

        End If

    End Function


    ''' <summary>
    ''' se il valore passato è DBNull ritorna Nothing, altrimenti il valore stesso 
    ''' </summary>
    ''' <param name="valore">Valore su cui effettuare il controllo</param>
    ''' <returns>se il valore passato è DBNull ritorna Nothing, altrimenti il valore stesso <br/>
    ''' </returns>
    ''' <remarks>
    ''' Autore: vanni<br/>
    ''' Data creazione: 23/08/2005
    ''' </remarks>
    Public Shared Function DBNullToNothing(ByVal valore As Object) As Object
        If System.Convert.IsDBNull(valore) Then
            Return Nothing
        Else
            Return valore
        End If
    End Function

    ''' <summary>
    ''' se il valore passato è Nothing ritorna DBNull.value, altrimenti il valore stesso
    ''' </summary>
    ''' <param name="valore">Valore su cui effettuare il controllo</param>
    ''' <returns>se il valore passato è Nothing ritorna DBNull.Value, altrimenti il valore stesso<br/>
    ''' </returns>
    ''' <remarks>
    ''' Autore: vanni<br/>
    ''' Data creazione: 23/08/2005
    ''' </remarks>
    Public Shared Function NothingToDBNull(ByVal valore As Object) As Object
        If valore Is Nothing Then
            Return DBNull.Value
        Else
            Return valore
        End If
    End Function




    '##############################################################################################
    Public Shared Function SeparatoreDecimale() As String

        Dim StrSeparatoreDecimale As String = ""
        Dim xNumberFormatInfo As NumberFormatInfo

        xNumberFormatInfo = CultureInfo.CurrentCulture.NumberFormat
        StrSeparatoreDecimale = xNumberFormatInfo.NumberDecimalSeparator()

        Return StrSeparatoreDecimale

    End Function

    Public Shared Function VB_Save_Decimal(ByVal StringaNumero As String) As String

        Dim StrSeparatoreDecimale As String = ""

        'Recupero il Separatore Decimale di Sistema
        StrSeparatoreDecimale = SeparatoreDecimale()

        'Verifico il separatore decimale desiderato
        If (StrSeparatoreDecimale <> ",") AndAlso (StrSeparatoreDecimale <> ".") Then
            Return "0"
            Throw New Exception("Il separatore decimale indicato non e' supportato.")
            Exit Function
        End If

        'Se StringaNumero e' nullo, restituisco uno zero (0)
        If IsNothing(StringaNumero) Then
            Return "0"
            Throw New Exception("Il valore numerico non e' corretto.")
            Exit Function
        End If

        'Se StringaNumero e' una stringa vuota, restituisco lo zero (0)
        If StringaNumero = "" Then
            Return "0"
            Throw New Exception("Il valore numerico non e' corretto.")
            Exit Function
        End If

        'Se StringaNumero non e' numerico, restituisco lo zero (0)
        If Not IsNumeric(StringaNumero.ToString) Then
            Return "0"
            Throw New Exception("Il valore numerico non e' corretto.")
            Exit Function
        End If

        'Verifico in quale situazione mi trovo
        If StrSeparatoreDecimale = "," Then

            '===== Caso ITALIANO =====

            'Sostituisco la virgola col punto
            StringaNumero = Replace(StringaNumero, ".", ",")

            'Restituisco in uscita il risultato
            Return StringaNumero

        Else

            '===== Caso ANGLOSASSONE =====

            'Sostituisco la virgola col punto
            StringaNumero = Replace(StringaNumero, ",", ".")

            'Restituisco in uscita il risultato
            Return StringaNumero

        End If

    End Function




    '##############################################################################################
    Public Shared Function Agro_XML_GetInteger(
                                        ByRef ElementoXml As XmlElement,
                                        ByVal NomeAttributo As String,
                                        ByVal ValoreDefault As Integer) _
                                            As Integer

        Dim Testo As String

        'Se l'attributo e' presente nella stringa XML ...
        If ElementoXml.HasAttribute(NomeAttributo) Then

            'Recupero il valore dell'attributo
            Testo = ElementoXml.GetAttribute(NomeAttributo)

            If Testo <> "" Then
                'Verifico la correttezza del valore
                If IsNumeric(Testo) Then
                    'Restituisco il valore convertito
                    Return CInt(Testo)
                Else
                    'Valore non corretto
                    Return ValoreDefault
                End If
            Else
                'Valore non corretto
                Return ValoreDefault
            End If
        Else
            'Attributo non presente nella stringa XML
            Return ValoreDefault
        End If

    End Function



    '##############################################################################################
    Public Shared Function Agro_XML_GetString(
                                        ByRef ElementoXml As XmlElement,
                                        ByVal NomeAttributo As String,
                                        ByVal ValoreDefault As String) _
                                            As String

        Dim Testo As String

        'Se l'attributo e' presente nella stringa XML ...
        If ElementoXml.HasAttribute(NomeAttributo) Then

            'Recupero il valore dell'attributo
            Testo = ElementoXml.GetAttribute(NomeAttributo)

            Return Testo

        Else
            'Attributo non presente nella stringa XML
            Return ValoreDefault
        End If

    End Function

    '##############################################################################################
    Public Shared Function Agro_XML_GetString_NoVuota(
                                        ByRef ElementoXml As XmlElement,
                                        ByVal NomeAttributo As String,
                                        ByVal ValoreDefault As String) _
                                            As String

        Dim Testo As String

        'Se l'attributo e' presente nella stringa XML ...
        If ElementoXml.HasAttribute(NomeAttributo) Then

            'Recupero il valore dell'attributo
            Testo = ElementoXml.GetAttribute(NomeAttributo)

            'Verifico la correttezza del valore
            If Testo <> "" Then
                'Restituisco il valore convertito
                Return Testo
            Else
                'Valore non corretto
                Return ValoreDefault
            End If

            Return Testo

        Else
            'Attributo non presente nella stringa XML
            Return ValoreDefault
        End If

    End Function



    '##############################################################################################
    Public Shared Function Agro_XML_GetDate(
                                        ByRef ElementoXml As XmlElement,
                                        ByVal NomeAttributo As String,
                                        ByVal ValoreDefault As Date) _
                                            As Date

        Dim Testo As String

        'Se l'attributo e' presente nella stringa XML ...
        If ElementoXml.HasAttribute(NomeAttributo) Then

            'Recupero il valore dell'attributo
            Testo = ElementoXml.GetAttribute(NomeAttributo)

            If Testo <> "" Then
                'Verifico la correttezza del valore
                If IsDate(Testo) Then
                    'Restituisco il valore convertito
                    Return CDate(Testo)
                Else
                    'Valore non corretto
                    Return ValoreDefault
                End If
            Else
                'Valore non corretto
                Return ValoreDefault
            End If
        Else
            'Attributo non presente nella stringa XML
            Return ValoreDefault
        End If

    End Function


    '##############################################################################################
    Public Shared Function Agro_XML_GetDateString(
                                        ByRef ElementoXml As XmlElement,
                                        ByVal NomeAttributo As String,
                                        ByVal ValoreDefault As String) _
                                            As String

        Dim Testo As String

        'Se l'attributo e' presente nella stringa XML ...
        If ElementoXml.HasAttribute(NomeAttributo) Then

            'Recupero il valore dell'attributo
            Testo = ElementoXml.GetAttribute(NomeAttributo)

            If Testo <> "" Then
                'Verifico la correttezza del valore
                If IsDate(Testo) Then
                    'Restituisco il valore convertito
                    Return Testo
                Else
                    'Valore non corretto
                    Return ValoreDefault
                End If
            Else
                'Valore non corretto
                Return ValoreDefault
            End If
        Else
            'Attributo non presente nella stringa XML
            Return ValoreDefault
        End If

    End Function



    '##############################################################################################
    Public Shared Function Agro_XML_GetDecimal(
                                        ByRef ElementoXml As XmlElement,
                                        ByVal NomeAttributo As String,
                                        ByVal ValoreDefault As Decimal) _
                                            As Decimal

        Dim Testo As String

        'Se l'attributo e' presente nella stringa XML ...
        If ElementoXml.HasAttribute(NomeAttributo) Then

            'Recupero il valore dell'attributo
            Testo = ElementoXml.GetAttribute(NomeAttributo)

            If Testo <> "" Then
                'Verifico la correttezza del valore
                If IsNumeric(Testo) Then
                    'Restituisco il valore convertito
                    Return CDbl(Testo)
                Else
                    'Valore non corretto
                    Return ValoreDefault
                End If
            Else
                'Valore non corretto
                Return ValoreDefault
            End If
        Else
            'Attributo non presente nella stringa XML
            Return ValoreDefault
        End If

    End Function

    '################################################################################
    Public Shared Function TitoloPossessoDes_from_TitoloPossessoCod(ByVal TitoloPossessoCod As Integer) As String

        Dim Des As String

        Select Case TitoloPossessoCod

            Case enum_TitoloPossesso.Altro
                Des = "Altro"
            Case enum_TitoloPossesso.Proprieta
                Des = "Proprietà"
            Case enum_TitoloPossesso.Comodato
                Des = "Comodato d'uso"
            Case enum_TitoloPossesso.AffittoContratto
                Des = "Affitto con contratto"
            Case enum_TitoloPossesso.AffittoSenzaContratto
                Des = "Affitto senza contratto"
            Case enum_TitoloPossesso.InContoTerzi
                Des = "In conto terzi"
            Case enum_TitoloPossesso.InConvenzione
                Des = "In convenzione"
            Case enum_TitoloPossesso.InCompartecipazione
                Des = "In compartecipazione"
            Case Else
                Des = "Altro (non definito)"
        End Select

        'Restituisco il risultato
        Return Des

    End Function

    '========================================================================
    'controlla l'uguaglianza di 2 oggetti
    Public Function ColonneUguali(ByVal A As Object, ByVal B As Object) As Boolean

        'confronta 2 valori x vedere se sono uguali, confronta anche il dbnull 
        If IsDBNull(A) AndAlso IsDBNull(B) Then
            Return True 'entrambi db null
        End If

        If IsDBNull(A) OrElse IsDBNull(B) Then
            Return False 'solo uno è db null
        End If

        Return A.Equals(B) 'confronta i 2 oggetti e ritorna un booleano x indicare se sono uguali o meno..

    End Function

    '========================================================================
    'Dato un Datatable fa il distinct su un campo e ritorna un array di stringhe...
    Public Function SelectDistinct(ByVal SourceTable As DataTable, ByVal FieldName As String, Optional ByVal Ordinato As Boolean = True) As String()

        'array da ritornare...
        Dim strRet As String()
        Dim LastValue As Object = Nothing
        Dim dr As DataRow
        'indice x l'array...
        Dim i As Integer = 0

        If Ordinato Then

            'ciclo filtro vuoto ordinando x il nome campo richiesto x il distinct
            For Each dr In SourceTable.Select("", FieldName)

                'se l'ultimo valori è nothing prendo il valore....(succede solo la prima volta)
                If LastValue Is Nothing Then
                    'assegno l'ultimo valore...
                    LastValue = dr(FieldName)
                    'aggiungo la nuova riga....
                    ReDim Preserve strRet(i)
                    strRet(i) = LastValue.ToString 'assegno il valore del campo....
                    'incremento l'indice...
                    i += 1
                Else

                    If Not ColonneUguali(LastValue, dr(FieldName)) Then 'le volte successive controllo se sono uguali
                        'assegno l'ultimo valore...
                        LastValue = dr(FieldName)
                        'aggiungo la nuova riga....
                        ReDim Preserve strRet(i)
                        strRet(i) = LastValue.ToString 'assegno il valore del campo....
                        'incremento l'indice...
                        i += 1
                    End If

                End If

            Next

        Else

            'ciclo filtro vuoto ordinando x il nome campo richiesto x il distinct
            For Each dr In SourceTable.Select()

                'se l'ultimo valori è nothing prendo il valore....(succede solo la prima volta)
                If LastValue Is Nothing Then
                    'assegno l'ultimo valore...
                    LastValue = dr(FieldName)
                    'aggiungo la nuova riga....
                    ReDim Preserve strRet(i)
                    strRet(i) = LastValue.ToString 'assegno il valore del campo....
                    'incremento l'indice...
                    i += 1
                Else

                    If Not ColonneUguali(LastValue, dr(FieldName)) Then 'le volte successive controllo se sono uguali
                        'assegno l'ultimo valore...
                        LastValue = dr(FieldName)
                        'aggiungo la nuova riga....
                        ReDim Preserve strRet(i)
                        strRet(i) = LastValue.ToString 'assegno il valore del campo....
                        'incremento l'indice...
                        i += 1
                    End If

                End If

            Next

        End If

        Return strRet 'ritorno altrimenti solo la tabella

    End Function


    '################################################################################

    Public Shared Function Ettari_from_EttariAreCentiare(
        ByVal Ettari As Decimal,
        ByVal Are As Decimal,
        ByVal Centiare As Decimal
        ) As Decimal

        Dim SupHa As Decimal

        'Calcolo
        SupHa = CInt(Ettari) + (Are / 100) + (Centiare / 10000)

        'Restituisco il risultato
        Return SupHa

    End Function

    Public Shared Function Ettari_from_StringaEttariAreCentiare(
        ByVal ettariAreCentiare As String,
        Optional ByVal separatore As String = "."
        ) As Decimal

        Dim arrayEttariAreCentiare As String() = ettariAreCentiare.Split(separatore)

        Const indiceEttari = 0
        Const indiceAre = 1
        Const indeiceCentiare = 2

        Dim ettari = CInt(arrayEttariAreCentiare(indiceEttari))
        Dim are = CInt(arrayEttariAreCentiare(indiceAre))
        Dim centiare = CInt(arrayEttariAreCentiare(indeiceCentiare))

        Return Ettari_from_EttariAreCentiare(ettari, are, centiare)

    End Function

    '################################################################################

    Public Shared Sub EttariAreCentiare_from_Ettari(
        ByVal EttariAreCentiare As Decimal,
        ByRef Ettari As Decimal,
        ByRef Are As Decimal,
        ByRef Centiare As Decimal)

        Dim EttariAreCentiareLocal As Decimal

        Dim SupAreCentiare As Decimal
        Dim SupCentiare As Decimal

        If EttariAreCentiare < 0 Then
            EttariAreCentiareLocal = (-1) * EttariAreCentiare
        Else
            EttariAreCentiareLocal = EttariAreCentiare
        End If

        Ettari = Int(EttariAreCentiareLocal)
        SupAreCentiare = EttariAreCentiareLocal - Ettari + 0.00001D
        Are = Int(SupAreCentiare * 100)
        SupCentiare = SupAreCentiare * 100 - Are
        Centiare = Int(SupCentiare * 100)

        If EttariAreCentiare < 0 Then
            Ettari = (-1) * Ettari
        End If

    End Sub

    '#########################################################################
    Public Shared Function MetodoProduzioneDes_from_Cod(ByVal MP_Cod As Integer) As String
        Dim MP_des As String = ""
        Select Case MP_Cod
            Case enum_MetodoProduzione.Integrato
                MP_des = "Integrato"
            Case enum_MetodoProduzione.InConversione
                MP_des = "In conversione"
            Case enum_MetodoProduzione.Biologico
                MP_des = "Biologico"
        End Select
        Return MP_des
    End Function

    ''################################################################################
    <Obsolete("usare Numeri_from_StringaAlfaNumerica di questa classe!!!!!!!")>
    Public Function Numero_from_Stringa(ByVal Stringa As String) As Decimal
        If IsNothing(Stringa) Then
            Return -1
        End If
        Dim i As Integer
        Dim Array As Char()
        Dim Numero As String = ""

        Array = Stringa.ToCharArray()

        For i = 0 To Array.Length - 1
            If IsNumeric(Array(i)) Then
                Numero += Array(i)
            End If
        Next

        If IsNumeric(Numero) Then
            Return CDbl(Numero)
        Else
            Return -1
        End If

    End Function

    ''################################################################################
    '
    Public Function Numero_from_Stringa_MaxCaratteri(ByVal Stringa As String, ByVal NumMaxCaratteri As Integer) As Decimal
        If IsNothing(Stringa) Then
            Return -1
        End If
        Dim i As Integer
        Dim Array As Char()
        Dim Numero As String = ""

        Array = Stringa.ToCharArray()

        For i = 0 To Array.Length - 1
            If IsNumeric(Array(i)) Then
                Numero += Array(i)
            End If
        Next

        If Numero.Length > NumMaxCaratteri Then

            Numero = ""
            For i = 0 To Array.Length - 1
                If IsNumeric(Array(i)) Then
                    Numero += Array(i)
                Else
                    Exit For
                End If
            Next


        End If

        If IsNumeric(Numero) Then
            Return CDbl(Numero)
        Else
            Return -1
        End If

    End Function

    ''################################################################################
    <Obsolete("usare StringaLettere_from_StringaconNumeri di questa classe!!!!!!!")>
    Public Function Stringa_from_StringaconNumeri(ByVal StringaconNumero As String) As String

        Dim i As Integer
        Dim Array As Char()
        Dim Stringa As String = ""

        Array = StringaconNumero.ToCharArray()

        For i = 0 To Array.Length - 1
            If Not IsNumeric(Array(i)) Then
                If Asc(UCase(Array(i))) >= 65 AndAlso Asc(UCase(Array(i))) <= 90 Then
                    Stringa += Array(i)
                End If
            End If
        Next

        Return Stringa

    End Function

    '##########################################################################################
    Public Shared Sub Calcola_BaseCode_TopCode(ByRef BaseCode As Integer,
                                               ByRef TopCode As Integer,
                                               ByVal IndiceProgressivoGIAS As Integer)

        '----- Calcolo il BaseCode e il TopCode
        BaseCode = IndiceProgressivoGIAS * (2 ^ AgroCode_BitPerCodice)
        TopCode = BaseCode + (2 ^ AgroCode_BitPerCodice) - 1

    End Sub

    '##########################################################################################
    Public Shared Function BaseCode_from_ProgressivoGias(ByVal ProgressivoGias As Integer) As Integer
        Return ProgressivoGias * (2 ^ AgroCode_BitPerCodice)
    End Function

    '##########################################################################################
    Public Shared Function TopCode_from_ProgressivoGias(ByVal ProgressivoGias As Integer) As Integer
        Return ProgressivoGias * (2 ^ AgroCode_BitPerCodice) + (2 ^ AgroCode_BitPerCodice) - 1
    End Function


    '################################################################
    Public Shared Sub AgroMsgBox(ByVal Testo As String,
                                ByRef objPage As System.Web.UI.Page,
                                Optional ByVal NomeForm As String = "FORM1",
                                Optional ByVal EsisteMaster As Boolean = False,
                                Optional ByVal scriptAggiuntivo As String = Nothing)

        '----- Formatto il testo di ingresso in modo che non crei problemi ...

        'Elimino il carattere \ e lo sostituisco con \\
        Testo = Replace(Testo, "\", "\\")

        'Elimino il carattere (o coppia di caratteri) VBCRLF che crea problemi ...
        Testo = Replace(Testo, vbCrLf, Chr(13))

        'Elimino i doppi apici sostituendoli con due singoli apostrofi chr(96)
        Testo = Replace(Testo, Chr(34), Chr(96) & Chr(96))

        'Elimino i singoli apici sostituendoli con un apostrofo chr(96)
        Testo = Replace(Testo, Chr(39), Chr(96))

        'Gli eventuali Chr(13) li sostituisco con l'equivalente JS
        'attenzione! va messo dopo la sostituzione di \ con \\ !!!
        Testo = Replace(Testo, Chr(13), "\r")

        Dim csType = objPage.GetType
        Dim csName = "KendoAlertScript"
        Dim cs = objPage.ClientScript
        Dim csText As New StringBuilder()
        csText.AppendLine("<script type=""text/javascript"">")
        csText.AppendLine("alert('" & Testo & "');")
        If scriptAggiuntivo IsNot Nothing Then
            csText.AppendLine(scriptAggiuntivo)
        End If
        csText.AppendLine("</script>")

        '----- Faccio apparire un msgbox aggiungendo il controllo alla form
        If EsisteMaster Then

            'questa riga non fa compilare l'agronicastampe 2003
            objPage.Master.FindControl(NomeForm).Controls.Add(
                New LiteralControl(
                    csText.ToString))

        Else

            If objPage.FindControl(NomeForm) IsNot Nothing Then

                objPage.FindControl(NomeForm).Controls.Add(
                New LiteralControl(csText.ToString))

            Else


                cs.RegisterClientScriptBlock(csType, csName, csText.ToString())

            End If

        End If

    End Sub


    '##########################################################################################
    'la querystring vuole il ?
    Public Shared Sub Page_NewWindow(ByRef objPage As System.Web.UI.Page,
                                ByVal NomePaginaAspx As String,
                                ByVal QueryString As String,
                                Optional ByVal Pagina_Titolo As String = "GiasOnline",
                                Optional ByVal Pagina_Height As Integer = 700,
                                Optional ByVal Pagina_Width As Integer = 1000,
                                Optional ByVal Pagina_Top As Integer = 0,
                                Optional ByVal Pagina_Left As Integer = 0,
                                Optional ByVal Menubar As String = "yes",
                                Optional ByVal Resizable As String = "yes",
                                Optional ByVal Scrollbars As String = "yes",
                                Optional ByVal NomeForm As String = "FORM1",
                                Optional ByVal EsisteMaster As Boolean = False)


        Dim StrWindowOpen As String

        '----- Preparo la stringa di apertura di una nuova pagina

        StrWindowOpen = "<script language='javascript'>" &
                        vbNewLine &
                        "window.open('" &
                            NomePaginaAspx &
                            "" & QueryString & "'," &
                            "'" & Pagina_Titolo & "'," &
                            "'height=" & CStr(Pagina_Height) & "," &
                            "width=" & CStr(Pagina_Width) & "," &
                            "menubar=" & Menubar & "," &
                            "resizable=" & Resizable & "," &
                            "scrollbars=" & Scrollbars & "," &
                            "top=" & CStr(Pagina_Top) & ",left=" & CStr(Pagina_Left) & "');" &
                        vbNewLine &
                        "</script>"

        'Apro la finestra...

        If EsisteMaster Then
            'questa riga non fa compilare l'agronicastampe 2003
            'objPage.Master.FindControl(NomeForm).Controls.Add(New LiteralControl(StrWindowOpen))
        Else
            objPage.FindControl(NomeForm).Controls.Add(New LiteralControl(StrWindowOpen))
        End If



    End Sub


    '##########################################################################################
    Public Shared Sub AgroHelp(ByVal LinkPagina As String,
                       ByRef objPage As Object,
                       Optional ByVal NomeForm As String = "FORM1")

        Dim strOpen As String

        strOpen = "<script language='javascript'>" & vbNewLine &
                        "window.open('" & LinkPagina & "'," &
                        "'Help','height=700,width=1000,scrollbars=yes,top=0,left=0,toolbar=yes');" & vbNewLine &
                        "</script>"

        'Apro la finestra...
        objPage.FindControl("Form1").Controls.Add(New LiteralControl(strOpen))


    End Sub

    ''##########################################################################################
    ''usare UtilityProvider.Page_NewWindow_xUpdatePanel
    'Public Shared Sub Page_NewWindow_xUpdatePanel()

    'End Sub

    '##########################################################################################
    'la querystring vuole il ?
    Public Shared Function Page_NewWindow_RitornaJavascript(ByRef objPage As System.Web.UI.Page,
                                                        ByVal NomePaginaAspx As String,
                                                        ByVal QueryString As String,
                                                        Optional ByVal Pagina_Titolo As String = "GiasOnline",
                                                        Optional ByVal Pagina_Height As Integer = 700,
                                                        Optional ByVal Pagina_Width As Integer = 1000,
                                                        Optional ByVal Pagina_Top As Integer = 0,
                                                        Optional ByVal Pagina_Left As Integer = 0,
                                                        Optional ByVal Menubar As String = "yes",
                                                        Optional ByVal Resizable As String = "yes",
                                                        Optional ByVal Scrollbars As String = "yes") As String


        Dim StrWindowOpen As String

        '----- Preparo la stringa di apertura di una nuova pagina

        StrWindowOpen = "<script language='javascript'>" &
                        vbNewLine &
                        "myPopUp = window.open('" &
                            NomePaginaAspx &
                            "" & QueryString & "'," &
                            "'" & Pagina_Titolo & "'," &
                            "'height=" & CStr(Pagina_Height) & "," &
                            "width=" & CStr(Pagina_Width) & "," &
                            "menubar=" & Menubar & "," &
                            "resizable=" & Resizable & "," &
                            "scrollbars=" & Scrollbars & "," &
                            "top=" & CStr(Pagina_Top) & ",left=" & CStr(Pagina_Left) & "');" &
                            vbNewLine & "myPopUp.focus();" &
                        vbNewLine &
                        "</script>"

        Return StrWindowOpen

    End Function

    '##########################################################################################
    'la querystring vuole il ? 
    Public Shared Sub Page_ModalDialog(ByRef objPage As System.Web.UI.Page,
                                       ByVal Path_NomePaginaAspx As String,
                                       ByVal QueryString As String,
                                       ByVal TextBox As String,
                                       Optional ByVal Pagina_Height As Integer = 700,
                                       Optional ByVal Pagina_Width As Integer = 1000,
                                       Optional ByVal Pagina_Top As Integer = 0,
                                       Optional ByVal Pagina_Left As Integer = 0,
                                       Optional ByVal Status As String = "no",
                                       Optional ByVal Center As String = "yes",
                                       Optional ByVal Edge As String = "raised",
                                       Optional ByVal Menubar As String = "yes",
                                       Optional ByVal Resizable As String = "yes",
                                       Optional ByVal Scrollbars As String = "yes",
                                       Optional ByVal NomeForm As String = "FORM1")


        Dim StrWindowOpen As String

        '----- Preparo la stringa di apertura di una nuova pagina
        StrWindowOpen = Page_ModalDialog_Script(Path_NomePaginaAspx, QueryString, TextBox, Pagina_Height, Pagina_Width, Pagina_Top, Pagina_Left, Status, Center, Edge, Menubar, Resizable, Scrollbars, NomeForm)

        'Apro la finestra...
        objPage.FindControl(NomeForm).Controls.Add(New LiteralControl(StrWindowOpen))

    End Sub


    Public Shared Function Page_ModalDialog_Script(ByVal Path_NomePaginaAspx As String,
                                                   ByVal QueryString As String,
                                                   ByVal TextBox As String,
                                                   Optional ByVal Pagina_Height As Integer = 700,
                                                   Optional ByVal Pagina_Width As Integer = 1000,
                                                   Optional ByVal Pagina_Top As Integer = 0,
                                                   Optional ByVal Pagina_Left As Integer = 0,
                                                   Optional ByVal Status As String = "no",
                                                   Optional ByVal Center As String = "yes",
                                                   Optional ByVal Edge As String = "raised",
                                                   Optional ByVal Menubar As String = "yes",
                                                   Optional ByVal Resizable As String = "yes",
                                                   Optional ByVal Scrollbars As String = "yes",
                                                   Optional ByVal NomeForm As String = "FORM1") As String

        Dim features As String = "dialog=yes," &
                                "width=" & Pagina_Width & "," &
                                "height=" & Pagina_Height & "," &
                                "top=" & Pagina_Top & "," &
                                "left=" & Pagina_Left & "," &
                                "menubar=" & Menubar & "," &
                                "resizable=" & Resizable & "," &
                                "scrollbars=" & Scrollbars

        Dim strWindowOpen As String = "<script type='text/javascript'>" &
                                      "var win = window.open('" & Path_NomePaginaAspx & QueryString & "', '', '" & features & "');" &
                                      "win.focus();" &
                                      "var timer = setInterval(function() {" &
                                      "  if(win.closed) {" &
                                      "    clearInterval(timer);" &
                                      "    if(window.returnValue !== undefined) {" &
                                      "      document.getElementById('" & TextBox & "').value = window.returnValue;" &
                                      "      document.getElementById('" & NomeForm & "').submit();" &
                                      "    }" &
                                      "  }" &
                                      "}, 500);" &
                                      "</script>"

        Return strWindowOpen
    End Function


    Public Shared Function JqueryModalDialogScript(
                                ByVal Path_NomePaginaAspx As String,
                                ByVal QueryString As String,
                                ByVal TextBox As String,
                                Optional ByVal Pagina_Height As Integer = 700,
                                Optional ByVal Pagina_Width As Integer = 1000,
                                Optional ByVal Pagina_Top As Integer = 0,
                                Optional ByVal Pagina_Left As Integer = 0,
                                Optional ByVal Status As String = "no",
                                Optional ByVal Center As String = "yes",
                                Optional ByVal Edge As String = "raised",
                                Optional ByVal Menubar As String = "yes",
                                Optional ByVal Resizable As String = "yes",
                                Optional ByVal Scrollbars As String = "yes",
                                Optional ByVal NomeForm As String = "FORM1") As String


        Dim StrWindowOpen As String = "apriFormDialog('" & Path_NomePaginaAspx & QueryString & "', " & Pagina_Height & "," & Pagina_Width & "); " & vbCrLf

        '----- Preparo la stringa di apertura di una nuova pagina

        'QueryString = " + document.all(" & Chr(34) & "TxtQueryStringProdotto" & Chr(34) & ").value"


        Return StrWindowOpen



    End Function




    '################################################################################################
    Public Shared Function VerificaEspressioneRegolare(
                                        ByVal StringaDaAnalizzare As String,
                                        ByVal EspressioneRegolare As String,
                                        ByVal EspressioneRegolareStandard As enum_EspressioniRegolari) _
                                        As Boolean


        Dim StringaRegExp As String = ""

        'NOTA
        'Se la stringa di convalida e' nulla, allora utilizzo una delle stringhe standard

        If EspressioneRegolare <> "" Then

            'L'espressione regolare di convalida e' passata dall'utente
            StringaRegExp = EspressioneRegolare

        Else

            'Utilizzo una delle stringhe standard
            Select Case EspressioneRegolareStandard

                Case enum_EspressioniRegolari.RegExp_Nessuna  '=================================

                    StringaRegExp = "."

                    '===========================================================================


                Case enum_EspressioniRegolari.RegExp_Email  '===================================

                    StringaRegExp = "^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"

                    '===========================================================================


                Case enum_EspressioniRegolari.RegExp_CodiceFiscale  '===========================
                    '
                    '   AAABBB22C33d456e
                    '
                    StringaRegExp = "^([a-zA-Z]{6}[0-9]{2}[a-zA-Z]{1}[0-9]{2}[a-zA-Z]{1}[0-9]{3}[a-zA-Z]{1})$"
                    '
                    '   [\w{6}\d{2}\w{1}\d{2}\w{1}\d{3}\w{1}]
                    '
                    '===========================================================================


                Case enum_EspressioniRegolari.RegExp_CAP  '===========================
                    '
                    '   012345
                    '
                    StringaRegExp = "([0-9]{5})"
                    '
                    '   [^\d{5}$]
                    '
                    '===========================================================================


                Case enum_EspressioniRegolari.RegExp_PartitaIVA  '==============================
                    '
                    '   01234567890
                    '
                    StringaRegExp = "([0-9]{11})"

                    '===========================================================================


                Case enum_EspressioniRegolari.RegExp_Username  '==============================

                    StringaRegExp = EXPREG_USERNAME

                    '===========================================================================

                Case enum_EspressioniRegolari.RegExp_Password '==============================

                    StringaRegExp = EXPREG_PASSWORD

                    '===========================================================================

            End Select

        End If

        'Restituisco il risultato
        Dim Regolus As New System.Text.RegularExpressions.Regex(StringaRegExp, RegexOptions.None, TimeSpan.FromSeconds(3))
        Return Regolus.IsMatch(StringaDaAnalizzare)



    End Function

    Public Shared Function DataNascita_from_CodFisc(ByVal CodFisc As String) As String

        If VerificaEspressioneRegolare(CodFisc, "", enum_EspressioniRegolari.RegExp_CodiceFiscale) Then

            CodFisc = UCase(CodFisc)

            Dim Giorno As String = ""
            Dim Mese As String = ""
            Dim Anno As String = ""
            Dim DataNascita As String = ""

            Anno = "19" & CodFisc.Substring(6, 2)

            Select Case CodFisc.Substring(8, 1)
                Case "A"
                    Mese = "01"
                Case "B"
                    Mese = "02"
                Case "C"
                    Mese = "03"
                Case "D"
                    Mese = "04"
                Case "E"
                    Mese = "05"
                Case "H"
                    Mese = "06"
                Case "L"
                    Mese = "07"
                Case "M"
                    Mese = "08"
                Case "P"
                    Mese = "09"
                Case "R"
                    Mese = "10"
                Case "S"
                    Mese = "11"
                Case "T"
                    Mese = "12"
            End Select

            Select Case Sesso_from_CodFisc(CodFisc, 1)
                Case "M"
                    Giorno = CodFisc.Substring(9, 2)
                Case "F"
                    Giorno = CStr(CInt(CodFisc.Substring(9, 2)) - 40)
            End Select

            DataNascita = Giorno & "/" & Mese & "/" & Anno

            If IsDate(DataNascita) Then
                DataNascita = Format(CDate(DataNascita), "dd/MM/yyyy")
            Else
                DataNascita = ""
            End If

            'Restituisco il risultato
            Return DataNascita

        Else

            Return ""

        End If


    End Function

    Public Shared Function Sesso_from_CodFisc(ByVal CodFisc As String,
                                              ByVal Flag_Iniziale_Completo As Integer
                                              ) As String

        Dim Sesso As String = ""
        If CodFisc <> "" Then
            Select Case Flag_Iniziale_Completo

                Case 1
                    If CodFisc.Substring(9, 1) < "4" Then
                        Sesso = "M"
                    Else
                        Sesso = "F"
                    End If

                Case 2
                    If CodFisc.Substring(9, 1) >= "4" Then
                        Sesso = "Maschio"
                    Else
                        Sesso = "Femmina"
                    End If

            End Select
        Else
            Return ""
        End If

        Return Sesso

    End Function

    '################################################################
    Public Shared Function QS_SaveText(ByVal Testo As String) As String

        'Funzione per formattare le stringhe prima di essere
        'inserite in una querystring

        'Se la stringa e' nulla, restituisco la stringa nulla
        If IsNothing(Testo) Then
            Testo = ""
            Return Testo
        End If

        Testo = Testo.Replace("°", ".")
        Testo = Testo.Replace("è", "e'")
        Testo = Testo.Replace("é", "e'")
        Testo = Testo.Replace("ò", "o'")
        Testo = Testo.Replace("à", "a'")
        Testo = Testo.Replace("ù", "u'")
        Testo = Testo.Replace("ì", "i'")

        'Restituisco il risultato
        Return Testo

    End Function


    '################################################################
    Public Shared Function XML_SaveText(ByVal Testo As String) As String

        'Funzione per formattare le stringhe prima di essere
        'inserite in un xml

        Testo = Testo.Replace("&", "&amp;")
        Testo = Testo.Replace("<", "&lt;")
        Testo = Testo.Replace(">", "&gt;")
        Testo = Testo.Replace("'", "&apos;")
        Testo = Testo.Replace("""", "&quot;")

        'Restituisco il risultato
        Return Testo

    End Function


    '################################################################
    Public Shared Function File_SaveText(ByVal Testo As String) As String

        'Funzione per formattare i nomi di file e cartelle

        Testo = Testo.Replace("\", "")
        Testo = Testo.Replace("/", "")
        Testo = Testo.Replace(":", "")
        Testo = Testo.Replace("*", "")
        Testo = Testo.Replace("?", "")
        Testo = Testo.Replace("""", "")
        Testo = Testo.Replace("<", "")
        Testo = Testo.Replace(">", "")

        'Restituisco il risultato
        Return Testo

    End Function

    '################################################################
    Public Shared Function EliminaAccentiCaratteriSpeciali(ByVal Testo As String) As String

        Testo = Testo.Replace("°", ".")
        Testo = Testo.Replace("'", " ")
        Testo = Testo.Replace("è", "e")
        Testo = Testo.Replace("é", "e")
        Testo = Testo.Replace("ò", "o")
        Testo = Testo.Replace("à", "a")
        Testo = Testo.Replace("ù", "u")
        Testo = Testo.Replace("ì", "i")
        Testo = Testo.Replace("(", "")
        Testo = Testo.Replace(")", "")
        Testo = Testo.Replace("&", "e")

        Return Testo

    End Function

    '##############################################################
    Public Shared Function Des_from_Cod_SuDT(ByVal DT As DataTable,
                                                ByVal NomeCampoCod As String,
                                                ByVal NomeCampoDes As String,
                                                ByVal Codice As Integer) As String

        Dim Des As String = ""

        If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

            Dim Dr As DataRow()

            Dr = DT.Select(" " & NomeCampoCod & " = " & Agro_SQL_SaveNum(Codice))

            If Not IsNothing(Dr) AndAlso Dr.Length > 0 Then
                Des = CStr(Dr(0).Item(NomeCampoDes))
            End If

        End If

        Return Des

    End Function

    '##############################################################
    Public Shared Function Des_from_Cod2_SuDT(ByVal DT As DataTable,
                                                ByVal NomeCampoCod1 As String,
                                                ByVal NomeCampoCod2 As String,
                                                ByVal NomeCampoDes As String,
                                                ByVal Codice1 As Integer,
                                                ByVal Codice2 As Integer) As String

        Dim Des As String = ""

        If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

            Dim Dr As DataRow()

            Dr = DT.Select(" " & NomeCampoCod1 & " = " & Agro_SQL_SaveNum(Codice1) & " AND " & NomeCampoCod2 & " = " & Agro_SQL_SaveNum(Codice2))

            If Not IsNothing(Dr) AndAlso Dr.Length > 0 Then
                Des = CStr(Dr(0).Item(NomeCampoDes))
            End If

        End If

        Return Des

    End Function

    '##############################################################
    Public Shared Function Cod_from_Cod_SuDT(ByVal DT As DataTable,
                                             ByVal NomeCampoCodFiltro As String,
                                             ByVal NomeCampoCodReturn As String,
                                             ByVal Codice As Integer
                                             ) As String

        Dim CodReturn As Integer = 0

        If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

            Dim Dr As DataRow()

            Dr = DT.Select(" " & NomeCampoCodFiltro & " = " & Agro_SQL_SaveNum(Codice))

            If Not IsNothing(Dr) AndAlso Dr.Length > 0 Then
                CodReturn = Dr(0).Item(NomeCampoCodReturn)
            End If

        End If

        Return CodReturn

    End Function


    '################################################################################
    'questa funzione equivale a:
    'appezzamento_Read.AppezzamentoNumero_from_AppezzamentoNome 
    'ma restituisce un Decimal (il numero restituito può superare un integer)
    Public Shared Function Numeri_from_StringaAlfaNumerica(ByVal StringaAlfaNumerica As String) As Decimal

        Dim i As Integer
        Dim Array As Char()
        Dim Numero As String = ""

        Array = StringaAlfaNumerica.ToCharArray()

        For i = 0 To Array.Length - 1
            If IsNumeric(Array(i)) Then
                Numero += Array(i)
            End If
        Next

        If IsNumeric(Numero) Then
            Return CDbl(Numero)
        Else
            Return -1
        End If

    End Function

    '################################################################################
    ' Come quella sopra,  ma si ferma alla prima lettera che trova
    Public Shared Function Numeri_from_StringaAlfaNumerica_Primo(ByVal StringaAlfaNumerica As String) As Decimal

        Dim i As Integer
        Dim Array As Char()
        Dim Numero As String = ""

        Array = StringaAlfaNumerica.ToCharArray()

        Dim primo As Boolean = False

        For i = 0 To Array.Length - 1
            If IsNumeric(Array(i)) Then
                Numero += Array(i)
                primo = True
            ElseIf primo Then
                Exit For
            End If
        Next

        If IsNumeric(Numero) Then
            Return CDbl(Numero)
        Else
            Return -1
        End If

    End Function

    '################################################################################
    Public Shared Function Lettere_from_StringaAlfaNumerica(ByVal StringaAlfaNumerica As String) As String

        Dim Array As Char()
        Dim Lettere As String = ""

        Array = StringaAlfaNumerica.ToCharArray()

        For i As Integer = 0 To Array.Length - 1
            If Not IsNumeric(Array(i)) Then
                Lettere &= Array(i)
            End If
        Next

        Return Lettere

    End Function


    '####################################################################################################################################
    Public Shared Sub CalcolaDate_ScorriMese(ByVal Avanti0_Indietro1 As Integer,
                                            ByRef Data_Inizio As String,
                                            ByRef Data_Fine As String)

        Dim Anno_Inizio As Integer
        Dim Anno_Fine As Integer
        Dim Mese_Inizio As Integer
        Dim Mese_Fine As Integer
        Dim Giorno_Inizio As Integer
        Dim Giorno_Fine As Integer

        Giorno_Inizio = 1 'Primo del mese

        Select Case IsDate(Data_Inizio)

            Case True

                Mese_Inizio = CDate(Data_Inizio).Month
                Anno_Inizio = CDate(Data_Inizio).Year

                Select Case Avanti0_Indietro1

                    Case 0

                        If Mese_Inizio = 12 Then
                            'Anno Successivo
                            Anno_Inizio = Anno_Inizio + 1
                            Mese_Inizio = 1
                        Else
                            Mese_Inizio = Mese_Inizio + 1
                        End If

                    Case 1

                        If Mese_Inizio = 1 Then
                            'Anno Precedente
                            Anno_Inizio = Anno_Inizio - 1
                            Mese_Inizio = 12
                        Else
                            Mese_Inizio = Mese_Inizio - 1
                        End If

                End Select

                Anno_Fine = Anno_Inizio
                Mese_Fine = Mese_Inizio

                Giorno_Fine = CDate(DateSerial(Anno_Fine, Mese_Fine + 1, 0)).Day

                Data_Inizio = Format(CDate(Right("00" & Giorno_Inizio, 2) & "/" & Right("00" & Mese_Inizio, 2) & "/" & Anno_Inizio), "dd/MM/yyyy")
                Data_Fine = Format(CDate(Right("00" & Giorno_Fine, 2) & "/" & Right("00" & Mese_Fine, 2) & "/" & Anno_Fine), "dd/MM/yyyy")


            Case False


        End Select

    End Sub


    '############################################################################################################################
    Public Shared Function Ricava_Anno(ByVal Data_Giorno As String, ByVal Data_Inizio As String, ByVal Data_Fine As String) As String

        Dim Anno As String = ""

        'se ho scelto un giorno metto un solo anno
        If Data_Giorno <> "" Then

            Anno = CStr(CDate(Data_Giorno).Year)

        Else
            'se ho scelto un intervallo..
            Dim Anno1 = CStr(CDate(Data_Inizio).Year)

            Dim Anno2 = CStr(CDate(Data_Fine).Year)

            Dim Differenza = DateDiff(DateInterval.Year, CDate(Data_Inizio), CDate(Data_Fine))

            'metto un anno se l'intervallo è inferiore ad un anno
            If Differenza = 0 Then
                Anno = Anno1
            Else
                'altrimenti metto i due anni dell'intervallo
                Anno = Anno1 & " - " & Anno2
            End If

        End If

        Return Anno

    End Function

    '###########################################################
    Public Sub AnnataAgraria_from_Data(ByVal Data As Date,
                                      ByRef DataInizio_AnnataAgraria As Date,
                                      ByRef DataFine_AnnataAgraria As Date)

        Select Case Data.Month

            Case 11, 12
                'se sono nei mesi di novembre o dicembre..........
                'l'annata agraria va dal 1/11 di quest'anno al 31/10 del prossimo
                DataInizio_AnnataAgraria = CDate("01/11/" & CStr(Data.Year))
                DataFine_AnnataAgraria = CDate("31/10/" & CStr(Data.Year + 1))

            Case Else
                'l'annata agraria va dal 1/11 dell'anno scorso al 31/10 di quest'anno
                DataInizio_AnnataAgraria = CDate("01/11/" & CStr(Data.Year - 1))
                DataFine_AnnataAgraria = CDate("31/10/" & CStr(Data.Year))

        End Select

    End Sub


    Public Shared Sub ElaboraCellaHTML(
                        ByRef Cella As HtmlTableCell,
                        ByVal Bordo_Spessore As Integer,
                        ByVal Bordo_Tipo As String,
                        ByVal Bordo_Colore As String,
                        ByVal Sfondo_Colore As String,
                        ByVal Testo_Allineamento As String,
                        Optional ByVal Testo_Allineamento_Verticale As String = "top",
                        Optional ByVal Colspan As Integer = 0,
                        Optional ByVal Font_Weigth As String = "",
                        Optional ByVal Font_Size As String = "",
                        Optional ByVal Font_Color As String = "",
                        Optional ByVal Tipo As enum_FormattazioneExcel = enum_FormattazioneExcel.FE_Non_Specificato)


        'ESEMPIO : ElaboraCellaHTML(Riga.Cells(2), 1, "", "", "silver", "right")

        'Dim Riga As HtmlTableRow
        'Dim Cella As HtmlTableCell
        Dim TestoStile As String

        Dim xBordoSpessore As String
        Dim xBordoColore As String
        Dim xBordoTipo As String


        '-----------------------
        '----- BORDO CELLA -----
        '-----------------------

        If Bordo_Spessore > 0 Then

            'SPESSORE BORDO
            Select Case Bordo_Spessore
                Case 1
                    xBordoSpessore = "thin"
                Case 2
                    xBordoSpessore = "1px"
                Case 3
                    xBordoSpessore = "2px"
                Case Else
                    xBordoSpessore = "2px"
            End Select

            'COLORE BORDO
            If Bordo_Colore = "" Then
                xBordoColore = "black"
            Else
                xBordoColore = Bordo_Colore
            End If

            'TIPO DI BORDO
            If Bordo_Tipo = "" Then
                xBordoTipo = "solid"
            Else
                xBordoTipo = Bordo_Tipo
            End If


            TestoStile = xBordoColore & " " & xBordoSpessore & " " & xBordoTipo

            Cella.Style.Item("border-right") = TestoStile
            Cella.Style.Item("border-top") = TestoStile
            Cella.Style.Item("border-left") = TestoStile
            Cella.Style.Item("border-bottom") = TestoStile

        Else

            Cella.Style.Item("border-right") = "none"
            Cella.Style.Item("border-top") = "none"
            Cella.Style.Item("border-left") = "none"
            Cella.Style.Item("border-bottom") = "none"

        End If


        'BACKGROUND CELLA
        If Sfondo_Colore <> "" Then
            Cella.Style.Item("background-color") = Sfondo_Colore
        End If

        'COLSPAN
        If Colspan <> 0 Then
            Cella.ColSpan = Colspan
        End If

        'ALLINEAMENTO ORIZZONTALE TESTO CELLA
        If Testo_Allineamento <> "" Then
            Cella.Style.Item("text-align") = Testo_Allineamento
        End If

        'ALLINEAMENTO VERTICALE TESTO
        Cella.Style.Item("vertical-align") = Testo_Allineamento_Verticale
        Cella.VAlign = Testo_Allineamento_Verticale

        'STILE TESTO CELLA
        If Font_Weigth <> "" Then
            Cella.Style.Item("font-weight") = Font_Weigth
        End If

        'SIZE TESTO CELLA
        If Font_Size <> "" Then
            Cella.Style.Item("font-size") = Font_Size
        End If

        'COLORE TESTO CELLA
        If Font_Color <> "" Then
            Cella.Style.Item("color") = Font_Color
        End If

        '-----------------------------------------
        '----- FORMATTAZIONE CONTENUTO CELLA -----
        '-----------------------------------------
        Select Case Tipo
            Case enum_FormattazioneExcel.FE_Text
                Cella.Style.Item("mso-number-format") = "\@" 'Text

            Case enum_FormattazioneExcel.FE_Short_Date
                Cella.Style.Item("mso-number-format") = "Short Date"

            Case enum_FormattazioneExcel.FE_Long_Date
                Cella.Style.Item("mso-number-format") = "d\-mmm\-yyyy"

            Case enum_FormattazioneExcel.FE_Number_NO_Decimal
                Cella.Style.Item("mso-number-format") = "0"

            Case enum_FormattazioneExcel.FE_Number_1_Decimal
                Cella.Style.Item("mso-number-format") = "\#\,\#\#0\.0"

            Case enum_FormattazioneExcel.FE_Number_2_Decimal
                Cella.Style.Item("mso-number-format") = "\#\,\#\#0\.00"

            Case enum_FormattazioneExcel.FE_Number_3_Decimal
                Cella.Style.Item("mso-number-format") = "\#\,\#\#0\.000"

            Case enum_FormattazioneExcel.FE_Percent_NO_Decimal
                Cella.Style.Item("mso-number-format") = "0%"

            Case enum_FormattazioneExcel.FE_Percent_2_Decimal
                Cella.Style.Item("mso-number-format") = "Percent"

            Case enum_FormattazioneExcel.FE_Short_Time
                Cella.Style.Item("mso-number-format") = "Short_Time"

            Case enum_FormattazioneExcel.FE_Medium_Time
                Cella.Style.Item("mso-number-format") = "Medium_Time"

            Case enum_FormattazioneExcel.FE_Long_Time
                Cella.Style.Item("mso-number-format") = "Long_Time"

        End Select

    End Sub



    Public Shared Function Sistema_ValiditaInizio(ByVal Validita_Inizio As Date) As String

        If Validita_Inizio = AGRODATAINIZIO Then
            Return "..."
        Else
            Return CStr(Validita_Inizio)
        End If

    End Function

    '#############################################################################
    Public Shared Function Sistema_ValiditaFine(ByVal Validita_Fine As Date) As String

        If Validita_Fine = AGRODATAFINE Then
            Return "..."
        Else
            Return CStr(Validita_Fine)
        End If

    End Function


    '##############################################################################################
    Public Shared Function Agro_vb_SingleQuote(ByVal Testo As String) As String

        'Se la stringa e' nulla, restituisco la stringa nulla
        If IsNothing(Testo) Then
            Testo = ""
            Return Testo
        End If

        'Sostituisco i singoli apici con due singoli apici
        Testo = Testo.Replace("'", "&quot;")

        'Restituisco il risultato
        Return Testo

    End Function

    Public Shared Function EsisteColonna(ByVal table As DataTable, ByVal nomeColonna As String) As Boolean

        If table Is Nothing Then Return False

        Return table.Columns.Contains(nomeColonna)

    End Function

    Private Shared Function ControllaPerInjection(ByRef identificatoreMetodo As Guid) As Boolean

        Dim inject As Boolean = False

        Dim st As New StackTrace
        Dim mi As MethodBase = st.GetFrame(2).GetMethod()
        If mi.IsDefined(GetType(DataProviderInjectParameterAttribute), False) Then
            Dim attr = DirectCast(mi.GetCustomAttributes(GetType(DataProviderInjectParameterAttribute), False).FirstOrDefault, DataProviderInjectParameterAttribute)
            inject = attr.UsaInjectionParametri
            If inject Then
                identificatoreMetodo = attr.IdentificatoreMetodo
            End If
        End If

        Return inject

    End Function

    Public Shared Function GetWrongPivaCharacters() As List(Of String)
        Return New List(Of String) From {" ", "|", "\", "!", """",
            "£", "$", "%", "&", "/", "(", ")", "=",
            "?", "", "^", "ì", "è", "è", "*", "+",
            "]", "[", "ç", "ò", "@", "°", "à", "#",
            "§", "ù", ",", ";", ".", ":", "-", "_", "<", ">"}
    End Function

    Public Shared Function RegExSoloNumeriECaratteri() As String
        Return "^[0-9a-zA-Z]+$"
    End Function

    Public Shared Function PivaValida(Piva As String) As Boolean

        Dim m = Regex.Match(Piva, RegExSoloNumeriECaratteri(), RegexOptions.IgnoreCase, TimeSpan.FromSeconds(3))

        Return m.Success

    End Function

    Public Shared Iterator Function ChunkBy(Of TSource)(ByVal source As IEnumerable(Of TSource), ByVal chunkSize As Integer) As IEnumerable(Of IEnumerable(Of TSource))
        While source.Any()
            Yield source.Take(chunkSize)
            source = source.Skip(chunkSize)
        End While
    End Function
End Class
