Public Class Fixes

    Public Shared Function FixData(dataAggiornamento As String) As String
        Dim newData As String = Trim(dataAggiornamento)

        If IsDate(newData) Then
            newData = CDate(newData).ToString("yyyy-MM-dd")
        End If

        Return newData
    End Function

    Public Shared Function FixProdottoRequisito(titolo As String) As String
        Dim newTitolo As String = Trim(titolo)

        newTitolo = newTitolo.Replace("GiasBase", "Gias Base")
        newTitolo = newTitolo.Replace("GIAS BASE", "Gias Base")
        newTitolo = newTitolo.Replace("Configurazione_Siti", "Configurazione Siti")
        newTitolo = newTitolo.Replace("Agronica Meteo Web Service", "Meteo WS")
        newTitolo = newTitolo.Replace("Meteo Web Service", "Meteo WS")
        newTitolo = newTitolo.Replace("METEO WS", "Meteo WS")
        newTitolo = newTitolo.Replace("CORE API", "Core API")
        newTitolo = newTitolo.Replace("COREAPI", "Core API")
        newTitolo = newTitolo.Replace("CoreAPI", "Core API")
        newTitolo = newTitolo.Replace("AgronicaCoreAPI", "Core API")
        newTitolo = newTitolo.Replace("CORE WS", "Core WS")
        newTitolo = newTitolo.Replace("AgronicaCore", "Core WS")
        newTitolo = newTitolo.Replace("AgroProfilazione", "Profilazione")
        newTitolo = newTitolo.Replace("AgronicaWebService2010", "WEB Services")
        newTitolo = newTitolo.Replace("WebService2010", "WEB Services")
        newTitolo = newTitolo.Replace("WebService", "WEB Services")
        newTitolo = newTitolo.Replace("Web Service", "WEB Services")

        If newTitolo.EndsWith(":") Then
            newTitolo = Trim(newTitolo.TrimEnd(":"))
        End If

        If newTitolo.EndsWith(" ''") Then
            newTitolo = newTitolo.Remove(newTitolo.Length-(" ''").Length, (" ''").Length)
        End If

        Return Trim(newTitolo)
    End Function

    Public Shared Function FixArea(titolo As String) As String
        Dim newTitolo As String = Trim(titolo)

        If newTitolo.EndsWith(":") Then
            newTitolo = newTitolo.Substring(0, newTitolo.Length - 1)
            'newTitolo = newTitolo.Remove(newTitolo.Length - 2, 1)
        End If

        Return Trim(newTitolo)
    End Function

    Public Shared Function FixDescrizione(testo As String) As String
        Return Trim(testo.Replace(vbCrLf, "<br>"))
    End Function

End Class
