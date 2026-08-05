
Imports AgronicaCoreDataProviderSTD.CostantiPersonalizzate
Imports AgronicaCoreDataProviderSTD.TipiEnumerativi

Public Class CategorieMagazzino

    Public Shared Function LeggiCategorieMagazzinoDatoLavCod(Lav_Cod As Integer) As List(Of Integer)

        Dim ListaElemCod As New List(Of Integer)

        Select Case Lav_Cod
            Case LAVCOD_CONCIA_SEME,
             LAVCOD_DISERBO,
             LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
             LAVCOD_TRATTAMENTO_FITOREGOLATORE,
             LAVCOD_GEODISINFESTAZIONE,
             LAVCOD_DISSECCAMENTO,
             LAVCOD_TRATTAMENTO_POST_RACCOLTA

                ListaElemCod.Add(enum_CategorieMagazzino.FORMULATI)

            Case LAVCOD_DISTRIBUZIONE_INSETTI
                ListaElemCod.Add(enum_CategorieMagazzino.INSETTI)


            Case LAVCOD_CONFUSIONE_SESSUALE,
                LAVCOD_DISORIENTAMENTO_SESSUALE,
                LAVCOD_CATTURE_MASSA,
                LAVCOD_INSTALLAZIONE_TRAPPOLE

                ListaElemCod.Add(enum_CategorieMagazzino.TRAPPOLE)

            Case LAVCOD_FERTIRRIGAZIONE,
                 LAVCOD_DISTRIBUZIONE_CONCIME,
                 LAVCOD_CONCIMAZIONE_FOGLIARE,
                 LAVCOD_DISTRIBUZIONE_AMMENDANTI,
                 LAVCOD_SARCHIATURA_CONCIMAZIONE,
                 LAVCOD_TRATTAMENTO_ANTIBUTTERATURA

                ListaElemCod.Add(enum_CategorieMagazzino.FERTILIZZANTI)


            Case LAVCOD_SEMINA,
                 LAVCOD_TRAPIANTO,
                 LAVCOD_SOVESCIO,
                 LAVCOD_SOD_SEDDING

                ListaElemCod.Add(enum_CategorieMagazzino.SEMENTI)


        End Select

        Return ListaElemCod

    End Function

    Public Shared Function LeggiFiltroCategoriaMagazzino(Elem_Cod As Integer, FiltroProdottiMagazzino As String) As String

        Dim filtro As String = ""

        If Not String.IsNullOrEmpty(FiltroProdottiMagazzino) Then
            Dim filtro_categorie = FiltroProdottiMagazzino.Split(CChar("|"))
            For Each filtro In filtro_categorie
                Dim items = filtro.Split(CChar("_"))
                If items.Length > 1 AndAlso Elem_Cod = CInt(items(0)) Then
                    filtro = items(1)
                    Exit For
                End If
            Next
        End If

        Return filtro

    End Function

End Class
