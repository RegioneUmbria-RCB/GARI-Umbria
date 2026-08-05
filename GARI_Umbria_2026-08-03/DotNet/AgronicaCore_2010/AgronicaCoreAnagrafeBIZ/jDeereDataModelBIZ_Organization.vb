Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class jDeereDataModelBIZ_Organization_W
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


    Public Function Aggiorna_jDeereDataModelBIZ_Organization(
            ByVal righeInserite As String,
            ByVal righeModificate As String,
            ByVal righeCancellate As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String


        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty

        Dim NomeRoutine As String = "jDeereDataModelBIZ_Organization_W.Aggiorna_jDeereDataModelBIZ_Organization_W()"

        Try
            Dim jDRead As New jDeereDataModelDAL_Organization_R

            Dim curjDeereDataModelBIZ_Organization As New jDeereDataModel_Organization

            Dim righeInseriteArray As JArray = JArray.Parse(righeInserite)
            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)
            Dim righeCancellateArray As JArray = JArray.Parse(righeCancellate)
            Dim EFArrayToInsert As New ArrayList
            Dim EFArrayToUpdate As New ArrayList
            Dim EFArrayToDelete As New ArrayList
            For Each obj As JObject In righeInseriteArray
                curjDeereDataModelBIZ_Organization = JsonConvert.DeserializeObject(Of jDeereDataModel_Organization)(obj.ToString())
                curjDeereDataModelBIZ_Organization.guid = curjDeereDataModelBIZ_Organization.ID.ToString()
                If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                    curjDeereDataModelBIZ_Organization.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                Else
                    curjDeereDataModelBIZ_Organization.Validita_Inizio = ValiditaInizio
                End If
                If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                    curjDeereDataModelBIZ_Organization.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                Else
                    curjDeereDataModelBIZ_Organization.Validita_Fine = ValiditaFine
                End If
                curjDeereDataModelBIZ_Organization.Data_Creazione = Date.Now
                curjDeereDataModelBIZ_Organization.Username_Creazione = objParametri.UsernameOperazione
                curjDeereDataModelBIZ_Organization.Data_Modifica = Date.Now
                curjDeereDataModelBIZ_Organization.Username_Modifica = objParametri.UsernameOperazione
                curjDeereDataModelBIZ_Organization.inviato = 0

                EFArrayToInsert.Add(curjDeereDataModelBIZ_Organization)
            Next

            For Each obj As JObject In righeModificateArray
                curjDeereDataModelBIZ_Organization = jDRead.LeggiViaGUID(obj("ID").ToString(), objParametri)

                If curjDeereDataModelBIZ_Organization Is Nothing Then
                    MessaggioErrore &= "Riga da aggiornare " & obj("Validita_Inizio").ToString & " - " & obj("Validita_Fine").ToString & " non trovata"
                Else

                    curjDeereDataModelBIZ_Organization.Name = obj("Name")
                    curjDeereDataModelBIZ_Organization.internal = obj("internal")
                    curjDeereDataModelBIZ_Organization.member = obj("member")


                    If Not String.IsNullOrEmpty(obj("Validita_Inizio")) Then
                        curjDeereDataModelBIZ_Organization.Validita_Inizio = Date.ParseExact(obj("Validita_Inizio"), Format, Provider)
                    Else
                        curjDeereDataModelBIZ_Organization.Validita_Fine = ValiditaFine
                    End If
                    If Not String.IsNullOrEmpty(obj("Validita_Fine")) Then
                        curjDeereDataModelBIZ_Organization.Validita_Fine = Date.ParseExact(obj("Validita_Fine"), Format, Provider)
                    Else
                        curjDeereDataModelBIZ_Organization.Validita_Fine = ValiditaFine
                    End If
                    curjDeereDataModelBIZ_Organization.Data_Modifica = Date.Now
                    curjDeereDataModelBIZ_Organization.Username_Modifica = objParametri.UsernameOperazione
                    EFArrayToUpdate.Add(curjDeereDataModelBIZ_Organization)

                End If
            Next
            For Each obj As JObject In righeCancellateArray
                curjDeereDataModelBIZ_Organization = New jDeereDataModel_Organization
                curjDeereDataModelBIZ_Organization.ID = obj("ID")
                EFArrayToDelete.Add(curjDeereDataModelBIZ_Organization)
            Next


            If String.IsNullOrEmpty(MessaggioErrore) Then
                Dim campConf_W As New jDeereDataModeldal_Organization_W

                MessaggioErrore = campConf_W.Aggiorna_jDeereDataModel_Organization(
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