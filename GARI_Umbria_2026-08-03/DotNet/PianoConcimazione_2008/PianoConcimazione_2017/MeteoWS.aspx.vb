Imports System.Web.Services
Imports AgronicaConversioneCartografiaGias.Agronica
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json.Linq

Public Class MeteoWS
    Inherits System.Web.UI.Page


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function RicaricaSorgenteDati() As RispostaStandard

        Dim risp As New RispostaStandard With {.RispostaOK = False}

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            risp.Sessione = False
            Return risp
        End If

        'Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim objParams As New JObject(
                New JProperty("PIVA_Superuser", objParametri_Server.PivaSuperUser),
                New JProperty("RilievoPiogge", True)
                )

            Dim objMeteoNT As New AgronicaCoreWebService.MeteoNT

            Dim json_risp = JObject.Parse(objMeteoNT.ElencoSorgentiMeteo(objParams, objParametri_Server))

            Dim elenco = JArray.Parse(json_risp("RispostaStringa"))

            'For Each s As JObject In elenco

            '    Try

            '        Dim resx As String = "SorgenteMeteo_" & CInt(s("sorgente_cod")).ToString("000")
            '        Dim out_des As String = AgronicaAgenda_2010.ResourceManager.GetString(resx, AgronicaAgenda_2010.Culture)
            '        If out_des IsNot Nothing Then

            '            s("sorgente_des") = out_des
            '        End If

            '    Catch ex As Exception

            '    End Try
            'Next

            risp.RispostaStringa = elenco.ToString()
            risp.RispostaOK = True

        Catch ex As Exception

            risp.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return risp
    End Function


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function RicaricaOrigineDati(ByVal PIVA As String, ByVal SaCod As Integer, ByVal TipoSorgenteDati As Integer) As RispostaStandard

        Dim risp As New RispostaStandard With {.RispostaOK = False}

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            risp.Sessione = False
            Return risp
        End If

        'Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim Lat As Decimal = 0
            Dim Lng As Decimal = 0

            Dim objCentri = New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
            Dim DtCentri = objCentri.CentroAziendaliLatLongDescrizione(PIVA, SaCod, objParametri_Server)

            Dim leggiAncora As Boolean = False
            If DtCentri Is Nothing OrElse DtCentri.Rows.Count = 0 Then

                leggiAncora = True
            Else
                Lat = CDec(DtCentri.Rows(0)("lat"))
                Lng = CDec(DtCentri.Rows(0)("Long"))

                If Lat <= 0 OrElse Lng <= 0 Then
                    leggiAncora = True
                End If
            End If

            'La conversione in WKTPolygonWGS84_from_WKTPolygonED50 non funziona -cit. Gabri
            leggiAncora = False
            If leggiAncora Then
                DtCentri = objCentri.Leggi_Coordinate(PIVA, SaCod, "", "", objParametri_Server)
                If DtCentri IsNot Nothing AndAlso DtCentri.Rows.Count > 0 Then

                    Dim utm_x As Integer = DtCentri.Rows(0)("X")
                    Dim utm_y As Integer = DtCentri.Rows(0)("Y")
                    If utm_x > 0 AndAlso utm_y > 0 Then

                        Dim geoRifCod As Integer = 1
                        Dim leggiTxform As New AgronicaCoreGisDAL.GIS_SistemiRiferimentoCartografia_R
                        Dim dtLeggiTrasformazione As DataTable = leggiTxform.Leggi(geoRifCod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                        Dim cconverter As New CoordinateConverter
                        Dim ParametriCartograficiWGS84ED50 As New ParametriCoordinateConverter With {
                            .CSFromText = dtLeggiTrasformazione(0)("CSFrom"),
                            .CStoText = dtLeggiTrasformazione(0)("CSTo"),
                            .CStoGeoText = dtLeggiTrasformazione(0)("CStoGeo"),
                            .AgronicaLatOffset = dtLeggiTrasformazione(0)("AgronicaLatOffset"),
                            .AgronicaLonOffset = dtLeggiTrasformazione(0)("AgronicaLonOffset"),
                            .LibreriaDaUsare = dtLeggiTrasformazione(0)("LibreriaDaUsare")
                        }

                        Dim CoordWkt As String = "((" & utm_x & "," & utm_y & "))"

                        Dim risultato As String = cconverter.WKTPolygonWGS84_from_WKTPolygonED50(CoordWkt, True, ParametriCartograficiWGS84ED50)
                        Dim aRis() = risultato.Split(" ")
                        Dim sRis1 = aRis(0).Replace("(", "")
                        Dim sRis2 = aRis(1).Replace(")", "")
                        Lng = Convert.ToDecimal(sRis1, Globalization.CultureInfo.InvariantCulture)
                        Lat = Convert.ToDecimal(sRis2, Globalization.CultureInfo.InvariantCulture)
                    End If
                End If
            End If

            Dim objParams As New JObject
            objParams("PIVA_Superuser") = objParametri_Server.PivaSuperUser
            objParams("PIVA") = PIVA
            objParams("TipoSorgente") = TipoSorgenteDati
            objParams("DistanzaDa") = New JObject(New JProperty("lat", Lat), New JProperty("lng", Lng))
            objParams("CodiceCentro") = SaCod

            Dim objMeteoNT As New AgronicaCoreWebService.MeteoNT

            Dim json = JObject.Parse(objMeteoNT.LeggiStazioniXSorgente(objParams, objParametri_Server))

            Dim jarr = JArray.Parse(json("RispostaStringa"))

            For Each jstaz As JObject In jarr

                If jstaz("distanza") IsNot Nothing Then

                    jstaz("distanza") = " Distanza " & CInt(CInt(jstaz("distanza")) / 1000) & " km"
                Else

                    jstaz.Add(New JProperty("distanza", " Distanza non disponibile"))
                End If
            Next

            risp.RispostaStringa = jarr.ToString

            risp.RispostaOK = True

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return risp
    End Function


End Class