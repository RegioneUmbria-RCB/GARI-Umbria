Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class jDeereDataModelBIZ_FieldOperationFileRequest_W
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

    Public Function Aggiorna_jDeereDataModelBIZ_FieldOperationFileRequest(
            ByVal righeInserite As String,
            ByVal righeModificate As String,
            ByVal righeCancellate As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String


        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty

        Dim NomeRoutine As String = "jDeereDataModelBIZ_FieldOperationFileRequest_W.Aggiorna_jDeereDataModelBIZ_FieldOperationFileRequest()"

        Try
            Dim jDRead As New jDeereDataModelDAL_FieldOperationFileRequest_R

            Dim curjDeereDataModelBIZ_FieldOperationFileRequest As New jDeereDataModel_FieldOperationFileRequest

            Dim righeInseriteArray As JArray = JArray.Parse(righeInserite)
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim EFArrayToInsert As New ArrayList
            Dim EFArrayToUpdate As New ArrayList
            Dim EFArrayToDelete As New ArrayList

            For Each obj As JObject In righeInseriteArray
                curjDeereDataModelBIZ_FieldOperationFileRequest = JsonConvert.DeserializeObject(Of jDeereDataModel_FieldOperationFileRequest)(obj.ToString())

                curjDeereDataModelBIZ_FieldOperationFileRequest.FieldOperationID = obj("FieldOperationID")
                curjDeereDataModelBIZ_FieldOperationFileRequest.FieldOperationGUID = obj("FieldOperationGUID")

                If obj("Allegati_Documenti_Cod") IsNot Nothing Then
                    curjDeereDataModelBIZ_FieldOperationFileRequest.Allegati_Documenti_Cod = Integer.Parse(obj("Allegati_Documenti_Cod"))
                End If

                If obj("RequestState") Is Nothing Then
                    curjDeereDataModelBIZ_FieldOperationFileRequest.RequestState = 0
                Else
                    curjDeereDataModelBIZ_FieldOperationFileRequest.RequestState = Integer.Parse(obj("RequestState"))
                End If

                If obj("OAuth_Token") IsNot Nothing Then
                    curjDeereDataModelBIZ_FieldOperationFileRequest.OAuth_Token = obj("OAuth_Token")
                End If

                If obj("Request_Parameters") IsNot Nothing Then
                    curjDeereDataModelBIZ_FieldOperationFileRequest.Request_Parameters = obj("Request_Parameters")
                End If

                If obj("Esito") IsNot Nothing Then
                    curjDeereDataModelBIZ_FieldOperationFileRequest.Esito = obj("Esito")
                End If

                If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                    curjDeereDataModelBIZ_FieldOperationFileRequest.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                Else
                    curjDeereDataModelBIZ_FieldOperationFileRequest.Validita_Inizio = ValiditaInizio
                End If
                If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                    curjDeereDataModelBIZ_FieldOperationFileRequest.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                Else
                    curjDeereDataModelBIZ_FieldOperationFileRequest.Validita_Fine = ValiditaFine
                End If
                curjDeereDataModelBIZ_FieldOperationFileRequest.Data_Creazione = Date.Now
                curjDeereDataModelBIZ_FieldOperationFileRequest.Username_Creazione = objParametri.UsernameOperazione
                curjDeereDataModelBIZ_FieldOperationFileRequest.Data_Modifica = Date.Now
                curjDeereDataModelBIZ_FieldOperationFileRequest.Username_Modifica = objParametri.UsernameOperazione
                curjDeereDataModelBIZ_FieldOperationFileRequest.inviato = 0

                EFArrayToInsert.Add(curjDeereDataModelBIZ_FieldOperationFileRequest)
            Next

            For Each obj As JObject In righeModificateArray
                curjDeereDataModelBIZ_FieldOperationFileRequest = jDRead.Leggi(Integer.Parse(obj("ID")), objParametri)

                If curjDeereDataModelBIZ_FieldOperationFileRequest Is Nothing Then
                    MessaggioErrore &= "Riga da aggiornare " & obj("Validita_Inizio").ToString & " - " & obj("Validita_Fine").ToString & " non trovata"
                Else

                    curjDeereDataModelBIZ_FieldOperationFileRequest.FieldOperationID = obj("FieldOperationID")
                    curjDeereDataModelBIZ_FieldOperationFileRequest.FieldOperationGUID = obj("FieldOperationGUID")

                    If obj("Allegati_Documenti_Cod") IsNot Nothing Then
                        curjDeereDataModelBIZ_FieldOperationFileRequest.Allegati_Documenti_Cod = Integer.Parse(obj("Allegati_Documenti_Cod"))
                    End If

                    If obj("RequestState") Is Nothing Then
                        curjDeereDataModelBIZ_FieldOperationFileRequest.RequestState = 0
                    Else
                        curjDeereDataModelBIZ_FieldOperationFileRequest.RequestState = Integer.Parse(obj("RequestState"))
                    End If

                    If obj("OAuth_Token") IsNot Nothing Then
                        curjDeereDataModelBIZ_FieldOperationFileRequest.OAuth_Token = obj("OAuth_Token")
                    End If

                    If obj("Request_Parameters") IsNot Nothing Then
                        curjDeereDataModelBIZ_FieldOperationFileRequest.Request_Parameters = obj("Request_Parameters")
                    End If

                    If obj("Esito") IsNot Nothing Then
                        curjDeereDataModelBIZ_FieldOperationFileRequest.Esito = obj("Esito")
                    End If


                    If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                        curjDeereDataModelBIZ_FieldOperationFileRequest.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                    Else
                        curjDeereDataModelBIZ_FieldOperationFileRequest.Validita_Fine = ValiditaFine
                    End If
                    If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                        curjDeereDataModelBIZ_FieldOperationFileRequest.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                    Else
                        curjDeereDataModelBIZ_FieldOperationFileRequest.Validita_Fine = ValiditaFine
                    End If
                    curjDeereDataModelBIZ_FieldOperationFileRequest.Data_Modifica = Date.Now
                    curjDeereDataModelBIZ_FieldOperationFileRequest.Username_Modifica = objParametri.UsernameOperazione
                    EFArrayToUpdate.Add(curjDeereDataModelBIZ_FieldOperationFileRequest)

                End If
            Next
            For Each obj As JObject In righeCancellateArray
                curjDeereDataModelBIZ_FieldOperationFileRequest = New jDeereDataModel_FieldOperationFileRequest
                curjDeereDataModelBIZ_FieldOperationFileRequest.ID = obj("ID")
                EFArrayToDelete.Add(curjDeereDataModelBIZ_FieldOperationFileRequest)
            Next


            If String.IsNullOrEmpty(MessaggioErrore) Then
                Dim campConf_W As New jDeereDataModelDAL_FieldOperationFileRequest_W

                MessaggioErrore = campConf_W.Aggiorna_jDeereDataModel_FieldOperationFileRequest(
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
