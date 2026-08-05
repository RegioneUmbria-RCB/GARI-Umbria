Imports AgronicaCoreDTOStd.Identity
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.DataProviderExtensions

Public Class ProgettiXContributi_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Read(objServer As AgronicaCoreParametri,
                         Optional project As Int32 = 0,
                         Optional contribute As Int32 = 0,
                         Optional type As Int32 = 0,
                         Optional piva As String = "",
                         Optional startValidity As Date = CostantiPersonalizzate.AGRODATAINIZIO,
                         Optional endValidity As Date = CostantiPersonalizzate.AGRODATAFINE) As DataTable

        Const routineName = "AgronicaCoreAnagrafeDAL.ProgettiXContributi.Read()"
        Dim StrSQL As New Text.StringBuilder

        Try

            StrSQL.AppendLine("SELECT pxc.*")
            StrSQL.AppendLine("    , c.ContributoDes")
            StrSQL.AppendLine("    , c.Validita_Inizio AS ContributoValidita_Inizio")
            StrSQL.AppendLine("    , c.Validita_Fine AS ContributoValidita_Fine")
            StrSQL.AppendLine("FROM Imprese_ProgettiXContributi pxc")
            StrSQL.AppendLine("")

            StrSQL.AppendLine("JOIN Contributi c ON c.ContributoCod = pxc.ContributoCod")
            StrSQL.AppendLine("    AND c.Tipo = pxc.ContributoTipo")
            StrSQL.AppendLine("")

            StrSQL.AppendLine("WHERE 1 = 1")

            If Not String.IsNullOrEmpty(piva) Then
                StrSQL.AppendLine($"    AND pxc.Piva = '{Agro_SQL_SaveText(piva)}'")
            End If

            If project <> 0 Then
                StrSQL.AppendLine($"    AND pxc.ProgettoCod = {Agro_SQL_SaveNum(project)}")
            End If

            If contribute <> 0 Then
                StrSQL.AppendLine($"    AND pxc.ContributoCod = {Agro_SQL_SaveNum(contribute)}")
            End If

            If type <> 0 Then
                StrSQL.AppendLine($"    AND pxc.ContributoTipo = {Agro_SQL_SaveNum(type)}")
            End If

            If startValidity <> CostantiPersonalizzate.AGRODATAINIZIO Then
                StrSQL.AppendLine($"    AND pxc.Validita_Inizio <= {Agro_SQL_SaveDate(endValidity)}")
            End If

            If endValidity <> CostantiPersonalizzate.AGRODATAFINE Then
                StrSQL.AppendLine($"    AND pxc.Validita_Fine >= {Agro_SQL_SaveDate(startValidity)}")
            End If

            Return EseguiQuery_Lettura(objServer, StrSQL.ToString, routineName)

        Catch ex As Exception
            Scrivi_LOG(objServer, routineName, ex.Message)
            Throw New Exception("[" & routineName & "] : " & ex.Message)
        End Try

    End Function


    Public Function Leggi_Disticit_IdAgenda(objServer As AgronicaCoreParametri,
                                            Tipo As Integer,
                                            ContributoCod As Integer,
                                            Piva As String,
                                            ValiditaInizio As Date,
                                            ValiditaFine As Date) As DataTable
        Const routineName = "AgronicaCoreAnagrafeDAL.ProgettiXContributi.Leggi_Disticit_IdAgenda()"
        Dim StrSQL As New Text.StringBuilder

        Try

            StrSQL.AppendLine("select DISTINCT Mov_Destinazioni.Id_Agenda from Imprese_ProgettiXContributi ipc").
                AppendLine("INNER JOIN Imprese_Progetti ip ON ip.Progetto_Cod = ipc.ProgettoCod and ip.Piva = ipc.Piva").
                AppendLine("INNER JOIN Mov_Destinazioni ON Mov_Destinazioni.Piva = ip.piva AND Mov_Destinazioni.Sa_Cod = ip.Sa_Cod AND  Mov_Destinazioni.Appezza = ip.Appezza
                        	AND  Mov_Destinazioni.Id_Destinazione = ip.Id_Reg AND Mov_Destinazioni.Tipo_Destinazione = 0").
                AppendLine("INNER JOIN Agenda On Agenda.Id_Agenda = Mov_Destinazioni.Id_Agenda").
                AppendLine("WHERE 1 = 1")

            If Not String.IsNullOrEmpty(Piva) Then
                StrSQL.AppendLine($"    AND ipc.piva = '{Agro_SQL_SaveText(Piva)}'")
            End If

            If Tipo <> 0 Then
                StrSQL.AppendLine($"    AND ipc.ContributoTipo = {Agro_SQL_SaveNum(Tipo)}")
            End If

            If ContributoCod <> 0 Then
                StrSQL.AppendLine($"    AND ipc.ContributoCod = {Agro_SQL_SaveNum(ContributoCod)}")
            End If

            If ValiditaInizio <> CostantiPersonalizzate.AGRODATAINIZIO Then
                StrSQL.AppendLine($"    AND Agenda.Validita_Inizio <= {Agro_SQL_SaveDate(ValiditaInizio)}")
            End If

            If ValiditaFine <> CostantiPersonalizzate.AGRODATAFINE Then
                StrSQL.AppendLine($"    AND Agenda.Validita_Fine >= {Agro_SQL_SaveDate(ValiditaFine)}")
            End If

            Return EseguiQuery_Lettura(objServer, StrSQL.ToString, routineName)

        Catch ex As Exception
            Scrivi_LOG(objServer, routineName, ex.Message)
            Throw New Exception("[" & routineName & "] : " & ex.Message)
        End Try
    End Function
End Class

Public Class ProgettiXContributi_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Write(project As Int32,
                          contribute As Int32,
                          type As Int32,
                          piva As String,
                          objServer As AgronicaCoreParametri,
                          Optional startValidity As Date = CostantiPersonalizzate.AGRODATAINIZIO,
                          Optional endValidity As Date = CostantiPersonalizzate.AGRODATAFINE) As Boolean

        Const routineName = "AgronicaCoreAnagrafeDAL.ProgettiXContributi.Write()"
        Dim StrSQL As New Text.StringBuilder

        Try

            StrSQL.AppendLine("IF NOT EXISTS")
            StrSQL.AppendLine("(")
            StrSQL.AppendLine("    SELECT 1")
            StrSQL.AppendLine("    FROM Imprese_ProgettiXContributi")
            StrSQL.AppendLine($"    WHERE ProgettoCod = {Agro_SQL_SaveNum(project)}")
            StrSQL.AppendLine($"        AND ContributoCod = {Agro_SQL_SaveNum(contribute)}")
            StrSQL.AppendLine($"        AND ContributoTipo = {Agro_SQL_SaveNum(type)}")
            StrSQL.AppendLine(")")
            StrSQL.AppendLine("    BEGIN")
            StrSQL.AppendLine("    INSERT Imprese_ProgettiXContributi")
            StrSQL.AppendLine("    (")
            StrSQL.AppendLine("        ProgettoCod")
            StrSQL.AppendLine("        , ContributoCod")
            StrSQL.AppendLine("        , ContributoTipo")
            StrSQL.AppendLine("        , Piva")
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
            StrSQL.AppendLine($"         {Agro_SQL_SaveNum(project)}")
            StrSQL.AppendLine($"        , {Agro_SQL_SaveNum(contribute)}")
            StrSQL.AppendLine($"        , {Agro_SQL_SaveNum(type)}")
            StrSQL.AppendLine($"        , '{Agro_SQL_SaveText(piva)}'")
            StrSQL.AppendLine($"        , {0}")
            StrSQL.AppendLine($"        , NULL")
            StrSQL.AppendLine($"        , {Agro_SQL_SaveDate(DateTime.Today)}")
            StrSQL.AppendLine($"        , {Agro_SQL_SaveDate(DateTime.Today)}")
            StrSQL.AppendLine($"        , '{objServer.UsernameOperazione}'")
            StrSQL.AppendLine($"        , '{objServer.UsernameOperazione}'")
            StrSQL.AppendLine($"        , {Agro_SQL_SaveDate(startValidity)}")
            StrSQL.AppendLine($"        , {Agro_SQL_SaveDate(endValidity)}")
            StrSQL.AppendLine("    )")
            StrSQL.AppendLine("    END;")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("ELSE")
            StrSQL.AppendLine("    BEGIN")
            StrSQL.AppendLine("    UPDATE Imprese_ProgettiXContributi")
            StrSQL.AppendLine("    SET")
            StrSQL.AppendLine($"        Data_Modifica =  {Agro_SQL_SaveDate(DateTime.Today)}")
            StrSQL.AppendLine($"        , Piva =  '{Agro_SQL_SaveText(piva)}'")
            StrSQL.AppendLine($"        , Username_Modifica =  '{objServer.UsernameOperazione}'")
            StrSQL.AppendLine($"        , Validita_Inizio =  {Agro_SQL_SaveDate(startValidity)}")
            StrSQL.AppendLine($"        , Validita_Fine =  {Agro_SQL_SaveDate(endValidity)}")
            StrSQL.AppendLine("")
            StrSQL.AppendLine($"    WHERE ProgettoCod = {Agro_SQL_SaveNum(project)}")
            StrSQL.AppendLine($"        AND ContributoCod = {Agro_SQL_SaveNum(contribute)}")
            StrSQL.AppendLine($"        AND ContributoTipo = {Agro_SQL_SaveNum(type)}")
            StrSQL.AppendLine("    END;")

            Return EseguiQuery_Scrittura(objServer, StrSQL.ToString, routineName)

        Catch ex As Exception
            Scrivi_LOG(objServer, routineName, ex.Message)
            Throw New Exception("[" & routineName & "] : " & ex.Message)
        End Try

    End Function

    Public Function Edit(objServer As AgronicaCoreParametri,
                         Optional project As Int32 = 0,
                         Optional contribute As Int32 = 0,
                         Optional type As Int32 = 0,
                         Optional piva As String = "",
                         Optional startValidity As Date = CostantiPersonalizzate.AGRODATAINIZIO,
                         Optional endValidity As Date = CostantiPersonalizzate.AGRODATAFINE) As Boolean

        Const routineName = "AgronicaCoreAnagrafeDAL.ProgettiXContributi.Edit()"
        Dim StrSQL As New Text.StringBuilder

        Try

            StrSQL.AppendLine("UPDATE Imprese_ProgettiXContributi")
            StrSQL.AppendLine("SET")
            StrSQL.AppendLine($"    Data_Modifica = {Agro_SQL_SaveDate(DateTime.Today)}")
            StrSQL.AppendLine($"    , Username_Modifica = '{objServer.UsernameOperazione}'")
            StrSQL.AppendLine($"    , Validita_Inizio = {Agro_SQL_SaveDate(startValidity)}")
            StrSQL.AppendLine($"    , Validita_Fine = {Agro_SQL_SaveDate(endValidity)}")
            StrSQL.AppendLine("")
            StrSQL.AppendLine("WHERE 1 = 1")

            If Not String.IsNullOrEmpty(piva) Then
                StrSQL.AppendLine($"    AND Piva = '{Agro_SQL_SaveText(piva)}'")
            End If

            If project <> 0 Then
                StrSQL.AppendLine($"    AND ProgettoCod = {Agro_SQL_SaveNum(project)}")
            End If

            If contribute <> 0 Then
                StrSQL.AppendLine($"    AND ContributoCod = {Agro_SQL_SaveNum(contribute)}")
            End If

            If type <> 0 Then
                StrSQL.AppendLine($"    AND ContributoTipo = {Agro_SQL_SaveNum(type)}")
            End If

            Return EseguiQuery_Scrittura(objServer, StrSQL.ToString, routineName)

        Catch ex As Exception
            Scrivi_LOG(objServer, routineName, ex.Message)
            Throw New Exception("[" & routineName & "] : " & ex.Message)
        End Try
    End Function

    Public Function Delete(objServer As AgronicaCoreParametri,
                           Optional project As Int32 = 0,
                           Optional contribute As Int32 = 0,
                           Optional type As Int32 = 0,
                           Optional piva As String = "",
                           Optional startValidity As Date = CostantiPersonalizzate.AGRODATAINIZIO,
                           Optional endValidity As Date = CostantiPersonalizzate.AGRODATAFINE) As Boolean

        Const routineName = "AgronicaCoreAnagrafeDAL.ProgettiXContributi.Delete()"
        Dim StrSQL As New Text.StringBuilder

        Try

            StrSQL.AppendLine("DELETE FROM Imprese_ProgettiXContributi")
            StrSQL.AppendLine("WHERE 1 = 1")

            If Not String.IsNullOrEmpty(piva) Then
                StrSQL.AppendLine($"    AND Piva = '{Agro_SQL_SaveText(piva)}'")
            End If

            If project <> 0 Then
                StrSQL.AppendLine($"    AND ProgettoCod = {Agro_SQL_SaveNum(project)}")
            End If

            If contribute <> 0 Then
                StrSQL.AppendLine($"    AND ContributoCod = {Agro_SQL_SaveNum(contribute)}")
            End If

            If type <> 0 Then
                StrSQL.AppendLine($"    AND ContributoTipo = {Agro_SQL_SaveNum(type)}")
            End If

            If startValidity <> CostantiPersonalizzate.AGRODATAINIZIO Then
                StrSQL.AppendLine($"    AND Imprese_ProgettiXContributi.Validita_Fine >= {Agro_SQL_SaveDate(startValidity)}")
            End If

            If endValidity <> CostantiPersonalizzate.AGRODATAFINE Then
                StrSQL.AppendLine($"    AND Imprese_ProgettiXContributi.Validita_Inizio <= {Agro_SQL_SaveDate(endValidity)}")
            End If

            Return EseguiQuery_Scrittura(objServer, StrSQL.ToString, routineName)

        Catch ex As Exception
            Scrivi_LOG(objServer, routineName, ex.Message)
            Throw New Exception("[" & routineName & "] : " & ex.Message)
        End Try
    End Function
End Class
