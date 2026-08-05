Imports AgronicaCoreDataProvider

Public Class Utenti_Retail_R
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function LeggiDaRiportare(ByVal codiceAttivazione As String,
                                     ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiRetailDAL.Utenti_Retail_R.LeggiDaRiportare()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT JsonData FROM APP_UtentiRetail ")
            StrSQL.AppendLine(" WHERE ")
            StrSQL.AppendLine(String.Format(" ID = '{0}' ", Agro_SQL_SaveText(codiceAttivazione)))
            StrSQL.AppendLine(" AND Cancellato = 0 ")

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
Public Class Utenti_Retail_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(codiceAttivazione As String,
                           datiJson As String,
                           objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreUtentiRetailDAL.Utenti_Retail_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.Append("INSERT INTO APP_UtentiRetail ( " & vbCrLf)
            StrSQL.Append("	ID, " & vbCrLf)
            StrSQL.Append("	JsonData, " & vbCrLf)
            StrSQL.Append("	Username_Creazione, " & vbCrLf)
            StrSQL.Append("	Username_Modifica " & vbCrLf)
            StrSQL.Append("	) VALUES ( " & vbCrLf)
            StrSQL.Append(String.Format(" '{0}', ", Agro_SQL_SaveText(codiceAttivazione)) & vbCrLf)
            StrSQL.Append(String.Format(" '{0}', ", Agro_SQL_SaveText(datiJson)) & vbCrLf)
            StrSQL.Append(String.Format(" '{0}', ", Agro_SQL_SaveText(objParametri.UtenteUsername)) & vbCrLf)
            StrSQL.Append(String.Format(" '{0}' ) ", Agro_SQL_SaveText(objParametri.UtenteUsername)) & vbCrLf)

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function
    Public Function CancellaLogicamenteUtenteCreato(codiceAttivazione As String,
                                                    objParametri As AgronicaCoreParametri) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreUtentiRetailDAL.Utenti_Retail_W.CancellaLogicamenteUtenteCreato()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.Append("UPDATE APP_UtentiRetail SET")
            StrSQL.Append(String.Format("	Username_Modifica = '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)) & vbCrLf)
            StrSQL.Append("	, Data_Modifica = GETDATE() " & vbCrLf)
            StrSQL.Append("	, cancellato = 1 " & vbCrLf)
            StrSQL.Append(String.Format("	WHERE ID = '{0}' ", Agro_SQL_SaveText(codiceAttivazione)) & vbCrLf)

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    Public Function LoggaCambioMail(ByVal vecchiaEmail As String,
                                    ByVal nuovaEmail As String,
                                    ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreUtentiRetailDAL.Utenti_Retail_W.LoggaCambioMail()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" INSERT INTO LOG_Utenti_CambioEMail ( ")
            StrSQL.AppendLine(" PIvaSuperUser ")
            StrSQL.AppendLine(" , UsernameOperazione ")
            StrSQL.AppendLine(" , Vecchia_Email ")
            StrSQL.AppendLine(" , Nuova_Email ")
            StrSQL.AppendLine(" , Username_Creazione ")
            StrSQL.AppendLine(" , Username_Modifica ")
            StrSQL.AppendLine(" ) VALUES ( ")

            StrSQL.AppendLine(String.Format(" '{0}' ", Agro_SQL_SaveText(objParametri.PivaSuperUser)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri.UtenteUsername)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(vecchiaEmail)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(nuovaEmail)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))

            StrSQL.AppendLine(" ) ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp
    End Function
End Class
