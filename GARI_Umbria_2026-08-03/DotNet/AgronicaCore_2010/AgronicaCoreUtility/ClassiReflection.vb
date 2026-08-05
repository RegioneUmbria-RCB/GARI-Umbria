Imports System.Reflection

Public Class ClassiReflection



    ''' <summary>
    ''' Imposta in automatico tutte le property che non sono state inizializzate
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="target"></param>
    ''' <param name="listaEsclusi">Lista di property (case sentitive, separata da virgola)</param>
    Public Shared Sub PropertyImpostaSeNothing(Of T)(ByVal target As T, listaEsclusi As String)

        Dim properties = target.GetType().GetProperties()
        Dim aListaEsclusi As List(Of String) = listaEsclusi.Split(",").ToList()

        For i As Integer = 0 To properties.Length - 1
            Dim propertyToGet As PropertyInfo = target.GetType().GetProperty(properties(i).Name)
            Dim property_value = propertyToGet.GetValue(target, Nothing)
            If Not listaEsclusi.Contains(properties(i).Name) AndAlso IsNothing(property_value) Then
                If propertyToGet.PropertyType.IsClass Then
                    property_value = Activator.CreateInstance(propertyToGet.PropertyType)
                    propertyToGet.SetValue(target, property_value, Nothing)
                End If
            End If
        Next


    End Sub

End Class
