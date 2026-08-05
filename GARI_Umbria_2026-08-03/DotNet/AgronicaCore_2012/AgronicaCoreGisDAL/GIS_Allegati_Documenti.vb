Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class GIS_Allegati_R
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function UltimoID(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                             Optional ByVal username_creazione As String = "",
                             Optional ByVal username_modifica As String = "",
                             Optional ByVal data_upload As String = "",
                             Optional ByVal data_creazione As String = "",
                             Optional ByVal data_modifica As String = ""
                             ) As Integer

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Allegati_R.UltimoID()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT isnull(max(Allegati_Documenti_Cod),0) as MAX from GIS_Allegati_Documenti   ")

            If username_creazione <> "" Then
                StrSQL.AppendLine(" WHERE username_creazione = '" & Agro_SQL_SaveText(username_creazione) & "'  ")
            End If

            If username_modifica <> "" Then
                StrSQL.AppendLine(" AND username_modifica = '" & Agro_SQL_SaveText(username_modifica) & "'  ")
            End If

            If data_upload <> "" Then
                StrSQL.AppendLine(" AND data_upload =  CONVERT(DateTime,'" & Agro_SQL_SaveText(data_upload) & "',121)")
            End If

            If data_creazione <> "" Then
                StrSQL.AppendLine(" AND data_creazione =  CONVERT(DateTime,'" & Agro_SQL_SaveText(data_creazione) & "',121)")
            End If

            If data_modifica <> "" Then
                StrSQL.AppendLine(" AND data_modifica =  CONVERT(DateTime,'" & Agro_SQL_SaveText(data_modifica) & "',121)")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            Return 0

        End Try

        Return DT.Rows(0).Item("MAX")

    End Function

    Public Function LeggiAllegatoDaEntita(ByVal entita_cod As Int32,
                                          ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Allegati_R.LeggiAllegatoDaEntita()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("     E.Entita_Cod ")
            StrSQL.AppendLine("     , GAD.Allegati_Documenti_NomeFile ")
            StrSQL.AppendLine(" FROM GIS_Entita E ")
            StrSQL.AppendLine(" INNER JOIN GIS_Allegati_Documenti GAD ")
            StrSQL.AppendLine("     ON GAD.Allegati_Documenti_Cod = E.GIS_Allegati_Documenti_Cod ")

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format("     E.Entita_Cod = {0} ", Agro_SQL_SaveNum(entita_cod)))

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            Return Nothing
        End Try

        Return DT
    End Function
End Class
Public Class GIS_Allegati_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Allegati_Documenti_Piva As String,
                           ByVal Allegati_Documenti_Des As String,
                           ByVal Allegati_Documenti_NomeFile As String,
                           ByVal Allegati_Documenti_Estensione As String,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef OUTPUT_Allegati_Documenti_Cod As Integer,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreGisDAL.GIS_Allegati_W.Scrivi()"

        Dim messaggioErrore As String
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine("INSERT INTO GIS_Allegati_Documenti ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Allegati_Documenti_SuperUser, ")
            StrSQL.AppendLine(" Allegati_Documenti_Piva, ")
            StrSQL.AppendLine(" Allegati_Documenti_Des, ")
            StrSQL.AppendLine(" Allegati_Documenti_NomeFile, ")
            StrSQL.AppendLine(" Allegati_Documenti_Estensione, ")

            If username_creazione <> "" Then
                StrSQL.AppendLine(" UserName_Creazione, ")
            End If

            If username_modifica <> "" Then
                StrSQL.AppendLine(" Username_Modifica, ")
            End If

            StrSQL.AppendLine(" Validita_Inizio,  ")
            StrSQL.AppendLine(" Validita_Fine ")
            StrSQL.AppendLine(" ) VALUES ( ")

            StrSQL.AppendLine(String.Format(" '{0}' ", Agro_SQL_SaveText(objParametri.PivaSuperUser)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(Allegati_Documenti_Piva)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(Allegati_Documenti_Des)))

            If DataProviderFactory.Instance.TipoProvider = enum_DataProvidersType.SqlDataProvider Then
                StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText_UNICODE(Allegati_Documenti_NomeFile, "N")))
            Else
                StrSQL.AppendLine(String.Format(" , N'{0}' ", Agro_SQL_SaveText(Allegati_Documenti_NomeFile)))
            End If

            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(Allegati_Documenti_Estensione)))

            If username_creazione <> "" Then
                StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(username_creazione)))
            End If

            If username_modifica <> "" Then
                StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(username_modifica)))
            End If

            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveDate(Validita_Inizio)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveDate(Validita_Fine)))
            StrSQL.AppendLine(" ) ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------  

            Dim objleggi As New GIS_Allegati_R
            OUTPUT_Allegati_Documenti_Cod = objleggi.UltimoID(objParametri,
                                                              username_creazione:=username_creazione,
                                                              username_modifica:=username_modifica)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function ScriviAllegatoLayer(ByVal newAllegatoID As Int32,
                                        ByVal layerElementiGrafici_Cod As Int32,
                                        ByVal tipologiaLayer_Cod As Int32,
                                        ByRef objParametri As AgronicaCoreParametri,
                                        Optional ByVal override_username As String = "") As Boolean

        Const nomeRoutine = "AgronicaCoreGisDAL.GIS_Allegati_W.ScriviAllegatoLayer()"

        Dim messaggioErrore As String
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean

        Dim usernameOperazione = objParametri.UsernameOperazione

        If Not override_username.Equals("") Then
            usernameOperazione = override_username
        End If

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" INSERT INTO GIS_Allegati_DocumentiXLayer ( ")
            StrSQL.AppendLine(" PivaSuperUser ")
            StrSQL.AppendLine(" , Allegati_Documenti_Cod ")
            StrSQL.AppendLine(" , LayerElementiGrafici_Cod ")
            StrSQL.AppendLine(" , TipologiaLayer_Cod ")
            StrSQL.AppendLine(" , Username_Creazione ")
            StrSQL.AppendLine(" , Username_Modifica ")

            StrSQL.AppendLine(" ) VALUES ( ")

            StrSQL.AppendLine(String.Format(" '{0}' ", Agro_SQL_SaveText(objParametri.PivaSuperUser)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(newAllegatoID)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(layerElementiGrafici_Cod)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(tipologiaLayer_Cod)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(usernameOperazione)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(usernameOperazione)))

            StrSQL.AppendLine(" ) ")

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
