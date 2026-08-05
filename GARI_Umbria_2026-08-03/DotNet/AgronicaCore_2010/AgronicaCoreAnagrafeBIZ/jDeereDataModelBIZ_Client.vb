Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreAnagrafeDAL
Imports Newtonsoft.Json.Linq
Public Class jDeereDataModelBIZ_Client_W
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


    Public Function Aggiorna_jDeereDataModel_Client(
            ByVal righeInserite As String,
            ByVal righeModificate As String,
            ByVal righeCancellate As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String


        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty

        Dim NomeRoutine As String = "jDeereDataModel_Client_W.Aggiorna_jDeereDataModel_Client()"

        Try
            Dim campConf_R As New jDeereDataModelDAL_Client_R

            Dim curjDeereDataModel_Client As New jDeereDataModel_Client

            Dim righeInseriteArray As JArray = JArray.Parse(righeInserite)
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim EFArrayToInsert As New ArrayList
            Dim EFArrayToUpdate As New ArrayList
            Dim EFArrayToDelete As New ArrayList
            Dim IdCurr As Integer = 0
            For Each obj As JObject In righeInseriteArray
                curjDeereDataModel_Client = New jDeereDataModel_Client
                curjDeereDataModel_Client.ID = 0
                curjDeereDataModel_Client.Name = obj("Name")
                curjDeereDataModel_Client.guid = obj("ID")
                curjDeereDataModel_Client.OrganizationID = obj("OrganizationID")


                If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                    curjDeereDataModel_Client.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                Else
                    curjDeereDataModel_Client.Validita_Inizio = ValiditaInizio
                End If
                If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                    curjDeereDataModel_Client.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                Else
                    curjDeereDataModel_Client.Validita_Fine = ValiditaFine
                End If
                curjDeereDataModel_Client.Data_Creazione = Date.Now
                curjDeereDataModel_Client.Username_Creazione = objParametri.UsernameOperazione
                curjDeereDataModel_Client.Data_Modifica = Date.Now
                curjDeereDataModel_Client.Username_Modifica = objParametri.UsernameOperazione
                curjDeereDataModel_Client.inviato = 0

                EFArrayToInsert.Add(curjDeereDataModel_Client)
            Next
            For Each obj As JObject In righeModificateArray
                IdCurr = campConf_R.LeggiIDViaGUID(obj("ID"), objParametri)
                curjDeereDataModel_Client = campConf_R.Leggi(IdCurr, 0, objParametri)

                If curjDeereDataModel_Client Is Nothing Then
                    MessaggioErrore &= "Riga da aggiornare " & obj("Validita_Inizio").ToString & " - " & obj("Validita_Fine").ToString & " non trovata"
                Else

                    curjDeereDataModel_Client.Name = obj("Name")
                    curjDeereDataModel_Client.guid = obj("ID")

                    If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                        curjDeereDataModel_Client.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                    Else
                        curjDeereDataModel_Client.Validita_Fine = ValiditaFine
                    End If
                    If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                        curjDeereDataModel_Client.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                    Else
                        curjDeereDataModel_Client.Validita_Fine = ValiditaFine
                    End If
                    curjDeereDataModel_Client.Data_Modifica = Date.Now
                    curjDeereDataModel_Client.Username_Modifica = objParametri.UsernameOperazione
                    EFArrayToUpdate.Add(curjDeereDataModel_Client)

                End If
            Next
            For Each obj As JObject In righeCancellateArray
                IdCurr = campConf_R.LeggiIDViaGUID(obj("ID"), objParametri)
                curjDeereDataModel_Client = New jDeereDataModel_Client
                curjDeereDataModel_Client.ID = IdCurr
                EFArrayToDelete.Add(curjDeereDataModel_Client)
            Next


            If String.IsNullOrEmpty(MessaggioErrore) Then
                Dim campConf_W As New jDeereDataModelDAL_Client_W

                MessaggioErrore = campConf_W.Aggiorna_jDeereDataModel_Client(
                      EFArrayToInsert,
                      EFArrayToUpdate,
                      EFArrayToDelete,
                      objParametri
                 )

            End If

        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
        End Try


        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return MessaggioErrore
    End Function
End Class
