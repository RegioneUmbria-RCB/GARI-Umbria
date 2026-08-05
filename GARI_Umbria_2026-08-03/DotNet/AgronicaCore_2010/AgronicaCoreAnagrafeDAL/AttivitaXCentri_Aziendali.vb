Imports System.Data.Entity
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json

Public Class AttivitaXCentri_Aziendali_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Private ReadOnly _giasContext As Gias_DeveloperServer_Entities
    Public Sub New()

    End Sub
    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub

    Public Sub New(ByVal giasContext As Gias_DeveloperServer_Entities)
        _giasContext = giasContext
    End Sub

    Public Function Leggi(ByVal piva As String,
                          ByVal Id_Attivita As Integer,
                          ByVal Sa_Cod As Integer,
                          ByRef objParametri_Server As AgronicaCoreParametri) As List(Of AttivitaXCentri_Aziendali)

        Dim nomeProcedura As String = "AgronicaCoreContabDAL.AttivitaXCentri_Aziendali_R.Leggi()"

        Try


            Dim AttivXCentri_Az =
            From AxC In _giasContext.AttivitaXCentri_Aziendali

            'Filtro Dinamico
            If piva <> "" Then
                AttivXCentri_Az = AttivXCentri_Az.Where(Function(x) x.Piva = piva)
            End If
            If Sa_Cod <> 0 Then
                AttivXCentri_Az = AttivXCentri_Az.Where(Function(x) x.Sa_Cod.Equals(Sa_Cod))
            End If
            If Id_Attivita <> 0 Then
                AttivXCentri_Az = AttivXCentri_Az.Where(Function(x) x.ID_Attivita.Equals(Id_Attivita))
            End If

            Return AttivXCentri_Az.Distinct().ToList()

        Catch ex As Exception
            Throw New Exception("[" & nomeProcedura & "] : " & ex.Message)
        End Try

    End Function

    Public Function Leggi(ByVal piva As String,
                          ByVal Id_Attivita As Integer,
                          ByVal Sa_Cod As Integer,
                          ByVal flag_inclusa As Integer,
                            ByVal Utilizzo_GiasAPP As Boolean,
                           ByRef objParametri_Server As AgronicaCoreParametri
                            ) As String

        Dim risposta As String = ""

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.AttivitaXCentri_Aziendali_R.Leggi()"

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim AttivXCentri_Az =
            From AxC In GiasContext.AttivitaXCentri_Aziendali
            Join A In GiasContext.Attivita.Where(Function(x) x.Piva = piva Or x.Sa_Cod = -1)
                On A.Piva_SuperUser Equals AxC.Piva_SuperUser And
                    A.ID_Attivita Equals AxC.ID_Attivita
            Join CA In GiasContext.Centri_Aziendali
                On AxC.Piva Equals CA.PIVA And
                    AxC.Sa_Cod Equals CA.sa_cod
            Select New With {
            .Chiave = "",
            .Piva = AxC.Piva,
            .Sa_Cod = AxC.Sa_Cod,
            .Sa_Nome = CA.sa_nome,
            .ID_Attivita = AxC.ID_Attivita,
            .DescrizioneAttivita = A.Desc,
            .Inclusa = AxC.Inclusa,
            .Validita_Inizio = AxC.Validita_Inizio,
            .Validita_Fine = AxC.Validita_Fine,
            AxC.Data_Creazione,
            AxC.Data_Modifica,
            AxC.datainvio,
            AxC.inviato,
            .Utilizzo_GiasAPP = A.Utilizzo_GiasAPP
            }

            'Filtro Dinamico
            If piva <> "" Then
                AttivXCentri_Az = AttivXCentri_Az.Where(Function(x) x.Piva = piva)
            End If
            If Sa_Cod <> 0 Then
                AttivXCentri_Az = AttivXCentri_Az.Where(Function(x) x.Sa_Cod = UtilityProvider.Agro_SQL_SaveNum(Sa_Cod))
            End If
            If Id_Attivita <> 0 Then
                AttivXCentri_Az = AttivXCentri_Az.Where(Function(x) x.ID_Attivita = UtilityProvider.Agro_SQL_SaveNum(Id_Attivita))
            End If
            If flag_inclusa = 0 OrElse flag_inclusa = 1 Then
                AttivXCentri_Az = AttivXCentri_Az.Where(Function(x) x.Inclusa = flag_inclusa)
            End If
            If Utilizzo_GiasAPP Then
                AttivXCentri_Az = AttivXCentri_Az.Where(Function(x) x.Utilizzo_GiasAPP = 1)
            End If

            Dim myList = AttivXCentri_Az.Distinct().ToList()
            For Each obj In myList
                obj.Chiave = CStr(obj.Piva) & "_" & CStr(obj.Sa_Cod) & "_" & CStr(obj.ID_Attivita)
            Next

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(myList, Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

End Class

Public Class AttivitaXCentri_Aziendali_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Private ReadOnly _giasContext As Gias_DeveloperServer_Entities
    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer

        Dim gefutils As New Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(_objParametriServer.StringaConnessione)

        _giasContext = New Gias_DeveloperServer_Entities(efConnString)

    End Sub

    Public Sub New(ByVal giasContext As Gias_DeveloperServer_Entities, ByVal objParametriServer As AgronicaCoreParametri)
        _giasContext = giasContext
        _objParametriServer = objParametriServer
    End Sub

    Public Sub Elimina(ByVal entita As List(Of AttivitaXCentri_Aziendali))

        Const nomeProcedura = "AttivitaXCentri_Aziendali_W.Elimina"
        Try
            For Each e As AttivitaXCentri_Aziendali In entita
                _giasContext.AttivitaXCentri_Aziendali.Remove(Nothing)
            Next
        Catch ex As Exception
            Throw New Exception("[" & nomeProcedura & "] : " & ex.Message)
        End Try

    End Sub

    Public Sub Elimina(ByVal entita As AttivitaXCentri_Aziendali)

        Const nomeProcedura = "AttivitaXCentri_Aziendali_W.Elimina"

        Try
            _giasContext.AttivitaXCentri_Aziendali.Remove(entita)
        Catch ex As Exception
            Throw New Exception("[" & nomeProcedura & "] : " & ex.Message)
        End Try

    End Sub

    Public Sub Modifica(ByVal entita As AttivitaXCentri_Aziendali)

        Const nomeProcedura = "AttivitaXCentri_Aziendali_W.Modifica"

        Try
            entita.Data_Modifica = DateTime.Now
            entita.Username_Modifica = _objParametriServer.UtenteUsername
            entita.Piva_SuperUser = _objParametriServer.PivaSuperUser
            _giasContext.Entry(entita).State = EntityState.Modified

        Catch ex As Exception
            Throw New Exception("[" & nomeProcedura & "] : " & ex.Message)
        End Try

    End Sub

    Public Sub Inserisci(ByVal entita As AttivitaXCentri_Aziendali)

        Const nomeProcedura = "AttivitaXCentri_Aziendali_W.Salva"

        Try

            entita.Data_Modifica = DateTime.Now
            entita.Username_Modifica = _objParametriServer.UtenteUsername
            entita.Username_Creazione = _objParametriServer.UtenteUsername
            entita.Piva_SuperUser = _objParametriServer.PivaSuperUser
            _giasContext.AttivitaXCentri_Aziendali.Add(entita)

        Catch ex As Exception
            Throw New Exception("[" & nomeProcedura & "] : " & ex.Message)
        End Try

    End Sub

    Public Sub Committa()
        _giasContext.SaveChanges()
    End Sub

End Class
