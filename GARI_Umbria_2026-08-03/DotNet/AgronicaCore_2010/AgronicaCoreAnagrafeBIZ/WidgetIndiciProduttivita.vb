Imports AgronicaCoreDataProvider
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDTOStd.InData.Widgets
Imports AgronicaCoreModelsSTD.baseClass
Imports AgronicaCoreModelsSTD.metaschema.utilizzi
Imports AgronicaCoreAnagrafeBIZ.My.Resources.AgronicaCoreAnagrafeBIZ

Public Class WidgetIndiciProduttivitaRead
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' <summary>
    ''' Extracts all records from IndiciProduttivitaAi_COD for a given vegCod and year
    ''' </summary>
    ''' <param name="piva"></param>
    ''' <param name="year"></param>
    ''' <param name="vegCod"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function readSpecificIndiciProduttivita(
                                                  ByVal piva As String,
                                                  ByVal year As Integer,
                                                  ByRef objParametri As AgronicaCoreParametri,
                                                  Optional ByVal vegCod As Integer = 0
                                                  ) As List(Of WidgetIndiciProduttivita)

        Dim indiciProduttivita As New List(Of WidgetIndiciProduttivita)
        Dim objWidget As New WidgetIndiciProduttivita_R

        Dim dt = objWidget.readSpecificIndiciProduttivita(piva, year, vegCod, objParametri)

        For Each row As DataRow In dt.Rows
            Dim idx As New WidgetIndiciProduttivita
            idx.indici = New List(Of BaseCodeValue(Of String, Integer))
            idx.indiceProduttivitaAICod = row("IndiciProduttivitaAi_COD")
            idx.sup = row("Sup_Imp")

            Dim varieta As New Varieta
            varieta.codice = row("Cul_Cod")
            varieta.descrizione = row("Cul_Des")
            varieta.specie = New Specie(row("Veg_Cod"), row("Veg_Des"))
            varieta.classType = "Varieta"
            idx.utilizzoTerreno = varieta

            Dim colsList As New List(Of DataColumn)

            Dim enumerator = dt.Columns.GetEnumerator()
            skipFirstIndexItems(6, enumerator)

            While enumerator.MoveNext()
                Dim col = CType(enumerator.Current, DataColumn)
                colsList.Add(col)
            End While

            For Each col As DataColumn In colsList
                Dim indice As New BaseCodeValue(Of String, Integer)(getIndiceProduttivitaEnum(col.ColumnName), If(IsDBNull(row(col.ColumnName)), 0, row(col.ColumnName)), col.ColumnName)
                idx.indici.Add(indice)
            Next

            indiciProduttivita.Add(idx)
        Next

        Return indiciProduttivita

    End Function

    Public Function readGeneralIndiciProduttivita(
                                                 ByVal regione As String,
                                                 ByVal year As Integer,
                                                 ByRef objParametri As AgronicaCoreParametri
                                                 ) As List(Of WidgetIndiciProduttivita)

        Dim indiciProduttivita As New List(Of WidgetIndiciProduttivita)
        Dim objWidget As New WidgetIndiciProduttivita_R

        Dim dt = objWidget.readGeneralIndiciProduttivita(regione, year, objParametri)

        For Each row As DataRow In dt.Rows
            Dim idx As New WidgetIndiciProduttivita
            idx.indici = New List(Of BaseCodeValue(Of String, Integer))
            idx.indiceProduttivitaAICod = row("IndiciProduttivitaAi_COD")

            Dim colsList As New List(Of DataColumn)
            Dim enumerator = dt.Columns.GetEnumerator()
            skipFirstIndexItems(1, enumerator)

            While enumerator.MoveNext()
                Dim col = CType(enumerator.Current, DataColumn)
                colsList.Add(col)
            End While

            For Each col As DataColumn In colsList
                Dim indice As New BaseCodeValue(Of String, Integer)(getIndiceProduttivitaEnum(col.ColumnName), If(IsDBNull(row(col.ColumnName)), 0, row(col.ColumnName)), col.ColumnName)
                idx.indici.Add(indice)
            Next

            indiciProduttivita.Add(idx)
        Next

        Return indiciProduttivita

    End Function

    Public Function initializeWidgetIndiciProduttivitaXImpresa(
                                                     ByVal piva As String,
                                                     ByRef objParametri As AgronicaCoreParametri
                                                     ) As WidgetIndiciProduttivitaXImpresa

        Dim objWidget As New WidgetIndiciProduttivita_R
        Dim indiciXImpresa As New WidgetIndiciProduttivitaXImpresa


        Dim dt = objWidget.readDatiImpresaIndiceProduttivita(piva, objParametri)
        Dim cuaa = dt.Rows(0)("cuaa")
        Dim ragSoc = dt.Rows(0)("rag_soc")
        Dim regioneCod = dt.Rows(0)("regioneCod")
        Dim regioneDes = dt.Rows(0)("regioneDes")

        indiciXImpresa.piva = piva
        indiciXImpresa.ragSoc = ragSoc
        indiciXImpresa.cuaa = cuaa
        indiciXImpresa.regione = New BaseCodeDescrStr(regioneCod, regioneDes)

        Return indiciXImpresa

    End Function

    Public Function readAvailableYears(
                                      ByVal piva As String,
                                      ByRef objParametri As AgronicaCoreParametri
                                      ) As List(Of Integer)

        Dim objWidget As New WidgetIndiciProduttivita_R
        Dim years As New List(Of Integer)

        Dim dt = objWidget.readAvailableYears(piva, objParametri)

        For Each row In dt.Rows
            Dim startDate = CType(row("Validita_Inizio"), Date)
            Dim endDate = CType(row("Validita_Fine"), Date)

            If Not years.Contains(startDate.Year) Then
                years.Add(startDate.Year)
            End If

            If Not years.Contains(endDate.Year) Then
                years.Add(endDate.Year)
            End If
        Next

        Return years

    End Function

    Public Function readKPI(
                           ByVal piva As String,
                           ByVal year As Integer,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As List(Of WidgetKPI)

        Dim objWidget As New WidgetIndiciProduttivita_R
        Dim kpi As New List(Of WidgetKPI)

        Dim dt = objWidget.readKPI(piva, year, objParametri)

        For Each row In dt.Rows
            Dim co2 As New WidgetKPI
            co2.des = "<ul><li>" & WidgetKPI_Co2_Des & "</li></ul>"
            co2.kpi = WidgetKPI_Co2
            co2.val = If(CType(row("indiceCO2NN"), Integer) > 5, 5, CType(row("indiceCO2NN"), Integer))
            co2.icon = "k-i-xi-widget-carbon"
            co2.udm = ""
            co2.max = 5
            co2.min = 1
            co2.indicator = If(CType(row("indiceCO2NN"), Integer) < 4, If(CType(row("indiceCO2NN"), Integer) > 1, IndicatoreRatingUnico.Medio, IndicatoreRatingUnico.Basso), IndicatoreRatingUnico.Alto)

            Dim erosione As New WidgetKPI
            erosione.des = "<ul><li>" & WidgetKPI_Erosione_Des & "</li></ul>"
            erosione.kpi = WidgetKPI_Erosione
            erosione.val = If(CType(row("indiceErosioneNN"), Integer) > 5, 5, CType(row("indiceErosioneNN"), Integer))
            erosione.icon = "k-i-xi-widget-erosion"
            erosione.udm = ""
            erosione.max = 5
            erosione.min = 1
            erosione.indicator = If(CType(row("indiceErosioneNN"), Integer) < 4, If(CType(row("indiceErosioneNN"), Integer) > 1, IndicatoreRatingUnico.Medio, IndicatoreRatingUnico.Alto), IndicatoreRatingUnico.Basso)

            'Dim produttivita As New WidgetKPI
            'produttivita.des = "Stima la resa agricola valutata a prezzi di mercato e la confronta con il benchmark di mercato"
            'produttivita.kpi = "Valutazione della produttività agronomica (PLV)"
            'produttivita.val = CType(row("indiceProduttivitaNN"), Integer)
            'produttivita.icon = ""
            'produttivita.udm = ""
            'produttivita.max = 5
            'produttivita.min = 1
            'produttivita.indicator = If(CType(row("indiceProduttivitaNN"), Integer) < 5, If(CType(row("indiceProduttivitaNN"), Integer) > 1, IndicatoreRatingUnico.Medio, IndicatoreRatingUnico.Alto), IndicatoreRatingUnico.Basso)

            Dim plv As New WidgetKPI
            plv.des = "<ul><li>" & WidgetKPI_PLV_Des & "</li></ul>"
            plv.kpi = WidgetKPI_PLV
            plv.val = If(CType(row("indicePlvNN"), Integer) > 5, 5, CType(row("indicePlvNN"), Integer))
            plv.icon = "k-i-xi-widget-productivity"
            plv.udm = ""
            plv.max = 5
            plv.min = 1
            plv.indicator = If(CType(row("indicePlvNN"), Integer) < 4, If(CType(row("indicePlvNN"), Integer) > 1, IndicatoreRatingUnico.Medio, IndicatoreRatingUnico.Basso), IndicatoreRatingUnico.Alto)

            Dim meteo As New WidgetKPI
            meteo.des = "<ul><li>" & WidgetKPI_Meteo_Des_Produzione & "</li></ul>"
            meteo.kpi = WidgetKPI_Meteo
            meteo.val = CType(row("indiceRischioMeteoAggregatoNN"), Integer)
            meteo.icon = "k-i-xi-widget-weather"
            meteo.udm = "%"
            meteo.max = 100
            meteo.min = 0
            meteo.indicator = If(CType(row("indiceRischioMeteoAggregatoNN"), Integer) < 60, If(CType(row("indiceRischioMeteoAggregatoNN"), Integer) > 20, IndicatoreRatingUnico.Medio, IndicatoreRatingUnico.Alto), IndicatoreRatingUnico.Basso)

            kpi.Add(co2)
            kpi.Add(erosione)
            kpi.Add(plv)
            'kpi.Add(produttivita)
            kpi.Add(meteo)
        Next

        Return kpi

    End Function

    Private Sub skipFirstIndexItems(
                                   ByVal index As Integer,
                                   ByRef enumerator As IEnumerator
                                   )

        Do While index > 0
            index -= 1
            enumerator.MoveNext()
        Loop

    End Sub

    Private Function getIndiceProduttivitaEnum(ByVal indiceDes As String) As Integer
        Select Case indiceDes
            Case "Produttivita"
                Return 1
            Case "PLV"
                Return 2
            Case "IndiceErosione"
                Return 3
            Case "IndiceCO2"
                Return 4
            Case "IndiceRischioMeteoAggregato"
                Return 5
            Case "IndiceRischioGelata"
                Return 6
            Case "IndiceRischioVentoForte"
                Return 7
            Case "IndiceRischioSiccita"
                Return 8
            Case "IndiceRischioGrandine"
                Return 9
            Case "IndiceRischioAllagamento"
                Return 10
            Case Else
                Return 0
        End Select
    End Function


End Class
