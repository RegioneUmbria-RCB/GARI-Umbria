Imports System.Text
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class XML_Export_Mag_Attuali_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal piva As String,
                          ByVal idAgenda As Integer,
                          ByVal idMovDet As Integer,
                          ByVal cauMov As String,
                          ByVal nomeFile As String,
                          ByVal messaggioLike As String,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreXMLUniversale.XML_Export_Mag_Attuali_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM XML_Export_Mag_Attuali ")
            stb.AppendLine(" WHERE 1=1 ")

            If piva <> "" Then
                stb.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If idAgenda <> 0 Then
                stb.AppendLine(" AND Id_Agenda = " & Agro_SQL_SaveNum(idAgenda) & " ")
            End If

            If idMovDet <> 0 Then
                stb.AppendLine(" AND Id_Mov_Det = " & Agro_SQL_SaveNum(idMovDet) & " ")
            End If

            If cauMov <> "" Then
                stb.AppendLine(" AND Cau_Mov = '" & Agro_SQL_SaveText(cauMov) & "' ")
            End If

            If nomeFile <> "" Then
                stb.AppendLine(" AND NomeFile = '" & Agro_SQL_SaveText(nomeFile) & "' ")
            End If

            If messaggioLike <> "" Then
                stb.AppendLine(" AND Messaggio LIKE '%" & Agro_SQL_SaveText(messaggioLike) & "%' ")
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
    Public Function Leggi_Id_Mov_Det(ByVal piva As String,
                                     ByVal idAgenda As Integer,
                                     ByVal idMovDet As Integer,
                                     ByVal cauMov As String,
                                     ByVal xFiltroAggiuntivo As String,
                                     ByVal xOrderBy As String,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                     ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreXMLUniversale.XML_Export_Attuali_R.Leggi_Id_Mov_Det()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM XML_Export_Mag_Attuali ")
            stb.AppendLine(" WHERE Id_Mov_Det = " & Agro_SQL_SaveNum(idMovDet))

            If piva <> "" Then
                stb.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If idAgenda <> 0 Then
                stb.AppendLine(" AND Id_Agenda = " & Agro_SQL_SaveNum(idAgenda) & " ")
            End If

            If cauMov <> "" Then
                stb.AppendLine(" AND Cau_Mov = '" & Agro_SQL_SaveText(cauMov) & "' ")
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
    Public Function TrovaOperazioniModificate(ByVal cauMov As String,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String,
                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                              ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreXMLUniversale.XML_Export_Attuali_R.TrovaOperazioniModificate()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine("SELECT x.Piva, x.Id_Agenda, x.Id_Mov_Det, x.Cau_Mov ")
            stb.AppendLine(" FROM XML_Export_Mag_Attuali x ")
            stb.AppendLine(" WHERE x.Stato = " & enum_WFlow_Export_XML_Universale.Operazione_Esportata)
            stb.AppendLine("  ")
            'Tipo_Operazione 1 = Creazione
            stb.AppendLine(" AND (SELECT TOP 1 l.Tipo_Operazione ")
            stb.AppendLine("        FROM Agronica_Log_Agenda l ")
            stb.AppendLine("        WHERE l.Piva = x.Piva AND l.Id_Agenda = x.Id_Agenda ")
            stb.AppendLine("        ORDER BY Data_Ora_RegistrazioneLog DESC ")
            stb.AppendLine("        ) = 1 ")
            stb.AppendLine(" AND (SELECT TOP 1 l.Data_Ora_RegistrazioneLog ")
            stb.AppendLine("        FROM Agronica_Log_Agenda l ")
            stb.AppendLine("        WHERE l.Piva = x.Piva AND l.Id_Agenda = x.Id_Agenda ")
            stb.AppendLine("        ORDER BY Data_Ora_RegistrazioneLog DESC ")
            stb.AppendLine("        ) > x.Data_Modifica ")

            If cauMov <> "" Then
                stb.AppendLine(" AND Cau_Mov = '" & Agro_SQL_SaveText(cauMov) & "' ")
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
    Public Function Verifica_Esistenza_Record(ByVal piva As String,
                                              ByVal idAgenda As Integer,
                                              ByVal idMovDet As Integer,
                                              ByVal cauMov As String,
                                              ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                              ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreXMLUniversale.XML_Export_Mag_Attuali_R.Verifica_Esistenza_Record()"

        Dim messaggioErrore As String = ""
        Dim xRisp As Boolean = False
        Dim dt As DataTable

        Try

            dt = Leggi(piva, idAgenda, idMovDet, cauMov, "", "", "", "", objParametri)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                xRisp = True
            Else
                xRisp = False
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

End Class

'#################################################################
'#################################################################
'#################################################################

Public Class XML_Export_Mag_Attuali_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Scrivi(ByVal piva As String,
                           ByVal idAgenda As Integer,
                           ByVal idMovDet As Integer,
                           ByVal cauMov As String,
                           ByVal stato As Integer,
                           ByVal nomeFile As String,
                           ByVal messaggio As String,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal dataCreazione As Date = #2/1/1900#,
                           Optional ByVal dataModifica As Date = #2/1/1900#,
                           Optional ByVal usernameCreazione As String = "",
                           Optional ByVal usernameModifica As String = ""
                           ) As Boolean
        
        Dim nomeRoutine As String = "AgronicaCoreXMLUniversale.XML_Export_Mag_Attuali_W.Scrivi()"

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
            stb.AppendLine(" INSERT INTO XML_Export_Mag_Attuali ")

            stb.AppendLine("              (")
            stb.AppendLine("              Piva,             Id_Agenda,          Id_Mov_Det,     Cau_Mov, ")
            stb.AppendLine("              Stato,            NomeFile,          Messaggio, ")

            stb.AppendLine("              Inviato,            datainvio, ")
            stb.AppendLine("              Data_Creazione,     Data_Modifica, ")
            stb.AppendLine("              UserName_Creazione, UserName_Modifica, ")
            stb.AppendLine("              Validita_Inizio,    Validita_Fine ")
            stb.AppendLine("              ) ")

            stb.AppendLine(" VALUES ( ")


            stb.AppendLine("		  '" & Agro_SQL_SaveText(piva) & "' ")
            stb.AppendLine("         , " & Agro_SQL_SaveNum(idAgenda) & " ")
            stb.AppendLine("         , " & Agro_SQL_SaveNum(idMovDet) & " ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(cauMov) & "' ")

            stb.AppendLine("         , " & Agro_SQL_SaveNum(stato) & "  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(nomeFile) & "'  ")
            stb.AppendLine("         , '" & Agro_SQL_SaveText(messaggio) & "'  ")

            stb.AppendLine("         , 0  ")
            stb.AppendLine("         , Null  ")

            stb.AppendLine("		 , " & Agro_SQL_SaveDateTime(dataCreazione) & "  ")
            stb.AppendLine("		 , " & Agro_SQL_SaveDateTime(dataModifica) & "  ")
            stb.AppendLine("		 ,'" & Agro_SQL_SaveText(usernameCreazione) & "' ")
            stb.AppendLine("		 ,'" & Agro_SQL_SaveText(usernameModifica) & "' ")
            stb.AppendLine("		 , " & Agro_SQL_SaveDate(AGRODATAINIZIO) & "  ")
            stb.AppendLine("		 , " & Agro_SQL_SaveDate(AGRODATAFINE) & "  ")
            
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
    Public Function Aggiorna_Id_Mov_Det(ByVal piva As String,
                                        ByVal idAgenda As Integer,
                                        ByVal idMovDet As Integer,
                                        ByVal cauMov As String,
                                        ByVal stato As enum_WFlow_Export_XML_Universale,
                                        ByVal nomeFile As String,
                                        ByVal messaggio As String,
                                        ByVal flagUpdateDataModifica As Boolean,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreXMLUniversale.XML_Export_Mag_Attuali_W.Aggiorna_Id_Mov_Det()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            stb.AppendLine(" UPDATE XML_Export_Mag_Attuali ")
            stb.AppendLine(" SET ")
            stb.AppendLine("           Stato = " & Agro_SQL_SaveNum(stato) & " ")

            If nomeFile <> "" Then
                stb.AppendLine("         , NomeFile = '" & Agro_SQL_SaveText(nomeFile) & "' ")
            End If

            If messaggio <> "" Then
                stb.AppendLine("         , Messaggio = '" & Agro_SQL_SaveText(messaggio) & "' ")
            End If

            'se sto passando da 254 (export ok) a 255 (op. modificata) non devo aggiornare la Data_Modifica perché devo sapere il momento in cui ho esportato
            If flagUpdateDataModifica = True Then
                stb.AppendLine("         , Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                stb.AppendLine("         , Data_Modifica = " & Agro_SQL_SaveDateTime(Date.Now) & " ")
            End If

            stb.AppendLine(" WHERE   Id_Agenda = " & Agro_SQL_SaveNum(idAgenda) & " ")
            stb.AppendLine(" AND   Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            stb.AppendLine(" AND   Id_Mov_Det = " & Agro_SQL_SaveNum(idMovDet) & " ")
            stb.AppendLine(" AND   Cau_Mov = '" & Agro_SQL_SaveText(cauMov) & "' ")

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
