Imports AgronicaCoreDataProvider

Public Class Permessi_Maschera_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiPermessiUtenteByMascheraCod(ByVal maschera_Cod As Integer,
                                                     ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.Permessi_Maschera_R.LeggiPermessiUtenteByMascheraCod()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ")

            StrSQL.AppendLine(" Maschera_Cod ")
            StrSQL.AppendLine(" , UserName ")
            StrSQL.AppendLine(" , Flag_Inserimento ")
            StrSQL.AppendLine(" , Flag_Modifica ")
            StrSQL.AppendLine(" , Flag_Cancellazione ")
            StrSQL.AppendLine(" , Flag_Informazioni ")
            StrSQL.AppendLine(" , Flag_Amministrazione ")
            StrSQL.AppendLine(" , Flag_Attivazione ")
            StrSQL.AppendLine(" FROM GIS_MascheraXUtente")

            StrSQL.AppendLine(" WHERE ")
            StrSQL.AppendLine(String.Format(" Maschera_Cod = {0} ", Agro_SQL_SaveNum(maschera_Cod)))

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

    Public Function LeggiPermessiGruppiUtenteByMascheraCod(ByVal maschera_Cod As Integer,
                                                           ByRef objParametri_Server As AgronicaCoreParametri,
                                                           ByRef objParametri_Utenti As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.Permessi_Maschera_R.LeggiPermessiGruppiUtenteByMascheraCod()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim connBuilder = New Common.DbConnectionStringBuilder
        connBuilder.ConnectionString = objParametri_Utenti.StringaConnessione

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ")

            StrSQL.AppendLine(" MACGU.Maschera_Cod ")
            StrSQL.AppendLine(" , MACGU.Gruppi_Utente_cod ")
            StrSQL.AppendLine(" , GU.Gruppi_Utente_des ")
            StrSQL.AppendLine(" , MACGU.Flag_Inserimento ")
            StrSQL.AppendLine(" , MACGU.Flag_Modifica ")
            StrSQL.AppendLine(" , MACGU.Flag_Cancellazione ")
            StrSQL.AppendLine(" , MACGU.Flag_Informazioni ")
            StrSQL.AppendLine(" , MACGU.Flag_Amministrazione ")
            StrSQL.AppendLine(" , Flag_Attivazione ")
            StrSQL.AppendLine(" FROM GIS_MascheraXGruppiUtente MACGU ")

            StrSQL.AppendLine(String.Format(" INNER JOIN {0}.dbo.Gruppi_Utente GU ", connBuilder("Initial Catalog")))

            StrSQL.AppendLine("     ON GU.Gruppi_Utente_Cod = MACGU.Gruppi_Utente_cod ")

            StrSQL.AppendLine(" WHERE ")
            StrSQL.AppendLine(String.Format(" MACGU.Maschera_Cod = {0} ", Agro_SQL_SaveNum(maschera_Cod)))

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function
    Public Function LeggiPermessiDaUtenteOGruppo(ByVal gruppi_appartenenza As List(Of Int32),
                                                 ByVal maschera_cod As Int32,
                                                 ByRef objParametri_Utente As AgronicaCoreParametri,
                                                 ByRef objParametri_Server As AgronicaCoreParametri
                                                 ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.Permessi_Maschera_R.LeggiPermessiDaUtenteOGruppo()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" Select Maschera_Cod ")
            StrSQL.AppendLine(" , Flag_Inserimento ")
            StrSQL.AppendLine(" , Flag_Modifica ")
            StrSQL.AppendLine(" , Flag_Cancellazione ")
            StrSQL.AppendLine(" , Flag_Informazioni ")
            StrSQL.AppendLine(" , Flag_Amministrazione ")
            StrSQL.AppendLine(" , Flag_Attivazione ")
            StrSQL.AppendLine(" FROM GIS_MascheraXUtente ")

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" UserName = '{0}' ", Agro_SQL_SaveText(objParametri_Utente.UtenteUsername)))

            If maschera_cod > 0 Then
                StrSQL.AppendLine(String.Format(" AND Maschera_Cod = {0} ", Agro_SQL_SaveNum(maschera_cod)))
            End If

            If gruppi_appartenenza IsNot Nothing AndAlso gruppi_appartenenza.Count > 0 Then

                StrSQL.AppendLine(" UNION ")

                StrSQL.AppendLine(" SELECT Maschera_Cod ")
                StrSQL.AppendLine(" , Flag_Inserimento ")
                StrSQL.AppendLine(" , Flag_Modifica ")
                StrSQL.AppendLine(" , Flag_Cancellazione ")
                StrSQL.AppendLine(" , Flag_Informazioni ")
                StrSQL.AppendLine(" , Flag_Amministrazione ")
                StrSQL.AppendLine(" , Flag_Attivazione ")
                StrSQL.AppendLine(" FROM GIS_MascheraXGruppiUtente ")

                StrSQL.AppendLine(" WHERE ")

                StrSQL.AppendLine(String.Format(" Gruppi_Utente_cod IN ({0}) ", String.Join(",", gruppi_appartenenza)))

                If maschera_cod > 0 Then
                    StrSQL.AppendLine(String.Format(" AND Maschera_Cod = {0} ", Agro_SQL_SaveNum(maschera_cod)))
                End If

            End If

            StrSQL.AppendLine(" ORDER BY Maschera_Cod ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

End Class
Public Class Permessi_Maschera_W
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function InsertPermesso(ByVal Flag_Inserimento As Int32,
                                   ByVal Flag_Modifica As Int32,
                                   ByVal Flag_Cancellazione As Int32,
                                   ByVal Flag_Informazioni As Int32,
                                   ByVal Flag_Amministrazione As Int32,
                                   ByVal Flag_Attivazione As Int32,
                                   ByVal Maschera_Cod As Int32,
                                   ByVal UserName As String,
                                   ByVal Gruppi_Utente_cod As Int32,
                                   ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.Permessi_Maschera_W.InsertPermesso()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        If UserName.Equals("") And Gruppi_Utente_cod = 0 Then
            Throw New Exception("E' necessario inserire almeno uno dei due valori tra UserName e Gruppi_Utente_cod.")
        End If

        Dim nomeTabella = "GIS_MascheraXGruppiUtente"

        If Not UserName.Equals("") Then
            nomeTabella = "GIS_MascheraXUtente"
        End If

        Try
            StrSQL.Length = 0
            StrSQL.AppendLine(String.Format("INSERT INTO {0} ( ", nomeTabella))
            StrSQL.AppendLine(" Maschera_Cod ")

            If Not UserName.Equals("") Then
                StrSQL.AppendLine(" , UserName ")
            Else
                StrSQL.AppendLine(" , Gruppi_Utente_cod ")
            End If

            StrSQL.AppendLine(" , Flag_Inserimento ")
            StrSQL.AppendLine(" , Flag_Modifica ")
            StrSQL.AppendLine(" , Flag_Cancellazione ")
            StrSQL.AppendLine(" , Flag_Informazioni ")
            StrSQL.AppendLine(" , Flag_Amministrazione ")
            StrSQL.AppendLine(" , Flag_Attivazione ")
            StrSQL.AppendLine(" , Username_Creazione ")
            StrSQL.AppendLine(" , Username_Modifica ")
            StrSQL.AppendLine(" ) VALUES (")

            StrSQL.AppendLine(String.Format(" {0} ", Agro_SQL_SaveNum(Maschera_Cod)))

            If Not UserName.Equals("") Then
                StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(UserName)))
            Else
                StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(Gruppi_Utente_cod)))
            End If

            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(Flag_Inserimento)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(Flag_Modifica)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(Flag_Cancellazione)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(Flag_Informazioni)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(Flag_Amministrazione)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(Flag_Attivazione)))

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

    Public Function DeletePermesso(ByVal Maschera_Cod As Int32,
                                   ByVal UserName As String,
                                   ByVal Gruppi_Utente_cod As Int32,
                                   ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.Permessi_Maschera_W.DeletePermesso()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        If UserName.Equals("") And Gruppi_Utente_cod = 0 Then
            Throw New Exception("E' necessario inserire almeno uno dei due valori tra UserName e Gruppi_Utente_cod.")
        End If

        Dim nomeTabella = "GIS_MascheraXGruppiUtente"

        If Not UserName.Equals("") Then
            nomeTabella = "GIS_MascheraXUtente"
        End If

        Try
            StrSQL.Length = 0
            StrSQL.AppendLine(String.Format("DELETE FROM {0} ", nomeTabella))
            StrSQL.AppendLine(" WHERE ")
            StrSQL.AppendLine(String.Format(" Maschera_Cod = {0} ", Agro_SQL_SaveNum(Maschera_Cod)))

            If Not UserName.Equals("") Then
                StrSQL.AppendLine(String.Format(" AND UserName = '{0}' ", Agro_SQL_SaveText(UserName)))
            Else
                StrSQL.AppendLine(String.Format(" AND Gruppi_Utente_cod = {0} ", Agro_SQL_SaveNum(Gruppi_Utente_cod)))
            End If

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

    Public Function DeletePermessiInteraMaschera(ByVal Maschera_Cod As Int32,
                                                 ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.Permessi_Maschera_W.DeletePermessiInteraMaschera()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine("DELETE FROM GIS_MascheraXUtente ")
            StrSQL.AppendLine(" WHERE ")
            StrSQL.AppendLine(String.Format(" Maschera_Cod = {0} ", Agro_SQL_SaveNum(Maschera_Cod)))

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            StrSQL.Length = 0

            StrSQL.AppendLine("DELETE FROM GIS_MascheraXGruppiUtente ")
            StrSQL.AppendLine(" WHERE ")
            StrSQL.AppendLine(String.Format(" Maschera_Cod = {0} ", Agro_SQL_SaveNum(Maschera_Cod)))

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


    Public Function UpdatePermesso(ByVal Flag_Inserimento As Int32,
                                   ByVal Flag_Modifica As Int32,
                                   ByVal Flag_Cancellazione As Int32,
                                   ByVal Flag_Informazioni As Int32,
                                   ByVal Flag_Amministrazione As Int32,
                                   ByVal Flag_Attivazione As Int32,
                                   ByVal Maschera_Cod As Int32,
                                   ByVal UserName As String,
                                   ByVal Gruppi_Utente_cod As Int32,
                                   ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.Permessi_Maschera_W.UpdatePermesso()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        If UserName.Equals("") And Gruppi_Utente_cod = 0 Then
            Throw New Exception("E' necessario inserire almeno uno dei due valori tra UserName e Gruppi_Utente_cod.")
        End If

        Try
            xRisp = DeletePermesso(Maschera_Cod, UserName, Gruppi_Utente_cod, objParametri)

            If Not xRisp Then
                Throw New Exception("Errore nella cancellazione dei permessi precedenti.")
            End If

            xRisp = InsertPermesso(Flag_Inserimento,
                                   Flag_Modifica,
                                   Flag_Cancellazione,
                                   Flag_Informazioni,
                                   Flag_Amministrazione,
                                   Flag_Attivazione,
                                   Maschera_Cod,
                                   UserName,
                                   Gruppi_Utente_cod,
                                   objParametri)

            If Not xRisp Then
                Throw New Exception("Errore nell'inserimento dei nuovi permessi.")
            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp
    End Function

End Class
