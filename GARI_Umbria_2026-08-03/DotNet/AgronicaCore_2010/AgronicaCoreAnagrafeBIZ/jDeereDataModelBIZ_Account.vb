

Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreAnagrafeDAL
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json

Public Class jDeereDataModelBIZ_Account_W
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


    Public Function Aggiorna_jDeereDataModelBIZ_Account(
            ByVal righeInserite As String,
            ByVal righeModificate As String,
            ByVal righeCancellate As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String


        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty

        Dim NomeRoutine As String = "jDeereDataModelBIZ_Account_W.Aggiorna_jDeereDataModelBIZ_Account_W()"

        Try
            Dim campConf_R As New jDeereDataModelDAL_Account_R

            Dim curjDeereDataModelBIZ_Account As New jDeereDataModel_Account

            Dim righeInseriteArray As JArray = JArray.Parse(righeInserite)
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim EFArrayToInsert As New ArrayList
            Dim EFArrayToUpdate As New ArrayList
            Dim EFArrayToDelete As New ArrayList
            For Each obj As JObject In righeInseriteArray
                curjDeereDataModelBIZ_Account = JsonConvert.DeserializeObject(Of jDeereDataModel_Account)(obj.ToString())

                If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                    curjDeereDataModelBIZ_Account.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                Else
                    curjDeereDataModelBIZ_Account.Validita_Inizio = ValiditaInizio
                End If
                If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                    curjDeereDataModelBIZ_Account.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                Else
                    curjDeereDataModelBIZ_Account.Validita_Fine = ValiditaFine
                End If
                curjDeereDataModelBIZ_Account.Data_Creazione = Date.Now
                curjDeereDataModelBIZ_Account.Username_Creazione = objParametri.UsernameOperazione
                curjDeereDataModelBIZ_Account.Data_Modifica = Date.Now
                curjDeereDataModelBIZ_Account.Username_Modifica = objParametri.UsernameOperazione
                curjDeereDataModelBIZ_Account.inviato = 0

                EFArrayToInsert.Add(curjDeereDataModelBIZ_Account)
            Next
            For Each obj As JObject In righeModificateArray
                curjDeereDataModelBIZ_Account = campConf_R.Leggi(obj("ID"), objParametri)
                If curjDeereDataModelBIZ_Account Is Nothing Then
                    MessaggioErrore += "Riga da aggiornare " & obj("Validita_Inizio").ToString & " - " & obj("Validita_Fine").ToString & " non trovata"
                Else

                    curjDeereDataModelBIZ_Account.Name = obj("Name")

                    If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                        curjDeereDataModelBIZ_Account.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                    Else
                        curjDeereDataModelBIZ_Account.Validita_Fine = ValiditaFine
                    End If
                    If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                        curjDeereDataModelBIZ_Account.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                    Else
                        curjDeereDataModelBIZ_Account.Validita_Fine = ValiditaFine
                    End If
                    curjDeereDataModelBIZ_Account.Data_Modifica = Date.Now
                    curjDeereDataModelBIZ_Account.Username_Modifica = objParametri.UsernameOperazione
                    EFArrayToUpdate.Add(curjDeereDataModelBIZ_Account)

                End If
            Next
            For Each obj As JObject In righeCancellateArray
                curjDeereDataModelBIZ_Account = New jDeereDataModel_Account
                curjDeereDataModelBIZ_Account.ID = obj("ID")
                EFArrayToDelete.Add(curjDeereDataModelBIZ_Account)
            Next


            If String.IsNullOrEmpty(MessaggioErrore) Then
                Dim campConf_W As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Account_W

                MessaggioErrore = campConf_W.Aggiorna_jDeereDataModelDAL_Account(
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