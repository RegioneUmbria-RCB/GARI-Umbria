Imports System.Data
Imports System.Web.UI.WebControls

Public Class DatatableUtility

    ''' <summary>
    ''' '  Galassi, 27/06/2017 18.42.49: Inserito parametro ordinato come secondo param. Prima era di default true quindi per retro-compatibilità mettere a true.
    ''' </summary>
    ''' <param name="SourceTable">Elemento da cui fare il distinct</param>
    ''' <param name="ordinato">Boolean per scegliere se ordinare o meno secondo i campi inseriti</param>
    ''' <param name="FieldNames">Campi inseriti per la quale si fa il distinct</param>
    ''' <returns>DataTable con distinct eseguito</returns>
    ''' <remarks></remarks>
    Public Shared Function SelectDistinct_To_DT(ByVal SourceTable As DataTable,
                                                ByVal ordinato As Boolean,
                                                ByVal ParamArray FieldNames() As String
                                                ) As DataTable

        Dim lastValues As Object()
        Dim newTable As DataTable
        'Dim Keys(FieldNames.Length - 1) As DataColumn

        If FieldNames Is Nothing OrElse FieldNames.Length = 0 Then
            Throw New ArgumentNullException("FieldNames")
        End If

        lastValues = New Object(FieldNames.Length - 1) {}
        newTable = New DataTable

        For Each field As String In FieldNames
            newTable.Columns.Add(field, SourceTable.Columns(field).DataType)
            'Keys(newTable.Columns.Count - 1) = newTable.Columns(newTable.Columns.Count - 1)
        Next
        'newTable.PrimaryKey = Keys

        If ordinato Then
            For Each Row As DataRow In SourceTable.Select("", String.Join(", ", FieldNames))
                If Not FieldValuesAreEqual(lastValues, Row, FieldNames) Then
                    newTable.Rows.Add(CreateRowClone(Row, newTable.NewRow(), FieldNames))
                    SetLastValues(lastValues, Row, FieldNames)
                End If
            Next
        Else
            For Each Row As DataRow In SourceTable.Rows
                If Not PrevFieldValuesAreEqual(newTable, Row, FieldNames) Then
                    newTable.Rows.Add(CreateRowClone(Row, newTable.NewRow(), FieldNames))
                End If
            Next
        End If

        Return newTable
    End Function


    Private Shared Function FieldValuesAreEqual(ByVal lastValues() As Object,
                                                ByVal currentRow As DataRow,
                                                ByVal fieldNames() As String
                                                ) As Boolean

        Dim areEqual As Boolean = True

        For i As Integer = 0 To fieldNames.Length - 1
            If lastValues(i) Is Nothing OrElse Not lastValues(i).Equals(currentRow(fieldNames(i))) Then
                areEqual = False
                Exit For
            End If
        Next

        Return areEqual
    End Function

    Private Shared Function PrevFieldValuesAreEqual(ByVal AllPrevValues As DataTable,
                                                    ByVal currentRow As DataRow,
                                                    ByVal fieldNames() As String
                                                    ) As Boolean

        Dim filter As String = "1 = 1"
        For Each fi In fieldNames
            Select Case AllPrevValues.Columns(fi).DataType.Name.ToLowerInvariant()
                Case "string"
                    filter &= " AND " & fi.ToString & "='" & CStr(currentRow(fi.ToString)) & "'"
                Case Else
                    filter &= " AND " & fi.ToString & "=" & CStr(currentRow(fi.ToString))
            End Select
        Next

        If AllPrevValues Is Nothing OrElse AllPrevValues.Rows.Count <= 0 OrElse AllPrevValues.Select(filter).Length <= 0 Then
            Return False
        Else
            Return True
        End If

        'For i As Integer = 0 To fieldNames.Length - 1
        '    If lastValues(i) Is Nothing OrElse Not lastValues(i).Equals(currentRow(fieldNames(i))) Then
        '        areEqual = False
        '        Exit For
        '    End If
        'Next

        'Return areEqual
    End Function

    Private Shared Function CreateRowClone(ByVal sourceRow As DataRow,
                                           ByVal newRow As DataRow,
                                           ByVal fieldNames() As String
                                           ) As DataRow

        For Each field As String In fieldNames
            newRow(field) = sourceRow(field)
        Next

        Return newRow
    End Function

    Private Shared Sub SetLastValues(ByVal lastValues() As Object,
                                     ByVal sourceRow As DataRow,
                                     ByVal fieldNames() As String)

        For i As Integer = 0 To fieldNames.Length - 1
            lastValues(i) = sourceRow(fieldNames(i))
        Next
    End Sub

    ''' <summary>
    ''' Ritorna un datatable con un'unica colonna distinta...
    ''' </summary>
    ''' <param name="TableName"></param>
    ''' <param name="SourceTable"></param>
    ''' <param name="FieldName"></param>
    ''' <param name="blnRitornaSingolaColonna"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SelectDistinct(ByVal TableName As String, ByVal SourceTable As DataTable,
                                   ByVal FieldName As String, Optional ByVal blnRitornaSingolaColonna As Boolean = True
                                   ) As DataTable

        Dim dt As New DataTable(TableName) 'creo la nuova tabella da ritornare con nome impostato dal parametro
        'controllo se devo ritornare solo un campo.....
        If blnRitornaSingolaColonna Then
            'aggiungo il nuovo campo x il distinct prendendo il tipo corrispondente
            dt.Columns.Add(FieldName, SourceTable.Columns(FieldName).DataType)
        Else 'in questo caso li devo ritornare tutti clono la struttura della tabella
            dt = SourceTable.Clone()
        End If

        Dim LastValue As Object = Nothing
        Dim dr As DataRow

        'ciclo filtro vuoto ordinando x il nome campo richiesto x il distinct
        For Each dr In SourceTable.Select("", FieldName)
            'se l'ultimo valori è nothing prendo il valore...(succede solo la prima volta)
            If LastValue Is Nothing Then
                'assegno l'ultimo valore...
                LastValue = dr(FieldName)
                'anche qui se devo ritornare un campo solo aggiungo solo quello
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
                    'anche qui se devo ritornare un campo solo aggiungo solo quello
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



    'Public Function SelectDistinct(ByVal SourceTable As DataTable, ByVal FieldName As String) As String()

    '    'array da ritornare...
    '    Dim strRet() As String
    '    Dim LastValue As Object
    '    Dim dr As DataRow
    '    'indice x l'array...
    '    Dim i As Integer = 0

    '    'ciclo filtro vuoto ordinando x il nome campo richiesto x il distinct
    '    For Each dr In SourceTable.Select("", FieldName)

    '        If Not IsDBNull(LastValue) Then

    '            'se l'ultimo valori è nothing prendo il valore...(succede solo la prima volta)
    '            If LastValue Is Nothing Then
    '                'assegno l'ultimo valore...
    '                LastValue = dr(FieldName)
    '                'aggiungo la nuova riga....
    '                ReDim Preserve strRet(i)
    '                strRet(i) = LastValue.ToString 'assegno il valore del campo
    '                'incremento l'indice...
    '                i += 1
    '            Else

    '                If Not ColonneUguali(LastValue, dr(FieldName)) Then 'le volte successive controllo se sono uguali
    '                    'assegno l'ultimo valore...
    '                    LastValue = dr(FieldName)
    '                    'aggiungo la nuova riga....
    '                    ReDim Preserve strRet(i)
    '                    strRet(i) = LastValue.ToString 'assegno il valore del campo
    '                    'incremento l'indice...
    '                    i += 1
    '                End If

    '            End If 'fine controllo 

    '        End If 'fine controllo valore null

    '    Next

    '    Return strRet 'ritorno altrimenti solo la tabella

    'End Function

    'ritorna un array di stringhe...
    Public Function SelectDistinct(ByVal SourceTable As DataTable,
                                   ByVal FieldName As String,
                                   Optional ByVal Ordinato As Boolean = True
                                   ) As String()

        'array da ritornare...
        Dim strRet As String()
        Dim LastValue As Object = Nothing
        Dim dr As DataRow
        'indice x l'array...
        Dim i As Integer = 0

        If Ordinato Then

            'ciclo filtro vuoto ordinando x il nome campo richiesto x il distinct
            For Each dr In SourceTable.Select("", FieldName)

                'se l'ultimo valori è nothing prendo il valore...(succede solo la prima volta)
                If LastValue Is Nothing Then
                    'assegno l'ultimo valore...
                    LastValue = dr(FieldName)
                    'aggiungo la nuova riga....
                    ReDim Preserve strRet(i)
                    strRet(i) = LastValue.ToString 'assegno il valore del campo
                    'incremento l'indice...
                    i += 1
                Else

                    If Not ColonneUguali(LastValue, dr(FieldName)) Then 'le volte successive controllo se sono uguali
                        'assegno l'ultimo valore...
                        LastValue = dr(FieldName)
                        'aggiungo la nuova riga....
                        ReDim Preserve strRet(i)
                        strRet(i) = LastValue.ToString 'assegno il valore del campo
                        'incremento l'indice...
                        i += 1
                    End If

                End If

            Next

        Else

            'ciclo filtro vuoto ordinando x il nome campo richiesto x il distinct
            For Each dr In SourceTable.Select()

                'se l'ultimo valori è nothing prendo il valore...(succede solo la prima volta)
                If LastValue Is Nothing Then
                    'assegno l'ultimo valore...
                    LastValue = dr(FieldName)
                    'aggiungo la nuova riga....
                    ReDim Preserve strRet(i)
                    strRet(i) = LastValue.ToString 'assegno il valore del campo...
                    'incremento l'indice...
                    i += 1
                Else

                    If Not ColonneUguali(LastValue, dr(FieldName)) Then 'le volte successive controllo se sono uguali
                        'assegno l'ultimo valore...
                        LastValue = dr(FieldName)
                        'aggiungo la nuova riga....
                        ReDim Preserve strRet(i)
                        strRet(i) = LastValue.ToString 'assegno il valore del campo...
                        'incremento l'indice...
                        i += 1
                    End If

                End If

            Next

        End If

        Return strRet 'ritorno altrimenti solo la tabella

    End Function

    'a differenza della precedente considera uguale le stringhe con spazi
    Public Function SelectDistinctTrim(ByVal SourceTable As DataTable,
                                       ByVal FieldName As String,
                                       Optional ByVal Ordinato As Boolean = True
                                       ) As String()

        'array da ritornare...
        Dim strRet As String()
        Dim LastValue As Object = Nothing
        Dim dr As DataRow
        'indice x l'array...
        Dim i As Integer = 0

        If Ordinato Then

            'ciclo filtro vuoto ordinando x il nome campo richiesto x il distinct
            For Each dr In SourceTable.Select("", FieldName)

                'se l'ultimo valori è nothing prendo il valore...(succede solo la prima volta)
                If LastValue Is Nothing Then
                    'assegno l'ultimo valore...
                    LastValue = Trim(CStr(dr(FieldName)))
                    'aggiungo la nuova riga....
                    ReDim Preserve strRet(i)
                    strRet(i) = LastValue.ToString 'assegno il valore del campo...
                    'incremento l'indice...
                    i += 1
                Else

                    If Not ColonneUguali(LastValue, Trim(CStr(dr(FieldName)))) Then 'le volte successive controllo se sono uguali
                        'assegno l'ultimo valore...
                        LastValue = Trim(CStr(dr(FieldName)))
                        'aggiungo la nuova riga....
                        ReDim Preserve strRet(i)
                        strRet(i) = LastValue.ToString 'assegno il valore del campo...
                        'incremento l'indice...
                        i += 1
                    End If

                End If

            Next

        Else

            'ciclo filtro vuoto ordinando x il nome campo richiesto x il distinct
            For Each dr In SourceTable.Select()

                'se l'ultimo valori è nothing prendo il valore...(succede solo la prima volta)
                If LastValue Is Nothing Then
                    'assegno l'ultimo valore...
                    LastValue = Trim(CStr(dr(FieldName)))
                    'aggiungo la nuova riga....
                    ReDim Preserve strRet(i)
                    strRet(i) = LastValue.ToString 'assegno il valore del campo...
                    'incremento l'indice...
                    i += 1
                Else

                    If Not ColonneUguali(LastValue, Trim(CStr(dr(FieldName)))) Then 'le volte successive controllo se sono uguali
                        'assegno l'ultimo valore...
                        LastValue = Trim(CStr(dr(FieldName)))
                        'aggiungo la nuova riga....
                        ReDim Preserve strRet(i)
                        strRet(i) = LastValue.ToString 'assegno il valore del campo...
                        'incremento l'indice...
                        i += 1
                    End If

                End If

            Next

        End If

        Return strRet 'ritorno altrimenti solo la tabella

    End Function


    Public Function SelectDistinct(ByVal SourceTable As DataTable,
                                   ByVal FieldName1 As String,
                                   ByVal FieldName2 As String,
                                   Optional ByVal Ordinato As Boolean = True
                                   ) As String(,)

        Dim distinctTable As DataTable = SelectDistinct_To_DT(SourceTable, Ordinato, New String() {FieldName1, FieldName2})

        Dim strRet(distinctTable.Rows.Count - 1, 1) As String
        For i As Integer = 0 To distinctTable.Rows.Count - 1
            strRet(i, 0) = CStr(distinctTable.Rows(i).Item("Appezza"))
            strRet(i, 1) = CStr(distinctTable.Rows(i).Item("Id_Reg"))
        Next

        Return strRet

    End Function

    Public Function SelectDistincFromFieldList(ByVal SourceTable As DataTable,
                                               ByVal fieldList As List(Of String),
                                               Optional ByVal Ordinato As Boolean = True
                                               ) As String(,)

        Dim distinctTable As DataTable = SelectDistinct_To_DT(SourceTable, Ordinato, fieldList.ToArray())

        Dim strRet(distinctTable.Rows.Count - 1, fieldList.Count - 1) As String

        For Each row In distinctTable.Rows
            Dim row_index As Integer = distinctTable.Rows.IndexOf(row)
            For Each field In fieldList
                Dim field_index As Integer = fieldList.IndexOf(field)
                strRet(row_index, field_index) = CStr(distinctTable.Rows(row_index).Item(field))
            Next
        Next

        Return strRet
    End Function


    'ritorna un array di list items, prende xò in entrata un array di data row...
    Public Function SelectDistinct(ByVal drToFilter() As DataRow,
                                   ByVal FieldNameValue0_Text1() As String
                                   ) As ListItem()

        'controllo la lunghezza dell'array stringhe deve essere 2
        If FieldNameValue0_Text1.Length = 2 Then

            'array da ritornare...
            Dim lstItem As ListItem()
            Dim Value As Object = Nothing
            Dim Text As Object
            Dim dr As DataRow
            'indice x l'array...
            Dim i As Integer = 0

            'ciclo filtro vuoto ordinando x il nome campo richiesto x il distinct
            For Each dr In drToFilter

                'CONTROLLO CHE IL CODICE NON SIA 0, IN QUESTO CASO LO PONGO A -1
                'QUESTO XCHè LO 0 EQUIVALE A NOTHING NEGLI OGGETTI..
                If dr(FieldNameValue0_Text1(0)).ToString = "0" Then
                    dr(FieldNameValue0_Text1(0)) = -1
                End If

                'controllo che il valore non sia null
                If Not IsDBNull(Value) Then

                    'se l'ultimo valori è nothing prendo il valore...(succede solo la prima volta)
                    If Value Is Nothing Then
                        'assegno il value se è 0 lo pongo a -1 perché 0 assegnato a un obj corrisponde a nothing
                        'e mi fallirebbe il controllo sopra...
                        Value = dr(FieldNameValue0_Text1(0))
                        'trovo il corrispondente testo...
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
                            'trovo il corrispondente testo...
                            Text = dr(FieldNameValue0_Text1(1))
                            'aggiungo la nuova riga....
                            ReDim Preserve lstItem(i)
                            lstItem(i) = New ListItem(Text.ToString, Value.ToString)
                            'incremento l'indice...
                            i += 1
                        End If

                    End If
                    'fine controllo db null x valore
                End If

            Next

            Return lstItem 'ritorno l'array di listitem

        Else 'se la lunghezza dell'array non è uguale a 2

            Return Nothing

        End If

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
        If IsDBNull(A) AndAlso IsDBNull(B) Then
            Return True 'entrambi db null
        End If

        If IsDBNull(A) OrElse IsDBNull(B) Then
            Return False 'solo uno è db null
        End If

        Return A.Equals(B) 'confronta i 2 oggetti e ritorna un booleano x indicare se sono uguali o meno...

    End Function


    '###################################################################
    Public Sub DT_Svuota_CampiRipetuti(ByRef DT As DataTable,
                                       ByVal StrCampo1 As String,
                                       ByVal StrCampo2 As String)

        Dim i As Integer
        Dim Campo1, Campo2 As String
        Dim TEMP_Campo1 As String = ""
        Dim TEMP_Campo2 As String = ""

        'se ha solo una riga, non devo fare niente
        If DT.Rows.Count > 1 Then

            For i = 0 To DT.Rows.Count - 1

                Campo1 = CStr(DT.Rows(i).Item(StrCampo1))
                Campo2 = CStr(DT.Rows(i).Item(StrCampo2))

                If TEMP_Campo1 <> Campo1 Then
                    'prima riga analizzata
                    'o cambio valore del campo1
                    TEMP_Campo1 = Campo1
                    TEMP_Campo2 = Campo2
                    'la riga del dt rimane così com'è
                Else
                    'stesso valore di Campo1
                    ' lo svuoto
                    DT.Rows(i).Item(StrCampo1) = ""

                    If TEMP_Campo2 <> Campo2 Then
                        'cambio valore del campo2
                        TEMP_Campo2 = Campo2
                        'la riga del dt rimane così com'è
                    Else
                        'svuoto il campo2
                        DT.Rows(i).Item(StrCampo2) = ""
                    End If 'TEMP_Campo2 <> Campo2 

                End If 'TEMP_Campo1 <> Campo1

            Next 'righe dt

        End If 'riga 1


    End Sub

    '#######################################################################################################
    Public Function SelectDistinctWithHash_Base2(ByRef DT As DataTable,
                                                 ByVal vet_campi_chiave() As String,
                                                 Optional ByVal vet_campi_valore() As String = Nothing
                                                 ) As Hashtable

        ' prende in input un datatable DT passato byref (su cui viene fatto il distinct)
        ' e un vettore di stringhe vet_campi_chiave() contenenti i nomi delle colonne su cui fare il distinct 
        '(usati quindi per costruire la chiave)

        'la chiave può essere formata da un unico elemento (equivale al distinct su un campo)
        'se è necessario fare il distinct su più campi, si crea la chiave separando i campi da "|"

        'se viene passato il vettore vet_campi_valore() viene costruito il valore da attribuire alla chiave
        '(la costruzione avviene come per la chiave, separando i campi da "~")

        'restituisce una tabella hash con il distinct
        '(sul datatable c'è il distinct)

        'a differenza della SelectDistinctWithHash_Base, se la chiave è già presente, non viene fatto nulla

        Dim i, j As Integer
        Dim DT_Finale As DataTable
        Dim Dr_finale As DataRow
        Dim table_hash As New Hashtable

        Dim chiave As String = ""
        Dim valore As String = ""

        'copio la struttura
        DT_Finale = DT.Clone

        'scorro il DT di input
        For i = 0 To DT.Rows.Count - 1

            chiave = ""
            valore = ""

            'creo la stringa CHIAVE
            ''esempio
            'chiave = CStr(DT.Rows(i).Item("piva")) & "|" & CStr(DT.Rows(i).Item("sa_cod")) & "|" & CStr(DT.Rows(i).Item("appezza")) & "|" & CStr(DT.Rows(i).Item("id_reg"))
            For j = 0 To vet_campi_chiave.Length - 1
                If j = 0 Then
                    chiave = CStr(DT.Rows(i).Item(CStr(vet_campi_chiave(0))))
                Else
                    chiave &= "|" & CStr(DT.Rows(i).Item(CStr(vet_campi_chiave(j))))
                End If
            Next


            'creo la stringa VALORE
            If vet_campi_valore IsNot Nothing Then
                For j = 0 To vet_campi_valore.Length - 1
                    If j = 0 Then
                        valore = CStr(DT.Rows(i).Item(CStr(vet_campi_valore(j))))
                    Else
                        'separo i campi del valore con ~
                        'la pipe | la utilizzo per separare i valori (nel caso ce ne siano più di uno)
                        valore &= "~" & CStr(DT.Rows(i).Item(CStr(vet_campi_valore(j))))
                    End If
                Next
            End If


            Try

                If Not table_hash.ContainsKey(chiave) Then

                    table_hash.Add(chiave, valore)

                    'importo la riga nel DT finale
                    Dr_finale = DT_Finale.NewRow
                    Dr_finale = CopiaDatiRiga(Dr_finale, DT.Rows(i))
                    DT_Finale.Rows.Add(Dr_finale)

                End If

            Catch ae As ArgumentException

                'eccezione

            End Try

        Next

        DT = Nothing
        DT = DT_Finale.Copy

        Return table_hash

    End Function


    '#######################################################################################################
    Public Function SelectDistinctWithHash_Base(ByRef DT As DataTable,
                                                ByVal vet_campi_chiave() As String,
                                                Optional ByVal vet_campi_valore() As String = Nothing
                                                ) As Hashtable

        ' prende in input un datatable DT passato byref (su cui viene fatto il distinct)
        ' e un vettore di stringhe vet_campi_chiave() contenenti i nomi delle colonne su cui fare il distinct 
        '(usati quindi per costruire la chiave)

        'la chiave può essere formata da un unico elemento (equivale al distinct su un campo)
        'se è necessario fare il distinct su più campi, si crea la chiave separando i campi da "|"

        'se viene passato il vettore vet_campi_valore() viene costruito il valore da attribuire alla chiave
        '(la costruzione avviene come per la chiave, separando i campi da "~")

        'restituisce una tabella hash con il distinct
        '(sul datatable c'è il distinct)

        Dim i, j As Integer
        Dim DT_Finale As DataTable
        Dim Dr_finale As DataRow
        Dim table_hash As New Hashtable

        Dim chiave As String = ""
        Dim valore As String = ""
        Dim old_valore, new_valore As String

        'copio la struttura
        DT_Finale = DT.Clone

        'scorro il DT di input
        For i = 0 To DT.Rows.Count - 1

            chiave = ""
            valore = ""

            'creo la stringa CHIAVE
            ''esempio
            'chiave = CStr(DT.Rows(i).Item("piva")) & "|" & CStr(DT.Rows(i).Item("sa_cod")) & "|" & CStr(DT.Rows(i).Item("appezza")) & "|" & CStr(DT.Rows(i).Item("id_reg"))
            For j = 0 To vet_campi_chiave.Length - 1
                If j = 0 Then
                    chiave = CStr(DT.Rows(i).Item(CStr(vet_campi_chiave(0))))
                Else
                    chiave &= "|" & CStr(DT.Rows(i).Item(CStr(vet_campi_chiave(j))))
                End If
            Next


            'creo la stringa VALORE
            If vet_campi_valore IsNot Nothing Then
                For j = 0 To vet_campi_valore.Length - 1
                    If j = 0 Then
                        valore = CStr(DT.Rows(i).Item(CStr(vet_campi_valore(j))))
                    Else
                        'separo i campi del valore con ~
                        'la pipe | la utilizzo per separare i valori (nel caso ce ne siano più di uno)
                        valore &= "~" & CStr(DT.Rows(i).Item(CStr(vet_campi_valore(j))))
                    End If
                Next
            End If


            Try

                If Not table_hash.ContainsKey(chiave) Then

                    table_hash.Add(chiave, valore)

                    'importo la riga nel DT finale
                    Dr_finale = DT_Finale.NewRow
                    Dr_finale = CopiaDatiRiga(Dr_finale, DT.Rows(i))
                    DT_Finale.Rows.Add(Dr_finale)

                Else

                    'la chiave è già inserita, quindi non viene inserita la riga del datatable

                    'se ho passato il vettore con i nomi dei campi x il valore
                    If vet_campi_valore IsNot Nothing Then

                        'mi salvo il vecchio valore
                        old_valore = CStr(table_hash.Item(chiave))

                        new_valore = ""

                        'calcolo il nuovo
                        For j = 0 To vet_campi_valore.Length - 1
                            If j = 0 Then
                                new_valore = CStr(DT.Rows(i).Item(CStr(vet_campi_valore(j))))
                            Else
                                new_valore &= "~" & CStr(DT.Rows(i).Item(CStr(vet_campi_valore(j))))
                            End If
                        Next

                        'sostituisco il vecchio valore
                        'unendo il nuovo al vecchio (separo con |)
                        table_hash.Item(chiave) = old_valore & "|" & new_valore

                    End If

                End If


            Catch ae As ArgumentException

                'eccezione

            End Try

        Next

        DT = Nothing
        DT = DT_Finale.Copy

        Return table_hash

    End Function

End Class
