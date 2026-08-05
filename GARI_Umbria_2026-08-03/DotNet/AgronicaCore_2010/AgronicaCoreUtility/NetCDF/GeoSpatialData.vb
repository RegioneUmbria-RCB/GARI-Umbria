

Imports System.IO

Namespace NetCDF

    Public Class GeoSpatialData
        Implements IDisposable

        Private ReadOnly Dimensions As List(Of Dimension)
        Private ReadOnly Attributes As Dictionary(Of String, AttributeValue)
        Private ReadOnly Variables As List(Of Variable)

        Private disposedValue As Boolean

        Public Sub New(s As Stream)

            Dim cdfStream As New NetCDFStream(s)

            'numrecs
            Dim num_recs As UInteger = cdfStream.ReadUInt32()
            If num_recs = &HFFFFFFFF Then
                ' streaming
            End If



            Dimensions = New List(Of Dimension)

            Dim dimension_tag As Integer = cdfStream.ReadInt32() ' should be 0xA
            Dim num_dims As UInteger = cdfStream.ReadUInt32()

            If dimension_tag = &HA AndAlso num_dims > 0 Then

                For i As UInteger = 1 To num_dims

                    Dimensions.Add(New Dimension(cdfStream))
                Next
            End If



            Attributes = AttributeFactory.ReadAttributes(cdfStream)



            Variables = New List(Of Variable)

            Dim variable_tag As Integer = cdfStream.ReadInt32()  ' should be 0xB
            Dim num_vars As UInteger = cdfStream.ReadUInt32()

            If variable_tag = &HB AndAlso num_vars > 0 Then

                For i As UInteger = 1 To num_vars

                    Variables.Add(New Variable(cdfStream, Dimensions))
                Next
            End If



            cdfStream.Dispose()

        End Sub

        Public Function GetVariable(names As String()) As Variable

            Dim iVar As Integer = 0
            Dim retVar As Variable = Nothing
            Dim iName As Integer

            While retVar Is Nothing AndAlso iVar < Variables.Count

                iName = 0
                While retVar Is Nothing AndAlso iName < names.Length

                    If Variables(iVar).name.Equals(names(iName)) Then

                        retVar = Variables(iVar)
                    End If

                    iName += 1
                End While

                iVar += 1
            End While

            Return retVar
        End Function


        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: eliminare lo stato gestito (oggetti gestiti)

                    Dimensions.Clear()
                    Attributes.Clear()
                    Variables.Clear()

                End If

                ' TODO: liberare risorse non gestite (oggetti non gestiti) ed eseguire l'override del finalizzatore
                ' TODO: impostare campi di grandi dimensioni su Null
                disposedValue = True
            End If
        End Sub

        ' ' TODO: eseguire l'override del finalizzatore solo se 'Dispose(disposing As Boolean)' contiene codice per liberare risorse non gestite
        ' Protected Overrides Sub Finalize()
        '     ' Non modificare questo codice. Inserire il codice di pulizia nel metodo 'Dispose(disposing As Boolean)'
        '     Dispose(disposing:=False)
        '     MyBase.Finalize()
        ' End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' Non modificare questo codice. Inserire il codice di pulizia nel metodo 'Dispose(disposing As Boolean)'
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub

    End Class

End Namespace
