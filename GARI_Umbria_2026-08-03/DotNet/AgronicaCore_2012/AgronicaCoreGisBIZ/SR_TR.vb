Imports System.Web.UI.WebControls
Imports AgronicaConversioneCartografiaGias.Agronica
Imports AgronicaConversioneCartografiaGias.FormatsConverter
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieBIZ
Imports AgronicaGIS2012.Commons
Imports Newtonsoft.Json.Linq

Public Class SR_TR

    Public Shared Function getGmlDaWKT(swktIn As String, ParametriCartografici As ParametriCoordinateConverter, prj As CoordinateConverter, trasformaSistemaRiferimento As Boolean, swaplatlng As Boolean) As String

        Dim wktToGeoML As New wkt_gml
        wktToGeoML.Soglia_ConsideraPuntiUguali = 0.000000005


        Dim sNodeDoc As XDocument = XDocument.Parse(
                            wktToGeoML.Trasforma(
                                swktIn,
                                swaplatlng,
                                trasformaSistemaRiferimento,
                                trasformaSistemaRiferimento,
                                0
                            )
                        )

        Return sNodeDoc.ToString()
    End Function

    Public Shared Function getWKT(X As String, Y As String, ParametriCartografici As ParametriCoordinateConverter, prj As CoordinateConverter) As String
        Dim lxyz As New List(Of xyz)
        Dim dX As Double = Double.Parse(X.Replace(".", ","))
        Dim dY As Double = Double.Parse(Y.Replace(".", ","))
        lxyz.Add(New xyz With {.X = dX, .Y = dY})

        Dim ruota As Boolean = False
        If lxyz.Count > 2 Then
            ruota = True
        End If

        Dim wktHelp As New WKT
        Dim sWKT As String = prj.WKTPolygonWGS84_from_WKTPolygonED50(wktHelp.CreaPoligonoDaCoordinate(lxyz), ruota, ParametriCartografici)
        Return sWKT
    End Function

    Public Function Carica(GEORiferimento_COD As Integer, ByVal objParametri_Server As AgronicaCoreParametri) As ParametriCoordinateConverter
        Dim leggiTrasformazione As New AgronicaCoreGisDAL.GIS_SistemiRiferimentoCartografia_R
        Dim dtLeggiTrasformazione As DataTable = leggiTrasformazione.Leggi(GEORiferimento_COD, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
        Dim cconverter As New CoordinateConverter
        Dim rval As New ParametriCoordinateConverter With {
            .CSFromText = dtLeggiTrasformazione(0)("CSFrom"),
            .CStoText = dtLeggiTrasformazione(0)("CSTo"),
            .CStoGeoText = dtLeggiTrasformazione(0)("CStoGeo"),
            .AgronicaLatOffset = dtLeggiTrasformazione(0)("AgronicaLatOffset"),
            .AgronicaLonOffset = dtLeggiTrasformazione(0)("AgronicaLonOffset"),
            .LibreriaDaUsare = dtLeggiTrasformazione(0)("LibreriaDaUsare"),
            .Descrizione = dtLeggiTrasformazione(0)("Descrizione"),
            .Note = dtLeggiTrasformazione(0)("Note"),
            .GEORiferimento_COD = dtLeggiTrasformazione(0)("GEORiferimento_COD")
        }
        Return rval
    End Function

    Public Sub CaricaComboSistemiRiferimento(s1 As DropDownList, tipologia As Integer, objParamentri_server As AgronicaCoreParametri, objParamentri_utenti As AgronicaCoreParametri)


        Dim leggi As New AgronicaCoreGisDAL.GIS_SistemiRiferimentoCartografia_R
        Dim dtLeggi As DataTable
        dtLeggi = leggi.Leggi(
            0,
            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
            "",
            "",
            objParamentri_server
        )

        s1.Items.Clear()

        Dim tr As Integer = getTrasformazioneFromTipologia(tipologia, objParamentri_utenti)


        Dim l As ListItem
        l = New ListItem With {.Value = "-1", .Text = "Nessuna Trasformazione"}
        s1.Items.Add(l)

        For Each drow In dtLeggi.Rows
            l = New ListItem With {.Value = drow("GEORiferimento_COD"), .Text = drow("descrizione")}
            If drow("GEORiferimento_COD") = tr Then
                l.Selected = True
            End If
            s1.Items.Add(l)

        Next

    End Sub

    Public Function LeggiSistemiRiferimento(ByVal tipologia As Integer,
                                            ByRef objParamentri_server As AgronicaCoreParametri,
                                            ByRef objParamentri_utenti As AgronicaCoreParametri) As String


        Dim xRead As New AgronicaCoreGisDAL.GIS_SistemiRiferimentoCartografia_R
        Dim tempString As String

        Dim resp As New JObject

        Dim DT = xRead.Leggi(0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                             "", "", objParamentri_server)

        Dim trasformazione As Integer = getTrasformazioneFromTipologia(tipologia, objParamentri_utenti)

        resp.Add("default", trasformazione.ToString)

        Dim listaSistemiRiferimento As New JArray

        tempString = "{""Value"" : ""-1"", ""Text"" : ""Nessuna Trasformazione""}"

        listaSistemiRiferimento.Add(JObject.Parse(tempString))

        For Each row In DT.Rows

            tempString = String.Format("{{""Value"" : ""{0}"", ""Text"" : ""{1}""}}",
                                       row("GEORiferimento_COD").ToString,
                                       row("descrizione").ToString)

            listaSistemiRiferimento.Add(JObject.Parse(tempString))
        Next

        resp.Add("ListaSistemiRiferimento", listaSistemiRiferimento)

        Return Newtonsoft.Json.JsonConvert.SerializeObject(resp)

    End Function

    Private Function cfgGisCaricaSistemaRiferimentoDaImpostazioni(objParametri_Utenti As AgronicaCoreParametri) As Integer



        Dim utentiImpostazioni_R As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim dt As DataTable
        dt = utentiImpostazioni_R.Leggi(enum_Impostazioni_Utenti.UTENTE_GIS, 1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

        Dim valorePredefinito As String = "-1"
        If dt.Rows.Count > 0 Then
            Dim imposta As String = dt(0)("Impostazione_Valore_1")
            Dim vImposta As String() = imposta.Split("§")
            For Each impostazione In vImposta
                If impostazione.Contains("SistemaDiRiferimentoPredefinito") Then
                    valorePredefinito = impostazione.Split("^")(1)
                End If
            Next
        End If

        Return valorePredefinito

    End Function

    Private Function getTrasformazioneFromTipologia(tipologia As Integer, objParamentri_utenti As AgronicaCoreParametri) As Integer

        Dim sistemaPredefinito As Integer = cfgGisCaricaSistemaRiferimentoDaImpostazioni(objParamentri_utenti)

        If sistemaPredefinito <> -1 Then
            Return sistemaPredefinito
        End If

        Select Case tipologia
            Case 1
            Case 2
            Case 3
            Case 4
                Return 3
            Case 8
                Return 0
            Case Else
                Return 1

        End Select

    End Function

End Class

Public Class SR_TR_W
    Public Function Scrivi() As RispostaStandard

    End Function
End Class