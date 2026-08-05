Public Class PUA_ClassiTessitura

    Public Function ClassiTessitura(ByVal Input As PUA_ClassiTessitura_input,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As PUA_ClassiTessitura_output

        Dim Output As New PUA_ClassiTessitura_output
        Dim ClasseTessitura As PUA_ClasseTessitura

        Try

            Dim objCore As New AgronicaCoreMetaSchemaDAL.ClassiTessituraB_R
            Dim dt As DataTable = objCore.LeggiDistinct(Input.id_classetessitura, Input.Regolamento_Cod,
                                                        "", "", objParametri)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then

                For Each row In dt.Rows

                    ClasseTessitura = New PUA_ClasseTessitura With {
                        .Tessitura_Cod = row.Item("id_classetessitura"),
                        .Tessitura_Des = row.Item("descrizione"),
                        .minimo = row.Item("minimo"),
                        .pesospecifico = row.Item("pesospecifico"),
                        .peso20 = row.Item("peso20"),
                        .peso30 = row.Item("peso30"),
                        .peso50 = row.Item("peso50")
                    }

                    Output.ListaClassiTessitura.Add(ClasseTessitura)

                Next

            End If

        Catch ex As Exception
            Output.MessaggioErrore = ex.Message
        End Try

        Return Output

    End Function

    Public Function ClassiTessituraDaAnalisi(ByVal Input As PUA_ClassiTessitura_Analisi_input,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                             ) As PUA_ClassiTessitura_Analisi_output

        Dim Output As New PUA_ClassiTessitura_Analisi_output
        Dim ClasseTessitura As PUA_ClasseTessitura_Analisi

        Try

            Dim objCore As New AgronicaCoreMetaSchemaDAL.PC_Tessiture_R
            Dim dt As DataTable = objCore.Leggi_Id_ClasseTessitura_Da_SabbiaArgilla(Input.Sabbia,
                                                                                    Input.Argilla,
                                                                                    objParametri,
                                                                                    Input.verbose)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then

                For Each row In dt.Rows

                    ClasseTessitura = New PUA_ClasseTessitura_Analisi With {
                        .Id_ClasseTessitura = row.Item("Id_ClasseTessitura"),
                        .Sabbia = row.Item("Sabbia"),
                        .Argilla = row.Item("argilla"),
                        .Descrizione = If(Input.verbose, row.Item("Descrizione"), "")
                    }

                    Output.ListaClassiTessitura.Add(ClasseTessitura)

                Next

            End If

        Catch ex As Exception
            Output.MessaggioErrore = ex.Message
        End Try

        Return Output

    End Function

    Public Function ClassiTessituraDaListaAnalisi(ByVal Input As List(Of AgronicaCorePianoConcimazioneBIZ.PUA_ClassiTessitura_Analisi_input),
                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                  ) As PUA_ClassiTessitura_Analisi_output

        Dim Output As New PUA_ClassiTessitura_Analisi_output
        Dim ClasseTessitura As PUA_ClasseTessitura_Analisi
        Dim dt As New DataTable
        Try

            Dim listaParams As New List(Of String)
            For Each i In Input
                listaParams.Add("(SABBIA = " & CStr(i.Sabbia) & " AND ARGILLA = " & CStr(i.Argilla) & ")")
            Next

            If listaParams.Count > 0 Then
                Dim xFiltroAggiuntivo As String = "(" & String.Join(" OR ", listaParams.Distinct.ToList()) & ")"


                Dim objCore As New AgronicaCoreMetaSchemaDAL.PC_Tessiture_R
                dt = objCore.Leggi_Id_ClasseTessitura_Da_SabbiaArgilla(-99, -99,
                                                                       objParametri,
                                                                       xFiltroAggiuntivo:=xFiltroAggiuntivo)
            End If

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then

                For Each row In dt.Rows

                    ClasseTessitura = New PUA_ClasseTessitura_Analisi With {
                        .Id_ClasseTessitura = row.Item("Id_ClasseTessitura"),
                        .Sabbia = row.Item("Sabbia"),
                        .Argilla = row.Item("argilla"),
                        .Descrizione = ""
                    }

                    Output.ListaClassiTessitura.Add(ClasseTessitura)
                Next

            End If

        Catch ex As Exception
            Output.MessaggioErrore = ex.Message
        End Try

        Return Output

    End Function

End Class

Public Class PUA_ClassiTessitura_input

    Public Url As String

    Public Regolamento_Cod As Integer
    Public id_classetessitura As Integer

    Public Sub New()
        Url = ""
        Regolamento_Cod = 0
        id_classetessitura = 0
    End Sub

End Class

Public Class PUA_ClassiTessitura_output

    Public ListaClassiTessitura As List(Of PUA_ClasseTessitura)
    Public MessaggioErrore As String

    Public Sub New()
        ListaClassiTessitura = New List(Of PUA_ClasseTessitura)
        MessaggioErrore = ""
    End Sub

End Class

Public Class PUA_ClasseTessitura

    Public Tessitura_Cod As Integer
    Public Tessitura_Des As String
    Public minimo As Decimal
    Public pesospecifico As Decimal
    Public peso20 As Decimal
    Public peso30 As Decimal
    Public peso50 As Decimal

    Public Sub New()
        Tessitura_Cod = 0
        Tessitura_Des = ""
        minimo = 0
        pesospecifico = 0
        peso20 = 0
        peso30 = 0
        peso50 = 0
    End Sub

End Class

Public Class PUA_ClassiTessitura_Analisi_input

    Public Url As String

    Public Sabbia As Integer
    Public Argilla As Integer

    Public verbose As Boolean

    Public Sub New()
        Url = ""
        Sabbia = -99
        Argilla = -99
        verbose = False
    End Sub

End Class

Public Class PUA_ClassiTessitura_Analisi_output

    Public ListaClassiTessitura As List(Of PUA_ClasseTessitura_Analisi)
    Public MessaggioErrore As String

    Public Sub New()
        ListaClassiTessitura = New List(Of PUA_ClasseTessitura_Analisi)
        MessaggioErrore = ""
    End Sub

End Class

Public Class PUA_ClasseTessitura_Analisi

    Public Id_ClasseTessitura As Integer
    Public Descrizione As String
    Public Sabbia As Integer
    Public Argilla As Integer

    Public Sub New()
        Id_ClasseTessitura = 0
        Descrizione = ""
        Sabbia = 0
        Argilla = 0
    End Sub

End Class

