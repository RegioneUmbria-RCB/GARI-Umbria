Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Public Class GIS_ElementiGrafici_CatastoSimplyfied_R
    Inherits DataProvider2010


End Class
Public Class GIS_ElementiGrafici_CatastoSimplyfied_W
    Inherits DataProvider2010

    Public Function InserisciPoligonoSemplificatoCatastoDaGIS(
                                    ByVal LayerElementiGrafici_Cod As Integer,
                                    ByRef objParametri As AgronicaCoreParametri
                                   ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_CatastoSimplyfied_W.InserisciPoligonoSemplificatoCatastoDaGIS()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim rval As Boolean

        Try
            '---------------------------------------------

            If LayerElementiGrafici_Cod <= 0 Then
                Throw New Exception("Valorizzare il parametro nella query (LayerElementiGrafici_Cod)")
            End If

            Stb.Length = 0

            Stb.AppendLine(" insert into [dbo].[GIS_ElementiGrafici_CatastoSimplyfied] ")
            Stb.AppendLine(" Select Case ")
            Stb.AppendLine("     [PivaSuperUser], ")
            Stb.AppendLine("     [ElementoGrafico_Cod], ")
            Stb.AppendLine("     [Entita_Cod], ")
            Stb.AppendLine("     geography: STGeomFromWKB((geometry: STGeomFromWKB([Poligono_GeoEntity].STAsBinary(), [Poligono_GeoEntity].STSrid).STEnvelope()).STAsBinary(),4326) As [Poligono_GeoEntity], ")
            Stb.AppendLine("     [inviato], ")
            Stb.AppendLine("     [datainvio], ")
            Stb.AppendLine("     [Data_creazione], ")
            Stb.AppendLine("     [Data_modifica], ")
            Stb.AppendLine("     [username_creazione], ")
            Stb.AppendLine("     [username_modifica], ")
            Stb.AppendLine("     [Validita_Inizio], ")
            Stb.AppendLine("     [Validita_Fine] ")
            Stb.AppendLine(" From GIS_ElementiGrafici a (NOLOCK) ")
            Stb.AppendLine(" Where 1=1 ")
            If LayerElementiGrafici_Cod <> 0 Then
                Stb.AppendLine(String.Format(" And LayerElementiGrafici_Cod = {0} ", Agro_SQL_SaveNum(LayerElementiGrafici_Cod)))
            End If
            Stb.AppendLine(" And Not exists( ")
            Stb.AppendLine(" Select ")
            Stb.AppendLine("     1  ")
            Stb.AppendLine(" from ")
            Stb.AppendLine("     GIS_ElementiGrafici_CatastoSimplyfied b (NOLOCK) ")
            Stb.AppendLine(" Where ")
            Stb.AppendLine("    b.PivaSuperUser = a.PivaSuperUser ")
            Stb.AppendLine("    And a.Entita_Cod = b.Entita_Cod ")
            Stb.AppendLine("  ) ")

            '--------------------------------------------------------------------------
            rval = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            rval = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return rval

    End Function
End Class