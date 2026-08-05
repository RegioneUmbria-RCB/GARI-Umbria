Imports System.Net
Imports Newtonsoft.Json.Linq

Public Class AgroFascicolo

    Public Function ScaricaAgroFascicolo(PivaSuperUser As String,
                                         username As String,
                                         password As String,
                                         Ente_Cod As Integer,
                                         CUAA As String,
                                         Numero_Validazione As String,
                                         Parametri_Extra As String,
                                         Leggi_Cache As Boolean,
                                         ObjParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                         ByRef ErrCod As Integer,
                                         ByRef ErrMessage As String,
                                         ByRef url As String,
                                         Optional timeout As Long = -1) As String

        'Dim importa As FascicoloManager
        Dim hlpHttp As New AgronicaCoreUtility.Http
        Dim hdr As System.Net.WebHeaderCollection = Nothing
        Dim auth = "bearer: Gj9CGUSgX0EyuWoO2LA9vDr28LV3Fy7B"
        If url = "" Then
            Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Try
                url = objConfSiti.Leggi_Valore(16, "AgroFascicolo_WS", "", "", ObjParametri)
            Catch ex As Exception
                url = ""
            End Try
            If url = "" Then
                url = "https://ws.netagronica.it/WS_AgroFascicoloBA/FascicoloManager.svc"
            End If
        End If


        url &= "/GetFascicolo"

        Dim j = New JObject(
            New JProperty("PivaSuperUser", PivaSuperUser),
            New JProperty("username", username),
            New JProperty("password", password),
            New JProperty("Ente_Cod", Ente_Cod),
            New JProperty("Numero_Validazione", Numero_Validazione),
            New JProperty("CUAA", CUAA),
            New JProperty("Parametri_Extra", CStr(Parametri_Extra)),
            New JProperty("Leggi_Cache", CBool(Leggi_Cache))
        )

        Dim s As String = j.ToString()
        If auth <> "" Then
            hdr = New System.Net.WebHeaderCollection
            hdr.Add("Authorization", auth)
        End If
        Dim rval As String = Nothing
        Try

            Dim pSecur As SecurityProtocolType = ServicePointManager.SecurityProtocol
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

            Dim timeOutVal = Integer.MaxValue
            If timeout <> -1 Then
                timeOutVal = timeout
            End If

            rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "", hdr, timeOutVal)

            ServicePointManager.SecurityProtocol = pSecur

        Catch ex As Exception
            Dim i = 0
            i += 1
        End Try

        If rval IsNot Nothing Then
            Dim jRval = JValue.Parse(rval)

            If jRval.Item("ErrCOD") IsNot Nothing AndAlso IsNumeric(jRval.Item("ErrCOD")) Then
                ErrCod = CInt(jRval.Item("ErrCOD"))
            End If

            If jRval.Item("ErrMsg") IsNot Nothing Then
                ErrMessage = jRval.Item("ErrMsg")
            End If

            Return jRval.Item("fascicolo")
        End If

        Return ""

    End Function

    Public Function GetSchede(PivaSuperUser As String,
                              username As String,
                              password As String,
                              Ente_Cod As Integer,
                              CUAA As String,
                              Parametri_Extra As String,
                              ObjParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                              ByRef ErrCod As Integer,
                              ByRef ErrMessage As String,
                              url As String) As String

        'Dim importa As FascicoloManager
        Dim hlpHttp As New AgronicaCoreUtility.Http

        Dim hdr As System.Net.WebHeaderCollection = Nothing
        Dim auth = "bearer: Gj9CGUSgX0EyuWoO2LA9vDr28LV3Fy7B"
        Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        If url = "" Then
            Try
                url = objConfSiti.Leggi_Valore(16, "AgroFascicolo_WS", "", "", ObjParametri)
            Catch ex As Exception
                url = ""
            End Try
            If url = "" Then
                url = "https://ws.netagronica.it/WS_AgroFascicoloBA/FascicoloManager.svc"
            End If

        End If

        url &= "/GetSchede"

        Dim j = New JObject(
            New JProperty("PivaSuperUser", PivaSuperUser),
            New JProperty("username", username),
            New JProperty("password", password),
            New JProperty("Ente_Cod", Ente_Cod),
            New JProperty("CUAA", CUAA),
            New JProperty("Parametri_Extra", CStr(Parametri_Extra))
        )

        Dim s As String = j.ToString()
        If auth <> "" Then
            hdr = New System.Net.WebHeaderCollection
            hdr.Add("Authorization", auth)
        End If
        Dim rval As String = Nothing
        Try
            rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "", hdr, Integer.MaxValue)
        Catch ex As Exception
            Dim i = 0
            i += 1
        End Try

        If rval IsNot Nothing Then

            Dim jRval = JValue.Parse(rval)

            If jRval.Item("ErrCOD") IsNot Nothing AndAlso IsNumeric(jRval.Item("ErrCOD")) Then
                ErrCod = CInt(jRval.Item("ErrCOD"))
            End If

            If jRval.Item("ErrMsg") IsNot Nothing Then
                ErrMessage = jRval.Item("ErrMsg")
            End If

            Return jRval.Item("schede").ToString
        End If

        Return ""

    End Function

    Public Function GetCuaaModificata(PivaSuperUser As String,
                              username As String,
                              password As String,
                              Ente_Cod As Integer,
                              Giorni As Integer,
                              Parametri_Extra As String,
                              ObjParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                              ByRef ErrCod As Integer,
                              ByRef ErrMessage As String,
                              url As String) As String

        'Dim importa As FascicoloManager
        Dim hlpHttp As New AgronicaCoreUtility.Http

        Dim hdr As System.Net.WebHeaderCollection = Nothing
        Dim auth = "bearer: Gj9CGUSgX0EyuWoO2LA9vDr28LV3Fy7B"
        Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        If url = "" Then

            Try
                url = objConfSiti.Leggi_Valore(16, "AgroFascicolo_WS", "", "", ObjParametri)
            Catch ex As Exception
                url = ""
            End Try
            If url = "" Then
                url = "https://ws.netagronica.it/WS_AgroFascicoloBA/FascicoloManager.svc"
            End If

        End If

        url &= "/GetCuaaModificata"

        Dim j = New JObject(
            New JProperty("PivaSuperUser", PivaSuperUser),
            New JProperty("username", username),
            New JProperty("password", password),
            New JProperty("Giorni", Giorni),
            New JProperty("Parametri_Extra", CStr(Parametri_Extra))
            )
        'New JProperty("username", username),
        'New JProperty("password", password),
        'New JProperty("Ente_Cod", Ente_Cod),
        'New JProperty("CUAA", CUAA),
        'New JProperty("Parametri_Extra", CStr(Parametri_Extra))
        ')

        Dim s As String = j.ToString()
        If auth <> "" Then
            hdr = New System.Net.WebHeaderCollection
            hdr.Add("Authorization", auth)
        End If
        Dim rval As String = Nothing
        Dim pSecur As SecurityProtocolType = ServicePointManager.SecurityProtocol
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

        Try
            rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "", hdr, Integer.MaxValue)
        Catch ex As Exception
            Dim i = 0
            i += 1
        End Try

        ServicePointManager.SecurityProtocol = pSecur

        If rval IsNot Nothing Then

            Dim jRval = JValue.Parse(rval)

            If jRval.Item("ErrCOD") IsNot Nothing AndAlso IsNumeric(jRval.Item("ErrCOD")) Then
                ErrCod = CInt(jRval.Item("ErrCOD"))
            End If

            If jRval.Item("ErrMsg") IsNot Nothing Then
                ErrMessage = jRval.Item("ErrMsg")
            End If

            Return jRval.Item("cuaaModificata").ToString
        End If


        Return ""

    End Function

End Class
