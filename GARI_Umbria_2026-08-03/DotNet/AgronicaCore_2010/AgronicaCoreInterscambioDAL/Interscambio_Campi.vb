Imports System.Data.Entity
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModelsSTD.anagrafiche
Imports Newtonsoft.Json

Public Class Interscambio_Campi_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(Sistema_Cod As Integer,
                          Piva As String,
                          Sa_Cod As Integer,
                          Campo_Cod As Integer,
                          Codice_Esterno As String,
                          Validita_Inizio As Date,
                          Validita_Fine As Date,
                          xFiltroAggiuntivo As String,
                          xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreInterscambioDAL.Interscambio_Campi_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM   Interscambio_Campi ")

            StrSQL.AppendLine(" WHERE Validita_Inizio >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            StrSQL.AppendLine(" AND   Validita_Fine <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")

            If Sistema_Cod <> 0 Then
                StrSQL.AppendLine(" AND Sistema_Cod = " & Agro_SQL_SaveNum(Sistema_Cod) & "")
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "")
            End If

            If Campo_Cod <> 0 Then
                StrSQL.AppendLine(" AND Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & "")
            End If

            If Codice_Esterno <> "" Then
                StrSQL.AppendLine(" AND Codice_Esterno = '" & Agro_SQL_SaveText(Codice_Esterno) & "'")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Data_Creazione DESC")
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

#Region "Metodi EntityFramework"
    Public Function GetInterscambioGruppiAppezzamentiEF(
                                                  ByVal codice_esterno As String,
                                                  ByVal sistemaCod As Integer,
                                                  ByRef GiasContext As Gias_DeveloperServer_Entities,
                                                  Optional ByVal piva As String = "",
                                                  Optional ByVal saCod As Integer = 0,
                                                  Optional ByVal campoCod As Integer = 0
                                                  ) As Interscambio_Campi

        Dim nomeRoutine As String = "AgronicaCoreInterscambioDAL.Interscambio_Campi_R.GetInterscambioGruppiAppezzamentiEF()"
        Dim interscambio As Interscambio_Campi
        Try
            If piva = "" AndAlso saCod = 0 AndAlso campoCod = 0 Then
                'verifico che sia semplicemente presente un record nella tabella interscambio con il codice esterno passato
                interscambio = (From i In GiasContext.Interscambio_Campi
                                Where i.Codice_Esterno = codice_esterno AndAlso
                                    i.Sistema_Cod = sistemaCod
                                Select i).FirstOrDefault()
            ElseIf codice_esterno = "" Then
                interscambio = (From i In GiasContext.Interscambio_Campi
                                Where i.Piva = piva AndAlso
                                    i.Sa_Cod = saCod AndAlso
                                    i.Campo_Cod = campoCod AndAlso
                                    i.Sistema_Cod = sistemaCod
                                Select i).FirstOrDefault()
            Else
                interscambio = (From i In GiasContext.Interscambio_Campi
                                Where i.Codice_Esterno = codice_esterno AndAlso
                                    i.Piva = piva AndAlso
                                    i.Sa_Cod = saCod AndAlso
                                    i.Campo_Cod = campoCod AndAlso
                                    i.Sistema_Cod = sistemaCod
                                Select i).FirstOrDefault()
            End If

        Catch ex As Exception
            Dim MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return interscambio
    End Function
#End Region

End Class

Public Class Interscambio_Campi_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(Sistema_Cod As Integer,
                           Piva As String,
                           Sa_Cod As Integer,
                           Campo_Cod As Integer,
                           Codice_Esterno As String,
                           Validita_Inizio As Date,
                           Validita_Fine As Date,
                            ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreInterscambioDAL.Interscambio_Campi_W.Scrivi()"

        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" INSERT INTO Interscambio_Campi ( ")
            StrSQL.AppendLine("           Sistema_Cod")

            StrSQL.AppendLine("         , Piva")

            StrSQL.AppendLine("         , Sa_Cod")

            StrSQL.AppendLine("         , Campo_Cod")

            StrSQL.AppendLine("         , Codice_Esterno")

            StrSQL.AppendLine("         , Data_Creazione")
            StrSQL.AppendLine("         , Data_Modifica")
            StrSQL.AppendLine("         , Username_Creazione")
            StrSQL.AppendLine("         , Username_Modifica")
            StrSQL.AppendLine("         , Validita_Inizio")
            StrSQL.AppendLine("         , Validita_Fine")
            StrSQL.AppendLine(" ) ")

            StrSQL.AppendLine(" VALUES (")
            StrSQL.AppendLine("            " & Agro_SQL_SaveNum(Sistema_Cod) & " ")

            StrSQL.AppendLine("          , '" & Agro_SQL_SaveText(Piva) & "' ")

            StrSQL.AppendLine("          , " & Agro_SQL_SaveNum(Sa_Cod) & " ")

            StrSQL.AppendLine("          , " & Agro_SQL_SaveNum(Campo_Cod) & " ")

            StrSQL.AppendLine("          , '" & Agro_SQL_SaveText(Codice_Esterno) & "' ")

            StrSQL.AppendLine("          , " & Agro_SQL_SaveDateTime(Now) & " ")
            StrSQL.AppendLine("          , " & Agro_SQL_SaveDateTime(Now) & " ")
            StrSQL.AppendLine("          , '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("          , '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("          , " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            StrSQL.AppendLine("          , " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.AppendLine(" )")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return xRisp

    End Function

#Region "Metodi EntityFramework"

    Private Function PrepareInterscambioEF(
                                          ByVal campo As Campo,
                                          ByVal codiceEsterno As String,
                                          ByVal sistemaCod As Integer,
                                          ByVal username As String,
                                          ByVal objParametri As AgronicaCoreParametri
                                          ) As Interscambio_Campi

        Dim interscambio As New Interscambio_Campi

        interscambio.Sistema_Cod = sistemaCod
        interscambio.Piva = campo.primaryKey.centroAziendalePK.partitaIva
        interscambio.Sa_Cod = campo.primaryKey.centroAziendalePK.codice
        interscambio.Campo_Cod = campo.primaryKey.codice
        interscambio.Codice_Gias = campo.primaryKey.centroAziendalePK.partitaIva & "_" & campo.primaryKey.centroAziendalePK.codice & "_" & campo.primaryKey.codice
        interscambio.Codice_Esterno = codiceEsterno
        interscambio.Data_Creazione = DateTime.Now
        interscambio.Data_Modifica = DateTime.Now
        interscambio.Username_Creazione = username
        interscambio.Username_Modifica = username

        Return interscambio

    End Function

    Public Function PrepareEditInterscambioEF(
                                             ByVal campo As Campo,
                                             ByVal codice_esterno As String,
                                             ByVal sistemaCod As Integer,
                                             ByRef GiasContext As Gias_DeveloperServer_Entities,
                                             ByVal objParametri_Server As AgronicaCoreParametri,
                                             ByVal objParametri_Utenti As AgronicaCoreParametri,
                                             Optional ByVal ScriviLog As Boolean = True
                                             ) As Interscambio_Campi

        Dim nomeRoutine As String = "AgronicaCoreInterscambioDAL.Interscambio_Campi_W.PrepareEditInterscambioEF()"
        Dim interscambioGA_R As New Interscambio_Campi_R
        Dim interscambio = interscambioGA_R.GetInterscambioGruppiAppezzamentiEF(
            codice_esterno,
            sistemaCod,
            GiasContext,
            campo.primaryKey.centroAziendalePK.partitaIva,
            campo.primaryKey.centroAziendalePK.codice,
            campo.primaryKey.codice)

        Try
            interscambio.Data_Modifica = Now
            interscambio.Username_Modifica = objParametri_Server.UsernameOperazione

            GiasContext.Interscambio_Campi.Attach(interscambio)
            GiasContext.Entry(interscambio).State = EntityState.Modified

            If ScriviLog Then
                Dim datiInterscambioStr = ""
                If campo IsNot Nothing Then
                    Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
                    datiInterscambioStr = JsonConvert.SerializeObject(campo, a)
                End If

                'Scrittura tabella Agronica_Log_Anagrafe
                Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
                Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(
                    "Interscambio_Campi",
                    CStr(interscambio.Sistema_Cod),
                    CStr(interscambio.Codice_Esterno),
                    CStr(interscambio.Campo_Cod),
                    Nothing,
                    Nothing, Nothing,
                    enum_TipoOperazioneDB.Modifica,
                    objParametri_Server,
                    enum_Id_Servizio.Nessuno,
                    "",
                    datiInterscambioStr
                    )

                GiasContext.Agronica_Log_Anagrafe.Add(log)
            End If

        Catch ex As Exception
            Dim MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return interscambio

    End Function

    Public Function PrepareCreateInterscambioEF(
                                               ByVal campo As Campo,
                                               ByVal codice_esterno As String,
                                               ByVal sistemaCod As Integer,
                                               ByRef GiasContext As Gias_DeveloperServer_Entities,
                                               ByVal objParametri_Server As AgronicaCoreParametri,
                                               ByVal objParametri_Utenti As AgronicaCoreParametri,
                                               Optional ByVal ScriviLog As Boolean = True
                                               ) As Interscambio_Campi

        Dim nomeRoutine As String = "AgronicaCoreInterscambioDAL.Interscambio_Campi_W.PrepareCreateInterscambioEF()"
        Dim interscambio = PrepareInterscambioEF(campo, codice_esterno, sistemaCod, objParametri_Server.UsernameOperazione, objParametri_Server)

        Try
            interscambio.Validita_Inizio = AGRODATAINIZIO
            interscambio.Validita_Fine = AGRODATAFINE

            GiasContext.Interscambio_Campi.Add(interscambio)

            If ScriviLog Then
                Dim datiInterscambioStr = ""
                If interscambio IsNot Nothing Then
                    Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
                    datiInterscambioStr = JsonConvert.SerializeObject(interscambio, a)
                End If

                'Scrittura tabella Agronica_Log_Anagrafe
                Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
                Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(
                    "Interscambio_Campi",
                    CStr(interscambio.Sistema_Cod),
                    CStr(interscambio.Codice_Esterno),
                    CStr(interscambio.Campo_Cod), Nothing,
                    Nothing, Nothing,
                    enum_TipoOperazioneDB.Scrittura,
                    objParametri_Server,
                    enum_Id_Servizio.Nessuno,
                    "",
                    datiInterscambioStr
                    )

                GiasContext.Agronica_Log_Anagrafe.Add(log)
            End If

        Catch ex As Exception
            Dim MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return interscambio

    End Function

    Public Function PrepareDeleteInterscambioEF(
                                               ByVal campo As Campo,
                                               ByVal codice_esterno As String,
                                               ByVal sistemaCod As Integer,
                                               ByRef GiasContext As Gias_DeveloperServer_Entities,
                                               ByVal objParametri_Server As AgronicaCoreParametri,
                                               ByVal objParametri_Utenti As AgronicaCoreParametri,
                                               Optional ByVal ScriviLog As Boolean = True
                                               ) As Interscambio_Campi

        Dim nomeRoutine As String = "AgronicaCoreInterscambioDAL.Interscambio_Campi_W.PrepareDeleteInterscambioEF()"
        Dim interscambioGA_R As New Interscambio_Campi_R
        Dim interscambio = interscambioGA_R.GetInterscambioGruppiAppezzamentiEF(
            codice_esterno,
            sistemaCod,
            GiasContext,
            campo.primaryKey.centroAziendalePK.partitaIva,
            campo.primaryKey.centroAziendalePK.codice,
            campo.primaryKey.codice)

        Try
            interscambio.Data_Modifica = DateTime.Now
            interscambio.Username_Modifica = objParametri_Server.UsernameOperazione

            GiasContext.Interscambio_Campi.Attach(interscambio)
            GiasContext.Interscambio_Campi.Remove(interscambio)

            If ScriviLog Then
                Dim datiInterscambioStr = ""
                If campo IsNot Nothing Then
                    Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
                    datiInterscambioStr = JsonConvert.SerializeObject(campo, a)
                End If

                'Scrittura tabella Agronica_Log_Anagrafe
                Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
                Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(
                    "Interscambio_Campi",
                    CStr(interscambio.Sistema_Cod),
                    CStr(interscambio.Codice_Esterno),
                    CStr(interscambio.Campo_Cod),
                    Nothing,
                    Nothing, Nothing,
                    enum_TipoOperazioneDB.Modifica,
                    objParametri_Server,
                    enum_Id_Servizio.Nessuno,
                    "",
                    datiInterscambioStr
                    )

                GiasContext.Agronica_Log_Anagrafe.Add(log)
            End If

        Catch ex As Exception
            Dim MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return interscambio

    End Function

#End Region

End Class