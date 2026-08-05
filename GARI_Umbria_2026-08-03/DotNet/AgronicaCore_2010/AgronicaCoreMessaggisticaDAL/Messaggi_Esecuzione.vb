Imports AgronicaCoreDataProvider

Public Class Messaggi_Esecuzione_R
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function LeggiMessaggiEsecuzioneNonLetti(ByRef objParametri As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Messaggi_Esecuzione_R.LeggiMessaggiEsecuzioneNonLetti()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine(" ID_Messaggio ")
            StrSQL.AppendLine(" , Destinatario_UserName ")
            StrSQL.AppendLine(" , Testo_Messaggio ")
            StrSQL.AppendLine(" FROM Messaggistica_Programmazione ")

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" PivaSuperUser = '{0}' ", Agro_SQL_SaveText(objParametri.PivaSuperUser)))
            StrSQL.AppendLine(" AND Letto = 0 ")
            StrSQL.AppendLine(" AND Annullata_Lettura = 0 ")

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

    Public Function LeggiMessaggioByID(ByVal ID_Messaggio As Int32,
                                       ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Messaggi_Esecuzione_R.LeggiMessaggioByID()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine(" ID_Messaggio ")
            StrSQL.AppendLine(" , Destinatario_UserName ")
            StrSQL.AppendLine(" , Testo_Messaggio ")
            StrSQL.AppendLine(" , Letto ")
            StrSQL.AppendLine(" , Annullata_Lettura ")
            StrSQL.AppendLine(" FROM Messaggistica_Programmazione ")

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" PivaSuperUser = '{0}' ", Agro_SQL_SaveText(objParametri.PivaSuperUser)))
            StrSQL.AppendLine(String.Format(" AND ID_Messaggio = {0} ", Agro_SQL_SaveNum(ID_Messaggio)))

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
Public Class Messaggi_Esecuzione_W
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function AccodaMessaggioEsecuzione(ByVal destinatario_UserName As String,
                                              ByVal testo_Messaggio As String,
                                              ByVal newIdMessaggio As Int32,
                                              ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.Messaggi_Esecuzione_W.AccodaMessaggioEsecuzione()"

        Dim MessaggioErrore As String = ""
        Dim StrSql As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try

            '---------------------------------------------
            StrSql.Length = 0

            StrSql.AppendLine(" INSERT INTO Messaggistica_Programmazione ( ")
            StrSql.AppendLine("     ID_Messaggio ")
            StrSql.AppendLine("     , PivaSuperUser ")
            StrSql.AppendLine("     , Destinatario_UserName ")
            StrSql.AppendLine("     , Testo_Messaggio ")
            StrSql.AppendLine("     , Username_Creazione ")
            StrSql.AppendLine("     , Username_Modifica ")

            StrSql.AppendLine(" ) VALUES ( ")

            StrSql.AppendLine(String.Format(" {0} ", Agro_SQL_SaveNum(newIdMessaggio)))
            StrSql.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri.PivaSuperUser)))
            StrSql.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(destinatario_UserName)))
            StrSql.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(testo_Messaggio)))
            StrSql.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSql.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))

            StrSql.AppendLine(" ) ")

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(objParametri, StrSql.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return resp

    End Function

    Public Function ModificaMessaggioEsecuzione(ByVal ID_Messaggio As Int32,
                                                ByVal letto As Boolean,
                                                ByVal annullato As Boolean,
                                                ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.Messaggi_Esecuzione_W.ModificaMessaggioEsecuzione()"

        Dim MessaggioErrore As String = ""
        Dim StrSql As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try

            '---------------------------------------------
            StrSql.Length = 0

            StrSql.AppendLine(" UPDATE Messaggistica_Programmazione ")
            StrSql.AppendLine(" SET ")
            StrSql.AppendLine(" Data_modifica = GETDATE() ")
            StrSql.AppendLine(String.Format(" , UserName_Modifica = '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))

            If letto Then
                StrSql.AppendLine(" , Letto = 1 ")
                StrSql.AppendLine(" , Data_Lettura = GETDATE() ")
            End If

            If annullato Then
                StrSql.AppendLine(" , Annullata_Lettura = 1 ")
                'TODO: gestire la data annullamento quando necessario
            End If

            StrSql.AppendLine(" WHERE ")
            StrSql.AppendLine(String.Format("     ID_Messaggio = {0} ", Agro_SQL_SaveNum(ID_Messaggio)))
            StrSql.AppendLine(String.Format("     AND PivaSuperUser = '{0}' ", Agro_SQL_SaveText(objParametri.PivaSuperUser)))

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(objParametri, StrSql.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return resp

    End Function
End Class
