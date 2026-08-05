Imports System.Reflection
Imports System.Data.Entity.Core
Imports System.Data.Entity.Core.EntityClient
Imports System.Data.Entity.Core.Objects

Public Class Gias_EF_Utility

    Public Function ObjectQueryToDataTable(Of T)(objlist As IEnumerable(Of T)) As DataTable

        Dim dtReturn As New DataTable()

        Dim objProps As PropertyInfo() = Nothing

        If objlist Is Nothing Then
            Return dtReturn
        End If

        For Each objRec As T In objlist
            If objProps Is Nothing Then
                objProps = DirectCast(objRec.[GetType](), Type).GetProperties()
                For Each objpi As PropertyInfo In objProps
                    Dim colType As Type = objpi.PropertyType

                    If (colType.IsGenericType) AndAlso (colType.GetGenericTypeDefinition() = GetType(Nullable(Of ))) Then
                        colType = colType.GetGenericArguments()(0)
                    End If

                    dtReturn.Columns.Add(New DataColumn(objpi.Name, colType))
                Next
            End If

            Dim dr As DataRow = dtReturn.NewRow()

            For Each pi As PropertyInfo In objProps
                dr(pi.Name) = If(pi.GetValue(objRec, Nothing) Is Nothing, DBNull.Value, pi.GetValue(objRec, Nothing))
            Next

            dtReturn.Rows.Add(dr)
        Next

        Return dtReturn

    End Function

    ' Restituisce stringa di connessione in formato EntityFramework
    Public Function GetEntityConnectionString(objParametriStringaConnessione As String, Optional ByVal xmodel As String = "Gias_DeveloperServer_Model") As String

        Dim scsb As New SqlClient.SqlConnectionStringBuilder(OleDB_Converti_SqlClient(objParametriStringaConnessione))
        scsb.IntegratedSecurity = False

        Dim ecb As New EntityConnectionStringBuilder With {
            .Metadata = "res://*/" & xmodel & ".csdl|" &
                        "res://*/" & xmodel & ".ssdl|" &
                        "res://*/" & xmodel & ".msl",
            .Provider = "System.Data.SqlClient",
            .ProviderConnectionString = scsb.ConnectionString
        }

        Return ecb.ConnectionString

    End Function


    ''' <summary>
    ''' conversione da string Ole db a stringa SqlClient
    ''' 
    ''' N.B.  Copiato da AgronicaCoreDataProvider: non posso utilizzare quello perchè nel prj
    '''       data provider c'è necessità di avere il riferimento al prj EF e non compila per riferimento 
    '''       circolare
    ''' </summary>
    ''' <param name="oleDB_connnectionString"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function OleDB_Converti_SqlClient(ByVal oleDB_connnectionString As String) As String
        Dim vAppoggio As String() = oleDB_connnectionString.Split(New Char() {";"c, "="c})

        Dim rval As String = ""

        For i As Integer = 0 To vAppoggio.Length - 2 Step 2
            Select Case vAppoggio(i).ToLower
                Case "provider"
                Case Else
                    rval &= vAppoggio(i) & "= " & vAppoggio(i + 1) & ";"
            End Select
        Next

        Return rval & "multipleactiveresultsets=True;App=EntityFramework"

    End Function

    Public Shared Function IsSimpleType(ByVal type As Type) As Boolean
        Return type.IsValueType OrElse type.IsPrimitive OrElse New Type() {GetType(String), GetType(Decimal), GetType(DateTime), GetType(DateTimeOffset), GetType(TimeSpan), GetType(Guid)}.Contains(type) OrElse Convert.GetTypeCode(type) <> TypeCode.Object
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="ctx"></param>
    ''' <param name="entity_from"></param>
    ''' <param name="entity_to"></param>
    ''' <param name="username">passare stringa vuota se non si vuole modificare il dato</param>
    ''' <param name="data">passare agroDataInizio se non si vuole modificare il dato</param>
    ''' <returns></returns>
    Public Shared Function CopyEntity(Of T As Class)(ByRef ctx As Gias_DeveloperServer_Entities, ByRef entity_from As T, ByRef entity_to As T, ByVal username As String, ByVal data As Date) As T
        
        '11/02/2021: questa riga andava bene per l'ObjectContext, ma non per il DbContext
        'Dim entity As T = If(entity_to Is Nothing, ctx.CreateObject(Of T)(), entity_to)
        Dim entity As T = If(entity_to Is Nothing, ctx.Set(Of T)().Create(), entity_to)

        Dim propInfo = entity_from.GetType().GetProperties() '.Where(Function(p) p.CanRead AndAlso p.CanWrite)
        For Each item In propInfo
            'If (item.Name = "Username_Creazione" OrElse item.Name = "Username_Modifica") AndAlso username <> "" Then
            '    entity.GetType().GetProperty(item.Name).SetValue(entity, username, Nothing)
            'ElseIf (item.Name = "Data_Creazione" OrElse item.Name = "Data_Modifica") AndAlso data <> #01/01/1900# Then
            '    entity.GetType().GetProperty(item.Name).SetValue(entity, data, Nothing)
            'ElseIf IsSimpleType(item.PropertyType) Then
            '    entity.GetType().GetProperty(item.Name).SetValue(entity, item.GetVax.ue(entity_from, Nothing), Nothing)
            'End If
            If IsSimpleType(item.PropertyType) Then
                entity.GetType().GetProperty(item.Name).SetValue(entity, item.GetValue(entity_from, Nothing), Nothing)
            End If
        Next
        Return entity
    End Function

    ''' <summary>
    ''' Copia oggetto EF
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="ctx">contesto EF</param>
    ''' <param name="entity_from">oggetto origine</param>
    ''' <param name="entity_to">oggetto destinazione, se non passato lo crea nuovo</param>
    ''' <returns>restituisce la copia dell'oggetto</returns>
    Public Shared Function CopyEntity(Of T As Class)(ByRef ctx As Gias_DeveloperServer_Entities, ByRef entity_from As T, ByRef entity_to As T) As T

        '11/02/2021: questa riga andava bene per l'ObjectContext, ma non per il DbContext
        'Dim entity As T = If(entity_to Is Nothing, ctx.CreateObject(Of T)(), entity_to)
        Dim entity As T = If(entity_to Is Nothing, ctx.Set(Of T)().Create(), entity_to)

        Dim propInfo = entity_from.GetType().GetProperties()
        For Each item In propInfo
            If IsSimpleType(item.PropertyType) Then
                entity.GetType().GetProperty(item.Name).SetValue(entity, item.GetValue(entity_from, Nothing), Nothing)
            End If
        Next
        Return entity
    End Function



    ''' <summary>
    ''' Creazione di una nuova connessione EntityFramework a partire da una string di connessione
    ''' </summary>
    ''' <param name="StringaConnessione"></param>
    ''' <returns>
    '''     new Gias_DeveloperServer_Entities
    ''' </returns>
    Public Shared Function CreateGiasContextConnection(ByVal StringaConnessione As String) As Gias_DeveloperServer_Entities

        Try
            Dim EFConnString As String = New Gias_EF_Utility().GetEntityConnectionString(StringaConnessione)
            Return New Gias_DeveloperServer_Entities(EFConnString)
        Catch ex As Exception
            Return Nothing
        End Try
        Dim gefutils As New Gias_EF_Utility

    End Function

End Class
