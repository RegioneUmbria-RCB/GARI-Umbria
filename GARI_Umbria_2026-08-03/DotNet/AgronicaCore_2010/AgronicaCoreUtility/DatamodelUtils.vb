Imports System.Reflection
Public Class DatamodelUtils
    Inherits AgronicaCoreDataProvider.DataProvider
    Implements IDisposable
    Private disposedValue As Boolean ' Per determinare chiamate rindondanti
    ''' <summary>
    ''' Popola le proprieta di un datamodel con i valori contenuti nella data table passata.
    ''' Se il nome della proprieta (che deve essere usuale al nome colonna della dt) è 
    ''' presente nella DT viene popolata
    ''' </summary>
    ''' <param name="dt"></param>
    ''' <param name="TypeOfDm"></param>
    ''' <returns></returns>
    Public Function loadDTonDataModel(ByRef dt As DataTable, TypeOfDm As Object) As List(Of Object)
        Dim dataModel As New List(Of Object)
        Dim colDT As DataColumnCollection = dt.Columns
        Dim TypeObj As Type = TypeOfDm.GetType()
        Dim fileds() As FieldInfo = TypeObj.GetFields(BindingFlags.NonPublic Or BindingFlags.Instance Or BindingFlags.Public)
        Dim fieldName As String = ""

        For Each dr As DataRow In dt.Rows
            Dim NewTypeObj = Activator.CreateInstance(TypeObj)
            For Each fieldInf As FieldInfo In fileds
                fieldName = fieldInf.Name
                If fieldName.StartsWith("_") Then fieldName = fieldName.Remove(0, 1)
                If colDT.Contains(fieldName) Then
                    Try
                        fieldInf.SetValue(NewTypeObj, getValueDr(fieldInf, dr(fieldName)))
                    Catch ex As Exception
                        Dim err As String = ex.Message
                    End Try
                End If
            Next

            dataModel.Add(NewTypeObj)
        Next

        Return dataModel
    End Function
    ''' <summary>
    ''' Forza una conversione in base al dato passato 
    ''' </summary>
    ''' <param name="fi"></param>
    ''' <param name="value"></param>
    ''' <returns></returns>
    Private Function getValueDr(fi As FieldInfo, value As Object) As Object
        Dim ret As Object = Nothing
        Dim TypeName As String = getTypeName(fi.FieldType.AssemblyQualifiedName.ToLower)
        If Not (value.Equals(DBNull.Value)) Then
            Select Case TypeName
                Case "string"
                    ret = Convert.ToString(value)
                Case "date", "datetime"
                    ret = Convert.ToDateTime(value)
                Case "boolean"
                    ret = Convert.ToBoolean(value)
                Case "int16", "int32", "integer"
                    ret = Convert.ToInt32(value)
                Case "decimal"
                    ret = Convert.ToDecimal(value)
                Case "double"
                    ret = Convert.ToDouble(value)
                Case "single"
                    ret = Convert.ToSingle(value)
                Case Else
                    ret = value
            End Select

        End If
        Return ret
    End Function
    ''' <summary>
    ''' Crea un Dictionary(Of String, String) dal modello passato e un oggetto per SQLinjection
    ''' I valori vengono tornati per referenza 
    ''' Torna vero o false se è riuscito
    ''' </summary>
    ''' <param name="objectModel"></param>
    ''' <param name="propertiesReturn">ByRef torna un dictionary per creare la stringa SQL</param>
    ''' <param name="parametriInjectionReturn">ByRef torna ula lista di oggetti SQLinjection</param>
    ''' <param name="Injection"></param>
    ''' <returns></returns>
    Public Function getPropertysOfDataModell(ByVal objectModel As Object, ByRef propertiesReturn As Object, ByRef parametriInjectionReturn As Object, Optional Injection As Boolean = True) As Boolean
        Dim dictRet As New Dictionary(Of String, String)
        Dim boolean2number As Integer = 0

        propertiesReturn = dictRet

        Try
            Dim TypeObj As Type = objectModel.GetType()
            Dim properties() As PropertyInfo = TypeObj.GetProperties()
            For Each propertyInf As PropertyInfo In properties
                Dim valore_formattato As Object = propertyInf.GetValue(objectModel, Nothing)
                Dim go As Boolean = True
                Dim FullName As String = propertyInf.PropertyType.FullName.ToLower
                Dim name As String = getTypeName(FullName)

                Select Case name
                    Case "string"
                        If Not IsNothing(valore_formattato) Then
                            valore_formattato = "'" + Agro_SQL_SaveText(valore_formattato, Injection) + "'"
                        Else
                            If IsNullableType(propertyInf) Then
                                valore_formattato = "'" + Agro_SQL_SaveText_NULL("NULL", Injection) + "'"
                            Else
                                valore_formattato = "'" + Agro_SQL_SaveText_NULL(valore_formattato, Injection) + "'"
                            End If
                        End If
                    Case "date"
                        If Not IsNothing(valore_formattato) Then
                            valore_formattato = Agro_SQL_SaveDate(valore_formattato, Injection)
                        Else
                            If IsNullableType(propertyInf) Then
                                valore_formattato = Agro_SQL_SaveDateTime_NULL("NULL", Injection)
                            Else
                                valore_formattato = Agro_SQL_SaveDateTime_NULL(valore_formattato, Injection)
                            End If
                        End If
                    Case "datetime"
                        If Not IsNothing(valore_formattato) Then
                            If valore_formattato = #1/1/0001# Then
                                ' -- equivale a NULL nel db
                                valore_formattato = Agro_SQL_SaveDateTime_NULL(DBNull.Value, Injection)
                            Else
                                valore_formattato = Agro_SQL_SaveDateTime(valore_formattato, Injection)
                            End If
                        Else
                            If IsNullableType(propertyInf) Then
                                valore_formattato = Agro_SQL_SaveDateTime_NULL(DBNull.Value, Injection)
                            Else
                                valore_formattato = Agro_SQL_SaveDateTime_NULL(valore_formattato, Injection)
                            End If
                        End If
                    Case "boolean"
                        If IsNothing(valore_formattato) Then
                            valore_formattato = False
                        End If
                        boolean2number = 0
                        If valore_formattato Then boolean2number = 1
                        valore_formattato = Agro_SQL_SaveNum(boolean2number, Injection)
                    Case Else
                        If IsNumeric(valore_formattato) Then
                            valore_formattato = Agro_SQL_SaveNum(valore_formattato, Injection)
                        Else
                            If IsNullableType(propertyInf) Then
                                valore_formattato = "NULL"
                            End If
                        End If
                End Select
                dictRet.Add(propertyInf.Name, valore_formattato)
            Next

        Catch ex As Exception

            Throw New Exception("[ AgronicaCoreUtility.DatamodelUtils.getPropertysOfDataModell() ] : " + ex.Message)

        End Try

        ' -- chiede i parametri creati e li assegna all'oggetto tornato per referenza
        parametriInjectionReturn = DammiParametriCollezionati()

        Return True
    End Function
    ''' <summary>
    ''' Torna il tipo letterale contenuto nel fullName
    ''' </summary>
    ''' <param name="FullName"></param>
    ''' <returns></returns>
    Private Function getTypeName(FullName As String) As String
        Dim ret As String = "number"

        Try
            If FullName.Contains("string") Then
                ret = "string"
                Return ret
            End If
            If FullName.Contains("datetime") Then
                ret = "datetime"
                Return ret
            End If
            If FullName.Contains("date") Then
                ret = "date"
                Return ret
            End If
            If FullName.Contains("boolean") Then
                ret = "boolean"
                Return ret
            End If
            If FullName.Contains("decimal") Then
                ret = "decimal"
                Return ret
            End If
            If FullName.Contains("double") Then
                ret = "double"
                Return ret
            End If
            If FullName.Contains("int32") Then
                ret = "int32"
                Return ret
            End If
            If FullName.Contains("int16") Then
                ret = "int16"
                Return ret
            End If
            If FullName.Contains("integer") Then
                ret = "int32"
                Return ret
            End If
            If FullName.Contains("single") Then
                ret = "single"
                Return ret
            End If
        Catch ex As Exception
            Throw New Exception("[ AgronicaCoreUtility.DatamodelUtils.getTypeName() ] : " + ex.Message)
            Return ""
        End Try

        Return ret

    End Function
    ''' <summary>
    ''' Torna booleano se il valore puo contenere valori NULL
    ''' </summary>
    ''' <param name="myType"></param>
    ''' <returns></returns>
    Private Function IsNullableType(ByVal myType As PropertyInfo) As Boolean
        Try
            If myType.PropertyType.FullName.ToLower.Contains("system.nullable") Then
                Return True
            End If
        Catch ex As Exception
            Throw New Exception("[ AgronicaCoreUtility.DatamodelUtils.getTyIsNullableTypepeName() ] : " + ex.Message)
            Return False
        End Try
        Return False
    End Function
    ''' <summary>
    ''' Crea sintassi update senza predicato WHERE 
    ''' </summary>
    ''' <param name="tabName"></param>
    ''' <param name="dic"></param>
    ''' <returns></returns>
    Public Function getSqlAggiorna(tabName As String, dic As Dictionary(Of String, String)) As String
        Dim sqlField As New System.Text.StringBuilder
        Dim sqlValue As New System.Text.StringBuilder
        Dim comma As String = ""
        ' -- primo ciclo campi e values  
        Try
            For Each kvp As KeyValuePair(Of String, String) In dic
                If sqlField.Length > 0 Then
                    comma = ","
                End If

                sqlField.Append($" {comma}{kvp.Key} = {kvp.Value} ")

            Next
        Catch ex As Exception
            Throw New Exception("[ AgronicaCoreUtility.DatamodelUtils.getSqlInsert() ] : " + ex.Message)
        End Try

        Return $"UPDATE {tabName}  SET {sqlField.ToString} "
    End Function
    ''' <summary>
    ''' Crea sintassi SQL insert dal Dictionary passato
    ''' </summary>
    ''' <param name="tabName">Nome tabella soggetto ad inserimento</param>
    ''' <param name="dic">Dictionary deè valori di inserimento</param>
    ''' <returns></returns>
    Public Function getSqlInserisci(tabName As String, dic As Dictionary(Of String, String)) As String
        Dim sqlField As New System.Text.StringBuilder
        Dim sqlValue As New System.Text.StringBuilder
        Dim comma As String = ""
        ' -- primo ciclo campi e values  
        Try
            For Each kvp As KeyValuePair(Of String, String) In dic
                If sqlField.Length > 0 Then
                    comma = ","
                End If

                sqlField.Append($" {comma}{kvp.Key}")
                sqlValue.Append($" {comma}{kvp.Value}")
            Next
        Catch ex As Exception
            Throw New Exception("[ AgronicaCoreUtility.DatamodelUtils.getSqlInsert() ] : " + ex.Message)
        End Try

        Return $"INSERT INTO {tabName} ( {sqlField.ToString} ) VALUES ( {sqlValue.ToString} )"
    End Function
    ''' <summary>
    ''' Dispose dell'oggetto 
    ''' </summary>
    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

    ' Interfaccia IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose degli oggetti gestiti
            End If

            ' TODO: Liberare gli oggetti non gestiti 
            ' TODO: Per gli oggetti molto gradi qui impostare a nothing 
        End If
        Me.disposedValue = True
    End Sub
End Class
