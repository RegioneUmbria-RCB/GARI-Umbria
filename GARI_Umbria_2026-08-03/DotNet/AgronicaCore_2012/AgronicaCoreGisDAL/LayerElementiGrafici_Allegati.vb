Imports AgronicaCoreDataProvider

Public Class LayerElementiGrafici_Allegati_R
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function LeggiElencoAllegatiLayer(ByVal layerElementiGrafici_Cod As Int32,
                                             ByVal tipologiaLayer_Cod As Int32,
                                             ByRef objParametri As AgronicaCoreParametri,
                                             ByRef objParametri_Utente As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.LayerElementiGrafici_Allegati_R.LeggiElencoAllegatiLayer()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim connBuilder As New Common.DbConnectionStringBuilder
        Dim DT As DataTable

        Try
            connBuilder.ConnectionString = objParametri_Utente.StringaConnessione

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("       AL.Allegati_Documenti_Des ")
            StrSQL.AppendLine("       , AL.Allegati_Documenti_Cod ")
            StrSQL.AppendLine(" 	  , AL.Validita_Inizio ")
            StrSQL.AppendLine(" 	  , AL.Validita_Fine ")
            StrSQL.AppendLine(" 	  , AL.Data_Creazione ")
            StrSQL.AppendLine(" 	  , ISNULL(UD.Nome, '') NomeUtenteEstrazione ")
            StrSQL.AppendLine(" 	  , ISNULL(UD.Cognome, '') CognomeUtenteEstrazione ")
            StrSQL.AppendLine(" 	  , AL.Data_Creazione ")
            StrSQL.AppendLine(" FROM dbo.GIS_Allegati_Documenti AL ")
            StrSQL.AppendLine(" INNER JOIN dbo.GIS_Allegati_DocumentiXLayer ALXL ON ")
            StrSQL.AppendLine(" 	AL.Allegati_Documenti_Cod = ALXL.Allegati_Documenti_Cod AND ")
            StrSQL.AppendLine(" 	AL.Allegati_Documenti_SuperUser = ALXL.PivaSuperUser ")
            StrSQL.AppendLine(String.Format(" LEFT JOIN {0}.dbo.utenti_dettagli UD ON ", connBuilder("Initial Catalog")))
            StrSQL.AppendLine("     UD.CodFisc = AL.Username_Creazione ")

            StrSQL.AppendLine(String.Format(" WHERE ALXL.LayerElementiGrafici_Cod = {0} ", Agro_SQL_SaveNum(layerElementiGrafici_Cod)))
            StrSQL.AppendLine(String.Format(" AND ALXL.TipologiaLayer_Cod = {0} ", Agro_SQL_SaveNum(tipologiaLayer_Cod)))
            StrSQL.AppendLine(String.Format(" AND ALXL.PivaSuperUser = '{0}' ", Agro_SQL_SaveText(objParametri.PivaSuperUser)))

            If objParametri.UtenteUsername <> objParametri.SuperUserUsername Then
                StrSQL.AppendLine($"AND (")
                StrSQL.AppendLine($"    AL.Username_Creazione = '{objParametri.UtenteUsername}' OR AL.Username_Modifica = '{objParametri.UtenteUsername}'")
                StrSQL.AppendLine($"    OR AL.Username_Creazione = '{objParametri.UsernameOperazione}' OR AL.Username_Modifica = '{objParametri.UsernameOperazione}'")
                StrSQL.AppendLine($")")
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

    Public Function LeggiAllegatoFile(ByVal allegato_cod As Integer,
                                      ByRef objParametri_Server As AgronicaCoreParametri,
                                      ByRef objParametri_Utenti As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.LayerElementiGrafici_Allegati_R.LeggiAllegatoFile()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("       Allegati_Documenti_Cod ")
            StrSQL.AppendLine(" 	  , Allegati_Documenti_NomeFile ")
            StrSQL.AppendLine(" 	  , ISNULL(File_Allegato_DB, CAST('' as varbinary)) As File_Allegato_DB ")
            StrSQL.AppendLine(" FROM dbo.GIS_Allegati_Documenti ")

            StrSQL.AppendLine(String.Format(" WHERE Allegati_Documenti_Cod = {0} ", Agro_SQL_SaveNum(allegato_cod)))
            StrSQL.AppendLine(String.Format(" AND Allegati_Documenti_SuperUser = '{0}' ", Agro_SQL_SaveText(objParametri_Utenti.PivaSuperUser)))

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
Public Class LayerElementiGrafici_Allegati_W
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function ModificaEstrazioneShape(ByVal allegati_Documenti_Cod As Int32,
                                            ByVal descrizione As String,
                                            ByVal inizio_validita As Date,
                                            ByVal fine_validita As Date,
                                            ByRef objParametri As AgronicaCoreParametri) As Boolean

        Const nomeRoutine = "AgronicaCoreGisDAL.LayerElementiGrafici_Allegati_W.ModificaEstrazioneShape()"

        Dim messaggioErrore As String
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE GIS_Allegati_Documenti SET ")

            StrSQL.AppendLine(String.Format(" Allegati_Documenti_Des = '{0}' ", Agro_SQL_SaveText(descrizione)))
            StrSQL.AppendLine(String.Format(" , Validita_Inizio = {0} ", Agro_SQL_SaveDate(inizio_validita)))
            StrSQL.AppendLine(String.Format(" , Validita_Fine = {0} ", Agro_SQL_SaveDate(fine_validita)))
            StrSQL.AppendLine(String.Format(" , Username_Modifica = '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(" , Data_Modifica = GETDATE() ")

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" Allegati_Documenti_Cod = {0} ", Agro_SQL_SaveNum(allegati_Documenti_Cod)))

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
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
