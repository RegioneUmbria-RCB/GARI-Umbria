Imports System.ComponentModel
Imports System.Dynamic
Imports CrystalDecisions.CrystalReports.Engine

Public Class ExpandoObjectToReportBinder

    Private _instance As ExpandoObject
    Private _report As ReportClass
    Private _dbTraduzioni As DataTable

    Public Sub New(ByVal obj As ExpandoObject, ByVal report As ReportClass, ByVal dbTraduzioni As DataTable)
        _instance = obj
        _report = report
        _dbTraduzioni = dbTraduzioni
    End Sub

    Public Sub FIllPropertiesFromDB()

        Dim dbSource = New DataBaseEntry().GetListFromDB(_dbTraduzioni)
        Dim SourceByTarget = (From record In dbSource
                              Group By TargetId = record.Target Into Targets = Group, Count())

        CheckBeforeFillFromDB(dbSource)

        Dim allTargets = TypeDescriptor.GetProperties(_instance)

        For Each target In SourceByTarget
            Dim targetObj = allTargets(target.TargetId)
            If Not targetObj Is Nothing Then
                If targetObj.PropertyType = GetType(ExpandoObject) Then

                    Dim targetProperties = TypeDescriptor.GetProperties(targetObj.GetValue(target))

                    For Each dbRecord As DataBaseEntry In target.Targets

                        Dim propKey = String.Empty
                        If String.IsNullOrEmpty(dbRecord.NomeSezione) Then
                            propKey = dbRecord.NomeProprieta
                        Else
                            propKey = String.Format("{0}.{1}", dbRecord.NomeProprieta, dbRecord.NomeSezione)
                        End If

                        targetProperties(propKey).SetValue(targetObj, dbRecord.ValoreProprieta)
                    Next

                End If
            End If
        Next

    End Sub

    Private Sub CheckBeforeFillFromDB(ByVal dbSource As List(Of DataBaseEntry))

        Dim SourceByTarget = (From record In dbSource
                              Group By TargetId = record.Target Into Targets = Group, Count())

        Dim allTargets = TypeDescriptor.GetProperties(_instance)

        For Each target In SourceByTarget
            Dim targetObj = allTargets(target.TargetId)
            If Not targetObj Is Nothing Then
                If targetObj.PropertyType = GetType(ExpandoObject) Then
                    Dim targetProperties = TypeDescriptor.GetProperties(targetObj.GetValue(target))
                    CheckTargetWithDB(targetObj.Name, targetProperties, target.Targets.ToList)
                End If
            Else
                Dim message As String = String.Format("I dati del DB non esistono come oggetto per il nodo < {0} >", target.TargetId)
                Throw New Exception(message)
            End If
        Next
    End Sub

    Private Sub CheckTargetWithDB(ByVal targetName As String, ByVal target As PropertyDescriptorCollection, ByVal dbSource As List(Of DataBaseEntry))

        Dim dbKeys = New List(Of String)
        Dim targetKeys = New List(Of String)
        For Each dbRecord As DataBaseEntry In dbSource

            Dim propKey = String.Empty
            If String.IsNullOrEmpty(dbRecord.NomeSezione) Then
                propKey = dbRecord.NomeProprieta
            Else
                propKey = String.Format("{0}.{1}", dbRecord.NomeProprieta, dbRecord.NomeSezione)
            End If
            dbKeys.Add(propKey)
        Next

        For Each tp As PropertyDescriptor In target
            If tp.Name <> "AttributeDictionary" Then
                targetKeys.Add(tp.Name)
            End If
        Next

        Dim areQuals As Boolean = New HashSet(Of String)(dbKeys).SetEquals(targetKeys)
        If Not areQuals Then

            Dim message As String = String.Format("I dati del DB non coincidono con le proprieta dell'oggetto < {0} >", targetName)
            Throw New Exception(message)

        End If

    End Sub

    Public Sub BindPropertiesToReport()

        RecursiveBindPropertiesToReport(_instance, Nothing)

    End Sub

    Private Sub RecursiveBindPropertiesToReport(ByVal obj As ExpandoObject, attributes As Dictionary(Of String, Attribute))

        Dim properties = TypeDescriptor.GetProperties(obj)
        For Each p As ExpandoObjectPropertyDescriptor In properties

            If p.Name <> "AttributeDictionary" Then

                If p.PropertyType = GetType(ExpandoObject) Then
                    Dim nextedObj = p.GetValue(obj)
                    Dim attrs = DirectCast(TypeDescriptor.GetProperties(nextedObj)("AttributeDictionary").GetValue(nextedObj),
                                        Dictionary(Of String, Attribute))

                    RecursiveBindPropertiesToReport(nextedObj, attrs)
                Else

                    Dim prop = p.Name
                    Dim value = p.GetValue(p)
                    Dim ca = DirectCast(attributes(prop), ExpandoObjectAttribute)

                    Dim reportName = ca.ReportName
                    Dim sectionName = ca.SectionName
                    Dim isSubreport = ca.IsSubreport

                    Dim parameterName = String.Format("{0}_{1}", "LP", p.Name)

                    Try
                        If Not isSubreport Then
                            _report.SetParameterValue(parameterName, value)
                        Else
                            _report.SetParameterValue(parameterName, value, reportName)

                        End If
                    Catch ex As Exception
                        Throw New Exception(parameterName)
                    End Try

                    Console.WriteLine(String.Format("{0} {1} {2} {3}", p.Name, value, reportName, parameterName))

                End If

            End If

        Next


    End Sub

    Private Sub RecursiveList(ByVal obj As ExpandoObject, attributes As Dictionary(Of String, Attribute))

        Dim properties = TypeDescriptor.GetProperties(obj)
        For Each p As ExpandoObjectPropertyDescriptor In properties

            If p.Name <> "AttributeDictionary" Then

                If p.PropertyType = GetType(ExpandoObject) Then
                    Dim nextedObj = p.GetValue(obj)
                    Dim attrs = DirectCast(TypeDescriptor.GetProperties(nextedObj)("AttributeDictionary").GetValue(nextedObj),
                                        Dictionary(Of String, Attribute))

                    RecursiveList(nextedObj, attrs)
                Else

                    Dim prop = p.Name
                    Dim value = p.GetValue(p)
                    Dim ca = DirectCast(attributes(prop), ExpandoObjectAttribute)

                    Dim reportName = ca.ReportName
                    Dim sectionName = ca.SectionName
                    Dim isSubreport = ca.IsSubreport

                    Dim parameterName = String.Empty
                    If String.IsNullOrEmpty(sectionName) Then
                        parameterName = String.Format("{0}_{1}", "LP", p.Name)
                    Else
                        parameterName = String.Format("{0}_{1}_{2}", "LP", p.Name.Split(".").FirstOrDefault, sectionName)
                    End If

                    Console.WriteLine(String.Format("{0} {1} {2} {3}", p.Name, value, reportName, parameterName))

                End If

            End If

        Next


    End Sub
    Public Sub ListAllProperties()
        RecursiveList(_instance, Nothing)
    End Sub

    Private Sub RecursiveParseObject(ByVal obj As ExpandoObject)

        Dim properties = TypeDescriptor.GetProperties(obj)
        For Each p As ExpandoObjectPropertyDescriptor In properties

            If p.PropertyType = GetType(ExpandoObject) Then
                Dim nextedObj = p.GetValue(obj)
                RecursiveParseObject(nextedObj)
            Else
                If p.Name <> "AttributeDictionary" Then
                    Console.WriteLine(p.Name)
                End If
            End If

        Next

    End Sub

End Class
