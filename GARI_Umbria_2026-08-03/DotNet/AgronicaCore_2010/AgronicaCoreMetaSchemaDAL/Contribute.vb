Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.DataProviderExtensions

Public Class Contribute_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Read(objServer As AgronicaCoreParametri,
                         Optional code As Int32 = 0,
                         Optional type As Int32 = 0,
                         Optional description As String = "",
                         Optional startValidity As Date = CostantiPersonalizzate.AGRODATAINIZIO,
                         Optional endValidity As Date = CostantiPersonalizzate.AGRODATAFINE) As DataTable

        Const routineName = "AgronicaCoreMetaSchemaDAL.Contribute_R.Read()"
        Dim strSQL As New Text.StringBuilder

        Try

            strSQL.AppendLine("SELECT *")
            strSQL.AppendLine("FROM Contributi")
            strSQL.AppendLine("WHERE 1 = 1")

            If code <> 0 Then
                strSQL.AppendLine($"    AND ContributoCod = {Agro_SQL_SaveNum(code)}")
            End If

            If type <> 0 Then
                strSQL.AppendLine($"    AND Tipo = {Agro_SQL_SaveNum(type)}")
            End If

            If Not String.IsNullOrEmpty(description) Then
                strSQL.AppendLine($"    AND ContributoDes LIKE '{Agro_SQL_SaveText(description)}%'")
            End If

            If startValidity <> CostantiPersonalizzate.AGRODATAINIZIO Then
                strSQL.AppendLine($"    AND Validita_Inizio <= {Agro_SQL_SaveDate(startValidity)}")
            End If

            If endValidity <> CostantiPersonalizzate.AGRODATAFINE Then
                strSQL.AppendLine($"    AND Validita_Fine >= {Agro_SQL_SaveDate(endValidity)}")
            End If

            Return EseguiQuery_Lettura(objServer, strSQL.ToString, routineName)

        Catch ex As Exception
            Scrivi_LOG(objServer, routineName, ex.Message)
            Throw New Exception("[" & routineName & "] : " & ex.Message)
        End Try

    End Function
End Class

Public Class Contribute_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Edit(code As Int32,
                         type As Int32,
                         objServer As AgronicaCoreParametri,
                         Optional description As String = "",
                         Optional startValidity As Date = CostantiPersonalizzate.AGRODATAINIZIO,
                         Optional endValidity As Date = CostantiPersonalizzate.AGRODATAFINE) As Boolean

        Const routineName = "AgronicaCoreMetaSchemaDAL.Contribute_W.Edit()"
        Dim strSQL As New Text.StringBuilder

        Try

            StrSQL.AppendLine("UPDATE Contributi")
            StrSQL.AppendLine("SET")
            StrSQL.AppendLine($"    Data_Modifica = {Agro_SQL_SaveDate(DateTime.Today)}")
            StrSQL.AppendLine($"    , Username_Modifica = '{Agro_SQL_SaveText(objServer.UsernameOperazione)}'")

            If Not String.IsNullOrEmpty(description) Then
                StrSQL.AppendLine($"    , ContributoDes = {Agro_SQL_SaveText(description)}")
            End If

            If startValidity <> CostantiPersonalizzate.AGRODATAINIZIO Then
                StrSQL.AppendLine($"    , Validita_Inizio = {Agro_SQL_SaveDate(startValidity)}")
            End If

            If endValidity <> CostantiPersonalizzate.AGRODATAFINE Then
                StrSQL.AppendLine($"    , Validita_Fine = {Agro_SQL_SaveDate(endValidity)}")
            End If

            StrSQL.AppendLine("")
            StrSQL.AppendLine("WHERE 1 = 1")

            StrSQL.AppendLine($"    AND ContributoCod = {Agro_SQL_SaveNum(code)}")
            strSQL.AppendLine($"    AND Tipo = {Agro_SQL_SaveNum(type)}")
            strSQL.AppendLine($"    AND NOT EXISTS (")
            strSQL.AppendLine($"        SELECT 1 FROM Imprese_ProgettiXContributi pxc")
            strSQL.AppendLine($"        WHERE pxc.ContributoCod = Contributi.ContributoCod")
            strSQL.AppendLine("             AND pxc.ContributoTipo = Contributi.Tipo")
            strSQL.AppendLine($"    )")

            Return EseguiQuery_Scrittura(objServer, StrSQL.ToString, routineName)

        Catch ex As Exception
            Scrivi_LOG(objServer, routineName, ex.Message)
            Throw New Exception("[" & routineName & "] : " & ex.Message)
        End Try

    End Function

    Public Function Delete(objServer As AgronicaCoreParametri,
                           Optional code As Int32 = 0,
                           Optional type As Int32 = 0,
                           Optional description As String = "",
                           Optional startValidity As Date = CostantiPersonalizzate.AGRODATAINIZIO,
                           Optional endValidity As Date = CostantiPersonalizzate.AGRODATAFINE) As Boolean

        Const routineName = "AgronicaCoreMetaSchemaDAL.Contribute_W.Delete()"
        Dim strSQL As New Text.StringBuilder

        Try

            strSQL.AppendLine("DELETE FROM Contributi")
            strSQL.AppendLine("WHERE NOT EXISTS (")
            StrSQL.AppendLine("    SELECT 1")
            strSQL.AppendLine("    FROM Imprese_ProgettiXContributi pxc")
            strSQL.AppendLine("    WHERE pxc.ContributoCod = Contributi.ContributoCod")
            strSQL.AppendLine("        AND pxc.ContributoTipo = Contributi.Tipo")
            strSQL.AppendLine(")")

            If code <> 0 Then
                strSQL.AppendLine($"    AND ContributoCod = {Agro_SQL_SaveNum(code)}")
            End If

            If type <> 0 Then
                strSQL.AppendLine($"    AND Tipo = {Agro_SQL_SaveNum(type)}")
            End If

            If Not String.IsNullOrEmpty(description) Then
                strSQL.AppendLine($"    AND ContributoDes LIKE '{Agro_SQL_SaveText(description)}'")
            End If

            If startValidity <> CostantiPersonalizzate.AGRODATAINIZIO Then
                strSQL.AppendLine($"    AND Validita_Fine >= {Agro_SQL_SaveDate(startValidity)}")
            End If

            If endValidity <> CostantiPersonalizzate.AGRODATAFINE Then
                strSQL.AppendLine($"    AND Validita_Inizio <= {Agro_SQL_SaveDate(endValidity)}")
            End If

            Return EseguiQuery_Scrittura(objServer, StrSQL.ToString, routineName)

        Catch ex As Exception
            Scrivi_LOG(objServer, routineName, ex.Message)
            Throw New Exception("[" & routineName & "] : " & ex.Message)
        End Try

    End Function
End Class
