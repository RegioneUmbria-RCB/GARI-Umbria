Imports AgronicaCoreDTOStd.Identity
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.DataProviderExtensions

Public Class AppezzamentiXParcoMacchine_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Read(objServer As AgronicaCoreParametri,
                         Optional piva As String = "",
                         Optional saCod As Int32 = 0,
                         Optional appezza As Int32 = 0,
                         Optional macCod As Int32 = 0,
                         Optional classCode As String = "",
                         Optional startValidity As Date = CostantiPersonalizzate.AGRODATAINIZIO,
                         Optional endValidity As Date = CostantiPersonalizzate.AGRODATAFINE) As DataTable

        Const routineName = "AgronicaCoreAnagrafeDAL.AppezzamentiXParcoMacchine_R.Read()"
        Dim StrSQL As New Text.StringBuilder

        Try

            StrSQL.AppendLine("SELECT axp.*")

            If Not String.IsNullOrEmpty(classCode) Then
                StrSQL.AppendLine("    , pm.Class_Code")
            End If

            StrSQL.AppendLine("FROM AppezzamentiXParcoMacchine axp")

            If Not String.IsNullOrEmpty(classCode) Then
                StrSQL.AppendLine("JOIN Parco_Macchine pm ON pm.Mac_Cod = axp.Mac_Cod")
            End If

            StrSQL.AppendLine("WHERE 1 = 1")

            If Not String.IsNullOrEmpty(piva) Then
                StrSQL.AppendLine($"    AND axp.Piva = '{Agro_SQL_SaveText(piva)}'")
            End If

            If saCod <> 0 Then
                StrSQL.AppendLine($"    AND axp.Sa_Cod = {Agro_SQL_SaveNum(saCod)}")
            End If

            If appezza <> 0 Then
                StrSQL.AppendLine($"    AND axp.Appezza = {Agro_SQL_SaveNum(appezza)}")
            End If

            If macCod <> 0 Then
                StrSQL.AppendLine($"    AND axp.Mac_Cod = {Agro_SQL_SaveNum(macCod)}")
            End If

            If Not String.IsNullOrEmpty(classCode) Then
                StrSQL.AppendLine($"    AND pm.Class_Code LIKE '{Agro_SQL_SaveText(classCode)}%'")
            End If

            If startValidity <> CostantiPersonalizzate.AGRODATAINIZIO Then
                StrSQL.AppendLine($"    AND axp.Validita_Inizio <= {Agro_SQL_SaveDate(endValidity)}")
            End If

            If endValidity <> CostantiPersonalizzate.AGRODATAFINE Then
                StrSQL.AppendLine($"    AND axp.Validita_Fine >= {Agro_SQL_SaveDate(startValidity)}")
            End If

            Return EseguiQuery_Lettura(objServer, StrSQL.ToString, routineName)

        Catch ex As Exception
            Scrivi_LOG(objServer, routineName, ex.Message)
            Throw New Exception("[" & routineName & "] : " & ex.Message)
        End Try
    End Function

    Public Function ReadJoinDescriptions(objServer As AgronicaCoreParametri,
                                         Optional piva As String = "",
                                         Optional saCod As Int32 = 0,
                                         Optional appezza As Int32 = 0,
                                         Optional macCod As Int32 = 0,
                                         Optional classCode As String = "",
                                         Optional startValidity As Date = CostantiPersonalizzate.AGRODATAINIZIO,
                                         Optional endValidity As Date = CostantiPersonalizzate.AGRODATAFINE) As DataTable

        Const routinName = "AgronicaCoreAnagrafeDAL.AppezzamentiXParcoMacchine_R.Read()"
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.AppendLine("SELECT axp.*")
            StrSQL.AppendLine("    , pm.Mac_Des")
            StrSQL.AppendLine("    , app.APP_NOME")

            If Not String.IsNullOrEmpty(classCode) Then
                StrSQL.AppendLine("    , pm.Class_Code")
            End If

            StrSQL.AppendLine("FROM AppezzamentiXParcoMacchine axp")
            StrSQL.AppendLine("JOIN Parco_Macchine pm ON pm.Mac_Cod = axp.Mac_Cod")
            StrSQL.AppendLine("JOIN Appezzamento app ON app.PIVA = axp.Piva")
            StrSQL.AppendLine("    AND app.SA_COD = axp.Sa_Cod")
            StrSQL.AppendLine("    AND app.APPEZZA = axp.Appezza")

            If Not String.IsNullOrEmpty(classCode) Then
                StrSQL.AppendLine("JOIN Parco_Macchine pm ON pm.Mac_Cod = axp.Mac_Cod")
            End If

            StrSQL.AppendLine("WHERE 1 = 1")

            If Not String.IsNullOrEmpty(piva) Then
                StrSQL.AppendLine($"    AND axp.Piva = '{Agro_SQL_SaveText(piva)}'")
            End If

            If saCod <> 0 Then
                StrSQL.AppendLine($"    AND axp.Sa_Cod = {Agro_SQL_SaveNum(saCod)}")
            End If

            If appezza <> 0 Then
                StrSQL.AppendLine($"    AND axp.Appezza = {Agro_SQL_SaveNum(appezza)}")
            End If

            If macCod <> 0 Then
                StrSQL.AppendLine($"    AND axp.Mac_Cod = {Agro_SQL_SaveNum(macCod)}")
            End If

            If Not String.IsNullOrEmpty(classCode) Then
                StrSQL.AppendLine($"    AND pm.Class_Code LIKE '{Agro_SQL_SaveText(classCode)}%'")
            End If

            If startValidity <> CostantiPersonalizzate.AGRODATAINIZIO Then
                StrSQL.AppendLine($"    AND axp.Validita_Inizio <= {Agro_SQL_SaveDate(startValidity)}")
            End If

            If endValidity <> CostantiPersonalizzate.AGRODATAFINE Then
                StrSQL.AppendLine($"    AND axp.Validita_Fine >= {Agro_SQL_SaveDate(endValidity)}")
            End If

            Return EseguiQuery_Lettura(objServer, StrSQL.ToString, routinName)

        Catch ex As Exception
            Scrivi_LOG(objServer, routinName, ex.Message)
            Throw New Exception("[" & routinName & "] : " & ex.Message)
        End Try
    End Function

    Public Function ReadJoinLettureContatori(objServer As AgronicaCoreParametri,
                                             Optional piva As String = "",
                                             Optional saCod As Int32 = 0,
                                             Optional appezza As Int32 = 0,
                                             Optional macCod As Int32 = 0) As DataTable

        Const routinName = "AgronicaCoreAnagrafeDAL.AppezzamentiXParcoMacchine_R.ReadJoinLettureContatori()"
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.AppendLine("SELECT")
            StrSQL.AppendLine("    axp.Piva")
            StrSQL.AppendLine("    , axp.Sa_Cod")
            StrSQL.AppendLine("    , axp.Appezza")
            StrSQL.AppendLine("    , axp.Mac_Cod")
            StrSQL.AppendLine("    , axp.Validita_Inizio")
            StrSQL.AppendLine("    , axp.Validita_Fine")
            StrSQL.AppendLine("    , lca.Id")
            StrSQL.AppendLine("    , lca.PIVA")
            StrSQL.AppendLine("    , lca.Id_Contatore")
            StrSQL.AppendLine("    , lca.DataLettura")
            StrSQL.AppendLine("    , lca.Valore")
            StrSQL.AppendLine("FROM AppezzamentiXParcoMacchine axp")
            StrSQL.AppendLine("    INNER JOIN LettureContatoriAziendali lca ON lca.Id_Contatore = axp.Mac_Cod")
            StrSQL.AppendLine("WHERE 1 = 1")

            If Not String.IsNullOrEmpty(piva) Then
                StrSQL.AppendLine($"    AND axp.Piva = '{Agro_SQL_SaveText(piva)}'")
            End If

            If saCod <> 0 Then
                StrSQL.AppendLine($"    AND axp.Sa_Cod = {Agro_SQL_SaveNum(saCod)}")
            End If

            If appezza <> 0 Then
                StrSQL.AppendLine($"    AND axp.Appezza = {Agro_SQL_SaveNum(appezza)}")
            End If

            If macCod <> 0 Then
                StrSQL.AppendLine($"    AND axp.Mac_Cod = {Agro_SQL_SaveNum(macCod)}")
            End If

            Return EseguiQuery_Lettura(objServer, StrSQL.ToString, routinName)

        Catch ex As Exception
            Scrivi_LOG(objServer, routinName, ex.Message)
            Throw New Exception("[" & routinName & "] : " & ex.Message)
        End Try
    End Function
End Class

Public Class AppezzamentiXParcoMacchine_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Write(piva As String,
                          saCod As Int32,
                          appezza As Int32,
                          macCod As Int32,
                          validity As AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale,
                          objServer As AgronicaCoreParametri) As Boolean

        Const routineName = "AgronicaCoreAnagrafeDAL.AppezzamentiXParcoMacchine_W.Write()"
        Dim StrSQL As New Text.StringBuilder

        Try

            'StrSQL.AppendLine("MERGE AppezzamentiXParcomacchine WITH (HOLDLOCK) AS axp")
            'StrSQL.AppendLine("    USING (VALUES")
            'StrSQL.AppendLine("        (")
            'StrSQL.AppendLine($"        '{Agro_SQL_SaveText(piva)}'")
            'StrSQL.AppendLine($"        , {Agro_SQL_SaveNum(saCod)}")
            'StrSQL.AppendLine($"        , {Agro_SQL_SaveNum(appezza)}")
            'StrSQL.AppendLine($"        , {Agro_SQL_SaveNum(macCod)}")
            'StrSQL.AppendLine($"        , {0}")
            'StrSQL.AppendLine($"        , NULL")
            'StrSQL.AppendLine($"        , {Agro_SQL_SaveDate(DateTime.Today)}")
            'StrSQL.AppendLine($"        , {Agro_SQL_SaveDate(DateTime.Today)}")
            'StrSQL.AppendLine($"        , '{objServer.UsernameOperazione}'")
            'StrSQL.AppendLine($"        , '{objServer.UsernameOperazione}'")
            ''StrSQL.AppendLine($"        , {Agro_SQL_SaveDate(validity.inizio)}")
            ''StrSQL.AppendLine($"        , {Agro_SQL_SaveDate(validity.fine)}")
            'StrSQL.AppendLine("        )")
            'StrSQL.AppendLine("    ) AS v(")
            'StrSQL.AppendLine("          Piva")
            'StrSQL.AppendLine("        , Sa_Cod")
            'StrSQL.AppendLine("        , Appezza")
            'StrSQL.AppendLine("        , Mac_Cod")
            'StrSQL.AppendLine("        , inviato")
            'StrSQL.AppendLine("        , datainvio")
            'StrSQL.AppendLine("        , Data_Creazione")
            'StrSQL.AppendLine("        , Data_Modifica")
            'StrSQL.AppendLine("        , Username_Creazione")
            'StrSQL.AppendLine("        , Username_Modifica")
            ''StrSQL.AppendLine("        , Validita_Inizio")
            ''StrSQL.AppendLine("        , Validita_Fine")
            'StrSQL.AppendLine("    )")
            'StrSQL.AppendLine("    ON v.Piva = axp.Piva")
            'StrSQL.AppendLine("        AND v.Sa_Cod = axp.Sa_Cod")
            'StrSQL.AppendLine("        AND v.Appezza = axp.Appezza")
            'StrSQL.AppendLine("        AND v.Mac_Cod = axp.Mac_Cod")
            'StrSQL.AppendLine("")
            'StrSQL.AppendLine("    WHEN MATCHED")
            'StrSQL.AppendLine("        THEN")
            'StrSQL.AppendLine("            UPDATE")
            'StrSQL.AppendLine("            SET Validita_Inizio = v.Validita_Inizio")
            'StrSQL.AppendLine("                , Validita_Fine = v.Validita_Fine")
            'StrSQL.AppendLine("    WHEN NOT MATCHED")
            'StrSQL.AppendLine("        THEN")
            'StrSQL.AppendLine("            INSERT (")
            'StrSQL.AppendLine("                Piva")
            'StrSQL.AppendLine("                , Sa_Cod")
            'StrSQL.AppendLine("                , Appezza")
            'StrSQL.AppendLine("                , Mac_Cod")
            'StrSQL.AppendLine("                , inviato")
            'StrSQL.AppendLine("                , datainvio")
            'StrSQL.AppendLine("                , Data_Creazione")
            'StrSQL.AppendLine("                , Data_Modifica")
            'StrSQL.AppendLine("                , Username_Creazione")
            'StrSQL.AppendLine("                , Username_Modifica")
            ''StrSQL.AppendLine("                , Validita_Inizio")
            ''StrSQL.AppendLine("                , Validita_Fine")
            'StrSQL.AppendLine("            )")
            'StrSQL.AppendLine("            VALUES (")
            'StrSQL.AppendLine("                v.Piva")
            'StrSQL.AppendLine("                , v.Sa_Cod")
            'StrSQL.AppendLine("                , v.Appezza")
            'StrSQL.AppendLine("                , v.Mac_Cod")
            'StrSQL.AppendLine("                , v.inviato")
            'StrSQL.AppendLine("                , v.datainvio")
            'StrSQL.AppendLine("                , v.Data_Creazione")
            'StrSQL.AppendLine("                , v.Data_Modifica")
            'StrSQL.AppendLine("                , v.Username_Creazione")
            'StrSQL.AppendLine("                , v.Username_Modifica")
            ''StrSQL.AppendLine("                , v.Validita_Inizio")
            ''StrSQL.AppendLine("                , v.Validita_Fine")
            'StrSQL.AppendLine("            );")

            StrSQL.AppendLine("IF NOT EXISTS")
            StrSQL.AppendLine("(")
            StrSQL.AppendLine("    SELECT 1")
            StrSQL.AppendLine("    FROM AppezzamentiXParcoMacchine")
            StrSQL.AppendLine($"    WHERE Piva = '{Agro_SQL_SaveText(piva)}'")
            StrSQL.AppendLine($"        AND Sa_Cod = {Agro_SQL_SaveNum(saCod)}")
            StrSQL.AppendLine($"        AND Appezza = {Agro_SQL_SaveNum(appezza)}")
            StrSQL.AppendLine($"        AND Mac_Cod = {Agro_SQL_SaveNum(macCod)}")
            StrSQL.AppendLine(")")
            StrSQL.AppendLine("    BEGIN")
            StrSQL.AppendLine("    INSERT AppezzamentiXParcoMacchine")
            StrSQL.AppendLine("    (")
            StrSQL.AppendLine("        Piva")
            StrSQL.AppendLine("        , Sa_Cod")
            StrSQL.AppendLine("        , Appezza")
            StrSQL.AppendLine("        , Mac_Cod")
            StrSQL.AppendLine("        , inviato")
            StrSQL.AppendLine("        , datainvio")
            StrSQL.AppendLine("        , Data_Creazione")
            StrSQL.AppendLine("        , Data_Modifica")
            StrSQL.AppendLine("        , Username_Creazione")
            StrSQL.AppendLine("        , Username_Modifica")
            StrSQL.AppendLine("        , Validita_Inizio")
            StrSQL.AppendLine("        , Validita_Fine")
            StrSQL.AppendLine("    )")
            StrSQL.AppendLine("    VALUES")
            StrSQL.AppendLine("    (")
            StrSQL.AppendLine($"        '{Agro_SQL_SaveText(piva)}'")
            StrSQL.AppendLine($"        , {Agro_SQL_SaveNum(saCod)}")
            StrSQL.AppendLine($"        , {Agro_SQL_SaveNum(appezza)}")
            StrSQL.AppendLine($"        , {Agro_SQL_SaveNum(macCod)}")
            StrSQL.AppendLine($"        , {0}")
            StrSQL.AppendLine($"        , NULL")
            StrSQL.AppendLine($"        , {Agro_SQL_SaveDate(DateTime.Today)}")
            StrSQL.AppendLine($"        , {Agro_SQL_SaveDate(DateTime.Today)}")
            StrSQL.AppendLine($"        , '{objServer.UsernameOperazione}'")
            StrSQL.AppendLine($"        , '{objServer.UsernameOperazione}'")
            StrSQL.AppendLine($"        , {Agro_SQL_SaveDate(validity.inizio)}")
            StrSQL.AppendLine($"        , {Agro_SQL_SaveDate(validity.fine)}")
            StrSQL.AppendLine("    )")
            StrSQL.AppendLine("    END;")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("ELSE")
            StrSQL.AppendLine("    BEGIN")
            StrSQL.AppendLine("    UPDATE AppezzamentiXParcoMacchine")
            StrSQL.AppendLine("    SET")
            StrSQL.AppendLine($"        Data_Modifica =  {Agro_SQL_SaveDate(DateTime.Today)}")
            StrSQL.AppendLine($"        , Username_Modifica =  '{objServer.UsernameOperazione}'")
            StrSQL.AppendLine($"        , Validita_Inizio =  {Agro_SQL_SaveDate(validity.inizio)}")
            StrSQL.AppendLine($"        , Validita_Fine =  {Agro_SQL_SaveDate(validity.fine)}")
            StrSQL.AppendLine("")
            StrSQL.AppendLine($"    WHERE Piva = '{Agro_SQL_SaveText(piva)}'")
            StrSQL.AppendLine($"        AND Sa_Cod = {Agro_SQL_SaveNum(saCod)}")
            StrSQL.AppendLine($"        AND Appezza = {Agro_SQL_SaveNum(appezza)}")
            StrSQL.AppendLine($"        AND Mac_Cod = {Agro_SQL_SaveNum(macCod)}")
            StrSQL.AppendLine("    END;")



            Return EseguiQuery_Scrittura(objServer, StrSQL.ToString, routineName)

        Catch ex As Exception
            Scrivi_LOG(objServer, routineName, ex.Message)
            Throw New Exception("[" & routineName & "] : " & ex.Message)
        End Try
    End Function

    Public Function Edit(validity As AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale,
                         objServer As AgronicaCoreParametri,
                         Optional piva As String = "",
                         Optional saCod As Int32 = 0,
                         Optional appezza As Int32 = 0,
                         Optional macCod As Int32 = 0)

        Const routineName = "AgronicaCoreAnagrafeDAL.AppezzamentiXParcoMacchine_W.Edit()"
        Dim StrSQL As New Text.StringBuilder

        Try

            StrSQL.AppendLine("UPDATE AppezzamentiXParcoMacchine")
            StrSQL.AppendLine("SET")
            StrSQL.AppendLine($"    Data_Modifica = {Agro_SQL_SaveDate(DateTime.Today)}")
            StrSQL.AppendLine($"    , Username_Modifica = '{objServer.UsernameOperazione}'")
            StrSQL.AppendLine($"    , Validita_Inizio = {Agro_SQL_SaveDate(validity.inizio)}")
            StrSQL.AppendLine($"    , Validita_Fine = {Agro_SQL_SaveDate(validity.fine)}")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("WHERE 1 = 1")

            If Not String.IsNullOrEmpty(piva) Then
                StrSQL.AppendLine($"    AND Piva = '{Agro_SQL_SaveText(piva)}'")
            End If

            If saCod <> 0 Then
                StrSQL.AppendLine($"    AND Sa_Cod = {Agro_SQL_SaveNum(saCod)}")
            End If

            If appezza <> 0 Then
                StrSQL.AppendLine($"    AND Appezza = {Agro_SQL_SaveNum(appezza)}")
            End If

            If macCod <> 0 Then
                StrSQL.AppendLine($"    AND Mac_Cod = {Agro_SQL_SaveNum(macCod)}")
            End If

            Return EseguiQuery_Scrittura(objServer, StrSQL.ToString, routineName)

        Catch ex As Exception
            Scrivi_LOG(objServer, routineName, ex.Message)
            Throw New Exception("[" & routineName & "] : " & ex.Message)
        End Try

    End Function

    Public Function Delete(objServer As AgronicaCoreParametri,
                           Optional piva As String = "",
                           Optional saCod As Int32 = 0,
                           Optional appezza As Int32 = 0,
                           Optional macCod As Int32 = 0,
                           Optional classCode As String = "",
                           Optional startValidity As Date = CostantiPersonalizzate.AGRODATAINIZIO,
                           Optional endValidity As Date = CostantiPersonalizzate.AGRODATAFINE) As Boolean

        Const routineName = "AgronicaCoreAnagrafeDAL.AppezzamentiXParcoMacchine_W.Delete()"
        Dim StrSQL As New Text.StringBuilder

        Try

            StrSQL.AppendLine("DELETE FROM AppezzamentiXParcoMacchine")
            StrSQL.AppendLine("WHERE NOT EXISTS (")
            StrSQL.AppendLine("    SELECT 1")
            StrSQL.AppendLine("    FROM LettureContatoriAziendali lca")
            StrSQL.AppendLine("    WHERE lca.Id_Contatore = AppezzamentiXParcoMacchine.Mac_Cod")
            StrSQL.AppendLine(")")

            If Not String.IsNullOrEmpty(piva) Then
                StrSQL.AppendLine($"    AND AppezzamentiXParcoMacchine.Piva = '{Agro_SQL_SaveText(piva)}'")
            End If

            If saCod <> 0 Then
                StrSQL.AppendLine($"    AND AppezzamentiXParcoMacchine.Sa_Cod = {Agro_SQL_SaveNum(saCod)}")
            End If

            If appezza <> 0 Then
                StrSQL.AppendLine($"    AND AppezzamentiXParcoMacchine.Appezza = {Agro_SQL_SaveNum(appezza)}")
            End If

            If macCod <> 0 Then
                StrSQL.AppendLine($"    AND AppezzamentiXParcoMacchine.Mac_Cod = {Agro_SQL_SaveNum(macCod)}")
            End If

            If Not String.IsNullOrEmpty(classCode) Then
                StrSQL.AppendLine($"    AND EXISTS (")
                StrSQL.AppendLine($"        SELECT 1 FROM Parco_Macchine")
                StrSQL.AppendLine($"        WHERE AppezzamentiXParcoMacchine.Mac_Cod = Parco_Macchine.Mac_Cod")
                StrSQL.AppendLine($"        AND Parco_Macchine.Class_Code = '{Agro_SQL_SaveText(classCode)}'")
                StrSQL.AppendLine($"    )")
            End If

            If startValidity <> CostantiPersonalizzate.AGRODATAINIZIO Then
                StrSQL.AppendLine($"    AND AppezzamentiXParcoMacchine.Validita_Fine >= {Agro_SQL_SaveDate(startValidity)}")
            End If

            If endValidity <> CostantiPersonalizzate.AGRODATAFINE Then
                StrSQL.AppendLine($"    AND AppezzamentiXParcoMacchine.Validita_Inizio <= {Agro_SQL_SaveDate(endValidity)}")
            End If

            Return EseguiQuery_Scrittura(objServer, StrSQL.ToString, routineName)

        Catch ex As Exception
            Scrivi_LOG(objServer, routineName, ex.Message)
            Throw New Exception("[" & routineName & "] : " & ex.Message)
        End Try
    End Function
End Class
