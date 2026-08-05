Imports System.Text.RegularExpressions
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider

Public Class SistemiEsterni_RicezioneNotifiche_R
    Inherits DataProvider
    Public Function Leggi(ByVal IDSistemaEsterno As Integer,
                          ByVal DataNotifica As Date,
                          ByVal Stato As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal XOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional ByVal TagName As String = "",
                          Optional ByVal Anagrafica As Integer = 0,
                          Optional ByVal Terreni As Integer = 0,
                          Optional ByVal PCG As Integer = 0,
                          Optional ByVal PCG_Terreni As Integer = 0,
                          Optional ByVal Equipaggiamenti As Integer = 0,
                          Optional ByVal Lavoratori As Integer = 0,
                          Optional ByVal TipoOperazione As Integer = 0,
                          Optional ByVal Gruppi_Appezzamenti As Integer = 0,
                          Optional ByVal CUAA As String = ""
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreNotifichePushDAL.SistemiEsterni_RicezioneNotifiche_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM SistemiEsterni_RicezioneNotifiche ")
            StrSQL.AppendLine(" WHERE 1 = 1")

            If IDSistemaEsterno <> 0 Then
                StrSQL.AppendLine(String.Format(" AND Id_SistemaEsterno = {0} ", Agro_SQL_SaveNum(IDSistemaEsterno)))
            End If

            If Stato <> 0 Then
                StrSQL.AppendLine(String.Format(" AND Stato = {0} ", Agro_SQL_SaveNum(Stato)))
            End If

            If TagName <> "" Then
                StrSQL.AppendLine(String.Format(" AND TagExecution = '{0}' ", Agro_SQL_SaveText(TagName)))
            End If

            Dim strAnagrafe As String = ""

            If Anagrafica = 1 OrElse Terreni = 1 OrElse PCG = 1 OrElse PCG_Terreni = 1 OrElse Equipaggiamenti = 1 OrElse Lavoratori = 1 Then
                If Anagrafica = 1 Then
                    strAnagrafe += " Anagrafica=1 or "
                End If
                If Terreni = 1 Then
                    strAnagrafe += " Terreni=1 or "
                End If
                If PCG = 1 Then
                    strAnagrafe += " PCG=1 or "
                End If
                If PCG_Terreni = 1 Then
                    strAnagrafe += " PCG_Terreni=1 or "
                End If
                If Equipaggiamenti = 1 Then
                    strAnagrafe += " Equipaggiamenti=1 or "
                End If
                If Lavoratori = 1 Then
                    strAnagrafe += " Lavoratori=1 or "
                End If
                If Gruppi_Appezzamenti = 1 Then
                    strAnagrafe += " Gruppi_Appezzamenti=1 or "
                End If
                If strAnagrafe.EndsWith(" or ") Then
                    strAnagrafe = strAnagrafe.Substring(1, Len(strAnagrafe) - 4)
                End If
            End If

            If strAnagrafe <> "" Then
                StrSQL.Append(" and ( " + strAnagrafe + " ) ")
            End If

            If TipoOperazione <> 0 Then
                Select Case TipoOperazione
                    Case 1
                        StrSQL.Append(" and TipoOperazione='C' ")
                    Case 2
                        StrSQL.Append(" and TipoOperazione='U' ")
                    Case Else
                End Select
            End If

            If CUAA <> "" Then
                StrSQL.Append(" AND CUAA = '" & Agro_SQL_SaveText(CUAA) & "' ")
            End If

            If Not String.IsNullOrWhiteSpace(xFiltroAggiuntivo) Then
                StrSQL.AppendLine(String.Format(" AND {0} ", Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri)))
            End If

            If Not String.IsNullOrWhiteSpace(XOrderBy) Then
                StrSQL.AppendLine(String.Format(" ORDER BY {0} ", Agro_SQL_Save_xOrderBy(XOrderBy, objParametri)))
            Else
                StrSQL.AppendLine(" ORDER BY Priorita Desc,ID ASC ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT
    End Function
End Class

Public Class SistemiEsterni_RicezioneNotifiche_W
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function Scrivi(ByVal IDSistemaEsterno As Integer,
                           ByVal Payload As String,
                           ByVal CUAA As String,
                           ByVal TipoOperazione As String,
                           ByVal Anagrafica As Integer,
                           ByVal Terreni As Integer,
                           ByVal PCG As Integer,
                           ByVal PCG_Terreni As Integer,
                           ByVal Equipaggiamenti As Integer,
                           ByVal Lavoratori As Integer,
                           ByVal GruppiAppezzamenti As Integer,
                           ByVal Priorita As Integer,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreNotifichePushDAL.SistemiEsterni_RicezioneNotifiche_W.Scrivi()"

        Dim MessaggioErrore As String
        Dim StrSQL As New System.Text.StringBuilder
        Dim isResp As Boolean

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" INSERT INTO SistemiEsterni_RicezioneNotifiche ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine("     Id_SistemaEsterno, ")
            StrSQL.AppendLine("     Payload, ")
            StrSQL.AppendLine("     Stato, ")
            StrSQL.AppendLine(" 	N_Retry, ")
            StrSQL.AppendLine(" 	CUAA, ")
            StrSQL.AppendLine(" 	TipoOperazione, ")
            StrSQL.AppendLine(" 	Anagrafica, ")
            StrSQL.AppendLine(" 	Terreni, ")
            StrSQL.AppendLine(" 	PCG, ")
            StrSQL.AppendLine(" 	PCG_Terreni, ")
            StrSQL.AppendLine(" 	Equipaggiamenti, ")
            StrSQL.AppendLine(" 	Lavoratori, ")
            StrSQL.AppendLine(" 	Gruppi_Appezzamenti, ")
            StrSQL.AppendLine(" 	Priorita, ")
            StrSQL.AppendLine(" 	Inviato, ")
            StrSQL.AppendLine(" 	Data_Creazione, ")
            StrSQL.AppendLine(" 	Data_Modifica, ")
            StrSQL.AppendLine(" 	Username_Creazione, ")
            StrSQL.AppendLine(" 	Username_Modifica, ")
            StrSQL.AppendLine(" 	Validita_Inizio, ")
            StrSQL.AppendLine(" 	Validita_Fine ")
            StrSQL.AppendLine(" ) VALUES ( ")
            StrSQL.AppendLine(String.Format(" {0}, ", Agro_SQL_SaveNum(IDSistemaEsterno)))
            StrSQL.AppendLine(String.Format(" '{0}', ", Agro_SQL_SaveText(Payload)))
            StrSQL.AppendLine(" 0, ")
            StrSQL.AppendLine(" 0, ")
            StrSQL.AppendLine(String.Format(" '{0}', ", Agro_SQL_SaveText(CUAA)))
            StrSQL.AppendLine(String.Format(" '{0}', ", Agro_SQL_SaveText(TipoOperazione)))
            StrSQL.AppendLine(String.Format(" {0}, ", Agro_SQL_SaveNum(Anagrafica)))
            StrSQL.AppendLine(String.Format(" {0}, ", Agro_SQL_SaveNum(Terreni)))
            StrSQL.AppendLine(String.Format(" {0}, ", Agro_SQL_SaveNum(PCG)))
            StrSQL.AppendLine(String.Format(" {0}, ", Agro_SQL_SaveNum(PCG_Terreni)))
            StrSQL.AppendLine(String.Format(" {0}, ", Agro_SQL_SaveNum(Equipaggiamenti)))
            StrSQL.AppendLine(String.Format(" {0}, ", Agro_SQL_SaveNum(Lavoratori)))
            StrSQL.AppendLine(String.Format(" {0}, ", Agro_SQL_SaveNum(GruppiAppezzamenti)))
            StrSQL.AppendLine(String.Format(" {0}, ", Agro_SQL_SaveNum(Priorita)))
            StrSQL.AppendLine(" 0, ")
            StrSQL.AppendLine(String.Format(" {0}, ", Agro_SQL_SaveDateTime(Date.Now)))
            StrSQL.AppendLine(String.Format(" {0}, ", Agro_SQL_SaveDateTime(Date.Now)))
            StrSQL.AppendLine(String.Format(" '{0}', ", Agro_SQL_SaveText(objParametri.UtenteUsername)))
            StrSQL.AppendLine(String.Format(" '{0}', ", Agro_SQL_SaveText(objParametri.UtenteUsername)))
            StrSQL.AppendLine(String.Format(" {0}, ", Agro_SQL_SaveDate(AGRODATAINIZIO)))
            StrSQL.AppendLine(String.Format(" {0} ", Agro_SQL_SaveDate(AGRODATAFINE)))
            StrSQL.AppendLine(" ) ")

            '--------------------------------------------------------------------------
            isResp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            isResp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return isResp
    End Function

    Public Function Modifica(ByVal ID As Integer,
                             ByVal Stato As Integer,
                             ByVal NRetry As Integer,
                             ByVal EsitoText As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                             Optional ByVal LogError As Boolean = True
                             ) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreNotifichePushDAL.SistemiEsterni_RicezioneNotifiche_W.Modifica()"

        Dim MessaggioErrore As String
        Dim StrSQL As New System.Text.StringBuilder
        Dim isResp As Boolean

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" update SistemiEsterni_RicezioneNotifiche with (ROWLOCK) set ")
            StrSQL.AppendLine("   Inviato           =  0 ")
            StrSQL.AppendLine("   ,DataInvio         =  Null ")
            StrSQL.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("   ,EsitoText = '" & Agro_SQL_SaveText(EsitoText) & "' ")
            If NRetry <> 0 Then
                StrSQL.AppendLine("   ,N_Retry = " & Agro_SQL_SaveNum(NRetry) & " ")
            End If
            If Stato <> 0 Then
                StrSQL.AppendLine("   ,Stato = " & Agro_SQL_SaveNum(Stato) & " ")
            End If

            StrSQL.AppendLine(" WHERE 1=1 ")
            StrSQL.AppendLine(" AND ID = " & Agro_SQL_SaveNum(ID) & " ")

            '--------------------------------------------------------------------------
            isResp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore, LogError)
            isResp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return isResp
    End Function

    Public Function AggiornaStatisticheNotifica(ByVal ID As Integer,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                Optional ByVal Inizio_Anagrafica As DateTime = AGRODATAINIZIO,
                                                Optional ByVal Fine_Anagrafica As DateTime = AGRODATAFINE,
                                                Optional ByVal Inizio_Catasto As DateTime = AGRODATAINIZIO,
                                                Optional ByVal Fine_Catasto As DateTime = AGRODATAFINE,
                                                Optional ByVal Inizio_PCG As DateTime = AGRODATAINIZIO,
                                                Optional ByVal Fine_PCG As DateTime = AGRODATAFINE,
                                                Optional ByVal N_App As Integer = -1,
                                                Optional ByVal Inizio_Lavoratori As DateTime = AGRODATAINIZIO,
                                                Optional ByVal Fine_Lavoratori As DateTime = AGRODATAFINE,
                                                Optional ByVal Inizio_Equipaggiamenti As DateTime = AGRODATAINIZIO,
                                                Optional ByVal Fine_Equipaggiamenti As DateTime = AGRODATAFINE,
                                                Optional ByVal Inizio_Gruppi_Appezzamenti As DateTime = AGRODATAINIZIO,
                                                Optional ByVal Fine_Gruppi_Appezzamenti As DateTime = AGRODATAFINE,
                                                Optional ByVal LogError As Boolean = True
                                            ) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreNotifichePushDAL.SistemiEsterni_RicezioneNotifiche_W.AggiornaStatisticheNotifica()"

        Dim MessaggioErrore As String
        Dim StrSQL As New System.Text.StringBuilder
        Dim isResp As Boolean

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" update SistemiEsterni_RicezioneNotifiche with (ROWLOCK) set ")
            StrSQL.AppendLine("   Inviato           =  0 ")
            StrSQL.AppendLine("   ,DataInvio         =  Null ")
            StrSQL.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            If Inizio_Anagrafica <> AGRODATAINIZIO Then
                StrSQL.AppendLine("   ,Inizio_Anagrafica = " & Agro_SQL_SaveDateTime(Inizio_Anagrafica) & " ")
            End If
            If Fine_Anagrafica <> AGRODATAFINE Then
                StrSQL.AppendLine("   ,Fine_Anagrafica = " & Agro_SQL_SaveDateTime(Fine_Anagrafica) & " ")
            End If
            If Inizio_Catasto <> AGRODATAINIZIO Then
                StrSQL.AppendLine("   ,Inizio_Catasto = " & Agro_SQL_SaveDateTime(Inizio_Catasto) & " ")
            End If
            If Fine_Catasto <> AGRODATAFINE Then
                StrSQL.AppendLine("   ,Fine_Catasto = " & Agro_SQL_SaveDateTime(Fine_Catasto) & " ")
            End If
            If Inizio_PCG <> AGRODATAINIZIO Then
                StrSQL.AppendLine("   ,Inizio_PCG = " & Agro_SQL_SaveDateTime(Inizio_PCG) & " ")
            End If
            If Fine_PCG <> AGRODATAFINE Then
                StrSQL.AppendLine("   ,Fine_PCG = " & Agro_SQL_SaveDateTime(Fine_PCG) & " ")
            End If
            If N_App <> -1 Then
                StrSQL.AppendLine("   ,N_App = " & Agro_SQL_SaveNum(N_App) & " ")
            End If
            If Inizio_Lavoratori <> AGRODATAINIZIO Then
                StrSQL.AppendLine("   ,Inizio_Lavoratori = " & Agro_SQL_SaveDateTime(Inizio_Lavoratori) & " ")
            End If
            If Fine_Lavoratori <> AGRODATAFINE Then
                StrSQL.AppendLine("   ,Fine_Lavoratori = " & Agro_SQL_SaveDateTime(Fine_Lavoratori) & " ")
            End If
            If Inizio_Equipaggiamenti <> AGRODATAINIZIO Then
                StrSQL.AppendLine("   ,Inizio_Equipaggiamenti = " & Agro_SQL_SaveDateTime(Inizio_Equipaggiamenti) & " ")
            End If
            If Fine_Equipaggiamenti <> AGRODATAFINE Then
                StrSQL.AppendLine("   ,Fine_Equipaggiamenti = " & Agro_SQL_SaveDateTime(Fine_Equipaggiamenti) & " ")
            End If
            If Inizio_Gruppi_Appezzamenti <> AGRODATAINIZIO Then
                StrSQL.AppendLine("   ,Inizio_Gruppi_Appezzamenti = " & Agro_SQL_SaveDateTime(Inizio_Gruppi_Appezzamenti) & " ")
            End If
            If Fine_Gruppi_Appezzamenti <> AGRODATAFINE Then
                StrSQL.AppendLine("   ,Fine_Gruppi_Appezzamenti = " & Agro_SQL_SaveDateTime(Fine_Gruppi_Appezzamenti) & " ")
            End If

            StrSQL.AppendLine(" WHERE 1=1 ")
            StrSQL.AppendLine(" AND ID = " & Agro_SQL_SaveNum(ID) & " ")

            '--------------------------------------------------------------------------
            isResp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore, LogError)
            isResp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return isResp
    End Function

    Public Function AssociaNotificheATagnameCreazione(ByVal TipoOperazione As Integer,
                                             ByVal BatchSize As Integer,
                                             ByVal TagName As String,
                                             ByVal Anagrafica As Integer,
                                             ByVal Terreni As Integer,
                                             ByVal PCG As Integer,
                                             ByVal PCG_Terreni As Integer,
                                             ByVal Equipaggiamenti As Integer,
                                             ByVal Lavoratori As Integer,
                                             ByVal Gruppi_Appezzamenti As Integer,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                             Optional ByVal LogError As Boolean = True
                                             ) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreNotifichePushDAL.SistemiEsterni_RicezioneNotifiche_W.AssociaNotificheATagnameCreazione()"

        Dim MessaggioErrore As String
        Dim StrSQL As New System.Text.StringBuilder
        Dim isResp As Boolean

        Try
            Dim strAnagrafe As String = ""

            If Anagrafica = 1 OrElse Terreni = 1 OrElse PCG = 1 OrElse PCG_Terreni = 1 OrElse Equipaggiamenti = 1 OrElse Lavoratori = 1 Then
                If Anagrafica = 1 Then
                    strAnagrafe += " Anagrafica=1 or "
                End If
                If Terreni = 1 Then
                    strAnagrafe += " Terreni=1 or "
                End If
                If PCG = 1 Then
                    strAnagrafe += " PCG=1 or "
                End If
                If PCG_Terreni = 1 Then
                    strAnagrafe += " PCG_Terreni=1 or "
                End If
                If Equipaggiamenti = 1 Then
                    strAnagrafe += " Equipaggiamenti=1 or "
                End If
                If Lavoratori = 1 Then
                    strAnagrafe += " Lavoratori=1 or "
                End If
                If Gruppi_Appezzamenti = 1 Then
                    strAnagrafe += " Gruppi_Appezzamenti=1 or "
                End If
                If strAnagrafe.EndsWith(" or ") Then
                    strAnagrafe = strAnagrafe.Substring(1, Len(strAnagrafe) - 4)
                End If
            End If

            StrSQL.Length = 0

            StrSQL.AppendLine(String.Format(" update dest with (ROWLOCK) set dest.tagexecution='{0}' ", Agro_SQL_SaveText(TagName)))
            StrSQL.AppendLine(" from ")
            StrSQL.AppendLine("     SistemiEsterni_RicezioneNotifiche dest inner join")
            StrSQL.AppendLine("  ")
            StrSQL.AppendLine(" ( select top (" + BatchSize.ToString() + ") ID ")
            StrSQL.AppendLine("     from SistemiEsterni_RicezioneNotifiche WITH (UPDLOCK,ROWLOCK,READPAST) ")
            StrSQL.AppendLine(String.Format(" where TipoOperazione='C' "))
            StrSQL.AppendLine(String.Format(" and (Stato = 0 And (TagExecution ='' or TagExecution is null or TagExecution='{0}' )) ", Agro_SQL_SaveText(TagName)))

            If strAnagrafe <> "" Then
                StrSQL.Append(" and ( " + strAnagrafe + " ) ")
            End If

            StrSQL.AppendLine(" order by Priorita Desc,ID ) source ")
            StrSQL.AppendLine(" on dest.id=source.id and dest.TagExecution='' ")


            '--------------------------------------------------------------------------
            isResp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore, LogError)
            isResp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return isResp
    End Function

    Public Function AssociaNotificheATagnameAggiornamenti(ByVal TipoOperazione As Integer,
                                             ByVal BatchSize As Integer,
                                             ByVal TagName As String,
                                             ByVal Anagrafica As Integer,
                                             ByVal Terreni As Integer,
                                             ByVal PCG As Integer,
                                             ByVal PCG_Terreni As Integer,
                                             ByVal Equipaggiamenti As Integer,
                                             ByVal Lavoratori As Integer,
                                             ByVal Gruppi_Appezzamenti As Integer,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                             Optional ByVal LogError As Boolean = True
                                             ) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreNotifichePushDAL.SistemiEsterni_RicezioneNotifiche_W.AssociaNotificheATagnameAggiornamenti()"

        Dim MessaggioErrore As String
        Dim StrSQL As New System.Text.StringBuilder
        Dim isResp As Boolean

        Try
            Dim strAnagrafe As String = ""
            Dim strAnagrafe2 As String = ""

            If Anagrafica = 1 OrElse Terreni = 1 OrElse PCG = 1 OrElse PCG_Terreni = 1 OrElse Equipaggiamenti = 1 OrElse Lavoratori = 1 Then
                If Anagrafica = 1 Then
                    strAnagrafe += " a.Anagrafica=1 or "
                    strAnagrafe2 += " Anagrafica=1 or "
                End If
                If Terreni = 1 Then
                    strAnagrafe += " a.Terreni=1 or "
                    strAnagrafe2 += " Terreni=1 or "
                End If
                If PCG = 1 Then
                    strAnagrafe += " a.PCG=1 or "
                    strAnagrafe2 += " PCG=1 or "
                End If
                If PCG_Terreni = 1 Then
                    strAnagrafe += " a.PCG_Terreni=1 or "
                    strAnagrafe2 += " PCG_Terreni=1 or "
                End If
                If Equipaggiamenti = 1 Then
                    strAnagrafe += " a.Equipaggiamenti=1 or "
                    strAnagrafe2 += " Equipaggiamenti=1 or "
                End If
                If Lavoratori = 1 Then
                    strAnagrafe += " a.Lavoratori=1 or "
                    strAnagrafe2 += " Lavoratori=1 or "
                End If
                If Gruppi_Appezzamenti = 1 Then
                    strAnagrafe += " a.Gruppi_Appezzamenti=1 or "
                    strAnagrafe2 += " Gruppi_Appezzamenti=1 or "
                End If
                If strAnagrafe.EndsWith(" or ") Then
                    strAnagrafe = strAnagrafe.Substring(1, Len(strAnagrafe) - 4)
                End If
                If strAnagrafe2.EndsWith(" or ") Then
                    strAnagrafe2 = strAnagrafe2.Substring(1, Len(strAnagrafe2) - 4)
                End If
            End If

            StrSQL.Length = 0

            StrSQL.AppendLine(String.Format(" update dest With (ROWLOCK) Set dest.tagexecution=Case When source.TagExecution Is null Or source.TagExecution='' then '{0}' else source.TagExecution end ", Agro_SQL_SaveText(TagName)))
            StrSQL.AppendLine("     from ")
            StrSQL.AppendLine("     SistemiEsterni_RicezioneNotifiche dest inner join ")
            StrSQL.AppendLine(" ( select top (" + BatchSize.ToString() + ") a.ID, ")
            StrSQL.AppendLine("     a.CUAA, ")
            StrSQL.AppendLine("     a.Priorita, ")
            StrSQL.AppendLine("     C.TagExecution ")
            StrSQL.AppendLine("   From SistemiEsterni_RicezioneNotifiche a WITH (UPDLOCK,ROWLOCK,READPAST) left Join ")
            StrSQL.AppendLine(" (Select CUAA, TipoOperazione, MAX(ID) As ID from SistemiEsterni_RicezioneNotifiche  WITH (UPDLOCK,ROWLOCK,READPAST) ")
            StrSQL.AppendLine("     where ")
            StrSQL.AppendLine("     TipoOperazione ='U' ")
            If strAnagrafe2 <> "" Then
                StrSQL.Append(" and ( " + strAnagrafe2 + " ) ")
            End If
            StrSQL.AppendLine(String.Format(" And TagExecution = '{0}' ", Agro_SQL_SaveText(TagName)))
            StrSQL.AppendLine(" Group by CUAA,TipoOperazione) b ")
            StrSQL.AppendLine(" inner Join SistemiEsterni_RicezioneNotifiche c WITH (UPDLOCK,ROWLOCK,READPAST) ")
            StrSQL.AppendLine(" On b.ID = c.Id ")
            StrSQL.AppendLine(" On a.CUAA=b.CUAA And a.TipoOperazione=b.TipoOperazione ")
            StrSQL.AppendLine(String.Format(" where a.tipooperazione ='U' and (a.Stato = 0 And (a.TagExecution ='' or a.TagExecution='{0}') ", Agro_SQL_SaveText(TagName)))
            StrSQL.AppendLine(String.Format(" And (c.TagExecution ='' or c.TagExecution is null or c.TagExecution='{0}') ", Agro_SQL_SaveText(TagName)))
            If strAnagrafe <> "" Then
                StrSQL.Append(" and ( " + strAnagrafe + " ) ")
            End If
            StrSQL.AppendLine(" ) ")
            StrSQL.AppendLine(" order by A.Priorita Desc,a.ID asc ) source ")
            StrSQL.AppendLine(" On dest.id=source.id And dest.TagExecution='' ")

            '--------------------------------------------------------------------------
            isResp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore, LogError)
            isResp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return isResp
    End Function




End Class
