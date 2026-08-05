Imports System.Text
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class XML_Export_Attuali_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal piva As String,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreXMLUniversale.XML_Export_Attuali_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM XML_Export_Attuali ")
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


    '##############################################################################################
    Public Function Leggi_Stato(ByVal piva As String,
                                ByVal stato As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreXMLUniversale.XML_Export_Attuali_R.Leggi_Stato()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM XML_Export_Attuali ")
            stb.AppendLine(" WHERE Stato = " & Agro_SQL_SaveNum(stato))

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


    '##############################################################################################
    Public Function Leggi_Id_Agenda(ByVal piva As String,
                                    ByVal idAgenda As Integer,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreXMLUniversale.XML_Export_Attuali_R.Leggi_Id_Agenda()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM XML_Export_Attuali ")
            stb.AppendLine(" WHERE Id_Agenda = " & Agro_SQL_SaveNum(idAgenda))

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


    '##############################################################################################
    Public Function Trova_Id_Agenda_Mancanti(ByVal piva As String,
                                             ByVal xOrderBy As String,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                             ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreXMLUniversale.XML_Export_Attuali_R.Trova_Id_Agenda_Mancanti()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT Piva, Id_Agenda ")
            stb.AppendLine(" FROM Agenda ")

            If piva <> "" Then
                stb.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            stb.AppendLine(" Except ")
            stb.AppendLine(" SELECT Piva, Id_Agenda ")
            stb.AppendLine(" FROM XML_Export_Attuali ")

            If Piva <> "" Then
                stb.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If
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

    '##############################################################################################
    ''' <summary>
    ''' Trova gli id agenda esportabili (volendo filtrati per Piva)
    ''' </summary>
    ''' <param name="Piva">OPZIONALE</param>
    ''' <param name="lavCod"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns>DataTable</returns>
    ''' <remarks></remarks>
    Public Function Trova_Id_Agenda_Esportabili(ByVal piva As String,
                                                ByVal lavCod As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreXMLUniversale.XML_Export_Attuali_R.Trova_Id_Agenda_Esportabili()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine("SELECT x.Piva, x.Id_Agenda ")
            stb.AppendLine(" FROM XML_Export_Attuali x ")
            stb.AppendLine(" INNER JOIN Agenda a ON a.Id_Agenda = x.Id_Agenda ")
            stb.AppendLine(" WHERE x.Stato = 251 ")
            stb.AppendLine(" AND a.lav_cod IN (" & Agro_SQL_Save_Clausola_IN(lavCod) & ")")


            If piva <> "" Then
                stb.AppendLine(" AND a.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
                stb.AppendLine(" AND x.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If
            
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

Public Class XML_Export_Attuali_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Scrivi(ByVal piva As String,
                           ByVal idAgenda As Integer,
                           ByVal stato As Integer,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal dataCreazione As Date = #2/1/1900#,
                           Optional ByVal dataModifica As Date = #2/1/1900#,
                           Optional ByVal usernameCreazione As String = "",
                           Optional ByVal usernameModifica As String = ""
                           ) As Boolean


        Dim nomeRoutine As String = "AgronicaCoreXMLUniversale.XML_Export_Attuali_W.Scrivi()"

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
            stb.AppendLine(" INSERT INTO XML_Export_Attuali ")

            stb.AppendLine("              (")
            stb.AppendLine("              Piva,             Id_Agenda,          Stato, ")

            stb.AppendLine("              Inviato,            datainvio, ")
            stb.AppendLine("              Data_Creazione,     Data_Modifica, ")
            stb.AppendLine("              UserName_Creazione, UserName_Modifica, ")
            stb.AppendLine("              Validita_Inizio,    Validita_Fine ")
            stb.AppendLine("              ) ")

            stb.AppendLine(" VALUES ( ")

            stb.AppendLine("		  '" & Agro_SQL_SaveText(piva) & "'  ")
            stb.AppendLine("         , " & Agro_SQL_SaveNum(idAgenda) & "  ")
            stb.AppendLine("         , " & Agro_SQL_SaveNum(stato) & "  ")

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

    '#################################################################
    Public Function Aggiorna_Id_Agenda(ByVal piva As String,
                                       ByVal idAgenda As Integer,
                                       ByVal stato As Integer,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                       ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreXMLUniversale.XML_Export_Attuali_W.Aggiorna_Id_Agenda()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            stb.Length = 0

            stb.AppendLine(" UPDATE XML_Export_Attuali ")
            stb.AppendLine(" SET ")
            stb.AppendLine("           Stato = " & Agro_SQL_SaveNum(stato) & " ")
            stb.AppendLine("         , Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            stb.AppendLine("         , Data_Modifica = " & Agro_SQL_SaveDateTime(Date.Now) & " ")
            stb.AppendLine(" WHERE   Id_Agenda = " & Agro_SQL_SaveNum(idAgenda) & " ")
            stb.AppendLine(" AND   Piva = '" & Agro_SQL_SaveText(piva) & "' ")

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

