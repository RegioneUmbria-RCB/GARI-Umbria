Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreAnagrafeDAL
Imports Newtonsoft.Json.Linq

Public Class jDeereDataModelBIZ_FieldXBoundary_W
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


    Public Function Aggiorna_jDeereDataModelBIZ_FieldXBoundary(
            ByVal righeInserite As String,
            ByVal righeModificate As String,
            ByVal righeCancellate As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String


        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty

        Dim NomeRoutine As String = "jDeereDataModelBIZ_FieldXBoundary_W.jDeereDataModelBIZ_FieldXBoundary()"

        Try
            Dim campConf_R As New jDeereDataModelDAL_FieldXBoundary_R

            Dim curjDeereDataModelBIZ_FieldXBoundary As New jDeereDataModel_FieldXBoundary

            Dim righeInseriteArray As JArray = JArray.Parse(righeInserite)
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim EFArrayToInsert As New ArrayList
            Dim EFArrayToUpdate As New ArrayList
            Dim EFArrayToDelete As New ArrayList
            For Each obj As JObject In righeInseriteArray
                curjDeereDataModelBIZ_FieldXBoundary = New jDeereDataModel_FieldXBoundary
                curjDeereDataModelBIZ_FieldXBoundary.IDField = obj("IDField")
                curjDeereDataModelBIZ_FieldXBoundary.IDBoundary = obj("IDBoundary")

                If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                    curjDeereDataModelBIZ_FieldXBoundary.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                Else
                    curjDeereDataModelBIZ_FieldXBoundary.Validita_Inizio = ValiditaInizio
                End If
                If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                    curjDeereDataModelBIZ_FieldXBoundary.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                Else
                    curjDeereDataModelBIZ_FieldXBoundary.Validita_Fine = ValiditaFine
                End If
                curjDeereDataModelBIZ_FieldXBoundary.Data_Creazione = Date.Now
                curjDeereDataModelBIZ_FieldXBoundary.Username_Creazione = objParametri.UsernameOperazione
                curjDeereDataModelBIZ_FieldXBoundary.Data_Modifica = Date.Now
                curjDeereDataModelBIZ_FieldXBoundary.Username_Modifica = objParametri.UsernameOperazione
                curjDeereDataModelBIZ_FieldXBoundary.inviato = 0

                EFArrayToInsert.Add(curjDeereDataModelBIZ_FieldXBoundary)
            Next
            For Each obj As JObject In righeModificateArray
                curjDeereDataModelBIZ_FieldXBoundary = campConf_R.Leggi(obj("IDField"), obj("IDBoundary"), objParametri)
                If curjDeereDataModelBIZ_FieldXBoundary Is Nothing Then
                    MessaggioErrore += "Riga da aggiornare " & obj("Validita_Inizio").ToString & " - " & obj("Validita_Fine").ToString & " non trovata"
                Else

                    If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                        curjDeereDataModelBIZ_FieldXBoundary.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                    Else
                        curjDeereDataModelBIZ_FieldXBoundary.Validita_Fine = ValiditaFine
                    End If
                    If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                        curjDeereDataModelBIZ_FieldXBoundary.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                    Else
                        curjDeereDataModelBIZ_FieldXBoundary.Validita_Fine = ValiditaFine
                    End If
                    curjDeereDataModelBIZ_FieldXBoundary.Data_Modifica = Date.Now
                    curjDeereDataModelBIZ_FieldXBoundary.Username_Modifica = objParametri.UsernameOperazione
                    EFArrayToUpdate.Add(curjDeereDataModelBIZ_FieldXBoundary)

                End If
            Next
            For Each obj As JObject In righeCancellateArray
                curjDeereDataModelBIZ_FieldXBoundary = New jDeereDataModel_FieldXBoundary
                curjDeereDataModelBIZ_FieldXBoundary.IDField = obj("IDField")
                curjDeereDataModelBIZ_FieldXBoundary.IDBoundary = obj("IDBoundary")
                EFArrayToDelete.Add(curjDeereDataModelBIZ_FieldXBoundary)
            Next


            If String.IsNullOrEmpty(MessaggioErrore) Then
                Dim campConf_W As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_FieldXBoundary_W

                MessaggioErrore = campConf_W.Aggiorna_jDeereDataModelDAL_FieldXBoundary(
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