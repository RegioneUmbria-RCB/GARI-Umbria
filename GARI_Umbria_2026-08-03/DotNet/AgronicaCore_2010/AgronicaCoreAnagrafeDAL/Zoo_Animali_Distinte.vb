Imports System.Data.Entity
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModelsSTD
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Runtime.Serialization
Imports System.Security
Imports System.Text

Public Class Zoo_Animali_Distinte
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal codProgetto As Integer,
                          ByRef objParametriServer As AgronicaCoreParametri
                          ) As Object

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Zoo_Animali_Distinte.Leggi()"
        Dim messaggioErrore As String = ""
        Dim obj As Object

        Try

            Dim gEfUtils As New Gias_EF_Utility
            Dim efConnString As String = gEfUtils.GetEntityConnectionString(objParametriServer.StringaConnessione)

            Using dal As New Gias_DeveloperServer_Entities(efConnString)

                Dim query = (From d In dal.Zoo_Animali_Distinte
                             Join a In dal.Zoo_Animali
                                On a.PIVA Equals d.PIVA And
                                   a.sa_cod Equals d.sa_cod And
                                   a.Cod_Progetto Equals d.Cod_Animale
                             Where d.Cod_Progetto = codProgetto
                             Select a.Progetto, a.Matricola, d.Cod_Animale, d.Cod_Progetto, d.Codice_Distinta, a.Sesso)

                obj = query.FirstOrDefault()

            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return obj

    End Function

    Public Sub Scrivi(ByRef zooAnimaleDistinta As AgronicaCoreEntityFramework_POCO.Zoo_Animali_Distinte,
                      ByRef giasContext As Gias_DeveloperServer_Entities,
                      ByRef objParametriServer As AgronicaCoreParametri,
                      Optional ByVal servizio As enum_Id_Servizio = enum_Id_Servizio.GiasOnline)

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Zoo_Animali_Distinte.Scrivi()"
        Dim messaggioErrore As String = ""

        Try

            Valida(zooAnimaleDistinta)

            zooAnimaleDistinta.inviato = 0
            zooAnimaleDistinta.datainvio = DateTime.Now
            zooAnimaleDistinta.Data_Creazione = DateTime.Now
            zooAnimaleDistinta.Data_Modifica = DateTime.Now
            zooAnimaleDistinta.Username_Creazione = objParametriServer.UsernameOperazione
            zooAnimaleDistinta.Username_Modifica = objParametriServer.UsernameOperazione

            If zooAnimaleDistinta.Validita_Inizio Is Nothing Then
                zooAnimaleDistinta.Validita_Inizio = AGRODATAINIZIO
            End If
            If zooAnimaleDistinta.Validita_Fine Is Nothing Then
                zooAnimaleDistinta.Validita_Fine = AGRODATAFINE
            End If
            If zooAnimaleDistinta.inviato Is Nothing Then
                zooAnimaleDistinta.inviato = 0
            End If


            giasContext.Zoo_Animali_Distinte.Add(zooAnimaleDistinta)
            giasContext.SaveChanges()

            'Scrittura tabella Agronica_Log_Anagrafe
            Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
            Dim log = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.ZooAnimaliDistinte,
                                                        zooAnimaleDistinta.PIVA, CStr(zooAnimaleDistinta.Cod_Progetto),
                                                        CStr(zooAnimaleDistinta.Cod_Animale), Nothing,
                                                        Nothing, Nothing,
                                                        enum_TipoOperazioneDB.Scrittura,
                                                        objParametriServer, servizio)

            giasContext.Agronica_Log_Anagrafe.Add(log)
            giasContext.SaveChanges()

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    Public Sub Modifica(ByRef zooAnimaleDistinta As AgronicaCoreEntityFramework_POCO.Zoo_Animali_Distinte,
                        ByRef giasContext As Gias_DeveloperServer_Entities,
                        ByRef objParametriServer As AgronicaCoreParametri,
                        Optional ByVal servizio As enum_Id_Servizio = enum_Id_Servizio.GiasOnline,
                        Optional ByVal saveChanges As Boolean = True)

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Zoo_Animali_Distinte.Modifica()"
        Dim messaggioErrore As String = ""

        Try

            Valida(zooAnimaleDistinta)

            zooAnimaleDistinta.Data_Modifica = DateTime.Now
            zooAnimaleDistinta.Username_Modifica = objParametriServer.UsernameOperazione

            If zooAnimaleDistinta.Validita_Inizio Is Nothing Then
                zooAnimaleDistinta.Validita_Inizio = AGRODATAINIZIO
            End If

            If zooAnimaleDistinta.Validita_Fine Is Nothing Then
                zooAnimaleDistinta.Validita_Fine = AGRODATAFINE
            End If

            giasContext.Entry(zooAnimaleDistinta).State = EntityState.Modified
            If saveChanges Then
                giasContext.SaveChanges()
            End If

            'Scrittura tabella Agronica_Log_Anagrafe
            Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
            Dim log = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.ZooAnimaliDistinte,
                                                        zooAnimaleDistinta.PIVA, CStr(zooAnimaleDistinta.Cod_Progetto),
                                                        CStr(zooAnimaleDistinta.Cod_Animale), Nothing,
                                                        Nothing, Nothing,
                                                        enum_TipoOperazioneDB.Modifica,
                                                        objParametriServer, servizio)

            giasContext.Agronica_Log_Anagrafe.Add(log)
            If saveChanges Then
                giasContext.SaveChanges()
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub




    Public Sub ModificaGriglia(ByVal Piva As String,
                             ByVal Cod_Animale As Integer,
                             ByVal Cod_Progetto As Integer,
                             ByVal Lotto As String,
                             ByRef objParametri As AgronicaCoreParametri)

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL_Zoo_Aninali_Distinte.ModificaGriglia()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Try

            'Cancellazione Dettagli
            StrSQL.Length = 0
            StrSQL.AppendLine(" Update Zoo_Animali_Distinte ")
            StrSQL.AppendLine(" Set Progetto_Des =  '" & Agro_SQL_SaveText(Lotto) & "'  ")
            StrSQL.AppendLine("   , Progetto_Nome =  '" & Agro_SQL_SaveText(Lotto) & "'  ")
            StrSQL.AppendLine("   , Codice_Distinta =  '" & Agro_SQL_SaveText(Lotto) & "'  ")
            StrSQL.AppendLine(" Where Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine(" And Cod_Animale  = " & Agro_SQL_SaveNum(Cod_Animale) & " ")
            StrSQL.AppendLine(" And Cod_Progetto  = " & Agro_SQL_SaveNum(Cod_Progetto) & " ")

            '--------------------------------------------------------------------------
            EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


    End Sub





    Private Sub Valida(ByRef zooAnimaleDistinta As AgronicaCoreEntityFramework_POCO.Zoo_Animali_Distinte)

        If zooAnimaleDistinta.PIVA = "" Then
            Throw New Exception("Impostare PIVA per Zoo_Animali_Distinte")
        End If

        If zooAnimaleDistinta.Cod_Animale = 0 Then
            Throw New Exception("Impostare Cod_Animale per Zoo_Animali_Distinte")
        End If

        If zooAnimaleDistinta.Cod_Progetto = 0 Then
            Throw New Exception("Impostare Cod_Progetto per Zoo_Animali_Distinte")
        End If

    End Sub

    Public Sub Elimina(ByRef zooAnimaleDistinta As AgronicaCoreEntityFramework_POCO.Zoo_Animali_Distinte,
                       ByRef giasContext As Gias_DeveloperServer_Entities,
                       ByRef objParametriServer As AgronicaCoreParametri,
                       Optional ByVal servizio As enum_Id_Servizio = enum_Id_Servizio.GiasOnline)

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Zoo_Animali_Distinte.Elimina()"
        Dim messaggioErrore As String = ""

        Try

            GiasContext.Entry(zooAnimaleDistinta).State = EntityState.Deleted
            giasContext.Zoo_Animali_Distinte.Remove(zooAnimaleDistinta)
            giasContext.SaveChanges()

            'Scrittura tabella Agronica_Log_Anagrafe
            Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
            Dim log = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.ZooAnimaliDistinte,
                                                        zooAnimaleDistinta.PIVA, CStr(zooAnimaleDistinta.Cod_Progetto),
                                                        CStr(zooAnimaleDistinta.Cod_Animale), Nothing,
                                                        Nothing, Nothing,
                                                        enum_TipoOperazioneDB.Cancellazione,
                                                        objParametriServer, servizio)

            giasContext.Agronica_Log_Anagrafe.Add(log)
            giasContext.SaveChanges()

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    Public Function LeggiDaCod_Animale(ByVal codAnimale As Integer,
                                       ByRef objParametriServer As AgronicaCoreParametri
                                       ) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Zoo_Animali_Distinte.LeggiDaCod_Animale()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As DataTable
        Dim strAssegnamento As String = String.Empty

        ' a seconda del tipo del valore che devo aggiornare, formatto la query
        Dim Stringa As Type = GetType(System.String)
        Dim Data As Type = GetType(System.DateTime)
        Dim Intero32 As Type = GetType(System.Int32)

        strAssegnamento = " Cod_Animale = '" & Agro_SQL_SaveText(codAnimale.ToString) & "' "
        Dim Risposta As String
        Try
            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.AppendLine("Select Progetto_Des From Zoo_Animali_Distinte Where ")
            StrSQL.AppendLine(strAssegnamento)
            StrSQL.AppendLine("AND validita_fine > GETDATE() ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Lettura(objParametriServer, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
            Dim Row As DataRow = xRisp.Select().FirstOrDefault
            Risposta = Row.Item("Progetto_Des")

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            xRisp = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return Risposta

    End Function

End Class
