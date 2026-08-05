Imports System.Text
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class XML_Export_Log_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal piva As String,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreXMLUniversale.XML_Export_Log_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM XML_Export_Log ")
            stb.AppendLine(" WHERE 1=1 ")

            If piva <> "" Then
                stb.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    stb.AppendLine(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    stb.AppendLine(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

End Class


'#################################################################
'#################################################################
'#################################################################

Public Class XML_Export_Log_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Scrivi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           ByVal piva As String,
                           ByVal idAgenda As Integer,
                           ByVal stato As enum_WFlow_Export_XML_Universale,
                           ByVal dataOperazione As Date,
                           Optional ByVal dataCreazione As Date = #2/1/1900#,
                           Optional ByVal dataModifica As Date = #2/1/1900#,
                           Optional ByVal usernameCreazione As String = "",
                           Optional ByVal usernameModifica As String = ""
                           ) As Boolean


        Dim nomeRoutine As String = "AgronicaCoreXMLUniversale.XML_Export_Log_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If dataCreazione = #2/1/1900# Then
                dataCreazione = Date.Now
            End If

            If dataModifica = #2/1/1900# Then
                dataModifica = Date.Now
            End If

            If usernameCreazione = "" Then
                usernameCreazione = objParametri.UsernameOperazione
            End If

            If usernameModifica = "" Then
                usernameModifica = objParametri.UsernameOperazione
            End If
            

            '---------------------------------------------
            stb.Length = 0
            stb.AppendLine(" INSERT INTO XML_Export_Log ")

            stb.AppendLine("              (")

            stb.AppendLine("              Piva,         Id_Agenda,            Stato,      Data_Operazione, ")

            stb.AppendLine("              Inviato,            datainvio, ")
            stb.AppendLine("              Data_Creazione,     Data_Modifica, ")
            stb.AppendLine("              UserName_Creazione, UserName_Modifica, ")
            stb.AppendLine("              Validita_Inizio,    Validita_Fine ")
            stb.AppendLine("              ) ")

            stb.AppendLine(" VALUES ( ")

            stb.AppendLine("		    '" & Agro_SQL_SaveText(piva) & "'  ")
            stb.AppendLine("			, " & Agro_SQL_SaveNum(idAgenda) & "  ")
            stb.AppendLine("			, " & Agro_SQL_SaveNum(stato) & "  ")
            stb.AppendLine("			, " & Agro_SQL_SaveDateTime(dataOperazione) & "  ")

            stb.AppendLine("         , 0  ")
            stb.AppendLine("         , Null  ")

            stb.AppendLine("			, " & Agro_SQL_SaveDateTime(dataCreazione) & "  ")
            stb.AppendLine("			, " & Agro_SQL_SaveDateTime(dataModifica) & "  ")
            stb.AppendLine("			,'" & Agro_SQL_SaveText(usernameCreazione) & "' ")
            stb.AppendLine("			,'" & Agro_SQL_SaveText(usernameModifica) & "' ")
            stb.AppendLine("			, " & Agro_SQL_SaveDate(AGRODATAINIZIO) & "  ")
            stb.AppendLine("			, " & Agro_SQL_SaveDate(AGRODATAFINE) & "  ")

            stb.AppendLine(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function

End Class
