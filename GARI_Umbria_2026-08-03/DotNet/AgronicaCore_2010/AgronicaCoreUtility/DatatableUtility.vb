Imports System.Data
Imports System.Web.UI.WebControls




'#############################################################################################################################
'#############################################################################################################################
'#############################################################################################################################
'#############################################################################################################################
'#############################################################################################################################
'#############################################################################################################################


' QUESTO CORE NON E' DA USARE!!!!
'
' usare AgronicaCoreDataProvider.DatatableUtility.vb





'#############################################################################################################################
'#############################################################################################################################
'#############################################################################################################################
'#############################################################################################################################
'#############################################################################################################################
'#############################################################################################################################
'#############################################################################################################################
'#############################################################################################################################
'#############################################################################################################################
'#############################################################################################################################



































Public Class DatatableUtility


    ''' <summary>
    ''' ritorna un datatable con un'unica colonna distinta...
    ''' </summary>
    ''' <param name="TableName"></param>
    ''' <param name="SourceTable"></param>
    ''' <param name="FieldName"></param>
    ''' <param name="blnRitornaSingolaColonna"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SelectDistinct(ByVal TableName As String, ByVal SourceTable As DataTable, _
                ByVal FieldName As String, Optional ByVal blnRitornaSingolaColonna As Boolean = True) As DataTable

        Dim dt As New DataTable(TableName) 'creo la nuova tabella da ritornare con nome impostato dal parametro
        'controllo se devo ritornare solo un campo.....
        If blnRitornaSingolaColonna Then
            'aggiungo il nuovo campo x il distinct prendendo il tipo corrispondente..
            dt.Columns.Add(FieldName, SourceTable.Columns(FieldName).DataType)
        Else 'in questo caso li devo ritornare tutti clono la struttura della tabella
            dt = SourceTable.Clone()
        End If

        Dim LastValue As Object
        Dim dr As DataRow

        If Not blnRitornaSingolaColonna Then
            Dim DTDistinct = SourceTable.DefaultView.ToTable(True, FieldName)

            For Each rowDistinct In DTDistinct.Rows
                Dim dr0 = SourceTable.Select(FieldName & " = " & rowDistinct(FieldName))
                dt.ImportRow(dr0(0))
            Next
            Return dt

        End If


        'ciclo filtro vuoto ordinando x il nome campo richiesto x il distinct
        For Each dr In SourceTable.Select("", FieldName)
            'se l'ultimo valori è nothing prendo il valore....(succede solo la prima volta)
            If LastValue Is Nothing Then
                'assegno l'ultimo valore...
                LastValue = dr(FieldName)
                'anche qui se devo ritornare un campo solo aggiungo solo quello..
                If blnRitornaSingolaColonna Then
                    'aggiungo la nuova riga....
                    dt.Rows.Add(New Object() {LastValue})
                Else 'aggiungo tutta la riga..
                    Dim drN As DataRow = dt.NewRow
                    dt.Rows.Add(CopiaDatiRiga(drN, dr))
                End If
            Else
                If Not ColonneUguali(LastValue, dr(FieldName)) Then 'le volte successive controllo se sono uguali
                    'assegno l'ultimo valore...
                    LastValue = dr(FieldName)
                    'anche qui se devo ritornare un campo solo aggiungo solo quello..
                    If blnRitornaSingolaColonna Then
                        'aggiungo la nuova riga....
                        dt.Rows.Add(New Object() {LastValue})
                    Else 'aggiungo tutta la riga..
                        Dim drN As DataRow = dt.NewRow
                        dt.Rows.Add(CopiaDatiRiga(drN, dr))
                    End If
                End If
            End If

        Next

        Return dt 'ritorno altrimenti solo la tabella

    End Function



    Public Function SelectDistinct(ByVal SourceTable As DataTable, ByVal FieldName As String) As String()

        'array da ritornare...
        Dim strRet() As String
        Dim LastValue As Object
        Dim dr As DataRow
        'indice x l'array...
        Dim i As Integer = 0

        'ciclo filtro vuoto ordinando x il nome campo richiesto x il distinct
        For Each dr In SourceTable.Select("", FieldName)

            If Not IsDBNull(LastValue) Then

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

                End If 'fine controllo 

            End If 'fine controllo valore null

        Next

        Return strRet 'ritorno altrimenti solo la tabella

    End Function



    Public Function CopiaDatiRiga(ByVal drNuova As DataRow, ByVal drOrigine As DataRow) As DataRow
        'ciclo su tutti le colonne
        Dim Dc As DataColumn
        For Each Dc In drOrigine.Table.Columns
            'copio i dati
            drNuova.Item(Dc.ColumnName) = drOrigine.Item(Dc)
        Next
        Return drNuova
    End Function

    Private Function ColonneUguali(ByVal A As Object, ByVal B As Object) As Boolean

        'confronta 2 valori x vedere se sono uguali, confronta anche il dbnull 
        If IsDBNull(A) And IsDBNull(B) Then
            Return True 'entrambi db null
        End If

        If IsDBNull(A) Or IsDBNull(B) Then
            Return False 'solo uno è db null
        End If

        Return A.Equals(B) 'confronta i 2 oggetti e ritorna un booleano x indicare se sono uguali o meno..

    End Function



    'ritorna un array di stringhe...
    Public Function SelectDistinct(ByVal RS() As DataRow, ByVal FieldNameValue0_Text1() As String) _
                                                                                                    As ListItem()

        'controllo la lunghezza dell'array stringhe deve essere 2 e nel rs ci deve essere qualcosa..
        If FieldNameValue0_Text1.Length = 2 AndAlso RS.Length <> 0 Then

            'trasformo il rs in un datatable...
            Dim SourceTable As New DataTable, OleDap As New OleDb.OleDbDataAdapter
            'aggiungo le colonne x ospitare testo e valore...
            SourceTable.Columns.Add(FieldNameValue0_Text1(0))
            SourceTable.Columns.Add(FieldNameValue0_Text1(1))
            'prendo solo la colonna che mi interessa
            OleDap.MissingSchemaAction = MissingSchemaAction.Ignore
            OleDap.Fill(SourceTable, RS)

            'array da ritornare...
            Dim lstItem() As ListItem
            Dim Value, Text As Object
            Dim dr As DataRow
            'indice x l'array...
            Dim i As Integer = 0

            'ciclo filtro vuoto ordinando x il nome campo richiesto x il distinct
            For Each dr In SourceTable.Select("", FieldNameValue0_Text1(0)) 'ordino x il valore

                'se l'ultimo valori è nothing prendo il valore....(succede solo la prima volta)
                If Value = Nothing Then
                    'assegno il value....
                    Value = dr(FieldNameValue0_Text1(0))
                    'trovo il corrispondente testo..
                    Text = dr(FieldNameValue0_Text1(1))
                    'aggiungo la nuova riga....
                    ReDim Preserve lstItem(i)
                    lstItem(i) = New ListItem(Text.ToString, Value.ToString)
                    'incremento l'indice...
                    i += 1
                Else

                    If Not ColonneUguali(Value, dr(FieldNameValue0_Text1(0))) Then 'le volte successive controllo se sono uguali
                        'assegno il value....
                        Value = dr(FieldNameValue0_Text1(0))
                        'trovo il corrispondente testo..
                        Text = dr(FieldNameValue0_Text1(1))
                        'aggiungo la nuova riga....
                        ReDim Preserve lstItem(i)
                        lstItem(i) = New ListItem(Text.ToString, Value.ToString)
                        'incremento l'indice...
                        i += 1
                    End If

                End If

            Next

            Return lstItem 'ritorno l'array di listitem

        Else 'se la lunghezza dell'array non è uguale a 2

            Return Nothing

        End If

    End Function
End Class
