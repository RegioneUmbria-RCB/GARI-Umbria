Imports AgronicaCoreDataProvider
Imports System.Text

Public Class ClassiTessitura
    Public Function ClasseTessituraDes_from_ClasseTessituraCod(Id_ClasseTessitura As Integer,
                                                               Regolamento_Cod As Integer,
                                                               ByRef objParametri As AgronicaCoreParametri
                                                               ) As String


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaBIZ.ClassiTessitura.ClasseTessituraDes_from_ClasseTessituraCod()"

        Dim descrizione As String = ""
        Dim DT As New DataTable

        If Id_ClasseTessitura <> 0 Then
            Dim objClasseTessitura As New AgronicaCoreMetaSchemaDAL.ClassiTessituraB_R
            DT = objClasseTessitura.LeggiDistinct(Id_ClasseTessitura, Regolamento_Cod, "", "", objParametri)
            If DT.Rows.Count > 0 Then
                descrizione = DT.Rows(0).Item("Descrizione")
            End If
        End If

        Return descrizione

    End Function

End Class
