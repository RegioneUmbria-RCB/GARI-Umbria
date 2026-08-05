Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.Transactions
Imports AgronicaCoreAnagrafeDAL

Public Class EF_GIS_ElementiGrafici
    Private Shared Function Create_GIS_ElementoGrafico(ByRef dal As Gias_DeveloperServer_Entities,
                                                        ByRef objParametri As AgronicaCoreParametri,
                                                        ByRef username As String
                                                        ) As GIS_ElementiGrafici

        Dim elementografico As New GIS_ElementiGrafici
        elementografico.PivaSuperUser = objParametri.PivaSuperUser
        elementografico.ElementoGrafico_Cod = NuovoElementoGrafico_Cod(dal, objParametri)
        elementografico.ElementoGrafico_Des = ""
        elementografico.Entita_Cod = 0
        elementografico.LayerElementiGrafici_Cod = 0
        elementografico.Flag_GPS = 0
        elementografico.inviato = 0
        elementografico.Data_Creazione = DateTime.Now
        elementografico.Data_Modifica = DateTime.Now
        elementografico.Username_Creazione = username
        elementografico.Username_Modifica = username
        elementografico.Validita_Inizio = AGRODATAINIZIO
        elementografico.Validita_Fine = AGRODATAFINE

        Return elementografico

    End Function

    Private Shared Function NuovoElementoGrafico_Cod(ByRef GiasContext As Gias_DeveloperServer_Entities, ByRef objParametri As AgronicaCoreParametri) As Integer

        Dim idGen As New Agro_Sequenze
        'Dim entita_cod = idGen.NuovoId_Tabella_EF(GiasContext, "gis_elementigrafici", 0, 2000000000, objParametri)
        Dim entita_cod = idGen.NuovoId_Tabella("gis_elementigrafici", 0, 2000000000, objParametri)

        Return entita_cod
    End Function

    Public Shared Function GIS_ElementoGrafico_Scrivi_EF(ByVal Dati_GIS_ElementoGrafico As ElementiGrafici,
                                                       ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                       ByVal username As String,
                                                       Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                       Optional ByVal NewTransaction As Boolean = True,
                                                         Optional ByVal LogVerbose As Boolean = False
                                                       ) As GIS_ElementiGrafici

        Dim nomeRoutine As String = "AgronicaCoreGisDAL.EF_GIS_ElementiGrafici.GIS_ElementoGrafico_Scrivi_EF()"
        Dim messaggioErrore As String = ""

        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        'Lavez - 27/05/2025 - Log verboso
        Dim logprovder As LogProvider = Nothing

        If LogVerbose Then
            logprovder = New LogProvider()
        End If
        'Lavez - 27/05/2025 - Log verbos

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        'Lavez - 27/05/2025 - Log verboso
        If LogVerbose Then
            logprovder.Scrivi_LOG(objParametriServer, nomeRoutine, "Prima di Create_GIS_ElementoGrafico (calcolo progressivo)", False)
        End If
        Dim elemento = Create_GIS_ElementoGrafico(GiasContext,
                                                 objParametriServer,
                                                 username
                                                 )
        'Lavez - 27/05/2025 - Log verboso
        If LogVerbose Then
            logprovder.Scrivi_LOG(objParametriServer, nomeRoutine, "Dopo di Create_GIS_ElementoGrafico (calcolo progressivo)", False)
        End If

        Try
            elemento.Entita_Cod = Dati_GIS_ElementoGrafico.EntitaCod
            elemento.LayerElementiGrafici_Cod = Dati_GIS_ElementoGrafico.Layer
            elemento.ElementoGrafico_Des = Dati_GIS_ElementoGrafico.ElementoGraficoDes

            If Dati_GIS_ElementoGrafico.Cartography <> "" Then

                Dim type = Dati_GIS_ElementoGrafico.Cartography.Split("(")(0).Trim.ToUpper
                If type.Equals("POINT") Then
                    Dim ef_GIS_elementiGrafici = New EF_GIS_ElementiGrafici
                    'Dim new_wkt = ef_GIS_elementiGrafici.SwapLatLngInWKT(Dati_GIS_ElementoGrafico.Cartography)
                    elemento.Poligono_GeoEntity = Entity.Spatial.DbGeography.PointFromText(Dati_GIS_ElementoGrafico.Cartography, 4326)
                ElseIf type.Equals("POLYGON") Then
                    ' VAnni: 29/12/2021: Orientamento del poligono in senso antiorario se necessario
                    Dim xTest As New AgronicaCoreGisDAL.GIS_ElementiGrafici_W
                    'Lavez - 27/05/2025 - Log verboso
                    If LogVerbose Then
                        logprovder.Scrivi_LOG(objParametriServer, nomeRoutine, "Prima di TestaPoligono", False)
                    End If
                    Dim esitoOrientamento As Boolean =
                    xTest.TestaPoligono(Dati_GIS_ElementoGrafico.Cartography)
                    'Lavez - 27/05/2025 - Log verboso
                    If LogVerbose Then
                        logprovder.Scrivi_LOG(objParametriServer, nomeRoutine, "Dopo di TestaPoligono", False)
                    End If
                    If Not esitoOrientamento Then
                        Dati_GIS_ElementoGrafico.Cartography = AgronicaGIS2012.Commons.PolygonOrder.InvertiPoligono_wkt(Dati_GIS_ElementoGrafico.Cartography, False)
                    End If

                    'conversione del dato cartografico
                    elemento.Poligono_GeoEntity = Entity.Spatial.DbGeography.PolygonFromText(Dati_GIS_ElementoGrafico.Cartography, 4326)
                ElseIf type.Equals("LINESTRING") Then
                    Dim ef_GIS_elementiGrafici = New EF_GIS_ElementiGrafici
                    'Dim new_wkt = ef_GIS_elementiGrafici.SwapLatLngInWKT(Dati_GIS_ElementoGrafico.Cartography)
                    elemento.Poligono_GeoEntity = Entity.Spatial.DbGeography.LineFromText(Dati_GIS_ElementoGrafico.Cartography, 4326)
                ElseIf type.Equals("MULTIPOLYGON") Then
                    Dim ef_GIS_elementiGrafici = New EF_GIS_ElementiGrafici
                    'Dim new_wkt = ef_GIS_elementiGrafici.SwapLatLngInWKT(Dati_GIS_ElementoGrafico.Cartography)
                    elemento.Poligono_GeoEntity = Entity.Spatial.DbGeography.MultiPolygonFromText(Dati_GIS_ElementoGrafico.Cartography, 4326)
                End If

            End If

            elemento.Flag_GPS = Dati_GIS_ElementoGrafico.Flag_GPS

            GiasContext.GIS_ElementiGrafici.Add(elemento)
            GiasContext.SaveChanges()

            'Lavez - 27/05/2025 - Log verboso
            If LogVerbose Then
                logprovder.Scrivi_LOG(objParametriServer, nomeRoutine, "Dopo di SaveChanges", False)
            End If

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return elemento

    End Function

    Private Function SwapLatLngInWKT(ByVal wkt As String) As String
        Try
            Dim new_wkt As String
            Dim type = wkt.Split("(")(0).Trim.ToUpper
            If type = "POINT" Then
                Dim coord = wkt.Split("(")(1).Replace(")", "")
                Dim ll = coord.Split(" ")
                new_wkt = type + "(" + ll(1) + " " + ll(0) + ")"
            ElseIf type = "LINESTRING" Then
                Dim coord = wkt.Split("(")(1).Replace(")", "").Replace(", ", ",")
                Dim ll = coord.Split(",")
                new_wkt = type + " ("

                For Each _ll In ll
                    Dim point = _ll.Split(" ")
                    new_wkt += point(1) + " " + point(0) + ", "
                Next

                new_wkt += ")"

                Dim idx_space = new_wkt.LastIndexOf(" ")
                Dim idx_comma = new_wkt.LastIndexOf(",")
                If idx_space <> -1 Then
                    new_wkt = new_wkt.Remove(idx_space, 1)
                End If
                If idx_comma <> -1 Then
                    new_wkt = new_wkt.Remove(idx_comma, 1)
                End If
            End If
            Return new_wkt
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Shared Function GIS_ElementoGrafico_Modifica_EF(ByVal Dati_GIS_ElementoGrafico As ElementiGrafici,
                                                           ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                           ByVal username As String,
                                                           Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                           Optional ByVal NewTransaction As Boolean = True,
                                                           Optional ByVal LogVerbose As Boolean = False
                                                           ) As GIS_ElementiGrafici

        Dim nomeRoutine As String = "AgronicaCoreGisDAL.EF_GIS_ElementiGrafici.GIS_ElementoGrafico_Modifica_EF()"
        Dim messaggioErrore As String = ""

        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        'Lavez - 27/05/2025 - Log verboso
        Dim logprovder As LogProvider = Nothing

        If LogVerbose Then
            logprovder = New LogProvider()
        End If
        'Lavez - 27/05/2025 - Log verbos

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        'Lavez - 27/05/2025 - Log verboso
        If LogVerbose Then
            logprovder.Scrivi_LOG(objParametriServer, nomeRoutine, "Prima di lettura elemento grafico", False)
        End If

        Dim elementiList = From ele In GiasContext.GIS_ElementiGrafici
                           Where ele.PivaSuperUser = Dati_GIS_ElementoGrafico.PivaSuperUser AndAlso
                                 ele.ElementoGrafico_Cod = Dati_GIS_ElementoGrafico.ElementoGraficoCod
                           Select ele

        Dim elemento = elementiList.FirstOrDefault

        'Lavez - 27/05/2025 - Log verboso
        If LogVerbose Then
            logprovder.Scrivi_LOG(objParametriServer, nomeRoutine, "Dopo di lettura elemento grafico", False)
        End If
        Try
            If Dati_GIS_ElementoGrafico.Cartography <> "" Then


                Dim type = Dati_GIS_ElementoGrafico.Cartography.Split("(")(0).Trim.ToUpper
                If type.Equals("POINT") Then
                    Dim ef_GIS_elementiGrafici = New EF_GIS_ElementiGrafici
                    Dim new_wkt = ef_GIS_elementiGrafici.SwapLatLngInWKT(Dati_GIS_ElementoGrafico.Cartography)
                    elemento.Poligono_GeoEntity = Entity.Spatial.DbGeography.PointFromText(Dati_GIS_ElementoGrafico.Cartography, 4326)
                ElseIf type.Equals("POLYGON") Then
                    ' VAnni: 29/12/2021: Orientamento del poligono in senso antiorario se necessario
                    Dim xTest As New AgronicaCoreGisDAL.GIS_ElementiGrafici_W
                    'Lavez - 27/05/2025 - Log verboso
                    If LogVerbose Then
                        logprovder.Scrivi_LOG(objParametriServer, nomeRoutine, "Prima di TestaPoligono", False)
                    End If
                    Dim esitoOrientamento As Boolean =
                        xTest.TestaPoligono(Dati_GIS_ElementoGrafico.Cartography)
                    'Lavez - 27/05/2025 - Log verboso
                    If LogVerbose Then
                        logprovder.Scrivi_LOG(objParametriServer, nomeRoutine, "Dopo di TestaPoligono", False)
                    End If

                    If Not esitoOrientamento Then
                        Dati_GIS_ElementoGrafico.Cartography = AgronicaGIS2012.Commons.PolygonOrder.InvertiPoligono_wkt(Dati_GIS_ElementoGrafico.Cartography, False)
                    End If

                    'conversione del dato cartografico
                    elemento.Poligono_GeoEntity = Entity.Spatial.DbGeography.PolygonFromText(Dati_GIS_ElementoGrafico.Cartography, 4326)
                ElseIf type.Equals("LINESTRING") Then
                    Dim ef_GIS_elementiGrafici = New EF_GIS_ElementiGrafici
                    'Dim new_wkt = ef_GIS_elementiGrafici.SwapLatLngInWKT(Dati_GIS_ElementoGrafico.Cartography)
                    elemento.Poligono_GeoEntity = Entity.Spatial.DbGeography.LineFromText(Dati_GIS_ElementoGrafico.Cartography, 4326)
                ElseIf type.Equals("MULTIPOLYGON") Then
                    Dim ef_GIS_elementiGrafici = New EF_GIS_ElementiGrafici
                    'Dim new_wkt = ef_GIS_elementiGrafici.SwapLatLngInWKT(Dati_GIS_ElementoGrafico.Cartography)
                    elemento.Poligono_GeoEntity = Entity.Spatial.DbGeography.MultiPolygonFromText(Dati_GIS_ElementoGrafico.Cartography, 4326)
                End If

            End If
            elemento.Flag_GPS = Dati_GIS_ElementoGrafico.Flag_GPS
            elemento.ElementoGrafico_Des = Dati_GIS_ElementoGrafico.ElementoGraficoDes
            elemento.Data_Modifica = DateTime.Now
            elemento.Username_Modifica = objParametriServer.UtenteUsername
            elemento.inviato = 0
            elemento.datainvio = Nothing

            GiasContext.GIS_ElementiGrafici.Attach(elemento)
            GiasContext.Entry(elemento).State = Entity.EntityState.Modified

            GiasContext.SaveChanges()

            'Lavez - 27/05/2025 - Log verboso
            If LogVerbose Then
                logprovder.Scrivi_LOG(objParametriServer, nomeRoutine, "Prima di SaveChanges", False)
            End If

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return elemento


    End Function

    Public Shared Sub GIS_ElementoGrafico_Cancella_EF(ByVal Dati_GIS_ElementoGrafico As ElementiGrafici,
                                                        ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                        ByVal username As String,
                                                        Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                        Optional ByVal NewTransaction As Boolean = True
                                                        )

        Dim nomeRoutine As String = "AgronicaCoreGisDAL.EF_GIS_ElementiGrafici.GIS_ElementoGrafico_Cancella_EF()"
        Dim messaggioErrore As String = ""

        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Dim elementiList = From ele In GiasContext.GIS_ElementiGrafici
                           Where ele.PivaSuperUser = Dati_GIS_ElementoGrafico.PivaSuperUser AndAlso
                                 ele.ElementoGrafico_Cod = Dati_GIS_ElementoGrafico.ElementoGraficoCod
                           Select ele

        Dim elemento = elementiList.FirstOrDefault

        Try

            GiasContext.GIS_ElementiGrafici.Attach(elemento)
            GiasContext.GIS_ElementiGrafici.Remove(elemento)
            GiasContext.SaveChanges()

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

    End Sub
End Class
