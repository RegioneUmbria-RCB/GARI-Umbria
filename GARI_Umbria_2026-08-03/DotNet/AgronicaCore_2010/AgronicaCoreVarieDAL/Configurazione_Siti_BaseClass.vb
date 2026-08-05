Imports AgronicaCoreDataProvider

Public MustInherit Class Configurazione_Siti_BaseClass

    Protected Function LeggiChiavi(ByVal filtroChiavi As String, ByRef objParametri As AgronicaCoreParametri) As DataTable
        Const nomeRoutine = "Configurazione_Siti_BaseClass.LeggiChiavi()"
        Dim dt As DataTable = Nothing

        Try
            Dim objR As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            dt = objR.Leggi(0, "", filtroChiavi, "", objParametri)
        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        End Try

        Return dt
    End Function

    Protected Function GetValoreChiave(ByRef dt As DataTable, ByVal chiave As String) As Object
        Const nomeRoutine = "Configurazione_Siti_BaseClass.GetValoreChiave()"
        Dim obj As Object = Nothing

        Try
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                Dim dr As DataRow = dt.Select("Chiave = '" & chiave & "'").FirstOrDefault()
                If dr Is Nothing Then
                    Throw New Exception(String.Format("Chiave {0} non presente in Configurazione_Siti", chiave))
                End If
                obj = If (Not IsDBNull(dr.Item("Valore")), dr.Item("Valore"), Nothing)
            End If
        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        End Try

        Return obj
    End Function

    Protected Function GetValoreChiaveNoException(ByRef dt As DataTable, ByVal chiave As String) As Object
        Const nomeRoutine = "Configurazione_Siti_BaseClass.GetValoreChiave()"
        Dim obj As Object = Nothing

        Try
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                Dim dr As DataRow = dt.Select("Chiave = '" & chiave & "'").FirstOrDefault()
                If dr IsNot Nothing Then obj = If(Not IsDBNull(dr.Item("Valore")), dr.Item("Valore"), Nothing)
            End If
        Catch ex As Exception
            Throw New Exception("[ " & nomeRoutine & " ] : " & ex.Message)
        End Try

        Return obj
    End Function

End Class
