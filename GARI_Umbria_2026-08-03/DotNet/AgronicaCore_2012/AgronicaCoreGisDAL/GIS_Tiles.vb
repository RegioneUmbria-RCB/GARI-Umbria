Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider

Public Class GIS_Tiles_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Tiles_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Stb.Length = 0
            Stb.AppendLine("SELECT ")
            Stb.AppendLine("PivaSuperUser, ElementoGrafico_Cod, Tiles_Cod, ")
            Stb.AppendLine("inviato, datainvio, Data_Creazione, Data_Modifica, ")
            Stb.AppendLine("Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine ")

            Stb.AppendLine("FROM GIS_Tiles ")
            Stb.AppendLine("WHERE PivaSuperUser = '" & objParametri.PivaSuperUser & "'")
            'Stb.AppendLine("AND GIS_TipoEntita.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.AppendLine("AND GIS_Tiles.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.AppendLine("AND GIS_Tiles.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

End Class


Public Class GIS_Tiles_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal ElementoGraficoCod As Integer,
                           ByVal TilesCod As List(Of Integer),
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Tiles_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            Stb.Length = 0
            Stb.AppendLine("INSERT INTO GIS_Tiles")
            Stb.AppendLine("( ")
            Stb.AppendLine("PivaSuperUser,")
            Stb.AppendLine("ElementoGrafico_Cod,")
            Stb.AppendLine("Tiles_Cod,")
            Stb.AppendLine("Tiles_GeoPolygon,")
            Stb.AppendLine("Inviato,")
            Stb.AppendLine("DataInvio,")
            Stb.AppendLine("Data_Creazione,")
            Stb.AppendLine("Data_Modifica,")
            Stb.AppendLine("UserName_Creazione,")
            Stb.AppendLine("UserName_Modifica,")
            Stb.AppendLine("Validita_Inizio,")
            Stb.AppendLine("Validita_Fine")
            Stb.AppendLine(")")
            Stb.AppendLine("VALUES")
            Dim prima As Boolean = True
            For Each TileCod In TilesCod

                If Not prima Then
                    Stb.AppendLine(",")
                End If

                Stb.AppendLine("(")
                Stb.AppendLine("'" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "',")
                Stb.AppendLine(Agro_SQL_SaveNum(ElementoGraficoCod) & ",")
                Stb.AppendLine(Agro_SQL_SaveNum(TileCod) & ",")
                Stb.AppendLine("NULL,")
                Stb.AppendLine("0,")
                Stb.AppendLine("NULL,")
                Stb.AppendLine(Agro_SQL_SaveDate(Date.Now) & ",")
                Stb.AppendLine(Agro_SQL_SaveDate(Date.Now) & ",")
                Stb.AppendLine("'" & Agro_SQL_SaveText(objParametri.SuperUserUsername) & "',")
                Stb.AppendLine("'" & Agro_SQL_SaveText(objParametri.SuperUserUsername) & "',")
                Stb.AppendLine(Agro_SQL_SaveDate(AGRODATAINIZIO) & ",  ")
                Stb.AppendLine(Agro_SQL_SaveDate(AGRODATAFINE))
                Stb.AppendLine(")")
                prima = False
            Next

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

End Class
