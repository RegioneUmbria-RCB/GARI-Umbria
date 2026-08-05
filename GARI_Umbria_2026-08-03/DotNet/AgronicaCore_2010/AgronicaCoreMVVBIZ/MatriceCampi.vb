Imports AgronicaCoreMVVCommon

Public Class MatriceCampi

    Private _mappaturaCampiMatrice As New Dictionary(Of String, Dictionary(Of Integer, Boolean))

    Public Sub New()

        Dim categorie As Dictionary(Of Integer, Boolean) = Nothing

        ' Titoli Alcol
        _mappaturaCampiMatrice.Add(enumAttributiSian.AlcoolPotenziale.ToString, Nothing)
        categorie = New Dictionary(Of Integer, Boolean) From
           {
                {4, False},
                {5, False},
                {6, False},
                {7, True},
                {8, False},
                {9, True},
                {10, True},
                {11, True},
                {12, True},
                {13, True},
                {14, False},
                {15, False},
                {16, True},
                {17, True}
           }
        _mappaturaCampiMatrice(enumAttributiSian.AlcoolPotenziale.ToString) = categorie

        _mappaturaCampiMatrice.Add(enumAttributiSian.AlcoolEffettivo.ToString, Nothing)
        categorie = New Dictionary(Of Integer, Boolean) From
           {
               {4, False},
               {5, False},
               {6, False},
               {7, False},
               {8, False},
               {9, False},
               {10, False},
               {11, False},
               {12, True},
               {13, False},
               {14, True},
               {15, True},
               {16, True},
               {17, True}
           }
        _mappaturaCampiMatrice(enumAttributiSian.AlcoolEffettivo.ToString) = categorie

        _mappaturaCampiMatrice.Add(enumAttributiSian.AlcoolTotale.ToString, Nothing)
        categorie = New Dictionary(Of Integer, Boolean) From
           {
                {4, True},
                {5, False},
                {6, False},
                {7, True},
                {8, True},
                {9, False},
                {10, False},
                {11, False},
                {12, False},
                {13, False},
                {14, True},
                {15, False},
                {16, False},
                {17, False}
           }
        _mappaturaCampiMatrice(enumAttributiSian.AlcoolTotale.ToString) = categorie

        ' Pratiche Enologiche
        _mappaturaCampiMatrice.Add(enumAttributiSian.PraticaEnologica_N.ToString, Nothing)
        categorie = New Dictionary(Of Integer, Boolean) From
           {
                {4, True},
                {5, False},
                {6, False},
                {7, True},
                {8, True},
                {9, True},
                {10, True},
                {11, False},
                {12, True},
                {13, True},
                {14, True},
                {15, True},
                {16, False},
                {17, False}
           }
        _mappaturaCampiMatrice(enumAttributiSian.PraticaEnologica_N.ToString) = categorie


    End Sub

    Public Function CampoObbligatorio(ByVal campo As enumAttributiSian, ByVal codCategoria As Integer) As Boolean

        If Not _mappaturaCampiMatrice.ContainsKey(campo.ToString) Then
            Return True
        End If

        If Not _mappaturaCampiMatrice(campo.ToString).ContainsKey(codCategoria) Then
            Return True
        End If

        Return _mappaturaCampiMatrice(campo.ToString)(codCategoria)

    End Function


End Class
