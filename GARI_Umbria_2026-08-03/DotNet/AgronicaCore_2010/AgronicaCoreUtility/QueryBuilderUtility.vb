Imports System.Text
Imports Newtonsoft.Json

Public Class QueryBuilderUtility

    ''' <summary>
    ''' Trasforma da stringa di chiavi serializzata come vettore jSonString es: ["piva,sa_cod,appezza", "..."] in una clausola sql server: (piva = '' and sa_Cod =  ... ) or  ...
    ''' </summary>
    ''' <param name="s">stringa di vettori json</param>
    ''' <param name="OrAndClauseChiavi">es: AND</param>
    ''' <param name="OrAndClauseComplessiva">es.: or</param>
    ''' <param name="separatoreFiltri">se passata allora si suppone che non si tratta di vettore json ma esiste un separatore fra chiavi</param>
    ''' <param name="separatoreValori">la virgola nell'esempio</param>
    ''' <param name="ListaCampiDBSeparatiDaVirgola">es: a.piva,a.sa_cod,a.appezza</param>
    ''' <param name="AggiungiApiceBitMask">indica in quali posizioni aggiungere gli apici, es: 00</param>
    ''' <param name="StartClause"></param>
    ''' <returns></returns>
    Public Shared Function EstraiFiltriSqlClauseDaStringaPipe(ByVal s As String,
                                                              OrAndClauseChiavi As String,
                                                              OrAndClauseComplessiva As String,
                                                              separatoreFiltri As String,
                                                              separatoreValori As String,
                                                              ByVal ListaCampiDBSeparatiDaVirgola As String,
                                                              ByVal AggiungiApiceBitMask As String,
                                                              Optional ByVal StartClause As String = " AND "
                                                              ) As String

        OrAndClauseChiavi = " " & OrAndClauseChiavi & " "
        OrAndClauseComplessiva = " " & OrAndClauseComplessiva & " "

        Dim vCampiDB As String() = ListaCampiDBSeparatiDaVirgola.Split(",")
        Dim risultato1 As New List(Of String)

        Dim listaDelleChiavi As List(Of String)

        If separatoreFiltri <> "" Then
            listaDelleChiavi = s.Split(separatoreFiltri).ToList()
        Else
            listaDelleChiavi = JsonConvert.DeserializeObject(Of List(Of String))(s)
        End If

        For Each singolachiave In listaDelleChiavi
            Dim v2 As String() = singolachiave.Split(separatoreValori)
            Dim idx1 As Integer = 0
            For Each campoDB In vCampiDB
                v2(idx1) = campoDB & " = " & AggiungiApice(v2(idx1), Convert.ToBoolean(CInt(AggiungiApiceBitMask(idx1).ToString())))
                idx1 += 1
            Next
            risultato1.Add("(" & String.Join(OrAndClauseChiavi, v2) & ")")
        Next

        Dim sRval As New StringBuilder
        If Not String.IsNullOrEmpty(StartClause) Then
            sRval.Append(StartClause)
        End If
        sRval.Append(" ( ")
        sRval.Append(String.Join(OrAndClauseComplessiva, risultato1))
        sRval.Append(" ) ")

        Return sRval.ToString

    End Function

    Private Shared Function AggiungiApice(s As String, bitFromMask As Boolean) As String

        If bitFromMask Then
            s = "'" & s & "'"
        End If

        Return s

    End Function

    Public Shared Function GeneraClausolaINDaList(ByVal List As List(Of Integer),
                                                  Optional ByVal aggiungiNull As Boolean = False) As String

        Dim stb As StringBuilder = New StringBuilder()

        Dim firstRow As Boolean = True

        stb.Length = 0

        stb.Append(" IN ( ")

        For Each value In List

            stb.Append(If(firstRow, " ", " , ") & value.ToString)

            firstRow = False

        Next

        If aggiungiNull Then

            stb.Append(If(List.Count > 0, " , ", "") & " Null ")

        End If

        stb.Append(" ) ")

        Return stb.ToString()

    End Function

    Public Shared Function GeneraClausolaINDaList(ByVal List As List(Of String),
                                                  Optional ByVal aggiungiNull As Boolean = False
                                                  ) As String

        Dim stb As StringBuilder = New StringBuilder()

        Dim firstRow As Boolean = True

        stb.Length = 0

        stb.Append(" ( ")

        For Each value In List

            stb.Append(If(firstRow, " '", " , '") & value.ToString & "' ")

            firstRow = False

        Next

        If aggiungiNull Then

            stb.Append(If(List.Count > 0, " , ", "") & " '' ")

        End If

        stb.Append(" ) ")

        Return stb.ToString()

    End Function


    Public Shared Function GeneraStringaInsertDaList(ByVal nomeTab As String, ByVal List As List(Of String)) As String

        Dim stb As StringBuilder = New StringBuilder()
        Dim cont As Integer = 0

        Dim firstRow As Boolean = True

        stb.Length = 0

        stb.Append(" INSERT INTO " & nomeTab & " values ")

        For Each value In List

            stb.Append(If(firstRow, " ( ", " ,( ") & value.ToString)

            firstRow = False

            stb.Append(" )")

            cont += 1

            If cont >= 999 Then
                firstRow = True
                cont = 0
                stb.Append(Environment.NewLine & " INSERT INTO " & nomeTab & " values ")

            End If

        Next

        stb.Append(";")

        Return stb.ToString().Replace("( (", "(").Replace(") )", ")")

    End Function

    Public Shared Function GeneraStringaInsertDaDT(ByVal nomeTab As String, ByVal dt As DataTable) As String

        Dim stb As StringBuilder = New StringBuilder()
        Dim cont As Integer = 0
        Dim numeroColonne As Integer = dt.Columns.Count

        Dim firstItem As Boolean = True
        Dim firstRow As Boolean = True

        stb.Length = 0

        stb.Append(" INSERT INTO " & nomeTab & " values ")

        For Each row As DataRow In dt.Rows

            stb.Append(If(firstRow, "(", ",("))

            firstRow = False
            firstItem = True

            For Each item In row.ItemArray

                stb.Append(If(firstItem, "", ",") & item.ToString & "")
                firstItem = False

            Next

            stb.Append(" )")

            cont += 1

            If cont >= 999 Then
                firstRow = True
                cont = 0
                stb.Append(Environment.NewLine & " INSERT INTO " & nomeTab & " values ")

            End If

        Next

        stb.Append(";")

        Return stb.ToString()

    End Function

End Class
