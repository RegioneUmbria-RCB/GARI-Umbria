Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class jDeereDataModelBIZ_Files_W
    Inherits AgronicaCoreDataProvider.LogProvider

#Region "Costruttori"

    Public Sub New()
        Provider = Globalization.CultureInfo.InvariantCulture
        Format = "yyyyMMdd"
        ValiditaInizio = Date.ParseExact("19000101", Format, Provider)
        ValiditaFine = Date.ParseExact("21001231", Format, Provider)
    End Sub

#End Region
    Private _format As String
    Public Shadows Property Format() As String
        Get
            Return _format
        End Get
        Set
            _format = Value
        End Set
    End Property

    Private _provider As Globalization.CultureInfo
    Public Shadows Property Provider() As Globalization.CultureInfo
        Get
            Return _provider
        End Get
        Set
            _provider = Value
        End Set
    End Property


    Private _validitaInizio As Date
    Public Shadows Property ValiditaInizio() As Date
        Get
            Return _validitaInizio
        End Get
        Set
            _validitaInizio = Value
        End Set
    End Property

    Private _validitaFine As Date
    Public Shadows Property ValiditaFine() As Date
        Get
            Return _validitaFine
        End Get
        Set
            _validitaFine = Value
        End Set
    End Property

    Public Function Aggiorna_jDeereDataModel_Files(
            ByVal righeInserite As String,
            ByVal righeModificate As String,
            ByVal righeCancellate As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String


        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty

        Dim NomeRoutine As String = "jDeereDataModel_Files_W.Aggiorna_jDeereDataModel_Files()"

        Try
            Dim campConf_R As New jDeereDataModelDAL_Files_R

            Dim curjDeereDataModel_Files As New jDeereDataModel_Files

            Dim righeInseriteArray As JArray = JArray.Parse(righeInserite)
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim EFArrayToInsert As New ArrayList
            Dim EFArrayToUpdate As New ArrayList
            Dim EFArrayToDelete As New ArrayList
            Dim IdCurr As Integer = 0
            Dim idCurrViaGUID As Integer = -1

            For Each obj As JObject In righeInseriteArray
                curjDeereDataModel_Files = New jDeereDataModel_Files
                curjDeereDataModel_Files.ID = 0
                curjDeereDataModel_Files.Name = obj("Name")
                curjDeereDataModel_Files.delayProcessing = obj("delayProcessing")
                curjDeereDataModel_Files.guid = obj("guid")
                curjDeereDataModel_Files.OrganizationID = obj("OrganizationID")

                If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                    curjDeereDataModel_Files.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                Else
                    curjDeereDataModel_Files.Validita_Inizio = ValiditaInizio
                End If
                If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                    curjDeereDataModel_Files.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                Else
                    curjDeereDataModel_Files.Validita_Fine = ValiditaFine
                End If
                curjDeereDataModel_Files.Data_Creazione = Date.Now
                curjDeereDataModel_Files.Username_Creazione = objParametri.UsernameOperazione
                curjDeereDataModel_Files.Data_Modifica = Date.Now
                curjDeereDataModel_Files.Username_Modifica = objParametri.UsernameOperazione
                curjDeereDataModel_Files.inviato = 0

                EFArrayToInsert.Add(curjDeereDataModel_Files)
            Next

            For Each obj As JObject In righeModificateArray
                idCurrViaGUID = campConf_R.LeggiIDViaGUID(obj("ID"), objParametri)
                IdCurr = If(idCurrViaGUID <> -1, idCurrViaGUID, obj("ID"))
                curjDeereDataModel_Files = campConf_R.Leggi(IdCurr, objParametri)

                If curjDeereDataModel_Files Is Nothing Then
                    MessaggioErrore &= "Riga da aggiornare " & obj("Validita_Inizio").ToString & " - " & obj("Validita_Fine").ToString & " non trovata"
                Else

                    curjDeereDataModel_Files.Name = obj("Name")
                    curjDeereDataModel_Files.delayProcessing = obj("delayProcessing")
                    curjDeereDataModel_Files.StatoInvio = obj("StatoInvio")
                    curjDeereDataModel_Files.EsitoInvio = obj("EsitoInvio")
                    curjDeereDataModel_Files.guid = obj("guid")


                    If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                        curjDeereDataModel_Files.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                    Else
                        curjDeereDataModel_Files.Validita_Fine = ValiditaFine
                    End If
                    If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                        curjDeereDataModel_Files.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                    Else
                        curjDeereDataModel_Files.Validita_Fine = ValiditaFine
                    End If
                    curjDeereDataModel_Files.Data_Modifica = Date.Now
                    curjDeereDataModel_Files.Username_Modifica = objParametri.UsernameOperazione
                    EFArrayToUpdate.Add(curjDeereDataModel_Files)

                End If
            Next
            For Each obj As JObject In righeCancellateArray
                IdCurr = campConf_R.LeggiIDViaGUID(obj("ID"), objParametri)
                curjDeereDataModel_Files = New jDeereDataModel_Files
                curjDeereDataModel_Files.ID = IdCurr
                EFArrayToDelete.Add(curjDeereDataModel_Files)
            Next


            If String.IsNullOrEmpty(MessaggioErrore) Then
                Dim campConf_W As New jDeereDataModelDAL_Files_W

                MessaggioErrore = campConf_W.Aggiorna_jDeereDataModel_Files(
                      EFArrayToInsert,
                      EFArrayToUpdate,
                      EFArrayToDelete,
                      objParametri
                 )

            End If

        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
        Finally

        End Try


        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return MessaggioErrore
    End Function

End Class
