Imports System.Data.Entity
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework

Public Class Configurazione_Servizi_Guida
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal pivaSuperuser As String,
                          ByVal idServizio As enum_Id_Servizio,
                          ByVal tipoSincro As enum_Tipi_Servizi_Background,
                          ByVal idRiga As Integer,
                          ByVal piva As String,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal stato As Integer = -99
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreVarieDAL.Configurazione_Servizi_Guida.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT a.*, b.Rag_Soc ")
            strSql.AppendLine(" , ISNULL(Configurazioni_Interscambio.Parametri_Standard, '') AS Parametri_Standard ")
            strSql.AppendLine(" , ISNULL(Configurazioni_Interscambio.Parametri_Runtime, '') AS Parametri_Runtime ")
            strSql.AppendLine(" , ISNULL(Configurazioni_Interscambio.Tipizzazione_Parametri_Runtime, '') AS Tipizzazione_Parametri_Runtime ")
            strSql.AppendLine(" FROM  Configurazione_Servizi_Guida a ")
            strSql.AppendLine(" LEFT  JOIN Imprese b ON a.piva = b.piva ")
            strSql.AppendLine(" LEFT  JOIN Configurazioni_Interscambio ON a.Id_Config = Configurazioni_Interscambio.Id_Config ")
            strSql.AppendLine(" WHERE 1=1  ")

            If pivaSuperuser <> "" Then
                strSql.AppendLine(" AND a.PivaSuperuser = '" & Agro_SQL_SaveText(pivaSuperuser) & "' ")
            End If

            If idServizio <> 0 Then
                strSql.AppendLine(" AND a.Id_Servizio = " & Agro_SQL_SaveNum(idServizio) & " ")
            End If

            If tipoSincro <> 0 Then
                strSql.AppendLine(" AND a.Tipo_Sincro = " & Agro_SQL_SaveNum(tipoSincro) & " ")
            End If

            If idRiga <> 0 Then
                strSql.AppendLine(" AND a.Id_Riga = " & Agro_SQL_SaveNum(idRiga) & " ")
            End If

            If piva <> "" Then
                strSql.AppendLine(" AND a.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If stato <> -99 Then
                strSql.AppendLine(" AND a.Stato = " & Agro_SQL_SaveNum(stato) & " ")
            End If

            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY a.PivaSuperUser, a.Id_Servizio, a.Tipo_Sincro, a.Id_Riga  ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function GetGuida(ByRef objParametriServer As AgronicaCoreParametri,
                             ByVal objConfigServizi As Configurazione_Servizio
                             ) As AgronicaCoreEntityFramework_POCO.Configurazione_Servizi_Guida

        Const nomeRoutine = "GetGuida"
        Dim messaggioErrore As String = ""
        Dim objGuida As AgronicaCoreEntityFramework_POCO.Configurazione_Servizi_Guida = Nothing

        Try

            objGuida = GetGuida(objParametriServer,
                                objConfigServizi.PivaSuperuser,
                                objConfigServizi.Id_Servizio,
                                objConfigServizi.Tipo_Sincro,
                                objConfigServizi.Id_Riga)

        Catch ex As Exception
            messaggioErrore = ex.Message & " [" & If(ex.InnerException Is Nothing, "", ex.InnerException.ToString) & "]"
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            objGuida = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return objGuida

    End Function

    Public Function GetGuida(ByRef objParametriServer As AgronicaCoreParametri,
                             ByVal pivaSuperUser As String,
                             ByVal idServizio As Integer,
                             ByVal tipoSincro As Integer,
                             ByVal idRiga As Integer,
                             Optional ByVal id As Integer = 0
                             ) As AgronicaCoreEntityFramework_POCO.Configurazione_Servizi_Guida

        Const nomeRoutine = "GetGuida"
        Dim messaggioErrore As String = ""
        Dim objGuida As AgronicaCoreEntityFramework_POCO.Configurazione_Servizi_Guida = Nothing

        Try

            Dim gEfUtils As New Gias_EF_Utility
            Dim efConnString As String = gEfUtils.GetEntityConnectionString(objParametriServer.StringaConnessione)

            Dim listGuide As List(Of AgronicaCoreEntityFramework_POCO.Configurazione_Servizi_Guida)

            Using dal As New Gias_DeveloperServer_Entities(efConnString)

                listGuide = (From g In dal.Configurazione_Servizi_Guida
                             Where g.ID = id OrElse
                                  (g.PivaSuperuser = pivaSuperUser AndAlso
                                   g.Id_Servizio = idServizio AndAlso
                                   g.Tipo_Sincro = tipoSincro AndAlso
                                   g.Id_Riga = idRiga)
                             Select g).ToList()

                If listGuide Is Nothing OrElse listGuide.Count <> 1 Then
                    'TODO: Se è stato impostato l'uso della guida almeno dovrebbe esserci solo una riga?!?
                    Throw New Exception(String.Format("Non è presente alcuna riga guida per [{0}-{1}-{2}-{3}]",
                                                      pivaSuperUser, idServizio, tipoSincro, idRiga))
                Else
                    objGuida = listGuide(0)
                End If

            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message & " [" & If(ex.InnerException Is Nothing, "", ex.InnerException.ToString) & "]"
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            objGuida = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return objGuida

    End Function

    Public Sub AggiornaGuida(ByRef objParametriServer As AgronicaCoreParametri,
                             ByRef objGuida As AgronicaCoreEntityFramework_POCO.Configurazione_Servizi_Guida)

        Const nomeRoutine = "AggiornaGuida"
        Dim messaggioErrore As String = ""

        Try

            Dim gEfUtils As New Gias_EF_Utility
            Dim efConnString As String = gEfUtils.GetEntityConnectionString(objParametriServer.StringaConnessione)

            Using dal As New Gias_DeveloperServer_Entities(efConnString)

                objGuida.Data_Modifica = DateTime.Now
                objGuida.Username_Modifica = objParametriServer.UsernameOperazione

                If objGuida.Validita_Inizio Is Nothing Then
                    objGuida.Validita_Inizio = AGRODATAINIZIO
                End If

                If objGuida.Validita_Fine Is Nothing Then
                    objGuida.Validita_Fine = AGRODATAFINE
                End If

                dal.Entry(objGuida).State = EntityState.Modified
                dal.SaveChanges()

            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    Public Function AggiornaStatoGuida(ByRef objParametriServer As AgronicaCoreParametri,
                             ByVal id As Integer,
                             ByVal stato As enum_Stato_Guida_Servizi_Background,
                             Optional ByVal dataUltimaEsecuzione As DateTime? = Nothing,
                             Optional ByVal msg As String = Nothing) As enum_Stato_Guida_Servizi_Background

        Dim aggiorna_stato As Boolean = False
        Dim objGuida = GetGuida(objParametriServer, "", 0, 0, 0, id)
        Dim statoServizio = objGuida.Stato

        ' aggiorno lo stato solo se si verificano le seguenti condizioni
        If stato = enum_Stato_Guida_Servizi_Background.Spento OrElse stato = enum_Stato_Guida_Servizi_Background.Disabilitato Then
            'If objGuida.Stato <> enum_Stato_Guida_Servizi_Background.InEsecuzione Then
            aggiorna_stato = True
            'End If
        ElseIf stato = enum_Stato_Guida_Servizi_Background.Attivo OrElse stato = enum_Stato_Guida_Servizi_Background.SempreAttivo Then
            If objGuida.Stato = enum_Stato_Guida_Servizi_Background.Spento OrElse objGuida.Stato = enum_Stato_Guida_Servizi_Background.Errore Then
                aggiorna_stato = True
            End If
        ElseIf stato = enum_Stato_Guida_Servizi_Background.InEsecuzione Then
            If objGuida.Stato = enum_Stato_Guida_Servizi_Background.Attivo OrElse objGuida.Stato = enum_Stato_Guida_Servizi_Background.SempreAttivo Then
                aggiorna_stato = True
            End If
        ElseIf stato = enum_Stato_Guida_Servizi_Background.Errore Then
            If objGuida.Stato = enum_Stato_Guida_Servizi_Background.InEsecuzione Then
                aggiorna_stato = True
            End If
        End If

        If aggiorna_stato Then
            SetStatoGuida(objGuida, stato, dataUltimaEsecuzione, msg)
            AggiornaGuida(objParametriServer, objGuida)
        End If

        Return statoServizio

    End Function

    Public Sub SetStatoGuida(ByRef objGuida As AgronicaCoreEntityFramework_POCO.Configurazione_Servizi_Guida,
                             ByVal stato As enum_Stato_Guida_Servizi_Background,
                             Optional ByVal dataUltimaEsecuzione As DateTime? = Nothing,
                             Optional ByVal msg As String = Nothing,
                             Optional ByVal listIdLog As List(Of Integer) = Nothing)

        Const nomeRoutine = "SetStatoGuida"

        Try

            objGuida.Stato = CInt(stato)

            If dataUltimaEsecuzione IsNot Nothing Then
                objGuida.Data_UltimaEsecuzione = dataUltimaEsecuzione
            End If

            If msg IsNot Nothing Then
                objGuida.Note = msg
            End If

            If listIdLog IsNot Nothing Then
                Dim logInizio As Integer? = Nothing
                Dim logFine As Integer? = Nothing

                If listIdLog.Count > 0 Then
                    logInizio = listIdLog.FirstOrDefault()
                    logFine = listIdLog.LastOrDefault()
                End If
                
                objGuida.Ultimo_Invio_Inizio = logInizio
                objGuida.Ultimo_Invio_Fine = logFine
            End If

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

    End Sub

    Public Function ModificaPuntuale(ByVal id As Integer,
                                     ByVal pivaSuperUser As String,
                                     ByVal idServizio As enum_Id_Servizio,
                                     ByVal tipoSincro As enum_Tipi_Servizi_Background,
                                     ByVal idRiga As Integer,
                                     ByVal objParametri As AgronicaCoreParametri,
                                     Optional ByVal piva As String = Nothing,
                                     Optional ByVal descrizione As String = Nothing,
                                     Optional ByVal stato As Integer? = Nothing,
                                     Optional ByVal dataUltimaEsecuzione As DateTime? = Nothing,
                                     Optional ByVal note As String = Nothing,
                                     Optional ByVal validitaInizio As Date? = Nothing,
                                     Optional ByVal validitaFine As Date? = Nothing,
                                     Optional ByVal dataModifica As DateTime = #2/1/1900#,
                                     Optional ByVal usernameModifica As String = ""
                                     ) As Boolean

        Const nomeRoutine = "Configurazione_Servizi_Guida.ModificaPuntuale()"

        '====================================================================================
        'Parametri opzionali :
        '   Tutti i valori non chiave (se impostati a nothing o non passati 
        '   non ne verrà fatto l'aggiornamento e rimarranno i valori precedenti)
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If pivaSuperUser = "" Then
                Throw New Exception("PivaSuperuser non valorizzata")
            End If

            If idServizio = 0 Then
                Throw New Exception("Id_Servizio non valorizzato")
            End If

            If tipoSincro = 0 Then
                Throw New Exception("Tipo_Sincro non valorizzato")
            End If

            If idRiga = 0 Then
                Throw New Exception("Id_Riga non valorizzata")
            End If

            If dataModifica = #2/1/1900# Then
                dataModifica = DateTime.Now
            End If

            If usernameModifica = "" Then
                usernameModifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine(" UPDATE Configurazione_Servizi_Guida ")
            strSql.AppendLine(" SET Data_Modifica = " & Agro_SQL_SaveDateTime(dataModifica) & " ")
            strSql.AppendLine("   , Username_Modifica = '" & Agro_SQL_SaveText(usernameModifica) & "' ")

            If Not IsNothing(piva) Then
                strSql.AppendLine("   , Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If Not IsNothing(descrizione) Then
                strSql.AppendLine("   , Descrizione = '" & Agro_SQL_SaveText(descrizione) & "' ")
            End If

            If Not IsNothing(stato) Then
                strSql.AppendLine("   , Stato = " & Agro_SQL_SaveNum(stato) & " ")
            End If

            If Not IsNothing(dataUltimaEsecuzione) Then
                strSql.AppendLine("   , Data_UltimaEsecuzione = " & Agro_SQL_SaveDateTime(dataUltimaEsecuzione) & " ")
            End If

            If Not IsNothing(note) Then
                strSql.AppendLine("   , Note = '" & Agro_SQL_SaveText(note) & "' ")
            End If

            If Not IsNothing(validitaInizio) Then
                strSql.AppendLine("   , Validita_Inizio = " & Agro_SQL_SaveDate(validitaInizio) & " ")
            End If

            If Not IsNothing(validitaFine) Then
                strSql.AppendLine("   , Validita_Fine = " & Agro_SQL_SaveDate(validitaFine) & " ")
            End If

            '---------------------------------------------            

            strSql.AppendLine(" WHERE ID = " & Agro_SQL_SaveNum(id) & " ")
            strSql.AppendLine(" AND PivaSuperuser = '" & Agro_SQL_SaveText(pivaSuperUser) & "' ")
            strSql.AppendLine(" AND Id_Servizio =" & Agro_SQL_SaveNum(idServizio) & " ")
            strSql.AppendLine(" AND Tipo_Sincro =" & Agro_SQL_SaveNum(tipoSincro) & " ")
            strSql.AppendLine(" AND Id_Riga =" & Agro_SQL_SaveNum(idRiga) & " ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
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
