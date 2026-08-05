Option Strict Off
Imports System.ComponentModel
Imports System.Dynamic
Imports System.Reflection
Imports System.Runtime.CompilerServices

Public Module ExpandoObjectExtension

    <Extension()>
    Sub [Set](ByVal obj As ExpandoObject, ByVal propertyName As String, ByVal value As Object)
        Dim dic As IDictionary(Of String, Object) = obj
        dic(propertyName) = value
    End Sub


    <Extension()>
    Function ToExpando(ByVal initialObj As List(Of ParametroReportLocalizzabile)) As ExpandoObject

        Dim obj As Object = New ExpandoObject()
        Dim dic As IDictionary(Of String, Object) = obj

        Dim attributesDic = New Dictionary(Of String, Attribute)


        For Each lp As ParametroReportLocalizzabile In initialObj
            Dim propKey = String.Empty
            If String.IsNullOrEmpty(lp.NomeSezione) Then
                propKey = lp.NomeParametro
            Else
                propKey = String.Format("{0}.{1}", lp.NomeParametro, lp.NomeSezione)
            End If

            dic.Add(propKey, lp.ValoreParametro)
            attributesDic.Add(propKey, New ExpandoObjectAttribute(lp.NomeReport, lp.NomeSezione, lp.Sottoreport))
        Next
        obj.AttributeDictionary = attributesDic

        TypeDescriptor.AddProvider(New ExpandoObjectTypeDescriptionProvider(), obj)

        Return obj

    End Function

    <Extension()>
    Function ToExpando(ByVal initialObj As Dictionary(Of String, Tuple(Of String, String, String, Boolean))) As ExpandoObject
        Dim obj As Object = New ExpandoObject()
        Dim dic As IDictionary(Of String, Object) = obj
        Dim tipo As Type = initialObj.[GetType]()

        Dim attributesDic = New Dictionary(Of String, Attribute)


        For Each kvp As KeyValuePair(Of String, Tuple(Of String, String, String, Boolean)) In initialObj
            Dim propKey = String.Empty
            If String.IsNullOrEmpty(kvp.Value.Item3) Then
                propKey = kvp.Key
            Else
                propKey = String.Format("{0}.{1}", kvp.Key, kvp.Value.Item3)
            End If

            dic.Add(propKey, kvp.Value.Item1)
            attributesDic.Add(propKey, New ExpandoObjectAttribute(kvp.Value.Item2, kvp.Value.Item3, kvp.Value.Item4))
        Next
        obj.AttributeDictionary = attributesDic

        TypeDescriptor.AddProvider(New ExpandoObjectTypeDescriptionProvider(), obj)

        Return obj
    End Function

    <Extension()>
    Function ToExpando(ByVal initialObj As Dictionary(Of String, Object)) As ExpandoObject
        Dim obj As Object = New ExpandoObject()
        Dim dic As IDictionary(Of String, Object) = obj
        Dim tipo As Type = initialObj.[GetType]()

        Dim attributesDic = New Dictionary(Of String, Attribute)


        For Each kvp As KeyValuePair(Of String, Object) In initialObj
            dic.Add(kvp.Key, kvp.Value)
        Next

        TypeDescriptor.AddProvider(New ExpandoObjectTypeDescriptionProvider(), obj)

        Return obj

    End Function
End Module



