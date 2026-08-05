

Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreAnagrafeDAL
Imports Newtonsoft.Json.Linq

Public Class jDeereDataModelBIZ_AccountXOrganization_W
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


    Public Function Aggiorna_jDeereDataModelBIZ_AccountXOrganization(
            ByVal righeInserite As String,
            ByVal righeModificate As String,
            ByVal righeCancellate As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String


        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty

        Dim NomeRoutine As String = "jDeereDataModelBIZ_AccountXOrganization_W.Aggiorna_jDeereDataModelBIZ_AccountXOrganization_W()"

        Try
            Dim campConf_R As New jDeereDataModelDAL_AccountXOrganization_R

            Dim curjDeereDataModelBIZ_AccountXOrganization As New jDeereDataModel_AccountXOrganization

            Dim righeInseriteArray As JArray = JArray.Parse(righeInserite)
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim EFArrayToInsert As New ArrayList
            Dim EFArrayToUpdate As New ArrayList
            Dim EFArrayToDelete As New ArrayList
            For Each obj As JObject In righeInseriteArray
                curjDeereDataModelBIZ_AccountXOrganization = New jDeereDataModel_AccountXOrganization
                curjDeereDataModelBIZ_AccountXOrganization.IDOrganization = obj("IDOrganization")
                curjDeereDataModelBIZ_AccountXOrganization.IDAccount = obj("IDAccount")

                If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                    curjDeereDataModelBIZ_AccountXOrganization.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                Else
                    curjDeereDataModelBIZ_AccountXOrganization.Validita_Inizio = ValiditaInizio
                End If
                If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                    curjDeereDataModelBIZ_AccountXOrganization.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                Else
                    curjDeereDataModelBIZ_AccountXOrganization.Validita_Fine = ValiditaFine
                End If
                curjDeereDataModelBIZ_AccountXOrganization.Data_Creazione = Date.Now
                curjDeereDataModelBIZ_AccountXOrganization.Username_Creazione = objParametri.UsernameOperazione
                curjDeereDataModelBIZ_AccountXOrganization.Data_Modifica = Date.Now
                curjDeereDataModelBIZ_AccountXOrganization.Username_Modifica = objParametri.UsernameOperazione
                curjDeereDataModelBIZ_AccountXOrganization.inviato = 0

                EFArrayToInsert.Add(curjDeereDataModelBIZ_AccountXOrganization)
            Next
            For Each obj As JObject In righeModificateArray
                curjDeereDataModelBIZ_AccountXOrganization = campConf_R.Leggi(obj("Username"), obj("IDOrganization"), objParametri)
                If curjDeereDataModelBIZ_AccountXOrganization Is Nothing Then
                    MessaggioErrore += "Riga da aggiornare " & obj("Validita_Inizio").ToString & " - " & obj("Validita_Fine").ToString & " non trovata"
                Else

                    If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                        curjDeereDataModelBIZ_AccountXOrganization.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                    Else
                        curjDeereDataModelBIZ_AccountXOrganization.Validita_Fine = ValiditaFine
                    End If
                    If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                        curjDeereDataModelBIZ_AccountXOrganization.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                    Else
                        curjDeereDataModelBIZ_AccountXOrganization.Validita_Fine = ValiditaFine
                    End If
                    curjDeereDataModelBIZ_AccountXOrganization.Data_Modifica = Date.Now
                    curjDeereDataModelBIZ_AccountXOrganization.Username_Modifica = objParametri.UsernameOperazione
                    EFArrayToUpdate.Add(curjDeereDataModelBIZ_AccountXOrganization)

                End If
            Next
            For Each obj As JObject In righeCancellateArray
                curjDeereDataModelBIZ_AccountXOrganization = New jDeereDataModel_AccountXOrganization
                curjDeereDataModelBIZ_AccountXOrganization.IDOrganization = obj("IDOrganization")
                curjDeereDataModelBIZ_AccountXOrganization.IDAccount = obj("IDAccount")
                EFArrayToDelete.Add(curjDeereDataModelBIZ_AccountXOrganization)
            Next


            If String.IsNullOrEmpty(MessaggioErrore) Then
                Dim campConf_W As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_AccountXOrganization_W

                MessaggioErrore = campConf_W.Aggiorna_jDeereDataModelDAL_AccountXOrganization(
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