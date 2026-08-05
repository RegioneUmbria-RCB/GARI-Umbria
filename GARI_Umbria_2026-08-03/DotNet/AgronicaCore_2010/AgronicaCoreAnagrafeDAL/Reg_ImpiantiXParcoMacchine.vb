Imports AgronicaCoreDataProvider

Public Class Reg_ImpiantiXParcoMacchine_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Read(objServer As AgronicaCoreParametri,
                         Optional piva As String = "",
                         Optional saCod As Int32 = 0,
                         Optional appezza As Int32 = 0,
                         Optional idReg As Int32 = 0,
                         Optional macCod As Int32 = 0,
                         Optional classCode As String = "",
                         Optional startValidity As Date = CostantiPersonalizzate.AGRODATAINIZIO,
                         Optional endValidity As Date = CostantiPersonalizzate.AGRODATAFINE) As DataTable

        Const routineName = "AgronicaCoreAnagrafeDAL.Reg_ImpiantiXParcoMacchine_R.Read()"
        Dim StrSQL As New Text.StringBuilder

        Try

            StrSQL.AppendLine("SELECT rixp.*")

            If Not String.IsNullOrEmpty(classCode) Then
                StrSQL.AppendLine("    , pm.Class_Code")
            End If

            StrSQL.AppendLine("FROM Reg_ImpiantiXParcoMacchine rixp")

            If Not String.IsNullOrEmpty(classCode) Then
                StrSQL.AppendLine("JOIN Parco_Macchine pm ON pm.Mac_Cod = rixp.Mac_Cod")
            End If

            StrSQL.AppendLine("WHERE 1 = 1")

            If Not String.IsNullOrEmpty(piva) Then
                StrSQL.AppendLine($"    AND rixp.Piva = '{Agro_SQL_SaveText(piva)}'")
            End If

            If saCod <> 0 Then
                StrSQL.AppendLine($"    AND rixp.Sa_Cod = {Agro_SQL_SaveNum(saCod)}")
            End If

            If appezza <> 0 Then
                StrSQL.AppendLine($"    AND rixp.Appezza = {Agro_SQL_SaveNum(appezza)}")
            End If

            If idReg <> 0 Then
                StrSQL.AppendLine($"    AND rixp.ID_Reg = {Agro_SQL_SaveNum(idReg)}")
            End If

            If macCod <> 0 Then
                StrSQL.AppendLine($"    AND rixp.Mac_Cod = {Agro_SQL_SaveNum(macCod)}")
            End If

            If Not String.IsNullOrEmpty(classCode) Then
                StrSQL.AppendLine($"    AND pm.Class_Code LIKE '{Agro_SQL_SaveText(classCode)}%'")
            End If

            If startValidity <> CostantiPersonalizzate.AGRODATAINIZIO Then
                StrSQL.AppendLine($"    AND rixp.Validita_Inizio <= {Agro_SQL_SaveDate(endValidity)}")
            End If

            If endValidity <> CostantiPersonalizzate.AGRODATAFINE Then
                StrSQL.AppendLine($"    AND rixp.Validita_Fine >= {Agro_SQL_SaveDate(startValidity)}")
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
                                         Optional idReg As Int32 = 0,
                                         Optional macCod As Int32 = 0,
                                         Optional classCode As String = "",
                                         Optional startValidity As Date = CostantiPersonalizzate.AGRODATAINIZIO,
                                         Optional endValidity As Date = CostantiPersonalizzate.AGRODATAFINE) As DataTable

        Const routinName = "AgronicaCoreAnagrafeDAL.Reg_ImpiantiXParcoMacchine_R.ReadJoinDescriptions()"
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.AppendLine("SELECT rixp.*")
            StrSQL.AppendLine("    , pm.Mac_Des")
            StrSQL.AppendLine("    , pm.portata")
            StrSQL.AppendLine("    , pm.efficienza")
            StrSQL.AppendLine("    , pm.Class_Code")
            StrSQL.AppendLine("    , pm.IMP_COD")
            StrSQL.AppendLine("    , app.APP_NOME")

            StrSQL.AppendLine("FROM Reg_ImpiantiXParcoMacchine rixp")
            StrSQL.AppendLine("JOIN Parco_Macchine pm ON pm.Mac_Cod = rixp.Mac_Cod")
            StrSQL.AppendLine("JOIN Appezzamento app ON app.PIVA = rixp.Piva")
            StrSQL.AppendLine("    AND app.SA_COD = rixp.Sa_Cod")
            StrSQL.AppendLine("    AND app.APPEZZA = rixp.Appezza")

            StrSQL.AppendLine("WHERE 1 = 1")

            If Not String.IsNullOrEmpty(piva) Then
                StrSQL.AppendLine($"    AND rixp.Piva = '{Agro_SQL_SaveText(piva)}'")
            End If

            If saCod <> 0 Then
                StrSQL.AppendLine($"    AND rixp.Sa_Cod = {Agro_SQL_SaveNum(saCod)}")
            End If

            If appezza <> 0 Then
                StrSQL.AppendLine($"    AND rixp.Appezza = {Agro_SQL_SaveNum(appezza)}")
            End If

            If idReg <> 0 Then
                StrSQL.AppendLine($"    AND rixp.ID_Reg = {Agro_SQL_SaveNum(idReg)}")
            End If

            If macCod <> 0 Then
                StrSQL.AppendLine($"    AND rixp.Mac_Cod = {Agro_SQL_SaveNum(macCod)}")
            End If

            If Not String.IsNullOrEmpty(classCode) Then
                StrSQL.AppendLine($"    AND pm.Class_Code LIKE '{Agro_SQL_SaveText(classCode)}%'")
            End If

            If startValidity <> CostantiPersonalizzate.AGRODATAINIZIO Then
                StrSQL.AppendLine($"    AND rixp.Validita_Inizio <= {Agro_SQL_SaveDate(startValidity)}")
            End If

            If endValidity <> CostantiPersonalizzate.AGRODATAFINE Then
                StrSQL.AppendLine($"    AND rixp.Validita_Fine >= {Agro_SQL_SaveDate(endValidity)}")
            End If

            Return EseguiQuery_Lettura(objServer, StrSQL.ToString, routinName)

        Catch ex As Exception
            Scrivi_LOG(objServer, routinName, ex.Message)
            Throw New Exception("[" & routinName & "] : " & ex.Message)
        End Try
    End Function

    Public Function ReadJoinDescriptions_Budget(objServer As AgronicaCoreParametri,
                                                 Optional idBudget As Integer = 0,
                                                 Optional piva As String = "",
                                                 Optional saCod As Int32 = 0,
                                                 Optional appezza As Int32 = 0,
                                                 Optional idReg As Int32 = 0,
                                                 Optional macCod As Int32 = 0,
                                                 Optional classCode As String = "",
                                                 Optional startValidity As Date = CostantiPersonalizzate.AGRODATAINIZIO,
                                                 Optional endValidity As Date = CostantiPersonalizzate.AGRODATAFINE) As DataTable

        Const routinName = "AgronicaCoreAnagrafeDAL.Reg_ImpiantiXParcoMacchine_R.ReadJoinDescriptions()"
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.AppendLine("SELECT rixp.*")
            StrSQL.AppendLine("    , pm.Mac_Des")
            StrSQL.AppendLine("    , pm.portata")
            StrSQL.AppendLine("    , pm.efficienza")
            StrSQL.AppendLine("    , pm.Class_Code")
            StrSQL.AppendLine("    , pm.IMP_COD")
            StrSQL.AppendLine("    , app.APP_NOME")

            StrSQL.AppendLine("FROM Budget_Reg_ImpiantiXParcoMacchine rixp")
            StrSQL.AppendLine("JOIN Parco_Macchine pm ON pm.Mac_Cod = rixp.Mac_Cod")
            StrSQL.AppendLine("JOIN Budget_Appezzamento app ON app.Id_Budget = rixp.Id_Budget")
            StrSQL.AppendLine("    AND app.PIVA = rixp.Piva")
            StrSQL.AppendLine("    AND app.SA_COD = rixp.Sa_Cod")
            StrSQL.AppendLine("    AND app.APPEZZA = rixp.Appezza")

            StrSQL.AppendLine("WHERE 1 = 1")

            If idBudget <> 0 Then
                StrSQL.AppendLine($"    AND rixp.Id_Budget = {Agro_SQL_SaveNum(idBudget)}")
            End If

            If Not String.IsNullOrEmpty(piva) Then
                StrSQL.AppendLine($"    AND rixp.Piva = '{Agro_SQL_SaveText(piva)}'")
            End If

            If saCod <> 0 Then
                StrSQL.AppendLine($"    AND rixp.Sa_Cod = {Agro_SQL_SaveNum(saCod)}")
            End If

            If appezza <> 0 Then
                StrSQL.AppendLine($"    AND rixp.Appezza = {Agro_SQL_SaveNum(appezza)}")
            End If

            If idReg <> 0 Then
                StrSQL.AppendLine($"    AND rixp.ID_Reg = {Agro_SQL_SaveNum(idReg)}")
            End If

            If macCod <> 0 Then
                StrSQL.AppendLine($"    AND rixp.Mac_Cod = {Agro_SQL_SaveNum(macCod)}")
            End If

            If Not String.IsNullOrEmpty(classCode) Then
                StrSQL.AppendLine($"    AND pm.Class_Code LIKE '{Agro_SQL_SaveText(classCode)}%'")
            End If

            If startValidity <> CostantiPersonalizzate.AGRODATAINIZIO Then
                StrSQL.AppendLine($"    AND rixp.Validita_Inizio <= {Agro_SQL_SaveDate(startValidity)}")
            End If

            If endValidity <> CostantiPersonalizzate.AGRODATAFINE Then
                StrSQL.AppendLine($"    AND rixp.Validita_Fine >= {Agro_SQL_SaveDate(endValidity)}")
            End If

            Return EseguiQuery_Lettura(objServer, StrSQL.ToString, routinName)

        Catch ex As Exception
            Scrivi_LOG(objServer, routinName, ex.Message)
            Throw New Exception("[" & routinName & "] : " & ex.Message)
        End Try
    End Function

End Class

Public Class Reg_ImpiantiXParcoMacchine_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Write(piva As String,
                          saCod As Integer,
                          appezza As Integer,
                          idReg As Integer,
                          macCod As Integer,
                          objServer As AgronicaCoreParametri,
                          Optional validity As AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale = Nothing) As Boolean

        Const routineName = "AgronicaCoreAnagrafeDAL.Reg_ImpiantiXParcoMacchine_W.Write()"
        Dim StrSQL As New Text.StringBuilder

        If validity Is Nothing Then
            validity = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale
        End If

        Try

            StrSQL.AppendLine("IF NOT EXISTS")
            StrSQL.AppendLine("(")
            StrSQL.AppendLine("    SELECT 1")
            StrSQL.AppendLine("    FROM Reg_ImpiantiXParcoMacchine")
            StrSQL.AppendLine($"    WHERE Piva = '{Agro_SQL_SaveText(piva)}'")
            StrSQL.AppendLine($"        AND Sa_Cod = {Agro_SQL_SaveNum(saCod)}")
            StrSQL.AppendLine($"        AND Appezza = {Agro_SQL_SaveNum(appezza)}")
            StrSQL.AppendLine($"        AND ID_Reg = {Agro_SQL_SaveNum(idReg)}")
            StrSQL.AppendLine($"        AND Mac_Cod = {Agro_SQL_SaveNum(macCod)}")
            StrSQL.AppendLine(")")
            StrSQL.AppendLine("    BEGIN")
            StrSQL.AppendLine("    INSERT Reg_ImpiantiXParcoMacchine")
            StrSQL.AppendLine("    (")
            StrSQL.AppendLine("        Piva")
            StrSQL.AppendLine("        , Sa_Cod")
            StrSQL.AppendLine("        , Appezza")
            StrSQL.AppendLine("        , ID_Reg")
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
            StrSQL.AppendLine($"        , {Agro_SQL_SaveNum(idReg)}")
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
            StrSQL.AppendLine("    UPDATE Reg_ImpiantiXParcoMacchine")
            StrSQL.AppendLine("    SET")
            StrSQL.AppendLine($"        Data_Modifica =  {Agro_SQL_SaveDate(DateTime.Today)}")
            StrSQL.AppendLine($"        , Username_Modifica =  '{objServer.UsernameOperazione}'")
            StrSQL.AppendLine($"        , Validita_Inizio =  {Agro_SQL_SaveDate(validity.inizio)}")
            StrSQL.AppendLine($"        , Validita_Fine =  {Agro_SQL_SaveDate(validity.fine)}")
            StrSQL.AppendLine("")
            StrSQL.AppendLine($"    WHERE Piva = '{Agro_SQL_SaveText(piva)}'")
            StrSQL.AppendLine($"        AND Sa_Cod = {Agro_SQL_SaveNum(saCod)}")
            StrSQL.AppendLine($"        AND Appezza = {Agro_SQL_SaveNum(appezza)}")
            StrSQL.AppendLine($"        AND ID_Reg = {Agro_SQL_SaveNum(idReg)}")
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
                         Optional idReg As Int32 = 0,
                         Optional macCod As Int32 = 0)

        Const routineName = "AgronicaCoreAnagrafeDAL.Reg_ImpiantiXParcoMacchine_W.Edit()"
        Dim StrSQL As New Text.StringBuilder

        Try

            StrSQL.AppendLine("UPDATE Reg_ImpiantiXParcoMacchine")
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

            If idReg <> 0 Then
                StrSQL.AppendLine($"    AND ID_Reg = {Agro_SQL_SaveNum(idReg)}")
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
                           Optional idReg As Int32 = 0,
                           Optional macCod As Int32 = 0,
                           Optional classCode As String = "",
                           Optional startValidity As Date = CostantiPersonalizzate.AGRODATAINIZIO,
                           Optional endValidity As Date = CostantiPersonalizzate.AGRODATAFINE) As Boolean

        Const routineName = "AgronicaCoreAnagrafeDAL.Reg_ImpiantiXParcoMacchine_W.Delete()"
        Dim StrSQL As New Text.StringBuilder

        Try

            StrSQL.AppendLine("DELETE FROM Reg_ImpiantiXParcoMacchine")
            StrSQL.AppendLine("WHERE 1 = 1")

            If Not String.IsNullOrEmpty(piva) Then
                StrSQL.AppendLine($"    AND Reg_ImpiantiXParcoMacchine.Piva = '{Agro_SQL_SaveText(piva)}'")
            End If

            If saCod <> 0 Then
                StrSQL.AppendLine($"    AND Reg_ImpiantiXParcoMacchine.Sa_Cod = {Agro_SQL_SaveNum(saCod)}")
            End If

            If appezza <> 0 Then
                StrSQL.AppendLine($"    AND Reg_ImpiantiXParcoMacchine.Appezza = {Agro_SQL_SaveNum(appezza)}")
            End If

            If idReg <> 0 Then
                StrSQL.AppendLine($"    AND Reg_ImpiantiXParcoMacchine.ID_Reg = {Agro_SQL_SaveNum(idReg)}")
            End If

            If macCod <> 0 Then
                StrSQL.AppendLine($"    AND Reg_ImpiantiXParcoMacchine.Mac_Cod = {Agro_SQL_SaveNum(macCod)}")
            End If

            If Not String.IsNullOrEmpty(classCode) Then
                StrSQL.AppendLine($"    AND EXISTS (")
                StrSQL.AppendLine($"        SELECT 1 FROM Parco_Macchine")
                StrSQL.AppendLine($"        WHERE Reg_ImpiantiXParcoMacchine.Mac_Cod = Parco_Macchine.Mac_Cod")
                StrSQL.AppendLine($"        AND Parco_Macchine.Class_Code = '{Agro_SQL_SaveText(classCode)}'")
                StrSQL.AppendLine($"    )")
            End If

            If startValidity <> CostantiPersonalizzate.AGRODATAINIZIO Then
                StrSQL.AppendLine($"    AND Reg_ImpiantiXParcoMacchine.Validita_Fine >= {Agro_SQL_SaveDate(startValidity)}")
            End If

            If endValidity <> CostantiPersonalizzate.AGRODATAFINE Then
                StrSQL.AppendLine($"    AND Reg_ImpiantiXParcoMacchine.Validita_Inizio <= {Agro_SQL_SaveDate(endValidity)}")
            End If

            Return EseguiQuery_Scrittura(objServer, StrSQL.ToString, routineName)

        Catch ex As Exception
            Scrivi_LOG(objServer, routineName, ex.Message)
            Throw New Exception("[" & routineName & "] : " & ex.Message)
        End Try
    End Function
End Class

