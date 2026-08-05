Imports System.Web.Script.Serialization
Imports AgronicaCoreGestioneRichieste


Public Class Fertilizzanti_WS

    Public Function Fertilizzanti(ByVal Input As AgronicaCoreMetaSchemaBIZ.Fertilizzanti_input, Optional ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = Nothing, Optional ByRef objParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri = Nothing, Optional ByVal leggiUrlDaConfigurazioniSiti As Boolean = False) As AgronicaCoreMetaSchemaBIZ.Fertilizzanti_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        If Input.Url <> "" Then
            url = Input.Url
        Else
            If Not leggiUrlDaConfigurazioniSiti Then
                Dim objAgroWebConfig As New AgroWebConfig
                url = objAgroWebConfig.GiasOnline_WS_Fertilizzanti_IAgroAPI_Fertilizzanti
            End If

            If leggiUrlDaConfigurazioniSiti OrElse String.IsNullOrEmpty(url) Then
                Dim objConfig As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                url = objConfig.Leggi_Valore(0, "GiasOnline_WS_Fertilizzanti_IAgroAPI_Fertilizzanti", "", "", objParametri_Server)

                If url = "" AndAlso objParametri_SuperServer IsNot Nothing Then
                    url = objConfig.Leggi_Valore(0, "GiasOnline_WS_Fertilizzanti_IAgroAPI_Fertilizzanti", "", "", objParametri_SuperServer)
                End If
            End If
            url &= "/Fertilizzanti"
        End If


        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCoreMetaSchemaBIZ.Fertilizzanti_output =
            jss.Deserialize(Of AgronicaCoreMetaSchemaBIZ.Fertilizzanti_output)(rval)

        Return Output

    End Function


    ' richiamata da RiepilogoProdotti.aspx (report Riepilogo Prodotti Utilizzati)
    'tipo = PUA_RegolamentoCod
    'regolamento = PUA_RegolamentoCod
    Public Function RecuperaDT_FertilizzantiConDitteConcatenate_daWS(ByVal URL_WS_Fert As String, _
                                                                     ByVal Fer_Cod As Integer, _
                                                                      ByVal tipo As Integer,
                                                                      ByVal regolamento As Integer,
                                                                      ByVal FlagIncludiApporti As Boolean,
                                                                      ByVal FlagIncludiTipologia As Boolean,
                                                                      ByVal Data_inizio As Date,
                                                                      ByVal Data_fine As Date,
                                                                      ByVal FiltroAgg As String) As DataTable

        Dim Dt As New DataTable

        Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.Fertilizzanti_input With {
            .Codice = Fer_Cod,
            .Descrizione = "",
            .DataInizio = Data_inizio,
            .DataFine = Data_fine,
            .Tipo = tipo,
            .IncludiApporti = FlagIncludiApporti,
            .IncludiTipologia = FlagIncludiTipologia,
            .Regolamento = regolamento,
            .strFiltro = FiltroAgg
        }

        objParametriIngresso.Url = URL_WS_Fert

        Dim objParametriUscita As AgronicaCoreMetaSchemaBIZ.Fertilizzanti_output
        Dim objFert_WS As New AgronicaCoreWebService.Fertilizzanti_WS
        objParametriUscita = objFert_WS.Fertilizzanti(objParametriIngresso)

        Dim ListaDitte As List(Of AgronicaCoreMetaSchemaBIZ.Ditta)
        Dim DrFine As DataRow
        Dt.Columns.Add(New DataColumn("Fer_cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Ditta", GetType(String)))


        If Not objParametriUscita.ListaFertilizzanti Is Nothing AndAlso objParametriUscita.ListaFertilizzanti.Count > 0 Then

            Dim StrDitte As String = ""
            For Each Fertilizzante In objParametriUscita.ListaFertilizzanti
                fer_cod = Fertilizzante.Codice
                ListaDitte = Fertilizzante.ListaDitte

                For Each Ditta In ListaDitte
                    StrDitte = Ditta.Descrizione & ", "
                Next
                If StrDitte <> "" Then
                    StrDitte = Mid(StrDitte, 1, StrDitte.Length - 1)
                End If

                DrFine = Dt.NewRow
                DrFine("Fer_cod") = Fer_Cod
                DrFine("Ditta") = StrDitte
                Dt.Rows.Add(DrFine)

            Next

        End If

        'If objParametriUscita.MessaggioErrore = "" Then


        'End If

        Return Dt

    End Function



End Class
