Public Class PianoConcimazione_FinalitaRER

    Public Sub New()

    End Sub

    Public Function FinalitaRER(ByVal Input As PianoConcimazione_FinalitaRER_input,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                       As PianoConcimazione_FinalitaRER_output

        Dim Output As New PianoConcimazione_FinalitaRER_output
        Dim Finalita As Finalita
        Dim FinalitaGias As FinalitaGias

        Dim FiltraDaFinalitaGias As Boolean = False
        Dim InserisciFinalita As Boolean = True

        If Not IsNothing(Input.Grfi_Cod) AndAlso Input.Grfi_Cod > 0 Then
            FiltraDaFinalitaGias = True
        End If

        Dim objCoreM As New AgronicaCoreMetaSchemaDAL.GruppoFinalitaMappatura_R
        Dim DtM As New DataTable
        Dim FiltroM As String = ""
        If Not IsNothing(Input.Veg_Cod_Elenco) AndAlso Input.Veg_Cod_Elenco <> "" Then
            FiltroM = " AND Veg_Cod IN (" & Input.Veg_Cod_Elenco & ") "
        End If
        DtM = objCoreM.Leggi(Input.Veg_Cod, 0, 0,
                           FiltroM, "",
                            objParametri)

        Dim Dt As DataTable
        Dim objCore As New AgronicaCoreMetaSchemaDAL.GruppoFinalita_Rer_R
        Dim Filtro As String = ""
        If Not IsNothing(Input.Veg_Cod_Elenco) AndAlso Input.Veg_Cod_Elenco <> "" Then
            Filtro = " AND GruppoFinalitaxSpecieVegetalixConc.Veg_Cod IN (" & Input.Veg_Cod_Elenco & ") "
        End If

        Dt = objCore.Leggi(Input.Regolamento_Cod,
                           0, Input.Veg_Cod, "",
                           Filtro, "",
                            objParametri)

        For i = 0 To Dt.Rows.Count - 1

            InserisciFinalita = True

            If FiltraDaFinalitaGias = True Then

                Dim DrMTmp() As DataRow = DtM.Select("veg_cod=" & Dt.Rows(i).Item("Veg_COD") & " and grfi_cod_gias=" & Input.Grfi_Cod)

                If Not (Not DrMTmp Is Nothing AndAlso DrMTmp.Length > 0) Then
                    InserisciFinalita = False
                End If

            End If


            'aggiunto elenco finalita gias con cui è mappata quella del pua

            If InserisciFinalita = True Then

                Finalita = New Finalita
                Finalita.CodiceSpecie = Dt.Rows(i).Item("Veg_COD")
                Finalita.Descrizione = Dt.Rows(i).Item("Grfi_DES")
                Finalita.Codice = Dt.Rows(i).Item("Grfi_COD")

                Dim DrM() As DataRow

                If FiltraDaFinalitaGias = True Then
                    DrM = DtM.Select("veg_cod=" & Dt.Rows(i).Item("Veg_COD") & " and grfi_cod_pua=" & Dt.Rows(i).Item("Grfi_COD") & " and grfi_cod_gias=" & Input.Grfi_Cod)
                Else
                    DrM = DtM.Select("veg_cod=" & Dt.Rows(i).Item("Veg_COD") & " and grfi_cod_pua=" & Dt.Rows(i).Item("Grfi_COD"))
                End If

                If Not DrM Is Nothing Then
                    For j = 0 To DrM.Length - 1
                        FinalitaGias = New FinalitaGias
                        FinalitaGias.Codice = DrM(j).Item("Grfi_COD_gias")
                        FinalitaGias.Descrizione = DrM(j).Item("Grfi_des_gias")
                        Finalita.ListaFinalitaGias.Add(FinalitaGias)
                    Next
                End If

                Output.ListaFinalita.Add(Finalita)

            End If

        Next

        Return Output

    End Function


End Class

Public Class PianoConcimazione_FinalitaRER_input

    Public Regolamento_Cod As Integer

    Public Veg_Cod As Integer
    Public Veg_Cod_Elenco As String

    Public Grfi_Cod As Integer

    Public Url As String

    Sub New()


    End Sub

End Class

Public Class PianoConcimazione_FinalitaRER_output

    Public ListaFinalita As List(Of Finalita)

    Public MessaggioErrore As String

    Public Sub New()

        ListaFinalita = New List(Of Finalita)
        MessaggioErrore = ""

    End Sub

End Class

Public Class Finalita

    Public CodiceSpecie As Integer
    Public Descrizione As String
    Public Codice As Integer


    Public ListaFinalitaGias As List(Of FinalitaGias)

    Sub New()

        CodiceSpecie = 0
        Descrizione = ""
        Codice = 0

        ListaFinalitaGias = New List(Of FinalitaGias)

    End Sub

End Class

Public Class FinalitaGias

    Public Descrizione As String
    Public Codice As Integer


    Sub New()

        Descrizione = ""
        Codice = 0

    End Sub

End Class


