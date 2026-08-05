Public Class PianoConcimazione_Precessione

    Public Sub New()

    End Sub

    Public Function Precessione(ByVal Input As PianoConcimazione_Precessione_input,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                       As PianoConcimazione_Precessione_output

        Dim Output As New PianoConcimazione_Precessione_output
        Dim Prec As Precessione

        Dim Dt As DataTable
        Dim objCore As New AgronicaCoreMetaSchemaDAL.PC_PrecessioneColturale_R

        Dim PUA_Tipo As Integer = 0
        If Not IsNothing(Input.PUA_Tipo) Then
            PUA_Tipo = Input.PUA_Tipo
        End If

        Dt = objCore.Leggi(Input.Regolamento_Cod, PUA_Tipo,
                           Input.Codice, "", "", objParametri)

        For i = 0 To Dt.Rows.Count - 1

            Prec = New Precessione
            Prec.Descrizione = Dt.Rows(i).Item("Pre_Des")
            Prec.Codice = Dt.Rows(i).Item("Pre_COD")
            If Not IsDBNull(Dt.Rows(i).Item("N_Residuo")) Then
                Prec.N_Residuo = Dt.Rows(i).Item("N_Residuo")
            End If
            Output.ListaPrecessione.Add(Prec)

        Next

        Return Output

    End Function

    Public Function PrecessionexSpecie(ByVal Input As PianoConcimazione_PrecessionexSpecie_input,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                       As PianoConcimazione_PrecessionexSpecie_output

        Dim Output As New PianoConcimazione_PrecessionexSpecie_output
        Dim Prec As PrecessionexSpecie

        Dim Dt As DataTable
        Dim objCore As New AgronicaCoreMetaSchemaDAL.PC_PrecessioneColturale_R

        Dim PUA_Tipo As Integer = 0
        If Not IsNothing(Input.PUA_Tipo) Then
            PUA_Tipo = Input.PUA_Tipo
        End If

        Dt = objCore.Leggi_conSpecie(Input.Regolamento_Cod, PUA_Tipo,
                               Input.Codice, Input.Veg_Cod, "", "", objParametri)

        For i = 0 To Dt.Rows.Count - 1

            Prec = New PrecessionexSpecie
            Prec.Descrizione = Dt.Rows(i).Item("Pre_Des")
            Prec.Codice = Dt.Rows(i).Item("Pre_Cod")
            Prec.Veg_Cod = Dt.Rows(i).Item("Veg_Cod")
            If Not IsDBNull(Dt.Rows(i).Item("N_Residuo")) Then
                Prec.N_Residuo = Dt.Rows(i).Item("N_Residuo")
            End If
            Output.ListaPrecessionexSpecie.Add(Prec)

        Next

        Return Output

    End Function

End Class

Public Class PianoConcimazione_Precessione_input

    Public Regolamento_Cod As Integer
    Public PUA_Tipo As Integer
    Public Codice As Integer
    Public Url As String

    Sub New()

        PUA_Tipo = 0
        Codice = 0

    End Sub

End Class

Public Class PianoConcimazione_Precessione_output

    Public ListaPrecessione As List(Of Precessione)

    Public MessaggioErrore As String

    Public Sub New()

        ListaPrecessione = New List(Of Precessione)
        MessaggioErrore = ""

    End Sub

End Class

Public Class Precessione

    Public Descrizione As String
    Public Codice As Integer
    Public N_Residuo As Decimal

    Sub New()

        Descrizione = ""
        Codice = 0
        N_Residuo = 0

    End Sub

End Class

Public Class PianoConcimazione_PrecessionexSpecie_input

    Public Regolamento_Cod As Integer
    Public PUA_Tipo As Integer
    Public Codice As Integer
    Public Veg_Cod As Integer

    Public Url As String

    Sub New()

        PUA_Tipo = 0
        Codice = 0
        Veg_Cod = 0

    End Sub

End Class

Public Class PianoConcimazione_PrecessionexSpecie_output

    Public ListaPrecessionexSpecie As List(Of PrecessionexSpecie)

    Public MessaggioErrore As String

    Public Sub New()

        ListaPrecessionexSpecie = New List(Of PrecessionexSpecie)
        MessaggioErrore = ""

    End Sub

End Class

Public Class PrecessionexSpecie

    Public Descrizione As String
    Public Codice As Integer
    Public Veg_Cod As Integer
    Public N_Residuo As Decimal

    Sub New()

        Descrizione = ""
        Codice = 0
        Veg_Cod = 0
        N_Residuo = 0

    End Sub

End Class



