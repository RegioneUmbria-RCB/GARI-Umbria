Imports System.Data.Entity
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json

Public Class Interscambio_Parco_Macchine_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(Sistema_Cod As Integer,
                          Mac_Cod As Integer,
                          Codice_Esterno As String,
                          Validita_Inizio As Date,
                          Validita_Fine As Date,
                          xFiltroAggiuntivo As String,
                          xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreInterscambioDAL.Interscambio_Parco_Macchine_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM   Interscambio_Parco_Macchine ")

            StrSQL.AppendLine(" WHERE Validita_Inizio >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            StrSQL.AppendLine(" AND   Validita_Fine <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")

            If Sistema_Cod <> 0 Then
                StrSQL.AppendLine(" AND Sistema_Cod = " & Agro_SQL_SaveNum(Sistema_Cod) & "")
            End If

            If Mac_Cod <> 0 Then
                StrSQL.AppendLine(" AND Mac_Cod = " & Agro_SQL_SaveNum(Mac_Cod) & "")
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
    Public Function GetInterscambioParcoMacchineEF(
                                                  ByVal codice_esterno As String,
                                                  ByVal sistemaCod As Integer,
                                                  ByRef GiasContext As Gias_DeveloperServer_Entities,
                                                  Optional ByVal macCod As Integer = 0
                                                  ) As Interscambio_Parco_Macchine

        Dim nomeRoutine As String = "AgronicaCoreInterscambioDAL.Interscambio_Parco_Macchine_R.GetInterscambioParcoMacchineEF()"
        Dim interscambio As Interscambio_Parco_Macchine
        Try
            If macCod = 0 Then
                'verifico che sia semplicemente presente un record nella tabella interscambio con il codice esterno passato
                interscambio = (From i In GiasContext.Interscambio_Parco_Macchine
                                Where i.Codice_Esterno = codice_esterno AndAlso
                                    i.Sistema_Cod = sistemaCod
                                Select i).FirstOrDefault()
            ElseIf codice_esterno = "" Then
                interscambio = (From i In GiasContext.Interscambio_Parco_Macchine
                                Where i.Mac_Cod = macCod AndAlso
                                    i.Sistema_Cod = sistemaCod
                                Select i).FirstOrDefault()
            Else
                interscambio = (From i In GiasContext.Interscambio_Parco_Macchine
                                Where i.Codice_Esterno = codice_esterno AndAlso
                                    i.Mac_Cod = macCod AndAlso
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

Public Class Interscambio_Parco_Macchine_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(Sistema_Cod As Integer,
                           Mac_Cod As Integer,
                           Codice_Esterno As String,
                           Validita_Inizio As Date,
                           Validita_Fine As Date,
                            ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreInterscambioDAL.Interscambio_Parco_Macchine_W.Scrivi()"

        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" INSERT INTO Interscambio_Parco_Macchine ( ")
            StrSQL.AppendLine("           Sistema_Cod")

            StrSQL.AppendLine("         , Mac_Cod")

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

            StrSQL.AppendLine("          , " & Agro_SQL_SaveNum(Mac_Cod) & " ")

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
                                          ByVal macchina As Parco_Macchine,
                                          ByVal codiceEsterno As String,
                                          ByVal sistemaCod As Integer,
                                          ByVal username As String,
                                          ByVal objParametri As AgronicaCoreParametri
                                          ) As Interscambio_Parco_Macchine

        Dim interscambio As New Interscambio_Parco_Macchine

        interscambio.Sistema_Cod = sistemaCod
        interscambio.Mac_Cod = macchina.Mac_Cod
        interscambio.Codice_Esterno = codiceEsterno
        interscambio.Data_Creazione = DateTime.Now
        interscambio.Data_Modifica = DateTime.Now
        interscambio.Username_Creazione = username
        interscambio.Username_Modifica = username

        Return interscambio

    End Function

    Public Function PrepareEditInterscambioEF(
                                             ByVal macchina As Parco_Macchine,
                                             ByVal codice_esterno As String,
                                             ByVal sistemaCod As Integer,
                                             ByRef GiasContext As Gias_DeveloperServer_Entities,
                                             ByVal objParametri_Server As AgronicaCoreParametri,
                                             ByVal objParametri_Utenti As AgronicaCoreParametri,
                                             Optional ByVal ScriviLog As Boolean = True
                                             ) As Interscambio_Parco_Macchine

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.EFMacchine.PrepareEditInterscambioD2G()"
        Dim interscambioPM_R As New Interscambio_Parco_Macchine_R
        Dim interscambio = interscambioPM_R.GetInterscambioParcoMacchineEF(codice_esterno, sistemaCod, GiasContext, macchina.Codice)

        Try
            interscambio.Data_Modifica = Now
            interscambio.Username_Modifica = objParametri_Server.UsernameOperazione

            GiasContext.Interscambio_Parco_Macchine.Attach(interscambio)
            GiasContext.Entry(interscambio).State = EntityState.Modified

            If ScriviLog Then
                Dim datiInterscambioStr = ""
                If macchina IsNot Nothing Then
                    Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
                    datiInterscambioStr = JsonConvert.SerializeObject(macchina, a)
                End If

                'Scrittura tabella Agronica_Log_Anagrafe
                Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
                Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(
                    "Interscambio_Parco_Macchine",
                    CStr(interscambio.Sistema_Cod),
                    CStr(interscambio.Codice_Esterno),
                    CStr(interscambio.Mac_Cod),
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
                                               ByVal macchina As Parco_Macchine,
                                               ByVal codice_esterno As String,
                                               ByVal sistemaCod As Integer,
                                               ByRef GiasContext As Gias_DeveloperServer_Entities,
                                               ByVal objParametri_Server As AgronicaCoreParametri,
                                               ByVal objParametri_Utenti As AgronicaCoreParametri,
                                               Optional ByVal ScriviLog As Boolean = True
                                               ) As Interscambio_Parco_Macchine

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.EFMacchine.PrepareCreateInterscambioD2G()"
        Dim interscambio = PrepareInterscambioEF(macchina, codice_esterno, sistemaCod, objParametri_Server.UsernameOperazione, objParametri_Server)

        Try
            'lavez 18/02/2024 - sposto valorizzazione date inizio\fine validità 
            interscambio.Validita_Inizio = AGRODATAINIZIO
            interscambio.Validita_Fine = AGRODATAFINE

            GiasContext.Interscambio_Parco_Macchine.Add(interscambio)

            If ScriviLog Then
                Dim datiInterscambioStr = ""
                If interscambio IsNot Nothing Then
                    Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
                    datiInterscambioStr = JsonConvert.SerializeObject(interscambio, a)
                End If

                'Scrittura tabella Agronica_Log_Anagrafe
                Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
                Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(
                    "Interscambio_Parco_Macchine",
                    CStr(interscambio.Sistema_Cod),
                    CStr(interscambio.Codice_Esterno),
                    CStr(interscambio.Mac_Cod), Nothing,
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
                                               ByVal macchina As Parco_Macchine,
                                               ByVal codice_esterno As String,
                                               ByVal sistemaCod As Integer,
                                               ByRef GiasContext As Gias_DeveloperServer_Entities,
                                               ByVal objParametri_Server As AgronicaCoreParametri,
                                               ByVal objParametri_Utenti As AgronicaCoreParametri,
                                               Optional ByVal ScriviLog As Boolean = True
                                               ) As Interscambio_Parco_Macchine

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.EFMacchine.PrepareDeleteInterscambioD2G()"
        Dim interscambioPM_R As New Interscambio_Parco_Macchine_R
        Dim interscambio = interscambioPM_R.GetInterscambioParcoMacchineEF(codice_esterno, sistemaCod, GiasContext, macchina.Codice)

        Try
            interscambio.Data_Modifica = DateTime.Now
            interscambio.Username_Modifica = objParametri_Server.UsernameOperazione

            GiasContext.Interscambio_Parco_Macchine.Attach(interscambio)
            GiasContext.Interscambio_Parco_Macchine.Remove(interscambio)

            If ScriviLog Then
                Dim datiInterscambioStr = ""
                If macchina IsNot Nothing Then
                    Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
                    datiInterscambioStr = JsonConvert.SerializeObject(macchina, a)
                End If

                'Scrittura tabella Agronica_Log_Anagrafe
                Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
                Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(
                    "Interscambio_Parco_Macchine",
                    CStr(interscambio.Sistema_Cod),
                    CStr(interscambio.Codice_Esterno),
                    CStr(interscambio.Mac_Cod),
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
