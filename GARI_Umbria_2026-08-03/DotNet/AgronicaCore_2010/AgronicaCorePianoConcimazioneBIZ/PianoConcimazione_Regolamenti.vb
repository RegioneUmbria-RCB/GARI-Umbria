Public Class PianoConcimazione_Regolamenti

    Public Sub New()

    End Sub

    Public Function Regolamenti(ByVal Input As PianoConcimazione_Regolamenti_input,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                       As PianoConcimazione_Regolamenti_output

        Dim Output As New PianoConcimazione_Regolamenti_output
        Dim Elemento As Regolamento
        Dim Parametro As Regolamento_Parametro

        Dim Dt As DataTable
        Dim objReg As New AgronicaCoreMetaSchemaDAL.PUA_Regolamenti_R

        Dim DtPar As DataTable
        Dim DrPar() As DataRow
        Dim objPar As New AgronicaCoreMetaSchemaDAL.PUA_ParametrixRegolamenti_R

        Dim TipoMetodo As Integer = 0
        If Not IsNothing(Input.TipoMetodo) Then
            TipoMetodo = Input.TipoMetodo
        End If

        Dim VisualizzaPrivati As Boolean = False
        If Not IsNothing(Input.VisualizzaPrivati) Then
            VisualizzaPrivati = Input.VisualizzaPrivati
        End If
        Dim Piva_Superuser As String = ""
        If Not IsNothing(Input.VisualizzaPrivati) Then
            Piva_Superuser = Input.Piva_Superuser
        End If

        Dt = objReg.Leggi(Input.Regolamento_Cod,
                           Input.Tipo, TipoMetodo, VisualizzaPrivati, Piva_Superuser,
                           Input.DataInizio, Input.DataFine,
                           Input.strFiltro, Input.strOrdinamento,
                           objParametri)

        DtPar = objPar.Leggi(0, Input.Regolamento_Cod, "", "", objParametri)

        For i = 0 To Dt.Rows.Count - 1

            DrPar = DtPar.Select("regolamento_cod=" & Dt.Rows(i).Item("regolamento_cod"))

            Elemento = New Regolamento
            Elemento.Descrizione = Dt.Rows(i).Item("Regolamento_Des")
            Elemento.Codice = Dt.Rows(i).Item("Regolamento_Cod")
            Elemento.Tipo = Dt.Rows(i).Item("Tipo")
            Elemento.Validita_Inizio = Dt.Rows(i).Item("Validita_Inizio")
            Elemento.Validita_Fine = Dt.Rows(i).Item("Validita_Fine")

            If Not IsDBNull(Dt.Rows(i).Item("IDEnte")) Then
                Elemento.IDEnte = Dt.Rows(i).Item("IDEnte")
            End If

            If Not DrPar Is Nothing AndAlso DrPar.Length > 0 Then
                For j = 0 To DrPar.Length - 1
                    Parametro = New Regolamento_Parametro
                    Parametro.Codice = DrPar(j).Item("parametro_cod")
                    Parametro.Descrizione = DrPar(j).Item("parametro_des")
                    Parametro.Valore = DrPar(j).Item("parametro_valore")
                    Elemento.ListaParametri.Add(Parametro)
                Next
            End If

            Output.ListaRegolamenti.Add(Elemento)

        Next

        Return Output

    End Function

End Class

Public Class PianoConcimazione_Regolamenti_input

    Public Regolamento_Cod As Integer
    Public Tipo As Integer
    Public DataInizio As Date
    Public DataFine As Date

    Public strFiltro As String
    Public strOrdinamento As String

    Public TipoMetodo As Integer

    Public VisualizzaPrivati As Boolean
    Public Piva_Superuser As String

    Public url As String

    Sub New()

        Regolamento_Cod = 0
        Tipo = 0
        DataInizio = #1/1/1900#
        DataFine = #12/31/2100#
        strFiltro = ""
        strOrdinamento = ""
        TipoMetodo = 0
        VisualizzaPrivati = False
        Piva_Superuser = ""

    End Sub

End Class

Public Class PianoConcimazione_Regolamenti_output

    Public ListaRegolamenti As List(Of Regolamento)

    Public MessaggioErrore As String

    Public Sub New()

        ListaRegolamenti = New List(Of Regolamento)
        MessaggioErrore = ""

    End Sub

End Class

Public Class Regolamento

    Public Descrizione As String
    Public Codice As Integer
    Public Tipo As Integer
    Public Validita_Inizio As Date
    Public Validita_Fine As Date
    Public IDEnte As Integer

    Public ListaParametri As List(Of Regolamento_Parametro)

    Sub New()

        Descrizione = ""
        Codice = 0
        Tipo = 0
        IDEnte = 0

        ListaParametri = New List(Of Regolamento_Parametro)

        Validita_Inizio = #1/1/1900#
        Validita_Fine = #12/31/2100#

    End Sub

End Class

Public Class Regolamento_Parametro

    Public Descrizione As String
    Public Codice As Integer
    Public Valore As String
    Public Validita_Inizio As Date
    Public Validita_Fine As Date

    Sub New()

        Descrizione = ""
        Codice = 0
        Valore = ""
        Validita_Inizio = #1/1/1900#
        Validita_Fine = #12/31/2100#

    End Sub

End Class

